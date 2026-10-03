namespace LanguageCenter.Application.Auth;

/// <summary>
/// Thông báo trả về cho client khi authentication thất bại.
/// Dùng MỘT message chung cho mọi trường hợp để chống account enumeration
/// (không phân biệt "tài khoản không tồn tại" với "sai mật khẩu").
/// </summary>
public static class AuthMessages
{
    public const string InvalidCredentials = "Tên đăng nhập hoặc mật khẩu không đúng.";
}
