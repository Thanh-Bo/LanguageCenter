using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Students;
using LanguageCenter.Application.Students.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Hồ sơ của CHÍNH học viên đang đăng nhập.
/// <para>
/// KHÔNG có tham số studentId / MaHocVien: danh tính lấy từ JWT
/// (claim <c>unique_name</c> = TenDangNhap = MaHocVien), nên HV001 không thể xem hồ sơ người khác.
/// </para>
/// </summary>
[ApiController]
[Route("api/student/profile")]
[Authorize(Roles = RoleNames.HocVien)]
public sealed class StudentProfileController : ControllerBase
{
    private readonly IStudentService _studentService;

    public StudentProfileController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>Xem hồ sơ của chính mình (kèm thông tin phụ huynh nếu có).</summary>
    [HttpGet]
    [ProducesResponseType(typeof(StudentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetMyProfileAsync(CancellationToken cancellationToken)
    {
        var tenDangNhap = User.GetTenDangNhap();

        if (tenDangNhap is null)
        {
            return Unauthorized(new { message = "Token không hợp lệ." });
        }

        return (await _studentService.GetMyProfileAsync(tenDangNhap, cancellationToken)).ToActionResult();
    }

    /// <summary>
    /// Tự cập nhật hồ sơ: HoTen, GioiTinh, SoDienThoai, Email, DiaChi, MucTieuHocTap.
    /// KHÔNG thể tự đổi TrangThaiHocTap / NgayNhapHoc / MaHocVien / vai trò / trạng thái tài khoản.
    /// </summary>
    [HttpPatch]
    [ProducesResponseType(typeof(StudentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateMyProfileAsync(
        [FromBody] UpdateMyStudentProfileRequest request,
        CancellationToken cancellationToken)
    {
        var tenDangNhap = User.GetTenDangNhap();

        if (tenDangNhap is null)
        {
            return Unauthorized(new { message = "Token không hợp lệ." });
        }

        return (await _studentService.UpdateMyProfileAsync(tenDangNhap, request, cancellationToken)).ToActionResult();
    }
}
