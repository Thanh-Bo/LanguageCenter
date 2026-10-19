using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Programs.Models;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="IProgramStore"/>
public sealed class ProgramStore : IProgramStore
{
    private readonly AppDbContext _dbContext;

    public ProgramStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<ProgramListItemDto>> ListAsync(
        ProgramListQuery query,
        CancellationToken cancellationToken)
    {
        var source = _dbContext.ChuongTrinhs.AsNoTracking();

        // Lọc ngay trong SQL — KHÔNG load toàn bộ rồi mới Where().
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            source = source.Where(x => x.TenChuongTrinh.Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(query.TrangThai))
        {
            var trangThai = query.TrangThai.Trim();
            source = source.Where(x => x.TrangThai == trangThai);
        }

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderBy(x => x.MaChuongTrinh)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(x => new ProgramListItemDto
            {
                MaChuongTrinh = x.MaChuongTrinh,
                TenChuongTrinh = x.TenChuongTrinh,
                NhomDoiTuong = x.NhomDoiTuong,
                DoTuoiTu = x.DoTuoiTu,
                DoTuoiDen = x.DoTuoiDen,
                TrangThai = x.TrangThai,
                SoCapDo = x.CapDos.Count()
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<ProgramListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public Task<ProgramDetailDto?> GetDetailAsync(int maChuongTrinh, CancellationToken cancellationToken)
        => _dbContext.ChuongTrinhs
            .AsNoTracking()
            .Where(x => x.MaChuongTrinh == maChuongTrinh)
            .Select(x => new ProgramDetailDto
            {
                MaChuongTrinh = x.MaChuongTrinh,
                TenChuongTrinh = x.TenChuongTrinh,
                NhomDoiTuong = x.NhomDoiTuong,
                DoTuoiTu = x.DoTuoiTu,
                DoTuoiDen = x.DoTuoiDen,
                MucTieu = x.MucTieu,
                MoTa = x.MoTa,
                TrangThai = x.TrangThai,
                SoCapDo = x.CapDos.Count(),
                SoKhoaHoc = x.CapDos.SelectMany(capDo => capDo.KhoaHocs).Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> ExistsAsync(int maChuongTrinh, CancellationToken cancellationToken)
        => _dbContext.ChuongTrinhs
            .AsNoTracking()
            .AnyAsync(x => x.MaChuongTrinh == maChuongTrinh, cancellationToken);

    public Task<bool> NameExistsAsync(
        string tenChuongTrinh,
        int? exceptId,
        CancellationToken cancellationToken)
    {
        var name = tenChuongTrinh.Trim();

        return _dbContext.ChuongTrinhs
            .AsNoTracking()
            .AnyAsync(
                x => x.TenChuongTrinh == name && (exceptId == null || x.MaChuongTrinh != exceptId),
                cancellationToken);
    }

    public async Task<WriteResult> CreateAsync(
        CreateProgramRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new ChuongTrinh
        {
            TenChuongTrinh = request.TenChuongTrinh.Trim(),
            NhomDoiTuong = TextValue.Optional(request.NhomDoiTuong),
            DoTuoiTu = request.DoTuoiTu,
            DoTuoiDen = request.DoTuoiDen,
            MucTieu = TextValue.Optional(request.MucTieu),
            MoTa = TextValue.Optional(request.MoTa),
            // Chương trình mới luôn ở trạng thái hoạt động.
            TrangThai = ActivityStatuses.HoatDong
        };

        _dbContext.ChuongTrinhs.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsUniqueViolation(exception))
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
            return WriteResult.Conflict();
        }

        return WriteResult.Success(entity.MaChuongTrinh);
    }

    public async Task<WriteOutcome> UpdateAsync(
        int maChuongTrinh,
        UpdateProgramRequest changes,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.ChuongTrinhs
            .FirstOrDefaultAsync(x => x.MaChuongTrinh == maChuongTrinh, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        // PATCH semantics: null = không đổi.
        if (changes.TenChuongTrinh is not null)
        {
            entity.TenChuongTrinh = changes.TenChuongTrinh.Trim();
        }

        if (changes.NhomDoiTuong is not null)
        {
            entity.NhomDoiTuong = TextValue.Optional(changes.NhomDoiTuong);
        }

        if (changes.DoTuoiTu is not null)
        {
            entity.DoTuoiTu = changes.DoTuoiTu;
        }

        if (changes.DoTuoiDen is not null)
        {
            entity.DoTuoiDen = changes.DoTuoiDen;
        }

        if (changes.MucTieu is not null)
        {
            entity.MucTieu = TextValue.Optional(changes.MucTieu);
        }

        if (changes.MoTa is not null)
        {
            entity.MoTa = TextValue.Optional(changes.MoTa);
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
        int maChuongTrinh,
        string trangThai,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.ChuongTrinhs
            .FirstOrDefaultAsync(x => x.MaChuongTrinh == maChuongTrinh, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        entity.TrangThai = trangThai;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return WriteOutcome.Success;
    }
}
