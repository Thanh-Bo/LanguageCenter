using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

public partial class FormSuaPhanCong : Form
{
    public FormSuaPhanCong()
    {
        InitializeComponent();
        btnSave.Click += BtnSave_Click;
        btnCancel.Click += (_, _) => DialogResult = DialogResult.Cancel;
        LoadData();
    }

    private void LoadData()
    {
        cboLopHoc.Items.Clear();
        foreach (var c in MockDataStore.GetClasses()) cboLopHoc.Items.Add(c.TenLop);
        if (cboLopHoc.Items.Count > 0) cboLopHoc.SelectedIndex = 0;

        cboNhanSu.Items.Clear();
        cboNhanSu.Items.Add("GV001 - ThS. Phạm Minh Tuấn (Giáo viên)");
        cboNhanSu.Items.Add("GV002 - Sarah Jenkins (Giáo viên)");
        cboNhanSu.Items.Add("TG001 - Nguyễn Văn An (Trợ giảng)");
        cboNhanSu.Items.Add("TG002 - Đặng Mai Phương (Trợ giảng)");
        cboNhanSu.Items.Add("TG003 - Lê Hoàng Phúc (Trợ giảng)");
        cboNhanSu.SelectedIndex = 0;

        dtpBatDau.Value = DateTime.Today;
        txtGhiChu.Text = "Giảng dạy theo đề cương chuẩn của trung tâm";
    }

    private void BtnSave_Click(object? sender, EventArgs e)
    {
        var selectedLop = cboLopHoc.SelectedItem?.ToString();
        var selectedNS = cboNhanSu.SelectedItem?.ToString();
        if (string.IsNullOrEmpty(selectedLop) || string.IsNullOrEmpty(selectedNS))
        {
            MessageBox.Show(this, "Vui lòng chọn lớp học và nhân sự.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var parts = selectedNS.Split(" - ");
        var maGv = parts[0].Trim();
        var rest = parts[1];
        var tenGv = rest.Split('(')[0].Trim();
        var loai = selectedNS.Contains("Trợ giảng") ? "Trợ giảng" : "Giáo viên";

        var cls = MockDataStore.GetClasses().FirstOrDefault(c => c.TenLop == selectedLop);

        var item = new PhanCongGiangDayDto
        {
            MaLopHoc = cls?.MaLopHoc ?? 1,
            MaLop = cls?.MaLop ?? "Lop01",
            TenLop = selectedLop,
            MaGiaoVien = maGv,
            TenGiaoVien = tenGv,
            LoaiNhanSu = loai,
            VaiTroPhanCong = cboVaiTro.SelectedItem?.ToString() ?? "Giáo viên chính",
            NgayBatDau = DateOnly.FromDateTime(dtpBatDau.Value),
            TrangThai = "Đang giảng dạy",
            GhiChu = txtGhiChu.Text.Trim()
        };

        MockDataStore.AddOrUpdateAssignment(item);
        DialogResult = DialogResult.OK;
    }
}

