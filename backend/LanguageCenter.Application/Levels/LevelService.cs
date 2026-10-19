using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Levels.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Levels;

/// <inheritdoc cref="ILevelService"/>
public sealed class LevelService : ILevelService
{
    private readonly ILevelStore _levelStore;
    private readonly IProgramStore _programStore;

    public LevelService(ILevelStore levelStore, IProgramStore programStore)
    {
        _levelStore = levelStore;
        _programStore = programStore;
    }

    public async Task<OperationResult<PagedResult<LevelListItemDto>>> ListAsync(
        LevelListQuery query,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(query.TrangThai) && !ActivityStatuses.IsValid(query.TrangThai.Trim()))
        {
            return OperationResult<PagedResult<LevelListItemDto>>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", ActivityStatuses.All)}."]);
        }

        if (query.MaChuongTrinh is { } maChuongTrinh
            && !await _programStore.ExistsAsync(maChuongTrinh, cancellationToken))
        {
            return OperationResult<PagedResult<LevelListItemDto>>.BadRequest(
                "Chương trình không tồn tại.",
                [$"Không tìm thấy chương trình có id = {maChuongTrinh}."]);
        }

        var page = await _levelStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<LevelListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<LevelDetailDto>> GetDetailAsync(
        int maCapDo,
        CancellationToken cancellationToken)
    {
        var detail = await _levelStore.GetDetailAsync(maCapDo, cancellationToken);

        return detail is null
            ? OperationResult<LevelDetailDto>.NotFound($"Không tìm thấy cấp độ có id = {maCapDo}.")
            : OperationResult<LevelDetailDto>.Ok(detail);
    }

    public async Task<OperationResult<LevelDetailDto>> CreateAsync(
        CreateLevelRequest request,
        CancellationToken cancellationToken)
    {
        var errors = Validate(request.ThuTu, request.ThoiLuongDuKienTuan);

        if (errors.Count > 0)
        {
            return OperationResult<LevelDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        // Chương trình cha PHẢI tồn tại (FK của database sẽ chặn, nhưng trả 400 dễ hiểu hơn 500).
        if (!await _programStore.ExistsAsync(request.MaChuongTrinh, cancellationToken))
        {
            return OperationResult<LevelDetailDto>.BadRequest(
                "Chương trình không tồn tại.",
                [$"Không tìm thấy chương trình có id = {request.MaChuongTrinh}."]);
        }

        var ten = request.TenCapDo.Trim();

        if (await _levelStore.NameExistsAsync(request.MaChuongTrinh, ten, exceptId: null, cancellationToken))
        {
            return OperationResult<LevelDetailDto>.Conflict(
                $"Tên cấp độ '{ten}' đã tồn tại trong chương trình này.");
        }

        if (await _levelStore.OrderExistsAsync(request.MaChuongTrinh, request.ThuTu, exceptId: null, cancellationToken))
        {
            return OperationResult<LevelDetailDto>.Conflict(
                $"Thứ tự {request.ThuTu} đã được dùng bởi cấp độ khác trong chương trình này.");
        }

        var result = await _levelStore.CreateAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            return OperationResult<LevelDetailDto>.Conflict(
                "Không thể tạo cấp độ: tên hoặc thứ tự đã tồn tại trong chương trình này.");
        }

        var created = await _levelStore.GetDetailAsync(result.Id, cancellationToken);

        return OperationResult<LevelDetailDto>.Created(
            created!,
            $"Tạo cấp độ '{ten}' thành công.");
    }

    public async Task<OperationResult<LevelDetailDto>> UpdateAsync(
        int maCapDo,
        UpdateLevelRequest request,
        CancellationToken cancellationToken)
    {
        var current = await _levelStore.GetDetailAsync(maCapDo, cancellationToken);

        if (current is null)
        {
            return OperationResult<LevelDetailDto>.NotFound($"Không tìm thấy cấp độ có id = {maCapDo}.");
        }

        if (IsEmptyUpdate(request))
        {
            return OperationResult<LevelDetailDto>.BadRequest(
                "Không có trường nào được cập nhật.",
                ["Hãy gửi ít nhất một trường cần thay đổi."]);
        }

        var errors = Validate(request.ThuTu ?? current.ThuTu, request.ThoiLuongDuKienTuan);

        if (errors.Count > 0)
        {
            return OperationResult<LevelDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        if (request.TenCapDo is not null)
        {
            var ten = request.TenCapDo.Trim();

            if (await _levelStore.NameExistsAsync(current.MaChuongTrinh, ten, maCapDo, cancellationToken))
            {
                return OperationResult<LevelDetailDto>.Conflict(
                    $"Tên cấp độ '{ten}' đã tồn tại trong chương trình này.");
            }
        }

        if (request.ThuTu is { } thuTu
            && await _levelStore.OrderExistsAsync(current.MaChuongTrinh, thuTu, maCapDo, cancellationToken))
        {
            return OperationResult<LevelDetailDto>.Conflict(
                $"Thứ tự {thuTu} đã được dùng bởi cấp độ khác trong chương trình này.");
        }

        var outcome = await _levelStore.UpdateAsync(maCapDo, request, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return outcome == WriteOutcome.NotFound
                ? OperationResult<LevelDetailDto>.NotFound($"Không tìm thấy cấp độ có id = {maCapDo}.")
                : OperationResult<LevelDetailDto>.Conflict("Không thể cập nhật cấp độ.");
        }

        var updated = await _levelStore.GetDetailAsync(maCapDo, cancellationToken);

        return OperationResult<LevelDetailDto>.Ok(updated!);
    }

    public async Task<OperationResult<LevelDetailDto>> UpdateStatusAsync(
        int maCapDo,
        UpdateStatusRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = TextValue.Required(request.TrangThai);

        if (trangThai is null || !ActivityStatuses.IsValid(trangThai))
        {
            return OperationResult<LevelDetailDto>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", ActivityStatuses.All)}."]);
        }

        if (!await _levelStore.ExistsAsync(maCapDo, cancellationToken))
        {
            return OperationResult<LevelDetailDto>.NotFound($"Không tìm thấy cấp độ có id = {maCapDo}.");
        }

        var outcome = await _levelStore.UpdateStatusAsync(maCapDo, trangThai, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return OperationResult<LevelDetailDto>.Conflict("Không thể cập nhật trạng thái cấp độ.");
        }

        var updated = await _levelStore.GetDetailAsync(maCapDo, cancellationToken);

        return OperationResult<LevelDetailDto>.Ok(updated!);
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    private static List<string> Validate(int thuTu, int? thoiLuongTuan)
    {
        var errors = new List<string>();

        if (thuTu < 1)
        {
            errors.Add("Thứ tự phải lớn hơn 0.");
        }

        if (thoiLuongTuan is < 1)
        {
            errors.Add("Thời lượng dự kiến phải lớn hơn 0 tuần.");
        }

        return errors;
    }

    private static bool IsEmptyUpdate(UpdateLevelRequest request)
        => request.KyHieuCapDo is null
           && request.TenCapDo is null
           && request.ThuTu is null
           && request.DieuKienDauVao is null
           && request.KetQuaDauRa is null
           && request.ThoiLuongDuKienTuan is null;
}
