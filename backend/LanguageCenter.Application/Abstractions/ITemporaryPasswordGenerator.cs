namespace LanguageCenter.Application.Abstractions;

/// <summary>
/// Sinh mật khẩu tạm khi Admin tạo tài khoản hoặc reset mật khẩu.
/// Triển khai thật ở Infrastructure (<c>TemporaryPasswordGenerator</c>) dùng CSPRNG.
/// </summary>
public interface ITemporaryPasswordGenerator
{
    /// <summary>Sinh mật khẩu tạm ngẫu nhiên, đủ mạnh, không hard-code.</summary>
    string Generate();
}
