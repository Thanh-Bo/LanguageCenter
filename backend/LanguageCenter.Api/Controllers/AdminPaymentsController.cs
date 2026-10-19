using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Payments;
using LanguageCenter.Application.Payments.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>Giao dịch thanh toán — CHỈ ĐỌC (ghi nhận qua <c>/api/admin/tuition-fees/{id}/payments</c>).</summary>
[ApiController]
[Route("api/admin/payments")]
[Authorize(Roles = RoleNames.QuanTriVien
                   + ","
                   + RoleNames.NhanVienKeToan
                   + ","
                   + RoleNames.NhanVienTuVan)]
public sealed class AdminPaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public AdminPaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    /// <summary>Danh sách giao dịch — lọc theo học phí, học viên, khóa, phương thức, trạng thái, loại, khoảng ngày.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] PaymentListQuery query,
        CancellationToken cancellationToken)
        => (await _paymentService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết giao dịch (kèm biên nhận).</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _paymentService.GetDetailAsync(id, cancellationToken)).ToActionResult();
}
