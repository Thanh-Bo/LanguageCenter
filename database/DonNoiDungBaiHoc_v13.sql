/* =============================================================================
   DỌN NỘI DUNG v13 — BỎ TIỀN TỐ "Buổi N — " BỊ LẶP
   -----------------------------------------------------------------------------
   VÌ SAO: bản seed v10 đã ghi TieuDe dạng "Buổi 1 — Giới thiệu ...".
           Khi sinh ca học, hệ thống lại ghép thêm "Buổi 1 — " ở trước
           → nội dung buổi bị lặp: "Buổi 1 — Buổi 1 — Giới thiệu ...".

   GỒM:
     1. NoiDungBaiHoc : bỏ tiền tố "Buổi N — " trong TieuDe (giữ tiêu đề sạch)
     2. CaHoc         : bỏ tiền tố lặp thứ hai trong NoiDungBaiHoc (nếu có)
     3. Kiểm tra

   CHẠY 1 LẦN — chạy lại vẫn an toàn.
   ============================================================================= */

USE QuanLyTrungTamNgoaiNgu;
GO

SET NOCOUNT ON;
GO

/* =============================================================================
   1. NOIDUNGBAIHOC — bỏ tiền tố "Buổi N — " trong tiêu đề
   ============================================================================= */

PRINT N'===== BƯỚC 1: Chuẩn hóa TieuDe trong NoiDungBaiHoc =====';

UPDATE NoiDungBaiHoc
   SET TieuDe = LTRIM(RTRIM(SUBSTRING(
        TieuDe,
        LEN(N'Buổi ' + CAST(SoBuoi AS NVARCHAR(10)) + N' — ') + 1,
        200)))
 WHERE TieuDe LIKE N'Buổi ' + CAST(SoBuoi AS NVARCHAR(10)) + N' — %';

PRINT N'  - Đã bỏ tiền tố cho ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' buổi.';
GO

/* =============================================================================
   2. CAHOC — bỏ tiền tố lặp "Buổi N — Buổi N — "
   ============================================================================= */

PRINT N'===== BƯỚC 2: Bỏ tiền tố lặp trong CaHoc.NoiDungBaiHoc =====';

-- "Buổi 1 — Buổi 1 — Giới thiệu" → "Buổi 1 — Giới thiệu"
-- (bỏ phần giữa dấu '— ' thứ nhất và thứ hai)
UPDATE CaHoc
   SET NoiDungBaiHoc = LTRIM(SUBSTRING(
        NoiDungBaiHoc,
        CHARINDEX(N'— ', NoiDungBaiHoc, CHARINDEX(N'— ', NoiDungBaiHoc) + 2) + 2,
        1000))
 WHERE NoiDungBaiHoc LIKE N'Buổi %— Buổi %';

PRINT N'  - Đã chuẩn hóa ' + CAST(@@ROWCOUNT AS NVARCHAR(10)) + N' buổi học.';
GO

/* =============================================================================
   3. KIỂM TRA
   ============================================================================= */

PRINT N'===== KẾT QUẢ =====';

SELECT TOP (30)
       k.TenKhoaHoc,
       n.SoBuoi,
       n.TieuDe,
       n.NoiDung,
       n.MucTieu
  FROM NoiDungBaiHoc n
  JOIN KhoaHoc k ON k.MaKhoaHoc = n.MaKhoaHoc
 ORDER BY k.TenKhoaHoc, n.SoBuoi;
GO

PRINT N'=== DỌN NỘI DUNG v13 HOÀN TẤT ===';
GO
