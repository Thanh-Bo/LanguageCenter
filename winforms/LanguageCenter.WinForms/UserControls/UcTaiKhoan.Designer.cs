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
    private Label lblStatus = null!;
    private FlowLayoutPanel flpPager = null!;
    private Label lblPageSize = null!;
    private ComboBox cboPageSize = null!;
    private Label lblPageInfo = null!;
    private Button btnFirstPage = null!;
    private Button btnPrevPage = null!;
    private Button btnNextPage = null!;
    private Button btnLastPage = null!;

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
        lblStatus = new Label();
        flpPager = new FlowLayoutPanel();
        lblPageSize = new Label();
        cboPageSize = new ComboBox();
        lblPageInfo = new Label();

        pnlToolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvAccounts).BeginInit();
        pnlFooter.SuspendLayout();
        flpPager.SuspendLayout();
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
        txtSearch.Width = 240;
        txtSearch.Location = new Point(0, 5);
        txtSearch.PlaceholderText = "Tìm theo tên đăng nhập, email hoặc họ tên...";

        cboRole.Font = new Font("Segoe UI", 9.5f);
        cboRole.DropDownStyle = ComboBoxStyle.DropDownList;
        cboRole.Width = 160;
        cboRole.Location = new Point(248, 5);

        cboStatus.Font = new Font("Segoe UI", 9.5f);
        cboStatus.DropDownStyle = ComboBoxStyle.DropDownList;
        cboStatus.Width = 150;
        cboStatus.Location = new Point(416, 5);

        btnSearch.Text = "Tìm kiếm";
        btnSearch.Size = new Size(110, 32);
        btnSearch.FlatStyle = FlatStyle.Flat;
        btnSearch.FlatAppearance.BorderSize = 0;
        btnSearch.BackColor = Color.FromArgb(30, 86, 240);
        btnSearch.ForeColor = Color.White;
        btnSearch.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnSearch.Cursor = Cursors.Hand;
        btnSearch.Location = new Point(574, 2);

        btnRefresh.Text = "Làm mới";
        btnRefresh.Size = new Size(105, 32);
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.FlatAppearance.BorderSize = 1;
        btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnRefresh.BackColor = Color.White;
        btnRefresh.ForeColor = Color.FromArgb(15, 23, 42);
        btnRefresh.Font = new Font("Segoe UI", 9.5f);
        btnRefresh.Cursor = Cursors.Hand;
        btnRefresh.Location = new Point(682, 2);

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

        // Footer — chứa thông báo lỗi (trái) và cụm phân trang (phải).
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Height = 54;

        lblStatus.AutoSize = true;
        lblStatus.Font = new Font("Segoe UI", 9.5f);
        lblStatus.ForeColor = Color.FromArgb(239, 68, 68);
        lblStatus.Location = new Point(0, 20);

        // Cụm phân trang: neo phải bằng FlowLayoutPanel (RightToLeft) để không
        // phụ thuộc vào bề rộng cửa sổ — tránh bị tràn/che khuất như bản cũ.
        flpPager.Dock = DockStyle.Right;
        flpPager.AutoSize = true;
        flpPager.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        flpPager.WrapContents = false;
        flpPager.FlowDirection = FlowDirection.RightToLeft;
        flpPager.Padding = new Padding(0, 11, 4, 0);

        lblPageSize.AutoSize = true;
        lblPageSize.Text = "Số dòng/trang:";
        lblPageSize.Font = new Font("Segoe UI", 9.5f);
        lblPageSize.ForeColor = Color.FromArgb(100, 116, 139);
        lblPageSize.Margin = new Padding(14, 10, 4, 0);

        cboPageSize.Font = new Font("Segoe UI", 9.5f);
        cboPageSize.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPageSize.Width = 70;
        cboPageSize.Margin = new Padding(0, 6, 0, 0);

        lblPageInfo.AutoSize = true;
        lblPageInfo.Font = new Font("Segoe UI", 9.5f);
        lblPageInfo.ForeColor = Color.FromArgb(100, 116, 139);
        lblPageInfo.Margin = new Padding(18, 10, 14, 0);

        btnFirstPage = TaoNutPhanTrang("«", 44);
        btnPrevPage = TaoNutPhanTrang("‹ Trước", 82);
        btnNextPage = TaoNutPhanTrang("Sau ›", 82);
        btnLastPage = TaoNutPhanTrang("»", 44);

        // Thứ tự Add = thứ tự từ PHẢI sang TRÁI.
        flpPager.Controls.Add(btnLastPage);
        flpPager.Controls.Add(btnNextPage);
        flpPager.Controls.Add(btnPrevPage);
        flpPager.Controls.Add(btnFirstPage);
        flpPager.Controls.Add(lblPageInfo);
        flpPager.Controls.Add(cboPageSize);
        flpPager.Controls.Add(lblPageSize);

        pnlFooter.Controls.Add(flpPager);
        pnlFooter.Controls.Add(lblStatus);

        Controls.Add(dgvAccounts);
        Controls.Add(pnlFooter);
        Controls.Add(pnlToolbar);

        pnlToolbar.ResumeLayout(false);
        pnlToolbar.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvAccounts).EndInit();
        pnlFooter.ResumeLayout(false);
        pnlFooter.PerformLayout();
        flpPager.ResumeLayout(false);
        flpPager.PerformLayout();
        ResumeLayout(false);
    }

    private static Button TaoNutPhanTrang(string text, int width) => new()
    {
        Text = text,
        Size = new Size(width, 30),
        FlatStyle = FlatStyle.Flat,
        FlatAppearance = { BorderSize = 1, BorderColor = Color.FromArgb(226, 232, 240) },
        BackColor = Color.White,
        ForeColor = Color.FromArgb(15, 23, 42),
        Font = new Font("Segoe UI", 9.5f),
        Cursor = Cursors.Hand,
        Margin = new Padding(4, 6, 0, 0)
    };
}
