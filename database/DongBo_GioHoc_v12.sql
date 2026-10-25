/* =============================================================================
   ĐỒNG BỘ v12 — GIỜ HỌC 2 TIẾNG Ở MỌI BẢNG
   -----------------------------------------------------------------------------
   VÌ SAO: từ v11, form Lớp học đã tính Giờ kết thúc = Giờ bắt đầu + 2 giờ.
           Nhưng bảng LichHoc (lịch tuần) và CaHoc (buổi học thực tế) vẫn giữ
           giờ cũ (ví dụ 18:00 – 19:30) nên màn "Lịch học" hiện sai.

   NGUYÊN TẮC: LopHoc là NGUỒN DUY NHẤT của giờ học.
                LichHoc / CaHoc chỉ copy lại từ LopHoc.

   GỒM:
     1. LopHoc   : GioKetThuc = GioBatDau + 2 giờ
     2. LichHoc  : GioBatDau/GioKetThuc = của LopHoc
     3. CaHoc    : GioBatDau/GioKetThuc = của LopHoc (buổi đã sinh)
     4. Kiểm tra

   CHẠY 1 LẦN — chạy lại vẫn an toàn.
   ============================================================================= */

USE QuanLyTrungTamNgoaiNgu;
GO

SET NOCOUNT ON;
GO

/* =============================================================================
   1. LOPHOC — chuẩn hóa giờ kết thúc = giờ bắt đầu + 2 giờ
   ============================================================================= */

PRINT N'===== BƯỚC 1: LopHoc.GioKetThuc = GioBatDau + 2 giờ =====';

UPDATE LopHoc
   SET GioKetThuc = DATEADD(HOUR, 2, CAST(GioBatDau AS DATETIME))
 WHERE GioBatDau IS NOT NULL
   AND (GioKetThuc IS NULL
        OR GioKetThuc <> DATEADD(HOUR, 2, CAST(GioBatDau AS DATETIME)));

PRINT N'  - Đã chuẩn hóa ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' lớp.';
GO

/* =============================================================================
   2. LICHHOC — copy giờ từ lớp (lịch tuần không tự khai giờ nữa)
   ============================================================================= */

PRINT N'===== BƯỚC 2: LichHoc lấy giờ từ LopHoc =====';

UPDATE lh
   SET lh.GioBatDau = l.GioBatDau,
       lh.GioKetThuc = l.GioKetThuc
  FROM LichHoc lh
  JOIN LopHoc l ON l.MaLopHoc = lh.MaLopHoc
 WHERE l.GioBatDau IS NOT NULL
   AND l.GioKetThuc IS NOT NULL
   AND (lh.GioBatDau <> l.GioBatDau OR lh.GioKetThuc <> l.GioKetThuc);

PRINT N'  - Đã đồng bộ ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' lịch tuần.';
GO

/* =============================================================================
   3. CAHOC — buổi học thực tế đã sinh cũng phải đúng 2 giờ
   ============================================================================= */

PRINT N'===== BƯỚC 3: CaHoc lấy giờ từ LopHoc =====';

IF COL_LENGTH('dbo.CaHoc', 'GioBatDau') IS NOT NULL
   AND COL_LENGTH('dbo.CaHoc', 'GioKetThuc') IS NOT NULL
BEGIN
    UPDATE ch
       SET ch.GioBatDau = l.GioBatDau,
           ch.GioKetThuc = l.GioKetThuc
      FROM CaHoc ch
      JOIN LopHoc l ON l.MaLopHoc = ch.MaLopHoc
     WHERE l.GioBatDau IS NOT NULL
       AND l.GioKetThuc IS NOT NULL
       AND (ch.GioBatDau <> l.GioBatDau OR ch.GioKetThuc <> l.GioKetThuc);

    PRINT N'  - Đã đồng bộ ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' buổi học.';
END
ELSE PRINT N'  - Bảng CaHoc không có cột giờ (bỏ qua).';
GO

/* =============================================================================
   4. KIỂM TRA
   ============================================================================= */

PRINT N'===== KẾT QUẢ =====';

SELECT l.MaLop,
       l.GioBatDau        AS Lop_BatDau,
       l.GioKetThuc       AS Lop_KetThuc,
       lh.ThuTrongTuan    AS Lich_Thu,
       lh.GioBatDau       AS Lich_BatDau,
       lh.GioKetThuc      AS Lich_KetThuc,
       (SELECT COUNT(*) FROM CaHoc ch WHERE ch.MaLopHoc = l.MaLopHoc) AS SoBuoi
  FROM LopHoc l
  LEFT JOIN LichHoc lh ON lh.MaLopHoc = l.MaLopHoc
 ORDER BY l.MaLopHoc, lh.ThuTrongTuan;
GO

PRINT N'=== ĐỒNG BỘ v12 HOÀN TẤT ===';
GO
