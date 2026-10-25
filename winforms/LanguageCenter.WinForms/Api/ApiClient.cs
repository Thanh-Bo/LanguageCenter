using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace LanguageCenter.WinForms.Api;

/// <summary>Kết quả chuẩn hoá của một lời gọi API — thay cho việc ném exception ở mọi nơi.</summary>
public sealed class ApiResult<T>
{
    public bool Success { get; init; }

    /// <summary>0 nghĩa là không kết nối được tới máy chủ (mất mạng, sai URL, API chưa chạy...).</summary>
    public int StatusCode { get; init; }

    public T? Data { get; init; }

    public string ErrorMessage { get; init; } = string.Empty;

    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

    public bool IsUnauthorized => StatusCode == 401;

    public bool IsConflict => StatusCode == 409;

    public bool IsNotFound => StatusCode == 404;
}

/// <summary>
/// Client HTTP mỏng cho LanguageCenter.Api — tự gắn JWT Bearer token, tự parse lỗi
/// theo đúng khuôn dạng mà <c>ExceptionHandlingMiddleware</c> / <c>OperationResultExtensions</c>
/// của backend trả về: <c>{ message, errors }</c> hoặc <c>{ title, errors: { field: [...] } }</c>
/// (shape mặc định của ASP.NET Core khi model validation thất bại).
/// </summary>
public sealed class ApiClient
{
    private readonly HttpClient _http;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public ApiClient(string baseUrl)
    {
        _http = new HttpClient
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(30)
        };
        _http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public void SetAccessToken(string? token)
    {
        _http.DefaultRequestHeaders.Authorization = string.IsNullOrWhiteSpace(token)
            ? null
            : new AuthenticationHeaderValue("Bearer", token);
    }

    public Task<ApiResult<T>> GetAsync<T>(string path, CancellationToken ct = default)
        => SendAsync<T>(HttpMethod.Get, path, null, ct);

    public Task<ApiResult<TResp>> PostAsync<TResp>(string path, object? body, CancellationToken ct = default)
        => SendAsync<TResp>(HttpMethod.Post, path, body, ct);

    public Task<ApiResult<TResp>> PatchAsync<TResp>(string path, object? body, CancellationToken ct = default)
        => SendAsync<TResp>(HttpMethod.Patch, path, body, ct);

    public Task<ApiResult<TResp>> PutAsync<TResp>(string path, object? body, CancellationToken ct = default)
        => SendAsync<TResp>(HttpMethod.Put, path, body, ct);

    /// <summary>Dùng khi 2xx không có nội dung (204) nhưng khi lỗi (vd 409) có body cần đọc.</summary>
    public Task<ApiResult<T>> DeleteAsync<T>(string path, CancellationToken ct = default)
        => SendAsync<T>(HttpMethod.Delete, path, null, ct);

    private async Task<ApiResult<T>> SendAsync<T>(
        HttpMethod method,
        string path,
        object? body,
        CancellationToken ct)
    {
        if (AppConfig.UseMockData)
        {
            if (method == HttpMethod.Get)
            {
                return MockDataStore.HandleGet<T>(path);
            }
            if (method == HttpMethod.Post)
            {
                return MockDataStore.HandlePost<T>(path, body);
            }
            if (method == HttpMethod.Put || method == HttpMethod.Patch)
            {
                return MockDataStore.HandlePut<T>(path, body);
            }
            if (method == HttpMethod.Delete)
            {
                return (ApiResult<T>)(object)MockDataStore.HandleDelete(path);
            }
        }

        using var request = new HttpRequestMessage(method, path);

        if (body is not null)
        {
            request.Content = JsonContent.Create(body, options: JsonOptions);
        }

        HttpResponseMessage response;
        string responseBody;

        try
        {
            response = await _http.SendAsync(request, ct);
            responseBody = await response.Content.ReadAsStringAsync(ct);
        }
        catch (HttpRequestException ex)
        {
            return new ApiResult<T>
            {
                Success = false,
                StatusCode = 0,
                ErrorMessage = "Không thể kết nối tới máy chủ API HUIT LingoHub. Kiểm tra dịch vụ backend đã khởi động chưa và đường dẫn "
                    + $"'{AppConfig.ApiBaseUrl}' có đúng không.\n\nChi tiết: {ex.Message}"
            };
        }
        catch (TaskCanceledException) when (!ct.IsCancellationRequested)
        {
            return new ApiResult<T>
            {
                Success = false,
                StatusCode = 0,
                ErrorMessage = "Hết thời gian chờ phản hồi từ máy chủ API."
            };
        }

        var success = response.IsSuccessStatusCode;
        T? data = default;

        if (!string.IsNullOrWhiteSpace(responseBody))
        {
            try
            {
                data = JsonSerializer.Deserialize<T>(responseBody, JsonOptions);
            }
            catch (JsonException)
            {
                // Body lỗi không khớp kiểu T mong đợi — bỏ qua, ErrorMessage bên dưới vẫn được rút ra.
            }
        }

        if (success)
        {
            return new ApiResult<T> { Success = true, StatusCode = (int)response.StatusCode, Data = data };
        }

        var (errorMessage, errors) = ExtractError(responseBody, response.StatusCode);

        return new ApiResult<T>
        {
            Success = false,
            StatusCode = (int)response.StatusCode,
            Data = data,
            ErrorMessage = errorMessage,
            Errors = errors
        };
    }

    /// <summary>
    /// Đọc thông báo lỗi từ body JSON. Hỗ trợ cả ba khuôn dạng:
    /// (1) OperationResult của app: <c>{ message, errors: string[] }</c>.
    /// (2) ValidationProblem mặc định của [ApiController]: <c>{ title, errors: { field: string[] } }</c>.
    /// (3) 401 đơn giản: <c>{ message }</c>.
    /// <para>
    /// <b>Quan trọng:</b> <c>message</c> trả về đã bao gồm CHI TIẾT từng lỗi (mỗi dòng bắt đầu bằng
    /// "• "), để màn hình nào chỉ hiển thị <see cref="ApiResult{T}.ErrorMessage"/> cũng thấy
    /// ngay dữ liệu nào sai — không cần sửa từng chỗ gọi.
    /// </para>
    /// </summary>
    private static (string Message, IReadOnlyList<string> Errors) ExtractError(string body, System.Net.HttpStatusCode statusCode)
    {
        var fallback = $"Yêu cầu thất bại ({(int)statusCode} {statusCode}).";

        if (string.IsNullOrWhiteSpace(body))
        {
            return (fallback, Array.Empty<string>());
        }

        try
        {
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            var message = TryGetString(root, "message") ?? TryGetString(root, "title") ?? fallback;
            var errors = new List<string>();

            if (root.TryGetProperty("errors", out var errorsElement))
            {
                if (errorsElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var item in errorsElement.EnumerateArray())
                    {
                        if (item.ValueKind == JsonValueKind.String)
                        {
                            errors.Add(item.GetString() ?? string.Empty);
                        }
                    }
                }
                else if (errorsElement.ValueKind == JsonValueKind.Object)
                {
                    // Model validation: { "MaLop": ["Mã lớp là bắt buộc."] } → "MaLop: Mã lớp là bắt buộc."
                    foreach (var field in errorsElement.EnumerateObject())
                    {
                        if (field.Value.ValueKind != JsonValueKind.Array)
                        {
                            continue;
                        }

                        foreach (var item in field.Value.EnumerateArray())
                        {
                            if (item.ValueKind == JsonValueKind.String)
                            {
                                var noiDung = item.GetString();

                                if (!string.IsNullOrWhiteSpace(noiDung))
                                {
                                    errors.Add($"{field.Name}: {noiDung}");
                                }
                            }
                        }
                    }
                }
            }

            var chiTiet = errors
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Select(x => "• " + x.Trim())
                .ToList();

            var messageDayDu = chiTiet.Count == 0
                ? message
                : message + Environment.NewLine + string.Join(Environment.NewLine, chiTiet);

            return (messageDayDu, errors);
        }
        catch (JsonException)
        {
            return (fallback, Array.Empty<string>());
        }
    }

    private static string? TryGetString(JsonElement root, string propertyName)
        => root.TryGetProperty(propertyName, out var element) && element.ValueKind == JsonValueKind.String
            ? element.GetString()
            : null;
}
