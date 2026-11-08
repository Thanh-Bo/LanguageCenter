using LanguageCenter.Domain.Constants;
using LanguageCenter.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// SAO LƯU / PHỤC HỒI cơ sở dữ liệu — CHỈ Quản trị viên.
/// <para>
/// <c>GET  /api/admin/database/backups</c>  — danh sách file .bak có sẵn
/// <c>POST /api/admin/database/backups</c>  — tạo bản sao lưu mới
/// <c>POST /api/admin/database/restore</c>  — phục hồi từ 1 file .bak (PHÁ HUỶ dữ liệu hiện tại)
/// </para>
/// </summary>
[ApiController]
[Route("api/admin/database")]
[Authorize(Roles = RoleNames.QuanTriVien)]
public sealed class AdminDatabaseController : ControllerBase
{
    private readonly IDatabaseBackupService _backupService;

    public AdminDatabaseController(IDatabaseBackupService backupService)
    {
        _backupService = backupService;
    }

    /// <summary>Thông tin database + thư mục sao lưu mặc định.</summary>
    [HttpGet("info")]
    public IActionResult GetInfo()
        => Ok(new
        {
            tenDatabase = _backupService.TenDatabase,
            thuMucMacDinh = _backupService.ThuMucMacDinh
        });

    /// <summary>Danh sách file sao lưu (mới nhất trước).</summary>
    [HttpGet("backups")]
    public async Task<IActionResult> ListAsync(
        [FromQuery] string? thuMuc,
        CancellationToken cancellationToken)
    {
        try
        {
            var files = await _backupService.DanhSachAsync(thuMuc, cancellationToken);

            return Ok(new
            {
                thuMuc = string.IsNullOrWhiteSpace(thuMuc) ? _backupService.ThuMucMacDinh : thuMuc,
                items = files.Select(f => new
                {
                    tenFile = f.TenFile,
                    duongDan = f.DuongDan,
                    kichThuocByte = f.KichThuocByte,
                    kichThuocMb = Math.Round(f.KichThuocByte / 1024d / 1024d, 2),
                    thoiDiem = f.ThoiDiem
                })
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Không đọc được danh sách sao lưu: {ex.Message}" });
        }
    }

    /// <summary>Tạo bản sao lưu mới. Bỏ trống <paramref name="thuMuc"/> = dùng thư mục mặc định.</summary>
    [HttpPost("backups")]
    public async Task<IActionResult> CreateAsync(
        [FromBody] BackupRequest? request,
        CancellationToken cancellationToken)
    {
        try
        {
            var duongDan = await _backupService.SaoLuuAsync(request?.ThuMuc, cancellationToken);

            return Ok(new
            {
                message = "Sao lưu thành công.",
                duongDan,
                tenFile = Path.GetFileName(duongDan)
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Sao lưu thất bại: {ex.Message}" });
        }
    }

    /// <summary>
    /// PHỤC HỒI database từ file .bak. Dữ liệu hiện tại bị THAY THẾ HOÀN TOÀN.
    /// Sau khi phục hồi, nên khởi động lại API.
    /// </summary>
    [HttpPost("restore")]
    public async Task<IActionResult> RestoreAsync(
        [FromBody] RestoreRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.DuongDan))
        {
            return BadRequest(new { message = "Thiếu đường dẫn file sao lưu." });
        }

        try
        {
            await _backupService.PhucHoiAsync(request.DuongDan.Trim(), cancellationToken);

            return Ok(new
            {
                message = "Phục hồi thành công. Hãy khởi động lại API để làm mới kết nối."
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = $"Phục hồi thất bại: {ex.Message}" });
        }
    }

    public sealed class BackupRequest
    {
        public string? ThuMuc { get; set; }
    }

    public sealed class RestoreRequest
    {
        public string DuongDan { get; set; } = string.Empty;
    }
}
