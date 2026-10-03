namespace LanguageCenter.Domain.Entities;

/// <summary>DiemDanh — Điểm danh học viên trong một ca học.</summary>
public class DiemDanh
{
    public int MaDiemDanh { get; set; }

    public int MaCaHoc { get; set; }

    public string MaHocVien { get; set; } = null!;

    public string TrangThaiDiemDanh { get; set; } = null!;

    public DateTime ThoiGianDiemDanh { get; set; }

    public int MaTaiKhoanThucHien { get; set; }

    public string? GhiChu { get; set; }

    // References
    public CaHoc CaHoc { get; set; } = null!;

    public HocVien HocVien { get; set; } = null!;

    public TaiKhoan TaiKhoanThucHien { get; set; } = null!;
}
