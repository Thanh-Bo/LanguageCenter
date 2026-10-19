using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Receipts.Models;

// ============================================================================
// REQUEST
// ============================================================================

/// <summary>Tham số lọc danh sách biên nhận.</summary>
public sealed class ReceiptListQuery : PagedQuery
{
    /// <summary>Tìm gần đúng theo số biên nhận.</summary>
    public string? SoBienNhan { get; set; }

    public string? MaHocVien { get; set; }

    public int? MaThanhToan { get; set; }

    /// <summary>"Đã phát hành" / "Đã hủy".</summary>
    public string? TrangThai { get; set; }

    /// <summary>Lọc theo <c>NgayLap</c>.</summary>
    public DateOnly? TuNgay { get; set; }

    public DateOnly? DenNgay { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>
/// Một biên nhận — kèm thông tin giao dịch, học phí, học viên và khóa học
/// để in/tra cứu mà không cần gọi thêm API.
/// </summary>
public sealed class ReceiptDto
{
    public int MaBienNhan { get; init; }

    public string SoBienNhan { get; init; } = string.Empty;

    public DateTime NgayLap { get; init; }

    public decimal SoTienXacNhan { get; init; }

    public string? NoiDung { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    public string MaNhanVienLap { get; init; } = string.Empty;

    public string TenNhanVienLap { get; init; } = string.Empty;

    // ---- Giao dịch gốc ----
    public int MaThanhToan { get; init; }

    public string? MaGiaoDich { get; init; }

    public string PhuongThucThanhToan { get; init; } = string.Empty;

    public string TrangThaiThanhToan { get; init; } = string.Empty;

    public DateTime NgayThanhToan { get; init; }

    // ---- Học phí / đăng ký / học viên ----
    public int MaHocPhi { get; init; }

    public int MaDangKy { get; init; }

    public string MaHocVien { get; init; } = string.Empty;

    public string HoTenHocVien { get; init; } = string.Empty;

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;
}
