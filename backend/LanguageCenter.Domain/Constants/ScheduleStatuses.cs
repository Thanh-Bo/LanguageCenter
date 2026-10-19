namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Trạng thái lịch học định kỳ (<c>LichHoc.TrangThai</c>).
/// <para>Khớp DEFAULT của database (<c>N'Hoạt động'</c>). Không có CHECK constraint.</para>
/// </summary>
public static class ScheduleStatuses
{
    public const string HoatDong = "Hoạt động";

    public const string NgungApDung = "Ngừng áp dụng";

    public static readonly IReadOnlyList<string> All = [HoatDong, NgungApDung];

    public static bool IsValid(string? status) => status is not null && All.Contains(status);

    /// <summary>Trạng thái dùng khi so trùng lịch (bỏ qua lịch đã ngừng áp dụng).</summary>
    public static bool IsActive(string? status) => status == HoatDong;
}
