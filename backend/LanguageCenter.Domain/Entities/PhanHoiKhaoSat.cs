namespace LanguageCenter.Domain.Entities;

/// <summary>PhanHoiKhaoSat — Một lượt trả lời khảo sát của học viên.</summary>
public class PhanHoiKhaoSat
{
    public int MaPhanHoiKhaoSat { get; set; }

    public int MaKhaoSat { get; set; }

    public string MaHocVien { get; set; } = null!;

    public DateTime ThoiGianGui { get; set; }

    public decimal? DiemDanhGiaChung { get; set; }

    public string? GhiChu { get; set; }

    // References
    public KhaoSat KhaoSat { get; set; } = null!;

    public HocVien HocVien { get; set; } = null!;

    // Collections
    public ICollection<CauTraLoiKhaoSat> CauTraLoiKhaoSats { get; set; } = new List<CauTraLoiKhaoSat>();
}
