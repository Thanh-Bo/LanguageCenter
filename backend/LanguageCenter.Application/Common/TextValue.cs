namespace LanguageCenter.Application.Common;

/// <summary>Chuẩn hoá chuỗi nhập từ client: cắt khoảng trắng, chuỗi rỗng/trắng -> null.</summary>
public static class TextValue
{
    /// <summary>Cắt khoảng trắng. Trả <c>null</c> nếu chuỗi null/rỗng/toàn khoảng trắng.</summary>
    public static string? Optional(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    /// <summary>Cắt khoảng trắng. Trả <c>null</c> nếu rỗng — dùng cho field bắt buộc để service tự báo lỗi.</summary>
    public static string? Required(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    /// <summary>Ngày sinh hợp lệ: không ở tương lai, không quá cũ (>= năm 1900).</summary>
    public static bool IsPlausibleBirthDate(DateOnly ngaySinh, out string? error)
    {
        if (ngaySinh == default)
        {
            error = "Ngày sinh là bắt buộc.";
            return false;
        }

        if (ngaySinh > DateOnly.FromDateTime(DateTime.Today))
        {
            error = "Ngày sinh không được ở tương lai.";
            return false;
        }

        if (ngaySinh.Year < 1900)
        {
            error = "Ngày sinh không hợp lệ (trước năm 1900).";
            return false;
        }

        error = null;
        return true;
    }

    /// <summary>Số điện thoại: chỉ chữ số, khoảng trắng, +, -, ., ( ) và dài 8-20 ký tự.</summary>
    public static bool IsPlausiblePhone(string? phone, out string? error)
    {
        error = null;

        if (string.IsNullOrWhiteSpace(phone))
        {
            return true; // không bắt buộc
        }

        var trimmed = phone.Trim();

        if (trimmed.Length is < 8 or > 20)
        {
            error = "Số điện thoại phải từ 8 đến 20 ký tự.";
            return false;
        }

        foreach (var c in trimmed)
        {
            if (!char.IsDigit(c) && c is not (' ' or '+' or '-' or '.' or '(' or ')'))
            {
                error = "Số điện thoại chỉ được chứa chữ số và các ký tự + - . ( )";
                return false;
            }
        }

        return true;
    }
}
