using LanguageCenter.Application.Abstractions;
using LanguageCenter.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace LanguageCenter.Infrastructure.Authentication;

/// <summary>
/// Băm mật khẩu bằng <see cref="PasswordHasher{TUser}"/> chuẩn của ASP.NET Core Identity
/// (PBKDF2 — HMAC-SHA512, salt ngẫu nhiên 128 bit, 100.000 vòng lặp).
/// </summary>
/// <remarks>
/// <see cref="PasswordHasher{TUser}"/> mặc định KHÔNG dùng tới tham số <c>user</c>
/// (chỉ cần thiết cho implementation có salt riêng theo từng user), nên truyền <c>null</c> là an toàn.
/// Domain không hề tham chiếu ASP.NET Core Identity — chỉ Infrastructure tham chiếu.
/// </remarks>
public sealed class PasswordHasherService : IPasswordHasherService
{
    private readonly PasswordHasher<TaiKhoan> _passwordHasher = new();

    public string HashPassword(string password)
    {
        ArgumentNullException.ThrowIfNull(password);

        return _passwordHasher.HashPassword(user: null!, password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordHash))
        {
            return false;
        }

        try
        {
            var result = _passwordHasher.VerifyHashedPassword(user: null!, passwordHash, password);

            // SuccessRehashNeeded = hash cũ nhưng vẫn đúng -> coi như thành công.
            return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
        }
        catch (FormatException)
        {
            // Hash lưu trong database không đúng định dạng (ví dụ dữ liệu placeholder).
            // Trả về false thay vì ném exception để không lộ chi tiết ra ngoài.
            return false;
        }
    }
}
