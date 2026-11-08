using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Employees.Models;
using LanguageCenter.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <summary>
/// Đọc/ghi hồ sơ nhân viên (<c>NhanVien</c>) + đồng bộ <c>TaiKhoan</c>.
/// </summary>
/// <remarks>
/// <para>
/// <b>Vì sao phải đồng bộ email?</b> <c>NhanVien.MaNhanVien</c> vừa là PK vừa là FK trỏ tới
/// <c>TaiKhoan.TenDangNhap</c>. Hai bảng cùng lưu email, nếu chỉ sửa một bên thì:
/// đăng nhập lại bằng email cũ, hoặc UI hiển thị email khác với email dùng để reset mật khẩu.
/// </para>
/// </remarks>
public sealed class EmployeeStore : IEmployeeStore
{
    private readonly AppDbContext _dbContext;

    public EmployeeStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<EmployeeListItemDto>> ListAsync(
        EmployeeListQuery query,
        CancellationToken cancellationToken)
    {
        var source = _dbContext.NhanViens.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.ChucVu))
        {
            var chucVu = query.ChucVu.Trim();
            source = source.Where(x => x.ChucVu == chucVu);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            source = source.Where(x =>
                x.MaNhanVien.Contains(search)
                || x.HoTen.Contains(search)
                || (x.Email != null && x.Email.Contains(search))
                || (x.SoDienThoai != null && x.SoDienThoai.Contains(search)));
        }

        // Lọc trạng thái PHẢI chạy ở SQL: lọc sau khi phân trang chỉ đúng với 20 dòng của trang hiện tại.
        if (!string.IsNullOrWhiteSpace(query.TrangThai))
        {
            var trangThai = query.TrangThai.Trim();
            source = source.Where(x => x.TrangThaiLamViec == trangThai);
        }

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderBy(x => x.MaNhanVien)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(x => new EmployeeListItemDto
            {
                MaNhanVien = x.MaNhanVien,
                HoTen = x.HoTen,
                ChucVu = x.ChucVu,
                SoDienThoai = x.SoDienThoai,
                Email = x.Email,
                NgayVaoLam = x.NgayVaoLam,
                TrangThaiLamViec = x.TrangThaiLamViec
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<EmployeeListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<EmployeeDetailDto?> GetByCodeAsync(string maNhanVien, CancellationToken cancellationToken)
        => await _dbContext.NhanViens
            .AsNoTracking()
            .Where(x => x.MaNhanVien == maNhanVien)
            .Select(x => new EmployeeDetailDto
            {
                MaNhanVien = x.MaNhanVien,
                HoTen = x.HoTen,
                NgaySinh = x.NgaySinh,
                GioiTinh = x.GioiTinh,
                SoDienThoai = x.SoDienThoai,
                Email = x.Email,
                DiaChi = x.DiaChi,
                ChucVu = x.ChucVu,
                NgayVaoLam = x.NgayVaoLam,
                TrangThaiLamViec = x.TrangThaiLamViec,
                TenDangNhap = x.TaiKhoan.TenDangNhap,
                TrangThaiTaiKhoan = x.TaiKhoan.TrangThaiTaiKhoan,
                TenVaiTro = x.TaiKhoan.VaiTro.TenVaiTro
            })
            .FirstOrDefaultAsync(cancellationToken);

    /// <summary>
    /// Số điện thoại dùng chung giữa học viên / phụ huynh / giáo viên / nhân viên — trùng ở bất kỳ
    /// hồ sơ nào cũng chặn, vì hệ thống tra cứu người dùng bằng SĐT ở nhiều màn hình.
    /// </summary>
    public async Task<bool> PhoneExistsAsync(
        string soDienThoai,
        string? exceptMaNhanVien,
        CancellationToken cancellationToken)
    {
        var soDienThoaiDaChuanHoa = soDienThoai.Trim();

        var trungNhanVien = await _dbContext.NhanViens
            .AnyAsync(
                x => x.SoDienThoai == soDienThoaiDaChuanHoa
                     && (exceptMaNhanVien == null || x.MaNhanVien != exceptMaNhanVien),
                cancellationToken);

        if (trungNhanVien)
        {
            return true;
        }

        var trungGiaoVien = await _dbContext.GiaoViens
            .AnyAsync(x => x.SoDienThoai == soDienThoaiDaChuanHoa, cancellationToken);

        if (trungGiaoVien)
        {
            return true;
        }

        var trungHocVien = await _dbContext.HocViens
            .AnyAsync(x => x.SoDienThoai == soDienThoaiDaChuanHoa, cancellationToken);

        if (trungHocVien)
        {
            return true;
        }

        return await _dbContext.PhuHuynhs
            .AnyAsync(x => x.SoDienThoai == soDienThoaiDaChuanHoa, cancellationToken);
    }

    public async Task<WriteOutcome> UpdateAsync(
        string maNhanVien,
        UpdateEmployeeRequest changes,
        CancellationToken cancellationToken)
    {
        var nhanVien = await _dbContext.NhanViens
            .FirstOrDefaultAsync(x => x.MaNhanVien == maNhanVien, cancellationToken);

        if (nhanVien is null)
        {
            return WriteOutcome.NotFound;
        }

        var taiKhoan = await _dbContext.TaiKhoans
            .FirstOrDefaultAsync(x => x.TenDangNhap == maNhanVien, cancellationToken);

        if (taiKhoan is null)
        {
            return WriteOutcome.NotFound;
        }

        // ChucVu CỐ Ý không sửa được: đổi chức vụ = đổi vai trò + mã đăng nhập
        // → làm hỏng dữ liệu lịch sử (lớp phụ trách, giao dịch kế toán đã lập).

        if (changes.HoTen is not null)
        {
            nhanVien.HoTen = changes.HoTen.Trim();
        }

        if (changes.NgaySinh is { } ngaySinh)
        {
            nhanVien.NgaySinh = ngaySinh;
        }

        if (changes.GioiTinh is not null)
        {
            nhanVien.GioiTinh = TextValue.Optional(changes.GioiTinh);
        }

        if (changes.SoDienThoai is not null)
        {
            nhanVien.SoDienThoai = TextValue.Optional(changes.SoDienThoai);
        }

        if (changes.DiaChi is not null)
        {
            nhanVien.DiaChi = TextValue.Optional(changes.DiaChi);
        }

        if (changes.NgayVaoLam is { } ngayVaoLam)
        {
            nhanVien.NgayVaoLam = ngayVaoLam;
        }

        if (changes.TrangThaiLamViec is not null)
        {
            nhanVien.TrangThaiLamViec = changes.TrangThaiLamViec.Trim();
        }

        if (changes.Email is not null)
        {
            var email = changes.Email.Trim();

            if (!string.Equals(taiKhoan.Email, email, StringComparison.Ordinal))
            {
                var alreadyTaken = await _dbContext.TaiKhoans
                    .AnyAsync(x => x.Email == email && x.TenDangNhap != maNhanVien, cancellationToken);

                if (alreadyTaken)
                {
                    return WriteOutcome.DuplicateEmail;
                }
            }

            taiKhoan.Email = email;
            nhanVien.Email = email;
        }

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return WriteOutcome.DuplicateEmail;
        }

        return WriteOutcome.Success;
    }

    /// <summary>Lỗi vi phạm UNIQUE của SQL Server: 2601 (unique index) hoặc 2627 (unique constraint/PK).</summary>
    private static bool IsUniqueViolation(Exception exception)
    {
        var inner = exception is DbUpdateException dbUpdateException
            ? dbUpdateException.InnerException
            : exception;

        return inner is SqlException { Number: 2601 or 2627 };
    }
}
