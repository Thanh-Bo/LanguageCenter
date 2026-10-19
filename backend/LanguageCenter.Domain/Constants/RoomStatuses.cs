namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Trạng thái phòng học (<c>PhongHoc.TrangThai</c>).
/// <para>
/// Cột này KHÔNG có CHECK constraint trong database nên đây là lớp bảo vệ duy nhất.
/// </para>
/// </summary>
public static class RoomStatuses
{
    public const string HoatDong = "Hoạt động";

    public const string DangSuDung = "Đang sử dụng";

    public const string BaoTri = "Bảo trì";

    public const string NgungSuDung = "Ngừng sử dụng";

    public static readonly IReadOnlyList<string> All =
        [HoatDong, DangSuDung, BaoTri, NgungSuDung];

    public static bool IsValid(string? status)
        => status is not null && All.Contains(status);
}
