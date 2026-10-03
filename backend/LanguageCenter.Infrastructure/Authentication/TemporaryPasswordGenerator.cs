using System.Security.Cryptography;
using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Auth;

namespace LanguageCenter.Infrastructure.Authentication;

/// <summary>
/// Sinh mật khẩu tạm ngẫu nhiên bằng CSPRNG (<see cref="RandomNumberGenerator"/>).
/// </summary>
/// <remarks>
/// <list type="bullet">
///   <item>Độ dài 12 ký tự — thoả <see cref="PasswordPolicy"/> (>= 8, có chữ, có số).</item>
///   <item>Luôn chứa đủ 4 nhóm: chữ hoa, chữ thường, chữ số, ký tự đặc biệt.</item>
///   <item>Loại bỏ ký tự dễ nhầm khi đọc/đánh máy: <c>0 O 1 l I</c> và <c>" ' ` \</c>.</item>
///   <item>KHÔNG hard-code, KHÔNG dùng mặc định như 123456 / password / mã tài khoản / ngày sinh.</item>
///   <item>Mật khẩu sinh ra KHÔNG bao giờ được ghi log — chỉ trả về đúng một lần trong response.</item>
/// </list>
/// </remarks>
public sealed class TemporaryPasswordGenerator : ITemporaryPasswordGenerator
{
    private const int Length = 12;

    private const string Uppercase = "ABCDEFGHJKLMNPQRSTUVWXYZ";   // bỏ I, O
    private const string Lowercase = "abcdefghijkmnopqrstuvwxyz";  // bỏ l
    private const string Digits = "23456789";                      // bỏ 0, 1
    private const string Specials = "!@#$%^&*()-_=+[]{}:,.?";

    private static readonly string AllCharacters = Uppercase + Lowercase + Digits + Specials;

    public string Generate()
    {
        var characters = new char[Length];

        // Bắt buộc có đủ 4 nhóm ký tự.
        characters[0] = Pick(Uppercase);
        characters[1] = Pick(Lowercase);
        characters[2] = Pick(Digits);
        characters[3] = Pick(Specials);

        for (var i = 4; i < Length; i++)
        {
            characters[i] = Pick(AllCharacters);
        }

        // Fisher-Yates shuffle bằng CSPRNG để vị trí các nhóm ký tự không đoán được.
        for (var i = Length - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (characters[i], characters[j]) = (characters[j], characters[i]);
        }

        return new string(characters);
    }

    private static char Pick(string source)
        => source[RandomNumberGenerator.GetInt32(source.Length)];
}
