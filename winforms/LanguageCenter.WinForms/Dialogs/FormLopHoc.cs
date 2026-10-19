using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Tạo mới / chỉnh sửa lớp học — gọi trực tiếp API <c>POST|PATCH /api/admin/classes</c>.
/// Không phụ thuộc Designer để tránh phải sửa file .Designer.cs.
/// </summary>
public sealed class FormLopHoc : Form
{
    private readonly ClassListItemDto? _existing;
    private readonly IReadOnlyList<CourseListItemDto> _courses;

    private readonly TextBox _txtMaLop = UiHelpers.CreateTextBox(320);
    private readonly TextBox _txtTenLop = UiHelpers.CreateTextBox(320);
    private readonly ComboBox _cboKhoaHoc = UiHelpers.CreateComboBox(320);
    private readonly ComboBox _cboHocVu = UiHelpers.CreateComboBox(320);
    private readonly DateTimePicker _dtpBatDau = new();
    private readonly DateTimePicker _dtpKetThuc = new();
    private readonly NumericUpDown _numSiSo = new();
    private readonly Label _lblThongBao = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Lưu lớp học", 150, 36);
    private readonly Button _btnHuy = UiHelpers.CreateSecondaryButton("Hủy bỏ", 110, 36);

    /// <summary>True khi đã ghi dữ liệu thành công — nơi gọi nên tải lại danh sách.</summary>
    public bool DataChanged { get; private set; }

    public FormLopHoc(
        IReadOnlyList<CourseListItemDto> courses,
        ClassListItemDto? existing = null,
        IReadOnlyList<string>? nhanVienHocVu = null)
    {
        _courses = courses;
        _existing = existing;

        Text = existing is null ? "Thêm lớp học mới" : $"Cập nhật lớp {existing.MaLop}";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(600, 560);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        NapDuLieu(nhanVienHocVu);
    }

    private void DungGiaoDien()
    {
        var tieuDe = new Label
        {
            Text = _existing is null ? "THÊM LỚP HỌC MỚI" : "CẬP NHẬT THÔNG TIN LỚP HỌC",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 18)
        };

        Controls.Add(tieuDe);

        DatTruong("Mã lớp *", _txtMaLop, 56);
        DatTruong("Tên lớp *", _txtTenLop, 116);
        DatTruong("Khóa học *", _cboKhoaHoc, 176);
        DatTruong("Nhân viên học vụ phụ trách *", _cboHocVu, 236);

        _dtpBatDau.Format = DateTimePickerFormat.Short;
        _dtpBatDau.Width = 320;
        _dtpKetThuc.Format = DateTimePickerFormat.Short;
        _dtpKetThuc.Width = 320;

        DatTruong("Ngày bắt đầu", _dtpBatDau, 296);
        DatTruong("Ngày kết thúc dự kiến", _dtpKetThuc, 356);

        _numSiSo.Minimum = 1;
        _numSiSo.Maximum = 300;
        _numSiSo.Value = 20;
        _numSiSo.Width = 120;
        DatTruong("Sĩ số tối đa", _numSiSo, 416);

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(552, 40);
        _lblThongBao.Location = new Point(24, 452);
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Font = Theme.FontSub;
        Controls.Add(_lblThongBao);

        _btnLuu.Location = new Point(304, 500);
        _btnLuu.Click += async (_, _) => await LuuAsync();

        _btnHuy.Location = new Point(462, 500);
        _btnHuy.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        Controls.Add(_btnLuu);
        Controls.Add(_btnHuy);

        AcceptButton = _btnLuu;
        CancelButton = _btnHuy;
    }

    private void DatTruong(string nhan, Control control, int y)
    {
        var lbl = new Label
        {
            Text = nhan,
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, y)
        };

        control.Location = new Point(24, y + 20);

        Controls.Add(lbl);
        Controls.Add(control);
    }

    private void NapDuLieu(IReadOnlyList<string>? nhanVienHocVu)
    {
        // Khóa học
        _cboKhoaHoc.Items.Clear();
        foreach (var k in _courses.OrderBy(c => c.TenKhoaHoc))
        {
            _cboKhoaHoc.Items.Add(new KhoaHocItem(k.MaKhoaHoc, $"{k.TenKhoaHoc} · {k.TenCapDo}"));
        }
        if (_cboKhoaHoc.Items.Count > 0) _cboKhoaHoc.SelectedIndex = 0;

        // Nhân viên học vụ (backend chưa có endpoint danh mục nhân viên → lấy từ dữ liệu lớp đã có)
        _cboHocVu.Items.Clear();
        foreach (var ma in (nhanVienHocVu ?? Array.Empty<string>()).Distinct().OrderBy(x => x))
        {
            _cboHocVu.Items.Add(ma);
        }
        if (_cboHocVu.Items.Count == 0) _cboHocVu.Items.Add("NVHV001");
        _cboHocVu.SelectedIndex = 0;

        if (_existing is null) return;

        // Chế độ sửa
        _txtMaLop.Text = _existing.MaLop;
        _txtMaLop.ReadOnly = true;
        _txtMaLop.BackColor = Color.FromArgb(241, 245, 249);

        _txtTenLop.Text = _existing.TenLop;

        for (var i = 0; i < _cboKhoaHoc.Items.Count; i++)
        {
            if (_cboKhoaHoc.Items[i] is KhoaHocItem it && it.MaKhoaHoc == _existing.MaKhoaHoc)
            {
                _cboKhoaHoc.SelectedIndex = i;
                break;
            }
        }
        _cboKhoaHoc.Enabled = false; // API không cho đổi khóa học của lớp

        if (!string.IsNullOrWhiteSpace(_existing.MaNhanVienHocVu))
        {
            if (!_cboHocVu.Items.Contains(_existing.MaNhanVienHocVu)) _cboHocVu.Items.Add(_existing.MaNhanVienHocVu);
            _cboHocVu.SelectedItem = _existing.MaNhanVienHocVu;
        }

        _dtpBatDau.Value = _existing.NgayBatDau.ToDateTime(TimeOnly.MinValue);
        _dtpKetThuc.Value = _existing.NgayKetThucDuKien.ToDateTime(TimeOnly.MinValue);
        _numSiSo.Value = Math.Clamp(_existing.SiSoToiDa, (int)_numSiSo.Minimum, (int)_numSiSo.Maximum);
    }

    private async Task LuuAsync()
    {
        _lblThongBao.Text = string.Empty;

        var maLop = _txtMaLop.Text.Trim();
        var tenLop = _txtTenLop.Text.Trim();

        if (maLop.Length == 0 || tenLop.Length == 0)
        {
            _lblThongBao.Text = "Vui lòng nhập đầy đủ Mã lớp và Tên lớp.";
            return;
        }

        if (_cboKhoaHoc.SelectedItem is not KhoaHocItem khoaHoc)
        {
            _lblThongBao.Text = "Vui lòng chọn khóa học.";
            return;
        }

        var hocVu = _cboHocVu.SelectedItem?.ToString();
        if (string.IsNullOrWhiteSpace(hocVu))
        {
            _lblThongBao.Text = "Vui lòng chọn nhân viên học vụ.";
            return;
        }

        var batDau = DateOnly.FromDateTime(_dtpBatDau.Value);
        var ketThuc = DateOnly.FromDateTime(_dtpKetThuc.Value);

        if (ketThuc < batDau)
        {
            _lblThongBao.Text = "Ngày kết thúc phải sau hoặc bằng ngày bắt đầu.";
            return;
        }

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        ApiResult<ClassListItemDto> result;

        if (_existing is null)
        {
            result = await AppSession.Api.PostAsync<ClassListItemDto>("admin/classes", new
            {
                maLop,
                tenLop,
                maKhoaHoc = khoaHoc.MaKhoaHoc,
                maNhanVienHocVu = hocVu,
                ngayBatDau = batDau.ToString("yyyy-MM-dd"),
                ngayKetThucDuKien = ketThuc.ToString("yyyy-MM-dd"),
                siSoToiDa = (int)_numSiSo.Value
            });
        }
        else
        {
            result = await AppSession.Api.PatchAsync<ClassListItemDto>($"admin/classes/{_existing.MaLopHoc}", new
            {
                tenLop,
                maNhanVienHocVu = hocVu,
                ngayBatDau = batDau.ToString("yyyy-MM-dd"),
                ngayKetThucDuKien = ketThuc.ToString("yyyy-MM-dd"),
                siSoToiDa = (int)_numSiSo.Value
            });
        }

        UseWaitCursor = false;
        _btnLuu.Enabled = true;

        if (!result.Success)
        {
            _lblThongBao.Text = string.IsNullOrWhiteSpace(result.ErrorMessage)
                ? "Không lưu được lớp học."
                : result.ErrorMessage;
            return;
        }

        DataChanged = true;
        DialogResult = DialogResult.OK;
        Close();
    }

    private sealed record KhoaHocItem(int MaKhoaHoc, string MoTa)
    {
        public override string ToString() => MoTa;
    }
}
