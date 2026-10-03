using LanguageCenter.Application.Auth.Models;

namespace LanguageCenter.Application.Auth;

/// <summary>Nghiệp vụ authentication. Controller chỉ gọi interface này.</summary>
public interface IAuthService
{
    Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Lấy thông tin tài khoản hiện tại.
    /// <paramref name="maTaiKhoan"/> BẮT BUỘC lấy từ JWT, không bao giờ lấy từ query/body.
    /// </summary>
    Task<CurrentUserDto?> GetCurrentUserAsync(int maTaiKhoan, CancellationToken cancellationToken);

    /// <summary>
    /// Đổi mật khẩu cho CHÍNH tài khoản đang đăng nhập.
    /// <paramref name="maTaiKhoan"/> BẮT BUỘC lấy từ JWT.
    /// </summary>
    Task<ChangePasswordResult> ChangePasswordAsync(
        int maTaiKhoan,
        ChangePasswordRequest request,
        CancellationToken cancellationToken);
}
