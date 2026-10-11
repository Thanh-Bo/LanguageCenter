namespace LanguageCenter.WinForms.UserControls;

partial class UcHocPhi
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel tblStats = null!;
    private Panel pnlToolbar = null!;
    private TextBox txtSearch = null!;
    private ComboBox cboTrangThai = null!;
    private ComboBox cboPhuongThuc = null!;
    private Button btnSearch = null!;
    private Button btnRefresh = null!;
    private Button btnCollectFee = null!;
    private Button btnPrintReceipt = null!;
    private DataGridView dgvTuition = null!;
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

        tblStats = new TableLayoutPanel();
        pnlToolbar = new Panel();
        txtSearch = UI.UiHelpers.CreateTextBox(220);
        cboTrangThai = UI.UiHelpers.CreateComboBox(160);
        cboPhuongThuc = UI.UiHelpers.CreateComboBox(160);
        btnSearch = UI.UiHelpers.CreatePrimaryButton("Tìm kiếm", 90, 32);
        btnRefresh = UI.UiHelpers.CreateSecondaryButton("Làm mới", 80, 32);
        btnCollectFee = UI.UiHelpers.CreatePrimaryButton("Thu học phí", 120, 32);
        btnPrintReceipt = UI.UiHelpers.CreateSecondaryButton("In biên lai", 100, 32);
        dgvTuition = new DataGridView();
        pnlFooter = new Panel();
        lblSummary = new Label();

        SuspendLayout();

        Dock = DockStyle.Fill;
        BackColor = Color.White;

        // tblStats (4 cards)
        tblStats.Dock = DockStyle.Top;
        tblStats.Height = 84;
        tblStats.ColumnCount = 4;
        tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tblStats.RowCount = 1;
        tblStats.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tblStats.Margin = new Padding(0);
        tblStats.Padding = new Padding(0, 0, 0, 10);

        // pnlToolbar
        pnlToolbar.Dock = DockStyle.Top;
        pnlToolbar.Height = 52;
        pnlToolbar.Padding = new Padding(0, 6, 0, 10);

        txtSearch.Location = new Point(0, 8);
        txtSearch.PlaceholderText = "Tìm mã HP, mã HV, họ tên, lớp...";
        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; FilterData(); } };

        cboTrangThai.Location = new Point(230, 8);
        cboTrangThai.SelectedIndexChanged += (_, _) => FilterData();

        cboPhuongThuc.Location = new Point(400, 8);
        cboPhuongThuc.SelectedIndexChanged += (_, _) => FilterData();

        btnSearch.Location = new Point(570, 7);
        btnSearch.Click += (_, _) => FilterData();

        btnRefresh.Location = new Point(668, 7);
        btnRefresh.Click += (_, _) => ReloadData();

        btnPrintReceipt.Location = new Point(718, 7);
        btnPrintReceipt.Click += BtnPrintReceipt_Click;

        btnCollectFee.Location = new Point(848, 7);
        btnCollectFee.Click += BtnCollectFee_Click;

        pnlToolbar.Resize += (_, _) =>
        {
            btnCollectFee.Location = new Point(pnlToolbar.Width - btnCollectFee.Width - 4, 7);
            btnPrintReceipt.Location = new Point(btnCollectFee.Left - btnPrintReceipt.Width - 8, 7);
        };

        pnlToolbar.Controls.Add(btnCollectFee);
        pnlToolbar.Controls.Add(btnPrintReceipt);
        pnlToolbar.Controls.Add(btnRefresh);
        pnlToolbar.Controls.Add(btnSearch);
        pnlToolbar.Controls.Add(cboPhuongThuc);
        pnlToolbar.Controls.Add(cboTrangThai);
        pnlToolbar.Controls.Add(txtSearch);

        // dgvTuition
        dgvTuition.Dock = DockStyle.Fill;
        UI.UiHelpers.StyleGrid(dgvTuition);
        dgvTuition.CellDoubleClick += DgvTuition_CellDoubleClick;

        // Columns matching HocPhi / ThanhToan / BienNhan DB tables
        dgvTuition.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaHocPhi", HeaderText = "Mã HP", Width = 90 });
        dgvTuition.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaHocVien", HeaderText = "Mã HV", Width = 80 });
        dgvTuition.Columns.Add(new DataGridViewTextBoxColumn { Name = "HoTenHocVien", HeaderText = "Họ tên học viên", Width = 150 });
        dgvTuition.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenKhoaLop", HeaderText = "Khóa học / Lớp học", Width = 210 });
        dgvTuition.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoTienGoc", HeaderText = "Học phí gốc", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
        dgvTuition.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoTienGiam", HeaderText = "Giảm trừ", Width = 95, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
        dgvTuition.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoTienPhaiDong", HeaderText = "Phải đóng", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
        dgvTuition.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoTienDaDong", HeaderText = "Đã thu", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
        dgvTuition.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoTienConThieu", HeaderText = "Còn nợ", Width = 110, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
        dgvTuition.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThaiHocPhi", HeaderText = "Trạng thái", Width = 135 });
        dgvTuition.Columns.Add(new DataGridViewTextBoxColumn { Name = "PhuongThuc", HeaderText = "Phương thức", Width = 130 });
        dgvTuition.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoBienNhan", HeaderText = "Số biên lai", Width = 110 });
        dgvTuition.Columns.Add(new DataGridViewTextBoxColumn { Name = "HanThanhToan", HeaderText = "Hạn nộp", Width = 95 });

        // pnlFooter
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Height = 36;
        lblSummary.AutoSize = true;
        lblSummary.Font = UI.Theme.FontRegular;
        lblSummary.ForeColor = UI.Theme.Muted;
        lblSummary.Location = new Point(4, 8);
        pnlFooter.Controls.Add(lblSummary);

        Controls.Add(dgvTuition);
        Controls.Add(pnlFooter);
        Controls.Add(pnlToolbar);
        Controls.Add(tblStats);

        ResumeLayout(false);
    }
}
