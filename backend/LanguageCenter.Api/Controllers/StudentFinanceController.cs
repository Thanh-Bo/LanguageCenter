using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Payments;
using LanguageCenter.Application.Payments.Models;
using LanguageCenter.Application.Receipts;
using LanguageCenter.Application.Receipts.Models;
using LanguageCenter.Application.TuitionFees;
using LanguageCenter.Application.TuitionFees.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Tài chính của Học viên — CHỈ ĐỌC, chỉ dữ liệu của CHÍNH MÌNH.
/// <para>
/// Danh tính lấy từ JWT (<c>unique_name</c> = <c>MaHocVien</c>).
/// Không có tham số mã học viên trên URL ⇒ không thể xem dữ liệu người khác.
/// Học viên KHÔNG có endpoint nào để xác nhận/gửi thanh toán.
/// </para>
/// </summary>
[ApiController]
[Route("api/students/me")]
[Authorize(Roles = RoleNames.HocVien)]
public sealed class StudentFinanceController : ControllerBase
{
    private readonly ITuitionFeeService _tuitionFeeService;
    private readonly IPaymentService _paymentService;
    private readonly IReceiptService _receiptService;

    public StudentFinanceController(
        ITuitionFeeService tuitionFeeService,
        IPaymentService paymentService,
        IReceiptService receiptService)
    {
        _tuitionFeeService = tuitionFeeService;
        _paymentService = paymentService;
        _receiptService = receiptService;
    }

    /// <summary>Nghĩa vụ học phí của chính mình.</summary>
    [HttpGet("tuition-fees")]
    [ProducesResponseType(typeof(PagedResult<TuitionFeeListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListTuitionFeesAsync(
        [FromQuery] TuitionFeeListQuery query,
        CancellationToken cancellationToken)
    {
        var maHocVien = User.GetTenDangNhap();

        if (string.IsNullOrWhiteSpace(maHocVien))
        {
            return Unauthorized();
        }

        return (await _tuitionFeeService.ListForStudentAsync(maHocVien, query, cancellationToken))
            .ToActionResult();
    }

    /// <summary>Chi tiết một nghĩa vụ học phí của chính mình.</summary>
    [HttpGet("tuition-fees/{id:int}")]
    [ProducesResponseType(typeof(TuitionFeeDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetTuitionFeeAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var maHocVien = User.GetTenDangNhap();

        if (string.IsNullOrWhiteSpace(maHocVien))
        {
            return Unauthorized();
        }

        return (await _tuitionFeeService.GetDetailForStudentAsync(id, maHocVien, cancellationToken))
            .ToActionResult();
    }

    /// <summary>Các giao dịch thanh toán của chính mình.</summary>
    [HttpGet("payments")]
    [ProducesResponseType(typeof(PagedResult<PaymentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListPaymentsAsync(
        [FromQuery] PaymentListQuery query,
        CancellationToken cancellationToken)
    {
        var maHocVien = User.GetTenDangNhap();

        if (string.IsNullOrWhiteSpace(maHocVien))
        {
            return Unauthorized();
        }

        return (await _paymentService.ListForStudentAsync(maHocVien, query, cancellationToken))
            .ToActionResult();
    }

    /// <summary>Các biên nhận của chính mình.</summary>
    [HttpGet("receipts")]
    [ProducesResponseType(typeof(PagedResult<ReceiptDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListReceiptsAsync(
        [FromQuery] ReceiptListQuery query,
        CancellationToken cancellationToken)
    {
        var maHocVien = User.GetTenDangNhap();

        if (string.IsNullOrWhiteSpace(maHocVien))
        {
            return Unauthorized();
        }

        return (await _receiptService.ListForStudentAsync(maHocVien, query, cancellationToken))
            .ToActionResult();
    }

    /// <summary>Chi tiết một biên nhận của chính mình.</summary>
    [HttpGet("receipts/{id:int}")]
    [ProducesResponseType(typeof(ReceiptDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReceiptAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var maHocVien = User.GetTenDangNhap();

        if (string.IsNullOrWhiteSpace(maHocVien))
        {
            return Unauthorized();
        }

        return (await _receiptService.GetDetailForStudentAsync(id, maHocVien, cancellationToken))
            .ToActionResult();
    }
}
