using LanguageCenter.Application.Common;
using LanguageCenter.Application.Students.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>
/// Cổng truy cập dữ liệu học viên. Triển khai ở Infrastructure (<c>StudentStore</c>) bằng EF Core.
/// <para>
/// Method <c>UpdateAsync</c> PHẢI đảm bảo đồng bộ email giữa <c>HocVien.Email</c> và
/// <c>TaiKhoan.Email</c> trong CÙNG một transaction (§26).
/// </para>
/// </summary>
public interface IStudentStore
{
    /// <summary>Lấy chi tiết học viên theo mã (kèm phụ huynh).</summary>
    Task<StudentDetailDto?> GetByCodeAsync(string maHocVien, CancellationToken cancellationToken);

    /// <summary>Danh sách học viên có phân trang / tìm kiếm — thực hiện ở tầng SQL.</summary>
    Task<PagedResult<StudentListItemDto>> ListAsync(StudentListQuery query, CancellationToken cancellationToken);

    /// <summary>
    /// Cập nhật hồ sơ học viên. Field null trong <paramref name="changes"/> = không đổi.
    /// Nếu <c>Email</c> thay đổi thì cập nhật cả <c>HocVien.Email</c> và <c>TaiKhoan.Email</c>.
    /// </summary>
    Task<WriteOutcome> UpdateAsync(string maHocVien, UpdateStudentRequest changes, CancellationToken cancellationToken);
}
