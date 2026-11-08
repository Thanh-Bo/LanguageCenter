/* =====================================================================================
   Reset_DuLieu_v42.sql
   -------------------------------------------------------------------------------------
   MỤC ĐÍCH
       XOÁ TOÀN BỘ dữ liệu nghiệp vụ, chỉ GIỮ LẠI:
         • Tài khoản Quản trị viên  (TaiKhoan có vai trò "Quản trị viên" + NhanVien tương ứng)
         • Chương trình đào tạo     ChuongTrinh → CapDo → KhoaHoc → NoiDungBaiHoc
         • Đầu điểm của khóa học    LoaiDiem
         • Bảng VaiTro              (bắt buộc — TaiKhoan tham chiếu tới)

       VÀ XOÁ THÊM
         • Khóa học rác "dfsaf" (MaKhoaHoc = 15) + toàn bộ nội dung/đầu điểm của nó

   XOÁ SẠCH (mọi bảng khác)
       Học viên · Phụ huynh · Giáo viên · Bằng cấp · Nhân viên (không phải admin)
       Lớp học · Lịch học · Ca học · Phân công giảng dạy · Đăng ký học
       Học phí · Thanh toán · Biên nhận · Điểm số · Điểm danh
       Kiểm tra đầu vào · Chứng chỉ · Ưu đãi
       Khảo sát + Câu hỏi + Câu trả lời + Phản hồi khảo sát + Phản hồi dịch vụ
       Thông báo · Dịch vụ · Ngày lễ · Phòng học

   CÁCH CHẠY
       Mở trong SSMS → bấm Execute (F5).
       Muốn XEM TRƯỚC số dòng sẽ xoá mà không xoá thật:
           đổi   DECLARE @CheDo nvarchar(10) = N'XOA';
           thành DECLARE @CheDo nvarchar(10) = N'THU';

   AN TOÀN
       • Toàn bộ nằm trong MỘT transaction: lỗi giữa đường → ROLLBACK, dữ liệu nguyên vẹn.
       • Khoá ngoại được TẮT TẠM trong transaction rồi BẬT LẠI kèm WITH CHECK
         → không cần sắp thứ tự xoá, và bảng tự tham chiếu cũng xoá được.
       • Kiểm tra TRƯỚC KHI XOÁ: nếu không có tài khoản "Quản trị viên" nào
         thì HUỶ toàn bộ (tránh tự khoá mình ra khỏi hệ thống).
       • Không DROP / ALTER cấu trúc — chỉ DELETE dữ liệu.
       • Sau khi xoá, IDENTITY của các bảng đã xoá sạch được đặt lại về 1.

   ⚠ HÃY SAO LƯU TRƯỚC
       WinForms → "Sao lưu & phục hồi" → Sao lưu ngay
   ===================================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;

USE QuanLyTrungTamNgoaiNgu;
GO

PRINT N'=====================================================================';
PRINT N'  v42 — RESET DỮ LIỆU (giữ Quản trị viên + Chương trình đào tạo)';
PRINT N'=====================================================================';
GO

BEGIN TRY
    /* -----------------------------------------------------------------------------
       0. KIỂM TRA MÔI TRƯỜNG
       ----------------------------------------------------------------------------- */
    -- Bảng tạm #... tồn tại theo SESSION, không tự mất khi batch lỗi.
    -- Nếu lần chạy trước thất bại giữa đường, #Target / #TruocDo vẫn còn → dọn trước.
    IF OBJECT_ID(N'tempdb..#Target')  IS NOT NULL DROP TABLE #Target;
    IF OBJECT_ID(N'tempdb..#TruocDo') IS NOT NULL DROP TABLE #TruocDo;

    IF OBJECT_ID(N'dbo.TaiKhoan', N'U') IS NULL
    BEGIN
        THROW 50042, N'Không tìm thấy bảng dbo.TaiKhoan — kiểm tra database đang trỏ tới.', 1;
    END

    IF OBJECT_ID(N'dbo.VaiTro', N'U') IS NULL
    BEGIN
        THROW 50043, N'Không tìm thấy bảng dbo.VaiTro — kiểm tra database đang trỏ tới.', 1;
    END

    /* -----------------------------------------------------------------------------
       1. CHẾ ĐỘ CHẠY
          N'XOA' = xoá thật  |  N'THU' = chỉ đếm rồi ROLLBACK
       ----------------------------------------------------------------------------- */
    DECLARE @CheDo nvarchar(10) = N'XOA';

    /* -----------------------------------------------------------------------------
       2. DANH SÁCH BẢNG GIỮ LẠI HOÀN TOÀN
          Muốn xoá cả khóa học / cấp độ thì bỏ dòng tương ứng khỏi đây.
       ----------------------------------------------------------------------------- */
    DECLARE @GiuBang TABLE (TenBang sysname PRIMARY KEY);

    INSERT INTO @GiuBang (TenBang)
    VALUES (N'VaiTro'),
           (N'ChuongTrinh'),
           (N'CapDo'),
           (N'KhoaHoc'),
           (N'NoiDungBaiHoc'),
           (N'LoaiDiem');

    /* -----------------------------------------------------------------------------
       2b. DANH SÁCH KHÓA HỌC CẦN XOÁ (khóa rác)
           Mặc định: MaKhoaHoc = 15 HOẶC tên là 'dfsaf'.
       ----------------------------------------------------------------------------- */
    DECLARE @KhoaHocXoa TABLE (MaKhoaHoc int NOT NULL PRIMARY KEY);

    INSERT INTO @KhoaHocXoa (MaKhoaHoc)
    SELECT k.MaKhoaHoc
    FROM dbo.KhoaHoc AS k
    WHERE k.MaKhoaHoc = 15
       OR LTRIM(RTRIM(k.TenKhoaHoc)) = N'dfsaf';

    PRINT N'';
    PRINT N'--- Khóa học sẽ bị xóa ---';

    SELECT k.MaKhoaHoc AS [Mã khóa], k.TenKhoaHoc AS [Tên khóa học]
    FROM dbo.KhoaHoc AS k
    INNER JOIN @KhoaHocXoa AS x ON x.MaKhoaHoc = k.MaKhoaHoc
    ORDER BY k.MaKhoaHoc;

    /* -----------------------------------------------------------------------------
       3. CHẶN TỰ KHOÁ MÌNH RA NGOÀI HỆ THỐNG
       ----------------------------------------------------------------------------- */
    IF NOT EXISTS
    (
        SELECT 1
        FROM dbo.TaiKhoan AS tk
        INNER JOIN dbo.VaiTro AS vt ON vt.MaVaiTro = tk.MaVaiTro
        WHERE LTRIM(RTRIM(vt.TenVaiTro)) = N'Quản trị viên'
    )
    BEGIN
        THROW 50044, N'Không tìm thấy tài khoản vai trò "Quản trị viên" — HUỶ toàn bộ, không xoá gì.', 1;
    END

    /* -----------------------------------------------------------------------------
       4. XÂY DANH SÁCH BẢNG SẼ XOÁ
       ----------------------------------------------------------------------------- */
    CREATE TABLE #Target
    (
        TenBang sysname       NOT NULL PRIMARY KEY,
        CauLenh nvarchar(max) NOT NULL
    );

    -- 4a. Các bảng KHÔNG nằm trong danh sách giữ → xoá sạch toàn bộ.
    INSERT INTO #Target (TenBang, CauLenh)
    SELECT t.name,
           N'DELETE FROM ' + QUOTENAME(s.name) + N'.' + QUOTENAME(t.name) + N';'
    FROM sys.tables AS t
    INNER JOIN sys.schemas AS s ON s.schema_id = t.schema_id
    WHERE t.is_ms_shipped = 0
      AND t.name <> N'sysdiagrams'
      AND t.name NOT IN (SELECT TenBang FROM @GiuBang)
      AND t.name NOT IN (N'TaiKhoan', N'NhanVien');

    -- 4b. NhanVien / TaiKhoan KHÔNG đưa vào #Target: hai bảng này có quan hệ 1-1
    --      (NhanVien.MaNhanVien = TaiKhoan.TenDangNhap) nên phải xoá ĐÚNG THỨ TỰ
    --      TaiKhoan trước → NhanVien sau. Xem mục 6b-1.

    /* -----------------------------------------------------------------------------
       5. ẢNH CHỤP SỐ DÒNG TRƯỚC KHI XOÁ (đọc trước transaction để xem được cả ở chế độ THỬ)
       ----------------------------------------------------------------------------- */
    CREATE TABLE #TruocDo (TenBang sysname NOT NULL PRIMARY KEY, SoDong bigint NOT NULL);

    DECLARE @bang sysname, @lenh nvarchar(max);

    DECLARE curDem CURSOR LOCAL FAST_FORWARD FOR
        SELECT TenBang, N'SELECT @n = COUNT_BIG(*) FROM ' + QUOTENAME(OBJECT_SCHEMA_NAME(OBJECT_ID(t.TenBang)))
                       + N'.' + QUOTENAME(t.TenBang) + N';'
        FROM #Target AS t;

    DECLARE @n bigint;

    OPEN curDem;
    FETCH NEXT FROM curDem INTO @bang, @lenh;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @n = 0;
        EXEC sys.sp_executesql @lenh, N'@n bigint OUTPUT', @n OUTPUT;
        INSERT INTO #TruocDo (TenBang, SoDong) VALUES (@bang, @n);
        FETCH NEXT FROM curDem INTO @bang, @lenh;
    END

    CLOSE curDem;
    DEALLOCATE curDem;

    PRINT N'';
    PRINT N'--- Dự kiến xoá ---';

    SELECT TenBang AS [Bảng], SoDong AS [Số dòng hiện có]
    FROM #TruocDo
    WHERE SoDong > 0
    ORDER BY SoDong DESC, TenBang;

    /* -----------------------------------------------------------------------------
       6. XOÁ TRONG MỘT TRANSACTION
       ----------------------------------------------------------------------------- */
    BEGIN TRANSACTION;

    -- 6a. Tắt kiểm tra khoá ngoại (DDL có tính transaction → rollback sẽ khôi phục).
    SET @lenh = N'';

    SELECT @lenh = @lenh
                 + N'ALTER TABLE '
                 + QUOTENAME(ISNULL(OBJECT_SCHEMA_NAME(fk.parent_object_id), N'dbo')) + N'.'
                 + QUOTENAME(OBJECT_NAME(fk.parent_object_id))
                 + N' NOCHECK CONSTRAINT ' + QUOTENAME(fk.name) + N';' + CHAR(13) + CHAR(10)
    FROM sys.foreign_keys AS fk;

    IF LEN(@lenh) > 0
        EXEC sys.sp_executesql @lenh;

    -- 6b. Xoá.
    DECLARE curXoa CURSOR LOCAL FAST_FORWARD FOR
        SELECT TenBang, CauLenh FROM #Target ORDER BY TenBang;

    OPEN curXoa;
    FETCH NEXT FROM curXoa INTO @bang, @lenh;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        EXEC sys.sp_executesql @lenh;
        FETCH NEXT FROM curXoa INTO @bang, @lenh;
    END

    CLOSE curXoa;
    DEALLOCATE curXoa;

    -- 6b-1. Xoá tài khoản KHÔNG phải Quản trị viên — phải chạy TRƯỚC 6b-2.
    DELETE FROM dbo.TaiKhoan
     WHERE NOT EXISTS (SELECT 1
                         FROM dbo.VaiTro AS vt
                        WHERE vt.MaVaiTro = dbo.TaiKhoan.MaVaiTro
                          AND LTRIM(RTRIM(vt.TenVaiTro)) = N'Quản trị viên');

    -- 6b-2. Xoá nhân viên không còn tài khoản tương ứng (chỉ còn admin sống sót).
    DELETE FROM dbo.NhanVien
     WHERE NOT EXISTS (SELECT 1
                         FROM dbo.TaiKhoan AS tk
                        WHERE tk.TenDangNhap = dbo.NhanVien.MaNhanVien);

    -- 6b'. Xoá khóa học rác + dữ liệu con của nó.
    --      LoaiDiem / NoiDungBaiHoc là bảng con của KhoaHoc và nằm trong danh sách GIỮ,
    --      nên phải xoá tay các dòng thuộc khóa học bị xoá (DiemSo / CaHoc đã bị xoá sạch ở 6b).
    DELETE FROM dbo.LoaiDiem
     WHERE MaKhoaHoc IN (SELECT MaKhoaHoc FROM @KhoaHocXoa);

    DELETE FROM dbo.NoiDungBaiHoc
     WHERE MaKhoaHoc IN (SELECT MaKhoaHoc FROM @KhoaHocXoa);

    DELETE FROM dbo.KhoaHoc
     WHERE MaKhoaHoc IN (SELECT MaKhoaHoc FROM @KhoaHocXoa);

    -- 6c. Bật lại khoá ngoại kèm kiểm tra toàn vẹn (sẽ báo lỗi nếu còn dòng mồ côi).
    SET @lenh = N'';

    SELECT @lenh = @lenh
                 + N'ALTER TABLE '
                 + QUOTENAME(ISNULL(OBJECT_SCHEMA_NAME(fk.parent_object_id), N'dbo')) + N'.'
                 + QUOTENAME(OBJECT_NAME(fk.parent_object_id))
                 + N' WITH CHECK CHECK CONSTRAINT ' + QUOTENAME(fk.name) + N';' + CHAR(13) + CHAR(10)
    FROM sys.foreign_keys AS fk;

    IF LEN(@lenh) > 0
        EXEC sys.sp_executesql @lenh;

    -- 6d. Chốt.
    IF @CheDo = N'XOA'
    BEGIN
        COMMIT TRANSACTION;
        PRINT N'';
        PRINT N'*** ĐÃ COMMIT — dữ liệu đã bị xoá.';
    END
    ELSE
    BEGIN
        ROLLBACK TRANSACTION;
        PRINT N'';
        PRINT N'*** CHẾ ĐỘ THỬ — đã ROLLBACK, KHÔNG có dữ liệu nào bị xoá.';
    END

    /* -----------------------------------------------------------------------------
       7. ĐẶT LẠI IDENTITY CHO CÁC BẢNG ĐÃ XOÁ SẠCH (chỉ khi xoá thật)
       ----------------------------------------------------------------------------- */
    IF @CheDo = N'XOA'
    BEGIN
        DECLARE @lenhReseed nvarchar(1000);

        DECLARE curReseed CURSOR LOCAL FAST_FORWARD FOR
            SELECT t.name
            FROM sys.tables AS t
            INNER JOIN sys.identity_columns AS ic ON ic.object_id = t.object_id
            WHERE t.is_ms_shipped = 0
              AND t.name NOT IN (SELECT TenBang FROM @GiuBang)
              AND t.name NOT IN (N'TaiKhoan', N'NhanVien');

        OPEN curReseed;
        FETCH NEXT FROM curReseed INTO @bang;

        WHILE @@FETCH_STATUS = 0
        BEGIN
            SET @lenhReseed = N'DBCC CHECKIDENT ('''
                            + QUOTENAME(OBJECT_SCHEMA_NAME(OBJECT_ID(@bang))) + N'.'
                            + QUOTENAME(@bang) + N''', RESEED, 0) WITH NO_INFOMSGS;';
            EXEC sys.sp_executesql @lenhReseed;
            FETCH NEXT FROM curReseed INTO @bang;
        END

        CLOSE curReseed;
        DEALLOCATE curReseed;

        PRINT N'*** Đã đặt lại IDENTITY của các bảng đã xoá sạch về 1.';
    END

    /* -----------------------------------------------------------------------------
       8. BÁO CÁO SAU KHI CHẠY
       ----------------------------------------------------------------------------- */
    PRINT N'';
    PRINT N'--- Bảng còn dữ liệu ---';

    SELECT t.name                          AS [Bảng],
           SUM(p.[rows])                   AS [Số dòng còn lại]
    FROM sys.tables AS t
    INNER JOIN sys.partitions AS p
            ON p.object_id = t.object_id
           AND p.index_id IN (0, 1)
    WHERE t.is_ms_shipped = 0
    GROUP BY t.name
    HAVING SUM(p.[rows]) > 0
    ORDER BY t.name;

    PRINT N'';
    PRINT N'--- Tài khoản còn lại ---';

    SELECT tk.MaTaiKhoan      AS [Mã TK],
           tk.TenDangNhap     AS [Tên đăng nhập],
           vt.TenVaiTro       AS [Vai trò],
           tk.Email           AS [Email],
           tk.TrangThaiTaiKhoan AS [Trạng thái]
    FROM dbo.TaiKhoan AS tk
    INNER JOIN dbo.VaiTro AS vt ON vt.MaVaiTro = tk.MaVaiTro
    ORDER BY tk.TenDangNhap;

    PRINT N'';
    PRINT N'=====================================================================';
    PRINT N'  XONG. Nếu đã xoá thật: ĐÓNG và CHẠY LẠI API + WinForms.';
    PRINT N'=====================================================================';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
    BEGIN
        ROLLBACK TRANSACTION;
    END

    PRINT N'';
    PRINT N'*** LỖI — đã ROLLBACK, dữ liệu giữ nguyên: ' + ERROR_MESSAGE();
    THROW;
END CATCH
GO
