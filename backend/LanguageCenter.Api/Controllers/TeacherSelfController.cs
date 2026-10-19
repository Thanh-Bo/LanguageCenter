using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Sessions;
using LanguageCenter.Application.Sessions.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// API tự phục vụ của Giáo viên / Trợ giảng — CHỈ xem lịch dạy của CHÍNH MÌNH.
/// <para>Danh tính lấy từ JWT (<c>unique_name</c> = <c>MaGiaoVien</c>).</para>
/// </summary>
[ApiController]
[Route("api/teachers/me")]
[Authorize(Roles = RoleNames.GiaoVien + "," + RoleNames.TroGiang)]
public sealed class TeacherSelfController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public TeacherSelfController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    /// <summary>Ca học mà mình là người dạy thực tế hoặc lớp mình được phân công.</summary>
    [HttpGet("schedule")]
    [ProducesResponseType(typeof(IReadOnlyList<SessionListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyScheduleAsync(
        [FromQuery] DateOnly? tuNgay,
        [FromQuery] DateOnly? denNgay,
        CancellationToken cancellationToken)
    {
        var maGiaoVien = User.GetTenDangNhap();

        if (string.IsNullOrWhiteSpace(maGiaoVien))
        {
            return Unauthorized();
        }

        return (await _sessionService.ListForTeacherAsync(
                maGiaoVien, tuNgay, denNgay, cancellationToken))
            .ToActionResult();
    }

    /// <summary>Lớp mà mình đang được phân công giảng dạy.</summary>
    [HttpGet("classes")]
    [ProducesResponseType(typeof(IReadOnlyList<TeacherClassDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMyClassesAsync(CancellationToken cancellationToken)
    {
        var maGiaoVien = User.GetTenDangNhap();

        if (string.IsNullOrWhiteSpace(maGiaoVien))
        {
            return Unauthorized();
        }

        return (await _sessionService.ListMyClassesAsync(maGiaoVien, cancellationToken))
            .ToActionResult();
    }

    /// <summary>Học viên của một lớp mình phụ trách (403 nếu không được phân công).</summary>
    [HttpGet("classes/{maLopHoc:int}/students")]
    [ProducesResponseType(typeof(IReadOnlyList<TeacherClassStudentDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetMyClassStudentsAsync(
        int maLopHoc,
        CancellationToken cancellationToken)
    {
        var maGiaoVien = User.GetTenDangNhap();

        if (string.IsNullOrWhiteSpace(maGiaoVien))
        {
            return Unauthorized();
        }

        return (await _sessionService.ListMyClassStudentsAsync(
                maGiaoVien, maLopHoc, cancellationToken))
            .ToActionResult();
    }
}
