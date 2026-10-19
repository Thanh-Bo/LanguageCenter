using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Thêm phòng học mới — gọi <c>POST /api/admin/rooms</c>.
/// <para>
/// Mã phòng là UNIQUE; server tự chuyển chữ HOA và trả 409 nếu trùng.
/// Sức chứa phải &gt; 0.
/// </para>
/// </summary>
public sealed class FormPhongHoc : Form
{
    private static readonly string[] TrangThaiPhong =
        ["Hoạt động", "Đang sử dụng", "Bảo trì", "Ngừng sử dụng"];

    private readonly TextBox _txtMaPhong = UiHelpers.CreateTextBox(320);
    private readonly TextBox _txtTenPhong = UiHelpers.CreateTextBox(320);
    private readonly TextBox _txtViTri = UiHelpers.CreateTextBox(320);
    private readonly NumericUpDown _numSucChua = new();
    private readonly TextBox _txtThietBi = UiHelpers.CreateTextBox(320);
    private readonly ComboBox _cboTrangThai = UiHelpers.CreateComboBox(320);
    private readonly Label _lblThongBao = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Lưu phòng học", 150, 36);
    private readonly Button _btnHuy = UiHelpers.CreateSecondaryButton("Hủy bỏ", 110, 36);

    public RoomListItemDto? PhongVuaTao { get; private set; }

    public FormPhongHoc()
    {
        Text = "Thêm phòng học";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(600, 520);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
    }

    private void DungGiaoDien()
    {
        Controls.Add(new Label
        {
            Text = "THÊM PHÒNG HỌC MỚI",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 18)
        });

        DatTruong("Mã phòng * (ví dụ: P105)", _txtMaPhong, 56);
        _txtMaPhong.CharacterCasing = CharacterCasing.Upper;
        _txtMaPhong.MaxLength = 30;

        DatTruong("Tên phòng * (ví dụ: Phòng 105)", _txtTenPhong, 116);
        _txtTenPhong.MaxLength = 100;

        DatTruong("Vị trí (tầng / toà nhà)", _txtViTri, 176);
        _txtViTri.MaxLength = 255;

        DatTruong("Sức chứa *", _numSucChua, 236);
        _numSucChua.Font = Theme.FontRegular;
        _numSucChua.Width = 140;
        _numSucChua.Minimum = 1;
        _numSucChua.Maximum = 1000;
        _numSucChua.Value = 25;

        DatTruong("Thiết bị chuyên dụng", _txtThietBi, 296);
        _txtThietBi.MaxLength = 1000;

        DatTruong("Trạng thái *", _cboTrangThai, 356);
        _cboTrangThai.Items.AddRange(TrangThaiPhong);
        _cboTrangThai.SelectedIndex = 0;

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(552, 40);
        _lblThongBao.Location = new Point(24, 416);
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Font = Theme.FontSub;
        Controls.Add(_lblThongBao);

        _btnLuu.Location = new Point(304, 464);
        _btnLuu.Click += async (_, _) => await LuuAsync();

        _btnHuy.Location = new Point(462, 464);
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

    private async Task LuuAsync()
    {
        _lblThongBao.Text = string.Empty;

        var maPhong = _txtMaPhong.Text.Trim();
        if (maPhong.Length == 0)
        {
            _lblThongBao.Text = "Vui lòng nhập mã phòng.";
            _txtMaPhong.Focus();
            return;
        }

        var tenPhong = _txtTenPhong.Text.Trim();
        if (tenPhong.Length == 0)
        {
            _lblThongBao.Text = "Vui lòng nhập tên phòng.";
            _txtTenPhong.Focus();
            return;
        }

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        var result = await AppSession.Api.PostAsync<RoomListItemDto>("admin/rooms", new
        {
            maPhong,
            tenPhong,
            viTri = string.IsNullOrWhiteSpace(_txtViTri.Text) ? null : _txtViTri.Text.Trim(),
            sucChua = (int)_numSucChua.Value,
            thietBi = string.IsNullOrWhiteSpace(_txtThietBi.Text) ? null : _txtThietBi.Text.Trim(),
            trangThai = _cboTrangThai.SelectedItem as string
        });

        UseWaitCursor = false;
        _btnLuu.Enabled = true;

        if (!result.Success)
        {
            if (result.StatusCode == 405)
            {
                _lblThongBao.Text = "API đang chạy bản cũ chưa có chức năng này. Hãy khởi động lại backend (dotnet run) rồi thử lại.";
                return;
            }

            var chiTiet = result.Errors.Count > 0 ? $" ({string.Join("; ", result.Errors)})" : string.Empty;
            _lblThongBao.Text = string.IsNullOrWhiteSpace(result.ErrorMessage)
                ? "Không lưu được phòng học." + chiTiet
                : result.ErrorMessage + chiTiet;
            return;
        }

        PhongVuaTao = result.Data;
        DialogResult = DialogResult.OK;
        Close();
    }
}
