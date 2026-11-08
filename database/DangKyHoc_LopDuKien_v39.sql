/* =====================================================================================
   DangKyHoc_LopDuKien_v39.sql
   -------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Thêm cột dbo.DangKyHoc.MaLopDuKien — "LỚP DỰ KIẾN" mà học viên sẽ vào học.

   VÌ SAO CẦN
       Luồng nghiệp vụ mới: thu học phí TRƯỚC → hệ thống TỰ ĐỘNG xếp lớp ngay khi thu đủ.
       Muốn tự xếp thì đăng ký phải nhớ lớp đích. Không thể dùng MaLopHoc vì
       MaLopHoc có giá trị = "đã ở trong lớp"; còn giai đoạn chờ thu tiền thì chưa vào lớp.

       • MaLopDuKien : lớp sẽ vào khi thanh toán đủ (admin chọn lúc tạo đăng ký)
       • MaLopHoc    : lớp THỰC TẾ đã vào (chỉ được gán khi tự xếp / xếp tay)

       Cách dùng:
         - Tạo đăng ký  → BẮT BUỘC có MaLopDuKien, kiểm tra 4 quy tắc xếp lớp.
         - Thu đủ tiền  → kiểm tra lại 4 quy tắc → MaLopHoc = MaLopDuKien,
                          TrangThaiDangKy = 'Đã xếp lớp'.
         - Đổi lớp dự kiến khi đã thu đủ → tự xếp lại vào lớp mới.

   AN TOÀN
       • Kiểm tra COL_LENGTH / sys.foreign_keys → chạy nhiều lần vô hại.
       • Chỉ THÊM cột NULL + FK, không đổi cột đã có, không xoá dữ liệu.
       • Không backfill: đăng ký cũ đang chờ xử lý sẽ không có lớp dự kiến —
         admin chọn lại lớp khi cần (xem truy vấn kiểm tra ở cuối file).
   ===================================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

PRINT N'=====================================================================';
PRINT N'  v39 — THÊM CỘT DangKyHoc.MaLopDuKien (LỚP DỰ KIẾN)';
PRINT N'=====================================================================';

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.DangKyHoc', N'U') IS NULL
    BEGIN
        THROW 50030, N'Không tìm thấy bảng dbo.DangKyHoc. Kiểm tra database đang trỏ tới.', 1;
    END

    IF OBJECT_ID(N'dbo.LopHoc', N'U') IS NULL
    BEGIN
        THROW 50031, N'Không tìm thấy bảng dbo.LopHoc. Kiểm tra database đang trỏ tới.', 1;
    END

    -- ------------------------------------------------------------------------------
    -- 1. Cột MaLopDuKien
    -- ------------------------------------------------------------------------------
    IF COL_LENGTH('dbo.DangKyHoc', 'MaLopDuKien') IS NULL
    BEGIN
        ALTER TABLE dbo.DangKyHoc ADD MaLopDuKien INT NULL;
        PRINT N'  + Thêm cột DangKyHoc.MaLopDuKien';
    END
    ELSE
    BEGIN
        PRINT N'  = Cột DangKyHoc.MaLopDuKien đã có — bỏ qua';
    END

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
    BEGIN
        ROLLBACK TRANSACTION;
    END

    PRINT N'*** LỖI (cột): ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO

-- =====================================================================================
-- BATCH 2 — KHOÁ NGOẠI + CHỈ MỤC
--   Phải là batch RIÊNG: SQL Server biên dịch cả batch trước khi chạy, nên câu lệnh
--   dùng cột vừa ALTER ở batch 1 trong CÙNG batch sẽ báo "Msg 207 Invalid column name".
-- =====================================================================================
BEGIN TRY
    BEGIN TRANSACTION;

    -- ------------------------------------------------------------------------------
    -- 2. Khoá ngoại tới LopHoc (đặt tên riêng, KHÔNG đụng FK_DangKyHoc_LopHoc)
    -- ------------------------------------------------------------------------------
    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.foreign_keys
        WHERE name = N'FK_DangKyHoc_LopDuKien'
          AND parent_object_id = OBJECT_ID(N'dbo.DangKyHoc')
    )
    BEGIN
        ALTER TABLE dbo.DangKyHoc WITH CHECK
            ADD CONSTRAINT FK_DangKyHoc_LopDuKien
                FOREIGN KEY (MaLopDuKien) REFERENCES dbo.LopHoc (MaLopHoc);

        PRINT N'  + Thêm khoá ngoại FK_DangKyHoc_LopDuKien → LopHoc(MaLopHoc)';
    END
    ELSE
    BEGIN
        PRINT N'  = Khoá ngoại FK_DangKyHoc_LopDuKien đã có — bỏ qua';
    END

    -- ------------------------------------------------------------------------------
    -- 3. Chỉ mục: tra "lớp này đang có bao nhiêu đăng ký giữ chỗ" (sĩ số dự kiến)
    -- ------------------------------------------------------------------------------
    IF NOT EXISTS
    (
        SELECT 1
        FROM sys.indexes
        WHERE name = N'IX_DangKyHoc_MaLopDuKien'
          AND object_id = OBJECT_ID(N'dbo.DangKyHoc')
    )
    BEGIN
        CREATE INDEX IX_DangKyHoc_MaLopDuKien
            ON dbo.DangKyHoc (MaLopDuKien)
            INCLUDE (MaKhoaHoc, TrangThaiDangKy);

        PRINT N'  + Tạo chỉ mục IX_DangKyHoc_MaLopDuKien';
    END
    ELSE
    BEGIN
        PRINT N'  = Chỉ mục IX_DangKyHoc_MaLopDuKien đã có — bỏ qua';
    END

    COMMIT TRANSACTION;
    PRINT N'---------------------------------------------------------------------';
    PRINT N'  HOÀN TẤT: DangKyHoc đã có cột MaLopDuKien.';
    PRINT N'---------------------------------------------------------------------';
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
-- KIỂM TRA SAU KHI CHẠY (chỉ đọc)
-- =====================================================================================
PRINT N'';
PRINT N'--- Đăng ký đang chờ xử lý (chưa có lớp dự kiến → cần chọn lại lớp) ---';

SELECT
    dk.MaDangKy,
    dk.MaHocVien,
    dk.MaKhoaHoc,
    dk.MaLopDuKien,
    dk.MaLopHoc,
    dk.TrangThaiDangKy
FROM dbo.DangKyHoc AS dk
WHERE dk.TrangThaiDangKy = N'Chờ xử lý'
ORDER BY dk.MaDangKy;
GO
