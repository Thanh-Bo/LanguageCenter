namespace LanguageCenter.WinForms;

partial class FormMain
{
    private System.ComponentModel.IContainer components = null;

    private Panel pnlSidebar = null!;
    private Label lblLogo = null!;
    private Label lblLogoSub = null!;
    private Button btnNavDashboard = null!;
    private Button btnNavCourses = null!;
    private Button btnNavTuition = null!;
    private Button btnNavStudents = null!;
    private Button btnNavTeachers = null!;
    private Button btnNavAssistants = null!;
    private Button btnNavAccounts = null!;
    private Button btnNavLogout = null!;
    private Panel pnlHeader = null!;
    private Label lblPageTitle = null!;
    private Label lblCampus = null!;
    private Label lblCurrentUser = null!;
    private Panel pnlContent = null!;

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

        pnlSidebar = new Panel();
        lblLogo = new Label();
        lblLogoSub = new Label();
        btnNavDashboard = new Button();
        btnNavCourses = new Button();
        btnNavTuition = new Button();
        btnNavStudents = new Button();
        btnNavTeachers = new Button();
        btnNavAssistants = new Button();
        btnNavAccounts = new Button();
        btnNavLogout = new Button();
        pnlHeader = new Panel();
        lblPageTitle = new Label();
        lblCampus = new Label();
        lblCurrentUser = new Label();
        pnlContent = new Panel();

        SuspendLayout();

        // pnlSidebar (LanguageCenter Navy Sidebar)
        pnlSidebar.Dock = DockStyle.Left;
        pnlSidebar.Width = 250;
        pnlSidebar.BackColor = UI.Theme.SidebarBackground;
        pnlSidebar.Controls.Add(btnNavLogout);
        pnlSidebar.Controls.Add(btnNavAccounts);
        pnlSidebar.Controls.Add(btnNavAssistants);
        pnlSidebar.Controls.Add(btnNavTeachers);
        pnlSidebar.Controls.Add(btnNavStudents);
        pnlSidebar.Controls.Add(btnNavTuition);
        pnlSidebar.Controls.Add(btnNavCourses);
        pnlSidebar.Controls.Add(btnNavDashboard);
        pnlSidebar.Controls.Add(lblLogoSub);
        pnlSidebar.Controls.Add(lblLogo);

        // lblLogo
        lblLogo.Text = $"{Api.AppConfig.BrandShortName}";
        lblLogo.ForeColor = Color.White;
        lblLogo.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
        lblLogo.AutoSize = false;
        lblLogo.TextAlign = ContentAlignment.MiddleLeft;
        lblLogo.Location = new Point(24, 20);
        lblLogo.Size = new Size(200, 36);

        // lblLogoSub
        lblLogoSub.Text = "ACADEMY MANAGEMENT";
        lblLogoSub.ForeColor = UI.Theme.SidebarTextMuted;
        lblLogoSub.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
        lblLogoSub.AutoSize = false;
        lblLogoSub.TextAlign = ContentAlignment.MiddleLeft;
        lblLogoSub.Location = new Point(25, 54);
        lblLogoSub.Size = new Size(200, 20);

        ConfigureNavButton(btnNavDashboard, "Tổng quan hệ thống", 95);
        ConfigureNavButton(btnNavCourses, "Quản lý khóa học", 145);
        ConfigureNavButton(btnNavTuition, "Quản lý học phí", 195);
        ConfigureNavButton(btnNavStudents, "Quản lý học viên", 245);
        ConfigureNavButton(btnNavTeachers, "Quản lý giáo viên", 295);
        ConfigureNavButton(btnNavAssistants, "Quản lý trợ giảng", 345);
        ConfigureNavButton(btnNavAccounts, "Quản lý tài khoản", 395);

        btnNavDashboard.Click += (_, _) => NavigateTo(NavItem.Dashboard);
        btnNavCourses.Click += (_, _) => NavigateTo(NavItem.Courses);
        btnNavTuition.Click += (_, _) => NavigateTo(NavItem.Tuition);
        btnNavStudents.Click += (_, _) => NavigateTo(NavItem.Students);
        btnNavTeachers.Click += (_, _) => NavigateTo(NavItem.Teachers);
        btnNavAssistants.Click += (_, _) => NavigateTo(NavItem.Assistants);
        btnNavAccounts.Click += (_, _) => NavigateTo(NavItem.Accounts);

        // btnNavLogout
        btnNavLogout.Text = "Đăng xuất";
        btnNavLogout.FlatStyle = FlatStyle.Flat;
        btnNavLogout.FlatAppearance.BorderSize = 0;
        btnNavLogout.BackColor = UI.Theme.SidebarBackground;
        btnNavLogout.ForeColor = UI.Theme.SidebarTextMuted;
        btnNavLogout.Font = UI.Theme.FontRegular;
        btnNavLogout.TextAlign = ContentAlignment.MiddleLeft;
        btnNavLogout.Padding = new Padding(24, 0, 0, 0);
        btnNavLogout.Dock = DockStyle.Bottom;
        btnNavLogout.Height = 54;
        btnNavLogout.Cursor = Cursors.Hand;
        btnNavLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 50, 70);
        btnNavLogout.Click += BtnNavLogout_Click;

        // pnlHeader
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Height = 64;
        pnlHeader.BackColor = UI.Theme.HeaderBackground;
        pnlHeader.Controls.Add(lblCurrentUser);
        pnlHeader.Controls.Add(lblCampus);
        pnlHeader.Controls.Add(lblPageTitle);
        pnlHeader.Paint += (_, e) =>
        {
            using var pen = new Pen(UI.Theme.BorderColor);
            e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
        };

        // lblPageTitle
        lblPageTitle.Text = "Tổng quan";
        lblPageTitle.Font = UI.Theme.FontHeading;
        lblPageTitle.ForeColor = UI.Theme.TextDark;
        lblPageTitle.AutoSize = true;
        lblPageTitle.Location = new Point(28, 20);

        // lblCampus
        lblCampus.Text = $"{Api.AppConfig.BrandShortName} Campus";
        lblCampus.Font = new Font("Segoe UI", 9f);
        lblCampus.ForeColor = UI.Theme.Muted;
        lblCampus.AutoSize = true;
        lblCampus.Location = new Point(780, 22);

        // lblCurrentUser
        lblCurrentUser.Text = "";
        lblCurrentUser.Font = UI.Theme.FontBold;
        lblCurrentUser.ForeColor = UI.Theme.Primary;
        lblCurrentUser.AutoSize = true;
        lblCurrentUser.Location = new Point(1020, 22);

        void AlignHeaderLabels()
        {
            lblCurrentUser.Location = new Point(pnlHeader.Width - lblCurrentUser.Width - 28, 22);
            lblCampus.Location = new Point(lblCurrentUser.Left - lblCampus.Width - 24, 22);
        }

        pnlHeader.Resize += (_, _) => AlignHeaderLabels();
        lblCurrentUser.TextChanged += (_, _) => AlignHeaderLabels();

        // pnlContent
        pnlContent.Dock = DockStyle.Fill;
        pnlContent.BackColor = UI.Theme.ContentBackground;
        pnlContent.Padding = new Padding(24);

        // FormMain
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(1280, 800);
        Controls.Add(pnlContent);
        Controls.Add(pnlHeader);
        Controls.Add(pnlSidebar);
        MinimumSize = new Size(1100, 700);
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        Text = Api.AppConfig.ApplicationTitle;
        Load += FormMain_Load;

        ResumeLayout(false);
    }

    private void ConfigureNavButton(Button button, string text, int top)
    {
        button.Text = text;
        button.Location = new Point(0, top);
        button.Size = new Size(250, 46);
        button.FlatStyle = FlatStyle.Flat;
        button.FlatAppearance.BorderSize = 0;
        button.BackColor = UI.Theme.SidebarBackground;
        button.ForeColor = UI.Theme.SidebarText;
        button.Font = UI.Theme.FontRegular;
        button.TextAlign = ContentAlignment.MiddleLeft;
        button.Padding = new Padding(24, 0, 0, 0);
        button.Cursor = Cursors.Hand;
        button.FlatAppearance.MouseOverBackColor = UI.Theme.SidebarButtonHover;
    }
}
