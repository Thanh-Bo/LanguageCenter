namespace LanguageCenter.Domain.Entities;

/// <summary>LoaiDiem — Đầu điểm của một khóa học.</summary>
public class LoaiDiem
{
    public int MaLoaiDiem { get; set; }

    public int MaKhoaHoc { get; set; }

    public string TenLoaiDiem { get; set; } = null!;

    public decimal TrongSo { get; set; }

    public decimal DiemToiDa { get; set; }

    public int ThuTu { get; set; }

    public string TrangThai { get; set; } = null!;

    // References
    public KhoaHoc KhoaHoc { get; set; } = null!;

    // Collections
    public ICollection<DiemSo> DiemSos { get; set; } = new List<DiemSo>();
}
