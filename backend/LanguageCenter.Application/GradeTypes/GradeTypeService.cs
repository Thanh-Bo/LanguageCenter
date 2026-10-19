using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.GradeTypes.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.GradeTypes;

/// <inheritdoc cref="IGradeTypeService"/>
/// <remarks>
/// GIỚI HẠN CỦA SCHEMA: <c>LoaiDiem</c> có ĐỒNG THỜI
/// <c>UNIQUE (MaKhoaHoc, TenLoaiDiem)</c> và <c>UNIQUE (MaKhoaHoc, ThuTu)</c>,
/// nên đổi <c>ThuTu</c> rất dễ vướng 409. Hãy đổi theo thứ tự tránh trùng.
/// </remarks>
public sealed class GradeTypeService : IGradeTypeService
{
    private readonly IGradeTypeStore _gradeTypeStore;
    private readonly IAcademicLookupStore _lookupStore;

    public GradeTypeService(IGradeTypeStore gradeTypeStore, IAcademicLookupStore lookupStore)
    {
        _gradeTypeStore = gradeTypeStore;
        _lookupStore = lookupStore;
    }

    public async Task<OperationResult<PagedResult<GradeTypeListItemDto>>> ListAsync(
        GradeTypeListQuery query,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(query.TrangThai)
            && !GradeTypeStatuses.IsValid(query.TrangThai.Trim()))
        {
            return OperationResult<PagedResult<GradeTypeListItemDto>>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", GradeTypeStatuses.All)}."]);
        }

        var page = await _gradeTypeStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<GradeTypeListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<GradeTypeDetailDto>> GetDetailAsync(
        int maLoaiDiem,
        CancellationToken cancellationToken)
    {
        var detail = await _gradeTypeStore.GetDetailAsync(maLoaiDiem, cancellationToken);

        return detail is null
            ? OperationResult<GradeTypeDetailDto>.NotFound(
                $"Không tìm thấy đầu điểm có id = {maLoaiDiem}.")
            : OperationResult<GradeTypeDetailDto>.Ok(detail);
    }

    public async Task<OperationResult<GradeTypeDetailDto>> CreateAsync(
        CreateGradeTypeRequest request,
        CancellationToken cancellationToken)
    {
        var tenLoaiDiem = TextValue.Required(request.TenLoaiDiem);

        var errors = Validate(tenLoaiDiem, request.TrongSo, request.DiemToiDa, request.ThuTu);

        if (errors.Count > 0)
        {
            return OperationResult<GradeTypeDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        if (!await _lookupStore.CourseExistsAsync(request.MaKhoaHoc, cancellationToken))
        {
            return OperationResult<GradeTypeDetailDto>.BadRequest(
                "Khóa học không tồn tại.",
                [$"Không tìm thấy khóa học có id = {request.MaKhoaHoc}."]);
        }

        if (await _gradeTypeStore.NameTakenAsync(
                request.MaKhoaHoc, tenLoaiDiem!, exceptId: null, cancellationToken))
        {
            return OperationResult<GradeTypeDetailDto>.Conflict(
                $"Khóa học đã có đầu điểm tên '{tenLoaiDiem}'.");
        }

        if (await _gradeTypeStore.OrderTakenAsync(
                request.MaKhoaHoc, request.ThuTu, exceptId: null, cancellationToken))
        {
            return OperationResult<GradeTypeDetailDto>.Conflict(
                $"Khóa học đã có đầu điểm ở thứ tự {request.ThuTu}.");
        }

        var result = await _gradeTypeStore.CreateAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            return OperationResult<GradeTypeDetailDto>.Conflict("Không thể tạo đầu điểm.");
        }

        var created = await _gradeTypeStore.GetDetailAsync(result.Id, cancellationToken);

        return OperationResult<GradeTypeDetailDto>.Created(created!, "Tạo đầu điểm thành công.");
    }

    public async Task<OperationResult<GradeTypeDetailDto>> UpdateAsync(
        int maLoaiDiem,
        UpdateGradeTypeRequest request,
        CancellationToken cancellationToken)
    {
        var current = await _gradeTypeStore.GetDetailAsync(maLoaiDiem, cancellationToken);

        if (current is null)
        {
            return OperationResult<GradeTypeDetailDto>.NotFound(
                $"Không tìm thấy đầu điểm có id = {maLoaiDiem}.");
        }

        if (request.TenLoaiDiem is null
            && request.TrongSo is null
            && request.DiemToiDa is null
            && request.ThuTu is null)
        {
            return OperationResult<GradeTypeDetailDto>.BadRequest(
                "Không có trường nào được cập nhật.",
                ["Hãy gửi ít nhất một trường cần thay đổi."]);
        }

        var tenLoaiDiem = request.TenLoaiDiem is null
            ? current.TenLoaiDiem
            : TextValue.Required(request.TenLoaiDiem);

        var trongSo = request.TrongSo ?? current.TrongSo;
        var diemToiDa = request.DiemToiDa ?? current.DiemToiDa;
        var thuTu = request.ThuTu ?? current.ThuTu;

        var errors = Validate(tenLoaiDiem, trongSo, diemToiDa, thuTu);

        if (errors.Count > 0)
        {
            return OperationResult<GradeTypeDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        if (request.TenLoaiDiem is not null
            && await _gradeTypeStore.NameTakenAsync(
                current.MaKhoaHoc, tenLoaiDiem!, maLoaiDiem, cancellationToken))
        {
            return OperationResult<GradeTypeDetailDto>.Conflict(
                $"Khóa học đã có đầu điểm tên '{tenLoaiDiem}'.");
        }

        if (request.ThuTu is not null
            && await _gradeTypeStore.OrderTakenAsync(
                current.MaKhoaHoc, thuTu, maLoaiDiem, cancellationToken))
        {
            return OperationResult<GradeTypeDetailDto>.Conflict(
                $"Khóa học đã có đầu điểm ở thứ tự {thuTu}.");
        }

        var outcome = await _gradeTypeStore.UpdateAsync(maLoaiDiem, request, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return outcome == WriteOutcome.NotFound
                ? OperationResult<GradeTypeDetailDto>.NotFound(
                    $"Không tìm thấy đầu điểm có id = {maLoaiDiem}.")
                : OperationResult<GradeTypeDetailDto>.Conflict("Không thể cập nhật đầu điểm.");
        }

        var updated = await _gradeTypeStore.GetDetailAsync(maLoaiDiem, cancellationToken);

        return OperationResult<GradeTypeDetailDto>.Ok(updated!);
    }

    public async Task<OperationResult<GradeTypeDetailDto>> UpdateStatusAsync(
        int maLoaiDiem,
        UpdateStatusRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = TextValue.Required(request.TrangThai);

        if (trangThai is null || !GradeTypeStatuses.IsValid(trangThai))
        {
            return OperationResult<GradeTypeDetailDto>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", GradeTypeStatuses.All)}."]);
        }

        if (!await _gradeTypeStore.ExistsAsync(maLoaiDiem, cancellationToken))
        {
            return OperationResult<GradeTypeDetailDto>.NotFound(
                $"Không tìm thấy đầu điểm có id = {maLoaiDiem}.");
        }

        var outcome = await _gradeTypeStore.UpdateStatusAsync(maLoaiDiem, trangThai, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return OperationResult<GradeTypeDetailDto>.Conflict("Không thể cập nhật trạng thái đầu điểm.");
        }

        var updated = await _gradeTypeStore.GetDetailAsync(maLoaiDiem, cancellationToken);

        return OperationResult<GradeTypeDetailDto>.Ok(updated!);
    }

    public async Task<OperationResult<bool>> DeleteAsync(int maLoaiDiem, CancellationToken cancellationToken)
    {
        if (!await _gradeTypeStore.ExistsAsync(maLoaiDiem, cancellationToken))
        {
            return OperationResult<bool>.NotFound($"Không tìm thấy đầu điểm có id = {maLoaiDiem}.");
        }

        var gradeCount = await _gradeTypeStore.CountGradesAsync(maLoaiDiem, cancellationToken);

        if (gradeCount > 0)
        {
            return OperationResult<bool>.Conflict(
                $"Không thể xóa đầu điểm vì đã có {gradeCount} bản ghi điểm số tham chiếu. " +
                "Hãy chuyển trạng thái sang 'Ngừng sử dụng'.");
        }

        var outcome = await _gradeTypeStore.DeleteAsync(maLoaiDiem, cancellationToken);

        return outcome switch
        {
            WriteOutcome.Success => OperationResult<bool>.NoContent(),
            WriteOutcome.NotFound => OperationResult<bool>.NotFound(
                $"Không tìm thấy đầu điểm có id = {maLoaiDiem}."),
            _ => OperationResult<bool>.Conflict("Không thể xóa đầu điểm do ràng buộc dữ liệu.")
        };
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    private static List<string> Validate(
        string? tenLoaiDiem,
        decimal trongSo,
        decimal diemToiDa,
        int thuTu)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(tenLoaiDiem))
        {
            errors.Add("Tên đầu điểm là bắt buộc.");
        }
        else if (tenLoaiDiem.Trim().Length is < 2 or > 150)
        {
            errors.Add("Tên đầu điểm phải từ 2 đến 150 ký tự.");
        }

        if (trongSo is < 0m or > 100m)
        {
            errors.Add("Trọng số phải trong khoảng 0..100.");
        }

        if (diemToiDa <= 0m)
        {
            errors.Add("Điểm tối đa phải lớn hơn 0.");
        }

        if (thuTu <= 0)
        {
            errors.Add("Thứ tự phải lớn hơn 0.");
        }

        return errors;
    }
}
