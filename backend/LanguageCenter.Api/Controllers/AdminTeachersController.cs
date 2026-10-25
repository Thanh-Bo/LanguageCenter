using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Teachers;
using LanguageCenter.Application.Teachers.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Admin quản lý Giáo viên và Trợ giảng (dùng chung endpoint; <c>LoaiGiaoVien</c> quyết định mã và vai trò).
/// </summary>
[ApiController]
[Route("api/admin/teachers")]
[Authorize(Roles = RoleNames.QuanTriVien)]
public sealed class AdminTeachersController : ControllerBase
{
    private readonly ITeacherService _teacherService;

    public AdminTeachersController(ITeacherService teacherService)
    {
        _teacherService = teacherService;
    }

    /// <summary>
    /// Tạo giáo viên hoặc trợ giảng: backend tự sinh GVxxx/TGxxx, tự tạo TaiKhoan + GiaoVien trong một transaction,
    /// tự sinh mật khẩu tạm và trả về ĐÚNG MỘT LẦN.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateTeacherResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateTeacherRequest request,
        CancellationToken cancellationToken)
        => (await _teacherService.CreateAsync(request, cancellationToken)).ToActionResult();

    /// <summary>Danh sách giáo viên — phân trang, tìm kiếm, lọc theo loại giáo viên.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<TeacherListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] TeacherListQuery query,
        CancellationToken cancellationToken)
        => (await _teacherService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết giáo viên (kèm Bằng cấp / Chứng chỉ — chỉ đọc ở Phase 3).</summary>
    [HttpGet("{teacherId}")]
    [ProducesResponseType(typeof(TeacherDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(string teacherId, CancellationToken cancellationToken)
        => (await _teacherService.GetDetailAsync(teacherId, cancellationToken)).ToActionResult();

    /// <summary>
    /// Cập nhật hồ sơ giáo viên.
    /// <b>LoaiGiaoVien KHÔNG sửa được qua endpoint này</b> — xem ghi chú trong tài liệu Phase 3.
    /// </summary>
    [HttpPatch("{teacherId}")]
    [ProducesResponseType(typeof(TeacherDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        string teacherId,
        [FromBody] UpdateTeacherRequest request,
        CancellationToken cancellationToken)
        => (await _teacherService.UpdateAsync(teacherId, request, cancellationToken)).ToActionResult();

    /// <summary>
    /// Xóa cứng giáo viên / trợ giảng + tài khoản — CHỈ khi không có dữ liệu nghiệp vụ phụ thuộc.
    /// Nếu có, trả 409 và hướng dẫn đổi Trạng thái làm việc.
    /// </summary>
    [HttpDelete("{teacherId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(string teacherId, CancellationToken cancellationToken)
        => (await _teacherService.DeleteAsync(teacherId, cancellationToken)).ToActionResult();
}
