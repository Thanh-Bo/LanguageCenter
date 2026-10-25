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

    /// <summary>
    /// Màn hình của lần điều hướng TRƯỚC ĐÓ — chỉ bị Dispose khi điều hướng tiếp theo.
    /// <para>
    /// Không Dispose ngay màn vừa rời đi: các lời gọi API của nó có thể còn đang chờ,
    /// khi trả về mà control đã Dispose thì lưới mất hết cột và
    /// <c>DataGridView.Rows.Add</c> ném "No row can be added to a DataGridView control
    /// that does not have columns".
    /// </para>
    /// </summary>
    private UserControl? _viewTruocDo;

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

        if (role == RoleNames.GiaoVien || role == RoleNames.TroGiang)
        {
            HienThiVaiTroKhongHoTro(role);
            return;
        }

        NavigateTo(NavItem.Dashboard);
    }

    /// <summary>
    /// Bản WinForms CHỈ là phần mềm quản trị: mọi màn hình gọi <c>/api/admin/*</c>
    /// (Quản trị viên / Nhân viên học vụ). Giáo viên &amp; Trợ giảng dùng bản Web,
    /// nên hiện thông báo rõ ràng thay vì để các bảng dữ liệu trống + lỗi 403.
    /// </summary>
    private void HienThiVaiTroKhongHoTro(string role)
    {
        _currentView?.Dispose();
        pnlContent.Controls.Clear();
        lblPageTitle.Text = "Không hỗ trợ vai trò này trên bản WinForms";

        var card = new Panel
        {
            Dock = DockStyle.Top,
            Height = 260,
            BackColor = Color.White,
            Padding = new Padding(28)
        };

        card.Controls.Add(new Label
        {
            Text = "KHÔNG HỖ TRỢ VAI TRÒ NÀY TRÊN BẢN WINFORMS",
            Font = UI.Theme.FontHeading,
            ForeColor = UI.Theme.Danger,
            AutoSize = true,
            Location = new Point(28, 24)
        });

        card.Controls.Add(new Label
        {
            Text = $"Tài khoản đang đăng nhập: {AppSession.CurrentUser?.Username} — vai trò: {role}.\n\n"
                + "Bản WinForms là PHẦN MỀM QUẢN TRỊ: toàn bộ màn hình gọi API /api/admin/* nên chỉ "
                + "Quản trị viên và Nhân viên học vụ mới dùng được.\n"
                + "Giáo viên / Trợ giảng đăng nhập vào đây sẽ bị từ chối (403) và các bảng dữ liệu trống.\n\n"
                + "Vui lòng dùng bản Web để giảng dạy: http://localhost:5173",
            Font = UI.Theme.FontRegular,
            ForeColor = UI.Theme.TextMedium,
            AutoSize = false,
            Size = new Size(760, 130),
            Location = new Point(28, 64)
        });

        var btnCopy = UI.UiHelpers.CreateSecondaryButton("Sao chép địa chỉ Web", 190, 36);
        btnCopy.Location = new Point(28, 204);
        btnCopy.Click += (_, _) =>
        {
            try
            {
                Clipboard.SetText("http://localhost:5173");
                MessageBox.Show(this, "Đã sao chép địa chỉ bản Web.", "Thông báo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch
            {
                MessageBox.Show(this, "http://localhost:5173", "Địa chỉ bản Web",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        };
        card.Controls.Add(btnCopy);

        var btnDangXuat = UI.UiHelpers.CreatePrimaryButton("Đăng xuất", 150, 36);
        btnDangXuat.Location = new Point(230, 204);
        btnDangXuat.Click += BtnNavLogout_Click;
        card.Controls.Add(btnDangXuat);

        pnlContent.Controls.Add(card);
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

        // Giáo viên / Trợ giảng: bản WinForms chỉ có màn hình quản trị (mọi API là /api/admin/*)
        // → mọi trang đều 403. Không mở phân hệ nào, hiện thông báo hướng dẫn thay vì màn trống.
        if (role == RoleNames.GiaoVien || role == RoleNames.TroGiang)
        {
            return false;
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

        // Dọn màn của lần điều hướng trước đó (request của nó đã kịp xong),
        // KHÔNG Dispose màn vừa rời đi vì nó có thể còn chờ dữ liệu.
        _viewTruocDo?.Dispose();
        _viewTruocDo = _currentView;
        _currentView = null;

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
