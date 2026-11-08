namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Chức vụ của <c>NhanVien</c> — giá trị PHẢI khớp CHÍNH XÁC dữ liệu trong cột
/// <c>NhanVien.ChucVu</c> và cũng là tên vai trò tương ứng trong bảng <c>VaiTro</c>.
/// <para>
/// Quy ước: <b>Chức vụ quyết định VAI TRÒ</b>. Client KHÔNG bao giờ gửi vai trò —
/// xem <see cref="ResolveRole"/>.
/// </para>
/// <para>
/// Chức vụ cũng quyết định TIỀN TỐ mã nhân viên do backend sinh, nhờ vậy mã tự sinh
/// luôn có ý nghĩa và không đụng nhau giữa các phòng ban:
/// <list type="bullet">
///   <item>Nhân viên học vụ → <c>NVHV001</c>, <c>NVHV002</c>, …</item>
///   <item>Nhân viên kế toán → <c>NVKT001</c>, <c>NVKT002</c>, … (NVKT001 là nhân viên kế toán hệ thống)</item>
///   <item>Nhân viên tư vấn → <c>NVTV001</c>, <c>NVTV002</c>, …</item>
/// </list>
/// </para>
/// </summary>
public static class StaffPositions
{
    public const string NhanVienHocVu = "Nhân viên học vụ";
    public const string NhanVienTuVan = "Nhân viên tư vấn";
    public const string NhanVienKeToan = "Nhân viên kế toán";

    /// <summary>Tiền tố mã nhân viên theo chức vụ.</summary>
    public const string PrefixHocVu = "NVHV";
    public const string PrefixTuVan = "NVTV";
    public const string PrefixKeToan = "NVKT";

    public static readonly IReadOnlyList<string> All =
    [
        NhanVienHocVu,
        NhanVienTuVan,
        NhanVienKeToan
    ];

    /// <summary>Chức vụ có nằm trong danh sách cho phép không (so sánh sau khi Trim).</summary>
    public static bool IsValid(string? chucVu)
        => !string.IsNullOrWhiteSpace(chucVu) && All.Contains(chucVu.Trim());

    /// <summary>Chức vụ → tên vai trò trong bảng <c>VaiTro</c>.</summary>
    public static string ResolveRole(string chucVu) => chucVu.Trim() switch
    {
        NhanVienKeToan => RoleNames.NhanVienKeToan,
        NhanVienTuVan => RoleNames.NhanVienTuVan,
        _ => RoleNames.NhanVienHocVu
    };

    /// <summary>Chức vụ → tiền tố mã nhân viên.</summary>
    public static string ResolvePrefix(string chucVu) => chucVu.Trim() switch
    {
        NhanVienKeToan => PrefixKeToan,
        NhanVienTuVan => PrefixTuVan,
        _ => PrefixHocVu
    };

    /// <summary>
    /// Nhân viên kế toán hệ thống — người được gán làm "người ghi nhận" cho giao dịch
    /// do học viên tự thực hiện. Xem <c>PaymentService.SystemCollectorCode</c>.
    /// </summary>
    public const string SystemAccountantCode = "NVKT001";
}
