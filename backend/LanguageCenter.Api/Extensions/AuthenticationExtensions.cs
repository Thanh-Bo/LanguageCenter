using LanguageCenter.Infrastructure.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace LanguageCenter.Api.Extensions;

/// <summary>
/// Cấu hình JWT Bearer authentication cho Web API.
/// </summary>
/// <remarks>
/// Validation BẮT BUỘC bật đầy đủ: signature, issuer, audience, lifetime.
/// KHÔNG disable signature validation, KHÔNG đặt ValidateLifetime = false (§21).
/// </remarks>
public static class AuthenticationExtensions
{
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        // Dùng chung 1 nguồn parse cấu hình với Infrastructure (đã validate ở AddInfrastructure).
        var jwtSettings = JwtSettings.FromConfiguration(configuration);
        jwtSettings.Validate();

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                // Giữ NGUYÊN tên claim trong token: sub / unique_name / role / jti.
                // Không để ASP.NET Core map sang ClaimTypes.* để tránh nhập nhằng phiên bản.
                options.MapInboundClaims = false;

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,

                    ValidateLifetime = true,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtSettings.GetSigningKeyBytes()),

                    // Token 60 phút: không cho phép lệch giờ 5 phút mặc định,
                    // nếu không token hết hạn vẫn dùng được thêm 5 phút.
                    ClockSkew = TimeSpan.Zero,

                    NameClaimType = JwtTokenGenerator.UsernameClaim,
                    RoleClaimType = JwtTokenGenerator.RoleClaim
                };
            });

        services.AddAuthorization();

        return services;
    }
}
