namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Phương thức thanh toán (<c>ThanhToan.PhuongThucThanhToan</c>).
/// <para>
/// Database KHÔNG có CHECK cho cột này (NVARCHAR(100)), nên service chỉ bắt buộc
/// có giá trị và độ dài hợp lệ — KHÔNG whitelist cứng, để tránh chặn các phương thức
/// hợp lệ khác được bổ sung sau này. Hai hằng số dưới đây là giá trị chuẩn đang dùng.
/// </para>
/// </summary>
public static class PaymentMethods
{
    public const string TienMat = "Tiền mặt";
    public const string ChuyenKhoan = "Chuyển khoản";

    /// <summary>Giá trị chuẩn đang dùng trong dữ liệu — dùng cho tài liệu và thống kê.</summary>
    public static readonly IReadOnlyList<string> Standard =
    [
        TienMat,
        ChuyenKhoan
    ];
}
