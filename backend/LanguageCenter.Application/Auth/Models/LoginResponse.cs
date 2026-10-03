namespace LanguageCenter.Application.Auth.Models;

/// <summary>Body trả về khi đăng nhập thành công.</summary>
public sealed class LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;

    public string TokenType { get; init; } = "Bearer";

    /// <summary>Thời điểm token hết hạn (UTC).</summary>
    public DateTime ExpiresAt { get; init; }

    public CurrentUserDto User { get; init; } = new();
}
