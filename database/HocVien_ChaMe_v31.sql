/* =====================================================================================
   HocVien_ChaMe_v31.sql
   -------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Bổ sung HỌ TÊN CHA và HỌ TÊN MẸ cho học viên.

       Trước: chỉ có bảng PhuHuynh (1 phụ huynh / 1 học viên) + HocVien.MoiQuanHePhuHuynh
              → không lưu được CẢ cha và mẹ cùng lúc.
       Sau:   thêm 2 cột mô tả trực tiếp trên hồ sơ học viên.

   THAY ĐỔI SCHEMA
       1. ALTER TABLE HocVien ADD HoTenCha nvarchar(150) NULL
       2. ALTER TABLE HocVien ADD HoTenMe  nvarchar(150) NULL

   KHÔNG ĐỘNG TỚI
       • Bảng PhuHuynh, MaPhuHuynh, MoiQuanHePhuHuynh — giữ nguyên.
       • Mọi dữ liệu học viên hiện có.

   AN TOÀN
       • Idempotent: chạy lại nhiều lần vẫn cho cùng kết quả.
       • Không xoá / không sửa dữ liệu.
       • Mỗi cột ở một batch riêng? KHÔNG cần — script này không SELECT hai cột mới
         trong cùng batch với ALTER, nên không gặp lỗi Msg 207.
   ===================================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

PRINT N'=== v31 — THÊM HỌ TÊN CHA / MẸ CHO HỌC VIÊN ===';

IF OBJECT_ID(N'dbo.HocVien', N'U') IS NULL
BEGIN
    RAISERROR(N'Không tìm thấy bảng dbo.HocVien. Kiểm tra database đang trỏ tới.', 16, 1);
END
GO

/* =====================================================================================
   1. THÊM CỘT (mỗi ALTER là một batch riêng cho an toàn)
   ===================================================================================== */
IF COL_LENGTH(N'dbo.HocVien', N'HoTenCha') IS NULL
BEGIN
    ALTER TABLE dbo.HocVien ADD HoTenCha nvarchar(150) NULL;
    PRINT N'  • Đã thêm cột HoTenCha.';
END
ELSE
BEGIN
    PRINT N'  • Cột HoTenCha đã có — bỏ qua.';
END
GO

IF COL_LENGTH(N'dbo.HocVien', N'HoTenMe') IS NULL
BEGIN
    ALTER TABLE dbo.HocVien ADD HoTenMe nvarchar(150) NULL;
    PRINT N'  • Đã thêm cột HoTenMe.';
END
ELSE
BEGIN
    PRINT N'  • Cột HoTenMe đã có — bỏ qua.';
END
GO

/* =====================================================================================
   2. KIỂM TRA
   ===================================================================================== */
PRINT N'--- KIỂM TRA ---';

SELECT c.name AS Cot,
       t.name AS KieuDuLieu,
       c.max_length / 2 AS DoDai,
       c.is_nullable AS ChoPhepNull
FROM sys.columns c
JOIN sys.types t ON t.user_type_id = c.user_type_id
WHERE c.object_id = OBJECT_ID(N'dbo.HocVien')
  AND c.name IN (N'HoTenCha', N'HoTenMe')
ORDER BY c.name;

SELECT TOP 5 MaHocVien, HoTen, HoTenCha, HoTenMe
FROM dbo.HocVien
ORDER BY MaHocVien DESC;

PRINT N'=== HOÀN TẤT v31 ===';
GO
