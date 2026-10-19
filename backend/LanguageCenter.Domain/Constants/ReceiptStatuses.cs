namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Trạng thái biên nhận (<c>BienNhan.TrangThai</c>).
/// <para>KHÔNG có CHECK constraint trong database.</para>
/// </summary>
public static class ReceiptStatuses
{
    public const string DaPhatHanh = "Đã phát hành";
    public const string DaHuy = "Đã hủy";

    public static readonly IReadOnlyList<string> All =
    [
        DaPhatHanh,
        DaHuy
    ];

    public static bool IsValid(string? value)
        => !string.IsNullOrWhiteSpace(value)
           && All.Contains(value.Trim(), StringComparer.Ordinal);
}
