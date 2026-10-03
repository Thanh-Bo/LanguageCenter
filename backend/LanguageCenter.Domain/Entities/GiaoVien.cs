namespace LanguageCenter.Domain.Entities;

/// <summary>GiaoVien — Giáo viên / Trợ giảng. MaGiaoVien đồng thời là tên đăng nhập.</summary>
public class GiaoVien
{
    public string MaGiaoVien { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    public string? GioiTinh { get; set; }

    public string? SoDienThoai { get; set; }

    public string? Email { get; set; }

    public string? DiaChi { get; set; }

    public string LoaiGiaoVien { get; set; } = null!;

    public string? ChuyenMon { get; set; }

    public int? SoNamKinhNghiem { get; set; }

    public DateOnly? NgayVaoLam { get; set; }

    public string TrangThai { get; set; } = null!;

    // References
    public TaiKhoan TaiKhoan { get; set; } = null!;

    // Collections
    public ICollection<BangCap> BangCaps { get; set; } = new List<BangCap>();

    public ICollection<ChungChi> ChungChis { get; set; } = new List<ChungChi>();

    public ICollection<PhanCongGiangDay> PhanCongGiangDays { get; set; } = new List<PhanCongGiangDay>();

    public ICollection<CaHoc> CaHocGiaoVienThucTes { get; set; } = new List<CaHoc>();

    public ICollection<CaHoc> CaHocTroGiangThucTes { get; set; } = new List<CaHoc>();

    public ICollection<DiemSo> DiemSos { get; set; } = new List<DiemSo>();
}
