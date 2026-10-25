namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Trạng thái phân công giảng dạy (<c>PhanCongGiangDay.TrangThai</c>).
/// <para>Khớp DEFAULT của database (<c>N'Đang phân công'</c>). Không có CHECK constraint.</para>
/// </summary>
public static class AssignmentStatuses
{
    public const string DangPhanCong = "Đang phân công";

    /// <summary>Khung phân công đã tạo nhưng CHƯA gắn giáo viên / trợ giảng.</summary>
    public const string ChoPhanCong = "Chờ phân công";

    public const string DaKetThuc = "Đã kết thúc";

    public const string DaHuy = "Đã hủy";

    public static readonly IReadOnlyList<string> All =
        [ChoPhanCong, DangPhanCong, DaKetThuc, DaHuy];

    /// <summary>Phân công còn hiệu lực — dùng để chặn trùng lặp.</summary>
    public static readonly IReadOnlyList<string> Active = [DangPhanCong, ChoPhanCong];

    public static bool IsValid(string? status)
        => status is not null && All.Contains(status);

    public static bool IsActive(string? status)
        => status is not null && Active.Contains(status);
}
