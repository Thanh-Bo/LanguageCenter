using System.ComponentModel.DataAnnotations;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Courses.Models;

// ============================================================================
// REQUEST
// ============================================================================

/// <summary>
/// Body của <c>POST /api/admin/courses</c>.
/// <para>
/// <c>NgayBatDau</c> / <c>NgayKetThuc</c> là THỜI GIAN TỔ CHỨC KHÓA HỌC (chính sách),
/// khác với thời gian của từng lớp cụ thể trong <c>LopHoc</c>.
/// </para>
/// </summary>
public sealed class CreateCourseRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Cấp độ là bắt buộc.")]
    public int MaCapDo { get; set; }

    [Required(ErrorMessage = "Tên khóa học là bắt buộc.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Tên khóa học phải từ 2 đến 200 ký tự.")]
    public string TenKhoaHoc { get; set; } = string.Empty;

    public string? NoiDung { get; set; }

    [Range(1, 1000, ErrorMessage = "Số buổi phải lớn hơn 0.")]
    public int SoBuoi { get; set; }

    [Range(0.01, 100000, ErrorMessage = "Tổng số giờ phải lớn hơn 0.")]
    public decimal? TongSoGio { get; set; }

    [Range(0, 1000000000000, ErrorMessage = "Học phí cơ bản không được âm.")]
    public decimal HocPhiCoBan { get; set; }

    [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc.")]
    public DateOnly NgayBatDau { get; set; }

    [Required(ErrorMessage = "Ngày kết thúc là bắt buộc.")]
    public DateOnly NgayKetThuc { get; set; }
}

/// <summary>
/// Body của <c>PATCH /api/admin/courses/{id}</c> — mọi field OPTIONAL.
/// <para>KHÔNG có <c>MaCapDo</c>: không cho chuyển khóa học sang cấp độ khác.</para>
/// </summary>
public sealed class UpdateCourseRequest
{
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Tên khóa học phải từ 2 đến 200 ký tự.")]
    public string? TenKhoaHoc { get; set; }

    public string? NoiDung { get; set; }

    [Range(1, 1000, ErrorMessage = "Số buổi phải lớn hơn 0.")]
    public int? SoBuoi { get; set; }

    [Range(0.01, 100000, ErrorMessage = "Tổng số giờ phải lớn hơn 0.")]
    public decimal? TongSoGio { get; set; }

    [Range(0, 1000000000000, ErrorMessage = "Học phí cơ bản không được âm.")]
    public decimal? HocPhiCoBan { get; set; }

    public DateOnly? NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }
}

/// <summary>Tham số lọc danh sách khóa học.</summary>
public sealed class CourseListQuery : PagedQuery
{
    /// <summary>Lọc theo cấp độ.</summary>
    public int? MaCapDo { get; set; }

    /// <summary>Lọc theo chương trình (gián tiếp qua cấp độ).</summary>
    public int? MaChuongTrinh { get; set; }

    /// <summary>Tìm theo tên khóa học.</summary>
    public string? Search { get; set; }

    /// <summary>"Đang tuyển sinh" / "Đang học" / "Đã kết thúc" / "Đã hủy".</summary>
    public string? TrangThai { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>Một dòng trong danh sách khóa học.</summary>
public sealed class CourseListItemDto
{
    public int MaKhoaHoc { get; init; }

    public int MaCapDo { get; init; }

    public string TenCapDo { get; init; } = string.Empty;

    public int MaChuongTrinh { get; init; }

    public string TenChuongTrinh { get; init; } = string.Empty;

    public string TenKhoaHoc { get; init; } = string.Empty;

    public int SoBuoi { get; init; }

    public decimal? TongSoGio { get; init; }

    public decimal HocPhiCoBan { get; init; }

    public DateOnly NgayBatDau { get; init; }

    public DateOnly NgayKetThuc { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    public int SoLop { get; init; }
}

/// <summary>Chi tiết khóa học.</summary>
public sealed class CourseDetailDto
{
    public int MaKhoaHoc { get; init; }

    public int MaCapDo { get; init; }

    public string TenCapDo { get; init; } = string.Empty;

    public int MaChuongTrinh { get; init; }

    public string TenChuongTrinh { get; init; } = string.Empty;

    public string TenKhoaHoc { get; init; } = string.Empty;

    public string? NoiDung { get; init; }

    public int SoBuoi { get; init; }

    public decimal? TongSoGio { get; init; }

    public decimal HocPhiCoBan { get; init; }

    public DateOnly NgayBatDau { get; init; }

    public DateOnly NgayKetThuc { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    public int SoLop { get; init; }

    public int SoDangKy { get; init; }
}
