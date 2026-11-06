/* ============================================================================================
   DanhDauBuoiKiemTraThi_v21.sql
   --------------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Đánh dấu những buổi nào là KIỂM TRA, buổi nào là THI cho một khóa học
       — ở CẢ hai mức: đề cương (NoiDungBaiHoc) và ca học thực tế (CaHoc).

   VÌ SAO CẦN
       Cột LoaiBuoi của toàn bộ 20 buổi hiện đang là N'Buổi học'.
       Những chữ như "Buổi 9 — Kiểm tra giữa kỳ" chỉ là TIÊU ĐỀ NỘI DUNG,
       KHÔNG phải loại buổi — nên giao diện chưa phân biệt được.

   KẾT QUẢ SAU KHI CHẠY
       - Web    : thẻ ca học đổi màu + badge KT / THI, chú thích dưới lưới đúng loại.
       - WinForms: dòng ca học được tô màu + cột "Buổi · Loại buổi" in đậm.

   CÁCH DÙNG
       Sửa @MaKhoaHoc và bảng @Map bên dưới rồi chạy toàn bộ file.
       Idempotent — chạy lại nhiều lần vẫn cho cùng kết quả.
   ============================================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

/* --------------------------------------------------------------------------------------------
   THAM SỐ
   -------------------------------------------------------------------------------------------- */
DECLARE @MaKhoaHoc int = 14;      -- IELTS Advanced
DECLARE @MaLopHoc  int = NULL;    -- NULL = mọi lớp của khóa; ví dụ: 1

/** Buổi nào là loại gì. Bỏ bớt/thêm dòng tùy chương trình học. */
DECLARE @Map TABLE
(
    SoBuoi   int          NOT NULL PRIMARY KEY,
    LoaiBuoi nvarchar(30) NOT NULL
);

INSERT INTO @Map (SoBuoi, LoaiBuoi)
VALUES
    (9,  N'Kiểm tra'),   -- kiểm tra giữa kỳ
    (18, N'Buổi thi');   -- thi cuối kỳ

/* --------------------------------------------------------------------------------------------
   KIỂM TRA ĐIỀU KIỆN
   -------------------------------------------------------------------------------------------- */
IF NOT EXISTS (SELECT 1 FROM dbo.KhoaHoc WHERE MaKhoaHoc = @MaKhoaHoc)
BEGIN
    RAISERROR(N'Không tìm thấy khóa học MaKhoaHoc = %d.', 16, 1, @MaKhoaHoc);
    RETURN;
END;

IF EXISTS (SELECT 1 FROM @Map WHERE LoaiBuoi NOT IN (N'Buổi học', N'Kiểm tra', N'Buổi thi'))
BEGIN
    RAISERROR(N'LoaiBuoi trong @Map không hợp lệ (chỉ nhận Buổi học / Kiểm tra / Buổi thi).', 16, 1);
    RETURN;
END;

/* --------------------------------------------------------------------------------------------
   TRƯỚC KHI SỬA
   -------------------------------------------------------------------------------------------- */
PRINT N'=== TRƯỚC KHI SỬA ===';

SELECT N'NoiDungBaiHoc' AS Bang, LoaiBuoi, COUNT(*) AS SoDong
FROM dbo.NoiDungBaiHoc
WHERE MaKhoaHoc = @MaKhoaHoc
GROUP BY LoaiBuoi
UNION ALL
SELECT N'CaHoc', ch.LoaiBuoi, COUNT(*)
FROM dbo.CaHoc AS ch
JOIN dbo.LopHoc AS l ON l.MaLopHoc = ch.MaLopHoc
WHERE l.MaKhoaHoc = @MaKhoaHoc
  AND (@MaLopHoc IS NULL OR ch.MaLopHoc = @MaLopHoc)
GROUP BY ch.LoaiBuoi;

/* --------------------------------------------------------------------------------------------
   CẬP NHẬT
   -------------------------------------------------------------------------------------------- */
BEGIN TRY
    BEGIN TRANSACTION;

    /* 1) Đề cương khóa học ------------------------------------------------ */
    -- Buổi không nằm trong @Map -> trả về "Buổi học"
    UPDATE nd
    SET LoaiBuoi = N'Buổi học'
    FROM dbo.NoiDungBaiHoc AS nd
    WHERE nd.MaKhoaHoc = @MaKhoaHoc
      AND nd.SoBuoi NOT IN (SELECT SoBuoi FROM @Map)
      AND nd.LoaiBuoi <> N'Buổi học';

    -- Buổi nằm trong @Map -> đúng loại
    UPDATE nd
    SET LoaiBuoi = m.LoaiBuoi
    FROM dbo.NoiDungBaiHoc AS nd
    JOIN @Map AS m ON m.SoBuoi = nd.SoBuoi
    WHERE nd.MaKhoaHoc = @MaKhoaHoc
      AND nd.LoaiBuoi <> m.LoaiBuoi;

    /* 2) Ca học thực tế --------------------------------------------------- */
    UPDATE ch
    SET LoaiBuoi = N'Buổi học'
    FROM dbo.CaHoc AS ch
    JOIN dbo.LopHoc AS l ON l.MaLopHoc = ch.MaLopHoc
    WHERE l.MaKhoaHoc = @MaKhoaHoc
      AND (@MaLopHoc IS NULL OR ch.MaLopHoc = @MaLopHoc)
      AND (ch.SoBuoi IS NULL OR ch.SoBuoi NOT IN (SELECT SoBuoi FROM @Map))
      AND ch.LoaiBuoi <> N'Buổi học';

    UPDATE ch
    SET LoaiBuoi = m.LoaiBuoi
    FROM dbo.CaHoc AS ch
    JOIN dbo.LopHoc AS l ON l.MaLopHoc = ch.MaLopHoc
    JOIN @Map AS m ON m.SoBuoi = ch.SoBuoi
    WHERE l.MaKhoaHoc = @MaKhoaHoc
      AND (@MaLopHoc IS NULL OR ch.MaLopHoc = @MaLopHoc)
      AND ch.LoaiBuoi <> m.LoaiBuoi;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    DECLARE @ThongDiep nvarchar(2000) =
        N'LỖI: ' + ERROR_MESSAGE()
        + N' (dòng ' + CAST(ERROR_LINE() AS nvarchar(10)) + N') — đã ROLLBACK.';

    RAISERROR(@ThongDiep, 16, 1);
    RETURN;
END CATCH;

/* --------------------------------------------------------------------------------------------
   SAU KHI SỬA
   -------------------------------------------------------------------------------------------- */
PRINT N'';
PRINT N'=== SAU KHI SỬA ===';

SELECT N'NoiDungBaiHoc' AS Bang, LoaiBuoi, COUNT(*) AS SoDong
FROM dbo.NoiDungBaiHoc
WHERE MaKhoaHoc = @MaKhoaHoc
GROUP BY LoaiBuoi
UNION ALL
SELECT N'CaHoc', ch.LoaiBuoi, COUNT(*)
FROM dbo.CaHoc AS ch
JOIN dbo.LopHoc AS l ON l.MaLopHoc = ch.MaLopHoc
WHERE l.MaKhoaHoc = @MaKhoaHoc
  AND (@MaLopHoc IS NULL OR ch.MaLopHoc = @MaLopHoc)
GROUP BY ch.LoaiBuoi;

-- Chi tiết các buổi đặc biệt để đối chiếu trên giao diện
SELECT
    ch.MaLopHoc,
    l.MaLop,
    ch.SoBuoi,
    ch.LoaiBuoi,
    ch.NgayHoc,
    ch.MaGiaoVienThucTe,
    ch.TrangThai
FROM dbo.CaHoc AS ch
JOIN dbo.LopHoc AS l ON l.MaLopHoc = ch.MaLopHoc
WHERE l.MaKhoaHoc = @MaKhoaHoc
  AND (@MaLopHoc IS NULL OR ch.MaLopHoc = @MaLopHoc)
  AND ch.LoaiBuoi <> N'Buổi học'
ORDER BY ch.MaLopHoc, ch.SoBuoi;

PRINT N'Xong. Mở lại màn "Lịch học → Ca học thực tế" để thấy dòng được tô màu.';
