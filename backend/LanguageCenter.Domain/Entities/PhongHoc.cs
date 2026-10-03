namespace LanguageCenter.Domain.Entities;

/// <summary>PhongHoc — Phòng học của trung tâm.</summary>
public class PhongHoc
{
    public int MaPhongHoc { get; set; }

    public string MaPhong { get; set; } = null!;

    public string TenPhong { get; set; } = null!;

    public string? ViTri { get; set; }

    public int SucChua { get; set; }

    public string? ThietBi { get; set; }

    public string TrangThai { get; set; } = null!;

    public ICollection<LichHoc> LichHocs { get; set; } = new List<LichHoc>();

    public ICollection<CaHoc> CaHocs { get; set; } = new List<CaHoc>();
}
