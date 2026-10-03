namespace LanguageCenter.Application.Common;

/// <summary>
/// Kết quả phân trang. <see cref="TotalPages"/> được tính sẵn để client không phải tự chia.
/// </summary>
public sealed class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; init; } = [];

    public int Page { get; init; }

    public int PageSize { get; init; }

    public int TotalItems { get; init; }

    public int TotalPages => TotalItems == 0 ? 0 : (int)Math.Ceiling(TotalItems / (double)PageSize);

    public static PagedResult<T> Empty(int page, int pageSize)
        => new() { Items = [], Page = page, PageSize = pageSize, TotalItems = 0 };
}

/// <summary>
/// Query phân trang có TỰ CHUẨN HOÁ: <c>page &gt;= 1</c> và <c>1 &lt;= pageSize &lt;= 100</c>.
/// Nhờ vậy service và store không cần kiểm tra lại giá trị client gửi lên.
/// </summary>
public class PagedQuery
{
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;

    private int _page = 1;
    private int _pageSize = DefaultPageSize;

    public int Page
    {
        get => _page;
        set => _page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        set => _pageSize = value switch
        {
            < 1 => DefaultPageSize,
            > MaxPageSize => MaxPageSize,
            _ => value
        };
    }

    /// <summary>Số bản ghi cần bỏ qua — dùng trực tiếp cho <c>Skip()</c> ở tầng SQL.</summary>
    public int Skip => (Page - 1) * PageSize;

    /// <summary>Số bản ghi cần lấy — dùng trực tiếp cho <c>Take()</c> ở tầng SQL.</summary>
    public int Take => PageSize;
}
