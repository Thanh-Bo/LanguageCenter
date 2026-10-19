using LanguageCenter.Application.Common;
using LanguageCenter.Application.GradeTypes.Models;

namespace LanguageCenter.Application.GradeTypes;

/// <summary>Quản lý đầu điểm của khóa học.</summary>
public interface IGradeTypeService
{
    Task<OperationResult<PagedResult<GradeTypeListItemDto>>> ListAsync(
        GradeTypeListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<GradeTypeDetailDto>> GetDetailAsync(
        int maLoaiDiem,
        CancellationToken cancellationToken);

    Task<OperationResult<GradeTypeDetailDto>> CreateAsync(
        CreateGradeTypeRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<GradeTypeDetailDto>> UpdateAsync(
        int maLoaiDiem,
        UpdateGradeTypeRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<GradeTypeDetailDto>> UpdateStatusAsync(
        int maLoaiDiem,
        UpdateStatusRequest request,
        CancellationToken cancellationToken);

    /// <summary>Xóa đầu điểm — CHỈ khi chưa có điểm số nào tham chiếu.</summary>
    Task<OperationResult<bool>> DeleteAsync(int maLoaiDiem, CancellationToken cancellationToken);
}
