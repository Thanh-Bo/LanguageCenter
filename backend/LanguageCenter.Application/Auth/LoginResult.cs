using LanguageCenter.Application.Auth.Models;

namespace LanguageCenter.Application.Auth;

/// <summary>Lý do đăng nhập thất bại — CHỈ dùng để ghi log phía server.</summary>
public enum LoginFailureReason
{
    None = 0,

    /// <summary>Không tìm thấy tài khoản HOẶC sai mật khẩu (gộp chung để chống enumeration).</summary>
    InvalidCredentials = 1,

    /// <summary>Tài khoản tồn tại nhưng TrangThaiTaiKhoan khác "Hoạt động".</summary>
    AccountNotActive = 2
}

/// <summary>Kết quả đăng nhập.</summary>
public sealed class LoginResult
{
    private LoginResult(bool succeeded, LoginFailureReason failureReason, LoginResponse? response)
    {
        Succeeded = succeeded;
        FailureReason = failureReason;
        Response = response;
    }

    public bool Succeeded { get; }

    public LoginFailureReason FailureReason { get; }

    public LoginResponse? Response { get; }

    public static LoginResult Success(LoginResponse response)
        => new(true, LoginFailureReason.None, response);

    public static LoginResult Failure(LoginFailureReason reason)
        => new(false, reason, null);
}
