using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Schedules.Models;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="IScheduleStore"/>
public sealed class ScheduleStore : IScheduleStore
{
    private static readonly DateOnly MaxDate = new(9999, 12, 31);

    private readonly AppDbContext _dbContext;

    public ScheduleStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<ScheduleListItemDto>> ListAsync(
        ScheduleListQuery query,
        CancellationToken cancellationToken)
    {
        var source = _dbContext.LichHocs.AsNoTracking();

        if (query.MaLopHoc is { } maLopHoc)
        {
            source = source.Where(x => x.MaLopHoc == maLopHoc);
        }

        if (query.MaPhongHoc is { } maPhongHoc)
        {
            source = source.Where(x => x.MaPhongHoc == maPhongHoc);
        }

        if (query.ThuTrongTuan is { } thu)
        {
            source = source.Where(x => x.ThuTrongTuan == thu);
        }

        if (!string.IsNullOrWhiteSpace(query.TrangThai))
        {
            var trangThai = query.TrangThai.Trim();
            source = source.Where(x => x.TrangThai == trangThai);
        }

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderBy(x => x.MaLopHoc)
            .ThenBy(x => x.ThuTrongTuan)
            .ThenBy(x => x.GioBatDau)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(x => new ScheduleListItemDto
            {
                MaLichHoc = x.MaLichHoc,
                MaLopHoc = x.MaLopHoc,
                MaLop = x.LopHoc.MaLop,
                TenLop = x.LopHoc.TenLop,
                MaKhoaHoc = x.LopHoc.MaKhoaHoc,
                TenKhoaHoc = x.LopHoc.KhoaHoc.TenKhoaHoc,
                MaPhongHoc = x.MaPhongHoc,
                MaPhong = x.PhongHoc.MaPhong,
                TenPhong = x.PhongHoc.TenPhong,
                ThuTrongTuan = x.ThuTrongTuan,
                GioBatDau = x.GioBatDau,
                GioKetThuc = x.GioKetThuc,
                NgayApDungTu = x.NgayApDungTu,
                NgayApDungDen = x.NgayApDungDen,
                TrangThai = x.TrangThai,
                SoCaHoc = x.CaHocs.Count()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<ScheduleListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public Task<ScheduleDetailDto?> GetDetailAsync(int maLichHoc, CancellationToken cancellationToken)
        => _dbContext.LichHocs
            .AsNoTracking()
            .Where(x => x.MaLichHoc == maLichHoc)
            .Select(x => new ScheduleDetailDto
            {
                MaLichHoc = x.MaLichHoc,
                MaLopHoc = x.MaLopHoc,
                MaLop = x.LopHoc.MaLop,
                TenLop = x.LopHoc.TenLop,
                MaKhoaHoc = x.LopHoc.MaKhoaHoc,
                TenKhoaHoc = x.LopHoc.KhoaHoc.TenKhoaHoc,
                MaPhongHoc = x.MaPhongHoc,
                MaPhong = x.PhongHoc.MaPhong,
                TenPhong = x.PhongHoc.TenPhong,
                ThuTrongTuan = x.ThuTrongTuan,
                GioBatDau = x.GioBatDau,
                GioKetThuc = x.GioKetThuc,
                NgayApDungTu = x.NgayApDungTu,
                NgayApDungDen = x.NgayApDungDen,
                TrangThai = x.TrangThai,
                SoCaHoc = x.CaHocs.Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> ExistsAsync(int maLichHoc, CancellationToken cancellationToken)
        => _dbContext.LichHocs
            .AsNoTracking()
            .AnyAsync(x => x.MaLichHoc == maLichHoc, cancellationToken);

    public Task<bool> HasRoomConflictAsync(
        int maPhongHoc,
        int thuTrongTuan,
        TimeOnly gioBatDau,
        TimeOnly gioKetThuc,
        DateOnly ngayApDungTu,
        DateOnly? ngayApDungDen,
        int? exceptId,
        CancellationToken cancellationToken)
    {
        var thu = (byte)thuTrongTuan;
        var end = ngayApDungDen ?? MaxDate;

        return _dbContext.LichHocs
            .AsNoTracking()
            .AnyAsync(
                x => x.MaPhongHoc == maPhongHoc
                     && x.ThuTrongTuan == thu
                     && x.TrangThai != ScheduleStatuses.NgungApDung
                     // Giờ chồng lấn
                     && x.GioBatDau < gioKetThuc
                     && gioBatDau < x.GioKetThuc
                     // Khoảng ngày chồng lấn
                     && x.NgayApDungTu <= end
                     && (x.NgayApDungDen ?? MaxDate) >= ngayApDungTu
                     && (exceptId == null || x.MaLichHoc != exceptId),
                cancellationToken);
    }

    public Task<int> CountSessionsAsync(int maLichHoc, CancellationToken cancellationToken)
        => _dbContext.CaHocs
            .AsNoTracking()
            .CountAsync(x => x.MaLichHoc == maLichHoc, cancellationToken);

    public async Task<WriteResult> CreateAsync(
        CreateScheduleRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new LichHoc
        {
            MaLopHoc = request.MaLopHoc,
            MaPhongHoc = request.MaPhongHoc,
            ThuTrongTuan = (byte)request.ThuTrongTuan,
            GioBatDau = request.GioBatDau,
            GioKetThuc = request.GioKetThuc,
            NgayApDungTu = request.NgayApDungTu,
            NgayApDungDen = request.NgayApDungDen,
            TrangThai = ScheduleStatuses.HoatDong
        };

        _dbContext.LichHocs.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
            return WriteResult.Conflict();
        }

        return WriteResult.Success(entity.MaLichHoc);
    }

    public async Task<WriteOutcome> UpdateAsync(
        int maLichHoc,
        UpdateScheduleRequest changes,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.LichHocs
            .FirstOrDefaultAsync(x => x.MaLichHoc == maLichHoc, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        if (changes.MaPhongHoc is not null)
        {
            entity.MaPhongHoc = changes.MaPhongHoc.Value;
        }

        if (changes.ThuTrongTuan is not null)
        {
            entity.ThuTrongTuan = (byte)changes.ThuTrongTuan.Value;
        }

        if (changes.GioBatDau is not null)
        {
            entity.GioBatDau = changes.GioBatDau.Value;
        }

        if (changes.GioKetThuc is not null)
        {
            entity.GioKetThuc = changes.GioKetThuc.Value;
        }

        if (changes.NgayApDungTu is not null)
        {
            entity.NgayApDungTu = changes.NgayApDungTu.Value;
        }

        if (changes.NgayApDungDen is not null)
        {
            entity.NgayApDungDen = changes.NgayApDungDen;
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
        int maLichHoc,
        string trangThai,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.LichHocs
            .FirstOrDefaultAsync(x => x.MaLichHoc == maLichHoc, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        entity.TrangThai = trangThai;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return WriteOutcome.Success;
    }

    public async Task<WriteOutcome> DeleteAsync(int maLichHoc, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.LichHocs
            .FirstOrDefaultAsync(x => x.MaLichHoc == maLichHoc, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        _dbContext.LichHocs.Remove(entity);

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
