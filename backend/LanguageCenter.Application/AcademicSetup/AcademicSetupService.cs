using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.AcademicSetup.Models;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.AcademicSetup;

/// <inheritdoc cref="IAcademicSetupService"/>
public sealed class AcademicSetupService : IAcademicSetupService
{
    /// <summary>Mỗi khóa học cố định 20 buổi.</summary>
    public const int SoBuoiCoDinh = 20;

    private readonly IAcademicSetupStore _store;
    private readonly ICourseStore _courseStore;

    public AcademicSetupService(IAcademicSetupStore store, ICourseStore courseStore)
    {
        _store = store;
        _courseStore = courseStore;
    }

    // ======================================================================
    // 20 BUỔI HỌC
    // ======================================================================

    public async Task<OperationResult<IReadOnlyList<LessonContentDto>>> ListLessonContentsAsync(
        int maKhoaHoc,
        CancellationToken cancellationToken)
    {
        if (maKhoaHoc <= 0)
        {
            return OperationResult<IReadOnlyList<LessonContentDto>>.BadRequest(
                "Khóa học không hợp lệ.",
                ["Mã khóa học phải lớn hơn 0."]);
        }

        if (!await _courseStore.ExistsAsync(maKhoaHoc, cancellationToken))
        {
            return OperationResult<IReadOnlyList<LessonContentDto>>.NotFound(
                $"Không tìm thấy khóa học có id = {maKhoaHoc}.");
        }

        var items = await _store.ListLessonContentsAsync(maKhoaHoc, cancellationToken);

        return OperationResult<IReadOnlyList<LessonContentDto>>.Ok(items);
    }

    public async Task<OperationResult<IReadOnlyList<LessonContentDto>>> SaveLessonContentsAsync(
        int maKhoaHoc,
        SaveLessonContentsRequest request,
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();

        if (maKhoaHoc <= 0)
        {
            errors.Add("Mã khóa học phải lớn hơn 0.");
        }

        var items = request.Items ?? [];

        if (items.Count == 0)
        {
            errors.Add("Chưa có nội dung buổi học nào để lưu.");
        }

        var trungSoBuoi = items
            .GroupBy(x => x.SoBuoi)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .OrderBy(x => x)
            .ToList();

        if (trungSoBuoi.Count > 0)
        {
            errors.Add($"Số buổi bị lặp: {string.Join(", ", trungSoBuoi)}. Mỗi buổi chỉ có 1 nội dung.");
        }

        var ngoaiPhamVi = items
            .Where(x => x.SoBuoi < 1 || x.SoBuoi > SoBuoiCoDinh)
            .Select(x => x.SoBuoi)
            .OrderBy(x => x)
            .ToList();

        if (ngoaiPhamVi.Count > 0)
        {
            errors.Add($"Số buổi phải nằm trong 1..{SoBuoiCoDinh}. Sai: {string.Join(", ", ngoaiPhamVi)}.");
        }

        var thieuTieuDe = items
            .Where(x => string.IsNullOrWhiteSpace(x.TieuDe))
            .Select(x => x.SoBuoi)
            .OrderBy(x => x)
            .ToList();

        if (thieuTieuDe.Count > 0)
        {
            errors.Add($"Buổi {string.Join(", ", thieuTieuDe)} chưa có tiêu đề.");
        }

        if (errors.Count > 0)
        {
            return OperationResult<IReadOnlyList<LessonContentDto>>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        if (!await _courseStore.ExistsAsync(maKhoaHoc, cancellationToken))
        {
            return OperationResult<IReadOnlyList<LessonContentDto>>.NotFound(
                $"Không tìm thấy khóa học có id = {maKhoaHoc}.");
        }

        await _store.SaveLessonContentsAsync(maKhoaHoc, items, cancellationToken);

        var after = await _store.ListLessonContentsAsync(maKhoaHoc, cancellationToken);

        return OperationResult<IReadOnlyList<LessonContentDto>>.Ok(after);
    }

    // ======================================================================
    // NGÀY LỄ
    // ======================================================================

    public async Task<OperationResult<IReadOnlyList<HolidayDto>>> ListHolidaysAsync(
        CancellationToken cancellationToken)
    {
        var items = await _store.ListHolidaysAsync(cancellationToken);

        return OperationResult<IReadOnlyList<HolidayDto>>.Ok(items);
    }

    public async Task<OperationResult<HolidayDto>> CreateHolidayAsync(
        CreateHolidayRequest request,
        CancellationToken cancellationToken)
    {
        var errors = ValidateHoliday(request.Ngay, request.TenNgayLe);

        if (errors.Count > 0)
        {
            return OperationResult<HolidayDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        if (await _store.HolidayDateExistsAsync(request.Ngay, exceptId: null, cancellationToken))
        {
            return OperationResult<HolidayDto>.Conflict(
                $"Ngày {request.Ngay:dd/MM/yyyy} đã được khai là ngày lễ.");
        }

        var result = await _store.CreateHolidayAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            return OperationResult<HolidayDto>.Conflict(
                $"Ngày {request.Ngay:dd/MM/yyyy} đã được khai là ngày lễ.");
        }

        var created = await _store.GetHolidayAsync(result.Id, cancellationToken);

        return OperationResult<HolidayDto>.Created(created!, "Đã thêm ngày lễ.");
    }

    public async Task<OperationResult<HolidayDto>> UpdateHolidayAsync(
        int maNgayLe,
        UpdateHolidayRequest request,
        CancellationToken cancellationToken)
    {
        var current = await _store.GetHolidayAsync(maNgayLe, cancellationToken);

        if (current is null)
        {
            return OperationResult<HolidayDto>.NotFound($"Không tìm thấy ngày lễ có id = {maNgayLe}.");
        }

        if (request.Ngay is null && request.TenNgayLe is null && request.GhiChu is null)
        {
            return OperationResult<HolidayDto>.BadRequest(
                "Không có trường nào được cập nhật.",
                ["Hãy gửi ít nhất một trường cần thay đổi."]);
        }

        var ngay = request.Ngay ?? current.Ngay;
        var ten = request.TenNgayLe ?? current.TenNgayLe;

        var errors = ValidateHoliday(ngay, ten);

        if (errors.Count > 0)
        {
            return OperationResult<HolidayDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        if (await _store.HolidayDateExistsAsync(ngay, exceptId: maNgayLe, cancellationToken))
        {
            return OperationResult<HolidayDto>.Conflict(
                $"Ngày {ngay:dd/MM/yyyy} đã được khai là ngày lễ khác.");
        }

        var outcome = await _store.UpdateHolidayAsync(maNgayLe, request, cancellationToken);

        if (outcome == WriteOutcome.NotFound)
        {
            return OperationResult<HolidayDto>.NotFound($"Không tìm thấy ngày lễ có id = {maNgayLe}.");
        }

        if (outcome != WriteOutcome.Success)
        {
            return OperationResult<HolidayDto>.Conflict(
                $"Ngày {ngay:dd/MM/yyyy} đã được khai là ngày lễ khác.");
        }

        var updated = await _store.GetHolidayAsync(maNgayLe, cancellationToken);

        return OperationResult<HolidayDto>.Ok(updated!);
    }

    public async Task<OperationResult<HolidayDto>> DeleteHolidayAsync(
        int maNgayLe,
        CancellationToken cancellationToken)
    {
        var current = await _store.GetHolidayAsync(maNgayLe, cancellationToken);

        if (current is null)
        {
            return OperationResult<HolidayDto>.NotFound($"Không tìm thấy ngày lễ có id = {maNgayLe}.");
        }

        var outcome = await _store.DeleteHolidayAsync(maNgayLe, cancellationToken);

        return outcome == WriteOutcome.Success
            ? OperationResult<HolidayDto>.Ok(current)
            : OperationResult<HolidayDto>.Conflict("Không xóa được ngày lễ.");
    }

    private static List<string> ValidateHoliday(DateOnly ngay, string? tenNgayLe)
    {
        var errors = new List<string>();

        if (ngay == default)
        {
            errors.Add("Ngày lễ là bắt buộc.");
        }

        if (string.IsNullOrWhiteSpace(tenNgayLe))
        {
            errors.Add("Tên ngày lễ là bắt buộc.");
        }

        return errors;
    }
}
