using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Payments;
using LanguageCenter.Application.Payments.Models;
using LanguageCenter.Application.TuitionFees;
using LanguageCenter.Application.TuitionFees.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Học phí — Quản trị viên, Nhân viên kế toán và Nhân viên tư vấn (chỉ đọc).
/// <para>
/// Ghi nhận thanh toán chỉ dành cho Quản trị viên và Nhân viên kế toán.
/// </para>
/// </summary>
[ApiController]
[Route("api/admin/tuition-fees")]
[Authorize(Roles = RoleNames.QuanTriVien
                   + ","
                   + RoleNames.NhanVienKeToan
                   + ","
                   + RoleNames.NhanVienTuVan)]
public sealed class AdminTuitionFeesController : ControllerBase
{
    private const string WriteRoles = RoleNames.QuanTriVien + "," + RoleNames.NhanVienKeToan;

    private readonly ITuitionFeeService _tuitionFeeService;
    private readonly IPaymentService _paymentService;

    public AdminTuitionFeesController(
        ITuitionFeeService tuitionFeeService,
        IPaymentService paymentService)
    {
        _tuitionFeeService = tuitionFeeService;
        _paymentService = paymentService;
    }

    /// <summary>Danh sách nghĩa vụ học phí — lọc theo học viên, khóa, lớp, trạng thái, quá hạn, khoảng ngày tạo.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TuitionFeeListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] TuitionFeeListQuery query,
        CancellationToken cancellationToken)
        => (await _tuitionFeeService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>
    /// Báo cáo học phí: tổng phải thu / đã thu / còn thiếu, danh sách còn nợ,
    /// thống kê giao dịch theo phương thức và trạng thái.
    /// </summary>
    [HttpGet("reports/summary")]
    [ProducesResponseType(typeof(TuitionSummaryReportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetReportAsync(
        [FromQuery] TuitionReportQuery query,
        CancellationToken cancellationToken)
        => (await _tuitionFeeService.GetReportAsync(query, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết một nghĩa vụ học phí.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TuitionFeeDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _tuitionFeeService.GetDetailAsync(id, cancellationToken)).ToActionResult();

    /// <summary>Các giao dịch của một nghĩa vụ học phí.</summary>
    [HttpGet("{id:int}/payments")]
    [ProducesResponseType(typeof(PagedResult<PaymentDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListPaymentsAsync(
        int id,
        [FromQuery] PaymentListQuery query,
        CancellationToken cancellationToken)
        => (await _paymentService.ListByTuitionAsync(id, query, cancellationToken)).ToActionResult();

    /// <summary>
    /// Tạo nghĩa vụ học phí từ một đăng ký học.
    /// <para>
    /// Số tiền được COPY từ <c>DangKyHoc.HocPhiGoc</c> / <c>DangKyHoc.SoTienGiam</c>.
    /// Mỗi đăng ký chỉ có đúng một nghĩa vụ học phí (409 nếu đã tồn tại).
    /// </para>
    /// </summary>
    [HttpPost]
    [Authorize(Roles = WriteRoles)]
    [ProducesResponseType(typeof(TuitionFeeDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateTuitionFeeRequest request,
        CancellationToken cancellationToken)
        => (await _tuitionFeeService.CreateAsync(request, cancellationToken)).ToActionResult();

    /// <summary>
    /// Cập nhật hạn thanh toán, hoặc đồng bộ lại số tiền từ đăng ký.
    /// <para>
    /// Đồng bộ số tiền CHỈ thực hiện được khi học phí chưa có giao dịch nào (409 nếu đã đóng tiền).
    /// </para>
    /// </summary>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = WriteRoles)]
    [ProducesResponseType(typeof(TuitionFeeDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] UpdateTuitionFeeRequest request,
        CancellationToken cancellationToken)
        => (await _tuitionFeeService.UpdateAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>
    /// Ghi nhận thanh toán cho nghĩa vụ học phí.
    /// <para>
    /// Idempotent theo <c>maGiaoDich</c>: gửi lại cùng mã trả về 200 kèm giao dịch đã có.
    /// Biên nhận được tạo tự động trong cùng transaction.
    /// </para>
    /// </summary>
    [HttpPost("{id:int}/payments")]
    [Authorize(Roles = WriteRoles)]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(PaymentDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> RecordPaymentAsync(
        int id,
        [FromBody] RecordPaymentRequest request,
        CancellationToken cancellationToken)
        => (await _paymentService.RecordAsync(id, request, BuildActor(), cancellationToken))
            .ToActionResult();

    // ========================================================================
    // PRIVATE
    // ========================================================================

    private PaymentActor BuildActor() => new(User.GetTenDangNhap() ?? string.Empty);
}
