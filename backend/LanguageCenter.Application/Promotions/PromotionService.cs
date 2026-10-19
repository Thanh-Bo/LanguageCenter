using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Promotions.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Promotions;

/// <inheritdoc cref="IPromotionService"/>
public sealed class PromotionService : IPromotionService
{
    private readonly IPromotionStore _promotionStore;

    public PromotionService(IPromotionStore promotionStore)
    {
        _promotionStore = promotionStore;
    }

    public async Task<OperationResult<PagedResult<PromotionListItemDto>>> ListAsync(
        PromotionListQuery query,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(query.TrangThai) && !ActivityStatuses.IsValid(query.TrangThai.Trim()))
        {
            return OperationResult<PagedResult<PromotionListItemDto>>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", ActivityStatuses.All)}."]);
        }

        var page = await _promotionStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<PromotionListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<PromotionDetailDto>> GetDetailAsync(
        int maUuDai,
        CancellationToken cancellationToken)
    {
        var detail = await _promotionStore.GetDetailAsync(maUuDai, cancellationToken);

        return detail is null
            ? OperationResult<PromotionDetailDto>.NotFound($"Không tìm thấy ưu đãi có id = {maUuDai}.")
            : OperationResult<PromotionDetailDto>.Ok(detail);
    }

    public async Task<OperationResult<PromotionDetailDto>> CreateAsync(
        CreatePromotionRequest request,
        CancellationToken cancellationToken)
    {
        var loai = TextValue.Required(request.LoaiGiamGia);

        var errors = Validate(
            loai,
            request.GiaTriGiam,
            request.SoKhoaHocToiThieu,
            request.NgayBatDau,
            request.NgayKetThuc);

        if (errors.Count > 0)
        {
            return OperationResult<PromotionDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        var result = await _promotionStore.CreateAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            return OperationResult<PromotionDetailDto>.Conflict("Không thể tạo ưu đãi.");
        }

        var created = await _promotionStore.GetDetailAsync(result.Id, cancellationToken);

        return OperationResult<PromotionDetailDto>.Created(
            created!,
            $"Tạo ưu đãi '{request.TenUuDai.Trim()}' thành công.");
    }

    public async Task<OperationResult<PromotionDetailDto>> UpdateAsync(
        int maUuDai,
        UpdatePromotionRequest request,
        CancellationToken cancellationToken)
    {
        var current = await _promotionStore.GetDetailAsync(maUuDai, cancellationToken);

        if (current is null)
        {
            return OperationResult<PromotionDetailDto>.NotFound($"Không tìm thấy ưu đãi có id = {maUuDai}.");
        }

        if (IsEmptyUpdate(request))
        {
            return OperationResult<PromotionDetailDto>.BadRequest(
                "Không có trường nào được cập nhật.",
                ["Hãy gửi ít nhất một trường cần thay đổi."]);
        }

        // Validate trên GIÁ TRỊ SAU KHI GỘP — đổi Loại giảm giá có thể làm giá trị cũ không còn hợp lệ.
        var loai = request.LoaiGiamGia is not null ? TextValue.Required(request.LoaiGiamGia) : current.LoaiGiamGia;

        var errors = Validate(
            loai,
            request.GiaTriGiam ?? current.GiaTriGiam,
            request.SoKhoaHocToiThieu ?? current.SoKhoaHocToiThieu,
            request.NgayBatDau ?? current.NgayBatDau,
            request.NgayKetThuc ?? current.NgayKetThuc);

        if (errors.Count > 0)
        {
            return OperationResult<PromotionDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        var outcome = await _promotionStore.UpdateAsync(maUuDai, request, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return outcome == WriteOutcome.NotFound
                ? OperationResult<PromotionDetailDto>.NotFound($"Không tìm thấy ưu đãi có id = {maUuDai}.")
                : OperationResult<PromotionDetailDto>.Conflict("Không thể cập nhật ưu đãi.");
        }

        var updated = await _promotionStore.GetDetailAsync(maUuDai, cancellationToken);

        return OperationResult<PromotionDetailDto>.Ok(updated!);
    }

    public async Task<OperationResult<PromotionDetailDto>> UpdateStatusAsync(
        int maUuDai,
        UpdateStatusRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = TextValue.Required(request.TrangThai);

        if (trangThai is null || !ActivityStatuses.IsValid(trangThai))
        {
            return OperationResult<PromotionDetailDto>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", ActivityStatuses.All)}."]);
        }

        if (!await _promotionStore.ExistsAsync(maUuDai, cancellationToken))
        {
            return OperationResult<PromotionDetailDto>.NotFound($"Không tìm thấy ưu đãi có id = {maUuDai}.");
        }

        var outcome = await _promotionStore.UpdateStatusAsync(maUuDai, trangThai, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return OperationResult<PromotionDetailDto>.Conflict("Không thể cập nhật trạng thái ưu đãi.");
        }

        var updated = await _promotionStore.GetDetailAsync(maUuDai, cancellationToken);

        return OperationResult<PromotionDetailDto>.Ok(updated!);
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    /// <summary>Khớp CHECK constraint CK_UuDai_Loai / CK_UuDai_GiaTri / CK_UuDai_Ngay.</summary>
    private static List<string> Validate(
        string? loaiGiamGia,
        decimal giaTriGiam,
        int soKhoaHocToiThieu,
        DateOnly ngayBatDau,
        DateOnly ngayKetThuc)
    {
        var errors = new List<string>();

        if (!PromotionTypes.IsValid(loaiGiamGia))
        {
            errors.Add($"Loại giảm giá không hợp lệ. Chỉ chấp nhận: {string.Join(", ", PromotionTypes.All)}.");
        }

        if (giaTriGiam <= 0)
        {
            errors.Add("Giá trị giảm phải lớn hơn 0.");
        }

        if (PromotionTypes.IsPercent(loaiGiamGia) && giaTriGiam > 100)
        {
            errors.Add("Giảm theo phần trăm không được vượt quá 100.");
        }

        if (soKhoaHocToiThieu < 1)
        {
            errors.Add("Số khóa học tối thiểu phải lớn hơn 0.");
        }

        if (ngayBatDau == default)
        {
            errors.Add("Ngày bắt đầu là bắt buộc.");
        }

        if (ngayKetThuc == default)
        {
            errors.Add("Ngày kết thúc là bắt buộc.");
        }

        if (ngayBatDau != default && ngayKetThuc != default && ngayKetThuc < ngayBatDau)
        {
            errors.Add("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");
        }

        return errors;
    }

    private static bool IsEmptyUpdate(UpdatePromotionRequest request)
        => request.TenUuDai is null
           && request.LoaiGiamGia is null
           && request.GiaTriGiam is null
           && request.SoKhoaHocToiThieu is null
           && request.NgayBatDau is null
           && request.NgayKetThuc is null
           && request.Poster is null
           && request.MoTa is null;
}
