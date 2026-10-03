/* ============================================================================
   KHÔI PHỤC TÀI KHOẢN QUẢN TRỊ VIÊN
   ----------------------------------------------------------------------------
   Dùng khi đã xóa hết tài khoản Quản trị viên → không còn admin nào để gọi API.
   File này TỰ KIỂM TRA trước khi ghi → CHẠY LẠI NHIỀU LẦN VẪN AN TOÀN.

   Nguyên tắc:
   - MaNhanVien  = TenDangNhap  (đúng quy ước của hệ thống)
   - Mật khẩu: COPY MatKhauHash từ tài khoản NVHV001 (không tạo hash giả).
     => 3 admin mới sẽ đăng nhập được bằng đúng mật khẩu của NVHV001.
   - TrangThaiTaiKhoan và TrangThaiLamViec KHÔNG truyền → dùng DEFAULT của DB
     (N'Hoạt động' / N'Đang làm việc'), tránh lỗi CHECK constraint.

   Chạy trong SSMS / Azure Data Studio, kết nối tới localhost\SQLEXPRESS.
   ============================================================================ */

USE QuanLyTrungTamNgoaiNgu;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

/* ============================================================================
   BƯỚC 0 — KIỂM TRA HIỆN TRẠNG (đọc kết quả trước khi chạy bước 1)
   ============================================================================ */

PRINT N'=== BƯỚC 0: HIỆN TRẠNG ===';

SELECT
    SoQuanTriVienHienCo   = (SELECT COUNT(*)
                             FROM TaiKhoan t
                             JOIN VaiTro  v ON v.MaVaiTro = t.MaVaiTro
                             WHERE v.TenVaiTro = N'Quản trị viên'),
    MaVaiTroQuanTri       = (SELECT MaVaiTro FROM VaiTro WHERE TenVaiTro = N'Quản trị viên'),
    CoNVHV001             = (SELECT COUNT(*) FROM TaiKhoan WHERE TenDangNhap = 'NVHV001'),
    CoADMIN001            = (SELECT COUNT(*) FROM TaiKhoan WHERE TenDangNhap = 'ADMIN001'),
    CoADMIN002            = (SELECT COUNT(*) FROM TaiKhoan WHERE TenDangNhap = 'ADMIN002'),
    CoADMIN003            = (SELECT COUNT(*) FROM TaiKhoan WHERE TenDangNhap = 'ADMIN003'),
    EmailAdminDangDung    = (SELECT COUNT(*) FROM TaiKhoan WHERE Email IN
                                ('admin@trungtam.edu.vn',
                                 'admin02@trungtam.edu.vn',
                                 'admin03@trungtam.edu.vn'));
GO

/* ============================================================================
   BƯỚC 1 — TẠO LẠI 3 TÀI KHOẢN QUẢN TRỊ VIÊN
   Thứ tự BẮT BUỘC: TaiKhoan trước, NhanVien sau
   (NhanVien.MaNhanVien là FK trỏ tới TaiKhoan.TenDangNhap)
   ============================================================================ */

BEGIN TRY
    BEGIN TRANSACTION;

    ---------------------------------------------------------------
    -- (1) Lấy vai trò Quản trị viên — kiểm tra ngay, không đoán
    ---------------------------------------------------------------
    DECLARE @MaVaiTroQuanTri INT =
        (SELECT MaVaiTro FROM VaiTro WHERE TenVaiTro = N'Quản trị viên');

    IF @MaVaiTroQuanTri IS NULL
    BEGIN
        THROW 50001, N'KHÔNG tìm thấy vai trò "Quản trị viên" trong bảng VaiTro. Kiểm tra lại bảng VaiTro.', 1;
    END;

    ---------------------------------------------------------------
    -- (2) Lấy hash mẫu từ NVHV001 (mật khẩu: LanguageCenter@2026)
    --     PBKDF2 hash không gắn với user → dùng lại được cho tài khoản khác.
    ---------------------------------------------------------------
    DECLARE @HashMau NVARCHAR(500) =
        (SELECT MatKhauHash FROM TaiKhoan WHERE TenDangNhap = 'NVHV001');

    IF @HashMau IS NULL
    BEGIN
        THROW 50002, N'KHÔNG tìm thấy TaiKhoan NVHV001 để lấy hash mẫu. Chọn tài khoản khác rồi sửa lại câu SELECT.', 1;
    END;

    PRINT N'MaVaiTro Quản trị viên = ' + CAST(@MaVaiTroQuanTri AS NVARCHAR(10));
    PRINT N'Độ dài hash mẫu       = ' + CAST(LEN(@HashMau) AS NVARCHAR(10));

    ---------------------------------------------------------------
    -- (3) INSERT TaiKhoan (bỏ TrangThaiTaiKhoan → dùng DEFAULT N'Hoạt động')
    ---------------------------------------------------------------
    INSERT INTO TaiKhoan (MaVaiTro, TenDangNhap, MatKhauHash, Email)
    SELECT
        @MaVaiTroQuanTri,
        v.TenDangNhap,
        @HashMau,
        v.Email
    FROM (VALUES
            ('ADMIN001', 'admin@trungtam.edu.vn'  ),
            ('ADMIN002', 'admin02@trungtam.edu.vn'),
            ('ADMIN003', 'admin03@trungtam.edu.vn')
         ) AS v(TenDangNhap, Email)
    WHERE NOT EXISTS (SELECT 1 FROM TaiKhoan t WHERE t.TenDangNhap = v.TenDangNhap);

    PRINT N'Đã thêm TaiKhoan: ' + CAST(@@ROWCOUNT AS NVARCHAR(10));

    ---------------------------------------------------------------
    -- (4) INSERT NhanVien (bỏ TrangThaiLamViec → dùng DEFAULT N'Đang làm việc')
    ---------------------------------------------------------------
    INSERT INTO NhanVien
    (
        MaNhanVien, HoTen, NgaySinh, GioiTinh,
        SoDienThoai, Email, DiaChi, ChucVu, NgayVaoLam
    )
    SELECT
        v.MaNhanVien, v.HoTen, v.NgaySinh, v.GioiTinh,
        v.SoDienThoai, v.Email, v.DiaChi, v.ChucVu, v.NgayVaoLam
    FROM (VALUES
            ('ADMIN001', N'Đỗ Minh Khoa',    CONVERT(DATE, '1990-02-12'), N'Nam',
             '0901000001', 'admin@trungtam.edu.vn',  N'Quận 10, TP.HCM', N'Quản trị viên', CONVERT(DATE, '2023-01-10')),

            ('ADMIN002', N'Nguyễn Hoàng Long', CONVERT(DATE, '1992-07-05'), N'Nam',
             '0901000005', 'admin02@trungtam.edu.vn', N'Quận 3, TP.HCM',  N'Quản trị viên', CONVERT(DATE, '2024-02-01')),

            ('ADMIN003', N'Trần Thị Bích',    CONVERT(DATE, '1995-11-23'), N'Nữ',
             '0901000006', 'admin03@trungtam.edu.vn', N'Quận 1, TP.HCM',  N'Quản trị viên', CONVERT(DATE, '2024-08-15'))
         ) AS v(MaNhanVien, HoTen, NgaySinh, GioiTinh,
                SoDienThoai, Email, DiaChi, ChucVu, NgayVaoLam)
    WHERE NOT EXISTS (SELECT 1 FROM NhanVien n WHERE n.MaNhanVien = v.MaNhanVien);

    PRINT N'Đã thêm NhanVien: ' + CAST(@@ROWCOUNT AS NVARCHAR(10));

    COMMIT TRANSACTION;
    PRINT N'>>> COMMIT thành công.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT N'>>> LỖI — đã ROLLBACK. Không có thay đổi nào được ghi.';
    PRINT N'   Dòng lỗi : ' + CAST(ERROR_LINE() AS NVARCHAR(10));
    PRINT N'   Nội dung : ' + ERROR_MESSAGE();

    THROW;
END CATCH;
GO

/* ============================================================================
   BƯỚC 2 — KIỂM TRA KẾT QUẢ
   ============================================================================ */

PRINT N'=== BƯỚC 2: KẾT QUẢ ===';

SELECT
    t.MaTaiKhoan,
    t.TenDangNhap,
    n.HoTen,
    v.TenVaiTro,
    t.TrangThaiTaiKhoan,
    t.Email,
    HashThat = CASE WHEN t.MatKhauHash LIKE 'AQAAAA%' THEN N'CÓ (PBKDF2)' ELSE N'KHÔNG' END,
    DoDaiHash = LEN(t.MatKhauHash),
    DaoHash   = CASE WHEN t.MatKhauHash = (SELECT MatKhauHash FROM TaiKhoan WHERE TenDangNhap = 'NVHV001')
                     THEN N'GIỐNG NVHV001' ELSE N'KHÁC' END
FROM TaiKhoan t
JOIN VaiTro  v ON v.MaVaiTro = t.MaVaiTro
LEFT JOIN NhanVien n ON n.MaNhanVien = t.TenDangNhap
WHERE v.TenVaiTro = N'Quản trị viên'
ORDER BY t.TenDangNhap;

PRINT N'=== TỔNG SỐ QUẢN TRỊ VIÊN ===';

SELECT SoQuanTriVien = COUNT(*)
FROM TaiKhoan t
JOIN VaiTro  v ON v.MaVaiTro = t.MaVaiTro
WHERE v.TenVaiTro = N'Quản trị viên';
GO
