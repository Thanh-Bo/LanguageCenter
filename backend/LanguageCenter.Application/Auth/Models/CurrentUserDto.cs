namespace LanguageCenter.Application.Auth.Models;

/// <summary>
/// Thông tin người dùng trả về cho client.
/// TUYỆT ĐỐI KHÔNG chứa MatKhauHash, connection string hay dữ liệu nhạy cảm khác.
/// </summary>
public sealed class CurrentUserDto
{
    /// <summary>MaTaiKhoan.</summary>
    public int Id { get; init; }

    /// <summary>TenDangNhap.</summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>Email.</summary>
    public string Email { get; init; } = string.Empty;

    /// <summary>TenVaiTro — giữ nguyên tiếng Việt như trong database.</summary>
    public string Role { get; init; } = string.Empty;
}
