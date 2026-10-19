using LanguageCenter.Application.Common;
using LanguageCenter.Application.Schedules.Models;

namespace LanguageCenter.Application.Schedules;

/// <summary>Quản lý lịch học định kỳ.</summary>
public interface IScheduleService
{
    Task<OperationResult<PagedResult<ScheduleListItemDto>>> ListAsync(
        ScheduleListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<ScheduleDetailDto>> GetDetailAsync(
        int maLichHoc,
        CancellationToken cancellationToken);

    Task<OperationResult<ScheduleDetailDto>> CreateAsync(
        CreateScheduleRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<ScheduleDetailDto>> UpdateAsync(
        int maLichHoc,
        UpdateScheduleRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<ScheduleDetailDto>> UpdateStatusAsync(
        int maLichHoc,
        UpdateStatusRequest request,
        CancellationToken cancellationToken);

    /// <summary>Xóa lịch — CHỈ khi chưa có ca học thực tế tham chiếu.</summary>
    Task<OperationResult<bool>> DeleteAsync(int maLichHoc, CancellationToken cancellationToken);
}
