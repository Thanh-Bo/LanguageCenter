/* =====================================================================================
   NhanVien_HoSo_v33.sql
   -------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Bổ sung các cột HỒ SƠ cho bảng dbo.NhanVien để màn hình WinForms
       "Quản lý nhân viên học vụ" và "Quản lý kế toán" lưu được đầy đủ thông tin
       (thêm / sửa nhân viên).

   VÌ SAO CẦN
       Entity LanguageCenter.Domain.Entities.NhanVien có các thuộc tính NgaySinh,
       GioiTinh, SoDienThoai, DiaChi, NgayVaoLam. Nếu database chưa có các cột này thì
       mọi truy vấn SELECT/INSERT đụng tới chúng sẽ lỗi
       "Invalid column name ..." ngay khi mở 2 tab nhân viên.

   AN TOÀN
       • Mỗi cột được kiểm tra bằng COL_LENGTH → chạy nhiều lần vô hại.
       • KHÔNG xoá, KHÔNG đổi kiểu cột đã có; chỉ THÊM cột còn thiếu (NULL được phép).
       • Chạy bằng tài khoản có quyền ALTER trên database QuanLyTrungTamNgoaiNgu.
   ===================================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

PRINT N'=====================================================================';
PRINT N'  v33 — BỔ SUNG CỘT HỒ SƠ CHO BẢNG NhanVien';
PRINT N'=====================================================================';

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.NhanVien', N'U') IS NULL
    BEGIN
        THROW 50010, N'Không tìm thấy bảng dbo.NhanVien. Kiểm tra database đang trỏ tới.', 1;
    END

    IF COL_LENGTH('dbo.NhanVien', 'NgaySinh') IS NULL
    BEGIN
        ALTER TABLE dbo.NhanVien ADD NgaySinh date NULL;
        PRINT N'  + Thêm cột NgaySinh';
    END
    ELSE
        PRINT N'  = NgaySinh đã có, bỏ qua';

    IF COL_LENGTH('dbo.NhanVien', 'GioiTinh') IS NULL
    BEGIN
        ALTER TABLE dbo.NhanVien ADD GioiTinh nvarchar(20) NULL;
        PRINT N'  + Thêm cột GioiTinh';
    END
    ELSE
        PRINT N'  = GioiTinh đã có, bỏ qua';

    IF COL_LENGTH('dbo.NhanVien', 'SoDienThoai') IS NULL
    BEGIN
        ALTER TABLE dbo.NhanVien ADD SoDienThoai varchar(20) NULL;
        PRINT N'  + Thêm cột SoDienThoai';
    END
    ELSE
        PRINT N'  = SoDienThoai đã có, bỏ qua';

    IF COL_LENGTH('dbo.NhanVien', 'DiaChi') IS NULL
    BEGIN
        ALTER TABLE dbo.NhanVien ADD DiaChi nvarchar(500) NULL;
        PRINT N'  + Thêm cột DiaChi';
    END
    ELSE
        PRINT N'  = DiaChi đã có, bỏ qua';

    IF COL_LENGTH('dbo.NhanVien', 'NgayVaoLam') IS NULL
    BEGIN
        ALTER TABLE dbo.NhanVien ADD NgayVaoLam date NULL;
        PRINT N'  + Thêm cột NgayVaoLam';
    END
    ELSE
        PRINT N'  = NgayVaoLam đã có, bỏ qua';

    IF COL_LENGTH('dbo.NhanVien', 'Email') IS NULL
    BEGIN
        ALTER TABLE dbo.NhanVien ADD Email varchar(255) NULL;
        PRINT N'  + Thêm cột Email';
    END
    ELSE
        PRINT N'  = Email đã có, bỏ qua';

    COMMIT TRANSACTION;

    PRINT N'  ✔ ĐÃ CẬP NHẬT SCHEMA NhanVien.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT N'  ✘ LỖI: ' + ERROR_MESSAGE() + N' (dòng ' + CAST(ERROR_LINE() AS nvarchar(10)) + N')';
    THROW;
END CATCH;
GO

/* -------------------------------------------------------------------------------------
   KIỂM TRA
   ------------------------------------------------------------------------------------- */
PRINT N'--- CỘT HIỆN CÓ CỦA dbo.NhanVien ---';

SELECT c.name AS Cot,
       t.name AS KieuDuLieu,
       CASE WHEN c.max_length = -1 THEN N'MAX'
            ELSE CAST(c.max_length AS nvarchar(10)) END AS DoDai,
       c.is_nullable AS ChoPhepNull
FROM sys.columns c
JOIN sys.types t ON t.user_type_id = c.user_type_id
WHERE c.object_id = OBJECT_ID(N'dbo.NhanVien')
ORDER BY c.column_id;

PRINT N'=== HOÀN TẤT v33 ===';
GO
