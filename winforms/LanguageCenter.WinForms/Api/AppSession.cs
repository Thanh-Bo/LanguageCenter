using System.Text;

namespace LanguageCenter.WinForms.Api;

/// <summary>Phiên đăng nhập hiện tại của ứng dụng WinForms — một người dùng tại một thời điểm.</summary>
public static class AppSession
{
    public static ApiClient Api { get; } = new(AppConfig.ApiBaseUrl);

    public static CurrentUserDto? CurrentUser { get; private set; }

    public static DateTime? TokenExpiresAtUtc { get; private set; }

    public static bool IsLoggedIn => CurrentUser is not null;

    public static void SetLoggedIn(LoginResponse response)
    {
        Api.SetAccessToken(response.AccessToken);
        CurrentUser = response.User;
        TokenExpiresAtUtc = response.ExpiresAt;
    }

    public static void Clear()
    {
        Api.SetAccessToken(null);
        CurrentUser = null;
        TokenExpiresAtUtc = null;
    }
}

/// <summary>Helper dựng query string cho các API danh sách (phân trang / tìm kiếm / lọc).</summary>
public static class QueryStringBuilder
{
    public static string Build(string path, params (string Key, string? Value)[] parameters)
    {
        var query = new List<string>();

        foreach (var (key, value) in parameters)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                query.Add($"{Uri.EscapeDataString(key)}={Uri.EscapeDataString(value)}");
            }
        }

        if (query.Count == 0)
        {
            return path;
        }

        var sb = new StringBuilder(path);
        sb.Append('?').Append(string.Join('&', query));
        return sb.ToString();
    }
}
