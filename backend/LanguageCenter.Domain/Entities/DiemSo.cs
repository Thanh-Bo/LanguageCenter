namespace LanguageCenter.Domain.Entities;

/// <summary>DiemSo — Điểm số của học viên theo đầu điểm.</summary>
public class DiemSo
{
    public int MaDiemSo { get; set; }

    public int MaDangKy { get; set; }

    public int MaLoaiDiem { get; set; }

    public string MaGiaoVien { get; set; } = null!;

    public decimal Diem { get; set; }

    public string? NhanXet { get; set; }

    public string? TrangThaiKetQua { get; set; }

    public DateTime NgayCham { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    // References
    public DangKyHoc DangKyHoc { get; set; } = null!;

    public LoaiDiem LoaiDiem { get; set; } = null!;

    public GiaoVien GiaoVien { get; set; } = null!;
}
