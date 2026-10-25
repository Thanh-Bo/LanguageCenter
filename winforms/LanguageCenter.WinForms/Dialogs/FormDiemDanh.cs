using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Điểm danh một ca học — <c>GET|POST /api/admin/sessions/{id}/attendance</c>.
/// </summary>
public sealed class FormDiemDanh : Form
{
    /// <summary>
    /// 4 trạng thái — PHẢI khớp CHECK constraint <c>CK_DiemDanh_TrangThai</c> của database
    /// và <c>AttendanceStatuses.All</c> ở backend.
    /// Mỗi trạng thái là MỘT CỘT tick, admin chỉ việc bấm vào ô tương ứng.
    /// </summary>
    private static readonly (string TrangThai, string Cot, string TieuDe, int Rong)[] CacTrangThai =
    [
        ("Có mặt",        "CoMat",      "Có mặt",       80),
        ("Đi trễ",        "DiTre",      "Đi trễ",       75),
        ("Vắng có phép", "VangCoPhep", "Vắng có phép", 100),
        ("Vắng",          "Vang",       "Vắng",         65)
    ];

    private static readonly string[] CotTrangThai =
        CacTrangThai.Select(x => x.Cot).ToArray();

    private readonly int _maCaHoc;

    private readonly Label _lblTieuDe = new();
    private readonly Label _lblThongBao = new();
    private readonly DataGridView _grid = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Lưu điểm danh", 150, 36);
    private readonly Button _btnDong = UiHelpers.CreateSecondaryButton("Đóng", 100, 36);

    private AttendanceSheetDto? _sheet;

    private bool _dangCapNhat;

    public bool DataChanged { get; private set; }

    public FormDiemDanh(int maCaHoc)
    {
        _maCaHoc = maCaHoc;

        Text = "Điểm danh ca học";
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(760, 520);
        BackColor = Color.White;
        Font = Theme.FontRegular;
        MinimumSize = new Size(640, 420);

        DungGiaoDien();
        Load += async (_, _) => await TaiAsync();
    }

    private void DungGiaoDien()
    {
        _lblTieuDe.Text = "ĐANG TẢI CA HỌC...";
        _lblTieuDe.Font = Theme.FontHeading;
        _lblTieuDe.ForeColor = Theme.Primary;
        _lblTieuDe.AutoSize = true;
        _lblTieuDe.Location = new Point(20, 16);

        _grid.Location = new Point(20, 56);
        _grid.Size = new Size(720, 380);
        _grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.RowHeadersVisible = false;
        _grid.AutoGenerateColumns = false;
        _grid.SelectionMode = DataGridViewSelectionMode.CellSelect;
        UiHelpers.StyleGrid(_grid);

        // StyleGrid đặt ReadOnly = true → phải mở lại, nếu không sẽ KHÔNG bấm được
        // vào ô Trạng thái để chọn (chỉ thấy giá trị mà không có danh sách xổ xuống).
        _grid.ReadOnly = false;
        _grid.EditMode = DataGridViewEditMode.EditOnEnter;

        // StyleGrid đặt AutoSizeColumnsMode = Fill (bỏ qua Width) → tắt để cột tick đủ rộng,
        // chỉ 2 cột Họ tên / Ghi chú tự co giãn.
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "MaHocVien",
            HeaderText = "Mã HV",
            ReadOnly = true,
            Width = 90
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "HoTen",
            HeaderText = "Họ tên",
            ReadOnly = true,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            MinimumWidth = 150
        });

        // 4 CỘT TICK — mỗi học viên tick đúng 1 ô (nhanh hơn chọn dropdown).
        foreach (var (_, cot, tieuDe, rong) in CacTrangThai)
        {
            _grid.Columns.Add(new DataGridViewCheckBoxColumn
            {
                Name = cot,
                HeaderText = tieuDe,
                Width = rong,
                FlatStyle = FlatStyle.Standard,
                SortMode = DataGridViewColumnSortMode.NotSortable
            });
        }

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "GhiChu",
            HeaderText = "Ghi chú",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            MinimumWidth = 130
        });

        // Tick xong phải ghi giá trị NGAY (mặc định phải rời ô mới ghi).
        _grid.CurrentCellDirtyStateChanged += (_, _) =>
        {
            if (_grid.IsCurrentCellDirty)
            {
                _grid.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        };

        _grid.CellValueChanged += Grid_CellValueChanged;

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(430, 40);
        _lblThongBao.Location = new Point(20, 448);
        _lblThongBao.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        _lblThongBao.ForeColor = Theme.Muted;
        _lblThongBao.Font = Theme.FontSub;

        _btnLuu.Location = new Point(472, 452);
        _btnLuu.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _btnLuu.Click += async (_, _) => await LuuAsync();

        _btnDong.Location = new Point(634, 452);
        _btnDong.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _btnDong.Click += (_, _) => Close();

        Controls.Add(_lblTieuDe);
        Controls.Add(_grid);
        Controls.Add(_lblThongBao);
        Controls.Add(_btnLuu);
        Controls.Add(_btnDong);

        CancelButton = _btnDong;
    }

    private async Task TaiAsync()
    {
        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        var result = await AppSession.Api.GetAsync<AttendanceSheetDto>($"admin/sessions/{_maCaHoc}/attendance");

        UseWaitCursor = false;

        if (!result.Success || result.Data is null)
        {
            _lblTieuDe.Text = "KHÔNG TẢI ĐƯỢC BẢNG ĐIỂM DANH";
            _lblThongBao.Text = string.IsNullOrWhiteSpace(result.ErrorMessage)
                ? "Vui lòng thử lại sau."
                : result.ErrorMessage;
            return;
        }

        _sheet = result.Data;

        _lblTieuDe.Text = $"ĐIỂM DANH — {_sheet.TenLop} · {_sheet.NgayHoc:dd/MM/yyyy} · "
                          + $"{_sheet.GioBatDau:HH\\:mm}–{_sheet.GioKetThuc:HH\\:mm}";

        _grid.Rows.Clear();

        foreach (var hv in _sheet.HocViens)
        {
            var trangThai = string.IsNullOrWhiteSpace(hv.TrangThaiDiemDanh) ? "Có mặt" : hv.TrangThaiDiemDanh;

            if (!CacTrangThai.Any(x => x.TrangThai == trangThai))
            {
                trangThai = "Có mặt";
            }

            var giaTri = new object[CacTrangThai.Length];

            for (var i = 0; i < CacTrangThai.Length; i++)
            {
                giaTri[i] = CacTrangThai[i].TrangThai == trangThai;
            }

            var idx = _grid.Rows.Add(hv.MaHocVien, hv.HoTen);

            for (var i = 0; i < CacTrangThai.Length; i++)
            {
                _grid.Rows[idx].Cells[CacTrangThai[i].Cot].Value = giaTri[i];
            }

            _grid.Rows[idx].Cells["GhiChu"].Value = hv.GhiChu ?? string.Empty;
            _grid.Rows[idx].Tag = hv.MaHocVien;
        }

        if (!_sheet.CoTheDiemDanh)
        {
            _btnLuu.Enabled = false;
            _lblThongBao.ForeColor = Theme.Warning;
            _lblThongBao.Text = _sheet.LyDoKhongTheDiemDanh
                                ?? "Ca học này hiện không cho phép điểm danh.";
        }
        else
        {
            _btnLuu.Enabled = _grid.Rows.Count > 0;
            _lblThongBao.ForeColor = Theme.Muted;
            _lblThongBao.Text = _grid.Rows.Count == 0
                ? "Lớp chưa có học viên nào được xếp lớp."
                : $"Tổng {_grid.Rows.Count} học viên. Tick vào cột trạng thái rồi bấm Lưu điểm danh.";
        }
    }

    /// <summary>Mỗi học viên chỉ được 1 trạng thái: tick 1 ô thì 3 ô còn lại tự bỏ tick.</summary>
    private void Grid_CellValueChanged(object? sender, DataGridViewCellEventArgs e)
    {
        if (_dangCapNhat || e.RowIndex < 0 || e.ColumnIndex < 0)
        {
            return;
        }

        var tenCot = _grid.Columns[e.ColumnIndex].Name;

        if (!CotTrangThai.Contains(tenCot))
        {
            return;
        }

        var row = _grid.Rows[e.RowIndex];

        if (row.Cells[tenCot].Value is not true)
        {
            return; // chỉ xử lý khi người dùng TICK
        }

        _dangCapNhat = true;

        try
        {
            foreach (var khac in CotTrangThai.Where(x => x != tenCot))
            {
                row.Cells[khac].Value = false;
            }
        }
        finally
        {
            _dangCapNhat = false;
        }
    }

    private async Task LuuAsync()
    {
        if (_sheet is null) return;

        var items = new List<object>();

        foreach (DataGridViewRow row in _grid.Rows)
        {
            var maHocVien = row.Cells["MaHocVien"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(maHocVien)) continue;

            // Trạng thái = cột đang được TICK; không tick ô nào → mặc định "Có mặt".
            var trangThai = CacTrangThai
                .FirstOrDefault(x => row.Cells[x.Cot].Value is true).TrangThai ?? "Có mặt";

            var ghiChu = row.Cells["GhiChu"].Value?.ToString();

            items.Add(new
            {
                maHocVien,
                trangThaiDiemDanh = trangThai,
                ghiChu = string.IsNullOrWhiteSpace(ghiChu) ? null : ghiChu
            });
        }

        if (items.Count == 0)
        {
            _lblThongBao.ForeColor = Theme.Danger;
            _lblThongBao.Text = "Không có học viên nào để điểm danh.";
            return;
        }

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        var result = await AppSession.Api.PostAsync<AttendanceSheetDto>(
            $"admin/sessions/{_maCaHoc}/attendance",
            new { items });

        UseWaitCursor = false;
        _btnLuu.Enabled = true;

        if (!result.Success)
        {
            _lblThongBao.ForeColor = Theme.Danger;
            _lblThongBao.Text = string.IsNullOrWhiteSpace(result.ErrorMessage)
                ? "Không lưu được điểm danh."
                : result.ErrorMessage;
            return;
        }

        DataChanged = true;
        MessageBox.Show(
            FindForm(),
            $"Đã lưu điểm danh cho {items.Count} học viên.",
            "Thành công",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);

        Close();
    }
}
