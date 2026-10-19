using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcTaiKhoan : UserControl
{
    private const string AllOption = "— Tất cả —";

    /// <summary>Các lựa chọn số dòng mỗi trang — phải khớp với server (PagedQuery).</summary>
    private static readonly int[] PageSizeOptions = { 15, 25, 50, 100 };

    private int _pageSize = PageSizeOptions[0];
    private int _currentPage = 1;
    private int _totalPages = 1;
    private int _totalItems;

    private Button _btnThemTaiKhoan = null!;
    private Button _btnSuaTaiKhoan = null!;
    private Button _btnXoaTaiKhoan = null!;

    public UcTaiKhoan()
    {
        InitializeComponent();

        UI.UiHelpers.StyleGrid(dgvAccounts);
        dgvAccounts.CellDoubleClick += DgvAccounts_CellDoubleClick;

        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; _ = ReloadAsync(resetPage: true); } };
        btnSearch.Click += async (_, _) => await ReloadAsync(resetPage: true);
        btnRefresh.Click += async (_, _) => await ReloadAsync(resetPage: false);

        // ---- Phân trang ----
        btnFirstPage.Click += async (_, _) => await DiToiTrangAsync(1);
        btnPrevPage.Click += async (_, _) => await DiToiTrangAsync(_currentPage - 1);
        btnNextPage.Click += async (_, _) => await DiToiTrangAsync(_currentPage + 1);
        btnLastPage.Click += async (_, _) => await DiToiTrangAsync(_totalPages);

        foreach (var size in PageSizeOptions)
        {
            cboPageSize.Items.Add(size);
        }

        cboPageSize.SelectedItem = _pageSize;

        // Gắn sau khi đã chọn giá trị để không kích hoạt tải lại ngay khi khởi tạo.
        cboPageSize.SelectedIndexChanged += async (_, _) =>
        {
            if (cboPageSize.SelectedItem is int size && size != _pageSize)
            {
                _pageSize = size;
                await ReloadAsync(resetPage: true);
            }
        };

        cboRole.Items.Add(AllOption);
        cboRole.Items.AddRange(RoleNames.All.ToArray());
        cboRole.SelectedIndex = 0;

        cboStatus.Items.Add(AllOption);
        cboStatus.Items.AddRange(AccountStatuses.All.ToArray());
        cboStatus.SelectedIndex = 0;

        _btnThemTaiKhoan = UI.UiHelpers.CreatePrimaryButton("+ Thêm tài khoản", 150, 34);
        _btnThemTaiKhoan.Click += BtnThemTaiKhoan_Click;

        _btnSuaTaiKhoan = UI.UiHelpers.CreateSecondaryButton("Sửa tài khoản", 130, 34);
        _btnSuaTaiKhoan.Click += BtnSuaTaiKhoan_Click;

        _btnXoaTaiKhoan = UI.UiHelpers.CreateDangerButton("Xoá tài khoản", 130, 34);
        _btnXoaTaiKhoan.Click += BtnXoaTaiKhoan_Click;

        // Neo phải bằng FlowLayoutPanel: tự co giãn theo bề rộng cửa sổ,
        // không còn bị cắt chữ khi cửa sổ hẹp như cách đặt Location thủ công.
        var flpActions = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 3, 4, 0)
        };

        _btnThemTaiKhoan.Margin = new Padding(8, 0, 0, 0);
        _btnSuaTaiKhoan.Margin = new Padding(8, 0, 0, 0);
        _btnXoaTaiKhoan.Margin = new Padding(8, 0, 0, 0);

        // Thứ tự Add = thứ tự từ PHẢI sang TRÁI.
        flpActions.Controls.Add(_btnXoaTaiKhoan);
        flpActions.Controls.Add(_btnSuaTaiKhoan);
        flpActions.Controls.Add(_btnThemTaiKhoan);

        pnlToolbar.Controls.Add(flpActions);

        Load += async (_, _) => await ReloadAsync(resetPage: true);
    }

    private async Task DiToiTrangAsync(int trang)
    {
        var target = Math.Clamp(trang, 1, Math.Max(_totalPages, 1));

        if (target == _currentPage)
        {
            return;
        }

        _currentPage = target;
        await ReloadAsync(resetPage: false);
    }

    private async Task ReloadAsync(bool resetPage, bool daDieuChinhTrang = false)
    {
        if (resetPage)
        {
            _currentPage = 1;
        }

        lblStatus.Text = string.Empty;
        dgvAccounts.Rows.Clear();
        UseWaitCursor = true;

        var role = cboRole.SelectedItem as string;
        var status = cboStatus.SelectedItem as string;

        var path = QueryStringBuilder.Build(
            "admin/accounts",
            ("page", _currentPage.ToString()),
            ("pageSize", _pageSize.ToString()),
            ("search", txtSearch.Text.Trim()),
            ("role", role == AllOption ? null : role),
            ("status", status == AllOption ? null : status));

        var result = await AppSession.Api.GetAsync<PagedResult<AccountListItemDto>>(path);

        UseWaitCursor = false;

        if (!result.Success || result.Data is null)
        {
            lblStatus.Text = result.ErrorMessage;
            lblPageInfo.Text = string.Empty;
            CapNhatNutPhanTrang();
            return;
        }

        var data = result.Data;
        _totalPages = Math.Max(data.TotalPages, 1);
        _totalItems = data.TotalItems;

        // Đang đứng ở trang vượt quá số trang thực tế (ví dụ vừa xoá dòng cuối
        // của trang cuối) → lùi về trang cuối rồi tải lại một lần.
        if (_currentPage > _totalPages && !daDieuChinhTrang)
        {
            _currentPage = _totalPages;
            await ReloadAsync(resetPage: false, daDieuChinhTrang: true);
            return;
        }

        foreach (var item in data.Items)
        {
            var coHoSo = !string.IsNullOrWhiteSpace(item.ProfileType);

            var rowIndex = dgvAccounts.Rows.Add(
                item.Id,
                item.Username,
                coHoSo ? item.FullName : "(chưa có hồ sơ)",
                item.Email,
                item.Role,
                item.Status,
                UI.UiHelpers.FormatDateTime(item.LastLoginAt));

            var row = dgvAccounts.Rows[rowIndex];
            row.Cells["Status"].Style.ForeColor = UI.UiHelpers.StatusColor(item.Status);

            if (!coHoSo)
            {
                // Tài khoản "trơn": chỉ có dòng trong bảng TaiKhoan, không có hồ sơ
                // Học viên/Giáo viên → sẽ không xuất hiện ở màn Quản lý học viên.
                row.Cells["FullName"].Style.ForeColor = Color.FromArgb(217, 119, 6);
                row.Cells["FullName"].ToolTipText =
                    $"Tài khoản '{item.Username}' chưa gắn hồ sơ {item.Role}. "
                    + "Hãy tạo ở màn Quản lý học viên / Quản lý giáo viên để có hồ sơ đầy đủ.";
            }

            row.Tag = item;
        }

        lblPageInfo.Text = _totalItems == 0
            ? "Không có tài khoản nào khớp bộ lọc."
            : $"Trang {_currentPage}/{_totalPages} — {_totalItems} tài khoản";

        CapNhatNutPhanTrang();
    }

    private void CapNhatNutPhanTrang()
    {
        var coNhieuTrang = _totalPages > 1;
        var conTrangTruoc = coNhieuTrang && _currentPage > 1;
        var conTrangSau = coNhieuTrang && _currentPage < _totalPages;

        btnFirstPage.Enabled = conTrangTruoc;
        btnPrevPage.Enabled = conTrangTruoc;
        btnNextPage.Enabled = conTrangSau;
        btnLastPage.Enabled = conTrangSau;
    }

    private AccountListItemDto? TaiKhoanDangChon => dgvAccounts.CurrentRow?.Tag as AccountListItemDto;

    private async void BtnThemTaiKhoan_Click(object? sender, EventArgs e)
    {
        UseWaitCursor = true;

        var tatCa = await AppSession.Api.GetAsync<PagedResult<AccountListItemDto>>(
            QueryStringBuilder.Build("admin/accounts", ("page", "1"), ("pageSize", "500")));

        UseWaitCursor = false;

        var tenDaCo = tatCa.Data?.Items.Select(x => x.Username).ToList() ?? new List<string>();

        using var dialog = new FormTaiKhoan(null, tenDaCo);

        if (dialog.ShowDialog(FindForm()) == DialogResult.OK)
        {
            await ReloadAsync(resetPage: true);
        }
    }

    private async void BtnSuaTaiKhoan_Click(object? sender, EventArgs e)
    {
        if (dgvAccounts.CurrentRow?.Tag is not AccountListItemDto taiKhoan)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn một tài khoản trong bảng.",
                "Chưa chọn tài khoản", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new FormTaiKhoan(taiKhoan);

        if (dialog.ShowDialog(FindForm()) == DialogResult.OK)
        {
            await ReloadAsync(resetPage: false);
        }
    }

    private async void BtnXoaTaiKhoan_Click(object? sender, EventArgs e)
    {
        var taiKhoan = TaiKhoanDangChon;

        if (taiKhoan is null)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn một tài khoản trong bảng.",
                "Chưa chọn tài khoản", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var xacNhan = MessageBox.Show(FindForm(),
            $"Xoá vĩnh viễn tài khoản '{taiKhoan.Username}'{(string.IsNullOrWhiteSpace(taiKhoan.FullName) ? string.Empty : " — " + taiKhoan.FullName)}?\n\n"
            + "Chỉ xoá được khi tài khoản không có dữ liệu nghiệp vụ phụ thuộc "
            + "(lớp học, điểm danh, thanh toán...). Nếu có, hãy đùng trạng thái 'Ngừng sử dụng'.",
            "Xác nhận xoá tài khoản", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (xacNhan != DialogResult.Yes) return;

        UseWaitCursor = true;
        var result = await AppSession.Api.DeleteAsync<AccountDeleteBlockedResponse>($"admin/accounts/{taiKhoan.Id}");
        UseWaitCursor = false;

        if (result.Success)
        {
            await ReloadAsync(resetPage: false);
            return;
        }

        if (result.IsConflict && result.Data is not null)
        {
            MessageBox.Show(FindForm(),
                $"{result.Data.Message}\n\nLý do: {result.Data.Reason}\n\nGợi ý: {result.Data.Suggestion}",
                "Không thể xoá", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        MessageBox.Show(FindForm(),
            string.IsNullOrWhiteSpace(result.ErrorMessage) ? "Không xoá được tài khoản." : result.ErrorMessage,
            "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private async void DgvAccounts_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        var accountId = Convert.ToInt32(dgvAccounts.Rows[e.RowIndex].Cells["Id"].Value);

        using var dialog = new FormChiTietTaiKhoan(accountId);
        dialog.ShowDialog(FindForm());

        if (dialog.DataChanged)
        {
            await ReloadAsync(resetPage: false);
        }
    }
}
