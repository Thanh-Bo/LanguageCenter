namespace LanguageCenter.WinForms.Dialogs;

partial class FormSuaLopHoc
{
    private System.ComponentModel.IContainer components = null;

    private Label lblTitle = null!;
    private Label lblMaLop = null!;
    private TextBox txtMaLop = null!;
    private Label lblTenLop = null!;
    private TextBox txtTenLop = null!;
    private Label lblCourse = null!;
    private ComboBox cboKhoaHoc = null!;
    private Label lblGV = null!;
    private ComboBox cboGiaoVien = null!;
    private Label lblTG = null!;
    private ComboBox cboTroGiang = null!;
    private Label lblPhong = null!;
    private ComboBox cboPhongHoc = null!;
    private Label lblLichHoc = null!;
    private TextBox txtLichHoc = null!;
    private Label lblSiSo = null!;
    private NumericUpDown numSiSo = null!;
    private Label lblBD = null!;
    private DateTimePicker dtpBatDau = null!;
    private Label lblKT = null!;
    private DateTimePicker dtpKetThuc = null!;
    private Label lblTT = null!;
    private ComboBox cboTrangThai = null!;
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

        lblTitle = new Label();
        lblMaLop = new Label();
        txtMaLop = new TextBox();
        lblTenLop = new Label();
        txtTenLop = new TextBox();
        lblCourse = new Label();
        cboKhoaHoc = new ComboBox();
        lblGV = new Label();
        cboGiaoVien = new ComboBox();
        lblTG = new Label();
        cboTroGiang = new ComboBox();
        lblPhong = new Label();
        cboPhongHoc = new ComboBox();
        lblLichHoc = new Label();
        txtLichHoc = new TextBox();
        lblSiSo = new Label();
        numSiSo = new NumericUpDown();
        lblBD = new Label();
        dtpBatDau = new DateTimePicker();
        lblKT = new Label();
        dtpKetThuc = new DateTimePicker();
        lblTT = new Label();
        cboTrangThai = new ComboBox();
        btnSave = new Button();
        btnCancel = new Button();

        SuspendLayout();

        // FormSuaLopHoc
        Text = "Quản lý lớp học — HUIT LingoHub";
        Size = new Size(540, 600);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.White;

        // lblTitle
        lblTitle.Text = "THÔNG TIN LỚP HỌC";
        lblTitle.Font = new Font("Segoe UI", 15f, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(23, 67, 197);
        lblTitle.AutoSize = true;
        lblTitle.Location = new Point(24, 20);

        // lblMaLop & txtMaLop
        lblMaLop.Text = "Mã lớp (*):";
        lblMaLop.Font = new Font("Segoe UI", 9.5f);
        lblMaLop.Location = new Point(24, 69);
        lblMaLop.AutoSize = true;
        txtMaLop.Location = new Point(160, 65);
        txtMaLop.Width = 320;
        txtMaLop.Font = new Font("Segoe UI", 9.5f);

        // lblTenLop & txtTenLop
        lblTenLop.Text = "Tên lớp học (*):";
        lblTenLop.Font = new Font("Segoe UI", 9.5f);
        lblTenLop.Location = new Point(24, 111);
        lblTenLop.AutoSize = true;
        txtTenLop.Location = new Point(160, 107);
        txtTenLop.Width = 320;
        txtTenLop.Font = new Font("Segoe UI", 9.5f);

        // lblCourse & cboKhoaHoc
        lblCourse.Text = "Khóa học:";
        lblCourse.Font = new Font("Segoe UI", 9.5f);
        lblCourse.Location = new Point(24, 153);
        lblCourse.AutoSize = true;
        cboKhoaHoc.Location = new Point(160, 149);
        cboKhoaHoc.Width = 320;
        cboKhoaHoc.Font = new Font("Segoe UI", 9.5f);
        cboKhoaHoc.DropDownStyle = ComboBoxStyle.DropDownList;

        // lblGV & cboGiaoVien
        lblGV.Text = "Giảng viên chính:";
        lblGV.Font = new Font("Segoe UI", 9.5f);
        lblGV.Location = new Point(24, 195);
        lblGV.AutoSize = true;
        cboGiaoVien.Location = new Point(160, 191);
        cboGiaoVien.Width = 320;
        cboGiaoVien.Font = new Font("Segoe UI", 9.5f);
        cboGiaoVien.DropDownStyle = ComboBoxStyle.DropDownList;

        // lblTG & cboTroGiang
        lblTG.Text = "Trợ giảng học vụ:";
        lblTG.Font = new Font("Segoe UI", 9.5f);
        lblTG.Location = new Point(24, 237);
        lblTG.AutoSize = true;
        cboTroGiang.Location = new Point(160, 233);
        cboTroGiang.Width = 320;
        cboTroGiang.Font = new Font("Segoe UI", 9.5f);
        cboTroGiang.DropDownStyle = ComboBoxStyle.DropDownList;

        // lblPhong & cboPhongHoc
        lblPhong.Text = "Phòng học:";
        lblPhong.Font = new Font("Segoe UI", 9.5f);
        lblPhong.Location = new Point(24, 279);
        lblPhong.AutoSize = true;
        cboPhongHoc.Location = new Point(160, 275);
        cboPhongHoc.Width = 320;
        cboPhongHoc.Font = new Font("Segoe UI", 9.5f);
        cboPhongHoc.DropDownStyle = ComboBoxStyle.DropDownList;

        // lblLichHoc & txtLichHoc
        lblLichHoc.Text = "Lịch học tuần:";
        lblLichHoc.Font = new Font("Segoe UI", 9.5f);
        lblLichHoc.Location = new Point(24, 321);
        lblLichHoc.AutoSize = true;
        txtLichHoc.Location = new Point(160, 317);
        txtLichHoc.Width = 320;
        txtLichHoc.Font = new Font("Segoe UI", 9.5f);

        // lblSiSo & numSiSo
        lblSiSo.Text = "Sĩ số tối đa:";
        lblSiSo.Font = new Font("Segoe UI", 9.5f);
        lblSiSo.Location = new Point(24, 363);
        lblSiSo.AutoSize = true;
        numSiSo.Location = new Point(160, 359);
        numSiSo.Width = 120;
        numSiSo.Minimum = 5;
        numSiSo.Maximum = 60;
        numSiSo.Value = 20;
        numSiSo.Font = new Font("Segoe UI", 9.5f);

        // lblBD & dtpBatDau
        lblBD.Text = "Ngày bắt đầu:";
        lblBD.Font = new Font("Segoe UI", 9.5f);
        lblBD.Location = new Point(24, 405);
        lblBD.AutoSize = true;
        dtpBatDau.Location = new Point(160, 401);
        dtpBatDau.Width = 150;
        dtpBatDau.Format = DateTimePickerFormat.Short;
        dtpBatDau.Font = new Font("Segoe UI", 9.5f);

        // lblKT & dtpKetThuc
        lblKT.Text = "Ngày kết thúc:";
        lblKT.Font = new Font("Segoe UI", 9.5f);
        lblKT.Location = new Point(24, 447);
        lblKT.AutoSize = true;
        dtpKetThuc.Location = new Point(160, 443);
        dtpKetThuc.Width = 150;
        dtpKetThuc.Format = DateTimePickerFormat.Short;
        dtpKetThuc.Font = new Font("Segoe UI", 9.5f);

        // lblTT & cboTrangThai
        lblTT.Text = "Trạng thái:";
        lblTT.Font = new Font("Segoe UI", 9.5f);
        lblTT.Location = new Point(24, 489);
        lblTT.AutoSize = true;
        cboTrangThai.Location = new Point(160, 485);
        cboTrangThai.Width = 320;
        cboTrangThai.Font = new Font("Segoe UI", 9.5f);
        cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;

        // btnSave
        btnSave.Text = "Lưu lớp học";
        btnSave.Location = new Point(160, 527);
        btnSave.Size = new Size(140, 36);
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.BackColor = Color.FromArgb(30, 86, 240);
        btnSave.ForeColor = Color.White;
        btnSave.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnSave.Cursor = Cursors.Hand;
        btnSave.FlatAppearance.BorderSize = 0;

        // btnCancel
        btnCancel.Text = "Hủy bỏ";
        btnCancel.Location = new Point(310, 527);
        btnCancel.Size = new Size(100, 36);
        btnCancel.FlatStyle = FlatStyle.Flat;
        btnCancel.BackColor = Color.White;
        btnCancel.ForeColor = Color.FromArgb(15, 23, 42);
        btnCancel.Font = new Font("Segoe UI", 9.5f);
        btnCancel.Cursor = Cursors.Hand;
        btnCancel.FlatAppearance.BorderSize = 1;
        btnCancel.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);

        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(cboTrangThai);
        Controls.Add(lblTT);
        Controls.Add(dtpKetThuc);
        Controls.Add(lblKT);
        Controls.Add(dtpBatDau);
        Controls.Add(lblBD);
        Controls.Add(numSiSo);
        Controls.Add(lblSiSo);
        Controls.Add(txtLichHoc);
        Controls.Add(lblLichHoc);
        Controls.Add(cboPhongHoc);
        Controls.Add(lblPhong);
        Controls.Add(cboTroGiang);
        Controls.Add(lblTG);
        Controls.Add(cboGiaoVien);
        Controls.Add(lblGV);
        Controls.Add(cboKhoaHoc);
        Controls.Add(lblCourse);
        Controls.Add(txtTenLop);
        Controls.Add(lblTenLop);
        Controls.Add(txtMaLop);
        Controls.Add(lblMaLop);
        Controls.Add(lblTitle);

        ResumeLayout(false);
    }
}

