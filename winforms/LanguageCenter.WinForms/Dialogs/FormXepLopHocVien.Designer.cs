namespace LanguageCenter.WinForms.Dialogs;

partial class FormXepLopHocVien
{
    private System.ComponentModel.IContainer components = null;

    private Panel pnlHeader = null!;
    private Label lblTitle = null!;
    private Label lblCapacity = null!;
    private Label lblClassInfo = null!;
    private Panel pnlToolbar = null!;
    private TextBox txtSearch = null!;
    private CheckBox chkFilterByCourse = null!;
    private Panel pnlBottom = null!;
    private Button btnConfirm = null!;
    private Button btnCancel = null!;
    private DataGridView dgvWaiting = null!;

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
        pnlHeader = new Panel();
        lblTitle = new Label();
        lblCapacity = new Label();
        lblClassInfo = new Label();
        pnlToolbar = new Panel();
        txtSearch = new TextBox();
        chkFilterByCourse = new CheckBox();
        pnlBottom = new Panel();
        btnConfirm = new Button();
        btnCancel = new Button();
        dgvWaiting = new DataGridView();
        dataGridViewTextBoxColumn1 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn2 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn3 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn4 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn5 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn6 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn7 = new DataGridViewTextBoxColumn();
        dataGridViewTextBoxColumn8 = new DataGridViewTextBoxColumn();
        pnlHeader.SuspendLayout();
        pnlToolbar.SuspendLayout();
        pnlBottom.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvWaiting).BeginInit();
        SuspendLayout();
        // 
        // pnlHeader
        // 
        pnlHeader.BackColor = Color.FromArgb(248, 250, 252);
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Controls.Add(lblCapacity);
        pnlHeader.Controls.Add(lblClassInfo);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Padding = new Padding(20, 12, 20, 10);
        pnlHeader.Size = new Size(762, 80);
        pnlHeader.TabIndex = 2;
        // 
        // lblTitle
        // 
        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 15F, FontStyle.Bold);
        lblTitle.ForeColor = Color.FromArgb(23, 67, 197);
        lblTitle.Location = new Point(16, 12);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(380, 35);
        lblTitle.TabIndex = 0;
        lblTitle.Text = "CHỌN HỌC VIÊN XẾP VÀO LỚP";
        // 
        // lblCapacity
        // 
        lblCapacity.AutoSize = true;
        lblCapacity.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        lblCapacity.ForeColor = Color.FromArgb(16, 185, 129);
        lblCapacity.Location = new Point(402, 23);
        lblCapacity.Name = "lblCapacity";
        lblCapacity.Size = new Size(209, 21);
        lblCapacity.TabIndex = 1;
        lblCapacity.Text = "Sĩ số hiện tại: 0/0 học viên";
        lblCapacity.Click += lblCapacity_Click;
        // 
        // lblClassInfo
        // 
        lblClassInfo.AutoSize = true;
        lblClassInfo.Font = new Font("Segoe UI", 9.5F);
        lblClassInfo.ForeColor = Color.FromArgb(100, 116, 139);
        lblClassInfo.Location = new Point(16, 49);
        lblClassInfo.Name = "lblClassInfo";
        lblClassInfo.Size = new Size(229, 21);
        lblClassInfo.TabIndex = 2;
        lblClassInfo.Text = "Khóa học | Phòng học | Lịch học";
        // 
        // pnlToolbar
        // 
        pnlToolbar.BackColor = Color.White;
        pnlToolbar.Controls.Add(txtSearch);
        pnlToolbar.Controls.Add(chkFilterByCourse);
        pnlToolbar.Dock = DockStyle.Top;
        pnlToolbar.Location = new Point(0, 80);
        pnlToolbar.Name = "pnlToolbar";
        pnlToolbar.Padding = new Padding(16, 6, 16, 6);
        pnlToolbar.Size = new Size(762, 44);
        pnlToolbar.TabIndex = 1;
        // 
        // txtSearch
        // 
        txtSearch.Font = new Font("Segoe UI", 9.5F);
        txtSearch.Location = new Point(16, 6);
        txtSearch.Name = "txtSearch";
        txtSearch.PlaceholderText = "Tìm theo tên, mã HV hoặc SĐT...";
        txtSearch.Size = new Size(280, 29);
        txtSearch.TabIndex = 0;
        // 
        // chkFilterByCourse
        // 
        chkFilterByCourse.AutoSize = true;
        chkFilterByCourse.Checked = true;
        chkFilterByCourse.CheckState = CheckState.Checked;
        chkFilterByCourse.Font = new Font("Segoe UI", 9.5F);
        chkFilterByCourse.ForeColor = Color.FromArgb(15, 23, 42);
        chkFilterByCourse.Location = new Point(310, 10);
        chkFilterByCourse.Name = "chkFilterByCourse";
        chkFilterByCourse.Size = new Size(306, 25);
        chkFilterByCourse.TabIndex = 1;
        chkFilterByCourse.Text = "Chỉ hiện học viên đăng ký khóa học này";
        // 
        // pnlBottom
        // 
        pnlBottom.BackColor = Color.FromArgb(248, 250, 252);
        pnlBottom.Controls.Add(btnConfirm);
        pnlBottom.Controls.Add(btnCancel);
        pnlBottom.Dock = DockStyle.Bottom;
        pnlBottom.Location = new Point(0, 459);
        pnlBottom.Name = "pnlBottom";
        pnlBottom.Padding = new Padding(16, 10, 16, 10);
        pnlBottom.Size = new Size(762, 54);
        pnlBottom.TabIndex = 3;
        // 
        // btnConfirm
        // 
        btnConfirm.BackColor = Color.FromArgb(30, 86, 240);
        btnConfirm.Cursor = Cursors.Hand;
        btnConfirm.FlatAppearance.BorderSize = 0;
        btnConfirm.FlatStyle = FlatStyle.Flat;
        btnConfirm.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnConfirm.ForeColor = Color.White;
        btnConfirm.Location = new Point(0, 0);
        btnConfirm.Name = "btnConfirm";
        btnConfirm.Size = new Size(150, 34);
        btnConfirm.TabIndex = 0;
        btnConfirm.Text = "Xác nhận xếp lớp";
        btnConfirm.UseVisualStyleBackColor = false;
        // 
        // btnCancel
        // 
        btnCancel.BackColor = Color.White;
        btnCancel.Cursor = Cursors.Hand;
        btnCancel.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
        btnCancel.FlatStyle = FlatStyle.Flat;
        btnCancel.Font = new Font("Segoe UI", 9.5F);
        btnCancel.ForeColor = Color.FromArgb(15, 23, 42);
        btnCancel.Location = new Point(0, 0);
        btnCancel.Name = "btnCancel";
        btnCancel.Size = new Size(100, 34);
        btnCancel.TabIndex = 1;
        btnCancel.Text = "Đóng";
        btnCancel.UseVisualStyleBackColor = false;
        // 
        // dgvWaiting
        // 
        dgvWaiting.AllowUserToAddRows = false;
        dgvWaiting.AllowUserToDeleteRows = false;
        dgvWaiting.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvWaiting.BackgroundColor = Color.White;
        dgvWaiting.BorderStyle = BorderStyle.None;
        dgvWaiting.ColumnHeadersHeight = 29;
        dgvWaiting.Columns.AddRange(new DataGridViewColumn[] { dataGridViewTextBoxColumn1, dataGridViewTextBoxColumn2, dataGridViewTextBoxColumn3, dataGridViewTextBoxColumn4, dataGridViewTextBoxColumn5, dataGridViewTextBoxColumn6, dataGridViewTextBoxColumn7, dataGridViewTextBoxColumn8 });
        dgvWaiting.Dock = DockStyle.Fill;
        dgvWaiting.Location = new Point(0, 124);
        dgvWaiting.Name = "dgvWaiting";
        dgvWaiting.ReadOnly = true;
        dgvWaiting.RowHeadersVisible = false;
        dgvWaiting.RowHeadersWidth = 51;
        dgvWaiting.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvWaiting.Size = new Size(762, 335);
        dgvWaiting.TabIndex = 0;
        // 
        // dataGridViewTextBoxColumn1
        // 
        dataGridViewTextBoxColumn1.MinimumWidth = 6;
        dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
        dataGridViewTextBoxColumn1.ReadOnly = true;
        // 
        // dataGridViewTextBoxColumn2
        // 
        dataGridViewTextBoxColumn2.MinimumWidth = 6;
        dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
        dataGridViewTextBoxColumn2.ReadOnly = true;
        // 
        // dataGridViewTextBoxColumn3
        // 
        dataGridViewTextBoxColumn3.MinimumWidth = 6;
        dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
        dataGridViewTextBoxColumn3.ReadOnly = true;
        // 
        // dataGridViewTextBoxColumn4
        // 
        dataGridViewTextBoxColumn4.MinimumWidth = 6;
        dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
        dataGridViewTextBoxColumn4.ReadOnly = true;
        // 
        // dataGridViewTextBoxColumn5
        // 
        dataGridViewTextBoxColumn5.MinimumWidth = 6;
        dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
        dataGridViewTextBoxColumn5.ReadOnly = true;
        // 
        // dataGridViewTextBoxColumn6
        // 
        dataGridViewTextBoxColumn6.MinimumWidth = 6;
        dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
        dataGridViewTextBoxColumn6.ReadOnly = true;
        // 
        // dataGridViewTextBoxColumn7
        // 
        dataGridViewTextBoxColumn7.MinimumWidth = 6;
        dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
        dataGridViewTextBoxColumn7.ReadOnly = true;
        // 
        // dataGridViewTextBoxColumn8
        // 
        dataGridViewTextBoxColumn8.MinimumWidth = 6;
        dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
        dataGridViewTextBoxColumn8.ReadOnly = true;
        // 
        // FormXepLopHocVien
        // 
        BackColor = Color.White;
        ClientSize = new Size(762, 513);
        Controls.Add(dgvWaiting);
        Controls.Add(pnlToolbar);
        Controls.Add(pnlHeader);
        Controls.Add(pnlBottom);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "FormXepLopHocVien";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Xếp học viên vào lớp — HUIT LingoHub";
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        pnlToolbar.ResumeLayout(false);
        pnlToolbar.PerformLayout();
        pnlBottom.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvWaiting).EndInit();
        ResumeLayout(false);
    }

    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
    private DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
}

