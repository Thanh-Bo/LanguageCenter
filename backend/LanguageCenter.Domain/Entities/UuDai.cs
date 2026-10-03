namespace LanguageCenter.Domain.Entities;

/// <summary>UuDai — Chính sách ưu đãi độc lập (không có khóa ngoại).</summary>
public class UuDai
{
    public int MaUuDai { get; set; }

    public string TenUuDai { get; set; } = null!;

    public string LoaiGiamGia { get; set; } = null!;

    public decimal GiaTriGiam { get; set; }

    public int SoKhoaHocToiThieu { get; set; }

    public DateOnly NgayBatDau { get; set; }

    public DateOnly NgayKetThuc { get; set; }

    public string? Poster { get; set; }

    public string? MoTa { get; set; }

    public string TrangThai { get; set; } = null!;

    public ICollection<DangKyHoc> DangKyHocs { get; set; } = new List<DangKyHoc>();
}
