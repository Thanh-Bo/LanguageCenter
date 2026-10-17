using LanguageCenter.WinForms.Api;

namespace LanguageCenter.WinForms;

public partial class FormDangNhap : Form
{
    public FormDangNhap()
    {
        InitializeComponent();
    }

    private void ChkShowPassword_CheckedChanged(object? sender, EventArgs e)
    {
        txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
    }

    private async void BtnLogin_Click(object? sender, EventArgs e)
    {
        var username = txtUsername.Text.Trim();
        var password = txtPassword.Text;

        lblError.Visible = false;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ShowError("Vui lòng nhập tên đăng nhập và mật khẩu.");
            return;
        }

        SetLoading(true);

        try
        {
            var result = await AppSession.Api.PostAsync<LoginResponse>(
                "auth/login",
                new LoginRequest { Username = username, Password = password });

            if (!result.Success || result.Data is null)
            {
                ShowError(result.StatusCode == 401
                    ? "Sai tên đăng nhập hoặc mật khẩu, hoặc tài khoản không ở trạng thái hoạt động."
                    : result.ErrorMessage);
                return;
            }

            var role = result.Data.User.Role;
            if (role != RoleNames.QuanTriVien && role != RoleNames.GiaoVien && role != RoleNames.TroGiang)
            {
                ShowError($"Tài khoản \"{result.Data.User.Username}\" có vai trò \"{role}\" — "
                    + "không có quyền truy cập vào phân hệ này.");
                return;
            }

            AppSession.SetLoggedIn(result.Data);

            Hide();
            using var formMain = new FormChinh();
            formMain.ShowDialog();

            // Khi FormChinh đóng (đăng xuất hoặc thoát) quay lại màn đăng nhập sạch.
            AppSession.Clear();
            txtPassword.Clear();
            lblError.Visible = false;
            Show();
            txtUsername.Focus();
        }
        finally
        {
            SetLoading(false);
        }
    }

    private void ShowError(string message)
    {
        lblError.Text = message;
        lblError.Visible = true;
    }

    private void SetLoading(bool loading)
    {
        btnLogin.Enabled = !loading;
        txtUsername.Enabled = !loading;
        txtPassword.Enabled = !loading;
        btnLogin.Text = loading ? "ĐANG ĐĂNG NHẬP..." : "ĐĂNG NHẬP";
        UseWaitCursor = loading;
    }

    private void LnkForgotPassword_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        using var dlg = new Form
        {
            Text = "Khôi phục mật khẩu — Admin",
            Width = 480,
            Height = 370,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            MaximizeBox = false,
            MinimizeBox = false,
            StartPosition = FormStartPosition.CenterParent,
            BackColor = Color.White
        };

        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 70,
            BackColor = UI.Theme.PrimaryLight,
            Padding = new Padding(20, 15, 20, 15)
        };

        var lblHeader = new Label
        {
            Text = "KHÔI PHỤC MẬT KHẨU QUẢN TRỊ",
            Font = UI.Theme.FontHeading,
            ForeColor = UI.Theme.PrimaryDark,
            AutoSize = true,
            Location = new Point(20, 15)
        };

        var lblSub = new Label
        {
            Text = "Quy trình hỗ trợ cấp lại thông tin đăng nhập hệ thống",
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = UI.Theme.Muted,
            AutoSize = true,
            Location = new Point(20, 40)
        };
        pnlHeader.Controls.Add(lblHeader);
        pnlHeader.Controls.Add(lblSub);

        var lblGuide = new Label
        {
            Text = "Để bảo vệ dữ liệu trung tâm, tài khoản Quản trị viên cần được cấp lại qua quy trình bảo mật nội bộ:\n\n"
                 + "Vui lòng liên hệ Bộ phận Kỹ thuật hệ thống HUIT LingoHub:\n"
                 + "- Hotline hỗ trợ nội bộ: 1900 6868 (Nhánh 1)\n"
                 + "- Email hỗ trợ: it-support@lingohub.edu.vn\n\n"
                 + "Mẹo môi trường thử nghiệm (Dev):\n"
                 + "Mật khẩu mặc định hệ thống: LingoHub@2026",
            Font = UI.Theme.FontRegular,
            ForeColor = UI.Theme.TextDark,
            Location = new Point(24, 85),
            Size = new Size(420, 175)
        };

        var btnClose = UI.UiHelpers.CreatePrimaryButton("Đã hiểu & Đóng", 140, 36);
        btnClose.Location = new Point(165, 275);
        btnClose.Click += (_, _) => dlg.Close();

        dlg.Controls.Add(btnClose);
        dlg.Controls.Add(lblGuide);
        dlg.Controls.Add(pnlHeader);

        dlg.ShowDialog(this);
    }
}
