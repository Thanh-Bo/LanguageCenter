namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Trạng thái lớp học (<c>LopHoc.TrangThai</c>).
/// <para>
/// Khớp DEFAULT của database (<c>N'Chuẩn bị mở'</c>). Cột này KHÔNG có CHECK constraint.
/// </para>
/// </summary>
public static class ClassStatuses
{
    /// <summary>Trạng thái mặc định khi vừa tạo lớp.</summary>
    public const string ChuanBiMo = "Chuẩn bị mở";

    public const string DangHoc = "Đang học";

    public const string DaKetThuc = "Đã kết thúc";

    public const string DaHuy = "Đã hủy";

    public static readonly IReadOnlyList<string> All =
        [ChuanBiMo, DangHoc, DaKetThuc, DaHuy];

    /// <summary>Chỉ lớp ở các trạng thái này mới nhận thêm học viên.</summary>
    public static readonly IReadOnlyList<string> AcceptingStudents = [ChuanBiMo, DangHoc];

    public static bool IsValid(string? status)
        => status is not null && All.Contains(status);

    public static bool CanAcceptStudents(string? status)
        => status is not null && AcceptingStudents.Contains(status);
}
