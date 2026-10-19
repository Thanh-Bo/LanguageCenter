using LanguageCenter.Application.Common;
using LanguageCenter.Application.Receipts.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>Cổng truy cập biên nhận (<c>BienNhan</c>) — chỉ đọc trong Phase 6.</summary>
public interface IReceiptStore
{
    Task<PagedResult<ReceiptDto>> ListAsync(
        ReceiptListQuery query,
        CancellationToken cancellationToken);

    Task<ReceiptDto?> GetDetailAsync(int maBienNhan, CancellationToken cancellationToken);

    Task<ReceiptDto?> GetByNumberAsync(string soBienNhan, CancellationToken cancellationToken);
}
