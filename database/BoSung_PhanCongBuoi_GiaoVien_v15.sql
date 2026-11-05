/* =============================================================================
   BỔ SUNG v15 — PHÂN CÔNG THEO BUỔI (GV VIỆT / GV NƯỚC NGOÀI) + LOẠI BUỔI
   -----------------------------------------------------------------------------
   VÌ SAO:
     1. Lớp học 2 buổi/tuần cần 1 buổi do GIÁO VIÊN NƯỚC NGOÀI dạy và 1 buổi do
        GIÁO VIÊN VIỆT dạy. Trước đây PhanCongGiangDay chỉ có 3 vai trò nên không
        biểu diễn được "giáo viên nước ngoài".
        → Thêm vai trò N'Giáo viên nước ngoài'.
        Quy ước: buổi ĐẦU TUẦN (thứ nhỏ hơn trong 2 thứ của lớp) = GV nước ngoài,
                 buổi còn lại = GV chính (Việt).
     2. Tiến độ buổi học cần biết buổi nào là BUỔI HỌC, buổi nào là BUỔI THI và
        buổi đó là buổi thứ mấy trong khóa.
        → CaHoc: thêm LoaiBuoi + SoBuoi.
        → NoiDungBaiHoc: thêm LoaiBuoi (khai báo buổi thi ngay trong khóa học).
     3. Một đầu điểm có thể có NHIỀU BÀI KIỂM TRA (lần 1, lần 2, ...) và mỗi bài
        có thể gắn với buổi thi tương ứng.
        → DiemSo: thêm LanKiemTra + MaCaHoc (nullable).
        → Đổi UNIQUE (MaDangKy, MaLoaiDiem) → (MaDangKy, MaLoaiDiem, LanKiemTra).

   CHẠY 1 LẦN — chạy lại vẫn an toàn.
   ============================================================================= */

USE QuanLyTrungTamNgoaiNgu;
GO

SET NOCOUNT ON;
GO

/* =============================================================================
   1. CaHoc: LoaiBuoi + SoBuoi
   ============================================================================= */

PRINT N'===== BƯỚC 1: CaHoc them LoaiBuoi / SoBuoi =====';

IF COL_LENGTH('dbo.CaHoc', 'LoaiBuoi') IS NULL
BEGIN
    ALTER TABLE CaHoc
        ADD LoaiBuoi NVARCHAR(30) NOT NULL
            CONSTRAINT DF_CaHoc_LoaiBuoi DEFAULT N'Buổi học';
    PRINT N'  - Đã thêm CaHoc.LoaiBuoi';
END
ELSE PRINT N'  - CaHoc.LoaiBuoi đã tồn tại (bỏ qua)';
GO

IF COL_LENGTH('dbo.CaHoc', 'SoBuoi') IS NULL
BEGIN
    ALTER TABLE CaHoc ADD SoBuoi INT NULL;
    PRINT N'  - Đã thêm CaHoc.SoBuoi';
END
ELSE PRINT N'  - CaHoc.SoBuoi đã tồn tại (bỏ qua)';
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_CaHoc_LoaiBuoi')
BEGIN
    ALTER TABLE CaHoc WITH CHECK
        ADD CONSTRAINT CK_CaHoc_LoaiBuoi
            CHECK (LoaiBuoi IN (N'Buổi học', N'Buổi thi'));
    PRINT N'  - Đã thêm CK_CaHoc_LoaiBuoi';
END
ELSE PRINT N'  - CK_CaHoc_LoaiBuoi đã tồn tại (bỏ qua)';
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_CaHoc_SoBuoi')
BEGIN
    ALTER TABLE CaHoc WITH CHECK
        ADD CONSTRAINT CK_CaHoc_SoBuoi
            CHECK (SoBuoi IS NULL OR SoBuoi >= 1);
    PRINT N'  - Đã thêm CK_CaHoc_SoBuoi';
END
ELSE PRINT N'  - CK_CaHoc_SoBuoi đã tồn tại (bỏ qua)';
GO

/* =============================================================================
   2. NoiDungBaiHoc: LoaiBuoi (khai báo buổi thi trong khóa học)
   ============================================================================= */

PRINT N'===== BƯỚC 2: NoiDungBaiHoc them LoaiBuoi =====';

IF COL_LENGTH('dbo.NoiDungBaiHoc', 'LoaiBuoi') IS NULL
BEGIN
    ALTER TABLE NoiDungBaiHoc
        ADD LoaiBuoi NVARCHAR(30) NOT NULL
            CONSTRAINT DF_NoiDungBaiHoc_LoaiBuoi DEFAULT N'Buổi học';
    PRINT N'  - Đã thêm NoiDungBaiHoc.LoaiBuoi';
END
ELSE PRINT N'  - NoiDungBaiHoc.LoaiBuoi đã tồn tại (bỏ qua)';
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_NoiDungBaiHoc_LoaiBuoi')
BEGIN
    ALTER TABLE NoiDungBaiHoc WITH CHECK
        ADD CONSTRAINT CK_NoiDungBaiHoc_LoaiBuoi
            CHECK (LoaiBuoi IN (N'Buổi học', N'Buổi thi'));
    PRINT N'  - Đã thêm CK_NoiDungBaiHoc_LoaiBuoi';
END
ELSE PRINT N'  - CK_NoiDungBaiHoc_LoaiBuoi đã tồn tại (bỏ qua)';
GO

/* =============================================================================
   3. DiemSo: LanKiemTra + MaCaHoc + đổi UNIQUE
   ============================================================================= */

PRINT N'===== BƯỚC 3: DiemSo them LanKiemTra / MaCaHoc =====';

IF COL_LENGTH('dbo.DiemSo', 'LanKiemTra') IS NULL
BEGIN
    ALTER TABLE DiemSo
        ADD LanKiemTra INT NOT NULL
            CONSTRAINT DF_DiemSo_LanKiemTra DEFAULT 1;
    PRINT N'  - Đã thêm DiemSo.LanKiemTra';
END
ELSE PRINT N'  - DiemSo.LanKiemTra đã tồn tại (bỏ qua)';
GO

IF COL_LENGTH('dbo.DiemSo', 'MaCaHoc') IS NULL
BEGIN
    ALTER TABLE DiemSo ADD MaCaHoc INT NULL;
    PRINT N'  - Đã thêm DiemSo.MaCaHoc';
END
ELSE PRINT N'  - DiemSo.MaCaHoc đã tồn tại (bỏ qua)';
GO

IF NOT EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_DiemSo_LanKiemTra')
BEGIN
    ALTER TABLE DiemSo WITH CHECK
        ADD CONSTRAINT CK_DiemSo_LanKiemTra CHECK (LanKiemTra >= 1);
    PRINT N'  - Đã thêm CK_DiemSo_LanKiemTra';
END
ELSE PRINT N'  - CK_DiemSo_LanKiemTra đã tồn tại (bỏ qua)';
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = N'FK_DiemSo_CaHoc')
BEGIN
    ALTER TABLE DiemSo WITH CHECK
        ADD CONSTRAINT FK_DiemSo_CaHoc
            FOREIGN KEY (MaCaHoc) REFERENCES CaHoc(MaCaHoc);
    PRINT N'  - Đã thêm FK_DiemSo_CaHoc';
END
ELSE PRINT N'  - FK_DiemSo_CaHoc đã tồn tại (bỏ qua)';
GO

-- Bỏ UNIQUE cũ (nếu còn) rồi tạo UNIQUE mới gồm LanKiemTra.
IF EXISTS (
    SELECT 1
      FROM sys.key_constraints
     WHERE name = N'UQ_DiemSo'
       AND parent_object_id = OBJECT_ID(N'dbo.DiemSo')
)
BEGIN
    ALTER TABLE DiemSo DROP CONSTRAINT UQ_DiemSo;
    PRINT N'  - Đã xóa UNIQUE cũ UQ_DiemSo(MaDangKy, MaLoaiDiem)';
END
ELSE PRINT N'  - UNIQUE cũ UQ_DiemSo không còn (bỏ qua)';
GO

IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UQ_DiemSo' AND object_id = OBJECT_ID(N'dbo.DiemSo'))
BEGIN
    DROP INDEX UQ_DiemSo ON DiemSo;
    PRINT N'  - Đã xóa index UQ_DiemSo cũ';
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.key_constraints WHERE name = N'UQ_DiemSo')
BEGIN
    ALTER TABLE DiemSo
        ADD CONSTRAINT UQ_DiemSo UNIQUE (MaDangKy, MaLoaiDiem, LanKiemTra);
    PRINT N'  - Đã tạo UQ_DiemSo(MaDangKy, MaLoaiDiem, LanKiemTra)';
END
ELSE PRINT N'  - UQ_DiemSo đã tồn tại (bỏ qua)';
GO

/* =============================================================================
   4. PhanCongGiangDay: thêm vai trò "Giáo viên nước ngoài"
   ============================================================================= */

PRINT N'===== BƯỚC 4: PhanCongGiangDay them vai tro GV nuoc ngoai =====';

IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_PhanCong_VaiTro')
BEGIN
    ALTER TABLE PhanCongGiangDay DROP CONSTRAINT CK_PhanCong_VaiTro;
    PRINT N'  - Đã xóa CK_PhanCong_VaiTro cũ';
END
GO

ALTER TABLE PhanCongGiangDay WITH CHECK
    ADD CONSTRAINT CK_PhanCong_VaiTro
        CHECK (
            VaiTroPhanCong IN
            (
                N'Giáo viên chính',
                N'Giáo viên nước ngoài',
                N'Giáo viên thay thế',
                N'Trợ giảng'
            )
        );
PRINT N'  - Đã tạo CK_PhanCong_VaiTro (4 vai trò)';
GO

/* =============================================================================
   5. Backfill SoBuoi cho các ca học đã có
   ============================================================================= */

PRINT N'===== BƯỚC 5: Backfill CaHoc.SoBuoi =====';

WITH X AS
(
    SELECT MaCaHoc,
           ROW_NUMBER() OVER (PARTITION BY MaLopHoc
                              ORDER BY NgayHoc, GioBatDau, MaCaHoc) AS Rn
      FROM CaHoc
)
UPDATE c
   SET c.SoBuoi = x.Rn
  FROM CaHoc c
  JOIN X x ON x.MaCaHoc = c.MaCaHoc
 WHERE c.SoBuoi IS NULL;

PRINT N'  - Đã đánh số buổi cho ca học hiện có';
GO

PRINT N'===== HOÀN TẤT v15 =====';
GO
