using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcStudents : UserControl
{
    private const string AllOption = "— Tất cả —";
    private const int PageSize = 20;

    private int _currentPage = 1;
    private int _totalPages = 1;

    public UcStudents()
    {
        InitializeComponent();

        cboStatus.Items.Add(AllOption);
        cboStatus.Items.AddRange(StudentStatuses.All.ToArray());
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
        dgvStudents.Rows.Clear();
        UseWaitCursor = true;

        var status = cboStatus.SelectedItem as string;

        var path = QueryStringBuilder.Build(
            "admin/students",
            ("page", _currentPage.ToString()),
            ("pageSize", PageSize.ToString()),
            ("search", txtSearch.Text.Trim()),
            ("trangThai", status == AllOption ? null : status));

        var result = await AppSession.Api.GetAsync<PagedResult<StudentListItemDto>>(path);

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
            var rowIndex = dgvStudents.Rows.Add(
                item.MaHocVien,
                item.HoTen,
                UI.UiHelpers.FormatDate(item.NgaySinh),
                string.IsNullOrWhiteSpace(item.GioiTinh) ? "—" : item.GioiTinh,
                string.IsNullOrWhiteSpace(item.Email) ? "—" : item.Email,
                string.IsNullOrWhiteSpace(item.SoDienThoai) ? "—" : item.SoDienThoai,
                item.TrangThaiHocTap,
                UI.UiHelpers.FormatDate(item.NgayNhapHoc));

            dgvStudents.Rows[rowIndex].Cells["TrangThaiHocTap"].Style.ForeColor = UI.UiHelpers.StatusColor(item.TrangThaiHocTap);
        }

        lblPageInfo.Text = data.TotalItems == 0
            ? "Không có học viên nào khớp bộ lọc."
            : $"Trang {data.Page}/{_totalPages} — {data.TotalItems} học viên";

        btnPrevPage.Enabled = _currentPage > 1;
        btnNextPage.Enabled = _currentPage < _totalPages;
    }

    private async void BtnAdd_Click(object? sender, EventArgs e)
    {
        using var dialog = new FormStudentEdit();
        dialog.ShowDialog(FindForm());

        if (dialog.DataChanged)
        {
            await ReloadAsync(resetPage: true);
        }
    }

    private async void DgvStudents_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        var studentId = Convert.ToString(dgvStudents.Rows[e.RowIndex].Cells["MaHocVien"].Value) ?? string.Empty;

        using var dialog = new FormStudentEdit(studentId);
        dialog.ShowDialog(FindForm());

        if (dialog.DataChanged)
        {
            await ReloadAsync(resetPage: false);
        }
    }
}
