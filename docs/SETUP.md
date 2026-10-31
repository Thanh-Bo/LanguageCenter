# 🛠️ Hướng dẫn cài đặt & chạy — LingoHub

Tài liệu này đi từ **số 0** đến lúc chạy được cả 3 giao diện (API, website, WinForms).
Xem tổng quan dự án tại [README](../README.md) và danh sách API tại [api-endpoints.md](api-endpoints.md).

---

## 1. Yêu cầu hệ thống

| Thành phần | Phiên bản | Ghi chú |
|---|---|---|
| .NET SDK | **10** | `dotnet --version` phải ra `10.x` |
| Node.js | **20+** | Vite 8 yêu cầu Node 20.19+ / 22.12+ |
| SQL Server | Express 2012+ | Instance mặc định `localhost\SQLEXPRESS`, xác thực Windows |
| Visual Studio | 2022 | Chỉ cần khi chạy WinForms (hoặc `dotnet run` từ CLI) |
| Công cụ SQL | SSMS / Azure Data Studio | Để chạy các script trong `database/` |

Kiểm tra nhanh:

```bash
dotnet --version     # 10.x
node --version       # v20+
npm --version
```

---

## 2. Tạo cơ sở dữ liệu

> Thứ tự **bắt buộc**: schema → dữ liệu mẫu → các bản vá v3 → v14.
> Không chạy lại `v2` sau khi đã có dữ liệu (script DROP/CREATE bảng).

| Bước | File | Nội dung |
|---|---|---|
| 1 | `database/QuanLyTrungTamNgoaiNgu_v2.sql` | Tạo 32 bảng, khoá ngoại, CHECK, computed column, dữ liệu vai trò |
| 2 | `database/DuLieuMau_QuanLyTrungTamNgoaiNgu_v2.sql` | Dữ liệu mẫu (tài khoản, học viên, lớp, lịch, điểm…) |
| 3 | `BoSung_ChuongTrinh_v3.sql` | Bổ sung chương trình đào tạo |
| 4 | `BoSung_PhanCong_v4.sql` | Phân công giảng dạy |
| 5 | `DonDep_DuLieu_v5.sql` | Dọn dữ liệu rác |
| 6 | `DoiTen_KhoaHoc_v6.sql` | Đổi tên khóa học |
| 7 | `ChinhTrangThaiHocVien_v7.sql` | Chuẩn hoá trạng thái học viên |
| 8 | `BoSung_GioHoc_LopHoc_v8.sql` | Thêm giờ học vào `LopHoc` |
| 9 | `BoSung_GanGiaoVienCaHoc_v9.sql` | Gán giáo viên thực tế cho ca học |
| 10 | `BoSung_20Buoi_NoiDung_NgayLe_v10.sql` | 20 buổi, nội dung bài học, ngày lễ |
| 11 | `BoSung_ThuHoc_LopHoc_v11.sql` | Thứ học của lớp |
| 12 | `DongBo_GioHoc_v12.sql` | Đồng bộ giờ học về một nguồn duy nhất |
| 13 | `DonNoiDungBaiHoc_v13.sql` | Chuẩn hoá nội dung bài học |
| 14 | `DongBo_NgayNhapHoc_v14.sql` | Ngày nhập học = ngày bắt đầu lớp |

Chạy bằng SSMS: **File → Open → File…** → Execute (`F5`). Mỗi script đều idempotent (chạy lại an toàn) trừ `v2`/`DuLieuMau_v2`.

### Khôi phục tài khoản Quản trị viên

`database/KhoiPhuc_QuanTriVien.sql` — dùng khi đã xoá hết admin: script **tự kiểm tra hiện trạng**, copy hash mật khẩu từ `NVHV001` sang các tài khoản admin mới.

---

## 3. Cấu hình Backend

`backend/LanguageCenter.Api/appsettings.Development.json`:

```jsonc
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=QuanLyTrungTamNgoaiNgu;Trusted_Connection=True;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;Application Name=LanguageCenter.Api"
  },
  "Cors": {
    "AllowedOrigins": ["http://localhost:3000", "http://localhost:5173"]
  }
}
```

| Thiết lập | Ghi chú |
|---|---|
| `ConnectionStrings:DefaultConnection` | Đổi `Server=` nếu SQL Server của bạn dùng instance khác |
| `Jwt:Issuer` / `Jwt:Audience` / `Jwt:AccessTokenExpirationMinutes` | Trong `appsettings.json`; **khoá bí mật JWT để ở User Secrets**, không commit |
| `Cors:AllowedOrigins` | Phải chứa origin của web (`5173` với Vite) |

Đặt khoá JWT bằng User Secrets (khuyến nghị):

```bash
cd backend/LanguageCenter.Api
dotnet user-secrets init
dotnet user-secrets set "Jwt:SecretKey" "<chuỗi-bí-mật-dài-32-ký-tự>"
```

> Lưu ý: cấu hình dùng **Windows Authentication** nên không có mật khẩu trong file. Nếu chuyển sang SQL Login / Azure SQL, hãy đưa connection string vào User Secrets hoặc biến môi trường.

---

## 4. Chạy Backend (API)

```bash
cd backend/LanguageCenter.Api
dotnet run
```

| Mục | Giá trị |
|---|---|
| HTTP | `http://localhost:5204` |
| HTTPS | `https://localhost:7103` |
| Swagger UI | `http://localhost:5204/swagger` |
| OpenAPI JSON | `http://localhost:5204/swagger/v1/swagger.json` |
| Health check | `GET /health`, `GET /health/database` |

Kiểm tra nhanh API đã lên:

```bash
curl http://localhost:5204/health
```

---

## 5. Chạy Website

```bash
cd web/HUITLINGO
npm install
npm run dev
```

| Lệnh | Việc |
|---|---|
| `npm run dev` | Dev server `http://localhost:5173` |
| `npm run build` | `tsc -b && vite build` (build thất bại nếu sai kiểu) |
| `npm run preview` | Xem bản build |
| `npm run lint` | `tsc --noEmit` |

Đường dẫn theo vai trò sau khi đăng nhập:

| Vai trò | Khu vực |
|---|---|
| Quản trị viên, NV học vụ, NV tư vấn, NV kế toán | `/admin` |
| Giáo viên, Trợ giảng | `/giaovien` |
| Học viên | `/hocvien` |

---

## 6. Chạy WinForms (Desktop)

**Cách 1 — Visual Studio:** mở `LanguageCenter.sln` → chọn project `LanguageCenter.WinForms` → `F5`.

**Cách 2 — CLI:**

```bash
cd winforms/LanguageCenter.WinForms
dotnet run
```

> WinForms gọi cùng API (`http://localhost:5204`) nên **API phải chạy trước**.
> Giao diện này chỉ dành cho **Quản trị viên / Nhân viên**; giáo viên và trợ giảng dùng bản web.

---

## 7. Tài khoản demo

Mật khẩu mặc định của tài khoản mới / sau khi đặt lại: **`111111`**.

| Vai trò | Tên đăng nhập | Dùng ở đâu |
|---|---|---|
| Quản trị viên | `ADMIN001` | WinForms + `/admin` |
| Nhân viên học vụ | `NVHV001` | `/admin` |
| Nhân viên tư vấn / kế toán | `NVTV001` / `NVKT001` | `/admin` |
| Giáo viên | `GV001`, `GV002` | `/giaovien` |
| Trợ giảng | `TG001` | `/giaovien` |
| Học viên | `HV001` … `HV004` | `/hocvien` |

Nếu đăng nhập báo sai mật khẩu (seed lưu hash giả `HASH_xxx`):

```bash
# Cách 1 (Development, đặt TẤT CẢ tài khoản về 111111)
curl -X POST http://localhost:5204/api/development/reset-all-passwords

# Cách 2: chạy database/KhoiPhuc_QuanTriVien.sql trong SSMS
```

---

## 8. Lỗi thường gặp

| Lỗi | Nguyên nhân | Cách xử lý |
|---|---|---|
| `MSB3021` / `MSB3026` / `MSB3027` khi build | API đang chạy giữ file DLL trong `bin/` | Tắt API (hoặc `dotnet build` khi app đã đóng) rồi build lại |
| `Failed to determine the https port for redirect` | Chạy profile `http` nhưng app bật `UseHttpsRedirection` | Dùng `http://localhost:5204`, hoặc chạy profile `https` |
| Web báo lỗi CORS | Origin không nằm trong `Cors:AllowedOrigins` | Thêm `http://localhost:5173` vào `appsettings.Development.json` |
| 401 khi gọi API | Thiếu/hết hạn token (60 phút) | Đăng nhập lại, gửi header `Authorization: Bearer <token>` |
| 403 ở WinForms | Vai trò không đủ quyền | Dùng tài khoản Quản trị viên / Nhân viên |
| `Login failed for user` | Chuỗi kết nối sai instance | Sửa `Server=` trong `appsettings.Development.json` |
| Dữ liệu lệch giờ học / ngày nhập học | Chưa chạy script đồng bộ | Chạy `DongBo_GioHoc_v12.sql` và `DongBo_NgayNhapHoc_v14.sql` |
| `No row can be added to a DataGridView…` (WinForms) | Màn hình cũ chưa được dispose khi chuyển trang | Đã xử lý trong code; nếu gặp lại, build lại toàn bộ solution |
| `tsc` báo lỗi kiểu ở web | Sửa DTO/type không đồng bộ | Chạy `npm run lint` và sửa theo thông báo |

---

## 9. Checklist trước khi demo / nộp bài

- [ ] Chạy đủ script `v2` → `v14`, kiểm tra `SELECT COUNT(*) FROM sys.tables` = **32**
- [ ] `dotnet build LanguageCenter.sln` không lỗi (bỏ qua cảnh báo MSB nếu API đang chạy)
- [ ] API lên, `/health/database` trả OK, Swagger mở được
- [ ] Web build sạch: `cd web/HUITLINGO && npm run lint`
- [ ] Đăng nhập thử 4 loại tài khoản: admin, học vụ, giáo viên, học viên
- [ ] WinForms mở được, CRUD học viên/giáo viên/lớp hoạt động
- [ ] Điểm danh 1 ca + lưu thành công, xem lại dữ liệu đã lưu
