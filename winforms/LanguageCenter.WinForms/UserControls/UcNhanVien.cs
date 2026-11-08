using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;

namespace LanguageCenter.WinForms.UserControls;

/// <summary>
/// Màn hình quản lý nhân viên — dùng CHUNG cho 2 tab "Nhân viên học vụ" và "Nhân viên kế toán".
/// <para>
/// Chức vụ được truyền vào constructor và gửi lên server dưới dạng bộ lọc
/// <c>?chucVu=</c>, nên chỉ cần một lớp là phục vụ được cả hai tab.
/// </para>
/// </summary>
public partial class UcNhanVien : UserControl
{
    private const string AllOption = "— Tất cả —";
    private const int PageSize = 20;

    private readonly string _chucVu;
    private readonly Button _btnSua;
    private readonly Button _btnXoa;

    private int _currentPage = 1;
    private int _totalPages = 1;

    /// <param name="chucVu">Một giá trị trong <c>StaffPositions</c>.</param>
    public UcNhanVien(string chucVu)
    {
        _chucVu = chucVu;

        InitializeComponent();

        UI.UiHelpers.StyleGrid(dgvEmployees);
        dgvEmployees.CellDoubleClick += DgvEmployees_CellDoubleClick;

        // Nhãn theo chức vụ để người dùng biết đang ở tab nào.
        var danhXung = chucVu == StaffPositions.NhanVienKeToan ? "kế toán" : "nhân viên học vụ";
        btnAdd.Text = $"+ Thêm {danhXung}";
        txtSearch.PlaceholderText = $"Tìm {danhXung} theo mã, họ tên, email hoặc SĐT...";

        txtSearch.KeyDown += (_, e) =>
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                _ = ReloadAsync(resetPage: true);
            }
        };

        btnSearch.Click += async (_, _) => await ReloadAsync(resetPage: true);
        btnRefresh.Click += async (_, _) => await ReloadAsync(resetPage: false);
        btnAdd.Click += BtnAdd_Click;
        btnPrevPage.Click += async (_, _) => await ChangePageAsync(-1);
        btnNextPage.Click += async (_, _) => await ChangePageAsync(1);

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
            dgvEmployees,
            ("MaNhanVien", "Mã NV", 85),
            ("HoTen", "Họ tên", 170),
            ("SoDienThoai", "SĐT", 100),
            ("Email", "Email", 170),
            ("NgayVaoLam", "Ngày vào làm", 110),
            ("TrangThaiLamViec", "Trạng thái", 100));

        dgvEmployees.Rows.Clear();
        UseWaitCursor = true;

        var status = cboTrangThai.SelectedItem as string;

        var path = QueryStringBuilder.Build(
            "admin/employees/list",
            ("page", _currentPage.ToString()),
            ("pageSize", PageSize.ToString()),
            ("search", txtSearch.Text.Trim()),
            ("chucVu", _chucVu),
            ("trangThai", status == AllOption ? null : status));

        var result = await AppSession.Api.GetAsync<PagedResult<EmployeeListItemDto>>(path);

        if (IsDisposed || Disposing)
        {
            return;
        }

        UseWaitCursor = false;

        if (!result.Success || result.Data is null)
        {
            lblStatus.ForeColor = UI.Theme.Danger;
            lblStatus.Text = string.IsNullOrWhiteSpace(result.ErrorMessage)
                ? "Không tải được danh sách nhân viên."
                : result.ErrorMessage;
            lblPageInfo.Text = string.Empty;
            return;
        }

        var data = result.Data;
        _totalPages = Math.Max(data.TotalPages, 1);

        var itemList = data.Items.ToList();

        foreach (var item in itemList)
        {
            var rowIndex = dgvEmployees.Rows.Add(
                item.MaNhanVien,
                item.HoTen,
                string.IsNullOrWhiteSpace(item.SoDienThoai) ? "—" : item.SoDienThoai,
                string.IsNullOrWhiteSpace(item.Email) ? "—" : item.Email,
                item.NgayVaoLam is { } ngay ? UI.UiHelpers.FormatDate(ngay) : "—",
                item.TrangThaiLamViec);

            dgvEmployees.Rows[rowIndex].Cells["TrangThaiLamViec"].Style.ForeColor =
                UI.UiHelpers.StatusColor(item.TrangThaiLamViec);
        }

        lblPageInfo.Text = itemList.Count == 0
            ? "Không có nhân viên nào khớp bộ lọc."
            : $"Trang {data.Page}/{_totalPages} — {itemList.Count} nhân viên";

        btnPrevPage.Enabled = _currentPage > 1;
        btnNextPage.Enabled = _currentPage < _totalPages;
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

    /// <summary>Mã nhân viên của dòng đang chọn (null nếu chưa chọn).</summary>
    private string? LayMaDangChon()
    {
        if (dgvEmployees.CurrentRow is null)
        {
            return null;
        }

        var ma = Convert.ToString(dgvEmployees.CurrentRow.Cells["MaNhanVien"].Value);
        return string.IsNullOrWhiteSpace(ma) ? null : ma;
    }

    private async void BtnAdd_Click(object? sender, EventArgs e)
    {
        using var dialog = new FormNhanVien(_chucVu);
        dialog.ShowDialog(FindForm());

        if (dialog.DataChanged)
        {
            await ReloadAsync(resetPage: true);
        }
    }

    private async void DgvEmployees_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        var maNhanVien = Convert.ToString(dgvEmployees.Rows[e.RowIndex].Cells["MaNhanVien"].Value) ?? string.Empty;

        using var dialog = new FormNhanVien(_chucVu, maNhanVien);
        dialog.ShowDialog(FindForm());

        if (dialog.DataChanged)
        {
            await ReloadAsync(resetPage: false);
        }
    }

    private async void BtnSua_Click(object? sender, EventArgs e)
    {
        var maNhanVien = LayMaDangChon();

        if (maNhanVien is null)
        {
            lblStatus.ForeColor = UI.Theme.Danger;
            lblStatus.Text = "Chọn một nhân viên trong danh sách để sửa.";
            return;
        }

        using var dialog = new FormNhanVien(_chucVu, maNhanVien);
        dialog.ShowDialog(FindForm());

        if (dialog.DataChanged)
        {
            await ReloadAsync(resetPage: false);
        }
    }

    private async void BtnXoa_Click(object? sender, EventArgs e)
    {
        var maNhanVien = LayMaDangChon();

        if (maNhanVien is null)
        {
            lblStatus.ForeColor = UI.Theme.Danger;
            lblStatus.Text = "Chọn một nhân viên trong danh sách để xóa.";
            return;
        }

        var hoTen = Convert.ToString(dgvEmployees.CurrentRow!.Cells["HoTen"].Value) ?? string.Empty;

        var canhBao = $"Xóa vĩnh viễn nhân viên {maNhanVien} — {hoTen}?\n\n"
            + "Chỉ xóa được khi chưa có dữ liệu nghiệp vụ phụ thuộc (lớp phụ trách, "
            + "đăng ký tư vấn, giao dịch kế toán, biên nhận).\n"
            + "Thao tác không hoàn tác được.";

        // NVKT001 là nhân viên kế toán hệ thống — xoá sẽ làm hỏng luồng thanh toán của học viên.
        if (maNhanVien == StaffPositions.SystemAccountantCode)
        {
            canhBao = $"⚠ {maNhanVien} là NHÂN VIÊN KẾ TOÁN HỆ THỐNG.\n\n"
                + "Đây là người được gán làm \"người ghi nhận\" cho các giao dịch do học viên "
                + "tự thực hiện. Xoá hồ sơ này sẽ khiến chức năng thanh toán của học viên bị từ chối.\n\n"
                + "Nên đổi Trạng thái làm việc thành \"Đã nghỉ việc\" thay vì xoá.\n\n"
                + "Vẫn tiếp tục xoá?";
        }

        var xacNhan = MessageBox.Show(
            FindForm(),
            canhBao,
            "Xác nhận xóa",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2);

        if (xacNhan != DialogResult.Yes)
        {
            return;
        }

        var result = await AppSession.Api.DeleteAsync<bool>($"admin/employees/{maNhanVien}");

        if (!result.Success)
        {
            lblStatus.ForeColor = UI.Theme.Danger;
            lblStatus.Text = string.IsNullOrWhiteSpace(result.ErrorMessage)
                ? "Không xóa được nhân viên."
                : result.ErrorMessage;
            return;
        }

        lblStatus.ForeColor = UI.Theme.Success;
        lblStatus.Text = $"Đã xóa nhân viên {maNhanVien}.";

        await ReloadAsync(resetPage: false);
    }
}
