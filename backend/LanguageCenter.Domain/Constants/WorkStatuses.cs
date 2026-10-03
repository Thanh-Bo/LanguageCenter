namespace LanguageCenter.Domain.Constants;

/// <summary>
/// Trạng thái làm việc của giáo viên (<c>GiaoVien.TrangThai</c>) và nhân viên
/// (<c>NhanVien.TrangThaiLamViec</c>).
/// <para>
/// ⚠️ Hai cột này <b>KHÔNG có CHECK CONSTRAINT</b>. Giá trị <see cref="DangLamViec"/>
/// là giá trị THẬT đang tồn tại trong dữ liệu. Danh sách dưới đây là tập hợp được phép
/// dùng trong code để không nhận giá trị rác từ client.
/// </para>
/// </summary>
public static class WorkStatuses
{
    public const string DangLamViec = "Đang làm việc";
    public const string DaNghiViec = "Đã nghỉ việc";

    public static readonly IReadOnlyList<string> All = [DangLamViec, DaNghiViec];

    public static bool IsValid(string? status)
        => status is not null && All.Contains(status);
}
