using LanguageCenter.Application.Abstractions;

namespace LanguageCenter.Infrastructure.Authentication;

/// <summary>
/// Mật khẩu mặc định dùng khi Admin tạo tài khoản mới (học viên / giáo viên / trợ giảng)
/// hoặc khi Admin đặt lại mật khẩu.
/// </summary>
/// <remarks>
/// <list type="bullet">
///   <item>Yêu cầu nghiệp vụ của trung tâm: mọi tài khoản mới dùng chung mật khẩu <c>111111</c>,
///   người dùng đăng nhập lần đầu rồi tự đổi mật khẩu.</item>
///   <item>Đây là mật khẩu DUY NHẤT được hard-code trong hệ thống.</item>
///   <item>Vẫn chỉ lưu dạng hash — không lưu plain text, không ghi log.</item>
///   <item><c>LanguageCenter.Application.Auth.PasswordPolicy</c> KHÔNG áp cho mật khẩu mặc định;
///   policy chỉ kiểm tra khi người dùng TỰ đổi mật khẩu.</item>
/// </list>
/// </remarks>
public sealed class TemporaryPasswordGenerator : ITemporaryPasswordGenerator
{
    /// <summary>Mật khẩu mặc định của mọi tài khoản mới / sau khi đặt lại mật khẩu.</summary>
    public const string MatKhauMacDinh = "111111";

    public string Generate() => MatKhauMacDinh;
}
