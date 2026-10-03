namespace LanguageCenter.Domain.Entities;

/// <summary>KiemTraDauVao — Kết quả kiểm tra đầu vào của học viên.</summary>
public class KiemTraDauVao
{
    public int MaKiemTra { get; set; }

    public string MaHocVien { get; set; } = null!;

    public int? MaCapDoDanhGia { get; set; }

    public DateTime NgayKiemTra { get; set; }

    public decimal? DiemNghe { get; set; }

    public decimal? DiemNoi { get; set; }

    public decimal? DiemDoc { get; set; }

    public decimal? DiemViet { get; set; }

    public decimal? TongDiem { get; set; }

    public string? NhanXet { get; set; }

    // References
    public HocVien HocVien { get; set; } = null!;

    public CapDo? CapDoDanhGia { get; set; }
}
