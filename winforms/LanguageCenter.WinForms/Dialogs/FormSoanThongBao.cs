using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

public partial class FormSoanThongBao : Form
{
    public FormSoanThongBao()
    {
        InitializeComponent();
        btnSend.Click += (_, _) => SaveNotification("Đã gửi");
        btnDraft.Click += (_, _) => SaveNotification("Bản nháp");
        btnCancel.Click += (_, _) => DialogResult = DialogResult.Cancel;
    }

    private void SaveNotification(string trangThai)
    {
        if (string.IsNullOrWhiteSpace(txtTieuDe.Text) || string.IsNullOrWhiteSpace(txtNoiDung.Text))
        {
            MessageBox.Show(this, "Vui lòng nhập Tiêu đề và Nội dung thông báo.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var notif = new ThongBaoDto
        {
            TieuDe = txtTieuDe.Text.Trim(),
            LoaiThongBao = cboLoai.SelectedItem?.ToString() ?? "Học vụ",
            DoiTuongNhan = cboDoiTuong.SelectedItem?.ToString() ?? "Toàn trung tâm",
            NoiDung = txtNoiDung.Text.Trim(),
            NguoiGui = "Ban Quản Trị Trung Tâm",
            TrangThai = trangThai
        };

        MockDataStore.AddNotification(notif);
        DialogResult = DialogResult.OK;
    }
}

