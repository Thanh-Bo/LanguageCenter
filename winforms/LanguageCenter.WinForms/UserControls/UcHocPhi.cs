using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

public class TuitionFeeItem
{
    public string MaHocPhi { get; set; } = string.Empty;
    public string MaDangKy { get; set; } = string.Empty;
    public string MaHocVien { get; set; } = string.Empty;
    public string HoTenHocVien { get; set; } = string.Empty;
    public string TenKhoaLop { get; set; } = string.Empty;
    public decimal SoTienGoc { get; set; }
    public decimal SoTienGiam { get; set; }
    public decimal SoTienPhaiDong { get; set; }
    public decimal SoTienDaDong { get; set; }
    public decimal SoTienConThieu { get; set; }
    public DateOnly HanThanhToan { get; set; }
    public string TrangThaiHocPhi { get; set; } = string.Empty;
    public string PhuongThuc { get; set; } = string.Empty;
    public string SoBienNhan { get; set; } = string.Empty;
    public DateTime? NgayThanhToan { get; set; }
}

public partial class UcHocPhi : UserControl
{
    private const string AllOption = "— Tất cả —";
    private readonly List<TuitionFeeItem> _items = new();
    private int _currentTab = 0; // 0: Sổ học phí, 1: Lịch sử thanh toán, 2: Quản lý biên nhận

    public UcHocPhi()
    {
        InitializeComponent();

        UI.UiHelpers.StyleGrid(dgvTuition);
        UI.UiHelpers.StyleGrid(dgvPayments);
        UI.UiHelpers.StyleGrid(dgvReceipts);

        btnTabTuition.Click += (_, _) => SwitchTab(0);
        btnTabPayments.Click += (_, _) => SwitchTab(1);
        btnTabReceipts.Click += (_, _) => SwitchTab(2);

        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; FilterData(); } };
        cboTrangThai.SelectedIndexChanged += (_, _) => FilterData();
        cboPhuongThuc.SelectedIndexChanged += (_, _) => FilterData();
        btnSearch.Click += (_, _) => FilterData();
        btnRefresh.Click += (_, _) => ReloadData();
        btnCollectFee.Click += BtnCollectFee_Click;
        btnPrintReceipt.Click += BtnPrintReceipt_Click;
        dgvTuition.CellDoubleClick += DgvTuition_CellDoubleClick;

        pnlToolbar.Resize += (_, _) =>
        {
            btnPrintReceipt.Location = new Point(pnlToolbar.Width - btnPrintReceipt.Width - 4, 3);
            btnCollectFee.Location = new Point(btnPrintReceipt.Left - btnCollectFee.Width - 8, 3);
        };

        InitFilterOptions();
        LoadSampleData();
        SwitchTab(0);
    }

    private void InitFilterOptions()
    {
        cboTrangThai.Items.Clear();
        cboTrangThai.Items.Add(AllOption);
        cboTrangThai.Items.AddRange(["Đã hoàn thành", "Đóng đợt 1", "Chưa thanh toán"]);
        cboTrangThai.SelectedIndex = 0;

        cboPhuongThuc.Items.Clear();
        cboPhuongThuc.Items.Add(AllOption);
        cboPhuongThuc.Items.AddRange(["Chuyển khoản (Vietcombank)", "Tiền mặt tại quầy", "VNPay-QR / Thẻ"]);
        cboPhuongThuc.SelectedIndex = 0;
    }

    private void UpdateStats()
    {
        var totalMustPay = _items.Sum(x => x.SoTienPhaiDong);
        var totalCollected = _items.Sum(x => x.SoTienDaDong);
        var totalDebt = _items.Sum(x => x.SoTienConThieu);
        var rate = totalMustPay > 0 ? (totalCollected / totalMustPay) * 100m : 0m;

        tblStats.Controls.Clear();
        tblStats.Controls.Add(CreateStatCard("Tổng học phí phải thu", $"{totalMustPay:N0} ₫", "Sau chiết khấu & ưu đãi", Theme.Primary), 0, 0);
        tblStats.Controls.Add(CreateStatCard("Đã thực thu vào quỹ", $"{totalCollected:N0} ₫", "Tiền mặt & Chuyển khoản", Theme.Success), 1, 0);
        tblStats.Controls.Add(CreateStatCard("Công nợ cần thu", $"{totalDebt:N0} ₫", "Học viên còn nợ học phí", Theme.Danger), 2, 0);
        tblStats.Controls.Add(CreateStatCard("Tỷ lệ thu học phí", $"{rate:F1}%", "Chỉ tiêu thu kỳ hiện hành", Theme.Purple), 3, 0);
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

    private void LoadSampleData()
    {
        _items.Clear();
        _items.AddRange([
            new TuitionFeeItem
            {
                MaHocPhi = "HP2026-001",
                MaDangKy = "DK001",
                MaHocVien = "HV001",
                HoTenHocVien = "Trần Bảo Ngọc",
                TenKhoaLop = "IELTS Master 7.5+ (Lớp IE-A01)",
                SoTienGoc = 12500000m,
                SoTienGiam = 1250000m,
                SoTienPhaiDong = 11250000m,
                SoTienDaDong = 11250000m,
                SoTienConThieu = 0m,
                HanThanhToan = new DateOnly(2026, 10, 15),
                TrangThaiHocPhi = "Đã hoàn thành",
                PhuongThuc = "Chuyển khoản (Vietcombank)",
                SoBienNhan = "BN-2026-0018",
                NgayThanhToan = DateTime.Now.AddDays(-10)
            },
            new TuitionFeeItem
            {
                MaHocPhi = "HP2026-002",
                MaDangKy = "DK002",
                MaHocVien = "HV002",
                HoTenHocVien = "Lê Hoàng Nam",
                TenKhoaLop = "Giao tiếp phản xạ PRO (Lớp GT-B02)",
                SoTienGoc = 6800000m,
                SoTienGiam = 500000m,
                SoTienPhaiDong = 6300000m,
                SoTienDaDong = 3500000m,
                SoTienConThieu = 2800000m,
                HanThanhToan = new DateOnly(2026, 10, 25),
                TrangThaiHocPhi = "Đóng đợt 1",
                PhuongThuc = "Tiền mặt tại quầy",
                SoBienNhan = "BN-2026-0034",
                NgayThanhToan = DateTime.Now.AddDays(-5)
            },
            new TuitionFeeItem
            {
                MaHocPhi = "HP2026-003",
                MaDangKy = "DK003",
                MaHocVien = "HV003",
                HoTenHocVien = "Vũ Thuỳ Linh",
                TenKhoaLop = "Luyện thi TOEIC 750+ (Lớp TC-C01)",
                SoTienGoc = 5500000m,
                SoTienGiam = 0m,
                SoTienPhaiDong = 5500000m,
                SoTienDaDong = 0m,
                SoTienConThieu = 5500000m,
                HanThanhToan = new DateOnly(2026, 10, 20),
                TrangThaiHocPhi = "Chưa thanh toán",
                PhuongThuc = "— Chưa nộp —",
                SoBienNhan = "—",
                NgayThanhToan = null
            },
            new TuitionFeeItem
            {
                MaHocPhi = "HP2026-004",
                MaDangKy = "DK004",
                MaHocVien = "HV004",
                HoTenHocVien = "Nguyễn Minh Khang",
                TenKhoaLop = "Tiếng Anh Thiếu nhi Movers (Lớp KD-03)",
                SoTienGoc = 7500000m,
                SoTienGiam = 750000m,
                SoTienPhaiDong = 6750000m,
                SoTienDaDong = 6750000m,
                SoTienConThieu = 0m,
                HanThanhToan = new DateOnly(2026, 10, 10),
                TrangThaiHocPhi = "Đã hoàn thành",
                PhuongThuc = "VNPay-QR / Thẻ",
                SoBienNhan = "BN-2026-0052",
                NgayThanhToan = DateTime.Now.AddDays(-8)
            },
            new TuitionFeeItem
            {
                MaHocPhi = "HP2026-005",
                MaDangKy = "DK005",
                MaHocVien = "HV005",
                HoTenHocVien = "Phạm Quỳnh Anh",
                TenKhoaLop = "IELTS Intensive 6.5 (Lớp IE-B01)",
                SoTienGoc = 9800000m,
                SoTienGiam = 800000m,
                SoTienPhaiDong = 9000000m,
                SoTienDaDong = 4500000m,
                SoTienConThieu = 4500000m,
                HanThanhToan = new DateOnly(2026, 10, 30),
                TrangThaiHocPhi = "Đóng đợt 1",
                PhuongThuc = "Chuyển khoản (Vietcombank)",
                SoBienNhan = "BN-2026-0067",
                NgayThanhToan = DateTime.Now.AddDays(-2)
            },
            new TuitionFeeItem
            {
                MaHocPhi = "HP2026-006",
                MaDangKy = "DK006",
                MaHocVien = "HV006",
                HoTenHocVien = "Đỗ Gia Huy",
                TenKhoaLop = "Tiếng Anh Giao tiếp căn bản (Lớp GT-A01)",
                SoTienGoc = 4500000m,
                SoTienGiam = 0m,
                SoTienPhaiDong = 4500000m,
                SoTienDaDong = 4500000m,
                SoTienConThieu = 0m,
                HanThanhToan = new DateOnly(2026, 10, 12),
                TrangThaiHocPhi = "Đã hoàn thành",
                PhuongThuc = "Tiền mặt tại quầy",
                SoBienNhan = "BN-2026-0071",
                NgayThanhToan = DateTime.Now.AddDays(-6)
            },
            new TuitionFeeItem
            {
                MaHocPhi = "HP2026-007",
                MaDangKy = "DK007",
                MaHocVien = "HV007",
                HoTenHocVien = "Hoàng Mai Linh",
                TenKhoaLop = "Luyện thi TOEIC Chuyên sâu 800+ (Lớp TC-A02)",
                SoTienGoc = 7200000m,
                SoTienGiam = 500000m,
                SoTienPhaiDong = 6700000m,
                SoTienDaDong = 0m,
                SoTienConThieu = 6700000m,
                HanThanhToan = new DateOnly(2026, 10, 28),
                TrangThaiHocPhi = "Chưa thanh toán",
                PhuongThuc = "— Chưa nộp —",
                SoBienNhan = "—",
                NgayThanhToan = null
            }
        ]);

        FilterData();
    }

    private void SwitchTab(int tabIndex)
    {
        _currentTab = tabIndex;

        btnTabTuition.BackColor = _currentTab == 0 ? Theme.Primary : Color.White;
        btnTabTuition.ForeColor = _currentTab == 0 ? Color.White : Theme.TextDark;
        btnTabTuition.Font = _currentTab == 0 ? Theme.FontBold : Theme.FontRegular;

        btnTabPayments.BackColor = _currentTab == 1 ? Theme.Primary : Color.White;
        btnTabPayments.ForeColor = _currentTab == 1 ? Color.White : Theme.TextDark;
        btnTabPayments.Font = _currentTab == 1 ? Theme.FontBold : Theme.FontRegular;

        btnTabReceipts.BackColor = _currentTab == 2 ? Theme.Primary : Color.White;
        btnTabReceipts.ForeColor = _currentTab == 2 ? Color.White : Theme.TextDark;
        btnTabReceipts.Font = _currentTab == 2 ? Theme.FontBold : Theme.FontRegular;

        dgvTuition.Visible = _currentTab == 0;
        dgvPayments.Visible = _currentTab == 1;
        dgvReceipts.Visible = _currentTab == 2;

        btnCollectFee.Visible = _currentTab == 0;
        btnPrintReceipt.Text = _currentTab == 2 ? "In lại biên nhận" : "In biên lai";
        btnPrintReceipt.Location = new Point(pnlToolbar.Width - btnPrintReceipt.Width - 4, 3);
        btnCollectFee.Location = new Point(btnPrintReceipt.Left - btnCollectFee.Width - 8, 3);

        cboTrangThai.Visible = _currentTab == 0;
        cboPhuongThuc.Visible = _currentTab != 2;

        FilterData();
    }

    private void FilterData()
    {
        UpdateStats();

        var search = txtSearch.Text.Trim();
        var status = cboTrangThai.SelectedItem?.ToString();
        var method = cboPhuongThuc.SelectedItem?.ToString();

        if (_currentTab == 0)
        {
            var query = _items.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.MaHocPhi.Contains(search, StringComparison.OrdinalIgnoreCase)
                                      || x.MaHocVien.Contains(search, StringComparison.OrdinalIgnoreCase)
                                      || x.HoTenHocVien.Contains(search, StringComparison.OrdinalIgnoreCase)
                                      || x.TenKhoaLop.Contains(search, StringComparison.OrdinalIgnoreCase)
                                      || x.SoBienNhan.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(status) && status != AllOption)
            {
                query = query.Where(x => x.TrangThaiHocPhi == status);
            }

            if (!string.IsNullOrWhiteSpace(method) && method != AllOption)
            {
                query = query.Where(x => x.PhuongThuc == method);
            }

            var list = query.ToList();
            dgvTuition.Rows.Clear();

            foreach (var item in list)
            {
                var idx = dgvTuition.Rows.Add(
                    item.MaHocPhi,
                    item.MaHocVien,
                    item.HoTenHocVien,
                    item.TenKhoaLop,
                    $"{item.SoTienGoc:N0} ₫",
                    item.SoTienGiam > 0 ? $"-{item.SoTienGiam:N0} ₫" : "0 ₫",
                    $"{item.SoTienPhaiDong:N0} ₫",
                    $"{item.SoTienDaDong:N0} ₫",
                    $"{item.SoTienConThieu:N0} ₫",
                    item.TrangThaiHocPhi,
                    item.PhuongThuc,
                    item.SoBienNhan,
                    item.HanThanhToan.ToString("dd/MM/yyyy")
                );

                if (item.TrangThaiHocPhi.Contains("hoàn thành"))
                    dgvTuition.Rows[idx].Cells["TrangThaiHocPhi"].Style.ForeColor = Theme.Success;
                else if (item.TrangThaiHocPhi.Contains("Đóng đợt 1"))
                    dgvTuition.Rows[idx].Cells["TrangThaiHocPhi"].Style.ForeColor = Theme.Warning;
                else
                    dgvTuition.Rows[idx].Cells["TrangThaiHocPhi"].Style.ForeColor = Theme.Danger;

                if (item.SoTienConThieu > 0)
                    dgvTuition.Rows[idx].Cells["SoTienConThieu"].Style.ForeColor = Theme.Danger;
            }

            lblSummary.Text = $"Hiển thị {list.Count} / {_items.Count} phiếu học phí học viên (Mock Data trực tiếp)";
        }
        else if (_currentTab == 1)
        {
            var query = Api.MockDataStore.GetPayments().AsEnumerable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.TenHocVien.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                         p.MaHocVien.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                         (p.MaGiaoDich != null && p.MaGiaoDich.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            var list = query.ToList();
            dgvPayments.Rows.Clear();
            foreach (var p in list)
            {
                var idx = dgvPayments.Rows.Add(
                    $"GD-{p.MaThanhToan:D4}",
                    p.MaHocVien,
                    p.TenHocVien,
                    $"{p.SoTien:N0} ₫",
                    UiHelpers.FormatDateTime(p.NgayThanhToan),
                    p.PhuongThucThanhToan,
                    p.MaGiaoDich ?? "—",
                    p.MaNhanVienKeToan,
                    p.TrangThai,
                    p.GhiChu ?? "—"
                );
                dgvPayments.Rows[idx].Cells["TrangThai"].Style.ForeColor = Theme.Success;
            }
            lblSummary.Text = $"Hiển thị {list.Count} giao dịch thanh toán học phí";
        }
        else
        {
            var query = Api.MockDataStore.GetReceipts().AsEnumerable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r => r.TenHocVien.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                         r.SoBienNhan.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                         r.MaHocVien.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            var list = query.ToList();
            dgvReceipts.Rows.Clear();
            foreach (var r in list)
            {
                var idx = dgvReceipts.Rows.Add(
                    r.SoBienNhan,
                    r.MaHocVien,
                    r.TenHocVien,
                    r.TenKhoaHoc,
                    $"{r.SoTienXacNhan:N0} ₫",
                    r.PhuongThuc,
                    r.MaNhanVienLap,
                    UiHelpers.FormatDateTime(r.NgayLap),
                    r.TrangThai
                );
                dgvReceipts.Rows[idx].Cells["TrangThai"].Style.ForeColor = Theme.Success;
            }
            lblSummary.Text = $"Hiển thị {list.Count} biên nhận học phí chính thức";
        }
    }

    private void ReloadData()
    {
        txtSearch.Clear();
        cboTrangThai.SelectedIndex = 0;
        cboPhuongThuc.SelectedIndex = 0;
        FilterData();
    }

    private void BtnCollectFee_Click(object? sender, EventArgs e)
    {
        // Dialog ghi nhận thu học phí mới
        using var dlg = new Form
        {
            Text = "Ghi nhận thu tiền học phí & Phát hành biên lai",
            Width = 520,
            Height = 460,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            MaximizeBox = false,
            MinimizeBox = false,
            BackColor = Color.White
        };

        var lblTitle = new Label { Text = "THU HỌC PHÍ HỌC VIÊN", Font = Theme.FontHeading, ForeColor = Theme.Primary, Location = new Point(24, 16), AutoSize = true };

        var lblSelect = new Label { Text = "Chọn phiếu học phí / học viên:", Location = new Point(24, 52), AutoSize = true, Font = Theme.FontBold };
        var cboFees = new ComboBox { Location = new Point(24, 76), Width = 456, DropDownStyle = ComboBoxStyle.DropDownList, Font = Theme.FontRegular };
        
        var debtItems = _items.Where(x => x.SoTienConThieu > 0).ToList();
        if (debtItems.Count == 0) debtItems = _items.ToList();

        foreach (var item in debtItems)
        {
            cboFees.Items.Add($"{item.MaHocPhi} - {item.HoTenHocVien} ({item.TenKhoaLop}) - Còn nợ: {item.SoTienConThieu:N0} ₫");
        }
        cboFees.SelectedIndex = 0;

        var lblAmount = new Label { Text = "Số tiền thu đợt này (VNĐ):", Location = new Point(24, 120), AutoSize = true, Font = Theme.FontBold };
        var txtAmount = new TextBox { Location = new Point(24, 144), Width = 456, Font = Theme.FontRegular, Text = $"{debtItems[0].SoTienConThieu:0}" };

        cboFees.SelectedIndexChanged += (_, _) =>
        {
            if (cboFees.SelectedIndex >= 0 && cboFees.SelectedIndex < debtItems.Count)
            {
                txtAmount.Text = $"{debtItems[cboFees.SelectedIndex].SoTienConThieu:0}";
            }
        };

        var lblMethod = new Label { Text = "Phương thức thanh toán:", Location = new Point(24, 188), AutoSize = true, Font = Theme.FontBold };
        var cboMethod = new ComboBox { Location = new Point(24, 212), Width = 456, DropDownStyle = ComboBoxStyle.DropDownList, Font = Theme.FontRegular };
        cboMethod.Items.AddRange(["Chuyển khoản (Vietcombank)", "Tiền mặt tại quầy", "VNPay-QR / Thẻ"]);
        cboMethod.SelectedIndex = 0;

        var lblNote = new Label { Text = "Ghi chú phiếu thu / Biên nhận:", Location = new Point(24, 256), AutoSize = true, Font = Theme.FontBold };
        var txtNote = new TextBox { Location = new Point(24, 280), Width = 456, Font = Theme.FontRegular, Text = "Học viên đóng học phí kỳ học mới" };

        var btnSubmit = new Button
        {
            Text = "Xác nhận thu tiền",
            Location = new Point(220, 360),
            Size = new Size(150, 38),
            BackColor = Theme.Success,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = Theme.FontBold,
            DialogResult = DialogResult.OK
        };
        btnSubmit.FlatAppearance.BorderSize = 0;

        var btnCancel = new Button
        {
            Text = "Hủy bỏ",
            Location = new Point(380, 360),
            Size = new Size(100, 38),
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Theme.TextDark,
            FlatStyle = FlatStyle.Flat,
            DialogResult = DialogResult.Cancel
        };
        btnCancel.FlatAppearance.BorderSize = 0;

        dlg.Controls.AddRange([lblTitle, lblSelect, cboFees, lblAmount, txtAmount, lblMethod, cboMethod, lblNote, txtNote, btnSubmit, btnCancel]);
        dlg.AcceptButton = btnSubmit;

        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            var selectedIdx = cboFees.SelectedIndex;
            if (selectedIdx >= 0 && selectedIdx < debtItems.Count)
            {
                var target = debtItems[selectedIdx];
                if (decimal.TryParse(txtAmount.Text.Replace(",", "").Replace(".", ""), out var amount) && amount > 0)
                {
                    target.SoTienDaDong += amount;
                    if (target.SoTienDaDong > target.SoTienPhaiDong)
                        target.SoTienDaDong = target.SoTienPhaiDong;
                    
                    target.SoTienConThieu = target.SoTienPhaiDong - target.SoTienDaDong;
                    target.TrangThaiHocPhi = target.SoTienConThieu == 0 ? "Đã hoàn thành" : "Đóng đợt 1";
                    target.PhuongThuc = cboMethod.SelectedItem?.ToString() ?? "Chuyển khoản (Vietcombank)";
                    target.SoBienNhan = $"BN-2026-{Random.Shared.Next(1000, 9999)}";
                    target.NgayThanhToan = DateTime.Now;

                    FilterData();

                    MessageBox.Show(
                        FindForm(),
                        $"Đã ghi nhận thu {amount:N0} VNĐ cho học viên {target.HoTenHocVien}!\n" +
                        $"Số biên nhận mới: {target.SoBienNhan}\n" +
                        $"Trạng thái: {target.TrangThaiHocPhi}",
                        "Thu học phí thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                    );
                }
            }
        }
    }

    private void BtnPrintReceipt_Click(object? sender, EventArgs e)
    {
        if (dgvTuition.CurrentRow is null || dgvTuition.CurrentRow.Index < 0)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn một dòng học phí để xem và in biên lai!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        ShowReceiptDetail(dgvTuition.CurrentRow.Index);
    }

    private void DgvTuition_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        ShowReceiptDetail(e.RowIndex);
    }

    private void ShowReceiptDetail(int rowIndex)
    {
        var maHp = dgvTuition.Rows[rowIndex].Cells["MaHocPhi"].Value?.ToString();
        var item = _items.FirstOrDefault(x => x.MaHocPhi == maHp);
        if (item is null) return;

        using var dlg = new Form
        {
            Text = $"Biên lai thu tiền học phí — {item.SoBienNhan}",
            Width = 560,
            Height = 520,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            MaximizeBox = false,
            MinimizeBox = false,
            BackColor = Color.White
        };

        var pnlHdr = new Panel { Dock = DockStyle.Top, Height = 75, BackColor = Color.FromArgb(240, 249, 255), Padding = new Padding(20, 12, 20, 12) };
        var lblHdrTitle = new Label { Text = "BIÊN LAI THU HỌC PHÍ ĐIỆN TỬ", Font = Theme.FontHeading, ForeColor = Theme.PrimaryDark, AutoSize = true, Location = new Point(20, 12) };
        var lblHdrSub = new Label { Text = $"Số chứng từ: {item.SoBienNhan} | Ngày lập: {(item.NgayThanhToan?.ToString("dd/MM/yyyy HH:mm") ?? "Chưa lập")}", Font = new Font("Segoe UI", 8.8f), ForeColor = Theme.Muted, AutoSize = true, Location = new Point(20, 42) };
        pnlHdr.Controls.AddRange([lblHdrTitle, lblHdrSub]);

        var txtContent = new TextBox
        {
            Multiline = true,
            ReadOnly = true,
            BackColor = Color.FromArgb(248, 250, 252),
            Font = new Font("Consolas", 10f),
            Location = new Point(24, 95),
            Size = new Size(496, 310),
            BorderStyle = BorderStyle.FixedSingle,
            Text =
                $"========================================================\r\n" +
                $"          TRUNG TÂM NGOẠI NGỮ HUIT LINGOHUB             \r\n" +
                $"           HỆ THỐNG QUẢN LÝ ĐÀO TẠO TRUNG TÂM ANH NGỮ   \r\n" +
                $"========================================================\r\n\r\n" +
                $" Mã phiếu học phí : {item.MaHocPhi}\r\n" +
                $" Mã đăng ký học   : {item.MaDangKy}\r\n" +
                $" Mã học viên      : {item.MaHocVien}\r\n" +
                $" Họ tên học viên  : {item.HoTenHocVien}\r\n" +
                $" Khóa học/Lớp     : {item.TenKhoaLop}\r\n\r\n" +
                $" ------------------------------------------------------\r\n" +
                $" Học phí gốc      : {item.SoTienGoc,16:N0} VNĐ\r\n" +
                $" Giảm trừ ưu đãi  : {item.SoTienGiam,16:N0} VNĐ\r\n" +
                $" Học phí phải đóng: {item.SoTienPhaiDong,16:N0} VNĐ\r\n" +
                $" ĐÃ THANH TOÁN    : {item.SoTienDaDong,16:N0} VNĐ\r\n" +
                $" CÒN LẠI CẦN THU  : {item.SoTienConThieu,16:N0} VNĐ\r\n" +
                $" ------------------------------------------------------\r\n" +
                $" Phương thức      : {item.PhuongThuc}\r\n" +
                $" Trạng thái       : {item.TrangThaiHocPhi}\r\n" +
                $" Kế toán thu tiền : NVKT001 - Lê Ngọc Mai\r\n\r\n" +
                $" Ghi chú: Biên lai điện tử có giá trị pháp lý kế toán."
        };

        var btnPrint = new Button
        {
            Text = "In hóa đơn / Lưu PDF",
            Location = new Point(250, 422),
            Size = new Size(160, 36),
            BackColor = Theme.Primary,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = Theme.FontBold
        };
        btnPrint.FlatAppearance.BorderSize = 0;
        btnPrint.Click += (_, _) =>
        {
            MessageBox.Show(dlg, $"Đã gửi lệnh in biên lai \"{item.SoBienNhan}\" thành công!", "In biên lai", MessageBoxButtons.OK, MessageBoxIcon.Information);
        };

        var btnClose = new Button
        {
            Text = "Đóng",
            Location = new Point(420, 422),
            Size = new Size(100, 36),
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Theme.TextDark,
            FlatStyle = FlatStyle.Flat
        };
        btnClose.FlatAppearance.BorderSize = 0;
        btnClose.Click += (_, _) => dlg.Close();

        dlg.Controls.AddRange([pnlHdr, txtContent, btnPrint, btnClose]);
        dlg.ShowDialog(FindForm());
    }
}
