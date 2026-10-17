namespace LanguageCenter.WinForms;

partial class FormDangNhap
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
        pnlBrand = new Panel();
        lblBrandCopyright = new Label();
        lblBrandStats = new Label();
        lblBrandSubtitle = new Label();
        lblBrandTitle = new Label();
        lblBrandBadge = new Label();
        pnlForm = new Panel();
        lblSecurityNote = new Label();
        lblError = new Label();
        btnLogin = new Button();
        lnkForgotPassword = new LinkLabel();
        chkShowPassword = new CheckBox();
        txtPassword = new TextBox();
        lblPassword = new Label();
        txtUsername = new TextBox();
        lblUsername = new Label();
        lblFormSubtitle = new Label();
        lblFormTitle = new Label();
        pnlBrand.SuspendLayout();
        pnlForm.SuspendLayout();
        SuspendLayout();
        // 
        // pnlBrand
        // 
        pnlBrand.BackColor = Color.FromArgb(37, 99, 235);
        pnlBrand.Controls.Add(lblBrandCopyright);
        pnlBrand.Controls.Add(lblBrandStats);
        pnlBrand.Controls.Add(lblBrandSubtitle);
        pnlBrand.Controls.Add(lblBrandTitle);
        pnlBrand.Controls.Add(lblBrandBadge);
        pnlBrand.Dock = DockStyle.Left;
        pnlBrand.Location = new Point(0, 0);
        pnlBrand.Margin = new Padding(4, 4, 4, 4);
        pnlBrand.Name = "pnlBrand";
        pnlBrand.Size = new Size(525, 700);
        pnlBrand.TabIndex = 1;
        // 
        // lblBrandCopyright
        // 
        lblBrandCopyright.AutoSize = true;
        lblBrandCopyright.Font = new Font("Segoe UI", 8.2F);
        lblBrandCopyright.ForeColor = Color.FromArgb(191, 219, 254);
        lblBrandCopyright.Location = new Point(50, 606);
        lblBrandCopyright.Margin = new Padding(4, 0, 4, 0);
        lblBrandCopyright.Name = "lblBrandCopyright";
        lblBrandCopyright.Size = new Size(465, 45);
        lblBrandCopyright.TabIndex = 0;
        lblBrandCopyright.UseMnemonic = false;
        lblBrandCopyright.Text = "© 2026 HUIT LingoHub\r\nBản quyền thuộc về Hệ thống Đào tạo Ngoại ngữ HUIT LingoHub.";
        // 
        // lblBrandStats
        // 
        lblBrandStats.Font = new Font("Segoe UI", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
        lblBrandStats.ForeColor = Color.FromArgb(219, 234, 254);
        lblBrandStats.Location = new Point(63, 356);
        lblBrandStats.Margin = new Padding(4, 0, 4, 0);
        lblBrandStats.Name = "lblBrandStats";
        lblBrandStats.Size = new Size(412, 131);
        lblBrandStats.TabIndex = 1;
        lblBrandStats.Text = "15.000+ Học viên theo học\r\n98% Tỉ lệ đạt mục tiêu đầu ra\r\n12+ Chi nhánh toàn quốc\r\nĐánh giá chất lượng hàng đầu";
        // 
        // lblBrandSubtitle
        // 
        lblBrandSubtitle.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
        lblBrandSubtitle.ForeColor = Color.FromArgb(238, 242, 255);
        lblBrandSubtitle.Location = new Point(56, 272);
        lblBrandSubtitle.Margin = new Padding(4, 0, 4, 0);
        lblBrandSubtitle.Name = "lblBrandSubtitle";
        lblBrandSubtitle.Size = new Size(412, 60);
        lblBrandSubtitle.TabIndex = 2;
        lblBrandSubtitle.Text = "Hệ thống quản trị đào tạo, học viên và học phí chuẩn quốc tế.";
        // 
        // lblBrandTitle
        // 
        lblBrandTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
        lblBrandTitle.ForeColor = Color.White;
        lblBrandTitle.Location = new Point(50, 94);
        lblBrandTitle.Margin = new Padding(4, 0, 4, 0);
        lblBrandTitle.Name = "lblBrandTitle";
        lblBrandTitle.Size = new Size(425, 169);
        lblBrandTitle.TabIndex = 3;
        lblBrandTitle.Text = "Chinh phục\r\ntiếng Anh cùng\r\nHUIT LingoHub";
        // 
        // lblBrandBadge
        // 
        lblBrandBadge.AutoSize = true;
        lblBrandBadge.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblBrandBadge.ForeColor = Color.FromArgb(219, 234, 254);
        lblBrandBadge.Location = new Point(56, 56);
        lblBrandBadge.Margin = new Padding(4, 0, 4, 0);
        lblBrandBadge.Name = "lblBrandBadge";
        lblBrandBadge.Size = new Size(117, 20);
        lblBrandBadge.TabIndex = 4;
        lblBrandBadge.Text = "HUIT LingoHub";
        // 
        // pnlForm
        // 
        pnlForm.BackColor = Color.White;
        pnlForm.Controls.Add(lblSecurityNote);
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
        pnlForm.Dock = DockStyle.Fill;
        pnlForm.Location = new Point(525, 0);
        pnlForm.Margin = new Padding(4, 4, 4, 4);
        pnlForm.Name = "pnlForm";
        pnlForm.Size = new Size(625, 700);
        pnlForm.TabIndex = 0;
        // 
        // lblSecurityNote
        // 
        lblSecurityNote.Font = new Font("Segoe UI", 8.2F);
        lblSecurityNote.ForeColor = Color.FromArgb(100, 116, 139);
        lblSecurityNote.Location = new Point(75, 474);
        lblSecurityNote.Margin = new Padding(4, 0, 4, 0);
        lblSecurityNote.Name = "lblSecurityNote";
        lblSecurityNote.Size = new Size(425, 42);
        lblSecurityNote.TabIndex = 0;
        lblSecurityNote.Text = "Phiên đăng nhập được mã hóa và tự động kết thúc sau 30 phút không hoạt động.";
        // 
        // lblError
        // 
        lblError.Font = new Font("Segoe UI", 9F);
        lblError.ForeColor = Color.FromArgb(239, 68, 68);
        lblError.Location = new Point(75, 360);
        lblError.Margin = new Padding(4, 0, 4, 0);
        lblError.Name = "lblError";
        lblError.Size = new Size(425, 45);
        lblError.TabIndex = 2;
        lblError.Visible = false;
        // 
        // btnLogin
        // 
        btnLogin.BackColor = Color.FromArgb(37, 99, 235);
        btnLogin.Cursor = Cursors.Hand;
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        btnLogin.ForeColor = Color.White;
        btnLogin.Location = new Point(75, 395);
        btnLogin.Margin = new Padding(4, 4, 4, 4);
        btnLogin.Name = "btnLogin";
        btnLogin.Size = new Size(425, 52);
        btnLogin.TabIndex = 3;
        btnLogin.Text = "ĐĂNG NHẬP NGAY";
        btnLogin.UseVisualStyleBackColor = false;
        btnLogin.Click += BtnLogin_Click;
        // 
        // lnkForgotPassword
        // 
        lnkForgotPassword.ActiveLinkColor = Color.FromArgb(29, 78, 216);
        lnkForgotPassword.AutoSize = true;
        lnkForgotPassword.Cursor = Cursors.Hand;
        lnkForgotPassword.Font = new Font("Segoe UI", 9F);
        lnkForgotPassword.LinkBehavior = LinkBehavior.HoverUnderline;
        lnkForgotPassword.LinkColor = Color.FromArgb(37, 99, 235);
        lnkForgotPassword.Location = new Point(365, 325);
        lnkForgotPassword.Margin = new Padding(4, 0, 4, 0);
        lnkForgotPassword.Name = "lnkForgotPassword";
        lnkForgotPassword.Size = new Size(116, 20);
        lnkForgotPassword.TabIndex = 4;
        lnkForgotPassword.TabStop = true;
        lnkForgotPassword.Text = "Quên mật khẩu?";
        lnkForgotPassword.VisitedLinkColor = Color.FromArgb(37, 99, 235);
        lnkForgotPassword.LinkClicked += LnkForgotPassword_LinkClicked;
        // 
        // chkShowPassword
        // 
        chkShowPassword.AutoSize = true;
        chkShowPassword.Font = new Font("Segoe UI", 9F);
        chkShowPassword.ForeColor = Color.FromArgb(100, 116, 139);
        chkShowPassword.Location = new Point(75, 325);
        chkShowPassword.Margin = new Padding(4, 4, 4, 4);
        chkShowPassword.Name = "chkShowPassword";
        chkShowPassword.Size = new Size(127, 24);
        chkShowPassword.TabIndex = 5;
        chkShowPassword.Text = "Hiện mật khẩu";
        chkShowPassword.CheckedChanged += ChkShowPassword_CheckedChanged;
        // 
        // txtPassword
        // 
        txtPassword.Font = new Font("Segoe UI", 11F);
        txtPassword.Location = new Point(75, 275);
        txtPassword.Margin = new Padding(4, 4, 4, 4);
        txtPassword.Name = "txtPassword";
        txtPassword.PlaceholderText = "Nhập mật khẩu...";
        txtPassword.Size = new Size(424, 32);
        txtPassword.TabIndex = 6;
        txtPassword.UseSystemPasswordChar = true;
        // 
        // lblPassword
        // 
        lblPassword.AutoSize = true;
        lblPassword.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblPassword.ForeColor = Color.FromArgb(15, 23, 42);
        lblPassword.Location = new Point(75, 244);
        lblPassword.Margin = new Padding(4, 0, 4, 0);
        lblPassword.Name = "lblPassword";
        lblPassword.Size = new Size(75, 20);
        lblPassword.TabIndex = 7;
        lblPassword.Text = "Mật khẩu";
        // 
        // txtUsername
        // 
        txtUsername.Font = new Font("Segoe UI", 11F);
        txtUsername.Location = new Point(75, 181);
        txtUsername.Margin = new Padding(4, 4, 4, 4);
        txtUsername.Name = "txtUsername";
        txtUsername.PlaceholderText = "VD: ADMIN001, GV001, TG001";
        txtUsername.Size = new Size(424, 32);
        txtUsername.TabIndex = 8;
        // 
        // lblUsername
        // 
        lblUsername.AutoSize = true;
        lblUsername.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        lblUsername.ForeColor = Color.FromArgb(15, 23, 42);
        lblUsername.Location = new Point(75, 150);
        lblUsername.Margin = new Padding(4, 0, 4, 0);
        lblUsername.Name = "lblUsername";
        lblUsername.Size = new Size(112, 20);
        lblUsername.TabIndex = 9;
        lblUsername.Text = "Tên đăng nhập";
        // 
        // lblFormSubtitle
        // 
        lblFormSubtitle.AutoSize = true;
        lblFormSubtitle.Font = new Font("Segoe UI", 9.2F);
        lblFormSubtitle.ForeColor = Color.FromArgb(100, 116, 139);
        lblFormSubtitle.Location = new Point(75, 98);
        lblFormSubtitle.Margin = new Padding(4, 0, 4, 0);
        lblFormSubtitle.Name = "lblFormSubtitle";
        lblFormSubtitle.Size = new Size(440, 21);
        lblFormSubtitle.TabIndex = 10;
        lblFormSubtitle.Text = "Chào mừng bạn trở lại! Vui lòng đăng nhập tài khoản quản trị.";
        // 
        // lblFormTitle
        // 
        lblFormTitle.AutoSize = true;
        lblFormTitle.Font = new Font("Segoe UI", 19.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
        lblFormTitle.ForeColor = Color.FromArgb(15, 23, 42);
        lblFormTitle.Location = new Point(75, 36);
        lblFormTitle.Margin = new Padding(4, 0, 4, 0);
        lblFormTitle.Name = "lblFormTitle";
        lblFormTitle.Size = new Size(184, 45);
        lblFormTitle.TabIndex = 11;
        lblFormTitle.Text = "Đăng nhập";
        // 
        // FormDangNhap
        // 
        AcceptButton = btnLogin;
        AutoScaleDimensions = new SizeF(120F, 120F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(1150, 700);
        Controls.Add(pnlForm);
        Controls.Add(pnlBrand);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        Margin = new Padding(4, 4, 4, 4);
        MaximizeBox = false;
        Name = "FormDangNhap";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Đăng nhập — HUIT LingoHub";
        pnlBrand.ResumeLayout(false);
        pnlBrand.PerformLayout();
        pnlForm.ResumeLayout(false);
        pnlForm.PerformLayout();
        ResumeLayout(false);
    }
}
