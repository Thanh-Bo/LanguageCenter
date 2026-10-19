using LanguageCenter.Application.Common;
using LanguageCenter.Application.Programs.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>
/// Cổng truy cập dữ liệu chương trình đào tạo (<c>ChuongTrinh</c>).
/// Triển khai thật ở Infrastructure (<c>ProgramStore</c>) bằng EF Core.
/// </summary>
public interface IProgramStore
{
    /// <summary>Danh sách có phân trang / tìm theo tên / lọc trạng thái — thực hiện ở SQL Server.</summary>
    Task<PagedResult<ProgramListItemDto>> ListAsync(
        ProgramListQuery query,
        CancellationToken cancellationToken);

    /// <summary>Chi tiết chương trình (kèm số cấp độ và số khóa học).</summary>
    Task<ProgramDetailDto?> GetDetailAsync(int maChuongTrinh, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int maChuongTrinh, CancellationToken cancellationToken);

    /// <summary>Tên chương trình đã tồn tại chưa (<paramref name="exceptId"/> để bỏ qua chính nó khi sửa).</summary>
    Task<bool> NameExistsAsync(
        string tenChuongTrinh,
        int? exceptId,
        CancellationToken cancellationToken);

    Task<WriteResult> CreateAsync(CreateProgramRequest request, CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateAsync(
        int maChuongTrinh,
        UpdateProgramRequest changes,
        CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateStatusAsync(
        int maChuongTrinh,
        string trangThai,
        CancellationToken cancellationToken);
}
