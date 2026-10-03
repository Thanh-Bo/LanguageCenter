using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Students;
using LanguageCenter.Application.Students.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>Admin quản lý học viên. KHÔNG có public signup — học viên không tự tạo tài khoản.</summary>
[ApiController]
[Route("api/admin/students")]
[Authorize(Roles = RoleNames.QuanTriVien)]
public sealed class AdminStudentsController : ControllerBase
{
    private readonly IStudentService _studentService;

    public AdminStudentsController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    /// <summary>
    /// Tạo học viên: backend tự sinh mã HVxxx, tự tạo TaiKhoan + HocVien (+ PhuHuynh) trong một transaction,
    /// tự sinh mật khẩu tạm và trả về ĐÚNG MỘT LẦN.
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CreateStudentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateStudentRequest request,
        CancellationToken cancellationToken)
        => (await _studentService.CreateAsync(request, cancellationToken)).ToActionResult();

    /// <summary>Danh sách học viên — phân trang, tìm kiếm, lọc theo trạng thái học tập.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<StudentListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListAsync(
        [FromQuery] StudentListQuery query,
        CancellationToken cancellationToken)
        => (await _studentService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết học viên theo mã (ví dụ HV001).</summary>
    [HttpGet("{studentId}")]
    [ProducesResponseType(typeof(StudentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(string studentId, CancellationToken cancellationToken)
        => (await _studentService.GetDetailAsync(studentId, cancellationToken)).ToActionResult();

    /// <summary>Cập nhật hồ sơ học viên (kể cả TrangThaiHocTap / NgayNhapHoc). Email được đồng bộ sang TaiKhoan.</summary>
    [HttpPatch("{studentId}")]
    [ProducesResponseType(typeof(StudentDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        string studentId,
        [FromBody] UpdateStudentRequest request,
        CancellationToken cancellationToken)
        => (await _studentService.UpdateAsync(studentId, request, cancellationToken)).ToActionResult();
}
