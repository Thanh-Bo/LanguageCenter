using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcPhanCongGiangDay : UserControl
{
    private const string AllOption = "— Tất cả —";

    public UcPhanCongGiangDay()
    {
        InitializeComponent();

        UI.UiHelpers.StyleGrid(dgvAssignments);

        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; FilterData(); } };
        cboLopHoc.SelectedIndexChanged += (_, _) => FilterData();
        cboVaiTro.SelectedIndexChanged += (_, _) => FilterData();
        btnSearch.Click += (_, _) => FilterData();
        btnRefresh.Click += (_, _) => ReloadData();
        btnAdd.Click += BtnAdd_Click;
        btnDelete.Click += BtnDelete_Click;

        pnlToolbar.Resize += (_, _) =>
        {
            btnDelete.Location = new Point(pnlToolbar.Width - btnDelete.Width - 4, 3);
            btnAdd.Location = new Point(btnDelete.Left - btnAdd.Width - 8, 3);
        };

        InitStats();
        InitFilterOptions();
        LoadAssignments();
    }

    private void InitStats()
    {
        var list = MockDataStore.GetAssignments();
        var gvChinh = list.Count(a => a.VaiTroPhanCong == "Giáo viên chính");
        var troGiang = list.Count(a => a.VaiTroPhanCong == "Trợ giảng");

        tblStats.Controls.Clear();
        tblStats.Controls.Add(CreateStatCard("Tổng lượt phân công", $"{list.Count} Phân công", "Học vụ đào tạo đang chạy", Theme.Primary), 0, 0);
        tblStats.Controls.Add(CreateStatCard("Giảng viên chính", $"{gvChinh} Giảng viên", "Đang trực tiếp đứng lớp", Theme.Success), 1, 0);
        tblStats.Controls.Add(CreateStatCard("Trợ giảng học vụ", $"{troGiang} Trợ giảng", "Hỗ trợ chấm bài & học thuật", Theme.Purple), 2, 0);
        tblStats.Controls.Add(CreateStatCard("Tiến độ giảng dạy", "100%", "Tất cả các lớp đã đủ nhân sự", Theme.Warning), 3, 0);
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
        cboLopHoc.Items.Clear();
        cboLopHoc.Items.Add(AllOption);
        foreach (var c in MockDataStore.GetClasses()) cboLopHoc.Items.Add(c.TenLop);
        cboLopHoc.SelectedIndex = 0;

        cboVaiTro.Items.Clear();
        cboVaiTro.Items.Add(AllOption);
        cboVaiTro.Items.AddRange(["Giáo viên chính", "Trợ giảng"]);
        cboVaiTro.SelectedIndex = 0;
    }

    public void LoadAssignments()
    {
        FilterData();
    }

    private void FilterData()
    {
        var kw = txtSearch.Text.Trim();
        var lop = cboLopHoc.SelectedItem?.ToString() ?? AllOption;
        var vaiTro = cboVaiTro.SelectedItem?.ToString() ?? AllOption;

        var query = MockDataStore.GetAssignments().AsEnumerable();

        if (lop != AllOption) query = query.Where(a => a.TenLop == lop);
        if (vaiTro != AllOption) query = query.Where(a => a.VaiTroPhanCong == vaiTro);

        if (!string.IsNullOrWhiteSpace(kw))
        {
            query = query.Where(a => a.TenGiaoVien.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                     a.MaGiaoVien.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                     a.TenLop.Contains(kw, StringComparison.OrdinalIgnoreCase));
        }

        var list = query.ToList();
        dgvAssignments.Rows.Clear();

        foreach (var a in list)
        {
            var rowIdx = dgvAssignments.Rows.Add(
                a.MaPhanCong,
                a.TenLop,
                a.MaGiaoVien,
                a.TenGiaoVien,
                a.LoaiNhanSu,
                a.VaiTroPhanCong,
                UiHelpers.FormatDate(a.NgayBatDau),
                a.TrangThai,
                a.GhiChu ?? "—"
            );
            dgvAssignments.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = UiHelpers.StatusColor(a.TrangThai);
        }

        lblSummary.Text = $"Hiển thị {list.Count} lượt phân công nhân sự";
    }

    private void ReloadData()
    {
        txtSearch.Clear();
        cboLopHoc.SelectedIndex = 0;
        cboVaiTro.SelectedIndex = 0;
        InitStats();
        FilterData();
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        using var dlg = new FormSuaPhanCong();
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            InitStats();
            FilterData();
        }
    }

    private void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (dgvAssignments.CurrentRow == null)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn dòng phân công cần hủy.", "Chưa chọn dòng", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var idVal = dgvAssignments.CurrentRow.Cells["MaPhanCong"].Value;
        if (idVal is int id || (idVal is string s && int.TryParse(s, out id)))
        {
            var teacherName = dgvAssignments.CurrentRow.Cells["TenGiaoVien"].Value?.ToString() ?? "nhân sự";
            var lopName = dgvAssignments.CurrentRow.Cells["TenLop"].Value?.ToString() ?? "lớp";

            var confirm = MessageBox.Show(
                FindForm(),
                $"Bạn có chắc chắn muốn hủy phân công {teacherName} tại lớp {lopName}?",
                "Xác nhận hủy phân công",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm == DialogResult.Yes)
            {
                MockDataStore.DeleteAssignment(id);
                InitStats();
                FilterData();
            }
        }
    }
}

