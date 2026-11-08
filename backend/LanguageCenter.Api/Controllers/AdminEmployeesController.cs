using LanguageCenter.Application.Abstractions;
using LanguageCenter.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LanguageCenter.Api.Controllers;

/// <summary>
/// Danh mục NHÂN VIÊN (bảng <c>NhanVien</c>) — chỉ đọc.
/// <para>
/// Dùng cho các ô chọn "Nhân viên học vụ phụ trách", "Nhân viên tư vấn",
/// "Nhân viên kế toán" trên WinForms. Trước đây các form này lấy danh sách
/// từ dữ liệu lớp đã có nên khi cơ sở dữ liệu trống thì không tạo được lớp.
/// </para>
/// </summary>
[ApiController]
[Route("api/admin/employees")]
[Authorize(Roles = RoleNames.QuanTriVien + "," + RoleNames.NhanVienHocVu)]
public sealed class AdminEmployeesController : ControllerBase
{
    private readonly IReferenceStore _referenceStore;

    public AdminEmployeesController(IReferenceStore referenceStore)
    {
        _referenceStore = referenceStore;
    }

    /// <summary>
    /// Toàn bộ nhân viên (mới nhất theo mã giảm dần KHÔNG áp dụng — sắp theo mã tăng dần).
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
}
