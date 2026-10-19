using System.ComponentModel.DataAnnotations;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Promotions.Models;

// ============================================================================
// REQUEST
// ============================================================================

/// <summary>
/// Body của <c>POST /api/admin/promotions</c>.
/// <para>
/// Cho phép tạo ưu đãi với <c>soKhoaHocToiThieu &gt; 1</c> để LƯU CHÍNH SÁCH,
/// nhưng hệ thống sẽ TỪ CHỐI áp dụng khi tạo đăng ký vì schema không có khóa nhóm đăng ký.
/// </para>
/// </summary>
public sealed class CreatePromotionRequest
{
    [Required(ErrorMessage = "Tên ưu đãi là bắt buộc.")]
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Tên ưu đãi phải từ 2 đến 200 ký tự.")]
    public string TenUuDai { get; set; } = string.Empty;

    [Required(ErrorMessage = "Loại giảm giá là bắt buộc.")]
    [StringLength(50)]
    public string LoaiGiamGia { get; set; } = string.Empty;

    [Range(0.01, 1000000000000, ErrorMessage = "Giá trị giảm phải lớn hơn 0.")]
    public decimal GiaTriGiam { get; set; }

    [Range(1, 100, ErrorMessage = "Số khóa học tối thiểu phải từ 1 đến 100.")]
    public int SoKhoaHocToiThieu { get; set; } = 1;

    [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc.")]
    public DateOnly NgayBatDau { get; set; }

    [Required(ErrorMessage = "Ngày kết thúc là bắt buộc.")]
    public DateOnly NgayKetThuc { get; set; }

    [StringLength(500, ErrorMessage = "Poster tối đa 500 ký tự.")]
    public string? Poster { get; set; }

    [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự.")]
    public string? MoTa { get; set; }
}

/// <summary>Body của <c>PATCH /api/admin/promotions/{id}</c> — mọi field OPTIONAL.</summary>
public sealed class UpdatePromotionRequest
{
    [StringLength(200, MinimumLength = 2, ErrorMessage = "Tên ưu đãi phải từ 2 đến 200 ký tự.")]
    public string? TenUuDai { get; set; }

    [StringLength(50)]
    public string? LoaiGiamGia { get; set; }

    [Range(0.01, 1000000000000, ErrorMessage = "Giá trị giảm phải lớn hơn 0.")]
    public decimal? GiaTriGiam { get; set; }

    [Range(1, 100, ErrorMessage = "Số khóa học tối thiểu phải từ 1 đến 100.")]
    public int? SoKhoaHocToiThieu { get; set; }

    public DateOnly? NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    [StringLength(500, ErrorMessage = "Poster tối đa 500 ký tự.")]
    public string? Poster { get; set; }

    [StringLength(1000, ErrorMessage = "Mô tả tối đa 1000 ký tự.")]
    public string? MoTa { get; set; }
}

/// <summary>Tham số lọc danh sách ưu đãi.</summary>
public sealed class PromotionListQuery : PagedQuery
{
    /// <summary>Tìm theo tên ưu đãi.</summary>
    public string? Search { get; set; }

    /// <summary>"Hoạt động" / "Ngừng hoạt động".</summary>
    public string? TrangThai { get; set; }

    /// <summary><c>true</c> = chỉ lấy ưu đãi đang trong thời hạn và đang hoạt động.</summary>
    public bool? ConHieuLuc { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>Một dòng trong danh sách ưu đãi.</summary>
public sealed class PromotionListItemDto
{
    public int MaUuDai { get; init; }

    public string TenUuDai { get; init; } = string.Empty;

    public string LoaiGiamGia { get; init; } = string.Empty;

    public decimal GiaTriGiam { get; init; }

    public int SoKhoaHocToiThieu { get; init; }

    public DateOnly NgayBatDau { get; init; }

    public DateOnly NgayKetThuc { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    /// <summary>Đang "Hoạt động" VÀ hôm nay nằm trong khoảng [NgayBatDau, NgayKetThuc].</summary>
    public bool ConHieuLuc { get; init; }
}

/// <summary>Chi tiết ưu đãi.</summary>
public sealed class PromotionDetailDto
{
    public int MaUuDai { get; init; }

    public string TenUuDai { get; init; } = string.Empty;

    public string LoaiGiamGia { get; init; } = string.Empty;

    public decimal GiaTriGiam { get; init; }

    public int SoKhoaHocToiThieu { get; init; }

    public DateOnly NgayBatDau { get; init; }

    public DateOnly NgayKetThuc { get; init; }

    public string? Poster { get; init; }

    public string? MoTa { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    public bool ConHieuLuc { get; init; }

    /// <summary>Số đăng ký đã áp dụng ưu đãi này.</summary>
    public int SoLanApDung { get; init; }
}
