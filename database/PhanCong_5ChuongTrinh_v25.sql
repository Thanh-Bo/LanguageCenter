/* =====================================================================================
   PhanCong_5ChuongTrinh_v25.sql
   -------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Chuyển dữ liệu v24 (39 GV/TG rải đều 13 lớp) sang MÔ HÌNH 5 CHƯƠNG TRÌNH:
         1 chương trình = 1 GV Việt + 1 GV nước ngoài + 1..2 Trợ giảng
         Mỗi người dạy 2–4 buổi/tuần, KHÔNG trùng giờ.

   ĐÃ CHỐT VỚI NGƯỜI DÙNG
       • 5 GV Việt + 5 GV nước ngoài + 9 Trợ giảng = 19 người (xoá 20 hồ sơ dư)
       • Mỗi lớp 5 học viên → 14 lớp = 70 học viên, mỗi học viên CHỈ học 1 lớp
       • IELTS-A-01 (khoá 14) chuyển sang bộ mới: Hưng + Michael + TG Ánh
       • Thiếu nhi (Mầm non / Tiểu học / Teen) KHÔNG học buổi tối
       • Dồn toàn bộ 13 lớp mới về tuần khai giảng 09/11/2026
       • Xoá & sinh lại toàn bộ CaHoc của 13 lớp mới (chưa có điểm danh/điểm số)

   BẢNG PHÂN CÔNG
       Chương trình   Lớp (khoá)                         Thứ      Giờ            Phòng  GV Việt   GV NN     Trợ giảng
       IELTS          IELTS-F-01 (k1)  IELTS-I-01 (k2)    3 & 5    17:30/19:30    P101   Hưng      Michael   Trang (F) · Ánh (I + A)
                      IELTS-A-01 (k14) – ĐANG CHẠY         2 & 4    18:00–20:00    giữ    Hưng      Michael   Ánh
       Giao tiếp      COMM-B-01 (k3)  COMM-I-01 (k12)    2 & 4    17:30/19:30    P103   Phúc      David     Duyên (B) · Tú (I + N)
                      COMM-N-01 (k13)                     3 & 5    17:30–19:30    P104
       Teen           TEEN-1-01 (k4)                     3 & 5    14:00–16:00    P102   Hằng      Emily     Nhung (1) · Yến (2 + 3)
                      TEEN-2-01 (k10)                     3 & 5    08:00–10:00    P102
                      TEEN-3-01 (k11)                     2 & 4    14:00–16:00    P102
       Mầm non        PRE-1-01 (k5)                       4 & 6    08:00–10:00    P105   Tuấn      Daniel    Huyền (cả 2 lớp)
                      PRE-2-01 (k6)                       4 & 6    14:00–16:00    P105
       Tiểu học       PRI-S-01 (k7)                       3 & 5    08:00–10:00    P106   Bảo       Chris     Thư (S) · Lan Anh (M + F)
                      PRI-M-01 (k8)                       3 & 5    14:00–16:00    P106
                      PRI-F-01 (k9)                       6 & 7    08:00–10:00    P106

   QUY ƯỚC
       • Thứ dùng số ISO: 1 = Thứ Hai … 7 = Chủ Nhật.
       • ThuBuoi1 = ngày của BUỔI LẺ  → Giáo viên nước ngoài
         ThuBuoi2 = ngày của BUỔI CHẴN → Giáo viên Việt (Giáo viên chính)
       • Mã GV/TG KHÔNG hard-code: script tra theo HoTen nên chạy được trên mọi bản sao DB.

   AN TOÀN
       • 1 transaction + XACT_ABORT, lỗi giữa đường → ROLLBACK.
       • Chặn chạy lại bằng dấu v24 trong PhanCongGiangDay.
       • Không sinh lại ca học nếu đã có bản ghi điểm danh.

   LIÊN QUAN
       Chạy SAU: TaoDuLieu_13Khoa_v24.sql
   ===================================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

/* =====================================================================================
   CỔNG CHẶN — dùng IF/ELSE thay cho RETURN vì RETURN chỉ thoát khỏi BATCH hiện tại,
   các batch sau (sau GO) vẫn chạy tiếp.
   ===================================================================================== */
DECLARE @LyDoChan nvarchar(400) = NULL;

IF NOT EXISTS (SELECT 1 FROM dbo.LopHoc WHERE MaLop = N'IELTS-F-01')
    SET @LyDoChan = N'Chưa có dữ liệu v24 (thiếu lớp IELTS-F-01). Hãy chạy TaoDuLieu_13Khoa_v24.sql trước.';
ELSE IF NOT EXISTS (SELECT 1 FROM dbo.PhanCongGiangDay WHERE GhiChu = N'Phân công tự động (script v24)')
    SET @LyDoChan = N'Script v25 đã chạy trước đó (không còn dấu phân công v24). Không làm gì thêm.';

IF @LyDoChan IS NOT NULL
BEGIN
    PRINT N'>>> ' + @LyDoChan;
END
ELSE
BEGIN

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @HomNay date = CAST(GETDATE() AS date);
    DECLARE @NgayKhaiGiang date = '2026-11-09';

    PRINT N'=== v25 — CHUYỂN SANG MÔ HÌNH 5 CHƯƠNG TRÌNH ===';

    /* =============================================================================
       1. TRA MÃ LỚP
       ============================================================================= */
    DECLARE @LopIeltsF int, @LopIeltsI int, @LopIeltsA int,
            @LopCommB  int, @LopCommI  int, @LopCommN  int,
            @LopTeen1  int, @LopTeen2  int, @LopTeen3  int,
            @LopPre1   int, @LopPre2   int,
            @LopPriS   int, @LopPriM   int, @LopPriF   int;

    SELECT @LopIeltsF = MaLopHoc FROM dbo.LopHoc WHERE MaLop = N'IELTS-F-01';
    SELECT @LopIeltsI = MaLopHoc FROM dbo.LopHoc WHERE MaLop = N'IELTS-I-01';
    SELECT @LopIeltsA = MaLopHoc FROM dbo.LopHoc WHERE MaLop = N'IELTS-A-01';
    SELECT @LopCommB  = MaLopHoc FROM dbo.LopHoc WHERE MaLop = N'COMM-B-01';
    SELECT @LopCommI  = MaLopHoc FROM dbo.LopHoc WHERE MaLop = N'COMM-I-01';
    SELECT @LopCommN  = MaLopHoc FROM dbo.LopHoc WHERE MaLop = N'COMM-N-01';
    SELECT @LopTeen1  = MaLopHoc FROM dbo.LopHoc WHERE MaLop = N'TEEN-1-01';
    SELECT @LopTeen2  = MaLopHoc FROM dbo.LopHoc WHERE MaLop = N'TEEN-2-01';
    SELECT @LopTeen3  = MaLopHoc FROM dbo.LopHoc WHERE MaLop = N'TEEN-3-01';
    SELECT @LopPre1   = MaLopHoc FROM dbo.LopHoc WHERE MaLop = N'PRE-1-01';
    SELECT @LopPre2   = MaLopHoc FROM dbo.LopHoc WHERE MaLop = N'PRE-2-01';
    SELECT @LopPriS   = MaLopHoc FROM dbo.LopHoc WHERE MaLop = N'PRI-S-01';
    SELECT @LopPriM   = MaLopHoc FROM dbo.LopHoc WHERE MaLop = N'PRI-M-01';
    SELECT @LopPriF   = MaLopHoc FROM dbo.LopHoc WHERE MaLop = N'PRI-F-01';

    IF @LopIeltsF IS NULL OR @LopIeltsI IS NULL OR @LopIeltsA IS NULL
       OR @LopCommB IS NULL OR @LopCommI IS NULL OR @LopCommN IS NULL
       OR @LopTeen1 IS NULL OR @LopTeen2 IS NULL OR @LopTeen3 IS NULL
       OR @LopPre1 IS NULL OR @LopPre2 IS NULL
       OR @LopPriS IS NULL OR @LopPriM IS NULL OR @LopPriF IS NULL
    BEGIN
        RAISERROR(N'Thiếu lớp bắt buộc (14 lớp: 13 lớp v24 + IELTS-A-01). Kiểm tra lại dữ liệu.', 16, 1);
    END

    PRINT N'  • Đã tìm thấy đủ 14 lớp.';

    /* =============================================================================
       2. TRA MÃ GIÁO VIÊN / TRỢ GIẢNG THEO HỌ TÊN
       ============================================================================= */
    DECLARE @GvHung varchar(20), @GvPhuc varchar(20), @GvHang varchar(20),
            @GvTuan varchar(20), @GvBao  varchar(20);

    DECLARE @GvMichael varchar(20), @GvDavid varchar(20), @GvEmily varchar(20),
            @GvDaniel  varchar(20), @GvChris varchar(20);

    DECLARE @TgTrang varchar(20), @TgAnh  varchar(20), @TgDuyen varchar(20),
            @TgTu    varchar(20), @TgNhung varchar(20), @TgYen  varchar(20),
            @TgHuyen varchar(20), @TgThu  varchar(20), @TgLanAnh varchar(20);

    SELECT @GvHung = MaGiaoVien FROM dbo.GiaoVien WHERE HoTen = N'Nguyễn Văn Hưng';
    SELECT @GvPhuc = MaGiaoVien FROM dbo.GiaoVien WHERE HoTen = N'Lê Hoàng Phúc';
    SELECT @GvHang = MaGiaoVien FROM dbo.GiaoVien WHERE HoTen = N'Phạm Thu Hằng';
    SELECT @GvTuan = MaGiaoVien FROM dbo.GiaoVien WHERE HoTen = N'Hoàng Anh Tuấn';
    SELECT @GvBao  = MaGiaoVien FROM dbo.GiaoVien WHERE HoTen = N'Đặng Quốc Bảo';

    SELECT @GvMichael = MaGiaoVien FROM dbo.GiaoVien WHERE HoTen = N'Michael Andrew Smith';
    SELECT @GvDavid   = MaGiaoVien FROM dbo.GiaoVien WHERE HoTen = N'David James Wilson';
    SELECT @GvEmily   = MaGiaoVien FROM dbo.GiaoVien WHERE HoTen = N'Emily Rose Thompson';
    SELECT @GvDaniel  = MaGiaoVien FROM dbo.GiaoVien WHERE HoTen = N'Daniel Robert Brown';
    SELECT @GvChris   = MaGiaoVien FROM dbo.GiaoVien WHERE HoTen = N'Christopher Lee Davis';

    SELECT @TgTrang  = MaGiaoVien FROM dbo.GiaoVien WHERE HoTen = N'Nguyễn Thị Thu Trang';
    SELECT @TgAnh    = MaGiaoVien FROM dbo.GiaoVien WHERE HoTen = N'Trần Ngọc Ánh';
    SELECT @TgDuyen  = MaGiaoVien FROM dbo.GiaoVien WHERE HoTen = N'Lê Thị Mỹ Duyên';
    SELECT @TgTu     = MaGiaoVien FROM dbo.GiaoVien WHERE HoTen = N'Lý Thị Cẩm Tú';
    SELECT @TgNhung  = MaGiaoVien FROM dbo.GiaoVien WHERE HoTen = N'Phạm Hồng Nhung';
    SELECT @TgYen    = MaGiaoVien FROM dbo.GiaoVien WHERE HoTen = N'Đỗ Thị Hải Yến';
    SELECT @TgHuyen  = MaGiaoVien FROM dbo.GiaoVien WHERE HoTen = N'Võ Thị Thanh Huyền';
    SELECT @TgThu    = MaGiaoVien FROM dbo.GiaoVien WHERE HoTen = N'Bùi Minh Thư';
    SELECT @TgLanAnh = MaGiaoVien FROM dbo.GiaoVien WHERE HoTen = N'Hoàng Thị Lan Anh';

    IF @GvHung IS NULL OR @GvPhuc IS NULL OR @GvHang IS NULL OR @GvTuan IS NULL OR @GvBao IS NULL
       OR @GvMichael IS NULL OR @GvDavid IS NULL OR @GvEmily IS NULL OR @GvDaniel IS NULL OR @GvChris IS NULL
       OR @TgTrang IS NULL OR @TgAnh IS NULL OR @TgDuyen IS NULL OR @TgTu IS NULL OR @TgNhung IS NULL
       OR @TgYen IS NULL OR @TgHuyen IS NULL OR @TgThu IS NULL OR @TgLanAnh IS NULL
    BEGIN
        RAISERROR(N'Không tra đủ 19 hồ sơ GV/TG cần giữ. Kiểm tra lại họ tên trong bảng GiaoVien.', 16, 1);
    END

    PRINT N'  • Đã tra đủ 19 hồ sơ giáo viên / trợ giảng cần giữ.';

    /* =============================================================================
       3. DANH SÁCH GIỮ LẠI + XOÁ 20 HỒ SƠ DƯ
       ============================================================================= */
    DECLARE @Giu TABLE (HoTen nvarchar(100) PRIMARY KEY);

    INSERT INTO @Giu (HoTen) VALUES
        (N'Nguyễn Văn Hưng'),   (N'Lê Hoàng Phúc'),  (N'Phạm Thu Hằng'),
        (N'Hoàng Anh Tuấn'),    (N'Đặng Quốc Bảo'),
        (N'Michael Andrew Smith'), (N'David James Wilson'), (N'Emily Rose Thompson'),
        (N'Daniel Robert Brown'),  (N'Christopher Lee Davis'),
        (N'Nguyễn Thị Thu Trang'), (N'Trần Ngọc Ánh'),  (N'Lê Thị Mỹ Duyên'),
        (N'Lý Thị Cẩm Tú'),       (N'Phạm Hồng Nhung'),(N'Đỗ Thị Hải Yến'),
        (N'Võ Thị Thanh Huyền'),  (N'Bùi Minh Thư'),   (N'Hoàng Thị Lan Anh');

    -- Bộ giáo viên CŨ của khoá 14: KHÔNG xoá (giữ lịch sử điểm số), chỉ ngừng sử dụng.
    DECLARE @GvNeoCu TABLE (Ma varchar(20) PRIMARY KEY);

    INSERT INTO @GvNeoCu (Ma) VALUES
        (N'GV001'), (N'GV002'), (N'GV009'), (N'GV011'),
        (N'GV012'), (N'GV013'), (N'TG001'), (N'TG003');

    DECLARE @Xoa TABLE (Ma varchar(20) PRIMARY KEY);

    INSERT INTO @Xoa (Ma)
    SELECT g.MaGiaoVien
    FROM dbo.GiaoVien g
    WHERE g.HoTen NOT IN (SELECT HoTen FROM @Giu)
      AND g.MaGiaoVien NOT IN (SELECT Ma FROM @GvNeoCu);

    DECLARE @SoXoa int;
    SELECT @SoXoa = COUNT(*) FROM @Xoa;

    -- gỡ tham chiếu trước khi xoá
    UPDATE dbo.CaHoc SET MaGiaoVienThucTe = NULL WHERE MaGiaoVienThucTe IN (SELECT Ma FROM @Xoa);
    UPDATE dbo.CaHoc SET MaTroGiangThucTe = NULL WHERE MaTroGiangThucTe IN (SELECT Ma FROM @Xoa);

    DELETE ds FROM dbo.DiemSo ds WHERE ds.MaGiaoVien IN (SELECT Ma FROM @Xoa);
    DELETE pc FROM dbo.PhanCongGiangDay pc WHERE pc.MaGiaoVien IN (SELECT Ma FROM @Xoa);
    DELETE bc FROM dbo.BangCap bc WHERE bc.MaGiaoVien IN (SELECT Ma FROM @Xoa);
    DELETE cc FROM dbo.ChungChi cc WHERE cc.MaGiaoVien IN (SELECT Ma FROM @Xoa);
    DELETE gv FROM dbo.GiaoVien gv WHERE gv.MaGiaoVien IN (SELECT Ma FROM @Xoa);

    DELETE dd
    FROM dbo.DiemDanh dd
    WHERE dd.MaTaiKhoanThucHien IN (SELECT t.MaTaiKhoan FROM dbo.TaiKhoan t WHERE t.TenDangNhap IN (SELECT Ma FROM @Xoa));

    DELETE tb
    FROM dbo.ThongBao tb
    WHERE tb.MaTaiKhoanGui  IN (SELECT t.MaTaiKhoan FROM dbo.TaiKhoan t WHERE t.TenDangNhap IN (SELECT Ma FROM @Xoa))
       OR tb.MaTaiKhoanNhan IN (SELECT t.MaTaiKhoan FROM dbo.TaiKhoan t WHERE t.TenDangNhap IN (SELECT Ma FROM @Xoa));

    DELETE t FROM dbo.TaiKhoan t WHERE t.TenDangNhap IN (SELECT Ma FROM @Xoa);

    PRINT N'  • Đã xoá ' + CAST(@SoXoa AS nvarchar(10)) + N' hồ sơ GV/TG dư (kèm tài khoản và dữ liệu liên quan).';

    /* =============================================================================
       4. NGỪNG SỬ DỤNG BỘ GIÁO VIÊN CŨ CỦA KHOÁ 14
       ============================================================================= */
    DELETE pc FROM dbo.PhanCongGiangDay pc WHERE pc.MaGiaoVien IN (SELECT Ma FROM @GvNeoCu);

    UPDATE gv
    SET TrangThai = N'Đã nghỉ việc'
    FROM dbo.GiaoVien gv
    WHERE gv.MaGiaoVien IN (SELECT Ma FROM @GvNeoCu);

    UPDATE t
    SET TrangThaiTaiKhoan = N'Ngừng sử dụng'
    FROM dbo.TaiKhoan t
    WHERE t.TenDangNhap IN (SELECT Ma FROM @GvNeoCu);

    PRINT N'  • Đã ngừng sử dụng 6 GV + 2 TG cũ (giữ điểm số lịch sử, khoá đăng nhập).';

    /* =============================================================================
       5. IELTS-A-01 (KHOÁ 14) — CHUYỂN SANG BỘ MỚI
       ============================================================================= */
    UPDATE ch
    SET MaGiaoVienThucTe = CASE WHEN ch.SoBuoi % 2 = 1 THEN @GvMichael ELSE @GvHung END,
        MaTroGiangThucTe = @TgAnh
    FROM dbo.CaHoc ch
    WHERE ch.MaLopHoc = @LopIeltsA;

    PRINT N'  • IELTS-A-01: đã gán Hưng (buổi chẵn) + Michael (buổi lẻ) + TG Ánh.';

    /* =============================================================================
       6. KIỂM TRA AN TOÀN TRƯỚC KHI SINH LẠI CA HỌC
       ============================================================================= */
    IF EXISTS (
        SELECT 1
        FROM dbo.DiemDanh dd
        JOIN dbo.CaHoc ch ON ch.MaCaHoc = dd.MaCaHoc
        WHERE ch.MaLopHoc IN (@LopIeltsF, @LopIeltsI, @LopCommB, @LopCommI, @LopCommN,
                              @LopTeen1, @LopTeen2, @LopTeen3, @LopPre1, @LopPre2,
                              @LopPriS, @LopPriM, @LopPriF))
    BEGIN
        RAISERROR(N'Đã có dữ liệu điểm danh ở 13 lớp mới — không thể xoá & sinh lại ca học.', 16, 1);
    END

    -- DiemSo cũng tham chiếu CaHoc (FK_DiemSo_CaHoc) → phải kiểm tra trước khi xoá ca
    IF EXISTS (
        SELECT 1
        FROM dbo.DiemSo ds
        JOIN dbo.CaHoc ch ON ch.MaCaHoc = ds.MaCaHoc
        WHERE ch.MaLopHoc IN (@LopIeltsF, @LopIeltsI, @LopCommB, @LopCommI, @LopCommN,
                              @LopTeen1, @LopTeen2, @LopTeen3, @LopPre1, @LopPre2,
                              @LopPriS, @LopPriM, @LopPriF))
    BEGIN
        RAISERROR(N'Đã có điểm số gắn với ca học của 13 lớp mới — không thể xoá & sinh lại ca học.', 16, 1);
    END

    /* =============================================================================
       7. LỊCH MỚI (không trùng giờ trong cùng chương trình, thiếu nhi không học tối)
       ============================================================================= */
    DECLARE @Lop TABLE
    (
        ThuTu        int         NOT NULL PRIMARY KEY,
        MaLopHoc     int         NOT NULL,
        MaKhoaHoc    int         NOT NULL,
        Thu1         tinyint     NOT NULL,
        Thu2         tinyint     NOT NULL,
        GioBatDau    time        NOT NULL,
        GioKetThuc   time        NOT NULL,
        MaPhong      varchar(20) NOT NULL,
        MaGvViet     varchar(20) NOT NULL,
        MaGvNuocNgoai varchar(20) NOT NULL,
        MaTroGiang   varchar(20) NOT NULL
    );

    INSERT INTO @Lop (ThuTu, MaLopHoc, MaKhoaHoc, Thu1, Thu2, GioBatDau, GioKetThuc, MaPhong,
                      MaGvViet, MaGvNuocNgoai, MaTroGiang)
    SELECT x.ThuTu, x.MaLopHoc, lh.MaKhoaHoc, x.Thu1, x.Thu2, x.GioBatDau, x.GioKetThuc,
           x.MaPhong, x.MaGvViet, x.MaGvNuocNgoai, x.MaTroGiang
    FROM (VALUES
        --  IELTS: Thứ 3 + Thứ 5 (tránh Thứ 2/Thứ 4 của IELTS-A-01)
        (1,  @LopIeltsF, 3, 5, CAST('17:30' AS time), CAST('19:30' AS time), N'P101', @GvHung, @GvMichael, @TgTrang),
        (2,  @LopIeltsI, 3, 5, CAST('19:30' AS time), CAST('21:30' AS time), N'P101', @GvHung, @GvMichael, @TgAnh),
        --  Giao tiếp (người lớn): cho phép buổi tối
        (3,  @LopCommB,  2, 4, CAST('17:30' AS time), CAST('19:30' AS time), N'P103', @GvPhuc, @GvDavid, @TgDuyen),
        (4,  @LopCommI,  2, 4, CAST('19:30' AS time), CAST('21:30' AS time), N'P103', @GvPhuc, @GvDavid, @TgTu),
        (5,  @LopCommN,  3, 5, CAST('17:30' AS time), CAST('19:30' AS time), N'P104', @GvPhuc, @GvDavid, @TgTu),
        --  Teen (thiếu nhi): chỉ 08:00 và 14:00
        (6,  @LopTeen1,  3, 5, CAST('14:00' AS time), CAST('16:00' AS time), N'P102', @GvHang, @GvEmily, @TgNhung),
        (7,  @LopTeen2,  3, 5, CAST('08:00' AS time), CAST('10:00' AS time), N'P102', @GvHang, @GvEmily, @TgYen),
        (8,  @LopTeen3,  2, 4, CAST('14:00' AS time), CAST('16:00' AS time), N'P102', @GvHang, @GvEmily, @TgYen),
        --  Mầm non: chỉ 08:00 và 14:00
        (9,  @LopPre1,   4, 6, CAST('08:00' AS time), CAST('10:00' AS time), N'P105', @GvTuan, @GvDaniel, @TgHuyen),
        (10, @LopPre2,   4, 6, CAST('14:00' AS time), CAST('16:00' AS time), N'P105', @GvTuan, @GvDaniel, @TgHuyen),
        --  Tiểu học: chỉ 08:00 và 14:00
        (11, @LopPriS,   3, 5, CAST('08:00' AS time), CAST('10:00' AS time), N'P106', @GvBao, @GvChris, @TgThu),
        (12, @LopPriM,   3, 5, CAST('14:00' AS time), CAST('16:00' AS time), N'P106', @GvBao, @GvChris, @TgLanAnh),
        (13, @LopPriF,   6, 7, CAST('08:00' AS time), CAST('10:00' AS time), N'P106', @GvBao, @GvChris, @TgLanAnh)
    ) AS x (ThuTu, MaLopHoc, Thu1, Thu2, GioBatDau, GioKetThuc, MaPhong, MaGvViet, MaGvNuocNgoai, MaTroGiang)
    INNER JOIN dbo.LopHoc lh ON lh.MaLopHoc = x.MaLopHoc;

    DECLARE @SoLop int;
    SELECT @SoLop = COUNT(*) FROM @Lop;

    IF @SoLop <> 13
    BEGIN
        RAISERROR(N'Bảng lịch mới phải có đúng 13 lớp.', 16, 1);
    END

    PRINT N'  • Đã dựng bảng lịch 13 lớp (không trùng giờ, thiếu nhi không học tối).';

    -- 7.1 Xoá ca học + lịch học cũ của 13 lớp
    DELETE ch FROM dbo.CaHoc ch WHERE ch.MaLopHoc IN (SELECT MaLopHoc FROM @Lop);
    DELETE lh FROM dbo.LichHoc lh WHERE lh.MaLopHoc IN (SELECT MaLopHoc FROM @Lop);

    -- 7.2 Cập nhật thông tin lớp: cùng khai giảng 09/11
    UPDATE lp
    SET NgayBatDau         = @NgayKhaiGiang,
        NgayKetThucDuKien  = DATEADD(day, 69, @NgayKhaiGiang),
        TrangThai          = CASE WHEN @NgayKhaiGiang <= @HomNay THEN N'Đang học' ELSE N'Chuẩn bị mở' END,
        ThuBuoi1           = l.Thu1,
        ThuBuoi2           = l.Thu2,
        GioBatDau          = l.GioBatDau,
        GioKetThuc         = l.GioKetThuc
    FROM dbo.LopHoc lp
    INNER JOIN @Lop l ON l.MaLopHoc = lp.MaLopHoc;

    -- 7.3 Lịch học lặp: 2 buổi/tuần
    INSERT INTO dbo.LichHoc (MaLopHoc, MaPhongHoc, ThuTrongTuan, GioBatDau, GioKetThuc,
                             NgayApDungTu, TrangThai)
    SELECT l.MaLopHoc, ph.MaPhongHoc, x.Thu, l.GioBatDau, l.GioKetThuc,
           @NgayKhaiGiang, N'Hoạt động'
    FROM @Lop l
    INNER JOIN dbo.PhongHoc ph ON ph.MaPhong = l.MaPhong
    CROSS APPLY (VALUES (l.Thu1), (l.Thu2)) AS x (Thu);

    PRINT N'  • Đã tạo lại 26 lịch học lặp.';

    /* =============================================================================
       8. SINH LẠI 20 CA HỌC / LỚP
       ============================================================================= */
    DECLARE @maLopHoc int, @maKhoaHoc int, @thu1 tinyint, @thu2 tinyint,
            @gioBD time, @gioKT time, @maPhongHoc int,
            @maGvViet varchar(20), @maGvNuocNgoai varchar(20), @maTroGiang varchar(20);

    DECLARE @n int, @ngay date, @thuHomNay tinyint, @thuCanTim tinyint,
            @tieuDe nvarchar(200), @loaiBuoi nvarchar(50),
            @maLichHoc int, @giaoVien varchar(20);

    DECLARE curLop CURSOR LOCAL FAST_FORWARD FOR
        SELECT l.MaLopHoc, l.MaKhoaHoc, l.Thu1, l.Thu2, l.GioBatDau, l.GioKetThuc,
               ph.MaPhongHoc, l.MaGvViet, l.MaGvNuocNgoai, l.MaTroGiang
        FROM @Lop l
        INNER JOIN dbo.PhongHoc ph ON ph.MaPhong = l.MaPhong
        ORDER BY l.ThuTu;

    OPEN curLop;
    FETCH NEXT FROM curLop INTO @maLopHoc, @maKhoaHoc, @thu1, @thu2, @gioBD, @gioKT,
                                @maPhongHoc, @maGvViet, @maGvNuocNgoai, @maTroGiang;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @n = 1;
        SET @ngay = @NgayKhaiGiang;

        WHILE @n <= 20
        BEGIN
            -- thứ ISO (1 = Thứ Hai) của @ngay — không phụ thuộc @@DATEFIRST
            SET @thuHomNay = CAST(((DATEDIFF(day, CAST('19000101' AS date), @ngay) % 7) + 7) % 7 + 1 AS tinyint);
            SET @thuCanTim = CASE WHEN @n % 2 = 1 THEN @thu1 ELSE @thu2 END;

            IF @thuHomNay = @thuCanTim
               AND NOT EXISTS (SELECT 1 FROM dbo.NgayLe nl WHERE nl.Ngay = @ngay)
            BEGIN
                SET @tieuDe = NULL;
                SET @loaiBuoi = NULL;
                SET @maLichHoc = NULL;
                SET @giaoVien = CASE WHEN @n % 2 = 1 THEN @maGvNuocNgoai ELSE @maGvViet END;

                SELECT @tieuDe = nd.TieuDe, @loaiBuoi = nd.LoaiBuoi
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

            IF DATEDIFF(day, @NgayKhaiGiang, @ngay) > 300
            BEGIN
                RAISERROR(N'Sinh ca học vượt quá 300 ngày cho lớp %d.', 16, 1, @maLopHoc);
            END
        END

        -- Ngày kết thúc dự kiến = buổi cuối
        UPDATE lp
        SET NgayKetThucDuKien = (SELECT MAX(c.NgayHoc) FROM dbo.CaHoc c WHERE c.MaLopHoc = @maLopHoc)
        FROM dbo.LopHoc lp
        WHERE lp.MaLopHoc = @maLopHoc;

        FETCH NEXT FROM curLop INTO @maLopHoc, @maKhoaHoc, @thu1, @thu2, @gioBD, @gioKT,
                                    @maPhongHoc, @maGvViet, @maGvNuocNgoai, @maTroGiang;
    END

    CLOSE curLop;
    DEALLOCATE curLop;

    DECLARE @SoCaMoi int;
    SELECT @SoCaMoi = COUNT(*) FROM dbo.CaHoc WHERE MaLopHoc IN (SELECT MaLopHoc FROM @Lop);

    PRINT N'  • Đã sinh ' + CAST(@SoCaMoi AS nvarchar(10)) + N' ca học cho 13 lớp (khai giảng 09/11/2026).';

    /* =============================================================================
       9. PHÂN CÔNG GIẢNG DẠY (3 dòng / lớp × 14 lớp = 42)
       ============================================================================= */
    DELETE pc
    FROM dbo.PhanCongGiangDay pc
    WHERE pc.MaLopHoc IN (SELECT MaLopHoc FROM @Lop)
       OR pc.MaLopHoc = @LopIeltsA;

    IF OBJECT_ID(N'tempdb..#Pc') IS NOT NULL DROP TABLE #Pc;

    CREATE TABLE #Pc
    (
        MaLopHoc  int         NOT NULL,
        MaGiaoVien varchar(20) NOT NULL,
        VaiTro    nvarchar(50) NOT NULL
    );

    INSERT INTO #Pc (MaLopHoc, MaGiaoVien, VaiTro)
    SELECT l.MaLopHoc, l.MaGvViet, N'Giáo viên chính' FROM @Lop l
    UNION ALL
    SELECT l.MaLopHoc, l.MaGvNuocNgoai, N'Giáo viên nước ngoài' FROM @Lop l
    UNION ALL
    SELECT l.MaLopHoc, l.MaTroGiang, N'Trợ giảng' FROM @Lop l
    UNION ALL
    -- IELTS-A-01 (khoá 14) dùng chung bộ giáo viên của chương trình IELTS
    SELECT @LopIeltsA, @GvHung, N'Giáo viên chính'
    UNION ALL
    SELECT @LopIeltsA, @GvMichael, N'Giáo viên nước ngoài'
    UNION ALL
    SELECT @LopIeltsA, @TgAnh, N'Trợ giảng';

    INSERT INTO dbo.PhanCongGiangDay (MaLopHoc, MaGiaoVien, VaiTroPhanCong,
                                      NgayBatDau, NgayKetThuc, TrangThai, GhiChu)
    SELECT p.MaLopHoc, p.MaGiaoVien, p.VaiTro,
           lp.NgayBatDau, NULL, N'Đang phân công',
           N'Phân công theo chương trình (script v25)'
    FROM #Pc p
    INNER JOIN dbo.LopHoc lp ON lp.MaLopHoc = p.MaLopHoc;

    DECLARE @SoPhanCong int;
    SELECT @SoPhanCong = COUNT(*) FROM dbo.PhanCongGiangDay
    WHERE GhiChu = N'Phân công theo chương trình (script v25)';

    PRINT N'  • Đã tạo ' + CAST(@SoPhanCong AS nvarchar(10)) + N' phân công giảng dạy.';

    /* =============================================================================
       10. HỌC VIÊN — 5 học viên / lớp (tổng 70), mỗi học viên 1 lớp
       ============================================================================= */
    DECLARE @MatKhauHash nvarchar(500);

    SELECT TOP (1) @MatKhauHash = MatKhauHash
    FROM dbo.TaiKhoan WHERE LanDangNhapCuoi IS NULL ORDER BY MaTaiKhoan;

    IF @MatKhauHash IS NULL
        SELECT TOP (1) @MatKhauHash = MatKhauHash FROM dbo.TaiKhoan ORDER BY MaTaiKhoan;

    DECLARE @HvTiep int;
    SELECT @HvTiep = ISNULL(MAX(TRY_CONVERT(int, SUBSTRING(MaHocVien, 3, 10))), 0) + 1
    FROM dbo.HocVien
    WHERE MaHocVien LIKE N'HV%' AND TRY_CONVERT(int, SUBSTRING(MaHocVien, 3, 10)) IS NOT NULL;

    -- 10.1 Nhu cầu bổ sung của từng lớp (mục tiêu 5 học viên / lớp)
    IF OBJECT_ID(N'tempdb..#NhuCau') IS NOT NULL DROP TABLE #NhuCau;

    CREATE TABLE #NhuCau (MaLopHoc int PRIMARY KEY, MaKhoaHoc int, CanThem int);

    INSERT INTO #NhuCau (MaLopHoc, MaKhoaHoc, CanThem)
    SELECT lp.MaLopHoc, lp.MaKhoaHoc,
           5 - (SELECT COUNT(*)
                FROM dbo.DangKyHoc dk
                WHERE dk.MaLopHoc = lp.MaLopHoc
                  AND dk.TrangThaiDangKy IN (N'Đã xếp lớp', N'Hoàn thành'))
    FROM dbo.LopHoc lp
    WHERE lp.MaLopHoc IN (SELECT MaLopHoc FROM @Lop) OR lp.MaLopHoc = @LopIeltsA;

    DECLARE @CanThemTong int;
    SELECT @CanThemTong = ISNULL(SUM(CASE WHEN CanThem > 0 THEN CanThem ELSE 0 END), 0) FROM #NhuCau;

    -- 10.2 Kho tên học viên mới (đủ dùng cho 54 chỗ trống)
    DECLARE @Ten TABLE (Stt int IDENTITY(1, 1) PRIMARY KEY, HoTen nvarchar(100), GioiTinh nvarchar(10));

    INSERT INTO @Ten (HoTen, GioiTinh) VALUES
        (N'Nguyễn Hoàng Anh',      N'Nữ'),  (N'Trần Minh Quân',        N'Nam'),
        (N'Lê Thị Hồng Nhung',     N'Nữ'),  (N'Phạm Văn Đức',          N'Nam'),
        (N'Hoàng Thị Thu Hà',      N'Nữ'),  (N'Vũ Minh Trí',           N'Nam'),
        (N'Đặng Thị Kim Oanh',     N'Nữ'),  (N'Bùi Văn Lộc',           N'Nam'),
        (N'Ngô Thị Thanh Tâm',     N'Nữ'),  (N'Đỗ Minh Hiếu',          N'Nam'),
        (N'Trịnh Thị Mỹ Linh',     N'Nữ'),  (N'Lý Văn Phú',            N'Nam'),
        (N'Phan Thị Bích Ngọc',    N'Nữ'),  (N'Nguyễn Văn Tài',        N'Nam'),
        (N'Trần Thị Diễm Quỳnh',   N'Nữ'),  (N'Lê Công Thành',         N'Nam'),
        (N'Phạm Thị Ánh Tuyết',    N'Nữ'),  (N'Hoàng Văn Hòa',         N'Nam'),
        (N'Vũ Thị Lan Phương',     N'Nữ'),  (N'Đặng Minh Nhật',        N'Nam'),
        (N'Bùi Thị Thuỳ Trang',    N'Nữ'),  (N'Ngô Văn Khánh',         N'Nam'),
        (N'Đỗ Thị Mai Anh',        N'Nữ'),  (N'Trịnh Văn Dũng',        N'Nam'),
        (N'Lý Thị Kim Chi',        N'Nữ'),  (N'Phan Văn Hùng',         N'Nam'),
        (N'Nguyễn Thị Yến Nhi',    N'Nữ'),  (N'Trần Văn Sơn',          N'Nam'),
        (N'Lê Thị Ngọc Bích',      N'Nữ'),  (N'Phạm Minh Khoa',        N'Nam'),
        (N'Hoàng Thị Diệu Linh',   N'Nữ'),  (N'Vũ Văn Toàn',           N'Nam'),
        (N'Đặng Thị Hồng',         N'Nữ'),  (N'Bùi Văn Thắng',         N'Nam'),
        (N'Ngô Thị Minh Châu',     N'Nữ'),  (N'Đỗ Văn Kiên',           N'Nam'),
        (N'Trịnh Thị Thanh Hằng',  N'Nữ'),  (N'Lý Minh Tuấn',          N'Nam'),
        (N'Phan Thị Thu Hương',    N'Nữ'),  (N'Nguyễn Văn Hậu',        N'Nam'),
        (N'Trần Thị Ngọc Mai',     N'Nữ'),  (N'Lê Văn Tùng',           N'Nam'),
        (N'Phạm Thị Hải Yến',      N'Nữ'),  (N'Hoàng Văn Nam',         N'Nam'),
        (N'Vũ Thị Kim Ngân',       N'Nữ'),  (N'Đặng Văn Lâm',          N'Nam'),
        (N'Bùi Thị Phương Linh',   N'Nữ'),  (N'Ngô Văn Tú',            N'Nam'),
        (N'Đỗ Thị Thanh Hà',       N'Nữ'),  (N'Trịnh Văn Hải',         N'Nam'),
        (N'Lý Thị Thu Hằng',       N'Nữ'),  (N'Phan Văn Cường',        N'Nam'),
        (N'Nguyễn Thị Bảo Trân',   N'Nữ'),  (N'Trần Văn Huy',          N'Nam');

    DECLARE @TongTen int;
    SELECT @TongTen = COUNT(*) FROM @Ten;

    IF @TongTen < @CanThemTong
    BEGIN
        RAISERROR(N'Kho tên chỉ có %d nhưng cần thêm %d học viên. Bổ sung tên rồi chạy lại.', 16, 1,
                  @TongTen, @CanThemTong);
    END

    -- 10.3 Sinh học viên cho từng lớp
    DECLARE @maLop int, @maKhoa int, @can int, @j int, @idx int,
            @maHv varchar(20), @hoTen nvarchar(100), @gioiTinh nvarchar(10),
            @ngaySinh date, @email nvarchar(150), @dienThoai varchar(20),
            @ngayBatDau date, @hocPhi decimal(18, 2);

    SET @idx = 1;

    DECLARE curNhuCau CURSOR LOCAL FAST_FORWARD FOR
        SELECT MaLopHoc, MaKhoaHoc, CanThem FROM #NhuCau WHERE CanThem > 0 ORDER BY MaLopHoc;

    OPEN curNhuCau;
    FETCH NEXT FROM curNhuCau INTO @maLop, @maKhoa, @can;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SELECT @ngayBatDau = NgayBatDau FROM dbo.LopHoc WHERE MaLopHoc = @maLop;
        SELECT @hocPhi = HocPhiCoBan FROM dbo.KhoaHoc WHERE MaKhoaHoc = @maKhoa;

        SET @j = 1;

        WHILE @j <= @can AND @idx <= @TongTen
        BEGIN
            SELECT @hoTen = HoTen, @gioiTinh = GioiTinh FROM @Ten WHERE Stt = @idx;

            SET @maHv = N'HV' + RIGHT(N'000' + CAST(@HvTiep + @idx - 1 AS nvarchar(10)), 3);
            SET @email = N'hv' + RIGHT(N'000' + CAST(@HvTiep + @idx - 1 AS nvarchar(10)), 3) + N'@gmail.com';
            SET @dienThoai = N'0935' + RIGHT(N'000000' + CAST(@HvTiep + @idx - 1 AS nvarchar(10)), 6);

            -- Độ tuổi theo chương trình
            SET @ngaySinh = CASE
                                WHEN @maKhoa IN (5, 6)      THEN DATEADD(year, -4, @HomNay)   -- mầm non
                                WHEN @maKhoa IN (7, 8, 9)   THEN DATEADD(year, -9, @HomNay)   -- tiểu học
                                WHEN @maKhoa IN (4, 10, 11) THEN DATEADD(year, -14, @HomNay)  -- teen
                                ELSE                             DATEADD(year, -22, @HomNay)  -- người lớn
                            END;

            INSERT INTO dbo.TaiKhoan (MaVaiTro, TenDangNhap, MatKhauHash, Email,
                                      TrangThaiTaiKhoan, NgayTao)
            VALUES (7, @maHv, @MatKhauHash, @email, N'Hoạt động', GETDATE());

            INSERT INTO dbo.HocVien (MaHocVien, MaPhuHuynh, HoTen, NgaySinh, GioiTinh,
                                     SoDienThoai, Email, DiaChi, MoiQuanHePhuHuynh,
                                     MucTieuHocTap, TrinhDoHienTai, TrangThaiHocTap,
                                     NgayTaoHoSo, NgayNhapHoc)
            VALUES (@maHv, NULL, @hoTen, @ngaySinh, @gioiTinh,
                    @dienThoai, @email, N'TP. Hồ Chí Minh', NULL,
                    N'Hoàn thành khóa học đúng tiến độ', N'Đang học', N'Đang học',
                    GETDATE(), @ngayBatDau);

            INSERT INTO dbo.DangKyHoc (MaHocVien, MaKhoaHoc, MaLopHoc, MaNhanVienTuVan, MaUuDai,
                                       NgayDangKy, LichHocMongMuon, HocPhiGoc, SoTienGiam,
                                       NgayXepLop, TrangThaiDangKy, GhiChu)
            VALUES (@maHv, @maKhoa, @maLop, N'NVTV001', NULL,
                    DATEADD(day, -14, @ngayBatDau), NULL, @hocPhi, 0,
                    @ngayBatDau, N'Đã xếp lớp', N'Xếp lớp tự động (script v25)');

            SET @idx = @idx + 1;
            SET @j = @j + 1;
        END

        FETCH NEXT FROM curNhuCau INTO @maLop, @maKhoa, @can;
    END

    CLOSE curNhuCau;
    DEALLOCATE curNhuCau;

    DECLARE @SoHvMoi int;
    SELECT @SoHvMoi = COUNT(*) FROM dbo.DangKyHoc WHERE GhiChu = N'Xếp lớp tự động (script v25)';

    PRINT N'  • Đã thêm ' + CAST(@SoHvMoi AS nvarchar(10)) + N' học viên (mục tiêu 5 học viên/lớp).';

    /* =============================================================================
       11. KIỂM TRA KẾT QUẢ
       ============================================================================= */
    PRINT N'';
    PRINT N'--- KIỂM TRA ---';

    -- 11.1 Tổng quan
    SELECT N'Giáo viên đang làm việc' AS HangMuc, COUNT(*) AS SoLuong
    FROM dbo.GiaoVien WHERE TrangThai = N'Đang làm việc'
    UNION ALL
    SELECT N'Trợ giảng đang làm việc', COUNT(*)
    FROM dbo.GiaoVien WHERE TrangThai = N'Đang làm việc' AND LoaiGiaoVien = N'Trợ giảng'
    UNION ALL
    SELECT N'Lớp học', COUNT(*) FROM dbo.LopHoc
    UNION ALL
    SELECT N'Phân công v25', COUNT(*) FROM dbo.PhanCongGiangDay
            WHERE GhiChu = N'Phân công theo chương trình (script v25)'
    UNION ALL
    SELECT N'Học viên', COUNT(*) FROM dbo.HocVien;

    -- 11.2 Sĩ số từng lớp (mục tiêu = 5)
    SELECT lp.MaLop, lp.TenLop,
           (SELECT COUNT(*) FROM dbo.DangKyHoc dk
             WHERE dk.MaLopHoc = lp.MaLopHoc
               AND dk.TrangThaiDangKy IN (N'Đã xếp lớp', N'Hoàn thành'))      AS SiSo,
           CASE WHEN (SELECT COUNT(*) FROM dbo.DangKyHoc dk
                       WHERE dk.MaLopHoc = lp.MaLopHoc
                         AND dk.TrangThaiDangKy IN (N'Đã xếp lớp', N'Hoàn thành')) = 5
                THEN N'OK' ELSE N'KIỂM TRA LẠI' END                          AS KetQua
    FROM dbo.LopHoc lp
    WHERE lp.MaLopHoc IN (SELECT MaLopHoc FROM @Lop) OR lp.MaLopHoc = @LopIeltsA
    ORDER BY lp.MaLop;

    -- 11.3 CẢNH BÁO: giáo viên / trợ giảng bị trùng giờ (phải RỖNG)
    SELECT N'CẢNH BÁO: trùng giờ giáo viên/trợ giảng' AS CanhBao,
           a.NgayHoc,
           a.GioBatDau   AS Ca1_BatDau,
           a.GioKetThuc  AS Ca1_KetThuc,
           b.GioBatDau   AS Ca2_BatDau,
           b.GioKetThuc  AS Ca2_KetThuc,
           CASE WHEN a.MaGiaoVienThucTe = b.MaGiaoVienThucTe
                     AND a.MaGiaoVienThucTe IS NOT NULL THEN a.MaGiaoVienThucTe
                ELSE a.MaTroGiangThucTe END AS MaNguoiTrung,
           la.MaLop AS Lop1, lb.MaLop AS Lop2
    FROM dbo.CaHoc a
    JOIN dbo.CaHoc b
        ON b.NgayHoc = a.NgayHoc
       AND b.MaCaHoc > a.MaCaHoc
       AND a.GioBatDau < b.GioKetThuc
       AND b.GioBatDau < a.GioKetThuc
       AND (a.MaGiaoVienThucTe = b.MaGiaoVienThucTe OR a.MaTroGiangThucTe = b.MaTroGiangThucTe)
    JOIN dbo.LopHoc la ON la.MaLopHoc = a.MaLopHoc
    JOIN dbo.LopHoc lb ON lb.MaLopHoc = b.MaLopHoc
    WHERE a.TrangThai <> N'Đã hủy'
      AND b.TrangThai <> N'Đã hủy'
      AND (a.MaGiaoVienThucTe IS NOT NULL OR a.MaTroGiangThucTe IS NOT NULL)
    ORDER BY a.NgayHoc, a.GioBatDau;

    -- 11.4 CẢNH BÁO: trùng phòng & khung giờ (phải RỖNG)
    SELECT N'CẢNH BÁO: trùng phòng & giờ' AS CanhBao,
           a.NgayHoc, a.GioBatDau, a.GioKetThuc,
           b.GioBatDau AS Ca2_BatDau, b.GioKetThuc AS Ca2_KetThuc,
           a.MaPhongHoc, la.MaLop AS Lop1, lb.MaLop AS Lop2
    FROM dbo.CaHoc a
    JOIN dbo.CaHoc b
        ON b.NgayHoc = a.NgayHoc
       AND b.MaCaHoc > a.MaCaHoc
       AND a.GioBatDau < b.GioKetThuc
       AND b.GioBatDau < a.GioKetThuc
       AND a.MaPhongHoc = b.MaPhongHoc
    JOIN dbo.LopHoc la ON la.MaLopHoc = a.MaLopHoc
    JOIN dbo.LopHoc lb ON lb.MaLopHoc = b.MaLopHoc
    WHERE a.TrangThai <> N'Đã hủy' AND b.TrangThai <> N'Đã hủy'
    ORDER BY a.NgayHoc, a.GioBatDau;

    -- 11.5 CẢNH BÁO: thiếu nhi học buổi tối (phải RỖNG)
    SELECT N'CẢNH BÁO: thiếu nhi học sau 16:00' AS CanhBao,
           lp.MaLop, c.NgayHoc, c.GioBatDau
    FROM dbo.CaHoc c
    JOIN dbo.LopHoc lp ON lp.MaLopHoc = c.MaLopHoc
    WHERE lp.MaKhoaHoc IN (4, 5, 6, 7, 8, 9, 10, 11)
      AND c.GioBatDau >= CAST('16:00' AS time);

    -- 11.6 CẢNH BÁO: học viên học nhiều hơn 1 lớp (phải RỖNG)
    SELECT N'CẢNH BÁO: học viên học >1 lớp' AS CanhBao, dk.MaHocVien, COUNT(*) AS SoLop
    FROM dbo.DangKyHoc dk
    WHERE dk.TrangThaiDangKy IN (N'Đã xếp lớp', N'Hoàn thành')
    GROUP BY dk.MaHocVien
    HAVING COUNT(DISTINCT dk.MaLopHoc) > 1;

    -- 11.7 CẢNH BÁO: ca học chưa gán giáo viên / trợ giảng (phải RỖNG)
    SELECT N'CẢNH BÁO: ca thiếu GV/TG' AS CanhBao, lp.MaLop, COUNT(*) AS SoCa
    FROM dbo.CaHoc c
    JOIN dbo.LopHoc lp ON lp.MaLopHoc = c.MaLopHoc
    WHERE c.MaGiaoVienThucTe IS NULL OR c.MaTroGiangThucTe IS NULL
    GROUP BY lp.MaLop;

    -- 11.8 Tải giảng dạy mỗi người / tuần
    SELECT pc.MaGiaoVien, gv.HoTen, gv.LoaiGiaoVien,
           COUNT(DISTINCT pc.MaLopHoc) AS SoLop,
           COUNT(DISTINCT c.MaCaHoc)   AS SoCaToanKhoa
    FROM dbo.PhanCongGiangDay pc
    JOIN dbo.GiaoVien gv ON gv.MaGiaoVien = pc.MaGiaoVien
    LEFT JOIN dbo.CaHoc c ON c.MaLopHoc = pc.MaLopHoc
                         AND (c.MaGiaoVienThucTe = pc.MaGiaoVien OR c.MaTroGiangThucTe = pc.MaGiaoVien)
    WHERE pc.TrangThai = N'Đang phân công'
      AND pc.GhiChu = N'Phân công theo chương trình (script v25)'
    GROUP BY pc.MaGiaoVien, gv.HoTen, gv.LoaiGiaoVien
    ORDER BY gv.LoaiGiaoVien, pc.MaGiaoVien;

    COMMIT TRANSACTION;

    PRINT N'=== HOÀN TẤT v25 — 5 CHƯƠNG TRÌNH, 19 GV/TG, 70 HỌC VIÊN ===';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT N'LỖI: ' + ERROR_MESSAGE() + N' (dòng ' + CAST(ERROR_LINE() AS nvarchar(10)) + N')';
    THROW;
END CATCH;

END  -- ELSE (cổng chặn: không chặn thì mới chạy phần thân)
