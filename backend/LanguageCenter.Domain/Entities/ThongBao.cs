namespace LanguageCenter.Domain.Entities;

/// <summary>ThongBao — Thông báo gửi cho một tài khoản hoặc cho cả lớp.</summary>
public class ThongBao
{
    public int MaThongBao { get; set; }

    public int MaTaiKhoanGui { get; set; }

    /// <summary>Gửi trực tiếp cho một tài khoản.</summary>
    public int? MaTaiKhoanNhan { get; set; }

    /// <summary>Hoặc thông báo cho cả lớp.</summary>
    public int? MaLopHoc { get; set; }

    public string TieuDe { get; set; } = null!;

    public string NoiDung { get; set; } = null!;

    public string? LoaiThongBao { get; set; }

    public DateTime ThoiGianGui { get; set; }

    public string TrangThai { get; set; } = null!;

    // References
    public TaiKhoan TaiKhoanGui { get; set; } = null!;

    public TaiKhoan? TaiKhoanNhan { get; set; }

    public LopHoc? LopHoc { get; set; }
}
