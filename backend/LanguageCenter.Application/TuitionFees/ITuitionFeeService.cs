using LanguageCenter.Application.Common;
using LanguageCenter.Application.TuitionFees.Models;

namespace LanguageCenter.Application.TuitionFees;

/// <summary>Quản lý nghĩa vụ học phí.</summary>
public interface ITuitionFeeService
{
    Task<OperationResult<PagedResult<TuitionFeeListItemDto>>> ListAsync(
        TuitionFeeListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<TuitionFeeDetailDto>> GetDetailAsync(
        int maHocPhi,
        CancellationToken cancellationToken);

    /// <summary>Tạo nghĩa vụ học phí từ một đăng ký học — idempotent, 409 nếu đã tồn tại.</summary>
    Task<OperationResult<TuitionFeeDetailDto>> CreateAsync(
        CreateTuitionFeeRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<TuitionFeeDetailDto>> UpdateAsync(
        int maHocPhi,
        UpdateTuitionFeeRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<TuitionSummaryReportDto>> GetReportAsync(
        TuitionReportQuery query,
        CancellationToken cancellationToken);

    // ------------------------------------------------------------------
    // HỌC VIÊN — danh tính lấy từ JWT, KHÔNG tin tham số client
    // ------------------------------------------------------------------

    Task<OperationResult<PagedResult<TuitionFeeListItemDto>>> ListForStudentAsync(
        string maHocVien,
        TuitionFeeListQuery query,
        CancellationToken cancellationToken);

    /// <summary>Chi tiết học phí của chính mình — không thuộc sở hữu thì trả 404.</summary>
    Task<OperationResult<TuitionFeeDetailDto>> GetDetailForStudentAsync(
        int maHocPhi,
        string maHocVien,
        CancellationToken cancellationToken);
}
