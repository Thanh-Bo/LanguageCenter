using System.ComponentModel.DataAnnotations;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Levels.Models;

// ============================================================================
// REQUEST
// ============================================================================

/// <summary>
/// Body của <c>POST /api/admin/levels</c>.
/// <para>KHÔNG nhận <c>MaCapDo</c>, <c>TrangThai</c> — backend tự xác định.</para>
/// </summary>
public sealed class CreateLevelRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Chương trình là bắt buộc.")]
    public int MaChuongTrinh { get; set; }

    [StringLength(30, ErrorMessage = "Ký hiệu cấp độ tối đa 30 ký tự.")]
    public string? KyHieuCapDo { get; set; }

    [Required(ErrorMessage = "Tên cấp độ là bắt buộc.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Tên cấp độ phải từ 2 đến 150 ký tự.")]
    public string TenCapDo { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Thứ tự phải lớn hơn 0.")]
    public int ThuTu { get; set; } = 1;

    [StringLength(1000, ErrorMessage = "Điều kiện đầu vào tối đa 1000 ký tự.")]
    public string? DieuKienDauVao { get; set; }

    [StringLength(1000, ErrorMessage = "Kết quả đầu ra tối đa 1000 ký tự.")]
    public string? KetQuaDauRa { get; set; }

    [Range(1, 520, ErrorMessage = "Thời lượng dự kiến phải trong khoảng 1 - 520 tuần.")]
    public int? ThoiLuongDuKienTuan { get; set; }
}

/// <summary>
/// Body của <c>PATCH /api/admin/levels/{id}</c> — mọi field OPTIONAL.
/// <para>
/// KHÔNG có <c>MaChuongTrinh</c>: không cho chuyển cấp độ sang chương trình khác,
/// vì sẽ làm khóa học hiện có trỏ sai chương trình.
/// </para>
/// </summary>
public sealed class UpdateLevelRequest
{
    [StringLength(30, ErrorMessage = "Ký hiệu cấp độ tối đa 30 ký tự.")]
    public string? KyHieuCapDo { get; set; }

    [StringLength(150, MinimumLength = 2, ErrorMessage = "Tên cấp độ phải từ 2 đến 150 ký tự.")]
    public string? TenCapDo { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Thứ tự phải lớn hơn 0.")]
    public int? ThuTu { get; set; }

    [StringLength(1000, ErrorMessage = "Điều kiện đầu vào tối đa 1000 ký tự.")]
    public string? DieuKienDauVao { get; set; }

    [StringLength(1000, ErrorMessage = "Kết quả đầu ra tối đa 1000 ký tự.")]
    public string? KetQuaDauRa { get; set; }

    [Range(1, 520, ErrorMessage = "Thời lượng dự kiến phải trong khoảng 1 - 520 tuần.")]
    public int? ThoiLuongDuKienTuan { get; set; }
}

/// <summary>Tham số lọc danh sách cấp độ.</summary>
public sealed class LevelListQuery : PagedQuery
{
    /// <summary>Lọc theo chương trình.</summary>
    public int? MaChuongTrinh { get; set; }

    /// <summary>Tìm theo tên hoặc ký hiệu cấp độ.</summary>
    public string? Search { get; set; }

    /// <summary>"Hoạt động" / "Ngừng hoạt động".</summary>
    public string? TrangThai { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>Một dòng trong danh sách cấp độ.</summary>
public sealed class LevelListItemDto
{
    public int MaCapDo { get; init; }

    public int MaChuongTrinh { get; init; }

    public string TenChuongTrinh { get; init; } = string.Empty;

    public string? KyHieuCapDo { get; init; }

    public string TenCapDo { get; init; } = string.Empty;

    public int ThuTu { get; init; }

    /// <summary>Đơn vị: TUẦN.</summary>
    public int? ThoiLuongDuKienTuan { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    public int SoKhoaHoc { get; init; }
}

/// <summary>Chi tiết cấp độ.</summary>
public sealed class LevelDetailDto
{
    public int MaCapDo { get; init; }

    public int MaChuongTrinh { get; init; }

    public string TenChuongTrinh { get; init; } = string.Empty;

    public string? KyHieuCapDo { get; init; }

    public string TenCapDo { get; init; } = string.Empty;

    public int ThuTu { get; init; }

    public string? DieuKienDauVao { get; init; }

    public string? KetQuaDauRa { get; init; }

    /// <summary>Đơn vị: TUẦN.</summary>
    public int? ThoiLuongDuKienTuan { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    public int SoKhoaHoc { get; init; }
}
