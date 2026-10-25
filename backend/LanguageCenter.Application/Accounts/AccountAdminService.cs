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

    public async Task<OperationResult<AccountDetailDto>> CreateAsync(
        CreateAccountRequest request,
        CancellationToken cancellationToken)
    {
        var tenDangNhap = TextValue.Required(request.TenDangNhap);
        if (tenDangNhap is null)
        {
            return OperationResult<AccountDetailDto>.BadRequest("Tên đăng nhập là bắt buộc.");
        }

        var email = TextValue.Required(request.Email);
        if (email is null)
        {
            return OperationResult<AccountDetailDto>.BadRequest("Email là bắt buộc.");
        }

        if (string.IsNullOrWhiteSpace(request.MatKhau) || request.MatKhau.Length < 6)
        {
            return OperationResult<AccountDetailDto>.BadRequest("Mật khẩu phải có ít nhất 6 ký tự.");
        }

        var vaiTro = TextValue.Required(request.VaiTro);
        if (vaiTro is null)
        {
            return OperationResult<AccountDetailDto>.BadRequest("Vai trò là bắt buộc.");
        }

        var trangThai = TextValue.Required(request.TrangThai) ?? AccountStatuses.HoatDong;
        if (!AccountStatuses.IsValid(trangThai))
        {
            return OperationResult<AccountDetailDto>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", AccountStatuses.All)}."]);
        }

        if (await _accountStore.UsernameExistsAsync(tenDangNhap, cancellationToken))
        {
            return OperationResult<AccountDetailDto>.Conflict($"Tên đăng nhập '{tenDangNhap}' đã tồn tại.");
        }

        if (await _accountStore.EmailExistsAsync(email, null, cancellationToken))
        {
            return OperationResult<AccountDetailDto>.Conflict($"Email '{email}' đã được sử dụng.");
        }

        var maVaiTro = await _accountStore.FindRoleIdAsync(vaiTro, cancellationToken);
        if (maVaiTro is null)
        {
            return OperationResult<AccountDetailDto>.BadRequest($"Vai trò '{vaiTro}' không tồn tại.");
        }

        var passwordHash = _passwordHasherService.HashPassword(request.MatKhau);

        var maTaiKhoan = await _accountStore.CreateAsync(
            tenDangNhap, email, passwordHash, maVaiTro.Value, trangThai, cancellationToken);

        if (maTaiKhoan is null)
        {
            return OperationResult<AccountDetailDto>.Conflict("Không tạo được tài khoản.");
        }

        var created = await _accountStore.GetDetailAsync(maTaiKhoan.Value, cancellationToken);

        return OperationResult<AccountDetailDto>.Ok(created!);
    }

    public async Task<OperationResult<AccountDetailDto>> UpdateAsync(
        int maTaiKhoan,
        UpdateAccountRequest request,
        int currentUserMaTaiKhoan,
        CancellationToken cancellationToken)
    {
        var account = await _accountStore.FindByIdAsync(maTaiKhoan, cancellationToken);

        if (account is null)
        {
            return OperationResult<AccountDetailDto>.NotFound($"Không tìm thấy tài khoản có id = {maTaiKhoan}.");
        }

        var email = TextValue.Required(request.Email);
        if (request.Email is not null && email is null)
        {
            return OperationResult<AccountDetailDto>.BadRequest("Email không được để trống.");
        }

        var trangThai = TextValue.Required(request.TrangThai);
        if (request.TrangThai is not null && (trangThai is null || !AccountStatuses.IsValid(trangThai)))
        {
            return OperationResult<AccountDetailDto>.BadRequest(
                "Trạng thái không hợp lệ.",
                [$"Chỉ chấp nhận: {string.Join(", ", AccountStatuses.All)}."]);
        }

        if (currentUserMaTaiKhoan == maTaiKhoan && trangThai is not null && trangThai != AccountStatuses.HoatDong)
        {
            return OperationResult<AccountDetailDto>.BadRequest(
                "Không thể tự khóa hoặc ngừng sử dụng tài khoản đang đăng nhập.",
                ["Hãy nhờ một Quản trị viên khác thực hiện."]);
        }

        int? maVaiTro = null;
        var vaiTro = TextValue.Required(request.VaiTro);
        if (vaiTro is not null)
        {
            maVaiTro = await _accountStore.FindRoleIdAsync(vaiTro, cancellationToken);
            if (maVaiTro is null)
            {
                return OperationResult<AccountDetailDto>.BadRequest($"Vai trò '{vaiTro}' không tồn tại.");
            }
        }

        if (email is not null && await _accountStore.EmailExistsAsync(email, account.TenDangNhap, cancellationToken))
        {
            return OperationResult<AccountDetailDto>.Conflict($"Email '{email}' đã được sử dụng.");
        }

        string? passwordHash = null;
        if (!string.IsNullOrWhiteSpace(request.MatKhau))
        {
            if (request.MatKhau.Length < 6)
            {
                return OperationResult<AccountDetailDto>.BadRequest("Mật khẩu phải có ít nhất 6 ký tự.");
            }

            passwordHash = _passwordHasherService.HashPassword(request.MatKhau);
        }

        var outcome = await _accountStore.UpdateAsync(
            maTaiKhoan, email, maVaiTro, trangThai, passwordHash, cancellationToken);

        if (outcome != WriteOutcome.Success)
        {
            return outcome == WriteOutcome.DuplicateEmail
                ? OperationResult<AccountDetailDto>.Conflict("Email đã được sử dụng bởi tài khoản khác.")
                : OperationResult<AccountDetailDto>.Conflict("Không cập nhật được tài khoản.");
        }

        var detail = await _accountStore.GetDetailAsync(maTaiKhoan, cancellationToken);

        return OperationResult<AccountDetailDto>.Ok(detail!);
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

        // Mật khẩu mặc định của trung tâm ("111111") — ghi đè hash cũ, không log plain text.
        var temporaryPassword = _temporaryPasswordGenerator.Generate();
        var passwordHash = _passwordHasherService.HashPassword(temporaryPassword);

        await _accountStore.UpdatePasswordHashAsync(maTaiKhoan, passwordHash, cancellationToken);

        return OperationResult<ResetPasswordResponse>.Ok(new ResetPasswordResponse
        {
            Username = account.TenDangNhap,
            TemporaryPassword = temporaryPassword,
            Message = "Đã đặt lại mật khẩu về giá trị mặc định 111111. Vui lòng yêu cầu người dùng đổi mật khẩu ngay sau khi đăng nhập."
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
