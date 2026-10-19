using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcKhoaHoc : UserControl
{
    private const string AllOption = "— Tất cả —";
    private int _currentTab = 0; // 0: Khóa học, 1: Cấp độ, 2: Chương trình

    private IReadOnlyList<CourseListItemDto> _courses = Array.Empty<CourseListItemDto>();
    private IReadOnlyList<LevelListItemDto> _levels = Array.Empty<LevelListItemDto>();
    private IReadOnlyList<ProgramListItemDto> _programs = Array.Empty<ProgramListItemDto>();
    private bool _dangNap;

    public UcKhoaHoc()
    {
        InitializeComponent();

        UI.UiHelpers.StyleGrid(dgvCourses);
        UI.UiHelpers.StyleGrid(dgvLevels);
        UI.UiHelpers.StyleGrid(dgvPrograms);

        dgvCourses.CellDoubleClick += DgvCourses_CellDoubleClick;

        btnTabCourses.Click += (_, _) => SwitchTab(0);
        btnTabLevels.Click += (_, _) => SwitchTab(1);
        btnTabPrograms.Click += (_, _) => SwitchTab(2);

        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; ApplyFilter(); } };
        cboChuongTrinh.SelectedIndexChanged += (_, _) => { if (!_dangNap) ApplyFilter(); };
        cboTrangThai.SelectedIndexChanged += (_, _) => { if (!_dangNap) ApplyFilter(); };
        btnSearch.Click += (_, _) => ApplyFilter();
        btnRefresh.Click += async (_, _) => await ReloadAsync();
        btnAdd.Click += BtnAdd_Click;

        pnlToolbar.Resize += (_, _) =>
        {
            btnAdd.Location = new Point(pnlToolbar.Width - btnAdd.Width - 4, 3);
        };

        SwitchTab(0);
        Load += async (_, _) => await ReloadAsync();
    }

    /// <summary>Tải khóa học, cấp độ và chương trình từ API.</summary>
    private async Task ReloadAsync()
    {
        lblSummary.Text = "Đang tải dữ liệu đào tạo...";
        UseWaitCursor = true;

        var coursesTask = AppSession.Api.GetAsync<PagedResult<CourseListItemDto>>(
            QueryStringBuilder.Build("admin/courses", ("page", "1"), ("pageSize", "500")));
        var levelsTask = AppSession.Api.GetAsync<PagedResult<LevelListItemDto>>(
            QueryStringBuilder.Build("admin/levels", ("page", "1"), ("pageSize", "500")));
        var programsTask = AppSession.Api.GetAsync<PagedResult<ProgramListItemDto>>(
            QueryStringBuilder.Build("admin/programs", ("page", "1"), ("pageSize", "500")));

        await Task.WhenAll(coursesTask, levelsTask, programsTask);

        UseWaitCursor = false;

        _courses = coursesTask.Result.Data?.Items ?? Array.Empty<CourseListItemDto>();
        _levels = levelsTask.Result.Data?.Items ?? Array.Empty<LevelListItemDto>();
        _programs = programsTask.Result.Data?.Items ?? Array.Empty<ProgramListItemDto>();

        if (!coursesTask.Result.Success)
        {
            lblSummary.Text = coursesTask.Result.ErrorMessage;
        }

        FillStats();
        InitFilterOptions();
        FillStatusFilter();
        ApplyFilter();
    }

    private void FillStats()
    {
        var dangHoatDong = _courses.Count(c => c.TrangThai != "Đã kết thúc" && c.TrangThai != "Ngừng hoạt động");

        tblStats.Controls.Clear();
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Tổng số khóa học", $"{_courses.Count} Khóa", "Các cấp độ từ A1 đến C1", Theme.Primary), 0, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Chương trình đào tạo", $"{_programs.Count} Hệ", "IELTS, TOEIC, Giao tiếp, Thiếu nhi", Theme.Purple), 1, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Cấp độ đào tạo", $"{_levels.Count} Cấp độ", "Chuẩn khung tham chiếu CEFR", Theme.Success), 2, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Khóa đang hoạt động", $"{dangHoatDong} Khóa", "Đang tuyển sinh / đang giảng dạy", Theme.Warning), 3, 0);
    }

    private void InitFilterOptions()
    {
        _dangNap = true;

        var dangChon = cboChuongTrinh.SelectedItem?.ToString() ?? AllOption;

        cboChuongTrinh.Items.Clear();
        cboChuongTrinh.Items.Add(AllOption);
        foreach (var ten in _programs.Select(p => p.TenChuongTrinh).Distinct().OrderBy(x => x))
        {
            cboChuongTrinh.Items.Add(ten);
        }

        var viTri = cboChuongTrinh.Items.IndexOf(dangChon);
        cboChuongTrinh.SelectedIndex = viTri >= 0 ? viTri : 0;

        _dangNap = false;
    }

    private void SwitchTab(int tabIndex)
    {
        _currentTab = tabIndex;

        btnTabCourses.BackColor = _currentTab == 0 ? Theme.Primary : Color.White;
        btnTabCourses.ForeColor = _currentTab == 0 ? Color.White : Theme.TextDark;
        btnTabCourses.Font = _currentTab == 0 ? Theme.FontBold : Theme.FontRegular;

        btnTabLevels.BackColor = _currentTab == 1 ? Theme.Primary : Color.White;
        btnTabLevels.ForeColor = _currentTab == 1 ? Color.White : Theme.TextDark;
        btnTabLevels.Font = _currentTab == 1 ? Theme.FontBold : Theme.FontRegular;

        btnTabPrograms.BackColor = _currentTab == 2 ? Theme.Primary : Color.White;
        btnTabPrograms.ForeColor = _currentTab == 2 ? Color.White : Theme.TextDark;
        btnTabPrograms.Font = _currentTab == 2 ? Theme.FontBold : Theme.FontRegular;

        dgvCourses.Visible = _currentTab == 0;
        dgvLevels.Visible = _currentTab == 1;
        dgvPrograms.Visible = _currentTab == 2;

        btnAdd.Text = _currentTab switch
        {
            0 => "+ Thêm khóa học",
            1 => "+ Thêm cấp độ",
            2 => "+ Thêm chương trình",
            _ => "+ Thêm mới"
        };

        cboChuongTrinh.Visible = _currentTab != 2;

        FillStatusFilter();
        ApplyFilter();
    }

    /// <summary>Danh mục trạng thái lấy từ dữ liệu thật của tab đang chọn.</summary>
    private void FillStatusFilter()
    {
        _dangNap = true;

        var dangChon = cboTrangThai.SelectedItem?.ToString() ?? AllOption;

        IEnumerable<string> muc = _currentTab switch
        {
            0 => _courses.Select(c => c.TrangThai),
            1 => _levels.Select(l => l.TrangThai),
            _ => _programs.Select(p => p.TrangThai)
        };

        cboTrangThai.Items.Clear();
        cboTrangThai.Items.Add(AllOption);
        foreach (var m in muc.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x))
        {
            cboTrangThai.Items.Add(m);
        }

        var viTri = cboTrangThai.Items.IndexOf(dangChon);
        cboTrangThai.SelectedIndex = viTri >= 0 ? viTri : 0;

        _dangNap = false;
    }

    private void ApplyFilter()
    {
        var kw = txtSearch.Text.Trim();
        var prog = cboChuongTrinh.SelectedItem?.ToString() ?? AllOption;
        var status = cboTrangThai.SelectedItem?.ToString() ?? AllOption;

        if (_currentTab == 0)
        {
            var courses = _courses.AsEnumerable();
            if (prog != AllOption) courses = courses.Where(c => c.TenChuongTrinh == prog);
            if (status != AllOption) courses = courses.Where(c => c.TrangThai == status);
            if (!string.IsNullOrWhiteSpace(kw))
            {
                courses = courses.Where(c => c.TenKhoaHoc.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                             c.MaKhoaHoc.ToString().Contains(kw, StringComparison.OrdinalIgnoreCase));
            }

            var list = courses.OrderBy(c => c.TenKhoaHoc).ToList();
            dgvCourses.Rows.Clear();
            foreach (var c in list)
            {
                var rowIdx = dgvCourses.Rows.Add(
                    c.MaKhoaHoc.ToString(),
                    c.TenKhoaHoc,
                    c.TenChuongTrinh,
                    c.TenCapDo,
                    $"{c.SoBuoi} buổi",
                    c.TongSoGio.HasValue ? $"{c.TongSoGio.Value:0.#} giờ" : "—",
                    $"{c.HocPhiCoBan:N0} ₫",
                    UiHelpers.FormatDate(c.NgayBatDau),
                    c.TrangThai
                );
                dgvCourses.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = UiHelpers.StatusColor(c.TrangThai);
                dgvCourses.Rows[rowIdx].Tag = c;
            }
            lblSummary.Text = $"Hiển thị {list.Count}/{_courses.Count} khóa học phù hợp";
        }
        else if (_currentTab == 1)
        {
            var levels = _levels.AsEnumerable();
            if (prog != AllOption) levels = levels.Where(l => l.TenChuongTrinh == prog);
            if (status != AllOption) levels = levels.Where(l => l.TrangThai == status);
            if (!string.IsNullOrWhiteSpace(kw))
            {
                levels = levels.Where(l => l.TenCapDo.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                           l.KyHieuCapDo.Contains(kw, StringComparison.OrdinalIgnoreCase));
            }

            var list = levels.OrderBy(l => l.TenCapDo).ToList();
            dgvLevels.Rows.Clear();
            foreach (var l in list)
            {
                var rowIdx = dgvLevels.Rows.Add(
                    l.MaCapDo,
                    l.TenCapDo,
                    l.KyHieuCapDo,
                    l.TenChuongTrinh,
                    l.ThuTu,
                    l.ThoiLuongDuKienTuan.HasValue ? $"{l.ThoiLuongDuKienTuan} tuần" : "—",
                    l.DieuKienDauVao ?? "—",
                    l.KetQuaDauRa ?? "—",
                    l.TrangThai
                );
                dgvLevels.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = UiHelpers.StatusColor(l.TrangThai);
            }
            lblSummary.Text = $"Hiển thị {list.Count}/{_levels.Count} cấp độ đào tạo phù hợp";
        }
        else
        {
            var progs = _programs.AsEnumerable();
            if (status != AllOption) progs = progs.Where(p => p.TrangThai == status);
            if (!string.IsNullOrWhiteSpace(kw))
            {
                progs = progs.Where(p => p.TenChuongTrinh.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                         (p.NhomDoiTuong != null && p.NhomDoiTuong.Contains(kw, StringComparison.OrdinalIgnoreCase)));
            }

            var list = progs.OrderBy(p => p.TenChuongTrinh).ToList();
            dgvPrograms.Rows.Clear();
            foreach (var p in list)
            {
                var doTuoi = p.DoTuoiTu.HasValue || p.DoTuoiDen.HasValue
                    ? $"{p.DoTuoiTu ?? 0} - {p.DoTuoiDen ?? 0} tuổi"
                    : "—";

                var rowIdx = dgvPrograms.Rows.Add(
                    p.MaChuongTrinh,
                    p.TenChuongTrinh,
                    p.NhomDoiTuong ?? "—",
                    doTuoi,
                    p.MucTieu ?? "—",
                    p.MoTa ?? "—",
                    p.TrangThai
                );
                dgvPrograms.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = UiHelpers.StatusColor(p.TrangThai);
            }
            lblSummary.Text = $"Hiển thị {list.Count}/{_programs.Count} chương trình đào tạo phù hợp";
        }
    }

    private void ReloadData()
    {
        _ = ReloadAsync();
    }

    private async void BtnAdd_Click(object? sender, EventArgs e)
    {
        if (_currentTab == 0)
        {
            using var dlg = new FormKhoaHoc(_levels);
            if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
            {
                await ReloadAsync();
            }
            return;
        }

        var itemType = _currentTab switch { 1 => "cấp độ", 2 => "chương trình", _ => "mục" };

        MessageBox.Show(
            FindForm(),
            $"Chức năng thêm {itemType} mới chưa hỗ trợ trên bản WinForms.\n"
            + "Vui lòng dùng bản Web (Quản trị → Chương trình) để tạo mới.",
            $"Thêm {itemType} mới",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private async void DgvCourses_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        if (dgvCourses.Rows[e.RowIndex].Tag is not CourseListItemDto khoaHoc) return;

        using var dlg = new FormKhoaHoc(_levels, khoaHoc);
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            await ReloadAsync();
        }
    }
}
