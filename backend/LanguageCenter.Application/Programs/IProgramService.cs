using LanguageCenter.Application.Common;
using LanguageCenter.Application.Programs.Models;

namespace LanguageCenter.Application.Programs;

/// <summary>Quản lý chương trình đào tạo. Controller chỉ gọi interface này.</summary>
public interface IProgramService
{
    Task<OperationResult<PagedResult<ProgramListItemDto>>> ListAsync(
        ProgramListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<ProgramDetailDto>> GetDetailAsync(
        int maChuongTrinh,
        CancellationToken cancellationToken);

    Task<OperationResult<ProgramDetailDto>> CreateAsync(
        CreateProgramRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<ProgramDetailDto>> UpdateAsync(
        int maChuongTrinh,
        UpdateProgramRequest request,
        CancellationToken cancellationToken);

    /// <summary>Đổi trạng thái: "Hoạt động" / "Ngừng hoạt động". KHÔNG cascade xuống cấp độ/khóa học.</summary>
    Task<OperationResult<ProgramDetailDto>> UpdateStatusAsync(
        int maChuongTrinh,
        UpdateStatusRequest request,
        CancellationToken cancellationToken);
}
