using System.Globalization;
using LanguageCenter.Application.Abstractions;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace LanguageCenter.Infrastructure.Persistence;

/// <summary>
/// Tạo tài khoản kèm hồ sơ (Học viên / Giáo viên / Trợ giảng) trong MỘT transaction.
/// </summary>
/// <remarks>
/// <para><b>Sinh mã concurrency-safe — không dùng <c>Count() + 1</c>:</b></para>
/// <list type="number">
///   <item>Mở transaction.</item>
///   <item>Tính mã kế tiếp bằng <c>SELECT TOP 1 MaHocVien ORDER BY MaHocVien DESC</c> (chạy ở SQL Server,
///         chỉ lấy đúng 1 giá trị, không load danh sách).</item>
///   <item>INSERT <c>TaiKhoan</c> + hồ sơ (mã = TenDangNhap = MaHocVien).</item>
///   <item>COMMIT. Nếu vi phạm UNIQUE (SQL 2601/2627) do request đồng thời → ROLLBACK, xoá change tracker
///         và THỬ LẠI với mã kế tiếp (tối đa 5 lần).</item>
/// </list>
/// <para>
/// Chính ràng buộc UNIQUE của database là "trọng tài" — nhờ vậy không cần Sequence, không cần bảng mới,
/// không thay đổi schema, và không phụ thuộc vào tên index (tên UNIQUE constraint do SQL Server tự sinh
/// nên KHÔNG dùng để nhận diện lỗi).
/// </para>
/// </remarks>
public sealed class AccountProvisioningStore : IAccountProvisioningStore
{
    private const int MaxAttempts = 5;

    private const string StudentPrefix = "HV";
    private const string TeacherPrefix = "GV";
    private const string AssistantPrefix = "TG";

    private readonly AppDbContext _dbContext;

    public AccountProvisioningStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<ProvisionResult> CreateStudentAsync(
        StudentProvisionData data,
        string matKhauHash,
        CancellationToken cancellationToken)
        => ProvisionAsync(
            StudentPrefix,
            data.VaiTro,
            data.Email,
            cancellationToken,
            (code, maVaiTro) => StageStudent(code, maVaiTro, data, matKhauHash));

    public Task<ProvisionResult> CreateTeacherAsync(
        TeacherProvisionData data,
        string matKhauHash,
        CancellationToken cancellationToken)
    {
        var prefix = data.LoaiGiaoVien == TeacherTypes.TroGiang ? AssistantPrefix : TeacherPrefix;

        return ProvisionAsync(
            prefix,
            data.VaiTro,
            data.Email,
            cancellationToken,
            (code, maVaiTro) => StageTeacher(code, maVaiTro, data, matKhauHash));
    }

    // ========================================================================
    // CORE
    // ========================================================================

    private async Task<ProvisionResult> ProvisionAsync(
        string prefix,
        string vaiTro,
        string email,
        CancellationToken cancellationToken,
        Action<string, int> stageEntities)
    {
        // Phase 1 đã bật EnableRetryOnFailure() -> EF Core BẮT BUỘC mọi transaction do người dùng
        // khởi tạo phải được bao trong ExecutionStrategy của chính nó.
        // Nhờ vậy vẫn giữ được khả năng tự retry khi lỗi transient (mất kết nối, deadlock...).
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(
            state: (prefix, vaiTro, email, stageEntities),
            operation: (_, state, token) => ProvisionCoreAsync(state.prefix, state.vaiTro, state.email, token, state.stageEntities),
            verifySucceeded: null,
            cancellationToken: cancellationToken);
    }

    private async Task<ProvisionResult> ProvisionCoreAsync(
        string prefix,
        string vaiTro,
        string email,
        CancellationToken cancellationToken,
        Action<string, int> stageEntities)
    {
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var maVaiTro = await FindRoleIdAsync(vaiTro, cancellationToken);

                if (maVaiTro is null)
                {
                    await SafeRollbackAsync(transaction);
                    return ProvisionResult.Failure(ProvisionFailureReason.RoleNotFound);
                }

                // Kiểm tra email NGAY TRONG transaction để thu hẹp cửa sổ race condition.
                // Sau đó ràng buộc UNIQUE của database vẫn là chốt cuối.
                if (await _dbContext.TaiKhoans.AnyAsync(taiKhoan => taiKhoan.Email == email, cancellationToken))
                {
                    await SafeRollbackAsync(transaction);
                    return ProvisionResult.Failure(ProvisionFailureReason.EmailAlreadyExists);
                }

                var code = await GenerateNextCodeAsync(prefix, cancellationToken);

                stageEntities(code, maVaiTro.Value);

                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return ProvisionResult.Success(code);
            }
            catch (Exception exception) when (IsUniqueViolation(exception))
            {
                await SafeRollbackAsync(transaction);
                _dbContext.ChangeTracker.Clear();

                // KHÔNG nhận diện theo tên index (tên do SQL Server tự sinh, khác nhau giữa các máy).
                // Kiểm tra lại email: nếu email đã tồn tại -> đây là lỗi trùng EMAIL, trả luôn.
                // Ngược lại -> trùng MÃ do request đồng thời, thử lại với mã kế tiếp.
                if (await IsEmailTakenAsync(email, cancellationToken))
                {
                    return ProvisionResult.Failure(ProvisionFailureReason.EmailAlreadyExists);
                }
            }
            catch
            {
                await SafeRollbackAsync(transaction);
                _dbContext.ChangeTracker.Clear();
                throw;
            }
            finally
            {
                await transaction.DisposeAsync();
            }
        }

        return ProvisionResult.Failure(ProvisionFailureReason.CodeGenerationFailed);
    }

    /// <summary>
    /// Sinh mã kế tiếp: <c>SELECT TOP 1 &lt;Mã&gt; ... ORDER BY &lt;Mã&gt; DESC</c>.
    /// Chỉ lấy ĐÚNG MỘT giá trị từ SQL Server — không load danh sách về memory.
    /// </summary>
    /// <remarks>
    /// Pattern <c>LIKE 'HV[0-9][0-9][0-9]'</c> đảm bảo chỉ so sánh các mã đúng 3 chữ số,
    /// nên so sánh CHUỖI cho kết quả trùng với so sánh SỐ (do đã zero-padding).
    /// </remarks>
    private async Task<string> GenerateNextCodeAsync(string prefix, CancellationToken cancellationToken)
    {
        var pattern = $"{prefix}[0-9][0-9][0-9]";

        var codes = prefix switch
        {
            StudentPrefix => _dbContext.HocViens
                .Where(hocVien => EF.Functions.Like(hocVien.MaHocVien, pattern))
                .Select(hocVien => hocVien.MaHocVien),

            _ => _dbContext.GiaoViens
                .Where(giaoVien => EF.Functions.Like(giaoVien.MaGiaoVien, pattern))
                .Select(giaoVien => giaoVien.MaGiaoVien)
        };

        var maxCode = await codes
            .OrderByDescending(code => code)
            .FirstOrDefaultAsync(cancellationToken);

        var nextNumber = maxCode is null ? 1 : int.Parse(maxCode.AsSpan(prefix.Length), CultureInfo.InvariantCulture) + 1;

        if (nextNumber > 999)
        {
            throw new InvalidOperationException(
                $"Đã vượt giới hạn mã {prefix}999. Cần thiết kế lại quy tắc sinh mã (ngoài phạm vi Phase 3).");
        }

        return prefix + nextNumber.ToString("D3", CultureInfo.InvariantCulture);
    }

    private async Task<int?> FindRoleIdAsync(string tenVaiTro, CancellationToken cancellationToken)
        => await _dbContext.VaiTros
            .Where(vaiTro => vaiTro.TenVaiTro == tenVaiTro)
            .Select(vaiTro => (int?)vaiTro.MaVaiTro)
            .FirstOrDefaultAsync(cancellationToken);

    private async Task<bool> IsEmailTakenAsync(string email, CancellationToken cancellationToken)
        => await _dbContext.TaiKhoans.AnyAsync(taiKhoan => taiKhoan.Email == email, cancellationToken);

    /// <summary>Lỗi vi phạm UNIQUE của SQL Server: 2601 (unique index) hoặc 2627 (unique constraint/PK).</summary>
    private static bool IsUniqueViolation(Exception exception)
    {
        var inner = exception is DbUpdateException dbUpdateException
            ? dbUpdateException.InnerException
            : exception;

        return inner is SqlException { Number: 2601 or 2627 };
    }

    private static async Task SafeRollbackAsync(IDbContextTransaction transaction)
    {
        try
        {
            await transaction.RollbackAsync();
        }
        catch
        {
            // Bỏ qua: transaction có thể đã bị SQL Server huỷ (deadlock, mất kết nối).
            // Việc dọn dẹp không được che lỗi gốc.
        }
    }

    // ========================================================================
    // STAGING
    // ========================================================================

    private void StageStudent(string code, int maVaiTro, StudentProvisionData data, string matKhauHash)
    {
        _dbContext.TaiKhoans.Add(new TaiKhoan
        {
            MaVaiTro = maVaiTro,
            TenDangNhap = code,
            MatKhauHash = matKhauHash,
            Email = data.Email,
            TrangThaiTaiKhoan = AccountStatuses.HoatDong
        });

        PhuHuynh? phuHuynh = null;

        if (data.PhuHuynh is { } parent)
        {
            phuHuynh = new PhuHuynh
            {
                HoTen = parent.HoTen,
                SoDienThoai = parent.SoDienThoai,
                Email = parent.Email,
                DiaChi = parent.DiaChi,
                NgheNghiep = parent.NgheNghiep
            };

            _dbContext.PhuHuynhs.Add(phuHuynh);
        }

        // MaHocVien == TenDangNhap (ràng buộc nghiệp vụ của hệ thống).
        // MaPhuHuynh và MoiQuanHePhuHuynh luôn được set cùng nhau để thoả CHECK CK_HocVien_PhuHuynh.
        _dbContext.HocViens.Add(new HocVien
        {
            MaHocVien = code,
            HoTen = data.HoTen,
            NgaySinh = data.NgaySinh,
            GioiTinh = data.GioiTinh,
            Email = data.Email,
            SoDienThoai = data.SoDienThoai,
            DiaChi = data.DiaChi,
            MucTieuHocTap = data.MucTieuHocTap,
            TrinhDoHienTai = data.TrinhDoHienTai,
            TrangThaiHocTap = data.TrangThaiHocTap,
            MoiQuanHePhuHuynh = data.PhuHuynh?.MoiQuanHe,
            PhuHuynh = phuHuynh
        });
    }

    private void StageTeacher(string code, int maVaiTro, TeacherProvisionData data, string matKhauHash)
    {
        _dbContext.TaiKhoans.Add(new TaiKhoan
        {
            MaVaiTro = maVaiTro,
            TenDangNhap = code,
            MatKhauHash = matKhauHash,
            Email = data.Email,
            TrangThaiTaiKhoan = AccountStatuses.HoatDong
        });

        // MaGiaoVien == TenDangNhap (GVxxx hoặc TGxxx tuỳ LoaiGiaoVien).
        _dbContext.GiaoViens.Add(new GiaoVien
        {
            MaGiaoVien = code,
            HoTen = data.HoTen,
            NgaySinh = data.NgaySinh,
            GioiTinh = data.GioiTinh,
            Email = data.Email,
            SoDienThoai = data.SoDienThoai,
            DiaChi = data.DiaChi,
            LoaiGiaoVien = data.LoaiGiaoVien,
            ChuyenMon = data.ChuyenMon,
            SoNamKinhNghiem = data.SoNamKinhNghiem,
            NgayVaoLam = data.NgayVaoLam,
            TrangThai = data.TrangThai
        });
    }
}
