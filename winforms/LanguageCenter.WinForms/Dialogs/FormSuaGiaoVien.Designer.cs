namespace LanguageCenter.WinForms.Dialogs;

partial class FormSuaGiaoVien
{
    private System.ComponentModel.IContainer components = null;

    private GroupBox grpInfo = null!;
    private Label lblHoTen = null!;
    private TextBox txtHoTen = null!;
    private CheckBox chkNgaySinh = null!;
    private DateTimePicker dtpNgaySinh = null!;
    private Label lblGioiTinh = null!;
    private ComboBox cboGioiTinh = null!;
    private Label lblEmail = null!;
    private TextBox txtEmail = null!;
    private Label lblSoDienThoai = null!;
    private TextBox txtSoDienThoai = null!;
    private Label lblDiaChi = null!;
    private TextBox txtDiaChi = null!;
    private Label lblLoaiGiaoVien = null!;
    private ComboBox cboLoaiGiaoVien = null!;
    private Label lblSoNamKinhNghiem = null!;
    private NumericUpDown numSoNamKinhNghiem = null!;
    private Label lblChuyenMon = null!;
    private TextBox txtChuyenMon = null!;
    private CheckBox chkNgayVaoLam = null!;
    private DateTimePicker dtpNgayVaoLam = null!;

    private GroupBox grpStatus = null!;
    private Label lblTrangThai = null!;
    private ComboBox cboTrangThai = null!;

    private GroupBox grpQualifications = null!;
    private Label lblQualificationsReadonly = null!;

    private Label lblMessage = null!;
    private Button btnSave = null!;
    private Button btnCancel = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();

        grpInfo = new GroupBox();
        lblHoTen = new Label { Text = "Họ tên (*)", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(16, 28) };
        txtHoTen = new TextBox { Width = 460, Font = new Font("Segoe UI", 9.5f), Location = new Point(16, 48) };
        chkNgaySinh = new CheckBox { Text = "Có ngày sinh", AutoSize = true, Font = new Font("Segoe UI", 9.5f), Location = new Point(16, 84) };
        dtpNgaySinh = new DateTimePicker { Width = 220, Format = DateTimePickerFormat.Short, Enabled = false, Location = new Point(16, 104) };
        lblGioiTinh = new Label { Text = "Giới tính", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(250, 84) };
        cboGioiTinh = new ComboBox { Width = 210, DropDownStyle = ComboBoxStyle.DropDown, Font = new Font("Segoe UI", 9.5f), Location = new Point(250, 104) };
        lblEmail = new Label { Text = "Email (*)", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(16, 140) };
        txtEmail = new TextBox { Width = 220, Font = new Font("Segoe UI", 9.5f), Location = new Point(16, 160) };
        lblSoDienThoai = new Label { Text = "Số điện thoại", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(250, 140) };
        txtSoDienThoai = new TextBox { Width = 210, Font = new Font("Segoe UI", 9.5f), Location = new Point(250, 160) };
        lblDiaChi = new Label { Text = "Địa chỉ", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(16, 196) };
        txtDiaChi = new TextBox { Width = 460, Font = new Font("Segoe UI", 9.5f), Location = new Point(16, 216) };
        lblLoaiGiaoVien = new Label { Text = "Loại giáo viên (*)", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(16, 252) };
        cboLoaiGiaoVien = new ComboBox { Width = 210, Font = new Font("Segoe UI", 9.5f), DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(16, 272) };
        lblSoNamKinhNghiem = new Label { Text = "Số năm kinh nghiệm", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(250, 252) };
        numSoNamKinhNghiem = new NumericUpDown { Width = 100, Minimum = 0, Maximum = 80, Font = new Font("Segoe UI", 9.5f), Location = new Point(250, 272) };
        lblChuyenMon = new Label { Text = "Chuyên môn", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(16, 308) };
        txtChuyenMon = new TextBox { Width = 460, Font = new Font("Segoe UI", 9.5f), Location = new Point(16, 328) };
        chkNgayVaoLam = new CheckBox { Text = "Có ngày vào làm", AutoSize = true, Font = new Font("Segoe UI", 9.5f), Location = new Point(16, 364) };
        dtpNgayVaoLam = new DateTimePicker { Width = 220, Format = DateTimePickerFormat.Short, Enabled = false, Location = new Point(16, 384) };

        grpStatus = new GroupBox();
        lblTrangThai = new Label { Text = "Trạng thái làm việc", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(16, 26) };
        cboTrangThai = new ComboBox { Width = 220, Font = new Font("Segoe UI", 9.5f), DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(16, 46) };

        grpQualifications = new GroupBox();
        lblQualificationsReadonly = new Label
        {
            AutoSize = false,
            Font = new Font("Segoe UI", 9.5f),
            ForeColor = Color.FromArgb(15, 23, 42),
            Location = new Point(16, 26),
            Size = new Size(468, 124)
        };

        lblMessage = new Label();
        btnSave = new Button
        {
            Text = "Lưu",
            Size = new Size(140, 36),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(30, 86, 240),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnSave.FlatAppearance.BorderSize = 0;

        btnCancel = new Button
        {
            Text = "Hủy",
            Size = new Size(110, 36),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(15, 23, 42),
            Font = new Font("Segoe UI", 9.5f),
            Cursor = Cursors.Hand
        };
        btnCancel.FlatAppearance.BorderSize = 1;
        btnCancel.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);

        SuspendLayout();

        // grpInfo
        grpInfo.Text = "Thông tin giáo viên / trợ giảng";
        grpInfo.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        grpInfo.ForeColor = Color.FromArgb(15, 23, 42);
        grpInfo.Location = new Point(20, 16);
        grpInfo.Size = new Size(500, 430);

        grpInfo.Controls.AddRange(new Control[]
        {
            lblHoTen, txtHoTen, chkNgaySinh, dtpNgaySinh, lblGioiTinh, cboGioiTinh,
            lblEmail, txtEmail, lblSoDienThoai, txtSoDienThoai, lblDiaChi, txtDiaChi,
            lblLoaiGiaoVien, cboLoaiGiaoVien, lblSoNamKinhNghiem, numSoNamKinhNghiem,
            lblChuyenMon, txtChuyenMon, chkNgayVaoLam, dtpNgayVaoLam
        });

        // grpStatus
        grpStatus.Text = "Trạng thái làm việc (Quản trị viên)";
        grpStatus.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        grpStatus.ForeColor = Color.FromArgb(15, 23, 42);
        grpStatus.Location = new Point(20, 462);
        grpStatus.Size = new Size(500, 90);
        grpStatus.Controls.AddRange(new Control[] { lblTrangThai, cboTrangThai });

        // grpQualifications
        grpQualifications.Text = "Bằng cấp / Chứng chỉ (chỉ đọc)";
        grpQualifications.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        grpQualifications.ForeColor = Color.FromArgb(15, 23, 42);
        grpQualifications.Location = new Point(20, 568);
        grpQualifications.Size = new Size(500, 160);
        grpQualifications.Controls.Add(lblQualificationsReadonly);

        // message + buttons
        lblMessage.Location = new Point(20, 744);
        lblMessage.Size = new Size(500, 40);
        lblMessage.Font = new Font("Segoe UI", 9.5f);
        lblMessage.ForeColor = Color.FromArgb(239, 68, 68);

        btnSave.Location = new Point(20, 792);
        btnSave.Click += BtnSave_Click;

        btnCancel.Location = new Point(170, 792);

        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(lblMessage);
        Controls.Add(grpQualifications);
        Controls.Add(grpStatus);
        Controls.Add(grpInfo);

        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(560, 860);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        AutoScroll = true;
        Text = "Giáo viên / Trợ giảng";

        ResumeLayout(false);
    }
}
