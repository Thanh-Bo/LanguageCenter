using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcLichHoc : UserControl
{
    private const string AllOption = "— Tất cả —";
    private int _currentTab = 0; // 0: Phòng học, 1: Lịch học tuần, 2: Ca học thực tế

    private IReadOnlyList<RoomListItemDto> _rooms = Array.Empty<RoomListItemDto>();
    private IReadOnlyList<ScheduleListItemDto> _schedules = Array.Empty<ScheduleListItemDto>();
    private IReadOnlyList<SessionListItemDto> _sessions = Array.Empty<SessionListItemDto>();
    private IReadOnlyList<ClassListItemDto> _classes = Array.Empty<ClassListItemDto>();
    private IReadOnlyList<TeacherListItemDto> _teachers = Array.Empty<TeacherListItemDto>();
    private ReportOverviewDto? _overview;
    private bool _dangNap;

    private Button _btnDiemDanh = null!;
    private Button _btnTrangThai = null!;
    private Button _btnSinhCa = null!;

    public UcLichHoc()
    {
        InitializeComponent();

        DamBaoCotBang();

        UI.UiHelpers.StyleGrid(dgvRooms);
        UI.UiHelpers.StyleGrid(dgvSchedules);
        UI.UiHelpers.StyleGrid(dgvShifts);

        dgvSchedules.CellDoubleClick += DgvSchedules_CellDoubleClick;
        dgvShifts.CellDoubleClick += DgvShifts_CellDoubleClick;

        btnTabRooms.Click += (_, _) => SwitchTab(0);
        btnTabSchedules.Click += (_, _) => SwitchTab(1);
        btnTabShifts.Click += (_, _) => SwitchTab(2);

        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; ApplyFilter(); } };
        cboFilter.SelectedIndexChanged += (_, _) => { if (!_dangNap) ApplyFilter(); };
        btnSearch.Click += (_, _) => ApplyFilter();
        btnRefresh.Click += async (_, _) => await ReloadAsync();
        btnAdd.Click += BtnAdd_Click;

        _btnDiemDanh = UiHelpers.CreatePrimaryButton("Điểm danh ca học", 170, 34);
        _btnDiemDanh.Click += BtnDiemDanh_Click;
        _btnDiemDanh.Visible = false;
        pnlToolbar.Controls.Add(_btnDiemDanh);

        _btnTrangThai = UiHelpers.CreateSecondaryButton("Đổi trạng thái", 130, 34);
        _btnTrangThai.Click += BtnTrangThai_Click;
        _btnTrangThai.Visible = false;
        pnlToolbar.Controls.Add(_btnTrangThai);

        // Sinh ca học hàng loạt từ lịch định kỳ (chỉ ở tab Lịch học).
        _btnSinhCa = UiHelpers.CreateSecondaryButton("Sinh ca học", 140, 34);
        _btnSinhCa.Click += BtnSinhCa_Click;
        _btnSinhCa.Visible = false;
        pnlToolbar.Controls.Add(_btnSinhCa);

        DungThanhCongCu();

        // Tạo phòng/lịch/ca học thực hiện trên bản Web; riêng lịch và ca học đã hỗ trợ tại đây.
        btnAdd.Visible = true;

        SwitchTab(0);
        Load += async (_, _) => await ReloadAsync();
    }

    /// <summary>
    /// Bảo đảm CẢ 3 bảng LUÔN có cột trước khi thêm dòng.
    /// <para>
    /// <c>DataGridView.Rows.Add</c> ném InvalidOperationException
    /// ("No row can be added to a DataGridView control that does not have columns")
    /// khi <c>Columns.Count == 0</c>. Cột đã khai báo trong Designer, nhưng nếu vì lý do nào đó
    /// (Designer lệch, build cũ, designer không nạp) mà mất thì hàm này dựng lại — màn hình
    /// không sập. Hàm idempotent: đã có cột thì bỏ qua.
    /// </para>
    /// </summary>
    private void DamBaoCotBang()
    {
        UiHelpers.DamBaoCot(dgvRooms,
            ("MaPhong", "Mã phòng", 85),
            ("TenPhong", "Tên phòng học", 160),
            ("ViTri", "Vị trí", 140),
            ("SucChua", "Sức chứa", 80),
            ("ThietBi", "Trang thiết bị chuyên dụng", 240),
            ("TrangThai", "Trạng thái", 95));

        UiHelpers.DamBaoCot(dgvSchedules,
            ("MaLop", "Mã lớp", 85),
            ("TenLop", "Lớp học", 160),
            ("TenPhong", "Phòng học", 90),
            ("ThuText", "Thứ học trong tuần", 170),
            ("KhungGio", "Khung giờ học", 120),
            ("NgayApDungTu", "Ngày áp dụng", 90),
            ("TrangThai", "Trạng thái", 95));

        UiHelpers.DamBaoCot(dgvShifts,
            ("MaCaHoc", "Mã ca", 60),
            ("NgayHoc", "Ngày học", 85),
            ("TenLop", "Lớp học", 150),
            ("TenPhong", "Phòng học", 85),
            ("GiaoVien", "Giáo viên giảng dạy", 130),
            ("TroGiang", "Trợ giảng", 110),
            ("GioHoc", "Thời gian", 110),
            ("NoiDung", "Nội dung bài học", 200),
            ("TrangThai", "Trạng thái", 95));
    }

    /// <summary>
    /// Chia thanh công cụ thành 2 nhóm tự co giãn: bộ lọc neo trái, các nút hành động neo phải.
    /// Dùng FlowLayoutPanel để không phải tính toạ độ thủ công (tránh các nút đè lên nhau).
    /// </summary>
    private void DungThanhCongCu()
    {
        var flpLoc = new FlowLayoutPanel
        {
            Dock = DockStyle.Left,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            FlowDirection = FlowDirection.LeftToRight,
            Padding = new Padding(0, 2, 0, 0)
        };

        txtSearch.Margin = new Padding(0, 4, 10, 0);
        cboFilter.Margin = new Padding(0, 3, 10, 0);
        btnSearch.Margin = new Padding(0, 0, 10, 0);
        btnRefresh.Margin = new Padding(0, 0, 10, 0);

        flpLoc.Controls.Add(txtSearch);
        flpLoc.Controls.Add(cboFilter);
        flpLoc.Controls.Add(btnSearch);
        flpLoc.Controls.Add(btnRefresh);

        var flpHanhDong = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            WrapContents = false,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 2, 4, 0)
        };

        foreach (var button in new[] { btnAdd, _btnSinhCa, _btnTrangThai, _btnDiemDanh })
        {
            button.Margin = new Padding(8, 0, 0, 0);
        }

        // Thứ tự Add = thứ tự từ PHẢI sang TRÁI.
        flpHanhDong.Controls.Add(btnAdd);
        flpHanhDong.Controls.Add(_btnSinhCa);
        flpHanhDong.Controls.Add(_btnTrangThai);
        flpHanhDong.Controls.Add(_btnDiemDanh);

        pnlToolbar.Controls.Add(flpLoc);
        pnlToolbar.Controls.Add(flpHanhDong);
    }

    /// <summary>Tải song song phòng học, lịch học tuần, ca học và số liệu tổng quan từ API.</summary>
    private async Task ReloadAsync()
    {
        lblSummary.Text = "Đang tải dữ liệu lịch học...";
        UseWaitCursor = true;

        var roomsTask = AppSession.Api.GetAsync<PagedResult<RoomListItemDto>>(
            QueryStringBuilder.Build("admin/rooms", ("page", "1"), ("pageSize", "500")));
        var schedulesTask = AppSession.Api.GetAsync<PagedResult<ScheduleListItemDto>>(
            QueryStringBuilder.Build("admin/schedules", ("page", "1"), ("pageSize", "500")));
        var sessionsTask = AppSession.Api.GetAsync<PagedResult<SessionListItemDto>>(
            QueryStringBuilder.Build("admin/sessions", ("page", "1"), ("pageSize", "500")));
        var classesTask = AppSession.Api.GetAsync<PagedResult<ClassListItemDto>>(
            QueryStringBuilder.Build("admin/classes", ("page", "1"), ("pageSize", "500")));
        var teachersTask = AppSession.Api.GetAsync<PagedResult<TeacherListItemDto>>(
            QueryStringBuilder.Build("admin/teachers", ("page", "1"), ("pageSize", "300")));
        var overviewTask = AppSession.Api.GetAsync<ReportOverviewDto>("admin/reports/overview");

        await Task.WhenAll(roomsTask, schedulesTask, sessionsTask, classesTask, teachersTask, overviewTask);

        UseWaitCursor = false;

        _rooms = roomsTask.Result.Data?.Items ?? Array.Empty<RoomListItemDto>();
        _schedules = schedulesTask.Result.Data?.Items ?? Array.Empty<ScheduleListItemDto>();
        _sessions = sessionsTask.Result.Data?.Items ?? Array.Empty<SessionListItemDto>();
        _classes = classesTask.Result.Data?.Items ?? Array.Empty<ClassListItemDto>();
        _teachers = teachersTask.Result.Data?.Items ?? Array.Empty<TeacherListItemDto>();
        _overview = overviewTask.Result.Data;

        var loiDau =
            (roomsTask.Result.Success ? null : roomsTask.Result.ErrorMessage)
            ?? (schedulesTask.Result.Success ? null : schedulesTask.Result.ErrorMessage)
            ?? (sessionsTask.Result.Success ? null : sessionsTask.Result.ErrorMessage);

        if (loiDau is not null)
        {
            lblSummary.Text = loiDau;
        }

        FillStats();
        FillFilterOptions();
        ApplyFilter();
    }

    private void FillStats()
    {
        var usingRooms = _rooms.Count(r => r.TrangThai == "Đang sử dụng");
        var tongCaHoc = _overview?.CaHoc.TongSo ?? _sessions.Count;

        tblStats.Controls.Clear();
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Tổng số phòng học", $"{_rooms.Count} Phòng", "Các phòng chuẩn âm thanh & máy lạnh", Theme.Primary), 0, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Đang sử dụng", $"{usingRooms} Phòng", "Các lớp đang học trong ca", Theme.Success), 1, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Lịch học tuần", $"{_schedules.Count} Thời khóa biểu", "Lịch học cố định theo lớp", Theme.Purple), 2, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Ca học thực tế", $"{tongCaHoc} Buổi học", "Tiến độ bài học đang diễn ra", Theme.Warning), 3, 0);
    }

    private void SwitchTab(int tabIndex)
    {
        _currentTab = tabIndex;

        btnTabRooms.BackColor = _currentTab == 0 ? Theme.Primary : Color.White;
        btnTabRooms.ForeColor = _currentTab == 0 ? Color.White : Theme.TextDark;
        btnTabRooms.Font = _currentTab == 0 ? Theme.FontBold : Theme.FontRegular;

        btnTabSchedules.BackColor = _currentTab == 1 ? Theme.Primary : Color.White;
        btnTabSchedules.ForeColor = _currentTab == 1 ? Color.White : Theme.TextDark;
        btnTabSchedules.Font = _currentTab == 1 ? Theme.FontBold : Theme.FontRegular;

        btnTabShifts.BackColor = _currentTab == 2 ? Theme.Primary : Color.White;
        btnTabShifts.ForeColor = _currentTab == 2 ? Color.White : Theme.TextDark;
        btnTabShifts.Font = _currentTab == 2 ? Theme.FontBold : Theme.FontRegular;

        dgvRooms.Visible = _currentTab == 0;
        dgvSchedules.Visible = _currentTab == 1;
        dgvShifts.Visible = _currentTab == 2;

        if (_btnDiemDanh is not null) _btnDiemDanh.Visible = _currentTab == 2;
        if (_btnTrangThai is not null) _btnTrangThai.Visible = _currentTab != 0;
        if (_btnTrangThai is not null) _btnTrangThai.Text = _currentTab == 2 ? "Trạng thái ca" : "Trạng thái lịch";
        if (_btnSinhCa is not null) _btnSinhCa.Visible = _currentTab == 1;

        btnAdd.Text = _currentTab switch
        {
            0 => "Thêm phòng học",
            1 => "Xếp lịch học",
            2 => "Thêm ca học",
            _ => "Thêm mới"
        };

        FillFilterOptions();
        ApplyFilter();
    }

    /// <summary>Nạp lại danh mục lọc theo tab đang chọn (dữ liệu thật từ API).</summary>
    private void FillFilterOptions()
    {
        _dangNap = true;

        var dangChon = cboFilter.SelectedItem?.ToString() ?? AllOption;

        cboFilter.Items.Clear();
        cboFilter.Items.Add(AllOption);

        IEnumerable<string> muc = _currentTab switch
        {
            0 => _rooms.Select(r => r.TrangThai),
            1 => _schedules.Select(s => s.TenLop),
            _ => _sessions.Select(s => s.TenLop)
        };

        foreach (var item in muc.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x))
        {
            cboFilter.Items.Add(item);
        }

        var viTri = cboFilter.Items.IndexOf(dangChon);
        cboFilter.SelectedIndex = viTri >= 0 ? viTri : 0;

        _dangNap = false;
    }

    private void ApplyFilter()
    {
        // Chốt chặn cuối: không bao giờ thêm dòng vào bảng chưa có cột.
        DamBaoCotBang();

        var kw = txtSearch.Text.Trim();
        var filter = cboFilter.SelectedItem?.ToString() ?? AllOption;

        if (_currentTab == 0)
        {
            var query = _rooms.AsEnumerable();
            if (filter != AllOption) query = query.Where(r => r.TrangThai == filter);
            if (!string.IsNullOrWhiteSpace(kw))
            {
                query = query.Where(r => r.TenPhong.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                         r.MaPhong.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                         (r.ThietBi != null && r.ThietBi.Contains(kw, StringComparison.OrdinalIgnoreCase)));
            }

            var list = query.OrderBy(r => r.MaPhong).ToList();
            dgvRooms.Rows.Clear();
            foreach (var r in list)
            {
                var rowIdx = dgvRooms.Rows.Add(
                    r.MaPhong,
                    r.TenPhong,
                    r.ViTri ?? "—",
                    $"{r.SucChua} chỗ",
                    r.ThietBi ?? "—",
                    r.TrangThai
                );
                dgvRooms.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = UiHelpers.StatusColor(r.TrangThai);
            }
            lblSummary.Text = $"Hiển thị {list.Count}/{_rooms.Count} phòng học phù hợp";
        }
        else if (_currentTab == 1)
        {
            var query = _schedules.AsEnumerable();
            if (filter != AllOption) query = query.Where(s => s.TenLop == filter);
            if (!string.IsNullOrWhiteSpace(kw))
            {
                query = query.Where(s => s.TenLop.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                         s.TenPhong.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                         ThuText(s.ThuTrongTuan).Contains(kw, StringComparison.OrdinalIgnoreCase));
            }

            var list = query.OrderBy(s => s.TenLop).ToList();
            dgvSchedules.Rows.Clear();
            foreach (var s in list)
            {
                var rowIdx = dgvSchedules.Rows.Add(
                    s.MaLop,
                    s.TenLop,
                    s.TenPhong,
                    ThuText(s.ThuTrongTuan),
                    $"{s.GioBatDau:HH\\:mm} - {s.GioKetThuc:HH\\:mm}",
                    UiHelpers.FormatDate(s.NgayApDungTu),
                    s.TrangThai
                );
                dgvSchedules.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = UiHelpers.StatusColor(s.TrangThai);
                dgvSchedules.Rows[rowIdx].Tag = s;
            }
            lblSummary.Text = $"Hiển thị {list.Count}/{_schedules.Count} thời khóa biểu tuần";
        }
        else
        {
            var query = _sessions.AsEnumerable();
            if (filter != AllOption) query = query.Where(s => s.TenLop == filter);
            if (!string.IsNullOrWhiteSpace(kw))
            {
                query = query.Where(s => s.TenLop.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                         s.TenPhong.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                         (s.NoiDungBaiHoc != null && s.NoiDungBaiHoc.Contains(kw, StringComparison.OrdinalIgnoreCase)) ||
                                         (s.TenGiaoVienThucTe != null && s.TenGiaoVienThucTe.Contains(kw, StringComparison.OrdinalIgnoreCase)));
            }

            var list = query.OrderByDescending(s => s.NgayHoc).ToList();
            dgvShifts.Rows.Clear();
            foreach (var s in list)
            {
                var rowIdx = dgvShifts.Rows.Add(
                    s.MaCaHoc,
                    UiHelpers.FormatDate(s.NgayHoc),
                    s.TenLop,
                    s.TenPhong,
                    s.TenGiaoVienThucTe ?? "—",
                    s.TenTroGiangThucTe ?? "—",
                    $"{s.GioBatDau:HH\\:mm} - {s.GioKetThuc:HH\\:mm}",
                    s.NoiDungBaiHoc ?? "—",
                    s.TrangThai
                );
                dgvShifts.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = UiHelpers.StatusColor(s.TrangThai);
                dgvShifts.Rows[rowIdx].Tag = s;
            }
            lblSummary.Text = $"Hiển thị {list.Count}/{_sessions.Count} ca học thực tế";
        }
    }

    private static string ThuText(int thuTrongTuan) => thuTrongTuan switch
    {
        1 => "Thứ 2",
        2 => "Thứ 3",
        3 => "Thứ 4",
        4 => "Thứ 5",
        5 => "Thứ 6",
        6 => "Thứ 7",
        7 => "Chủ nhật",
        _ => "—"
    };

    /// <summary>
    /// Sinh ca học hàng loạt từ lịch định kỳ đang chọn — <c>POST /api/admin/sessions/generate</c>.
    /// <para>
    /// Backend chỉ tạo ca cho các ngày khớp <c>ThuTrongTuan</c> nằm trong khoảng
    /// [Từ ngày, Đến ngày] giao với khoảng áp dụng của lịch; ngày đã có ca thì bỏ qua,
    /// ngày trùng phòng/giáo viên thì trả về danh sách xung đột kèm lý do.
    /// </para>
    /// </summary>
    private async void BtnSinhCa_Click(object? sender, EventArgs e)
    {
        if (dgvSchedules.CurrentRow?.Tag is not ScheduleListItemDto lich)
        {
            MessageBox.Show(FindForm(),
                "Vui lòng chọn một lịch học trong bảng (tab 'Lịch học') để sinh ca.",
                "Chưa chọn lịch học", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new FormSinhCaHoc(lich);

        if (dlg.ShowDialog(FindForm()) != DialogResult.OK)
        {
            return;
        }

        UseWaitCursor = true;

        var result = await AppSession.Api.PostAsync<GenerateSessionsResponseDto>(
            "admin/sessions/generate",
            new
            {
                maLichHoc = lich.MaLichHoc,
                tuNgay = dlg.TuNgay.ToString("yyyy-MM-dd"),
                denNgay = dlg.DenNgay.ToString("yyyy-MM-dd")
            });

        UseWaitCursor = false;

        if (!result.Success || result.Data is null)
        {
            MessageBox.Show(FindForm(),
                string.IsNullOrWhiteSpace(result.ErrorMessage) ? "Không sinh được ca học." : result.ErrorMessage,
                "Sinh ca học thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var data = result.Data;

        var thongBao = new List<string>
        {
            $"Lớp: {lich.TenLop}",
            $"Khoảng áp dụng thực tế: {UiHelpers.FormatDate(data.ApDungTuThucTe)} → {UiHelpers.FormatDate(data.ApDungDenThucTe)}",
            $"Số ngày khớp thứ trong tuần: {data.TongNgayKhop}",
            string.Empty,
            $"• Tạo mới: {data.NgayTao.Count} ca",
            $"• Đã có sẵn (bỏ qua): {data.NgayDaCo.Count} ca",
            $"• Xung đột: {data.NgayXungDot.Count} ca"
        };

        foreach (var xungDot in data.NgayXungDot.Take(10))
        {
            thongBao.Add($"    - {UiHelpers.FormatDate(xungDot.NgayHoc)}: {xungDot.LyDo}");
        }

        if (data.NgayXungDot.Count > 10)
        {
            thongBao.Add($"    … và {data.NgayXungDot.Count - 10} ngày khác");
        }

        MessageBox.Show(FindForm(), string.Join(Environment.NewLine, thongBao),
            "Kết quả sinh ca học", MessageBoxButtons.OK,
            data.NgayXungDot.Count > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);

        await ReloadAsync();
    }

    private void BtnTrangThai_Click(object? sender, EventArgs e)
    {
        var menu = new ContextMenuStrip { Font = Theme.FontRegular };

        if (_currentTab == 2)
        {
            if (dgvShifts.CurrentRow?.Tag is not SessionListItemDto caHoc)
            {
                MessageBox.Show(FindForm(), "Vui lòng chọn một ca học trong danh sách.",
                    "Chưa chọn ca học", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (var trangThai in new[] { "Chưa diễn ra", "Đang diễn ra", "Đã hoàn thành", "Đã hủy" })
            {
                var item = menu.Items.Add($"Ca #{caHoc.MaCaHoc} → {trangThai}");
                item.Enabled = trangThai != caHoc.TrangThai;
                item.Click += async (_, _) => await DoiTrangThaiCaAsync(caHoc, trangThai);
            }
        }
        else if (_currentTab == 1)
        {
            if (dgvSchedules.CurrentRow?.Tag is not ScheduleListItemDto lich)
            {
                MessageBox.Show(FindForm(), "Vui lòng chọn một lịch học trong danh sách.",
                    "Chưa chọn lịch", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            foreach (var trangThai in new[] { "Hoạt động", "Ngừng áp dụng" })
            {
                var item = menu.Items.Add($"{lich.MaLop} → {trangThai}");
                item.Enabled = trangThai != lich.TrangThai;
                item.Click += async (_, _) => await DoiTrangThaiLichAsync(lich, trangThai);
            }
        }
        else
        {
            return;
        }

        menu.Show(_btnTrangThai, new Point(0, _btnTrangThai.Height));
    }

    private async Task DoiTrangThaiCaAsync(SessionListItemDto caHoc, string trangThai)
    {
        var result = await AppSession.Api.PatchAsync<SessionListItemDto>(
            $"admin/sessions/{caHoc.MaCaHoc}/status", new { trangThai });

        if (!result.Success)
        {
            MessageBox.Show(FindForm(),
                string.IsNullOrWhiteSpace(result.ErrorMessage) ? "Không đổi được trạng thái ca học." : result.ErrorMessage,
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        await ReloadAsync();
    }

    private async Task DoiTrangThaiLichAsync(ScheduleListItemDto lich, string trangThai)
    {
        var result = await AppSession.Api.PatchAsync<ScheduleListItemDto>(
            $"admin/schedules/{lich.MaLichHoc}/status", new { trangThai });

        if (!result.Success)
        {
            MessageBox.Show(FindForm(),
                string.IsNullOrWhiteSpace(result.ErrorMessage) ? "Không đổi được trạng thái lịch học." : result.ErrorMessage,
                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        await ReloadAsync();
    }

    private async void BtnDiemDanh_Click(object? sender, EventArgs e)
    {
        if (dgvShifts.CurrentRow?.Tag is not SessionListItemDto caHoc)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn một ca học trong danh sách.",
                "Chưa chọn ca học", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dlg = new FormDiemDanh(caHoc.MaCaHoc);

        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            await ReloadAsync();
        }
    }

    private async void BtnAdd_Click(object? sender, EventArgs e)
    {
        if (_currentTab == 0)
        {
            using var dlgPhong = new FormPhongHoc();

            if (dlgPhong.ShowDialog(FindForm()) == DialogResult.OK)
            {
                await ReloadAsync();

                if (dlgPhong.PhongVuaTao is { } phong)
                {
                    MessageBox.Show(FindForm(),
                        $"Đã thêm phòng '{phong.MaPhong}' — {phong.TenPhong} (sức chứa {phong.SucChua}).\n\n"
                        + "Phòng mới xuất hiện trong danh sách phòng và trong ô chọn phòng khi xếp lịch / tạo ca học.",
                        "Thêm phòng học thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            return;
        }

        if (_currentTab == 1)
        {
            using var dlg = new FormLichHoc(_classes, _rooms);
            if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
            {
                await ReloadAsync();
            }
            return;
        }

        if (_currentTab == 2)
        {
            using var dlg = new FormCaHoc(_classes, _rooms, _schedules, _teachers);
            if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
            {
                await ReloadAsync();
            }
            return;
        }
    }

    private async void DgvSchedules_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        if (dgvSchedules.Rows[e.RowIndex].Tag is not ScheduleListItemDto lich) return;

        using var dlg = new FormLichHoc(_classes, _rooms, lich);
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            await ReloadAsync();
        }
    }

    private async void DgvShifts_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        if (dgvShifts.Rows[e.RowIndex].Tag is not SessionListItemDto caHoc) return;

        using var dlg = new FormCaHoc(_classes, _rooms, _schedules, _teachers, caHoc);
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            await ReloadAsync();
        }
    }
}

