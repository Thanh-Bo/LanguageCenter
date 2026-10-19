using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Enrollments;
using LanguageCenter.Application.Enrollments.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Đăng ký học &amp; xếp lớp — Quản trị viên và Nhân viên tư vấn.
/// <para>
/// <c>MaNhanVienTuVan</c> luôn lấy từ JWT (claim <c>unique_name</c>), KHÔNG nhận từ client.
/// </para>
/// </summary>
[ApiController]
[Route("api/admin/enrollments")]
[Authorize(Roles = RoleNames.QuanTriVien + "," + RoleNames.NhanVienTuVan)]
public sealed class AdminEnrollmentsController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public AdminEnrollmentsController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }

    /// <summary>Danh sách đăng ký — tìm theo học viên/khóa học/mã đăng ký, lọc trạng thái.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<EnrollmentListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] EnrollmentListQuery query,
        CancellationToken cancellationToken)
        => (await _enrollmentService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết đăng ký (kèm học phí gốc / giảm giá / phải đóng).</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EnrollmentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _enrollmentService.GetDetailAsync(id, cancellationToken)).ToActionResult();

    /// <summary>
    /// Tạo đăng ký học.
    /// <para>
    /// Học phí gốc lấy từ khóa học; số tiền giảm do server tính từ ưu đãi;
    /// người tư vấn lấy từ JWT; <c>HocPhiSauGiam</c> do SQL Server tự tính.
    /// </para>
    /// <para>Nếu gửi kèm <c>maLopHoc</c> thì hệ thống xếp lớp luôn và kiểm tra sĩ số.</para>
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(EnrollmentDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateEnrollmentRequest request,
        CancellationToken cancellationToken)
    {
        var tenDangNhap = User.GetTenDangNhap();

        if (tenDangNhap is null)
        {
            return Unauthorized(new { message = "Token không hợp lệ." });
        }

        return (await _enrollmentService.CreateAsync(request, tenDangNhap, cancellationToken))
            .ToActionResult();
    }

    /// <summary>Cập nhật đăng ký (ưu đãi / lịch học mong muốn / ghi chú). Server tính lại tiền giảm.</summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(EnrollmentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] UpdateEnrollmentRequest request,
        CancellationToken cancellationToken)
        => (await _enrollmentService.UpdateAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>
    /// Xếp học viên vào lớp. Lớp phải thuộc đúng khóa học và còn chỗ.
    /// Sĩ số được kiểm tra NGUYÊN TỬ nên nhiều request đồng thời không thể vượt sĩ số.
    /// </summary>
    [HttpPatch("{id:int}/assign-class")]
    [ProducesResponseType(typeof(EnrollmentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AssignClassAsync(
        int id,
        [FromBody] AssignClassRequest request,
        CancellationToken cancellationToken)
        => (await _enrollmentService.AssignClassAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>Bỏ lớp: trả đăng ký về trạng thái "Chờ xử lý" để xếp lại lớp khác.</summary>
    [HttpPatch("{id:int}/unassign-class")]
    [ProducesResponseType(typeof(EnrollmentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UnassignClassAsync(int id, CancellationToken cancellationToken)
        => (await _enrollmentService.UnassignClassAsync(id, cancellationToken)).ToActionResult();

    /// <summary>
    /// Đổi trạng thái đăng ký. KHÔNG dùng endpoint này để xếp lớp —
    /// trạng thái "Đã xếp lớp" chỉ đạt được qua <c>assign-class</c>.
    /// </summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(EnrollmentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatusAsync(
        int id,
        [FromBody] UpdateStatusRequest request,
        CancellationToken cancellationToken)
        => (await _enrollmentService.UpdateStatusAsync(id, request, cancellationToken)).ToActionResult();
}
