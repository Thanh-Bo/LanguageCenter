/* ============================================================================================
   ThemLoaiBuoi_KiemTra_v20.sql
   --------------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Bổ sung loại buổi thứ 3: N'Kiểm tra' cho hai cột LoaiBuoi.

           NoiDungBaiHoc.LoaiBuoi : Buổi học | Kiểm tra | Buổi thi
           CaHoc.LoaiBuoi         : Buổi học | Kiểm tra | Buổi thi

   VÌ SAO CẦN
       CHECK constraint cũ chỉ cho 2 giá trị (CK_..._LoaiBuoi) nên mọi câu
       INSERT/UPDATE sang N'Kiểm tra' đều bị SQL Server từ chối.

   GHI CHÚ
       - KHÔNG mất dữ liệu: chỉ thay constraint, dữ liệu cũ giữ nguyên.
       - Idempotent: chạy lại nhiều lần vẫn OK (xóa nếu tồn tại rồi tạo lại).
       - Nên chạy cùng lúc với bản build có SessionKinds mới.
   ============================================================================================ */

SET NOCOUNT ON;
USE QuanLyTrungTamNgoaiNgu;
GO

/* ---------------------------------------------------------------- CaHoc */
IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_CaHoc_LoaiBuoi')
    ALTER TABLE [dbo].[CaHoc] DROP CONSTRAINT [CK_CaHoc_LoaiBuoi];
GO

ALTER TABLE [dbo].[CaHoc] WITH CHECK ADD CONSTRAINT [CK_CaHoc_LoaiBuoi]
    CHECK ([LoaiBuoi] IN (N'Buổi học', N'Kiểm tra', N'Buổi thi'));
GO

ALTER TABLE [dbo].[CaHoc] CHECK CONSTRAINT [CK_CaHoc_LoaiBuoi];
GO

/* ------------------------------------------------------- NoiDungBaiHoc */
IF EXISTS (SELECT 1 FROM sys.check_constraints WHERE name = N'CK_NoiDungBaiHoc_LoaiBuoi')
    ALTER TABLE [dbo].[NoiDungBaiHoc] DROP CONSTRAINT [CK_NoiDungBaiHoc_LoaiBuoi];
GO

ALTER TABLE [dbo].[NoiDungBaiHoc] WITH CHECK ADD CONSTRAINT [CK_NoiDungBaiHoc_LoaiBuoi]
    CHECK ([LoaiBuoi] IN (N'Buổi học', N'Kiểm tra', N'Buổi thi'));
GO

ALTER TABLE [dbo].[NoiDungBaiHoc] CHECK CONSTRAINT [CK_NoiDungBaiHoc_LoaiBuoi];
GO

/* --------------------------------------------------------------- KIỂM TRA */
PRINT N'=== Kiểm tra lại constraint ===';

SELECT
    c.name                     AS TenConstraint,
    OBJECT_NAME(c.parent_object_id) AS Bang,
    c.definition               AS DinhNghia
FROM sys.check_constraints AS c
WHERE c.name IN (N'CK_CaHoc_LoaiBuoi', N'CK_NoiDungBaiHoc_LoaiBuoi');

PRINT N'=== Thống kê LoaiBuoi hiện có ===';

SELECT N'CaHoc' AS Bang, LoaiBuoi, COUNT(*) AS SoDong
FROM dbo.CaHoc
GROUP BY LoaiBuoi
UNION ALL
SELECT N'NoiDungBaiHoc', LoaiBuoi, COUNT(*)
FROM dbo.NoiDungBaiHoc
GROUP BY LoaiBuoi;

PRINT N'Xong. Từ giờ có thể dùng N''Kiểm tra'' cho LoaiBuoi.';
