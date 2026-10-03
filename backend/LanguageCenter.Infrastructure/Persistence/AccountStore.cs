using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Accounts.Models;
using LanguageCenter.Application.Common;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="IAccountStore"/>
public sealed class AccountStore : IAccountStore
{
    private readonly AppDbContext _dbContext;

    public AccountStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<AccountAuthInfo?> FindByUsernameAsync(string tenDangNhap, CancellationToken cancellationToken)
    {
        return await _dbContext.TaiKhoans
            .AsNoTracking()
            .Where(taiKhoan => taiKhoan.TenDangNhap == tenDangNhap)
            .Select(taiKhoan => new AccountAuthInfo(
                taiKhoan.MaTaiKhoan,
                taiKhoan.TenDangNhap,
                taiKhoan.Email,
                taiKhoan.TrangThaiTaiKhoan,
                taiKhoan.MatKhauHash,
                taiKhoan.VaiTro.TenVaiTro))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<AccountAuthInfo?> FindByIdAsync(int maTaiKhoan, CancellationToken cancellationToken)
    {
        return await _dbContext.TaiKhoans
            .AsNoTracking()
            .Where(taiKhoan => taiKhoan.MaTaiKhoan == maTaiKhoan)
            .Select(taiKhoan => new AccountAuthInfo(
                taiKhoan.MaTaiKhoan,
                taiKhoan.TenDangNhap,
                taiKhoan.Email,
                taiKhoan.TrangThaiTaiKhoan,
                taiKhoan.MatKhauHash,
                taiKhoan.VaiTro.TenVaiTro))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task TouchLastLoginAsync(int maTaiKhoan, CancellationToken cancellationToken)
    {
        // Dùng SYSDATETIME() của SQL Server để NHẤT QUÁN với mọi default timestamp khác
        // trong database (toàn bộ 32 bảng đều dùng SYSDATETIME()).
        // Câu lệnh được tham số hoá (interpolated -> parameter), không có rủi ro SQL injection.
        await _dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE TaiKhoan SET LanDangNhapCuoi = SYSDATETIME() WHERE MaTaiKhoan = {maTaiKhoan}",
            cancellationToken);
    }

    public async Task UpdatePasswordHashAsync(int maTaiKhoan, string matKhauHash, CancellationToken cancellationToken)
    {
        // Cập nhật cả NgayCapNhat, cùng dùng giờ của SQL Server.
        await _dbContext.Database.ExecuteSqlInterpolatedAsync(
            $"UPDATE TaiKhoan SET MatKhauHash = {matKhauHash}, NgayCapNhat = SYSDATETIME() WHERE MaTaiKhoan = {maTaiKhoan}",
            cancellationToken);
    }

    // ========================================================================
    // PHASE 3 — QUẢN TRỊ TÀI KHOẢN
    // ========================================================================

    public async Task<PagedResult<AccountListItemDto>> ListAsync(
        AccountListQuery query,
        CancellationToken cancellationToken)
    {
        var source = BuildAccountQuery();

        // Lọc ngay trong SQL — KHÔNG load toàn bộ rồi mới Where().
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            source = source.Where(row =>
                row.TenDangNhap.Contains(search)
                || row.Email.Contains(search)
                || (row.TenNhanVien != null && row.TenNhanVien.Contains(search))
                || (row.TenGiaoVien != null && row.TenGiaoVien.Contains(search))
                || (row.TenHocVien != null && row.TenHocVien.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(query.Role))
        {
            var role = query.Role.Trim();
            source = source.Where(row => row.TenVaiTro == role);
        }

        if (!string.IsNullOrWhiteSpace(query.Status))
        {
            var status = query.Status.Trim();
            source = source.Where(row => row.TrangThaiTaiKhoan == status);
        }

        // Count và Skip/Take đều thực hiện ở SQL Server.
        var totalItems = await source.CountAsync(cancellationToken);

        var rows = await source
            .OrderBy(row => row.MaTaiKhoan)
            .Skip(query.Skip)
            .Take(query.Take)
            .ToListAsync(cancellationToken);

        var items = rows
            .Select(row => new AccountListItemDto
            {
                Id = row.MaTaiKhoan,
                Username = row.TenDangNhap,
                FullName = row.HoTen ?? row.TenDangNhap,
                Email = row.Email,
                Role = row.TenVaiTro,
                Status = row.TrangThaiTaiKhoan,
                CreatedAt = row.NgayTao,
                LastLoginAt = row.LanDangNhapCuoi
            })
            .ToList();

        return new PagedResult<AccountListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<AccountDetailDto?> GetDetailAsync(int maTaiKhoan, CancellationToken cancellationToken)
    {
        var row = await BuildAccountQuery()
            .Where(item => item.MaTaiKhoan == maTaiKhoan)
            .FirstOrDefaultAsync(cancellationToken);

        if (row is null)
        {
            return null;
        }

        var profileType = row.TenVaiTro switch
        {
            RoleNames.HocVien => "HocVien",
            RoleNames.GiaoVien or RoleNames.TroGiang => "GiaoVien",
            _ => row.TenNhanVien is not null ? "NhanVien" : null
        };

        return new AccountDetailDto
        {
            Id = row.MaTaiKhoan,
            Username = row.TenDangNhap,
            FullName = row.HoTen ?? row.TenDangNhap,
            Email = row.Email,
            Role = row.TenVaiTro,
            RoleId = row.MaVaiTro,
            Status = row.TrangThaiTaiKhoan,
            CreatedAt = row.NgayTao,
            UpdatedAt = row.NgayCapNhat,
            LastLoginAt = row.LanDangNhapCuoi,
            ProfileType = profileType,
            ProfileCode = profileType is null ? null : row.TenDangNhap,
            TeacherType = row.LoaiGiaoVien,
            StudentStatus = row.TrangThaiHocTap
        };
    }

    public async Task<WriteOutcome> UpdateStatusAsync(
        int maTaiKhoan,
        string trangThai,
        CancellationToken cancellationToken)
    {
        var affected = await _dbContext.TaiKhoans
            .Where(taiKhoan => taiKhoan.MaTaiKhoan == maTaiKhoan)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(
                    taiKhoan => taiKhoan.TrangThaiTaiKhoan,
                    trangThai),
                cancellationToken);

        return affected == 0 ? WriteOutcome.NotFound : WriteOutcome.Success;
    }

    public async Task<bool> EmailExistsAsync(
        string email,
        string? exceptUsername,
        CancellationToken cancellationToken)
    {
        var query = _dbContext.TaiKhoans.Where(taiKhoan => taiKhoan.Email == email);

        if (!string.IsNullOrWhiteSpace(exceptUsername))
        {
            query = query.Where(taiKhoan => taiKhoan.TenDangNhap != exceptUsername);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<AccountDeleteCheck> CheckDeletableAsync(
        int maTaiKhoan,
        CancellationToken cancellationToken)
    {
        var account = await FindByIdAsync(maTaiKhoan, cancellationToken);

        if (account is null)
        {
            return AccountDeleteCheck.Allowed();
        }

        var username = account.TenDangNhap;

        // ---------- Dữ liệu phụ thuộc trực tiếp vào TaiKhoan ----------
        if (await _dbContext.DiemDanhs.AnyAsync(x => x.MaTaiKhoanThucHien == maTaiKhoan, cancellationToken))
        {
            return AccountDeleteCheck.Blocked("DiemDanh (lịch sử điểm danh do tài khoản này thực hiện)");
        }

        if (await _dbContext.ThongBaos.AnyAsync(
                x => x.MaTaiKhoanGui == maTaiKhoan || x.MaTaiKhoanNhan == maTaiKhoan,
                cancellationToken))
        {
            return AccountDeleteCheck.Blocked("ThongBao (thông báo đã gửi/nhận)");
        }

        // ---------- Dữ liệu phụ thuộc vào hồ sơ tương ứng ----------
        switch (account.TenVaiTro)
        {
            case RoleNames.HocVien:
            {
                if (await _dbContext.KiemTraDauVaos.AnyAsync(x => x.MaHocVien == username, cancellationToken))
                {
                    return AccountDeleteCheck.Blocked("KiemTraDauVao (kết quả kiểm tra đầu vào)");
                }

                if (await _dbContext.DangKyHocs.AnyAsync(x => x.MaHocVien == username, cancellationToken))
                {
                    return AccountDeleteCheck.Blocked("DangKyHoc (đăng ký học)");
                }

                if (await _dbContext.DiemDanhs.AnyAsync(x => x.MaHocVien == username, cancellationToken))
                {
                    return AccountDeleteCheck.Blocked("DiemDanh (điểm danh của học viên)");
                }

                if (await _dbContext.PhanHoiDichVus.AnyAsync(x => x.MaHocVien == username, cancellationToken))
                {
                    return AccountDeleteCheck.Blocked("PhanHoiDichVu (phản hồi dịch vụ)");
                }

                if (await _dbContext.PhanHoiKhaoSats.AnyAsync(x => x.MaHocVien == username, cancellationToken))
                {
                    return AccountDeleteCheck.Blocked("PhanHoiKhaoSat (phản hồi khảo sát)");
                }

                break;
            }

            case RoleNames.GiaoVien:
            case RoleNames.TroGiang:
            {
                if (await _dbContext.PhanCongGiangDays.AnyAsync(x => x.MaGiaoVien == username, cancellationToken))
                {
                    return AccountDeleteCheck.Blocked("PhanCongGiangDay (lịch sử phân công giảng dạy)");
                }

                if (await _dbContext.CaHocs.AnyAsync(
                        x => x.MaGiaoVienThucTe == username || x.MaTroGiangThucTe == username,
                        cancellationToken))
                {
                    return AccountDeleteCheck.Blocked("CaHoc (ca học đã dạy)");
                }

                if (await _dbContext.DiemSos.AnyAsync(x => x.MaGiaoVien == username, cancellationToken))
                {
                    return AccountDeleteCheck.Blocked("DiemSo (điểm đã chấm)");
                }

                // BangCap / ChungChi có ON DELETE CASCADE trong database nên không cần chặn.
                break;
            }

            default:
            {
                if (await _dbContext.LopHocs.AnyAsync(x => x.MaNhanVienHocVu == username, cancellationToken))
                {
                    return AccountDeleteCheck.Blocked("LopHoc (lớp học do nhân viên phụ trách)");
                }

                if (await _dbContext.DangKyHocs.AnyAsync(x => x.MaNhanVienTuVan == username, cancellationToken))
                {
                    return AccountDeleteCheck.Blocked("DangKyHoc (đăng ký do nhân viên tư vấn)");
                }

                if (await _dbContext.ThanhToans.AnyAsync(x => x.MaNhanVienKeToan == username, cancellationToken))
                {
                    return AccountDeleteCheck.Blocked("ThanhToan (giao dịch do nhân viên kế toán lập)");
                }

                if (await _dbContext.BienNhans.AnyAsync(x => x.MaNhanVienLap == username, cancellationToken))
                {
                    return AccountDeleteCheck.Blocked("BienNhan (biên nhận do nhân viên lập)");
                }

                break;
            }
        }

        return AccountDeleteCheck.Allowed();
    }

    public async Task<WriteOutcome> DeleteAsync(int maTaiKhoan, CancellationToken cancellationToken)
    {
        // EnableRetryOnFailure() được bật ở Phase 1 -> transaction tự mở phải nằm trong ExecutionStrategy.
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(
            state: maTaiKhoan,
            operation: (_, state, token) => DeleteCoreAsync(state, token),
            verifySucceeded: null,
            cancellationToken: cancellationToken);
    }

    private async Task<WriteOutcome> DeleteCoreAsync(int maTaiKhoan, CancellationToken cancellationToken)
    {
        var account = await FindByIdAsync(maTaiKhoan, cancellationToken);

        if (account is null)
        {
            return WriteOutcome.NotFound;
        }

        var username = account.TenDangNhap;
        var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        try
        {
            // Xóa hồ sơ TRƯỚC (khóa ngoại của hồ sơ trỏ tới TaiKhoan).
            switch (account.TenVaiTro)
            {
                case RoleNames.HocVien:
                {
                    var hocVien = await _dbContext.HocViens
                        .FirstOrDefaultAsync(x => x.MaHocVien == username, cancellationToken);

                    if (hocVien is not null)
                    {
                        _dbContext.HocViens.Remove(hocVien);
                    }

                    break;
                }

                case RoleNames.GiaoVien:
                case RoleNames.TroGiang:
                {
                    var giaoVien = await _dbContext.GiaoViens
                        .FirstOrDefaultAsync(x => x.MaGiaoVien == username, cancellationToken);

                    if (giaoVien is not null)
                    {
                        _dbContext.GiaoViens.Remove(giaoVien);
                    }

                    break;
                }

                default:
                {
                    var nhanVien = await _dbContext.NhanViens
                        .FirstOrDefaultAsync(x => x.MaNhanVien == username, cancellationToken);

                    if (nhanVien is not null)
                    {
                        _dbContext.NhanViens.Remove(nhanVien);
                    }

                    break;
                }
            }

            var taiKhoan = await _dbContext.TaiKhoans
                .FirstOrDefaultAsync(x => x.MaTaiKhoan == maTaiKhoan, cancellationToken);

            if (taiKhoan is null)
            {
                await transaction.RollbackAsync(cancellationToken);
                return WriteOutcome.NotFound;
            }

            _dbContext.TaiKhoans.Remove(taiKhoan);

            await _dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);

            return WriteOutcome.Success;
        }
        catch (DbUpdateException)
        {
            // Có ràng buộc khóa ngoại chưa được kiểm tra hết -> rollback, không xóa nửa vời.
            await transaction.RollbackAsync(cancellationToken);
            _dbContext.ChangeTracker.Clear();
            return WriteOutcome.Conflict;
        }
        finally
        {
            await transaction.DisposeAsync();
        }
    }

    /// <summary>
    /// Query nền cho danh sách/chi tiết tài khoản: LEFT JOIN sang cả 3 bảng hồ sơ
    /// (NhanVien / GiaoVien / HocVien) để lấy HoTen tương ứng với vai trò.
    /// </summary>
    private IQueryable<AccountJoinRow> BuildAccountQuery()
    {
        return
            from taiKhoan in _dbContext.TaiKhoans.AsNoTracking()
            join vaiTro in _dbContext.VaiTros on taiKhoan.MaVaiTro equals vaiTro.MaVaiTro
            join nhanVien in _dbContext.NhanViens on taiKhoan.TenDangNhap equals nhanVien.MaNhanVien into nhanVienGroup
            from nhanVien in nhanVienGroup.DefaultIfEmpty()
            join giaoVien in _dbContext.GiaoViens on taiKhoan.TenDangNhap equals giaoVien.MaGiaoVien into giaoVienGroup
            from giaoVien in giaoVienGroup.DefaultIfEmpty()
            join hocVien in _dbContext.HocViens on taiKhoan.TenDangNhap equals hocVien.MaHocVien into hocVienGroup
            from hocVien in hocVienGroup.DefaultIfEmpty()
            select new AccountJoinRow
            {
                MaTaiKhoan = taiKhoan.MaTaiKhoan,
                TenDangNhap = taiKhoan.TenDangNhap,
                Email = taiKhoan.Email,
                TrangThaiTaiKhoan = taiKhoan.TrangThaiTaiKhoan,
                NgayTao = taiKhoan.NgayTao,
                NgayCapNhat = taiKhoan.NgayCapNhat,
                LanDangNhapCuoi = taiKhoan.LanDangNhapCuoi,
                MaVaiTro = taiKhoan.MaVaiTro,
                TenVaiTro = vaiTro.TenVaiTro,
                TenNhanVien = nhanVien.HoTen,
                TenGiaoVien = giaoVien.HoTen,
                LoaiGiaoVien = giaoVien.LoaiGiaoVien,
                TenHocVien = hocVien.HoTen,
                TrangThaiHocTap = hocVien.TrangThaiHocTap
            };
    }

    /// <summary>
    /// Dòng dữ liệu phẳng sau khi JOIN — CHỈ dùng nội bộ trong tầng Infrastructure.
    /// <c>HoTen</c> là COALESCE của 3 bảng hồ sơ; nếu không có hồ sơ nào thì fallback về TenDangNhap.
    /// </summary>
    private sealed class AccountJoinRow
    {
        public int MaTaiKhoan { get; set; }

        public string TenDangNhap { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string TrangThaiTaiKhoan { get; set; } = null!;

        public DateTime NgayTao { get; set; }

        public DateTime? NgayCapNhat { get; set; }

        public DateTime? LanDangNhapCuoi { get; set; }

        public int MaVaiTro { get; set; }

        public string TenVaiTro { get; set; } = null!;

        public string? TenNhanVien { get; set; }

        public string? TenGiaoVien { get; set; }

        public string? LoaiGiaoVien { get; set; }

        public string? TenHocVien { get; set; }

        public string? TrangThaiHocTap { get; set; }

        /// <summary>HoTen hiệu dụng: hồ sơ tương ứng, fallback về TenDangNhap.</summary>
        public string? HoTen => TenNhanVien ?? TenGiaoVien ?? TenHocVien;
    }
}
