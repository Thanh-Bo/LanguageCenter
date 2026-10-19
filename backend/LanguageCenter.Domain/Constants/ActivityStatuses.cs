namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Trạng thái hoạt động dùng chung cho các bảng danh mục Phase 4:
/// <c>ChuongTrinh</c>, <c>CapDo</c>, <c>UuDai</c>.
/// <para>
/// Khớp DEFAULT của database (<c>N'Hoạt động'</c>). Các cột này KHÔNG có CHECK constraint
/// nên đây là lớp bảo vệ DUY NHẤT chống giá trị rác từ client.
/// </para>
/// </summary>
public static class ActivityStatuses
{
    public const string HoatDong = "Hoạt động";

    public const string NgungHoatDong = "Ngừng hoạt động";

    public static readonly IReadOnlyList<string> All = [HoatDong, NgungHoatDong];

    public static bool IsValid(string? status)
        => status is not null && All.Contains(status);
}
