namespace LanguageCenter.Application.Abstractions;

/// <summary>
/// Băm & kiểm tra mật khẩu.
/// Triển khai thật nằm ở Infrastructure (<c>PasswordHasherService</c>) bằng
/// <c>Microsoft.AspNetCore.Identity.PasswordHasher&lt;T&gt;</c> (PBKDF2-HMAC-SHA512).
/// Domain KHÔNG phụ thuộc ASP.NET Core Identity.
/// </summary>
public interface IPasswordHasherService
{
    /// <summary>Tạo hash mới cho mật khẩu. Mỗi lần gọi cho ra salt khác nhau.</summary>
    string HashPassword(string password);

    /// <summary>
    /// Kiểm tra mật khẩu với hash đã lưu.
    /// Trả về <c>false</c> (KHÔNG ném exception) nếu hash lưu trong DB không đúng định dạng.
    /// </summary>
    bool VerifyPassword(string password, string passwordHash);
}
