namespace LanguageCenter.WinForms.UserControls;

partial class UcTaiKhoan
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
        txtSearch = new TextBox();
        cboRole = new ComboBox();
        cboStatus = new ComboBox();
        btnSearch = new Button();
        btnRefresh = new Button();
        dgvAccounts = new DataGridView();
        pnlFooter = new Panel();
        lblPageInfo = new Label();
        btnPrevPage = new Button();
        btnNextPage = new Button();
        lblStatus = new Label();

        pnlToolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvAccounts).BeginInit();
        pnlFooter.SuspendLayout();
        SuspendLayout();

        // UcTaiKhoan
        Dock = DockStyle.Fill;
        BackColor = Color.White;
        Padding = new Padding(0);

        // Toolbar
        pnlToolbar.Dock = DockStyle.Top;
        pnlToolbar.Height = 56;
        pnlToolbar.Padding = new Padding(0, 0, 0, 12);

        txtSearch.Font = new Font("Segoe UI", 9.5f);
        txtSearch.Width = 260;
        txtSearch.Location = new Point(0, 2);
        txtSearch.PlaceholderText = "Tìm theo tên đăng nhập, email hoặc họ tên...";

        cboRole.Font = new Font("Segoe UI", 9.5f);
        cboRole.DropDownStyle = ComboBoxStyle.DropDownList;
        cboRole.Width = 180;
        cboRole.Location = new Point(272, 2);

        cboStatus.Font = new Font("Segoe UI", 9.5f);
        cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cboStatus.Width = 160;
        cboStatus.Location = new Point(464, 2);

        btnSearch.Text = "Tìm kiếm";
        btnSearch.Size = new Size(110, 32);
        btnSearch.FlatStyle = FlatStyle.Flat;
        btnSearch.FlatAppearance.BorderSize = 0;
        btnSearch.BackColor = Color.FromArgb(30, 86, 240);
        btnSearch.ForeColor = Color.White;
        btnSearch.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnSearch.Cursor = Cursors.Hand;
        btnSearch.Location = new Point(636, 1);

        btnRefresh.Text = "Làm mới";
        btnRefresh.Size = new Size(105, 32);
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.FlatAppearance.BorderSize = 1;
        btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnRefresh.BackColor = Color.White;
        btnRefresh.ForeColor = Color.FromArgb(15, 23, 42);
        btnRefresh.Font = new Font("Segoe UI", 9.5f);
        btnRefresh.Cursor = Cursors.Hand;
        btnRefresh.Location = new Point(754, 1);

        pnlToolbar.Controls.Add(btnRefresh);
        pnlToolbar.Controls.Add(btnSearch);
        pnlToolbar.Controls.Add(cboStatus);
        pnlToolbar.Controls.Add(cboRole);
        pnlToolbar.Controls.Add(txtSearch);

        // DataGridView
        dgvAccounts.Dock = DockStyle.Fill;
        dgvAccounts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Id", HeaderText = "ID", Visible = false });
        dgvAccounts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Username", HeaderText = "Tên đăng nhập", FillWeight = 90 });
        dgvAccounts.Columns.Add(new DataGridViewTextBoxColumn { Name = "FullName", HeaderText = "Họ tên", FillWeight = 130 });
        dgvAccounts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", FillWeight = 140 });
        dgvAccounts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Role", HeaderText = "Vai trò", FillWeight = 110 });
        dgvAccounts.Columns.Add(new DataGridViewTextBoxColumn { Name = "Status", HeaderText = "Trạng thái", FillWeight = 90 });
        dgvAccounts.Columns.Add(new DataGridViewTextBoxColumn { Name = "LastLoginAt", HeaderText = "Đăng nhập gần nhất", FillWeight = 110 });

        // Footer
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Height = 44;

        lblStatus.AutoSize = true;
        lblStatus.Font = new Font("Segoe UI", 9.5f);
        lblStatus.ForeColor = Color.FromArgb(239, 68, 68);
        lblStatus.Location = new Point(0, 12);

        lblPageInfo.AutoSize = true;
        lblPageInfo.Font = new Font("Segoe UI", 9.5f);
        lblPageInfo.ForeColor = Color.FromArgb(100, 116, 139);
        lblPageInfo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblPageInfo.Location = new Point(620, 12);

        btnPrevPage.Text = "Trước";
        btnPrevPage.Size = new Size(80, 30);
        btnPrevPage.FlatStyle = FlatStyle.Flat;
        btnPrevPage.FlatAppearance.BorderSize = 1;
        btnPrevPage.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnPrevPage.BackColor = Color.White;
        btnPrevPage.ForeColor = Color.FromArgb(15, 23, 42);
        btnPrevPage.Font = new Font("Segoe UI", 9.5f);
        btnPrevPage.Cursor = Cursors.Hand;
        btnPrevPage.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnPrevPage.Location = new Point(800, 6);

        btnNextPage.Text = "Sau";
        btnNextPage.Size = new Size(80, 30);
        btnNextPage.FlatStyle = FlatStyle.Flat;
        btnNextPage.FlatAppearance.BorderSize = 1;
        btnNextPage.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnNextPage.BackColor = Color.White;
        btnNextPage.ForeColor = Color.FromArgb(15, 23, 42);
        btnNextPage.Font = new Font("Segoe UI", 9.5f);
        btnNextPage.Cursor = Cursors.Hand;
        btnNextPage.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnNextPage.Location = new Point(896, 6);

        pnlFooter.Controls.Add(btnNextPage);
        pnlFooter.Controls.Add(btnPrevPage);
        pnlFooter.Controls.Add(lblPageInfo);
        pnlFooter.Controls.Add(lblStatus);

        Controls.Add(dgvAccounts);
        Controls.Add(pnlFooter);
        Controls.Add(pnlToolbar);

        pnlToolbar.ResumeLayout(false);
        pnlToolbar.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvAccounts).EndInit();
        pnlFooter.ResumeLayout(false);
        pnlFooter.PerformLayout();
        ResumeLayout(false);
    }
}
