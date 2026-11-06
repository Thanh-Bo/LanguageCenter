/* ============================================================================================
   PhanCongBuoiDay_GiaoVien_v18.sql
   --------------------------------------------------------------------------------------------
   MỤC ĐÍCH
       Áp dụng quy ước:  BUỔI ĐẦU TUẦN  -> GIÁO VIÊN NƯỚC NGOÀI
                         BUỔI CUỐI TUẦN -> GIÁO VIÊN VIỆT (Giáo viên chính)
       cho một hoặc nhiều lớp, trong MỘT lần chạy.

   QUY ƯỚC THỨ (rất quan trọng — schema dùng số ISO, KHÔNG phải số đếm)
       1 = Thứ Hai ... 7 = Chủ Nhật.
       => "Thứ Hai + Thứ Tư" phải khai là (1, 3), KHÔNG phải (2, 4).

   CÁCH BACKEND HIỂU (đã có sẵn, script này không sửa code)
       - Sinh ca học: nếu ThuBuoi1 > ThuBuoi2 thì tự hoán đổi  -> buổi 1 luôn là thứ SỚM hơn.
       - Đồng bộ giáo viên (SessionStore.SyncTeachersAsync):
             SoBuoi lẻ  (1,3,5...) = buổi đầu tuần  -> PhanCongGiangDay 'Giáo viên nước ngoài'
             SoBuoi chẵn (2,4,6...) = buổi cuối tuần -> PhanCongGiangDay 'Giáo viên chính'

   SCRIPT NÀY LÀM 4 VIỆC CHO MỖI LỚP
       1. Cập nhật LopHoc.ThuBuoi1 / ThuBuoi2 đúng quy ước.
       2. Giữ đúng 2 dòng LichHoc đang hoạt động (các thứ khác -> N'Ngừng áp dụng').
       3. Bảo đảm PhanCongGiangDay có 'Giáo viên nước ngoài' và 'Giáo viên chính'.
       4. Gán lại CaHoc đang N'Chưa diễn ra' theo quy ước lẻ/chẵn.

   AN TOÀN
       - Idempotent: chạy lại nhiều lần vẫn cho cùng kết quả.
       - Chỉ đụng ca học N'Chưa diễn ra' (không sửa ca đã học / đã hủy).
       - Toàn bộ nằm trong 1 transaction; lỗi giữa đường -> ROLLBACK.
       - KHÔNG xoá gì cả: LichHoc cũ chỉ đổi trạng thái, dữ liệu lịch sử giữ nguyên.

   ĐIỀU KIỆN
       - Đã chạy TaoHoSo_GiaoVien_v17.sql (hoặc đã có hồ sơ giáo viên nước ngoài).
       - Mã lớp / mã giáo viên phải tồn tại trong DB.
   ============================================================================================ */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

/* ============================================================================================
   BƯỚC 0 — KIỂM TRA & TRA MÃ GIÁO VIÊN
   ============================================================================================ */
IF OBJECT_ID(N'dbo.LopHoc', N'U') IS NULL
   OR OBJECT_ID(N'dbo.LichHoc', N'U') IS NULL
   OR OBJECT_ID(N'dbo.PhanCongGiangDay', N'U') IS NULL
BEGIN
    RAISERROR(N'Thiếu bảng LopHoc / LichHoc / PhanCongGiangDay. Kiểm tra lại database đang trỏ tới.', 16, 1);
    RETURN;
END;

-- Giáo viên nước ngoài: để NULL nếu muốn script tự tra theo email.
DECLARE @MaGVNuocNgoai varchar(20) = NULL;
DECLARE @EmailGVNuocNgoai varchar(255) = 'gv012@trungtam.edu.vn';

-- Giáo viên Việt (buổi cuối tuần): NULL = giữ nguyên 'Giáo viên chính' đang phân công.
DECLARE @MaGVViet varchar(20) = NULL;

IF @MaGVNuocNgoai IS NULL
    SELECT @MaGVNuocNgoai = MaGiaoVien
    FROM dbo.GiaoVien
    WHERE Email = @EmailGVNuocNgoai;

IF @MaGVNuocNgoai IS NULL
    SELECT TOP (1) @MaGVNuocNgoai = MaGiaoVien
    FROM dbo.GiaoVien
    WHERE HoTen LIKE N'%Michael Andrew Smith%'
    ORDER BY MaGiaoVien;

IF @MaGVNuocNgoai IS NULL
BEGIN
    RAISERROR(N'Không tìm thấy giáo viên nước ngoài (email %s). Hãy chạy TaoHoSo_GiaoVien_v17.sql trước.', 16, 1, @EmailGVNuocNgoai);
    RETURN;
END;

IF @MaGVViet IS NOT NULL
   AND NOT EXISTS (SELECT 1 FROM dbo.GiaoVien WHERE MaGiaoVien = @MaGVViet)
BEGIN
    RAISERROR(N'Mã giáo viên Việt %s không tồn tại trong bảng GiaoVien.', 16, 1, @MaGVViet);
    RETURN;
END;

PRINT N'[OK] Giáo viên nước ngoài (buổi đầu tuần): ' + @MaGVNuocNgoai;
PRINT N'[OK] Giáo viên Việt (buổi cuối tuần): '
    + ISNULL(@MaGVViet, N'(giữ nguyên "Giáo viên chính" đang phân công)');

/* ============================================================================================
   BƯỚC 1 — CẤU HÌNH LỚP  (sửa ở đây)
   --------------------------------------------------------------------------------------------
   MaLopHoc | ThuDauTuan | ThuCuoiTuan        (1 = Thứ Hai ... 7 = Chủ Nhật)
   --------------------------------------------------------------------------------------------
   Lưu ý: 3 lớp hiện tại đều học 18:00–20:00 và DB chỉ có 2 phòng.
          Bật thêm lớp 2 / lớp 3 sẽ đụng xung đột phòng khi Sinh ca học.
   ============================================================================================ */
DECLARE @CauHinh TABLE
(
    MaLopHoc    int     NOT NULL PRIMARY KEY,
    ThuDauTuan  tinyint NOT NULL,
    ThuCuoiTuan tinyint NOT NULL
);

INSERT INTO @CauHinh (MaLopHoc, ThuDauTuan, ThuCuoiTuan)
VALUES
    (1, 1, 3);   -- lớp 1: Thứ Hai (đầu tuần) + Thứ Tư (cuối tuần)

-- Muốn áp dụng cho lớp 2 và lớp 3 thì bỏ comment 2 dòng dưới:
--    ,(2, 2, 4)    -- lớp 2: Thứ Ba + Thứ Năm
--    ,(3, 3, 5);   -- lớp 3: Thứ Tư + Thứ Sáu

/* ============================================================================================
   BƯỚC 2 — BÁO CÁO (khởi tạo)
   ============================================================================================ */
DECLARE @BaoCao TABLE
(
    ThuTu        int IDENTITY(1, 1) PRIMARY KEY,
    MaLopHoc     int           NOT NULL,
    MaLop        varchar(50)   NULL,
    ThuDauTuan   tinyint       NULL,
    ThuCuoiTuan  tinyint       NULL,
    GVNuocNgoai  varchar(20)   NULL,
    GVViet       varchar(20)   NULL,
    SoCaTong     int           NULL,
    SoCaCapNhat  int           NULL,
    GhiChu       nvarchar(400) NULL
);

/* ============================================================================================
   BƯỚC 3 — ÁP DỤNG
   ============================================================================================ */
DECLARE
    @Lop         int,
    @ThuDau      tinyint,
    @ThuCuoi     tinyint,
    @MaLopText   varchar(50),
    @GioBatDau   time,
    @GioKetThuc  time,
    @MaPhongHoc  int,
    @NgayApDung  date,
    @NgayKetThuc date,
    @MaGVChinh   varchar(20),
    @SoCaTong    int,
    @SoCaCapNhat int,
    @GhiChu      nvarchar(400);

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE curLop CURSOR LOCAL FAST_FORWARD FOR
        SELECT MaLopHoc, ThuDauTuan, ThuCuoiTuan
        FROM @CauHinh
        ORDER BY MaLopHoc;

    OPEN curLop;

    FETCH NEXT FROM curLop INTO @Lop, @ThuDau, @ThuCuoi;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @MaLopText   = NULL;
        SET @GhiChu      = NULL;
        SET @MaGVChinh   = NULL;
        SET @SoCaCapNhat = 0;

        SELECT @MaLopText   = MaLop,
               @NgayApDung  = NgayBatDau,
               @NgayKetThuc = NgayKetThucDuKien
        FROM dbo.LopHoc
        WHERE MaLopHoc = @Lop;

        IF @MaLopText IS NULL
        BEGIN
            INSERT INTO @BaoCao (MaLopHoc, GhiChu)
            VALUES (@Lop, N'Không tìm thấy lớp — bỏ qua.');

            FETCH NEXT FROM curLop INTO @Lop, @ThuDau, @ThuCuoi;
            CONTINUE;
        END;

        -- --------------------------------------------------------------------------------
        -- 3.1  LopHoc.ThuBuoi1 / ThuBuoi2
        -- --------------------------------------------------------------------------------
        UPDATE dbo.LopHoc
        SET ThuBuoi1 = @ThuDau,
            ThuBuoi2 = @ThuCuoi
        WHERE MaLopHoc = @Lop;

        -- --------------------------------------------------------------------------------
        -- 3.2  LichHoc: đúng 2 thứ đang hoạt động
        -- --------------------------------------------------------------------------------
        SET @MaPhongHoc = NULL;

        SELECT TOP (1) @MaPhongHoc = MaPhongHoc
        FROM dbo.LichHoc
        WHERE MaLopHoc = @Lop AND TrangThai <> N'Ngừng áp dụng'
        ORDER BY MaLichHoc;

        IF @MaPhongHoc IS NULL
            SELECT TOP (1) @MaPhongHoc = MaPhongHoc
            FROM dbo.PhongHoc
            ORDER BY MaPhongHoc;

        IF @MaPhongHoc IS NULL
        BEGIN
            SET @GhiChu = N'Chưa có phòng học nào -> bỏ qua phần Lịch học.';
        END
        ELSE
        BEGIN
            SET @GioBatDau  = NULL;
            SET @GioKetThuc = NULL;

            SELECT TOP (1) @GioBatDau = GioBatDau, @GioKetThuc = GioKetThuc
            FROM dbo.LichHoc
            WHERE MaLopHoc = @Lop AND TrangThai <> N'Ngừng áp dụng'
            ORDER BY MaLichHoc;

            SET @GioBatDau  = ISNULL(@GioBatDau, CAST('18:00' AS time));
            SET @GioKetThuc = ISNULL(@GioKetThuc, DATEADD(HOUR, 2, @GioBatDau));
            SET @NgayApDung = ISNULL(@NgayApDung, CAST(GETDATE() AS date));

            -- Các thứ không còn dùng -> ngừng áp dụng (KHÔNG xoá, để CaHoc cũ không vỡ FK)
            UPDATE dbo.LichHoc
            SET TrangThai = N'Ngừng áp dụng'
            WHERE MaLopHoc = @Lop
              AND ThuTrongTuan NOT IN (@ThuDau, @ThuCuoi)
              AND TrangThai <> N'Ngừng áp dụng';

            -- Bật lại 2 thứ cần dùng nếu đã bị ngừng trước đó
            UPDATE dbo.LichHoc
            SET TrangThai = N'Hoạt động'
            WHERE MaLopHoc = @Lop
              AND ThuTrongTuan IN (@ThuDau, @ThuCuoi)
              AND TrangThai = N'Ngừng áp dụng';

            IF NOT EXISTS (SELECT 1 FROM dbo.LichHoc WHERE MaLopHoc = @Lop AND ThuTrongTuan = @ThuDau)
                INSERT INTO dbo.LichHoc
                    (MaLopHoc, MaPhongHoc, ThuTrongTuan, GioBatDau, GioKetThuc, NgayApDungTu, TrangThai)
                VALUES
                    (@Lop, @MaPhongHoc, @ThuDau, @GioBatDau, @GioKetThuc, @NgayApDung, N'Hoạt động');

            IF NOT EXISTS (SELECT 1 FROM dbo.LichHoc WHERE MaLopHoc = @Lop AND ThuTrongTuan = @ThuCuoi)
                INSERT INTO dbo.LichHoc
                    (MaLopHoc, MaPhongHoc, ThuTrongTuan, GioBatDau, GioKetThuc, NgayApDungTu, TrangThai)
                VALUES
                    (@Lop, @MaPhongHoc, @ThuCuoi, @GioBatDau, @GioKetThuc, @NgayApDung, N'Hoạt động');
        END;

        -- --------------------------------------------------------------------------------
        -- 3.3  PhanCongGiangDay — 'Giáo viên nước ngoài' (buổi đầu tuần)
        -- --------------------------------------------------------------------------------
        IF NOT EXISTS
            (
                SELECT 1
                FROM dbo.PhanCongGiangDay
                WHERE MaLopHoc = @Lop
                  AND VaiTroPhanCong = N'Giáo viên nước ngoài'
                  AND TrangThai = N'Đang phân công'
            )
        BEGIN
            INSERT INTO dbo.PhanCongGiangDay
                (MaLopHoc, MaGiaoVien, VaiTroPhanCong, NgayBatDau, NgayKetThuc, TrangThai)
            VALUES
                (@Lop, @MaGVNuocNgoai, N'Giáo viên nước ngoài',
                 ISNULL(@NgayApDung, CAST(GETDATE() AS date)), @NgayKetThuc, N'Đang phân công');
        END
        ELSE
        BEGIN
            UPDATE dbo.PhanCongGiangDay
            SET MaGiaoVien = @MaGVNuocNgoai
            WHERE MaLopHoc = @Lop
              AND VaiTroPhanCong = N'Giáo viên nước ngoài'
              AND TrangThai = N'Đang phân công'
              AND ISNULL(MaGiaoVien, '') <> @MaGVNuocNgoai;
        END;

        -- --------------------------------------------------------------------------------
        -- 3.4  PhanCongGiangDay — 'Giáo viên chính' (buổi cuối tuần)
        --      Chỉ ghi đè khi @MaGVViet được chỉ định.
        -- --------------------------------------------------------------------------------
        IF @MaGVViet IS NOT NULL
        BEGIN
            IF NOT EXISTS
                (
                    SELECT 1
                    FROM dbo.PhanCongGiangDay
                    WHERE MaLopHoc = @Lop
                      AND VaiTroPhanCong = N'Giáo viên chính'
                      AND TrangThai = N'Đang phân công'
                )
            BEGIN
                INSERT INTO dbo.PhanCongGiangDay
                    (MaLopHoc, MaGiaoVien, VaiTroPhanCong, NgayBatDau, NgayKetThuc, TrangThai)
                VALUES
                    (@Lop, @MaGVViet, N'Giáo viên chính',
                     ISNULL(@NgayApDung, CAST(GETDATE() AS date)), @NgayKetThuc, N'Đang phân công');
            END
            ELSE
            BEGIN
                UPDATE dbo.PhanCongGiangDay
                SET MaGiaoVien = @MaGVViet
                WHERE MaLopHoc = @Lop
                  AND VaiTroPhanCong = N'Giáo viên chính'
                  AND TrangThai = N'Đang phân công'
                  AND ISNULL(MaGiaoVien, '') <> @MaGVViet;
            END;
        END;

        SELECT TOP (1) @MaGVChinh = MaGiaoVien
        FROM dbo.PhanCongGiangDay
        WHERE MaLopHoc = @Lop
          AND VaiTroPhanCong = N'Giáo viên chính'
          AND TrangThai = N'Đang phân công'
        ORDER BY MaPhanCong DESC;

        -- --------------------------------------------------------------------------------
        -- 3.5  Gán lại CaHoc theo quy ước lẻ/chẵn
        -- --------------------------------------------------------------------------------
        SELECT @SoCaTong = COUNT(*) FROM dbo.CaHoc WHERE MaLopHoc = @Lop;

        IF @MaGVChinh IS NULL
        BEGIN
            SET @GhiChu = CONCAT(ISNULL(@GhiChu + N' ', N''),
                N'Chưa có "Giáo viên chính" -> không gán được buổi cuối tuần.');
        END
        ELSE
        BEGIN
            UPDATE ch
            SET MaGiaoVienThucTe =
                CASE WHEN ch.SoBuoi % 2 = 1 THEN @MaGVNuocNgoai ELSE @MaGVChinh END
            FROM dbo.CaHoc AS ch
            WHERE ch.MaLopHoc = @Lop
              AND ch.TrangThai = N'Chưa diễn ra'
              AND ch.SoBuoi IS NOT NULL
              AND ISNULL(ch.MaGiaoVienThucTe, '')
                  <> CASE WHEN ch.SoBuoi % 2 = 1 THEN @MaGVNuocNgoai ELSE @MaGVChinh END;

            SET @SoCaCapNhat = @@ROWCOUNT;
        END;

        IF @SoCaTong = 0
            SET @GhiChu = CONCAT(ISNULL(@GhiChu + N' ', N''),
                N'Lớp chưa có ca học — cần bấm "Sinh ca học".');

        INSERT INTO @BaoCao
            (MaLopHoc, MaLop, ThuDauTuan, ThuCuoiTuan, GVNuocNgoai, GVViet, SoCaTong, SoCaCapNhat, GhiChu)
        VALUES
            (@Lop, @MaLopText, @ThuDau, @ThuCuoi, @MaGVNuocNgoai, @MaGVChinh,
             @SoCaTong, @SoCaCapNhat, @GhiChu);

        FETCH NEXT FROM curLop INTO @Lop, @ThuDau, @ThuCuoi;
    END;

    CLOSE curLop;
    DEALLOCATE curLop;

    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF CURSOR_STATUS('local', 'curLop') >= -1
    BEGIN
        IF CURSOR_STATUS('local', 'curLop') > -1 CLOSE curLop;
        DEALLOCATE curLop;
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
   BƯỚC 4 — KẾT QUẢ
   ============================================================================================ */
SELECT
    b.MaLopHoc,
    b.MaLop,
    b.ThuDauTuan,
    b.ThuCuoiTuan,
    gvnn.HoTen            AS GiaoVienNuocNgoai,
    b.GVNuocNgoai         AS MaGVNuocNgoai,
    gvv.HoTen             AS GiaoVienViet,
    b.GVViet              AS MaGVViet,
    b.SoCaTong            AS SoCaHoc,
    b.SoCaCapNhat         AS SoCaDaCapNhat,
    b.GhiChu
FROM @BaoCao AS b
LEFT JOIN dbo.GiaoVien AS gvnn ON gvnn.MaGiaoVien = b.GVNuocNgoai
LEFT JOIN dbo.GiaoVien AS gvv  ON gvv.MaGiaoVien  = b.GVViet
ORDER BY b.ThuTu;

-- Danh sách ca học sau khi áp dụng (chỉ các ca CHƯA diễn ra)
SELECT
    ch.MaLopHoc,
    ch.SoBuoi,
    ch.NgayHoc,
    ch.GioBatDau,
    ch.GioKetThuc,
    CASE WHEN ch.SoBuoi % 2 = 1 THEN N'Đầu tuần' ELSE N'Cuối tuần' END AS ViTriTrongTuan,
    ch.MaGiaoVienThucTe,
    g.HoTen AS TenGiaoVien,
    ch.LoaiBuoi,
    ch.TrangThai
FROM dbo.CaHoc AS ch
LEFT JOIN dbo.GiaoVien AS g ON g.MaGiaoVien = ch.MaGiaoVienThucTe
WHERE ch.MaLopHoc IN (SELECT MaLopHoc FROM @CauHinh)
  AND ch.TrangThai = N'Chưa diễn ra'
ORDER BY ch.MaLopHoc, ch.NgayHoc;

PRINT N'';
PRINT N'================ KẾT LUẬN ================';
PRINT N'Quy ước thứ: 1 = Thứ Hai ... 7 = Chủ Nhật.';
PRINT N'Buổi lẻ (1,3,5...) = đầu tuần  -> Giáo viên nước ngoài.';
PRINT N'Buổi chẵn (2,4,6...) = cuối tuần -> Giáo viên chính (người Việt).';
PRINT N'Ca đã học / đã hủy KHÔNG bị thay đổi.';
PRINT N'Nếu lớp chưa có ca học: vào Lịch học -> Sinh ca học rồi chạy lại script này.';
PRINT N'==========================================';
