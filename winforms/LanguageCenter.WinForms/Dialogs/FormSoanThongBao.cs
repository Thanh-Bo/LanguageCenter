using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

public partial class FormSoanThongBao : Form
{
    private IReadOnlyList<ClassListItemDto> _classes = Array.Empty<ClassListItemDto>();

    public FormSoanThongBao()
    {
        InitializeComponent();

        btnSend.DialogResult = DialogResult.None;
        btnDraft.Visible = false; // Backend không có trạng thái "Bản nháp" cho thông báo

        btnSend.Click += async (_, _) => await SaveNotificationAsync();
        btnCancel.Click += (_, _) => DialogResult = DialogResult.Cancel;

        cboLoai.Items.Clear();
        cboLoai.Items.AddRange(["Lịch học", "Học tập", "Học phí", "Khác"]);
        cboLoai.SelectedIndex = 1;

        Load += async (_, _) => await LoadClassesAsync();
    }

    /// <summary>
    /// Thông báo trong DB phải có người nhận cụ thể: MaTaiKhoanNhan HOẶC MaLopHoc.
    /// Bản WinForms gửi theo lớp nên ô "Đối tượng nhận" được dùng làm danh sách lớp.
    /// </summary>
    private async Task LoadClassesAsync()
    {
        btnSend.Enabled = false;
        UseWaitCursor = true;

        var result = await AppSession.Api.GetAsync<PagedResult<ClassListItemDto>>(
            QueryStringBuilder.Build("admin/classes", ("page", "1"), ("pageSize", "500")));

        UseWaitCursor = false;
        btnSend.Enabled = true;

        _classes = (result.Data?.Items ?? Array.Empty<ClassListItemDto>())
            .OrderBy(c => c.MaLop)
            .ToList();

        cboDoiTuong.Items.Clear();
        foreach (var c in _classes)
        {
            cboDoiTuong.Items.Add($"{c.MaLop} — {c.TenLop}");
        }
        if (cboDoiTuong.Items.Count > 0) cboDoiTuong.SelectedIndex = 0;

        if (!result.Success)
        {
            MessageBox.Show(this, result.ErrorMessage, "Không tải được lớp học", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task SaveNotificationAsync()
    {
        if (string.IsNullOrWhiteSpace(txtTieuDe.Text) || string.IsNullOrWhiteSpace(txtNoiDung.Text))
        {
            MessageBox.Show(this, "Vui lòng nhập Tiêu đề và Nội dung thông báo.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cboDoiTuong.SelectedIndex < 0 || cboDoiTuong.SelectedIndex >= _classes.Count)
        {
            MessageBox.Show(this, "Vui lòng chọn lớp nhận thông báo.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var lop = _classes[cboDoiTuong.SelectedIndex];

        btnSend.Enabled = false;
        UseWaitCursor = true;

        var result = await AppSession.Api.PostAsync<NotificationListItemDto>("admin/notifications", new
        {
            maLopHoc = lop.MaLopHoc,
            tieuDe = txtTieuDe.Text.Trim(),
            noiDung = txtNoiDung.Text.Trim(),
            loaiThongBao = cboLoai.SelectedItem?.ToString() ?? "Học tập"
        });

        UseWaitCursor = false;
        btnSend.Enabled = true;

        if (!result.Success)
        {
            MessageBox.Show(this, result.ErrorMessage, "Gửi thông báo thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        DialogResult = DialogResult.OK;
    }
}

