using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Attendance;
using LanguageCenter.Application.Attendance.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Điểm danh — Quản trị viên, Nhân viên học vụ, Giáo viên, Trợ giảng.
/// <para>Giáo viên / Trợ giảng chỉ điểm danh được ca của mình hoặc lớp mình được phân công.</para>
/// </summary>
[ApiController]
[Route("api/admin")]
[Authorize(Roles = RoleNames.QuanTriVien
                   + ","
                   + RoleNames.NhanVienHocVu
                   + ","
                   + RoleNames.GiaoVien
                   + ","
                   + RoleNames.TroGiang)]
public sealed class AdminAttendanceController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;

    public AdminAttendanceController(IAttendanceService attendanceService)
    {
        _attendanceService = attendanceService;
    }

    /// <summary>Bảng điểm danh của ca học — gồm cả học viên chưa điểm danh.</summary>
    [HttpGet("sessions/{id:int}/attendance")]
    [ProducesResponseType(typeof(SessionAttendanceSheetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSheetAsync(int id, CancellationToken cancellationToken)
        => (await _attendanceService.GetSheetAsync(id, BuildActor(), cancellationToken))
            .ToActionResult();

    /// <summary>
    /// Ghi điểm danh hàng loạt cho ca học (upsert). Sai một dòng là từ chối TOÀN BỘ yêu cầu.
    /// </summary>
    [HttpPost("sessions/{id:int}/attendance")]
    [ProducesResponseType(typeof(SessionAttendanceSheetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SaveAsync(
        int id,
        [FromBody] SaveAttendanceRequest request,
        CancellationToken cancellationToken)
        => (await _attendanceService.SaveBatchAsync(id, request, BuildActor(), cancellationToken))
            .ToActionResult();

    /// <summary>Sửa một bản ghi điểm danh.</summary>
    [HttpPatch("attendance/{maDiemDanh:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(
        int maDiemDanh,
        [FromBody] UpdateAttendanceRequest request,
        CancellationToken cancellationToken)
        => (await _attendanceService.UpdateAsync(maDiemDanh, request, BuildActor(), cancellationToken))
            .ToActionResult();

    /// <summary>Lịch sử điểm danh + thống kê chuyên cần của một học viên.</summary>
    [HttpGet("students/{maHocVien}/attendance")]
    [ProducesResponseType(typeof(StudentAttendanceHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStudentHistoryAsync(
        string maHocVien,
        [FromQuery] DateOnly? tuNgay,
        [FromQuery] DateOnly? denNgay,
        CancellationToken cancellationToken)
        => (await _attendanceService.GetStudentHistoryAsync(
                maHocVien, tuNgay, denNgay, cancellationToken))
            .ToActionResult();

    // ========================================================================
    // PRIVATE
    // ========================================================================

    /// <summary>
    /// Dựng danh tính người gọi TỪ JWT. <c>sub</c> có thể null nếu token cũ —
    /// khi đó dùng 0 và service sẽ tự từ chối vì không khớp phân công nào.
    /// </summary>
    private AttendanceActor BuildActor()
    {
        var vaiTro = User.GetVaiTro();

        var isStaff = vaiTro is RoleNames.QuanTriVien or RoleNames.NhanVienHocVu;

        return new AttendanceActor(
            User.GetMaTaiKhoan() ?? 0,
            User.GetTenDangNhap() ?? string.Empty,
            isStaff);
    }
}
