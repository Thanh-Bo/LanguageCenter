namespace LanguageCenter.WinForms.UserControls;

partial class UcPhanCongGiangDay
{
    private System.ComponentModel.IContainer components = null;

    private Panel pnlHeaderStats = null!;
    private TableLayoutPanel tblStats = null!;
    private Panel pnlToolbar = null!;
    private TextBox txtSearch = null!;
    private ComboBox cboLopHoc = null!;
    private ComboBox cboVaiTro = null!;
    private Button btnSearch = null!;
    private Button btnRefresh = null!;
    private Button btnAdd = null!;
    private Button btnDelete = null!;
    private DataGridView dgvAssignments = null!;
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
        cboLopHoc = new ComboBox();
        cboVaiTro = new ComboBox();
        btnSearch = new Button();
        btnRefresh = new Button();
        btnAdd = new Button();
        btnDelete = new Button();
        dgvAssignments = new DataGridView();
        pnlFooter = new Panel();
        lblSummary = new Label();

        pnlHeaderStats.SuspendLayout();
        pnlToolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvAssignments).BeginInit();
        pnlFooter.SuspendLayout();
        SuspendLayout();

        // UcPhanCongGiangDay
        Dock = DockStyle.Fill;
        BackColor = Color.White;

        // 1. STATS BANNER
        pnlHeaderStats.Dock = DockStyle.Top;
        pnlHeaderStats.Height = 118;
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
        txtSearch.Width = 220;
        txtSearch.Location = new Point(0, 4);
        txtSearch.PlaceholderText = "Tìm theo tên GV, trợ giảng...";

        cboLopHoc.Font = new Font("Segoe UI", 9.5f);
        cboLopHoc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLopHoc.Width = 200;
        cboLopHoc.Location = new Point(230, 4);

        cboVaiTro.Font = new Font("Segoe UI", 9.5f);
        cboVaiTro.DropDownStyle = ComboBoxStyle.DropDownList;
        cboVaiTro.Width = 160;
        cboVaiTro.Location = new Point(440, 4);

        btnSearch.Text = "Tìm kiếm";
        btnSearch.Size = new Size(95, 32);
        btnSearch.FlatStyle = FlatStyle.Flat;
        btnSearch.FlatAppearance.BorderSize = 0;
        btnSearch.BackColor = Color.FromArgb(30, 86, 240);
        btnSearch.ForeColor = Color.White;
        btnSearch.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnSearch.Cursor = Cursors.Hand;
        btnSearch.Location = new Point(610, 3);

        btnRefresh.Text = "Làm mới";
        btnRefresh.Size = new Size(105, 32);
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.FlatAppearance.BorderSize = 1;
        btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnRefresh.BackColor = Color.White;
        btnRefresh.ForeColor = Color.FromArgb(15, 23, 42);
        btnRefresh.Font = new Font("Segoe UI", 9.5f);
        btnRefresh.Cursor = Cursors.Hand;
        btnRefresh.Location = new Point(715, 3);

        btnAdd.Text = "+ Phân công mới";
        btnAdd.Size = new Size(160, 32);
        btnAdd.FlatStyle = FlatStyle.Flat;
        btnAdd.FlatAppearance.BorderSize = 0;
        btnAdd.BackColor = Color.FromArgb(30, 86, 240);
        btnAdd.ForeColor = Color.White;
        btnAdd.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnAdd.Cursor = Cursors.Hand;
        btnAdd.Location = new Point(830, 3);

        btnDelete.Text = "Hủy phân công";
        btnDelete.Size = new Size(130, 32);
        btnDelete.FlatStyle = FlatStyle.Flat;
        btnDelete.FlatAppearance.BorderSize = 0;
        btnDelete.BackColor = Color.FromArgb(239, 68, 68);
        btnDelete.ForeColor = Color.White;
        btnDelete.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnDelete.Cursor = Cursors.Hand;
        btnDelete.Location = new Point(1000, 3);

        pnlToolbar.Controls.Add(btnDelete);
        pnlToolbar.Controls.Add(btnAdd);
        pnlToolbar.Controls.Add(btnRefresh);
        pnlToolbar.Controls.Add(btnSearch);
        pnlToolbar.Controls.Add(cboVaiTro);
        pnlToolbar.Controls.Add(cboLopHoc);
        pnlToolbar.Controls.Add(txtSearch);

        // 3. DATAGRIDVIEW
        dgvAssignments.Dock = DockStyle.Fill;
        dgvAssignments.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaPhanCong", HeaderText = "Mã PC", FillWeight = 60 });
        dgvAssignments.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenLop", HeaderText = "Lớp học phụ trách", FillWeight = 160 });
        dgvAssignments.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaGiaoVien", HeaderText = "Mã GV/TG", FillWeight = 85 });
        dgvAssignments.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenGiaoVien", HeaderText = "Họ tên nhân sự", FillWeight = 150 });
        dgvAssignments.Columns.Add(new DataGridViewTextBoxColumn { Name = "LoaiNhanSu", HeaderText = "Phân loại", FillWeight = 90 });
        dgvAssignments.Columns.Add(new DataGridViewTextBoxColumn { Name = "VaiTroPhanCong", HeaderText = "Vai trò phân công", FillWeight = 120 });
        dgvAssignments.Columns.Add(new DataGridViewTextBoxColumn { Name = "NgayBatDau", HeaderText = "Ngày bắt đầu", FillWeight = 85 });
        dgvAssignments.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", FillWeight = 95 });
        dgvAssignments.Columns.Add(new DataGridViewTextBoxColumn { Name = "GhiChu", HeaderText = "Ghi chú nhiệm vụ", FillWeight = 180 });

        // 4. FOOTER
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Height = 36;

        lblSummary.AutoSize = true;
        lblSummary.Font = new Font("Segoe UI", 9.5f);
        lblSummary.ForeColor = Color.FromArgb(100, 116, 139);
        lblSummary.Location = new Point(0, 8);

        pnlFooter.Controls.Add(lblSummary);

        // ADD CONTROLS
        Controls.Add(dgvAssignments);
        Controls.Add(pnlFooter);
        Controls.Add(pnlToolbar);
        Controls.Add(pnlHeaderStats);

        pnlHeaderStats.ResumeLayout(false);
        pnlToolbar.ResumeLayout(false);
        pnlToolbar.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvAssignments).EndInit();
        pnlFooter.ResumeLayout(false);
        pnlFooter.PerformLayout();
        ResumeLayout(false);
    }
}
