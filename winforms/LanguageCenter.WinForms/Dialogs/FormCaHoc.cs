using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>Ca học thực tế — <c>POST|PATCH /api/admin/sessions</c>.</summary>
public sealed class FormCaHoc : Form
{
    private readonly SessionListItemDto? _existing;
    private readonly IReadOnlyList<ClassListItemDto> _classes;
    private readonly IReadOnlyList<RoomListItemDto> _rooms;
    private readonly IReadOnlyList<ScheduleListItemDto> _schedules;
    private readonly IReadOnlyList<TeacherListItemDto> _teachers;

    private readonly ComboBox _cboLop = UiHelpers.CreateComboBox(320);
    private readonly ComboBox _cboLich = UiHelpers.CreateComboBox(320);
    private readonly ComboBox _cboPhong = UiHelpers.CreateComboBox(320);
    private readonly ComboBox _cboGiaoVien = UiHelpers.CreateComboBox(320);
    private readonly ComboBox _cboTroGiang = UiHelpers.CreateComboBox(320);
    private readonly DateTimePicker _dtpNgayHoc = new();
    private readonly DateTimePicker _dtpBatDau = new();
    private readonly DateTimePicker _dtpKetThuc = new();
    private readonly TextBox _txtNoiDung = new();
    private readonly TextBox _txtLyDo = UiHelpers.CreateTextBox(320);
    private readonly Label _lblThongBao = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Lưu ca học", 150, 36);
    private readonly Button _btnHuy = UiHelpers.CreateSecondaryButton("Hủy bỏ", 110, 36);

    public bool DataChanged { get; private set; }

    public FormCaHoc(
        IReadOnlyList<ClassListItemDto> classes,
        IReadOnlyList<RoomListItemDto> rooms,
        IReadOnlyList<ScheduleListItemDto> schedules,
        IReadOnlyList<TeacherListItemDto> teachers,
        SessionListItemDto? existing = null)
    {
        _classes = classes;
        _rooms = rooms;
        _schedules = schedules;
        _teachers = teachers;
        _existing = existing;

        Text = existing is null ? "Thêm ca học" : "Cập nhật ca học";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(600, 740);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        NapDuLieu();

        if (existing is not null) NapCaHocDangSua();
    }

    private void DungGiaoDien()
    {
        Controls.Add(new Label
        {
            Text = _existing is null ? "THÊM CA HỌC THỰC TẾ" : "CẬP NHẬT CA HỌC",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 18)
        });

        DatTruong("Lớp học *", _cboLop, 56);
        DatTruong("Lịch định kỳ (để trống nếu là ca phát sinh)", _cboLich, 116);
        DatTruong("Phòng học *", _cboPhong, 176);
        DatTruong("Giáo viên thực tế", _cboGiaoVien, 236);
        DatTruong("Trợ giảng thực tế", _cboTroGiang, 296);

        _dtpNgayHoc.Format = DateTimePickerFormat.Short;
        _dtpNgayHoc.Width = 320;
        DatTruong("Ngày học", _dtpNgayHoc, 356);

        _dtpBatDau.Format = DateTimePickerFormat.Time;
        _dtpBatDau.ShowUpDown = true;
        _dtpBatDau.Width = 140;
        _dtpBatDau.Value = DateTime.Today.AddHours(18);
        _dtpBatDau.Location = new Point(24, 416);
        Controls.Add(new Label
        {
            Text = "Giờ bắt đầu",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, 396)
        });
        Controls.Add(_dtpBatDau);

        _dtpKetThuc.Format = DateTimePickerFormat.Time;
        _dtpKetThuc.ShowUpDown = true;
        _dtpKetThuc.Width = 140;
        _dtpKetThuc.Value = DateTime.Today.AddHours(19).AddMinutes(30);
        _dtpKetThuc.Location = new Point(200, 416);
        Controls.Add(new Label
        {
            Text = "Giờ kết thúc",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(200, 396)
        });
        Controls.Add(_dtpKetThuc);

        _txtNoiDung.Multiline = true;
        _txtNoiDung.ScrollBars = ScrollBars.Vertical;
        _txtNoiDung.Size = new Size(552, 80);
        _txtNoiDung.Location = new Point(24, 476);
        Controls.Add(new Label
        {
            Text = "Nội dung bài học",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, 456)
        });
        Controls.Add(_txtNoiDung);

        DatTruong("Lý do thay đổi (nếu có)", _txtLyDo, 566);

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(552, 34);
        _lblThongBao.Location = new Point(24, 626);
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Font = Theme.FontSub;
        Controls.Add(_lblThongBao);

        _btnLuu.Location = new Point(304, 672);
        _btnLuu.Click += async (_, _) => await LuuAsync();

        _btnHuy.Location = new Point(462, 672);
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
        _cboLop.SelectedIndexChanged += (_, _) => NapLichCuaLop();

        _cboPhong.Items.Clear();
        foreach (var r in _rooms.OrderBy(r => r.TenPhong))
        {
            _cboPhong.Items.Add(new PhongItem(r.MaPhongHoc, $"{r.TenPhong} (sức chứa {r.SucChua})"));
        }
        if (_cboPhong.Items.Count > 0) _cboPhong.SelectedIndex = 0;

        _cboGiaoVien.Items.Clear();
        _cboGiaoVien.Items.Add("— Không chỉ định —");
        foreach (var gv in _teachers.OrderBy(t => t.MaGiaoVien))
        {
            _cboGiaoVien.Items.Add(new GiaoVienItem(gv.MaGiaoVien,
                $"{gv.MaGiaoVien} — {gv.HoTen} ({gv.LoaiGiaoVien})"));
        }
        _cboGiaoVien.SelectedIndex = 0;

        _cboTroGiang.Items.Clear();
        _cboTroGiang.Items.Add("— Không chỉ định —");
        foreach (var gv in _teachers.Where(t => t.LoaiGiaoVien == TeacherTypes.TroGiang).OrderBy(t => t.MaGiaoVien))
        {
            _cboTroGiang.Items.Add(new GiaoVienItem(gv.MaGiaoVien, $"{gv.MaGiaoVien} — {gv.HoTen}"));
        }
        _cboTroGiang.SelectedIndex = 0;

        NapLichCuaLop();
    }

    private void NapLichCuaLop()
    {
        _cboLich.Items.Clear();
        _cboLich.Items.Add("— Ca phát sinh ngoài lịch —");

        if (_cboLop.SelectedItem is LopItem lop)
        {
            foreach (var lich in _schedules.Where(s => s.MaLopHoc == lop.MaLopHoc))
            {
                _cboLich.Items.Add(new LichItem(lich.MaLichHoc, $"{ThuText(lich.ThuTrongTuan)} · "
                    + $"{lich.GioBatDau:HH\\:mm}–{lich.GioKetThuc:HH\\:mm} · {lich.TenPhong}"));
            }
        }

        _cboLich.SelectedIndex = 0;
    }

    private static string ThuText(int thu) => thu switch
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

    private void NapCaHocDangSua()
    {
        if (_existing is null) return;

        for (var i = 0; i < _cboLop.Items.Count; i++)
        {
            if (_cboLop.Items[i] is LopItem it && it.MaLopHoc == _existing.MaLopHoc)
            {
                _cboLop.SelectedIndex = i;
                break;
            }
        }
        _cboLop.Enabled = false;
        _cboLich.Enabled = false; // PATCH không cho đổi lịch

        for (var i = 0; i < _cboPhong.Items.Count; i++)
        {
            if (_cboPhong.Items[i] is PhongItem it && it.MaPhongHoc == _existing.MaPhongHoc)
            {
                _cboPhong.SelectedIndex = i;
                break;
            }
        }

        ChonGiaoVien(_cboGiaoVien, _existing.TenGiaoVienThucTe);
        ChonTroGiang(_cboTroGiang, _existing.TenTroGiangThucTe);

        _dtpNgayHoc.Value = _existing.NgayHoc.ToDateTime(TimeOnly.MinValue);
        _dtpBatDau.Value = DateTime.Today.Add(_existing.GioBatDau.ToTimeSpan());
        _dtpKetThuc.Value = DateTime.Today.Add(_existing.GioKetThuc.ToTimeSpan());
        _txtNoiDung.Text = _existing.NoiDungBaiHoc ?? string.Empty;
    }

    private static void ChonGiaoVien(ComboBox cbo, string? tenGiaoVien)
    {
        if (string.IsNullOrWhiteSpace(tenGiaoVien)) return;

        for (var i = 0; i < cbo.Items.Count; i++)
        {
            if (cbo.Items[i] is GiaoVienItem it && it.MoTa.Contains(tenGiaoVien, StringComparison.OrdinalIgnoreCase))
            {
                cbo.SelectedIndex = i;
                return;
            }
        }
    }

    private static void ChonTroGiang(ComboBox cbo, string? tenTroGiang) => ChonGiaoVien(cbo, tenTroGiang);

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

        var gioBatDau = TimeOnly.FromDateTime(_dtpBatDau.Value);
        var gioKetThuc = TimeOnly.FromDateTime(_dtpKetThuc.Value);

        if (gioKetThuc <= gioBatDau)
        {
            _lblThongBao.Text = "Giờ kết thúc phải sau giờ bắt đầu.";
            return;
        }

        var ngayHoc = DateOnly.FromDateTime(_dtpNgayHoc.Value);
        var maGiaoVien = (_cboGiaoVien.SelectedItem as GiaoVienItem)?.MaGiaoVien;
        var maTroGiang = (_cboTroGiang.SelectedItem as GiaoVienItem)?.MaGiaoVien;
        var noiDung = string.IsNullOrWhiteSpace(_txtNoiDung.Text) ? null : _txtNoiDung.Text.Trim();
        var lyDo = string.IsNullOrWhiteSpace(_txtLyDo.Text) ? null : _txtLyDo.Text.Trim();

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        ApiResult<SessionListItemDto> result;

        if (_existing is null)
        {
            var maLichHoc = (_cboLich.SelectedItem as LichItem)?.MaLichHoc;

            result = await AppSession.Api.PostAsync<SessionListItemDto>("admin/sessions", new
            {
                maLopHoc = lop.MaLopHoc,
                maLichHoc,
                maPhongHoc = phong.MaPhongHoc,
                maGiaoVienThucTe = maGiaoVien,
                maTroGiangThucTe = maTroGiang,
                ngayHoc = ngayHoc.ToString("yyyy-MM-dd"),
                gioBatDau = gioBatDau.ToString("HH:mm:ss"),
                gioKetThuc = gioKetThuc.ToString("HH:mm:ss"),
                noiDungBaiHoc = noiDung,
                lyDoThayDoi = lyDo
            });
        }
        else
        {
            result = await AppSession.Api.PatchAsync<SessionListItemDto>($"admin/sessions/{_existing.MaCaHoc}", new
            {
                maPhongHoc = phong.MaPhongHoc,
                maGiaoVienThucTe = maGiaoVien,
                maTroGiangThucTe = maTroGiang,
                ngayHoc = ngayHoc.ToString("yyyy-MM-dd"),
                gioBatDau = gioBatDau.ToString("HH:mm:ss"),
                gioKetThuc = gioKetThuc.ToString("HH:mm:ss"),
                noiDungBaiHoc = noiDung,
                lyDoThayDoi = lyDo
            });
        }

        UseWaitCursor = false;
        _btnLuu.Enabled = true;

        if (!result.Success)
        {
            _lblThongBao.Text = string.IsNullOrWhiteSpace(result.ErrorMessage)
                ? "Không lưu được ca học."
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

    private sealed record LichItem(int MaLichHoc, string MoTa)
    {
        public override string ToString() => MoTa;
    }

    private sealed record GiaoVienItem(string MaGiaoVien, string MoTa)
    {
        public override string ToString() => MoTa;
    }
}
