using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Rooms;
using LanguageCenter.Application.Rooms.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Danh mục phòng học — đọc + tạo mới. Dùng để chọn phòng khi tạo lịch học / ca học.
/// </summary>
[ApiController]
[Route("api/admin/rooms")]
[Authorize(Roles = RoleNames.QuanTriVien + "," + RoleNames.NhanVienHocVu)]
public sealed class AdminRoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public AdminRoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    /// <summary>Danh sách phòng học — tìm theo mã/tên, lọc theo trạng thái.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<RoomListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] RoomListQuery query,
        CancellationToken cancellationToken)
        => (await _roomService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết phòng học.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(RoomListItemDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _roomService.GetDetailAsync(id, cancellationToken)).ToActionResult();

    /// <summary>Tạo phòng học mới — mã phòng (UNIQUE) tự chuyển chữ HOA.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(RoomListItemDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateRoomRequest request,
        CancellationToken cancellationToken)
        => (await _roomService.CreateAsync(request, cancellationToken)).ToActionResult();
}
