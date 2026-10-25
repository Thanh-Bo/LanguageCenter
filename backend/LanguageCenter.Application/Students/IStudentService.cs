using LanguageCenter.Application.Common;
using LanguageCenter.Application.Students.Models;

namespace LanguageCenter.Application.Students;

/// <summary>Use case học viên: Admin quản lý + học viên tự xem/sửa hồ sơ.</summary>
public interface IStudentService
{
    // ---------- Admin ----------

    Task<OperationResult<CreateStudentResponse>> CreateAsync(CreateStudentRequest request, CancellationToken cancellationToken);

    Task<OperationResult<PagedResult<StudentListItemDto>>> ListAsync(StudentListQuery query, CancellationToken cancellationToken);

    Task<OperationResult<StudentDetailDto>> GetDetailAsync(string maHocVien, CancellationToken cancellationToken);

    Task<OperationResult<StudentDetailDto>> UpdateAsync(string maHocVien, UpdateStudentRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Xóa cứng học viên + tài khoản — CHỈ khi không có dữ liệu nghiệp vụ phụ thuộc.
    /// Nếu đã có đăng ký học / điểm danh / học phí..., trả 409 và hướng dẫn đổi trạng thái sang "Đã nghỉ".
    /// </summary>
    Task<OperationResult<bool>> DeleteAsync(string maHocVien, CancellationToken cancellationToken);

    // ---------- Self profile ----------

    /// <summary>
    /// Hồ sơ của CHÍNH học viên đang đăng nhập.
    /// <paramref name="tenDangNhap"/> lấy từ JWT — không bao giờ lấy từ query/path/body.
    /// </summary>
    Task<OperationResult<StudentDetailDto>> GetMyProfileAsync(string tenDangNhap, CancellationToken cancellationToken);

    Task<OperationResult<StudentDetailDto>> UpdateMyProfileAsync(
        string tenDangNhap,
        UpdateMyStudentProfileRequest request,
        CancellationToken cancellationToken);
}
