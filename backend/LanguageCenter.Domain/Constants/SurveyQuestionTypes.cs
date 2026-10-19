namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Loại câu hỏi khảo sát (<c>CauHoiKhaoSat.LoaiCauHoi</c>).
/// <para>
/// Cột là <c>NVARCHAR(100) NOT NULL</c>, KHÔNG có CHECK và KHÔNG có bảng lựa chọn
/// (options) đi kèm. Vì vậy hệ thống chỉ có thể kiểm tra câu trả lời theo hai dạng
/// dữ liệu mà <c>CauTraLoiKhaoSat</c> thực sự lưu được:
/// </para>
/// <list type="bullet">
/// <item><c>GiaTriDanhGia</c> — DECIMAL(5,2), CHECK 0..5</item>
/// <item><c>NoiDungTraLoi</c> — NVARCHAR(2000)</item>
/// </list>
/// <para>
/// Hai giá trị dưới đây là dữ liệu THẬT đang tồn tại. Loại khác vẫn được chấp nhận
/// (không whitelist) và áp dụng quy tắc chung: phải có ít nhất một trong hai dạng.
/// </para>
/// </summary>
public static class SurveyQuestionTypes
{
    /// <summary>Trả lời bằng <c>GiaTriDanhGia</c> (0..5).</summary>
    public const string DanhGia = "Đánh giá 1-5";

    /// <summary>Trả lời bằng <c>NoiDungTraLoi</c>.</summary>
    public const string TuLuan = "Tự luận";

    /// <summary>Giá trị chuẩn đang dùng trong dữ liệu — không dùng để chặn.</summary>
    public static readonly IReadOnlyList<string> Standard = [DanhGia, TuLuan];

    public static bool IsRating(string? loaiCauHoi)
        => string.Equals(loaiCauHoi?.Trim(), DanhGia, StringComparison.Ordinal);

    public static bool IsEssay(string? loaiCauHoi)
        => string.Equals(loaiCauHoi?.Trim(), TuLuan, StringComparison.Ordinal);
}
