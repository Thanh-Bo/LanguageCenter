/* =============================================================================
   BỔ SUNG v10 — KHUNG 20 BUỔI / 10 TUẦN (2 buổi/tuần, 2 giờ/buổi)
   -----------------------------------------------------------------------------
   GỒM 4 VIỆC:
     0. XÓA toàn bộ CaHoc (14 dòng đang sai) + DiemDanh liên quan, reset identity
     1. TẠO bảng NgayLe           — danh sách ngày lễ để dời buổi
     2. TẠO bảng NoiDungBaiHoc    — 20 nội dung cho mỗi khóa (seed 14 × 20 = 280 dòng)
     3. SỬA KhoaHoc: SoBuoi = 20, TongSoGio = 40  (giữ nguyên học phí)

   CHẠY 1 LẦN. Chạy lại nhiều lần vẫn an toàn (có kiểm tra IF NOT EXISTS).
   ============================================================================= */

USE QuanLyTrungTamNgoaiNgu;
GO

SET NOCOUNT ON;
GO

/* =============================================================================
   0. XÓA CA HỌC ĐANG SAI
   ============================================================================= */

PRINT N'===== BƯỚC 0: XÓA CA HỌC =====';

SELECT COUNT(*) AS SoCaHocTruocKhiXoa FROM CaHoc;
SELECT COUNT(*) AS SoDiemDanhTruocKhiXoa FROM DiemDanh;

BEGIN TRY
    BEGIN TRANSACTION;

    -- Điểm danh là bảng con → xóa trước
    DELETE FROM DiemDanh;
    DELETE FROM CaHoc;

    COMMIT TRANSACTION;

    PRINT N'  - Đã xóa DiemDanh + CaHoc.';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;

    PRINT N'  LỖI — đã ROLLBACK: ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO

DBCC CHECKIDENT ('DiemDanh', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('CaHoc',    RESEED, 0) WITH NO_INFOMSGS;
GO

/* =============================================================================
   1. BẢNG NgayLe — NGÀY LỄ
   ============================================================================= */

PRINT N'===== BƯỚC 1: BẢNG NgayLe =====';

IF OBJECT_ID(N'dbo.NgayLe', N'U') IS NULL
BEGIN
    CREATE TABLE NgayLe
    (
        MaNgayLe   INT IDENTITY(1,1) PRIMARY KEY,
        Ngay       DATE NOT NULL,
        TenNgayLe  NVARCHAR(100) NOT NULL,
        GhiChu     NVARCHAR(200) NULL,

        CONSTRAINT UQ_NgayLe_Ngay UNIQUE (Ngay)
    );

    PRINT N'  - Đã tạo bảng NgayLe';
END
ELSE
    PRINT N'  - Bảng NgayLe đã tồn tại (bỏ qua)';
GO

/* Ngày lễ DƯƠNG LỊCH cố định (âm lịch như Tết, Giỗ Tổ thì admin tự thêm) */
INSERT INTO NgayLe (Ngay, TenNgayLe)
SELECT v.Ngay, v.Ten
  FROM (VALUES
        ('2026-01-01', N'Tết Dương lịch'),
        ('2026-04-30', N'Ngày Giải phóng miền Nam'),
        ('2026-05-01', N'Ngày Quốc tế Lao động'),
        ('2026-09-02', N'Quốc khánh'),
        ('2027-01-01', N'Tết Dương lịch'),
        ('2027-04-30', N'Ngày Giải phóng miền Nam'),
        ('2027-05-01', N'Ngày Quốc tế Lao động'),
        ('2027-09-02', N'Quốc khánh')
       ) v(Ngay, Ten)
 WHERE NOT EXISTS (SELECT 1 FROM NgayLe nl WHERE nl.Ngay = v.Ngay);
GO

/* =============================================================================
   2. BẢNG NoiDungBaiHoc — 20 NỘI DUNG MỖI KHÓA
   ============================================================================= */

PRINT N'===== BƯỚC 2: BẢNG NoiDungBaiHoc =====';

IF OBJECT_ID(N'dbo.NoiDungBaiHoc', N'U') IS NULL
BEGIN
    CREATE TABLE NoiDungBaiHoc
    (
        MaNoiDung  INT IDENTITY(1,1) PRIMARY KEY,
        MaKhoaHoc  INT NOT NULL,
        SoBuoi     INT NOT NULL,              -- 1..20
        TieuDe     NVARCHAR(200) NOT NULL,
        NoiDung    NVARCHAR(1000) NULL,
        MucTieu    NVARCHAR(500) NULL,

        CONSTRAINT FK_NoiDungBaiHoc_KhoaHoc
            FOREIGN KEY (MaKhoaHoc) REFERENCES KhoaHoc(MaKhoaHoc),

        CONSTRAINT UQ_NoiDungBaiHoc_KhoaHoc_SoBuoi UNIQUE (MaKhoaHoc, SoBuoi),

        CONSTRAINT CK_NoiDungBaiHoc_SoBuoi CHECK (SoBuoi BETWEEN 1 AND 20)
    );

    PRINT N'  - Đã tạo bảng NoiDungBaiHoc';
END
ELSE
    PRINT N'  - Bảng NoiDungBaiHoc đã tồn tại (bỏ qua)';
GO

/* Seed 20 buổi cho MỌI khóa — tiêu đề theo cấp độ, nội dung soạn sẵn, admin sửa lại sau */
INSERT INTO NoiDungBaiHoc (MaKhoaHoc, SoBuoi, TieuDe, NoiDung)
SELECT k.MaKhoaHoc,
       m.SoBuoi,
       CONCAT(N'Buổi ', m.SoBuoi, N' — ', m.TieuDe),
       CONCAT(N'[', c.TenCapDo, N'] ', m.MoTa)
  FROM KhoaHoc k
  JOIN CapDo c ON c.MaCapDo = k.MaCapDo
 CROSS JOIN (VALUES
    ( 1, N'Giới thiệu khóa học & kiểm tra trình độ', N'Làm quen, phổ biến nội quy, kiểm tra đầu vào 4 kỹ năng.'),
    ( 2, N'Unit 1 — Từ vựng & phát âm',              N'Chủ đề quen thuộc, trọng âm từ, sửa lỗi phát âm thường gặp.'),
    ( 3, N'Unit 1 — Ngữ pháp trọng tâm',             N'Cấu trúc câu cơ bản của Unit 1, bài tập vận dụng.'),
    ( 4, N'Unit 2 — Từ vựng & phát âm',              N'Mở rộng vốn từ chủ đề 2, luyện nói theo cặp.'),
    ( 5, N'Unit 2 — Ngữ pháp trọng tâm',             N'Thì/cấu trúc của Unit 2, luyện tập có kiểm soát.'),
    ( 6, N'Unit 3 — Từ vựng & phát âm',              N'Từ vựng Unit 3, ngữ điệu trong câu hỏi.'),
    ( 7, N'Unit 3 — Ngữ pháp trọng tâm',             N'Cấu trúc Unit 3, bài tập viết câu.'),
    ( 8, N'Ôn tập giữa kỳ 1',                        N'Hệ thống Unit 1–3, luyện đề tổng hợp.'),
    ( 9, N'Kiểm tra giữa kỳ 1',                      N'Bài kiểm tra 4 kỹ năng, chấm và nhận xét.'),
    (10, N'Unit 4 — Nghe & nói',                     N'Kỹ năng nghe hiểu và phản xạ nói theo chủ đề Unit 4.'),
    (11, N'Unit 4 — Đọc & viết',                     N'Đọc hiểu đoạn văn, viết đoạn ngắn theo mẫu.'),
    (12, N'Unit 5 — Nghe & nói',                     N'Hội thoại tình huống thực tế, thuyết trình ngắn.'),
    (13, N'Unit 5 — Đọc & viết',                     N'Kỹ thuật đọc lướt, viết email/thư ngắn.'),
    (14, N'Unit 6 — Từ vựng & ngữ pháp',             N'Hệ thống từ vựng và cấu trúc nâng cao của Unit 6.'),
    (15, N'Unit 6 — Kỹ năng tổng hợp',               N'Bài tập tích hợp 4 kỹ năng trên nền Unit 6.'),
    (16, N'Luyện đề & thực hành tình huống',         N'Làm đề theo dạng bài, xử lý tình huống giao tiếp.'),
    (17, N'Ôn tập cuối kỳ',                          N'Ôn toàn bộ chương trình, chữa lỗi thường gặp.'),
    (18, N'Kiểm tra cuối kỳ',                         N'Bài kiểm tra tổng kết, so sánh với đầu vào.'),
    (19, N'Chữa đề & phản hồi cá nhân',              N'Chữa bài kiểm tra, kế hoạch cải thiện cho từng học viên.'),
    (20, N'Tổng kết & định hướng bước tiếp',         N'Tổng kết khóa, cấp chứng nhận, tư vấn lộ trình tiếp theo.')
 ) m(SoBuoi, TieuDe, MoTa)
 WHERE NOT EXISTS (
        SELECT 1 FROM NoiDungBaiHoc n
         WHERE n.MaKhoaHoc = k.MaKhoaHoc AND n.SoBuoi = m.SoBuoi);

PRINT N'  - Đã seed ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' dòng nội dung bài học.';
GO

/* =============================================================================
   3. CHUẨN HÓA KHÓA HỌC: 20 BUỔI × 2 GIỜ = 40 GIỜ  (GIỮ NGUYÊN HỌC PHÍ)
   ============================================================================= */

PRINT N'===== BƯỚC 3: CHUẨN HÓA KhoaHoc =====';

UPDATE KhoaHoc
   SET SoBuoi    = 20,
       TongSoGio = 40
 WHERE SoBuoi <> 20 OR TongSoGio IS NULL OR TongSoGio <> 40;

PRINT N'  - Đã chuẩn hóa ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' khóa học về 20 buổi / 40 giờ.';
GO

/* =============================================================================
   4. KIỂM TRA
   ============================================================================= */

PRINT N'===== KIỂM TRA KẾT QUẢ =====';

SELECT N'CaHoc'          AS Bang, COUNT(*) AS SoDong FROM CaHoc
UNION ALL SELECT N'DiemDanh',       COUNT(*) FROM DiemDanh
UNION ALL SELECT N'NgayLe',         COUNT(*) FROM NgayLe
UNION ALL SELECT N'NoiDungBaiHoc',  COUNT(*) FROM NoiDungBaiHoc;
GO

SELECT k.MaKhoaHoc,
       k.TenKhoaHoc,
       k.SoBuoi,
       k.TongSoGio,
       k.HocPhiCoBan,
       (SELECT COUNT(*) FROM NoiDungBaiHoc n WHERE n.MaKhoaHoc = k.MaKhoaHoc) AS SoNoiDung
  FROM KhoaHoc k
 ORDER BY k.MaKhoaHoc;
GO

PRINT N'===== NGÀY LỄ =====';

SELECT Ngay, TenNgayLe FROM NgayLe ORDER BY Ngay;
GO

PRINT N'=== BỔ SUNG v10 HOÀN TẤT ===';
GO
