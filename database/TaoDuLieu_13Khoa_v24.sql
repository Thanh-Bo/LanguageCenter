/* =====================================================================================
   TaoDuLieu_13Khoa_v24.sql
   -------------------------------------------------------------------------------------
   MỤC ĐÍCH: Sinh dữ liệu mẫu đầy đủ cho 13 khoá học (1..13) — TRỪ khoá 14 (IELTS Advanced).

   MỖI KHOÁ CÓ:
     • 1 lớp học           (mã = <KyHieuCapDo>-01, VD IELTS-F-01)
     • 2 buổi/tuần         (LichHoc) — không trùng phòng & giờ với lớp khác
     • 20 ca học           (CaHoc)   — buổi lẻ = GV nước ngoài, buổi chẵn = GV Việt
     • 3 phân công         (GV chính + GV nước ngoài + Trợ giảng)
     • 1 học viên đăng ký  (DangKyHoc) — KHÔNG tạo HocPhi (để làm sau)

   THÊM MỚI: 13 GV Việt · 13 GV nước ngoài · 13 Trợ giảng · 13 Học viên
             (kèm 52 tài khoản, mật khẩu chung 111111)
             4 phòng học (P103..P106) → tổng 6 phòng

   ĐẦU ĐIỂM: 11 khoá (trừ mầm non 5,6) được chuẩn hoá về 6 đầu điểm
             Chuyên cần 10 · Bài tập 20 · Giữa kỳ 1 15 · Giữa kỳ 2 15
             · Cuối kỳ 1 20 · Cuối kỳ 2 20  = 100%
             Mầm non GIỮ NGUYÊN: Nghe / Nói / Thái độ / Tham gia (25% mỗi cái)

   NỘI DUNG: 20 buổi/khoá đã có sẵn → chỉ sửa buổi 8/13 (Kiểm tra) và 18/19 (Buổi thi)
             theo nhóm khoá; mầm non không có buổi thi.

   DỌN RÁC: xoá GV003..GV008, GV010, TG002, TG004 + mọi dữ liệu liên quan.

   AN TOÀN: bọc 1 transaction + XACT_ABORT. Nếu 13 lớp đã tồn tại → dừng ngay.
   -------------------------------------------------------------------------------------
   Chạy: SSMS → mở file → F5.
   ===================================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;

/* ---------------------------------------------------------------------------------
   CHẶN CHẠY LẠI: nếu lớp IELTS-F-01 đã có thì dừng (tránh nhân đôi dữ liệu)
   --------------------------------------------------------------------------------- */
IF EXISTS (SELECT 1 FROM dbo.LopHoc WHERE MaLop = N'IELTS-F-01')
BEGIN
    PRINT N'>>> Script đã chạy trước đó (đã có lớp IELTS-F-01). Không làm gì thêm.';
    RETURN;
END

BEGIN TRY
    BEGIN TRANSACTION;

    /* =============================================================================
       0. CẤU HÌNH
       ============================================================================= */
    DECLARE @NgayKhaiGiang1 date = '2026-11-09';   -- lớp 1 (Thứ Hai)
    DECLARE @LechTuan int = 7;                      -- mỗi lớp lệch 1 tuần
    DECLARE @MaNhanVienHocVu nvarchar(20) = N'NVHV001';
    DECLARE @MaNhanVienTuVan nvarchar(20) = N'NVTV001';

    PRINT N'=== BẮT ĐẦU TẠO DỮ LIỆU 13 KHÓA ===';

    /* =============================================================================
       1. CHO PHÉP LoaiBuoi = N''Kiểm tra'' (nếu database chưa chạy v20)
       ============================================================================= */
    IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_NoiDungBaiHoc_LoaiBuoi')
        ALTER TABLE dbo.NoiDungBaiHoc DROP CONSTRAINT CK_NoiDungBaiHoc_LoaiBuoi;

    ALTER TABLE dbo.NoiDungBaiHoc WITH CHECK ADD CONSTRAINT CK_NoiDungBaiHoc_LoaiBuoi
        CHECK (LoaiBuoi IN (N'Buổi học', N'Kiểm tra', N'Buổi thi'));

    IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_CaHoc_LoaiBuoi')
        ALTER TABLE dbo.CaHoc DROP CONSTRAINT CK_CaHoc_LoaiBuoi;

    ALTER TABLE dbo.CaHoc WITH CHECK ADD CONSTRAINT CK_CaHoc_LoaiBuoi
        CHECK (LoaiBuoi IN (N'Buổi học', N'Kiểm tra', N'Buổi thi'));

    PRINT N'  • Đã cập nhật CHECK LoaiBuoi (Buổi học / Kiểm tra / Buổi thi).';

    /* =============================================================================
       2. DỌN DỮ LIỆU RÁC
       ============================================================================= */
    DECLARE @Rac TABLE (Ma nvarchar(20) PRIMARY KEY);

    INSERT INTO @Rac (Ma) VALUES
        (N'GV003'), (N'GV004'), (N'GV005'), (N'GV006'), (N'GV007'),
        (N'GV008'), (N'GV010'), (N'TG002'), (N'TG004');

    DECLARE @SoRac int;
    SELECT @SoRac = COUNT(*) FROM dbo.GiaoVien WHERE MaGiaoVien IN (SELECT Ma FROM @Rac);

    -- gỡ tham chiếu trên ca học trước
    UPDATE dbo.CaHoc SET MaGiaoVienThucTe = NULL WHERE MaGiaoVienThucTe IN (SELECT Ma FROM @Rac);
    UPDATE dbo.CaHoc SET MaTroGiangThucTe = NULL WHERE MaTroGiangThucTe IN (SELECT Ma FROM @Rac);

    DELETE d FROM dbo.DiemSo d WHERE d.MaGiaoVien IN (SELECT Ma FROM @Rac);
    DELETE p FROM dbo.PhanCongGiangDay p WHERE p.MaGiaoVien IN (SELECT Ma FROM @Rac);
    DELETE b FROM dbo.BangCap b WHERE b.MaGiaoVien IN (SELECT Ma FROM @Rac);
    DELETE c FROM dbo.ChungChi c WHERE c.MaGiaoVien IN (SELECT Ma FROM @Rac);
    DELETE g FROM dbo.GiaoVien g WHERE g.MaGiaoVien IN (SELECT Ma FROM @Rac);

    -- tài khoản rác: xoá điểm danh / thông báo liên quan rồi mới xoá tài khoản
    DELETE dd
    FROM dbo.DiemDanh dd
    WHERE dd.MaTaiKhoanThucHien IN (SELECT t.MaTaiKhoan FROM dbo.TaiKhoan t WHERE t.TenDangNhap IN (SELECT Ma FROM @Rac));

    DELETE tb
    FROM dbo.ThongBao tb
    WHERE tb.MaTaiKhoanGui IN (SELECT t.MaTaiKhoan FROM dbo.TaiKhoan t WHERE t.TenDangNhap IN (SELECT Ma FROM @Rac))
       OR tb.MaTaiKhoanNhan IN (SELECT t.MaTaiKhoan FROM dbo.TaiKhoan t WHERE t.TenDangNhap IN (SELECT Ma FROM @Rac));

    DELETE t
    FROM dbo.TaiKhoan t
    WHERE t.TenDangNhap IN (SELECT Ma FROM @Rac);

    PRINT N'  • Đã dọn ' + CAST(@SoRac AS nvarchar(10)) + N' hồ sơ GV/TG rác và dữ liệu liên quan.';

    /* =============================================================================
       3. PHÒNG HỌC: thêm P103..P106, sửa tên P102
       ============================================================================= */
    IF EXISTS (SELECT 1 FROM dbo.PhongHoc WHERE MaPhong = N'P102')
        UPDATE dbo.PhongHoc SET TenPhong = N'Phòng 102', ViTri = N'Tầng 2 — Toà A'
        WHERE MaPhong = N'P102';

    IF NOT EXISTS (SELECT 1 FROM dbo.PhongHoc WHERE MaPhong = N'P103')
        INSERT INTO dbo.PhongHoc (MaPhong, TenPhong, ViTri, SucChua, ThietBi, TrangThai)
        VALUES (N'P103', N'Phòng 103', N'Tầng 2 — Toà A', 25, N'Máy chiếu, loa, máy lạnh', N'Hoạt động');

    IF NOT EXISTS (SELECT 1 FROM dbo.PhongHoc WHERE MaPhong = N'P104')
        INSERT INTO dbo.PhongHoc (MaPhong, TenPhong, ViTri, SucChua, ThietBi, TrangThai)
        VALUES (N'P104', N'Phòng 104', N'Tầng 3 — Toà A', 25, N'Máy chiếu, loa, máy lạnh', N'Hoạt động');

    IF NOT EXISTS (SELECT 1 FROM dbo.PhongHoc WHERE MaPhong = N'P105')
        INSERT INTO dbo.PhongHoc (MaPhong, TenPhong, ViTri, SucChua, ThietBi, TrangThai)
        VALUES (N'P105', N'Phòng 105', N'Tầng 3 — Toà B', 20, N'Loa, máy lạnh', N'Hoạt động');

    IF NOT EXISTS (SELECT 1 FROM dbo.PhongHoc WHERE MaPhong = N'P106')
        INSERT INTO dbo.PhongHoc (MaPhong, TenPhong, ViTri, SucChua, ThietBi, TrangThai)
        VALUES (N'P106', N'Phòng 106', N'Tầng 3 — Toà B', 20, N'Loa, máy lạnh', N'Hoạt động');

    PRINT N'  • Đã chuẩn bị 6 phòng học.';

    /* =============================================================================
       4. GIÁO VIÊN / TRỢ GIẢNG / HỌC VIÊN MỚI
       ============================================================================= */
    DECLARE @GvTiep int, @TgTiep int, @HvTiep int;

    SELECT @GvTiep = ISNULL(MAX(TRY_CONVERT(int, SUBSTRING(MaGiaoVien, 3, 10))), 0) + 1
    FROM dbo.GiaoVien WHERE MaGiaoVien LIKE N'GV%' AND TRY_CONVERT(int, SUBSTRING(MaGiaoVien, 3, 10)) IS NOT NULL;

    SELECT @TgTiep = ISNULL(MAX(TRY_CONVERT(int, SUBSTRING(MaGiaoVien, 3, 10))), 0) + 1
    FROM dbo.GiaoVien WHERE MaGiaoVien LIKE N'TG%' AND TRY_CONVERT(int, SUBSTRING(MaGiaoVien, 3, 10)) IS NOT NULL;

    SELECT @HvTiep = ISNULL(MAX(TRY_CONVERT(int, SUBSTRING(MaHocVien, 3, 10))), 0) + 1
    FROM dbo.HocVien WHERE MaHocVien LIKE N'HV%' AND TRY_CONVERT(int, SUBSTRING(MaHocVien, 3, 10)) IS NOT NULL;

    PRINT N'  • Mã tiếp theo: GV' + RIGHT(N'000' + CAST(@GvTiep AS nvarchar(10)), 3)
        + N' · TG' + RIGHT(N'000' + CAST(@TgTiep AS nvarchar(10)), 3)
        + N' · HV' + RIGHT(N'000' + CAST(@HvTiep AS nvarchar(10)), 3);

    -- Lấy hash mật khẩu mẫu (tài khoản chưa từng đăng nhập = mật khẩu demo 111111)
    DECLARE @MatKhauHash nvarchar(500);

    SELECT TOP (1) @MatKhauHash = MatKhauHash
    FROM dbo.TaiKhoan WHERE LanDangNhapCuoi IS NULL ORDER BY MaTaiKhoan;

    IF @MatKhauHash IS NULL
        SELECT TOP (1) @MatKhauHash = MatKhauHash FROM dbo.TaiKhoan ORDER BY MaTaiKhoan;

    IF @MatKhauHash IS NULL
    BEGIN
        RAISERROR(N'Không tìm thấy tài khoản mẫu để lấy mật khẩu (bảng TaiKhoan rỗng).', 16, 1);
    END

    -- 4.1 Danh sách 13 GV Việt
    DECLARE @GvVN TABLE (Stt int PRIMARY KEY, HoTen nvarchar(100), GioiTinh nvarchar(10), NgaySinh date, ChuyenMon nvarchar(150));

    INSERT INTO @GvVN (Stt, HoTen, GioiTinh, NgaySinh, ChuyenMon) VALUES
        (1,  N'Nguyễn Văn Hưng',   N'Nam', '1993-02-14', N'IELTS Foundation – Listening & Reading'),
        (2,  N'Trần Thị Mai Linh', N'Nữ',  '1994-07-09', N'IELTS Intermediate – Writing & Speaking'),
        (3,  N'Lê Hoàng Phúc',     N'Nam', '1991-11-21', N'Tiếng Anh giao tiếp'),
        (4,  N'Phạm Thu Hằng',     N'Nữ',  '1996-05-03', N'Tiếng Anh thiếu niên (Teen)'),
        (5,  N'Hoàng Anh Tuấn',    N'Nam', '1990-09-30', N'Tiếng Anh mầm non – phonics'),
        (6,  N'Vũ Thị Ngọc Hân',   N'Nữ',  '1995-12-12', N'Tiếng Anh mầm non – vận động & bài hát'),
        (7,  N'Đặng Quốc Bảo',     N'Nam', '1992-03-08', N'Cambridge Starters'),
        (8,  N'Bùi Thanh Vân',     N'Nữ',  '1997-08-19', N'Cambridge Movers'),
        (9,  N'Ngô Đức Thịnh',     N'Nam', '1989-06-25', N'Cambridge Flyers'),
        (10, N'Đỗ Thị Kim Ngân',   N'Nữ',  '1993-10-05', N'Tiếng Anh thiếu niên – ngữ pháp'),
        (11, N'Trịnh Minh Quân',   N'Nam', '1991-01-17', N'Tiếng Anh thiếu niên – 4 kỹ năng'),
        (12, N'Lý Bảo Trâm',       N'Nữ',  '1994-04-22', N'Giao tiếp trung cấp – B1'),
        (13, N'Phan Văn Kiệt',     N'Nam', '1988-07-11', N'Giao tiếp nâng cao – thuyết trình');

    -- 4.2 Danh sách 13 GV nước ngoài (tên hư cấu)
    DECLARE @GvNN TABLE (Stt int PRIMARY KEY, HoTen nvarchar(100), GioiTinh nvarchar(10), NgaySinh date, ChuyenMon nvarchar(150));

    INSERT INTO @GvNN (Stt, HoTen, GioiTinh, NgaySinh, ChuyenMon) VALUES
        (1,  N'Michael Andrew Smith',   N'Nam', '1987-04-18', N'Giáo viên bản ngữ – IELTS Foundation'),
        (2,  N'Sarah Elizabeth Johnson',N'Nữ',  '1990-09-02', N'Giáo viên bản ngữ – IELTS Intermediate'),
        (3,  N'David James Wilson',     N'Nam', '1985-12-27', N'Giáo viên bản ngữ – Giao tiếp'),
        (4,  N'Emily Rose Thompson',    N'Nữ',  '1992-06-14', N'Giáo viên bản ngữ – Teen'),
        (5,  N'Daniel Robert Brown',    N'Nam', '1988-02-05', N'Giáo viên bản ngữ – Mầm non'),
        (6,  N'Jessica Anne Miller',    N'Nữ',  '1993-08-23', N'Giáo viên bản ngữ – Mầm non'),
        (7,  N'Christopher Lee Davis',  N'Nam', '1986-10-30', N'Giáo viên bản ngữ – Cambridge Starters'),
        (8,  N'Rachel Marie Clark',     N'Nữ',  '1991-01-19', N'Giáo viên bản ngữ – Cambridge Movers'),
        (9,  N'Thomas Edward Walker',   N'Nam', '1984-05-08', N'Giáo viên bản ngữ – Cambridge Flyers'),
        (10, N'Laura Jane Harris',      N'Nữ',  '1989-11-11', N'Giáo viên bản ngữ – Teen Elementary'),
        (11, N'Andrew Paul Robinson',   N'Nam', '1987-07-26', N'Giáo viên bản ngữ – Teen Intermediate'),
        (12, N'Hannah Grace Lewis',     N'Nữ',  '1994-03-03', N'Giáo viên bản ngữ – Giao tiếp trung cấp'),
        (13, N'Matthew Scott Young',    N'Nam', '1986-09-16', N'Giáo viên bản ngữ – Giao tiếp nâng cao');

    -- 4.3 Danh sách 13 Trợ giảng
    DECLARE @Tg TABLE (Stt int PRIMARY KEY, HoTen nvarchar(100), GioiTinh nvarchar(10), NgaySinh date, ChuyenMon nvarchar(150));

    INSERT INTO @Tg (Stt, HoTen, GioiTinh, NgaySinh, ChuyenMon) VALUES
        (1,  N'Nguyễn Thị Thu Trang', N'Nữ', '2001-02-11', N'Trợ giảng lớp IELTS'),
        (2,  N'Trần Ngọc Ánh',        N'Nữ', '2002-06-07', N'Trợ giảng lớp IELTS'),
        (3,  N'Lê Thị Mỹ Duyên',      N'Nữ', '2000-09-24', N'Trợ giảng lớp giao tiếp'),
        (4,  N'Phạm Hồng Nhung',      N'Nữ', '2002-01-15', N'Trợ giảng lớp Teen'),
        (5,  N'Võ Thị Thanh Huyền',   N'Nữ', '2001-10-09', N'Trợ giảng lớp mầm non'),
        (6,  N'Đặng Thùy Dương',      N'Nữ', '2003-04-02', N'Trợ giảng lớp mầm non'),
        (7,  N'Bùi Minh Thư',         N'Nữ', '2001-07-21', N'Trợ giảng lớp thiếu nhi'),
        (8,  N'Hoàng Thị Lan Anh',    N'Nữ', '2002-11-05', N'Trợ giảng lớp thiếu nhi'),
        (9,  N'Ngô Thị Bích Trâm',    N'Nữ', '2000-05-13', N'Trợ giảng lớp thiếu nhi'),
        (10, N'Đỗ Thị Hải Yến',       N'Nữ', '2002-08-28', N'Trợ giảng lớp Teen'),
        (11, N'Trịnh Thị Phương Thảo',N'Nữ', '2001-12-30', N'Trợ giảng lớp Teen'),
        (12, N'Lý Thị Cẩm Tú',        N'Nữ', '2003-02-17', N'Trợ giảng lớp giao tiếp'),
        (13, N'Phan Thị Ngọc Diệp',   N'Nữ', '2000-03-06', N'Trợ giảng lớp giao tiếp');

    -- 4.4 Danh sách 13 Học viên
    DECLARE @Hv TABLE (Stt int PRIMARY KEY, HoTen nvarchar(100), GioiTinh nvarchar(10), NgaySinh date, TrinhDo nvarchar(100));

    INSERT INTO @Hv (Stt, HoTen, GioiTinh, NgaySinh, TrinhDo) VALUES
        (1,  N'Nguyễn Gia Bảo',    N'Nam', '2007-03-12', N'IELTS 5.0'),
        (2,  N'Trần Khánh Linh',   N'Nữ',  '2006-09-08', N'IELTS 5.5'),
        (3,  N'Lê Minh Khang',     N'Nam', '2005-01-26', N'Giao tiếp A2'),
        (4,  N'Phạm Bảo Ngọc',     N'Nữ',  '2012-06-19', N'Lớp 9 – tiếng Anh khá'),
        (5,  N'Hoàng Nhật Nam',    N'Nam', '2021-04-05', N'Mẫu giáo lớn'),
        (6,  N'Vũ Phương Thảo',    N'Nữ',  '2022-02-21', N'Mẫu giáo nhỡ'),
        (7,  N'Đặng Hữu Phước',    N'Nam', '2018-07-14', N'Sẵn sàng Cambridge Starters'),
        (8,  N'Bùi Thảo Vy',       N'Nữ',  '2017-11-03', N'Đã học Starters'),
        (9,  N'Ngô Gia Huy',       N'Nam', '2016-05-27', N'Đã học Movers'),
        (10, N'Đỗ Ngọc Diệp',      N'Nữ',  '2013-08-09', N'Lớp 8 – mục tiêu lớp 10'),
        (11, N'Trịnh Tuấn Kiệt',   N'Nam', '2012-12-01', N'Lớp 9 – mục tiêu lớp 10'),
        (12, N'Lý Khả Hân',        N'Nữ',  '1999-03-23', N'Giao tiếp B1'),
        (13, N'Phan Đức Anh',      N'Nam', '1997-10-17', N'Giao tiếp B2');

    -- 4.5 Bảng tạm hợp nhất giáo viên / trợ giảng
    CREATE TABLE #GV (
        Ma nvarchar(20) PRIMARY KEY,
        HoTen nvarchar(100),
        GioiTinh nvarchar(10),
        NgaySinh date,
        LoaiGiaoVien nvarchar(20),
        ChuyenMon nvarchar(150),
        MaVaiTro int,
        Email nvarchar(150),
        DienThoai nvarchar(20)
    );

    INSERT INTO #GV (Ma, HoTen, GioiTinh, NgaySinh, LoaiGiaoVien, ChuyenMon, MaVaiTro, Email, DienThoai)
    SELECT N'GV' + RIGHT(N'000' + CAST(@GvTiep + Stt - 1 AS nvarchar(10)), 3),
           HoTen, GioiTinh, NgaySinh, N'Giáo viên', ChuyenMon, 5,
           N'gv' + RIGHT(N'000' + CAST(@GvTiep + Stt - 1 AS nvarchar(10)), 3) + N'@trungtam.edu.vn',
           N'0912' + RIGHT(N'000000' + CAST(@GvTiep + Stt - 1 AS nvarchar(10)), 6)
    FROM @GvVN;

    INSERT INTO #GV (Ma, HoTen, GioiTinh, NgaySinh, LoaiGiaoVien, ChuyenMon, MaVaiTro, Email, DienThoai)
    SELECT N'GV' + RIGHT(N'000' + CAST(@GvTiep + 13 + Stt - 1 AS nvarchar(10)), 3),
           HoTen, GioiTinh, NgaySinh, N'Giáo viên', ChuyenMon, 5,
           N'gv' + RIGHT(N'000' + CAST(@GvTiep + 13 + Stt - 1 AS nvarchar(10)), 3) + N'@trungtam.edu.vn',
           N'0913' + RIGHT(N'000000' + CAST(@GvTiep + 13 + Stt - 1 AS nvarchar(10)), 6)
    FROM @GvNN;

    INSERT INTO #GV (Ma, HoTen, GioiTinh, NgaySinh, LoaiGiaoVien, ChuyenMon, MaVaiTro, Email, DienThoai)
    SELECT N'TG' + RIGHT(N'000' + CAST(@TgTiep + Stt - 1 AS nvarchar(10)), 3),
           HoTen, GioiTinh, NgaySinh, N'Trợ giảng', ChuyenMon, 6,
           N'tg' + RIGHT(N'000' + CAST(@TgTiep + Stt - 1 AS nvarchar(10)), 3) + N'@trungtam.edu.vn',
           N'0914' + RIGHT(N'000000' + CAST(@TgTiep + Stt - 1 AS nvarchar(10)), 6)
    FROM @Tg;

    -- 4.6 Tạo tài khoản + hồ sơ giáo viên / trợ giảng
    INSERT INTO dbo.TaiKhoan (MaVaiTro, TenDangNhap, MatKhauHash, Email, TrangThaiTaiKhoan, NgayTao)
    SELECT MaVaiTro, Ma, @MatKhauHash, Email, N'Hoạt động', GETDATE() FROM #GV;

    INSERT INTO dbo.GiaoVien (MaGiaoVien, HoTen, NgaySinh, GioiTinh, SoDienThoai, Email, DiaChi,
                              LoaiGiaoVien, ChuyenMon, SoNamKinhNghiem, NgayVaoLam, TrangThai)
    SELECT Ma, HoTen, NgaySinh, GioiTinh, DienThoai, Email, N'TP. Hồ Chí Minh',
           LoaiGiaoVien, ChuyenMon, 3, CAST('2026-10-01' AS date), N'Đang làm việc'
    FROM #GV;

    DECLARE @SoGvMoi int;
    SELECT @SoGvMoi = COUNT(*) FROM #GV;

    PRINT N'  • Đã tạo ' + CAST(@SoGvMoi AS nvarchar(10)) + N' tài khoản + hồ sơ GV/TG.';

    -- 4.7 Học viên
    CREATE TABLE #HV (
        Ma nvarchar(20) PRIMARY KEY,
        HoTen nvarchar(100),
        GioiTinh nvarchar(10),
        NgaySinh date,
        TrinhDo nvarchar(100),
        Email nvarchar(150),
        DienThoai nvarchar(20)
    );

    INSERT INTO #HV (Ma, HoTen, GioiTinh, NgaySinh, TrinhDo, Email, DienThoai)
    SELECT N'HV' + RIGHT(N'000' + CAST(@HvTiep + Stt - 1 AS nvarchar(10)), 3),
           HoTen, GioiTinh, NgaySinh, TrinhDo,
           N'hv' + RIGHT(N'000' + CAST(@HvTiep + Stt - 1 AS nvarchar(10)), 3) + N'@gmail.com',
           N'0934' + RIGHT(N'000000' + CAST(@HvTiep + Stt - 1 AS nvarchar(10)), 6)
    FROM @Hv;

    INSERT INTO dbo.TaiKhoan (MaVaiTro, TenDangNhap, MatKhauHash, Email, TrangThaiTaiKhoan, NgayTao)
    SELECT 7, Ma, @MatKhauHash, Email, N'Hoạt động', GETDATE() FROM #HV;

    INSERT INTO dbo.HocVien (MaHocVien, MaPhuHuynh, HoTen, NgaySinh, GioiTinh, SoDienThoai, Email, DiaChi,
                             MoiQuanHePhuHuynh, MucTieuHocTap, TrinhDoHienTai, TrangThaiHocTap,
                             NgayTaoHoSo, NgayNhapHoc)
    SELECT Ma, NULL, HoTen, NgaySinh, GioiTinh, DienThoai, Email, N'TP. Hồ Chí Minh',
           NULL, N'Hoàn thành khóa học đúng tiến độ', TrinhDo, N'Đang học',
           GETDATE(), NULL
    FROM #HV;

    DECLARE @SoHvMoi int;
    SELECT @SoHvMoi = COUNT(*) FROM #HV;

    PRINT N'  • Đã tạo ' + CAST(@SoHvMoi AS nvarchar(10)) + N' tài khoản + hồ sơ học viên.';

    /* =============================================================================
       5. ĐẦU ĐIỂM CHUẨN (6 đầu điểm) CHO 11 KHOÁ — BỎ QUA MẦM NON (5, 6)
       ============================================================================= */
    DECLARE @KhoaChuan TABLE (MaKhoaHoc int PRIMARY KEY);
    INSERT INTO @KhoaChuan (MaKhoaHoc) VALUES (1), (2), (3), (4), (7), (8), (9), (10), (11), (12), (13);

    DECLARE @k int;

    DECLARE curDauDiem CURSOR LOCAL FAST_FORWARD FOR SELECT MaKhoaHoc FROM @KhoaChuan;
    OPEN curDauDiem;
    FETCH NEXT FROM curDauDiem INTO @k;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- 5.1 Xoá đầu điểm lạ CHƯA có điểm (VD: "Đánh giá nói" của khoá 3)
        DELETE ld
        FROM dbo.LoaiDiem ld
        WHERE ld.MaKhoaHoc = @k
          AND ld.TenLoaiDiem NOT IN (N'Chuyên cần', N'Bài tập', N'Giữa kỳ', N'Giữa kỳ 1',
                                     N'Giữa kỳ 2', N'Cuối kỳ', N'Cuối kỳ 1', N'Cuối kỳ 2')
          AND NOT EXISTS (SELECT 1 FROM dbo.DiemSo d WHERE d.MaLoaiDiem = ld.MaLoaiDiem);

        -- 5.2 Đầu điểm lạ ĐÃ có điểm → ngừng sử dụng (không xoá dữ liệu)
        UPDATE ld
        SET TrangThai = N'Ngừng sử dụng'
        FROM dbo.LoaiDiem ld
        WHERE ld.MaKhoaHoc = @k
          AND ld.TenLoaiDiem NOT IN (N'Chuyên cần', N'Bài tập', N'Giữa kỳ', N'Giữa kỳ 1',
                                     N'Giữa kỳ 2', N'Cuối kỳ', N'Cuối kỳ 1', N'Cuối kỳ 2');

        -- 5.3 Đổi tên Giữa kỳ → Giữa kỳ 1, Cuối kỳ → Cuối kỳ 1 (giữ nguyên điểm cũ)
        IF EXISTS (SELECT 1 FROM dbo.LoaiDiem WHERE MaKhoaHoc = @k AND TenLoaiDiem = N'Giữa kỳ')
           AND NOT EXISTS (SELECT 1 FROM dbo.LoaiDiem WHERE MaKhoaHoc = @k AND TenLoaiDiem = N'Giữa kỳ 1')
            UPDATE dbo.LoaiDiem SET TenLoaiDiem = N'Giữa kỳ 1'
            WHERE MaKhoaHoc = @k AND TenLoaiDiem = N'Giữa kỳ';

        IF EXISTS (SELECT 1 FROM dbo.LoaiDiem WHERE MaKhoaHoc = @k AND TenLoaiDiem = N'Cuối kỳ')
           AND NOT EXISTS (SELECT 1 FROM dbo.LoaiDiem WHERE MaKhoaHoc = @k AND TenLoaiDiem = N'Cuối kỳ 1')
            UPDATE dbo.LoaiDiem SET TenLoaiDiem = N'Cuối kỳ 1'
            WHERE MaKhoaHoc = @k AND TenLoaiDiem = N'Cuối kỳ';

        -- 5.4 Bổ sung đầu điểm còn thiếu (ThuTu tạm 90.. để không vướng UQ_LoaiDiem_ThuTu)
        IF NOT EXISTS (SELECT 1 FROM dbo.LoaiDiem WHERE MaKhoaHoc = @k AND TenLoaiDiem = N'Chuyên cần')
            INSERT INTO dbo.LoaiDiem (MaKhoaHoc, TenLoaiDiem, TrongSo, DiemToiDa, ThuTu, TrangThai)
            VALUES (@k, N'Chuyên cần', 10, 10, 90, N'Hoạt động');

        IF NOT EXISTS (SELECT 1 FROM dbo.LoaiDiem WHERE MaKhoaHoc = @k AND TenLoaiDiem = N'Bài tập')
            INSERT INTO dbo.LoaiDiem (MaKhoaHoc, TenLoaiDiem, TrongSo, DiemToiDa, ThuTu, TrangThai)
            VALUES (@k, N'Bài tập', 20, 10, 91, N'Hoạt động');

        IF NOT EXISTS (SELECT 1 FROM dbo.LoaiDiem WHERE MaKhoaHoc = @k AND TenLoaiDiem = N'Giữa kỳ 1')
            INSERT INTO dbo.LoaiDiem (MaKhoaHoc, TenLoaiDiem, TrongSo, DiemToiDa, ThuTu, TrangThai)
            VALUES (@k, N'Giữa kỳ 1', 15, 10, 92, N'Hoạt động');

        IF NOT EXISTS (SELECT 1 FROM dbo.LoaiDiem WHERE MaKhoaHoc = @k AND TenLoaiDiem = N'Giữa kỳ 2')
            INSERT INTO dbo.LoaiDiem (MaKhoaHoc, TenLoaiDiem, TrongSo, DiemToiDa, ThuTu, TrangThai)
            VALUES (@k, N'Giữa kỳ 2', 15, 10, 93, N'Hoạt động');

        IF NOT EXISTS (SELECT 1 FROM dbo.LoaiDiem WHERE MaKhoaHoc = @k AND TenLoaiDiem = N'Cuối kỳ 1')
            INSERT INTO dbo.LoaiDiem (MaKhoaHoc, TenLoaiDiem, TrongSo, DiemToiDa, ThuTu, TrangThai)
            VALUES (@k, N'Cuối kỳ 1', 20, 10, 94, N'Hoạt động');

        IF NOT EXISTS (SELECT 1 FROM dbo.LoaiDiem WHERE MaKhoaHoc = @k AND TenLoaiDiem = N'Cuối kỳ 2')
            INSERT INTO dbo.LoaiDiem (MaKhoaHoc, TenLoaiDiem, TrongSo, DiemToiDa, ThuTu, TrangThai)
            VALUES (@k, N'Cuối kỳ 2', 20, 10, 95, N'Hoạt động');

        -- 5.5 Gán trọng số chuẩn
        UPDATE dbo.LoaiDiem SET TrongSo = 10 WHERE MaKhoaHoc = @k AND TenLoaiDiem = N'Chuyên cần';
        UPDATE dbo.LoaiDiem SET TrongSo = 20 WHERE MaKhoaHoc = @k AND TenLoaiDiem = N'Bài tập';
        UPDATE dbo.LoaiDiem SET TrongSo = 15 WHERE MaKhoaHoc = @k AND TenLoaiDiem = N'Giữa kỳ 1';
        UPDATE dbo.LoaiDiem SET TrongSo = 15 WHERE MaKhoaHoc = @k AND TenLoaiDiem = N'Giữa kỳ 2';
        UPDATE dbo.LoaiDiem SET TrongSo = 20 WHERE MaKhoaHoc = @k AND TenLoaiDiem = N'Cuối kỳ 1';
        UPDATE dbo.LoaiDiem SET TrongSo = 20 WHERE MaKhoaHoc = @k AND TenLoaiDiem = N'Cuối kỳ 2';

        -- 5.6 Đánh số lại ThuTu 1..6 (2 lượt để không vướng UNIQUE)
        UPDATE dbo.LoaiDiem SET ThuTu = 1000 + MaLoaiDiem WHERE MaKhoaHoc = @k;

        UPDATE dbo.LoaiDiem
        SET ThuTu = CASE TenLoaiDiem
                        WHEN N'Chuyên cần' THEN 1
                        WHEN N'Bài tập'    THEN 2
                        WHEN N'Giữa kỳ 1'  THEN 3
                        WHEN N'Giữa kỳ 2'  THEN 4
                        WHEN N'Cuối kỳ 1'  THEN 5
                        WHEN N'Cuối kỳ 2'  THEN 6
                    END
        WHERE MaKhoaHoc = @k
          AND TenLoaiDiem IN (N'Chuyên cần', N'Bài tập', N'Giữa kỳ 1', N'Giữa kỳ 2', N'Cuối kỳ 1', N'Cuối kỳ 2');

        FETCH NEXT FROM curDauDiem INTO @k;
    END

    CLOSE curDauDiem;
    DEALLOCATE curDauDiem;

    PRINT N'  • Đã chuẩn hoá 6 đầu điểm cho 11 khoá (mầm non giữ nguyên).';

    /* =============================================================================
       6. NỘI DUNG 20 BUỔI: đặt LoaiBuoi + sửa tiêu đề buổi kiểm tra/thi
       ============================================================================= */

    -- 6.1 IELTS (khoá 1, 2): KT1 = Reading & Writing · KT2 = Speaking & Listening
    UPDATE dbo.NoiDungBaiHoc SET TieuDe = N'Kiểm tra giữa kỳ 1 — Reading & Writing',
           NoiDung = N'Bài kiểm tra giữa kỳ 1 gồm 2 kỹ năng Reading và Writing.', LoaiBuoi = N'Kiểm tra'
    WHERE MaKhoaHoc IN (1, 2) AND SoBuoi = 8;

    UPDATE dbo.NoiDungBaiHoc SET TieuDe = N'Kiểm tra giữa kỳ 2 — Speaking & Listening',
           NoiDung = N'Bài kiểm tra giữa kỳ 2 gồm 2 kỹ năng Speaking và Listening.', LoaiBuoi = N'Kiểm tra'
    WHERE MaKhoaHoc IN (1, 2) AND SoBuoi = 13;

    UPDATE dbo.NoiDungBaiHoc SET TieuDe = N'Bài thi cuối kỳ 1 — Reading & Writing',
           NoiDung = N'Bài thi cuối kỳ 1 gồm 2 kỹ năng Reading và Writing.', LoaiBuoi = N'Buổi thi'
    WHERE MaKhoaHoc IN (1, 2) AND SoBuoi = 18;

    UPDATE dbo.NoiDungBaiHoc SET TieuDe = N'Bài thi cuối kỳ 2 — Speaking & Listening',
           NoiDung = N'Bài thi cuối kỳ 2 gồm 2 kỹ năng Speaking và Listening.', LoaiBuoi = N'Buổi thi'
    WHERE MaKhoaHoc IN (1, 2) AND SoBuoi = 19;

    -- 6.2 Giao tiếp (3, 12, 13) + Teen (4, 10, 11): KT1 = Listening & Speaking · KT2 = Reading & Writing
    UPDATE dbo.NoiDungBaiHoc SET TieuDe = N'Kiểm tra giữa kỳ 1 — Listening & Speaking',
           NoiDung = N'Kiểm tra giữa kỳ 1: nghe hiểu và phản xạ nói theo tình huống.', LoaiBuoi = N'Kiểm tra'
    WHERE MaKhoaHoc IN (3, 4, 10, 11, 12, 13) AND SoBuoi = 8;

    UPDATE dbo.NoiDungBaiHoc SET TieuDe = N'Kiểm tra giữa kỳ 2 — Reading & Writing',
           NoiDung = N'Kiểm tra giữa kỳ 2: đọc hiểu và viết đoạn văn/email ngắn.', LoaiBuoi = N'Kiểm tra'
    WHERE MaKhoaHoc IN (3, 4, 10, 11, 12, 13) AND SoBuoi = 13;

    UPDATE dbo.NoiDungBaiHoc SET TieuDe = N'Bài thi cuối kỳ 1 — Listening & Speaking',
           NoiDung = N'Thi cuối kỳ 1: nghe – nói tổng hợp (role-play/thuyết trình ngắn).', LoaiBuoi = N'Buổi thi'
    WHERE MaKhoaHoc IN (3, 4, 10, 11, 12, 13) AND SoBuoi = 18;

    UPDATE dbo.NoiDungBaiHoc SET TieuDe = N'Bài thi cuối kỳ 2 — Reading & Writing',
           NoiDung = N'Thi cuối kỳ 2: đọc hiểu và bài viết tổng hợp.', LoaiBuoi = N'Buổi thi'
    WHERE MaKhoaHoc IN (3, 4, 10, 11, 12, 13) AND SoBuoi = 19;

    -- 6.3 Tiểu học Cambridge (7, 8, 9): KT1 = Listening & Reading/Writing · KT2 = Speaking
    UPDATE dbo.NoiDungBaiHoc SET TieuDe = N'Kiểm tra giữa kỳ 1 — Listening & Reading/Writing',
           NoiDung = N'Kiểm tra giữa kỳ 1 theo dạng bài Cambridge: Listening và Reading & Writing.', LoaiBuoi = N'Kiểm tra'
    WHERE MaKhoaHoc IN (7, 8, 9) AND SoBuoi = 8;

    UPDATE dbo.NoiDungBaiHoc SET TieuDe = N'Kiểm tra giữa kỳ 2 — Speaking',
           NoiDung = N'Kiểm tra giữa kỳ 2: kỹ năng Speaking theo chủ đề đã học.', LoaiBuoi = N'Kiểm tra'
    WHERE MaKhoaHoc IN (7, 8, 9) AND SoBuoi = 13;

    UPDATE dbo.NoiDungBaiHoc SET TieuDe = N'Bài thi cuối kỳ 1 — Listening & Reading/Writing',
           NoiDung = N'Thi cuối kỳ 1 theo dạng đề Cambridge: Listening và Reading & Writing.', LoaiBuoi = N'Buổi thi'
    WHERE MaKhoaHoc IN (7, 8, 9) AND SoBuoi = 18;

    UPDATE dbo.NoiDungBaiHoc SET TieuDe = N'Bài thi cuối kỳ 2 — Speaking',
           NoiDung = N'Thi cuối kỳ 2: phỏng vấn Speaking theo chuẩn Cambridge.', LoaiBuoi = N'Buổi thi'
    WHERE MaKhoaHoc IN (7, 8, 9) AND SoBuoi = 19;

    -- 6.4 Buổi 9 (trước là "Kiểm tra giữa kỳ 1") → chữa bài, không phải buổi thi
    UPDATE dbo.NoiDungBaiHoc SET TieuDe = N'Chữa bài kiểm tra giữa kỳ 1 & phản hồi cá nhân',
           NoiDung = N'Chữa bài kiểm tra giữa kỳ 1, phân tích lỗi và kế hoạch cải thiện.', LoaiBuoi = N'Buổi học'
    WHERE MaKhoaHoc IN (1, 2, 3, 4, 7, 8, 9, 10, 11, 12, 13) AND SoBuoi = 9;

    -- 6.5 Mầm non (5, 6): KHÔNG có buổi kiểm tra/thi — đánh giá qua quan sát
    UPDATE dbo.NoiDungBaiHoc SET TieuDe = N'Ôn tập & đánh giá qua quan sát (giữa kỳ)',
           NoiDung = N'Trò chơi ôn tập; giáo viên quan sát và ghi nhận mức độ Nghe – Nói – Thái độ – Tham gia.', LoaiBuoi = N'Buổi học'
    WHERE MaKhoaHoc IN (5, 6) AND SoBuoi = 9;

    UPDATE dbo.NoiDungBaiHoc SET TieuDe = N'Tổng kết & đánh giá qua quan sát (cuối kỳ)',
           NoiDung = N'Tổng kết khóa; đánh giá qua hồ sơ học tập (portfolio) và quan sát trên lớp.', LoaiBuoi = N'Buổi học'
    WHERE MaKhoaHoc IN (5, 6) AND SoBuoi = 18;

    PRINT N'  • Đã cập nhật nội dung buổi kiểm tra / buổi thi cho 13 khoá.';

    /* =============================================================================
       7. LỚP HỌC + LỊCH HỌC
       Lịch được xếp để KHÔNG trùng (thứ, khung giờ, phòng) giữa các lớp.
       ============================================================================= */
    CREATE TABLE #Lop (
        Stt int PRIMARY KEY,
        MaKhoaHoc int,
        MaLop nvarchar(50),
        TenLop nvarchar(150),
        Thu1 tinyint,
        Thu2 tinyint,
        GioBatDau time,
        GioKetThuc time,
        MaPhong nvarchar(20)
    );

    INSERT INTO #Lop (Stt, MaKhoaHoc, MaLop, TenLop, Thu1, Thu2, GioBatDau, GioKetThuc, MaPhong) VALUES
        (1,  1,  N'IELTS-F-01', N'IELTS Foundation K01',                  1, 4, CAST('17:30' AS time), CAST('19:30' AS time), N'P101'),
        (2,  2,  N'IELTS-I-01', N'IELTS Intermediate K01',                1, 4, CAST('19:30' AS time), CAST('21:30' AS time), N'P101'),
        (3,  3,  N'COMM-B-01',  N'English Communication Basic K01',       1, 4, CAST('19:30' AS time), CAST('21:30' AS time), N'P102'),
        (4,  4,  N'TEEN-1-01',  N'Teen Starter K01',                      2, 5, CAST('17:30' AS time), CAST('19:30' AS time), N'P101'),
        (5,  5,  N'PRE-1-01',   N'Mầm non Preschool 1 K01',               3, 6, CAST('08:00' AS time), CAST('10:00' AS time), N'P102'),
        (6,  6,  N'PRE-2-01',   N'Mầm non Preschool 2 K01',               3, 6, CAST('14:00' AS time), CAST('16:00' AS time), N'P102'),
        (7,  7,  N'PRI-S-01',   N'Tiểu học Starters K01',                 4, 7, CAST('08:00' AS time), CAST('10:00' AS time), N'P101'),
        (8,  8,  N'PRI-M-01',   N'Tiểu học Movers K01',                   4, 7, CAST('14:00' AS time), CAST('16:00' AS time), N'P101'),
        (9,  9,  N'PRI-F-01',   N'Tiểu học Flyers K01',                   4, 7, CAST('17:30' AS time), CAST('19:30' AS time), N'P102'),
        (10, 10, N'TEEN-2-01',  N'Teen Elementary K01',                   1, 4, CAST('14:00' AS time), CAST('16:00' AS time), N'P103'),
        (11, 11, N'TEEN-3-01',  N'Teen Intermediate K01',                 3, 6, CAST('08:00' AS time), CAST('10:00' AS time), N'P103'),
        (12, 12, N'COMM-I-01',  N'English Communication Intermediate K01',3, 6, CAST('19:30' AS time), CAST('21:30' AS time), N'P103'),
        (13, 13, N'COMM-N-01',  N'English Communication Advanced K01',    3, 7, CAST('08:00' AS time), CAST('10:00' AS time), N'P104');

    -- 7.1 Lớp học
    INSERT INTO dbo.LopHoc (MaLop, TenLop, MaKhoaHoc, MaNhanVienHocVu, NgayBatDau, NgayKetThucDuKien,
                            NgayKetThucThucTe, SiSoToiDa, TrangThai, GioBatDau, GioKetThuc, ThuBuoi1, ThuBuoi2)
    SELECT l.MaLop, l.TenLop, l.MaKhoaHoc, @MaNhanVienHocVu,
           DATEADD(day, (l.Stt - 1) * @LechTuan, @NgayKhaiGiang1)                     AS NgayBatDau,
           DATEADD(day, (l.Stt - 1) * @LechTuan + 69, @NgayKhaiGiang1)                AS NgayKetThucDuKien,
           NULL, 20,
           CASE WHEN DATEADD(day, (l.Stt - 1) * @LechTuan, @NgayKhaiGiang1) <= CAST(GETDATE() AS date)
                THEN N'Đang học' ELSE N'Chuẩn bị mở' END,
           l.GioBatDau, l.GioKetThuc, l.Thu1, l.Thu2
    FROM #Lop l;

    PRINT N'  • Đã tạo 13 lớp học.';

    -- 7.2 Lịch học: mỗi lớp 2 buổi/tuần
    INSERT INTO dbo.LichHoc (MaLopHoc, MaPhongHoc, ThuTrongTuan, GioBatDau, GioKetThuc, NgayApDungTu, TrangThai)
    SELECT lop.MaLopHoc, ph.MaPhongHoc, x.Thu, l.GioBatDau, l.GioKetThuc, lop.NgayBatDau, N'Hoạt động'
    FROM #Lop l
    INNER JOIN dbo.LopHoc lop ON lop.MaLop = l.MaLop
    INNER JOIN dbo.PhongHoc ph ON ph.MaPhong = l.MaPhong
    CROSS APPLY (VALUES (l.Thu1), (l.Thu2)) AS x(Thu);

    PRINT N'  • Đã tạo 26 lịch học lặp (2 buổi/lớp, không trùng phòng & giờ).';

    /* =============================================================================
       8. PHÂN CÔNG GIẢNG DẠY — mỗi lớp: GV chính (Việt) + GV nước ngoài + Trợ giảng
       ============================================================================= */
    CREATE TABLE #PhanCong (
        Stt int,
        VaiTro nvarchar(50),
        MaGiaoVien nvarchar(20)
    );

    INSERT INTO #PhanCong (Stt, VaiTro, MaGiaoVien)
    SELECT Stt, N'Giáo viên chính', N'GV' + RIGHT(N'000' + CAST(@GvTiep + Stt - 1 AS nvarchar(10)), 3) FROM @GvVN;

    INSERT INTO #PhanCong (Stt, VaiTro, MaGiaoVien)
    SELECT Stt, N'Giáo viên nước ngoài', N'GV' + RIGHT(N'000' + CAST(@GvTiep + 13 + Stt - 1 AS nvarchar(10)), 3) FROM @GvNN;

    INSERT INTO #PhanCong (Stt, VaiTro, MaGiaoVien)
    SELECT Stt, N'Trợ giảng', N'TG' + RIGHT(N'000' + CAST(@TgTiep + Stt - 1 AS nvarchar(10)), 3) FROM @Tg;

    INSERT INTO dbo.PhanCongGiangDay (MaLopHoc, MaGiaoVien, VaiTroPhanCong, NgayBatDau, NgayKetThuc, TrangThai, GhiChu)
    SELECT lop.MaLopHoc, pc.MaGiaoVien, pc.VaiTro, lop.NgayBatDau, NULL, N'Đang phân công',
           N'Phân công tự động (script v24)'
    FROM #PhanCong pc
    INNER JOIN #Lop l ON l.Stt = pc.Stt
    INNER JOIN dbo.LopHoc lop ON lop.MaLop = l.MaLop;

    PRINT N'  • Đã tạo 39 phân công giảng dạy (3/lớp).';

    /* =============================================================================
       9. CA HỌC: 20 buổi/lớp
          - buổi lẻ  → Giáo viên nước ngoài
          - buổi chẵn → Giáo viên chính (Việt)
          - bỏ qua ngày lễ (bảng NgayLe)
       ============================================================================= */
    DECLARE @maLopHoc int, @maKhoaHoc int, @ngayBatDau date,
            @thu1 tinyint, @thu2 tinyint, @gioBD time, @gioKT time, @maPhongHoc int,
            @maGvViet nvarchar(20), @maGvNuocNgoai nvarchar(20), @maTroGiang nvarchar(20);

    DECLARE @n int, @ngay date, @thuHomNay tinyint, @thuCanTim tinyint,
            @tieuDe nvarchar(200), @noiDung nvarchar(500), @loaiBuoi nvarchar(50),
            @maLichHoc int, @giaoVien nvarchar(20);

    DECLARE curLop CURSOR LOCAL FAST_FORWARD FOR
        SELECT lop.MaLopHoc, l.MaKhoaHoc, lop.NgayBatDau, l.Thu1, l.Thu2, l.GioBatDau, l.GioKetThuc,
               ph.MaPhongHoc,
               N'GV' + RIGHT(N'000' + CAST(@GvTiep + l.Stt - 1 AS nvarchar(10)), 3),
               N'GV' + RIGHT(N'000' + CAST(@GvTiep + 13 + l.Stt - 1 AS nvarchar(10)), 3),
               N'TG' + RIGHT(N'000' + CAST(@TgTiep + l.Stt - 1 AS nvarchar(10)), 3)
        FROM #Lop l
        INNER JOIN dbo.LopHoc lop ON lop.MaLop = l.MaLop
        INNER JOIN dbo.PhongHoc ph ON ph.MaPhong = l.MaPhong
        ORDER BY l.Stt;

    OPEN curLop;
    FETCH NEXT FROM curLop INTO @maLopHoc, @maKhoaHoc, @ngayBatDau, @thu1, @thu2,
                                @gioBD, @gioKT, @maPhongHoc, @maGvViet, @maGvNuocNgoai, @maTroGiang;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @n = 1;
        SET @ngay = @ngayBatDau;

        WHILE @n <= 20
        BEGIN
            -- thứ (ISO: 1 = Thứ Hai) của @ngay
            SET @thuHomNay = CAST((DATEDIFF(day, CAST('19000101' AS date), @ngay) % 7) + 1 AS tinyint);
            SET @thuCanTim = CASE WHEN @n % 2 = 1 THEN @thu1 ELSE @thu2 END;

            IF @thuHomNay = @thuCanTim
               AND NOT EXISTS (SELECT 1 FROM dbo.NgayLe nl WHERE nl.Ngay = @ngay)
            BEGIN
                SET @tieuDe = NULL;
                SET @noiDung = NULL;
                SET @loaiBuoi = NULL;
                SET @maLichHoc = NULL;
                SET @giaoVien = CASE WHEN @n % 2 = 1 THEN @maGvNuocNgoai ELSE @maGvViet END;

                SELECT @tieuDe = nd.TieuDe, @noiDung = nd.NoiDung, @loaiBuoi = nd.LoaiBuoi
                FROM dbo.NoiDungBaiHoc nd
                WHERE nd.MaKhoaHoc = @maKhoaHoc AND nd.SoBuoi = @n;

                SELECT @maLichHoc = lh.MaLichHoc
                FROM dbo.LichHoc lh
                WHERE lh.MaLopHoc = @maLopHoc AND lh.ThuTrongTuan = @thuHomNay;

                IF @tieuDe IS NOT NULL AND @maLichHoc IS NOT NULL
                BEGIN
                    INSERT INTO dbo.CaHoc (MaLopHoc, MaLichHoc, MaPhongHoc, MaGiaoVienThucTe, MaTroGiangThucTe,
                                           NgayHoc, GioBatDau, GioKetThuc, NoiDungBaiHoc, SoBuoi, LoaiBuoi, TrangThai)
                    VALUES (@maLopHoc, @maLichHoc, @maPhongHoc, @giaoVien, @maTroGiang,
                            @ngay, @gioBD, @gioKT,
                            ISNULL(@tieuDe, N'Buổi ' + CAST(@n AS nvarchar(10))),
                            @n, ISNULL(@loaiBuoi, N'Buổi học'), N'Chưa diễn ra');

                    SET @n = @n + 1;
                END
            END

            SET @ngay = DATEADD(day, 1, @ngay);

            -- chốt an toàn: không quét quá 300 ngày
            IF DATEDIFF(day, @ngayBatDau, @ngay) > 300
            BEGIN
                RAISERROR(N'Sinh ca học vượt quá 300 ngày cho lớp %d.', 16, 1, @maLopHoc);
            END
        END

        -- Cập nhật ngày kết thúc thực tế theo buổi cuối
        UPDATE lh
        SET NgayKetThucDuKien = (SELECT MAX(c.NgayHoc) FROM dbo.CaHoc c WHERE c.MaLopHoc = @maLopHoc)
        FROM dbo.LopHoc lh
        WHERE lh.MaLopHoc = @maLopHoc;

        FETCH NEXT FROM curLop INTO @maLopHoc, @maKhoaHoc, @ngayBatDau, @thu1, @thu2,
                                    @gioBD, @gioKT, @maPhongHoc, @maGvViet, @maGvNuocNgoai, @maTroGiang;
    END

    CLOSE curLop;
    DEALLOCATE curLop;

    DECLARE @SoCaHeThong int;
    SELECT @SoCaHeThong = COUNT(*) FROM dbo.CaHoc;

    PRINT N'  • Đã tạo ca học: ' + CAST(@SoCaHeThong AS nvarchar(10)) + N' ca (toàn hệ thống).';

    /* =============================================================================
       10. ĐĂNG KÝ HỌC — mỗi lớp 1 học viên (KHÔNG tạo HocPhi)
       ============================================================================= */
    INSERT INTO dbo.DangKyHoc (MaHocVien, MaKhoaHoc, MaLopHoc, MaNhanVienTuVan, MaUuDai,
                               NgayDangKy, LichHocMongMuon, HocPhiGoc, SoTienGiam,
                               NgayXepLop, TrangThaiDangKy, GhiChu)
    SELECT hv.Ma, l.MaKhoaHoc, lop.MaLopHoc, @MaNhanVienTuVan, NULL,
           DATEADD(day, -14, lop.NgayBatDau), NULL,
           kh.HocPhiCoBan, 0,
           lop.NgayBatDau, N'Đã xếp lớp', N'Xếp lớp tự động (script v24)'
    FROM #Lop l
    INNER JOIN dbo.LopHoc lop ON lop.MaLop = l.MaLop
    INNER JOIN dbo.KhoaHoc kh ON kh.MaKhoaHoc = l.MaKhoaHoc
    INNER JOIN #HV hv ON hv.Ma = N'HV' + RIGHT(N'000' + CAST(@HvTiep + l.Stt - 1 AS nvarchar(10)), 3);

    -- Ngày nhập học = ngày khai giảng của lớp
    UPDATE hv
    SET NgayNhapHoc = lop.NgayBatDau
    FROM dbo.HocVien hv
    INNER JOIN dbo.DangKyHoc dk ON dk.MaHocVien = hv.MaHocVien
    INNER JOIN dbo.LopHoc lop ON lop.MaLopHoc = dk.MaLopHoc
    WHERE dk.GhiChu = N'Xếp lớp tự động (script v24)';

    PRINT N'  • Đã tạo 13 đăng ký học (chưa tạo học phí).';

    /* =============================================================================
       11. KIỂM TRA KẾT QUẢ
       ============================================================================= */
    PRINT N'--- KIỂM TRA ---';

    SELECT N'Lớp học' AS HangMuc, COUNT(*) AS SoLuong FROM dbo.LopHoc WHERE MaLop IN (SELECT MaLop FROM #Lop)
    UNION ALL SELECT N'Lịch học', COUNT(*) FROM dbo.LichHoc WHERE MaLopHoc IN (SELECT MaLopHoc FROM dbo.LopHoc WHERE MaLop IN (SELECT MaLop FROM #Lop))
    UNION ALL SELECT N'Ca học', COUNT(*) FROM dbo.CaHoc WHERE MaLopHoc IN (SELECT MaLopHoc FROM dbo.LopHoc WHERE MaLop IN (SELECT MaLop FROM #Lop))
    UNION ALL SELECT N'Phân công', COUNT(*) FROM dbo.PhanCongGiangDay WHERE MaLopHoc IN (SELECT MaLopHoc FROM dbo.LopHoc WHERE MaLop IN (SELECT MaLop FROM #Lop))
    UNION ALL SELECT N'Đăng ký học', COUNT(*) FROM dbo.DangKyHoc WHERE GhiChu = N'Xếp lớp tự động (script v24)'
    UNION ALL SELECT N'GV/Trợ giảng mới', COUNT(*) FROM dbo.GiaoVien WHERE MaGiaoVien IN (SELECT Ma FROM #GV)
    UNION ALL SELECT N'Học viên mới', COUNT(*) FROM dbo.HocVien WHERE MaHocVien IN (SELECT Ma FROM #HV);

    -- Kiểm tra trùng lịch: cùng (ngày, giờ bắt đầu, phòng) mà khác lớp
    SELECT N'CẢNH BÁO: ca học trùng phòng & giờ' AS CanhBao,
           c.NgayHoc, c.GioBatDau, c.MaPhongHoc, COUNT(*) AS SoCa
    FROM dbo.CaHoc c
    WHERE c.MaLopHoc IN (SELECT MaLopHoc FROM dbo.LopHoc WHERE MaLop IN (SELECT MaLop FROM #Lop))
    GROUP BY c.NgayHoc, c.GioBatDau, c.MaPhongHoc
    HAVING COUNT(*) > 1;

    -- Kiểm tra giáo viên bị trùng giờ dạy (chỉ trong 13 lớp mới)
    SELECT N'CẢNH BÁO: giáo viên trùng giờ' AS CanhBao, c.MaGiaoVienThucTe, c.NgayHoc, c.GioBatDau, COUNT(*) AS SoCa
    FROM dbo.CaHoc c
    WHERE c.MaGiaoVienThucTe IS NOT NULL
      AND c.MaLopHoc IN (SELECT MaLopHoc FROM dbo.LopHoc WHERE MaLop IN (SELECT MaLop FROM #Lop))
    GROUP BY c.MaGiaoVienThucTe, c.NgayHoc, c.GioBatDau
    HAVING COUNT(*) > 1;

    -- Tổng trọng số đầu điểm từng khoá
    SELECT ld.MaKhoaHoc, SUM(ld.TrongSo) AS TongTrongSo,
           CASE WHEN SUM(ld.TrongSo) = 100 THEN N'OK' ELSE N'KIỂM TRA LẠI' END AS KetQua
    FROM dbo.LoaiDiem ld
    WHERE ld.TrangThai = N'Hoạt động' AND ld.MaKhoaHoc BETWEEN 1 AND 13
    GROUP BY ld.MaKhoaHoc
    ORDER BY ld.MaKhoaHoc;

    COMMIT TRANSACTION;

    PRINT N'=== HOÀN TẤT: 13 KHÓA ĐÃ CÓ LỚP, GIÁO VIÊN, HỌC VIÊN, CA HỌC ===';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT N'LỖI: ' + ERROR_MESSAGE() + N' (dòng ' + CAST(ERROR_LINE() AS nvarchar(10)) + N')';
    THROW;
END CATCH;
