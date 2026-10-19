namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Trạng thái ca học thực tế (<c>CaHoc.TrangThai</c>).
/// <para>
/// Khớp DEFAULT của database (<c>N'Chưa diễn ra'</c>). Cột này KHÔNG có CHECK constraint
/// nên đây là lớp bảo vệ DUY NHẤT chống giá trị rác từ client.
/// </para>
/// </summary>
public static class SessionStatuses
{
    public const string ChuaDienRa = "Chưa diễn ra";

    public const string DangDienRa = "Đang diễn ra";

    public const string DaHoanThanh = "Đã hoàn thành";

    public const string DaHuy = "Đã hủy";

    public static readonly IReadOnlyList<string> All =
        [ChuaDienRa, DangDienRa, DaHoanThanh, DaHuy];

    /// <summary>Ca ở trạng thái này mới cho phép điểm danh.</summary>
    public static readonly IReadOnlyList<string> AttendanceAllowed = [ChuaDienRa, DangDienRa, DaHoanThanh];

    public static bool IsValid(string? status) => status is not null && All.Contains(status);

    public static bool CanTakeAttendance(string? status)
        => status is not null && AttendanceAllowed.Contains(status);
}
