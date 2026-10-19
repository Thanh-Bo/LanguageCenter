using LanguageCenter.Application.Common;
using LanguageCenter.Application.Receipts.Models;

namespace LanguageCenter.Application.Receipts;

/// <summary>Tra cứu biên nhận — CHỈ ĐỌC. Biên nhận được tạo tự động khi ghi nhận thanh toán.</summary>
public interface IReceiptService
{
    Task<OperationResult<PagedResult<ReceiptDto>>> ListAsync(
        ReceiptListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<ReceiptDto>> GetDetailAsync(
        int maBienNhan,
        CancellationToken cancellationToken);

    Task<OperationResult<ReceiptDto>> GetByNumberAsync(
        string soBienNhan,
        CancellationToken cancellationToken);

    // ------------------------------------------------------------------
    // HỌC VIÊN
    // ------------------------------------------------------------------

    Task<OperationResult<PagedResult<ReceiptDto>>> ListForStudentAsync(
        string maHocVien,
        ReceiptListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<ReceiptDto>> GetDetailForStudentAsync(
        int maBienNhan,
        string maHocVien,
        CancellationToken cancellationToken);
}
