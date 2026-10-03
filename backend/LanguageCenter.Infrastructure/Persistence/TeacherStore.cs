using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Teachers.Models;
using LanguageCenter.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="ITeacherStore"/>
public sealed class TeacherStore : ITeacherStore
{
    private readonly AppDbContext _dbContext;

    public TeacherStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TeacherDetailDto?> GetByCodeAsync(string maGiaoVien, CancellationToken cancellationToken)
    {
        var giaoVien = await _dbContext.GiaoViens
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.MaGiaoVien == maGiaoVien, cancellationToken);

        if (giaoVien is null)
        {
            return null;
        }

        // Bằng cấp / Chứng chỉ: CHỈ ĐỌC ở Phase 3 (chưa có upload file, chưa có CRUD).
        var bangCaps = await _dbContext.BangCaps
            .AsNoTracking()
            .Where(bangCap => bangCap.MaGiaoVien == maGiaoVien)
            .OrderByDescending(bangCap => bangCap.NamTotNghiep)
            .Select(bangCap => new BangCapDto
            {
                MaBangCap = bangCap.MaBangCap,
                TenBangCap = bangCap.TenBangCap,
                ChuyenNganh = bangCap.ChuyenNganh,
                CoSoDaoTao = bangCap.CoSoDaoTao,
                NamTotNghiep = bangCap.NamTotNghiep,
                SoBang = bangCap.SoBang,
                NgayCap = bangCap.NgayCap
            })
            .ToListAsync(cancellationToken);

        var chungChis = await _dbContext.ChungChis
            .AsNoTracking()
            .Where(chungChi => chungChi.MaGiaoVien == maGiaoVien)
            .OrderByDescending(chungChi => chungChi.NgayCap)
            .Select(chungChi => new ChungChiDto
            {
                MaChungChi = chungChi.MaChungChi,
                TenChungChi = chungChi.TenChungChi,
                LoaiChungChi = chungChi.LoaiChungChi,
                DonViCap = chungChi.DonViCap,
                KetQua = chungChi.KetQua,
                SoChungChi = chungChi.SoChungChi,
                NgayCap = chungChi.NgayCap,
                NgayHetHan = chungChi.NgayHetHan
            })
            .ToListAsync(cancellationToken);

        return new TeacherDetailDto
        {
            MaGiaoVien = giaoVien.MaGiaoVien,
            HoTen = giaoVien.HoTen,
            NgaySinh = giaoVien.NgaySinh,
            GioiTinh = giaoVien.GioiTinh,
            SoDienThoai = giaoVien.SoDienThoai,
            Email = giaoVien.Email,
            DiaChi = giaoVien.DiaChi,
            LoaiGiaoVien = giaoVien.LoaiGiaoVien,
            ChuyenMon = giaoVien.ChuyenMon,
            SoNamKinhNghiem = giaoVien.SoNamKinhNghiem,
            NgayVaoLam = giaoVien.NgayVaoLam,
            TrangThai = giaoVien.TrangThai,
            BangCaps = bangCaps,
            ChungChis = chungChis
        };
    }

    public async Task<PagedResult<TeacherListItemDto>> ListAsync(
        TeacherListQuery query,
        CancellationToken cancellationToken)
    {
        var source = _dbContext.GiaoViens.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();

            source = source.Where(giaoVien =>
                giaoVien.MaGiaoVien.Contains(search)
                || giaoVien.HoTen.Contains(search)
                || (giaoVien.Email != null && giaoVien.Email.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(query.LoaiGiaoVien))
        {
            source = source.Where(giaoVien => giaoVien.LoaiGiaoVien == query.LoaiGiaoVien);
        }

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderBy(giaoVien => giaoVien.MaGiaoVien)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(giaoVien => new TeacherListItemDto
            {
                MaGiaoVien = giaoVien.MaGiaoVien,
                HoTen = giaoVien.HoTen,
                LoaiGiaoVien = giaoVien.LoaiGiaoVien,
                Email = giaoVien.Email,
                SoDienThoai = giaoVien.SoDienThoai,
                ChuyenMon = giaoVien.ChuyenMon,
                SoNamKinhNghiem = giaoVien.SoNamKinhNghiem,
                TrangThai = giaoVien.TrangThai
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<TeacherListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public async Task<WriteOutcome> UpdateAsync(
        string maGiaoVien,
        UpdateTeacherRequest changes,
        CancellationToken cancellationToken)
    {
        var giaoVien = await _dbContext.GiaoViens
            .FirstOrDefaultAsync(x => x.MaGiaoVien == maGiaoVien, cancellationToken);

        if (giaoVien is null)
        {
            return WriteOutcome.NotFound;
        }

        var taiKhoan = await _dbContext.TaiKhoans
            .FirstOrDefaultAsync(x => x.TenDangNhap == maGiaoVien, cancellationToken);

        if (taiKhoan is null)
        {
            return WriteOutcome.NotFound;
        }

        // LoaiGiaoVien CỐ Ý không có trong UpdateTeacherRequest -> không thể bị thay đổi từ client
        // (đổi loại sẽ phải đổi khóa chính MaGiaoVien và mã đăng nhập, làm hỏng dữ liệu lịch sử).

        if (changes.HoTen is not null)
        {
            giaoVien.HoTen = changes.HoTen.Trim();
        }

        if (changes.NgaySinh is { } ngaySinh)
        {
            giaoVien.NgaySinh = ngaySinh;
        }

        if (changes.GioiTinh is not null)
        {
            giaoVien.GioiTinh = TextValue.Optional(changes.GioiTinh);
        }

        if (changes.SoDienThoai is not null)
        {
            giaoVien.SoDienThoai = TextValue.Optional(changes.SoDienThoai);
        }

        if (changes.DiaChi is not null)
        {
            giaoVien.DiaChi = TextValue.Optional(changes.DiaChi);
        }

        if (changes.ChuyenMon is not null)
        {
            giaoVien.ChuyenMon = TextValue.Optional(changes.ChuyenMon);
        }

        if (changes.SoNamKinhNghiem is { } soNam)
        {
            giaoVien.SoNamKinhNghiem = soNam;
        }

        if (changes.NgayVaoLam is { } ngayVaoLam)
        {
            giaoVien.NgayVaoLam = ngayVaoLam;
        }

        if (changes.TrangThai is not null)
        {
            giaoVien.TrangThai = changes.TrangThai.Trim();
        }

        if (changes.Email is not null)
        {
            var email = changes.Email.Trim();

            if (!string.Equals(taiKhoan.Email, email, StringComparison.Ordinal))
            {
                var alreadyTaken = await _dbContext.TaiKhoans
                    .AnyAsync(x => x.Email == email && x.TenDangNhap != maGiaoVien, cancellationToken);

                if (alreadyTaken)
                {
                    return WriteOutcome.DuplicateEmail;
                }
            }

            // ĐỒNG BỘ HAI BẢNG: TaiKhoan.Email và GiaoVien.Email luôn phải giống nhau.
            taiKhoan.Email = email;
            giaoVien.Email = email;
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

    private static bool IsUniqueViolation(Exception exception)
    {
        var inner = exception is DbUpdateException dbUpdateException
            ? dbUpdateException.InnerException
            : exception;

        return inner is SqlException { Number: 2601 or 2627 };
    }
}
