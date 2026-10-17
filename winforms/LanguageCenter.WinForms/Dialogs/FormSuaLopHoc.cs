using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

public partial class FormSuaLopHoc : Form
{
    private readonly LopHocDto? _existingClass;
    public bool DataChanged { get; private set; }

    public FormSuaLopHoc() : this(null)
    {
    }

    public FormSuaLopHoc(LopHocDto? existing = null)
    {
        _existingClass = existing;
        InitializeComponent();
        btnSave.Click += BtnSave_Click;
        btnCancel.Click += (_, _) => DialogResult = DialogResult.Cancel;
        if (_existingClass != null)
        {
            Text = $"Chỉnh sửa lớp học {_existingClass.MaLop} — HUIT LingoHub";
            lblTitle.Text = "CẬP NHẬT THÔNG TIN LỚP HỌC";
        }
        LoadData();
    }

    private void LoadData()
    {
        cboKhoaHoc.Items.Clear();
        foreach (var k in MockDataStore.GetCourses()) cboKhoaHoc.Items.Add(k.TenKhoaHoc);
        if (cboKhoaHoc.Items.Count > 0) cboKhoaHoc.SelectedIndex = 0;

        cboGiaoVien.Items.Clear();
        cboGiaoVien.Items.Add("— Chưa phân công —");
        cboGiaoVien.Items.Add("ThS. Phạm Minh Tuấn");
        cboGiaoVien.Items.Add("Sarah Jenkins");
        cboGiaoVien.SelectedIndex = 1;

        cboTroGiang.Items.Clear();
        cboTroGiang.Items.Add("— Chưa phân công —");
        cboTroGiang.Items.Add("Nguyễn Văn An");
        cboTroGiang.Items.Add("Đặng Mai Phương");
        cboTroGiang.Items.Add("Lê Hoàng Phúc");
        cboTroGiang.SelectedIndex = 1;

        cboPhongHoc.Items.Clear();
        foreach (var r in MockDataStore.GetRooms()) cboPhongHoc.Items.Add(r.TenPhong);
        if (cboPhongHoc.Items.Count > 0) cboPhongHoc.SelectedIndex = 0;

        if (_existingClass != null)
        {
            txtMaLop.Text = _existingClass.MaLop;
            txtMaLop.ReadOnly = true;
            txtTenLop.Text = _existingClass.TenLop;
            cboKhoaHoc.SelectedItem = _existingClass.TenKhoaHoc;
            cboGiaoVien.SelectedItem = _existingClass.TenGiaoVienChinh;
            cboTroGiang.SelectedItem = _existingClass.TenTroGiang;
            cboPhongHoc.SelectedItem = _existingClass.TenPhongHoc;
            txtLichHoc.Text = _existingClass.LichHoc;
            numSiSo.Value = _existingClass.SiSoToiDa;
            dtpBatDau.Value = _existingClass.NgayBatDau.ToDateTime(TimeOnly.MinValue);
            dtpKetThuc.Value = _existingClass.NgayKetThucDuKien.ToDateTime(TimeOnly.MinValue);
            cboTrangThai.SelectedItem = _existingClass.TrangThai;
        }
        else
        {
            txtMaLop.Text = $"ENG-K{MockDataStore.GetClasses().Count + 1:D2}";
            txtTenLop.Text = "Tiếng Anh Tổng Quát - Lớp mới";
            txtLichHoc.Text = "T2-4-6 (18:00 - 19:30)";
            dtpBatDau.Value = DateTime.Today.AddDays(7);
            dtpKetThuc.Value = DateTime.Today.AddDays(97);
        }
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtMaLop.Text) || string.IsNullOrWhiteSpace(txtTenLop.Text))
        {
            MessageBox.Show(this, "Vui lòng nhập đầy đủ Mã lớp và Tên lớp học.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var item = _existingClass ?? new LopHocDto();
        item.MaLop = txtMaLop.Text.Trim();
        item.TenLop = txtTenLop.Text.Trim();
        item.TenKhoaHoc = cboKhoaHoc.SelectedItem?.ToString() ?? "Khóa học";
        item.TenGiaoVienChinh = cboGiaoVien.SelectedItem?.ToString() ?? "—";
        item.TenTroGiang = cboTroGiang.SelectedItem?.ToString() ?? "—";
        item.TenPhongHoc = cboPhongHoc.SelectedItem?.ToString() ?? "—";
        item.LichHoc = txtLichHoc.Text.Trim();
        item.SiSoToiDa = (int)numSiSo.Value;
        item.NgayBatDau = DateOnly.FromDateTime(dtpBatDau.Value);
        item.NgayKetThucDuKien = DateOnly.FromDateTime(dtpKetThuc.Value);
        item.TrangThai = cboTrangThai.SelectedItem?.ToString() ?? "Sắp khai giảng";

        MockDataStore.AddOrUpdateClass(item);
        DataChanged = true;
        DialogResult = DialogResult.OK;
    }
}

