namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Trạng thái khóa học (<c>KhoaHoc.TrangThai</c>).
/// <para>
/// Cột này KHÔNG có CHECK constraint trong database. Giá trị <see cref="DangTuyenSinh"/>
/// là giá trị THẬT đang dùng trong dữ liệu mẫu.
/// </para>
/// </summary>
public static class CourseStatuses
{
    public const string DangTuyenSinh = "Đang tuyển sinh";

    public const string DangHoc = "Đang học";

    public const string DaKetThuc = "Đã kết thúc";

    public const string DaHuy = "Đã hủy";

    public static readonly IReadOnlyList<string> All =
        [DangTuyenSinh, DangHoc, DaKetThuc, DaHuy];

    /// <summary>Chỉ khóa học ở các trạng thái này mới nhận đăng ký mới.</summary>
    public static readonly IReadOnlyList<string> AcceptingEnrollment = [DangTuyenSinh, DangHoc];

    public static bool IsValid(string? status)
        => status is not null && All.Contains(status);

    public static bool CanAcceptEnrollment(string? status)
        => status is not null && AcceptingEnrollment.Contains(status);
}
