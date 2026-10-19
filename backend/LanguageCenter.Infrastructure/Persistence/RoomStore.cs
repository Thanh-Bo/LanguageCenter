using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Rooms.Models;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="IRoomStore"/>
public sealed class RoomStore : IRoomStore
{
    private readonly AppDbContext _dbContext;

    public RoomStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<RoomListItemDto>> ListAsync(
        RoomListQuery query,
        CancellationToken cancellationToken)
    {
        var source = _dbContext.PhongHocs.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            source = source.Where(x => x.MaPhong.Contains(search) || x.TenPhong.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(query.TrangThai))
        {
            var trangThai = query.TrangThai.Trim();
            source = source.Where(x => x.TrangThai == trangThai);
        }

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderBy(x => x.MaPhongHoc)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(x => new RoomListItemDto
            {
                MaPhongHoc = x.MaPhongHoc,
                MaPhong = x.MaPhong,
                TenPhong = x.TenPhong,
                ViTri = x.ViTri,
                SucChua = x.SucChua,
                ThietBi = x.ThietBi,
                TrangThai = x.TrangThai
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<RoomListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public Task<RoomListItemDto?> GetDetailAsync(int maPhongHoc, CancellationToken cancellationToken)
        => _dbContext.PhongHocs
            .AsNoTracking()
            .Where(x => x.MaPhongHoc == maPhongHoc)
            .Select(x => new RoomListItemDto
            {
                MaPhongHoc = x.MaPhongHoc,
                MaPhong = x.MaPhong,
                TenPhong = x.TenPhong,
                ViTri = x.ViTri,
                SucChua = x.SucChua,
                ThietBi = x.ThietBi,
                TrangThai = x.TrangThai
            })
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> CodeExistsAsync(string maPhong, CancellationToken cancellationToken)
    {
        var code = maPhong.Trim();

        return _dbContext.PhongHocs
            .AsNoTracking()
            .AnyAsync(x => x.MaPhong == code, cancellationToken);
    }

    public async Task<int?> CreateAsync(RoomCreateData data, CancellationToken cancellationToken)
    {
        var entity = new PhongHoc
        {
            MaPhong = data.MaPhong,
            TenPhong = data.TenPhong,
            ViTri = data.ViTri,
            SucChua = data.SucChua,
            ThietBi = data.ThietBi,
            TrangThai = data.TrangThai
        };

        _dbContext.PhongHocs.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return entity.MaPhongHoc;
        }
        catch (DbUpdateException)
        {
            // Trùng MaPhong do race condition (UNIQUE) → detach để context sạch.
            _dbContext.Entry(entity).State = EntityState.Detached;
            return null;
        }
    }
}
