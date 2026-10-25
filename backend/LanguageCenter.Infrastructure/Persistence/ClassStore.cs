using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Classes.Models;
using LanguageCenter.Application.Common;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="IClassStore"/>
public sealed class ClassStore : IClassStore
{
    private readonly AppDbContext _dbContext;

    public ClassStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<ClassListItemDto>> ListAsync(
        ClassListQuery query,
        CancellationToken cancellationToken)
    {
        var source = _dbContext.LopHocs.AsNoTracking();

        if (query.MaKhoaHoc is { } maKhoaHoc)
        {
            source = source.Where(x => x.MaKhoaHoc == maKhoaHoc);
        }

        if (!string.IsNullOrWhiteSpace(query.TrangThai))
        {
            var trangThai = query.TrangThai.Trim();
            source = source.Where(x => x.TrangThai == trangThai);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            source = source.Where(x => x.MaLop.Contains(search) || x.TenLop.Contains(search));
        }

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderBy(x => x.MaLopHoc)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(x => new ClassListItemDto
            {
                MaLopHoc = x.MaLopHoc,
                MaLop = x.MaLop,
                TenLop = x.TenLop,
                MaKhoaHoc = x.MaKhoaHoc,
                TenKhoaHoc = x.KhoaHoc.TenKhoaHoc,
                MaNhanVienHocVu = x.MaNhanVienHocVu,
                TenNhanVienHocVu = x.NhanVienHocVu.HoTen,
                NgayBatDau = x.NgayBatDau,
                NgayKetThucDuKien = x.NgayKetThucDuKien,
                NgayKetThucThucTe = x.NgayKetThucThucTe,
                SiSoToiDa = x.SiSoToiDa,
                SiSoHienTai = x.DangKyHocs.Count(
                    d => d.TrangThaiDangKy == EnrollmentStatuses.DaXepLop
                         || d.TrangThaiDangKy == EnrollmentStatuses.HoanThanh),
                SoCaHoc = x.CaHocs.Count(),
                TrangThai = x.TrangThai
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<ClassListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public Task<ClassDetailDto?> GetDetailAsync(int maLopHoc, CancellationToken cancellationToken)
        => _dbContext.LopHocs
            .AsNoTracking()
            .Where(x => x.MaLopHoc == maLopHoc)
            .Select(x => new ClassDetailDto
            {
                MaLopHoc = x.MaLopHoc,
                MaLop = x.MaLop,
                TenLop = x.TenLop,
                MaKhoaHoc = x.MaKhoaHoc,
                TenKhoaHoc = x.KhoaHoc.TenKhoaHoc,
                MaCapDo = x.KhoaHoc.MaCapDo,
                TenCapDo = x.KhoaHoc.CapDo.TenCapDo,
                MaChuongTrinh = x.KhoaHoc.CapDo.MaChuongTrinh,
                TenChuongTrinh = x.KhoaHoc.CapDo.ChuongTrinh.TenChuongTrinh,
                MaNhanVienHocVu = x.MaNhanVienHocVu,
                TenNhanVienHocVu = x.NhanVienHocVu.HoTen,
                NgayBatDau = x.NgayBatDau,
                NgayKetThucDuKien = x.NgayKetThucDuKien,
                NgayKetThucThucTe = x.NgayKetThucThucTe,
                SiSoToiDa = x.SiSoToiDa,
                SiSoHienTai = x.DangKyHocs.Count(
                    d => d.TrangThaiDangKy == EnrollmentStatuses.DaXepLop
                         || d.TrangThaiDangKy == EnrollmentStatuses.HoanThanh),
                SoPhanCong = x.PhanCongGiangDays.Count(),
                TrangThai = x.TrangThai
            })
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> ExistsAsync(int maLopHoc, CancellationToken cancellationToken)
        => _dbContext.LopHocs
            .AsNoTracking()
            .AnyAsync(x => x.MaLopHoc == maLopHoc, cancellationToken);

    public Task<bool> CodeExistsAsync(string maLop, int? exceptId, CancellationToken cancellationToken)
    {
        var code = maLop.Trim();

        return _dbContext.LopHocs
            .AsNoTracking()
            .AnyAsync(
                x => x.MaLop == code && (exceptId == null || x.MaLopHoc != exceptId),
                cancellationToken);
    }

    public Task<ClassCapacityInfo?> GetCapacityInfoAsync(int maLopHoc, CancellationToken cancellationToken)
        => _dbContext.LopHocs
            .AsNoTracking()
            .Where(x => x.MaLopHoc == maLopHoc)
            .Select(x => new ClassCapacityInfo(
                x.MaLopHoc, x.MaLop, x.MaKhoaHoc, x.SiSoToiDa, x.TrangThai))
            .FirstOrDefaultAsync(cancellationToken);

    /// <summary>
    /// Sinh mã lớp kế tiếp: <c>{KyHieuCapDo}-{STT:00}</c> — ví dụ cấp độ <c>TEEN-2</c> → <c>TEEN-2-01</c>,
    /// <c>TEEN-2-02</c>... STT lấy theo mã lớn nhất đang có cùng tiền tố.
    /// </summary>
    public async Task<string?> GenerateNextCodeAsync(int maKhoaHoc, CancellationToken cancellationToken)
    {
        var kyHieu = await _dbContext.KhoaHocs
            .AsNoTracking()
            .Where(x => x.MaKhoaHoc == maKhoaHoc)
            .Join(
                _dbContext.CapDos.AsNoTracking(),
                khoa => khoa.MaCapDo,
                cap => cap.MaCapDo,
                (khoa, cap) => cap.KyHieuCapDo)
            .FirstOrDefaultAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(kyHieu))
        {
            return null;
        }

        var prefix = kyHieu.Trim().ToUpperInvariant() + "-";

        var codes = await _dbContext.LopHocs
            .AsNoTracking()
            .Where(x => x.MaLop.StartsWith(prefix))
            .Select(x => x.MaLop)
            .ToListAsync(cancellationToken);

        var max = 0;

        foreach (var code in codes)
        {
            var duoi = code[prefix.Length..].Trim();

            if (int.TryParse(duoi, out var so) && so > max)
            {
                max = so;
            }
        }

        return $"{prefix}{(max + 1):00}";
    }

    public async Task<WriteResult> CreateAsync(
        CreateClassRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new LopHoc
        {
            MaLop = (request.MaLop ?? string.Empty).Trim(),
            TenLop = request.TenLop.Trim(),
            MaKhoaHoc = request.MaKhoaHoc,
            MaNhanVienHocVu = request.MaNhanVienHocVu.Trim(),
            NgayBatDau = request.NgayBatDau,
            NgayKetThucDuKien = request.NgayKetThucDuKien,
            NgayKetThucThucTe = null,
            SiSoToiDa = request.SiSoToiDa,
            TrangThai = ClassStatuses.ChuanBiMo
        };

        _dbContext.LopHocs.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsUniqueViolation(exception))
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
            return WriteResult.Conflict();
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
            return WriteResult.Conflict();
        }

        return WriteResult.Success(entity.MaLopHoc);
    }

    public async Task<WriteOutcome> UpdateAsync(
        int maLopHoc,
        UpdateClassRequest changes,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.LopHocs
            .FirstOrDefaultAsync(x => x.MaLopHoc == maLopHoc, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        if (changes.MaLop is not null)
        {
            entity.MaLop = changes.MaLop.Trim();
        }

        if (changes.TenLop is not null)
        {
            entity.TenLop = changes.TenLop.Trim();
        }

        if (changes.MaNhanVienHocVu is not null)
        {
            entity.MaNhanVienHocVu = changes.MaNhanVienHocVu.Trim();
        }

        if (changes.NgayBatDau is not null)
        {
            entity.NgayBatDau = changes.NgayBatDau.Value;
        }

        if (changes.NgayKetThucDuKien is not null)
        {
            entity.NgayKetThucDuKien = changes.NgayKetThucDuKien.Value;
        }

        if (changes.NgayKetThucThucTe is not null)
        {
            entity.NgayKetThucThucTe = changes.NgayKetThucThucTe;
        }

        if (changes.SiSoToiDa is not null)
        {
            entity.SiSoToiDa = changes.SiSoToiDa.Value;
        }

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsUniqueViolation(exception))
        {
            return WriteOutcome.Conflict;
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            return WriteOutcome.Conflict;
        }

        return WriteOutcome.Success;
    }

    public async Task<WriteOutcome> UpdateStatusAsync(
        int maLopHoc,
        string trangThai,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.LopHocs
            .FirstOrDefaultAsync(x => x.MaLopHoc == maLopHoc, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        entity.TrangThai = trangThai;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return WriteOutcome.Success;
    }

    public async Task<ClassDeleteCheck> CheckDeletableAsync(
        int maLopHoc,
        CancellationToken cancellationToken)
    {
        if (await _dbContext.DangKyHocs.AnyAsync(x => x.MaLopHoc == maLopHoc, cancellationToken))
        {
            return ClassDeleteCheck.Blocked("DangKyHoc (đăng ký học của lớp)");
        }

        if (await _dbContext.PhanCongGiangDays.AnyAsync(x => x.MaLopHoc == maLopHoc, cancellationToken))
        {
            return ClassDeleteCheck.Blocked("PhanCongGiangDay (phân công giảng dạy)");
        }

        if (await _dbContext.LichHocs.AnyAsync(x => x.MaLopHoc == maLopHoc, cancellationToken))
        {
            return ClassDeleteCheck.Blocked("LichHoc (lịch học)");
        }

        if (await _dbContext.CaHocs.AnyAsync(x => x.MaLopHoc == maLopHoc, cancellationToken))
        {
            return ClassDeleteCheck.Blocked("CaHoc (ca học đã diễn ra)");
        }

        if (await _dbContext.ThongBaos.AnyAsync(x => x.MaLopHoc == maLopHoc, cancellationToken))
        {
            return ClassDeleteCheck.Blocked("ThongBao (thông báo của lớp)");
        }

        return ClassDeleteCheck.Allowed();
    }

    public async Task<WriteOutcome> DeleteAsync(int maLopHoc, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.LopHocs
            .FirstOrDefaultAsync(x => x.MaLopHoc == maLopHoc, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        _dbContext.LopHocs.Remove(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            return WriteOutcome.Conflict;
        }

        return WriteOutcome.Success;
    }
}
