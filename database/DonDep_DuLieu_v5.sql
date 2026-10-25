/* =============================================================================
   DỌN DẸP DỮ LIỆU v5 — GIỮ LẠI CHƯƠNG TRÌNH HỌC + TÀI KHOẢN
   -----------------------------------------------------------------------------
   GIỮ LẠI:
     · Chương trình học : ChuongTrinh, CapDo, KhoaHoc, LoaiDiem
     · Tài khoản        : VaiTro, TaiKhoan, NhanVien, GiaoVien, HocVien, PhuHuynh,
                          BangCap, ChungChi (hồ sơ đi kèm tài khoản)

   XÓA SẠCH:
     · Lớp học & vận hành   : LopHoc, PhanCongGiangDay, LichHoc, CaHoc
     · Đăng ký & tài chính  : DangKyHoc, HocPhi, ThanhToan, BienNhan
     · Điểm & điểm danh     : DiemSo, DiemDanh
     · Khảo sát / dịch vụ   : KhaoSat, CauHoiKhaoSat, PhanHoiKhaoSat,
                              CauTraLoiKhaoSat, DichVu, PhanHoiDichVu
     · Khác                 : KiemTraDauVao, ThongBao, UuDai, PhongHoc

   AN TOÀN:
     · Toàn bộ nằm trong 1 TRANSACTION + XACT_ABORT → lỗi là tự ROLLBACK.
     · Bước 1 IN ra số dòng hiện có để bạn xem trước.
     · Muốn chạy kiểu "thử" : đổi  COMMIT TRANSACTION  thành  ROLLBACK TRANSACTION

   LƯU Ý: Không thể hoàn tác sau khi COMMIT. Nên backup trước:
          SSMS → chuột phải database → Tasks → Back Up…
   ============================================================================= */

USE QuanLyTrungTamNgoaiNgu;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;
GO

/* -----------------------------------------------------------------------------
   BƯỚC 1 — XEM TRƯỚC: bảng nào đang có bao nhiêu dòng
   ----------------------------------------------------------------------------- */
PRINT N'===== SỐ DÒNG HIỆN TẠI =====';

SELECT t.name                              AS Bang,
       SUM(p.rows)                         AS SoDong
  FROM sys.tables t
  JOIN sys.partitions p
       ON p.object_id = t.object_id
      AND p.index_id IN (0, 1)
 GROUP BY t.name
 ORDER BY t.name;
GO

/* -----------------------------------------------------------------------------
   BƯỚC 1b — CẢNH BÁO: bảng KHÔNG xóa nhưng đang tham chiếu bảng sẽ xóa
   (kết quả RỖNG = an toàn; có dòng nào thì phải xóa bảng đó trước)
   ----------------------------------------------------------------------------- */
PRINT N'===== CẢNH BÁO KHOÁ NGOẠI CHÉO (rỗng = an toàn) =====';

WITH Xoa(Bang) AS
(
    SELECT v.Bang
      FROM (VALUES
            (N'LopHoc'), (N'PhanCongGiangDay'), (N'LichHoc'), (N'CaHoc'),
            (N'DangKyHoc'), (N'HocPhi'), (N'ThanhToan'), (N'BienNhan'),
            (N'DiemSo'), (N'DiemDanh'), (N'KhaoSat'), (N'CauHoiKhaoSat'),
            (N'PhanHoiKhaoSat'), (N'CauTraLoiKhaoSat'), (N'DichVu'),
            (N'PhanHoiDichVu'), (N'KiemTraDauVao'), (N'ThongBao'),
            (N'UuDai'), (N'PhongHoc')
           ) v(Bang)
)
SELECT fk.name                              AS KhoaNgoai,
       OBJECT_NAME(fk.parent_object_id)     AS BangThamChieu,
       OBJECT_NAME(fk.referenced_object_id) AS BangBiXoa
  FROM sys.foreign_keys fk
 WHERE OBJECT_NAME(fk.referenced_object_id) IN (SELECT Bang FROM Xoa)
   AND OBJECT_NAME(fk.parent_object_id)     NOT IN (SELECT Bang FROM Xoa);
GO

/* -----------------------------------------------------------------------------
   BƯỚC 2 — XÓA DỮ LIỆU (thứ tự con → cha)
   ----------------------------------------------------------------------------- */
BEGIN TRY
    BEGIN TRANSACTION;

    /* --- Khảo sát --- */
    DELETE FROM CauTraLoiKhaoSat;
    DELETE FROM PhanHoiKhaoSat;
    DELETE FROM CauHoiKhaoSat;
    DELETE FROM KhaoSat;

    /* --- Dịch vụ --- */
    DELETE FROM PhanHoiDichVu;
    DELETE FROM DichVu;

    /* --- Thông báo (phụ thuộc LopHoc + TaiKhoan → phải xóa TRƯỚC LopHoc) --- */
    DELETE FROM ThongBao;

    /* --- Tài chính --- */
    DELETE FROM BienNhan;
    DELETE FROM ThanhToan;
    DELETE FROM HocPhi;

    /* --- Điểm & điểm danh --- */
    DELETE FROM DiemSo;
    DELETE FROM DiemDanh;

    /* --- Lịch & ca học --- */
    DELETE FROM CaHoc;
    DELETE FROM LichHoc;

    /* --- Đăng ký học --- */
    DELETE FROM DangKyHoc;

    /* --- Phân công giảng dạy & lớp học --- */
    DELETE FROM PhanCongGiangDay;
    DELETE FROM LopHoc;

    /* --- Còn lại --- */
    DELETE FROM KiemTraDauVao;
    DELETE FROM UuDai;
    DELETE FROM PhongHoc;

    COMMIT TRANSACTION;

    PRINT N'===== ĐÃ XÓA XONG DỮ LIỆU — GIỮ LẠI CHƯƠNG TRÌNH + TÀI KHOẢN =====';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
    BEGIN
        ROLLBACK TRANSACTION;
    END

    PRINT N'===== LỖI — ĐÃ ROLLBACK, DATABASE KHÔNG THAY ĐỔI =====';
    PRINT N'Mã lỗi : ' + CAST(ERROR_NUMBER() AS NVARCHAR(20));
    PRINT N'Nội dung: ' + ERROR_MESSAGE();

    THROW;
END CATCH
GO

/* -----------------------------------------------------------------------------
   BƯỚC 3 — ĐẶT LẠI IDENTITY (bản ghi mới sẽ bắt đầu từ 1)
   ----------------------------------------------------------------------------- */
PRINT N'===== RESET IDENTITY =====';

DBCC CHECKIDENT ('CauTraLoiKhaoSat', RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('PhanHoiKhaoSat',    RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('CauHoiKhaoSat',     RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('KhaoSat',           RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('PhanHoiDichVu',     RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('DichVu',            RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('BienNhan',          RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('ThanhToan',         RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('HocPhi',            RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('DiemSo',            RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('DiemDanh',          RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('CaHoc',             RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('LichHoc',           RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('DangKyHoc',         RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('PhanCongGiangDay',  RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('LopHoc',            RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('KiemTraDauVao',     RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('ThongBao',          RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('UuDai',             RESEED, 0) WITH NO_INFOMSGS;
DBCC CHECKIDENT ('PhongHoc',          RESEED, 0) WITH NO_INFOMSGS;
GO

/* -----------------------------------------------------------------------------
   BƯỚC 4 — KIỂM TRA KẾT QUẢ
   ----------------------------------------------------------------------------- */
PRINT N'===== SỐ DÒNG SAU KHI DỌN =====';

SELECT t.name                              AS Bang,
       SUM(p.rows)                         AS SoDong
  FROM sys.tables t
  JOIN sys.partitions p
       ON p.object_id = t.object_id
      AND p.index_id IN (0, 1)
 GROUP BY t.name
 ORDER BY SUM(p.rows) DESC, t.name;
GO

PRINT N'--- Danh mục giữ lại ---';

SELECT N'Chương trình' AS Nhom, COUNT(*) AS SoDong FROM ChuongTrinh
UNION ALL SELECT N'Cấp độ',       COUNT(*) FROM CapDo
UNION ALL SELECT N'Khóa học',     COUNT(*) FROM KhoaHoc
UNION ALL SELECT N'Loại điểm',    COUNT(*) FROM LoaiDiem
UNION ALL SELECT N'Tài khoản',    COUNT(*) FROM TaiKhoan
UNION ALL SELECT N'Nhân viên',    COUNT(*) FROM NhanVien
UNION ALL SELECT N'Giáo viên',    COUNT(*) FROM GiaoVien
UNION ALL SELECT N'Học viên',     COUNT(*) FROM HocVien;
GO

/* Kiểm tra toàn vẹn khoá ngoại — kết quả rỗng là database sạch */
PRINT N'===== KIỂM TRA KHOÁ NGOẠI (rỗng = OK) =====';

DBCC CHECKCONSTRAINTS WITH ALL_CONSTRAINTS;
GO

/* -----------------------------------------------------------------------------
   TÙY CHỌN — bỏ comment nếu muốn dọn thêm
   ----------------------------------------------------------------------------- */

-- Đưa toàn bộ học viên về trạng thái "Đang tư vấn" (vì đã xóa hết đăng ký/lớp).
-- UPDATE HocVien SET TrangThaiHocTap = N'Đang tư vấn' WHERE TrangThaiHocTap = N'Đang học';

-- Xóa các tài khoản KHÔNG có hồ sơ tương ứng (tài khoản mồ côi kiểu HV008/HV009).
-- DELETE tk
--   FROM TaiKhoan tk
--   JOIN VaiTro vt ON vt.MaVaiTro = tk.MaVaiTro
--  WHERE (vt.TenVaiTro = N'Học viên'
--         AND NOT EXISTS (SELECT 1 FROM HocVien  hv WHERE hv.MaHocVien  = tk.TenDangNhap))
--     OR (vt.TenVaiTro IN (N'Giáo viên', N'Trợ giảng')
--         AND NOT EXISTS (SELECT 1 FROM GiaoVien gv WHERE gv.MaGiaoVien = tk.TenDangNhap))
--     OR (vt.TenVaiTro IN (N'Quản trị viên', N'Nhân viên học vụ',
--                          N'Nhân viên tư vấn', N'Nhân viên kế toán')
--         AND NOT EXISTS (SELECT 1 FROM NhanVien nv WHERE nv.MaNhanVien = tk.TenDangNhap));
GO
