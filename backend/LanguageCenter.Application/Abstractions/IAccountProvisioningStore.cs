namespace LanguageCenter.Application.Abstractions;

/// <summary>Dữ liệu phụ huynh khi Admin tạo học viên kèm phụ huynh (OPTIONAL).</summary>
public sealed record ParentProvisionData(
    string HoTen,
    string? SoDienThoai,
    string? Email,
    string? DiaChi,
    string? NgheNghiep,
    string MoiQuanHe);

/// <summary>
/// Dữ liệu đã được validate ở Application, sẵn sàng ghi xuống database.
/// KHÔNG chứa: MaHocVien, TenDangNhap, MaVaiTro, MatKhauHash, TrangThaiTaiKhoan —
/// tất cả do backend tự quyết định.
/// </summary>
public sealed record StudentProvisionData(
    string VaiTro,
    string HoTen,
    DateOnly NgaySinh,
    string? GioiTinh,
    string Email,
    string? SoDienThoai,
    string? DiaChi,
    string? MucTieuHocTap,
    string? TrinhDoHienTai,
    string TrangThaiHocTap,
    ParentProvisionData? PhuHuynh);

/// <summary>Dữ liệu giáo viên/trợ giảng đã validate, sẵn sàng ghi xuống database.</summary>
public sealed record TeacherProvisionData(
    string VaiTro,
    string HoTen,
    DateOnly? NgaySinh,
    string? GioiTinh,
    string Email,
    string? SoDienThoai,
    string? DiaChi,
    string LoaiGiaoVien,
    string? ChuyenMon,
    int? SoNamKinhNghiem,
    DateOnly? NgayVaoLam,
    string TrangThai);

/// <summary>Lý do tạo tài khoản thất bại.</summary>
public enum ProvisionFailureReason
{
    None = 0,

    /// <summary>Email đã tồn tại ở tài khoản khác.</summary>
    EmailAlreadyExists = 1,

    /// <summary>Không tìm thấy vai trò trong bảng VaiTro.</summary>
    RoleNotFound = 2,

    /// <summary>Không sinh được mã duy nhất sau nhiều lần thử.</summary>
    CodeGenerationFailed = 3
}

/// <summary>Kết quả tạo tài khoản + hồ sơ.</summary>
public sealed record ProvisionResult(bool Succeeded, string? Code, ProvisionFailureReason FailureReason)
{
    public static ProvisionResult Success(string code) => new(true, code, ProvisionFailureReason.None);

    public static ProvisionResult Failure(ProvisionFailureReason reason) => new(false, null, reason);
}

/// <summary>
/// Cổng ghi dữ liệu để tạo tài khoản kèm hồ sơ.
/// <para>
/// Triển khai ở Infrastructure PHẢI đảm bảo:
/// <list type="bullet">
///   <item>TaiKhoan + hồ sơ (+ PhuHuynh) nằm trong MỘT transaction — rollback toàn bộ nếu lỗi.</item>
///   <item><c>TaiKhoan.TenDangNhap</c> == mã hồ sơ (<c>HocVien.MaHocVien</c> / <c>GiaoVien.MaGiaoVien</c>).</item>
///   <item>Sinh mã concurrency-safe (không dùng <c>Count() + 1</c>).</item>
/// </list>
/// </para>
/// </summary>
public interface IAccountProvisioningStore
{
    Task<ProvisionResult> CreateStudentAsync(StudentProvisionData data, string matKhauHash, CancellationToken cancellationToken);

    Task<ProvisionResult> CreateTeacherAsync(TeacherProvisionData data, string matKhauHash, CancellationToken cancellationToken);
}
