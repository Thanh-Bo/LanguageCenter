using System.ComponentModel.DataAnnotations;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Employees.Models;

// ============================================================================
// REQUEST
// ============================================================================

/// <summary>Body của <c>POST /api/admin/employees</c>.</summary>
/// <remarks>
/// Client KHÔNG gửi <c>MaNhanVien</c> (backend sinh theo chức vụ) và KHÔNG gửi vai trò —
/// <see cref="ChucVu"/> quyết định vai trò.
/// </remarks>
public sealed class CreateEmployeeRequest
{
    [Required(ErrorMessage = "Họ tên là bắt buộc.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2 đến 150 ký tự.")]
    public string HoTen { get; set; } = string.Empty;

    public DateOnly? NgaySinh { get; set; }

    [StringLength(20, ErrorMessage = "Giới tính tối đa 20 ký tự.")]
    public string? GioiTinh { get; set; }

    [Required(ErrorMessage = "Email là bắt buộc.")]
    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [StringLength(255, ErrorMessage = "Email tối đa 255 ký tự.")]
    public string Email { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự.")]
    public string? SoDienThoai { get; set; }

    [StringLength(500, ErrorMessage = "Địa chỉ tối đa 500 ký tự.")]
    public string? DiaChi { get; set; }

    /// <summary>"Nhân viên học vụ" / "Nhân viên tư vấn" / "Nhân viên kế toán".</summary>
    [Required(ErrorMessage = "Chức vụ là bắt buộc.")]
    [StringLength(100)]
    public string ChucVu { get; set; } = string.Empty;

    public DateOnly? NgayVaoLam { get; set; }
}

/// <summary>Body của <c>PATCH /api/admin/employees/{maNhanVien}</c> — mọi field OPTIONAL.</summary>
/// <remarks>
/// <c>ChucVu</c> CỐ Ý không có ở đây: đổi chức vụ sẽ phải đổi vai trò và mã đăng nhập
/// → làm hỏng dữ liệu lịch sử (lớp phụ trách, giao dịch kế toán). Hãy tạo hồ sơ mới.
/// </remarks>
public sealed class UpdateEmployeeRequest
{
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2 đến 150 ký tự.")]
    public string? HoTen { get; set; }

    public DateOnly? NgaySinh { get; set; }

    [StringLength(20, ErrorMessage = "Giới tính tối đa 20 ký tự.")]
    public string? GioiTinh { get; set; }

    [EmailAddress(ErrorMessage = "Email không đúng định dạng.")]
    [StringLength(255, ErrorMessage = "Email tối đa 255 ký tự.")]
    public string? Email { get; set; }

    [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự.")]
    public string? SoDienThoai { get; set; }

    [StringLength(500, ErrorMessage = "Địa chỉ tối đa 500 ký tự.")]
    public string? DiaChi { get; set; }

    public DateOnly? NgayVaoLam { get; set; }

    /// <summary>"Đang làm việc" / "Đã nghỉ việc".</summary>
    [StringLength(50)]
    public string? TrangThaiLamViec { get; set; }
}

/// <summary>Tham số lọc danh sách nhân viên.</summary>
public sealed class EmployeeListQuery : PagedQuery
{
    /// <summary>Tìm theo mã, họ tên, email hoặc số điện thoại.</summary>
    public string? Search { get; set; }

    /// <summary>Lọc theo chức vụ — mỗi tab WinForms truyền đúng 1 giá trị.</summary>
    public string? ChucVu { get; set; }

    /// <summary>"Đang làm việc" / "Đã nghỉ việc".</summary>
    public string? TrangThai { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>Một dòng trong danh sách nhân viên.</summary>
public sealed class EmployeeListItemDto
{
    public string MaNhanVien { get; init; } = string.Empty;

    public string HoTen { get; init; } = string.Empty;

    public string ChucVu { get; init; } = string.Empty;

    public string? SoDienThoai { get; init; }

    public string? Email { get; init; }

    public DateOnly? NgayVaoLam { get; init; }

    public string TrangThaiLamViec { get; init; } = string.Empty;
}

/// <summary>Chi tiết nhân viên (kèm thông tin tài khoản đăng nhập).</summary>
public sealed class EmployeeDetailDto
{
    public string MaNhanVien { get; init; } = string.Empty;

    public string HoTen { get; init; } = string.Empty;

    public DateOnly? NgaySinh { get; init; }

    public string? GioiTinh { get; init; }

    public string? SoDienThoai { get; init; }

    public string? Email { get; init; }

    public string? DiaChi { get; init; }

    public string ChucVu { get; init; } = string.Empty;

    public DateOnly? NgayVaoLam { get; init; }

    public string TrangThaiLamViec { get; init; } = string.Empty;

    public string TenDangNhap { get; init; } = string.Empty;

    public string TrangThaiTaiKhoan { get; init; } = string.Empty;

    public string TenVaiTro { get; init; } = string.Empty;
}

/// <summary>Kết quả tạo nhân viên — mật khẩu tạm chỉ trả về ĐÚNG MỘT LẦN.</summary>
public sealed class CreateEmployeeResponse
{
    public string EmployeeId { get; init; } = string.Empty;

    public string Username { get; init; } = string.Empty;

    public string TemporaryPassword { get; init; } = string.Empty;

    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string ChucVu { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}
