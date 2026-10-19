using LanguageCenter.Application.Reports.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>Cổng truy cập dữ liệu tổng hợp cho báo cáo — CHỈ ĐỌC.</summary>
public interface IReportStore
{
    Task<ReportOverviewDto> GetOverviewAsync(CancellationToken cancellationToken);

    /// <summary>Báo cáo học tập của lớp; <c>null</c> nếu lớp không tồn tại.</summary>
    Task<AcademicReportDto?> GetAcademicAsync(
        int maLopHoc,
        CancellationToken cancellationToken);

    /// <summary>Báo cáo điểm danh của lớp; <c>null</c> nếu lớp không tồn tại.</summary>
    Task<AttendanceReportDto?> GetAttendanceAsync(
        int maLopHoc,
        DateOnly? tuNgay,
        DateOnly? denNgay,
        int nguongVang,
        CancellationToken cancellationToken);

    Task<SurveyReportDto> GetSurveyReportAsync(CancellationToken cancellationToken);
}
