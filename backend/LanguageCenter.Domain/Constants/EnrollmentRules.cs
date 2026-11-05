namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Quy tắc xếp lớp cho học viên.
/// </summary>
public static class EnrollmentRules
{
    /// <summary>
    /// Lớp chỉ nhận thêm học viên khi số buổi ĐÃ DIỄN RA không vượt quá giá trị này.
    /// <para>
    /// Khung đào tạo là 20 buổi → mặc định chỉ nhận học viên trong 4 buổi đầu,
    /// tránh việc học viên vào lớp khi khóa đã gần kết thúc.
    /// </para>
    /// </summary>
    public const int SoBuoiDaDienRaToiDa = 4;
}
