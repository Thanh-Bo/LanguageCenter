using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Điểm danh một ca học — <c>GET|POST /api/admin/sessions/{id}/attendance</c>.
/// </summary>
public sealed class FormDiemDanh : Form
{
    private static readonly string[] TrangThais =
    [
        "Có mặt",
        "Đi trễ",
        "Vắng có phép",
        "Vắng không phép"
    ];

    private readonly int _maCaHoc;

    private readonly Label _lblTieuDe = new();
    private readonly Label _lblThongBao = new();
    private readonly DataGridView _grid = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Lưu điểm danh", 150, 36);
    private readonly Button _btnDong = UiHelpers.CreateSecondaryButton("Đóng", 100, 36);

    private AttendanceSheetDto? _sheet;

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

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "MaHocVien",
            HeaderText = "Mã HV",
            ReadOnly = true,
            Width = 100
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "HoTen",
            HeaderText = "Họ tên",
            ReadOnly = true,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });

        var cbo = new DataGridViewComboBoxColumn
        {
            Name = "TrangThai",
            HeaderText = "Trạng thái",
            Width = 160,
            FlatStyle = FlatStyle.Flat,
            DisplayStyle = DataGridViewComboBoxDisplayStyle.DropDownButton
        };
        foreach (var t in TrangThais) cbo.Items.Add(t);
        _grid.Columns.Add(cbo);

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "GhiChu",
            HeaderText = "Ghi chú",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            MinimumWidth = 160
        });

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(520, 36);
        _lblThongBao.Location = new Point(20, 448);
        _lblThongBao.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        _lblThongBao.ForeColor = Theme.Muted;
        _lblThongBao.Font = Theme.FontSub;

        _btnLuu.Location = new Point(560, 468);
        _btnLuu.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _btnLuu.Click += async (_, _) => await LuuAsync();

        _btnDong.Location = new Point(720, 468);
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
            if (!TrangThais.Contains(trangThai)) trangThai = "Có mặt";

            var idx = _grid.Rows.Add(hv.MaHocVien, hv.HoTen, trangThai, hv.GhiChu ?? string.Empty);
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
                : $"Tổng {_grid.Rows.Count} học viên. Chọn trạng thái rồi bấm Lưu điểm danh.";
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

            var trangThai = row.Cells["TrangThai"].Value?.ToString();
            if (string.IsNullOrWhiteSpace(trangThai)) trangThai = "Có mặt";

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
