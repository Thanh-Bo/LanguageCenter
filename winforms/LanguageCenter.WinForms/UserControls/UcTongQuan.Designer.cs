namespace LanguageCenter.WinForms.UserControls;

partial class UcTongQuan
{
    private System.ComponentModel.IContainer components = null!;

    // Top Hero Banner
    private Panel pnlHero = null!;
    private Label lblHeroBadge = null!;
    private Label lblHeroTitle = null!;
    private Panel pnlHeroRight = null!;
    private Label lblHeroClock = null!;
    private Label lblHeroServer = null!;
    private Button btnRefresh = null!;

    // Section 1: Stats Header & Cards 
    private FlowLayoutPanel pnlStatsHeader = null!;
    private Label lblStatsHeaderTitle = null!;
    private Label lblLoading = null!;
    private TableLayoutPanel tblCards = null!;
    private Panel cardAccounts = null!;
    private Panel cardActiveAccounts = null!;
    private Panel cardStudents = null!;
    private Panel cardTeachers = null!;
    private Label lblAccountsTitle = null!;
    private Label lblAccountsValue = null!;
    private Label lblAccountsSub = null!;
    private Label lblActiveAccountsTitle = null!;
    private Label lblActiveAccountsValue = null!;
    private Label lblActiveAccountsSub = null!;
    private Label lblStudentsTitle = null!;
    private Label lblStudentsValue = null!;
    private Label lblStudentsSub = null!;
    private Label lblTeachersTitle = null!;
    private Label lblTeachersValue = null!;
    private Label lblTeachersSub = null!;

    // Spacer
    private Panel pnlSectionSpacer = null!;

    // Section 2: Main Split 2-Column Section 
    private TableLayoutPanel tblMainSection = null!;

    // Left Column
    private Panel pnlLeftCol = null!;
    private Panel pnlQuickActions = null!;
    private Label lblQuickTitle = null!;
    private Label lblQuickDesc = null!;
    private TableLayoutPanel tblQuickButtons = null!;
    private Button btnGoTuition = null!;
    private Button btnGoStudents = null!;
    private Button btnGoTeachers = null!;
    private Button btnGoAccounts = null!;

    private Panel pnlLeftSpacer = null!;
    private Panel pnlFeaturedCourses = null!;
    private Panel pnlCoursesHeader = null!;
    private Label lblCoursesTitle = null!;
    private LinkLabel lnkViewAllCourses = null!;
    private DataGridView dgvFeaturedCourses = null!;

    // Right Column
    private Panel pnlRightCol = null!;
    private Panel pnlNotices = null!;
    private Label lblNoticesTitle = null!;
    private Panel pnlNoticeItem1 = null!;
    private Label lblNoticeTag1 = null!;
    private Label lblNoticeTitle1 = null!;
    private Label lblNoticeDetail1 = null!;
    private Panel pnlNoticeItem2 = null!;
    private Label lblNoticeTag2 = null!;
    private Label lblNoticeTitle2 = null!;
    private Label lblNoticeDetail2 = null!;
    private Panel pnlNoticeItem3 = null!;
    private Label lblNoticeTag3 = null!;
    private Label lblNoticeTitle3 = null!;
    private Label lblNoticeDetail3 = null!;
    private Panel pnlNoticeItem4 = null!;
    private Label lblNoticeTag4 = null!;
    private Label lblNoticeTitle4 = null!;
    private Label lblNoticeDetail4 = null!;

    private Panel pnlBottomPadding = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components is not null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        pnlHero = new Panel();
        lblHeroBadge = new Label();
        lblHeroTitle = new Label();
        pnlHeroRight = new Panel();
        lblHeroClock = new Label();
        lblHeroServer = new Label();
        btnRefresh = new Button();

        pnlStatsHeader = new FlowLayoutPanel();
        lblStatsHeaderTitle = new Label();
        lblLoading = new Label();

        tblCards = new TableLayoutPanel();
        cardAccounts = new Panel();
        lblAccountsTitle = new Label();
        lblAccountsValue = new Label();
        lblAccountsSub = new Label();

        cardActiveAccounts = new Panel();
        lblActiveAccountsTitle = new Label();
        lblActiveAccountsValue = new Label();
        lblActiveAccountsSub = new Label();

        cardStudents = new Panel();
        lblStudentsTitle = new Label();
        lblStudentsValue = new Label();
        lblStudentsSub = new Label();

        cardTeachers = new Panel();
        lblTeachersTitle = new Label();
        lblTeachersValue = new Label();
        lblTeachersSub = new Label();

        pnlSectionSpacer = new Panel();

        tblMainSection = new TableLayoutPanel();
        pnlLeftCol = new Panel();
        pnlQuickActions = new Panel();
        lblQuickTitle = new Label();
        lblQuickDesc = new Label();
        tblQuickButtons = new TableLayoutPanel();
        btnGoTuition = new Button();
        btnGoStudents = new Button();
        btnGoTeachers = new Button();
        btnGoAccounts = new Button();

        pnlLeftSpacer = new Panel();
        pnlFeaturedCourses = new Panel();
        pnlCoursesHeader = new Panel();
        lblCoursesTitle = new Label();
        lnkViewAllCourses = new LinkLabel();
        dgvFeaturedCourses = new DataGridView();

        pnlRightCol = new Panel();
        pnlNotices = new Panel();
        lblNoticesTitle = new Label();
        pnlNoticeItem1 = new Panel();
        lblNoticeTag1 = new Label();
        lblNoticeTitle1 = new Label();
        lblNoticeDetail1 = new Label();

        pnlNoticeItem2 = new Panel();
        lblNoticeTag2 = new Label();
        lblNoticeTitle2 = new Label();
        lblNoticeDetail2 = new Label();

        pnlNoticeItem3 = new Panel();
        lblNoticeTag3 = new Label();
        lblNoticeTitle3 = new Label();
        lblNoticeDetail3 = new Label();

        pnlNoticeItem4 = new Panel();
        lblNoticeTag4 = new Label();
        lblNoticeTitle4 = new Label();
        lblNoticeDetail4 = new Label();

        pnlBottomPadding = new Panel();

        pnlHero.SuspendLayout();
        pnlHeroRight.SuspendLayout();
        pnlStatsHeader.SuspendLayout();
        tblCards.SuspendLayout();
        cardAccounts.SuspendLayout();
        cardActiveAccounts.SuspendLayout();
        cardStudents.SuspendLayout();
        cardTeachers.SuspendLayout();
        tblMainSection.SuspendLayout();
        pnlLeftCol.SuspendLayout();
        pnlQuickActions.SuspendLayout();
        tblQuickButtons.SuspendLayout();
        pnlFeaturedCourses.SuspendLayout();
        pnlCoursesHeader.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvFeaturedCourses).BeginInit();
        pnlRightCol.SuspendLayout();
        pnlNotices.SuspendLayout();
        pnlNoticeItem1.SuspendLayout();
        pnlNoticeItem2.SuspendLayout();
        pnlNoticeItem3.SuspendLayout();
        pnlNoticeItem4.SuspendLayout();
        SuspendLayout();

        // UcTongQuan
        Dock = DockStyle.Fill;
        AutoScroll = true;
        BackColor = Color.FromArgb(248, 250, 252);

        // 1. HERO BANNER
        pnlHero.Dock = DockStyle.Top;
        pnlHero.Height = 125;
        pnlHero.BackColor = Color.FromArgb(30, 86, 240);
        pnlHero.Padding = new Padding(24, 16, 24, 16);

        // pnlHeroRight
        pnlHeroRight.Dock = DockStyle.Right;
        pnlHeroRight.Width = 240;
        pnlHeroRight.BackColor = Color.Transparent;

        lblHeroClock.Text = "Hôm nay";
        lblHeroClock.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        lblHeroClock.ForeColor = Color.FromArgb(219, 234, 254);
        lblHeroClock.AutoSize = true;
        lblHeroClock.Location = new Point(10, 8);

        lblHeroServer.Text = "Hệ thống sẵn sàng (Online)";
        lblHeroServer.Font = new Font("Segoe UI", 8.5f);
        lblHeroServer.ForeColor = Color.FromArgb(167, 243, 208);
        lblHeroServer.AutoSize = true;
        lblHeroServer.Location = new Point(10, 32);

        btnRefresh.Text = "Làm mới";
        btnRefresh.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnRefresh.ForeColor = Color.FromArgb(30, 86, 240);
        btnRefresh.BackColor = Color.White;
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.FlatAppearance.BorderSize = 0;
        btnRefresh.Cursor = Cursors.Hand;
        btnRefresh.Size = new Size(130, 34);
        btnRefresh.Location = new Point(10, 58);

        pnlHeroRight.Controls.Add(btnRefresh);
        pnlHeroRight.Controls.Add(lblHeroServer);
        pnlHeroRight.Controls.Add(lblHeroClock);

        // Hero Left
        lblHeroBadge.Text = "HỆ THỐNG QUẢN TRỊ TRUNG TÂM NGOẠI NGỮ";
        lblHeroBadge.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        lblHeroBadge.ForeColor = Color.FromArgb(219, 234, 254);
        lblHeroBadge.AutoSize = true;
        lblHeroBadge.Location = new Point(20, 18);

        lblHeroTitle.Text = "Bảng điều khiển tổng quan";
        lblHeroTitle.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
        lblHeroTitle.ForeColor = Color.White;
        lblHeroTitle.AutoSize = true;
        lblHeroTitle.Location = new Point(19, 46);

        pnlHero.Controls.Add(pnlHeroRight);
        pnlHero.Controls.Add(lblHeroTitle);
        pnlHero.Controls.Add(lblHeroBadge);

        // 2. STATS HEADER ROW
        pnlStatsHeader.Dock = DockStyle.Top;
        pnlStatsHeader.Height = 44;
        pnlStatsHeader.WrapContents = false;
        pnlStatsHeader.FlowDirection = FlowDirection.LeftToRight;
        pnlStatsHeader.Padding = new Padding(2, 10, 0, 0);

        lblStatsHeaderTitle.Text = "CHỈ SỐ HOẠT ĐỘNG TOÀN HỆ THỐNG";
        lblStatsHeaderTitle.Font = new Font("Segoe UI", 11.5f, FontStyle.Bold);
        lblStatsHeaderTitle.ForeColor = Color.FromArgb(15, 23, 42);
        lblStatsHeaderTitle.AutoSize = true;
        lblStatsHeaderTitle.Margin = new Padding(0, 2, 16, 0);

        lblLoading.Text = "";
        lblLoading.Font = new Font("Segoe UI", 9.5f);
        lblLoading.ForeColor = Color.FromArgb(100, 116, 139);
        lblLoading.AutoSize = true;
        lblLoading.Margin = new Padding(0, 5, 0, 0);

        pnlStatsHeader.Controls.Add(lblStatsHeaderTitle);
        pnlStatsHeader.Controls.Add(lblLoading);

        // 3. STATS CARDS
        tblCards.Dock = DockStyle.Top;
        tblCards.Height = 136;
        tblCards.ColumnCount = 4;
        tblCards.RowCount = 1;
        tblCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tblCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tblCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tblCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tblCards.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        // Card 1: Accounts
        cardAccounts.Dock = DockStyle.Fill;
        cardAccounts.BackColor = Color.White;
        cardAccounts.Margin = new Padding(0, 0, 12, 0);
        lblAccountsTitle.Text = "Tổng tài khoản trên hệ thống";
        lblAccountsTitle.Font = new Font("Segoe UI", 9f);
        lblAccountsTitle.ForeColor = Color.FromArgb(100, 116, 139);
        lblAccountsTitle.AutoSize = true;
        lblAccountsTitle.Location = new Point(18, 16);
        lblAccountsValue.Text = "—";
        lblAccountsValue.Font = new Font("Segoe UI", 24f, FontStyle.Bold);
        lblAccountsValue.ForeColor = Color.FromArgb(30, 86, 240);
        lblAccountsValue.AutoSize = true;
        lblAccountsValue.Location = new Point(16, 40);
        lblAccountsSub.Text = "Quản trị viên & Cán bộ";
        lblAccountsSub.UseMnemonic = false;
        lblAccountsSub.Font = new Font("Segoe UI", 8.5f);
        lblAccountsSub.ForeColor = Color.FromArgb(71, 85, 105);
        lblAccountsSub.AutoSize = true;
        lblAccountsSub.Location = new Point(18, 96);
        cardAccounts.Controls.Add(lblAccountsSub);
        cardAccounts.Controls.Add(lblAccountsValue);
        cardAccounts.Controls.Add(lblAccountsTitle);

        // Card 2: Active Accounts
        cardActiveAccounts.Dock = DockStyle.Fill;
        cardActiveAccounts.BackColor = Color.White;
        cardActiveAccounts.Margin = new Padding(6, 0, 6, 0);
        lblActiveAccountsTitle.Text = "Tài khoản đang hoạt động";
        lblActiveAccountsTitle.Font = new Font("Segoe UI", 9f);
        lblActiveAccountsTitle.ForeColor = Color.FromArgb(100, 116, 139);
        lblActiveAccountsTitle.AutoSize = true;
        lblActiveAccountsTitle.Location = new Point(18, 16);
        lblActiveAccountsValue.Text = "—";
        lblActiveAccountsValue.Font = new Font("Segoe UI", 24f, FontStyle.Bold);
        lblActiveAccountsValue.ForeColor = Color.FromArgb(16, 185, 129);
        lblActiveAccountsValue.AutoSize = true;
        lblActiveAccountsValue.Location = new Point(16, 40);
        lblActiveAccountsSub.Text = "100% Đang kích hoạt";
        lblActiveAccountsSub.Font = new Font("Segoe UI", 8.5f);
        lblActiveAccountsSub.ForeColor = Color.FromArgb(71, 85, 105);
        lblActiveAccountsSub.AutoSize = true;
        lblActiveAccountsSub.Location = new Point(18, 96);
        cardActiveAccounts.Controls.Add(lblActiveAccountsSub);
        cardActiveAccounts.Controls.Add(lblActiveAccountsValue);
        cardActiveAccounts.Controls.Add(lblActiveAccountsTitle);

        // Card 3: Students
        cardStudents.Dock = DockStyle.Fill;
        cardStudents.BackColor = Color.White;
        cardStudents.Margin = new Padding(6, 0, 6, 0);
        lblStudentsTitle.Text = "Hồ sơ học viên";
        lblStudentsTitle.Font = new Font("Segoe UI", 9f);
        lblStudentsTitle.ForeColor = Color.FromArgb(100, 116, 139);
        lblStudentsTitle.AutoSize = true;
        lblStudentsTitle.Location = new Point(18, 16);
        lblStudentsValue.Text = "—";
        lblStudentsValue.Font = new Font("Segoe UI", 24f, FontStyle.Bold);
        lblStudentsValue.ForeColor = Color.FromArgb(139, 92, 246);
        lblStudentsValue.AutoSize = true;
        lblStudentsValue.Location = new Point(16, 40);
        lblStudentsSub.Text = "Học viên các khoá học";
        lblStudentsSub.Font = new Font("Segoe UI", 8.5f);
        lblStudentsSub.ForeColor = Color.FromArgb(71, 85, 105);
        lblStudentsSub.AutoSize = true;
        lblStudentsSub.Location = new Point(18, 96);
        cardStudents.Controls.Add(lblStudentsSub);
        cardStudents.Controls.Add(lblStudentsValue);
        cardStudents.Controls.Add(lblStudentsTitle);

        // Card 4: Teachers
        cardTeachers.Dock = DockStyle.Fill;
        cardTeachers.BackColor = Color.White;
        cardTeachers.Margin = new Padding(12, 0, 0, 0);
        lblTeachersTitle.Text = "Giáo viên & Trợ giảng";
        lblTeachersTitle.UseMnemonic = false;
        lblTeachersTitle.Font = new Font("Segoe UI", 9f);
        lblTeachersTitle.ForeColor = Color.FromArgb(100, 116, 139);
        lblTeachersTitle.AutoSize = true;
        lblTeachersTitle.Location = new Point(18, 16);
        lblTeachersValue.Text = "—";
        lblTeachersValue.Font = new Font("Segoe UI", 24f, FontStyle.Bold);
        lblTeachersValue.ForeColor = Color.FromArgb(245, 158, 11);
        lblTeachersValue.AutoSize = true;
        lblTeachersValue.Location = new Point(16, 40);
        lblTeachersSub.Text = "Giảng viên giàu kinh nghiệm";
        lblTeachersSub.Font = new Font("Segoe UI", 8.5f);
        lblTeachersSub.ForeColor = Color.FromArgb(71, 85, 105);
        lblTeachersSub.AutoSize = true;
        lblTeachersSub.Location = new Point(18, 96);
        cardTeachers.Controls.Add(lblTeachersSub);
        cardTeachers.Controls.Add(lblTeachersValue);
        cardTeachers.Controls.Add(lblTeachersTitle);

        tblCards.Controls.Add(cardAccounts, 0, 0);
        tblCards.Controls.Add(cardActiveAccounts, 1, 0);
        tblCards.Controls.Add(cardStudents, 2, 0);
        tblCards.Controls.Add(cardTeachers, 3, 0);

        // SPACER
        pnlSectionSpacer.Dock = DockStyle.Top;
        pnlSectionSpacer.Height = 18;

        // 4. MAIN SPLIT 2-COLUMN SECTION 
        tblMainSection.Dock = DockStyle.Top;
        tblMainSection.Height = 540;
        tblMainSection.ColumnCount = 2;
        tblMainSection.RowCount = 1;
        tblMainSection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
        tblMainSection.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
        tblMainSection.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        // LEFT COLUMN (62%)
        pnlLeftCol.Dock = DockStyle.Fill;
        pnlLeftCol.Margin = new Padding(0, 0, 10, 0);

        pnlQuickActions.Dock = DockStyle.Top;
        pnlQuickActions.Height = 188;
        pnlQuickActions.BackColor = Color.White;
        pnlQuickActions.Padding = new Padding(20, 16, 20, 16);

        lblQuickTitle.Text = "TRUY CẬP NHANH";
        lblQuickTitle.Font = new Font("Segoe UI", 11.5f, FontStyle.Bold);
        lblQuickTitle.ForeColor = Color.FromArgb(15, 23, 42);
        lblQuickTitle.AutoSize = true;
        lblQuickTitle.Location = new Point(18, 14);

        lblQuickDesc.Text = "Chọn nhanh nghiệp vụ bên dưới để chuyển đến màn hình tương ứng:";
        lblQuickDesc.Font = new Font("Segoe UI", 9f);
        lblQuickDesc.ForeColor = Color.FromArgb(100, 116, 139);
        lblQuickDesc.AutoSize = true;
        lblQuickDesc.Location = new Point(18, 44);

        tblQuickButtons.Location = new Point(18, 78);
        tblQuickButtons.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        tblQuickButtons.Height = 92;
        tblQuickButtons.ColumnCount = 2;
        tblQuickButtons.RowCount = 2;
        tblQuickButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tblQuickButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tblQuickButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tblQuickButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

        btnGoTuition.Text = "Quản lý học phí";
        btnGoTuition.Dock = DockStyle.Fill;
        btnGoTuition.Margin = new Padding(4);
        btnGoTuition.FlatStyle = FlatStyle.Flat;
        btnGoTuition.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnGoTuition.BackColor = Color.FromArgb(248, 250, 252);
        btnGoTuition.ForeColor = Color.FromArgb(30, 86, 240);
        btnGoTuition.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnGoTuition.TextAlign = ContentAlignment.MiddleLeft;
        btnGoTuition.Padding = new Padding(12, 0, 0, 0);
        btnGoTuition.Cursor = Cursors.Hand;

        btnGoStudents.Text = "Quản lý học viên";
        btnGoStudents.Dock = DockStyle.Fill;
        btnGoStudents.Margin = new Padding(4);
        btnGoStudents.FlatStyle = FlatStyle.Flat;
        btnGoStudents.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnGoStudents.BackColor = Color.FromArgb(248, 250, 252);
        btnGoStudents.ForeColor = Color.FromArgb(71, 85, 105);
        btnGoStudents.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnGoStudents.TextAlign = ContentAlignment.MiddleLeft;
        btnGoStudents.Padding = new Padding(12, 0, 0, 0);
        btnGoStudents.Cursor = Cursors.Hand;

        btnGoTeachers.Text = "Quản lý giáo viên";
        btnGoTeachers.Dock = DockStyle.Fill;
        btnGoTeachers.Margin = new Padding(4);
        btnGoTeachers.FlatStyle = FlatStyle.Flat;
        btnGoTeachers.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnGoTeachers.BackColor = Color.FromArgb(248, 250, 252);
        btnGoTeachers.ForeColor = Color.FromArgb(71, 85, 105);
        btnGoTeachers.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnGoTeachers.TextAlign = ContentAlignment.MiddleLeft;
        btnGoTeachers.Padding = new Padding(12, 0, 0, 0);
        btnGoTeachers.Cursor = Cursors.Hand;

        btnGoAccounts.Text = "Quản lý tài khoản";
        btnGoAccounts.Dock = DockStyle.Fill;
        btnGoAccounts.Margin = new Padding(4);
        btnGoAccounts.FlatStyle = FlatStyle.Flat;
        btnGoAccounts.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnGoAccounts.BackColor = Color.FromArgb(248, 250, 252);
        btnGoAccounts.ForeColor = Color.FromArgb(71, 85, 105);
        btnGoAccounts.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnGoAccounts.TextAlign = ContentAlignment.MiddleLeft;
        btnGoAccounts.Padding = new Padding(12, 0, 0, 0);
        btnGoAccounts.Cursor = Cursors.Hand;

        tblQuickButtons.Controls.Add(btnGoTuition, 0, 0);
        tblQuickButtons.Controls.Add(btnGoStudents, 1, 0);
        tblQuickButtons.Controls.Add(btnGoTeachers, 0, 1);
        tblQuickButtons.Controls.Add(btnGoAccounts, 1, 1);

        pnlQuickActions.Controls.Add(tblQuickButtons);
        pnlQuickActions.Controls.Add(lblQuickDesc);
        pnlQuickActions.Controls.Add(lblQuickTitle);

        pnlLeftSpacer.Dock = DockStyle.Top;
        pnlLeftSpacer.Height = 14;

        pnlFeaturedCourses.Dock = DockStyle.Fill;
        pnlFeaturedCourses.BackColor = Color.White;
        pnlFeaturedCourses.Padding = new Padding(20, 16, 20, 16);

        pnlCoursesHeader.Dock = DockStyle.Top;
        pnlCoursesHeader.Height = 50;

        lblCoursesTitle.Text = "CHƯƠNG TRÌNH ĐÀO TẠO TIÊU BIỂU";
        lblCoursesTitle.Font = new Font("Segoe UI", 11.5f, FontStyle.Bold);
        lblCoursesTitle.ForeColor = Color.FromArgb(15, 23, 42);
        lblCoursesTitle.AutoSize = true;
        lblCoursesTitle.Location = new Point(0, 6);

        lnkViewAllCourses.Text = "Xem toàn bộ";
        lnkViewAllCourses.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        lnkViewAllCourses.LinkColor = Color.FromArgb(30, 86, 240);
        lnkViewAllCourses.ActiveLinkColor = Color.FromArgb(23, 67, 197);
        lnkViewAllCourses.AutoSize = true;
        lnkViewAllCourses.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lnkViewAllCourses.Cursor = Cursors.Hand;
        lnkViewAllCourses.LinkBehavior = LinkBehavior.HoverUnderline;
        lnkViewAllCourses.Location = new Point(500, 8);

        pnlCoursesHeader.Controls.Add(lnkViewAllCourses);
        pnlCoursesHeader.Controls.Add(lblCoursesTitle);

        dgvFeaturedCourses.Dock = DockStyle.Fill;

        pnlFeaturedCourses.Controls.Add(dgvFeaturedCourses);
        pnlFeaturedCourses.Controls.Add(pnlCoursesHeader);

        pnlLeftCol.Controls.Add(pnlFeaturedCourses);
        pnlLeftCol.Controls.Add(pnlLeftSpacer);
        pnlLeftCol.Controls.Add(pnlQuickActions);

        // RIGHT COLUMN (38%)
        pnlRightCol.Dock = DockStyle.Fill;
        pnlRightCol.Margin = new Padding(10, 0, 0, 0);

        pnlNotices.Dock = DockStyle.Fill;
        pnlNotices.BackColor = Color.White;
        pnlNotices.Padding = new Padding(20, 16, 20, 16);

        lblNoticesTitle.Text = "THÔNG BÁO & NHẮC VIỆC";
        lblNoticesTitle.UseMnemonic = false;
        lblNoticesTitle.Font = new Font("Segoe UI", 11.5f, FontStyle.Bold);
        lblNoticesTitle.ForeColor = Color.FromArgb(15, 23, 42);
        lblNoticesTitle.AutoSize = true;
        lblNoticesTitle.Location = new Point(18, 14);

        // Notice 1
        pnlNoticeItem1.Location = new Point(18, 48);
        pnlNoticeItem1.Size = new Size(390, 72);
        pnlNoticeItem1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        pnlNoticeItem1.BackColor = Color.FromArgb(248, 250, 252);

        lblNoticeTag1.Text = "THI THỬ";
        lblNoticeTag1.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
        lblNoticeTag1.ForeColor = Color.White;
        lblNoticeTag1.BackColor = Color.FromArgb(245, 158, 11);
        lblNoticeTag1.TextAlign = ContentAlignment.MiddleCenter;
        lblNoticeTag1.Size = new Size(78, 22);
        lblNoticeTag1.Location = new Point(12, 10);

        lblNoticeTitle1.Text = "Kỳ thi thử IELTS Mock Test định kỳ";
        lblNoticeTitle1.Font = new Font("Segoe UI", 9.2f, FontStyle.Bold);
        lblNoticeTitle1.ForeColor = Color.FromArgb(15, 23, 42);
        lblNoticeTitle1.AutoSize = true;
        lblNoticeTitle1.Location = new Point(98, 11);

        lblNoticeDetail1.Text = "Diễn ra Thứ Bảy tuần này lúc 08:30 tại Hội trường A. Học viên mang theo CCCD.";
        lblNoticeDetail1.Font = new Font("Segoe UI", 8.2f);
        lblNoticeDetail1.ForeColor = Color.FromArgb(100, 116, 139);
        lblNoticeDetail1.Location = new Point(12, 36);
        lblNoticeDetail1.Size = new Size(365, 30);
        lblNoticeDetail1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        pnlNoticeItem1.Controls.Add(lblNoticeDetail1);
        pnlNoticeItem1.Controls.Add(lblNoticeTitle1);
        pnlNoticeItem1.Controls.Add(lblNoticeTag1);

        // Notice 2
        pnlNoticeItem2.Location = new Point(18, 132);
        pnlNoticeItem2.Size = new Size(390, 72);
        pnlNoticeItem2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        pnlNoticeItem2.BackColor = Color.FromArgb(248, 250, 252);

        lblNoticeTag2.Text = "ƯU ĐÃI";
        lblNoticeTag2.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
        lblNoticeTag2.ForeColor = Color.White;
        lblNoticeTag2.BackColor = Color.FromArgb(30, 86, 240);
        lblNoticeTag2.TextAlign = ContentAlignment.MiddleCenter;
        lblNoticeTag2.Size = new Size(78, 22);
        lblNoticeTag2.Location = new Point(12, 10);

        lblNoticeTitle2.Text = "Chính sách giảm giá 30% học phí";
        lblNoticeTitle2.Font = new Font("Segoe UI", 9.2f, FontStyle.Bold);
        lblNoticeTitle2.ForeColor = Color.FromArgb(15, 23, 42);
        lblNoticeTitle2.AutoSize = true;
        lblNoticeTitle2.Location = new Point(98, 11);

        lblNoticeDetail2.Text = "Áp dụng cho học viên đăng ký theo nhóm từ 3 bạn và thanh toán trước ngày 25.";
        lblNoticeDetail2.Font = new Font("Segoe UI", 8.2f);
        lblNoticeDetail2.ForeColor = Color.FromArgb(100, 116, 139);
        lblNoticeDetail2.Location = new Point(12, 36);
        lblNoticeDetail2.Size = new Size(365, 30);
        lblNoticeDetail2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        pnlNoticeItem2.Controls.Add(lblNoticeDetail2);
        pnlNoticeItem2.Controls.Add(lblNoticeTitle2);
        pnlNoticeItem2.Controls.Add(lblNoticeTag2);

        // Notice 3
        pnlNoticeItem3.Location = new Point(18, 216);
        pnlNoticeItem3.Size = new Size(390, 72);
        pnlNoticeItem3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        pnlNoticeItem3.BackColor = Color.FromArgb(248, 250, 252);

        lblNoticeTag3.Text = "BẢO MẬT";
        lblNoticeTag3.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
        lblNoticeTag3.ForeColor = Color.White;
        lblNoticeTag3.BackColor = Color.FromArgb(16, 185, 129);
        lblNoticeTag3.TextAlign = ContentAlignment.MiddleCenter;
        lblNoticeTag3.Size = new Size(78, 22);
        lblNoticeTag3.Location = new Point(12, 10);

        lblNoticeTitle3.Text = "Hệ thống sao lưu tự động an toàn";
        lblNoticeTitle3.Font = new Font("Segoe UI", 9.2f, FontStyle.Bold);
        lblNoticeTitle3.ForeColor = Color.FromArgb(15, 23, 42);
        lblNoticeTitle3.AutoSize = true;
        lblNoticeTitle3.Location = new Point(98, 11);

        lblNoticeDetail3.Text = "Cơ sở dữ liệu đám mây đã hoàn tất sao lưu 24/7 lúc 03:00. Dữ liệu mã hóa hoàn toàn.";
        lblNoticeDetail3.Font = new Font("Segoe UI", 8.2f);
        lblNoticeDetail3.ForeColor = Color.FromArgb(100, 116, 139);
        lblNoticeDetail3.Location = new Point(12, 36);
        lblNoticeDetail3.Size = new Size(365, 30);
        lblNoticeDetail3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        pnlNoticeItem3.Controls.Add(lblNoticeDetail3);
        pnlNoticeItem3.Controls.Add(lblNoticeTitle3);
        pnlNoticeItem3.Controls.Add(lblNoticeTag3);

        // Notice 4
        pnlNoticeItem4.Location = new Point(18, 300);
        pnlNoticeItem4.Size = new Size(390, 72);
        pnlNoticeItem4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        pnlNoticeItem4.BackColor = Color.FromArgb(248, 250, 252);

        lblNoticeTag4.Text = "LỊCH HỌC";
        lblNoticeTag4.Font = new Font("Segoe UI", 8f, FontStyle.Bold);
        lblNoticeTag4.ForeColor = Color.White;
        lblNoticeTag4.BackColor = Color.FromArgb(14, 165, 233);
        lblNoticeTag4.TextAlign = ContentAlignment.MiddleCenter;
        lblNoticeTag4.Size = new Size(78, 22);
        lblNoticeTag4.Location = new Point(12, 10);

        lblNoticeTitle4.Text = "Cập nhật lịch khai giảng khóa mới";
        lblNoticeTitle4.Font = new Font("Segoe UI", 9.2f, FontStyle.Bold);
        lblNoticeTitle4.ForeColor = Color.FromArgb(15, 23, 42);
        lblNoticeTitle4.AutoSize = true;
        lblNoticeTitle4.Location = new Point(98, 11);

        lblNoticeDetail4.Text = "Đã công bố thời khóa biểu và danh sách lớp học tiếng Anh giao tiếp tháng tới.";
        lblNoticeDetail4.Font = new Font("Segoe UI", 8.2f);
        lblNoticeDetail4.ForeColor = Color.FromArgb(100, 116, 139);
        lblNoticeDetail4.Location = new Point(12, 36);
        lblNoticeDetail4.Size = new Size(365, 30);
        lblNoticeDetail4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        pnlNoticeItem4.Controls.Add(lblNoticeDetail4);
        pnlNoticeItem4.Controls.Add(lblNoticeTitle4);
        pnlNoticeItem4.Controls.Add(lblNoticeTag4);

        pnlNotices.Controls.Add(pnlNoticeItem4);
        pnlNotices.Controls.Add(pnlNoticeItem3);
        pnlNotices.Controls.Add(pnlNoticeItem2);
        pnlNotices.Controls.Add(pnlNoticeItem1);
        pnlNotices.Controls.Add(lblNoticesTitle);

        pnlRightCol.Controls.Add(pnlNotices);

        tblMainSection.Controls.Add(pnlLeftCol, 0, 0);
        tblMainSection.Controls.Add(pnlRightCol, 1, 0);

        pnlBottomPadding.Dock = DockStyle.Top;
        pnlBottomPadding.Height = 24;

        Controls.Add(pnlBottomPadding);
        Controls.Add(tblMainSection);
        Controls.Add(pnlSectionSpacer);
        Controls.Add(tblCards);
        Controls.Add(pnlStatsHeader);
        Controls.Add(pnlHero);

        pnlHero.ResumeLayout(false);
        pnlHero.PerformLayout();
        pnlHeroRight.ResumeLayout(false);
        pnlHeroRight.PerformLayout();
        pnlStatsHeader.ResumeLayout(false);
        pnlStatsHeader.PerformLayout();
        tblCards.ResumeLayout(false);
        cardAccounts.ResumeLayout(false);
        cardAccounts.PerformLayout();
        cardActiveAccounts.ResumeLayout(false);
        cardActiveAccounts.PerformLayout();
        cardStudents.ResumeLayout(false);
        cardStudents.PerformLayout();
        cardTeachers.ResumeLayout(false);
        cardTeachers.PerformLayout();
        tblMainSection.ResumeLayout(false);
        pnlLeftCol.ResumeLayout(false);
        pnlQuickActions.ResumeLayout(false);
        pnlQuickActions.PerformLayout();
        tblQuickButtons.ResumeLayout(false);
        pnlFeaturedCourses.ResumeLayout(false);
        pnlCoursesHeader.ResumeLayout(false);
        pnlCoursesHeader.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvFeaturedCourses).EndInit();
        pnlRightCol.ResumeLayout(false);
        pnlNotices.ResumeLayout(false);
        pnlNotices.PerformLayout();
        pnlNoticeItem1.ResumeLayout(false);
        pnlNoticeItem1.PerformLayout();
        pnlNoticeItem2.ResumeLayout(false);
        pnlNoticeItem2.PerformLayout();
        pnlNoticeItem3.ResumeLayout(false);
        pnlNoticeItem3.PerformLayout();
        pnlNoticeItem4.ResumeLayout(false);
        pnlNoticeItem4.PerformLayout();
        ResumeLayout(false);
    }
}
