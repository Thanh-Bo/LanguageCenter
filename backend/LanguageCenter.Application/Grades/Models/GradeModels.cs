using System.ComponentModel.DataAnnotations;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Grades.Models;

// ============================================================================
// ACTOR
// ============================================================================

/// <summary>Danh tính người nhập điểm, dựng từ JWT.</summary>
/// <param name="TenDangNhap">Với giáo viên, giá trị này trùng mã giáo viên (<c>GiaoVien.MaGiaoVien</c>).</param>
public sealed record GradeActor(string TenDangNhap, bool IsAdmin);

// ============================================================================
// REQUEST
// ============================================================================

/// <summary>
/// Body của <c>POST /api/admin/grades</c>.
/// <para>
/// GIỚI HẠN SCHEMA: <c>DiemSo.MaGiaoVien</c> là NOT NULL và FK tới <c>GiaoVien</c>,
/// nên Quản trị viên BẮT BUỘC gửi <c>maGiaoVien</c>. Giáo viên thì hệ thống tự lấy từ JWT
/// và bỏ qua trường này.
/// </para>
/// </summary>
public sealed class CreateGradeRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Đăng ký học là bắt buộc.")]
    public int MaDangKy { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Đầu điểm là bắt buộc.")]
    public int MaLoaiDiem { get; set; }

    /// <summary>Chỉ dùng khi người gọi là Quản trị viên.</summary>
    [StringLength(20, MinimumLength = 1)]
    public string? MaGiaoVien { get; set; }

    [Range(typeof(decimal), "0", "1000000", ErrorMessage = "Điểm phải lớn hơn hoặc bằng 0.")]
    public decimal Diem { get; set; }

    [StringLength(500, ErrorMessage = "Nhận xét tối đa 500 ký tự.")]
    public string? NhanXet { get; set; }

    /// <summary>"Đạt" / "Không đạt" / để trống.</summary>
    [StringLength(50)]
    public string? TrangThaiKetQua { get; set; }
}

/// <summary>Body của <c>PATCH /api/admin/grades/{id}</c> — mọi field OPTIONAL.</summary>
public sealed class UpdateGradeRequest
{
    [Range(typeof(decimal), "0", "1000000", ErrorMessage = "Điểm phải lớn hơn hoặc bằng 0.")]
    public decimal? Diem { get; set; }

    [StringLength(500, ErrorMessage = "Nhận xét tối đa 500 ký tự.")]
    public string? NhanXet { get; set; }

    /// <summary>Gửi chuỗi rỗng để xóa kết quả.</summary>
    [StringLength(50)]
    public string? TrangThaiKetQua { get; set; }
}

/// <summary>Tham số lọc danh sách điểm.</summary>
public sealed class GradeListQuery : PagedQuery
{
    public int? MaDangKy { get; set; }

    public int? MaLoaiDiem { get; set; }

    public int? MaKhoaHoc { get; set; }

    public int? MaLopHoc { get; set; }

    public string? MaHocVien { get; set; }

    public string? MaGiaoVien { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>Một dòng trong danh sách điểm.</summary>
public sealed class GradeListItemDto
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

    public DateTime NgayCham { get; init; }

    public DateTime? NgayCapNhat { get; init; }
}

/// <summary>Chi tiết một bản ghi điểm.</summary>
public sealed class GradeDetailDto
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

    public DateTime NgayCham { get; init; }

    public DateTime? NgayCapNhat { get; init; }
}

// ============================================================================
// BẢNG ĐIỂM TỔNG KẾT
// ============================================================================

/// <summary>Tham số của <c>GET /api/admin/grades/summary</c>.</summary>
public sealed class GradeSummaryQuery
{
    [Range(1, int.MaxValue, ErrorMessage = "Lớp học là bắt buộc.")]
    public int MaLopHoc { get; set; }
}

/// <summary>Bảng điểm tổng kết của một lớp. CHỈ TÍNH, KHÔNG lưu vào database.</summary>
public sealed class GradeSummaryDto
{
    public int MaLopHoc { get; init; }

    public string MaLop { get; init; } = string.Empty;

    public string TenLop { get; init; } = string.Empty;

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    /// <summary>Tổng trọng số của các đầu điểm đang hoạt động.</summary>
    public decimal TongTrongSo { get; init; }

    /// <summary>True nếu tổng trọng số khác 100 — công thức vẫn chạy nhưng kết quả có thể lệch.</summary>
    public bool CanhBaoTrongSo { get; init; }

    public string? CanhBao { get; init; }

    public IReadOnlyList<StudentGradeSummaryDto> HocViens { get; init; } = [];
}

/// <summary>Điểm tổng kết của một học viên.</summary>
public sealed class StudentGradeSummaryDto
{
    public int MaDangKy { get; init; }

    public string MaHocVien { get; init; } = string.Empty;

    public string HoTen { get; init; } = string.Empty;

    /// <summary>Đủ tất cả đầu điểm đang hoạt động hay chưa.</summary>
    public bool DuDieuKien { get; init; }

    /// <summary>Điểm tổng kết thang 10; null nếu còn thiếu đầu điểm.</summary>
    public decimal? DiemTongKet { get; init; }

    public IReadOnlyList<GradeComponentScoreDto> ThanhPhan { get; init; } = [];

    /// <summary>Tên các đầu điểm còn thiếu.</summary>
    public IReadOnlyList<string> ThanhPhanConThieu { get; init; } = [];
}

/// <summary>Điểm của một đầu điểm trong bảng tổng kết.</summary>
public sealed class GradeComponentScoreDto
{
    public int MaLoaiDiem { get; init; }

    public string TenLoaiDiem { get; init; } = string.Empty;

    public decimal TrongSo { get; init; }

    public decimal DiemToiDa { get; init; }

    public decimal? Diem { get; init; }
}

// ============================================================================
// ĐIỂM CỦA CHÍNH HỌC VIÊN
// ============================================================================

/// <summary>Điểm tổng kết của học viên trong một lớp.</summary>
public sealed class StudentCourseGradeDto
{
    public int MaDangKy { get; init; }

    public int MaLopHoc { get; init; }

    public string MaLop { get; init; } = string.Empty;

    public string TenLop { get; init; } = string.Empty;

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    /// <summary>Ngày bắt đầu của lớp.</summary>
    public DateOnly NgayBatDau { get; init; }

    /// <summary>Ngày kết thúc dự kiến của lớp.</summary>
    public DateOnly NgayKetThucDuKien { get; init; }

    /// <summary>Thời điểm học viên được xếp vào lớp (null nếu chưa có).</summary>
    public DateTime? NgayXepLop { get; init; }

    public decimal TongTrongSo { get; init; }

    public bool CanhBaoTrongSo { get; init; }

    public bool DuDieuKien { get; init; }

    public decimal? DiemTongKet { get; init; }

    public IReadOnlyList<GradeComponentScoreDto> ThanhPhan { get; init; } = [];

    public IReadOnlyList<string> ThanhPhanConThieu { get; init; } = [];
}
