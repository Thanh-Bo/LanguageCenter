namespace LanguageCenter.Domain.Entities;

/// <summary>CapDo — Cấp độ trong chương trình. ThoiLuongDuKienTuan tính bằng tuần.</summary>
public class CapDo
{
    public int MaCapDo { get; set; }

    public int MaChuongTrinh { get; set; }

    public string? KyHieuCapDo { get; set; }

    public string TenCapDo { get; set; } = null!;

    public int ThuTu { get; set; }

    public string? DieuKienDauVao { get; set; }

    public string? KetQuaDauRa { get; set; }

    public int? ThoiLuongDuKienTuan { get; set; }

    public string TrangThai { get; set; } = null!;

    // References
    public ChuongTrinh ChuongTrinh { get; set; } = null!;

    // Collections
    public ICollection<KhoaHoc> KhoaHocs { get; set; } = new List<KhoaHoc>();

    public ICollection<KiemTraDauVao> KiemTraDauVaos { get; set; } = new List<KiemTraDauVao>();
}
