using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>Lịch học định kỳ (thứ + giờ lặp lại) — <c>POST|PATCH /api/admin/schedules</c>.</summary>
public sealed class FormLichHoc : Form
{
    private static readonly string[] ThuLabels =
    [
        "Thứ 2", "Thứ 3", "Thứ 4", "Thứ 5", "Thứ 6", "Thứ 7", "Chủ nhật"
    ];

    private readonly ScheduleListItemDto? _existing;
    private readonly IReadOnlyList<ClassListItemDto> _classes;
    private readonly IReadOnlyList<RoomListItemDto> _rooms;

    private readonly ComboBox _cboLop = UiHelpers.CreateComboBox(320);
    private readonly ComboBox _cboPhong = UiHelpers.CreateComboBox(320);
    private readonly ComboBox _cboThu = UiHelpers.CreateComboBox(160);
    private readonly DateTimePicker _dtpGioBatDau = new();
    private readonly DateTimePicker _dtpGioKetThuc = new();
    private readonly DateTimePicker _dtpTuNgay = new();
    private readonly CheckBox _chkCoNgayKetThuc = new();
    private readonly DateTimePicker _dtpDenNgay = new();
    private readonly Label _lblThongBao = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Lưu lịch học", 150, 36);
    private readonly Button _btnHuy = UiHelpers.CreateSecondaryButton("Hủy bỏ", 110, 36);

    public bool DataChanged { get; private set; }

    public FormLichHoc(
        IReadOnlyList<ClassListItemDto> classes,
        IReadOnlyList<RoomListItemDto> rooms,
        ScheduleListItemDto? existing = null)
    {
        _classes = classes;
        _rooms = rooms;
        _existing = existing;

        Text = existing is null ? "Thêm lịch học định kỳ" : "Cập nhật lịch học định kỳ";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(600, 560);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        NapDuLieu();
    }

    private void DungGiaoDien()
    {
        Controls.Add(new Label
        {
            Text = _existing is null ? "THÊM LỊCH HỌC ĐỊNH KỲ" : "CẬP NHẬT LỊCH HỌC ĐỊNH KỲ",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 18)
        });

        DatTruong("Lớp học *", _cboLop, 56);
        DatTruong("Phòng học *", _cboPhong, 116);
        DatTruong("Thứ trong tuần *", _cboThu, 176);

        foreach (var t in ThuLabels) _cboThu.Items.Add(t);
        _cboThu.SelectedIndex = 0;

        _dtpGioBatDau.Format = DateTimePickerFormat.Time;
        _dtpGioBatDau.ShowUpDown = true;
        _dtpGioBatDau.Width = 140;
        _dtpGioBatDau.Value = DateTime.Today.AddHours(18);
        DatTruong("Giờ bắt đầu", _dtpGioBatDau, 236);

        _dtpGioKetThuc.Format = DateTimePickerFormat.Time;
        _dtpGioKetThuc.ShowUpDown = true;
        _dtpGioKetThuc.Width = 140;
        _dtpGioKetThuc.Value = DateTime.Today.AddHours(19).AddMinutes(30);
        _dtpGioKetThuc.Location = new Point(200, 256);
        Controls.Add(_dtpGioKetThuc);
        Controls.Add(new Label
        {
            Text = "Giờ kết thúc",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(200, 236)
        });

        _dtpTuNgay.Format = DateTimePickerFormat.Short;
        _dtpTuNgay.Width = 320;
        DatTruong("Ngày áp dụng từ", _dtpTuNgay, 306);

        _chkCoNgayKetThuc.Text = "Có ngày kết thúc";
        _chkCoNgayKetThuc.AutoSize = true;
        _chkCoNgayKetThuc.Location = new Point(24, 372);
        _chkCoNgayKetThuc.CheckedChanged += (_, _) => _dtpDenNgay.Enabled = _chkCoNgayKetThuc.Checked;
        Controls.Add(_chkCoNgayKetThuc);

        _dtpDenNgay.Format = DateTimePickerFormat.Short;
        _dtpDenNgay.Width = 320;
        _dtpDenNgay.Enabled = false;
        _dtpDenNgay.Location = new Point(24, 396);
        Controls.Add(_dtpDenNgay);

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(552, 34);
        _lblThongBao.Location = new Point(24, 448);
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Font = Theme.FontSub;
        Controls.Add(_lblThongBao);

        _btnLuu.Location = new Point(304, 490);
        _btnLuu.Click += async (_, _) => await LuuAsync();

        _btnHuy.Location = new Point(462, 490);
        _btnHuy.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        Controls.Add(_btnLuu);
        Controls.Add(_btnHuy);

        AcceptButton = _btnLuu;
        CancelButton = _btnHuy;
    }

    private void DatTruong(string nhan, Control control, int y)
    {
        Controls.Add(new Label
        {
            Text = nhan,
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, y)
        });

        control.Location = new Point(24, y + 20);
        Controls.Add(control);
    }

    private void NapDuLieu()
    {
        _cboLop.Items.Clear();
        foreach (var c in _classes.OrderBy(c => c.MaLop))
        {
            _cboLop.Items.Add(new LopItem(c.MaLopHoc, $"{c.MaLop} — {c.TenLop}"));
        }
        if (_cboLop.Items.Count > 0) _cboLop.SelectedIndex = 0;

        _cboPhong.Items.Clear();
        foreach (var r in _rooms.OrderBy(r => r.TenPhong))
        {
            _cboPhong.Items.Add(new PhongItem(r.MaPhongHoc, $"{r.TenPhong} (sức chứa {r.SucChua})"));
        }
        if (_cboPhong.Items.Count > 0) _cboPhong.SelectedIndex = 0;

        if (_existing is null) return;

        for (var i = 0; i < _cboLop.Items.Count; i++)
        {
            if (_cboLop.Items[i] is LopItem it && it.MaLopHoc == _existing.MaLopHoc)
            {
                _cboLop.SelectedIndex = i;
                break;
            }
        }
        _cboLop.Enabled = false; // API không cho đổi lớp của lịch

        for (var i = 0; i < _cboPhong.Items.Count; i++)
        {
            if (_cboPhong.Items[i] is PhongItem it && it.MaPhongHoc == _existing.MaPhongHoc)
            {
                _cboPhong.SelectedIndex = i;
                break;
            }
        }

        _cboThu.SelectedIndex = Math.Clamp(_existing.ThuTrongTuan - 1, 0, 6);
        _dtpGioBatDau.Value = DateTime.Today.Add(_existing.GioBatDau.ToTimeSpan());
        _dtpGioKetThuc.Value = DateTime.Today.Add(_existing.GioKetThuc.ToTimeSpan());
        _dtpTuNgay.Value = _existing.NgayApDungTu.ToDateTime(TimeOnly.MinValue);

        if (_existing.NgayApDungDen.HasValue)
        {
            _chkCoNgayKetThuc.Checked = true;
            _dtpDenNgay.Value = _existing.NgayApDungDen.Value.ToDateTime(TimeOnly.MinValue);
        }
    }

    private async Task LuuAsync()
    {
        _lblThongBao.Text = string.Empty;

        if (_cboLop.SelectedItem is not LopItem lop)
        {
            _lblThongBao.Text = "Vui lòng chọn lớp học.";
            return;
        }

        if (_cboPhong.SelectedItem is not PhongItem phong)
        {
            _lblThongBao.Text = "Vui lòng chọn phòng học.";
            return;
        }

        var gioBatDau = TimeOnly.FromDateTime(_dtpGioBatDau.Value);
        var gioKetThuc = TimeOnly.FromDateTime(_dtpGioKetThuc.Value);

        if (gioKetThuc <= gioBatDau)
        {
            _lblThongBao.Text = "Giờ kết thúc phải sau giờ bắt đầu.";
            return;
        }

        var tuNgay = DateOnly.FromDateTime(_dtpTuNgay.Value);
        DateOnly? denNgay = _chkCoNgayKetThuc.Checked ? DateOnly.FromDateTime(_dtpDenNgay.Value) : null;

        if (denNgay.HasValue && denNgay.Value < tuNgay)
        {
            _lblThongBao.Text = "Ngày kết thúc phải sau hoặc bằng ngày áp dụng từ.";
            return;
        }

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        ApiResult<ScheduleListItemDto> result;

        if (_existing is null)
        {
            result = await AppSession.Api.PostAsync<ScheduleListItemDto>("admin/schedules", new
            {
                maLopHoc = lop.MaLopHoc,
                maPhongHoc = phong.MaPhongHoc,
                thuTrongTuan = _cboThu.SelectedIndex + 1,
                gioBatDau = gioBatDau.ToString("HH:mm:ss"),
                gioKetThuc = gioKetThuc.ToString("HH:mm:ss"),
                ngayApDungTu = tuNgay.ToString("yyyy-MM-dd"),
                ngayApDungDen = denNgay?.ToString("yyyy-MM-dd")
            });
        }
        else
        {
            result = await AppSession.Api.PatchAsync<ScheduleListItemDto>($"admin/schedules/{_existing.MaLichHoc}", new
            {
                maPhongHoc = phong.MaPhongHoc,
                thuTrongTuan = _cboThu.SelectedIndex + 1,
                gioBatDau = gioBatDau.ToString("HH:mm:ss"),
                gioKetThuc = gioKetThuc.ToString("HH:mm:ss"),
                ngayApDungTu = tuNgay.ToString("yyyy-MM-dd"),
                ngayApDungDen = denNgay?.ToString("yyyy-MM-dd")
            });
        }

        UseWaitCursor = false;
        _btnLuu.Enabled = true;

        if (!result.Success)
        {
            _lblThongBao.Text = string.IsNullOrWhiteSpace(result.ErrorMessage)
                ? "Không lưu được lịch học."
                : result.ErrorMessage;
            return;
        }

        DataChanged = true;
        DialogResult = DialogResult.OK;
        Close();
    }

    private sealed record LopItem(int MaLopHoc, string MoTa)
    {
        public override string ToString() => MoTa;
    }

    private sealed record PhongItem(int MaPhongHoc, string MoTa)
    {
        public override string ToString() => MoTa;
    }
}
