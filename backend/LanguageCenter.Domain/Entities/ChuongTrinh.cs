namespace LanguageCenter.Domain.Entities;

/// <summary>ChuongTrinh — Chương trình đào tạo.</summary>
public class ChuongTrinh
{
    public int MaChuongTrinh { get; set; }

    public string TenChuongTrinh { get; set; } = null!;

    public string? NhomDoiTuong { get; set; }

    public int? DoTuoiTu { get; set; }

    public int? DoTuoiDen { get; set; }

    public string? MucTieu { get; set; }

    public string? MoTa { get; set; }

    public string TrangThai { get; set; } = null!;

    public ICollection<CapDo> CapDos { get; set; } = new List<CapDo>();
}
