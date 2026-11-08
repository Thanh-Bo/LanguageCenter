using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Quản lý ĐẦU ĐIỂM (loại điểm + trọng số) của một khóa học.
/// <para>
/// Gọi <c>/api/admin/grade-types</c>: GET (theo khóa học), POST, PATCH, PATCH .../status, DELETE.
/// Trọng số của tất cả đầu điểm đang hoạt động phải cộng lại = 100%.
/// </para>
/// </summary>
public sealed class FormLoaiDiem : Form
{
    private readonly IReadOnlyList<CourseListItemDto> _courses;

    private readonly ComboBox _cboKhoaHoc = UiHelpers.CreateComboBox(420);
    private readonly DataGridView _grid = new();

    private readonly TextBox _txtTen = UiHelpers.CreateTextBox(300);
    private readonly NumericUpDown _numTrongSo = new() { Width = 110, Minimum = 0, Maximum = 100, DecimalPlaces = 2, Increment = 5 };
    private readonly NumericUpDown _numDiemToiDa = new() { Width = 110, Minimum = 0.01m, Maximum = 1000, DecimalPlaces = 2, Value = 10 };
    private readonly NumericUpDown _numThuTu = new() { Width = 90, Minimum = 1, Maximum = 99, Value = 1 };

    private readonly Label _lblThongBao = new();
    private readonly Label _lblTongTrongSo = new();

    private readonly Button _btnThem = UiHelpers.CreatePrimaryButton("Thêm đầu điểm", 150, 34);
    private readonly Button _btnSua = UiHelpers.CreateSecondaryButton("Cập nhật", 120, 34);
    private readonly Button _btnNgung = UiHelpers.CreateSecondaryButton("Ngừng / Mở lại", 140, 34);
    private readonly Button _btnXoa = UiHelpers.CreateDangerButton("Xoá", 90, 34);
    private readonly Button _btnLamMoi = UiHelpers.CreateSecondaryButton("Làm mới", 110, 34);

    private IReadOnlyList<LoaiDiemDto> _items = Array.Empty<LoaiDiemDto>();
    private LoaiDiemDto? _dangChon;

    public bool DataChanged { get; private set; }

    public FormLoaiDiem(IReadOnlyList<CourseListItemDto> courses, int? maKhoaHocMacDinh = null)
    {
        _courses = courses;

        Text = "Quản lý đầu điểm & trọng số";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.Sizable;
        MinimumSize = new Size(920, 620);
        ClientSize = new Size(980, 700);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        UI.KiemTraNhap.BatTuNoiChieuCao(this);
        NapKhoaHoc(maKhoaHocMacDinh);

        Load += async (_, _) => await NapAsync();
    }

    private void DungGiaoDien()
    {
        Controls.Add(new Label
        {
            Text = "ĐẦU ĐIỂM & TRỌNG SỐ",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 16)
        });

        Controls.Add(new Label
        {
            Text = "Tổng trọng số của các đầu điểm đang hoạt động phải bằng 100%.",
            Font = Theme.FontSub,
            ForeColor = Theme.Muted,
            AutoSize = true,
            Location = new Point(24, 44)
        });

        Controls.Add(new Label
        {
            Text = "Khóa học",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, 74)
        });

        _cboKhoaHoc.Location = new Point(24, 94);
        _cboKhoaHoc.SelectedIndexChanged += async (_, _) => await NapAsync();
        Controls.Add(_cboKhoaHoc);

        _lblTongTrongSo.AutoSize = true;
        _lblTongTrongSo.Location = new Point(470, 98);
        _lblTongTrongSo.Font = Theme.FontBold;
        _lblTongTrongSo.ForeColor = Theme.Primary;
        Controls.Add(_lblTongTrongSo);

        _grid.Location = new Point(24, 132);
        _grid.Size = new Size(932, 300);
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

        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "ThuTu", HeaderText = "Thứ tự", FillWeight = 60 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "TenLoaiDiem", HeaderText = "Tên đầu điểm", FillWeight = 200 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrongSo", HeaderText = "Trọng số (%)", FillWeight = 90 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "DiemToiDa", HeaderText = "Điểm tối đa", FillWeight = 90 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", FillWeight = 100 });

        _grid.SelectionChanged += (_, _) => ChonDong();
        Controls.Add(_grid);

        var y = 452;

        Controls.Add(new Label
        {
            Text = "Tên đầu điểm *",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, y)
        });
        _txtTen.Location = new Point(24, y + 20);
        Controls.Add(_txtTen);

        Controls.Add(new Label
        {
            Text = "Trọng số %",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(344, y)
        });
        _numTrongSo.Location = new Point(344, y + 22);
        Controls.Add(_numTrongSo);

        Controls.Add(new Label
        {
            Text = "Điểm tối đa",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(474, y)
        });
        _numDiemToiDa.Location = new Point(474, y + 22);
        Controls.Add(_numDiemToiDa);

        Controls.Add(new Label
        {
            Text = "Thứ tự",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(604, y)
        });
        _numThuTu.Location = new Point(604, y + 22);
        Controls.Add(_numThuTu);

        _btnThem.Location = new Point(24, y + 62);
        _btnThem.Click += async (_, _) => await ThemAsync();
        Controls.Add(_btnThem);

        _btnSua.Location = new Point(182, y + 62);
        _btnSua.Click += async (_, _) => await SuaAsync();
        Controls.Add(_btnSua);

        _btnNgung.Location = new Point(310, y + 62);
        _btnNgung.Click += async (_, _) => await DoiTrangThaiAsync();
        Controls.Add(_btnNgung);

        _btnXoa.Location = new Point(458, y + 62);
        _btnXoa.Click += async (_, _) => await XoaAsync();
        Controls.Add(_btnXoa);

        _btnLamMoi.Location = new Point(556, y + 62);
        _btnLamMoi.Click += async (_, _) => await NapAsync();
        Controls.Add(_btnLamMoi);

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(860, 44);
        _lblThongBao.Location = new Point(24, y + 106);
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Font = Theme.FontSub;
        Controls.Add(_lblThongBao);

        var btnDong = UiHelpers.CreateSecondaryButton("Đóng", 100, 34);
        btnDong.Location = new Point(856, y + 62);
        btnDong.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnDong.Click += (_, _) => Close();
        Controls.Add(btnDong);
    }

    private void NapKhoaHoc(int? maChon)
    {
        _cboKhoaHoc.Items.Clear();

        foreach (var c in _courses.OrderByDescending(c => c.MaKhoaHoc))
        {
            _cboKhoaHoc.Items.Add(new KhoaHocItem(c.MaKhoaHoc, c.TenKhoaHoc, c.SoBuoi));
        }

        if (_cboKhoaHoc.Items.Count == 0)
        {
            _lblThongBao.Text = "Chưa có khóa học nào. Hãy tạo khóa học trước khi khai báo đầu điểm.";
            _btnThem.Enabled = false;
            return;
        }

        var viTri = 0;

        if (maChon is { } ma)
        {
            for (var i = 0; i < _cboKhoaHoc.Items.Count; i++)
            {
                if (_cboKhoaHoc.Items[i] is KhoaHocItem it && it.MaKhoaHoc == ma)
                {
                    viTri = i;
                    break;
                }
            }
        }

        _cboKhoaHoc.SelectedIndex = viTri;
    }

    private async Task NapAsync()
    {
        if (_cboKhoaHoc.SelectedItem is not KhoaHocItem khoaHoc)
        {
            return;
        }

        UseWaitCursor = true;

        var result = await AppSession.Api.GetAsync<PagedResult<LoaiDiemDto>>(
            QueryStringBuilder.Build("admin/grade-types",
                ("maKhoaHoc", khoaHoc.MaKhoaHoc.ToString()),
                ("page", "1"),
                ("pageSize", "200")));

        UseWaitCursor = false;

        _grid.Rows.Clear();

        if (!result.Success || result.Data is null)
        {
            _lblThongBao.Text = result.ErrorMessage;
            return;
        }

        _items = result.Data.Items;
        _dangChon = null;

        foreach (var item in _items.OrderBy(x => x.ThuTu))
        {
            var idx = _grid.Rows.Add(
                item.ThuTu,
                item.TenLoaiDiem,
                $"{item.TrongSo:0.##}%",
                item.DiemToiDa.ToString("0.##"),
                item.TrangThai);

            _grid.Rows[idx].Tag = item;
            _grid.Rows[idx].Cells["TrangThai"].Style.ForeColor =
                item.TrangThai == "Hoạt động" ? Theme.Success : Theme.Muted;

            if (item.TrangThai != "Hoạt động")
            {
                _grid.Rows[idx].DefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            }
        }

        var tong = _items.Where(x => x.TrangThai == "Hoạt động").Sum(x => x.TrongSo);
        _lblTongTrongSo.Text = $"Tổng trọng số: {tong:0.##}%";
        _lblTongTrongSo.ForeColor = Math.Abs(tong - 100m) < 0.01m ? Theme.Success : Theme.Danger;

        _lblThongBao.Text = _items.Count == 0
            ? $"Khóa '{khoaHoc.TenKhoaHoc}' chưa có đầu điểm nào — hãy thêm để nhập được điểm."
            : Math.Abs(tong - 100m) < 0.01m
                ? string.Empty
                : $"⚠ Tổng trọng số đang là {tong:0.##}% (phải bằng 100%).";

        _lblThongBao.ForeColor = Math.Abs(tong - 100m) < 0.01m ? Theme.Muted : Theme.Danger;
    }

    private void ChonDong()
    {
        if (_grid.CurrentRow?.Tag is not LoaiDiemDto item)
        {
            return;
        }

        _dangChon = item;
        _txtTen.Text = item.TenLoaiDiem;
        _numTrongSo.Value = Math.Clamp(item.TrongSo, 0, 100);
        _numDiemToiDa.Value = Math.Clamp(item.DiemToiDa, 0.01m, 1000);
        _numThuTu.Value = Math.Clamp(item.ThuTu, 1, 99);
        _lblThongBao.Text = string.Empty;
    }

    private async Task ThemAsync()
    {
        if (_cboKhoaHoc.SelectedItem is not KhoaHocItem khoaHoc)
        {
            return;
        }

        if (!KiemTraNhap(out var ten))
        {
            return;
        }

        UseWaitCursor = true;

        var result = await AppSession.Api.PostAsync<LoaiDiemDto>("admin/grade-types", new
        {
            maKhoaHoc = khoaHoc.MaKhoaHoc,
            tenLoaiDiem = ten,
            trongSo = _numTrongSo.Value,
            diemToiDa = _numDiemToiDa.Value,
            thuTu = (int)_numThuTu.Value
        });

        UseWaitCursor = false;

        if (!result.Success)
        {
            _lblThongBao.Text = HienThiLoi(result);
            return;
        }

        DataChanged = true;
        await NapAsync();
    }

    private async Task SuaAsync()
    {
        if (_dangChon is null)
        {
            _lblThongBao.Text = "Vui lòng chọn một đầu điểm trong bảng để cập nhật.";
            return;
        }

        if (!KiemTraNhap(out var ten))
        {
            return;
        }

        UseWaitCursor = true;

        var result = await AppSession.Api.PatchAsync<LoaiDiemDto>(
            $"admin/grade-types/{_dangChon.MaLoaiDiem}",
            new
            {
                tenLoaiDiem = ten,
                trongSo = _numTrongSo.Value,
                diemToiDa = _numDiemToiDa.Value,
                thuTu = (int)_numThuTu.Value
            });

        UseWaitCursor = false;

        if (!result.Success)
        {
            _lblThongBao.Text = HienThiLoi(result);
            return;
        }

        DataChanged = true;
        await NapAsync();
    }

    private async Task DoiTrangThaiAsync()
    {
        if (_dangChon is null)
        {
            _lblThongBao.Text = "Vui lòng chọn một đầu điểm trong bảng.";
            return;
        }

        var trangThaiMoi = _dangChon.TrangThai == "Hoạt động" ? "Ngừng sử dụng" : "Hoạt động";

        UseWaitCursor = true;

        var result = await AppSession.Api.PatchAsync<LoaiDiemDto>(
            $"admin/grade-types/{_dangChon.MaLoaiDiem}/status",
            new { trangThai = trangThaiMoi });

        UseWaitCursor = false;

        if (!result.Success)
        {
            _lblThongBao.Text = HienThiLoi(result);
            return;
        }

        DataChanged = true;
        await NapAsync();
    }

    private async Task XoaAsync()
    {
        if (_dangChon is null)
        {
            _lblThongBao.Text = "Vui lòng chọn một đầu điểm trong bảng để xoá.";
            return;
        }

        var xacNhan = MessageBox.Show(FindForm(),
            $"Xoá đầu điểm '{_dangChon.TenLoaiDiem}'?\n\n"
            + "Chỉ xoá được khi chưa có điểm nào được nhập cho đầu điểm này.",
            "Xác nhận xoá", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);

        if (xacNhan != DialogResult.Yes)
        {
            return;
        }

        UseWaitCursor = true;

        var result = await AppSession.Api.DeleteAsync<LoaiDiemDto>($"admin/grade-types/{_dangChon.MaLoaiDiem}");

        UseWaitCursor = false;

        if (!result.Success)
        {
            _lblThongBao.Text = HienThiLoi(result);
            return;
        }

        DataChanged = true;
        await NapAsync();
    }

    private bool KiemTraNhap(out string ten)
    {
        _lblThongBao.Text = string.Empty;
        ten = _txtTen.Text.Trim();

        if (ten.Length < 2)
        {
            _lblThongBao.Text = "Tên đầu điểm phải có ít nhất 2 ký tự.";
            return false;
        }

        return true;
    }

    private static string HienThiLoi<T>(ApiResult<T> result)
        => result.Errors.Count > 0
            ? $"{result.ErrorMessage}\n- {string.Join("\n- ", result.Errors)}"
            : (string.IsNullOrWhiteSpace(result.ErrorMessage) ? "Thao tác không thành công." : result.ErrorMessage);

    private sealed record KhoaHocItem(int MaKhoaHoc, string TenKhoaHoc, int SoBuoi)
    {
        public override string ToString() => $"{TenKhoaHoc} ({SoBuoi} buổi)";
    }
}
