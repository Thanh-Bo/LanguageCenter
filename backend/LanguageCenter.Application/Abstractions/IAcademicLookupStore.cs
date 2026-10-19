namespace LanguageCenter.Application.Abstractions;

/// <summary>
/// Cổng tra cứu dùng chung cho Phase 5 (lịch học, ca học, điểm danh, điểm số).
/// <para>
/// Tách riêng để KHÔNG phải sửa <see cref="IReferenceStore"/> của Phase 4.
/// Mọi method chỉ đọc, không ghi.
/// </para>
/// </summary>
public interface IAcademicLookupStore
{
    /// <summary>Phòng học có tồn tại không.</summary>
    Task<bool> RoomExistsAsync(int maPhongHoc, CancellationToken cancellationToken);

    Task<bool> CourseExistsAsync(int maKhoaHoc, CancellationToken cancellationToken);

    /// <summary>Thông tin tối thiểu của lớp; <c>null</c> nếu không tồn tại.</summary>
    Task<ClassInfo?> GetClassInfoAsync(int maLopHoc, CancellationToken cancellationToken);

    /// <summary>
    /// Giáo viên / trợ giảng có phân công đang hiệu lực cho lớp không
    /// (<c>PhanCongGiangDay.TrangThai = "Đang phân công"</c>).
    /// </summary>
    Task<bool> TeacherAssignedToClassAsync(
        int maLopHoc,
        string maGiaoVien,
        CancellationToken cancellationToken);

    /// <summary>Học viên đã được xếp vào lớp chưa (đăng ký "Đã xếp lớp" hoặc "Hoàn thành").</summary>
    Task<bool> StudentInClassAsync(
        int maLopHoc,
        string maHocVien,
        CancellationToken cancellationToken);

    /// <summary>Danh sách học viên đang chiếm chỗ trong lớp (kèm họ tên).</summary>
    Task<IReadOnlyList<ClassStudentInfo>> GetClassStudentsAsync(
        int maLopHoc,
        CancellationToken cancellationToken);

    /// <summary>Các lớp mà học viên đang theo học.</summary>
    Task<IReadOnlyList<int>> GetStudentClassIdsAsync(
        string maHocVien,
        CancellationToken cancellationToken);

    /// <summary>Các lớp mà giáo viên/trợ giảng đang được phân công giảng dạy.</summary>
    Task<IReadOnlyList<int>> GetTeacherClassIdsAsync(
        string maGiaoVien,
        CancellationToken cancellationToken);

    /// <summary>Tài khoản (<c>TaiKhoan.MaTaiKhoan</c>) có tồn tại không.</summary>
    Task<bool> TaiKhoanExistsAsync(int maTaiKhoan, CancellationToken cancellationToken);

    /// <summary>Lớp học có tồn tại không.</summary>
    Task<bool> ClassExistsAsync(int maLopHoc, CancellationToken cancellationToken);

    /// <summary>Thông tin đăng ký học; <c>null</c> nếu không tồn tại.</summary>
    Task<EnrollmentInfo?> GetEnrollmentInfoAsync(int maDangKy, CancellationToken cancellationToken);

    /// <summary>Giáo viên / trợ giảng có phân công hiệu lực ở lớp chứa đăng ký này không.</summary>
    Task<bool> TeacherAssignedToEnrollmentClassAsync(
        int maDangKy,
        string maGiaoVien,
        CancellationToken cancellationToken);
}

/// <summary>Thông tin tối thiểu của lớp học.</summary>
public sealed record ClassInfo(
    int MaLopHoc,
    string MaLop,
    string TenLop,
    int MaKhoaHoc,
    string TrangThai,
    string MaNhanVienHocVu);

/// <summary>Học viên trong lớp.</summary>
public sealed record ClassStudentInfo(
    string MaHocVien,
    string HoTen,
    int MaDangKy);

/// <summary>Thông tin tối thiểu của một đăng ký học.</summary>
public sealed record EnrollmentInfo(
    int MaDangKy,
    string MaHocVien,
    int MaKhoaHoc,
    int? MaLopHoc,
    string TrangThaiDangKy);
