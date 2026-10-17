namespace LanguageCenter.WinForms.Dialogs;

partial class FormSoanThongBao
{
    private System.ComponentModel.IContainer components = null;

    private Label lblTitle = null!;
    private Label lblTD = null!;
    private TextBox txtTieuDe = null!;
    private Label lblL = null!;
    private ComboBox cboLoai = null!;
    private Label lblDT = null!;
    private ComboBox cboDoiTuong = null!;
    private Label lblND = null!;
    private TextBox txtNoiDung = null!;
    private Button btnSend = null!;
    private Button btnDraft = null!;
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
        lblTD = new Label();
        txtTieuDe = new TextBox();
        lblL = new Label();
        cboLoai = new ComboBox();
        lblDT = new Label();
        cboDoiTuong = new ComboBox();
        lblND = new Label();
        txtNoiDung = new TextBox();
        btnSend = new Button();
        btnDraft = new Button();
        btnCancel = new Button();

        SuspendLayout();

        // FormSoanThongBao
        Text = "Soạn thảo thông báo mới — HUIT LingoHub";
        Size = new Size(560, 480);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        BackColor = Color.White;

        // lblTitle
        lblTitle.Text = "SOẠN THẢO THÔNG BÁO MỚI";
        lblTitle.Font = new Font("Segoe UI", 15f, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(23, 67, 197);
        lblTitle.AutoSize = true;
        lblTitle.Location = new Point(24, 20);

        // lblTD & txtTieuDe
        lblTD.Text = "Tiêu đề thông báo (*):";
        lblTD.Font = new Font("Segoe UI", 9.5f);
        lblTD.Location = new Point(24, 69);
        lblTD.AutoSize = true;
        txtTieuDe.Location = new Point(150, 65);
        txtTieuDe.Width = 360;
        txtTieuDe.Font = new Font("Segoe UI", 9.5f);

        // lblL & cboLoai
        lblL.Text = "Loại thông báo:";
        lblL.Font = new Font("Segoe UI", 9.5f);
        lblL.Location = new Point(24, 111);
        lblL.AutoSize = true;
        cboLoai.Location = new Point(150, 107);
        cboLoai.Width = 360;
        cboLoai.Font = new Font("Segoe UI", 9.5f);
        cboLoai.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLoai.Items.AddRange(new object[] { "Học vụ", "Lịch nghỉ / Khai giảng", "Học phí", "Sự kiện", "Khẩn cấp" });
        cboLoai.SelectedIndex = 0;

        // lblDT & cboDoiTuong
        lblDT.Text = "Đối tượng nhận:";
        lblDT.Font = new Font("Segoe UI", 9.5f);
        lblDT.Location = new Point(24, 153);
        lblDT.AutoSize = true;
        cboDoiTuong.Location = new Point(150, 149);
        cboDoiTuong.Width = 360;
        cboDoiTuong.Font = new Font("Segoe UI", 9.5f);
        cboDoiTuong.DropDownStyle = ComboBoxStyle.DropDownList;
        cboDoiTuong.Items.AddRange(new object[] { "Toàn trung tâm", "Tất cả giáo viên", "Tất cả trợ giảng", "Tất cả học viên" });
        cboDoiTuong.SelectedIndex = 0;

        // lblND & txtNoiDung
        lblND.Text = "Nội dung (*):";
        lblND.Font = new Font("Segoe UI", 9.5f);
        lblND.Location = new Point(24, 195);
        lblND.AutoSize = true;
        txtNoiDung.Location = new Point(150, 191);
        txtNoiDung.Width = 360;
        txtNoiDung.Height = 140;
        txtNoiDung.Multiline = true;
        txtNoiDung.Font = new Font("Segoe UI", 9.5f);
        txtNoiDung.ScrollBars = ScrollBars.Vertical;

        // btnSend
        btnSend.Text = "Phát hành ngay";
        btnSend.Location = new Point(150, 351);
        btnSend.Size = new Size(140, 36);
        btnSend.FlatStyle = FlatStyle.Flat;
        btnSend.BackColor = Color.FromArgb(30, 86, 240);
        btnSend.ForeColor = Color.White;
        btnSend.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnSend.Cursor = Cursors.Hand;
        btnSend.FlatAppearance.BorderSize = 0;

        // btnDraft
        btnDraft.Text = "Lưu bản nháp";
        btnDraft.Location = new Point(300, 351);
        btnDraft.Size = new Size(110, 36);
        btnDraft.FlatStyle = FlatStyle.Flat;
        btnDraft.BackColor = Color.White;
        btnDraft.ForeColor = Color.FromArgb(15, 23, 42);
        btnDraft.Font = new Font("Segoe UI", 9.5f);
        btnDraft.Cursor = Cursors.Hand;
        btnDraft.FlatAppearance.BorderSize = 1;
        btnDraft.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);

        // btnCancel
        btnCancel.Text = "Hủy";
        btnCancel.Location = new Point(420, 351);
        btnCancel.Size = new Size(80, 36);
        btnCancel.FlatStyle = FlatStyle.Flat;
        btnCancel.BackColor = Color.White;
        btnCancel.ForeColor = Color.FromArgb(15, 23, 42);
        btnCancel.Font = new Font("Segoe UI", 9.5f);
        btnCancel.Cursor = Cursors.Hand;
        btnCancel.FlatAppearance.BorderSize = 1;
        btnCancel.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);

        Controls.Add(btnCancel);
        Controls.Add(btnDraft);
        Controls.Add(btnSend);
        Controls.Add(txtNoiDung);
        Controls.Add(lblND);
        Controls.Add(cboDoiTuong);
        Controls.Add(lblDT);
        Controls.Add(cboLoai);
        Controls.Add(lblL);
        Controls.Add(txtTieuDe);
        Controls.Add(lblTD);
        Controls.Add(lblTitle);

        ResumeLayout(false);
    }
}

