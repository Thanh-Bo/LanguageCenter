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
    private IReadOnlyList<CourseListItemDto> _courses;
    private string? _loiKhoaHoc;
    private bool _dangNap;

    private readonly TextBox _txtMaLop = UiHelpers.CreateTextBox(320);
    private readonly TextBox _txtTenLop = UiHelpers.CreateTextBox(320);
    private readonly ComboBox _cboChuongTrinh = UiHelpers.CreateComboBox(320);
    private readonly ComboBox _cboKhoaHoc = UiHelpers.CreateComboBox(320);
    private readonly ComboBox _cboHocVu = UiHelpers.CreateComboBox(320);
    private readonly Button _btnTaiLaiKhoaHoc = UiHelpers.CreateSecondaryButton("Tải lại DS", 110, 26);
    private readonly DateTimePicker _dtpBatDau = new();
    private readonly DateTimePicker _dtpKetThuc = new();
    private readonly NumericUpDown _numSiSo = new();
    private readonly DateTimePicker _dtpGioBatDau = new();
    private readonly DateTimePicker _dtpGioKetThuc = new();
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
        ClientSize = new Size(620, 600);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        NapDuLieu(nhanVienHocVu);

        // Luôn tải lại danh sách khóa học mới nhất khi mở hộp thoại — tránh combo rỗng do
        // danh sách cũ trong bộ nhớ hoặc lần tải trước bị lỗi.
        Shown += async (_, _) =>
        {
            if (_existing is null) await NapKhoaHocAsync();
        };
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
        DatTruong("Tên lớp *", _txtTenLop, 112);
        DatTruong("Chương trình *", _cboChuongTrinh, 168);
        DatTruong("Khóa học *", _cboKhoaHoc, 224);
        DatTruong("Nhân viên học vụ phụ trách *", _cboHocVu, 280);

        // Chọn chương trình → combo khóa học chỉ hiện khóa thuộc chương trình đó.
        _cboChuongTrinh.SelectedIndexChanged += (_, _) =>
        {
            if (!_dangNap) DoVaoComboKhoaHoc();
        };

        // Mã lớp do hệ thống sinh — người dùng chỉ xem trước.
        if (_existing is null)
        {
            _txtMaLop.ReadOnly = true;
            _txtMaLop.BackColor = Color.FromArgb(241, 245, 249);
            _txtMaLop.Text = "(tự động sinh khi lưu)";
        }

        _btnTaiLaiKhoaHoc.Location = new Point(352, 244);
        _btnTaiLaiKhoaHoc.Click += async (_, _) => await NapKhoaHocAsync();
        Controls.Add(_btnTaiLaiKhoaHoc);

        _dtpBatDau.Format = DateTimePickerFormat.Short;
        _dtpBatDau.Width = 320;
        _dtpKetThuc.Format = DateTimePickerFormat.Short;
        _dtpKetThuc.Width = 320;

        DatTruong("Ngày bắt đầu", _dtpBatDau, 336);
        DatTruong("Ngày kết thúc dự kiến", _dtpKetThuc, 392);

        _numSiSo.Minimum = 1;
        _numSiSo.Maximum = 300;
        _numSiSo.Value = 20;
        _numSiSo.Width = 120;
        DatTruong("Sĩ số tối đa", _numSiSo, 448);

        // KHUNG GIỜ HỌC của lớp — cùng hàng với sĩ số để không kéo dài hộp thoại.
        Controls.Add(new Label
        {
            Text = "Giờ bắt đầu",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(200, 448)
        });

        Controls.Add(new Label
        {
            Text = "Giờ kết thúc",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(392, 448)
        });

        foreach (var dtp in new[] { _dtpGioBatDau, _dtpGioKetThuc })
        {
            dtp.Format = DateTimePickerFormat.Custom;
            dtp.CustomFormat = "HH:mm";
            dtp.ShowUpDown = true;
            dtp.Width = 110;
            dtp.Font = Theme.FontRegular;
            Controls.Add(dtp);
        }

        _dtpGioBatDau.Location = new Point(200, 468);
        _dtpGioBatDau.Value = DateTime.Today.AddHours(18);

        _dtpGioKetThuc.Location = new Point(392, 468);
        _dtpGioKetThuc.Value = DateTime.Today.AddHours(19).AddMinutes(30);

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(572, 40);
        _lblThongBao.Location = new Point(24, 500);
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Font = Theme.FontSub;
        Controls.Add(_lblThongBao);

        _btnLuu.Location = new Point(320, 548);
        _btnLuu.Click += async (_, _) => await LuuAsync();

        _btnHuy.Location = new Point(478, 548);
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

    private async Task NapKhoaHocAsync()
    {
        // Chế độ sửa: khóa học không đổi được nên dùng luôn danh sách đã có.
        if (_existing is not null && _courses.Count > 0)
        {
            return;
        }

        var result = await AppSession.Api.GetAsync<PagedResult<CourseListItemDto>>(
            QueryStringBuilder.Build("admin/courses", ("page", "1"), ("pageSize", "100")));

        if (result.Success && result.Data is not null && result.Data.Items.Count > 0)
        {
            _courses = result.Data.Items;
            _loiKhoaHoc = null;
        }
        else
        {
            _loiKhoaHoc = result.Success
                ? "Hệ thống chưa có khóa học nào. Hãy tạo khóa học trước khi mở lớp."
                : (string.IsNullOrWhiteSpace(result.ErrorMessage)
                    ? "Không tải được danh sách khóa học (kiểm tra API đã chạy và bạn đăng nhập bằng tài khoản Quản trị viên)."
                    : result.ErrorMessage);
        }

        // Giữ nguyên chương trình đang chọn (nếu còn) sau khi tải lại danh sách.
        DoVaoComboChuongTrinh(_cboChuongTrinh.SelectedItem?.ToString());
    }

    /// <summary>Đổ danh sách CHƯƠNG TRÌNH (lấy từ khóa học) rồi nạp khóa học theo chương trình đó.</summary>
    private void DoVaoComboChuongTrinh(string? chonTen = null, int? chonMaKhoaHoc = null)
    {
        _dangNap = true;

        var tenCu = chonTen ?? _cboChuongTrinh.SelectedItem?.ToString();

        _cboChuongTrinh.Items.Clear();

        foreach (var ten in _courses
                     .Select(c => c.TenChuongTrinh)
                     .Where(x => !string.IsNullOrWhiteSpace(x))
                     .Distinct()
                     .OrderBy(x => x))
        {
            _cboChuongTrinh.Items.Add(ten);
        }

        if (_cboChuongTrinh.Items.Count > 0)
        {
            var viTri = tenCu is null ? -1 : _cboChuongTrinh.Items.IndexOf(tenCu);
            _cboChuongTrinh.SelectedIndex = viTri >= 0 ? viTri : 0;
        }

        // Chế độ sửa: khóa học đã cố định → cả 2 ô đều khóa.
        _cboChuongTrinh.Enabled = _existing is null && _cboChuongTrinh.Items.Count > 0;

        _dangNap = false;

        DoVaoComboKhoaHoc(chonMaKhoaHoc);
    }

    /// <summary>Chỉ hiện KHÓA HỌC thuộc chương trình đang chọn, tên gọn (không kèm tên cấp độ).</summary>
    private void DoVaoComboKhoaHoc(int? chonMaKhoaHoc = null)
    {
        var chuongTrinh = _cboChuongTrinh.SelectedItem?.ToString();

        _dangNap = true;
        _cboKhoaHoc.Items.Clear();

        foreach (var k in _courses
                     .Where(c => chuongTrinh is null || c.TenChuongTrinh == chuongTrinh)
                     .OrderBy(c => c.TenKhoaHoc))
        {
            _cboKhoaHoc.Items.Add(new KhoaHocItem(k.MaKhoaHoc, k.TenKhoaHoc));
        }

        _cboKhoaHoc.Enabled = _existing is null && _cboKhoaHoc.Items.Count > 0;

        if (chonMaKhoaHoc is { } maKhoaHoc)
        {
            for (var i = 0; i < _cboKhoaHoc.Items.Count; i++)
            {
                if (_cboKhoaHoc.Items[i] is KhoaHocItem it && it.MaKhoaHoc == maKhoaHoc)
                {
                    _cboKhoaHoc.SelectedIndex = i;
                    _dangNap = false;
                    return;
                }
            }
        }

        _dangNap = false;

        if (_cboKhoaHoc.Items.Count > 0)
        {
            _cboKhoaHoc.SelectedIndex = 0;
            return;
        }

        _lblThongBao.Text = _loiKhoaHoc
            ?? $"Chương trình '{chuongTrinh}' chưa có khóa học nào.";
    }

    private void NapDuLieu(IReadOnlyList<string>? nhanVienHocVu)
    {
        // Nhân viên học vụ (backend chưa có endpoint danh mục nhân viên → lấy từ dữ liệu lớp đã có)
        _cboHocVu.Items.Clear();
        foreach (var ma in (nhanVienHocVu ?? Array.Empty<string>()).Distinct().OrderBy(x => x))
        {
            _cboHocVu.Items.Add(ma);
        }
        if (_cboHocVu.Items.Count == 0) _cboHocVu.Items.Add("NVHV001");
        _cboHocVu.SelectedIndex = 0;

        if (_existing is null)
        {
            DoVaoComboChuongTrinh();
            return;
        }

        // Chế độ sửa
        _txtMaLop.Text = _existing.MaLop;
        _txtMaLop.ReadOnly = true;
        _txtMaLop.BackColor = Color.FromArgb(241, 245, 249);

        _txtTenLop.Text = _existing.TenLop;

        var khoaHocCuaLop = _courses.FirstOrDefault(c => c.MaKhoaHoc == _existing.MaKhoaHoc);

        DoVaoComboChuongTrinh(khoaHocCuaLop?.TenChuongTrinh, _existing.MaKhoaHoc);

        _cboChuongTrinh.Enabled = false;
        _cboKhoaHoc.Enabled = false; // API không cho đổi khóa học của lớp

        if (!string.IsNullOrWhiteSpace(_existing.MaNhanVienHocVu))
        {
            if (!_cboHocVu.Items.Contains(_existing.MaNhanVienHocVu)) _cboHocVu.Items.Add(_existing.MaNhanVienHocVu);
            _cboHocVu.SelectedItem = _existing.MaNhanVienHocVu;
        }

        _dtpBatDau.Value = _existing.NgayBatDau.ToDateTime(TimeOnly.MinValue);
        _dtpKetThuc.Value = _existing.NgayKetThucDuKien.ToDateTime(TimeOnly.MinValue);

        if (_existing.GioBatDau is { } gioBD)
        {
            _dtpGioBatDau.Value = DateTime.Today.Add(gioBD.ToTimeSpan());
        }

        if (_existing.GioKetThuc is { } gioKT)
        {
            _dtpGioKetThuc.Value = DateTime.Today.Add(gioKT.ToTimeSpan());
        }

        _numSiSo.Value = Math.Clamp(_existing.SiSoToiDa, (int)_numSiSo.Minimum, (int)_numSiSo.Maximum);
    }

    private async Task LuuAsync()
    {
        _lblThongBao.Text = string.Empty;

        var tenLop = _txtTenLop.Text.Trim();

        if (tenLop.Length < 2)
        {
            _lblThongBao.Text = "Vui lòng nhập Tên lớp (tối thiểu 2 ký tự).";
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

        var gioBatDau = TimeOnly.FromDateTime(_dtpGioBatDau.Value);
        var gioKetThuc = TimeOnly.FromDateTime(_dtpGioKetThuc.Value);

        if (gioKetThuc <= gioBatDau)
        {
            _lblThongBao.Text = $"Giờ kết thúc ({gioKetThuc:HH\\:mm}) phải sau giờ bắt đầu ({gioBatDau:HH\\:mm}).";
            return;
        }

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        ApiResult<ClassListItemDto> result;

        if (_existing is null)
        {
            result = await AppSession.Api.PostAsync<ClassListItemDto>("admin/classes", new
            {
                tenLop,
                maKhoaHoc = khoaHoc.MaKhoaHoc,
                maNhanVienHocVu = hocVu,
                ngayBatDau = batDau.ToString("yyyy-MM-dd"),
                ngayKetThucDuKien = ketThuc.ToString("yyyy-MM-dd"),
                gioBatDau = gioBatDau.ToString("HH:mm:ss"),
                gioKetThuc = gioKetThuc.ToString("HH:mm:ss"),
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
                gioBatDau = gioBatDau.ToString("HH:mm:ss"),
                gioKetThuc = gioKetThuc.ToString("HH:mm:ss"),
                siSoToiDa = (int)_numSiSo.Value
            });
        }

        UseWaitCursor = false;
        _btnLuu.Enabled = true;

        if (!result.Success)
        {
            var chiTiet = result.ThongDiepDayDu();

            _lblThongBao.Text = string.IsNullOrWhiteSpace(result.ErrorMessage)
                ? "Không lưu được lớp học."
                : result.ErrorMessage;

            // Lỗi kiểm tra dữ liệu của server chỉ trả "title" chung chung —
            // hiện đầy đủ từng trường để biết sai ở đâu.
            if (result.Errors.Count > 0)
            {
                MessageBox.Show(this, chiTiet, "Không lưu được lớp học",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

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
