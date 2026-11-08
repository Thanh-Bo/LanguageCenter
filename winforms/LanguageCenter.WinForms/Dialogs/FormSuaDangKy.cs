using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Sửa ĐĂNG KÝ HỌC — đổi LỚP DỰ KIẾN và ghi chú; lịch học là dữ liệu CHỈ ĐỌC suy ra từ lớp dự kiến.
/// <para>
/// Gọi <c>PATCH /api/admin/enrollments/{id}</c>. Form không gửi <c>maUuDai</c> nên
/// server giữ nguyên ưu đãi đã áp — không có chuyện vô tình xoá giảm giá.
/// </para>
/// <para>
/// Đây là ĐƯỜNG SỬA khi đã thu đủ học phí nhưng lớp dự kiến cũ không xếp được
/// (đầy sĩ số / quá 4 buổi / trùng giờ / sai tuổi): đổi lớp dự kiến → backend tự xếp ngay.
/// </para>
/// </summary>
public sealed class FormSuaDangKy : Form
{
    private readonly EnrollmentListItemDto _item;
    private readonly IReadOnlyList<ClassListItemDto> _lopHocs;

    private readonly ComboBox _cboLopDuKien = UiHelpers.CreateComboBox(460);
    private readonly TextBox _txtLichHoc = UiHelpers.CreateTextBox(460);
    private readonly TextBox _txtGhiChu = UiHelpers.CreateTextBox(460);
    private readonly Label _lblThongBao = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Lưu thay đổi", 150, 36);
    private readonly Button _btnHuy = UiHelpers.CreateSecondaryButton("Hủy", 110, 36);

    public bool DataChanged { get; private set; }

    /// <summary>Đăng ký đã vào lớp thật rồi → không cho đổi lớp dự kiến nữa.</summary>
    private bool DaVaoLop => _item.MaLopHoc is not null;

    public FormSuaDangKy(EnrollmentListItemDto item)
        : this(item, Array.Empty<ClassListItemDto>())
    {
    }

    public FormSuaDangKy(EnrollmentListItemDto item, IReadOnlyList<ClassListItemDto> lopHocs)
    {
        _item = item;
        _lopHocs = lopHocs;

        Text = $"Sửa đăng ký học — {item.TenHocVien}";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(540, 400);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        UI.KiemTraNhap.BatTuNoiChieuCao(this);
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
            Text = DaVaoLop ? "Lớp đã vào" : "Lớp dự kiến * (đổi lớp → tự xếp nếu đã thu đủ)",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, 100)
        });

        _cboLopDuKien.Location = new Point(24, 120);
        _cboLopDuKien.DropDownHeight = 260;
        _cboLopDuKien.IntegralHeight = false;
        _cboLopDuKien.SelectedIndexChanged += (_, _) => CapNhatLichHoc();
        Controls.Add(_cboLopDuKien);
        NapLopDuKien();

        Controls.Add(new Label
        {
            Text = "Lịch học (tự động theo lớp dự kiến)",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, 158)
        });

        // Lịch học suy ra từ lớp → khoá nhập, chỉ để xem.
        _txtLichHoc.Location = new Point(24, 178);
        _txtLichHoc.ReadOnly = true;
        _txtLichHoc.TabStop = false;
        _txtLichHoc.BackColor = Theme.ContentBackground;
        _txtLichHoc.ForeColor = Theme.TextDark;
        Controls.Add(_txtLichHoc);

        Controls.Add(new Label
        {
            Text = "Ghi chú",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, 216)
        });

        _txtGhiChu.Location = new Point(24, 236);
        Controls.Add(_txtGhiChu);

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(492, 60);
        _lblThongBao.Location = new Point(24, 272);
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Font = Theme.FontSub;
        Controls.Add(_lblThongBao);

        _btnLuu.Location = new Point(244, 340);
        _btnLuu.Click += async (_, _) => await LuuAsync();
        Controls.Add(_btnLuu);

        _btnHuy.Location = new Point(402, 340);
        _btnHuy.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        Controls.Add(_btnHuy);

        AcceptButton = _btnLuu;
        CancelButton = _btnHuy;
    }

    /// <summary>
    /// Nạp danh sách lớp dự kiến: cùng khóa học, còn chỗ — LUÔN giữ lớp hiện tại
    /// để không mất giá trị đang lưu dù lớp đã đầy.
    /// </summary>
    private void NapLopDuKien()
    {
        _cboLopDuKien.Items.Clear();

        if (DaVaoLop)
        {
            _cboLopDuKien.Items.Add($"{_item.MaLop} — đã vào lớp");
            _cboLopDuKien.SelectedIndex = 0;
            _cboLopDuKien.Enabled = false;
            return;
        }

        foreach (var lop in _lopHocs
                     .Where(l => l.MaKhoaHoc == _item.MaKhoaHoc)
                     .Where(l => l.MaLopHoc == _item.MaLopDuKien
                                 || l.SiSoHienTai + l.SiSoDuKien < l.SiSoToiDa)
                     .OrderBy(l => l.MaLop))
        {
            _cboLopDuKien.Items.Add(new LopItem(lop.MaLopHoc,
                $"{lop.MaLop} — {lop.TenLop} ({lop.SiSoHienTai + lop.SiSoDuKien}/{lop.SiSoToiDa} chỗ)"));
        }

        if (_cboLopDuKien.Items.Count == 0)
        {
            _cboLopDuKien.Items.Add("— Không còn lớp nào còn chỗ cho khóa này —");
            _cboLopDuKien.SelectedIndex = 0;
            _btnLuu.Enabled = false;
            return;
        }

        var viTri = 0;

        if (_item.MaLopDuKien is { } maLopHienTai)
        {
            for (var i = 0; i < _cboLopDuKien.Items.Count; i++)
            {
                if (_cboLopDuKien.Items[i] is LopItem it && it.MaLopHoc == maLopHienTai)
                {
                    viTri = i;
                    break;
                }
            }
        }

        _cboLopDuKien.SelectedIndex = viTri;
        CapNhatLichHoc();
    }

    /// <summary>Lịch học của lớp dự kiến đang chọn (chỉ đọc).</summary>
    private void CapNhatLichHoc()
    {
        var maLopHoc = (_cboLopDuKien.SelectedItem as LopItem)?.MaLopHoc
                       ?? _item.MaLopHoc;

        var lop = maLopHoc is null ? null : _lopHocs.FirstOrDefault(l => l.MaLopHoc == maLopHoc);

        _txtLichHoc.Text = lop is null ? LichHocText.ChuaXepLop : LichHocText.TuLop(lop);
    }

    private sealed record LopItem(int MaLopHoc, string MoTa)
    {
        public override string ToString() => MoTa;
    }

    private async Task LuuAsync()
    {
        _lblThongBao.Text = string.Empty;

        var maLopDuKien = (_cboLopDuKien.SelectedItem as LopItem)?.MaLopHoc;

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        // Không gửi maUuDai / boUuDai: server giữ nguyên ưu đãi hiện có của đăng ký.
        // Gửi maLopDuKien: nếu đã thu đủ tiền, backend tự xếp lớp ngay vào lớp mới.
        var result = await AppSession.Api.PatchAsync<EnrollmentListItemDto>(
            $"admin/enrollments/{_item.MaDangKy}",
            new
            {
                maLopDuKien,
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
