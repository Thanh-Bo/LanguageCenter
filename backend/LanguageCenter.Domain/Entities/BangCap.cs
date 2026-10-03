namespace LanguageCenter.Domain.Entities;

/// <summary>BangCap — Bằng cấp của giáo viên.</summary>
public class BangCap
{
    public int MaBangCap { get; set; }

    public string MaGiaoVien { get; set; } = null!;

    public string TenBangCap { get; set; } = null!;

    public string? ChuyenNganh { get; set; }

    public string? CoSoDaoTao { get; set; }

    public int? NamTotNghiep { get; set; }

    public string? SoBang { get; set; }

    public DateOnly? NgayCap { get; set; }

    public string? TepMinhChung { get; set; }

    // References
    public GiaoVien GiaoVien { get; set; } = null!;
}
