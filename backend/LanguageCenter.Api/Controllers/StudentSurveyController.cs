using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Surveys;
using LanguageCenter.Application.Surveys.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Khảo sát dành cho Học viên — danh tính lấy từ JWT, chỉ dữ liệu của chính mình.
/// <para>
/// Mỗi học viên chỉ được trả lời MỘT LẦN cho mỗi khảo sát
/// (ràng buộc <c>UQ_PhanHoiKhaoSat (MaKhaoSat, MaHocVien)</c> của database).
/// </para>
/// </summary>
[ApiController]
[Route("api/students/me/surveys")]
[Authorize(Roles = RoleNames.HocVien)]
public sealed class StudentSurveyController : ControllerBase
{
    private readonly ISurveyService _surveyService;

    public StudentSurveyController(ISurveyService surveyService)
    {
        _surveyService = surveyService;
    }

    /// <summary>Khảo sát đang hoạt động (kèm câu hỏi và trạng thái đã trả lời của tôi).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<MySurveyDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListAsync(CancellationToken cancellationToken)
        => (await _surveyService.ListForStudentAsync(BuildActor(), cancellationToken)).ToActionResult();

    /// <summary>Chi tiết khảo sát (chỉ khảo sát đang hoạt động hoặc khảo sát tôi đã trả lời).</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MySurveyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _surveyService.GetForStudentAsync(id, BuildActor(), cancellationToken))
            .ToActionResult();

    /// <summary>Phản hồi tôi đã gửi cho khảo sát này.</summary>
    [HttpGet("{id:int}/response")]
    [ProducesResponseType(typeof(SurveyResponseDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyResponseAsync(int id, CancellationToken cancellationToken)
        => (await _surveyService.GetMyResponseAsync(id, BuildActor(), cancellationToken))
            .ToActionResult();

    /// <summary>Gửi phản hồi. Mỗi học viên chỉ được trả lời một lần (409 nếu đã gửi).</summary>
    [HttpPost("{id:int}/responses")]
    [ProducesResponseType(typeof(SurveyResponseDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> SubmitAsync(
        int id,
        [FromBody] SubmitSurveyResponseRequest request,
        CancellationToken cancellationToken)
        => (await _surveyService.SubmitAsync(id, request, BuildActor(), cancellationToken))
            .ToActionResult();

    // ========================================================================
    // PRIVATE
    // ========================================================================

    private SurveyStudentActor BuildActor() => new(User.GetTenDangNhap() ?? string.Empty);
}
