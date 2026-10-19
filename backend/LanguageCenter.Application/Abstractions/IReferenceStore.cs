namespace LanguageCenter.Application.Abstractions;

/// <summary>
/// Kiểm tra tồn tại của các bản ghi THAM CHIẾU nằm ngoài phạm vi Phase 4
/// (<c>NhanVien</c>, <c>GiaoVien</c>, <c>HocVien</c>).
/// <para>
/// Tách riêng thành một cổng nhỏ để service Phase 4 không phải phụ thuộc vào store
/// của module khác, và không phải sửa code Phase 3.
/// </para>
/// </summary>
public interface IReferenceStore
{
    /// <summary>Nhân viên (bảng <c>NhanVien</c>) có tồn tại không.</summary>
    Task<bool> StaffExistsAsync(string maNhanVien, CancellationToken cancellationToken);

    /// <summary>Giáo viên / trợ giảng (bảng <c>GiaoVien</c>) có tồn tại không.</summary>
    Task<bool> TeacherExistsAsync(string maGiaoVien, CancellationToken cancellationToken);

    /// <summary>Trả "Giáo viên" hoặc "Trợ giảng"; <c>null</c> nếu không tồn tại.</summary>
    Task<string?> GetTeacherTypeAsync(string maGiaoVien, CancellationToken cancellationToken);

    /// <summary>Học viên (bảng <c>HocVien</c>) có tồn tại không.</summary>
    Task<bool> StudentExistsAsync(string maHocVien, CancellationToken cancellationToken);

    /// <summary>Trả <c>TrangThaiHocTap</c> của học viên; <c>null</c> nếu không tồn tại.</summary>
    Task<string?> GetStudentStatusAsync(string maHocVien, CancellationToken cancellationToken);
}
