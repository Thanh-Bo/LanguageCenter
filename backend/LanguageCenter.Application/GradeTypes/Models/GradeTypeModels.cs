using System.ComponentModel.DataAnnotations;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.GradeTypes.Models;

// ============================================================================
// REQUEST
// ============================================================================

/// <summary>Body của <c>POST /api/admin/grade-types</c>.</summary>
public sealed class CreateGradeTypeRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Khóa học là bắt buộc.")]
    public int MaKhoaHoc { get; set; }

    [Required(ErrorMessage = "Tên đầu điểm là bắt buộc.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Tên đầu điểm phải từ 2 đến 150 ký tự.")]
    public string TenLoaiDiem { get; set; } = string.Empty;

    /// <summary>Trọng số phần trăm của đầu điểm trong điểm tổng kết.</summary>
    [Range(typeof(decimal), "0", "100", ErrorMessage = "Trọng số phải trong khoảng 0..100.")]
    public decimal TrongSo { get; set; }

    /// <summary>Điểm tối đa của đầu điểm (phải &gt; 0).</summary>
    [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "Điểm tối đa phải lớn hơn 0.")]
    public decimal DiemToiDa { get; set; }

    /// <summary>Thứ tự hiển thị (phải &gt; 0, không trùng trong cùng khóa học).</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Thứ tự phải lớn hơn 0.")]
    public int ThuTu { get; set; }
}

/// <summary>Body của <c>PATCH /api/admin/grade-types/{id}</c> — mọi field OPTIONAL.</summary>
public sealed class UpdateGradeTypeRequest
{
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Tên đầu điểm phải từ 2 đến 150 ký tự.")]
    public string? TenLoaiDiem { get; set; }

    [Range(typeof(decimal), "0", "100", ErrorMessage = "Trọng số phải trong khoảng 0..100.")]
    public decimal? TrongSo { get; set; }

    [Range(typeof(decimal), "0.01", "1000000", ErrorMessage = "Điểm tối đa phải lớn hơn 0.")]
    public decimal? DiemToiDa { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Thứ tự phải lớn hơn 0.")]
    public int? ThuTu { get; set; }
}

/// <summary>Tham số lọc danh sách đầu điểm.</summary>
public sealed class GradeTypeListQuery : PagedQuery
{
    public int? MaKhoaHoc { get; set; }

    /// <summary>"Hoạt động" / "Ngừng sử dụng".</summary>
    public string? TrangThai { get; set; }

    public string? Search { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>Một dòng trong danh sách đầu điểm.</summary>
public sealed class GradeTypeListItemDto
{
    public int MaLoaiDiem { get; init; }

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    public string TenLoaiDiem { get; init; } = string.Empty;

    public decimal TrongSo { get; init; }

    public decimal DiemToiDa { get; init; }

    public int ThuTu { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    /// <summary>Số bản ghi điểm đã nhập cho đầu điểm này.</summary>
    public int SoDiemDaNhap { get; init; }
}

/// <summary>Chi tiết đầu điểm.</summary>
public sealed class GradeTypeDetailDto
{
    public int MaLoaiDiem { get; init; }

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    public string TenLoaiDiem { get; init; } = string.Empty;

    public decimal TrongSo { get; init; }

    public decimal DiemToiDa { get; init; }

    public int ThuTu { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    public int SoDiemDaNhap { get; init; }
}

/// <summary>Thông tin rút gọn của một đầu điểm — dùng khi tính điểm tổng kết.</summary>
public sealed record GradeComponentInfo(
    int MaLoaiDiem,
    string TenLoaiDiem,
    decimal TrongSo,
    decimal DiemToiDa,
    int ThuTu);
