namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Trạng thái giao dịch thanh toán (<c>ThanhToan.TrangThai</c>).
/// <para>KHÔNG có CHECK constraint trong database.</para>
/// <para>
/// Phase 6 ghi nhận thanh toán theo MỘT bước ⇒ giá trị mặc định là <see cref="ThanhCong"/>.
/// <see cref="DaHuy"/> được khai báo sẵn cho tương lai nhưng Phase 6 CHƯA có endpoint hủy.
/// </para>
/// </summary>
public static class PaymentStatuses
{
    public const string ThanhCong = "Thành công";
    public const string DaHuy = "Đã hủy";

    public static readonly IReadOnlyList<string> All =
    [
        ThanhCong,
        DaHuy
    ];

    /// <summary>Chỉ giao dịch "Thành công" mới được tính vào số tiền đã thu.</summary>
    public const string Effective = ThanhCong;

    public static bool IsValid(string? value)
        => !string.IsNullOrWhiteSpace(value)
           && All.Contains(value.Trim(), StringComparer.Ordinal);
}
