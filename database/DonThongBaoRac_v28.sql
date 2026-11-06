/* =====================================================================================
   DonThongBaoRac_v28.sql
   -------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Xoá các thông báo KHÔNG CÒN HỢP LỆ theo quy tắc hiện hành:

           Giáo viên / Trợ giảng CHỈ được gửi thông báo CHO LỚP mình phụ trách,
           KHÔNG được gửi trực tiếp cho một tài khoản cụ thể.

       Những bản ghi được tạo trước khi áp quy tắc (ví dụ do GV014 gửi riêng cho
       một tài khoản) đang nằm rác trong danh sách "Đã gửi" / hộp thư người nhận.

   ĐIỀU KIỆN XOÁ (cả 3 phải đúng)
       1. Người gửi có vai trò Giáo viên (MaVaiTro = 5) hoặc Trợ giảng (MaVaiTro = 6)
       2. Thông báo KHÔNG gắn lớp            (MaLopHoc IS NULL)
       3. Thông báo gửi cho một tài khoản    (MaTaiKhoanNhan IS NOT NULL)

   KHÔNG ĐỘNG TỚI
       • Thông báo do Quản trị viên / Nhân viên gửi (cá nhân là hợp lệ với họ)
       • Thông báo gửi cho lớp (kể cả của giáo viên)
       • Thông báo đã thu hồi (vẫn giữ để người gửi thấy trong tab "Đã gửi")

   AN TOÀN
       • 1 transaction + XACT_ABORT
       • In danh sách TRƯỚC khi xoá để bạn đối chiếu
       • Idempotent: chạy lại không xoá thêm gì
   ===================================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

/* Đổi thành 1 nếu muốn GIỮ LẠI (chỉ xem danh sách, không xoá). */
DECLARE @ChiXem bit = 0;

/* =====================================================================================
   1. DANH SÁCH RÁC
   ===================================================================================== */
IF OBJECT_ID(N'tempdb..#Rac') IS NOT NULL DROP TABLE #Rac;

CREATE TABLE #Rac (MaThongBao int PRIMARY KEY);

INSERT INTO #Rac (MaThongBao)
SELECT tb.MaThongBao
FROM dbo.ThongBao tb
JOIN dbo.TaiKhoan t ON t.MaTaiKhoan = tb.MaTaiKhoanGui
WHERE t.MaVaiTro IN (5, 6)
  AND tb.MaLopHoc IS NULL
  AND tb.MaTaiKhoanNhan IS NOT NULL
  AND tb.TrangThai <> N'Đã thu hồi';

DECLARE @SoRac int;
SELECT @SoRac = COUNT(*) FROM #Rac;

PRINT N'=== THÔNG BÁO RÁC TÌM THẤY: ' + CAST(@SoRac AS nvarchar(10)) + N' ===';

SELECT tb.MaThongBao,
       t.TenDangNhap                AS NguoiGui,
       t.MaVaiTro                   AS VaiTro,
       tb.TieuDe,
       tb.NoiDung,
       nhan.TenDangNhap             AS NguoiNhan,
       tb.ThoiGianGui,
       tb.TrangThai
FROM dbo.ThongBao tb
JOIN #Rac r                 ON r.MaThongBao = tb.MaThongBao
JOIN dbo.TaiKhoan t         ON t.MaTaiKhoan = tb.MaTaiKhoanGui
LEFT JOIN dbo.TaiKhoan nhan ON nhan.MaTaiKhoan = tb.MaTaiKhoanNhan
ORDER BY tb.ThoiGianGui DESC;

/* =====================================================================================
   2. XOÁ
   ===================================================================================== */
IF @ChiXem = 1
BEGIN
    PRINT N'>>> Chế độ CHỈ XEM (@ChiXem = 1) — không xoá gì.';
END
ELSE IF @SoRac = 0
BEGIN
    PRINT N'>>> Không có bản ghi rác. Không làm gì thêm.';
END
ELSE
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        DELETE tb
        FROM dbo.ThongBao tb
        WHERE tb.MaThongBao IN (SELECT MaThongBao FROM #Rac);

        COMMIT TRANSACTION;

        PRINT N'>>> Đã xoá ' + CAST(@SoRac AS nvarchar(10)) + N' thông báo rác.';
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;

        PRINT N'LỖI: ' + ERROR_MESSAGE() + N' (dòng ' + CAST(ERROR_LINE() AS nvarchar(10)) + N')';
        THROW;
    END CATCH
END
GO

/* =====================================================================================
   3. KIỂM TRA SAU KHI XOÁ
   ===================================================================================== */
PRINT N'--- KIỂM TRA ---';

SELECT N'Thông báo còn lại' AS HangMuc, COUNT(*) AS SoLuong FROM dbo.ThongBao
UNION ALL
SELECT N'Còn rác (phải = 0)', COUNT(*)
FROM dbo.ThongBao tb
JOIN dbo.TaiKhoan t ON t.MaTaiKhoan = tb.MaTaiKhoanGui
WHERE t.MaVaiTro IN (5, 6)
  AND tb.MaLopHoc IS NULL
  AND tb.MaTaiKhoanNhan IS NOT NULL;
