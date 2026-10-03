using System.Globalization;
using Microsoft.Extensions.Configuration;

namespace LanguageCenter.Infrastructure.Authentication;

/// <summary>
/// Cấu hình JWT. Bind từ section <c>Jwt</c> trong configuration.
/// <para>
/// <b>Jwt:Key KHÔNG được đặt trong appsettings.json</b> — phải lấy từ User Secrets
/// (Development) hoặc biến môi trường <c>Jwt__Key</c> (Production).
/// </para>
/// </summary>
public sealed class JwtSettings
{
    public const string SectionName = "Jwt";

    /// <summary>Độ dài tối thiểu của signing key: 32 byte = 256 bit (yêu cầu của HS256).</summary>
    public const int MinimumSigningKeyBytes = 32;

    /// <summary>Thời hạn mặc định khi cấu hình không ghi rõ: 60 phút.</summary>
    public const int DefaultAccessTokenExpirationMinutes = 60;

    /// <summary>Signing key dạng Base64. BẮT BUỘC — không có giá trị mặc định.</summary>
    public string Key { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Thời hạn access token, tính bằng phút.
    /// Phase 2 dùng token ngắn hạn (mặc định 60 phút) và KHÔNG có refresh token.
    /// Giá trị <c>0</c> chỉ dùng để kiểm thử trường hợp token đã hết hạn.
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = DefaultAccessTokenExpirationMinutes;

    /// <summary>
    /// Đọc cấu hình từ section <c>Jwt</c>. Dùng CHUNG cho Infrastructure và Api
    /// để tránh 2 nơi parse cấu hình theo 2 cách khác nhau.
    /// </summary>
    public static JwtSettings FromConfiguration(IConfiguration configuration)
    {
        var section = configuration.GetSection(SectionName);

        var expirationMinutes = int.TryParse(
            section["AccessTokenExpirationMinutes"],
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var parsed)
            ? parsed
            : DefaultAccessTokenExpirationMinutes;

        return new JwtSettings
        {
            Key = section["Key"] ?? string.Empty,
            Issuer = section["Issuer"] ?? string.Empty,
            Audience = section["Audience"] ?? string.Empty,
            AccessTokenExpirationMinutes = expirationMinutes
        };
    }

    /// <summary>Kiểm tra cấu hình lúc khởi động — fail fast, không chạy với key thiếu/yếu.</summary>
    public void Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Issuer))
        {
            errors.Add("Jwt:Issuer chưa được cấu hình.");
        }

        if (string.IsNullOrWhiteSpace(Audience))
        {
            errors.Add("Jwt:Audience chưa được cấu hình.");
        }

        if (AccessTokenExpirationMinutes < 0)
        {
            errors.Add("Jwt:AccessTokenExpirationMinutes không được âm.");
        }

        if (string.IsNullOrWhiteSpace(Key))
        {
            errors.Add(
                "Jwt:Key chưa được cấu hình. Chạy lệnh:\n" +
                "  dotnet user-secrets set \"Jwt:Key\" \"<base64-key>\" --project backend/LanguageCenter.Api");
        }
        else
        {
            try
            {
                var keyBytes = Convert.FromBase64String(Key);

                if (keyBytes.Length < MinimumSigningKeyBytes)
                {
                    errors.Add(
                        $"Jwt:Key quá ngắn ({keyBytes.Length} byte). Cần tối thiểu {MinimumSigningKeyBytes} byte (256 bit).");
                }
            }
            catch (FormatException)
            {
                errors.Add("Jwt:Key phải là chuỗi Base64 hợp lệ.");
            }
        }

        if (errors.Count > 0)
        {
            throw new InvalidOperationException(
                "Cấu hình JWT không hợp lệ:" + Environment.NewLine +
                string.Join(Environment.NewLine, errors));
        }
    }

    /// <summary>Giải mã signing key từ Base64. Chỉ gọi sau khi <see cref="Validate"/> đã pass.</summary>
    public byte[] GetSigningKeyBytes() => Convert.FromBase64String(Key);
}
