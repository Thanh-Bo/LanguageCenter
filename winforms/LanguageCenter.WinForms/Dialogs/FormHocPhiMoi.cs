using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Tạo phiếu học phí cho một đăng ký học — <c>POST /api/admin/tuition-fees</c>.
/// Số tiền được backend copy từ <c>DangKyHoc</c>, client không gửi tiền.
/// </summary>
public sealed class FormHocPhiMoi : Form
{
    private readonly IReadOnlyList<EnrollmentListItemDto> _dangKys;

    private readonly ComboBox _cboDangKy = UiHelpers.CreateComboBox(460);
    private readonly CheckBox _chkCoHan = new();
    private readonly DateTimePicker _dtpHan = new();
    private readonly Label _lblThongTin = new();
    private readonly Label _lblThongBao = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Tạo phiếu học phí", 180, 36);
    private readonly Button _btnHuy = UiHelpers.CreateSecondaryButton("Hủy bỏ", 110, 36);

    public bool DataChanged { get; private set; }

    public FormHocPhiMoi(IReadOnlyList<EnrollmentListItemDto> dangKys)
    {
        _dangKys = dangKys;

        Text = "Tạo phiếu học phí";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(680, 340);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        NapDuLieu();
    }

    private void DungGiaoDien()
    {
        Controls.Add(new Label
        {
            Text = "TẠO PHIẾU HỌC PHÍ",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 18)
        });

        Controls.Add(new Label
        {
            Text = "Đăng ký học *",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, 56)
        });

        _cboDangKy.Location = new Point(24, 76);
        _cboDangKy.Width = 620;
        _cboDangKy.SelectedIndexChanged += (_, _) => CapNhatThongTin();
        Controls.Add(_cboDangKy);

        _lblThongTin.AutoSize = false;
        _lblThongTin.Size = new Size(620, 46);
        _lblThongTin.Location = new Point(24, 112);
        _lblThongTin.Font = Theme.FontSub;
        _lblThongTin.ForeColor = Theme.TextMedium;
        Controls.Add(_lblThongTin);

        _chkCoHan.Text = "Đặt hạn thanh toán";
        _chkCoHan.AutoSize = true;
        _chkCoHan.Location = new Point(24, 172);
        _chkCoHan.CheckedChanged += (_, _) => _dtpHan.Enabled = _chkCoHan.Checked;
        Controls.Add(_chkCoHan);

        _dtpHan.Format = DateTimePickerFormat.Short;
        _dtpHan.Width = 200;
        _dtpHan.Location = new Point(200, 170);
        _dtpHan.Enabled = false;
        _dtpHan.Value = DateTime.Today.AddDays(14);
        Controls.Add(_dtpHan);

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(620, 36);
        _lblThongBao.Location = new Point(24, 214);
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Font = Theme.FontSub;
        Controls.Add(_lblThongBao);

        _btnLuu.Location = new Point(330, 280);
        _btnLuu.Click += async (_, _) => await LuuAsync();

        _btnHuy.Location = new Point(522, 280);
        _btnHuy.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        Controls.Add(_btnLuu);
        Controls.Add(_btnHuy);

        AcceptButton = _btnLuu;
        CancelButton = _btnHuy;
    }

    private void NapDuLieu()
    {
        _cboDangKy.Items.Clear();

        foreach (var dk in _dangKys.OrderBy(d => d.MaHocVien))
        {
            _cboDangKy.Items.Add(new DangKyItem(dk.MaDangKy,
                $"#{dk.MaDangKy} · {dk.MaHocVien} · {dk.TenKhoaHoc} · {dk.TrangThaiDangKy}"));
        }

        if (_cboDangKy.Items.Count == 0)
        {
            _lblThongBao.Text = "Chưa có đăng ký học nào để tạo phiếu.";
            _btnLuu.Enabled = false;
            return;
        }

        _cboDangKy.SelectedIndex = 0;
    }

    private void CapNhatThongTin()
    {
        if (_cboDangKy.SelectedItem is not DangKyItem item)
        {
            _lblThongTin.Text = string.Empty;
            return;
        }

        var dk = _dangKys.FirstOrDefault(x => x.MaDangKy == item.MaDangKy);
        if (dk is null)
        {
            _lblThongTin.Text = string.Empty;
            return;
        }

        _lblThongTin.Text =
            $"Học phí gốc: {dk.HocPhiGoc:N0} đ · Giảm: {dk.SoTienGiam:N0} đ · "
            + $"Phải đóng: {dk.HocPhiSauGiam:N0} đ\n"
            + $"Lớp: {(string.IsNullOrWhiteSpace(dk.MaLop) ? "chưa xếp lớp" : dk.MaLop)}";
    }

    private async Task LuuAsync()
    {
        _lblThongBao.Text = string.Empty;

        if (_cboDangKy.SelectedItem is not DangKyItem item)
        {
            _lblThongBao.Text = "Vui lòng chọn đăng ký học.";
            return;
        }

        var han = _chkCoHan.Checked
            ? DateOnly.FromDateTime(_dtpHan.Value).ToString("yyyy-MM-dd")
            : null;

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        var result = await AppSession.Api.PostAsync<TuitionItemDto>("admin/tuition-fees", new
        {
            maDangKy = item.MaDangKy,
            hanThanhToan = han
        });

        UseWaitCursor = false;
        _btnLuu.Enabled = true;

        if (!result.Success)
        {
            _lblThongBao.Text = string.IsNullOrWhiteSpace(result.ErrorMessage)
                ? "Không tạo được phiếu học phí."
                : result.ErrorMessage;
            return;
        }

        DataChanged = true;
        DialogResult = DialogResult.OK;
        Close();
    }

    private sealed record DangKyItem(int MaDangKy, string MoTa)
    {
        public override string ToString() => MoTa;
    }
}
