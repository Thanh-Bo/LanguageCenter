using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Receipts.Models;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="IReceiptStore"/>
public sealed class ReceiptStore : IReceiptStore
{
    private readonly AppDbContext _dbContext;

    public ReceiptStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<PagedResult<ReceiptDto>> ListAsync(
        ReceiptListQuery query,
        CancellationToken cancellationToken)
    {
        var source = BuildFilteredQuery(query);

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await ProjectList(source
                .OrderByDescending(x => x.MaBienNhan)
                .Skip(query.Skip)
                .Take(query.Take))
            .ToListAsync(cancellationToken);

        return new PagedResult<ReceiptDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public Task<ReceiptDto?> GetDetailAsync(int maBienNhan, CancellationToken cancellationToken)
        => ProjectList(_dbContext.BienNhans.AsNoTracking().Where(x => x.MaBienNhan == maBienNhan))
            .FirstOrDefaultAsync(cancellationToken);

    public Task<ReceiptDto?> GetByNumberAsync(string soBienNhan, CancellationToken cancellationToken)
        => ProjectList(_dbContext.BienNhans.AsNoTracking().Where(x => x.SoBienNhan == soBienNhan))
            .FirstOrDefaultAsync(cancellationToken);

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    private IQueryable<BienNhan> BuildFilteredQuery(ReceiptListQuery query)
    {
        var source = _dbContext.BienNhans.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(query.SoBienNhan))
        {
            var soBienNhan = query.SoBienNhan.Trim();
            source = source.Where(x => x.SoBienNhan.Contains(soBienNhan));
        }

        if (!string.IsNullOrWhiteSpace(query.MaHocVien))
        {
            var maHocVien = query.MaHocVien.Trim();
            source = source.Where(x => x.ThanhToan.HocPhi.DangKyHoc.MaHocVien == maHocVien);
        }

        if (query.MaThanhToan is { } maThanhToan)
        {
            source = source.Where(x => x.MaThanhToan == maThanhToan);
        }

        if (!string.IsNullOrWhiteSpace(query.TrangThai))
        {
            var trangThai = query.TrangThai.Trim();
            source = source.Where(x => x.TrangThai == trangThai);
        }

        if (query.TuNgay is { } tuNgay)
        {
            var from = tuNgay.ToDateTime(TimeOnly.MinValue);
            source = source.Where(x => x.NgayLap >= from);
        }

        if (query.DenNgay is { } denNgay)
        {
            var toExclusive = denNgay.ToDateTime(TimeOnly.MinValue).AddDays(1);
            source = source.Where(x => x.NgayLap < toExclusive);
        }

        return source;
    }

    private static IQueryable<ReceiptDto> ProjectList(IQueryable<BienNhan> source)
        => source.Select(x => new ReceiptDto
        {
            MaBienNhan = x.MaBienNhan,
            SoBienNhan = x.SoBienNhan,
            NgayLap = x.NgayLap,
            SoTienXacNhan = x.SoTienXacNhan,
            NoiDung = x.NoiDung,
            TrangThai = x.TrangThai,
            MaNhanVienLap = x.MaNhanVienLap,
            TenNhanVienLap = x.NhanVienLap.HoTen,

            MaThanhToan = x.MaThanhToan,
            MaGiaoDich = x.ThanhToan.MaGiaoDich,
            PhuongThucThanhToan = x.ThanhToan.PhuongThucThanhToan,
            TrangThaiThanhToan = x.ThanhToan.TrangThai,
            NgayThanhToan = x.ThanhToan.NgayThanhToan,

            MaHocPhi = x.ThanhToan.MaHocPhi,
            MaDangKy = x.ThanhToan.HocPhi.MaDangKy,
            MaHocVien = x.ThanhToan.HocPhi.DangKyHoc.MaHocVien,
            HoTenHocVien = x.ThanhToan.HocPhi.DangKyHoc.HocVien.HoTen,
            MaKhoaHoc = x.ThanhToan.HocPhi.DangKyHoc.MaKhoaHoc,
            TenKhoaHoc = x.ThanhToan.HocPhi.DangKyHoc.KhoaHoc.TenKhoaHoc
        });
}
