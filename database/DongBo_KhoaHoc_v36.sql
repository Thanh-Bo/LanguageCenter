/* =====================================================================================
   DongBo_KhoaHoc_v36.sql
   -------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Chuẩn hoá lại dữ liệu KHÓA HỌC theo quy tắc mới:
         • Tổng số giờ  = Số buổi × 2          (mỗi buổi 2 tiếng, cố định)
         • Ngày kết thúc (DỰ KIẾN) = Ngày bắt đầu + SỐ TUẦN × 7 ngày
           trong đó SỐ TUẦN = Số buổi ÷ 2, LÀM TRÒN LÊN (25 buổi → 13 tuần)

   VÌ SAO
       WinForms đã chuyển 2 trường này thành CHỈ XEM / TỰ ĐỘNG TÍNH, nhưng dữ liệu CŨ
       vẫn còn số nhập tay (ví dụ 24 buổi nhưng 50 giờ thay vì 48). Script này đồng bộ
       toàn bộ một lần.

   LƯU Ý
       • Ngày kết thúc chỉ là DỰ KIẾN — lịch thực tế có thể lùi vì ngày lễ.
       • Ngày chính xác chỉ có khi LỚP đã có lịch tuần và đã sinh ca học.
       • Chạy nhiều lần vô hại (kết quả luôn như nhau).
   ===================================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

PRINT N'=====================================================================';
PRINT N'  v36 — ĐỒNG BỘ Số buổi / Tổng số giờ / Ngày kết thúc của KHÓA HỌC';
PRINT N'=====================================================================';

/* -------------------------------------------------------------------------------------
   1. XEM TRƯỚC: những khóa học sẽ bị thay đổi
   ------------------------------------------------------------------------------------- */
PRINT N'--- 1. KHÓA HỌC SẼ ĐƯỢC CẬP NHẬT ---';

SELECT MaKhoaHoc,
       TenKhoaHoc,
       SoBuoi,
       TongSoGio                                   AS GioHienTai,
       SoBuoi * 2                                  AS GioDung,
       NgayBatDau,
       NgayKetThuc                                 AS KetThucHienTai,
       DATEADD(day, CAST(CEILING(SoBuoi / 2.0) AS int) * 7, CAST(NgayBatDau AS date))
                                                   AS KetThucDung,
       CAST(CEILING(SoBuoi / 2.0) AS int)          AS SoTuan
FROM dbo.KhoaHoc
WHERE ISNULL(TongSoGio, -1) <> SoBuoi * 2
   OR CAST(NgayKetThuc AS date) <>
      DATEADD(day, CAST(CEILING(SoBuoi / 2.0) AS int) * 7, CAST(NgayBatDau AS date))
ORDER BY MaKhoaHoc;

PRINT N'  (0 dòng = dữ liệu đã đúng, không cần cập nhật)';
PRINT N'';

/* -------------------------------------------------------------------------------------
   2. CẬP NHẬT
   ------------------------------------------------------------------------------------- */
BEGIN TRY
    BEGIN TRANSACTION;

    IF OBJECT_ID(N'dbo.KhoaHoc', N'U') IS NULL
    BEGIN
        THROW 50030, N'Không tìm thấy bảng dbo.KhoaHoc. Kiểm tra database đang trỏ tới.', 1;
    END

    DECLARE @soDong int = 0;

    UPDATE dbo.KhoaHoc
    SET TongSoGio   = SoBuoi * 2,
        NgayKetThuc = DATEADD(day, CAST(CEILING(SoBuoi / 2.0) AS int) * 7, CAST(NgayBatDau AS date))
    WHERE ISNULL(TongSoGio, -1) <> SoBuoi * 2
       OR CAST(NgayKetThuc AS date) <>
          DATEADD(day, CAST(CEILING(SoBuoi / 2.0) AS int) * 7, CAST(NgayBatDau AS date));

    SET @soDong = @@ROWCOUNT;

    COMMIT TRANSACTION;

    PRINT N'  ✔ Đã cập nhật ' + CAST(@soDong AS nvarchar(10)) + N' khóa học.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT N'  ✘ LỖI: ' + ERROR_MESSAGE() + N' (dòng ' + CAST(ERROR_LINE() AS nvarchar(10)) + N')';
    THROW;
END CATCH;
GO

/* -------------------------------------------------------------------------------------
   3. KIỂM TRA SAU KHI CHẠY — phải ra 0 dòng
   ------------------------------------------------------------------------------------- */
PRINT N'--- 3. KHÓA HỌC CÒN SAI QUY TẮC ---';

SELECT MaKhoaHoc,
       TenKhoaHoc,
       SoBuoi,
       TongSoGio,
       NgayBatDau,
       NgayKetThuc
FROM dbo.KhoaHoc
WHERE ISNULL(TongSoGio, -1) <> SoBuoi * 2
   OR CAST(NgayKetThuc AS date) <>
      DATEADD(day, CAST(CEILING(SoBuoi / 2.0) AS int) * 7, CAST(NgayBatDau AS date));

PRINT N'=== HOÀN TẤT v36 ===';
GO
