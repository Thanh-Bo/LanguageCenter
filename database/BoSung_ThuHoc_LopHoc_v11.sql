/* =============================================================================
   BỔ SUNG v11 — 2 THỨ HỌC CỦA LỚP + CHUẨN HÓA NGÀY KẾT THÚC
   -----------------------------------------------------------------------------
   VÌ SAO: khung đào tạo nay cố định 10 tuần × 2 buổi/tuần. Nhưng lớp học chưa
           lưu "học thứ mấy" → admin phải nhớ tạo 2 dòng LichHoc, dễ thiếu.
           Đưa 2 thứ về chính bảng LopHoc để form lớp là đủ điều kiện sinh buổi.

   GỒM:
     1. Thêm cột ThuBuoi1 / ThuBuoi2 (1 = Thứ Hai … 7 = Chủ Nhật)
     2. Ràng buộc: 1..7, và ThuBuoi1 <> ThuBuoi2
     3. Suy ra 2 thứ từ LichHoc cho các lớp đã khai lịch (nếu còn trống)
     4. Chuẩn hóa NgàyKetThucDuKien = ngày buổi 20 (cho lớp CHƯA sinh buổi nào)

   CHẠY 1 LẦN — chạy lại vẫn an toàn.
   ============================================================================= */

USE QuanLyTrungTamNgoaiNgu;
GO

SET NOCOUNT ON;
GO

/* =============================================================================
   1. THÊM CỘT
   ============================================================================= */

PRINT N'===== BƯỚC 1: THÊM CỘT ThuBuoi1 / ThuBuoi2 =====';

IF COL_LENGTH('dbo.LopHoc', 'ThuBuoi1') IS NULL
BEGIN
    ALTER TABLE LopHoc ADD ThuBuoi1 TINYINT NULL;
    PRINT N'  - Đã thêm ThuBuoi1';
END
ELSE PRINT N'  - ThuBuoi1 đã tồn tại (bỏ qua)';
GO

IF COL_LENGTH('dbo.LopHoc', 'ThuBuoi2') IS NULL
BEGIN
    ALTER TABLE LopHoc ADD ThuBuoi2 TINYINT NULL;
    PRINT N'  - Đã thêm ThuBuoi2';
END
ELSE PRINT N'  - ThuBuoi2 đã tồn tại (bỏ qua)';
GO

/* =============================================================================
   2. RÀNG BUỘC
   ============================================================================= */

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_LopHoc_ThuHoc')
BEGIN
    ALTER TABLE LopHoc WITH CHECK
        ADD CONSTRAINT CK_LopHoc_ThuHoc
            CHECK (
                (ThuBuoi1 IS NULL OR ThuBuoi1 BETWEEN 1 AND 7)
                AND (ThuBuoi2 IS NULL OR ThuBuoi2 BETWEEN 1 AND 7)
                AND (ThuBuoi1 IS NULL OR ThuBuoi2 IS NULL OR ThuBuoi1 <> ThuBuoi2)
            );
    PRINT N'  - Đã thêm CK_LopHoc_ThuHoc';
END
ELSE PRINT N'  - CK_LopHoc_ThuHoc đã tồn tại (bỏ qua)';
GO

/* =============================================================================
   3. SUY RA 2 THỨ TỪ LỊCH HỌC ĐÃ KHAI (cho lớp cũ)
   ============================================================================= */

PRINT N'===== BƯỚC 3: LẤY 2 THỨ TỪ LichHoc =====';

WITH Lich AS
(
    SELECT MaLopHoc,
           ThuTrongTuan,
           ROW_NUMBER() OVER (PARTITION BY MaLopHoc ORDER BY ThuTrongTuan) AS Rn
      FROM LichHoc
     WHERE TrangThai <> N'Ngừng áp dụng'
)
UPDATE l
   SET l.ThuBuoi1 = (SELECT MAX(x.ThuTrongTuan) FROM Lich x WHERE x.MaLopHoc = l.MaLopHoc AND x.Rn = 1),
       l.ThuBuoi2 = (SELECT MAX(x.ThuTrongTuan) FROM Lich x WHERE x.MaLopHoc = l.MaLopHoc AND x.Rn = 2)
  FROM LopHoc l
 WHERE l.ThuBuoi1 IS NULL
   AND EXISTS (SELECT 1 FROM Lich x WHERE x.MaLopHoc = l.MaLopHoc AND x.Rn = 2);

PRINT N'  - Đã suy ra thứ cho ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' lớp.';
GO

/* =============================================================================
   4. CHUẨN HÓA NGÀY KẾT THÚC
      Lớp CHƯA sinh buổi nào → Ngày ket thúc = ngày buổi 20 (10 tuần = 63 ngày
      + chênh lệch 2 thứ). Lớp đã có buổi → giữ nguyên (ngày thật của buổi cuối).
   ============================================================================= */

PRINT N'===== BƯỚC 4: CHUẨN HÓA NGÀY KẾT THÚC =====';

UPDATE l
   SET l.NgayKetThucDuKien = DATEADD(
        DAY,
        63 + ((CAST(l.ThuBuoi2 AS INT) - CAST(l.ThuBuoi1 AS INT) + 7) % 7),
        DATEADD(
            DAY,
            ((CAST(l.ThuBuoi1 AS INT) - (CASE DATEPART(WEEKDAY, l.NgayBatDau)
                                            WHEN 1 THEN 7
                                            ELSE DATEPART(WEEKDAY, l.NgayBatDau) - 1
                                         END) + 7) % 7),
            l.NgayBatDau))
  FROM LopHoc l
 WHERE l.ThuBuoi1 IS NOT NULL
   AND l.ThuBuoi2 IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM CaHoc ch WHERE ch.MaLopHoc = l.MaLopHoc);

PRINT N'  - Đã chuẩn hóa ngày kết thúc cho ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' lớp (chưa sinh buổi).';
GO

/* =============================================================================
   4b. GIỜ KẾT THÚC = GIỜ BẮT ĐẦU + 2 GIỜ (mỗi buổi cố định 2 tiếng)
   ============================================================================= */

PRINT N'===== BƯỚC 4b: CHUẨN HÓA GIỜ HỌC (+2 giờ) =====';

UPDATE LopHoc
   SET GioKetThuc = DATEADD(HOUR, 2, CAST(GioBatDau AS DATETIME))
 WHERE GioBatDau IS NOT NULL
   AND (GioKetThuc IS NULL OR GioKetThuc <> DATEADD(HOUR, 2, CAST(GioBatDau AS DATETIME)));

PRINT N'  - Đã chuẩn hóa giờ kết thúc cho ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' lớp.';
GO

/* =============================================================================
   5. KIỂM TRA
   ============================================================================= */

PRINT N'===== KẾT QUẢ =====';

SELECT l.MaLop,
       l.TenLop,
       l.NgayBatDau,
       l.ThuBuoi1,
       l.ThuBuoi2,
       l.GioBatDau,
       l.GioKetThuc,
       l.NgayKetThucDuKien,
       (SELECT COUNT(*) FROM CaHoc ch WHERE ch.MaLopHoc = l.MaLopHoc) AS SoBuoiDaSinh
  FROM LopHoc l
 ORDER BY l.MaLopHoc;
GO

PRINT N'=== BỔ SUNG v11 HOÀN TẤT ===';
GO
