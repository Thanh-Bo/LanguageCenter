using LanguageCenter.Application.Accounts;
using LanguageCenter.Application.Attendance;
using LanguageCenter.Application.Auth;
using LanguageCenter.Application.Classes;
using LanguageCenter.Application.Courses;
using LanguageCenter.Application.Enrollments;
using LanguageCenter.Application.Grades;
using LanguageCenter.Application.GradeTypes;
using LanguageCenter.Application.Levels;
using LanguageCenter.Application.Notifications;
using LanguageCenter.Application.Payments;
using LanguageCenter.Application.Programs;
using LanguageCenter.Application.Promotions;
using LanguageCenter.Application.Receipts;
using LanguageCenter.Application.Reports;
using LanguageCenter.Application.Rooms;
using LanguageCenter.Application.Schedules;
using LanguageCenter.Application.Sessions;
using LanguageCenter.Application.Students;
using LanguageCenter.Application.Surveys;
using LanguageCenter.Application.Teachers;
using LanguageCenter.Application.TeachingAssignments;
using LanguageCenter.Application.TuitionFees;
using Microsoft.Extensions.DependencyInjection;

namespace LanguageCenter.Application;

/// <summary>
/// Đăng ký các service nghiệp vụ thuộc tầng Application.
/// Api chỉ cần gọi <c>builder.Services.AddApplication()</c>.
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // Tất cả service đều scoped vì phụ thuộc các store dùng DbContext (scoped).
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IAccountAdminService, AccountAdminService>();
        services.AddScoped<IStudentService, StudentService>();
        services.AddScoped<ITeacherService, TeacherService>();

        // ---------- Phase 4: quản lý học vụ ----------
        services.AddScoped<IProgramService, ProgramService>();
        services.AddScoped<ILevelService, LevelService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IPromotionService, PromotionService>();
        services.AddScoped<IClassService, ClassService>();
        services.AddScoped<ITeachingAssignmentService, TeachingAssignmentService>();
        services.AddScoped<IEnrollmentService, EnrollmentService>();

        // ---------- Phase 5: lịch học, điểm danh và điểm số ----------
        services.AddScoped<IRoomService, RoomService>();
        services.AddScoped<IScheduleService, ScheduleService>();
        services.AddScoped<ISessionService, SessionService>();
        services.AddScoped<IAttendanceService, AttendanceService>();
        services.AddScoped<IGradeTypeService, GradeTypeService>();
        services.AddScoped<IGradeService, GradeService>();

        // ---------- Phase 6: học phí, thanh toán và biên nhận ----------
        services.AddScoped<ITuitionFeeService, TuitionFeeService>();
        services.AddScoped<IPaymentService, PaymentService>();
        services.AddScoped<IReceiptService, ReceiptService>();

        // ---------- Phase 7: thông báo, khảo sát và báo cáo ----------
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<ISurveyService, SurveyService>();
        services.AddScoped<IReportService, ReportService>();

        return services;
    }
}
