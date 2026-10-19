using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Chọn khoảng ngày để <b>sinh ca học hàng loạt</b> từ một lịch định kỳ (<c>LichHoc</c>).
/// <para>
/// Gọi <c>POST /api/admin/sessions/generate</c> ở màn hình gọi (UcLichHoc),
/// hộp thoại này chỉ thu thập khoảng ngày và kiểm tra hợp lệ.
/// </para>
/// </summary>
public sealed class FormSinhCaHoc : Form
{
    private readonly ScheduleListItemDto _lich;
    private readonly DateTimePicker _dtTu = new();
    private readonly DateTimePicker _dtDen = new();
    private readonly Label _lblLoi = new();

    public DateOnly TuNgay { get; private set; }

    public DateOnly DenNgay { get; private set; }

    public FormSinhCaHoc(ScheduleListItemDto lich)
    {
        _lich = lich;

        Text = "Sinh ca học";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(540, 340);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
    }

    private void DungGiaoDien()
    {
        Controls.Add(new Label
        {
            Text = "SINH CA HỌC HÀNG LOẠT",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 18)
        });

        Controls.Add(new Label
        {
            Text =
                $"Lớp: {_lich.TenLop}  •  {ThuText(_lich.ThuTrongTuan)}  •  "
                + $"{_lich.GioBatDau:HH\\:mm} – {_lich.GioKetThuc:HH\\:mm}  •  Phòng {_lich.TenPhong}",
            Font = Theme.FontSub,
            ForeColor = Theme.TextMedium,
            AutoSize = false,
            Size = new Size(492, 40),
            Location = new Point(24, 52)
        });

        DatTruong("Từ ngày *", _dtTu, 104);
        DatTruong("Đến ngày *", _dtDen, 168);

        // Mặc định: từ hôm nay (hoặc ngày áp dụng nếu ở tương lai) → hết khoảng áp dụng (hoặc +90 ngày).
        var homNay = DateOnly.FromDateTime(DateTime.Today);
        var tu = _lich.NgayApDungTu > homNay ? _lich.NgayApDungTu : homNay;
        var den = _lich.NgayApDungDen ?? tu.AddDays(90);

        if (den < tu)
        {
            den = tu.AddDays(90);
        }

        _dtTu.Value = tu.ToDateTime(TimeOnly.MinValue);
        _dtDen.Value = den.ToDateTime(TimeOnly.MinValue);

        Controls.Add(new Label
        {
            Text = _lich.NgayApDungDen is { } apDungDen
                ? $"Lịch có hiệu lực {UiHelpers.FormatDate(_lich.NgayApDungTu)} → {UiHelpers.FormatDate(apDungDen)}. "
                  + "Ca học chỉ được sinh trong khoảng này."
                : $"Lịch có hiệu lực từ {UiHelpers.FormatDate(_lich.NgayApDungTu)} (không giới hạn ngày kết thúc).",
            Font = Theme.FontSub,
            ForeColor = Theme.Muted,
            AutoSize = false,
            Size = new Size(492, 34),
            Location = new Point(24, 228)
        });

        _lblLoi.AutoSize = false;
        _lblLoi.Size = new Size(492, 32);
        _lblLoi.Location = new Point(24, 258);
        _lblLoi.ForeColor = Theme.Danger;
        _lblLoi.Font = Theme.FontSub;
        Controls.Add(_lblLoi);

        var btnSinh = UiHelpers.CreatePrimaryButton("Sinh ca học", 150, 36);
        btnSinh.Location = new Point(258, 294);
        btnSinh.Click += (_, _) => Sinh();

        var btnHuy = UiHelpers.CreateSecondaryButton("Hủy bỏ", 110, 36);
        btnHuy.Location = new Point(416, 294);
        btnHuy.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        Controls.Add(btnSinh);
        Controls.Add(btnHuy);

        AcceptButton = btnSinh;
        CancelButton = btnHuy;
    }

    private void DatTruong(string nhan, DateTimePicker control, int y)
    {
        Controls.Add(new Label
        {
            Text = nhan,
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, y)
        });

        control.Format = DateTimePickerFormat.Short;
        control.Font = Theme.FontRegular;
        control.Width = 200;
        control.Location = new Point(24, y + 20);
        Controls.Add(control);
    }

    private void Sinh()
    {
        var tu = DateOnly.FromDateTime(_dtTu.Value);
        var den = DateOnly.FromDateTime(_dtDen.Value);

        if (den < tu)
        {
            _lblLoi.Text = "Đến ngày phải sau hoặc bằng Từ ngày.";
            return;
        }

        if (_lich.NgayApDungDen is { } gioiHan && tu > gioiHan)
        {
            _lblLoi.Text = $"Từ ngày vượt quá hạn áp dụng của lịch ({UiHelpers.FormatDate(gioiHan)}).";
            return;
        }

        TuNgay = tu;
        DenNgay = den;

        DialogResult = DialogResult.OK;
        Close();
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
}
