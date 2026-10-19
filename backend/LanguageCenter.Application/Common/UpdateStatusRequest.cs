using System.ComponentModel.DataAnnotations;

namespace LanguageCenter.Application.Common;

/// <summary>
/// Body dùng CHUNG cho các endpoint đổi trạng thái danh mục Phase 4:
/// <c>PATCH /api/admin/{resource}/{id}/status</c>.
/// <para>
/// Mỗi service tự kiểm tra giá trị có thuộc tập trạng thái hợp lệ của mình hay không,
/// vì mỗi bảng có bộ giá trị khác nhau.
/// </para>
/// </summary>
public sealed class UpdateStatusRequest
{
    [Required(ErrorMessage = "Trạng thái là bắt buộc.")]
    public string TrangThai { get; set; } = string.Empty;
}
