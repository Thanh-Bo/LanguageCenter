using LanguageCenter.WinForms.Api;
using LanguageCenter.WinForms.UI;

namespace LanguageCenter.WinForms.Dialogs;

/// <summary>
/// Nhập / sửa NỘI DUNG 20 BUỔI HỌC của một khóa.
/// <para>
/// Mỗi khóa cố định 20 buổi (10 tuần × 2 buổi). Khi sinh ca học, buổi thứ i
/// sẽ tự lấy tiêu đề của dòng <c>SoBuoi = i</c> ở đây → cột "Nội dung" trong
/// tab "Ca học thực tế" hiện đúng bài.
/// </para>
/// Gọi <c>GET|PUT /api/admin/lesson-contents</c>.
/// </summary>
public sealed class FormNoiDungBaiHoc : Form
{
    /// <summary>Khung đào tạo cố định 20 buổi.</summary>
    private const int SoBuoiCoDinh = 20;

    private readonly IReadOnlyList<CourseListItemDto> _courses;
    private readonly int? _maKhoaHocChonSan;

    private readonly ComboBox _cboKhoaHoc = UiHelpers.CreateComboBox(420);
    private readonly DataGridView _grid = new();
    private readonly Label _lblThongBao = new();
    private readonly Label _lblDem = new();
    private readonly Button _btnLuu = UiHelpers.CreatePrimaryButton("Lưu 20 buổi", 150, 36);
    private readonly Button _btnDong = UiHelpers.CreateSecondaryButton("Đóng", 110, 36);

    private bool _dangNap;

    public bool DataChanged { get; private set; }

    public FormNoiDungBaiHoc(
        IReadOnlyList<CourseListItemDto> courses,
        int? maKhoaHocChonSan = null)
    {
        _courses = courses;
        _maKhoaHocChonSan = maKhoaHocChonSan;

        Text = "Nội dung 20 buổi học";
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.Sizable;
        MinimumSize = new Size(880, 560);
        ClientSize = new Size(940, 640);
        BackColor = Color.White;
        Font = Theme.FontRegular;

        DungGiaoDien();
        NapComboKhoaHoc();

        _cboKhoaHoc.SelectedIndexChanged += async (_, _) =>
        {
            if (!_dangNap) await NapNoiDungAsync();
        };

        Load += async (_, _) => await NapNoiDungAsync();
    }

    private void DungGiaoDien()
    {
        Controls.Add(new Label
        {
            Text = "NỘI DUNG 20 BUỔI HỌC",
            Font = Theme.FontHeading,
            ForeColor = Theme.Primary,
            AutoSize = true,
            Location = new Point(24, 18)
        });

        Controls.Add(new Label
        {
            Text = "Khóa học *",
            AutoSize = true,
            ForeColor = Theme.TextMedium,
            Font = Theme.FontSub,
            Location = new Point(24, 52)
        });

        _cboKhoaHoc.DropDownStyle = ComboBoxStyle.DropDownList;
        _cboKhoaHoc.Location = new Point(24, 72);
        Controls.Add(_cboKhoaHoc);

        _lblDem.AutoSize = true;
        _lblDem.Font = Theme.FontSub;
        _lblDem.ForeColor = Theme.Muted;
        _lblDem.Location = new Point(460, 78);
        Controls.Add(_lblDem);

        _grid.Dock = DockStyle.None;
        _grid.Location = new Point(24, 108);
        _grid.Size = new Size(892, 430);
        _grid.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
        _grid.AllowUserToAddRows = false;
        _grid.AllowUserToDeleteRows = false;
        _grid.RowHeadersVisible = false;
        _grid.MultiSelect = false;
        _grid.SelectionMode = DataGridViewSelectionMode.CellSelect;
        _grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _grid.EditMode = DataGridViewEditMode.EditOnEnter;
        _grid.BackgroundColor = Color.White;
        _grid.GridColor = Theme.BorderColor;
        _grid.ColumnHeadersDefaultCellStyle.Font = Theme.FontBold;
        _grid.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
        _grid.ColumnHeadersHeight = 38;
        _grid.RowTemplate.Height = 30;
        _grid.DefaultCellStyle.Font = Theme.FontRegular;
        _grid.DefaultCellStyle.SelectionBackColor = Theme.PrimaryLight;
        _grid.DefaultCellStyle.SelectionForeColor = Theme.TextDark;

        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "SoBuoi",
            HeaderText = "Buổi",
            FillWeight = 45,
            ReadOnly = true,
            DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
        });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "TieuDe", HeaderText = "Tiêu đề buổi học *", FillWeight = 180 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "NoiDung", HeaderText = "Nội dung giảng dạy", FillWeight = 260 });
        _grid.Columns.Add(new DataGridViewTextBoxColumn { Name = "MucTieu", HeaderText = "Mục tiêu / chuẩn đầu ra", FillWeight = 200 });

        Controls.Add(_grid);

        _lblThongBao.AutoSize = false;
        _lblThongBao.Size = new Size(700, 20);
        _lblThongBao.Location = new Point(24, 552);
        _lblThongBao.Anchor = AnchorStyles.Left | AnchorStyles.Bottom;
        _lblThongBao.ForeColor = Theme.Danger;
        _lblThongBao.Font = Theme.FontSub;
        Controls.Add(_lblThongBao);

        _btnLuu.Location = new Point(606, 580);
        _btnLuu.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
        _btnLuu.Click += async (_, _) => await LuuAsync();
        Controls.Add(_btnLuu);

        _btnDong.Location = new Point(766, 580);
        _btnDong.Anchor = AnchorStyles.Right | AnchorStyles.Bottom;
        _btnDong.Click += (_, _) => { DialogResult = DialogResult.Cancel; Close(); };
        Controls.Add(_btnDong);

        AcceptButton = _btnLuu;
        CancelButton = _btnDong;
    }

    private void NapComboKhoaHoc()
    {
        _dangNap = true;

        _cboKhoaHoc.Items.Clear();

        foreach (var c in _courses.OrderBy(c => c.TenKhoaHoc))
        {
            _cboKhoaHoc.Items.Add(new KhoaHocItem(c.MaKhoaHoc, c.TenKhoaHoc));
        }

        if (_cboKhoaHoc.Items.Count == 0)
        {
            _lblThongBao.Text = "Chưa có khóa học nào trong hệ thống.";
            _btnLuu.Enabled = false;
            _dangNap = false;
            return;
        }

        var viTri = 0;

        if (_maKhoaHocChonSan is { } maChon)
        {
            for (var i = 0; i < _cboKhoaHoc.Items.Count; i++)
            {
                if (_cboKhoaHoc.Items[i] is KhoaHocItem it && it.MaKhoaHoc == maChon)
                {
                    viTri = i;
                    break;
                }
            }
        }

        _cboKhoaHoc.SelectedIndex = viTri;
        _dangNap = false;
    }

    /// <summary>Tải nội dung đã lưu của khóa và luôn dựng đủ 20 dòng.</summary>
    private async Task NapNoiDungAsync()
    {
        if (_cboKhoaHoc.SelectedItem is not KhoaHocItem khoa)
        {
            return;
        }

        _lblThongBao.Text = string.Empty;
        UseWaitCursor = true;

        var result = await AppSession.Api.GetAsync<IReadOnlyList<LessonContentDto>>(
            $"admin/lesson-contents?maKhoaHoc={khoa.MaKhoaHoc}");

        UseWaitCursor = false;

        if (!result.Success || result.Data is null)
        {
            _lblThongBao.Text = result.ErrorMessage;
            _grid.Rows.Clear();
            _lblDem.Text = string.Empty;
            return;
        }

        var daLuu = result.Data.ToDictionary(x => x.SoBuoi);

        _grid.Rows.Clear();

        for (var buoi = 1; buoi <= SoBuoiCoDinh; buoi++)
        {
            daLuu.TryGetValue(buoi, out var nd);

            _grid.Rows.Add(
                buoi,
                nd?.TieuDe ?? string.Empty,
                nd?.NoiDung ?? string.Empty,
                nd?.MucTieu ?? string.Empty);
        }

        var soDaCo = daLuu.Count;
        _lblDem.Text = soDaCo == 0
            ? "Khóa này chưa có nội dung — hãy nhập đủ 20 buổi."
            : $"Đã có {soDaCo}/{SoBuoiCoDinh} buổi có nội dung.";
        _lblDem.ForeColor = soDaCo == SoBuoiCoDinh ? Theme.Success : Theme.Warning;
    }

    private async Task LuuAsync()
    {
        _lblThongBao.Text = string.Empty;

        if (_cboKhoaHoc.SelectedItem is not KhoaHocItem khoa)
        {
            _lblThongBao.Text = "Vui lòng chọn khóa học.";
            return;
        }

        // Bỏ dòng trắng cuối (nếu lưới đang ở chế độ cho thêm dòng).
        _grid.EndEdit();

        var items = new List<object>();
        var thieuTieuDe = new List<int>();

        foreach (DataGridViewRow row in _grid.Rows)
        {
            if (row.IsNewRow) continue;

            if (row.Cells["SoBuoi"].Value is not { } giaTri || !int.TryParse(giaTri.ToString(), out var soBuoi))
            {
                continue;
            }

            var tieuDe = row.Cells["TieuDe"].Value?.ToString()?.Trim() ?? string.Empty;

            if (tieuDe.Length == 0)
            {
                thieuTieuDe.Add(soBuoi);
                continue;
            }

            items.Add(new
            {
                soBuoi,
                tieuDe,
                noiDung = row.Cells["NoiDung"].Value?.ToString(),
                mucTieu = row.Cells["MucTieu"].Value?.ToString()
            });
        }

        if (thieuTieuDe.Count > 0)
        {
            _lblThongBao.Text = $"Buổi {string.Join(", ", thieuTieuDe)} chưa có tiêu đề. "
                + "Hãy nhập tiêu đề cho đủ 20 buổi (hoặc để trống cả dòng).";
            return;
        }

        if (items.Count == 0)
        {
            _lblThongBao.Text = "Chưa có buổi nào có nội dung để lưu.";
            return;
        }

        _btnLuu.Enabled = false;
        UseWaitCursor = true;

        var result = await AppSession.Api.PutAsync<IReadOnlyList<LessonContentDto>>(
            $"admin/lesson-contents/{khoa.MaKhoaHoc}",
            new { items });

        UseWaitCursor = false;
        _btnLuu.Enabled = true;

        if (!result.Success)
        {
            _lblThongBao.Text = result.ThongDiepDayDu();
            return;
        }

        DataChanged = true;

        var soLuu = result.Data?.Count(x => !string.IsNullOrWhiteSpace(x.TieuDe)) ?? items.Count;

        MessageBox.Show(FindForm(),
            $"Đã lưu nội dung {soLuu}/{SoBuoiCoDinh} buổi học cho khóa:\n   • {khoa.TenKhoaHoc}\n\n"
            + "Khi bấm 'Sinh ca học' ở màn Lịch học, cột Nội dung sẽ tự lấy theo đúng buổi.",
            "Lưu thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);

        await NapNoiDungAsync();
    }

    private sealed record KhoaHocItem(int MaKhoaHoc, string TenKhoaHoc)
    {
        public override string ToString() => TenKhoaHoc;
    }
}
