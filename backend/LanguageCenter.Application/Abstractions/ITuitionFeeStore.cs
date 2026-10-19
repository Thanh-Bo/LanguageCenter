using LanguageCenter.Application.Common;
using LanguageCenter.Application.TuitionFees.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>Cổng truy cập nghĩa vụ học phí (<c>HocPhi</c>) — quan hệ 1-1 với <c>DangKyHoc</c>.</summary>
public interface ITuitionFeeStore
{
    Task<PagedResult<TuitionFeeListItemDto>> ListAsync(
        TuitionFeeListQuery query,
        CancellationToken cancellationToken);

    Task<TuitionFeeDetailDto?> GetDetailAsync(int maHocPhi, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int maHocPhi, CancellationToken cancellationToken);

    /// <summary>Mã học phí đã tồn tại cho đăng ký này chưa (<c>MaDangKy</c> là UNIQUE).</summary>
    Task<int?> FindByEnrollmentAsync(int maDangKy, CancellationToken cancellationToken);

    /// <summary>Ảnh chụp số tiền CHÍNH THỨC từ bảng đăng ký học.</summary>
    Task<EnrollmentTuitionSnapshot?> GetEnrollmentSnapshotAsync(
        int maDangKy,
        CancellationToken cancellationToken);

    Task<WriteResult> CreateAsync(
        EnrollmentTuitionSnapshot snapshot,
        DateOnly? hanThanhToan,
        CancellationToken cancellationToken);

    /// <summary>
    /// Cập nhật hạn thanh toán và/hoặc đồng bộ số tiền từ đăng ký.
    /// <para>KHÔNG bao giờ ghi vào <c>SoTienDaDong</c> hay hai cột computed.</para>
    /// </summary>
    Task<WriteOutcome> UpdateAsync(
        int maHocPhi,
        TuitionFeeUpdateData changes,
        CancellationToken cancellationToken);

    Task<TuitionSummaryReportDto> GetSummaryReportAsync(
        TuitionReportQuery query,
        CancellationToken cancellationToken);
}

/// <summary>Ảnh chụp số tiền của một đăng ký học — nguồn duy nhất để tạo nghĩa vụ học phí.</summary>
public sealed record EnrollmentTuitionSnapshot(
    int MaDangKy,
    string MaHocVien,
    int MaKhoaHoc,
    int? MaLopHoc,
    string TrangThaiDangKy,
    decimal HocPhiGoc,
    decimal SoTienGiam);

/// <summary>Dữ liệu cập nhật học phí đã được gộp và kiểm tra ở tầng Application.</summary>
public sealed record TuitionFeeUpdateData(
    bool CapNhatHanThanhToan,
    DateOnly? HanThanhToan,
    bool CapNhatSoTien,
    decimal SoTienGoc,
    decimal SoTienGiam);
