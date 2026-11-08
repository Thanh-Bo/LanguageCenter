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

    /// <summary>
    /// Dữ liệu để kiểm tra quy tắc xếp lớp: tuổi học viên vs chương trình, các khung
    /// lịch của lớp mới và của các lớp học viên đang học, số buổi đã diễn ra.
    /// <c>null</c> nếu đăng ký hoặc lớp không tổn tại.
    /// </summary>
    Task<PlacementContext?> GetPlacementContextAsync(
        int maDangKy,
        int maLopHoc,
        CancellationToken cancellationToken);

    /// <summary>
    /// Như <see cref="GetPlacementContextAsync"/> nhưng kiểm tra theo HỌC VIÊN — dùng lúc
    /// TẠO đăng ký, khi đăng ký chưa tồn tại nên chưa có <c>MaDangKy</c>.
    /// <para>
    /// <paramref name="exceptDangKyId"/>: bỏ qua chính đăng ký đang sửa (tránh tự tính mình).
    /// </para>
    /// </summary>
    Task<PlacementContext?> GetPlacementContextForStudentAsync(
        string maHocVien,
        int maLopHoc,
        int? exceptDangKyId,
        CancellationToken cancellationToken);
}

/// <summary>Một khung lịch của lớp: thứ + giờ + khoảng ngày áp dụng.</summary>
public sealed record ClassScheduleSlot(
    int MaLopHoc,
    string MaLop,
    int ThuTrongTuan,
    TimeOnly GioBatDau,
    TimeOnly GioKetThuc,
    DateOnly NgayApDungTu,
    DateOnly? NgayApDungDen);

/// <summary>Dữ liệu thô để kiểm tra quy tắc xếp lớp.</summary>
public sealed record PlacementContext(
    int MaLopHoc,
    string MaLop,
    string TenLop,
    DateOnly NgayBatDauLop,
    DateOnly NgayKetThucDuKien,
    string MaHocVien,
    DateOnly NgaySinh,
    int? DoTuoiTu,
    int? DoTuoiDen,
    int SoBuoiDaDienRa,
    int TongSoBuoi,
    IReadOnlyList<ClassScheduleSlot> LopMoiSlots,
    IReadOnlyList<ClassScheduleSlot> LopDangHocSlots)
{
    /// <summary>
    /// Số tiền CÒN THIẾU của phiếu học phí gắn với đăng ký này.
    /// <c>null</c> = CHƯA có phiếu học phí.
    /// Dùng để chặn xếp lớp khi chưa thanh toán đủ (trung tâm thu tiền trước khi vào học).
    /// </summary>
    public decimal? SoTienConThieuHocPhi { get; init; }

    /// <summary>Trạng thái phiếu học phí (<c>null</c> nếu chưa có phiếu).</summary>
    public string? TrangThaiHocPhi { get; init; }

    /// <summary>Sĩ số tối đa của lớp.</summary>
    public int SiSoToiDa { get; init; }

    /// <summary>Số đăng ký ĐÃ xếp lớp (chiếm chỗ thật).</summary>
    public int SoDaXepLop { get; init; }

    /// <summary>
    /// Số đăng ký đang GIỮ CHỖ cho lớp này (<c>MaLopDuKien = lớp</c>, còn "Chờ xử lý").
    /// Không tính chính đăng ký đang được kiểm tra.
    /// </summary>
    public int SoGiuCho { get; init; }
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
    string? GhiChu)
{
    /// <summary>
    /// LỚP DỰ KIẾN — bắt buộc trong luồng mới: khi thu đủ học phí hệ thống tự xếp vào lớp này.
    /// </summary>
    public int? MaLopDuKien { get; init; }
}

/// <summary>Dữ liệu cập nhật đã được gộp sẵn ở tầng Application.</summary>
public sealed record EnrollmentUpdateData(
    int? MaUuDai,
    decimal SoTienGiam,
    string? LichHocMongMuon,
    string? GhiChu)
{
    /// <summary>Đổi lớp dự kiến. <c>null</c> = giữ nguyên.</summary>
    public int? MaLopDuKien { get; init; }
}

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
