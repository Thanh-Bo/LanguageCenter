using System.ComponentModel.DataAnnotations;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Schedules.Models;

// ============================================================================
// REQUEST
// ============================================================================

/// <summary>
/// Body của <c>POST /api/admin/schedules</c>.
/// <para>
/// Lịch định kỳ KHÔNG phải một buổi học thực tế — điểm danh chỉ ghi trên <c>CaHoc</c>.
/// <c>ThuTrongTuan</c>: 1 = Thứ Hai … 7 = Chủ Nhật.
/// </para>
/// </summary>
public sealed class CreateScheduleRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Lớp học là bắt buộc.")]
    public int MaLopHoc { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Phòng học là bắt buộc.")]
    public int MaPhongHoc { get; set; }

    [Range(1, 7, ErrorMessage = "Thứ trong tuần phải từ 1 (Thứ Hai) đến 7 (Chủ Nhật).")]
    public int ThuTrongTuan { get; set; }

    [Required(ErrorMessage = "Giờ bắt đầu là bắt buộc.")]
    public TimeOnly GioBatDau { get; set; }

    [Required(ErrorMessage = "Giờ kết thúc là bắt buộc.")]
    public TimeOnly GioKetThuc { get; set; }

    [Required(ErrorMessage = "Ngày áp dụng từ là bắt buộc.")]
    public DateOnly NgayApDungTu { get; set; }

    /// <summary>Để trống = áp dụng không giới hạn.</summary>
    public DateOnly? NgayApDungDen { get; set; }
}

/// <summary>
/// Body của <c>PATCH /api/admin/schedules/{id}</c> — mọi field OPTIONAL.
/// <para>KHÔNG có <c>MaLopHoc</c>: muốn đổi lớp thì tạo lịch mới.</para>
/// </summary>
public sealed class UpdateScheduleRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Phòng học không hợp lệ.")]
    public int? MaPhongHoc { get; set; }

    [Range(1, 7, ErrorMessage = "Thứ trong tuần phải từ 1 (Thứ Hai) đến 7 (Chủ Nhật).")]
    public int? ThuTrongTuan { get; set; }

    public TimeOnly? GioBatDau { get; set; }

    public TimeOnly? GioKetThuc { get; set; }

    public DateOnly? NgayApDungTu { get; set; }

    public DateOnly? NgayApDungDen { get; set; }
}

/// <summary>Tham số lọc danh sách lịch học.</summary>
public sealed class ScheduleListQuery : PagedQuery
{
    public int? MaLopHoc { get; set; }

    public int? MaPhongHoc { get; set; }

    /// <summary>1 = Thứ Hai … 7 = Chủ Nhật.</summary>
    public int? ThuTrongTuan { get; set; }

    /// <summary>"Hoạt động" / "Ngừng áp dụng".</summary>
    public string? TrangThai { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>Một dòng trong danh sách lịch học định kỳ.</summary>
public sealed class ScheduleListItemDto
{
    public int MaLichHoc { get; init; }

    public int MaLopHoc { get; init; }

    public string MaLop { get; init; } = string.Empty;

    public string TenLop { get; init; } = string.Empty;

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    public int MaPhongHoc { get; init; }

    public string MaPhong { get; init; } = string.Empty;

    public string TenPhong { get; init; } = string.Empty;

    /// <summary>1 = Thứ Hai … 7 = Chủ Nhật.</summary>
    public int ThuTrongTuan { get; init; }

    public TimeOnly GioBatDau { get; init; }

    public TimeOnly GioKetThuc { get; init; }

    public DateOnly NgayApDungTu { get; init; }

    public DateOnly? NgayApDungDen { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    public int SoCaHoc { get; init; }
}

/// <summary>Chi tiết lịch học định kỳ.</summary>
public sealed class ScheduleDetailDto
{
    public int MaLichHoc { get; init; }

    public int MaLopHoc { get; init; }

    public string MaLop { get; init; } = string.Empty;

    public string TenLop { get; init; } = string.Empty;

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    public int MaPhongHoc { get; init; }

    public string MaPhong { get; init; } = string.Empty;

    public string TenPhong { get; init; } = string.Empty;

    public int ThuTrongTuan { get; init; }

    public TimeOnly GioBatDau { get; init; }

    public TimeOnly GioKetThuc { get; init; }

    public DateOnly NgayApDungTu { get; init; }

    public DateOnly? NgayApDungDen { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    public int SoCaHoc { get; init; }
}
