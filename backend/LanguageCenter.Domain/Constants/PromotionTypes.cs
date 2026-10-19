namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Loại giảm giá của ưu đãi (<c>UuDai.LoaiGiamGia</c>).
/// <para>
/// PHẢI khớp CHECK constraint <c>CK_UuDai_Loai</c> và <c>CK_UuDai_GiaTri</c>:
/// khi là "Phần trăm" thì <c>GiaTriGiam &lt;= 100</c>.
/// </para>
/// </summary>
public static class PromotionTypes
{
    public const string PhanTram = "Phần trăm";

    public const string SoTien = "Số tiền";

    public static readonly IReadOnlyList<string> All = [PhanTram, SoTien];

    public static bool IsValid(string? loai)
        => loai is not null && All.Contains(loai);

    public static bool IsPercent(string? loai) => loai == PhanTram;
}
