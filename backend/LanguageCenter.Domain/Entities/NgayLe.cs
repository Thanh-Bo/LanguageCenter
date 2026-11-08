namespace LanguageCenter.Domain.Entities;

/// <summary>
/// NgayLe — Kỳ nghỉ của trung tâm (1 dòng = 1 kỳ nghỉ, có thể nhiều ngày).
/// <para>
/// Dùng khi sinh ca học: buổi nào rơi vào kỳ nghỉ sẽ được dời sang tuần sau
/// (cùng thứ) và toàn bộ buổi phía sau cũng dời theo để giữ 2 buổi/tuần.
/// </para>
/// </summary>
public class NgayLe
{
    public int MaNgayLe { get; set; }

    /// <summary>Ngày bắt đầu kỳ nghỉ.</summary>
    public DateOnly Ngay { get; set; }

    /// <summary>
    /// Ngày kết thúc kỳ nghỉ. <c>null</c> = chỉ nghỉ đúng 1 ngày (<see cref="Ngay"/>).
    /// </summary>
    public DateOnly? DenNgay { get; set; }

    public string TenNgayLe { get; set; } = null!;

    public string? GhiChu { get; set; }
}
