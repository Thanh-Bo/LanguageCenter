using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Promotions;
using LanguageCenter.Application.Promotions.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>Quản lý danh mục ưu đãi — CHỈ Quản trị viên.</summary>
[ApiController]
[Route("api/admin/promotions")]
[Authorize(Roles = RoleNames.QuanTriVien)]
public sealed class AdminPromotionsController : ControllerBase
{
    private readonly IPromotionService _promotionService;

    public AdminPromotionsController(IPromotionService promotionService)
    {
        _promotionService = promotionService;
    }

    /// <summary>Danh sách ưu đãi — tìm theo tên, lọc trạng thái, lọc còn hiệu lực.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PromotionListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] PromotionListQuery query,
        CancellationToken cancellationToken)
        => (await _promotionService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết ưu đãi.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PromotionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _promotionService.GetDetailAsync(id, cancellationToken)).ToActionResult();

    /// <summary>
    /// Tạo ưu đãi. Loại giảm giá phải là "Phần trăm" hoặc "Số tiền";
    /// nếu là "Phần trăm" thì giá trị không được vượt quá 100.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PromotionDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreatePromotionRequest request,
        CancellationToken cancellationToken)
        => (await _promotionService.CreateAsync(request, cancellationToken)).ToActionResult();

    /// <summary>Cập nhật ưu đãi (PATCH).</summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(PromotionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] UpdatePromotionRequest request,
        CancellationToken cancellationToken)
        => (await _promotionService.UpdateAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>Đổi trạng thái: "Hoạt động" / "Ngừng hoạt động".</summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(PromotionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatusAsync(
        int id,
        [FromBody] UpdateStatusRequest request,
        CancellationToken cancellationToken)
        => (await _promotionService.UpdateStatusAsync(id, request, cancellationToken)).ToActionResult();
}
