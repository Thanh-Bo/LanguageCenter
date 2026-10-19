using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Courses.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Courses;

/// <inheritdoc cref="ICourseService"/>
public sealed class CourseService : ICourseService
{
    private readonly ICourseStore _courseStore;
    private readonly ILevelStore _levelStore;

    public CourseService(ICourseStore courseStore, ILevelStore levelStore)
    {
        _courseStore = courseStore;
        _levelStore = levelStore;
    }

    public async Task<OperationResult<PagedResult<CourseListItemDto>>> ListAsync(
        CourseListQuery query,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(query.TrangThai) && !CourseStatuses.IsValid(query.TrangThai.Trim()))
        {
            return OperationResult<PagedResult<CourseListItemDto>>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", CourseStatuses.All)}."]);
        }

        var page = await _courseStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<CourseListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<CourseDetailDto>> GetDetailAsync(
        int maKhoaHoc,
        CancellationToken cancellationToken)
    {
        var detail = await _courseStore.GetDetailAsync(maKhoaHoc, cancellationToken);

        return detail is null
            ? OperationResult<CourseDetailDto>.NotFound($"Không tìm thấy khóa học có id = {maKhoaHoc}.")
            : OperationResult<CourseDetailDto>.Ok(detail);
    }

    public async Task<OperationResult<CourseDetailDto>> CreateAsync(
        CreateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var errors = Validate(request.SoBuoi, request.TongSoGio, request.HocPhiCoBan, request.NgayBatDau, request.NgayKetThuc);

        if (errors.Count > 0)
        {
            return OperationResult<CourseDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        if (!await _levelStore.ExistsAsync(request.MaCapDo, cancellationToken))
        {
            return OperationResult<CourseDetailDto>.BadRequest(
                "Cấp độ không tồn tại.",
                [$"Không tìm thấy cấp độ có id = {request.MaCapDo}."]);
        }

        var ten = request.TenKhoaHoc.Trim();

        if (await _courseStore.NameExistsAsync(request.MaCapDo, ten, exceptId: null, cancellationToken))
        {
            return OperationResult<CourseDetailDto>.Conflict(
                $"Tên khóa học '{ten}' đã tồn tại trong cấp độ này.");
        }

        var result = await _courseStore.CreateAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            return OperationResult<CourseDetailDto>.Conflict(
                "Không thể tạo khóa học: tên đã tồn tại trong cấp độ này.");
        }

        var created = await _courseStore.GetDetailAsync(result.Id, cancellationToken);

        return OperationResult<CourseDetailDto>.Created(
            created!,
            $"Tạo khóa học '{ten}' thành công.");
    }

    public async Task<OperationResult<CourseDetailDto>> UpdateAsync(
        int maKhoaHoc,
        UpdateCourseRequest request,
        CancellationToken cancellationToken)
    {
        var current = await _courseStore.GetDetailAsync(maKhoaHoc, cancellationToken);

        if (current is null)
        {
            return OperationResult<CourseDetailDto>.NotFound($"Không tìm thấy khóa học có id = {maKhoaHoc}.");
        }

        if (IsEmptyUpdate(request))
        {
            return OperationResult<CourseDetailDto>.BadRequest(
                "Không có trường nào được cập nhật.",
                ["Hãy gửi ít nhất một trường cần thay đổi."]);
        }

        // Validate trên GIÁ TRỊ SAU KHI GỘP.
        var errors = Validate(
            request.SoBuoi ?? current.SoBuoi,
            request.TongSoGio ?? current.TongSoGio,
            request.HocPhiCoBan ?? current.HocPhiCoBan,
            request.NgayBatDau ?? current.NgayBatDau,
            request.NgayKetThuc ?? current.NgayKetThuc);

        if (errors.Count > 0)
        {
            return OperationResult<CourseDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        if (request.TenKhoaHoc is not null)
        {
            var ten = request.TenKhoaHoc.Trim();

            if (await _courseStore.NameExistsAsync(current.MaCapDo, ten, maKhoaHoc, cancellationToken))
            {
                return OperationResult<CourseDetailDto>.Conflict(
                    $"Tên khóa học '{ten}' đã tồn tại trong cấp độ này.");
            }
        }

        var outcome = await _courseStore.UpdateAsync(maKhoaHoc, request, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return outcome == WriteOutcome.NotFound
                ? OperationResult<CourseDetailDto>.NotFound($"Không tìm thấy khóa học có id = {maKhoaHoc}.")
                : OperationResult<CourseDetailDto>.Conflict("Không thể cập nhật khóa học.");
        }

        var updated = await _courseStore.GetDetailAsync(maKhoaHoc, cancellationToken);

        return OperationResult<CourseDetailDto>.Ok(updated!);
    }

    public async Task<OperationResult<CourseDetailDto>> UpdateStatusAsync(
        int maKhoaHoc,
        UpdateStatusRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = TextValue.Required(request.TrangThai);

        if (trangThai is null || !CourseStatuses.IsValid(trangThai))
        {
            return OperationResult<CourseDetailDto>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", CourseStatuses.All)}."]);
        }

        if (!await _courseStore.ExistsAsync(maKhoaHoc, cancellationToken))
        {
            return OperationResult<CourseDetailDto>.NotFound($"Không tìm thấy khóa học có id = {maKhoaHoc}.");
        }

        var outcome = await _courseStore.UpdateStatusAsync(maKhoaHoc, trangThai, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return OperationResult<CourseDetailDto>.Conflict("Không thể cập nhật trạng thái khóa học.");
        }

        var updated = await _courseStore.GetDetailAsync(maKhoaHoc, cancellationToken);

        return OperationResult<CourseDetailDto>.Ok(updated!);
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    private static List<string> Validate(
        int soBuoi,
        decimal? tongSoGio,
        decimal hocPhiCoBan,
        DateOnly ngayBatDau,
        DateOnly ngayKetThuc)
    {
        var errors = new List<string>();

        if (soBuoi < 1)
        {
            errors.Add("Số buổi phải lớn hơn 0.");
        }

        if (tongSoGio is <= 0)
        {
            errors.Add("Tổng số giờ phải lớn hơn 0.");
        }

        if (hocPhiCoBan < 0)
        {
            errors.Add("Học phí cơ bản không được âm.");
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

    private static bool IsEmptyUpdate(UpdateCourseRequest request)
        => request.TenKhoaHoc is null
           && request.NoiDung is null
           && request.SoBuoi is null
           && request.TongSoGio is null
           && request.HocPhiCoBan is null
           && request.NgayBatDau is null
           && request.NgayKetThuc is null;
}
