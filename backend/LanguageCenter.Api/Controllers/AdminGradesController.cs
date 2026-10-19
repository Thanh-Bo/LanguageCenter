using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Grades;
using LanguageCenter.Application.Grades.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Điểm số — Quản trị viên và Giáo viên.
/// <para>TRỢ GIẢNG KHÔNG được nhập điểm (chỉ điểm danh).</para>
/// <para>
/// GIỚI HẠN SCHEMA: <c>DiemSo.MaGiaoVien</c> là NOT NULL và FK tới <c>GiaoVien</c>,
/// nên Quản trị viên phải gửi <c>maGiaoVien</c> trong body khi tạo điểm.
/// </para>
/// </summary>
[ApiController]
[Route("api/admin/grades")]
[Authorize(Roles = RoleNames.QuanTriVien + "," + RoleNames.GiaoVien)]
public sealed class AdminGradesController : ControllerBase
{
    private readonly IGradeService _gradeService;

    public AdminGradesController(IGradeService gradeService)
    {
        _gradeService = gradeService;
    }

    /// <summary>Danh sách điểm — lọc theo đăng ký, đầu điểm, khóa, lớp, học viên, giáo viên.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GradeListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] GradeListQuery query,
        CancellationToken cancellationToken)
        => (await _gradeService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>
    /// Bảng điểm tổng kết của lớp — CHỈ TÍNH, KHÔNG LƯU vào database.
    /// <para>
    /// Điểm tổng kết (thang 10) = 10 × Σ((Điểm_i / ĐiểmTốiĐa_i) × TrọngSố_i) / Σ(TrọngSố_i).
    /// Chỉ tính khi đã đủ điểm của mọi đầu điểm đang hoạt động.
    /// </para>
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(GradeSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSummaryAsync(
        [FromQuery] GradeSummaryQuery query,
        CancellationToken cancellationToken)
        => (await _gradeService.GetClassSummaryAsync(query.MaLopHoc, cancellationToken))
            .ToActionResult();

    /// <summary>Chi tiết một bản ghi điểm.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GradeDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _gradeService.GetDetailAsync(id, cancellationToken)).ToActionResult();

    /// <summary>Nhập điểm mới.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(GradeDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateGradeRequest request,
        CancellationToken cancellationToken)
        => (await _gradeService.CreateAsync(request, BuildActor(), cancellationToken))
            .ToActionResult();

    /// <summary>Sửa điểm.</summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(GradeDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] UpdateGradeRequest request,
        CancellationToken cancellationToken)
        => (await _gradeService.UpdateAsync(id, request, BuildActor(), cancellationToken))
            .ToActionResult();

    /// <summary>Xóa điểm.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        => (await _gradeService.DeleteAsync(id, BuildActor(), cancellationToken)).ToActionResult();

    // ========================================================================
    // PRIVATE
    // ========================================================================

    private GradeActor BuildActor()
        => new(
            User.GetTenDangNhap() ?? string.Empty,
            User.GetVaiTro() == RoleNames.QuanTriVien);
}
