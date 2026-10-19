using LanguageCenter.Application.Common;
using LanguageCenter.Application.Courses.Models;

namespace LanguageCenter.Application.Courses;

/// <summary>Quản lý khóa học.</summary>
public interface ICourseService
{
    Task<OperationResult<PagedResult<CourseListItemDto>>> ListAsync(
        CourseListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<CourseDetailDto>> GetDetailAsync(
        int maKhoaHoc,
        CancellationToken cancellationToken);

    Task<OperationResult<CourseDetailDto>> CreateAsync(
        CreateCourseRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<CourseDetailDto>> UpdateAsync(
        int maKhoaHoc,
        UpdateCourseRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<CourseDetailDto>> UpdateStatusAsync(
        int maKhoaHoc,
        UpdateStatusRequest request,
        CancellationToken cancellationToken);
}
