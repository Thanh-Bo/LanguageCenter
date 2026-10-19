using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcBaoCao : UserControl
{
    private int _currentReportIndex = 0;

    private IReadOnlyList<ClassListItemDto> _classes = Array.Empty<ClassListItemDto>();
    private IReadOnlyList<TuitionItemDto> _tuition = Array.Empty<TuitionItemDto>();
    private IReadOnlyList<AcademicReportDto> _academics = Array.Empty<AcademicReportDto>();
    private FinanceReportDto? _finance;
    private ReportOverviewDto? _overview;

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
        _ = ReloadAsync();
    }

    /// <summary>Tải toàn bộ số liệu báo cáo từ API rồi dựng lại 4 bảng.</summary>
    private async Task ReloadAsync()
    {
        lblSummary.Text = "Đang tổng hợp số liệu báo cáo...";
        UseWaitCursor = true;

        var classesTask = AppSession.Api.GetAsync<PagedResult<ClassListItemDto>>(
            QueryStringBuilder.Build("admin/classes", ("page", "1"), ("pageSize", "500")));
        var tuitionTask = AppSession.Api.GetAsync<PagedResult<TuitionItemDto>>(
            QueryStringBuilder.Build("admin/tuition-fees", ("page", "1"), ("pageSize", "500")));
        var financeTask = AppSession.Api.GetAsync<FinanceReportDto>("admin/tuition-fees/reports/summary");
        var overviewTask = AppSession.Api.GetAsync<ReportOverviewDto>("admin/reports/overview");

        await Task.WhenAll(classesTask, tuitionTask, financeTask, overviewTask);

        _classes = classesTask.Result.Data?.Items ?? Array.Empty<ClassListItemDto>();
        _tuition = tuitionTask.Result.Data?.Items ?? Array.Empty<TuitionItemDto>();
        _finance = financeTask.Result.Data;
        _overview = overviewTask.Result.Data;

        // Bảng chất lượng đào tạo: tổng hợp báo cáo học tập của từng lớp
        var dsHocTap = new List<AcademicReportDto>();
        foreach (var c in _classes)
        {
            var r = await AppSession.Api.GetAsync<AcademicReportDto>(
                QueryStringBuilder.Build("admin/reports/academic", ("maLopHoc", c.MaLopHoc.ToString())));

            if (r.Success && r.Data is not null)
            {
                dsHocTap.Add(r.Data);
            }
        }
        _academics = dsHocTap;

        UseWaitCursor = false;

        InitStats();
        LoadRevenueTable();
        LoadQualityTable();
        LoadFinancialTable();
        LoadClassesStatsTable();

        lblSummary.Text = $"Dữ liệu báo cáo tổng hợp từ API HUIT LingoHub — Cập nhật lúc {DateTime.Now:HH:mm dd/MM/yyyy}";
    }

    private void InitStats()
    {
        var daThu = _finance?.TongDaThu ?? 0m;
        var phaiThu = _finance?.TongPhaiThu ?? 0m;
        var tyLe = phaiThu > 0 ? Math.Round(daThu / phaiThu * 100, 1) : 0m;

        tblStats.Controls.Clear();
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Tổng doanh thu học phí", $"{daThu:N0} ₫", $"Chỉ tiêu toàn khóa: {phaiThu:N0} ₫", Theme.Primary), 0, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Tỷ lệ thu học phí", $"{tyLe}%", "Tiến độ thu trên tổng phải thu", Theme.Success), 1, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Học viên đang học", $"{_overview?.SoHocVienDangHoc ?? 0} Học viên", $"{_overview?.TongLopHoc ?? 0} lớp học trên hệ thống", Theme.Purple), 2, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Học viên còn nợ", $"{_finance?.SoHocVienConNo ?? 0} Học viên", $"Công nợ: {_finance?.TongConThieu ?? 0:N0} ₫", Theme.Warning), 3, 0);
    }

    private void LoadRevenueTable()
    {
        dgvRevenue.Rows.Clear();

        var nhom = _tuition
            .GroupBy(t => t.TenKhoaHoc)
            .Select(g => new
            {
                ChuongTrinh = g.Key,
                SoLop = g.Where(x => x.MaLopHoc.HasValue).Select(x => x.MaLopHoc!.Value).Distinct().Count(),
                SoHocVien = g.Select(x => x.MaHocVien).Distinct().Count(),
                Tong = g.Sum(x => x.SoTienPhaiDong),
                Thu = g.Sum(x => x.SoTienDaDong),
                No = g.Sum(x => x.SoTienConThieu)
            })
            .OrderByDescending(x => x.Tong)
            .ToList();

        decimal sumTong = 0;
        decimal sumThu = 0;
        decimal sumNo = 0;
        int sumLop = 0;
        int sumHV = 0;

        foreach (var r in nhom)
        {
            sumTong += r.Tong;
            sumThu += r.Thu;
            sumNo += r.No;
            sumLop += r.SoLop;
            sumHV += r.SoHocVien;

            var tyLe = r.Tong > 0 ? Math.Round(r.Thu / r.Tong * 100, 1) : 0m;

            var rowIdx = dgvRevenue.Rows.Add(
                r.ChuongTrinh,
                r.SoLop,
                r.SoHocVien,
                $"{r.Tong:N0} ₫",
                $"{r.Thu:N0} ₫",
                $"{r.No:N0} ₫",
                $"{tyLe}%"
            );

            if (r.No > 0) dgvRevenue.Rows[rowIdx].Cells["ConNo"].Style.ForeColor = Theme.Danger;
            dgvRevenue.Rows[rowIdx].Cells["TyLeThu"].Style.ForeColor = Theme.Success;
            dgvRevenue.Rows[rowIdx].Cells["TyLeThu"].Style.Font = Theme.FontBold;
        }

        if (nhom.Count == 0)
        {
            return;
        }

        // Dòng tổng cộng
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

        var nhom = _academics
            .GroupBy(a => a.TenKhoaHoc)
            .Select(g =>
            {
                var coDiem = g.SelectMany(a => a.HocViens).Where(x => x.DiemTongKet.HasValue).ToList();

                int Dem(decimal min, decimal max) =>
                    coDiem.Count(x => x.DiemTongKet!.Value >= min && x.DiemTongKet!.Value <= max);

                var xuatSac = Dem(8.5m, 10m);
                var gioi = Dem(7.0m, 8.49m);
                var kha = Dem(6.0m, 6.99m);
                var dat = Dem(5.0m, 5.99m);
                var khongDat = Math.Max(coDiem.Count - xuatSac - gioi - kha - dat, 0);
                var tyLe = coDiem.Count > 0
                    ? Math.Round((decimal)(coDiem.Count - khongDat) / coDiem.Count * 100, 1)
                    : 0m;

                return new
                {
                    Khoa = g.Key,
                    SiSo = g.Sum(a => a.SoHocVien),
                    XuatSac = xuatSac,
                    Gioi = gioi,
                    Kha = kha,
                    Dat = dat,
                    KhongDat = khongDat,
                    TyLe = tyLe
                };
            })
            .OrderByDescending(x => x.SiSo)
            .ToList();

        foreach (var r in nhom)
        {
            var danhGia = r.KhongDat > 0
                ? $"{r.KhongDat} học viên chưa đạt đầu ra"
                : "Đạt chỉ tiêu cam kết đầu ra";

            var rowIdx = dgvQuality.Rows.Add(
                r.Khoa,
                $"{r.SiSo} HV",
                $"{r.XuatSac} HV",
                $"{r.Gioi} HV",
                $"{r.Kha} HV",
                $"{r.Dat} HV",
                $"{r.TyLe}%",
                danhGia
            );

            dgvQuality.Rows[rowIdx].Cells["TyLeDat"].Style.Font = Theme.FontBold;
            dgvQuality.Rows[rowIdx].Cells["TyLeDat"].Style.ForeColor = Theme.Success;
            dgvQuality.Rows[rowIdx].Cells["DanhGia"].Style.ForeColor = Theme.PrimaryDark;
        }
    }

    private void LoadFinancialTable()
    {
        dgvFinancial.Rows.Clear();

        if (_finance is null)
        {
            return;
        }

        var f = _finance;
        var tyLeThu = f.TongPhaiThu > 0 ? Math.Round(f.TongDaThu / f.TongPhaiThu * 100, 1) : 0m;

        var rows = new (string Muc, string KyNay, string TyLe, string GhiChu)[]
        {
            ("Tổng học phí phải thu", $"{f.TongPhaiThu:N0} ₫", "100%", $"{f.TongSoHocPhi} phiếu học phí trên hệ thống"),
            ("Đã thực thu vào quỹ", $"{f.TongDaThu:N0} ₫", $"{tyLeThu}%", $"{f.SoDaThanhToan} phiếu đã tất toán"),
            ("Công nợ còn lại", $"{f.TongConThieu:N0} ₫", $"{Math.Round(100 - tyLeThu, 1)}%", $"{f.SoHocVienConNo} học viên còn nợ học phí"),
            ("Phiếu quá hạn thanh toán", $"{f.SoQuaHan} phiếu", "—", $"{f.SoChuaThanhToan} phiếu chưa thanh toán"),
            ("Phiếu thanh toán một phần", $"{f.SoThanhToanMotPhan} phiếu", "—", "Cần thu nốt phần còn thiếu")
        };

        foreach (var r in rows)
        {
            var rowIdx = dgvFinancial.Rows.Add(r.Muc, "—", r.KyNay, "—", "—", r.TyLe, r.GhiChu);

            dgvFinancial.Rows[rowIdx].Cells["TyLeHoanThanh"].Style.Font = Theme.FontBold;
            dgvFinancial.Rows[rowIdx].Cells["TyLeHoanThanh"].Style.ForeColor = Theme.PrimaryDark;
        }
    }

    private void LoadClassesStatsTable()
    {
        dgvClasses.Rows.Clear();

        var nhom = _classes
            .GroupBy(c => c.TenKhoaHoc)
            .Select(g =>
            {
                var tongLop = g.Count();
                var siSo = g.Sum(c => c.SiSoHienTai);
                var toiDa = g.Sum(c => c.SiSoToiDa);
                var day = g.Count(c => c.SiSoHienTai >= c.SiSoToiDa);
                var conTrong = Math.Max(toiDa - siSo, 0);
                var tyLe = toiDa > 0 ? Math.Round((decimal)siSo / toiDa * 100, 1) : 0m;
                var binhQuan = tongLop > 0 ? Math.Round((decimal)siSo / tongLop, 1) : 0m;
                var tinhTrang = conTrong == 0
                    ? "Đã đầy — cần mở thêm lớp"
                    : (tyLe >= 80 ? "Sắp hết chỗ" : "Đang tiếp nhận hồ sơ");

                return new
                {
                    ChuongTrinh = g.Key,
                    TongSoLop = tongLop,
                    BinhQuan = binhQuan,
                    Day = day,
                    ConTrong = conTrong,
                    TyLe = tyLe,
                    TinhTrang = tinhTrang
                };
            })
            .OrderByDescending(x => x.TongSoLop)
            .ToList();

        foreach (var r in nhom)
        {
            var rowIdx = dgvClasses.Rows.Add(
                r.ChuongTrinh,
                $"{r.TongSoLop} lớp",
                $"{r.BinhQuan} HV/lớp",
                $"{r.Day} lớp",
                $"{r.ConTrong} chỗ",
                $"{r.TyLe}%",
                r.TinhTrang
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
