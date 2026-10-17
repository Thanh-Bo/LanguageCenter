using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

public partial class FormXepLopHocVien : Form
{
    private readonly LopHocDto _targetClass;

    public FormXepLopHocVien() : this(new LopHocDto { MaLop = "LỚP_MẪU" })
    {
    }

    public FormXepLopHocVien(LopHocDto targetClass)
    {
        _targetClass = targetClass;
        InitializeComponent();

        Text = $"Xếp học viên vào lớp {_targetClass.MaLop} — HUIT LingoHub";
        lblTitle.Text = $"CHỌN HỌC VIÊN XẾP VÀO LỚP: {_targetClass.TenLop} ({_targetClass.MaLop})";
        var isFull = _targetClass.SiSoHienTai >= _targetClass.SiSoToiDa;
        lblCapacity.Text = $"Sĩ số hiện tại: {_targetClass.SiSoHienTai}/{_targetClass.SiSoToiDa} học viên" + (isFull ? " (ĐÃ ĐẦY LỚP)" : $" (Còn {_targetClass.SiSoToiDa - _targetClass.SiSoHienTai} chỗ trống)");
        lblCapacity.ForeColor = isFull ? Color.FromArgb(239, 68, 68) : Color.FromArgb(16, 185, 129);
        lblClassInfo.Text = $"Khóa học: {_targetClass.TenKhoaHoc}  |  Phòng: {_targetClass.TenPhongHoc}  |  Lịch học: {_targetClass.LichHoc}";
        chkFilterByCourse.Text = $"Chỉ hiện học viên đăng ký khóa \"{_targetClass.TenKhoaHoc}\"";

        txtSearch.TextChanged += (_, _) => FilterWaitingStudents();
        chkFilterByCourse.CheckedChanged += (_, _) => FilterWaitingStudents();
        btnConfirm.Click += BtnConfirm_Click;
        btnCancel.Click += (_, _) => Close();
        dgvWaiting.CellDoubleClick += (_, _) => BtnConfirm_Click(null, EventArgs.Empty);

        pnlBottom.Resize += (_, _) =>
        {
            btnCancel.Location = new Point(pnlBottom.Width - btnCancel.Width - 16, 10);
            btnConfirm.Location = new Point(btnCancel.Left - btnConfirm.Width - 10, 10);
        };

        LoadWaitingStudents();
    }

    private void LoadWaitingStudents()
    {
        FilterWaitingStudents();
    }

    private void FilterWaitingStudents()
    {
        dgvWaiting.Rows.Clear();
        var kw = txtSearch.Text.Trim();

        var query = MockDataStore.GetEnrollments()
            .Where(e => e.TrangThaiDangKy == "Chờ xếp lớp" || !e.MaLopHoc.HasValue);

        if (chkFilterByCourse.Checked && !string.IsNullOrWhiteSpace(_targetClass.TenKhoaHoc))
        {
            query = query.Where(e => e.TenKhoaHoc.Equals(_targetClass.TenKhoaHoc, StringComparison.OrdinalIgnoreCase) ||
                                     e.MaKhoaHoc == _targetClass.MaKhoaHoc);
        }

        if (!string.IsNullOrWhiteSpace(kw))
        {
            query = query.Where(e => e.TenHocVien.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                     e.MaHocVien.Contains(kw, StringComparison.OrdinalIgnoreCase) ||
                                     e.SoDienThoai.Contains(kw));
        }

        var list = query.ToList();
        foreach (var p in list)
        {
            var rowIdx = dgvWaiting.Rows.Add(
                p.MaDangKy,
                p.MaHocVien,
                p.TenHocVien,
                p.SoDienThoai,
                p.TenKhoaHoc,
                UiHelpers.FormatDateTime(p.NgayDangKy),
                p.TrangThaiHocPhi,
                p.GhiChu ?? "Chờ sắp xếp lịch phù hợp"
            );
            dgvWaiting.Rows[rowIdx].Tag = p.MaDangKy;
            dgvWaiting.Rows[rowIdx].Cells["HocPhi"].Style.ForeColor = p.TrangThaiHocPhi == "Đã hoàn thành" ? Theme.Success : Theme.Warning;
        }

        btnConfirm.Enabled = dgvWaiting.Rows.Count > 0 && _targetClass.SiSoHienTai < _targetClass.SiSoToiDa;
    }

    private void BtnConfirm_Click(object? sender, EventArgs e)
    {
        if (dgvWaiting.CurrentRow == null)
        {
            MessageBox.Show(this, "Vui lòng chọn một học viên trong danh sách để xếp vào lớp.", "Chưa chọn học viên", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        if (_targetClass.SiSoHienTai >= _targetClass.SiSoToiDa)
        {
            MessageBox.Show(this, $"Lớp {_targetClass.MaLop} đã đạt sĩ số tối đa ({_targetClass.SiSoToiDa} học viên).\nKhông thể xếp thêm học viên vào lớp này.", "Lớp đã đầy", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var regId = Convert.ToInt32(dgvWaiting.CurrentRow.Tag ?? dgvWaiting.CurrentRow.Cells["MaDangKy"].Value);
        var studentName = dgvWaiting.CurrentRow.Cells["TenHocVien"].Value?.ToString() ?? "Học viên";

        var ok = MockDataStore.EnrollStudentToClass(regId, _targetClass.MaLopHoc);
        if (ok)
        {
            MessageBox.Show(this, $"Đã xếp học viên {studentName} vào lớp {_targetClass.TenLop} thành công!", "Xếp lớp thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
    }

    private void lblCapacity_Click(object sender, EventArgs e)
    {

    }
}

