namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Trạng thái thành phần đánh giá (<c>LoaiDiem.TrangThai</c>).
/// <para>Khớp DEFAULT của database (<c>N'Hoạt động'</c>). Không có CHECK constraint.</para>
/// </summary>
public static class GradeTypeStatuses
{
    public const string HoatDong = "Hoạt động";

    public const string NgungSuDung = "Ngừng sử dụng";

    public static readonly IReadOnlyList<string> All = [HoatDong, NgungSuDung];

    public static bool IsValid(string? status) => status is not null && All.Contains(status);

    public static bool IsActive(string? status) => status == HoatDong;
}
