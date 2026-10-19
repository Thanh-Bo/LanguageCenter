using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.GradeTypes.Models;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="IGradeTypeStore"/>
public sealed class GradeTypeStore : IGradeTypeStore
{
    private readonly AppDbContext _dbContext;

    public GradeTypeStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<GradeTypeListItemDto>> ListAsync(
        GradeTypeListQuery query,
        CancellationToken cancellationToken)
    {
        var source = _dbContext.LoaiDiems.AsNoTracking();

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
            source = source.Where(x => x.TenLoaiDiem.Contains(search));
        }

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderBy(x => x.MaKhoaHoc)
            .ThenBy(x => x.ThuTu)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(x => new GradeTypeListItemDto
            {
                MaLoaiDiem = x.MaLoaiDiem,
                MaKhoaHoc = x.MaKhoaHoc,
                TenKhoaHoc = x.KhoaHoc.TenKhoaHoc,
                TenLoaiDiem = x.TenLoaiDiem,
                TrongSo = x.TrongSo,
                DiemToiDa = x.DiemToiDa,
                ThuTu = x.ThuTu,
                TrangThai = x.TrangThai,
                SoDiemDaNhap = x.DiemSos.Count()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<GradeTypeListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public Task<GradeTypeDetailDto?> GetDetailAsync(int maLoaiDiem, CancellationToken cancellationToken)
        => _dbContext.LoaiDiems
            .AsNoTracking()
            .Where(x => x.MaLoaiDiem == maLoaiDiem)
            .Select(x => new GradeTypeDetailDto
            {
                MaLoaiDiem = x.MaLoaiDiem,
                MaKhoaHoc = x.MaKhoaHoc,
                TenKhoaHoc = x.KhoaHoc.TenKhoaHoc,
                TenLoaiDiem = x.TenLoaiDiem,
                TrongSo = x.TrongSo,
                DiemToiDa = x.DiemToiDa,
                ThuTu = x.ThuTu,
                TrangThai = x.TrangThai,
                SoDiemDaNhap = x.DiemSos.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> ExistsAsync(int maLoaiDiem, CancellationToken cancellationToken)
        => _dbContext.LoaiDiems
            .AsNoTracking()
            .AnyAsync(x => x.MaLoaiDiem == maLoaiDiem, cancellationToken);

    public async Task<IReadOnlyList<GradeComponentInfo>> GetActiveByCourseAsync(
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

    public Task<int> CountGradesAsync(int maLoaiDiem, CancellationToken cancellationToken)
        => _dbContext.DiemSos
            .AsNoTracking()
            .CountAsync(x => x.MaLoaiDiem == maLoaiDiem, cancellationToken);

    public async Task<WriteResult> CreateAsync(
        CreateGradeTypeRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new LoaiDiem
        {
            MaKhoaHoc = request.MaKhoaHoc,
            TenLoaiDiem = request.TenLoaiDiem.Trim(),
            TrongSo = request.TrongSo,
            DiemToiDa = request.DiemToiDa,
            ThuTu = request.ThuTu,
            TrangThai = GradeTypeStatuses.HoatDong
        };

        _dbContext.LoaiDiems.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
            return WriteResult.Conflict();
        }

        return WriteResult.Success(entity.MaLoaiDiem);
    }

    public async Task<WriteOutcome> UpdateAsync(
        int maLoaiDiem,
        UpdateGradeTypeRequest changes,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.LoaiDiems
            .FirstOrDefaultAsync(x => x.MaLoaiDiem == maLoaiDiem, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        if (changes.TenLoaiDiem is not null)
        {
            entity.TenLoaiDiem = changes.TenLoaiDiem.Trim();
        }

        if (changes.TrongSo is not null)
        {
            entity.TrongSo = changes.TrongSo.Value;
        }

        if (changes.DiemToiDa is not null)
        {
            entity.DiemToiDa = changes.DiemToiDa.Value;
        }

        if (changes.ThuTu is not null)
        {
            entity.ThuTu = changes.ThuTu.Value;
        }

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

    public async Task<WriteOutcome> UpdateStatusAsync(
        int maLoaiDiem,
        string trangThai,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.LoaiDiems
            .FirstOrDefaultAsync(x => x.MaLoaiDiem == maLoaiDiem, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        entity.TrangThai = trangThai;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return WriteOutcome.Success;
    }

    public async Task<WriteOutcome> DeleteAsync(int maLoaiDiem, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.LoaiDiems
            .FirstOrDefaultAsync(x => x.MaLoaiDiem == maLoaiDiem, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        _dbContext.LoaiDiems.Remove(entity);

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

    public Task<bool> NameTakenAsync(
        int maKhoaHoc,
        string tenLoaiDiem,
        int? exceptId,
        CancellationToken cancellationToken)
    {
        var name = tenLoaiDiem.Trim();

        return _dbContext.LoaiDiems
            .AsNoTracking()
            .AnyAsync(
                x => x.MaKhoaHoc == maKhoaHoc
                     && x.TenLoaiDiem == name
                     && (exceptId == null || x.MaLoaiDiem != exceptId),
                cancellationToken);
    }

    public Task<bool> OrderTakenAsync(
        int maKhoaHoc,
        int thuTu,
        int? exceptId,
        CancellationToken cancellationToken)
        => _dbContext.LoaiDiems
            .AsNoTracking()
            .AnyAsync(
                x => x.MaKhoaHoc == maKhoaHoc
                     && x.ThuTu == thuTu
                     && (exceptId == null || x.MaLoaiDiem != exceptId),
                cancellationToken);
}
