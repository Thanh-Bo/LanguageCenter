using LanguageCenter.Application.Common;
using LanguageCenter.Application.Promotions.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>Cổng truy cập dữ liệu ưu đãi (<c>UuDai</c>).</summary>
public interface IPromotionStore
{
    Task<PagedResult<PromotionListItemDto>> ListAsync(PromotionListQuery query, CancellationToken cancellationToken);

    Task<PromotionDetailDto?> GetDetailAsync(int maUuDai, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int maUuDai, CancellationToken cancellationToken);

    Task<WriteResult> CreateAsync(CreatePromotionRequest request, CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateAsync(int maUuDai, UpdatePromotionRequest changes, CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateStatusAsync(int maUuDai, string trangThai, CancellationToken cancellationToken);

    /// <summary>Thông tin ưu đãi để tầng Application tự tính giảm giá (KHÔNG tin client).</summary>
    Task<PromotionForApply?> GetForApplyAsync(int maUuDai, CancellationToken cancellationToken);
}

/// <summary>Dữ liệu ưu đãi cần cho việc áp dụng khi tạo đăng ký.</summary>
public sealed record PromotionForApply(
    int MaUuDai,
    string TenUuDai,
    string LoaiGiamGia,
    decimal GiaTriGiam,
    int SoKhoaHocToiThieu,
    DateOnly NgayBatDau,
    DateOnly NgayKetThuc,
    string TrangThai);
