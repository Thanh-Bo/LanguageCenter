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
                TenGiaoVien = x.GiaoVien == null ? null : x.GiaoVien.HoTen,
                LoaiGiaoVien = x.GiaoVien == null ? null : x.GiaoVien.LoaiGiaoVien,
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
                TenGiaoVien = x.GiaoVien == null ? null : x.GiaoVien.HoTen,
                LoaiGiaoVien = x.GiaoVien == null ? null : x.GiaoVien.LoaiGiaoVien,
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

    public Task<bool> HasSamePersonOverlapAsync(
        int maLopHoc,
        string maGiaoVien,
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
                     && x.TrangThai != AssignmentStatuses.DaHuy
                     && x.NgayBatDau <= end
                     && (x.NgayKetThuc ?? MaxDate) >= ngayBatDau
                     && (exceptId == null || x.MaPhanCong != exceptId),
                cancellationToken);
    }

    /// <inheritdoc />
    /// <remarks>
    /// So sánh trong bộ nhớ: số dòng <c>LichHoc</c> của một lớp rất nhỏ (1–3 dòng),
    /// đổi lại truy vấn rõ ràng và không phụ thuộc khả năng dịch LINQ phức tạp của EF.
    /// </remarks>
    public async Task<IReadOnlyList<TeachingScheduleConflict>> FindScheduleConflictsAsync(
        int maLopHoc,
        string maGiaoVien,
        DateOnly ngayBatDau,
        DateOnly? ngayKetThuc,
        int? exceptId,
        CancellationToken cancellationToken)
    {
        var code = maGiaoVien.Trim();
        var end = ngayKetThuc ?? MaxDate;

        var lichCuaLop = await _dbContext.LichHocs
            .AsNoTracking()
            .Where(l => l.MaLopHoc == maLopHoc && l.TrangThai != ScheduleStatuses.NgungApDung)
            .Select(l => new
            {
                l.ThuTrongTuan,
                l.GioBatDau,
                l.GioKetThuc,
                l.NgayApDungTu,
                l.NgayApDungDen
            })
            .ToListAsync(cancellationToken);

        if (lichCuaLop.Count == 0)
        {
            return Array.Empty<TeachingScheduleConflict>();
        }

        // Các lớp khác mà người này đang được phân công trong khoảng ngày chồng lấn.
        var lopKhac = await _dbContext.PhanCongGiangDays
            .AsNoTracking()
            .Where(x => x.MaGiaoVien == code
                        && x.MaLopHoc != maLopHoc
                        && x.TrangThai != AssignmentStatuses.DaHuy
                        && x.NgayBatDau <= end
                        && (x.NgayKetThuc ?? MaxDate) >= ngayBatDau
                        && (exceptId == null || x.MaPhanCong != exceptId))
            .Select(x => new { x.MaLopHoc, x.LopHoc.MaLop, x.LopHoc.TenLop })
            .Distinct()
            .ToListAsync(cancellationToken);

        if (lopKhac.Count == 0)
        {
            return Array.Empty<TeachingScheduleConflict>();
        }

        var maCacLopKhac = lopKhac.Select(x => x.MaLopHoc).ToList();

        var lichLopKhac = await _dbContext.LichHocs
            .AsNoTracking()
            .Where(l => maCacLopKhac.Contains(l.MaLopHoc) && l.TrangThai != ScheduleStatuses.NgungApDung)
            .Select(l => new
            {
                l.MaLopHoc,
                l.ThuTrongTuan,
                l.GioBatDau,
                l.GioKetThuc,
                l.NgayApDungTu,
                l.NgayApDungDen
            })
            .ToListAsync(cancellationToken);

        var tenLop = lopKhac
            .GroupBy(x => x.MaLopHoc)
            .ToDictionary(g => g.Key, g => (g.First().MaLop, g.First().TenLop));

        var conflicts = new List<TeachingScheduleConflict>();

        foreach (var a in lichCuaLop)
        {
            var aDen = a.NgayApDungDen ?? MaxDate;

            foreach (var b in lichLopKhac)
            {
                if (a.ThuTrongTuan != b.ThuTrongTuan)
                {
                    continue;
                }

                var bDen = b.NgayApDungDen ?? MaxDate;

                // Khoảng ngày áp dụng phải chồng lấn.
                if (a.NgayApDungTu > bDen || b.NgayApDungTu > aDen)
                {
                    continue;
                }

                // Giờ phải chồng lấn (biên bằng nhau không tính là trùng).
                if (a.GioBatDau >= b.GioKetThuc || b.GioBatDau >= a.GioKetThuc)
                {
                    continue;
                }

                var info = tenLop[b.MaLopHoc];

                conflicts.Add(new TeachingScheduleConflict(
                    info.MaLop,
                    info.TenLop,
                    a.ThuTrongTuan,
                    b.GioBatDau,
                    b.GioKetThuc));
            }
        }

        return conflicts
            .GroupBy(c => (c.MaLop, c.ThuTrongTuan, c.GioBatDau, c.GioKetThuc))
            .Select(g => g.First())
            .ToList();
    }

    public Task<bool> PlaceholderExistsAsync(
        int maLopHoc,
        string vaiTroPhanCong,
        CancellationToken cancellationToken)
        => _dbContext.PhanCongGiangDays
            .AsNoTracking()
            .AnyAsync(
                x => x.MaLopHoc == maLopHoc
                     && x.VaiTroPhanCong == vaiTroPhanCong
                     && x.MaGiaoVien == null
                     && x.TrangThai == AssignmentStatuses.ChoPhanCong,
                cancellationToken);

    public async Task<WriteResult> CreateAsync(
        CreateAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        var maGiaoVien = TextValue.Optional(request.MaGiaoVien);

        var entity = new PhanCongGiangDay
        {
            MaLopHoc = request.MaLopHoc,
            MaGiaoVien = maGiaoVien,
            VaiTroPhanCong = request.VaiTroPhanCong.Trim(),
            NgayBatDau = request.NgayBatDau,
            NgayKetThuc = request.NgayKetThuc,
            GhiChu = TextValue.Optional(request.GhiChu),
            TrangThai = maGiaoVien is null
                ? AssignmentStatuses.ChoPhanCong
                : AssignmentStatuses.DangPhanCong
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
            var maGiaoVienMoi = TextValue.Optional(changes.MaGiaoVien);
            entity.MaGiaoVien = maGiaoVienMoi;

            // Gắn người → chuyển "Chờ phân công" sang "Đang phân công";
            // Bỏ người → quay lại "Chờ phân công" (giữ nguyên nếu đã kết thúc / đã hủy).
            if (entity.TrangThai is AssignmentStatuses.DangPhanCong or AssignmentStatuses.ChoPhanCong)
            {
                entity.TrangThai = maGiaoVienMoi is null
                    ? AssignmentStatuses.ChoPhanCong
                    : AssignmentStatuses.DangPhanCong;
            }
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
