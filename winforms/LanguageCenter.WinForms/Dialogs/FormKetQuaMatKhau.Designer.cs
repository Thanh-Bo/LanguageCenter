namespace LanguageCenter.WinForms.Dialogs;

partial class FormKetQuaMatKhau
{
    private System.ComponentModel.IContainer components = null;

    private Label lblIcon = null!;
    private Label lblTitle = null!;
    private Label lblMessage = null!;
    private Label lblUsernameCaption = null!;
    private TextBox txtUsername = null!;
    private Label lblPasswordCaption = null!;
    private TextBox txtPassword = null!;
    private Label lblWarning = null!;
    private Button btnCopy = null!;
    private Button btnClose = null!;

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

        lblIcon = new Label();
        lblTitle = new Label();
        lblMessage = new Label();
        lblUsernameCaption = new Label();
        txtUsername = new TextBox();
        lblPasswordCaption = new Label();
        txtPassword = new TextBox();
        lblWarning = new Label();

        btnCopy = new Button();
        btnCopy.Text = "Sao chép thông tin";
        btnCopy.Size = new Size(200, 36);
        btnCopy.FlatStyle = FlatStyle.Flat;
        btnCopy.BackColor = Color.FromArgb(30, 86, 240);
        btnCopy.ForeColor = Color.White;
        btnCopy.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnCopy.FlatAppearance.BorderSize = 0;
        btnCopy.Cursor = Cursors.Hand;

        btnClose = new Button();
        btnClose.Text = "Đóng";
        btnClose.Size = new Size(120, 36);
        btnClose.FlatStyle = FlatStyle.Flat;
        btnClose.BackColor = Color.White;
        btnClose.ForeColor = Color.FromArgb(15, 23, 42);
        btnClose.Font = new Font("Segoe UI", 9.5f);
        btnClose.FlatAppearance.BorderSize = 1;
        btnClose.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnClose.Cursor = Cursors.Hand;

        SuspendLayout();

        lblIcon.Text = "";
        lblIcon.Visible = false;

        lblTitle.Font = new Font("Segoe UI", 11.5f, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(15, 23, 42);
        lblTitle.AutoSize = true;
        lblTitle.Location = new Point(30, 28);

        lblMessage.Font = new Font("Segoe UI", 9.5f);
        lblMessage.ForeColor = Color.FromArgb(100, 116, 139);
        lblMessage.AutoSize = false;
        lblMessage.Location = new Point(30, 70);
        lblMessage.Size = new Size(420, 40);

        lblUsernameCaption.Text = "Tên đăng nhập";
        lblUsernameCaption.Font = new Font("Segoe UI", 9.5f);
        lblUsernameCaption.ForeColor = Color.FromArgb(100, 116, 139);
        lblUsernameCaption.AutoSize = true;
        lblUsernameCaption.Location = new Point(30, 118);

        txtUsername.Location = new Point(30, 140);
        txtUsername.Size = new Size(420, 28);
        txtUsername.Font = new Font("Consolas", 11f, FontStyle.Bold);
        txtUsername.ReadOnly = true;
        txtUsername.BackColor = Color.White;

        lblPasswordCaption.Text = "Mật khẩu mặc định";
        lblPasswordCaption.Font = new Font("Segoe UI", 9.5f);
        lblPasswordCaption.ForeColor = Color.FromArgb(100, 116, 139);
        lblPasswordCaption.AutoSize = true;
        lblPasswordCaption.Location = new Point(30, 178);

        txtPassword.Location = new Point(30, 200);
        txtPassword.Size = new Size(420, 28);
        txtPassword.Font = new Font("Consolas", 11f, FontStyle.Bold);
        txtPassword.ReadOnly = true;
        txtPassword.BackColor = Color.White;

        lblWarning.Text = " Mật khẩu mặc định của trung tâm là 111111. "
            + "Hãy nhắc người dùng đổi mật khẩu ngay sau lần đăng nhập đầu tiên.";
        lblWarning.Font = new Font("Segoe UI", 8.75f, FontStyle.Italic);
        lblWarning.ForeColor = Color.FromArgb(245, 158, 11);
        lblWarning.AutoSize = false;
        lblWarning.Location = new Point(30, 240);
        lblWarning.Size = new Size(420, 40);

        btnCopy.Location = new Point(30, 290);
        btnCopy.Click += BtnCopy_Click;

        btnClose.Location = new Point(330, 290);

        Controls.Add(btnClose);
        Controls.Add(btnCopy);
        Controls.Add(lblWarning);
        Controls.Add(txtPassword);
        Controls.Add(lblPasswordCaption);
        Controls.Add(txtUsername);
        Controls.Add(lblUsernameCaption);
        Controls.Add(lblMessage);
        Controls.Add(lblTitle);
        Controls.Add(lblIcon);

        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(480, 350);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Tạo thành công";

        ResumeLayout(false);
    }
}
