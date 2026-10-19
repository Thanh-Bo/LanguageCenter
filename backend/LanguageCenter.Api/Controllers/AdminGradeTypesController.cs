using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.GradeTypes;
using LanguageCenter.Application.GradeTypes.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>Đầu điểm của khóa học — CHỈ Quản trị viên.</summary>
[ApiController]
[Route("api/admin/grade-types")]
[Authorize(Roles = RoleNames.QuanTriVien)]
public sealed class AdminGradeTypesController : ControllerBase
{
    private readonly IGradeTypeService _gradeTypeService;

    public AdminGradeTypesController(IGradeTypeService gradeTypeService)
    {
        _gradeTypeService = gradeTypeService;
    }

    /// <summary>Danh sách đầu điểm — lọc theo khóa học, trạng thái, tên.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GradeTypeListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] GradeTypeListQuery query,
        CancellationToken cancellationToken)
        => (await _gradeTypeService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết đầu điểm.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GradeTypeDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _gradeTypeService.GetDetailAsync(id, cancellationToken)).ToActionResult();

    /// <summary>Tạo đầu điểm. Tên và thứ tự không được trùng trong cùng khóa học.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(GradeTypeDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateGradeTypeRequest request,
        CancellationToken cancellationToken)
        => (await _gradeTypeService.CreateAsync(request, cancellationToken)).ToActionResult();

    /// <summary>Cập nhật đầu điểm (PATCH).</summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(GradeTypeDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] UpdateGradeTypeRequest request,
        CancellationToken cancellationToken)
        => (await _gradeTypeService.UpdateAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>Đổi trạng thái đầu điểm: "Hoạt động" / "Ngừng sử dụng".</summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(GradeTypeDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatusAsync(
        int id,
        [FromBody] UpdateStatusRequest request,
        CancellationToken cancellationToken)
        => (await _gradeTypeService.UpdateStatusAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>Xóa đầu điểm — CHỈ khi chưa có điểm số nào tham chiếu.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        => (await _gradeTypeService.DeleteAsync(id, cancellationToken)).ToActionResult();
}
