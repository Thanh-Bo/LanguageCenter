namespace LanguageCenter.Application.Abstractions;

/// <summary>
/// Cung cấp mật khẩu khi Admin tạo tài khoản hoặc đặt lại mật khẩu.
/// Triển khai ở Infrastructure (<c>TemporaryPasswordGenerator</c>) trả về mật khẩu MẶC ĐỊNH
/// của trung tâm (<c>111111</c>) theo yêu cầu nghiệp vụ.
/// </summary>
public interface ITemporaryPasswordGenerator
{
    /// <summary>Mật khẩu mặc định gán cho tài khoản mới / sau khi đặt lại.</summary>
    string Generate();
}
