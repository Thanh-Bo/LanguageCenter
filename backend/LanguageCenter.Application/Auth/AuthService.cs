using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Auth.Models;
using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Auth;

/// <inheritdoc cref="IAuthService"/>
/// <remarks>
/// Toàn bộ logic authentication nằm ở đây — KHÔNG nằm trong Controller.
/// Chỉ phụ thuộc abstraction (IAccountStore / IPasswordHasherService / IJwtTokenGenerator),
/// KHÔNG phụ thuộc EF Core, ASP.NET Core Identity hay Api.
/// </remarks>
public sealed class AuthService : IAuthService
{
    private readonly IAccountStore _accountStore;
    private readonly IPasswordHasherService _passwordHasherService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;

    public AuthService(
        IAccountStore accountStore,
        IPasswordHasherService passwordHasherService,
        IJwtTokenGenerator jwtTokenGenerator)
    {
        _accountStore = accountStore;
        _passwordHasherService = passwordHasherService;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken)
    {
        var tenDangNhap = request.Username.Trim();

        var account = await _accountStore.FindByUsernameAsync(tenDangNhap, cancellationToken);

        // Không tìm thấy tài khoản -> vẫn trả lỗi CHUNG, không tiết lộ tài khoản có tồn tại hay không.
        if (account is null)
        {
            return LoginResult.Failure(LoginFailureReason.InvalidCredentials);
        }

        // Chỉ tài khoản "Hoạt động" được đăng nhập. Kiểm tra TRƯỚC khi verify mật khẩu.
        if (!string.Equals(account.TrangThaiTaiKhoan, AccountStatuses.HoatDong, StringComparison.Ordinal))
        {
            return LoginResult.Failure(LoginFailureReason.AccountNotActive);
        }

        if (!_passwordHasherService.VerifyPassword(request.Password, account.MatKhauHash))
        {
            return LoginResult.Failure(LoginFailureReason.InvalidCredentials);
        }

        // Ghi nhận thời điểm đăng nhập (theo giờ SQL Server, nhất quán với các default khác trong DB).
        await _accountStore.TouchLastLoginAsync(account.MaTaiKhoan, cancellationToken);

        var token = _jwtTokenGenerator.Generate(account.MaTaiKhoan, account.TenDangNhap, account.TenVaiTro);

        return LoginResult.Success(new LoginResponse
        {
            AccessToken = token.AccessToken,
            TokenType = "Bearer",
            ExpiresAt = token.ExpiresAtUtc,
            User = new CurrentUserDto
            {
                Id = account.MaTaiKhoan,
                Username = account.TenDangNhap,
                Email = account.Email,
                Role = account.TenVaiTro
            }
        });
    }

    public async Task<CurrentUserDto?> GetCurrentUserAsync(int maTaiKhoan, CancellationToken cancellationToken)
    {
        var account = await _accountStore.FindByIdAsync(maTaiKhoan, cancellationToken);

        if (account is null)
        {
            return null;
        }

        return new CurrentUserDto
        {
            Id = account.MaTaiKhoan,
            Username = account.TenDangNhap,
            Email = account.Email,
            Role = account.TenVaiTro
        };
    }

    public async Task<ChangePasswordResult> ChangePasswordAsync(
        int maTaiKhoan,
        ChangePasswordRequest request,
        CancellationToken cancellationToken)
    {
        // (1) Tài khoản hiện tại — luôn từ JWT, KHÔNG bao giờ từ body.
        var account = await _accountStore.FindByIdAsync(maTaiKhoan, cancellationToken);

        if (account is null)
        {
            return ChangePasswordResult.Failure(
                ChangePasswordFailureReason.AccountNotFound,
                ["Không tìm thấy tài khoản."]);
        }

        // (2) Xác minh mật khẩu hiện tại.
        if (!_passwordHasherService.VerifyPassword(request.CurrentPassword, account.MatKhauHash))
        {
            return ChangePasswordResult.Failure(
                ChangePasswordFailureReason.InvalidCurrentPassword,
                ["Mật khẩu hiện tại không đúng."]);
        }

        // (3) Chính sách mật khẩu mới.
        var policyErrors = PasswordPolicy.Validate(request.NewPassword);

        if (policyErrors.Count > 0)
        {
            return ChangePasswordResult.Failure(
                ChangePasswordFailureReason.NewPasswordPolicyViolation,
                policyErrors);
        }

        // (4) Xác nhận mật khẩu.
        if (!string.Equals(request.NewPassword, request.ConfirmPassword, StringComparison.Ordinal))
        {
            return ChangePasswordResult.Failure(
                ChangePasswordFailureReason.PasswordConfirmationMismatch,
                ["Mật khẩu xác nhận không khớp với mật khẩu mới."]);
        }

        // (5) Không cho "đổi" thành đúng mật khẩu cũ.
        if (string.Equals(request.NewPassword, request.CurrentPassword, StringComparison.Ordinal))
        {
            return ChangePasswordResult.Failure(
                ChangePasswordFailureReason.NewPasswordSameAsCurrent,
                ["Mật khẩu mới phải khác mật khẩu hiện tại."]);
        }

        // (6) + (7) Băm mật khẩu mới rồi cập nhật MatKhauHash & NgayCapNhat.
        var newHash = _passwordHasherService.HashPassword(request.NewPassword);

        await _accountStore.UpdatePasswordHashAsync(maTaiKhoan, newHash, cancellationToken);

        return ChangePasswordResult.Success();
    }
}
