namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Trạng thái học tập của học viên (<c>HocVien.TrangThaiHocTap</c>).
/// <para>
/// ⚠️ Cột này <b>KHÔNG có CHECK CONSTRAINT</b> trong database. Hai giá trị
/// <see cref="DangTuVan"/> và <see cref="DangHoc"/> là giá trị THẬT đang tồn tại trong dữ liệu
/// (đã kiểm tra bằng query <c>GROUP BY TrangThaiHocTap</c>). Danh sách dưới đây là tập hợp
/// được phép dùng trong code để không nhận giá trị rác từ client.
/// </para>
/// </summary>
public static class StudentStatuses
{
    public const string DangTuVan = "Đang tư vấn";
    public const string DangHoc = "Đang học";
    public const string BaoLuu = "Bảo lưu";
    public const string DaNghi = "Đã nghỉ";

    public static readonly IReadOnlyList<string> All = [DangTuVan, DangHoc, BaoLuu, DaNghi];

    public static bool IsValid(string? status)
        => status is not null && All.Contains(status);
}
