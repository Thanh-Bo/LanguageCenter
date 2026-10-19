using LanguageCenter.Application.Common;
using LanguageCenter.Application.GradeTypes.Models;
using LanguageCenter.Application.Grades.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>Cổng truy cập điểm số (<c>DiemSo</c>).</summary>
public interface IGradeStore
{
    Task<PagedResult<GradeListItemDto>> ListAsync(
        GradeListQuery query,
        CancellationToken cancellationToken);

    Task<GradeDetailDto?> GetDetailAsync(int maDiemSo, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int maDiemSo, CancellationToken cancellationToken);

    /// <summary>Đã có điểm cho cặp (đăng ký, đầu điểm) chưa — UNIQUE (MaDangKy, MaLoaiDiem).</summary>
    Task<bool> DuplicateAsync(
        int maDangKy,
        int maLoaiDiem,
        int? exceptId,
        CancellationToken cancellationToken);

    Task<WriteResult> CreateAsync(
        CreateGradeRequest request,
        string maGiaoVien,
        CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateAsync(
        int maDiemSo,
        UpdateGradeRequest changes,
        CancellationToken cancellationToken);

    Task<WriteOutcome> DeleteAsync(int maDiemSo, CancellationToken cancellationToken);

    /// <summary>Dữ liệu thô để tính điểm tổng kết cho cả lớp (null nếu lớp không tồn tại).</summary>
    Task<ClassGradeData?> GetClassGradeDataAsync(int maLopHoc, CancellationToken cancellationToken);

    /// <summary>Dữ liệu thô để tính điểm tổng kết của một học viên (null nếu không tồn tại).</summary>
    Task<StudentGradeData?> GetStudentGradeDataAsync(
        string maHocVien,
        CancellationToken cancellationToken);
}

/// <summary>Dữ liệu thô điểm số của một lớp.</summary>
public sealed record ClassGradeData(
    int MaLopHoc,
    string MaLop,
    string TenLop,
    int MaKhoaHoc,
    string TenKhoaHoc,
    IReadOnlyList<GradeComponentInfo> Components,
    IReadOnlyList<StudentGradeRow> Students);

/// <summary>Điểm của một học viên theo từng đầu điểm (khoá = <c>MaLoaiDiem</c>).</summary>
public sealed record StudentGradeRow(
    int MaDangKy,
    string MaHocVien,
    string HoTen,
    IReadOnlyDictionary<int, decimal> Scores);

/// <summary>Dữ liệu thô điểm số của một học viên, nhóm theo lớp.</summary>
public sealed record StudentGradeData(
    string MaHocVien,
    string HoTen,
    IReadOnlyList<StudentGradeGroup> Groups);

/// <summary>Điểm của học viên trong một lớp.</summary>
public sealed record StudentGradeGroup(
    int MaDangKy,
    int MaLopHoc,
    string MaLop,
    string TenLop,
    int MaKhoaHoc,
    string TenKhoaHoc,
    DateOnly NgayBatDau,
    DateOnly NgayKetThucDuKien,
    DateTime? NgayXepLop,
    IReadOnlyList<GradeComponentInfo> Components,
    IReadOnlyDictionary<int, decimal> Scores);
