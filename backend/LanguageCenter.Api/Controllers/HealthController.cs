using LanguageCenter.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Endpoint kiểm tra "sức khỏe" của hệ thống (Phase 1).
/// KHÔNG phải controller nghiệp vụ — chỉ chứng minh API → EF Core → SQL Server hoạt động.
/// </summary>
[ApiController]
[Route("health")]
public sealed class HealthController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<HealthController> _logger;

    public HealthController(AppDbContext dbContext, ILogger<HealthController> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    /// <summary>Kiểm tra API còn sống. Không truy cập database.</summary>
    /// <response code="200">API đang chạy.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get() => Ok(new { status = "Healthy" });

    /// <summary>Kiểm tra kết nối tới database QuanLyTrungTamNgoaiNgu qua EF Core.</summary>
    /// <response code="200">Kết nối thành công.</response>
    /// <response code="503">Không kết nối được database.</response>
    [HttpGet("database")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetDatabaseAsync(CancellationToken cancellationToken)
    {
        // Chỉ lấy TÊN database, tuyệt đối không trả connection string.
        var databaseName = _dbContext.Database.GetDbConnection().Database;

        // Health endpoint phải báo lỗi bằng 503 thay vì ném exception —
        // đây là ngoại lệ duy nhất được try/catch trong controller.
        try
        {
            if (!await _dbContext.Database.CanConnectAsync(cancellationToken))
            {
                _logger.LogWarning("Không kết nối được database {Database}.", databaseName);

                return StatusCode(StatusCodes.Status503ServiceUnavailable, new
                {
                    database = databaseName,
                    status = "Disconnected"
                });
            }

            var vaiTroCount = await _dbContext.VaiTros.CountAsync(cancellationToken);

            return Ok(new
            {
                database = databaseName,
                status = "Connected",
                vaiTroCount
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Kiểm tra kết nối database {Database} thất bại.", databaseName);

            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                database = databaseName,
                status = "Disconnected"
            });
        }
    }
}
