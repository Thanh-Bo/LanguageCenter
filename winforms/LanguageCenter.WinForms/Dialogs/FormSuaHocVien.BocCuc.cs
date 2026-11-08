using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Bố cục GỌN dùng chung cho form hồ sơ (học viên), đồng nhất với <c>FormNhanVien</c>.
/// <para>
/// Giữ nguyên các control do Visual Studio Designer tạo (để logic tải/lưu không phải sửa)
/// nhưng xếp lại vị trí vào 2 panel:
/// </para>
/// <list type="bullet">
///   <item><c>_pnlThan</c> — Dock = Fill, <c>AutoScroll = true</c>: nhãn lỗi cao bao nhiêu dòng cũng không bị cắt</item>
///   <item><c>_pnlNut</c> — Dock = Bottom, cao 64: nút Lưu / Hủy luôn cố định ở đáy</item>
/// </list>
/// </summary>
partial class FormSuaHocVien
{
    private Panel _pnlThan = null!;
    private Panel _pnlNut = null!;

    /// <summary>Mã học viên hiển thị chỉ đọc (mã thật do backend sinh khi lưu).</summary>
    private Label _lblMa = null!;

    /// <summary>Nhãn của ô Email — đổi chữ theo tuổi (bắt buộc / không bắt buộc).</summary>
    private Label _lblEmail = null!;

    private int _doc;

    /// <summary>Dựng lại bố cục gọn. Gọi ở CUỐI constructor, sau khi đã cấu hình chế độ tạo/sửa.</summary>
    private void BocCucGon()
    {
        _pnlNut = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 64,
            BackColor = Color.White
        };

        _pnlThan = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            BackColor = Color.White
        };

        // Gỡ toàn bộ control khỏi form + groupbox cũ (control vẫn còn tham chiếu trong field,
        // KHÔNG bị dispose — chỉ tách khỏi cây hiển thị).
        Controls.Clear();

        Controls.Add(_pnlThan);
        Controls.Add(_pnlNut);

        _pnlThan.Controls.Add(new Label
        {
            Text = _isEditMode ? "CẬP NHẬT HỒ SƠ HỌC VIÊN" : "THÊM HỌC VIÊN MỚI",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 18)
        });

        _doc = 56;

        // ---- Mã học viên (chỉ đọc) ----
        _lblMa = new Label
        {
            Text = _studentId ?? "(tự động sinh khi lưu)",
            AutoSize = false,
            Size = new Size(620, 26),
            Font = Theme.FontRegular,
            ForeColor = Theme.TextDark,
            TextAlign = ContentAlignment.MiddleLeft
        };

        DatNhanVaDieuKhien("Mã học viên", _lblMa);

        // ---- Thông tin học viên ----
        ChuanHoa(textBox: txtHoTen);
        DatNhanVaDieuKhien("Họ tên *", txtHoTen);

        ChuanHoaNgay(dtpNgaySinh);
        DatNhanVaDieuKhien("Ngày sinh", dtpNgaySinh);

        ChuanHoa(comboBox: cboGioiTinh);
        DatNhanVaDieuKhien("Giới tính", cboGioiTinh);

        ChuanHoa(textBox: txtEmail);
        _lblEmail = DatNhanVaDieuKhien("Email", txtEmail);

        ChuanHoa(textBox: txtSoDienThoai);
        DatNhanVaDieuKhien("Số điện thoại", txtSoDienThoai);

        ChuanHoa(textBox: txtDiaChi);
        DatNhanVaDieuKhien("Địa chỉ", txtDiaChi);

        ChuanHoa(textBox: txtTrinhDoHienTai);
        DatNhanVaDieuKhien("Trình độ hiện tại", txtTrinhDoHienTai);

        // Mục tiêu học tập là văn bản dài → ô nhiều dòng.
        txtMucTieuHocTap.Multiline = true;
        txtMucTieuHocTap.ScrollBars = ScrollBars.Vertical;
        ChuanHoa(textBox: txtMucTieuHocTap);
        DatNhanVaDieuKhien("Mục tiêu học tập", txtMucTieuHocTap, chieuCao: 70);

        ChuanHoa(textBox: txtHoTenCha);
        DatNhanVaDieuKhien("Họ tên cha", txtHoTenCha);

        ChuanHoa(textBox: txtHoTenMe);
        DatNhanVaDieuKhien("Họ tên mẹ", txtHoTenMe);

        // ---- Chỉ có ý nghĩa với hồ sơ đã tồn tại ----
        if (_isEditMode)
        {
            ChuanHoa(comboBox: cboTrangThaiHocTap);
            DatNhanVaDieuKhien("Trạng thái học tập", cboTrangThaiHocTap);

            chkNgayNhapHoc.Text = "Đã nhập học (bật để chọn ngày)";
            chkNgayNhapHoc.AutoSize = true;
            chkNgayNhapHoc.Font = Theme.FontRegular;

            ChuanHoaNgay(dtpNgayNhapHoc);

            // Một hàng gồm: checkbox + ô ngày.
            var hang = new Panel
            {
                Size = new Size(620, 28),
                BackColor = Color.White
            };

            chkNgayNhapHoc.Location = new Point(0, 4);
            dtpNgayNhapHoc.Location = new Point(210, 0);
            dtpNgayNhapHoc.Width = 110;

            hang.Controls.Add(chkNgayNhapHoc);
            hang.Controls.Add(dtpNgayNhapHoc);

            DatNhanVaDieuKhien("Ngày nhập học", hang);
        }

        // ---- Nhãn thông báo lỗi ----
        lblMessage.AutoSize = false;
        lblMessage.Width = 852;
        lblMessage.Height = 24;
        lblMessage.ForeColor = Theme.Danger;
        lblMessage.Font = Theme.FontSub;
        lblMessage.Location = new Point(24, _doc);
        _pnlThan.Controls.Add(lblMessage);

        _doc += 34;

        // ---- Nút ----
        btnSave.Text = _isEditMode ? "Lưu thay đổi" : "Tạo học viên";
        btnSave.Location = new Point(610, 14);
        btnSave.Size = new Size(150, 36);

        btnCancel.Text = "Hủy bỏ";
        btnCancel.Location = new Point(766, 14);
        btnCancel.Size = new Size(110, 36);

        _pnlNut.Controls.Add(btnSave);
        _pnlNut.Controls.Add(btnCancel);

        AcceptButton = btnSave;
        CancelButton = btnCancel;

        // ---- Kích thước form: vừa nội dung, không vượt màn hình ----
        var gioiHan = Screen.PrimaryScreen?.WorkingArea.Height ?? 900;
        var cao = Math.Min(_doc + 12 + _pnlNut.Height, gioiHan - 80);

        ClientSize = new Size(900, Math.Max(cao, 540));

        // ---- Kiểm tra email ĐỘNG theo ngày sinh (không chờ bấm Lưu) ----
        dtpNgaySinh.ValueChanged += (_, _) => CapNhatQuyTacEmail();
        txtEmail.TextChanged += (_, _) => KiemTraEmailNgay();

        CapNhatQuyTacEmail();
    }

    /// <summary>Học viên đã từ 18 tuổi chưa (tính tại hôm nay).</summary>
    private bool LaNguoiLon => KiemTraNhap.TinhTuoi(dtpNgaySinh.Value) >= 18;

    /// <summary>
    /// Cập nhật nhãn Email theo tuổi và tô/không tô đỏ ô email.
    /// Gọi khi mở form và mỗi khi ngày sinh thay đổi.
    /// </summary>
    private void CapNhatQuyTacEmail()
    {
        if (_lblEmail is null)
        {
            return;
        }

        var laNguoiLon = LaNguoiLon;

        _lblEmail.Text = laNguoiLon
            ? "Email * (bắt buộc — học viên từ 18 tuổi)"
            : "Email (không bắt buộc — học viên dưới 18 tuổi)";

        if (string.IsNullOrWhiteSpace(txtEmail.Text))
        {
            if (laNguoiLon)
            {
                KiemTraNhap.DanhDauLoi(txtEmail);
            }
            else
            {
                KiemTraNhap.BoDanhDau(txtEmail);
            }

            return;
        }

        KiemTraEmailNgay();
    }

    /// <summary>Kiểm tra định dạng email ngay khi gõ (rỗng + dưới 18 tuổi thì bỏ qua).</summary>
    private void KiemTraEmailNgay()
    {
        var value = txtEmail.Text.Trim();

        if (value.Length == 0)
        {
            if (LaNguoiLon)
            {
                KiemTraNhap.DanhDauLoi(txtEmail);
            }
            else
            {
                KiemTraNhap.BoDanhDau(txtEmail);
            }

            return;
        }

        var loi = new List<string>();

        if (KiemTraNhap.Email(txtEmail, "Email", loi))
        {
            KiemTraNhap.BoDanhDau(txtEmail);
        }
    }

    /// <summary>Đặt nhãn mô tả phía trên rồi tịnh tiến con trỏ dọc.</summary>
    private Label DatNhanVaDieuKhien(string nhan, Control dieuKhien, int chieuCao = 0)
    {
        var nhanMo = new Label
        {
            Text = nhan,
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, _doc)
        };

        _pnlThan.Controls.Add(nhanMo);

        if (chieuCao > 0)
        {
            dieuKhien.Height = chieuCao;
        }

        dieuKhien.Location = new Point(24, _doc + 20);
        _pnlThan.Controls.Add(dieuKhien);

        _doc += 60 + Math.Max(dieuKhien.Height - 26, 0);

        return nhanMo;
    }

    private static void ChuanHoa(TextBox? textBox = null, ComboBox? comboBox = null, string? themOTrong = null)
    {
        if (textBox is not null)
        {
            textBox.Width = 620;
            textBox.Font = Theme.FontRegular;
        }

        if (comboBox is not null)
        {
            comboBox.Width = 620;
            comboBox.Font = Theme.FontRegular;
            comboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            comboBox.DropDownHeight = 320;
            comboBox.IntegralHeight = false;

            // Chèn một dòng trống ở đầu để có thể để trống giới tính.
            if (themOTrong is not null
                && (comboBox.Items.Count == 0 || Convert.ToString(comboBox.Items[0]) != themOTrong))
            {
                comboBox.Items.Insert(0, themOTrong);
                comboBox.SelectedIndex = 0;
            }
        }
    }

    private static void ChuanHoaNgay(DateTimePicker picker)
    {
        picker.Width = 620;
        picker.Font = Theme.FontRegular;
        picker.Format = DateTimePickerFormat.Custom;
        picker.CustomFormat = UiHelpers.DinhDangNgay;
    }
}
