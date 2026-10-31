# 🎓 LingoHub — Hệ thống quản lý Trung tâm Ngoại ngữ

> **Đồ án khóa luận tốt nghiệp** — Khoa Công nghệ Thông tin, Trường Đại học Công Thương TP.HCM (HUIT).
> Backend .NET 10 · Web React 19 · Desktop WinForms · SQL Server (Database-First, 32 bảng).

![Status](https://img.shields.io/badge/status-completed-brightgreen)
![License](https://img.shields.io/badge/license-academic-blue)
![Backend](https://img.shields.io/badge/backend-.NET%2010-512BD4)
![Frontend](https://img.shields.io/badge/frontend-React%2019%20%2B%20Vite%208-61DAFB)
![Desktop](https://img.shields.io/badge/desktop-WinForms-0C54A3)
![Database](https://img.shields.io/badge/database-SQL%20Server-CC2927)

**Liên kết nhanh:** [GitHub](https://github.com/Thanh-Bo/LanguageCenter) · [Cài đặt chi tiết](docs/SETUP.md) · [Danh sách API](docs/api-endpoints.md) · Swagger: `http://localhost:5204/swagger`

---

## 📖 Thông tin đề tài

| Mục | Nội dung |
|-----|----------|
| **Mã đề tài** | CNTT-KLCN013 |
| **Tên đề tài** | Xây dựng hệ thống quản lý Trung tâm Ngoại ngữ |
| **GVHD** | Lâm Thị Họa Mi |
| **Thành viên** | Lê Thị Thanh Huyền, Trần Ngọc Trúc Huỳnh, Nguyễn Văn Thành |
| **Thời gian** | 12 tuần (08/09/2025 → 30/11/2025) |
| **Mã nguồn** | https://github.com/Thanh-Bo/LanguageCenter |

---

## 🏗️ Kiến trúc hệ thống

Một backend ASP.NET Core dùng chung cho **ba** giao diện khách:

```
                    ┌──────────────────────────────────────────────┐
                    │        LanguageCenter.Api (.NET 10)          │
                    │  JWT · 7 vai trò · 33 controller · 155 API   │
                    └───────┬───────────────┬──────────────┬───────┘
                            │               │              │
                 ┌──────────▼───┐   ┌───────▼───────┐  ┌───▼──────────────┐
                 │ WinForms     │   │ Web /admin    │  │ Web /giaovien    │
                 │ (Quản trị)   │   │ (4 vai trò    │  │ Web /hocvien     │
                 │              │   │  quản trị)    │  │ (GV · TG · HV)   │
                 └──────────────┘   └───────────────┘  └──────────────────┘
```

| Lớp | Trách nhiệm | Ghi chú |
|---|---|---|
| `LanguageCenter.Domain` | Entity + hằng số nghiệp vụ (`RoleNames`, `ClassStatuses`, …) | Không phụ thuộc package |
| `LanguageCenter.Application` | Use case, DTO, validation, `OperationResult`, interface store | Không tham chiếu EF Core / ASP.NET Core |
| `LanguageCenter.Infrastructure` | EF Core 10 (Database-First), store, JWT, hash mật khẩu | Projection và phân trang chạy ở SQL |
| `LanguageCenter.Api` | Controller, middleware, Swagger, DI | Không chứa logic nghiệp vụ |

**Quy ước chính:** Database-First (không migration) · phân trang/tổng hợp chạy ở SQL Server (`Skip/Take`, `AsNoTracking`) · thao tác nhiều bước dùng transaction qua `CreateExecutionStrategy` (do bật `EnableRetryOnFailure`) · controller trả `OperationResult` → 200/201/204/400/404/409.

---

## 🛠️ Công nghệ sử dụng

### ⚙️ Backend

| Công nghệ | Phiên bản | Vai trò |
|---|---|---|
| ASP.NET Core Web API | .NET 10 | REST API |
| Entity Framework Core | 10 (SQL Server provider) | ORM, Database-First |
| JWT Bearer | 10 | Xác thực, phân quyền 7 vai trò |
| ASP.NET Identity Core | 10 | Hash / verify mật khẩu |
| Swashbuckle (OpenAPI) | 10 | Swagger UI tại `/swagger` |

### 🌐 Website (Admin · Giáo viên · Học viên)

| Công nghệ | Phiên bản | Vai trò |
|---|---|---|
| React | 19 | UI |
| TypeScript | 5.7 | Type safety (build thất bại nếu sai kiểu) |
| Vite | 8 | Dev server / build |
| Tailwind CSS | 4 | Styling (palette navy `#1D4ED8`) |
| React Router | 8 | Routing theo vai trò |

### 💻 Desktop & Cơ sở dữ liệu

| Công nghệ | Vai trò |
|---|---|
| WinForms (`net10.0-windows`) | Ứng dụng quản trị cho Quản trị viên |
| SQL Server | 32 bảng, ràng buộc CHECK/FK, cột computed |

---

## 📁 Cấu trúc thư mục

```
LanguageCenter/
├── backend/
│   ├── LanguageCenter.Api/             # Controller, Program.cs, Swagger, middleware
│   ├── LanguageCenter.Application/     # Use case + DTO (26 module nghiệp vụ)
│   ├── LanguageCenter.Domain/          # Entity + hằng số
│   └── LanguageCenter.Infrastructure/  # EF Core store, JWT, hash mật khẩu
│
├── database/
│   ├── QuanLyTrungTamNgoaiNgu_v2.sql          # Tạo schema (32 bảng)
│   ├── DuLieuMau_QuanLyTrungTamNgoaiNgu_v2.sql # Dữ liệu mẫu
│   ├── BoSung_..._v3 … v13.sql                 # Các bản vá tuần tự
│   ├── DongBo_GioHoc_v12.sql
│   ├── DongBo_NgayNhapHoc_v14.sql
│   └── KhoiPhuc_QuanTriVien.sql                # Cứu tài khoản admin
│
├── web/HUITLINGO/                      # Website React
│   └── src/
│       ├── components/                 # ui/, layout/, TongQuanPortal, ThoiKhoaBieuTuan…
│       ├── contexts/NguCanhXacThuc.tsx
│       ├── lib/                        # api/{endpoints,types}, constants, format, roles, useApi
│       └── pages/                      # TrangChu, KhoaHoc, admin/, giaovien/, hocvien/
│
├── winforms/LanguageCenter.WinForms/   # Api/, Dialogs/, UserControls/, UI/
├── docs/                               # SETUP.md, api-endpoints.md, phase-1 → phase-7
├── LanguageCenter.sln
└── README.md
```

---

## 🚀 Cài đặt nhanh

| Yêu cầu | Phiên bản |
|---|---|
| .NET SDK | **10** |
| Node.js | **20+** (Vite 8) |
| SQL Server | Express 2012+ (`localhost\SQLEXPRESS`) |
| Visual Studio | 2022 (cho WinForms) |

1. **Tạo database**: chạy `database/QuanLyTrungTamNgoaiNgu_v2.sql` → `DuLieuMau_..._v2.sql` → **lần lượt v3 → v14**.
2. **Chạy API**: `cd backend/LanguageCenter.Api && dotnet run` → `http://localhost:5204` (Swagger: `/swagger`).
3. **Chạy web**: `cd web/HUITLINGO && npm install && npm run dev` → `http://localhost:5173`.
4. **Chạy WinForms**: mở `LanguageCenter.sln` bằng Visual Studio 2022 → F5.

> Chi tiết từng bước, thứ tự script SQL, cấu hình `appsettings`, JWT key và xử lý lỗi: xem **[docs/SETUP.md](docs/SETUP.md)**.

---

## 🔑 Tài khoản demo

Mật khẩu mặc định của **mọi tài khoản mới / sau khi đặt lại** là `111111` (chỉ lưu dạng hash).

| Vai trò | Tên đăng nhập | Ghi chú |
|---|---|---|
| Quản trị viên | `ADMIN001` | Dùng cho WinForms và `/admin` |
| Nhân viên học vụ | `NVHV001` | `/admin` |
| Giáo viên | `GV001`, `GV002` | `/giaovien` |
| Trợ giảng | `TG001` | `/giaovien` (cùng giao diện giáo viên) |
| Học viên | `HV001` … `HV004` | `/hocvien` |

> Seed data lưu `MatKhauHash` giả (`HASH_xxx`) nên cần khôi phục mật khẩu trước khi đăng nhập: chạy `database/KhoiPhuc_QuanTriVien.sql`, hoặc gọi `POST /api/development/reset-all-passwords` (chỉ Development) để đặt tất cả về `111111`.

---

## ✨ Tính năng theo vai trò

### 💻 WinForms — Quản trị viên

| Nhóm | Chức năng |
|---|---|
| Học viên | Thêm / sửa / xoá (chặn xoá khi còn dữ liệu), lọc theo trạng thái, phân trang |
| Giáo viên · Trợ giảng | CRUD, loại cố định theo tab, trạng thái làm việc, bằng cấp/chứng chỉ (chỉ đọc) |
| Chương trình · Cấp độ · Khóa học | CRUD, sắp xếp, gán cấp độ |
| Lớp học | CRUD, sĩ số, phòng, 2 thứ học, giờ học (tự suy ra giờ kết thúc +2h) |
| Phân công giảng dạy | Gán giáo viên chính / trợ giảng theo lớp |
| Lịch học | Sửa / xoá lịch tuần, sinh ca học hàng loạt, quản lý ngày lễ |
| Điểm số | Nhập điểm theo lớp, tính tổng kết theo trọng số |
| Học phí | Tạo phiếu, ghi nhận thanh toán, phiếu thu |
| Khác | Tài khoản, thông báo, khảo sát, báo cáo, dashboard tổng hợp |

### 🌐 Web — Quản trị (`/admin`)

Tổng quan số liệu toàn trung tâm · Học viên · Lớp học · Học phí · Thông báo · Khảo sát · Báo cáo · Cài đặt.

### 🌐 Web — Giáo viên & Trợ giảng (`/giaovien`)

| Chức năng | Ghi chú |
|---|---|
| Tổng quan | Hồ sơ, thông báo mới, ca dạy tuần này, tiến độ giảng dạy, lớp phụ trách |
| Lớp của tôi | Danh sách lớp được phân công kèm sĩ số, vai trò |
| Lịch dạy | Thời khoá biểu tuần, bấm ca → mở điểm danh |
| Điểm danh | Bảng ca học có bộ lọc, chọn ca bằng URL, tick "Có mặt" + lý do vắng, ghi chú, lưu hàng loạt |
| Bảng điểm | Lưới nhập điểm trực tiếp, tính tổng kết realtime |
| Thông báo · Cài đặt | Đọc thông báo, đổi mật khẩu |

### 🌐 Web — Học viên (`/hocvien`)

| Chức năng | Ghi chú |
|---|---|
| Tổng quan | Hồ sơ, thông báo mới, buổi học tuần này, học phí còn thiếu |
| Tiến độ khóa học | Timeline từng buổi, điểm danh, thành phần điểm, học phí |
| Lịch học | Thời khoá biểu tuần, chọn ngày xem chi tiết |
| Điểm số · Học phí | Bảng điểm theo lớp, công nợ |
| Khảo sát · Thông báo | Làm khảo sát, đọc thông báo |

---

## 📌 Nghiệp vụ cốt lõi

| Quy tắc | Chi tiết |
|---|---|
| Cấu trúc khóa học | 1 lớp = **20 buổi / 40 giờ** = 10 tuần × 2 buổi |
| Giờ học | Giờ kết thúc = giờ bắt đầu + **2 giờ**; giờ lưu trên `LopHoc` là nguồn duy nhất |
| Thứ học | `ThuBuoi1` và `ThuBuoi2` phải khác nhau |
| Ngày lễ | Ca trùng ngày lễ được đẩy **+7 ngày** và dây chuyền các buổi sau |
| Ngày nhập học | = ngày **bắt đầu lớp** (lớp sớm nhất học viên tham gia) |
| Điểm tổng kết | Tính theo trọng số từng đầu điểm; cảnh báo khi tổng trọng số ≠ 100% |
| Xếp lớp | Khoá dòng lớp bằng `UPDLOCK/HOLDLOCK` trong transaction → không vượt sĩ số |
| Mật khẩu | Mặc định `111111` khi tạo tài khoản / đặt lại; đổi mật khẩu phải theo policy |
| Xoá dữ liệu | Xoá cứng chỉ khi không còn bản ghi phụ thuộc, nếu không trả **409** kèm hướng dẫn |

---

## 🔌 API

- **Base URL:** `http://localhost:5204` · **Swagger UI:** `/swagger` · **OpenAPI JSON:** `/swagger/v1/swagger.json`
- **Xác thực:** `Authorization: Bearer <accessToken>` (JWT HS256, hết hạn sau 60 phút)
- **Phân trang:** `PagedResult<T>` = `{ items, page, pageSize, totalItems, totalPages }`

| Nhóm | Controller |
|---|---|
| Health / Development | `Health`, `Development` |
| Xác thực | `Auth` |
| Tài khoản | `AdminAccounts` |
| Đào tạo | `AdminPrograms`, `AdminLevels`, `AdminCourses`, `AdminAcademicSetup` |
| Lớp & lịch | `AdminClasses`, `AdminSchedules`, `AdminSessions`, `AdminRooms`, `AdminTeachingAssignments` |
| Học vụ | `AdminStudents`, `AdminTeachers`, `AdminEnrollments`, `AdminPromotions` |
| Điểm & điểm danh | `AdminGrades`, `AdminGradeTypes`, `AdminAttendance` |
| Học phí | `AdminTuitionFees`, `AdminPayments`, `AdminReceipts` |
| Khác | `AdminNotifications`, `AdminSurveys`, `AdminReports` |
| Tự phục vụ | `StudentProfile`, `StudentSelf`, `StudentFinance`, `StudentSurvey`, `TeacherProfile`, `TeacherSelf`, `Notifications` |

> Bảng đầy đủ 155 endpoint (method, URL, quyền, body): **[docs/api-endpoints.md](docs/api-endpoints.md)**.

---

## 🧯 Troubleshooting nhanh

| Hiện tượng | Nguyên nhân / cách xử lý |
|---|---|
| `MSB3021 / MSB3026 / MSB3027` khi build API | API đang chạy đang giữ file DLL → tắt API rồi build lại |
| Đăng nhập báo sai mật khẩu | Seed dùng hash giả → chạy `KhoiPhuc_QuanTriVien.sql` hoặc reset mật khẩu |
| 403 ở WinForms | Vai trò không đủ quyền — WinForms chỉ dành cho Quản trị viên / Nhân viên |
| `Failed to determine the https port` | Chạy profile `http` hoặc dùng `http://localhost:5204` |
| Web gọi API bị CORS | Kiểm tra `Cors:AllowedOrigins` có `http://localhost:5173` |

> Chi tiết hơn: **[docs/SETUP.md](docs/SETUP.md)**.

---

## 🚧 Hạn chế & hướng phát triển

- Chưa có unit/integration test tự động và CI/CD.
- Chưa tích hợp thanh toán online, gửi email/SMS thông báo, upload tài liệu học tập.
- Báo cáo chưa xuất Excel/PDF; dashboard chưa có bộ lọc theo thời gian.
- Chưa có app mobile cho học viên.

---

## 👥 Thành viên nhóm

| # | Họ tên | MSSV | Vai trò |
|---|--------|------|---------|
| 1 | **Lê Thị Thanh Huyền** | 2001230317 | Full-stack Developer |
| 2 | **Trần Ngọc Trúc Huỳnh** | 2001230318 | Full-stack Developer |
| 3 | **Nguyễn Văn Thành** | 2001230895 | Full-stack Developer |

**GVHD:** Lâm Thị Họa Mi — [milth@huit.edu.vn](mailto:milth@huit.edu.vn)

---

## 📚 Tài liệu tham khảo

1. Phạm Nguyễn Cương, Nguyễn Trần Minh Thư, Hồ Bảo Quốc, *Giáo trình Phân tích thiết kế hệ thống thông tin theo hướng đối tượng*, NXB Khoa học và Kỹ thuật, 2016.
2. *Bài giảng Công nghệ phần mềm*, Khoa CNTT, ĐH Công Thương TP.HCM.
3. [React Documentation](https://react.dev)
4. [Vite Documentation](https://vitejs.dev)
5. [ASP.NET Core Documentation](https://docs.microsoft.com/aspnet/core)

---

## 📞 Liên hệ
- **Trường:** Đại học Công Thương TP.HCM (HUIT)
- **Khoa:** Công nghệ Thông tin
- **Lớp:** 14DHTH14
- **GitHub:** [github.com/Thanh-Bo/LanguageCenter](https://github.com/Thanh-Bo/LanguageCenter)
- **Báo lỗi / góp ý:** [GitHub Issues](https://github.com/Thanh-Bo/LanguageCenter/issues)

---

## 📄 Giấy phép

Đồ án khóa luận tốt nghiệp — **HUIT © 2026**

---

<p align="center">
  <strong>Made with ❤️ by LingoHub Team — HUIT</strong>
</p>
