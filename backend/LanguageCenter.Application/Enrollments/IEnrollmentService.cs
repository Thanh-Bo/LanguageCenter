using LanguageCenter.Application.Common;
using LanguageCenter.Application.Enrollments.Models;

namespace LanguageCenter.Application.Enrollments;

/// <summary>Quản lý đăng ký học và xếp lớp.</summary>
public interface IEnrollmentService
{
    Task<OperationResult<PagedResult<EnrollmentListItemDto>>> ListAsync(
        EnrollmentListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<EnrollmentDetailDto>> GetDetailAsync(
        int maDangKy,
        CancellationToken cancellationToken);

    Task<OperationResult<EnrollmentDetailDto>> CreateAsync(
        CreateEnrollmentRequest request,
        string currentUserTenDangNhap,
        CancellationToken cancellationToken);

    Task<OperationResult<EnrollmentDetailDto>> UpdateAsync(
        int maDangKy,
        UpdateEnrollmentRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<EnrollmentDetailDto>> AssignClassAsync(
        int maDangKy,
        AssignClassRequest request,
        CancellationToken cancellationToken);

    /// <summary>Bỏ lớp: trả đăng ký về trạng thái "Chờ xử lý" để xếp lại lớp khác.</summary>
    Task<OperationResult<EnrollmentDetailDto>> UnassignClassAsync(
        int maDangKy,
        CancellationToken cancellationToken);

    Task<OperationResult<EnrollmentDetailDto>> UpdateStatusAsync(
        int maDangKy,
        UpdateStatusRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// TỰ XẾP LỚP nếu học phí của đăng ký đã thu ĐỦ (Còn nợ = 0).
    /// <para>
    /// Gọi ngay sau khi ghi nhận thanh toán. Trả về <c>null</c> khi không có lỗi
    /// (hoặc chưa đủ tiền / đã ở trong lớp), trả về chuỗi lý do khi không xếp được
    /// (lớp đầy, quá 4 buổi, trùng giờ, sai tuổi) — đăng ký giữ nguyên "Chờ xử lý".
    /// </para>
    /// </summary>
    Task<string?> TryAutoPlaceIfPaidAsync(int maDangKy, CancellationToken cancellationToken);
}
