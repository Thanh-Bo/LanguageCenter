using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Tiến độ từng buổi của một lớp — <c>GET /api/admin/classes/{id}/progress</c>.
/// Tô màu: đã học (xanh lá), buổi thi (tím), buổi hiện tại (xanh dương), đã hủy (đỏ).
/// </summary>
public sealed class FormTienDoLop : Form
{
    private readonly int _maLopHoc;

    private readonly Label _lblTieuDe = new();
    private readonly Label _lblTomTat = new();
    private readonly Label _lblThongBao = new();
    private readonly DataGridView _grid = new();
    private readonly Button _btnDong = UiHelpers.CreateSecondaryButton("Đóng", 100, 36);

    public FormTienDoLop(int maLopHoc, string tenLop)
    {
        _maLopHoc = maLopHoc;

        Text = $"Tiến độ lớp {tenLop}";
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(900, 560);
        MinimumSize = new Size(760, 480);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        UI.KiemTraNhap.BatTuNoiChieuCao(this);
        Load += async (_, _) => await TaiAsync();
    }

    private void DungGiaoDien()
    {
        _lblTieuDe.Text = "TIẾN ĐỘ BUỔI HỌC";
        _lblTieuDe.Font = Theme.FontHeading;
        _lblTieuDe.ForeColor = Theme.Primary;
        _lblTieuDe.AutoSize = true;
        _lblTieuDe.Location = new Point(20, 16);

        _lblTomTat.AutoSize = true;
        _lblTomTat.Font = Theme.FontSub;
        _lblTomTat.ForeColor = Theme.TextMedium;
        _lblTomTat.Location = new Point(20, 48);

        _grid.Location = new Point(20, 78);
        _grid.Size = new Size(860, 420);
        _grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.RowHeadersVisible = false;
        _grid.ReadOnly = true;
        _grid.AutoGenerateColumns = false;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        UiHelpers.StyleGrid(_grid);

        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "Buoi", HeaderText = "Buổi", FillWeight = 50 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "NgayHoc", HeaderText = "Ngày học", FillWeight = 85 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "GioHoc", HeaderText = "Giờ", FillWeight = 95 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "LoaiBuoi", HeaderText = "Loại buổi", FillWeight = 80 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "TrangThai", HeaderText = "Trạng thái", FillWeight = 90 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "GiaoVien", HeaderText = "Giáo viên", FillWeight = 140 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "TroGiang", HeaderText = "Trợ giảng", FillWeight = 120 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "DiemDanh", HeaderText = "Điểm danh", FillWeight = 70 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "NoiDung", HeaderText = "Nội dung", FillWeight = 200 });

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(700, 20);
        _lblThongBao.Location = new Point(20, 510);
        _lblThongBao.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Font = Theme.FontSub;

        _btnDong.Location = new Point(780, 508);
        _btnDong.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
        _btnDong.Click += (_, _) => Close();

        Controls.Add(_lblTieuDe);
        Controls.Add(_lblTomTat);
        Controls.Add(_grid);
        Controls.Add(_lblThongBao);
        Controls.Add(_btnDong);

        CancelButton = _btnDong;
    }

    private async Task TaiAsync()
    {
        UseWaitCursor = true;

        var result = await AppSession.Api.GetAsync<ClassProgressDto>($"admin/classes/{_maLopHoc}/progress");

        UseWaitCursor = false;

        if (!result.Success || result.Data is null)
        {
            _lblThongBao.Text = string.IsNullOrWhiteSpace(result.ErrorMessage)
                ? "Không tải được tiến độ lớp."
                : result.ErrorMessage;
            return;
        }

        var td = result.Data;

        _lblTieuDe.Text = $"TIẾN ĐỘ — {td.MaLop} · {td.TenLop}";
        _lblTomTat.Text = $"Đã học {td.SoBuoiDaHoc}/{td.TongSoBuoi} buổi ({td.TienDoPhanTram:0.#}%)"
                          + (td.SoBuoiHienTai is { } so ? $" · Buổi hiện tại: {so}" : string.Empty)
                          + $" · Đã sinh {td.SoBuoiDaSinh} ca";

        _grid.Rows.Clear();

        foreach (var b in td.Buois)
        {
            var idx = _grid.Rows.Add(
                b.SoBuoi?.ToString() ?? "—",
                UiHelpers.FormatDate(b.NgayHoc),
                $"{b.GioBatDau:HH\\:mm}–{b.GioKetThuc:HH\\:mm}",
                b.LoaiBuoi,
                b.TrangThai,
                b.TenGiaoVienThucTe ?? "—",
                b.TenTroGiangThucTe ?? "—",
                b.SoDiemDanh,
                b.NoiDungBaiHoc ?? "—");

            var row = _grid.Rows[idx];

            if (b.TrangThai == "Đã hủy")
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(254, 242, 242);
                row.DefaultCellStyle.ForeColor = Color.FromArgb(185, 28, 28);
            }
            else if (b.LaBuoiHienTai)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(239, 246, 255);
            }
            else if (b.LaBuoiThi)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(245, 243, 255);
                row.Cells["LoaiBuoi"].Style.Font = Theme.FontBold;
            }
            else if (b.DaHoc)
            {
                row.DefaultCellStyle.BackColor = Color.FromArgb(236, 253, 245);
            }
        }

        _lblThongBao.ForeColor = td.Buois.Count == 0 ? Theme.Warning : Theme.Muted;
        _lblThongBao.Text = td.Buois.Count == 0
            ? "Lớp chưa sinh buổi học. Hãy vào màn Lịch học để sinh 20 buổi."
            : string.Empty;
    }
}
