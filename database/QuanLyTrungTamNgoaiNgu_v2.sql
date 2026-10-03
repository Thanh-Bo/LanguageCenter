/* ============================================================
   HỆ THỐNG QUẢN LÝ TRUNG TÂM NGOẠI NGỮ
   Microsoft SQL Server
   Phiên bản đã chỉnh sửa theo góp ý giảng viên

   THAY ĐỔI CHÍNH:
   - NguoiDung -> TaiKhoan
   - Mã NV/GV/HV được dùng làm tên đăng nhập:
       NV001, GV001, TG001, HV001...
   - Xóa bảng HocVien_PhuHuynh
   - PhuHuynh 1 - N HocVien
   - GiaoVien thêm NgayVaoLam
   - CapDo: ThoiLuongDuKien -> ThoiLuongDuKienTuan
   - KhoaHoc thêm NgayBatDau, NgayKetThuc
   ============================================================ */

USE master;
GO

/* ============================================================
   1. XÓA DATABASE CŨ VÀ TẠO LẠI
   CẢNH BÁO: ĐOẠN NÀY XÓA TOÀN BỘ DỮ LIỆU CŨ
   ============================================================ */

IF DB_ID(N'QuanLyTrungTamNgoaiNgu') IS NOT NULL
BEGIN
    ALTER DATABASE QuanLyTrungTamNgoaiNgu
    SET SINGLE_USER WITH ROLLBACK IMMEDIATE;

    DROP DATABASE QuanLyTrungTamNgoaiNgu;
END
GO

CREATE DATABASE QuanLyTrungTamNgoaiNgu;
GO

USE QuanLyTrungTamNgoaiNgu;
GO


/* ============================================================
   2. VAI TRÒ
   ============================================================ */

CREATE TABLE VaiTro
(
    MaVaiTro INT IDENTITY(1,1) PRIMARY KEY,

    TenVaiTro NVARCHAR(100) NOT NULL UNIQUE,
    MoTa NVARCHAR(500) NULL,

    TrangThai BIT NOT NULL
        CONSTRAINT DF_VaiTro_TrangThai DEFAULT 1,

    NgayTao DATETIME2 NOT NULL
        CONSTRAINT DF_VaiTro_NgayTao DEFAULT SYSDATETIME()
);
GO


/* ============================================================
   3. TÀI KHOẢN
   TenDangNhap chính là mã nghiệp vụ:
   NV001, GV001, TG001, HV001,...
   ============================================================ */

CREATE TABLE TaiKhoan
(
    MaTaiKhoan INT IDENTITY(1,1) PRIMARY KEY,

    MaVaiTro INT NOT NULL,

    TenDangNhap VARCHAR(20) NOT NULL UNIQUE,

    MatKhauHash NVARCHAR(500) NOT NULL,

    Email VARCHAR(255) NOT NULL UNIQUE,

    TrangThaiTaiKhoan NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_TaiKhoan_TrangThai DEFAULT N'Hoạt động',

    LanDangNhapCuoi DATETIME2 NULL,

    NgayTao DATETIME2 NOT NULL
        CONSTRAINT DF_TaiKhoan_NgayTao DEFAULT SYSDATETIME(),

    NgayCapNhat DATETIME2 NULL,

    CONSTRAINT FK_TaiKhoan_VaiTro
        FOREIGN KEY (MaVaiTro)
        REFERENCES VaiTro(MaVaiTro),

    CONSTRAINT CK_TaiKhoan_TrangThai
        CHECK
        (
            TrangThaiTaiKhoan IN
            (
                N'Hoạt động',
                N'Bị khóa',
                N'Ngừng sử dụng'
            )
        )
);
GO


/* ============================================================
   4. NHÂN VIÊN
   MaNhanVien đồng thời là tên đăng nhập.
   Ví dụ: NV001
   ============================================================ */

CREATE TABLE NhanVien
(
    MaNhanVien VARCHAR(20) PRIMARY KEY,

    HoTen NVARCHAR(150) NOT NULL,
    NgaySinh DATE NULL,
    GioiTinh NVARCHAR(20) NULL,

    SoDienThoai VARCHAR(20) NULL,
    Email VARCHAR(255) NULL,
    DiaChi NVARCHAR(500) NULL,

    ChucVu NVARCHAR(100) NOT NULL,
    NgayVaoLam DATE NULL,

    TrangThaiLamViec NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_NhanVien_TrangThai DEFAULT N'Đang làm việc',

    CONSTRAINT FK_NhanVien_TaiKhoan
        FOREIGN KEY (MaNhanVien)
        REFERENCES TaiKhoan(TenDangNhap)
);
GO


/* ============================================================
   5. GIÁO VIÊN / TRỢ GIẢNG
   MaGiaoVien đồng thời là tên đăng nhập.
   Ví dụ: GV001, TG001
   ============================================================ */

CREATE TABLE GiaoVien
(
    MaGiaoVien VARCHAR(20) PRIMARY KEY,

    HoTen NVARCHAR(150) NOT NULL,
    NgaySinh DATE NULL,
    GioiTinh NVARCHAR(20) NULL,

    SoDienThoai VARCHAR(20) NULL,
    Email VARCHAR(255) NULL,
    DiaChi NVARCHAR(500) NULL,

    LoaiGiaoVien NVARCHAR(50) NOT NULL,

    ChuyenMon NVARCHAR(500) NULL,
    SoNamKinhNghiem INT NULL,

    NgayVaoLam DATE NULL,

    TrangThai NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_GiaoVien_TrangThai DEFAULT N'Đang làm việc',

    CONSTRAINT FK_GiaoVien_TaiKhoan
        FOREIGN KEY (MaGiaoVien)
        REFERENCES TaiKhoan(TenDangNhap),

    CONSTRAINT CK_GiaoVien_Loai
        CHECK
        (
            LoaiGiaoVien IN
            (
                N'Giáo viên',
                N'Trợ giảng'
            )
        ),

    CONSTRAINT CK_GiaoVien_KinhNghiem
        CHECK
        (
            SoNamKinhNghiem IS NULL
            OR SoNamKinhNghiem >= 0
        )
);
GO


/* ============================================================
   6. BẰNG CẤP
   ============================================================ */

CREATE TABLE BangCap
(
    MaBangCap INT IDENTITY(1,1) PRIMARY KEY,

    MaGiaoVien VARCHAR(20) NOT NULL,

    TenBangCap NVARCHAR(150) NOT NULL,
    ChuyenNganh NVARCHAR(200) NULL,
    CoSoDaoTao NVARCHAR(255) NULL,

    NamTotNghiep INT NULL,
    SoBang NVARCHAR(100) NULL,
    NgayCap DATE NULL,

    TepMinhChung NVARCHAR(500) NULL,

    CONSTRAINT FK_BangCap_GiaoVien
        FOREIGN KEY (MaGiaoVien)
        REFERENCES GiaoVien(MaGiaoVien)
        ON DELETE CASCADE,

    CONSTRAINT CK_BangCap_NamTotNghiep
        CHECK
        (
            NamTotNghiep IS NULL
            OR NamTotNghiep BETWEEN 1900 AND 2100
        )
);
GO


/* ============================================================
   7. CHỨNG CHỈ
   ============================================================ */

CREATE TABLE ChungChi
(
    MaChungChi INT IDENTITY(1,1) PRIMARY KEY,

    MaGiaoVien VARCHAR(20) NOT NULL,

    TenChungChi NVARCHAR(150) NOT NULL,
    LoaiChungChi NVARCHAR(100) NULL,

    DonViCap NVARCHAR(255) NULL,
    KetQua NVARCHAR(100) NULL,

    SoChungChi NVARCHAR(100) NULL,

    NgayCap DATE NULL,
    NgayHetHan DATE NULL,

    TepMinhChung NVARCHAR(500) NULL,

    CONSTRAINT FK_ChungChi_GiaoVien
        FOREIGN KEY (MaGiaoVien)
        REFERENCES GiaoVien(MaGiaoVien)
        ON DELETE CASCADE,

    CONSTRAINT CK_ChungChi_Ngay
        CHECK
        (
            NgayHetHan IS NULL
            OR NgayCap IS NULL
            OR NgayHetHan >= NgayCap
        )
);
GO


/* ============================================================
   8. PHỤ HUYNH
   Một phụ huynh có thể đại diện cho nhiều học viên.
   ============================================================ */

CREATE TABLE PhuHuynh
(
    MaPhuHuynh INT IDENTITY(1,1) PRIMARY KEY,

    HoTen NVARCHAR(150) NOT NULL,

    SoDienThoai VARCHAR(20) NULL,
    Email VARCHAR(255) NULL,

    DiaChi NVARCHAR(500) NULL,
    NgheNghiep NVARCHAR(200) NULL
);
GO


/* ============================================================
   9. HỌC VIÊN
   - MaHocVien đồng thời là tên đăng nhập.
   - Mỗi học viên lưu tối đa 1 phụ huynh.
   ============================================================ */

CREATE TABLE HocVien
(
    MaHocVien VARCHAR(20) PRIMARY KEY,

    MaPhuHuynh INT NULL,

    HoTen NVARCHAR(150) NOT NULL,
    NgaySinh DATE NOT NULL,
    GioiTinh NVARCHAR(20) NULL,

    SoDienThoai VARCHAR(20) NULL,
    Email VARCHAR(255) NULL,
    DiaChi NVARCHAR(500) NULL,

    MoiQuanHePhuHuynh NVARCHAR(50) NULL,

    MucTieuHocTap NVARCHAR(1000) NULL,
    TrinhDoHienTai NVARCHAR(200) NULL,

    TrangThaiHocTap NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_HocVien_TrangThai DEFAULT N'Đang tư vấn',

    NgayTaoHoSo DATETIME2 NOT NULL
        CONSTRAINT DF_HocVien_NgayTaoHoSo DEFAULT SYSDATETIME(),

    NgayNhapHoc DATE NULL,

    CONSTRAINT FK_HocVien_TaiKhoan
        FOREIGN KEY (MaHocVien)
        REFERENCES TaiKhoan(TenDangNhap),

    CONSTRAINT FK_HocVien_PhuHuynh
        FOREIGN KEY (MaPhuHuynh)
        REFERENCES PhuHuynh(MaPhuHuynh),

    CONSTRAINT CK_HocVien_PhuHuynh
        CHECK
        (
            (MaPhuHuynh IS NULL AND MoiQuanHePhuHuynh IS NULL)
            OR
            (MaPhuHuynh IS NOT NULL AND MoiQuanHePhuHuynh IS NOT NULL)
        )
);
GO


/* ============================================================
   10. CHƯƠNG TRÌNH
   ============================================================ */

CREATE TABLE ChuongTrinh
(
    MaChuongTrinh INT IDENTITY(1,1) PRIMARY KEY,

    TenChuongTrinh NVARCHAR(200) NOT NULL UNIQUE,

    NhomDoiTuong NVARCHAR(200) NULL,

    DoTuoiTu INT NULL,
    DoTuoiDen INT NULL,

    MucTieu NVARCHAR(1000) NULL,
    MoTa NVARCHAR(MAX) NULL,

    TrangThai NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_ChuongTrinh_TrangThai DEFAULT N'Hoạt động',

    CONSTRAINT CK_ChuongTrinh_DoTuoi
        CHECK
        (
            (DoTuoiTu IS NULL OR DoTuoiTu >= 0)
            AND
            (DoTuoiDen IS NULL OR DoTuoiDen >= 0)
            AND
            (
                DoTuoiTu IS NULL
                OR DoTuoiDen IS NULL
                OR DoTuoiDen >= DoTuoiTu
            )
        )
);
GO


/* ============================================================
   11. CẤP ĐỘ
   ThoiLuongDuKienTuan: đơn vị là TUẦN
   ============================================================ */

CREATE TABLE CapDo
(
    MaCapDo INT IDENTITY(1,1) PRIMARY KEY,

    MaChuongTrinh INT NOT NULL,

    KyHieuCapDo VARCHAR(30) NULL,
    TenCapDo NVARCHAR(150) NOT NULL,

    ThuTu INT NOT NULL
        CONSTRAINT DF_CapDo_ThuTu DEFAULT 1,

    DieuKienDauVao NVARCHAR(1000) NULL,
    KetQuaDauRa NVARCHAR(1000) NULL,

    ThoiLuongDuKienTuan INT NULL,

    TrangThai NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_CapDo_TrangThai DEFAULT N'Hoạt động',

    CONSTRAINT FK_CapDo_ChuongTrinh
        FOREIGN KEY (MaChuongTrinh)
        REFERENCES ChuongTrinh(MaChuongTrinh),

    CONSTRAINT UQ_CapDo_ChuongTrinh_Ten
        UNIQUE (MaChuongTrinh, TenCapDo),

    CONSTRAINT UQ_CapDo_ChuongTrinh_ThuTu
        UNIQUE (MaChuongTrinh, ThuTu),

    CONSTRAINT CK_CapDo_ThuTu
        CHECK (ThuTu > 0),

    CONSTRAINT CK_CapDo_ThoiLuong
        CHECK
        (
            ThoiLuongDuKienTuan IS NULL
            OR ThoiLuongDuKienTuan > 0
        )
);
GO


/* ============================================================
   12. KHÓA HỌC
   Đã thêm NgayBatDau và NgayKetThuc theo góp ý giảng viên.
   ============================================================ */

CREATE TABLE KhoaHoc
(
    MaKhoaHoc INT IDENTITY(1,1) PRIMARY KEY,

    MaCapDo INT NOT NULL,

    TenKhoaHoc NVARCHAR(200) NOT NULL,
    NoiDung NVARCHAR(MAX) NULL,

    SoBuoi INT NOT NULL,
    TongSoGio DECIMAL(6,2) NULL,

    HocPhiCoBan DECIMAL(18,2) NOT NULL,

    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE NOT NULL,

    TrangThai NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_KhoaHoc_TrangThai DEFAULT N'Hoạt động',

    CONSTRAINT FK_KhoaHoc_CapDo
        FOREIGN KEY (MaCapDo)
        REFERENCES CapDo(MaCapDo),

    CONSTRAINT UQ_KhoaHoc_CapDo_Ten
        UNIQUE (MaCapDo, TenKhoaHoc),

    CONSTRAINT CK_KhoaHoc_SoBuoi
        CHECK (SoBuoi > 0),

    CONSTRAINT CK_KhoaHoc_TongGio
        CHECK
        (
            TongSoGio IS NULL
            OR TongSoGio > 0
        ),

    CONSTRAINT CK_KhoaHoc_HocPhi
        CHECK (HocPhiCoBan >= 0),

    CONSTRAINT CK_KhoaHoc_Ngay
        CHECK (NgayKetThuc >= NgayBatDau)
);
GO


/* ============================================================
   13. KIỂM TRA ĐẦU VÀO
   ============================================================ */

CREATE TABLE KiemTraDauVao
(
    MaKiemTra INT IDENTITY(1,1) PRIMARY KEY,

    MaHocVien VARCHAR(20) NOT NULL,
    MaCapDoDanhGia INT NULL,

    NgayKiemTra DATETIME2
        NOT NULL
        CONSTRAINT DF_KiemTraDauVao_Ngay DEFAULT SYSDATETIME(),

    DiemNghe DECIMAL(6,2) NULL,
    DiemNoi DECIMAL(6,2) NULL,
    DiemDoc DECIMAL(6,2) NULL,
    DiemViet DECIMAL(6,2) NULL,

    TongDiem DECIMAL(7,2) NULL,

    NhanXet NVARCHAR(1000) NULL,

    CONSTRAINT FK_KiemTraDauVao_HocVien
        FOREIGN KEY (MaHocVien)
        REFERENCES HocVien(MaHocVien),

    CONSTRAINT FK_KiemTraDauVao_CapDo
        FOREIGN KEY (MaCapDoDanhGia)
        REFERENCES CapDo(MaCapDo),

    CONSTRAINT CK_KiemTraDauVao_Diem
        CHECK
        (
            (DiemNghe IS NULL OR DiemNghe >= 0)
            AND
            (DiemNoi IS NULL OR DiemNoi >= 0)
            AND
            (DiemDoc IS NULL OR DiemDoc >= 0)
            AND
            (DiemViet IS NULL OR DiemViet >= 0)
            AND
            (TongDiem IS NULL OR TongDiem >= 0)
        )
);
GO


/* ============================================================
   14. PHÒNG HỌC
   ============================================================ */

CREATE TABLE PhongHoc
(
    MaPhongHoc INT IDENTITY(1,1) PRIMARY KEY,

    MaPhong VARCHAR(30) NOT NULL UNIQUE,
    TenPhong NVARCHAR(100) NOT NULL,

    ViTri NVARCHAR(255) NULL,

    SucChua INT NOT NULL,

    ThietBi NVARCHAR(1000) NULL,

    TrangThai NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_PhongHoc_TrangThai DEFAULT N'Hoạt động',

    CONSTRAINT CK_PhongHoc_SucChua
        CHECK (SucChua > 0)
);
GO


/* ============================================================
   15. LỚP HỌC
   Ngày lớp học vẫn giữ riêng với ngày của khóa học.
   ============================================================ */

CREATE TABLE LopHoc
(
    MaLopHoc INT IDENTITY(1,1) PRIMARY KEY,

    MaLop VARCHAR(30) NOT NULL UNIQUE,
    TenLop NVARCHAR(200) NOT NULL,

    MaKhoaHoc INT NOT NULL,
    MaNhanVienHocVu VARCHAR(20) NOT NULL,

    NgayBatDau DATE NOT NULL,
    NgayKetThucDuKien DATE NOT NULL,
    NgayKetThucThucTe DATE NULL,

    SiSoToiDa INT NOT NULL,

    TrangThai NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_LopHoc_TrangThai DEFAULT N'Chuẩn bị mở',

    CONSTRAINT FK_LopHoc_KhoaHoc
        FOREIGN KEY (MaKhoaHoc)
        REFERENCES KhoaHoc(MaKhoaHoc),

    CONSTRAINT FK_LopHoc_NhanVienHocVu
        FOREIGN KEY (MaNhanVienHocVu)
        REFERENCES NhanVien(MaNhanVien),

    CONSTRAINT CK_LopHoc_Ngay
        CHECK
        (
            NgayKetThucDuKien >= NgayBatDau
            AND
            (
                NgayKetThucThucTe IS NULL
                OR NgayKetThucThucTe >= NgayBatDau
            )
        ),

    CONSTRAINT CK_LopHoc_SiSo
        CHECK (SiSoToiDa > 0)
);
GO


/* ============================================================
   16. PHÂN CÔNG GIẢNG DẠY
   Lưu lịch sử thay đổi giáo viên/trợ giảng.
   ============================================================ */

CREATE TABLE PhanCongGiangDay
(
    MaPhanCong INT IDENTITY(1,1) PRIMARY KEY,

    MaLopHoc INT NOT NULL,
    MaGiaoVien VARCHAR(20) NOT NULL,

    VaiTroPhanCong NVARCHAR(50) NOT NULL,

    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE NULL,

    TrangThai NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_PhanCong_TrangThai DEFAULT N'Đang phân công',

    GhiChu NVARCHAR(500) NULL,

    CONSTRAINT FK_PhanCong_LopHoc
        FOREIGN KEY (MaLopHoc)
        REFERENCES LopHoc(MaLopHoc),

    CONSTRAINT FK_PhanCong_GiaoVien
        FOREIGN KEY (MaGiaoVien)
        REFERENCES GiaoVien(MaGiaoVien),

    CONSTRAINT CK_PhanCong_VaiTro
        CHECK
        (
            VaiTroPhanCong IN
            (
                N'Giáo viên chính',
                N'Giáo viên thay thế',
                N'Trợ giảng'
            )
        ),

    CONSTRAINT CK_PhanCong_Ngay
        CHECK
        (
            NgayKetThuc IS NULL
            OR NgayKetThuc >= NgayBatDau
        )
);
GO


/* ============================================================
   17. ƯU ĐÃI
   Không có FK vì đây là danh mục/chính sách ưu đãi độc lập.
   ============================================================ */

CREATE TABLE UuDai
(
    MaUuDai INT IDENTITY(1,1) PRIMARY KEY,

    TenUuDai NVARCHAR(200) NOT NULL,

    LoaiGiamGia NVARCHAR(50) NOT NULL,
    GiaTriGiam DECIMAL(18,2) NOT NULL,

    SoKhoaHocToiThieu INT NOT NULL
        CONSTRAINT DF_UuDai_SoKhoaHoc DEFAULT 1,

    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE NOT NULL,

    Poster NVARCHAR(500) NULL,
    MoTa NVARCHAR(1000) NULL,

    TrangThai NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_UuDai_TrangThai DEFAULT N'Hoạt động',

    CONSTRAINT CK_UuDai_Loai
        CHECK
        (
            LoaiGiamGia IN
            (
                N'Phần trăm',
                N'Số tiền'
            )
        ),

    CONSTRAINT CK_UuDai_GiaTri
        CHECK
        (
            GiaTriGiam > 0
            AND
            (
                LoaiGiamGia <> N'Phần trăm'
                OR GiaTriGiam <= 100
            )
        ),

    CONSTRAINT CK_UuDai_SoKhoaHoc
        CHECK (SoKhoaHocToiThieu > 0),

    CONSTRAINT CK_UuDai_Ngay
        CHECK (NgayKetThuc >= NgayBatDau)
);
GO


/* ============================================================
   18. ĐĂNG KÝ HỌC
   Học viên đăng ký KHÓA HỌC, không đăng ký trực tiếp chương trình.
   ============================================================ */

CREATE TABLE DangKyHoc
(
    MaDangKy INT IDENTITY(1,1) PRIMARY KEY,

    MaHocVien VARCHAR(20) NOT NULL,
    MaKhoaHoc INT NOT NULL,

    -- Chưa xếp lớp thì để NULL.
    MaLopHoc INT NULL,

    MaNhanVienTuVan VARCHAR(20) NOT NULL,

    MaUuDai INT NULL,

    NgayDangKy DATETIME2
        NOT NULL
        CONSTRAINT DF_DangKyHoc_Ngay DEFAULT SYSDATETIME(),

    LichHocMongMuon NVARCHAR(500) NULL,

    HocPhiGoc DECIMAL(18,2) NOT NULL,

    SoTienGiam DECIMAL(18,2)
        NOT NULL
        CONSTRAINT DF_DangKyHoc_SoTienGiam DEFAULT 0,

    HocPhiSauGiam AS
    (
        HocPhiGoc - SoTienGiam
    ) PERSISTED,

    NgayXepLop DATETIME2 NULL,

    TrangThaiDangKy NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_DangKyHoc_TrangThai DEFAULT N'Chờ xử lý',

    GhiChu NVARCHAR(500) NULL,

    CONSTRAINT FK_DangKyHoc_HocVien
        FOREIGN KEY (MaHocVien)
        REFERENCES HocVien(MaHocVien),

    CONSTRAINT FK_DangKyHoc_KhoaHoc
        FOREIGN KEY (MaKhoaHoc)
        REFERENCES KhoaHoc(MaKhoaHoc),

    CONSTRAINT FK_DangKyHoc_LopHoc
        FOREIGN KEY (MaLopHoc)
        REFERENCES LopHoc(MaLopHoc),

    CONSTRAINT FK_DangKyHoc_NhanVien
        FOREIGN KEY (MaNhanVienTuVan)
        REFERENCES NhanVien(MaNhanVien),

    CONSTRAINT FK_DangKyHoc_UuDai
        FOREIGN KEY (MaUuDai)
        REFERENCES UuDai(MaUuDai),

    CONSTRAINT CK_DangKyHoc_HocPhi
        CHECK
        (
            HocPhiGoc >= 0
            AND SoTienGiam >= 0
            AND SoTienGiam <= HocPhiGoc
        )
);
GO


/* ============================================================
   19. LỊCH HỌC
   NgayApDungTu/Den = khoảng thời gian lịch lặp có hiệu lực.
   ============================================================ */

CREATE TABLE LichHoc
(
    MaLichHoc INT IDENTITY(1,1) PRIMARY KEY,

    MaLopHoc INT NOT NULL,
    MaPhongHoc INT NOT NULL,

    -- 1 = Thứ Hai ... 7 = Chủ Nhật
    ThuTrongTuan TINYINT NOT NULL,

    GioBatDau TIME NOT NULL,
    GioKetThuc TIME NOT NULL,

    NgayApDungTu DATE NOT NULL,
    NgayApDungDen DATE NULL,

    TrangThai NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_LichHoc_TrangThai DEFAULT N'Hoạt động',

    CONSTRAINT FK_LichHoc_LopHoc
        FOREIGN KEY (MaLopHoc)
        REFERENCES LopHoc(MaLopHoc),

    CONSTRAINT FK_LichHoc_PhongHoc
        FOREIGN KEY (MaPhongHoc)
        REFERENCES PhongHoc(MaPhongHoc),

    CONSTRAINT CK_LichHoc_Thu
        CHECK (ThuTrongTuan BETWEEN 1 AND 7),

    CONSTRAINT CK_LichHoc_Gio
        CHECK (GioKetThuc > GioBatDau),

    CONSTRAINT CK_LichHoc_Ngay
        CHECK
        (
            NgayApDungDen IS NULL
            OR NgayApDungDen >= NgayApDungTu
        )
);
GO


/* ============================================================
   20. CA HỌC THỰC TẾ
   ============================================================ */

CREATE TABLE CaHoc
(
    MaCaHoc INT IDENTITY(1,1) PRIMARY KEY,

    MaLopHoc INT NOT NULL,

    -- Có thể NULL nếu là ca phát sinh ngoài lịch định kỳ.
    MaLichHoc INT NULL,

    MaPhongHoc INT NOT NULL,

    MaGiaoVienThucTe VARCHAR(20) NULL,
    MaTroGiangThucTe VARCHAR(20) NULL,

    NgayHoc DATE NOT NULL,

    GioBatDau TIME NOT NULL,
    GioKetThuc TIME NOT NULL,

    NoiDungBaiHoc NVARCHAR(MAX) NULL,

    TrangThai NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_CaHoc_TrangThai DEFAULT N'Chưa diễn ra',

    LyDoThayDoi NVARCHAR(500) NULL,

    CONSTRAINT FK_CaHoc_LopHoc
        FOREIGN KEY (MaLopHoc)
        REFERENCES LopHoc(MaLopHoc),

    CONSTRAINT FK_CaHoc_LichHoc
        FOREIGN KEY (MaLichHoc)
        REFERENCES LichHoc(MaLichHoc),

    CONSTRAINT FK_CaHoc_PhongHoc
        FOREIGN KEY (MaPhongHoc)
        REFERENCES PhongHoc(MaPhongHoc),

    CONSTRAINT FK_CaHoc_GiaoVien
        FOREIGN KEY (MaGiaoVienThucTe)
        REFERENCES GiaoVien(MaGiaoVien),

    CONSTRAINT FK_CaHoc_TroGiang
        FOREIGN KEY (MaTroGiangThucTe)
        REFERENCES GiaoVien(MaGiaoVien),

    CONSTRAINT CK_CaHoc_Gio
        CHECK (GioKetThuc > GioBatDau),

    CONSTRAINT CK_CaHoc_GiaoVienTroGiang
        CHECK
        (
            MaGiaoVienThucTe IS NULL
            OR MaTroGiangThucTe IS NULL
            OR MaGiaoVienThucTe <> MaTroGiangThucTe
        )
);
GO


/* ============================================================
   21. ĐIỂM DANH
   Giáo viên hoặc trợ giảng đều có thể thực hiện.
   ============================================================ */

CREATE TABLE DiemDanh
(
    MaDiemDanh INT IDENTITY(1,1) PRIMARY KEY,

    MaCaHoc INT NOT NULL,
    MaHocVien VARCHAR(20) NOT NULL,

    TrangThaiDiemDanh NVARCHAR(50) NOT NULL,

    ThoiGianDiemDanh DATETIME2
        NOT NULL
        CONSTRAINT DF_DiemDanh_ThoiGian DEFAULT SYSDATETIME(),

    MaTaiKhoanThucHien INT NOT NULL,

    GhiChu NVARCHAR(500) NULL,

    CONSTRAINT FK_DiemDanh_CaHoc
        FOREIGN KEY (MaCaHoc)
        REFERENCES CaHoc(MaCaHoc),

    CONSTRAINT FK_DiemDanh_HocVien
        FOREIGN KEY (MaHocVien)
        REFERENCES HocVien(MaHocVien),

    CONSTRAINT FK_DiemDanh_TaiKhoan
        FOREIGN KEY (MaTaiKhoanThucHien)
        REFERENCES TaiKhoan(MaTaiKhoan),

    CONSTRAINT UQ_DiemDanh_CaHoc_HocVien
        UNIQUE (MaCaHoc, MaHocVien),

    CONSTRAINT CK_DiemDanh_TrangThai
        CHECK
        (
            TrangThaiDiemDanh IN
            (
                N'Có mặt',
                N'Vắng',
                N'Vắng có phép',
                N'Đi trễ'
            )
        )
);
GO


/* ============================================================
   22. LOẠI ĐIỂM
   ThuTu = thứ tự hiển thị/đánh giá.
   ============================================================ */

CREATE TABLE LoaiDiem
(
    MaLoaiDiem INT IDENTITY(1,1) PRIMARY KEY,

    MaKhoaHoc INT NOT NULL,

    TenLoaiDiem NVARCHAR(150) NOT NULL,

    TrongSo DECIMAL(5,2) NOT NULL,
    DiemToiDa DECIMAL(6,2) NOT NULL,

    ThuTu INT NOT NULL
        CONSTRAINT DF_LoaiDiem_ThuTu DEFAULT 1,

    TrangThai NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_LoaiDiem_TrangThai DEFAULT N'Hoạt động',

    CONSTRAINT FK_LoaiDiem_KhoaHoc
        FOREIGN KEY (MaKhoaHoc)
        REFERENCES KhoaHoc(MaKhoaHoc),

    CONSTRAINT UQ_LoaiDiem_Ten
        UNIQUE (MaKhoaHoc, TenLoaiDiem),

    CONSTRAINT UQ_LoaiDiem_ThuTu
        UNIQUE (MaKhoaHoc, ThuTu),

    CONSTRAINT CK_LoaiDiem_TrongSo
        CHECK
        (
            TrongSo >= 0
            AND TrongSo <= 100
        ),

    CONSTRAINT CK_LoaiDiem_DiemToiDa
        CHECK (DiemToiDa > 0),

    CONSTRAINT CK_LoaiDiem_ThuTu
        CHECK (ThuTu > 0)
);
GO


/* ============================================================
   23. ĐIỂM SỐ
   ============================================================ */

CREATE TABLE DiemSo
(
    MaDiemSo INT IDENTITY(1,1) PRIMARY KEY,

    MaDangKy INT NOT NULL,
    MaLoaiDiem INT NOT NULL,
    MaGiaoVien VARCHAR(20) NOT NULL,

    Diem DECIMAL(6,2) NOT NULL,

    NhanXet NVARCHAR(1000) NULL,

    TrangThaiKetQua NVARCHAR(50) NULL,

    NgayCham DATETIME2
        NOT NULL
        CONSTRAINT DF_DiemSo_NgayCham DEFAULT SYSDATETIME(),

    NgayCapNhat DATETIME2 NULL,

    CONSTRAINT FK_DiemSo_DangKyHoc
        FOREIGN KEY (MaDangKy)
        REFERENCES DangKyHoc(MaDangKy),

    CONSTRAINT FK_DiemSo_LoaiDiem
        FOREIGN KEY (MaLoaiDiem)
        REFERENCES LoaiDiem(MaLoaiDiem),

    CONSTRAINT FK_DiemSo_GiaoVien
        FOREIGN KEY (MaGiaoVien)
        REFERENCES GiaoVien(MaGiaoVien),

    CONSTRAINT UQ_DiemSo
        UNIQUE (MaDangKy, MaLoaiDiem),

    CONSTRAINT CK_DiemSo_Diem
        CHECK (Diem >= 0)
);
GO


/* ============================================================
   24. HỌC PHÍ
   ============================================================ */

CREATE TABLE HocPhi
(
    MaHocPhi INT IDENTITY(1,1) PRIMARY KEY,

    MaDangKy INT NOT NULL UNIQUE,

    SoTienGoc DECIMAL(18,2) NOT NULL,

    SoTienGiam DECIMAL(18,2)
        NOT NULL
        CONSTRAINT DF_HocPhi_SoTienGiam DEFAULT 0,

    SoTienPhaiDong AS
    (
        SoTienGoc - SoTienGiam
    ) PERSISTED,

    SoTienDaDong DECIMAL(18,2)
        NOT NULL
        CONSTRAINT DF_HocPhi_SoTienDaDong DEFAULT 0,

    SoTienConThieu AS
    (
        (SoTienGoc - SoTienGiam) - SoTienDaDong
    ) PERSISTED,

    HanThanhToan DATE NULL,

    TrangThaiHocPhi NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_HocPhi_TrangThai DEFAULT N'Chưa thanh toán',

    NgayTao DATETIME2
        NOT NULL
        CONSTRAINT DF_HocPhi_NgayTao DEFAULT SYSDATETIME(),

    CONSTRAINT FK_HocPhi_DangKy
        FOREIGN KEY (MaDangKy)
        REFERENCES DangKyHoc(MaDangKy),

    CONSTRAINT CK_HocPhi_SoTien
        CHECK
        (
            SoTienGoc >= 0
            AND SoTienGiam >= 0
            AND SoTienGiam <= SoTienGoc
            AND SoTienDaDong >= 0
            AND SoTienDaDong <= (SoTienGoc - SoTienGiam)
        )
);
GO


/* ============================================================
   25. THANH TOÁN
   ============================================================ */

CREATE TABLE ThanhToan
(
    MaThanhToan INT IDENTITY(1,1) PRIMARY KEY,

    MaHocPhi INT NOT NULL,
    MaNhanVienKeToan VARCHAR(20) NOT NULL,

    SoTien DECIMAL(18,2) NOT NULL,

    NgayThanhToan DATETIME2
        NOT NULL
        CONSTRAINT DF_ThanhToan_Ngay DEFAULT SYSDATETIME(),

    PhuongThucThanhToan NVARCHAR(100) NOT NULL,

    MaGiaoDich VARCHAR(150) NULL,

    LoaiGiaoDich NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_ThanhToan_Loai DEFAULT N'Thu tiền',

    TrangThai NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_ThanhToan_TrangThai DEFAULT N'Thành công',

    GhiChu NVARCHAR(500) NULL,

    CONSTRAINT FK_ThanhToan_HocPhi
        FOREIGN KEY (MaHocPhi)
        REFERENCES HocPhi(MaHocPhi),

    CONSTRAINT FK_ThanhToan_NhanVien
        FOREIGN KEY (MaNhanVienKeToan)
        REFERENCES NhanVien(MaNhanVien),

    CONSTRAINT CK_ThanhToan_SoTien
        CHECK (SoTien > 0),

    CONSTRAINT CK_ThanhToan_Loai
        CHECK
        (
            LoaiGiaoDich IN
            (
                N'Thu tiền',
                N'Hoàn tiền',
                N'Điều chỉnh'
            )
        )
);
GO

CREATE UNIQUE INDEX UX_ThanhToan_MaGiaoDich
ON ThanhToan(MaGiaoDich)
WHERE MaGiaoDich IS NOT NULL;
GO


/* ============================================================
   26. BIÊN NHẬN
   SoBienNhan = mã nghiệp vụ in trên biên nhận.
   ============================================================ */

CREATE TABLE BienNhan
(
    MaBienNhan INT IDENTITY(1,1) PRIMARY KEY,

    MaThanhToan INT NOT NULL UNIQUE,

    MaNhanVienLap VARCHAR(20) NOT NULL,

    SoBienNhan VARCHAR(50) NOT NULL UNIQUE,

    NgayLap DATETIME2
        NOT NULL
        CONSTRAINT DF_BienNhan_NgayLap DEFAULT SYSDATETIME(),

    SoTienXacNhan DECIMAL(18,2) NOT NULL,

    NoiDung NVARCHAR(500) NULL,

    TrangThai NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_BienNhan_TrangThai DEFAULT N'Đã phát hành',

    CONSTRAINT FK_BienNhan_ThanhToan
        FOREIGN KEY (MaThanhToan)
        REFERENCES ThanhToan(MaThanhToan),

    CONSTRAINT FK_BienNhan_NhanVien
        FOREIGN KEY (MaNhanVienLap)
        REFERENCES NhanVien(MaNhanVien),

    CONSTRAINT CK_BienNhan_SoTien
        CHECK (SoTienXacNhan > 0)
);
GO


/* ============================================================
   27. DỊCH VỤ
   ============================================================ */

CREATE TABLE DichVu
(
    MaDichVu INT IDENTITY(1,1) PRIMARY KEY,

    TenDichVu NVARCHAR(200) NOT NULL UNIQUE,

    MoTa NVARCHAR(1000) NULL,

    TrangThai NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_DichVu_TrangThai DEFAULT N'Hoạt động'
);
GO


/* ============================================================
   28. PHẢN HỒI DỊCH VỤ
   ============================================================ */

CREATE TABLE PhanHoiDichVu
(
    MaPhanHoiDichVu INT IDENTITY(1,1) PRIMARY KEY,

    MaDichVu INT NOT NULL,
    MaHocVien VARCHAR(20) NOT NULL,

    DiemDanhGia INT NOT NULL,

    NoiDungPhanHoi NVARCHAR(2000) NULL,

    ThoiGianPhanHoi DATETIME2
        NOT NULL
        CONSTRAINT DF_PhanHoiDichVu_ThoiGian DEFAULT SYSDATETIME(),

    TrangThai NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_PhanHoiDichVu_TrangThai DEFAULT N'Đã gửi',

    CONSTRAINT FK_PhanHoiDichVu_DichVu
        FOREIGN KEY (MaDichVu)
        REFERENCES DichVu(MaDichVu),

    CONSTRAINT FK_PhanHoiDichVu_HocVien
        FOREIGN KEY (MaHocVien)
        REFERENCES HocVien(MaHocVien),

    CONSTRAINT CK_PhanHoiDichVu_Diem
        CHECK (DiemDanhGia BETWEEN 1 AND 5)
);
GO


/* ============================================================
   29. KHẢO SÁT
   ============================================================ */

CREATE TABLE KhaoSat
(
    MaKhaoSat INT IDENTITY(1,1) PRIMARY KEY,

    TenKhaoSat NVARCHAR(255) NOT NULL,

    MoTa NVARCHAR(1000) NULL,

    NgayBatDau DATE NOT NULL,
    NgayKetThuc DATE NULL,

    TrangThai NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_KhaoSat_TrangThai DEFAULT N'Hoạt động',

    CONSTRAINT CK_KhaoSat_Ngay
        CHECK
        (
            NgayKetThuc IS NULL
            OR NgayKetThuc >= NgayBatDau
        )
);
GO


/* ============================================================
   30. CÂU HỎI KHẢO SÁT
   ============================================================ */

CREATE TABLE CauHoiKhaoSat
(
    MaCauHoi INT IDENTITY(1,1) PRIMARY KEY,

    MaKhaoSat INT NOT NULL,

    NoiDungCauHoi NVARCHAR(1000) NOT NULL,

    LoaiCauHoi NVARCHAR(100) NOT NULL,

    ThuTu INT NOT NULL
        CONSTRAINT DF_CauHoiKhaoSat_ThuTu DEFAULT 1,

    BatBuoc BIT NOT NULL
        CONSTRAINT DF_CauHoiKhaoSat_BatBuoc DEFAULT 0,

    TrangThai NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_CauHoiKhaoSat_TrangThai DEFAULT N'Hoạt động',

    CONSTRAINT FK_CauHoi_KhaoSat
        FOREIGN KEY (MaKhaoSat)
        REFERENCES KhaoSat(MaKhaoSat)
        ON DELETE CASCADE,

    CONSTRAINT UQ_CauHoiKhaoSat_ThuTu
        UNIQUE (MaKhaoSat, ThuTu),

    CONSTRAINT CK_CauHoi_ThuTu
        CHECK (ThuTu > 0)
);
GO


/* ============================================================
   31. PHẢN HỒI KHẢO SÁT
   ============================================================ */

CREATE TABLE PhanHoiKhaoSat
(
    MaPhanHoiKhaoSat INT IDENTITY(1,1) PRIMARY KEY,

    MaKhaoSat INT NOT NULL,
    MaHocVien VARCHAR(20) NOT NULL,

    ThoiGianGui DATETIME2
        NOT NULL
        CONSTRAINT DF_PhanHoiKhaoSat_ThoiGian DEFAULT SYSDATETIME(),

    DiemDanhGiaChung DECIMAL(5,2) NULL,

    GhiChu NVARCHAR(1000) NULL,

    CONSTRAINT FK_PhanHoiKhaoSat_KhaoSat
        FOREIGN KEY (MaKhaoSat)
        REFERENCES KhaoSat(MaKhaoSat),

    CONSTRAINT FK_PhanHoiKhaoSat_HocVien
        FOREIGN KEY (MaHocVien)
        REFERENCES HocVien(MaHocVien),

    CONSTRAINT UQ_PhanHoiKhaoSat
        UNIQUE (MaKhaoSat, MaHocVien),

    CONSTRAINT CK_PhanHoiKhaoSat_Diem
        CHECK
        (
            DiemDanhGiaChung IS NULL
            OR DiemDanhGiaChung BETWEEN 0 AND 5
        )
);
GO


/* ============================================================
   32. CÂU TRẢ LỜI KHẢO SÁT
   ============================================================ */

CREATE TABLE CauTraLoiKhaoSat
(
    MaCauTraLoi INT IDENTITY(1,1) PRIMARY KEY,

    MaPhanHoiKhaoSat INT NOT NULL,
    MaCauHoi INT NOT NULL,

    GiaTriDanhGia DECIMAL(5,2) NULL,

    NoiDungTraLoi NVARCHAR(2000) NULL,

    CONSTRAINT FK_CauTraLoi_PhanHoi
        FOREIGN KEY (MaPhanHoiKhaoSat)
        REFERENCES PhanHoiKhaoSat(MaPhanHoiKhaoSat)
        ON DELETE CASCADE,

    CONSTRAINT FK_CauTraLoi_CauHoi
        FOREIGN KEY (MaCauHoi)
        REFERENCES CauHoiKhaoSat(MaCauHoi),

    CONSTRAINT UQ_CauTraLoi
        UNIQUE (MaPhanHoiKhaoSat, MaCauHoi),

    CONSTRAINT CK_CauTraLoi_GiaTriDanhGia
        CHECK
        (
            GiaTriDanhGia IS NULL
            OR GiaTriDanhGia BETWEEN 0 AND 5
        )
);
GO


/* ============================================================
   33. THÔNG BÁO
   Đây là bảng thứ 32 vì đã xóa HocVien_PhuHuynh.
   ============================================================ */

CREATE TABLE ThongBao
(
    MaThongBao INT IDENTITY(1,1) PRIMARY KEY,

    MaTaiKhoanGui INT NOT NULL,

    -- Gửi trực tiếp cho một tài khoản.
    MaTaiKhoanNhan INT NULL,

    -- Hoặc thông báo cho cả lớp.
    MaLopHoc INT NULL,

    TieuDe NVARCHAR(255) NOT NULL,
    NoiDung NVARCHAR(MAX) NOT NULL,

    LoaiThongBao NVARCHAR(100) NULL,

    ThoiGianGui DATETIME2
        NOT NULL
        CONSTRAINT DF_ThongBao_ThoiGian DEFAULT SYSDATETIME(),

    TrangThai NVARCHAR(50)
        NOT NULL
        CONSTRAINT DF_ThongBao_TrangThai DEFAULT N'Đã gửi',

    CONSTRAINT FK_ThongBao_TaiKhoanGui
        FOREIGN KEY (MaTaiKhoanGui)
        REFERENCES TaiKhoan(MaTaiKhoan),

    CONSTRAINT FK_ThongBao_TaiKhoanNhan
        FOREIGN KEY (MaTaiKhoanNhan)
        REFERENCES TaiKhoan(MaTaiKhoan),

    CONSTRAINT FK_ThongBao_LopHoc
        FOREIGN KEY (MaLopHoc)
        REFERENCES LopHoc(MaLopHoc),

    CONSTRAINT CK_ThongBao_DoiTuongNhan
        CHECK
        (
            MaTaiKhoanNhan IS NOT NULL
            OR MaLopHoc IS NOT NULL
        )
);
GO


/* ============================================================
   34. INDEX HỖ TRỢ TRUY VẤN
   ============================================================ */

CREATE INDEX IX_TaiKhoan_MaVaiTro
ON TaiKhoan(MaVaiTro);
GO

CREATE INDEX IX_HocVien_MaPhuHuynh
ON HocVien(MaPhuHuynh);
GO

CREATE INDEX IX_BangCap_MaGiaoVien
ON BangCap(MaGiaoVien);
GO

CREATE INDEX IX_ChungChi_MaGiaoVien
ON ChungChi(MaGiaoVien);
GO

CREATE INDEX IX_CapDo_MaChuongTrinh
ON CapDo(MaChuongTrinh);
GO

CREATE INDEX IX_KhoaHoc_MaCapDo
ON KhoaHoc(MaCapDo);
GO

CREATE INDEX IX_KiemTraDauVao_HocVien
ON KiemTraDauVao(MaHocVien);
GO

CREATE INDEX IX_LopHoc_KhoaHoc
ON LopHoc(MaKhoaHoc);
GO

CREATE INDEX IX_PhanCongGiangDay_LopHoc
ON PhanCongGiangDay(MaLopHoc);
GO

CREATE INDEX IX_PhanCongGiangDay_GiaoVien
ON PhanCongGiangDay(MaGiaoVien);
GO

CREATE INDEX IX_DangKyHoc_HocVien
ON DangKyHoc(MaHocVien);
GO

CREATE INDEX IX_DangKyHoc_KhoaHoc
ON DangKyHoc(MaKhoaHoc);
GO

CREATE INDEX IX_DangKyHoc_LopHoc
ON DangKyHoc(MaLopHoc);
GO

CREATE INDEX IX_LichHoc_LopHoc
ON LichHoc(MaLopHoc);
GO

CREATE INDEX IX_LichHoc_PhongHoc
ON LichHoc(MaPhongHoc);
GO

CREATE INDEX IX_CaHoc_LopHoc_NgayHoc
ON CaHoc(MaLopHoc, NgayHoc);
GO

CREATE INDEX IX_DiemDanh_HocVien
ON DiemDanh(MaHocVien);
GO

CREATE INDEX IX_DiemSo_DangKy
ON DiemSo(MaDangKy);
GO

CREATE INDEX IX_ThanhToan_HocPhi
ON ThanhToan(MaHocPhi);
GO

CREATE INDEX IX_PhanHoiDichVu_HocVien
ON PhanHoiDichVu(MaHocVien);
GO

CREATE INDEX IX_PhanHoiKhaoSat_HocVien
ON PhanHoiKhaoSat(MaHocVien);
GO

CREATE INDEX IX_ThongBao_TaiKhoanNhan
ON ThongBao(MaTaiKhoanNhan);
GO

CREATE INDEX IX_ThongBao_LopHoc
ON ThongBao(MaLopHoc);
GO


/* ============================================================
   35. DỮ LIỆU VAI TRÒ BAN ĐẦU
   ============================================================ */

INSERT INTO VaiTro
(
    TenVaiTro,
    MoTa
)
VALUES
(N'Quản trị viên', N'Quản trị và cấu hình hệ thống'),
(N'Nhân viên học vụ', N'Quản lý đào tạo, lớp học và lịch học'),
(N'Nhân viên tư vấn', N'Tiếp nhận và tư vấn học viên'),
(N'Nhân viên kế toán', N'Quản lý học phí và thanh toán'),
(N'Giáo viên', N'Thực hiện giảng dạy và đánh giá học viên'),
(N'Trợ giảng', N'Hỗ trợ giáo viên và quản lý lớp'),
(N'Học viên', N'Người tham gia học tại trung tâm');
GO


/* ============================================================
   36. KIỂM TRA SỐ LƯỢNG BẢNG
   Kết quả mong đợi: 32
   ============================================================ */

SELECT COUNT(*) AS SoLuongBang
FROM sys.tables;
GO

PRINT N'Đã tạo lại database QuanLyTrungTamNgoaiNgu với 32 bảng.';
GO
