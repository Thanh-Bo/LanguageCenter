/* ============================================================================================
   TaoHoSo_GiaoVien_v17.sql
   --------------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Tạo HỒ SƠ GIÁO VIÊN mới (2 bảng: TaiKhoan + GiaoVien) trực tiếp bằng SQL.
       Danh sách mặc định gồm 3 giáo viên, trong đó 2 người là GIÁO VIÊN NƯỚC NGOÀI.

   VÌ SAO KHÔNG DÙNG LẠI SCRIPT CŨ (v16)
       Database hiện tại ĐÃ có GV003..GV010 (dữ liệu test ph3.*), nên nếu hard-code
       GV003/GV004/GV005 thì script cũ sẽ bỏ qua hết và không tạo được gì.
       Script này TỰ DÒ mã trống lớn nhất (GV011, GV012...) nên chạy được trên mọi DB.

   ĐẶC ĐIỂM
       - Idempotent : chạy lại nhiều lần không tạo trùng (chặn theo Email).
       - An toàn    : toàn bộ nằm trong 1 transaction; lỗi giữa đường sẽ ROLLBACK sạch.
       - Không hard-code MaVaiTro: tra theo tên vai trò trong bảng VaiTro.
       - Không tự tăng ID: MaTaiKhoan / MaPhanCong là IDENTITY, để SQL Server tự sinh.

   MẬT KHẨU SAU KHI TẠO
       Không thể băm PBKDF2 bằng T-SQL, nên script SAO CHÉP MatKhauHash từ một tài khoản
       đang có. Mặc định nó chọn tài khoản CHƯA TỪNG ĐĂNG NHẬP (LanDangNhapCuoi IS NULL):
       mọi tài khoản như vậy vẫn đang dùng mật khẩu mặc định của hệ thống là  111111
       (TemporaryPasswordGenerator.MatKhauMacDinh).
       => Giáo viên mới đăng nhập bằng  Tên đăng nhập (ví dụ GV011) / 111111.

       Nếu muốn dùng hash khác: dán hash vào biến @MatKhauHashTuChinh bên dưới.
       Nếu nghi ngờ: sau khi chạy, gọi  POST /api/admin/accounts/{MaTaiKhoan}/reset-password
       để buộc mật khẩu về 111111.

   CÁCH CHẠY
       1. Sửa danh sách giáo viên ở BƯỚC 2 nếu cần (họ tên, email, SĐT...).
       2. (Tuỳ chọn) đặt @MaLopHocPhanCong ở BƯỚC 3 để phân công ngay cho 1 lớp.
       3. Chạy toàn bộ file bằng SSMS / Azure Data Studio / sqlcmd.
       4. Đọc bảng kết quả in ra cuối cùng để lấy Tên đăng nhập.

   LƯU Ý NGHIỆP VỤ
       - "Giáo viên nước ngoài" KHÔNG phải là giá trị của GiaoVien.LoaiGiaoVien
         (cột này chỉ nhận N'Giáo viên' hoặc N'Trợ giảng' theo CK_GiaoVien_Loai).
         Nó là VAI TRÒ TRONG PHÂN CÔNG: PhanCongGiangDay.VaiTroPhanCong = N'Giáo viên nước ngoài'.
       - Sau khi phân công, phải ĐỒNG BỘ LẠI CA HỌC để buổi đầu tuần nhận đúng giáo viên:
         POST /api/admin/sessions/sync-teachers  (hoặc nút "Đồng bộ giáo viên" trên UI).
   ============================================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

/* ============================================================================================
   BƯỚC 0 — KIỂM TRA ĐIỀU KIỆN ĐẦU VÀO
   ============================================================================================ */
IF OBJECT_ID(N'dbo.GiaoVien', N'U') IS NULL
BEGIN
    RAISERROR(N'Không tìm thấy bảng dbo.GiaoVien. Kiểm tra lại database đang trỏ tới.', 16, 1);
    RETURN;
END;

DECLARE @MaVaiTroGiaoVien int =
    (SELECT MaVaiTro FROM dbo.VaiTro WHERE TenVaiTro = N'Giáo viên');

DECLARE @MaVaiTroTroGiang int =
    (SELECT MaVaiTro FROM dbo.VaiTro WHERE TenVaiTro = N'Trợ giảng');

IF @MaVaiTroGiaoVien IS NULL
BEGIN
    RAISERROR(N'Bảng VaiTro thiếu vai trò N''Giáo viên''. Không thể tạo hồ sơ.', 16, 1);
    RETURN;
END;

/* --------------------------------------------------------------------------------------------
   Nguồn hash mật khẩu.
   - @MatKhauHashTuChinh = NULL  -> tự lấy hash của tài khoản chưa từng đăng nhập (mật khẩu 111111)
   - @MatKhauHashTuChinh = '...' -> dùng đúng hash bạn dán vào
   -------------------------------------------------------------------------------------------- */
DECLARE @MatKhauHashTuChinh nvarchar(500) = NULL;
DECLARE @MatKhauHash nvarchar(500) = NULL;
DECLARE @NguonHash nvarchar(50) = NULL;

IF @MatKhauHashTuChinh IS NOT NULL
BEGIN
    SET @MatKhauHash = @MatKhauHashTuChinh;
    SET @NguonHash = N'(hash tự nhập)';
END
ELSE
BEGIN
    -- Ưu tiên tài khoản chưa từng đăng nhập => mật khẩu vẫn là 111111.
    SELECT TOP (1)
           @MatKhauHash = tk.MatKhauHash,
           @NguonHash   = tk.TenDangNhap
    FROM dbo.TaiKhoan AS tk
    WHERE tk.LanDangNhapCuoi IS NULL
      AND LEN(tk.MatKhauHash) > 50
    ORDER BY tk.MaTaiKhoan DESC;

    -- Không có tài khoản nào như vậy -> lấy đại một hash hợp lệ.
    IF @MatKhauHash IS NULL
    BEGIN
        SELECT TOP (1)
               @MatKhauHash = tk.MatKhauHash,
               @NguonHash   = tk.TenDangNhap
        FROM dbo.TaiKhoan AS tk
        WHERE LEN(tk.MatKhauHash) > 50
        ORDER BY tk.MaTaiKhoan DESC;

        IF @MatKhauHash IS NOT NULL
            PRINT N'[CẢNH BÁO] Không có tài khoản nào chưa đăng nhập. Đang dùng hash của '
                + @NguonHash + N' — mật khẩu mới KHÔNG chắc là 111111.'
                + N' Hãy gọi API reset-password sau khi chạy script.';
    END;
END;

IF @MatKhauHash IS NULL
BEGIN
    RAISERROR(N'Không tìm được MatKhauHash nguồn trong bảng TaiKhoan. Hãy dán hash vào @MatKhauHashTuChinh.', 16, 1);
    RETURN;
END;

PRINT N'[OK] Hash mật khẩu lấy từ tài khoản: ' + ISNULL(@NguonHash, N'(không rõ)');

/* ============================================================================================
   BƯỚC 1 — KẾT QUẢ (dùng để tra mã vừa tạo ở bước sau)
   ============================================================================================ */
DECLARE @KetQua TABLE
(
    ThuTu      int IDENTITY(1, 1) PRIMARY KEY,
    MaGiaoVien varchar(20)   NOT NULL,
    HoTen      nvarchar(150) NOT NULL,
    Email      varchar(255)  NOT NULL,
    VaiTro     nvarchar(50)  NOT NULL
);

/* ============================================================================================
   BƯỚC 2 — DANH SÁCH GIÁO VIÊN CẦN TẠO  (sửa ở đây)
   --------------------------------------------------------------------------------------------
   LoaiGiaoVien : N'Giáo viên' (mã GVxxx) hoặc N'Trợ giảng' (mã TGxxx)
   GioiTinh     : N'Nam' / N'Nu'  (cột nvarchar(20), không ràng buộc CHECK)
   SoDienThoai  : varchar(20), phải KHÁC số đã có (script tự bỏ qua nếu trùng)
   Email        : BẮT BUỘC, duy nhất toàn hệ thống (TaiKhoan.Email là UNIQUE)
   ============================================================================================ */
DECLARE @GiaoVienMoi TABLE
(
    ThuTu            int IDENTITY(1, 1) PRIMARY KEY,
    HoTen            nvarchar(150) NOT NULL,
    NgaySinh         date          NULL,
    GioiTinh         nvarchar(20)  NULL,
    SoDienThoai      varchar(20)   NULL,
    Email            varchar(255)  NOT NULL,
    DiaChi           nvarchar(500) NULL,
    LoaiGiaoVien     nvarchar(50)  NOT NULL,
    ChuyenMon        nvarchar(500) NULL,
    SoNamKinhNghiem  int           NULL,
    NgayVaoLam       date          NULL
);

INSERT INTO @GiaoVienMoi
    (HoTen, NgaySinh, GioiTinh, SoDienThoai, Email, DiaChi, LoaiGiaoVien, ChuyenMon, SoNamKinhNghiem, NgayVaoLam)
VALUES
    -- 1) Giáo viên người Việt
    (N'Nguyễn Thị Mai Lan', '1995-05-18', N'Nữ', '0912000011', 'gv011@trungtam.edu.vn',
     N'Quận 7, TP.HCM', N'Giáo viên', N'IELTS Listening & Reading', 6, '2026-11-05'),

    -- 2) Giáo viên nước ngoài (native UK) — sẽ giữ vai trò "Giáo viên nước ngoài"
    (N'Michael Andrew Smith', '1989-02-14', N'Nam', '0912000012', 'gv012@trungtam.edu.vn',
     N'Quận 1, TP.HCM', N'Giáo viên', N'IELTS Speaking & Listening (native, UK)', 9, '2026-11-05'),

    -- 3) Giáo viên nước ngoài (native US)
    (N'Sarah Elizabeth Johnson', '1991-09-03', N'Nữ', '0912000013', 'gv013@trungtam.edu.vn',
     N'Thủ Đức, TP.HCM', N'Giáo viên', N'IELTS Speaking & Writing (native, US)', 7, '2026-11-05');

/* ============================================================================================
   BƯỚC 3 — TẠO TÀI KHOẢN + HỒ SƠ (transaction duy nhất)
   ============================================================================================ */
DECLARE
    @HoTen           nvarchar(150),
    @NgaySinh        date,
    @GioiTinh        nvarchar(20),
    @SoDienThoai     varchar(20),
    @Email           varchar(255),
    @DiaChi          nvarchar(500),
    @LoaiGiaoVien    nvarchar(50),
    @ChuyenMon       nvarchar(500),
    @SoNamKinhNghiem int,
    @NgayVaoLam      date;

DECLARE
    @TienTo    char(2),
    @SoLonNhat int,
    @SoTiep    int,
    @MaMoi     varchar(20),
    @UngVien   varchar(20),
    @MaVaiTro  int,
    @VaiTro    nvarchar(50);

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE curGiaoVien CURSOR LOCAL FAST_FORWARD FOR
        SELECT HoTen, NgaySinh, GioiTinh, SoDienThoai, Email, DiaChi,
               LoaiGiaoVien, ChuyenMon, SoNamKinhNghiem, NgayVaoLam
        FROM @GiaoVienMoi
        ORDER BY ThuTu;

    OPEN curGiaoVien;

    FETCH NEXT FROM curGiaoVien INTO
        @HoTen, @NgaySinh, @GioiTinh, @SoDienThoai, @Email, @DiaChi,
        @LoaiGiaoVien, @ChuyenMon, @SoNamKinhNghiem, @NgayVaoLam;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        -- (a) Chặn trùng email trên toàn hệ thống tài khoản
        IF EXISTS (SELECT 1 FROM dbo.TaiKhoan WHERE Email = @Email)
           OR EXISTS (SELECT 1 FROM dbo.GiaoVien WHERE Email = @Email)
        BEGIN
            PRINT N'[BỎ QUA] Email đã tồn tại: ' + @Email + N' (giáo viên: ' + @HoTen + N')';
        END
        ELSE
        BEGIN
            -- (b) Số điện thoại trùng thì bỏ trống, không chặn cả hồ sơ
            IF @SoDienThoai IS NOT NULL
               AND EXISTS (SELECT 1 FROM dbo.GiaoVien WHERE SoDienThoai = @SoDienThoai)
            BEGIN
                PRINT N'[LƯU Ý] SĐT ' + @SoDienThoai + N' đã dùng cho hồ sơ khác -> để trống.';
                SET @SoDienThoai = NULL;
            END;

            -- (c) Xác định tiền tố + vai trò theo LoaiGiaoVien
            IF @LoaiGiaoVien = N'Trợ giảng'
            BEGIN
                SET @TienTo   = 'TG';
                SET @MaVaiTro = @MaVaiTroTroGiang;
                SET @VaiTro   = N'Trợ giảng';
            END
            ELSE
            BEGIN
                SET @TienTo   = 'GV';
                SET @MaVaiTro = @MaVaiTroGiaoVien;
                SET @VaiTro   = N'Giáo viên';
            END;

            IF @MaVaiTro IS NULL
            BEGIN
                PRINT N'[BỎ QUA] Bảng VaiTro chưa có vai trò "' + @VaiTro + N'".';
            END
            ELSE
            BEGIN
                -- (d) Dò mã trống kế tiếp — KHÔNG dùng COUNT(*)+1
                SET @SoLonNhat = 0;

                SELECT @SoLonNhat = ISNULL(MAX(CAST(SUBSTRING(MaGiaoVien, 3, 10) AS int)), 0)
                FROM dbo.GiaoVien
                WHERE MaGiaoVien LIKE @TienTo + '[0-9][0-9][0-9]';

                SET @SoTiep  = @SoLonNhat + 1;
                SET @MaMoi   = NULL;
                SET @UngVien = NULL;

                WHILE @MaMoi IS NULL
                BEGIN
                    IF @SoTiep > 999
                    BEGIN
                        RAISERROR(N'Đã vượt giới hạn %s999 — cần thiết kế lại quy tắc sinh mã.', 16, 1, @TienTo);
                    END;

                    SET @UngVien = @TienTo + RIGHT('000' + CAST(@SoTiep AS varchar(10)), 3);

                    IF NOT EXISTS (SELECT 1 FROM dbo.GiaoVien  WHERE MaGiaoVien  = @UngVien)
                       AND NOT EXISTS (SELECT 1 FROM dbo.TaiKhoan WHERE TenDangNhap = @UngVien)
                        SET @MaMoi = @UngVien;
                    ELSE
                        SET @SoTiep = @SoTiep + 1;
                END;

                -- (e) Tài khoản đăng nhập (TenDangNhap = MaGiaoVien)
                INSERT INTO dbo.TaiKhoan
                    (MaVaiTro, TenDangNhap, MatKhauHash, Email, TrangThaiTaiKhoan,
                     LanDangNhapCuoi, NgayTao, NgayCapNhat)
                VALUES
                    (@MaVaiTro, @MaMoi, @MatKhauHash, @Email, N'Hoạt động',
                     NULL, SYSDATETIME(), SYSDATETIME());

                -- (f) Hồ sơ giáo viên (PK = FK trỏ tới TaiKhoan.TenDangNhap)
                INSERT INTO dbo.GiaoVien
                    (MaGiaoVien, HoTen, NgaySinh, GioiTinh, SoDienThoai, Email, DiaChi,
                     LoaiGiaoVien, ChuyenMon, SoNamKinhNghiem, NgayVaoLam, TrangThai)
                VALUES
                    (@MaMoi, @HoTen, @NgaySinh, @GioiTinh, @SoDienThoai, @Email, @DiaChi,
                     @LoaiGiaoVien, @ChuyenMon, @SoNamKinhNghiem, @NgayVaoLam, N'Đang làm việc');

                INSERT INTO @KetQua (MaGiaoVien, HoTen, Email, VaiTro)
                VALUES (@MaMoi, @HoTen, @Email, @VaiTro);

                PRINT N'[TẠO MỚI] ' + @MaMoi + N' — ' + @HoTen + N' — ' + @Email;
            END;
        END;

        FETCH NEXT FROM curGiaoVien INTO
            @HoTen, @NgaySinh, @GioiTinh, @SoDienThoai, @Email, @DiaChi,
            @LoaiGiaoVien, @ChuyenMon, @SoNamKinhNghiem, @NgayVaoLam;
    END;

    CLOSE curGiaoVien;
    DEALLOCATE curGiaoVien;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF CURSOR_STATUS('local', 'curGiaoVien') >= -1
    BEGIN
        IF CURSOR_STATUS('local', 'curGiaoVien') > -1 CLOSE curGiaoVien;
        DEALLOCATE curGiaoVien;
    END;

    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    DECLARE @ThongDiep nvarchar(2000) =
        N'LỖI: ' + ERROR_MESSAGE()
        + N' (dòng ' + CAST(ERROR_LINE() AS nvarchar(10)) + N') — đã ROLLBACK, không có dữ liệu nào được ghi.';

    RAISERROR(@ThongDiep, 16, 1);
    RETURN;
END CATCH;

/* ============================================================================================
   BƯỚC 4 — (TUỲ CHỌN) PHÂN CÔNG GIẢNG DẠY NGAY
   --------------------------------------------------------------------------------------------
   Điền MaLopHoc vào @MaLopHocPhanCong để gán:
     - Giáo viên nước ngoài : gv012@trungtam.edu.vn (buổi ĐẦU TUẦN)
     - Giáo viên chính      : giữ nguyên người đang phụ trách lớp (nếu đã có)
   Để NULL nếu chỉ muốn tạo hồ sơ rồi phân công trên giao diện.
   ============================================================================================ */
DECLARE @MaLopHocPhanCong int = NULL;   -- <<< ví dụ: 1
DECLARE @EmailGiaoVienNuocNgoai varchar(255) = 'gv012@trungtam.edu.vn';

IF @MaLopHocPhanCong IS NULL
BEGIN
    PRINT N'[BƯỚC 4] Bỏ qua phân công giảng dạy (@MaLopHocPhanCong = NULL).';
END
ELSE IF NOT EXISTS (SELECT 1 FROM dbo.LopHoc WHERE MaLopHoc = @MaLopHocPhanCong)
BEGIN
    PRINT N'[BƯỚC 4] Không tìm thấy lớp MaLopHoc = ' + CAST(@MaLopHocPhanCong AS varchar(10)) + N' -> bỏ qua.';
END
ELSE
BEGIN
    DECLARE @MaGiaoVienNuocNgoai varchar(20) =
        (SELECT MaGiaoVien FROM dbo.GiaoVien WHERE Email = @EmailGiaoVienNuocNgoai);

    IF @MaGiaoVienNuocNgoai IS NULL
    BEGIN
        PRINT N'[BƯỚC 4] Không tìm thấy hồ sơ có email ' + @EmailGiaoVienNuocNgoai + N' -> bỏ qua.';
    END
    ELSE IF EXISTS
        (
            SELECT 1
            FROM dbo.PhanCongGiangDay
            WHERE MaLopHoc = @MaLopHocPhanCong
              AND VaiTroPhanCong = N'Giáo viên nước ngoài'
              AND TrangThai = N'Đang phân công'
        )
    BEGIN
        PRINT N'[BƯỚC 4] Lớp đã có phân công "Giáo viên nước ngoài" -> không thêm trùng.';
    END
    ELSE
    BEGIN
        DECLARE @NgayBatDauLop date, @NgayKetThucLop date;

        SELECT @NgayBatDauLop = NgayBatDau,
               @NgayKetThucLop = NgayKetThucDuKien
        FROM dbo.LopHoc
        WHERE MaLopHoc = @MaLopHocPhanCong;

        INSERT INTO dbo.PhanCongGiangDay
            (MaLopHoc, MaGiaoVien, VaiTroPhanCong, NgayBatDau, NgayKetThuc, TrangThai)
        VALUES
            (@MaLopHocPhanCong, @MaGiaoVienNuocNgoai, N'Giáo viên nước ngoài',
             @NgayBatDauLop, @NgayKetThucLop, N'Đang phân công');

        PRINT N'[BƯỚC 4] Đã phân công ' + @MaGiaoVienNuocNgoai
            + N' làm "Giáo viên nước ngoài" cho lớp ' + CAST(@MaLopHocPhanCong AS varchar(10))
            + N'. Nhớ gọi POST /api/admin/sessions/sync-teachers để cập nhật ca học.';
    END;
END;

/* ============================================================================================
   BƯỚC 5 — KẾT QUẢ
   ============================================================================================ */
SELECT
    k.MaGiaoVien       AS MaGiaoVien,
    k.HoTen            AS HoTen,
    k.Email            AS Email,
    k.VaiTro           AS VaiTroHeThong,
    g.LoaiGiaoVien     AS LoaiGiaoVien,
    g.ChuyenMon        AS ChuyenMon,
    g.TrangThai        AS TrangThaiHoSo
FROM @KetQua AS k
LEFT JOIN dbo.GiaoVien AS g ON g.MaGiaoVien = k.MaGiaoVien
ORDER BY k.ThuTu;

-- PRINT không cho phép subquery trong biểu thức -> phải gán ra biến trước.
DECLARE @SoHoSoVuaTao int = (SELECT COUNT(*) FROM @KetQua);

PRINT N'';
PRINT N'================ KẾT LUẬN ================';
PRINT N'Số hồ sơ vừa tạo: ' + CAST(@SoHoSoVuaTao AS varchar(10));
PRINT N'Mật khẩu đăng nhập: 111111 (đổi ngay sau lần đăng nhập đầu tiên).';
PRINT N'Bước tiếp theo:';
PRINT N'  1) Vào "Phân công giảng dạy" (WinForms) hoặc API để gán vai trò Giáo viên nước ngoài.';
PRINT N'  2) Gọi POST /api/admin/sessions/sync-teachers để ca học nhận đúng giáo viên.';
PRINT N'  3) Nếu cần đặt lại mật khẩu: POST /api/admin/accounts/{MaTaiKhoan}/reset-password';
PRINT N'==========================================';
