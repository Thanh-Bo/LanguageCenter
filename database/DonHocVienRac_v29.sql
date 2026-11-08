/* =====================================================================================
   DonHocVienRac_v29.sql
   -------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Xoá các HỌC VIÊN RÁC còn sót lại từ đợt test Phase 3
       (tên dạng "Ph3 ..." và email dạng ...@test.local):

           HV006  Ph3 Student Two 001928
           HV007  Ph3 Concurrent 2 001928
           HV010  Ph3 Concurrent 5 001928
           HV011  Ph3 Concurrent 1 001928
           HV012  Ph3 ResetTarget 002119
           HV013  Ph3 UpdStudent Renamed
           ... (bất kỳ bản ghi nào khớp điều kiện bên dưới)

   ĐIỀU KIỆN XOÁ (phải khớp CẢ HAI phần)
       A. HoTen LIKE N'Ph3 %'  HOẶC  Email LIKE N'%@test.local'
       B. KHÔNG có đăng ký học nào ở trạng thái N'Đã xếp lớp' / N'Hoàn thành'

       Điều kiện B là chốt an toàn: học viên thật (70 em của v25) không bao giờ
       khớp mẫu tên/email trên, và nếu có khớp thì vẫn được giữ lại.

   THỨ TỰ XOÁ (theo khoá ngoại)
       BienNhan → ThanhToan → HocPhi → DiemSo → DangKyHoc
       → DiemDanh / KiemTraDauVao / PhanHoiDichVu / PhanHoiKhaoSat
       → ThongBao → HocVien → TaiKhoan

   ⚠️ LƯU Ý VỀ CHIỀU KHOÁ NGOẠI
       FK_HocVien_TaiKhoan là  HocVien.MaHocVien → TaiKhoan.TenDangNhap
       (KHÔNG phải TaiKhoan → HocVien), nên phải xoá HocVien TRƯỚC TaiKhoan.
       Xoá tài khoản trước sẽ bị chặn với lỗi 547 "conflicted with the
       REFERENCE constraint FK_HocVien_TaiKhoan".

   CÁCH DÙNG
       • Mặc định @ChiXem = 0 → XOÁ THẬT.
       • Muốn xem trước: đổi @ChiXem = 1, chạy, xem danh sách rồi đổi lại 0.
   ===================================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

DECLARE @ChiXem bit = 0;

/* =====================================================================================
   1. LẬP DANH SÁCH HỌC VIÊN RÁC
   ===================================================================================== */
IF OBJECT_ID(N'tempdb..#HvRac') IS NOT NULL DROP TABLE #HvRac;

CREATE TABLE #HvRac
(
    MaHocVien   varchar(20) PRIMARY KEY,
    HoTen       nvarchar(100),
    Email       nvarchar(150),
    MaTaiKhoan  int NULL
);

INSERT INTO #HvRac (MaHocVien, HoTen, Email, MaTaiKhoan)
SELECT hv.MaHocVien,
       hv.HoTen,
       hv.Email,
       t.MaTaiKhoan
FROM dbo.HocVien hv
LEFT JOIN dbo.TaiKhoan t ON t.TenDangNhap = hv.MaHocVien
WHERE (hv.HoTen LIKE N'Ph3 %' OR hv.Email LIKE N'%@test.local')
  AND NOT EXISTS (
        SELECT 1
        FROM dbo.DangKyHoc dk
        WHERE dk.MaHocVien = hv.MaHocVien
          AND dk.TrangThaiDangKy IN (N'Đã xếp lớp', N'Hoàn thành'));

DECLARE @SoRac int;
SELECT @SoRac = COUNT(*) FROM #HvRac;

PRINT N'=== HỌC VIÊN RÁC TÌM THẤY: ' + CAST(@SoRac AS nvarchar(10)) + N' ===';

SELECT r.MaHocVien, r.HoTen, r.Email, r.MaTaiKhoan,
       (SELECT COUNT(*) FROM dbo.DangKyHoc dk WHERE dk.MaHocVien = r.MaHocVien)
           AS SoDangKy,
       (SELECT COUNT(*) FROM dbo.DiemDanh dd WHERE dd.MaHocVien = r.MaHocVien)
           AS SoDiemDanh
FROM #HvRac r
ORDER BY r.MaHocVien;

/* Các bản ghi "na ná" nhưng KHÔNG bị xoá — để bạn kiểm tra thêm nếu cần */
SELECT N'Không xoá (có đăng ký còn hiệu lực)' AS GhiChu,
       hv.MaHocVien, hv.HoTen, hv.Email, hv.TrangThaiHocTap
FROM dbo.HocVien hv
WHERE (hv.HoTen LIKE N'Ph3 %' OR hv.Email LIKE N'%@test.local')
  AND EXISTS (
        SELECT 1 FROM dbo.DangKyHoc dk
        WHERE dk.MaHocVien = hv.MaHocVien
          AND dk.TrangThaiDangKy IN (N'Đã xếp lớp', N'Hoàn thành'));

/* =====================================================================================
   2. GOM DỮ LIỆU LIÊN QUAN
   ===================================================================================== */
IF OBJECT_ID(N'tempdb..#DangKy') IS NOT NULL DROP TABLE #DangKy;
IF OBJECT_ID(N'tempdb..#HocPhi') IS NOT NULL DROP TABLE #HocPhi;
IF OBJECT_ID(N'tempdb..#ThanhToan') IS NOT NULL DROP TABLE #ThanhToan;

CREATE TABLE #DangKy (MaDangKy int PRIMARY KEY);
CREATE TABLE #HocPhi (MaHocPhi int PRIMARY KEY);
CREATE TABLE #ThanhToan (MaThanhToan int PRIMARY KEY);

INSERT INTO #DangKy (MaDangKy)
SELECT dk.MaDangKy
FROM dbo.DangKyHoc dk
WHERE dk.MaHocVien IN (SELECT MaHocVien FROM #HvRac);

INSERT INTO #HocPhi (MaHocPhi)
SELECT hp.MaHocPhi
FROM dbo.HocPhi hp
WHERE hp.MaDangKy IN (SELECT MaDangKy FROM #DangKy);

INSERT INTO #ThanhToan (MaThanhToan)
SELECT tt.MaThanhToan
FROM dbo.ThanhToan tt
WHERE tt.MaHocPhi IN (SELECT MaHocPhi FROM #HocPhi);

SELECT N'Đăng ký học' AS HangMuc, COUNT(*) AS SoLuong FROM #DangKy
UNION ALL SELECT N'Học phí', COUNT(*) FROM #HocPhi
UNION ALL SELECT N'Thanh toán', COUNT(*) FROM #ThanhToan;

/* =====================================================================================
   3. XOÁ
   ===================================================================================== */
IF @ChiXem = 1
BEGIN
    PRINT N'>>> Chế độ CHỈ XEM (@ChiXem = 1) — chưa xoá gì.';
END
ELSE IF @SoRac = 0
BEGIN
    PRINT N'>>> Không còn học viên rác. Không làm gì thêm.';
END
ELSE
BEGIN
    BEGIN TRY
        BEGIN TRANSACTION;

        -- 3.1 Tiền: biên nhận → thanh toán → học phí
        DELETE bn FROM dbo.BienNhan bn
        WHERE bn.MaThanhToan IN (SELECT MaThanhToan FROM #ThanhToan);

        DELETE tt FROM dbo.ThanhToan tt
        WHERE tt.MaThanhToan IN (SELECT MaThanhToan FROM #ThanhToan);

        DELETE hp FROM dbo.HocPhi hp
        WHERE hp.MaHocPhi IN (SELECT MaHocPhi FROM #HocPhi);

        -- 3.2 Điểm số → đăng ký học
        DELETE ds FROM dbo.DiemSo ds
        WHERE ds.MaDangKy IN (SELECT MaDangKy FROM #DangKy);

        DELETE dk FROM dbo.DangKyHoc dk
        WHERE dk.MaDangKy IN (SELECT MaDangKy FROM #DangKy);

        -- 3.3 Các bảng gắn trực tiếp học viên
        DELETE dd FROM dbo.DiemDanh dd
        WHERE dd.MaHocVien IN (SELECT MaHocVien FROM #HvRac);

        DELETE kt FROM dbo.KiemTraDauVao kt
        WHERE kt.MaHocVien IN (SELECT MaHocVien FROM #HvRac);

        DELETE p1 FROM dbo.PhanHoiDichVu p1
        WHERE p1.MaHocVien IN (SELECT MaHocVien FROM #HvRac);

        DELETE p2 FROM dbo.PhanHoiKhaoSat p2
        WHERE p2.MaHocVien IN (SELECT MaHocVien FROM #HvRac);

        -- 3.4 Tài khoản: gỡ điểm danh / thông báo liên quan trước
        DELETE dd
        FROM dbo.DiemDanh dd
        WHERE dd.MaTaiKhoanThucHien IN (
            SELECT MaTaiKhoan FROM #HvRac WHERE MaTaiKhoan IS NOT NULL);

        DELETE tb
        FROM dbo.ThongBao tb
        WHERE tb.MaTaiKhoanGui IN (
                  SELECT MaTaiKhoan FROM #HvRac WHERE MaTaiKhoan IS NOT NULL)
           OR tb.MaTaiKhoanNhan IN (
                  SELECT MaTaiKhoan FROM #HvRac WHERE MaTaiKhoan IS NOT NULL);

        -- 3.5 HỌC VIÊN PHẢI XOÁ TRƯỚC TÀI KHOẢN:
        --     FK_HocVien_TaiKhoan là HocVien.MaHocVien → TaiKhoan.TenDangNhap
        --     (xoá tài khoản trước sẽ bị chặn).
        DELETE hv FROM dbo.HocVien hv
        WHERE hv.MaHocVien IN (SELECT MaHocVien FROM #HvRac);

        -- 3.6 Tài khoản
        DELETE t FROM dbo.TaiKhoan t
        WHERE t.TenDangNhap IN (SELECT MaHocVien FROM #HvRac);

        COMMIT TRANSACTION;

        PRINT N'>>> Đã xoá ' + CAST(@SoRac AS nvarchar(10)) + N' học viên rác và toàn bộ dữ liệu liên quan.';

        SELECT r.MaHocVien, r.HoTen FROM #HvRac r ORDER BY r.MaHocVien;
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
   4. KIỂM TRA SAU KHI XOÁ
   ===================================================================================== */
PRINT N'--- KIỂM TRA ---';

SELECT N'Tổng học viên' AS HangMuc, COUNT(*) AS SoLuong FROM dbo.HocVien
UNION ALL
SELECT N'Học viên rác còn lại (phải = 0)', COUNT(*)
FROM dbo.HocVien
WHERE HoTen LIKE N'Ph3 %' OR Email LIKE N'%@test.local'
UNION ALL
SELECT N'Tài khoản học viên', COUNT(*) FROM dbo.TaiKhoan WHERE MaVaiTro = 7
UNION ALL
SELECT N'Đăng ký học', COUNT(*) FROM dbo.DangKyHoc
UNION ALL
SELECT N'Học phí', COUNT(*) FROM dbo.HocPhi;

/* Học viên có đăng ký nhưng mất hồ sơ (phải RỖNG) */
SELECT N'CẢNH BÁO: đăng ký trỏ tới học viên không tồn tại' AS CanhBao, dk.MaDangKy, dk.MaHocVien
FROM dbo.DangKyHoc dk
WHERE NOT EXISTS (SELECT 1 FROM dbo.HocVien hv WHERE hv.MaHocVien = dk.MaHocVien);
