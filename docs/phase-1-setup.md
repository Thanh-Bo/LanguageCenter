# Phase 1 — Nền tảng Backend (ASP.NET Core Web API + SQL Server)

> Trạng thái: **HOÀN THÀNH** — solution build sạch, Swagger chạy, EF Core đọc được dữ liệu thật.

## 1. Phạm vi Phase 1

| Có làm | KHÔNG làm (để phase sau) |
|---|---|
| Dựng 4 project backend + project reference | Login / JWT / Authentication / Authorization |
| EF Core Database First (32 entity + Fluent API) | CRUD nghiệp vụ (HocVien, GiaoVien, LopHoc...) |
| Swagger UI, CORS, Global Exception Handling | Attendance, Grade, Payment, Notification, Report |
| Health check + test kết nối database | WinForms UI, Next.js UI |

## 2. Kiến trúc & dependency

```
LanguageCenter.sln
├── backend/
│   ├── LanguageCenter.Domain/          (Entities/, 32 entity POCO — KHÔNG reference ai)
│   ├── LanguageCenter.Application/     (→ Domain)
│   ├── LanguageCenter.Infrastructure/  (→ Domain, Application; EF Core + SQL Server)
│   └── LanguageCenter.Api/             (→ Application, Infrastructure)
├── winforms/   (ngoài phạm vi Phase 1)
├── web/        (Next.js, ngoài phạm vi Phase 1)
└── database/   (script SQL gốc — nguồn dữ liệu chính)
```

Quy tắc đã tuân thủ: `Domain` không reference `Infrastructure`/`Api`; `Application` không reference `Api`.

## 3. Cách chạy backend

### Cách 1 — dòng lệnh

```powershell
cd d:\LanguageCenter
dotnet restore
dotnet build
dotnet run --project backend/LanguageCenter.Api/LanguageCenter.Api.csproj --launch-profile http
```

### Cách 2 — VS Code

- `Ctrl+Shift+B` → chạy task **build**
- Terminal → Run Task → **run-api**
- `F5` → debug bằng cấu hình `LanguageCenter.Api (http)` (đã tự mở Swagger)

### URL

| Endpoint | Mô tả |
|---|---|
| http://localhost:5204/swagger | **Swagger UI** |
| http://localhost:5204/swagger/v1/swagger.json | OpenAPI document |
| http://localhost:5204/health | API còn sống? |
| http://localhost:5204/health/database | EF Core → SQL Server OK? |
| http://localhost:5204/api/development/database-info | **Development-only** — thông tin + số record |

## 4. Cấu hình

`appsettings.json` — chỉ cấu hình chung:

```jsonc
"Cors": { "AllowedOrigins": [ "http://localhost:3000" ] }
```

`appsettings.Development.json` — connection string (Windows Authentication, **không có password**):

```
Server=localhost\SQLEXPRESS;Database=QuanLyTrungTamNgoaiNgu;Trusted_Connection=True;
Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True
```

> ⚠️ Nếu chuyển sang SQL Login hoặc Azure SQL: chuyển connection string sang **User Secrets**
> (`dotnet user-secrets set "ConnectionStrings:DefaultConnection" "..."`) — **không commit password**.

## 5. CORS

- Policy: `NextJsFrontend`
- Origin tường minh: `http://localhost:3000`
- **Không** dùng `AllowAnyOrigin` + `AllowCredentials`
- WinForms không bị ảnh hưởng (không chạy trong browser)

## 6. Kết quả kiểm chứng (đã chạy thật)

```jsonc
// GET /health
{ "status": "Healthy" }

// GET /health/database
{ "database": "QuanLyTrungTamNgoaiNgu", "status": "Connected", "vaiTroCount": 7 }

// GET /api/development/database-info
{
  "database": "QuanLyTrungTamNgoaiNgu",
  "status": "Connected",
  "provider": "Microsoft.EntityFrameworkCore.SqlServer",
  "sqlServerVersion": "11.00.6020",
  "vaiTroCount": 7,
  "taiKhoanCount": 11,
  "hocVienCount": 4
}
```

Chuỗi đã được chứng minh:
`Swagger → ASP.NET Core API → EF Core → SQL Server → database QuanLyTrungTamNgoaiNgu`

## 7. Những điểm mapping quan trọng (bẫy schema)

| Bẫy | Xử lý trong `Infrastructure/Data/Configurations` |
|---|---|
| 3 cột **computed PERSISTED** | `DangKyHoc.HocPhiSauGiam`, `HocPhi.SoTienPhaiDong`, `HocPhi.SoTienConThieu` → `HasComputedColumnSql(..., stored: true)`. **Không bao giờ ghi** các cột này khi INSERT/UPDATE |
| FK trỏ vào **cột UNIQUE**, không phải PK | `NhanVien.MaNhanVien`, `GiaoVien.MaGiaoVien`, `HocVien.MaHocVien` → `TaiKhoan.TenDangNhap` qua `HasPrincipalKey` + `HasAlternateKey` |
| **2 FK cùng trỏ 1 bảng** | `CaHoc` → `GiaoVien` (GV thực tế / trợ giảng); `ThongBao` → `TaiKhoan` (người gửi / người nhận) → cấu hình `HasOne/WithMany` tường minh |
| **ON DELETE** | Chỉ `BangCap`, `ChungChi`, `CauHoiKhaoSat`, `CauTraLoiKhaoSat` là `CASCADE`; còn lại `NO ACTION` → `DeleteBehavior.Restrict` |
| **Filtered unique index** | `UX_ThanhToan_MaGiaoDich` → `HasFilter("[MaGiaoDich] IS NOT NULL")` |
| **Kiểu dữ liệu** | `TINYINT → byte`, `DATE → DateOnly`, `TIME → TimeOnly`, `DATETIME2 → DateTime`, `DECIMAL(p,s) → decimal` + `HasPrecision` |

**Không** khai báo lại các `CHECK CONSTRAINT` trong EF (database là nguồn sự thật — tránh lệch schema).

## 8. ⚠️ CẢNH BÁO VỀ SCRIPT DATABASE

| File | Cảnh báo |
|---|---|
| `database/QuanLyTrungTamNgoaiNgu_v2.sql` | **CÓ `DROP DATABASE`** — chạy lại sẽ **XOÁ SẠCH** database. TUYỆT ĐỐI không chạy khi DB đã có dữ liệu |
| `database/DuLieuMau_QuanLyTrungTamNgoaiNgu_v2.sql` | **Không idempotent** — chạy lần 2 sẽ vi phạm UNIQUE/PK. Chỉ chạy 1 lần trên DB trống |

Phase 1 **không chạy migration** và **không sửa schema**.

## 9. Ghi chú kỹ thuật

- `Microsoft.EntityFrameworkCore.Design` được cài ở project `Api` (`PrivateAssets="all"`) để dùng `dotnet ef` ở Phase 2 — Phase 1 không chạy migration.
- `Microsoft.OpenApi` được **ghim trực tiếp 2.7.5** để vá CVE-2026-49451 (GHSA-v5pm-xwqc-g5wc) mà Swashbuckle kéo theo bản 2.3.0 lỗi.
- `UseHttpsRedirection()` **không bật** ở Phase 1 (profile `http` chỉ bind cổng http).
- SQL Server instance hiện tại báo version `11.00.6020` (SQL Server 2012) — kết nối và truy vấn vẫn hoạt động bình thường.
