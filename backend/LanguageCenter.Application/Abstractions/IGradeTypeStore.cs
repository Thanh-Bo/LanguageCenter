using LanguageCenter.Application.Common;
using LanguageCenter.Application.GradeTypes.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>Cổng truy cập đầu điểm (<c>LoaiDiem</c>).</summary>
public interface IGradeTypeStore
{
    Task<PagedResult<GradeTypeListItemDto>> ListAsync(
        GradeTypeListQuery query,
        CancellationToken cancellationToken);

    Task<GradeTypeDetailDto?> GetDetailAsync(int maLoaiDiem, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int maLoaiDiem, CancellationToken cancellationToken);

    /// <summary>Các đầu điểm đang hoạt động của một khóa học, sắp theo <c>ThuTu</c>.</summary>
    Task<IReadOnlyList<GradeComponentInfo>> GetActiveByCourseAsync(
        int maKhoaHoc,
        CancellationToken cancellationToken);

    /// <summary>Đã có điểm số tham chiếu đầu điểm này chưa.</summary>
    Task<int> CountGradesAsync(int maLoaiDiem, CancellationToken cancellationToken);

    Task<WriteResult> CreateAsync(
        CreateGradeTypeRequest request,
        CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateAsync(
        int maLoaiDiem,
        UpdateGradeTypeRequest changes,
        CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateStatusAsync(
        int maLoaiDiem,
        string trangThai,
        CancellationToken cancellationToken);

    Task<WriteOutcome> DeleteAsync(int maLoaiDiem, CancellationToken cancellationToken);

    /// <summary>Đã có đầu điểm khác cùng tên trong khóa học chưa (UNIQUE MaKhoaHoc + TenLoaiDiem).</summary>
    Task<bool> NameTakenAsync(
        int maKhoaHoc,
        string tenLoaiDiem,
        int? exceptId,
        CancellationToken cancellationToken);

    /// <summary>Đã có đầu điểm khác cùng thứ tự trong khóa học chưa (UNIQUE MaKhoaHoc + ThuTu).</summary>
    Task<bool> OrderTakenAsync(
        int maKhoaHoc,
        int thuTu,
        int? exceptId,
        CancellationToken cancellationToken);
}
