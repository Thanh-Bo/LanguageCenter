using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Grades;
using LanguageCenter.Application.Grades.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Điểm số — Quản trị viên, Giáo viên và Trợ giảng (CHỈ ĐỌC).
/// <para>
/// TRỢ GIẢNG chỉ được ĐỌC điểm của lớp mình được phân công, KHÔNG được nhập/sửa/xoá điểm.
/// </para>
/// <para>
/// GIỚI HẠN SCHEMA: <c>DiemSo.MaGiaoVien</c> là NOT NULL và FK tới <c>GiaoVien</c>,
/// nên Quản trị viên phải gửi <c>maGiaoVien</c> trong body khi tạo điểm.
/// </para>
/// </summary>
[ApiController]
[Route("api/admin/grades")]
[Authorize(Roles = RoleNames.QuanTriVien + "," + RoleNames.GiaoVien + "," + RoleNames.TroGiang)]
public sealed class AdminGradesController : ControllerBase
{
    /// <summary>Quản trị viên và Giáo viên — nhóm được GHI điểm.</summary>
    private const string GhiRoles = RoleNames.QuanTriVien + "," + RoleNames.GiaoVien;

    private readonly IGradeService _gradeService;
    private readonly IAcademicLookupStore _lookupStore;

    public AdminGradesController(IGradeService gradeService, IAcademicLookupStore lookupStore)
    {
        _gradeService = gradeService;
        _lookupStore = lookupStore;
    }

    /// <summary>Danh sách điểm — lọc theo đăng ký, đầu điểm, khóa, lớp, học viên, giáo viên.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<GradeListItemDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] GradeListQuery query,
        CancellationToken cancellationToken)
    {
        var scopeError = await CheckReadScopeAsync(query.MaLopHoc, cancellationToken);

        if (scopeError is not null)
        {
            return OperationResult<PagedResult<GradeListItemDto>>.Forbidden(scopeError).ToActionResult();
        }

        return (await _gradeService.ListAsync(query, cancellationToken)).ToActionResult();
    }

    /// <summary>
    /// Bảng điểm tổng kết của lớp — CHỈ TÍNH, KHÔNG LƯU vào database.
    /// <para>
    /// Điểm tổng kết (thang 10) = 10 × Σ((Điểm_i / ĐiểmTốiĐa_i) × TrọngSố_i) / Σ(TrọngSố_i).
    /// Chỉ tính khi đã đủ điểm của mọi đầu điểm đang hoạt động.
    /// </para>
    /// </summary>
    [HttpGet("summary")]
    [ProducesResponseType(typeof(GradeSummaryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetSummaryAsync(
        [FromQuery] GradeSummaryQuery query,
        CancellationToken cancellationToken)
    {
        var scopeError = await CheckReadScopeAsync(query.MaLopHoc, cancellationToken);

        if (scopeError is not null)
        {
            return OperationResult<GradeSummaryDto>.Forbidden(scopeError).ToActionResult();
        }

        return (await _gradeService.GetClassSummaryAsync(query.MaLopHoc, cancellationToken))
            .ToActionResult();
    }

    /// <summary>
    /// Chi tiết một bản ghi điểm.
    /// <para>Trợ giảng chỉ xem được điểm thuộc lớp mình được phân công.</para>
    /// </summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(GradeDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
    {
        var result = await _gradeService.GetDetailAsync(id, cancellationToken);

        if (!result.Succeeded)
        {
            return result.ToActionResult();
        }

        var scopeError = await CheckReadScopeAsync(result.Value?.MaLopHoc, cancellationToken);

        if (scopeError is not null)
        {
            return OperationResult<GradeDetailDto>.Forbidden(scopeError).ToActionResult();
        }

        return result.ToActionResult();
    }

    /// <summary>Nhập điểm mới — KHÔNG dành cho Trợ giảng.</summary>
    [HttpPost]
    [Authorize(Roles = GhiRoles)]
    [ProducesResponseType(typeof(GradeDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateGradeRequest request,
        CancellationToken cancellationToken)
        => (await _gradeService.CreateAsync(request, BuildActor(), cancellationToken))
            .ToActionResult();

    /// <summary>Sửa điểm — KHÔNG dành cho Trợ giảng.</summary>
    [HttpPatch("{id:int}")]
    [Authorize(Roles = GhiRoles)]
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

    /// <summary>Xóa điểm — KHÔNG dành cho Trợ giảng.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = GhiRoles)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        => (await _gradeService.DeleteAsync(id, BuildActor(), cancellationToken)).ToActionResult();

    // ========================================================================
    // PRIVATE
    // ========================================================================

    /// <summary>
    /// Trợ giảng chỉ được ĐỌC điểm của lớp mình được phân công.
    /// Quản trị viên / Giáo viên / Nhân viên → không giới hạn ở đây.
    /// Trả về <c>null</c> nếu hợp lệ, ngược lại là lý do từ chối.
    /// </summary>
    private async Task<string?> CheckReadScopeAsync(int? maLopHoc, CancellationToken cancellationToken)
    {
        if (User.GetVaiTro() != RoleNames.TroGiang)
        {
            return null;
        }

        if (maLopHoc is not { } lop)
        {
            return "Trợ giảng phải chọn một lớp cụ thể để xem điểm.";
        }

        var maGiaoVien = User.GetTenDangNhap();

        if (string.IsNullOrWhiteSpace(maGiaoVien))
        {
            return "Không xác định được tài khoản từ token.";
        }

        return await _lookupStore.TeacherAssignedToClassAsync(lop, maGiaoVien, cancellationToken)
            ? null
            : "Bạn không được phân công giảng dạy lớp này.";
    }

    private GradeActor BuildActor()
        => new(
            User.GetTenDangNhap() ?? string.Empty,
            User.GetVaiTro() == RoleNames.QuanTriVien);
}
