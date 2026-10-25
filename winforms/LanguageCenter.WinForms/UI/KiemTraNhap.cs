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
    }

    /// <summary>Đưa con trỏ về ô sai đầu tiên để người dùng sửa ngay.</summary>
    public static void FocusLoi(params Control[] controls)
    {
        var dauTien = controls.FirstOrDefault(c => c.BackColor == Theme.DangerLight);
        dauTien?.Focus();
    }
}
