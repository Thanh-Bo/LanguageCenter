using LanguageCenter.Application.Common;
using System.ComponentModel.DataAnnotations;

namespace LanguageCenter.Application.Rooms.Models;

/// <summary>Tham số lọc danh sách phòng học.</summary>
public sealed class RoomListQuery : PagedQuery
{
    /// <summary>Tìm theo mã phòng hoặc tên phòng.</summary>
    public string? Search { get; set; }

    /// <summary>Lọc theo trạng thái phòng.</summary>
    public string? TrangThai { get; set; }
}

/// <summary>Một dòng trong danh sách phòng học (chỉ đọc).</summary>
public sealed class RoomListItemDto
{
    public int MaPhongHoc { get; init; }

    public string MaPhong { get; init; } = string.Empty;

    public string TenPhong { get; init; } = string.Empty;

    public string? ViTri { get; init; }

    public int SucChua { get; init; }

    public string? ThietBi { get; init; }

    public string TrangThai { get; init; } = string.Empty;
}

/// <summary>Body của <c>POST /api/admin/rooms</c> — tạo phòng học mới.</summary>
public sealed class CreateRoomRequest
{
    /// <summary>Mã phòng — UNIQUE, ví dụ "P101". Tự động chuyển chữ HOA.</summary>
    [Required(ErrorMessage = "Mã phòng là bắt buộc.")]
    [StringLength(30, ErrorMessage = "Mã phòng tối đa 30 ký tự.")]
    public string MaPhong { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tên phòng là bắt buộc.")]
    [StringLength(100, MinimumLength = 1, ErrorMessage = "Tên phòng tối đa 100 ký tự.")]
    public string TenPhong { get; set; } = string.Empty;

    [StringLength(255)]
    public string? ViTri { get; set; }

    [Range(1, 1000, ErrorMessage = "Sức chứa phải từ 1 đến 1000.")]
    public int SucChua { get; set; }

    [StringLength(1000)]
    public string? ThietBi { get; set; }

    /// <summary>Mặc định "Hoạt động" nếu để trống.</summary>
    [StringLength(50)]
    public string? TrangThai { get; set; }
}
