using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.TeachingAssignments.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.TeachingAssignments;

/// <inheritdoc cref="ITeachingAssignmentService"/>
/// <remarks>
/// QUY TẮC NGHIỆP VỤ:
/// <list type="bullet">
///   <item>Được phép tạo "khung" phân công <b>chưa gắn người</b> (trạng thái <c>Chờ phân công</c>).</item>
///   <item>Một người không thể giữ 2 vai trò (giáo viên chính + trợ giảng) trong cùng một lớp.</item>
///   <item>Chặn trùng lịch dạy giữa các lớp khác nhau — so sánh trên <c>LichHoc</c> (thứ, giờ, khoảng ngày).</item>
///   <item>Thời gian phân công luôn lấy theo ngày của lớp, client không tự đặt.</item>
/// </list>
/// </remarks>
public sealed class TeachingAssignmentService : ITeachingAssignmentService
{
    private readonly ITeachingAssignmentStore _assignmentStore;
    private readonly IClassStore _classStore;
    private readonly IReferenceStore _referenceStore;

    public TeachingAssignmentService(
        ITeachingAssignmentStore assignmentStore,
        IClassStore classStore,
        IReferenceStore referenceStore)
    {
        _assignmentStore = assignmentStore;
        _classStore = classStore;
        _referenceStore = referenceStore;
    }

    public async Task<OperationResult<PagedResult<AssignmentListItemDto>>> ListAsync(
        AssignmentListQuery query,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(query.TrangThai) && !AssignmentStatuses.IsValid(query.TrangThai.Trim()))
        {
            return OperationResult<PagedResult<AssignmentListItemDto>>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", AssignmentStatuses.All)}."]);
        }

        var page = await _assignmentStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<AssignmentListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<AssignmentDetailDto>> GetDetailAsync(
        int maPhanCong,
        CancellationToken cancellationToken)
    {
        var detail = await _assignmentStore.GetDetailAsync(maPhanCong, cancellationToken);

        return detail is null
            ? OperationResult<AssignmentDetailDto>.NotFound($"Không tìm thấy phân công có id = {maPhanCong}.")
            : OperationResult<AssignmentDetailDto>.Ok(detail);
    }

    public async Task<OperationResult<AssignmentDetailDto>> CreateAsync(
        CreateAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        var errors = new List<string>();
        var vaiTro = TextValue.Required(request.VaiTroPhanCong);

        if (!AssignmentRoles.IsValid(vaiTro))
        {
            errors.Add($"Vai trò phân công không hợp lệ. Chỉ chấp nhận: {string.Join(", ", AssignmentRoles.All)}.");
        }

        if (errors.Count > 0)
        {
            return OperationResult<AssignmentDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        // THỜI GIAN PHÂN CÔNG LUÔN THEO LỚP HỌC — không cho client tự đặt.
        // Tránh trường hợp lớp chạy 2 tháng nhưng phân công chỉ ghi 10 ngày,
        // hoặc phân công kéo dài quá ngày kết thúc của lớp.
        var lop = await _classStore.GetDetailAsync(request.MaLopHoc, cancellationToken);

        if (lop is null)
        {
            return OperationResult<AssignmentDetailDto>.BadRequest(
                "Lớp học không tồn tại.",
                [$"Không tìm thấy lớp có id = {request.MaLopHoc}."]);
        }

        request.NgayBatDau = lop.NgayBatDau;
        request.NgayKetThuc = lop.NgayKetThucDuKien;

        // KHUNG PHÂN CÔNG: cho phép để trống người — tạo trước, gắn giáo viên / trợ giảng sau.
        var maGiaoVien = TextValue.Optional(request.MaGiaoVien);

        if (maGiaoVien is null)
        {
            if (await _assignmentStore.PlaceholderExistsAsync(request.MaLopHoc, vaiTro!, cancellationToken))
            {
                return OperationResult<AssignmentDetailDto>.Conflict(
                    $"Lớp này đã có khung phân công '{vaiTro}' đang chờ phân công.");
            }

            request.MaGiaoVien = null;
        }
        else
        {
            var conflict = await KiemTraNguoiDuocPhanCongAsync(
                request.MaLopHoc, vaiTro!, maGiaoVien,
                request.NgayBatDau, request.NgayKetThuc,
                exceptId: null, cancellationToken);

            if (conflict is not null)
            {
                return conflict;
            }

            request.MaGiaoVien = maGiaoVien;
        }

        var result = await _assignmentStore.CreateAsync(request, cancellationToken);

        if (!result.Succeeded)
        {
            return OperationResult<AssignmentDetailDto>.Conflict("Không thể tạo phân công.");
        }

        var created = await _assignmentStore.GetDetailAsync(result.Id, cancellationToken);

        return OperationResult<AssignmentDetailDto>.Created(created!, "Tạo phân công thành công.");
    }

    public async Task<OperationResult<AssignmentDetailDto>> UpdateAsync(
        int maPhanCong,
        UpdateAssignmentRequest request,
        CancellationToken cancellationToken)
    {
        var current = await _assignmentStore.GetDetailAsync(maPhanCong, cancellationToken);

        if (current is null)
        {
            return OperationResult<AssignmentDetailDto>.NotFound($"Không tìm thấy phân công có id = {maPhanCong}.");
        }

        if (IsEmptyUpdate(request))
        {
            return OperationResult<AssignmentDetailDto>.BadRequest(
                "Không có trường nào được cập nhật.",
                ["Hãy gửi ít nhất một trường cần thay đổi."]);
        }

        var errors = new List<string>();
        var vaiTro = request.VaiTroPhanCong is not null
            ? TextValue.Required(request.VaiTroPhanCong)
            : current.VaiTroPhanCong;

        if (!AssignmentRoles.IsValid(vaiTro))
        {
            errors.Add($"Vai trò phân công không hợp lệ. Chỉ chấp nhận: {string.Join(", ", AssignmentRoles.All)}.");
        }

        var ngayBatDau = request.NgayBatDau ?? current.NgayBatDau;
        var ngayKetThuc = request.NgayKetThuc ?? current.NgayKetThuc;

        ValidateDates(ngayBatDau, ngayKetThuc, errors);

        if (errors.Count > 0)
        {
            return OperationResult<AssignmentDetailDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        var maGiaoVien = request.MaGiaoVien is not null
            ? TextValue.Optional(request.MaGiaoVien)   // "" = bỏ gán người
            : current.MaGiaoVien;

        if (maGiaoVien is not null)
        {
            var conflict = await KiemTraNguoiDuocPhanCongAsync(
                current.MaLopHoc, vaiTro!, maGiaoVien,
                ngayBatDau, ngayKetThuc,
                maPhanCong, cancellationToken);

            if (conflict is not null)
            {
                return conflict;
            }
        }

        if (request.MaGiaoVien is not null)
        {
            request.MaGiaoVien = maGiaoVien ?? string.Empty;
        }

        var outcome = await _assignmentStore.UpdateAsync(maPhanCong, request, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return outcome == WriteOutcome.NotFound
                ? OperationResult<AssignmentDetailDto>.NotFound($"Không tìm thấy phân công có id = {maPhanCong}.")
                : OperationResult<AssignmentDetailDto>.Conflict("Không thể cập nhật phân công.");
        }

        var updated = await _assignmentStore.GetDetailAsync(maPhanCong, cancellationToken);

        return OperationResult<AssignmentDetailDto>.Ok(updated!);
    }

    public async Task<OperationResult<AssignmentDetailDto>> UpdateStatusAsync(
        int maPhanCong,
        UpdateStatusRequest request,
        CancellationToken cancellationToken)
    {
        var trangThai = TextValue.Required(request.TrangThai);

        if (trangThai is null || !AssignmentStatuses.IsValid(trangThai))
        {
            return OperationResult<AssignmentDetailDto>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", AssignmentStatuses.All)}."]);
        }

        if (!await _assignmentStore.ExistsAsync(maPhanCong, cancellationToken))
        {
            return OperationResult<AssignmentDetailDto>.NotFound($"Không tìm thấy phân công có id = {maPhanCong}.");
        }

        var outcome = await _assignmentStore.UpdateStatusAsync(maPhanCong, trangThai, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return OperationResult<AssignmentDetailDto>.Conflict("Không thể cập nhật trạng thái phân công.");
        }

        var updated = await _assignmentStore.GetDetailAsync(maPhanCong, cancellationToken);

        return OperationResult<AssignmentDetailDto>.Ok(updated!);
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    private static void ValidateDates(DateOnly ngayBatDau, DateOnly? ngayKetThuc, List<string> errors)
    {
        if (ngayBatDau == default)
        {
            errors.Add("Ngày bắt đầu là bắt buộc.");
        }

        if (ngayKetThuc is { } ketThuc && ngayBatDau != default && ketThuc < ngayBatDau)
        {
            errors.Add("Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.");
        }
    }

    /// <summary>
    /// Kiểm tra một người có được phép nhận vai trò này của lớp hay không:
    /// đúng loại hồ sơ, không trùng vai trò khác trong cùng lớp, không trùng lịch dạy lớp khác.
    /// </summary>
    /// <returns><c>null</c> nếu hợp lệ; ngược lại là kết quả lỗi cần trả về client.</returns>
    private async Task<OperationResult<AssignmentDetailDto>?> KiemTraNguoiDuocPhanCongAsync(
        int maLopHoc,
        string vaiTro,
        string maGiaoVien,
        DateOnly ngayBatDau,
        DateOnly? ngayKetThuc,
        int? exceptId,
        CancellationToken cancellationToken)
    {
        var loaiGiaoVien = await _referenceStore.GetTeacherTypeAsync(maGiaoVien, cancellationToken);

        if (loaiGiaoVien is null)
        {
            return OperationResult<AssignmentDetailDto>.BadRequest(
                "Giáo viên không tồn tại.",
                [$"Không tìm thấy giáo viên / trợ giảng '{maGiaoVien}'."]);
        }

        var roleError = CheckTeacherTypeMatch(vaiTro, maGiaoVien, loaiGiaoVien);

        if (roleError is not null)
        {
            return OperationResult<AssignmentDetailDto>.BadRequest("Vai trò không phù hợp với hồ sơ.", [roleError]);
        }

        // Một người KHÔNG thể vừa là giáo viên chính vừa là trợ giảng của cùng một lớp.
        if (await _assignmentStore.HasSamePersonOverlapAsync(
                maLopHoc, maGiaoVien, ngayBatDau, ngayKetThuc, exceptId, cancellationToken))
        {
            return OperationResult<AssignmentDetailDto>.Conflict(
                $"'{maGiaoVien}' đã được phân công cho lớp này ở vai trò khác trong khoảng thời gian chồng lấn.");
        }

        // Không để một người dạy 2 lớp trùng giờ.
        var conflicts = await _assignmentStore.FindScheduleConflictsAsync(
            maLopHoc, maGiaoVien, ngayBatDau, ngayKetThuc, exceptId, cancellationToken);

        if (conflicts.Count > 0)
        {
            var chiTiet = string.Join("; ", conflicts.Take(5));

            return OperationResult<AssignmentDetailDto>.Conflict(
                $"'{maGiaoVien}' bị trùng lịch dạy với {chiTiet}.");
        }

        return null;
    }

    /// <summary>Vai trò "Trợ giảng" cần hồ sơ Trợ giảng; các vai trò còn lại cần hồ sơ Giáo viên.</summary>
    private static string? CheckTeacherTypeMatch(string vaiTro, string maGiaoVien, string loaiGiaoVien)
    {
        var needsTeacher = AssignmentRoles.NeedsTeacherProfile(vaiTro);

        if (needsTeacher && loaiGiaoVien != TeacherTypes.GiaoVien)
        {
            return $"Vai trò '{vaiTro}' yêu cầu hồ sơ loại '{TeacherTypes.GiaoVien}', " +
                   $"nhưng '{maGiaoVien}' đang là '{loaiGiaoVien}'.";
        }

        if (!needsTeacher && loaiGiaoVien != TeacherTypes.TroGiang)
        {
            return $"Vai trò '{vaiTro}' yêu cầu hồ sơ loại '{TeacherTypes.TroGiang}', " +
                   $"nhưng '{maGiaoVien}' đang là '{loaiGiaoVien}'.";
        }

        return null;
    }

    private static bool IsEmptyUpdate(UpdateAssignmentRequest request)
        => request.MaGiaoVien is null
           && request.VaiTroPhanCong is null
           && request.NgayBatDau is null
           && request.NgayKetThuc is null
           && request.GhiChu is null;
}
