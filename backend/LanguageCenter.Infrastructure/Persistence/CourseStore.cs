using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Courses.Models;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="ICourseStore"/>
public sealed class CourseStore : ICourseStore
{
    private readonly AppDbContext _dbContext;

    public CourseStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<CourseListItemDto>> ListAsync(
        CourseListQuery query,
        CancellationToken cancellationToken)
    {
        var source = _dbContext.KhoaHocs.AsNoTracking();

        if (query.MaCapDo is { } maCapDo)
        {
            source = source.Where(x => x.MaCapDo == maCapDo);
        }

        if (query.MaChuongTrinh is { } maChuongTrinh)
        {
            source = source.Where(x => x.CapDo.MaChuongTrinh == maChuongTrinh);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            source = source.Where(x => x.TenKhoaHoc.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(query.TrangThai))
        {
            var trangThai = query.TrangThai.Trim();
            source = source.Where(x => x.TrangThai == trangThai);
        }

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderBy(x => x.MaKhoaHoc)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(x => new CourseListItemDto
            {
                MaKhoaHoc = x.MaKhoaHoc,
                MaCapDo = x.MaCapDo,
                TenCapDo = x.CapDo.TenCapDo,
                MaChuongTrinh = x.CapDo.MaChuongTrinh,
                TenChuongTrinh = x.CapDo.ChuongTrinh.TenChuongTrinh,
                TenKhoaHoc = x.TenKhoaHoc,
                SoBuoi = x.SoBuoi,
                TongSoGio = x.TongSoGio,
                HocPhiCoBan = x.HocPhiCoBan,
                NgayBatDau = x.NgayBatDau,
                NgayKetThuc = x.NgayKetThuc,
                TrangThai = x.TrangThai,
                SoLop = x.LopHocs.Count()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<CourseListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public Task<CourseDetailDto?> GetDetailAsync(int maKhoaHoc, CancellationToken cancellationToken)
        => _dbContext.KhoaHocs
            .AsNoTracking()
            .Where(x => x.MaKhoaHoc == maKhoaHoc)
            .Select(x => new CourseDetailDto
            {
                MaKhoaHoc = x.MaKhoaHoc,
                MaCapDo = x.MaCapDo,
                TenCapDo = x.CapDo.TenCapDo,
                MaChuongTrinh = x.CapDo.MaChuongTrinh,
                TenChuongTrinh = x.CapDo.ChuongTrinh.TenChuongTrinh,
                TenKhoaHoc = x.TenKhoaHoc,
                NoiDung = x.NoiDung,
                SoBuoi = x.SoBuoi,
                TongSoGio = x.TongSoGio,
                HocPhiCoBan = x.HocPhiCoBan,
                NgayBatDau = x.NgayBatDau,
                NgayKetThuc = x.NgayKetThuc,
                TrangThai = x.TrangThai,
                SoLop = x.LopHocs.Count(),
                SoDangKy = x.DangKyHocs.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> ExistsAsync(int maKhoaHoc, CancellationToken cancellationToken)
        => _dbContext.KhoaHocs
            .AsNoTracking()
            .AnyAsync(x => x.MaKhoaHoc == maKhoaHoc, cancellationToken);

    public Task<bool> NameExistsAsync(
        int maCapDo,
        string tenKhoaHoc,
        int? exceptId,
        CancellationToken cancellationToken)
    {
        var name = tenKhoaHoc.Trim();

        return _dbContext.KhoaHocs
            .AsNoTracking()
            .AnyAsync(
                x => x.MaCapDo == maCapDo
                     && x.TenKhoaHoc == name
                     && (exceptId == null || x.MaKhoaHoc != exceptId),
                cancellationToken);
    }

    public Task<CourseTuitionInfo?> GetTuitionInfoAsync(int maKhoaHoc, CancellationToken cancellationToken)
        => _dbContext.KhoaHocs
            .AsNoTracking()
            .Where(x => x.MaKhoaHoc == maKhoaHoc)
            .Select(x => new CourseTuitionInfo(x.MaKhoaHoc, x.TenKhoaHoc, x.HocPhiCoBan, x.TrangThai))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<WriteResult> CreateAsync(
        CreateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new KhoaHoc
        {
            MaCapDo = request.MaCapDo,
            TenKhoaHoc = request.TenKhoaHoc.Trim(),
            NoiDung = TextValue.Optional(request.NoiDung),
            SoBuoi = request.SoBuoi,
            TongSoGio = request.TongSoGio,
            HocPhiCoBan = request.HocPhiCoBan,
            NgayBatDau = request.NgayBatDau,
            NgayKetThuc = request.NgayKetThuc,
            // Khóa học mới mở để tuyển sinh.
            TrangThai = CourseStatuses.DangTuyenSinh
        };

        _dbContext.KhoaHocs.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsUniqueViolation(exception))
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
            return WriteResult.Conflict();
        }

        return WriteResult.Success(entity.MaKhoaHoc);
    }

    public async Task<WriteOutcome> UpdateAsync(
        int maKhoaHoc,
        UpdateCourseRequest changes,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.KhoaHocs
            .FirstOrDefaultAsync(x => x.MaKhoaHoc == maKhoaHoc, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        if (changes.TenKhoaHoc is not null)
        {
            entity.TenKhoaHoc = changes.TenKhoaHoc.Trim();
        }

        if (changes.NoiDung is not null)
        {
            entity.NoiDung = TextValue.Optional(changes.NoiDung);
        }

        if (changes.SoBuoi is not null)
        {
            entity.SoBuoi = changes.SoBuoi.Value;
        }

        if (changes.TongSoGio is not null)
        {
            entity.TongSoGio = changes.TongSoGio;
        }

        if (changes.HocPhiCoBan is not null)
        {
            entity.HocPhiCoBan = changes.HocPhiCoBan.Value;
        }

        if (changes.NgayBatDau is not null)
        {
            entity.NgayBatDau = changes.NgayBatDau.Value;
        }

        if (changes.NgayKetThuc is not null)
        {
            entity.NgayKetThuc = changes.NgayKetThuc.Value;
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
        int maKhoaHoc,
        string trangThai,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.KhoaHocs
            .FirstOrDefaultAsync(x => x.MaKhoaHoc == maKhoaHoc, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        entity.TrangThai = trangThai;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return WriteOutcome.Success;
    }
}
