/* ============================================================================================
   ChotBuoiDaQua_v19.sql
   --------------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Chốt trạng thái ca học theo NGÀY, đúng một quy tắc duy nhất:

           Ngày ĐÃ QUA      ->  N'Đã hoàn thành'
           Ngày CHƯA TỚI    ->  N'Chưa diễn ra'
           ĐÚNG HÔM NAY     ->  giữ nguyên

   KHÔNG ĐỤNG TỚI
       - Ca N'Đã hủy' (luôn giữ nguyên, dù ngày đã qua hay chưa).
       - Ca N'Đã hoàn thành' ở tương lai (giữ nguyên — có thể do nhập tay).

   ĐẶC ĐIỂM
       - Idempotent: chạy lại nhiều lần vẫn cho cùng kết quả.
       - Có transaction + in ra số dòng thay đổi trước/sau.

   CÁCH CHẠY
       Chạy toàn bộ file bằng SSMS. Muốn giới hạn 1 lớp thì đặt @MaLopHoc = <id>.

   LIÊN QUAN
       - API tương đương: POST /api/admin/sessions/settle  body { "maLopHoc": null }
       - Nút trên Web: Lịch & ca học -> "Chốt buổi đã qua"
       - Ngoài ra: lưu điểm danh cũng tự chốt ca đó sang "Đã hoàn thành".
   ============================================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

/* --------------------------------------------------------------------------------------------
   THAM SỐ
   -------------------------------------------------------------------------------------------- */
DECLARE @HomNay   date = CAST(GETDATE() AS date);
DECLARE @MaLopHoc int  = NULL;   -- NULL = toàn bộ các lớp; ví dụ: 1

/* --------------------------------------------------------------------------------------------
   TRƯỚC KHI CHỐT — xem hiện trạng
   -------------------------------------------------------------------------------------------- */
PRINT N'=== TRƯỚC KHI CHỐT (hôm nay: ' + CONVERT(nvarchar(10), @HomNay, 103) + N') ===';

SELECT
    ch.TrangThai                AS TrangThaiHienTai,
    CASE WHEN ch.NgayHoc <  @HomNay THEN N'Đã qua'
         WHEN ch.NgayHoc =  @HomNay THEN N'Hôm nay'
         ELSE N'Chưa tới' END   AS ViTriNgay,
    COUNT(*)                    AS SoCa
FROM dbo.CaHoc AS ch
WHERE @MaLopHoc IS NULL OR ch.MaLopHoc = @MaLopHoc
GROUP BY
    ch.TrangThai,
    CASE WHEN ch.NgayHoc <  @HomNay THEN N'Đã qua'
         WHEN ch.NgayHoc =  @HomNay THEN N'Hôm nay'
         ELSE N'Chưa tới' END
ORDER BY ViTriNgay, TrangThaiHienTai;

/* --------------------------------------------------------------------------------------------
   CHỐT
   -------------------------------------------------------------------------------------------- */
DECLARE @SoCaDaChot int = 0;
DECLARE @SoCaMoLai  int = 0;

BEGIN TRY
    BEGIN TRANSACTION;

    -- (1) Ngày đã qua: Chưa diễn ra / Đang diễn ra  ->  Đã hoàn thành
    UPDATE ch
    SET TrangThai = N'Đã hoàn thành'
    FROM dbo.CaHoc AS ch
    WHERE ch.NgayHoc < @HomNay
      AND ch.TrangThai IN (N'Chưa diễn ra', N'Đang diễn ra')
      AND (@MaLopHoc IS NULL OR ch.MaLopHoc = @MaLopHoc);

    SET @SoCaDaChot = @@ROWCOUNT;

    -- (2) Ngày còn ở tương lai: Đang diễn ra  ->  Chưa diễn ra
    UPDATE ch
    SET TrangThai = N'Chưa diễn ra'
    FROM dbo.CaHoc AS ch
    WHERE ch.NgayHoc > @HomNay
      AND ch.TrangThai = N'Đang diễn ra'
      AND (@MaLopHoc IS NULL OR ch.MaLopHoc = @MaLopHoc);

    SET @SoCaMoLai = @@ROWCOUNT;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    DECLARE @ThongDiep nvarchar(2000) =
        N'LỖI: ' + ERROR_MESSAGE()
        + N' (dòng ' + CAST(ERROR_LINE() AS nvarchar(10)) + N') — đã ROLLBACK.';

    RAISERROR(@ThongDiep, 16, 1);
    RETURN;
END CATCH;

/* --------------------------------------------------------------------------------------------
   SAU KHI CHỐT — kết quả
   -------------------------------------------------------------------------------------------- */
PRINT N'';
PRINT N'=== KẾT QUẢ ===';
PRINT N'Số ca chuyển sang "Đã hoàn thành": ' + CAST(@SoCaDaChot AS nvarchar(10));
PRINT N'Số ca mở lại "Chưa diễn ra":       ' + CAST(@SoCaMoLai AS nvarchar(10));
PRINT N'';

SELECT
    ch.TrangThai                AS TrangThaiHienTai,
    CASE WHEN ch.NgayHoc <  @HomNay THEN N'Đã qua'
         WHEN ch.NgayHoc =  @HomNay THEN N'Hôm nay'
         ELSE N'Chưa tới' END   AS ViTriNgay,
    COUNT(*)                    AS SoCa
FROM dbo.CaHoc AS ch
WHERE @MaLopHoc IS NULL OR ch.MaLopHoc = @MaLopHoc
GROUP BY
    ch.TrangThai,
    CASE WHEN ch.NgayHoc <  @HomNay THEN N'Đã qua'
         WHEN ch.NgayHoc =  @HomNay THEN N'Hôm nay'
         ELSE N'Chưa tới' END
ORDER BY ViTriNgay, TrangThaiHienTai;

-- Tổng quan theo lớp để dễ đối chiếu trên giao diện
SELECT
    l.MaLopHoc,
    l.MaLop,
    l.TenLop,
    COUNT(*)                                                                AS TongSoCa,
    SUM(CASE WHEN ch.TrangThai = N'Đã hoàn thành' THEN 1 ELSE 0 END)         AS DaHoanThanh,
    SUM(CASE WHEN ch.TrangThai = N'Chưa diễn ra'  THEN 1 ELSE 0 END)         AS ChuaDienRa,
    SUM(CASE WHEN ch.TrangThai = N'Đang diễn ra'  THEN 1 ELSE 0 END)         AS DangDienRa,
    SUM(CASE WHEN ch.TrangThai = N'Đã hủy'        THEN 1 ELSE 0 END)         AS DaHuy
FROM dbo.CaHoc AS ch
JOIN dbo.LopHoc AS l ON l.MaLopHoc = ch.MaLopHoc
WHERE @MaLopHoc IS NULL OR ch.MaLopHoc = @MaLopHoc
GROUP BY l.MaLopHoc, l.MaLop, l.TenLop
ORDER BY l.MaLopHoc;

PRINT N'Xong. Chạy lại file này bất cứ lúc nào để cập nhật theo ngày mới.';
