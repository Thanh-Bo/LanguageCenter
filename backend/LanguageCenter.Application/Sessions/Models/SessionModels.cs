using System.ComponentModel.DataAnnotations;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Sessions.Models;

// ============================================================================
// REQUEST
// ============================================================================

/// <summary>
/// Body của <c>POST /api/admin/sessions</c>.
/// <para>
/// <c>MaLichHoc</c> để trống nếu là ca phát sinh ngoài lịch định kỳ.
/// <c>LyDoThayDoi</c> chỉ điền khi ca có thay đổi so với kế hoạch.
/// </para>
/// </summary>
public sealed class CreateSessionRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Lớp học là bắt buộc.")]
    public int MaLopHoc { get; set; }

    /// <summary>Ca phát sinh ngoài lịch định kỳ thì để trống.</summary>
    public int? MaLichHoc { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Phòng học là bắt buộc.")]
    public int MaPhongHoc { get; set; }

    [StringLength(20, MinimumLength = 1)]
    public string? MaGiaoVienThucTe { get; set; }

    [StringLength(20, MinimumLength = 1)]
    public string? MaTroGiangThucTe { get; set; }

    [Required(ErrorMessage = "Ngày học là bắt buộc.")]
    public DateOnly NgayHoc { get; set; }

    [Required(ErrorMessage = "Giờ bắt đầu là bắt buộc.")]
    public TimeOnly GioBatDau { get; set; }

    [Required(ErrorMessage = "Giờ kết thúc là bắt buộc.")]
    public TimeOnly GioKetThuc { get; set; }

    public string? NoiDungBaiHoc { get; set; }

    [StringLength(500, ErrorMessage = "Lý do thay đổi tối đa 500 ký tự.")]
    public string? LyDoThayDoi { get; set; }
}

/// <summary>
/// Body của <c>PATCH /api/admin/sessions/{id}</c> — mọi field OPTIONAL.
/// <para>KHÔNG có <c>MaLopHoc</c> và <c>MaLichHoc</c>.</para>
/// </summary>
public sealed class UpdateSessionRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Phòng học không hợp lệ.")]
    public int? MaPhongHoc { get; set; }

    [StringLength(20, MinimumLength = 1)]
    public string? MaGiaoVienThucTe { get; set; }

    [StringLength(20, MinimumLength = 1)]
    public string? MaTroGiangThucTe { get; set; }

    public DateOnly? NgayHoc { get; set; }

    public TimeOnly? GioBatDau { get; set; }

    public TimeOnly? GioKetThuc { get; set; }

    public string? NoiDungBaiHoc { get; set; }

    [StringLength(500, ErrorMessage = "Lý do thay đổi tối đa 500 ký tự.")]
    public string? LyDoThayDoi { get; set; }
}

/// <summary>Body của <c>POST /api/admin/sessions/generate</c>.</summary>
public sealed class GenerateSessionsRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Lịch học là bắt buộc.")]
    public int MaLichHoc { get; set; }

    [Required(ErrorMessage = "Từ ngày là bắt buộc.")]
    public DateOnly TuNgay { get; set; }

    [Required(ErrorMessage = "Đến ngày là bắt buộc.")]
    public DateOnly DenNgay { get; set; }
}

/// <summary>Tham số lọc danh sách ca học.</summary>
public sealed class SessionListQuery : PagedQuery
{
    public int? MaLopHoc { get; set; }

    public int? MaPhongHoc { get; set; }

    /// <summary>Lọc ca học có giáo viên hoặc trợ giảng là mã này.</summary>
    public string? MaGiaoVien { get; set; }

    /// <summary>"Chưa diễn ra" / "Đang diễn ra" / "Đã hoàn thành" / "Đã hủy".</summary>
    public string? TrangThai { get; set; }

    public DateOnly? TuNgay { get; set; }

    public DateOnly? DenNgay { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>Một dòng trong danh sách ca học.</summary>
public sealed class SessionListItemDto
{
    public int MaCaHoc { get; init; }

    public int MaLopHoc { get; init; }

    public string MaLop { get; init; } = string.Empty;

    public string TenLop { get; init; } = string.Empty;

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    public int? MaLichHoc { get; init; }

    public int MaPhongHoc { get; init; }

    public string MaPhong { get; init; } = string.Empty;

    public string TenPhong { get; init; } = string.Empty;

    public string? MaGiaoVienThucTe { get; init; }

    public string? TenGiaoVienThucTe { get; init; }

    public string? MaTroGiangThucTe { get; init; }

    public string? TenTroGiangThucTe { get; init; }

    public DateOnly NgayHoc { get; init; }

    public TimeOnly GioBatDau { get; init; }

    public TimeOnly GioKetThuc { get; init; }

    public string? NoiDungBaiHoc { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    public string? LyDoThayDoi { get; init; }

    public int SoDiemDanh { get; init; }
}

/// <summary>Chi tiết ca học.</summary>
public sealed class SessionDetailDto
{
    public int MaCaHoc { get; init; }

    public int MaLopHoc { get; init; }

    public string MaLop { get; init; } = string.Empty;

    public string TenLop { get; init; } = string.Empty;

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    public int? MaLichHoc { get; init; }

    public int MaPhongHoc { get; init; }

    public string MaPhong { get; init; } = string.Empty;

    public string TenPhong { get; init; } = string.Empty;

    public string? MaGiaoVienThucTe { get; init; }

    public string? TenGiaoVienThucTe { get; init; }

    public string? MaTroGiangThucTe { get; init; }

    public string? TenTroGiangThucTe { get; init; }

    public DateOnly NgayHoc { get; init; }

    public TimeOnly GioBatDau { get; init; }

    public TimeOnly GioKetThuc { get; init; }

    public string? NoiDungBaiHoc { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    public string? LyDoThayDoi { get; init; }

    public int SoDiemDanh { get; init; }
}

/// <summary>Kết quả tạo ca học hàng loạt từ lịch định kỳ.</summary>
public sealed class GenerateSessionsResponse
{
    public int MaLichHoc { get; init; }

    public DateOnly TuNgay { get; init; }

    public DateOnly DenNgay { get; init; }

    /// <summary>Ngày bắt đầu thực tế sau khi giao với khoảng áp dụng của lịch.</summary>
    public DateOnly? ApDungTuThucTe { get; init; }

    /// <summary>Ngày kết thúc thực tế sau khi giao với khoảng áp dụng của lịch.</summary>
    public DateOnly? ApDungDenThucTe { get; init; }

    /// <summary>Số ngày khớp thứ trong tuần của lịch.</summary>
    public int TongNgayKhop { get; init; }

    public IReadOnlyList<DateOnly> NgayTao { get; init; } = [];

    public IReadOnlyList<DateOnly> NgayDaCo { get; init; } = [];

    public IReadOnlyList<GenerateConflictDto> NgayXungDot { get; init; } = [];
}

/// <summary>Ngày không tạo được ca học và lý do.</summary>
public sealed class GenerateConflictDto
{
    public DateOnly NgayHoc { get; init; }

    public string LyDo { get; init; } = string.Empty;
}

/// <summary>Lớp mà giáo viên/trợ giảng đang được phân công giảng dạy.</summary>
public sealed class TeacherClassDto
{
    public int MaLopHoc { get; init; }

    public string MaLop { get; init; } = string.Empty;

    public string TenLop { get; init; } = string.Empty;

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    /// <summary>Số học viên đang học (Đã xếp lớp / Hoàn thành).</summary>
    public int SiSo { get; init; }

    public int SiSoToiDa { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    public DateOnly NgayBatDau { get; init; }

    public DateOnly NgayKetThucDuKien { get; init; }

    /// <summary>Giáo viên chính / Giáo viên thay thế / Trợ giảng.</summary>
    public string VaiTroPhanCong { get; init; } = string.Empty;
}

/// <summary>Học viên thuộc một lớp (dùng cho giáo viên).</summary>
public sealed class TeacherClassStudentDto
{
    public string MaHocVien { get; init; } = string.Empty;

    public string HoTen { get; init; } = string.Empty;

    public int MaDangKy { get; init; }
}
