using LanguageCenter.Application.Common;
using LanguageCenter.Application.Notifications.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>
/// Phạm vi mà một người dùng được phép thấy thông báo:
/// thông báo gửi trực tiếp cho tài khoản, và (nếu có) thông báo gửi cho các lớp của họ.
/// </summary>
public sealed record NotificationAudience(
    int MaTaiKhoan,
    IReadOnlyList<int> ClassIds,
    bool IncludeClassBroadcasts);

/// <summary>Dữ liệu tạo thông báo đã được service kiểm tra quyền và tồn tại.</summary>
public sealed record CreateNotificationData(
    int MaTaiKhoanGui,
    int? MaTaiKhoanNhan,
    int? MaLopHoc,
    string TieuDe,
    string NoiDung,
    string? LoaiThongBao);

/// <summary>Dữ liệu cập nhật thông báo (KHÔNG cho đổi người gửi/người nhận).</summary>
public sealed record NotificationUpdateData(
    bool CapNhatTieuDe,
    string? TieuDe,
    bool CapNhatNoiDung,
    string? NoiDung,
    bool CapNhatLoai,
    string? LoaiThongBao,
    string? TrangThai);

/// <summary>Cổng truy cập thông báo (<c>ThongBao</c>).</summary>
public interface INotificationStore
{
    Task<PagedResult<NotificationDto>> ListForAudienceAsync(
        NotificationAudience audience,
        NotificationListQuery query,
        CancellationToken cancellationToken);

    Task<int> CountForAudienceAsync(
        NotificationAudience audience,
        CancellationToken cancellationToken);

    Task<NotificationDto?> GetDetailForAudienceAsync(
        int maThongBao,
        NotificationAudience audience,
        CancellationToken cancellationToken);

    Task<PagedResult<NotificationDto>> ListAsync(
        AdminNotificationListQuery query,
        CancellationToken cancellationToken);

    Task<NotificationDto?> GetDetailAsync(int maThongBao, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int maThongBao, CancellationToken cancellationToken);

    Task<WriteResult> CreateAsync(
        CreateNotificationData data,
        CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateAsync(
        int maThongBao,
        NotificationUpdateData changes,
        CancellationToken cancellationToken);

    Task<WriteOutcome> DeleteAsync(int maThongBao, CancellationToken cancellationToken);
}
