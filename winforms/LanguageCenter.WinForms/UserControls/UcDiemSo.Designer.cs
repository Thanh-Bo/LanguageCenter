namespace LanguageCenter.WinForms.UserControls;

partial class UcDiemSo
{
    private System.ComponentModel.IContainer components = null;

    private Panel pnlHeaderStats = null!;
    private TableLayoutPanel tblStats = null!;
    private Panel pnlToolbar = null!;
    private Label lblClass = null!;
    private ComboBox cboLopHoc = null!;
    private Button btnCalculate = null!;
    private Button btnSave = null!;
    private Button btnExport = null!;
    private Button btnRefresh = null!;
    private DataGridView dgvGrades = null!;
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
        lblClass = new Label();
        cboLopHoc = new ComboBox();
        btnCalculate = new Button();
        btnSave = new Button();
        btnExport = new Button();
        btnRefresh = new Button();
        dgvGrades = new DataGridView();
        pnlFooter = new Panel();
        lblSummary = new Label();

        pnlHeaderStats.SuspendLayout();
        pnlToolbar.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvGrades).BeginInit();
        pnlFooter.SuspendLayout();
        SuspendLayout();

        // UcDiemSo
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

        lblClass.Text = "Lớp học:";
        lblClass.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        lblClass.ForeColor = Color.FromArgb(15, 23, 42);
        lblClass.AutoSize = true;
        lblClass.Location = new Point(0, 8);

        cboLopHoc.Font = new Font("Segoe UI", 9.5f);
        cboLopHoc.DropDownStyle = ComboBoxStyle.DropDownList;
        cboLopHoc.Width = 240;
        cboLopHoc.Location = new Point(65, 4);

        btnRefresh.Text = "Làm mới";
        btnRefresh.Size = new Size(105, 32);
        btnRefresh.FlatStyle = FlatStyle.Flat;
        btnRefresh.FlatAppearance.BorderSize = 1;
        btnRefresh.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnRefresh.BackColor = Color.White;
        btnRefresh.ForeColor = Color.FromArgb(15, 23, 42);
        btnRefresh.Font = new Font("Segoe UI", 9.5f);
        btnRefresh.Cursor = Cursors.Hand;
        btnRefresh.Location = new Point(315, 3);

        btnCalculate.Text = "Tính lại điểm TB";
        btnCalculate.Size = new Size(130, 32);
        btnCalculate.FlatStyle = FlatStyle.Flat;
        btnCalculate.FlatAppearance.BorderSize = 1;
        btnCalculate.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnCalculate.BackColor = Color.White;
        btnCalculate.ForeColor = Color.FromArgb(15, 23, 42);
        btnCalculate.Font = new Font("Segoe UI", 9.5f);
        btnCalculate.Cursor = Cursors.Hand;
        btnCalculate.Location = new Point(430, 3);

        btnSave.Text = "Lưu bảng điểm";
        btnSave.Size = new Size(130, 32);
        btnSave.FlatStyle = FlatStyle.Flat;
        btnSave.FlatAppearance.BorderSize = 0;
        btnSave.BackColor = Color.FromArgb(30, 86, 240);
        btnSave.ForeColor = Color.White;
        btnSave.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
        btnSave.Cursor = Cursors.Hand;
        btnSave.Location = new Point(570, 3);

        btnExport.Text = "Xuất bảng điểm";
        btnExport.Size = new Size(130, 32);
        btnExport.FlatStyle = FlatStyle.Flat;
        btnExport.FlatAppearance.BorderSize = 1;
        btnExport.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnExport.BackColor = Color.White;
        btnExport.ForeColor = Color.FromArgb(15, 23, 42);
        btnExport.Font = new Font("Segoe UI", 9.5f);
        btnExport.Cursor = Cursors.Hand;
        btnExport.Location = new Point(710, 3);

        pnlToolbar.Controls.Add(btnExport);
        pnlToolbar.Controls.Add(btnSave);
        pnlToolbar.Controls.Add(btnCalculate);
        pnlToolbar.Controls.Add(btnRefresh);
        pnlToolbar.Controls.Add(cboLopHoc);
        pnlToolbar.Controls.Add(lblClass);

        // 3. DATAGRIDVIEW
        dgvGrades.Dock = DockStyle.Fill;
        dgvGrades.ReadOnly = false;

        var colMa = new DataGridViewTextBoxColumn { Name = "MaHocVien", HeaderText = "Mã HV", FillWeight = 75, ReadOnly = true };
        var colTen = new DataGridViewTextBoxColumn { Name = "TenHocVien", HeaderText = "Họ và tên học viên", FillWeight = 160, ReadOnly = true };
        var colCC = new DataGridViewTextBoxColumn { Name = "DiemChuyenCan", HeaderText = "Chuyên cần (10%)", FillWeight = 85 };
        var colGK = new DataGridViewTextBoxColumn { Name = "DiemGiuaKy", HeaderText = "Giữa kỳ (20%)", FillWeight = 85 };
        var colSP = new DataGridViewTextBoxColumn { Name = "DiemSpeaking", HeaderText = "Speaking (20%)", FillWeight = 85 };
        var colWR = new DataGridViewTextBoxColumn { Name = "DiemWriting", HeaderText = "Writing (20%)", FillWeight = 85 };
        var colCK = new DataGridViewTextBoxColumn { Name = "DiemCuoiKy", HeaderText = "Cuối kỳ (30%)", FillWeight = 85 };
        var colTK = new DataGridViewTextBoxColumn { Name = "DiemTongKet", HeaderText = "Điểm tổng kết", FillWeight = 90, ReadOnly = true };
        var colXL = new DataGridViewTextBoxColumn { Name = "XepLoai", HeaderText = "Xếp loại", FillWeight = 85, ReadOnly = true };
        var colNX = new DataGridViewTextBoxColumn { Name = "NhanXet", HeaderText = "Nhận xét của giảng viên", FillWeight = 180 };

        dgvGrades.Columns.AddRange(new DataGridViewColumn[] { colMa, colTen, colCC, colGK, colSP, colWR, colCK, colTK, colXL, colNX });

        // 4. FOOTER
        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Height = 36;

        lblSummary.AutoSize = true;
        lblSummary.Font = new Font("Segoe UI", 9.5f);
        lblSummary.ForeColor = Color.FromArgb(100, 116, 139);
        lblSummary.Location = new Point(0, 8);

        pnlFooter.Controls.Add(lblSummary);

        // ADD ALL
        Controls.Add(dgvGrades);
        Controls.Add(pnlFooter);
        Controls.Add(pnlToolbar);
        Controls.Add(pnlHeaderStats);

        pnlHeaderStats.ResumeLayout(false);
        pnlToolbar.ResumeLayout(false);
        pnlToolbar.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvGrades).EndInit();
        pnlFooter.ResumeLayout(false);
        pnlFooter.PerformLayout();
        ResumeLayout(false);
    }
}
