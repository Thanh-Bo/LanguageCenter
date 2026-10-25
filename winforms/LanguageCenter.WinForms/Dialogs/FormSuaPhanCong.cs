using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

public partial class FormSuaPhanCong : Form
{
    private const string ChuaPhanCong = "— Chưa phân công —";
    private const string VaiTroGiaoVienChinh = "Giáo viên chính";
    private const string VaiTroTroGiang = "Trợ giảng";

    private IReadOnlyList<ClassListItemDto> _classes = Array.Empty<ClassListItemDto>();
    private IReadOnlyList<TeacherListItemDto> _teachers = Array.Empty<TeacherListItemDto>();

    /// <summary>Phân công đã có của lớp đang thao tác (dùng khi gắn người vào khung có sẵn).</summary>
    private IReadOnlyList<AssignmentListItemDto> _phanCongHienCo = Array.Empty<AssignmentListItemDto>();

    private readonly ClassListItemDto? _lopChiDinh;
    private readonly Label _lblThoiGian = new();

    /// <summary>True khi đã ghi dữ liệu thành công — nơi gọi nên tải lại danh sách.</summary>
    public bool DataChanged { get; private set; }

    /// <param name="lop">
    /// Truyền vào khi mở từ một dòng phân công có sẵn (gắn / đổi người cho lớp đó).
    /// Để <c>null</c> khi tạo phân công mới từ đầu.
    /// </param>
    /// <param name="phanCongHienCo">Các phân công đã có của <paramref name="lop"/>.</param>
    public FormSuaPhanCong(
        ClassListItemDto? lop = null,
        IReadOnlyList<AssignmentListItemDto>? phanCongHienCo = null)
    {
        _lopChiDinh = lop;
        _phanCongHienCo = phanCongHienCo ?? Array.Empty<AssignmentListItemDto>();

        InitializeComponent();

        Text = lop is null
            ? "Phân công giảng dạy mới — HUIT LingoHub"
            : $"Gán nhân sự cho lớp {lop.MaLop} — HUIT LingoHub";

        lblTitle.Text = lop is null
            ? "PHÂN CÔNG GIẢNG VIÊN / TRỢ GIẢNG"
            : "GẮN GIÁO VIÊN / TRỢ GIẢNG VÀO LỚP";

        btnSave.DialogResult = DialogResult.None;
        btnSave.Click += BtnSave_Click;
        btnCancel.Click += (_, _) => DialogResult = DialogResult.Cancel;

        // Vai trò được xác định bằng Ô CHỌN NGƯỜI (1 ô = Giáo viên chính, 1 ô = Trợ giảng),
        // không cần combobox vai trò riêng.
        cboVaiTro.Visible = false;
        lblVT.Visible = false;

        txtGhiChu.Text = "Giảng dạy theo đề cương chuẩn của trung tâm";

        // Thời gian phân công là THUỘC TÍNH CỦA LỚP → không cho chọn tay.
        lblKT.Visible = false;
        dtpBatDau.Visible = false;
        dtpKetThuc.Visible = false;
        lblBD.Text = "Thời gian phân công:";

        _lblThoiGian.AutoSize = false;
        _lblThoiGian.Size = new Size(450, 40);
        _lblThoiGian.Location = new Point(230, 202);
        _lblThoiGian.Font = new Font("Segoe UI", 9.5f);
        _lblThoiGian.ForeColor = Color.FromArgb(71, 85, 105);
        _lblThoiGian.Text = "—";
        Controls.Add(_lblThoiGian);

        cboLopHoc.SelectedIndexChanged += async (_, _) =>
        {
            CapNhatThoiGian();
            await NapPhanCongCuaLopAsync();
        };

        Load += async (_, _) => await LoadDataAsync();
    }

    /// <summary>
    /// Tải phân công hiện có của lớp đang chọn — nhờ vậy lưu lần 2 sẽ CẬP NHẬT
    /// thay vì tạo thêm dòng trùng.
    /// </summary>
    private async Task NapPhanCongCuaLopAsync()
    {
        if (_lopChiDinh is not null)
        {
            return; // lớp cố định → đã có danh sách truyền vào
        }

        if (cboLopHoc.SelectedIndex < 0 || cboLopHoc.SelectedIndex >= _classes.Count)
        {
            return;
        }

        var lop = _classes[cboLopHoc.SelectedIndex];

        var result = await AppSession.Api.GetAsync<PagedResult<AssignmentListItemDto>>(
            QueryStringBuilder.Build(
                "admin/teaching-assignments",
                ("page", "1"),
                ("pageSize", "100"),
                ("maLopHoc", lop.MaLopHoc.ToString())));

        _phanCongHienCo = result.Success && result.Data is not null
            ? result.Data.Items
            : Array.Empty<AssignmentListItemDto>();

        if (cboNhanSu.Items.Count > 0) cboNhanSu.SelectedIndex = 0;
        if (cboTroGiang.Items.Count > 0) cboTroGiang.SelectedIndex = 0;

        ChonSanNguoiHienCo();
    }

    /// <summary>Hiển thị khoảng ngày của lớp đang chọn (phân công bám theo lớp).</summary>
    private void CapNhatThoiGian()
    {
        if (cboLopHoc.SelectedIndex < 0 || cboLopHoc.SelectedIndex >= _classes.Count)
        {
            _lblThoiGian.Text = "—";
            return;
        }

        var lop = _classes[cboLopHoc.SelectedIndex];

        _lblThoiGian.Text = $"{lop.NgayBatDau:dd/MM/yyyy}  →  {lop.NgayKetThucDuKien:dd/MM/yyyy}";
    }

    /// <summary>Nạp danh sách lớp và giáo viên/trợ giảng thật từ API.</summary>
    private async Task LoadDataAsync()
    {
        btnSave.Enabled = false;
        UseWaitCursor = true;

        var classesTask = AppSession.Api.GetAsync<PagedResult<ClassListItemDto>>(
            QueryStringBuilder.Build("admin/classes", ("page", "1"), ("pageSize", "500")));
        var teachersTask = AppSession.Api.GetAsync<PagedResult<TeacherListItemDto>>(
            QueryStringBuilder.Build("admin/teachers", ("page", "1"), ("pageSize", "500")));

        await Task.WhenAll(classesTask, teachersTask);

        UseWaitCursor = false;
        btnSave.Enabled = true;

        _classes = (classesTask.Result.Data?.Items ?? Array.Empty<ClassListItemDto>())
            .OrderBy(c => c.MaLop)
            .ToList();

        _teachers = (teachersTask.Result.Data?.Items ?? Array.Empty<TeacherListItemDto>())
            .OrderBy(t => t.MaGiaoVien)
            .ToList();

        cboLopHoc.Items.Clear();
        foreach (var c in _classes)
        {
            // Chỉ hiển thị TÊN lớp — mã lớp không cần thiết trong ô chọn này.
            cboLopHoc.Items.Add(c.TenLop);
        }
        if (cboLopHoc.Items.Count > 0)
        {
            var viTri = _lopChiDinh is null
                ? 0
                : _classes.ToList().FindIndex(c => c.MaLopHoc == _lopChiDinh.MaLopHoc);

            cboLopHoc.SelectedIndex = viTri >= 0 ? viTri : 0;
        }

        // Chỉ được chọn đúng loại hồ sơ cho từng vai trò (backend cũng kiểm tra lại).
        DoVaoCombo(cboNhanSu, "Giáo viên");
        DoVaoCombo(cboTroGiang, "Trợ giảng");

        if (_lopChiDinh is not null)
        {
            cboLopHoc.Enabled = false;
        }

        ChonSanNguoiHienCo();

        CapNhatThoiGian();

        if (!classesTask.Result.Success)
        {
            MessageBox.Show(this, classesTask.Result.ErrorMessage, "Không tải được lớp học", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        else if (!teachersTask.Result.Success)
        {
            MessageBox.Show(this, teachersTask.Result.ErrorMessage, "Không tải được giáo viên", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>Đổ danh sách người theo loại hồ sơ vào combo, dòng đầu là "chưa phân công".</summary>
    private void DoVaoCombo(ComboBox cbo, string loaiHoSo)
    {
        cbo.Items.Clear();
        cbo.Items.Add(ChuaPhanCong);

        foreach (var t in _teachers.Where(t => string.Equals(t.LoaiGiaoVien, loaiHoSo, StringComparison.OrdinalIgnoreCase)))
        {
            cbo.Items.Add($"{t.MaGiaoVien} - {t.HoTen}");
        }

        cbo.SelectedIndex = 0;
    }

    /// <summary>Nếu lớp đã có phân công thì chọn sẵn người tương ứng.</summary>
    private void ChonSanNguoiHienCo()
    {
        foreach (var a in _phanCongHienCo)
        {
            var cbo = a.VaiTroPhanCong == VaiTroTroGiang ? cboTroGiang : cboNhanSu;

            for (var i = 1; i < cbo.Items.Count; i++)
            {
                var item = cbo.Items[i]?.ToString();

                if (item is not null && a.MaGiaoVien is not null
                    && item.StartsWith(a.MaGiaoVien + " ", StringComparison.OrdinalIgnoreCase))
                {
                    cbo.SelectedIndex = i;
                    break;
                }
            }
        }
    }

    private TeacherListItemDto? NguoiDangChon(ComboBox cbo)
    {
        if (cbo.SelectedIndex <= 0)
        {
            return null;
        }

        var dong = cbo.Items[cbo.SelectedIndex]?.ToString();
        var ma = dong?.Split(" - ")[0];

        return _teachers.FirstOrDefault(t => t.MaGiaoVien == ma);
    }

    private async void BtnSave_Click(object? sender, EventArgs e)
    {
        if (cboLopHoc.SelectedIndex < 0 || cboLopHoc.SelectedIndex >= _classes.Count)
        {
            MessageBox.Show(this, "Vui lòng chọn lớp học.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var lop = _classes[cboLopHoc.SelectedIndex];
        var giaoVienChinh = NguoiDangChon(cboNhanSu);
        var troGiang = NguoiDangChon(cboTroGiang);

        if (giaoVienChinh is null && troGiang is null)
        {
            var daCoKhung = _phanCongHienCo.Any(a => a.MaLopHoc == lop.MaLopHoc);

            if (daCoKhung)
            {
                MessageBox.Show(this, "Bạn chưa chọn ai để gắn vào lớp này.",
                    "Chưa chọn nhân sự", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Cho phép tạo "khung" chờ phân công — bấm Có để tiếp tục.
            var taoKhung = MessageBox.Show(this,
                "Chưa chọn giáo viên / trợ giảng. Tạo KHUNG phân công ở trạng thái 'Chờ phân công'?",
                "Tạo khung phân công", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);

            if (taoKhung != DialogResult.Yes)
            {
                return;
            }
        }

        btnSave.Enabled = false;
        UseWaitCursor = true;

        var ghiChu = string.IsNullOrWhiteSpace(txtGhiChu.Text) ? null : txtGhiChu.Text.Trim();
        var loi = new List<string>();

        loi.AddRange(await LuuVaiTroAsync(lop, VaiTroGiaoVienChinh, giaoVienChinh, ghiChu));
        loi.AddRange(await LuuVaiTroAsync(lop, VaiTroTroGiang, troGiang, ghiChu));

        UseWaitCursor = false;
        btnSave.Enabled = true;

        if (loi.Count > 0)
        {
            MessageBox.Show(this, string.Join(Environment.NewLine + Environment.NewLine, loi),
                "Phân công thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        DialogResult = DialogResult.OK;
    }

    /// <summary>
    /// Ghi 1 vai trò: chưa có → tạo mới; đã có → cập nhật người (gửi chuỗi rỗng để bỏ gán).
    /// Trả về danh sách lỗi (rỗng = thành công).
    /// </summary>
    private async Task<List<string>> LuuVaiTroAsync(
        ClassListItemDto lop,
        string vaiTro,
        TeacherListItemDto? nguoiChon,
        string? ghiChu)
    {
        var loi = new List<string>();

        var hienCo = _phanCongHienCo.FirstOrDefault(a =>
            a.MaLopHoc == lop.MaLopHoc && a.VaiTroPhanCong == vaiTro);

        if (hienCo is null)
        {
            if (nguoiChon is null)
            {
                return loi;
            }

            var tao = await AppSession.Api.PostAsync<AssignmentListItemDto>(
                "admin/teaching-assignments", new
                {
                    maLopHoc = lop.MaLopHoc,
                    maGiaoVien = nguoiChon.MaGiaoVien,
                    vaiTroPhanCong = vaiTro,
                    ghiChu
                });

            if (tao.Success)
            {
                DataChanged = true;
            }
            else
            {
                loi.Add($"{vaiTro}: {tao.ErrorMessage}");
            }

            return loi;
        }

        if (string.Equals(hienCo.MaGiaoVien ?? string.Empty, nguoiChon?.MaGiaoVien ?? string.Empty,
                StringComparison.OrdinalIgnoreCase))
        {
            return loi; // không đổi gì
        }

        if (hienCo.TrangThai is "Đã kết thúc" or "Đã hủy")
        {
            loi.Add($"{vaiTro}: phân công đã ở trạng thái '{hienCo.TrangThai}' — hãy tạo phân công mới.");
            return loi;
        }

        var sua = await AppSession.Api.PatchAsync<AssignmentListItemDto>(
            $"admin/teaching-assignments/{hienCo.MaPhanCong}", new
            {
                maGiaoVien = nguoiChon?.MaGiaoVien ?? string.Empty,
                ghiChu
            });

        if (sua.Success)
        {
            DataChanged = true;
        }
        else
        {
            loi.Add($"{vaiTro}: {sua.ErrorMessage}");
        }

        return loi;
    }
}

