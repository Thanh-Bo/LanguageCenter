using LanguageCenter.WinForms.Api;

namespace LanguageCenter.WinForms;

public partial class FormDangNhap : Form
{
    /// <summary>Mật khẩu dùng cho các nút "đăng nhập nhanh" (đã đặt lại toàn bộ về 111111).</summary>
    private const string MatKhauNhanh = "111111";

    /// <summary>Tài khoản mẫu — chỉ gồm vai trò được phép vào phân hệ WinForms.</summary>
    private static readonly string[] TaiKhoanNhanh =
    {
        "ADMIN001",
        "GV001",
        "GV002",
        "GV003",
        "TG001"
    };

    private readonly List<Button> _btnDangNhapNhanh = new();

    public FormDangNhap()
    {
        InitializeComponent();

        DungDangNhapNhanh();
    }

    /// <summary>
    /// Khối "đăng nhập nhanh" dưới nút Đăng nhập: mỗi tài khoản là một nút,
    /// bấm là điền sẵn tên đăng nhập + mật khẩu và đăng nhập luôn.
    /// </summary>
    private void DungDangNhapNhanh()
    {
        var pnl = new Panel
        {
            Location = new Point(75, 528),
            Size = new Size(425, 160),
            BackColor = Color.White
        };

        pnl.Controls.Add(new Label
        {
            Text = $"ĐĂNG NHẬP NHANH — mật khẩu: {MatKhauNhanh}",
            Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(100, 116, 139),
            AutoSize = true,
            Location = new Point(0, 0)
        });

        var flp = new FlowLayoutPanel
        {
            Location = new Point(0, 24),
            Size = new Size(425, 74),
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            BackColor = Color.White
        };

        foreach (var tenDangNhap in TaiKhoanNhanh)
        {
            var btn = UI.UiHelpers.CreateSecondaryButton(tenDangNhap, 84, 30);
            btn.Font = new Font("Segoe UI", 9f);
            btn.Margin = new Padding(0, 0, 8, 8);
            btn.Click += (_, _) => DangNhapNhanh(tenDangNhap);

            _btnDangNhapNhanh.Add(btn);
            flp.Controls.Add(btn);
        }

        pnl.Controls.Add(flp);

        pnl.Controls.Add(new Label
        {
            Text = "Chỉ tài khoản Quản trị viên / Giáo viên / Trợ giảng mới vào được phân hệ này "
                   + "(tài khoản Học viên dùng bản Web).",
            Font = new Font("Segoe UI", 8.2f),
            ForeColor = Color.FromArgb(148, 163, 184),
            AutoSize = false,
            Size = new Size(425, 32),
            Location = new Point(0, 100)
        });

        pnlForm.Controls.Add(pnl);
        pnl.BringToFront();
    }

    /// <summary>Điền sẵn tài khoản + mật khẩu mặc định rồi đăng nhập ngay.</summary>
    private void DangNhapNhanh(string tenDangNhap)
    {
        if (!btnLogin.Enabled)
        {
            return;
        }

        txtUsername.Text = tenDangNhap;
        txtPassword.Text = MatKhauNhanh;
        chkShowPassword.Checked = false;

        BtnLogin_Click(null, EventArgs.Empty);
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

        foreach (var btn in _btnDangNhapNhanh)
        {
            btn.Enabled = !loading;
        }
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
