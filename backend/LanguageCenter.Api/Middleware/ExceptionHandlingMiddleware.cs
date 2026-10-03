using System.Net;

namespace LanguageCenter.Api.Middleware;

/// <summary>
/// Middleware xử lý exception tập trung — controller không cần try/catch lặp lại.
/// Development: trả thêm type/detail/stackTrace để debug.
/// Production : CHỈ trả thông báo chung, KHÔNG lộ stack trace hay chi tiết nội bộ.
/// </summary>
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            await WriteErrorResponseAsync(context, exception);
        }
    }

    private async Task WriteErrorResponseAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(
            exception,
            "Unhandled exception tại {Method} {Path} (TraceId: {TraceId})",
            context.Request.Method,
            context.Request.Path,
            context.TraceIdentifier);

        // Response đã bắt đầu gửi thì không thể sửa status/body nữa.
        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Response đã bắt đầu gửi — không thể ghi error response.");
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/json";

        var payload = new Dictionary<string, object?>
        {
            ["status"] = StatusCodes.Status500InternalServerError,
            ["title"] = "Đã xảy ra lỗi không mong muốn. Vui lòng thử lại sau.",
            ["traceId"] = context.TraceIdentifier
        };

        if (_environment.IsDevelopment())
        {
            payload["type"] = exception.GetType().Name;
            payload["detail"] = exception.Message;
            payload["stackTrace"] = exception.StackTrace;
        }

        await context.Response.WriteAsJsonAsync(payload);
    }
}
