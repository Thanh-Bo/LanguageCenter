using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.UserControls;

/// <summary>
/// SAO LƯU &amp; PHỤC HỒI cơ sở dữ liệu.
/// <para>
/// Gọi <c>GET/POST /api/admin/database/backups</c> và <c>POST /api/admin/database/restore</c>.
/// Chỉ tài khoản Quản trị viên mới thấy mục này trên thanh điều hướng.
/// </para>
/// </summary>
public sealed class UcSaoLuu : UserControl
{
    private readonly DataGridView _grid = new();
    private readonly Label _lblThongTin = new();
    private readonly Label _lblThongBao = new();
    private readonly TextBox _txtThuMuc = UiHelpers.CreateTextBox(560);
    private readonly Button _btnSaoLuu = UiHelpers.CreatePrimaryButton("Sao lưu ngay", 150, 36);
    private readonly Button _btnLamMoi = UiHelpers.CreateSecondaryButton("Làm mới", 110, 36);
    private readonly Button _btnPhucHoi = UiHelpers.CreateDangerButton("Phục hồi từ file đã chọn", 210, 36);
    private string? _thuMucMacDinh;

    public UcSaoLuu()
    {
        Dock = DockStyle.Fill;
        BackColor = Theme.ContentBackground;
        Font = Theme.FontRegular;

        DungGiaoDien();

        Load += async (_, _) => await NapThongTinAsync();
    }

    private void DungGiaoDien()
    {
        var header = new Panel { Dock = DockStyle.Top, Height = 92, BackColor = Color.White, Padding = new Padding(24, 16, 24, 12) };

        header.Controls.Add(new Label
        {
            Text = "SAO LƯU & PHỤC HỒI CƠ SỞ DỮ LIỆU",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 14)
        });

        _lblThongTin.AutoSize = true;
        _lblThongTin.Font = Theme.FontSub;
        _lblThongTin.ForeColor = Theme.Muted;
        _lblThongTin.Location = new Point(24, 46);
        _lblThongTin.Text = "Đang đọc thông tin database...";
        header.Controls.Add(_lblThongTin);

        Controls.Add(header);

        // ----- Thanh công cụ -----
        var toolbar = new Panel { Dock = DockStyle.Top, Height = 54, BackColor = Color.White, Padding = new Padding(24, 9, 24, 9) };

        toolbar.Controls.Add(new Label
        {
            Text = "Thư mục lưu:",
            AutoSize = true,
            Font = Theme.FontSub,
            ForeColor = Theme.TextMedium,
            Location = new Point(24, 17)
        });

        _txtThuMuc.Location = new Point(110, 12);
        _txtThuMuc.PlaceholderText = "Bỏ trống = thư mục Backups mặc định của API";
        toolbar.Controls.Add(_txtThuMuc);

        _btnSaoLuu.Location = new Point(690, 9);
        _btnSaoLuu.Click += async (_, _) => await SaoLuuAsync();
        toolbar.Controls.Add(_btnSaoLuu);

        _btnLamMoi.Location = new Point(848, 9);
        _btnLamMoi.Click += async (_, _) => await NapDanhSachAsync();
        toolbar.Controls.Add(_btnLamMoi);

        Controls.Add(toolbar);

        // ----- Lưới danh sách file sao lưu -----
        _grid.Dock = DockStyle.Fill;
        _grid.ReadOnly = true;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.RowHeadersVisible = false;
        _grid.MultiSelect = false;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.BackgroundColor = Color.White;
        _grid.GridColor = Theme.BorderColor;
        _grid.ColumnHeadersDefaultCellStyle.Font = Theme.FontBold;
        _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        _grid.ColumnHeadersHeight = 38;
        _grid.RowTemplate.Height = 30;
        _grid.DefaultCellStyle.Font = Theme.FontRegular;

        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenFile", HeaderText = "Tên file", FillWeight = 260 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "KichThuoc", HeaderText = "Kích thước", FillWeight = 100 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "ThoiDiem", HeaderText = "Thời điểm", FillWeight = 140 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "DuongDan", HeaderText = "Đường dẫn", FillWeight = 320 });

        var gridHost = new Panel { Dock = DockStyle.Fill, Padding = new Padding(24, 12, 24, 12), BackColor = Theme.ContentBackground };
        _grid.Dock = DockStyle.Fill;
        gridHost.Controls.Add(_grid);
        Controls.Add(gridHost);

        // ----- Chân trang: nút phục hồi + thông báo -----
        var footer = new Panel { Dock = DockStyle.Bottom, Height = 76, BackColor = Color.White, Padding = new Padding(24, 12, 24, 12) };

        _btnPhucHoi.Location = new Point(24, 14);
        _btnPhucHoi.Click += async (_, _) => await PhucHoiAsync();
        footer.Controls.Add(_btnPhucHoi);

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(760, 44);
        _lblThongBao.Font = Theme.FontSub;
        _lblThongBao.ForeColor = Theme.Muted;
        _lblThongBao.Location = new Point(250, 18);
        _lblThongBao.Text = "Chọn một file trong bảng rồi bấm 'Phục hồi' — dữ liệu hiện tại sẽ bị THAY THẾ.";
        footer.Controls.Add(_lblThongBao);

        Controls.Add(footer);
    }

    private async Task NapThongTinAsync()
    {
        UseWaitCursor = true;

        var info = await AppSession.Api.GetAsync<DatabaseInfoDto>("admin/database/info");

        UseWaitCursor = false;

        if (info.Success && info.Data is not null)
        {
            _thuMucMacDinh = info.Data.ThuMucMacDinh;
            _lblThongTin.Text = $"Database: {info.Data.TenDatabase}   •   Thư mục mặc định: {info.Data.ThuMucMacDinh}";
            _txtThuMuc.Text = info.Data.ThuMucMacDinh;
        }
        else
        {
            _lblThongTin.Text = info.ErrorMessage;
        }

        await NapDanhSachAsync();
    }

    private async Task NapDanhSachAsync()
    {
        _grid.Rows.Clear();
        UseWaitCursor = true;

        var thuMuc = _txtThuMuc.Text.Trim();

        var result = await AppSession.Api.GetAsync<BackupListDto>(
            QueryStringBuilder.Build("admin/database/backups",
                ("thuMuc", string.IsNullOrWhiteSpace(thuMuc) ? null : thuMuc)));

        UseWaitCursor = false;

        if (!result.Success || result.Data is null)
        {
            _lblThongBao.ForeColor = Theme.Danger;
            _lblThongBao.Text = result.ErrorMessage;
            return;
        }

        foreach (var f in result.Data.Items)
        {
            var idx = _grid.Rows.Add(
                f.TenFile,
                $"{f.KichThuocMb:N2} MB",
                f.ThoiDiem.ToString("dd/MM/yyyy HH:mm:ss"),
                f.DuongDan);

            _grid.Rows[idx].Tag = f.DuongDan;
        }

        _lblThongBao.ForeColor = Theme.Muted;
        _lblThongBao.Text = result.Data.Items.Count == 0
            ? "Chưa có bản sao lưu nào trong thư mục này."
            : $"Tìm thấy {result.Data.Items.Count} bản sao lưu trong {result.Data.ThuMuc}.";
    }

    private async Task SaoLuuAsync()
    {
        var thuMuc = _txtThuMuc.Text.Trim();

        UseWaitCursor = true;
        _btnSaoLuu.Enabled = false;

        var result = await AppSession.Api.PostAsync<BackupResultDto>("admin/database/backups", new
        {
            thuMuc = string.IsNullOrWhiteSpace(thuMuc) ? null : thuMuc
        });

        _btnSaoLuu.Enabled = true;
        UseWaitCursor = false;

        if (!result.Success || result.Data is null)
        {
            _lblThongBao.ForeColor = Theme.Danger;
            _lblThongBao.Text = result.ErrorMessage;
            return;
        }

        _lblThongBao.ForeColor = Theme.Success;
        _lblThongBao.Text = $"{result.Data.Message} → {result.Data.TenFile}";

        await NapDanhSachAsync();
    }

    private async Task PhucHoiAsync()
    {
        if (_grid.CurrentRow?.Tag is not string duongDan)
        {
            MessageBox.Show(FindForm(), "Vui lòng chọn một file sao lưu trong bảng.",
                "Chưa chọn file", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var xacNhan = MessageBox.Show(FindForm(),
            "PHỤC HỒI SẼ THAY THẾ TOÀN BỘ DỮ LIỆU HIỆN TẠI bằng nội dung của file:\n\n"
            + duongDan
            + "\n\nMọi dữ liệu phát sinh sau thời điểm sao lưu sẽ MẤT và không thể hoàn tác.\n"
            + "Bạn có chắc chắn muốn tiếp tục?",
            "Xác nhận phục hồi database",
            MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

        if (xacNhan != DialogResult.Yes)
        {
            return;
        }

        UseWaitCursor = true;
        _btnPhucHoi.Enabled = false;

        var result = await AppSession.Api.PostAsync<BackupResultDto>("admin/database/restore", new
        {
            duongDan
        });

        _btnPhucHoi.Enabled = true;
        UseWaitCursor = false;

        if (!result.Success)
        {
            _lblThongBao.ForeColor = Theme.Danger;
            _lblThongBao.Text = result.ErrorMessage;
            return;
        }

        _lblThongBao.ForeColor = Theme.Success;
        _lblThongBao.Text = result.Data?.Message ?? "Phục hồi thành công.";

        MessageBox.Show(FindForm(),
            "Phục hồi thành công.\n\nHãy ĐÓNG và CHẠY LẠI API + ứng dụng để làm mới kết nối dữ liệu.",
            "Hoàn tất", MessageBoxButtons.OK, MessageBoxIcon.Information);

        await NapDanhSachAsync();
    }
}

// ===== DTO nội bộ cho API sao lưu =====
public sealed class DatabaseInfoDto
{
    public string TenDatabase { get; init; } = string.Empty;
    public string ThuMucMacDinh { get; init; } = string.Empty;
}

public sealed class BackupFileDto
{
    public string TenFile { get; init; } = string.Empty;
    public string DuongDan { get; init; } = string.Empty;
    public long KichThuocByte { get; init; }
    public double KichThuocMb { get; init; }
    public DateTime ThoiDiem { get; init; }
}

public sealed class BackupListDto
{
    public string ThuMuc { get; init; } = string.Empty;
    public IReadOnlyList<BackupFileDto> Items { get; init; } = Array.Empty<BackupFileDto>();
}

public sealed class BackupResultDto
{
    public string Message { get; init; } = string.Empty;
    public string? DuongDan { get; init; }
    public string? TenFile { get; init; }
}
