namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Loại thông báo (<c>ThongBao.LoaiThongBao</c>).
/// <para>
/// Cột là <c>NVARCHAR(100) NULL</c> và KHÔNG có CHECK constraint, nên hệ thống
/// KHÔNG whitelist cứng — chỉ kiểm tra độ dài. Các hằng số dưới đây là giá trị
/// chuẩn đang dùng trong dữ liệu thật.
/// </para>
/// </summary>
public static class NotificationTypes
{
    public const string LichHoc = "Lịch học";

    public const string HocTap = "Học tập";

    public const string HocPhi = "Học phí";

    public const string Khac = "Khác";

    /// <summary>Giá trị chuẩn — chỉ dùng cho tài liệu và gợi ý, KHÔNG dùng để chặn.</summary>
    public static readonly IReadOnlyList<string> Standard = [LichHoc, HocTap, HocPhi, Khac];
}
