using System.ComponentModel.DataAnnotations;

namespace LanguageCenter.Application.Auth.Models;

/// <summary>
/// Body của POST /api/auth/change-password.
/// KHÔNG có MaTaiKhoan — tài khoản luôn được lấy từ JWT của người đang gọi.
/// </summary>
public sealed class ChangePasswordRequest
{
    [Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc.")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu mới là bắt buộc.")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
