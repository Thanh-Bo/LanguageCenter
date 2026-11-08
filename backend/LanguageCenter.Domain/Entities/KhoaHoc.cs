namespace LanguageCenter.Domain.Entities;

/// <summary>
/// KhoaHoc — Khóa học thuộc một cấp độ.
/// <para>
/// Đây là KHUÔN MẪU dùng lại cho nhiều lớp (số buổi, tổng giờ, học phí, nội dung).
/// <b>KHÔNG có ngày bắt đầu / kết thúc</b>: ngày khai giảng và ngày kết thúc thuộc về
/// từng LỚP cụ thể (<see cref="LopHoc.NgayBatDau"/> / <see cref="LopHoc.NgayKetThucDuKien"/>).
/// </para>
/// </summary>
public class KhoaHoc
{
    public int MaKhoaHoc { get; set; }

    public int MaCapDo { get; set; }

    public string TenKhoaHoc { get; set; } = null!;

    public string? NoiDung { get; set; }

    public int SoBuoi { get; set; }

    public decimal? TongSoGio { get; set; }

    public decimal HocPhiCoBan { get; set; }

    public string TrangThai { get; set; } = null!;

    // References
    public CapDo CapDo { get; set; } = null!;

    // Collections
    public ICollection<LopHoc> LopHocs { get; set; } = new List<LopHoc>();

    public ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();

    public ICollection<LoaiDiem> LoaiDiems { get; set; } = new List<LoaiDiem>();
}
