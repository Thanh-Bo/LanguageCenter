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

        // ---- (6) Tạo đăng ký (chưa xếp lớp) ----
        var createData = new EnrollmentCreateData(
            MaHocVien: maHocVien,
            MaKhoaHoc: request.MaKhoaHoc,
            MaUuDai: request.MaUuDai,
            MaNhanVienTuVan: maNhanVienTuVan,
            HocPhiGoc: course.HocPhiCoBan,
            SoTienGiam: promotion.SoTienGiam,
            LichHocMongMuon: TextValue.Optional(request.LichHocMongMuon),
            GhiChu: TextValue.Optional(request.GhiChu));

        var created = await _enrollmentStore.CreateAsync(createData, cancellationToken);

        if (!created.Succeeded)
        {
            return OperationResult<EnrollmentDetailDto>.Conflict("Không thể tạo đăng ký học.");
        }

        // ---- (7) Nếu client chỉ định lớp thì xếp lớp luôn ----
        if (request.MaLopHoc is { } maLopHoc)
        {
            var assignOutcome = await _enrollmentStore.AssignClassAsync(created.Id, maLopHoc, cancellationToken);

            if (assignOutcome != AssignClassOutcome.Success)
            {
                // Hoàn tác: xóa đăng ký vừa tạo để không để lại bản ghi mồ côi.
                await _enrollmentStore.DeleteAsync(created.Id, cancellationToken);

                return OperationResult<EnrollmentDetailDto>.BadRequest(
                    "Không thể xếp lớp.",
                    [DescribeAssignFailure(assignOutcome)]);
            }
        }

        var detail = await _enrollmentStore.GetDetailAsync(created.Id, cancellationToken);

        return OperationResult<EnrollmentDetailDto>.Created(detail!, "Tạo đăng ký học thành công.");
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

        if (request.MaUuDai is null && request.BoUuDai != true
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
                : current.GhiChu);

        var outcome = await _enrollmentStore.UpdateAsync(maDangKy, updateData, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return outcome == WriteOutcome.NotFound
                ? OperationResult<EnrollmentDetailDto>.NotFound($"Không tìm thấy đăng ký có id = {maDangKy}.")
                : OperationResult<EnrollmentDetailDto>.Conflict("Không thể cập nhật đăng ký.");
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

        if (outcome == AssignClassOutcome.ClassFull)
        {
            return OperationResult<EnrollmentDetailDto>.Conflict(DescribeAssignFailure(outcome));
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
}
