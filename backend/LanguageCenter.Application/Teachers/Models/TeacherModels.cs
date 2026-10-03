using System.ComponentModel.DataAnnotations;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Teachers.Models;

// ============================================================================
// REQUEST
// ============================================================================

/// <summary>
/// Body của POST /api/admin/teachers (dùng CHUNG cho Giáo viên và Trợ giảng).
/// <para>
/// <c>LoaiGiaoVien</c> quyết định mã sinh ra và vai trò:
/// <list type="bullet">
///   <item>"Giáo viên" → mã <c>GVxxx</c>, role <c>Giáo viên</c></item>
///   <item>"Trợ giảng" → mã <c>TGxxx</c>, role <c>Trợ giảng</c></item>
/// </list>
/// KHÔNG nhận MaGiaoVien / TenDangNhap / MaVaiTro / MatKhauHash.
/// </para>
/// </summary>
public sealed class CreateTeacherRequest
{
    [Required(ErrorMessage = "Họ tên là bắt buộc.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2 đến 150 ký tự.")]
    public string HoTen { get; set; } = string.Empty;

    public DateOnly? NgaySinh { get; set; }

    [StringLength(20, ErrorMessage = "Giới tính tối đa 20 ký tự.")]
    public string? GioiTinh { get; set; }

    [Required(ErrorMessage = "Email là bắt buộc.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(255, ErrorMessage = "Email tối đa 255 ký tự.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự.")]
    public string? SoDienThoai { get; set; }

    [StringLength(500, ErrorMessage = "Địa chỉ tối đa 500 ký tự.")]
    public string? DiaChi { get; set; }

    /// <summary>BẮT BUỘC. Phải là "Giáo viên" hoặc "Trợ giảng" (khớp CHECK constraint trong database).</summary>
    [Required(ErrorMessage = "Loại giáo viên là bắt buộc.")]
    [StringLength(50)]
    public string LoaiGiaoVien { get; set; } = string.Empty;

    [StringLength(500, ErrorMessage = "Chuyên môn tối đa 500 ký tự.")]
    public string? ChuyenMon { get; set; }

    [Range(0, 80, ErrorMessage = "Số năm kinh nghiệm phải từ 0 đến 80.")]
    public int? SoNamKinhNghiem { get; set; }

    public DateOnly? NgayVaoLam { get; set; }
}

/// <summary>
/// Body của PATCH /api/admin/teachers/{teacherId}.
/// <para>
/// <b>LoaiGiaoVien KHÔNG nằm trong request này</b> — xem ghi chú trong tài liệu Phase 3:
/// mã giáo viên (<c>MaGiaoVien</c> = <c>TenDangNhap</c>) đã gắn prefix GV/TG, đổi loại sẽ
/// phải đổi khóa chính và mã đăng nhập, làm hỏng dữ liệu lịch sử.
/// </para>
/// </summary>
public sealed class UpdateTeacherRequest
{
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2 đến 150 ký tự.")]
    public string? HoTen { get; set; }

    public DateOnly? NgaySinh { get; set; }

    [StringLength(20)] public string? GioiTinh { get; set; }

    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(255)] public string? Email { get; set; }

    [StringLength(20)] public string? SoDienThoai { get; set; }

    [StringLength(500)] public string? DiaChi { get; set; }

    [StringLength(500)] public string? ChuyenMon { get; set; }

    [Range(0, 80, ErrorMessage = "Số năm kinh nghiệm phải từ 0 đến 80.")]
    public int? SoNamKinhNghiem { get; set; }

    public DateOnly? NgayVaoLam { get; set; }

    /// <summary>Phải thuộc <c>WorkStatuses.All</c>.</summary>
    [StringLength(50)] public string? TrangThai { get; set; }
}

/// <summary>Body của PATCH /api/teacher/profile — giáo viên/trợ giảng TỰ sửa.</summary>
/// <remarks>
/// KHÔNG có: MaGiaoVien, LoaiGiaoVien, ChuyenMon, SoNamKinhNghiem, NgayVaoLam, TrangThai,
/// Role, TrangThaiTaiKhoan — các field nghiệp vụ do Admin quản lý.
/// </remarks>
public sealed class UpdateMyTeacherProfileRequest
{
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2 đến 150 ký tự.")]
    public string? HoTen { get; set; }

    [StringLength(20)] public string? GioiTinh { get; set; }

    [StringLength(20)] public string? SoDienThoai { get; set; }

    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(255)] public string? Email { get; set; }

    [StringLength(500)] public string? DiaChi { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>Response của POST /api/admin/teachers — temporaryPassword CHỈ trả đúng MỘT LẦN.</summary>
public sealed class CreateTeacherResponse
{
    public string TeacherId { get; init; } = string.Empty;

    public string Username { get; init; } = string.Empty;

    public string TemporaryPassword { get; init; } = string.Empty;

    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string LoaiGiaoVien { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}

public sealed class BangCapDto
{
    public int MaBangCap { get; init; }

    public string TenBangCap { get; init; } = string.Empty;

    public string? ChuyenNganh { get; init; }

    public string? CoSoDaoTao { get; init; }

    public int? NamTotNghiep { get; init; }

    public string? SoBang { get; init; }

    public DateOnly? NgayCap { get; init; }
}

public sealed class ChungChiDto
{
    public int MaChungChi { get; init; }

    public string TenChungChi { get; init; } = string.Empty;

    public string? LoaiChungChi { get; init; }

    public string? DonViCap { get; init; }

    public string? KetQua { get; init; }

    public string? SoChungChi { get; init; }

    public DateOnly? NgayCap { get; init; }

    public DateOnly? NgayHetHan { get; init; }
}

/// <summary>Chi tiết giáo viên — dùng cho cả Admin detail và Teacher self-profile.</summary>
public sealed class TeacherDetailDto
{
    public string MaGiaoVien { get; init; } = string.Empty;

    public string HoTen { get; init; } = string.Empty;

    public DateOnly? NgaySinh { get; init; }

    public string? GioiTinh { get; init; }

    public string? SoDienThoai { get; init; }

    public string? Email { get; init; }

    public string? DiaChi { get; init; }

    public string LoaiGiaoVien { get; init; } = string.Empty;

    public string? ChuyenMon { get; init; }

    public int? SoNamKinhNghiem { get; init; }

    public DateOnly? NgayVaoLam { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    /// <summary>Chỉ đọc ở Phase 3 (chưa có CRUD Bằng cấp).</summary>
    public IReadOnlyList<BangCapDto> BangCaps { get; init; } = [];

    /// <summary>Chỉ đọc ở Phase 3 (chưa có CRUD Chứng chỉ).</summary>
    public IReadOnlyList<ChungChiDto> ChungChis { get; init; } = [];
}

/// <summary>Một dòng trong danh sách giáo viên (Admin).</summary>
public sealed class TeacherListItemDto
{
    public string MaGiaoVien { get; init; } = string.Empty;

    public string HoTen { get; init; } = string.Empty;

    public string LoaiGiaoVien { get; init; } = string.Empty;

    public string? Email { get; init; }

    public string? SoDienThoai { get; init; }

    public string? ChuyenMon { get; init; }

    public int? SoNamKinhNghiem { get; init; }

    public string TrangThai { get; init; } = string.Empty;
}

/// <summary>Tham số lọc danh sách giáo viên (Admin).</summary>
public sealed class TeacherListQuery : PagedQuery
{
    /// <summary>Tìm theo MaGiaoVien, HoTen hoặc Email.</summary>
    public string? Search { get; set; }

    /// <summary>Lọc theo LoaiGiaoVien ("Giáo viên" / "Trợ giảng").</summary>
    public string? LoaiGiaoVien { get; set; }
}
