using LanguageCenter.Application.AcademicSetup.Models;
using LanguageCenter.Application.Common;

namespace LanguageCenter.Application.AcademicSetup;

/// <summary>Cấu hình học vụ: nội dung 20 buổi của khóa học và ngày lễ của trung tâm.</summary>
public interface IAcademicSetupService
{
    Task<OperationResult<IReadOnlyList<LessonContentDto>>> ListLessonContentsAsync(
        int maKhoaHoc,
        CancellationToken cancellationToken);

    Task<OperationResult<IReadOnlyList<LessonContentDto>>> SaveLessonContentsAsync(
        int maKhoaHoc,
        SaveLessonContentsRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<IReadOnlyList<HolidayDto>>> ListHolidaysAsync(CancellationToken cancellationToken);

    Task<OperationResult<HolidayDto>> CreateHolidayAsync(
        CreateHolidayRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<HolidayDto>> UpdateHolidayAsync(
        int maNgayLe,
        UpdateHolidayRequest request,
        CancellationToken cancellationToken);

    Task<OperationResult<HolidayDto>> DeleteHolidayAsync(int maNgayLe, CancellationToken cancellationToken);
}
