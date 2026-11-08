/* =====================================================================================
   HocPhi_AnPhieu_v40.sql
   -------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Thêm cột dbo.HocPhi.DaAn — cho phép ẨN một phiếu học phí khỏi sổ học phí
       mà KHÔNG xoá dữ liệu tài chính.

   VÌ SAO KHÔNG XOÁ THẲNG
       Phiếu học phí gắn với giao dịch thanh toán (ThanhToan) và biên nhận (BienNhan).
       Xoá phiếu sẽ làm mất dấu vết thu tiền. Ẩn bằng cờ là cách an toàn:
         • Phiếu ẩn KHÔNG hiện trong danh sách mặc định.
         • Phiếu ẩn KHÔNG được tính vào 4 thẻ tổng (phải thu / đã thu / công nợ / tỷ lệ).
         • Bật "Hiện phiếu đã ẩn" để xem lại và bỏ ẩn bất cứ lúc nào.

   AN TOÀN
       • Kiểm tra COL_LENGTH → chạy nhiều lần vô hại.
       • Chỉ THÊM cột có DEFAULT 0, không đổi cột đã có, không xoá dữ liệu.
   ===================================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

PRINT N'=====================================================================';
PRINT N'  v40 — THÊM CỘT HocPhi.DaAn (ẨN PHIẾU HỌC PHÍ)';
PRINT N'=====================================================================';

BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.HocPhi', N'U') IS NULL
    BEGIN
        THROW 50040, N'Không tìm thấy bảng dbo.HocPhi. Kiểm tra database đang trỏ tới.', 1;
    END

    IF COL_LENGTH('dbo.HocPhi', 'DaAn') IS NULL
    BEGIN
        ALTER TABLE dbo.HocPhi
            ADD DaAn BIT NOT NULL
                CONSTRAINT DF_HocPhi_DaAn DEFAULT (0);

        PRINT N'  + Thêm cột HocPhi.DaAn (BIT, mặc định 0)';
    END
    ELSE
    BEGIN
        PRINT N'  = Cột HocPhi.DaAn đã có — bỏ qua';
    END

    COMMIT TRANSACTION;
    PRINT N'---------------------------------------------------------------------';
    PRINT N'  HOÀN TẤT: HocPhi đã có cột DaAn.';
    PRINT N'---------------------------------------------------------------------';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
    BEGIN
        ROLLBACK TRANSACTION;
    END

    PRINT N'*** LỖI: ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO

-- =====================================================================================
-- KIỂM TRA SAU KHI CHẠY (chỉ đọc)
-- =====================================================================================
PRINT N'';
PRINT N'--- Phiếu học phí theo trạng thái ẩn ---';

SELECT
    DaAn,
    COUNT(*) AS SoPhieu,
    SUM(SoTienPhaiDong) AS TongPhaiDong,
    SUM(SoTienDaDong)   AS TongDaThu
FROM dbo.HocPhi
GROUP BY DaAn;
GO
