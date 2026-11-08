using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcGiaoVien : UserControl
{
    private const string AllOption = "— Tất cả —";
    private const int PageSize = 20;

    private int _currentPage = 1;
    private int _totalPages = 1;

    /// <summary>Loại hồ sơ màn hình này quản lý: Giáo viên / Giáo viên nước ngoài.</summary>
    private readonly string _loaiGiaoVien;

    private readonly Button _btnSua;
    private readonly Button _btnXoa;

    public UcGiaoVien() : this(TeacherTypes.GiaoVien)
    {
    }

    public UcGiaoVien(string loaiGiaoVien)
    {
        _loaiGiaoVien = loaiGiaoVien;

        InitializeComponent();

        // Nhãn theo loại hồ sơ để người dùng biết đang ở tab nào.
        var danhXung = loaiGiaoVien == TeacherTypes.GiaoVienNuocNgoai
            ? "giáo viên nước ngoài"
            : "giáo viên";

        btnAdd.Text = $"+ Thêm {danhXung}";
        txtSearch.PlaceholderText = $"Tìm {danhXung} theo mã, họ tên hoặc email...";
        lblPageInfo.Text = string.Empty;

        UI.UiHelpers.StyleGrid(dgvTeachers);
        dgvTeachers.CellDoubleClick += DgvTeachers_CellDoubleClick;

        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; _ = ReloadAsync(resetPage: true); } };
        btnSearch.Click += async (_, _) => await ReloadAsync(resetPage: true);
        btnRefresh.Click += async (_, _) => await ReloadAsync(resetPage: false);
        btnAdd.Click += BtnAdd_Click;
        btnPrevPage.Click += async (_, _) => await ChangePageAsync(-1);
        btnNextPage.Click += async (_, _) => await ChangePageAsync(1);

        // Nút Sửa / Xoá đặt cạnh nút Thêm giáo viên.
        _btnSua = UI.UiHelpers.CreateSecondaryButton("Sửa", 80, 30);
        _btnSua.Click += BtnSua_Click;

        _btnXoa = UI.UiHelpers.CreateDangerButton("Xoá", 80, 30);
        _btnXoa.Click += BtnXoa_Click;

        pnlToolbar.Controls.Add(_btnSua);
        pnlToolbar.Controls.Add(_btnXoa);

        pnlToolbar.Resize += (_, _) => SapXepNut();
        SapXepNut();

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
        UI.UiHelpers.DamBaoCot(
            dgvTeachers,
            ("MaGiaoVien", "Mã GV", 75),
            ("HoTen", "Họ tên giáo viên", 150),
            ("Email", "Email", 140),
            ("SoDienThoai", "SĐT", 95),
            ("ChuyenMon", "Chuyên môn giảng dạy", 160),
            ("SoNamKinhNghiem", "Số năm KN", 80),
            ("TrangThai", "Trạng thái", 90));

        dgvTeachers.Rows.Clear();
        UseWaitCursor = true;

        var status = cboTrangThai.SelectedItem as string;

        var path = QueryStringBuilder.Build(
            "admin/teachers",
            ("page", _currentPage.ToString()),
            ("pageSize", PageSize.ToString()),
            ("search", txtSearch.Text.Trim()),
            ("loaiGiaoVien", _loaiGiaoVien),
            // Lọc trạng thái Ở SERVER: lọc sau khi phân trang chỉ đúng 20 dòng/trang.
            ("trangThai", status == AllOption ? null : status));

        var result = await AppSession.Api.GetAsync<PagedResult<TeacherListItemDto>>(path);

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

        // Không lọc lại ở client nữa — server đã lọc theo trangThai + search.
        var itemList = data.Items.ToList();

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
        // Tab Giáo viên: loại cố định "Giáo viên".
        using var dialog = new FormSuaGiaoVien(_loaiGiaoVien);
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

        using var dialog = new FormSuaGiaoVien(teacherId, _loaiGiaoVien);
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

    /// <summary>Mã giáo viên của dòng đang chọn (null nếu chưa chọn).</summary>
    private string? LayMaDangChon()
    {
        if (dgvTeachers.CurrentRow is null)
        {
            return null;
        }

        var ma = Convert.ToString(dgvTeachers.CurrentRow.Cells["MaGiaoVien"].Value);
        return string.IsNullOrWhiteSpace(ma) ? null : ma;
    }

    private async void BtnSua_Click(object? sender, EventArgs e)
    {
        var maGiaoVien = LayMaDangChon();

        if (maGiaoVien is null)
        {
            lblStatus.Text = "Chọn một giáo viên trong danh sách để sửa.";
            return;
        }

        using var dialog = new FormSuaGiaoVien(maGiaoVien, _loaiGiaoVien);
        dialog.ShowDialog(FindForm());

        if (dialog.DataChanged)
        {
            await ReloadAsync(resetPage: false);
        }
    }

    private async void BtnXoa_Click(object? sender, EventArgs e)
    {
        var maGiaoVien = LayMaDangChon();

        if (maGiaoVien is null)
        {
            lblStatus.Text = "Chọn một giáo viên trong danh sách để xóa.";
            return;
        }

        var hoTen = Convert.ToString(dgvTeachers.CurrentRow!.Cells["HoTen"].Value) ?? string.Empty;

        var xacNhan = MessageBox.Show(
            FindForm(),
            $"Xóa vĩnh viễn giáo viên {maGiaoVien} — {hoTen}?\n\n"
            + "Chỉ xóa được khi chưa có phân công giảng dạy / ca học / điểm đã chấm. "
            + "Thao tác không hoàn tác được.",
            "Xác nhận xóa",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (xacNhan != DialogResult.Yes)
        {
            return;
        }

        var result = await AppSession.Api.DeleteAsync<bool>($"admin/teachers/{maGiaoVien}");

        if (!result.Success)
        {
            lblStatus.ForeColor = UI.Theme.Danger;
            lblStatus.Text = result.ErrorMessage;
            return;
        }

        lblStatus.ForeColor = UI.Theme.Success;
        lblStatus.Text = $"Đã xóa giáo viên {maGiaoVien}.";

        await ReloadAsync(resetPage: false);
    }
}
