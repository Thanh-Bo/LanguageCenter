using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcLichHoc : UserControl
{
    private const string AllOption = "— Tất cả —";
    private int _currentTab = 0; // 0: Phòng học, 1: Lịch học tuần, 2: Ca học thực tế

    public UcLichHoc()
    {
        InitializeComponent();

        UI.UiHelpers.StyleGrid(dgvRooms);
        UI.UiHelpers.StyleGrid(dgvSchedules);
        UI.UiHelpers.StyleGrid(dgvShifts);

        btnTabRooms.Click += (_, _) => SwitchTab(0);
        btnTabSchedules.Click += (_, _) => SwitchTab(1);
        btnTabShifts.Click += (_, _) => SwitchTab(2);

        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; FilterData(); } };
        cboFilter.SelectedIndexChanged += (_, _) => FilterData();
        btnSearch.Click += (_, _) => FilterData();
        btnRefresh.Click += (_, _) => ReloadData();
        btnAdd.Click += BtnAdd_Click;

        pnlToolbar.Resize += (_, _) =>
        {
            btnAdd.Location = new Point(pnlToolbar.Width - btnAdd.Width - 4, 3);
        };

        InitStats();
        SwitchTab(0);
    }

    private void InitStats()
    {
        var rooms = MockDataStore.GetRooms();
        var schedules = MockDataStore.GetSchedules();
        var shifts = MockDataStore.GetShifts();
        var usingRooms = rooms.Count(r => r.TrangThai == "Đang sử dụng");

        tblStats.Controls.Clear();
        tblStats.Controls.Add(CreateStatCard("Tổng số phòng học", $"{rooms.Count} Phòng", "Các phòng chuẩn âm thanh & máy lạnh", Theme.Primary), 0, 0);
        tblStats.Controls.Add(CreateStatCard("Đang sử dụng", $"{usingRooms} Phòng", "Các lớp đang học trong ca", Theme.Success), 1, 0);
        tblStats.Controls.Add(CreateStatCard("Lịch học tuần", $"{schedules.Count} Thời khóa biểu", "Lịch học cố định theo lớp", Theme.Purple), 2, 0);
        tblStats.Controls.Add(CreateStatCard("Ca học thực tế", $"{shifts.Count} Buổi học", "Tiến độ bài học đang diễn ra", Theme.Warning), 3, 0);
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

    private void SwitchTab(int tabIndex)
    {
        _currentTab = tabIndex;

        btnTabRooms.BackColor = _currentTab == 0 ? Theme.Primary : Color.White;
        btnTabRooms.ForeColor = _currentTab == 0 ? Color.White : Theme.TextDark;
        btnTabRooms.Font = _currentTab == 0 ? Theme.FontBold : Theme.FontRegular;

        btnTabSchedules.BackColor = _currentTab == 1 ? Theme.Primary : Color.White;
        btnTabSchedules.ForeColor = _currentTab == 1 ? Color.White : Theme.TextDark;
        btnTabSchedules.Font = _currentTab == 1 ? Theme.FontBold : Theme.FontRegular;

        btnTabShifts.BackColor = _currentTab == 2 ? Theme.Primary : Color.White;
        btnTabShifts.ForeColor = _currentTab == 2 ? Color.White : Theme.TextDark;
        btnTabShifts.Font = _currentTab == 2 ? Theme.FontBold : Theme.FontRegular;

        dgvRooms.Visible = _currentTab == 0;
        dgvSchedules.Visible = _currentTab == 1;
        dgvShifts.Visible = _currentTab == 2;

        btnAdd.Text = _currentTab switch
        {
            0 => "Thêm phòng học",
            1 => "Xếp lịch học",
            2 => "Thêm ca học",
            _ => "Thêm mới"
        };

        cboFilter.Items.Clear();
        cboFilter.Items.Add(AllOption);

        if (_currentTab == 0)
        {
            cboFilter.Items.AddRange(["Sẵn sàng", "Đang sử dụng", "Bảo trì"]);
        }
        else
        {
            foreach (var c in MockDataStore.GetClasses()) cboFilter.Items.Add(c.TenLop);
        }
        cboFilter.SelectedIndex = 0;

        FilterData();
    }

    private void FilterData()
    {
        var kw = txtSearch.Text.Trim();
        var filter = cboFilter.SelectedItem?.ToString() ?? AllOption;

        if (_currentTab == 0)
        {
            var query = MockDataStore.GetRooms().AsEnumerable();
            if (filter != AllOption) query = query.Where(r => r.TrangThai == filter);
            if (!string.IsNullOrWhiteSpace(kw))
            {
                query = query.Where(r => r.TenPhong.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                         r.MaPhong.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                         (r.ThietBi != null && r.ThietBi.Contains(kw, StringComparison.OrdinalIgnoreCase)));
            }

            var list = query.ToList();
            dgvRooms.Rows.Clear();
            foreach (var r in list)
            {
                var rowIdx = dgvRooms.Rows.Add(
                    r.MaPhong,
                    r.TenPhong,
                    r.ViTri ?? "—",
                    $"{r.SucChua} chỗ",
                    r.ThietBi ?? "—",
                    r.TrangThai
                );
                dgvRooms.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = UiHelpers.StatusColor(r.TrangThai);
            }
            lblSummary.Text = $"Hiển thị {list.Count} phòng học phù hợp";
        }
        else if (_currentTab == 1)
        {
            var query = MockDataStore.GetSchedules().AsEnumerable();
            if (filter != AllOption) query = query.Where(s => s.TenLop == filter);
            if (!string.IsNullOrWhiteSpace(kw))
            {
                query = query.Where(s => s.TenLop.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                         s.TenPhong.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                         s.ThuText.Contains(kw, StringComparison.OrdinalIgnoreCase));
            }

            var list = query.ToList();
            dgvSchedules.Rows.Clear();
            foreach (var s in list)
            {
                var rowIdx = dgvSchedules.Rows.Add(
                    s.MaLop,
                    s.TenLop,
                    s.TenPhong,
                    s.ThuText,
                    $"{s.GioBatDau:HH\\:mm} - {s.GioKetThuc:HH\\:mm}",
                    UiHelpers.FormatDate(s.NgayApDungTu),
                    s.TrangThai
                );
                dgvSchedules.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = UiHelpers.StatusColor(s.TrangThai);
            }
            lblSummary.Text = $"Hiển thị {list.Count} thời khóa biểu tuần";
        }
        else
        {
            var query = MockDataStore.GetShifts().AsEnumerable();
            if (filter != AllOption) query = query.Where(s => s.TenLop == filter);
            if (!string.IsNullOrWhiteSpace(kw))
            {
                query = query.Where(s => s.TenLop.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                         (s.NoiDungBaiHoc != null && s.NoiDungBaiHoc.Contains(kw, StringComparison.OrdinalIgnoreCase)) ||
                                         (s.TenGiaoVien != null && s.TenGiaoVien.Contains(kw, StringComparison.OrdinalIgnoreCase)));
            }

            var list = query.ToList();
            dgvShifts.Rows.Clear();
            foreach (var s in list)
            {
                var rowIdx = dgvShifts.Rows.Add(
                    s.MaCaHoc,
                    UiHelpers.FormatDate(s.NgayHoc),
                    s.TenLop,
                    s.TenPhong,
                    s.TenGiaoVien ?? "—",
                    s.TenTroGiang ?? "—",
                    $"{s.GioBatDau:HH\\:mm} - {s.GioKetThuc:HH\\:mm}",
                    s.NoiDungBaiHoc ?? "—",
                    s.TrangThai
                );
                dgvShifts.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = UiHelpers.StatusColor(s.TrangThai);
            }
            lblSummary.Text = $"Hiển thị {list.Count} ca học thực tế";
        }
    }

    private void ReloadData()
    {
        txtSearch.Clear();
        cboFilter.SelectedIndex = 0;
        InitStats();
        FilterData();
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        var itemType = _currentTab switch { 0 => "phòng học", 1 => "lịch học tuần", 2 => "ca học", _ => "mục" };
        MessageBox.Show(
            FindForm(),
            $"Biểu mẫu thiết lập {itemType} mới đã sẵn sàng.\nBạn có thể cấu hình tài nguyên phòng và lịch trình đào tạo.",
            $"Thêm {itemType} mới",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}

