using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcThongBao : UserControl
{
    private const string AllOption = "— Tất cả —";

    private IReadOnlyList<NotificationListItemDto> _all = Array.Empty<NotificationListItemDto>();
    private bool _dangNap;

    private Button _btnSua = null!;
    private Button _btnXoa = null!;

    public UcThongBao()
    {
        InitializeComponent();

        UI.UiHelpers.StyleGrid(dgvNotifications);
        dgvNotifications.CellDoubleClick += DgvNotifications_CellDoubleClick;

        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; ApplyFilter(); } };
        cboLoai.SelectedIndexChanged += (_, _) => { if (!_dangNap) ApplyFilter(); };
        cboDoiTuong.SelectedIndexChanged += (_, _) => { if (!_dangNap) ApplyFilter(); };
        btnSearch.Click += (_, _) => ApplyFilter();
        btnRefresh.Click += async (_, _) => await ReloadAsync();
        btnAdd.Click += BtnAdd_Click;

        _btnSua = UiHelpers.CreateSecondaryButton("Sửa", 90, 34);
        _btnSua.Click += BtnSua_Click;

        _btnXoa = UiHelpers.CreateDangerButton("Thu hồi / Xoá", 130, 34);
        _btnXoa.Click += BtnXoa_Click;

        pnlToolbar.Controls.Add(_btnSua);
        pnlToolbar.Controls.Add(_btnXoa);

        pnlToolbar.Resize += (_, _) => SapXepThanhCongCu();
        SapXepThanhCongCu();

        Load += async (_, _) => await ReloadAsync();
    }

    /// <summary>Tải danh sách thông báo (quản trị) từ API.</summary>
    private async Task ReloadAsync()
    {
        lblSummary.Text = "Đang tải danh sách thông báo...";
        UseWaitCursor = true;

        var result = await AppSession.Api.GetAsync<PagedResult<NotificationListItemDto>>(
            QueryStringBuilder.Build("admin/notifications", ("page", "1"), ("pageSize", "500")));

        UseWaitCursor = false;

        _all = result.Data?.Items ?? Array.Empty<NotificationListItemDto>();

        if (!result.Success)
        {
            lblSummary.Text = result.ErrorMessage;
        }

        FillStats();
        InitFilterOptions();
        ApplyFilter();
    }

    private void FillStats()
    {
        var tong = _all.Count;
        var lichHoc = _all.Count(n => n.LoaiThongBao == "Lịch học");
        var hocTap = _all.Count(n => n.LoaiThongBao == "Học tập");
        var hocPhi = _all.Count(n => n.LoaiThongBao == "Học phí");

        tblStats.Controls.Clear();
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Tổng số thông báo", $"{tong} Bản tin", "Hệ thống tin tức nội bộ", Theme.Primary), 0, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Lịch học", $"{lichHoc} Bản tin", "Thay đổi lịch & phòng học", Theme.Success), 1, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Học tập", $"{hocTap} Bản tin", "Thi cử, chuyên môn & lớp học", Theme.Purple), 2, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Học phí", $"{hocPhi} Bản tin", "Tài chính & hạn nộp học phí", Theme.Warning), 3, 0);
    }

    private void InitFilterOptions()
    {
        _dangNap = true;

        var loaiDangChon = cboLoai.SelectedItem?.ToString() ?? AllOption;
        var phamViDangChon = cboDoiTuong.SelectedItem?.ToString() ?? AllOption;

        cboLoai.Items.Clear();
        cboLoai.Items.Add(AllOption);
        cboLoai.Items.AddRange(["Lịch học", "Học tập", "Học phí", "Khác"]);
        var viTriLoai = cboLoai.Items.IndexOf(loaiDangChon);
        cboLoai.SelectedIndex = viTriLoai >= 0 ? viTriLoai : 0;

        cboDoiTuong.Items.Clear();
        cboDoiTuong.Items.Add(AllOption);
        cboDoiTuong.Items.AddRange(["Theo lớp", "Cá nhân"]);
        var viTriPhamVi = cboDoiTuong.Items.IndexOf(phamViDangChon);
        cboDoiTuong.SelectedIndex = viTriPhamVi >= 0 ? viTriPhamVi : 0;

        _dangNap = false;
    }

    public void LoadNotifications()
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var kw = txtSearch.Text.Trim();
        var loai = cboLoai.SelectedItem?.ToString() ?? AllOption;
        var dt = cboDoiTuong.SelectedItem?.ToString() ?? AllOption;

        var query = _all.AsEnumerable();

        if (loai != AllOption) query = query.Where(n => n.LoaiThongBao == loai);

        if (dt == "Theo lớp") query = query.Where(n => n.MaLopHoc.HasValue);
        else if (dt == "Cá nhân") query = query.Where(n => n.MaTaiKhoanNhan.HasValue);

        if (!string.IsNullOrWhiteSpace(kw))
        {
            query = query.Where(n => n.TieuDe.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                     n.NoiDung.Contains(kw, StringComparison.OrdinalIgnoreCase));
        }

        var list = query.OrderByDescending(n => n.ThoiGianGui).ToList();
        dgvNotifications.Rows.Clear();

        foreach (var n in list)
        {
            var rowIdx = dgvNotifications.Rows.Add(
                $"TB-{n.MaThongBao:D3}",
                n.TieuDe,
                n.LoaiThongBao ?? "—",
                n.MaLopHoc.HasValue
                    ? $"Lớp {n.MaLop}"
                    : (n.TenDangNhapNguoiNhan ?? "Cá nhân"),
                n.TenDangNhapNguoiGui,
                UiHelpers.FormatDateTime(n.ThoiGianGui),
                n.TrangThai
            );

            dgvNotifications.Rows[rowIdx].Tag = n;
            dgvNotifications.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = Theme.Success;
        }

        lblSummary.Text = $"Hiển thị {list.Count}/{_all.Count} thông báo (Nhấp đúp chuột để xem chi tiết)";
    }

    private async void BtnAdd_Click(object? sender, EventArgs e)
    {
        using var dlg = new FormSoanThongBao();
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            await ReloadAsync();
        }
    }

    private void SapXepThanhCongCu()
    {
        var right = pnlToolbar.Width - 4;

        foreach (var button in new[] { _btnXoa, _btnSua, btnAdd })
        {
            if (button is null || !button.Visible) continue;

            right -= button.Width;
            button.Location = new Point(right, 3);
            right -= 8;
        }
    }

    private NotificationListItemDto? ThongBaoDangChon
        => dgvNotifications.CurrentRow?.Tag as NotificationListItemDto;

    private async void BtnSua_Click(object? sender, EventArgs e)
    {
        var thongBao = ThongBaoDangChon;
        if (thongBao is null)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn một thông báo trong bảng.",
                "Chưa chọn thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new FormSuaThongBao(thongBao);

        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            await ReloadAsync();
        }
    }

    private async void BtnXoa_Click(object? sender, EventArgs e)
    {
        var thongBao = ThongBaoDangChon;
        if (thongBao is null)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn một thông báo trong bảng.",
                "Chưa chọn thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var xacNhan = MessageBox.Show(FindForm(),
            $"Xoá vĩnh viễn thông báo TB-{thongBao.MaThongBao:D3}?\n\n"
            + $"Tiêu đề: {thongBao.TieuDe}\n\n"
            + "Nếu chỉ muốn ẩn khỏi học viên, hãy dùng nút Sửa và đặt trạng thái 'Đã thu hồi'.",
            "Xác nhận xoá", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (xacNhan != DialogResult.Yes) return;

        var result = await AppSession.Api.DeleteAsync<NotificationListItemDto>(
            $"admin/notifications/{thongBao.MaThongBao}");

        if (!result.Success)
        {
            MessageBox.Show(FindForm(),
                string.IsNullOrWhiteSpace(result.ErrorMessage) ? "Không xoá được thông báo." : result.ErrorMessage,
                "Không thể xoá", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        await ReloadAsync();
    }

    private void DgvNotifications_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        if (dgvNotifications.Rows[e.RowIndex].Tag is not NotificationListItemDto thongBao) return;

        BtnSua_Click(sender, e);
    }
}

