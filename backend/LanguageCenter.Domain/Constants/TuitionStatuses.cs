namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Trạng thái nghĩa vụ học phí (<c>HocPhi.TrangThaiHocPhi</c>).
/// <para>
/// Cột này KHÔNG có CHECK constraint trong database → các hằng số dưới đây là hàng rào duy nhất.
/// Giá trị phải khớp CHÍNH XÁC dữ liệu đã có trong bảng.
/// </para>
/// <para>
/// "Quá hạn" KHÔNG được lưu vào cột — nó là trạng thái SUY RA từ
/// <c>SoTienConThieu &gt; 0</c> và <c>HanThanhToan &lt; hôm nay</c>.
/// </para>
/// </summary>
public static class TuitionStatuses
{
    public const string ChuaThanhToan = "Chưa thanh toán";
    public const string ThanhToanMotPhan = "Thanh toán một phần";
    public const string DaThanhToan = "Đã thanh toán";

    public static readonly IReadOnlyList<string> All =
    [
        ChuaThanhToan,
        ThanhToanMotPhan,
        DaThanhToan
    ];

    public static bool IsValid(string? value)
        => !string.IsNullOrWhiteSpace(value)
           && All.Contains(value.Trim(), StringComparer.Ordinal);

    /// <summary>
    /// Suy ra trạng thái từ hai mốc tiền — nguồn duy nhất để đồng bộ cột sau mỗi giao dịch.
    /// </summary>
    public static string FromAmounts(decimal soTienPhaiDong, decimal soTienDaDong)
    {
        // Học phí 0 đồng (giảm 100%) coi như đã hoàn tất nghĩa vụ.
        if (soTienPhaiDong <= 0m)
        {
            return DaThanhToan;
        }

        if (soTienDaDong <= 0m)
        {
            return ChuaThanhToan;
        }

        return soTienDaDong >= soTienPhaiDong ? DaThanhToan : ThanhToanMotPhan;
    }
}
