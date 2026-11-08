using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Enrollments;
using LanguageCenter.Application.Payments.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Payments;

/// <inheritdoc cref="IPaymentService"/>
/// <remarks>
/// <para>
/// Nguyên tắc bảo mật: client KHÔNG gửi <c>maNhanVienKeToan</c>, <c>ngayThanhToan</c>
/// hay <c>trangThai</c>. Người ghi nhận lấy từ JWT, thời gian và trạng thái do server quyết định.
/// </para>
/// <para>
/// Idempotency: <c>maGiaoDich</c> là BẮT BUỘC và được bảo vệ bằng unique index có điều kiện
/// <c>UX_ThanhToan_MaGiaoDich</c>. Gửi lại cùng mã ⇒ trả về giao dịch đã có (200), không tạo mới.
/// </para>
/// </remarks>
public sealed class PaymentService : IPaymentService
{
    /// <summary>
    /// Nhân viên kế toán được gán làm "người ghi nhận" cho giao dịch do HỌC VIÊN tự thực hiện
    /// (bảng <c>ThanhToan.MaNhanVienKeToan</c> là NOT NULL và tham chiếu <c>NhanVien</c>).
    /// </summary>
    public const string SystemCollectorCode = "NVKT001";

    private readonly IPaymentStore _paymentStore;
    private readonly IReferenceStore _referenceStore;
    private readonly ITuitionFeeStore _tuitionFeeStore;
    private readonly IEnrollmentService _enrollmentService;

    public PaymentService(
        IPaymentStore paymentStore,
        IReferenceStore referenceStore,
        ITuitionFeeStore tuitionFeeStore,
        IEnrollmentService enrollmentService)
    {
        _paymentStore = paymentStore;
        _referenceStore = referenceStore;
        _tuitionFeeStore = tuitionFeeStore;
        _enrollmentService = enrollmentService;
    }

    public async Task<OperationResult<PagedResult<PaymentDto>>> ListAsync(
        PaymentListQuery query,
        CancellationToken cancellationToken)
    {
        var invalid = ValidateFilter(query);

        if (invalid is not null)
        {
            return invalid;
        }

        var page = await _paymentStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<PaymentDto>>.Ok(page);
    }

    public async Task<OperationResult<PaymentDto>> GetDetailAsync(
        int maThanhToan,
        CancellationToken cancellationToken)
    {
        var detail = await _paymentStore.GetDetailAsync(maThanhToan, cancellationToken);

        return detail is null
            ? OperationResult<PaymentDto>.NotFound($"Không tìm thấy giao dịch có id = {maThanhToan}.")
            : OperationResult<PaymentDto>.Ok(detail);
    }

    public async Task<OperationResult<PagedResult<PaymentDto>>> ListByTuitionAsync(
        int maHocPhi,
        PaymentListQuery query,
        CancellationToken cancellationToken)
    {
        var invalid = ValidateFilter(query);

        if (invalid is not null)
        {
            return invalid;
        }

        var page = await _paymentStore.ListByTuitionAsync(maHocPhi, query, cancellationToken);

        return OperationResult<PagedResult<PaymentDto>>.Ok(page);
    }

    public async Task<OperationResult<PaymentDto>> RecordAsync(
        int maHocPhi,
        RecordPaymentRequest request,
        PaymentActor actor,
        CancellationToken cancellationToken)
    {
        // ---- (1) Loại giao dịch ----
        var loaiGiaoDich = TextValue.Required(request.LoaiGiaoDich) ?? PaymentTypes.ThuTien;

        if (!PaymentTypes.IsValid(loaiGiaoDich))
        {
            return OperationResult<PaymentDto>.BadRequest(
                "Loại giao dịch không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", PaymentTypes.All)}."]);
        }

        if (!PaymentTypes.IsSupported(loaiGiaoDich))
        {
            return OperationResult<PaymentDto>.BadRequest(
                $"Loại giao dịch '{loaiGiaoDich}' chưa được hỗ trợ.",
                ["Phase 6 chỉ ghi nhận giao dịch 'Thu tiền'. Hoàn tiền/điều chỉnh cần quy trình kế toán riêng."]);
        }

        // ---- (2) Khoá chống trùng ----
        var maGiaoDich = TextValue.Required(request.MaGiaoDich);

        if (maGiaoDich is null)
        {
            return OperationResult<PaymentDto>.BadRequest(
                "Mã giao dịch là bắt buộc.",
                ["Dùng 'maGiaoDich' làm khoá chống trùng; gửi lại cùng giá trị sẽ không tạo giao dịch mới."]);
        }

        if (maGiaoDich.Length is < 3 or > 150)
        {
            return OperationResult<PaymentDto>.BadRequest(
                "Mã giao dịch không hợp lệ.",
                ["Mã giao dịch phải từ 3 đến 150 ký tự."]);
        }

        // ---- (3) Số tiền ----
        if (request.SoTien <= 0m)
        {
            return OperationResult<PaymentDto>.BadRequest(
                "Số tiền không hợp lệ.",
                ["Số tiền phải lớn hơn 0."]);
        }

        if (decimal.Round(request.SoTien, 2) != request.SoTien)
        {
            return OperationResult<PaymentDto>.BadRequest(
                "Số tiền không hợp lệ.",
                ["Số tiền chỉ được có tối đa 2 chữ số thập phân (DECIMAL(18,2))."]);
        }

        // ---- (3b) NGUYÊN TẮC THU MỘT LẦN ----
        // Trung tâm thu ĐỦ học phí trong một giao dịch duy nhất (không thu góp),
        // nhờ vậy phiếu chỉ có 2 trạng thái: Chưa thanh toán / Đã thanh toán.
        var hocPhiHienTai = await _tuitionFeeStore.GetDetailAsync(maHocPhi, cancellationToken);

        if (hocPhiHienTai is null)
        {
            return OperationResult<PaymentDto>.NotFound($"Không tìm thấy học phí có id = {maHocPhi}.");
        }

        if (hocPhiHienTai.SoTienConThieu <= 0m)
        {
            return OperationResult<PaymentDto>.Conflict("Học phí này đã được thu đủ.");
        }

        if (decimal.Round(request.SoTien, 2) != decimal.Round(hocPhiHienTai.SoTienConThieu, 2))
        {
            return OperationResult<PaymentDto>.BadRequest(
                "Phải thu đủ học phí trong MỘT lần thanh toán.",
                [
                    $"Số tiền cần thu đúng bằng số còn thiếu: {hocPhiHienTai.SoTienConThieu:N0} ₫.",
                    $"Số tiền gửi lên: {request.SoTien:N0} ₫."
                ]);
        }

        // ---- (4) Phương thức ----
        var phuongThuc = TextValue.Required(request.PhuongThucThanhToan);

        if (phuongThuc is null)
        {
            return OperationResult<PaymentDto>.BadRequest(
                "Phương thức thanh toán là bắt buộc.",
                [$"Ví dụ: '{PaymentMethods.TienMat}' hoặc '{PaymentMethods.ChuyenKhoan}'."]);
        }

        if (phuongThuc.Length > 100)
        {
            return OperationResult<PaymentDto>.BadRequest(
                "Phương thức thanh toán không hợp lệ.",
                ["Tối đa 100 ký tự."]);
        }

        // ---- (5) Người ghi nhận: lấy từ JWT, phải có hồ sơ NhanVien ----
        var maNhanVien = TextValue.Required(actor.TenDangNhap);

        if (maNhanVien is null)
        {
            return OperationResult<PaymentDto>.Forbidden(
                "Không xác định được người thực hiện từ token.");
        }

        if (!await _referenceStore.StaffExistsAsync(maNhanVien, cancellationToken))
        {
            return OperationResult<PaymentDto>.BadRequest(
                "Tài khoản chưa có hồ sơ nhân viên.",
                [
                    $"Bảng ThanhToan yêu cầu 'MaNhanVienKeToan' tham chiếu bảng NhanVien, " +
                    $"nhưng không tìm thấy nhân viên '{maNhanVien}'."
                ]);
        }

        // ---- (6) Ghi nhận nguyên tử ----
        var data = new PaymentWriteData(
            MaHocPhi: maHocPhi,
            MaNhanVienKeToan: maNhanVien,
            SoTien: request.SoTien,
            PhuongThucThanhToan: phuongThuc,
            MaGiaoDich: maGiaoDich,
            LoaiGiaoDich: loaiGiaoDich,
            GhiChu: TextValue.Required(request.GhiChu),
            NoiDungBienNhan: TextValue.Required(request.NoiDungBienNhan));

        var result = await _paymentStore.RecordAsync(data, cancellationToken);

        switch (result.Status)
        {
            case PaymentWriteStatus.TuitionNotFound:
                return OperationResult<PaymentDto>.NotFound(
                    $"Không tìm thấy học phí có id = {maHocPhi}.");

            case PaymentWriteStatus.ExceedsBalance:
                return OperationResult<PaymentDto>.Conflict(
                    $"Số tiền vượt quá số dư còn thiếu ({result.SoTienConThieu:N0}). " +
                    "Vui lòng kiểm tra lại số tiền cần thu.");

            case PaymentWriteStatus.Duplicate:
                return await HandleDuplicateAsync(maHocPhi, result.MaThanhToan, cancellationToken);

            case PaymentWriteStatus.Conflict:
                return OperationResult<PaymentDto>.Conflict(
                    "Không thể ghi nhận thanh toán do xung đột dữ liệu. Vui lòng thử lại.");

            default:
                var created = await _paymentStore.GetDetailAsync(result.MaThanhToan, cancellationToken);

                if (created is null)
                {
                    return OperationResult<PaymentDto>.Conflict("Đã ghi thanh toán nhưng không đọc lại được giao dịch.");
                }

                // Thu đủ tiền ⇒ TỰ ĐỘNG xếp học viên vào lớp dự kiến.
                await ThuXepLopSauThanhToanAsync(maHocPhi, cancellationToken);

                return OperationResult<PaymentDto>.Created(created, "Ghi nhận thanh toán thành công.");
        }
    }

    /// <summary>
    /// Sau khi ghi nhận thanh toán: nếu phiếu đã tất toán thì tự xếp học viên vào LỚP DỰ KIẾN.
    /// <para>
    /// Cố tình KHÔNG làm hỏng kết quả thanh toán khi xếp lớp thất bại (lớp đầy / quá 4 buổi /
    /// trùng giờ / sai tuổi): tiền đã thu là sự thật, còn việc chưa xếp được lớp sẽ hiện
    /// cảnh báo ở màn Đăng ký / Xếp lớp để admin đổi lớp dự kiến.
    /// </para>
    /// </summary>
    private async Task ThuXepLopSauThanhToanAsync(int maHocPhi, CancellationToken cancellationToken)
    {
        var hocPhi = await _tuitionFeeStore.GetDetailAsync(maHocPhi, cancellationToken);

        if (hocPhi is null)
        {
            return;
        }

        await _enrollmentService.TryAutoPlaceIfPaidAsync(hocPhi.MaDangKy, cancellationToken);
    }

    public async Task<OperationResult<PaymentDto>> PayByStudentAsync(
        int maHocPhi,
        string maHocVien,
        StudentPayRequest request,
        CancellationToken cancellationToken)
    {
        var code = TextValue.Required(maHocVien);

        if (code is null)
        {
            return OperationResult<PaymentDto>.Forbidden(
                "Không xác định được học viên từ token.");
        }

        // ---- (1) Phiếu học phí phải thuộc CHÍNH học viên này ----
        var hocPhi = await _tuitionFeeStore.GetDetailAsync(maHocPhi, cancellationToken);

        // 404 thay vì 403 để không lộ sự tồn tại của phiếu của người khác.
        if (hocPhi is null || !string.Equals(hocPhi.MaHocVien, code, StringComparison.Ordinal))
        {
            return OperationResult<PaymentDto>.NotFound($"Không tìm thấy học phí có id = {maHocPhi}.");
        }

        if (string.Equals(hocPhi.TrangThaiHocPhi, TuitionStatuses.DaThanhToan, StringComparison.Ordinal)
            || hocPhi.SoTienConThieu <= 0m)
        {
            return OperationResult<PaymentDto>.Conflict(
                "Học phí này đã được thanh toán đủ, không cần đóng thêm.");
        }

        // ---- (2) Số tiền ----
        if (request.SoTien <= 0m)
        {
            return OperationResult<PaymentDto>.BadRequest(
                "Số tiền không hợp lệ.",
                ["Số tiền phải lớn hơn 0."]);
        }

        if (decimal.Round(request.SoTien, 2) != request.SoTien)
        {
            return OperationResult<PaymentDto>.BadRequest(
                "Số tiền không hợp lệ.",
                ["Số tiền chỉ được có tối đa 2 chữ số thập phân."]);
        }

        if (request.SoTien > hocPhi.SoTienConThieu)
        {
            return OperationResult<PaymentDto>.BadRequest(
                "Số tiền vượt quá số còn thiếu.",
                [$"Số còn thiếu của phiếu này là {hocPhi.SoTienConThieu:N0}."]);
        }

        // Nguyên tắc THU MỘT LẦN: học viên tự đóng phải đóng đúng số còn thiếu.
        if (decimal.Round(request.SoTien, 2) != decimal.Round(hocPhi.SoTienConThieu, 2))
        {
            return OperationResult<PaymentDto>.BadRequest(
                "Phải thanh toán đủ học phí trong MỘT lần.",
                [$"Số tiền cần thanh toán: {hocPhi.SoTienConThieu:N0} ₫."]);
        }

        // ---- (3) Phương thức (chỉ các phương thức chuẩn của cổng mô phỏng) ----
        var phuongThuc = TextValue.Required(request.PhuongThucThanhToan);

        if (phuongThuc is null || !PaymentMethods.IsValid(phuongThuc))
        {
            return OperationResult<PaymentDto>.BadRequest(
                "Phương thức thanh toán không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", PaymentMethods.StudentSelfServe)}."]);
        }

        // ---- (4) Khoá chống trùng ----
        var maGiaoDich = TextValue.Required(request.MaGiaoDich);

        if (maGiaoDich is null || maGiaoDich.Length is < 3 or > 150)
        {
            return OperationResult<PaymentDto>.BadRequest(
                "Mã giao dịch không hợp lệ.",
                ["Mã giao dịch phải từ 3 đến 150 ký tự."]);
        }

        // ---- (5) Người ghi nhận: nhân viên kế toán hệ thống ----
        if (!await _referenceStore.StaffExistsAsync(SystemCollectorCode, cancellationToken))
        {
            return OperationResult<PaymentDto>.BadRequest(
                "Thiếu nhân viên kế toán hệ thống.",
                [
                    $"Không tìm thấy nhân viên '{SystemCollectorCode}' trong bảng NhanVien. " +
                    "Hãy bổ sung nhân viên kế toán trước khi bật thanh toán mô phỏng."
                ]);
        }

        var data = new PaymentWriteData(
            MaHocPhi: maHocPhi,
            MaNhanVienKeToan: SystemCollectorCode,
            SoTien: request.SoTien,
            PhuongThucThanhToan: phuongThuc.Trim(),
            MaGiaoDich: maGiaoDich,
            LoaiGiaoDich: PaymentTypes.ThuTien,
            GhiChu: $"Thanh toán mô phỏng do học viên {code} thực hiện",
            NoiDungBienNhan: $"Thu học phí – {hocPhi.TenKhoaHoc}");

        var result = await _paymentStore.RecordAsync(data, cancellationToken);

        switch (result.Status)
        {
            case PaymentWriteStatus.TuitionNotFound:
                return OperationResult<PaymentDto>.NotFound(
                    $"Không tìm thấy học phí có id = {maHocPhi}.");

            case PaymentWriteStatus.ExceedsBalance:
                return OperationResult<PaymentDto>.Conflict(
                    $"Số tiền vượt quá số dư còn thiếu ({result.SoTienConThieu:N0}).");

            case PaymentWriteStatus.Duplicate:
                return await HandleDuplicateAsync(maHocPhi, result.MaThanhToan, cancellationToken);

            case PaymentWriteStatus.Conflict:
                return OperationResult<PaymentDto>.Conflict(
                    "Không thể ghi nhận thanh toán do xung đột dữ liệu. Vui lòng thử lại.");

            default:
                var created = await _paymentStore.GetDetailAsync(result.MaThanhToan, cancellationToken);

                if (created is null)
                {
                    return OperationResult<PaymentDto>.Conflict("Đã ghi thanh toán nhưng không đọc lại được giao dịch.");
                }

                // Thu đủ tiền ⇒ TỰ ĐỘNG xếp học viên vào lớp dự kiến.
                await ThuXepLopSauThanhToanAsync(maHocPhi, cancellationToken);

                return OperationResult<PaymentDto>.Created(created, "Thanh toán mô phỏng thành công.");
        }
    }

    public async Task<OperationResult<PagedResult<PaymentDto>>> ListForStudentAsync(
        string maHocVien,
        PaymentListQuery query,
        CancellationToken cancellationToken)
    {
        query.MaHocVien = maHocVien;
        query.MaKhoaHoc = null;

        var invalid = ValidateFilter(query);

        if (invalid is not null)
        {
            return invalid;
        }

        var page = await _paymentStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<PaymentDto>>.Ok(page);
    }

    public async Task<OperationResult<PaymentDto>> GetDetailForStudentAsync(
        int maThanhToan,
        string maHocVien,
        CancellationToken cancellationToken)
    {
        var detail = await _paymentStore.GetDetailAsync(maThanhToan, cancellationToken);

        if (detail is null || !string.Equals(detail.MaHocVien, maHocVien, StringComparison.Ordinal))
        {
            return OperationResult<PaymentDto>.NotFound($"Không tìm thấy giao dịch có id = {maThanhToan}.");
        }

        return OperationResult<PaymentDto>.Ok(detail);
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    /// <summary>Mã giao dịch đã tồn tại — trả về bản ghi cũ (idempotent replay) thay vì tạo mới.</summary>
    private async Task<OperationResult<PaymentDto>> HandleDuplicateAsync(
        int maHocPhi,
        int maThanhToan,
        CancellationToken cancellationToken)
    {
        var existing = await _paymentStore.GetDetailAsync(maThanhToan, cancellationToken);

        if (existing is null)
        {
            return OperationResult<PaymentDto>.Conflict(
                "Mã giao dịch đã tồn tại nhưng không đọc lại được bản ghi.");
        }

        if (existing.MaHocPhi != maHocPhi)
        {
            return OperationResult<PaymentDto>.Conflict(
                $"Mã giao dịch '{existing.MaGiaoDich}' đã được dùng cho học phí khác " +
                $"(id = {existing.MaHocPhi}).");
        }

        return OperationResult<PaymentDto>.Ok(existing);
    }

    private static OperationResult<PagedResult<PaymentDto>>? ValidateFilter(PaymentListQuery query)
    {
        if (!string.IsNullOrWhiteSpace(query.TrangThai)
            && !PaymentStatuses.IsValid(query.TrangThai.Trim()))
        {
            return OperationResult<PagedResult<PaymentDto>>.BadRequest(
                "Trạng thái giao dịch không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", PaymentStatuses.All)}."]);
        }

        if (!string.IsNullOrWhiteSpace(query.LoaiGiaoDich)
            && !PaymentTypes.IsValid(query.LoaiGiaoDich.Trim()))
        {
            return OperationResult<PagedResult<PaymentDto>>.BadRequest(
                "Loại giao dịch không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", PaymentTypes.All)}."]);
        }

        if (query.TuNgay is { } tu && query.DenNgay is { } den && den < tu)
        {
            return OperationResult<PagedResult<PaymentDto>>.BadRequest(
                "Khoảng ngày không hợp lệ.",
                ["Đến ngày phải lớn hơn hoặc bằng từ ngày."]);
        }

        return null;
    }
}
