using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Students.Models;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="IStudentStore"/>
public sealed class StudentStore : IStudentStore
{
    private readonly AppDbContext _dbContext;

    public StudentStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<StudentDetailDto?> GetByCodeAsync(string maHocVien, CancellationToken cancellationToken)
    {
        var hocVien = await _dbContext.HocViens
            .AsNoTracking()
            .Include(x => x.PhuHuynh)
            .FirstOrDefaultAsync(x => x.MaHocVien == maHocVien, cancellationToken);

        if (hocVien is null)
        {
            return null;
        }

        return new StudentDetailDto
        {
            MaHocVien = hocVien.MaHocVien,
            HoTen = hocVien.HoTen,
            NgaySinh = hocVien.NgaySinh,
            GioiTinh = hocVien.GioiTinh,
            SoDienThoai = hocVien.SoDienThoai,
            Email = hocVien.Email,
            DiaChi = hocVien.DiaChi,
            MucTieuHocTap = hocVien.MucTieuHocTap,
            TrinhDoHienTai = hocVien.TrinhDoHienTai,
            TrangThaiHocTap = hocVien.TrangThaiHocTap,
            NgayTaoHoSo = hocVien.NgayTaoHoSo,
            NgayNhapHoc = hocVien.NgayNhapHoc,
            PhuHuynh = hocVien.PhuHuynh is null
                ? null
                : new ParentDto
                {
                    MaPhuHuynh = hocVien.PhuHuynh.MaPhuHuynh,
                    HoTen = hocVien.PhuHuynh.HoTen,
                    SoDienThoai = hocVien.PhuHuynh.SoDienThoai,
                    Email = hocVien.PhuHuynh.Email,
                    DiaChi = hocVien.PhuHuynh.DiaChi,
                    NgheNghiep = hocVien.PhuHuynh.NgheNghiep,
                    // MoiQuanHe nằm trên HocVien, KHÔNG nằm trên PhuHuynh.
                    MoiQuanHe = hocVien.MoiQuanHePhuHuynh
                }
        };
    }

    public async Task<PagedResult<StudentListItemDto>> ListAsync(
        StudentListQuery query,
        CancellationToken cancellationToken)
    {
        var source = _dbContext.HocViens.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            source = source.Where(hocVien =>
                hocVien.MaHocVien.Contains(search)
                || hocVien.HoTen.Contains(search)
                || (hocVien.Email != null && hocVien.Email.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(query.TrangThai))
        {
            source = source.Where(hocVien => hocVien.TrangThaiHocTap == query.TrangThai);
        }

        // Count + Skip/Take đều chạy ở SQL Server — KHÔNG load toàn bộ rồi phân trang ở memory.
        var totalItems = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderBy(hocVien => hocVien.MaHocVien)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(hocVien => new StudentListItemDto
            {
                MaHocVien = hocVien.MaHocVien,
                HoTen = hocVien.HoTen,
                NgaySinh = hocVien.NgaySinh,
                GioiTinh = hocVien.GioiTinh,
                Email = hocVien.Email,
                SoDienThoai = hocVien.SoDienThoai,
                TrangThaiHocTap = hocVien.TrangThaiHocTap,
                NgayNhapHoc = hocVien.NgayNhapHoc
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<StudentListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<WriteOutcome> UpdateAsync(
        string maHocVien,
        UpdateStudentRequest changes,
        CancellationToken cancellationToken)
    {
        var hocVien = await _dbContext.HocViens
            .FirstOrDefaultAsync(x => x.MaHocVien == maHocVien, cancellationToken);

        if (hocVien is null)
        {
            return WriteOutcome.NotFound;
        }

        var taiKhoan = await _dbContext.TaiKhoans
            .FirstOrDefaultAsync(x => x.TenDangNhap == maHocVien, cancellationToken);

        if (taiKhoan is null)
        {
            return WriteOutcome.NotFound;
        }

        // PATCH: field null = không đổi. Chuỗi rỗng = xóa (về NULL) với field cho phép null.
        if (changes.HoTen is not null)
        {
            hocVien.HoTen = changes.HoTen.Trim();
        }

        if (changes.NgaySinh is { } ngaySinh)
        {
            hocVien.NgaySinh = ngaySinh;
        }

        if (changes.GioiTinh is not null)
        {
            hocVien.GioiTinh = TextValue.Optional(changes.GioiTinh);
        }

        if (changes.SoDienThoai is not null)
        {
            hocVien.SoDienThoai = TextValue.Optional(changes.SoDienThoai);
        }

        if (changes.DiaChi is not null)
        {
            hocVien.DiaChi = TextValue.Optional(changes.DiaChi);
        }

        if (changes.MucTieuHocTap is not null)
        {
            hocVien.MucTieuHocTap = TextValue.Optional(changes.MucTieuHocTap);
        }

        if (changes.TrinhDoHienTai is not null)
        {
            hocVien.TrinhDoHienTai = TextValue.Optional(changes.TrinhDoHienTai);
        }

        if (changes.TrangThaiHocTap is not null)
        {
            hocVien.TrangThaiHocTap = changes.TrangThaiHocTap.Trim();
        }

        if (changes.NgayNhapHoc is { } ngayNhapHoc)
        {
            hocVien.NgayNhapHoc = ngayNhapHoc;
        }

        if (changes.Email is not null)
        {
            var email = changes.Email.Trim();

            if (!string.Equals(taiKhoan.Email, email, StringComparison.Ordinal))
            {
                // Kiểm tra unique trước cho thông báo rõ ràng. Database vẫn là chốt cuối (xử lý ở catch).
                var alreadyTaken = await _dbContext.TaiKhoans
                    .AnyAsync(x => x.Email == email && x.TenDangNhap != maHocVien, cancellationToken);

                if (alreadyTaken)
                {
                    return WriteOutcome.DuplicateEmail;
                }
            }

            // ĐỒNG BỘ HAI BẢNG: TaiKhoan.Email và HocVien.Email luôn phải giống nhau.
            // Cả hai entity đều tracked nên SaveChangesAsync ghi chúng trong CÙNG một transaction ngầm định.
            taiKhoan.Email = email;
            hocVien.Email = email;
        }

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            // Race condition: tài khoản khác vừa chiếm email trong lúc đang xử lý.
            return WriteOutcome.DuplicateEmail;
        }

        return WriteOutcome.Success;
    }

    private static bool IsUniqueViolation(Exception exception)
    {
        var inner = exception is DbUpdateException dbUpdateException
            ? dbUpdateException.InnerException
            : exception;

        return inner is SqlException { Number: 2601 or 2627 };
    }
}
