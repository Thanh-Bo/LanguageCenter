using System.ComponentModel.DataAnnotations;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Enrollments.Models;

// ============================================================================
// REQUEST
// ============================================================================

/// <summary>
/// Body của <c>POST /api/admin/enrollments</c>.
/// <para>
/// KHÔNG nhận: <c>MaDangKy</c>, <c>MaNhanVienTuVan</c> (lấy từ JWT),
/// <c>HocPhiGoc</c> (lấy từ khóa học), <c>SoTienGiam</c> (server tự tính từ ưu đãi),
/// <c>HocPhiSauGiam</c> (cột computed).
/// </para>
/// </summary>
public sealed class CreateEnrollmentRequest
{
    [Required(ErrorMessage = "Học viên là bắt buộc.")]
    [StringLength(20, MinimumLength = 1)]
    public string MaHocVien { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Khóa học là bắt buộc.")]
    public int MaKhoaHoc { get; set; }

    /// <summary>Để trống = đăng ký khóa học trước, xếp lớp sau.</summary>
    public int? MaLopHoc { get; set; }

    /// <summary>Ưu đãi áp dụng — server tự kiểm tra và tự tính số tiền giảm.</summary>
    public int? MaUuDai { get; set; }

    [StringLength(500, ErrorMessage = "Lịch học mong muốn tối đa 500 ký tự.")]
    public string? LichHocMongMuon { get; set; }

    [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự.")]
    public string? GhiChu { get; set; }
}

/// <summary>
/// Body của <c>PATCH /api/admin/enrollments/{id}</c>.
/// <para>Đổi ưu đãi sẽ được server TÍNH LẠI số tiền giảm.</para>
/// </summary>
public sealed class UpdateEnrollmentRequest
{
    /// <summary>Ưu đãi mới. <c>null</c> = giữ nguyên.</summary>
    public int? MaUuDai { get; set; }

    /// <summary>Đặt <c>true</c> để BỎ ưu đãi hiện tại (số tiền giảm về 0).</summary>
    public bool? BoUuDai { get; set; }

    [StringLength(500, ErrorMessage = "Lịch học mong muốn tối đa 500 ký tự.")]
    public string? LichHocMongMuon { get; set; }

    [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự.")]
    public string? GhiChu { get; set; }
}

/// <summary>Body của <c>PATCH /api/admin/enrollments/{id}/assign-class</c>.</summary>
public sealed class AssignClassRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Lớp học là bắt buộc.")]
    public int MaLopHoc { get; set; }
}

/// <summary>Tham số lọc danh sách đăng ký.</summary>
public sealed class EnrollmentListQuery : PagedQuery
{
    /// <summary>Tìm theo mã đăng ký, mã/tên học viên hoặc tên khóa học.</summary>
    public string? Search { get; set; }

    /// <summary>"Chờ xử lý" / "Đã xếp lớp" / "Đã hủy" / "Hoàn thành".</summary>
    public string? TrangThai { get; set; }

    public int? MaKhoaHoc { get; set; }

    public int? MaLopHoc { get; set; }

    public string? MaHocVien { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>Một dòng trong danh sách đăng ký.</summary>
public sealed class EnrollmentListItemDto
{
    public int MaDangKy { get; init; }

    public string MaHocVien { get; init; } = string.Empty;

    public string TenHocVien { get; init; } = string.Empty;

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    public int? MaLopHoc { get; init; }

    public string? MaLop { get; init; }

    public DateTime NgayDangKy { get; init; }

    public decimal HocPhiGoc { get; init; }

    public decimal SoTienGiam { get; init; }

    /// <summary>Cột computed PERSISTED do SQL Server tính.</summary>
    public decimal HocPhiSauGiam { get; init; }

    public string TrangThaiDangKy { get; init; } = string.Empty;

    public DateTime? NgayXepLop { get; init; }
}

/// <summary>Chi tiết đăng ký.</summary>
public sealed class EnrollmentDetailDto
{
    public int MaDangKy { get; init; }

    public string MaHocVien { get; init; } = string.Empty;

    public string TenHocVien { get; init; } = string.Empty;

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    public int? MaLopHoc { get; init; }

    public string? MaLop { get; init; }

    public string MaNhanVienTuVan { get; init; } = string.Empty;

    public string TenNhanVienTuVan { get; init; } = string.Empty;

    public int? MaUuDai { get; init; }

    public string? TenUuDai { get; init; }

    public DateTime NgayDangKy { get; init; }

    public string? LichHocMongMuon { get; init; }

    public decimal HocPhiGoc { get; init; }

    public decimal SoTienGiam { get; init; }

    public decimal HocPhiSauGiam { get; init; }

    public DateTime? NgayXepLop { get; init; }

    public string TrangThaiDangKy { get; init; } = string.Empty;

    public string? GhiChu { get; init; }
}
