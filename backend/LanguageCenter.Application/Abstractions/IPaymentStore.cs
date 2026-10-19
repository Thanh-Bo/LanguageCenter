using LanguageCenter.Application.Common;
using LanguageCenter.Application.Payments.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>Cổng truy cập giao dịch thanh toán (<c>ThanhToan</c>).</summary>
public interface IPaymentStore
{
    Task<PagedResult<PaymentDto>> ListAsync(
        PaymentListQuery query,
        CancellationToken cancellationToken);

    Task<PaymentDto?> GetDetailAsync(int maThanhToan, CancellationToken cancellationToken);

    /// <summary>Giao dịch đã tồn tại với mã ngoài này chưa (idempotency).</summary>
    Task<int?> FindByTransactionCodeAsync(string maGiaoDich, CancellationToken cancellationToken);

    Task<PagedResult<PaymentDto>> ListByTuitionAsync(
        int maHocPhi,
        PaymentListQuery query,
        CancellationToken cancellationToken);

    /// <summary>
    /// Ghi nhận thanh toán NGUYÊN TỬ: khoá dòng <c>HocPhi</c>, kiểm tra số dư,
    /// cộng <c>SoTienDaDong</c>, đồng bộ <c>TrangThaiHocPhi</c> và tạo biên nhận
    /// trong CÙNG một transaction.
    /// </summary>
    Task<PaymentWriteResult> RecordAsync(
        PaymentWriteData data,
        CancellationToken cancellationToken);
}
