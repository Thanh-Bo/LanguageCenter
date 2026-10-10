namespace LanguageCenter.WinForms.UserControls;

partial class UcStudents
{
    private System.ComponentModel.IContainer components = null;

    private Panel pnlToolbar = null!;
    private TextBox txtSearch = null!;
    private ComboBox cboStatus = null!;
    private Button btnSearch = null!;
    private Button btnRefresh = null!;
    private Button btnAdd = null!;
    private DataGridView dgvStudents = null!;
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
        cboStatus = UI.UiHelpers.CreateComboBox(160);
        btnSearch = UI.UiHelpers.CreatePrimaryButton("Tìm kiếm", 110, 32);
        btnRefresh = UI.UiHelpers.CreateSecondaryButton("⟳", 42, 32);
        btnAdd = UI.UiHelpers.CreatePrimaryButton("+ Thêm học viên", 160, 32);
        dgvStudents = new DataGridView();
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
        txtSearch.PlaceholderText = "Tìm theo mã, họ tên hoặc email...";
        txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; _ = ReloadAsync(resetPage: true); } };

        cboStatus.Location = new Point(272, 2);

        btnSearch.Location = new Point(444, 1);
        btnSearch.Click += async (_, _) => await ReloadAsync(resetPage: true);

        btnRefresh.Location = new Point(562, 1);
        btnRefresh.Click += async (_, _) => await ReloadAsync(resetPage: false);

        btnAdd.Location = new Point(614, 1);
        btnAdd.Click += BtnAdd_Click;

        pnlToolbar.Resize += (_, _) =>
        {
            btnAdd.Location = new Point(pnlToolbar.Width - btnAdd.Width - 4, 1);
        };

        pnlToolbar.Controls.Add(btnAdd);
        pnlToolbar.Controls.Add(btnRefresh);
        pnlToolbar.Controls.Add(btnSearch);
        pnlToolbar.Controls.Add(cboStatus);
        pnlToolbar.Controls.Add(txtSearch);

        dgvStudents.Dock = DockStyle.Fill;
        UI.UiHelpers.StyleGrid(dgvStudents);
        dgvStudents.CellDoubleClick += DgvStudents_CellDoubleClick;

        dgvStudents.Columns.Add(new DataGridViewTextBoxColumn { Name = "MaHocVien", HeaderText = "Mã HV", FillWeight = 70 });
        dgvStudents.Columns.Add(new DataGridViewTextBoxColumn { Name = "HoTen", HeaderText = "Họ tên", FillWeight = 140 });
        dgvStudents.Columns.Add(new DataGridViewTextBoxColumn { Name = "NgaySinh", HeaderText = "Ngày sinh", FillWeight = 90 });
        dgvStudents.Columns.Add(new DataGridViewTextBoxColumn { Name = "GioiTinh", HeaderText = "Giới tính", FillWeight = 70 });
        dgvStudents.Columns.Add(new DataGridViewTextBoxColumn { Name = "Email", HeaderText = "Email", FillWeight = 140 });
        dgvStudents.Columns.Add(new DataGridViewTextBoxColumn { Name = "SoDienThoai", HeaderText = "SĐT", FillWeight = 90 });
        dgvStudents.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThaiHocTap", HeaderText = "Trạng thái", FillWeight = 90 });
        dgvStudents.Columns.Add(new DataGridViewTextBoxColumn { Name = "NgayNhapHoc", HeaderText = "Ngày nhập học", FillWeight = 90 });

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
        lblPageInfo.Location = new Point(620, 12);

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

        Controls.Add(dgvStudents);
        Controls.Add(pnlFooter);
        Controls.Add(pnlToolbar);

        ResumeLayout(false);
    }
}
