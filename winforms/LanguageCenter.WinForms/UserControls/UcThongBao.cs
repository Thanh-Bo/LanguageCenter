using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcThongBao : UserControl
{
    private const string AllOption = "— Tất cả —";

    public UcThongBao()
    {
        InitializeComponent();

        UI.UiHelpers.StyleGrid(dgvNotifications);
        dgvNotifications.CellDoubleClick += DgvNotifications_CellDoubleClick;

        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; FilterData(); } };
        cboLoai.SelectedIndexChanged += (_, _) => FilterData();
        cboDoiTuong.SelectedIndexChanged += (_, _) => FilterData();
        btnSearch.Click += (_, _) => FilterData();
        btnRefresh.Click += (_, _) => ReloadData();
        btnAdd.Click += BtnAdd_Click;

        pnlToolbar.Resize += (_, _) =>
        {
            btnAdd.Location = new Point(pnlToolbar.Width - btnAdd.Width - 4, 3);
        };

        InitStats();
        InitFilterOptions();
        LoadNotifications();
    }

    private void InitStats()
    {
        var list = MockDataStore.GetNotifications();
        var hocVu = list.Count(n => n.LoaiThongBao == "Học vụ");
        var lichNghi = list.Count(n => n.LoaiThongBao.Contains("Lịch"));
        var hocPhi = list.Count(n => n.LoaiThongBao == "Học phí");

        tblStats.Controls.Clear();
        tblStats.Controls.Add(CreateStatCard("Tổng số thông báo", $"{list.Count} Bản tin", "Hệ thống tin tức nội bộ", Theme.Primary), 0, 0);
        tblStats.Controls.Add(CreateStatCard("Thông báo học vụ", $"{hocVu} Bản tin", "Thi cử, chuyên môn & lớp học", Theme.Success), 1, 0);
        tblStats.Controls.Add(CreateStatCard("Lịch nghỉ & Khai giảng", $"{lichNghi} Bản tin", "Kế hoạch đào tạo trung tâm", Theme.Purple), 2, 0);
        tblStats.Controls.Add(CreateStatCard("Thông báo học phí", $"{hocPhi} Bản tin", "Tài chính & hạn nộp học phí", Theme.Warning), 3, 0);
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
        cboLoai.Items.Clear();
        cboLoai.Items.Add(AllOption);
        cboLoai.Items.AddRange(["Học vụ", "Lịch nghỉ / Khai giảng", "Học phí", "Sự kiện", "Khẩn cấp"]);
        cboLoai.SelectedIndex = 0;

        cboDoiTuong.Items.Clear();
        cboDoiTuong.Items.Add(AllOption);
        cboDoiTuong.Items.AddRange(["Toàn trung tâm", "Tất cả giáo viên", "Tất cả học viên"]);
        cboDoiTuong.SelectedIndex = 0;
    }

    public void LoadNotifications()
    {
        FilterData();
    }

    private void FilterData()
    {
        var kw = txtSearch.Text.Trim();
        var loai = cboLoai.SelectedItem?.ToString() ?? AllOption;
        var dt = cboDoiTuong.SelectedItem?.ToString() ?? AllOption;

        var query = MockDataStore.GetNotifications().AsEnumerable();

        if (loai != AllOption) query = query.Where(n => n.LoaiThongBao == loai);
        if (dt != AllOption) query = query.Where(n => n.DoiTuongNhan == dt);

        if (!string.IsNullOrWhiteSpace(kw))
        {
            query = query.Where(n => n.TieuDe.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                     n.NoiDung.Contains(kw, StringComparison.OrdinalIgnoreCase));
        }

        var list = query.ToList();
        dgvNotifications.Rows.Clear();

        foreach (var n in list)
        {
            var rowIdx = dgvNotifications.Rows.Add(
                $"TB-{n.MaThongBao:D3}",
                n.TieuDe,
                n.LoaiThongBao,
                n.DoiTuongNhan,
                n.NguoiGui,
                UiHelpers.FormatDateTime(n.ThoiGianGui),
                n.TrangThai
            );

            dgvNotifications.Rows[rowIdx].Tag = n;
            dgvNotifications.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = Theme.Success;
        }

        lblSummary.Text = $"Hiển thị {list.Count} thông báo (Nhấp đúp chuột để xem chi tiết)";
    }

    private void ReloadData()
    {
        txtSearch.Clear();
        cboLoai.SelectedIndex = 0;
        cboDoiTuong.SelectedIndex = 0;
        InitStats();
        FilterData();
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        using var dlg = new FormSoanThongBao();
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            InitStats();
            FilterData();
        }
    }

    private void DgvNotifications_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        if (dgvNotifications.Rows[e.RowIndex].Tag is ThongBaoDto notif)
        {
            MessageBox.Show(
                FindForm(),
                $"TIÊU ĐỀ: {notif.TieuDe}\n" +
                $"Phân loại: {notif.LoaiThongBao}  |  Gửi tới: {notif.DoiTuongNhan}\n" +
                $"Thời gian: {UiHelpers.FormatDateTime(notif.ThoiGianGui)} — Người gửi: {notif.NguoiGui}\n\n" +
                $"NỘI DUNG:\n{notif.NoiDung}",
                "Chi tiết thông báo",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
        }
    }
}

