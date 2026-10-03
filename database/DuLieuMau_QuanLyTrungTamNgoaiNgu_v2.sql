USE QuanLyTrungTamNgoaiNgu;
GO

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    /* ============================================================
       DỮ LIỆU MẪU ĐẦY ĐỦ CHO 32 BẢNG
       Chạy sau file QuanLyTrungTamNgoaiNgu_v2.sql
       ============================================================ */


    /* ============================================================
       1. TÀI KHOẢN
       VaiTro đã được tạo sẵn trong script schema.
       ============================================================ */

    INSERT INTO TaiKhoan
    (
        MaVaiTro,
        TenDangNhap,
        MatKhauHash,
        Email,
        TrangThaiTaiKhoan
    )
    VALUES
    ((SELECT MaVaiTro FROM VaiTro WHERE TenVaiTro = N'Quản trị viên'),
        'ADMIN001', N'HASH_ADMIN001', 'admin@trungtam.edu.vn', N'Hoạt động'),

    ((SELECT MaVaiTro FROM VaiTro WHERE TenVaiTro = N'Nhân viên học vụ'),
        'NVHV001', N'HASH_NVHV001', 'hocvu01@trungtam.edu.vn', N'Hoạt động'),

    ((SELECT MaVaiTro FROM VaiTro WHERE TenVaiTro = N'Nhân viên tư vấn'),
        'NVTV001', N'HASH_NVTV001', 'tuvan01@trungtam.edu.vn', N'Hoạt động'),

    ((SELECT MaVaiTro FROM VaiTro WHERE TenVaiTro = N'Nhân viên kế toán'),
        'NVKT001', N'HASH_NVKT001', 'ketoan01@trungtam.edu.vn', N'Hoạt động'),

    ((SELECT MaVaiTro FROM VaiTro WHERE TenVaiTro = N'Giáo viên'),
        'GV001', N'HASH_GV001', 'gv001@trungtam.edu.vn', N'Hoạt động'),

    ((SELECT MaVaiTro FROM VaiTro WHERE TenVaiTro = N'Giáo viên'),
        'GV002', N'HASH_GV002', 'gv002@trungtam.edu.vn', N'Hoạt động'),

    ((SELECT MaVaiTro FROM VaiTro WHERE TenVaiTro = N'Trợ giảng'),
        'TG001', N'HASH_TG001', 'tg001@trungtam.edu.vn', N'Hoạt động'),

    ((SELECT MaVaiTro FROM VaiTro WHERE TenVaiTro = N'Học viên'),
        'HV001', N'HASH_HV001', 'minhanh@gmail.com', N'Hoạt động'),

    ((SELECT MaVaiTro FROM VaiTro WHERE TenVaiTro = N'Học viên'),
        'HV002', N'HASH_HV002', 'quanghuy@gmail.com', N'Hoạt động'),

    ((SELECT MaVaiTro FROM VaiTro WHERE TenVaiTro = N'Học viên'),
        'HV003', N'HASH_HV003', 'ngoclinh@gmail.com', N'Hoạt động'),

    ((SELECT MaVaiTro FROM VaiTro WHERE TenVaiTro = N'Học viên'),
        'HV004', N'HASH_HV004', 'hoangnam@gmail.com', N'Hoạt động');


    /* ============================================================
       2. NHÂN VIÊN
       ============================================================ */

    INSERT INTO NhanVien
    (
        MaNhanVien,
        HoTen,
        NgaySinh,
        GioiTinh,
        SoDienThoai,
        Email,
        DiaChi,
        ChucVu,
        NgayVaoLam,
        TrangThaiLamViec
    )
    VALUES
    (
        'ADMIN001',
        N'Đỗ Minh Khoa',
        '1990-02-12',
        N'Nam',
        '0901000001',
        'admin@trungtam.edu.vn',
        N'Quận 10, TP.HCM',
        N'Quản trị viên',
        '2023-01-10',
        N'Đang làm việc'
    ),
    (
        'NVHV001',
        N'Nguyễn Thị Lan',
        '1998-05-10',
        N'Nữ',
        '0901000002',
        'hocvu01@trungtam.edu.vn',
        N'Bình Tân, TP.HCM',
        N'Nhân viên học vụ',
        '2024-06-01',
        N'Đang làm việc'
    ),
    (
        'NVTV001',
        N'Trần Minh Hoàng',
        '1999-08-20',
        N'Nam',
        '0901000003',
        'tuvan01@trungtam.edu.vn',
        N'Tân Phú, TP.HCM',
        N'Nhân viên tư vấn',
        '2025-02-01',
        N'Đang làm việc'
    ),
    (
        'NVKT001',
        N'Lê Ngọc Mai',
        '1997-03-15',
        N'Nữ',
        '0901000004',
        'ketoan01@trungtam.edu.vn',
        N'Quận 6, TP.HCM',
        N'Nhân viên kế toán',
        '2024-11-05',
        N'Đang làm việc'
    );


    /* ============================================================
       3. GIÁO VIÊN / TRỢ GIẢNG
       ============================================================ */

    INSERT INTO GiaoVien
    (
        MaGiaoVien,
        HoTen,
        NgaySinh,
        GioiTinh,
        SoDienThoai,
        Email,
        DiaChi,
        LoaiGiaoVien,
        ChuyenMon,
        SoNamKinhNghiem,
        NgayVaoLam,
        TrangThai
    )
    VALUES
    (
        'GV001',
        N'Phạm Minh Tuấn',
        '1994-04-12',
        N'Nam',
        '0912000001',
        'gv001@trungtam.edu.vn',
        N'Quận 10, TP.HCM',
        N'Giáo viên',
        N'IELTS và tiếng Anh học thuật',
        7,
        '2024-03-01',
        N'Đang làm việc'
    ),
    (
        'GV002',
        N'Nguyễn Thu Hà',
        '1993-11-25',
        N'Nữ',
        '0912000002',
        'gv002@trungtam.edu.vn',
        N'Phú Nhuận, TP.HCM',
        N'Giáo viên',
        N'Tiếng Anh giao tiếp và phát âm',
        8,
        '2023-08-15',
        N'Đang làm việc'
    ),
    (
        'TG001',
        N'Võ Thanh Thảo',
        '2001-09-20',
        N'Nữ',
        '0912000003',
        'tg001@trungtam.edu.vn',
        N'Quận 11, TP.HCM',
        N'Trợ giảng',
        N'Tiếng Anh học thuật',
        2,
        '2025-06-01',
        N'Đang làm việc'
    );


    /* ============================================================
       4. BẰNG CẤP
       ============================================================ */

    INSERT INTO BangCap
    (
        MaGiaoVien,
        TenBangCap,
        ChuyenNganh,
        CoSoDaoTao,
        NamTotNghiep,
        SoBang,
        NgayCap,
        TepMinhChung
    )
    VALUES
    (
        'GV001',
        N'Cử nhân',
        N'Ngôn ngữ Anh',
        N'Đại học Sư phạm TP.HCM',
        2016,
        N'BC-GV001-2016',
        '2016-08-20',
        N'/uploads/bangcap/GV001-cu-nhan.pdf'
    ),
    (
        'GV001',
        N'Thạc sĩ',
        N'Ngôn ngữ học ứng dụng',
        N'Đại học Khoa học Xã hội và Nhân văn TP.HCM',
        2020,
        N'TS-GV001-2020',
        '2020-10-15',
        N'/uploads/bangcap/GV001-thac-si.pdf'
    ),
    (
        'GV002',
        N'Cử nhân',
        N'Sư phạm Tiếng Anh',
        N'Đại học Sư phạm TP.HCM',
        2015,
        N'BC-GV002-2015',
        '2015-08-25',
        N'/uploads/bangcap/GV002-cu-nhan.pdf'
    ),
    (
        'TG001',
        N'Cử nhân',
        N'Ngôn ngữ Anh',
        N'Đại học Ngoại ngữ - Tin học TP.HCM',
        2023,
        N'BC-TG001-2023',
        '2023-09-01',
        N'/uploads/bangcap/TG001-cu-nhan.pdf'
    );


    /* ============================================================
       5. CHỨNG CHỈ
       ============================================================ */

    INSERT INTO ChungChi
    (
        MaGiaoVien,
        TenChungChi,
        LoaiChungChi,
        DonViCap,
        KetQua,
        SoChungChi,
        NgayCap,
        NgayHetHan,
        TepMinhChung
    )
    VALUES
    (
        'GV001',
        N'IELTS',
        N'Ngoại ngữ',
        N'British Council',
        N'8.0',
        N'IELTS-GV001-2025',
        '2025-02-10',
        '2027-02-10',
        N'/uploads/chungchi/GV001-ielts.pdf'
    ),
    (
        'GV001',
        N'TESOL',
        N'Giảng dạy',
        N'TESOL International',
        N'Đạt',
        N'TESOL-GV001-01',
        '2024-01-10',
        NULL,
        N'/uploads/chungchi/GV001-tesol.pdf'
    ),
    (
        'GV002',
        N'TOEIC',
        N'Ngoại ngữ',
        N'IIG Việt Nam',
        N'950',
        N'TOEIC-GV002-2025',
        '2025-04-05',
        '2027-04-05',
        N'/uploads/chungchi/GV002-toeic.pdf'
    ),
    (
        'TG001',
        N'IELTS',
        N'Ngoại ngữ',
        N'IDP',
        N'7.0',
        N'IELTS-TG001-2025',
        '2025-06-15',
        '2027-06-15',
        N'/uploads/chungchi/TG001-ielts.pdf'
    );


    /* ============================================================
       6. PHỤ HUYNH
       Một phụ huynh có thể đại diện cho nhiều học viên.
       ============================================================ */

    INSERT INTO PhuHuynh
    (
        HoTen,
        SoDienThoai,
        Email,
        DiaChi,
        NgheNghiep
    )
    VALUES
    (
        N'Nguyễn Văn Hùng',
        '0981000001',
        'hung.nguyen@gmail.com',
        N'Bình Tân, TP.HCM',
        N'Kinh doanh'
    ),
    (
        N'Trần Thị Hoa',
        '0981000002',
        'hoa.tran@gmail.com',
        N'Tân Phú, TP.HCM',
        N'Nhân viên văn phòng'
    ),
    (
        N'Lê Văn Sơn',
        '0981000003',
        'son.le@gmail.com',
        N'Quận 8, TP.HCM',
        N'Kỹ sư'
    );


    /* ============================================================
       7. HỌC VIÊN
       ============================================================ */

    INSERT INTO HocVien
    (
        MaHocVien,
        MaPhuHuynh,
        HoTen,
        NgaySinh,
        GioiTinh,
        SoDienThoai,
        Email,
        DiaChi,
        MoiQuanHePhuHuynh,
        MucTieuHocTap,
        TrinhDoHienTai,
        TrangThaiHocTap,
        NgayTaoHoSo,
        NgayNhapHoc
    )
    VALUES
    (
        'HV001',
        (SELECT MaPhuHuynh FROM PhuHuynh WHERE Email = 'hung.nguyen@gmail.com'),
        N'Nguyễn Minh Anh',
        '2007-06-15',
        N'Nữ',
        '0934000001',
        'minhanh@gmail.com',
        N'Bình Tân, TP.HCM',
        N'Cha',
        N'Đạt IELTS 6.5 để xét tuyển đại học',
        N'IELTS 5.0',
        N'Đang học',
        '2026-08-20 09:00:00',
        '2026-09-15'
    ),
    (
        'HV002',
        (SELECT MaPhuHuynh FROM PhuHuynh WHERE Email = 'hoa.tran@gmail.com'),
        N'Trần Quang Huy',
        '2008-11-02',
        N'Nam',
        '0934000002',
        'quanghuy@gmail.com',
        N'Tân Phú, TP.HCM',
        N'Mẹ',
        N'Cải thiện giao tiếp và phát âm',
        N'Sơ trung cấp',
        N'Đang học',
        '2026-08-22 10:30:00',
        '2026-09-16'
    ),
    (
        'HV003',
        (SELECT MaPhuHuynh FROM PhuHuynh WHERE Email = 'hung.nguyen@gmail.com'),
        N'Nguyễn Ngọc Linh',
        '2011-03-18',
        N'Nữ',
        '0934000003',
        'ngoclinh@gmail.com',
        N'Bình Tân, TP.HCM',
        N'Cha',
        N'Củng cố tiếng Anh nền tảng',
        N'Cơ bản',
        N'Đang học',
        '2026-08-25 14:00:00',
        '2026-09-20'
    ),
    (
        'HV004',
        NULL,
        N'Lê Hoàng Nam',
        '2003-09-09',
        N'Nam',
        '0934000004',
        'hoangnam@gmail.com',
        N'Quận 8, TP.HCM',
        NULL,
        N'Nâng cao tiếng Anh giao tiếp cho công việc',
        N'Trung cấp',
        N'Đang tư vấn',
        '2026-09-10 15:00:00',
        NULL
    );


    /* ============================================================
       8. CHƯƠNG TRÌNH
       ============================================================ */

    INSERT INTO ChuongTrinh
    (
        TenChuongTrinh,
        NhomDoiTuong,
        DoTuoiTu,
        DoTuoiDen,
        MucTieu,
        MoTa,
        TrangThai
    )
    VALUES
    (
        N'IELTS',
        N'Học sinh, sinh viên và người đi làm',
        15,
        NULL,
        N'Phát triển toàn diện 4 kỹ năng IELTS',
        N'Chương trình luyện thi IELTS từ nền tảng đến nâng cao',
        N'Hoạt động'
    ),
    (
        N'Tiếng Anh Giao tiếp',
        N'Sinh viên và người đi làm',
        16,
        NULL,
        N'Cải thiện phản xạ giao tiếp và phát âm',
        N'Chương trình tập trung vào giao tiếp thực tế',
        N'Hoạt động'
    ),
    (
        N'Tiếng Anh Thiếu niên',
        N'Học sinh THCS',
        11,
        15,
        N'Xây dựng nền tảng tiếng Anh toàn diện',
        N'Chương trình tiếng Anh dành cho học sinh THCS',
        N'Hoạt động'
    );


    /* ============================================================
       9. CẤP ĐỘ
       ============================================================ */

    INSERT INTO CapDo
    (
        MaChuongTrinh,
        KyHieuCapDo,
        TenCapDo,
        ThuTu,
        DieuKienDauVao,
        KetQuaDauRa,
        ThoiLuongDuKienTuan,
        TrangThai
    )
    VALUES
    (
        (SELECT MaChuongTrinh FROM ChuongTrinh WHERE TenChuongTrinh = N'IELTS'),
        'IELTS-F',
        N'IELTS Foundation',
        1,
        N'IELTS khoảng 4.0 - 5.0',
        N'Mục tiêu IELTS 5.5 - 6.0',
        12,
        N'Hoạt động'
    ),
    (
        (SELECT MaChuongTrinh FROM ChuongTrinh WHERE TenChuongTrinh = N'IELTS'),
        'IELTS-I',
        N'IELTS Intermediate',
        2,
        N'IELTS từ 5.5',
        N'Mục tiêu IELTS 6.0 - 6.5',
        12,
        N'Hoạt động'
    ),
    (
        (SELECT MaChuongTrinh FROM ChuongTrinh WHERE TenChuongTrinh = N'IELTS'),
        'IELTS-A',
        N'IELTS Advanced',
        3,
        N'IELTS từ 6.0',
        N'Mục tiêu IELTS 7.0+',
        14,
        N'Hoạt động'
    ),
    (
        (SELECT MaChuongTrinh FROM ChuongTrinh WHERE TenChuongTrinh = N'Tiếng Anh Giao tiếp'),
        'COMM-B',
        N'Giao tiếp căn bản',
        1,
        N'Không yêu cầu',
        N'Giao tiếp được trong các tình huống thường ngày',
        10,
        N'Hoạt động'
    ),
    (
        (SELECT MaChuongTrinh FROM ChuongTrinh WHERE TenChuongTrinh = N'Tiếng Anh Giao tiếp'),
        'COMM-I',
        N'Giao tiếp trung cấp',
        2,
        N'Có kiến thức tiếng Anh cơ bản',
        N'Giao tiếp tự tin trong học tập và công việc',
        10,
        N'Hoạt động'
    ),
    (
        (SELECT MaChuongTrinh FROM ChuongTrinh WHERE TenChuongTrinh = N'Tiếng Anh Thiếu niên'),
        'TEEN-1',
        N'Teen Starter',
        1,
        N'Kiến thức tiếng Anh THCS cơ bản',
        N'Củng cố từ vựng và ngữ pháp nền tảng',
        12,
        N'Hoạt động'
    );


    /* ============================================================
       10. KHÓA HỌC
       ============================================================ */

    INSERT INTO KhoaHoc
    (
        MaCapDo,
        TenKhoaHoc,
        NoiDung,
        SoBuoi,
        TongSoGio,
        HocPhiCoBan,
        NgayBatDau,
        NgayKetThuc,
        TrangThai
    )
    VALUES
    (
        (SELECT MaCapDo FROM CapDo WHERE KyHieuCapDo = 'IELTS-F'),
        N'IELTS Foundation K01',
        N'Luyện 4 kỹ năng và kỹ thuật làm bài IELTS nền tảng',
        24,
        48,
        6000000,
        '2026-09-01',
        '2026-12-31',
        N'Đang tuyển sinh'
    ),
    (
        (SELECT MaCapDo FROM CapDo WHERE KyHieuCapDo = 'IELTS-I'),
        N'IELTS Intermediate K01',
        N'Nâng cao kỹ năng làm bài IELTS',
        24,
        48,
        7200000,
        '2026-10-01',
        '2027-01-31',
        N'Đang tuyển sinh'
    ),
    (
        (SELECT MaCapDo FROM CapDo WHERE KyHieuCapDo = 'COMM-B'),
        N'English Communication Basic K01',
        N'Phát âm, phản xạ và giao tiếp cơ bản',
        20,
        40,
        4500000,
        '2026-09-01',
        '2026-12-15',
        N'Đang tuyển sinh'
    ),
    (
        (SELECT MaCapDo FROM CapDo WHERE KyHieuCapDo = 'TEEN-1'),
        N'Teen Starter K01',
        N'Từ vựng, ngữ pháp và giao tiếp cho học sinh THCS',
        24,
        48,
        4800000,
        '2026-09-10',
        '2026-12-31',
        N'Đang tuyển sinh'
    );


    /* ============================================================
       11. KIỂM TRA ĐẦU VÀO
       ============================================================ */

    INSERT INTO KiemTraDauVao
    (
        MaHocVien,
        MaCapDoDanhGia,
        NgayKiemTra,
        DiemNghe,
        DiemNoi,
        DiemDoc,
        DiemViet,
        TongDiem,
        NhanXet
    )
    VALUES
    (
        'HV001',
        (SELECT MaCapDo FROM CapDo WHERE KyHieuCapDo = 'IELTS-F'),
        '2026-08-25 09:00:00',
        5.0,
        5.0,
        5.5,
        4.5,
        5.0,
        N'Phù hợp IELTS Foundation'
    ),
    (
        'HV002',
        (SELECT MaCapDo FROM CapDo WHERE KyHieuCapDo = 'COMM-B'),
        '2026-08-27 14:00:00',
        6.0,
        5.5,
        5.0,
        5.0,
        5.4,
        N'Phù hợp lớp giao tiếp căn bản'
    ),
    (
        'HV003',
        (SELECT MaCapDo FROM CapDo WHERE KyHieuCapDo = 'TEEN-1'),
        '2026-08-28 15:00:00',
        6.5,
        6.0,
        6.0,
        5.5,
        6.0,
        N'Phù hợp Teen Starter'
    ),
    (
        'HV004',
        (SELECT MaCapDo FROM CapDo WHERE KyHieuCapDo = 'COMM-I'),
        '2026-09-12 10:00:00',
        6.5,
        6.5,
        6.0,
        6.0,
        6.25,
        N'Đề xuất lớp giao tiếp trung cấp'
    );


    /* ============================================================
       12. PHÒNG HỌC
       ============================================================ */

    INSERT INTO PhongHoc
    (
        MaPhong,
        TenPhong,
        ViTri,
        SucChua,
        ThietBi,
        TrangThai
    )
    VALUES
    (
        'P101',
        N'Phòng 101',
        N'Tầng 1',
        25,
        N'Máy chiếu, loa, máy lạnh',
        N'Hoạt động'
    ),
    (
        'P202',
        N'Phòng 202',
        N'Tầng 2',
        30,
        N'TV, loa, máy lạnh',
        N'Hoạt động'
    ),
    (
        'P303',
        N'Phòng 303',
        N'Tầng 3',
        20,
        N'Máy chiếu, loa, bảng tương tác',
        N'Hoạt động'
    );


    /* ============================================================
       13. LỚP HỌC
       ============================================================ */

    INSERT INTO LopHoc
    (
        MaLop,
        TenLop,
        MaKhoaHoc,
        MaNhanVienHocVu,
        NgayBatDau,
        NgayKetThucDuKien,
        NgayKetThucThucTe,
        SiSoToiDa,
        TrangThai
    )
    VALUES
    (
        'IELTS-F01',
        N'IELTS Foundation 01',
        (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'IELTS Foundation K01'),
        'NVHV001',
        '2026-09-15',
        '2026-12-15',
        NULL,
        20,
        N'Đang học'
    ),
    (
        'COMM-B01',
        N'Giao tiếp căn bản 01',
        (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'English Communication Basic K01'),
        'NVHV001',
        '2026-09-16',
        '2026-11-30',
        NULL,
        25,
        N'Đang học'
    ),
    (
        'TEEN-01',
        N'Teen Starter 01',
        (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'Teen Starter K01'),
        'NVHV001',
        '2026-09-20',
        '2026-12-20',
        NULL,
        18,
        N'Đang học'
    );


    /* ============================================================
       14. PHÂN CÔNG GIẢNG DẠY
       ============================================================ */

    INSERT INTO PhanCongGiangDay
    (
        MaLopHoc,
        MaGiaoVien,
        VaiTroPhanCong,
        NgayBatDau,
        NgayKetThuc,
        TrangThai,
        GhiChu
    )
    VALUES
    (
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'IELTS-F01'),
        'GV001',
        N'Giáo viên chính',
        '2026-09-15',
        NULL,
        N'Đang phân công',
        N'Phụ trách chính lớp IELTS Foundation 01'
    ),
    (
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'IELTS-F01'),
        'TG001',
        N'Trợ giảng',
        '2026-09-15',
        NULL,
        N'Đang phân công',
        N'Hỗ trợ lớp và điểm danh'
    ),
    (
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'COMM-B01'),
        'GV002',
        N'Giáo viên chính',
        '2026-09-16',
        NULL,
        N'Đang phân công',
        NULL
    ),
    (
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'TEEN-01'),
        'GV002',
        N'Giáo viên chính',
        '2026-09-20',
        NULL,
        N'Đang phân công',
        N'Phụ trách chương trình thiếu niên'
    ),
    (
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'IELTS-F01'),
        'GV002',
        N'Giáo viên thay thế',
        '2026-10-05',
        '2026-10-05',
        N'Đã kết thúc',
        N'Dạy thay 1 buổi do GV001 nghỉ phép'
    );


    /* ============================================================
       15. ƯU ĐÃI
       ============================================================ */

    INSERT INTO UuDai
    (
        TenUuDai,
        LoaiGiamGia,
        GiaTriGiam,
        SoKhoaHocToiThieu,
        NgayBatDau,
        NgayKetThuc,
        Poster,
        MoTa,
        TrangThai
    )
    VALUES
    (
        N'Ưu đãi đăng ký từ 2 khóa',
        N'Phần trăm',
        10,
        2,
        '2026-09-01',
        '2026-12-31',
        N'/uploads/poster/uudai-2-khoa.jpg',
        N'Giảm 10% khi học viên đăng ký từ 2 khóa học trở lên',
        N'Hoạt động'
    ),
    (
        N'Ưu đãi học viên mới',
        N'Số tiền',
        300000,
        1,
        '2026-09-01',
        '2026-10-31',
        N'/uploads/poster/hoc-vien-moi.jpg',
        N'Giảm 300.000 đồng cho học viên mới',
        N'Hoạt động'
    );


    /* ============================================================
       16. ĐĂNG KÝ HỌC
       ============================================================ */

    INSERT INTO DangKyHoc
    (
        MaHocVien,
        MaKhoaHoc,
        MaLopHoc,
        MaNhanVienTuVan,
        MaUuDai,
        NgayDangKy,
        LichHocMongMuon,
        HocPhiGoc,
        SoTienGiam,
        NgayXepLop,
        TrangThaiDangKy,
        GhiChu
    )
    VALUES
    (
        'HV001',
        (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'IELTS Foundation K01'),
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'IELTS-F01'),
        'NVTV001',
        (SELECT MaUuDai FROM UuDai WHERE TenUuDai = N'Ưu đãi học viên mới'),
        '2026-09-05 10:00:00',
        N'Thứ 2, 4 buổi tối',
        6000000,
        300000,
        '2026-09-07 14:00:00',
        N'Đã xếp lớp',
        N'Học viên ưu tiên học buổi tối'
    ),
    (
        'HV002',
        (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'English Communication Basic K01'),
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'COMM-B01'),
        'NVTV001',
        NULL,
        '2026-09-06 14:30:00',
        N'Thứ 3, 5 buổi tối',
        4500000,
        0,
        '2026-09-08 09:00:00',
        N'Đã xếp lớp',
        NULL
    ),
    (
        'HV003',
        (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'Teen Starter K01'),
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'TEEN-01'),
        'NVTV001',
        (SELECT MaUuDai FROM UuDai WHERE TenUuDai = N'Ưu đãi học viên mới'),
        '2026-09-08 08:30:00',
        N'Thứ 7 và Chủ nhật',
        4800000,
        300000,
        '2026-09-10 13:30:00',
        N'Đã xếp lớp',
        N'Phụ huynh yêu cầu ưu tiên lớp cuối tuần'
    ),
    (
        'HV004',
        (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'English Communication Basic K01'),
        NULL,
        'NVTV001',
        NULL,
        '2026-09-15 11:00:00',
        N'Thứ 3, 5 buổi tối',
        4500000,
        0,
        NULL,
        N'Chờ xử lý',
        N'Đang chờ lớp phù hợp'
    );


    /* ============================================================
       17. LỊCH HỌC
       ============================================================ */

    INSERT INTO LichHoc
    (
        MaLopHoc,
        MaPhongHoc,
        ThuTrongTuan,
        GioBatDau,
        GioKetThuc,
        NgayApDungTu,
        NgayApDungDen,
        TrangThai
    )
    VALUES
    (
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'IELTS-F01'),
        (SELECT MaPhongHoc FROM PhongHoc WHERE MaPhong = 'P101'),
        1,
        '18:00',
        '20:00',
        '2026-09-15',
        '2026-12-15',
        N'Hoạt động'
    ),
    (
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'IELTS-F01'),
        (SELECT MaPhongHoc FROM PhongHoc WHERE MaPhong = 'P101'),
        3,
        '18:00',
        '20:00',
        '2026-09-15',
        '2026-12-15',
        N'Hoạt động'
    ),
    (
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'COMM-B01'),
        (SELECT MaPhongHoc FROM PhongHoc WHERE MaPhong = 'P202'),
        2,
        '18:30',
        '20:30',
        '2026-09-16',
        '2026-11-30',
        N'Hoạt động'
    ),
    (
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'COMM-B01'),
        (SELECT MaPhongHoc FROM PhongHoc WHERE MaPhong = 'P202'),
        4,
        '18:30',
        '20:30',
        '2026-09-16',
        '2026-11-30',
        N'Hoạt động'
    ),
    (
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'TEEN-01'),
        (SELECT MaPhongHoc FROM PhongHoc WHERE MaPhong = 'P303'),
        6,
        '08:00',
        '10:00',
        '2026-09-20',
        '2026-12-20',
        N'Hoạt động'
    ),
    (
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'TEEN-01'),
        (SELECT MaPhongHoc FROM PhongHoc WHERE MaPhong = 'P303'),
        7,
        '08:00',
        '10:00',
        '2026-09-20',
        '2026-12-20',
        N'Hoạt động'
    );


    /* ============================================================
       18. CA HỌC
       ============================================================ */

    INSERT INTO CaHoc
    (
        MaLopHoc,
        MaLichHoc,
        MaPhongHoc,
        MaGiaoVienThucTe,
        MaTroGiangThucTe,
        NgayHoc,
        GioBatDau,
        GioKetThuc,
        NoiDungBaiHoc,
        TrangThai,
        LyDoThayDoi
    )
    VALUES
    (
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'IELTS-F01'),
        (
            SELECT MaLichHoc
            FROM LichHoc
            WHERE MaLopHoc = (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'IELTS-F01')
              AND ThuTrongTuan = 1
        ),
        (SELECT MaPhongHoc FROM PhongHoc WHERE MaPhong = 'P101'),
        'GV001',
        'TG001',
        '2026-09-21',
        '18:00',
        '20:00',
        N'Giới thiệu cấu trúc bài thi IELTS',
        N'Đã hoàn thành',
        NULL
    ),
    (
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'IELTS-F01'),
        (
            SELECT MaLichHoc
            FROM LichHoc
            WHERE MaLopHoc = (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'IELTS-F01')
              AND ThuTrongTuan = 3
        ),
        (SELECT MaPhongHoc FROM PhongHoc WHERE MaPhong = 'P101'),
        'GV001',
        'TG001',
        '2026-09-23',
        '18:00',
        '20:00',
        N'Listening - Form Completion',
        N'Đã hoàn thành',
        NULL
    ),
    (
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'COMM-B01'),
        (
            SELECT MaLichHoc
            FROM LichHoc
            WHERE MaLopHoc = (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'COMM-B01')
              AND ThuTrongTuan = 2
        ),
        (SELECT MaPhongHoc FROM PhongHoc WHERE MaPhong = 'P202'),
        'GV002',
        NULL,
        '2026-09-22',
        '18:30',
        '20:30',
        N'Greetings and Introductions',
        N'Đã hoàn thành',
        NULL
    ),
    (
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'TEEN-01'),
        (
            SELECT MaLichHoc
            FROM LichHoc
            WHERE MaLopHoc = (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'TEEN-01')
              AND ThuTrongTuan = 7
        ),
        (SELECT MaPhongHoc FROM PhongHoc WHERE MaPhong = 'P303'),
        'GV002',
        NULL,
        '2026-09-27',
        '08:00',
        '10:00',
        N'Vocabulary - School Life',
        N'Đã hoàn thành',
        NULL
    ),
    (
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'IELTS-F01'),
        (
            SELECT MaLichHoc
            FROM LichHoc
            WHERE MaLopHoc = (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'IELTS-F01')
              AND ThuTrongTuan = 1
        ),
        (SELECT MaPhongHoc FROM PhongHoc WHERE MaPhong = 'P202'),
        'GV002',
        'TG001',
        '2026-10-05',
        '18:00',
        '20:00',
        N'Reading - Matching Headings',
        N'Đã hoàn thành',
        N'GV001 nghỉ phép, GV002 dạy thay; P101 bảo trì nên chuyển sang P202'
    );


    /* ============================================================
       19. ĐIỂM DANH
       ============================================================ */

    INSERT INTO DiemDanh
    (
        MaCaHoc,
        MaHocVien,
        TrangThaiDiemDanh,
        ThoiGianDiemDanh,
        MaTaiKhoanThucHien,
        GhiChu
    )
    VALUES
    (
        (
            SELECT MaCaHoc FROM CaHoc
            WHERE MaLopHoc = (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'IELTS-F01')
              AND NgayHoc = '2026-09-21'
        ),
        'HV001',
        N'Có mặt',
        '2026-09-21 18:05:00',
        (SELECT MaTaiKhoan FROM TaiKhoan WHERE TenDangNhap = 'TG001'),
        NULL
    ),
    (
        (
            SELECT MaCaHoc FROM CaHoc
            WHERE MaLopHoc = (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'COMM-B01')
              AND NgayHoc = '2026-09-22'
        ),
        'HV002',
        N'Đi trễ',
        '2026-09-22 18:45:00',
        (SELECT MaTaiKhoan FROM TaiKhoan WHERE TenDangNhap = 'GV002'),
        N'Đi trễ 15 phút'
    ),
    (
        (
            SELECT MaCaHoc FROM CaHoc
            WHERE MaLopHoc = (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'TEEN-01')
              AND NgayHoc = '2026-09-27'
        ),
        'HV003',
        N'Có mặt',
        '2026-09-27 08:02:00',
        (SELECT MaTaiKhoan FROM TaiKhoan WHERE TenDangNhap = 'GV002'),
        NULL
    ),
    (
        (
            SELECT MaCaHoc FROM CaHoc
            WHERE MaLopHoc = (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'IELTS-F01')
              AND NgayHoc = '2026-10-05'
        ),
        'HV001',
        N'Vắng có phép',
        '2026-10-05 18:10:00',
        (SELECT MaTaiKhoan FROM TaiKhoan WHERE TenDangNhap = 'TG001'),
        N'Phụ huynh báo nghỉ do bệnh'
    );


    /* ============================================================
       20. LOẠI ĐIỂM
       ============================================================ */

    INSERT INTO LoaiDiem
    (
        MaKhoaHoc,
        TenLoaiDiem,
        TrongSo,
        DiemToiDa,
        ThuTu,
        TrangThai
    )
    VALUES
    (
        (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'IELTS Foundation K01'),
        N'Bài tập',
        20,
        10,
        1,
        N'Hoạt động'
    ),
    (
        (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'IELTS Foundation K01'),
        N'Giữa kỳ',
        30,
        10,
        2,
        N'Hoạt động'
    ),
    (
        (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'IELTS Foundation K01'),
        N'Cuối kỳ',
        50,
        10,
        3,
        N'Hoạt động'
    ),
    (
        (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'English Communication Basic K01'),
        N'Đánh giá nói',
        40,
        10,
        1,
        N'Hoạt động'
    ),
    (
        (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'English Communication Basic K01'),
        N'Giữa kỳ',
        20,
        10,
        2,
        N'Hoạt động'
    ),
    (
        (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'English Communication Basic K01'),
        N'Cuối kỳ',
        40,
        10,
        3,
        N'Hoạt động'
    ),
    (
        (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'Teen Starter K01'),
        N'Bài tập',
        30,
        10,
        1,
        N'Hoạt động'
    ),
    (
        (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'Teen Starter K01'),
        N'Giữa kỳ',
        30,
        10,
        2,
        N'Hoạt động'
    ),
    (
        (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'Teen Starter K01'),
        N'Cuối kỳ',
        40,
        10,
        3,
        N'Hoạt động'
    );


    /* ============================================================
       21. ĐIỂM SỐ
       ============================================================ */

    INSERT INTO DiemSo
    (
        MaDangKy,
        MaLoaiDiem,
        MaGiaoVien,
        Diem,
        NhanXet,
        TrangThaiKetQua,
        NgayCham,
        NgayCapNhat
    )
    VALUES
    (
        (
            SELECT MaDangKy FROM DangKyHoc
            WHERE MaHocVien = 'HV001'
              AND MaKhoaHoc = (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'IELTS Foundation K01')
        ),
        (
            SELECT MaLoaiDiem FROM LoaiDiem
            WHERE MaKhoaHoc = (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'IELTS Foundation K01')
              AND TenLoaiDiem = N'Bài tập'
        ),
        'GV001',
        8.5,
        N'Hoàn thành tốt bài tập',
        N'Đạt',
        '2026-09-30 20:30:00',
        NULL
    ),
    (
        (
            SELECT MaDangKy FROM DangKyHoc
            WHERE MaHocVien = 'HV001'
              AND MaKhoaHoc = (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'IELTS Foundation K01')
        ),
        (
            SELECT MaLoaiDiem FROM LoaiDiem
            WHERE MaKhoaHoc = (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'IELTS Foundation K01')
              AND TenLoaiDiem = N'Giữa kỳ'
        ),
        'GV001',
        7.5,
        N'Cần cải thiện Writing',
        N'Đạt',
        '2026-10-25 20:30:00',
        NULL
    ),
    (
        (
            SELECT MaDangKy FROM DangKyHoc
            WHERE MaHocVien = 'HV002'
              AND MaKhoaHoc = (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'English Communication Basic K01')
        ),
        (
            SELECT MaLoaiDiem FROM LoaiDiem
            WHERE MaKhoaHoc = (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'English Communication Basic K01')
              AND TenLoaiDiem = N'Đánh giá nói'
        ),
        'GV002',
        8.0,
        N'Phản xạ tốt, cần chú ý phát âm cuối từ',
        N'Đạt',
        '2026-10-01 20:45:00',
        NULL
    ),
    (
        (
            SELECT MaDangKy FROM DangKyHoc
            WHERE MaHocVien = 'HV003'
              AND MaKhoaHoc = (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'Teen Starter K01')
        ),
        (
            SELECT MaLoaiDiem FROM LoaiDiem
            WHERE MaKhoaHoc = (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'Teen Starter K01')
              AND TenLoaiDiem = N'Bài tập'
        ),
        'GV002',
        9.0,
        N'Hoàn thành đầy đủ và đúng hạn',
        N'Đạt',
        '2026-10-02 10:30:00',
        NULL
    );


    /* ============================================================
       22. HỌC PHÍ
       ============================================================ */

    INSERT INTO HocPhi
    (
        MaDangKy,
        SoTienGoc,
        SoTienGiam,
        SoTienDaDong,
        HanThanhToan,
        TrangThaiHocPhi,
        NgayTao
    )
    VALUES
    (
        (
            SELECT MaDangKy FROM DangKyHoc
            WHERE MaHocVien = 'HV001'
              AND MaKhoaHoc = (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'IELTS Foundation K01')
        ),
        6000000,
        300000,
        5700000,
        '2026-09-15',
        N'Đã thanh toán',
        '2026-09-05 10:05:00'
    ),
    (
        (
            SELECT MaDangKy FROM DangKyHoc
            WHERE MaHocVien = 'HV002'
              AND MaKhoaHoc = (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'English Communication Basic K01')
        ),
        4500000,
        0,
        2500000,
        '2026-09-20',
        N'Thanh toán một phần',
        '2026-09-06 14:35:00'
    ),
    (
        (
            SELECT MaDangKy FROM DangKyHoc
            WHERE MaHocVien = 'HV003'
              AND MaKhoaHoc = (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'Teen Starter K01')
        ),
        4800000,
        300000,
        4500000,
        '2026-09-20',
        N'Đã thanh toán',
        '2026-09-08 08:35:00'
    ),
    (
        (
            SELECT MaDangKy FROM DangKyHoc
            WHERE MaHocVien = 'HV004'
              AND MaKhoaHoc = (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'English Communication Basic K01')
        ),
        4500000,
        0,
        0,
        '2026-09-30',
        N'Chưa thanh toán',
        '2026-09-15 11:05:00'
    );


    /* ============================================================
       23. THANH TOÁN
       ============================================================ */

    INSERT INTO ThanhToan
    (
        MaHocPhi,
        MaNhanVienKeToan,
        SoTien,
        NgayThanhToan,
        PhuongThucThanhToan,
        MaGiaoDich,
        LoaiGiaoDich,
        TrangThai,
        GhiChu
    )
    VALUES
    (
        (
            SELECT MaHocPhi FROM HocPhi
            WHERE MaDangKy =
            (
                SELECT MaDangKy FROM DangKyHoc
                WHERE MaHocVien = 'HV001'
                  AND MaKhoaHoc = (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'IELTS Foundation K01')
            )
        ),
        'NVKT001',
        5700000,
        '2026-09-08 09:30:00',
        N'Chuyển khoản',
        'GD20260908001',
        N'Thu tiền',
        N'Thành công',
        N'Thanh toán đủ học phí'
    ),
    (
        (
            SELECT MaHocPhi FROM HocPhi
            WHERE MaDangKy =
            (
                SELECT MaDangKy FROM DangKyHoc
                WHERE MaHocVien = 'HV002'
                  AND MaKhoaHoc = (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'English Communication Basic K01')
            )
        ),
        'NVKT001',
        2500000,
        '2026-09-09 10:00:00',
        N'Tiền mặt',
        'GD20260909001',
        N'Thu tiền',
        N'Thành công',
        N'Đóng đợt 1'
    ),
    (
        (
            SELECT MaHocPhi FROM HocPhi
            WHERE MaDangKy =
            (
                SELECT MaDangKy FROM DangKyHoc
                WHERE MaHocVien = 'HV003'
                  AND MaKhoaHoc = (SELECT MaKhoaHoc FROM KhoaHoc WHERE TenKhoaHoc = N'Teen Starter K01')
            )
        ),
        'NVKT001',
        4500000,
        '2026-09-10 14:20:00',
        N'Chuyển khoản',
        'GD20260910001',
        N'Thu tiền',
        N'Thành công',
        N'Thanh toán đủ học phí'
    );


    /* ============================================================
       24. BIÊN NHẬN
       ============================================================ */

    INSERT INTO BienNhan
    (
        MaThanhToan,
        MaNhanVienLap,
        SoBienNhan,
        NgayLap,
        SoTienXacNhan,
        NoiDung,
        TrangThai
    )
    VALUES
    (
        (SELECT MaThanhToan FROM ThanhToan WHERE MaGiaoDich = 'GD20260908001'),
        'NVKT001',
        'BN20260908001',
        '2026-09-08 09:35:00',
        5700000,
        N'Thu học phí IELTS Foundation K01',
        N'Đã phát hành'
    ),
    (
        (SELECT MaThanhToan FROM ThanhToan WHERE MaGiaoDich = 'GD20260909001'),
        'NVKT001',
        'BN20260909001',
        '2026-09-09 10:05:00',
        2500000,
        N'Thu học phí đợt 1 - English Communication Basic K01',
        N'Đã phát hành'
    ),
    (
        (SELECT MaThanhToan FROM ThanhToan WHERE MaGiaoDich = 'GD20260910001'),
        'NVKT001',
        'BN20260910001',
        '2026-09-10 14:25:00',
        4500000,
        N'Thu học phí Teen Starter K01',
        N'Đã phát hành'
    );


    /* ============================================================
       25. DỊCH VỤ
       ============================================================ */

    INSERT INTO DichVu
    (
        TenDichVu,
        MoTa,
        TrangThai
    )
    VALUES
    (
        N'Tư vấn khóa học',
        N'Tư vấn chương trình, cấp độ và khóa học phù hợp',
        N'Hoạt động'
    ),
    (
        N'Hỗ trợ học viên',
        N'Hỗ trợ học viên trong quá trình học',
        N'Hoạt động'
    ),
    (
        N'Hỗ trợ học phí',
        N'Hỗ trợ giải đáp và xử lý các vấn đề liên quan đến học phí',
        N'Hoạt động'
    );


    /* ============================================================
       26. PHẢN HỒI DỊCH VỤ
       ============================================================ */

    INSERT INTO PhanHoiDichVu
    (
        MaDichVu,
        MaHocVien,
        DiemDanhGia,
        NoiDungPhanHoi,
        ThoiGianPhanHoi,
        TrangThai
    )
    VALUES
    (
        (SELECT MaDichVu FROM DichVu WHERE TenDichVu = N'Tư vấn khóa học'),
        'HV001',
        5,
        N'Nhân viên tư vấn giải thích rõ ràng và nhiệt tình.',
        '2026-09-18 20:00:00',
        N'Đã gửi'
    ),
    (
        (SELECT MaDichVu FROM DichVu WHERE TenDichVu = N'Hỗ trợ học viên'),
        'HV002',
        4,
        N'Phản hồi khá nhanh, hỗ trợ tốt.',
        '2026-09-19 19:30:00',
        N'Đã gửi'
    ),
    (
        (SELECT MaDichVu FROM DichVu WHERE TenDichVu = N'Hỗ trợ học phí'),
        'HV003',
        5,
        N'Thông tin học phí và biên nhận rõ ràng.',
        '2026-09-20 18:00:00',
        N'Đã gửi'
    );


    /* ============================================================
       27. KHẢO SÁT
       ============================================================ */

    INSERT INTO KhaoSat
    (
        TenKhaoSat,
        MoTa,
        NgayBatDau,
        NgayKetThuc,
        TrangThai
    )
    VALUES
    (
        N'Khảo sát chất lượng học kỳ 1',
        N'Đánh giá chất lượng giảng dạy và dịch vụ của trung tâm',
        '2026-09-01',
        '2026-12-31',
        N'Hoạt động'
    ),
    (
        N'Khảo sát cơ sở vật chất',
        N'Đánh giá phòng học và thiết bị',
        '2026-09-15',
        '2026-11-30',
        N'Hoạt động'
    );


    /* ============================================================
       28. CÂU HỎI KHẢO SÁT
       ============================================================ */

    INSERT INTO CauHoiKhaoSat
    (
        MaKhaoSat,
        NoiDungCauHoi,
        LoaiCauHoi,
        ThuTu,
        BatBuoc,
        TrangThai
    )
    VALUES
    (
        (SELECT MaKhaoSat FROM KhaoSat WHERE TenKhaoSat = N'Khảo sát chất lượng học kỳ 1'),
        N'Bạn đánh giá chất lượng giảng dạy như thế nào?',
        N'Đánh giá 1-5',
        1,
        1,
        N'Hoạt động'
    ),
    (
        (SELECT MaKhaoSat FROM KhaoSat WHERE TenKhaoSat = N'Khảo sát chất lượng học kỳ 1'),
        N'Bạn có góp ý gì cho giáo viên hoặc trung tâm?',
        N'Tự luận',
        2,
        0,
        N'Hoạt động'
    ),
    (
        (SELECT MaKhaoSat FROM KhaoSat WHERE TenKhaoSat = N'Khảo sát cơ sở vật chất'),
        N'Bạn đánh giá phòng học như thế nào?',
        N'Đánh giá 1-5',
        1,
        1,
        N'Hoạt động'
    ),
    (
        (SELECT MaKhaoSat FROM KhaoSat WHERE TenKhaoSat = N'Khảo sát cơ sở vật chất'),
        N'Thiết bị nào cần được cải thiện?',
        N'Tự luận',
        2,
        0,
        N'Hoạt động'
    );


    /* ============================================================
       29. PHẢN HỒI KHẢO SÁT
       ============================================================ */

    INSERT INTO PhanHoiKhaoSat
    (
        MaKhaoSat,
        MaHocVien,
        ThoiGianGui,
        DiemDanhGiaChung,
        GhiChu
    )
    VALUES
    (
        (SELECT MaKhaoSat FROM KhaoSat WHERE TenKhaoSat = N'Khảo sát chất lượng học kỳ 1'),
        'HV001',
        '2026-10-01 20:00:00',
        4.5,
        N'Hài lòng với chương trình học'
    ),
    (
        (SELECT MaKhaoSat FROM KhaoSat WHERE TenKhaoSat = N'Khảo sát cơ sở vật chất'),
        'HV002',
        '2026-10-02 19:00:00',
        4.0,
        N'Phòng học tốt, âm thanh đôi lúc chưa rõ'
    );


    /* ============================================================
       30. CÂU TRẢ LỜI KHẢO SÁT
       ============================================================ */

    INSERT INTO CauTraLoiKhaoSat
    (
        MaPhanHoiKhaoSat,
        MaCauHoi,
        GiaTriDanhGia,
        NoiDungTraLoi
    )
    VALUES
    (
        (
            SELECT MaPhanHoiKhaoSat
            FROM PhanHoiKhaoSat
            WHERE MaKhaoSat = (SELECT MaKhaoSat FROM KhaoSat WHERE TenKhaoSat = N'Khảo sát chất lượng học kỳ 1')
              AND MaHocVien = 'HV001'
        ),
        (
            SELECT MaCauHoi
            FROM CauHoiKhaoSat
            WHERE MaKhaoSat = (SELECT MaKhaoSat FROM KhaoSat WHERE TenKhaoSat = N'Khảo sát chất lượng học kỳ 1')
              AND ThuTu = 1
        ),
        5,
        NULL
    ),
    (
        (
            SELECT MaPhanHoiKhaoSat
            FROM PhanHoiKhaoSat
            WHERE MaKhaoSat = (SELECT MaKhaoSat FROM KhaoSat WHERE TenKhaoSat = N'Khảo sát chất lượng học kỳ 1')
              AND MaHocVien = 'HV001'
        ),
        (
            SELECT MaCauHoi
            FROM CauHoiKhaoSat
            WHERE MaKhaoSat = (SELECT MaKhaoSat FROM KhaoSat WHERE TenKhaoSat = N'Khảo sát chất lượng học kỳ 1')
              AND ThuTu = 2
        ),
        NULL,
        N'Nên bổ sung thêm bài luyện Speaking sau mỗi buổi học.'
    ),
    (
        (
            SELECT MaPhanHoiKhaoSat
            FROM PhanHoiKhaoSat
            WHERE MaKhaoSat = (SELECT MaKhaoSat FROM KhaoSat WHERE TenKhaoSat = N'Khảo sát cơ sở vật chất')
              AND MaHocVien = 'HV002'
        ),
        (
            SELECT MaCauHoi
            FROM CauHoiKhaoSat
            WHERE MaKhaoSat = (SELECT MaKhaoSat FROM KhaoSat WHERE TenKhaoSat = N'Khảo sát cơ sở vật chất')
              AND ThuTu = 1
        ),
        4,
        NULL
    ),
    (
        (
            SELECT MaPhanHoiKhaoSat
            FROM PhanHoiKhaoSat
            WHERE MaKhaoSat = (SELECT MaKhaoSat FROM KhaoSat WHERE TenKhaoSat = N'Khảo sát cơ sở vật chất')
              AND MaHocVien = 'HV002'
        ),
        (
            SELECT MaCauHoi
            FROM CauHoiKhaoSat
            WHERE MaKhaoSat = (SELECT MaKhaoSat FROM KhaoSat WHERE TenKhaoSat = N'Khảo sát cơ sở vật chất')
              AND ThuTu = 2
        ),
        NULL,
        N'Nên kiểm tra hệ thống loa ở phòng P202.'
    );


    /* ============================================================
       31. THÔNG BÁO
       ============================================================ */

    INSERT INTO ThongBao
    (
        MaTaiKhoanGui,
        MaTaiKhoanNhan,
        MaLopHoc,
        TieuDe,
        NoiDung,
        LoaiThongBao,
        ThoiGianGui,
        TrangThai
    )
    VALUES
    (
        (SELECT MaTaiKhoan FROM TaiKhoan WHERE TenDangNhap = 'NVHV001'),
        NULL,
        (SELECT MaLopHoc FROM LopHoc WHERE MaLop = 'IELTS-F01'),
        N'Thông báo lịch học',
        N'Lớp IELTS Foundation 01 học lúc 18:00 tại phòng P101.',
        N'Lịch học',
        '2026-09-14 10:00:00',
        N'Đã gửi'
    ),
    (
        (SELECT MaTaiKhoan FROM TaiKhoan WHERE TenDangNhap = 'GV001'),
        (SELECT MaTaiKhoan FROM TaiKhoan WHERE TenDangNhap = 'HV001'),
        NULL,
        N'Nhắc nộp bài tập',
        N'Vui lòng hoàn thành bài Reading trước buổi học tiếp theo.',
        N'Học tập',
        '2026-09-25 20:30:00',
        N'Đã gửi'
    ),
    (
        (SELECT MaTaiKhoan FROM TaiKhoan WHERE TenDangNhap = 'NVKT001'),
        (SELECT MaTaiKhoan FROM TaiKhoan WHERE TenDangNhap = 'HV002'),
        NULL,
        N'Nhắc học phí',
        N'Học viên còn 2.000.000 đồng học phí chưa thanh toán.',
        N'Học phí',
        '2026-09-20 09:00:00',
        N'Đã gửi'
    );


    COMMIT TRANSACTION;

    PRINT N'Đã thêm dữ liệu mẫu đầy đủ thành công.';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;

    PRINT N'Lỗi khi thêm dữ liệu mẫu:';
    PRINT ERROR_MESSAGE();

    THROW;
END CATCH;
GO


/* ============================================================
   KIỂM TRA NHANH SAU KHI IMPORT
   ============================================================ */

SELECT N'VaiTro' AS TenBang, COUNT(*) AS SoDong FROM VaiTro
UNION ALL
SELECT N'TaiKhoan', COUNT(*) FROM TaiKhoan
UNION ALL
SELECT N'NhanVien', COUNT(*) FROM NhanVien
UNION ALL
SELECT N'GiaoVien', COUNT(*) FROM GiaoVien
UNION ALL
SELECT N'BangCap', COUNT(*) FROM BangCap
UNION ALL
SELECT N'ChungChi', COUNT(*) FROM ChungChi
UNION ALL
SELECT N'PhuHuynh', COUNT(*) FROM PhuHuynh
UNION ALL
SELECT N'HocVien', COUNT(*) FROM HocVien
UNION ALL
SELECT N'ChuongTrinh', COUNT(*) FROM ChuongTrinh
UNION ALL
SELECT N'CapDo', COUNT(*) FROM CapDo
UNION ALL
SELECT N'KhoaHoc', COUNT(*) FROM KhoaHoc
UNION ALL
SELECT N'KiemTraDauVao', COUNT(*) FROM KiemTraDauVao
UNION ALL
SELECT N'PhongHoc', COUNT(*) FROM PhongHoc
UNION ALL
SELECT N'LopHoc', COUNT(*) FROM LopHoc
UNION ALL
SELECT N'PhanCongGiangDay', COUNT(*) FROM PhanCongGiangDay
UNION ALL
SELECT N'UuDai', COUNT(*) FROM UuDai
UNION ALL
SELECT N'DangKyHoc', COUNT(*) FROM DangKyHoc
UNION ALL
SELECT N'LichHoc', COUNT(*) FROM LichHoc
UNION ALL
SELECT N'CaHoc', COUNT(*) FROM CaHoc
UNION ALL
SELECT N'DiemDanh', COUNT(*) FROM DiemDanh
UNION ALL
SELECT N'LoaiDiem', COUNT(*) FROM LoaiDiem
UNION ALL
SELECT N'DiemSo', COUNT(*) FROM DiemSo
UNION ALL
SELECT N'HocPhi', COUNT(*) FROM HocPhi
UNION ALL
SELECT N'ThanhToan', COUNT(*) FROM ThanhToan
UNION ALL
SELECT N'BienNhan', COUNT(*) FROM BienNhan
UNION ALL
SELECT N'DichVu', COUNT(*) FROM DichVu
UNION ALL
SELECT N'PhanHoiDichVu', COUNT(*) FROM PhanHoiDichVu
UNION ALL
SELECT N'KhaoSat', COUNT(*) FROM KhaoSat
UNION ALL
SELECT N'CauHoiKhaoSat', COUNT(*) FROM CauHoiKhaoSat
UNION ALL
SELECT N'PhanHoiKhaoSat', COUNT(*) FROM PhanHoiKhaoSat
UNION ALL
SELECT N'CauTraLoiKhaoSat', COUNT(*) FROM CauTraLoiKhaoSat
UNION ALL
SELECT N'ThongBao', COUNT(*) FROM ThongBao
ORDER BY TenBang;
GO


/* ============================================================
   TRUY VẤN DEMO 1:
   Xem học viên + khóa học + lớp + học phí
   ============================================================ */

SELECT
    hv.MaHocVien,
    hv.HoTen AS HocVien,
    ct.TenChuongTrinh,
    cd.TenCapDo,
    kh.TenKhoaHoc,
    lh.MaLop,
    lh.TenLop,
    dk.NgayDangKy,
    dk.HocPhiSauGiam,
    hp.SoTienDaDong,
    hp.SoTienConThieu,
    hp.TrangThaiHocPhi
FROM DangKyHoc dk
JOIN HocVien hv
    ON dk.MaHocVien = hv.MaHocVien
JOIN KhoaHoc kh
    ON dk.MaKhoaHoc = kh.MaKhoaHoc
JOIN CapDo cd
    ON kh.MaCapDo = cd.MaCapDo
JOIN ChuongTrinh ct
    ON cd.MaChuongTrinh = ct.MaChuongTrinh
LEFT JOIN LopHoc lh
    ON dk.MaLopHoc = lh.MaLopHoc
LEFT JOIN HocPhi hp
    ON dk.MaDangKy = hp.MaDangKy
ORDER BY hv.MaHocVien;
GO


/* ============================================================
   TRUY VẤN DEMO 2:
   Xem lịch sử phân công giảng dạy
   ============================================================ */

SELECT
    lh.MaLop,
    lh.TenLop,
    gv.MaGiaoVien,
    gv.HoTen AS GiaoVien,
    pc.VaiTroPhanCong,
    pc.NgayBatDau,
    pc.NgayKetThuc,
    pc.TrangThai,
    pc.GhiChu
FROM PhanCongGiangDay pc
JOIN LopHoc lh
    ON pc.MaLopHoc = lh.MaLopHoc
JOIN GiaoVien gv
    ON pc.MaGiaoVien = gv.MaGiaoVien
ORDER BY lh.MaLop, pc.NgayBatDau;
GO


/* ============================================================
   TRUY VẤN DEMO 3:
   Xem điểm danh
   ============================================================ */

SELECT
    ch.NgayHoc,
    lh.MaLop,
    hv.MaHocVien,
    hv.HoTen,
    dd.TrangThaiDiemDanh,
    dd.ThoiGianDiemDanh,
    tk.TenDangNhap AS NguoiDiemDanh,
    dd.GhiChu
FROM DiemDanh dd
JOIN CaHoc ch
    ON dd.MaCaHoc = ch.MaCaHoc
JOIN LopHoc lh
    ON ch.MaLopHoc = lh.MaLopHoc
JOIN HocVien hv
    ON dd.MaHocVien = hv.MaHocVien
JOIN TaiKhoan tk
    ON dd.MaTaiKhoanThucHien = tk.MaTaiKhoan
ORDER BY ch.NgayHoc, hv.MaHocVien;
GO
