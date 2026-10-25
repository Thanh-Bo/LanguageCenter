namespace LanguageCenter.WinForms.Api;

/// <summary>
/// Tiện ích hiển thị lỗi API.
/// <para>
/// <b>Lưu ý:</b> chi tiết từng trường đã được <c>ApiClient.ExtractError</c> ghép sẵn vào
/// <see cref="ApiResult{T}.ErrorMessage"/> (mỗi dòng bắt đầu bằng "• "), nên màn hình nào
/// hiển thị <c>ErrorMessage</c> đều đã có thông tin "sai ở phần nào".
/// Hàm dưới đây giữ lại để nơi gọi cũ không phải sửa, và để sau này đổi cách trình bày
/// ở một chỗ duy nhất.
/// </para>
/// </summary>
public static class ApiResultExtensions
{
    /// <summary>Thông điệp đầy đủ (đã bao gồm chi tiết từng lỗi) để hiển thị cho người dùng.</summary>
    public static string ThongDiepDayDu<T>(this ApiResult<T> result)
        => string.IsNullOrWhiteSpace(result.ErrorMessage)
            ? "Yêu cầu thất bại."
            : result.ErrorMessage;
}

