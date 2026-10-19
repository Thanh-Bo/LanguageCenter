using LanguageCenter.Application.GradeTypes.Models;

namespace LanguageCenter.Application.Reports.Models;

// ============================================================================
// ACTOR
// ============================================================================

/// <summary>Danh tính người xem báo cáo, dựng từ JWT.</summary>
public sealed record ReportActor(string TenDangNhap, string VaiTro);

// ============================================================================
// QUERY
// ============================================================================

/// <summary>Tham số báo cáo học tập — bắt buộc chọn một lớp.</summary>
public sealed class AcademicReportQuery
{
    public int MaLopHoc { get; set; }
}

/// <summary>Tham số báo cáo điểm danh — bắt buộc chọn một lớp.</summary>
public sealed class AttendanceReportQuery
{
    public int MaLopHoc { get; set; }

    public DateOnly? TuNgay { get; set; }

    public DateOnly? DenNgay { get; set; }

    /// <summary>Ngưỡng % vắng bị coi là cao — mặc định 30.</summary>
    public int NguongVang { get; set; } = 30;
}

// ============================================================================
// TỔNG QUAN
// ============================================================================

/// <summary>Các chỉ số tổng quan toàn trung tâm.</summary>
public sealed class ReportOverviewDto
{
    public int TongTaiKhoan { get; init; }

    public int TongHocVien { get; init; }

    public int SoHocVienDangHoc { get; init; }

    public int TongGiaoVien { get; init; }

    public int TongNhanVien { get; init; }

    public int TongChuongTrinh { get; init; }

    public int TongCapDo { get; init; }

    public int TongKhoaHoc { get; init; }

    public int TongLopHoc { get; init; }

    public int SoLopDangHoc { get; init; }

    public EnrollmentSummaryDto DangKy { get; init; } = new();

    public SessionSummaryDto CaHoc { get; init; } = new();

    public AttendanceSummaryDto DiemDanh { get; init; } = new();

    public TuitionOverviewDto HocPhi { get; init; } = new();

    public SurveyOverviewDto KhaoSat { get; init; } = new();
}

/// <summary>Đăng ký học theo trạng thái.</summary>
public sealed class EnrollmentSummaryDto
{
    public int TongSo { get; init; }

    public int ChoXuLy { get; init; }

    public int DaXepLop { get; init; }

    public int HoanThanh { get; init; }

    public int DaHuy { get; init; }
}

/// <summary>Ca học theo trạng thái.</summary>
public sealed class SessionSummaryDto
{
    public int TongSo { get; init; }

    public int ChuaDienRa { get; init; }

    public int DangDienRa { get; init; }

    public int DaHoanThanh { get; init; }

    public int DaHuy { get; init; }

    /// <summary>Ca học sắp diễn ra tính từ hôm nay (chưa hủy, chưa hoàn thành).</summary>
    public int SapDienRa { get; init; }
}

/// <summary>Tình hình điểm danh toàn hệ thống.</summary>
public sealed class AttendanceSummaryDto
{
    public int TongLuot { get; init; }

    public int CoMat { get; init; }

    public int Vang { get; init; }

    public int VangCoPhep { get; init; }

    public int DiTre { get; init; }

    /// <summary>Tỉ lệ có mặt (%) làm tròn 2 chữ số.</summary>
    public decimal TiLeCoMat { get; init; }
}

/// <summary>Tình hình học phí.</summary>
public sealed class TuitionOverviewDto
{
    public decimal TongPhaiThu { get; init; }

    public decimal TongDaThu { get; init; }

    public decimal TongConThieu { get; init; }

    public int SoHocPhiConNo { get; init; }

    public int SoHocVienConNo { get; init; }
}

/// <summary>Tình hình khảo sát.</summary>
public sealed class SurveyOverviewDto
{
    public int TongSo { get; init; }

    public int SoHoatDong { get; init; }

    public int TongCauHoi { get; init; }

    public int TongPhanHoi { get; init; }
}

// ============================================================================
// HỌC TẬP
// ============================================================================

/// <summary>Báo cáo học tập của một lớp.</summary>
public sealed class AcademicReportDto
{
    public int MaLopHoc { get; init; }

    public string MaLop { get; init; } = string.Empty;

    public string TenLop { get; init; } = string.Empty;

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    public int SoHocVien { get; init; }

    /// <summary>Tổng trọng số các đầu điểm đang hoạt động.</summary>
    public decimal TongTrongSo { get; init; }

    /// <summary>True khi tổng trọng số khác 100.</summary>
    public bool CanhBaoTrongSo { get; init; }

    public IReadOnlyList<GradeComponentInfo> ThanhPhan { get; init; } = [];

    public int SoDuDieuKien { get; init; }

    public int SoThieuDiem { get; init; }

    public decimal? DiemTongKetTrungBinh { get; init; }

    public decimal? DiemCaoNhat { get; init; }

    public decimal? DiemThapNhat { get; init; }

    public IReadOnlyList<AcademicStudentRowDto> HocViens { get; init; } = [];
}

/// <summary>Kết quả học tập của một học viên trong lớp.</summary>
public sealed class AcademicStudentRowDto
{
    public int MaDangKy { get; init; }

    public string MaHocVien { get; init; } = string.Empty;

    public string HoTen { get; init; } = string.Empty;

    public bool DuDieuKien { get; init; }

    /// <summary>Điểm tổng kết thang 10 — cùng công thức đã chốt ở Phase 5.</summary>
    public decimal? DiemTongKet { get; init; }

    public IReadOnlyList<AcademicComponentScoreDto> ThanhPhan { get; init; } = [];

    public IReadOnlyList<string> ThanhPhanConThieu { get; init; } = [];
}

/// <summary>Điểm của một đầu điểm.</summary>
public sealed class AcademicComponentScoreDto
{
    public int MaLoaiDiem { get; init; }

    public string TenLoaiDiem { get; init; } = string.Empty;

    public decimal TrongSo { get; init; }

    public decimal DiemToiDa { get; init; }

    public decimal? Diem { get; init; }
}

// ============================================================================
// ĐIỂM DANH
// ============================================================================

/// <summary>Báo cáo điểm danh của một lớp.</summary>
public sealed class AttendanceReportDto
{
    public int MaLopHoc { get; init; }

    public string MaLop { get; init; } = string.Empty;

    public string TenLop { get; init; } = string.Empty;

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    public DateOnly? TuNgay { get; init; }

    public DateOnly? DenNgay { get; init; }

    /// <summary>Số ca học (chưa hủy) đã có ít nhất một bản ghi điểm danh.</summary>
    public int SoCaHocDaDiemDanh { get; init; }

    public int NguongVang { get; init; }

    public decimal TiLeCoMatToanLop { get; init; }

    public IReadOnlyList<AttendanceStudentRowDto> HocViens { get; init; } = [];

    /// <summary>Học viên có tỉ lệ vắng &gt;= <see cref="NguongVang"/> (và có ít nhất một lượt điểm danh).</summary>
    public IReadOnlyList<AttendanceStudentRowDto> HocVienVangCao { get; init; } = [];
}

/// <summary>Chuyên cần của một học viên.</summary>
public sealed class AttendanceStudentRowDto
{
    public string MaHocVien { get; init; } = string.Empty;

    public string HoTen { get; init; } = string.Empty;

    public int TongLuot { get; init; }

    public int CoMat { get; init; }

    public int Vang { get; init; }

    public int VangCoPhep { get; init; }

    public int DiTre { get; init; }

    public decimal TiLeCoMat { get; init; }

    public decimal TiLeVang { get; init; }

    public bool VangCao { get; init; }
}

// ============================================================================
// KHẢO SÁT
// ============================================================================

/// <summary>Báo cáo tổng hợp khảo sát.</summary>
public sealed class SurveyReportDto
{
    public int TongKhaoSat { get; init; }

    public int SoHoatDong { get; init; }

    public int SoTamDung { get; init; }

    public int SoDaKetThuc { get; init; }

    public int TongCauHoi { get; init; }

    public int TongPhanHoi { get; init; }

    public decimal? DiemDanhGiaChungTrungBinh { get; init; }

    public int TongHocVien { get; init; }

    public IReadOnlyList<SurveyReportRowDto> KhaoSats { get; init; } = [];
}

/// <summary>Một dòng thống kê khảo sát.</summary>
public sealed class SurveyReportRowDto
{
    public int MaKhaoSat { get; init; }

    public string TenKhaoSat { get; init; } = string.Empty;

    public string TrangThai { get; init; } = string.Empty;

    public DateOnly NgayBatDau { get; init; }

    public DateOnly? NgayKetThuc { get; init; }

    public int SoCauHoi { get; init; }

    public int SoPhanHoi { get; init; }

    public decimal? DiemTrungBinh { get; init; }

    /// <summary>
    /// Tỉ lệ phản hồi (%) trên TỔNG số học viên hiện có — chỉ là chỉ số tham khảo
    /// vì database không lưu "đối tượng mục tiêu" của khảo sát.
    /// </summary>
    public decimal? TiLePhanHoi { get; init; }
}
