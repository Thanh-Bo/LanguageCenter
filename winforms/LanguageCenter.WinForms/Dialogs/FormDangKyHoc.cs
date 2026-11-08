using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Tạo đăng ký học cho học viên — <c>POST /api/admin/enrollments</c>.
/// Học phí gốc và số tiền giảm do backend tự tính, client không gửi.
/// <para>
/// Form KHÔNG chọn ưu đãi (ưu đãi được xử lý ở màn hình khuyến mãi) và
/// KHÔNG cho gõ "lịch học mong muốn": lịch học là dữ liệu SUY RA từ lớp đã chọn
/// nên chỉ hiển thị, không gửi lên server.
/// </para>
/// </summary>
public sealed class FormDangKyHoc : Form
{
    private readonly IReadOnlyList<StudentListItemDto> _hocViens;
    private readonly IReadOnlyList<CourseListItemDto> _khoaHocs;
    private readonly IReadOnlyList<ClassListItemDto> _lopHocs;
    private readonly IReadOnlyList<EnrollmentListItemDto> _dangKys;

    private readonly ComboBox _cboHocVien = UiHelpers.CreateComboBox(560);
    private readonly ComboBox _cboChuongTrinh = UiHelpers.CreateComboBox(560);
    private readonly ComboBox _cboKhoaHoc = UiHelpers.CreateComboBox(560);
    private readonly ComboBox _cboLop = UiHelpers.CreateComboBox(560);
    private readonly TextBox _txtTimHocVien = UiHelpers.CreateTextBox(180);
    private readonly TextBox _txtLichHoc = UiHelpers.CreateTextBox(560);
    private readonly TextBox _txtGhiChu = UiHelpers.CreateTextBox(560);
    private readonly Label _lblHocPhi = new();
    private readonly Label _lblThongBao = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Tạo đăng ký", 150, 36);
    private readonly Button _btnHuy = UiHelpers.CreateSecondaryButton("Hủy bỏ", 110, 36);

    /// <summary>Không còn ai để đăng ký khóa đang chọn — hiện thay cho danh sách học viên.</summary>
    private const string KhongConHocVien = "— Không còn học viên nào phù hợp để đăng ký khóa này —";

    /// <summary>Khóa học không còn lớp nào đủ chỗ — hiện thay cho danh sách lớp.</summary>
    private const string KhongConLop = "— Không còn lớp nào còn chỗ cho khóa này —";

    private bool _dangNap;

    public bool DataChanged { get; private set; }

    public FormDangKyHoc(
        IReadOnlyList<StudentListItemDto> hocViens,
        IReadOnlyList<CourseListItemDto> khoaHocs,
        IReadOnlyList<ClassListItemDto> lopHocs)
        : this(hocViens, khoaHocs, lopHocs, Array.Empty<EnrollmentListItemDto>())
    {
    }

    public FormDangKyHoc(
        IReadOnlyList<StudentListItemDto> hocViens,
        IReadOnlyList<CourseListItemDto> khoaHocs,
        IReadOnlyList<ClassListItemDto> lopHocs,
        IReadOnlyList<EnrollmentListItemDto> dangKys)
    {
        _hocViens = hocViens;
        _khoaHocs = khoaHocs;
        _lopHocs = lopHocs;
        _dangKys = dangKys;

        Text = "Tạo đăng ký học";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(620, 580);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        UI.KiemTraNhap.BatTuNoiChieuCao(this);

        // ComboBox mặc định chỉ cao ~106px (≈5 dòng) nên danh sách dài trông như
        // "không đổ hết". Mở rộng vùng xổ xuống cho học viên / khóa học / lớp.
        foreach (var cbo in new[] { _cboHocVien, _cboChuongTrinh, _cboKhoaHoc, _cboLop })
        {
            cbo.DropDownHeight = 320;
            cbo.IntegralHeight = false;
        }

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

        // ---- Học viên + ô tìm nhanh ----
        Controls.Add(new Label
        {
            Text = "Học viên * (gõ để tìm nhanh theo mã / tên)",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, 56)
        });

        _txtTimHocVien.Location = new Point(24, 76);
        _txtTimHocVien.PlaceholderText = "Tìm mã / tên…";
        _txtTimHocVien.TextChanged += (_, _) => LocHocVien();
        Controls.Add(_txtTimHocVien);

        _cboHocVien.Location = new Point(214, 76);
        _cboHocVien.Width = 370;
        Controls.Add(_cboHocVien);

        DatTruong("Chương trình *", _cboChuongTrinh, 136);
        DatTruong("Khóa học *", _cboKhoaHoc, 196);
        DatTruong("Lớp học dự kiến * (tự xếp khi thu đủ học phí)", _cboLop, 256);
        DatTruong("Lịch học (tự động theo lớp đã chọn)", _txtLichHoc, 316);
        DatTruong("Ghi chú", _txtGhiChu, 376);

        // Lịch học là dữ liệu SUY RA từ lớp → khoá nhập, chỉ để xem.
        _txtLichHoc.ReadOnly = true;
        _txtLichHoc.TabStop = false;
        _txtLichHoc.BackColor = Theme.ContentBackground;
        _txtLichHoc.ForeColor = Theme.TextDark;

        // Học phí hiện ngay cạnh ô khóa học (không nhồi vào tên khóa học cho dài dòng).
        _cboKhoaHoc.Width = 430;
        _lblHocPhi.AutoSize = false;
        _lblHocPhi.Size = new Size(126, 24);
        _lblHocPhi.Location = new Point(462, 218);
        _lblHocPhi.Font = Theme.FontBold;
        _lblHocPhi.ForeColor = Theme.Primary;
        _lblHocPhi.TextAlign = ContentAlignment.MiddleRight;
        Controls.Add(_lblHocPhi);

        // Chọn chương trình → chỉ hiện các khóa học thuộc chương trình đó.
        _cboChuongTrinh.SelectedIndexChanged += (_, _) =>
        {
            if (!_dangNap) DoVaoComboKhoaHoc();
        };

        _cboKhoaHoc.SelectedIndexChanged += (_, _) => NapLopTheoKhoaHoc();

        // Đổi lớp → lịch học cập nhật ngay.
        _cboLop.SelectedIndexChanged += (_, _) => CapNhatLichHoc();

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(570, 46);
        _lblThongBao.Location = new Point(24, 438);
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Font = Theme.FontSub;
        Controls.Add(_lblThongBao);

        _btnLuu.Location = new Point(324, 504);
        _btnLuu.Click += async (_, _) => await LuuAsync();

        _btnHuy.Location = new Point(482, 504);
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
        LocHocVien();
        DoVaoComboChuongTrinh();
    }

    /// <summary>
    /// Sinh nội dung ô "Lịch học" từ lớp đang chọn.
    /// <para>
    /// Chỉ đọc: lấy thẳng <c>ThuBuoi1 / ThuBuoi2 / GioBatDau / GioKetThuc</c> đã có
    /// trong <see cref="ClassListItemDto"/> — không gọi thêm API.
    /// Ví dụ: <c>Thứ 2 &amp; Thứ 4 · 18:00 – 19:30</c>.
    /// </para>
    /// </summary>
    private void CapNhatLichHoc()
    {
        var maLopHoc = (_cboLop.SelectedItem as LopItem)?.MaLopHoc;

        _txtLichHoc.Text = LichHocText.TuLop(
            maLopHoc is null ? null : _lopHocs.FirstOrDefault(l => l.MaLopHoc == maLopHoc));
    }

    /// <summary>Lọc nhanh combo học viên theo mã hoặc tên (gõ vào ô bên trái).</summary>
    private void LocHocVien()
    {
        var kw = _txtTimHocVien.Text.Trim();
        var dangChon = (_cboHocVien.SelectedItem as HocVienItem)?.MaHocVien;

        _dangNap = true;
        _cboHocVien.Items.Clear();

        IEnumerable<StudentListItemDto> nguon = _hocViens;

        // Ẩn học viên ĐÃ có đăng ký còn hiệu lực cho khóa đang chọn
        // (đã ở trong lớp → "Đã xếp lớp", hoặc đang chờ xếp lớp → "Chờ xử lý"):
        // backend cũng chặn trùng, nên không đưa ra cho người dùng chọn nữa.
        if (MaKhoaHocDangChon is { } maKhoaHoc)
        {
            var daDangKy = _dangKys
                .Where(dk => dk.MaKhoaHoc == maKhoaHoc && LaDangKyConHieuLuc(dk.TrangThaiDangKy))
                .Select(dk => dk.MaHocVien)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            nguon = nguon.Where(h => !daDangKy.Contains(h.MaHocVien));
        }

        if (!string.IsNullOrWhiteSpace(kw))
        {
            nguon = nguon.Where(h =>
                h.MaHocVien.Contains(kw, StringComparison.OrdinalIgnoreCase)
                || h.HoTen.Contains(kw, StringComparison.OrdinalIgnoreCase)
                || (h.SoDienThoai ?? string.Empty).Contains(kw, StringComparison.OrdinalIgnoreCase));
        }

        foreach (var hv in nguon.OrderBy(h => h.MaHocVien))
        {
            _cboHocVien.Items.Add(new HocVienItem(hv.MaHocVien,
                $"{hv.MaHocVien} — {hv.HoTen} ({hv.TrangThaiHocTap})"));
        }

        if (_cboHocVien.Items.Count == 0)
        {
            _cboHocVien.Items.Add(KhongConHocVien);
            _cboHocVien.SelectedIndex = 0;
            _dangNap = false;
            return;
        }

        var viTri = 0;

        if (dangChon is not null)
        {
            for (var i = 0; i < _cboHocVien.Items.Count; i++)
            {
                if (_cboHocVien.Items[i] is HocVienItem it && it.MaHocVien == dangChon)
                {
                    viTri = i;
                    break;
                }
            }
        }

        _cboHocVien.SelectedIndex = viTri;

        _dangNap = false;
    }

    /// <summary>Khóa học đang chọn ở combo "Khóa học".</summary>
    private int? MaKhoaHocDangChon => (_cboKhoaHoc.SelectedItem as KhoaHocItem)?.MaKhoaHoc;

    /// <summary>
    /// Đăng ký còn hiệu lực = đang ở trong lớp (<c>Đã xếp lớp</c>) hoặc đang chờ xếp lớp
    /// (<c>Chờ xử lý</c>). Đã hủy / hoàn thành thì vẫn cho đăng ký lại.
    /// </summary>
    private static bool LaDangKyConHieuLuc(string? trangThaiDangKy)
        => trangThaiDangKy is "Chờ xử lý" or "Đã xếp lớp";

    /// <summary>Đổ danh sách CHƯƠNG TRÌNH (lấy từ các khóa học đang có) rồi nạp khóa học theo chương trình.</summary>
    private void DoVaoComboChuongTrinh()
    {
        _dangNap = true;

        var tenCu = _cboChuongTrinh.SelectedItem?.ToString();

        _cboChuongTrinh.Items.Clear();

        foreach (var ten in _khoaHocs
                     .Select(k => k.TenChuongTrinh)
                     .Where(x => !string.IsNullOrWhiteSpace(x))
                     .Distinct()
                     .OrderBy(x => x))
        {
            _cboChuongTrinh.Items.Add(ten);
        }

        if (_cboChuongTrinh.Items.Count > 0)
        {
            var viTri = tenCu is null ? -1 : _cboChuongTrinh.Items.IndexOf(tenCu);
            _cboChuongTrinh.SelectedIndex = viTri >= 0 ? viTri : 0;
        }

        _dangNap = false;

        DoVaoComboKhoaHoc();
    }

    /// <summary>Chỉ hiện KHÓA HỌC thuộc chương trình đang chọn — tên gọn, không kèm cấp độ/học phí.</summary>
    private void DoVaoComboKhoaHoc()
    {
        var chuongTrinh = _cboChuongTrinh.SelectedItem?.ToString();

        _dangNap = true;
        _cboKhoaHoc.Items.Clear();

        foreach (var kh in _khoaHocs
                     .Where(k => chuongTrinh is null || k.TenChuongTrinh == chuongTrinh)
                     .OrderBy(k => k.TenKhoaHoc))
        {
            _cboKhoaHoc.Items.Add(new KhoaHocItem(kh.MaKhoaHoc, kh.TenKhoaHoc, kh.HocPhiCoBan));
        }

        if (_cboKhoaHoc.Items.Count > 0)
        {
            _cboKhoaHoc.SelectedIndex = 0;
        }
        else
        {
            _lblThongBao.Text = _khoaHocs.Count == 0
                ? "Hệ thống chưa có khóa học nào. Hãy tạo khóa học trước."
                : $"Chương trình '{chuongTrinh}' chưa có khóa học nào.";
        }

        _dangNap = false;

        NapLopTheoKhoaHoc();
    }

    private void NapLopTheoKhoaHoc()
    {
        _cboLop.Items.Clear();

        if (_cboKhoaHoc.SelectedItem is KhoaHocItem khoaHoc)
        {
            _lblHocPhi.Text = $"{khoaHoc.HocPhiCoBan:N0} đ";

            // Chỉ hiện lớp CÒN CHỖ: đã xếp lớp + đang giữ chỗ phải nhỏ hơn sĩ số tối đa.
            foreach (var lop in _lopHocs
                         .Where(l => l.MaKhoaHoc == khoaHoc.MaKhoaHoc)
                         .Where(l => l.SiSoHienTai + l.SiSoDuKien < l.SiSoToiDa)
                         .OrderBy(l => l.MaLop))
            {
                _cboLop.Items.Add(new LopItem(lop.MaLopHoc,
                    $"{lop.MaLop} — {lop.TenLop} ({lop.SiSoHienTai + lop.SiSoDuKien}/{lop.SiSoToiDa} chỗ)"));
            }

            if (_cboLop.Items.Count == 0)
            {
                _cboLop.Items.Add(KhongConLop);
            }
        }
        else
        {
            _lblHocPhi.Text = string.Empty;
        }

        _cboLop.SelectedIndex = 0;

        // Lớp đổi theo khóa → lịch học phải tính lại.
        CapNhatLichHoc();

        // Đổi khóa cũng đổi danh sách học viên được phép đăng ký (ẩn người đã có đăng ký khóa này).
        LocHocVien();
    }

    private async Task LuuAsync()
    {
        _lblThongBao.Text = string.Empty;

        if (_cboHocVien.SelectedItem is not HocVienItem hocVien)
        {
            _lblThongBao.Text = _cboHocVien.Items.Count == 1 && _cboHocVien.Items[0] is string
                ? "Tất cả học viên đều đã có đăng ký cho khóa này — không còn ai để đăng ký. Hãy chọn khóa khác."
                : "Vui lòng chọn học viên.";
            return;
        }

        if (_cboKhoaHoc.SelectedItem is not KhoaHocItem khoaHoc)
        {
            _lblThongBao.Text = "Vui lòng chọn khóa học.";
            return;
        }

        var maLopHoc = (_cboLop.SelectedItem as LopItem)?.MaLopHoc;

        // Lớp dự kiến là BẮT BUỘC: khi thu đủ học phí hệ thống tự xếp vào lớp này.
        if (maLopHoc is null)
        {
            _lblThongBao.Text = _cboLop.Items.Count == 1 && _cboLop.Items[0] is string
                ? "Khóa này không còn lớp nào còn chỗ. Hãy mở thêm lớp hoặc chọn khóa khác."
                : "Vui lòng chọn lớp học dự kiến — học phí thu đủ sẽ tự xếp vào lớp này.";
            return;
        }

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        // KHÔNG gửi ưu đãi, KHÔNG gửi "lịch học": chỉ gửi lớp dự kiến.
        // Backend kiểm tra 5 điều kiện xếp lớp và báo lỗi nếu lớp không phù hợp.
        var result = await AppSession.Api.PostAsync<EnrollmentListItemDto>("admin/enrollments", new
        {
            maHocVien = hocVien.MaHocVien,
            maKhoaHoc = khoaHoc.MaKhoaHoc,
            maLopDuKien = maLopHoc.Value,
            ghiChu = string.IsNullOrWhiteSpace(_txtGhiChu.Text) ? null : _txtGhiChu.Text.Trim()
        });

        UseWaitCursor = false;
        _btnLuu.Enabled = true;

        if (!result.Success)
        {
            // Hiện đủ 5 điều kiện bị vi phạm (sĩ số / tuổi / trùng giờ / quá 4 buổi / học phí).
            _lblThongBao.Text = result.ThongDiepDayDu();
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

    private sealed record KhoaHocItem(int MaKhoaHoc, string MoTa, decimal HocPhiCoBan)
    {
        public override string ToString() => MoTa;
    }

    private sealed record LopItem(int MaLopHoc, string MoTa)
    {
        public override string ToString() => MoTa;
    }
}
