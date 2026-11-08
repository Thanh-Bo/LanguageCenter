using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Bố cục GỌN dùng chung cho form hồ sơ (giáo viên / trợ giảng), đồng nhất với <c>FormNhanVien</c>.
/// <para>
/// Form được dựng bằng Visual Studio Designer từ trước, nay <b>giữ nguyên các control</b>
/// (để toàn bộ logic tải/lưu không phải sửa) nhưng <b>xếp lại vị trí</b> vào 2 panel:
/// </para>
/// <list type="bullet">
///   <item><c>_pnlThan</c> — Dock = Fill, <c>AutoScroll = true</c>: nhãn lỗi cao bao nhiêu dòng cũng không bị cắt</item>
///   <item><c>_pnlNut</c> — Dock = Bottom, cao 64: nút Lưu / Hủy luôn nằm cố định ở đáy, không bị đè</item>
/// </list>
/// </summary>
partial class FormSuaGiaoVien
{
    private Panel _pnlThan = null!;
    private Panel _pnlNut = null!;

    /// <summary>Mã hồ sơ hiển thị chỉ đọc (mã thật do backend sinh khi lưu).</summary>
    private Label _lblMa = null!;

    /// <summary>Loại hồ sơ (Giáo viên / Trợ giảng) — cố định theo tab đang mở.</summary>
    private Label _lblLoai = null!;

    /// <summary>Bằng cấp / chứng chỉ — GÕ TAY, sửa được, KHÔNG đính kèm file.</summary>
    private TextBox _txtBangCap = null!;

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

        // Gỡ toàn bộ control khỏi form + groupbox cũ. Control vẫn còn tham chiếu trong field
        // nên KHÔNG bị dispose — chỉ tách khỏi cây hiển thị.
        Controls.Clear();

        Controls.Add(_pnlThan);
        Controls.Add(_pnlNut);

        _pnlThan.Controls.Add(new Label
        {
            Text = _isEditMode ? "CẬP NHẬT HỒ SƠ" : "THÊM HỒ SƠ MỚI",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 18)
        });

        _doc = 56;

        // ---- Mã hồ sơ (chỉ đọc) ----
        _lblMa = TaoNhanGiaTri(_teacherId ?? "(tự động sinh khi lưu)");
        DatNhanVaDieuKhien(
            LaTroGiang ? "Mã trợ giảng" : "Mã giáo viên",
            _lblMa);

        // ---- Loại hồ sơ (chỉ đọc, in đậm) ----
        _lblLoai = TaoNhanGiaTri(LoaiGiaoVien);
        _lblLoai.Font = new Font(Theme.FontRegular, FontStyle.Bold);
        DatNhanVaDieuKhien("Loại", _lblLoai);

        // ---- Thông tin cá nhân ----
        ChuanHoa(textBox: txtHoTen);
        DatNhanVaDieuKhien("Họ tên *", txtHoTen);

        ChuanHoaNgay(dtpNgaySinh);
        DatNhanVaDieuKhien("Ngày sinh", dtpNgaySinh);

        ChuanHoa(comboBox: cboGioiTinh);
        DatNhanVaDieuKhien("Giới tính *", cboGioiTinh);

        ChuanHoa(textBox: txtEmail);
        DatNhanVaDieuKhien("Email *", txtEmail);

        ChuanHoa(textBox: txtSoDienThoai);
        DatNhanVaDieuKhien("Số điện thoại *", txtSoDienThoai);

        ChuanHoa(textBox: txtDiaChi);
        DatNhanVaDieuKhien("Địa chỉ *", txtDiaChi);

        ChuanHoa(textBox: txtChuyenMon);
        DatNhanVaDieuKhien("Chuyên môn *", txtChuyenMon);

        numSoNamKinhNghiem.Width = 140;
        numSoNamKinhNghiem.Font = Theme.FontRegular;
        DatNhanVaDieuKhien("Số năm kinh nghiệm", numSoNamKinhNghiem);

        // ---- Chỉ có ý nghĩa với hồ sơ đã tồn tại ----
        if (_isEditMode)
        {
            ChuanHoa(comboBox: cboTrangThai);
            DatNhanVaDieuKhien("Trạng thái làm việc", cboTrangThai);
        }

        // ---- Bằng cấp & chứng chỉ: gõ tay, sửa được, không đính kèm file ----
        _txtBangCap = UiHelpers.CreateTextBox(620);
        _txtBangCap.Multiline = true;
        _txtBangCap.ScrollBars = ScrollBars.Vertical;
        _txtBangCap.MaxLength = 1000;
        _txtBangCap.Font = Theme.FontRegular;
        _txtBangCap.PlaceholderText = "Ví dụ: Cử nhân Ngôn ngữ Anh — ĐH Sư phạm TP.HCM (2018); IELTS 8.0 (2023)";

        DatNhanVaDieuKhien("Bằng cấp & chứng chỉ", _txtBangCap, chieuCao: 96);

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
        btnSave.Text = _isEditMode ? "Lưu thay đổi" : $"Tạo {LoaiGiaoVien.ToLowerInvariant()}";
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
    }

    /// <summary>Nhãn "nhãn trên + giá trị" cho các trường chỉ đọc (mã, loại).</summary>
    private static Label TaoNhanGiaTri(string giaTri) => new()
    {
        Text = giaTri,
        AutoSize = false,
        Size = new Size(620, 26),
        Font = Theme.FontRegular,
        ForeColor = Theme.TextDark,
        TextAlign = ContentAlignment.MiddleLeft
    };

    /// <summary>Đặt nhãn mô tả phía trên rồi tịnh tiến con trỏ dọc.</summary>
    private void DatNhanVaDieuKhien(string nhan, Control dieuKhien, int chieuCao = 0)
    {
        _pnlThan.Controls.Add(new Label
        {
            Text = nhan,
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, _doc)
        });

        if (chieuCao > 0)
        {
            dieuKhien.Height = chieuCao;
        }

        dieuKhien.Location = new Point(24, _doc + 20);
        _pnlThan.Controls.Add(dieuKhien);

        _doc += 60 + Math.Max(dieuKhien.Height - 26, 0);
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

            // Chèn một dòng trống ở đầu để "bắt buộc chọn" có ý nghĩa.
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
