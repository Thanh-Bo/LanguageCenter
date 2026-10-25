using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Rooms.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Rooms;

/// <inheritdoc cref="IRoomService"/>
public sealed class RoomService : IRoomService
{
    private readonly IRoomStore _roomStore;

    public RoomService(IRoomStore roomStore)
    {
        _roomStore = roomStore;
    }

    public async Task<OperationResult<PagedResult<RoomListItemDto>>> ListAsync(
        RoomListQuery query,
        CancellationToken cancellationToken)
    {
        var page = await _roomStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<RoomListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<RoomListItemDto>> GetDetailAsync(
        int maPhongHoc,
        CancellationToken cancellationToken)
    {
        var room = await _roomStore.GetDetailAsync(maPhongHoc, cancellationToken);

        return room is null
            ? OperationResult<RoomListItemDto>.NotFound($"Không tìm thấy phòng có id = {maPhongHoc}.")
            : OperationResult<RoomListItemDto>.Ok(room);
    }

    public async Task<OperationResult<RoomListItemDto>> CreateAsync(
        CreateRoomRequest request,
        CancellationToken cancellationToken)
    {
        // Mã phòng là mã nghiệp vụ, luôn lưu dạng chữ HOA để không lệch P101 / p101.
        // Để trống → hệ thống TỰ SINH (P + số lớn nhất + 1).
        var maPhong = TextValue.Required(request.MaPhong)?.ToUpperInvariant();

        if (maPhong is null)
        {
            maPhong = await _roomStore.GenerateNextCodeAsync(cancellationToken);
        }

        if (maPhong.Length > 30)
        {
            return OperationResult<RoomListItemDto>.BadRequest("Mã phòng tối đa 30 ký tự.");
        }

        var tenPhong = TextValue.Required(request.TenPhong);

        if (tenPhong is null)
        {
            return OperationResult<RoomListItemDto>.BadRequest("Tên phòng là bắt buộc.");
        }

        if (request.SucChua <= 0)
        {
            return OperationResult<RoomListItemDto>.BadRequest(
                "Sức chứa phải lớn hơn 0.",
                ["Ví dụ: 25."]);
        }

        var trangThai = TextValue.Required(request.TrangThai) ?? RoomStatuses.HoatDong;

        if (!RoomStatuses.IsValid(trangThai))
        {
            return OperationResult<RoomListItemDto>.BadRequest(
                "Trạng thái phòng không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", RoomStatuses.All)}."]);
        }

        if (await _roomStore.CodeExistsAsync(maPhong, cancellationToken))
        {
            return OperationResult<RoomListItemDto>.Conflict($"Mã phòng '{maPhong}' đã tồn tại.");
        }

        var maPhongHoc = await _roomStore.CreateAsync(
            new RoomCreateData(
                maPhong,
                tenPhong,
                TextValue.Optional(request.ViTri),
                request.SucChua,
                TextValue.Optional(request.ThietBi),
                trangThai),
            cancellationToken);

        if (maPhongHoc is null)
        {
            return OperationResult<RoomListItemDto>.Conflict($"Mã phòng '{maPhong}' đã tồn tại.");
        }

        var created = await _roomStore.GetDetailAsync(maPhongHoc.Value, cancellationToken);

        return OperationResult<RoomListItemDto>.Created(created!, "Tạo phòng học thành công.");
    }
}
