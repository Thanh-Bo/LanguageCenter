namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Tên vai trò — giá trị PHẢI khớp CHÍNH XÁC dữ liệu thật trong bảng <c>VaiTro</c>.
/// KHÔNG dịch sang tiếng Anh ("Giáo viên" không được đổi thành "Teacher").
/// Dùng <c>const</c> để có thể truyền vào <c>[Authorize(Roles = ...)]</c> (yêu cầu compile-time constant).
/// </summary>
public static class RoleNames
{
    public const string QuanTriVien = "Quản trị viên";
    public const string NhanVienHocVu = "Nhân viên học vụ";
    public const string NhanVienTuVan = "Nhân viên tư vấn";
    public const string NhanVienKeToan = "Nhân viên kế toán";
    public const string GiaoVien = "Giáo viên";
    public const string TroGiang = "Trợ giảng";
    public const string HocVien = "Học viên";

    /// <summary>Toàn bộ vai trò hợp lệ — dùng cho validation/test.</summary>
    public static readonly IReadOnlyList<string> All =
    [
        QuanTriVien,
        NhanVienHocVu,
        NhanVienTuVan,
        NhanVienKeToan,
        GiaoVien,
        TroGiang,
        HocVien
    ];
}
