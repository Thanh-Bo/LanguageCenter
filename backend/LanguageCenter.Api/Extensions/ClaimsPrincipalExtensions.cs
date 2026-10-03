using System.Globalization;
using System.Security.Claims;
using LanguageCenter.Infrastructure.Authentication;

namespace LanguageCenter.Api.Extensions;

/// <summary>
/// Đọc thông tin người dùng TỪ JWT — nguồn duy nhất đáng tin cho danh tính.
/// TUYỆT ĐỐI KHÔNG lấy MaTaiKhoan từ query string, route hay body.
/// </summary>
public static class ClaimsPrincipalExtensions
{
    /// <summary>MaTaiKhoan từ claim <c>sub</c>. Trả về null nếu token thiếu/sai định dạng.</summary>
    public static int? GetMaTaiKhoan(this ClaimsPrincipal principal)
    {
        var rawValue = principal.FindFirst(JwtTokenGenerator.UserIdClaim)?.Value;

        return int.TryParse(rawValue, NumberStyles.Integer, CultureInfo.InvariantCulture, out var maTaiKhoan)
            ? maTaiKhoan
            : null;
    }

    /// <summary>TenDangNhap từ claim <c>unique_name</c>.</summary>
    public static string? GetTenDangNhap(this ClaimsPrincipal principal)
        => principal.FindFirst(JwtTokenGenerator.UsernameClaim)?.Value;

    /// <summary>TenVaiTro từ claim <c>role</c> (giá trị tiếng Việt như trong database).</summary>
    public static string? GetVaiTro(this ClaimsPrincipal principal)
        => principal.FindFirst(JwtTokenGenerator.RoleClaim)?.Value;
}
