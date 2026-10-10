namespace LanguageCenter.WinForms.Api;

// ============================================================================
// Toàn bộ DTO dưới đây phản chiếu 1-1 các model trong LanguageCenter.Application
// (Auth/Models, Accounts/Models, Students/Models, Teachers/Models) và
// LanguageCenter.Application.Common.PagedResult<T>. Tên property giữ nguyên
// (ASP.NET Core serialize JSON theo camelCase, System.Text.Json ở đây dùng
// PropertyNameCaseInsensitive nên không cần [JsonPropertyName]).
// ============================================================================

#region Common

public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = Array.Empty<T>();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalItems { get; init; }
    public int TotalPages { get; init; }
}

#endregion

#region Auth

public sealed class LoginRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public sealed class LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string TokenType { get; init; } = "Bearer";
    public DateTime ExpiresAt { get; init; }
    public CurrentUserDto User { get; init; } = new();
}

public sealed class CurrentUserDto
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
}

#endregion

#region Accounts

public sealed class AccountListItemDto
{
    public int Id { get; init; }
    public string Username { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
}

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
    public string? ProfileType { get; init; }
    public string? ProfileCode { get; init; }
    public string? TeacherType { get; init; }
    public string? StudentStatus { get; init; }
}

public sealed class UpdateAccountStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

public sealed class ResetPasswordResponse
{
    public string Username { get; init; } = string.Empty;
    public string TemporaryPassword { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}

public sealed class AccountDeleteBlockedResponse
{
    public string? Message { get; init; }
    public string? Reason { get; init; }
    public string? Suggestion { get; init; }
}

#endregion

#region Students

public sealed class CreateParentRequest
{
    public string HoTen { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; }
    public string? Email { get; set; }
    public string? DiaChi { get; set; }
    public string? NgheNghiep { get; set; }
    public string MoiQuanHe { get; set; } = string.Empty;
}

public sealed class CreateStudentRequest
{
    public string HoTen { get; set; } = string.Empty;
    public DateOnly NgaySinh { get; set; }
    public string? GioiTinh { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; }
    public string? DiaChi { get; set; }
    public string? MucTieuHocTap { get; set; }
    public string? TrinhDoHienTai { get; set; }
    public CreateParentRequest? PhuHuynh { get; set; }
}

public sealed class UpdateStudentRequest
{
    public string? HoTen { get; set; }
    public DateOnly? NgaySinh { get; set; }
    public string? GioiTinh { get; set; }
    public string? Email { get; set; }
    public string? SoDienThoai { get; set; }
    public string? DiaChi { get; set; }
    public string? MucTieuHocTap { get; set; }
    public string? TrinhDoHienTai { get; set; }
    public string? TrangThaiHocTap { get; set; }
    public DateOnly? NgayNhapHoc { get; set; }
}

public sealed class CreateStudentResponse
{
    public string StudentId { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string TemporaryPassword { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string Role { get; init; } = string.Empty;
    public string Message { get; init; } = string.Empty;
}

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

public sealed class StudentListItemDto
{
    public string MaHocVien { get; init; } = string.Empty;
    public string HoTen { get; init; } = string.Empty;
    public DateOnly NgaySinh { get; init; }
    public string? GioiTinh { get; init; }
    public string? Email { get; init; }
    public string? SoDienThoai { get; init; }
    public string TrangThaiHocTap { get; init; } = string.Empty;
    public DateOnly? NgayNhapHoc { get; init; }
}

#endregion

#region Teachers

public sealed class CreateTeacherRequest
{
    public string HoTen { get; set; } = string.Empty;
    public DateOnly? NgaySinh { get; set; }
    public string? GioiTinh { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? SoDienThoai { get; set; }
    public string? DiaChi { get; set; }
    public string LoaiGiaoVien { get; set; } = string.Empty;
    public string? ChuyenMon { get; set; }
    public int? SoNamKinhNghiem { get; set; }
    public DateOnly? NgayVaoLam { get; set; }
}

public sealed class UpdateTeacherRequest
{
    public string? HoTen { get; set; }
    public DateOnly? NgaySinh { get; set; }
    public string? GioiTinh { get; set; }
    public string? Email { get; set; }
    public string? SoDienThoai { get; set; }
    public string? DiaChi { get; set; }
    public string? ChuyenMon { get; set; }
    public int? SoNamKinhNghiem { get; set; }
    public DateOnly? NgayVaoLam { get; set; }
    public string? TrangThai { get; set; }
}

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
    public IReadOnlyList<BangCapDto> BangCaps { get; init; } = Array.Empty<BangCapDto>();
    public IReadOnlyList<ChungChiDto> ChungChis { get; init; } = Array.Empty<ChungChiDto>();
}

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

#endregion

#region Constants (phản chiếu LanguageCenter.Domain.Constants)

public static class RoleNames
{
    public const string QuanTriVien = "Quản trị viên";
    public const string NhanVienHocVu = "Nhân viên học vụ";
    public const string NhanVienTuVan = "Nhân viên tư vấn";
    public const string NhanVienKeToan = "Nhân viên kế toán";
    public const string GiaoVien = "Giáo viên";
    public const string TroGiang = "Trợ giảng";
    public const string HocVien = "Học viên";

    public static readonly IReadOnlyList<string> All =
    [
        QuanTriVien, NhanVienHocVu, NhanVienTuVan, NhanVienKeToan, GiaoVien, TroGiang, HocVien
    ];
}

public static class AccountStatuses
{
    public const string HoatDong = "Hoạt động";
    public const string BiKhoa = "Bị khóa";
    public const string NgungSuDung = "Ngừng sử dụng";

    public static readonly IReadOnlyList<string> All = [HoatDong, BiKhoa, NgungSuDung];
}

public static class StudentStatuses
{
    public const string DangTuVan = "Đang tư vấn";
    public const string DangHoc = "Đang học";
    public const string BaoLuu = "Bảo lưu";
    public const string DaNghi = "Đã nghỉ";

    public static readonly IReadOnlyList<string> All = [DangTuVan, DangHoc, BaoLuu, DaNghi];
}

public static class WorkStatuses
{
    public const string DangLamViec = "Đang làm việc";
    public const string DaNghiViec = "Đã nghỉ việc";

    public static readonly IReadOnlyList<string> All = [DangLamViec, DaNghiViec];
}

public static class TeacherTypes
{
    public const string GiaoVien = "Giáo viên";
    public const string TroGiang = "Trợ giảng";

    public static readonly IReadOnlyList<string> All = [GiaoVien, TroGiang];
}

#endregion
