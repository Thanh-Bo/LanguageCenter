using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.TeachingAssignments.Models;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="ITeachingAssignmentStore"/>
public sealed class TeachingAssignmentStore : ITeachingAssignmentStore
{
    /// <summary>Ngày "vô cực" thay cho NgayKetThuc NULL khi so sánh chồng lấn.</summary>
    private static readonly DateOnly MaxDate = new(9999, 12, 31);

    private readonly AppDbContext _dbContext;

    public TeachingAssignmentStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<AssignmentListItemDto>> ListAsync(
        AssignmentListQuery query,
        CancellationToken cancellationToken)
    {
        var source = _dbContext.PhanCongGiangDays.AsNoTracking();

        if (query.MaLopHoc is { } maLopHoc)
        {
            source = source.Where(x => x.MaLopHoc == maLopHoc);
        }

        if (!string.IsNullOrWhiteSpace(query.MaGiaoVien))
        {
            var maGiaoVien = query.MaGiaoVien.Trim();
            source = source.Where(x => x.MaGiaoVien == maGiaoVien);
        }

        if (!string.IsNullOrWhiteSpace(query.TrangThai))
        {
            var trangThai = query.TrangThai.Trim();
            source = source.Where(x => x.TrangThai == trangThai);
        }

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await source
            .OrderBy(x => x.MaLopHoc)
            .ThenBy(x => x.NgayBatDau)
            .Skip(query.Skip)
            .Take(query.Take)
            .Select(x => new AssignmentListItemDto
            {
                MaPhanCong = x.MaPhanCong,
                MaLopHoc = x.MaLopHoc,
                MaLop = x.LopHoc.MaLop,
                TenLop = x.LopHoc.TenLop,
                MaKhoaHoc = x.LopHoc.MaKhoaHoc,
                TenKhoaHoc = x.LopHoc.KhoaHoc.TenKhoaHoc,
                MaGiaoVien = x.MaGiaoVien,
                TenGiaoVien = x.GiaoVien.HoTen,
                LoaiGiaoVien = x.GiaoVien.LoaiGiaoVien,
                VaiTroPhanCong = x.VaiTroPhanCong,
                NgayBatDau = x.NgayBatDau,
                NgayKetThuc = x.NgayKetThuc,
                TrangThai = x.TrangThai
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<AssignmentListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public Task<AssignmentDetailDto?> GetDetailAsync(int maPhanCong, CancellationToken cancellationToken)
        => _dbContext.PhanCongGiangDays
            .AsNoTracking()
            .Where(x => x.MaPhanCong == maPhanCong)
            .Select(x => new AssignmentDetailDto
            {
                MaPhanCong = x.MaPhanCong,
                MaLopHoc = x.MaLopHoc,
                MaLop = x.LopHoc.MaLop,
                TenLop = x.LopHoc.TenLop,
                MaKhoaHoc = x.LopHoc.MaKhoaHoc,
                TenKhoaHoc = x.LopHoc.KhoaHoc.TenKhoaHoc,
                MaGiaoVien = x.MaGiaoVien,
                TenGiaoVien = x.GiaoVien.HoTen,
                LoaiGiaoVien = x.GiaoVien.LoaiGiaoVien,
                VaiTroPhanCong = x.VaiTroPhanCong,
                NgayBatDau = x.NgayBatDau,
                NgayKetThuc = x.NgayKetThuc,
                TrangThai = x.TrangThai,
                GhiChu = x.GhiChu
            })
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> ExistsAsync(int maPhanCong, CancellationToken cancellationToken)
        => _dbContext.PhanCongGiangDays
            .AsNoTracking()
            .AnyAsync(x => x.MaPhanCong == maPhanCong, cancellationToken);

    public Task<bool> HasOverlapAsync(
        int maLopHoc,
        string maGiaoVien,
        string vaiTroPhanCong,
        DateOnly ngayBatDau,
        DateOnly? ngayKetThuc,
        int? exceptId,
        CancellationToken cancellationToken)
    {
        var code = maGiaoVien.Trim();
        var end = ngayKetThuc ?? MaxDate;

        return _dbContext.PhanCongGiangDays
            .AsNoTracking()
            .AnyAsync(
                x => x.MaLopHoc == maLopHoc
                     && x.MaGiaoVien == code
                     && x.VaiTroPhanCong == vaiTroPhanCong
                     && x.TrangThai != AssignmentStatuses.DaHuy
                     // Hai khoảng [a1,a2] và [b1,b2] chồng lấn khi a1 <= b2 && b1 <= a2.
                     && x.NgayBatDau <= end
                     && (x.NgayKetThuc ?? MaxDate) >= ngayBatDau
                     && (exceptId == null || x.MaPhanCong != exceptId),
                cancellationToken);
    }

    public async Task<WriteResult> CreateAsync(
        CreateAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new PhanCongGiangDay
        {
            MaLopHoc = request.MaLopHoc,
            MaGiaoVien = request.MaGiaoVien.Trim(),
            VaiTroPhanCong = request.VaiTroPhanCong.Trim(),
            NgayBatDau = request.NgayBatDau,
            NgayKetThuc = request.NgayKetThuc,
            GhiChu = TextValue.Optional(request.GhiChu),
            TrangThai = AssignmentStatuses.DangPhanCong
        };

        _dbContext.PhanCongGiangDays.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
            return WriteResult.Conflict();
        }

        return WriteResult.Success(entity.MaPhanCong);
    }

    public async Task<WriteOutcome> UpdateAsync(
        int maPhanCong,
        UpdateAssignmentRequest changes,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.PhanCongGiangDays
            .FirstOrDefaultAsync(x => x.MaPhanCong == maPhanCong, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        if (changes.MaGiaoVien is not null)
        {
            entity.MaGiaoVien = changes.MaGiaoVien.Trim();
        }

        if (changes.VaiTroPhanCong is not null)
        {
            entity.VaiTroPhanCong = changes.VaiTroPhanCong.Trim();
        }

        if (changes.NgayBatDau is not null)
        {
            entity.NgayBatDau = changes.NgayBatDau.Value;
        }

        if (changes.NgayKetThuc is not null)
        {
            entity.NgayKetThuc = changes.NgayKetThuc;
        }

        if (changes.GhiChu is not null)
        {
            entity.GhiChu = TextValue.Optional(changes.GhiChu);
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
        int maPhanCong,
        string trangThai,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.PhanCongGiangDays
            .FirstOrDefaultAsync(x => x.MaPhanCong == maPhanCong, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        entity.TrangThai = trangThai;

        await _dbContext.SaveChangesAsync(cancellationToken);

        return WriteOutcome.Success;
    }
}
