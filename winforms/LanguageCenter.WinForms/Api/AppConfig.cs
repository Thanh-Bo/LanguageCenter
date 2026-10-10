namespace LanguageCenter.WinForms.Api;

/// <summary>
/// Cấu hình chung của ứng dụng WinForms.
/// </summary>
public static class AppConfig
{
    // ========================================================================
    // 1. TÊN THƯƠNG HIỆU & TIÊU ĐỀ
    // Khi nhóm chốt tên chính thức, bạn chỉ cần sửa ở 3 dòng dưới đây:
    // ========================================================================
    public const string BrandName = "TRUNG TÂM NGOẠI NGỮ";
    public const string BrandShortName = "LANGUAGE CENTER";
    public const string BrandSlogan = "Hệ thống Quản lý & Đào tạo Chuẩn Quốc tế";

    public const string ApplicationTitle = $"{BrandName} — Quản trị viên";

    // ========================================================================
    // 2. CHẾ ĐỘ DỮ LIỆU (MOCK DATA)
    // - true  : Chạy độc lập với dữ liệu mẫu trong bộ nhớ (KHÔNG cần bật backend hay SQL Server).
    // - false : Kết nối vào Backend API thật tại ApiBaseUrl.
    // ========================================================================
    public static bool UseMockData = true;

    public const string ApiBaseUrl = "http://localhost:5204/api/";
}
