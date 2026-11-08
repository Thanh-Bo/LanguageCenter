using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Thêm mới / cập nhật CẤP ĐỘ đào tạo.
/// <para>
/// Gọi <c>POST /api/admin/levels</c> (tạo) hoặc <c>PATCH /api/admin/levels/{id}</c> (sửa).
/// Không cho chuyển cấp độ sang chương trình khác sau khi tạo (backend cũng chặn).
/// </para>
/// </summary>
public sealed class FormCapDo : Form
{
    private readonly IReadOnlyList<ProgramListItemDto> _programs;
    private readonly LevelListItemDto? _existing;

    private readonly ComboBox _cboChuongTrinh = UiHelpers.CreateComboBox(460);
    private readonly TextBox _txtTen = UiHelpers.CreateTextBox(460);
    private readonly TextBox _txtKyHieu = UiHelpers.CreateTextBox(210);
    private readonly NumericUpDown _numThuTu = new() { Width = 210, Minimum = 1, Maximum = 999, Value = 1 };
    private readonly CheckBox _chkCoThoiLuong = new() { Text = "Có thời lượng dự kiến", AutoSize = true };
    private readonly NumericUpDown _numThoiLuong = new() { Width = 140, Minimum = 1, Maximum = 520, Value = 10 };
    private readonly TextBox _txtDieuKien = UiHelpers.CreateTextBox(460);
    private readonly TextBox _txtKetQua = UiHelpers.CreateTextBox(460);

    private readonly Label _lblThongBao = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Lưu", 140, 36);
    private readonly Button _btnHuy = UiHelpers.CreateSecondaryButton("Hủy", 110, 36);

    public bool DataChanged { get; private set; }

    public FormCapDo(IReadOnlyList<ProgramListItemDto> programs, LevelListItemDto? existing = null, int? maChuongTrinhMacDinh = null)
    {
        _programs = programs;
        _existing = existing;

        Text = existing is null ? "Thêm cấp độ đào tạo" : $"Cập nhật cấp độ — {existing.TenCapDo}";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(560, 480);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        UI.KiemTraNhap.BatTuNoiChieuCao(this);
        NapDuLieu(maChuongTrinhMacDinh);
    }

    private void DungGiaoDien()
    {
        Controls.Add(new Label
        {
            Text = _existing is null ? "THÊM CẤP ĐỘ ĐÀO TẠO" : "CẬP NHẬT CẤP ĐỘ ĐÀO TẠO",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 16)
        });

        DatTruong("Chương trình *", _cboChuongTrinh, 56);
        DatTruong("Tên cấp độ *", _txtTen, 116);

        Controls.Add(new Label
        {
            Text = "Ký hiệu",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, 176)
        });
        _txtKyHieu.Location = new Point(24, 196);
        Controls.Add(_txtKyHieu);

        Controls.Add(new Label
        {
            Text = "Thứ tự *",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(250, 176)
        });
        _numThuTu.Location = new Point(250, 196);
        Controls.Add(_numThuTu);

        _chkCoThoiLuong.Location = new Point(24, 232);
        Controls.Add(_chkCoThoiLuong);

        _numThoiLuong.Location = new Point(200, 230);
        _numThoiLuong.Enabled = false;
        Controls.Add(_numThoiLuong);

        _chkCoThoiLuong.CheckedChanged += (_, _) => _numThoiLuong.Enabled = _chkCoThoiLuong.Checked;

        DatTruong("Điều kiện đầu vào", _txtDieuKien, 262);
        DatTruong("Chuẩn đầu ra cam kết", _txtKetQua, 322);

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(512, 40);
        _lblThongBao.Location = new Point(24, 384);
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

    private void NapDuLieu(int? maChuongTrinhMacDinh)
    {
        _cboChuongTrinh.Items.Clear();

        foreach (var p in _programs.OrderBy(p => p.TenChuongTrinh))
        {
            _cboChuongTrinh.Items.Add(new ChuongTrinhItem(p.MaChuongTrinh, p.TenChuongTrinh));
        }

        if (_existing is not null)
        {
            _txtTen.Text = _existing.TenCapDo;
            _txtKyHieu.Text = _existing.KyHieuCapDo;
            _numThuTu.Value = Math.Clamp(_existing.ThuTu, 1, 999);
            _txtDieuKien.Text = _existing.DieuKienDauVao ?? string.Empty;
            _txtKetQua.Text = _existing.KetQuaDauRa ?? string.Empty;

            if (_existing.ThoiLuongDuKienTuan is { } tuan)
            {
                _chkCoThoiLuong.Checked = true;
                _numThoiLuong.Value = Math.Clamp(tuan, 1, 520);
            }

            // Không cho đổi chương trình sau khi tạo.
            for (var i = 0; i < _cboChuongTrinh.Items.Count; i++)
            {
                if (_cboChuongTrinh.Items[i] is ChuongTrinhItem it && it.MaChuongTrinh == _existing.MaChuongTrinh)
                {
                    _cboChuongTrinh.SelectedIndex = i;
                    break;
                }
            }

            _cboChuongTrinh.Enabled = false;
            return;
        }

        if (_cboChuongTrinh.Items.Count > 0)
        {
            var viTri = 0;

            if (maChuongTrinhMacDinh is { } ma)
            {
                for (var i = 0; i < _cboChuongTrinh.Items.Count; i++)
                {
                    if (_cboChuongTrinh.Items[i] is ChuongTrinhItem it && it.MaChuongTrinh == ma)
                    {
                        viTri = i;
                        break;
                    }
                }
            }

            _cboChuongTrinh.SelectedIndex = viTri;
        }
        else
        {
            _lblThongBao.Text = "Chưa có chương trình nào. Hãy tạo Chương trình trước.";
            _btnLuu.Enabled = false;
        }
    }

    private async Task LuuAsync()
    {
        _lblThongBao.Text = string.Empty;

        var ten = _txtTen.Text.Trim();

        if (ten.Length < 2)
        {
            _lblThongBao.Text = "Tên cấp độ phải có ít nhất 2 ký tự.";
            return;
        }

        if (_cboChuongTrinh.SelectedItem is not ChuongTrinhItem chuongTrinh)
        {
            _lblThongBao.Text = "Vui lòng chọn chương trình.";
            return;
        }

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        var thoiLuong = _chkCoThoiLuong.Checked ? (int?)_numThoiLuong.Value : null;

        var result = _existing is null
            ? await AppSession.Api.PostAsync<LevelListItemDto>("admin/levels", new
            {
                maChuongTrinh = chuongTrinh.MaChuongTrinh,
                tenCapDo = ten,
                kyHieuCapDo = string.IsNullOrWhiteSpace(_txtKyHieu.Text) ? null : _txtKyHieu.Text.Trim(),
                thuTu = (int)_numThuTu.Value,
                thoiLuongDuKienTuan = thoiLuong,
                dieuKienDauVao = string.IsNullOrWhiteSpace(_txtDieuKien.Text) ? null : _txtDieuKien.Text.Trim(),
                ketQuaDauRa = string.IsNullOrWhiteSpace(_txtKetQua.Text) ? null : _txtKetQua.Text.Trim()
            })
            : await AppSession.Api.PatchAsync<LevelListItemDto>($"admin/levels/{_existing.MaCapDo}", new
            {
                tenCapDo = ten,
                kyHieuCapDo = _txtKyHieu.Text.Trim(),
                thuTu = (int)_numThuTu.Value,
                thoiLuongDuKienTuan = thoiLuong,
                dieuKienDauVao = _txtDieuKien.Text.Trim(),
                ketQuaDauRa = _txtKetQua.Text.Trim()
            });

        UseWaitCursor = false;
        _btnLuu.Enabled = true;

        if (!result.Success)
        {
            _lblThongBao.Text = HienThiLoi(result);
            return;
        }

        DataChanged = true;
        DialogResult = DialogResult.OK;
        Close();
    }

    private static string HienThiLoi<T>(ApiResult<T> result)
        => result.Errors.Count > 0
            ? $"{result.ErrorMessage}\n- {string.Join("\n- ", result.Errors)}"
            : (string.IsNullOrWhiteSpace(result.ErrorMessage) ? "Không lưu được cấp độ." : result.ErrorMessage);

    private sealed record ChuongTrinhItem(int MaChuongTrinh, string TenChuongTrinh)
    {
        public override string ToString() => TenChuongTrinh;
    }
}
