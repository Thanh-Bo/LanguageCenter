namespace LanguageCenter.Domain.Entities;

/// <summary>VaiTro — Vai trò người dùng trong hệ thống.</summary>
public class VaiTro
{
    public int MaVaiTro { get; set; }

    public string TenVaiTro { get; set; } = null!;

    public string? MoTa { get; set; }

    public bool TrangThai { get; set; }

    public DateTime NgayTao { get; set; }

    public ICollection<TaiKhoan> TaiKhoans { get; set; } = new List<TaiKhoan>();
}
