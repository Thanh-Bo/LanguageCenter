/* =====================================================================================
   ChinhDauDiem_GiuaKyCuoiKy_v23.sql
   -------------------------------------------------------------------------------------
   MỤC ĐÍCH: Giữa kỳ có 2 bài, Cuối kỳ có 2 bài → tách thành 4 đầu điểm riêng để chấm
             từng bài, thay vì dồn 2 bài vào 1 cột.

   TRƯỚC:  Chuyên cần 10 · Bài tập 20 · Giữa kỳ 30 · Cuối kỳ 40                    = 100%
   SAU:    Chuyên cần 10 · Bài tập 20 · Giữa kỳ 1 15 · Giữa kỳ 2 15
                                       · Cuối kỳ 1 20 · Cuối kỳ 2 20               = 100%

   CÁCH LÀM: KHÔNG xóa đầu điểm cũ (để không mất điểm đã chấm). Đầu điểm "Giữa kỳ" được
             ĐỔI TÊN thành "Giữa kỳ 1" và chia đôi trọng số, rồi thêm mới "Giữa kỳ 2".
             Tương tự với "Cuối kỳ" → "Cuối kỳ 1" + "Cuối kỳ 2".

   LƯU Ý:
     • Chạy được nhiều lần (idempotent) — lần 2 trở đi sẽ tự bỏ qua.
     • Không cần sửa code web / WinForms: lưới điểm tự hiện thêm cột theo dữ liệu API.
     • Ô "Lần kiểm tra" trong lưới điểm để nguyên = 1. Chỉ khi một bài được thi lại
       mới nhập Lần 2/3 — hệ thống luôn lấy điểm của LẦN GẦN NHẤT.
     • Bảng LoaiDiem có UNIQUE (MaKhoaHoc, ThuTu) → đầu điểm mới được chèn với ThuTu tạm
       (MAX+1), sau đó BƯỚC 3 đánh số lại toàn bộ về 1..6 (làm 2 lượt để không vướng UNIQUE).
   -------------------------------------------------------------------------------------
   Đổi @MaKhoaHoc nếu muốn áp dụng cho khóa học khác (14 = IELTS Advanced).
   ===================================================================================== */

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @MaKhoaHoc int = 14;
    DECLARE @ThuTuTam int;   -- chỗ đứng tạm cho đầu điểm mới (tránh UNIQUE MaKhoaHoc+ThuTu)

    IF NOT EXISTS (SELECT 1 FROM dbo.LoaiDiem WHERE MaKhoaHoc = @MaKhoaHoc)
    BEGIN
        RAISERROR(N'Không tìm thấy đầu điểm nào của khóa học %d.', 16, 1, @MaKhoaHoc);
    END

    PRINT N'=== KHÓA HỌC ' + CAST(@MaKhoaHoc AS nvarchar(10)) + N' ===';
    PRINT N'--- Trước khi chỉnh ---';

    SELECT MaLoaiDiem, TenLoaiDiem, TrongSo, DiemToiDa, ThuTu, TrangThai
    FROM dbo.LoaiDiem
    WHERE MaKhoaHoc = @MaKhoaHoc
    ORDER BY ThuTu, MaLoaiDiem;

    /* ---------------------------------------------------------------------------------
       BƯỚC 1: GIỮA KỲ  →  "Giữa kỳ 1" (½ trọng số)  +  "Giữa kỳ 2" (½ trọng số)
       --------------------------------------------------------------------------------- */
    DECLARE @GiuaKy int,
            @TrongSoGiuaKy decimal(5, 2),
            @DiemToiDaGiuaKy decimal(6, 2);

    SELECT @GiuaKy = MaLoaiDiem,
           @TrongSoGiuaKy = TrongSo,
           @DiemToiDaGiuaKy = DiemToiDa
    FROM dbo.LoaiDiem
    WHERE MaKhoaHoc = @MaKhoaHoc
      AND TenLoaiDiem = N'Giữa kỳ';

    IF @GiuaKy IS NOT NULL
    BEGIN
        UPDATE dbo.LoaiDiem
        SET TenLoaiDiem = N'Giữa kỳ 1',
            TrongSo = @TrongSoGiuaKy / 2
        WHERE MaLoaiDiem = @GiuaKy;

        -- ThuTu chỉ là số tạm, BƯỚC 3 sẽ đánh số lại toàn bộ
        SET @ThuTuTam = (SELECT ISNULL(MAX(ThuTu), 0) + 1
                         FROM dbo.LoaiDiem
                         WHERE MaKhoaHoc = @MaKhoaHoc);

        INSERT INTO dbo.LoaiDiem (MaKhoaHoc, TenLoaiDiem, TrongSo, DiemToiDa, ThuTu, TrangThai)
        VALUES (@MaKhoaHoc, N'Giữa kỳ 2', @TrongSoGiuaKy / 2, @DiemToiDaGiuaKy, @ThuTuTam, N'Hoạt động');

        PRINT N'  • Giữa kỳ → Giữa kỳ 1 + Giữa kỳ 2 (mỗi bài ' +
              CAST(@TrongSoGiuaKy / 2 AS nvarchar(10)) + N'%)';
    END
    ELSE
    BEGIN
        PRINT N'  • Giữa kỳ: đã tách trước đó (hoặc khóa này không có) → bỏ qua.';
    END

    /* ---------------------------------------------------------------------------------
       BƯỚC 2: CUỐI KỲ  →  "Cuối kỳ 1" (½ trọng số)  +  "Cuối kỳ 2" (½ trọng số)
       --------------------------------------------------------------------------------- */
    DECLARE @CuoiKy int,
            @TrongSoCuoiKy decimal(5, 2),
            @DiemToiDaCuoiKy decimal(6, 2);

    SELECT @CuoiKy = MaLoaiDiem,
           @TrongSoCuoiKy = TrongSo,
           @DiemToiDaCuoiKy = DiemToiDa
    FROM dbo.LoaiDiem
    WHERE MaKhoaHoc = @MaKhoaHoc
      AND TenLoaiDiem = N'Cuối kỳ';

    IF @CuoiKy IS NOT NULL
    BEGIN
        UPDATE dbo.LoaiDiem
        SET TenLoaiDiem = N'Cuối kỳ 1',
            TrongSo = @TrongSoCuoiKy / 2
        WHERE MaLoaiDiem = @CuoiKy;

        SET @ThuTuTam = (SELECT ISNULL(MAX(ThuTu), 0) + 1
                         FROM dbo.LoaiDiem
                         WHERE MaKhoaHoc = @MaKhoaHoc);

        INSERT INTO dbo.LoaiDiem (MaKhoaHoc, TenLoaiDiem, TrongSo, DiemToiDa, ThuTu, TrangThai)
        VALUES (@MaKhoaHoc, N'Cuối kỳ 2', @TrongSoCuoiKy / 2, @DiemToiDaCuoiKy, @ThuTuTam, N'Hoạt động');

        PRINT N'  • Cuối kỳ → Cuối kỳ 1 + Cuối kỳ 2 (mỗi bài ' +
              CAST(@TrongSoCuoiKy / 2 AS nvarchar(10)) + N'%)';
    END
    ELSE
    BEGIN
        PRINT N'  • Cuối kỳ: đã tách trước đó (hoặc khóa này không có) → bỏ qua.';
    END

    /* ---------------------------------------------------------------------------------
       BƯỚC 3: ĐÁNH SỐ LẠI THỨ TỰ ĐẦU ĐIỂM (1..6)
       Làm 2 lượt: đẩy tạm ra ngoài dải 1..N rồi mới gán số cuối — tránh vi phạm
       UNIQUE (MaKhoaHoc, ThuTu) giữa các dòng trong cùng một câu lệnh.
       --------------------------------------------------------------------------------- */

    -- 3a. đẩy tạm
    ;WITH Tam AS (
        SELECT MaLoaiDiem,
               1000 + ROW_NUMBER() OVER (ORDER BY MaLoaiDiem) AS ThuTuTam
        FROM dbo.LoaiDiem
        WHERE MaKhoaHoc = @MaKhoaHoc
    )
    UPDATE ld
    SET ThuTu = t.ThuTuTam
    FROM dbo.LoaiDiem AS ld
    INNER JOIN Tam AS t ON t.MaLoaiDiem = ld.MaLoaiDiem;

    -- 3b. gán số cuối theo thứ tự mong muốn
    ;WITH XepHang AS (
        SELECT MaLoaiDiem,
               ROW_NUMBER() OVER (
                   ORDER BY CASE TenLoaiDiem
                                WHEN N'Chuyên cần' THEN 1
                                WHEN N'Bài tập'    THEN 2
                                WHEN N'Giữa kỳ 1'  THEN 3
                                WHEN N'Giữa kỳ 2'  THEN 4
                                WHEN N'Cuối kỳ 1'  THEN 5
                                WHEN N'Cuối kỳ 2'  THEN 6
                                ELSE 50
                            END,
                            ThuTu,
                            MaLoaiDiem) AS ThuTuMoi
        FROM dbo.LoaiDiem
        WHERE MaKhoaHoc = @MaKhoaHoc
    )
    UPDATE ld
    SET ThuTu = x.ThuTuMoi
    FROM dbo.LoaiDiem AS ld
    INNER JOIN XepHang AS x ON x.MaLoaiDiem = ld.MaLoaiDiem;

    /* ---------------------------------------------------------------------------------
       BƯỚC 4: KIỂM TRA TỔNG TRỌNG SỐ
       --------------------------------------------------------------------------------- */
    DECLARE @TongTrongSo decimal(5, 2) =
        (SELECT SUM(TrongSo)
         FROM dbo.LoaiDiem
         WHERE MaKhoaHoc = @MaKhoaHoc
           AND TrangThai = N'Hoạt động');

    IF @TongTrongSo IS NULL
    BEGIN
        PRINT N'  ! Khóa học không có đầu điểm nào đang hoạt động.';
    END
    ELSE IF @TongTrongSo <> 100
    BEGIN
        PRINT N'  ! Tổng trọng số đang là ' + CAST(@TongTrongSo AS nvarchar(10)) +
              N'% (nên bằng 100%) → kiểm tra lại.';
    END
    ELSE
    BEGIN
        PRINT N'  ✓ Tổng trọng số = 100%.';
    END

    PRINT N'--- Sau khi chỉnh ---';

    SELECT MaLoaiDiem, TenLoaiDiem, TrongSo, DiemToiDa, ThuTu, TrangThai
    FROM dbo.LoaiDiem
    WHERE MaKhoaHoc = @MaKhoaHoc
    ORDER BY ThuTu, MaLoaiDiem;

    COMMIT TRANSACTION;

    PRINT N'=== HOÀN TẤT ===';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT N'LỖI: ' + ERROR_MESSAGE();
    THROW;
END CATCH;
