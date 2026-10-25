using LanguageCenter.Application.Common;
using LanguageCenter.Application.Rooms.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>Cổng truy cập danh mục phòng học (<c>PhongHoc</c>).</summary>
public interface IRoomStore
{
    Task<PagedResult<RoomListItemDto>> ListAsync(RoomListQuery query, CancellationToken cancellationToken);

    Task<RoomListItemDto?> GetDetailAsync(int maPhongHoc, CancellationToken cancellationToken);

    /// <summary>Mã phòng đã tồn tại chưa (UNIQUE).</summary>
    Task<bool> CodeExistsAsync(string maPhong, CancellationToken cancellationToken);

    /// <summary>
    /// Sinh mã phòng kế tiếp: <c>P</c> + số lớn nhất đang có + 1 (ví dụ P101, P202, P303 → P304).
    /// Chưa có phòng nào → <c>P101</c>.
    /// </summary>
    Task<string> GenerateNextCodeAsync(CancellationToken cancellationToken);

    /// <summary>Tạo phòng. Trả về MaPhongHoc mới, hoặc null nếu trùng mã (race condition).</summary>
    Task<int?> CreateAsync(RoomCreateData data, CancellationToken cancellationToken);
}

/// <summary>Dữ liệu tạo phòng học (đã qua kiểm tra ở tầng Application).</summary>
public sealed record RoomCreateData(
    string MaPhong,
    string TenPhong,
    string? ViTri,
    int SucChua,
    string? ThietBi,
    string TrangThai);
