/* =============================================================================
   BỔ SUNG v9 — GẮN GIÁO VIÊN / TRỢ GIẢNG VÀO CÁC CA HỌC ĐÃ SINH
   -----------------------------------------------------------------------------
   VẤN ĐỀ: "Sinh ca học" trước đây KHÔNG chép giáo viên / trợ giảng từ
           PhanCongGiangDay vào CaHoc → cột Giáo viên / Trợ giảng trong màn
           "Ca học thực tế" luôn hiện "—" dù lớp đã được phân công.

   SỬA Ở CODE: SessionStore.GenerateCoreAsync nay tự chép
               Giáo viên chính + Trợ giảng đang phân công vào từng ca mới sinh.
   SCRIPT NÀY: vá các ca ĐÃ sinh trước đó (dữ liệu cũ).

   Ghi chú:
     · Chỉ điền vào ca đang TRỐNG người dạy (NULL) — không ghi đè ca đã thay GV.
     · Chạy lại nhiều lần vẫn an toàn.
   ============================================================================= */

USE QuanLyTrungTamNgoaiNgu;
GO

SET NOCOUNT ON;
GO

/* -----------------------------------------------------------------------------
   XEM TRƯỚC: bao nhiêu ca đang thiếu người dạy
   ----------------------------------------------------------------------------- */
PRINT N'===== CA HỌC ĐANG THIẾU GIÁO VIÊN / TRỢ GIẢNG =====';

SELECT COUNT(*) AS SoCaThieuGiaoVien FROM CaHoc WHERE MaGiaoVienThucTe IS NULL;
SELECT COUNT(*) AS SoCaThieuTroGiang  FROM CaHoc WHERE MaTroGiangThucTe  IS NULL;
GO

BEGIN TRY
    BEGIN TRANSACTION;

    /* --- Giáo viên chính --- */
    UPDATE ch
       SET ch.MaGiaoVienThucTe = pc.MaGiaoVien
      FROM CaHoc ch
     OUTER APPLY
     (
         SELECT TOP 1 p.MaGiaoVien
           FROM PhanCongGiangDay p
          WHERE p.MaLopHoc = ch.MaLopHoc
            AND p.VaiTroPhanCong = N'Giáo viên chính'
            AND p.TrangThai = N'Đang phân công'
            AND p.MaGiaoVien IS NOT NULL
          ORDER BY p.MaPhanCong DESC
     ) pc
     WHERE ch.MaGiaoVienThucTe IS NULL
       AND pc.MaGiaoVien IS NOT NULL;

    PRINT N'Đã gắn giáo viên chính cho ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' ca học.';

    /* --- Trợ giảng --- */
    UPDATE ch
       SET ch.MaTroGiangThucTe = pc.MaGiaoVien
      FROM CaHoc ch
     OUTER APPLY
     (
         SELECT TOP 1 p.MaGiaoVien
           FROM PhanCongGiangDay p
          WHERE p.MaLopHoc = ch.MaLopHoc
            AND p.VaiTroPhanCong = N'Trợ giảng'
            AND p.TrangThai = N'Đang phân công'
            AND p.MaGiaoVien IS NOT NULL
          ORDER BY p.MaPhanCong DESC
     ) pc
     WHERE ch.MaTroGiangThucTe IS NULL
       AND pc.MaGiaoVien IS NOT NULL;

    PRINT N'Đã gắn trợ giảng cho ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' ca học.';

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;

    PRINT N'===== LỖI — ĐÃ ROLLBACK =====';
    PRINT ERROR_MESSAGE();

    THROW;
END CATCH
GO

/* -----------------------------------------------------------------------------
   KIỂM TRA KẾT QUẢ
   ----------------------------------------------------------------------------- */
PRINT N'===== CA HỌC SAU KHI CẬP NHẬT =====';

SELECT ch.MaCaHoc            AS MaCa,
       ch.NgayHoc            AS NgayHoc,
       l.MaLop               AS MaLop,
       gv.HoTen              AS GiaoVien,
       tg.HoTen              AS TroGiang,
       ch.TrangThai          AS TrangThai
  FROM CaHoc ch
  JOIN LopHoc l      ON l.MaLopHoc      = ch.MaLopHoc
  LEFT JOIN GiaoVien gv ON gv.MaGiaoVien = ch.MaGiaoVienThucTe
  LEFT JOIN GiaoVien tg ON tg.MaGiaoVien = ch.MaTroGiangThucTe
 ORDER BY ch.NgayHoc;
GO

PRINT N'=== BỔ SUNG v9 HOÀN TẤT ===';
GO
