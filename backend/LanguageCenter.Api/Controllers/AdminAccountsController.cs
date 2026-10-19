using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Accounts;
using LanguageCenter.Application.Accounts.Models;
using LanguageCenter.Application.Common;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Quản trị tài khoản — CHỈ Quản trị viên.
/// Controller chỉ nhận request, gọi service và map kết quả sang HTTP.
/// </summary>
[ApiController]
[Route("api/admin/accounts")]
[Authorize(Roles = RoleNames.QuanTriVien)]
public sealed class AdminAccountsController : ControllerBase
{
    private readonly IAccountAdminService _accountAdminService;

    public AdminAccountsController(IAccountAdminService accountAdminService)
    {
        _accountAdminService = accountAdminService;
    }

    /// <summary>Danh sách tài khoản — phân trang, tìm kiếm, lọc theo vai trò và trạng thái.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<AccountListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] AccountListQuery query,
        CancellationToken cancellationToken)
        => (await _accountAdminService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết một tài khoản (kèm thông tin hồ sơ tương ứng).</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(AccountDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _accountAdminService.GetDetailAsync(id, cancellationToken)).ToActionResult();

    /// <summary>Tạo tài khoản mới (không kèm hồ sơ) — dùng cho Nhân viên / Kế toán / Tư vấn.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(AccountDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateAccountRequest request,
        CancellationToken cancellationToken)
        => (await _accountAdminService.CreateAsync(request, cancellationToken)).ToActionResult();

    /// <summary>Cập nhật email / vai trò / trạng thái / mật khẩu của tài khoản.</summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(AccountDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] UpdateAccountRequest request,
        CancellationToken cancellationToken)
    {
        var currentUserMaTaiKhoan = User.GetMaTaiKhoan();

        if (currentUserMaTaiKhoan is null)
        {
            return Unauthorized(new { message = "Token không hợp lệ." });
        }

        var result = await _accountAdminService.UpdateAsync(
            id,
            request,
            currentUserMaTaiKhoan.Value,
            cancellationToken);

        return result.ToActionResult();
    }

    /// <summary>Đổi trạng thái tài khoản: Hoạt động / Bị khóa / Ngừng sử dụng.</summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(AccountDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatusAsync(
        int id,
        [FromBody] UpdateAccountStatusRequest request,
        CancellationToken cancellationToken)
    {
        var currentUserMaTaiKhoan = User.GetMaTaiKhoan();

        if (currentUserMaTaiKhoan is null)
        {
            return Unauthorized(new { message = "Token không hợp lệ." });
        }

        var result = await _accountAdminService.UpdateStatusAsync(
            id,
            request,
            currentUserMaTaiKhoan.Value,
            cancellationToken);

        return result.ToActionResult();
    }

    /// <summary>Đặt lại mật khẩu — trả mật khẩu tạm ĐÚNG MỘT LẦN.</summary>
    [HttpPost("{id:int}/reset-password")]
    [ProducesResponseType(typeof(ResetPasswordResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetPasswordAsync(int id, CancellationToken cancellationToken)
        => (await _accountAdminService.ResetPasswordAsync(id, cancellationToken)).ToActionResult();

    /// <summary>
    /// Xóa cứng tài khoản — CHỈ khi không có dữ liệu nghiệp vụ phụ thuộc.
    /// Nếu có, trả 409 và hướng dẫn dùng trạng thái "Ngừng sử dụng".
    /// </summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(AccountDeleteBlockedResponse), StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        => (await _accountAdminService.DeleteAsync(id, cancellationToken)).ToActionResult();
}
