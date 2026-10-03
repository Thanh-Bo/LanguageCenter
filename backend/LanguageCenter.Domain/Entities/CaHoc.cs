namespace LanguageCenter.Domain.Entities;

/// <summary>CaHoc — Ca học thực tế của lớp.</summary>
public class CaHoc
{
    public int MaCaHoc { get; set; }

    public int MaLopHoc { get; set; }

    /// <summary>Có thể NULL nếu là ca phát sinh ngoài lịch định kỳ.</summary>
    public int? MaLichHoc { get; set; }

    public int MaPhongHoc { get; set; }

    public string? MaGiaoVienThucTe { get; set; }

    public string? MaTroGiangThucTe { get; set; }

    public DateOnly NgayHoc { get; set; }

    public TimeOnly GioBatDau { get; set; }

    public TimeOnly GioKetThuc { get; set; }

    public string? NoiDungBaiHoc { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? LyDoThayDoi { get; set; }

    // References
    public LopHoc LopHoc { get; set; } = null!;

    public LichHoc? LichHoc { get; set; }

    public PhongHoc PhongHoc { get; set; } = null!;

    public GiaoVien? GiaoVienThucTe { get; set; }

    public GiaoVien? TroGiangThucTe { get; set; }

    // Collections
    public ICollection<DiemDanh> DiemDanhs { get; set; } = new List<DiemDanh>();
}
