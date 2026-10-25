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

    /// <summary>NhanVien / GiaoVien / HocVien — null nếu tài khoản chưa có hồ sơ.</summary>
    public string? ProfileType { get; init; }

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

#region API thật: Lớp học / Ca học / Điểm / Học phí / Báo cáo
// Phản chiếu DTO của backend (camelCase khi serialize, đọc với PropertyNameCaseInsensitive).

public sealed class ClassListItemDto
{
    public int MaLopHoc { get; init; }
    public string MaLop { get; init; } = string.Empty;
    public string TenLop { get; init; } = string.Empty;
    public int MaKhoaHoc { get; init; }
    public string TenKhoaHoc { get; init; } = string.Empty;
    public string MaNhanVienHocVu { get; init; } = string.Empty;
    public string TenNhanVienHocVu { get; init; } = string.Empty;
    public DateOnly NgayBatDau { get; init; }
    public DateOnly NgayKetThucDuKien { get; init; }
    public DateOnly? NgayKetThucThucTe { get; init; }
    public int SiSoToiDa { get; init; }
    public int SiSoHienTai { get; init; }
    public int SoCaHoc { get; init; }
    public string TrangThai { get; init; } = string.Empty;
}

/// <summary>Kết quả <c>POST /api/admin/sessions/generate</c>.</summary>
public sealed class GenerateSessionsResponseDto
{
    public int MaLichHoc { get; init; }
    public DateOnly TuNgay { get; init; }
    public DateOnly DenNgay { get; init; }
    public DateOnly? ApDungTuThucTe { get; init; }
    public DateOnly? ApDungDenThucTe { get; init; }
    public int TongNgayKhop { get; init; }
    public IReadOnlyList<DateOnly> NgayTao { get; init; } = Array.Empty<DateOnly>();
    public IReadOnlyList<DateOnly> NgayDaCo { get; init; } = Array.Empty<DateOnly>();
    public IReadOnlyList<GenerateConflictDto> NgayXungDot { get; init; } = Array.Empty<GenerateConflictDto>();
}

public sealed class GenerateConflictDto
{
    public DateOnly NgayHoc { get; init; }
    public string LyDo { get; init; } = string.Empty;
}

public sealed class SessionListItemDto
{
    public int MaCaHoc { get; init; }
    public int MaLopHoc { get; init; }
    public string MaLop { get; init; } = string.Empty;
    public string TenLop { get; init; } = string.Empty;
    public string TenKhoaHoc { get; init; } = string.Empty;
    public int? MaLichHoc { get; init; }
    public int MaPhongHoc { get; init; }
    public string MaPhong { get; init; } = string.Empty;
    public string TenPhong { get; init; } = string.Empty;
    public string? TenGiaoVienThucTe { get; init; }
    public string? TenTroGiangThucTe { get; init; }
    public DateOnly NgayHoc { get; init; }
    public TimeOnly GioBatDau { get; init; }
    public TimeOnly GioKetThuc { get; init; }
    public string? NoiDungBaiHoc { get; init; }
    public string TrangThai { get; init; } = string.Empty;
    public string? LyDoThayDoi { get; init; }
    public int SoDiemDanh { get; init; }
}

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

public sealed class CourseListItemDto
{
    public int MaKhoaHoc { get; init; }
    public int MaCapDo { get; init; }
    public string TenCapDo { get; init; } = string.Empty;
    public int MaChuongTrinh { get; init; }
    public string TenChuongTrinh { get; init; } = string.Empty;
    public string TenKhoaHoc { get; init; } = string.Empty;
    public int SoBuoi { get; init; }
    public decimal? TongSoGio { get; init; }
    public decimal HocPhiCoBan { get; init; }
    public DateOnly NgayBatDau { get; init; }
    public DateOnly NgayKetThuc { get; init; }
    public string TrangThai { get; init; } = string.Empty;
    public int SoLop { get; init; }
}

public sealed class LevelListItemDto
{
    public int MaCapDo { get; init; }
    public int MaChuongTrinh { get; init; }
    public string TenChuongTrinh { get; init; } = string.Empty;
    public string KyHieuCapDo { get; init; } = string.Empty;
    public string TenCapDo { get; init; } = string.Empty;
    public int ThuTu { get; init; }
    public int? ThoiLuongDuKienTuan { get; init; }
    public string? DieuKienDauVao { get; init; }
    public string? KetQuaDauRa { get; init; }
    public string TrangThai { get; init; } = string.Empty;
}

public sealed class ProgramListItemDto
{
    public int MaChuongTrinh { get; init; }
    public string TenChuongTrinh { get; init; } = string.Empty;
    public string? NhomDoiTuong { get; init; }
    public int? DoTuoiTu { get; init; }
    public int? DoTuoiDen { get; init; }
    public string? MucTieu { get; init; }
    public string? MoTa { get; init; }
    public string TrangThai { get; init; } = string.Empty;
}

public sealed class EnrollmentListItemDto
{
    public int MaDangKy { get; init; }
    public string MaHocVien { get; init; } = string.Empty;
    public string TenHocVien { get; init; } = string.Empty;
    public int MaKhoaHoc { get; init; }
    public string TenKhoaHoc { get; init; } = string.Empty;
    public int? MaLopHoc { get; init; }
    public string? MaLop { get; init; }
    public DateTime NgayDangKy { get; init; }
    public decimal HocPhiGoc { get; init; }
    public decimal SoTienGiam { get; init; }
    public decimal HocPhiSauGiam { get; init; }
    public string TrangThaiDangKy { get; init; } = string.Empty;
    public DateTime? NgayXepLop { get; init; }
}

public sealed class NotificationListItemDto
{
    public int MaThongBao { get; init; }
    public int MaTaiKhoanGui { get; init; }
    public string TenDangNhapNguoiGui { get; init; } = string.Empty;
    public int? MaTaiKhoanNhan { get; init; }
    public string? TenDangNhapNguoiNhan { get; init; }
    public int? MaLopHoc { get; init; }
    public string? MaLop { get; init; }
    public string? TenLop { get; init; }
    public string TieuDe { get; init; } = string.Empty;
    public string NoiDung { get; init; } = string.Empty;
    public string? LoaiThongBao { get; init; }
    public DateTime ThoiGianGui { get; init; }
    public string TrangThai { get; init; } = string.Empty;
    public string PhamVi { get; init; } = string.Empty;
}

public sealed class AssignmentListItemDto
{
    public int MaPhanCong { get; init; }
    public int MaLopHoc { get; init; }
    public string MaLop { get; init; } = string.Empty;
    public string TenLop { get; init; } = string.Empty;
    public int MaKhoaHoc { get; init; }
    public string TenKhoaHoc { get; init; } = string.Empty;
    /// <summary>NULL = khung phân công chưa gắn người (trạng thái "Chờ phân công").</summary>
    public string? MaGiaoVien { get; init; }
    public string? TenGiaoVien { get; init; }
    public string? LoaiGiaoVien { get; init; }
    public string VaiTroPhanCong { get; init; } = string.Empty;
    public DateOnly NgayBatDau { get; init; }
    public DateOnly? NgayKetThuc { get; init; }
    public string TrangThai { get; init; } = string.Empty;
}

public sealed class GradeComponentDto
{
    public int MaLoaiDiem { get; init; }
    public string TenLoaiDiem { get; init; } = string.Empty;
    public decimal TrongSo { get; init; }
    public decimal DiemToiDa { get; init; }
    public int ThuTu { get; init; }
}

public sealed class ComponentScoreDto
{
    public int MaLoaiDiem { get; init; }
    public string TenLoaiDiem { get; init; } = string.Empty;
    public decimal TrongSo { get; init; }
    public decimal DiemToiDa { get; init; }
    public decimal? Diem { get; init; }
}

public sealed class AcademicStudentRowDto
{
    public int MaDangKy { get; init; }
    public string MaHocVien { get; init; } = string.Empty;
    public string HoTen { get; init; } = string.Empty;
    public bool DuDieuKien { get; init; }
    public decimal? DiemTongKet { get; init; }
    public IReadOnlyList<ComponentScoreDto> ThanhPhan { get; init; } = Array.Empty<ComponentScoreDto>();
    public IReadOnlyList<string> ThanhPhanConThieu { get; init; } = Array.Empty<string>();
}

public sealed class AcademicReportDto
{
    public int MaLopHoc { get; init; }
    public string MaLop { get; init; } = string.Empty;
    public string TenLop { get; init; } = string.Empty;
    public string TenKhoaHoc { get; init; } = string.Empty;
    public int SoHocVien { get; init; }
    public decimal TongTrongSo { get; init; }
    public bool CanhBaoTrongSo { get; init; }
    public IReadOnlyList<GradeComponentDto> ThanhPhan { get; init; } = Array.Empty<GradeComponentDto>();
    public int SoDuDieuKien { get; init; }
    public int SoThieuDiem { get; init; }
    public decimal? DiemTongKetTrungBinh { get; init; }
    public decimal? DiemCaoNhat { get; init; }
    public decimal? DiemThapNhat { get; init; }
    public IReadOnlyList<AcademicStudentRowDto> HocViens { get; init; } = Array.Empty<AcademicStudentRowDto>();
}

public sealed class TuitionItemDto
{
    public int MaHocPhi { get; init; }
    public int MaDangKy { get; init; }
    public string MaHocVien { get; init; } = string.Empty;
    public string HoTenHocVien { get; init; } = string.Empty;
    public int MaKhoaHoc { get; init; }
    public string TenKhoaHoc { get; init; } = string.Empty;
    public int? MaLopHoc { get; init; }
    public string? MaLop { get; init; }
    public string? TenLop { get; init; }
    public decimal SoTienGoc { get; init; }
    public decimal SoTienGiam { get; init; }
    public decimal SoTienPhaiDong { get; init; }
    public decimal SoTienDaDong { get; init; }
    public decimal SoTienConThieu { get; init; }
    public DateOnly? HanThanhToan { get; init; }
    public string TrangThaiHocPhi { get; init; } = string.Empty;
    public DateTime NgayTao { get; init; }
    public int SoGiaoDich { get; init; }
    public bool QuaHan { get; init; }
    public bool ConNo { get; init; }
}

public sealed class ReceiptBriefDto
{
    public int MaBienNhan { get; init; }
    public string SoBienNhan { get; init; } = string.Empty;
    public DateTime NgayLap { get; init; }
    public string TrangThai { get; init; } = string.Empty;
}

public sealed class PaymentDto
{
    public int MaThanhToan { get; init; }
    public int MaHocPhi { get; init; }
    public string MaHocVien { get; init; } = string.Empty;
    public string HoTenHocVien { get; init; } = string.Empty;
    public string TenKhoaHoc { get; init; } = string.Empty;
    public decimal SoTien { get; init; }
    public DateTime NgayThanhToan { get; init; }
    public string PhuongThucThanhToan { get; init; } = string.Empty;
    public string? MaGiaoDich { get; init; }
    public string LoaiGiaoDich { get; init; } = string.Empty;
    public string TrangThai { get; init; } = string.Empty;
    public string? GhiChu { get; init; }
    public string TenNhanVienKeToan { get; init; } = string.Empty;
    public ReceiptBriefDto? BienNhan { get; init; }
}

public sealed class FinanceReportDto
{
    public int TongSoHocPhi { get; init; }
    public decimal TongPhaiThu { get; init; }
    public decimal TongDaThu { get; init; }
    public decimal TongConThieu { get; init; }
    public int SoChuaThanhToan { get; init; }
    public int SoThanhToanMotPhan { get; init; }
    public int SoDaThanhToan { get; init; }
    public int SoQuaHan { get; init; }
    public int SoHocVienConNo { get; init; }
    public IReadOnlyList<FinanceDebtorDto> DanhSachConNo { get; init; } = Array.Empty<FinanceDebtorDto>();
    public IReadOnlyList<FinanceGroupDto> ThongKeGiaoDichTheoPhuongThuc { get; init; } = Array.Empty<FinanceGroupDto>();
    public IReadOnlyList<FinanceGroupDto> ThongKeGiaoDichTheoTrangThai { get; init; } = Array.Empty<FinanceGroupDto>();
}

public sealed class FinanceDebtorDto
{
    public string MaHocVien { get; init; } = string.Empty;
    public string HoTen { get; init; } = string.Empty;
    public int SoHocPhiConNo { get; init; }
    public decimal TongConThieu { get; init; }
}

public sealed class FinanceGroupDto
{
    public string Nhan { get; init; } = string.Empty;
    public int SoLuong { get; init; }
    public decimal TongTien { get; init; }
}

public sealed class ReportOverviewDto
{
    public int TongTaiKhoan { get; init; }
    public int TongHocVien { get; init; }
    public int SoHocVienDangHoc { get; init; }
    public int TongGiaoVien { get; init; }
    public int TongNhanVien { get; init; }
    public int TongChuongTrinh { get; init; }
    public int TongKhoaHoc { get; init; }
    public int TongLopHoc { get; init; }
    public int SoLopDangHoc { get; init; }
    public OverviewDangKyDto DangKy { get; init; } = new();
    public OverviewCaHocDto CaHoc { get; init; } = new();
    public OverviewDiemDanhDto DiemDanh { get; init; } = new();
    public OverviewHocPhiDto HocPhi { get; init; } = new();
    public OverviewKhaoSatDto KhaoSat { get; init; } = new();
}

public sealed class OverviewDangKyDto
{
    public int TongSo { get; init; }
    public int ChoXuLy { get; init; }
    public int DaXepLop { get; init; }
    public int HoanThanh { get; init; }
    public int DaHuy { get; init; }
}

public sealed class OverviewCaHocDto
{
    public int TongSo { get; init; }
    public int ChuaDienRa { get; init; }
    public int DangDienRa { get; init; }
    public int DaHoanThanh { get; init; }
    public int DaHuy { get; init; }
    public int SapDienRa { get; init; }
}

public sealed class OverviewDiemDanhDto
{
    public int TongLuot { get; init; }
    public int CoMat { get; init; }
    public int Vang { get; init; }
    public int VangCoPhep { get; init; }
    public int DiTre { get; init; }
    public decimal TiLeCoMat { get; init; }
}

public sealed class OverviewHocPhiDto
{
    public decimal TongPhaiThu { get; init; }
    public decimal TongDaThu { get; init; }
    public decimal TongConThieu { get; init; }
    public int SoHocPhiConNo { get; init; }
    public int SoHocVienConNo { get; init; }
}

public sealed class OverviewKhaoSatDto
{
    public int TongSo { get; init; }
    public int SoHoatDong { get; init; }
    public int TongCauHoi { get; init; }
    public int TongPhanHoi { get; init; }
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

#region API thật: Điểm số & Điểm danh

/// <summary>Một bản ghi điểm (dạng danh sách) — dùng để tra <c>MaDiemSo</c> khi sửa/xoá.</summary>
public sealed class GradeRowDto
{
    public int MaDiemSo { get; init; }
    public int MaDangKy { get; init; }
    public string MaHocVien { get; init; } = string.Empty;
    public string HoTenHocVien { get; init; } = string.Empty;
    public int? MaLopHoc { get; init; }
    public string? MaLop { get; init; }
    public string? TenLop { get; init; }
    public int MaLoaiDiem { get; init; }
    public string TenLoaiDiem { get; init; } = string.Empty;
    public decimal DiemToiDa { get; init; }
    public decimal Diem { get; init; }
    public string? NhanXet { get; init; }
    public string? TrangThaiKetQua { get; init; }
    public string MaGiaoVien { get; init; } = string.Empty;
    public string TenGiaoVien { get; init; } = string.Empty;
}

/// <summary>Một học viên trong bảng điểm danh của ca học.</summary>
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

/// <summary>Bảng điểm danh của một ca học.</summary>
public sealed class AttendanceSheetDto
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
    public bool CoTheDiemDanh { get; set; }
    public string? LyDoKhongTheDiemDanh { get; set; }
    public IReadOnlyList<AttendanceRowDto> HocViens { get; set; } = Array.Empty<AttendanceRowDto>();
}

#endregion
