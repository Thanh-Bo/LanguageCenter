using System.Text.Json;

namespace LanguageCenter.WinForms.Api;

/// <summary>
/// Kho dữ liệu giả lập (Mock Data Store) chạy trực tiếp trong bộ nhớ.
/// Cho phép nhóm thiết kế, chạy thử và thuyết trình toàn bộ giao diện mà KHÔNG cần bật Backend API hay SQL Server.
/// </summary>
public static class MockDataStore
{
    private static readonly List<AccountListItemDto> _accounts = new();
    private static readonly List<StudentDetailDto> _students = new();
    private static readonly List<TeacherDetailDto> _teachers = new();

    private static readonly List<ChuongTrinhDto> _programs = new();
    private static readonly List<CapDoDto> _levels = new();
    private static readonly List<KhoaHocDto> _courses = new();
    private static readonly List<LopHocDto> _classes = new();
    private static readonly List<PhanCongGiangDayDto> _assignments = new();
    private static readonly List<DangKyHocDto> _enrollments = new();
    private static readonly List<PhongHocDto> _rooms = new();
    private static readonly List<LichHocDto> _schedules = new();
    private static readonly List<CaHocDto> _shifts = new();
    private static readonly List<LoaiDiemDto> _gradeTypes = new();
    private static readonly List<DiemSoHocVienDto> _grades = new();
    private static readonly List<HocPhiDto> _tuitions = new();
    private static readonly List<ThanhToanDto> _payments = new();
    private static readonly List<BienNhanDto> _receipts = new();
    private static readonly List<ThongBaoDto> _notifications = new();
    private static readonly List<BaoCaoDoanhThuDto> _revenueReports = new();

    static MockDataStore()
    {
        InitSampleData();
    }

    private static void InitSampleData()
    {
        // 1. Dữ liệu mẫu Tài khoản
        _accounts.AddRange(new[]
        {
            new AccountListItemDto { Id = 1, Username = "ADMIN001", FullName = "Đỗ Minh Khoa", Email = "admin01@trungtam.edu.vn", Role = RoleNames.QuanTriVien, Status = AccountStatuses.HoatDong, CreatedAt = DateTime.Now.AddMonths(-10), LastLoginAt = DateTime.Now.AddHours(-1) },
            new AccountListItemDto { Id = 2, Username = "ADMIN002", FullName = "Nguyễn Hoàng Long", Email = "admin02@trungtam.edu.vn", Role = RoleNames.QuanTriVien, Status = AccountStatuses.HoatDong, CreatedAt = DateTime.Now.AddMonths(-8), LastLoginAt = DateTime.Now.AddDays(-2) },
            new AccountListItemDto { Id = 3, Username = "NVHV001", FullName = "Nguyễn Thị Lan", Email = "lan.hocvu@trungtam.edu.vn", Role = RoleNames.NhanVienHocVu, Status = AccountStatuses.HoatDong, CreatedAt = DateTime.Now.AddMonths(-6), LastLoginAt = DateTime.Now.AddHours(-3) },
            new AccountListItemDto { Id = 4, Username = "NVTV001", FullName = "Trần Minh Hoàng", Email = "hoang.tuvan@trungtam.edu.vn", Role = RoleNames.NhanVienTuVan, Status = AccountStatuses.HoatDong, CreatedAt = DateTime.Now.AddMonths(-5), LastLoginAt = DateTime.Now.AddDays(-1) },
            new AccountListItemDto { Id = 5, Username = "NVKT001", FullName = "Lê Ngọc Mai", Email = "mai.ketoan@trungtam.edu.vn", Role = RoleNames.NhanVienKeToan, Status = AccountStatuses.HoatDong, CreatedAt = DateTime.Now.AddMonths(-5), LastLoginAt = DateTime.Now.AddDays(-1) },
            new AccountListItemDto { Id = 6, Username = "GV001", FullName = "ThS. Phạm Minh Tuấn", Email = "tuan.ielts@trungtam.edu.vn", Role = RoleNames.GiaoVien, Status = AccountStatuses.HoatDong, CreatedAt = DateTime.Now.AddMonths(-7), LastLoginAt = DateTime.Now.AddDays(-1) },
            new AccountListItemDto { Id = 7, Username = "GV002", FullName = "Sarah Jenkins", Email = "sarah.native@trungtam.edu.vn", Role = RoleNames.GiaoVien, Status = AccountStatuses.HoatDong, CreatedAt = DateTime.Now.AddMonths(-4), LastLoginAt = DateTime.Now.AddDays(-3) },
            new AccountListItemDto { Id = 8, Username = "TG001", FullName = "Nguyễn Văn An", Email = "an.ta@trungtam.edu.vn", Role = RoleNames.TroGiang, Status = AccountStatuses.HoatDong, CreatedAt = DateTime.Now.AddMonths(-3), LastLoginAt = DateTime.Now.AddDays(-2) },
            new AccountListItemDto { Id = 9, Username = "HV001", FullName = "Trần Bảo Ngọc", Email = "baongoc2004@gmail.com", Role = RoleNames.HocVien, Status = AccountStatuses.HoatDong, CreatedAt = DateTime.Now.AddMonths(-2), LastLoginAt = DateTime.Now.AddDays(-1) },
            new AccountListItemDto { Id = 10, Username = "HV002", FullName = "Lê Hoàng Nam", Email = "hoangnam.it@gmail.com", Role = RoleNames.HocVien, Status = AccountStatuses.HoatDong, CreatedAt = DateTime.Now.AddMonths(-2), LastLoginAt = DateTime.Now.AddDays(-4) },
            new AccountListItemDto { Id = 11, Username = "HV003", FullName = "Vũ Thuỳ Linh", Email = "thuylinh.student@gmail.com", Role = RoleNames.HocVien, Status = AccountStatuses.BiKhoa, CreatedAt = DateTime.Now.AddMonths(-1), LastLoginAt = null }
        });

        // 2. Dữ liệu mẫu Học viên
        _students.AddRange(new[]
        {
            new StudentDetailDto
            {
                MaHocVien = "HV001",
                HoTen = "Trần Bảo Ngọc",
                NgaySinh = new DateOnly(2004, 5, 14),
                GioiTinh = "Nữ",
                Email = "baongoc2004@gmail.com",
                SoDienThoai = "0912345678",
                DiaChi = "128 Nguyễn Trãi, Quận 5, TP.HCM",
                MucTieuHocTap = "IELTS 7.0 du học Úc",
                TrinhDoHienTai = "B1 (Intermediate)",
                TrangThaiHocTap = StudentStatuses.DangHoc,
                NgayTaoHoSo = DateTime.Now.AddMonths(-3),
                NgayNhapHoc = new DateOnly(2026, 8, 15),
                PhuHuynh = new ParentDto { HoTen = "Trần Văn Đức", SoDienThoai = "0988776655", MoiQuanHe = "Bố" }
            },
            new StudentDetailDto
            {
                MaHocVien = "HV002",
                HoTen = "Lê Hoàng Nam",
                NgaySinh = new DateOnly(2002, 11, 20),
                GioiTinh = "Nam",
                Email = "hoangnam.it@gmail.com",
                SoDienThoai = "0987654321",
                DiaChi = "45 Lê Văn Sỹ, Quận 3, TP.HCM",
                MucTieuHocTap = "Giao tiếp công việc IT & phỏng vấn xin việc",
                TrinhDoHienTai = "A2 (Pre-Intermediate)",
                TrangThaiHocTap = StudentStatuses.DangHoc,
                NgayTaoHoSo = DateTime.Now.AddMonths(-2),
                NgayNhapHoc = new DateOnly(2026, 9, 1)
            },
            new StudentDetailDto
            {
                MaHocVien = "HV003",
                HoTen = "Vũ Thuỳ Linh",
                NgaySinh = new DateOnly(2006, 3, 8),
                GioiTinh = "Nữ",
                Email = "thuylinh.student@gmail.com",
                SoDienThoai = "0909112233",
                DiaChi = "78 Hai Bà Trưng, Quận 1, TP.HCM",
                MucTieuHocTap = "TOEIC 750+ tốt nghiệp đại học",
                TrinhDoHienTai = "B1",
                TrangThaiHocTap = StudentStatuses.BaoLuu,
                NgayTaoHoSo = DateTime.Now.AddMonths(-2),
                NgayNhapHoc = new DateOnly(2026, 8, 20)
            },
            new StudentDetailDto
            {
                MaHocVien = "HV004",
                HoTen = "Nguyễn Minh Khang",
                NgaySinh = new DateOnly(2014, 9, 2),
                GioiTinh = "Nam",
                Email = "phuhuynh.khang@gmail.com",
                SoDienThoai = "0933445566",
                DiaChi = "12 Nguyễn Văn Cừ, Quận 5, TP.HCM",
                MucTieuHocTap = "Chứng chỉ Cambridge Flyers",
                TrinhDoHienTai = "Movers",
                TrangThaiHocTap = StudentStatuses.DangHoc,
                NgayTaoHoSo = DateTime.Now.AddMonths(-1),
                NgayNhapHoc = new DateOnly(2026, 9, 10),
                PhuHuynh = new ParentDto { HoTen = "Nguyễn Hữu Trọng", SoDienThoai = "0933445566", MoiQuanHe = "Bố" }
            },
            new StudentDetailDto
            {
                MaHocVien = "HV005",
                HoTen = "Phạm Quỳnh Anh",
                NgaySinh = new DateOnly(2005, 12, 18),
                GioiTinh = "Nữ",
                Email = "quynhanh.pham@gmail.com",
                SoDienThoai = "0977889900",
                DiaChi = "34 Điện Biên Phủ, Bình Thạnh, TP.HCM",
                MucTieuHocTap = "IELTS 6.5 xét tuyển Đại học",
                TrinhDoHienTai = "A2",
                TrangThaiHocTap = StudentStatuses.DangHoc,
                NgayTaoHoSo = DateTime.Now.AddDays(-20),
                NgayNhapHoc = new DateOnly(2026, 10, 1)
            }
        });

        // 3. Dữ liệu mẫu Giáo viên & Trợ giảng
        _teachers.AddRange(new[]
        {
            new TeacherDetailDto
            {
                MaGiaoVien = "GV001",
                HoTen = "ThS. Phạm Minh Tuấn",
                NgaySinh = new DateOnly(1989, 4, 12),
                GioiTinh = "Nam",
                Email = "tuan.ielts@trungtam.edu.vn",
                SoDienThoai = "0903123456",
                DiaChi = "Quận 10, TP.HCM",
                LoaiGiaoVien = TeacherTypes.GiaoVien,
                ChuyenMon = "IELTS Academic, Writing & Speaking Master",
                SoNamKinhNghiem = 9,
                NgayVaoLam = new DateOnly(2020, 3, 1),
                TrangThai = WorkStatuses.DangLamViec,
                BangCaps = new[] { new BangCapDto { TenBangCap = "Thạc sĩ Giảng dạy Ngôn ngữ Anh (TESOL)", CoSoDaoTao = "ĐH Victoria (Úc)", NamTotNghiep = 2016 } },
                ChungChis = new[] { new ChungChiDto { TenChungChi = "IELTS Overall 8.5 (Speaking 9.0)", DonViCap = "IDP Education", KetQua = "8.5" } }
            },
            new TeacherDetailDto
            {
                MaGiaoVien = "GV002",
                HoTen = "Sarah Jenkins",
                NgaySinh = new DateOnly(1993, 8, 25),
                GioiTinh = "Nữ",
                Email = "sarah.native@trungtam.edu.vn",
                SoDienThoai = "0908889999",
                DiaChi = "Quận 2 (TP. Thủ Đức), TP.HCM",
                LoaiGiaoVien = TeacherTypes.GiaoVien,
                ChuyenMon = "Giao tiếp phản xạ & Phát âm chuẩn Anh - Mỹ",
                SoNamKinhNghiem = 6,
                NgayVaoLam = new DateOnly(2022, 6, 15),
                TrangThai = WorkStatuses.DangLamViec,
                BangCaps = new[] { new BangCapDto { TenBangCap = "Bachelor of English Literature", CoSoDaoTao = "University of Leeds (UK)", NamTotNghiep = 2015 } },
                ChungChis = new[] { new ChungChiDto { TenChungChi = "CELTA Certificate", DonViCap = "Cambridge English", KetQua = "Pass A" } }
            },
            new TeacherDetailDto
            {
                MaGiaoVien = "GV003",
                HoTen = "ThS. Đặng Thu Thảo",
                NgaySinh = new DateOnly(1991, 11, 5),
                GioiTinh = "Nữ",
                Email = "thao.toeic@trungtam.edu.vn",
                SoDienThoai = "0918776655",
                DiaChi = "Quận Bình Thạnh, TP.HCM",
                LoaiGiaoVien = TeacherTypes.GiaoVien,
                ChuyenMon = "Luyện thi TOEIC Chuyên sâu & Ngữ pháp ứng dụng",
                SoNamKinhNghiem = 7,
                NgayVaoLam = new DateOnly(2021, 9, 1),
                TrangThai = WorkStatuses.DangLamViec,
                BangCaps = new[] { new BangCapDto { TenBangCap = "Thạc sĩ Lý luận và PPDH Tiếng Anh", CoSoDaoTao = "ĐH Sư Phạm TP.HCM", NamTotNghiep = 2018 } },
                ChungChis = new[] { new ChungChiDto { TenChungChi = "TOEIC 990/990 (Max Score)", DonViCap = "IIG Việt Nam", KetQua = "990" } }
            },
            new TeacherDetailDto
            {
                MaGiaoVien = "TG001",
                HoTen = "Nguyễn Văn An",
                NgaySinh = new DateOnly(2003, 1, 10),
                GioiTinh = "Nam",
                Email = "an.ta@trungtam.edu.vn",
                SoDienThoai = "0944556677",
                DiaChi = "Quận Phú Nhuận, TP.HCM",
                LoaiGiaoVien = TeacherTypes.TroGiang,
                ChuyenMon = "Hỗ trợ học vụ lớp IELTS Foundation & Kèm 1-1",
                SoNamKinhNghiem = 2,
                NgayVaoLam = new DateOnly(2024, 8, 1),
                TrangThai = WorkStatuses.DangLamViec,
                BangCaps = new[] { new BangCapDto { TenBangCap = "Sinh viên Xuất sắc Ngôn ngữ Anh", CoSoDaoTao = "ĐH Khoa Học Xã Hội & Nhân Văn", NamTotNghiep = 2025 } },
                ChungChis = new[] { new ChungChiDto { TenChungChi = "IELTS Overall 7.5", DonViCap = "British Council", KetQua = "7.5" } }
            },
            new TeacherDetailDto
            {
                MaGiaoVien = "TG002",
                HoTen = "Trần Thị Mai Phương",
                NgaySinh = new DateOnly(2002, 6, 18),
                GioiTinh = "Nữ",
                Email = "phuong.ta@trungtam.edu.vn",
                SoDienThoai = "0933221144",
                DiaChi = "Quận 3, TP.HCM",
                LoaiGiaoVien = TeacherTypes.TroGiang,
                ChuyenMon = "Mentor Speaking & Luyện phát âm IPA cho người mới bắt đầu",
                SoNamKinhNghiem = 2,
                NgayVaoLam = new DateOnly(2024, 2, 15),
                TrangThai = WorkStatuses.DangLamViec,
                BangCaps = new[] { new BangCapDto { TenBangCap = "Cử nhân Biên - Phiên dịch Tiếng Anh", CoSoDaoTao = "ĐH Ngoại Thương CS2", NamTotNghiep = 2024 } },
                ChungChis = new[] { new ChungChiDto { TenChungChi = "IELTS Overall 8.0 (Speaking 8.5)", DonViCap = "IDP Education", KetQua = "8.0" } }
            },
            new TeacherDetailDto
            {
                MaGiaoVien = "TG003",
                HoTen = "Lê Hoàng Phúc",
                NgaySinh = new DateOnly(2004, 3, 22),
                GioiTinh = "Nam",
                Email = "phuc.ta@trungtam.edu.vn",
                SoDienThoai = "0977665544",
                DiaChi = "Quận Tân Bình, TP.HCM",
                LoaiGiaoVien = TeacherTypes.TroGiang,
                ChuyenMon = "Chấm bài Writing & Quản lý nhóm học viên ôn thi TOEIC",
                SoNamKinhNghiem = 1,
                NgayVaoLam = new DateOnly(2025, 1, 10),
                TrangThai = WorkStatuses.DangLamViec,
                BangCaps = new[] { new BangCapDto { TenBangCap = "Sinh viên Ngành Sư phạm Tiếng Anh", CoSoDaoTao = "ĐH Sài Gòn", NamTotNghiep = 2026 } },
                ChungChis = new[] { new ChungChiDto { TenChungChi = "TOEIC 925/990", DonViCap = "IIG Việt Nam", KetQua = "925" } }
            }
        });

        // 4. Chương trình đào tạo (Programs)
        _programs.AddRange(new[]
        {
            new ChuongTrinhDto { MaChuongTrinh = 1, TenChuongTrinh = "IELTS Academic", NhomDoiTuong = "Học sinh, sinh viên, người đi làm định cư du học", DoTuoiTu = 15, DoTuoiDen = 45, MucTieu = "Đạt chứng chỉ IELTS 6.5 - 8.0+", MoTa = "Chương trình luyện thi IELTS chuẩn quốc tế Cambridge.", TrangThai = "Đang mở" },
            new ChuongTrinhDto { MaChuongTrinh = 2, TenChuongTrinh = "Tiếng Anh Giao tiếp", NhomDoiTuong = "Người đi làm, sinh viên cần phản xạ giao tiếp", DoTuoiTu = 18, DoTuoiDen = 50, MucTieu = "Phản xạ giao tiếp tự nhiên chuẩn bản xứ", MoTa = "Phương pháp học phản xạ tương tác 100% tiếng Anh.", TrangThai = "Đang mở" },
            new ChuongTrinhDto { MaChuongTrinh = 3, TenChuongTrinh = "Luyện thi TOEIC", NhomDoiTuong = "Sinh viên chuẩn bị tốt nghiệp, người xin việc", DoTuoiTu = 18, DoTuoiDen = 40, MucTieu = "Cam kết đầu ra TOEIC 650 - 850+", MoTa = "Chiến thuật làm bài nghe hiểu và đọc hiểu tốc độ cao.", TrangThai = "Đang mở" },
            new ChuongTrinhDto { MaChuongTrinh = 4, TenChuongTrinh = "Tiếng Anh Thiếu nhi", NhomDoiTuong = "Trẻ em độ tuổi tiểu học và trung học", DoTuoiTu = 6, DoTuoiDen = 14, MucTieu = "Chứng chỉ Cambridge Starters, Movers, Flyers", MoTa = "Phát triển tư duy ngôn ngữ sớm qua hình ảnh sinh động.", TrangThai = "Đang mở" }
        });

        // 5. Cấp độ đào tạo (Levels)
        _levels.AddRange(new[]
        {
            new CapDoDto { MaCapDo = 1, MaChuongTrinh = 1, TenChuongTrinh = "IELTS Academic", KyHieuCapDo = "IELTS-INT", TenCapDo = "IELTS Intermediate (Band 6.0 - 6.5)", ThuTu = 1, DieuKienDauVao = "Đầu vào 5.0 - 5.5", KetQuaDauRa = "Cam kết tối thiểu 6.5+", ThoiLuongDuKienTuan = 12, TrangThai = "Đang mở" },
            new CapDoDto { MaCapDo = 2, MaChuongTrinh = 1, TenChuongTrinh = "IELTS Academic", KyHieuCapDo = "IELTS-MAS", TenCapDo = "IELTS Master (Band 7.0 - 8.0+)", ThuTu = 2, DieuKienDauVao = "Đầu vào 6.5+", KetQuaDauRa = "Đạt mục tiêu 7.5 - 8.0+", ThoiLuongDuKienTuan = 14, TrangThai = "Đang mở" },
            new CapDoDto { MaCapDo = 3, MaChuongTrinh = 2, TenChuongTrinh = "Tiếng Anh Giao tiếp", KyHieuCapDo = "COM-BAS", TenCapDo = "Giao tiếp căn bản (A1 - A2)", ThuTu = 1, DieuKienDauVao = "Mất gốc hoặc căn bản", KetQuaDauRa = "Giao tiếp đời sống hàng ngày", ThoiLuongDuKienTuan = 8, TrangThai = "Đang mở" },
            new CapDoDto { MaCapDo = 4, MaChuongTrinh = 2, TenChuongTrinh = "Tiếng Anh Giao tiếp", KyHieuCapDo = "COM-PRO", TenCapDo = "Giao tiếp phản xạ nâng cao (B1 - B2)", ThuTu = 2, DieuKienDauVao = "Đạt trình độ A2+", KetQuaDauRa = "Thuyết trình và đàm phán thương mại", ThoiLuongDuKienTuan = 10, TrangThai = "Đang mở" },
            new CapDoDto { MaCapDo = 5, MaChuongTrinh = 3, TenChuongTrinh = "Luyện thi TOEIC", KyHieuCapDo = "TOEIC-650", TenCapDo = "Luyện thi TOEIC Mục tiêu 650+", ThuTu = 1, DieuKienDauVao = "Đầu vào 400+", KetQuaDauRa = "Đạt chuẩn 650+", ThoiLuongDuKienTuan = 8, TrangThai = "Đang mở" },
            new CapDoDto { MaCapDo = 6, MaChuongTrinh = 4, TenChuongTrinh = "Tiếng Anh Thiếu nhi", KyHieuCapDo = "KIDS-STA", TenCapDo = "Cambridge Starters & Movers", ThuTu = 1, DieuKienDauVao = "Mới bắt đầu", KetQuaDauRa = "12 - 15 khiên Cambridge", ThoiLuongDuKienTuan = 16, TrangThai = "Đang mở" }
        });

        // 6. Khóa học (Courses)
        _courses.AddRange(new[]
        {
            new KhoaHocDto { MaKhoaHoc = 1, MaKhoaHocStr = "KH-IELTS-65", MaCapDo = 1, TenCapDo = "IELTS Intermediate (Band 6.0 - 6.5)", TenChuongTrinh = "IELTS Academic", TenKhoaHoc = "IELTS Intensive 6.5 Cấp tốc", NoiDung = "Rèn luyện 4 kỹ năng nghe nói đọc viết với giảng viên bản xứ & Việt Nam", SoBuoi = 30, TongSoGio = 60, HocPhiCoBan = 9800000m, NgayBatDau = new DateOnly(2026, 10, 20), NgayKetThuc = new DateOnly(2027, 1, 10), TrangThai = "Đang tuyển sinh" },
            new KhoaHocDto { MaKhoaHoc = 2, MaKhoaHocStr = "KH-IELTS-75", MaCapDo = 2, TenCapDo = "IELTS Master (Band 7.0 - 8.0+)", TenChuongTrinh = "IELTS Academic", TenKhoaHoc = "IELTS Master 7.5+ Chuyên sâu", NoiDung = "Chiến thuật xử lý Writing Task 2 nâng cao và Speaking Part 3", SoBuoi = 36, TongSoGio = 72, HocPhiCoBan = 12500000m, NgayBatDau = new DateOnly(2026, 10, 15), NgayKetThuc = new DateOnly(2027, 1, 15), TrangThai = "Đang tuyển sinh" },
            new KhoaHocDto { MaKhoaHoc = 3, MaKhoaHocStr = "KH-COM-BAS", MaCapDo = 3, TenCapDo = "Giao tiếp căn bản (A1 - A2)", TenChuongTrinh = "Tiếng Anh Giao tiếp", TenKhoaHoc = "Tiếng Anh Giao tiếp Căn bản (Beginner)", NoiDung = "Học phát âm chuẩn IPA, ngữ pháp giao tiếp cơ bản", SoBuoi = 20, TongSoGio = 40, HocPhiCoBan = 4500000m, NgayBatDau = new DateOnly(2026, 11, 1), NgayKetThuc = new DateOnly(2026, 12, 20), TrangThai = "Sắp khai giảng" },
            new KhoaHocDto { MaKhoaHoc = 4, MaKhoaHocStr = "KH-COM-PRO", MaCapDo = 4, TenCapDo = "Giao tiếp phản xạ nâng cao (B1 - B2)", TenChuongTrinh = "Tiếng Anh Giao tiếp", TenKhoaHoc = "Tiếng Anh Giao tiếp phản xạ PRO", NoiDung = "Thuyết trình, tranh luận, phỏng vấn tuyển dụng", SoBuoi = 24, TongSoGio = 48, HocPhiCoBan = 6800000m, NgayBatDau = new DateOnly(2026, 10, 1), NgayKetThuc = new DateOnly(2026, 11, 25), TrangThai = "Đang diễn ra" },
            new KhoaHocDto { MaKhoaHoc = 5, MaKhoaHocStr = "KH-TOEIC-65", MaCapDo = 5, TenCapDo = "Luyện thi TOEIC Mục tiêu 650+", TenChuongTrinh = "Luyện thi TOEIC", TenKhoaHoc = "Luyện thi TOEIC Mục tiêu 650+", NoiDung = "Mẹo làm bài thi Listening Part 2, 3 và Reading Part 5, 7", SoBuoi = 24, TongSoGio = 48, HocPhiCoBan = 5500000m, NgayBatDau = new DateOnly(2026, 10, 18), NgayKetThuc = new DateOnly(2026, 12, 15), TrangThai = "Đang tuyển sinh" },
            new KhoaHocDto { MaKhoaHoc = 6, MaKhoaHocStr = "KH-KIDS-STA", MaCapDo = 6, TenCapDo = "Cambridge Starters & Movers", TenChuongTrinh = "Tiếng Anh Thiếu nhi", TenKhoaHoc = "Tiếng Anh Thiếu nhi Starters", NoiDung = "Học từ vựng và phản xạ qua trò chơi, bài hát thiếu nhi", SoBuoi = 32, TongSoGio = 64, HocPhiCoBan = 7200000m, NgayBatDau = new DateOnly(2026, 10, 12), NgayKetThuc = new DateOnly(2027, 2, 12), TrangThai = "Đang tuyển sinh" }
        });

        // 7. Lớp học (Classes)
        _classes.AddRange(new[]
        {
            new LopHocDto { MaLopHoc = 1, MaLop = "IELTS-65-K01", TenLop = "IELTS Intensive 6.5 - K01", MaKhoaHoc = 1, TenKhoaHoc = "IELTS Intensive 6.5 Cấp tốc", MaNhanVienHocVu = "NVHV001", TenNhanVienHocVu = "Nguyễn Thị Lan", TenGiaoVienChinh = "ThS. Phạm Minh Tuấn", TenTroGiang = "Nguyễn Văn An", TenPhongHoc = "Phòng P101", LichHoc = "T2-4-6 (18:00 - 19:30)", NgayBatDau = new DateOnly(2026, 10, 20), NgayKetThucDuKien = new DateOnly(2027, 1, 10), SiSoToiDa = 20, SiSoHienTai = 18, TrangThai = "Đang học" },
            new LopHocDto { MaLopHoc = 2, MaLop = "IELTS-75-K01", TenLop = "IELTS Master 7.5 - K01", MaKhoaHoc = 2, TenKhoaHoc = "IELTS Master 7.5+ Chuyên sâu", MaNhanVienHocVu = "NVHV001", TenNhanVienHocVu = "Nguyễn Thị Lan", TenGiaoVienChinh = "Sarah Jenkins", TenTroGiang = "Đặng Mai Phương", TenPhongHoc = "Phòng LAB01", LichHoc = "T3-5-7 (19:30 - 21:00)", NgayBatDau = new DateOnly(2026, 10, 15), NgayKetThucDuKien = new DateOnly(2027, 1, 15), SiSoToiDa = 15, SiSoHienTai = 14, TrangThai = "Đang học" },
            new LopHocDto { MaLopHoc = 3, MaLop = "COM-PRO-K02", TenLop = "Giao tiếp phản xạ PRO - K02", MaKhoaHoc = 4, TenKhoaHoc = "Tiếng Anh Giao tiếp phản xạ PRO", MaNhanVienHocVu = "NVHV001", TenNhanVienHocVu = "Nguyễn Thị Lan", TenGiaoVienChinh = "ThS. Phạm Minh Tuấn", TenTroGiang = "Lê Hoàng Phúc", TenPhongHoc = "Phòng P102", LichHoc = "T2-4-6 (19:30 - 21:00)", NgayBatDau = new DateOnly(2026, 10, 1), NgayKetThucDuKien = new DateOnly(2026, 11, 25), SiSoToiDa = 15, SiSoHienTai = 15, TrangThai = "Đang học" },
            new LopHocDto { MaLopHoc = 4, MaLop = "COM-BAS-K01", TenLop = "Giao tiếp căn bản - K01", MaKhoaHoc = 3, TenKhoaHoc = "Tiếng Anh Giao tiếp Căn bản (Beginner)", MaNhanVienHocVu = "NVHV001", TenNhanVienHocVu = "Nguyễn Thị Lan", TenGiaoVienChinh = "Sarah Jenkins", TenTroGiang = "Nguyễn Văn An", TenPhongHoc = "Phòng P103", LichHoc = "T3-5-7 (18:00 - 19:30)", NgayBatDau = new DateOnly(2026, 11, 1), NgayKetThucDuKien = new DateOnly(2026, 12, 20), SiSoToiDa = 15, SiSoHienTai = 10, TrangThai = "Sắp khai giảng" },
            new LopHocDto { MaLopHoc = 5, MaLop = "TOEIC-65-K01", TenLop = "Luyện thi TOEIC 650 - K01", MaKhoaHoc = 5, TenKhoaHoc = "Luyện thi TOEIC Mục tiêu 650+", MaNhanVienHocVu = "NVHV001", TenNhanVienHocVu = "Nguyễn Thị Lan", TenGiaoVienChinh = "ThS. Phạm Minh Tuấn", TenTroGiang = "Lê Hoàng Phúc", TenPhongHoc = "Phòng LAB02", LichHoc = "T7-CN (08:30 - 10:30)", NgayBatDau = new DateOnly(2026, 10, 18), NgayKetThucDuKien = new DateOnly(2026, 12, 15), SiSoToiDa = 20, SiSoHienTai = 16, TrangThai = "Đang học" },
            new LopHocDto { MaLopHoc = 6, MaLop = "KIDS-STA-K01", TenLop = "Thiếu nhi Starters - K01", MaKhoaHoc = 6, TenKhoaHoc = "Tiếng Anh Thiếu nhi Starters", MaNhanVienHocVu = "NVHV001", TenNhanVienHocVu = "Nguyễn Thị Lan", TenGiaoVienChinh = "Sarah Jenkins", TenTroGiang = "Đặng Mai Phương", TenPhongHoc = "Phòng P104", LichHoc = "T7-CN (15:00 - 16:30)", NgayBatDau = new DateOnly(2026, 10, 12), NgayKetThucDuKien = new DateOnly(2027, 2, 12), SiSoToiDa = 16, SiSoHienTai = 12, TrangThai = "Đang học" }
        });

        // 8. Phân công giảng dạy (Teaching Assignments)
        _assignments.AddRange(new[]
        {
            new PhanCongGiangDayDto { MaPhanCong = 1, MaLopHoc = 1, MaLop = "IELTS-65-K01", TenLop = "IELTS Intensive 6.5 - K01", MaGiaoVien = "GV001", TenGiaoVien = "ThS. Phạm Minh Tuấn", LoaiNhanSu = "Giáo viên", VaiTroPhanCong = "Giáo viên chính", NgayBatDau = new DateOnly(2026, 10, 20), TrangThai = "Đang giảng dạy", GhiChu = "Phụ trách giảng dạy chính & luyện đề thi" },
            new PhanCongGiangDayDto { MaPhanCong = 2, MaLopHoc = 1, MaLop = "IELTS-65-K01", TenLop = "IELTS Intensive 6.5 - K01", MaGiaoVien = "TG001", TenGiaoVien = "Nguyễn Văn An", LoaiNhanSu = "Trợ giảng", VaiTroPhanCong = "Trợ giảng", NgayBatDau = new DateOnly(2026, 10, 20), TrangThai = "Đang giảng dạy", GhiChu = "Chấm bài Writing và luyện phát âm Speaking" },
            new PhanCongGiangDayDto { MaPhanCong = 3, MaLopHoc = 2, MaLop = "IELTS-75-K01", TenLop = "IELTS Master 7.5 - K01", MaGiaoVien = "GV002", TenGiaoVien = "Sarah Jenkins", LoaiNhanSu = "Giáo viên", VaiTroPhanCong = "Giáo viên chính", NgayBatDau = new DateOnly(2026, 10, 15), TrangThai = "Đang giảng dạy", GhiChu = "Chuyên gia bản ngữ luyện Speaking & Writing" },
            new PhanCongGiangDayDto { MaPhanCong = 4, MaLopHoc = 2, MaLop = "IELTS-75-K01", TenLop = "IELTS Master 7.5 - K01", MaGiaoVien = "TG002", TenGiaoVien = "Đặng Mai Phương", LoaiNhanSu = "Trợ giảng", VaiTroPhanCong = "Trợ giảng", NgayBatDau = new DateOnly(2026, 10, 15), TrangThai = "Đang giảng dạy", GhiChu = "Hỗ trợ học thuật và giải đáp ngữ pháp chuyên sâu" },
            new PhanCongGiangDayDto { MaPhanCong = 5, MaLopHoc = 3, MaLop = "COM-PRO-K02", TenLop = "Giao tiếp phản xạ PRO - K02", MaGiaoVien = "GV001", TenGiaoVien = "ThS. Phạm Minh Tuấn", LoaiNhanSu = "Giáo viên", VaiTroPhanCong = "Giáo viên chính", NgayBatDau = new DateOnly(2026, 10, 1), TrangThai = "Đang giảng dạy", GhiChu = "Hướng dẫn thuyết trình & tranh biện thương mại" },
            new PhanCongGiangDayDto { MaPhanCong = 6, MaLopHoc = 3, MaLop = "COM-PRO-K02", TenLop = "Giao tiếp phản xạ PRO - K02", MaGiaoVien = "TG003", TenGiaoVien = "Lê Hoàng Phúc", LoaiNhanSu = "Trợ giảng", VaiTroPhanCong = "Trợ giảng", NgayBatDau = new DateOnly(2026, 10, 1), TrangThai = "Đang giảng dạy", GhiChu = "Tổ chức hoạt động phản xạ nhóm" }
        });

        // 9. Đăng ký & Xếp học viên vào lớp (Enrollments)
        _enrollments.AddRange(new[]
        {
            new DangKyHocDto { MaDangKy = 1, MaHocVien = "HV001", TenHocVien = "Trần Bảo Ngọc", SoDienThoai = "0912345678", Email = "baongoc2004@gmail.com", MaKhoaHoc = 1, TenKhoaHoc = "IELTS Intensive 6.5 Cấp tốc", MaLopHoc = 1, MaLop = "IELTS-65-K01", TenLop = "IELTS Intensive 6.5 - K01", NgayDangKy = DateTime.Now.AddMonths(-2), NgayXepLop = DateTime.Now.AddMonths(-1), HocPhiGoc = 9800000m, SoTienGiam = 500000m, HocPhiPhaiDong = 9300000m, SoTienDaDong = 9300000m, TrangThaiHocPhi = "Đã hoàn thành", TrangThaiDangKy = "Đã xếp lớp" },
            new DangKyHocDto { MaDangKy = 2, MaHocVien = "HV002", TenHocVien = "Lê Hoàng Nam", SoDienThoai = "0987654321", Email = "hoangnam.it@gmail.com", MaKhoaHoc = 4, TenKhoaHoc = "Tiếng Anh Giao tiếp phản xạ PRO", MaLopHoc = 3, MaLop = "COM-PRO-K02", TenLop = "Giao tiếp phản xạ PRO - K02", NgayDangKy = DateTime.Now.AddMonths(-2), NgayXepLop = DateTime.Now.AddMonths(-1), HocPhiGoc = 6800000m, SoTienGiam = 0m, HocPhiPhaiDong = 6800000m, SoTienDaDong = 6800000m, TrangThaiHocPhi = "Đã hoàn thành", TrangThaiDangKy = "Đã xếp lớp" },
            new DangKyHocDto { MaDangKy = 3, MaHocVien = "HV003", TenHocVien = "Vũ Thuỳ Linh", SoDienThoai = "0909112233", Email = "thuylinh.student@gmail.com", MaKhoaHoc = 5, TenKhoaHoc = "Luyện thi TOEIC Mục tiêu 650+", MaLopHoc = 5, MaLop = "TOEIC-65-K01", TenLop = "Luyện thi TOEIC 650 - K01", NgayDangKy = DateTime.Now.AddMonths(-1), NgayXepLop = DateTime.Now.AddDays(-20), HocPhiGoc = 5500000m, SoTienGiam = 500000m, HocPhiPhaiDong = 5000000m, SoTienDaDong = 3000000m, TrangThaiHocPhi = "Đóng một phần", TrangThaiDangKy = "Đã xếp lớp" },
            new DangKyHocDto { MaDangKy = 4, MaHocVien = "HV004", TenHocVien = "Nguyễn Minh Khang", SoDienThoai = "0933445566", Email = "phuhuynh.khang@gmail.com", MaKhoaHoc = 6, TenKhoaHoc = "Tiếng Anh Thiếu nhi Starters", MaLopHoc = 6, MaLop = "KIDS-STA-K01", TenLop = "Thiếu nhi Starters - K01", NgayDangKy = DateTime.Now.AddMonths(-1), NgayXepLop = DateTime.Now.AddDays(-15), HocPhiGoc = 7200000m, SoTienGiam = 1000000m, HocPhiPhaiDong = 6200000m, SoTienDaDong = 6200000m, TrangThaiHocPhi = "Đã hoàn thành", TrangThaiDangKy = "Đã xếp lớp" },
            new DangKyHocDto { MaDangKy = 5, MaHocVien = "HV005", TenHocVien = "Hoàng Thị Ánh Tuyết", SoDienThoai = "0944556677", Email = "anhtuyet.law@gmail.com", MaKhoaHoc = 2, TenKhoaHoc = "IELTS Master 7.5+ Chuyên sâu", MaLopHoc = 2, MaLop = "IELTS-75-K01", TenLop = "IELTS Master 7.5 - K01", NgayDangKy = DateTime.Now.AddDays(-10), NgayXepLop = DateTime.Now.AddDays(-5), HocPhiGoc = 12500000m, SoTienGiam = 1500000m, HocPhiPhaiDong = 11000000m, SoTienDaDong = 11000000m, TrangThaiHocPhi = "Đã hoàn thành", TrangThaiDangKy = "Đã xếp lớp" },
            new DangKyHocDto { MaDangKy = 6, MaHocVien = "HV006", TenHocVien = "Đặng Quốc Hưng", SoDienThoai = "0966778899", Email = "hung.dang@gmail.com", MaKhoaHoc = 1, TenKhoaHoc = "IELTS Intensive 6.5 Cấp tốc", MaLopHoc = null, MaLop = null, TenLop = null, NgayDangKy = DateTime.Now.AddDays(-3), NgayXepLop = null, HocPhiGoc = 9800000m, SoTienGiam = 0m, HocPhiPhaiDong = 9800000m, SoTienDaDong = 9800000m, TrangThaiHocPhi = "Đã hoàn thành", TrangThaiDangKy = "Chờ xếp lớp", GhiChu = "Học viên ưu tiên ca học buổi tối các ngày chẵn" },
            new DangKyHocDto { MaDangKy = 7, MaHocVien = "HV007", TenHocVien = "Phạm Quỳnh Như", SoDienThoai = "0977889900", Email = "quynhnhu.edu@gmail.com", MaKhoaHoc = 3, TenKhoaHoc = "Tiếng Anh Giao tiếp Căn bản (Beginner)", MaLopHoc = null, MaLop = null, TenLop = null, NgayDangKy = DateTime.Now.AddDays(-2), NgayXepLop = null, HocPhiGoc = 4500000m, SoTienGiam = 500000m, HocPhiPhaiDong = 4000000m, SoTienDaDong = 2000000m, TrangThaiHocPhi = "Đóng một phần", TrangThaiDangKy = "Chờ xếp lớp", GhiChu = "Chờ khai giảng lớp tháng 11" }
        });

        // 10. Phòng học (Rooms)
        _rooms.AddRange(new[]
        {
            new PhongHocDto { MaPhongHoc = 1, MaPhong = "P101", TenPhong = "Phòng Lý thuyết A1", ViTri = "Tầng 1 - Khu A", SucChua = 20, ThietBi = "Máy chiếu Sony, Loa trợ giảng, Điều hòa 2 chiều", TrangThai = "Đang sử dụng" },
            new PhongHocDto { MaPhongHoc = 2, MaPhong = "P102", TenPhong = "Phòng Giao tiếp A2", ViTri = "Tầng 1 - Khu A", SucChua = 20, ThietBi = "Smart TV 75 inch, Hệ thống loa vòm, Điều hòa", TrangThai = "Đang sử dụng" },
            new PhongHocDto { MaPhongHoc = 3, MaPhong = "P103", TenPhong = "Phòng Đa năng A3", ViTri = "Tầng 2 - Khu A", SucChua = 25, ThietBi = "Máy chiếu EPSON, Bảng từ trượt, Điều hòa", TrangThai = "Sẵn sàng" },
            new PhongHocDto { MaPhongHoc = 4, MaPhong = "P104", TenPhong = "Phòng Thiếu nhi Kids Care", ViTri = "Tầng 1 - Khu C", SucChua = 16, ThietBi = "Bàn ghế thiếu nhi chuẩn quốc tế, TV thông minh, Đồ chơi học thuật", TrangThai = "Đang sử dụng" },
            new PhongHocDto { MaPhongHoc = 5, MaPhong = "LAB01", TenPhong = "Phòng Lab Nghe nhìn 1", ViTri = "Tầng 2 - Khu B", SucChua = 25, ThietBi = "25 Bộ máy tính Dell Core i5, Tai nghe chống ồn Sony, Máy chiếu", TrangThai = "Đang sử dụng" },
            new PhongHocDto { MaPhongHoc = 6, MaPhong = "LAB02", TenPhong = "Phòng Lab Thi TOEIC/IELTS", ViTri = "Tầng 2 - Khu B", SucChua = 25, ThietBi = "25 Bộ máy tính chuẩn khảo thí quốc tế, Hệ thống camera giám sát", TrangThai = "Đang sử dụng" },
            new PhongHocDto { MaPhongHoc = 7, MaPhong = "AUD01", TenPhong = "Hội trường Hội thảo Quốc tế", ViTri = "Tầng 3 - Tòa nhà Trung tâm", SucChua = 80, ThietBi = "Sân khấu, Màn hình LED 200 inch, Hệ thống âm thanh hội trường", TrangThai = "Sẵn sàng" }
        });

        // 11. Lịch học tuần (Weekly Schedules)
        _schedules.AddRange(new[]
        {
            new LichHocDto { MaLichHoc = 1, MaLopHoc = 1, MaLop = "IELTS-65-K01", TenLop = "IELTS Intensive 6.5 - K01", MaPhongHoc = 1, TenPhong = "P101", ThuTrongTuan = 2, ThuText = "Thứ Hai - Thứ Tư - Thứ Sáu", GioBatDau = new TimeOnly(18, 0), GioKetThuc = new TimeOnly(19, 30), NgayApDungTu = new DateOnly(2026, 10, 20), TrangThai = "Đang áp dụng" },
            new LichHocDto { MaLichHoc = 2, MaLopHoc = 2, MaLop = "IELTS-75-K01", TenLop = "IELTS Master 7.5 - K01", MaPhongHoc = 5, TenPhong = "LAB01", ThuTrongTuan = 3, ThuText = "Thứ Ba - Thứ Năm - Thứ Bảy", GioBatDau = new TimeOnly(19, 30), GioKetThuc = new TimeOnly(21, 0), NgayApDungTu = new DateOnly(2026, 10, 15), TrangThai = "Đang áp dụng" },
            new LichHocDto { MaLichHoc = 3, MaLopHoc = 3, MaLop = "COM-PRO-K02", TenLop = "Giao tiếp phản xạ PRO - K02", MaPhongHoc = 2, TenPhong = "P102", ThuTrongTuan = 2, ThuText = "Thứ Hai - Thứ Tư - Thứ Sáu", GioBatDau = new TimeOnly(19, 30), GioKetThuc = new TimeOnly(21, 0), NgayApDungTu = new DateOnly(2026, 10, 1), TrangThai = "Đang áp dụng" },
            new LichHocDto { MaLichHoc = 4, MaLopHoc = 4, MaLop = "COM-BAS-K01", TenLop = "Giao tiếp căn bản - K01", MaPhongHoc = 3, TenPhong = "P103", ThuTrongTuan = 3, ThuText = "Thứ Ba - Thứ Năm - Thứ Bảy", GioBatDau = new TimeOnly(18, 0), GioKetThuc = new TimeOnly(19, 30), NgayApDungTu = new DateOnly(2026, 11, 1), TrangThai = "Đang áp dụng" },
            new LichHocDto { MaLichHoc = 5, MaLopHoc = 5, MaLop = "TOEIC-65-K01", TenLop = "Luyện thi TOEIC 650 - K01", MaPhongHoc = 6, TenPhong = "LAB02", ThuTrongTuan = 7, ThuText = "Thứ Bảy - Chủ Nhật", GioBatDau = new TimeOnly(8, 30), GioKetThuc = new TimeOnly(10, 30), NgayApDungTu = new DateOnly(2026, 10, 18), TrangThai = "Đang áp dụng" },
            new LichHocDto { MaLichHoc = 6, MaLopHoc = 6, MaLop = "KIDS-STA-K01", TenLop = "Thiếu nhi Starters - K01", MaPhongHoc = 4, TenPhong = "P104", ThuTrongTuan = 7, ThuText = "Thứ Bảy - Chủ Nhật", GioBatDau = new TimeOnly(15, 0), GioKetThuc = new TimeOnly(16, 30), NgayApDungTu = new DateOnly(2026, 10, 12), TrangThai = "Đang áp dụng" }
        });

        // 12. Ca học thực tế (Shifts)
        _shifts.AddRange(new[]
        {
            new CaHocDto { MaCaHoc = 1, MaLopHoc = 1, MaLop = "IELTS-65-K01", TenLop = "IELTS Intensive 6.5 - K01", MaPhongHoc = 1, TenPhong = "P101", TenGiaoVien = "ThS. Phạm Minh Tuấn", TenTroGiang = "Nguyễn Văn An", NgayHoc = DateOnly.FromDateTime(DateTime.Today), GioBatDau = new TimeOnly(18, 0), GioKetThuc = new TimeOnly(19, 30), NoiDungBaiHoc = "Unit 4: Listening Multiple Choice Strategy & Practice", TrangThai = "Chưa diễn ra" },
            new CaHocDto { MaCaHoc = 2, MaLopHoc = 3, MaLop = "COM-PRO-K02", TenLop = "Giao tiếp phản xạ PRO - K02", MaPhongHoc = 2, TenPhong = "P102", TenGiaoVien = "ThS. Phạm Minh Tuấn", TenTroGiang = "Lê Hoàng Phúc", NgayHoc = DateOnly.FromDateTime(DateTime.Today), GioBatDau = new TimeOnly(19, 30), GioKetThuc = new TimeOnly(21, 0), NoiDungBaiHoc = "Business English: Pitching Ideas and Handling Difficult Clients", TrangThai = "Chưa diễn ra" },
            new CaHocDto { MaCaHoc = 3, MaLopHoc = 2, MaLop = "IELTS-75-K01", TenLop = "IELTS Master 7.5 - K01", MaPhongHoc = 5, TenPhong = "LAB01", TenGiaoVien = "Sarah Jenkins", TenTroGiang = "Đặng Mai Phương", NgayHoc = DateOnly.FromDateTime(DateTime.Today.AddDays(-1)), GioBatDau = new TimeOnly(19, 30), GioKetThuc = new TimeOnly(21, 0), NoiDungBaiHoc = "Academic Writing Task 2: Advanced Cohesion and Lexical Resource", TrangThai = "Đã hoàn thành" },
            new CaHocDto { MaCaHoc = 4, MaLopHoc = 5, MaLop = "TOEIC-65-K01", TenLop = "Luyện thi TOEIC 650 - K01", MaPhongHoc = 6, TenPhong = "LAB02", TenGiaoVien = "ThS. Phạm Minh Tuấn", TenTroGiang = "Lê Hoàng Phúc", NgayHoc = DateOnly.FromDateTime(DateTime.Today.AddDays(1)), GioBatDau = new TimeOnly(8, 30), GioKetThuc = new TimeOnly(10, 30), NoiDungBaiHoc = "Part 5 & 6 Grammar Traps and Time Management Tactics", TrangThai = "Chưa diễn ra" }
        });

        // 13. Điểm số (Grades)
        _grades.AddRange(new[]
        {
            new DiemSoHocVienDto { MaDangKy = 1, MaHocVien = "HV001", TenHocVien = "Trần Bảo Ngọc", MaLop = "IELTS-65-K01", DiemChuyenCan = 9.5m, DiemGiuaKy = 7.0m, DiemSpeaking = 7.5m, DiemWriting = 6.5m, DiemCuoiKy = 7.5m, DiemTongKet = 7.3m, XepLoai = "Giỏi", NhanXet = "Tiến bộ vượt bậc về phản xạ Speaking, ngữ điệu tự nhiên." },
            new DiemSoHocVienDto { MaDangKy = 2, MaHocVien = "HV002", TenHocVien = "Lê Hoàng Nam", MaLop = "COM-PRO-K02", DiemChuyenCan = 10.0m, DiemGiuaKy = 8.0m, DiemSpeaking = 8.5m, DiemWriting = 7.5m, DiemCuoiKy = 8.0m, DiemTongKet = 8.2m, XepLoai = "Giỏi", NhanXet = "Kỹ năng thuyết trình tự tin, làm việc nhóm tích cực." },
            new DiemSoHocVienDto { MaDangKy = 3, MaHocVien = "HV003", TenHocVien = "Vũ Thuỳ Linh", MaLop = "TOEIC-65-K01", DiemChuyenCan = 8.0m, DiemGiuaKy = 6.5m, DiemSpeaking = 6.0m, DiemWriting = 6.0m, DiemCuoiKy = 6.5m, DiemTongKet = 6.5m, XepLoai = "Khá", NhanXet = "Cần tăng tốc độ giải bài đọc hiểu Part 7." },
            new DiemSoHocVienDto { MaDangKy = 4, MaHocVien = "HV004", TenHocVien = "Nguyễn Minh Khang", MaLop = "KIDS-STA-K01", DiemChuyenCan = 9.5m, DiemGiuaKy = 9.0m, DiemSpeaking = 9.5m, DiemWriting = 9.0m, DiemCuoiKy = 9.5m, DiemTongKet = 9.3m, XepLoai = "Xuất sắc", NhanXet = "Bé rất nhạy bén, nhớ từ vựng nhanh và phát âm chuẩn." },
            new DiemSoHocVienDto { MaDangKy = 5, MaHocVien = "HV005", TenHocVien = "Hoàng Thị Ánh Tuyết", MaLop = "IELTS-75-K01", DiemChuyenCan = 10.0m, DiemGiuaKy = 7.5m, DiemSpeaking = 8.0m, DiemWriting = 7.5m, DiemCuoiKy = 8.0m, DiemTongKet = 8.0m, XepLoai = "Xuất sắc", NhanXet = "Tư duy lập luận logic, vốn từ vựng học thuật C1 phong phú." }
        });

        // 14. Học phí (Tuition)
        _tuitions.AddRange(new[]
        {
            new HocPhiDto { MaHocPhi = 1, MaDangKy = 1, MaHocVien = "HV001", TenHocVien = "Trần Bảo Ngọc", TenKhoaHoc = "IELTS Intensive 6.5 Cấp tốc", MaLop = "IELTS-65-K01", SoTienGoc = 9800000m, SoTienGiam = 500000m, SoTienPhaiDong = 9300000m, SoTienDaDong = 9300000m, SoTienConThieu = 0m, HanThanhToan = new DateOnly(2026, 10, 15), TrangThaiHocPhi = "Đã hoàn thành", NgayTao = DateTime.Now.AddMonths(-2) },
            new HocPhiDto { MaHocPhi = 2, MaDangKy = 2, MaHocVien = "HV002", TenHocVien = "Lê Hoàng Nam", TenKhoaHoc = "Tiếng Anh Giao tiếp phản xạ PRO", MaLop = "COM-PRO-K02", SoTienGoc = 6800000m, SoTienGiam = 0m, SoTienPhaiDong = 6800000m, SoTienDaDong = 6800000m, SoTienConThieu = 0m, HanThanhToan = new DateOnly(2026, 10, 1), TrangThaiHocPhi = "Đã hoàn thành", NgayTao = DateTime.Now.AddMonths(-2) },
            new HocPhiDto { MaHocPhi = 3, MaDangKy = 3, MaHocVien = "HV003", TenHocVien = "Vũ Thuỳ Linh", TenKhoaHoc = "Luyện thi TOEIC Mục tiêu 650+", MaLop = "TOEIC-65-K01", SoTienGoc = 5500000m, SoTienGiam = 500000m, SoTienPhaiDong = 5000000m, SoTienDaDong = 3000000m, SoTienConThieu = 2000000m, HanThanhToan = new DateOnly(2026, 10, 25), TrangThaiHocPhi = "Đóng một phần", NgayTao = DateTime.Now.AddMonths(-1) },
            new HocPhiDto { MaHocPhi = 4, MaDangKy = 4, MaHocVien = "HV004", TenHocVien = "Nguyễn Minh Khang", TenKhoaHoc = "Tiếng Anh Thiếu nhi Starters", MaLop = "KIDS-STA-K01", SoTienGoc = 7200000m, SoTienGiam = 1000000m, SoTienPhaiDong = 6200000m, SoTienDaDong = 6200000m, SoTienConThieu = 0m, HanThanhToan = new DateOnly(2026, 10, 10), TrangThaiHocPhi = "Đã hoàn thành", NgayTao = DateTime.Now.AddMonths(-1) },
            new HocPhiDto { MaHocPhi = 5, MaDangKy = 5, MaHocVien = "HV005", TenHocVien = "Hoàng Thị Ánh Tuyết", TenKhoaHoc = "IELTS Master 7.5+ Chuyên sâu", MaLop = "IELTS-75-K01", SoTienGoc = 12500000m, SoTienGiam = 1500000m, SoTienPhaiDong = 11000000m, SoTienDaDong = 11000000m, SoTienConThieu = 0m, HanThanhToan = new DateOnly(2026, 10, 12), TrangThaiHocPhi = "Đã hoàn thành", NgayTao = DateTime.Now.AddDays(-10) }
        });

        // 15. Lịch sử thanh toán & Biên nhận (Payments & Receipts)
        _payments.AddRange(new[]
        {
            new ThanhToanDto { MaThanhToan = 1, MaHocPhi = 1, MaHocVien = "HV001", TenHocVien = "Trần Bảo Ngọc", MaNhanVienKeToan = "NVKT001", SoTien = 9300000m, NgayThanhToan = DateTime.Now.AddDays(-20), PhuongThucThanhToan = "Chuyển khoản ngân hàng", MaGiaoDich = "FT26102001", LoaiGiaoDich = "Thu học phí", TrangThai = "Thành công", GhiChu = "Đã thanh toán đủ toàn khóa" },
            new ThanhToanDto { MaThanhToan = 2, MaHocPhi = 2, MaHocVien = "HV002", TenHocVien = "Lê Hoàng Nam", MaNhanVienKeToan = "NVKT001", SoTien = 6800000m, NgayThanhToan = DateTime.Now.AddDays(-25), PhuongThucThanhToan = "Tiền mặt", MaGiaoDich = "CS26100201", LoaiGiaoDich = "Thu học phí", TrangThai = "Thành công", GhiChu = "Đóng tiền mặt trực tiếp tại quầy" },
            new ThanhToanDto { MaThanhToan = 3, MaHocPhi = 3, MaHocVien = "HV003", TenHocVien = "Vũ Thuỳ Linh", MaNhanVienKeToan = "NVKT001", SoTien = 3000000m, NgayThanhToan = DateTime.Now.AddDays(-15), PhuongThucThanhToan = "Cổng thanh toán VNPAY", MaGiaoDich = "VN26101503", LoaiGiaoDich = "Thu học phí (Đợt 1)", TrangThai = "Thành công", GhiChu = "Thanh toán đợt 1, còn lại 2.000.000đ" },
            new ThanhToanDto { MaThanhToan = 4, MaHocPhi = 4, MaHocVien = "HV004", TenHocVien = "Nguyễn Minh Khang", MaNhanVienKeToan = "NVKT001", SoTien = 6200000m, NgayThanhToan = DateTime.Now.AddDays(-18), PhuongThucThanhToan = "Chuyển khoản ngân hàng", MaGiaoDich = "FT26101804", LoaiGiaoDich = "Thu học phí", TrangThai = "Thành công", GhiChu = "Phụ huynh chuyển khoản học phí Starters" }
        });

        _receipts.AddRange(new[]
        {
            new BienNhanDto { MaBienNhan = 1, MaThanhToan = 1, SoBienNhan = "BN-2026-00101", TenHocVien = "Trần Bảo Ngọc", MaHocVien = "HV001", TenKhoaHoc = "IELTS Intensive 6.5 Cấp tốc", MaNhanVienLap = "NVKT001", NgayLap = DateTime.Now.AddDays(-20), SoTienXacNhan = 9300000m, NoiDung = "Thu học phí khóa học IELTS Intensive 6.5", PhuongThuc = "Chuyển khoản", TrangThai = "Hợp lệ" },
            new BienNhanDto { MaBienNhan = 2, MaThanhToan = 2, SoBienNhan = "BN-2026-00102", TenHocVien = "Lê Hoàng Nam", MaHocVien = "HV002", TenKhoaHoc = "Tiếng Anh Giao tiếp phản xạ PRO", MaNhanVienLap = "NVKT001", NgayLap = DateTime.Now.AddDays(-25), SoTienXacNhan = 6800000m, NoiDung = "Thu học phí khóa học Giao tiếp phản xạ PRO", PhuongThuc = "Tiền mặt", TrangThai = "Hợp lệ" },
            new BienNhanDto { MaBienNhan = 3, MaThanhToan = 3, SoBienNhan = "BN-2026-00103", TenHocVien = "Vũ Thuỳ Linh", MaHocVien = "HV003", TenKhoaHoc = "Luyện thi TOEIC Mục tiêu 650+", MaNhanVienLap = "NVKT001", NgayLap = DateTime.Now.AddDays(-15), SoTienXacNhan = 3000000m, NoiDung = "Thu học phí TOEIC 650+ (Đợt 1)", PhuongThuc = "VNPAY", TrangThai = "Hợp lệ" }
        });

        // 16. Thông báo (Notifications)
        _notifications.AddRange(new[]
        {
            new ThongBaoDto { MaThongBao = 1, TieuDe = "Thông báo Lịch nghỉ Lễ và Kế hoạch học bù", NoiDung = "Trung tâm thông báo toàn thể cán bộ, giáo viên và học viên lịch nghỉ lễ sắp tới. Các lớp học có lịch trùng ngày nghỉ sẽ được bố trí học bù vào cuối tuần.", LoaiThongBao = "Lịch nghỉ / Khai giảng", DoiTuongNhan = "Toàn trung tâm", NguoiGui = "Ban Giám Đốc", ThoiGianGui = DateTime.Now.AddDays(-1), TrangThai = "Đã gửi" },
            new ThongBaoDto { MaThongBao = 2, TieuDe = "Lịch thi Mock Test IELTS định kỳ tháng 10/2026", NoiDung = "Kỳ thi thử IELTS Mock Test 4 kỹ năng chuẩn khảo thí IDP sẽ diễn ra vào sáng Chủ Nhật tại Phòng LAB01 và LAB02. Đề nghị học viên có mặt đúng giờ.", LoaiThongBao = "Học vụ", DoiTuongNhan = "Tất cả học viên", NguoiGui = "Phòng Học Vụ", ThoiGianGui = DateTime.Now.AddDays(-3), TrangThai = "Đã gửi" },
            new ThongBaoDto { MaThongBao = 3, TieuDe = "Nhắc nhở hoàn thành học phí học kỳ mới", NoiDung = "Phòng Kế toán trân trọng nhắc các học viên còn công nợ học phí vui lòng hoàn tất thủ tục trước ngày 25 hàng tháng để đảm bảo quyền lợi tích lũy điểm chuyên cần.", LoaiThongBao = "Học phí", DoiTuongNhan = "Tất cả học viên", NguoiGui = "Phòng Kế Toán", ThoiGianGui = DateTime.Now.AddDays(-5), TrangThai = "Đã gửi" },
            new ThongBaoDto { MaThongBao = 4, TieuDe = "Họp chuyên môn Giáo viên và Trợ giảng Quý 4", NoiDung = "Kính mời toàn thể Thầy Cô giáo viên và các bạn trợ giảng tham dự buổi sinh hoạt chuyên môn đổi mới phương pháp giảng dạy tương tác ứng dụng AI.", LoaiThongBao = "Học vụ", DoiTuongNhan = "Tất cả giáo viên", NguoiGui = "Ban Đào Tạo", ThoiGianGui = DateTime.Now.AddDays(-7), TrangThai = "Đã gửi" }
        });

        // 17. Báo cáo doanh thu theo chương trình (Revenue Reports)
        _revenueReports.AddRange(new[]
        {
            new BaoCaoDoanhThuDto { ChuongTrinh = "IELTS Academic", SoLop = 6, SoHocVien = 95, TongHocPhi = 985000000m, DaThu = 945000000m, ConNo = 40000000m },
            new BaoCaoDoanhThuDto { ChuongTrinh = "Tiếng Anh Giao tiếp", SoLop = 5, SoHocVien = 68, TongHocPhi = 345000000m, DaThu = 330000000m, ConNo = 15000000m },
            new BaoCaoDoanhThuDto { ChuongTrinh = "Luyện thi TOEIC", SoLop = 4, SoHocVien = 52, TongHocPhi = 260000000m, DaThu = 248000000m, ConNo = 12000000m },
            new BaoCaoDoanhThuDto { ChuongTrinh = "Tiếng Anh Thiếu nhi", SoLop = 3, SoHocVien = 33, TongHocPhi = 220000000m, DaThu = 215000000m, ConNo = 5000000m }
        });
    }

    #region Public Mock Query & Mutation APIs

    public static List<ChuongTrinhDto> GetPrograms() => _programs.ToList();
    public static List<CapDoDto> GetLevels(int? maChuongTrinh = null) => maChuongTrinh.HasValue ? _levels.Where(x => x.MaChuongTrinh == maChuongTrinh.Value).ToList() : _levels.ToList();
    public static List<KhoaHocDto> GetCourses(int? maCapDo = null) => maCapDo.HasValue ? _courses.Where(x => x.MaCapDo == maCapDo.Value).ToList() : _courses.ToList();

    public static List<LopHocDto> GetClasses() => _classes.ToList();
    public static void AddOrUpdateClass(LopHocDto item)
    {
        var idx = _classes.FindIndex(c => c.MaLopHoc == item.MaLopHoc);
        if (idx >= 0) _classes[idx] = item;
        else
        {
            item.MaLopHoc = _classes.Count > 0 ? _classes.Max(x => x.MaLopHoc) + 1 : 1;
            _classes.Insert(0, item);
        }
    }
    public static void DeleteClass(int maLopHoc) => _classes.RemoveAll(x => x.MaLopHoc == maLopHoc);

    public static List<PhanCongGiangDayDto> GetAssignments() => _assignments.ToList();
    public static void AddOrUpdateAssignment(PhanCongGiangDayDto item)
    {
        var idx = _assignments.FindIndex(x => x.MaPhanCong == item.MaPhanCong);
        if (idx >= 0) _assignments[idx] = item;
        else
        {
            item.MaPhanCong = _assignments.Count > 0 ? _assignments.Max(x => x.MaPhanCong) + 1 : 1;
            _assignments.Insert(0, item);
        }
    }
    public static void DeleteAssignment(int maPhanCong) => _assignments.RemoveAll(x => x.MaPhanCong == maPhanCong);

    public static List<DangKyHocDto> GetEnrollments() => _enrollments.ToList();
    public static bool EnrollStudentToClass(int maDangKy, int maLopHoc)
    {
        var enroll = _enrollments.FirstOrDefault(e => e.MaDangKy == maDangKy);
        var cls = _classes.FirstOrDefault(c => c.MaLopHoc == maLopHoc);
        if (enroll is null || cls is null) return false;
        if (cls.SiSoHienTai >= cls.SiSoToiDa) return false;

        enroll.MaLopHoc = cls.MaLopHoc;
        enroll.MaLop = cls.MaLop;
        enroll.TenLop = cls.TenLop;
        enroll.NgayXepLop = DateTime.Now;
        enroll.TrangThaiDangKy = "Đã xếp lớp";
        cls.SiSoHienTai++;
        return true;
    }
    public static bool RemoveEnrollmentFromClass(int maDangKy)
    {
        var enroll = _enrollments.FirstOrDefault(e => e.MaDangKy == maDangKy);
        if (enroll is null || !enroll.MaLopHoc.HasValue) return false;

        var cls = _classes.FirstOrDefault(c => c.MaLopHoc == enroll.MaLopHoc.Value);
        if (cls is not null && cls.SiSoHienTai > 0) cls.SiSoHienTai--;

        enroll.MaLopHoc = null;
        enroll.MaLop = null;
        enroll.TenLop = null;
        enroll.NgayXepLop = null;
        enroll.TrangThaiDangKy = "Chờ xếp lớp";
        return true;
    }

    public static List<PhongHocDto> GetRooms() => _rooms.ToList();
    public static void AddOrUpdateRoom(PhongHocDto room)
    {
        var idx = _rooms.FindIndex(r => r.MaPhongHoc == room.MaPhongHoc);
        if (idx >= 0) _rooms[idx] = room;
        else
        {
            room.MaPhongHoc = _rooms.Count > 0 ? _rooms.Max(x => x.MaPhongHoc) + 1 : 1;
            _rooms.Insert(0, room);
        }
    }

    public static List<LichHocDto> GetSchedules() => _schedules.ToList();
    public static void AddOrUpdateSchedule(LichHocDto sch)
    {
        var idx = _schedules.FindIndex(s => s.MaLichHoc == sch.MaLichHoc);
        if (idx >= 0) _schedules[idx] = sch;
        else
        {
            sch.MaLichHoc = _schedules.Count > 0 ? _schedules.Max(x => x.MaLichHoc) + 1 : 1;
            _schedules.Insert(0, sch);
        }
    }

    public static List<CaHocDto> GetShifts() => _shifts.ToList();

    public static List<DiemSoHocVienDto> GetGrades(string? maLop = null)
    {
        if (string.IsNullOrWhiteSpace(maLop) || maLop == "— Tất cả —") return _grades.ToList();
        return _grades.Where(g => g.MaLop.Equals(maLop, StringComparison.OrdinalIgnoreCase)).ToList();
    }
    public static void SaveGrade(DiemSoHocVienDto grade)
    {
        var idx = _grades.FindIndex(g => g.MaDangKy == grade.MaDangKy);
        if (idx >= 0) _grades[idx] = grade;
        else _grades.Add(grade);
    }

    public static List<HocPhiDto> GetTuitionList() => _tuitions.ToList();
    public static List<ThanhToanDto> GetPayments() => _payments.ToList();
    public static List<BienNhanDto> GetReceipts() => _receipts.ToList();

    public static BienNhanDto RecordPayment(int maHocPhi, decimal soTien, string phuongThuc, string ghiChu)
    {
        var tuition = _tuitions.FirstOrDefault(t => t.MaHocPhi == maHocPhi);
        var newPayId = _payments.Count > 0 ? _payments.Max(p => p.MaThanhToan) + 1 : 1;
        var newRcId = _receipts.Count > 0 ? _receipts.Max(r => r.MaBienNhan) + 1 : 1;
        var soBienNhan = $"BN-2026-{newRcId:D5}";

        var payment = new ThanhToanDto
        {
            MaThanhToan = newPayId,
            MaHocPhi = maHocPhi,
            MaHocVien = tuition?.MaHocVien ?? "HV001",
            TenHocVien = tuition?.TenHocVien ?? "Học viên",
            MaNhanVienKeToan = "NVKT001",
            SoTien = soTien,
            NgayThanhToan = DateTime.Now,
            PhuongThucThanhToan = phuongThuc,
            MaGiaoDich = $"TX2026{newPayId:D4}",
            LoaiGiaoDich = "Thu học phí",
            TrangThai = "Thành công",
            GhiChu = ghiChu
        };
        _payments.Insert(0, payment);

        var receipt = new BienNhanDto
        {
            MaBienNhan = newRcId,
            MaThanhToan = newPayId,
            SoBienNhan = soBienNhan,
            TenHocVien = tuition?.TenHocVien ?? "Học viên",
            MaHocVien = tuition?.MaHocVien ?? "HV001",
            TenKhoaHoc = tuition?.TenKhoaHoc ?? "Khóa học",
            MaNhanVienLap = "NVKT001",
            NgayLap = DateTime.Now,
            SoTienXacNhan = soTien,
            NoiDung = $"Thu học phí - {tuition?.TenKhoaHoc} ({ghiChu})",
            PhuongThuc = phuongThuc,
            TrangThai = "Hợp lệ"
        };
        _receipts.Insert(0, receipt);

        if (tuition is not null)
        {
            tuition.SoTienDaDong += soTien;
            tuition.SoTienConThieu = Math.Max(0, tuition.SoTienPhaiDong - tuition.SoTienDaDong);
            tuition.TrangThaiHocPhi = tuition.SoTienConThieu == 0 ? "Đã hoàn thành" : "Đóng một phần";
        }

        return receipt;
    }

    public static List<ThongBaoDto> GetNotifications() => _notifications.ToList();
    public static void AddNotification(ThongBaoDto notif)
    {
        notif.MaThongBao = _notifications.Count > 0 ? _notifications.Max(n => n.MaThongBao) + 1 : 1;
        notif.ThoiGianGui = DateTime.Now;
        _notifications.Insert(0, notif);
    }

    public static List<BaoCaoDoanhThuDto> GetRevenueReports() => _revenueReports.ToList();

    #endregion

    public static ApiResult<T> HandleGet<T>(string path)
    {
        // 1. Dashboard stats: admin/accounts?page=1...
        if (path.StartsWith("admin/accounts"))
        {
            var isFilterActive = path.Contains("Hoạt động");
            var filtered = _accounts.AsEnumerable();
            if (isFilterActive) filtered = filtered.Where(a => a.Status == AccountStatuses.HoatDong);

            var list = filtered.ToList();
            var paged = new PagedResult<AccountListItemDto>
            {
                Items = list,
                Page = 1,
                PageSize = 20,
                TotalItems = list.Count,
                TotalPages = 1
            };
            return Ok((T)(object)paged);
        }

        // 2. Students
        if (path.StartsWith("admin/students"))
        {
            if (path.StartsWith("admin/students/") && !path.Contains('?'))
            {
                var id = path["admin/students/".Length..];
                var s = _students.FirstOrDefault(x => x.MaHocVien.Equals(id, StringComparison.OrdinalIgnoreCase));
                if (s is not null) return Ok((T)(object)s);
                return NotFound<T>($"Không tìm thấy học viên với mã {id}");
            }

            var list = _students.Select(s => new StudentListItemDto
            {
                MaHocVien = s.MaHocVien,
                HoTen = s.HoTen,
                NgaySinh = s.NgaySinh,
                GioiTinh = s.GioiTinh,
                Email = s.Email,
                SoDienThoai = s.SoDienThoai,
                TrangThaiHocTap = s.TrangThaiHocTap,
                NgayNhapHoc = s.NgayNhapHoc
            }).ToList();

            var paged = new PagedResult<StudentListItemDto>
            {
                Items = list,
                Page = 1,
                PageSize = 20,
                TotalItems = list.Count,
                TotalPages = 1
            };
            return Ok((T)(object)paged);
        }

        // 3. Teachers / Teaching Assistants
        if (path.StartsWith("admin/teachers"))
        {
            if (path.StartsWith("admin/teachers/") && !path.Contains('?'))
            {
                var id = path["admin/teachers/".Length..];
                var t = _teachers.FirstOrDefault(x => x.MaGiaoVien.Equals(id, StringComparison.OrdinalIgnoreCase));
                if (t is not null) return Ok((T)(object)t);
                return NotFound<T>($"Không tìm thấy giáo viên/trợ giảng với mã {id}");
            }

            var unescaped = Uri.UnescapeDataString(path);
            var isGiaoVienOnly = unescaped.Contains("loaiGiaoVien=Giáo viên");
            var isTroGiangOnly = unescaped.Contains("loaiGiaoVien=Trợ giảng");

            var filtered = _teachers.AsEnumerable();
            if (isGiaoVienOnly)
            {
                filtered = filtered.Where(t => t.LoaiGiaoVien == TeacherTypes.GiaoVien);
            }
            else if (isTroGiangOnly)
            {
                filtered = filtered.Where(t => t.LoaiGiaoVien == TeacherTypes.TroGiang);
            }

            var list = filtered.Select(t => new TeacherListItemDto
            {
                MaGiaoVien = t.MaGiaoVien,
                HoTen = t.HoTen,
                LoaiGiaoVien = t.LoaiGiaoVien,
                Email = t.Email,
                SoDienThoai = t.SoDienThoai,
                ChuyenMon = t.ChuyenMon,
                SoNamKinhNghiem = t.SoNamKinhNghiem,
                TrangThai = t.TrangThai
            }).ToList();

            var paged = new PagedResult<TeacherListItemDto>
            {
                Items = list,
                Page = 1,
                PageSize = 20,
                TotalItems = list.Count,
                TotalPages = 1
            };
            return Ok((T)(object)paged);
        }

        return NotFound<T>("Endpoint không tìm thấy");
    }

    public static ApiResult<TResp> HandlePost<TResp>(string path, object? body)
    {
        // Login
        if (path == "auth/login")
        {
            var req = body as LoginRequest;
            var username = string.IsNullOrWhiteSpace(req?.Username) ? "ADMIN001" : req.Username.Trim().ToUpper();

            string role = RoleNames.QuanTriVien;
            if (username.StartsWith("GV")) role = RoleNames.GiaoVien;
            else if (username.StartsWith("TG")) role = RoleNames.TroGiang;
            else if (username.StartsWith("HV")) role = RoleNames.HocVien;

            var resp = new LoginResponse
            {
                AccessToken = "mock-jwt-token-demo",
                TokenType = "Bearer",
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                User = new CurrentUserDto
                {
                    Id = 1,
                    Username = username,
                    Email = $"{username.ToLower()}@trungtam.edu.vn",
                    Role = role
                }
            };
            return Ok((TResp)(object)resp);
        }

        // Create student
        if (path == "admin/students")
        {
            var req = body as CreateStudentRequest;
            var newId = $"HV{_students.Count + 1:D3}";
            var newStudent = new StudentDetailDto
            {
                MaHocVien = newId,
                HoTen = req?.HoTen ?? "Học viên mới",
                NgaySinh = req?.NgaySinh ?? new DateOnly(2000, 1, 1),
                GioiTinh = req?.GioiTinh ?? "Nam",
                Email = req?.Email ?? $"{newId.ToLower()}@student.edu.vn",
                SoDienThoai = req?.SoDienThoai,
                DiaChi = req?.DiaChi,
                TrangThaiHocTap = StudentStatuses.DangHoc,
                NgayTaoHoSo = DateTime.Now,
                NgayNhapHoc = DateOnly.FromDateTime(DateTime.Now)
            };
            _students.Insert(0, newStudent);

            var resp = new CreateStudentResponse
            {
                StudentId = newId,
                Username = newId,
                TemporaryPassword = "Password@2026",
                FullName = newStudent.HoTen,
                Email = newStudent.Email,
                Role = RoleNames.HocVien,
                Message = "Tạo học viên thành công (Dữ liệu mẫu)"
            };
            return Ok((TResp)(object)resp);
        }

        // Create teacher
        if (path == "admin/teachers")
        {
            var req = body as CreateTeacherRequest;
            var newId = (req?.LoaiGiaoVien == TeacherTypes.TroGiang ? "TG" : "GV") + $"{_teachers.Count + 1:D3}";
            var newTeacher = new TeacherDetailDto
            {
                MaGiaoVien = newId,
                HoTen = req?.HoTen ?? "Giảng viên mới",
                Email = req?.Email ?? $"{newId.ToLower()}@trungtam.edu.vn",
                SoDienThoai = req?.SoDienThoai,
                LoaiGiaoVien = req?.LoaiGiaoVien ?? TeacherTypes.GiaoVien,
                ChuyenMon = req?.ChuyenMon,
                TrangThai = WorkStatuses.DangLamViec,
                NgayVaoLam = req?.NgayVaoLam ?? DateOnly.FromDateTime(DateTime.Now)
            };
            _teachers.Insert(0, newTeacher);

            var resp = new CreateTeacherResponse
            {
                TeacherId = newId,
                Username = newId,
                TemporaryPassword = "Teacher@2026",
                FullName = newTeacher.HoTen,
                Email = newTeacher.Email,
                LoaiGiaoVien = newTeacher.LoaiGiaoVien,
                Role = req?.LoaiGiaoVien == TeacherTypes.TroGiang ? RoleNames.TroGiang : RoleNames.GiaoVien,
                Message = "Tạo giáo viên/trợ giảng thành công (Dữ liệu mẫu)"
            };
            return Ok((TResp)(object)resp);
        }

        // Reset password
        if (path.Contains("reset-password"))
        {
            var resp = new ResetPasswordResponse
            {
                Username = "ADMIN001",
                TemporaryPassword = "TempPassword@2026",
                Message = "Cấp mật khẩu tạm thành công (Dữ liệu mẫu)"
            };
            return Ok((TResp)(object)resp);
        }

        return Ok(default(TResp)!);
    }

    public static ApiResult<TResp> HandlePut<TResp>(string path, object? body)
    {
        if (path.StartsWith("admin/teachers/"))
        {
            var id = path["admin/teachers/".Length..];
            var idx = _teachers.FindIndex(x => x.MaGiaoVien.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (idx >= 0 && body is UpdateTeacherRequest req)
            {
                var old = _teachers[idx];
                var updated = new TeacherDetailDto
                {
                    MaGiaoVien = old.MaGiaoVien,
                    HoTen = req.HoTen ?? old.HoTen,
                    NgaySinh = req.NgaySinh ?? old.NgaySinh,
                    GioiTinh = req.GioiTinh ?? old.GioiTinh,
                    Email = req.Email ?? old.Email,
                    SoDienThoai = req.SoDienThoai ?? old.SoDienThoai,
                    DiaChi = req.DiaChi ?? old.DiaChi,
                    LoaiGiaoVien = old.LoaiGiaoVien,
                    ChuyenMon = req.ChuyenMon ?? old.ChuyenMon,
                    SoNamKinhNghiem = req.SoNamKinhNghiem ?? old.SoNamKinhNghiem,
                    NgayVaoLam = req.NgayVaoLam ?? old.NgayVaoLam,
                    TrangThai = req.TrangThai ?? old.TrangThai,
                    BangCaps = old.BangCaps,
                    ChungChis = old.ChungChis
                };
                _teachers[idx] = updated;
                return Ok((TResp)(object)updated);
            }
        }

        if (path.StartsWith("admin/students/"))
        {
            var id = path["admin/students/".Length..];
            var idx = _students.FindIndex(x => x.MaHocVien.Equals(id, StringComparison.OrdinalIgnoreCase));
            if (idx >= 0 && body is UpdateStudentRequest req)
            {
                var old = _students[idx];
                var updated = new StudentDetailDto
                {
                    MaHocVien = old.MaHocVien,
                    HoTen = req.HoTen ?? old.HoTen,
                    NgaySinh = req.NgaySinh ?? old.NgaySinh,
                    GioiTinh = req.GioiTinh ?? old.GioiTinh,
                    Email = req.Email ?? old.Email,
                    SoDienThoai = req.SoDienThoai ?? old.SoDienThoai,
                    DiaChi = req.DiaChi ?? old.DiaChi,
                    MucTieuHocTap = req.MucTieuHocTap ?? old.MucTieuHocTap,
                    TrinhDoHienTai = req.TrinhDoHienTai ?? old.TrinhDoHienTai,
                    TrangThaiHocTap = req.TrangThaiHocTap ?? old.TrangThaiHocTap,
                    NgayTaoHoSo = old.NgayTaoHoSo,
                    NgayNhapHoc = req.NgayNhapHoc ?? old.NgayNhapHoc,
                    PhuHuynh = old.PhuHuynh
                };
                _students[idx] = updated;
                return Ok((TResp)(object)updated);
            }
        }

        return Ok(default(TResp)!);
    }

    public static ApiResult<object?> HandleDelete(string path)
    {
        if (path.StartsWith("admin/teachers/"))
        {
            var id = path["admin/teachers/".Length..];
            _teachers.RemoveAll(x => x.MaGiaoVien.Equals(id, StringComparison.OrdinalIgnoreCase));
            return Ok<object?>(null);
        }

        if (path.StartsWith("admin/students/"))
        {
            var id = path["admin/students/".Length..];
            _students.RemoveAll(x => x.MaHocVien.Equals(id, StringComparison.OrdinalIgnoreCase));
            return Ok<object?>(null);
        }

        return Ok<object?>(null);
    }

    private static ApiResult<T> Ok<T>(T data) => new()
    {
        Success = true,
        StatusCode = 200,
        Data = data
    };

    private static ApiResult<T> NotFound<T>(string error) => new()
    {
        Success = false,
        StatusCode = 404,
        ErrorMessage = error
    };
}
