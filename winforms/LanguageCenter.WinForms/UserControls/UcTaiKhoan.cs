using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcTaiKhoan : UserControl
{
    private const string AllOption = "— Tất cả —";
    private const int PageSize = 20;

    private int _currentPage = 1;
    private int _totalPages = 1;

    public UcTaiKhoan()
    {
        InitializeComponent();

        UI.UiHelpers.StyleGrid(dgvAccounts);
        dgvAccounts.CellDoubleClick += DgvAccounts_CellDoubleClick;

        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; _ = ReloadAsync(resetPage: true); } };
        btnSearch.Click += async (_, _) => await ReloadAsync(resetPage: true);
        btnRefresh.Click += async (_, _) => await ReloadAsync(resetPage: false);
        btnPrevPage.Click += async (_, _) => await ChangePageAsync(-1);
        btnNextPage.Click += async (_, _) => await ChangePageAsync(1);

        cboRole.Items.Add(AllOption);
        cboRole.Items.AddRange(RoleNames.All.ToArray());
        cboRole.SelectedIndex = 0;

        cboStatus.Items.Add(AllOption);
        cboStatus.Items.AddRange(AccountStatuses.All.ToArray());
        cboStatus.SelectedIndex = 0;

        Load += async (_, _) => await ReloadAsync(resetPage: true);
    }

    private async Task ChangePageAsync(int delta)
    {
        var target = _currentPage + delta;
        if (target < 1 || target > _totalPages)
        {
            return;
        }

        _currentPage = target;
        await ReloadAsync(resetPage: false);
    }

    private async Task ReloadAsync(bool resetPage)
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
            ("pageSize", PageSize.ToString()),
            ("search", txtSearch.Text.Trim()),
            ("role", role == AllOption ? null : role),
            ("status", status == AllOption ? null : status));

        var result = await AppSession.Api.GetAsync<PagedResult<AccountListItemDto>>(path);

        UseWaitCursor = false;

        if (!result.Success || result.Data is null)
        {
            lblStatus.Text = result.ErrorMessage;
            lblPageInfo.Text = string.Empty;
            return;
        }

        var data = result.Data;
        _totalPages = Math.Max(data.TotalPages, 1);

        foreach (var item in data.Items)
        {
            var rowIndex = dgvAccounts.Rows.Add(
                item.Id,
                item.Username,
                string.IsNullOrWhiteSpace(item.FullName) ? "—" : item.FullName,
                item.Email,
                item.Role,
                item.Status,
                UI.UiHelpers.FormatDateTime(item.LastLoginAt));

            dgvAccounts.Rows[rowIndex].Cells["Status"].Style.ForeColor = UI.UiHelpers.StatusColor(item.Status);
        }

        lblPageInfo.Text = data.TotalItems == 0
            ? "Không có tài khoản nào khớp bộ lọc."
            : $"Trang {data.Page}/{_totalPages} — {data.TotalItems} tài khoản";

        btnPrevPage.Enabled = _currentPage > 1;
        btnNextPage.Enabled = _currentPage < _totalPages;
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
