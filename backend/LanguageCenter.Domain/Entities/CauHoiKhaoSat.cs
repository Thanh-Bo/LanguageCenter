namespace LanguageCenter.Domain.Entities;

/// <summary>CauHoiKhaoSat — Câu hỏi thuộc một đợt khảo sát.</summary>
public class CauHoiKhaoSat
{
    public int MaCauHoi { get; set; }

    public int MaKhaoSat { get; set; }

    public string NoiDungCauHoi { get; set; } = null!;

    public string LoaiCauHoi { get; set; } = null!;

    public int ThuTu { get; set; }

    public bool BatBuoc { get; set; }

    public string TrangThai { get; set; } = null!;

    // References
    public KhaoSat KhaoSat { get; set; } = null!;

    // Collections
    public ICollection<CauTraLoiKhaoSat> CauTraLoiKhaoSats { get; set; } = new List<CauTraLoiKhaoSat>();
}
