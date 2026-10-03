namespace LanguageCenter.Domain.Entities;

/// <summary>DichVu — Danh mục dịch vụ của trung tâm.</summary>
public class DichVu
{
    public int MaDichVu { get; set; }

    public string TenDichVu { get; set; } = null!;

    public string? MoTa { get; set; }

    public string TrangThai { get; set; } = null!;

    public ICollection<PhanHoiDichVu> PhanHoiDichVus { get; set; } = new List<PhanHoiDichVu>();
}
