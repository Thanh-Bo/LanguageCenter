using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace LanguageCenter.WinForms.UI;

/// <summary>
/// Kiểm tra dữ liệu nhập ở các form Thêm/Sửa (học viên, giáo viên, trợ giảng).
/// <para>
/// Cách dùng:
/// <code>
/// KiemTraNhap.BoDanhDau(txtHoTen, txtEmail);          // xoá highlight cũ
/// var loi = new List&lt;string&gt;();
/// if (!KiemTraNhap.BatBuoc(txtHoTen, "Họ tên", loi)) { }
/// KiemTraNhap.HienThi(lblMessage, loi);               // hiện danh sách lỗi rõ ràng
/// if (loi.Count > 0) return;
/// </code>
/// </para>
/// </summary>
public static class KiemTraNhap
{
    /// <summary>Regex email tối thiểu: có chữ trước/sau @ và có dấu chấm ở phần domain.</summary>
    private static readonly Regex EmailRegex =
        new(@"^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$", RegexOptions.Compiled);

    /// <summary>Tô nền đỏ nhạt + giữ nguyên chữ để người dùng thấy ngay ô nào sai.</summary>
    public static void DanhDauLoi(Control control) => control.BackColor = Theme.DangerLight;

    /// <summary>Trả các ô về nền trắng trước khi kiểm tra lại.</summary>
    public static void BoDanhDau(params Control[] controls)
    {
        foreach (var control in controls)
        {
            control.BackColor = Color.White;
        }
    }

    /// <summary>Bắt buộc nhập. Trả <c>true</c> nếu hợp lệ; ngược lại đánh dấu ô và thêm lỗi.</summary>
    public static bool BatBuoc(TextBox textBox, string tenTruong, List<string> loi, int minLength = 1)
    {
        var value = textBox.Text.Trim();

        if (value.Length < minLength)
        {
            DanhDauLoi(textBox);
            loi.Add(minLength > 1
                ? $"{tenTruong}: bắt buộc, tối thiểu {minLength} ký tự."
                : $"{tenTruong}: bắt buộc nhập.");
            return false;
        }

        return true;
    }

    /// <summary>Tuổi tại hôm nay theo ngày sinh (đã trừ năm chưa tới sinh nhật).</summary>
    public static int TinhTuoi(DateTime ngaySinh)
    {
        var ngay = ngaySinh.Date;
        var moc = DateTime.Today;

        var tuoi = moc.Year - ngay.Year;

        if (moc < ngay.AddYears(tuoi))
        {
            tuoi--;
        }

        return tuoi;
    }

    /// <summary>Email bắt buộc + đúng định dạng.</summary>
    public static bool Email(TextBox textBox, string tenTruong, List<string> loi, bool batBuoc = true)
    {
        var value = textBox.Text.Trim();

        if (value.Length == 0)
        {
            if (!batBuoc)
            {
                return true;
            }

            DanhDauLoi(textBox);
            loi.Add($"{tenTruong}: bắt buộc nhập.");
            return false;
        }

        if (!EmailRegex.IsMatch(value))
        {
            DanhDauLoi(textBox);
            loi.Add($"{tenTruong}: \"{value}\" không đúng định dạng (ví dụ: ten@gmail.com).");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Số điện thoại: không bắt buộc; nếu nhập thì <b>chỉ chữ số</b>, <b>bắt đầu bằng số 0</b>
    /// và <b>đúng 10 chữ số</b>.
    /// </summary>
    public static bool SoDienThoai(TextBox textBox, string tenTruong, List<string> loi)
    {
        var value = textBox.Text.Trim();

        if (value.Length == 0)
        {
            return true;
        }

        if (value.Any(c => !char.IsDigit(c)))
        {
            DanhDauLoi(textBox);
            loi.Add($"{tenTruong}: chỉ được nhập số (không chữ, không khoảng trắng, không ký tự đặc biệt).");
            return false;
        }

        if (!value.StartsWith('0'))
        {
            DanhDauLoi(textBox);
            loi.Add($"{tenTruong}: phải bắt đầu bằng số 0.");
            return false;
        }

        if (value.Length != 10)
        {
            DanhDauLoi(textBox);
            loi.Add($"{tenTruong}: phải có đúng 10 chữ số (hiện có {value.Length}).");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Chặn ngay khi gõ: chỉ cho nhập chữ số, tối đa <paramref name="maxLength"/> ký tự
    /// và luôn bắt đầu bằng 0 (tự thêm 0 nếu người dùng gõ số khác trước).
    /// </summary>
    public static void ChiNhapSoDienThoai(TextBox textBox)
    {
        textBox.MaxLength = 10;

        textBox.KeyPress += (_, e) =>
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        };

        textBox.TextChanged += (_, _) =>
        {
            var digits = new string(textBox.Text.Where(char.IsDigit).ToArray());

            if (digits != textBox.Text)
            {
                textBox.Text = digits;
                textBox.SelectionStart = digits.Length;
            }
        };
    }

    /// <summary>Ngày sinh: không ở tương lai, không trước năm 1900.</summary>
    public static bool NgaySinh(DateTimePicker picker, string tenTruong, List<string> loi)
    {
        var value = picker.Value.Date;

        if (value > DateTime.Today)
        {
            DanhDauLoi(picker);
            loi.Add($"{tenTruong}: không được ở tương lai.");
            return false;
        }

        if (value.Year < 1900)
        {
            DanhDauLoi(picker);
            loi.Add($"{tenTruong}: không hợp lệ (trước năm 1900).");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Tuổi tối thiểu tại hôm nay — học viên từ 3 tuổi, nhân viên từ 18 tuổi.
    /// Ngày sinh không hợp lệ (tương lai / &lt; 1900) đã được <see cref="NgaySinh"/> báo.
    /// </summary>
    /// <param name="doiTuong">Chủ thể trong thông báo lỗi — "học viên" / "nhân viên".</param>
    public static bool TuoiToiThieu(
        DateTimePicker picker,
        string tenTruong,
        int tuoiToiThieu,
        List<string> loi,
        string doiTuong = "học viên")
    {
        var ngaySinh = picker.Value.Date;
        var moc = DateTime.Today;

        var tuoi = moc.Year - ngaySinh.Year;

        if (moc < ngaySinh.AddYears(tuoi))
        {
            tuoi--;
        }

        if (tuoi < tuoiToiThieu)
        {
            DanhDauLoi(picker);
            loi.Add($"{tenTruong}: {doiTuong} phải từ {tuoiToiThieu} tuổi trở lên (hiện {tuoi} tuổi).");
            return false;
        }

        return true;
    }

    /// <summary>ComboBox bắt buộc chọn (dùng cho DropDownList).</summary>
    public static bool ChonBatBuoc(ComboBox comboBox, string tenTruong, List<string> loi)
    {
        if (string.IsNullOrWhiteSpace(comboBox.Text))
        {
            DanhDauLoi(comboBox);
            loi.Add($"{tenTruong}: bắt buộc chọn.");
            return false;
        }

        return true;
    }

    /// <summary>Giới hạn độ dài tối đa (khớp StringLength của backend).</summary>
    public static bool DoDai(TextBox textBox, string tenTruong, int maxLength, List<string> loi)
    {
        if (textBox.Text.Trim().Length <= maxLength)
        {
            return true;
        }

        DanhDauLoi(textBox);
        loi.Add($"{tenTruong}: tối đa {maxLength} ký tự.");
        return false;
    }

    /// <summary>
    /// Hiện danh sách lỗi trong label: tiêu đề + mỗi lỗi một dòng "• ...".
    /// <c>loi</c> rỗng → xoá nội dung và tô màu xanh (dùng cho thông báo thành công).
    /// </summary>
    public static void HienThiLoi(Label label, IReadOnlyList<string> loi)
    {
        if (loi.Count == 0)
        {
            label.Text = string.Empty;
            return;
        }

        label.ForeColor = Theme.Danger;
        label.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        label.Text = $"Vui lòng kiểm tra lại {loi.Count} thông tin:"
            + Environment.NewLine
            + "• " + string.Join(Environment.NewLine + "• ", loi);
    }

    /// <summary>Hiện thông báo thành công (xanh) trong cùng label lỗi.</summary>
    public static void HienThiThanhCong(Label label, string thongDiep)
    {
        label.ForeColor = Theme.Success;
        label.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        label.Text = thongDiep;
        VuaChieuCaoNoiDung(label);
    }

    /// <summary>
    /// Hiện danh sách lỗi và <b>TỰ NỚI chiều cao label</b> theo số dòng thực tế,
    /// rồi cuộn panel cha tới chỗ label.
    /// <para>
    /// Vì sao cần: <see cref="HienThiLoi"/> ghi nhiều dòng ("Vui lòng kiểm tra lại N thông tin:"
    /// + mỗi lỗi một dòng "• ...") vào label có <c>AutoSize = false</c>. Nếu label chỉ cao
    /// 1 dòng thì các lỗi phía dưới bị CẮT MẤT, người dùng chỉ thấy dòng tiêu đề mà không
    /// biết sai ở đâu.
    /// </para>
    /// <para>
    /// Label phải nằm trong một <see cref="ScrollableControl"/> có <c>AutoScroll = true</c>
    /// (ví dụ <c>Panel</c>) để khi phình cao quá khung form vẫn cuộn tới được.
    /// </para>
    /// </summary>
    public static void HienThiLoiTuDong(Label label, IReadOnlyList<string> loi)
    {
        HienThiLoi(label, loi);

        if (loi.Count == 0)
        {
            return;
        }

        VuaChieuCaoNoiDung(label);

        (label.Parent as ScrollableControl)?.ScrollControlIntoView(label);
    }

    /// <summary>
    /// Đặt chiều cao label vừa đúng nội dung hiện tại (đo theo bề rộng thực tế, có ngắt dòng).
    /// Label phải đang ở trạng thái <c>AutoSize = false</c>.
    /// </summary>
    private static void VuaChieuCaoNoiDung(Label label)
    {
        if (label.AutoSize || string.IsNullOrEmpty(label.Text))
        {
            return;
        }

        var beRong = label.Width > 0 ? label.Width : 400;
        var font = label.Font ?? Theme.FontSub;

        var kichThuoc = TextRenderer.MeasureText(
            label.Text,
            font,
            new Size(beRong, int.MaxValue),
            TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);

        label.Height = Math.Max(kichThuoc.Height + 8, 24);
    }

    // ========================================================================
    // TỰ ĐỘNG NỚI CHỖ CHỨA THÔNG BÁO — gọi 1 lần cho MỌI form Thêm/Sửa
    // ========================================================================

    /// <summary>Đã gắn bộ điều chỉnh cho label nào chưa (tránh gắn 2 lần).</summary>
    private static readonly ConditionalWeakTable<Label, BoDieuChinhNhan> DaGan = new();

    /// <summary>
    /// Gắn cơ chế <b>tự nới chỗ chứa thông báo</b> cho toàn bộ form.
    /// <para>
    /// Gọi MỘT LẦN ở cuối constructor (sau khi đã dựng xong giao diện):
    /// <code>KiemTraNhap.BatTuNoiChieuCao(this);</code>
    /// </para>
    /// <para>
    /// Sau đó, mỗi khi <b>bất kỳ</b> label thông báo nào trên form đổi <c>Text</c> (dù do
    /// <see cref="HienThiLoi"/>, do <c>lblMessage.Text = ...</c> hay do API trả lỗi),
    /// label tự cao lên vừa đủ số dòng, các control nằm dưới được đẩy xuống tương ứng
    /// và form tự cao thêm (hoặc bật thanh cuộn nếu đã chạm mép màn hình).
    /// Nhờ vậy <b>không lỗi nào bị che khuất</b> mà không phải sửa tay từng form.
    /// </para>
    /// </summary>
    public static void BatTuNoiChieuCao(Form form)
    {
        if (form is null)
        {
            return;
        }

        // Tiện thể chuẩn hoá luôn định dạng ngày của mọi DateTimePicker về dd/MM/yyyy
        // (Format = Short phụ thuộc locale máy → máy en-US hiển thị MM/dd/yyyy).
        UiHelpers.ApDungDinhDangNgay(form);

        GanNhanTrong(form, form);
    }

    private static void GanNhanTrong(Control cha, Form form)
    {
        foreach (Control con in cha.Controls)
        {
            // Nhãn thông báo của các form này đều được khai báo AutoSize = false + Size cố định.
            // Nhãn tiêu đề / nhãn trường dùng AutoSize = true nên KHÔNG bị ảnh hưởng.
            if (con is Label nhan && !nhan.AutoSize)
            {
                Gan(nhan, form);
            }

            if (con.HasChildren)
            {
                GanNhanTrong(con, form);
            }
        }
    }

    private static void Gan(Label label, Form form)
    {
        if (DaGan.TryGetValue(label, out _))
        {
            return;
        }

        var bo = new BoDieuChinhNhan(label, form);
        DaGan.Add(label, bo);

        // KHÔNG gọi ApDung() ngay: bố cục lúc mở form giữ nguyên 100% như thiết kế.
        // Chỉ khi nội dung thông báo THAY ĐỔI (lỗi / thành công mới) mới nới chỗ.
        label.TextChanged += (_, _) => bo.ApDung();
    }

    /// <summary>
    /// Theo dõi MỘT label thông báo: nhớ kích thước gốc + vị trí gốc của các control nằm dưới,
    /// rồi mỗi lần nội dung đổi thì tính lại chiều cao và đẩy bố cục theo.
    /// </summary>
    private sealed class BoDieuChinhNhan
    {
        private const int LeDuoi = 12;

        private readonly Label _label;
        private readonly Form _form;

        private readonly int _caoGoc;
        private readonly int _clientCaoGoc;
        private readonly List<(Control Control, int Top)> _duoi = new();

        public BoDieuChinhNhan(Label label, Form form)
        {
            _label = label;
            _form = form;

            _caoGoc = label.Height;
            _clientCaoGoc = form.ClientSize.Height;

            // Chỉ nhớ các control KHÔNG dock nằm dưới nhãn — đó là những control cần bị đẩy xuống.
            if (label.Parent is null)
            {
                return;
            }

            var dayGoc = label.Bottom;

            foreach (Control anhEm in label.Parent.Controls)
            {
                if (ReferenceEquals(anhEm, label) || anhEm.Dock != DockStyle.None)
                {
                    continue;
                }

                if (anhEm.Top >= dayGoc)
                {
                    _duoi.Add((anhEm, anhEm.Top));
                }
            }
        }

        public void ApDung()
        {
            if (_form.IsDisposed || _label.IsDisposed)
            {
                return;
            }

            var caoMoi = TinhCaoMoi();
            var delta = Math.Max(caoMoi - _caoGoc, 0);

            if (_label.Height != caoMoi)
            {
                _label.Height = caoMoi;
            }

            foreach (var (control, topGoc) in _duoi)
            {
                if (control.IsDisposed)
                {
                    continue;
                }

                var topMongMuon = topGoc + delta;

                if (control.Top != topMongMuon)
                {
                    control.Top = topMongMuon;
                }
            }

            NoiKhung();
        }

        /// <summary>Chiều cao cần thiết cho nội dung hiện tại (không nhỏ hơn chiều cao gốc).</summary>
        private int TinhCaoMoi()
        {
            if (string.IsNullOrEmpty(_label.Text))
            {
                return _caoGoc;
            }

            var beRong = _label.Width > 0 ? _label.Width : 400;
            var font = _label.Font ?? Theme.FontSub;

            var kichThuoc = TextRenderer.MeasureText(
                _label.Text,
                font,
                new Size(beRong, int.MaxValue),
                TextFormatFlags.WordBreak | TextFormatFlags.TextBoxControl);

            return Math.Max(_caoGoc, kichThuoc.Height + 8);
        }

        /// <summary>
        /// Nới form cho đủ chỗ. Nếu đã chạm mép dưới màn hình thì bật thanh cuộn thay vì cao thêm.
        /// </summary>
        private void NoiKhung()
        {
            // Nhãn nằm trong panel / groupbox (form tự quản lý việc cuộn) → không nới form.
            if (!ReferenceEquals(_label.Parent, _form))
            {
                return;
            }

            var dayNoiDung = _label.Bottom;

            foreach (var (control, _) in _duoi)
            {
                if (!control.IsDisposed)
                {
                    dayNoiDung = Math.Max(dayNoiDung, control.Bottom);
                }
            }

            var caoMongMuon = Math.Max(_clientCaoGoc, dayNoiDung + LeDuoi);
            var manHinh = Screen.FromControl(_form).WorkingArea;
            var gioiHan = manHinh.Height - 80;

            if (caoMongMuon <= gioiHan)
            {
                _form.AutoScroll = false;

                if (_form.ClientSize.Height != caoMongMuon)
                {
                    _form.ClientSize = new Size(_form.ClientSize.Width, caoMongMuon);
                }

                // Form cao thêm có thể tràn khỏi mép dưới màn hình → kéo lên cho vừa.
                if (_form.Visible && _form.Bottom > manHinh.Bottom)
                {
                    _form.Top = Math.Max(manHinh.Top, manHinh.Bottom - _form.Height - 8);
                }

                return;
            }

            // Hết chỗ → cho cuộn để mọi dòng lỗi vẫn đọc được.
            _form.AutoScroll = true;
            _form.AutoScrollMinSize = new Size(0, caoMongMuon);
            _form.ScrollControlIntoView(_label);
        }
    }
    /// <summary>Đưa con trỏ về ô sai đầu tiên để người dùng sửa ngay.</summary>
    public static void FocusLoi(params Control[] controls)
    {
        var dauTien = controls.FirstOrDefault(c => c.BackColor == Theme.DangerLight);
        dauTien?.Focus();
    }
}
