using System.ComponentModel.DataAnnotations;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Notifications.Models;

// ============================================================================
// ACTOR
// ============================================================================

/// <summary>
/// Danh tính người gọi, dựng từ JWT.
/// <para><c>TenDangNhap</c> trùng <c>HocVien.MaHocVien</c> hoặc <c>GiaoVien.MaGiaoVien</c>.</para>
/// </summary>
public sealed record NotificationActor(int MaTaiKhoan, string TenDangNhap, string VaiTro);

// ============================================================================
// REQUEST
// ============================================================================

/// <summary>Tham số lọc danh sách thông báo của chính người dùng.</summary>
public sealed class NotificationListQuery : PagedQuery
{
    public string? LoaiThongBao { get; set; }

    /// <summary>Lọc theo <c>ThoiGianGui</c>.</summary>
    public DateOnly? TuNgay { get; set; }

    public DateOnly? DenNgay { get; set; }

    /// <summary>Tìm trong tiêu đề và nội dung.</summary>
    public string? Search { get; set; }
}

/// <summary>Tham số lọc danh sách thông báo dành cho quản trị.</summary>
public sealed class AdminNotificationListQuery : PagedQuery
{
    public int? MaTaiKhoanGui { get; set; }

    public int? MaTaiKhoanNhan { get; set; }

    public int? MaLopHoc { get; set; }

    /// <summary>"Đã gửi" / "Đã thu hồi".</summary>
    public string? TrangThai { get; set; }

    public string? LoaiThongBao { get; set; }

    public DateOnly? TuNgay { get; set; }

    public DateOnly? DenNgay { get; set; }

    public string? Search { get; set; }
}

/// <summary>
/// Body của <c>POST /api/admin/notifications</c>.
/// <para>
/// Phải có ÍT NHẤT một đích: <c>maTaiKhoanNhan</c> hoặc <c>maLopHoc</c>
/// (khớp CHECK <c>CK_ThongBao_DoiTuongNhan</c> của database).
/// </para>
/// <para>KHÔNG nhận <c>maTaiKhoanGui</c> — người gửi luôn lấy từ JWT.</para>
/// </summary>
public sealed class CreateNotificationRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Tài khoản nhận không hợp lệ.")]
    public int? MaTaiKhoanNhan { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Lớp học không hợp lệ.")]
    public int? MaLopHoc { get; set; }

    [Required(ErrorMessage = "Tiêu đề là bắt buộc.")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Tiêu đề phải từ 3 đến 255 ký tự.")]
    public string TieuDe { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nội dung là bắt buộc.")]
    [StringLength(8000, MinimumLength = 1, ErrorMessage = "Nội dung tối đa 8000 ký tự.")]
    public string NoiDung { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Loại thông báo tối đa 100 ký tự.")]
    public string? LoaiThongBao { get; set; }
}

/// <summary>Body của <c>PATCH /api/admin/notifications/{id}</c> — mọi field OPTIONAL.</summary>
public sealed class UpdateNotificationRequest
{
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Tiêu đề phải từ 3 đến 255 ký tự.")]
    public string? TieuDe { get; set; }

    [StringLength(8000, MinimumLength = 1)]
    public string? NoiDung { get; set; }

    /// <summary>Gửi chuỗi rỗng để xóa loại thông báo.</summary>
    [StringLength(100)]
    public string? LoaiThongBao { get; set; }

    /// <summary>"Đã gửi" / "Đã thu hồi".</summary>
    [StringLength(50)]
    public string? TrangThai { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>Một thông báo.</summary>
public sealed class NotificationDto
{
    public int MaThongBao { get; init; }

    public int MaTaiKhoanGui { get; init; }

    public string TenDangNhapNguoiGui { get; init; } = string.Empty;

    public int? MaTaiKhoanNhan { get; init; }

    public string? TenDangNhapNguoiNhan { get; init; }

    public int? MaLopHoc { get; init; }

    public string? MaLop { get; init; }

    public string? TenLop { get; init; }

    public string TieuDe { get; init; } = string.Empty;

    public string NoiDung { get; init; } = string.Empty;

    public string? LoaiThongBao { get; init; }

    public DateTime ThoiGianGui { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    /// <summary>Phạm vi gửi — giá trị suy ra, không lưu trong database.</summary>
    public string PhamVi
        => MaTaiKhoanNhan is not null && MaLopHoc is not null
            ? "Cá nhân và lớp"
            : MaTaiKhoanNhan is not null
                ? "Cá nhân"
                : "Cả lớp";
}

/// <summary>Số lượng thông báo người dùng hiện thấy được.</summary>
/// <remarks>
/// Schema KHÔNG có cột theo dõi đã đọc/chưa đọc, nên hệ thống chỉ trả TỔNG SỐ.
/// Trường <c>soChuaDoc</c> luôn <c>null</c> và được giữ lại để client biết rõ giới hạn này.
/// </remarks>
public sealed class NotificationSummaryDto
{
    public int TongSo { get; init; }

    /// <summary>Luôn <c>null</c> — database không hỗ trợ trạng thái đã đọc.</summary>
    public int? SoChuaDoc => null;

    public string GhiChu
        => "Bảng ThongBao không có cột theo dõi đã đọc/chưa đọc nên không thể đếm số chưa đọc.";
}
