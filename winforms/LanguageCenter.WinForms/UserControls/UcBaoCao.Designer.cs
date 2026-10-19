namespace LanguageCenter.WinForms.UserControls;

partial class UcBaoCao
{
    private System.ComponentModel.IContainer components = null;

    private Panel pnlHeaderStats = null!;
    private TableLayoutPanel tblStats = null!;

    // Main Body Layout (Menu dọc bên trái & Khung nội dung bên phải)
    private Panel pnlBody = null!;
    private Panel pnlSideMenu = null!;
    private Label lblSideMenuTitle = null!;
    private Button btnMenuRevenue = null!;
    private Button btnMenuQuality = null!;
    private Button btnMenuFinancial = null!;
    private Button btnMenuClasses = null!;

    private Panel pnlContentArea = null!;
    private Panel pnlToolbar = null!;
    private Label lblCurrentReportTitle = null!;
    private Label lblPeriod = null!;
    private ComboBox cboPeriod = null!;
    private Button btnRefresh = null!;
    private Button btnExportExcel = null!;
    private Button btnPrint = null!;

    private Panel pnlGridContainer = null!;
    private DataGridView dgvRevenue = null!;
    private DataGridView dgvQuality = null!;
    private DataGridView dgvFinancial = null!;
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

        pnlBody = new Panel();
        pnlSideMenu = new Panel();
        lblSideMenuTitle = new Label();
        btnMenuRevenue = new Button();
        btnMenuQuality = new Button();
        btnMenuFinancial = new Button();
        btnMenuClasses = new Button();

        pnlContentArea = new Panel();
        pnlToolbar = new Panel();
        lblCurrentReportTitle = new Label();
        lblPeriod = new Label();
        cboPeriod = new ComboBox();
        btnRefresh = new Button();
        btnExportExcel = new Button();
        btnPrint = new Button();

        pnlGridContainer = new Panel();
        dgvRevenue = new DataGridView();
        dgvQuality = new DataGridView();
        dgvFinancial = new DataGridView();
        dgvClasses = new DataGridView();

        pnlFooter = new Panel();
        lblSummary = new Label();

        pnlHeaderStats.SuspendLayout();
        pnlBody.SuspendLayout();
        pnlSideMenu.SuspendLayout();
        pnlContentArea.SuspendLayout();
        pnlToolbar.SuspendLayout();
        pnlGridContainer.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvRevenue).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvQuality).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvFinancial).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvClasses).BeginInit();
        pnlFooter.SuspendLayout();
        SuspendLayout();

        // UcBaoCao
        Dock = DockStyle.Fill;
        BackColor = Color.White;

        // 1. STATS BANNER TRÊN CÙNG
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

        // 2. MAIN BODY
        pnlBody.Dock = DockStyle.Fill;
        pnlBody.BackColor = Color.White;

        // 2A. MENU DỌC BÊN TRÁI
        pnlSideMenu.Dock = DockStyle.Left;
        pnlSideMenu.Width = 230;
        pnlSideMenu.BackColor = Color.FromArgb(248, 250, 252);
        pnlSideMenu.Padding = new Padding(12, 10, 12, 10);

        lblSideMenuTitle.Text = "DANH MỤC BÁO CÁO";
        lblSideMenuTitle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);
        lblSideMenuTitle.ForeColor = Color.FromArgb(71, 85, 105);
        lblSideMenuTitle.AutoSize = true;
        lblSideMenuTitle.Location = new Point(14, 12);

        // Button Revenue
        btnMenuRevenue.Text = "Doanh thu đào tạo";
        btnMenuRevenue.Location = new Point(10, 38);
        btnMenuRevenue.Size = new Size(208, 44);
        btnMenuRevenue.FlatStyle = FlatStyle.Flat;
        btnMenuRevenue.FlatAppearance.BorderSize = 1;
        btnMenuRevenue.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnMenuRevenue.TextAlign = ContentAlignment.MiddleLeft;
        btnMenuRevenue.Padding = new Padding(14, 0, 0, 0);
        btnMenuRevenue.Cursor = Cursors.Hand;
        btnMenuRevenue.Font = new Font("Segoe UI", 9.5f);

        // Button Quality
        btnMenuQuality.Text = "Chất lượng đầu ra";
        btnMenuQuality.Location = new Point(10, 88);
        btnMenuQuality.Size = new Size(208, 44);
        btnMenuQuality.FlatStyle = FlatStyle.Flat;
        btnMenuQuality.FlatAppearance.BorderSize = 1;
        btnMenuQuality.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnMenuQuality.TextAlign = ContentAlignment.MiddleLeft;
        btnMenuQuality.Padding = new Padding(14, 0, 0, 0);
        btnMenuQuality.Cursor = Cursors.Hand;
        btnMenuQuality.Font = new Font("Segoe UI", 9.5f);

        // Button Financial
        btnMenuFinancial.Text = "Tổng quan tài chính";
        btnMenuFinancial.Location = new Point(10, 138);
        btnMenuFinancial.Size = new Size(208, 44);
        btnMenuFinancial.FlatStyle = FlatStyle.Flat;
        btnMenuFinancial.FlatAppearance.BorderSize = 1;
        btnMenuFinancial.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnMenuFinancial.TextAlign = ContentAlignment.MiddleLeft;
        btnMenuFinancial.Padding = new Padding(14, 0, 0, 0);
        btnMenuFinancial.Cursor = Cursors.Hand;
        btnMenuFinancial.Font = new Font("Segoe UI", 9.5f);

        // Button Classes
        btnMenuClasses.Text = "Thống kê sĩ số lớp";
        btnMenuClasses.Location = new Point(10, 188);
        btnMenuClasses.Size = new Size(208, 44);
        btnMenuClasses.FlatStyle = FlatStyle.Flat;
        btnMenuClasses.FlatAppearance.BorderSize = 1;
        btnMenuClasses.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnMenuClasses.TextAlign = ContentAlignment.MiddleLeft;
        btnMenuClasses.Padding = new Padding(14, 0, 0, 0);
        btnMenuClasses.Cursor = Cursors.Hand;
        btnMenuClasses.Font = new Font("Segoe UI", 9.5f);

        pnlSideMenu.Controls.Add(btnMenuClasses);
        pnlSideMenu.Controls.Add(btnMenuFinancial);
        pnlSideMenu.Controls.Add(btnMenuQuality);
        pnlSideMenu.Controls.Add(btnMenuRevenue);
        pnlSideMenu.Controls.Add(lblSideMenuTitle);

        // 2B. CONTENT AREA
        pnlContentArea.Dock = DockStyle.Fill;
        pnlContentArea.BackColor = Color.White;
        pnlContentArea.Padding = new Padding(16, 0, 0, 0);

        // TOOLBAR
        pnlToolbar.Dock = DockStyle.Top;
        pnlToolbar.Height = 50;
        pnlToolbar.BackColor = Color.White;
        pnlToolbar.Padding = new Padding(0, 8, 0, 8);

        lblCurrentReportTitle.Text = "Báo cáo Doanh thu đào tạo";
        lblCurrentReportTitle.Font = new Font("Segoe UI", 10.5f, FontStyle.Bold);
        lblCurrentReportTitle.ForeColor = Color.FromArgb(23, 67, 197);
        lblCurrentReportTitle.UseMnemonic = false;
        lblCurrentReportTitle.AutoSize = true;
        lblCurrentReportTitle.Location = new Point(0, 14);

        lblPeriod.Text = "Kỳ:";
        lblPeriod.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        lblPeriod.ForeColor = Color.FromArgb(15, 23, 42);
        lblPeriod.AutoSize = true;
        lblPeriod.Location = new Point(360, 14);

        cboPeriod.Font = new Font("Segoe UI", 9f);
        cboPeriod.DropDownStyle = ComboBoxStyle.DropDownList;
        cboPeriod.Width = 145;
        cboPeriod.Location = new Point(390, 10);
        cboPeriod.Items.AddRange(new object[] { "Kỳ hiện hành", "Quý 4/2026", "Năm 2026", "Toàn bộ dữ liệu" });

        btnRefresh.Text = "Làm mới";
        btnRefresh.Size = new Size(100, 32);
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.FlatAppearance.BorderSize = 1;
        btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnRefresh.BackColor = Color.White;
        btnRefresh.ForeColor = Color.FromArgb(15, 23, 42);
        btnRefresh.Font = new Font("Segoe UI", 9.5f);
        btnRefresh.Cursor = Cursors.Hand;
        btnRefresh.Location = new Point(580, 9);

        btnExportExcel.Text = "Xuất Excel";
        btnExportExcel.Size = new Size(115, 32);
        btnExportExcel.FlatStyle = FlatStyle.Flat;
        btnExportExcel.FlatAppearance.BorderSize = 0;
        btnExportExcel.BackColor = Color.FromArgb(30, 86, 240);
        btnExportExcel.ForeColor = Color.White;
        btnExportExcel.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnExportExcel.Cursor = Cursors.Hand;
        btnExportExcel.Location = new Point(690, 9);

        btnPrint.Text = "In báo cáo";
        btnPrint.Size = new Size(115, 32);
        btnPrint.FlatStyle = FlatStyle.Flat;
        btnPrint.FlatAppearance.BorderSize = 1;
        btnPrint.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnPrint.BackColor = Color.White;
        btnPrint.ForeColor = Color.FromArgb(15, 23, 42);
        btnPrint.Font = new Font("Segoe UI", 9.5f);
        btnPrint.Cursor = Cursors.Hand;
        btnPrint.Location = new Point(815, 9);

        pnlToolbar.Controls.Add(btnPrint);
        pnlToolbar.Controls.Add(btnExportExcel);
        pnlToolbar.Controls.Add(btnRefresh);
        pnlToolbar.Controls.Add(cboPeriod);
        pnlToolbar.Controls.Add(lblPeriod);
        pnlToolbar.Controls.Add(lblCurrentReportTitle);

        // GRID CONTAINER
        pnlGridContainer.Dock = DockStyle.Fill;
        pnlGridContainer.BackColor = Color.White;

        // Bảng 1: Doanh thu
        dgvRevenue.Dock = DockStyle.Fill;
        dgvRevenue.Columns.Add(new DataGridViewTextBoxColumn { Name = "ChuongTrinh", HeaderText = "Chương trình", FillWeight = 125 });
        dgvRevenue.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoLop", HeaderText = "Lớp", FillWeight = 55, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
        dgvRevenue.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoHocVien", HeaderText = "Số HV", FillWeight = 70, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
        dgvRevenue.Columns.Add(new DataGridViewTextBoxColumn { Name = "TongHocPhi", HeaderText = "Tổng học phí", FillWeight = 130, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
        dgvRevenue.Columns.Add(new DataGridViewTextBoxColumn { Name = "DaThu", HeaderText = "Đã thực thu", FillWeight = 130, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
        dgvRevenue.Columns.Add(new DataGridViewTextBoxColumn { Name = "ConNo", HeaderText = "Công nợ", FillWeight = 120, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
        dgvRevenue.Columns.Add(new DataGridViewTextBoxColumn { Name = "TyLeThu", HeaderText = "Tỷ lệ", FillWeight = 70, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });

        // Bảng 2: Chất lượng
        dgvQuality.Dock = DockStyle.Fill;
        dgvQuality.Columns.Add(new DataGridViewTextBoxColumn { Name = "KhoaHoc", HeaderText = "Khóa đào tạo", FillWeight = 170 });
        dgvQuality.Columns.Add(new DataGridViewTextBoxColumn { Name = "SiSo", HeaderText = "Sĩ số tốt nghiệp", FillWeight = 90 });
        dgvQuality.Columns.Add(new DataGridViewTextBoxColumn { Name = "XuatSac", HeaderText = "Xuất sắc (>= 8.5)", FillWeight = 100 });
        dgvQuality.Columns.Add(new DataGridViewTextBoxColumn { Name = "Gioi", HeaderText = "Giỏi (7.0 - 8.4)", FillWeight = 95 });
        dgvQuality.Columns.Add(new DataGridViewTextBoxColumn { Name = "Kha", HeaderText = "Khá (6.0 - 6.9)", FillWeight = 90 });
        dgvQuality.Columns.Add(new DataGridViewTextBoxColumn { Name = "Dat", HeaderText = "Đạt (5.0 - 5.9)", FillWeight = 85 });
        dgvQuality.Columns.Add(new DataGridViewTextBoxColumn { Name = "TyLeDat", HeaderText = "Tỷ lệ Đạt", FillWeight = 90 });
        dgvQuality.Columns.Add(new DataGridViewTextBoxColumn { Name = "DanhGia", HeaderText = "Đánh giá chất lượng", FillWeight = 140 });

        // Bảng 3: Tổng quan tài chính
        dgvFinancial.Dock = DockStyle.Fill;
        dgvFinancial.Columns.Add(new DataGridViewTextBoxColumn { Name = "KhoanMuc", HeaderText = "Khoản mục tài chính", FillWeight = 170 });
        dgvFinancial.Columns.Add(new DataGridViewTextBoxColumn { Name = "KyTruoc", HeaderText = "Số liệu kỳ trước", FillWeight = 120, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
        dgvFinancial.Columns.Add(new DataGridViewTextBoxColumn { Name = "KyNay", HeaderText = "Số liệu kỳ này", FillWeight = 125, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
        dgvFinancial.Columns.Add(new DataGridViewTextBoxColumn { Name = "TangTruong", HeaderText = "Tăng trưởng", FillWeight = 90, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
        dgvFinancial.Columns.Add(new DataGridViewTextBoxColumn { Name = "KeHoach", HeaderText = "Kế hoạch giao", FillWeight = 125, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleRight } });
        dgvFinancial.Columns.Add(new DataGridViewTextBoxColumn { Name = "TyLeHoanThanh", HeaderText = "Tỷ lệ đạt", FillWeight = 90, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
        dgvFinancial.Columns.Add(new DataGridViewTextBoxColumn { Name = "GhiChu", HeaderText = "Đánh giá ngân sách", FillWeight = 140 });

        // Bảng 4: Sĩ số lớp
        dgvClasses.Dock = DockStyle.Fill;
        dgvClasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "ChuongTrinh", HeaderText = "Nhóm chương trình", FillWeight = 165 });
        dgvClasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "TongSoLop", HeaderText = "Tổng số lớp", FillWeight = 85 });
        dgvClasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "SiSoBinhQuan", HeaderText = "Sĩ số BQ/lớp", FillWeight = 105 });
        dgvClasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "LopDaDay", HeaderText = "Lớp đã đầy", FillWeight = 90 });
        dgvClasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "ChoConTrong", HeaderText = "Chỗ còn trống", FillWeight = 95 });
        dgvClasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "TyLeLapDay", HeaderText = "Tỷ lệ lấp đầy", FillWeight = 95, DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter } });
        dgvClasses.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Tình trạng tuyển sinh", FillWeight = 135 });

        pnlGridContainer.Controls.Add(dgvRevenue);
        pnlGridContainer.Controls.Add(dgvQuality);
        pnlGridContainer.Controls.Add(dgvFinancial);
        pnlGridContainer.Controls.Add(dgvClasses);

        // FOOTER
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Height = 44;
        pnlFooter.BackColor = Color.FromArgb(248, 250, 252);
        pnlFooter.Padding = new Padding(16, 8, 16, 8);

        lblSummary.Dock = DockStyle.Fill;
        lblSummary.TextAlign = ContentAlignment.MiddleLeft;
        lblSummary.Font = new Font("Segoe UI", 9.5f);
        lblSummary.ForeColor = Color.FromArgb(71, 85, 105);

        pnlFooter.Controls.Add(lblSummary);

        // Ghép nối cấu trúc Content Area
        pnlContentArea.Controls.Add(pnlGridContainer);
        pnlContentArea.Controls.Add(pnlToolbar);

        // Ghép nối Main Body
        pnlBody.Controls.Add(pnlContentArea);
        pnlBody.Controls.Add(pnlSideMenu);
        pnlBody.Controls.Add(pnlFooter);

        // Ghép vào UserControl tổng thể
        Controls.Add(pnlBody);
        Controls.Add(pnlHeaderStats);

        pnlHeaderStats.ResumeLayout(false);
        pnlBody.ResumeLayout(false);
        pnlSideMenu.ResumeLayout(false);
        pnlSideMenu.PerformLayout();
        pnlContentArea.ResumeLayout(false);
        pnlToolbar.ResumeLayout(false);
        pnlToolbar.PerformLayout();
        pnlGridContainer.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvRevenue).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvQuality).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvFinancial).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvClasses).EndInit();
        pnlFooter.ResumeLayout(false);
        ResumeLayout(false);
    }
}
