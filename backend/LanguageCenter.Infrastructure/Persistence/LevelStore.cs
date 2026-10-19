using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Levels.Models;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="ILevelStore"/>
public sealed class LevelStore : ILevelStore
{
    private readonly AppDbContext _dbContext;

    public LevelStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<LevelListItemDto>> ListAsync(
        LevelListQuery query,
        CancellationToken cancellationToken)
    {
        var source = _dbContext.CapDos.AsNoTracking();

        if (query.MaChuongTrinh is { } maChuongTrinh)
        {
            source = source.Where(x => x.MaChuongTrinh == maChuongTrinh);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            source = source.Where(
                x => x.TenCapDo.Contains(search)
                     || (x.KyHieuCapDo != null && x.KyHieuCapDo.Contains(search)));
        }

        if (!string.IsNullOrWhiteSpace(query.TrangThai))
        {
            var trangThai = query.TrangThai.Trim();
            source = source.Where(x => x.TrangThai == trangThai);
        }

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderBy(x => x.MaChuongTrinh)
            .ThenBy(x => x.ThuTu)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(x => new LevelListItemDto
            {
                MaCapDo = x.MaCapDo,
                MaChuongTrinh = x.MaChuongTrinh,
                TenChuongTrinh = x.ChuongTrinh.TenChuongTrinh,
                KyHieuCapDo = x.KyHieuCapDo,
                TenCapDo = x.TenCapDo,
                ThuTu = x.ThuTu,
                ThoiLuongDuKienTuan = x.ThoiLuongDuKienTuan,
                TrangThai = x.TrangThai,
                SoKhoaHoc = x.KhoaHocs.Count()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<LevelListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public Task<LevelDetailDto?> GetDetailAsync(int maCapDo, CancellationToken cancellationToken)
        => _dbContext.CapDos
            .AsNoTracking()
            .Where(x => x.MaCapDo == maCapDo)
            .Select(x => new LevelDetailDto
            {
                MaCapDo = x.MaCapDo,
                MaChuongTrinh = x.MaChuongTrinh,
                TenChuongTrinh = x.ChuongTrinh.TenChuongTrinh,
                KyHieuCapDo = x.KyHieuCapDo,
                TenCapDo = x.TenCapDo,
                ThuTu = x.ThuTu,
                DieuKienDauVao = x.DieuKienDauVao,
                KetQuaDauRa = x.KetQuaDauRa,
                ThoiLuongDuKienTuan = x.ThoiLuongDuKienTuan,
                TrangThai = x.TrangThai,
                SoKhoaHoc = x.KhoaHocs.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> ExistsAsync(int maCapDo, CancellationToken cancellationToken)
        => _dbContext.CapDos
            .AsNoTracking()
            .AnyAsync(x => x.MaCapDo == maCapDo, cancellationToken);

    public Task<bool> NameExistsAsync(
        int maChuongTrinh,
        string tenCapDo,
        int? exceptId,
        CancellationToken cancellationToken)
    {
        var name = tenCapDo.Trim();

        return _dbContext.CapDos
            .AsNoTracking()
            .AnyAsync(
                x => x.MaChuongTrinh == maChuongTrinh
                     && x.TenCapDo == name
                     && (exceptId == null || x.MaCapDo != exceptId),
                cancellationToken);
    }

    public Task<bool> OrderExistsAsync(
        int maChuongTrinh,
        int thuTu,
        int? exceptId,
        CancellationToken cancellationToken)
        => _dbContext.CapDos
            .AsNoTracking()
            .AnyAsync(
                x => x.MaChuongTrinh == maChuongTrinh
                     && x.ThuTu == thuTu
                     && (exceptId == null || x.MaCapDo != exceptId),
                cancellationToken);

    public async Task<WriteResult> CreateAsync(
        CreateLevelRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new CapDo
        {
            MaChuongTrinh = request.MaChuongTrinh,
            KyHieuCapDo = TextValue.Optional(request.KyHieuCapDo),
            TenCapDo = request.TenCapDo.Trim(),
            ThuTu = request.ThuTu,
            DieuKienDauVao = TextValue.Optional(request.DieuKienDauVao),
            KetQuaDauRa = TextValue.Optional(request.KetQuaDauRa),
            ThoiLuongDuKienTuan = request.ThoiLuongDuKienTuan,
            TrangThai = ActivityStatuses.HoatDong
        };

        _dbContext.CapDos.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsUniqueViolation(exception))
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
            return WriteResult.Conflict();
        }

        return WriteResult.Success(entity.MaCapDo);
    }

    public async Task<WriteOutcome> UpdateAsync(
        int maCapDo,
        UpdateLevelRequest changes,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.CapDos
            .FirstOrDefaultAsync(x => x.MaCapDo == maCapDo, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        if (changes.KyHieuCapDo is not null)
        {
            entity.KyHieuCapDo = TextValue.Optional(changes.KyHieuCapDo);
        }

        if (changes.TenCapDo is not null)
        {
            entity.TenCapDo = changes.TenCapDo.Trim();
        }

        if (changes.ThuTu is not null)
        {
            entity.ThuTu = changes.ThuTu.Value;
        }

        if (changes.DieuKienDauVao is not null)
        {
            entity.DieuKienDauVao = TextValue.Optional(changes.DieuKienDauVao);
        }

        if (changes.KetQuaDauRa is not null)
        {
            entity.KetQuaDauRa = TextValue.Optional(changes.KetQuaDauRa);
        }

        if (changes.ThoiLuongDuKienTuan is not null)
        {
            entity.ThoiLuongDuKienTuan = changes.ThoiLuongDuKienTuan;
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
        int maCapDo,
        string trangThai,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.CapDos
            .FirstOrDefaultAsync(x => x.MaCapDo == maCapDo, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        entity.TrangThai = trangThai;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return WriteOutcome.Success;
    }
}
