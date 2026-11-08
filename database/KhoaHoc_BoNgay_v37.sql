/* =====================================================================================
   KhoaHoc_BoNgay_v37.sql
   -------------------------------------------------------------------------------------
   MỤC ĐÍCH
       BỎ HẲN 2 cột ngày khỏi bảng dbo.KhoaHoc:
           • NgayBatDau
           • NgayKetThuc

   LÝ DO
       Khóa học là KHUÔN MẪU dùng lại cho nhiều lớp. Ngày khai giảng và ngày kết thúc
       thuộc về TỪNG LỚP (LopHoc.NgayBatDau / LopHoc.NgayKetThucDuKien).
       Trước đây 2 cột này gây mâu thuẫn: khóa ghi khai giảng 27/11 nhưng lớp lại mở
       08/11 — không có ràng buộc nào chặn.

   ⚠ AN TOÀN — ĐỌC TRƯỚC KHI CHẠY
       1. SAO LƯU database trước (menu "Sao lưu & phục hồi" trong WinForms, hoặc
          chuột phải database → Tasks → Back Up…).
       2. Script IN RA toàn bộ giá trị 2 cột trước khi xoá (để đối chiếu nếu cần).
       3. Nếu 2 cột đang được object khác dùng (view, computed column, index) thì
          SQL Server sẽ BÁO LỖI và tự huỷ — khi đó xử lý object đó rồi chạy lại.
       4. Chạy nhiều lần vô hại: cột nào đã bị xoá thì bỏ qua.

   THỨ TỰ TRIỂN KHAI
       1) Chạy script này.
       2) Build & chạy lại API (entity đã bỏ 2 thuộc tính).
       3) Build & chạy lại WinForms.
   ===================================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

PRINT N'=====================================================================';
PRINT N'  v37 — BỎ CỘT NgayBatDau / NgayKetThuc KHỎI BẢNG KhoaHoc';
PRINT N'=====================================================================';

/* -------------------------------------------------------------------------------------
   1. IN GIÁ TRỊ HIỆN TẠI (để đối chiếu / khôi phục tay nếu cần)
   ------------------------------------------------------------------------------------- */
IF COL_LENGTH('dbo.KhoaHoc', 'NgayBatDau') IS NOT NULL
   AND COL_LENGTH('dbo.KhoaHoc', 'NgayKetThuc') IS NOT NULL
BEGIN
    PRINT N'--- 1. GIÁ TRỊ NGÀY ĐANG LƯU TRONG KhoaHoc ---';

    EXEC (N'SELECT MaKhoaHoc, TenKhoaHoc, NgayBatDau, NgayKetThuc FROM dbo.KhoaHoc ORDER BY MaKhoaHoc;');
END
ELSE
    PRINT N'--- 1. Hai cột ngày đã được bỏ từ trước ---';

PRINT N'';

/* -------------------------------------------------------------------------------------
   2. KIỂM TRA PHỤ THUỘC — object nào đang dùng 2 cột này
   ------------------------------------------------------------------------------------- */
PRINT N'--- 2. OBJECT ĐANG THAM CHIẾU 2 CỘT (rỗng là an toàn) ---';

SELECT OBJECT_NAME(d.referencing_id) AS DoiTuong,
       d.referenced_entity_name     AS Bang,
       d.referenced_minor_name      AS Cot
FROM sys.sql_expression_dependencies d
WHERE d.referenced_id = OBJECT_ID(N'dbo.KhoaHoc')
  AND d.referenced_minor_name IN (N'NgayBatDau', N'NgayKetThuc');

PRINT N'';

/* -------------------------------------------------------------------------------------
   3. XOÁ CỘT
   ------------------------------------------------------------------------------------- */
BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.KhoaHoc', N'U') IS NULL
    BEGIN
        THROW 50040, N'Không tìm thấy bảng dbo.KhoaHoc. Kiểm tra database đang trỏ tới.', 1;
    END

    -- Cột có DEFAULT constraint thì phải bỏ constraint trước khi DROP COLUMN.
    DECLARE @tenDefault sysname;

    SELECT @tenDefault = dc.name
    FROM sys.default_constraints dc
    JOIN sys.columns c
         ON c.object_id = dc.parent_object_id
        AND c.column_id = dc.parent_column_id
    WHERE dc.parent_object_id = OBJECT_ID(N'dbo.KhoaHoc')
      AND c.name IN (N'NgayBatDau', N'NgayKetThuc');

    IF @tenDefault IS NOT NULL
    BEGIN
        EXEC (N'ALTER TABLE dbo.KhoaHoc DROP CONSTRAINT ' + @tenDefault);
        PRINT N'  - Đã bỏ DEFAULT constraint ' + @tenDefault;
    END

    IF COL_LENGTH('dbo.KhoaHoc', 'NgayBatDau') IS NOT NULL
    BEGIN
        ALTER TABLE dbo.KhoaHoc DROP COLUMN NgayBatDau;
        PRINT N'  - Đã bỏ cột NgayBatDau';
    END
    ELSE
        PRINT N'  = NgayBatDau đã không còn, bỏ qua';

    IF COL_LENGTH('dbo.KhoaHoc', 'NgayKetThuc') IS NOT NULL
    BEGIN
        ALTER TABLE dbo.KhoaHoc DROP COLUMN NgayKetThuc;
        PRINT N'  - Đã bỏ cột NgayKetThuc';
    END
    ELSE
        PRINT N'  = NgayKetThuc đã không còn, bỏ qua';

    COMMIT TRANSACTION;

    PRINT N'  ✔ ĐÃ CẬP NHẬT SCHEMA KhoaHoc.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT N'  ✘ LỖI: ' + ERROR_MESSAGE() + N' (dòng ' + CAST(ERROR_LINE() AS nvarchar(10)) + N')';
    THROW;
END CATCH;
GO

/* -------------------------------------------------------------------------------------
   4. KIỂM TRA — không còn cột ngày nào
   ------------------------------------------------------------------------------------- */
PRINT N'--- 3. CỘT HIỆN CÓ CỦA dbo.KhoaHoc ---';

SELECT c.name AS Cot,
       t.name AS KieuDuLieu,
       CASE WHEN c.max_length = -1 THEN N'MAX'
            ELSE CAST(c.max_length AS nvarchar(10)) END AS DoDai,
       c.is_nullable AS ChoPhepNull
FROM sys.columns c
JOIN sys.types t ON t.user_type_id = c.user_type_id
WHERE c.object_id = OBJECT_ID(N'dbo.KhoaHoc')
ORDER BY c.column_id;

PRINT N'=== HOÀN TẤT v37 ===';
GO
