namespace LanguageCenter.WinForms.Dialogs;

partial class FormSecretResult
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
        btnCopy = UI.UiHelpers.CreatePrimaryButton("Sao chép thông tin", 200, 36);
        btnClose = UI.UiHelpers.CreateSecondaryButton("Đóng", 120, 36);

        SuspendLayout();

        lblIcon.Text = "✓";
        lblIcon.Font = new Font("Segoe UI", 24f, FontStyle.Bold);
        lblIcon.ForeColor = UI.Theme.Success;
        lblIcon.AutoSize = true;
        lblIcon.Location = new Point(30, 24);

        lblTitle.Font = UI.Theme.FontHeading;
        lblTitle.ForeColor = UI.Theme.TextDark;
        lblTitle.AutoSize = true;
        lblTitle.Location = new Point(80, 28);

        lblMessage.Font = UI.Theme.FontRegular;
        lblMessage.ForeColor = UI.Theme.Muted;
        lblMessage.AutoSize = false;
        lblMessage.Location = new Point(30, 70);
        lblMessage.Size = new Size(420, 40);

        lblUsernameCaption.Text = "Tên đăng nhập";
        lblUsernameCaption.Font = UI.Theme.FontRegular;
        lblUsernameCaption.ForeColor = UI.Theme.Muted;
        lblUsernameCaption.AutoSize = true;
        lblUsernameCaption.Location = new Point(30, 118);

        txtUsername.Location = new Point(30, 140);
        txtUsername.Size = new Size(420, 28);
        txtUsername.Font = new Font("Consolas", 11f, FontStyle.Bold);
        txtUsername.ReadOnly = true;
        txtUsername.BackColor = Color.White;

        lblPasswordCaption.Text = "Mật khẩu tạm";
        lblPasswordCaption.Font = UI.Theme.FontRegular;
        lblPasswordCaption.ForeColor = UI.Theme.Muted;
        lblPasswordCaption.AutoSize = true;
        lblPasswordCaption.Location = new Point(30, 178);

        txtPassword.Location = new Point(30, 200);
        txtPassword.Size = new Size(420, 28);
        txtPassword.Font = new Font("Consolas", 11f, FontStyle.Bold);
        txtPassword.ReadOnly = true;
        txtPassword.BackColor = Color.White;

        lblWarning.Text = "⚠ Mật khẩu này CHỈ hiển thị một lần và không thể xem lại. "
            + "Hãy sao chép và gửi cho người dùng ngay bây giờ.";
        lblWarning.Font = new Font("Segoe UI", 8.75f, FontStyle.Italic);
        lblWarning.ForeColor = UI.Theme.Warning;
        lblWarning.AutoSize = false;
        lblWarning.Location = new Point(30, 240);
        lblWarning.Size = new Size(420, 40);

        btnCopy.Location = new Point(30, 290);
        btnCopy.Click += BtnCopy_Click;

        btnClose.Location = new Point(330, 290);
        btnClose.Click += (_, _) => Close();

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
