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
    private readonly Label _lblGoiY = new();

    private bool _chiXemThoiGian;

    /// <summary>Mã các lớp ĐÃ có lịch học — không cho tạo thêm lịch thứ 2 cho cùng lớp.</summary>
    private HashSet<int> _lopDaLich = new();

    /// <summary>Thứ đã được xếp lịch của từng lớp (MaLopHoc → các thứ).</summary>
    private readonly Dictionary<int, HashSet<int>> _thuDaLich = new();

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
        ClientSize = new Size(600, 590);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        NapDuLieu();
        ApDungChiXemThoiGian();

        // Tải danh sách lịch hiện có để ẩn những lớp đã được xếp lịch.
        Shown += async (_, _) =>
        {
            if (_existing is null) await NapLopDaLichAsync();
        };
    }

    /// <summary>
    /// Tải toàn bộ lịch học hiện có, ghi nhớ lớp nào đã có lịch rồi dựng lại combo Lớp học.
    /// <para>Mỗi lớp chỉ cần 1 lần xếp lịch (nhiều thứ thì thêm nhiều dòng lịch cho lớp đó,
    /// nhưng KHÔNG tạo lại từ đầu cho lớp đã có).</para>
    /// </summary>
    private async Task NapLopDaLichAsync()
    {
        var result = await AppSession.Api.GetAsync<PagedResult<ScheduleListItemDto>>(
            QueryStringBuilder.Build("admin/schedules", ("page", "1"), ("pageSize", "500")));

        if (!result.Success || result.Data is null)
        {
            return; // giữ nguyên danh sách lớp ban đầu
        }

        _lopDaLich = result.Data.Items.Select(x => x.MaLopHoc).ToHashSet();

        _thuDaLich.Clear();

        foreach (var lich in result.Data.Items)
        {
            if (!_thuDaLich.TryGetValue(lich.MaLopHoc, out var ds))
            {
                ds = [];
                _thuDaLich[lich.MaLopHoc] = ds;
            }

            ds.Add(lich.ThuTrongTuan);
        }

        NapComboLop();
        NapComboThu();

        if (_cboLop.Items.Count == 0)
        {
            _lblThongBao.Text =
                "Tất cả lớp học đã đủ 2 lịch (2 buổi/tuần). Hãy chọn dòng trong bảng để Sửa lịch "
                + "thay vì tạo mới.";
            _btnLuu.Enabled = false;
            _lblGoiY.Text = string.Empty;
        }
    }

    /// <summary>Số thứ học của lớp còn THIẾU lịch (mỗi lớp học 2 thứ/tuần).</summary>
    private int SoThuConThieu(ClassListItemDto lop)
    {
        var dsThu = new HashSet<int>();

        if (lop.ThuBuoi1 is { } thu1) dsThu.Add(thu1);
        if (lop.ThuBuoi2 is { } thu2) dsThu.Add(thu2);

        // Lớp chưa khai 2 thứ → vẫn cho xếp tối đa 2 dòng lịch.
        var tongSoThu = dsThu.Count > 0 ? dsThu.Count : 2;

        var daCo = _thuDaLich.TryGetValue(lop.MaLopHoc, out var thuDaLich) ? thuDaLich.Count : 0;

        return tongSoThu - daCo;
    }

    /// <summary>Dựng combo Lớp học: bỏ qua các lớp đã có lịch (trừ khi đang SỬA lịch của lớp đó).</summary>
    private void NapComboLop()
    {
        var dangChon = (_cboLop.SelectedItem as LopItem)?.MaLopHoc;

        IEnumerable<ClassListItemDto> nguon = _classes;

        if (_existing is null)
        {
            // Mỗi lớp cần ĐỦ 2 thứ (2 buổi/tuần) — chỉ ẩn khi đã đủ, không ẩn sau dòng đầu tiên.
            nguon = nguon.Where(c => SoThuConThieu(c) > 0);
        }

        _cboLop.Items.Clear();

        foreach (var c in nguon.OrderBy(c => c.MaLop))
        {
            _cboLop.Items.Add(new LopItem(c.MaLopHoc, $"{c.MaLop} — {c.TenLop}"));
        }

        if (_cboLop.Items.Count == 0)
        {
            return;
        }

        var viTri = 0;

        if (dangChon is { } maLopHoc)
        {
            for (var i = 0; i < _cboLop.Items.Count; i++)
            {
                if (_cboLop.Items[i] is LopItem it && it.MaLopHoc == maLopHoc)
                {
                    viTri = i;
                    break;
                }
            }
        }

        _cboLop.SelectedIndex = viTri;
    }

    /// <summary>
    /// Giờ bắt đầu/kết thúc và khoảng ngày áp dụng là THUỘC TÍNH CỦA LỚP →
    /// form này chỉ cho XEM, muốn đổi phải sửa ở màn Lớp học.
    /// </summary>
    private void ApDungChiXemThoiGian()
    {
        _chiXemThoiGian = true;

        _dtpGioBatDau.Enabled = false;
        _dtpGioKetThuc.Enabled = false;
        _dtpTuNgay.Enabled = false;
        _chkCoNgayKetThuc.Enabled = false;
        _dtpDenNgay.Enabled = false;

        _lblGoiY.Text = "Giờ học và khoảng ngày lấy theo LỚP (chỉ xem). Muốn đổi: Lớp học → Sửa lớp.";
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
        _chkCoNgayKetThuc.CheckedChanged += (_, _) =>
            _dtpDenNgay.Enabled = !_chiXemThoiGian && _chkCoNgayKetThuc.Checked;
        Controls.Add(_chkCoNgayKetThuc);

        _dtpDenNgay.Format = DateTimePickerFormat.Short;
        _dtpDenNgay.Width = 320;
        _dtpDenNgay.Enabled = false;
        _dtpDenNgay.Location = new Point(24, 396);
        Controls.Add(_dtpDenNgay);

        // Ghi chú: giờ & khoảng ngày lấy theo lớp → chỉ xem.
        _lblGoiY.AutoSize = false;
        _lblGoiY.Size = new Size(552, 20);
        _lblGoiY.Location = new Point(24, 424);
        _lblGoiY.Font = Theme.FontSub;
        _lblGoiY.ForeColor = Color.FromArgb(100, 116, 139);
        Controls.Add(_lblGoiY);

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
        NapComboLop();

        _cboPhong.Items.Clear();
        foreach (var r in _rooms.OrderBy(r => r.TenPhong))
        {
            _cboPhong.Items.Add(new PhongItem(r.MaPhongHoc, $"{r.TenPhong} (sức chứa {r.SucChua})"));
        }
        if (_cboPhong.Items.Count > 0) _cboPhong.SelectedIndex = 0;

        if (_existing is null)
        {
            // Đổi lớp → tự điền giờ học, khoảng ngày và 2 thứ học của lớp.
            _cboLop.SelectedIndexChanged += (_, _) =>
            {
                DienGoiYGioHoc();
                NapComboThu();
            };

            DienGoiYGioHoc();
            NapComboThu();
            return;
        }

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

        NapComboThu();
        ChonThu(_existing.ThuTrongTuan);
        _dtpGioBatDau.Value = DateTime.Today.Add(_existing.GioBatDau.ToTimeSpan());        _dtpGioKetThuc.Value = DateTime.Today.Add(_existing.GioKetThuc.ToTimeSpan());
        _dtpTuNgay.Value = _existing.NgayApDungTu.ToDateTime(TimeOnly.MinValue);

        if (_existing.NgayApDungDen.HasValue)
        {
            _chkCoNgayKetThuc.Checked = true;
            _dtpDenNgay.Value = _existing.NgayApDungDen.Value.ToDateTime(TimeOnly.MinValue);
        }
    }

    /// <summary>
    /// Gợi ý sẵn dữ liệu theo LỚP đang chọn: giờ bắt đầu/kết thúc lấy từ khung giờ của lớp,
    /// ngày áp dụng lấy theo Ngày bắt đầu → Ngày kết thúc dự kiến của lớp.
    /// (Chỉ áp dụng khi TẠO MỚI — chế độ sửa giữ nguyên dữ liệu đã lưu.)
    /// </summary>
    private void DienGoiYGioHoc()
    {
        if (_existing is not null)
        {
            return;
        }

        if (_cboLop.SelectedItem is not LopItem item)
        {
            return;
        }

        var lop = _classes.FirstOrDefault(c => c.MaLopHoc == item.MaLopHoc);

        if (lop is null)
        {
            return;
        }

        if (lop.GioBatDau is { } gioBatDau)
        {
            _dtpGioBatDau.Value = DateTime.Today.Add(gioBatDau.ToTimeSpan());
        }

        if (lop.GioKetThuc is { } gioKetThuc)
        {
            _dtpGioKetThuc.Value = DateTime.Today.Add(gioKetThuc.ToTimeSpan());
        }

        if (lop.NgayBatDau != default)
        {
            _dtpTuNgay.Value = lop.NgayBatDau.ToDateTime(TimeOnly.MinValue);
        }

        if (lop.NgayKetThucDuKien != default)
        {
            _chkCoNgayKetThuc.Checked = true;
            _dtpDenNgay.Value = lop.NgayKetThucDuKien.ToDateTime(TimeOnly.MinValue);
        }

        if (lop.GioBatDau is null || lop.GioKetThuc is null)
        {
            _lblThongBao.Text =
                $"Lớp '{lop.MaLop}' chưa có khung giờ học. "
                + "Vào Lớp học → Sửa lớp để đặt Giờ bắt đầu / Giờ kết thúc trước.";
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

        var thuTrongTuan = ThuDangChon();

        if (thuTrongTuan is null)
        {
            UseWaitCursor = false;
            _btnLuu.Enabled = true;
            _lblThongBao.Text = "Vui lòng chọn thứ trong tuần.";
            return;
        }

        ApiResult<ScheduleListItemDto> result;

        if (_existing is null)
        {
            result = await AppSession.Api.PostAsync<ScheduleListItemDto>("admin/schedules", new
            {
                maLopHoc = lop.MaLopHoc,
                maPhongHoc = phong.MaPhongHoc,
                thuTrongTuan = thuTrongTuan.Value,
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
                thuTrongTuan = thuTrongTuan.Value,
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

    /// <summary>
    /// Nạp combo THỨ THEO LỚP: lớp chỉ học 2 thứ cố định (ThuBuoi1 / ThuBuoi2),
    /// nên chỉ cho chọn trong 2 thứ đó. Mặc định chọn thứ CHƯA được xếp lịch.
    /// </summary>
    private void NapComboThu()
    {
        if (_cboLop.SelectedItem is not LopItem item)
        {
            _cboThu.Items.Clear();
            return;
        }

        var thuDangChon = ThuDangChon();

        var lop = _classes.FirstOrDefault(c => c.MaLopHoc == item.MaLopHoc);

        var dsThu = new List<int>();

        if (lop?.ThuBuoi1 is { } thu1) dsThu.Add(thu1);
        if (lop?.ThuBuoi2 is { } thu2 && !dsThu.Contains(thu2)) dsThu.Add(thu2);

        var lopCoKhaiThu = dsThu.Count > 0;

        if (!lopCoKhaiThu)
        {
            // Lớp chưa khai 2 thứ → cho chọn tự do, kèm cảnh báo.
            dsThu = [1, 2, 3, 4, 5, 6, 7];
        }

        _thuDaLich.TryGetValue(item.MaLopHoc, out var daDung);

        // Ẩn thứ đã xếp lịch (chỉ khi TẠO MỚI) để mỗi lớp đúng 2 dòng lịch.
        var conLai = _existing is null && daDung is not null
            ? dsThu.Where(t => !daDung.Contains(t)).ToList()
            : dsThu;

        if (conLai.Count == 0)
        {
            conLai = dsThu; // đã đủ thứ → cho sửa lại thay vì chặn
        }

        _cboThu.Items.Clear();

        foreach (var thu in conLai)
        {
            _cboThu.Items.Add(new ThuItem(thu, ThuLabels[thu - 1]));
        }

        var viTri = 0;

        if (thuDangChon is { } cu)
        {
            var i = conLai.IndexOf(cu);
            if (i >= 0) viTri = i;
        }
        else if (daDung is not null)
        {
            var i = conLai.FindIndex(t => !daDung.Contains(t));
            if (i >= 0) viTri = i;
        }

        if (_cboThu.Items.Count > 0)
        {
            _cboThu.SelectedIndex = viTri;
        }

        _lblThongBao.Text = lopCoKhaiThu
            ? string.Empty
            : $"Lớp '{lop?.MaLop}' chưa khai 2 thứ học. Vào Lớp học → Sửa lớp để đặt Thứ buổi 1 / Thứ buổi 2.";
    }

    /// <summary>Chọn đúng thứ của lịch đang sửa (thêm vào combo nếu lớp không khai thứ đó).</summary>
    private void ChonThu(int thu)
    {
        for (var i = 0; i < _cboThu.Items.Count; i++)
        {
            if (_cboThu.Items[i] is ThuItem it && it.Thu == thu)
            {
                _cboThu.SelectedIndex = i;
                return;
            }
        }
    }

    private int? ThuDangChon() => (_cboThu.SelectedItem as ThuItem)?.Thu;

    private sealed record ThuItem(int Thu, string MoTa)
    {
        public override string ToString() => MoTa;
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
