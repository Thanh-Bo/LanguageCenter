/* ============================================================================================
   KiemTra_DiemDanh.sql
   --------------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Trả lời câu hỏi: "Hôm nay có ca học nào điểm danh được không? Trợ giảng nào phụ trách?"

   QUY TẮC ĐIỂM DANH CỦA BACKEND (AttendanceService.EvaluatePermissionAsync)
       1. CaHoc.NgayHoc <= HÔM NAY           (không cho điểm danh trước ngày học)
       2. CaHoc.TrangThai <> N'Đã hủy'       (Chưa diễn ra / Đang diễn ra / Đã hoàn thành đều OK)
       3. Người bấm phải là:
            - Quản trị viên / Nhân viên học vụ  -> mọi ca
            - Trợ giảng là MaTroGiangThucTe của ca
            - Hoặc có dòng PhanCongGiangDay N'Đang phân công' cho lớp đó
          GIÁO VIÊN (vai trò Giáo viên) KHÔNG được điểm danh.
       4. Lưu điểm danh xong -> ca tự chốt sang N'Đã hoàn thành'.

   CÁCH CHẠY
       Mở bằng SSMS, chạy toàn bộ file. Không sửa dữ liệu (chỉ SELECT).
   ============================================================================================ */

SET NOCOUNT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

DECLARE @HomNay date = CAST(GETDATE() AS date);

/* --------------------------------------------------------------------------------------------
   0. HÔM NAY LÀ THỨ MẤY
   -------------------------------------------------------------------------------------------- */
PRINT N'>>> HÔM NAY: ' + CONVERT(nvarchar(10), @HomNay, 103)
      + N' — ' + DATENAME(weekday, @HomNay)
      + N' (thứ ISO = ' + CAST(((DATEDIFF(day, '19000101', @HomNay) % 7) + 7) % 7 + 1 AS nvarchar(2)) + N')';

/* --------------------------------------------------------------------------------------------
   1. CA HỌC HÔM NAY (mọi lớp) — nhìn cột CoTheDiemDanh là biết ngay
   -------------------------------------------------------------------------------------------- */
PRINT N'';
PRINT N'=== 1. CA HỌC HÔM NAY ===';

SELECT
    ch.MaCaHoc,
    lh.MaKhoaHoc,
    lh.MaLop,
    lh.TenLop,
    ch.SoBuoi,
    ch.LoaiBuoi,
    CONVERT(varchar(5), ch.GioBatDau, 108) + N'–' + CONVERT(varchar(5), ch.GioKetThuc, 108) AS GioHoc,
    ph.MaPhong,
    ch.MaGiaoVienThucTe                       AS MaGiaoVien,
    gv.HoTen                                  AS TenGiaoVien,
    ch.MaTroGiangThucTe                       AS MaTroGiang,
    tg.HoTen                                  AS TenTroGiang,
    ch.TrangThai,
    (SELECT COUNT(*)
       FROM dbo.DangKyHoc dk
      WHERE dk.MaLopHoc = ch.MaLopHoc
        AND dk.TrangThaiDangKy IN (N'Đã xếp lớp', N'Hoàn thành'))                AS SoHocVien,
    CASE
        WHEN ch.TrangThai = N'Đã hủy'      THEN N'KHÔNG — ca đã hủy'
        WHEN ch.MaTroGiangThucTe IS NULL   THEN N'KHÔNG — ca chưa gán trợ giảng'
        ELSE                                    N'CÓ — trợ giảng bấm được'
    END                                       AS CoTheDiemDanh
FROM dbo.CaHoc ch
JOIN dbo.LopHoc  lh ON lh.MaLopHoc    = ch.MaLopHoc
LEFT JOIN dbo.PhongHoc ph ON ph.MaPhongHoc = ch.MaPhongHoc
LEFT JOIN dbo.GiaoVien gv ON gv.MaGiaoVien = ch.MaGiaoVienThucTe
LEFT JOIN dbo.GiaoVien tg ON tg.MaGiaoVien = ch.MaTroGiangThucTe
WHERE ch.NgayHoc = @HomNay
ORDER BY ch.GioBatDau, lh.MaLop;

/* --------------------------------------------------------------------------------------------
   2. TUẦN NÀY (Thứ Hai -> Chủ Nhật) — lớp nào đang mở, buổi nào sắp tới
   -------------------------------------------------------------------------------------------- */
PRINT N'';
PRINT N'=== 2. CA HỌC TRONG TUẦN NÀY ===';

/* 1900-01-01 là Thứ Hai → DATEDIFF % 7 cho biết đã lệch mấy ngày so với Thứ Hai */
DECLARE @ThuHai date =
    DATEADD(day, -(((DATEDIFF(day, '19000101', @HomNay) % 7) + 7) % 7), @HomNay);

SELECT
    ch.NgayHoc,
    DATENAME(weekday, ch.NgayHoc)             AS Thu,
    lh.MaLop,
    lh.TenLop,
    lh.TrangThai                              AS TrangThaiLop,
    ch.SoBuoi,
    ch.LoaiBuoi,
    CONVERT(varchar(5), ch.GioBatDau, 108) + N'–' + CONVERT(varchar(5), ch.GioKetThuc, 108) AS GioHoc,
    ch.MaGiaoVienThucTe                       AS MaGiaoVien,
    ch.MaTroGiangThucTe                       AS MaTroGiang,
    ch.TrangThai,
    CASE WHEN ch.NgayHoc <  @HomNay THEN N'Đã qua'
         WHEN ch.NgayHoc =  @HomNay THEN N'HÔM NAY'
         ELSE                            N'Chưa tới' END AS ViTriNgay
FROM dbo.CaHoc ch
JOIN dbo.LopHoc lh ON lh.MaLopHoc = ch.MaLopHoc
WHERE ch.NgayHoc BETWEEN @ThuHai AND DATEADD(day, 6, @ThuHai)
ORDER BY ch.NgayHoc, ch.GioBatDau;

/* --------------------------------------------------------------------------------------------
   3. CA GẦN NHẤT CÓ THỂ ĐIỂM DANH NGAY (gợi ý ca để test)
      = ca đã qua hoặc hôm nay, chưa hủy, có trợ giảng
   -------------------------------------------------------------------------------------------- */
PRINT N'';
PRINT N'=== 3. GỢI Ý CA TEST ĐIỂM DANH (3 ca gần nhất) ===';

SELECT TOP (3)
    ch.MaCaHoc,
    ch.NgayHoc,
    lh.MaLop,
    lh.TenLop,
    ch.SoBuoi,
    ch.TrangThai,
    ch.MaTroGiangThucTe AS DangNhapTroGiang,
    tg.HoTen            AS TenTroGiang,
    N'/giaovien/diem-danh?ca=' + CAST(ch.MaCaHoc AS nvarchar(10))
                        + N'&ngay=' + CONVERT(nvarchar(10), ch.NgayHoc, 23) AS DuongDanWeb
FROM dbo.CaHoc ch
JOIN dbo.LopHoc  lh ON lh.MaLopHoc = ch.MaLopHoc
LEFT JOIN dbo.GiaoVien tg ON tg.MaGiaoVien = ch.MaTroGiangThucTe
WHERE ch.NgayHoc <= @HomNay
  AND ch.TrangThai <> N'Đã hủy'
  AND ch.MaTroGiangThucTe IS NOT NULL
ORDER BY ch.NgayHoc DESC, ch.SoBuoi DESC;

/* --------------------------------------------------------------------------------------------
   4. TRỢ GIẢNG VÀ LỚP ĐANG PHỤ TRÁCH
   -------------------------------------------------------------------------------------------- */
PRINT N'';
PRINT N'=== 4. TRỢ GIẢNG ĐANG PHÂN CÔNG ===';

SELECT
    pc.MaGiaoVien                             AS MaTroGiang,
    gv.HoTen                                  AS TenTroGiang,
    lh.MaLop,
    lh.TenLop,
    lh.TrangThai                              AS TrangThaiLop,
    pc.VaiTroPhanCong,
    pc.TrangThai                              AS TrangThaiPhanCong,
    (SELECT COUNT(*)
       FROM dbo.CaHoc c
      WHERE c.MaLopHoc = lh.MaLopHoc
        AND c.NgayHoc <= @HomNay
        AND c.TrangThai <> N'Đã hủy')                                              AS SoCaDaToi,
    (SELECT COUNT(*)
       FROM dbo.CaHoc c
      WHERE c.MaLopHoc = lh.MaLopHoc
        AND c.NgayHoc > @HomNay
        AND c.TrangThai <> N'Đã hủy')                                              AS SoCaSapToi
FROM dbo.PhanCongGiangDay pc
JOIN dbo.LopHoc  lh ON lh.MaLopHoc  = pc.MaLopHoc
JOIN dbo.GiaoVien gv ON gv.MaGiaoVien = pc.MaGiaoVien
WHERE pc.VaiTroPhanCong = N'Trợ giảng'
  AND pc.TrangThai      = N'Đang phân công'
ORDER BY pc.MaGiaoVien, lh.MaLop;
