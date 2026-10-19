using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.TuitionFees.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.TuitionFees;

/// <inheritdoc cref="ITuitionFeeService"/>
/// <remarks>
/// <para>
/// Mọi số tiền của nghĩa vụ học phí được COPY từ <c>DangKyHoc</c> — nguồn chính thức.
/// Client KHÔNG bao giờ gửi số tiền lên.
/// </para>
/// <para>
/// Hai cột <c>SoTienPhaiDong</c> và <c>SoTienConThieu</c> là computed PERSISTED:
/// không ghi trực tiếp, cũng không tính lại ở code.
/// </para>
/// </remarks>
public sealed class TuitionFeeService : ITuitionFeeService
{
    private readonly ITuitionFeeStore _tuitionFeeStore;

    public TuitionFeeService(ITuitionFeeStore tuitionFeeStore)
    {
        _tuitionFeeStore = tuitionFeeStore;
    }

    public async Task<OperationResult<PagedResult<TuitionFeeListItemDto>>> ListAsync(
        TuitionFeeListQuery query,
        CancellationToken cancellationToken)
    {
        var invalid = ValidateFilter(query);

        if (invalid is not null)
        {
            return invalid;
        }

        var page = await _tuitionFeeStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<TuitionFeeListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<TuitionFeeDetailDto>> GetDetailAsync(
        int maHocPhi,
        CancellationToken cancellationToken)
    {
        var detail = await _tuitionFeeStore.GetDetailAsync(maHocPhi, cancellationToken);

        return detail is null
            ? OperationResult<TuitionFeeDetailDto>.NotFound($"Không tìm thấy học phí có id = {maHocPhi}.")
            : OperationResult<TuitionFeeDetailDto>.Ok(detail);
    }

    public async Task<OperationResult<TuitionFeeDetailDto>> CreateAsync(
        CreateTuitionFeeRequest request,
        CancellationToken cancellationToken)
    {
        var snapshot = await _tuitionFeeStore.GetEnrollmentSnapshotAsync(
            request.MaDangKy,
            cancellationToken);

        if (snapshot is null)
        {
            return OperationResult<TuitionFeeDetailDto>.BadRequest(
                "Đăng ký học không tồn tại.",
                [$"Không tìm thấy đăng ký có id = {request.MaDangKy}."]);
        }

        if (snapshot.TrangThaiDangKy == EnrollmentStatuses.DaHuy)
        {
            return OperationResult<TuitionFeeDetailDto>.BadRequest(
                "Đăng ký học đã bị hủy.",
                [$"Không thể tạo nghĩa vụ học phí cho đăng ký đang ở trạng thái '{snapshot.TrangThaiDangKy}'."]);
        }

        var existingId = await _tuitionFeeStore.FindByEnrollmentAsync(request.MaDangKy, cancellationToken);

        if (existingId is not null)
        {
            return OperationResult<TuitionFeeDetailDto>.Conflict(
                $"Đăng ký {request.MaDangKy} đã có nghĩa vụ học phí (id = {existingId}). " +
                "Mỗi đăng ký chỉ được có đúng một nghĩa vụ học phí.");
        }

        var result = await _tuitionFeeStore.CreateAsync(
            snapshot,
            request.HanThanhToan,
            cancellationToken);

        if (!result.Succeeded)
        {
            return OperationResult<TuitionFeeDetailDto>.Conflict(
                "Không thể tạo nghĩa vụ học phí (có thể vừa được tạo bởi một yêu cầu khác).");
        }

        var created = await _tuitionFeeStore.GetDetailAsync(result.Id, cancellationToken);

        return OperationResult<TuitionFeeDetailDto>.Created(created!, "Tạo nghĩa vụ học phí thành công.");
    }

    public async Task<OperationResult<TuitionFeeDetailDto>> UpdateAsync(
        int maHocPhi,
        UpdateTuitionFeeRequest request,
        CancellationToken cancellationToken)
    {
        var current = await _tuitionFeeStore.GetDetailAsync(maHocPhi, cancellationToken);

        if (current is null)
        {
            return OperationResult<TuitionFeeDetailDto>.NotFound($"Không tìm thấy học phí có id = {maHocPhi}.");
        }

        var hasDueDateChange = request.HanThanhToan is not null || request.XoaHanThanhToan;

        if (!hasDueDateChange && !request.DongBoTuDangKy)
        {
            return OperationResult<TuitionFeeDetailDto>.BadRequest(
                "Không có trường nào được cập nhật.",
                ["Hãy gửi 'hanThanhToan', 'xoaHanThanhToan' hoặc 'dongBoTuDangKy'."]);
        }

        if (request.XoaHanThanhToan && request.HanThanhToan is not null)
        {
            return OperationResult<TuitionFeeDetailDto>.BadRequest(
                "Tham số mâu thuẫn.",
                ["Không thể vừa đặt 'hanThanhToan' vừa bật 'xoaHanThanhToan'."]);
        }

        var soTienGoc = current.SoTienGoc;
        var soTienGiam = current.SoTienGiam;

        if (request.DongBoTuDangKy)
        {
            if (current.SoTienDaDong > 0m)
            {
                return OperationResult<TuitionFeeDetailDto>.Conflict(
                    "Học phí đã có thanh toán nên không thể đồng bộ lại số tiền từ đăng ký. " +
                    "Hãy xử lý bằng điều chỉnh/hoàn tiền theo quy trình kế toán.");
            }

            var snapshot = await _tuitionFeeStore.GetEnrollmentSnapshotAsync(
                current.MaDangKy,
                cancellationToken);

            if (snapshot is null)
            {
                return OperationResult<TuitionFeeDetailDto>.BadRequest(
                    "Không tìm thấy đăng ký học tương ứng.",
                    [$"Không tìm thấy đăng ký có id = {current.MaDangKy}."]);
            }

            soTienGoc = snapshot.HocPhiGoc;
            soTienGiam = snapshot.SoTienGiam;

            if (soTienGiam > soTienGoc)
            {
                return OperationResult<TuitionFeeDetailDto>.BadRequest(
                    "Số tiền giảm vượt quá học phí gốc của đăng ký.",
                    [$"Học phí gốc = {soTienGoc:N0}, số tiền giảm = {soTienGiam:N0}."]);
            }
        }

        var data = new TuitionFeeUpdateData(
            CapNhatHanThanhToan: hasDueDateChange,
            HanThanhToan: request.XoaHanThanhToan ? null : request.HanThanhToan,
            CapNhatSoTien: request.DongBoTuDangKy,
            SoTienGoc: soTienGoc,
            SoTienGiam: soTienGiam);

        var outcome = await _tuitionFeeStore.UpdateAsync(maHocPhi, data, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return outcome == WriteOutcome.NotFound
                ? OperationResult<TuitionFeeDetailDto>.NotFound($"Không tìm thấy học phí có id = {maHocPhi}.")
                : OperationResult<TuitionFeeDetailDto>.Conflict(
                    "Không thể cập nhật học phí do ràng buộc dữ liệu. " +
                    "Kiểm tra lại học phí gốc / số tiền giảm so với số đã đóng.");
        }

        var updated = await _tuitionFeeStore.GetDetailAsync(maHocPhi, cancellationToken);

        return OperationResult<TuitionFeeDetailDto>.Ok(updated!);
    }

    public async Task<OperationResult<TuitionSummaryReportDto>> GetReportAsync(
        TuitionReportQuery query,
        CancellationToken cancellationToken)
    {
        if (query.TuNgay is { } tu && query.DenNgay is { } den && den < tu)
        {
            return OperationResult<TuitionSummaryReportDto>.BadRequest(
                "Khoảng ngày không hợp lệ.",
                ["Đến ngày phải lớn hơn hoặc bằng từ ngày."]);
        }

        var report = await _tuitionFeeStore.GetSummaryReportAsync(query, cancellationToken);

        return OperationResult<TuitionSummaryReportDto>.Ok(report);
    }

    public async Task<OperationResult<PagedResult<TuitionFeeListItemDto>>> ListForStudentAsync(
        string maHocVien,
        TuitionFeeListQuery query,
        CancellationToken cancellationToken)
    {
        // Ghi đè bộ lọc: học viên chỉ thấy dữ liệu của chính mình.
        query.MaHocVien = maHocVien;
        query.MaKhoaHoc = null;
        query.MaLopHoc = null;

        var invalid = ValidateFilter(query);

        if (invalid is not null)
        {
            return invalid;
        }

        var page = await _tuitionFeeStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<TuitionFeeListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<TuitionFeeDetailDto>> GetDetailForStudentAsync(
        int maHocPhi,
        string maHocVien,
        CancellationToken cancellationToken)
    {
        var detail = await _tuitionFeeStore.GetDetailAsync(maHocPhi, cancellationToken);

        // Trả 404 thay vì 403 để không lộ sự tồn tại của bản ghi người khác.
        if (detail is null || !string.Equals(detail.MaHocVien, maHocVien, StringComparison.Ordinal))
        {
            return OperationResult<TuitionFeeDetailDto>.NotFound($"Không tìm thấy học phí có id = {maHocPhi}.");
        }

        return OperationResult<TuitionFeeDetailDto>.Ok(detail);
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    private static OperationResult<PagedResult<TuitionFeeListItemDto>>? ValidateFilter(
        TuitionFeeListQuery query)
    {
        if (!string.IsNullOrWhiteSpace(query.TrangThai)
            && !TuitionStatuses.IsValid(query.TrangThai.Trim()))
        {
            return OperationResult<PagedResult<TuitionFeeListItemDto>>.BadRequest(
                "Trạng thái học phí không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", TuitionStatuses.All)}."]);
        }

        if (query.TuNgay is { } tu && query.DenNgay is { } den && den < tu)
        {
            return OperationResult<PagedResult<TuitionFeeListItemDto>>.BadRequest(
                "Khoảng ngày không hợp lệ.",
                ["Đến ngày phải lớn hơn hoặc bằng từ ngày."]);
        }

        return null;
    }
}
