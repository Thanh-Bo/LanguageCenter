using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace LanguageCenter.Infrastructure.Persistence;

/// <summary>
/// Nhận diện lỗi ràng buộc của SQL Server để tầng lưu trữ trả về
/// <see cref="LanguageCenter.Application.Common.WriteOutcome.Conflict"/>
/// thay vì để exception thoát ra ngoài thành HTTP 500.
/// </summary>
internal static class SqlErrorHelper
{
    /// <summary>Vi phạm UNIQUE index (2601) hoặc UNIQUE constraint / PRIMARY KEY (2627).</summary>
    public static bool IsUniqueViolation(Exception exception)
    {
        var inner = exception is DbUpdateException dbUpdateException
            ? dbUpdateException.InnerException
            : exception;

        return inner is SqlException { Number: 2601 or 2627 };
    }

    /// <summary>Vi phạm CHECK constraint hoặc FOREIGN KEY (547).</summary>
    public static bool IsConstraintViolation(Exception exception)
    {
        var inner = exception is DbUpdateException dbUpdateException
            ? dbUpdateException.InnerException
            : exception;

        return inner is SqlException { Number: 547 };
    }

    /// <summary>Deadlock (1205) — request nên được thử lại.</summary>
    public static bool IsDeadlock(Exception exception)
    {
        var inner = exception is DbUpdateException dbUpdateException
            ? dbUpdateException.InnerException
            : exception;

        return inner is SqlException { Number: 1205 };
    }
}
