namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Quy tắc nghiệp vụ cho việc phân công giảng dạy.
/// <para>
/// Mô hình tổ chức của trung tâm: mỗi chương trình gồm 3 khóa (riêng Mầm non 2 khóa),
/// và <b>một giáo viên phụ trách trọn một chương trình</b> → tối đa 3 lớp song song.
/// Đổi hằng số dưới đây nếu trung tâm thay đổi mô hình.
/// </para>
/// </summary>
public static class TeachingRules
{
    /// <summary>
    /// Số lớp ĐANG DẠY tối đa của một giáo viên / trợ giảng.
    /// Chỉ đếm phân công còn hiệu lực (không tính "Đã hủy", "Đã kết thúc", khung chờ phân công).
    /// </summary>
    public const int SoLopDongThoiToiDa = 3;

    /// <summary>
    /// Số buổi dạy tối đa của một giáo viên trong MỘT TUẦN.
    /// Mỗi lớp học 2 buổi/tuần → 3 lớp = 6 buổi.
    /// </summary>
    public const int SoBuoiMoiTuanToiDa = 6;
}
