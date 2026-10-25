using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Quản lý NGÀY LỄ / NGÀY NGHỈ của trung tâm.
/// <para>
/// Khi bấm "Sinh ca học" ở màn Lịch học, buổi nào rơi vào ngày lễ sẽ tự dời
/// sang tuần sau (cùng thứ) và các buổi phía sau dời theo để giữ 2 buổi/tuần.
/// </para>
/// Gọi <c>GET|POST|PATCH|DELETE /api/admin/holidays</c>.
/// </summary>
public sealed class FormNgayLe : Form
{
    private readonly DataGridView _grid = new();
    private readonly DateTimePicker _dtpNgay = new();
    private readonly TextBox _txtTen = UiHelpers.CreateTextBox(320);
    private readonly TextBox _txtGhiChu = UiHelpers.CreateTextBox(320);
    private readonly Label _lblThongBao = new();
    private readonly Label _lblDem = new();

    private readonly Button _btnThem = UiHelpers.CreatePrimaryButton("Thêm mới", 130, 36);
    private readonly Button _btnCapNhat = UiHelpers.CreateSecondaryButton("Cập nhật", 120, 36);
    private readonly Button _btnXoa = UiHelpers.CreateDangerButton("Xóa", 100, 36);
    private readonly Button _btnDong = UiHelpers.CreateSecondaryButton("Đóng", 100, 36);

    private IReadOnlyList<HolidayDto> _items = Array.Empty<HolidayDto>();
    private HolidayDto? _dangChon;

    public bool DataChanged { get; private set; }

    public FormNgayLe()
    {
        Text = "Quản lý ngày lễ";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.Sizable;
        MinimumSize = new Size(860, 560);
        ClientSize = new Size(900, 620);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();

        Load += async (_, _) => await NapAsync();
    }

    private void DungGiaoDien()
    {
        Controls.Add(new Label
        {
            Text = "QUẢN LÝ NGÀY LỄ",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 18)
        });

        Controls.Add(new Label
        {
            Text = "Buổi học rơi vào ngày lễ sẽ tự dời sang tuần sau khi bấm 'Sinh ca học'.",
            Font = Theme.FontSub,
            ForeColor = Theme.Muted,
            AutoSize = true,
            Location = new Point(24, 46)
        });

        _grid.Location = new Point(24, 76);
        _grid.Size = new Size(852, 300);
        _grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
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
        _grid.DefaultCellStyle.SelectionBackColor = Theme.PrimaryLight;
        _grid.DefaultCellStyle.SelectionForeColor = Theme.TextDark;

        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Ngay", HeaderText = "Ngày", FillWeight = 80 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenNgayLe", HeaderText = "Tên ngày lễ", FillWeight = 220 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "GhiChu", HeaderText = "Ghi chú", FillWeight = 240 });

        _grid.SelectionChanged += (_, _) => ChonDong();
        Controls.Add(_grid);

        // ----- Khung nhập liệu -----
        var y = 396;

        Controls.Add(new Label
        {
            Text = "Ngày *",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, y)
        });

        _dtpNgay.Format = DateTimePickerFormat.Short;
        _dtpNgay.Width = 150;
        _dtpNgay.Location = new Point(24, y + 20);
        Controls.Add(_dtpNgay);

        Controls.Add(new Label
        {
            Text = "Tên ngày lễ *",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(196, y)
        });

        _txtTen.Location = new Point(196, y + 20);
        _txtTen.PlaceholderText = "Ví dụ: Tết Dương lịch, Quốc khánh 2/9...";
        Controls.Add(_txtTen);

        Controls.Add(new Label
        {
            Text = "Ghi chú",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(536, y)
        });

        _txtGhiChu.Location = new Point(536, y + 20);
        _txtGhiChu.PlaceholderText = "Trung tâm nghỉ toàn bộ";
        Controls.Add(_txtGhiChu);

        _lblDem.AutoSize = true;
        _lblDem.Font = Theme.FontSub;
        _lblDem.ForeColor = Theme.Muted;
        _lblDem.Location = new Point(24, y + 54);
        Controls.Add(_lblDem);

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(600, 20);
        _lblThongBao.Font = Theme.FontSub;
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Location = new Point(24, y + 76);
        Controls.Add(_lblThongBao);

        _btnThem.Location = new Point(24, y + 104);
        _btnThem.Click += async (_, _) => await ThemAsync();
        Controls.Add(_btnThem);

        _btnCapNhat.Location = new Point(166, y + 104);
        _btnCapNhat.Click += async (_, _) => await CapNhatAsync();
        Controls.Add(_btnCapNhat);

        _btnXoa.Location = new Point(298, y + 104);
        _btnXoa.Click += async (_, _) => await XoaAsync();
        Controls.Add(_btnXoa);

        _btnDong.Location = new Point(776, y + 104);
        _btnDong.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
        _btnDong.Click += (_, _) => Close();
        Controls.Add(_btnDong);
    }

    /// <summary>Nạp danh sách ngày lễ từ API.</summary>
    private async Task NapAsync()
    {
        _lblThongBao.Text = string.Empty;
        UseWaitCursor = true;

        var result = await AppSession.Api.GetAsync<IReadOnlyList<HolidayDto>>("admin/holidays");

        UseWaitCursor = false;

        if (!result.Success || result.Data is null)
        {
            _lblThongBao.Text = result.ErrorMessage;
            return;
        }

        _items = result.Data;

        _grid.Rows.Clear();

        foreach (var h in _items)
        {
            var rowIdx = _grid.Rows.Add(
                h.Ngay.ToString("dd/MM/yyyy"),
                h.TenNgayLe,
                string.IsNullOrWhiteSpace(h.GhiChu) ? "—" : h.GhiChu);

            _grid.Rows[rowIdx].Tag = h;
        }

        _lblDem.Text = $"Tổng {_items.Count} ngày lễ trong hệ thống.";
        _lblDem.ForeColor = Theme.Muted;

        if (_grid.Rows.Count > 0)
        {
            _grid.Rows[0].Selected = true;
            _grid.CurrentCell = _grid.Rows[0].Cells["Ngay"];
        }
        else
        {
            _dangChon = null;
        }
    }

    private void ChonDong()
    {
        if (_grid.CurrentRow?.Tag is not HolidayDto h)
        {
            return;
        }

        _dangChon = h;
        _dtpNgay.Value = h.Ngay.ToDateTime(TimeOnly.MinValue);
        _txtTen.Text = h.TenNgayLe;
        _txtGhiChu.Text = h.GhiChu ?? string.Empty;
        _lblThongBao.Text = string.Empty;
    }

    private bool KiemTraHopLe(out DateOnly ngay, out string ten)
    {
        ngay = DateOnly.FromDateTime(_dtpNgay.Value);
        ten = _txtTen.Text.Trim();

        if (ten.Length == 0)
        {
            _lblThongBao.Text = "Vui lòng nhập tên ngày lễ.";
            return false;
        }

        return true;
    }

    private async Task ThemAsync()
    {
        if (!KiemTraHopLe(out var ngay, out var ten))
        {
            return;
        }

        UseWaitCursor = true;

        var result = await AppSession.Api.PostAsync<HolidayDto>("admin/holidays", new
        {
            ngay = ngay.ToString("yyyy-MM-dd"),
            tenNgayLe = ten,
            ghiChu = _txtGhiChu.Text.Trim()
        });

        UseWaitCursor = false;

        if (!result.Success)
        {
            _lblThongBao.Text = result.ThongDiepDayDu();
            return;
        }

        DataChanged = true;
        await NapAsync();
    }

    private async Task CapNhatAsync()
    {
        if (_dangChon is null)
        {
            _lblThongBao.Text = "Vui lòng chọn một ngày lễ trong bảng để cập nhật.";
            return;
        }

        if (!KiemTraHopLe(out var ngay, out var ten))
        {
            return;
        }

        UseWaitCursor = true;

        var result = await AppSession.Api.PatchAsync<HolidayDto>(
            $"admin/holidays/{_dangChon.MaNgayLe}",
            new
            {
                ngay = ngay.ToString("yyyy-MM-dd"),
                tenNgayLe = ten,
                ghiChu = _txtGhiChu.Text.Trim()
            });

        UseWaitCursor = false;

        if (!result.Success)
        {
            _lblThongBao.Text = result.ThongDiepDayDu();
            return;
        }

        DataChanged = true;
        await NapAsync();
    }

    private async Task XoaAsync()
    {
        if (_dangChon is null)
        {
            _lblThongBao.Text = "Vui lòng chọn một ngày lễ trong bảng để xóa.";
            return;
        }

        var xacNhan = MessageBox.Show(FindForm(),
            $"Xóa ngày lễ {_dangChon.Ngay:dd/MM/yyyy} — {_dangChon.TenNgayLe}?",
            "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (xacNhan != DialogResult.Yes)
        {
            return;
        }

        UseWaitCursor = true;

        var result = await AppSession.Api.DeleteAsync<HolidayDto>($"admin/holidays/{_dangChon.MaNgayLe}");

        UseWaitCursor = false;

        if (!result.Success)
        {
            _lblThongBao.Text = result.ThongDiepDayDu();
            return;
        }

        DataChanged = true;
        await NapAsync();
    }
}
