using System.Globalization;
using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Payments.Models;
using LanguageCenter.Domain.Constants;
using LanguageCenter.Domain.Entities;
using LanguageCenter.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <inheritdoc cref="IPaymentStore"/>
/// <remarks>
/// <para>
/// Ghi nhận thanh toán chạy trong MỘT transaction có khoá dòng <c>HocPhi</c>
/// (<c>WITH (UPDLOCK, HOLDLOCK)</c>) — nhờ đó nhiều request đồng thời trên cùng một
/// nghĩa vụ học phí được tuần tự hoá và số dư không thể bị tính sai.
/// </para>
/// <para>
/// Số biên nhận sinh theo mẫu <c>BN + yyyyMMdd + 3 chữ số</c>, đọc bản ghi lớn nhất
/// theo tiền tố (KHÔNG dùng <c>MAX(id) + 1</c>) và dựa vào UNIQUE index
/// <c>SoBienNhan</c> để chống trùng khi đồng thời.
/// </para>
/// </remarks>
public sealed class PaymentStore : IPaymentStore
{
    private const string ReceiptNumberPrefix = "BN";
    private const int MaxAttempts = 5;

    private readonly AppDbContext _dbContext;

    public PaymentStore(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // ========================================================================
    // ĐỌC
    // ========================================================================

    public async Task<PagedResult<PaymentDto>> ListAsync(
        PaymentListQuery query,
        CancellationToken cancellationToken)
    {
        var source = BuildFilteredQuery(query);

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await ProjectList(source
                .OrderByDescending(x => x.MaThanhToan)
                .Skip(query.Skip)
                .Take(query.Take))
            .ToListAsync(cancellationToken);

        return new PagedResult<PaymentDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    public Task<PaymentDto?> GetDetailAsync(int maThanhToan, CancellationToken cancellationToken)
        => ProjectList(_dbContext.ThanhToans.AsNoTracking().Where(x => x.MaThanhToan == maThanhToan))
            .FirstOrDefaultAsync(cancellationToken);

    public Task<int?> FindByTransactionCodeAsync(
        string maGiaoDich,
        CancellationToken cancellationToken)
        => _dbContext.ThanhToans
            .AsNoTracking()
            .Where(x => x.MaGiaoDich == maGiaoDich)
            .Select(x => (int?)x.MaThanhToan)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<PagedResult<PaymentDto>> ListByTuitionAsync(
        int maHocPhi,
        PaymentListQuery query,
        CancellationToken cancellationToken)
    {
        var source = BuildFilteredQuery(query).Where(x => x.MaHocPhi == maHocPhi);

        var totalItems = await source.CountAsync(cancellationToken);

        var items = await ProjectList(source
                .OrderByDescending(x => x.MaThanhToan)
                .Skip(query.Skip)
                .Take(query.Take))
            .ToListAsync(cancellationToken);

        return new PagedResult<PaymentDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = totalItems
        };
    }

    // ========================================================================
    // GHI NHẬN THANH TOÁN
    // ========================================================================

    public async Task<PaymentWriteResult> RecordAsync(
        PaymentWriteData data,
        CancellationToken cancellationToken)
    {
        // Đường nhanh: mã giao dịch đã tồn tại -> trả luôn bản ghi cũ (idempotent).
        var existingId = await FindByTransactionCodeAsync(data.MaGiaoDich, cancellationToken);

        if (existingId is not null)
        {
            return new PaymentWriteResult(PaymentWriteStatus.Duplicate, existingId.Value, 0m);
        }

        // EnableRetryOnFailure cấm BeginTransaction trực tiếp -> bọc ExecutionStrategy.
        var strategy = _dbContext.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(() => RecordCoreAsync(data, cancellationToken));
    }

    private async Task<PaymentWriteResult> RecordCoreAsync(
        PaymentWriteData data,
        CancellationToken cancellationToken)
    {
        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                // Khoá dòng học phí: mọi giao dịch đồng thời trên cùng nghĩa vụ phải xếp hàng.
                var hocPhi = await _dbContext.HocPhis
                    .FromSqlInterpolated(
                        $"SELECT * FROM HocPhi WITH (UPDLOCK, HOLDLOCK) WHERE MaHocPhi = {data.MaHocPhi}")
                    .FirstOrDefaultAsync(cancellationToken);

                if (hocPhi is null)
                {
                    await transaction.RollbackAsync(cancellationToken);

                    return new PaymentWriteResult(PaymentWriteStatus.TuitionNotFound, 0, 0m);
                }

                // Kiểm tra lại mã giao dịch SAU khi đã khoá.
                var duplicateId = await _dbContext.ThanhToans
                    .AsNoTracking()
                    .Where(x => x.MaGiaoDich == data.MaGiaoDich)
                    .Select(x => (int?)x.MaThanhToan)
                    .FirstOrDefaultAsync(cancellationToken);

                if (duplicateId is not null)
                {
                    await transaction.RollbackAsync(cancellationToken);

                    return new PaymentWriteResult(PaymentWriteStatus.Duplicate, duplicateId.Value, 0m);
                }

                // SoTienPhaiDong / SoTienConThieu là computed -> tự tính để không phụ thuộc refresh.
                var soTienPhaiDong = hocPhi.SoTienGoc - hocPhi.SoTienGiam;
                var soTienConThieu = soTienPhaiDong - hocPhi.SoTienDaDong;

                if (soTienConThieu < 0m)
                {
                    soTienConThieu = 0m;
                }

                if (data.SoTien > soTienConThieu)
                {
                    await transaction.RollbackAsync(cancellationToken);

                    return new PaymentWriteResult(
                        PaymentWriteStatus.ExceedsBalance,
                        0,
                        soTienConThieu);
                }

                var now = DateTime.Now;

                var thanhToan = new ThanhToan
                {
                    MaHocPhi = hocPhi.MaHocPhi,
                    MaNhanVienKeToan = data.MaNhanVienKeToan,
                    SoTien = data.SoTien,
                    NgayThanhToan = now,
                    PhuongThucThanhToan = data.PhuongThucThanhToan,
                    MaGiaoDich = data.MaGiaoDich,
                    LoaiGiaoDich = data.LoaiGiaoDich,
                    TrangThai = PaymentStatuses.ThanhCong,
                    GhiChu = data.GhiChu
                };

                _dbContext.ThanhToans.Add(thanhToan);

                hocPhi.SoTienDaDong += data.SoTien;
                hocPhi.TrangThaiHocPhi = TuitionStatuses.FromAmounts(
                    soTienPhaiDong,
                    hocPhi.SoTienDaDong);

                var soBienNhan = await NextReceiptNumberAsync(now, cancellationToken);

                _dbContext.BienNhans.Add(new BienNhan
                {
                    ThanhToan = thanhToan,
                    MaNhanVienLap = data.MaNhanVienKeToan,
                    SoBienNhan = soBienNhan,
                    NgayLap = now,
                    SoTienXacNhan = data.SoTien,
                    NoiDung = data.NoiDungBienNhan ?? "Thu học phí",
                    TrangThai = ReceiptStatuses.DaPhatHanh
                });

                await _dbContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                var conLai = soTienPhaiDong - hocPhi.SoTienDaDong;

                return new PaymentWriteResult(
                    PaymentWriteStatus.Success,
                    thanhToan.MaThanhToan,
                    conLai < 0m ? 0m : conLai);
            }
            catch (DbUpdateException exception) when (SqlErrorHelper.IsUniqueViolation(exception))
            {
                // Trùng SoBienNhan (hoặc MaGiaoDich) do request đồng thời -> thử lại.
                await SafeRollbackAsync(transaction, cancellationToken);
                _dbContext.ChangeTracker.Clear();
            }
            catch (DbUpdateException exception) when (SqlErrorHelper.IsConstraintViolation(exception))
            {
                await SafeRollbackAsync(transaction, cancellationToken);
                _dbContext.ChangeTracker.Clear();

                return new PaymentWriteResult(PaymentWriteStatus.Conflict, 0, 0m);
            }
        }

        return new PaymentWriteResult(PaymentWriteStatus.Conflict, 0, 0m);
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    private IQueryable<ThanhToan> BuildFilteredQuery(PaymentListQuery query)
    {
        var source = _dbContext.ThanhToans.AsNoTracking();

        if (query.MaHocPhi is { } maHocPhi)
        {
            source = source.Where(x => x.MaHocPhi == maHocPhi);
        }

        if (!string.IsNullOrWhiteSpace(query.MaHocVien))
        {
            var maHocVien = query.MaHocVien.Trim();
            source = source.Where(x => x.HocPhi.DangKyHoc.MaHocVien == maHocVien);
        }

        if (query.MaKhoaHoc is { } maKhoaHoc)
        {
            source = source.Where(x => x.HocPhi.DangKyHoc.MaKhoaHoc == maKhoaHoc);
        }

        if (!string.IsNullOrWhiteSpace(query.PhuongThucThanhToan))
        {
            var phuongThuc = query.PhuongThucThanhToan.Trim();
            source = source.Where(x => x.PhuongThucThanhToan == phuongThuc);
        }

        if (!string.IsNullOrWhiteSpace(query.TrangThai))
        {
            var trangThai = query.TrangThai.Trim();
            source = source.Where(x => x.TrangThai == trangThai);
        }

        if (!string.IsNullOrWhiteSpace(query.LoaiGiaoDich))
        {
            var loai = query.LoaiGiaoDich.Trim();
            source = source.Where(x => x.LoaiGiaoDich == loai);
        }

        if (query.TuNgay is { } tuNgay)
        {
            var from = tuNgay.ToDateTime(TimeOnly.MinValue);
            source = source.Where(x => x.NgayThanhToan >= from);
        }

        if (query.DenNgay is { } denNgay)
        {
            var toExclusive = denNgay.ToDateTime(TimeOnly.MinValue).AddDays(1);
            source = source.Where(x => x.NgayThanhToan < toExclusive);
        }

        return source;
    }

    private static IQueryable<PaymentDto> ProjectList(IQueryable<ThanhToan> source)
        => source.Select(x => new PaymentDto
        {
            MaThanhToan = x.MaThanhToan,
            MaHocPhi = x.MaHocPhi,
            MaDangKy = x.HocPhi.MaDangKy,
            MaHocVien = x.HocPhi.DangKyHoc.MaHocVien,
            HoTenHocVien = x.HocPhi.DangKyHoc.HocVien.HoTen,
            MaKhoaHoc = x.HocPhi.DangKyHoc.MaKhoaHoc,
            TenKhoaHoc = x.HocPhi.DangKyHoc.KhoaHoc.TenKhoaHoc,
            SoTien = x.SoTien,
            NgayThanhToan = x.NgayThanhToan,
            PhuongThucThanhToan = x.PhuongThucThanhToan,
            MaGiaoDich = x.MaGiaoDich,
            LoaiGiaoDich = x.LoaiGiaoDich,
            TrangThai = x.TrangThai,
            GhiChu = x.GhiChu,
            MaNhanVienKeToan = x.MaNhanVienKeToan,
            TenNhanVienKeToan = x.NhanVienKeToan.HoTen,
            BienNhan = x.BienNhan != null
                ? new PaymentReceiptBriefDto
                {
                    MaBienNhan = x.BienNhan.MaBienNhan,
                    SoBienNhan = x.BienNhan.SoBienNhan,
                    NgayLap = x.BienNhan.NgayLap,
                    TrangThai = x.BienNhan.TrangThai
                }
                : null
        });

    /// <summary>Sinh số biên nhận kế tiếp cho ngày hiện tại (đọc theo tiền tố, KHÔNG dùng MAX(id)).</summary>
    private async Task<string> NextReceiptNumberAsync(DateTime now, CancellationToken cancellationToken)
    {
        var prefix = ReceiptNumberPrefix + now.ToString("yyyyMMdd", CultureInfo.InvariantCulture);

        var last = await _dbContext.BienNhans
            .AsNoTracking()
            .Where(x => x.SoBienNhan.StartsWith(prefix))
            .OrderByDescending(x => x.SoBienNhan)
            .Select(x => x.SoBienNhan)
            .FirstOrDefaultAsync(cancellationToken);

        var next = 1;

        if (last is not null
            && last.Length > prefix.Length
            && int.TryParse(last.AsSpan(prefix.Length), NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed))
        {
            next = parsed + 1;
        }

        return prefix + next.ToString("000", CultureInfo.InvariantCulture);
    }

    private static async Task SafeRollbackAsync(
        Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction,
        CancellationToken cancellationToken)
    {
        try
        {
            await transaction.RollbackAsync(cancellationToken);
        }
        catch
        {
            // Bỏ qua: transaction có thể đã bị huỷ do lỗi trước đó.
        }
    }
}
