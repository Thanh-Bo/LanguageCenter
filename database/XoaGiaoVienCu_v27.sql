/* =====================================================================================
   XoaGiaoVienCu_v27.sql
   -------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Xoá hẳn bộ giáo viên / trợ giảng CŨ của khoá IELTS Advanced (IELTS-A-01) —
       những người đã được chuyển sang trạng thái N'Đã nghỉ việc' từ script v25:

           GV001, GV002, GV009, GV011, GV012, GV013, TG001, TG003

       Sau v25, lớp IELTS-A-01 đã do bộ mới phụ trách (GV014 Hưng + GV027 Michael
       + TG005 Ánh), nên nhóm cũ không còn phân công nào.

   ⚠️ CẢNH BÁO QUAN TRỌNG
       Nếu các giáo viên này ĐÃ TỪNG NHẬP ĐIỂM (bảng DiemSo), xoá họ sẽ kéo theo
       việc xoá các dòng điểm đó (FK_DiemSo_GiaoVien).
       Script mặc định DỪNG LẠI và liệt kê số dòng điểm bị ảnh hưởng.
       Muốn xoá luôn cả điểm → đổi cờ bên dưới thành 1.

   KHÔNG ĐỘNG TỚI
       • Học viên, đăng ký học, học phí, thanh toán, biên nhận.
       • Ca học (chỉ NULL hoá tham chiếu nếu còn).

   AN TOÀN
       • 1 transaction + XACT_ABORT.
       • Idempotent: chạy lại không lỗi khi hồ sơ đã bị xoá.
   ===================================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

/* Đổi thành 1 nếu chấp nhận xoá luôn điểm số do nhóm giáo viên cũ đã nhập. */
DECLARE @XoaCaDiemSo bit = 0;

DECLARE @Cu TABLE (Ma varchar(20) PRIMARY KEY);

INSERT INTO @Cu (Ma) VALUES
    (N'GV001'), (N'GV002'), (N'GV009'), (N'GV011'),
    (N'GV012'), (N'GV013'), (N'TG001'), (N'TG003');

DECLARE @ConTonTai int;
SELECT @ConTonTai = COUNT(*) FROM dbo.GiaoVien WHERE MaGiaoVien IN (SELECT Ma FROM @Cu);

IF @ConTonTai = 0
BEGIN
    PRINT N'>>> Không còn hồ sơ nào trong danh sách — script đã chạy trước đó.';
END
ELSE
BEGIN
    DECLARE @SoDiemSo int;
    SELECT @SoDiemSo = COUNT(*) FROM dbo.DiemSo WHERE MaGiaoVien IN (SELECT Ma FROM @Cu);

    IF @SoDiemSo > 0 AND @XoaCaDiemSo = 0
    BEGIN
        PRINT N'>>> DỪNG: nhóm giáo viên cũ còn ' + CAST(@SoDiemSo AS nvarchar(10))
              + N' dòng điểm trong bảng DiemSo.';
        PRINT N'    Nếu chấp nhận xoá luôn số điểm này, đổi @XoaCaDiemSo = 1 rồi chạy lại.';

        SELECT N'Điểm sẽ bị xoá' AS CanhBao,
               ds.MaGiaoVien, gv.HoTen, COUNT(*) AS SoDongDiem
        FROM dbo.DiemSo ds
        JOIN dbo.GiaoVien gv ON gv.MaGiaoVien = ds.MaGiaoVien
        WHERE ds.MaGiaoVien IN (SELECT Ma FROM @Cu)
        GROUP BY ds.MaGiaoVien, gv.HoTen
        ORDER BY ds.MaGiaoVien;
    END
    ELSE
    BEGIN
        BEGIN TRY
            BEGIN TRANSACTION;

            -- gỡ tham chiếu trên ca học
            UPDATE ch SET MaGiaoVienThucTe = NULL
            FROM dbo.CaHoc ch
            WHERE ch.MaGiaoVienThucTe IN (SELECT Ma FROM @Cu);

            UPDATE ch SET MaTroGiangThucTe = NULL
            FROM dbo.CaHoc ch
            WHERE ch.MaTroGiangThucTe IN (SELECT Ma FROM @Cu);

            -- dữ liệu phụ thuộc
            IF @XoaCaDiemSo = 1
                DELETE ds FROM dbo.DiemSo ds WHERE ds.MaGiaoVien IN (SELECT Ma FROM @Cu);

            DELETE pc FROM dbo.PhanCongGiangDay pc WHERE pc.MaGiaoVien IN (SELECT Ma FROM @Cu);
            DELETE bc FROM dbo.BangCap bc WHERE bc.MaGiaoVien IN (SELECT Ma FROM @Cu);
            DELETE cc FROM dbo.ChungChi cc WHERE cc.MaGiaoVien IN (SELECT Ma FROM @Cu);

            -- thông báo / điểm danh do tài khoản cũ thực hiện
            DELETE dd
            FROM dbo.DiemDanh dd
            WHERE dd.MaTaiKhoanThucHien IN (
                SELECT t.MaTaiKhoan FROM dbo.TaiKhoan t
                WHERE t.TenDangNhap IN (SELECT Ma FROM @Cu));

            DELETE tb
            FROM dbo.ThongBao tb
            WHERE tb.MaTaiKhoanGui IN (
                      SELECT t.MaTaiKhoan FROM dbo.TaiKhoan t
                      WHERE t.TenDangNhap IN (SELECT Ma FROM @Cu))
               OR tb.MaTaiKhoanNhan IN (
                      SELECT t.MaTaiKhoan FROM dbo.TaiKhoan t
                      WHERE t.TenDangNhap IN (SELECT Ma FROM @Cu));

            -- hồ sơ + tài khoản
            DELETE gv FROM dbo.GiaoVien gv WHERE gv.MaGiaoVien IN (SELECT Ma FROM @Cu);
            DELETE t  FROM dbo.TaiKhoan  t  WHERE t.TenDangNhap IN (SELECT Ma FROM @Cu);

            COMMIT TRANSACTION;

            PRINT N'>>> Đã xoá ' + CAST(@ConTonTai AS nvarchar(10)) + N' hồ sơ giáo viên/trợ giảng cũ.';
        END TRY
        BEGIN CATCH
            IF @@TRANCOUNT > 0
                ROLLBACK TRANSACTION;

            PRINT N'LỖI: ' + ERROR_MESSAGE() + N' (dòng ' + CAST(ERROR_LINE() AS nvarchar(10)) + N')';
            THROW;
        END CATCH
    END
END
GO

/* Kiểm tra nhanh sau khi chạy */
SELECT N'Giáo viên đang làm việc' AS HangMuc, COUNT(*) AS SoLuong
FROM dbo.GiaoVien WHERE TrangThai = N'Đang làm việc'
UNION ALL
SELECT N'Tổng hồ sơ giáo viên/trợ giảng', COUNT(*) FROM dbo.GiaoVien
UNION ALL
SELECT N'Ca học chưa gán giáo viên', COUNT(*) FROM dbo.CaHoc WHERE MaGiaoVienThucTe IS NULL
UNION ALL
SELECT N'Ca học chưa gán trợ giảng', COUNT(*) FROM dbo.CaHoc WHERE MaTroGiangThucTe IS NULL;
