namespace LanguageCenter.WinForms;

static class Program
{
    ///  The main entry point for the application.
    [STAThread]
    static void Main()
    {
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