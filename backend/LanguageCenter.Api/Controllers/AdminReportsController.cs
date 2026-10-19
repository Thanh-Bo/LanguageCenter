using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Reports;
using LanguageCenter.Application.Reports.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Báo cáo và thống kê — chỉ đọc.
/// <para>
/// Báo cáo tài chính nằm ở Phase 6: <c>GET /api/admin/tuition-fees/reports/summary</c>
/// (không lặp lại ở đây để tránh hai nguồn số liệu khác nhau).
/// </para>
/// </summary>
[ApiController]
[Route("api/admin/reports")]
[Authorize]
public sealed class AdminReportsController : ControllerBase
{
    private const string StaffRoles = RoleNames.QuanTriVien + "," + RoleNames.NhanVienHocVu;

    private const string ClassScopeRoles = StaffRoles
                                           + ","
                                           + RoleNames.GiaoVien
                                           + ","
                                           + RoleNames.TroGiang;

    private readonly IReportService _reportService;

    public AdminReportsController(IReportService reportService)
    {
        _reportService = reportService;
    }

    /// <summary>
    /// Báo cáo tổng quan toàn trung tâm: tài khoản, học viên, giáo viên, nhân viên,
    /// khóa học, lớp học, đăng ký, ca học, điểm danh, học phí, khảo sát.
    /// </summary>
    [HttpGet("overview")]
    [Authorize(Roles = StaffRoles)]
    [ProducesResponseType(typeof(ReportOverviewDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOverviewAsync(CancellationToken cancellationToken)
        => (await _reportService.GetOverviewAsync(cancellationToken)).ToActionResult();

    /// <summary>
    /// Báo cáo học tập của MỘT lớp — điểm từng đầu điểm và điểm tổng kết theo công thức Phase 5.
    /// <para>Giáo viên/Trợ giảng chỉ xem được lớp mình được phân công.</para>
    /// </summary>
    [HttpGet("academic")]
    [Authorize(Roles = ClassScopeRoles)]
    [ProducesResponseType(typeof(AcademicReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAcademicAsync(
        [FromQuery] AcademicReportQuery query,
        CancellationToken cancellationToken)
        => (await _reportService.GetAcademicAsync(query, BuildActor(), cancellationToken))
            .ToActionResult();

    /// <summary>
    /// Báo cáo điểm danh của MỘT lớp — tỉ lệ có mặt/vắng và danh sách học viên vắng cao.
    /// <para>Ca học đã hủy KHÔNG được tính.</para>
    /// </summary>
    [HttpGet("attendance")]
    [Authorize(Roles = ClassScopeRoles)]
    [ProducesResponseType(typeof(AttendanceReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAttendanceAsync(
        [FromQuery] AttendanceReportQuery query,
        CancellationToken cancellationToken)
        => (await _reportService.GetAttendanceAsync(query, BuildActor(), cancellationToken))
            .ToActionResult();

    /// <summary>Báo cáo tổng hợp khảo sát (số phản hồi, điểm trung bình, tỉ lệ phản hồi tham khảo).</summary>
    [HttpGet("surveys")]
    [Authorize(Roles = StaffRoles)]
    [ProducesResponseType(typeof(SurveyReportDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSurveyReportAsync(CancellationToken cancellationToken)
        => (await _reportService.GetSurveyReportAsync(cancellationToken)).ToActionResult();

    // ========================================================================
    // PRIVATE
    // ========================================================================

    private ReportActor BuildActor() => new(
        User.GetTenDangNhap() ?? string.Empty,
        User.GetVaiTro() ?? string.Empty);
}
