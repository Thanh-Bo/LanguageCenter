namespace LanguageCenter.WinForms.UserControls;

partial class UcThongBao
{
    private System.ComponentModel.IContainer components = null;

    private Panel pnlHeaderStats = null!;
    private TableLayoutPanel tblStats = null!;
    private Panel pnlToolbar = null!;
    private TextBox txtSearch = null!;
    private ComboBox cboLoai = null!;
    private ComboBox cboDoiTuong = null!;
    private Button btnSearch = null!;
    private Button btnRefresh = null!;
    private Button btnAdd = null!;
    private DataGridView dgvNotifications = null!;
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
        cboLoai = new ComboBox();
        cboDoiTuong = new ComboBox();
        btnSearch = new Button();
        btnRefresh = new Button();
        btnAdd = new Button();
        dgvNotifications = new DataGridView();
        pnlFooter = new Panel();
        lblSummary = new Label();

        pnlHeaderStats.SuspendLayout();
        pnlToolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvNotifications).BeginInit();
        pnlFooter.SuspendLayout();
        SuspendLayout();

        // UcThongBao
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
        txtSearch.PlaceholderText = "Tìm theo tiêu đề, nội dung...";

        cboLoai.Font = new Font("Segoe UI", 9.5f);
        cboLoai.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLoai.Width = 180;
        cboLoai.Location = new Point(250, 4);

        cboDoiTuong.Font = new Font("Segoe UI", 9.5f);
        cboDoiTuong.DropDownStyle = ComboBoxStyle.DropDownList;
        cboDoiTuong.Width = 160;
        cboDoiTuong.Location = new Point(440, 4);

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

        btnAdd.Text = "+ Soạn thông báo";
        btnAdd.Size = new Size(160, 32);
        btnAdd.FlatStyle = FlatStyle.Flat;
        btnAdd.FlatAppearance.BorderSize = 0;
        btnAdd.BackColor = Color.FromArgb(30, 86, 240);
        btnAdd.ForeColor = Color.White;
        btnAdd.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnAdd.Cursor = Cursors.Hand;
        btnAdd.Location = new Point(830, 3);

        pnlToolbar.Controls.Add(btnAdd);
        pnlToolbar.Controls.Add(btnRefresh);
        pnlToolbar.Controls.Add(btnSearch);
        pnlToolbar.Controls.Add(cboDoiTuong);
        pnlToolbar.Controls.Add(cboLoai);
        pnlToolbar.Controls.Add(txtSearch);

        // 3. DATAGRIDVIEW
        dgvNotifications.Dock = DockStyle.Fill;
        dgvNotifications.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaThongBao", HeaderText = "Mã TB", FillWeight = 60 });
        dgvNotifications.Columns.Add(new DataGridViewTextBoxColumn { Name = "TieuDe", HeaderText = "Tiêu đề thông báo", FillWeight = 240 });
        dgvNotifications.Columns.Add(new DataGridViewTextBoxColumn { Name = "LoaiThongBao", HeaderText = "Loại thông báo", FillWeight = 130 });
        dgvNotifications.Columns.Add(new DataGridViewTextBoxColumn { Name = "DoiTuongNhan", HeaderText = "Đối tượng nhận", FillWeight = 130 });
        dgvNotifications.Columns.Add(new DataGridViewTextBoxColumn { Name = "NguoiGui", HeaderText = "Người phát hành", FillWeight = 110 });
        dgvNotifications.Columns.Add(new DataGridViewTextBoxColumn { Name = "ThoiGianGui", HeaderText = "Thời gian gửi", FillWeight = 110 });
        dgvNotifications.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", FillWeight = 85 });

        // 4. FOOTER
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Height = 36;

        lblSummary.AutoSize = true;
        lblSummary.Font = new Font("Segoe UI", 9.5f);
        lblSummary.ForeColor = Color.FromArgb(100, 116, 139);
        lblSummary.Location = new Point(0, 8);

        pnlFooter.Controls.Add(lblSummary);

        // ADD ALL
        Controls.Add(dgvNotifications);
        Controls.Add(pnlFooter);
        Controls.Add(pnlToolbar);
        Controls.Add(pnlHeaderStats);

        pnlHeaderStats.ResumeLayout(false);
        pnlToolbar.ResumeLayout(false);
        pnlToolbar.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvNotifications).EndInit();
        pnlFooter.ResumeLayout(false);
        pnlFooter.PerformLayout();
        ResumeLayout(false);
    }
}
