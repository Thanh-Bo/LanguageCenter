namespace LanguageCenter.Application.Common;

/// <summary>Kết quả của một thao tác ghi (update/delete) ở tầng lưu trữ.</summary>
public enum WriteOutcome
{
    Success = 0,

    /// <summary>Không tìm thấy bản ghi.</summary>
    NotFound = 1,

    /// <summary>Vi phạm ràng buộc UNIQUE (email đã tồn tại).</summary>
    DuplicateEmail = 2,

    /// <summary>Vi phạm ràng buộc khác (khóa ngoại, check constraint...).</summary>
    Conflict = 3
}
