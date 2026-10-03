namespace LanguageCenter.Application.Abstractions;

/// <summary>Kết quả sinh JWT access token.</summary>
/// <param name="AccessToken">Chuỗi JWT hoàn chỉnh.</param>
/// <param name="ExpiresAtUtc">Thời điểm hết hạn (UTC, Kind = Utc).</param>
public sealed record JwtTokenResult(string AccessToken, DateTime ExpiresAtUtc);

/// <summary>
/// Sinh JWT access token. Triển khai thật nằm ở Infrastructure (<c>JwtTokenGenerator</c>).
/// Application chỉ phụ thuộc abstraction này, KHÔNG biết gì về thư viện JWT.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <param name="maTaiKhoan">Khóa chính TaiKhoan — đưa vào claim <c>sub</c>.</param>
    /// <param name="tenDangNhap">Tên đăng nhập — đưa vào claim <c>unique_name</c>.</param>
    /// <param name="vaiTro">Tên vai trò tiếng Việt lấy từ <c>VaiTro.TenVaiTro</c> — đưa vào claim <c>role</c>.</param>
    JwtTokenResult Generate(int maTaiKhoan, string tenDangNhap, string vaiTro);
}
