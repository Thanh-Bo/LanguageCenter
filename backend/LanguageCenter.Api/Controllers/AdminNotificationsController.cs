using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Notifications;
using LanguageCenter.Application.Notifications.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Quản trị thông báo.
/// <para>
/// Gửi thông báo: Quản trị viên, Nhân viên học vụ, Giáo viên, Trợ giảng
/// (Giáo viên/Trợ giảng chỉ được gửi cho lớp mình phụ trách).
/// Sửa/xóa/xem toàn bộ: Quản trị viên, Nhân viên học vụ.
/// </para>
/// </summary>
[ApiController]
[Route("api/admin/notifications")]
[Authorize]
public sealed class AdminNotificationsController : ControllerBase
{
    private const string ManageRoles = RoleNames.QuanTriVien + "," + RoleNames.NhanVienHocVu;

    private const string SendRoles = ManageRoles
                                     + ","
                                     + RoleNames.GiaoVien
                                     + ","
                                     + RoleNames.TroGiang;

    private readonly INotificationService _notificationService;

    public AdminNotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>Danh sách toàn bộ thông báo — lọc theo người gửi/nhận, lớp, trạng thái, loại, thời gian.</summary>
    [HttpGet]
    [Authorize(Roles = ManageRoles)]
    [ProducesResponseType(typeof(PagedResult<NotificationDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] AdminNotificationListQuery query,
        CancellationToken cancellationToken)
        => (await _notificationService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết thông báo (toàn bộ hệ thống).</summary>
    [HttpGet("{id:int}")]
    [Authorize(Roles = ManageRoles)]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _notificationService.GetDetailAsync(id, cancellationToken)).ToActionResult();

    /// <summary>
    /// Gửi thông báo. Phải có ít nhất một đích: <c>maTaiKhoanNhan</c> hoặc <c>maLopHoc</c>.
    /// <para>Người gửi luôn lấy từ JWT — client không gửi được.</para>
    /// </summary>
    [HttpPost]
    [Authorize(Roles = SendRoles)]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateNotificationRequest request,
        CancellationToken cancellationToken)
        => (await _notificationService.CreateAsync(request, BuildActor(), cancellationToken))
            .ToActionResult();

    /// <summary>Cập nhật tiêu đề/nội dung/loại/trạng thái. KHÔNG đổi được người gửi hay người nhận.</summary>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = ManageRoles)]
    [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] UpdateNotificationRequest request,
        CancellationToken cancellationToken)
        => (await _notificationService.UpdateAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>Xóa thông báo.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = ManageRoles)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        => (await _notificationService.DeleteAsync(id, cancellationToken)).ToActionResult();

    // ========================================================================
    // PRIVATE
    // ========================================================================

    private NotificationActor BuildActor() => new(
        User.GetMaTaiKhoan() ?? 0,
        User.GetTenDangNhap() ?? string.Empty,
        User.GetVaiTro() ?? string.Empty);
}
