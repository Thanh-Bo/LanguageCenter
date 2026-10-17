using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcXepLop : UserControl
{
    private const string AllCoursesOption = "-- Tất cả khóa học --";
    private LopHocDto? _currentClass;
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

        InitFilterOptions();
        LoadClassList();
        SwitchTab(0);
    }

    private void AlignToolbarButtons()
    {
        int right = pnlToolbar.ClientSize.Width - 20;
        btnRemoveFromClass.Location = new Point(right - btnRemoveFromClass.Width, 9);
        btnOpenEnrollDialog.Location = new Point(btnRemoveFromClass.Left - 12 - btnOpenEnrollDialog.Width, 9);
        btnEnrollSelectedPending.Location = new Point(right - btnEnrollSelectedPending.Width, 9);
    }

    private void InitFilterOptions()
    {
        cboFilterCourse.Items.Clear();
        cboFilterCourse.Items.Add(AllCoursesOption);
        foreach (var c in MockDataStore.GetCourses())
        {
            cboFilterCourse.Items.Add(c.TenKhoaHoc);
        }
        cboFilterCourse.SelectedIndex = 0;
    }

    private void LoadClassList()
    {
        cboTargetClass.Items.Clear();
        var classes = MockDataStore.GetClasses();
        foreach (var c in classes)
        {
            cboTargetClass.Items.Add($"{c.MaLop} — {c.TenLop}");
        }

        if (cboTargetClass.Items.Count > 0)
        {
            cboTargetClass.SelectedIndex = 0;
        }
        else
        {
            OnTargetClassChanged();
        }
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
        _currentClass = MockDataStore.GetClasses().FirstOrDefault(c => c.MaLop == maLop);

        if (_currentClass != null)
        {
            var isFull = _currentClass.SiSoHienTai >= _currentClass.SiSoToiDa;
            var remaining = _currentClass.SiSoToiDa - _currentClass.SiSoHienTai;
            lblCapacityBadge.Text = $"Sĩ số: {_currentClass.SiSoHienTai}/{_currentClass.SiSoToiDa} học viên" + (isFull ? " (ĐÃ ĐẦY LỚP)" : $" (Còn {remaining} chỗ trống)");
            lblCapacityBadge.ForeColor = isFull ? Theme.Danger : Theme.Success;

            lblClassDetails.Text = $"Khóa học: {_currentClass.TenKhoaHoc}    •    GV chính: {_currentClass.TenGiaoVienChinh}    •    Trợ giảng: {_currentClass.TenTroGiang}    •    Phòng: {_currentClass.TenPhongHoc}    •    Lịch học: {_currentClass.LichHoc}";

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
        if (_currentClass == null) return;

        var inClass = MockDataStore.GetEnrollments()
            .Where(e => e.MaLop == _currentClass.MaLop)
            .ToList();

        foreach (var s in inClass)
        {
            var rowIdx = dgvInClass.Rows.Add(
                s.MaHocVien,
                s.TenHocVien,
                s.SoDienThoai,
                s.Email,
                UiHelpers.FormatDateTime(s.NgayXepLop),
                s.TrangThaiHocPhi
            );
            dgvInClass.Rows[rowIdx].Tag = s.MaDangKy;
            dgvInClass.Rows[rowIdx].Cells["TrangThaiHocPhi"].Style.ForeColor = s.TrangThaiHocPhi == "Đã hoàn thành" ? Theme.Success : Theme.Warning;
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
        if (_currentClass == null) return;

        var query = MockDataStore.GetEnrollments()
            .Where(e => e.MaLop == _currentClass.MaLop);

        if (!string.IsNullOrWhiteSpace(kw))
        {
            query = query.Where(e => e.TenHocVien.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                     e.MaHocVien.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                     e.SoDienThoai.Contains(kw));
        }

        dgvInClass.Rows.Clear();
        var list = query.ToList();
        foreach (var s in list)
        {
            var rowIdx = dgvInClass.Rows.Add(
                s.MaHocVien,
                s.TenHocVien,
                s.SoDienThoai,
                s.Email,
                UiHelpers.FormatDateTime(s.NgayXepLop),
                s.TrangThaiHocPhi
            );
            dgvInClass.Rows[rowIdx].Tag = s.MaDangKy;
            dgvInClass.Rows[rowIdx].Cells["TrangThaiHocPhi"].Style.ForeColor = s.TrangThaiHocPhi == "Đã hoàn thành" ? Theme.Success : Theme.Warning;
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

        var query = MockDataStore.GetEnrollments()
            .Where(e => e.TrangThaiDangKy == "Chờ xếp lớp" || !e.MaLopHoc.HasValue);

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

        var list = query.ToList();
        foreach (var p in list)
        {
            var rowIdx = dgvPending.Rows.Add(
                p.MaDangKy,
                p.MaHocVien,
                p.TenHocVien,
                p.SoDienThoai,
                p.TenKhoaHoc,
                UiHelpers.FormatDateTime(p.NgayDangKy),
                p.TrangThaiHocPhi,
                p.GhiChu ?? "Chờ sắp xếp lịch phù hợp"
            );
            dgvPending.Rows[rowIdx].Tag = p.MaDangKy;
            dgvPending.Rows[rowIdx].Cells["HocPhi"].Style.ForeColor = p.TrangThaiHocPhi == "Đã hoàn thành" ? Theme.Success : Theme.Warning;
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
        LoadClassList();
        OnTargetClassChanged();
    }

    private void BtnOpenEnrollDialog_Click(object? sender, EventArgs e)
    {
        if (_currentClass == null)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn lớp học mục tiêu trước.", "Chưa chọn lớp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (_currentClass.SiSoHienTai >= _currentClass.SiSoToiDa)
        {
            MessageBox.Show(FindForm(), $"Lớp {_currentClass.MaLop} đã đạt sĩ số tối đa ({_currentClass.SiSoToiDa} học viên).\nKhông thể xếp thêm học viên vào lớp này.", "Lớp đã đầy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var dlg = new FormXepLopHocVien(_currentClass);
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            OnTargetClassChanged();
        }
    }

    private void BtnRemoveFromClass_Click(object? sender, EventArgs e)
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
            $"Bạn có chắc muốn rút học viên {studentName} khỏi lớp {_currentClass?.TenLop} và chuyển về danh sách Chờ xếp lớp?",
            "Xác nhận rút khỏi lớp",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm == DialogResult.Yes)
        {
            MockDataStore.RemoveEnrollmentFromClass(regId);
            OnTargetClassChanged();
            MessageBox.Show(FindForm(), $"Đã rút học viên {studentName} khỏi lớp thành công.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void BtnEnrollSelectedPending_Click(object? sender, EventArgs e)
    {
        if (_currentClass == null)
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

        if (confirm == DialogResult.Yes)
        {
            var ok = MockDataStore.EnrollStudentToClass(regId, _currentClass.MaLopHoc);
            if (ok)
            {
                MessageBox.Show(FindForm(), $"Đã xếp học viên {studentName} vào lớp {_currentClass.TenLop} thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                OnTargetClassChanged();
            }
        }
    }
}
