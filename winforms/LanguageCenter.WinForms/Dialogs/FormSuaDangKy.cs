using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Sửa ĐĂNG KÝ HỌC — đổi ưu đãi và ghi chú.
/// <para>Gọi <c>PATCH /api/admin/enrollments/{id}</c>. Server tự tính lại số tiền giảm.</para>
/// </summary>
public sealed class FormSuaDangKy : Form
{
    private readonly EnrollmentListItemDto _item;

    private readonly ComboBox _cboUuDai = UiHelpers.CreateComboBox(460);
    private readonly TextBox _txtGhiChu = UiHelpers.CreateTextBox(460);
    private readonly Label _lblThongBao = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Lưu thay đổi", 150, 36);
    private readonly Button _btnHuy = UiHelpers.CreateSecondaryButton("Hủy", 110, 36);

    public bool DataChanged { get; private set; }

    private sealed record UuDaiItem(int? MaUuDai, string MoTa)
    {
        public override string ToString() => MoTa;
    }

    public FormSuaDangKy(EnrollmentListItemDto item)
    {
        _item = item;

        Text = $"Sửa đăng ký học — {item.TenHocVien}";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(540, 330);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        Load += async (_, _) => await NapUuDaiAsync();
    }

    private void DungGiaoDien()
    {
        Controls.Add(new Label
        {
            Text = "SỬA ĐĂNG KÝ HỌC",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 16)
        });

        Controls.Add(new Label
        {
            Text = $"{_item.MaHocVien} — {_item.TenHocVien} · {_item.TenKhoaHoc}"
                 + $"\nHọc phí gốc {_item.HocPhiGoc:N0} ₫ · Giảm {_item.SoTienGiam:N0} ₫"
                 + $" · Phải đóng {_item.HocPhiSauGiam:N0} ₫",
            Font = Theme.FontSub,
            ForeColor = Theme.Muted,
            AutoSize = false,
            Size = new Size(492, 46),
            Location = new Point(24, 46)
        });

        Controls.Add(new Label
        {
            Text = "Ưu đãi / khuyến mãi",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, 108)
        });

        _cboUuDai.Location = new Point(24, 128);
        _cboUuDai.DropDownHeight = 260;
        _cboUuDai.IntegralHeight = false;
        Controls.Add(_cboUuDai);

        Controls.Add(new Label
        {
            Text = "Ghi chú",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, 168)
        });

        _txtGhiChu.Location = new Point(24, 188);
        Controls.Add(_txtGhiChu);

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(492, 40);
        _lblThongBao.Location = new Point(24, 222);
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Font = Theme.FontSub;
        Controls.Add(_lblThongBao);

        _btnLuu.Location = new Point(244, 272);
        _btnLuu.Click += async (_, _) => await LuuAsync();
        Controls.Add(_btnLuu);

        _btnHuy.Location = new Point(402, 272);
        _btnHuy.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        Controls.Add(_btnHuy);

        AcceptButton = _btnLuu;
        CancelButton = _btnHuy;
    }

    private async Task NapUuDaiAsync()
    {
        _cboUuDai.Items.Clear();
        _cboUuDai.Items.Add(new UuDaiItem(null, "— Không áp dụng ưu đãi —"));

        var result = await AppSession.Api.GetAsync<PagedResult<PromotionListItemDto>>(
            QueryStringBuilder.Build("admin/promotions", ("page", "1"), ("pageSize", "1000")));

        if (result.Success && result.Data is not null)
        {
            foreach (var u in result.Data.Items.Where(x => x.TrangThai == "Hoạt động"))
            {
                var moTa = u.LoaiGiamGia == "Phần trăm"
                    ? $"{u.TenUuDai} — giảm {u.GiaTriGiam:0.##}%"
                    : $"{u.TenUuDai} — giảm {u.GiaTriGiam:N0} ₫";

                _cboUuDai.Items.Add(new UuDaiItem(u.MaUuDai, moTa));
            }
        }

        _cboUuDai.SelectedIndex = 0;
    }

    private async Task LuuAsync()
    {
        _lblThongBao.Text = string.Empty;

        var uuDai = _cboUuDai.SelectedItem as UuDaiItem;
        var boUuDai = uuDai is null || uuDai.MaUuDai is null;

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        var result = await AppSession.Api.PatchAsync<EnrollmentListItemDto>(
            $"admin/enrollments/{_item.MaDangKy}",
            new
            {
                maUuDai = uuDai?.MaUuDai,
                boUuDai,
                ghiChu = _txtGhiChu.Text.Trim()
            });

        UseWaitCursor = false;
        _btnLuu.Enabled = true;

        if (!result.Success)
        {
            _lblThongBao.Text = result.Errors.Count > 0
                ? $"{result.ErrorMessage}\n- {string.Join("\n- ", result.Errors)}"
                : (string.IsNullOrWhiteSpace(result.ErrorMessage) ? "Không cập nhật được đăng ký." : result.ErrorMessage);
            return;
        }

        DataChanged = true;
        DialogResult = DialogResult.OK;
        Close();
    }
}
