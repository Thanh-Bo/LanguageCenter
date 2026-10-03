namespace LanguageCenter.Domain.Entities;

/// <summary>LichHoc — Lịch học lặp theo tuần của lớp. ThuTrongTuan: 1 = Thứ Hai ... 7 = Chủ Nhật.</summary>
public class LichHoc
{
    public int MaLichHoc { get; set; }

    public int MaLopHoc { get; set; }

    public int MaPhongHoc { get; set; }

    public byte ThuTrongTuan { get; set; }

    public TimeOnly GioBatDau { get; set; }

    public TimeOnly GioKetThuc { get; set; }

    public DateOnly NgayApDungTu { get; set; }

    public DateOnly? NgayApDungDen { get; set; }

    public string TrangThai { get; set; } = null!;

    // References
    public LopHoc LopHoc { get; set; } = null!;

    public PhongHoc PhongHoc { get; set; } = null!;

    // Collections
    public ICollection<CaHoc> CaHocs { get; set; } = new List<CaHoc>();
}
