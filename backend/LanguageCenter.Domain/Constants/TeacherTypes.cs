namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Loại giáo viên — PHẢI khớp CHECK CONSTRAINT <c>CK_GiaoVien_Loai</c>
/// (<c>LoaiGiaoVien IN (N'Giáo viên', N'Giáo viên nước ngoài', N'Trợ giảng')</c>).
/// </summary>
public static class TeacherTypes
{
    public const string GiaoVien = "Giáo viên";

    /// <summary>
    /// Giáo viên bản ngữ / nước ngoài — vẫn là GIÁO VIÊN (dạy được, nhận vai trò giáo viên),
    /// chỉ khác loại hồ sơ để tách riêng màn hình quản lý.
    /// </summary>
    public const string GiaoVienNuocNgoai = "Giáo viên nước ngoài";

    public const string TroGiang = "Trợ giảng";

    public static readonly IReadOnlyList<string> All = [GiaoVien, GiaoVienNuocNgoai, TroGiang];

    /// <summary>Chỉ các loại có vai trò GIẢNG DẠY (Giáo viên + Giáo viên nước ngoài).</summary>
    public static readonly IReadOnlyList<string> Teaching = [GiaoVien, GiaoVienNuocNgoai];

    public static bool IsValid(string? loaiGiaoVien)
        => loaiGiaoVien is not null && All.Contains(loaiGiaoVien);

    /// <summary>Hồ sơ có thể đảm nhận vai trò GIÁO VIÊN (Việt/Nước ngoài) hay không.</summary>
    public static bool IsTeacherType(string? loaiGiaoVien)
        => loaiGiaoVien is not null && Teaching.Contains(loaiGiaoVien);
}
