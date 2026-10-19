namespace LanguageCenter.WinForms.UserControls;

partial class UcLichHoc
{
    private System.ComponentModel.IContainer components = null;

    private Panel pnlHeaderStats = null!;
    private TableLayoutPanel tblStats = null!;
    private Panel pnlTabBar = null!;
    private Button btnTabRooms = null!;
    private Button btnTabSchedules = null!;
    private Button btnTabShifts = null!;
    private Panel pnlToolbar = null!;
    private TextBox txtSearch = null!;
    private ComboBox cboFilter = null!;
    private Button btnSearch = null!;
    private Button btnRefresh = null!;
    private Button btnAdd = null!;
    private Panel pnlGridContainer = null!;
    private DataGridView dgvRooms = null!;
    private DataGridView dgvSchedules = null!;
    private DataGridView dgvShifts = null!;
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
        btnTabRooms = new Button();
        btnTabSchedules = new Button();
        btnTabShifts = new Button();
        pnlToolbar = new Panel();
        txtSearch = new TextBox();
        cboFilter = new ComboBox();
        btnSearch = new Button();
        btnRefresh = new Button();
        btnAdd = new Button();
        pnlGridContainer = new Panel();
        dgvRooms = new DataGridView();
        dgvSchedules = new DataGridView();
        dgvShifts = new DataGridView();
        pnlFooter = new Panel();
        lblSummary = new Label();

        pnlHeaderStats.SuspendLayout();
        pnlTabBar.SuspendLayout();
        pnlToolbar.SuspendLayout();
        pnlGridContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvRooms).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvSchedules).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvShifts).BeginInit();
        pnlFooter.SuspendLayout();
        SuspendLayout();

        // UcLichHoc
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
        pnlTabBar.Height = 48;
        pnlTabBar.BackColor = Color.White;

        btnTabRooms.Text = "Quản lý phòng học";
        btnTabRooms.Location = new Point(0, 8);
        btnTabRooms.Size = new Size(160, 34);
        btnTabRooms.FlatStyle = FlatStyle.Flat;
        btnTabRooms.FlatAppearance.BorderSize = 1;
        btnTabRooms.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnTabRooms.BackColor = Color.White;
        btnTabRooms.ForeColor = Color.FromArgb(15, 23, 42);
        btnTabRooms.Font = new Font("Segoe UI", 9.5f);
        btnTabRooms.Cursor = Cursors.Hand;

        btnTabSchedules.Text = "Lịch học tuần";
        btnTabSchedules.Location = new Point(170, 8);
        btnTabSchedules.Size = new Size(160, 34);
        btnTabSchedules.FlatStyle = FlatStyle.Flat;
        btnTabSchedules.FlatAppearance.BorderSize = 1;
        btnTabSchedules.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnTabSchedules.BackColor = Color.White;
        btnTabSchedules.ForeColor = Color.FromArgb(15, 23, 42);
        btnTabSchedules.Font = new Font("Segoe UI", 9.5f);
        btnTabSchedules.Cursor = Cursors.Hand;

        btnTabShifts.Text = "Ca học thực tế";
        btnTabShifts.Location = new Point(340, 8);
        btnTabShifts.Size = new Size(160, 34);
        btnTabShifts.FlatStyle = FlatStyle.Flat;
        btnTabShifts.FlatAppearance.BorderSize = 1;
        btnTabShifts.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnTabShifts.BackColor = Color.White;
        btnTabShifts.ForeColor = Color.FromArgb(15, 23, 42);
        btnTabShifts.Font = new Font("Segoe UI", 9.5f);
        btnTabShifts.Cursor = Cursors.Hand;

        pnlTabBar.Controls.Add(btnTabRooms);
        pnlTabBar.Controls.Add(btnTabSchedules);
        pnlTabBar.Controls.Add(btnTabShifts);

        // 3. TOOLBAR
        pnlToolbar.Dock = DockStyle.Top;
        pnlToolbar.Height = 48;
        pnlToolbar.Padding = new Padding(0, 4, 0, 8);

        txtSearch.Font = new Font("Segoe UI", 9.5f);
        txtSearch.Width = 240;
        txtSearch.Location = new Point(0, 4);
        txtSearch.PlaceholderText = "Tìm kiếm...";

        cboFilter.Font = new Font("Segoe UI", 9.5f);
        cboFilter.DropDownStyle = ComboBoxStyle.DropDownList;
        cboFilter.Width = 180;
        cboFilter.Location = new Point(250, 4);

        btnSearch.Text = "Tìm kiếm";
        btnSearch.Size = new Size(95, 32);
        btnSearch.FlatStyle = FlatStyle.Flat;
        btnSearch.FlatAppearance.BorderSize = 0;
        btnSearch.BackColor = Color.FromArgb(30, 86, 240);
        btnSearch.ForeColor = Color.White;
        btnSearch.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnSearch.Cursor = Cursors.Hand;
        btnSearch.Location = new Point(440, 3);

        btnRefresh.Text = "Làm mới";
        btnRefresh.Size = new Size(105, 32);
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.FlatAppearance.BorderSize = 1;
        btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnRefresh.BackColor = Color.White;
        btnRefresh.ForeColor = Color.FromArgb(15, 23, 42);
        btnRefresh.Font = new Font("Segoe UI", 9.5f);
        btnRefresh.Cursor = Cursors.Hand;
        btnRefresh.Location = new Point(545, 3);

        btnAdd.Text = "Thêm phòng học";
        btnAdd.Size = new Size(150, 32);
        btnAdd.FlatStyle = FlatStyle.Flat;
        btnAdd.FlatAppearance.BorderSize = 0;
        btnAdd.BackColor = Color.FromArgb(30, 86, 240);
        btnAdd.ForeColor = Color.White;
        btnAdd.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnAdd.Cursor = Cursors.Hand;
        btnAdd.Location = new Point(660, 3);

        pnlToolbar.Controls.Add(btnAdd);
        pnlToolbar.Controls.Add(btnRefresh);
        pnlToolbar.Controls.Add(btnSearch);
        pnlToolbar.Controls.Add(cboFilter);
        pnlToolbar.Controls.Add(txtSearch);

        // 4. GRIDS
        pnlGridContainer.Dock = DockStyle.Fill;

        dgvRooms.Dock = DockStyle.Fill;
        dgvRooms.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaPhong", HeaderText = "Mã phòng", FillWeight = 85 });
        dgvRooms.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenPhong", HeaderText = "Tên phòng học", FillWeight = 160 });
        dgvRooms.Columns.Add(new DataGridViewTextBoxColumn { Name = "ViTri", HeaderText = "Vị trí", FillWeight = 140 });
        dgvRooms.Columns.Add(new DataGridViewTextBoxColumn { Name = "SucChua", HeaderText = "Sức chứa", FillWeight = 80 });
        dgvRooms.Columns.Add(new DataGridViewTextBoxColumn { Name = "ThietBi", HeaderText = "Trang thiết bị chuyên dụng", FillWeight = 240 });
        dgvRooms.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", FillWeight = 95 });

        dgvSchedules.Dock = DockStyle.Fill;
        dgvSchedules.Visible = false;
        dgvSchedules.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaLop", HeaderText = "Mã lớp", FillWeight = 85 });
        dgvSchedules.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenLop", HeaderText = "Lớp học", FillWeight = 160 });
        dgvSchedules.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenPhong", HeaderText = "Phòng học", FillWeight = 90 });
        dgvSchedules.Columns.Add(new DataGridViewTextBoxColumn { Name = "ThuText", HeaderText = "Thứ học trong tuần", FillWeight = 170 });
        dgvSchedules.Columns.Add(new DataGridViewTextBoxColumn { Name = "KhungGio", HeaderText = "Khung giờ học", FillWeight = 120 });
        dgvSchedules.Columns.Add(new DataGridViewTextBoxColumn { Name = "NgayApDungTu", HeaderText = "Ngày áp dụng", FillWeight = 90 });
        dgvSchedules.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", FillWeight = 95 });

        dgvShifts.Dock = DockStyle.Fill;
        dgvShifts.Visible = false;
        dgvShifts.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaCaHoc", HeaderText = "Mã ca", FillWeight = 60 });
        dgvShifts.Columns.Add(new DataGridViewTextBoxColumn { Name = "NgayHoc", HeaderText = "Ngày học", FillWeight = 85 });
        dgvShifts.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenLop", HeaderText = "Lớp học", FillWeight = 150 });
        dgvShifts.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenPhong", HeaderText = "Phòng học", FillWeight = 85 });
        dgvShifts.Columns.Add(new DataGridViewTextBoxColumn { Name = "GiaoVien", HeaderText = "Giáo viên giảng dạy", FillWeight = 130 });
        dgvShifts.Columns.Add(new DataGridViewTextBoxColumn { Name = "TroGiang", HeaderText = "Trợ giảng", FillWeight = 110 });
        dgvShifts.Columns.Add(new DataGridViewTextBoxColumn { Name = "GioHoc", HeaderText = "Thời gian", FillWeight = 110 });
        dgvShifts.Columns.Add(new DataGridViewTextBoxColumn { Name = "NoiDung", HeaderText = "Nội dung bài học", FillWeight = 200 });
        dgvShifts.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", FillWeight = 95 });

        pnlGridContainer.Controls.Add(dgvShifts);
        pnlGridContainer.Controls.Add(dgvSchedules);
        pnlGridContainer.Controls.Add(dgvRooms);

        // 5. FOOTER
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Height = 36;

        lblSummary.AutoSize = true;
        lblSummary.Font = new Font("Segoe UI", 9.5f);
        lblSummary.ForeColor = Color.FromArgb(100, 116, 139);
        lblSummary.Location = new Point(0, 8);

        pnlFooter.Controls.Add(lblSummary);

        // ADD ALL
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
        ((System.ComponentModel.ISupportInitialize)dgvRooms).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvSchedules).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvShifts).EndInit();
        pnlFooter.ResumeLayout(false);
        pnlFooter.PerformLayout();
        ResumeLayout(false);
    }
}
