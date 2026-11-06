using LanguageCenter.Application.Common;
using LanguageCenter.Application.Sessions.Models;

namespace LanguageCenter.Application.Sessions;

/// <summary>Quản lý ca học thực tế.</summary>
public interface ISessionService
{
    Task<OperationResult<PagedResult<SessionListItemDto>>> ListAsync(
        SessionListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<SessionDetailDto>> GetDetailAsync(
        int maCaHoc,
        CancellationToken cancellationToken);

    Task<OperationResult<SessionDetailDto>> CreateAsync(
        CreateSessionRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<SessionDetailDto>> UpdateAsync(
        int maCaHoc,
        UpdateSessionRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<SessionDetailDto>> UpdateStatusAsync(
        int maCaHoc,
        UpdateStatusRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<bool>> DeleteAsync(int maCaHoc, CancellationToken cancellationToken);

    Task<OperationResult<GenerateSessionsResponse>> GenerateAsync(
        GenerateSessionsRequest request,
        CancellationToken cancellationToken);

    /// <summary>Lịch của lớp mà học viên đang theo học.</summary>
    Task<OperationResult<IReadOnlyList<SessionListItemDto>>> ListForStudentAsync(
        string maHocVien,
        DateOnly? tuNgay,
        DateOnly? denNgay,
        CancellationToken cancellationToken);

    /// <summary>Ca học mà giáo viên / trợ giảng được phân công.</summary>
    Task<OperationResult<IReadOnlyList<SessionListItemDto>>> ListForTeacherAsync(
        string maGiaoVien,
        DateOnly? tuNgay,
        DateOnly? denNgay,
        CancellationToken cancellationToken);

    /// <summary>Lớp mà giáo viên / trợ giảng đang được phân công.</summary>
    Task<OperationResult<IReadOnlyList<TeacherClassDto>>> ListMyClassesAsync(
        string maGiaoVien,
        CancellationToken cancellationToken);

    /// <summary>Học viên của một lớp — chỉ khi người gọi được phân công lớp đó.</summary>
    Task<OperationResult<IReadOnlyList<TeacherClassStudentDto>>> ListMyClassStudentsAsync(
        string maGiaoVien,
        int maLopHoc,
        CancellationToken cancellationToken);

    /// <summary>Tiến độ từng buổi của lớp — dùng để tô màu buổi đã học / buổi thi / buổi hiện tại.</summary>
    Task<OperationResult<ClassProgressDto>> GetClassProgressAsync(
        int maLopHoc,
        CancellationToken cancellationToken);

    /// <summary>Tiến độ lớp cho giáo viên — chỉ khi được phân công lớp đó.</summary>
    Task<OperationResult<ClassProgressDto>> GetClassProgressForTeacherAsync(
        string maGiaoVien,
        int maLopHoc,
        CancellationToken cancellationToken);

    /// <summary>Tiến độ lớp cho học viên — chỉ khi đang học lớp đó.</summary>
    Task<OperationResult<ClassProgressDto>> GetClassProgressForStudentAsync(
        string maHocVien,
        int maLopHoc,
        CancellationToken cancellationToken);

    /// <summary>
    /// Đồng bộ giáo viên cho các ca CHƯA diễn ra của lớp theo quy ước
    /// \"buổi đầu tuần = GV nước ngoài, buổi còn lại = GV chính\".
    /// </summary>
    Task<OperationResult<SyncSessionTeachersResponse>> SyncTeachersAsync(
        SyncSessionTeachersRequest request,
        CancellationToken cancellationToken);

    /// <summary>
    /// Chốt trạng thái ca học theo NGÀY: buổi đã qua → "Đã hoàn thành",
    /// buổi chưa tới ngày → "Chưa diễn ra". Không đụng ca "Đã hủy".
    /// </summary>
    Task<OperationResult<SettleSessionsResponse>> SettleAsync(
        SettleSessionsRequest request,
        CancellationToken cancellationToken);
}
