namespace LanguageCenter.Api.Middleware;

public static class ExceptionHandlingMiddlewareExtensions
{
    /// <summary>Đăng ký middleware xử lý exception toàn cục. Đặt sớm trong pipeline.</summary>
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();
}
