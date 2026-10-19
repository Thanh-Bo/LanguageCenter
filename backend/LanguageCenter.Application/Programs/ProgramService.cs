using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Programs.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Programs;

/// <inheritdoc cref="IProgramService"/>
/// <remarks>
/// Toàn bộ nghiệp vụ chương trình nằm ở đây. Chỉ phụ thuộc abstraction,
/// KHÔNG phụ thuộc EF Core hay ASP.NET Core.
/// </remarks>
public sealed class ProgramService : IProgramService
{
    private readonly IProgramStore _programStore;

    public ProgramService(IProgramStore programStore)
    {
        _programStore = programStore;
    }

    public async Task<OperationResult<PagedResult<ProgramListItemDto>>> ListAsync(
        ProgramListQuery query,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(query.TrangThai) && !ActivityStatuses.IsValid(query.TrangThai.Trim()))
        {
            return OperationResult<PagedResult<ProgramListItemDto>>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", ActivityStatuses.All)}."]);
        }

        var page = await _programStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<ProgramListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<ProgramDetailDto>> GetDetailAsync(
        int maChuongTrinh,
        CancellationToken cancellationToken)
    {
        var detail = await _programStore.GetDetailAsync(maChuongTrinh, cancellationToken);

        return detail is null
            ? OperationResult<ProgramDetailDto>.NotFound($"Không tìm thấy chương trình có id = {maChuongTrinh}.")
            : OperationResult<ProgramDetailDto>.Ok(detail);
    }

    public async Task<OperationResult<ProgramDetailDto>> CreateAsync(
        CreateProgramRequest request,
        CancellationToken cancellationToken)
    {
        var errors = ValidateCreate(request);

        if (errors.Count > 0)
        {
            return OperationResult<ProgramDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        var ten = request.TenChuongTrinh.Trim();

        if (await _programStore.NameExistsAsync(ten, exceptId: null, cancellationToken))
        {
            return OperationResult<ProgramDetailDto>.Conflict($"Tên chương trình '{ten}' đã tồn tại.");
        }

        var result = await _programStore.CreateAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            return OperationResult<ProgramDetailDto>.Conflict($"Tên chương trình '{ten}' đã tồn tại.");
        }

        var created = await _programStore.GetDetailAsync(result.Id, cancellationToken);

        return OperationResult<ProgramDetailDto>.Created(
            created!,
            $"Tạo chương trình '{ten}' thành công.");
    }

    public async Task<OperationResult<ProgramDetailDto>> UpdateAsync(
        int maChuongTrinh,
        UpdateProgramRequest request,
        CancellationToken cancellationToken)
    {
        var current = await _programStore.GetDetailAsync(maChuongTrinh, cancellationToken);

        if (current is null)
        {
            return OperationResult<ProgramDetailDto>.NotFound($"Không tìm thấy chương trình có id = {maChuongTrinh}.");
        }

        if (IsEmptyUpdate(request))
        {
            return OperationResult<ProgramDetailDto>.BadRequest(
                "Không có trường nào được cập nhật.",
                ["Hãy gửi ít nhất một trường cần thay đổi."]);
        }

        // Kiểm tra độ tuổi trên GIÁ TRỊ SAU KHI GỘP (giá trị cũ + thay đổi).
        var errors = ValidateAgeRange(
            request.DoTuoiTu ?? current.DoTuoiTu,
            request.DoTuoiDen ?? current.DoTuoiDen);

        if (errors.Count > 0)
        {
            return OperationResult<ProgramDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        if (request.TenChuongTrinh is not null)
        {
            var ten = request.TenChuongTrinh.Trim();

            if (await _programStore.NameExistsAsync(ten, exceptId: maChuongTrinh, cancellationToken))
            {
                return OperationResult<ProgramDetailDto>.Conflict($"Tên chương trình '{ten}' đã tồn tại.");
            }
        }

        var outcome = await _programStore.UpdateAsync(maChuongTrinh, request, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return outcome == WriteOutcome.NotFound
                ? OperationResult<ProgramDetailDto>.NotFound($"Không tìm thấy chương trình có id = {maChuongTrinh}.")
                : OperationResult<ProgramDetailDto>.Conflict("Không thể cập nhật chương trình.");
        }

        var updated = await _programStore.GetDetailAsync(maChuongTrinh, cancellationToken);

        return OperationResult<ProgramDetailDto>.Ok(updated!);
    }

    public async Task<OperationResult<ProgramDetailDto>> UpdateStatusAsync(
        int maChuongTrinh,
        UpdateStatusRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = TextValue.Required(request.TrangThai);

        if (trangThai is null || !ActivityStatuses.IsValid(trangThai))
        {
            return OperationResult<ProgramDetailDto>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", ActivityStatuses.All)}."]);
        }

        if (!await _programStore.ExistsAsync(maChuongTrinh, cancellationToken))
        {
            return OperationResult<ProgramDetailDto>.NotFound($"Không tìm thấy chương trình có id = {maChuongTrinh}.");
        }

        var outcome = await _programStore.UpdateStatusAsync(maChuongTrinh, trangThai, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return OperationResult<ProgramDetailDto>.Conflict("Không thể cập nhật trạng thái chương trình.");
        }

        var updated = await _programStore.GetDetailAsync(maChuongTrinh, cancellationToken);

        return OperationResult<ProgramDetailDto>.Ok(updated!);
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    private static List<string> ValidateCreate(CreateProgramRequest request)
    {
        var errors = ValidateAgeRange(request.DoTuoiTu, request.DoTuoiDen);

        return errors;
    }

    private static List<string> ValidateAgeRange(int? tu, int? den)
    {
        var errors = new List<string>();

        if (tu is < 0)
        {
            errors.Add("Độ tuổi từ không được âm.");
        }

        if (den is < 0)
        {
            errors.Add("Độ tuổi đến không được âm.");
        }

        if (tu is not null && den is not null && den < tu)
        {
            errors.Add("Độ tuổi đến phải lớn hơn hoặc bằng độ tuổi từ.");
        }

        return errors;
    }

    private static bool IsEmptyUpdate(UpdateProgramRequest request)
        => request.TenChuongTrinh is null
           && request.NhomDoiTuong is null
           && request.DoTuoiTu is null
           && request.DoTuoiDen is null
           && request.MucTieu is null
           && request.MoTa is null;
}
