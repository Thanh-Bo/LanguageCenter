using LanguageCenter.Application.Classes.Models;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Abstractions;

/// <summary>Cổng truy cập dữ liệu lớp học (<c>LopHoc</c>).</summary>
public interface IClassStore
{
    Task<PagedResult<ClassListItemDto>> ListAsync(ClassListQuery query, CancellationToken cancellationToken);

    Task<ClassDetailDto?> GetDetailAsync(int maLopHoc, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int maLopHoc, CancellationToken cancellationToken);

    /// <summary>Trùng mã lớp (UNIQUE trong database).</summary>
    Task<bool> CodeExistsAsync(string maLop, int? exceptId, CancellationToken cancellationToken);

    /// <summary>
    /// Sinh mã lớp kế tiếp cho khóa học: <c>{Ký hiệu cấp độ}-{STT:00}</c>.
    /// Trả về <c>null</c> nếu khóa học/cấp độ không có ký hiệu.
    /// </summary>
    Task<string?> GenerateNextCodeAsync(int maKhoaHoc, CancellationToken cancellationToken);

    Task<WriteResult> CreateAsync(CreateClassRequest request, CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateAsync(int maLopHoc, UpdateClassRequest changes, CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateStatusAsync(int maLopHoc, string trangThai, CancellationToken cancellationToken);

    /// <summary>Kiểm tra dữ liệu con trước khi xóa cứng.</summary>
    Task<ClassDeleteCheck> CheckDeletableAsync(int maLopHoc, CancellationToken cancellationToken);

    Task<WriteOutcome> DeleteAsync(int maLopHoc, CancellationToken cancellationToken);

    /// <summary>Sĩ số tối đa + trạng thái + khóa học — dùng khi xếp lớp.</summary>
    Task<ClassCapacityInfo?> GetCapacityInfoAsync(int maLopHoc, CancellationToken cancellationToken);
}

/// <summary>Kết quả kiểm tra điều kiện xóa lớp.</summary>
public sealed record ClassDeleteCheck(bool CanDelete, string? Reason)
{
    public static ClassDeleteCheck Allowed() => new(true, null);

    public static ClassDeleteCheck Blocked(string reason) => new(false, reason);
}

/// <summary>Thông tin lớp cần cho nghiệp vụ xếp lớp.</summary>
public sealed record ClassCapacityInfo(
    int MaLopHoc,
    string MaLop,
    int MaKhoaHoc,
    int SiSoToiDa,
    string TrangThai);
