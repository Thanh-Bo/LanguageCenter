using LanguageCenter.Application.Common;
using LanguageCenter.Application.Enrollments.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>Cổng truy cập dữ liệu đăng ký học (<c>DangKyHoc</c>).</summary>
public interface IEnrollmentStore
{
    Task<PagedResult<EnrollmentListItemDto>> ListAsync(
        EnrollmentListQuery query,
        CancellationToken cancellationToken);

    Task<EnrollmentDetailDto?> GetDetailAsync(int maDangKy, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int maDangKy, CancellationToken cancellationToken);

    /// <summary>
    /// Đã có đăng ký CÒN HIỆU LỰC ("Chờ xử lý" / "Đã xếp lớp") của cùng học viên + khóa học chưa.
    /// </summary>
    Task<bool> HasActiveDuplicateAsync(
        string maHocVien,
        int maKhoaHoc,
        int? exceptId,
        CancellationToken cancellationToken);

    Task<WriteResult> CreateAsync(EnrollmentCreateData data, CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateAsync(
        int maDangKy,
        EnrollmentUpdateData changes,
        CancellationToken cancellationToken);

    /// <summary>
    /// Xếp lớp NGUYÊN TỬ: khóa dòng lớp, đếm sĩ số, cập nhật. Chống vượt sĩ số khi đồng thời.
    /// </summary>
    Task<AssignClassOutcome> AssignClassAsync(
        int maDangKy,
        int maLopHoc,
        CancellationToken cancellationToken);

    /// <summary>Bỏ lớp: xóa lớp đã gán, trả đăng ký về trạng thái "Chờ xử lý".</summary>
    Task<WriteOutcome> UnassignClassAsync(int maDangKy, CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateStatusAsync(
        int maDangKy,
        string trangThai,
        CancellationToken cancellationToken);

    /// <summary>Xóa cứng — chỉ dùng để hoàn tác khi xếp lớp thất bại ngay sau khi tạo.</summary>
    Task<WriteOutcome> DeleteAsync(int maDangKy, CancellationToken cancellationToken);
}

/// <summary>Dữ liệu đã được tầng Application TÍNH SẴN (học phí, giảm giá, người tư vấn).</summary>
public sealed record EnrollmentCreateData(
    string MaHocVien,
    int MaKhoaHoc,
    int? MaUuDai,
    string MaNhanVienTuVan,
    decimal HocPhiGoc,
    decimal SoTienGiam,
    string? LichHocMongMuon,
    string? GhiChu);

/// <summary>Dữ liệu cập nhật đã được gộp sẵn ở tầng Application.</summary>
public sealed record EnrollmentUpdateData(
    int? MaUuDai,
    decimal SoTienGiam,
    string? LichHocMongMuon,
    string? GhiChu);

/// <summary>Kết quả của thao tác xếp lớp.</summary>
public enum AssignClassOutcome
{
    Success = 0,
    EnrollmentNotFound = 1,
    ClassNotFound = 2,
    WrongCourse = 3,
    ClassClosed = 4,
    ClassFull = 5,
    AlreadyAssigned = 6,
    Conflict = 7
}
