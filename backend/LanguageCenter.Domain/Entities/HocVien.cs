namespace LanguageCenter.Domain.Entities;

/// <summary>HocVien — Học viên. MaHocVien đồng thời là tên đăng nhập.</summary>
public class HocVien
{
    public string MaHocVien { get; set; } = null!;

    public int? MaPhuHuynh { get; set; }

    public string HoTen { get; set; } = null!;

    public DateOnly NgaySinh { get; set; }

    public string? GioiTinh { get; set; }

    public string? SoDienThoai { get; set; }

    public string? Email { get; set; }

    public string? DiaChi { get; set; }

    public string? MoiQuanHePhuHuynh { get; set; }

    public string? MucTieuHocTap { get; set; }

    public string? TrinhDoHienTai { get; set; }

    public string TrangThaiHocTap { get; set; } = null!;

    public DateTime NgayTaoHoSo { get; set; }

    public DateOnly? NgayNhapHoc { get; set; }

    // References
    public TaiKhoan TaiKhoan { get; set; } = null!;

    public PhuHuynh? PhuHuynh { get; set; }

    // Collections
    public ICollection<KiemTraDauVao> KiemTraDauVaos { get; set; } = new List<KiemTraDauVao>();

    public ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();

    public ICollection<DiemDanh> DiemDanhs { get; set; } = new List<DiemDanh>();

    public ICollection<PhanHoiDichVu> PhanHoiDichVus { get; set; } = new List<PhanHoiDichVu>();

    public ICollection<PhanHoiKhaoSat> PhanHoiKhaoSats { get; set; } = new List<PhanHoiKhaoSat>();
}
