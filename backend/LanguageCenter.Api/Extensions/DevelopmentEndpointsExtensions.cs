using System.Security.Claims;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;

namespace LanguageCenter.Api.Extensions;

/// <summary>
/// Endpoint kiểm chứng Role Authorization.
/// <para>
/// Phương thức này CHỈ được gọi khi <c>app.Environment.IsDevelopment()</c>, nên ở
/// Production các route này <b>không hề được đăng ký</b> → trả 404 thật, không phải
/// "có route nhưng chặn bằng if".
/// </para>
/// Không chứa business logic — chỉ để chứng minh 401/403 hoạt động đúng.
/// </summary>
public static class DevelopmentEndpointsExtensions
{
    private const string GroupTag = "Development";

    public static IEndpointRouteBuilder MapDevelopmentAuthEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/development/authenticated", (ClaimsPrincipal user) => Results.Ok(new
        {
            message = "Xác thực thành công.",
            userId = user.GetMaTaiKhoan(),
            username = user.GetTenDangNhap(),
            role = user.GetVaiTro()
        }))
        .RequireAuthorization()
        .WithTags(GroupTag)
        .WithSummary("[Development-only] Yêu cầu đăng nhập — mọi vai trò.");

        endpoints.MapGet("/api/development/admin-only", (ClaimsPrincipal user) => Results.Ok(new
        {
            message = "Bạn có quyền Quản trị viên.",
            username = user.GetTenDangNhap(),
            role = user.GetVaiTro()
        }))
        .RequireAuthorization(new AuthorizeAttribute { Roles = RoleNames.QuanTriVien })
        .WithTags(GroupTag)
        .WithSummary($"[Development-only] Chỉ vai trò '{RoleNames.QuanTriVien}'.");

        endpoints.MapGet("/api/development/teacher-only", (ClaimsPrincipal user) => Results.Ok(new
        {
            message = "Bạn có quyền Giáo viên.",
            username = user.GetTenDangNhap(),
            role = user.GetVaiTro()
        }))
        .RequireAuthorization(new AuthorizeAttribute { Roles = RoleNames.GiaoVien })
        .WithTags(GroupTag)
        .WithSummary($"[Development-only] Chỉ vai trò '{RoleNames.GiaoVien}'.");

        return endpoints;
    }
}
