using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Accounts.Models;
using LanguageCenter.Application.Common;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Accounts;

/// <inheritdoc cref="IAccountAdminService"/>
public sealed class AccountAdminService : IAccountAdminService
{
    private readonly IAccountStore _accountStore;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly ITemporaryPasswordGenerator _temporaryPasswordGenerator;

    public AccountAdminService(
        IAccountStore accountStore,
        IPasswordHasherService passwordHasherService,
        ITemporaryPasswordGenerator temporaryPasswordGenerator)
    {
        _accountStore = accountStore;
        _passwordHasherService = passwordHasherService;
        _temporaryPasswordGenerator = temporaryPasswordGenerator;
    }

    public async Task<OperationResult<PagedResult<AccountListItemDto>>> ListAsync(
        AccountListQuery query,
        CancellationToken cancellationToken)
    {
        var page = await _accountStore.ListAsync(query, cancellationToken);

        return OperationResult<PagedResult<AccountListItemDto>>.Ok(page);
    }

    public async Task<OperationResult<AccountDetailDto>> GetDetailAsync(
        int maTaiKhoan,
        CancellationToken cancellationToken)
    {
        var detail = await _accountStore.GetDetailAsync(maTaiKhoan, cancellationToken);

        return detail is null
            ? OperationResult<AccountDetailDto>.NotFound($"Không tìm thấy tài khoản có id = {maTaiKhoan}.")
            : OperationResult<AccountDetailDto>.Ok(detail);
    }

    public async Task<OperationResult<AccountDetailDto>> UpdateStatusAsync(
        int maTaiKhoan,
        UpdateAccountStatusRequest request,
        int currentUserMaTaiKhoan,
        CancellationToken cancellationToken)
    {
        var normalizedStatus = TextValue.Required(request.Status);

        if (normalizedStatus is null || !AccountStatuses.IsValid(normalizedStatus))
        {
            return OperationResult<AccountDetailDto>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", AccountStatuses.All)}."]);
        }

        var account = await _accountStore.FindByIdAsync(maTaiKhoan, cancellationToken);

        if (account is null)
        {
            return OperationResult<AccountDetailDto>.NotFound($"Không tìm thấy tài khoản có id = {maTaiKhoan}.");
        }

        // Chốt an toàn: không cho Quản trị viên tự khóa / tự ngừng tài khoản đang dùng
        // để tránh tự khóa chính mình khỏi hệ thống.
        if (currentUserMaTaiKhoan == maTaiKhoan && normalizedStatus != AccountStatuses.HoatDong)
        {
            return OperationResult<AccountDetailDto>.BadRequest(
                "Không thể tự khóa hoặc ngừng sử dụng tài khoản đang đăng nhập.",
                ["Hãy nhờ một Quản trị viên khác thực hiện."]);
        }

        var outcome = await _accountStore.UpdateStatusAsync(maTaiKhoan, normalizedStatus, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return OperationResult<AccountDetailDto>.Conflict("Không thể cập nhật trạng thái tài khoản.");
        }

        var detail = await _accountStore.GetDetailAsync(maTaiKhoan, cancellationToken);

        return OperationResult<AccountDetailDto>.Ok(detail!);
    }

    public async Task<OperationResult<ResetPasswordResponse>> ResetPasswordAsync(
        int maTaiKhoan,
        CancellationToken cancellationToken)
    {
        var account = await _accountStore.FindByIdAsync(maTaiKhoan, cancellationToken);

        if (account is null)
        {
            return OperationResult<ResetPasswordResponse>.NotFound($"Không tìm thấy tài khoản có id = {maTaiKhoan}.");
        }

        // Mật khẩu tạm MỚI, random — KHÔNG dùng lại mật khẩu cũ, KHÔNG hard-code, KHÔNG log.
        var temporaryPassword = _temporaryPasswordGenerator.Generate();
        var passwordHash = _passwordHasherService.HashPassword(temporaryPassword);

        await _accountStore.UpdatePasswordHashAsync(maTaiKhoan, passwordHash, cancellationToken);

        return OperationResult<ResetPasswordResponse>.Ok(new ResetPasswordResponse
        {
            Username = account.TenDangNhap,
            TemporaryPassword = temporaryPassword,
            Message = "Đã đặt lại mật khẩu. Vui lòng gửi mật khẩu tạm này cho người dùng và yêu cầu đổi ngay sau khi đăng nhập."
        });
    }

    public async Task<OperationResult<bool>> DeleteAsync(int maTaiKhoan, CancellationToken cancellationToken)
    {
        var account = await _accountStore.FindByIdAsync(maTaiKhoan, cancellationToken);

        if (account is null)
        {
            return OperationResult<bool>.NotFound($"Không tìm thấy tài khoản có id = {maTaiKhoan}.");
        }

        var check = await _accountStore.CheckDeletableAsync(maTaiKhoan, cancellationToken);

        if (!check.CanDelete)
        {
            return OperationResult<bool>.Conflict(
                $"Không thể xóa tài khoản '{account.TenDangNhap}' vì đang có dữ liệu liên quan ({check.Reason}). " +
                "Hãy dùng chức năng đổi trạng thái sang 'Ngừng sử dụng' thay vì xóa.");
        }

        var outcome = await _accountStore.DeleteAsync(maTaiKhoan, cancellationToken);

        return outcome switch
        {
            WriteOutcome.Success => OperationResult<bool>.NoContent(),
            WriteOutcome.NotFound => OperationResult<bool>.NotFound($"Không tìm thấy tài khoản có id = {maTaiKhoan}."),
            _ => OperationResult<bool>.Conflict("Không thể xóa tài khoản do ràng buộc dữ liệu.")
        };
    }

}
