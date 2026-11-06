/* =====================================================================================
   TaoHocPhi_ConLai_v26.sql
   -------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Tạo nghĩa vụ học phí (bảng HocPhi) cho TẤT CẢ đăng ký học còn hiệu lực
       mà CHƯA có học phí.

       Dùng sau khi đã tạo tay một vài học phí mẫu → script tự bỏ qua những đăng ký đó
       (quan hệ 1-1: mỗi DangKyHoc chỉ có tối đa MỘT HocPhi).

   ĐIỀU KIỆN LẤY
       DangKyHoc.TrangThaiDangKy IN (N'Đã xếp lớp', N'Hoàn thành')
       AND chưa tồn tại dòng HocPhi cùng MaDangKy
       AND đăng ký chưa bị huỷ.

   GIÁ TRỊ GHI
       SoTienGoc      = DangKyHoc.HocPhiGoc          (đã chốt lúc đăng ký)
       SoTienGiam     = DangKyHoc.SoTienGiam         (ưu đãi đã áp)
       SoTienDaDong   = 0
       HanThanhToan   = NgayBatDau của lớp + 7 ngày  (NULL nếu đăng ký chưa xếp lớp)
       TrangThaiHocPhi= N'Đã thanh toán' nếu phải đóng <= 0, ngược lại N'Chưa thanh toán'
       NgayTao        = DangKyHoc.NgayDangKy (không có thì GETDATE())

   ⚠️ KHÔNG ghi 2 cột computed PERSISTED (SQL Server tự tính):
         SoTienPhaiDong  = SoTienGoc - SoTienGiam
         SoTienConThieu  = (SoTienGoc - SoTienGiam) - SoTienDaDong

   AN TOÀN
       • 1 transaction + XACT_ABORT → lỗi giữa đường thì ROLLBACK.
       • Idempotent: chạy lại nhiều lần không tạo trùng.
       • Không xoá / không sửa học phí đã có.

   LIÊN QUAN
       Chạy sau: PhanCong_5ChuongTrinh_v25.sql
       Thanh toán mẫu: dùng trang "Học phí" của web hoặc POST
       /api/admin/tuition-fees/{id}/payments
   ===================================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @HomNay date = CAST(GETDATE() AS date);

    PRINT N'=== v26 — TẠO HỌC PHÍ CHO CÁC ĐĂNG KÝ CÒN THIẾU ===';

    IF OBJECT_ID(N'dbo.HocPhi', N'U') IS NULL OR OBJECT_ID(N'dbo.DangKyHoc', N'U') IS NULL
    BEGIN
        RAISERROR(N'Thiếu bảng HocPhi / DangKyHoc. Kiểm tra database đang trỏ tới.', 16, 1);
    END

    /* =============================================================================
       1. LẬP DANH SÁCH ĐĂNG KÝ CẦN TẠO HỌC PHÍ
       ============================================================================= */
    IF OBJECT_ID(N'tempdb..#CanTao') IS NOT NULL DROP TABLE #CanTao;

    CREATE TABLE #CanTao
    (
        MaDangKy    int            NOT NULL PRIMARY KEY,
        MaHocVien   varchar(20)    NOT NULL,
        MaLop       varchar(50)    NULL,
        HocPhiGoc   decimal(18, 2) NOT NULL,
        SoTienGiam  decimal(18, 2) NOT NULL,
        NgayBatDau  date           NULL,
        NgayDangKy  datetime       NULL
    );

    INSERT INTO #CanTao (MaDangKy, MaHocVien, MaLop, HocPhiGoc, SoTienGiam, NgayBatDau, NgayDangKy)
    SELECT dk.MaDangKy,
           dk.MaHocVien,
           lp.MaLop,
           dk.HocPhiGoc,
           ISNULL(dk.SoTienGiam, 0),
           lp.NgayBatDau,
           dk.NgayDangKy
    FROM dbo.DangKyHoc dk
    LEFT JOIN dbo.LopHoc lp ON lp.MaLopHoc = dk.MaLopHoc
    WHERE dk.TrangThaiDangKy IN (N'Đã xếp lớp', N'Hoàn thành')
      AND NOT EXISTS (SELECT 1 FROM dbo.HocPhi hp WHERE hp.MaDangKy = dk.MaDangKy);

    DECLARE @SoCanTao int;
    SELECT @SoCanTao = COUNT(*) FROM #CanTao;

    PRINT N'  • Đăng ký cần tạo học phí: ' + CAST(@SoCanTao AS nvarchar(10));

    /* =============================================================================
       2. TẠO HỌC PHÍ
       ============================================================================= */
    IF @SoCanTao > 0
    BEGIN
        INSERT INTO dbo.HocPhi (MaDangKy, SoTienGoc, SoTienGiam, SoTienDaDong,
                                HanThanhToan, TrangThaiHocPhi, NgayTao)
        SELECT c.MaDangKy,
               c.HocPhiGoc,
               c.SoTienGiam,
               0,
               CASE WHEN c.NgayBatDau IS NULL THEN NULL
                    ELSE DATEADD(day, 7, c.NgayBatDau) END,
               CASE WHEN c.HocPhiGoc - c.SoTienGiam <= 0
                    THEN N'Đã thanh toán'
                    ELSE N'Chưa thanh toán' END,
               ISNULL(c.NgayDangKy, GETDATE())
        FROM #CanTao c;
    END

    DECLARE @SoDaTao int;
    SELECT @SoDaTao = COUNT(*) FROM dbo.HocPhi;

    PRINT N'  • Tổng số học phí trong hệ thống sau khi tạo: ' + CAST(@SoDaTao AS nvarchar(10));

    /* =============================================================================
       3. KIỂM TRA KẾT QUẢ
       ============================================================================= */
    PRINT N'';
    PRINT N'--- KIỂM TRA ---';

    -- 3.1 Tổng quan: đăng ký hiệu lực vs số học phí
    SELECT N'Đăng ký còn hiệu lực' AS HangMuc, COUNT(*) AS SoLuong
    FROM dbo.DangKyHoc
    WHERE TrangThaiDangKy IN (N'Đã xếp lớp', N'Hoàn thành')
    UNION ALL
    SELECT N'Đã có học phí', COUNT(*)
    FROM dbo.HocPhi
    UNION ALL
    SELECT N'Đăng ký còn THIẾU học phí', COUNT(*)
    FROM dbo.DangKyHoc dk
    WHERE dk.TrangThaiDangKy IN (N'Đã xếp lớp', N'Hoàn thành')
      AND NOT EXISTS (SELECT 1 FROM dbo.HocPhi hp WHERE hp.MaDangKy = dk.MaDangKy)
    UNION ALL
    SELECT N'Tổng tiền phải thu (đ)', CAST(ISNULL(SUM(SoTienPhaiDong), 0) AS int)
    FROM dbo.HocPhi
    UNION ALL
    SELECT N'Tổng tiền còn thiếu (đ)', CAST(ISNULL(SUM(SoTienConThieu), 0) AS int)
    FROM dbo.HocPhi;

    -- 3.2 Theo từng lớp: sĩ số phải bằng số học phí (cột KetQua phải toàn OK)
    SELECT lp.MaLop,
           lp.TenLop,
           lp.TrangThai                                          AS TrangThaiLop,
           (SELECT COUNT(*) FROM dbo.DangKyHoc dk
             WHERE dk.MaLopHoc = lp.MaLopHoc
               AND dk.TrangThaiDangKy IN (N'Đã xếp lớp', N'Hoàn thành')) AS SiSo,
           (SELECT COUNT(*) FROM dbo.HocPhi hp
             JOIN dbo.DangKyHoc dk2 ON dk2.MaDangKy = hp.MaDangKy
             WHERE dk2.MaLopHoc = lp.MaLopHoc)                            AS SoHocPhi,
           CASE WHEN (SELECT COUNT(*) FROM dbo.DangKyHoc dk
                       WHERE dk.MaLopHoc = lp.MaLopHoc
                         AND dk.TrangThaiDangKy IN (N'Đã xếp lớp', N'Hoàn thành'))
                  = (SELECT COUNT(*) FROM dbo.HocPhi hp
                      JOIN dbo.DangKyHoc dk3 ON dk3.MaDangKy = hp.MaDangKy
                      WHERE dk3.MaLopHoc = lp.MaLopHoc)
                THEN N'OK' ELSE N'KIỂM TRA LẠI' END                       AS KetQua
    FROM dbo.LopHoc lp
    ORDER BY lp.MaLop;

    -- 3.3 Danh sách học phí vừa tạo (theo mã đăng ký)
    SELECT dk.MaDangKy, dk.MaHocVien, hv.HoTen, lp.MaLop, kh.TenKhoaHoc,
           hp.SoTienGoc, hp.SoTienGiam, hp.SoTienPhaiDong, hp.SoTienDaDong,
           hp.SoTienConThieu, hp.HanThanhToan, hp.TrangThaiHocPhi
    FROM dbo.HocPhi hp
    JOIN dbo.DangKyHoc dk ON dk.MaDangKy = hp.MaDangKy
    JOIN dbo.HocVien   hv ON hv.MaHocVien = dk.MaHocVien
    LEFT JOIN dbo.LopHoc lp ON lp.MaLopHoc = dk.MaLopHoc
    LEFT JOIN dbo.KhoaHoc kh ON kh.MaKhoaHoc = dk.MaKhoaHoc
    ORDER BY lp.MaLop, dk.MaHocVien;

    -- 3.4 CẢNH BÁO: học phí gốc lệch học phí cơ bản của khoá (phải RỖNG)
    SELECT N'CẢNH BÁO: HocPhiGoc lệch KhoaHoc.HocPhiCoBan' AS CanhBao,
           dk.MaDangKy, dk.MaHocVien, kh.MaKhoaHoc, kh.TenKhoaHoc,
           dk.HocPhiGoc, kh.HocPhiCoBan
    FROM dbo.DangKyHoc dk
    JOIN dbo.KhoaHoc kh ON kh.MaKhoaHoc = dk.MaKhoaHoc
    WHERE dk.TrangThaiDangKy IN (N'Đã xếp lớp', N'Hoàn thành')
      AND dk.HocPhiGoc <> kh.HocPhiCoBan;

    -- 3.5 CẢNH BÁO: học phí đã quá hạn mà chưa đóng đủ (chỉ để biết, KHÔNG phải lỗi)
    SELECT N'Quá hạn' AS GhiChu, COUNT(*) AS SoHocPhiQuaHan
    FROM dbo.HocPhi
    WHERE SoTienConThieu > 0
      AND HanThanhToan IS NOT NULL
      AND HanThanhToan < @HomNay;

    COMMIT TRANSACTION;

    PRINT N'=== HOÀN TẤT v26 ===';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT N'LỖI: ' + ERROR_MESSAGE() + N' (dòng ' + CAST(ERROR_LINE() AS nvarchar(10)) + N')';
    THROW;
END CATCH;
