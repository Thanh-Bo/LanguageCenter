using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Schedules;
using LanguageCenter.Application.Schedules.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Lịch học định kỳ — Quản trị viên và Nhân viên học vụ.
/// <para>Lịch định kỳ KHÔNG phải buổi học thực tế; điểm danh ghi trên <c>CaHoc</c>.</para>
/// </summary>
[ApiController]
[Route("api/admin/schedules")]
[Authorize(Roles = RoleNames.QuanTriVien + "," + RoleNames.NhanVienHocVu)]
public sealed class AdminSchedulesController : ControllerBase
{
    private readonly IScheduleService _scheduleService;

    public AdminSchedulesController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    /// <summary>Danh sách lịch học — lọc theo lớp, phòng, thứ, trạng thái.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ScheduleListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] ScheduleListQuery query,
        CancellationToken cancellationToken)
        => (await _scheduleService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết lịch học.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ScheduleDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _scheduleService.GetDetailAsync(id, cancellationToken)).ToActionResult();

    /// <summary>
    /// Tạo lịch học định kỳ. Kiểm tra xung đột PHÒNG (cùng phòng, cùng thứ,
    /// trùng giờ, chồng lấn khoảng ngày áp dụng).
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ScheduleDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateScheduleRequest request,
        CancellationToken cancellationToken)
        => (await _scheduleService.CreateAsync(request, cancellationToken)).ToActionResult();

    /// <summary>Cập nhật lịch học (PATCH). Không cho đổi lớp.</summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(ScheduleDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] UpdateScheduleRequest request,
        CancellationToken cancellationToken)
        => (await _scheduleService.UpdateAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>Đổi trạng thái lịch: "Hoạt động" / "Ngừng áp dụng".</summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(ScheduleDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatusAsync(
        int id,
        [FromBody] UpdateStatusRequest request,
        CancellationToken cancellationToken)
        => (await _scheduleService.UpdateStatusAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>Xóa lịch — CHỈ khi chưa có ca học thực tế tham chiếu.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        => (await _scheduleService.DeleteAsync(id, cancellationToken)).ToActionResult();
}
