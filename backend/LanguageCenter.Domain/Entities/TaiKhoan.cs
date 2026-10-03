namespace LanguageCenter.Domain.Entities;

/// <summary>TaiKhoan — Tài khoản đăng nhập. TenDangNhap chính là mã nghiệp vụ (NV001, GV001, HV001...).</summary>
public class TaiKhoan
{
    public int MaTaiKhoan { get; set; }

    public int MaVaiTro { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhauHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string TrangThaiTaiKhoan { get; set; } = null!;

    public DateTime? LanDangNhapCuoi { get; set; }

    public DateTime NgayTao { get; set; }

    public DateTime? NgayCapNhat { get; set; }

    // References
    public VaiTro VaiTro { get; set; } = null!;

    public NhanVien? NhanVien { get; set; }

    public GiaoVien? GiaoVien { get; set; }

    public HocVien? HocVien { get; set; }

    // Collections
    public ICollection<DiemDanh> DiemDanhs { get; set; } = new List<DiemDanh>();

    public ICollection<ThongBao> ThongBaoGui { get; set; } = new List<ThongBao>();

    public ICollection<ThongBao> ThongBaoNhan { get; set; } = new List<ThongBao>();
}
