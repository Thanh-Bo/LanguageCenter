using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcTeachers : UserControl
{
    private const string AllOption = "— Tất cả —";
    private const int PageSize = 20;

    private int _currentPage = 1;
    private int _totalPages = 1;

    public UcTeachers()
    {
        InitializeComponent();

        cboTrangThai.Items.Add(AllOption);
        cboTrangThai.Items.AddRange(WorkStatuses.All.ToArray());
        cboTrangThai.SelectedIndex = 0;

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
        dgvTeachers.Rows.Clear();
        UseWaitCursor = true;

        var status = cboTrangThai.SelectedItem as string;

        var path = QueryStringBuilder.Build(
            "admin/teachers",
            ("page", _currentPage.ToString()),
            ("pageSize", PageSize.ToString()),
            ("search", txtSearch.Text.Trim()),
            ("loaiGiaoVien", TeacherTypes.GiaoVien));

        var result = await AppSession.Api.GetAsync<PagedResult<TeacherListItemDto>>(path);

        UseWaitCursor = false;

        if (!result.Success || result.Data is null)
        {
            lblStatus.Text = result.ErrorMessage;
            lblPageInfo.Text = string.Empty;
            return;
        }

        var data = result.Data;
        _totalPages = Math.Max(data.TotalPages, 1);

        var items = data.Items.AsEnumerable();
        if (!string.IsNullOrWhiteSpace(status) && status != AllOption)
        {
            items = items.Where(t => t.TrangThai == status);
        }

        var searchTxt = txtSearch.Text.Trim();
        if (!string.IsNullOrWhiteSpace(searchTxt))
        {
            items = items.Where(t => t.HoTen.Contains(searchTxt, StringComparison.OrdinalIgnoreCase)
                                  || t.MaGiaoVien.Contains(searchTxt, StringComparison.OrdinalIgnoreCase)
                                  || (!string.IsNullOrEmpty(t.Email) && t.Email.Contains(searchTxt, StringComparison.OrdinalIgnoreCase)));
        }

        var itemList = items.ToList();

        foreach (var item in itemList)
        {
            var rowIndex = dgvTeachers.Rows.Add(
                item.MaGiaoVien,
                item.HoTen,
                string.IsNullOrWhiteSpace(item.Email) ? "—" : item.Email,
                string.IsNullOrWhiteSpace(item.SoDienThoai) ? "—" : item.SoDienThoai,
                string.IsNullOrWhiteSpace(item.ChuyenMon) ? "—" : item.ChuyenMon,
                item.SoNamKinhNghiem?.ToString() ?? "—",
                item.TrangThai);

            dgvTeachers.Rows[rowIndex].Cells["TrangThai"].Style.ForeColor = UI.UiHelpers.StatusColor(item.TrangThai);
        }

        lblPageInfo.Text = itemList.Count == 0
            ? "Không có giáo viên nào khớp bộ lọc."
            : $"Trang {data.Page}/{_totalPages} — {itemList.Count} giáo viên giảng dạy";

        btnPrevPage.Enabled = _currentPage > 1;
        btnNextPage.Enabled = _currentPage < _totalPages;
    }

    private async void BtnAdd_Click(object? sender, EventArgs e)
    {
        using var dialog = new FormTeacherEdit();
        dialog.ShowDialog(FindForm());

        if (dialog.DataChanged)
        {
            await ReloadAsync(resetPage: true);
        }
    }

    private async void DgvTeachers_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        var teacherId = Convert.ToString(dgvTeachers.Rows[e.RowIndex].Cells["MaGiaoVien"].Value) ?? string.Empty;

        using var dialog = new FormTeacherEdit(teacherId);
        dialog.ShowDialog(FindForm());

        if (dialog.DataChanged)
        {
            await ReloadAsync(resetPage: false);
        }
    }
}
