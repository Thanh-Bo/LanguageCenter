using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcHocVien : UserControl
{
    private const string AllOption = "— Tất cả —";
    private const int PageSize = 20;

    private int _currentPage = 1;
    private int _totalPages = 1;

    private readonly Button _btnSua;
    private readonly Button _btnXoa;

    public UcHocVien()
    {
        InitializeComponent();

        UI.UiHelpers.StyleGrid(dgvStudents);
        dgvStudents.CellDoubleClick += DgvStudents_CellDoubleClick;

        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; _ = ReloadAsync(resetPage: true); } };
        btnSearch.Click += async (_, _) => await ReloadAsync(resetPage: true);
        btnRefresh.Click += async (_, _) => await ReloadAsync(resetPage: false);
        btnAdd.Click += BtnAdd_Click;
        btnPrevPage.Click += async (_, _) => await ChangePageAsync(-1);
        btnNextPage.Click += async (_, _) => await ChangePageAsync(1);

        // Nút Sửa / Xoá đặt cạnh nút Thêm học viên.
        _btnSua = UI.UiHelpers.CreateSecondaryButton("Sửa", 80, 30);
        _btnSua.Click += BtnSua_Click;

        _btnXoa = UI.UiHelpers.CreateDangerButton("Xoá", 80, 30);
        _btnXoa.Click += BtnXoa_Click;

        pnlToolbar.Controls.Add(_btnSua);
        pnlToolbar.Controls.Add(_btnXoa);

        pnlToolbar.Resize += (_, _) => SapXepNut();
        SapXepNut();

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
        // Người dùng có thể chuyển sang màn khác trong lúc chờ API -> control bị Dispose.
        // Lúc đó lưới đã bị hủy (Columns rỗng) nên Rows.Add sẽ ném
        // "No row can be added to a DataGridView control that does not have columns".
        if (IsDisposed || Disposing)
        {
            return;
        }

        if (resetPage)
        {
            _currentPage = 1;
        }

        lblStatus.Text = string.Empty;

        // Phòng hờ: luôn bảo đảm lưới CÓ CỘT trước khi thêm dòng.
        // Nếu không, DataGridView.Rows.Add sẽ ném InvalidOperationException
        // "No row can be added to a DataGridView control that does not have columns".
        UI.UiHelpers.DamBaoCot(
            dgvStudents,
            ("MaHocVien", "Mã HV", 70),
            ("HoTen", "Họ tên", 140),
            ("NgaySinh", "Ngày sinh", 90),
            ("GioiTinh", "Giới tính", 70),
            ("Email", "Email", 140),
            ("SoDienThoai", "SĐT", 90),
            ("TrangThaiHocTap", "Trạng thái", 90),
            ("NgayNhapHoc", "Ngày nhập học", 90));

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

        if (IsDisposed || Disposing)
        {
            return;
        }

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
        using var dialog = new FormSuaHocVien();
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

        using var dialog = new FormSuaHocVien(studentId);
        dialog.ShowDialog(FindForm());

        if (dialog.DataChanged)
        {
            await ReloadAsync(resetPage: false);
        }
    }

    /// <summary>Xếp nút Thêm / Sửa / Xoá sát mép phải thanh công cụ.</summary>
    private void SapXepNut()
    {
        var x = pnlToolbar.Width - btnAdd.Width - 4;
        btnAdd.Location = new Point(x, 1);

        x -= _btnSua.Width + 6;
        _btnSua.Location = new Point(x, 4);

        x -= _btnXoa.Width + 6;
        _btnXoa.Location = new Point(x, 4);
    }

    /// <summary>Mã học viên của dòng đang chọn (null nếu chưa chọn).</summary>
    private string? LayMaDangChon()
    {
        if (dgvStudents.CurrentRow is null)
        {
            return null;
        }

        var ma = Convert.ToString(dgvStudents.CurrentRow.Cells["MaHocVien"].Value);
        return string.IsNullOrWhiteSpace(ma) ? null : ma;
    }

    private async void BtnSua_Click(object? sender, EventArgs e)
    {
        var maHocVien = LayMaDangChon();

        if (maHocVien is null)
        {
            lblStatus.Text = "Chọn một học viên trong danh sách để sửa.";
            return;
        }

        using var dialog = new FormSuaHocVien(maHocVien);
        dialog.ShowDialog(FindForm());

        if (dialog.DataChanged)
        {
            await ReloadAsync(resetPage: false);
        }
    }

    private async void BtnXoa_Click(object? sender, EventArgs e)
    {
        var maHocVien = LayMaDangChon();

        if (maHocVien is null)
        {
            lblStatus.Text = "Chọn một học viên trong danh sách để xóa.";
            return;
        }

        var hoTen = Convert.ToString(dgvStudents.CurrentRow!.Cells["HoTen"].Value) ?? string.Empty;

        var xacNhan = MessageBox.Show(
            FindForm(),
            $"Xóa vĩnh viễn học viên {maHocVien} — {hoTen}?\n\n"
            + "Chỉ xóa được khi học viên chưa có đăng ký học / điểm danh / học phí. "
            + "Thao tác không hoàn tác được.",
            "Xác nhận xóa",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (xacNhan != DialogResult.Yes)
        {
            return;
        }

        var result = await AppSession.Api.DeleteAsync<bool>($"admin/students/{maHocVien}");

        if (!result.Success)
        {
            lblStatus.ForeColor = UI.Theme.Danger;
            lblStatus.Text = result.ErrorMessage;
            return;
        }

        lblStatus.ForeColor = UI.Theme.Success;
        lblStatus.Text = $"Đã xóa học viên {maHocVien}.";

        await ReloadAsync(resetPage: false);
    }
}
