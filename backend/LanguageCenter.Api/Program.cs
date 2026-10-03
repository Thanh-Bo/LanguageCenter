using LanguageCenter.Api.Extensions;
using LanguageCenter.Api.Middleware;
using LanguageCenter.Application;
using LanguageCenter.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// ============================================================
// 1. ĐĂNG KÝ SERVICE (Dependency Injection)
// ============================================================

builder.Services.AddControllers();

// Swagger / OpenAPI
builder.Services.AddSwaggerDocumentation();

// DbContext + SQL Server + password hasher + JWT generator
builder.Services.AddInfrastructure(builder.Configuration);

// Nghiệp vụ tầng Application (AuthService)
builder.Services.AddApplication();

// JWT Bearer authentication + authorization
builder.Services.AddJwtAuthentication(builder.Configuration);

// CORS cho frontend Next.js (http://localhost:3000)
builder.Services.AddNextJsCors(builder.Configuration);

var app = builder.Build();

// ============================================================
// 2. HTTP REQUEST PIPELINE
// ============================================================

// Xử lý exception tập trung — đặt sớm để bắt mọi lỗi phía sau.
app.UseGlobalExceptionHandling();

if (app.Environment.IsDevelopment())
{
    // Swagger UI: http://localhost:5204/swagger
    app.UseSwaggerDocumentation();
}

// CORS phải chạy TRƯỚC Authentication/Authorization để request preflight (OPTIONS)
// được xử lý và short-circuit đúng cách.
app.UseCors(CorsExtensions.PolicyName);

// Thứ tự BẮT BUỘC: Authentication trước, Authorization sau.
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

if (app.Environment.IsDevelopment())
{
    // Endpoint kiểm chứng Role Authorization — route chỉ được ĐĂNG KÝ ở Development.
    app.MapDevelopmentAuthEndpoints();
}

// Lưu ý: KHÔNG bật UseHttpsRedirection ở môi trường dev —
// profile "http" chỉ bind cổng http, bật redirect sẽ gây cảnh báo
// "Failed to determine the https port" và làm Swagger khó truy cập.

app.Run();
