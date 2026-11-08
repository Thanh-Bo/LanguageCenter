/* =====================================================================================
   KiemTraTrungEmail_v34.sql
   -------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Kiểm tra dữ liệu EMAIL trong bảng dbo.TaiKhoan có bị trùng hoặc sai định dạng
       hay không, và xác nhận ràng buộc UNIQUE trên cột Email đang tồn tại.

   BỐI CẢNH
       Backend đã chặn trùng email ở 3 lớp:
         1) Application  : AccountStore.EmailExistsAsync(...)  → trả 409 Conflict
         2) Database     : UNIQUE INDEX trên TaiKhoan.Email     → chốt cuối, chống race
         3) EF provision : lỗi 2601/2627 → ProvisionFailureReason.EmailAlreadyExists
       Script này dùng để SOÁT dữ liệu đã có (trường hợp dữ liệu nạp tay / nạp từ file).

   CHỈ ĐỌC — không sửa, không xoá gì.
   ===================================================================================== */

SET NOCOUNT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

PRINT N'=====================================================================';
PRINT N'  v34 — KIỂM TRA EMAIL TRÙNG / SAI ĐỊNH DẠNG';
PRINT N'=====================================================================';

/* -------------------------------------------------------------------------------------
   1. RÀNG BUỘC UNIQUE CÓ ĐANG TỒN TẠI KHÔNG
   ------------------------------------------------------------------------------------- */
PRINT N'--- 1. INDEX UNIQUE TRÊN CỘT Email ---';

SELECT i.name                                   AS TenIndex,
       i.is_unique                              AS DuyNhat,
       i.is_unique_constraint                   AS LaRangBuocUnique,
       COL_NAME(ic.object_id, ic.column_id)     AS Cot
FROM sys.indexes i
JOIN sys.index_columns ic
     ON ic.object_id = i.object_id
    AND ic.index_id  = i.index_id
WHERE i.object_id = OBJECT_ID(N'dbo.TaiKhoan')
  AND COL_NAME(ic.object_id, ic.column_id) = N'Email';

PRINT N'  (Kỳ vọng: có ít nhất 1 dòng với DuyNhat = 1)';
PRINT N'';

/* -------------------------------------------------------------------------------------
   2. TRÙNG TUYỆT ĐỐI (phải RỖNG nếu index UNIQUE đang hoạt động)
   ------------------------------------------------------------------------------------- */
PRINT N'--- 2. EMAIL TRÙNG TUYỆT ĐỐI ---';

SELECT Email,
       COUNT(*)                AS SoTaiKhoan,
       STRING_AGG(TenDangNhap, N', ') AS CacTaiKhoan
FROM dbo.TaiKhoan
GROUP BY Email
HAVING COUNT(*) > 1;

PRINT N'  (Kỳ vọng: 0 dòng)';
PRINT N'';

/* -------------------------------------------------------------------------------------
   3. TRÙNG KHI BỎ QUA HOA/THƯỜNG VÀ KHOẢNG TRẮNG
      SQL Server thường cài collation CI (case-insensitive) nên mục 2 cũng đã bắt được,
      nhưng nếu database dùng collation CS thì phép so sánh LOWER dưới đây mới phát hiện.
   ------------------------------------------------------------------------------------- */
PRINT N'--- 3. EMAIL TRÙNG KHI BỎ QUA HOA/THƯỜNG + KHOẢNG TRẮNG ---';

SELECT LOWER(LTRIM(RTRIM(Email)))       AS EmailChuanHoa,
       COUNT(*)                          AS SoTaiKhoan,
       STRING_AGG(TenDangNhap, N', ')    AS CacTaiKhoan,
       STRING_AGG(Email, N' | ')         AS CacEmailThucTe
FROM dbo.TaiKhoan
GROUP BY LOWER(LTRIM(RTRIM(Email)))
HAVING COUNT(*) > 1;

PRINT N'  (Kỳ vọng: 0 dòng)';
PRINT N'';

/* -------------------------------------------------------------------------------------
   4. EMAIL SAI ĐỊNH DẠNG
      Quy tắc tối thiểu: chuỗi@chuỗi.chuỗi, không có khoảng trắng.
   ------------------------------------------------------------------------------------- */
PRINT N'--- 4. EMAIL SAI ĐỊNH DẠNG ---';

SELECT tk.TenDangNhap,
       tk.Email,
       vt.TenVaiTro
FROM dbo.TaiKhoan tk
JOIN dbo.VaiTro vt ON vt.MaVaiTro = tk.MaVaiTro
WHERE tk.Email NOT LIKE N'%_@_%._%'
   OR tk.Email LIKE N'% %'
   OR tk.Email <> LTRIM(RTRIM(tk.Email));

PRINT N'  (Kỳ vọng: 0 dòng)';
PRINT N'';

/* -------------------------------------------------------------------------------------
   5. TOÀN BỘ EMAIL THEO VAI TRÒ — để soát bằng mắt những cặp GẦN GIỐNG
      (ví dụ nhhv002@... và nhvh002@... là HAI chuỗi KHÁC NHAU nên hệ thống cho qua)
   ------------------------------------------------------------------------------------- */
PRINT N'--- 5. DANH SÁCH EMAIL THEO VAI TRÒ ---';

SELECT vt.TenVaiTro,
       tk.TenDangNhap,
       tk.Email,
       tk.TrangThaiTaiKhoan,
       tk.NgayTao
FROM dbo.TaiKhoan tk
JOIN dbo.VaiTro vt ON vt.MaVaiTro = tk.MaVaiTro
ORDER BY vt.TenVaiTro, tk.Email;

PRINT N'';

/* -------------------------------------------------------------------------------------
   6. GỢI Ý: CÁC CẶP EMAIL CHỈ KHÁC NHAU Ở THỨ TỰ KÝ TỰ (hoán vị) — dấu hiệu gõ nhầm
      Cách làm: so sánh phần trước @ sau khi sắp xếp lại các ký tự theo alphabet.
   ------------------------------------------------------------------------------------- */
PRINT N'--- 6. EMAIL CÓ CÙNG TẬP KÝ TỰ (nghi gõ nhầm / hoán vị) ---';

WITH ChuanHoa AS
(
    SELECT tk.TenDangNhap,
           tk.Email,
           LEFT(tk.Email, CHARINDEX(N'@', tk.Email) - 1)                          AS PhanTruoc,
           SUBSTRING(tk.Email, CHARINDEX(N'@', tk.Email), 255)                     AS PhanSau
    FROM dbo.TaiKhoan tk
    WHERE CHARINDEX(N'@', tk.Email) > 0
),
SapXep AS
(
    SELECT TenDangNhap,
           Email,
           PhanSau,
           -- Sắp xếp các ký tự của phần trước @ theo alphabet (đệ quy đơn giản bằng STRING_AGG trên bảng số)
           (SELECT STRING_AGG(k.ky_tu, N'') WITHIN GROUP (ORDER BY k.ky_tu)
            FROM (SELECT SUBSTRING(PhanTruoc, v.number + 1, 1) AS ky_tu
                  FROM (SELECT TOP (200) ROW_NUMBER() OVER (ORDER BY (SELECT NULL)) - 1 AS number
                        FROM sys.all_objects) v
                  WHERE v.number < LEN(PhanTruoc)) k)                              AS PhanTruocDaSapXep
    FROM ChuanHoa
)
SELECT a.Email   AS Email1,
       b.Email   AS Email2,
       a.TenDangNhap AS TaiKhoan1,
       b.TenDangNhap AS TaiKhoan2
FROM SapXep a
JOIN SapXep b
     ON a.PhanTruocDaSapXep = b.PhanTruocDaSapXep
    AND a.PhanSau = b.PhanSau
    AND a.Email < b.Email;

PRINT N'  (Kỳ vọng: 0 dòng. Có dòng = 2 email khác nhau về thứ tự ký tự → nhiều khả năng gõ nhầm)';
PRINT N'';

PRINT N'=== HOÀN TẤT v34 ===';
GO
