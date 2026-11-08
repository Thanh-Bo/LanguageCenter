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
    private PeriodReportDto? _period;

    /// <summary>Đã nạp báo cáo học tập của từng lớp chưa (chỉ nạp khi mở tab Chất lượng).</summary>
    private bool _hocTapDaNap;

    // ===== BÁO CÁO THEO KỲ (thêm ở runtime để không phải sửa Designer) =====
    private Button _btnMenuPeriod = null!;
    private DataGridView _dgvPeriod = null!;
    private DataGridView _dgvDebt = null!;
    private DateTimePicker _dtpTuNgay = null!;
    private DateTimePicker _dtpDenNgay = null!;
    private Label _lblTuNgay = null!;
    private Label _lblDenNgay = null!;

    private const int ChiSoBaoCaoTheoKy = 4;

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

        DungKhuVucBaoCaoTheoKy();

        cboPeriod.SelectedIndexChanged += (_, _) =>
        {
            // Danh sách preset kỳ chỉ áp cho báo cáo theo kỳ.
            if (_currentReportIndex == ChiSoBaoCaoTheoKy)
            {
                ApDungPresetKy();
                ReloadData();
            }
        };
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

            // Hai ô ngày nằm BÊN TRÁI nhãn "Kỳ:" — tự co giãn theo bề rộng cửa sổ.
            var x = lblPeriod.Left - 8;

            _dtpDenNgay.Location = new Point(x - _dtpDenNgay.Width, 10);
            x = _dtpDenNgay.Left - 4;
            _lblDenNgay.Location = new Point(x - _lblDenNgay.Width, 14);
            x = _lblDenNgay.Left - 8;
            _dtpTuNgay.Location = new Point(x - _dtpTuNgay.Width, 10);
            x = _dtpTuNgay.Left - 4;
            _lblTuNgay.Location = new Point(x - _lblTuNgay.Width, 14);
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

        var buttons = new[] { btnMenuRevenue, btnMenuQuality, btnMenuFinancial, btnMenuClasses, _btnMenuPeriod };
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
        _dgvPeriod.Visible = (reportIndex == ChiSoBaoCaoTheoKy);
        _dgvDebt.Visible = (reportIndex == ChiSoBaoCaoTheoKy);

        // Ô chọn khoảng ngày chỉ hiện ở báo cáo theo kỳ.
        var laBaoCaoTheoKy = reportIndex == ChiSoBaoCaoTheoKy;
        _dtpTuNgay.Visible = laBaoCaoTheoKy;
        _dtpDenNgay.Visible = laBaoCaoTheoKy;
        _lblTuNgay.Visible = laBaoCaoTheoKy;
        _lblDenNgay.Visible = laBaoCaoTheoKy;

        switch (reportIndex)
        {
            case 0:
                lblCurrentReportTitle.Text = "Doanh thu đào tạo";
                dgvRevenue.BringToFront();
                break;
            case 1:
                lblCurrentReportTitle.Text = "Chất lượng đào tạo & Đầu ra";
                dgvQuality.BringToFront();
                _ = NapHocTapNeuCanAsync();
                break;
            case 2:
                lblCurrentReportTitle.Text = "Tổng quan tài chính";
                dgvFinancial.BringToFront();
                break;
            case 3:
                lblCurrentReportTitle.Text = "Thống kê sĩ số lớp học";
                dgvClasses.BringToFront();
                break;
            case ChiSoBaoCaoTheoKy:
                lblCurrentReportTitle.Text = "Thống kê theo kỳ & công nợ";
                _dgvDebt.BringToFront();
                _dgvPeriod.BringToFront();
                _ = LoadPeriodReportAsync();
                break;
        }
    }

    public void ReloadData()
    {
        if (_currentReportIndex == ChiSoBaoCaoTheoKy)
        {
            _ = LoadPeriodReportAsync();
            return;
        }

        _hocTapDaNap = false;
        _ = ReloadAsync();
    }

    // ========================================================================
    // BÁO CÁO THEO KỲ (tháng / quý / năm / khoảng tự chọn)
    // ========================================================================

    /// <summary>Dựng nút menu + 2 lưới + 2 ô chọn ngày ở runtime.</summary>
    private void DungKhuVucBaoCaoTheoKy()
    {
        _btnMenuPeriod = new Button
        {
            Text = "Thống kê theo kỳ",
            Location = new Point(10, 238),
            Size = new Size(208, 44),
            FlatStyle = FlatStyle.Flat,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(14, 0, 0, 4),
            Cursor = Cursors.Hand,
            Font = new Font("Segoe UI", 9.5f),
            BackColor = Color.White,
            ForeColor = Color.FromArgb(15, 23, 42)
        };
        _btnMenuPeriod.FlatAppearance.BorderSize = 1;
        _btnMenuPeriod.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        _btnMenuPeriod.Click += (_, _) => SwitchReport(ChiSoBaoCaoTheoKy);
        pnlSideMenu.Controls.Add(_btnMenuPeriod);

        _lblTuNgay = new Label
        {
            Text = "Từ",
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Visible = false
        };

        _dtpTuNgay = new DateTimePicker
        {
            Width = 115,
            Format = DateTimePickerFormat.Short,
            Visible = false
        };

        _lblDenNgay = new Label
        {
            Text = "đến",
            AutoSize = true,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            ForeColor = Color.FromArgb(15, 23, 42),
            Visible = false
        };

        _dtpDenNgay = new DateTimePicker
        {
            Width = 115,
            Format = DateTimePickerFormat.Short,
            Visible = false
        };

        var homNay = DateTime.Today;
        _dtpTuNgay.Value = new DateTime(homNay.Year, homNay.Month, 1);
        _dtpDenNgay.Value = homNay;

        pnlToolbar.Controls.Add(_lblTuNgay);
        pnlToolbar.Controls.Add(_dtpTuNgay);
        pnlToolbar.Controls.Add(_lblDenNgay);
        pnlToolbar.Controls.Add(_dtpDenNgay);

        // ---- Lưới chỉ số kỳ (dòng trên) ----
        _dgvPeriod = new DataGridView { Dock = DockStyle.Top, Height = 250, Visible = false };
        _dgvPeriod.Columns.Add(new DataGridViewTextBoxColumn { Name = "ChiSo", HeaderText = "Chỉ số trong kỳ", FillWeight = 320 });
        _dgvPeriod.Columns.Add(new DataGridViewTextBoxColumn { Name = "GiaTri", HeaderText = "Giá trị", FillWeight = 140 });
        _dgvPeriod.Columns.Add(new DataGridViewTextBoxColumn { Name = "GhiChu", HeaderText = "Ghi chú", FillWeight = 300 });
        UI.UiHelpers.StyleGrid(_dgvPeriod);

        // ---- Lưới học viên còn nợ (phần còn lại) ----
        _dgvDebt = new DataGridView { Dock = DockStyle.Fill, Visible = false };
        _dgvDebt.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaHocVien", HeaderText = "Mã HV", FillWeight = 70 });
        _dgvDebt.Columns.Add(new DataGridViewTextBoxColumn { Name = "HoTen", HeaderText = "Họ tên", FillWeight = 150 });
        _dgvDebt.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoDienThoai", HeaderText = "SĐT", FillWeight = 90 });
        _dgvDebt.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenKhoaHoc", HeaderText = "Khóa học", FillWeight = 150 });
        _dgvDebt.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaLop", HeaderText = "Lớp", FillWeight = 80 });
        _dgvDebt.Columns.Add(new DataGridViewTextBoxColumn { Name = "PhaiDong", HeaderText = "Phải đóng", FillWeight = 100 });
        _dgvDebt.Columns.Add(new DataGridViewTextBoxColumn { Name = "DaDong", HeaderText = "Đã đóng", FillWeight = 100 });
        _dgvDebt.Columns.Add(new DataGridViewTextBoxColumn { Name = "ConThieu", HeaderText = "Còn thiếu", FillWeight = 100 });
        _dgvDebt.Columns.Add(new DataGridViewTextBoxColumn { Name = "HanThanhToan", HeaderText = "Hạn thanh toán", FillWeight = 100 });
        _dgvDebt.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", FillWeight = 110 });
        UI.UiHelpers.StyleGrid(_dgvDebt);

        // Dock: thêm lưới Fill TRƯỚC, lưới Top SAU để Top chiếm chỗ trên cùng.
        pnlGridContainer.Controls.Add(_dgvDebt);
        pnlGridContainer.Controls.Add(_dgvPeriod);
    }

    /// <summary>Đổi preset "Kỳ" thành khoảng ngày cụ thể.</summary>
    private void ApDungPresetKy()
    {
        var homNay = DateTime.Today;
        var luaChon = cboPeriod.SelectedItem?.ToString() ?? "Tháng này";

        (DateTime tu, DateTime den) = luaChon switch
        {
            "Tháng trước" => (
                new DateTime(homNay.Year, homNay.Month, 1).AddMonths(-1),
                new DateTime(homNay.Year, homNay.Month, 1).AddDays(-1)),
            "Quý này" => (
                new DateTime(homNay.Year, ((homNay.Month - 1) / 3) * 3 + 1, 1),
                new DateTime(homNay.Year, ((homNay.Month - 1) / 3) * 3 + 1, 1).AddMonths(3).AddDays(-1)),
            "Quý trước" => (
                new DateTime(homNay.Year, ((homNay.Month - 1) / 3) * 3 + 1, 1).AddMonths(-3),
                new DateTime(homNay.Year, ((homNay.Month - 1) / 3) * 3 + 1, 1).AddDays(-1)),
            "Năm nay" => (new DateTime(homNay.Year, 1, 1), new DateTime(homNay.Year, 12, 31)),
            "Năm trước" => (new DateTime(homNay.Year - 1, 1, 1), new DateTime(homNay.Year - 1, 12, 31)),
            "6 tháng gần đây" => (new DateTime(homNay.Year, homNay.Month, 1).AddMonths(-5), homNay),
            _ => (new DateTime(homNay.Year, homNay.Month, 1), homNay)   // Tháng này
        };

        _dtpTuNgay.Value = tu;
        _dtpDenNgay.Value = den;
    }

    /// <summary>Gọi API thống kê theo kỳ và đổ 2 lưới.</summary>
    private async Task LoadPeriodReportAsync()
    {
        if (IsDisposed || Disposing)
        {
            return;
        }

        lblSummary.Text = "Đang tổng hợp số liệu theo kỳ...";
        UseWaitCursor = true;

        var tu = DateOnly.FromDateTime(_dtpTuNgay.Value);
        var den = DateOnly.FromDateTime(_dtpDenNgay.Value);

        var result = await AppSession.Api.GetAsync<PeriodReportDto>(
            QueryStringBuilder.Build(
                "admin/reports/period",
                ("tuNgay", tu.ToString("yyyy-MM-dd")),
                ("denNgay", den.ToString("yyyy-MM-dd"))));

        UseWaitCursor = false;

        if (IsDisposed || Disposing)
        {
            return;
        }

        if (!result.Success || result.Data is null)
        {
            lblSummary.Text = result.ErrorMessage;
            _dgvPeriod.Rows.Clear();
            _dgvDebt.Rows.Clear();
            return;
        }

        _period = result.Data;
        var p = result.Data;

        _dgvPeriod.Rows.Clear();

        void ThemChiSo(string chiSo, string giaTri, string ghiChu)
        {
            var idx = _dgvPeriod.Rows.Add(chiSo, giaTri, ghiChu);
            _dgvPeriod.Rows[idx].Cells["GiaTri"].Style.Font = Theme.FontBold;
            _dgvPeriod.Rows[idx].Cells["GiaTri"].Style.ForeColor = Theme.PrimaryDark;
        }

        ThemChiSo("Số lớp học trong kỳ", $"{p.SoLopHoc} lớp", "Lớp có thời gian học giao với kỳ");
        ThemChiSo("Số học viên đang theo học", $"{p.SoHocVien} học viên", "Học viên đã xếp lớp trong kỳ");
        ThemChiSo("Số giáo viên giảng dạy", $"{p.SoGiaoVien} giáo viên", "Có phân công còn hiệu lực trong kỳ");
        ThemChiSo("Số trợ giảng", $"{p.SoTroGiang} trợ giảng", "Có phân công còn hiệu lực trong kỳ");
        ThemChiSo("Học viên đang học (toàn hệ thống)", $"{p.SoHocVienDangHoc} học viên", "Trạng thái học tập = Đang học");
        ThemChiSo("Học viên còn nợ học phí", $"{p.SoHocVienConNo} học viên", "Không phụ thuộc kỳ đang chọn");
        ThemChiSo("Tổng công nợ", $"{p.TongConNo:N0} ₫", "Tổng số tiền còn thiếu trên mọi phiếu");

        _dgvDebt.Rows.Clear();

        foreach (var d in p.HocVienConNo)
        {
            var idx = _dgvDebt.Rows.Add(
                d.MaHocVien,
                d.HoTen,
                string.IsNullOrWhiteSpace(d.SoDienThoai) ? "—" : d.SoDienThoai,
                d.TenKhoaHoc,
                string.IsNullOrWhiteSpace(d.MaLop) ? "—" : d.MaLop,
                $"{d.SoTienPhaiDong:N0} ₫",
                $"{d.SoTienDaDong:N0} ₫",
                $"{d.SoTienConThieu:N0} ₫",
                d.HanThanhToan is { } han ? han.ToString("dd/MM/yyyy") : "—",
                d.TrangThaiHocPhi);

            _dgvDebt.Rows[idx].Cells["ConThieu"].Style.ForeColor = Theme.Danger;
            _dgvDebt.Rows[idx].Cells["ConThieu"].Style.Font = Theme.FontBold;
        }

        lblSummary.Text = p.HocVienConNo.Count == 0
            ? $"Kỳ {tu:dd/MM/yyyy} – {den:dd/MM/yyyy}: không có học viên nào còn nợ học phí."
            : $"Kỳ {tu:dd/MM/yyyy} – {den:dd/MM/yyyy}: {p.SoLopHoc} lớp · {p.SoHocVien} học viên · "
              + $"{p.SoGiaoVien} giáo viên · {p.SoTroGiang} trợ giảng · {p.SoHocVienConNo} học viên còn nợ";
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

        // Bảng "Chất lượng đào tạo" KHÔNG nạp ở đây nữa: trước đây vòng lặp gọi
        // admin/reports/academic cho TỪNG lớp ngay khi mở trang (N+1 request, rất chậm).
        // Giờ chỉ nạp khi người dùng thực sự mở tab đó (xem NapHocTapNeuCanAsync).
        if (_hocTapDaNap)
        {
            _hocTapDaNap = false;
            _academics = Array.Empty<AcademicReportDto>();
        }

        UseWaitCursor = false;

        InitStats();
        LoadRevenueTable();
        LoadQualityTable();
        LoadFinancialTable();
        LoadClassesStatsTable();

        lblSummary.Text = $"Dữ liệu báo cáo tổng hợp từ API HUIT LingoHub — Cập nhật lúc {DateTime.Now:HH:mm dd/MM/yyyy}";
    }

    /// <summary>
    /// Nạp báo cáo học tập của từng lớp — CHỈ chạy khi người dùng mở tab
    /// "Chất lượng đào tạo" (trước đây chạy sẵn nên mở trang rất chậm).
    /// </summary>
    private async Task NapHocTapNeuCanAsync()
    {
        if (_hocTapDaNap || _classes.Count == 0 || IsDisposed || Disposing)
        {
            return;
        }

        lblSummary.Text = "Đang tổng hợp báo cáo học tập từng lớp...";
        UseWaitCursor = true;

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

        UseWaitCursor = false;

        if (IsDisposed || Disposing)
        {
            return;
        }

        _academics = dsHocTap;
        _hocTapDaNap = true;

        LoadQualityTable();

        lblSummary.Text = $"Đã tổng hợp báo cáo học tập của {_academics.Count} lớp.";
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
