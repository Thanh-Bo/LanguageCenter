namespace LanguageCenter.Domain.Entities;

/// <summary>
/// PhanCongGiangDay — Lịch sử phân công giáo viên / trợ giảng cho lớp.
/// <para>
/// <c>MaGiaoVien</c> được phép NULL: khi đó dòng này là "khung" phân công
/// (trạng thái <c>Chờ phân công</c>) — tạo trước rồi gắn người sau.
/// </para>
/// </summary>
public class PhanCongGiangDay
{
    public int MaPhanCong { get; set; }

    public int MaLopHoc { get; set; }

    public string? MaGiaoVien { get; set; }

    public string VaiTroPhanCong { get; set; } = null!;

    public DateOnly NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    public string TrangThai { get; set; } = null!;

    public string? GhiChu { get; set; }

    // References
    public LopHoc LopHoc { get; set; } = null!;

    public GiaoVien? GiaoVien { get; set; }
}
