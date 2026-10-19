namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Kết quả của một điểm số (<c>DiemSo.TrangThaiKetQua</c>).
/// <para>
/// Cột này NULLABLE và KHÔNG có CHECK constraint. Nếu client gửi giá trị thì phải thuộc
/// tập dưới đây; gửi <c>null</c> nghĩa là chưa xác định kết quả.
/// </para>
/// </summary>
public static class GradeResults
{
    public const string Dat = "Đạt";

    public const string KhongDat = "Không đạt";

    public static readonly IReadOnlyList<string> All = [Dat, KhongDat];

    public static bool IsValid(string? ketQua) => ketQua is not null && All.Contains(ketQua);
}
