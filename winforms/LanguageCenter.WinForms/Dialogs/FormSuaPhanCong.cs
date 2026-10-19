using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

public partial class FormSuaPhanCong : Form
{
    private IReadOnlyList<ClassListItemDto> _classes = Array.Empty<ClassListItemDto>();
    private IReadOnlyList<TeacherListItemDto> _teachers = Array.Empty<TeacherListItemDto>();

    public FormSuaPhanCong()
    {
        InitializeComponent();

        btnSave.DialogResult = DialogResult.None;
        btnSave.Click += BtnSave_Click;
        btnCancel.Click += (_, _) => DialogResult = DialogResult.Cancel;

        cboVaiTro.Items.Clear();
        cboVaiTro.Items.Add("Giáo viên chính");
        cboVaiTro.Items.Add("Trợ giảng");
        cboVaiTro.SelectedIndex = 0;

        dtpBatDau.Value = DateTime.Today;
        txtGhiChu.Text = "Giảng dạy theo đề cương chuẩn của trung tâm";

        Load += async (_, _) => await LoadDataAsync();
    }

    /// <summary>Nạp danh sách lớp và giáo viên/trợ giảng thật từ API.</summary>
    private async Task LoadDataAsync()
    {
        btnSave.Enabled = false;
        UseWaitCursor = true;

        var classesTask = AppSession.Api.GetAsync<PagedResult<ClassListItemDto>>(
            QueryStringBuilder.Build("admin/classes", ("page", "1"), ("pageSize", "500")));
        var teachersTask = AppSession.Api.GetAsync<PagedResult<TeacherListItemDto>>(
            QueryStringBuilder.Build("admin/teachers", ("page", "1"), ("pageSize", "500")));

        await Task.WhenAll(classesTask, teachersTask);

        UseWaitCursor = false;
        btnSave.Enabled = true;

        _classes = (classesTask.Result.Data?.Items ?? Array.Empty<ClassListItemDto>())
            .OrderBy(c => c.MaLop)
            .ToList();

        _teachers = (teachersTask.Result.Data?.Items ?? Array.Empty<TeacherListItemDto>())
            .OrderBy(t => t.MaGiaoVien)
            .ToList();

        cboLopHoc.Items.Clear();
        foreach (var c in _classes)
        {
            cboLopHoc.Items.Add($"{c.MaLop} — {c.TenLop}");
        }
        if (cboLopHoc.Items.Count > 0) cboLopHoc.SelectedIndex = 0;

        cboNhanSu.Items.Clear();
        foreach (var t in _teachers)
        {
            cboNhanSu.Items.Add($"{t.MaGiaoVien} - {t.HoTen} ({t.LoaiGiaoVien})");
        }
        if (cboNhanSu.Items.Count > 0) cboNhanSu.SelectedIndex = 0;

        if (!classesTask.Result.Success)
        {
            MessageBox.Show(this, classesTask.Result.ErrorMessage, "Không tải được lớp học", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        else if (!teachersTask.Result.Success)
        {
            MessageBox.Show(this, teachersTask.Result.ErrorMessage, "Không tải được giáo viên", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        if (cboLopHoc.SelectedIndex < 0 || cboLopHoc.SelectedIndex >= _classes.Count)
        {
            MessageBox.Show(this, "Vui lòng chọn lớp học.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cboNhanSu.SelectedIndex < 0 || cboNhanSu.SelectedIndex >= _teachers.Count)
        {
            MessageBox.Show(this, "Vui lòng chọn giáo viên / trợ giảng.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var lop = _classes[cboLopHoc.SelectedIndex];
        var nhanSu = _teachers[cboNhanSu.SelectedIndex];

        var ngayBatDau = DateOnly.FromDateTime(dtpBatDau.Value);
        DateOnly? ngayKetThuc = dtpKetThuc.Checked ? DateOnly.FromDateTime(dtpKetThuc.Value) : null;

        if (ngayKetThuc.HasValue && ngayKetThuc.Value < ngayBatDau)
        {
            MessageBox.Show(this, "Ngày kết thúc phải sau hoặc bằng ngày bắt đầu.", "Ngày không hợp lệ", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        btnSave.Enabled = false;
        UseWaitCursor = true;

        // Phase 4: tạo phân công thật trong DB (UNIQUE lớp + nhân sự + vai trò + khoảng ngày chồng lấn)
        var result = await AppSession.Api.PostAsync<AssignmentListItemDto>("admin/teaching-assignments", new
        {
            maLopHoc = lop.MaLopHoc,
            maGiaoVien = nhanSu.MaGiaoVien,
            vaiTroPhanCong = cboVaiTro.SelectedItem?.ToString() ?? "Giáo viên chính",
            ngayBatDau = ngayBatDau.ToString("yyyy-MM-dd"),
            ngayKetThuc = ngayKetThuc.HasValue ? ngayKetThuc.Value.ToString("yyyy-MM-dd") : null,
            ghiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? null : txtGhiChu.Text.Trim()
        });

        UseWaitCursor = false;
        btnSave.Enabled = true;

        if (!result.Success)
        {
            MessageBox.Show(this, result.ErrorMessage, "Phân công thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        DialogResult = DialogResult.OK;
    }
}

