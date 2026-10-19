namespace LanguageCenter.WinForms.UserControls;

partial class UcKhoaHoc
{
    private System.ComponentModel.IContainer components = null;

    private Panel pnlHeaderStats = null!;
    private TableLayoutPanel tblStats = null!;
    private Panel pnlTabBar = null!;
    private Button btnTabCourses = null!;
    private Button btnTabLevels = null!;
    private Button btnTabPrograms = null!;
    private Panel pnlToolbar = null!;
    private TextBox txtSearch = null!;
    private ComboBox cboChuongTrinh = null!;
    private ComboBox cboTrangThai = null!;
    private Button btnSearch = null!;
    private Button btnRefresh = null!;
    private Button btnAdd = null!;
    private Panel pnlGridContainer = null!;
    private DataGridView dgvCourses = null!;
    private DataGridView dgvLevels = null!;
    private DataGridView dgvPrograms = null!;
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
        pnlTabBar = new Panel();
        btnTabCourses = new Button();
        btnTabLevels = new Button();
        btnTabPrograms = new Button();
        pnlToolbar = new Panel();
        txtSearch = new TextBox();
        cboChuongTrinh = new ComboBox();
        cboTrangThai = new ComboBox();
        btnSearch = new Button();
        btnRefresh = new Button();
        btnAdd = new Button();
        pnlGridContainer = new Panel();
        dgvCourses = new DataGridView();
        dgvLevels = new DataGridView();
        dgvPrograms = new DataGridView();
        pnlFooter = new Panel();
        lblSummary = new Label();

        pnlHeaderStats.SuspendLayout();
        pnlTabBar.SuspendLayout();
        pnlToolbar.SuspendLayout();
        pnlGridContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvCourses).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvLevels).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvPrograms).BeginInit();
        pnlFooter.SuspendLayout();
        SuspendLayout();

        // UcKhoaHoc
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

        // 2. SUB-TAB BAR
        pnlTabBar.Dock = DockStyle.Top;
        pnlTabBar.Height = 54;
        pnlTabBar.BackColor = Color.White;

        btnTabCourses.Text = "Khóa học";
        btnTabCourses.Location = new Point(0, 12);
        btnTabCourses.Size = new Size(130, 36);
        btnTabCourses.FlatStyle = FlatStyle.Flat;
        btnTabCourses.FlatAppearance.BorderSize = 1;
        btnTabCourses.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnTabCourses.BackColor = Color.White;
        btnTabCourses.ForeColor = Color.FromArgb(15, 23, 42);
        btnTabCourses.Font = new Font("Segoe UI", 9.5f);
        btnTabCourses.Cursor = Cursors.Hand;

        btnTabLevels.Text = "Cấp độ đào tạo";
        btnTabLevels.Location = new Point(140, 12);
        btnTabLevels.Size = new Size(150, 36);
        btnTabLevels.FlatStyle = FlatStyle.Flat;
        btnTabLevels.FlatAppearance.BorderSize = 1;
        btnTabLevels.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnTabLevels.BackColor = Color.White;
        btnTabLevels.ForeColor = Color.FromArgb(15, 23, 42);
        btnTabLevels.Font = new Font("Segoe UI", 9.5f);
        btnTabLevels.Cursor = Cursors.Hand;

        btnTabPrograms.Text = "Chương trình đào tạo";
        btnTabPrograms.Location = new Point(300, 12);
        btnTabPrograms.Size = new Size(190, 36);
        btnTabPrograms.FlatStyle = FlatStyle.Flat;
        btnTabPrograms.FlatAppearance.BorderSize = 1;
        btnTabPrograms.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnTabPrograms.BackColor = Color.White;
        btnTabPrograms.ForeColor = Color.FromArgb(15, 23, 42);
        btnTabPrograms.Font = new Font("Segoe UI", 9.5f);
        btnTabPrograms.Cursor = Cursors.Hand;

        pnlTabBar.Controls.Add(btnTabCourses);
        pnlTabBar.Controls.Add(btnTabLevels);
        pnlTabBar.Controls.Add(btnTabPrograms);

        // 3. TOOLBAR
        pnlToolbar.Dock = DockStyle.Top;
        pnlToolbar.Height = 48;
        pnlToolbar.Padding = new Padding(0, 4, 0, 8);

        txtSearch.Font = new Font("Segoe UI", 9.5f);
        txtSearch.Width = 240;
        txtSearch.Location = new Point(0, 4);
        txtSearch.PlaceholderText = "Tìm theo mã, tên...";

        cboChuongTrinh.Font = new Font("Segoe UI", 9.5f);
        cboChuongTrinh.DropDownStyle = ComboBoxStyle.DropDownList;
        cboChuongTrinh.Width = 180;
        cboChuongTrinh.Location = new Point(250, 4);

        cboTrangThai.Font = new Font("Segoe UI", 9.5f);
        cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTrangThai.Width = 160;
        cboTrangThai.Location = new Point(440, 4);

        btnSearch.Text = "Tìm kiếm";
        btnSearch.Size = new Size(100, 32);
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
        btnRefresh.Location = new Point(718, 3);

        btnAdd.Text = "Thêm khóa học";
        btnAdd.Size = new Size(140, 32);
        btnAdd.FlatStyle = FlatStyle.Flat;
        btnAdd.FlatAppearance.BorderSize = 0;
        btnAdd.BackColor = Color.FromArgb(30, 86, 240);
        btnAdd.ForeColor = Color.White;
        btnAdd.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnAdd.Cursor = Cursors.Hand;
        btnAdd.Location = new Point(832, 3);

        pnlToolbar.Controls.Add(btnAdd);
        pnlToolbar.Controls.Add(btnRefresh);
        pnlToolbar.Controls.Add(btnSearch);
        pnlToolbar.Controls.Add(cboTrangThai);
        pnlToolbar.Controls.Add(cboChuongTrinh);
        pnlToolbar.Controls.Add(txtSearch);

        // 4. GRID CONTAINER & GRIDS
        pnlGridContainer.Dock = DockStyle.Fill;

        dgvCourses.Dock = DockStyle.Fill;
        dgvCourses.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaKhoaHoc", HeaderText = "Mã khóa", FillWeight = 85 });
        dgvCourses.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenKhoaHoc", HeaderText = "Tên khóa học", FillWeight = 190 });
        dgvCourses.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenChuongTrinh", HeaderText = "Chương trình", FillWeight = 130 });
        dgvCourses.Columns.Add(new DataGridViewTextBoxColumn { Name = "CapDo", HeaderText = "Cấp độ", FillWeight = 120 });
        dgvCourses.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoBuoi", HeaderText = "Số buổi", FillWeight = 70 });
        dgvCourses.Columns.Add(new DataGridViewTextBoxColumn { Name = "TongSoGio", HeaderText = "Thời lượng", FillWeight = 80 });
        dgvCourses.Columns.Add(new DataGridViewTextBoxColumn { Name = "HocPhiCoBan", HeaderText = "Học phí niêm yết", FillWeight = 110 });
        dgvCourses.Columns.Add(new DataGridViewTextBoxColumn { Name = "NgayBatDau", HeaderText = "Khai giảng", FillWeight = 85 });
        dgvCourses.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", FillWeight = 95 });

        dgvLevels.Dock = DockStyle.Fill;
        dgvLevels.Visible = false;
        dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaCapDo", HeaderText = "Mã CĐ", FillWeight = 60 });
        dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenCapDo", HeaderText = "Tên cấp độ đào tạo", FillWeight = 180 });
        dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { Name = "KyHieu", HeaderText = "Ký hiệu", FillWeight = 85 });
        dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenChuongTrinh", HeaderText = "Chương trình", FillWeight = 130 });
        dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { Name = "ThuTu", HeaderText = "Thứ tự", FillWeight = 60 });
        dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { Name = "ThoiLuong", HeaderText = "Thời lượng (tuần)", FillWeight = 90 });
        dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { Name = "DieuKienDauVao", HeaderText = "Điều kiện đầu vào", FillWeight = 140 });
        dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { Name = "KetQuaDauRa", HeaderText = "Chuẩn đầu ra cam kết", FillWeight = 150 });
        dgvLevels.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", FillWeight = 90 });

        dgvPrograms.Dock = DockStyle.Fill;
        dgvPrograms.Visible = false;
        dgvPrograms.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaChuongTrinh", HeaderText = "Mã CT", FillWeight = 60 });
        dgvPrograms.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenChuongTrinh", HeaderText = "Tên chương trình đào tạo", FillWeight = 160 });
        dgvPrograms.Columns.Add(new DataGridViewTextBoxColumn { Name = "NhomDoiTuong", HeaderText = "Nhóm đối tượng mục tiêu", FillWeight = 180 });
        dgvPrograms.Columns.Add(new DataGridViewTextBoxColumn { Name = "DoTuoi", HeaderText = "Độ tuổi", FillWeight = 75 });
        dgvPrograms.Columns.Add(new DataGridViewTextBoxColumn { Name = "MucTieu", HeaderText = "Mục tiêu cam kết", FillWeight = 170 });
        dgvPrograms.Columns.Add(new DataGridViewTextBoxColumn { Name = "MoTa", HeaderText = "Mô tả tổng quan", FillWeight = 180 });
        dgvPrograms.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", FillWeight = 90 });

        pnlGridContainer.Controls.Add(dgvPrograms);
        pnlGridContainer.Controls.Add(dgvLevels);
        pnlGridContainer.Controls.Add(dgvCourses);

        // 5. FOOTER
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Height = 36;

        lblSummary.AutoSize = true;
        lblSummary.Font = new Font("Segoe UI", 9.5f);
        lblSummary.ForeColor = Color.FromArgb(100, 116, 139);
        lblSummary.Location = new Point(0, 8);

        pnlFooter.Controls.Add(lblSummary);

        // ADD CONTROLS TO FORM
        Controls.Add(pnlGridContainer);
        Controls.Add(pnlFooter);
        Controls.Add(pnlToolbar);
        Controls.Add(pnlTabBar);
        Controls.Add(pnlHeaderStats);

        pnlHeaderStats.ResumeLayout(false);
        pnlTabBar.ResumeLayout(false);
        pnlToolbar.ResumeLayout(false);
        pnlToolbar.PerformLayout();
        pnlGridContainer.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvCourses).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvLevels).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvPrograms).EndInit();
        pnlFooter.ResumeLayout(false);
        pnlFooter.PerformLayout();
        ResumeLayout(false);
    }
}
