namespace LanguageCenter.Domain.Entities;

/// <summary>NhanVien — Nhân viên. MaNhanVien đồng thời là tên đăng nhập.</summary>
public class NhanVien
{
    public string MaNhanVien { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    public string? GioiTinh { get; set; }

    public string? SoDienThoai { get; set; }

    public string? Email { get; set; }

    public string? DiaChi { get; set; }

    public string ChucVu { get; set; } = null!;

    public DateOnly? NgayVaoLam { get; set; }

    public string TrangThaiLamViec { get; set; } = null!;

    // References
    public TaiKhoan TaiKhoan { get; set; } = null!;

    // Collections
    public ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();

    public ICollection<DangKyHoc> DangKyHocTuVans { get; set; } = new List<DangKyHoc>();

    public ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();

    public ICollection<BienNhan> BienNhans { get; set; } = new List<BienNhan>();
}
