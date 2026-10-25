#nullable enable

namespace LanguageCenter.WinForms.Dialogs;

partial class FormSuaHocVien
{
    private System.ComponentModel.IContainer components = null!;

    private GroupBox grpInfo = null!;
    private Label lblHoTen = null!;
    private TextBox txtHoTen = null!;
    private Label lblNgaySinh = null!;
    private DateTimePicker dtpNgaySinh = null!;
    private Label lblGioiTinh = null!;
    private ComboBox cboGioiTinh = null!;
    private Label lblEmail = null!;
    private TextBox txtEmail = null!;
    private Label lblSoDienThoai = null!;
    private TextBox txtSoDienThoai = null!;
    private Label lblDiaChi = null!;
    private TextBox txtDiaChi = null!;
    private Label lblTrinhDo = null!;
    private TextBox txtTrinhDoHienTai = null!;
    private Label lblMucTieu = null!;
    private TextBox txtMucTieuHocTap = null!;

    private GroupBox grpStatus = null!;
    private Label lblTrangThai = null!;
    private ComboBox cboTrangThaiHocTap = null!;
    private CheckBox chkNgayNhapHoc = null!;
    private DateTimePicker dtpNgayNhapHoc = null!;

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
        lblNgaySinh = new Label { Text = "Ngày sinh (*)", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(16, 84) };
        dtpNgaySinh = new DateTimePicker { Width = 220, Format = DateTimePickerFormat.Short, Location = new Point(16, 104) };
        lblGioiTinh = new Label { Text = "Giới tính", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(250, 84) };
        cboGioiTinh = new ComboBox { Width = 210, DropDownStyle = ComboBoxStyle.DropDown, Font = new Font("Segoe UI", 9.5f), Location = new Point(250, 104) };
        lblEmail = new Label { Text = "Email (*)", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(16, 140) };
        txtEmail = new TextBox { Width = 220, Font = new Font("Segoe UI", 9.5f), Location = new Point(16, 160) };
        lblSoDienThoai = new Label { Text = "Số điện thoại", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(250, 140) };
        txtSoDienThoai = new TextBox { Width = 210, Font = new Font("Segoe UI", 9.5f), Location = new Point(250, 160) };
        lblDiaChi = new Label { Text = "Địa chỉ", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(16, 196) };
        txtDiaChi = new TextBox { Width = 460, Font = new Font("Segoe UI", 9.5f), Location = new Point(16, 216) };
        lblTrinhDo = new Label { Text = "Trình độ hiện tại", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(16, 252) };
        txtTrinhDoHienTai = new TextBox { Width = 460, Font = new Font("Segoe UI", 9.5f), Location = new Point(16, 272) };
        lblMucTieu = new Label { Text = "Mục tiêu học tập", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(16, 308) };
        txtMucTieuHocTap = new TextBox { Width = 460, Font = new Font("Segoe UI", 9.5f), Location = new Point(16, 328) };

        grpStatus = new GroupBox();
        lblTrangThai = new Label { Text = "Trạng thái học tập", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(16, 26) };
        cboTrangThaiHocTap = new ComboBox { Width = 220, Font = new Font("Segoe UI", 9.5f), DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(16, 46) };
        chkNgayNhapHoc = new CheckBox { Text = "Đã nhập học", AutoSize = true, Font = new Font("Segoe UI", 9.5f), Location = new Point(250, 28) };
        dtpNgayNhapHoc = new DateTimePicker { Width = 210, Format = DateTimePickerFormat.Short, Enabled = false, Location = new Point(250, 46) };

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
        grpInfo.Text = "Thông tin học viên";
        grpInfo.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        grpInfo.ForeColor = Color.FromArgb(15, 23, 42);
        grpInfo.Location = new Point(20, 16);
        grpInfo.Size = new Size(500, 370);

        grpInfo.Controls.AddRange(new Control[]
        {
            lblHoTen, txtHoTen, lblNgaySinh, dtpNgaySinh, lblGioiTinh, cboGioiTinh,
            lblEmail, txtEmail, lblSoDienThoai, txtSoDienThoai, lblDiaChi, txtDiaChi,
            lblTrinhDo, txtTrinhDoHienTai, lblMucTieu, txtMucTieuHocTap
        });

        // grpStatus
        grpStatus.Text = "Trạng thái học tập (Quản trị viên)";
        grpStatus.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        grpStatus.ForeColor = Color.FromArgb(15, 23, 42);
        grpStatus.Location = new Point(20, 396);
        grpStatus.Size = new Size(500, 90);
        grpStatus.Controls.AddRange(new Control[] { lblTrangThai, cboTrangThaiHocTap, chkNgayNhapHoc, dtpNgayNhapHoc });

        // message + buttons
        lblMessage.Location = new Point(20, 490);
        lblMessage.Size = new Size(500, 92);
        lblMessage.Font = new Font("Segoe UI", 9.5f);
        lblMessage.ForeColor = Color.FromArgb(239, 68, 68);

        btnSave.Location = new Point(20, 592);
        btnSave.Click += BtnSave_Click;

        btnCancel.Location = new Point(170, 592);

        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(lblMessage);
        Controls.Add(grpStatus);
        Controls.Add(grpInfo);

        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(560, 650);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        AutoScroll = true;
        Text = "Học viên";

        ResumeLayout(false);
    }
}
