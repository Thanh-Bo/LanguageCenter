namespace LanguageCenter.WinForms.UserControls;

partial class UcTroGiang
{
    private System.ComponentModel.IContainer components = null;

    private Panel pnlToolbar = null!;
    private TextBox txtSearch = null!;
    private ComboBox cboTrangThai = null!;
    private Button btnSearch = null!;
    private Button btnRefresh = null!;
    private Button btnAdd = null!;
    private DataGridView dgvAssistants = null!;
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
        cboTrangThai = new ComboBox();
        btnSearch = new Button();
        btnRefresh = new Button();
        btnAdd = new Button();
        dgvAssistants = new DataGridView();
        pnlFooter = new Panel();
        lblPageInfo = new Label();
        btnPrevPage = new Button();
        btnNextPage = new Button();
        lblStatus = new Label();

        pnlToolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvAssistants).BeginInit();
        pnlFooter.SuspendLayout();
        SuspendLayout();

        // UcTroGiang
        Dock = DockStyle.Fill;
        BackColor = Color.White;

        // Toolbar
        pnlToolbar.Dock = DockStyle.Top;
        pnlToolbar.Height = 56;
        pnlToolbar.Padding = new Padding(0, 0, 0, 12);

        txtSearch.Font = new Font("Segoe UI", 9.5f);
        txtSearch.Width = 260;
        txtSearch.Location = new Point(0, 2);
        txtSearch.PlaceholderText = "Tìm trợ giảng theo mã, họ tên hoặc email...";

        cboTrangThai.Font = new Font("Segoe UI", 9.5f);
        cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTrangThai.Width = 160;
        cboTrangThai.Location = new Point(272, 2);

        btnSearch.Text = "Tìm kiếm";
        btnSearch.Size = new Size(100, 32);
        btnSearch.FlatStyle = FlatStyle.Flat;
        btnSearch.FlatAppearance.BorderSize = 0;
        btnSearch.BackColor = Color.FromArgb(30, 86, 240);
        btnSearch.ForeColor = Color.White;
        btnSearch.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnSearch.Cursor = Cursors.Hand;
        btnSearch.Location = new Point(444, 1);

        btnRefresh.Text = "Làm mới";
        btnRefresh.Size = new Size(105, 32);
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.FlatAppearance.BorderSize = 1;
        btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnRefresh.BackColor = Color.White;
        btnRefresh.ForeColor = Color.FromArgb(15, 23, 42);
        btnRefresh.Font = new Font("Segoe UI", 9.5f);
        btnRefresh.Cursor = Cursors.Hand;
        btnRefresh.Location = new Point(552, 1);

        btnAdd.Text = "+ Thêm trợ giảng";
        btnAdd.Size = new Size(160, 32);
        btnAdd.FlatStyle = FlatStyle.Flat;
        btnAdd.FlatAppearance.BorderSize = 0;
        btnAdd.BackColor = Color.FromArgb(30, 86, 240);
        btnAdd.ForeColor = Color.White;
        btnAdd.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnAdd.Cursor = Cursors.Hand;
        btnAdd.Location = new Point(665, 1);

        pnlToolbar.Controls.Add(btnAdd);
        pnlToolbar.Controls.Add(btnRefresh);
        pnlToolbar.Controls.Add(btnSearch);
        pnlToolbar.Controls.Add(cboTrangThai);
        pnlToolbar.Controls.Add(txtSearch);

        // DataGridView
        dgvAssistants.Dock = DockStyle.Fill;
        dgvAssistants.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaGiaoVien", HeaderText = "Mã TG", FillWeight = 75 });
        dgvAssistants.Columns.Add(new DataGridViewTextBoxColumn { Name = "HoTen", HeaderText = "Họ tên trợ giảng", FillWeight = 150 });
        dgvAssistants.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", FillWeight = 140 });
        dgvAssistants.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoDienThoai", HeaderText = "SĐT", FillWeight = 95 });
        dgvAssistants.Columns.Add(new DataGridViewTextBoxColumn { Name = "ChuyenMon", HeaderText = "Kỹ năng hỗ trợ / Chuyên môn", FillWeight = 160 });
        dgvAssistants.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoNamKinhNghiem", HeaderText = "Số năm KN", FillWeight = 80 });
        dgvAssistants.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", FillWeight = 90 });

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
        lblPageInfo.Location = new Point(600, 12);

        btnPrevPage.Text = "Trước";
        btnPrevPage.Size = new Size(90, 30);
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
        btnNextPage.Size = new Size(90, 30);
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

        Controls.Add(dgvAssistants);
        Controls.Add(pnlFooter);
        Controls.Add(pnlToolbar);

        pnlToolbar.ResumeLayout(false);
        pnlToolbar.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvAssistants).EndInit();
        pnlFooter.ResumeLayout(false);
        pnlFooter.PerformLayout();
        ResumeLayout(false);
    }
}
