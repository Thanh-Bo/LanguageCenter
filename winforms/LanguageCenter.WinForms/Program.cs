namespace LanguageCenter.WinForms;

static class Program
{
    ///  The main entry point for the application.
    [STAThread]
    static void Main()
    {
        // Đặt culture vi-VN TRƯỚC khi tạo control nào.
        // Nếu không, DateTimePicker (Format = Short) sẽ lấy locale của Windows
        // (máy cài en-US sẽ hiển thị MM/dd/yyyy → "ngày bị ngược").
        var viVn = new System.Globalization.CultureInfo("vi-VN");
        System.Globalization.CultureInfo.DefaultThreadCurrentCulture = viVn;
        System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = viVn;
        Thread.CurrentThread.CurrentCulture = viVn;
        Thread.CurrentThread.CurrentUICulture = viVn;
        Application.CurrentCulture = viVn;

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
        Application.ThreadException += (s, e) =>
        {
            try { File.AppendAllText("app_error.log", $"[{DateTime.Now}] ThreadException: {e.Exception}\n"); } catch { }
            MessageBox.Show($"Lỗi khởi chạy ứng dụng: {e.Exception.Message}\n\n{e.Exception.StackTrace}", "Lỗi HUIT LingoHub", MessageBoxButtons.OK, MessageBoxIcon.Error);
        };
        AppDomain.CurrentDomain.UnhandledException += (s, e) =>
        {
            try { File.AppendAllText("app_error.log", $"[{DateTime.Now}] UnhandledException: {e.ExceptionObject}\n"); } catch { }
        };

        Application.Run(new FormDangNhap());
    }    
}