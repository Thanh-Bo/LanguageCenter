using LanguageCenter.Application.Classes.Models;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Classes;

/// <summary>Quản lý lớp học.</summary>
public interface IClassService
{
    Task<OperationResult<PagedResult<ClassListItemDto>>> ListAsync(
        ClassListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<ClassDetailDto>> GetDetailAsync(
        int maLopHoc,
        CancellationToken cancellationToken);

    Task<OperationResult<ClassDetailDto>> CreateAsync(
        CreateClassRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<ClassDetailDto>> UpdateAsync(
        int maLopHoc,
        UpdateClassRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<ClassDetailDto>> UpdateStatusAsync(
        int maLopHoc,
        UpdateStatusRequest request,
        CancellationToken cancellationToken);

    /// <summary>Xóa cứng — CHỈ khi lớp chưa có đăng ký / phân công / lịch / ca học.</summary>
    Task<OperationResult<bool>> DeleteAsync(
        int maLopHoc,
        CancellationToken cancellationToken);
}
