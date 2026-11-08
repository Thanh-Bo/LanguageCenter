using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;
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
    public DateOnly? HanThanhToan { get; set; }
    public string TrangThaiHocPhi { get; set; } = string.Empty;
    public string PhuongThuc { get; set; } = string.Empty;
    public string SoBienNhan { get; set; } = string.Empty;
    public DateTime? NgayThanhToan { get; set; }
    /// <summary>Phiếu đang bị ẩn khỏi sổ học phí.</summary>
    public bool DaAn { get; set; }
}

public partial class UcHocPhi : UserControl
{
    private const string AllOption = "— Tất cả —";

    /// <summary>Mục lọc "Đã ẩn" — xem lại các phiếu đã ẩn để bỏ ẩn.</summary>
    private const string AnOption = "Đã ẩn";
    private readonly List<TuitionFeeItem> _items = new();
    private List<PaymentDto> _payments = new();
    private int _currentTab = 0; // 0: Sổ học phí, 1: Lịch sử thanh toán, 2: Quản lý biên nhận

    private Button _btnTaoPhieu = null!;
    private Button _btnSuaPhieu = null!;
    private Button _btnAnPhieu = null!;

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
        btnRefresh.Click += async (_, _) => await ReloadAsync();
        btnCollectFee.Click += BtnCollectFee_Click;
        btnPrintReceipt.Click += BtnPrintReceipt_Click;
        dgvTuition.CellDoubleClick += DgvTuition_CellDoubleClick;

        _btnTaoPhieu = UiHelpers.CreateSecondaryButton("Tạo phiếu học phí", 150, 34);
        _btnTaoPhieu.Click += BtnTaoPhieu_Click;
        pnlToolbar.Controls.Add(_btnTaoPhieu);

        _btnSuaPhieu = UiHelpers.CreateSecondaryButton("Sửa phiếu", 120, 34);
        _btnSuaPhieu.Click += async (_, _) => await SuaPhieuAsync();
        pnlToolbar.Controls.Add(_btnSuaPhieu);

        _btnAnPhieu = UiHelpers.CreateSecondaryButton("Ẩn phiếu", 120, 34);
        _btnAnPhieu.Click += async (_, _) => await AnPhieuAsync();
        pnlToolbar.Controls.Add(_btnAnPhieu);

        pnlToolbar.Resize += (_, _) => SapXepThanhCongCu();
        SapXepThanhCongCu();

        InitFilterOptions();
        SwitchTab(0);

        Load += async (_, _) => await ReloadAsync();
    }

    private void InitFilterOptions()
    {
        cboTrangThai.Items.Clear();
        cboTrangThai.Items.Add(AllOption);
        cboTrangThai.Items.AddRange(["Chưa thanh toán", "Thanh toán một phần", "Đã thanh toán", AnOption]);
        cboTrangThai.SelectedIndex = 0;

        cboPhuongThuc.Items.Clear();
        cboPhuongThuc.Items.Add(AllOption);
        cboPhuongThuc.Items.AddRange(["Tiền mặt", "Chuyển khoản"]);
        cboPhuongThuc.SelectedIndex = 0;
    }

    /// <summary>Tải sổ học phí và toàn bộ giao dịch thanh toán từ API.</summary>
    private async Task ReloadAsync()
    {
        lblSummary.Text = "Đang tải dữ liệu học phí...";
        UseWaitCursor = true;

        var tuitionTask = AppSession.Api.GetAsync<PagedResult<TuitionItemDto>>(
            QueryStringBuilder.Build(
                "admin/tuition-fees",
                ("page", "1"),
                ("pageSize", "500"),
                // Lấy CẢ phiếu đã ẩn → lọc phía client theo mục "Đã ẩn", tránh gọi lại API.
                ("baoGomDaAn", "true")));
        var paymentsTask = AppSession.Api.GetAsync<PagedResult<PaymentDto>>(
            QueryStringBuilder.Build("admin/payments", ("page", "1"), ("pageSize", "500")));

        await Task.WhenAll(tuitionTask, paymentsTask);

        UseWaitCursor = false;

        _payments = paymentsTask.Result.Data?.Items?.ToList() ?? new List<PaymentDto>();

        var dsHocPhi = tuitionTask.Result.Data?.Items;
        if (dsHocPhi is null)
        {
            lblSummary.Text = tuitionTask.Result.ErrorMessage;
            return;
        }

        _items.Clear();
        foreach (var d in dsHocPhi)
        {
            // Giao dịch gần nhất của phiếu học phí này (để hiển thị phương thức + số biên nhận)
            var gd = _payments
                .Where(p => p.MaHocPhi == d.MaHocPhi)
                .OrderByDescending(p => p.NgayThanhToan)
                .FirstOrDefault();

            _items.Add(new TuitionFeeItem
            {
                MaHocPhi = d.MaHocPhi.ToString(),
                MaDangKy = d.MaDangKy.ToString(),
                MaHocVien = d.MaHocVien,
                HoTenHocVien = d.HoTenHocVien,
                TenKhoaLop = string.IsNullOrWhiteSpace(d.MaLop)
                    ? d.TenKhoaHoc
                    : $"{d.TenKhoaHoc} (Lớp {d.MaLop})",
                SoTienGoc = d.SoTienGoc,
                SoTienGiam = d.SoTienGiam,
                SoTienPhaiDong = d.SoTienPhaiDong,
                SoTienDaDong = d.SoTienDaDong,
                SoTienConThieu = d.SoTienConThieu,
                HanThanhToan = d.HanThanhToan,
                TrangThaiHocPhi = d.TrangThaiHocPhi,
                PhuongThuc = gd?.PhuongThucThanhToan ?? "— Chưa nộp —",
                SoBienNhan = gd?.BienNhan?.SoBienNhan ?? "—",
                NgayThanhToan = gd?.NgayThanhToan,
                DaAn = d.DaAn
            });
        }

        FilterData();
    }

    private void UpdateStats()
    {
        // Phiếu đã ẩn KHÔNG được tính vào tổng (khớp với báo cáo ở backend).
        var hienThi = _items.Where(x => !x.DaAn).ToList();

        var totalMustPay = hienThi.Sum(x => x.SoTienPhaiDong);
        var totalCollected = hienThi.Sum(x => x.SoTienDaDong);
        var totalDebt = hienThi.Sum(x => x.SoTienConThieu);
        var rate = totalMustPay > 0 ? (totalCollected / totalMustPay) * 100m : 0m;

        tblStats.Controls.Clear();
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Tổng học phí phải thu", $"{totalMustPay:N0} ₫", "Sau chiết khấu & ưu đãi", Theme.Primary), 0, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Đã thực thu vào quỹ", $"{totalCollected:N0} ₫", "Tiền mặt & Chuyển khoản", Theme.Success), 1, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Công nợ cần thu", $"{totalDebt:N0} ₫", "Học viên còn nợ học phí", Theme.Danger), 2, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Tỷ lệ thu học phí", $"{rate:F1}%", "Chỉ tiêu thu kỳ hiện hành", Theme.Purple), 3, 0);
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
        if (_btnTaoPhieu is not null) _btnTaoPhieu.Visible = _currentTab == 0;
        if (_btnSuaPhieu is not null) _btnSuaPhieu.Visible = _currentTab == 0;
        if (_btnAnPhieu is not null) _btnAnPhieu.Visible = _currentTab == 0;
        SapXepThanhCongCu();

        cboTrangThai.Visible = _currentTab == 0;
        cboPhuongThuc.Visible = _currentTab != 2;

        FilterData();
    }

    private void SapXepThanhCongCu()
    {
        var right = pnlToolbar.Width - 4;

        foreach (var button in new[] { btnPrintReceipt, btnCollectFee, _btnTaoPhieu, _btnAnPhieu, _btnSuaPhieu })
        {
            if (button is null || !button.Visible) continue;

            right -= button.Width;
            button.Location = new Point(right, 3);
            right -= 8;
        }
    }

    /// <summary>
    /// Ẩn / bỏ ẩn phiếu học phí đang chọn khỏi sổ.
    /// <para>
    /// Chỉ đổi CỜ ẨN — không xoá giao dịch hay biên nhận.
    /// Phiếu đã ẩn không tính vào 4 thẻ tổng và không hiện ở danh sách mặc định.
    /// </para>
    /// </summary>
    private async Task AnPhieuAsync()
    {
        if (dgvTuition.CurrentRow?.Tag is not TuitionFeeItem item)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn một phiếu học phí cần ẩn / bỏ ẩn.",
                "Chưa chọn phiếu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var seAn = !item.DaAn;

        var hoi = MessageBox.Show(
            FindForm(),
            seAn
                ? $"Ẩn phiếu #{item.MaHocPhi} của {item.HoTenHocVien} khỏi sổ học phí?\n\n"
                  + "Dữ liệu thu tiền và biên nhận vẫn được giữ nguyên. "
                  + "Phiếu sẽ không còn hiện trong danh sách và không tính vào tổng."
                : $"Bỏ ẩn phiếu #{item.MaHocPhi} của {item.HoTenHocVien}?\n\n"
                  + "Phiếu sẽ hiện lại trong sổ học phí và được tính vào tổng.",
            seAn ? "Ẩn phiếu học phí" : "Bỏ ẩn phiếu học phí",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (hoi != DialogResult.Yes)
        {
            return;
        }

        if (!int.TryParse(item.MaHocPhi, out var maHocPhi))
        {
            return;
        }

        UseWaitCursor = true;

        var result = await AppSession.Api.PatchAsync<TuitionItemDto>(
            $"admin/tuition-fees/{maHocPhi}/hide",
            new { daAn = seAn });

        UseWaitCursor = false;

        if (!result.Success)
        {
            MessageBox.Show(FindForm(),
                string.IsNullOrWhiteSpace(result.ErrorMessage)
                    ? "Không đổi được trạng thái ẩn của phiếu."
                    : result.ErrorMessage,
                "Thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        await ReloadAsync();
    }

    /// <summary>Tạo phiếu học phí cho các đăng ký chưa có phiếu.</summary>
    private async void BtnTaoPhieu_Click(object? sender, EventArgs e)
    {
        UseWaitCursor = true;

        var dangKy = await AppSession.Api.GetAsync<PagedResult<EnrollmentListItemDto>>(
            QueryStringBuilder.Build("admin/enrollments", ("page", "1"), ("pageSize", "500")));

        UseWaitCursor = false;

        if (!dangKy.Success || dangKy.Data is null)
        {
            MessageBox.Show(FindForm(),
                string.IsNullOrWhiteSpace(dangKy.ErrorMessage)
                    ? "Không tải được danh sách đăng ký học."
                    : dangKy.ErrorMessage,
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var chuaCoPhieu = dangKy.Data.Items
            .Where(x => !_items.Any(i => i.MaDangKy == x.MaDangKy.ToString()))
            .ToList();

        if (chuaCoPhieu.Count == 0)
        {
            MessageBox.Show(FindForm(),
                "Tất cả đăng ký học đều đã có phiếu học phí.",
                "Không còn đăng ký nào", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new FormHocPhiMoi(chuaCoPhieu);

        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            await ReloadAsync();
        }
    }

    /// <summary>Mở hộp thoại sửa phiếu học phí đang chọn (hạn thanh toán / đồng bộ ưu đãi).</summary>
    private async Task SuaPhieuAsync()
    {
        if (dgvTuition.CurrentRow is null || dgvTuition.CurrentRow.Index < 0)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn một dòng học phí cần sửa.",
                "Chưa chọn phiếu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var maHocPhi = dgvTuition.Rows[dgvTuition.CurrentRow.Index].Cells["MaHocPhi"].Value?.ToString();
        var phieu = _items.FirstOrDefault(x => x.MaHocPhi == maHocPhi);

        if (phieu is null)
        {
            MessageBox.Show(FindForm(), "Không tìm thấy phiếu học phí tương ứng trong dữ liệu đã tải.",
                "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var chiTiet = await AppSession.Api.GetAsync<TuitionItemDto>($"admin/tuition-fees/{phieu.MaHocPhi}");

        if (!chiTiet.Success || chiTiet.Data is null)
        {
            MessageBox.Show(FindForm(), chiTiet.ErrorMessage, "Không tải được phiếu",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        using var dlg = new FormSuaHocPhi(chiTiet.Data);

        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            await ReloadAsync();
        }
    }

    private void FilterData()
    {
        UpdateStats();
        var search = txtSearch.Text.Trim();
        var status = cboTrangThai.SelectedItem?.ToString();
        var method = cboPhuongThuc.SelectedItem?.ToString();

        if (_currentTab == 0)
        {
            // Mặc định ẩn các phiếu đã bị ẩn; chọn mục "Đã ẩn" để xem lại và bỏ ẩn.
            var dangXemPhieuAn = string.Equals(status, AnOption, StringComparison.Ordinal);

            if (_btnAnPhieu is not null)
            {
                _btnAnPhieu.Text = dangXemPhieuAn ? "Bỏ ẩn phiếu" : "Ẩn phiếu";
            }

            var query = _items.Where(x => x.DaAn == dangXemPhieuAn);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.MaHocPhi.Contains(search, StringComparison.OrdinalIgnoreCase)
                                      || x.MaHocVien.Contains(search, StringComparison.OrdinalIgnoreCase)
                                      || x.HoTenHocVien.Contains(search, StringComparison.OrdinalIgnoreCase)
                                      || x.TenKhoaLop.Contains(search, StringComparison.OrdinalIgnoreCase)
                                      || x.SoBienNhan.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(status) && status != AllOption && !dangXemPhieuAn)
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
                    item.HanThanhToan?.ToString("dd/MM/yyyy") ?? "—"
                );

                if (item.TrangThaiHocPhi == "Đã thanh toán")
                    dgvTuition.Rows[idx].Cells["TrangThaiHocPhi"].Style.ForeColor = Theme.Success;
                else if (item.TrangThaiHocPhi == "Thanh toán một phần")
                    dgvTuition.Rows[idx].Cells["TrangThaiHocPhi"].Style.ForeColor = Theme.Warning;
                else
                    dgvTuition.Rows[idx].Cells["TrangThaiHocPhi"].Style.ForeColor = Theme.Danger;

                if (item.SoTienConThieu > 0)
                    dgvTuition.Rows[idx].Cells["SoTienConThieu"].Style.ForeColor = Theme.Danger;

                // Gắn dữ liệu để nút "Ẩn phiếu / Bỏ ẩn phiếu" biết đang thao tác phiếu nào.
                dgvTuition.Rows[idx].Tag = item;

                if (item.DaAn)
                {
                    dgvTuition.Rows[idx].DefaultCellStyle.ForeColor = Theme.Muted;
                }
            }

            var tongHienThi = _items.Count - _items.Count(x => x.DaAn);

            lblSummary.Text = dangXemPhieuAn
                ? $"Hiển thị {list.Count} phiếu ĐÃ ẨN — bấm \"Bỏ ẩn phiếu\" để đưa trở lại sổ"
                : $"Hiển thị {list.Count} / {tongHienThi} phiếu học phí học viên"
                  + (_items.Count > tongHienThi ? $" (+{_items.Count - tongHienThi} phiếu đang ẩn)" : string.Empty);
        }
        else if (_currentTab == 1)
        {
            var query = _payments.AsEnumerable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.HoTenHocVien.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                         p.MaHocVien.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                         (p.MaGiaoDich != null && p.MaGiaoDich.Contains(search, StringComparison.OrdinalIgnoreCase)));
            }

            var list = query.OrderByDescending(p => p.NgayThanhToan).ToList();
            dgvPayments.Rows.Clear();
            foreach (var p in list)
            {
                var idx = dgvPayments.Rows.Add(
                    $"GD-{p.MaThanhToan:D4}",
                    p.MaHocVien,
                    p.HoTenHocVien,
                    $"{p.SoTien:N0} ₫",
                    UiHelpers.FormatDateTime(p.NgayThanhToan),
                    p.PhuongThucThanhToan,
                    p.MaGiaoDich ?? "—",
                    p.TenNhanVienKeToan,
                    p.TrangThai,
                    p.GhiChu ?? "—"
                );
                dgvPayments.Rows[idx].Cells["TrangThai"].Style.ForeColor = Theme.Success;
            }
            lblSummary.Text = $"Hiển thị {list.Count} giao dịch thanh toán học phí";
        }
        else
        {
            var query = _payments.Where(p => p.BienNhan is not null).AsEnumerable();
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.HoTenHocVien.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                         (p.BienNhan?.SoBienNhan.Contains(search, StringComparison.OrdinalIgnoreCase) ?? false) ||
                                         p.MaHocVien.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            var list = query.OrderByDescending(p => p.BienNhan!.NgayLap).ToList();
            dgvReceipts.Rows.Clear();
            foreach (var p in list)
            {
                var bn = p.BienNhan!;
                var idx = dgvReceipts.Rows.Add(
                    bn.SoBienNhan,
                    p.MaHocVien,
                    p.HoTenHocVien,
                    p.TenKhoaHoc,
                    $"{p.SoTien:N0} ₫",
                    p.PhuongThucThanhToan,
                    p.TenNhanVienKeToan,
                    UiHelpers.FormatDateTime(bn.NgayLap),
                    bn.TrangThai
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
        _ = ReloadAsync();
    }

    private async void BtnCollectFee_Click(object? sender, EventArgs e)
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

        var lblAmount = new Label { Text = "Số tiền thu (bắt buộc thu ĐỦ trong 1 lần):", Location = new Point(24, 120), AutoSize = true, Font = Theme.FontBold };
        var txtAmount = new TextBox
        {
            Location = new Point(24, 144),
            Width = 456,
            Font = Theme.FontRegular,
            Text = $"{debtItems[0].SoTienConThieu:0}",
            // NGUYÊN TẮC THU MỘT LẦN: khoá cứng đúng số còn thiếu, không cho sửa.
            ReadOnly = true,
            TabStop = false,
            BackColor = Theme.ContentBackground
        };

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

        if (dlg.ShowDialog(FindForm()) != DialogResult.OK)
        {
            return;
        }

        var selectedIdx = cboFees.SelectedIndex;
        if (selectedIdx < 0 || selectedIdx >= debtItems.Count)
        {
            return;
        }

        var target = debtItems[selectedIdx];

        if (!decimal.TryParse(txtAmount.Text.Replace(",", "").Replace(".", ""), out var amount) || amount <= 0)
        {
            MessageBox.Show(FindForm(), "Số tiền thu không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var phuongThuc = cboMethod.SelectedItem?.ToString() ?? "Tiền mặt";

        UseWaitCursor = true;

        // Phase 6: ghi nhận thanh toán + tự sinh biên nhận ở backend (transaction + khoá phiếu học phí)
        var result = await AppSession.Api.PostAsync<PaymentDto>(
            $"admin/tuition-fees/{target.MaHocPhi}/payments",
            new
            {
                maGiaoDich = $"GD{DateTime.Now:yyyyMMddHHmmssfff}",
                soTien = amount,
                phuongThucThanhToan = phuongThuc,
                ghiChu = string.IsNullOrWhiteSpace(txtNote.Text) ? null : txtNote.Text.Trim()
            });

        UseWaitCursor = false;

        if (!result.Success || result.Data is null)
        {
            MessageBox.Show(FindForm(), result.ErrorMessage, "Thu học phí thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        MessageBox.Show(
            FindForm(),
            $"Đã ghi nhận thu {amount:N0} VNĐ cho học viên {target.HoTenHocVien}!\n"
            + $"Số biên nhận mới: {result.Data.BienNhan?.SoBienNhan ?? "—"}\n"
            + $"Mã giao dịch: {result.Data.MaGiaoDich ?? "—"}\n"
            + $"Phương thức: {phuongThuc}",
            "Thu học phí thành công",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

        await ReloadAsync();
    }

    private void BtnPrintReceipt_Click(object? sender, EventArgs e)
    {
        // Tab 2: in lại biên nhận đã phát hành (trước đây đọc nhầm lưới học phí).
        if (_currentTab == 2)
        {
            if (dgvReceipts.CurrentRow is null || dgvReceipts.CurrentRow.Index < 0)
            {
                MessageBox.Show(FindForm(), "Vui lòng chọn một biên nhận để in lại!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var soBienNhan = dgvReceipts.Rows[dgvReceipts.CurrentRow.Index].Cells[0].Value?.ToString();
            var payment = _payments.FirstOrDefault(p => p.BienNhan?.SoBienNhan == soBienNhan);
            var phieu = payment is null
                ? null
                : _items.FirstOrDefault(x => x.MaHocPhi == payment.MaHocPhi.ToString());

            if (phieu is null)
            {
                MessageBox.Show(FindForm(), "Không tìm thấy phiếu học phí tương ứng với biên nhận này.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ShowReceiptDetail(phieu);
            return;
        }

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

        ShowReceiptDetail(item);
    }

    private void ShowReceiptDetail(TuitionFeeItem item)
    {
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
            // In thật: chọn máy in "Microsoft Print to PDF" để lưu biên nhận thành PDF.
            using var tai = new System.Drawing.Printing.PrintDocument();
            tai.DocumentName = $"BienNhan_{item.SoBienNhan}";
            tai.PrintPage += (_, ev) =>
            {
                using var font = new Font("Consolas", 10f);
                var vung = new RectangleF(40, 40, ev.PageBounds.Width - 80, ev.PageBounds.Height - 80);
                ev.Graphics!.DrawString(txtContent.Text, font, Brushes.Black, vung);
                ev.HasMorePages = false;
            };

            using var hopThoaiIn = new PrintDialog
            {
                Document = tai,
                UseEXDialog = true,
                AllowSomePages = false,
                AllowSelection = false
            };

            if (hopThoaiIn.ShowDialog(dlg) == DialogResult.OK)
            {
                tai.Print();
            }
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
