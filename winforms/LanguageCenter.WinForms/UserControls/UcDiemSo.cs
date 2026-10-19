using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.Dialogs;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

public partial class UcDiemSo : UserControl
{
    private const string AllOption = "— Tất cả —";

    private IReadOnlyList<ClassListItemDto> _classes = Array.Empty<ClassListItemDto>();
    private IReadOnlyList<TeacherListItemDto> _teachers = Array.Empty<TeacherListItemDto>();
    private AcademicReportDto? _report;
    private bool _dangNap;

    public UcDiemSo()
    {
        InitializeComponent();

        UI.UiHelpers.StyleGrid(dgvGrades);
        dgvGrades.ReadOnly = true;

        cboLopHoc.SelectedIndexChanged += (_, _) => { if (!_dangNap) _ = LoadGradesAsync(); };
        btnRefresh.Click += async (_, _) => await ReloadAsync();
        btnExport.Click += BtnExport_Click;

        // Điểm tổng kết do backend tính theo trọng số; WinForms không cần nút tính lại.
        btnCalculate.Visible = false;

        // Nút "Lưu bảng điểm" cũ được dùng lại thành "Nhập điểm".
        btnSave.Text = "Nhập điểm";
        btnSave.Visible = true;
        btnSave.Click += BtnNhapDiem_Click;

        Load += async (_, _) => await ReloadAsync();
    }

    private async Task ReloadAsync()
    {
        await LoadClassesAsync();
        await LoadTeachersAsync();
        await LoadGradesAsync();
    }

    private async Task LoadTeachersAsync()
    {
        var result = await AppSession.Api.GetAsync<PagedResult<TeacherListItemDto>>(
            QueryStringBuilder.Build("admin/teachers", ("page", "1"), ("pageSize", "300")));

        _teachers = result.Data?.Items ?? Array.Empty<TeacherListItemDto>();
    }

    private async Task LoadClassesAsync()
    {
        var result = await AppSession.Api.GetAsync<PagedResult<ClassListItemDto>>(
            QueryStringBuilder.Build("admin/classes", ("page", "1"), ("pageSize", "500")));

        _classes = result.Data?.Items ?? Array.Empty<ClassListItemDto>();

        _dangNap = true;
        var dangChon = cboLopHoc.SelectedItem?.ToString();

        cboLopHoc.Items.Clear();
        cboLopHoc.Items.Add(AllOption);

        foreach (var c in _classes.OrderBy(c => c.MaLop))
        {
            cboLopHoc.Items.Add($"{c.MaLop} — {c.TenLop}");
        }

        var viTri = dangChon is null ? -1 : cboLopHoc.Items.IndexOf(dangChon);
        cboLopHoc.SelectedIndex = viTri >= 0 ? viTri : (cboLopHoc.Items.Count > 1 ? 1 : 0);
        _dangNap = false;
    }

    private async Task LoadGradesAsync()
    {
        var selected = cboLopHoc.SelectedItem?.ToString();

        var lop = selected is null || selected == AllOption
            ? null
            : _classes.FirstOrDefault(c => $"{c.MaLop} — {c.TenLop}" == selected);

        if (lop is null)
        {
            _report = null;
            dgvGrades.Rows.Clear();
            UpdateStats(null);
            lblSummary.Text = "Vui lòng chọn một lớp học để xem bảng điểm.";
            return;
        }

        lblSummary.Text = "Đang tải bảng điểm...";
        UseWaitCursor = true;

        var result = await AppSession.Api.GetAsync<AcademicReportDto>(
            QueryStringBuilder.Build("admin/reports/academic", ("maLopHoc", lop.MaLopHoc.ToString())));

        UseWaitCursor = false;

        if (!result.Success || result.Data is null)
        {
            _report = null;
            dgvGrades.Rows.Clear();
            UpdateStats(null);
            lblSummary.Text = result.ErrorMessage;
            return;
        }

        _report = result.Data;
        FillGrid(_report);
        UpdateStats(_report);
    }

    private void FillGrid(AcademicReportDto report)
    {
        RebuildColumns(report.ThanhPhan);

        dgvGrades.Rows.Clear();

        foreach (var hv in report.HocViens)
        {
            var values = new List<object> { hv.MaHocVien, hv.HoTen };

            foreach (var tp in report.ThanhPhan)
            {
                var diem = hv.ThanhPhan.FirstOrDefault(x => x.MaLoaiDiem == tp.MaLoaiDiem)?.Diem;
                values.Add(diem.HasValue ? diem.Value.ToString("0.0") : "—");
            }

            values.Add(hv.DiemTongKet.HasValue ? hv.DiemTongKet.Value.ToString("0.0") : "—");
            values.Add(hv.DuDieuKien ? "Đủ điều kiện" : "Thiếu điểm");

            var rowIdx = dgvGrades.Rows.Add(values.ToArray());
            var row = dgvGrades.Rows[rowIdx];
            row.Tag = hv;

            row.Cells["DiemTongKet"].Style.Font = Theme.FontBold;
            row.Cells["KetQua"].Style.ForeColor = hv.DuDieuKien ? Theme.Success : Theme.Warning;
            row.Cells["KetQua"].Style.Font = Theme.FontBold;
        }

        lblSummary.Text = report.CanhBaoTrongSo
            ? $"Bảng điểm {report.SoHocVien} học viên — CẢNH BÁO: tổng trọng số = {report.TongTrongSo}%"
            : $"Bảng điểm {report.SoHocVien} học viên — tổng trọng số {report.TongTrongSo}%";
    }

    /// <summary>Mở hộp thoại nhập điểm cho học viên đang chọn.</summary>
    private async void BtnNhapDiem_Click(object? sender, EventArgs e)
    {
        if (_report is null || dgvGrades.CurrentRow?.Tag is not AcademicStudentRowDto hocVien)
        {
            MessageBox.Show(FindForm(),
                "Vui lòng chọn một lớp và một học viên trong bảng điểm.",
                "Chưa chọn học viên", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var diemDaCo = await AppSession.Api.GetAsync<PagedResult<GradeRowDto>>(
            QueryStringBuilder.Build(
                "admin/grades",
                ("maLopHoc", _report.MaLopHoc.ToString()),
                ("page", "1"),
                ("pageSize", "1000")));

        var map = new Dictionary<int, int>();

        if (diemDaCo.Success && diemDaCo.Data is not null)
        {
            foreach (var g in diemDaCo.Data.Items.Where(x => x.MaDangKy == hocVien.MaDangKy))
            {
                map[g.MaLoaiDiem] = g.MaDiemSo;
            }
        }

        using var dlg = new FormNhapDiem(hocVien, _report.ThanhPhan, map, _teachers);

        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
        {
            await LoadGradesAsync();
        }
    }

    /// <summary>Dựng lại cột điểm theo đúng thành phần điểm mà backend trả về.</summary>
    private void RebuildColumns(IReadOnlyList<GradeComponentDto> thanhPhan)
    {
        dgvGrades.Columns.Clear();
        dgvGrades.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaHocVien", HeaderText = "Mã HV", FillWeight = 70 });
        dgvGrades.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenHocVien", HeaderText = "Họ và tên", FillWeight = 160 });

        foreach (var tp in thanhPhan)
        {
            dgvGrades.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = $"TP{tp.MaLoaiDiem}",
                HeaderText = $"{tp.TenLoaiDiem} ({tp.TrongSo}%)",
                FillWeight = 90
            });
        }

        dgvGrades.Columns.Add(new DataGridViewTextBoxColumn { Name = "DiemTongKet", HeaderText = "Tổng kết", FillWeight = 85 });
        dgvGrades.Columns.Add(new DataGridViewTextBoxColumn { Name = "KetQua", HeaderText = "Kết quả", FillWeight = 120 });
    }

    private void UpdateStats(AcademicReportDto? report)
    {
        tblStats.Controls.Clear();

        if (report is null || report.SoHocVien == 0)
        {
            tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Sĩ số lớp", "0 HV", "Chưa có học viên", Theme.Primary), 0, 0);
            tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Điểm TB lớp", "—", "Chưa có điểm", Theme.Success), 1, 0);
            tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Đủ điều kiện", "0 HV", "Tiêu chuẩn đầu ra", Theme.Warning), 2, 0);
            tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Điểm cao nhất", "—", "Học viên xuất sắc", Theme.Purple), 3, 0);
            return;
        }

        var tiLe = Math.Round((decimal)report.SoDuDieuKien / report.SoHocVien * 100, 1);

        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Sĩ số lớp", $"{report.SoHocVien} Học viên", "Đang theo học lớp", Theme.Primary), 0, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Điểm TB lớp", report.DiemTongKetTrungBinh?.ToString("0.00") ?? "—", "Thang điểm 10 chuẩn đào tạo", Theme.Success), 1, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Đủ điều kiện", $"{report.SoDuDieuKien}/{report.SoHocVien}", $"{tiLe}% học viên đủ đầu điểm", Theme.Warning), 2, 0);
        tblStats.Controls.Add(UI.UiHelpers.CreateStatCard("Điểm cao nhất", report.DiemCaoNhat?.ToString("0.00") ?? "—", $"Thấp nhất: {report.DiemThapNhat?.ToString("0.00") ?? "—"}", Theme.Purple), 3, 0);
    }

    private void BtnExport_Click(object? sender, EventArgs e)
    {
        MessageBox.Show(
            FindForm(),
            $"Bảng điểm lớp {_report?.TenLop ?? "—"} gồm {dgvGrades.Rows.Count} học viên.\n"
            + "Chức năng xuất Excel/PDF được thực hiện trên bản Web (Báo cáo → Kết quả học tập).",
            "Xuất bảng điểm",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void ReloadData()
    {
        _ = ReloadAsync();
    }
}

