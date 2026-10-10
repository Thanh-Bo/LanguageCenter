namespace LanguageCenter.WinForms.Dialogs;

partial class FormTeacherEdit
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
        lblHoTen = Caption("Họ tên (*)");
        txtHoTen = UI.UiHelpers.CreateTextBox(460);
        chkNgaySinh = new CheckBox { Text = "Có ngày sinh", AutoSize = true, Font = UI.Theme.FontRegular };
        dtpNgaySinh = new DateTimePicker { Width = 220, Format = DateTimePickerFormat.Short, Enabled = false };
        lblGioiTinh = Caption("Giới tính");
        cboGioiTinh = new ComboBox { Width = 210, DropDownStyle = ComboBoxStyle.DropDown, Font = UI.Theme.FontRegular };
        lblEmail = Caption("Email (*)");
        txtEmail = UI.UiHelpers.CreateTextBox(220);
        lblSoDienThoai = Caption("Số điện thoại");
        txtSoDienThoai = UI.UiHelpers.CreateTextBox(210);
        lblDiaChi = Caption("Địa chỉ");
        txtDiaChi = UI.UiHelpers.CreateTextBox(460);
        lblLoaiGiaoVien = Caption("Loại giáo viên (*)");
        cboLoaiGiaoVien = UI.UiHelpers.CreateComboBox(210);
        lblSoNamKinhNghiem = Caption("Số năm kinh nghiệm");
        numSoNamKinhNghiem = new NumericUpDown { Width = 100, Minimum = 0, Maximum = 80, Font = UI.Theme.FontRegular };
        lblChuyenMon = Caption("Chuyên môn");
        txtChuyenMon = UI.UiHelpers.CreateTextBox(460);
        chkNgayVaoLam = new CheckBox { Text = "Có ngày vào làm", AutoSize = true, Font = UI.Theme.FontRegular };
        dtpNgayVaoLam = new DateTimePicker { Width = 220, Format = DateTimePickerFormat.Short, Enabled = false };

        grpStatus = new GroupBox();
        lblTrangThai = Caption("Trạng thái làm việc");
        cboTrangThai = UI.UiHelpers.CreateComboBox(220);

        grpQualifications = new GroupBox();
        lblQualificationsReadonly = new Label
        {
            AutoSize = false,
            Font = UI.Theme.FontRegular,
            ForeColor = UI.Theme.TextDark,
            Location = new Point(16, 26),
            Size = new Size(468, 124)
        };

        lblMessage = new Label();
        btnSave = UI.UiHelpers.CreatePrimaryButton("Lưu", 140, 36);
        btnCancel = UI.UiHelpers.CreateSecondaryButton("Hủy", 110, 36);

        SuspendLayout();

        // grpInfo
        grpInfo.Text = "Thông tin giáo viên / trợ giảng";
        grpInfo.Font = UI.Theme.FontBold;
        grpInfo.ForeColor = UI.Theme.TextDark;
        grpInfo.Location = new Point(20, 16);
        grpInfo.Size = new Size(500, 430);

        Place(lblHoTen, 16, 28);
        Place(txtHoTen, 16, 48);
        Place(chkNgaySinh, 16, 84);
        Place(dtpNgaySinh, 16, 104);
        chkNgaySinh.CheckedChanged += (_, _) => dtpNgaySinh.Enabled = chkNgaySinh.Checked;
        Place(lblGioiTinh, 250, 84);
        Place(cboGioiTinh, 250, 104);
        Place(lblEmail, 16, 140);
        Place(txtEmail, 16, 160);
        Place(lblSoDienThoai, 250, 140);
        Place(txtSoDienThoai, 250, 160);
        Place(lblDiaChi, 16, 196);
        Place(txtDiaChi, 16, 216);
        Place(lblLoaiGiaoVien, 16, 252);
        Place(cboLoaiGiaoVien, 16, 272);
        Place(lblSoNamKinhNghiem, 250, 252);
        Place(numSoNamKinhNghiem, 250, 272);
        Place(lblChuyenMon, 16, 308);
        Place(txtChuyenMon, 16, 328);
        Place(chkNgayVaoLam, 16, 364);
        Place(dtpNgayVaoLam, 16, 384);
        chkNgayVaoLam.CheckedChanged += (_, _) => dtpNgayVaoLam.Enabled = chkNgayVaoLam.Checked;

        grpInfo.Controls.AddRange(new Control[]
        {
            lblHoTen, txtHoTen, chkNgaySinh, dtpNgaySinh, lblGioiTinh, cboGioiTinh,
            lblEmail, txtEmail, lblSoDienThoai, txtSoDienThoai, lblDiaChi, txtDiaChi,
            lblLoaiGiaoVien, cboLoaiGiaoVien, lblSoNamKinhNghiem, numSoNamKinhNghiem,
            lblChuyenMon, txtChuyenMon, chkNgayVaoLam, dtpNgayVaoLam
        });

        // grpStatus (chỉ hiện ở chế độ sửa)
        grpStatus.Text = "Trạng thái làm việc (Quản trị viên)";
        grpStatus.Font = UI.Theme.FontBold;
        grpStatus.ForeColor = UI.Theme.TextDark;
        grpStatus.Location = new Point(20, grpInfo.Bottom + 16);
        grpStatus.Size = new Size(500, 90);

        Place(lblTrangThai, 16, 26);
        Place(cboTrangThai, 16, 46);

        grpStatus.Controls.AddRange(new Control[] { lblTrangThai, cboTrangThai });

        // grpQualifications (chỉ hiện ở chế độ sửa — Phase 3 chưa có CRUD Bằng cấp/Chứng chỉ)
        grpQualifications.Text = "Bằng cấp / Chứng chỉ (chỉ đọc)";
        grpQualifications.Font = UI.Theme.FontBold;
        grpQualifications.ForeColor = UI.Theme.TextDark;
        grpQualifications.Location = new Point(20, grpStatus.Bottom + 16);
        grpQualifications.Size = new Size(500, 160);

        grpQualifications.Controls.Add(lblQualificationsReadonly);

        // message + buttons
        lblMessage.Location = new Point(20, grpQualifications.Bottom + 16);
        lblMessage.Size = new Size(500, 40);
        lblMessage.Font = UI.Theme.FontRegular;
        lblMessage.ForeColor = UI.Theme.Danger;

        btnSave.Location = new Point(20, lblMessage.Bottom + 8);
        btnSave.Click += BtnSave_Click;

        btnCancel.Location = new Point(170, lblMessage.Bottom + 8);
        btnCancel.Click += (_, _) => Close();

        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(lblMessage);
        Controls.Add(grpQualifications);
        Controls.Add(grpStatus);
        Controls.Add(grpInfo);

        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(560, btnSave.Bottom + 60);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        AutoScroll = true;
        Text = "Giáo viên / Trợ giảng";

        ResumeLayout(false);
    }

    private static Label Caption(string text) => new()
    {
        Text = text,
        AutoSize = true,
        Font = UI.Theme.FontRegular,
        ForeColor = UI.Theme.Muted
    };

    private static void Place(Control control, int x, int y)
    {
        control.Location = new Point(x, y);
    }
}
