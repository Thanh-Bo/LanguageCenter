namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Trạng thái đăng ký học (<c>DangKyHoc.TrangThaiDangKy</c>).
/// <para>
/// Khớp DEFAULT của database (<c>N'Chờ xử lý'</c>). Cột này KHÔNG có CHECK constraint
/// nên đây là lớp bảo vệ DUY NHẤT chống giá trị rác.
/// </para>
/// </summary>
public static class EnrollmentStatuses
{
    public const string ChoXuLy = "Chờ xử lý";

    public const string DaXepLop = "Đã xếp lớp";

    public const string DaHuy = "Đã hủy";

    public const string HoanThanh = "Hoàn thành";

    public static readonly IReadOnlyList<string> All =
        [ChoXuLy, DaXepLop, DaHuy, HoanThanh];

    /// <summary>
    /// Trạng thái ĐƯỢC TÍNH VÀO SĨ SỐ lớp.
    /// "Hoàn thành" vẫn chiếm chỗ (học viên đã/đang học trong lớp),
    /// "Chờ xử lý" chưa vào lớp, "Đã hủy" đã rời lớp.
    /// </summary>
    public static readonly IReadOnlyList<string> CountedForCapacity = [DaXepLop, HoanThanh];

    /// <summary>Đăng ký còn hiệu lực — dùng để chặn đăng ký trùng.</summary>
    public static readonly IReadOnlyList<string> Active = [ChoXuLy, DaXepLop];

    public static bool IsValid(string? status)
        => status is not null && All.Contains(status);

    public static bool CountsForCapacity(string? status)
        => status is not null && CountedForCapacity.Contains(status);

    public static bool IsActive(string? status)
        => status is not null && Active.Contains(status);
}
