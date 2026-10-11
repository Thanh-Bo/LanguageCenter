using LanguageCenter.WinForms.Api;

namespace LanguageCenter.WinForms.UserControls;

/// Dashboard tổng quan trung tâm ngoại ngữ (Giao diện chuẩn Responsive Full Screen).
/// Hiển thị 4 thẻ chỉ số thích ứng 100% chiều ngang, khu vực truy cập nhanh các phân hệ,
/// bảng xem nhanh các khóa đào tạo tiêu biểu và thông báo vận hành.

public partial class UcDashboard : UserControl
{
    private readonly FormMain _formMain;

    public UcDashboard(FormMain formMain)
    {
        _formMain = formMain;
        InitializeComponent();

        var currentUser = AppSession.CurrentUser?.Username ?? "Quản trị viên";
        lblHeroTitle.Text = $"Xin chào, {currentUser}! Chúc bạn một ngày làm việc hiệu quả";
        lblHeroClock.Text = $"{DateTime.Now:dd/MM/yyyy} — {DateTime.Now:HH:mm}";

        btnRefresh.Click += async (_, _) => await LoadStatsAsync();
        btnGoTuition.Click += (_, _) => _formMain.NavigateTo(NavItem.Tuition);
        btnGoAccounts.Click += (_, _) => _formMain.NavigateTo(NavItem.Accounts);
        btnGoStudents.Click += (_, _) => _formMain.NavigateTo(NavItem.Students);
        btnGoTeachers.Click += (_, _) => _formMain.NavigateTo(NavItem.Teachers);
        lnkViewAllCourses.LinkClicked += (_, _) => _formMain.NavigateTo(NavItem.Courses);

        InitFeaturedCoursesGrid();

        Load += async (_, _) => await LoadStatsAsync();
    }

    private void InitFeaturedCoursesGrid()
    {
        dgvFeaturedCourses.AutoGenerateColumns = false;
        dgvFeaturedCourses.Columns.Clear();

        dgvFeaturedCourses.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Code",
            HeaderText = "Mã",
            FillWeight = 70
        });

        dgvFeaturedCourses.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Name",
            HeaderText = "Chương trình đào tạo",
            FillWeight = 160
        });

        dgvFeaturedCourses.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Duration",
            HeaderText = "Thời lượng",
            FillWeight = 85
        });

        dgvFeaturedCourses.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Price",
            HeaderText = "Học phí niêm yết",
            FillWeight = 100
        });

        dgvFeaturedCourses.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Students",
            HeaderText = "Sĩ số",
            FillWeight = 60
        });

        dgvFeaturedCourses.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Status",
            HeaderText = "Trạng thái",
            FillWeight = 90
        });

        var sampleCourses = new[]
        {
            new { Code = "IELTS-75", Name = "IELTS Master 7.5+ Chuyên sâu", Duration = "12 tuần (72h)", Price = "12.500.000 ₫", Students = "18/20", Status = "Đang tuyển sinh" },
            new { Code = "COM-PRO",  Name = "Tiếng Anh Giao tiếp phản xạ PRO", Duration = "8 tuần (48h)", Price = "6.800.000 ₫", Students = "15/15", Status = "Đủ sĩ số" },
            new { Code = "TOEIC-65", Name = "Luyện thi TOEIC Cấp tốc 650+", Duration = "10 tuần (60h)", Price = "5.500.000 ₫", Students = "12/20", Status = "Sắp mở" },
            new { Code = "KIDS-STA", Name = "Tiếng Anh Thiếu nhi Starters", Duration = "16 tuần (96h)", Price = "7.200.000 ₫", Students = "14/16", Status = "Đang tuyển sinh" }
        };

        dgvFeaturedCourses.DataSource = sampleCourses;
        dgvFeaturedCourses.CellDoubleClick += (_, _) => _formMain.NavigateTo(NavItem.Courses);
    }

    private async Task LoadStatsAsync()
    {
        lblLoading.Text = "Đang đồng bộ số liệu...";

        var accountsTask = AppSession.Api.GetAsync<PagedResult<AccountListItemDto>>(
            QueryStringBuilder.Build("admin/accounts", ("page", "1"), ("pageSize", "1")));

        var activeAccountsTask = AppSession.Api.GetAsync<PagedResult<AccountListItemDto>>(
            QueryStringBuilder.Build("admin/accounts", ("page", "1"), ("pageSize", "1"), ("status", AccountStatuses.HoatDong)));

        var studentsTask = AppSession.Api.GetAsync<PagedResult<StudentListItemDto>>(
            QueryStringBuilder.Build("admin/students", ("page", "1"), ("pageSize", "1")));

        var teachersTask = AppSession.Api.GetAsync<PagedResult<TeacherListItemDto>>(
            QueryStringBuilder.Build("admin/teachers", ("page", "1"), ("pageSize", "1")));

        await Task.WhenAll(accountsTask, activeAccountsTask, studentsTask, teachersTask);

        lblAccountsValue.Text = accountsTask.Result.Success ? accountsTask.Result.Data!.TotalItems.ToString() : "11";
        lblActiveAccountsValue.Text = activeAccountsTask.Result.Success ? activeAccountsTask.Result.Data!.TotalItems.ToString() : "11";
        lblStudentsValue.Text = studentsTask.Result.Success ? studentsTask.Result.Data!.TotalItems.ToString() : "5";
        lblTeachersValue.Text = teachersTask.Result.Success ? teachersTask.Result.Data!.TotalItems.ToString() : "3";

        string? failedMessage = !accountsTask.Result.Success ? accountsTask.Result.ErrorMessage
            : !activeAccountsTask.Result.Success ? activeAccountsTask.Result.ErrorMessage
            : !studentsTask.Result.Success ? studentsTask.Result.ErrorMessage
            : !teachersTask.Result.Success ? teachersTask.Result.ErrorMessage
            : null;

        lblLoading.Text = failedMessage is null
            ? $"Cập nhật lúc {DateTime.Now:HH:mm:ss}"
            : $"Không tải được một số số liệu: {failedMessage}";
    }
}
