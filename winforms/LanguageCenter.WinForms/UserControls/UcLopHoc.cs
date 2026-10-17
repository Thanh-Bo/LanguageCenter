using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcLopHoc : UserControl
{
    private const string AllOption = "— Tất cả —";

    public UcLopHoc()
    {
        InitializeComponent();

        UI.UiHelpers.StyleGrid(dgvClasses);
        dgvClasses.CellDoubleClick += DgvClasses_CellDoubleClick;

        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; FilterData(); } };
        cboKhoaHoc.SelectedIndexChanged += (_, _) => FilterData();
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
        LoadClasses();
    }

    private void InitStats()
    {
        var classes = MockDataStore.GetClasses();
        var dangHoc = classes.Count(c => c.TrangThai == "Đang học");
        var sapMo = classes.Count(c => c.TrangThai == "Sắp khai giảng");
        var ketThuc = classes.Count(c => c.TrangThai == "Đã kết thúc");

        tblStats.Controls.Clear();
        tblStats.Controls.Add(CreateStatCard("Tổng số lớp học", $"{classes.Count} Lớp", "Đang quản lý trên hệ thống", Theme.Primary), 0, 0);
        tblStats.Controls.Add(CreateStatCard("Đang diễn ra", $"{dangHoc} Lớp", "Lớp đang trong tiến độ học", Theme.Success), 1, 0);
        tblStats.Controls.Add(CreateStatCard("Sắp khai giảng", $"{sapMo} Lớp", "Chuẩn bị mở trong kỳ tới", Theme.Warning), 2, 0);
        tblStats.Controls.Add(CreateStatCard("Đã kết thúc", $"{ketThuc} Lớp", "Khóa học đã hoàn tất", Theme.Muted), 3, 0);
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
        cboKhoaHoc.Items.Clear();
        cboKhoaHoc.Items.Add(AllOption);
        foreach (var c in MockDataStore.GetCourses())
        {
            cboKhoaHoc.Items.Add(c.TenKhoaHoc);
        }
        cboKhoaHoc.SelectedIndex = 0;

        cboTrangThai.Items.Clear();
        cboTrangThai.Items.Add(AllOption);
        cboTrangThai.Items.AddRange(["Đang học", "Sắp khai giảng", "Đã kết thúc"]);
        cboTrangThai.SelectedIndex = 0;
    }

    public void LoadClasses()
    {
        FilterData();
    }

    private void FilterData()
    {
        var kw = txtSearch.Text.Trim();
        var course = cboKhoaHoc.SelectedItem?.ToString() ?? AllOption;
        var status = cboTrangThai.SelectedItem?.ToString() ?? AllOption;

        var query = MockDataStore.GetClasses().AsEnumerable();

        if (course != AllOption) query = query.Where(c => c.TenKhoaHoc == course);
        if (status != AllOption) query = query.Where(c => c.TrangThai == status);

        if (!string.IsNullOrWhiteSpace(kw))
        {
            query = query.Where(c => c.MaLop.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                     c.TenLop.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                     c.TenGiaoVienChinh.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                     c.TenPhongHoc.Contains(kw, StringComparison.OrdinalIgnoreCase));
        }

        var list = query.ToList();
        dgvClasses.Rows.Clear();

        foreach (var c in list)
        {
            var rowIdx = dgvClasses.Rows.Add(
                c.MaLop,
                c.TenLop,
                c.TenKhoaHoc,
                c.TenGiaoVienChinh,
                c.TenTroGiang,
                c.TenPhongHoc,
                c.LichHoc,
                $"{c.SiSoHienTai}/{c.SiSoToiDa}",
                UiHelpers.FormatDate(c.NgayBatDau),
                UiHelpers.FormatDate(c.NgayKetThucDuKien),
                c.TrangThai
            );
            dgvClasses.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = UiHelpers.StatusColor(c.TrangThai);
        }

        lblSummary.Text = $"Hiển thị {list.Count} lớp học phù hợp với bộ lọc";
    }

    private void ReloadData()
    {
        txtSearch.Clear();
        cboKhoaHoc.SelectedIndex = 0;
        cboTrangThai.SelectedIndex = 0;
        InitStats();
        FilterData();
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        using var dlg = new FormSuaLopHoc();
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            InitStats();
            FilterData();
        }
    }

    private void DgvClasses_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var maLop = dgvClasses.Rows[e.RowIndex].Cells["MaLop"].Value?.ToString();
        if (string.IsNullOrWhiteSpace(maLop)) return;

        var cls = MockDataStore.GetClasses().FirstOrDefault(c => c.MaLop == maLop);
        if (cls is null) return;

        using var dlg = new FormSuaLopHoc(cls);
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            InitStats();
            FilterData();
        }
    }
}

