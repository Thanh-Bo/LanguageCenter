using System.ComponentModel.DataAnnotations;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Students.Models;

// ============================================================================
// REQUEST
// ============================================================================

/// <summary>
/// Body của POST /api/admin/students.
/// <para>
/// KHÔNG nhận từ client: MaHocVien, TenDangNhap, MaVaiTro, Role, MatKhauHash,
/// TrangThaiTaiKhoan, TrangThaiHocTap, NgayTaoHoSo — backend tự xác định.
/// </para>
/// </summary>
public sealed class CreateStudentRequest
{
    [Required(ErrorMessage = "Họ tên là bắt buộc.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2 đến 150 ký tự.")]
    public string HoTen { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ngày sinh là bắt buộc.")]
    public DateOnly NgaySinh { get; set; }

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

    [StringLength(1000, ErrorMessage = "Mục tiêu học tập tối đa 1000 ký tự.")]
    public string? MucTieuHocTap { get; set; }

    [StringLength(200, ErrorMessage = "Trình độ hiện tại tối đa 200 ký tự.")]
    public string? TrinhDoHienTai { get; set; }

    /// <summary>Phụ huynh — OPTIONAL.</summary>
    public CreateParentRequest? PhuHuynh { get; set; }
}

/// <summary>
/// Thông tin phụ huynh khi tạo học viên.
/// <para>
/// <b>MoiQuanHe là BẮT BUỘC khi có phụ huynh</b> vì database có CHECK
/// <c>CK_HocVien_PhuHuynh</c>: MaPhuHuynh và MoiQuanHePhuHuynh phải cùng NULL hoặc cùng NOT NULL.
/// </para>
/// </summary>
public sealed class CreateParentRequest
{
    [Required(ErrorMessage = "Họ tên phụ huynh là bắt buộc.")]
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Họ tên phụ huynh phải từ 2 đến 150 ký tự.")]
    public string HoTen { get; set; } = string.Empty;

    [StringLength(20, ErrorMessage = "Số điện thoại tối đa 20 ký tự.")]
    public string? SoDienThoai { get; set; }

    [EmailAddress(ErrorMessage = "Email phụ huynh không hợp lệ.")]
    [StringLength(255, ErrorMessage = "Email tối đa 255 ký tự.")]
    public string? Email { get; set; }

    [StringLength(500, ErrorMessage = "Địa chỉ tối đa 500 ký tự.")]
    public string? DiaChi { get; set; }

    [StringLength(200, ErrorMessage = "Nghề nghiệp tối đa 200 ký tự.")]
    public string? NgheNghiep { get; set; }

    /// <summary>Mối quan hệ với học viên — ví dụ: Cha, Mẹ, Người giám hộ.</summary>
    [Required(ErrorMessage = "Mối quan hệ với học viên là bắt buộc khi có phụ huynh.")]
    [StringLength(50, ErrorMessage = "Mối quan hệ tối đa 50 ký tự.")]
    public string MoiQuanHe { get; set; } = string.Empty;
}

/// <summary>
/// Body của PATCH /api/admin/students/{studentId} — mọi field đều OPTIONAL.
/// <para>null = không thay đổi. Chuỗi rỗng = xóa (đưa về NULL) với các field được phép null.</para>
/// </summary>
public sealed class UpdateStudentRequest
{
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2 đến 150 ký tự.")]
    public string? HoTen { get; set; }

    public DateOnly? NgaySinh { get; set; }

    [StringLength(20)] public string? GioiTinh { get; set; }

    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(255)] public string? Email { get; set; }

    [StringLength(20)] public string? SoDienThoai { get; set; }

    [StringLength(500)] public string? DiaChi { get; set; }

    [StringLength(1000)] public string? MucTieuHocTap { get; set; }

    [StringLength(200)] public string? TrinhDoHienTai { get; set; }

    /// <summary>Admin only. Phải thuộc <c>StudentStatuses.All</c>.</summary>
    [StringLength(50)] public string? TrangThaiHocTap { get; set; }

    /// <summary>Admin only.</summary>
    public DateOnly? NgayNhapHoc { get; set; }
}

/// <summary>
/// Body của PATCH /api/student/profile — học viên TỰ sửa.
/// <para>
/// KHÔNG có TrangThaiHocTap / NgayNhapHoc / NgayTaoHoSo / MaHocVien / Email tài khoản khác.
/// NgaySinh cố ý KHÔNG cho tự sửa (dữ liệu định danh do Admin quản lý).
/// </para>
/// </summary>
public sealed class UpdateMyStudentProfileRequest
{
    [StringLength(150, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2 đến 150 ký tự.")]
    public string? HoTen { get; set; }

    [StringLength(20)] public string? GioiTinh { get; set; }

    [StringLength(20)] public string? SoDienThoai { get; set; }

    [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
    [StringLength(255)] public string? Email { get; set; }

    [StringLength(500)] public string? DiaChi { get; set; }

    [StringLength(1000)] public string? MucTieuHocTap { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>Response của POST /api/admin/students — temporaryPassword CHỈ trả đúng MỘT LẦN.</summary>
public sealed class CreateStudentResponse
{
    public string StudentId { get; init; } = string.Empty;

    public string Username { get; init; } = string.Empty;

    /// <summary>Mật khẩu tạm — chỉ xuất hiện trong response này, không lưu plain text, không log.</summary>
    public string TemporaryPassword { get; init; } = string.Empty;

    public string FullName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;

    public string Message { get; init; } = string.Empty;
}

/// <summary>Thông tin phụ huynh trả về (chỉ đọc ở Phase 3).</summary>
public sealed class ParentDto
{
    public int MaPhuHuynh { get; init; }

    public string HoTen { get; init; } = string.Empty;

    public string? SoDienThoai { get; init; }

    public string? Email { get; init; }

    public string? DiaChi { get; init; }

    public string? NgheNghiep { get; init; }

    public string? MoiQuanHe { get; init; }
}

/// <summary>Chi tiết học viên — dùng cho cả Admin detail và Student self-profile.</summary>
public sealed class StudentDetailDto
{
    public string MaHocVien { get; init; } = string.Empty;

    public string HoTen { get; init; } = string.Empty;

    public DateOnly NgaySinh { get; init; }

    public string? GioiTinh { get; init; }

    public string? SoDienThoai { get; init; }

    public string? Email { get; init; }

    public string? DiaChi { get; init; }

    public string? MucTieuHocTap { get; init; }

    public string? TrinhDoHienTai { get; init; }

    public string TrangThaiHocTap { get; init; } = string.Empty;

    public DateTime NgayTaoHoSo { get; init; }

    public DateOnly? NgayNhapHoc { get; init; }

    public ParentDto? PhuHuynh { get; init; }
}

/// <summary>Một dòng trong danh sách học viên (Admin).</summary>
public sealed class StudentListItemDto
{
    public string MaHocVien { get; init; } = string.Empty;

    public string HoTen { get; init; } = string.Empty;

    public DateOnly NgaySinh { get; init; }

    public string? GioiTinh { get; init; }

    public string? Email { get; init; }

    public string? SoDienThoai { get; init; }

    public string TrangThaiHocTap { get; init; } = string.Empty;

    /// <summary>
    /// Ngày nhập học = ngày BẮT ĐẦU lớp học (lớp sớm nhất học viên đã vào).
    /// <c>set</c> để store điền bổ sung cho dữ liệu cũ chưa đồng bộ.
    /// </summary>
    public DateOnly? NgayNhapHoc { get; set; }
}

/// <summary>Tham số lọc danh sách học viên (Admin).</summary>
public sealed class StudentListQuery : PagedQuery
{
    /// <summary>Tìm theo MaHocVien, HoTen hoặc Email.</summary>
    public string? Search { get; set; }

    /// <summary>Lọc theo TrangThaiHocTap.</summary>
    public string? TrangThai { get; set; }
}
