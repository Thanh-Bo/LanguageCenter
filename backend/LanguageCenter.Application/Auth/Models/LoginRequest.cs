using System.ComponentModel.DataAnnotations;

namespace LanguageCenter.Application.Auth.Models;

/// <summary>Body của POST /api/auth/login.</summary>
public sealed class LoginRequest
{
    [Required(ErrorMessage = "Tên đăng nhập là bắt buộc.")]
    [StringLength(20, ErrorMessage = "Tên đăng nhập tối đa 20 ký tự.")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc.")]
    [StringLength(128, ErrorMessage = "Mật khẩu tối đa 128 ký tự.")]
    public string Password { get; set; } = string.Empty;
}
