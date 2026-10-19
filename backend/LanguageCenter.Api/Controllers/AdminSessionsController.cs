using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Sessions;
using LanguageCenter.Application.Sessions.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Ca học thực tế — Quản trị viên và Nhân viên học vụ.
/// <para>Điểm danh ghi trên ca học, không ghi trên lịch định kỳ.</para>
/// </summary>
[ApiController]
[Route("api/admin/sessions")]
[Authorize(Roles = RoleNames.QuanTriVien + "," + RoleNames.NhanVienHocVu)]
public sealed class AdminSessionsController : ControllerBase
{
    private readonly ISessionService _sessionService;

    public AdminSessionsController(ISessionService sessionService)
    {
        _sessionService = sessionService;
    }

    /// <summary>Danh sách ca học — lọc theo lớp, phòng, giáo viên, trạng thái, khoảng ngày.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<SessionListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] SessionListQuery query,
        CancellationToken cancellationToken)
        => (await _sessionService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết ca học.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(SessionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(int id, CancellationToken cancellationToken)
        => (await _sessionService.GetDetailAsync(id, cancellationToken)).ToActionResult();

    /// <summary>
    /// Tạo ca học. Kiểm tra xung đột phòng, xung đột giáo viên/trợ giảng,
    /// và chống trùng ca trong cùng ngày của một lớp.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SessionDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateSessionRequest request,
        CancellationToken cancellationToken)
        => (await _sessionService.CreateAsync(request, cancellationToken)).ToActionResult();

    /// <summary>Cập nhật ca học (PATCH).</summary>
    [HttpPatch("{id:int}")]
    [ProducesResponseType(typeof(SessionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        int id,
        [FromBody] UpdateSessionRequest request,
        CancellationToken cancellationToken)
        => (await _sessionService.UpdateAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>Đổi trạng thái ca học: "Chưa diễn ra" / "Đang diễn ra" / "Đã hoàn thành" / "Đã hủy".</summary>
    [HttpPatch("{id:int}/status")]
    [ProducesResponseType(typeof(SessionDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatusAsync(
        int id,
        [FromBody] UpdateStatusRequest request,
        CancellationToken cancellationToken)
        => (await _sessionService.UpdateStatusAsync(id, request, cancellationToken)).ToActionResult();

    /// <summary>Xóa ca học — CHỈ khi chưa có bản ghi điểm danh.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteAsync(int id, CancellationToken cancellationToken)
        => (await _sessionService.DeleteAsync(id, cancellationToken)).ToActionResult();

    /// <summary>
    /// Tạo hàng loạt ca học từ một lịch định kỳ trong khoảng ngày.
    /// <para>
    /// Idempotent: ngày đã có ca học được bỏ qua (trả về trong <c>ngayDaCo</c>).
    /// Ngày bị trùng phòng trả về trong <c>ngayXungDot</c> kèm lý do.
    /// </para>
    /// </summary>
    [HttpPost("generate")]
    [ProducesResponseType(typeof(GenerateSessionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GenerateAsync(
        [FromBody] GenerateSessionsRequest request,
        CancellationToken cancellationToken)
        => (await _sessionService.GenerateAsync(request, cancellationToken)).ToActionResult();
}
