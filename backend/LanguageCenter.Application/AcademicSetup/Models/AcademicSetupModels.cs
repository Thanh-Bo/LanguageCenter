using System.ComponentModel.DataAnnotations;

namespace LanguageCenter.Application.AcademicSetup.Models;

// ============================================================================
// NỘI DUNG 20 BUỔI HỌC
// ============================================================================

/// <summary>Một buổi trong khung 20 buổi của khóa học.</summary>
public sealed class LessonContentDto
{
    public int MaNoiDung { get; init; }

    public int MaKhoaHoc { get; init; }

    /// <summary>Buổi thứ mấy (1 → 20).</summary>
    public int SoBuoi { get; init; }

    public string TieuDe { get; init; } = string.Empty;

    public string? NoiDung { get; init; }

    public string? MucTieu { get; init; }
}

/// <summary>Một dòng gửi lên khi lưu nội dung buổi học.</summary>
public sealed class LessonContentInput
{
    [Range(1, 20, ErrorMessage = "Số buổi phải từ 1 đến 20.")]
    public int SoBuoi { get; set; }

    [Required(ErrorMessage = "Tiêu đề buổi học là bắt buộc.")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Tiêu đề tối đa 200 ký tự.")]
    public string TieuDe { get; set; } = string.Empty;

    [StringLength(1000, ErrorMessage = "Nội dung tối đa 1000 ký tự.")]
    public string? NoiDung { get; set; }

    [StringLength(500, ErrorMessage = "Mục tiêu tối đa 500 ký tự.")]
    public string? MucTieu { get; set; }
}

/// <summary>Body của <c>PUT /api/admin/lesson-contents/{maKhoaHoc}</c> — lưu cả 20 buổi 1 lần.</summary>
public sealed class SaveLessonContentsRequest
{
    public List<LessonContentInput> Items { get; set; } = [];
}

// ============================================================================
// NGÀY LỄ
// ============================================================================

/// <summary>Một ngày lễ / ngày nghỉ của trung tâm.</summary>
public sealed class HolidayDto
{
    public int MaNgayLe { get; init; }

    public DateOnly Ngay { get; init; }

    public string TenNgayLe { get; init; } = string.Empty;

    public string? GhiChu { get; init; }
}

/// <summary>Body của <c>POST /api/admin/holidays</c>.</summary>
public sealed class CreateHolidayRequest
{
    [Required(ErrorMessage = "Ngày lễ là bắt buộc.")]
    public DateOnly Ngay { get; set; }

    [Required(ErrorMessage = "Tên ngày lễ là bắt buộc.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Tên ngày lễ tối đa 100 ký tự.")]
    public string TenNgayLe { get; set; } = string.Empty;

    [StringLength(200, ErrorMessage = "Ghi chú tối đa 200 ký tự.")]
    public string? GhiChu { get; set; }
}

/// <summary>Body của <c>PATCH /api/admin/holidays/{id}</c> — mọi field OPTIONAL.</summary>
public sealed class UpdateHolidayRequest
{
    public DateOnly? Ngay { get; set; }

    [StringLength(100, MinimumLength = 1, ErrorMessage = "Tên ngày lễ tối đa 100 ký tự.")]
    public string? TenNgayLe { get; set; }

    [StringLength(200, ErrorMessage = "Ghi chú tối đa 200 ký tự.")]
    public string? GhiChu { get; set; }
}
