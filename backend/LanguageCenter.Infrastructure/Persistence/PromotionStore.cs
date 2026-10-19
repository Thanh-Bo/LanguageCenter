using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Promotions.Models;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="IPromotionStore"/>
public sealed class PromotionStore : IPromotionStore
{
    private readonly AppDbContext _dbContext;

    public PromotionStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<PromotionListItemDto>> ListAsync(
        PromotionListQuery query,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var source = _dbContext.UuDais.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            source = source.Where(x => x.TenUuDai.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(query.TrangThai))
        {
            var trangThai = query.TrangThai.Trim();
            source = source.Where(x => x.TrangThai == trangThai);
        }

        if (query.ConHieuLuc == true)
        {
            source = source.Where(
                x => x.TrangThai == ActivityStatuses.HoatDong
                     && x.NgayBatDau <= today
                     && x.NgayKetThuc >= today);
        }

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderByDescending(x => x.MaUuDai)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(x => new PromotionListItemDto
            {
                MaUuDai = x.MaUuDai,
                TenUuDai = x.TenUuDai,
                LoaiGiamGia = x.LoaiGiamGia,
                GiaTriGiam = x.GiaTriGiam,
                SoKhoaHocToiThieu = x.SoKhoaHocToiThieu,
                NgayBatDau = x.NgayBatDau,
                NgayKetThuc = x.NgayKetThuc,
                TrangThai = x.TrangThai,
                ConHieuLuc = x.TrangThai == ActivityStatuses.HoatDong
                             && x.NgayBatDau <= today
                             && x.NgayKetThuc >= today
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<PromotionListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public Task<PromotionDetailDto?> GetDetailAsync(int maUuDai, CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        return _dbContext.UuDais
            .AsNoTracking()
            .Where(x => x.MaUuDai == maUuDai)
            .Select(x => new PromotionDetailDto
            {
                MaUuDai = x.MaUuDai,
                TenUuDai = x.TenUuDai,
                LoaiGiamGia = x.LoaiGiamGia,
                GiaTriGiam = x.GiaTriGiam,
                SoKhoaHocToiThieu = x.SoKhoaHocToiThieu,
                NgayBatDau = x.NgayBatDau,
                NgayKetThuc = x.NgayKetThuc,
                Poster = x.Poster,
                MoTa = x.MoTa,
                TrangThai = x.TrangThai,
                ConHieuLuc = x.TrangThai == ActivityStatuses.HoatDong
                             && x.NgayBatDau <= today
                             && x.NgayKetThuc >= today,
                SoLanApDung = x.DangKyHocs.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task<bool> ExistsAsync(int maUuDai, CancellationToken cancellationToken)
        => _dbContext.UuDais
            .AsNoTracking()
            .AnyAsync(x => x.MaUuDai == maUuDai, cancellationToken);

    public Task<PromotionForApply?> GetForApplyAsync(int maUuDai, CancellationToken cancellationToken)
        => _dbContext.UuDais
            .AsNoTracking()
            .Where(x => x.MaUuDai == maUuDai)
            .Select(x => new PromotionForApply(
                x.MaUuDai,
                x.TenUuDai,
                x.LoaiGiamGia,
                x.GiaTriGiam,
                x.SoKhoaHocToiThieu,
                x.NgayBatDau,
                x.NgayKetThuc,
                x.TrangThai))
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<WriteResult> CreateAsync(
        CreatePromotionRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new UuDai
        {
            TenUuDai = request.TenUuDai.Trim(),
            LoaiGiamGia = request.LoaiGiamGia.Trim(),
            GiaTriGiam = request.GiaTriGiam,
            SoKhoaHocToiThieu = request.SoKhoaHocToiThieu,
            NgayBatDau = request.NgayBatDau,
            NgayKetThuc = request.NgayKetThuc,
            Poster = TextValue.Optional(request.Poster),
            MoTa = TextValue.Optional(request.MoTa),
            TrangThai = ActivityStatuses.HoatDong
        };

        _dbContext.UuDais.Add(entity);

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

        return WriteResult.Success(entity.MaUuDai);
    }

    public async Task<WriteOutcome> UpdateAsync(
        int maUuDai,
        UpdatePromotionRequest changes,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.UuDais
            .FirstOrDefaultAsync(x => x.MaUuDai == maUuDai, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        if (changes.TenUuDai is not null)
        {
            entity.TenUuDai = changes.TenUuDai.Trim();
        }

        if (changes.LoaiGiamGia is not null)
        {
            entity.LoaiGiamGia = changes.LoaiGiamGia.Trim();
        }

        if (changes.GiaTriGiam is not null)
        {
            entity.GiaTriGiam = changes.GiaTriGiam.Value;
        }

        if (changes.SoKhoaHocToiThieu is not null)
        {
            entity.SoKhoaHocToiThieu = changes.SoKhoaHocToiThieu.Value;
        }

        if (changes.NgayBatDau is not null)
        {
            entity.NgayBatDau = changes.NgayBatDau.Value;
        }

        if (changes.NgayKetThuc is not null)
        {
            entity.NgayKetThuc = changes.NgayKetThuc.Value;
        }

        if (changes.Poster is not null)
        {
            entity.Poster = TextValue.Optional(changes.Poster);
        }

        if (changes.MoTa is not null)
        {
            entity.MoTa = TextValue.Optional(changes.MoTa);
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
        int maUuDai,
        string trangThai,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.UuDais
            .FirstOrDefaultAsync(x => x.MaUuDai == maUuDai, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        entity.TrangThai = trangThai;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return WriteOutcome.Success;
    }
}
