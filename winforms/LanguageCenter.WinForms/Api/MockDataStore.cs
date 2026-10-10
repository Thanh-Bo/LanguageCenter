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
    }

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
