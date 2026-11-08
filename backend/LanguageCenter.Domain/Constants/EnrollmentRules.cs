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

    /// <summary>
    /// Trung tâm thu học phí TRƯỚC khi học viên vào lớp ⇒ chỉ cho xếp lớp khi
    /// phiếu học phí đã tất toán, nhờ vậy hồ sơ không bao giờ phát sinh công nợ.
    /// <para>
    /// Đổi thành <c>false</c> nếu muốn cho phép xếp lớp trước rồi thu tiền sau.
    /// </para>
    /// </summary>
    public const bool BatBuocThanhToanTruocKhiXepLop = true;

    /// <summary>
    /// Tạo đăng ký học là TỰ ĐỘNG sinh luôn phiếu học phí (trạng thái "Chưa thanh toán",
    /// số tiền copy từ đăng ký) trong CÙNG transaction.
    /// <para>
    /// Nhờ vậy màn hình Học phí có ngay phiếu để thu tiền, không phải bấm tay
    /// "Tạo phiếu học phí" cho từng đăng ký. Đổi thành <c>false</c> nếu muốn tạo phiếu thủ công.
    /// </para>
    /// </summary>
    public const bool TuDongTaoPhieuHocPhi = true;
}
