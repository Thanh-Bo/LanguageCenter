/* ============================================================================================
   ChinhNoiDung_KhoaHoc_v22.sql
   --------------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Viết lại ĐỀ CƯƠNG 20 BUỔI (Đề xuất B — 4 buổi đánh giá, tách theo 4 kỹ năng)
       cho khóa IELTS Advanced và áp dụng luôn vào lớp K01.

   ĐỀ XUẤT B — các buổi ĐÁNH GIÁ
       Buổi 8  → Kiểm tra   : Reading + Writing
       Buổi 13 → Kiểm tra   : Speaking + Listening
       Buổi 18 → Buổi thi   : Reading + Writing (cuối kỳ)
       Buổi 19 → Buổi thi   : Speaking + Listening (cuối kỳ)
       (buổi 9, 14 = chữa bài; buổi 20 = tổng kết — vẫn là Buổi học)

   ĐỐI CHIẾU QUY ƯỚC GIÁO VIÊN (lẻ = GV nước ngoài, chẵn = GV Việt)
       Buổi 8, 18 (chẵn) → GV Việt  : chấm Reading/Writing
       Buổi 13, 19 (lẻ)  → GV nước ngoài : chấm Speaking/Listening

   PHẠM VI TÁC ĐỘNG
       1. NoiDungBaiHoc của khóa  → đề cương dùng cho MỌI lớp mới sinh buổi.
       2. CaHoc của lớp K01      → cập nhật LoaiBuoi + Nội dung (snapshot) của 20 buổi.

   LƯU Ý
       - PHẢI chạy ThemLoaiBuoi_KiemTra_v20.sql trước (CHECK constraint).
       - Idempotent: chạy lại nhiều lần vẫn cho cùng kết quả.
       - KHÔNG đổi giáo viên / phòng / giờ / trạng thái ca học.
   ============================================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

/* --------------------------------------------------------------------------------------------
   THAM SỐ
   -------------------------------------------------------------------------------------------- */
DECLARE @MaKhoaHoc int         = 14;              -- IELTS Advanced
DECLARE @MaLop     varchar(50) = 'IELTS-A-01';    -- lớp K01 (đổi nếu mã lớp khác)

/* --------------------------------------------------------------------------------------------
   ĐỀ CƯƠNG 20 BUỔI
   -------------------------------------------------------------------------------------------- */
DECLARE @DeCuong TABLE
(
    SoBuoi   int          NOT NULL PRIMARY KEY,
    TieuDe   nvarchar(200) NOT NULL,
    NoiDung  nvarchar(1000) NOT NULL,
    LoaiBuoi nvarchar(30)  NOT NULL
);

INSERT INTO @DeCuong (SoBuoi, TieuDe, NoiDung, LoaiBuoi)
VALUES
    (1,  N'Chẩn đoán đầu vào & định hướng lộ trình',
         N'Làm test nhanh 4 kỹ năng để xác định band hiện tại; phổ biến nội quy, cách tính điểm; đặt mục tiêu band cho từng học viên.',
         N'Buổi học'),

    (2,  N'Unit 1 — Từ vựng & phát âm',
         N'Hệ thống từ vựng chủ đề quen thuộc; trọng âm từ, âm cuối, sửa lỗi phát âm thường gặp của người Việt.',
         N'Buổi học'),

    (3,  N'Unit 1 — Ngữ pháp trọng tâm & Speaking Part 1',
         N'Cấu trúc câu nâng cao của Unit 1; luyện trả lời Part 1 theo 3 bước (trả lời – giải thích – ví dụ).',
         N'Buổi học'),

    (4,  N'Unit 2 — Từ vựng học thuật & Reading',
         N'Mở rộng vốn từ học thuật; kỹ năng Skimming/Scanning, dạng Matching Headings và True/False/Not Given.',
         N'Buổi học'),

    (5,  N'Unit 2 — Nghe & Nói: dạng bài thường gặp',
         N'Listening Section 1–2 (form completion, map labelling); Speaking Part 2 – xây dựng 2 phút nói liên tục.',
         N'Buổi học'),

    (6,  N'Unit 3 — Ngữ pháp & Writing Task 1',
         N'Cấu trúc câu phức, mệnh đề quan hệ; phân tích và mô tả biểu đồ (line, bar, pie) theo 4 đoạn chuẩn.',
         N'Buổi học'),

    (7,  N'Unit 3 — Writing Task 2: mở bài & thân bài',
         N'Phân tích đề, lập dàn ý 2 câu hỏi; viết mở bài (paraphrase + thesis) và 2 thân bài có ví dụ.',
         N'Buổi học'),

    (8,  N'Kiểm tra Reading + Writing',
         N'Bài kiểm tra 60 phút: Reading 30 phút (2 passage) + Writing Task 1 (20 phút) và Task 2 rút gọn (40 phút). Chấm theo band descriptor.',
         N'Kiểm tra'),

    (9,  N'Chữa Reading & Writing — phân tích lỗi',
         N'Trả bài, chữa từng lỗi sai phổ biến; so sánh bài làm với band 6.5/7.0; xây danh sách lỗi cá nhân cần sửa.',
         N'Buổi học'),

    (10, N'Unit 4 — Reading nâng cao & từ vựng học thuật',
         N'Dạng Yes/No/Not Given, Matching Information; đọc hiểu đoạn dài, quản lý thời gian 20 phút/passage.',
         N'Buổi học'),

    (11, N'Unit 4 — Listening nâng cao & chiến lược bắt từ khoá',
         N'Section 3–4 (multiple choice, note completion); dự đoán đáp án, nhận diện paraphrase, xử lý accent khó.',
         N'Buổi học'),

    (12, N'Unit 5 — Writing Task 2: phát triển ý & liên kết',
         N'Kỹ thuật mở rộng ý (PEEL), từ nối, tránh lặp từ; viết kết bài; chấm chéo theo checklist.',
         N'Buổi học'),

    (13, N'Kiểm tra Speaking + Listening',
         N'Bài kiểm tra 45 phút: Listening 30 phút (2 section) + Speaking 10 phút (full 3 part, ghi âm). Chấm theo 4 tiêu chí.',
         N'Kiểm tra'),

    (14, N'Chữa Speaking & Listening — band descriptors',
         N'Nghe lại bản ghi, sửa phát âm/ngữ điệu; phân tích lỗi nghe; hướng dẫn tự đánh giá theo 4 tiêu chí Speaking.',
         N'Buổi học'),

    (15, N'Unit 6 — Speaking Part 2 & 3 chuyên sâu',
         N'Chủ đề khó, câu hỏi mở rộng; kỹ thuật kéo dài câu trả lời, paraphrase, đưa ví dụ và quan điểm cá nhân.',
         N'Buổi học'),

    (16, N'Unit 6 — Luyện đề tổng hợp Reading + Writing',
         N'Làm full Reading 60 phút và 1 đề Writing hoàn chỉnh dưới áp lực thời gian; chữa nhanh theo lỗi nhóm.',
         N'Buổi học'),

    (17, N'Ôn tập cuối kỳ — hệ thống 4 kỹ năng',
         N'Hệ thống lại toàn bộ chiến lược làm bài, các dạng đề hay ra; tổng hợp lỗi cá nhân và checklist trước thi.',
         N'Buổi học'),

    (18, N'Thi cuối kỳ — Reading + Writing',
         N'Bài thi cuối kỳ: Reading 60 phút (full 3 passage) + Writing Task 1 (20 phút) và Task 2 (40 phút).',
         N'Buổi thi'),

    (19, N'Thi cuối kỳ — Speaking + Listening',
         N'Bài thi cuối kỳ: Listening 30 phút (full 4 section) + Speaking 11–14 phút (full 3 part, ghi âm).',
         N'Buổi thi'),

    (20, N'Tổng kết & định hướng lộ trình tiếp theo',
         N'Trả kết quả cuối kỳ, so sánh với mục tiêu band; nhận xét từng học viên; tư vấn lộ trình và tài liệu tự học.',
         N'Buổi học');

/* --------------------------------------------------------------------------------------------
   KIỂM TRA ĐIỀU KIỆN
   -------------------------------------------------------------------------------------------- */
-- RAISERROR không nhận subquery trong danh sách tham số -> phải gán ra biến trước.
DECLARE @SoBuoiDeCuong int = (SELECT COUNT(*) FROM @DeCuong);

IF @SoBuoiDeCuong <> 20
BEGIN
    RAISERROR(N'Đề cương phải đủ 20 buổi (hiện có %d).', 16, 1, @SoBuoiDeCuong);
    RETURN;
END;

IF NOT EXISTS (SELECT 1 FROM dbo.KhoaHoc WHERE MaKhoaHoc = @MaKhoaHoc)
BEGIN
    RAISERROR(N'Không tìm thấy khóa học MaKhoaHoc = %d.', 16, 1, @MaKhoaHoc);
    RETURN;
END;

DECLARE @MaLopHoc int = (SELECT MaLopHoc FROM dbo.LopHoc WHERE MaLop = @MaLop);

IF @MaLopHoc IS NULL
BEGIN
    RAISERROR(N'Không tìm thấy lớp có mã = %s.', 16, 1, @MaLop);
    RETURN;
END;

IF NOT EXISTS (SELECT 1 FROM dbo.LopHoc WHERE MaLopHoc = @MaLopHoc AND MaKhoaHoc = @MaKhoaHoc)
    PRINT N'[CẢNH BÁO] Lớp ' + @MaLop + N' không thuộc khóa ' + CAST(@MaKhoaHoc AS varchar(10)) + N' — vẫn tiếp tục cập nhật ca học.';

/* --------------------------------------------------------------------------------------------
   TRƯỚC KHI SỬA
   -------------------------------------------------------------------------------------------- */
PRINT N'=== TRƯỚC KHI SỬA ===';

SELECT
    nd.SoBuoi,
    nd.TieuDe           AS TieuDeHienTai,
    nd.LoaiBuoi         AS LoaiBuoiHienTai
FROM dbo.NoiDungBaiHoc AS nd
WHERE nd.MaKhoaHoc = @MaKhoaHoc
ORDER BY nd.SoBuoi;

/* --------------------------------------------------------------------------------------------
   CẬP NHẬT
   -------------------------------------------------------------------------------------------- */
BEGIN TRY
    BEGIN TRANSACTION;

    /* 1) ĐỀ CƯƠNG KHÓA HỌC ------------------------------------------------ */
    UPDATE nd
    SET TieuDe   = d.TieuDe,
        NoiDung  = d.NoiDung,
        LoaiBuoi = d.LoaiBuoi
    FROM dbo.NoiDungBaiHoc AS nd
    JOIN @DeCuong AS d ON d.SoBuoi = nd.SoBuoi
    WHERE nd.MaKhoaHoc = @MaKhoaHoc
      AND (nd.TieuDe <> d.TieuDe OR nd.NoiDung <> d.NoiDung OR nd.LoaiBuoi <> d.LoaiBuoi);

    /* 2) CA HỌC THỰC TẾ CỦA LỚP K01 --------------------------------------- */
    -- Nội dung ca học là bản snapshot "Buổi N — Tiêu đề" -> cập nhật theo đề cương mới
    UPDATE ch
    SET LoaiBuoi        = d.LoaiBuoi,
        NoiDungBaiHoc   = N'Buổi ' + CAST(d.SoBuoi AS nvarchar(10)) + N' — ' + d.TieuDe
    FROM dbo.CaHoc AS ch
    JOIN @DeCuong AS d ON d.SoBuoi = ch.SoBuoi
    WHERE ch.MaLopHoc = @MaLopHoc
      AND (ch.LoaiBuoi <> d.LoaiBuoi
           OR ISNULL(ch.NoiDungBaiHoc, N'') <> N'Buổi ' + CAST(d.SoBuoi AS nvarchar(10)) + N' — ' + d.TieuDe);

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    DECLARE @ThongDiep nvarchar(2000) =
        N'LỖI: ' + ERROR_MESSAGE()
        + N' (dòng ' + CAST(ERROR_LINE() AS nvarchar(10)) + N') — đã ROLLBACK, không có dữ liệu nào được ghi.';

    RAISERROR(@ThongDiep, 16, 1);
    RETURN;
END CATCH;

/* --------------------------------------------------------------------------------------------
   SAU KHI SỬA
   -------------------------------------------------------------------------------------------- */
PRINT N'';
PRINT N'=== SAU KHI SỬA — ĐỀ CƯƠNG KHÓA ===';

SELECT nd.SoBuoi, nd.LoaiBuoi, nd.TieuDe
FROM dbo.NoiDungBaiHoc AS nd
WHERE nd.MaKhoaHoc = @MaKhoaHoc
ORDER BY nd.SoBuoi;

PRINT N'';
PRINT N'=== CA HỌC CỦA LỚP ' + @MaLop + N' ===';

SELECT
    ch.SoBuoi,
    ch.LoaiBuoi,
    ch.NgayHoc,
    ch.MaGiaoVienThucTe AS GiaoVien,
    ch.TrangThai,
    ch.NoiDungBaiHoc
FROM dbo.CaHoc AS ch
WHERE ch.MaLopHoc = @MaLopHoc
ORDER BY ch.SoBuoi;

PRINT N'';
PRINT N'=== CÁC BUỔI ĐÁNH GIÁ (để đối chiếu giao diện) ===';

SELECT
    ch.SoBuoi,
    ch.LoaiBuoi,
    ch.NgayHoc,
    ch.MaGiaoVienThucTe AS GiaoVien
FROM dbo.CaHoc AS ch
WHERE ch.MaLopHoc = @MaLopHoc
  AND ch.LoaiBuoi <> N'Buổi học'
ORDER BY ch.SoBuoi;

PRINT N'Xong. Mở "Lịch học → Ca học thực tế" để xem 4 dòng được tô màu (8, 13 = Kiểm tra / 18, 19 = Buổi thi).';
