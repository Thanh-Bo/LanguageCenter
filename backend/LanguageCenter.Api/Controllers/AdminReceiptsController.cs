using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Receipts;
using LanguageCenter.Application.Receipts.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Biên nhận — CHỈ ĐỌC. Biên nhận được sinh tự động khi ghi nhận thanh toán;
/// không có endpoint tạo/sửa/xoá để bảo đảm tính nhất quán với giao dịch.
/// </summary>
[ApiController]
[Route("api/admin/receipts")]
[Authorize(Roles = RoleNames.QuanTriVien
                   + ","
                   + RoleNames.NhanVienKeToan
                   + ","
                   + RoleNames.NhanVienTuVan)]
public sealed class AdminReceiptsController : ControllerBase
{
    private readonly IReceiptService _receiptService;

    public AdminReceiptsController(IReceiptService receiptService)
    {
        _receiptService = receiptService;
    }

    /// <summary>Danh sách biên nhận — lọc theo số biên nhận, học viên, giao dịch, trạng thái, khoảng ngày lập.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ReceiptDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] ReceiptListQuery query,
        CancellationToken cancellationToken)
        => (await _receiptService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>Tra cứu biên nhận theo SỐ biên nhận (ví dụ <c>BN20261018001</c>).</summary>
    [HttpGet("by-number/{soBienNhan}")]
    [ProducesResponseType(typeof(ReceiptDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetByNumberAsync(
        string soBienNhan,
        CancellationToken cancellationToken)
        => (await _receiptService.GetByNumberAsync(soBienNhan, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết biên nhận theo id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ReceiptDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _receiptService.GetDetailAsync(id, cancellationToken)).ToActionResult();
}
