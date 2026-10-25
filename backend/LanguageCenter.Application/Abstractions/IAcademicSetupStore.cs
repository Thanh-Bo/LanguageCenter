using LanguageCenter.Application.AcademicSetup.Models;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.Abstractions;

/// <summary>
/// Truy cập 2 bảng cấu hình học vụ: <c>NoiDungBaiHoc</c> (20 buổi của khóa)
/// và <c>NgayLe</c> (ngày lễ dùng khi sinh ca học).
/// </summary>
public interface IAcademicSetupStore
{
    // ---------- 20 buổi học ----------

    Task<IReadOnlyList<LessonContentDto>> ListLessonContentsAsync(
        int maKhoaHoc,
        CancellationToken cancellationToken);

    /// <summary>Ghi đè nội dung 20 buổi của khóa (thêm dòng còn thiếu, cập nhật dòng đã có).</summary>
    Task<int> SaveLessonContentsAsync(
        int maKhoaHoc,
        IReadOnlyList<LessonContentInput> items,
        CancellationToken cancellationToken);

    // ---------- Ngày lễ ----------

    Task<IReadOnlyList<HolidayDto>> ListHolidaysAsync(CancellationToken cancellationToken);

    Task<HolidayDto?> GetHolidayAsync(int maNgayLe, CancellationToken cancellationToken);

    Task<WriteResult> CreateHolidayAsync(
        CreateHolidayRequest request,
        CancellationToken cancellationToken);

    Task<WriteOutcome> UpdateHolidayAsync(
        int maNgayLe,
        UpdateHolidayRequest changes,
        CancellationToken cancellationToken);

    Task<WriteOutcome> DeleteHolidayAsync(int maNgayLe, CancellationToken cancellationToken);

    /// <summary>Ngày này đã có ngày lễ khác chưa (UNIQUE theo Ngay).</summary>
    Task<bool> HolidayDateExistsAsync(
        DateOnly ngay,
        int? exceptId,
        CancellationToken cancellationToken);
}
