using LanguageCenter.WinForms.Api;

namespace LanguageCenter.WinForms.Dialogs;

/// Thêm mới hoặc sửa hồ sơ học viên.
/// Constructor không tham số → chế độ TẠO MỚI (hiện khối phụ huynh có thể nhập).
/// Constructor(studentId) → chế độ SỬA (hiện khối trạng thái học tập, phụ huynh chỉ đọc —
/// backend Phase 3 chưa có endpoint cập nhật phụ huynh).
public partial class FormSuaHocVien : Form
{
    private readonly string? _studentId;
    private readonly bool _isEditMode;

    public bool DataChanged { get; private set; }

    /// <summary>Chế độ tạo mới.</summary>
    public FormSuaHocVien()
    {
        _isEditMode = false;
        InitializeComponent();
        WireEvents();
        ConfigureForCreateMode();
    }

    /// Chế độ sửa.
    public FormSuaHocVien(string studentId)
    {
        _studentId = studentId;
        _isEditMode = true;
        InitializeComponent();
        WireEvents();
        ConfigureForEditMode();
    }

    private void WireEvents()
    {
        chkNgayNhapHoc.CheckedChanged += (_, _) => dtpNgayNhapHoc.Enabled = chkNgayNhapHoc.Checked;
        chkHasParent.CheckedChanged += (_, _) => pnlParentFields.Enabled = chkHasParent.Checked;
        btnCancel.Click += (_, _) => Close();
    }

    private void ConfigureForCreateMode()
    {
        Text = "Thêm học viên mới";
        dtpNgaySinh.Value = DateTime.Today.AddYears(-10);
        cboGioiTinh.Items.AddRange(["Nam", "Nữ", "Khác"]);
        cboMoiQuanHe.Items.AddRange(["Cha", "Mẹ", "Người giám hộ"]);

        // Khối trạng thái học tập do backend tự set khi tạo mới -> ẩn và kéo các control bên dưới lên.
        grpStatus.Visible = false;
        var shift = grpParent.Top - grpStatus.Top;
        grpParent.Top -= shift;
        lblMessage.Top -= shift;
        btnSave.Top -= shift;
        btnCancel.Top -= shift;
        ClientSize = new Size(ClientSize.Width, ClientSize.Height - shift);
    }

    private void ConfigureForEditMode()
    {
        Text = $"Sửa hồ sơ học viên — {_studentId}";
        cboGioiTinh.Items.AddRange(["Nam", "Nữ", "Khác"]);
        cboTrangThaiHocTap.Items.AddRange(StudentStatuses.All.ToArray());

        // backend chưa hỗ trợ sửa phụ huynh qua API -> chỉ hiển thị, không cho nhập.
        chkHasParent.Visible = false;
        pnlParentFields.Visible = false;
        lblParentReadonly.Visible = true;

        Load += async (_, _) => await LoadStudentAsync();
    }

    private async Task LoadStudentAsync()
    {
        SetBusy(true);

        var result = await AppSession.Api.GetAsync<StudentDetailDto>($"admin/students/{_studentId}");

        SetBusy(false);

        if (!result.Success || result.Data is null)
        {
            lblMessage.Text = result.ErrorMessage;
            btnSave.Enabled = false;
            return;
        }

        var student = result.Data;

        txtHoTen.Text = student.HoTen;
        dtpNgaySinh.Value = student.NgaySinh.ToDateTime(TimeOnly.MinValue);
        cboGioiTinh.Text = student.GioiTinh ?? string.Empty;
        txtEmail.Text = student.Email ?? string.Empty;
        txtSoDienThoai.Text = student.SoDienThoai ?? string.Empty;
        txtDiaChi.Text = student.DiaChi ?? string.Empty;
        txtTrinhDoHienTai.Text = student.TrinhDoHienTai ?? string.Empty;
        txtMucTieuHocTap.Text = student.MucTieuHocTap ?? string.Empty;

        cboTrangThaiHocTap.SelectedItem = student.TrangThaiHocTap;

        if (student.NgayNhapHoc is { } ngayNhapHoc)
        {
            chkNgayNhapHoc.Checked = true;
            dtpNgayNhapHoc.Value = ngayNhapHoc.ToDateTime(TimeOnly.MinValue);
        }
        else
        {
            chkNgayNhapHoc.Checked = false;
        }

        lblParentReadonly.Text = student.PhuHuynh is { } parent
            ? $"Họ tên: {parent.HoTen}\n"
                + $"Mối quan hệ: {parent.MoiQuanHe}\n"
                + $"SĐT: {parent.SoDienThoai ?? "—"}\n"
                + $"Email: {parent.Email ?? "—"}\n"
                + $"Địa chỉ: {parent.DiaChi ?? "—"}"
            : "Học viên chưa có thông tin phụ huynh.";
    }

    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        lblMessage.ForeColor = UI.Theme.Danger;
        lblMessage.Text = string.Empty;

        if (string.IsNullOrWhiteSpace(txtHoTen.Text) || string.IsNullOrWhiteSpace(txtEmail.Text))
        {
            lblMessage.Text = "Vui lòng nhập Họ tên và Email.";
            return;
        }

        if (!_isEditMode && chkHasParent.Checked)
        {
            if (string.IsNullOrWhiteSpace(txtParentHoTen.Text) || string.IsNullOrWhiteSpace(cboMoiQuanHe.Text))
            {
                lblMessage.Text = "Đã chọn \"Có thông tin phụ huynh\" — vui lòng nhập Họ tên và Mối quan hệ của phụ huynh.";
                return;
            }
        }

        SetBusy(true);

        if (_isEditMode)
        {
            await SaveEditAsync();
        }
        else
        {
            await SaveCreateAsync();
        }

        SetBusy(false);
    }

    private async Task SaveCreateAsync()
    {
        var request = new CreateStudentRequest
        {
            HoTen = txtHoTen.Text.Trim(),
            NgaySinh = DateOnly.FromDateTime(dtpNgaySinh.Value),
            GioiTinh = NullIfEmpty(cboGioiTinh.Text),
            Email = txtEmail.Text.Trim(),
            SoDienThoai = NullIfEmpty(txtSoDienThoai.Text),
            DiaChi = NullIfEmpty(txtDiaChi.Text),
            MucTieuHocTap = NullIfEmpty(txtMucTieuHocTap.Text),
            TrinhDoHienTai = NullIfEmpty(txtTrinhDoHienTai.Text),
            PhuHuynh = chkHasParent.Checked
                ? new CreateParentRequest
                {
                    HoTen = txtParentHoTen.Text.Trim(),
                    SoDienThoai = NullIfEmpty(txtParentSoDienThoai.Text),
                    Email = NullIfEmpty(txtParentEmail.Text),
                    DiaChi = NullIfEmpty(txtParentDiaChi.Text),
                    MoiQuanHe = cboMoiQuanHe.Text.Trim()
                }
                : null
        };

        var result = await AppSession.Api.PostAsync<CreateStudentResponse>("admin/students", request);

        if (!result.Success || result.Data is null)
        {
            lblMessage.Text = FormatError(result);
            return;
        }

        DataChanged = true;

        using var dialog = new FormKetQuaMatKhau(
            "Đã tạo học viên",
            $"{result.Data.Message}\nMã học viên: {result.Data.StudentId}",
            result.Data.Username,
            result.Data.TemporaryPassword);
        dialog.ShowDialog(this);

        Close();
    }

    private async Task SaveEditAsync()
    {
        var request = new UpdateStudentRequest
        {
            HoTen = txtHoTen.Text.Trim(),
            NgaySinh = DateOnly.FromDateTime(dtpNgaySinh.Value),
            GioiTinh = NullIfEmpty(cboGioiTinh.Text),
            Email = NullIfEmpty(txtEmail.Text),
            SoDienThoai = NullIfEmpty(txtSoDienThoai.Text),
            DiaChi = NullIfEmpty(txtDiaChi.Text),
            MucTieuHocTap = NullIfEmpty(txtMucTieuHocTap.Text),
            TrinhDoHienTai = NullIfEmpty(txtTrinhDoHienTai.Text),
            TrangThaiHocTap = cboTrangThaiHocTap.SelectedItem as string,
            NgayNhapHoc = chkNgayNhapHoc.Checked ? DateOnly.FromDateTime(dtpNgayNhapHoc.Value) : null
        };

        var result = await AppSession.Api.PatchAsync<StudentDetailDto>($"admin/students/{_studentId}", request);

        if (!result.Success)
        {
            lblMessage.Text = FormatError(result);
            return;
        }

        DataChanged = true;
        lblMessage.ForeColor = UI.Theme.Success;
        lblMessage.Text = "Đã lưu thay đổi.";
    }

    private static string? NullIfEmpty(string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string FormatError<T>(ApiResult<T> result)
        => result.Errors.Count > 0
            ? $"{result.ErrorMessage}\n- {string.Join("\n- ", result.Errors)}"
            : result.ErrorMessage;

    private void SetBusy(bool busy)
    {
        UseWaitCursor = busy;
        btnSave.Enabled = !busy;
        btnCancel.Enabled = !busy;
    }
}
