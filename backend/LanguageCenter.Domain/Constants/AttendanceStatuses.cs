namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Trạng thái điểm danh (<c>DiemDanh.TrangThaiDiemDanh</c>).
/// <para>PHẢI khớp CHECK constraint <c>CK_DiemDanh_TrangThai</c> — chỉ 4 giá trị.</para>
/// </summary>
public static class AttendanceStatuses
{
    public const string CoMat = "Có mặt";

    public const string Vang = "Vắng";

    public const string VangCoPhep = "Vắng có phép";

    public const string DiTre = "Đi trễ";

    public static readonly IReadOnlyList<string> All = [CoMat, Vang, VangCoPhep, DiTre];

    public static bool IsValid(string? status) => status is not null && All.Contains(status);
}
