namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Trạng thái thông báo (<c>ThongBao.TrangThai</c>).
/// <para>
/// Khớp DEFAULT của database (<c>N'Đã gửi'</c>). Cột KHÔNG có CHECK constraint
/// nên các hằng số dưới đây là hàng rào duy nhất.
/// </para>
/// <para>
/// ⚠️ Bảng <c>ThongBao</c> KHÔNG có cột theo dõi đã đọc/chưa đọc
/// (không có <c>DaDoc</c>, không có bảng người nhận riêng).
/// </para>
/// </summary>
public static class NotificationStatuses
{
    public const string DaGui = "Đã gửi";

    public const string DaThuHoi = "Đã thu hồi";

    public static readonly IReadOnlyList<string> All = [DaGui, DaThuHoi];

    public static bool IsValid(string? status)
        => !string.IsNullOrWhiteSpace(status)
           && All.Contains(status.Trim(), StringComparer.Ordinal);

    /// <summary>Chỉ thông báo chưa thu hồi mới hiển thị cho người nhận.</summary>
    public static bool IsVisible(string? status)
        => !string.Equals(status, DaThuHoi, StringComparison.Ordinal);
}
