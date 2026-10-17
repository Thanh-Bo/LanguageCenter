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

#region Programs, Levels & Courses (Chương trình, Cấp độ & Khóa học)

public sealed class ChuongTrinhDto
{
    public int MaChuongTrinh { get; set; }
    public string TenChuongTrinh { get; set; } = string.Empty;
    public string? NhomDoiTuong { get; set; }
    public int? DoTuoiTu { get; set; }
    public int? DoTuoiDen { get; set; }
    public string? MucTieu { get; set; }
    public string? MoTa { get; set; }
    public string TrangThai { get; set; } = "Đang mở";
}

public sealed class CapDoDto
{
    public int MaCapDo { get; set; }
    public int MaChuongTrinh { get; set; }
    public string TenChuongTrinh { get; set; } = string.Empty;
    public string KyHieuCapDo { get; set; } = string.Empty;
    public string TenCapDo { get; set; } = string.Empty;
    public int ThuTu { get; set; }
    public string? DieuKienDauVao { get; set; }
    public string? KetQuaDauRa { get; set; }
    public int? ThoiLuongDuKienTuan { get; set; }
    public string TrangThai { get; set; } = "Đang mở";
}

public sealed class KhoaHocDto
{
    public int MaKhoaHoc { get; set; }
    public string MaKhoaHocStr { get; set; } = string.Empty;
    public int MaCapDo { get; set; }
    public string TenCapDo { get; set; } = string.Empty;
    public string TenChuongTrinh { get; set; } = string.Empty;
    public string TenKhoaHoc { get; set; } = string.Empty;
    public string? NoiDung { get; set; }
    public int SoBuoi { get; set; }
    public decimal TongSoGio { get; set; }
    public decimal HocPhiCoBan { get; set; }
    public DateOnly NgayBatDau { get; set; }
    public DateOnly NgayKetThuc { get; set; }
    public string TrangThai { get; set; } = "Đang tuyển sinh";
}

#endregion

#region Classes & Teaching Assignments (Lớp học & Phân công)

public sealed class LopHocDto
{
    public int MaLopHoc { get; set; }
    public string MaLop { get; set; } = string.Empty;
    public string TenLop { get; set; } = string.Empty;
    public int MaKhoaHoc { get; set; }
    public string TenKhoaHoc { get; set; } = string.Empty;
    public string MaNhanVienHocVu { get; set; } = string.Empty;
    public string TenNhanVienHocVu { get; set; } = string.Empty;
    public string TenGiaoVienChinh { get; set; } = string.Empty;
    public string TenTroGiang { get; set; } = string.Empty;
    public string TenPhongHoc { get; set; } = string.Empty;
    public string LichHoc { get; set; } = string.Empty;
    public DateOnly NgayBatDau { get; set; }
    public DateOnly NgayKetThucDuKien { get; set; }
    public int SiSoToiDa { get; set; }
    public int SiSoHienTai { get; set; }
    public string TrangThai { get; set; } = "Đang học";
}

public sealed class PhanCongGiangDayDto
{
    public int MaPhanCong { get; set; }
    public int MaLopHoc { get; set; }
    public string MaLop { get; set; } = string.Empty;
    public string TenLop { get; set; } = string.Empty;
    public string MaGiaoVien { get; set; } = string.Empty;
    public string TenGiaoVien { get; set; } = string.Empty;
    public string LoaiNhanSu { get; set; } = "Giáo viên";
    public string VaiTroPhanCong { get; set; } = "Giáo viên chính";
    public DateOnly NgayBatDau { get; set; }
    public DateOnly? NgayKetThuc { get; set; }
    public string TrangThai { get; set; } = "Đang giảng dạy";
    public string? GhiChu { get; set; }
}

#endregion

#region Enrollment & Class Placement (Xếp học viên vào lớp)

public sealed class DangKyHocDto
{
    public int MaDangKy { get; set; }
    public string MaHocVien { get; set; } = string.Empty;
    public string TenHocVien { get; set; } = string.Empty;
    public string SoDienThoai { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int MaKhoaHoc { get; set; }
    public string TenKhoaHoc { get; set; } = string.Empty;
    public int? MaLopHoc { get; set; }
    public string? MaLop { get; set; }
    public string? TenLop { get; set; }
    public DateTime NgayDangKy { get; set; }
    public DateTime? NgayXepLop { get; set; }
    public decimal HocPhiGoc { get; set; }
    public decimal SoTienGiam { get; set; }
    public decimal HocPhiPhaiDong { get; set; }
    public decimal SoTienDaDong { get; set; }
    public string TrangThaiHocPhi { get; set; } = "Chờ đóng học phí";
    public string TrangThaiDangKy { get; set; } = "Chờ xếp lớp";
    public string? GhiChu { get; set; }
}

#endregion

#region Rooms, Weekly Schedules & Shifts (Phòng học, Lịch học & Ca học)

public sealed class PhongHocDto
{
    public int MaPhongHoc { get; set; }
    public string MaPhong { get; set; } = string.Empty;
    public string TenPhong { get; set; } = string.Empty;
    public string? ViTri { get; set; }
    public int SucChua { get; set; }
    public string? ThietBi { get; set; }
    public string TrangThai { get; set; } = "Sẵn sàng";
}

public sealed class LichHocDto
{
    public int MaLichHoc { get; set; }
    public int MaLopHoc { get; set; }
    public string MaLop { get; set; } = string.Empty;
    public string TenLop { get; set; } = string.Empty;
    public int MaPhongHoc { get; set; }
    public string TenPhong { get; set; } = string.Empty;
    public byte ThuTrongTuan { get; set; }
    public string ThuText { get; set; } = string.Empty;
    public TimeOnly GioBatDau { get; set; }
    public TimeOnly GioKetThuc { get; set; }
    public DateOnly NgayApDungTu { get; set; }
    public DateOnly? NgayApDungDen { get; set; }
    public string TrangThai { get; set; } = "Đang áp dụng";
}

public sealed class CaHocDto
{
    public int MaCaHoc { get; set; }
    public int MaLopHoc { get; set; }
    public string MaLop { get; set; } = string.Empty;
    public string TenLop { get; set; } = string.Empty;
    public int MaPhongHoc { get; set; }
    public string TenPhong { get; set; } = string.Empty;
    public string? TenGiaoVien { get; set; }
    public string? TenTroGiang { get; set; }
    public DateOnly NgayHoc { get; set; }
    public TimeOnly GioBatDau { get; set; }
    public TimeOnly GioKetThuc { get; set; }
    public string? NoiDungBaiHoc { get; set; }
    public string TrangThai { get; set; } = "Chưa diễn ra";
}

#endregion

#region Grades & Types (Điểm số & Đầu điểm)

public sealed class LoaiDiemDto
{
    public int MaLoaiDiem { get; set; }
    public int MaKhoaHoc { get; set; }
    public string TenLoaiDiem { get; set; } = string.Empty;
    public decimal TrongSo { get; set; }
    public decimal DiemToiDa { get; set; }
    public int ThuTu { get; set; }
    public string TrangThai { get; set; } = "Hoạt động";
}

public sealed class DiemSoHocVienDto
{
    public int MaDangKy { get; set; }
    public string MaHocVien { get; set; } = string.Empty;
    public string TenHocVien { get; set; } = string.Empty;
    public string MaLop { get; set; } = string.Empty;
    public decimal DiemChuyenCan { get; set; }
    public decimal DiemGiuaKy { get; set; }
    public decimal DiemSpeaking { get; set; }
    public decimal DiemWriting { get; set; }
    public decimal DiemCuoiKy { get; set; }
    public decimal DiemTongKet { get; set; }
    public string XepLoai { get; set; } = "Đạt";
    public string? NhanXet { get; set; }
    public DateTime? NgayCham { get; set; }
}

#endregion

#region Payments & Receipts (Học phí, Thanh toán & Biên nhận)

public sealed class HocPhiDto
{
    public int MaHocPhi { get; set; }
    public int MaDangKy { get; set; }
    public string MaHocVien { get; set; } = string.Empty;
    public string TenHocVien { get; set; } = string.Empty;
    public string TenKhoaHoc { get; set; } = string.Empty;
    public string MaLop { get; set; } = string.Empty;
    public decimal SoTienGoc { get; set; }
    public decimal SoTienGiam { get; set; }
    public decimal SoTienPhaiDong { get; set; }
    public decimal SoTienDaDong { get; set; }
    public decimal SoTienConThieu { get; set; }
    public DateOnly? HanThanhToan { get; set; }
    public string TrangThaiHocPhi { get; set; } = "Chờ đóng học phí";
    public DateTime NgayTao { get; set; }
}

public sealed class ThanhToanDto
{
    public int MaThanhToan { get; set; }
    public int MaHocPhi { get; set; }
    public string MaHocVien { get; set; } = string.Empty;
    public string TenHocVien { get; set; } = string.Empty;
    public string MaNhanVienKeToan { get; set; } = string.Empty;
    public decimal SoTien { get; set; }
    public DateTime NgayThanhToan { get; set; }
    public string PhuongThucThanhToan { get; set; } = "Chuyển khoản";
    public string? MaGiaoDich { get; set; }
    public string LoaiGiaoDich { get; set; } = "Thu học phí";
    public string TrangThai { get; set; } = "Thành công";
    public string? GhiChu { get; set; }
}

public sealed class BienNhanDto
{
    public int MaBienNhan { get; set; }
    public int MaThanhToan { get; set; }
    public string SoBienNhan { get; set; } = string.Empty;
    public string TenHocVien { get; set; } = string.Empty;
    public string MaHocVien { get; set; } = string.Empty;
    public string TenKhoaHoc { get; set; } = string.Empty;
    public string MaNhanVienLap { get; set; } = string.Empty;
    public DateTime NgayLap { get; set; }
    public decimal SoTienXacNhan { get; set; }
    public string? NoiDung { get; set; }
    public string PhuongThuc { get; set; } = "Chuyển khoản";
    public string TrangThai { get; set; } = "Hợp lệ";
}

#endregion

#region Notifications (Quản lý thông báo)

public sealed class ThongBaoDto
{
    public int MaThongBao { get; set; }
    public string TieuDe { get; set; } = string.Empty;
    public string NoiDung { get; set; } = string.Empty;
    public string LoaiThongBao { get; set; } = "Học vụ";
    public string DoiTuongNhan { get; set; } = "Toàn trung tâm";
    public int? MaLopHoc { get; set; }
    public string? TenLop { get; set; }
    public string NguoiGui { get; set; } = "Ban Giám Đốc";
    public DateTime ThoiGianGui { get; set; }
    public string TrangThai { get; set; } = "Đã gửi";
}

#endregion

#region Reports (Báo cáo & Thống kê)

public sealed class BaoCaoDoanhThuDto
{
    public string ChuongTrinh { get; set; } = string.Empty;
    public int SoLop { get; set; }
    public int SoHocVien { get; set; }
    public decimal TongHocPhi { get; set; }
    public decimal DaThu { get; set; }
    public decimal ConNo { get; set; }
    public decimal TyLeThu => TongHocPhi == 0 ? 0 : Math.Round((DaThu / TongHocPhi) * 100, 1);
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
