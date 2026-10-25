/* ============================================================
   v14 — ĐỒNG BỘ NGÀY NHẬP HỌC = NGÀY BẮT ĐẦU LỚP HỌC
   ------------------------------------------------------------
   Nghiệp vụ: "Ngày nhập học" của học viên chính là ngày BẮT ĐẦU
   của lớp học mà học viên đã được xếp vào.
   - Học viên chưa được xếp lớp: giữ nguyên giá trị hiện có.
   - Học viên học nhiều lớp: lấy ngày bắt đầu SỚM NHẤT.
   ------------------------------------------------------------
   Chạy 1 lần trên database QuanLyTrungTamNgoaiNgu.
   ============================================================ */

USE QuanLyTrungTamNgoaiNgu;
GO

/* 1. Xem trước dữ liệu sẽ thay đổi */
SELECT
    hv.MaHocVien,
    hv.HoTen,
    hv.NgayNhapHoc                AS NgayNhapHocHienTai,
    x.NgayBatDauLop               AS NgayNhapHocMoi
FROM HocVien hv
JOIN (
    SELECT dk.MaHocVien, MIN(lh.NgayBatDau) AS NgayBatDauLop
    FROM DangKyHoc dk
    JOIN LopHoc lh ON lh.MaLopHoc = dk.MaLopHoc
    WHERE dk.MaLopHoc IS NOT NULL
    GROUP BY dk.MaHocVien
) x ON x.MaHocVien = hv.MaHocVien
WHERE hv.NgayNhapHoc IS NULL
   OR hv.NgayNhapHoc <> x.NgayBatDauLop;
GO

/* 2. Cập nhật */
UPDATE hv
SET NgayNhapHoc = x.NgayBatDauLop
FROM HocVien hv
JOIN (
    SELECT dk.MaHocVien, MIN(lh.NgayBatDau) AS NgayBatDauLop
    FROM DangKyHoc dk
    JOIN LopHoc lh ON lh.MaLopHoc = dk.MaLopHoc
    WHERE dk.MaLopHoc IS NOT NULL
    GROUP BY dk.MaHocVien
) x ON x.MaHocVien = hv.MaHocVien
WHERE hv.NgayNhapHoc IS NULL
   OR hv.NgayNhapHoc <> x.NgayBatDauLop;
GO

/* 3. Kiểm tra lại */
SELECT hv.MaHocVien, hv.HoTen, hv.NgayNhapHoc
FROM HocVien hv
ORDER BY hv.MaHocVien;
GO
