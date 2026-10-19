using LanguageCenter.Application.Common;
using LanguageCenter.Application.Sessions.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>Cổng truy cập ca học thực tế (<c>CaHoc</c>).</summary>
public interface ISessionStore
{
    Task<PagedResult<SessionListItemDto>> ListAsync(
        SessionListQuery query,
        CancellationToken cancellationToken);

    Task<SessionDetailDto?> GetDetailAsync(int maCaHoc, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int maCaHoc, CancellationToken cancellationToken);

    Task<int> CountAttendanceAsync(int maCaHoc, CancellationToken cancellationToken);

    /// <summary>Xung đột PHÒNG: cùng phòng + cùng ngày + giờ chồng lấn (bỏ qua ca đã hủy).</summary>
    Task<bool> HasRoomConflictAsync(
        int maPhongHoc,
        DateOnly ngayHoc,
        TimeOnly gioBatDau,
        TimeOnly gioKetThuc,
        int? exceptId,
        CancellationToken cancellationToken);

    /// <summary>Xung đột GIÁO VIÊN/TRỢ GIẢNG: cùng người + cùng ngày + giờ chồng lấn.</summary>
    Task<bool> HasTeacherConflictAsync(
        string maGiaoVien,
        DateOnly ngayHoc,
        TimeOnly gioBatDau,
        TimeOnly gioKetThuc,
        int? exceptId,
        CancellationToken cancellationToken);

    /// <summary>Đã có ca học cùng lớp + cùng ngày chưa (chống trùng vô lý).</summary>
    Task<bool> HasSameClassSameDayAsync(
        int maLopHoc,
        DateOnly ngayHoc,
        int? exceptId,
        CancellationToken cancellationToken);

    Task<WriteResult> CreateAsync(CreateSessionRequest request, CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateAsync(
        int maCaHoc,
        UpdateSessionRequest changes,
        CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateStatusAsync(
        int maCaHoc,
        string trangThai,
        CancellationToken cancellationToken);

    Task<WriteOutcome> DeleteAsync(int maCaHoc, CancellationToken cancellationToken);

    /// <summary>Tạo nhiều ca học từ một lịch định kỳ — idempotent, có transaction.</summary>
    Task<GenerateOutcome> GenerateSessionsAsync(
        int maLichHoc,
        DateOnly tuNgay,
        DateOnly denNgay,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<SessionListItemDto>> ListByClassIdsAsync(
        IReadOnlyList<int> classIds,
        DateOnly? tuNgay,
        DateOnly? denNgay,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<SessionListItemDto>> ListByTeacherAsync(
        string maGiaoVien,
        DateOnly? tuNgay,
        DateOnly? denNgay,
        CancellationToken cancellationToken);

    /// <summary>Các lớp giáo viên/trợ giảng đang được phân công giảng dạy.</summary>
    Task<IReadOnlyList<TeacherClassDto>> ListTeacherClassesAsync(
        string maGiaoVien,
        CancellationToken cancellationToken);
}

/// <summary>Kết quả tạo ca học hàng loạt từ lịch định kỳ.</summary>
public sealed record GenerateOutcome(
    bool ScheduleFound,
    int MaLichHoc,
    DateOnly TuNgay,
    DateOnly DenNgay,
    DateOnly? ApDungTuThucTe,
    DateOnly? ApDungDenThucTe,
    int TongNgayKhop,
    IReadOnlyList<DateOnly> NgayTao,
    IReadOnlyList<DateOnly> NgayDaCo,
    IReadOnlyList<GenerateConflict> NgayXungDot);

/// <summary>Một ngày không tạo được ca học và lý do.</summary>
public sealed record GenerateConflict(DateOnly NgayHoc, string LyDo);
