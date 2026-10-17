using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcBaoCao : UserControl
{
    private int _currentReportIndex = 0;

    public UcBaoCao()
    {
        InitializeComponent();

        UI.UiHelpers.StyleGrid(dgvRevenue);
        UI.UiHelpers.StyleGrid(dgvQuality);
        UI.UiHelpers.StyleGrid(dgvFinancial);
        UI.UiHelpers.StyleGrid(dgvClasses);

        btnMenuRevenue.Click += (_, _) => SwitchReport(0);
        btnMenuQuality.Click += (_, _) => SwitchReport(1);
        btnMenuFinancial.Click += (_, _) => SwitchReport(2);
        btnMenuClasses.Click += (_, _) => SwitchReport(3);

        cboPeriod.SelectedIndexChanged += (_, _) => ReloadData();
        btnRefresh.Click += (_, _) => ReloadData();
        btnExportExcel.Click += BtnExportExcel_Click;
        btnPrint.Click += BtnPrint_Click;

        if (cboPeriod.Items.Count > 0) cboPeriod.SelectedIndex = 0;

        pnlToolbar.Resize += (_, _) =>
        {
            btnPrint.Location = new Point(pnlToolbar.Width - btnPrint.Width - 4, 9);
            btnExportExcel.Location = new Point(btnPrint.Left - btnExportExcel.Width - 6, 9);
            btnRefresh.Location = new Point(btnExportExcel.Left - btnRefresh.Width - 6, 9);
            cboPeriod.Location = new Point(btnRefresh.Left - cboPeriod.Width - 8, 10);
            lblPeriod.Location = new Point(cboPeriod.Left - lblPeriod.Width - 4, 14);
        };

        pnlSideMenu.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240));
            e.Graphics.DrawLine(pen, pnlSideMenu.Width - 1, 0, pnlSideMenu.Width - 1, pnlSideMenu.Height);
        };

        pnlFooter.Paint += (s, e) =>
        {
            using var pen = new Pen(Color.FromArgb(226, 232, 240));
            e.Graphics.DrawLine(pen, 0, 0, pnlFooter.Width, 0);
        };

        ReloadData();
        SwitchReport(0);
    }

    public void SwitchReport(int reportIndex)
    {
        _currentReportIndex = reportIndex;

        var buttons = new[] { btnMenuRevenue, btnMenuQuality, btnMenuFinancial, btnMenuClasses };
        for (int i = 0; i < buttons.Length; i++)
        {
            var btn = buttons[i];
            bool isSelected = (i == reportIndex);
            btn.BackColor = isSelected ? Theme.Primary : Color.White;
            btn.ForeColor = isSelected ? Color.White : Theme.TextDark;
            btn.Font = isSelected ? new Font("Segoe UI", 9.5f, FontStyle.Bold) : new Font("Segoe UI", 9.5f, FontStyle.Regular);
            btn.FlatAppearance.BorderColor = isSelected ? Theme.PrimaryDark : Color.FromArgb(226, 232, 240);
        }

        dgvRevenue.Visible = (reportIndex == 0);
        dgvQuality.Visible = (reportIndex == 1);
        dgvFinancial.Visible = (reportIndex == 2);
        dgvClasses.Visible = (reportIndex == 3);

        switch (reportIndex)
        {
            case 0:
                lblCurrentReportTitle.Text = "Doanh thu đào tạo";
                dgvRevenue.BringToFront();
                break;
            case 1:
                lblCurrentReportTitle.Text = "Chất lượng đào tạo & Đầu ra";
                dgvQuality.BringToFront();
                break;
            case 2:
                lblCurrentReportTitle.Text = "Tổng quan tài chính";
                dgvFinancial.BringToFront();
                break;
            case 3:
                lblCurrentReportTitle.Text = "Thống kê sĩ số lớp học";
                dgvClasses.BringToFront();
                break;
        }
    }

    public void ReloadData()
    {
        InitStats();
        LoadRevenueTable();
        LoadQualityTable();
        LoadFinancialTable();
        LoadClassesStatsTable();
        lblSummary.Text = $"Dữ liệu báo cáo tự động tổng hợp từ toàn hệ thống đào tạo HUIT LingoHub — Cập nhật lúc {DateTime.Now:HH:mm dd/MM/yyyy}";
    }

    private void InitStats()
    {
        var rev = MockDataStore.GetRevenueReports();
        var totalRev = rev.Sum(r => r.TongHocPhi);
        var totalCollected = rev.Sum(r => r.DaThu);
        var totalStudents = rev.Sum(r => r.SoHocVien);
        var totalClasses = rev.Sum(r => r.SoLop);
        var rate = totalRev > 0 ? Math.Round(totalCollected / totalRev * 100, 1) : 0;

        tblStats.Controls.Clear();
        tblStats.Controls.Add(CreateStatCard("Tổng doanh thu học phí", $"{totalCollected:N0} ₫", $"Chỉ tiêu toàn khóa: {totalRev:N0} ₫", Theme.Primary), 0, 0);
        tblStats.Controls.Add(CreateStatCard("Tỷ lệ thu học phí", $"{rate}%", "Đạt tiến độ tài chính kỳ học", Theme.Success), 1, 0);
        tblStats.Controls.Add(CreateStatCard("Học viên đang học", $"{totalStudents} Học viên", $"{totalClasses} lớp học đang mở", Theme.Purple), 2, 0);
        tblStats.Controls.Add(CreateStatCard("Tỷ lệ tốt nghiệp / Đạt", "94.6%", "Cam kết chuẩn đầu ra quốc tế", Theme.Warning), 3, 0);
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

    private void LoadRevenueTable()
    {
        dgvRevenue.Rows.Clear();
        var list = MockDataStore.GetRevenueReports();

        decimal sumTong = 0;
        decimal sumThu = 0;
        decimal sumNo = 0;
        int sumLop = 0;
        int sumHV = 0;

        foreach (var r in list)
        {
            sumTong += r.TongHocPhi;
            sumThu += r.DaThu;
            sumNo += r.ConNo;
            sumLop += r.SoLop;
            sumHV += r.SoHocVien;

            var rowIdx = dgvRevenue.Rows.Add(
                r.ChuongTrinh,
                r.SoLop,
                r.SoHocVien,
                $"{r.TongHocPhi:N0} ₫",
                $"{r.DaThu:N0} ₫",
                $"{r.ConNo:N0} ₫",
                $"{r.TyLeThu}%"
            );

            if (r.ConNo > 0) dgvRevenue.Rows[rowIdx].Cells["ConNo"].Style.ForeColor = Theme.Danger;
            dgvRevenue.Rows[rowIdx].Cells["TyLeThu"].Style.ForeColor = Theme.Success;
            dgvRevenue.Rows[rowIdx].Cells["TyLeThu"].Style.Font = Theme.FontBold;
        }

        // Summary row
        var totalRate = sumTong > 0 ? Math.Round(sumThu / sumTong * 100, 1) : 0;
        var totalRowIdx = dgvRevenue.Rows.Add(
            "TỔNG CỘNG",
            sumLop,
            sumHV,
            $"{sumTong:N0} ₫",
            $"{sumThu:N0} ₫",
            $"{sumNo:N0} ₫",
            $"{totalRate}%"
        );
        dgvRevenue.Rows[totalRowIdx].DefaultCellStyle.Font = Theme.FontBold;
        dgvRevenue.Rows[totalRowIdx].DefaultCellStyle.BackColor = Theme.ContentBackground;
    }

    private void LoadQualityTable()
    {
        dgvQuality.Rows.Clear();

        var rows = new[]
        {
            new { Khoa = "IELTS Intensive 6.5 Cấp tốc", SiSo = 18, XuatSac = 4, Gioi = 8, Kha = 5, Dat = 1, TyLe = "100%", DanhGia = "Đạt chỉ tiêu cam kết đầu ra" },
            new { Khoa = "IELTS Master 7.5+ Chuyên sâu", SiSo = 14, XuatSac = 6, Gioi = 6, Kha = 2, Dat = 0, TyLe = "100%", DanhGia = "Chất lượng vượt trội" },
            new { Khoa = "Tiếng Anh Giao tiếp phản xạ PRO", SiSo = 15, XuatSac = 3, Gioi = 7, Kha = 4, Dat = 1, TyLe = "100%", DanhGia = "Kỹ năng phản xạ tiến bộ rõ rệt" },
            new { Khoa = "Luyện thi TOEIC Mục tiêu 650+", SiSo = 16, XuatSac = 2, Gioi = 6, Kha = 6, Dat = 2, TyLe = "100%", DanhGia = "Đạt mục tiêu điểm thi TOEIC" },
            new { Khoa = "Tiếng Anh Thiếu nhi Starters", SiSo = 12, XuatSac = 5, Gioi = 5, Kha = 2, Dat = 0, TyLe = "100%", DanhGia = "Đạt 12-15 khiên Cambridge" }
        };

        foreach (var r in rows)
        {
            var rowIdx = dgvQuality.Rows.Add(
                r.Khoa,
                $"{r.SiSo} HV",
                $"{r.XuatSac} HV",
                $"{r.Gioi} HV",
                $"{r.Kha} HV",
                $"{r.Dat} HV",
                r.TyLe,
                r.DanhGia
            );

            dgvQuality.Rows[rowIdx].Cells["TyLeDat"].Style.Font = Theme.FontBold;
            dgvQuality.Rows[rowIdx].Cells["TyLeDat"].Style.ForeColor = Theme.Success;
            dgvQuality.Rows[rowIdx].Cells["DanhGia"].Style.ForeColor = Theme.PrimaryDark;
        }
    }

    private void LoadFinancialTable()
    {
        dgvFinancial.Rows.Clear();

        var rows = new[]
        {
            new { Muc = "Doanh thu học phí đào tạo toàn trung tâm", KyTruoc = "1.250.000.000 ₫", KyNay = "1.580.000.000 ₫", TangTruong = "+26.4%", KeHoach = "1.500.000.000 ₫", HoanThanh = "105.3%", GhiChu = "Vượt chỉ tiêu kế hoạch quý" },
            new { Muc = "Chi phí thù lao giảng viên & trợ giảng", KyTruoc = "380.000.000 ₫", KyNay = "440.000.000 ₫", TangTruong = "+15.8%", KeHoach = "450.000.000 ₫", HoanThanh = "97.8%", GhiChu = "Đảm bảo đúng định mức tiết dạy" },
            new { Muc = "Chi phí vận hành phòng lab & cơ sở vật chất", KyTruoc = "110.000.000 ₫", KyNay = "125.000.000 ₫", TangTruong = "+13.6%", KeHoach = "130.000.000 ₫", HoanThanh = "96.2%", GhiChu = "Tiết kiệm 3.8% ngân sách được duyệt" },
            new { Muc = "Chi phí bản quyền giáo trình & đề thi thử chuẩn", KyTruoc = "65.000.000 ₫", KyNay = "80.000.000 ₫", TangTruong = "+23.1%", KeHoach = "85.000.000 ₫", HoanThanh = "94.1%", GhiChu = "Đã cập nhật bộ tài liệu bản quyền mới" },
            new { Muc = "Lợi nhuận gộp từ hoạt động đào tạo", KyTruoc = "695.000.000 ₫", KyNay = "935.000.000 ₫", TangTruong = "+34.5%", KeHoach = "835.000.000 ₫", HoanThanh = "112.0%", GhiChu = "Tăng trưởng tài chính lành mạnh và ổn định" }
        };

        foreach (var r in rows)
        {
            var rowIdx = dgvFinancial.Rows.Add(
                r.Muc,
                r.KyTruoc,
                r.KyNay,
                r.TangTruong,
                r.KeHoach,
                r.HoanThanh,
                r.GhiChu
            );

            dgvFinancial.Rows[rowIdx].Cells["TangTruong"].Style.ForeColor = r.TangTruong.StartsWith('+') ? Theme.Success : Theme.Danger;
            dgvFinancial.Rows[rowIdx].Cells["TangTruong"].Style.Font = Theme.FontBold;
            dgvFinancial.Rows[rowIdx].Cells["TyLeHoanThanh"].Style.Font = Theme.FontBold;
            dgvFinancial.Rows[rowIdx].Cells["TyLeHoanThanh"].Style.ForeColor = Theme.PrimaryDark;
        }
    }

    private void LoadClassesStatsTable()
    {
        dgvClasses.Rows.Clear();

        var rows = new[]
        {
            new { ChuongTrinh = "IELTS Chuyên sâu (Foundation - Master)", SoLop = "8 lớp", SiSoBQ = "16.5 HV/lớp", LopDay = "6 lớp", ChoTrong = "12 chỗ", TyLe = "91.7%", TrangThai = "Sắp hết chỗ — Đang mở thêm ca tối" },
            new { ChuongTrinh = "Tiếng Anh Giao tiếp quốc tế (PRO)", SoLop = "6 lớp", SiSoBQ = "17.2 HV/lớp", LopDay = "5 lớp", ChoTrong = "8 chỗ", TyLe = "93.3%", TrangThai = "Đạt hiệu suất tối ưu phòng học" },
            new { ChuongTrinh = "Luyện thi TOEIC Chuẩn đầu ra Đại học", SoLop = "5 lớp", SiSoBQ = "18.0 HV/lớp", LopDay = "4 lớp", ChoTrong = "10 chỗ", TyLe = "90.0%", TrangThai = "Tuyển sinh ổn định theo định mức" },
            new { ChuongTrinh = "Tiếng Anh Thiếu nhi & Thiếu niên (Cambridge)", SoLop = "4 lớp", SiSoBQ = "14.0 HV/lớp", LopDay = "2 lớp", ChoTrong = "14 chỗ", TyLe = "80.0%", TrangThai = "Đang tiếp nhận hồ sơ đăng ký mới" },
            new { ChuongTrinh = "Tiếng Anh Thương mại Doanh nghiệp", SoLop = "3 lớp", SiSoBQ = "15.0 HV/lớp", LopDay = "2 lớp", ChoTrong = "6 chỗ", TyLe = "88.2%", TrangThai = "Theo hợp đồng đào tạo chuyên đề" }
        };

        foreach (var r in rows)
        {
            var rowIdx = dgvClasses.Rows.Add(
                r.ChuongTrinh,
                r.SoLop,
                r.SiSoBQ,
                r.LopDay,
                r.ChoTrong,
                r.TyLe,
                r.TrangThai
            );

            dgvClasses.Rows[rowIdx].Cells["TyLeLapDay"].Style.Font = Theme.FontBold;
            dgvClasses.Rows[rowIdx].Cells["TyLeLapDay"].Style.ForeColor = Theme.PrimaryDark;
            dgvClasses.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = Theme.TextDark;
        }
    }

    private void BtnExportExcel_Click(object? sender, EventArgs e)
    {
        string reportName = _currentReportIndex switch
        {
            0 => "DoanhThuHocPhi",
            1 => "ChatLuongDauRa",
            2 => "TongQuanTaiChinh",
            3 => "ThongKeSiSoLop",
            _ => "TongHop"
        };

        MessageBox.Show(
            FindForm(),
            $"Báo cáo đã được xuất thành công.\nFile: HUIT_LingoHub_BaoCao_{reportName}_{DateTime.Now:yyyyMMdd_HHmm}.xlsx",
            "Xuất file Excel thành công",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void BtnPrint_Click(object? sender, EventArgs e)
    {
        string reportTitle = _currentReportIndex switch
        {
            0 => "Báo cáo Doanh thu & Học phí",
            1 => "Báo cáo Chất lượng đào tạo & Đầu ra",
            2 => "Báo cáo Tổng quan Tài chính & Ngân sách",
            3 => "Thống kê Sĩ số & Công suất lớp học",
            _ => "Báo cáo Thống kê"
        };

        MessageBox.Show(
            FindForm(),
            $"Đã gửi lệnh in cho: {reportTitle}\nĐến máy in mạng văn phòng trung tâm HUIT LingoHub.",
            "In báo cáo",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }
}
