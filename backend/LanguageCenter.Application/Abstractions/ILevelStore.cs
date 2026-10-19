using LanguageCenter.Application.Common;
using LanguageCenter.Application.Levels.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>Cổng truy cập dữ liệu cấp độ (<c>CapDo</c>).</summary>
public interface ILevelStore
{
    Task<PagedResult<LevelListItemDto>> ListAsync(LevelListQuery query, CancellationToken cancellationToken);

    Task<LevelDetailDto?> GetDetailAsync(int maCapDo, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int maCapDo, CancellationToken cancellationToken);

    /// <summary>Trùng tên cấp độ TRONG CÙNG chương trình (UQ_CapDo_ChuongTrinh_Ten).</summary>
    Task<bool> NameExistsAsync(
        int maChuongTrinh,
        string tenCapDo,
        int? exceptId,
        CancellationToken cancellationToken);

    /// <summary>Trùng thứ tự TRONG CÙNG chương trình (UQ_CapDo_ChuongTrinh_ThuTu).</summary>
    Task<bool> OrderExistsAsync(
        int maChuongTrinh,
        int thuTu,
        int? exceptId,
        CancellationToken cancellationToken);

    Task<WriteResult> CreateAsync(CreateLevelRequest request, CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateAsync(int maCapDo, UpdateLevelRequest changes, CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateStatusAsync(int maCapDo, string trangThai, CancellationToken cancellationToken);
}
