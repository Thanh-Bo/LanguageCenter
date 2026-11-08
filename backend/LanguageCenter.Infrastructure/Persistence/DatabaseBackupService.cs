using System.Data;
using LanguageCenter.Domain.Constants;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace LanguageCenter.Infrastructure.Persistence;

/// <summary>
/// SAO LƯU / PHỤC HỒI cơ sở dữ liệu SQL Server bằng <c>BACKUP DATABASE</c> /
/// <c>RESTORE DATABASE</c>.
/// <para>
/// Khác với mọi store khác, lớp này kết nối vào database <c>master</c> vì
/// <c>RESTORE</c> không thể chạy khi chính database đích đang được dùng.
/// </para>
/// <para>
/// ⚠ PHỤC HỒI là thao tác PHÁ HUỶ: dữ liệu hiện tại bị thay thế hoàn toàn và
/// API sẽ mất kết nối trong vài giây. Chỉ Quản trị viên mới được gọi.
/// </para>
/// </summary>
public interface IDatabaseBackupService
{
    /// <summary>Tên database đang dùng (lấy từ connection string).</summary>
    string TenDatabase { get; }

    /// <summary>Thư mục mặc định chứa file .bak.</summary>
    string ThuMucMacDinh { get; }

    Task<string> SaoLuuAsync(string? thuMuc, CancellationToken cancellationToken);

    Task<IReadOnlyList<FileBackupInfo>> DanhSachAsync(string? thuMuc, CancellationToken cancellationToken);

    Task PhucHoiAsync(string duongDanFile, CancellationToken cancellationToken);
}

/// <summary>Thông tin một file sao lưu.</summary>
public sealed record FileBackupInfo(string TenFile, string DuongDan, long KichThuocByte, DateTime ThoiDiem);

/// <inheritdoc cref="IDatabaseBackupService"/>
public sealed class DatabaseBackupService : IDatabaseBackupService
{
    private readonly string _connectionString;

    public DatabaseBackupService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException(
                "Thiếu ConnectionStrings:DefaultConnection — không thể sao lưu / phục hồi.");
    }

    public string TenDatabase
    {
        get
        {
            var builder = new SqlConnectionStringBuilder(_connectionString);

            if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
            {
                throw new InvalidOperationException("Connection string không có Database.");
            }

            return builder.InitialCatalog;
        }
    }

    public string ThuMucMacDinh
    {
        get
        {
            // Thư mục "Backups" cạnh thư mục chạy ứng dụng.
            var thuMuc = Path.Combine(AppContext.BaseDirectory, "Backups");
            Directory.CreateDirectory(thuMuc);
            return thuMuc;
        }
    }

    public async Task<string> SaoLuuAsync(string? thuMuc, CancellationToken cancellationToken)
    {
        var folder = ChuanHoaThuMuc(thuMuc);
        Directory.CreateDirectory(folder);

        var tenFile = $"{TenDatabase}_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
        var duongDan = Path.Combine(folder, tenFile);

        await using var connection = new SqlConnection(BuildMasterConnectionString());
        await connection.OpenAsync(cancellationToken);

        // SQL Server EXPRESS KHÔNG hỗ trợ WITH COMPRESSION → phải dò phiên bản trước,
        // nếu không sẽ nhận lỗi:
        //   "BACKUP DATABASE WITH COMPRESSION is not supported on Express Edition".
        var hoTroNen = await HoTroNenAsync(connection, cancellationToken);

        var tuyChon = hoTroNen
            ? "INIT, FORMAT, COMPRESSION, CHECKSUM, STATS = 10"
            : "INIT, FORMAT, CHECKSUM, STATS = 10";

        // Đường dẫn phải nhìn từ phía SQL Server. Với SQL Server chạy cùng máy
        // (localhost\SQLEXPRESS) đường dẫn tuyệt đối của máy là hợp lệ.
        var sql = $"""
            BACKUP DATABASE [{TenDatabase}]
            TO DISK = @duongDan
            WITH {tuyChon};
            """;

        await using var command = new SqlCommand(sql, connection)
        {
            CommandTimeout = 600
        };

        command.Parameters.Add(new SqlParameter("@duongDan", SqlDbType.NVarChar, 4000) { Value = duongDan });

        await command.ExecuteNonQueryAsync(cancellationToken);

        return duongDan;
    }

    public Task<IReadOnlyList<FileBackupInfo>> DanhSachAsync(string? thuMuc, CancellationToken cancellationToken)
    {
        var folder = ChuanHoaThuMuc(thuMuc);

        if (!Directory.Exists(folder))
        {
            return Task.FromResult<IReadOnlyList<FileBackupInfo>>([]);
        }

        var danhSach = new DirectoryInfo(folder)
            .GetFiles("*.bak")
            .OrderByDescending(f => f.LastWriteTime)
            .Select(f => new FileBackupInfo(f.Name, f.FullName, f.Length, f.LastWriteTime))
            .ToList();

        return Task.FromResult<IReadOnlyList<FileBackupInfo>>(danhSach);
    }

    public async Task PhucHoiAsync(string duongDanFile, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(duongDanFile) || !File.Exists(duongDanFile))
        {
            throw new FileNotFoundException($"Không tìm thấy file sao lưu: {duongDanFile}", duongDanFile);
        }

        await using var connection = new SqlConnection(BuildMasterConnectionString());
        await connection.OpenAsync(cancellationToken);

        // Chuyển sang SINGLE_USER để đá mọi kết nối đang mở (kể cả chính API).
        var sql = $"""
            ALTER DATABASE [{TenDatabase}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
            RESTORE DATABASE [{TenDatabase}]
            FROM DISK = @duongDan
            WITH REPLACE, RECOVERY, STATS = 10;
            ALTER DATABASE [{TenDatabase}] SET MULTI_USER;
            """;

        await using var command = new SqlCommand(sql, connection)
        {
            CommandTimeout = 900
        };

        command.Parameters.Add(new SqlParameter("@duongDan", SqlDbType.NVarChar, 4000) { Value = duongDanFile });

        try
        {
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
        finally
        {
            // Bảo đảm database luôn quay lại MULTI_USER dù RESTORE lỗi.
            try
            {
                await using var reset = new SqlCommand(
                    $"ALTER DATABASE [{TenDatabase}] SET MULTI_USER;", connection);
                await reset.ExecuteNonQueryAsync(CancellationToken.None);
            }
            catch
            {
                // bỏ qua — database có thể đang ở trạng thái restore
            }
        }
    }

    private string BuildMasterConnectionString()
    {
        var builder = new SqlConnectionStringBuilder(_connectionString)
        {
            InitialCatalog = "master"
        };

        // BACKUP/RESTORE không dùng MARS.
        builder.MultipleActiveResultSets = false;

        return builder.ConnectionString;
    }

    /// <summary>
    /// Cho biết SQL Server đang chạy có hỗ trợ <c>WITH COMPRESSION</c> hay không.
    /// <para>
    /// Express / LocalDB / Developer-with-limits: KHÔNG hỗ trợ nén ở bản Express.
    /// Dùng <c>SERVERPROPERTY('Edition')</c> vì đây là thứ duy nhất phân biệt được
    /// Express với Standard/Enterprise mà không cần quyền cao.
    /// </para>
    /// </summary>
    private static async Task<bool> HoTroNenAsync(
        SqlConnection connection,
        CancellationToken cancellationToken)
    {
        await using var command = new SqlCommand(
            "SELECT CAST(SERVERPROPERTY('Edition') AS nvarchar(128));",
            connection);

        var edition = await command.ExecuteScalarAsync(cancellationToken) as string;

        if (string.IsNullOrWhiteSpace(edition))
        {
            // Không đọc được → chọn phương án an toàn (không nén).
            return false;
        }

        return !edition.Contains("Express", StringComparison.OrdinalIgnoreCase);
    }

    private static string ChuanHoaThuMuc(string? thuMuc)
    {
        if (string.IsNullOrWhiteSpace(thuMuc))
        {
            var mac = Path.Combine(AppContext.BaseDirectory, "Backups");
            Directory.CreateDirectory(mac);
            return mac;
        }

        var duongDan = Path.GetFullPath(thuMuc.Trim());
        Directory.CreateDirectory(duongDan);
        return duongDan;
    }
}
