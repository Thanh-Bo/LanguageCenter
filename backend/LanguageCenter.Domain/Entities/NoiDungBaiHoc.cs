namespace LanguageCenter.Domain.Entities;

/// <summary>
/// NoiDungBaiHoc — Nội dung của từng buổi học trong một khóa (1..20).
/// <para>
/// Mỗi khóa có đúng 20 dòng (UNIQUE theo <c>MaKhoaHoc + SoBuoi</c>).
/// Khi sinh ca học, buổi thứ i sẽ lấy nội dung có <c>SoBuoi = i</c>.
/// </para>
/// </summary>
public class NoiDungBaiHoc
{
    public int MaNoiDung { get; set; }

    public int MaKhoaHoc { get; set; }

    /// <summary>Buổi thứ mấy trong khóa (1 → 20).</summary>
    public int SoBuoi { get; set; }

    public string TieuDe { get; set; } = null!;

    public string? NoiDung { get; set; }

    public string? MucTieu { get; set; }

    // References
    public KhoaHoc KhoaHoc { get; set; } = null!;
}
