using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Tạo mới / chỉnh sửa tài khoản đăng nhập.
/// Gọi <c>POST /api/admin/accounts</c> hoặc <c>PATCH /api/admin/accounts/{id}</c>.
/// </summary>
public sealed class FormTaiKhoan : Form
{
    private readonly AccountListItemDto? _existing;
    private readonly IReadOnlyList<string> _tenDangNhapDaCo;

    private readonly TextBox _txtTenDangNhap = UiHelpers.CreateTextBox(320);
    private readonly TextBox _txtEmail = UiHelpers.CreateTextBox(320);
    private readonly TextBox _txtMatKhau = UiHelpers.CreateTextBox(320);
    private readonly ComboBox _cboVaiTro = UiHelpers.CreateComboBox(320);
    private readonly ComboBox _cboTrangThai = UiHelpers.CreateComboBox(320);
    private readonly Label _lblThongBao = new();
    private readonly Label _lblGoiY = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Lưu tài khoản", 150, 36);
    private readonly Button _btnHuy = UiHelpers.CreateSecondaryButton("Hủy bỏ", 110, 36);

    public bool DataChanged { get; private set; }

    public FormTaiKhoan(AccountListItemDto? existing = null, IReadOnlyList<string>? tenDangNhapDaCo = null)
    {
        _existing = existing;
        _tenDangNhapDaCo = tenDangNhapDaCo ?? Array.Empty<string>();

        Text = existing is null ? "Thêm tài khoản" : $"Cập nhật tài khoản {existing.Username}";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(600, 500);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        NapDuLieu();
    }

    private void DungGiaoDien()
    {
        Controls.Add(new Label
        {
            Text = _existing is null ? "THÊM TÀI KHOẢN MỚI" : "CẬP NHẬT TÀI KHOẢN",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 18)
        });

        DatTruong("Tên đăng nhập * (chính là mã nghiệp vụ)", _txtTenDangNhap, 56);
        DatTruong("Email *", _txtEmail, 116);
        DatTruong(_existing is null ? "Mật khẩu *" : "Mật khẩu mới (để trống nếu không đổi)", _txtMatKhau, 176);

        _txtMatKhau.UseSystemPasswordChar = true;

        DatTruong("Vai trò *", _cboVaiTro, 236);
        DatTruong("Trạng thái *", _cboTrangThai, 296);

        _lblGoiY.AutoSize = false;
        _lblGoiY.Size = new Size(552, 34);
        _lblGoiY.Location = new Point(24, 356);
        _lblGoiY.Font = Theme.FontSub;
        _lblGoiY.ForeColor = Theme.Muted;
        Controls.Add(_lblGoiY);

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(552, 34);
        _lblThongBao.Location = new Point(24, 392);
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Font = Theme.FontSub;
        Controls.Add(_lblThongBao);

        _btnLuu.Location = new Point(304, 440);
        _btnLuu.Click += async (_, _) => await LuuAsync();

        _btnHuy.Location = new Point(462, 440);
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

    private void NapDuLieu()
    {
        _cboVaiTro.Items.Clear();
        _cboVaiTro.Items.AddRange(RoleNames.All.ToArray());

        _cboTrangThai.Items.Clear();
        _cboTrangThai.Items.AddRange(AccountStatuses.All.ToArray());

        if (_existing is null)
        {
            _cboVaiTro.SelectedItem = RoleNames.NhanVienHocVu;
            if (_cboVaiTro.SelectedIndex < 0) _cboVaiTro.SelectedIndex = 0;
            _cboTrangThai.SelectedItem = AccountStatuses.HoatDong;
            if (_cboTrangThai.SelectedIndex < 0) _cboTrangThai.SelectedIndex = 0;

            _lblGoiY.Text = "Lưu ý: Học viên nên tạo ở mục Quản lý học viên, Giáo viên ở mục Quản lý giáo viên "
                            + "để hệ thống sinh kèm hồ sơ.";
            return;
        }

        _txtTenDangNhap.Text = _existing.Username;
        _txtTenDangNhap.ReadOnly = true;
        _txtTenDangNhap.BackColor = Color.FromArgb(241, 245, 249);

        _txtEmail.Text = _existing.Email;

        _cboVaiTro.SelectedItem = _existing.Role;
        if (_cboVaiTro.SelectedIndex < 0) _cboVaiTro.SelectedIndex = 0;

        _cboTrangThai.SelectedItem = _existing.Status;
        if (_cboTrangThai.SelectedIndex < 0) _cboTrangThai.SelectedIndex = 0;

        _lblGoiY.Text = "Không thể đổi tên đăng nhập vì đây là mã nghiệp vụ đang được các bảng khác tham chiếu.";
    }

    private async Task LuuAsync()
    {
        _lblThongBao.Text = string.Empty;

        var email = _txtEmail.Text.Trim();
        if (email.Length == 0)
        {
            _lblThongBao.Text = "Vui lòng nhập email.";
            return;
        }

        var vaiTro = _cboVaiTro.SelectedItem?.ToString();
        if (string.IsNullOrWhiteSpace(vaiTro))
        {
            _lblThongBao.Text = "Vui lòng chọn vai trò.";
            return;
        }

        var trangThai = _cboTrangThai.SelectedItem?.ToString() ?? AccountStatuses.HoatDong;
        var matKhau = _txtMatKhau.Text;

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        ApiResult<AccountDetailDto> result;

        if (_existing is null)
        {
            var tenDangNhap = _txtTenDangNhap.Text.Trim();

            if (tenDangNhap.Length == 0)
            {
                UseWaitCursor = false;
                _btnLuu.Enabled = true;
                _lblThongBao.Text = "Vui lòng nhập tên đăng nhập.";
                return;
            }

            // Kiểm tra trùng ngay trên client để có phản hồi tức thì.
            if (_tenDangNhapDaCo.Any(x => string.Equals(x, tenDangNhap, StringComparison.OrdinalIgnoreCase)))
            {
                UseWaitCursor = false;
                _btnLuu.Enabled = true;
                _lblThongBao.Text = $"Tên đăng nhập '{tenDangNhap}' đã tồn tại. Hãy dùng 'Sửa tài khoản' nếu muốn cập nhật tài khoản đó.";
                _txtTenDangNhap.Focus();
                _txtTenDangNhap.SelectAll();
                return;
            }

            if (matKhau.Length < 6)
            {
                UseWaitCursor = false;
                _btnLuu.Enabled = true;
                _lblThongBao.Text = "Mật khẩu phải có ít nhất 6 ký tự.";
                return;
            }

            // Vai trò Học viên / Giáo viên / Trợ giảng BẮT BUỘC có hồ sơ đi kèm.
            // Nếu tạo ở đây, hệ thống chỉ sinh dòng trong bảng TaiKhoan → tài khoản
            // sẽ không xuất hiện ở màn Quản lý học viên / Quản lý giáo viên.
            if (vaiTro is RoleNames.HocVien or RoleNames.GiaoVien or RoleNames.TroGiang)
            {
                var xacNhan = MessageBox.Show(this,
                    $"Vai trò '{vaiTro}' cần có hồ sơ đi kèm.\n\n"
                    + $"Nếu tạo ở đây, tài khoản '{tenDangNhap}' chỉ có dòng trong bảng TaiKhoan "
                    + "và KHÔNG xuất hiện trong danh sách học viên/giáo viên.\n\n"
                    + "Cách đúng: tạo ở màn 'Quản lý học viên' hoặc 'Quản lý giáo viên' "
                    + "(các màn đó tự sinh tài khoản + hồ sơ).\n\n"
                    + "Vẫn tiếp tục tạo tài khoản không kèm hồ sơ?",
                    "Vai trò cần hồ sơ", MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2);

                if (xacNhan != DialogResult.Yes)
                {
                    UseWaitCursor = false;
                    _btnLuu.Enabled = true;
                    return;
                }
            }

            result = await AppSession.Api.PostAsync<AccountDetailDto>("admin/accounts", new
            {
                tenDangNhap,
                email,
                matKhau,
                vaiTro,
                trangThai
            });
        }
        else
        {
            if (matKhau.Length > 0 && matKhau.Length < 6)
            {
                UseWaitCursor = false;
                _btnLuu.Enabled = true;
                _lblThongBao.Text = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                return;
            }

            result = await AppSession.Api.PatchAsync<AccountDetailDto>($"admin/accounts/{_existing.Id}", new
            {
                email,
                matKhau = string.IsNullOrWhiteSpace(matKhau) ? null : matKhau,
                vaiTro,
                trangThai
            });
        }

        UseWaitCursor = false;
        _btnLuu.Enabled = true;

        if (!result.Success)
        {
            // 405 = API đang chạy bản cũ chưa có endpoint POST/PATCH tài khoản.
            if (result.StatusCode == 405)
            {
                _lblThongBao.Text = "API đang chạy bản cũ chưa có chức năng này. Hãy khởi động lại backend (dotnet run) rồi thử lại.";
                return;
            }

            var chiTiet = result.Errors.Count > 0 ? $" ({string.Join("; ", result.Errors)})" : string.Empty;
            _lblThongBao.Text = string.IsNullOrWhiteSpace(result.ErrorMessage)
                ? "Không lưu được tài khoản." + chiTiet
                : result.ErrorMessage + chiTiet;
            return;
        }

        DataChanged = true;
        DialogResult = DialogResult.OK;
        Close();
    }
}
