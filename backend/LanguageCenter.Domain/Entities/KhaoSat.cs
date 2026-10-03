namespace LanguageCenter.Domain.Entities;

/// <summary>KhaoSat — Đợt khảo sát.</summary>
public class KhaoSat
{
    public int MaKhaoSat { get; set; }

    public string TenKhaoSat { get; set; } = null!;

    public string? MoTa { get; set; }

    public DateOnly NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    public string TrangThai { get; set; } = null!;

    public ICollection<CauHoiKhaoSat> CauHoiKhaoSats { get; set; } = new List<CauHoiKhaoSat>();

    public ICollection<PhanHoiKhaoSat> PhanHoiKhaoSats { get; set; } = new List<PhanHoiKhaoSat>();
}
