using LanguageCenter.Application.Attendance.Models;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Abstractions;

/// <summary>Cổng truy cập điểm danh (<c>DiemDanh</c>).</summary>
public interface IAttendanceStore
{
    /// <summary>Bảng điểm danh của một ca học (null nếu ca không tồn tại).</summary>
    Task<SessionAttendanceSheetDto?> GetSheetAsync(int maCaHoc, CancellationToken cancellationToken);

    /// <summary>
    /// Ghi điểm danh hàng loạt — upsert theo khoá <c>(MaCaHoc, MaHocVien)</c>.
    /// Toàn bộ nằm trong MỘT transaction: hoặc ghi hết, hoặc không ghi gì.
    /// </summary>
    Task<WriteOutcome> SaveBatchAsync(
        int maCaHoc,
        IReadOnlyList<AttendanceItemRequest> items,
        int maTaiKhoanThucHien,
        CancellationToken cancellationToken);

    Task<bool> AttendanceExistsAsync(int maDiemDanh, CancellationToken cancellationToken);

    /// <summary>Ca học chứa bản ghi điểm danh này (null nếu không tìm thấy).</summary>
    Task<int?> GetSessionIdAsync(int maDiemDanh, CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateAsync(
        int maDiemDanh,
        string trangThaiDiemDanh,
        string? ghiChu,
        int maTaiKhoanThucHien,
        CancellationToken cancellationToken);

    /// <summary>Lịch sử điểm danh của một học viên (null nếu học viên không tồn tại).</summary>
    Task<StudentAttendanceHistoryDto?> GetStudentHistoryAsync(
        string maHocVien,
        DateOnly? tuNgay,
        DateOnly? denNgay,
        CancellationToken cancellationToken);
}
