namespace LanguageCenter.WinForms.Dialogs;

partial class FormAccountDetail
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

        lblUsernameCaption = CreateCaption("Tên đăng nhập");
        lblUsernameValue = CreateValue();
        lblFullNameCaption = CreateCaption("Họ tên");
        lblFullNameValue = CreateValue();
        lblEmailCaption = CreateCaption("Email");
        lblEmailValue = CreateValue();
        lblRoleCaption = CreateCaption("Vai trò");
        lblRoleValue = CreateValue();
        lblProfileCaption = CreateCaption("Hồ sơ liên kết");
        lblProfileValue = CreateValue();
        lblCreatedCaption = CreateCaption("Ngày tạo");
        lblCreatedValue = CreateValue();
        lblLastLoginCaption = CreateCaption("Đăng nhập gần nhất");
        lblLastLoginValue = CreateValue();
        lblStatusCaption = CreateCaption("Trạng thái tài khoản");
        cboStatus = UI.UiHelpers.CreateComboBox(220);
        btnSaveStatus = UI.UiHelpers.CreatePrimaryButton("Lưu trạng thái", 140, 32);
        btnResetPassword = UI.UiHelpers.CreateSecondaryButton("Đặt lại mật khẩu", 160, 32);
        btnDelete = UI.UiHelpers.CreateDangerButton("Xóa tài khoản", 140, 32);
        btnCloseDialog = UI.UiHelpers.CreateSecondaryButton("Đóng", 100, 32);
        lblMessage = new Label();
        pnlButtons = new Panel();

        SuspendLayout();

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

        AddRow(tableInfo, 0, lblUsernameCaption, lblUsernameValue);
        AddRow(tableInfo, 1, lblFullNameCaption, lblFullNameValue);
        AddRow(tableInfo, 2, lblEmailCaption, lblEmailValue);
        AddRow(tableInfo, 3, lblRoleCaption, lblRoleValue);
        AddRow(tableInfo, 4, lblProfileCaption, lblProfileValue);
        AddRow(tableInfo, 5, lblCreatedCaption, lblCreatedValue);
        AddRow(tableInfo, 6, lblLastLoginCaption, lblLastLoginValue);

        // status row
        lblStatusCaption.Location = new Point(24, 296);
        cboStatus.Location = new Point(194, 292);
        btnSaveStatus.Location = new Point(424, 290);
        btnSaveStatus.Click += BtnSaveStatus_Click;

        // pnlButtons
        pnlButtons.Location = new Point(24, 340);
        pnlButtons.Size = new Size(500, 40);
        btnResetPassword.Location = new Point(0, 0);
        btnResetPassword.Click += BtnResetPassword_Click;
        btnDelete.Location = new Point(170, 0);
        btnDelete.Click += BtnDelete_Click;
        btnCloseDialog.Location = new Point(400, 0);
        btnCloseDialog.Click += (_, _) => Close();
        pnlButtons.Controls.Add(btnResetPassword);
        pnlButtons.Controls.Add(btnDelete);
        pnlButtons.Controls.Add(btnCloseDialog);

        // lblMessage
        lblMessage.Location = new Point(24, 390);
        lblMessage.Size = new Size(500, 40);
        lblMessage.Font = UI.Theme.FontRegular;
        lblMessage.ForeColor = UI.Theme.Danger;

        Controls.Add(lblMessage);
        Controls.Add(pnlButtons);
        Controls.Add(btnSaveStatus);
        Controls.Add(cboStatus);
        Controls.Add(lblStatusCaption);
        Controls.Add(tableInfo);

        AutoScaleDimensions = new SizeF(96F, 96F);
        AutoScaleMode = AutoScaleMode.Dpi;
        ClientSize = new Size(548, 440);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        Text = "Chi tiết tài khoản";
        Load += FormAccountDetail_Load;

        ResumeLayout(false);
    }

    private static Label CreateCaption(string text) => new()
    {
        Text = text,
        AutoSize = true,
        Font = UI.Theme.FontRegular,
        ForeColor = UI.Theme.Muted
    };

    private static Label CreateValue() => new()
    {
        Text = "—",
        AutoSize = true,
        Font = UI.Theme.FontBold,
        ForeColor = UI.Theme.TextDark
    };

    private static void AddRow(TableLayoutPanel table, int row, Label caption, Label value)
    {
        caption.Anchor = AnchorStyles.Left;
        value.Anchor = AnchorStyles.Left;
        table.Controls.Add(caption, 0, row);
        table.Controls.Add(value, 1, row);
    }
}
