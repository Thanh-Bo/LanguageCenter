using System.ComponentModel.DataAnnotations;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.TeachingAssignments.Models;

// ============================================================================
// REQUEST
// ============================================================================

/// <summary>
/// Body của <c>POST /api/admin/teaching-assignments</c>.
/// <para>
/// <c>VaiTroPhanCong</c> PHẢI thuộc 3 giá trị CHECK constraint của database.
/// Người được phân công phải có hồ sơ trong bảng <c>GiaoVien</c> với loại phù hợp.
/// </para>
/// </summary>
public sealed class CreateAssignmentRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Lớp học là bắt buộc.")]
    public int MaLopHoc { get; set; }

    [Required(ErrorMessage = "Giáo viên / trợ giảng là bắt buộc.")]
    [StringLength(20, MinimumLength = 1)]
    public string MaGiaoVien { get; set; } = string.Empty;

    [Required(ErrorMessage = "Vai trò phân công là bắt buộc.")]
    [StringLength(50)]
    public string VaiTroPhanCong { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc.")]
    public DateOnly NgayBatDau { get; set; }

    /// <summary>Để trống = phân công không có ngày kết thúc (còn hiệu lực).</summary>
    public DateOnly? NgayKetThuc { get; set; }

    [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự.")]
    public string? GhiChu { get; set; }
}

/// <summary>
/// Body của <c>PATCH /api/admin/teaching-assignments/{id}</c> — mọi field OPTIONAL.
/// <para>KHÔNG có <c>MaLopHoc</c>: muốn phân công lớp khác thì tạo bản ghi mới (giữ lịch sử).</para>
/// </summary>
public sealed class UpdateAssignmentRequest
{
    [StringLength(20, MinimumLength = 1)]
    public string? MaGiaoVien { get; set; }

    [StringLength(50)]
    public string? VaiTroPhanCong { get; set; }

    public DateOnly? NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự.")]
    public string? GhiChu { get; set; }
}

/// <summary>Tham số lọc danh sách phân công.</summary>
public sealed class AssignmentListQuery : PagedQuery
{
    /// <summary>Lọc theo lớp.</summary>
    public int? MaLopHoc { get; set; }

    /// <summary>Lọc theo giáo viên / trợ giảng.</summary>
    public string? MaGiaoVien { get; set; }

    /// <summary>"Đang phân công" / "Đã kết thúc" / "Đã hủy".</summary>
    public string? TrangThai { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>Một dòng trong danh sách phân công.</summary>
public sealed class AssignmentListItemDto
{
    public int MaPhanCong { get; init; }

    public int MaLopHoc { get; init; }

    public string MaLop { get; init; } = string.Empty;

    public string TenLop { get; init; } = string.Empty;

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    public string MaGiaoVien { get; init; } = string.Empty;

    public string TenGiaoVien { get; init; } = string.Empty;

    public string LoaiGiaoVien { get; init; } = string.Empty;

    public string VaiTroPhanCong { get; init; } = string.Empty;

    public DateOnly NgayBatDau { get; init; }

    public DateOnly? NgayKetThuc { get; init; }

    public string TrangThai { get; init; } = string.Empty;
}

/// <summary>Chi tiết phân công.</summary>
public sealed class AssignmentDetailDto
{
    public int MaPhanCong { get; init; }

    public int MaLopHoc { get; init; }

    public string MaLop { get; init; } = string.Empty;

    public string TenLop { get; init; } = string.Empty;

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    public string MaGiaoVien { get; init; } = string.Empty;

    public string TenGiaoVien { get; init; } = string.Empty;

    public string LoaiGiaoVien { get; init; } = string.Empty;

    public string VaiTroPhanCong { get; init; } = string.Empty;

    public DateOnly NgayBatDau { get; init; }

    public DateOnly? NgayKetThuc { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    public string? GhiChu { get; init; }
}
