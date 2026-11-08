using LanguageCenter.Application.Common;
using LanguageCenter.Application.Employees.Models;

namespace LanguageCenter.Application.Employees;

/// <summary>Nghiệp vụ quản lý NHÂN VIÊN (học vụ / tư vấn / kế toán).</summary>
public interface IEmployeeService
{
    /// <summary>
    /// Tạo nhân viên: backend tự sinh mã theo chức vụ (NVHV/NVTV/NVKT), tự tạo
    /// <c>TaiKhoan</c> + <c>NhanVien</c> trong MỘT transaction, tự sinh mật khẩu tạm
    /// và chỉ trả về ĐÚNG MỘT LẦN.
    /// </summary>
    Task<OperationResult<CreateEmployeeResponse>> CreateAsync(
        CreateEmployeeRequest request,
        CancellationToken cancellationToken);

    /// <summary>Danh sách nhân viên — phân trang, tìm kiếm, lọc chức vụ / trạng thái.</summary>
    Task<OperationResult<PagedResult<EmployeeListItemDto>>> ListAsync(
        EmployeeListQuery query,
        CancellationToken cancellationToken);

    /// <summary>Chi tiết nhân viên.</summary>
    Task<OperationResult<EmployeeDetailDto>> GetDetailAsync(
        string maNhanVien,
        CancellationToken cancellationToken);

    /// <summary>Cập nhật hồ sơ nhân viên (không đổi được chức vụ).</summary>
    Task<OperationResult<EmployeeDetailDto>> UpdateAsync(
        string maNhanVien,
        UpdateEmployeeRequest request,
        CancellationToken cancellationToken);

    /// <summary>Xoá cứng nhân viên + tài khoản — chỉ khi không có dữ liệu nghiệp vụ phụ thuộc.</summary>
    Task<OperationResult<bool>> DeleteAsync(string maNhanVien, CancellationToken cancellationToken);
}
