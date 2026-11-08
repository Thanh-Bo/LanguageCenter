/* =====================================================================================
   GiaoVien_BangCap_v35.sql
   -------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Thêm cột dbo.GiaoVien.GhiChuBangCap để màn hình "Sửa hồ sơ giáo viên / trợ giảng"
       lưu được phần BẰNG CẤP & CHỨNG CHỈ dạng VĂN BẢN TỰ DO (gõ tay, không đính kèm file).

   VÌ SAO KHÔNG DÙNG BẢNG BangCap / ChungChi
       Hai bảng đó có cấu trúc chi tiết (tên bằng, chuyên ngành, cơ sở đào tạo, số bằng,
       ngày cấp…) nhưng Phase 3 CHƯA có endpoint ghi. Người dùng chỉ cần một ô ghi chú
       để gõ và sửa nhanh → dùng cột văn bản là đủ; khi nào cần CRUD chi tiết thì
       chuyển dữ liệu sang bảng BangCap / ChungChi sau.

   AN TOÀN
       • Kiểm tra COL_LENGTH → chạy nhiều lần vô hại.
       • Chỉ THÊM cột còn thiếu, NULL được phép, không đổi cột đã có.
   ===================================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

PRINT N'=====================================================================';
PRINT N'  v35 — THÊM CỘT GhiChuBangCap CHO BẢNG GiaoVien';
PRINT N'=====================================================================';

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.GiaoVien', N'U') IS NULL
    BEGIN
        THROW 50020, N'Không tìm thấy bảng dbo.GiaoVien. Kiểm tra database đang trỏ tới.', 1;
    END

    IF COL_LENGTH('dbo.GiaoVien', 'GhiChuBangCap') IS NULL
    BEGIN
        ALTER TABLE dbo.GiaoVien ADD GhiChuBangCap nvarchar(1000) NULL;
        PRINT N'  + Thêm cột GhiChuBangCap';
    END
    ELSE
        PRINT N'  = GhiChuBangCap đã có, bỏ qua';

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
   KIỂM TRA
   ------------------------------------------------------------------------------------- */
PRINT N'--- CỘT HIỆN CÓ CỦA dbo.GiaoVien ---';

SELECT c.name AS Cot,
       t.name AS KieuDuLieu,
       CASE WHEN c.max_length = -1 THEN N'MAX'
            ELSE CAST(c.max_length AS nvarchar(10)) END AS DoDai,
       c.is_nullable AS ChoPhepNull
FROM sys.columns c
JOIN sys.types t ON t.user_type_id = c.user_type_id
WHERE c.object_id = OBJECT_ID(N'dbo.GiaoVien')
ORDER BY c.column_id;

PRINT N'=== HOÀN TẤT v35 ===';
GO
