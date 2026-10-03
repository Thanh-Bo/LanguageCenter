namespace LanguageCenter.Domain.Entities;

/// <summary>KhoaHoc — Khóa học thuộc một cấp độ.</summary>
public class KhoaHoc
{
    public int MaKhoaHoc { get; set; }

    public int MaCapDo { get; set; }

    public string TenKhoaHoc { get; set; } = null!;

    public string? NoiDung { get; set; }

    public int SoBuoi { get; set; }

    public decimal? TongSoGio { get; set; }

    public decimal HocPhiCoBan { get; set; }

    public DateOnly NgayBatDau { get; set; }

    public DateOnly NgayKetThuc { get; set; }

    public string TrangThai { get; set; } = null!;

    // References
    public CapDo CapDo { get; set; } = null!;

    // Collections
    public ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();

    public ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();

    public ICollection<LoaiDiem> LoaiDiems { get; set; } = new List<LoaiDiem>();
}
