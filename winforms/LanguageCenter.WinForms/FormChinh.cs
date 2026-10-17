using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UserControls;

namespace LanguageCenter.WinForms;

public enum NavItem
{
    Dashboard,
    Courses,
    Classes,
    Assignments,
    Enrollment,
    RoomsSchedules,
    Grades,
    Tuition,
    Students,
    Teachers,
    Assistants,
    Notifications,
    Reports,
    Accounts
}

public partial class FormChinh : Form
{
    private UserControl? _currentView;

    public FormChinh()
    {
        InitializeComponent();
        WireEvents();
    }

    private void WireEvents()
    {
        btnNavDashboard.Click += (_, _) => NavigateTo(NavItem.Dashboard);
        btnNavCourses.Click += (_, _) => NavigateTo(NavItem.Courses);
        btnNavClasses.Click += (_, _) => NavigateTo(NavItem.Classes);
        btnNavAssignments.Click += (_, _) => NavigateTo(NavItem.Assignments);
        btnNavEnrollment.Click += (_, _) => NavigateTo(NavItem.Enrollment);
        btnNavRoomsSchedules.Click += (_, _) => NavigateTo(NavItem.RoomsSchedules);
        btnNavGrades.Click += (_, _) => NavigateTo(NavItem.Grades);
        btnNavTuition.Click += (_, _) => NavigateTo(NavItem.Tuition);
        btnNavStudents.Click += (_, _) => NavigateTo(NavItem.Students);
        btnNavTeachers.Click += (_, _) => NavigateTo(NavItem.Teachers);
        btnNavAssistants.Click += (_, _) => NavigateTo(NavItem.Assistants);
        btnNavNotifications.Click += (_, _) => NavigateTo(NavItem.Notifications);
        btnNavReports.Click += (_, _) => NavigateTo(NavItem.Reports);
        btnNavAccounts.Click += (_, _) => NavigateTo(NavItem.Accounts);

        pnlHeader.Paint += (_, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240));
            e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
        };
        pnlHeader.Resize += (_, _) => AlignHeaderLabels();
        lblCurrentUser.TextChanged += (_, _) => AlignHeaderLabels();
        Shown += (_, _) => AlignHeaderLabels();
    }

    private void AlignHeaderLabels()
    {
        if (pnlHeader is not null && lblCurrentUser is not null)
        {
            lblCurrentUser.Location = new Point(pnlHeader.ClientSize.Width - lblCurrentUser.Width - 28, (pnlHeader.ClientSize.Height - lblCurrentUser.Height) / 2);
        }
    }

    private void FormMain_Load(object? sender, EventArgs e)
    {
        var user = AppSession.CurrentUser;
        var role = user?.Role ?? RoleNames.QuanTriVien;

        if (role == RoleNames.GiaoVien)
        {
            lblCurrentUser.Text = $"{user?.Username} (Giáo viên)";
        }
        else if (role == RoleNames.TroGiang)
        {
            lblCurrentUser.Text = $"{user?.Username} (Trợ giảng)";
        }
        else
        {
            lblCurrentUser.Text = $"{user?.Username ?? "ADMIN001"} (Quản trị viên)";
        }

        AlignHeaderLabels();
        ApplyRolePermissions(role);
        NavigateTo(NavItem.Dashboard);
    }

    private void ApplyRolePermissions(string role)
    {
        var allNavItems = new List<(Button Button, NavItem Item)>
        {
            (btnNavDashboard, NavItem.Dashboard),
            (btnNavCourses, NavItem.Courses),
            (btnNavClasses, NavItem.Classes),
            (btnNavAssignments, NavItem.Assignments),
            (btnNavEnrollment, NavItem.Enrollment),
            (btnNavRoomsSchedules, NavItem.RoomsSchedules),
            (btnNavGrades, NavItem.Grades),
            (btnNavTuition, NavItem.Tuition),
            (btnNavStudents, NavItem.Students),
            (btnNavTeachers, NavItem.Teachers),
            (btnNavAssistants, NavItem.Assistants),
            (btnNavNotifications, NavItem.Notifications),
            (btnNavReports, NavItem.Reports),
            (btnNavAccounts, NavItem.Accounts)
        };

        int top = 4;
        const int btnHeight = 40;
        const int btnGap = 2;

        foreach (var (btn, item) in allNavItems)
        {
            bool isAllowed = IsItemAllowedForRole(item, role);
            btn.Visible = isAllowed;
            if (isAllowed)
            {
                btn.Location = new Point(0, top);
                top += btnHeight + btnGap;
            }
        }
    }

    private static bool IsItemAllowedForRole(NavItem item, string role)
    {
        // Quản trị viên hệ thống có toàn quyền xem và quản lý tất cả các phân hệ
        if (role == RoleNames.QuanTriVien)
        {
            return true;
        }

        // Giáo viên và Trợ giảng: Tuyệt đối KHÔNG có quyền truy cập vào các chức năng quản lý nhân sự, tài chính, tài khoản
        if (role == RoleNames.GiaoVien || role == RoleNames.TroGiang)
        {
            return item switch
            {
                NavItem.Teachers => false,      // Giáo viên & Trợ giảng KHÔNG quản lý giáo viên
                NavItem.Assistants => false,    // Giáo viên & Trợ giảng KHÔNG quản lý trợ giảng
                NavItem.Accounts => false,      // KHÔNG quản lý tài khoản người dùng
                NavItem.Tuition => false,       // KHÔNG quản lý học phí & biên nhận
                NavItem.Reports => false,       // KHÔNG xem báo cáo doanh thu tài chính
                NavItem.Assignments => false,   // KHÔNG phân công giảng dạy
                NavItem.Enrollment => false,    // KHÔNG xếp học viên vào lớp
                NavItem.Courses => false,       // KHÔNG quản lý chương trình/học phí khóa học
                _ => true                       // Được phép: Dashboard, Lớp học, Lịch học, Điểm số, Học viên, Thông báo
            };
        }

        return true;
    }

    public void NavigateTo(NavItem item)
    {
        var role = AppSession.CurrentUser?.Role ?? RoleNames.QuanTriVien;

        // Phân quyền bảo mật theo vai trò
        if (!IsItemAllowedForRole(item, role))
        {
            MessageBox.Show(
                this,
                $"TRUY CẬP BỊ TỪ CHỐI!\n\n" +
                $"Phân hệ này thuộc quyền hạn quản trị trung tâm.\n" +
                $"Tài khoản hiện tại ({AppSession.CurrentUser?.Username} — vai trò: {role}) không có thẩm quyền truy cập.",
                "Phân quyền bảo mật",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        HighlightActiveButton(item);

        _currentView?.Dispose();
        pnlContent.Controls.Clear();

        UserControl view = item switch
        {
            NavItem.Dashboard => new UcTongQuan(this),
            NavItem.Courses => new UcKhoaHoc(),
            NavItem.Classes => new UcLopHoc(),
            NavItem.Assignments => new UcPhanCongGiangDay(),
            NavItem.Enrollment => new UcXepLop(),
            NavItem.RoomsSchedules => new UcLichHoc(),
            NavItem.Grades => new UcDiemSo(),
            NavItem.Tuition => new UcHocPhi(),
            NavItem.Students => new UcHocVien(),
            NavItem.Teachers => new UcGiaoVien(),
            NavItem.Assistants => new UcTroGiang(),
            NavItem.Notifications => new UcThongBao(),
            NavItem.Reports => new UcBaoCao(),
            NavItem.Accounts => new UcTaiKhoan(),
            _ => new UcTongQuan(this)
        };

        lblPageTitle.Text = item switch
        {
            NavItem.Dashboard => "Tổng quan hệ thống đào tạo",
            NavItem.Courses => "Quản lý chương trình học",
            NavItem.Classes => "Quản lý danh sách lớp học & sĩ số",
            NavItem.Assignments => "Phân công giảng dạy",
            NavItem.Enrollment => "Xếp học viên vào lớp học",
            NavItem.RoomsSchedules => "Quản lý lịch học",
            NavItem.Grades => "Quản lý điểm số & bảng điểm",
            NavItem.Tuition => "Quản lý học phí",
            NavItem.Students => "Quản lý hồ sơ học viên",
            NavItem.Teachers => "Quản lý hồ sơ giáo viên",
            NavItem.Assistants => "Quản lý hồ sơ trợ giảng",
            NavItem.Notifications => "Quản lý thông báo & tin tức nội bộ",
            NavItem.Reports => "Báo cáo thống kê & tài chính",
            NavItem.Accounts => "Quản lý tài khoản hệ thống",
            _ => string.Empty
        };

        view.Dock = DockStyle.Fill;
        pnlContent.Controls.Add(view);
        _currentView = view;
    }

    private void HighlightActiveButton(NavItem item)
    {
        ResetNavButton(btnNavDashboard);
        ResetNavButton(btnNavCourses);
        ResetNavButton(btnNavClasses);
        ResetNavButton(btnNavAssignments);
        ResetNavButton(btnNavEnrollment);
        ResetNavButton(btnNavRoomsSchedules);
        ResetNavButton(btnNavGrades);
        ResetNavButton(btnNavTuition);
        ResetNavButton(btnNavStudents);
        ResetNavButton(btnNavTeachers);
        ResetNavButton(btnNavAssistants);
        ResetNavButton(btnNavNotifications);
        ResetNavButton(btnNavReports);
        ResetNavButton(btnNavAccounts);

        var active = item switch
        {
            NavItem.Dashboard => btnNavDashboard,
            NavItem.Courses => btnNavCourses,
            NavItem.Classes => btnNavClasses,
            NavItem.Assignments => btnNavAssignments,
            NavItem.Enrollment => btnNavEnrollment,
            NavItem.RoomsSchedules => btnNavRoomsSchedules,
            NavItem.Grades => btnNavGrades,
            NavItem.Tuition => btnNavTuition,
            NavItem.Students => btnNavStudents,
            NavItem.Teachers => btnNavTeachers,
            NavItem.Assistants => btnNavAssistants,
            NavItem.Notifications => btnNavNotifications,
            NavItem.Reports => btnNavReports,
            NavItem.Accounts => btnNavAccounts,
            _ => btnNavDashboard
        };

        active.BackColor = UI.Theme.SidebarButtonActive;
        active.ForeColor = Color.White;
        active.Font = UI.Theme.FontBold;
    }

    private static void ResetNavButton(Button button)
    {
        button.BackColor = UI.Theme.SidebarBackground;
        button.ForeColor = UI.Theme.SidebarText;
        button.Font = UI.Theme.FontRegular;
    }

    private void BtnNavLogout_Click(object? sender, EventArgs e)
    {
        var confirm = MessageBox.Show(
            this,
            "Bạn có chắc muốn đăng xuất khỏi hệ thống HUIT LingoHub?",
            "Xác nhận đăng xuất",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm == DialogResult.Yes)
        {
            AppSession.Clear();
            Hide();
            using var login = new FormDangNhap();
            login.ShowDialog();
            Close();
        }
    }
}
