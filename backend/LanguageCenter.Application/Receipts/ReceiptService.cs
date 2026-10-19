using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Receipts.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Receipts;

/// <inheritdoc cref="IReceiptService"/>
/// <remarks>
/// Biên nhận KHÔNG có endpoint tạo/sửa/xoá — luôn được sinh tự động trong cùng
/// transaction với giao dịch thanh toán. Quan hệ <c>BienNhan.MaThanhToan</c> là UNIQUE
/// nên về mặt dữ liệu không thể có hai biên nhận cho một giao dịch.
/// </remarks>
public sealed class ReceiptService : IReceiptService
{
    private readonly IReceiptStore _receiptStore;

    public ReceiptService(IReceiptStore receiptStore)
    {
        _receiptStore = receiptStore;
    }

    public async Task<OperationResult<PagedResult<ReceiptDto>>> ListAsync(
        ReceiptListQuery query,
        CancellationToken cancellationToken)
    {
        var invalid = ValidateFilter(query);

        if (invalid is not null)
        {
            return invalid;
        }

        var page = await _receiptStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<ReceiptDto>>.Ok(page);
    }

    public async Task<OperationResult<ReceiptDto>> GetDetailAsync(
        int maBienNhan,
        CancellationToken cancellationToken)
    {
        var detail = await _receiptStore.GetDetailAsync(maBienNhan, cancellationToken);

        return detail is null
            ? OperationResult<ReceiptDto>.NotFound($"Không tìm thấy biên nhận có id = {maBienNhan}.")
            : OperationResult<ReceiptDto>.Ok(detail);
    }

    public async Task<OperationResult<ReceiptDto>> GetByNumberAsync(
        string soBienNhan,
        CancellationToken cancellationToken)
    {
        var code = TextValue.Required(soBienNhan);

        if (code is null)
        {
            return OperationResult<ReceiptDto>.BadRequest(
                "Số biên nhận là bắt buộc.",
                ["Ví dụ: BN20261018001."]);
        }

        var detail = await _receiptStore.GetByNumberAsync(code, cancellationToken);

        return detail is null
            ? OperationResult<ReceiptDto>.NotFound($"Không tìm thấy biên nhận số '{code}'.")
            : OperationResult<ReceiptDto>.Ok(detail);
    }

    public async Task<OperationResult<PagedResult<ReceiptDto>>> ListForStudentAsync(
        string maHocVien,
        ReceiptListQuery query,
        CancellationToken cancellationToken)
    {
        query.MaHocVien = maHocVien;

        var invalid = ValidateFilter(query);

        if (invalid is not null)
        {
            return invalid;
        }

        var page = await _receiptStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<ReceiptDto>>.Ok(page);
    }

    public async Task<OperationResult<ReceiptDto>> GetDetailForStudentAsync(
        int maBienNhan,
        string maHocVien,
        CancellationToken cancellationToken)
    {
        var detail = await _receiptStore.GetDetailAsync(maBienNhan, cancellationToken);

        if (detail is null || !string.Equals(detail.MaHocVien, maHocVien, StringComparison.Ordinal))
        {
            return OperationResult<ReceiptDto>.NotFound($"Không tìm thấy biên nhận có id = {maBienNhan}.");
        }

        return OperationResult<ReceiptDto>.Ok(detail);
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    private static OperationResult<PagedResult<ReceiptDto>>? ValidateFilter(ReceiptListQuery query)
    {
        if (!string.IsNullOrWhiteSpace(query.TrangThai)
            && !ReceiptStatuses.IsValid(query.TrangThai.Trim()))
        {
            return OperationResult<PagedResult<ReceiptDto>>.BadRequest(
                "Trạng thái biên nhận không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", ReceiptStatuses.All)}."]);
        }

        if (query.TuNgay is { } tu && query.DenNgay is { } den && den < tu)
        {
            return OperationResult<PagedResult<ReceiptDto>>.BadRequest(
                "Khoảng ngày không hợp lệ.",
                ["Đến ngày phải lớn hơn hoặc bằng từ ngày."]);
        }

        return null;
    }
}
