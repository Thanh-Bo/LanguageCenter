using LanguageCenter.WinForms.Api;

namespace LanguageCenter.WinForms.UserControls;

/// Dashboard tổng quan trung tâm ngoại ngữ.
/// Hiển thị TOÀN BỘ chỉ số hoạt động lấy từ <c>GET /api/admin/reports/overview</c>
/// (không còn khối "Truy cập nhanh" và "Thông báo & nhắc việc").
public partial class UcTongQuan : UserControl
{
    private readonly FormChinh _formMain;

    /// <summary>Giá trị lớn của từng thẻ KPI (khóa → label).</summary>
    private readonly Dictionary<string, Label> _oGiaTri = [];

    /// <summary>Dòng ghi chú nhỏ dưới mỗi thẻ KPI.</summary>
    private readonly Dictionary<string, Label> _oGhiChu = [];

    private readonly DataGridView _dgvChiTiet = new();
    private readonly DataGridView _dgvTaiChinh = new();

    public UcTongQuan() : this(null!)
    {
    }

    public UcTongQuan(FormChinh formMain)
    {
        _formMain = formMain;
        InitializeComponent();

        var currentUser = AppSession.CurrentUser?.Username ?? "Quản trị viên";
        lblHeroTitle.Text = $"Xin chào, {currentUser}! Chúc bạn một ngày làm việc hiệu quả!";
        lblHeroClock.Text = $"{DateTime.Now:dd/MM/yyyy} — {DateTime.Now:HH:mm}";

        btnRefresh.Click += async (_, _) => await TaiSoLieuAsync();
        lnkViewAllCourses.LinkClicked += (_, _) => _formMain?.NavigateTo(NavItem.Courses);
        pnlCoursesHeader.Resize += (_, _) =>
        {
            lnkViewAllCourses.Location = new Point(pnlCoursesHeader.ClientSize.Width - lnkViewAllCourses.Width - 4, 8);
        };

        DungLaiKhuVucSoLieu();

        UI.UiHelpers.StyleGrid(dgvFeaturedCourses);
        pnlQuickActions.Paint += UI.UiHelpers.PaintCardBorder;
        pnlFeaturedCourses.Paint += UI.UiHelpers.PaintCardBorder;
        pnlNotices.Paint += UI.UiHelpers.PaintCardBorder;

        Load += async (_, _) => await TaiSoLieuAsync();
    }

    /// <summary>
    /// Thay khối "Truy cập nhanh" bằng bảng số liệu chi tiết và khối "Thông báo"
    /// bằng bảng tài chính – điểm danh; đồng thời mở rộng lưới thẻ từ 4 lên 8 chỉ số.
    /// </summary>
    private void DungLaiKhuVucSoLieu()
    {
        // ---------- 8 thẻ chỉ số (2 hàng × 4 cột) ----------
        tblCards.Controls.Clear();
        tblCards.ColumnStyles.Clear();
        tblCards.RowStyles.Clear();
        tblCards.ColumnCount = 4;
        tblCards.RowCount = 2;
        tblCards.Height = 246;

        for (var i = 0; i < 4; i++)
        {
            tblCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
        }

        tblCards.RowStyles.Add(new RowStyle(SizeType.Absolute, 118F));
        tblCards.RowStyles.Add(new RowStyle(SizeType.Absolute, 118F));

        (string Khoa, string TieuDe, Color Mau)[] theChiSo =
        [
            ("taiKhoan", "Tổng tài khoản", UI.Theme.Primary),
            ("taiKhoanHoatDong", "Tài khoản đang hoạt động", UI.Theme.Success),
            ("hocVien", "Hồ sơ học viên", UI.Theme.Purple),
            ("hocVienDangHoc", "Học viên đang học", UI.Theme.Info),
            ("giaoVien", "Giáo viên", UI.Theme.Warning),
            ("troGiang", "Trợ giảng", UI.Theme.Info),
            ("lopHoc", "Lớp học", UI.Theme.Primary),
            ("lopDangHoc", "Lớp đang học", UI.Theme.Success)
        ];

        for (var i = 0; i < theChiSo.Length; i++)
        {
            var the = TaoTheChiSo(theChiSo[i].Khoa, theChiSo[i].TieuDe, theChiSo[i].Mau);
            the.Margin = new Padding(i % 4 == 0 ? 0 : 6, i < 4 ? 0 : 8, i % 4 == 3 ? 0 : 6, 0);
            tblCards.Controls.Add(the, i % 4, i / 4);
        }

        // ---------- Khối thay thế "Truy cập nhanh" ----------
        pnlQuickActions.Controls.Clear();
        pnlQuickActions.Height = 330;

        var lblChiTiet = new Label
        {
            Text = "SỐ LIỆU CHI TIẾT TOÀN HỆ THỐNG",
            Dock = DockStyle.Top,
            Height = 30,
            Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
            ForeColor = UI.Theme.TextDark
        };

        CauHinhLuoi(_dgvChiTiet);
        _dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { Name = "Nhom", HeaderText = "Nhóm", FillWeight = 30 });
        _dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { Name = "ChiTieu", HeaderText = "Chỉ tiêu", FillWeight = 45 });
        _dgvChiTiet.Columns.Add(new DataGridViewTextBoxColumn { Name = "GiaTri", HeaderText = "Giá trị", FillWeight = 25 });

        pnlQuickActions.Controls.Add(_dgvChiTiet);
        pnlQuickActions.Controls.Add(lblChiTiet);

        // ---------- Khối thay thế "Thông báo & nhắc việc" ----------
        pnlNotices.Controls.Clear();

        var lblTaiChinh = new Label
        {
            Text = "HỌC PHÍ & ĐIỂM DANH",
            Dock = DockStyle.Top,
            Height = 30,
            Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
            ForeColor = UI.Theme.TextDark
        };

        CauHinhLuoi(_dgvTaiChinh);
        _dgvTaiChinh.Columns.Add(new DataGridViewTextBoxColumn { Name = "Muc", HeaderText = "Mục", FillWeight = 60 });
        _dgvTaiChinh.Columns.Add(new DataGridViewTextBoxColumn { Name = "GiaTri", HeaderText = "Giá trị", FillWeight = 40 });

        pnlNotices.Controls.Add(_dgvTaiChinh);
        pnlNotices.Controls.Add(lblTaiChinh);

        // Cột trái/phải cao hơn để chứa đủ số liệu.
        tblMainSection.Height = 640;

        lblCoursesTitle.Text = "CHƯƠNG TRÌNH ĐÀO TẠO";

        InitFeaturedCoursesGrid();
    }

    /// <summary>Cấu hình lưới chỉ-đọc dùng để in số liệu.</summary>
    private static void CauHinhLuoi(DataGridView grid)
    {
        grid.Dock = DockStyle.Fill;
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.RowHeadersVisible = false;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.ReadOnly = true;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.MultiSelect = false;
        grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9f, FontStyle.Bold);
        grid.ColumnHeadersDefaultCellStyle.BackColor = UI.Theme.PrimaryLight;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = UI.Theme.TextDark;
        grid.DefaultCellStyle.Font = new Font("Segoe UI", 9f);
        grid.EnableHeadersVisualStyles = false;
    }

    /// <summary>Dựng một thẻ KPI và giữ tham chiếu tới label giá trị / ghi chú.</summary>
    private Panel TaoTheChiSo(string khoa, string tieuDe, Color mau)
    {
        var the = new Panel { Dock = DockStyle.Fill, BackColor = Color.White };
        the.Paint += UI.UiHelpers.PaintCardBorder;

        var lblTieuDe = new Label
        {
            Text = tieuDe.ToUpperInvariant(),
            Dock = DockStyle.Top,
            Height = 34,
            Padding = new Padding(14, 0, 10, 0),
            TextAlign = ContentAlignment.BottomLeft,
            Font = new Font("Segoe UI", 8.75f, FontStyle.Bold),
            ForeColor = UI.Theme.Muted,
            AutoEllipsis = true
        };

        var lblGiaTri = new Label
        {
            Text = "—",
            Dock = DockStyle.Top,
            Height = 44,
            Padding = new Padding(14, 0, 10, 0),
            TextAlign = ContentAlignment.MiddleLeft,
            Font = new Font("Segoe UI", 20f, FontStyle.Bold),
            ForeColor = mau
        };

        var lblGhiChu = new Label
        {
            Text = string.Empty,
            Dock = DockStyle.Fill,
            Padding = new Padding(14, 0, 10, 8),
            TextAlign = ContentAlignment.TopLeft,
            Font = new Font("Segoe UI", 8.5f),
            ForeColor = UI.Theme.TextMedium,
            AutoEllipsis = true
        };

        the.Controls.Add(lblGhiChu);
        the.Controls.Add(lblGiaTri);
        the.Controls.Add(lblTieuDe);

        _oGiaTri[khoa] = lblGiaTri;
        _oGhiChu[khoa] = lblGhiChu;

        return the;
    }

    /// <summary>Cột của bảng chương trình đào tạo (dữ liệu THẬT từ /admin/courses).</summary>
    private void InitFeaturedCoursesGrid()
    {
        dgvFeaturedCourses.AutoGenerateColumns = false;
        dgvFeaturedCourses.Columns.Clear();

        dgvFeaturedCourses.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Ma",
            HeaderText = "Mã",
            FillWeight = 60
        });

        dgvFeaturedCourses.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "Ten",
            HeaderText = "Chương trình đào tạo",
            FillWeight = 170
        });

        dgvFeaturedCourses.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "CapDo",
            HeaderText = "Cấp độ",
            FillWeight = 90
        });

        dgvFeaturedCourses.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "SoBuoi",
            HeaderText = "Số buổi",
            FillWeight = 60
        });

        dgvFeaturedCourses.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "HocPhi",
            HeaderText = "Học phí",
            FillWeight = 100
        });

        dgvFeaturedCourses.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "SoLop",
            HeaderText = "Số lớp",
            FillWeight = 55
        });

        dgvFeaturedCourses.Columns.Add(new DataGridViewTextBoxColumn
        {
            DataPropertyName = "TrangThai",
            HeaderText = "Trạng thái",
            FillWeight = 90
        });

        dgvFeaturedCourses.CellDoubleClick += (_, _) => _formMain?.NavigateTo(NavItem.Courses);
    }

    /// <summary>Tải toàn bộ số liệu: báo cáo tổng quan, danh sách chương trình, số lượng GV/TG.</summary>
    private async Task TaiSoLieuAsync()
    {
        // Control có thể đã bị Dispose trong lúc chờ API -> không thao tác UI nữa.
        if (IsDisposed || Disposing)
        {
            return;
        }

        lblLoading.Text = "Đang đồng bộ số liệu...";

        var overviewTask = AppSession.Api.GetAsync<ReportOverviewDto>("admin/reports/overview");

        var coursesTask = AppSession.Api.GetAsync<PagedResult<CourseListItemDto>>(
            QueryStringBuilder.Build("admin/courses", ("page", "1"), ("pageSize", "10")));

        var giaoVienTask = AppSession.Api.GetAsync<PagedResult<TeacherListItemDto>>(
            QueryStringBuilder.Build(
                "admin/teachers", ("page", "1"), ("pageSize", "1"), ("loaiGiaoVien", TeacherTypes.GiaoVien)));

        var troGiangTask = AppSession.Api.GetAsync<PagedResult<TeacherListItemDto>>(
            QueryStringBuilder.Build(
                "admin/teachers", ("page", "1"), ("pageSize", "1"), ("loaiGiaoVien", TeacherTypes.TroGiang)));

        var activeAccountsTask = AppSession.Api.GetAsync<PagedResult<AccountListItemDto>>(
            QueryStringBuilder.Build(
                "admin/accounts", ("page", "1"), ("pageSize", "1"), ("status", AccountStatuses.HoatDong)));

        await Task.WhenAll(overviewTask, coursesTask, giaoVienTask, troGiangTask, activeAccountsTask);

        if (IsDisposed || Disposing)
        {
            return;
        }

        var overview = overviewTask.Result.Data;

        if (overview is null)
        {
            lblLoading.Text = $"Không tải được số liệu: {overviewTask.Result.ErrorMessage}";
            return;
        }

        // ---------- 8 thẻ KPI ----------
        GanThe("taiKhoan", overview.TongTaiKhoan, "Tài khoản trên hệ thống");
        GanThe("taiKhoanHoatDong", activeAccountsTask.Result.Data?.TotalItems ?? 0, "Đang được kích hoạt");
        GanThe("hocVien", overview.TongHocVien, "Hồ sơ học viên");
        GanThe("hocVienDangHoc", overview.SoHocVienDangHoc, "Đang theo học các lớp");
        GanThe("giaoVien", giaoVienTask.Result.Data?.TotalItems ?? overview.TongGiaoVien, "Hồ sơ giáo viên");
        GanThe("troGiang", troGiangTask.Result.Data?.TotalItems ?? 0, "Hồ sơ trợ giảng");
        GanThe("lopHoc", overview.TongLopHoc, $"Trong đó {overview.SoLopDangHoc} lớp đang học");
        GanThe("lopDangHoc", overview.SoLopDangHoc, $"/ {overview.TongLopHoc} lớp");

        // ---------- Bảng số liệu chi tiết ----------
        var chiTiet = new List<(string Nhom, string ChiTieu, string GiaTri)>
        {
            ("Danh mục", "Chương trình đào tạo", overview.TongChuongTrinh.ToString()),
            ("Danh mục", "Cấp độ", overview.TongCapDo.ToString()),
            ("Danh mục", "Khóa học", overview.TongKhoaHoc.ToString()),
            ("Danh mục", "Lớp học", overview.TongLopHoc.ToString()),
            ("Nhân sự", "Giáo viên", (giaoVienTask.Result.Data?.TotalItems ?? overview.TongGiaoVien).ToString()),
            ("Nhân sự", "Trợ giảng", (troGiangTask.Result.Data?.TotalItems ?? 0).ToString()),
            ("Nhân sự", "Nhân viên", overview.TongNhanVien.ToString()),
            ("Nhân sự", "Tổng tài khoản", overview.TongTaiKhoan.ToString()),
            ("Đăng ký học", "Tổng số đăng ký", overview.DangKy.TongSo.ToString()),
            ("Đăng ký học", "Chờ xử lý / chờ xếp lớp", overview.DangKy.ChoXuLy.ToString()),
            ("Đăng ký học", "Đã xếp lớp", overview.DangKy.DaXepLop.ToString()),
            ("Đăng ký học", "Đã hoàn thành", overview.DangKy.HoanThanh.ToString()),
            ("Đăng ký học", "Đã hủy", overview.DangKy.DaHuy.ToString()),
            ("Ca học", "Tổng số ca học", overview.CaHoc.TongSo.ToString()),
            ("Ca học", "Chưa diễn ra", overview.CaHoc.ChuaDienRa.ToString()),
            ("Ca học", "Đang diễn ra", overview.CaHoc.DangDienRa.ToString()),
            ("Ca học", "Sắp diễn ra", overview.CaHoc.SapDienRa.ToString()),
            ("Ca học", "Đã hoàn thành", overview.CaHoc.DaHoanThanh.ToString()),
            ("Ca học", "Đã hủy", overview.CaHoc.DaHuy.ToString()),
            ("Khảo sát", "Tổng số khảo sát", overview.KhaoSat.TongSo.ToString()),
            ("Khảo sát", "Đang hoạt động", overview.KhaoSat.SoHoatDong.ToString()),
            ("Khảo sát", "Câu hỏi", overview.KhaoSat.TongCauHoi.ToString()),
            ("Khảo sát", "Phản hồi", overview.KhaoSat.TongPhanHoi.ToString())
        };

        _dgvChiTiet.Rows.Clear();

        foreach (var dong in chiTiet)
        {
            var index = _dgvChiTiet.Rows.Add(dong.Nhom, dong.ChiTieu, dong.GiaTri);
            _dgvChiTiet.Rows[index].Cells[2].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            _dgvChiTiet.Rows[index].Cells[2].Style.Font = UI.Theme.FontBold;
        }

        // ---------- Học phí & Điểm danh ----------
        _dgvTaiChinh.Rows.Clear();

        var taiChinh = new List<(string Muc, string GiaTri)>
        {
            ("Tổng học phí phải thu", Tien(overview.HocPhi.TongPhaiThu)),
            ("Đã thu", Tien(overview.HocPhi.TongDaThu)),
            ("Còn thiếu", Tien(overview.HocPhi.TongConThieu)),
            ("Số phiếu học phí còn nợ", overview.HocPhi.SoHocPhiConNo.ToString()),
            ("Số học viên còn nợ", overview.HocPhi.SoHocVienConNo.ToString()),
            ("— Điểm danh —", string.Empty),
            ("Tổng lượt điểm danh", overview.DiemDanh.TongLuot.ToString()),
            ("Có mặt", overview.DiemDanh.CoMat.ToString()),
            ("Đi trễ", overview.DiemDanh.DiTre.ToString()),
            ("Vắng có phép", overview.DiemDanh.VangCoPhep.ToString()),
            ("Vắng không phép", overview.DiemDanh.Vang.ToString()),
            ("Tỉ lệ có mặt", $"{overview.DiemDanh.TiLeCoMat:0.##}%")
        };

        foreach (var dong in taiChinh)
        {
            var index = _dgvTaiChinh.Rows.Add(dong.Muc, dong.GiaTri);
            _dgvTaiChinh.Rows[index].Cells[1].Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            _dgvTaiChinh.Rows[index].Cells[1].Style.Font = UI.Theme.FontBold;
        }

        // ---------- Chương trình đào tạo (dữ liệu thật) ----------
        var khoaHoc = coursesTask.Result.Data?.Items;

        if (khoaHoc is not null)
        {
            dgvFeaturedCourses.DataSource = khoaHoc.Select(k => new
            {
                Ma = k.MaKhoaHoc,
                Ten = k.TenKhoaHoc,
                CapDo = k.TenCapDo,
                SoBuoi = k.SoBuoi,
                HocPhi = Tien(k.HocPhiCoBan),
                SoLop = k.SoLop,
                TrangThai = k.TrangThai
            }).ToList();
        }

        lblLoading.Text = $"Cập nhật lúc {DateTime.Now:HH:mm:ss}";
    }

    /// <summary>Gán số liệu cho một thẻ KPI.</summary>
    private void GanThe(string khoa, int giaTri, string ghiChu)
    {
        if (_oGiaTri.TryGetValue(khoa, out var lblGiaTri))
        {
            lblGiaTri.Text = giaTri.ToString("N0");
        }

        if (_oGhiChu.TryGetValue(khoa, out var lblGhiChu))
        {
            lblGhiChu.Text = ghiChu;
        }
    }

    private static string Tien(decimal soTien) => $"{soTien:N0} ₫";
}
