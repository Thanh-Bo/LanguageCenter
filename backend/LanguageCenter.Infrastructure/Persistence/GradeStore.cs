using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.GradeTypes.Models;
using LanguageCenter.Application.Grades.Models;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="IGradeStore"/>
public sealed class GradeStore : IGradeStore
{
    private static readonly IReadOnlyDictionary<int, decimal> EmptyScores =
        new Dictionary<int, decimal>();

    private readonly AppDbContext _dbContext;

    public GradeStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<GradeListItemDto>> ListAsync(
        GradeListQuery query,
        CancellationToken cancellationToken)
    {
        var source = _dbContext.DiemSos.AsNoTracking();

        if (query.MaDangKy is { } maDangKy)
        {
            source = source.Where(x => x.MaDangKy == maDangKy);
        }

        if (query.MaLoaiDiem is { } maLoaiDiem)
        {
            source = source.Where(x => x.MaLoaiDiem == maLoaiDiem);
        }

        if (query.MaKhoaHoc is { } maKhoaHoc)
        {
            source = source.Where(x => x.DangKyHoc.MaKhoaHoc == maKhoaHoc);
        }

        if (query.MaLopHoc is { } maLopHoc)
        {
            source = source.Where(x => x.DangKyHoc.MaLopHoc == maLopHoc);
        }

        if (!string.IsNullOrWhiteSpace(query.MaHocVien))
        {
            var code = query.MaHocVien.Trim();
            source = source.Where(x => x.DangKyHoc.MaHocVien == code);
        }

        if (!string.IsNullOrWhiteSpace(query.MaGiaoVien))
        {
            var code = query.MaGiaoVien.Trim();
            source = source.Where(x => x.MaGiaoVien == code);
        }

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderBy(x => x.MaDangKy)
            .ThenBy(x => x.LoaiDiem.ThuTu)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(x => new GradeListItemDto
            {
                MaDiemSo = x.MaDiemSo,
                MaDangKy = x.MaDangKy,
                MaHocVien = x.DangKyHoc.MaHocVien,
                HoTenHocVien = x.DangKyHoc.HocVien.HoTen,
                MaLopHoc = x.DangKyHoc.MaLopHoc,
                MaLop = x.DangKyHoc.LopHoc != null ? x.DangKyHoc.LopHoc.MaLop : null,
                TenLop = x.DangKyHoc.LopHoc != null ? x.DangKyHoc.LopHoc.TenLop : null,
                MaLoaiDiem = x.MaLoaiDiem,
                TenLoaiDiem = x.LoaiDiem.TenLoaiDiem,
                DiemToiDa = x.LoaiDiem.DiemToiDa,
                Diem = x.Diem,
                NhanXet = x.NhanXet,
                TrangThaiKetQua = x.TrangThaiKetQua,
                MaGiaoVien = x.MaGiaoVien,
                TenGiaoVien = x.GiaoVien.HoTen,
                NgayCham = x.NgayCham,
                NgayCapNhat = x.NgayCapNhat
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<GradeListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public Task<GradeDetailDto?> GetDetailAsync(int maDiemSo, CancellationToken cancellationToken)
        => _dbContext.DiemSos
            .AsNoTracking()
            .Where(x => x.MaDiemSo == maDiemSo)
            .Select(x => new GradeDetailDto
            {
                MaDiemSo = x.MaDiemSo,
                MaDangKy = x.MaDangKy,
                MaHocVien = x.DangKyHoc.MaHocVien,
                HoTenHocVien = x.DangKyHoc.HocVien.HoTen,
                MaLopHoc = x.DangKyHoc.MaLopHoc,
                MaLop = x.DangKyHoc.LopHoc != null ? x.DangKyHoc.LopHoc.MaLop : null,
                TenLop = x.DangKyHoc.LopHoc != null ? x.DangKyHoc.LopHoc.TenLop : null,
                MaLoaiDiem = x.MaLoaiDiem,
                TenLoaiDiem = x.LoaiDiem.TenLoaiDiem,
                DiemToiDa = x.LoaiDiem.DiemToiDa,
                Diem = x.Diem,
                NhanXet = x.NhanXet,
                TrangThaiKetQua = x.TrangThaiKetQua,
                MaGiaoVien = x.MaGiaoVien,
                TenGiaoVien = x.GiaoVien.HoTen,
                NgayCham = x.NgayCham,
                NgayCapNhat = x.NgayCapNhat
            })
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> ExistsAsync(int maDiemSo, CancellationToken cancellationToken)
        => _dbContext.DiemSos
            .AsNoTracking()
            .AnyAsync(x => x.MaDiemSo == maDiemSo, cancellationToken);

    public Task<bool> DuplicateAsync(
        int maDangKy,
        int maLoaiDiem,
        int? exceptId,
        CancellationToken cancellationToken)
        => _dbContext.DiemSos
            .AsNoTracking()
            .AnyAsync(
                x => x.MaDangKy == maDangKy
                     && x.MaLoaiDiem == maLoaiDiem
                     && (exceptId == null || x.MaDiemSo != exceptId),
                cancellationToken);

    public async Task<WriteResult> CreateAsync(
        CreateGradeRequest request,
        string maGiaoVien,
        CancellationToken cancellationToken)
    {
        var entity = new DiemSo
        {
            MaDangKy = request.MaDangKy,
            MaLoaiDiem = request.MaLoaiDiem,
            MaGiaoVien = maGiaoVien,
            Diem = request.Diem,
            NhanXet = TextValue.Required(request.NhanXet),
            TrangThaiKetQua = TextValue.Required(request.TrangThaiKetQua),
            NgayCham = DateTime.Now,
            NgayCapNhat = null
        };

        _dbContext.DiemSos.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
            return WriteResult.Conflict();
        }

        return WriteResult.Success(entity.MaDiemSo);
    }

    public async Task<WriteOutcome> UpdateAsync(
        int maDiemSo,
        UpdateGradeRequest changes,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.DiemSos
            .FirstOrDefaultAsync(x => x.MaDiemSo == maDiemSo, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        if (changes.Diem is not null)
        {
            entity.Diem = changes.Diem.Value;
        }

        if (changes.NhanXet is not null)
        {
            entity.NhanXet = TextValue.Required(changes.NhanXet);
        }

        if (changes.TrangThaiKetQua is not null)
        {
            entity.TrangThaiKetQua = TextValue.Required(changes.TrangThaiKetQua);
        }

        entity.NgayCapNhat = DateTime.Now;

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

    public async Task<WriteOutcome> DeleteAsync(int maDiemSo, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.DiemSos
            .FirstOrDefaultAsync(x => x.MaDiemSo == maDiemSo, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        _dbContext.DiemSos.Remove(entity);

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

    public async Task<ClassGradeData?> GetClassGradeDataAsync(
        int maLopHoc,
        CancellationToken cancellationToken)
    {
        var lop = await _dbContext.LopHocs
            .AsNoTracking()
            .Where(x => x.MaLopHoc == maLopHoc)
            .Select(x => new
            {
                x.MaLopHoc,
                x.MaLop,
                x.TenLop,
                x.MaKhoaHoc,
                TenKhoaHoc = x.KhoaHoc.TenKhoaHoc
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (lop is null)
        {
            return null;
        }

        var components = await GetComponentsAsync(lop.MaKhoaHoc, cancellationToken);

        var enrollments = await _dbContext.DangKyHocs
            .AsNoTracking()
            .Where(x => x.MaLopHoc == maLopHoc
                        && (x.TrangThaiDangKy == EnrollmentStatuses.DaXepLop
                            || x.TrangThaiDangKy == EnrollmentStatuses.HoanThanh))
            .Select(x => new
            {
                x.MaDangKy,
                x.MaHocVien,
                HoTen = x.HocVien.HoTen
            })
            .ToListAsync(cancellationToken);

        var scoreMap = await GetScoreMapAsync(
            enrollments.Select(x => x.MaDangKy).ToList(),
            cancellationToken);

        var students = enrollments
            .OrderBy(x => x.HoTen)
            .ThenBy(x => x.MaHocVien)
            .Select(x => new StudentGradeRow(
                x.MaDangKy,
                x.MaHocVien,
                x.HoTen,
                scoreMap.GetValueOrDefault(x.MaDangKy) ?? EmptyScores))
            .ToList();

        return new ClassGradeData(
            lop.MaLopHoc,
            lop.MaLop,
            lop.TenLop,
            lop.MaKhoaHoc,
            lop.TenKhoaHoc,
            components,
            students);
    }

    public async Task<StudentGradeData?> GetStudentGradeDataAsync(
        string maHocVien,
        CancellationToken cancellationToken)
    {
        var code = maHocVien.Trim();

        var hocVien = await _dbContext.HocViens
            .AsNoTracking()
            .Where(x => x.MaHocVien == code)
            .Select(x => new { x.MaHocVien, x.HoTen })
            .FirstOrDefaultAsync(cancellationToken);

        if (hocVien is null)
        {
            return null;
        }

        var enrollments = await _dbContext.DangKyHocs
            .AsNoTracking()
            .Where(x => x.MaHocVien == code
                        && x.MaLopHoc != null
                        && (x.TrangThaiDangKy == EnrollmentStatuses.DaXepLop
                            || x.TrangThaiDangKy == EnrollmentStatuses.HoanThanh))
            .Select(x => new
            {
                x.MaDangKy,
                MaLopHoc = x.MaLopHoc!.Value,
                MaLop = x.LopHoc!.MaLop,
                TenLop = x.LopHoc!.TenLop,
                x.MaKhoaHoc,
                TenKhoaHoc = x.KhoaHoc.TenKhoaHoc,
                x.LopHoc!.NgayBatDau,
                x.LopHoc!.NgayKetThucDuKien,
                x.NgayXepLop
            })
            .ToListAsync(cancellationToken);

        if (enrollments.Count == 0)
        {
            return new StudentGradeData(hocVien.MaHocVien, hocVien.HoTen, []);
        }

        var componentCache = new Dictionary<int, IReadOnlyList<GradeComponentInfo>>();

        foreach (var maKhoaHoc in enrollments.Select(x => x.MaKhoaHoc).Distinct())
        {
            componentCache[maKhoaHoc] = await GetComponentsAsync(maKhoaHoc, cancellationToken);
        }

        var scoreMap = await GetScoreMapAsync(
            enrollments.Select(x => x.MaDangKy).ToList(),
            cancellationToken);

        var groups = enrollments
            .OrderBy(x => x.TenLop)
            .Select(x => new StudentGradeGroup(
                x.MaDangKy,
                x.MaLopHoc,
                x.MaLop,
                x.TenLop,
                x.MaKhoaHoc,
                x.TenKhoaHoc,
                x.NgayBatDau,
                x.NgayKetThucDuKien,
                x.NgayXepLop,
                componentCache[x.MaKhoaHoc],
                scoreMap.GetValueOrDefault(x.MaDangKy) ?? EmptyScores))
            .ToList();

        return new StudentGradeData(hocVien.MaHocVien, hocVien.HoTen, groups);
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    private async Task<IReadOnlyList<GradeComponentInfo>> GetComponentsAsync(
        int maKhoaHoc,
        CancellationToken cancellationToken)
        => await _dbContext.LoaiDiems
            .AsNoTracking()
            .Where(x => x.MaKhoaHoc == maKhoaHoc && x.TrangThai == GradeTypeStatuses.HoatDong)
            .OrderBy(x => x.ThuTu)
            .Select(x => new GradeComponentInfo(
                x.MaLoaiDiem,
                x.TenLoaiDiem,
                x.TrongSo,
                x.DiemToiDa,
                x.ThuTu))
            .ToListAsync(cancellationToken);

    private async Task<Dictionary<int, IReadOnlyDictionary<int, decimal>>> GetScoreMapAsync(
        IReadOnlyList<int> maDangKys,
        CancellationToken cancellationToken)
    {
        if (maDangKys.Count == 0)
        {
            return [];
        }

        var rows = await _dbContext.DiemSos
            .AsNoTracking()
            .Where(x => maDangKys.Contains(x.MaDangKy))
            .Select(x => new { x.MaDangKy, x.MaLoaiDiem, x.Diem })
            .ToListAsync(cancellationToken);

        return rows
            .GroupBy(x => x.MaDangKy)
            .ToDictionary(
                g => g.Key,
                g => (IReadOnlyDictionary<int, decimal>)g.ToDictionary(
                    x => x.MaLoaiDiem,
                    x => x.Diem));
    }
}
