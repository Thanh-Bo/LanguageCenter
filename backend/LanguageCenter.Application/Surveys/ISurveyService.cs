using LanguageCenter.Application.Common;
using LanguageCenter.Application.Surveys.Models;

namespace LanguageCenter.Application.Surveys;

/// <summary>Quản lý khảo sát và phản hồi.</summary>
public interface ISurveyService
{
    // ---------------- Quản trị ----------------
    Task<OperationResult<PagedResult<SurveyListItemDto>>> ListAsync(
        SurveyListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<SurveyDetailDto>> GetDetailAsync(
        int maKhaoSat,
        CancellationToken cancellationToken);

    Task<OperationResult<SurveyDetailDto>> CreateAsync(
        CreateSurveyRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<SurveyDetailDto>> UpdateAsync(
        int maKhaoSat,
        UpdateSurveyRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<SurveyDetailDto>> UpdateStatusAsync(
        int maKhaoSat,
        UpdateStatusRequest request,
        CancellationToken cancellationToken);

    /// <summary>Xóa khảo sát — 409 nếu đã có phản hồi.</summary>
    Task<OperationResult<bool>> DeleteAsync(int maKhaoSat, CancellationToken cancellationToken);

    Task<OperationResult<IReadOnlyList<SurveyQuestionDto>>> ListQuestionsAsync(
        int maKhaoSat,
        CancellationToken cancellationToken);

    Task<OperationResult<SurveyQuestionDto>> CreateQuestionAsync(
        int maKhaoSat,
        CreateSurveyQuestionRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<SurveyQuestionDto>> UpdateQuestionAsync(
        int maKhaoSat,
        int maCauHoi,
        UpdateSurveyQuestionRequest request,
        CancellationToken cancellationToken);

    /// <summary>Xóa câu hỏi — 409 nếu đã có câu trả lời.</summary>
    Task<OperationResult<bool>> DeleteQuestionAsync(
        int maKhaoSat,
        int maCauHoi,
        CancellationToken cancellationToken);

    Task<OperationResult<PagedResult<SurveyResponseListItemDto>>> ListResponsesAsync(
        int maKhaoSat,
        SurveyResponseListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<SurveyStatisticsDto>> GetStatisticsAsync(
        int maKhaoSat,
        CancellationToken cancellationToken);

    // ---------------- Học viên ----------------
    Task<OperationResult<IReadOnlyList<MySurveyDto>>> ListForStudentAsync(
        SurveyStudentActor actor,
        CancellationToken cancellationToken);

    Task<OperationResult<MySurveyDto>> GetForStudentAsync(
        int maKhaoSat,
        SurveyStudentActor actor,
        CancellationToken cancellationToken);

    Task<OperationResult<SurveyResponseDetailDto>> GetMyResponseAsync(
        int maKhaoSat,
        SurveyStudentActor actor,
        CancellationToken cancellationToken);

    Task<OperationResult<SurveyResponseDetailDto>> SubmitAsync(
        int maKhaoSat,
        SubmitSurveyResponseRequest request,
        SurveyStudentActor actor,
        CancellationToken cancellationToken);
}
