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

    /// <summary>Buổi kiểm tra giữa kỳ / bài kiểm tra nhỏ — vẫn được gắn điểm.</summary>
    public const string KiemTra = "Kiểm tra";

    public const string BuoiThi = "Buổi thi";

    public static readonly IReadOnlyList<string> All = [BuoiHoc, KiemTra, BuoiThi];

    public static bool IsValid(string? loaiBuoi) => loaiBuoi is not null && All.Contains(loaiBuoi);

    /// <summary>
    /// Buổi có ĐÁNH GIÁ (kiểm tra hoặc thi) — dùng để cho phép gắn điểm số vào ca học.
    /// </summary>
    public static bool IsExam(string? loaiBuoi) => loaiBuoi is KiemTra or BuoiThi;

    /// <summary>Chỉ buổi THI cuối kỳ (khác với kiểm tra thường xuyên).</summary>
    public static bool IsFinalExam(string? loaiBuoi) => loaiBuoi == BuoiThi;
}
