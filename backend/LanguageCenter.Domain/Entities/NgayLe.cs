namespace LanguageCenter.Domain.Entities;

/// <summary>
/// NgayLe — Ngày lễ / ngày nghỉ của trung tâm.
/// <para>
/// Dùng khi sinh ca học: buổi nào rơi vào ngày lễ sẽ được dời sang tuần sau
/// (cùng thứ) và toàn bộ buổi phía sau cũng dời theo để giữ 2 buổi/tuần.
/// </para>
/// </summary>
public class NgayLe
{
    public int MaNgayLe { get; set; }

    /// <summary>Ngày lễ (UNIQUE — mỗi ngày chỉ 1 dòng).</summary>
    public DateOnly Ngay { get; set; }

    public string TenNgayLe { get; set; } = null!;

    public string? GhiChu { get; set; }
}
