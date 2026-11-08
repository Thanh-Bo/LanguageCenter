using LanguageCenter.Api.Extensions;
using LanguageCenter.Application.Abstractions;
using LanguageCenter.Application.Common;
using LanguageCenter.Application.Employees;
using LanguageCenter.Application.Employees.Models;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Quản lý NHÂN VIÊN (bảng <c>NhanVien</c>) — Nhân viên học vụ / tư vấn / kế toán.
/// <para>
/// <c>GET /api/admin/employees</c> (không có <c>/list</c>) là danh mục RÚT GỌN dùng cho các ô chọn
/// trên WinForms — giữ nguyên hình dạng cũ (<c>items[].maNhanVien/hoTen/chucVu/trangThaiLamViec</c>)
/// để không làm hỏng <c>FormLopHoc</c>.
/// </para>
/// </summary>
[ApiController]
[Route("api/admin/employees")]
[Authorize(Roles = RoleNames.QuanTriVien + "," + RoleNames.NhanVienHocVu)]
public sealed class AdminEmployeesController : ControllerBase
{
    private readonly IReferenceStore _referenceStore;
    private readonly IEmployeeService _employeeService;

    public AdminEmployeesController(
        IReferenceStore referenceStore,
        IEmployeeService employeeService)
    {
        _referenceStore = referenceStore;
        _employeeService = employeeService;
    }

    // ========================================================================
    // DANH MỤC RÚT GỌN (ô chọn trên WinForms) — KHÔNG ĐỔI HÌNH DẠNG
    // ========================================================================

    /// <summary>
    /// Toàn bộ nhân viên dạng rút gọn (sắp theo mã tăng dần).
    /// Lọc <c>?dangLamViec=true</c> để chỉ lấy người đang làm việc.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> ListAsync(
        [FromQuery] bool dangLamViec,
        CancellationToken cancellationToken)
    {
        var items = await _referenceStore.ListStaffAsync(cancellationToken);

        if (dangLamViec)
        {
            items = items
                .Where(x => x.TrangThaiLamViec == WorkStatuses.DangLamViec)
                .ToList();
        }

        return Ok(new
        {
            items = items.Select(x => new
            {
                maNhanVien = x.MaNhanVien,
                hoTen = x.HoTen,
                chucVu = x.ChucVu,
                trangThaiLamViec = x.TrangThaiLamViec
            }),
            totalItems = items.Count
        });
    }

    // ========================================================================
    // QUẢN LÝ ĐẦY ĐỦ (CRUD) — chỉ Quản trị viên
    // ========================================================================

    /// <summary>
    /// Danh sách nhân viên có phân trang / tìm kiếm / lọc chức vụ / lọc trạng thái.
    /// <para>Dùng <c>/list</c> để không đụng route <c>{maNhanVien}</c>.</para>
    /// </summary>
    [HttpGet("list")]
    [Authorize(Roles = RoleNames.QuanTriVien)]
    [ProducesResponseType(typeof(PagedResult<EmployeeListItemDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListPagedAsync(
        [FromQuery] EmployeeListQuery query,
        CancellationToken cancellationToken)
        => (await _employeeService.ListAsync(query, cancellationToken)).ToActionResult();

    /// <summary>
    /// Tạo nhân viên: backend tự sinh mã theo chức vụ (NVHV/NVTV/NVKT), tự tạo
    /// <c>TaiKhoan</c> + <c>NhanVien</c> trong một transaction, tự sinh mật khẩu tạm
    /// và trả về ĐÚNG MỘT LẦN.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = RoleNames.QuanTriVien)]
    [ProducesResponseType(typeof(CreateEmployeeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CreateAsync(
        [FromBody] CreateEmployeeRequest request,
        CancellationToken cancellationToken)
        => (await _employeeService.CreateAsync(request, cancellationToken)).ToActionResult();

    /// <summary>Chi tiết nhân viên (kèm thông tin tài khoản đăng nhập).</summary>
    [HttpGet("{maNhanVien}")]
    [Authorize(Roles = RoleNames.QuanTriVien)]
    [ProducesResponseType(typeof(EmployeeDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetDetailAsync(string maNhanVien, CancellationToken cancellationToken)
        => (await _employeeService.GetDetailAsync(maNhanVien, cancellationToken)).ToActionResult();

    /// <summary>
    /// Cập nhật hồ sơ nhân viên. <b>Không đổi được chức vụ</b> — đổi chức vụ sẽ phải đổi vai trò
    /// và mã đăng nhập nên làm hỏng dữ liệu lịch sử.
    /// </summary>
    [HttpPatch("{maNhanVien}")]
    [Authorize(Roles = RoleNames.QuanTriVien)]
    [ProducesResponseType(typeof(EmployeeDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateAsync(
        string maNhanVien,
        [FromBody] UpdateEmployeeRequest request,
        CancellationToken cancellationToken)
        => (await _employeeService.UpdateAsync(maNhanVien, request, cancellationToken)).ToActionResult();

    /// <summary>
    /// Xóa cứng nhân viên + tài khoản — CHỈ khi không có dữ liệu nghiệp vụ phụ thuộc.
    /// Nếu có, trả 409 và hướng dẫn đổi Trạng thái làm việc.
    /// </summary>
    [HttpDelete("{maNhanVien}")]
    [Authorize(Roles = RoleNames.QuanTriVien)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteAsync(string maNhanVien, CancellationToken cancellationToken)
        => (await _employeeService.DeleteAsync(maNhanVien, cancellationToken)).ToActionResult();
}
