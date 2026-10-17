namespace LanguageCenter.WinForms.UserControls;

partial class UcHocPhi
{
    private System.ComponentModel.IContainer components = null;

    private TableLayoutPanel tblStats = null!;
    private Panel pnlTabBar = null!;
    private Button btnTabTuition = null!;
    private Button btnTabPayments = null!;
    private Button btnTabReceipts = null!;
    private Panel pnlToolbar = null!;
    private TextBox txtSearch = null!;
    private ComboBox cboTrangThai = null!;
    private ComboBox cboPhuongThuc = null!;
    private Button btnSearch = null!;
    private Button btnRefresh = null!;
    private Button btnCollectFee = null!;
    private Button btnPrintReceipt = null!;
    private Panel pnlGridContainer = null!;
    private DataGridView dgvTuition = null!;
    private DataGridView dgvPayments = null!;
    private DataGridView dgvReceipts = null!;
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
        pnlTabBar = new Panel();
        btnTabTuition = new Button();
        btnTabPayments = new Button();
        btnTabReceipts = new Button();
        pnlToolbar = new Panel();
        txtSearch = new TextBox();
        cboTrangThai = new ComboBox();
        cboPhuongThuc = new ComboBox();
        btnSearch = new Button();
        btnRefresh = new Button();
        btnCollectFee = new Button();
        btnPrintReceipt = new Button();
        pnlGridContainer = new Panel();
        dgvTuition = new DataGridView();
        dgvPayments = new DataGridView();
        dgvReceipts = new DataGridView();
        pnlFooter = new Panel();
        lblSummary = new Label();

        pnlTabBar.SuspendLayout();
        pnlToolbar.SuspendLayout();
        pnlGridContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvTuition).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvPayments).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvReceipts).BeginInit();
        pnlFooter.SuspendLayout();
        SuspendLayout();

        // UcHocPhi
        Dock = DockStyle.Fill;
        BackColor = Color.White;

        // 1. STATS BANNER
        tblStats.Dock = DockStyle.Top;
        tblStats.Height = 118;
        tblStats.ColumnCount = 4;
        tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tblStats.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        tblStats.RowCount = 1;
        tblStats.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        // 2. SUB-TAB BAR
        pnlTabBar.Dock = DockStyle.Top;
        pnlTabBar.Height = 48;
        pnlTabBar.BackColor = Color.White;

        btnTabTuition.Text = "Sổ học phí học viên";
        btnTabTuition.Location = new Point(0, 8);
        btnTabTuition.Size = new Size(170, 34);
        btnTabTuition.FlatStyle = FlatStyle.Flat;
        btnTabTuition.FlatAppearance.BorderSize = 1;
        btnTabTuition.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnTabTuition.BackColor = Color.White;
        btnTabTuition.ForeColor = Color.FromArgb(15, 23, 42);
        btnTabTuition.Font = new Font("Segoe UI", 9.5f);
        btnTabTuition.Cursor = Cursors.Hand;

        btnTabPayments.Text = "Lịch sử thanh toán";
        btnTabPayments.Location = new Point(180, 8);
        btnTabPayments.Size = new Size(170, 34);
        btnTabPayments.FlatStyle = FlatStyle.Flat;
        btnTabPayments.FlatAppearance.BorderSize = 1;
        btnTabPayments.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnTabPayments.BackColor = Color.White;
        btnTabPayments.ForeColor = Color.FromArgb(15, 23, 42);
        btnTabPayments.Font = new Font("Segoe UI", 9.5f);
        btnTabPayments.Cursor = Cursors.Hand;

        btnTabReceipts.Text = "Quản lý biên nhận";
        btnTabReceipts.Location = new Point(360, 8);
        btnTabReceipts.Size = new Size(170, 34);
        btnTabReceipts.FlatStyle = FlatStyle.Flat;
        btnTabReceipts.FlatAppearance.BorderSize = 1;
        btnTabReceipts.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnTabReceipts.BackColor = Color.White;
        btnTabReceipts.ForeColor = Color.FromArgb(15, 23, 42);
        btnTabReceipts.Font = new Font("Segoe UI", 9.5f);
        btnTabReceipts.Cursor = Cursors.Hand;

        pnlTabBar.Controls.Add(btnTabTuition);
        pnlTabBar.Controls.Add(btnTabPayments);
        pnlTabBar.Controls.Add(btnTabReceipts);

        // 3. TOOLBAR
        pnlToolbar.Dock = DockStyle.Top;
        pnlToolbar.Height = 48;
        pnlToolbar.Padding = new Padding(0, 4, 0, 8);

        txtSearch.Font = new Font("Segoe UI", 9.5f);
        txtSearch.Width = 220;
        txtSearch.Location = new Point(0, 4);
        txtSearch.PlaceholderText = "Tìm theo mã, tên học viên...";

        cboTrangThai.Font = new Font("Segoe UI", 9.5f);
        cboTrangThai.DropDownStyle = ComboBoxStyle.DropDownList;
        cboTrangThai.Width = 160;
        cboTrangThai.Location = new Point(230, 4);

        cboPhuongThuc.Font = new Font("Segoe UI", 9.5f);
        cboPhuongThuc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPhuongThuc.Width = 160;
        cboPhuongThuc.Location = new Point(400, 4);

        btnSearch.Text = "Tìm kiếm";
        btnSearch.Size = new Size(90, 32);
        btnSearch.FlatStyle = FlatStyle.Flat;
        btnSearch.FlatAppearance.BorderSize = 0;
        btnSearch.BackColor = Color.FromArgb(30, 86, 240);
        btnSearch.ForeColor = Color.White;
        btnSearch.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnSearch.Cursor = Cursors.Hand;
        btnSearch.Location = new Point(570, 3);

        btnRefresh.Text = "Làm mới";
        btnRefresh.Size = new Size(105, 32);
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.FlatAppearance.BorderSize = 1;
        btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnRefresh.BackColor = Color.White;
        btnRefresh.ForeColor = Color.FromArgb(15, 23, 42);
        btnRefresh.Font = new Font("Segoe UI", 9.5f);
        btnRefresh.Cursor = Cursors.Hand;
        btnRefresh.Location = new Point(668, 3);

        btnCollectFee.Text = "Thu học phí";
        btnCollectFee.Size = new Size(120, 32);
        btnCollectFee.FlatStyle = FlatStyle.Flat;
        btnCollectFee.FlatAppearance.BorderSize = 0;
        btnCollectFee.BackColor = Color.FromArgb(30, 86, 240);
        btnCollectFee.ForeColor = Color.White;
        btnCollectFee.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnCollectFee.Cursor = Cursors.Hand;
        btnCollectFee.Location = new Point(781, 3);

        btnPrintReceipt.Text = "In biên lai";
        btnPrintReceipt.Size = new Size(145, 32);
        btnPrintReceipt.FlatStyle = FlatStyle.Flat;
        btnPrintReceipt.FlatAppearance.BorderSize = 1;
        btnPrintReceipt.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnPrintReceipt.BackColor = Color.White;
        btnPrintReceipt.ForeColor = Color.FromArgb(15, 23, 42);
        btnPrintReceipt.Font = new Font("Segoe UI", 9.5f);
        btnPrintReceipt.Cursor = Cursors.Hand;
        btnPrintReceipt.Location = new Point(909, 3);

        pnlToolbar.Controls.Add(btnPrintReceipt);
        pnlToolbar.Controls.Add(btnCollectFee);
        pnlToolbar.Controls.Add(btnRefresh);
        pnlToolbar.Controls.Add(btnSearch);
        pnlToolbar.Controls.Add(cboPhuongThuc);
        pnlToolbar.Controls.Add(cboTrangThai);
        pnlToolbar.Controls.Add(txtSearch);

        // 4. GRIDS
        pnlGridContainer.Dock = DockStyle.Fill;

        dgvTuition.Dock = DockStyle.Fill;
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

        dgvPayments.Dock = DockStyle.Fill;
        dgvPayments.Visible = false;
        dgvPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaThanhToan", HeaderText = "Mã GD", FillWeight = 60 });
        dgvPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaHocVien", HeaderText = "Mã HV", FillWeight = 75 });
        dgvPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenHocVien", HeaderText = "Họ tên học viên", FillWeight = 150 });
        dgvPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoTien", HeaderText = "Số tiền thanh toán", FillWeight = 110, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
        dgvPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "NgayThanhToan", HeaderText = "Thời gian GD", FillWeight = 110 });
        dgvPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "PhuongThuc", HeaderText = "Hình thức nộp", FillWeight = 120 });
        dgvPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaGiaoDich", HeaderText = "Mã giao dịch ngoài", FillWeight = 100 });
        dgvPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "KeToanThu", HeaderText = "Kế toán thu", FillWeight = 85 });
        dgvPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", FillWeight = 85 });
        dgvPayments.Columns.Add(new DataGridViewTextBoxColumn { Name = "GhiChu", HeaderText = "Ghi chú thu tiền", FillWeight = 160 });

        dgvReceipts.Dock = DockStyle.Fill;
        dgvReceipts.Visible = false;
        dgvReceipts.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoBienNhan", HeaderText = "Số biên nhận", FillWeight = 100 });
        dgvReceipts.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaHocVien", HeaderText = "Mã HV", FillWeight = 75 });
        dgvReceipts.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenHocVien", HeaderText = "Họ tên học viên", FillWeight = 150 });
        dgvReceipts.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenKhoaHoc", HeaderText = "Nội dung khóa học", FillWeight = 170 });
        dgvReceipts.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoTien", HeaderText = "Số tiền xác nhận", FillWeight = 110, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
        dgvReceipts.Columns.Add(new DataGridViewTextBoxColumn { Name = "PhuongThuc", HeaderText = "Phương thức", FillWeight = 100 });
        dgvReceipts.Columns.Add(new DataGridViewTextBoxColumn { Name = "NguoiLap", HeaderText = "Người lập phiếu", FillWeight = 90 });
        dgvReceipts.Columns.Add(new DataGridViewTextBoxColumn { Name = "NgayLap", HeaderText = "Ngày lập phiếu", FillWeight = 100 });
        dgvReceipts.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", FillWeight = 85 });

        pnlGridContainer.Controls.Add(dgvReceipts);
        pnlGridContainer.Controls.Add(dgvPayments);
        pnlGridContainer.Controls.Add(dgvTuition);

        // 5. FOOTER
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Height = 36;
        lblSummary.AutoSize = true;
        lblSummary.Font = new Font("Segoe UI", 9.5f);
        lblSummary.ForeColor = Color.FromArgb(100, 116, 139);
        lblSummary.Location = new Point(4, 8);
        pnlFooter.Controls.Add(lblSummary);

        Controls.Add(pnlGridContainer);
        Controls.Add(pnlFooter);
        Controls.Add(pnlToolbar);
        Controls.Add(pnlTabBar);
        Controls.Add(tblStats);

        pnlTabBar.ResumeLayout(false);
        pnlToolbar.ResumeLayout(false);
        pnlToolbar.PerformLayout();
        pnlGridContainer.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvTuition).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvPayments).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvReceipts).EndInit();
        pnlFooter.ResumeLayout(false);
        pnlFooter.PerformLayout();
        ResumeLayout(false);
    }
}
