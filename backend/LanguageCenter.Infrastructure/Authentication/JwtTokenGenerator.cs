using System.Globalization;
using LanguageCenter.Application.Abstractions;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace LanguageCenter.Infrastructure.Authentication;

/// <summary>
/// Sinh JWT access token (HMAC-SHA256).
/// </summary>
/// <remarks>
/// Claims phát ra CỐ ĐỊNH (không dùng claim mapping của ASP.NET Core):
/// <list type="bullet">
///   <item><c>sub</c> — MaTaiKhoan</item>
///   <item><c>unique_name</c> — TenDangNhap</item>
///   <item><c>role</c> — TenVaiTro (nguyên tiếng Việt)</item>
///   <item><c>jti</c> — định danh token</item>
/// </list>
/// Phía Api sẽ đặt <c>MapInboundClaims = false</c> để giữ nguyên tên claim này.
/// </remarks>
public sealed class JwtTokenGenerator : IJwtTokenGenerator
{
    public const string UserIdClaim = "sub";
    public const string UsernameClaim = "unique_name";
    public const string RoleClaim = "role";
    public const string TokenIdClaim = "jti";

    private readonly JwtSettings _settings;

    public JwtTokenGenerator(JwtSettings settings)
    {
        _settings = settings;
    }

    public JwtTokenResult Generate(int maTaiKhoan, string tenDangNhap, string vaiTro)
    {
        var nowUtc = DateTime.UtcNow;
        var expiresAtUtc = nowUtc.AddMinutes(_settings.AccessTokenExpirationMinutes);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _settings.Issuer,
            Audience = _settings.Audience,
            IssuedAt = nowUtc,
            NotBefore = nowUtc,
            Expires = expiresAtUtc,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(_settings.GetSigningKeyBytes()),
                SecurityAlgorithms.HmacSha256),
            Claims = new Dictionary<string, object>
            {
                [UserIdClaim] = maTaiKhoan.ToString(CultureInfo.InvariantCulture),
                [UsernameClaim] = tenDangNhap,
                [RoleClaim] = vaiTro,
                [TokenIdClaim] = Guid.NewGuid().ToString("N")
            }
        };

        var handler = new JsonWebTokenHandler();
        var accessToken = handler.CreateToken(descriptor);

        return new JwtTokenResult(accessToken, DateTime.SpecifyKind(expiresAtUtc, DateTimeKind.Utc));
    }
}
