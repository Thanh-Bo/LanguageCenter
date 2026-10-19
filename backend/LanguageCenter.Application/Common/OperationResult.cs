namespace LanguageCenter.Application.Common;

/// <summary>Kết quả tổng quát của một use case. Controller chỉ cần map sang HTTP, không chứa logic.</summary>
public enum OperationStatus
{
    Ok = 1,
    Created = 2,
    NoContent = 3,
    BadRequest = 4,
    NotFound = 5,
    Conflict = 6,
    Forbidden = 7
}

/// <summary>
/// Kết quả của một use case ở tầng Application.
/// Giữ cho Controller MỎNG: controller chỉ gọi service rồi map <see cref="Status"/> sang HTTP.
/// </summary>
public sealed class OperationResult<T>
{
    private OperationResult()
    {
    }

    public OperationStatus Status { get; private init; }

    public T? Value { get; private init; }

    public string? Message { get; private init; }

    /// <summary>Chi tiết lỗi validate (dùng cho 400).</summary>
    public IReadOnlyList<string> Errors { get; private init; } = [];

    public bool Succeeded => Status is OperationStatus.Ok or OperationStatus.Created or OperationStatus.NoContent;

    public static OperationResult<T> Ok(T value) => new() { Status = OperationStatus.Ok, Value = value };

    public static OperationResult<T> Created(T value, string? message = null)
        => new() { Status = OperationStatus.Created, Value = value, Message = message };

    public static OperationResult<T> NoContent() => new() { Status = OperationStatus.NoContent };

    public static OperationResult<T> BadRequest(string message, IReadOnlyList<string>? errors = null)
        => new() { Status = OperationStatus.BadRequest, Message = message, Errors = errors ?? [] };

    public static OperationResult<T> NotFound(string message)
        => new() { Status = OperationStatus.NotFound, Message = message };

    public static OperationResult<T> Conflict(string message)
        => new() { Status = OperationStatus.Conflict, Message = message };

    /// <summary>Đã xác thực nhưng không đủ quyền trên tài nguyên cụ thể (403).</summary>
    public static OperationResult<T> Forbidden(string message)
        => new() { Status = OperationStatus.Forbidden, Message = message };
}
