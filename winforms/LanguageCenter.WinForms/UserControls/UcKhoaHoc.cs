using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcKhoaHoc : UserControl
{
    private const string AllOption = "— Tất cả —";
    private int _currentTab = 0; // 0: Khóa học, 1: Cấp độ, 2: Chương trình

    public UcKhoaHoc()
    {
        InitializeComponent();

        UI.UiHelpers.StyleGrid(dgvCourses);
        UI.UiHelpers.StyleGrid(dgvLevels);
        UI.UiHelpers.StyleGrid(dgvPrograms);

        btnTabCourses.Click += (_, _) => SwitchTab(0);
        btnTabLevels.Click += (_, _) => SwitchTab(1);
        btnTabPrograms.Click += (_, _) => SwitchTab(2);

        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; FilterData(); } };
        cboChuongTrinh.SelectedIndexChanged += (_, _) => FilterData();
        cboTrangThai.SelectedIndexChanged += (_, _) => FilterData();
        btnSearch.Click += (_, _) => FilterData();
        btnRefresh.Click += (_, _) => ReloadData();
        btnAdd.Click += BtnAdd_Click;

        pnlToolbar.Resize += (_, _) =>
        {
            btnAdd.Location = new Point(pnlToolbar.Width - btnAdd.Width - 4, 3);
        };

        InitStats();
        InitFilterOptions();
        SwitchTab(0);
    }

    private void InitStats()
    {
        var courses = MockDataStore.GetCourses();
        var programs = MockDataStore.GetPrograms();
        var levels = MockDataStore.GetLevels();

        tblStats.Controls.Clear();
        tblStats.Controls.Add(CreateStatCard("Tổng số khóa học", $"{courses.Count} Khóa", "Các cấp độ từ A1 đến C1", Theme.Primary), 0, 0);
        tblStats.Controls.Add(CreateStatCard("Chương trình đào tạo", $"{programs.Count} Hệ", "IELTS, TOEIC, Giao tiếp, Thiếu nhi", Theme.Purple), 1, 0);
        tblStats.Controls.Add(CreateStatCard("Cấp độ đào tạo", $"{levels.Count} Cấp độ", "Chuẩn khung tham chiếu CEFR", Theme.Success), 2, 0);
        tblStats.Controls.Add(CreateStatCard("Đang tuyển sinh", $"{courses.Count(c => c.TrangThai == "Đang tuyển sinh")} Khóa", "Tuyển sinh kỳ học mới", Theme.Warning), 3, 0);
    }

    private static Panel CreateStatCard(string title, string value, string note, Color accent)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(12, 8, 12, 8),
            Margin = new Padding(4, 2, 4, 6)
        };
        panel.Paint += (s, e) =>
        {
            if (s is Control ctrl)
            {
                using var borderPen = new Pen(Theme.BorderColor);
                e.Graphics.DrawRectangle(borderPen, 0, 0, ctrl.Width - 1, ctrl.Height - 1);
                using var accentBrush = new SolidBrush(accent);
                e.Graphics.FillRectangle(accentBrush, 0, 0, 4, ctrl.Height);
            }
        };

        var lblT = new Label { Text = title, Font = new Font("Segoe UI", 8.5f), ForeColor = Theme.Muted, AutoSize = true, Location = new Point(14, 6) };
        var lblV = new Label { Text = value, Font = new Font("Segoe UI", 15f, FontStyle.Bold), ForeColor = accent, AutoSize = true, Location = new Point(12, 24) };
        var lblN = new Label { Text = note, Font = new Font("Segoe UI", 8f), ForeColor = Theme.Muted, AutoSize = true, Location = new Point(14, 56) };

        panel.Controls.Add(lblN);
        panel.Controls.Add(lblV);
        panel.Controls.Add(lblT);
        return panel;
    }

    private void InitFilterOptions()
    {
        cboChuongTrinh.Items.Clear();
        cboChuongTrinh.Items.Add(AllOption);
        foreach (var p in MockDataStore.GetPrograms())
        {
            cboChuongTrinh.Items.Add(p.TenChuongTrinh);
        }
        cboChuongTrinh.SelectedIndex = 0;

        cboTrangThai.Items.Clear();
        cboTrangThai.Items.Add(AllOption);
        cboTrangThai.Items.AddRange(["Đang mở", "Đang tuyển sinh", "Đang diễn ra", "Sắp khai giảng"]);
        cboTrangThai.SelectedIndex = 0;
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

        FilterData();
    }

    private void FilterData()
    {
        var kw = txtSearch.Text.Trim();
        var prog = cboChuongTrinh.SelectedItem?.ToString() ?? AllOption;
        var status = cboTrangThai.SelectedItem?.ToString() ?? AllOption;

        if (_currentTab == 0)
        {
            var courses = MockDataStore.GetCourses().AsEnumerable();
            if (prog != AllOption) courses = courses.Where(c => c.TenChuongTrinh == prog);
            if (status != AllOption) courses = courses.Where(c => c.TrangThai == status);
            if (!string.IsNullOrWhiteSpace(kw))
            {
                courses = courses.Where(c => c.TenKhoaHoc.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                             c.MaKhoaHocStr.Contains(kw, StringComparison.OrdinalIgnoreCase));
            }

            var list = courses.ToList();
            dgvCourses.Rows.Clear();
            foreach (var c in list)
            {
                var rowIdx = dgvCourses.Rows.Add(
                    c.MaKhoaHocStr,
                    c.TenKhoaHoc,
                    c.TenChuongTrinh,
                    c.TenCapDo,
                    $"{c.SoBuoi} buổi",
                    $"{c.TongSoGio} giờ",
                    $"{c.HocPhiCoBan:N0} ₫",
                    UiHelpers.FormatDate(c.NgayBatDau),
                    c.TrangThai
                );
                dgvCourses.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = UiHelpers.StatusColor(c.TrangThai);
            }
            lblSummary.Text = $"Hiển thị {list.Count} khóa học phù hợp";
        }
        else if (_currentTab == 1)
        {
            var levels = MockDataStore.GetLevels().AsEnumerable();
            if (prog != AllOption) levels = levels.Where(l => l.TenChuongTrinh == prog);
            if (status != AllOption) levels = levels.Where(l => l.TrangThai == status);
            if (!string.IsNullOrWhiteSpace(kw))
            {
                levels = levels.Where(l => l.TenCapDo.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                           l.KyHieuCapDo.Contains(kw, StringComparison.OrdinalIgnoreCase));
            }

            var list = levels.ToList();
            dgvLevels.Rows.Clear();
            foreach (var l in list)
            {
                var rowIdx = dgvLevels.Rows.Add(
                    l.MaCapDo,
                    l.TenCapDo,
                    l.KyHieuCapDo,
                    l.TenChuongTrinh,
                    l.ThuTu,
                    $"{l.ThoiLuongDuKienTuan} tuần",
                    l.DieuKienDauVao ?? "—",
                    l.KetQuaDauRa ?? "—",
                    l.TrangThai
                );
                dgvLevels.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = UiHelpers.StatusColor(l.TrangThai);
            }
            lblSummary.Text = $"Hiển thị {list.Count} cấp độ đào tạo phù hợp";
        }
        else
        {
            var progs = MockDataStore.GetPrograms().AsEnumerable();
            if (status != AllOption) progs = progs.Where(p => p.TrangThai == status);
            if (!string.IsNullOrWhiteSpace(kw))
            {
                progs = progs.Where(p => p.TenChuongTrinh.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                         (p.NhomDoiTuong != null && p.NhomDoiTuong.Contains(kw, StringComparison.OrdinalIgnoreCase)));
            }

            var list = progs.ToList();
            dgvPrograms.Rows.Clear();
            foreach (var p in list)
            {
                var rowIdx = dgvPrograms.Rows.Add(
                    p.MaChuongTrinh,
                    p.TenChuongTrinh,
                    p.NhomDoiTuong ?? "—",
                    $"{p.DoTuoiTu} - {p.DoTuoiDen} tuổi",
                    p.MucTieu ?? "—",
                    p.MoTa ?? "—",
                    p.TrangThai
                );
                dgvPrograms.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = UiHelpers.StatusColor(p.TrangThai);
            }
            lblSummary.Text = $"Hiển thị {list.Count} chương trình đào tạo phù hợp";
        }
    }

    private void ReloadData()
    {
        txtSearch.Clear();
        cboChuongTrinh.SelectedIndex = 0;
        cboTrangThai.SelectedIndex = 0;
        InitStats();
        FilterData();
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        var itemType = _currentTab switch { 0 => "khóa học", 1 => "cấp độ", 2 => "chương trình", _ => "mục" };
        MessageBox.Show(
            FindForm(),
            $"Biểu mẫu khởi tạo {itemType} mới đã sẵn sàng.\nBạn có thể điền thông tin và kích hoạt khóa học vào hệ thống đào tạo.",
            $"Thêm {itemType} mới",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
