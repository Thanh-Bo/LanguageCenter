namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Vai trò trong phân công giảng dạy (<c>PhanCongGiangDay.VaiTroPhanCong</c>).
/// <para>
/// PHẢI khớp CHECK constraint <c>CK_PhanCong_VaiTro</c> của database:
/// chỉ 3 giá trị dưới đây được chấp nhận.
/// </para>
/// </summary>
public static class AssignmentRoles
{
    public const string GiaoVienChinh = "Giáo viên chính";

    public const string GiaoVienThayThe = "Giáo viên thay thế";

    public const string TroGiang = "Trợ giảng";

    public static readonly IReadOnlyList<string> All =
        [GiaoVienChinh, GiaoVienThayThe, TroGiang];

    /// <summary>Các vai trò yêu cầu hồ sơ <c>GiaoVien.LoaiGiaoVien = "Giáo viên"</c>.</summary>
    public static readonly IReadOnlyList<string> RequireTeacherProfile = [GiaoVienChinh, GiaoVienThayThe];

    public static bool IsValid(string? vaiTro)
        => vaiTro is not null && All.Contains(vaiTro);

    /// <summary>Vai trò này có bắt buộc hồ sơ phải là "Giáo viên" (không phải Trợ giảng) không.</summary>
    public static bool NeedsTeacherProfile(string vaiTro)
        => RequireTeacherProfile.Contains(vaiTro);
}
