using LanguageCenter.Application.Common;
using LanguageCenter.Application.Courses.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>Cổng truy cập dữ liệu khóa học (<c>KhoaHoc</c>).</summary>
public interface ICourseStore
{
    Task<PagedResult<CourseListItemDto>> ListAsync(CourseListQuery query, CancellationToken cancellationToken);

    Task<CourseDetailDto?> GetDetailAsync(int maKhoaHoc, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int maKhoaHoc, CancellationToken cancellationToken);

    /// <summary>Trùng tên khóa học TRONG CÙNG cấp độ (UQ_KhoaHoc_CapDo_Ten).</summary>
    Task<bool> NameExistsAsync(
        int maCapDo,
        string tenKhoaHoc,
        int? exceptId,
        CancellationToken cancellationToken);

    Task<WriteResult> CreateAsync(CreateCourseRequest request, CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateAsync(int maKhoaHoc, UpdateCourseRequest changes, CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateStatusAsync(int maKhoaHoc, string trangThai, CancellationToken cancellationToken);

    /// <summary>Học phí cơ bản + trạng thái hiện tại — dùng khi tạo đăng ký học.</summary>
    Task<CourseTuitionInfo?> GetTuitionInfoAsync(int maKhoaHoc, CancellationToken cancellationToken);
}

/// <summary>Thông tin tối thiểu của khóa học cần cho nghiệp vụ đăng ký.</summary>
public sealed record CourseTuitionInfo(
    int MaKhoaHoc,
    string TenKhoaHoc,
    decimal HocPhiCoBan,
    string TrangThai);
