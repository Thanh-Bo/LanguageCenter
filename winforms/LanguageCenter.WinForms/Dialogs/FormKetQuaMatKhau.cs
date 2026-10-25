namespace LanguageCenter.WinForms.Dialogs;

/// Hộp thoại hiển thị thông tin chỉ-xuất-hiện-một-lần (mật khẩu tạm khi tạo tài khoản / reset mật khẩu).
public partial class FormKetQuaMatKhau : Form
{
    public FormKetQuaMatKhau() : this("Mật khẩu tài khoản", "Thông tin bảo mật", "USER001", "123456")
    {
    }

    public FormKetQuaMatKhau(string title, string message, string username, string temporaryPassword)
    {
        InitializeComponent();

        lblTitle.Text = title;
        lblMessage.Text = message;
        txtUsername.Text = username;
        txtPassword.Text = temporaryPassword;
        btnClose.Click += (_, _) => Close();
    }

    private void BtnCopy_Click(object? sender, EventArgs e)
    {
        try
        {
            Clipboard.SetText($"Tên đăng nhập: {txtUsername.Text}\nMật khẩu: {txtPassword.Text}");
            MessageBox.Show(this, "Đã sao chép vào clipboard.", "Sao chép", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Không thể sao chép: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
