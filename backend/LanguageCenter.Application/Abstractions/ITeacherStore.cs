using LanguageCenter.Application.Common;
using LanguageCenter.Application.Teachers.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>
/// Cổng truy cập dữ liệu giáo viên / trợ giảng. Triển khai ở Infrastructure (<c>TeacherStore</c>).
/// <para>
/// Method <c>UpdateAsync</c> PHẢI đảm bảo đồng bộ email giữa <c>GiaoVien.Email</c> và
/// <c>TaiKhoan.Email</c> trong CÙNG một transaction (§26).
/// </para>
/// </summary>
public interface ITeacherStore
{
    /// <summary>Lấy chi tiết giáo viên theo mã (kèm Bằng cấp + Chứng chỉ — chỉ đọc ở Phase 3).</summary>
    Task<TeacherDetailDto?> GetByCodeAsync(string maGiaoVien, CancellationToken cancellationToken);

    /// <summary>Danh sách giáo viên có phân trang / tìm kiếm / lọc — thực hiện ở tầng SQL.</summary>
    Task<PagedResult<TeacherListItemDto>> ListAsync(TeacherListQuery query, CancellationToken cancellationToken);

    /// <summary>
    /// Cập nhật hồ sơ giáo viên. Field null trong <paramref name="changes"/> = không đổi.
    /// Nếu <c>Email</c> thay đổi thì cập nhật cả <c>GiaoVien.Email</c> và <c>TaiKhoan.Email</c>.
    /// </summary>
    Task<WriteOutcome> UpdateAsync(string maGiaoVien, UpdateTeacherRequest changes, CancellationToken cancellationToken);
}
