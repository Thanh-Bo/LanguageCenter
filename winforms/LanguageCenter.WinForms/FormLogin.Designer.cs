namespace LanguageCenter.WinForms;

partial class FormLogin
{
    private System.ComponentModel.IContainer components = null;

    private Panel pnlBrand = null!;
    private Label lblBrandBadge = null!;
    private Label lblBrandTitle = null!;
    private Label lblBrandSubtitle = null!;
    private Label lblBrandStats = null!;
    private Label lblBrandCopyright = null!;

    private Panel pnlForm = null!;
    private Label lblFormTitle = null!;
    private Label lblFormSubtitle = null!;
    private Label lblUsername = null!;
    private TextBox txtUsername = null!;
    private Label lblPassword = null!;
    private TextBox txtPassword = null!;
    private CheckBox chkShowPassword = null!;
    private LinkLabel lnkForgotPassword = null!;
    private Button btnLogin = null!;
    private Label lblError = null!;
    private Label lblSecurityNote = null!;
    private Label lblHint = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        pnlBrand = new Panel();
        lblBrandBadge = new Label();
        lblBrandTitle = new Label();
        lblBrandSubtitle = new Label();
        lblBrandStats = new Label();
        lblBrandCopyright = new Label();

        pnlForm = new Panel();
        lblFormTitle = new Label();
        lblFormSubtitle = new Label();
        lblUsername = new Label();
        txtUsername = new TextBox();
        lblPassword = new Label();
        txtPassword = new TextBox();
        chkShowPassword = new CheckBox();
        lnkForgotPassword = new LinkLabel();
        btnLogin = new Button();
        lblError = new Label();
        lblSecurityNote = new Label();
        lblHint = new Label();

        SuspendLayout();

        // ====================================================================
        // pnlBrand (Thương hiệu & Thông tin hệ thống bên trái)
        // ====================================================================
        pnlBrand.Dock = DockStyle.Left;
        pnlBrand.Width = 420;
        pnlBrand.BackColor = UI.Theme.Primary;
        pnlBrand.Controls.Add(lblBrandCopyright);
        pnlBrand.Controls.Add(lblBrandStats);
        pnlBrand.Controls.Add(lblBrandSubtitle);
        pnlBrand.Controls.Add(lblBrandTitle);
        pnlBrand.Controls.Add(lblBrandBadge);

        // lblBrandBadge
        lblBrandBadge.Text = $"★ {Api.AppConfig.BrandShortName} ACADEMY";
        lblBrandBadge.ForeColor = Color.FromArgb(219, 234, 254);
        lblBrandBadge.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
        lblBrandBadge.AutoSize = true;
        lblBrandBadge.Location = new Point(45, 45);

        // lblBrandTitle
        lblBrandTitle.Text = $"Chinh phục\ntiếng Anh cùng\n{Api.AppConfig.BrandShortName} 🚀";
        lblBrandTitle.ForeColor = Color.White;
        lblBrandTitle.Font = new Font("Segoe UI", 24f, FontStyle.Bold);
        lblBrandTitle.AutoSize = false;
        lblBrandTitle.Location = new Point(40, 75);
        lblBrandTitle.Size = new Size(340, 135);

        // lblBrandSubtitle
        lblBrandSubtitle.Text = "Hệ thống quản trị đào tạo, học viên và biểu phí chuẩn quốc tế.";
        lblBrandSubtitle.ForeColor = Color.FromArgb(238, 242, 255);
        lblBrandSubtitle.Font = new Font("Segoe UI", 10f);
        lblBrandSubtitle.AutoSize = false;
        lblBrandSubtitle.Location = new Point(45, 218);
        lblBrandSubtitle.Size = new Size(330, 48);

        // lblBrandStats (Chỉ số tin cậy)
        lblBrandStats.Text = "✓ 15.000+ Học viên theo học\n✓ 98% Tỉ lệ đạt mục tiêu đầu ra\n✓ 12+ Chi nhánh toàn quốc\n✓ 4.9⭐ Đánh giá hài lòng";
        lblBrandStats.ForeColor = Color.FromArgb(219, 234, 254);
        lblBrandStats.Font = new Font("Segoe UI", 9.5f);
        lblBrandStats.AutoSize = false;
        lblBrandStats.Location = new Point(45, 285);
        lblBrandStats.Size = new Size(330, 105);

        // lblBrandCopyright (Bản quyền bên trái)
        lblBrandCopyright.Text = $"© 2026 {Api.AppConfig.BrandShortName} Academy.\nBản quyền thuộc về {Api.AppConfig.BrandName}.";
        lblBrandCopyright.ForeColor = Color.FromArgb(191, 219, 254);
        lblBrandCopyright.Font = new Font("Segoe UI", 8.2f);
        lblBrandCopyright.AutoSize = false;
        lblBrandCopyright.Location = new Point(45, 485);
        lblBrandCopyright.Size = new Size(330, 45);

        // ====================================================================
        // pnlForm (Khung nhập liệu bên phải)
        // ====================================================================
        pnlForm.Dock = DockStyle.Fill;
        pnlForm.BackColor = Color.White;
        pnlForm.Controls.Add(lblSecurityNote);
        pnlForm.Controls.Add(lblHint);
        pnlForm.Controls.Add(lblError);
        pnlForm.Controls.Add(btnLogin);
        pnlForm.Controls.Add(lnkForgotPassword);
        pnlForm.Controls.Add(chkShowPassword);
        pnlForm.Controls.Add(txtPassword);
        pnlForm.Controls.Add(lblPassword);
        pnlForm.Controls.Add(txtUsername);
        pnlForm.Controls.Add(lblUsername);
        pnlForm.Controls.Add(lblFormSubtitle);
        pnlForm.Controls.Add(lblFormTitle);

        // lblFormTitle
        lblFormTitle.Text = "Đăng nhập";
        lblFormTitle.Font = new Font("Segoe UI", 18f, FontStyle.Bold);
        lblFormTitle.ForeColor = UI.Theme.TextDark;
        lblFormTitle.AutoSize = true;
        lblFormTitle.Location = new Point(60, 40);

        // lblFormSubtitle
        lblFormSubtitle.Text = "Chào mừng trở lại! Vui lòng đăng nhập tài khoản quản trị.";
        lblFormSubtitle.Font = new Font("Segoe UI", 9.2f);
        lblFormSubtitle.ForeColor = UI.Theme.Muted;
        lblFormSubtitle.AutoSize = true;
        lblFormSubtitle.Location = new Point(60, 78);

        // lblUsername
        lblUsername.Text = "Tên đăng nhập";
        lblUsername.Font = UI.Theme.FontBold;
        lblUsername.ForeColor = UI.Theme.TextDark;
        lblUsername.AutoSize = true;
        lblUsername.Location = new Point(60, 120);

        // txtUsername
        txtUsername.Location = new Point(60, 145);
        txtUsername.Size = new Size(340, 32);
        txtUsername.Font = new Font("Segoe UI", 11f);
        txtUsername.PlaceholderText = "VD: ADMIN001, GV001, TG001";

        // lblPassword
        lblPassword.Text = "Mật khẩu";
        lblPassword.Font = UI.Theme.FontBold;
        lblPassword.ForeColor = UI.Theme.TextDark;
        lblPassword.AutoSize = true;
        lblPassword.Location = new Point(60, 195);

        // txtPassword
        txtPassword.Location = new Point(60, 220);
        txtPassword.Size = new Size(340, 32);
        txtPassword.Font = new Font("Segoe UI", 11f);
        txtPassword.UseSystemPasswordChar = true;
        txtPassword.PlaceholderText = "Nhập mật khẩu...";

        // chkShowPassword
        chkShowPassword.Text = "Hiện mật khẩu";
        chkShowPassword.AutoSize = true;
        chkShowPassword.Font = UI.Theme.FontRegular;
        chkShowPassword.ForeColor = UI.Theme.Muted;
        chkShowPassword.Location = new Point(60, 260);
        chkShowPassword.CheckedChanged += ChkShowPassword_CheckedChanged;

        // lnkForgotPassword
        lnkForgotPassword.Text = "Quên mật khẩu?";
        lnkForgotPassword.AutoSize = true;
        lnkForgotPassword.Font = UI.Theme.FontRegular;
        lnkForgotPassword.LinkColor = UI.Theme.Primary;
        lnkForgotPassword.ActiveLinkColor = UI.Theme.PrimaryDark;
        lnkForgotPassword.VisitedLinkColor = UI.Theme.Primary;
        lnkForgotPassword.LinkBehavior = LinkBehavior.HoverUnderline;
        lnkForgotPassword.Location = new Point(292, 260);
        lnkForgotPassword.Cursor = Cursors.Hand;
        lnkForgotPassword.LinkClicked += LnkForgotPassword_LinkClicked;

        // lblError
        lblError.ForeColor = UI.Theme.Danger;
        lblError.Font = UI.Theme.FontRegular;
        lblError.AutoSize = false;
        lblError.Location = new Point(60, 288);
        lblError.Size = new Size(340, 36);
        lblError.Visible = false;

        // btnLogin
        btnLogin.Text = "ĐĂNG NHẬP NGAY";
        btnLogin.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
        btnLogin.ForeColor = Color.White;
        btnLogin.BackColor = UI.Theme.Primary;
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.Location = new Point(60, 330);
        btnLogin.Size = new Size(340, 42);
        btnLogin.Cursor = Cursors.Hand;
        btnLogin.Click += BtnLogin_Click;

        // lblSecurityNote (Thông báo bảo mật phiên đăng nhập)
        lblSecurityNote.Text = "🔒 Phiên đăng nhập được mã hóa và tự động kết thúc sau 30 phút không hoạt động.";
        lblSecurityNote.Font = new Font("Segoe UI", 8.2f);
        lblSecurityNote.ForeColor = UI.Theme.Muted;
        lblSecurityNote.AutoSize = false;
        lblSecurityNote.Location = new Point(60, 385);
        lblSecurityNote.Size = new Size(340, 34);

        // lblHint
        lblHint.Text = "💡 Mật khẩu: 1 | Demo: ADMIN001 (Quản trị) | GV001 (Giáo viên) | TG001 (Trợ giảng)";
        lblHint.Font = new Font("Segoe UI", 8.2f, FontStyle.Italic);
        lblHint.ForeColor = UI.Theme.Primary;
        lblHint.AutoSize = false;
        lblHint.Location = new Point(60, 425);
        lblHint.Size = new Size(340, 32);

        // ====================================================================
        // FormLogin
        // ====================================================================
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(920, 560);
        Controls.Add(pnlForm);
        Controls.Add(pnlBrand);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = $"Đăng nhập — {Api.AppConfig.BrandName}";
        AcceptButton = btnLogin;

        ResumeLayout(false);
    }
}
