using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Thêm mới / cập nhật CHƯƠNG TRÌNH đào tạo.
/// <para>Gọi <c>POST /api/admin/programs</c> hoặc <c>PATCH /api/admin/programs/{id}</c>.</para>
/// </summary>
public sealed class FormChuongTrinh : Form
{
    private readonly ProgramListItemDto? _existing;

    private readonly TextBox _txtTen = UiHelpers.CreateTextBox(480);
    private readonly TextBox _txtNhomDoiTuong = UiHelpers.CreateTextBox(480);
    private readonly CheckBox _chkCoDoTuoi = new() { Text = "Giới hạn độ tuổi", AutoSize = true };
    private readonly NumericUpDown _numTuTuoi = new() { Width = 100, Minimum = 0, Maximum = 200, Value = 6 };
    private readonly NumericUpDown _numDenTuoi = new() { Width = 100, Minimum = 0, Maximum = 200, Value = 18 };
    private readonly TextBox _txtMucTieu = UiHelpers.CreateTextBox(480);
    private readonly TextBox _txtMoTa = new()
    {
        Multiline = true,
        ScrollBars = ScrollBars.Vertical,
        Width = 480,
        Height = 70,
        Font = Theme.FontRegular
    };

    private readonly Label _lblThongBao = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Lưu", 140, 36);
    private readonly Button _btnHuy = UiHelpers.CreateSecondaryButton("Hủy", 110, 36);

    public bool DataChanged { get; private set; }

    public FormChuongTrinh(ProgramListItemDto? existing = null)
    {
        _existing = existing;

        Text = existing is null ? "Thêm chương trình đào tạo" : $"Cập nhật chương trình — {existing.TenChuongTrinh}";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(560, 500);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        NapDuLieu();
    }

    private void DungGiaoDien()
    {
        Controls.Add(new Label
        {
            Text = _existing is null ? "THÊM CHƯƠNG TRÌNH ĐÀO TẠO" : "CẬP NHẬT CHƯƠNG TRÌNH ĐÀO TẠO",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 16)
        });

        DatTruong("Tên chương trình *", _txtTen, 56);
        DatTruong("Nhóm đối tượng", _txtNhomDoiTuong, 116);

        _chkCoDoTuoi.Location = new Point(24, 176);
        _chkCoDoTuoi.CheckedChanged += (_, _) =>
        {
            _numTuTuoi.Enabled = _chkCoDoTuoi.Checked;
            _numDenTuoi.Enabled = _chkCoDoTuoi.Checked;
        };
        Controls.Add(_chkCoDoTuoi);

        _numTuTuoi.Location = new Point(200, 174);
        _numTuTuoi.Enabled = false;
        Controls.Add(_numTuTuoi);

        Controls.Add(new Label
        {
            Text = "đến",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(310, 178)
        });

        _numDenTuoi.Location = new Point(352, 174);
        _numDenTuoi.Enabled = false;
        Controls.Add(_numDenTuoi);

        DatTruong("Mục tiêu cam kết", _txtMucTieu, 212);

        Controls.Add(new Label
        {
            Text = "Mô tả tổng quan",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, 272)
        });
        _txtMoTa.Location = new Point(24, 292);
        Controls.Add(_txtMoTa);

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(512, 40);
        _lblThongBao.Location = new Point(24, 374);
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Font = Theme.FontSub;
        Controls.Add(_lblThongBao);

        _btnLuu.Location = new Point(266, 428);
        _btnLuu.Click += async (_, _) => await LuuAsync();
        Controls.Add(_btnLuu);

        _btnHuy.Location = new Point(414, 428);
        _btnHuy.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
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
        if (_existing is null)
        {
            return;
        }

        _txtTen.Text = _existing.TenChuongTrinh;
        _txtNhomDoiTuong.Text = _existing.NhomDoiTuong ?? string.Empty;
        _txtMucTieu.Text = _existing.MucTieu ?? string.Empty;
        _txtMoTa.Text = _existing.MoTa ?? string.Empty;

        if (_existing.DoTuoiTu is not null || _existing.DoTuoiDen is not null)
        {
            _chkCoDoTuoi.Checked = true;
            _numTuTuoi.Value = Math.Clamp(_existing.DoTuoiTu ?? 0, 0, 200);
            _numDenTuoi.Value = Math.Clamp(_existing.DoTuoiDen ?? 0, 0, 200);
        }
    }

    private async Task LuuAsync()
    {
        _lblThongBao.Text = string.Empty;

        var ten = _txtTen.Text.Trim();

        if (ten.Length < 2)
        {
            _lblThongBao.Text = "Tên chương trình phải có ít nhất 2 ký tự.";
            return;
        }

        var tuTuoi = _chkCoDoTuoi.Checked ? (int?)_numTuTuoi.Value : null;
        var denTuoi = _chkCoDoTuoi.Checked ? (int?)_numDenTuoi.Value : null;

        if (tuTuoi is { } t1 && denTuoi is { } t2 && t2 < t1)
        {
            _lblThongBao.Text = "Độ tuổi đến phải lớn hơn hoặc bằng độ tuổi từ.";
            return;
        }

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        var result = _existing is null
            ? await AppSession.Api.PostAsync<ProgramListItemDto>("admin/programs", new
            {
                tenChuongTrinh = ten,
                nhomDoiTuong = string.IsNullOrWhiteSpace(_txtNhomDoiTuong.Text) ? null : _txtNhomDoiTuong.Text.Trim(),
                doTuoiTu = tuTuoi,
                doTuoiDen = denTuoi,
                mucTieu = string.IsNullOrWhiteSpace(_txtMucTieu.Text) ? null : _txtMucTieu.Text.Trim(),
                moTa = string.IsNullOrWhiteSpace(_txtMoTa.Text) ? null : _txtMoTa.Text.Trim()
            })
            : await AppSession.Api.PatchAsync<ProgramListItemDto>($"admin/programs/{_existing.MaChuongTrinh}", new
            {
                tenChuongTrinh = ten,
                nhomDoiTuong = _txtNhomDoiTuong.Text.Trim(),
                doTuoiTu = tuTuoi,
                doTuoiDen = denTuoi,
                mucTieu = _txtMucTieu.Text.Trim(),
                moTa = _txtMoTa.Text.Trim()
            });

        UseWaitCursor = false;
        _btnLuu.Enabled = true;

        if (!result.Success)
        {
            _lblThongBao.Text = result.Errors.Count > 0
                ? $"{result.ErrorMessage}\n- {string.Join("\n- ", result.Errors)}"
                : (string.IsNullOrWhiteSpace(result.ErrorMessage) ? "Không lưu được chương trình." : result.ErrorMessage);
            return;
        }

        DataChanged = true;
        DialogResult = DialogResult.OK;
        Close();
    }
}
