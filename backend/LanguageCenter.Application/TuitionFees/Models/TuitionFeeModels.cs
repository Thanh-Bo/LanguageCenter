using System.ComponentModel.DataAnnotations;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.TuitionFees.Models;

// ============================================================================
// REQUEST
// ============================================================================

/// <summary>
/// Body của <c>POST /api/admin/tuition-fees</c>.
/// <para>
/// CHỈ nhận <c>maDangKy</c> — số tiền được COPY từ <c>DangKyHoc.HocPhiGoc</c> /
/// <c>DangKyHoc.SoTienGiam</c> (nguồn chính thức), KHÔNG lấy từ client.
/// </para>
/// </summary>
public sealed class CreateTuitionFeeRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Đăng ký học là bắt buộc.")]
    public int MaDangKy { get; set; }

    /// <summary>Hạn thanh toán — để trống nếu không đặt hạn.</summary>
    public DateOnly? HanThanhToan { get; set; }
}

/// <summary>
/// Body của <c>PATCH /api/admin/tuition-fees/{id}</c> — mọi field OPTIONAL.
/// <para>
/// KHÔNG có trường số tiền do client gửi. Việc đồng bộ tiền chỉ thực hiện được qua
/// <see cref="DongBoTuDangKy"/> và chỉ khi học phí CHƯA đóng đồng nào.
/// </para>
/// </summary>
public sealed class UpdateTuitionFeeRequest
{
    public DateOnly? HanThanhToan { get; set; }

    /// <summary>Gửi <c>true</c> để xóa hạn thanh toán (đặt về NULL).</summary>
    public bool XoaHanThanhToan { get; set; }

    /// <summary>
    /// Gửi <c>true</c> để copy lại <c>HocPhiGoc</c>/<c>SoTienGiam</c> từ <c>DangKyHoc</c>
    /// (dùng khi ưu đãi của đăng ký bị sửa sau khi đã tạo học phí).
    /// </summary>
    public bool DongBoTuDangKy { get; set; }
}

/// <summary>Tham số lọc danh sách học phí.</summary>
public sealed class TuitionFeeListQuery : PagedQuery
{
    public string? MaHocVien { get; set; }

    public int? MaKhoaHoc { get; set; }

    public int? MaLopHoc { get; set; }

    /// <summary>"Chưa thanh toán" / "Thanh toán một phần" / "Đã thanh toán".</summary>
    public string? TrangThai { get; set; }

    /// <summary><c>true</c> = chỉ lấy học phí quá hạn; <c>false</c> = chỉ lấy chưa quá hạn.</summary>
    public bool? QuaHan { get; set; }

    /// <summary>Lọc theo <c>NgayTao</c>.</summary>
    public DateOnly? TuNgay { get; set; }

    public DateOnly? DenNgay { get; set; }

    /// <summary>Tìm theo mã học viên hoặc họ tên học viên.</summary>
    public string? Search { get; set; }
}

/// <summary>Tham số báo cáo học phí.</summary>
public sealed class TuitionReportQuery
{
    /// <summary>Lọc theo <c>HocPhi.NgayTao</c>.</summary>
    public DateOnly? TuNgay { get; set; }

    public DateOnly? DenNgay { get; set; }

    public int? MaKhoaHoc { get; set; }
}

// ============================================================================
// RESPONSE
// ============================================================================

/// <summary>Một dòng trong danh sách học phí.</summary>
public sealed class TuitionFeeListItemDto
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

    /// <summary>Cột computed PERSISTED <c>SoTienGoc - SoTienGiam</c>.</summary>
    public decimal SoTienPhaiDong { get; init; }

    public decimal SoTienDaDong { get; init; }

    /// <summary>Cột computed PERSISTED — DB tự tính, không bao giờ ghi từ code.</summary>
    public decimal SoTienConThieu { get; init; }

    public DateOnly? HanThanhToan { get; init; }

    public string TrangThaiHocPhi { get; init; } = string.Empty;

    public DateTime NgayTao { get; init; }

    /// <summary>Số giao dịch "Thành công" loại "Thu tiền" của học phí này.</summary>
    public int SoGiaoDich { get; init; }

    /// <summary>
    /// SUY RA, không lưu trong database: còn thiếu tiền VÀ đã quá <c>HanThanhToan</c>.
    /// </summary>
    public bool QuaHan
        => SoTienConThieu > 0m
           && HanThanhToan.HasValue
           && HanThanhToan.Value < DateOnly.FromDateTime(DateTime.Today);

    /// <summary>SUY RA: còn nợ hay không.</summary>
    public bool ConNo => SoTienConThieu > 0m;
}

/// <summary>Chi tiết một nghĩa vụ học phí.</summary>
public sealed class TuitionFeeDetailDto
{
    public int MaHocPhi { get; init; }

    public int MaDangKy { get; init; }

    public string MaHocVien { get; init; } = string.Empty;

    public string HoTenHocVien { get; init; } = string.Empty;

    public string? SoDienThoaiHocVien { get; init; }

    public string? EmailHocVien { get; init; }

    public int MaKhoaHoc { get; init; }

    public string TenKhoaHoc { get; init; } = string.Empty;

    public int? MaLopHoc { get; init; }

    public string? MaLop { get; init; }

    public string? TenLop { get; init; }

    public string TrangThaiDangKy { get; init; } = string.Empty;

    public decimal SoTienGoc { get; init; }

    public decimal SoTienGiam { get; init; }

    public decimal SoTienPhaiDong { get; init; }

    public decimal SoTienDaDong { get; init; }

    public decimal SoTienConThieu { get; init; }

    public DateOnly? HanThanhToan { get; init; }

    public string TrangThaiHocPhi { get; init; } = string.Empty;

    public DateTime NgayTao { get; init; }

    public int SoGiaoDich { get; init; }

    public bool QuaHan
        => SoTienConThieu > 0m
           && HanThanhToan.HasValue
           && HanThanhToan.Value < DateOnly.FromDateTime(DateTime.Today);

    public bool ConNo => SoTienConThieu > 0m;
}

// ============================================================================
// BÁO CÁO
// ============================================================================

/// <summary>Báo cáo học phí tổng hợp.</summary>
public sealed class TuitionSummaryReportDto
{
    public int TongSoHocPhi { get; init; }

    /// <summary>Σ <c>SoTienPhaiDong</c> (cột computed — nguồn chuẩn).</summary>
    public decimal TongPhaiThu { get; init; }

    /// <summary>Σ <c>SoTienDaDong</c> (cột chuẩn, đã đồng bộ theo giao dịch).</summary>
    public decimal TongDaThu { get; init; }

    /// <summary>Σ <c>SoTienConThieu</c> (cột computed).</summary>
    public decimal TongConThieu { get; init; }

    public int SoChuaThanhToan { get; init; }

    public int SoThanhToanMotPhan { get; init; }

    public int SoDaThanhToan { get; init; }

    /// <summary>Số học phí còn nợ VÀ đã quá hạn thanh toán.</summary>
    public int SoQuaHan { get; init; }

    /// <summary>Số học viên còn nợ (khác nhau).</summary>
    public int SoHocVienConNo { get; init; }

    public IReadOnlyList<TuitionDebtorDto> DanhSachConNo { get; init; } = [];

    public IReadOnlyList<PaymentStatDto> ThongKeGiaoDichTheoPhuongThuc { get; init; } = [];

    public IReadOnlyList<PaymentStatDto> ThongKeGiaoDichTheoTrangThai { get; init; } = [];
}

/// <summary>Một học viên còn nợ học phí.</summary>
public sealed class TuitionDebtorDto
{
    public string MaHocVien { get; init; } = string.Empty;

    public string HoTen { get; init; } = string.Empty;

    public int SoHocPhiConNo { get; init; }

    public decimal TongConThieu { get; init; }
}

/// <summary>Một nhóm thống kê giao dịch.</summary>
public sealed class PaymentStatDto
{
    public string Nhan { get; init; } = string.Empty;

    public int SoLuong { get; init; }

    public decimal TongTien { get; init; }
}
