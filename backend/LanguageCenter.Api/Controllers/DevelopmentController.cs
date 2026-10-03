using LanguageCenter.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Endpoint DEVELOPMENT-ONLY để xác nhận đã đọc được dữ liệu thật từ SQL Server.
/// Chỉ đọc (COUNT) — KHÔNG có CRUD. Không trả connection string.
/// Ở môi trường khác Development luôn trả 404.
/// </summary>
[ApiController]
[Route("api/development")]
public sealed class DevelopmentController : ControllerBase
{
    private readonly AppDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<DevelopmentController> _logger;

    public DevelopmentController(
        AppDbContext dbContext,
        IWebHostEnvironment environment,
        ILogger<DevelopmentController> logger)
    {
        _dbContext = dbContext;
        _environment = environment;
        _logger = logger;
    }

    /// <summary>Thông tin kết nối + số lượng record thật đọc từ database.</summary>
    /// <response code="200">Đọc dữ liệu thành công.</response>
    /// <response code="404">Chỉ tồn tại ở môi trường Development.</response>
    /// <response code="503">Không đọc được database.</response>
    [HttpGet("database-info")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status503ServiceUnavailable)]
    public async Task<IActionResult> GetDatabaseInfoAsync(CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var connection = _dbContext.Database.GetDbConnection();
        var databaseName = connection.Database;

        try
        {
            await _dbContext.Database.OpenConnectionAsync(cancellationToken);
            var serverVersion = connection.ServerVersion;
            await _dbContext.Database.CloseConnectionAsync();

            return Ok(new
            {
                database = databaseName,
                status = "Connected",
                provider = _dbContext.Database.ProviderName,
                sqlServerVersion = serverVersion,
                vaiTroCount = await _dbContext.VaiTros.CountAsync(cancellationToken),
                taiKhoanCount = await _dbContext.TaiKhoans.CountAsync(cancellationToken),
                hocVienCount = await _dbContext.HocViens.CountAsync(cancellationToken)
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Đọc thông tin database {Database} thất bại.", databaseName);

            return StatusCode(StatusCodes.Status503ServiceUnavailable, new
            {
                database = databaseName,
                status = "Disconnected"
            });
        }
    }
}
