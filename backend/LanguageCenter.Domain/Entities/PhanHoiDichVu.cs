namespace LanguageCenter.Domain.Entities;

/// <summary>PhanHoiDichVu — Phản hồi / đánh giá dịch vụ của học viên.</summary>
public class PhanHoiDichVu
{
    public int MaPhanHoiDichVu { get; set; }

    public int MaDichVu { get; set; }

    public string MaHocVien { get; set; } = null!;

    public int DiemDanhGia { get; set; }

    public string? NoiDungPhanHoi { get; set; }

    public DateTime ThoiGianPhanHoi { get; set; }

    public string TrangThai { get; set; } = null!;

    // References
    public DichVu DichVu { get; set; } = null!;

    public HocVien HocVien { get; set; } = null!;
}
