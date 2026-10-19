using LanguageCenter.Application.Common;
using LanguageCenter.Application.Schedules.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>Cổng truy cập lịch học định kỳ (<c>LichHoc</c>).</summary>
public interface IScheduleStore
{
    Task<PagedResult<ScheduleListItemDto>> ListAsync(
        ScheduleListQuery query,
        CancellationToken cancellationToken);

    Task<ScheduleDetailDto?> GetDetailAsync(int maLichHoc, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int maLichHoc, CancellationToken cancellationToken);

    /// <summary>
    /// Xung đột PHÒNG: cùng phòng + cùng thứ + giờ chồng lấn + khoảng ngày chồng lấn
    /// (bỏ qua lịch đã ngừng áp dụng).
    /// </summary>
    Task<bool> HasRoomConflictAsync(
        int maPhongHoc,
        int thuTrongTuan,
        TimeOnly gioBatDau,
        TimeOnly gioKetThuc,
        DateOnly ngayApDungTu,
        DateOnly? ngayApDungDen,
        int? exceptId,
        CancellationToken cancellationToken);

    Task<WriteResult> CreateAsync(CreateScheduleRequest request, CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateAsync(
        int maLichHoc,
        UpdateScheduleRequest changes,
        CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateStatusAsync(
        int maLichHoc,
        string trangThai,
        CancellationToken cancellationToken);

    /// <summary>Đã có ca học thực tế tham chiếu lịch này chưa.</summary>
    Task<int> CountSessionsAsync(int maLichHoc, CancellationToken cancellationToken);

    Task<WriteOutcome> DeleteAsync(int maLichHoc, CancellationToken cancellationToken);
}
