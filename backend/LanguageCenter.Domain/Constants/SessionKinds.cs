namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Loại buổi học (<c>CaHoc.LoaiBuoi</c>, <c>NoiDungBaiHoc.LoaiBuoi</c>).
/// <para>
/// PHẢI khớp CHECK constraint <c>CK_CaHoc_LoaiBuoi</c> / <c>CK_NoiDungBaiHoc_LoaiBuoi</c>.
/// Dùng để tô màu tiến độ: buổi học bình thường và buổi thi khác nhau.
/// </para>
/// </summary>
public static class SessionKinds
{
    public const string BuoiHoc = "Buổi học";

    public const string BuoiThi = "Buổi thi";

    public static readonly IReadOnlyList<string> All = [BuoiHoc, BuoiThi];

    public static bool IsValid(string? loaiBuoi) => loaiBuoi is not null && All.Contains(loaiBuoi);

    public static bool IsExam(string? loaiBuoi) => loaiBuoi == BuoiThi;
}
