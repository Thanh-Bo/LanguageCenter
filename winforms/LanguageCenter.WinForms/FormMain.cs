using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UserControls;

namespace LanguageCenter.WinForms;

public enum NavItem
{
    Dashboard,
    Courses,
    Tuition,
    Students,
    Teachers,
    Assistants,
    Accounts
}

public partial class FormMain : Form
{
    private UserControl? _currentView;

    public FormMain()
    {
        InitializeComponent();
    }

    private void FormMain_Load(object? sender, EventArgs e)
    {
        var user = AppSession.CurrentUser;
        var role = user?.Role ?? RoleNames.QuanTriVien;

        if (role == RoleNames.GiaoVien)
        {
            lblCurrentUser.Text = $"👨‍🏫 {user?.Username} (Giáo viên)";
            btnNavAssistants.Text = "🔒  Quản lý trợ giảng";
            btnNavAccounts.Text = "🔒  Quản lý tài khoản";
        }
        else if (role == RoleNames.TroGiang)
        {
            lblCurrentUser.Text = $"🧑‍🏫 {user?.Username} (Trợ giảng)";
            btnNavTeachers.Text = "🔒  Quản lý giáo viên";
            btnNavAccounts.Text = "🔒  Quản lý tài khoản";
        }
        else
        {
            lblCurrentUser.Text = $"👑 {user?.Username ?? "ADMIN001"} (Quản trị viên)";
        }

        NavigateTo(NavItem.Dashboard);
    }

    public void NavigateTo(NavItem item)
    {
        var role = AppSession.CurrentUser?.Role ?? RoleNames.QuanTriVien;

        // Phân quyền chặt chẽ theo vai trò
        if (item == NavItem.Teachers && role != RoleNames.GiaoVien && role != RoleNames.QuanTriVien)
        {
            MessageBox.Show(
                this,
                $"🚫 TRUY CẬP BỊ TỪ CHỐI!\n\n" +
                $"Trang \"Quản lý giáo viên\" chỉ dành riêng cho Giáo viên giảng dạy và Ban Quản trị trung tâm.\n" +
                $"Tài khoản hiện tại ({AppSession.CurrentUser?.Username} — vai trò: {role}) không có thẩm quyền truy cập.",
                "Phân quyền bảo mật",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        if (item == NavItem.Assistants && role != RoleNames.TroGiang && role != RoleNames.QuanTriVien)
        {
            MessageBox.Show(
                this,
                $"🚫 TRUY CẬP BỊ TỪ CHỐI!\n\n" +
                $"Trang \"Quản lý trợ giảng\" chỉ dành riêng cho Trợ giảng học vụ và Ban Quản trị trung tâm.\n" +
                $"Tài khoản hiện tại ({AppSession.CurrentUser?.Username} — vai trò: {role}) không có thẩm quyền truy cập.",
                "Phân quyền bảo mật",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        if (item == NavItem.Accounts && role != RoleNames.QuanTriVien)
        {
            MessageBox.Show(
                this,
                $"🚫 TRUY CẬP BỊ TỪ CHỐI!\n\n" +
                $"Trang \"Quản lý tài khoản\" là phân hệ bảo mật cấp cao, chỉ dành riêng cho Quản trị viên hệ thống.\n" +
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
            NavItem.Dashboard => new UcDashboard(this),
            NavItem.Courses => new UcCourses(),
            NavItem.Tuition => new UcHocPhi(),
            NavItem.Students => new UcStudents(),
            NavItem.Teachers => new UcTeachers(),
            NavItem.Assistants => new UcTeachingAssistants(),
            NavItem.Accounts => new UcAccounts(),
            _ => new UcDashboard(this)
        };

        lblPageTitle.Text = item switch
        {
            NavItem.Dashboard => "Tổng quan hệ thống đào tạo",
            NavItem.Courses => "Quản lý khóa học & Chương trình đào tạo — LinguaHub",
            NavItem.Tuition => "Quản lý biểu phí & Sổ cái học phí — LinguaHub",
            NavItem.Students => "Quản lý hồ sơ học viên",
            NavItem.Teachers => "Quản lý hồ sơ giáo viên giảng dạy",
            NavItem.Assistants => "Quản lý hồ sơ trợ giảng học vụ",
            NavItem.Accounts => "Quản lý tài khoản người dùng hệ thống",
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
        ResetNavButton(btnNavTuition);
        ResetNavButton(btnNavStudents);
        ResetNavButton(btnNavTeachers);
        ResetNavButton(btnNavAssistants);
        ResetNavButton(btnNavAccounts);

        var active = item switch
        {
            NavItem.Dashboard => btnNavDashboard,
            NavItem.Courses => btnNavCourses,
            NavItem.Tuition => btnNavTuition,
            NavItem.Students => btnNavStudents,
            NavItem.Teachers => btnNavTeachers,
            NavItem.Assistants => btnNavAssistants,
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
            "Bạn có chắc muốn đăng xuất khỏi hệ thống LinguaHub?",
            "Xác nhận đăng xuất",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm == DialogResult.Yes)
        {
            Close();
        }
    }
}
