using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Classes;
using LanguageCenter.Application.Classes.Models;
using LanguageCenter.Application.Common;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Quản lý lớp học — Quản trị viên và Nhân viên học vụ.
/// </summary>
[ApiController]
[Route("api/admin/classes")]
[Authorize(Roles = RoleNames.QuanTriVien + "," + RoleNames.NhanVienHocVu)]
public sealed class AdminClassesController : ControllerBase
{
    private readonly IClassService _classService;

    public AdminClassesController(IClassService classService)
    {
        _classService = classService;
    }

    /// <summary>Danh sách lớp — lọc theo khóa học/trạng thái, tìm theo mã hoặc tên lớp.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ClassListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] ClassListQuery query,
        CancellationToken cancellationToken)
        => (await _classService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết lớp (kèm sĩ số hiện tại).</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ClassDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _classService.GetDetailAsync(id, cancellationToken)).ToActionResult();

    /// <summary>
    /// Tạo lớp. Mã lớp phải không trùng, khóa học và nhân viên học vụ phải tồn tại.
    /// Ngày của lớp độc lập với ngày của khóa học.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ClassDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateClassRequest request,
        CancellationToken cancellationToken)
        => (await _classService.CreateAsync(request, cancellationToken)).ToActionResult();

    /// <summary>Cập nhật lớp (PATCH). Không cho chuyển sang khóa học khác.</summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(ClassDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] UpdateClassRequest request,
        CancellationToken cancellationToken)
        => (await _classService.UpdateAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>Đổi trạng thái lớp. KHÔNG tự động xóa dữ liệu con.</summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(ClassDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatusAsync(
        int id,
        [FromBody] UpdateStatusRequest request,
        CancellationToken cancellationToken)
        => (await _classService.UpdateStatusAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>Xóa lớp — CHỈ khi chưa có đăng ký / phân công / lịch học / ca học / thông báo.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        => (await _classService.DeleteAsync(id, cancellationToken)).ToActionResult();
}
