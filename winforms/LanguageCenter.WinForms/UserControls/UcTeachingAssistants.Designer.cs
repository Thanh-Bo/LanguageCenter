namespace LanguageCenter.WinForms.UserControls;

partial class UcTeachingAssistants
{
    private System.ComponentModel.IContainer components = null;

    private Panel pnlToolbar = null!;
    private TextBox txtSearch = null!;
    private ComboBox cboTrangThai = null!;
    private Button btnSearch = null!;
    private Button btnRefresh = null!;
    private Button btnAdd = null!;
    private DataGridView dgvAssistants = null!;
    private Panel pnlFooter = null!;
    private Label lblPageInfo = null!;
    private Button btnPrevPage = null!;
    private Button btnNextPage = null!;
    private Label lblStatus = null!;

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

        pnlToolbar = new Panel();
        txtSearch = UI.UiHelpers.CreateTextBox(260);
        cboTrangThai = UI.UiHelpers.CreateComboBox(160);
        btnSearch = UI.UiHelpers.CreatePrimaryButton("Tìm kiếm", 100, 32);
        btnRefresh = UI.UiHelpers.CreateSecondaryButton("⟳", 42, 32);
        btnAdd = UI.UiHelpers.CreatePrimaryButton("+ Thêm trợ giảng", 160, 32);
        dgvAssistants = new DataGridView();
        pnlFooter = new Panel();
        lblPageInfo = new Label();
        btnPrevPage = UI.UiHelpers.CreateSecondaryButton("‹ Trước", 90, 30);
        btnNextPage = UI.UiHelpers.CreateSecondaryButton("Sau ›", 90, 30);
        lblStatus = new Label();

        SuspendLayout();

        Dock = DockStyle.Fill;
        BackColor = Color.White;

        pnlToolbar.Dock = DockStyle.Top;
        pnlToolbar.Height = 56;
        pnlToolbar.Padding = new Padding(0, 0, 0, 12);

        txtSearch.Location = new Point(0, 2);
        txtSearch.PlaceholderText = "Tìm trợ giảng theo mã, họ tên hoặc email...";
        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; _ = ReloadAsync(resetPage: true); } };

        cboTrangThai.Location = new Point(272, 2);

        btnSearch.Location = new Point(444, 1);
        btnSearch.Click += async (_, _) => await ReloadAsync(resetPage: true);

        btnRefresh.Location = new Point(552, 1);
        btnRefresh.Click += async (_, _) => await ReloadAsync(resetPage: false);

        btnAdd.Location = new Point(604, 1);
        btnAdd.Click += BtnAdd_Click;

        pnlToolbar.Resize += (_, _) =>
        {
            btnAdd.Location = new Point(pnlToolbar.Width - btnAdd.Width - 4, 1);
        };

        pnlToolbar.Controls.Add(btnAdd);
        pnlToolbar.Controls.Add(btnRefresh);
        pnlToolbar.Controls.Add(btnSearch);
        pnlToolbar.Controls.Add(cboTrangThai);
        pnlToolbar.Controls.Add(txtSearch);

        dgvAssistants.Dock = DockStyle.Fill;
        UI.UiHelpers.StyleGrid(dgvAssistants);
        dgvAssistants.CellDoubleClick += DgvAssistants_CellDoubleClick;

        dgvAssistants.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaGiaoVien", HeaderText = "Mã TG", FillWeight = 75 });
        dgvAssistants.Columns.Add(new DataGridViewTextBoxColumn { Name = "HoTen", HeaderText = "Họ tên trợ giảng", FillWeight = 150 });
        dgvAssistants.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", FillWeight = 140 });
        dgvAssistants.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoDienThoai", HeaderText = "SĐT", FillWeight = 95 });
        dgvAssistants.Columns.Add(new DataGridViewTextBoxColumn { Name = "ChuyenMon", HeaderText = "Kỹ năng hỗ trợ / Chuyên môn", FillWeight = 160 });
        dgvAssistants.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoNamKinhNghiem", HeaderText = "Số năm KN", FillWeight = 80 });
        dgvAssistants.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", FillWeight = 90 });

        pnlFooter.Dock = DockStyle.Bottom;
        pnlFooter.Height = 44;

        lblStatus.AutoSize = true;
        lblStatus.Font = UI.Theme.FontRegular;
        lblStatus.ForeColor = UI.Theme.Danger;
        lblStatus.Location = new Point(0, 12);

        lblPageInfo.AutoSize = true;
        lblPageInfo.Font = UI.Theme.FontRegular;
        lblPageInfo.ForeColor = UI.Theme.Muted;
        lblPageInfo.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblPageInfo.Location = new Point(600, 12);

        btnPrevPage.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnPrevPage.Location = new Point(800, 6);
        btnPrevPage.Click += async (_, _) => await ChangePageAsync(-1);

        btnNextPage.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnNextPage.Location = new Point(896, 6);
        btnNextPage.Click += async (_, _) => await ChangePageAsync(1);

        pnlFooter.Controls.Add(btnNextPage);
        pnlFooter.Controls.Add(btnPrevPage);
        pnlFooter.Controls.Add(lblPageInfo);
        pnlFooter.Controls.Add(lblStatus);

        Controls.Add(dgvAssistants);
        Controls.Add(pnlFooter);
        Controls.Add(pnlToolbar);

        ResumeLayout(false);
    }
}

