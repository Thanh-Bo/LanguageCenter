using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LanguageCenter.Api.Extensions;

/// <summary>
/// CORS cho frontend Next.js.
/// Dùng danh sách origin TƯỜNG MINH (không AllowAnyOrigin) và KHÔNG bật AllowCredentials,
/// tránh tổ hợp AllowAnyOrigin + AllowCredentials không an toàn.
/// WinForms không bị ảnh hưởng vì không chạy trong browser.
/// </summary>
public static class CorsExtensions
{
    public const string PolicyName = "NextJsFrontend";

    private static readonly string[] DefaultOrigins = ["http://localhost:3000"];

    public static IServiceCollection AddNextJsCors(this IServiceCollection services, IConfiguration configuration)
    {
        var configured = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

        var allowedOrigins = configured is { Length: > 0 }
            ? configured
            : DefaultOrigins;

        services.AddCors(options =>
        {
            options.AddPolicy(PolicyName, policy =>
            {
                policy.WithOrigins(allowedOrigins)
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        return services;
    }
}
