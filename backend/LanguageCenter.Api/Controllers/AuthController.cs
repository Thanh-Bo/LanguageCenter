using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Auth;
using LanguageCenter.Application.Auth.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Authentication + Authorization.
/// Controller CHỈ nhận request, gọi <see cref="IAuthService"/> và trả response —
/// không chứa logic nghiệp vụ, không try/catch (đã có Global Exception Handling).
/// </summary>
[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
        _authService = authService;
        _logger = logger;
    }

    /// <summary>Đăng nhập bằng tài khoản trong bảng TaiKhoan.</summary>
    /// <response code="200">Đăng nhập thành công — trả JWT access token.</response>
    /// <response code="400">Dữ liệu gửi lên không hợp lệ.</response>
    /// <response code="401">Sai thông tin đăng nhập hoặc tài khoản không ở trạng thái "Hoạt động".</response>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LoginAsync(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _authService.LoginAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            // Ghi log LÝ DO THẬT để debug — KHÔNG bao giờ ghi mật khẩu.
            // Client vẫn chỉ nhận 1 message chung để chống account enumeration.
            _logger.LogWarning(
                "Đăng nhập thất bại cho {TenDangNhap}. Lý do: {LyDo}",
                request.Username,
                result.FailureReason);

            return Unauthorized(new { message = AuthMessages.InvalidCredentials });
        }

        return Ok(result.Response);
    }

    /// <summary>
    /// Thông tin tài khoản ĐANG ĐĂNG NHẬP.
    /// Danh tính lấy từ JWT — không có tham số nào cho phép chọn tài khoản khác.
    /// </summary>
    /// <response code="200">Trả thông tin tài khoản hiện tại.</response>
    /// <response code="401">Thiếu token / token sai / token hết hạn.</response>
    [HttpGet("me")]
    [Authorize]
    [ProducesResponseType(typeof(CurrentUserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUserAsync(CancellationToken cancellationToken)
    {
        var maTaiKhoan = User.GetMaTaiKhoan();

        if (maTaiKhoan is null)
        {
            return Unauthorized(new { message = "Token không hợp lệ." });
        }

        var currentUser = await _authService.GetCurrentUserAsync(maTaiKhoan.Value, cancellationToken);

        if (currentUser is null)
        {
            return Unauthorized(new { message = "Tài khoản không còn tồn tại." });
        }

        return Ok(currentUser);
    }

    /// <summary>
    /// Đổi mật khẩu cho CHÍNH tài khoản đang đăng nhập.
    /// Không nhận MaTaiKhoan từ client — luôn lấy từ JWT.
    /// </summary>
    /// <response code="204">Đổi mật khẩu thành công.</response>
    /// <response code="400">Sai mật khẩu hiện tại, vi phạm chính sách, hoặc xác nhận không khớp.</response>
    /// <response code="401">Chưa đăng nhập.</response>
    [HttpPost("change-password")]
    [Authorize]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePasswordAsync(
        [FromBody] ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        var maTaiKhoan = User.GetMaTaiKhoan();

        if (maTaiKhoan is null)
        {
            return Unauthorized(new { message = "Token không hợp lệ." });
        }

        var result = await _authService.ChangePasswordAsync(maTaiKhoan.Value, request, cancellationToken);

        if (!result.Succeeded)
        {
            _logger.LogWarning(
                "Đổi mật khẩu thất bại cho MaTaiKhoan={MaTaiKhoan}. Lý do: {LyDo}",
                maTaiKhoan.Value,
                result.FailureReason);

            return BadRequest(new
            {
                message = "Đổi mật khẩu thất bại.",
                errors = result.Errors
            });
        }

        return NoContent();
    }
}
