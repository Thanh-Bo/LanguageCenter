namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Loại giao dịch (<c>ThanhToan.LoaiGiaoDich</c>) — PHẢI khớp CHECK constraint
/// <c>CK_ThanhToan_Loai</c> trong database.
/// <para>
/// Phase 6 chỉ hỗ trợ <see cref="ThuTien"/>. Hoàn tiền / điều chỉnh cần nghiệp vụ
/// phê duyệt và schema bổ sung nên CHƯA triển khai.
/// </para>
/// </summary>
public static class PaymentTypes
{
    public const string ThuTien = "Thu tiền";
    public const string HoanTien = "Hoàn tiền";
    public const string DieuChinh = "Điều chỉnh";

    /// <summary>Toàn bộ giá trị được CHECK constraint cho phép.</summary>
    public static readonly IReadOnlyList<string> All =
    [
        ThuTien,
        HoanTien,
        DieuChinh
    ];

    /// <summary>Các loại Phase 6 thực sự ghi nhận được.</summary>
    public static readonly IReadOnlyList<string> Supported =
    [
        ThuTien
    ];

    public static bool IsValid(string? value)
        => !string.IsNullOrWhiteSpace(value)
           && All.Contains(value.Trim(), StringComparer.Ordinal);

    public static bool IsSupported(string? value)
        => !string.IsNullOrWhiteSpace(value)
           && Supported.Contains(value.Trim(), StringComparer.Ordinal);
}
