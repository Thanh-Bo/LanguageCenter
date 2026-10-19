using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Classes.Models;
using LanguageCenter.Application.Common;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Classes;

/// <inheritdoc cref="IClassService"/>
public sealed class ClassService : IClassService
{
    private readonly IClassStore _classStore;
    private readonly ICourseStore _courseStore;
    private readonly IReferenceStore _referenceStore;

    public ClassService(
        IClassStore classStore,
        ICourseStore courseStore,
        IReferenceStore referenceStore)
    {
        _classStore = classStore;
        _courseStore = courseStore;
        _referenceStore = referenceStore;
    }

    public async Task<OperationResult<PagedResult<ClassListItemDto>>> ListAsync(
        ClassListQuery query,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(query.TrangThai) && !ClassStatuses.IsValid(query.TrangThai.Trim()))
        {
            return OperationResult<PagedResult<ClassListItemDto>>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", ClassStatuses.All)}."]);
        }

        var page = await _classStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<ClassListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<ClassDetailDto>> GetDetailAsync(
        int maLopHoc,
        CancellationToken cancellationToken)
    {
        var detail = await _classStore.GetDetailAsync(maLopHoc, cancellationToken);

        return detail is null
            ? OperationResult<ClassDetailDto>.NotFound($"Không tìm thấy lớp có id = {maLopHoc}.")
            : OperationResult<ClassDetailDto>.Ok(detail);
    }

    public async Task<OperationResult<ClassDetailDto>> CreateAsync(
        CreateClassRequest request,
        CancellationToken cancellationToken)
    {
        var maLop = request.MaLop.Trim();

        var errors = Validate(
            request.SiSoToiDa,
            request.NgayBatDau,
            request.NgayKetThucDuKien,
            ngayKetThucThucTe: null);

        if (errors.Count > 0)
        {
            return OperationResult<ClassDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        if (!await _courseStore.ExistsAsync(request.MaKhoaHoc, cancellationToken))
        {
            return OperationResult<ClassDetailDto>.BadRequest(
                "Khóa học không tồn tại.",
                [$"Không tìm thấy khóa học có id = {request.MaKhoaHoc}."]);
        }

        var maNhanVien = request.MaNhanVienHocVu.Trim();

        if (!await _referenceStore.StaffExistsAsync(maNhanVien, cancellationToken))
        {
            return OperationResult<ClassDetailDto>.BadRequest(
                "Nhân viên học vụ không tồn tại.",
                [$"Không tìm thấy nhân viên '{maNhanVien}'."]);
        }

        if (await _classStore.CodeExistsAsync(maLop, exceptId: null, cancellationToken))
        {
            return OperationResult<ClassDetailDto>.Conflict($"Mã lớp '{maLop}' đã tồn tại.");
        }

        var result = await _classStore.CreateAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            return OperationResult<ClassDetailDto>.Conflict($"Mã lớp '{maLop}' đã tồn tại.");
        }

        var created = await _classStore.GetDetailAsync(result.Id, cancellationToken);

        return OperationResult<ClassDetailDto>.Created(created!, $"Tạo lớp '{maLop}' thành công.");
    }

    public async Task<OperationResult<ClassDetailDto>> UpdateAsync(
        int maLopHoc,
        UpdateClassRequest request,
        CancellationToken cancellationToken)
    {
        var current = await _classStore.GetDetailAsync(maLopHoc, cancellationToken);

        if (current is null)
        {
            return OperationResult<ClassDetailDto>.NotFound($"Không tìm thấy lớp có id = {maLopHoc}.");
        }

        if (IsEmptyUpdate(request))
        {
            return OperationResult<ClassDetailDto>.BadRequest(
                "Không có trường nào được cập nhật.",
                ["Hãy gửi ít nhất một trường cần thay đổi."]);
        }

        var errors = Validate(
            request.SiSoToiDa ?? current.SiSoToiDa,
            request.NgayBatDau ?? current.NgayBatDau,
            request.NgayKetThucDuKien ?? current.NgayKetThucDuKien,
            request.NgayKetThucThucTe ?? current.NgayKetThucThucTe);

        if (errors.Count > 0)
        {
            return OperationResult<ClassDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        if (request.MaLop is not null)
        {
            var maLop = request.MaLop.Trim();

            if (await _classStore.CodeExistsAsync(maLop, maLopHoc, cancellationToken))
            {
                return OperationResult<ClassDetailDto>.Conflict($"Mã lớp '{maLop}' đã tồn tại.");
            }
        }

        if (request.MaNhanVienHocVu is not null)
        {
            var maNhanVien = request.MaNhanVienHocVu.Trim();

            if (!await _referenceStore.StaffExistsAsync(maNhanVien, cancellationToken))
            {
                return OperationResult<ClassDetailDto>.BadRequest(
                    "Nhân viên học vụ không tồn tại.",
                    [$"Không tìm thấy nhân viên '{maNhanVien}'."]);
            }
        }

        var outcome = await _classStore.UpdateAsync(maLopHoc, request, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return outcome == WriteOutcome.NotFound
                ? OperationResult<ClassDetailDto>.NotFound($"Không tìm thấy lớp có id = {maLopHoc}.")
                : OperationResult<ClassDetailDto>.Conflict("Không thể cập nhật lớp.");
        }

        var updated = await _classStore.GetDetailAsync(maLopHoc, cancellationToken);

        return OperationResult<ClassDetailDto>.Ok(updated!);
    }

    public async Task<OperationResult<ClassDetailDto>> UpdateStatusAsync(
        int maLopHoc,
        UpdateStatusRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = TextValue.Required(request.TrangThai);

        if (trangThai is null || !ClassStatuses.IsValid(trangThai))
        {
            return OperationResult<ClassDetailDto>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", ClassStatuses.All)}."]);
        }

        if (!await _classStore.ExistsAsync(maLopHoc, cancellationToken))
        {
            return OperationResult<ClassDetailDto>.NotFound($"Không tìm thấy lớp có id = {maLopHoc}.");
        }

        var outcome = await _classStore.UpdateStatusAsync(maLopHoc, trangThai, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return OperationResult<ClassDetailDto>.Conflict("Không thể cập nhật trạng thái lớp.");
        }

        var updated = await _classStore.GetDetailAsync(maLopHoc, cancellationToken);

        return OperationResult<ClassDetailDto>.Ok(updated!);
    }

    public async Task<OperationResult<bool>> DeleteAsync(
        int maLopHoc,
        CancellationToken cancellationToken)
    {
        if (!await _classStore.ExistsAsync(maLopHoc, cancellationToken))
        {
            return OperationResult<bool>.NotFound($"Không tìm thấy lớp có id = {maLopHoc}.");
        }

        var check = await _classStore.CheckDeletableAsync(maLopHoc, cancellationToken);

        if (!check.CanDelete)
        {
            return OperationResult<bool>.Conflict(
                $"Không thể xóa lớp vì đang có dữ liệu liên quan ({check.Reason}). " +
                "Hãy dùng chức năng đổi trạng thái sang 'Đã hủy' thay vì xóa.");
        }

        var outcome = await _classStore.DeleteAsync(maLopHoc, cancellationToken);

        return outcome switch
        {
            WriteOutcome.Success => OperationResult<bool>.NoContent(),
            WriteOutcome.NotFound => OperationResult<bool>.NotFound($"Không tìm thấy lớp có id = {maLopHoc}."),
            _ => OperationResult<bool>.Conflict("Không thể xóa lớp do ràng buộc dữ liệu.")
        };
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    private static List<string> Validate(
        int siSoToiDa,
        DateOnly ngayBatDau,
        DateOnly ngayKetThucDuKien,
        DateOnly? ngayKetThucThucTe)
    {
        var errors = new List<string>();

        if (siSoToiDa < 1)
        {
            errors.Add("Sĩ số tối đa phải lớn hơn 0.");
        }

        if (ngayBatDau == default)
        {
            errors.Add("Ngày bắt đầu là bắt buộc.");
        }

        if (ngayKetThucDuKien == default)
        {
            errors.Add("Ngày kết thúc dự kiến là bắt buộc.");
        }

        if (ngayBatDau != default && ngayKetThucDuKien != default && ngayKetThucDuKien < ngayBatDau)
        {
            errors.Add("Ngày kết thúc dự kiến phải lớn hơn hoặc bằng ngày bắt đầu.");
        }

        if (ngayKetThucThucTe is { } thucTe && ngayBatDau != default && thucTe < ngayBatDau)
        {
            errors.Add("Ngày kết thúc thực tế phải lớn hơn hoặc bằng ngày bắt đầu.");
        }

        return errors;
    }

    private static bool IsEmptyUpdate(UpdateClassRequest request)
        => request.MaLop is null
           && request.TenLop is null
           && request.MaNhanVienHocVu is null
           && request.NgayBatDau is null
           && request.NgayKetThucDuKien is null
           && request.NgayKetThucThucTe is null
           && request.SiSoToiDa is null;
}
