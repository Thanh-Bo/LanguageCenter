using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Tạo đăng ký học cho học viên — <c>POST /api/admin/enrollments</c>.
/// Học phí gốc và số tiền giảm do backend tự tính, client không gửi.
/// </summary>
public sealed class FormDangKyHoc : Form
{
    private readonly IReadOnlyList<StudentListItemDto> _hocViens;
    private readonly IReadOnlyList<CourseListItemDto> _khoaHocs;
    private readonly IReadOnlyList<ClassListItemDto> _lopHocs;

    private readonly ComboBox _cboHocVien = UiHelpers.CreateComboBox(560);
    private readonly ComboBox _cboKhoaHoc = UiHelpers.CreateComboBox(560);
    private readonly ComboBox _cboLop = UiHelpers.CreateComboBox(560);
    private readonly TextBox _txtLichMongMuon = UiHelpers.CreateTextBox(560);
    private readonly TextBox _txtGhiChu = UiHelpers.CreateTextBox(560);
    private readonly Label _lblThongBao = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Tạo đăng ký", 150, 36);
    private readonly Button _btnHuy = UiHelpers.CreateSecondaryButton("Hủy bỏ", 110, 36);

    public bool DataChanged { get; private set; }

    public FormDangKyHoc(
        IReadOnlyList<StudentListItemDto> hocViens,
        IReadOnlyList<CourseListItemDto> khoaHocs,
        IReadOnlyList<ClassListItemDto> lopHocs)
    {
        _hocViens = hocViens;
        _khoaHocs = khoaHocs;
        _lopHocs = lopHocs;

        Text = "Tạo đăng ký học";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(620, 520);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        NapDuLieu();
    }

    private void DungGiaoDien()
    {
        Controls.Add(new Label
        {
            Text = "TẠO ĐĂNG KÝ HỌC MỚI",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 18)
        });

        DatTruong("Học viên *", _cboHocVien, 56);
        DatTruong("Khóa học *", _cboKhoaHoc, 116);
        DatTruong("Lớp học (để trống nếu xếp lớp sau)", _cboLop, 176);
        DatTruong("Lịch học mong muốn", _txtLichMongMuon, 236);
        DatTruong("Ghi chú", _txtGhiChu, 296);

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(570, 46);
        _lblThongBao.Location = new Point(24, 356);
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Font = Theme.FontSub;
        Controls.Add(_lblThongBao);

        _btnLuu.Location = new Point(324, 462);
        _btnLuu.Click += async (_, _) => await LuuAsync();

        _btnHuy.Location = new Point(482, 462);
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
        _cboHocVien.Items.Clear();
        foreach (var hv in _hocViens.OrderBy(h => h.MaHocVien))
        {
            _cboHocVien.Items.Add(new HocVienItem(hv.MaHocVien,
                $"{hv.MaHocVien} — {hv.HoTen} ({hv.TrangThaiHocTap})"));
        }
        if (_cboHocVien.Items.Count > 0) _cboHocVien.SelectedIndex = 0;

        _cboKhoaHoc.Items.Clear();
        foreach (var kh in _khoaHocs.OrderBy(k => k.TenKhoaHoc))
        {
            _cboKhoaHoc.Items.Add(new KhoaHocItem(kh.MaKhoaHoc,
                $"{kh.TenKhoaHoc} · {kh.TenCapDo} · {kh.HocPhiCoBan:N0} đ"));
        }
        if (_cboKhoaHoc.Items.Count > 0) _cboKhoaHoc.SelectedIndex = 0;
        _cboKhoaHoc.SelectedIndexChanged += (_, _) => NapLopTheoKhoaHoc();

        NapLopTheoKhoaHoc();
    }

    private void NapLopTheoKhoaHoc()
    {
        _cboLop.Items.Clear();
        _cboLop.Items.Add("— Xếp lớp sau —");

        if (_cboKhoaHoc.SelectedItem is KhoaHocItem khoaHoc)
        {
            foreach (var lop in _lopHocs.Where(l => l.MaKhoaHoc == khoaHoc.MaKhoaHoc).OrderBy(l => l.MaLop))
            {
                _cboLop.Items.Add(new LopItem(lop.MaLopHoc,
                    $"{lop.MaLop} — {lop.TenLop} ({lop.SiSoHienTai}/{lop.SiSoToiDa})"));
            }
        }

        _cboLop.SelectedIndex = 0;
    }

    private async Task LuuAsync()
    {
        _lblThongBao.Text = string.Empty;

        if (_cboHocVien.SelectedItem is not HocVienItem hocVien)
        {
            _lblThongBao.Text = "Vui lòng chọn học viên.";
            return;
        }

        if (_cboKhoaHoc.SelectedItem is not KhoaHocItem khoaHoc)
        {
            _lblThongBao.Text = "Vui lòng chọn khóa học.";
            return;
        }

        var maLopHoc = (_cboLop.SelectedItem as LopItem)?.MaLopHoc;

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        var result = await AppSession.Api.PostAsync<EnrollmentListItemDto>("admin/enrollments", new
        {
            maHocVien = hocVien.MaHocVien,
            maKhoaHoc = khoaHoc.MaKhoaHoc,
            maLopHoc,
            lichHocMongMuon = string.IsNullOrWhiteSpace(_txtLichMongMuon.Text) ? null : _txtLichMongMuon.Text.Trim(),
            ghiChu = string.IsNullOrWhiteSpace(_txtGhiChu.Text) ? null : _txtGhiChu.Text.Trim()
        });

        UseWaitCursor = false;
        _btnLuu.Enabled = true;

        if (!result.Success)
        {
            _lblThongBao.Text = string.IsNullOrWhiteSpace(result.ErrorMessage)
                ? "Không tạo được đăng ký học."
                : result.ErrorMessage;
            return;
        }

        DataChanged = true;
        DialogResult = DialogResult.OK;
        Close();
    }

    private sealed record HocVienItem(string MaHocVien, string MoTa)
    {
        public override string ToString() => MoTa;
    }

    private sealed record KhoaHocItem(int MaKhoaHoc, string MoTa)
    {
        public override string ToString() => MoTa;
    }

    private sealed record LopItem(int MaLopHoc, string MoTa)
    {
        public override string ToString() => MoTa;
    }
}
