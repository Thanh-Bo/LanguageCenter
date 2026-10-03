namespace LanguageCenter.Application.Abstractions;

/// <summary>
/// Dữ liệu tài khoản cần cho authentication — KHÔNG chứa navigation phức tạp.
/// Đây là DTO nội bộ giữa Application và Infrastructure, KHÔNG bao giờ trả thẳng ra API.
/// </summary>
public sealed record AccountAuthInfo(
    int MaTaiKhoan,
    string TenDangNhap,
    string Email,
    string TrangThaiTaiKhoan,
    string MatKhauHash,
    string TenVaiTro);
