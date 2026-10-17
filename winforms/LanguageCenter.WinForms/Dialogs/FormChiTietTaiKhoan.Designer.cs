namespace LanguageCenter.WinForms.Dialogs;

partial class FormChiTietTaiKhoan
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel tableInfo = null!;
    private Label lblUsernameCaption = null!;
    private Label lblUsernameValue = null!;
    private Label lblFullNameCaption = null!;
    private Label lblFullNameValue = null!;
    private Label lblEmailCaption = null!;
    private Label lblEmailValue = null!;
    private Label lblRoleCaption = null!;
    private Label lblRoleValue = null!;
    private Label lblProfileCaption = null!;
    private Label lblProfileValue = null!;
    private Label lblCreatedCaption = null!;
    private Label lblCreatedValue = null!;
    private Label lblLastLoginCaption = null!;
    private Label lblLastLoginValue = null!;
    private Label lblStatusCaption = null!;
    private ComboBox cboStatus = null!;
    private Button btnSaveStatus = null!;
    private Button btnResetPassword = null!;
    private Button btnDelete = null!;
    private Button btnCloseDialog = null!;
    private Label lblMessage = null!;
    private Panel pnlButtons = null!;

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

        lblUsernameCaption = new Label { Text = "Tên đăng nhập", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Anchor = AnchorStyles.Left };
        lblUsernameValue = new Label { Text = "—", AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), Anchor = AnchorStyles.Left };

        lblFullNameCaption = new Label { Text = "Họ tên", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Anchor = AnchorStyles.Left };
        lblFullNameValue = new Label { Text = "—", AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), Anchor = AnchorStyles.Left };

        lblEmailCaption = new Label { Text = "Email", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Anchor = AnchorStyles.Left };
        lblEmailValue = new Label { Text = "—", AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), Anchor = AnchorStyles.Left };

        lblRoleCaption = new Label { Text = "Vai trò", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Anchor = AnchorStyles.Left };
        lblRoleValue = new Label { Text = "—", AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), Anchor = AnchorStyles.Left };

        lblProfileCaption = new Label { Text = "Hồ sơ liên kết", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Anchor = AnchorStyles.Left };
        lblProfileValue = new Label { Text = "—", AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), Anchor = AnchorStyles.Left };

        lblCreatedCaption = new Label { Text = "Ngày tạo", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Anchor = AnchorStyles.Left };
        lblCreatedValue = new Label { Text = "—", AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), Anchor = AnchorStyles.Left };

        lblLastLoginCaption = new Label { Text = "Đăng nhập gần nhất", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Anchor = AnchorStyles.Left };
        lblLastLoginValue = new Label { Text = "—", AutoSize = true, Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(15, 23, 42), Anchor = AnchorStyles.Left };

        lblStatusCaption = new Label { Text = "Trạng thái tài khoản", AutoSize = true, Font = new Font("Segoe UI", 9.5f), ForeColor = Color.FromArgb(100, 116, 139), Location = new Point(24, 296) };

        cboStatus = new ComboBox
        {
            Location = new Point(194, 292),
            Width = 220,
            Font = new Font("Segoe UI", 9.5f),
            DropDownStyle = ComboBoxStyle.DropDownList
        };

        btnSaveStatus = new Button
        {
            Text = "Lưu trạng thái",
            Location = new Point(424, 290),
            Size = new Size(140, 32),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(30, 86, 240),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnSaveStatus.FlatAppearance.BorderSize = 0;
        btnSaveStatus.Click += BtnSaveStatus_Click;

        btnResetPassword = new Button
        {
            Text = "Đặt lại mật khẩu",
            Location = new Point(0, 0),
            Size = new Size(160, 32),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(15, 23, 42),
            Font = new Font("Segoe UI", 9.5f),
            Cursor = Cursors.Hand
        };
        btnResetPassword.FlatAppearance.BorderSize = 1;
        btnResetPassword.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnResetPassword.Click += BtnResetPassword_Click;

        btnDelete = new Button
        {
            Text = "Xóa tài khoản",
            Location = new Point(170, 0),
            Size = new Size(140, 32),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.FromArgb(239, 68, 68),
            ForeColor = Color.White,
            Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
            Cursor = Cursors.Hand
        };
        btnDelete.FlatAppearance.BorderSize = 0;
        btnDelete.Click += BtnDelete_Click;

        btnCloseDialog = new Button
        {
            Text = "Đóng",
            Location = new Point(400, 0),
            Size = new Size(100, 32),
            FlatStyle = FlatStyle.Flat,
            BackColor = Color.White,
            ForeColor = Color.FromArgb(15, 23, 42),
            Font = new Font("Segoe UI", 9.5f),
            Cursor = Cursors.Hand
        };
        btnCloseDialog.FlatAppearance.BorderSize = 1;
        btnCloseDialog.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);

        lblMessage = new Label
        {
            Location = new Point(24, 390),
            Size = new Size(500, 40),
            Font = new Font("Segoe UI", 9.5f),
            ForeColor = Color.FromArgb(239, 68, 68)
        };

        pnlButtons = new Panel
        {
            Location = new Point(24, 340),
            Size = new Size(500, 40)
        };
        pnlButtons.Controls.Add(btnResetPassword);
        pnlButtons.Controls.Add(btnDelete);
        pnlButtons.Controls.Add(btnCloseDialog);

        tableInfo = new TableLayoutPanel
        {
            Location = new Point(24, 20),
            Size = new Size(500, 260),
            ColumnCount = 2,
            RowCount = 7,
            CellBorderStyle = TableLayoutPanelCellBorderStyle.None
        };
        tableInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
        tableInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (var i = 0; i < 7; i++)
        {
            tableInfo.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));
        }

        tableInfo.Controls.Add(lblUsernameCaption, 0, 0);
        tableInfo.Controls.Add(lblUsernameValue, 1, 0);
        tableInfo.Controls.Add(lblFullNameCaption, 0, 1);
        tableInfo.Controls.Add(lblFullNameValue, 1, 1);
        tableInfo.Controls.Add(lblEmailCaption, 0, 2);
        tableInfo.Controls.Add(lblEmailValue, 1, 2);
        tableInfo.Controls.Add(lblRoleCaption, 0, 3);
        tableInfo.Controls.Add(lblRoleValue, 1, 3);
        tableInfo.Controls.Add(lblProfileCaption, 0, 4);
        tableInfo.Controls.Add(lblProfileValue, 1, 4);
        tableInfo.Controls.Add(lblCreatedCaption, 0, 5);
        tableInfo.Controls.Add(lblCreatedValue, 1, 5);
        tableInfo.Controls.Add(lblLastLoginCaption, 0, 6);
        tableInfo.Controls.Add(lblLastLoginValue, 1, 6);

        SuspendLayout();

        Controls.Add(lblMessage);
        Controls.Add(pnlButtons);
        Controls.Add(btnSaveStatus);
        Controls.Add(cboStatus);
        Controls.Add(lblStatusCaption);
        Controls.Add(tableInfo);

        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(580, 450);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Chi tiết tài khoản";
        Load += FormAccountDetail_Load;

        ResumeLayout(false);
    }
}
