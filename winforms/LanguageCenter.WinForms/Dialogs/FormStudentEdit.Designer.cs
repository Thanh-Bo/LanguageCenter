#nullable enable

namespace LanguageCenter.WinForms.Dialogs;

partial class FormStudentEdit
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

    private GroupBox grpParent = null!;
    private CheckBox chkHasParent = null!;
    private Panel pnlParentFields = null!;
    private Label lblParentHoTen = null!;
    private TextBox txtParentHoTen = null!;
    private Label lblParentMoiQuanHe = null!;
    private ComboBox cboMoiQuanHe = null!;
    private Label lblParentSdt = null!;
    private TextBox txtParentSoDienThoai = null!;
    private Label lblParentEmail = null!;
    private TextBox txtParentEmail = null!;
    private Label lblParentDiaChi = null!;
    private TextBox txtParentDiaChi = null!;
    private Label lblParentReadonly = null!;

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
        lblNgaySinh = Caption("Ngày sinh (*)");
        dtpNgaySinh = new DateTimePicker { Width = 220, Format = DateTimePickerFormat.Short };
        lblGioiTinh = Caption("Giới tính");
        cboGioiTinh = new ComboBox { Width = 210, DropDownStyle = ComboBoxStyle.DropDown, Font = UI.Theme.FontRegular };
        lblEmail = Caption("Email (*)");
        txtEmail = UI.UiHelpers.CreateTextBox(220);
        lblSoDienThoai = Caption("Số điện thoại");
        txtSoDienThoai = UI.UiHelpers.CreateTextBox(210);
        lblDiaChi = Caption("Địa chỉ");
        txtDiaChi = UI.UiHelpers.CreateTextBox(460);
        lblTrinhDo = Caption("Trình độ hiện tại");
        txtTrinhDoHienTai = UI.UiHelpers.CreateTextBox(460);
        lblMucTieu = Caption("Mục tiêu học tập");
        txtMucTieuHocTap = UI.UiHelpers.CreateTextBox(460);

        grpStatus = new GroupBox();
        lblTrangThai = Caption("Trạng thái học tập");
        cboTrangThaiHocTap = UI.UiHelpers.CreateComboBox(220);
        chkNgayNhapHoc = new CheckBox { Text = "Đã nhập học", AutoSize = true, Font = UI.Theme.FontRegular };
        dtpNgayNhapHoc = new DateTimePicker { Width = 210, Format = DateTimePickerFormat.Short, Enabled = false };

        grpParent = new GroupBox();
        chkHasParent = new CheckBox { Text = "Có thông tin phụ huynh", AutoSize = true, Font = UI.Theme.FontRegular };
        pnlParentFields = new Panel();
        lblParentHoTen = Caption("Họ tên phụ huynh (*)");
        txtParentHoTen = UI.UiHelpers.CreateTextBox(220);
        lblParentMoiQuanHe = Caption("Mối quan hệ (*)");
        cboMoiQuanHe = new ComboBox { Width = 200, DropDownStyle = ComboBoxStyle.DropDown, Font = UI.Theme.FontRegular };
        lblParentSdt = Caption("Số điện thoại");
        txtParentSoDienThoai = UI.UiHelpers.CreateTextBox(220);
        lblParentEmail = Caption("Email");
        txtParentEmail = UI.UiHelpers.CreateTextBox(200);
        lblParentDiaChi = Caption("Địa chỉ");
        txtParentDiaChi = UI.UiHelpers.CreateTextBox(460);
        lblParentReadonly = new Label
        {
            AutoSize = false,
            Font = UI.Theme.FontRegular,
            ForeColor = UI.Theme.TextDark,
            Location = new Point(16, 26),
            Size = new Size(460, 150)
        };

        lblMessage = new Label();
        btnSave = UI.UiHelpers.CreatePrimaryButton("Lưu", 140, 36);
        btnCancel = UI.UiHelpers.CreateSecondaryButton("Hủy", 110, 36);

        SuspendLayout();

        // grpInfo
        grpInfo.Text = "Thông tin học viên";
        grpInfo.Font = UI.Theme.FontBold;
        grpInfo.ForeColor = UI.Theme.TextDark;
        grpInfo.Location = new Point(20, 16);
        grpInfo.Size = new Size(500, 370);

        Place(lblHoTen, 16, 28);
        Place(txtHoTen, 16, 48);
        Place(lblNgaySinh, 16, 84);
        Place(dtpNgaySinh, 16, 104);
        Place(lblGioiTinh, 250, 84);
        Place(cboGioiTinh, 250, 104);
        Place(lblEmail, 16, 140);
        Place(txtEmail, 16, 160);
        Place(lblSoDienThoai, 250, 140);
        Place(txtSoDienThoai, 250, 160);
        Place(lblDiaChi, 16, 196);
        Place(txtDiaChi, 16, 216);
        Place(lblTrinhDo, 16, 252);
        Place(txtTrinhDoHienTai, 16, 272);
        Place(lblMucTieu, 16, 308);
        Place(txtMucTieuHocTap, 16, 328);

        grpInfo.Controls.AddRange(new Control[]
        {
            lblHoTen, txtHoTen, lblNgaySinh, dtpNgaySinh, lblGioiTinh, cboGioiTinh,
            lblEmail, txtEmail, lblSoDienThoai, txtSoDienThoai, lblDiaChi, txtDiaChi,
            lblTrinhDo, txtTrinhDoHienTai, lblMucTieu, txtMucTieuHocTap
        });

        // grpStatus
        grpStatus.Text = "Trạng thái học tập (Quản trị viên)";
        grpStatus.Font = UI.Theme.FontBold;
        grpStatus.ForeColor = UI.Theme.TextDark;
        grpStatus.Location = new Point(20, 396);
        grpStatus.Size = new Size(500, 90);

        Place(lblTrangThai, 16, 26);
        Place(cboTrangThaiHocTap, 16, 46);
        Place(chkNgayNhapHoc, 250, 28);
        Place(dtpNgayNhapHoc, 250, 46);
        chkNgayNhapHoc.CheckedChanged += (_, _) => dtpNgayNhapHoc.Enabled = chkNgayNhapHoc.Checked;

        grpStatus.Controls.AddRange(new Control[] { lblTrangThai, cboTrangThaiHocTap, chkNgayNhapHoc, dtpNgayNhapHoc });

        // grpParent
        grpParent.Text = "Phụ huynh";
        grpParent.Font = UI.Theme.FontBold;
        grpParent.ForeColor = UI.Theme.TextDark;
        grpParent.Location = new Point(20, 496);
        grpParent.Size = new Size(500, 210);

        Place(chkHasParent, 16, 26);
        chkHasParent.CheckedChanged += (_, _) => pnlParentFields.Enabled = chkHasParent.Checked;

        pnlParentFields.Location = new Point(16, 54);
        pnlParentFields.Size = new Size(468, 140);
        pnlParentFields.Enabled = false;

        Place(lblParentHoTen, 0, 0, pnlParentFields);
        Place(txtParentHoTen, 0, 20, pnlParentFields);
        Place(lblParentMoiQuanHe, 240, 0, pnlParentFields);
        Place(cboMoiQuanHe, 240, 20, pnlParentFields);
        Place(lblParentSdt, 0, 54, pnlParentFields);
        Place(txtParentSoDienThoai, 0, 74, pnlParentFields);
        Place(lblParentEmail, 240, 54, pnlParentFields);
        Place(txtParentEmail, 240, 74, pnlParentFields);
        Place(lblParentDiaChi, 0, 108, pnlParentFields);
        Place(txtParentDiaChi, 0, 128, pnlParentFields);
        txtParentDiaChi.Width = 460;

        pnlParentFields.Controls.AddRange(new Control[]
        {
            lblParentHoTen, txtParentHoTen, lblParentMoiQuanHe, cboMoiQuanHe,
            lblParentSdt, txtParentSoDienThoai, lblParentEmail, txtParentEmail,
            lblParentDiaChi, txtParentDiaChi
        });

        grpParent.Controls.Add(lblParentReadonly);
        grpParent.Controls.Add(pnlParentFields);
        grpParent.Controls.Add(chkHasParent);

        // message + buttons
        lblMessage.Location = new Point(20, 716);
        lblMessage.Size = new Size(500, 40);
        lblMessage.Font = UI.Theme.FontRegular;
        lblMessage.ForeColor = UI.Theme.Danger;

        btnSave.Location = new Point(20, 760);
        btnSave.Click += BtnSave_Click;

        btnCancel.Location = new Point(170, 760);
        btnCancel.Click += (_, _) => Close();

        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(lblMessage);
        Controls.Add(grpParent);
        Controls.Add(grpStatus);
        Controls.Add(grpInfo);

        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(560, 820);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        AutoScroll = true;
        Text = "Học viên";

        ResumeLayout(false);
    }

    private static Label Caption(string text) => new()
    {
        Text = text,
        AutoSize = true,
        Font = UI.Theme.FontRegular,
        ForeColor = UI.Theme.Muted
    };

    private static void Place(Control control, int x, int y, Control? parent = null)
    {
        control.Location = new Point(x, y);
    }
}
