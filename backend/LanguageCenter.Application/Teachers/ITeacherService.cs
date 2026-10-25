using LanguageCenter.Application.Common;
using LanguageCenter.Application.Teachers.Models;

namespace LanguageCenter.Application.Teachers;

/// <summary>Use case giáo viên/trợ giảng: Admin quản lý + giáo viên tự xem/sửa hồ sơ.</summary>
public interface ITeacherService
{
    // ---------- Admin ----------

    Task<OperationResult<CreateTeacherResponse>> CreateAsync(CreateTeacherRequest request, CancellationToken cancellationToken);

    Task<OperationResult<PagedResult<TeacherListItemDto>>> ListAsync(TeacherListQuery query, CancellationToken cancellationToken);

    Task<OperationResult<TeacherDetailDto>> GetDetailAsync(string maGiaoVien, CancellationToken cancellationToken);

    Task<OperationResult<TeacherDetailDto>> UpdateAsync(string maGiaoVien, UpdateTeacherRequest request, CancellationToken cancellationToken);

    /// <summary>
    /// Xóa cứng giáo viên / trợ giảng + tài khoản — CHỈ khi không có dữ liệu nghiệp vụ phụ thuộc
    /// (phân công giảng dạy, ca học, điểm đã chấm...). Nếu có, trả 409 và hướng dẫn đổi trạng thái.
    /// </summary>
    Task<OperationResult<bool>> DeleteAsync(string maGiaoVien, CancellationToken cancellationToken);

    // ---------- Self profile ----------

    /// <summary>
    /// Hồ sơ của CHÍNH giáo viên/trợ giảng đang đăng nhập.
    /// <paramref name="tenDangNhap"/> lấy từ JWT — không bao giờ lấy từ query/path/body.
    /// </summary>
    Task<OperationResult<TeacherDetailDto>> GetMyProfileAsync(string tenDangNhap, CancellationToken cancellationToken);

    Task<OperationResult<TeacherDetailDto>> UpdateMyProfileAsync(
        string tenDangNhap,
        UpdateMyTeacherProfileRequest request,
        CancellationToken cancellationToken);
}
