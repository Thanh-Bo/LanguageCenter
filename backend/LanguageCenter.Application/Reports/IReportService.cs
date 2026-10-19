using LanguageCenter.Application.Common;
using LanguageCenter.Application.Reports.Models;

namespace LanguageCenter.Application.Reports;

/// <summary>Báo cáo và thống kê tổng hợp — chỉ đọc.</summary>
/// <remarks>
/// Báo cáo tài chính KHÔNG lặp lại ở đây: Phase 6 đã cung cấp
/// <c>GET /api/admin/tuition-fees/reports/summary</c>.
/// </remarks>
public interface IReportService
{
    Task<OperationResult<ReportOverviewDto>> GetOverviewAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Báo cáo học tập. Giáo viên/Trợ giảng chỉ xem được lớp mình được phân công (403 nếu khác).
    /// </summary>
    Task<OperationResult<AcademicReportDto>> GetAcademicAsync(
        AcademicReportQuery query,
        ReportActor actor,
        CancellationToken cancellationToken);

    /// <summary>Báo cáo điểm danh — cùng quy tắc phạm vi với báo cáo học tập.</summary>
    Task<OperationResult<AttendanceReportDto>> GetAttendanceAsync(
        AttendanceReportQuery query,
        ReportActor actor,
        CancellationToken cancellationToken);

    Task<OperationResult<SurveyReportDto>> GetSurveyReportAsync(CancellationToken cancellationToken);
}
