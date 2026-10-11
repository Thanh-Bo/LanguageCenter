using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

public record CourseModel(
    string MaKhoaHoc,
    string TenKhoaHoc,
    string TenChuongTrinh,
    string CapDo,
    int SoBuoi,
    decimal TongSoGio,
    decimal HocPhiCoBan,
    DateOnly NgayBatDau,
    DateOnly NgayKetThuc,
    string SiSo,
    string TrangThai
);

public partial class UcCourses : UserControl
{
    private const string AllOption = "— Tất cả —";
    private readonly List<CourseModel> _courses = new();

    public UcCourses()
    {
        InitializeComponent();
        InitStats();
        InitFilterOptions();
        LoadSampleCourses();
    }

    private void InitStats()
    {
        tblStats.Controls.Clear();
        tblStats.Controls.Add(CreateStatCard("Tổng số khóa học", "8 Khóa", "Các cấp độ từ A1 đến C1", Theme.Primary), 0, 0);
        tblStats.Controls.Add(CreateStatCard("Chương trình đào tạo", "4 Hệ", "IELTS, TOEIC, Giao tiếp, Thiếu nhi", Theme.Purple), 1, 0);
        tblStats.Controls.Add(CreateStatCard("Đang tuyển sinh", "5 Khóa", "Tuyển sinh cho kỳ học mới", Theme.Success), 2, 0);
        tblStats.Controls.Add(CreateStatCard("Tổng chỉ tiêu", "147 HV", "Học viên các lớp học", Theme.Warning), 3, 0);
    }

    private static Panel CreateStatCard(string title, string value, string note, Color accent)
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Padding = new Padding(12, 8, 12, 8),
            Margin = new Padding(4, 0, 4, 0)
        };
        panel.Paint += (s, e) =>
        {
            if (s is Control ctrl)
            {
                using var borderPen = new Pen(Theme.BorderColor);
                e.Graphics.DrawRectangle(borderPen, 0, 0, ctrl.Width - 1, ctrl.Height - 1);
                using var accentBrush = new SolidBrush(accent);
                e.Graphics.FillRectangle(accentBrush, 0, 0, 4, ctrl.Height);
            }
        };

        var lblT = new Label { Text = title, Font = new Font("Segoe UI", 8.5f), ForeColor = Theme.Muted, AutoSize = true, Location = new Point(12, 6) };
        var lblV = new Label { Text = value, Font = new Font("Segoe UI", 16f, FontStyle.Bold), ForeColor = accent, AutoSize = true, Location = new Point(10, 24) };
        var lblN = new Label { Text = note, Font = new Font("Segoe UI", 7.8f), ForeColor = Theme.Muted, AutoSize = true, Location = new Point(12, 53) };

        panel.Controls.Add(lblN);
        panel.Controls.Add(lblV);
        panel.Controls.Add(lblT);
        return panel;
    }

    private void InitFilterOptions()
    {
        cboChuongTrinh.Items.Add(AllOption);
        cboChuongTrinh.Items.AddRange(["IELTS Academic", "Tiếng Anh Giao tiếp", "Luyện thi TOEIC", "Tiếng Anh Thiếu nhi"]);
        cboChuongTrinh.SelectedIndex = 0;

        cboTrangThai.Items.Add(AllOption);
        cboTrangThai.Items.AddRange(["Đang tuyển sinh", "Đang diễn ra", "Sắp khai giảng"]);
        cboTrangThai.SelectedIndex = 0;
    }

    private void LoadSampleCourses()
    {
        _courses.Clear();
        _courses.AddRange([
            new CourseModel("KH-IELTS-75", "IELTS Master 7.5+ Chuyên sâu", "IELTS Academic", "Cao cấp (Band 7.5+)", 36, 72, 12500000m, new DateOnly(2026, 10, 15), new DateOnly(2027, 1, 15), "18/20", "Đang tuyển sinh"),
            new CourseModel("KH-IELTS-65", "IELTS Intensive 6.5 Cấp tốc", "IELTS Academic", "Trung cấp (Band 6.0-6.5)", 30, 60, 9800000m, new DateOnly(2026, 10, 20), new DateOnly(2027, 1, 10), "16/20", "Đang tuyển sinh"),
            new CourseModel("KH-COM-PRO", "Tiếng Anh Giao tiếp phản xạ PRO", "Tiếng Anh Giao tiếp", "Trung cấp (B1-B2)", 24, 48, 6800000m, new DateOnly(2026, 10, 1), new DateOnly(2026, 11, 25), "15/15", "Đang diễn ra"),
            new CourseModel("KH-COM-BAS", "Tiếng Anh Giao tiếp căn bản (Beginner)", "Tiếng Anh Giao tiếp", "Căn bản (A1-A2)", 20, 40, 4500000m, new DateOnly(2026, 11, 1), new DateOnly(2026, 12, 20), "8/15", "Sắp khai giảng"),
            new CourseModel("KH-TOEIC-65", "Luyện thi TOEIC Mục tiêu 650+", "Luyện thi TOEIC", "Trung cấp (Intermediate)", 24, 48, 5500000m, new DateOnly(2026, 10, 18), new DateOnly(2026, 12, 15), "12/20", "Đang tuyển sinh"),
            new CourseModel("KH-TOEIC-80", "Luyện thi TOEIC Chuyên sâu 800+", "Luyện thi TOEIC", "Nâng cao (Advanced)", 28, 56, 7200000m, new DateOnly(2026, 11, 5), new DateOnly(2027, 1, 10), "6/20", "Sắp khai giảng"),
            new CourseModel("KH-KIDS-STA", "Tiếng Anh Thiếu nhi Starters", "Tiếng Anh Thiếu nhi", "Starters (Pre A1)", 32, 64, 7200000m, new DateOnly(2026, 10, 12), new DateOnly(2027, 2, 12), "14/16", "Đang tuyển sinh"),
            new CourseModel("KH-KIDS-MOV", "Tiếng Anh Thiếu nhi Movers", "Tiếng Anh Thiếu nhi", "Movers (A1)", 32, 64, 7500000m, new DateOnly(2026, 10, 15), new DateOnly(2027, 2, 15), "10/16", "Đang tuyển sinh")
        ]);

        FilterData();
    }

    private void FilterData()
    {
        var search = txtSearch.Text.Trim();
        var prog = cboChuongTrinh.SelectedItem?.ToString();
        var status = cboTrangThai.SelectedItem?.ToString();

        var query = _courses.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.TenKhoaHoc.Contains(search, StringComparison.OrdinalIgnoreCase)
                                  || c.MaKhoaHoc.Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(prog) && prog != AllOption)
        {
            query = query.Where(c => c.TenChuongTrinh == prog);
        }

        if (!string.IsNullOrWhiteSpace(status) && status != AllOption)
        {
            query = query.Where(c => c.TrangThai == status);
        }

        var result = query.ToList();
        dgvCourses.Rows.Clear();

        foreach (var c in result)
        {
            var idx = dgvCourses.Rows.Add(
                c.MaKhoaHoc,
                c.TenKhoaHoc,
                c.TenChuongTrinh,
                c.CapDo,
                $"{c.SoBuoi} buổi",
                $"{c.TongSoGio}h",
                $"{c.HocPhiCoBan:N0} ₫",
                c.NgayBatDau.ToString("dd/MM/yyyy"),
                c.SiSo,
                c.TrangThai
            );

            if (c.TrangThai.Contains("tuyển sinh"))
                dgvCourses.Rows[idx].Cells["TrangThai"].Style.ForeColor = Theme.Success;
            else if (c.TrangThai.Contains("diễn ra"))
                dgvCourses.Rows[idx].Cells["TrangThai"].Style.ForeColor = Theme.Danger;
            else
                dgvCourses.Rows[idx].Cells["TrangThai"].Style.ForeColor = Theme.Warning;
        }

        lblSummary.Text = $"Hiển thị {result.Count} / {_courses.Count} khóa học đào tạo (Mock Data)";
    }

    private void ReloadData()
    {
        txtSearch.Clear();
        cboChuongTrinh.SelectedIndex = 0;
        cboTrangThai.SelectedIndex = 0;
        FilterData();
    }

    private void BtnAdd_Click(object? sender, EventArgs e)
    {
        using var dlg = new Form
        {
            Text = "Thêm khóa học đào tạo mới",
            Width = 480,
            Height = 420,
            FormBorderStyle = FormBorderStyle.FixedDialog,
            StartPosition = FormStartPosition.CenterParent,
            MaximizeBox = false,
            MinimizeBox = false,
            BackColor = Color.White
        };

        var lblName = new Label { Text = "Tên khóa học:", Location = new Point(24, 20), AutoSize = true, Font = Theme.FontBold };
        var txtName = new TextBox { Location = new Point(24, 44), Width = 416, Font = Theme.FontRegular };

        var lblProg = new Label { Text = "Chương trình đào tạo:", Location = new Point(24, 84), AutoSize = true, Font = Theme.FontBold };
        var cboProg = new ComboBox { Location = new Point(24, 108), Width = 416, DropDownStyle = ComboBoxStyle.DropDownList, Font = Theme.FontRegular };
        cboProg.Items.AddRange(["IELTS Academic", "Tiếng Anh Giao tiếp", "Luyện thi TOEIC", "Tiếng Anh Thiếu nhi"]);
        cboProg.SelectedIndex = 0;

        var lblFee = new Label { Text = "Học phí niêm yết (VNĐ):", Location = new Point(24, 148), AutoSize = true, Font = Theme.FontBold };
        var txtFee = new TextBox { Location = new Point(24, 172), Width = 196, Text = "8500000", Font = Theme.FontRegular };

        var lblLessons = new Label { Text = "Số buổi học:", Location = new Point(244, 148), AutoSize = true, Font = Theme.FontBold };
        var txtLessons = new TextBox { Location = new Point(244, 172), Width = 196, Text = "30", Font = Theme.FontRegular };

        var lblStart = new Label { Text = "Ngày khai giảng (dd/MM/yyyy):", Location = new Point(24, 212), AutoSize = true, Font = Theme.FontBold };
        var txtStart = new TextBox { Location = new Point(24, 236), Width = 416, Text = DateTime.Today.AddDays(15).ToString("dd/MM/yyyy"), Font = Theme.FontRegular };

        var btnSubmit = new Button
        {
            Text = "Lưu khóa học",
            Location = new Point(200, 310),
            Size = new Size(130, 36),
            BackColor = Theme.Primary,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat,
            Font = Theme.FontBold,
            DialogResult = DialogResult.OK
        };
        btnSubmit.FlatAppearance.BorderSize = 0;

        var btnClose = new Button
        {
            Text = "Hủy",
            Location = new Point(340, 310),
            Size = new Size(100, 36),
            BackColor = Color.FromArgb(241, 245, 249),
            ForeColor = Theme.TextDark,
            FlatStyle = FlatStyle.Flat,
            DialogResult = DialogResult.Cancel
        };
        btnClose.FlatAppearance.BorderSize = 0;

        dlg.Controls.AddRange([lblName, txtName, lblProg, cboProg, lblFee, txtFee, lblLessons, txtLessons, lblStart, txtStart, btnSubmit, btnClose]);
        dlg.AcceptButton = btnSubmit;

        if (dlg.ShowDialog(FindForm()) == DialogResult.OK && !string.IsNullOrWhiteSpace(txtName.Text))
        {
            var fee = decimal.TryParse(txtFee.Text, out var f) ? f : 8000000m;
            var lessons = int.TryParse(txtLessons.Text, out var l) ? l : 30;
            var newCourse = new CourseModel(
                $"KH-NEW-{_courses.Count + 1:D2}",
                txtName.Text.Trim(),
                cboProg.SelectedItem?.ToString() ?? "IELTS Academic",
                "Tiêu chuẩn",
                lessons,
                lessons * 2,
                fee,
                DateOnly.FromDateTime(DateTime.Today.AddDays(15)),
                DateOnly.FromDateTime(DateTime.Today.AddMonths(3)),
                "0/20",
                "Đang tuyển sinh"
            );
            _courses.Insert(0, newCourse);
            FilterData();
            MessageBox.Show(FindForm(), $"Đã thêm khóa học \"{newCourse.TenKhoaHoc}\" thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void DgvCourses_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0) return;
        var code = dgvCourses.Rows[e.RowIndex].Cells["MaKhoaHoc"].Value?.ToString();
        var course = _courses.FirstOrDefault(c => c.MaKhoaHoc == code);
        if (course is not null)
        {
            MessageBox.Show(
                FindForm(),
                $"Mã khóa học: {course.MaKhoaHoc}\n" +
                $"Tên khóa học: {course.TenKhoaHoc}\n" +
                $"Chương trình: {course.TenChuongTrinh}\n" +
                $"Cấp độ: {course.CapDo}\n" +
                $"Thời lượng: {course.SoBuoi} buổi ({course.TongSoGio} giờ đào tạo)\n" +
                $"Học phí niêm yết: {course.HocPhiCoBan:N0} VNĐ\n" +
                $"Lịch trình: {course.NgayBatDau:dd/MM/yyyy} đến {course.NgayKetThuc:dd/MM/yyyy}\n" +
                $"Sĩ số: {course.SiSo}\n" +
                $"Trạng thái: {course.TrangThai}",
                $"Chi tiết khóa học — {course.MaKhoaHoc}",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }
    }
}

