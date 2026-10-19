using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Surveys;
using LanguageCenter.Application.Surveys.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Quản lý khảo sát — Quản trị viên và Nhân viên học vụ.
/// </summary>
[ApiController]
[Route("api/admin/surveys")]
[Authorize(Roles = RoleNames.QuanTriVien + "," + RoleNames.NhanVienHocVu)]
public sealed class AdminSurveysController : ControllerBase
{
    private readonly ISurveyService _surveyService;

    public AdminSurveysController(ISurveyService surveyService)
    {
        _surveyService = surveyService;
    }

    /// <summary>Danh sách khảo sát — lọc theo trạng thái, khoảng ngày bắt đầu, từ khóa.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SurveyListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] SurveyListQuery query,
        CancellationToken cancellationToken)
        => (await _surveyService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết khảo sát kèm danh sách câu hỏi.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SurveyDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _surveyService.GetDetailAsync(id, cancellationToken)).ToActionResult();

    /// <summary>Tạo khảo sát. Trạng thái mặc định là "Hoạt động".</summary>
    [HttpPost]
    [ProducesResponseType(typeof(SurveyDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateSurveyRequest request,
        CancellationToken cancellationToken)
        => (await _surveyService.CreateAsync(request, cancellationToken)).ToActionResult();

    /// <summary>Cập nhật thông tin khảo sát.</summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(SurveyDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] UpdateSurveyRequest request,
        CancellationToken cancellationToken)
        => (await _surveyService.UpdateAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>Mở / tạm dừng / kết thúc khảo sát: "Hoạt động" | "Tạm dừng" | "Đã kết thúc".</summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(SurveyDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatusAsync(
        int id,
        [FromBody] UpdateStatusRequest request,
        CancellationToken cancellationToken)
        => (await _surveyService.UpdateStatusAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>Xóa khảo sát — 409 nếu đã có phản hồi.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        => (await _surveyService.DeleteAsync(id, cancellationToken)).ToActionResult();

    // ------------------------------------------------------------------
    // CÂU HỎI
    // ------------------------------------------------------------------

    /// <summary>Danh sách câu hỏi của khảo sát (sắp theo thứ tự).</summary>
    [HttpGet("{id:int}/questions")]
    [ProducesResponseType(typeof(IReadOnlyList<SurveyQuestionDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListQuestionsAsync(int id, CancellationToken cancellationToken)
        => (await _surveyService.ListQuestionsAsync(id, cancellationToken)).ToActionResult();

    /// <summary>Thêm câu hỏi. Thứ tự không được trùng trong cùng khảo sát.</summary>
    [HttpPost("{id:int}/questions")]
    [ProducesResponseType(typeof(SurveyQuestionDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateQuestionAsync(
        int id,
        [FromBody] CreateSurveyQuestionRequest request,
        CancellationToken cancellationToken)
        => (await _surveyService.CreateQuestionAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>Cập nhật câu hỏi.</summary>
    [HttpPatch("{id:int}/questions/{maCauHoi:int}")]
    [ProducesResponseType(typeof(SurveyQuestionDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateQuestionAsync(
        int id,
        int maCauHoi,
        [FromBody] UpdateSurveyQuestionRequest request,
        CancellationToken cancellationToken)
        => (await _surveyService.UpdateQuestionAsync(id, maCauHoi, request, cancellationToken))
            .ToActionResult();

    /// <summary>Xóa câu hỏi — 409 nếu đã có câu trả lời.</summary>
    [HttpDelete("{id:int}/questions/{maCauHoi:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteQuestionAsync(
        int id,
        int maCauHoi,
        CancellationToken cancellationToken)
        => (await _surveyService.DeleteQuestionAsync(id, maCauHoi, cancellationToken)).ToActionResult();

    // ------------------------------------------------------------------
    // PHẢN HỒI & THỐNG KÊ
    // ------------------------------------------------------------------

    /// <summary>Danh sách phản hồi của khảo sát.</summary>
    [HttpGet("{id:int}/responses")]
    [ProducesResponseType(typeof(PagedResult<SurveyResponseListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ListResponsesAsync(
        int id,
        [FromQuery] SurveyResponseListQuery query,
        CancellationToken cancellationToken)
        => (await _surveyService.ListResponsesAsync(id, query, cancellationToken)).ToActionResult();

    /// <summary>
    /// Thống kê kết quả khảo sát.
    /// <para>
    /// Với câu hỏi dạng đánh giá: trả về số lượt, giá trị trung bình và phân bố 0..5.
    /// Với câu hỏi tự luận: trả về tối đa 50 nội dung (KHÔNG kèm danh tính người trả lời).
    /// </para>
    /// </summary>
    [HttpGet("{id:int}/statistics")]
    [ProducesResponseType(typeof(SurveyStatisticsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetStatisticsAsync(int id, CancellationToken cancellationToken)
        => (await _surveyService.GetStatisticsAsync(id, cancellationToken)).ToActionResult();
}
