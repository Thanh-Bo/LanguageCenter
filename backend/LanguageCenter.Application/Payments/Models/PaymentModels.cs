using System.ComponentModel.DataAnnotations;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Payments.Models;

// ============================================================================
// ACTOR
// ============================================================================

/// <summary>
/// Danh tính người ghi nhận thanh toán, dựng từ JWT.
/// <para>
/// <c>TenDangNhap</c> trùng <c>NhanVien.MaNhanVien</c> theo quy ước của hệ thống.
/// </para>
/// </summary>
public sealed record PaymentActor(string TenDangNhap);

// ============================================================================
// REQUEST
// ============================================================================

/// <summary>
/// Body của <c>POST /api/admin/tuition-fees/{id}/payments</c>.
/// <para>
/// KHÔNG nhận <c>maNhanVienKeToan</c>, <c>ngayThanhToan</c>, <c>trangThai</c> từ client —
/// tất cả do server quyết định.
/// </para>
/// </summary>
public sealed class RecordPaymentRequest
{
    /// <summary>
    /// Khoá chống trùng (idempotency key) — BẮT BUỘC.
    /// Gửi lại cùng giá trị sẽ KHÔNG tạo giao dịch mới mà trả về giao dịch đã có.
    /// </summary>
    [Required(ErrorMessage = "Mã giao dịch là bắt buộc.")]
    [StringLength(150, MinimumLength = 3, ErrorMessage = "Mã giao dịch phải từ 3 đến 150 ký tự.")]
    public string MaGiaoDich { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.01", "1000000000000", ErrorMessage = "Số tiền phải lớn hơn 0.")]
    public decimal SoTien { get; set; }

    [Required(ErrorMessage = "Phương thức thanh toán là bắt buộc.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Phương thức thanh toán tối đa 100 ký tự.")]
    public string PhuongThucThanhToan { get; set; } = string.Empty;

    /// <summary>
    /// Loại giao dịch — để trống thì mặc định "Thu tiền".
    /// Phase 6 CHỈ hỗ trợ "Thu tiền"; giá trị khác bị từ chối.
    /// </summary>
    [StringLength(50)]
    public string? LoaiGiaoDich { get; set; }

    [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự.")]
    public string? GhiChu { get; set; }

    /// <summary>Nội dung in trên biên nhận — để trống thì server tự sinh.</summary>
    [StringLength(500, ErrorMessage = "Nội dung biên nhận tối đa 500 ký tự.")]
    public string? NoiDungBienNhan { get; set; }
}

/// <summary>Tham số lọc danh sách giao dịch thanh toán.</summary>
public sealed class PaymentListQuery : PagedQuery
{
    public int? MaHocPhi { get; set; }

    public string? MaHocVien { get; set; }

    public int? MaKhoaHoc { get; set; }

    public string? PhuongThucThanhToan { get; set; }

    /// <summary>"Thành công" / "Đã hủy".</summary>
    public string? TrangThai { get; set; }

    /// <summary>"Thu tiền" / "Hoàn tiền" / "Điều chỉnh".</summary>
    public string? LoaiGiaoDich { get; set; }

    /// <summary>Lọc theo <c>NgayThanhToan</c>.</summary>
    public DateOnly? TuNgay { get; set; }

    public DateOnly? DenNgay { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>Biên nhận rút gọn gắn kèm giao dịch.</summary>
public sealed class PaymentReceiptBriefDto
{
    public int MaBienNhan { get; init; }

    public string SoBienNhan { get; init; } = string.Empty;

    public DateTime NgayLap { get; init; }

    public string TrangThai { get; init; } = string.Empty;
}

/// <summary>Một giao dịch thanh toán (dùng cho cả danh sách và chi tiết).</summary>
public sealed class PaymentDto
{
    public int MaThanhToan { get; init; }

    public int MaHocPhi { get; init; }

    public int MaDangKy { get; init; }

    public string MaHocVien { get; init; } = string.Empty;

    public string HoTenHocVien { get; init; } = string.Empty;

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    public decimal SoTien { get; init; }

    public DateTime NgayThanhToan { get; init; }

    public string PhuongThucThanhToan { get; init; } = string.Empty;

    public string? MaGiaoDich { get; init; }

    public string LoaiGiaoDich { get; init; } = string.Empty;

    public string TrangThai { get; init; } = string.Empty;

    public string? GhiChu { get; init; }

    public string MaNhanVienKeToan { get; init; } = string.Empty;

    public string TenNhanVienKeToan { get; init; } = string.Empty;

    /// <summary>Biên nhận đi kèm — luôn có khi giao dịch được tạo qua Phase 6.</summary>
    public PaymentReceiptBriefDto? BienNhan { get; init; }
}

// ============================================================================
// KẾT QUẢ GHI (tầng hạ tầng → tầng nghiệp vụ)
// ============================================================================

/// <summary>Dữ liệu ghi nhận một khoản thanh toán (đã được service kiểm tra).</summary>
public sealed record PaymentWriteData(
    int MaHocPhi,
    string MaNhanVienKeToan,
    decimal SoTien,
    string PhuongThucThanhToan,
    string MaGiaoDich,
    string LoaiGiaoDich,
    string? GhiChu,
    string? NoiDungBienNhan);

/// <summary>Kết quả của thao tác ghi nhận thanh toán.</summary>
public enum PaymentWriteStatus
{
    Success = 0,
    TuitionNotFound = 1,
    ExceedsBalance = 2,
    Duplicate = 3,
    Conflict = 4
}

/// <summary>
/// <paramref name="MaThanhToan"/> là giao dịch MỚI khi <c>Success</c>,
/// là giao dịch ĐÃ CÓ khi <c>Duplicate</c>.
/// </summary>
public sealed record PaymentWriteResult(
    PaymentWriteStatus Status,
    int MaThanhToan,
    decimal SoTienConThieu);
