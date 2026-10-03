using LanguageCenter.Application.Accounts.Models;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Abstractions;

/// <summary>
/// Cổng truy cập dữ liệu tài khoản.
/// Đây KHÔNG phải GenericRepository / UnitOfWork: chỉ các method hẹp, đúng nhu cầu Auth + quản trị tài khoản.
/// Triển khai thật ở Infrastructure (<c>AccountStore</c>) bằng EF Core.
/// </summary>
public interface IAccountStore
{
    // ---------- Phase 2: authentication ----------
    /// <summary>Tìm tài khoản theo tên đăng nhập, đã join sẵn vai trò.</summary>
    Task<AccountAuthInfo?> FindByUsernameAsync(string tenDangNhap, CancellationToken cancellationToken);

    /// <summary>Tìm tài khoản theo khóa chính.</summary>
    Task<AccountAuthInfo?> FindByIdAsync(int maTaiKhoan, CancellationToken cancellationToken);

    /// <summary>
    /// Cập nhật <c>LanDangNhapCuoi</c> theo giờ CỦA SQL SERVER (<c>SYSDATETIME()</c>)
    /// để nhất quán với mọi default timestamp khác trong database.
    /// </summary>
    Task TouchLastLoginAsync(int maTaiKhoan, CancellationToken cancellationToken);

    /// <summary>Cập nhật <c>MatKhauHash</c> và <c>NgayCapNhat</c>.</summary>
    Task UpdatePasswordHashAsync(int maTaiKhoan, string matKhauHash, CancellationToken cancellationToken);

    // ---------- Phase 3: quản trị tài khoản ----------

    /// <summary>Danh sách tài khoản có phân trang / tìm kiếm / lọc — thực hiện ở tầng SQL.</summary>
    Task<PagedResult<AccountListItemDto>> ListAsync(AccountListQuery query, CancellationToken cancellationToken);

    /// <summary>Chi tiết tài khoản (kèm thông tin hồ sơ tương ứng).</summary>
    Task<AccountDetailDto?> GetDetailAsync(int maTaiKhoan, CancellationToken cancellationToken);

    /// <summary>Đổi <c>TrangThaiTaiKhoan</c>.</summary>
    Task<WriteOutcome> UpdateStatusAsync(int maTaiKhoan, string trangThai, CancellationToken cancellationToken);

    /// <summary>Email đã được dùng bởi tài khoản khác chưa (<paramref name="exceptUsername"/> để bỏ qua chính nó).</summary>
    Task<bool> EmailExistsAsync(string email, string? exceptUsername, CancellationToken cancellationToken);

    /// <summary>Kiểm tra tài khoản có dữ liệu nghiệp vụ phụ thuộc hay không trước khi xóa cứng.</summary>
    Task<AccountDeleteCheck> CheckDeletableAsync(int maTaiKhoan, CancellationToken cancellationToken);

    /// <summary>Xóa cứng tài khoản + hồ sơ tương ứng trong MỘT transaction.</summary>
    Task<WriteOutcome> DeleteAsync(int maTaiKhoan, CancellationToken cancellationToken);
}

/// <summary>Kết quả kiểm tra điều kiện xóa cứng tài khoản.</summary>
/// <param name="CanDelete">True nếu không có dữ liệu nghiệp vụ nào phụ thuộc.</param>
/// <param name="Reason">Lý do chặn (bảng dữ liệu đang tham chiếu).</param>
public sealed record AccountDeleteCheck(bool CanDelete, string? Reason)
{
    public static AccountDeleteCheck Allowed() => new(true, null);

    public static AccountDeleteCheck Blocked(string reason) => new(false, reason);
}
