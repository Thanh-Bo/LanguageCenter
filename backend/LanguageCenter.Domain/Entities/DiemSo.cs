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

    /// <summary>Lần kiểm tra của cùng một đầu điểm (1, 2, 3...). UNIQUE theo (đăng ký, đầu điểm, lần).</summary>
    public int LanKiemTra { get; set; } = 1;

    /// <summary>Buổi thi gắn với bài kiểm tra này (NULL nếu chấm ngoài lịch).</summary>
    public int? MaCaHoc { get; set; }

    public DateTime NgayCham { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    // References
    public DangKyHoc DangKyHoc { get; set; } = null!;

    public LoaiDiem LoaiDiem { get; set; } = null!;

    public GiaoVien GiaoVien { get; set; } = null!;

    public CaHoc? CaHoc { get; set; }
}
