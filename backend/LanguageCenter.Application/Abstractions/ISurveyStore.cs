using LanguageCenter.Application.Common;
using LanguageCenter.Application.Surveys.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>Cổng truy cập khảo sát (<c>KhaoSat</c>, <c>CauHoiKhaoSat</c>, <c>PhanHoiKhaoSat</c>, <c>CauTraLoiKhaoSat</c>).</summary>
public interface ISurveyStore
{
    // ---------------- Khảo sát ----------------
    Task<PagedResult<SurveyListItemDto>> ListAsync(
        SurveyListQuery query,
        CancellationToken cancellationToken);

    Task<SurveyDetailDto?> GetDetailAsync(int maKhaoSat, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int maKhaoSat, CancellationToken cancellationToken);

    Task<int> CountResponsesAsync(int maKhaoSat, CancellationToken cancellationToken);

    Task<WriteResult> CreateAsync(CreateSurveyData data, CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateAsync(
        int maKhaoSat,
        SurveyUpdateData changes,
        CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateStatusAsync(
        int maKhaoSat,
        string trangThai,
        CancellationToken cancellationToken);

    Task<WriteOutcome> DeleteAsync(int maKhaoSat, CancellationToken cancellationToken);

    // ---------------- Câu hỏi ----------------
    Task<IReadOnlyList<SurveyQuestionDto>> ListQuestionsAsync(
        int maKhaoSat,
        CancellationToken cancellationToken);

    Task<SurveyQuestionDto?> GetQuestionAsync(
        int maKhaoSat,
        int maCauHoi,
        CancellationToken cancellationToken);

    Task<bool> QuestionOrderTakenAsync(
        int maKhaoSat,
        int thuTu,
        int? exceptId,
        CancellationToken cancellationToken);

    Task<int> CountAnswersAsync(int maCauHoi, CancellationToken cancellationToken);

    Task<WriteResult> CreateQuestionAsync(
        int maKhaoSat,
        CreateQuestionData data,
        CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateQuestionAsync(
        int maCauHoi,
        QuestionUpdateData changes,
        CancellationToken cancellationToken);

    Task<WriteOutcome> DeleteQuestionAsync(int maCauHoi, CancellationToken cancellationToken);

    // ---------------- Học viên ----------------
    Task<IReadOnlyList<MySurveyDto>> ListForStudentAsync(
        string maHocVien,
        CancellationToken cancellationToken);

    Task<MySurveyDto?> GetForStudentAsync(
        int maKhaoSat,
        string maHocVien,
        CancellationToken cancellationToken);

    Task<SurveyResponseDetailDto?> GetResponseOfStudentAsync(
        int maKhaoSat,
        string maHocVien,
        CancellationToken cancellationToken);

    /// <summary>
    /// Ghi phản hồi NGUYÊN TỬ: kiểm tra trùng, INSERT <c>PhanHoiKhaoSat</c> và toàn bộ
    /// <c>CauTraLoiKhaoSat</c> trong một lần lưu.
    /// </summary>
    Task<SurveySubmitResult> SubmitAsync(
        int maKhaoSat,
        string maHocVien,
        SubmitResponseData data,
        CancellationToken cancellationToken);

    // ---------------- Quản trị: phản hồi & thống kê ----------------
    Task<PagedResult<SurveyResponseListItemDto>> ListResponsesAsync(
        int maKhaoSat,
        SurveyResponseListQuery query,
        CancellationToken cancellationToken);

    Task<SurveyStatisticsDto?> GetStatisticsAsync(
        int maKhaoSat,
        CancellationToken cancellationToken);
}
