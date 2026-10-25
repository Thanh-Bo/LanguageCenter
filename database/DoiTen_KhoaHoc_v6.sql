/* =============================================================================
   ĐỔI TÊN KHÓA HỌC v6 — BỎ HẬU TỐ "K01", "K02", … KHỎI TÊN KHÓA HỌC
   -----------------------------------------------------------------------------
   LÝ DO: "K01/K02…" là số ĐỢT KHAI GIẢNG — thuộc về LỚP HỌC (LopHoc),
          không phải thuộc tính của KHÓA HỌC (KhoaHoc).

   Ví dụ:
        IELTS Advanced K01            →  IELTS Advanced
        Teen Starter K01              →  Teen Starter
        Mầm non Preschool 1 K01       →  Mầm non Preschool 1
        English Communication Basic K01 → English Communication Basic

   AN TOÀN:
     · Chỉ đổi TenKhoaHoc. KHÔNG đụng MaKhoaHoc, MaCapDo, học phí, số buổi,
       hay bất kỳ bảng nào khác (lớp học / đăng ký / học phí giữ nguyên liên kết).
     · Chạy lại nhiều lần được (tên đã sạch thì không đổi gì).
     · Bỏ qua dòng nào mà tên mới bị trùng trong CÙNG cấp độ
       (ràng buộc UQ_KhoaHoc_CapDo_Ten).
   ============================================================================= */

USE QuanLyTrungTamNgoaiNgu;
GO

SET NOCOUNT ON;
GO

/* -----------------------------------------------------------------------------
   BƯỚC 1 — TÍNH TÊN MỚI (chỉ những tên kết thúc bằng " K" + chữ số)
   ----------------------------------------------------------------------------- */
IF OBJECT_ID('tempdb..#DoiTen') IS NOT NULL DROP TABLE #DoiTen;

;WITH X AS
(
    SELECT k.MaKhoaHoc,
           k.MaCapDo,
           k.TenKhoaHoc,
           LEN(k.TenKhoaHoc) AS DoDai,
           CASE WHEN PATINDEX('%[^0-9]%', REVERSE(k.TenKhoaHoc)) = 0
                THEN 0
                ELSE PATINDEX('%[^0-9]%', REVERSE(k.TenKhoaHoc)) - 1
           END AS SoChuSoCuoi
      FROM KhoaHoc k
)
SELECT MaKhoaHoc,
       MaCapDo,
       TenKhoaHoc                                   AS TenCu,
       RTRIM(LEFT(TenKhoaHoc, DoDai - SoChuSoCuoi - 2)) AS TenMoi
  INTO #DoiTen
  FROM X
 WHERE SoChuSoCuoi >= 1
   -- ký tự ngay trước phần số phải là 'K'
   AND SUBSTRING(TenKhoaHoc, DoDai - SoChuSoCuoi, 1) = N'K'
   -- và trước 'K' phải là dấu cách
   AND SUBSTRING(TenKhoaHoc, DoDai - SoChuSoCuoi - 1, 1) = N' '
   AND LEN(TenKhoaHoc) - SoChuSoCuoi - 2 > 0;

PRINT N'===== BƯỚC 1: CÁC KHÓA HỌC SẼ ĐƯỢC ĐỔI TÊN =====';

SELECT TenCu, TenMoi
  FROM #DoiTen
 ORDER BY TenMoi;
GO

/* -----------------------------------------------------------------------------
   BƯỚC 2 — KIỂM TRA TRÙNG TÊN TRONG CÙNG CẤP ĐỘ (ràng buộc UNIQUE)
   (rỗng = an toàn, không có xung đột)
   ----------------------------------------------------------------------------- */
PRINT N'===== BƯỚC 2: KIỂM TRA XUNG ĐỘT TÊN (rỗng = an toàn) =====';

SELECT d.TenMoi              AS TenMoi,
       d.MaCapDo             AS MaCapDo,
       k2.TenKhoaHoc         AS TenDaTonTai,
       k2.MaKhoaHoc          AS MaKhoaHocDaTonTai
  FROM #DoiTen d
  JOIN KhoaHoc k2
       ON k2.MaCapDo = d.MaCapDo
      AND k2.TenKhoaHoc = d.TenMoi
      AND k2.MaKhoaHoc <> d.MaKhoaHoc;
GO

/* -----------------------------------------------------------------------------
   BƯỚC 3 — CẬP NHẬT
   ----------------------------------------------------------------------------- */
BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE k
       SET k.TenKhoaHoc = d.TenMoi
      FROM KhoaHoc k
      JOIN #DoiTen d ON d.MaKhoaHoc = k.MaKhoaHoc
     WHERE NOT EXISTS
           (
               SELECT 1
                 FROM KhoaHoc k2
                WHERE k2.MaCapDo = k.MaCapDo
                  AND k2.TenKhoaHoc = d.TenMoi
                  AND k2.MaKhoaHoc <> k.MaKhoaHoc
           );

    PRINT N'Đã đổi tên ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' khóa học.';

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;

    PRINT N'===== LỖI — ĐÃ ROLLBACK =====';
    PRINT ERROR_MESSAGE();

    THROW;
END CATCH
GO

/* -----------------------------------------------------------------------------
   BƯỚC 4 — KIỂM TRA KẾT QUẢ
   ----------------------------------------------------------------------------- */
PRINT N'===== BƯỚC 4: DANH SÁCH KHÓA HỌC SAU KHI ĐỔI =====';

SELECT ct.TenChuongTrinh                AS ChuongTrinh,
       c.TenCapDo                       AS CapDo,
       k.MaKhoaHoc                      AS MaKhoaHoc,
       k.TenKhoaHoc                     AS TenKhoaHoc,
       k.SoBuoi                         AS SoBuoi,
       k.HocPhiCoBan                    AS HocPhi
  FROM KhoaHoc k
  JOIN CapDo     c  ON c.MaCapDo        = k.MaCapDo
  JOIN ChuongTrinh ct ON ct.MaChuongTrinh = c.MaChuongTrinh
 ORDER BY ct.TenChuongTrinh, c.ThuTu;
GO

/* Còn tên nào còn dính " K<so>" không? (rỗng = đã sạch) */
PRINT N'===== CÒN SÓT HẬU TỐ Kxx? (rỗng = đã sạch) =====';

SELECT MaKhoaHoc, TenKhoaHoc
  FROM KhoaHoc
 WHERE TenKhoaHoc LIKE N'% K%[0-9]';
GO

DROP TABLE #DoiTen;
GO

/* =============================================================================
   LƯU Ý
   -----------------------------------------------------------------------------
   Nếu sau này bạn restore lại 2 file seed:
        database/DuLieuMau_QuanLyTrungTamNgoaiNgu_v2.sql
        database/BoSung_ChuongTrinh_v3.sql
   thì tên "… K01" sẽ quay lại → chỉ cần chạy lại script này.
   ============================================================================= */
