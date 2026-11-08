using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Enrollments.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Enrollments;

/// <inheritdoc cref="IEnrollmentService"/>
/// <remarks>
/// GIỚI HẠN ĐÃ BIẾT: ưu đãi có <c>SoKhoaHocToiThieu &gt; 1</c> KHÔNG được áp dụng,
/// vì schema không có khóa nhóm đăng ký nên không xác định được các đăng ký cùng một giao dịch.
/// </remarks>
public sealed class EnrollmentService : IEnrollmentService
{
    private readonly IEnrollmentStore _enrollmentStore;
    private readonly IClassStore _classStore;
    private readonly ICourseStore _courseStore;
    private readonly IPromotionStore _promotionStore;
    private readonly IReferenceStore _referenceStore;

    public EnrollmentService(
        IEnrollmentStore enrollmentStore,
        IClassStore classStore,
        ICourseStore courseStore,
        IPromotionStore promotionStore,
        IReferenceStore referenceStore)
    {
        _enrollmentStore = enrollmentStore;
        _classStore = classStore;
        _courseStore = courseStore;
        _promotionStore = promotionStore;
        _referenceStore = referenceStore;
    }

    public async Task<OperationResult<PagedResult<EnrollmentListItemDto>>> ListAsync(
        EnrollmentListQuery query,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(query.TrangThai)
            && !EnrollmentStatuses.IsValid(query.TrangThai.Trim()))
        {
            return OperationResult<PagedResult<EnrollmentListItemDto>>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", EnrollmentStatuses.All)}."]);
        }

        var page = await _enrollmentStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<EnrollmentListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<EnrollmentDetailDto>> GetDetailAsync(
        int maDangKy,
        CancellationToken cancellationToken)
    {
        var detail = await _enrollmentStore.GetDetailAsync(maDangKy, cancellationToken);

        return detail is null
            ? OperationResult<EnrollmentDetailDto>.NotFound($"Không tìm thấy đăng ký có id = {maDangKy}.")
            : OperationResult<EnrollmentDetailDto>.Ok(detail);
    }

    public async Task<OperationResult<EnrollmentDetailDto>> CreateAsync(
        CreateEnrollmentRequest request,
        string currentUserTenDangNhap,
        CancellationToken cancellationToken)
    {
        var maHocVien = request.MaHocVien.Trim();

        // ---- (1) Học viên phải tồn tại và còn được đăng ký ----
        var trangThaiHocVien = await _referenceStore.GetStudentStatusAsync(maHocVien, cancellationToken);

        if (trangThaiHocVien is null)
        {
            return OperationResult<EnrollmentDetailDto>.BadRequest(
                "Học viên không tồn tại.",
                [$"Không tìm thấy học viên '{maHocVien}'."]);
        }

        if (trangThaiHocVien == StudentStatuses.DaNghi)
        {
            return OperationResult<EnrollmentDetailDto>.BadRequest(
                "Học viên đã nghỉ, không thể đăng ký học.",
                [$"Trạng thái học tập hiện tại: '{trangThaiHocVien}'."]);
        }

        // ---- (2) Khóa học phải tồn tại và đang nhận đăng ký ----
        var course = await _courseStore.GetTuitionInfoAsync(request.MaKhoaHoc, cancellationToken);

        if (course is null)
        {
            return OperationResult<EnrollmentDetailDto>.BadRequest(
                "Khóa học không tồn tại.",
                [$"Không tìm thấy khóa học có id = {request.MaKhoaHoc}."]);
        }

        if (!CourseStatuses.CanAcceptEnrollment(course.TrangThai))
        {
            return OperationResult<EnrollmentDetailDto>.BadRequest(
                "Khóa học không nhận đăng ký mới.",
                [$"Trạng thái khóa học hiện tại: '{course.TrangThai}'."]);
        }

        // ---- (3) Chống đăng ký trùng còn hiệu lực ----
        if (await _enrollmentStore.HasActiveDuplicateAsync(maHocVien, request.MaKhoaHoc, null, cancellationToken))
        {
            return OperationResult<EnrollmentDetailDto>.Conflict(
                $"Học viên '{maHocVien}' đã có đăng ký còn hiệu lực cho khóa học này.");
        }

        // ---- (4) Người tư vấn: LẤY TỪ JWT, không tin client ----
        var maNhanVienTuVan = currentUserTenDangNhap.Trim();

        if (!await _referenceStore.StaffExistsAsync(maNhanVienTuVan, cancellationToken))
        {
            return OperationResult<EnrollmentDetailDto>.BadRequest(
                "Tài khoản đang đăng nhập không có hồ sơ nhân viên.",
                [$"Không tìm thấy nhân viên '{maNhanVienTuVan}' để ghi nhận người tư vấn."]);
        }

        // ---- (5) Ưu đãi: server tự kiểm tra và tự tính số tiền giảm ----
        var promotion = await ResolvePromotionAsync(
            request.MaUuDai,
            course.HocPhiCoBan,
            allowRemove: false,
            cancellationToken);

        if (!promotion.Succeeded)
        {
            return OperationResult<EnrollmentDetailDto>.BadRequest(
                "Ưu đãi không hợp lệ.",
                [promotion.Error!]);
        }

        // ---- (6) LỚP DỰ KIẾN: bắt buộc, phải thuộc đúng khóa học ----
        if (request.MaLopDuKien <= 0)
        {
            return OperationResult<EnrollmentDetailDto>.BadRequest(
                "Lớp học dự kiến là bắt buộc.",
                ["Hãy chọn lớp học viên sẽ vào học. Học phí thu đủ sẽ tự xếp vào lớp này."]);
        }

        var lopDuKien = await _classStore.GetDetailAsync(request.MaLopDuKien, cancellationToken);

        if (lopDuKien is null)
        {
            return OperationResult<EnrollmentDetailDto>.BadRequest(
                "Lớp học dự kiến không tồn tại.",
                [$"Không tìm thấy lớp có id = {request.MaLopDuKien}."]);
        }

        if (lopDuKien.MaKhoaHoc != request.MaKhoaHoc)
        {
            return OperationResult<EnrollmentDetailDto>.BadRequest(
                "Lớp học không thuộc khóa học đã chọn.",
                [$"Lớp '{lopDuKien.MaLop}' thuộc khóa có id = {lopDuKien.MaKhoaHoc}, không phải {request.MaKhoaHoc}."]);
        }

        // ---- (7) KIỂM TRA LẦN 1: 4 quy tắc xếp lớp, ngay lúc tạo đăng ký ----
        // Báo lỗi sớm để người nhập liệu chọn lớp khác ngay, không phải chờ tới lúc thu tiền.
        var placementErrors = await CheckPlacementRulesForStudentAsync(
            maHocVien,
            request.MaLopDuKien,
            exceptDangKyId: null,
            cancellationToken);

        if (placementErrors.Count > 0)
        {
            return OperationResult<EnrollmentDetailDto>.BadRequest(
                "Không thể xếp học viên vào lớp dự kiến này.",
                placementErrors);
        }

        // ---- (8) Tạo đăng ký + phiếu học phí (CHƯA vào lớp — chờ thu tiền) ----
        var createData = new EnrollmentCreateData(
            MaHocVien: maHocVien,
            MaKhoaHoc: request.MaKhoaHoc,
            MaUuDai: request.MaUuDai,
            MaNhanVienTuVan: maNhanVienTuVan,
            HocPhiGoc: course.HocPhiCoBan,
            SoTienGiam: promotion.SoTienGiam,
            LichHocMongMuon: TextValue.Optional(request.LichHocMongMuon),
            GhiChu: TextValue.Optional(request.GhiChu))
        {
            MaLopDuKien = request.MaLopDuKien
        };

        var created = await _enrollmentStore.CreateAsync(createData, cancellationToken);

        if (!created.Succeeded)
        {
            return OperationResult<EnrollmentDetailDto>.Conflict("Không thể tạo đăng ký học.");
        }

        var detail = await _enrollmentStore.GetDetailAsync(created.Id, cancellationToken);

        return OperationResult<EnrollmentDetailDto>.Created(
            detail!,
            "Tạo đăng ký học thành công. Học phí thu đủ sẽ tự xếp vào lớp dự kiến.");
    }

    public async Task<OperationResult<EnrollmentDetailDto>> UpdateAsync(
        int maDangKy,
        UpdateEnrollmentRequest request,
        CancellationToken cancellationToken)
    {
        var current = await _enrollmentStore.GetDetailAsync(maDangKy, cancellationToken);

        if (current is null)
        {
            return OperationResult<EnrollmentDetailDto>.NotFound($"Không tìm thấy đăng ký có id = {maDangKy}.");
        }

        if (request.MaUuDai is null && request.BoUuDai != true && request.MaLopDuKien is null
            && request.LichHocMongMuon is null && request.GhiChu is null)
        {
            return OperationResult<EnrollmentDetailDto>.BadRequest(
                "Không có trường nào được cập nhật.",
                ["Hãy gửi ít nhất một trường cần thay đổi."]);
        }

        if (current.TrangThaiDangKy is EnrollmentStatuses.DaHuy or EnrollmentStatuses.HoanThanh)
        {
            return OperationResult<EnrollmentDetailDto>.Conflict(
                $"Không thể sửa đăng ký đang ở trạng thái '{current.TrangThaiDangKy}'.");
        }

        // ---- Đổi LỚP DỰ KIẾN: kiểm tra 4 quy tắc cho lớp mới ----
        var doiLopDuKien = request.MaLopDuKien is { } maLopMoi && maLopMoi != current.MaLopDuKien;

        if (request.MaLopDuKien is { } lopMoiId)
        {
            var lopMoi = await _classStore.GetDetailAsync(lopMoiId, cancellationToken);

            if (lopMoi is null)
            {
                return OperationResult<EnrollmentDetailDto>.BadRequest(
                    "Lớp học dự kiến không tồn tại.",
                    [$"Không tìm thấy lớp có id = {lopMoiId}."]);
            }

            if (lopMoi.MaKhoaHoc != current.MaKhoaHoc)
            {
                return OperationResult<EnrollmentDetailDto>.BadRequest(
                    "Lớp học không thuộc khóa học của đăng ký.",
                    [$"Lớp '{lopMoi.MaLop}' thuộc khóa có id = {lopMoi.MaKhoaHoc}, không phải {current.MaKhoaHoc}."]);
            }

            if (doiLopDuKien && current.MaLopHoc is null)
            {
                var loiXepLop = await CheckPlacementRulesForStudentAsync(
                    current.MaHocVien,
                    lopMoiId,
                    exceptDangKyId: maDangKy,
                    cancellationToken);

                if (loiXepLop.Count > 0)
                {
                    return OperationResult<EnrollmentDetailDto>.BadRequest(
                        "Không thể đổi sang lớp dự kiến này.",
                        loiXepLop);
                }
            }
        }

        // ---- Xác định ưu đãi + số tiền giảm sau khi sửa ----
        var targetPromotionId = request.BoUuDai == true ? null : (request.MaUuDai ?? current.MaUuDai);

        var promotion = await ResolvePromotionAsync(
            targetPromotionId,
            current.HocPhiGoc,
            allowRemove: targetPromotionId is null,
            cancellationToken);

        if (!promotion.Succeeded)
        {
            return OperationResult<EnrollmentDetailDto>.BadRequest("Ưu đãi không hợp lệ.", [promotion.Error!]);
        }

        var updateData = new EnrollmentUpdateData(
            MaUuDai: targetPromotionId,
            SoTienGiam: promotion.SoTienGiam,
            LichHocMongMuon: request.LichHocMongMuon is not null
                ? TextValue.Optional(request.LichHocMongMuon)
                : current.LichHocMongMuon,
            GhiChu: request.GhiChu is not null
                ? TextValue.Optional(request.GhiChu)
                : current.GhiChu)
        {
            MaLopDuKien = request.MaLopDuKien
        };

        var outcome = await _enrollmentStore.UpdateAsync(maDangKy, updateData, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return outcome == WriteOutcome.NotFound
                ? OperationResult<EnrollmentDetailDto>.NotFound($"Không tìm thấy đăng ký có id = {maDangKy}.")
                : OperationResult<EnrollmentDetailDto>.Conflict("Không thể cập nhật đăng ký.");
        }

        // ---- Đã thu đủ tiền mà vừa đổi lớp dự kiến → TỰ XẾP LỚP NGAY ----
        // Đây là "đường sửa" khi lớp dự kiến cũ đã đầy / quá 4 buổi / trùng giờ.
        if (doiLopDuKien && current.MaLopHoc is null && request.MaLopDuKien is { } lopSauKhiSua)
        {
            var loiTuXep = await TryAutoPlaceAsync(maDangKy, lopSauKhiSua, cancellationToken);

            if (loiTuXep is not null)
            {
                return OperationResult<EnrollmentDetailDto>.BadRequest(
                    "Đã đổi lớp dự kiến nhưng CHƯA xếp được lớp.",
                    [loiTuXep]);
            }
        }

        var updated = await _enrollmentStore.GetDetailAsync(maDangKy, cancellationToken);

        return OperationResult<EnrollmentDetailDto>.Ok(updated!);
    }

    /// <summary>Bỏ lớp: trả đăng ký về "Chờ xử lý" để xếp lại lớp khác.</summary>
    public async Task<OperationResult<EnrollmentDetailDto>> UnassignClassAsync(
        int maDangKy,
        CancellationToken cancellationToken)
    {
        var current = await _enrollmentStore.GetDetailAsync(maDangKy, cancellationToken);

        if (current is null)
        {
            return OperationResult<EnrollmentDetailDto>.NotFound($"Không tìm thấy đăng ký có id = {maDangKy}.");
        }

        if (current.TrangThaiDangKy is EnrollmentStatuses.DaHuy or EnrollmentStatuses.HoanThanh)
        {
            return OperationResult<EnrollmentDetailDto>.Conflict(
                $"Không thể bỏ lớp cho đăng ký ở trạng thái '{current.TrangThaiDangKy}'.");
        }

        var outcome = await _enrollmentStore.UnassignClassAsync(maDangKy, cancellationToken);

        if (outcome == WriteOutcome.Success)
        {
            var detail = await _enrollmentStore.GetDetailAsync(maDangKy, cancellationToken);
            return OperationResult<EnrollmentDetailDto>.Ok(detail!);
        }

        if (outcome == WriteOutcome.NotFound)
        {
            return OperationResult<EnrollmentDetailDto>.NotFound($"Không tìm thấy đăng ký có id = {maDangKy}.");
        }

        return OperationResult<EnrollmentDetailDto>.Conflict("Đăng ký này chưa được xếp lớp.");
    }

    public async Task<OperationResult<EnrollmentDetailDto>> AssignClassAsync(
        int maDangKy,
        AssignClassRequest request,
        CancellationToken cancellationToken)
    {
        var current = await _enrollmentStore.GetDetailAsync(maDangKy, cancellationToken);

        if (current is null)
        {
            return OperationResult<EnrollmentDetailDto>.NotFound($"Không tìm thấy đăng ký có id = {maDangKy}.");
        }

        if (current.TrangThaiDangKy is EnrollmentStatuses.DaHuy or EnrollmentStatuses.HoanThanh)
        {
            return OperationResult<EnrollmentDetailDto>.Conflict(
                $"Không thể xếp lớp cho đăng ký ở trạng thái '{current.TrangThaiDangKy}'.");
        }
        // QUY TẮC XẾP LỚP: độ tuổi, trùng thời gian với lớp khác, lớp đã gần kết thúc.
        var ruleErrors = await CheckPlacementRulesAsync(maDangKy, request.MaLopHoc, cancellationToken);

        if (ruleErrors.Count > 0)
        {
            return OperationResult<EnrollmentDetailDto>.BadRequest("Không thể xếp lớp.", ruleErrors);
        }
        var outcome = await _enrollmentStore.AssignClassAsync(maDangKy, request.MaLopHoc, cancellationToken);
        if (outcome == AssignClassOutcome.Success)
        {
            var detail = await _enrollmentStore.GetDetailAsync(maDangKy, cancellationToken);
            return OperationResult<EnrollmentDetailDto>.Ok(detail!);
        }

        if (outcome == AssignClassOutcome.EnrollmentNotFound)
        {
            return OperationResult<EnrollmentDetailDto>.NotFound($"Không tìm thấy đăng ký có id = {maDangKy}.");
        }

        if (outcome is AssignClassOutcome.ClassNotFound)
        {
            return OperationResult<EnrollmentDetailDto>.NotFound(
                $"Không tìm thấy lớp có id = {request.MaLopHoc}.");
        }

        // Mô tả CHÍNH XÁC phần dữ liệu không khớp để admin biết sửa ở đâu.
        var lop = await _classStore.GetDetailAsync(request.MaLopHoc, cancellationToken);
        var tenLop = lop is null ? $"id = {request.MaLopHoc}" : $"'{lop.MaLop} — {lop.TenLop}'";

        switch (outcome)
        {
            case AssignClassOutcome.WrongCourse:
                return OperationResult<EnrollmentDetailDto>.BadRequest(
                    "Không thể xếp lớp: khác khóa học.",
                    [
                        $"Lớp {tenLop} thuộc khóa '{lop?.TenKhoaHoc}', "
                        + $"còn đăng ký của '{current.TenHocVien}' là khóa '{current.TenKhoaHoc}'.",
                        "Hãy chọn lớp CÙNG khóa học với đăng ký (hoặc tạo lớp mới cho khóa này)."
                    ]);

            case AssignClassOutcome.ClassClosed:
                return OperationResult<EnrollmentDetailDto>.BadRequest(
                    "Không thể xếp lớp: lớp không nhận học viên.",
                    [
                        $"Lớp {tenLop} đang ở trạng thái '{lop?.TrangThai}'. "
                        + $"Chỉ lớp ở trạng thái {string.Join(" / ", ClassStatuses.AcceptingStudents)} mới nhận học viên."
                    ]);

            case AssignClassOutcome.ClassFull:
                return OperationResult<EnrollmentDetailDto>.Conflict(
                    $"Không thể xếp lớp: {tenLop} đã đủ sĩ số "
                    + $"({lop?.SiSoHienTai}/{lop?.SiSoToiDa} học viên).");

            case AssignClassOutcome.AlreadyAssigned:
                return OperationResult<EnrollmentDetailDto>.Conflict(
                    $"Đăng ký này đã được xếp vào lớp '{current.MaLop}' trước đó. "
                    + "Hãy dùng 'Rút khỏi lớp' nếu muốn xếp lại.");
        }

        return OperationResult<EnrollmentDetailDto>.BadRequest(
            "Không thể xếp lớp.",
            [DescribeAssignFailure(outcome)]);
    }

    public async Task<OperationResult<EnrollmentDetailDto>> UpdateStatusAsync(
        int maDangKy,
        UpdateStatusRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = TextValue.Required(request.TrangThai);

        if (trangThai is null || !EnrollmentStatuses.IsValid(trangThai))
        {
            return OperationResult<EnrollmentDetailDto>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", EnrollmentStatuses.All)}."]);
        }

        if (!await _enrollmentStore.ExistsAsync(maDangKy, cancellationToken))
        {
            return OperationResult<EnrollmentDetailDto>.NotFound($"Không tìm thấy đăng ký có id = {maDangKy}.");
        }

        // "Đã xếp lớp" chỉ đạt được qua endpoint assign-class (cần kiểm tra sĩ số).
        if (trangThai == EnrollmentStatuses.DaXepLop)
        {
            return OperationResult<EnrollmentDetailDto>.BadRequest(
                "Trạng thái không hợp lệ ở endpoint này.",
                ["Hãy dùng PATCH .../assign-class để xếp lớp."]);
        }

        var outcome = await _enrollmentStore.UpdateStatusAsync(maDangKy, trangThai, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return OperationResult<EnrollmentDetailDto>.Conflict("Không thể cập nhật trạng thái đăng ký.");
        }

        var updated = await _enrollmentStore.GetDetailAsync(maDangKy, cancellationToken);

        return OperationResult<EnrollmentDetailDto>.Ok(updated!);
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    /// <summary>Kết quả tính giảm giá ở phía server.</summary>
    private readonly record struct PromotionResolution(bool Succeeded, decimal SoTienGiam, string? Error);

    /// <summary>
    /// Kiểm tra ưu đãi và TÍNH số tiền giảm. Client không bao giờ gửi số tiền giảm.
    /// </summary>
    private async Task<PromotionResolution> ResolvePromotionAsync(
        int? maUuDai,
        decimal hocPhiGoc,
        bool allowRemove,
        CancellationToken cancellationToken)
    {
        if (maUuDai is null)
        {
            return allowRemove
                ? new PromotionResolution(true, 0m, null)
                : new PromotionResolution(true, 0m, null);
        }

        var promotion = await _promotionStore.GetForApplyAsync(maUuDai.Value, cancellationToken);

        if (promotion is null)
        {
            return new PromotionResolution(false, 0m, $"Không tìm thấy ưu đãi có id = {maUuDai}.");
        }

        if (promotion.TrangThai != ActivityStatuses.HoatDong)
        {
            return new PromotionResolution(
                false, 0m, $"Ưu đãi '{promotion.TenUuDai}' đang ở trạng thái '{promotion.TrangThai}'.");
        }

        var today = DateOnly.FromDateTime(DateTime.Today);

        if (today < promotion.NgayBatDau || today > promotion.NgayKetThuc)
        {
            return new PromotionResolution(
                false, 0m,
                $"Ưu đãi '{promotion.TenUuDai}' không nằm trong thời hạn áp dụng " +
                $"({promotion.NgayBatDau:dd/MM/yyyy} - {promotion.NgayKetThuc:dd/MM/yyyy}).");
        }

        if (promotion.SoKhoaHocToiThieu > 1)
        {
            return new PromotionResolution(
                false, 0m,
                $"Ưu đãi '{promotion.TenUuDai}' yêu cầu từ {promotion.SoKhoaHocToiThieu} khóa học trở lên. " +
                "Hiện chưa hỗ trợ: schema không có khóa nhóm đăng ký nên không xác định được " +
                "các đăng ký thuộc cùng một giao dịch.");
        }

        var soTienGiam = PromotionTypes.IsPercent(promotion.LoaiGiamGia)
            ? Math.Round(hocPhiGoc * promotion.GiaTriGiam / 100m, 2, MidpointRounding.AwayFromZero)
            : promotion.GiaTriGiam;

        if (soTienGiam > hocPhiGoc)
        {
            return new PromotionResolution(
                false, 0m,
                $"Số tiền giảm ({soTienGiam:N0}) vượt quá học phí ({hocPhiGoc:N0}).");
        }

        return new PromotionResolution(true, soTienGiam, null);
    }

    private static string DescribeAssignFailure(AssignClassOutcome outcome) => outcome switch
    {
        AssignClassOutcome.ClassFull => "Lớp đã đủ sĩ số.",
        AssignClassOutcome.ClassClosed => "Lớp không ở trạng thái nhận học viên.",
        AssignClassOutcome.WrongCourse => "Lớp được chọn không thuộc khóa học của đăng ký.",
        AssignClassOutcome.AlreadyAssigned => "Đăng ký này đã được xếp lớp trước đó.",
        AssignClassOutcome.ClassNotFound => "Lớp không tồn tại.",
        AssignClassOutcome.EnrollmentNotFound => "Đăng ký không tồn tại.",
        _ => "Ràng buộc dữ liệu không cho phép xếp lớp."
    };

    /// <summary>
    /// Kiểm tra 5 quy tắc xếp lớp:
    /// <list type="number">
    ///   <item>Sĩ số: số đã xếp lớp + số đang giữ chỗ phải còn dưới sĩ số tối đa.</item>
    ///   <item>Độ tuổi học viên phải nằm trong khoảng tuổi của chương trình.</item>
    ///   <item>Không trùng thứ/giờ với lớp khác mà học viên đang học.</item>
    ///   <item>Lớp chưa đi quá số buổi cho phép (tránh vào lớp khi gần kết thúc).</item>
    ///   <item>Học phí phải được thu ĐỦ trước khi vào lớp.</item>
    /// </list>
    /// </summary>
    private async Task<IReadOnlyList<string>> CheckPlacementRulesAsync(
        int maDangKy,
        int maLopHoc,
        CancellationToken cancellationToken)
    {
        var context = await _enrollmentStore.GetPlacementContextAsync(maDangKy, maLopHoc, cancellationToken);

        return context is null ? [] : CollectPlacementErrors(context, kiemTraThanhToan: true);
    }

    /// <summary>
    /// Kiểm tra 5 quy tắc xếp lớp theo HỌC VIÊN — dùng lúc TẠO đăng ký (chưa có MaDangKy).
    /// <paramref name="exceptDangKyId"/> để bỏ qua chính đăng ký đang sửa.
    /// </summary>
    private async Task<IReadOnlyList<string>> CheckPlacementRulesForStudentAsync(
        string maHocVien,
        int maLopHoc,
        int? exceptDangKyId,
        CancellationToken cancellationToken)
    {
        var context = await _enrollmentStore.GetPlacementContextForStudentAsync(
            maHocVien,
            maLopHoc,
            exceptDangKyId,
            cancellationToken);

        if (context is null)
        {
            return ["Không đọc được dữ liệu học viên / lớp học để kiểm tra điều kiện xếp lớp."];
        }

        // KHÔNG kiểm tra học phí ở đây: lúc TẠO đăng ký chưa có phiếu, và khi ĐỔI lớp dự kiến
        // thì việc chưa thu tiền không phải lỗi — tiền chỉ là điều kiện để VÀO lớp thật.
        return CollectPlacementErrors(context, kiemTraThanhToan: false);
    }

    /// <summary>
    /// Tự xếp lớp sau khi thu đủ học phí.
    /// <para>
    /// Trả về <c>null</c> = KHÔNG có lỗi (hoặc chưa đủ tiền nên chưa xếp — để chờ),
    /// trả về chuỗi = lý do không xếp được để hiển thị cho người dùng.
    /// </para>
    /// </summary>
    public async Task<string?> TryAutoPlaceIfPaidAsync(
        int maDangKy,
        CancellationToken cancellationToken)
    {
        var current = await _enrollmentStore.GetDetailAsync(maDangKy, cancellationToken);

        if (current is null)
        {
            return null;
        }

        // Đã ở trong lớp, đã huỷ/hoàn thành, hoặc không có lớp dự kiến → không làm gì.
        if (current.MaLopHoc is not null
            || current.TrangThaiDangKy is EnrollmentStatuses.DaHuy or EnrollmentStatuses.HoanThanh
            || current.MaLopDuKien is not { } maLopDuKien)
        {
            return null;
        }

        return await TryAutoPlaceAsync(maDangKy, maLopDuKien, cancellationToken);
    }

    /// <summary>
    /// Tự xếp lớp sau khi thu đủ học phí.
    /// <para>
    /// Trả về <c>null</c> = KHÔNG có lỗi (hoặc chưa đủ tiền nên chưa xếp — để chờ),
    /// trả về chuỗi = lý do không xếp được để hiển thị cho người dùng.
    /// </para>
    /// </summary>
    private async Task<string?> TryAutoPlaceAsync(
        int maDangKy,
        int maLopHoc,
        CancellationToken cancellationToken)
    {
        var context = await _enrollmentStore.GetPlacementContextAsync(maDangKy, maLopHoc, cancellationToken);

        if (context is null)
        {
            return "Không đọc được dữ liệu đăng ký / lớp học.";
        }

        // Chưa thu đủ → chỉ chờ, KHÔNG coi là lỗi.
        if (context.SoTienConThieuHocPhi is not { } conThieu || conThieu > 0m)
        {
            return null;
        }

        var errors = CollectPlacementErrors(context, kiemTraThanhToan: true);

        if (errors.Count > 0)
        {
            return string.Join(" ", errors);
        }

        var outcome = await _enrollmentStore.AssignClassAsync(maDangKy, maLopHoc, cancellationToken);

        return outcome == AssignClassOutcome.Success ? null : DescribeAssignFailure(outcome);
    }

    /// <summary>Bộ quy tắc xếp lớp dùng chung cho mọi thời điểm kiểm tra.</summary>
    /// <param name="kiemTraThanhToan">
    /// <c>true</c> chỉ khi chuẩn bị VÀO LỚP THẬT (xếp lớp / tự xếp sau khi thu tiền).
    /// Lúc tạo đăng ký hoặc đổi lớp dự kiến thì để <c>false</c> — chưa thu tiền không phải lỗi.
    /// </param>
    private static List<string> CollectPlacementErrors(PlacementContext context, bool kiemTraThanhToan)
    {
        var errors = new List<string>();

        // ---- (1) Sĩ số: đã xếp lớp + đang giữ chỗ ----
        var daChiemCho = context.SoDaXepLop + context.SoGiuCho;

        if (daChiemCho >= context.SiSoToiDa)
        {
            errors.Add(
                $"Lớp '{context.MaLop}' đã kín chỗ: {context.SoDaXepLop} đã xếp lớp + "
                + $"{context.SoGiuCho} đang giữ chỗ / tối đa {context.SiSoToiDa}.");
        }

        // ---- (2) Độ tuổi tại ngày khai giảng của lớp ----
        var tuoi = TinhTuoi(context.NgaySinh, context.NgayBatDauLop);

        if (context.DoTuoiTu is { } tuoiTu && tuoi < tuoiTu)
        {
            errors.Add(
                $"Học viên {tuoi} tuổi (sinh {context.NgaySinh:dd/MM/yyyy}) nhỏ hơn độ tuổi tối thiểu " +
                $"{tuoiTu} của chương trình — không thể xếp vào lớp '{context.MaLop}'.");
        }

        if (context.DoTuoiDen is { } tuoiDen && tuoi > tuoiDen)
        {
            errors.Add(
                $"Học viên {tuoi} tuổi (sinh {context.NgaySinh:dd/MM/yyyy}) lớn hơn độ tuổi tối đa " +
                $"{tuoiDen} của chương trình — không thể xếp vào lớp '{context.MaLop}'.");
        }

        // ---- (3) Trùng thời gian với lớp khác ----
        foreach (var slot in context.LopMoiSlots)
        {
            foreach (var other in context.LopDangHocSlots)
            {
                if (other.MaLopHoc == slot.MaLopHoc || other.ThuTrongTuan != slot.ThuTrongTuan)
                {
                    continue;
                }

                // Giờ phải chồng lấn (biên bằng nhau không tính là trùng).
                if (slot.GioBatDau >= other.GioKetThuc || other.GioBatDau >= slot.GioKetThuc)
                {
                    continue;
                }

                // Khoảng ngày áp dụng phải chồng lấn.
                var ketThucMoi = slot.NgayApDungDen ?? context.NgayKetThucDuKien;
                var ketThucCu = other.NgayApDungDen ?? context.NgayKetThucDuKien;

                if (slot.NgayApDungTu > ketThucCu || other.NgayApDungTu > ketThucMoi)
                {
                    continue;
                }

                errors.Add(
                    $"Trùng lịch với lớp '{other.MaLop}': {TenThu(slot.ThuTrongTuan)} " +
                    $"{slot.GioBatDau:HH\\:mm}–{slot.GioKetThuc:HH\\:mm}.");
            }
        }

        // ---- (4) Lớp đã gần kết thúc ----
        if (context.SoBuoiDaDienRa > EnrollmentRules.SoBuoiDaDienRaToiDa)
        {
            errors.Add(
                $"Lớp '{context.MaLop}' đã diễn ra {context.SoBuoiDaDienRa}/{context.TongSoBuoi} buổi " +
                $"(quá {EnrollmentRules.SoBuoiDaDienRaToiDa} buổi) — không nhận thêm học viên.");
        }

        // ---- (5) THANH TOÁN TRƯỚC KHI VÀO HỌC ----
        // Trung tâm thu học phí TRƯỚC khi xếp lớp ⇒ hồ sơ không bao giờ phát sinh công nợ.
        if (kiemTraThanhToan && EnrollmentRules.BatBuocThanhToanTruocKhiXepLop)
        {
            if (context.SoTienConThieuHocPhi is null)
            {
                errors.Add(
                    $"Chưa có phiếu học phí cho đăng ký này — hãy tạo và thu tiền trước khi xếp vào lớp '{context.MaLop}'.");
            }
            else if (context.SoTienConThieuHocPhi > 0m)
            {
                errors.Add(
                    $"Học phí còn thiếu {context.SoTienConThieuHocPhi:N0} ₫ "
                    + $"(trạng thái: {context.TrangThaiHocPhi}) — phải thanh toán đủ trước khi vào lớp '{context.MaLop}'.");
            }
        }

        return errors.Distinct().ToList();
    }

    private static int TinhTuoi(DateOnly ngaySinh, DateOnly moc)
    {
        var tuoi = moc.Year - ngaySinh.Year;

        if (moc < ngaySinh.AddYears(tuoi))
        {
            tuoi--;
        }

        return tuoi;
    }

    private static string TenThu(int thu) => thu switch
    {
        1 => "Thứ Hai",
        2 => "Thứ Ba",
        3 => "Thứ Tư",
        4 => "Thứ Năm",
        5 => "Thứ Sáu",
        6 => "Thứ Bảy",
        7 => "Chủ Nhật",
        _ => $"Thứ {thu}"
    };
}
