namespace LanguageCenter.Domain.Entities;

/// <summary>DangKyHoc — Học viên đăng ký khóa học.</summary>
public class DangKyHoc
{
    public int MaDangKy { get; set; }

    public string MaHocVien { get; set; } = null!;

    public int MaKhoaHoc { get; set; }

    /// <summary>Chưa xếp lớp thì để NULL.</summary>
    public int? MaLopHoc { get; set; }

    public string MaNhanVienTuVan { get; set; } = null!;

    public int? MaUuDai { get; set; }

    public DateTime NgayDangKy { get; set; }

    public string? LichHocMongMuon { get; set; }

    public decimal HocPhiGoc { get; set; }

    public decimal SoTienGiam { get; set; }

    /// <summary>Cột computed PERSISTED trong database — chỉ đọc.</summary>
    public decimal HocPhiSauGiam { get; private set; }

    public DateTime? NgayXepLop { get; set; }

    public string TrangThaiDangKy { get; set; } = null!;

    public string? GhiChu { get; set; }

    // References
    public HocVien HocVien { get; set; } = null!;

    public KhoaHoc KhoaHoc { get; set; } = null!;

    public LopHoc? LopHoc { get; set; }

    public NhanVien NhanVienTuVan { get; set; } = null!;

    public UuDai? UuDai { get; set; }

    // Collections
    public ICollection<DiemSo> DiemSos { get; set; } = new List<DiemSo>();

    public HocPhi? HocPhi { get; set; }
}
