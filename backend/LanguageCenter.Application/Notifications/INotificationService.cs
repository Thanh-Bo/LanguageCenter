using LanguageCenter.Application.Common;
using LanguageCenter.Application.Notifications.Models;

namespace LanguageCenter.Application.Notifications;

/// <summary>Thông báo — người dùng xem của mình, quản trị gửi và quản lý.</summary>
/// <remarks>
/// ⚠️ Schema <c>ThongBao</c> KHÔNG có cột đánh dấu đã đọc và không có bảng người nhận riêng.
/// Vì vậy Phase 7 KHÔNG cung cấp endpoint đánh dấu đã đọc / đếm chưa đọc.
/// </remarks>
public interface INotificationService
{
    /// <summary>Thông báo mà người đang đăng nhập được phép thấy.</summary>
    Task<OperationResult<PagedResult<NotificationDto>>> ListForCurrentUserAsync(
        NotificationActor actor,
        NotificationListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<NotificationDto>> GetDetailForCurrentUserAsync(
        int maThongBao,
        NotificationActor actor,
        CancellationToken cancellationToken);

    Task<OperationResult<NotificationSummaryDto>> GetSummaryAsync(
        NotificationActor actor,
        CancellationToken cancellationToken);

    // ------------------------------------------------------------------
    // QUẢN TRỊ
    // ------------------------------------------------------------------

    Task<OperationResult<PagedResult<NotificationDto>>> ListAsync(
        AdminNotificationListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<NotificationDto>> GetDetailAsync(
        int maThongBao,
        CancellationToken cancellationToken);

    Task<OperationResult<NotificationDto>> CreateAsync(
        CreateNotificationRequest request,
        NotificationActor actor,
        CancellationToken cancellationToken);

    Task<OperationResult<NotificationDto>> UpdateAsync(
        int maThongBao,
        UpdateNotificationRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<bool>> DeleteAsync(int maThongBao, CancellationToken cancellationToken);
}
