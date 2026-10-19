using LanguageCenter.Application.Common;
using LanguageCenter.Application.Promotions.Models;

namespace LanguageCenter.Application.Promotions;

/// <summary>Quản lý danh mục ưu đãi.</summary>
public interface IPromotionService
{
    Task<OperationResult<PagedResult<PromotionListItemDto>>> ListAsync(
        PromotionListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<PromotionDetailDto>> GetDetailAsync(
        int maUuDai,
        CancellationToken cancellationToken);

    Task<OperationResult<PromotionDetailDto>> CreateAsync(
        CreatePromotionRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<PromotionDetailDto>> UpdateAsync(
        int maUuDai,
        UpdatePromotionRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<PromotionDetailDto>> UpdateStatusAsync(
        int maUuDai,
        UpdateStatusRequest request,
        CancellationToken cancellationToken);
}
