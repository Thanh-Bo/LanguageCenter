using System.Globalization;
using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Nhập / sửa / xoá điểm của một học viên trong một lớp.
/// Gọi <c>POST|PATCH|DELETE /api/admin/grades</c>.
/// </summary>
public sealed class FormNhapDiem : Form
{
    private sealed class DongDiem
    {
        public GradeComponentDto ThanhPhan { get; init; } = new();
        public decimal? DiemCu { get; init; }
        public int? MaDiemSo { get; init; }
    }

    private readonly AcademicStudentRowDto _hocVien;
    private readonly IReadOnlyList<GradeComponentDto> _thanhPhan;
    private readonly IReadOnlyDictionary<int, int> _maDiemSoTheoLoaiDiem;
    private readonly IReadOnlyList<TeacherListItemDto> _giaoViens;

    /// <summary>Giáo viên đang phân công lớp — được chọn sẵn ở ô "Người chấm".</summary>
    private readonly string? _maGiaoVienMacDinh;

    private readonly Label _lblTieuDe = new();
    private readonly Label _lblThongBao = new();
    private readonly Label _lblTongKet = new();
    private readonly DataGridView _grid = new();
    private readonly ComboBox _cboNguoiCham = UiHelpers.CreateComboBox(260);
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Lưu điểm", 130, 36);
    private readonly Button _btnDong = UiHelpers.CreateSecondaryButton("Đóng", 100, 36);

    public bool DataChanged { get; private set; }

    public FormNhapDiem(
        AcademicStudentRowDto hocVien,
        IReadOnlyList<GradeComponentDto> thanhPhan,
        IReadOnlyDictionary<int, int> maDiemSoTheoLoaiDiem,
        IReadOnlyList<TeacherListItemDto> giaoViens,
        string? maGiaoVienMacDinh = null)
    {
        _hocVien = hocVien;
        _thanhPhan = thanhPhan;
        _maDiemSoTheoLoaiDiem = maDiemSoTheoLoaiDiem;
        _giaoViens = giaoViens;
        _maGiaoVienMacDinh = maGiaoVienMacDinh;

        Text = $"Nhập điểm — {hocVien.HoTen}";
        StartPosition = FormStartPosition.CenterParent;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(820, 520);
        MinimumSize = new Size(700, 440);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        NapDuLieu();
    }

    private void DungGiaoDien()
    {
        _lblTieuDe.Text = $"NHẬP ĐIỂM — {_hocVien.HoTen} ({_hocVien.MaHocVien})";
        _lblTieuDe.Font = Theme.FontHeading;
        _lblTieuDe.ForeColor = Theme.Primary;
        _lblTieuDe.AutoSize = true;
        _lblTieuDe.Location = new Point(20, 16);

        _grid.Location = new Point(20, 52);
        _grid.Size = new Size(780, 340);
        _grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.RowHeadersVisible = false;
        _grid.AutoGenerateColumns = false;
        UiHelpers.StyleGrid(_grid);

        // StyleGrid đặt ReadOnly = true → phải mở lại, nếu không sẽ KHÔNG nhập được
        // điểm, không gõ được nhận xét và không tích được ô "Xoá".
        _grid.ReadOnly = false;
        _grid.EditMode = DataGridViewEditMode.EditOnEnter;

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "DauDiem",
            HeaderText = "Đầu điểm",
            ReadOnly = true,
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "TrongSo",
            HeaderText = "Trọng số",
            ReadOnly = true,
            Width = 90
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "DiemToiDa",
            HeaderText = "Tối đa",
            ReadOnly = true,
            Width = 80
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "Diem",
            HeaderText = "Điểm",
            Width = 90
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "NhanXet",
            HeaderText = "Nhận xét",
            AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill,
            MinimumWidth = 160
        });
        _grid.Columns.Add(new DataGridViewCheckBoxColumn
        {
            Name = "Xoa",
            HeaderText = "Xoá",
            Width = 50
        });

        var lblNguoiCham = new Label
        {
            Text = "Người chấm (giáo viên):",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(20, 404),
            Anchor = AnchorStyles.Bottom | AnchorStyles.Left
        };

        _cboNguoiCham.Location = new Point(170, 398);
        _cboNguoiCham.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

        _lblTongKet.Text = "Tổng kết hiện tại: "
                           + (_hocVien.DiemTongKet.HasValue
                               ? _hocVien.DiemTongKet.Value.ToString("0.00", CultureInfo.InvariantCulture)
                               : "chưa đủ điểm");
        _lblTongKet.AutoSize = true;
        _lblTongKet.Font = Theme.FontSub;
        _lblTongKet.ForeColor = Theme.TextMedium;
        _lblTongKet.Location = new Point(450, 404);
        _lblTongKet.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(540, 34);
        _lblThongBao.Location = new Point(20, 430);
        _lblThongBao.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        _lblThongBao.ForeColor = Theme.Muted;
        _lblThongBao.Font = Theme.FontSub;

        _btnLuu.Location = new Point(580, 468);
        _btnLuu.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _btnLuu.Click += async (_, _) => await LuuAsync();

        _btnDong.Location = new Point(720, 468);
        _btnDong.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        _btnDong.Click += (_, _) => Close();

        Controls.Add(_lblTieuDe);
        Controls.Add(_grid);
        Controls.Add(lblNguoiCham);
        Controls.Add(_cboNguoiCham);
        Controls.Add(_lblTongKet);
        Controls.Add(_lblThongBao);
        Controls.Add(_btnLuu);
        Controls.Add(_btnDong);

        CancelButton = _btnDong;
    }

    private void NapDuLieu()
    {
        _grid.Rows.Clear();

        foreach (var tp in _thanhPhan.OrderBy(x => x.ThuTu))
        {
            var diemCu = _hocVien.ThanhPhan.FirstOrDefault(x => x.MaLoaiDiem == tp.MaLoaiDiem)?.Diem;
            var maDiemSo = _maDiemSoTheoLoaiDiem.TryGetValue(tp.MaLoaiDiem, out var id) ? id : (int?)null;

            var idx = _grid.Rows.Add(
                tp.TenLoaiDiem,
                $"{tp.TrongSo:0.#}%",
                tp.DiemToiDa.ToString("0.#", CultureInfo.InvariantCulture),
                diemCu.HasValue ? diemCu.Value.ToString("0.##", CultureInfo.InvariantCulture) : string.Empty,
                string.Empty,
                false);

            _grid.Rows[idx].Tag = new DongDiem
            {
                ThanhPhan = tp,
                DiemCu = diemCu,
                MaDiemSo = maDiemSo
            };
        }

        _cboNguoiCham.Items.Clear();

        // Ưu tiên giáo viên đang được phân công lớp này → Quản trị viên không phải chọn lại.
        var dsSapXep = _giaoViens
            .OrderByDescending(gv => string.Equals(
                gv.MaGiaoVien, _maGiaoVienMacDinh, StringComparison.OrdinalIgnoreCase))
            .ThenBy(gv => gv.MaGiaoVien)
            .ToList();

        foreach (var gv in dsSapXep)
        {
            _cboNguoiCham.Items.Add(new GiaoVienItem(gv.MaGiaoVien, $"{gv.MaGiaoVien} — {gv.HoTen} ({gv.LoaiGiaoVien})"));
        }

        if (_cboNguoiCham.Items.Count > 0) _cboNguoiCham.SelectedIndex = 0;

        _lblThongBao.Text = _thanhPhan.Count == 0
            ? "Khóa học của lớp này chưa cấu hình đầu điểm."
            : "Quản trị viên nhập điểm được — chỉ cần chọn 'Người chấm'. "
              + "Để trống điểm và tích 'Xoá' nếu muốn huỷ một đầu điểm.";
    }

    private async Task LuuAsync()
    {
        _lblThongBao.ForeColor = Theme.Danger;

        var maGiaoVien = (_cboNguoiCham.SelectedItem as GiaoVienItem)?.MaGiaoVien;
        if (string.IsNullOrWhiteSpace(maGiaoVien))
        {
            _lblThongBao.Text = "Vui lòng chọn người chấm (giáo viên).";
            return;
        }

        var thaoTac = new List<(DongDiem Dong, string? Kieu, decimal? Diem, string? NhanXet)>();

        foreach (DataGridViewRow row in _grid.Rows)
        {
            if (row.Tag is not DongDiem dong) continue;

            var xoa = row.Cells["Xoa"].Value is true;
            var diemText = row.Cells["Diem"].Value?.ToString()?.Trim();
            var nhanXet = row.Cells["NhanXet"].Value?.ToString()?.Trim();

            if (xoa)
            {
                if (dong.MaDiemSo.HasValue) thaoTac.Add((dong, "DELETE", null, null));
                continue;
            }

            if (string.IsNullOrWhiteSpace(diemText))
            {
                // Xoá trắng điểm đã có → coi như xoá bản ghi
                if (dong.MaDiemSo.HasValue) thaoTac.Add((dong, "DELETE", null, null));
                continue;
            }

            if (!decimal.TryParse(diemText, NumberStyles.Number, CultureInfo.CurrentCulture, out var diem)
                && !decimal.TryParse(diemText, NumberStyles.Number, CultureInfo.InvariantCulture, out diem))
            {
                _lblThongBao.Text = $"Điểm của '{dong.ThanhPhan.TenLoaiDiem}' không hợp lệ.";
                return;
            }

            if (diem < 0 || diem > dong.ThanhPhan.DiemToiDa)
            {
                _lblThongBao.Text = $"Điểm '{dong.ThanhPhan.TenLoaiDiem}' phải nằm trong 0 – {dong.ThanhPhan.DiemToiDa:0.#}.";
                return;
            }

            var khongDoi = dong.DiemCu.HasValue && Math.Abs(dong.DiemCu.Value - diem) < 0.001m
                           && string.IsNullOrWhiteSpace(nhanXet);

            if (khongDoi) continue;

            thaoTac.Add((dong, dong.MaDiemSo.HasValue ? "PATCH" : "POST", diem,
                string.IsNullOrWhiteSpace(nhanXet) ? null : nhanXet));
        }

        if (thaoTac.Count == 0)
        {
            _lblThongBao.ForeColor = Theme.Muted;
            _lblThongBao.Text = "Không có thay đổi nào để lưu.";
            return;
        }

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        var thanhCong = 0;
        string? loiDauTien = null;

        foreach (var (dong, kieu, diem, nhanXet) in thaoTac)
        {
            var path = kieu == "POST" ? "admin/grades" : $"admin/grades/{dong.MaDiemSo}";

            var ketQua = kieu switch
            {
                "POST" => await AppSession.Api.PostAsync<GradeRowDto>("admin/grades", new
                {
                    maDangKy = _hocVien.MaDangKy,
                    maLoaiDiem = dong.ThanhPhan.MaLoaiDiem,
                    maGiaoVien,
                    diem = diem!.Value,
                    nhanXet
                }),
                "PATCH" => await AppSession.Api.PatchAsync<GradeRowDto>($"admin/grades/{dong.MaDiemSo}", new
                {
                    diem = diem!.Value,
                    nhanXet
                }),
                _ => await AppSession.Api.DeleteAsync<GradeRowDto>($"admin/grades/{dong.MaDiemSo}")
            };

            if (ketQua.Success) thanhCong++;
            else loiDauTien ??= $"{dong.ThanhPhan.TenLoaiDiem}: {ketQua.ErrorMessage}";
        }

        UseWaitCursor = false;
        _btnLuu.Enabled = true;

        if (thanhCong == 0)
        {
            _lblThongBao.Text = loiDauTien ?? "Không lưu được điểm.";
            return;
        }

        DataChanged = true;

        _lblThongBao.ForeColor = loiDauTien is null ? Theme.Success : Theme.Warning;
        _lblThongBao.Text = loiDauTien is null
            ? $"Đã lưu {thanhCong} thay đổi."
            : $"Đã lưu {thanhCong}/{thaoTac.Count} thay đổi. Lỗi: {loiDauTien}";

        DialogResult = DialogResult.OK;
    }

    private sealed record GiaoVienItem(string MaGiaoVien, string MoTa)
    {
        public override string ToString() => MoTa;
    }
}
