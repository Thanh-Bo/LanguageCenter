namespace LanguageCenter.WinForms.UserControls;

partial class UcCourses
{
    private System.ComponentModel.IContainer components = null;

    private Panel pnlHeaderStats = null!;
    private TableLayoutPanel tblStats = null!;
    private Panel pnlToolbar = null!;
    private TextBox txtSearch = null!;
    private ComboBox cboChuongTrinh = null!;
    private ComboBox cboTrangThai = null!;
    private Button btnSearch = null!;
    private Button btnRefresh = null!;
    private Button btnAdd = null!;
    private DataGridView dgvCourses = null!;
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
        txtSearch = UI.UiHelpers.CreateTextBox(240);
        cboChuongTrinh = UI.UiHelpers.CreateComboBox(180);
        cboTrangThai = UI.UiHelpers.CreateComboBox(160);
        btnSearch = UI.UiHelpers.CreatePrimaryButton("Tìm kiếm", 100, 32);
        btnRefresh = UI.UiHelpers.CreateSecondaryButton("Làm mới", 80, 32);
        btnAdd = UI.UiHelpers.CreatePrimaryButton("+ Thêm khóa học", 160, 32);
        dgvCourses = new DataGridView();
        pnlFooter = new Panel();
        lblSummary = new Label();

        SuspendLayout();

        Dock = DockStyle.Fill;
        BackColor = Color.White;

        // 1. STATS BANNER
        pnlHeaderStats.Dock = DockStyle.Top;
        pnlHeaderStats.Height = 88;
        pnlHeaderStats.Padding = new Padding(0, 0, 0, 12);

        tblStats.Dock = DockStyle.Fill;
        tblStats.ColumnCount = 4;
        tblStats.RowCount = 1;
        tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));

        pnlHeaderStats.Controls.Add(tblStats);

        // 2. TOOLBAR
        pnlToolbar.Dock = DockStyle.Top;
        pnlToolbar.Height = 52;
        pnlToolbar.Padding = new Padding(0, 0, 0, 12);

        txtSearch.Location = new Point(0, 2);
        txtSearch.PlaceholderText = "Tìm theo mã, tên khóa học...";
        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; FilterData(); } };

        cboChuongTrinh.Location = new Point(250, 2);
        cboChuongTrinh.SelectedIndexChanged += (_, _) => FilterData();

        cboTrangThai.Location = new Point(440, 2);
        cboTrangThai.SelectedIndexChanged += (_, _) => FilterData();

        btnSearch.Location = new Point(610, 1);
        btnSearch.Click += (_, _) => FilterData();

        btnRefresh.Location = new Point(718, 1);
        btnRefresh.Click += (_, _) => ReloadData();

        btnAdd.Location = new Point(806, 1);
        btnAdd.Click += BtnAdd_Click;

        pnlToolbar.Resize += (_, _) =>
        {
            btnAdd.Location = new Point(pnlToolbar.Width - btnAdd.Width - 4, 1);
        };

        pnlToolbar.Controls.Add(btnAdd);
        pnlToolbar.Controls.Add(btnRefresh);
        pnlToolbar.Controls.Add(btnSearch);
        pnlToolbar.Controls.Add(cboTrangThai);
        pnlToolbar.Controls.Add(cboChuongTrinh);
        pnlToolbar.Controls.Add(txtSearch);

        // 3. DATAGRIDVIEW
        dgvCourses.Dock = DockStyle.Fill;
        UI.UiHelpers.StyleGrid(dgvCourses);
        dgvCourses.CellDoubleClick += DgvCourses_CellDoubleClick;

        dgvCourses.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaKhoaHoc", HeaderText = "Mã khóa", FillWeight = 85 });
        dgvCourses.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenKhoaHoc", HeaderText = "Tên khóa học", FillWeight = 190 });
        dgvCourses.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenChuongTrinh", HeaderText = "Chương trình", FillWeight = 130 });
        dgvCourses.Columns.Add(new DataGridViewTextBoxColumn { Name = "CapDo", HeaderText = "Cấp độ", FillWeight = 110 });
        dgvCourses.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoBuoi", HeaderText = "Số buổi", FillWeight = 70 });
        dgvCourses.Columns.Add(new DataGridViewTextBoxColumn { Name = "TongSoGio", HeaderText = "Thời lượng", FillWeight = 80 });
        dgvCourses.Columns.Add(new DataGridViewTextBoxColumn { Name = "HocPhiCoBan", HeaderText = "Học phí niêm yết", FillWeight = 110 });
        dgvCourses.Columns.Add(new DataGridViewTextBoxColumn { Name = "NgayBatDau", HeaderText = "Khai giảng", FillWeight = 85 });
        dgvCourses.Columns.Add(new DataGridViewTextBoxColumn { Name = "SiSo", HeaderText = "Sĩ số", FillWeight = 65 });
        dgvCourses.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", FillWeight = 95 });

        // 4. FOOTER
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Height = 38;

        lblSummary.AutoSize = true;
        lblSummary.Font = UI.Theme.FontRegular;
        lblSummary.ForeColor = UI.Theme.Muted;
        lblSummary.Location = new Point(0, 10);

        pnlFooter.Controls.Add(lblSummary);

        // CONTROLS ADD ORDER
        Controls.Add(dgvCourses);
        Controls.Add(pnlFooter);
        Controls.Add(pnlToolbar);
        Controls.Add(pnlHeaderStats);

        ResumeLayout(false);
    }
}

