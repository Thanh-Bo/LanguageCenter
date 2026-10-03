namespace LanguageCenter.Domain.Entities;

/// <summary>BienNhan — Biên nhận in cho một giao dịch thanh toán.</summary>
public class BienNhan
{
    public int MaBienNhan { get; set; }

    public int MaThanhToan { get; set; }

    public string MaNhanVienLap { get; set; } = null!;

    public string SoBienNhan { get; set; } = null!;

    public DateTime NgayLap { get; set; }

    public decimal SoTienXacNhan { get; set; }

    public string? NoiDung { get; set; }

    public string TrangThai { get; set; } = null!;

    // References
    public ThanhToan ThanhToan { get; set; } = null!;

    public NhanVien NhanVienLap { get; set; } = null!;
}
