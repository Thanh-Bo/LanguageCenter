using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcPhanCongGiangDay : UserControl
{
    private const string AllOption = "— Tất cả —";

    private IReadOnlyList<AssignmentListItemDto> _all = Array.Empty<AssignmentListItemDto>();
    private IReadOnlyList<ClassListItemDto> _classes = Array.Empty<ClassListItemDto>();
    private Button _btnKetThuc = null!;
    private bool _dangNap;

    public UcPhanCongGiangDay()
    {
        InitializeComponent();

        DamBaoCotBang();

        UI.UiHelpers.StyleGrid(dgvAssignments);

        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; ApplyFilter(); } };
        cboLopHoc.SelectedIndexChanged += (_, _) => { if (!_dangNap) ApplyFilter(); };
        cboVaiTro.SelectedIndexChanged += (_, _) => { if (!_dangNap) ApplyFilter(); };
        btnSearch.Click += (_, _) => ApplyFilter();
        btnRefresh.Click += async (_, _) => await ReloadAsync();
        btnAdd.Click += BtnAdd_Click;
        btnDelete.Click += BtnDelete_Click;

        // Nút "Kết thúc phân công" tạo tại runtime để không phải sửa Designer
        _btnKetThuc = UI.UiHelpers.CreateSecondaryButton("Kết thúc phân công", 150, 34);
        _btnKetThuc.Click += BtnKetThuc_Click;
        pnlToolbar.Controls.Add(_btnKetThuc);
        _btnKetThuc.Location = new Point(btnDelete.Left - _btnKetThuc.Width - 8, 3);

        pnlToolbar.Resize += (_, _) =>
        {
            btnDelete.Location = new Point(pnlToolbar.Width - btnDelete.Width - 4, 3);
            _btnKetThuc.Location = new Point(btnDelete.Left - _btnKetThuc.Width - 8, 3);
            btnAdd.Location = new Point(_btnKetThuc.Left - btnAdd.Width - 8, 3);
        };

        _dangNap = true;
        cboVaiTro.Items.Clear();
        cboVaiTro.Items.Add(AllOption);
        cboVaiTro.Items.AddRange(["Giáo viên chính", "Trợ giảng"]);
        cboVaiTro.SelectedIndex = 0;
        _dangNap = false;

        Load += async (_, _) => await ReloadAsync();
    }

    /// <summary>
    /// Bảo đảm <c>dgvAssignments</c> LUÔN có cột trước khi nạp dữ liệu.
    /// <para>
    /// <c>DataGridView.Rows.Add</c> ném InvalidOperationException
    /// ("No row can be added to a DataGridView control that does not have columns")
    /// khi <c>Columns.Count == 0</c>. Bình thường cột đã được khai báo trong
    /// <c>UcPhanCongGiangDay.Designer.cs</c>; hàm này chỉ dựng bổ sung khi thiếu
    /// (Designer bị lệch, hoặc chạy bản build cũ), nhờ vậy màn hình không sập.
    /// </para>
    /// </summary>
    private void DamBaoCotBang()
    {
        if (dgvAssignments.Columns.Count > 0)
        {
            return;
        }

        dgvAssignments.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaPhanCong", HeaderText = "Mã PC", FillWeight = 60 });
        dgvAssignments.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenLop", HeaderText = "Lớp học phụ trách", FillWeight = 160 });
        dgvAssignments.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaGiaoVien", HeaderText = "Mã GV/TG", FillWeight = 85 });
        dgvAssignments.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenGiaoVien", HeaderText = "Họ tên nhân sự", FillWeight = 150 });
        dgvAssignments.Columns.Add(new DataGridViewTextBoxColumn { Name = "LoaiNhanSu", HeaderText = "Phân loại", FillWeight = 90 });
        dgvAssignments.Columns.Add(new DataGridViewTextBoxColumn { Name = "VaiTroPhanCong", HeaderText = "Vai trò phân công", FillWeight = 120 });
        dgvAssignments.Columns.Add(new DataGridViewTextBoxColumn { Name = "NgayBatDau", HeaderText = "Ngày bắt đầu", FillWeight = 85 });
        dgvAssignments.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", FillWeight = 95 });
        dgvAssignments.Columns.Add(new DataGridViewTextBoxColumn { Name = "GhiChu", HeaderText = "Ghi chú nhiệm vụ", FillWeight = 180 });
    }

    /// <summary>Tải danh sách phân công + danh sách lớp từ API.</summary>
    private async Task ReloadAsync()
    {
        lblSummary.Text = "Đang tải dữ liệu phân công...";
        UseWaitCursor = true;

        var assignmentsTask = AppSession.Api.GetAsync<PagedResult<AssignmentListItemDto>>(
            QueryStringBuilder.Build("admin/teaching-assignments", ("page", "1"), ("pageSize", "500")));
        var classesTask = AppSession.Api.GetAsync<PagedResult<ClassListItemDto>>(
            QueryStringBuilder.Build("admin/classes", ("page", "1"), ("pageSize", "500")));

        await Task.WhenAll(assignmentsTask, classesTask);

        UseWaitCursor = false;

        _all = assignmentsTask.Result.Data?.Items ?? Array.Empty<AssignmentListItemDto>();
        _classes = classesTask.Result.Data?.Items ?? Array.Empty<ClassListItemDto>();

        if (!assignmentsTask.Result.Success)
        {
            lblSummary.Text = assignmentsTask.Result.ErrorMessage;
        }

        FillStats();
        InitFilterOptions();
        ApplyFilter();
    }

    private void FillStats()
    {
        var gvChinh = _all.Count(a => a.VaiTroPhanCong == "Giáo viên chính");
        var troGiang = _all.Count(a => a.VaiTroPhanCong == "Trợ giảng");
        var lopDaPhanCong = _all.Select(a => a.MaLopHoc).Distinct().Count();
        var tienDo = _classes.Count > 0
            ? Math.Round((decimal)lopDaPhanCong / _classes.Count * 100, 1)
            : 0m;

        tblStats.Controls.Clear();
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Tổng lượt phân công", $"{_all.Count} Phân công", "Học vụ đào tạo đang chạy", Theme.Primary), 0, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Giảng viên chính", $"{gvChinh} Giảng viên", "Đang trực tiếp đứng lớp", Theme.Success), 1, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Trợ giảng học vụ", $"{troGiang} Trợ giảng", "Hỗ trợ chấm bài & học thuật", Theme.Purple), 2, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Tiến độ giảng dạy", $"{tienDo}%", $"{lopDaPhanCong}/{_classes.Count} lớp đã có nhân sự", Theme.Warning), 3, 0);
    }

    private void InitFilterOptions()
    {
        _dangNap = true;

        var dangChon = cboLopHoc.SelectedItem?.ToString() ?? AllOption;

        cboLopHoc.Items.Clear();
        cboLopHoc.Items.Add(AllOption);
        foreach (var c in _classes.OrderBy(c => c.MaLop)) cboLopHoc.Items.Add(c.TenLop);

        var viTri = cboLopHoc.Items.IndexOf(dangChon);
        cboLopHoc.SelectedIndex = viTri >= 0 ? viTri : 0;

        _dangNap = false;
    }

    public void LoadAssignments()
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        var kw = txtSearch.Text.Trim();
        var lop = cboLopHoc.SelectedItem?.ToString() ?? AllOption;
        var vaiTro = cboVaiTro.SelectedItem?.ToString() ?? AllOption;

        var query = _all.AsEnumerable();

        if (lop != AllOption) query = query.Where(a => a.TenLop == lop);
        if (vaiTro != AllOption) query = query.Where(a => a.VaiTroPhanCong == vaiTro);

        if (!string.IsNullOrWhiteSpace(kw))
        {
            query = query.Where(a => a.TenGiaoVien.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                     a.MaGiaoVien.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                     a.TenLop.Contains(kw, StringComparison.OrdinalIgnoreCase));
        }

        var list = query.OrderBy(a => a.MaPhanCong).ToList();
        dgvAssignments.Rows.Clear();

        foreach (var a in list)
        {
            var rowIdx = dgvAssignments.Rows.Add(
                a.MaPhanCong,
                a.TenLop,
                a.MaGiaoVien,
                a.TenGiaoVien,
                a.LoaiGiaoVien,
                a.VaiTroPhanCong,
                UiHelpers.FormatDate(a.NgayBatDau),
                a.TrangThai,
                a.NgayKetThuc.HasValue ? $"Đến {UiHelpers.FormatDate(a.NgayKetThuc)}" : "—"
            );
            dgvAssignments.Rows[rowIdx].Cells["TrangThai"].Style.ForeColor = UiHelpers.StatusColor(a.TrangThai);
        }

        lblSummary.Text = $"Hiển thị {list.Count}/{_all.Count} lượt phân công nhân sự";
    }

    private async void BtnAdd_Click(object? sender, EventArgs e)
    {
        using var dlg = new FormSuaPhanCong();
        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            await ReloadAsync();
        }
    }

    private async void BtnKetThuc_Click(object? sender, EventArgs e)
    {
        if (dgvAssignments.CurrentRow == null)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn dòng phân công cần kết thúc.", "Chưa chọn dòng", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var idVal = dgvAssignments.CurrentRow.Cells["MaPhanCong"].Value;
        if (idVal is not int id && !(idVal is string s && int.TryParse(s, out id)))
        {
            return;
        }

        var teacherName = dgvAssignments.CurrentRow.Cells["TenGiaoVien"].Value?.ToString() ?? "nhân sự";
        var lopName = dgvAssignments.CurrentRow.Cells["TenLop"].Value?.ToString() ?? "lớp";

        var confirm = MessageBox.Show(
            FindForm(),
            $"Kết thúc phân công {teacherName} tại lớp {lopName}?\n"
            + "Sau khi kết thúc, có thể phân công lại cùng lớp/nhân sự/vai trò với ngày bắt đầu mới.",
            "Xác nhận kết thúc phân công",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        UseWaitCursor = true;

        var result = await AppSession.Api.PatchAsync<AssignmentListItemDto>(
            $"admin/teaching-assignments/{id}/status",
            new { trangThai = "Đã kết thúc" });

        UseWaitCursor = false;

        if (!result.Success)
        {
            MessageBox.Show(FindForm(), result.ErrorMessage, "Kết thúc phân công thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        await ReloadAsync();
    }

    private async void BtnDelete_Click(object? sender, EventArgs e)
    {
        if (dgvAssignments.CurrentRow == null)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn dòng phân công cần hủy.", "Chưa chọn dòng", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var idVal = dgvAssignments.CurrentRow.Cells["MaPhanCong"].Value;
        if (idVal is not int id && !(idVal is string s && int.TryParse(s, out id)))
        {
            return;
        }

        var teacherName = dgvAssignments.CurrentRow.Cells["TenGiaoVien"].Value?.ToString() ?? "nhân sự";
        var lopName = dgvAssignments.CurrentRow.Cells["TenLop"].Value?.ToString() ?? "lớp";

        var confirm = MessageBox.Show(
            FindForm(),
            $"Bạn có chắc chắn muốn hủy phân công {teacherName} tại lớp {lopName}?",
            "Xác nhận hủy phân công",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        UseWaitCursor = true;

        // Phase 4: huỷ phân công = đổi trạng thái sang "Đã hủy" (giữ lịch sử)
        var result = await AppSession.Api.PatchAsync<AssignmentListItemDto>(
            $"admin/teaching-assignments/{id}/status",
            new { trangThai = "Đã hủy" });

        UseWaitCursor = false;

        if (!result.Success)
        {
            MessageBox.Show(FindForm(), result.ErrorMessage, "Hủy phân công thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        await ReloadAsync();
    }
}

