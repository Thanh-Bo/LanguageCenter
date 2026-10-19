using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.TeachingAssignments;
using LanguageCenter.Application.TeachingAssignments.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Phân công giáo viên / trợ giảng — Quản trị viên và Nhân viên học vụ.
/// </summary>
[ApiController]
[Route("api/admin/teaching-assignments")]
[Authorize(Roles = RoleNames.QuanTriVien + "," + RoleNames.NhanVienHocVu)]
public sealed class AdminTeachingAssignmentsController : ControllerBase
{
    private readonly ITeachingAssignmentService _assignmentService;

    public AdminTeachingAssignmentsController(ITeachingAssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
    }

    /// <summary>Danh sách phân công — lọc theo lớp, theo giáo viên, theo trạng thái.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AssignmentListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] AssignmentListQuery query,
        CancellationToken cancellationToken)
        => (await _assignmentService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết phân công.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AssignmentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _assignmentService.GetDetailAsync(id, cancellationToken)).ToActionResult();

    /// <summary>
    /// Tạo phân công. Vai trò phải thuộc: "Giáo viên chính", "Giáo viên thay thế", "Trợ giảng".
    /// Vai trò "Trợ giảng" cần hồ sơ Trợ giảng; hai vai trò còn lại cần hồ sơ Giáo viên.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AssignmentDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateAssignmentRequest request,
        CancellationToken cancellationToken)
        => (await _assignmentService.CreateAsync(request, cancellationToken)).ToActionResult();

    /// <summary>Cập nhật phân công (PATCH). Không cho đổi lớp — tạo bản ghi mới để giữ lịch sử.</summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(AssignmentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] UpdateAssignmentRequest request,
        CancellationToken cancellationToken)
        => (await _assignmentService.UpdateAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>Đổi trạng thái phân công: "Đang phân công" / "Đã kết thúc" / "Đã hủy".</summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(AssignmentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatusAsync(
        int id,
        [FromBody] UpdateStatusRequest request,
        CancellationToken cancellationToken)
        => (await _assignmentService.UpdateStatusAsync(id, request, cancellationToken)).ToActionResult();
}
