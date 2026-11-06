using LanguageCenter.Application.Common;
using LanguageCenter.Application.Payments.Models;

namespace LanguageCenter.Application.Payments;

/// <summary>Ghi nhận và tra cứu giao dịch thanh toán.</summary>
public interface IPaymentService
{
    Task<OperationResult<PagedResult<PaymentDto>>> ListAsync(
        PaymentListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<PaymentDto>> GetDetailAsync(
        int maThanhToan,
        CancellationToken cancellationToken);

    Task<OperationResult<PagedResult<PaymentDto>>> ListByTuitionAsync(
        int maHocPhi,
        PaymentListQuery query,
        CancellationToken cancellationToken);

    /// <summary>
    /// Ghi nhận thanh toán. Idempotent theo <c>maGiaoDich</c>:
    /// gửi lại cùng mã sẽ trả về giao dịch đã có (200) thay vì tạo mới.
    /// </summary>
    Task<OperationResult<PaymentDto>> RecordAsync(
        int maHocPhi,
        RecordPaymentRequest request,
        PaymentActor actor,
        CancellationToken cancellationToken);

    // ------------------------------------------------------------------
    // HỌC VIÊN — chỉ đọc, chỉ dữ liệu của chính mình
    // ------------------------------------------------------------------

    Task<OperationResult<PagedResult<PaymentDto>>> ListForStudentAsync(
        string maHocVien,
        PaymentListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<PaymentDto>> GetDetailForStudentAsync(
        int maThanhToan,
        string maHocVien,
        CancellationToken cancellationToken);

    /// <summary>
    /// Học viên TỰ thanh toán học phí của chính mình — MÔ PHỎNG, không kết nối cổng thanh toán.
    /// <para>
    /// Người ghi nhận (<c>MaNhanVienKeToan</c>) được gán cố định cho nhân viên kế toán hệ thống;
    /// chỉ chấp nhận phiếu học phí thuộc chính học viên trong token.
    /// </para>
    /// </summary>
    Task<OperationResult<PaymentDto>> PayByStudentAsync(
        int maHocPhi,
        string maHocVien,
        StudentPayRequest request,
        CancellationToken cancellationToken);
}
