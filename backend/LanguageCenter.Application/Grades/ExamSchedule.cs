using LanguageCenter.Domain.Constants;

namespace LanguageCenter.Application.Grades;

/// <summary>Buổi kiểm tra / buổi thi của một lớp (đủ dữ liệu để xác định ngày thi của đầu điểm).</summary>
public sealed record ExamSessionInfo(int MaCaHoc, int? SoBuoi, DateOnly NgayHoc, string LoaiBuoi);

/// <summary>
/// Ghép một ĐẦU ĐIỂM với buổi kiểm tra / buổi thi tương ứng để biết NGÀY THI của đầu điểm đó.
/// <para>
/// Quy ước đặt tên đầu điểm (bảng <c>LoaiDiem</c>):
/// "Giữa kỳ 1", "Giữa kỳ 2" → buổi <c>Kiểm tra</c> thứ 1, thứ 2 (sắp theo ngày học);
/// "Cuối kỳ 1", "Cuối kỳ 2" → buổi <c>Buổi thi</c> thứ 1, thứ 2.
/// Các đầu điểm khác (Chuyên cần, Bài tập…) không có ngày thi.
/// </para>
/// </summary>
public static class ExamSchedule
{
    /// <summary>Loại buổi tương ứng với đầu điểm; <c>null</c> nếu đầu điểm không phải bài thi.</summary>
    public static string? ExamKindOf(string? tenLoaiDiem)
    {
        var ten = (tenLoaiDiem ?? string.Empty).Trim().ToLowerInvariant();

        if (ten.Contains("giữa kỳ", StringComparison.Ordinal)
            || ten.Contains("giữa kì", StringComparison.Ordinal))
        {
            return SessionKinds.KiemTra;
        }

        if (ten.Contains("cuối kỳ", StringComparison.Ordinal)
            || ten.Contains("cuối kì", StringComparison.Ordinal))
        {
            return SessionKinds.BuoiThi;
        }

        return null;
    }

    /// <summary>Số thứ tự của bài trong cùng loại ("Giữa kỳ 2" → 2); mặc định 1.</summary>
    public static int AttemptOf(string? tenLoaiDiem)
    {
        var ten = (tenLoaiDiem ?? string.Empty).Trim();

        var batDau = ten.Length;

        while (batDau > 0 && char.IsDigit(ten[batDau - 1]))
        {
            batDau--;
        }

        return batDau < ten.Length
               && int.TryParse(ten[batDau..], out var so)
               && so > 0
            ? so
            : 1;
    }

    /// <summary>
    /// Ngày thi của đầu điểm; <c>null</c> khi đầu điểm không phải bài thi
    /// hoặc lớp chưa sinh buổi kiểm tra / thi tương ứng.
    /// </summary>
    public static DateOnly? ResolveExamDate(
        string? tenLoaiDiem,
        IReadOnlyList<ExamSessionInfo>? sessions)
    {
        var loai = ExamKindOf(tenLoaiDiem);

        if (loai is null || sessions is null || sessions.Count == 0)
        {
            return null;
        }

        var dungLoai = sessions
            .Where(x => string.Equals(x.LoaiBuoi?.Trim(), loai, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.NgayHoc)
            .ThenBy(x => x.SoBuoi ?? int.MaxValue)
            .ThenBy(x => x.MaCaHoc)
            .ToList();

        var lan = AttemptOf(tenLoaiDiem);

        return dungLoai.Count >= lan ? dungLoai[lan - 1].NgayHoc : null;
    }

    /// <summary>Đã tới ngày thi chưa → có được phép nhập điểm hay không.</summary>
    public static bool CanEnterScore(
        string? tenLoaiDiem,
        IReadOnlyList<ExamSessionInfo>? sessions,
        DateOnly homNay)
        => ResolveExamDate(tenLoaiDiem, sessions) is not { } ngay || ngay <= homNay;
}
