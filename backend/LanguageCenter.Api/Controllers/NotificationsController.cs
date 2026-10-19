using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Notifications;
using LanguageCenter.Application.Notifications.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Thông báo của CHÍNH người đang đăng nhập (mọi vai trò).
/// <para>
/// Phạm vi hiển thị: thông báo gửi trực tiếp cho tài khoản + thông báo gửi cho lớp
/// mà người dùng thuộc về (học viên: lớp đang học; giáo viên/trợ giảng: lớp được phân công).
/// </para>
/// </summary>
[ApiController]
[Route("api/notifications")]
[Authorize]
public sealed class NotificationsController : ControllerBase
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>Danh sách thông báo — lọc theo loại, khoảng thời gian, từ khóa.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<NotificationDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] NotificationListQuery query,
        CancellationToken cancellationToken)
        => (await _notificationService.ListForCurrentUserAsync(BuildActor(), query, cancellationToken))
            .ToActionResult();

    /// <summary>
    /// Tổng số thông báo người dùng hiện thấy được.
    /// <para>
    /// Schema <c>ThongBao</c> KHÔNG có cột theo dõi đã đọc/chưa đọc,
    /// nên <c>soChuaDoc</c> luôn là <c>null</c>.
    /// </para>
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(NotificationSummaryDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSummaryAsync(CancellationToken cancellationToken)
        => (await _notificationService.GetSummaryAsync(BuildActor(), cancellationToken))
            .ToActionResult();

    /// <summary>Chi tiết thông báo — không thuộc phạm vi của mình thì trả 404.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _notificationService.GetDetailForCurrentUserAsync(id, BuildActor(), cancellationToken))
            .ToActionResult();

    // ========================================================================
    // PRIVATE
    // ========================================================================

    private NotificationActor BuildActor() => new(
        User.GetMaTaiKhoan() ?? 0,
        User.GetTenDangNhap() ?? string.Empty,
        User.GetVaiTro() ?? string.Empty);
}
