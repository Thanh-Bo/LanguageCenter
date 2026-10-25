using System.ComponentModel.DataAnnotations;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Classes.Models;

// ============================================================================
// REQUEST
// ============================================================================

/// <summary>
/// Body của <c>POST /api/admin/classes</c>.
/// <para>
/// Ngày của LỚP độc lập với ngày của KHÓA HỌC — không ràng buộc lớp phải nằm trong
/// khoảng thời gian của khóa (một khóa có thể mở nhiều đợt lớp khác nhau).
/// </para>
/// </summary>
public sealed class CreateClassRequest
{
    /// <summary>
    /// Mã lớp — ĐỂ TRỐNG thì hệ thống tự sinh theo định dạng
    /// <c>{Ký hiệu cấp độ}-{STT:00}</c> (ví dụ <c>TEEN-2-01</c>).
    /// </summary>
    [StringLength(30, ErrorMessage = "Mã lớp tối đa 30 ký tự.")]
    public string? MaLop { get; set; }

    [Required(ErrorMessage = "Tên lớp là bắt buộc.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Tên lớp phải từ 2 đến 200 ký tự.")]
    public string TenLop { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Khóa học là bắt buộc.")]
    public int MaKhoaHoc { get; set; }

    [Required(ErrorMessage = "Nhân viên học vụ phụ trách là bắt buộc.")]
    [StringLength(20, MinimumLength = 1)]
    public string MaNhanVienHocVu { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc.")]
    public DateOnly NgayBatDau { get; set; }

    [Required(ErrorMessage = "Ngày kết thúc dự kiến là bắt buộc.")]
    public DateOnly NgayKetThucDuKien { get; set; }

    [Range(1, 500, ErrorMessage = "Sĩ số tối đa phải lớn hơn 0.")]
    public int SiSoToiDa { get; set; }
}

/// <summary>
/// Body của <c>PATCH /api/admin/classes/{id}</c> — mọi field OPTIONAL.
/// <para>KHÔNG có <c>MaKhoaHoc</c>: không cho chuyển lớp sang khóa học khác.</para>
/// </summary>
public sealed class UpdateClassRequest
{
    [StringLength(30, MinimumLength = 1, ErrorMessage = "Mã lớp tối đa 30 ký tự.")]
    public string? MaLop { get; set; }

    [StringLength(200, MinimumLength = 2, ErrorMessage = "Tên lớp phải từ 2 đến 200 ký tự.")]
    public string? TenLop { get; set; }

    [StringLength(20, MinimumLength = 1)]
    public string? MaNhanVienHocVu { get; set; }

    public DateOnly? NgayBatDau { get; set; }

    public DateOnly? NgayKetThucDuKien { get; set; }

    public DateOnly? NgayKetThucThucTe { get; set; }

    [Range(1, 500, ErrorMessage = "Sĩ số tối đa phải lớn hơn 0.")]
    public int? SiSoToiDa { get; set; }
}

/// <summary>Tham số lọc danh sách lớp.</summary>
public sealed class ClassListQuery : PagedQuery
{
    /// <summary>Lọc theo khóa học.</summary>
    public int? MaKhoaHoc { get; set; }

    /// <summary>"Chuẩn bị mở" / "Đang học" / "Đã kết thúc" / "Đã hủy".</summary>
    public string? TrangThai { get; set; }

    /// <summary>Tìm theo mã lớp hoặc tên lớp.</summary>
    public string? Search { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>Một dòng trong danh sách lớp.</summary>
public sealed class ClassListItemDto
{
    public int MaLopHoc { get; init; }

    public string MaLop { get; init; } = string.Empty;

    public string TenLop { get; init; } = string.Empty;

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    public string MaNhanVienHocVu { get; init; } = string.Empty;

    public string TenNhanVienHocVu { get; init; } = string.Empty;

    public DateOnly NgayBatDau { get; init; }

    public DateOnly NgayKetThucDuKien { get; init; }

    public DateOnly? NgayKetThucThucTe { get; init; }

    public int SiSoToiDa { get; init; }

    /// <summary>Số học viên đang chiếm chỗ ("Đã xếp lớp" + "Hoàn thành").</summary>
    public int SiSoHienTai { get; init; }

    /// <summary>Số ca học đã sinh cho lớp. <c>0</c> = lớp chưa có buổi học nào.</summary>
    public int SoCaHoc { get; init; }

    public string TrangThai { get; init; } = string.Empty;
}

/// <summary>Chi tiết lớp.</summary>
public sealed class ClassDetailDto
{
    public int MaLopHoc { get; init; }

    public string MaLop { get; init; } = string.Empty;

    public string TenLop { get; init; } = string.Empty;

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    public int MaCapDo { get; init; }

    public string TenCapDo { get; init; } = string.Empty;

    public int MaChuongTrinh { get; init; }

    public string TenChuongTrinh { get; init; } = string.Empty;

    public string MaNhanVienHocVu { get; init; } = string.Empty;

    public string TenNhanVienHocVu { get; init; } = string.Empty;

    public DateOnly NgayBatDau { get; init; }

    public DateOnly NgayKetThucDuKien { get; init; }

    public DateOnly? NgayKetThucThucTe { get; init; }

    public int SiSoToiDa { get; init; }

    public int SiSoHienTai { get; init; }

    public int SoPhanCong { get; init; }

    public string TrangThai { get; init; } = string.Empty;
}
