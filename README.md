# 🎓 HUIT LingoHub — Hệ thống quản lý Trung tâm Ngoại ngữ

> **Đồ án khóa luận tốt nghiệp** — Khoa Công nghệ Thông tin, Trường Đại học Công Thương TP.HCM (HUIT).

![Status](https://img.shields.io/badge/status-in%20progress-yellow)
![License](https://img.shields.io/badge/license-academic-blue)
![Frontend](https://img.shields.io/badge/frontend-React%2019%20%2B%20Vite-61DAFB)
![Backend](https://img.shields.io/badge/backend-.NET%20Core-512BD4)
![Database](https://img.shields.io/badge/database-SQL%20Server-CC2927)

---

## 📖 Thông tin đề tài

| Mục | Nội dung |
|-----|----------|
| **Mã đề tài** | CNTT-KLCN013 |
| **Tên đề tài** | Xây dựng hệ thống quản lý Trung tâm Ngoại ngữ |
| **GVHD** | Lâm Thị Họa Mi |
| **Thành viên** | Lê Thị Thanh Huyền, Trần Ngọc Trúc Huỳnh, Nguyễn Văn Thành |
| **Thời gian** | 12 tuần (08/09/2025 → 30/11/2025) |

---

## 🏗️ Kiến trúc hệ thống

Hệ thống gồm **2 phân hệ chính**, dùng chung backend API:

```
┌──────────────────────────────────────────────┐
│       BACKEND API (.NET Core)                │
│       ✓ Đồng bộ dữ liệu 2 phân hệ            │
│       ✓ Hỗ trợ 500+ user đồng thời           │
└───────────────┬──────────────────────────────┘
                │
        ┌───────┴────────┐
        │                │
   ┌────▼────┐      ┌────▼─────┐
   │WinForms │      │ Website  │
   │(Admin)  │      │(GV + HV) │
   └─────────┘      └──────────┘
```

---

## 🛠️ Công nghệ sử dụng

### 🌐 Frontend (Website — Giáo viên & Học viên)

| Công nghệ | Phiên bản | Vai trò |
|-----------|-----------|---------|
| React | 19 | UI Framework |
| TypeScript | 5.7 | Type safety |
| Vite | 8 | Build tool |
| Tailwind CSS | 4 | Styling |
| React Router | 8 | Routing |

### ⚙️ Backend (API)

| Công nghệ | Vai trò |
|-----------|---------|
| ASP.NET Core | Web API |
| Entity Framework Core | ORM |
| SQL Server | Database |
| Clean Architecture | Kiến trúc |

### 💻 Desktop (Quản trị viên)

- **WinForms** (.NET)

---

## 📁 Cấu trúc thư mục

```
LanguageCenter/
├── backend/                          # ASP.NET Core API
│   ├── LanguageCenter.Api/           # Controllers, Program.cs
│   ├── LanguageCenter.Application/   # Use cases, DTOs
│   ├── LanguageCenter.Domain/        # Entities, Interfaces
│   └── LanguageCenter.Infrastructure/# EF Core, Repositories
│
├── database/                         # SQL Scripts
│   ├── DuLieuMau_QuanLyTrungTamNgoaiNgu_v*.sql
│   ├── KhoiPhuc_QuanTriVien.sql
│   └── QuanLyTrungTamNgoaiNgu_v2.sql
│
├── docs/                             # Tài liệu dự án
│   ├── phase-1-setup.md
│   ├── phase-2-auth.md
│   └── phase-3-accounts.md
│
├── web/
│   └── HUITLINGO/                    # Frontend React
│       ├── src/
│       │   ├── assets/               # Hình ảnh, màu sắc
│       │   ├── components/           # BoCuc, BaoVe, BieuTuong, TroLyTuVan
│       │   ├── contexts/             # NguCanhXacThuc
│       │   ├── imports/              # Imported images
│       │   ├── lib/                  # khoDuLieu, goiAPI
│       │   └── pages/                # TrangChu, KhoaHoc, GiaoVien, HocPhi...
│       ├── App.tsx
│       ├── duLieu.ts
│       ├── index.css
│       ├── main.tsx
│       ├── tuyenDuong.tsx            # Router
│       ├── vite-env.d.ts
│       ├── index.html
│       ├── package.json
│       ├── tsconfig.json
│       └── vite.config.ts
│
├── winforms/                         # Desktop app
│   └── LanguageCenter.WinForms/
│
├── LanguageCenter.sln                # Solution file
└── README.md                         # 📍 File này
```

---

## 🚀 Hướng dẫn cài đặt & chạy

### 📋 Yêu cầu hệ thống

- **Node.js** ≥ 20.0.0 — [Download](https://nodejs.org/)
- **.NET SDK** ≥ 8.0 — [Download](https://dotnet.microsoft.com/)
- **SQL Server** ≥ 2019 — [Download](https://www.microsoft.com/sql-server/)
- **Visual Studio** ≥ 2022 (cho WinForms)

### 🌐 Chạy Frontend (Website)

```bash
# 1. Vào thư mục frontend
cd web/HUITLINGO

# 2. Cài đặt dependencies
npm install

# 3. Chạy dev server
npm run dev
```

**→ Mở trình duyệt:** http://localhost:5173

### ⚙️ Chạy Backend (API)

```bash
# 1. Vào thư mục backend
cd backend/LanguageCenter.Api

# 2. Chạy API
dotnet run
```

**→ API chạy tại:** https://localhost:5001

### 💻 Chạy WinForms (Desktop)

1. Mở file `LanguageCenter.sln` bằng **Visual Studio 2022**
2. Nhấn **F5** để chạy

---

## 🔑 Tài khoản demo

### 🌐 Website (Giáo viên + Học viên)

| Vai trò | Email | Mật khẩu |
|---------|-------|----------|
| 👨‍💼 Quản trị viên | `admin@lingohub.vn` | `123456` |
| 👨‍🏫 Giáo viên | `giaovien@lingohub.vn` | `123456` |
| 👩‍🎓 Học viên | `hocvien@lingohub.vn` | `123456` |

### 💻 WinForms (Admin)

Xem trong `database/KhoiPhuc_QuanTriVien.sql`

---

## ✨ Tính năng

### ✅ Đã hoàn thành

#### Chung
- [x] Kiến trúc 2 phân hệ (WinForms + Website)
- [x] Backend API chung (Clean Architecture)
- [x] Giao diện tiếng Việt, responsive

#### 🌐 Website — Xác thực & Phân quyền
- [x] Đăng nhập / Đăng xuất
- [x] Phân quyền **3 vai trò** (Quản trị viên / Giáo viên / Học viên)
- [x] Chặn truy cập trái phép (BaoVe guard)
- [x] Tự động redirect theo vai trò
- [x] Sidebar động theo role
- [x] Navbar động ("Đăng nhập" ⇄ "Vào Portal")
- [x] Lưu session (localStorage)

#### 🌐 Website — Giao diện
- [x] Trang chủ (hero, khóa học, giáo viên, đánh giá)
- [x] Trang khóa học (filter, chi tiết cấp độ)
- [x] Trang đội ngũ giáo viên (filter, modal chi tiết)
- [x] Trang học phí (3 gói, bảng giá, FAQ)
- [x] Chatbot trợ lý tư vấn

### 🔄 Đang phát triển

#### 🌐 Website — Giáo viên
- [ ] Xem danh sách lớp dạy
- [ ] Xem chi tiết lớp + danh sách học viên
- [ ] Điểm danh học viên (checkbox + ghi chú)
- [ ] Nhập điểm (midterm, final, homework)
- [ ] Xem lịch dạy (tuần/tháng)
- [ ] Quản lý thông tin cá nhân

#### 🌐 Website — Học viên
- [ ] Xem lịch học cá nhân
- [ ] Xem điểm số & nhận xét
- [ ] Nhận thông báo từ trung tâm
- [ ] Quản lý thông tin cá nhân

#### 💻 WinForms — Quản trị viên
- [ ] Quản lý tài khoản (CRUD, khóa/mở)
- [ ] Quản lý lớp học (CRUD)
- [ ] Phân công giáo viên
- [ ] Quản lý lịch học
- [ ] Quản lý điểm số
- [ ] Quản lý học phí, công nợ
- [ ] Thống kê báo cáo + biểu đồ

---

## 📅 Tiến độ dự án

| Tuần | Nội dung công việc | Trạng thái |
|------|-------------------|-----------|
| **1** | Khảo sát, Use-case, phân công | ✅ Hoàn thành |
| **2** | ERD, phân quyền, chọn công nghệ | ✅ Hoàn thành |
| **3-6** | WinForms (admin) + API backend | 🔄 Đang làm |
| **7-9** | Website (đăng nhập, lớp, điểm danh, nhập điểm) | 🔄 Đang làm |
| **10** | Kết nối API Web ↔ Desktop | ⬜ Chưa bắt đầu |
| **11** | Kiểm thử các vai trò + chức năng | ⬜ Chưa bắt đầu |
| **12** | Báo cáo + PowerPoint + nộp | ⬜ Chưa bắt đầu |

---

## 👥 Thành viên nhóm
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
- **Lớp:**14DHTH14
- **Email liên hệ:** 

---

## 📄 Giấy phép

Đồ án khóa luận tốt nghiệp — **HUIT © 2026**

---

<p align="center">
  <strong>Made with ❤️ by HUIT LingoHub Team</strong>
</p>