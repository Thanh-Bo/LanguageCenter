namespace LanguageCenter.Application.Auth;

/// <summary>Lý do đổi mật khẩu thất bại.</summary>
public enum ChangePasswordFailureReason
{
    None = 0,
    InvalidCurrentPassword = 1,
    NewPasswordPolicyViolation = 2,
    PasswordConfirmationMismatch = 3,
    NewPasswordSameAsCurrent = 4,
    AccountNotFound = 5
}

/// <summary>Kết quả đổi mật khẩu.</summary>
public sealed class ChangePasswordResult
{
    private ChangePasswordResult(bool succeeded, ChangePasswordFailureReason reason, IReadOnlyList<string> errors)
    {
        Succeeded = succeeded;
        FailureReason = reason;
        Errors = errors;
    }

    public bool Succeeded { get; }

    public ChangePasswordFailureReason FailureReason { get; }

    /// <summary>Chi tiết lỗi để trả về client (400).</summary>
    public IReadOnlyList<string> Errors { get; }

    public static ChangePasswordResult Success()
        => new(true, ChangePasswordFailureReason.None, []);

    public static ChangePasswordResult Failure(ChangePasswordFailureReason reason, IReadOnlyList<string> errors)
        => new(false, reason, errors);
}
