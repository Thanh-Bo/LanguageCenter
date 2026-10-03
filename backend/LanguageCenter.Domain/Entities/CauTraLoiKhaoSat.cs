namespace LanguageCenter.Domain.Entities;

/// <summary>CauTraLoiKhaoSat — Câu trả lời chi tiết cho từng câu hỏi khảo sát.</summary>
public class CauTraLoiKhaoSat
{
    public int MaCauTraLoi { get; set; }

    public int MaPhanHoiKhaoSat { get; set; }

    public int MaCauHoi { get; set; }

    public decimal? GiaTriDanhGia { get; set; }

    public string? NoiDungTraLoi { get; set; }

    // References
    public PhanHoiKhaoSat PhanHoiKhaoSat { get; set; } = null!;

    public CauHoiKhaoSat CauHoiKhaoSat { get; set; } = null!;
}
