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

    /// <summary>
    /// Sinh mã phòng kế tiếp: lấy số lớn nhất trong các mã dạng <c>P&lt;số&gt;</c> rồi +1.
    /// Ví dụ đang có P101, P202, P303 → <c>P304</c>. Chưa có phòng nào → <c>P101</c>.
    /// </summary>
    public async Task<string> GenerateNextCodeAsync(CancellationToken cancellationToken)
    {
        var codes = await _dbContext.PhongHocs
            .AsNoTracking()
            .Where(x => x.MaPhong.StartsWith("P"))
            .Select(x => x.MaPhong)
            .ToListAsync(cancellationToken);

        var max = 100; // mặc định: phòng đầu tiên là P101

        foreach (var code in codes)
        {
            var duoi = code[1..].Trim();

            if (int.TryParse(duoi, out var so) && so > max)
            {
                max = so;
            }
        }

        return $"P{max + 1:000}";
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

    public async Task<WriteOutcome> UpdateAsync(
        int maPhongHoc,
        UpdateRoomRequest changes,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.PhongHocs
            .FirstOrDefaultAsync(x => x.MaPhongHoc == maPhongHoc, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        if (changes.TenPhong is not null)
        {
            entity.TenPhong = changes.TenPhong.Trim();
        }

        if (changes.ViTri is not null)
        {
            entity.ViTri = TextValue.Optional(changes.ViTri);
        }

        if (changes.SucChua is { } sucChua)
        {
            entity.SucChua = sucChua;
        }

        if (changes.ThietBi is not null)
        {
            entity.ThietBi = TextValue.Optional(changes.ThietBi);
        }

        if (changes.TrangThai is not null)
        {
            entity.TrangThai = changes.TrangThai.Trim();
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return WriteOutcome.Success;
    }

    public async Task<WriteOutcome> DeleteAsync(int maPhongHoc, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.PhongHocs
            .FirstOrDefaultAsync(x => x.MaPhongHoc == maPhongHoc, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        // Còn lịch học / ca học dùng phòng → không xoá (FK Restrict).
        var dangDung = await _dbContext.LichHocs.AnyAsync(x => x.MaPhongHoc == maPhongHoc, cancellationToken)
                       || await _dbContext.CaHocs.AnyAsync(x => x.MaPhongHoc == maPhongHoc, cancellationToken);

        if (dangDung)
        {
            return WriteOutcome.Conflict;
        }

        _dbContext.PhongHocs.Remove(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
            return WriteOutcome.Success;
        }
        catch (DbUpdateException)
        {
            return WriteOutcome.Conflict;
        }
    }
}
