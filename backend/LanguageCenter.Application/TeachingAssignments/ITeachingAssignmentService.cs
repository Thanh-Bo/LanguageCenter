using LanguageCenter.Application.Common;
using LanguageCenter.Application.TeachingAssignments.Models;

namespace LanguageCenter.Application.TeachingAssignments;

/// <summary>Quản lý phân công giáo viên / trợ giảng cho lớp.</summary>
public interface ITeachingAssignmentService
{
    Task<OperationResult<PagedResult<AssignmentListItemDto>>> ListAsync(
        AssignmentListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<AssignmentDetailDto>> GetDetailAsync(
        int maPhanCong,
        CancellationToken cancellationToken);

    Task<OperationResult<AssignmentDetailDto>> CreateAsync(
        CreateAssignmentRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<AssignmentDetailDto>> UpdateAsync(
        int maPhanCong,
        UpdateAssignmentRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<AssignmentDetailDto>> UpdateStatusAsync(
        int maPhanCong,
        UpdateStatusRequest request,
        CancellationToken cancellationToken);
}
