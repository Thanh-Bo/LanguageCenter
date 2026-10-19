using System.ComponentModel.DataAnnotations;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Programs.Models;

// ============================================================================
// REQUEST
// ============================================================================

/// <summary>
/// Body của <c>POST /api/admin/programs</c>.
/// <para>
/// KHÔNG nhận từ client: <c>MaChuongTrinh</c>, <c>TrangThai</c> — backend tự xác định
/// (mới tạo luôn ở trạng thái "Hoạt động").
/// </para>
/// </summary>
public sealed class CreateProgramRequest
{
    [Required(ErrorMessage = "Tên chương trình là bắt buộc.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Tên chương trình phải từ 2 đến 200 ký tự.")]
    public string TenChuongTrinh { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Nhóm đối tượng tối đa 200 ký tự.")]
    public string? NhomDoiTuong { get; set; }

    [Range(0, 200, ErrorMessage = "Độ tuổi từ phải trong khoảng 0 - 200.")]
    public int? DoTuoiTu { get; set; }

    [Range(0, 200, ErrorMessage = "Độ tuổi đến phải trong khoảng 0 - 200.")]
    public int? DoTuoiDen { get; set; }

    [StringLength(1000, ErrorMessage = "Mục tiêu tối đa 1000 ký tự.")]
    public string? MucTieu { get; set; }

    /// <summary>Cột <c>NVARCHAR(MAX)</c> — không giới hạn độ dài.</summary>
    public string? MoTa { get; set; }
}

/// <summary>
/// Body của <c>PATCH /api/admin/programs/{id}</c> — mọi field đều OPTIONAL.
/// <para><c>null</c> = không thay đổi. Chuỗi rỗng = xóa (đưa về NULL).</para>
/// <para>KHÔNG có <c>TrangThai</c> — dùng endpoint <c>PATCH .../status</c> riêng.</para>
/// </summary>
public sealed class UpdateProgramRequest
{
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Tên chương trình phải từ 2 đến 200 ký tự.")]
    public string? TenChuongTrinh { get; set; }

    [StringLength(200, ErrorMessage = "Nhóm đối tượng tối đa 200 ký tự.")]
    public string? NhomDoiTuong { get; set; }

    [Range(0, 200, ErrorMessage = "Độ tuổi từ phải trong khoảng 0 - 200.")]
    public int? DoTuoiTu { get; set; }

    [Range(0, 200, ErrorMessage = "Độ tuổi đến phải trong khoảng 0 - 200.")]
    public int? DoTuoiDen { get; set; }

    [StringLength(1000, ErrorMessage = "Mục tiêu tối đa 1000 ký tự.")]
    public string? MucTieu { get; set; }

    public string? MoTa { get; set; }
}

/// <summary>Tham số lọc danh sách chương trình.</summary>
public sealed class ProgramListQuery : PagedQuery
{
    /// <summary>Tìm theo tên chương trình (không phân biệt hoa thường theo collation của DB).</summary>
    public string? Search { get; set; }

    /// <summary>Lọc theo <c>TrangThai</c>: "Hoạt động" / "Ngừng hoạt động".</summary>
    public string? TrangThai { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>Một dòng trong danh sách chương trình.</summary>
public sealed class ProgramListItemDto
{
    public int MaChuongTrinh { get; init; }

    public string TenChuongTrinh { get; init; } = string.Empty;

    public string? NhomDoiTuong { get; init; }

    public int? DoTuoiTu { get; init; }

    public int? DoTuoiDen { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    /// <summary>Số cấp độ thuộc chương trình này.</summary>
    public int SoCapDo { get; init; }
}

/// <summary>Chi tiết chương trình.</summary>
public sealed class ProgramDetailDto
{
    public int MaChuongTrinh { get; init; }

    public string TenChuongTrinh { get; init; } = string.Empty;

    public string? NhomDoiTuong { get; init; }

    public int? DoTuoiTu { get; init; }

    public int? DoTuoiDen { get; init; }

    public string? MucTieu { get; init; }

    public string? MoTa { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    public int SoCapDo { get; init; }

    /// <summary>Tổng số khóa học của mọi cấp độ thuộc chương trình.</summary>
    public int SoKhoaHoc { get; init; }
}
