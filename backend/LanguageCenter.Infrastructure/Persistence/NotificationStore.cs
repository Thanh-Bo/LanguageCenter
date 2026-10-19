using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Notifications.Models;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="INotificationStore"/>
public sealed class NotificationStore : INotificationStore
{
    private readonly AppDbContext _dbContext;

    public NotificationStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // ========================================================================
    // NGƯỜI DÙNG — chỉ thấy thông báo thuộc phạm vi của mình
    // ========================================================================

    public async Task<PagedResult<NotificationDto>> ListForAudienceAsync(
        NotificationAudience audience,
        NotificationListQuery query,
        CancellationToken cancellationToken)
    {
        var source = BuildAudienceQuery(audience);

        if (!string.IsNullOrWhiteSpace(query.LoaiThongBao))
        {
            var loai = query.LoaiThongBao.Trim();
            source = source.Where(x => x.LoaiThongBao == loai);
        }

        if (query.TuNgay is { } tuNgay)
        {
            var from = tuNgay.ToDateTime(TimeOnly.MinValue);
            source = source.Where(x => x.ThoiGianGui >= from);
        }

        if (query.DenNgay is { } denNgay)
        {
            var toExclusive = denNgay.ToDateTime(TimeOnly.MinValue).AddDays(1);
            source = source.Where(x => x.ThoiGianGui < toExclusive);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            source = source.Where(x => x.TieuDe.Contains(search) || x.NoiDung.Contains(search));
        }

        return await ProjectPageAsync(source, query.Skip, query.Take, query.Page, query.PageSize, cancellationToken);
    }

    public Task<int> CountForAudienceAsync(
        NotificationAudience audience,
        CancellationToken cancellationToken)
        => BuildAudienceQuery(audience).CountAsync(cancellationToken);

    public Task<NotificationDto?> GetDetailForAudienceAsync(
        int maThongBao,
        NotificationAudience audience,
        CancellationToken cancellationToken)
        => Project(BuildAudienceQuery(audience).Where(x => x.MaThongBao == maThongBao))
            .FirstOrDefaultAsync(cancellationToken);

    // ========================================================================
    // QUẢN TRỊ
    // ========================================================================

    public async Task<PagedResult<NotificationDto>> ListAsync(
        AdminNotificationListQuery query,
        CancellationToken cancellationToken)
    {
        var source = _dbContext.ThongBaos.AsNoTracking();

        if (query.MaTaiKhoanGui is { } maGui)
        {
            source = source.Where(x => x.MaTaiKhoanGui == maGui);
        }

        if (query.MaTaiKhoanNhan is { } maNhan)
        {
            source = source.Where(x => x.MaTaiKhoanNhan == maNhan);
        }

        if (query.MaLopHoc is { } maLop)
        {
            source = source.Where(x => x.MaLopHoc == maLop);
        }

        if (!string.IsNullOrWhiteSpace(query.TrangThai))
        {
            var trangThai = query.TrangThai.Trim();
            source = source.Where(x => x.TrangThai == trangThai);
        }

        if (!string.IsNullOrWhiteSpace(query.LoaiThongBao))
        {
            var loai = query.LoaiThongBao.Trim();
            source = source.Where(x => x.LoaiThongBao == loai);
        }

        if (query.TuNgay is { } tuNgay)
        {
            var from = tuNgay.ToDateTime(TimeOnly.MinValue);
            source = source.Where(x => x.ThoiGianGui >= from);
        }

        if (query.DenNgay is { } denNgay)
        {
            var toExclusive = denNgay.ToDateTime(TimeOnly.MinValue).AddDays(1);
            source = source.Where(x => x.ThoiGianGui < toExclusive);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            source = source.Where(x => x.TieuDe.Contains(search) || x.NoiDung.Contains(search));
        }

        return await ProjectPageAsync(source, query.Skip, query.Take, query.Page, query.PageSize, cancellationToken);
    }

    public Task<NotificationDto?> GetDetailAsync(
        int maThongBao,
        CancellationToken cancellationToken)
        => Project(_dbContext.ThongBaos.AsNoTracking().Where(x => x.MaThongBao == maThongBao))
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> ExistsAsync(int maThongBao, CancellationToken cancellationToken)
        => _dbContext.ThongBaos
            .AsNoTracking()
            .AnyAsync(x => x.MaThongBao == maThongBao, cancellationToken);

    // ========================================================================
    // GHI
    // ========================================================================

    public async Task<WriteResult> CreateAsync(
        CreateNotificationData data,
        CancellationToken cancellationToken)
    {
        var entity = new ThongBao
        {
            MaTaiKhoanGui = data.MaTaiKhoanGui,
            MaTaiKhoanNhan = data.MaTaiKhoanNhan,
            MaLopHoc = data.MaLopHoc,
            TieuDe = data.TieuDe,
            NoiDung = data.NoiDung,
            LoaiThongBao = data.LoaiThongBao,
            ThoiGianGui = DateTime.Now,
            TrangThai = NotificationStatuses.DaGui
        };

        _dbContext.ThongBaos.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
            return WriteResult.Conflict();
        }

        return WriteResult.Success(entity.MaThongBao);
    }

    public async Task<WriteOutcome> UpdateAsync(
        int maThongBao,
        NotificationUpdateData changes,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.ThongBaos
            .FirstOrDefaultAsync(x => x.MaThongBao == maThongBao, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        if (changes.CapNhatTieuDe && changes.TieuDe is not null)
        {
            entity.TieuDe = changes.TieuDe;
        }

        if (changes.CapNhatNoiDung && changes.NoiDung is not null)
        {
            entity.NoiDung = changes.NoiDung;
        }

        if (changes.CapNhatLoai)
        {
            entity.LoaiThongBao = changes.LoaiThongBao;
        }

        if (changes.TrangThai is not null)
        {
            entity.TrangThai = changes.TrangThai;
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

    public async Task<WriteOutcome> DeleteAsync(int maThongBao, CancellationToken cancellationToken)
    {
        var entity = await _dbContext.ThongBaos
            .FirstOrDefaultAsync(x => x.MaThongBao == maThongBao, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        _dbContext.ThongBaos.Remove(entity);

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

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    /// <summary>
    /// Điều kiện hiển thị: gửi trực tiếp cho tôi, HOẶC gửi cho lớp của tôi.
    /// Thông báo đã thu hồi không hiển thị.
    /// </summary>
    private IQueryable<ThongBao> BuildAudienceQuery(NotificationAudience audience)
    {
        var source = _dbContext.ThongBaos
            .AsNoTracking()
            .Where(x => x.TrangThai != NotificationStatuses.DaThuHoi);

        if (audience.MaTaiKhoan <= 0)
        {
            return source.Where(_ => false);
        }

        var maTaiKhoan = audience.MaTaiKhoan;

        if (!audience.IncludeClassBroadcasts || audience.ClassIds.Count == 0)
        {
            return source.Where(x => x.MaTaiKhoanNhan == maTaiKhoan);
        }

        var classIds = audience.ClassIds.ToList();

        return source.Where(x => x.MaTaiKhoanNhan == maTaiKhoan
                                 || (x.MaLopHoc != null && classIds.Contains(x.MaLopHoc.Value)));
    }

    private async Task<PagedResult<NotificationDto>> ProjectPageAsync(
        IQueryable<ThongBao> source,
        int skip,
        int take,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var totalItems = await source.CountAsync(cancellationToken);

        var items = await Project(source
                .OrderByDescending(x => x.ThoiGianGui)
                .ThenByDescending(x => x.MaThongBao)
                .Skip(skip)
                .Take(take))
            .ToListAsync(cancellationToken);

        return new PagedResult<NotificationDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    private static IQueryable<NotificationDto> Project(IQueryable<ThongBao> source)
        => source.Select(x => new NotificationDto
        {
            MaThongBao = x.MaThongBao,
            MaTaiKhoanGui = x.MaTaiKhoanGui,
            TenDangNhapNguoiGui = x.TaiKhoanGui.TenDangNhap,
            MaTaiKhoanNhan = x.MaTaiKhoanNhan,
            TenDangNhapNguoiNhan = x.TaiKhoanNhan != null ? x.TaiKhoanNhan.TenDangNhap : null,
            MaLopHoc = x.MaLopHoc,
            MaLop = x.LopHoc != null ? x.LopHoc.MaLop : null,
            TenLop = x.LopHoc != null ? x.LopHoc.TenLop : null,
            TieuDe = x.TieuDe,
            NoiDung = x.NoiDung,
            LoaiThongBao = x.LoaiThongBao,
            ThoiGianGui = x.ThoiGianGui,
            TrangThai = x.TrangThai
        });
}
