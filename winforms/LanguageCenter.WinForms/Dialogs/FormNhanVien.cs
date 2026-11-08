using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Thêm / sửa hồ sơ NHÂN VIÊN (học vụ / tư vấn / kế toán).
/// <para>
/// Chức vụ do tab gọi quyết định và <b>không sửa được</b> ở chế độ cập nhật: đổi chức vụ
/// sẽ phải đổi vai trò + mã đăng nhập, làm hỏng dữ liệu lịch sử (lớp phụ trách,
/// giao dịch kế toán đã lập).
/// </para>
/// </summary>
public sealed class FormNhanVien : Form
{
    private static readonly string[] GioiTinhOptions = ["Nam", "Nữ", "Khác"];

    /// <summary>Nhân viên phải đủ 18 tuổi trở lên (khớp <c>EmployeeService.TuoiToiThieu</c>).</summary>
    private const int TuoiToiThieuNhanVien = 18;

    private readonly string _chucVu;
    private readonly string? _maNhanVien;

    private readonly TextBox _txtMaNhanVien = UiHelpers.CreateTextBox(620);
    private readonly TextBox _txtHoTen = UiHelpers.CreateTextBox(620);
    private readonly DateTimePicker _dtpNgaySinh = new();
    private readonly ComboBox _cboGioiTinh = UiHelpers.CreateComboBox(620);
    private readonly TextBox _txtEmail = UiHelpers.CreateTextBox(620);
    private readonly TextBox _txtSoDienThoai = UiHelpers.CreateTextBox(620);
    private readonly TextBox _txtDiaChi = UiHelpers.CreateTextBox(620);
    private readonly DateTimePicker _dtpNgayVaoLam = new();
    private readonly ComboBox _cboTrangThai = UiHelpers.CreateComboBox(620);

    private readonly Label _lblChucVu = new();
    private readonly Label _lblThongBao = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Lưu nhân viên", 150, 36);
    private readonly Button _btnHuy = UiHelpers.CreateSecondaryButton("Hủy bỏ", 110, 36);

    /// <summary>
    /// Vùng nội dung CUỘN ĐƯỢC. Nhờ vậy nhãn lỗi có thể cao bao nhiêu dòng tuỳ ý
    /// mà không bị cắt mất phần dưới.
    /// </summary>
    private readonly Panel _pnlBody = new();

    /// <summary>Dòng nút cố định ở đáy — không bao giờ bị nhãn lỗi che.</summary>
    private readonly Panel _pnlBottom = new();

    /// <summary>True khi đã lưu thành công — nơi gọi nên tải lại danh sách.</summary>
    public bool DataChanged { get; private set; }

    private bool DangSua => _maNhanVien is not null;

    /// <param name="chucVu">Chức vụ cố định của tab đang mở.</param>
    /// <param name="maNhanVien">Truyền vào để mở chế độ SỬA.</param>
    public FormNhanVien(string chucVu, string? maNhanVien = null)
    {
        _chucVu = chucVu;
        _maNhanVien = string.IsNullOrWhiteSpace(maNhanVien) ? null : maNhanVien.Trim();

        Text = DangSua ? $"Cập nhật nhân viên — {_maNhanVien}" : $"Thêm {chucVu.ToLowerInvariant()}";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(900, 800);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        UI.KiemTraNhap.BatTuNoiChieuCao(this);

        if (DangSua)
        {
            Load += async (_, _) => await NapChiTietAsync();
        }
    }

    private void DungGiaoDien()
    {
        // ---- Khung: nội dung cuộn ở trên, nút cố định ở dưới ----
        _pnlBottom.Dock = DockStyle.Bottom;
        _pnlBottom.Height = 64;
        _pnlBottom.BackColor = Color.White;

        _pnlBody.Dock = DockStyle.Fill;
        _pnlBody.AutoScroll = true;
        _pnlBody.BackColor = Color.White;

        Controls.Add(_pnlBody);
        Controls.Add(_pnlBottom);

        _pnlBody.Controls.Add(new Label
        {
            Text = DangSua ? "CẬP NHẬT HỒ SƠ NHÂN VIÊN" : "THÊM NHÂN VIÊN MỚI",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 18)
        });

        DatTruong("Mã nhân viên", _txtMaNhanVien, 56);
        _txtMaNhanVien.ReadOnly = true;
        _txtMaNhanVien.BackColor = Color.FromArgb(241, 245, 249);
        _txtMaNhanVien.Text = _maNhanVien ?? "(tự động sinh khi lưu)";

        DatTruong("Chức vụ", _lblChucVu, 116);

        _lblChucVu.Text = _chucVu;
        _lblChucVu.AutoSize = false;
        _lblChucVu.Size = new Size(620, 26);
        _lblChucVu.Font = new Font(Theme.FontRegular, FontStyle.Bold);
        _lblChucVu.ForeColor = Theme.TextDark;
        _lblChucVu.TextAlign = ContentAlignment.MiddleLeft;

        DatTruong("Họ tên *", _txtHoTen, 176);
        _txtHoTen.MaxLength = 150;
        _txtHoTen.PlaceholderText = "Ví dụ: Nguyễn Thị Lan";

        DatTruong("Ngày sinh", _dtpNgaySinh, 236);
        CauHinhNgay(_dtpNgaySinh);
        _dtpNgaySinh.Value = DateTime.Today.AddYears(-25);

        DatTruong("Giới tính", _cboGioiTinh, 296);
        _cboGioiTinh.Items.Add("— Chưa xác định —");
        _cboGioiTinh.Items.AddRange(GioiTinhOptions);
        _cboGioiTinh.SelectedIndex = 0;

        DatTruong("Email *", _txtEmail, 356);
        _txtEmail.MaxLength = 255;
        _txtEmail.PlaceholderText = "Ví dụ: lan.nguyen@trungtam.edu.vn";

        DatTruong("Số điện thoại", _txtSoDienThoai, 416);
        _txtSoDienThoai.MaxLength = 20;
        _txtSoDienThoai.PlaceholderText = "10 chữ số, bắt đầu bằng 0";
        KiemTraNhap.ChiNhapSoDienThoai(_txtSoDienThoai);

        DatTruong("Địa chỉ", _txtDiaChi, 476);
        _txtDiaChi.MaxLength = 500;

        DatTruong("Ngày vào làm", _dtpNgayVaoLam, 536);
        CauHinhNgay(_dtpNgayVaoLam);
        _dtpNgayVaoLam.Value = DateTime.Today;

        // Trạng thái chỉ có ý nghĩa khi sửa; khi tạo mới luôn là "Đang làm việc".
        DatTruong("Trạng thái làm việc", _cboTrangThai, 596);
        _cboTrangThai.Items.AddRange(WorkStatuses.All.ToArray());
        _cboTrangThai.SelectedIndex = 0;
        _cboTrangThai.Enabled = DangSua;

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(852, 24);
        _lblThongBao.Location = new Point(24, 648);
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Font = Theme.FontSub;
        _pnlBody.Controls.Add(_lblThongBao);

        _btnLuu.Text = DangSua ? "Lưu thay đổi" : "Tạo nhân viên";
        _btnLuu.Location = new Point(610, 14);
        _btnLuu.Click += async (_, _) => await LuuAsync();

        _btnHuy.Location = new Point(766, 14);
        _btnHuy.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };

        _pnlBottom.Controls.Add(_btnLuu);
        _pnlBottom.Controls.Add(_btnHuy);

        AcceptButton = _btnLuu;
        CancelButton = _btnHuy;
    }

    private static void CauHinhNgay(DateTimePicker picker)
    {
        picker.Font = Theme.FontRegular;
        picker.Width = 620;
        picker.Format = DateTimePickerFormat.Short;
    }

    private void DatTruong(string nhan, Control control, int y)
    {
        _pnlBody.Controls.Add(new Label
        {
            Text = nhan,
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, y)
        });

        control.Location = new Point(24, y + 20);
        _pnlBody.Controls.Add(control);
    }

    private async Task NapChiTietAsync()
    {
        SetBusy(true);

        var result = await AppSession.Api.GetAsync<EmployeeDetailDto>($"admin/employees/{_maNhanVien}");

        SetBusy(false);

        if (!result.Success || result.Data is null)
        {
            KiemTraNhap.HienThiLoiTuDong(_lblThongBao, [
                string.IsNullOrWhiteSpace(result.ErrorMessage)
                    ? "Không tải được hồ sơ nhân viên."
                    : result.ErrorMessage]);

            _btnLuu.Enabled = false;
            return;
        }

        var item = result.Data;

        _txtMaNhanVien.Text = item.MaNhanVien;
        _txtHoTen.Text = item.HoTen;

        if (item.NgaySinh is { } ngaySinh)
        {
            _dtpNgaySinh.Value = ngaySinh.ToDateTime(TimeOnly.MinValue);
        }

        var viTriGioiTinh = Array.IndexOf(GioiTinhOptions, item.GioiTinh ?? string.Empty);
        _cboGioiTinh.SelectedIndex = viTriGioiTinh >= 0 ? viTriGioiTinh + 1 : 0;

        _txtEmail.Text = item.Email ?? string.Empty;
        _txtSoDienThoai.Text = item.SoDienThoai ?? string.Empty;
        _txtDiaChi.Text = item.DiaChi ?? string.Empty;

        if (item.NgayVaoLam is { } ngayVaoLam)
        {
            _dtpNgayVaoLam.Value = ngayVaoLam.ToDateTime(TimeOnly.MinValue);
        }

        var viTriTrangThai = WorkStatuses.All.ToList().IndexOf(item.TrangThaiLamViec);
        _cboTrangThai.SelectedIndex = viTriTrangThai >= 0 ? viTriTrangThai : 0;
    }

    private void SetBusy(bool busy)
    {
        UseWaitCursor = busy;
        _btnLuu.Enabled = !busy;
        _btnHuy.Enabled = !busy;
    }

    private async Task LuuAsync()
    {
        // Xoá thông báo cũ VÀ thu nhỏ nhãn về 1 dòng trước khi kiểm tra lại.
        _lblThongBao.Text = string.Empty;
        _lblThongBao.Height = 24;

        KiemTraNhap.BoDanhDau(_txtHoTen, _txtEmail, _txtSoDienThoai);

        var loi = new List<string>();

        KiemTraNhap.BatBuoc(_txtHoTen, "Họ tên", loi, minLength: 2);
        KiemTraNhap.DoDai(_txtHoTen, "Họ tên", 150, loi);
        KiemTraNhap.Email(_txtEmail, "Email", loi);
        KiemTraNhap.SoDienThoai(_txtSoDienThoai, "Số điện thoại", loi);
        KiemTraNhap.DoDai(_txtDiaChi, "Địa chỉ", 500, loi);

        // Nhân viên phải đủ 18 tuổi (khớp ràng buộc phía server).
        KiemTraNhap.NgaySinh(_dtpNgaySinh, "Ngày sinh", loi);
        KiemTraNhap.TuoiToiThieu(_dtpNgaySinh, "Ngày sinh", TuoiToiThieuNhanVien, loi, "nhân viên");

        // Ngày vào làm không được ở tương lai (khớp validate phía server).
        if (_dtpNgayVaoLam.Value.Date > DateTime.Today)
        {
            KiemTraNhap.DanhDauLoi(_dtpNgayVaoLam);
            loi.Add("Ngày vào làm: không được ở tương lai.");
        }

        if (loi.Count > 0)
        {
            // Helper tự nới chiều cao nhãn → không bao giờ bị cắt mất dòng lỗi.
            KiemTraNhap.HienThiLoiTuDong(_lblThongBao, loi);
            KiemTraNhap.FocusLoi(_txtHoTen, _txtEmail, _txtSoDienThoai, _txtDiaChi, _dtpNgaySinh, _dtpNgayVaoLam);
            return;
        }

        SetBusy(true);

        if (DangSua)
        {
            await LuuCapNhatAsync();
        }
        else
        {
            await LuuTaoMoiAsync();
        }

        SetBusy(false);
    }

    private async Task LuuTaoMoiAsync()
    {
        var gioiTinh = _cboGioiTinh.SelectedIndex <= 0 ? null : _cboGioiTinh.SelectedItem as string;

        var request = new CreateEmployeeRequest
        {
            HoTen = _txtHoTen.Text.Trim(),
            NgaySinh = DateOnly.FromDateTime(_dtpNgaySinh.Value.Date),
            GioiTinh = gioiTinh,
            Email = _txtEmail.Text.Trim(),
            SoDienThoai = RongThanhNull(_txtSoDienThoai.Text),
            DiaChi = RongThanhNull(_txtDiaChi.Text),
            ChucVu = _chucVu,
            NgayVaoLam = DateOnly.FromDateTime(_dtpNgayVaoLam.Value.Date)
        };

        var result = await AppSession.Api.PostAsync<CreateEmployeeResponse>("admin/employees", request);

        if (!result.Success || result.Data is null)
        {
            KiemTraNhap.HienThiLoiTuDong(_lblThongBao, [
                string.IsNullOrWhiteSpace(result.ErrorMessage)
                    ? "Không tạo được nhân viên."
                    : result.ErrorMessage]);
            return;
        }

        DataChanged = true;

        using var dialog = new FormKetQuaMatKhau(
            "Đã tạo hồ sơ nhân viên",
            $"{result.Data.Message}\nMã: {result.Data.EmployeeId} ({result.Data.ChucVu})",
            result.Data.Username,
            result.Data.TemporaryPassword);

        dialog.ShowDialog(this);

        Close();
    }

    private async Task LuuCapNhatAsync()
    {
        var gioiTinh = _cboGioiTinh.SelectedIndex <= 0 ? null : _cboGioiTinh.SelectedItem as string;

        var request = new UpdateEmployeeRequest
        {
            HoTen = _txtHoTen.Text.Trim(),
            NgaySinh = DateOnly.FromDateTime(_dtpNgaySinh.Value.Date),
            GioiTinh = gioiTinh,
            Email = _txtEmail.Text.Trim(),
            // Gửi chuỗi rỗng để server hiểu là XOÁ giá trị (TextValue.Optional chuyển thành null).
            SoDienThoai = _txtSoDienThoai.Text.Trim(),
            DiaChi = _txtDiaChi.Text.Trim(),
            NgayVaoLam = DateOnly.FromDateTime(_dtpNgayVaoLam.Value.Date),
            TrangThaiLamViec = _cboTrangThai.SelectedItem as string
        };

        var result = await AppSession.Api.PatchAsync<EmployeeDetailDto>(
            $"admin/employees/{_maNhanVien}",
            request);

        if (!result.Success)
        {
            KiemTraNhap.HienThiLoiTuDong(_lblThongBao, [
                string.IsNullOrWhiteSpace(result.ErrorMessage)
                    ? "Không lưu được thay đổi."
                    : result.ErrorMessage]);
            return;
        }

        DataChanged = true;
        KiemTraNhap.HienThiThanhCong(_lblThongBao, "Đã lưu thay đổi.");
    }

    private static string? RongThanhNull(string? value)
        => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
