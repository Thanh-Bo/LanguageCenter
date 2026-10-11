namespace LanguageCenter.WinForms.UserControls;

partial class UcDashboard
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
    private Label lblAccountsValue = null!;
    private Label lblActiveAccountsValue = null!;
    private Label lblStudentsValue = null!;
    private Label lblTeachersValue = null!;

    // Spacer
    private Panel pnlSectionSpacer = null!;

    // Section 2: Main Split 2-Column Section 
    private TableLayoutPanel tblMainSection = null!;

    // Left Column (62%)
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

    // Right Column (38%)
    private Panel pnlRightCol = null!;
    private Panel pnlNotices = null!;
    private Label lblNoticesTitle = null!;
    private Panel pnlNoticeItem1 = null!;
    private Panel pnlNoticeItem2 = null!;
    private Panel pnlNoticeItem3 = null!;
    private Panel pnlNoticeItem4 = null!;

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
        cardActiveAccounts = new Panel();
        cardStudents = new Panel();
        cardTeachers = new Panel();
        lblAccountsValue = new Label();
        lblActiveAccountsValue = new Label();
        lblStudentsValue = new Label();
        lblTeachersValue = new Label();

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
        pnlNoticeItem2 = new Panel();
        pnlNoticeItem3 = new Panel();
        pnlNoticeItem4 = new Panel();

        pnlBottomPadding = new Panel();

        SuspendLayout();

        // UcDashboard
        Dock = DockStyle.Fill;
        AutoScroll = true;
        BackColor = UI.Theme.ContentBackground;

        // 1. HERO BANNER (Full width, gradient vibrant royal blue)
        pnlHero.Dock = DockStyle.Top;
        pnlHero.Height = 125;
        pnlHero.BackColor = UI.Theme.Primary;
        pnlHero.Padding = new Padding(24, 16, 24, 16);

        // pnlHeroRight (Chứa ngày tháng, trạng thái server và nút refresh)
        pnlHeroRight.Dock = DockStyle.Right;
        pnlHeroRight.Width = 240;
        pnlHeroRight.BackColor = Color.Transparent;

        lblHeroClock.Text = $" {DateTime.Now:dd/MM/yyyy}";
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
        btnRefresh.ForeColor = UI.Theme.Primary;
        btnRefresh.BackColor = Color.White;
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.FlatAppearance.BorderSize = 0;
        btnRefresh.Cursor = Cursors.Hand;
        btnRefresh.Size = new Size(130, 34);
        btnRefresh.Location = new Point(10, 58);

        pnlHeroRight.Controls.Add(btnRefresh);
        pnlHeroRight.Controls.Add(lblHeroServer);
        pnlHeroRight.Controls.Add(lblHeroClock);

        // Hero Left Controls (Chỉ giữ lại 2 mục: Badge và Lời chào)
        lblHeroBadge.Text = $"{Api.AppConfig.BrandShortName} ACADEMY — HỆ THỐNG QUẢN TRỊ ĐÀO TẠO & HỌC VIÊN";
        lblHeroBadge.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        lblHeroBadge.ForeColor = Color.FromArgb(219, 234, 254);
        lblHeroBadge.AutoSize = true;
        lblHeroBadge.Location = new Point(20, 18);

        lblHeroTitle.Text = $"Chào mừng bạn đến với {Api.AppConfig.BrandName}";
        lblHeroTitle.Font = new Font("Segoe UI", 16f, FontStyle.Bold);
        lblHeroTitle.ForeColor = Color.White;
        lblHeroTitle.AutoSize = true;
        lblHeroTitle.Location = new Point(19, 46);

        pnlHero.Controls.Add(pnlHeroRight);
        pnlHero.Controls.Add(lblHeroTitle);
        pnlHero.Controls.Add(lblHeroBadge);

        // 2. STATS HEADER ROW (FlowLayoutPanel
        pnlStatsHeader.Dock = DockStyle.Top;
        pnlStatsHeader.Height = 44;
        pnlStatsHeader.WrapContents = false;
        pnlStatsHeader.FlowDirection = FlowDirection.LeftToRight;
        pnlStatsHeader.Padding = new Padding(2, 10, 0, 0);

        lblStatsHeaderTitle.Text = "CHỈ SỐ HOẠT ĐỘNG TOÀN HỆ THỐNG";
        lblStatsHeaderTitle.Font = UI.Theme.FontHeading;
        lblStatsHeaderTitle.ForeColor = UI.Theme.TextDark;
        lblStatsHeaderTitle.AutoSize = true;
        lblStatsHeaderTitle.Margin = new Padding(0, 2, 16, 0);

        lblLoading.Text = "";
        lblLoading.Font = UI.Theme.FontRegular;
        lblLoading.ForeColor = UI.Theme.Muted;
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

        BuildStatMetricCard(cardAccounts, "Tổng tài khoản hệ thống", "", lblAccountsValue, UI.Theme.Primary, "Quản trị viên & Cán bộ");
        BuildStatMetricCard(cardActiveAccounts, "Tài khoản đang hoạt động", "", lblActiveAccountsValue, UI.Theme.Success, " 100% Đang kích hoạt");
        BuildStatMetricCard(cardStudents, "Hồ sơ học viên", "", lblStudentsValue, UI.Theme.Purple, "Học viên các khoá học");
        BuildStatMetricCard(cardTeachers, "Giáo viên & Trợ giảng", "", lblTeachersValue, UI.Theme.Warning, "Giảng viên chuẩn CELTA/TESOL");

        cardAccounts.Dock = DockStyle.Fill;
        cardAccounts.Margin = new Padding(0, 0, 12, 0);

        cardActiveAccounts.Dock = DockStyle.Fill;
        cardActiveAccounts.Margin = new Padding(6, 0, 6, 0);

        cardStudents.Dock = DockStyle.Fill;
        cardStudents.Margin = new Padding(6, 0, 6, 0);

        cardTeachers.Dock = DockStyle.Fill;
        cardTeachers.Margin = new Padding(12, 0, 0, 0);

        tblCards.Controls.Add(cardAccounts, 0, 0);
        tblCards.Controls.Add(cardActiveAccounts, 1, 0);
        tblCards.Controls.Add(cardStudents, 2, 0);
        tblCards.Controls.Add(cardTeachers, 3, 0);

        // SPACER GIỮA CHỈ SỐ VÀ KHU VỰC CHÍNH
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

        // CỘT TRÁI (62%): Truy cập nhanh + Danh sách khóa học tiêu biểu
        pnlLeftCol.Dock = DockStyle.Fill;
        pnlLeftCol.Margin = new Padding(0, 0, 10, 0);

        // Card Truy Cập Nhanh
        pnlQuickActions.Dock = DockStyle.Top;
        pnlQuickActions.Height = 175;
        pnlQuickActions.BackColor = Color.White;
        pnlQuickActions.Padding = new Padding(20, 16, 20, 16);
        pnlQuickActions.Paint += UI.UiHelpers.PaintCardBorder;

        lblQuickTitle.Text = " TRUY CẬP NHANH CÁC PHÂN HỆ";
        lblQuickTitle.Font = UI.Theme.FontHeading;
        lblQuickTitle.ForeColor = UI.Theme.TextDark;
        lblQuickTitle.AutoSize = true;
        lblQuickTitle.Location = new Point(18, 14);

        lblQuickDesc.Text = "Chọn phân hệ nghiệp vụ bên dưới để chuyển nhanh đến màn hình quản lý tương ứng:";
        lblQuickDesc.Font = new Font("Segoe UI", 9f);
        lblQuickDesc.ForeColor = UI.Theme.Muted;
        lblQuickDesc.AutoSize = true;
        lblQuickDesc.Location = new Point(20, 38);

        tblQuickButtons.Location = new Point(18, 66);
        tblQuickButtons.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        tblQuickButtons.Height = 90;
        tblQuickButtons.ColumnCount = 2;
        tblQuickButtons.RowCount = 2;
        tblQuickButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tblQuickButtons.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tblQuickButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tblQuickButtons.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

        ConfigureQuickButton(btnGoTuition, "Quản lý học phí", UI.Theme.Primary);
        ConfigureQuickButton(btnGoStudents, "Quản lý học viên", Color.FromArgb(71, 85, 105));
        ConfigureQuickButton(btnGoTeachers, "Quản lý giáo viên", Color.FromArgb(71, 85, 105));
        ConfigureQuickButton(btnGoAccounts, "Quản lý tài khoản", Color.FromArgb(71, 85, 105));

        tblQuickButtons.Controls.Add(btnGoTuition, 0, 0);
        tblQuickButtons.Controls.Add(btnGoStudents, 1, 0);
        tblQuickButtons.Controls.Add(btnGoTeachers, 0, 1);
        tblQuickButtons.Controls.Add(btnGoAccounts, 1, 1);

        pnlQuickActions.Controls.Add(tblQuickButtons);
        pnlQuickActions.Controls.Add(lblQuickDesc);
        pnlQuickActions.Controls.Add(lblQuickTitle);

        pnlLeftSpacer.Dock = DockStyle.Top;
        pnlLeftSpacer.Height = 14;

        // Card Khóa Học Tiêu Biểu
        pnlFeaturedCourses.Dock = DockStyle.Fill;
        pnlFeaturedCourses.BackColor = Color.White;
        pnlFeaturedCourses.Padding = new Padding(20, 16, 20, 16);
        pnlFeaturedCourses.Paint += UI.UiHelpers.PaintCardBorder;

        pnlCoursesHeader.Dock = DockStyle.Top;
        pnlCoursesHeader.Height = 36;

        lblCoursesTitle.Text = "CHƯƠNG TRÌNH ĐÀO TẠO TIÊU BIỂU";
        lblCoursesTitle.Font = UI.Theme.FontHeading;
        lblCoursesTitle.ForeColor = UI.Theme.TextDark;
        lblCoursesTitle.AutoSize = true;
        lblCoursesTitle.Location = new Point(0, 4);

        lnkViewAllCourses.Text = "Xem toàn bộ khóa học & chương trình";
        lnkViewAllCourses.Font = UI.Theme.FontBold;
        lnkViewAllCourses.LinkColor = UI.Theme.Primary;
        lnkViewAllCourses.ActiveLinkColor = UI.Theme.PrimaryDark;
        lnkViewAllCourses.AutoSize = true;
        lnkViewAllCourses.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lnkViewAllCourses.Cursor = Cursors.Hand;
        lnkViewAllCourses.LinkBehavior = LinkBehavior.HoverUnderline;

        pnlCoursesHeader.Controls.Add(lnkViewAllCourses);
        pnlCoursesHeader.Controls.Add(lblCoursesTitle);
        pnlCoursesHeader.Resize += (_, _) =>
        {
            lnkViewAllCourses.Location = new Point(pnlCoursesHeader.Width - lnkViewAllCourses.Width - 4, 6);
        };

        dgvFeaturedCourses.Dock = DockStyle.Fill;
        UI.UiHelpers.StyleGrid(dgvFeaturedCourses);

        pnlFeaturedCourses.Controls.Add(dgvFeaturedCourses);
        pnlFeaturedCourses.Controls.Add(pnlCoursesHeader);

        pnlLeftCol.Controls.Add(pnlFeaturedCourses);
        pnlLeftCol.Controls.Add(pnlLeftSpacer);
        pnlLeftCol.Controls.Add(pnlQuickActions);

        // CỘT PHẢI (38%): Thông báo vận hành & Nhắc việc
        pnlRightCol.Dock = DockStyle.Fill;
        pnlRightCol.Margin = new Padding(10, 0, 0, 0);

        // Card Thông Báo Vận Hành (Dock Fill toàn bộ cột phải)
        pnlNotices.Dock = DockStyle.Fill;
        pnlNotices.BackColor = Color.White;
        pnlNotices.Padding = new Padding(20, 16, 20, 16);
        pnlNotices.Paint += UI.UiHelpers.PaintCardBorder;

        lblNoticesTitle.Text = " THÔNG BÁO VẬN HÀNH & NHẮC VIỆC";
        lblNoticesTitle.Font = UI.Theme.FontHeading;
        lblNoticesTitle.ForeColor = UI.Theme.TextDark;
        lblNoticesTitle.AutoSize = true;
        lblNoticesTitle.Location = new Point(18, 14);

        BuildNoticeItem(pnlNoticeItem1, "THI THỬ", UI.Theme.Warning,
            "Kỳ thi thử IELTS Mock Test định kỳ",
            "Diễn ra Thứ Bảy tuần này lúc 08:30 tại Hội trường A. Học viên mang theo CCCD.", 48);

        BuildNoticeItem(pnlNoticeItem2, "ƯU ĐÃI", UI.Theme.Primary,
            "Chính sách giảm giá 30% học phí",
            "Áp dụng cho học viên đăng ký theo nhóm từ 3 bạn và thanh toán trước ngày 25.", 132);

        BuildNoticeItem(pnlNoticeItem3, "BẢO MẬT", UI.Theme.Success,
            "Hệ thống sao lưu tự động an toàn",
            "Cơ sở dữ liệu đám mây đã hoàn tất sao lưu 24/7 lúc 03:00. Dữ liệu mã hóa hoàn toàn.", 216);

        BuildNoticeItem(pnlNoticeItem4, "LỊCH HỌC", UI.Theme.Info,
            "Cập nhật lịch khai giảng khóa mới",
            "Đã công bố thời khóa biểu và danh sách lớp học tiếng Anh giao tiếp tháng tới.", 300);

        pnlNotices.Controls.Add(pnlNoticeItem4);
        pnlNotices.Controls.Add(pnlNoticeItem3);
        pnlNotices.Controls.Add(pnlNoticeItem2);
        pnlNoticeItem1.BringToFront();
        pnlNotices.Controls.Add(pnlNoticeItem1);
        pnlNotices.Controls.Add(lblNoticesTitle);

        pnlRightCol.Controls.Add(pnlNotices);

        tblMainSection.Controls.Add(pnlLeftCol, 0, 0);
        tblMainSection.Controls.Add(pnlRightCol, 1, 0);

        // Khoảng đệm chân trang
        pnlBottomPadding.Dock = DockStyle.Top;
        pnlBottomPadding.Height = 24;

        // Add controls in reverse order for correct WinForms Top docking
        Controls.Add(pnlBottomPadding);
        Controls.Add(tblMainSection);
        Controls.Add(pnlSectionSpacer);
        Controls.Add(tblCards);
        Controls.Add(pnlStatsHeader);
        Controls.Add(pnlHero);

        ResumeLayout(false);
    }

    private static void BuildStatMetricCard(Panel card, string title, string icon, Label valueLabel, Color accent, string subtext)
    {
        card.BackColor = Color.White;
        card.Paint += (s, e) =>
        {
            if (s is Control ctrl)
            {
                using var pen = new Pen(UI.Theme.BorderColor, 1);
                e.Graphics.DrawRectangle(pen, 0, 0, ctrl.Width - 1, ctrl.Height - 1);

                using var brush = new SolidBrush(accent);
                e.Graphics.FillRectangle(brush, 0, 0, 4, ctrl.Height);
            }
        };

        var lblIcon = new Label
        {
            Text = icon,
            Font = new Font("Segoe UI Emoji", 14f),
            AutoSize = true,
            Anchor = AnchorStyles.Top | AnchorStyles.Right,
            Location = new Point(card.Width - 42, 16)
        };

        var lblTitle = new Label
        {
            Text = title,
            Font = new Font("Segoe UI", 9f),
            ForeColor = UI.Theme.Muted,
            AutoSize = true,
            Location = new Point(18, 16)
        };

        valueLabel.Text = "—";
        valueLabel.Font = new Font("Segoe UI", 24f, FontStyle.Bold);
        valueLabel.ForeColor = accent;
        valueLabel.AutoSize = true;
        valueLabel.Location = new Point(16, 40);

        var lblSub = new Label
        {
            Text = subtext,
            Font = new Font("Segoe UI", 8.3f),
            ForeColor = UI.Theme.TextMedium,
            AutoSize = true,
            Location = new Point(18, 96)
        };

        card.Controls.Add(lblSub);
        card.Controls.Add(valueLabel);
        card.Controls.Add(lblTitle);
        card.Controls.Add(lblIcon);

        card.Resize += (_, _) =>
        {
            lblIcon.Location = new Point(card.Width - 45, 16);
        };
    }

    private static void ConfigureQuickButton(Button btn, string text, Color textColor)
    {
        btn.Text = text;
        btn.Dock = DockStyle.Fill;
        btn.Margin = new Padding(4);
        btn.FlatStyle = FlatStyle.Flat;
        btn.FlatAppearance.BorderSize = 1;
        btn.FlatAppearance.BorderColor = UI.Theme.BorderColor;
        btn.BackColor = Color.FromArgb(248, 250, 252);
        btn.ForeColor = textColor;
        btn.Font = UI.Theme.FontBold;
        btn.TextAlign = ContentAlignment.MiddleLeft;
        btn.Padding = new Padding(12, 0, 0, 0);
        btn.Cursor = Cursors.Hand;
        btn.FlatAppearance.MouseOverBackColor = UI.Theme.PrimaryLight;
    }

    private static void BuildNoticeItem(Panel panel, string tag, Color tagColor, string title, string detail, int top)
    {
        panel.Location = new Point(18, top);
        panel.Size = new Size(390, 72);
        panel.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        panel.BackColor = Color.FromArgb(248, 250, 252);

        panel.Paint += (s, e) =>
        {
            if (s is Control ctrl)
            {
                using var pen = new Pen(Color.FromArgb(235, 238, 245), 1);
                e.Graphics.DrawRectangle(pen, 0, 0, ctrl.Width - 1, ctrl.Height - 1);

                using var brush = new SolidBrush(tagColor);
                e.Graphics.FillRectangle(brush, 0, 0, 3, ctrl.Height);
            }
        };

        var lblTag = new Label
        {
            Text = tag,
            Font = new Font("Segoe UI", 8f, FontStyle.Bold),
            ForeColor = Color.White,
            BackColor = tagColor,
            TextAlign = ContentAlignment.MiddleCenter,
            Size = new Size(78, 22),
            Location = new Point(12, 10)
        };

        var lblTitle = new Label
        {
            Text = title,
            Font = new Font("Segoe UI", 9.2f, FontStyle.Bold),
            ForeColor = UI.Theme.TextDark,
            AutoSize = true,
            Location = new Point(98, 11)
        };

        var lblDetail = new Label
        {
            Text = detail,
            Font = new Font("Segoe UI", 8.2f),
            ForeColor = UI.Theme.Muted,
            AutoSize = false,
            Location = new Point(12, 36),
            Size = new Size(365, 30),
            Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
        };

        panel.Controls.Add(lblDetail);
        panel.Controls.Add(lblTitle);
        panel.Controls.Add(lblTag);
    }
}
