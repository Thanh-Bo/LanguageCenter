using LanguageCenter.WinForms.Api;

namespace LanguageCenter.WinForms.UI;

/// <summary>
/// Diễn giải LỊCH HỌC của một lớp thành chuỗi chỉ để hiển thị.
/// <para>
/// Lịch học KHÔNG phải dữ liệu người dùng nhập — nó suy ra từ lớp đã chọn
/// (<c>ThuBuoi1 / ThuBuoi2 / GioBatDau / GioKetThuc</c> của
/// <see cref="ClassListItemDto"/>) nên dùng chung một chỗ để mọi form hiển thị giống nhau.
/// </para>
/// <para>Ví dụ: <c>Thứ 2 &amp; Thứ 4 · 18:00 – 19:30</c>.</para>
/// </summary>
public static class LichHocText
{
    /// <summary>Chưa chọn lớp → chưa thể biết lịch.</summary>
    public const string ChuaXepLop = "— Chưa xếp lớp, lịch sẽ có sau —";

    /// <summary>Có lớp nhưng lớp chưa khai báo thứ / giờ học.</summary>
    public const string LopChuaCoLich = "Lớp chưa có lịch cố định";

    /// <summary>Tính lịch học từ lớp; trả về <see cref="ChuaXepLop"/> khi chưa có lớp.</summary>
    public static string TuLop(ClassListItemDto? lop)
    {
        if (lop is null)
        {
            return ChuaXepLop;
        }

        var thu = new[] { lop.ThuBuoi1, lop.ThuBuoi2 }
            .Select(TenThu)
            .Where(x => x is not null)
            .ToList();

        var gio = lop.GioBatDau is { } batDau && lop.GioKetThuc is { } ketThuc
            ? $"{batDau:HH\\:mm} – {ketThuc:HH\\:mm}"
            : null;

        if (thu.Count == 0 && gio is null)
        {
            return LopChuaCoLich;
        }

        var phan = new List<string>();

        if (thu.Count > 0)
        {
            phan.Add(string.Join(" & ", thu));
        }

        if (gio is not null)
        {
            phan.Add(gio);
        }

        return string.Join(" · ", phan);
    }

    /// <summary>1 → "Thứ 2" … 7 → "Chủ nhật"; null nếu chưa khai báo.</summary>
    public static string? TenThu(byte? thu) => thu switch
    {
        1 => "Thứ 2",
        2 => "Thứ 3",
        3 => "Thứ 4",
        4 => "Thứ 5",
        5 => "Thứ 6",
        6 => "Thứ 7",
        7 => "Chủ nhật",
        _ => null
    };
}
