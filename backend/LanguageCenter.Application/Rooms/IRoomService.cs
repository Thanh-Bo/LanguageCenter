using LanguageCenter.Application.Common;
using LanguageCenter.Application.Rooms.Models;

namespace LanguageCenter.Application.Rooms;

/// <summary>Danh mục phòng học — đọc + tạo mới.</summary>
public interface IRoomService
{
    Task<OperationResult<PagedResult<RoomListItemDto>>> ListAsync(
        RoomListQuery query,
        CancellationToken cancellationToken);

    Task<OperationResult<RoomListItemDto>> GetDetailAsync(
        int maPhongHoc,
        CancellationToken cancellationToken);

    Task<OperationResult<RoomListItemDto>> CreateAsync(
        CreateRoomRequest request,
        CancellationToken cancellationToken);
}
