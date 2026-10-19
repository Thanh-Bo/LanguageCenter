using LanguageCenter.Application.Common;
using LanguageCenter.Application.TeachingAssignments.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>Cổng truy cập dữ liệu phân công giảng dạy (<c>PhanCongGiangDay</c>).</summary>
public interface ITeachingAssignmentStore
{
    Task<PagedResult<AssignmentListItemDto>> ListAsync(
        AssignmentListQuery query,
        CancellationToken cancellationToken);

    Task<AssignmentDetailDto?> GetDetailAsync(int maPhanCong, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int maPhanCong, CancellationToken cancellationToken);

    /// <summary>
    /// Đã có phân công cùng lớp + cùng người + cùng vai trò với khoảng ngày CHỒNG LẤN chưa
    /// (bỏ qua phân công đã hủy).
    /// </summary>
    Task<bool> HasOverlapAsync(
        int maLopHoc,
        string maGiaoVien,
        string vaiTroPhanCong,
        DateOnly ngayBatDau,
        DateOnly? ngayKetThuc,
        int? exceptId,
        CancellationToken cancellationToken);

    Task<WriteResult> CreateAsync(CreateAssignmentRequest request, CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateAsync(
        int maPhanCong,
        UpdateAssignmentRequest changes,
        CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateStatusAsync(
        int maPhanCong,
        string trangThai,
        CancellationToken cancellationToken);
}
