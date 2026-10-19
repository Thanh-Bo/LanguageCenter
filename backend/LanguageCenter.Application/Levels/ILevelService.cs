using LanguageCenter.Application.Common;
using LanguageCenter.Application.Levels.Models;

namespace LanguageCenter.Application.Levels;

/// <summary>Quản lý cấp độ trong chương trình đào tạo.</summary>
public interface ILevelService
{
    Task<OperationResult<PagedResult<LevelListItemDto>>> ListAsync(
        LevelListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<LevelDetailDto>> GetDetailAsync(
        int maCapDo,
        CancellationToken cancellationToken);

    Task<OperationResult<LevelDetailDto>> CreateAsync(
        CreateLevelRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<LevelDetailDto>> UpdateAsync(
        int maCapDo,
        UpdateLevelRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<LevelDetailDto>> UpdateStatusAsync(
        int maCapDo,
        UpdateStatusRequest request,
        CancellationToken cancellationToken);
}
