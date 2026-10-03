using LanguageCenter.Application.Accounts;
using LanguageCenter.Application.Auth;
using LanguageCenter.Application.Students;
using LanguageCenter.Application.Teachers;
using Microsoft.Extensions.DependencyInjection;

namespace LanguageCenter.Application;

/// <summary>
/// Đăng ký các service nghiệp vụ thuộc tầng Application.
/// Api chỉ cần gọi <c>builder.Services.AddApplication()</c>.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Tất cả service đều scoped vì phụ thuộc các store dùng DbContext (scoped).
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAccountAdminService, AccountAdminService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ITeacherService, TeacherService>();

        return services;
    }
}
