/* =============================================================================
   BỔ SUNG v8 — GIỜ HỌC CỦA LỚP (GioBatDau / GioKetThuc)
   -----------------------------------------------------------------------------
   Mục tiêu: lớp học có KHUNG GIỜ mặc định (ví dụ 18:00 – 19:30) để:
     · Hiển thị ngay trong danh sách lớp
     · Là giá trị gợi ý khi khai báo Lịch học tuần
     · Tránh phải mở bảng LichHoc mới biết lớp học buổi tối hay buổi sáng

   Ghi chú:
     · Hai cột cho phép NULL (lớp cũ chưa có giờ vẫn hợp lệ).
     · Ràng buộc CK: nếu có đủ 2 giờ thì Giờ kết thúc phải SAU Giờ bắt đầu.
     · Chạy 1 lần, chạy lại nhiều lần vẫn an toàn (idempotent).
   ============================================================================= */

USE QuanLyTrungTamNgoaiNgu;
GO

SET NOCOUNT ON;
GO

/* 1. Thêm cột */
IF COL_LENGTH('dbo.LopHoc', 'GioBatDau') IS NULL
BEGIN
    ALTER TABLE LopHoc ADD GioBatDau TIME NULL;
    PRINT N'  - Đã thêm cột GioBatDau';
END
ELSE
    PRINT N'  - Cột GioBatDau đã tồn tại (bỏ qua)';
GO

IF COL_LENGTH('dbo.LopHoc', 'GioKetThuc') IS NULL
BEGIN
    ALTER TABLE LopHoc ADD GioKetThuc TIME NULL;
    PRINT N'  - Đã thêm cột GioKetThuc';
END
ELSE
    PRINT N'  - Cột GioKetThuc đã tồn tại (bỏ qua)';
GO

/* 2. Ràng buộc: giờ kết thúc phải sau giờ bắt đầu */
IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_LopHoc_GioHoc')
BEGIN
    ALTER TABLE LopHoc WITH CHECK
        ADD CONSTRAINT CK_LopHoc_GioHoc
            CHECK (
                GioBatDau IS NULL
                OR GioKetThuc IS NULL
                OR GioKetThuc > GioBatDau
            );
    PRINT N'  - Đã thêm CK_LopHoc_GioHoc';
END
GO

/* 3. Kiểm tra */
SELECT c.name        AS Cot,
       t.name        AS KieuDuLieu,
       c.is_nullable AS ChoPhepNull
  FROM sys.columns c
  JOIN sys.types   t ON t.user_type_id = c.user_type_id
 WHERE c.object_id = OBJECT_ID(N'LopHoc')
   AND c.name IN (N'GioBatDau', N'GioKetThuc');
GO

PRINT N'===== KIỂM TRA DỮ LIỆU LỚP =====';

SELECT MaLop,
       TenLop,
       GioBatDau,
       GioKetThuc,
       TrangThai
  FROM LopHoc
 ORDER BY MaLopHoc;
GO

PRINT N'=== BỔ SUNG v8 HOÀN TẤT ===';
GO

/* -----------------------------------------------------------------------------
   TÙY CHỌN — đặt giờ mặc định cho các lớp đang có
   ----------------------------------------------------------------------------- */

-- UPDATE LopHoc SET GioBatDau = '18:00', GioKetThuc = '19:30'
--  WHERE GioBatDau IS NULL AND TrangThai <> N'Đã kết thúc';
