/* =============================================================================
   BỔ SUNG v4 — PHÂN CÔNG GIẢNG DẠY (Khung phân công & kiểm tra trùng lịch)
   -----------------------------------------------------------------------------
   Mục tiêu:
     1. Cho phép PhanCongGiangDay.MaGiaoVien = NULL  → tạo "khung" phân công
        trước, gắn giáo viên / trợ giảng sau (trạng thái "Chờ phân công").
     2. Mỗi lớp chỉ được có MỘT khung trống cho mỗi vai trò (chống bấm lưu 2 lần).
     3. Thêm index phục vụ truy vấn phát hiện trùng lịch giữa các lớp.

   Lưu ý: SQL Server KHÔNG cho ALTER COLUMN khi cột đang có FK / index phụ thuộc,
          nên script bỏ FK + index cũ rồi tạo lại.
   Chạy 1 lần. Có thể chạy lại nhiều lần (idempotent).
   ============================================================================= */

USE QuanLyTrungTamNgoaiNgu;
GO

-- Bắt buộc cho filtered index (lỗi 1934 nếu thiếu)
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;
GO

/* -----------------------------------------------------------------------------
   1. Bỏ FK và index phụ thuộc cột MaGiaoVien
   ----------------------------------------------------------------------------- */
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_PhanCong_GiaoVien')
BEGIN
    ALTER TABLE PhanCongGiangDay DROP CONSTRAINT FK_PhanCong_GiaoVien;
    PRINT N'  - Đã bỏ FK_PhanCong_GiaoVien';
END
GO

IF EXISTS (
    SELECT 1 FROM sys.indexes
     WHERE name = N'IX_PhanCongGiangDay_GiaoVien'
       AND object_id = OBJECT_ID(N'PhanCongGiangDay')
)
BEGIN
    DROP INDEX IX_PhanCongGiangDay_GiaoVien ON PhanCongGiangDay;
    PRINT N'  - Đã bỏ index IX_PhanCongGiangDay_GiaoVien';
END
GO

/* -----------------------------------------------------------------------------
   2. Cho phép MaGiaoVien = NULL
   ----------------------------------------------------------------------------- */
IF EXISTS (
    SELECT 1 FROM sys.columns
     WHERE object_id = OBJECT_ID(N'PhanCongGiangDay')
       AND name = N'MaGiaoVien'
       AND is_nullable = 0
)
BEGIN
    ALTER TABLE PhanCongGiangDay ALTER COLUMN MaGiaoVien VARCHAR(20) NULL;
    PRINT N'  - MaGiaoVien giờ cho phép NULL';
END
ELSE
    PRINT N'  - MaGiaoVien đã cho phép NULL (bỏ qua)';
GO

/* -----------------------------------------------------------------------------
   3. Tạo lại index + FK (FK cho phép NULL: NULL không bị kiểm tra)
   ----------------------------------------------------------------------------- */
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
     WHERE name = N'IX_PhanCongGiangDay_GiaoVien'
       AND object_id = OBJECT_ID(N'PhanCongGiangDay')
)
    CREATE INDEX IX_PhanCongGiangDay_GiaoVien ON PhanCongGiangDay(MaGiaoVien);
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_PhanCong_GiaoVien')
BEGIN
    ALTER TABLE PhanCongGiangDay WITH CHECK
        ADD CONSTRAINT FK_PhanCong_GiaoVien
            FOREIGN KEY (MaGiaoVien) REFERENCES GiaoVien(MaGiaoVien);
    PRINT N'  - Đã tạo lại FK_PhanCong_GiaoVien';
END
GO

/* -----------------------------------------------------------------------------
   4. Chống trùng khung: 1 lớp chỉ có 1 dòng "chờ phân công" cho mỗi vai trò
   ----------------------------------------------------------------------------- */
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
     WHERE name = N'UX_PhanCong_ChoPhanCong'
       AND object_id = OBJECT_ID(N'PhanCongGiangDay')
)
BEGIN
    CREATE UNIQUE INDEX UX_PhanCong_ChoPhanCong
        ON PhanCongGiangDay(MaLopHoc, VaiTroPhanCong)
        WHERE MaGiaoVien IS NULL;
    PRINT N'  - Đã tạo UX_PhanCong_ChoPhanCong (lọc MaGiaoVien IS NULL)';
END
GO

/* -----------------------------------------------------------------------------
   5. Index tra cứu trùng lịch dạy (theo người + khoảng ngày)
   ----------------------------------------------------------------------------- */
IF NOT EXISTS (
    SELECT 1 FROM sys.indexes
     WHERE name = N'IX_PhanCong_GiaoVien_ThoiGian'
       AND object_id = OBJECT_ID(N'PhanCongGiangDay')
)
BEGIN
    CREATE INDEX IX_PhanCong_GiaoVien_ThoiGian
        ON PhanCongGiangDay(MaGiaoVien, TrangThai, NgayBatDau, NgayKetThuc);
    PRINT N'  - Đã tạo IX_PhanCong_GiaoVien_ThoiGian';
END
GO

PRINT N'=== BỔ SUNG v4 HOÀN TẤT ===';
GO

/* -----------------------------------------------------------------------------
   KIỂM TRA NHANH
   ----------------------------------------------------------------------------- */
SELECT c.name            AS Cot,
       t.name            AS KieuDuLieu,
       c.is_nullable     AS ChoPhepNull
  FROM sys.columns c
  JOIN sys.types   t ON t.user_type_id = c.user_type_id
 WHERE c.object_id = OBJECT_ID(N'PhanCongGiangDay')
   AND c.name IN (N'MaGiaoVien', N'VaiTroPhanCong', N'TrangThai');
GO
