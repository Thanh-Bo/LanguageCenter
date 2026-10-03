namespace LanguageCenter.Domain.Entities;

/// <summary>PhuHuynh — Phụ huynh. Một phụ huynh có thể đại diện cho nhiều học viên.</summary>
public class PhuHuynh
{
    public int MaPhuHuynh { get; set; }

    public string HoTen { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public string? Email { get; set; }

    public string? DiaChi { get; set; }

    public string? NgheNghiep { get; set; }

    public ICollection<HocVien> HocViens { get; set; } = new List<HocVien>();
}
