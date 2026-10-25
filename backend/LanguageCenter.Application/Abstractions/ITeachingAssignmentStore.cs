using LanguageCenter.Application.Common;
using LanguageCenter.Application.TeachingAssignments.Models;

namespace LanguageCenter.Application.Abstractions;

/// <summary>Cổng truy cập dữ liệu phân công giảng dạy (<c>PhanCongGiangDay</c>).</summary>
public interface ITeachingAssignmentStore
{
    Task<PagedResult<AssignmentListItemDto>> ListAsync(
        AssignmentListQuery query,
        CancellationToken cancellationToken);

    Task<AssignmentDetailDto?> GetDetailAsync(int maPhanCong, CancellationToken cancellationToken);

    Task<bool> ExistsAsync(int maPhanCong, CancellationToken cancellationToken);

    /// <summary>
    /// Đã có phân công cùng lớp + cùng người + cùng vai trò với khoảng ngày CHỒNG LẤN chưa
    /// (bỏ qua phân công đã hủy).
    /// </summary>
    Task<bool> HasOverlapAsync(
        int maLopHoc,
        string maGiaoVien,
        string vaiTroPhanCong,
        DateOnly ngayBatDau,
        DateOnly? ngayKetThuc,
        int? exceptId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Cùng một người đã được phân công cho CÙNG lớp với vai trò khác trong khoảng ngày chồng lấn chưa
    /// (một người không thể vừa là giáo viên chính vừa là trợ giảng của cùng một lớp).
    /// </summary>
    Task<bool> HasSamePersonOverlapAsync(
        int maLopHoc,
        string maGiaoVien,
        DateOnly ngayBatDau,
        DateOnly? ngayKetThuc,
        int? exceptId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Tìm xung đột thời khóa biểu giữa lớp đang phân công và các lớp KHÁC mà người này đang dạy.
    /// So sánh trên <c>LichHoc</c> (thứ trong tuần + giờ + khoảng ngày áp dụng).
    /// </summary>
    Task<IReadOnlyList<TeachingScheduleConflict>> FindScheduleConflictsAsync(
        int maLopHoc,
        string maGiaoVien,
        DateOnly ngayBatDau,
        DateOnly? ngayKetThuc,
        int? exceptId,
        CancellationToken cancellationToken);

    /// <summary>
    /// Lớp đã có khung phân công TRỐNG (chưa gắn người) cho vai trò này chưa.
    /// </summary>
    Task<bool> PlaceholderExistsAsync(
        int maLopHoc,
        string vaiTroPhanCong,
        CancellationToken cancellationToken);

    Task<WriteResult> CreateAsync(CreateAssignmentRequest request, CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateAsync(
        int maPhanCong,
        UpdateAssignmentRequest changes,
        CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateStatusAsync(
        int maPhanCong,
        string trangThai,
        CancellationToken cancellationToken);
}

/// <summary>Một buổi bị trùng lịch giữa 2 lớp của cùng một giáo viên / trợ giảng.</summary>
public sealed record TeachingScheduleConflict(
    string MaLop,
    string TenLop,
    int ThuTrongTuan,
    TimeOnly GioBatDau,
    TimeOnly GioKetThuc)
{
    /// <summary>Thứ trong tuần dạng chữ (2 → "Thứ Hai").</summary>
    public string TenThu => ThuTrongTuan switch
    {
        1 => "Thứ Hai",
        2 => "Thứ Ba",
        3 => "Thứ Tư",
        4 => "Thứ Năm",
        5 => "Thứ Sáu",
        6 => "Thứ Bảy",
        7 => "Chủ Nhật",
        _ => $"Thứ {ThuTrongTuan}"
    };

    public override string ToString()
        => $"lớp {MaLop} ({TenThu} {GioBatDau:HH\\:mm}–{GioKetThuc:HH\\:mm})";
}
