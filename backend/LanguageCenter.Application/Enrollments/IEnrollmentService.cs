using LanguageCenter.Application.Common;
using LanguageCenter.Application.Enrollments.Models;

namespace LanguageCenter.Application.Enrollments;

/// <summary>Quản lý đăng ký học và xếp lớp.</summary>
public interface IEnrollmentService
{
    Task<OperationResult<PagedResult<EnrollmentListItemDto>>> ListAsync(
        EnrollmentListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<EnrollmentDetailDto>> GetDetailAsync(
        int maDangKy,
        CancellationToken cancellationToken);

    Task<OperationResult<EnrollmentDetailDto>> CreateAsync(
        CreateEnrollmentRequest request,
        string currentUserTenDangNhap,
        CancellationToken cancellationToken);

    Task<OperationResult<EnrollmentDetailDto>> UpdateAsync(
        int maDangKy,
        UpdateEnrollmentRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<EnrollmentDetailDto>> AssignClassAsync(
        int maDangKy,
        AssignClassRequest request,
        CancellationToken cancellationToken);

    /// <summary>Bỏ lớp: trả đăng ký về trạng thái "Chờ xử lý" để xếp lại lớp khác.</summary>
    Task<OperationResult<EnrollmentDetailDto>> UnassignClassAsync(
        int maDangKy,
        CancellationToken cancellationToken);

    Task<OperationResult<EnrollmentDetailDto>> UpdateStatusAsync(
        int maDangKy,
        UpdateStatusRequest request,
        CancellationToken cancellationToken);
}
