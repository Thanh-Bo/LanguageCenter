namespace LanguageCenter.Application.Common;

/// <summary>
/// Kết quả của thao tác tạo mới — kèm khóa của bản ghi vừa tạo.
/// <para>
/// Dùng <see cref="WriteOutcome"/> để giữ nhất quán với Phase 3, nhưng bổ sung <see cref="Id"/>
/// vì tầng Application cần biết bản ghi nào vừa được tạo để trả chi tiết cho client.
/// </para>
/// </summary>
public readonly record struct WriteResult(WriteOutcome Outcome, int Id)
{
    public static WriteResult Success(int id) => new(WriteOutcome.Success, id);

    public static WriteResult NotFound() => new(WriteOutcome.NotFound, 0);

    /// <summary>Vi phạm ràng buộc UNIQUE hoặc CHECK constraint.</summary>
    public static WriteResult Conflict() => new(WriteOutcome.Conflict, 0);

    public bool Succeeded => Outcome == WriteOutcome.Success;
}
