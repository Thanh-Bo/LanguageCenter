using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.AcademicSetup.Models;
using LanguageCenter.Application.Common;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="IAcademicSetupStore"/>
public sealed class AcademicSetupStore : IAcademicSetupStore
{
    private readonly AppDbContext _dbContext;

    public AcademicSetupStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // ======================================================================
    // 20 BUỔI HỌC
    // ======================================================================

    public async Task<IReadOnlyList<LessonContentDto>> ListLessonContentsAsync(
        int maKhoaHoc,
        CancellationToken cancellationToken)
        => await _dbContext.NoiDungBaiHocs
            .AsNoTracking()
            .Where(x => x.MaKhoaHoc == maKhoaHoc)
            .OrderBy(x => x.SoBuoi)
            .Select(x => new LessonContentDto
            {
                MaNoiDung = x.MaNoiDung,
                MaKhoaHoc = x.MaKhoaHoc,
                SoBuoi = x.SoBuoi,
                TieuDe = x.TieuDe,
                NoiDung = x.NoiDung,
                MucTieu = x.MucTieu
            })
            .ToListAsync(cancellationToken);

    public async Task<int> SaveLessonContentsAsync(
        int maKhoaHoc,
        IReadOnlyList<LessonContentInput> items,
        CancellationToken cancellationToken)
    {
        var hienCo = await _dbContext.NoiDungBaiHocs
            .Where(x => x.MaKhoaHoc == maKhoaHoc)
            .ToListAsync(cancellationToken);

        var theoSoBuoi = hienCo.ToDictionary(x => x.SoBuoi);
        var soDong = 0;

        foreach (var item in items)
        {
            var tieuDe = (item.TieuDe ?? string.Empty).Trim();
            var noiDung = TextValue.Required(item.NoiDung);
            var mucTieu = TextValue.Required(item.MucTieu);

            if (theoSoBuoi.TryGetValue(item.SoBuoi, out var entity))
            {
                entity.TieuDe = tieuDe;
                entity.NoiDung = noiDung;
                entity.MucTieu = mucTieu;
            }
            else
            {
                _dbContext.NoiDungBaiHocs.Add(new NoiDungBaiHoc
                {
                    MaKhoaHoc = maKhoaHoc,
                    SoBuoi = item.SoBuoi,
                    TieuDe = tieuDe,
                    NoiDung = noiDung,
                    MucTieu = mucTieu
                });
            }

            soDong++;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return soDong;
    }

    // ======================================================================
    // NGÀY LỄ
    // ======================================================================

    public async Task<IReadOnlyList<HolidayDto>> ListHolidaysAsync(CancellationToken cancellationToken)
        => await _dbContext.NgayLes
            .AsNoTracking()
            .OrderBy(x => x.Ngay)
            .Select(x => new HolidayDto
            {
                MaNgayLe = x.MaNgayLe,
                Ngay = x.Ngay,
                TenNgayLe = x.TenNgayLe,
                GhiChu = x.GhiChu
            })
            .ToListAsync(cancellationToken);

    public Task<HolidayDto?> GetHolidayAsync(int maNgayLe, CancellationToken cancellationToken)
        => _dbContext.NgayLes
            .AsNoTracking()
            .Where(x => x.MaNgayLe == maNgayLe)
            .Select(x => new HolidayDto
            {
                MaNgayLe = x.MaNgayLe,
                Ngay = x.Ngay,
                TenNgayLe = x.TenNgayLe,
                GhiChu = x.GhiChu
            })
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> HolidayDateExistsAsync(
        DateOnly ngay,
        int? exceptId,
        CancellationToken cancellationToken)
        => _dbContext.NgayLes
            .AsNoTracking()
            .AnyAsync(
                x => x.Ngay == ngay && (exceptId == null || x.MaNgayLe != exceptId),
                cancellationToken);

    public async Task<WriteResult> CreateHolidayAsync(
        CreateHolidayRequest request,
        CancellationToken cancellationToken)
    {
        var entity = new NgayLe
        {
            Ngay = request.Ngay,
            TenNgayLe = request.TenNgayLe.Trim(),
            GhiChu = TextValue.Required(request.GhiChu)
        };

        _dbContext.NgayLes.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsUniqueViolation(exception))
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
            return WriteResult.Conflict();
        }

        return WriteResult.Success(entity.MaNgayLe);
    }

    public async Task<WriteOutcome> UpdateHolidayAsync(
        int maNgayLe,
        UpdateHolidayRequest changes,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.NgayLes
            .FirstOrDefaultAsync(x => x.MaNgayLe == maNgayLe, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        if (changes.Ngay is not null)
        {
            entity.Ngay = changes.Ngay.Value;
        }

        if (changes.TenNgayLe is not null)
        {
            entity.TenNgayLe = changes.TenNgayLe.Trim();
        }

        if (changes.GhiChu is not null)
        {
            entity.GhiChu = TextValue.Required(changes.GhiChu);
        }

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsUniqueViolation(exception))
        {
            return WriteOutcome.Conflict;
        }

        return WriteOutcome.Success;
    }

    public async Task<WriteOutcome> DeleteHolidayAsync(int maNgayLe, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.NgayLes
            .FirstOrDefaultAsync(x => x.MaNgayLe == maNgayLe, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        _dbContext.NgayLes.Remove(entity);

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
