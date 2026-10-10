namespace LanguageCenter.WinForms.Dialogs;

/// <summary>Hộp thoại hiển thị thông tin chỉ-xuất-hiện-một-lần (mật khẩu tạm khi tạo tài khoản / reset mật khẩu).</summary>
public partial class FormSecretResult : Form
{
    public FormSecretResult(string title, string message, string username, string temporaryPassword)
    {
        InitializeComponent();

        lblTitle.Text = title;
        lblMessage.Text = message;
        txtUsername.Text = username;
        txtPassword.Text = temporaryPassword;
    }

    private void BtnCopy_Click(object? sender, EventArgs e)
    {
        try
        {
            Clipboard.SetText($"Tên đăng nhập: {txtUsername.Text}\nMật khẩu tạm: {txtPassword.Text}");
            MessageBox.Show(this, "Đã sao chép vào clipboard.", "Sao chép", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, $"Không thể sao chép: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
