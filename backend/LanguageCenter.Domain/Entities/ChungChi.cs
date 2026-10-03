namespace LanguageCenter.Domain.Entities;

/// <summary>ChungChi — Chứng chỉ của giáo viên.</summary>
public class ChungChi
{
    public int MaChungChi { get; set; }

    public string MaGiaoVien { get; set; } = null!;

    public string TenChungChi { get; set; } = null!;

    public string? LoaiChungChi { get; set; }

    public string? DonViCap { get; set; }

    public string? KetQua { get; set; }

    public string? SoChungChi { get; set; }

    public DateOnly? NgayCap { get; set; }

    public DateOnly? NgayHetHan { get; set; }

    public string? TepMinhChung { get; set; }

    // References
    public GiaoVien GiaoVien { get; set; } = null!;
}
