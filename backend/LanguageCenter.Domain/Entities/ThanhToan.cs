namespace LanguageCenter.Domain.Entities;

/// <summary>ThanhToan — Giao dịch thanh toán học phí.</summary>
public class ThanhToan
{
    public int MaThanhToan { get; set; }

    public int MaHocPhi { get; set; }

    public string MaNhanVienKeToan { get; set; } = null!;

    public decimal SoTien { get; set; }

    public DateTime NgayThanhToan { get; set; }

    public string PhuongThucThanhToan { get; set; } = null!;

    /// <summary>Mã giao dịch ngoài hệ thống — có unique index có điều kiện (bỏ qua NULL).</summary>
    public string? MaGiaoDich { get; set; }

    public string LoaiGiaoDich { get; set; } = null!;

    public string TrangThai { get; set; } = null!;

    public string? GhiChu { get; set; }

    // References
    public HocPhi HocPhi { get; set; } = null!;

    public NhanVien NhanVienKeToan { get; set; } = null!;

    public BienNhan? BienNhan { get; set; }
}
