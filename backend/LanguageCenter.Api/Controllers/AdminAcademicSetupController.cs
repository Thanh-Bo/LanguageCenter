using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.AcademicSetup;
using LanguageCenter.Application.AcademicSetup.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Cấu hình học vụ — Quản trị viên và Nhân viên học vụ.
/// <para>
/// <c>lesson-contents</c>: nội dung 20 buổi của từng khóa (sinh ca học sẽ gắn theo).
/// <c>holidays</c>: kỳ nghỉ (1 ngày hoặc nhiều ngày) — buổi rơi vào kỳ nghỉ tự dời sang tuần sau.
/// </para>
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize(Roles = RoleNames.QuanTriVien + "," + RoleNames.NhanVienHocVu)]
public sealed class AdminAcademicSetupController : ControllerBase
{
    private readonly IAcademicSetupService _service;

    public AdminAcademicSetupController(IAcademicSetupService service)
    {
        _service = service;
    }

    // ======================================================================
    // 20 BUỔI HỌC
    // ======================================================================

    /// <summary>Danh sách nội dung 20 buổi của 1 khóa học.</summary>
    [HttpGet("lesson-contents")]
    public async Task<IActionResult> ListLessonContentsAsync(
        [FromQuery] int maKhoaHoc,
        CancellationToken cancellationToken)
        => (await _service.ListLessonContentsAsync(maKhoaHoc, cancellationToken)).ToActionResult();

    /// <summary>Lưu (thêm mới / cập nhật) nội dung 20 buổi của 1 khóa học.</summary>
    [HttpPut("lesson-contents/{maKhoaHoc:int}")]
    public async Task<IActionResult> SaveLessonContentsAsync(
        int maKhoaHoc,
        [FromBody] SaveLessonContentsRequest request,
        CancellationToken cancellationToken)
        => (await _service.SaveLessonContentsAsync(maKhoaHoc, request, cancellationToken)).ToActionResult();

    // ======================================================================
    // NGÀY LỄ
    // ======================================================================

    /// <summary>Danh sách kỳ nghỉ (sắp theo ngày bắt đầu).</summary>
    [HttpGet("holidays")]
    public async Task<IActionResult> ListHolidaysAsync(CancellationToken cancellationToken)
        => (await _service.ListHolidaysAsync(cancellationToken)).ToActionResult();

    /// <summary>Thêm kỳ nghỉ. <c>denNgay</c> bỏ trống = nghỉ đúng 1 ngày. Không được chồng lấn kỳ khác.</summary>
    [HttpPost("holidays")]
    public async Task<IActionResult> CreateHolidayAsync(
        [FromBody] CreateHolidayRequest request,
        CancellationToken cancellationToken)
        => (await _service.CreateHolidayAsync(request, cancellationToken)).ToActionResult();

    /// <summary>Cập nhật kỳ nghỉ (PATCH). Gửi <c>xoaDenNgay = true</c> để chuyển về nghỉ 1 ngày.</summary>
    [HttpPatch("holidays/{id:int}")]
    public async Task<IActionResult> UpdateHolidayAsync(
        int id,
        [FromBody] UpdateHolidayRequest request,
        CancellationToken cancellationToken)
        => (await _service.UpdateHolidayAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>Xóa kỳ nghỉ.</summary>
    [HttpDelete("holidays/{id:int}")]
    public async Task<IActionResult> DeleteHolidayAsync(int id, CancellationToken cancellationToken)
        => (await _service.DeleteHolidayAsync(id, cancellationToken)).ToActionResult();
}
