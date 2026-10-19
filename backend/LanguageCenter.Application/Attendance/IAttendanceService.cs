using LanguageCenter.Application.Attendance.Models;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Attendance;

/// <summary>Điểm danh học viên theo ca học.</summary>
public interface IAttendanceService
{
    /// <summary>Bảng điểm danh của ca học (kèm đánh giá quyền điểm danh của người gọi).</summary>
    Task<OperationResult<SessionAttendanceSheetDto>> GetSheetAsync(
        int maCaHoc,
        AttendanceActor actor,
        CancellationToken cancellationToken);

    /// <summary>Ghi điểm danh hàng loạt. Toàn bộ hợp lệ mới ghi; sai một dòng là từ chối tất cả.</summary>
    Task<OperationResult<SessionAttendanceSheetDto>> SaveBatchAsync(
        int maCaHoc,
        SaveAttendanceRequest request,
        AttendanceActor actor,
        CancellationToken cancellationToken);

    /// <summary>Sửa một bản ghi điểm danh.</summary>
    Task<OperationResult<bool>> UpdateAsync(
        int maDiemDanh,
        UpdateAttendanceRequest request,
        AttendanceActor actor,
        CancellationToken cancellationToken);

    /// <summary>Lịch sử điểm danh + thống kê chuyên cần.</summary>
    Task<OperationResult<StudentAttendanceHistoryDto>> GetStudentHistoryAsync(
        string maHocVien,
        DateOnly? tuNgay,
        DateOnly? denNgay,
        CancellationToken cancellationToken);
}
