using System.ComponentModel.DataAnnotations;

namespace LanguageCenter.Application.Attendance.Models;

// ============================================================================
// ACTOR — người đang thực hiện (lấy từ JWT)
// ============================================================================

/// <summary>
/// Danh tính người thực hiện, do controller dựng từ JWT.
/// <para>
/// <c>TenDangNhap</c> của giáo viên/trợ giảng trùng với mã giáo viên
/// (<c>GiaoVien.MaGiaoVien</c>) theo quy ước của hệ thống.
/// </para>
/// </summary>
public sealed record AttendanceActor(int MaTaiKhoan, string TenDangNhap, bool IsStaff);

// ============================================================================
// REQUEST
// ============================================================================

/// <summary>Body của <c>POST /api/admin/sessions/{id}/attendance</c> — ghi hàng loạt.</summary>
public sealed class SaveAttendanceRequest
{
    [Required(ErrorMessage = "Danh sách điểm danh là bắt buộc.")]
    [MinLength(1, ErrorMessage = "Cần ít nhất một bản ghi điểm danh.")]
    public List<AttendanceItemRequest> Items { get; set; } = [];
}

/// <summary>Một dòng điểm danh cần ghi.</summary>
public sealed class AttendanceItemRequest
{
    [Required(ErrorMessage = "Mã học viên là bắt buộc.")]
    [StringLength(20, MinimumLength = 1)]
    public string MaHocVien { get; set; } = string.Empty;

    /// <summary>"Có mặt" / "Vắng" / "Vắng có phép" / "Đi trễ".</summary>
    [Required(ErrorMessage = "Trạng thái điểm danh là bắt buộc.")]
    public string TrangThaiDiemDanh { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự.")]
    public string? GhiChu { get; set; }
}

/// <summary>Body của <c>PATCH /api/admin/attendance/{maDiemDanh}</c>.</summary>
public sealed class UpdateAttendanceRequest
{
    [Required(ErrorMessage = "Trạng thái điểm danh là bắt buộc.")]
    public string TrangThaiDiemDanh { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Ghi chú tối đa 500 ký tự.")]
    public string? GhiChu { get; set; }
}

/// <summary>Tham số lọc lịch sử điểm danh của học viên.</summary>
public sealed class StudentAttendanceQuery
{
    public DateOnly? TuNgay { get; set; }

    public DateOnly? DenNgay { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>Bảng điểm danh của một ca học.</summary>
public sealed class SessionAttendanceSheetDto
{
    public int MaCaHoc { get; init; }

    public int MaLopHoc { get; init; }

    public string MaLop { get; init; } = string.Empty;

    public string TenLop { get; init; } = string.Empty;

    public int MaPhongHoc { get; init; }

    public string MaPhong { get; init; } = string.Empty;

    public string TenPhong { get; init; } = string.Empty;

    public DateOnly NgayHoc { get; init; }

    public TimeOnly GioBatDau { get; init; }

    public TimeOnly GioKetThuc { get; init; }

    public string TrangThaiCaHoc { get; init; } = string.Empty;

    /// <summary>Có được phép điểm danh ca này hay không (kết hợp trạng thái ca và quyền của người gọi).</summary>
    public bool CoTheDiemDanh { get; set; }

    public string? LyDoKhongTheDiemDanh { get; set; }

    public IReadOnlyList<AttendanceRowDto> HocViens { get; set; } = [];
}

/// <summary>Một học viên trong bảng điểm danh.</summary>
public sealed class AttendanceRowDto
{
    public string MaHocVien { get; init; } = string.Empty;

    public string HoTen { get; init; } = string.Empty;

    public int MaDangKy { get; init; }

    public bool DaDiemDanh { get; init; }

    public int? MaDiemDanh { get; init; }

    public string? TrangThaiDiemDanh { get; init; }

    public string? GhiChu { get; init; }

    public DateTime? ThoiGianDiemDanh { get; init; }

    public string? TenTaiKhoanThucHien { get; init; }
}

/// <summary>Lịch sử điểm danh + thống kê của một học viên.</summary>
public sealed class StudentAttendanceHistoryDto
{
    public string MaHocVien { get; init; } = string.Empty;

    public string HoTen { get; init; } = string.Empty;

    public int TongCaHoc { get; init; }

    public int CoMat { get; init; }

    public int Vang { get; init; }

    public int VangCoPhep { get; init; }

    public int DiTre { get; init; }

    /// <summary>Tỉ lệ có mặt trên tổng số ca đã điểm danh (0..100, làm tròn 2 chữ số).</summary>
    public decimal TiLeCoMat { get; init; }

    public IReadOnlyList<StudentAttendanceItemDto> Items { get; init; } = [];
}

/// <summary>Một lượt điểm danh của học viên.</summary>
public sealed class StudentAttendanceItemDto
{
    public int MaDiemDanh { get; init; }

    public int MaCaHoc { get; init; }

    public string MaLop { get; init; } = string.Empty;

    public string TenLop { get; init; } = string.Empty;

    public string TenKhoaHoc { get; init; } = string.Empty;

    public DateOnly NgayHoc { get; init; }

    public TimeOnly GioBatDau { get; init; }

    public TimeOnly GioKetThuc { get; init; }

    public string TrangThaiDiemDanh { get; init; } = string.Empty;

    public string? GhiChu { get; init; }

    public DateTime ThoiGianDiemDanh { get; init; }

    public string TrangThaiCaHoc { get; init; } = string.Empty;
}
