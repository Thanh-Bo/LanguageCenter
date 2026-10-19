namespace LanguageCenter.WinForms.UserControls;

partial class UcLopHoc
{
    private System.ComponentModel.IContainer components = null;

    private Panel pnlHeaderStats = null!;
    private TableLayoutPanel tblStats = null!;
    private Panel pnlToolbar = null!;
    private TextBox txtSearch = null!;
    private ComboBox cboKhoaHoc = null!;
    private ComboBox cboTrangThai = null!;
    private Button btnSearch = null!;
    private Button btnRefresh = null!;
    private Button btnAdd = null!;
    private DataGridView dgvClasses = null!;
    private Panel pnlFooter = null!;
    private Label lblSummary = null!;

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

        pnlHeaderStats = new Panel();
        tblStats = new TableLayoutPanel();
        pnlToolbar = new Panel();
        txtSearch = new TextBox();
        cboKhoaHoc = new ComboBox();
        cboTrangThai = new ComboBox();
        btnSearch = new Button();
        btnRefresh = new Button();
        btnAdd = new Button();
        dgvClasses = new DataGridView();
        pnlFooter = new Panel();
        lblSummary = new Label();

        pnlHeaderStats.SuspendLayout();
        pnlToolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvClasses).BeginInit();
        pnlFooter.SuspendLayout();
        SuspendLayout();

        // UcLopHoc
        Dock = DockStyle.Fill;
        BackColor = Color.White;

        // 1. STATS BANNER
        pnlHeaderStats.Dock = DockStyle.Top;
        pnlHeaderStats.Height = 132;
        pnlHeaderStats.Padding = new Padding(0, 0, 0, 6);

        tblStats.Dock = DockStyle.Fill;
        tblStats.ColumnCount = 4;
        tblStats.RowCount = 1;
        tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tblStats.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        pnlHeaderStats.Controls.Add(tblStats);

        // 2. TOOLBAR
        pnlToolbar.Dock = DockStyle.Top;
        pnlToolbar.Height = 48;
        pnlToolbar.Padding = new Padding(0, 4, 0, 8);

        txtSearch.Font = new Font("Segoe UI", 9.5f);
        txtSearch.Width = 240;
        txtSearch.Location = new Point(0, 4);
        txtSearch.PlaceholderText = "Tìm theo mã lớp, tên lớp, giáo viên...";

        cboKhoaHoc.Font = new Font("Segoe UI", 9.5f);
        cboKhoaHoc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboKhoaHoc.Width = 200;
        cboKhoaHoc.Location = new Point(250, 4);

        cboTrangThai.Font = new Font("Segoe UI", 9.5f);
        cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTrangThai.Width = 150;
        cboTrangThai.Location = new Point(460, 4);

        btnSearch.Text = "Tìm kiếm";
        btnSearch.Size = new Size(100, 32);
        btnSearch.FlatStyle = FlatStyle.Flat;
        btnSearch.FlatAppearance.BorderSize = 0;
        btnSearch.BackColor = Color.FromArgb(30, 86, 240);
        btnSearch.ForeColor = Color.White;
        btnSearch.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnSearch.Cursor = Cursors.Hand;
        btnSearch.Location = new Point(620, 3);

        btnRefresh.Text = "Làm mới";
        btnRefresh.Size = new Size(105, 32);
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.FlatAppearance.BorderSize = 1;
        btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnRefresh.BackColor = Color.White;
        btnRefresh.ForeColor = Color.FromArgb(15, 23, 42);
        btnRefresh.Font = new Font("Segoe UI", 9.5f);
        btnRefresh.Cursor = Cursors.Hand;
        btnRefresh.Location = new Point(728, 3);

        btnAdd.Text = "+ Thêm lớp học";
        btnAdd.Size = new Size(150, 32);
        btnAdd.FlatStyle = FlatStyle.Flat;
        btnAdd.FlatAppearance.BorderSize = 0;
        btnAdd.BackColor = Color.FromArgb(30, 86, 240);
        btnAdd.ForeColor = Color.White;
        btnAdd.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnAdd.Cursor = Cursors.Hand;
        btnAdd.Location = new Point(841, 3);

        pnlToolbar.Controls.Add(btnAdd);
        pnlToolbar.Controls.Add(btnRefresh);
        pnlToolbar.Controls.Add(btnSearch);
        pnlToolbar.Controls.Add(cboTrangThai);
        pnlToolbar.Controls.Add(cboKhoaHoc);
        pnlToolbar.Controls.Add(txtSearch);

        // 3. DATAGRIDVIEW
        dgvClasses.Dock = DockStyle.Fill;
        dgvClasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaLop", HeaderText = "Mã lớp", FillWeight = 90 });
        dgvClasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenLop", HeaderText = "Tên lớp học", FillWeight = 170 });
        dgvClasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenKhoaHoc", HeaderText = "Khóa học", FillWeight = 170 });
        dgvClasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenNhanVienHocVu", HeaderText = "Học vụ phụ trách", FillWeight = 150 });
        dgvClasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "SiSo", HeaderText = "Sĩ số", FillWeight = 75 });
        dgvClasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoCaHoc", HeaderText = "Ca học", FillWeight = 80 });
        dgvClasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "NgayBatDau", HeaderText = "Ngày bắt đầu", FillWeight = 95 });
        dgvClasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "NgayKetThuc", HeaderText = "Kết thúc dự kiến", FillWeight = 105 });
        dgvClasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", FillWeight = 95 });

        // 4. FOOTER
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Height = 36;

        lblSummary.AutoSize = true;
        lblSummary.Font = new Font("Segoe UI", 9.5f);
        lblSummary.ForeColor = Color.FromArgb(100, 116, 139);
        lblSummary.Location = new Point(0, 8);

        pnlFooter.Controls.Add(lblSummary);

        // ADD CONTROLS
        Controls.Add(dgvClasses);
        Controls.Add(pnlFooter);
        Controls.Add(pnlToolbar);
        Controls.Add(pnlHeaderStats);

        pnlHeaderStats.ResumeLayout(false);
        pnlToolbar.ResumeLayout(false);
        pnlToolbar.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvClasses).EndInit();
        pnlFooter.ResumeLayout(false);
        pnlFooter.PerformLayout();
        ResumeLayout(false);
    }
}
