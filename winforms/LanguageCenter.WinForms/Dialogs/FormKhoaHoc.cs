using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>Tạo mới / chỉnh sửa khóa học — <c>POST|PATCH /api/admin/courses</c>.</summary>
public sealed class FormKhoaHoc : Form
{
    private readonly CourseListItemDto? _existing;
    private readonly IReadOnlyList<LevelListItemDto> _levels;

    private readonly ComboBox _cboCapDo = UiHelpers.CreateComboBox(320);
    private readonly TextBox _txtTen = UiHelpers.CreateTextBox(320);
    private readonly NumericUpDown _numSoBuoi = new();
    private readonly NumericUpDown _numTongGio = new();
    private readonly NumericUpDown _numHocPhi = new();
    private readonly TextBox _txtNoiDung = new();
    private readonly Label _lblThongBao = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Lưu khóa học", 150, 36);
    private readonly Button _btnHuy = UiHelpers.CreateSecondaryButton("Hủy bỏ", 110, 36);

    public bool DataChanged { get; private set; }

    public FormKhoaHoc(IReadOnlyList<LevelListItemDto> levels, CourseListItemDto? existing = null)
    {
        _levels = levels;
        _existing = existing;

        Text = existing is null ? "Thêm khóa học" : $"Cập nhật khóa học #{existing.MaKhoaHoc}";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(600, 660);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        UI.KiemTraNhap.BatTuNoiChieuCao(this);
        NapDuLieu();
    }

    private void DungGiaoDien()
    {
        Controls.Add(new Label
        {
            Text = _existing is null ? "THÊM KHÓA HỌC MỚI" : "CẬP NHẬT KHÓA HỌC",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 18)
        });

        DatTruong("Cấp độ *", _cboCapDo, 56);
        DatTruong("Tên khóa học *", _txtTen, 116);

        _numSoBuoi.Minimum = 1;
        _numSoBuoi.Maximum = 1000;
        _numSoBuoi.Value = 24;
        _numSoBuoi.Width = 120;
        DatTruong("Số buổi *", _numSoBuoi, 176);

        // Tổng số giờ KHÔNG nhập tay: luôn = Số buổi × 2 giờ/buổi.
        _numTongGio.Minimum = 0;
        _numTongGio.Maximum = 100000;
        _numTongGio.DecimalPlaces = 1;
        _numTongGio.Value = 48;
        _numTongGio.Width = 120;
        _numTongGio.ReadOnly = true;
        _numTongGio.Increment = 0;
        _numTongGio.BackColor = Color.FromArgb(241, 245, 249);
        DatTruong("Tổng số giờ (tự động = Số buổi × 2)", _numTongGio, 236);

        _numHocPhi.Minimum = 0;
        _numHocPhi.Maximum = 1_000_000_000;
        _numHocPhi.Increment = 100_000;
        _numHocPhi.ThousandsSeparator = true;
        _numHocPhi.Value = 5_000_000;
        _numHocPhi.Width = 200;
        DatTruong("Học phí cơ bản (đồng)", _numHocPhi, 296);

        _txtNoiDung.Multiline = true;
        _txtNoiDung.ScrollBars = ScrollBars.Vertical;
        _txtNoiDung.Size = new Size(552, 90);
        Controls.Add(new Label
        {
            Text = "Nội dung / mô tả",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, 356)
        });
        _txtNoiDung.Location = new Point(24, 376);
        Controls.Add(_txtNoiDung);

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(552, 34);
        _lblThongBao.Location = new Point(24, 478);
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Font = Theme.FontSub;
        Controls.Add(_lblThongBao);

        _btnLuu.Location = new Point(304, 522);
        _btnLuu.Click += async (_, _) => await LuuAsync();

        _btnHuy.Location = new Point(462, 522);
        _btnHuy.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        Controls.Add(_btnLuu);
        Controls.Add(_btnHuy);

        AcceptButton = _btnLuu;
        CancelButton = _btnHuy;

        // Tổng số giờ luôn = Số buổi × 2 (không nhập tay).
        _numSoBuoi.ValueChanged += (_, _) => CapNhatTongSoGio();

        CapNhatTongSoGio();
    }

    /// <summary>
    /// Tổng số giờ = Số buổi × 2 (mỗi buổi 2 tiếng, cố định).
    /// <para>
    /// KHÔNG còn ngày bắt đầu / ngày kết thúc ở khóa học: đó là thông tin của TỪNG LỚP
    /// (<c>LopHoc.NgayBatDau</c> / <c>NgayKetThucDuKien</c>).
    /// </para>
    /// </summary>
    private void CapNhatTongSoGio()
    {
        var soBuoi = (int)_numSoBuoi.Value;

        _numTongGio.Value = Math.Clamp(soBuoi * 2, _numTongGio.Minimum, _numTongGio.Maximum);
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
        _cboCapDo.Items.Clear();
        foreach (var lv in _levels.OrderBy(l => l.TenChuongTrinh).ThenBy(l => l.ThuTu))
        {
            _cboCapDo.Items.Add(new CapDoItem(lv.MaCapDo, $"{lv.TenChuongTrinh} · {lv.TenCapDo}"));
        }
        if (_cboCapDo.Items.Count > 0) _cboCapDo.SelectedIndex = 0;

        if (_existing is null) return;

        for (var i = 0; i < _cboCapDo.Items.Count; i++)
        {
            if (_cboCapDo.Items[i] is CapDoItem it && it.MaCapDo == _existing.MaCapDo)
            {
                _cboCapDo.SelectedIndex = i;
                break;
            }
        }
        _cboCapDo.Enabled = false; // API không cho đổi cấp độ của khóa học

        _txtTen.Text = _existing.TenKhoaHoc;
        _numSoBuoi.Value = Math.Clamp(_existing.SoBuoi, 1, 1000);
        _numTongGio.Value = Math.Clamp(_existing.TongSoGio ?? 0, 0, 100000);
        _numHocPhi.Value = Math.Clamp(_existing.HocPhiCoBan, 0, 1_000_000_000);

        CapNhatTongSoGio();
    }

    private async Task LuuAsync()
    {
        _lblThongBao.Text = string.Empty;

        var ten = _txtTen.Text.Trim();
        if (ten.Length < 2)
        {
            _lblThongBao.Text = "Tên khóa học phải có ít nhất 2 ký tự.";
            return;
        }

        if (_cboCapDo.SelectedItem is not CapDoItem capDo)
        {
            _lblThongBao.Text = "Vui lòng chọn cấp độ.";
            return;
        }

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        ApiResult<CourseListItemDto> result;

        if (_existing is null)
        {
            result = await AppSession.Api.PostAsync<CourseListItemDto>("admin/courses", new
            {
                maCapDo = capDo.MaCapDo,
                tenKhoaHoc = ten,
                noiDung = string.IsNullOrWhiteSpace(_txtNoiDung.Text) ? null : _txtNoiDung.Text.Trim(),
                soBuoi = (int)_numSoBuoi.Value,
                tongSoGio = _numTongGio.Value <= 0 ? (decimal?)null : _numTongGio.Value,
                hocPhiCoBan = _numHocPhi.Value
            });
        }
        else
        {
            result = await AppSession.Api.PatchAsync<CourseListItemDto>($"admin/courses/{_existing.MaKhoaHoc}", new
            {
                tenKhoaHoc = ten,
                noiDung = string.IsNullOrWhiteSpace(_txtNoiDung.Text) ? null : _txtNoiDung.Text.Trim(),
                soBuoi = (int)_numSoBuoi.Value,
                tongSoGio = _numTongGio.Value <= 0 ? (decimal?)null : _numTongGio.Value,
                hocPhiCoBan = _numHocPhi.Value
            });
        }

        UseWaitCursor = false;
        _btnLuu.Enabled = true;

        if (!result.Success)
        {
            _lblThongBao.Text = string.IsNullOrWhiteSpace(result.ErrorMessage)
                ? "Không lưu được khóa học."
                : result.ErrorMessage;
            return;
        }

        DataChanged = true;
        DialogResult = DialogResult.OK;
        Close();
    }

    private sealed record CapDoItem(int MaCapDo, string MoTa)
    {
        public override string ToString() => MoTa;
    }
}
