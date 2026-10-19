using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Attendance.Models;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Sessions;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Attendance;

/// <inheritdoc cref="IAttendanceService"/>
/// <remarks>
/// QUY TẮC QUYỀN (Phase 5):
/// <list type="bullet">
/// <item>Quản trị viên / Nhân viên học vụ: điểm danh mọi ca.</item>
/// <item>Giáo viên / Trợ giảng: chỉ ca mình là người dạy thực tế, hoặc lớp mình đang được phân công.</item>
/// <item>Học viên: không được điểm danh.</item>
/// </list>
/// </remarks>
public sealed class AttendanceService : IAttendanceService
{
    private readonly IAttendanceStore _attendanceStore;
    private readonly ISessionStore _sessionStore;
    private readonly IAcademicLookupStore _lookupStore;

    public AttendanceService(
        IAttendanceStore attendanceStore,
        ISessionStore sessionStore,
        IAcademicLookupStore lookupStore)
    {
        _attendanceStore = attendanceStore;
        _sessionStore = sessionStore;
        _lookupStore = lookupStore;
    }

    public async Task<OperationResult<SessionAttendanceSheetDto>> GetSheetAsync(
        int maCaHoc,
        AttendanceActor actor,
        CancellationToken cancellationToken)
    {
        var sheet = await _attendanceStore.GetSheetAsync(maCaHoc, cancellationToken);

        if (sheet is null)
        {
            return OperationResult<SessionAttendanceSheetDto>.NotFound(
                $"Không tìm thấy ca học có id = {maCaHoc}.");
        }

        var reason = await EvaluatePermissionAsync(maCaHoc, actor, cancellationToken);

        sheet.CoTheDiemDanh = reason is null;
        sheet.LyDoKhongTheDiemDanh = reason;

        return OperationResult<SessionAttendanceSheetDto>.Ok(sheet);
    }

    public async Task<OperationResult<SessionAttendanceSheetDto>> SaveBatchAsync(
        int maCaHoc,
        SaveAttendanceRequest request,
        AttendanceActor actor,
        CancellationToken cancellationToken)
    {
        var permissionError = await EvaluatePermissionAsync(maCaHoc, actor, cancellationToken);

        if (permissionError is not null)
        {
            return OperationResult<SessionAttendanceSheetDto>.BadRequest(
                "Không thể điểm danh ca học này.",
                [permissionError]);
        }

        if (request.Items.Count == 0)
        {
            return OperationResult<SessionAttendanceSheetDto>.BadRequest(
                "Dữ liệu không hợp lệ.",
                ["Cần ít nhất một bản ghi điểm danh."]);
        }

        var session = await _sessionStore.GetDetailAsync(maCaHoc, cancellationToken);

        if (session is null)
        {
            return OperationResult<SessionAttendanceSheetDto>.NotFound(
                $"Không tìm thấy ca học có id = {maCaHoc}.");
        }

        // -------- VALIDATE TOÀN BỘ TRƯỚC KHI GHI (all-or-nothing) --------
        var errors = new List<string>();
        var seen = new HashSet<string>(StringComparer.Ordinal);
        var normalized = new List<AttendanceItemRequest>(request.Items.Count);

        foreach (var item in request.Items)
        {
            var code = TextValue.Required(item.MaHocVien)?.Trim();

            if (string.IsNullOrEmpty(code))
            {
                errors.Add("Có dòng thiếu mã học viên.");
                continue;
            }

            if (!seen.Add(code))
            {
                errors.Add($"Học viên '{code}' xuất hiện nhiều lần trong cùng yêu cầu.");
                continue;
            }

            var trangThai = TextValue.Required(item.TrangThaiDiemDanh)?.Trim();

            if (trangThai is null || !AttendanceStatuses.IsValid(trangThai))
            {
                errors.Add(
                    $"Trạng thái điểm danh của học viên '{code}' không hợp lệ. " +
                    $"Chỉ chấp nhận: {string.Join(", ", AttendanceStatuses.All)}.");
                continue;
            }

            if (!await _lookupStore.StudentInClassAsync(session.MaLopHoc, code, cancellationToken))
            {
                errors.Add($"Học viên '{code}' không thuộc lớp '{session.MaLop}'.");
                continue;
            }

            normalized.Add(new AttendanceItemRequest
            {
                MaHocVien = code,
                TrangThaiDiemDanh = trangThai,
                GhiChu = TextValue.Required(item.GhiChu)
            });
        }

        if (errors.Count > 0)
        {
            return OperationResult<SessionAttendanceSheetDto>.BadRequest(
                "Dữ liệu điểm danh không hợp lệ.",
                errors);
        }

        var outcome = await _attendanceStore.SaveBatchAsync(
            maCaHoc,
            normalized,
            actor.MaTaiKhoan,
            cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return OperationResult<SessionAttendanceSheetDto>.Conflict(
                "Không thể lưu điểm danh do ràng buộc dữ liệu.");
        }

        var sheet = await _attendanceStore.GetSheetAsync(maCaHoc, cancellationToken);

        sheet!.CoTheDiemDanh = true;

        return OperationResult<SessionAttendanceSheetDto>.Ok(sheet);
    }

    public async Task<OperationResult<bool>> UpdateAsync(
        int maDiemDanh,
        UpdateAttendanceRequest request,
        AttendanceActor actor,
        CancellationToken cancellationToken)
    {
        var trangThai = TextValue.Required(request.TrangThaiDiemDanh)?.Trim();

        if (trangThai is null || !AttendanceStatuses.IsValid(trangThai))
        {
            return OperationResult<bool>.BadRequest(
                "Trạng thái điểm danh không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", AttendanceStatuses.All)}."]);
        }

        if (!await _attendanceStore.AttendanceExistsAsync(maDiemDanh, cancellationToken))
        {
            return OperationResult<bool>.NotFound($"Không tìm thấy bản ghi điểm danh có id = {maDiemDanh}.");
        }

        var maCaHoc = await _attendanceStore.GetSessionIdAsync(maDiemDanh, cancellationToken);

        if (maCaHoc is null)
        {
            return OperationResult<bool>.NotFound($"Không tìm thấy ca học của bản ghi điểm danh {maDiemDanh}.");
        }

        var permissionError = await EvaluatePermissionAsync(maCaHoc.Value, actor, cancellationToken);

        if (permissionError is not null)
        {
            return OperationResult<bool>.Forbidden(permissionError);
        }

        var outcome = await _attendanceStore.UpdateAsync(
            maDiemDanh,
            trangThai,
            TextValue.Required(request.GhiChu),
            actor.MaTaiKhoan,
            cancellationToken);

        return outcome switch
        {
            WriteOutcome.Success => OperationResult<bool>.Ok(true),
            WriteOutcome.NotFound => OperationResult<bool>.NotFound(
                $"Không tìm thấy bản ghi điểm danh có id = {maDiemDanh}."),
            _ => OperationResult<bool>.Conflict("Không thể cập nhật điểm danh.")
        };
    }

    public async Task<OperationResult<StudentAttendanceHistoryDto>> GetStudentHistoryAsync(
        string maHocVien,
        DateOnly? tuNgay,
        DateOnly? denNgay,
        CancellationToken cancellationToken)
    {
        if (tuNgay is { } tu && denNgay is { } den && den < tu)
        {
            return OperationResult<StudentAttendanceHistoryDto>.BadRequest(
                "Khoảng ngày không hợp lệ.",
                ["Đến ngày phải lớn hơn hoặc bằng từ ngày."]);
        }

        var history = await _attendanceStore.GetStudentHistoryAsync(
            maHocVien, tuNgay, denNgay, cancellationToken);

        return history is null
            ? OperationResult<StudentAttendanceHistoryDto>.NotFound(
                $"Không tìm thấy học viên '{maHocVien}'.")
            : OperationResult<StudentAttendanceHistoryDto>.Ok(history);
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    /// <summary>
    /// Trả về <c>null</c> nếu được phép điểm danh, ngược lại trả về lý do từ chối.
    /// </summary>
    private async Task<string?> EvaluatePermissionAsync(
        int maCaHoc,
        AttendanceActor actor,
        CancellationToken cancellationToken)
    {
        var session = await _sessionStore.GetDetailAsync(maCaHoc, cancellationToken);

        if (session is null)
        {
            return $"Không tìm thấy ca học có id = {maCaHoc}.";
        }

        if (!SessionStatuses.CanTakeAttendance(session.TrangThai))
        {
            return $"Ca học đang ở trạng thái '{session.TrangThai}', không thể điểm danh.";
        }

        if (actor.IsStaff)
        {
            return null;
        }

        var code = actor.TenDangNhap.Trim();

        var isActualTeacher = string.Equals(session.MaGiaoVienThucTe, code, StringComparison.Ordinal)
                              || string.Equals(session.MaTroGiangThucTe, code, StringComparison.Ordinal);

        if (isActualTeacher)
        {
            return null;
        }

        if (await _lookupStore.TeacherAssignedToClassAsync(
                session.MaLopHoc, code, cancellationToken))
        {
            return null;
        }

        return "Bạn không phải người dạy ca này và cũng không được phân công giảng dạy lớp này.";
    }
}
