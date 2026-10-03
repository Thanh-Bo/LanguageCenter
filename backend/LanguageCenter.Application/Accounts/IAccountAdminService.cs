using LanguageCenter.Application.Accounts.Models;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Accounts;

/// <summary>Quản trị tài khoản (chỉ Quản trị viên). Controller chỉ gọi interface này.</summary>
public interface IAccountAdminService
{
    Task<OperationResult<PagedResult<AccountListItemDto>>> ListAsync(AccountListQuery query, CancellationToken cancellationToken);

    Task<OperationResult<AccountDetailDto>> GetDetailAsync(int maTaiKhoan, CancellationToken cancellationToken);

    Task<OperationResult<AccountDetailDto>> UpdateStatusAsync(
        int maTaiKhoan,
        UpdateAccountStatusRequest request,
        int currentUserMaTaiKhoan,
        CancellationToken cancellationToken);

    /// <summary>Sinh mật khẩu tạm mới, hash rồi lưu. Trả plain text ĐÚNG MỘT LẦN.</summary>
    Task<OperationResult<ResetPasswordResponse>> ResetPasswordAsync(int maTaiKhoan, CancellationToken cancellationToken);

    /// <summary>
    /// Xóa cứng tài khoản — CHỈ khi không có dữ liệu nghiệp vụ phụ thuộc.
    /// Nếu có dữ liệu liên quan trả <see cref="OperationStatus.Conflict"/> và hướng dẫn dùng trạng thái "Ngừng sử dụng".
    /// </summary>
    Task<OperationResult<bool>> DeleteAsync(int maTaiKhoan, CancellationToken cancellationToken);
}
