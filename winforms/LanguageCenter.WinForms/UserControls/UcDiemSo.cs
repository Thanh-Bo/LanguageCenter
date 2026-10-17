using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcDiemSo : UserControl
{
    private const string AllOption = "— Tất cả —";

    public UcDiemSo()
    {
        InitializeComponent();

        UI.UiHelpers.StyleGrid(dgvGrades);
        dgvGrades.ReadOnly = false;
        dgvGrades.CellValueChanged += DgvGrades_CellValueChanged;

        cboLopHoc.SelectedIndexChanged += (_, _) => LoadGradesForClass();
        btnRefresh.Click += (_, _) => ReloadData();
        btnCalculate.Click += (_, _) => CalculateGrades();
        btnSave.Click += BtnSave_Click;
        btnExport.Click += BtnExport_Click;

        InitClassOptions();
    }

    private void InitClassOptions()
    {
        cboLopHoc.Items.Clear();
        cboLopHoc.Items.Add(AllOption);
        foreach (var c in MockDataStore.GetClasses())
        {
            cboLopHoc.Items.Add($"{c.MaLop} — {c.TenLop}");
        }

        if (cboLopHoc.Items.Count > 1) cboLopHoc.SelectedIndex = 1;
        else cboLopHoc.SelectedIndex = 0;
    }

    private void LoadGradesForClass()
    {
        var selected = cboLopHoc.SelectedItem?.ToString();
        string? maLop = null;
        if (!string.IsNullOrWhiteSpace(selected) && selected != AllOption)
        {
            maLop = selected.Split(" — ")[0].Trim();
        }

        var list = MockDataStore.GetGrades(maLop);
        dgvGrades.Rows.Clear();

        foreach (var g in list)
        {
            var rowIdx = dgvGrades.Rows.Add(
                g.MaHocVien,
                g.TenHocVien,
                g.DiemChuyenCan.ToString("F1"),
                g.DiemGiuaKy.ToString("F1"),
                g.DiemSpeaking.ToString("F1"),
                g.DiemWriting.ToString("F1"),
                g.DiemCuoiKy.ToString("F1"),
                g.DiemTongKet.ToString("F1"),
                g.XepLoai,
                g.NhanXet ?? string.Empty
            );

            dgvGrades.Rows[rowIdx].Tag = g.MaDangKy;
            StyleGradeRow(rowIdx, g.XepLoai);
        }

        UpdateStats(list);
        lblSummary.Text = $"Hiển thị bảng điểm {list.Count} học viên (Có thể chỉnh sửa điểm trực tiếp và bấm 'Lưu bảng điểm')";
    }

    private void StyleGradeRow(int rowIdx, string xepLoai)
    {
        var cell = dgvGrades.Rows[rowIdx].Cells["XepLoai"];
        cell.Style.Font = Theme.FontBold;
        cell.Style.ForeColor = xepLoai switch
        {
            "Xuất sắc" => Theme.Purple,
            "Giỏi" => Theme.Success,
            "Khá" => Theme.Primary,
            "Đạt" => Theme.Warning,
            _ => Theme.Danger
        };

        dgvGrades.Rows[rowIdx].Cells["DiemTongKet"].Style.Font = Theme.FontBold;
    }

    private void UpdateStats(List<DiemSoHocVienDto> list)
    {
        tblStats.Controls.Clear();
        if (list.Count == 0)
        {
            tblStats.Controls.Add(CreateStatCard("Sĩ số lớp", "0 HV", "Chưa có học viên", Theme.Primary), 0, 0);
            tblStats.Controls.Add(CreateStatCard("Điểm TB lớp", "0.0", "Chưa có điểm", Theme.Success), 1, 0);
            tblStats.Controls.Add(CreateStatCard("Tỷ lệ Đạt", "0%", "Tiêu chuẩn đầu ra", Theme.Warning), 2, 0);
            tblStats.Controls.Add(CreateStatCard("Xếp loại cao nhất", "—", "Học viên xuất sắc", Theme.Purple), 3, 0);
            return;
        }

        var avg = list.Average(g => g.DiemTongKet);
        var passCount = list.Count(g => g.XepLoai != "Không đạt");
        var passRate = Math.Round((double)passCount / list.Count * 100, 1);
        var top = list.OrderByDescending(g => g.DiemTongKet).First();

        tblStats.Controls.Add(CreateStatCard("Sĩ số lớp", $"{list.Count} Học viên", "Đang theo học lớp", Theme.Primary), 0, 0);
        tblStats.Controls.Add(CreateStatCard("Điểm TB lớp", $"{avg:F1} Điểm", "Thang điểm 10 chuẩn đào tạo", Theme.Success), 1, 0);
        tblStats.Controls.Add(CreateStatCard("Tỷ lệ Đạt", $"{passRate}%", $"{passCount}/{list.Count} học viên đạt chuẩn", Theme.Warning), 2, 0);
        tblStats.Controls.Add(CreateStatCard("Xếp loại cao nhất", $"{top.TenHocVien} ({top.DiemTongKet:F1})", top.XepLoai, Theme.Purple), 3, 0);
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

    private void DgvGrades_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var colName = dgvGrades.Columns[e.ColumnIndex].Name;
        if (colName is "DiemChuyenCan" or "DiemGiuaKy" or "DiemSpeaking" or "DiemWriting" or "DiemCuoiKy")
        {
            CalculateRowGrade(e.RowIndex);
        }
    }

    private void CalculateRowGrade(int rowIdx)
    {
        var row = dgvGrades.Rows[rowIdx];
        decimal.TryParse(row.Cells["DiemChuyenCan"].Value?.ToString(), out var cc);
        decimal.TryParse(row.Cells["DiemGiuaKy"].Value?.ToString(), out var gk);
        decimal.TryParse(row.Cells["DiemSpeaking"].Value?.ToString(), out var sp);
        decimal.TryParse(row.Cells["DiemWriting"].Value?.ToString(), out var wr);
        decimal.TryParse(row.Cells["DiemCuoiKy"].Value?.ToString(), out var ck);

        var tk = Math.Round(cc * 0.1m + gk * 0.2m + sp * 0.2m + wr * 0.2m + ck * 0.3m, 1);
        var xl = tk switch
        {
            >= 8.5m => "Xuất sắc",
            >= 7.0m => "Giỏi",
            >= 6.0m => "Khá",
            >= 5.0m => "Đạt",
            _ => "Không đạt"
        };

        row.Cells["DiemTongKet"].Value = tk.ToString("F1");
        row.Cells["XepLoai"].Value = xl;
        StyleGradeRow(rowIdx, xl);
    }

    private void CalculateGrades()
    {
        for (int i = 0; i < dgvGrades.Rows.Count; i++)
        {
            CalculateRowGrade(i);
        }
        MessageBox.Show(FindForm(), "Đã tính toán lại toàn bộ điểm tổng kết và xếp loại của lớp học.", "Tính điểm thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        for (int i = 0; i < dgvGrades.Rows.Count; i++)
        {
            CalculateRowGrade(i);
            var row = dgvGrades.Rows[i];
            var regId = Convert.ToInt32(row.Tag ?? i + 1);

            decimal.TryParse(row.Cells["DiemChuyenCan"].Value?.ToString(), out var cc);
            decimal.TryParse(row.Cells["DiemGiuaKy"].Value?.ToString(), out var gk);
            decimal.TryParse(row.Cells["DiemSpeaking"].Value?.ToString(), out var sp);
            decimal.TryParse(row.Cells["DiemWriting"].Value?.ToString(), out var wr);
            decimal.TryParse(row.Cells["DiemCuoiKy"].Value?.ToString(), out var ck);
            decimal.TryParse(row.Cells["DiemTongKet"].Value?.ToString(), out var tk);

            var item = new DiemSoHocVienDto
            {
                MaDangKy = regId,
                MaHocVien = row.Cells["MaHocVien"].Value?.ToString() ?? "",
                TenHocVien = row.Cells["TenHocVien"].Value?.ToString() ?? "",
                MaLop = cboLopHoc.SelectedItem?.ToString()?.Split(" — ")[0].Trim() ?? "",
                DiemChuyenCan = cc,
                DiemGiuaKy = gk,
                DiemSpeaking = sp,
                DiemWriting = wr,
                DiemCuoiKy = ck,
                DiemTongKet = tk,
                XepLoai = row.Cells["XepLoai"].Value?.ToString() ?? "Đạt",
                NhanXet = row.Cells["NhanXet"].Value?.ToString(),
                NgayCham = DateTime.Now
            };

            MockDataStore.SaveGrade(item);
        }

        MessageBox.Show(FindForm(), "Bảng điểm đã được lưu thành công vào cơ sở dữ liệu hệ thống!", "Lưu thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        LoadGradesForClass();
    }

    private void BtnExport_Click(object? sender, EventArgs e)
    {
        MessageBox.Show(
            FindForm(),
            $"Bảng điểm chính thức đã được xuất ra định dạng báo cáo chuẩn.\nTổng số học viên: {dgvGrades.Rows.Count}.",
            "Xuất bảng điểm",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void ReloadData()
    {
        LoadGradesForClass();
    }
}

