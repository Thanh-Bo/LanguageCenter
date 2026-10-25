using LanguageCenter.WinForms.Api;

namespace LanguageCenter.WinForms.Dialogs;

/// Thêm mới hoặc sửa hồ sơ giáo viên / trợ giảng.
/// <para>
/// <c>laTroGiang</c> quyết định form thuộc tab nào: tab Trợ giảng thì loại cố định là "Trợ giảng",
/// tab Giáo viên thì cố định là "Giáo viên" — người dùng không chọn được (tránh tạo sai mã GV/TG).
/// </para>
/// <para>
/// Ngày sinh luôn có (ai cũng có ngày sinh) và Giới tính chỉ chọn trong dropdown.
/// Ngày vào làm đã bỏ khỏi form.
/// </para>
public partial class FormSuaGiaoVien : Form
{
    private readonly string? _teacherId;
    private readonly bool _isEditMode;
    private readonly bool _laTroGiang;

    public bool DataChanged { get; private set; }

    /// Loại cố định theo tab đang mở.
    private string LoaiGiaoVien => _laTroGiang ? TeacherTypes.TroGiang : TeacherTypes.GiaoVien;

    /// Chế độ tạo mới.
    public FormSuaGiaoVien(bool laTroGiang)
    {
        _isEditMode = false;
        _laTroGiang = laTroGiang;
        InitializeComponent();
        WireEvents();
        ConfigureForCreateMode();
    }

    /// Chế độ sửa.
    public FormSuaGiaoVien(string teacherId, bool laTroGiang)
    {
        _teacherId = teacherId;
        _isEditMode = true;
        _laTroGiang = laTroGiang;
        InitializeComponent();
        WireEvents();
        ConfigureForEditMode();
    }

    private void WireEvents()
    {
        btnCancel.Click += (_, _) => Close();

        // Ô SĐT: chỉ cho gõ chữ số, tối đa 10 ký tự.
        UI.KiemTraNhap.ChiNhapSoDienThoai(txtSoDienThoai);
    }

    private void ConfigureForCreateMode()
    {
        Text = $"Thêm {LoaiGiaoVien.ToLowerInvariant()} mới";
        cboGioiTinh.Items.AddRange(["Nam", "Nữ", "Khác"]);
        dtpNgaySinh.Value = DateTime.Today.AddYears(-25);

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
        Text = $"Sửa hồ sơ {LoaiGiaoVien.ToLowerInvariant()} — {_teacherId}";
        cboGioiTinh.Items.AddRange(["Nam", "Nữ", "Khác"]);
        cboTrangThai.Items.AddRange(WorkStatuses.All.ToArray());

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

        // DropDownList: chọn theo index, không gán Text tự do.
        var chiSoGioiTinh = cboGioiTinh.Items.IndexOf(teacher.GioiTinh);
        cboGioiTinh.SelectedIndex = chiSoGioiTinh >= 0 ? chiSoGioiTinh : -1;

        txtEmail.Text = teacher.Email ?? string.Empty;
        txtSoDienThoai.Text = teacher.SoDienThoai ?? string.Empty;
        txtDiaChi.Text = teacher.DiaChi ?? string.Empty;
        txtChuyenMon.Text = teacher.ChuyenMon ?? string.Empty;

        if (teacher.SoNamKinhNghiem is { } soNam)
        {
            numSoNamKinhNghiem.Value = Math.Clamp(soNam, 0, 80);
        }

        dtpNgaySinh.Value = teacher.NgaySinh is { } ngaySinh
            ? ngaySinh.ToDateTime(TimeOnly.MinValue)
            : DateTime.Today.AddYears(-25);

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
                $"  - {b.TenBangCap}" + (b.ChuyenNganh is null ? "" : $" — {b.ChuyenNganh}")
                    + (b.NamTotNghiep is null ? "" : $" ({b.NamTotNghiep})")));
        }

        if (teacher.ChungChis.Count > 0)
        {
            if (lines.Count > 0) lines.Add(string.Empty);
            lines.Add("Chứng chỉ:");
            lines.AddRange(teacher.ChungChis.Select(c =>
                $"  - {c.TenChungChi}" + (c.KetQua is null ? "" : $" — {c.KetQua}")
                    + (c.NgayHetHan is null ? "" : $" (hạn {UI.UiHelpers.FormatDate(c.NgayHetHan)})")));
        }

        return string.Join('\n', lines);
    }

    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        lblMessage.ForeColor = UI.Theme.Danger;
        lblMessage.Text = string.Empty;

        if (!KiemTraDuLieuHopLe())
        {
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
            NgaySinh = DateOnly.FromDateTime(dtpNgaySinh.Value),
            GioiTinh = NullIfEmpty(cboGioiTinh.Text),
            Email = txtEmail.Text.Trim(),
            SoDienThoai = NullIfEmpty(txtSoDienThoai.Text),
            DiaChi = NullIfEmpty(txtDiaChi.Text),
            LoaiGiaoVien = LoaiGiaoVien,
            ChuyenMon = NullIfEmpty(txtChuyenMon.Text),
            SoNamKinhNghiem = (int)numSoNamKinhNghiem.Value
        };

        var result = await AppSession.Api.PostAsync<CreateTeacherResponse>("admin/teachers", request);

        if (!result.Success || result.Data is null)
        {
            lblMessage.Text = FormatError(result);
            return;
        }

        DataChanged = true;

        using var dialog = new FormKetQuaMatKhau(
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
            NgaySinh = DateOnly.FromDateTime(dtpNgaySinh.Value),
            GioiTinh = NullIfEmpty(cboGioiTinh.Text),
            Email = NullIfEmpty(txtEmail.Text),
            SoDienThoai = NullIfEmpty(txtSoDienThoai.Text),
            DiaChi = NullIfEmpty(txtDiaChi.Text),
            ChuyenMon = NullIfEmpty(txtChuyenMon.Text),
            SoNamKinhNghiem = (int)numSoNamKinhNghiem.Value,
            TrangThai = cboTrangThai.SelectedItem as string
        };

        var result = await AppSession.Api.PatchAsync<TeacherDetailDto>($"admin/teachers/{_teacherId}", request);

        if (!result.Success)
        {
            lblMessage.Text = FormatError(result);
            return;
        }

        DataChanged = true;
        UI.KiemTraNhap.HienThiThanhCong(lblMessage, "Đã lưu thay đổi.");
    }

    /// <summary>
    /// Kiểm tra dữ liệu nhập trước khi gọi API — cùng quy tắc với backend để báo lỗi ngay tại form.
    /// Ô sai được tô đỏ, danh sách lỗi hiển thị bên dưới và con trỏ nhảy về ô sai đầu tiên.
    /// </summary>
    private bool KiemTraDuLieuHopLe()
    {
        var controls = new Control[]
        {
            txtHoTen, txtEmail, txtSoDienThoai, txtDiaChi, txtChuyenMon, cboGioiTinh, dtpNgaySinh
        };
        UI.KiemTraNhap.BoDanhDau(controls);

        var loi = new List<string>();

        UI.KiemTraNhap.BatBuoc(txtHoTen, "Họ tên", loi, minLength: 2);
        UI.KiemTraNhap.DoDai(txtHoTen, "Họ tên", 150, loi);
        UI.KiemTraNhap.Email(txtEmail, "Email", loi);
        UI.KiemTraNhap.SoDienThoai(txtSoDienThoai, "Số điện thoại", loi);
        UI.KiemTraNhap.DoDai(txtDiaChi, "Địa chỉ", 500, loi);
        UI.KiemTraNhap.DoDai(txtChuyenMon, "Chuyên môn", 500, loi);
        UI.KiemTraNhap.NgaySinh(dtpNgaySinh, "Ngày sinh", loi);

        // Tạo mới: bắt buộc đủ thông tin để hồ sơ dùng được ngay (lịch dạy, liên hệ).
        if (!_isEditMode)
        {
            UI.KiemTraNhap.ChonBatBuoc(cboGioiTinh, "Giới tính", loi);

            if (string.IsNullOrWhiteSpace(txtSoDienThoai.Text))
            {
                UI.KiemTraNhap.DanhDauLoi(txtSoDienThoai);
                loi.Add("Số điện thoại: bắt buộc nhập.");
            }

            if (string.IsNullOrWhiteSpace(txtDiaChi.Text))
            {
                UI.KiemTraNhap.DanhDauLoi(txtDiaChi);
                loi.Add("Địa chỉ: bắt buộc nhập.");
            }

            if (string.IsNullOrWhiteSpace(txtChuyenMon.Text))
            {
                UI.KiemTraNhap.DanhDauLoi(txtChuyenMon);
                loi.Add("Chuyên môn: bắt buộc nhập.");
            }
        }

        UI.KiemTraNhap.HienThiLoi(lblMessage, loi);

        if (loi.Count > 0)
        {
            UI.KiemTraNhap.FocusLoi(controls);
            return false;
        }

        return true;
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
