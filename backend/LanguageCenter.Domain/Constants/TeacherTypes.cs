namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Loại giáo viên — PHẢI khớp CHECK CONSTRAINT <c>CK_GiaoVien_Loai</c>
/// (<c>CK_GiaoVien_Loai: LoaiGiaoVien IN (N'Giáo viên', N'Trợ giảng')</c>).
/// </summary>
public static class TeacherTypes
{
    public const string GiaoVien = "Giáo viên";
    public const string TroGiang = "Trợ giảng";

    public static readonly IReadOnlyList<string> All = [GiaoVien, TroGiang];

    public static bool IsValid(string? loaiGiaoVien)
        => loaiGiaoVien is not null && All.Contains(loaiGiaoVien);
}
