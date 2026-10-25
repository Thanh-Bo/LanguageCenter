/* =============================================================================
   CHỈNH TRẠNG THÁI HỌC VIÊN v7 — ĐƯA TẤT CẢ VỀ "ĐANG TƯ VẤN"
   -----------------------------------------------------------------------------
   BỐI CẢNH: sau khi dọn dữ liệu (DonDep_DuLieu_v5.sql) đã xóa toàn bộ
             lớp học / đăng ký / điểm / học phí, nhưng HocVien.TrangThaiHocTap
             vẫn còn giá trị cũ "Đang học" → danh sách học viên hiển thị sai.

   VIỆC CẦN LÀM: đặt lại toàn bộ học viên về trạng thái "Đang tư vấn"
                (đúng DEFAULT của bảng HocVien) và XÓA "Ngày nhập học"
                (vì chưa còn lớp nào — cột NgayNhapHoc cho phép NULL).

   AN TOÀN:
     · Chỉ UPDATE 2 cột TrangThaiHocTap + NgayNhapHoc — không xóa dòng,
       không đổi mã / họ tên / ngày sinh / email / SĐT.
     · Chạy lại nhiều lần được (không còn dòng nào cần đổi thì báo 0 dòng).
   ============================================================================= */

USE QuanLyTrungTamNgoaiNgu;
GO

SET NOCOUNT ON;
GO

/* -----------------------------------------------------------------------------
   BƯỚC 1 — XEM TRƯỚC: đang có bao nhiêu học viên ở mỗi trạng thái
   ----------------------------------------------------------------------------- */
PRINT N'===== TRẠNG THÁI HIỆN TẠI =====';

SELECT TrangThaiHocTap        AS TrangThai,
       COUNT(*)               AS SoHocVien
  FROM HocVien
 GROUP BY TrangThaiHocTap
 ORDER BY COUNT(*) DESC;
GO

/* -----------------------------------------------------------------------------
   BƯỚC 2 — CẬP NHẬT VỀ "ĐANG TƯ VẤN" + XÓA NGÀY NHẬP HỌC
   ----------------------------------------------------------------------------- */
BEGIN TRY
    BEGIN TRANSACTION;

    UPDATE HocVien
       SET TrangThaiHocTap = N'Đang tư vấn',
           NgayNhapHoc     = NULL
     WHERE TrangThaiHocTap <> N'Đang tư vấn'
        OR TrangThaiHocTap IS NULL
        OR NgayNhapHoc IS NOT NULL;

    PRINT N'Đã chỉnh ' + CAST(@@ROWCOUNT AS NVARCHAR(10))
        + N' học viên: trạng thái → "Đang tư vấn", xóa Ngày nhập học.';

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
   BƯỚC 3 — KIỂM TRA KẾT QUẢ (kỳ vọng: chỉ còn 1 dòng "Đang tư vấn")
   ----------------------------------------------------------------------------- */
PRINT N'===== TRẠNG THÁI SAU KHI CHỈNH =====';

SELECT TrangThaiHocTap        AS TrangThai,
       COUNT(*)               AS SoHocVien
  FROM HocVien
 GROUP BY TrangThaiHocTap
 ORDER BY COUNT(*) DESC;
GO

PRINT N'===== DANH SÁCH HỌC VIÊN =====';

SELECT MaHocVien        AS MaHocVien,
       HoTen            AS HoTen,
       NgaySinh         AS NgaySinh,
       TrangThaiHocTap  AS TrangThai,
       NgayNhapHoc      AS NgayNhapHoc
  FROM HocVien
 ORDER BY MaHocVien;
GO

/* -----------------------------------------------------------------------------
   TÙY CHỌN — bỏ comment nếu muốn dọn thêm
   ----------------------------------------------------------------------------- */

-- -----------------------------------------------------------------------------
-- Cũng nên kiểm tra luôn GIÁO VIÊN / TRỢ GIẢNG nếu cần:
-- UPDATE GiaoVien SET TrangThai = N'Đang làm việc' WHERE TrangThai <> N'Đang làm việc';
-- -----------------------------------------------------------------------------
