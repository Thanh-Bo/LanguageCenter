using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Sửa LỊCH HỌC TUẦN của MỘT lớp (2 buổi/tuần = 2 dòng lịch).
/// <para>
/// Thứ học, khung giờ và khoảng ngày áp dụng là THUỘC TÍNH CỦA LỚP nên chỉ xem;
/// form này cho đổi PHÒNG HỌC và TRẠNG THÁI cho cả 2 dòng lịch cùng lúc.
/// </para>
/// </summary>
public sealed class FormSuaLichTuan : Form
{
    private readonly IReadOnlyList<ScheduleListItemDto> _dongs;
    private readonly IReadOnlyList<RoomListItemDto> _rooms;

    private readonly ComboBox _cboPhong = UiHelpers.CreateComboBox(320);
    private readonly ComboBox _cboTrangThai = UiHelpers.CreateComboBox(320);
    private readonly Label _lblThongBao = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Lưu lịch tuần", 150, 36);
    private readonly Button _btnHuy = UiHelpers.CreateSecondaryButton("Hủy bỏ", 110, 36);

    public bool DataChanged { get; private set; }

    public FormSuaLichTuan(
        IReadOnlyList<ScheduleListItemDto> dongs,
        IReadOnlyList<RoomListItemDto> rooms)
    {
        _dongs = dongs;
        _rooms = rooms;

        Text = "Sửa lịch học tuần";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(620, 440);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        NapDuLieu();
    }

    private static string TenThu(int thu) => thu switch
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

    private string ThuText => string.Join(" · ", _dongs
        .Select(x => x.ThuTrongTuan)
        .Distinct()
        .OrderBy(x => x)
        .Select(TenThu));

    private void DungGiaoDien()
    {
        var dau = _dongs[0];

        Controls.Add(new Label
        {
            Text = "SỬA LỊCH HỌC TUẦN",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 18)
        });

        Controls.Add(new Label
        {
            Text = $"Lớp: {dau.TenLop} ({dau.MaLop})",
            Font = Theme.FontBold,
            ForeColor = Theme.TextDark,
            AutoSize = true,
            Location = new Point(24, 54)
        });

        Controls.Add(new Label
        {
            Text = $"Thứ học: {ThuText}        "
                + $"Khung giờ: {dau.GioBatDau:HH\\:mm} - {dau.GioKetThuc:HH\\:mm}        "
                + $"Từ ngày: {UiHelpers.FormatDate(dau.NgayApDungTu)}"
                + (dau.NgayApDungDen is { } den ? $" → {UiHelpers.FormatDate(den)}" : string.Empty),
            Font = Theme.FontSub,
            ForeColor = Theme.Muted,
            AutoSize = true,
            Location = new Point(24, 80)
        });

        Controls.Add(new Label
        {
            Text = "Phòng học *",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, 116)
        });

        _cboPhong.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboPhong.Location = new Point(24, 136);
        Controls.Add(_cboPhong);

        Controls.Add(new Label
        {
            Text = "Trạng thái",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, 186)
        });

        _cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboTrangThai.Location = new Point(24, 206);
        _cboTrangThai.Items.Add("Hoạt động");
        _cboTrangThai.Items.Add("Ngừng áp dụng");
        Controls.Add(_cboTrangThai);

        Controls.Add(new Label
        {
            Text = "Thứ học, khung giờ và khoảng ngày lấy theo LỚP (chỉ xem).\n"
                + "Muốn đổi: Lớp học → Sửa lớp.",
            AutoSize = false,
            Size = new Size(560, 36),
            Font = Theme.FontSub,
            ForeColor = Color.FromArgb(100, 116, 139),
            Location = new Point(24, 246)
        });

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(560, 34);
        _lblThongBao.Location = new Point(24, 288);
        _lblThongBao.Font = Theme.FontSub;
        _lblThongBao.ForeColor = Theme.Danger;
        Controls.Add(_lblThongBao);

        _btnLuu.Location = new Point(320, 350);
        _btnLuu.Click += async (_, _) => await LuuAsync();
        Controls.Add(_btnLuu);

        _btnHuy.Location = new Point(478, 350);
        _btnHuy.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        Controls.Add(_btnHuy);

        AcceptButton = _btnLuu;
        CancelButton = _btnHuy;
    }

    private void NapDuLieu()
    {
        var dau = _dongs[0];

        _cboPhong.Items.Clear();

        foreach (var r in _rooms.OrderBy(r => r.TenPhong))
        {
            _cboPhong.Items.Add(new PhongItem(r.MaPhongHoc, $"{r.TenPhong} (sức chứa {r.SucChua})"));
        }

        for (var i = 0; i < _cboPhong.Items.Count; i++)
        {
            if (_cboPhong.Items[i] is PhongItem it && it.MaPhongHoc == dau.MaPhongHoc)
            {
                _cboPhong.SelectedIndex = i;
                break;
            }
        }

        if (_cboPhong.SelectedIndex < 0 && _cboPhong.Items.Count > 0)
        {
            _cboPhong.SelectedIndex = 0;
        }

        _cboTrangThai.SelectedIndex = _dongs.Any(x => x.TrangThai == "Hoạt động") ? 0 : 1;
    }

    private async Task LuuAsync()
    {
        _lblThongBao.Text = string.Empty;

        if (_cboPhong.SelectedItem is not PhongItem phong)
        {
            _lblThongBao.Text = "Vui lòng chọn phòng học.";
            return;
        }

        var trangThaiMoi = _cboTrangThai.SelectedIndex == 0 ? "Hoạt động" : "Ngừng áp dụng";

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        foreach (var dong in _dongs)
        {
            var suaPhong = await AppSession.Api.PatchAsync<ScheduleListItemDto>(
                $"admin/schedules/{dong.MaLichHoc}",
                new { maPhongHoc = phong.MaPhongHoc });

            if (!suaPhong.Success)
            {
                UseWaitCursor = false;
                _btnLuu.Enabled = true;
                _lblThongBao.Text = suaPhong.ThongDiepDayDu();
                return;
            }

            if (dong.TrangThai != trangThaiMoi)
            {
                var doiTrangThai = await AppSession.Api.PatchAsync<ScheduleListItemDto>(
                    $"admin/schedules/{dong.MaLichHoc}/status",
                    new { trangThai = trangThaiMoi });

                if (!doiTrangThai.Success)
                {
                    UseWaitCursor = false;
                    _btnLuu.Enabled = true;
                    _lblThongBao.Text = doiTrangThai.ThongDiepDayDu();
                    return;
                }
            }
        }

        UseWaitCursor = false;
        _btnLuu.Enabled = true;

        DataChanged = true;
        DialogResult = DialogResult.OK;
        Close();
    }

    private sealed record PhongItem(int MaPhongHoc, string MoTa)
    {
        public override string ToString() => MoTa;
    }
}
