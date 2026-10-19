# Phase 4 — Academic Management + Enrollment (API Reference)

> Base URL: `http://localhost:5204`
> Swagger: `http://localhost:5204/swagger`
>
> Tất cả endpoint dưới đây yêu cầu header:
> `Authorization: Bearer <accessToken>`
> `Content-Type: application/json`

## 0. Đăng nhập theo vai trò

| Vai trò | Username | Password |
|---|---|---|
| Quản trị viên | `ADMIN001` | `LanguageCenter@2026` |
| Nhân viên học vụ | `NVHV001` | `LanguageCenter@2026` |
| Nhân viên tư vấn | `NVTV001` | `LanguageCenter@2026` |

```http
POST /api/auth/login
{ "username": "ADMIN001", "password": "LanguageCenter@2026" }
```

## Phân quyền

| Nhóm endpoint | Quản trị viên | Nhân viên học vụ | Nhân viên tư vấn |
|---|---|---|---|
| `/api/admin/programs` | ✅ | ❌ | ❌ |
| `/api/admin/levels` | ✅ | ❌ | ❌ |
| `/api/admin/courses` | ✅ | ❌ | ❌ |
| `/api/admin/promotions` | ✅ | ❌ | ❌ |
| `/api/admin/classes` | ✅ | ✅ | ❌ |
| `/api/admin/teaching-assignments` | ✅ | ✅ | ❌ |
| `/api/admin/enrollments` | ✅ | ❌ | ✅ |

Sai vai trò → `403`. Không có token → `401`.

> **Thứ tự test khuyến nghị:** Programs → Levels → Courses → Promotions → Classes → TeachingAssignments → Enrollments

---

## 1. Chương trình đào tạo — `/api/admin/programs`

| Method | URL | Mô tả |
|---|---|---|
| GET | `/api/admin/programs` | `?search=&trangThai=&page=1&pageSize=20` |
| GET | `/api/admin/programs/{id}` | Chi tiết |
| POST | `/api/admin/programs` | Tạo mới |
| PATCH | `/api/admin/programs/{id}` | Sửa (PATCH) |
| PATCH | `/api/admin/programs/{id}/status` | Đổi trạng thái |

**Tạo mới:**
```json
{
  "tenChuongTrinh": "IELTS 2027",
  "nhomDoiTuong": "Học sinh, sinh viên và người đi làm",
  "doTuoiTu": 15,
  "doTuoiDen": 45,
  "mucTieu": "Đạt IELTS 6.5+",
  "moTa": "Chương trình luyện thi IELTS"
}
```
→ `201`

**Trạng thái hợp lệ:** `Hoạt động` · `Ngừng hoạt động`

```json
{ "trangThai": "Ngừng hoạt động" }
```

**Lỗi thường gặp**
- `409` — tên chương trình đã tồn tại
- `400` — `doTuoiDen < doTuoiTu`

---

## 2. Cấp độ — `/api/admin/levels`

| Method | URL |
|---|---|
| GET | `/api/admin/levels?maChuongTrinh=1&search=&trangThai=&page=1&pageSize=20` |
| GET | `/api/admin/levels/{id}` |
| POST | `/api/admin/levels` |
| PATCH | `/api/admin/levels/{id}` |
| PATCH | `/api/admin/levels/{id}/status` |

**Tạo mới:**
```json
{
  "maChuongTrinh": 1,
  "kyHieuCapDo": "IELTS-F",
  "tenCapDo": "IELTS Foundation",
  "thuTu": 1,
  "dieuKienDauVao": "IELTS 4.0 - 5.0",
  "ketQuaDauRa": "IELTS 5.5 - 6.0",
  "thoiLuongDuKienTuan": 12
}
```
→ `201`

**Lỗi thường gặp**
- `400` — `maChuongTrinh` không tồn tại
- `409` — trùng **tên** hoặc trùng **thứ tự** trong cùng chương trình
  (DB có `UNIQUE (MaChuongTrinh, TenCapDo)` và `UNIQUE (MaChuongTrinh, ThuTu)`)

> `maChuongTrinh` **không** sửa được qua PATCH — muốn chuyển chương trình phải tạo cấp độ mới.

---

## 3. Khóa học — `/api/admin/courses`

| Method | URL |
|---|---|
| GET | `/api/admin/courses?maCapDo=1&maChuongTrinh=1&search=&trangThai=&page=1&pageSize=20` |
| GET | `/api/admin/courses/{id}` |
| POST | `/api/admin/courses` |
| PATCH | `/api/admin/courses/{id}` |
| PATCH | `/api/admin/courses/{id}/status` |

**Tạo mới:**
```json
{
  "maCapDo": 1,
  "tenKhoaHoc": "IELTS Foundation 2027 - Đợt 1",
  "noiDung": "Luyện 4 kỹ năng và kỹ thuật làm bài",
  "soBuoi": 24,
  "tongSoGio": 48,
  "hocPhiCoBan": 6000000,
  "ngayBatDau": "2027-01-05",
  "ngayKetThuc": "2027-04-30"
}
```
→ `201`

**Trạng thái hợp lệ:** `Đang tuyển sinh` · `Đang học` · `Đã kết thúc` · `Đã hủy`
(khóa mới tạo mặc định `Đang tuyển sinh`)

**Lỗi thường gặp**
- `400` — `soBuoi <= 0`, `hocPhiCoBan < 0`, `ngayKetThuc < ngayBatDau`, `maCapDo` không tồn tại
- `409` — trùng `(MaCapDo, TenKhoaHoc)`

> `ngayBatDau` / `ngayKetThuc` là **thời gian tổ chức khóa học**, độc lập với ngày của từng lớp.

---

## 4. Ưu đãi — `/api/admin/promotions`

| Method | URL |
|---|---|
| GET | `/api/admin/promotions?search=&trangThai=&conHieuLuc=true&page=1&pageSize=20` |
| GET | `/api/admin/promotions/{id}` |
| POST | `/api/admin/promotions` |
| PATCH | `/api/admin/promotions/{id}` |
| PATCH | `/api/admin/promotions/{id}/status` |

**Tạo mới — giảm theo phần trăm:**
```json
{
  "tenUuDai": "Ưu đãi 10% đầu năm 2027",
  "loaiGiamGia": "Phần trăm",
  "giaTriGiam": 10,
  "soKhoaHocToiThieu": 1,
  "ngayBatDau": "2026-10-01",
  "ngayKetThuc": "2027-12-31",
  "moTa": "Giảm 10% học phí"
}
```

**Tạo mới — giảm số tiền:**
```json
{
  "tenUuDai": "Giảm 500.000đ",
  "loaiGiamGia": "Số tiền",
  "giaTriGiam": 500000,
  "soKhoaHocToiThieu": 1,
  "ngayBatDau": "2026-10-01",
  "ngayKetThuc": "2027-12-31"
}
```

**Lỗi thường gặp**
- `400` — `loaiGiamGia` sai · `giaTriGiam <= 0` · **`Phần trăm` mà `giaTriGiam > 100`** · `ngayKetThuc < ngayBatDau`

> ⚠️ **Giới hạn đã biết:** ưu đãi có `soKhoaHocToiThieu > 1` **tạo được** nhưng **không áp dụng được** khi đăng ký
> (schema không có khóa nhóm đăng ký). Xem mục 7.

---

## 5. Lớp học — `/api/admin/classes`

| Method | URL |
|---|---|
| GET | `/api/admin/classes?maKhoaHoc=1&trangThai=&search=&page=1&pageSize=20` |
| GET | `/api/admin/classes/{id}` |
| GET | `/api/admin/classes/{id}/students` | *xem mục 7 — dùng `/enrollments?maLopHoc=`* |
| POST | `/api/admin/classes` |
| PATCH | `/api/admin/classes/{id}` |
| PATCH | `/api/admin/classes/{id}/status` |
| DELETE | `/api/admin/classes/{id}` |

**Tạo mới:**
```json
{
  "maLop": "IELTS-F01-2027",
  "tenLop": "IELTS Foundation 01 - 2027",
  "maKhoaHoc": 1,
  "maNhanVienHocVu": "NVHV001",
  "ngayBatDau": "2027-01-10",
  "ngayKetThucDuKien": "2027-04-20",
  "siSoToiDa": 20
}
```
→ `201`

**Trạng thái hợp lệ:** `Chuẩn bị mở` · `Đang học` · `Đã kết thúc` · `Đã hủy`
(lớp mới tạo mặc định `Chuẩn bị mở`)

**Sửa:**
```json
{
  "siSoToiDa": 25,
  "ngayKetThucThucTe": "2027-04-18"
}
```

**Lỗi thường gặp**
- `409` — mã lớp đã tồn tại · xóa lớp đã có đăng ký/phân công/lịch/ca học/thông báo
- `400` — khóa học hoặc nhân viên không tồn tại · `siSoToiDa <= 0` · ngày sai

> Ngừng hoạt động lớp **KHÔNG** xóa dữ liệu con. `DELETE` chỉ dành cho lớp tạo nhầm, chưa dùng.

---

## 6. Phân công giảng dạy — `/api/admin/teaching-assignments`

| Method | URL |
|---|---|
| GET | `/api/admin/teaching-assignments?maLopHoc=1&maGiaoVien=GV001&trangThai=&page=1&pageSize=20` |
| GET | `/api/admin/teaching-assignments/{id}` |
| POST | `/api/admin/teaching-assignments` |
| PATCH | `/api/admin/teaching-assignments/{id}` |
| PATCH | `/api/admin/teaching-assignments/{id}/status` |

**Tạo mới — giáo viên chính:**
```json
{
  "maLopHoc": 1,
  "maGiaoVien": "GV001",
  "vaiTroPhanCong": "Giáo viên chính",
  "ngayBatDau": "2027-01-10",
  "ngayKetThuc": null,
  "ghiChu": "Phụ trách chính"
}
```

**Tạo mới — trợ giảng:**
```json
{
  "maLopHoc": 1,
  "maGiaoVien": "TG001",
  "vaiTroPhanCong": "Trợ giảng",
  "ngayBatDau": "2027-01-10"
}
```

**Vai trò hợp lệ (CHECK constraint):** `Giáo viên chính` · `Giáo viên thay thế` · `Trợ giảng`

| Vai trò | Hồ sơ yêu cầu |
|---|---|
| `Giáo viên chính`, `Giáo viên thay thế` | `GiaoVien.LoaiGiaoVien = "Giáo viên"` |
| `Trợ giảng` | `GiaoVien.LoaiGiaoVien = "Trợ giảng"` |

**Lỗi thường gặp**
- `400` — vai trò sai · giáo viên không tồn tại · **vai trò không khớp loại hồ sơ** · `ngayKetThuc < ngayBatDau`
- `409` — đã có phân công cùng lớp + cùng người + cùng vai trò với **khoảng ngày chồng lấn**

> ⚠️ **Giới hạn:** chỉ phát hiện trùng trong **cùng lớp**. Không kiểm tra xung đột giờ/phòng giữa 2 lớp khác nhau
> (bảng `LichHoc`/`CaHoc` chưa triển khai — Phase 5).

---

## 7. Đăng ký học & xếp lớp — `/api/admin/enrollments`

| Method | URL | Mô tả |
|---|---|---|
| GET | `/api/admin/enrollments` | `?search=&trangThai=&maKhoaHoc=&maLopHoc=&maHocVien=&page=&pageSize=` |
| GET | `/api/admin/enrollments/{id}` | Chi tiết |
| POST | `/api/admin/enrollments` | Tạo đăng ký |
| PATCH | `/api/admin/enrollments/{id}` | Sửa ưu đãi / lịch / ghi chú |
| PATCH | `/api/admin/enrollments/{id}/assign-class` | Xếp lớp |
| PATCH | `/api/admin/enrollments/{id}/status` | Đổi trạng thái |

### 7.1 Tạo đăng ký — chưa xếp lớp

```json
{
  "maHocVien": "HV001",
  "maKhoaHoc": 1,
  "maUuDai": null,
  "lichHocMongMuon": "Thứ 2, 4 buổi tối",
  "ghiChu": "Đăng ký qua tư vấn viên"
}
```
→ `201` — `maLopHoc = null`, `trangThaiDangKy = "Chờ xử lý"`

### 7.2 Tạo đăng ký + xếp lớp ngay

```json
{
  "maHocVien": "HV002",
  "maKhoaHoc": 1,
  "maLopHoc": 1,
  "maUuDai": 1
}
```
→ `201` — `trangThaiDangKy = "Đã xếp lớp"`, có `ngayXepLop`

### 7.3 Xếp lớp cho đăng ký đã có

```http
PATCH /api/admin/enrollments/{id}/assign-class
```
```json
{ "maLopHoc": 1 }
```
→ `200`

### 7.4 Đổi ưu đãi (server tính lại tiền giảm)

```json
{ "maUuDai": 2 }
```

### 7.5 Bỏ ưu đãi

```json
{ "boUuDai": true }
```

### 7.6 Đổi trạng thái

```json
{ "trangThai": "Đã hủy" }
```

**Trạng thái hợp lệ:** `Chờ xử lý` · `Đã xếp lớp` · `Đã hủy` · `Hoàn thành`
> `Đã xếp lớp` **không** set được qua endpoint `/status` — phải dùng `/assign-class`.

### 7.7 Học phí — server tự tính, client KHÔNG gửi

| Cột | Nguồn |
|---|---|
| `hocPhiGoc` | `KhoaHoc.HocPhiCoBan` |
| `soTienGiam` | Tính từ `UuDai` (phần trăm hoặc số tiền) |
| `hocPhiSauGiam` | **Cột computed PERSISTED** — SQL Server tự tính `HocPhiGoc - SoTienGiam` |

Gửi `hocPhiGoc`, `soTienGiam`, `hocPhiSauGiam` trong body → **bị bỏ qua hoàn toàn**.

### 7.8 Lỗi thường gặp

| Mã | Nguyên nhân |
|---|---|
| `400` | Học viên không tồn tại / đã nghỉ · khóa học không nhận đăng ký · ưu đãi hết hạn hoặc sai · lớp khác khóa học · **đăng nhập bằng tài khoản không có hồ sơ nhân viên** |
| `409` | **Đã có đăng ký còn hiệu lực** cho cùng học viên + khóa học · **lớp đã đủ sĩ số** |
| `404` | Không tìm thấy đăng ký hoặc lớp |

### 7.9 Ưu đãi — giới hạn cần biết

Ưu đãi có `soKhoaHocToiThieu > 1` sẽ bị **từ chối áp dụng**:

```json
{
  "status": 400,
  "message": "Ưu đãi không hợp lệ.",
  "errors": [
    "Ưu đãi 'X' yêu cầu từ 2 khóa học trở lên. Hiện chưa hỗ trợ: schema không có khóa nhóm đăng ký nên không xác định được các đăng ký thuộc cùng một giao dịch."
  ]
}
```

**Lý do:** bảng `DangKyHoc` lưu **mỗi khóa học trên một dòng**, không có cột nhóm/giao dịch
→ không thể biết 2 đăng ký có thuộc cùng một lần mua hay không.

---

## 8. Xem học viên của một lớp

Dùng endpoint danh sách đăng ký với bộ lọc lớp:

```http
GET /api/admin/enrollments?maLopHoc=1&pageSize=100
```

Mỗi item trả `maHocVien`, `tenHocVien`, `trangThaiDangKy`, `ngayXepLop`.

---

## 9. Ghi chú kiến trúc

- Controller **không** chứa logic — chỉ gọi service và `ToActionResult()`.
- Nghiệp vụ nằm ở `Application/*Service.cs`.
- Truy vấn SQL nằm ở `Infrastructure/Persistence/*Store.cs`.
- Validate thủ công (ngày, số, quan hệ) ở service; validate định dạng ở DTO (DataAnnotations).
- `HocPhiSauGiam` có `private set` ở Entity + `HasComputedColumnSql(stored: true)` → không thể ghi.
- Xếp lớp dùng `SELECT ... WITH (UPDLOCK, HOLDLOCK)` trên dòng `LopHoc` trong transaction
  → nhiều request đồng thời **không thể** vượt sĩ số.
