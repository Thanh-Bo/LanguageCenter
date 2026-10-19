namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Trạng thái đợt khảo sát (<c>KhaoSat.TrangThai</c>).
/// <para>
/// Khớp DEFAULT của database (<c>N'Hoạt động'</c>) — giá trị đang có trong dữ liệu thật.
/// Cột KHÔNG có CHECK constraint.
/// </para>
/// <para>
/// Việc "đang nhận phản hồi" phụ thuộc ĐỒNG THỜI vào trạng thái VÀ khoảng ngày,
/// nên phải dùng <see cref="IsAcceptingResponses"/> thay vì so sánh chuỗi.
/// </para>
/// </summary>
public static class SurveyStatuses
{
    /// <summary>Đang mở, nhận phản hồi (nếu cả khoảng ngày cho phép).</summary>
    public const string HoatDong = "Hoạt động";

    /// <summary>Tạm dừng — không nhận phản hồi.</summary>
    public const string TamDung = "Tạm dừng";

    /// <summary>Đã kết thúc — không nhận phản hồi.</summary>
    public const string DaKetThuc = "Đã kết thúc";

    public static readonly IReadOnlyList<string> All = [HoatDong, TamDung, DaKetThuc];

    public static bool IsValid(string? status)
        => !string.IsNullOrWhiteSpace(status)
           && All.Contains(status.Trim(), StringComparer.Ordinal);

    /// <summary>Khảo sát có đang nhận phản hồi tại ngày <paramref name="today"/> hay không.</summary>
    public static bool IsAcceptingResponses(
        string? trangThai,
        DateOnly ngayBatDau,
        DateOnly? ngayKetThuc,
        DateOnly today)
        => DescribeBlockReason(trangThai, ngayBatDau, ngayKetThuc, today) is null;

    /// <summary>
    /// Trả về lý do KHÔNG nhận phản hồi, hoặc <c>null</c> nếu đang nhận.
    /// </summary>
    public static string? DescribeBlockReason(
        string? trangThai,
        DateOnly ngayBatDau,
        DateOnly? ngayKetThuc,
        DateOnly today)
    {
        if (!string.Equals(trangThai, HoatDong, StringComparison.Ordinal))
        {
            return $"Khảo sát đang ở trạng thái '{trangThai}', không nhận phản hồi.";
        }

        if (today < ngayBatDau)
        {
            return $"Khảo sát chưa bắt đầu (ngày bắt đầu {ngayBatDau:dd/MM/yyyy}).";
        }

        if (ngayKetThuc is { } ngayDen && today > ngayDen)
        {
            return $"Khảo sát đã hết hạn (ngày kết thúc {ngayDen:dd/MM/yyyy}).";
        }

        return null;
    }
}
