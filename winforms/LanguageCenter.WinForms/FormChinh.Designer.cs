namespace LanguageCenter.WinForms;

partial class FormChinh
{
    private System.ComponentModel.IContainer components = null;

    private Panel pnlSidebar = null!;
    private Panel pnlLogo = null!;
    private Label lblLogo = null!;
    private Label lblLogoSub = null!;
    private Panel pnlNavList = null!;

    private Button btnNavDashboard = null!;
    private Button btnNavCourses = null!;
    private Button btnNavClasses = null!;
    private Button btnNavAssignments = null!;
    private Button btnNavEnrollment = null!;
    private Button btnNavRoomsSchedules = null!;
    private Button btnNavGrades = null!;
    private Button btnNavTuition = null!;
    private Button btnNavStudents = null!;
    private Button btnNavTeachers = null!;
    private Button btnNavAssistants = null!;
    private Button btnNavNotifications = null!;
    private Button btnNavReports = null!;
    private Button btnNavAccounts = null!;
    private Button btnNavLogout = null!;

    private Panel pnlHeader = null!;
    private Label lblPageTitle = null!;
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
        pnlLogo = new Panel();
        lblLogo = new Label();
        lblLogoSub = new Label();
        pnlNavList = new Panel();

        btnNavDashboard = new Button();
        btnNavCourses = new Button();
        btnNavClasses = new Button();
        btnNavAssignments = new Button();
        btnNavEnrollment = new Button();
        btnNavRoomsSchedules = new Button();
        btnNavGrades = new Button();
        btnNavTuition = new Button();
        btnNavStudents = new Button();
        btnNavTeachers = new Button();
        btnNavAssistants = new Button();
        btnNavNotifications = new Button();
        btnNavReports = new Button();
        btnNavAccounts = new Button();
        btnNavLogout = new Button();

        pnlHeader = new Panel();
        lblPageTitle = new Label();
        lblCurrentUser = new Label();
        pnlContent = new Panel();

        SuspendLayout();

        // 1. pnlSidebar
        pnlSidebar.Dock = DockStyle.Left;
        pnlSidebar.Width = 250;
        pnlSidebar.BackColor = Color.FromArgb(15, 23, 42);

        // pnlLogo
        pnlLogo.Dock = DockStyle.Top;
        pnlLogo.Height = 80;
        pnlLogo.BackColor = Color.FromArgb(15, 23, 42);
        pnlLogo.Controls.Add(lblLogoSub);
        pnlLogo.Controls.Add(lblLogo);

        // lblLogo
        lblLogo.Text = "HUIT LingoHub";
        lblLogo.ForeColor = Color.White;
        lblLogo.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
        lblLogo.AutoSize = false;
        lblLogo.TextAlign = ContentAlignment.MiddleLeft;
        lblLogo.Location = new Point(24, 18);
        lblLogo.Size = new Size(200, 32);

        // lblLogoSub
        lblLogoSub.Text = "EDUCATION MANAGEMENT";
        lblLogoSub.ForeColor = Color.FromArgb(100, 116, 139);
        lblLogoSub.Font = new Font("Segoe UI", 7.8f, FontStyle.Bold);
        lblLogoSub.AutoSize = false;
        lblLogoSub.TextAlign = ContentAlignment.MiddleLeft;
        lblLogoSub.Location = new Point(25, 48);
        lblLogoSub.Size = new Size(200, 18);

        // btnNavLogout
        btnNavLogout.Text = "Đăng xuất";
        btnNavLogout.FlatStyle = FlatStyle.Flat;
        btnNavLogout.FlatAppearance.BorderSize = 0;
        btnNavLogout.BackColor = Color.FromArgb(15, 23, 42);
        btnNavLogout.ForeColor = Color.FromArgb(100, 116, 139);
        btnNavLogout.Font = new Font("Segoe UI", 9.5f);
        btnNavLogout.TextAlign = ContentAlignment.MiddleLeft;
        btnNavLogout.Padding = new Padding(24, 0, 0, 0);
        btnNavLogout.Dock = DockStyle.Bottom;
        btnNavLogout.Height = 50;
        btnNavLogout.Cursor = Cursors.Hand;
        btnNavLogout.FlatAppearance.MouseOverBackColor = Color.FromArgb(40, 50, 70);
        btnNavLogout.Click += BtnNavLogout_Click;

        // pnlNavList (Scrollable list of buttons)
        pnlNavList.Dock = DockStyle.Fill;
        pnlNavList.AutoScroll = true;
        pnlNavList.BackColor = Color.FromArgb(15, 23, 42);

        // btnNavDashboard
        btnNavDashboard.Text = "Tổng quan hệ thống";
        btnNavDashboard.Location = new Point(0, 4);
        btnNavDashboard.Size = new Size(232, 40);
        btnNavDashboard.FlatStyle = FlatStyle.Flat;
        btnNavDashboard.FlatAppearance.BorderSize = 0;
        btnNavDashboard.BackColor = Color.FromArgb(15, 23, 42);
        btnNavDashboard.ForeColor = Color.FromArgb(148, 163, 184);
        btnNavDashboard.Font = new Font("Segoe UI", 9.5f);
        btnNavDashboard.TextAlign = ContentAlignment.MiddleLeft;
        btnNavDashboard.Padding = new Padding(24, 0, 0, 0);
        btnNavDashboard.Cursor = Cursors.Hand;
        btnNavDashboard.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);

        // btnNavCourses
        btnNavCourses.Text = "Chương trình học";
        btnNavCourses.Location = new Point(0, 46);
        btnNavCourses.Size = new Size(232, 40);
        btnNavCourses.FlatStyle = FlatStyle.Flat;
        btnNavCourses.FlatAppearance.BorderSize = 0;
        btnNavCourses.BackColor = Color.FromArgb(15, 23, 42);
        btnNavCourses.ForeColor = Color.FromArgb(148, 163, 184);
        btnNavCourses.Font = new Font("Segoe UI", 9.5f);
        btnNavCourses.TextAlign = ContentAlignment.MiddleLeft;
        btnNavCourses.Padding = new Padding(24, 0, 0, 0);
        btnNavCourses.Cursor = Cursors.Hand;
        btnNavCourses.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);

        // btnNavClasses
        btnNavClasses.Text = "Quản lý lớp học";
        btnNavClasses.Location = new Point(0, 88);
        btnNavClasses.Size = new Size(232, 40);
        btnNavClasses.FlatStyle = FlatStyle.Flat;
        btnNavClasses.FlatAppearance.BorderSize = 0;
        btnNavClasses.BackColor = Color.FromArgb(15, 23, 42);
        btnNavClasses.ForeColor = Color.FromArgb(148, 163, 184);
        btnNavClasses.Font = new Font("Segoe UI", 9.5f);
        btnNavClasses.TextAlign = ContentAlignment.MiddleLeft;
        btnNavClasses.Padding = new Padding(24, 0, 0, 0);
        btnNavClasses.Cursor = Cursors.Hand;
        btnNavClasses.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);

        // btnNavAssignments
        btnNavAssignments.Text = "Phân công giảng dạy";
        btnNavAssignments.Location = new Point(0, 130);
        btnNavAssignments.Size = new Size(232, 40);
        btnNavAssignments.FlatStyle = FlatStyle.Flat;
        btnNavAssignments.FlatAppearance.BorderSize = 0;
        btnNavAssignments.BackColor = Color.FromArgb(15, 23, 42);
        btnNavAssignments.ForeColor = Color.FromArgb(148, 163, 184);
        btnNavAssignments.Font = new Font("Segoe UI", 9.5f);
        btnNavAssignments.TextAlign = ContentAlignment.MiddleLeft;
        btnNavAssignments.Padding = new Padding(24, 0, 0, 0);
        btnNavAssignments.Cursor = Cursors.Hand;
        btnNavAssignments.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);

        // btnNavEnrollment
        btnNavEnrollment.Text = "Xếp học viên vào lớp";
        btnNavEnrollment.Location = new Point(0, 172);
        btnNavEnrollment.Size = new Size(232, 40);
        btnNavEnrollment.FlatStyle = FlatStyle.Flat;
        btnNavEnrollment.FlatAppearance.BorderSize = 0;
        btnNavEnrollment.BackColor = Color.FromArgb(15, 23, 42);
        btnNavEnrollment.ForeColor = Color.FromArgb(148, 163, 184);
        btnNavEnrollment.Font = new Font("Segoe UI", 9.5f);
        btnNavEnrollment.TextAlign = ContentAlignment.MiddleLeft;
        btnNavEnrollment.Padding = new Padding(24, 0, 0, 0);
        btnNavEnrollment.Cursor = Cursors.Hand;
        btnNavEnrollment.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);

        // btnNavRoomsSchedules
        btnNavRoomsSchedules.Text = "Lịch học";
        btnNavRoomsSchedules.Location = new Point(0, 214);
        btnNavRoomsSchedules.Size = new Size(232, 40);
        btnNavRoomsSchedules.FlatStyle = FlatStyle.Flat;
        btnNavRoomsSchedules.FlatAppearance.BorderSize = 0;
        btnNavRoomsSchedules.BackColor = Color.FromArgb(15, 23, 42);
        btnNavRoomsSchedules.ForeColor = Color.FromArgb(148, 163, 184);
        btnNavRoomsSchedules.Font = new Font("Segoe UI", 9.5f);
        btnNavRoomsSchedules.TextAlign = ContentAlignment.MiddleLeft;
        btnNavRoomsSchedules.Padding = new Padding(24, 0, 0, 0);
        btnNavRoomsSchedules.Cursor = Cursors.Hand;
        btnNavRoomsSchedules.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);

        // btnNavGrades
        btnNavGrades.Text = "Quản lý điểm số";
        btnNavGrades.Location = new Point(0, 256);
        btnNavGrades.Size = new Size(232, 40);
        btnNavGrades.FlatStyle = FlatStyle.Flat;
        btnNavGrades.FlatAppearance.BorderSize = 0;
        btnNavGrades.BackColor = Color.FromArgb(15, 23, 42);
        btnNavGrades.ForeColor = Color.FromArgb(148, 163, 184);
        btnNavGrades.Font = new Font("Segoe UI", 9.5f);
        btnNavGrades.TextAlign = ContentAlignment.MiddleLeft;
        btnNavGrades.Padding = new Padding(24, 0, 0, 0);
        btnNavGrades.Cursor = Cursors.Hand;
        btnNavGrades.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);

        // btnNavTuition
        btnNavTuition.Text = "Học phí";
        btnNavTuition.Location = new Point(0, 298);
        btnNavTuition.Size = new Size(232, 40);
        btnNavTuition.FlatStyle = FlatStyle.Flat;
        btnNavTuition.FlatAppearance.BorderSize = 0;
        btnNavTuition.BackColor = Color.FromArgb(15, 23, 42);
        btnNavTuition.ForeColor = Color.FromArgb(148, 163, 184);
        btnNavTuition.Font = new Font("Segoe UI", 9.5f);
        btnNavTuition.TextAlign = ContentAlignment.MiddleLeft;
        btnNavTuition.Padding = new Padding(24, 0, 0, 0);
        btnNavTuition.Cursor = Cursors.Hand;
        btnNavTuition.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);

        // btnNavStudents
        btnNavStudents.Text = "Quản lý học viên";
        btnNavStudents.Location = new Point(0, 340);
        btnNavStudents.Size = new Size(232, 40);
        btnNavStudents.FlatStyle = FlatStyle.Flat;
        btnNavStudents.FlatAppearance.BorderSize = 0;
        btnNavStudents.BackColor = Color.FromArgb(15, 23, 42);
        btnNavStudents.ForeColor = Color.FromArgb(148, 163, 184);
        btnNavStudents.Font = new Font("Segoe UI", 9.5f);
        btnNavStudents.TextAlign = ContentAlignment.MiddleLeft;
        btnNavStudents.Padding = new Padding(24, 0, 0, 0);
        btnNavStudents.Cursor = Cursors.Hand;
        btnNavStudents.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);

        // btnNavTeachers
        btnNavTeachers.Text = "Quản lý giáo viên";
        btnNavTeachers.Location = new Point(0, 382);
        btnNavTeachers.Size = new Size(232, 40);
        btnNavTeachers.FlatStyle = FlatStyle.Flat;
        btnNavTeachers.FlatAppearance.BorderSize = 0;
        btnNavTeachers.BackColor = Color.FromArgb(15, 23, 42);
        btnNavTeachers.ForeColor = Color.FromArgb(148, 163, 184);
        btnNavTeachers.Font = new Font("Segoe UI", 9.5f);
        btnNavTeachers.TextAlign = ContentAlignment.MiddleLeft;
        btnNavTeachers.Padding = new Padding(24, 0, 0, 0);
        btnNavTeachers.Cursor = Cursors.Hand;
        btnNavTeachers.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);

        // btnNavAssistants
        btnNavAssistants.Text = "Quản lý trợ giảng";
        btnNavAssistants.Location = new Point(0, 424);
        btnNavAssistants.Size = new Size(232, 40);
        btnNavAssistants.FlatStyle = FlatStyle.Flat;
        btnNavAssistants.FlatAppearance.BorderSize = 0;
        btnNavAssistants.BackColor = Color.FromArgb(15, 23, 42);
        btnNavAssistants.ForeColor = Color.FromArgb(148, 163, 184);
        btnNavAssistants.Font = new Font("Segoe UI", 9.5f);
        btnNavAssistants.TextAlign = ContentAlignment.MiddleLeft;
        btnNavAssistants.Padding = new Padding(24, 0, 0, 0);
        btnNavAssistants.Cursor = Cursors.Hand;
        btnNavAssistants.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);

        // btnNavNotifications
        btnNavNotifications.Text = "Quản lý thông báo";
        btnNavNotifications.Location = new Point(0, 466);
        btnNavNotifications.Size = new Size(232, 40);
        btnNavNotifications.FlatStyle = FlatStyle.Flat;
        btnNavNotifications.FlatAppearance.BorderSize = 0;
        btnNavNotifications.BackColor = Color.FromArgb(15, 23, 42);
        btnNavNotifications.ForeColor = Color.FromArgb(148, 163, 184);
        btnNavNotifications.Font = new Font("Segoe UI", 9.5f);
        btnNavNotifications.TextAlign = ContentAlignment.MiddleLeft;
        btnNavNotifications.Padding = new Padding(24, 0, 0, 0);
        btnNavNotifications.Cursor = Cursors.Hand;
        btnNavNotifications.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);

        // btnNavReports
        btnNavReports.Text = "Thống kê & Báo cáo";
        btnNavReports.UseMnemonic = false;
        btnNavReports.Location = new Point(0, 508);
        btnNavReports.Size = new Size(232, 40);
        btnNavReports.FlatStyle = FlatStyle.Flat;
        btnNavReports.FlatAppearance.BorderSize = 0;
        btnNavReports.BackColor = Color.FromArgb(15, 23, 42);
        btnNavReports.ForeColor = Color.FromArgb(148, 163, 184);
        btnNavReports.Font = new Font("Segoe UI", 9.5f);
        btnNavReports.TextAlign = ContentAlignment.MiddleLeft;
        btnNavReports.Padding = new Padding(24, 0, 0, 0);
        btnNavReports.Cursor = Cursors.Hand;
        btnNavReports.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);

        // btnNavAccounts
        btnNavAccounts.Text = "Quản lý tài khoản";
        btnNavAccounts.Location = new Point(0, 550);
        btnNavAccounts.Size = new Size(232, 40);
        btnNavAccounts.FlatStyle = FlatStyle.Flat;
        btnNavAccounts.FlatAppearance.BorderSize = 0;
        btnNavAccounts.BackColor = Color.FromArgb(15, 23, 42);
        btnNavAccounts.ForeColor = Color.FromArgb(148, 163, 184);
        btnNavAccounts.Font = new Font("Segoe UI", 9.5f);
        btnNavAccounts.TextAlign = ContentAlignment.MiddleLeft;
        btnNavAccounts.Padding = new Padding(24, 0, 0, 0);
        btnNavAccounts.Cursor = Cursors.Hand;
        btnNavAccounts.FlatAppearance.MouseOverBackColor = Color.FromArgb(30, 41, 59);

        pnlNavList.Controls.Add(btnNavAccounts);
        pnlNavList.Controls.Add(btnNavReports);
        pnlNavList.Controls.Add(btnNavNotifications);
        pnlNavList.Controls.Add(btnNavAssistants);
        pnlNavList.Controls.Add(btnNavTeachers);
        pnlNavList.Controls.Add(btnNavStudents);
        pnlNavList.Controls.Add(btnNavTuition);
        pnlNavList.Controls.Add(btnNavGrades);
        pnlNavList.Controls.Add(btnNavRoomsSchedules);
        pnlNavList.Controls.Add(btnNavEnrollment);
        pnlNavList.Controls.Add(btnNavAssignments);
        pnlNavList.Controls.Add(btnNavClasses);
        pnlNavList.Controls.Add(btnNavCourses);
        pnlNavList.Controls.Add(btnNavDashboard);

        pnlSidebar.Controls.Add(pnlNavList);
        pnlSidebar.Controls.Add(btnNavLogout);
        pnlSidebar.Controls.Add(pnlLogo);

        // 2. pnlHeader
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Height = 64;
        pnlHeader.BackColor = Color.White;
        pnlHeader.Controls.Add(lblCurrentUser);
        pnlHeader.Controls.Add(lblPageTitle);

        // lblPageTitle
        lblPageTitle.Text = "Tổng quan";
        lblPageTitle.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
        lblPageTitle.ForeColor = Color.FromArgb(15, 23, 42);
        lblPageTitle.AutoSize = true;
        lblPageTitle.Location = new Point(28, 20);

        // lblCurrentUser
        lblCurrentUser.Text = "";
        lblCurrentUser.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        lblCurrentUser.ForeColor = Color.FromArgb(37, 99, 235);
        lblCurrentUser.AutoSize = false;
        lblCurrentUser.Size = new Size(400, 30);
        lblCurrentUser.TextAlign = ContentAlignment.MiddleRight;
        lblCurrentUser.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblCurrentUser.Location = new Point(602, 17);


        // 3. pnlContent
        pnlContent.Dock = DockStyle.Fill;
        pnlContent.BackColor = Color.FromArgb(248, 250, 252);
        pnlContent.Padding = new Padding(24);

        // FormChinh
        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(1280, 800);
        Controls.Add(pnlContent);
        Controls.Add(pnlHeader);
        Controls.Add(pnlSidebar);
        MinimumSize = new Size(1100, 700);
        StartPosition = FormStartPosition.CenterScreen;
        WindowState = FormWindowState.Maximized;
        Text = "Hệ thống Quản lý Đào tạo HUIT LingoHub";
        Load += FormMain_Load;

        ResumeLayout(false);
    }
}
