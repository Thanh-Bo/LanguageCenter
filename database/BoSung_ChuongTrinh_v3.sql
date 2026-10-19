/* =============================================================================
   BỔ SUNG DANH MỤC 5 CHƯƠNG TRÌNH / 14 CẤP ĐỘ / 14 KHÓA HỌC
   -----------------------------------------------------------------------------
   Mục tiêu : đưa database về đúng mô hình 5 chương trình phân theo độ tuổi:
              1. Tiếng Anh Mầm non       (PRE-1, PRE-2)
              2. Tiếng Anh Tiểu học      (PRI-S, PRI-M, PRI-F)
              3. Tiếng Anh THCS          (TEEN-1, TEEN-2, TEEN-3)
              4. Tiếng Anh Giao tiếp     (COMM-B, COMM-I, COMM-N)
              5. IELTS                   (IELTS-F, IELTS-I, IELTS-A)

   Học phí  : giữ nguyên 4 giá đã có trong KhoaHoc, bổ sung 10 khóa còn thiếu.
              Đơn vị: đồng / khóa (đúng bằng số buổi của khóa đó).

   An toàn  : script chạy lại nhiều lần vẫn không tạo bản ghi trùng
              (mọi INSERT đều có điều kiện IF NOT EXISTS).

   Cách chạy: mở file này trong SSMS / Azure Data Studio và Execute (F5).
   ============================================================================= */

USE QuanLyTrungTamNgoaiNgu;
GO

/* -----------------------------------------------------------------------------
   1. ĐỔI TÊN "Tiếng Anh Thiếu niên" -> "Tiếng Anh THCS"
   ----------------------------------------------------------------------------- */
IF EXISTS (SELECT 1 FROM ChuongTrinh WHERE TenChuongTrinh = N'Tiếng Anh Thiếu niên')
   AND NOT EXISTS (SELECT 1 FROM ChuongTrinh WHERE TenChuongTrinh = N'Tiếng Anh THCS')
BEGIN
    UPDATE ChuongTrinh
       SET TenChuongTrinh = N'Tiếng Anh THCS',
           NhomDoiTuong  = N'Học sinh THCS (lớp 6 – 9)',
           MucTieu       = N'Bám chương trình phổ thông, nâng điểm môn Tiếng Anh',
           MoTa          = N'Chương trình tiếng Anh dành cho học sinh THCS'
     WHERE TenChuongTrinh = N'Tiếng Anh Thiếu niên';

    PRINT N'[OK] Đổi tên chương trình: Tiếng Anh Thiếu niên -> Tiếng Anh THCS';
END
ELSE
    PRINT N'[SKIP] Chương trình "Tiếng Anh THCS" đã tồn tại hoặc không tìm thấy tên cũ.';
GO


/* -----------------------------------------------------------------------------
   2. BỔ SUNG 2 CHƯƠNG TRÌNH CÒN THIẾU: Mầm non, Tiểu học
   ----------------------------------------------------------------------------- */
INSERT INTO ChuongTrinh (TenChuongTrinh, NhomDoiTuong, DoTuoiTu, DoTuoiDen, MucTieu, MoTa, TrangThai)
SELECT N'Tiếng Anh Mầm non', N'Trẻ mẫu giáo', 4, 6,
       N'Làm quen tiếng Anh, phonics và phản xạ nghe – nói',
       N'Chương trình tiếng Anh mầm non: lớp nhỏ, luôn có giáo viên chính và trợ giảng',
       N'Hoạt động'
WHERE NOT EXISTS (SELECT 1 FROM ChuongTrinh WHERE TenChuongTrinh = N'Tiếng Anh Mầm non');

INSERT INTO ChuongTrinh (TenChuongTrinh, NhomDoiTuong, DoTuoiTu, DoTuoiDen, MucTieu, MoTa, TrangThai)
SELECT N'Tiếng Anh Tiểu học', N'Học sinh tiểu học', 6, 11,
       N'Nền tảng 4 kỹ năng theo khung Cambridge Young Learners',
       N'Chương trình tiếng Anh tiểu học: Starters – Movers – Flyers',
       N'Hoạt động'
WHERE NOT EXISTS (SELECT 1 FROM ChuongTrinh WHERE TenChuongTrinh = N'Tiếng Anh Tiểu học');
GO


/* -----------------------------------------------------------------------------
   3. BỔ SUNG 8 CẤP ĐỘ CÒN THIẾU (tổng thành 14)
   ----------------------------------------------------------------------------- */
INSERT INTO CapDo (MaChuongTrinh, KyHieuCapDo, TenCapDo, ThuTu, DieuKienDauVao, KetQuaDauRa, ThoiLuongDuKienTuan, TrangThai)
SELECT ct.MaChuongTrinh, v.KyHieu, v.Ten, v.ThuTu, v.DauVao, v.DauRa, v.Tuan, N'Hoạt động'
FROM (VALUES
    (N'Tiếng Anh Mầm non', 'PRE-1', N'Preschool 1', 1,
     N'Trẻ 4 – 5 tuổi, không yêu cầu kiểm tra trình độ',
     N'Nhận biết 26 âm phonics, 150 – 200 từ theo chủ đề', 10),
    (N'Tiếng Anh Mầm non', 'PRE-2', N'Preschool 2', 2,
     N'Trẻ 5 – 6 tuổi hoặc hoàn thành Preschool 1',
     N'300 – 400 từ, câu mẫu 4 – 5 từ, sẵn sàng vào lớp 1', 12),
    (N'Tiếng Anh Tiểu học', 'PRI-S', N'Primary Starters', 1,
     N'6 – 8 tuổi hoặc kết quả kiểm tra xếp lớp',
     N'Đạt chuẩn Cambridge Starters (Pre-A1)', 15),
    (N'Tiếng Anh Tiểu học', 'PRI-M', N'Primary Movers', 2,
     N'Hoàn thành Starters hoặc kiểm tra xếp lớp',
     N'Đạt chuẩn Cambridge Movers (A1)', 16),
    (N'Tiếng Anh Tiểu học', 'PRI-F', N'Primary Flyers', 3,
     N'Hoàn thành Movers hoặc kiểm tra xếp lớp',
     N'Đạt chuẩn Cambridge Flyers (A2)', 17),
    (N'Tiếng Anh THCS', 'TEEN-2', N'Teen Elementary', 2,
     N'Lớp 7 – 8 hoặc hoàn thành Teen Starter',
     N'Điểm trung bình môn Tiếng Anh trên lớp từ 8.0', 13),
    (N'Tiếng Anh THCS', 'TEEN-3', N'Teen Intermediate', 3,
     N'Lớp 8 – 9 hoặc hoàn thành Teen Elementary',
     N'Nền tảng vững để thi vào lớp 10', 14),
    (N'Tiếng Anh Giao tiếp', 'COMM-N', N'Giao tiếp nâng cao', 3,
     N'Trình độ B1 trở lên',
     N'B2 — thuyết trình và tranh luận', 14)
) v(TenChuongTrinh, KyHieu, Ten, ThuTu, DauVao, DauRa, Tuan)
JOIN ChuongTrinh ct ON ct.TenChuongTrinh = v.TenChuongTrinh
WHERE NOT EXISTS (SELECT 1 FROM CapDo c WHERE c.KyHieuCapDo = v.KyHieu);
GO


/* -----------------------------------------------------------------------------
   4. BỔ SUNG 10 KHÓA HỌC CÒN THIẾU
   -----------------------------------------------------------------------------
   4 khóa đã có (giữ nguyên):
      IELTS-F  IELTS Foundation K01        24 buổi / 48 giờ / 6.000.000
      IELTS-I  IELTS Intermediate K01      24 buổi / 48 giờ / 7.200.000
      COMM-B   English Communication Basic K01  20 buổi / 40 giờ / 4.500.000
      TEEN-1   Teen Starter K01            24 buổi / 48 giờ / 4.800.000
   ----------------------------------------------------------------------------- */

INSERT INTO KhoaHoc (MaCapDo, TenKhoaHoc, NoiDung, SoBuoi, TongSoGio, HocPhiCoBan, NgayBatDau, NgayKetThuc, TrangThai)
SELECT c.MaCapDo, N'Mầm non Preschool 1 K01',
       N'Làm quen tiếng Anh qua bài hát, trò chơi và phonics',
       20, 20.00, 3600000, '2026-11-02', '2027-01-15', N'Đang tuyển sinh'
FROM CapDo c WHERE c.KyHieuCapDo = 'PRE-1'
  AND NOT EXISTS (SELECT 1 FROM KhoaHoc k WHERE k.MaCapDo = c.MaCapDo AND k.TenKhoaHoc = N'Mầm non Preschool 1 K01');

INSERT INTO KhoaHoc (MaCapDo, TenKhoaHoc, NoiDung, SoBuoi, TongSoGio, HocPhiCoBan, NgayBatDau, NgayKetThuc, TrangThai)
SELECT c.MaCapDo, N'Mầm non Preschool 2 K01',
       N'Củng cố phonics, mẫu câu giao tiếp và chuẩn bị vào lớp 1',
       24, 24.00, 3900000, '2026-11-02', '2027-02-01', N'Đang tuyển sinh'
FROM CapDo c WHERE c.KyHieuCapDo = 'PRE-2'
  AND NOT EXISTS (SELECT 1 FROM KhoaHoc k WHERE k.MaCapDo = c.MaCapDo AND k.TenKhoaHoc = N'Mầm non Preschool 2 K01');

INSERT INTO KhoaHoc (MaCapDo, TenKhoaHoc, NoiDung, SoBuoi, TongSoGio, HocPhiCoBan, NgayBatDau, NgayKetThuc, TrangThai)
SELECT c.MaCapDo, N'Tiểu học Starters K01',
       N'Nền tảng 4 kỹ năng theo khung Cambridge Starters',
       30, 45.00, 4200000, '2026-11-03', '2027-02-15', N'Đang tuyển sinh'
FROM CapDo c WHERE c.KyHieuCapDo = 'PRI-S'
  AND NOT EXISTS (SELECT 1 FROM KhoaHoc k WHERE k.MaCapDo = c.MaCapDo AND k.TenKhoaHoc = N'Tiểu học Starters K01');

INSERT INTO KhoaHoc (MaCapDo, TenKhoaHoc, NoiDung, SoBuoi, TongSoGio, HocPhiCoBan, NgayBatDau, NgayKetThuc, TrangThai)
SELECT c.MaCapDo, N'Tiểu học Movers K01',
       N'Luyện nghe – nói – đọc – viết theo khung Cambridge Movers',
       32, 48.00, 4600000, '2026-11-03', '2027-03-01', N'Đang tuyển sinh'
FROM CapDo c WHERE c.KyHieuCapDo = 'PRI-M'
  AND NOT EXISTS (SELECT 1 FROM KhoaHoc k WHERE k.MaCapDo = c.MaCapDo AND k.TenKhoaHoc = N'Tiểu học Movers K01');

INSERT INTO KhoaHoc (MaCapDo, TenKhoaHoc, NoiDung, SoBuoi, TongSoGio, HocPhiCoBan, NgayBatDau, NgayKetThuc, TrangThai)
SELECT c.MaCapDo, N'Tiểu học Flyers K01',
       N'Hoàn thiện A2, sẵn sàng vào lớp 6',
       34, 51.00, 5000000, '2026-11-03', '2027-03-15', N'Đang tuyển sinh'
FROM CapDo c WHERE c.KyHieuCapDo = 'PRI-F'
  AND NOT EXISTS (SELECT 1 FROM KhoaHoc k WHERE k.MaCapDo = c.MaCapDo AND k.TenKhoaHoc = N'Tiểu học Flyers K01');

INSERT INTO KhoaHoc (MaCapDo, TenKhoaHoc, NoiDung, SoBuoi, TongSoGio, HocPhiCoBan, NgayBatDau, NgayKetThuc, TrangThai)
SELECT c.MaCapDo, N'Teen Elementary K01',
       N'Củng cố ngữ pháp, bám chương trình phổ thông lớp 7 – 8',
       26, 52.00, 5200000, '2026-11-04', '2027-02-05', N'Đang tuyển sinh'
FROM CapDo c WHERE c.KyHieuCapDo = 'TEEN-2'
  AND NOT EXISTS (SELECT 1 FROM KhoaHoc k WHERE k.MaCapDo = c.MaCapDo AND k.TenKhoaHoc = N'Teen Elementary K01');

INSERT INTO KhoaHoc (MaCapDo, TenKhoaHoc, NoiDung, SoBuoi, TongSoGio, HocPhiCoBan, NgayBatDau, NgayKetThuc, TrangThai)
SELECT c.MaCapDo, N'Teen Intermediate K01',
       N'Luyện 4 kỹ năng và nền tảng thi vào lớp 10',
       28, 56.00, 5600000, '2026-11-04', '2027-02-19', N'Đang tuyển sinh'
FROM CapDo c WHERE c.KyHieuCapDo = 'TEEN-3'
  AND NOT EXISTS (SELECT 1 FROM KhoaHoc k WHERE k.MaCapDo = c.MaCapDo AND k.TenKhoaHoc = N'Teen Intermediate K01');

INSERT INTO KhoaHoc (MaCapDo, TenKhoaHoc, NoiDung, SoBuoi, TongSoGio, HocPhiCoBan, NgayBatDau, NgayKetThuc, TrangThai)
SELECT c.MaCapDo, N'English Communication Intermediate K01',
       N'Phản xạ giao tiếp trong công việc, trình độ B1',
       24, 48.00, 5400000, '2026-11-05', '2027-01-28', N'Đang tuyển sinh'
FROM CapDo c WHERE c.KyHieuCapDo = 'COMM-I'
  AND NOT EXISTS (SELECT 1 FROM KhoaHoc k WHERE k.MaCapDo = c.MaCapDo AND k.TenKhoaHoc = N'English Communication Intermediate K01');

INSERT INTO KhoaHoc (MaCapDo, TenKhoaHoc, NoiDung, SoBuoi, TongSoGio, HocPhiCoBan, NgayBatDau, NgayKetThuc, TrangThai)
SELECT c.MaCapDo, N'English Communication Advanced K01',
       N'Thuyết trình, tranh luận và làm việc nhóm bằng tiếng Anh',
       28, 56.00, 6200000, '2026-11-05', '2027-02-26', N'Đang tuyển sinh'
FROM CapDo c WHERE c.KyHieuCapDo = 'COMM-N'
  AND NOT EXISTS (SELECT 1 FROM KhoaHoc k WHERE k.MaCapDo = c.MaCapDo AND k.TenKhoaHoc = N'English Communication Advanced K01');

INSERT INTO KhoaHoc (MaCapDo, TenKhoaHoc, NoiDung, SoBuoi, TongSoGio, HocPhiCoBan, NgayBatDau, NgayKetThuc, TrangThai)
SELECT c.MaCapDo, N'IELTS Advanced K01',
       N'Chuyên sâu 4 kỹ năng, mục tiêu band 7.0+',
       28, 56.00, 8400000, '2026-11-06', '2027-02-12', N'Đang tuyển sinh'
FROM CapDo c WHERE c.KyHieuCapDo = 'IELTS-A'
  AND NOT EXISTS (SELECT 1 FROM KhoaHoc k WHERE k.MaCapDo = c.MaCapDo AND k.TenKhoaHoc = N'IELTS Advanced K01');
GO


/* -----------------------------------------------------------------------------
   5. TẠO CƠ CẤU ĐIỂM CHO CÁC KHÓA HỌC CHƯA CÓ LoaiDiem
   -----------------------------------------------------------------------------
   - Chương trình Mầm non : 4 tiêu chí, thang điểm 5, mỗi tiêu chí 25%
   - Các chương trình khác: 4 đầu điểm, thang 10 (10 / 20 / 30 / 40)
   ----------------------------------------------------------------------------- */

-- 5a. Mầm non — đánh giá bằng nhận xét, thang 5
INSERT INTO LoaiDiem (MaKhoaHoc, TenLoaiDiem, TrongSo, DiemToiDa, ThuTu, TrangThai)
SELECT k.MaKhoaHoc, v.Ten, v.TrongSo, 5.00, v.ThuTu, N'Hoạt động'
FROM KhoaHoc k
JOIN CapDo c        ON c.MaCapDo = k.MaCapDo
JOIN ChuongTrinh ct ON ct.MaChuongTrinh = c.MaChuongTrinh
CROSS JOIN (VALUES (N'Nghe', 25.00, 1), (N'Nói', 25.00, 2), (N'Thái độ', 25.00, 3), (N'Tham gia', 25.00, 4)) v(Ten, TrongSo, ThuTu)
WHERE ct.TenChuongTrinh = N'Tiếng Anh Mầm non'
  AND NOT EXISTS (SELECT 1 FROM LoaiDiem l WHERE l.MaKhoaHoc = k.MaKhoaHoc);

-- 5b. Các chương trình còn lại — thang điểm 10
INSERT INTO LoaiDiem (MaKhoaHoc, TenLoaiDiem, TrongSo, DiemToiDa, ThuTu, TrangThai)
SELECT k.MaKhoaHoc, v.Ten, v.TrongSo, 10.00, v.ThuTu, N'Hoạt động'
FROM KhoaHoc k
JOIN CapDo c        ON c.MaCapDo = k.MaCapDo
JOIN ChuongTrinh ct ON ct.MaChuongTrinh = c.MaChuongTrinh
CROSS JOIN (VALUES (N'Chuyên cần', 10.00, 1), (N'Bài tập', 20.00, 2), (N'Giữa kỳ', 30.00, 3), (N'Cuối kỳ', 40.00, 4)) v(Ten, TrongSo, ThuTu)
WHERE ct.TenChuongTrinh <> N'Tiếng Anh Mầm non'
  AND NOT EXISTS (SELECT 1 FROM LoaiDiem l WHERE l.MaKhoaHoc = k.MaKhoaHoc);
GO


/* -----------------------------------------------------------------------------
   6. KIỂM TRA KẾT QUẢ
   ----------------------------------------------------------------------------- */
PRINT N'===== KIỂM TRA SAU KHI CHẠY =====';

SELECT ct.TenChuongTrinh                              AS ChuongTrinh,
       COUNT(DISTINCT c.MaCapDo)                       AS SoCapDo,
       COUNT(DISTINCT k.MaKhoaHoc)                     AS SoKhoaHoc,
       MIN(k.HocPhiCoBan)                              AS HocPhiThapNhat,
       MAX(k.HocPhiCoBan)                              AS HocPhiCaoNhat
FROM ChuongTrinh ct
LEFT JOIN CapDo   c ON c.MaChuongTrinh = ct.MaChuongTrinh
LEFT JOIN KhoaHoc k ON k.MaCapDo = c.MaCapDo
GROUP BY ct.TenChuongTrinh
ORDER BY ct.TenChuongTrinh;

SELECT N'TỔNG' AS Nhom,
       (SELECT COUNT(*) FROM ChuongTrinh) AS TongChuongTrinh,
       (SELECT COUNT(*) FROM CapDo)       AS TongCapDo,
       (SELECT COUNT(*) FROM KhoaHoc)     AS TongKhoaHoc;
GO
