using LanguageCenter.Application.Abstractions;
using LanguageCenter.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Endpoint DEVELOPMENT-ONLY để xác nhận đã đọc được dữ liệu thật từ SQL Server
/// và các tiện ích đặt lại dữ liệu phục vụ demo.
/// Ở môi trường khác Development luôn trả 404.
/// </summary>
[ApiController]
[Route("api/development")]
public sealed class DevelopmentController : ControllerBase
{
    /// <summary>Mật khẩu mặc định dùng cho tiện ích đặt lại toàn bộ.</summary>
    public const string MatKhauMacDinh = "111111";

    private readonly AppDbContext _dbContext;
    private readonly IWebHostEnvironment _environment;
    private readonly IPasswordHasherService _passwordHasher;
    private readonly ILogger<DevelopmentController> _logger;

    public DevelopmentController(
        AppDbContext dbContext,
        IWebHostEnvironment environment,
        IPasswordHasherService passwordHasher,
        ILogger<DevelopmentController> logger)
    {
        _dbContext = dbContext;
        _environment = environment;
        _passwordHasher = passwordHasher;
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

    /// <summary>
    /// Đặt LẠI MẬT KHẨU cho TOÀN BỘ tài khoản (Development only) — mặc định <c>111111</c>.
    /// <para>
    /// Mỗi tài khoản được hash riêng (salt khác nhau) bằng đúng
    /// <see cref="IPasswordHasherService"/> của ứng dụng, nên đăng nhập được ngay.
    /// </para>
    /// </summary>
    /// <response code="200">Đặt lại thành công, trả về số tài khoản đã đổi.</response>
    /// <response code="400">Mật khẩu quá ngắn.</response>
    /// <response code="404">Chỉ tồn tại ở môi trường Development.</response>
    [HttpPost("reset-all-passwords")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ResetAllPasswordsAsync(
        [FromBody] ResetAllPasswordsRequest? request,
        CancellationToken cancellationToken)
    {
        if (!_environment.IsDevelopment())
        {
            return NotFound();
        }

        var matKhau = string.IsNullOrWhiteSpace(request?.MatKhau)
            ? MatKhauMacDinh
            : request!.MatKhau!.Trim();

        if (matKhau.Length < 6)
        {
            return BadRequest(new { message = "Mật khẩu phải có ít nhất 6 ký tự." });
        }

        var taiKhoans = await _dbContext.TaiKhoans.ToListAsync(cancellationToken);

        foreach (var taiKhoan in taiKhoans)
        {
            taiKhoan.MatKhauHash = _passwordHasher.HashPassword(matKhau);
            taiKhoan.NgayCapNhat = DateTime.Now;
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogWarning(
            "Đã đặt lại mật khẩu cho {SoTaiKhoan} tài khoản về mật khẩu mặc định (Development).",
            taiKhoans.Count);

        return Ok(new
        {
            soTaiKhoan = taiKhoans.Count,
            matKhau,
            danhSachTaiKhoan = taiKhoans.Select(x => x.TenDangNhap).ToList()
        });
    }

    /// <summary>Body của <c>POST /api/development/reset-all-passwords</c>.</summary>
    public sealed class ResetAllPasswordsRequest
    {
        /// <summary>Để trống → dùng <see cref="MatKhauMacDinh"/> ("111111").</summary>
        public string? MatKhau { get; set; }
    }
}
