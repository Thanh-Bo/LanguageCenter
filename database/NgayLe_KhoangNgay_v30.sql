/* =====================================================================================
   NgayLe_KhoangNgay_v30.sql
   -------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Nâng bảng NgayLe từ "1 dòng = 1 NGÀY" thành "1 dòng = 1 KỲ NGHỈ".

       Trước:  nghỉ Tết 15 ngày phải nhập 15 dòng, thiếu 1 ngày là hỏng lịch.
       Sau:    thêm cột DenNgay → 1 dòng mô tả trọn kỳ nghỉ.

   THAY ĐỔI SCHEMA
       1. ALTER TABLE NgayLe ADD DenNgay date NULL      (NULL = nghỉ đúng 1 ngày)
       2. Bỏ UNIQUE index UQ_NgayLe_Ngay  → thay bằng index thường IX_NgayLe_Ngay
          (vì giờ nhiều kỳ nghỉ có thể bắt đầu khác ngày nhưng chồng lấn — việc chặn
           chồng lấn do tầng service lo, không dùng UNIQUE được nữa)
       3. Thêm CHECK CK_NgayLe_Khoang: DenNgay IS NULL OR DenNgay >= Ngay

   DỮ LIỆU
       Xoá toàn bộ ngày lễ cũ và nạp lại TRỌN BỘ 2026 → 2027 (theo yêu cầu):
           01/01/2026                        Tết Dương lịch
           14/02/2026 – 28/02/2026           Tết Nguyên đán Bính Ngọ
           30/04/2026 – 01/05/2026           Lễ 30/04 & 01/05
           02/09/2026                        Quốc khánh
           01/01/2027                        Tết Dương lịch
           04/02/2027 – 18/02/2027           Tết Nguyên đán Đinh Mùi
           30/04/2027 – 01/05/2027           Lễ 30/04 & 01/05
           02/09/2027                        Quốc khánh

   KHÔNG ĐỘNG TỚI
       • Ca học, lịch học, lớp học, phân công — dữ liệu ca học hiện có giữ nguyên.

   AN TOÀN
       • DDL (thêm cột / index / check) chạy ở BATCH RIÊNG: SQL Server biên dịch cả batch
         trước khi thực thi, nên nếu ALTER TABLE ADD DenNgay nằm cùng batch với câu lệnh
         dùng DenNgay thì sẽ báo "Invalid column name 'DenNgay'" (Msg 207).
       • Phần nạp dữ liệu: 1 transaction + XACT_ABORT.
       • Idempotent: chạy lại vẫn cho cùng kết quả (không nhân đôi dữ liệu).
   ===================================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

/* =====================================================================================
   0. TIỀN KIỂM TRA
   ===================================================================================== */
IF OBJECT_ID(N'dbo.NgayLe', N'U') IS NULL
BEGIN
    RAISERROR(N'Không tìm thấy bảng dbo.NgayLe. Kiểm tra database đang trỏ tới.', 16, 1);
END
GO

PRINT N'=== v30 — NGÀY LỄ THEO KHOẢNG ===';

/* =====================================================================================
   1. THÊM CỘT DenNgay
   -------------------------------------------------------------------------------------
   BẮT BUỘC là BATCH RIÊNG (GO ngay dưới). SQL Server biên dịch cả batch trước khi chạy,
   nên mọi câu lệnh nhắc tới DenNgay trong CÙNG batch với ALTER TABLE sẽ báo
   "Invalid column name 'DenNgay'" (Msg 207).
   ===================================================================================== */
IF COL_LENGTH(N'dbo.NgayLe', N'DenNgay') IS NULL
BEGIN
    ALTER TABLE dbo.NgayLe ADD DenNgay date NULL;
    PRINT N'  • Đã thêm cột DenNgay.';
END
ELSE
BEGIN
    PRINT N'  • Cột DenNgay đã có — bỏ qua.';
END
GO

/* =====================================================================================
   2. BỎ UNIQUE INDEX, THAY BẰNG INDEX THƯỜNG
   -------------------------------------------------------------------------------------
   Giờ nhiều kỳ nghỉ có thể chồng lấn nhau về khoảng ngày nên KHÔNG dùng UNIQUE(Ngay)
   được nữa — việc chặn chồng lấn do tầng service (AcademicSetupService) đảm nhận.
   ===================================================================================== */
IF EXISTS (SELECT 1 FROM sys.indexes
           WHERE name = N'UQ_NgayLe_Ngay'
             AND object_id = OBJECT_ID(N'dbo.NgayLe'))
BEGIN
    DROP INDEX UQ_NgayLe_Ngay ON dbo.NgayLe;
    PRINT N'  • Đã bỏ UNIQUE index UQ_NgayLe_Ngay.';
END

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = N'IX_NgayLe_Ngay'
                 AND object_id = OBJECT_ID(N'dbo.NgayLe'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_NgayLe_Ngay
        ON dbo.NgayLe (Ngay) INCLUDE (DenNgay);
    PRINT N'  • Đã tạo index IX_NgayLe_Ngay.';
END

/* =====================================================================================
   3. CHECK KHOẢNG NGÀY
   ===================================================================================== */
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints
               WHERE name = N'CK_NgayLe_Khoang'
                 AND parent_object_id = OBJECT_ID(N'dbo.NgayLe'))
BEGIN
    ALTER TABLE dbo.NgayLe WITH CHECK
        ADD CONSTRAINT CK_NgayLe_Khoang
        CHECK (DenNgay IS NULL OR DenNgay >= Ngay);
    PRINT N'  • Đã thêm ràng buộc CK_NgayLe_Khoang.';
END
GO

/* =====================================================================================
   4. NẠP LẠI DỮ LIỆU 2026 → 2027   (batch riêng — có tham chiếu DenNgay)
   ===================================================================================== */
BEGIN TRY
    BEGIN TRANSACTION;

    DELETE FROM dbo.NgayLe;

    DBCC CHECKIDENT('dbo.NgayLe', RESEED, 0) WITH NO_INFOMSGS;

    INSERT INTO dbo.NgayLe (Ngay, DenNgay, TenNgayLe, GhiChu) VALUES
        (CAST('2026-01-01' AS date), NULL,
         N'Tết Dương lịch', N'Nghỉ 1 ngày'),

        (CAST('2026-02-14' AS date), CAST('2026-02-28' AS date),
         N'Tết Nguyên đán Bính Ngọ', N'Nghỉ 15 ngày (28 tháng Chạp → mùng 6)'),

        (CAST('2026-04-30' AS date), CAST('2026-05-01' AS date),
         N'Lễ 30/04 & 01/05', N'Nghỉ 2 ngày'),

        (CAST('2026-09-02' AS date), NULL,
         N'Quốc khánh', N'Nghỉ 1 ngày'),

        (CAST('2027-01-01' AS date), NULL,
         N'Tết Dương lịch', N'Nghỉ 1 ngày'),

        (CAST('2027-02-04' AS date), CAST('2027-02-18' AS date),
         N'Tết Nguyên đán Đinh Mùi', N'Nghỉ 15 ngày (28 tháng Chạp → mùng 6)'),

        (CAST('2027-04-30' AS date), CAST('2027-05-01' AS date),
         N'Lễ 30/04 & 01/05', N'Nghỉ 2 ngày'),

        (CAST('2027-09-02' AS date), NULL,
         N'Quốc khánh', N'Nghỉ 1 ngày');

    COMMIT TRANSACTION;

    PRINT N'  • Đã nạp 8 kỳ nghỉ (2026 → 2027).';
    PRINT N'=== HOÀN TẤT v30 ===';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT N'LỖI: ' + ERROR_MESSAGE() + N' (dòng ' + CAST(ERROR_LINE() AS nvarchar(10)) + N')';
    THROW;
END CATCH;
GO

/* =====================================================================================
   5. KIỂM TRA
   ===================================================================================== */
PRINT N'--- KIỂM TRA ---';

SELECT MaNgayLe,
       Ngay                                                        AS TuNgay,
       DenNgay,
       ISNULL(DATEDIFF(day, Ngay, DenNgay) + 1, 1)                 AS SoNgayNghi,
       TenNgayLe,
       GhiChu
FROM dbo.NgayLe
ORDER BY Ngay;

/* Chồng lấn giữa các kỳ nghỉ (phải RỖNG) */
SELECT N'CẢNH BÁO: kỳ nghỉ chồng lấn' AS CanhBao,
       a.MaNgayLe AS Id1, a.TenNgayLe AS Ky1, a.Ngay AS Tu1,
       ISNULL(a.DenNgay, a.Ngay) AS Den1,
       b.MaNgayLe AS Id2, b.TenNgayLe AS Ky2, b.Ngay AS Tu2,
       ISNULL(b.DenNgay, b.Ngay) AS Den2
FROM dbo.NgayLe a
JOIN dbo.NgayLe b
    ON b.MaNgayLe > a.MaNgayLe
   AND a.Ngay <= ISNULL(b.DenNgay, b.Ngay)
   AND b.Ngay <= ISNULL(a.DenNgay, a.Ngay);

/* Trùng ngày bắt đầu (phải RỖNG) */
SELECT N'CẢNH BÁO: trùng ngày bắt đầu' AS CanhBao, Ngay, COUNT(*) AS SoKy
FROM dbo.NgayLe
GROUP BY Ngay
HAVING COUNT(*) > 1;
