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

    /// <summary>"Buổi học" hoặc "Buổi thi". Để trống = "Buổi học".</summary>
    [StringLength(30)]
    public string? LoaiBuoi { get; set; }

    /// <summary>Buổi thứ mấy trong khóa. Để trống = tự đánh số theo ngày.</summary>
    [Range(1, 200)]
    public int? SoBuoi { get; set; }

    /// <summary>Tham chiếu dòng syllabus của khóa (để trống nếu không có).</summary>
    public int? MaNoiDung { get; set; }

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

    /// <summary>"Buổi học" / "Buổi thi".</summary>
    [StringLength(30)]
    public string? LoaiBuoi { get; set; }

    [StringLength(500, ErrorMessage = "Lý do thay đổi tối đa 500 ký tự.")]
    public string? LyDoThayDoi { get; set; }
}

/// <summary>
/// Body của <c>POST /api/admin/sessions/generate</c>.
/// <para>
/// Hệ thống sinh ĐÚNG <c>KhoaHoc.SoBuoi</c> buổi bắt đầu từ <c>LopHoc.NgayBatDau</c>,
/// tự dời buổi trùng ngày lễ → không cần client gửi khoảng ngày.
/// </para>
/// </summary>
public sealed class GenerateSessionsRequest
{
    /// <summary>Lịch định kỳ bất kỳ của lớp cần sinh ca (hệ thống tự lấy cả 2 lịch của lớp).</summary>
    [Range(1, int.MaxValue, ErrorMessage = "Lịch học là bắt buộc.")]
    public int MaLichHoc { get; set; }

    /// <summary>
    /// <c>true</c> = vẫn sinh ca dù khóa còn buổi CHƯA có nội dung
    /// (các buổi đó sẽ trống nội dung). Mặc định <c>false</c> → chặn và báo rõ.
    /// </summary>
    public bool BoQuaThieuNoiDung { get; set; }
}

/// <summary>
/// Kết quả gợi ý khi tạo ca học từ một lịch định kỳ:
/// toàn bộ giá trị đều SUY RA từ lớp + lịch + phân công + syllabus của khóa,
/// nên màn hình chỉ việc hiển thị (read-only) thay vì bắt người dùng nhập lại.
/// </summary>
public sealed class SessionSuggestionDto
{
    public int MaLopHoc { get; init; }

    public int MaLichHoc { get; init; }

    /// <summary>Buổi thứ mấy trong khóa (NULL = lớp đã sinh đủ buổi).</summary>
    public int? SoBuoi { get; init; }

    public DateOnly NgayHoc { get; init; }

    public TimeOnly GioBatDau { get; init; }

    public TimeOnly GioKetThuc { get; init; }

    public int MaPhongHoc { get; init; }

    public string TenPhong { get; init; } = string.Empty;

    public string? MaGiaoVienThucTe { get; init; }

    public string? TenGiaoVienThucTe { get; init; }

    public string? MaTroGiangThucTe { get; init; }

    public string? TenTroGiangThucTe { get; init; }

    public int? MaNoiDung { get; init; }

    /// <summary>Tiêu đề nội dung bài học (từ syllabus của khóa).</summary>
    public string? TieuDeBaiHoc { get; init; }

    /// <summary>Chuỗi ghi vào <c>CaHoc.NoiDungBaiHoc</c>: "Buổi N — Tiêu đề".</summary>
    public string? NoiDungBaiHoc { get; init; }

    public string LoaiBuoi { get; init; } = "Buổi học";

    /// <summary>Cảnh báo mềm (ví dụ: khóa chưa có nội dung cho buổi này).</summary>
    public string? CanhBao { get; init; }
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

    /// <summary>Buổi thứ mấy trong khóa (NULL với ca phát sinh).</summary>
    public int? SoBuoi { get; init; }

    /// <summary>"Buổi học" hoặc "Buổi thi".</summary>
    public string LoaiBuoi { get; init; } = string.Empty;

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

    /// <summary>Buổi thứ mấy trong khóa (NULL với ca phát sinh).</summary>
    public int? SoBuoi { get; init; }

    /// <summary>"Buổi học" hoặc "Buổi thi".</summary>
    public string LoaiBuoi { get; init; } = string.Empty;

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

/// <summary>Body của <c>POST /api/admin/sessions/sync-teachers</c>.</summary>
public sealed class SyncSessionTeachersRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Lớp học là bắt buộc.")]
    public int MaLopHoc { get; set; }
}

/// <summary>Body của <c>POST /api/admin/sessions/settle</c> (null = toàn bộ lớp).</summary>
public sealed class SettleSessionsRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Lớp học không hợp lệ.")]
    public int? MaLopHoc { get; set; }
}

/// <summary>
/// Kết quả chốt trạng thái ca học theo ngày:
/// buổi đã qua → "Đã hoàn thành", buổi chưa tới ngày → "Chưa diễn ra".
/// </summary>
public sealed class SettleSessionsResponse
{
    public DateOnly NgayChot { get; init; }

    public int? MaLopHoc { get; init; }

    /// <summary>Số ca chuyển sang "Đã hoàn thành" (ngày đã qua).</summary>
    public int SoCaDaChot { get; init; }

    /// <summary>Số ca mở lại thành "Chưa diễn ra" (ngày còn ở tương lai).</summary>
    public int SoCaMoLai { get; init; }
}

/// <summary>
/// Kết quả đồng bộ giáo viên cho các ca CHƯA diễn ra theo quy ước
/// "buổi đầu tuần = GV nước ngoài, buổi còn lại = GV chính".
/// </summary>
public sealed class SyncSessionTeachersResponse
{
    public int MaLopHoc { get; init; }

    /// <summary>Tổng số ca CHƯA diễn ra được xét.</summary>
    public int TongSoCa { get; init; }

    /// <summary>Số ca được cập nhật giáo viên/trợ giảng.</summary>
    public int SoCaCapNhat { get; init; }

    /// <summary>Số ca đã đúng người (không cần sửa).</summary>
    public int SoCaKhongDoi { get; init; }

    /// <summary>Số ca bị bỏ qua (đã diễn ra / đã hủy / không xác định được buổi).</summary>
    public int SoCaBoQua { get; init; }

    public string? MaGiaoVienChinh { get; init; }

    public string? MaGiaoVienNuocNgoai { get; init; }

    public string? MaTroGiang { get; init; }
}

/// <summary>
/// Tiến độ học của một lớp — dùng để tô màu các buổi trên giao diện:
/// buổi đã học, buổi thi, buổi hiện tại.
/// </summary>
public sealed class ClassProgressDto
{
    public int MaLopHoc { get; init; }

    public string MaLop { get; init; } = string.Empty;

    public string TenLop { get; init; } = string.Empty;

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    /// <summary>Tổng số buổi theo khóa học (thường là 20).</summary>
    public int TongSoBuoi { get; init; }

    /// <summary>Số ca học đã sinh cho lớp.</summary>
    public int SoBuoiDaSinh { get; init; }

    /// <summary>Số buổi đã diễn ra (ngày học ≤ hôm nay và không bị hủy).</summary>
    public int SoBuoiDaHoc { get; init; }

    public int SoBuoiConLai { get; init; }

    /// <summary>Phần trăm hoàn thành (0..100), làm tròn 2 chữ số.</summary>
    public decimal TienDoPhanTram { get; init; }

    /// <summary>Mã ca học của buổi hiện tại (buổi gần nhất đã/chưa diễn ra).</summary>
    public int? MaCaHocHienTai { get; init; }

    /// <summary>Số thứ tự buổi hiện tại (1..N).</summary>
    public int? SoBuoiHienTai { get; init; }

    public DateOnly? NgayHocHienTai { get; init; }

    public string? LoaiBuoiHienTai { get; init; }

    public IReadOnlyList<SessionProgressItemDto> Buois { get; init; } = [];
}

/// <summary>Một buổi trong tiến độ lớp.</summary>
public sealed class SessionProgressItemDto
{
    public int MaCaHoc { get; init; }

    public int? SoBuoi { get; init; }

    public DateOnly NgayHoc { get; init; }

    public TimeOnly GioBatDau { get; init; }

    public TimeOnly GioKetThuc { get; init; }

    /// <summary>"Buổi học" hoặc "Buổi thi".</summary>
    public string LoaiBuoi { get; init; } = string.Empty;

    public bool LaBuoiThi { get; init; }

    public string TrangThai { get; init; } = string.Empty;

    public string? NoiDungBaiHoc { get; init; }

    public string? MaGiaoVienThucTe { get; init; }

    public string? TenGiaoVienThucTe { get; init; }

    public string? MaTroGiangThucTe { get; init; }

    public string? TenTroGiangThucTe { get; init; }

    /// <summary>Số học viên đã được điểm danh trong buổi.</summary>
    public int SoDiemDanh { get; init; }

    /// <summary>Buổi đã diễn ra (theo ngày) — giao diện tô màu "đã học".</summary>
    public bool DaHoc { get; init; }

    /// <summary>Buổi đang là buổi hiện tại — giao diện tô màu khác.</summary>
    public bool LaBuoiHienTai { get; init; }
}
