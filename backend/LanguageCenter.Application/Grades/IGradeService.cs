using LanguageCenter.Application.Common;
using LanguageCenter.Application.Grades.Models;

namespace LanguageCenter.Application.Grades;

/// <summary>Quản lý điểm số và tính điểm tổng kết.</summary>
public interface IGradeService
{
    Task<OperationResult<PagedResult<GradeListItemDto>>> ListAsync(
        GradeListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<GradeDetailDto>> GetDetailAsync(
        int maDiemSo,
        CancellationToken cancellationToken);

    Task<OperationResult<GradeDetailDto>> CreateAsync(
        CreateGradeRequest request,
        GradeActor actor,
        CancellationToken cancellationToken);

    Task<OperationResult<GradeDetailDto>> UpdateAsync(
        int maDiemSo,
        UpdateGradeRequest request,
        GradeActor actor,
        CancellationToken cancellationToken);

    Task<OperationResult<bool>> DeleteAsync(
        int maDiemSo,
        GradeActor actor,
        CancellationToken cancellationToken);

    /// <summary>Bảng điểm tổng kết của lớp — CHỈ TÍNH, KHÔNG GHI vào database.</summary>
    Task<OperationResult<GradeSummaryDto>> GetClassSummaryAsync(
        int maLopHoc,
        CancellationToken cancellationToken);

    /// <summary>Điểm tổng kết của chính học viên (dùng cho API tự phục vụ).</summary>
    Task<OperationResult<IReadOnlyList<StudentCourseGradeDto>>> GetForStudentAsync(
        string maHocVien,
        CancellationToken cancellationToken);
}
