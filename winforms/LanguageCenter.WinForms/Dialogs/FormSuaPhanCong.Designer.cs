namespace LanguageCenter.WinForms.Dialogs;

partial class FormSuaPhanCong
{
    private System.ComponentModel.IContainer components = null;

    private Label lblTitle = null!;
    private Label lblLop = null!;
    private ComboBox cboLopHoc = null!;
    private Label lblNS = null!;
    private ComboBox cboNhanSu = null!;
    private Label lblVT = null!;
    private ComboBox cboVaiTro = null!;
    private Label lblTG = null!;
    private ComboBox cboTroGiang = null!;
    private Label lblBD = null!;
    private DateTimePicker dtpBatDau = null!;
    private Label lblKT = null!;
    private DateTimePicker dtpKetThuc = null!;
    private Label lblGC = null!;
    private TextBox txtGhiChu = null!;
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
        lblLop = new Label();
        cboLopHoc = new ComboBox();
        lblNS = new Label();
        cboNhanSu = new ComboBox();
        lblVT = new Label();
        cboVaiTro = new ComboBox();
        lblTG = new Label();
        cboTroGiang = new ComboBox();
        lblBD = new Label();
        dtpBatDau = new DateTimePicker();
        lblKT = new Label();
        dtpKetThuc = new DateTimePicker();
        lblGC = new Label();
        txtGhiChu = new TextBox();
        btnSave = new Button();
        btnCancel = new Button();

        SuspendLayout();

        // FormSuaPhanCong
        Text = "Phân công giảng dạy — HUIT LingoHub";
        Size = new Size(740, 460);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.White;

        // lblTitle
        lblTitle.Text = "PHÂN CÔNG GIẢNG VIÊN / TRỢ GIẢNG";
        lblTitle.Font = new Font("Segoe UI", 15f, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(23, 67, 197);
        lblTitle.AutoSize = true;
        lblTitle.Location = new Point(24, 20);

        // lblLop & cboLopHoc
        lblLop.Text = "Lớp học phụ trách:";
        lblLop.Font = new Font("Segoe UI", 9.5f);
        lblLop.Location = new Point(24, 74);
        lblLop.AutoSize = true;
        cboLopHoc.Location = new Point(230, 70);
        cboLopHoc.Width = 450;
        cboLopHoc.Font = new Font("Segoe UI", 9.5f);
        cboLopHoc.DropDownStyle = ComboBoxStyle.DropDownList;

        // lblNS & cboNhanSu — GIÁO VIÊN CHÍNH (được phép để trống)
        lblNS.Text = "Giáo viên chính:";
        lblNS.Font = new Font("Segoe UI", 9.5f);
        lblNS.Location = new Point(24, 118);
        lblNS.AutoSize = true;
        cboNhanSu.Location = new Point(230, 114);
        cboNhanSu.Width = 450;
        cboNhanSu.Font = new Font("Segoe UI", 9.5f);
        cboNhanSu.DropDownStyle = ComboBoxStyle.DropDownList;

        // lblTG & cboTroGiang — TRỢ GIẢNG (được phép để trống)
        lblTG.Text = "Trợ giảng:";
        lblTG.Font = new Font("Segoe UI", 9.5f);
        lblTG.Location = new Point(24, 158);
        lblTG.AutoSize = true;
        cboTroGiang.Location = new Point(230, 154);
        cboTroGiang.Width = 450;
        cboTroGiang.Font = new Font("Segoe UI", 9.5f);
        cboTroGiang.DropDownStyle = ComboBoxStyle.DropDownList;

        // lblVT & cboVaiTro — KHÔNG dùng nữa (vai trò đã tách thành 2 ô chọn người)
        lblVT.Text = "Vai trò phân công:";
        lblVT.Font = new Font("Segoe UI", 9.5f);
        lblVT.Location = new Point(24, 198);
        lblVT.AutoSize = true;
        lblVT.Visible = false;
        cboVaiTro.Location = new Point(180, 194);
        cboVaiTro.Width = 330;
        cboVaiTro.Font = new Font("Segoe UI", 9.5f);
        cboVaiTro.DropDownStyle = ComboBoxStyle.DropDownList;
        cboVaiTro.Visible = false;

        // lblBD & dtpBatDau (ẩn — thời gian lấy theo lớp)
        lblBD.Text = "Thời gian:";
        lblBD.Font = new Font("Segoe UI", 9.5f);
        lblBD.Location = new Point(24, 206);
        lblBD.AutoSize = true;
        dtpBatDau.Location = new Point(230, 202);
        dtpBatDau.Width = 150;
        dtpBatDau.Format = DateTimePickerFormat.Short;
        dtpBatDau.Font = new Font("Segoe UI", 9.5f);

        // lblKT & dtpKetThuc (tích vào ô vuông = có ngày kết thúc)
        lblKT.Text = "Ngày kết thúc:";
        lblKT.Font = new Font("Segoe UI", 9.5f);
        lblKT.Location = new Point(24, 244);
        lblKT.AutoSize = true;
        dtpKetThuc.Location = new Point(230, 240);
        dtpKetThuc.Width = 150;
        dtpKetThuc.Format = DateTimePickerFormat.Short;
        dtpKetThuc.Font = new Font("Segoe UI", 9.5f);
        dtpKetThuc.ShowCheckBox = true;
        dtpKetThuc.Checked = false;

        // lblGC & txtGhiChu
        lblGC.Text = "Ghi chú:";
        lblGC.Font = new Font("Segoe UI", 9.5f);
        lblGC.Location = new Point(24, 262);
        lblGC.AutoSize = true;
        txtGhiChu.Location = new Point(230, 258);
        txtGhiChu.Width = 450;
        txtGhiChu.Font = new Font("Segoe UI", 9.5f);

        // btnSave
        btnSave.Text = "Lưu phân công";
        btnSave.Location = new Point(230, 312);
        btnSave.Size = new Size(150, 38);
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.BackColor = Color.FromArgb(30, 86, 240);
        btnSave.ForeColor = Color.White;
        btnSave.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnSave.Cursor = Cursors.Hand;
        btnSave.FlatAppearance.BorderSize = 0;

        // btnCancel
        btnCancel.Text = "Hủy bỏ";
        btnCancel.Location = new Point(396, 312);
        btnCancel.Size = new Size(120, 38);
        btnCancel.FlatStyle = FlatStyle.Flat;
        btnCancel.BackColor = Color.White;
        btnCancel.ForeColor = Color.FromArgb(15, 23, 42);
        btnCancel.Font = new Font("Segoe UI", 9.5f);
        btnCancel.Cursor = Cursors.Hand;
        btnCancel.FlatAppearance.BorderSize = 1;
        btnCancel.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);

        Controls.Add(btnCancel);
        Controls.Add(btnSave);
        Controls.Add(txtGhiChu);
        Controls.Add(lblGC);
        Controls.Add(dtpKetThuc);
        Controls.Add(lblKT);
        Controls.Add(dtpBatDau);
        Controls.Add(lblBD);
        Controls.Add(cboVaiTro);
        Controls.Add(lblVT);
        Controls.Add(cboTroGiang);
        Controls.Add(lblTG);
        Controls.Add(cboNhanSu);
        Controls.Add(lblNS);
        Controls.Add(cboLopHoc);
        Controls.Add(lblLop);
        Controls.Add(lblTitle);

        ResumeLayout(false);
    }
}

