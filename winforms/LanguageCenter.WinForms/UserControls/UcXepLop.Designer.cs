namespace LanguageCenter.WinForms.UserControls;

partial class UcXepLop
{
    private System.ComponentModel.IContainer components = null;

    private Panel pnlHeader = null!;
    private Label lblSelectClass = null!;
    private ComboBox cboTargetClass = null!;
    private Label lblClassDetails = null!;
    private Label lblCapacityBadge = null!;
    private Button btnRefresh = null!;

    private Panel pnlTabBar = null!;
    private Label lblTabGuide = null!;
    private Label lblTabHint = null!;
    private Button btnTabInClass = null!;
    private Button btnTabPending = null!;

    private Panel pnlToolbar = null!;
    private TextBox txtSearchInClass = null!;
    private Button btnOpenEnrollDialog = null!;
    private Button btnRemoveFromClass = null!;

    private TextBox txtSearchPending = null!;
    private ComboBox cboFilterCourse = null!;
    private Button btnEnrollSelectedPending = null!;
    private Button btnTuChoiDangKy = null!;

    private Panel pnlGridContainer = null!;
    private DataGridView dgvInClass = null!;
    private DataGridView dgvPending = null!;

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

        pnlHeader = new Panel();
        lblSelectClass = new Label();
        cboTargetClass = new ComboBox();
        lblClassDetails = new Label();
        lblCapacityBadge = new Label();
        btnRefresh = new Button();

        pnlTabBar = new Panel();
        lblTabGuide = new Label();
        lblTabHint = new Label();
        btnTabInClass = new Button();
        btnTabPending = new Button();

        pnlToolbar = new Panel();
        txtSearchInClass = new TextBox();
        btnOpenEnrollDialog = new Button();
        btnRemoveFromClass = new Button();

        txtSearchPending = new TextBox();
        cboFilterCourse = new ComboBox();
        btnEnrollSelectedPending = new Button();
        btnTuChoiDangKy = new Button();

        pnlGridContainer = new Panel();
        dgvInClass = new DataGridView();
        dgvPending = new DataGridView();

        pnlFooter = new Panel();
        lblSummary = new Label();

        pnlHeader.SuspendLayout();
        pnlTabBar.SuspendLayout();
        pnlToolbar.SuspendLayout();
        pnlGridContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvInClass).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvPending).BeginInit();
        pnlFooter.SuspendLayout();
        SuspendLayout();

        // UcXepLop
        Dock = DockStyle.Fill;
        BackColor = Color.White;

        // 1. TOP HEADER
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Height = 96;
        pnlHeader.BackColor = Color.White;
        pnlHeader.Padding = new Padding(20, 12, 20, 12);

        lblSelectClass.Text = "Lớp học mục tiêu:";
        lblSelectClass.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
        lblSelectClass.ForeColor = Color.FromArgb(15, 23, 42);
        lblSelectClass.AutoSize = true;
        lblSelectClass.Location = new Point(20, 16);

        cboTargetClass.Font = new Font("Segoe UI", 9.5f);
        cboTargetClass.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTargetClass.Width = 360;
        cboTargetClass.Location = new Point(165, 13);

        btnRefresh.Text = "Làm mới";
        btnRefresh.Size = new Size(110, 32);
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.FlatAppearance.BorderSize = 1;
        btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnRefresh.BackColor = Color.White;
        btnRefresh.ForeColor = Color.FromArgb(15, 23, 42);
        btnRefresh.Font = new Font("Segoe UI", 9.5f);
        btnRefresh.Cursor = Cursors.Hand;
        btnRefresh.Location = new Point(535, 12);

        lblCapacityBadge.AutoSize = true;
        lblCapacityBadge.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
        lblCapacityBadge.ForeColor = Color.FromArgb(30, 86, 240);
        lblCapacityBadge.Location = new Point(660, 18);

        lblClassDetails.AutoSize = true;
        lblClassDetails.Font = new Font("Segoe UI", 9.5f);
        lblClassDetails.ForeColor = Color.FromArgb(71, 85, 105);
        lblClassDetails.Location = new Point(20, 58);

        pnlHeader.Controls.Add(lblClassDetails);
        pnlHeader.Controls.Add(lblCapacityBadge);
        pnlHeader.Controls.Add(btnRefresh);
        pnlHeader.Controls.Add(cboTargetClass);
        pnlHeader.Controls.Add(lblSelectClass);

        // 2. TAB BAR
        pnlTabBar.Dock = DockStyle.Top;
        pnlTabBar.Height = 52;
        pnlTabBar.BackColor = Color.White;
        pnlTabBar.Padding = new Padding(20, 8, 20, 8);

        lblTabGuide.Visible = false;
        lblTabHint.Visible = false;

        btnTabInClass.Text = "Học viên trong lớp";
        btnTabInClass.Location = new Point(20, 8);
        btnTabInClass.Size = new Size(230, 36);
        btnTabInClass.FlatStyle = FlatStyle.Flat;
        btnTabInClass.FlatAppearance.BorderSize = 1;
        btnTabInClass.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnTabInClass.BackColor = Color.White;
        btnTabInClass.ForeColor = Color.FromArgb(15, 23, 42);
        btnTabInClass.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnTabInClass.Cursor = Cursors.Hand;

        btnTabPending.Text = "Học viên chờ xếp lớp";
        btnTabPending.Location = new Point(265, 8);
        btnTabPending.Size = new Size(250, 36);
        btnTabPending.FlatStyle = FlatStyle.Flat;
        btnTabPending.FlatAppearance.BorderSize = 1;
        btnTabPending.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
        btnTabPending.BackColor = Color.White;
        btnTabPending.ForeColor = Color.FromArgb(15, 23, 42);
        btnTabPending.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnTabPending.Cursor = Cursors.Hand;

        pnlTabBar.Controls.Add(btnTabPending);
        pnlTabBar.Controls.Add(btnTabInClass);

        // 3. TOOLBAR
        pnlToolbar.Dock = DockStyle.Top;
        pnlToolbar.Height = 52;
        pnlToolbar.BackColor = Color.White;
        pnlToolbar.Padding = new Padding(20, 9, 20, 9);

        // Tab 0 Controls
        txtSearchInClass.Font = new Font("Segoe UI", 9.5f);
        txtSearchInClass.Width = 280;
        txtSearchInClass.Location = new Point(20, 10);
        txtSearchInClass.PlaceholderText = "Tìm kiếm học viên trong lớp...";

        btnOpenEnrollDialog.Text = "+ Thêm học viên vào lớp";
        btnOpenEnrollDialog.Size = new Size(195, 34);
        btnOpenEnrollDialog.FlatStyle = FlatStyle.Flat;
        btnOpenEnrollDialog.FlatAppearance.BorderSize = 0;
        btnOpenEnrollDialog.BackColor = Color.FromArgb(30, 86, 240);
        btnOpenEnrollDialog.ForeColor = Color.White;
        btnOpenEnrollDialog.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnOpenEnrollDialog.Cursor = Cursors.Hand;
        btnOpenEnrollDialog.Location = new Point(780, 9);

        btnRemoveFromClass.Text = "Rút khỏi lớp";
        btnRemoveFromClass.Size = new Size(120, 34);
        btnRemoveFromClass.FlatStyle = FlatStyle.Flat;
        btnRemoveFromClass.FlatAppearance.BorderSize = 1;
        btnRemoveFromClass.FlatAppearance.BorderColor = Color.FromArgb(252, 165, 165);
        btnRemoveFromClass.BackColor = Color.White;
        btnRemoveFromClass.ForeColor = Color.FromArgb(239, 68, 68);
        btnRemoveFromClass.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnRemoveFromClass.Cursor = Cursors.Hand;
        btnRemoveFromClass.Location = new Point(985, 9);

        // Tab 1 Controls
        txtSearchPending.Font = new Font("Segoe UI", 9.5f);
        txtSearchPending.Width = 280;
        txtSearchPending.Location = new Point(20, 10);
        txtSearchPending.PlaceholderText = "Tìm học viên chờ xếp lớp...";

        cboFilterCourse.Font = new Font("Segoe UI", 9.5f);
        cboFilterCourse.DropDownStyle = ComboBoxStyle.DropDownList;
        cboFilterCourse.Width = 240;
        cboFilterCourse.Location = new Point(315, 10);

        btnEnrollSelectedPending.Text = "Xếp vào lớp mục tiêu";
        btnEnrollSelectedPending.Size = new Size(185, 34);
        btnEnrollSelectedPending.FlatStyle = FlatStyle.Flat;
        btnEnrollSelectedPending.FlatAppearance.BorderSize = 0;
        btnEnrollSelectedPending.BackColor = Color.FromArgb(30, 86, 240);
        btnEnrollSelectedPending.ForeColor = Color.White;
        btnEnrollSelectedPending.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnEnrollSelectedPending.Cursor = Cursors.Hand;
        btnEnrollSelectedPending.Location = new Point(920, 9);

        btnTuChoiDangKy.Text = "Từ chối đăng ký";
        btnTuChoiDangKy.Size = new Size(150, 34);
        btnTuChoiDangKy.FlatStyle = FlatStyle.Flat;
        btnTuChoiDangKy.FlatAppearance.BorderSize = 1;
        btnTuChoiDangKy.FlatAppearance.BorderColor = Color.FromArgb(252, 165, 165);
        btnTuChoiDangKy.BackColor = Color.White;
        btnTuChoiDangKy.ForeColor = Color.FromArgb(239, 68, 68);
        btnTuChoiDangKy.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnTuChoiDangKy.Cursor = Cursors.Hand;
        btnTuChoiDangKy.Location = new Point(720, 9);

        pnlToolbar.Controls.Add(txtSearchInClass);
        pnlToolbar.Controls.Add(btnOpenEnrollDialog);
        pnlToolbar.Controls.Add(btnRemoveFromClass);
        pnlToolbar.Controls.Add(txtSearchPending);
        pnlToolbar.Controls.Add(cboFilterCourse);
        pnlToolbar.Controls.Add(btnEnrollSelectedPending);
        pnlToolbar.Controls.Add(btnTuChoiDangKy);

        // 4. GRID CONTAINER
        pnlGridContainer.Dock = DockStyle.Fill;
        pnlGridContainer.BackColor = Color.White;

        // Bảng học viên trong lớp
        dgvInClass.Dock = DockStyle.Fill;
        dgvInClass.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaHocVien", HeaderText = "Mã HV", FillWeight = 80 });
        dgvInClass.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenHocVien", HeaderText = "Họ và tên học viên", FillWeight = 160 });
        dgvInClass.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoDienThoai", HeaderText = "SĐT", FillWeight = 100 });
        dgvInClass.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", FillWeight = 150 });
        dgvInClass.Columns.Add(new DataGridViewTextBoxColumn { Name = "NgayXepLop", HeaderText = "Ngày xếp lớp", FillWeight = 100 });
        dgvInClass.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThaiHocPhi", HeaderText = "Học phí", FillWeight = 110 });

        // Bảng học viên chờ xếp lớp
        dgvPending.Dock = DockStyle.Fill;
        dgvPending.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaDangKy", HeaderText = "Mã ĐK", FillWeight = 60 });
        dgvPending.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaHocVien", HeaderText = "Mã HV", FillWeight = 80 });
        dgvPending.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenHocVien", HeaderText = "Họ và tên học viên", FillWeight = 160 });
        dgvPending.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoDienThoai", HeaderText = "SĐT", FillWeight = 100 });
        dgvPending.Columns.Add(new DataGridViewTextBoxColumn { Name = "KhoaHoc", HeaderText = "Khóa học đăng ký", FillWeight = 160 });
        dgvPending.Columns.Add(new DataGridViewTextBoxColumn { Name = "NgayDangKy", HeaderText = "Ngày đăng ký", FillWeight = 100 });
        dgvPending.Columns.Add(new DataGridViewTextBoxColumn { Name = "HocPhi", HeaderText = "Tình trạng học phí", FillWeight = 110 });
        dgvPending.Columns.Add(new DataGridViewTextBoxColumn { Name = "GhiChu", HeaderText = "Ghi chú xếp lớp", FillWeight = 160 });

        pnlGridContainer.Controls.Add(dgvInClass);
        pnlGridContainer.Controls.Add(dgvPending);

        // 5. FOOTER
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Height = 36;
        pnlFooter.BackColor = Color.FromArgb(248, 250, 252);
        pnlFooter.Padding = new Padding(16, 8, 16, 8);

        lblSummary.Dock = DockStyle.Fill;
        lblSummary.Font = new Font("Segoe UI", 9.5f);
        lblSummary.ForeColor = Color.FromArgb(71, 85, 105);

        pnlFooter.Controls.Add(lblSummary);

        Controls.Add(pnlGridContainer);
        Controls.Add(pnlToolbar);
        Controls.Add(pnlTabBar);
        Controls.Add(pnlHeader);
        Controls.Add(pnlFooter);

        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        pnlTabBar.ResumeLayout(false);
        pnlTabBar.PerformLayout();
        pnlToolbar.ResumeLayout(false);
        pnlToolbar.PerformLayout();
        pnlGridContainer.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvInClass).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvPending).EndInit();
        pnlFooter.ResumeLayout(false);
        ResumeLayout(false);
    }
}
