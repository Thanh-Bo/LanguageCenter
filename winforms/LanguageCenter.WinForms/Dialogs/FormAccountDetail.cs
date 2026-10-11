using LanguageCenter.WinForms.Api;

namespace LanguageCenter.WinForms.Dialogs;

/// Xem chi tiết một tài khoản, đổi trạng thái, đặt lại mật khẩu, xóa (nếu không có dữ liệu phụ thuộc).
public partial class FormAccountDetail : Form
{
    private readonly int _accountId;

    /// True nếu danh sách phía sau cần được tải lại (có thay đổi trạng thái hoặc đã xóa).
    public bool DataChanged { get; private set; }

    public FormAccountDetail(int accountId)
    {
        _accountId = accountId;
        InitializeComponent();

        cboStatus.Items.AddRange(AccountStatuses.All.ToArray());
    }

    private async void FormAccountDetail_Load(object? sender, EventArgs e)
    {
        await LoadDetailAsync();
    }

    private async Task LoadDetailAsync()
    {
        SetBusy(true);

        var result = await AppSession.Api.GetAsync<AccountDetailDto>($"admin/accounts/{_accountId}");

        SetBusy(false);

        if (!result.Success || result.Data is null)
        {
            lblMessage.Text = result.ErrorMessage;

            if (result.IsNotFound)
            {
                DataChanged = true;
            }

            return;
        }

        var account = result.Data;

        lblUsernameValue.Text = account.Username;
        lblFullNameValue.Text = string.IsNullOrWhiteSpace(account.FullName) ? "—" : account.FullName;
        lblEmailValue.Text = account.Email;
        lblRoleValue.Text = account.Role;
        lblProfileValue.Text = account.ProfileType switch
        {
            "HocVien" => $"Học viên — {account.ProfileCode} ({account.StudentStatus})",
            "GiaoVien" => $"{account.TeacherType} — {account.ProfileCode}",
            "NhanVien" => $"Nhân viên — {account.ProfileCode}",
            _ => "—"
        };
        lblCreatedValue.Text = UI.UiHelpers.FormatDateTime(account.CreatedAt);
        lblLastLoginValue.Text = UI.UiHelpers.FormatDateTime(account.LastLoginAt);

        cboStatus.SelectedItem = account.Status;
    }

    private async void BtnSaveStatus_Click(object? sender, EventArgs e)
    {
        if (cboStatus.SelectedItem is not string newStatus)
        {
            return;
        }

        SetBusy(true);
        lblMessage.ForeColor = UI.Theme.Danger;
        lblMessage.Text = string.Empty;

        var result = await AppSession.Api.PatchAsync<AccountDetailDto>(
            $"admin/accounts/{_accountId}/status",
            new UpdateAccountStatusRequest { Status = newStatus });

        SetBusy(false);

        if (!result.Success)
        {
            lblMessage.Text = result.ErrorMessage;
            return;
        }

        DataChanged = true;
        lblMessage.ForeColor = UI.Theme.Success;
        lblMessage.Text = "Đã cập nhật trạng thái tài khoản.";
    }

    private async void BtnResetPassword_Click(object? sender, EventArgs e)
    {
        var confirm = MessageBox.Show(
            this,
            $"Đặt lại mật khẩu cho tài khoản \"{lblUsernameValue.Text}\"? "
                + "Mật khẩu hiện tại sẽ không còn dùng được.",
            "Xác nhận đặt lại mật khẩu",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Question);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        SetBusy(true);

        var result = await AppSession.Api.PostAsync<ResetPasswordResponse>($"admin/accounts/{_accountId}/reset-password", null);

        SetBusy(false);

        if (!result.Success || result.Data is null)
        {
            lblMessage.Text = result.ErrorMessage;
            return;
        }

        DataChanged = true;

        using var dialog = new FormSecretResult(
            "Đã đặt lại mật khẩu",
            result.Data.Message,
            result.Data.Username,
            result.Data.TemporaryPassword);
        dialog.ShowDialog(this);
    }

    private async void BtnDelete_Click(object? sender, EventArgs e)
    {
        var confirm = MessageBox.Show(
            this,
            $"Xóa VĨNH VIỄN tài khoản \"{lblUsernameValue.Text}\"?\n\n"
                + "Thao tác này chỉ thực hiện được khi tài khoản không có dữ liệu nghiệp vụ phụ thuộc "
                + "(lớp học, điểm danh, thanh toán...). Nếu có, hệ thống sẽ đề nghị dùng trạng thái "
                + "\"Ngừng sử dụng\" thay thế.",
            "Xác nhận xóa tài khoản",
            MessageBoxButtons.YesNo,
            MessageBoxIcon.Warning);

        if (confirm != DialogResult.Yes)
        {
            return;
        }

        SetBusy(true);
        lblMessage.Text = string.Empty;

        var result = await AppSession.Api.DeleteAsync<AccountDeleteBlockedResponse>($"admin/accounts/{_accountId}");

        SetBusy(false);

        if (result.Success)
        {
            DataChanged = true;
            MessageBox.Show(this, "Đã xóa tài khoản.", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Close();
            return;
        }

        if (result.IsConflict && result.Data is not null)
        {
            MessageBox.Show(
                this,
                $"{result.Data.Message}\n\nLý do: {result.Data.Reason}\n\nGợi ý: {result.Data.Suggestion}",
                "Không thể xóa",
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
            return;
        }

        lblMessage.Text = result.ErrorMessage;
    }

    private void SetBusy(bool busy)
    {
        UseWaitCursor = busy;
        cboStatus.Enabled = !busy;
        btnSaveStatus.Enabled = !busy;
        btnResetPassword.Enabled = !busy;
        btnDelete.Enabled = !busy;
    }
}
