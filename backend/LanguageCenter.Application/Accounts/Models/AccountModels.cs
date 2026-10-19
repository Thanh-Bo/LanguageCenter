using System.ComponentModel.DataAnnotations;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Accounts.Models;

/// <summary>Một dòng trong danh sách tài khoản. KHÔNG chứa MatKhauHash.</summary>
public sealed class AccountListItemDto
{
    /// <summary>MaTaiKhoan.</summary>
    public int Id { get; init; }

    /// <summary>TenDangNhap (= mã nghiệp vụ).</summary>
    public string Username { get; init; } = string.Empty;

    /// <summary>HoTen lấy từ hồ sơ tương ứng (NhanVien / GiaoVien / HocVien).</summary>
    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    /// <summary>TenVaiTro — nguyên tiếng Việt như trong database.</summary>
    public string Role { get; init; } = string.Empty;

    /// <summary>
    /// Loại hồ sơ thực tế đang gắn với tài khoản: <c>NhanVien</c> / <c>GiaoVien</c> / <c>HocVien</c>.
    /// <para>
    /// <c>null</c> = tài khoản <b>chưa có hồ sơ</b> (ví dụ tạo bằng <c>POST /admin/accounts</c> cho
    /// vai trò Học viên/Giáo viên). Khi đó <see cref="FullName"/> chỉ là bản sao của tên đăng nhập.
    /// </para>
    /// </summary>
    public string? ProfileType { get; init; }

    /// <summary>TrangThaiTaiKhoan.</summary>
    public string Status { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public DateTime? LastLoginAt { get; init; }
}

/// <summary>Chi tiết một tài khoản. KHÔNG chứa MatKhauHash.</summary>
public sealed class AccountDetailDto
{
    public int Id { get; init; }

    public string Username { get; init; } = string.Empty;

    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;

    public int RoleId { get; init; }

    public string Status { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }

    public DateTime? LastLoginAt { get; init; }

    /// <summary>Loại hồ sơ tương ứng: <c>NhanVien</c> / <c>GiaoVien</c> / <c>HocVien</c> / null.</summary>
    public string? ProfileType { get; init; }

    /// <summary>Mã hồ sơ: MaNhanVien / MaGiaoVien / MaHocVien.</summary>
    public string? ProfileCode { get; init; }

    /// <summary>Loại giáo viên nếu là GiaoVien: "Giáo viên" / "Trợ giảng".</summary>
    public string? TeacherType { get; init; }

    /// <summary>Trạng thái học tập nếu là HocVien.</summary>
    public string? StudentStatus { get; init; }
}

/// <summary>Tham số lọc danh sách tài khoản. Hỗ trợ phân trang nhờ kế thừa <see cref="PagedQuery"/>.</summary>
public sealed class AccountListQuery : PagedQuery
{
    /// <summary>Tìm theo TenDangNhap, Email hoặc HoTen (không phân biệt hoa thường).</summary>
    public string? Search { get; set; }

    /// <summary>Lọc theo TenVaiTro (khớp chính xác, ví dụ "Học viên").</summary>
    public string? Role { get; set; }

    /// <summary>Lọc theo TrangThaiTaiKhoan ("Hoạt động" / "Bị khóa" / "Ngừng sử dụng").</summary>
    public string? Status { get; set; }
}

/// <summary>Body của PATCH /api/admin/accounts/{id}/status.</summary>
public sealed class UpdateAccountStatusRequest
{
    [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Body của <c>POST /api/admin/accounts</c> — tạo tài khoản KHÔNG kèm hồ sơ.
/// <para>
/// Dùng cho các vai trò không có bảng hồ sơ riêng (Nhân viên học vụ / tư vấn / kế toán).
/// Học viên và Giáo viên nên tạo qua <c>/admin/students</c> và <c>/admin/teachers</c>
/// để sinh kèm hồ sơ + mật khẩu tạm.
/// </para>
/// </summary>
public sealed class CreateAccountRequest
{
    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "Tên đăng nhập tối đa 20 ký tự.")]
    public string TenDangNhap { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email là bắt buộc.")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự.")]
    public string MatKhau { get; set; } = string.Empty;

    /// <summary>Tên vai trò tiếng Việt — ví dụ "Nhân viên học vụ".</summary>
    [Required(ErrorMessage = "Vai trò là bắt buộc.")]
    [StringLength(100)]
    public string VaiTro { get; set; } = string.Empty;

    /// <summary>Mặc định "Hoạt động" nếu để trống.</summary>
    [StringLength(50)]
    public string? TrangThai { get; set; }
}

/// <summary>Body của <c>PATCH /api/admin/accounts/{id}</c> — mọi field OPTIONAL.</summary>
public sealed class UpdateAccountRequest
{
    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(255)]
    public string? Email { get; set; }

    /// <summary>Để trống nếu không đổi mật khẩu.</summary>
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự.")]
    public string? MatKhau { get; set; }

    [StringLength(100)]
    public string? VaiTro { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }
}

/// <summary>Response của POST /api/admin/accounts/{id}/reset-password.</summary>
public sealed class ResetPasswordResponse
{
    public string Username { get; init; } = string.Empty;

    /// <summary>CHỈ trả về đúng một lần tại thời điểm reset. Không lưu plain text.</summary>
    public string TemporaryPassword { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}

/// <summary>Response khi không thể xóa tài khoản (409 Conflict).</summary>
public sealed class AccountDeleteBlockedResponse
{
    public string Message { get; init; } = string.Empty;

    public string Reason { get; init; } = string.Empty;

    /// <summary>Gợi ý thay thế an toàn.</summary>
    public string Suggestion { get; init; } = string.Empty;
}
