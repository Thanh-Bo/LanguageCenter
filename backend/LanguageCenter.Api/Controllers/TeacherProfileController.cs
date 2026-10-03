using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Teachers;
using LanguageCenter.Application.Teachers.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Hồ sơ của CHÍNH giáo viên / trợ giảng đang đăng nhập.
/// <para>
/// KHÔNG có tham số teacherId: danh tính lấy từ JWT
/// (claim <c>unique_name</c> = TenDangNhap = MaGiaoVien), nên GV001 không thể xem hồ sơ người khác.
/// </para>
/// </summary>
[ApiController]
[Route("api/teacher/profile")]
[Authorize(Roles = RoleNames.GiaoVien + "," + RoleNames.TroGiang)]
public sealed class TeacherProfileController : ControllerBase
{
    private readonly ITeacherService _teacherService;

    public TeacherProfileController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    /// <summary>Xem hồ sơ của chính mình (kèm Bằng cấp / Chứng chỉ — chỉ đọc).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(TeacherDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfileAsync(CancellationToken cancellationToken)
    {
        var tenDangNhap = User.GetTenDangNhap();

        if (tenDangNhap is null)
        {
            return Unauthorized(new { message = "Token không hợp lệ." });
        }

        return (await _teacherService.GetMyProfileAsync(tenDangNhap, cancellationToken)).ToActionResult();
    }

    /// <summary>
    /// Tự cập nhật hồ sơ: HoTen, GioiTinh, SoDienThoai, Email, DiaChi.
    /// KHÔNG thể tự đổi LoaiGiaoVien / ChuyenMon / SoNamKinhNghiem / NgayVaoLam / TrangThai / vai trò.
    /// </summary>
    [HttpPatch]
    [ProducesResponseType(typeof(TeacherDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateMyProfileAsync(
        [FromBody] UpdateMyTeacherProfileRequest request,
        CancellationToken cancellationToken)
    {
        var tenDangNhap = User.GetTenDangNhap();

        if (tenDangNhap is null)
        {
            return Unauthorized(new { message = "Token không hợp lệ." });
        }

        return (await _teacherService.UpdateMyProfileAsync(tenDangNhap, request, cancellationToken)).ToActionResult();
    }
}
