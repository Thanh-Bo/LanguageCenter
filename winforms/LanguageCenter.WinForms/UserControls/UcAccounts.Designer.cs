namespace LanguageCenter.WinForms.UserControls;

partial class UcAccounts
{
    private System.ComponentModel.IContainer components = null;

    private Panel pnlToolbar = null!;
    private TextBox txtSearch = null!;
    private ComboBox cboRole = null!;
    private ComboBox cboStatus = null!;
    private Button btnSearch = null!;
    private Button btnRefresh = null!;
    private DataGridView dgvAccounts = null!;
    private Panel pnlFooter = null!;
    private Label lblPageInfo = null!;
    private Button btnPrevPage = null!;
    private Button btnNextPage = null!;
    private Label lblStatus = null!;

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

        pnlToolbar = new Panel();
        txtSearch = UI.UiHelpers.CreateTextBox(260);
        cboRole = UI.UiHelpers.CreateComboBox(180);
        cboStatus = UI.UiHelpers.CreateComboBox(160);
        btnSearch = UI.UiHelpers.CreatePrimaryButton("Tìm kiếm", 110, 32);
        btnRefresh = UI.UiHelpers.CreateSecondaryButton("⟳", 42, 32);
        dgvAccounts = new DataGridView();
        pnlFooter = new Panel();
        lblPageInfo = new Label();
        btnPrevPage = UI.UiHelpers.CreateSecondaryButton("‹ Trước", 90, 30);
        btnNextPage = UI.UiHelpers.CreateSecondaryButton("Sau ›", 90, 30);
        lblStatus = new Label();

        SuspendLayout();

        Dock = DockStyle.Fill;
        BackColor = Color.White;
        Padding = new Padding(0);

        // pnlToolbar
        pnlToolbar.Dock = DockStyle.Top;
        pnlToolbar.Height = 56;
        pnlToolbar.Padding = new Padding(0, 0, 0, 12);

        txtSearch.Location = new Point(0, 2);
        txtSearch.PlaceholderText = "Tìm theo tên đăng nhập, email hoặc họ tên...";
        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; _ = ReloadAsync(resetPage: true); } };

        cboRole.Location = new Point(272, 2);
        cboStatus.Location = new Point(464, 2);

        btnSearch.Location = new Point(636, 1);
        btnSearch.Click += async (_, _) => await ReloadAsync(resetPage: true);

        btnRefresh.Location = new Point(754, 1);
        btnRefresh.Click += async (_, _) => await ReloadAsync(resetPage: false);

        pnlToolbar.Controls.Add(btnRefresh);
        pnlToolbar.Controls.Add(btnSearch);
        pnlToolbar.Controls.Add(cboStatus);
        pnlToolbar.Controls.Add(cboRole);
        pnlToolbar.Controls.Add(txtSearch);

        // dgvAccounts
        dgvAccounts.Dock = DockStyle.Fill;
        UI.UiHelpers.StyleGrid(dgvAccounts);
        dgvAccounts.CellDoubleClick += DgvAccounts_CellDoubleClick;

        dgvAccounts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Visible = false });
        dgvAccounts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Username", HeaderText = "Tên đăng nhập", FillWeight = 90 });
        dgvAccounts.Columns.Add(new DataGridViewTextBoxColumn { Name = "FullName", HeaderText = "Họ tên", FillWeight = 130 });
        dgvAccounts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", FillWeight = 140 });
        dgvAccounts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Role", HeaderText = "Vai trò", FillWeight = 110 });
        dgvAccounts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Trạng thái", FillWeight = 90 });
        dgvAccounts.Columns.Add(new DataGridViewTextBoxColumn { Name = "LastLoginAt", HeaderText = "Đăng nhập gần nhất", FillWeight = 110 });

        // pnlFooter
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Height = 44;

        lblStatus.AutoSize = true;
        lblStatus.Font = UI.Theme.FontRegular;
        lblStatus.ForeColor = UI.Theme.Danger;
        lblStatus.Location = new Point(0, 12);

        lblPageInfo.AutoSize = true;
        lblPageInfo.Font = UI.Theme.FontRegular;
        lblPageInfo.ForeColor = UI.Theme.Muted;
        lblPageInfo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblPageInfo.Location = new Point(620, 12);

        btnPrevPage.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnPrevPage.Location = new Point(800, 6);
        btnPrevPage.Click += async (_, _) => await ChangePageAsync(-1);

        btnNextPage.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnNextPage.Location = new Point(896, 6);
        btnNextPage.Click += async (_, _) => await ChangePageAsync(1);

        pnlFooter.Controls.Add(btnNextPage);
        pnlFooter.Controls.Add(btnPrevPage);
        pnlFooter.Controls.Add(lblPageInfo);
        pnlFooter.Controls.Add(lblStatus);

        Controls.Add(dgvAccounts);
        Controls.Add(pnlFooter);
        Controls.Add(pnlToolbar);

        ResumeLayout(false);
    }
}
