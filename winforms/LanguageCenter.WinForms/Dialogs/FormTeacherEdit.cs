using LanguageCenter.WinForms.Api;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Thêm mới hoặc sửa hồ sơ giáo viên / trợ giảng.
/// Constructor không tham số → chế độ TẠO MỚI (chọn được Loại giáo viên).
/// Constructor(teacherId) → chế độ SỬA (Loại giáo viên khóa cứng — xem ghi chú trong
/// UpdateTeacherRequest của backend: đổi loại sẽ làm hỏng mã GV/TG và dữ liệu lịch sử;
/// hiện thêm khối trạng thái làm việc và Bằng cấp/Chứng chỉ chỉ đọc, Phase 3 chưa có CRUD).
/// </summary>
public partial class FormTeacherEdit : Form
{
    private readonly string? _teacherId;
    private readonly bool _isEditMode;

    public bool DataChanged { get; private set; }

    /// <summary>Chế độ tạo mới.</summary>
    public FormTeacherEdit()
    {
        _isEditMode = false;
        InitializeComponent();
        ConfigureForCreateMode();
    }

    /// <summary>Chế độ sửa.</summary>
    public FormTeacherEdit(string teacherId)
    {
        _teacherId = teacherId;
        _isEditMode = true;
        InitializeComponent();
        ConfigureForEditMode();
    }

    private void ConfigureForCreateMode()
    {
        Text = "Thêm giáo viên / trợ giảng mới";
        cboGioiTinh.Items.AddRange(["Nam", "Nữ", "Khác"]);
        cboLoaiGiaoVien.Items.AddRange(TeacherTypes.All.ToArray());
        cboLoaiGiaoVien.SelectedIndex = 0;

        // Trạng thái làm việc + Bằng cấp/Chứng chỉ chỉ có ý nghĩa với hồ sơ đã tồn tại.
        grpStatus.Visible = false;
        grpQualifications.Visible = false;
        var shift = (grpQualifications.Bottom + 16) - grpStatus.Top;
        lblMessage.Top -= shift;
        btnSave.Top -= shift;
        btnCancel.Top -= shift;
        ClientSize = new Size(ClientSize.Width, ClientSize.Height - shift);
    }

    private void ConfigureForEditMode()
    {
        Text = $"Sửa hồ sơ giáo viên / trợ giảng — {_teacherId}";
        cboGioiTinh.Items.AddRange(["Nam", "Nữ", "Khác"]);
        cboTrangThai.Items.AddRange(WorkStatuses.All.ToArray());

        // LoaiGiaoVien không sửa được qua API -> chỉ hiển thị, khóa combo.
        cboLoaiGiaoVien.Items.AddRange(TeacherTypes.All.ToArray());
        cboLoaiGiaoVien.Enabled = false;

        Load += async (_, _) => await LoadTeacherAsync();
    }

    private async Task LoadTeacherAsync()
    {
        SetBusy(true);

        var result = await AppSession.Api.GetAsync<TeacherDetailDto>($"admin/teachers/{_teacherId}");

        SetBusy(false);

        if (!result.Success || result.Data is null)
        {
            lblMessage.Text = result.ErrorMessage;
            btnSave.Enabled = false;
            return;
        }

        var teacher = result.Data;

        txtHoTen.Text = teacher.HoTen;
        cboGioiTinh.Text = teacher.GioiTinh ?? string.Empty;
        txtEmail.Text = teacher.Email ?? string.Empty;
        txtSoDienThoai.Text = teacher.SoDienThoai ?? string.Empty;
        txtDiaChi.Text = teacher.DiaChi ?? string.Empty;
        txtChuyenMon.Text = teacher.ChuyenMon ?? string.Empty;
        cboLoaiGiaoVien.SelectedItem = teacher.LoaiGiaoVien;

        if (teacher.SoNamKinhNghiem is { } soNam)
        {
            numSoNamKinhNghiem.Value = Math.Clamp(soNam, 0, 80);
        }

        if (teacher.NgaySinh is { } ngaySinh)
        {
            chkNgaySinh.Checked = true;
            dtpNgaySinh.Value = ngaySinh.ToDateTime(TimeOnly.MinValue);
        }
        else
        {
            chkNgaySinh.Checked = false;
        }

        if (teacher.NgayVaoLam is { } ngayVaoLam)
        {
            chkNgayVaoLam.Checked = true;
            dtpNgayVaoLam.Value = ngayVaoLam.ToDateTime(TimeOnly.MinValue);
        }
        else
        {
            chkNgayVaoLam.Checked = false;
        }

        cboTrangThai.SelectedItem = teacher.TrangThai;

        lblQualificationsReadonly.Text = BuildQualificationsText(teacher);
    }

    private static string BuildQualificationsText(TeacherDetailDto teacher)
    {
        var lines = new List<string>();

        if (teacher.BangCaps.Count == 0 && teacher.ChungChis.Count == 0)
        {
            return "Chưa có bằng cấp / chứng chỉ nào được ghi nhận.";
        }

        if (teacher.BangCaps.Count > 0)
        {
            lines.Add("Bằng cấp:");
            lines.AddRange(teacher.BangCaps.Select(b =>
                $"  • {b.TenBangCap}" + (b.ChuyenNganh is null ? "" : $" — {b.ChuyenNganh}")
                    + (b.NamTotNghiep is null ? "" : $" ({b.NamTotNghiep})")));
        }

        if (teacher.ChungChis.Count > 0)
        {
            if (lines.Count > 0) lines.Add(string.Empty);
            lines.Add("Chứng chỉ:");
            lines.AddRange(teacher.ChungChis.Select(c =>
                $"  • {c.TenChungChi}" + (c.KetQua is null ? "" : $" — {c.KetQua}")
                    + (c.NgayHetHan is null ? "" : $" (hạn {UI.UiHelpers.FormatDate(c.NgayHetHan)})")));
        }

        return string.Join('\n', lines);
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

        if (!_isEditMode && string.IsNullOrWhiteSpace(cboLoaiGiaoVien.Text))
        {
            lblMessage.Text = "Vui lòng chọn Loại giáo viên (Giáo viên / Trợ giảng).";
            return;
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
        var request = new CreateTeacherRequest
        {
            HoTen = txtHoTen.Text.Trim(),
            NgaySinh = chkNgaySinh.Checked ? DateOnly.FromDateTime(dtpNgaySinh.Value) : null,
            GioiTinh = NullIfEmpty(cboGioiTinh.Text),
            Email = txtEmail.Text.Trim(),
            SoDienThoai = NullIfEmpty(txtSoDienThoai.Text),
            DiaChi = NullIfEmpty(txtDiaChi.Text),
            LoaiGiaoVien = cboLoaiGiaoVien.Text.Trim(),
            ChuyenMon = NullIfEmpty(txtChuyenMon.Text),
            SoNamKinhNghiem = (int)numSoNamKinhNghiem.Value,
            NgayVaoLam = chkNgayVaoLam.Checked ? DateOnly.FromDateTime(dtpNgayVaoLam.Value) : null
        };

        var result = await AppSession.Api.PostAsync<CreateTeacherResponse>("admin/teachers", request);

        if (!result.Success || result.Data is null)
        {
            lblMessage.Text = FormatError(result);
            return;
        }

        DataChanged = true;

        using var dialog = new FormSecretResult(
            "Đã tạo hồ sơ giáo viên / trợ giảng",
            $"{result.Data.Message}\nMã: {result.Data.TeacherId} ({result.Data.LoaiGiaoVien})",
            result.Data.Username,
            result.Data.TemporaryPassword);
        dialog.ShowDialog(this);

        Close();
    }

    private async Task SaveEditAsync()
    {
        var request = new UpdateTeacherRequest
        {
            HoTen = txtHoTen.Text.Trim(),
            NgaySinh = chkNgaySinh.Checked ? DateOnly.FromDateTime(dtpNgaySinh.Value) : null,
            GioiTinh = NullIfEmpty(cboGioiTinh.Text),
            Email = NullIfEmpty(txtEmail.Text),
            SoDienThoai = NullIfEmpty(txtSoDienThoai.Text),
            DiaChi = NullIfEmpty(txtDiaChi.Text),
            ChuyenMon = NullIfEmpty(txtChuyenMon.Text),
            SoNamKinhNghiem = (int)numSoNamKinhNghiem.Value,
            NgayVaoLam = chkNgayVaoLam.Checked ? DateOnly.FromDateTime(dtpNgayVaoLam.Value) : null,
            TrangThai = cboTrangThai.SelectedItem as string
        };

        var result = await AppSession.Api.PatchAsync<TeacherDetailDto>($"admin/teachers/{_teacherId}", request);

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
