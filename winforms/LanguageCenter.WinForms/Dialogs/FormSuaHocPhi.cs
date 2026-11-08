using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Sửa PHIẾU HỌC PHÍ — hiện chỉ cho đổi HẠN THANH TOÁN và đồng bộ lại số tiền từ đăng ký.
/// <para>Gọi <c>PATCH /api/admin/tuition-fees/{id}</c>.</para>
/// </summary>
public sealed class FormSuaHocPhi : Form
{
    private readonly TuitionItemDto _item;

    private readonly CheckBox _chkCoHan = new() { Text = "Có hạn thanh toán", AutoSize = true };
    private readonly DateTimePicker _dtpHan = new() { Width = 200, Format = DateTimePickerFormat.Short };
    private readonly CheckBox _chkDongBo = new()
    {
        Text = "Đồng bộ lại số tiền giảm từ đăng ký học (áp dụng ưu đãi mới nhất)",
        AutoSize = true
    };

    private readonly Label _lblThongBao = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Lưu thay đổi", 150, 36);
    private readonly Button _btnHuy = UiHelpers.CreateSecondaryButton("Hủy", 110, 36);

    public bool DataChanged { get; private set; }

    public FormSuaHocPhi(TuitionItemDto item)
    {
        _item = item;

        Text = $"Sửa phiếu học phí — {item.HoTenHocVien}";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(580, 320);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
    }

    private void DungGiaoDien()
    {
        Controls.Add(new Label
        {
            Text = "SỬA PHIẾU HỌC PHÍ",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 16)
        });

        Controls.Add(new Label
        {
            Text = $"{_item.MaHocPhi} · {_item.HoTenHocVien} · {_item.TenKhoaHoc}"
                 + $"\nPhải đóng {_item.SoTienPhaiDong:N0} ₫ · Đã đóng {_item.SoTienDaDong:N0} ₫"
                 + $" · Còn thiếu {_item.SoTienConThieu:N0} ₫",
            Font = Theme.FontSub,
            ForeColor = Theme.Muted,
            AutoSize = false,
            Size = new Size(530, 46),
            Location = new Point(24, 46)
        });

        _chkCoHan.Location = new Point(24, 104);
        _chkCoHan.Checked = _item.HanThanhToan is not null;
        Controls.Add(_chkCoHan);

        _dtpHan.Location = new Point(24, 128);
        _dtpHan.Value = _item.HanThanhToan?.ToDateTime(TimeOnly.MinValue) ?? DateTime.Today.AddDays(14);
        _dtpHan.Enabled = _chkCoHan.Checked;
        Controls.Add(_dtpHan);

        _chkCoHan.CheckedChanged += (_, _) => _dtpHan.Enabled = _chkCoHan.Checked;

        _chkDongBo.Location = new Point(24, 172);
        Controls.Add(_chkDongBo);

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(530, 44);
        _lblThongBao.Location = new Point(24, 202);
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Font = Theme.FontSub;
        Controls.Add(_lblThongBao);

        _btnLuu.Location = new Point(284, 258);
        _btnLuu.Click += async (_, _) => await LuuAsync();
        Controls.Add(_btnLuu);

        _btnHuy.Location = new Point(442, 258);
        _btnHuy.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        Controls.Add(_btnHuy);

        AcceptButton = _btnLuu;
        CancelButton = _btnHuy;
    }

    private async Task LuuAsync()
    {
        _lblThongBao.Text = string.Empty;

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        var result = await AppSession.Api.PatchAsync<TuitionItemDto>(
            $"admin/tuition-fees/{_item.MaHocPhi}",
            new
            {
                hanThanhToan = _chkCoHan.Checked
                    ? DateOnly.FromDateTime(_dtpHan.Value).ToString("yyyy-MM-dd")
                    : null,
                xoaHanThanhToan = !_chkCoHan.Checked,
                dongBoTuDangKy = _chkDongBo.Checked
            });

        UseWaitCursor = false;
        _btnLuu.Enabled = true;

        if (!result.Success)
        {
            _lblThongBao.Text = result.Errors.Count > 0
                ? $"{result.ErrorMessage}\n- {string.Join("\n- ", result.Errors)}"
                : (string.IsNullOrWhiteSpace(result.ErrorMessage) ? "Không cập nhật được phiếu học phí." : result.ErrorMessage);
            return;
        }

        DataChanged = true;
        DialogResult = DialogResult.OK;
        Close();
    }
}
