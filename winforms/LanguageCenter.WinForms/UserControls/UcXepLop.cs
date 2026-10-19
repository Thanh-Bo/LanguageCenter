using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcXepLop : UserControl
{
    private const string AllCoursesOption = "-- Tất cả khóa học --";
    private ClassListItemDto? _currentClass;
    private IReadOnlyList<ClassListItemDto> _classesApi = Array.Empty<ClassListItemDto>();
    private IReadOnlyList<EnrollmentListItemDto> _enrollApi = Array.Empty<EnrollmentListItemDto>();
    private int _currentTab = 0;
    private int _inClassCount = 0;
    private int _pendingCount = 0;

    public UcXepLop()
    {
        InitializeComponent();

        UI.UiHelpers.StyleGrid(dgvInClass);
        UI.UiHelpers.StyleGrid(dgvPending);

        cboTargetClass.SelectedIndexChanged += (_, _) => OnTargetClassChanged();
        btnRefresh.Click += (_, _) => ReloadData();

        btnTabInClass.Click += (_, _) => SwitchTab(0);
        btnTabPending.Click += (_, _) => SwitchTab(1);

        txtSearchInClass.TextChanged += (_, _) => FilterInClassStudents();
        btnOpenEnrollDialog.Click += BtnOpenEnrollDialog_Click;
        btnRemoveFromClass.Click += BtnRemoveFromClass_Click;

        txtSearchPending.TextChanged += (_, _) => FilterPendingStudents();
        cboFilterCourse.SelectedIndexChanged += (_, _) => FilterPendingStudents();
        btnEnrollSelectedPending.Click += BtnEnrollSelectedPending_Click;
        btnTuChoiDangKy.Click += BtnTuChoiDangKy_Click;

        pnlHeader.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240));
            e.Graphics.DrawLine(pen, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
        };

        pnlTabBar.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240));
            e.Graphics.DrawLine(pen, 0, pnlTabBar.Height - 1, pnlTabBar.Width, pnlTabBar.Height - 1);
        };

        pnlToolbar.Resize += (_, _) => AlignToolbarButtons();

        btnOpenEnrollDialog.Visible = true; // Tạo đăng ký học trực tiếp trên WinForms

        SwitchTab(0);
        Load += async (_, _) => await ReloadAsync();
    }

    /// <summary>Tải danh sách lớp + đăng ký học từ API.</summary>
    private async Task ReloadAsync()
    {
        lblSummary.Text = "Đang tải dữ liệu xếp lớp...";
        UseWaitCursor = true;

        var classesTask = AppSession.Api.GetAsync<PagedResult<ClassListItemDto>>(
            QueryStringBuilder.Build("admin/classes", ("page", "1"), ("pageSize", "500")));
        var enrollTask = AppSession.Api.GetAsync<PagedResult<EnrollmentListItemDto>>(
            QueryStringBuilder.Build("admin/enrollments", ("page", "1"), ("pageSize", "500")));

        await Task.WhenAll(classesTask, enrollTask);

        UseWaitCursor = false;

        _classesApi = classesTask.Result.Data?.Items ?? Array.Empty<ClassListItemDto>();
        _enrollApi = enrollTask.Result.Data?.Items ?? Array.Empty<EnrollmentListItemDto>();

        if (!classesTask.Result.Success)
        {
            lblSummary.Text = classesTask.Result.ErrorMessage;
        }
        else if (!enrollTask.Result.Success)
        {
            lblSummary.Text = enrollTask.Result.ErrorMessage;
        }

        InitFilterOptions();
        LoadClassList();
        OnTargetClassChanged();
    }

    private void AlignToolbarButtons()
    {
        int right = pnlToolbar.ClientSize.Width - 20;
        btnRemoveFromClass.Location = new Point(right - btnRemoveFromClass.Width, 9);
        btnOpenEnrollDialog.Location = new Point(btnRemoveFromClass.Left - 12 - btnOpenEnrollDialog.Width, 9);
        btnEnrollSelectedPending.Location = new Point(right - btnEnrollSelectedPending.Width, 9);
        btnTuChoiDangKy.Location = new Point(btnEnrollSelectedPending.Left - 12 - btnTuChoiDangKy.Width, 9);
    }

    private void InitFilterOptions()
    {
        var dangChon = cboFilterCourse.SelectedItem?.ToString() ?? AllCoursesOption;

        cboFilterCourse.Items.Clear();
        cboFilterCourse.Items.Add(AllCoursesOption);

        foreach (var ten in _classesApi.Select(c => c.TenKhoaHoc).Distinct().OrderBy(x => x))
        {
            cboFilterCourse.Items.Add(ten);
        }

        var viTri = cboFilterCourse.Items.IndexOf(dangChon);
        cboFilterCourse.SelectedIndex = viTri >= 0 ? viTri : 0;
    }

    private void LoadClassList()
    {
        var dangChon = cboTargetClass.SelectedItem?.ToString();

        cboTargetClass.Items.Clear();
        foreach (var c in _classesApi.OrderBy(c => c.MaLop))
        {
            cboTargetClass.Items.Add($"{c.MaLop} — {c.TenLop}");
        }

        if (cboTargetClass.Items.Count == 0)
        {
            _currentClass = null;
            OnTargetClassChanged();
            return;
        }

        var viTri = dangChon is null ? -1 : cboTargetClass.Items.IndexOf(dangChon);
        cboTargetClass.SelectedIndex = viTri >= 0 ? viTri : 0;
    }

    private void OnTargetClassChanged()
    {
        var selected = cboTargetClass.SelectedItem?.ToString();
        if (string.IsNullOrWhiteSpace(selected))
        {
            _currentClass = null;
            lblClassDetails.Text = "Chưa chọn lớp học.";
            lblCapacityBadge.Text = string.Empty;
            dgvInClass.Rows.Clear();
            btnOpenEnrollDialog.Enabled = false;
            btnRemoveFromClass.Enabled = false;
            return;
        }

        var maLop = selected.Split(" — ")[0].Trim();
        _currentClass = _classesApi.FirstOrDefault(c => c.MaLop == maLop);

        if (_currentClass is not null)
        {
            var isFull = _currentClass.SiSoHienTai >= _currentClass.SiSoToiDa;
            var remaining = _currentClass.SiSoToiDa - _currentClass.SiSoHienTai;
            lblCapacityBadge.Text = $"Sĩ số: {_currentClass.SiSoHienTai}/{_currentClass.SiSoToiDa} học viên" + (isFull ? " (ĐÃ ĐẦY LỚP)" : $" (Còn {remaining} chỗ trống)");
            lblCapacityBadge.ForeColor = isFull ? Theme.Danger : Theme.Success;

            lblClassDetails.Text = $"Khóa học: {_currentClass.TenKhoaHoc}    •    Học vụ: {_currentClass.TenNhanVienHocVu}    •    Khai giảng: {UiHelpers.FormatDate(_currentClass.NgayBatDau)}    •    Kết thúc dự kiến: {UiHelpers.FormatDate(_currentClass.NgayKetThucDuKien)}";

            btnOpenEnrollDialog.Enabled = !isFull;
            btnRemoveFromClass.Enabled = true;
        }

        LoadInClassStudents();
        FilterPendingStudents();
    }

    private void UpdateTabButtonsAppearance()
    {
        btnTabInClass.Text = $"Học viên trong lớp ({_inClassCount})";
        btnTabPending.Text = $"Học viên chờ xếp lớp ({_pendingCount})";

        if (_currentTab == 0)
        {
            btnTabInClass.BackColor = Theme.Primary;
            btnTabInClass.ForeColor = Color.White;
            btnTabInClass.FlatAppearance.BorderColor = Theme.Primary;
            btnTabInClass.FlatAppearance.MouseOverBackColor = Theme.PrimaryDark;
            btnTabInClass.Font = Theme.FontBold;

            btnTabPending.BackColor = Color.White;
            btnTabPending.ForeColor = Color.FromArgb(71, 85, 105);
            btnTabPending.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
            btnTabPending.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 245, 249);
            btnTabPending.Font = Theme.FontRegular;
        }
        else
        {
            btnTabInClass.BackColor = Color.White;
            btnTabInClass.ForeColor = Color.FromArgb(71, 85, 105);
            btnTabInClass.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
            btnTabInClass.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 245, 249);
            btnTabInClass.Font = Theme.FontRegular;

            btnTabPending.BackColor = Theme.Primary;
            btnTabPending.ForeColor = Color.White;
            btnTabPending.FlatAppearance.BorderColor = Theme.Primary;
            btnTabPending.FlatAppearance.MouseOverBackColor = Theme.PrimaryDark;
            btnTabPending.Font = Theme.FontBold;
        }
    }

    private void SwitchTab(int tabIndex)
    {
        _currentTab = tabIndex;
        UpdateTabButtonsAppearance();

        // Toggle toolbar & grids
        bool isInClassTab = _currentTab == 0;

        txtSearchInClass.Visible = isInClassTab;
        btnOpenEnrollDialog.Visible = isInClassTab;
        btnRemoveFromClass.Visible = isInClassTab;

        txtSearchPending.Visible = !isInClassTab;
        cboFilterCourse.Visible = !isInClassTab;
        btnEnrollSelectedPending.Visible = !isInClassTab;
        btnTuChoiDangKy.Visible = !isInClassTab;

        dgvInClass.Visible = isInClassTab;
        dgvPending.Visible = !isInClassTab;

        AlignToolbarButtons();

        if (isInClassTab)
        {
            dgvInClass.BringToFront();
            lblSummary.Text = $"Đang hiển thị {dgvInClass.Rows.Count} học viên trong lớp {_currentClass?.TenLop ?? "mục tiêu"}. Sĩ số tối đa: {_currentClass?.SiSoToiDa ?? 0} học viên.";
        }
        else
        {
            dgvPending.BringToFront();
            lblSummary.Text = $"Đang hiển thị {dgvPending.Rows.Count} học viên trong hàng chờ xếp lớp trên toàn trung tâm.";
        }
    }

    private void LoadInClassStudents()
    {
        dgvInClass.Rows.Clear();
        if (_currentClass is null) return;

        var inClass = _enrollApi
            .Where(e => e.MaLopHoc == _currentClass.MaLopHoc)
            .ToList();

        foreach (var s in inClass)
        {
            var rowIdx = dgvInClass.Rows.Add(
                s.MaHocVien,
                s.TenHocVien,
                "—",
                "—",
                s.NgayXepLop.HasValue ? UiHelpers.FormatDateTime(s.NgayXepLop) : "—",
                s.TrangThaiDangKy
            );
            dgvInClass.Rows[rowIdx].Tag = s.MaDangKy;
            dgvInClass.Rows[rowIdx].Cells["TrangThaiHocPhi"].Style.ForeColor =
                s.TrangThaiDangKy == "Đã xếp lớp" ? Theme.Success : Theme.Warning;
        }

        _inClassCount = inClass.Count;
        UpdateTabButtonsAppearance();

        if (_currentTab == 0)
        {
            lblSummary.Text = $"Đang hiển thị {inClass.Count} học viên trong lớp {_currentClass.TenLop}. Sĩ số tối đa: {_currentClass.SiSoToiDa} học viên.";
        }
    }

    private void FilterInClassStudents()
    {
        var kw = txtSearchInClass.Text.Trim();
        if (_currentClass is null) return;

        var query = _enrollApi.Where(e => e.MaLopHoc == _currentClass.MaLopHoc);

        if (!string.IsNullOrWhiteSpace(kw))
        {
            query = query.Where(e => e.TenHocVien.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                     e.MaHocVien.Contains(kw, StringComparison.OrdinalIgnoreCase));
        }

        dgvInClass.Rows.Clear();
        var list = query.ToList();
        foreach (var s in list)
        {
            var rowIdx = dgvInClass.Rows.Add(
                s.MaHocVien,
                s.TenHocVien,
                "—",
                "—",
                s.NgayXepLop.HasValue ? UiHelpers.FormatDateTime(s.NgayXepLop) : "—",
                s.TrangThaiDangKy
            );
            dgvInClass.Rows[rowIdx].Tag = s.MaDangKy;
            dgvInClass.Rows[rowIdx].Cells["TrangThaiHocPhi"].Style.ForeColor =
                s.TrangThaiDangKy == "Đã xếp lớp" ? Theme.Success : Theme.Warning;
        }

        if (_currentTab == 0)
        {
            lblSummary.Text = $"Tìm thấy {list.Count} học viên trong lớp {_currentClass.TenLop}.";
        }
    }

    private void FilterPendingStudents()
    {
        dgvPending.Rows.Clear();
        var kw = txtSearchPending.Text.Trim();
        var selectedCourse = cboFilterCourse.SelectedItem?.ToString() ?? AllCoursesOption;

        // Chỉ lấy đăng ký CHƯA có lớp và CÒN hiệu lực (bỏ qua đã hủy / hoàn thành).
        var query = _enrollApi.Where(e => !e.MaLopHoc.HasValue && e.TrangThaiDangKy == "Chờ xử lý");

        if (selectedCourse != AllCoursesOption)
        {
            query = query.Where(e => e.TenKhoaHoc.Equals(selectedCourse, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(kw))
        {
            query = query.Where(e => e.TenHocVien.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                     e.MaHocVien.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                     e.TenKhoaHoc.Contains(kw, StringComparison.OrdinalIgnoreCase));
        }

        var list = query.OrderBy(e => e.NgayDangKy).ToList();
        foreach (var p in list)
        {
            var rowIdx = dgvPending.Rows.Add(
                p.MaDangKy,
                p.MaHocVien,
                p.TenHocVien,
                "—",
                p.TenKhoaHoc,
                UiHelpers.FormatDateTime(p.NgayDangKy),
                p.TrangThaiDangKy,
                $"Học phí sau giảm: {p.HocPhiSauGiam:N0} ₫"
            );
            dgvPending.Rows[rowIdx].Tag = p.MaDangKy;
            dgvPending.Rows[rowIdx].Cells["HocPhi"].Style.ForeColor = Theme.Warning;
        }

        _pendingCount = list.Count;
        UpdateTabButtonsAppearance();

        if (_currentTab == 1)
        {
            lblSummary.Text = $"Đang hiển thị {list.Count} học viên trong hàng chờ xếp lớp trên toàn trung tâm.";
        }
    }

    public void ReloadData()
    {
        _ = ReloadAsync();
    }

    private async void BtnOpenEnrollDialog_Click(object? sender, EventArgs e)
    {
        UseWaitCursor = true;

        var studentsTask = AppSession.Api.GetAsync<PagedResult<StudentListItemDto>>(
            QueryStringBuilder.Build("admin/students", ("page", "1"), ("pageSize", "500")));
        var coursesTask = AppSession.Api.GetAsync<PagedResult<CourseListItemDto>>(
            QueryStringBuilder.Build("admin/courses", ("page", "1"), ("pageSize", "500")));

        await Task.WhenAll(studentsTask, coursesTask);

        UseWaitCursor = false;

        var hocViens = studentsTask.Result.Data?.Items ?? Array.Empty<StudentListItemDto>();
        var khoaHocs = coursesTask.Result.Data?.Items ?? Array.Empty<CourseListItemDto>();

        if (hocViens.Count == 0)
        {
            MessageBox.Show(FindForm(), "Chưa có học viên nào trong hệ thống.",
                "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (khoaHocs.Count == 0)
        {
            MessageBox.Show(FindForm(), "Chưa có khóa học nào đang mở.",
                "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new FormDangKyHoc(hocViens, khoaHocs, _classesApi);

        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            await ReloadAsync();
        }
    }

    private async void BtnRemoveFromClass_Click(object? sender, EventArgs e)
    {
        if (dgvInClass.CurrentRow == null)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn một học viên trong lớp cần rút khỏi lớp.", "Chưa chọn học viên", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var regId = Convert.ToInt32(dgvInClass.CurrentRow.Tag);
        var studentName = dgvInClass.CurrentRow.Cells["TenHocVien"].Value?.ToString() ?? "Học viên";

        var confirm = MessageBox.Show(
            FindForm(),
            $"Bạn có chắc muốn rút học viên {studentName} khỏi lớp {_currentClass?.TenLop}?\n"
            + "Đăng ký học sẽ trở về trạng thái Chờ xử lý để có thể xếp lại lớp khác.",
            "Xác nhận rút khỏi lớp",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        UseWaitCursor = true;

        var result = await AppSession.Api.PatchAsync<EnrollmentListItemDto>(
            $"admin/enrollments/{regId}/unassign-class",
            null);

        UseWaitCursor = false;

        if (!result.Success)
        {
            MessageBox.Show(FindForm(), result.ErrorMessage, "Rút khỏi lớp thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        await ReloadAsync();
    }

    private async void BtnEnrollSelectedPending_Click(object? sender, EventArgs e)
    {
        if (_currentClass is null)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn lớp học mục tiêu ở thanh phía trên trước khi xếp lớp.", "Chưa chọn lớp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (dgvPending.CurrentRow == null)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn một học viên trong danh sách chờ xếp lớp.", "Chưa chọn học viên", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (_currentClass.SiSoHienTai >= _currentClass.SiSoToiDa)
        {
            MessageBox.Show(FindForm(), $"Lớp {_currentClass.MaLop} đã đạt sĩ số tối đa ({_currentClass.SiSoToiDa} học viên).\nKhông thể xếp thêm học viên vào lớp này.", "Lớp đã đầy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var regId = Convert.ToInt32(dgvPending.CurrentRow.Tag ?? dgvPending.CurrentRow.Cells["MaDangKy"].Value);
        var studentName = dgvPending.CurrentRow.Cells["TenHocVien"].Value?.ToString() ?? "Học viên";

        var confirm = MessageBox.Show(
            FindForm(),
            $"Xác nhận xếp học viên \"{studentName}\" vào lớp \"{_currentClass.TenLop}\"?",
            "Xác nhận xếp lớp",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        UseWaitCursor = true;

        // Phase 4: gán lớp cho đăng ký học (kiểm tra sĩ số + trạng thái ở backend)
        var result = await AppSession.Api.PatchAsync<EnrollmentListItemDto>(
            $"admin/enrollments/{regId}/assign-class",
            new { maLopHoc = _currentClass.MaLopHoc });

        UseWaitCursor = false;

        if (!result.Success)
        {
            MessageBox.Show(FindForm(), result.ErrorMessage, "Xếp lớp thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        await ReloadAsync();
    }

    /// <summary>
    /// Từ chối (hủy) một đăng ký học còn chờ xếp lớp — chuyển trạng thái sang "Đã hủy".
    /// <para>
    /// Hệ thống KHÔNG có endpoint xóa đăng ký; đăng ký ở trạng thái "Chờ xử lý" vẫn được
    /// tính là "còn hiệu lực" và sẽ chặn việc tạo đăng ký mới cho cùng khóa học.
    /// Vì vậy muốn bỏ một đăng ký thừa thì phải hủy ở đây.
    /// </para>
    /// </summary>
    private async void BtnTuChoiDangKy_Click(object? sender, EventArgs e)
    {
        if (dgvPending.CurrentRow is null)
        {
            MessageBox.Show(FindForm(),
                "Vui lòng chọn một đăng ký trong danh sách chờ xếp lớp cần từ chối.",
                "Chưa chọn đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var regId = Convert.ToInt32(dgvPending.CurrentRow.Tag ?? dgvPending.CurrentRow.Cells["MaDangKy"].Value);
        var studentName = dgvPending.CurrentRow.Cells["TenHocVien"].Value?.ToString() ?? "Học viên";
        var tenKhoaHoc = dgvPending.CurrentRow.Cells["KhoaHoc"].Value?.ToString() ?? "khóa học";

        var xacNhan = MessageBox.Show(FindForm(),
            $"Từ chối đăng ký của học viên \"{studentName}\" cho khóa \"{tenKhoaHoc}\"?\n\n"
            + "Đăng ký sẽ chuyển sang trạng thái 'Đã hủy' và không còn chặn việc tạo đăng ký mới cho khóa này.\n"
            + "Học viên KHÔNG bị xóa khỏi hệ thống.",
            "Xác nhận từ chối đăng ký", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (xacNhan != DialogResult.Yes)
        {
            return;
        }

        UseWaitCursor = true;

        var result = await AppSession.Api.PatchAsync<EnrollmentListItemDto>(
            $"admin/enrollments/{regId}/status",
            new { trangThai = "Đã hủy" });

        UseWaitCursor = false;

        if (!result.Success)
        {
            MessageBox.Show(FindForm(),
                string.IsNullOrWhiteSpace(result.ErrorMessage) ? "Không từ chối được đăng ký." : result.ErrorMessage,
                "Từ chối đăng ký thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        await ReloadAsync();
    }
}
