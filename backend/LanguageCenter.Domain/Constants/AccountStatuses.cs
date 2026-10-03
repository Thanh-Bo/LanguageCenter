namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Trạng thái tài khoản — giá trị PHẢI khớp CHECK CONSTRAINT <c>CK_TaiKhoan_TrangThai</c> trong database.
/// Chỉ <see cref="HoatDong"/> được phép đăng nhập.
/// </summary>
public static class AccountStatuses
{
    public const string HoatDong = "Hoạt động";
    public const string BiKhoa = "Bị khóa";
    public const string NgungSuDung = "Ngừng sử dụng";

    /// <summary>Toàn bộ giá trị hợp lệ — dùng để validate input từ client.</summary>
    public static readonly IReadOnlyList<string> All = [HoatDong, BiKhoa, NgungSuDung];

    public static bool IsValid(string? status)
        => status is not null && All.Contains(status);
}
