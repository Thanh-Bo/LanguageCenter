using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>Sửa thông báo — <c>PATCH /api/admin/notifications/{id}</c>.</summary>
public sealed class FormSuaThongBao : Form
{
    private static readonly string[] LoaiThongBaos =
    [
        "Lịch học", "Học tập", "Học phí", "Khác"
    ];

    private static readonly string[] TrangThais =
    [
        "Đã gửi", "Đã thu hồi"
    ];

    private readonly NotificationListItemDto _thongBao;

    private readonly TextBox _txtTieuDe = UiHelpers.CreateTextBox(300);
    private readonly TextBox _txtNoiDung = new();
    private readonly ComboBox _cboLoai = UiHelpers.CreateComboBox(200);
    private readonly ComboBox _cboTrangThai = UiHelpers.CreateComboBox(200);
    private readonly Label _lblThongBao = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Lưu thay đổi", 150, 36);
    private readonly Button _btnHuy = UiHelpers.CreateSecondaryButton("Hủy bỏ", 110, 36);

    public bool DataChanged { get; private set; }

    public FormSuaThongBao(NotificationListItemDto thongBao)
    {
        _thongBao = thongBao;

        Text = $"Sửa thông báo TB-{thongBao.MaThongBao:D3}";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(640, 520);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        NapDuLieu();
    }

    private void DungGiaoDien()
    {
        Controls.Add(new Label
        {
            Text = $"SỬA THÔNG BÁO TB-{_thongBao.MaThongBao:D3}",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 18)
        });

        Controls.Add(new Label
        {
            Text = $"Người gửi: {_thongBao.TenDangNhapNguoiGui}  ·  Gửi lúc: {UiHelpers.FormatDateTime(_thongBao.ThoiGianGui)}",
            AutoSize = true,
            Font = Theme.FontSub,
            ForeColor = Theme.Muted,
            Location = new Point(24, 48)
        });

        Controls.Add(new Label
        {
            Text = "Tiêu đề *",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, 80)
        });
        _txtTieuDe.Location = new Point(24, 100);
        _txtTieuDe.Width = 590;
        Controls.Add(_txtTieuDe);

        Controls.Add(new Label
        {
            Text = "Nội dung *",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, 140)
        });
        _txtNoiDung.Multiline = true;
        _txtNoiDung.ScrollBars = ScrollBars.Vertical;
        _txtNoiDung.Size = new Size(590, 180);
        _txtNoiDung.Location = new Point(24, 160);
        Controls.Add(_txtNoiDung);

        Controls.Add(new Label
        {
            Text = "Loại thông báo",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, 356)
        });
        _cboLoai.Location = new Point(24, 376);
        foreach (var loai in LoaiThongBaos) _cboLoai.Items.Add(loai);
        Controls.Add(_cboLoai);

        Controls.Add(new Label
        {
            Text = "Trạng thái",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(260, 356)
        });
        _cboTrangThai.Location = new Point(260, 376);
        foreach (var tt in TrangThais) _cboTrangThai.Items.Add(tt);
        Controls.Add(_cboTrangThai);

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(590, 32);
        _lblThongBao.Location = new Point(24, 416);
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Font = Theme.FontSub;
        Controls.Add(_lblThongBao);

        _btnLuu.Location = new Point(344, 462);
        _btnLuu.Click += async (_, _) => await LuuAsync();

        _btnHuy.Location = new Point(502, 462);
        _btnHuy.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        Controls.Add(_btnLuu);
        Controls.Add(_btnHuy);

        AcceptButton = _btnLuu;
        CancelButton = _btnHuy;
    }

    private void NapDuLieu()
    {
        _txtTieuDe.Text = _thongBao.TieuDe;
        _txtNoiDung.Text = _thongBao.NoiDung;
        _cboLoai.SelectedItem = _thongBao.LoaiThongBao;
        if (_cboLoai.SelectedIndex < 0) _cboLoai.SelectedIndex = 0;
        _cboTrangThai.SelectedItem = _thongBao.TrangThai;
        if (_cboTrangThai.SelectedIndex < 0) _cboTrangThai.SelectedIndex = 0;
    }

    private async Task LuuAsync()
    {
        _lblThongBao.Text = string.Empty;

        var tieuDe = _txtTieuDe.Text.Trim();
        var noiDung = _txtNoiDung.Text.Trim();

        if (tieuDe.Length < 3)
        {
            _lblThongBao.Text = "Tiêu đề phải có ít nhất 3 ký tự.";
            return;
        }

        if (noiDung.Length == 0)
        {
            _lblThongBao.Text = "Nội dung không được để trống.";
            return;
        }

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        var result = await AppSession.Api.PatchAsync<NotificationListItemDto>(
            $"admin/notifications/{_thongBao.MaThongBao}",
            new
            {
                tieuDe,
                noiDung,
                loaiThongBao = _cboLoai.SelectedItem?.ToString(),
                trangThai = _cboTrangThai.SelectedItem?.ToString()
            });

        UseWaitCursor = false;
        _btnLuu.Enabled = true;

        if (!result.Success)
        {
            _lblThongBao.Text = string.IsNullOrWhiteSpace(result.ErrorMessage)
                ? "Không lưu được thông báo."
                : result.ErrorMessage;
            return;
        }

        DataChanged = true;
        DialogResult = DialogResult.OK;
        Close();
    }
}
