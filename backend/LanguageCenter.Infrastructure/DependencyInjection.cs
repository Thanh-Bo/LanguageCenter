using LanguageCenter.Application.Abstractions;
using LanguageCenter.Infrastructure.Authentication;
using LanguageCenter.Infrastructure.Data;
using LanguageCenter.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LanguageCenter.Infrastructure;

/// <summary>
/// Đăng ký toàn bộ dependency thuộc tầng Infrastructure.
/// Tầng Api chỉ cần gọi <c>builder.Services.AddInfrastructure(builder.Configuration)</c>.
/// </summary>
public static class DependencyInjection
{
    /// <summary>Tên connection string trong appsettings / User Secrets.</summary>
    public const string ConnectionStringName = "DefaultConnection";

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        AddDatabase(services, configuration);
        AddAuthenticationServices(services, configuration);
        AddPersistenceServices(services);

        return services;
    }

    /// <summary>Phase 3 — các store đọc/ghi dữ liệu cho quản trị tài khoản và hồ sơ.</summary>
    private static void AddPersistenceServices(IServiceCollection services)
    {
        // Tất cả store đều scoped vì dùng AppDbContext (scoped).
        services.AddScoped<IAccountProvisioningStore, AccountProvisioningStore>();
        services.AddScoped<IStudentStore, StudentStore>();
        services.AddScoped<ITeacherStore, TeacherStore>();

        // ---------- Phase 4: quản lý học vụ ----------
        services.AddScoped<IReferenceStore, ReferenceStore>();
        services.AddScoped<IProgramStore, ProgramStore>();
        services.AddScoped<ILevelStore, LevelStore>();
        services.AddScoped<ICourseStore, CourseStore>();
        services.AddScoped<IPromotionStore, PromotionStore>();
        services.AddScoped<IClassStore, ClassStore>();
        services.AddScoped<ITeachingAssignmentStore, TeachingAssignmentStore>();
        services.AddScoped<IEnrollmentStore, EnrollmentStore>();

        // ---------- Phase 5: lịch học, điểm danh và điểm số ----------
        services.AddScoped<IAcademicLookupStore, AcademicLookupStore>();
        services.AddScoped<IRoomStore, RoomStore>();
        services.AddScoped<IScheduleStore, ScheduleStore>();
        services.AddScoped<ISessionStore, SessionStore>();
        services.AddScoped<IAttendanceStore, AttendanceStore>();
        services.AddScoped<IGradeTypeStore, GradeTypeStore>();
        services.AddScoped<IGradeStore, GradeStore>();

        // ---------- Phase 6: học phí, thanh toán và biên nhận ----------
        services.AddScoped<ITuitionFeeStore, TuitionFeeStore>();
        services.AddScoped<IPaymentStore, PaymentStore>();
        services.AddScoped<IReceiptStore, ReceiptStore>();

        // ---------- Phase 7: thông báo, khảo sát và báo cáo ----------
        services.AddScoped<INotificationStore, NotificationStore>();
        services.AddScoped<ISurveyStore, SurveyStore>();
        services.AddScoped<IReportStore, ReportStore>();

        // Sinh mật khẩu tạm là logic thuần, không giữ state -> singleton.
        services.AddSingleton<ITemporaryPasswordGenerator, TemporaryPasswordGenerator>();
    }

    private static void AddDatabase(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(ConnectionStringName);

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                $"Không tìm thấy connection string '{ConnectionStringName}'. " +
                "Hãy cấu hình trong appsettings.Development.json hoặc User Secrets.");
        }

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlServerOptions =>
            {
                sqlServerOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(5),
                    errorNumbersToAdd: null);

                sqlServerOptions.CommandTimeout(30);
            });
        });
    }

    private static void AddAuthenticationServices(IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = JwtSettings.FromConfiguration(configuration);

        // Fail fast: KHÔNG khởi động app với signing key thiếu / yếu / sai định dạng.
        jwtSettings.Validate();

        services.AddSingleton(jwtSettings);

        services.AddSingleton<IPasswordHasherService, PasswordHasherService>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        // AccountStore dùng AppDbContext nên phải scoped.
        services.AddScoped<IAccountStore, AccountStore>();
    }
}
