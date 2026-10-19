using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Courses;
using LanguageCenter.Application.Courses.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>Quản lý khóa học — CHỈ Quản trị viên.</summary>
[ApiController]
[Route("api/admin/courses")]
[Authorize(Roles = RoleNames.QuanTriVien)]
public sealed class AdminCoursesController : ControllerBase
{
    private readonly ICourseService _courseService;

    public AdminCoursesController(ICourseService courseService)
    {
        _courseService = courseService;
    }

    /// <summary>Danh sách khóa học — lọc theo cấp độ/chương trình, tìm theo tên, lọc trạng thái.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<CourseListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] CourseListQuery query,
        CancellationToken cancellationToken)
        => (await _courseService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết khóa học (kèm số lớp, số đăng ký).</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CourseDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _courseService.GetDetailAsync(id, cancellationToken)).ToActionResult();

    /// <summary>
    /// Tạo khóa học. Cấp độ phải tồn tại.
    /// Kiểm tra: Số buổi &gt; 0, Tổng số giờ &gt; 0, Học phí &gt;= 0, Ngày kết thúc &gt;= Ngày bắt đầu.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CourseDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateCourseRequest request,
        CancellationToken cancellationToken)
        => (await _courseService.CreateAsync(request, cancellationToken)).ToActionResult();

    /// <summary>Cập nhật khóa học (PATCH). Không cho chuyển sang cấp độ khác.</summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(CourseDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] UpdateCourseRequest request,
        CancellationToken cancellationToken)
        => (await _courseService.UpdateAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>Đổi trạng thái khóa học. KHÔNG cascade xuống lớp học.</summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(CourseDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatusAsync(
        int id,
        [FromBody] UpdateStatusRequest request,
        CancellationToken cancellationToken)
        => (await _courseService.UpdateStatusAsync(id, request, cancellationToken)).ToActionResult();
}
