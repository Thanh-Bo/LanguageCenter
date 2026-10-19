using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Notifications.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Notifications;

/// <inheritdoc cref="INotificationService"/>
public sealed class NotificationService : INotificationService
{
    private readonly INotificationStore _notificationStore;
    private readonly IAcademicLookupStore _lookupStore;

    public NotificationService(
        INotificationStore notificationStore,
        IAcademicLookupStore lookupStore)
    {
        _notificationStore = notificationStore;
        _lookupStore = lookupStore;
    }

    // ========================================================================
    // NGƯỜI DÙNG
    // ========================================================================

    public async Task<OperationResult<PagedResult<NotificationDto>>> ListForCurrentUserAsync(
        NotificationActor actor,
        NotificationListQuery query,
        CancellationToken cancellationToken)
    {
        var invalid = ValidateFilter(query.TuNgay, query.DenNgay);

        if (invalid is not null)
        {
            return OperationResult<PagedResult<NotificationDto>>.BadRequest(
                "Khoảng ngày không hợp lệ.",
                [invalid]);
        }

        var audience = await BuildAudienceAsync(actor, cancellationToken);
        var page = await _notificationStore.ListForAudienceAsync(audience, query, cancellationToken);

        return OperationResult<PagedResult<NotificationDto>>.Ok(page);
    }

    public async Task<OperationResult<NotificationDto>> GetDetailForCurrentUserAsync(
        int maThongBao,
        NotificationActor actor,
        CancellationToken cancellationToken)
    {
        var audience = await BuildAudienceAsync(actor, cancellationToken);
        var detail = await _notificationStore.GetDetailForAudienceAsync(
            maThongBao, audience, cancellationToken);

        // 404 (không phải 403) để không lộ sự tồn tại của thông báo người khác.
        return detail is null
            ? OperationResult<NotificationDto>.NotFound($"Không tìm thấy thông báo có id = {maThongBao}.")
            : OperationResult<NotificationDto>.Ok(detail);
    }

    public async Task<OperationResult<NotificationSummaryDto>> GetSummaryAsync(
        NotificationActor actor,
        CancellationToken cancellationToken)
    {
        var audience = await BuildAudienceAsync(actor, cancellationToken);
        var total = await _notificationStore.CountForAudienceAsync(audience, cancellationToken);

        return OperationResult<NotificationSummaryDto>.Ok(new NotificationSummaryDto { TongSo = total });
    }

    // ========================================================================
    // QUẢN TRỊ — ĐỌC
    // ========================================================================

    public async Task<OperationResult<PagedResult<NotificationDto>>> ListAsync(
        AdminNotificationListQuery query,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(query.TrangThai)
            && !NotificationStatuses.IsValid(query.TrangThai))
        {
            return OperationResult<PagedResult<NotificationDto>>.BadRequest(
                "Trạng thái thông báo không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", NotificationStatuses.All)}."]);
        }

        var invalid = ValidateFilter(query.TuNgay, query.DenNgay);

        if (invalid is not null)
        {
            return OperationResult<PagedResult<NotificationDto>>.BadRequest(
                "Khoảng ngày không hợp lệ.",
                [invalid]);
        }

        var page = await _notificationStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<NotificationDto>>.Ok(page);
    }

    public async Task<OperationResult<NotificationDto>> GetDetailAsync(
        int maThongBao,
        CancellationToken cancellationToken)
    {
        var detail = await _notificationStore.GetDetailAsync(maThongBao, cancellationToken);

        return detail is null
            ? OperationResult<NotificationDto>.NotFound($"Không tìm thấy thông báo có id = {maThongBao}.")
            : OperationResult<NotificationDto>.Ok(detail);
    }

    // ========================================================================
    // QUẢN TRỊ — GHI
    // ========================================================================

    public async Task<OperationResult<NotificationDto>> CreateAsync(
        CreateNotificationRequest request,
        NotificationActor actor,
        CancellationToken cancellationToken)
    {
        if (actor.MaTaiKhoan <= 0)
        {
            return OperationResult<NotificationDto>.Forbidden(
                "Không xác định được người gửi từ token.");
        }

        var tieuDe = TextValue.Required(request.TieuDe);
        var noiDung = TextValue.Required(request.NoiDung);
        var loai = TextValue.Optional(request.LoaiThongBao);

        var errors = new List<string>();

        if (tieuDe is null)
        {
            errors.Add("Tiêu đề là bắt buộc.");
        }
        else if (tieuDe.Length is < 3 or > 255)
        {
            errors.Add("Tiêu đề phải từ 3 đến 255 ký tự.");
        }

        if (noiDung is null)
        {
            errors.Add("Nội dung là bắt buộc.");
        }
        else if (noiDung.Length > 8000)
        {
            errors.Add("Nội dung tối đa 8000 ký tự.");
        }

        if (loai is not null && loai.Length > 100)
        {
            errors.Add("Loại thông báo tối đa 100 ký tự.");
        }

        if (request.MaTaiKhoanNhan is null && request.MaLopHoc is null)
        {
            errors.Add("Phải chọn ít nhất một đích: tài khoản nhận hoặc lớp học.");
        }

        if (errors.Count > 0)
        {
            return OperationResult<NotificationDto>.BadRequest("Dữ liệu không hợp lệ.", errors);
        }

        var laGiaoVien = actor.VaiTro is RoleNames.GiaoVien or RoleNames.TroGiang;

        // Giáo viên / trợ giảng chỉ được thông báo cho lớp mình phụ trách.
        if (laGiaoVien)
        {
            if (request.MaLopHoc is null)
            {
                return OperationResult<NotificationDto>.Forbidden(
                    "Giáo viên/Trợ giảng chỉ được gửi thông báo cho lớp mình phụ trách.");
            }

            if (request.MaTaiKhoanNhan is not null)
            {
                return OperationResult<NotificationDto>.Forbidden(
                    "Giáo viên/Trợ giảng không được gửi thông báo trực tiếp tới một tài khoản cụ thể.");
            }

            if (!await _lookupStore.TeacherAssignedToClassAsync(
                    request.MaLopHoc.Value, actor.TenDangNhap, cancellationToken))
            {
                return OperationResult<NotificationDto>.Forbidden(
                    "Bạn không được phân công giảng dạy lớp này.");
            }
        }

        if (request.MaTaiKhoanNhan is { } maNhan
            && !await _lookupStore.TaiKhoanExistsAsync(maNhan, cancellationToken))
        {
            return OperationResult<NotificationDto>.BadRequest(
                "Tài khoản nhận không tồn tại.",
                [$"Không tìm thấy tài khoản có id = {maNhan}."]);
        }

        if (request.MaLopHoc is { } maLop
            && !await _lookupStore.ClassExistsAsync(maLop, cancellationToken))
        {
            return OperationResult<NotificationDto>.BadRequest(
                "Lớp học không tồn tại.",
                [$"Không tìm thấy lớp có id = {maLop}."]);
        }

        var data = new CreateNotificationData(
            actor.MaTaiKhoan,
            request.MaTaiKhoanNhan,
            request.MaLopHoc,
            tieuDe!,
            noiDung!,
            loai);

        var result = await _notificationStore.CreateAsync(data, cancellationToken);

        if (!result.Succeeded)
        {
            return OperationResult<NotificationDto>.Conflict("Không thể gửi thông báo.");
        }

        var created = await _notificationStore.GetDetailAsync(result.Id, cancellationToken);

        return OperationResult<NotificationDto>.Created(created!, "Gửi thông báo thành công.");
    }

    public async Task<OperationResult<NotificationDto>> UpdateAsync(
        int maThongBao,
        UpdateNotificationRequest request,
        CancellationToken cancellationToken)
    {
        if (request.TieuDe is null
            && request.NoiDung is null
            && request.LoaiThongBao is null
            && request.TrangThai is null)
        {
            return OperationResult<NotificationDto>.BadRequest(
                "Không có trường nào được cập nhật.",
                ["Hãy gửi ít nhất một trường cần thay đổi."]);
        }

        var trangThai = TextValue.Required(request.TrangThai);

        if (trangThai is not null && !NotificationStatuses.IsValid(trangThai))
        {
            return OperationResult<NotificationDto>.BadRequest(
                "Trạng thái thông báo không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", NotificationStatuses.All)}."]);
        }

        var tieuDe = request.TieuDe is null ? null : TextValue.Required(request.TieuDe);

        if (request.TieuDe is not null && (tieuDe is null || tieuDe.Length is < 3 or > 255))
        {
            return OperationResult<NotificationDto>.BadRequest(
                "Tiêu đề không hợp lệ.",
                ["Tiêu đề phải từ 3 đến 255 ký tự."]);
        }

        var noiDung = request.NoiDung is null ? null : TextValue.Required(request.NoiDung);

        if (request.NoiDung is not null && (noiDung is null || noiDung.Length > 8000))
        {
            return OperationResult<NotificationDto>.BadRequest(
                "Nội dung không hợp lệ.",
                ["Nội dung bắt buộc và tối đa 8000 ký tự."]);
        }

        var changes = new NotificationUpdateData(
            CapNhatTieuDe: request.TieuDe is not null,
            TieuDe: tieuDe,
            CapNhatNoiDung: request.NoiDung is not null,
            NoiDung: noiDung,
            CapNhatLoai: request.LoaiThongBao is not null,
            LoaiThongBao: TextValue.Optional(request.LoaiThongBao),
            TrangThai: trangThai);

        var outcome = await _notificationStore.UpdateAsync(maThongBao, changes, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return outcome == WriteOutcome.NotFound
                ? OperationResult<NotificationDto>.NotFound($"Không tìm thấy thông báo có id = {maThongBao}.")
                : OperationResult<NotificationDto>.Conflict("Không thể cập nhật thông báo.");
        }

        var updated = await _notificationStore.GetDetailAsync(maThongBao, cancellationToken);

        return OperationResult<NotificationDto>.Ok(updated!);
    }

    public async Task<OperationResult<bool>> DeleteAsync(
        int maThongBao,
        CancellationToken cancellationToken)
    {
        var outcome = await _notificationStore.DeleteAsync(maThongBao, cancellationToken);

        return outcome switch
        {
            WriteOutcome.Success => OperationResult<bool>.NoContent(),
            WriteOutcome.NotFound => OperationResult<bool>.NotFound(
                $"Không tìm thấy thông báo có id = {maThongBao}."),
            _ => OperationResult<bool>.Conflict("Không thể xóa thông báo do ràng buộc dữ liệu.")
        };
    }

    // ========================================================================
    // PRIVATE HELPERS
    // ========================================================================

    /// <summary>
    /// Xác định phạm vi thông báo người dùng được thấy.
    /// <para>
    /// Học viên: thông báo trực tiếp + thông báo của các lớp đang theo học.<br/>
    /// Giáo viên/Trợ giảng: thông báo trực tiếp + thông báo của lớp được phân công.<br/>
    /// Vai trò khác: CHỈ thông báo trực tiếp (dùng API quản trị để xem toàn bộ).
    /// </para>
    /// </summary>
    private async Task<NotificationAudience> BuildAudienceAsync(
        NotificationActor actor,
        CancellationToken cancellationToken)
    {
        if (actor.MaTaiKhoan <= 0)
        {
            return new NotificationAudience(0, [], false);
        }

        if (string.Equals(actor.VaiTro, RoleNames.HocVien, StringComparison.Ordinal))
        {
            var classIds = await _lookupStore.GetStudentClassIdsAsync(
                actor.TenDangNhap, cancellationToken);

            return new NotificationAudience(actor.MaTaiKhoan, classIds, true);
        }

        if (actor.VaiTro is RoleNames.GiaoVien or RoleNames.TroGiang)
        {
            var classIds = await _lookupStore.GetTeacherClassIdsAsync(
                actor.TenDangNhap, cancellationToken);

            return new NotificationAudience(actor.MaTaiKhoan, classIds, true);
        }

        return new NotificationAudience(actor.MaTaiKhoan, [], false);
    }

    private static string? ValidateFilter(DateOnly? tuNgay, DateOnly? denNgay)
        => tuNgay is { } tu && denNgay is { } den && den < tu
            ? "Đến ngày phải lớn hơn hoặc bằng từ ngày."
            : null;
}
