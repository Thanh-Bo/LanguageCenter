namespace LanguageCenter.Domain.Entities;

/// <summary>LopHoc — Lớp học mở theo một khóa học.</summary>
public class LopHoc
{
    public int MaLopHoc { get; set; }

    public string MaLop { get; set; } = null!;

    public string TenLop { get; set; } = null!;

    public int MaKhoaHoc { get; set; }

    public string MaNhanVienHocVu { get; set; } = null!;

    public DateOnly NgayBatDau { get; set; }

    public DateOnly NgayKetThucDuKien { get; set; }

    public DateOnly? NgayKetThucThucTe { get; set; }

    /// <summary>Giờ bắt đầu buổi học mặc định của lớp (NULL = chưa xác định).</summary>
    public TimeOnly? GioBatDau { get; set; }

    /// <summary>Giờ kết thúc buổi học mặc định của lớp (NULL = chưa xác định).</summary>
    public TimeOnly? GioKetThuc { get; set; }

    /// <summary>Thứ của buổi 1 trong tuần (1 = Thứ Hai … 7 = Chủ Nhật).</summary>
    public byte? ThuBuoi1 { get; set; }

    /// <summary>Thứ của buổi 2 trong tuần (1 = Thứ Hai … 7 = Chủ Nhật).</summary>
    public byte? ThuBuoi2 { get; set; }

    public int SiSoToiDa { get; set; }

    public string TrangThai { get; set; } = null!;

    // References
    public KhoaHoc KhoaHoc { get; set; } = null!;

    public NhanVien NhanVienHocVu { get; set; } = null!;

    // Collections
    public ICollection<PhanCongGiangDay> PhanCongGiangDays { get; set; } = new List<PhanCongGiangDay>();

    public ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();

    public ICollection<LichHoc> LichHocs { get; set; } = new List<LichHoc>();

    public ICollection<CaHoc> CaHocs { get; set; } = new List<CaHoc>();

    public ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();
}
