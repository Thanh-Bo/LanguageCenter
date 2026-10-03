namespace LanguageCenter.Application.Auth;

/// <summary>
/// Chính sách mật khẩu — áp dụng NHẤT QUÁN cho mọi chỗ đổi/đặt mật khẩu.
/// Yêu cầu tối thiểu: >= 8 ký tự, có ít nhất 1 chữ cái, có ít nhất 1 chữ số.
/// (Cố ý KHÔNG bắt buộc chữ hoa / ký tự đặc biệt để tránh quy tắc nửa vời.)
/// Validation nằm DUY NHẤT ở đây, không lặp lại ở Controller.
/// </summary>
public static class PasswordPolicy
{
    public const int MinimumLength = 8;
    public const int MaximumLength = 128;

    /// <summary>Trả về danh sách lỗi. Rỗng = hợp lệ.</summary>
    public static IReadOnlyList<string> Validate(string? password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return ["Mật khẩu là bắt buộc."];
        }

        var errors = new List<string>();

        if (password.Length < MinimumLength)
        {
            errors.Add($"Mật khẩu phải có ít nhất {MinimumLength} ký tự.");
        }

        if (password.Length > MaximumLength)
        {
            errors.Add($"Mật khẩu tối đa {MaximumLength} ký tự.");
        }

        if (!password.Any(char.IsLetter))
        {
            errors.Add("Mật khẩu phải có ít nhất 1 chữ cái.");
        }

        if (!password.Any(char.IsDigit))
        {
            errors.Add("Mật khẩu phải có ít nhất 1 chữ số.");
        }

        return errors;
    }
}
