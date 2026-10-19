using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Schedules.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Schedules;

/// <inheritdoc cref="IScheduleService"/>
/// <remarks>
/// GIỚI HẠN: <c>LichHoc</c> không lưu giáo viên, nên xung đột GIÁO VIÊN không kiểm tra trực tiếp
/// trên bảng này. Xung đột giáo viên được kiểm tra ở <c>CaHoc</c> (nơi có người dạy thực tế).
/// </remarks>
public sealed class ScheduleService : IScheduleService
{
    private readonly IScheduleStore _scheduleStore;
    private readonly IAcademicLookupStore _lookupStore;

    public ScheduleService(IScheduleStore scheduleStore, IAcademicLookupStore lookupStore)
    {
        _scheduleStore = scheduleStore;
        _lookupStore = lookupStore;
    }

    public async Task<OperationResult<PagedResult<ScheduleListItemDto>>> ListAsync(
        ScheduleListQuery query,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(query.TrangThai)
            && !ScheduleStatuses.IsValid(query.TrangThai.Trim()))
        {
            return OperationResult<PagedResult<ScheduleListItemDto>>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", ScheduleStatuses.All)}."]);
        }

        if (query.ThuTrongTuan is { } thu && thu is < 1 or > 7)
        {
            return OperationResult<PagedResult<ScheduleListItemDto>>.BadRequest(
                "Thứ trong tuần không hợp lệ.",
                ["Thứ trong tuần phải từ 1 (Thứ Hai) đến 7 (Chủ Nhật)."]);
        }

        var page = await _scheduleStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<ScheduleListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<ScheduleDetailDto>> GetDetailAsync(
        int maLichHoc,
        CancellationToken cancellationToken)
    {
        var detail = await _scheduleStore.GetDetailAsync(maLichHoc, cancellationToken);

        return detail is null
            ? OperationResult<ScheduleDetailDto>.NotFound($"Không tìm thấy lịch học có id = {maLichHoc}.")
            : OperationResult<ScheduleDetailDto>.Ok(detail);
    }

    public async Task<OperationResult<ScheduleDetailDto>> CreateAsync(
        CreateScheduleRequest request,
        CancellationToken cancellationToken)
    {
        var errors = Validate(
            request.ThuTrongTuan,
            request.GioBatDau,
            request.GioKetThuc,
            request.NgayApDungTu,
            request.NgayApDungDen);

        if (errors.Count > 0)
        {
            return OperationResult<ScheduleDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        if (await _lookupStore.GetClassInfoAsync(request.MaLopHoc, cancellationToken) is null)
        {
            return OperationResult<ScheduleDetailDto>.BadRequest(
                "Lớp học không tồn tại.",
                [$"Không tìm thấy lớp có id = {request.MaLopHoc}."]);
        }

        if (!await _lookupStore.RoomExistsAsync(request.MaPhongHoc, cancellationToken))
        {
            return OperationResult<ScheduleDetailDto>.BadRequest(
                "Phòng học không tồn tại.",
                [$"Không tìm thấy phòng có id = {request.MaPhongHoc}."]);
        }

        if (await _scheduleStore.HasRoomConflictAsync(
                request.MaPhongHoc,
                request.ThuTrongTuan,
                request.GioBatDau,
                request.GioKetThuc,
                request.NgayApDungTu,
                request.NgayApDungDen,
                exceptId: null,
                cancellationToken))
        {
            return OperationResult<ScheduleDetailDto>.Conflict(
                "Phòng học đã được dùng bởi lịch khác cùng thứ, trùng giờ và chồng lấn khoảng ngày áp dụng.");
        }

        var result = await _scheduleStore.CreateAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            return OperationResult<ScheduleDetailDto>.Conflict("Không thể tạo lịch học.");
        }

        var created = await _scheduleStore.GetDetailAsync(result.Id, cancellationToken);

        return OperationResult<ScheduleDetailDto>.Created(created!, "Tạo lịch học thành công.");
    }

    public async Task<OperationResult<ScheduleDetailDto>> UpdateAsync(
        int maLichHoc,
        UpdateScheduleRequest request,
        CancellationToken cancellationToken)
    {
        var current = await _scheduleStore.GetDetailAsync(maLichHoc, cancellationToken);

        if (current is null)
        {
            return OperationResult<ScheduleDetailDto>.NotFound($"Không tìm thấy lịch học có id = {maLichHoc}.");
        }

        if (IsEmptyUpdate(request))
        {
            return OperationResult<ScheduleDetailDto>.BadRequest(
                "Không có trường nào được cập nhật.",
                ["Hãy gửi ít nhất một trường cần thay đổi."]);
        }

        // Validate trên GIÁ TRỊ SAU KHI GỘP (có thể chỉ đổi 1 trong 2 mốc giờ/ngày).
        var thu = request.ThuTrongTuan ?? current.ThuTrongTuan;
        var gioBatDau = request.GioBatDau ?? current.GioBatDau;
        var gioKetThuc = request.GioKetThuc ?? current.GioKetThuc;
        var ngayTu = request.NgayApDungTu ?? current.NgayApDungTu;
        var ngayDen = request.NgayApDungDen ?? current.NgayApDungDen;
        var maPhongHoc = request.MaPhongHoc ?? current.MaPhongHoc;

        var errors = Validate(thu, gioBatDau, gioKetThuc, ngayTu, ngayDen);

        if (errors.Count > 0)
        {
            return OperationResult<ScheduleDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        if (request.MaPhongHoc is not null
            && !await _lookupStore.RoomExistsAsync(request.MaPhongHoc.Value, cancellationToken))
        {
            return OperationResult<ScheduleDetailDto>.BadRequest(
                "Phòng học không tồn tại.",
                [$"Không tìm thấy phòng có id = {request.MaPhongHoc}."]);
        }

        if (ScheduleStatuses.IsActive(current.TrangThai)
            && await _scheduleStore.HasRoomConflictAsync(
                maPhongHoc, thu, gioBatDau, gioKetThuc, ngayTu, ngayDen, maLichHoc, cancellationToken))
        {
            return OperationResult<ScheduleDetailDto>.Conflict(
                "Phòng học đã được dùng bởi lịch khác cùng thứ, trùng giờ và chồng lấn khoảng ngày áp dụng.");
        }

        var outcome = await _scheduleStore.UpdateAsync(maLichHoc, request, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return outcome == WriteOutcome.NotFound
                ? OperationResult<ScheduleDetailDto>.NotFound($"Không tìm thấy lịch học có id = {maLichHoc}.")
                : OperationResult<ScheduleDetailDto>.Conflict("Không thể cập nhật lịch học.");
        }

        var updated = await _scheduleStore.GetDetailAsync(maLichHoc, cancellationToken);

        return OperationResult<ScheduleDetailDto>.Ok(updated!);
    }

    public async Task<OperationResult<ScheduleDetailDto>> UpdateStatusAsync(
        int maLichHoc,
        UpdateStatusRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = TextValue.Required(request.TrangThai);

        if (trangThai is null || !ScheduleStatuses.IsValid(trangThai))
        {
            return OperationResult<ScheduleDetailDto>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", ScheduleStatuses.All)}."]);
        }

        if (!await _scheduleStore.ExistsAsync(maLichHoc, cancellationToken))
        {
            return OperationResult<ScheduleDetailDto>.NotFound($"Không tìm thấy lịch học có id = {maLichHoc}.");
        }

        var outcome = await _scheduleStore.UpdateStatusAsync(maLichHoc, trangThai, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return OperationResult<ScheduleDetailDto>.Conflict("Không thể cập nhật trạng thái lịch học.");
        }

        var updated = await _scheduleStore.GetDetailAsync(maLichHoc, cancellationToken);

        return OperationResult<ScheduleDetailDto>.Ok(updated!);
    }

    public async Task<OperationResult<bool>> DeleteAsync(int maLichHoc, CancellationToken cancellationToken)
    {
        if (!await _scheduleStore.ExistsAsync(maLichHoc, cancellationToken))
        {
            return OperationResult<bool>.NotFound($"Không tìm thấy lịch học có id = {maLichHoc}.");
        }

        var sessionCount = await _scheduleStore.CountSessionsAsync(maLichHoc, cancellationToken);

        if (sessionCount > 0)
        {
            return OperationResult<bool>.Conflict(
                $"Không thể xóa lịch học vì đã có {sessionCount} ca học thực tế tham chiếu. " +
                "Hãy dùng chức năng đổi trạng thái sang 'Ngừng áp dụng'.");
        }

        var outcome = await _scheduleStore.DeleteAsync(maLichHoc, cancellationToken);

        return outcome switch
        {
            WriteOutcome.Success => OperationResult<bool>.NoContent(),
            WriteOutcome.NotFound => OperationResult<bool>.NotFound($"Không tìm thấy lịch học có id = {maLichHoc}."),
            _ => OperationResult<bool>.Conflict("Không thể xóa lịch học do ràng buộc dữ liệu.")
        };
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    private static List<string> Validate(
        int thuTrongTuan,
        TimeOnly gioBatDau,
        TimeOnly gioKetThuc,
        DateOnly ngayApDungTu,
        DateOnly? ngayApDungDen)
    {
        var errors = new List<string>();

        if (thuTrongTuan is < 1 or > 7)
        {
            errors.Add("Thứ trong tuần phải từ 1 (Thứ Hai) đến 7 (Chủ Nhật).");
        }

        if (gioBatDau == default)
        {
            errors.Add("Giờ bắt đầu là bắt buộc.");
        }

        if (gioKetThuc == default)
        {
            errors.Add("Giờ kết thúc là bắt buộc.");
        }

        if (gioBatDau != default && gioKetThuc != default && gioKetThuc <= gioBatDau)
        {
            errors.Add("Giờ kết thúc phải sau giờ bắt đầu.");
        }

        if (ngayApDungTu == default)
        {
            errors.Add("Ngày áp dụng từ là bắt buộc.");
        }

        if (ngayApDungTu != default && ngayApDungDen is { } den && den < ngayApDungTu)
        {
            errors.Add("Ngày áp dụng đến phải lớn hơn hoặc bằng ngày áp dụng từ.");
        }

        return errors;
    }

    private static bool IsEmptyUpdate(UpdateScheduleRequest request)
        => request.MaPhongHoc is null
           && request.ThuTrongTuan is null
           && request.GioBatDau is null
           && request.GioKetThuc is null
           && request.NgayApDungTu is null
           && request.NgayApDungDen is null;
}
