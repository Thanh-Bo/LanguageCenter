using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcLopHoc : UserControl
{
    private const string AllOption = "— Tất cả —";

    private IReadOnlyList<ClassListItemDto> _all = Array.Empty<ClassListItemDto>();
    private IReadOnlyList<CourseListItemDto> _courses = Array.Empty<CourseListItemDto>();
    private bool _dangNap;

    private Button _btnSua = null!;
    private Button _btnTrangThai = null!;
    private Button _btnXoa = null!;

    public UcLopHoc()
    {
        InitializeComponent();

        UI.UiHelpers.StyleGrid(dgvClasses);
        dgvClasses.CellDoubleClick += DgvClasses_CellDoubleClick;

        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; ApplyFilter(); } };
        cboKhoaHoc.SelectedIndexChanged += (_, _) => { if (!_dangNap) ApplyFilter(); };
        cboTrangThai.SelectedIndexChanged += (_, _) => { if (!_dangNap) ApplyFilter(); };
        btnSearch.Click += (_, _) => ApplyFilter();
        btnRefresh.Click += async (_, _) => await ReloadAsync();
        btnAdd.Click += BtnAdd_Click;

        _btnSua = UiHelpers.CreateSecondaryButton("Sửa lớp", 100, 34);
        _btnSua.Click += BtnSua_Click;

        _btnTrangThai = UiHelpers.CreateSecondaryButton("Đổi trạng thái", 130, 34);
        _btnTrangThai.Click += BtnTrangThai_Click;

        _btnXoa = UiHelpers.CreateDangerButton("Xoá lớp", 100, 34);
        _btnXoa.Click += BtnXoa_Click;

        pnlToolbar.Controls.Add(_btnSua);
        pnlToolbar.Controls.Add(_btnTrangThai);
        pnlToolbar.Controls.Add(_btnXoa);

        btnAdd.Visible = true;
        pnlToolbar.Resize += (_, _) => SapXepThanhCongCu();
        SapXepThanhCongCu();

        _dangNap = true;
        cboTrangThai.Items.Clear();
        cboTrangThai.Items.Add(AllOption);
        cboTrangThai.Items.AddRange(["Đang học", "Chuẩn bị mở", "Đã kết thúc", "Đã hủy"]);
        cboTrangThai.SelectedIndex = 0;

        cboKhoaHoc.Items.Clear();
        cboKhoaHoc.Items.Add(AllOption);
        cboKhoaHoc.SelectedIndex = 0;
        _dangNap = false;

        Load += async (_, _) => await ReloadAsync();
    }

    /// <summary>Tải toàn bộ lớp học từ API rồi lọc/phân trang phía client.</summary>
    private async Task ReloadAsync()
    {
        lblSummary.Text = "Đang tải dữ liệu lớp học...";
        dgvClasses.Rows.Clear();
        UseWaitCursor = true;

        var courses = await AppSession.Api.GetAsync<PagedResult<CourseListItemDto>>(
            QueryStringBuilder.Build("admin/courses", ("page", "1"), ("pageSize", "500")));
        _courses = courses.Data?.Items ?? Array.Empty<CourseListItemDto>();

        var result = await AppSession.Api.GetAsync<PagedResult<ClassListItemDto>>(
            QueryStringBuilder.Build("admin/classes", ("page", "1"), ("pageSize", "500")));

        UseWaitCursor = false;

        if (!result.Success || result.Data is null)
        {
            _all = Array.Empty<ClassListItemDto>();
            lblSummary.Text = result.ErrorMessage;
            FillStats();
            return;
        }

        _all = result.Data.Items;
        FillStats();
        FillCourseFilter();
        ApplyFilter();
    }

    private void FillStats()
    {
        var dangHoc = _all.Count(c => c.TrangThai == "Đang học");
        var chuanBi = _all.Count(c => c.TrangThai == "Chuẩn bị mở");
        var ketThuc = _all.Count(c => c.TrangThai == "Đã kết thúc");

        tblStats.Controls.Clear();
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Tổng số lớp học", $"{_all.Count} Lớp", "Đang quản lý trên hệ thống", Theme.Primary), 0, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Đang diễn ra", $"{dangHoc} Lớp", "Lớp đang trong tiến độ học", Theme.Success), 1, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Chuẩn bị mở", $"{chuanBi} Lớp", "Chuẩn bị khai giảng trong kỳ tới", Theme.Warning), 2, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Đã kết thúc", $"{ketThuc} Lớp", "Khóa học đã hoàn tất", Theme.Muted), 3, 0);
    }

    private void FillCourseFilter()
    {
        _dangNap = true;

        var dangChon = cboKhoaHoc.SelectedItem?.ToString() ?? AllOption;

        cboKhoaHoc.Items.Clear();
        cboKhoaHoc.Items.Add(AllOption);
        foreach (var ten in _all.Select(c => c.TenKhoaHoc).Distinct().OrderBy(x => x))
        {
            cboKhoaHoc.Items.Add(ten);
        }

        var viTri = cboKhoaHoc.Items.IndexOf(dangChon);
        cboKhoaHoc.SelectedIndex = viTri >= 0 ? viTri : 0;

        _dangNap = false;
    }

    private void ApplyFilter()
    {
        var kw = txtSearch.Text.Trim();
        var course = cboKhoaHoc.SelectedItem?.ToString() ?? AllOption;
        var status = cboTrangThai.SelectedItem?.ToString() ?? AllOption;

        var query = _all.AsEnumerable();

        if (course != AllOption) query = query.Where(c => c.TenKhoaHoc == course);
        if (status != AllOption) query = query.Where(c => c.TrangThai == status);

        if (!string.IsNullOrWhiteSpace(kw))
        {
            query = query.Where(c =>
                c.MaLop.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                c.TenLop.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                c.TenKhoaHoc.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                c.TenNhanVienHocVu.Contains(kw, StringComparison.OrdinalIgnoreCase));
        }

        var list = query.OrderBy(c => c.MaLop).ToList();
        dgvClasses.Rows.Clear();

        foreach (var c in list)
        {
            var rowIdx = dgvClasses.Rows.Add(
                c.MaLop,
                c.TenLop,
                c.TenKhoaHoc,
                string.IsNullOrWhiteSpace(c.TenNhanVienHocVu) ? "—" : c.TenNhanVienHocVu,
                $"{c.SiSoHienTai}/{c.SiSoToiDa}",
                c.GioBatDau is { } gioBD && c.GioKetThuc is { } gioKT
                    ? $"{gioBD:HH\\:mm} – {gioKT:HH\\:mm}"
                    : "—",
                TenThu(c.ThuBuoi1) is { } t1 && TenThu(c.ThuBuoi2) is { } t2
                    ? $"{t1} · {t2}"
                    : "—",
                c.SoCaHoc > 0 ? $"{c.SoCaHoc}/20 buổi" : "Chưa sinh buổi",
                UiHelpers.FormatDate(c.NgayBatDau),
                UiHelpers.FormatDate(c.NgayKetThucDuKien),
                c.TrangThai
            );
            dgvClasses.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = UiHelpers.StatusColor(c.TrangThai);

            if (c.SoCaHoc == 0)
            {
                // Lớp chưa có ca học → không điểm danh / không có lịch cho học viên.
                dgvClasses.Rows[rowIdx].Cells["SoCaHoc"].Style.ForeColor = Theme.Warning;
                dgvClasses.Rows[rowIdx].Cells["SoCaHoc"].ToolTipText =
                    "Lớp chưa sinh ca học. Vào màn Lịch học để khai báo lịch định kỳ và sinh ca.";
            }

            dgvClasses.Rows[rowIdx].Tag = c;
        }

        lblSummary.Text = _all.Count == 0
            ? "Chưa có lớp học nào trong hệ thống."
            : $"Hiển thị {list.Count}/{_all.Count} lớp học phù hợp với bộ lọc";
    }

    private void SapXepThanhCongCu()
    {
        var right = pnlToolbar.Width - 4;

        foreach (var button in new[] { _btnXoa, _btnTrangThai, _btnSua, btnAdd })
        {
            if (button is null || !button.Visible) continue;

            right -= button.Width;
            button.Location = new Point(right, 3);
            right -= 8;
        }
    }

    private ClassListItemDto? LopDangChon => dgvClasses.CurrentRow?.Tag as ClassListItemDto;

    /// <summary>1 → "Thứ 2" … 7 → "Chủ nhật"; null nếu chưa khai báo.</summary>
    private static string? TenThu(byte? thu) => thu switch
    {
        1 => "Thứ 2",
        2 => "Thứ 3",
        3 => "Thứ 4",
        4 => "Thứ 5",
        5 => "Thứ 6",
        6 => "Thứ 7",
        7 => "Chủ nhật",
        _ => null
    };

    private IReadOnlyList<string> DanhSachHocVu() => _all
        .Select(c => c.MaNhanVienHocVu)
        .Where(x => !string.IsNullOrWhiteSpace(x))
        .Distinct()
        .OrderBy(x => x)
        .ToList();

    private async void BtnAdd_Click(object? sender, EventArgs e)
    {
        using var dlg = new FormLopHoc(_courses, null, DanhSachHocVu());
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            await ReloadAsync();
        }
    }

    private async void BtnSua_Click(object? sender, EventArgs e)
    {
        var lop = LopDangChon;
        if (lop is null)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn một lớp học trong bảng.",
                "Chưa chọn lớp", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new FormLopHoc(_courses, lop, DanhSachHocVu());
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            await ReloadAsync();
        }
    }

    private void BtnTrangThai_Click(object? sender, EventArgs e)
    {
        var lop = LopDangChon;
        if (lop is null)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn một lớp học trong bảng.",
                "Chưa chọn lớp", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var menu = new ContextMenuStrip { Font = Theme.FontRegular };

        foreach (var trangThai in new[] { "Chuẩn bị mở", "Đang học", "Đã kết thúc", "Đã hủy" })
        {
            var item = menu.Items.Add($"{lop.MaLop} → {trangThai}");
            item.Enabled = trangThai != lop.TrangThai;
            item.Click += async (_, _) => await DoiTrangThaiAsync(lop, trangThai);
        }

        menu.Show(_btnTrangThai, new Point(0, _btnTrangThai.Height));
    }

    private async Task DoiTrangThaiAsync(ClassListItemDto lop, string trangThai)
    {
        var result = await AppSession.Api.PatchAsync<ClassListItemDto>(
            $"admin/classes/{lop.MaLopHoc}/status", new { trangThai });

        if (!result.Success)
        {
            MessageBox.Show(FindForm(),
                string.IsNullOrWhiteSpace(result.ErrorMessage) ? "Không đổi được trạng thái lớp." : result.ErrorMessage,
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        await ReloadAsync();
    }

    private async void BtnXoa_Click(object? sender, EventArgs e)
    {
        var lop = LopDangChon;
        if (lop is null)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn một lớp học trong bảng.",
                "Chưa chọn lớp", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var xacNhan = MessageBox.Show(FindForm(),
            $"Xoá lớp {lop.MaLop} — {lop.TenLop}?\n\n"
            + "Chỉ xoá được khi lớp chưa có đăng ký, phân công, lịch học, ca học hoặc thông báo.",
            "Xác nhận xoá", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (xacNhan != DialogResult.Yes) return;

        var result = await AppSession.Api.DeleteAsync<ClassListItemDto>($"admin/classes/{lop.MaLopHoc}");

        if (!result.Success)
        {
            MessageBox.Show(FindForm(),
                string.IsNullOrWhiteSpace(result.ErrorMessage) ? "Không xoá được lớp học." : result.ErrorMessage,
                "Không thể xoá", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        await ReloadAsync();
    }

    private async void DgvClasses_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;

        if (dgvClasses.Rows[e.RowIndex].Tag is not ClassListItemDto lop) return;

        using var dlg = new FormLopHoc(_courses, lop, DanhSachHocVu());
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            await ReloadAsync();
        }
    }
}

