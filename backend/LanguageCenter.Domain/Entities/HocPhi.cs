namespace LanguageCenter.Domain.Entities;

/// <summary>HocPhi — Học phí của một đăng ký học (quan hệ 1-1 với DangKyHoc).</summary>
public class HocPhi
{
    public int MaHocPhi { get; set; }

    public int MaDangKy { get; set; }

    public decimal SoTienGoc { get; set; }

    public decimal SoTienGiam { get; set; }

    /// <summary>Cột computed PERSISTED trong database — chỉ đọc.</summary>
    public decimal SoTienPhaiDong { get; private set; }

    public decimal SoTienDaDong { get; set; }

    /// <summary>Cột computed PERSISTED trong database — chỉ đọc.</summary>
    public decimal SoTienConThieu { get; private set; }

    public DateOnly? HanThanhToan { get; set; }

    public string TrangThaiHocPhi { get; set; } = null!;

    public DateTime NgayTao { get; set; }

    // References
    public DangKyHoc DangKyHoc { get; set; } = null!;

    // Collections
    public ICollection<ThanhToan> ThanhToans { get; set; } = new List<ThanhToan>();
}
