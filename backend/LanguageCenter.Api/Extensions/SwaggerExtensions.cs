using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi;

namespace LanguageCenter.Api.Extensions;

/// <summary>
/// Cấu hình Swagger / Swagger UI, bao gồm nút <b>Authorize</b> để nhập JWT Bearer token.
/// </summary>
public static class SwaggerExtensions
{
    private const string SecuritySchemeId = "Bearer";

    public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new()
            {
                Title = "LanguageCenter API",
                Version = "v1",
                Description =
                    "Hệ thống quản lý trung tâm ngoại ngữ.\n\n" +
                    "**Cách dùng JWT:** gọi `POST /api/auth/login` → copy `accessToken` → " +
                    "bấm nút **Authorize** → dán token (không cần gõ chữ `Bearer`) → gọi các API cần đăng nhập."
            });

            // Khai báo scheme Bearer để Swagger UI hiện nút Authorize.
            options.AddSecurityDefinition(SecuritySchemeId, new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Description = "JWT access token lấy từ /api/auth/login. Dán token trực tiếp, KHÔNG gõ thêm chữ 'Bearer'.",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });

            // Yêu cầu này làm Swagger UI tự gắn header Authorization vào mọi request "Try it out".
            options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference(SecuritySchemeId, document)] = []
            });
        });

        return services;
    }

    public static IApplicationBuilder UseSwaggerDocumentation(this IApplicationBuilder app)
    {
        app.UseSwagger();

        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "LanguageCenter API v1");
            options.DocumentTitle = "LanguageCenter API";
            options.DisplayRequestDuration();
        });

        return app;
    }
}
