using System.Globalization;
using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.TuitionFees.Models;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="ITuitionFeeStore"/>
/// <remarks>
/// <see cref="HocPhi.SoTienPhaiDong"/> và <see cref="HocPhi.SoTienConThieu"/> là cột computed
/// PERSISTED — không bao giờ được gán giá trị, chỉ đọc ra để tổng hợp báo cáo.
/// </remarks>
public sealed class TuitionFeeStore : ITuitionFeeStore
{
    private readonly AppDbContext _dbContext;

    public TuitionFeeStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // ========================================================================
    // ĐỌC
    // ========================================================================

    public async Task<PagedResult<TuitionFeeListItemDto>> ListAsync(
        TuitionFeeListQuery query,
        CancellationToken cancellationToken)
    {
        var source = BuildFilteredQuery(query);

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await ProjectList(source
                .OrderBy(x => x.MaHocPhi)
                .Skip(query.Skip)
                .Take(query.Take))
            .ToListAsync(cancellationToken);

        return new PagedResult<TuitionFeeListItemDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public Task<TuitionFeeDetailDto?> GetDetailAsync(
        int maHocPhi,
        CancellationToken cancellationToken)
        => _dbContext.HocPhis
            .AsNoTracking()
            .Where(x => x.MaHocPhi == maHocPhi)
            .Select(x => new TuitionFeeDetailDto
            {
                MaHocPhi = x.MaHocPhi,
                MaDangKy = x.MaDangKy,
                MaHocVien = x.DangKyHoc.MaHocVien,
                HoTenHocVien = x.DangKyHoc.HocVien.HoTen,
                SoDienThoaiHocVien = x.DangKyHoc.HocVien.SoDienThoai,
                EmailHocVien = x.DangKyHoc.HocVien.Email,
                MaKhoaHoc = x.DangKyHoc.MaKhoaHoc,
                TenKhoaHoc = x.DangKyHoc.KhoaHoc.TenKhoaHoc,
                MaLopHoc = x.DangKyHoc.MaLopHoc,
                MaLop = x.DangKyHoc.LopHoc != null ? x.DangKyHoc.LopHoc.MaLop : null,
                TenLop = x.DangKyHoc.LopHoc != null ? x.DangKyHoc.LopHoc.TenLop : null,
                TrangThaiDangKy = x.DangKyHoc.TrangThaiDangKy,
                SoTienGoc = x.SoTienGoc,
                SoTienGiam = x.SoTienGiam,
                SoTienPhaiDong = x.SoTienPhaiDong,
                SoTienDaDong = x.SoTienDaDong,
                SoTienConThieu = x.SoTienConThieu,
                HanThanhToan = x.HanThanhToan,
                TrangThaiHocPhi = x.TrangThaiHocPhi,
                NgayTao = x.NgayTao,
                SoGiaoDich = x.ThanhToans.Count(
                    t => t.TrangThai == PaymentStatuses.ThanhCong
                         && t.LoaiGiaoDich == PaymentTypes.ThuTien)
            })
            .FirstOrDefaultAsync(cancellationToken);

    public Task<bool> ExistsAsync(int maHocPhi, CancellationToken cancellationToken)
        => _dbContext.HocPhis
            .AsNoTracking()
            .AnyAsync(x => x.MaHocPhi == maHocPhi, cancellationToken);

    public Task<int?> FindByEnrollmentAsync(int maDangKy, CancellationToken cancellationToken)
        => _dbContext.HocPhis
            .AsNoTracking()
            .Where(x => x.MaDangKy == maDangKy)
            .Select(x => (int?)x.MaHocPhi)
            .FirstOrDefaultAsync(cancellationToken);

    public Task<EnrollmentTuitionSnapshot?> GetEnrollmentSnapshotAsync(
        int maDangKy,
        CancellationToken cancellationToken)
        => _dbContext.DangKyHocs
            .AsNoTracking()
            .Where(x => x.MaDangKy == maDangKy)
            .Select(x => new EnrollmentTuitionSnapshot(
                x.MaDangKy,
                x.MaHocVien,
                x.MaKhoaHoc,
                x.MaLopHoc,
                x.TrangThaiDangKy,
                x.HocPhiGoc,
                x.SoTienGiam))
            .FirstOrDefaultAsync(cancellationToken);

    // ========================================================================
    // GHI
    // ========================================================================

    public async Task<WriteResult> CreateAsync(
        EnrollmentTuitionSnapshot snapshot,
        DateOnly? hanThanhToan,
        CancellationToken cancellationToken)
    {
        var soTienPhaiDong = snapshot.HocPhiGoc - snapshot.SoTienGiam;

        var entity = new HocPhi
        {
            MaDangKy = snapshot.MaDangKy,
            SoTienGoc = snapshot.HocPhiGoc,
            SoTienGiam = snapshot.SoTienGiam,
            SoTienDaDong = 0m,
            HanThanhToan = hanThanhToan,
            TrangThaiHocPhi = TuitionStatuses.FromAmounts(soTienPhaiDong, 0m),
            NgayTao = DateTime.Now
            // SoTienPhaiDong / SoTienConThieu: computed PERSISTED — KHÔNG gán
        };

        _dbContext.HocPhis.Add(entity);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsUniqueViolation(exception))
        {
            // MaDangKy là UNIQUE -> một yêu cầu khác vừa tạo trước.
            _dbContext.Entry(entity).State = EntityState.Detached;
            return WriteResult.Conflict();
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
            return WriteResult.Conflict();
        }

        return WriteResult.Success(entity.MaHocPhi);
    }

    public async Task<WriteOutcome> UpdateAsync(
        int maHocPhi,
        TuitionFeeUpdateData changes,
        CancellationToken cancellationToken)
    {
        var entity = await _dbContext.HocPhis
            .FirstOrDefaultAsync(x => x.MaHocPhi == maHocPhi, cancellationToken);

        if (entity is null)
        {
            return WriteOutcome.NotFound;
        }

        if (changes.CapNhatHanThanhToan)
        {
            entity.HanThanhToan = changes.HanThanhToan;
        }

        if (changes.CapNhatSoTien)
        {
            entity.SoTienGoc = changes.SoTienGoc;
            entity.SoTienGiam = changes.SoTienGiam;
            entity.TrangThaiHocPhi = TuitionStatuses.FromAmounts(
                changes.SoTienGoc - changes.SoTienGiam,
                entity.SoTienDaDong);
        }

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
        {
            // Ví dụ CK_HocPhi_SoTien: SoTienDaDong > (SoTienGoc - SoTienGiam).
            return WriteOutcome.Conflict;
        }

        return WriteOutcome.Success;
    }

    // ========================================================================
    // BÁO CÁO
    // ========================================================================

    public async Task<TuitionSummaryReportDto> GetSummaryReportAsync(
        TuitionReportQuery query,
        CancellationToken cancellationToken)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var source = _dbContext.HocPhis.AsNoTracking();

        if (query.TuNgay is { } tuNgay)
        {
            var from = tuNgay.ToDateTime(TimeOnly.MinValue);
            source = source.Where(x => x.NgayTao >= from);
        }

        if (query.DenNgay is { } denNgay)
        {
            var toExclusive = denNgay.ToDateTime(TimeOnly.MinValue).AddDays(1);
            source = source.Where(x => x.NgayTao < toExclusive);
        }

        if (query.MaKhoaHoc is { } maKhoaHoc)
        {
            source = source.Where(x => x.DangKyHoc.MaKhoaHoc == maKhoaHoc);
        }

        var tongSoHocPhi = await source.CountAsync(cancellationToken);

        var tongPhaiThu = await source
            .SumAsync(x => (decimal?)x.SoTienPhaiDong, cancellationToken) ?? 0m;

        var tongDaThu = await source
            .SumAsync(x => (decimal?)x.SoTienDaDong, cancellationToken) ?? 0m;

        var tongConThieu = await source
            .SumAsync(x => (decimal?)x.SoTienConThieu, cancellationToken) ?? 0m;

        var soChuaThanhToan = await source.CountAsync(
            x => x.TrangThaiHocPhi == TuitionStatuses.ChuaThanhToan, cancellationToken);

        var soThanhToanMotPhan = await source.CountAsync(
            x => x.TrangThaiHocPhi == TuitionStatuses.ThanhToanMotPhan, cancellationToken);

        var soDaThanhToan = await source.CountAsync(
            x => x.TrangThaiHocPhi == TuitionStatuses.DaThanhToan, cancellationToken);

        var soQuaHan = await source.CountAsync(
            x => x.SoTienConThieu > 0
                 && x.HanThanhToan != null
                 && x.HanThanhToan < today,
            cancellationToken);

        var conNoQuery = source.Where(x => x.SoTienConThieu > 0);

        var soHocVienConNo = await conNoQuery
            .Select(x => x.DangKyHoc.MaHocVien)
            .Distinct()
            .CountAsync(cancellationToken);

        var danhSachConNo = await conNoQuery
            .GroupBy(x => new { x.DangKyHoc.MaHocVien, x.DangKyHoc.HocVien.HoTen })
            .Select(g => new TuitionDebtorDto
            {
                MaHocVien = g.Key.MaHocVien,
                HoTen = g.Key.HoTen,
                SoHocPhiConNo = g.Count(),
                TongConThieu = g.Sum(x => x.SoTienConThieu)
            })
            .OrderByDescending(x => x.TongConThieu)
            .Take(50)
            .ToListAsync(cancellationToken);

        // ---- Thống kê giao dịch (loại trừ giao dịch không thành công khi tính tiền) ----
        var payments = _dbContext.ThanhToans.AsNoTracking();

        if (query.TuNgay is { } tuPt)
        {
            var fromPt = tuPt.ToDateTime(TimeOnly.MinValue);
            payments = payments.Where(x => x.NgayThanhToan >= fromPt);
        }

        if (query.DenNgay is { } denPt)
        {
            var toPt = denPt.ToDateTime(TimeOnly.MinValue).AddDays(1);
            payments = payments.Where(x => x.NgayThanhToan < toPt);
        }

        if (query.MaKhoaHoc is { } mkPt)
        {
            payments = payments.Where(x => x.HocPhi.DangKyHoc.MaKhoaHoc == mkPt);
        }

        var thongKeTheoPhuongThuc = await payments
            .Where(x => x.TrangThai == PaymentStatuses.ThanhCong
                        && x.LoaiGiaoDich == PaymentTypes.ThuTien)
            .GroupBy(x => x.PhuongThucThanhToan)
            .Select(g => new PaymentStatDto
            {
                Nhan = g.Key,
                SoLuong = g.Count(),
                TongTien = g.Sum(x => x.SoTien)
            })
            .OrderByDescending(x => x.TongTien)
            .ToListAsync(cancellationToken);

        var thongKeTheoTrangThai = await payments
            .GroupBy(x => x.TrangThai)
            .Select(g => new PaymentStatDto
            {
                Nhan = g.Key,
                SoLuong = g.Count(),
                TongTien = g.Sum(x => x.SoTien)
            })
            .OrderByDescending(x => x.SoLuong)
            .ToListAsync(cancellationToken);

        return new TuitionSummaryReportDto
        {
            TongSoHocPhi = tongSoHocPhi,
            TongPhaiThu = tongPhaiThu,
            TongDaThu = tongDaThu,
            TongConThieu = tongConThieu,
            SoChuaThanhToan = soChuaThanhToan,
            SoThanhToanMotPhan = soThanhToanMotPhan,
            SoDaThanhToan = soDaThanhToan,
            SoQuaHan = soQuaHan,
            SoHocVienConNo = soHocVienConNo,
            DanhSachConNo = danhSachConNo,
            ThongKeGiaoDichTheoPhuongThuc = thongKeTheoPhuongThuc,
            ThongKeGiaoDichTheoTrangThai = thongKeTheoTrangThai
        };
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    private IQueryable<HocPhi> BuildFilteredQuery(TuitionFeeListQuery query)
    {
        var source = _dbContext.HocPhis.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.MaHocVien))
        {
            var maHocVien = query.MaHocVien.Trim();
            source = source.Where(x => x.DangKyHoc.MaHocVien == maHocVien);
        }

        if (query.MaKhoaHoc is { } maKhoaHoc)
        {
            source = source.Where(x => x.DangKyHoc.MaKhoaHoc == maKhoaHoc);
        }

        if (query.MaLopHoc is { } maLopHoc)
        {
            source = source.Where(x => x.DangKyHoc.MaLopHoc == maLopHoc);
        }

        if (!string.IsNullOrWhiteSpace(query.TrangThai))
        {
            var trangThai = query.TrangThai.Trim();
            source = source.Where(x => x.TrangThaiHocPhi == trangThai);
        }

        if (query.QuaHan is { } quaHan)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            source = quaHan
                ? source.Where(x => x.SoTienConThieu > 0
                                    && x.HanThanhToan != null
                                    && x.HanThanhToan < today)
                : source.Where(x => !(x.SoTienConThieu > 0
                                      && x.HanThanhToan != null
                                      && x.HanThanhToan < today));
        }

        if (query.TuNgay is { } tuNgay)
        {
            var from = tuNgay.ToDateTime(TimeOnly.MinValue);
            source = source.Where(x => x.NgayTao >= from);
        }

        if (query.DenNgay is { } denNgay)
        {
            var toExclusive = denNgay.ToDateTime(TimeOnly.MinValue).AddDays(1);
            source = source.Where(x => x.NgayTao < toExclusive);
        }

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            source = source.Where(x => x.DangKyHoc.MaHocVien.Contains(search)
                                       || x.DangKyHoc.HocVien.HoTen.Contains(search));
        }

        return source;
    }

    private static IQueryable<TuitionFeeListItemDto> ProjectList(IQueryable<HocPhi> source)
        => source.Select(x => new TuitionFeeListItemDto
        {
            MaHocPhi = x.MaHocPhi,
            MaDangKy = x.MaDangKy,
            MaHocVien = x.DangKyHoc.MaHocVien,
            HoTenHocVien = x.DangKyHoc.HocVien.HoTen,
            MaKhoaHoc = x.DangKyHoc.MaKhoaHoc,
            TenKhoaHoc = x.DangKyHoc.KhoaHoc.TenKhoaHoc,
            MaLopHoc = x.DangKyHoc.MaLopHoc,
            MaLop = x.DangKyHoc.LopHoc != null ? x.DangKyHoc.LopHoc.MaLop : null,
            TenLop = x.DangKyHoc.LopHoc != null ? x.DangKyHoc.LopHoc.TenLop : null,
            SoTienGoc = x.SoTienGoc,
            SoTienGiam = x.SoTienGiam,
            SoTienPhaiDong = x.SoTienPhaiDong,
            SoTienDaDong = x.SoTienDaDong,
            SoTienConThieu = x.SoTienConThieu,
            HanThanhToan = x.HanThanhToan,
            TrangThaiHocPhi = x.TrangThaiHocPhi,
            NgayTao = x.NgayTao,
            SoGiaoDich = x.ThanhToans.Count(
                t => t.TrangThai == PaymentStatuses.ThanhCong
                     && t.LoaiGiaoDich == PaymentTypes.ThuTien)
        });
}
