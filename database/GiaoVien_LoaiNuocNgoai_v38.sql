/* =====================================================================================
   GiaoVien_LoaiNuocNgoai_v38.sql
   -------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Mở rộng CHECK CONSTRAINT dbo.CK_GiaoVien_Loai để cho phép thêm loại hồ sơ
       "Giáo viên nước ngoài" (bên cạnh "Giáo viên" và "Trợ giảng").

   BỐI CẢNH
       WinForms có thêm tab "Quản lý giáo viên nước ngoài". Giáo viên nước ngoài vẫn
       mang VAI TRÒ "Giáo viên" (dạy được, nhận phân công giảng dạy) — chỉ khác
       LoaiGiaoVien để tách riêng màn hình quản lý.

   AN TOÀN
       • Chạy nhiều lần vô hại: chỉ DROP + ADD lại constraint.
       • Không sửa dữ liệu hiện có.
       • Kiểm tra lại dữ liệu cũ có vi phạm không TRƯỚC khi thêm constraint.
   ===================================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

PRINT N'=====================================================================';
PRINT N'  v38 — CHO PHÉP LoaiGiaoVien = N''Giáo viên nước ngoài''';
PRINT N'=====================================================================';

/* -------------------------------------------------------------------------------------
   1. DỮ LIỆU CŨ — giá trị LoaiGiaoVien đang tồn tại
   ------------------------------------------------------------------------------------- */
PRINT N'--- 1. GIÁ TRỊ LoaiGiaoVien ĐANG CÓ ---';

SELECT LoaiGiaoVien, COUNT(*) AS SoHoSo
FROM dbo.GiaoVien
GROUP BY LoaiGiaoVien
ORDER BY LoaiGiaoVien;

PRINT N'';

/* -------------------------------------------------------------------------------------
   2. GIÁ TRỊ LẠ — phải RỖNG, nếu có thì thêm constraint sẽ thất bại
   ------------------------------------------------------------------------------------- */
PRINT N'--- 2. GIÁ TRỊ NGOÀI DANH SÁCH CHO PHÉP (kỳ vọng: 0 dòng) ---';

SELECT MaGiaoVien, HoTen, LoaiGiaoVien
FROM dbo.GiaoVien
WHERE LoaiGiaoVien NOT IN (N'Giáo viên', N'Giáo viên nước ngoài', N'Trợ giảng');

PRINT N'';

/* -------------------------------------------------------------------------------------
   3. THAY CONSTRAINT
   ------------------------------------------------------------------------------------- */
BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.GiaoVien', N'U') IS NULL
    BEGIN
        THROW 50050, N'Không tìm thấy bảng dbo.GiaoVien. Kiểm tra database đang trỏ tới.', 1;
    END

    IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_GiaoVien_Loai')
    BEGIN
        ALTER TABLE dbo.GiaoVien DROP CONSTRAINT CK_GiaoVien_Loai;
        PRINT N'  - Đã bỏ CK_GiaoVien_Loai (bản cũ)';
    END

    ALTER TABLE dbo.GiaoVien WITH CHECK
        ADD CONSTRAINT CK_GiaoVien_Loai
        CHECK (LoaiGiaoVien IN (N'Giáo viên', N'Giáo viên nước ngoài', N'Trợ giảng'));

    ALTER TABLE dbo.GiaoVien CHECK CONSTRAINT CK_GiaoVien_Loai;

    PRINT N'  + Đã thêm CK_GiaoVien_Loai (3 giá trị)';

    COMMIT TRANSACTION;

    PRINT N'  ✔ ĐÃ CẬP NHẬT SCHEMA GiaoVien.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT N'  ✘ LỖI: ' + ERROR_MESSAGE() + N' (dòng ' + CAST(ERROR_LINE() AS nvarchar(10)) + N')';
    THROW;
END CATCH;
GO

/* -------------------------------------------------------------------------------------
   4. KIỂM TRA
   ------------------------------------------------------------------------------------- */
PRINT N'--- 3. ĐỊNH NGHĨA CK_GiaoVien_Loai HIỆN TẠI ---';

SELECT name        AS TenConstraint,
       definition  AS DinhNghia,
       is_disabled AS DangTat
FROM sys.check_constraints
WHERE parent_object_id = OBJECT_ID(N'dbo.GiaoVien');

PRINT N'=== HOÀN TẤT v38 ===';
GO
