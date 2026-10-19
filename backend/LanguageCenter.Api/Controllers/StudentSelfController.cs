using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Attendance;
using LanguageCenter.Application.Attendance.Models;
using LanguageCenter.Application.Grades;
using LanguageCenter.Application.Grades.Models;
using LanguageCenter.Application.Sessions;
using LanguageCenter.Application.Sessions.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// API tự phục vụ của Học viên — CHỈ xem dữ liệu của CHÍNH MÌNH.
/// <para>
/// Danh tính lấy từ JWT (<c>unique_name</c> = <c>MaHocVien</c>).
/// KHÔNG có tham số mã học viên trên URL — chống IDOR (xem dữ liệu người khác).
/// </para>
/// </summary>
[ApiController]
[Route("api/students/me")]
[Authorize(Roles = RoleNames.HocVien)]
public sealed class StudentSelfController : ControllerBase
{
    private readonly IAttendanceService _attendanceService;
    private readonly IGradeService _gradeService;
    private readonly ISessionService _sessionService;

    public StudentSelfController(
        IAttendanceService attendanceService,
        IGradeService gradeService,
        ISessionService sessionService)
    {
        _attendanceService = attendanceService;
        _gradeService = gradeService;
        _sessionService = sessionService;
    }

    /// <summary>Lịch sử điểm danh + thống kê chuyên cần của chính mình.</summary>
    [HttpGet("attendance")]
    [ProducesResponseType(typeof(StudentAttendanceHistoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyAttendanceAsync(
        [FromQuery] DateOnly? tuNgay,
        [FromQuery] DateOnly? denNgay,
        CancellationToken cancellationToken)
    {
        var maHocVien = User.GetTenDangNhap();

        if (string.IsNullOrWhiteSpace(maHocVien))
        {
            return Unauthorized();
        }

        return (await _attendanceService.GetStudentHistoryAsync(
                maHocVien, tuNgay, denNgay, cancellationToken))
            .ToActionResult();
    }

    /// <summary>Điểm tổng kết của chính mình theo từng lớp.</summary>
    [HttpGet("grades")]
    [ProducesResponseType(typeof(IReadOnlyList<StudentCourseGradeDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyGradesAsync(CancellationToken cancellationToken)
    {
        var maHocVien = User.GetTenDangNhap();

        if (string.IsNullOrWhiteSpace(maHocVien))
        {
            return Unauthorized();
        }

        return (await _gradeService.GetForStudentAsync(maHocVien, cancellationToken))
            .ToActionResult();
    }

    /// <summary>Lịch học của chính mình (các ca học của lớp đang theo học).</summary>
    [HttpGet("schedule")]
    [ProducesResponseType(typeof(IReadOnlyList<SessionListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyScheduleAsync(
        [FromQuery] DateOnly? tuNgay,
        [FromQuery] DateOnly? denNgay,
        CancellationToken cancellationToken)
    {
        var maHocVien = User.GetTenDangNhap();

        if (string.IsNullOrWhiteSpace(maHocVien))
        {
            return Unauthorized();
        }

        return (await _sessionService.ListForStudentAsync(
                maHocVien, tuNgay, denNgay, cancellationToken))
            .ToActionResult();
    }
}
