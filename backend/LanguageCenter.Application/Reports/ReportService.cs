using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Reports.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Reports;

/// <inheritdoc cref="IReportService"/>
/// <remarks>
/// Phạm vi dữ liệu:
/// <list type="bullet">
/// <item>Quản trị viên / Nhân viên học vụ: xem mọi lớp.</item>
/// <item>Giáo viên / Trợ giảng: CHỈ lớp mình được phân công (ngược lại trả 403).</item>
/// </list>
/// </remarks>
public sealed class ReportService : IReportService
{
    private readonly IReportStore _reportStore;
    private readonly IAcademicLookupStore _lookupStore;

    public ReportService(IReportStore reportStore, IAcademicLookupStore lookupStore)
    {
        _reportStore = reportStore;
        _lookupStore = lookupStore;
    }

    public async Task<OperationResult<ReportOverviewDto>> GetOverviewAsync(
        CancellationToken cancellationToken)
    {
        var overview = await _reportStore.GetOverviewAsync(cancellationToken);

        return OperationResult<ReportOverviewDto>.Ok(overview);
    }

    public async Task<OperationResult<AcademicReportDto>> GetAcademicAsync(
        AcademicReportQuery query,
        ReportActor actor,
        CancellationToken cancellationToken)
    {
        if (query.MaLopHoc <= 0)
        {
            return OperationResult<AcademicReportDto>.BadRequest(
                "Lớp học là bắt buộc.",
                ["Truyền 'maLopHoc' trên query string."]);
        }

        var scopeError = await CheckClassScopeAsync(query.MaLopHoc, actor, cancellationToken);

        if (scopeError is not null)
        {
            return OperationResult<AcademicReportDto>.Forbidden(scopeError);
        }

        var report = await _reportStore.GetAcademicAsync(query.MaLopHoc, cancellationToken);

        return report is null
            ? OperationResult<AcademicReportDto>.NotFound(
                $"Không tìm thấy lớp học có id = {query.MaLopHoc}.")
            : OperationResult<AcademicReportDto>.Ok(report);
    }

    public async Task<OperationResult<AttendanceReportDto>> GetAttendanceAsync(
        AttendanceReportQuery query,
        ReportActor actor,
        CancellationToken cancellationToken)
    {
        if (query.MaLopHoc <= 0)
        {
            return OperationResult<AttendanceReportDto>.BadRequest(
                "Lớp học là bắt buộc.",
                ["Truyền 'maLopHoc' trên query string."]);
        }

        if (query.NguongVang is < 0 or > 100)
        {
            return OperationResult<AttendanceReportDto>.BadRequest(
                "Ngưỡng vắng không hợp lệ.",
                ["'nguongVang' phải trong khoảng 0..100."]);
        }

        if (query.TuNgay is { } tu && query.DenNgay is { } den && den < tu)
        {
            return OperationResult<AttendanceReportDto>.BadRequest(
                "Khoảng ngày không hợp lệ.",
                ["Đến ngày phải lớn hơn hoặc bằng từ ngày."]);
        }

        var scopeError = await CheckClassScopeAsync(query.MaLopHoc, actor, cancellationToken);

        if (scopeError is not null)
        {
            return OperationResult<AttendanceReportDto>.Forbidden(scopeError);
        }

        var report = await _reportStore.GetAttendanceAsync(
            query.MaLopHoc,
            query.TuNgay,
            query.DenNgay,
            query.NguongVang,
            cancellationToken);

        return report is null
            ? OperationResult<AttendanceReportDto>.NotFound(
                $"Không tìm thấy lớp học có id = {query.MaLopHoc}.")
            : OperationResult<AttendanceReportDto>.Ok(report);
    }

    public async Task<OperationResult<SurveyReportDto>> GetSurveyReportAsync(
        CancellationToken cancellationToken)
    {
        var report = await _reportStore.GetSurveyReportAsync(cancellationToken);

        return OperationResult<SurveyReportDto>.Ok(report);
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    private async Task<string?> CheckClassScopeAsync(
        int maLopHoc,
        ReportActor actor,
        CancellationToken cancellationToken)
    {
        if (actor.VaiTro is not (RoleNames.GiaoVien or RoleNames.TroGiang))
        {
            return null;
        }

        var maGiaoVien = TextValue.Required(actor.TenDangNhap);

        if (maGiaoVien is null)
        {
            return "Không xác định được giáo viên từ token.";
        }

        return await _lookupStore.TeacherAssignedToClassAsync(maLopHoc, maGiaoVien, cancellationToken)
            ? null
            : "Bạn không được phân công giảng dạy lớp này.";
    }
}
