using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcKhoaHoc : UserControl
{
    private const string AllOption = "— Tất cả —";
    private int _currentTab = 0; // 0: Khóa học, 1: Cấp độ, 2: Chương trình

    private IReadOnlyList<CourseListItemDto> _courses = Array.Empty<CourseListItemDto>();
    private IReadOnlyList<LevelListItemDto> _levels = Array.Empty<LevelListItemDto>();
    private IReadOnlyList<ProgramListItemDto> _programs = Array.Empty<ProgramListItemDto>();
    private bool _dangNap;

    /// <summary>Nút mở màn nhập NỘI DUNG 20 BUỔI HỌC của khóa đang chọn.</summary>
    private readonly Button _btnNoiDung = UiHelpers.CreateSecondaryButton("Nội dung 20 buổi", 155, 32);

    private readonly Button _btnSua = UiHelpers.CreateSecondaryButton("Sửa", 90, 32);
    private readonly Button _btnXoa = UiHelpers.CreateDangerButton("Xoá / Ngừng", 120, 32);

    public UcKhoaHoc()
    {
        InitializeComponent();

        DamBaoCotBang();

        UI.UiHelpers.StyleGrid(dgvCourses);
        UI.UiHelpers.StyleGrid(dgvLevels);
        UI.UiHelpers.StyleGrid(dgvPrograms);

        dgvCourses.CellDoubleClick += DgvCourses_CellDoubleClick;
        dgvLevels.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) SuaCapDo(); };
        dgvPrograms.CellDoubleClick += (_, e) => { if (e.RowIndex >= 0) SuaChuongTrinh(); };

        btnTabCourses.Click += (_, _) => SwitchTab(0);
        btnTabLevels.Click += (_, _) => SwitchTab(1);
        btnTabPrograms.Click += (_, _) => SwitchTab(2);

        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; ApplyFilter(); } };
        cboChuongTrinh.SelectedIndexChanged += (_, _) => { if (!_dangNap) ApplyFilter(); };
        cboTrangThai.SelectedIndexChanged += (_, _) => { if (!_dangNap) ApplyFilter(); };
        btnSearch.Click += (_, _) => ApplyFilter();
        btnRefresh.Click += async (_, _) => await ReloadAsync();
        btnAdd.Click += BtnAdd_Click;

        _btnNoiDung.Click += BtnNoiDung_Click;
        pnlToolbar.Controls.Add(_btnNoiDung);

        _btnSua.Click += (_, _) => SuaDangChon();
        _btnXoa.Click += async (_, _) => await XoaDangChonAsync();
        pnlToolbar.Controls.Add(_btnSua);
        pnlToolbar.Controls.Add(_btnXoa);

        pnlToolbar.Resize += (_, _) =>
        {
            btnAdd.Location = new Point(pnlToolbar.Width - btnAdd.Width - 4, 3);
            _btnXoa.Location = new Point(btnAdd.Left - _btnXoa.Width - 8, 3);
            _btnSua.Location = new Point(_btnXoa.Left - _btnSua.Width - 8, 3);
            _btnNoiDung.Location = new Point(_btnSua.Left - _btnNoiDung.Width - 8, 3);
        };

        SwitchTab(0);
        Load += async (_, _) => await ReloadAsync();
    }

    /// <summary>
    /// Bảo đảm 3 lưới đều có cột trước khi nạp dữ liệu — chống lỗi
    /// "No row can be added to a DataGridView control that does not have columns"
    /// khi Designer chưa nạp cột.
    /// </summary>
    private void DamBaoCotBang()
    {
        UI.UiHelpers.DamBaoCot(dgvCourses,
            ("MaKhoaHoc", "Mã khóa", 85),
            ("TenKhoaHoc", "Tên khóa học", 190),
            ("TenChuongTrinh", "Chương trình", 130),
            ("CapDo", "Cấp độ", 120),
            ("SoBuoi", "Số buổi", 70),
            ("TongSoGio", "Thời lượng", 80),
            ("HocPhiCoBan", "Học phí niêm yết", 110),
            ("TrangThai", "Trạng thái", 95));

        UI.UiHelpers.DamBaoCot(dgvLevels,
            ("MaCapDo", "Mã CĐ", 60),
            ("TenCapDo", "Tên cấp độ đào tạo", 180),
            ("KyHieu", "Ký hiệu", 85),
            ("TenChuongTrinh", "Chương trình", 130),
            ("ThuTu", "Thứ tự", 60),
            ("ThoiLuong", "Thời lượng (tuần)", 90),
            ("DieuKienDauVao", "Điều kiện đầu vào", 140),
            ("KetQuaDauRa", "Chuẩn đầu ra cam kết", 150),
            ("TrangThai", "Trạng thái", 90));

        UI.UiHelpers.DamBaoCot(dgvPrograms,
            ("MaChuongTrinh", "Mã CT", 60),
            ("TenChuongTrinh", "Tên chương trình đào tạo", 160),
            ("NhomDoiTuong", "Nhóm đối tượng mục tiêu", 180),
            ("DoTuoi", "Độ tuổi", 75),
            ("MucTieu", "Mục tiêu cam kết", 170),
            ("MoTa", "Mô tả tổng quan", 180),
            ("TrangThai", "Trạng thái", 90));
    }

    /// <summary>Tải khóa học, cấp độ và chương trình từ API.</summary>
    private async Task ReloadAsync()
    {
        lblSummary.Text = "Đang tải dữ liệu đào tạo...";
        UseWaitCursor = true;

        var coursesTask = AppSession.Api.GetAsync<PagedResult<CourseListItemDto>>(
            QueryStringBuilder.Build("admin/courses", ("page", "1"), ("pageSize", "500")));
        var levelsTask = AppSession.Api.GetAsync<PagedResult<LevelListItemDto>>(
            QueryStringBuilder.Build("admin/levels", ("page", "1"), ("pageSize", "500")));
        var programsTask = AppSession.Api.GetAsync<PagedResult<ProgramListItemDto>>(
            QueryStringBuilder.Build("admin/programs", ("page", "1"), ("pageSize", "500")));

        await Task.WhenAll(coursesTask, levelsTask, programsTask);

        UseWaitCursor = false;

        _courses = coursesTask.Result.Data?.Items ?? Array.Empty<CourseListItemDto>();
        _levels = levelsTask.Result.Data?.Items ?? Array.Empty<LevelListItemDto>();
        _programs = programsTask.Result.Data?.Items ?? Array.Empty<ProgramListItemDto>();

        if (!coursesTask.Result.Success)
        {
            lblSummary.Text = coursesTask.Result.ErrorMessage;
        }

        FillStats();
        InitFilterOptions();
        FillStatusFilter();
        ApplyFilter();
    }

    private void FillStats()
    {
        var dangHoatDong = _courses.Count(c => c.TrangThai != "Đã kết thúc" && c.TrangThai != "Ngừng hoạt động");

        tblStats.Controls.Clear();
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Tổng số khóa học", $"{_courses.Count} Khóa", "Các cấp độ từ A1 đến C1", Theme.Primary), 0, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Chương trình đào tạo", $"{_programs.Count} Hệ", "IELTS, TOEIC, Giao tiếp, Thiếu nhi", Theme.Purple), 1, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Cấp độ đào tạo", $"{_levels.Count} Cấp độ", "Chuẩn khung tham chiếu CEFR", Theme.Success), 2, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Khóa đang hoạt động", $"{dangHoatDong} Khóa", "Đang tuyển sinh / đang giảng dạy", Theme.Warning), 3, 0);
    }

    private void InitFilterOptions()
    {
        _dangNap = true;

        var dangChon = (cboChuongTrinh.SelectedItem as ChuongTrinhLoc)?.MaChuongTrinh;

        cboChuongTrinh.Items.Clear();
        cboChuongTrinh.Items.Add(AllOption);

        // Lọc theo MÃ chương trình — hai chương trình trùng tên vẫn tách đúng.
        foreach (var ct in _programs.OrderBy(p => p.TenChuongTrinh))
        {
            cboChuongTrinh.Items.Add(new ChuongTrinhLoc(ct.MaChuongTrinh, ct.TenChuongTrinh));
        }

        if (dangChon is null)
        {
            cboChuongTrinh.SelectedIndex = 0;
        }
        else
        {
            var viTri = cboChuongTrinh.Items.Cast<object>()
                .Select((item, i) => (item, i))
                .FirstOrDefault(t => t.item is ChuongTrinhLoc c && c.MaChuongTrinh == dangChon.Value).i;

            cboChuongTrinh.SelectedIndex = viTri > 0 ? viTri : 0;
        }

        _dangNap = false;
    }

    /// <summary>Mục lọc chương trình — hiển thị tên nhưng so sánh bằng MÃ.</summary>
    private sealed record ChuongTrinhLoc(int MaChuongTrinh, string TenChuongTrinh)
    {
        public override string ToString() => TenChuongTrinh;
    }

    private void SwitchTab(int tabIndex)
    {
        _currentTab = tabIndex;

        btnTabCourses.BackColor = _currentTab == 0 ? Theme.Primary : Color.White;
        btnTabCourses.ForeColor = _currentTab == 0 ? Color.White : Theme.TextDark;
        btnTabCourses.Font = _currentTab == 0 ? Theme.FontBold : Theme.FontRegular;

        btnTabLevels.BackColor = _currentTab == 1 ? Theme.Primary : Color.White;
        btnTabLevels.ForeColor = _currentTab == 1 ? Color.White : Theme.TextDark;
        btnTabLevels.Font = _currentTab == 1 ? Theme.FontBold : Theme.FontRegular;

        btnTabPrograms.BackColor = _currentTab == 2 ? Theme.Primary : Color.White;
        btnTabPrograms.ForeColor = _currentTab == 2 ? Color.White : Theme.TextDark;
        btnTabPrograms.Font = _currentTab == 2 ? Theme.FontBold : Theme.FontRegular;

        dgvCourses.Visible = _currentTab == 0;
        dgvLevels.Visible = _currentTab == 1;
        dgvPrograms.Visible = _currentTab == 2;

        btnAdd.Text = _currentTab switch
        {
            0 => "+ Thêm khóa học",
            1 => "+ Thêm cấp độ",
            2 => "+ Thêm chương trình",
            _ => "+ Thêm mới"
        };

        cboChuongTrinh.Visible = _currentTab != 2;

        FillStatusFilter();
        ApplyFilter();
    }

    /// <summary>Danh mục trạng thái lấy từ dữ liệu thật của tab đang chọn.</summary>
    private void FillStatusFilter()
    {
        _dangNap = true;

        var dangChon = cboTrangThai.SelectedItem?.ToString() ?? AllOption;

        IEnumerable<string> muc = _currentTab switch
        {
            0 => _courses.Select(c => c.TrangThai),
            1 => _levels.Select(l => l.TrangThai),
            _ => _programs.Select(p => p.TrangThai)
        };

        cboTrangThai.Items.Clear();
        cboTrangThai.Items.Add(AllOption);
        foreach (var m in muc.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x))
        {
            cboTrangThai.Items.Add(m);
        }

        var viTri = cboTrangThai.Items.IndexOf(dangChon);
        cboTrangThai.SelectedIndex = viTri >= 0 ? viTri : 0;

        _dangNap = false;
    }

    private void ApplyFilter()
    {
        var kw = txtSearch.Text.Trim();
        var prog = cboChuongTrinh.SelectedItem as ChuongTrinhLoc;
        var status = cboTrangThai.SelectedItem?.ToString() ?? AllOption;

        if (_currentTab == 0)
        {
            var courses = _courses.AsEnumerable();
            if (prog is not null) courses = courses.Where(c => c.MaChuongTrinh == prog.MaChuongTrinh);
            if (status != AllOption) courses = courses.Where(c => c.TrangThai == status);
            if (!string.IsNullOrWhiteSpace(kw))
            {
                courses = courses.Where(c => c.TenKhoaHoc.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                             c.MaKhoaHoc.ToString().Contains(kw, StringComparison.OrdinalIgnoreCase));
            }

            var list = courses.OrderBy(c => c.TenKhoaHoc).ToList();
            dgvCourses.Rows.Clear();
            foreach (var c in list)
            {
                var rowIdx = dgvCourses.Rows.Add(
                    c.MaKhoaHoc.ToString(),
                    c.TenKhoaHoc,
                    c.TenChuongTrinh,
                    c.TenCapDo,
                    $"{c.SoBuoi} buổi",
                    c.TongSoGio.HasValue ? $"{c.TongSoGio.Value:0.#} giờ" : "—",
                    $"{c.HocPhiCoBan:N0} ₫",
                    c.TrangThai
                );
                dgvCourses.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = UiHelpers.StatusColor(c.TrangThai);
                dgvCourses.Rows[rowIdx].Tag = c;
            }
            lblSummary.Text = $"Hiển thị {list.Count}/{_courses.Count} khóa học phù hợp";
        }
        else if (_currentTab == 1)
        {
            var levels = _levels.AsEnumerable();
            if (prog is not null) levels = levels.Where(l => l.MaChuongTrinh == prog.MaChuongTrinh);
            if (status != AllOption) levels = levels.Where(l => l.TrangThai == status);
            if (!string.IsNullOrWhiteSpace(kw))
            {
                levels = levels.Where(l => l.TenCapDo.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                           l.KyHieuCapDo.Contains(kw, StringComparison.OrdinalIgnoreCase));
            }

            var list = levels.OrderBy(l => l.TenCapDo).ToList();
            dgvLevels.Rows.Clear();
            foreach (var l in list)
            {
                var rowIdx = dgvLevels.Rows.Add(
                    l.MaCapDo,
                    l.TenCapDo,
                    l.KyHieuCapDo,
                    l.TenChuongTrinh,
                    l.ThuTu,
                    l.ThoiLuongDuKienTuan.HasValue ? $"{l.ThoiLuongDuKienTuan} tuần" : "—",
                    l.DieuKienDauVao ?? "—",
                    l.KetQuaDauRa ?? "—",
                    l.TrangThai
                );
                dgvLevels.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = UiHelpers.StatusColor(l.TrangThai);
                dgvLevels.Rows[rowIdx].Tag = l;
            }
            lblSummary.Text = $"Hiển thị {list.Count}/{_levels.Count} cấp độ đào tạo phù hợp";
        }
        else
        {
            var progs = _programs.AsEnumerable();
            if (status != AllOption) progs = progs.Where(p => p.TrangThai == status);
            if (!string.IsNullOrWhiteSpace(kw))
            {
                progs = progs.Where(p => p.TenChuongTrinh.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                         (p.NhomDoiTuong != null && p.NhomDoiTuong.Contains(kw, StringComparison.OrdinalIgnoreCase)));
            }

            var list = progs.OrderBy(p => p.TenChuongTrinh).ToList();
            dgvPrograms.Rows.Clear();
            foreach (var p in list)
            {
                var doTuoi = p.DoTuoiTu.HasValue || p.DoTuoiDen.HasValue
                    ? $"{p.DoTuoiTu ?? 0} - {p.DoTuoiDen ?? 0} tuổi"
                    : "—";

                var rowIdx = dgvPrograms.Rows.Add(
                    p.MaChuongTrinh,
                    p.TenChuongTrinh,
                    p.NhomDoiTuong ?? "—",
                    doTuoi,
                    p.MucTieu ?? "—",
                    p.MoTa ?? "—",
                    p.TrangThai
                );
                dgvPrograms.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = UiHelpers.StatusColor(p.TrangThai);
                dgvPrograms.Rows[rowIdx].Tag = p;
            }
            lblSummary.Text = $"Hiển thị {list.Count}/{_programs.Count} chương trình đào tạo phù hợp";
        }
    }

    private void ReloadData()
    {
        _ = ReloadAsync();
    }

    /// <summary>
    /// Mở màn nhập NỘI DUNG 20 BUỔI HỌC — mặc định chọn khóa đang chọn trong bảng.
    /// Khi sinh ca học, "Nội dung" của từng buổi sẽ lấy theo đúng số buổi ở đây.
    /// </summary>
    private void BtnNoiDung_Click(object? sender, EventArgs e)
    {
        if (_courses.Count == 0)
        {
            MessageBox.Show(FindForm(), "Chưa có khóa học nào trong hệ thống.",
                "Thiếu dữ liệu", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var maDangChon = (dgvCourses.CurrentRow?.Tag as CourseListItemDto)?.MaKhoaHoc;

        using var dlg = new FormNoiDungBaiHoc(_courses, maDangChon);

        dlg.ShowDialog(FindForm());
    }

    private async void BtnAdd_Click(object? sender, EventArgs e)
    {
        if (_currentTab == 0)
        {
            using var dlg = new FormKhoaHoc(_levels);
            if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
            {
                await ReloadAsync();
            }
            return;
        }

        if (_currentTab == 1)
        {
            using var dlg = new FormCapDo(_programs);
            if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
            {
                await ReloadAsync();
            }
            return;
        }

        using var dlgChuongTrinh = new FormChuongTrinh();
        if (dlgChuongTrinh.ShowDialog(FindForm()) == DialogResult.OK)
        {
            await ReloadAsync();
        }
    }

    /// <summary>Sửa bản ghi đang chọn theo tab hiện tại.</summary>
    private void SuaDangChon()
    {
        if (_currentTab == 0)
        {
            if (dgvCourses.CurrentRow?.Tag is CourseListItemDto khoaHoc)
            {
                using var dlg = new FormKhoaHoc(_levels, khoaHoc);
                if (dlg.ShowDialog(FindForm()) == DialogResult.OK) ReloadData();
            }

            return;
        }

        if (_currentTab == 1)
        {
            SuaCapDo();
            return;
        }

        SuaChuongTrinh();
    }

    private void SuaCapDo()
    {
        if (dgvLevels.CurrentRow?.Tag is not LevelListItemDto capDo)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn một cấp độ trong bảng.",
                "Chưa chọn cấp độ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new FormCapDo(_programs, capDo);
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK) ReloadData();
    }

    private void SuaChuongTrinh()
    {
        if (dgvPrograms.CurrentRow?.Tag is not ProgramListItemDto chuongTrinh)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn một chương trình trong bảng.",
                "Chưa chọn chương trình", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new FormChuongTrinh(chuongTrinh);
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK) ReloadData();
    }

    /// <summary>Xoá (khóa học) hoặc Ngừng hoạt động (cấp độ / chương trình — API không có DELETE).</summary>
    private async Task XoaDangChonAsync()
    {
        if (_currentTab == 0)
        {
            if (dgvCourses.CurrentRow?.Tag is not CourseListItemDto khoaHoc)
            {
                MessageBox.Show(FindForm(), "Vui lòng chọn một khóa học trong bảng.",
                    "Chưa chọn khóa học", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var xacNhan = MessageBox.Show(FindForm(),
                $"Xoá khóa học '{khoaHoc.TenKhoaHoc}'?\n\n"
                + "Chỉ xoá được khi chưa có lớp học / đăng ký / nội dung buổi học / loại điểm.",
                "Xác nhận xoá", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

            if (xacNhan != DialogResult.Yes) return;

            var ketQua = await AppSession.Api.DeleteAsync<CourseListItemDto>($"admin/courses/{khoaHoc.MaKhoaHoc}");
            ThongBaoKetQua(ketQua.Success, ketQua.ErrorMessage, $"Đã xoá khóa học '{khoaHoc.TenKhoaHoc}'.");
            if (ketQua.Success) await ReloadAsync();
            return;
        }

        if (_currentTab == 1)
        {
            if (dgvLevels.CurrentRow?.Tag is not LevelListItemDto capDo) return;

            var ketQua = await AppSession.Api.PatchAsync<LevelListItemDto>(
                $"admin/levels/{capDo.MaCapDo}/status", new { trangThai = "Ngừng hoạt động" });

            ThongBaoKetQua(ketQua.Success, ketQua.ErrorMessage, $"Đã ngừng hoạt động cấp độ '{capDo.TenCapDo}'.");
            if (ketQua.Success) await ReloadAsync();
            return;
        }

        if (dgvPrograms.CurrentRow?.Tag is not ProgramListItemDto chuongTrinh) return;

        var kq = await AppSession.Api.PatchAsync<ProgramListItemDto>(
            $"admin/programs/{chuongTrinh.MaChuongTrinh}/status", new { trangThai = "Ngừng hoạt động" });

        ThongBaoKetQua(kq.Success, kq.ErrorMessage, $"Đã ngừng hoạt động chương trình '{chuongTrinh.TenChuongTrinh}'.");
        if (kq.Success) await ReloadAsync();
    }

    private void ThongBaoKetQua(bool thanhCong, string? loi, string thongDiep)
    {
        if (thanhCong)
        {
            lblSummary.ForeColor = Theme.Success;
            lblSummary.Text = thongDiep;
            return;
        }

        MessageBox.Show(FindForm(),
            string.IsNullOrWhiteSpace(loi) ? "Thao tác không thành công." : loi,
            "Không thực hiện được", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private async void DgvCourses_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        if (dgvCourses.Rows[e.RowIndex].Tag is not CourseListItemDto khoaHoc) return;

        using var dlg = new FormKhoaHoc(_levels, khoaHoc);
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            await ReloadAsync();
        }
    }
}
