using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Levels;
using LanguageCenter.Application.Levels.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Quản lý cấp độ trong chương trình đào tạo — CHỈ Quản trị viên.
/// </summary>
[ApiController]
[Route("api/admin/levels")]
[Authorize(Roles = RoleNames.QuanTriVien)]
public sealed class AdminLevelsController : ControllerBase
{
    private readonly ILevelService _levelService;

    public AdminLevelsController(ILevelService levelService)
    {
        _levelService = levelService;
    }

    /// <summary>Danh sách cấp độ — lọc theo chương trình, tìm theo tên/ký hiệu, lọc trạng thái.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<LevelListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] LevelListQuery query,
        CancellationToken cancellationToken)
        => (await _levelService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết cấp độ.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(LevelDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _levelService.GetDetailAsync(id, cancellationToken)).ToActionResult();

    /// <summary>
    /// Tạo cấp độ. Chương trình cha phải tồn tại.
    /// Tên và thứ tự phải không trùng trong cùng chương trình.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(LevelDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateLevelRequest request,
        CancellationToken cancellationToken)
        => (await _levelService.CreateAsync(request, cancellationToken)).ToActionResult();

    /// <summary>Cập nhật cấp độ (PATCH). Không cho chuyển sang chương trình khác.</summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(LevelDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] UpdateLevelRequest request,
        CancellationToken cancellationToken)
        => (await _levelService.UpdateAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>Đổi trạng thái: "Hoạt động" / "Ngừng hoạt động". KHÔNG cascade xuống khóa học.</summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(LevelDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatusAsync(
        int id,
        [FromBody] UpdateStatusRequest request,
        CancellationToken cancellationToken)
        => (await _levelService.UpdateStatusAsync(id, request, cancellationToken)).ToActionResult();
}
