using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Programs;
using LanguageCenter.Application.Programs.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Quản lý chương trình đào tạo — CHỈ Quản trị viên.
/// Controller chỉ nhận request, gọi service và map kết quả sang HTTP.
/// </summary>
[ApiController]
[Route("api/admin/programs")]
[Authorize(Roles = RoleNames.QuanTriVien)]
public sealed class AdminProgramsController : ControllerBase
{
    private readonly IProgramService _programService;

    public AdminProgramsController(IProgramService programService)
    {
        _programService = programService;
    }

    /// <summary>Danh sách chương trình — phân trang, tìm theo tên, lọc theo trạng thái.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProgramListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] ProgramListQuery query,
        CancellationToken cancellationToken)
        => (await _programService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết chương trình (kèm số cấp độ, số khóa học).</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProgramDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _programService.GetDetailAsync(id, cancellationToken)).ToActionResult();

    /// <summary>Tạo chương trình mới. Tên phải không trùng (UNIQUE trong database).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ProgramDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateProgramRequest request,
        CancellationToken cancellationToken)
        => (await _programService.CreateAsync(request, cancellationToken)).ToActionResult();

    /// <summary>Cập nhật thông tin chương trình (PATCH — chỉ field gửi lên mới thay đổi).</summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(ProgramDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] UpdateProgramRequest request,
        CancellationToken cancellationToken)
        => (await _programService.UpdateAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>
    /// Đổi trạng thái: "Hoạt động" / "Ngừng hoạt động".
    /// KHÔNG cascade xuống cấp độ hay khóa học.
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(ProgramDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatusAsync(
        int id,
        [FromBody] UpdateStatusRequest request,
        CancellationToken cancellationToken)
        => (await _programService.UpdateStatusAsync(id, request, cancellationToken)).ToActionResult();
}
