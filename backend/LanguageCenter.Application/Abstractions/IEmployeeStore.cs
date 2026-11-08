using LanguageCenter.Application.Common;
using LanguageCenter.Application.Employees.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>
/// Cổng truy cập dữ liệu NHÂN VIÊN (bảng <c>NhanVien</c>). Triển khai ở Infrastructure
/// (<c>EmployeeStore</c>).
/// <para>
/// <c>UpdateAsync</c> PHẢI đồng bộ email giữa <c>NhanVien.Email</c> và <c>TaiKhoan.Email</c>
/// trong CÙNG một lần SaveChanges.
/// </para>
/// </summary>
public interface IEmployeeStore
{
    /// <summary>Danh sách nhân viên có phân trang / tìm kiếm / lọc — thực hiện ở tầng SQL.</summary>
    Task<PagedResult<EmployeeListItemDto>> ListAsync(EmployeeListQuery query, CancellationToken cancellationToken);

    /// <summary>Chi tiết nhân viên theo mã; null nếu không tồn tại.</summary>
    Task<EmployeeDetailDto?> GetByCodeAsync(string maNhanVien, CancellationToken cancellationToken);

    /// <summary>Số điện thoại đã dùng cho hồ sơ khác chưa (nhân viên + giáo viên + học viên + phụ huynh).</summary>
    Task<bool> PhoneExistsAsync(
        string soDienThoai,
        string? exceptMaNhanVien,
        CancellationToken cancellationToken);

    /// <summary>Cập nhật hồ sơ nhân viên. Field null = không đổi.</summary>
    Task<WriteOutcome> UpdateAsync(
        string maNhanVien,
        UpdateEmployeeRequest changes,
        CancellationToken cancellationToken);
}
