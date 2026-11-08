/* =====================================================================================
   CaHoc_NoiDungThamChieu_v41.sql
   -------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Thêm cột dbo.CaHoc.MaNoiDung — ca học THAM CHIẾU tới dòng syllabus của khóa
       (bảng NoiDungBaiHoc) thay vì chỉ copy chuỗi tiêu đề.

   VÌ SAO CẦN
       Trước đây khi sinh ca, hệ thống ghi vào CaHoc.NoiDungBaiHoc chuỗi "Buổi N — Tiêu đề".
       Hệ quả: sửa syllabus sau đó thì ca cũ vẫn giữ chữ cũ, và nội dung chi tiết
       (NoiDung / MucTieu) không tới được giáo viên.

       Với MaNoiDung:
         • Nội dung bài học là dữ liệu của KHÓA HỌC — nhập 1 lần, mọi lớp dùng chung.
         • Ca học chỉ trỏ tới dòng syllabus tương ứng.
         • Nút "Đồng bộ nội dung → ca học" cập nhật được các ca CHƯA DIỄN RA.

   AN TOÀN
       • Kiểm tra COL_LENGTH / sys.foreign_keys / sys.indexes → chạy nhiều lần vô hại.
       • Chỉ THÊM cột NULL + FK + index, có BACKFILL cho dữ liệu cũ theo (MaLopHoc → MaKhoaHoc, SoBuoi).
       • Không xoá, không đổi cột đã có.

   ⚠ VÌ SAO CÓ NHIỀU "GO"
       SQL Server BIÊN DỊCH CẢ BATCH trước khi thực thi. Câu lệnh dùng cột vừa được
       ALTER thêm trong CÙNG batch sẽ báo "Msg 207 Invalid column name" và KHÔNG
       câu lệnh nào trong batch chạy (kể cả ALTER + BEGIN TRANSACTION).
       Vì vậy mỗi bước nằm ở batch riêng:
         Batch 1: thêm cột · Batch 2: FK + index · Batch 3: backfill · Batch 4: kiểm tra
   ===================================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

PRINT N'=====================================================================';
PRINT N'  v41 — THÊM CaHoc.MaNoiDung (THAM CHIẾU NỘI DUNG BÀI HỌC CỦA KHÓA)';
PRINT N'=====================================================================';

-- =====================================================================================
-- BATCH 1 — CỘT MaNoiDung
-- =====================================================================================
BEGIN TRY
    IF OBJECT_ID(N'dbo.CaHoc', N'U') IS NULL
    BEGIN
        THROW 50041, N'Không tìm thấy bảng dbo.CaHoc. Kiểm tra database đang trỏ tới.', 1;
    END

    IF OBJECT_ID(N'dbo.NoiDungBaiHoc', N'U') IS NULL
    BEGIN
        THROW 50042, N'Không tìm thấy bảng dbo.NoiDungBaiHoc. Kiểm tra database đang trỏ tới.', 1;
    END

    IF COL_LENGTH('dbo.CaHoc', 'MaNoiDung') IS NULL
    BEGIN
        ALTER TABLE dbo.CaHoc ADD MaNoiDung INT NULL;
        PRINT N'  + Thêm cột CaHoc.MaNoiDung';
    END
    ELSE
    BEGIN
        PRINT N'  = Cột CaHoc.MaNoiDung đã có — bỏ qua';
    END
END TRY
BEGIN CATCH
    PRINT N'*** LỖI (cột): ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO

-- =====================================================================================
-- BATCH 2 — KHOÁ NGOẠI + CHỈ MỤC (batch riêng vì có tham chiếu cột mới)
-- =====================================================================================
BEGIN TRY
    BEGIN TRANSACTION;

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.foreign_keys
        WHERE name = N'FK_CaHoc_NoiDungBaiHoc'
          AND parent_object_id = OBJECT_ID(N'dbo.CaHoc')
    )
    BEGIN
        ALTER TABLE dbo.CaHoc WITH CHECK
            ADD CONSTRAINT FK_CaHoc_NoiDungBaiHoc
                FOREIGN KEY (MaNoiDung) REFERENCES dbo.NoiDungBaiHoc (MaNoiDung);

        PRINT N'  + Thêm khoá ngoại FK_CaHoc_NoiDungBaiHoc → NoiDungBaiHoc(MaNoiDung)';
    END
    ELSE
    BEGIN
        PRINT N'  = Khoá ngoại FK_CaHoc_NoiDungBaiHoc đã có — bỏ qua';
    END

    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.indexes
        WHERE name = N'IX_CaHoc_MaNoiDung'
          AND object_id = OBJECT_ID(N'dbo.CaHoc')
    )
    BEGIN
        CREATE INDEX IX_CaHoc_MaNoiDung ON dbo.CaHoc (MaNoiDung);
        PRINT N'  + Tạo chỉ mục IX_CaHoc_MaNoiDung';
    END
    ELSE
    BEGIN
        PRINT N'  = Chỉ mục IX_CaHoc_MaNoiDung đã có — bỏ qua';
    END

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
    BEGIN
        ROLLBACK TRANSACTION;
    END

    PRINT N'*** LỖI (FK/index): ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO

-- =====================================================================================
-- BATCH 3 — BACKFILL: nối ca học cũ với syllabus theo (MaKhoaHoc của lớp, SoBuoi của ca)
-- =====================================================================================
BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE ch
       SET ch.MaNoiDung = nd.MaNoiDung
      FROM dbo.CaHoc AS ch
      INNER JOIN dbo.LopHoc AS lop
              ON lop.MaLopHoc = ch.MaLopHoc
      INNER JOIN dbo.NoiDungBaiHoc AS nd
              ON nd.MaKhoaHoc = lop.MaKhoaHoc
             AND nd.SoBuoi    = ch.SoBuoi
     WHERE ch.MaNoiDung IS NULL
       AND ch.SoBuoi IS NOT NULL;

    PRINT N'  + Backfill MaNoiDung cho ca học cũ: ' + CAST(@@ROWCOUNT AS nvarchar(20)) + N' dòng';

    COMMIT TRANSACTION;

    PRINT N'---------------------------------------------------------------------';
    PRINT N'  HOÀN TẤT: CaHoc đã có cột MaNoiDung.';
    PRINT N'---------------------------------------------------------------------';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
    BEGIN
        ROLLBACK TRANSACTION;
    END

    PRINT N'*** LỖI (backfill): ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO

-- =====================================================================================
-- BATCH 4 — KIỂM TRA SAU KHI CHẠY (chỉ đọc)
-- =====================================================================================
PRINT N'';
PRINT N'--- Tỷ lệ ca học đã nối được syllabus ---';

SELECT
    CASE WHEN MaNoiDung IS NULL THEN N'Chưa nối' ELSE N'Đã nối' END AS TrangThai,
    COUNT(*) AS SoCa
FROM dbo.CaHoc
GROUP BY CASE WHEN MaNoiDung IS NULL THEN N'Chưa nối' ELSE N'Đã nối' END;
GO
