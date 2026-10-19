# Phase 5 — Schedule, Attendance & Grading Management — API Reference (Postman)

Base URL: `http://localhost:5204`

Tất cả endpoint bên dưới đều cần header:

```
Authorization: Bearer <accessToken>
Content-Type: application/json
```

> Lấy token bằng `POST /api/auth/login` với `{ "tenDangNhap": "ADMIN001", "matKhau": "LanguageCenter@2026" }`.

---

## 0. Ma trận phân quyền

| Nhóm endpoint | Quản trị viên | NV học vụ | Giáo viên | Trợ giảng | Học viên |
|---|:--:|:--:|:--:|:--:|:--:|
| `/api/admin/rooms` | ✅ | ✅ | ❌ | ❌ | ❌ |
| `/api/admin/schedules` | ✅ | ✅ | ❌ | ❌ | ❌ |
| `/api/admin/sessions` (CRUD) | ✅ | ✅ | ❌ | ❌ | ❌ |
| `/api/admin/sessions/{id}/attendance` | ✅ | ✅ | ✅ (lớp mình) | ✅ (lớp mình) | ❌ |
| `/api/admin/attendance/{id}` | ✅ | ✅ | ✅ (lớp mình) | ✅ (lớp mình) | ❌ |
| `/api/admin/grade-types` | ✅ | ❌ | ❌ | ❌ | ❌ |
| `/api/admin/grades` (ghi) | ✅ (gửi `maGiaoVien`) | ❌ | ✅ (lớp mình) | ❌ | ❌ |
| `/api/students/me/*` | — | — | — | — | ✅ (chỉ của mình) |
| `/api/teachers/me/schedule` | — | — | ✅ | ✅ | — |

Giáo viên/Trợ giảng được coi là "lớp mình" khi:
- là `maGiaoVienThucTe` / `maTroGiangThucTe` của ca học, **hoặc**
- có `PhanCongGiangDay` đang `Đang phân công` cho lớp đó.

---

## 1. Phòng học (chỉ đọc)

### `GET /api/admin/rooms`

Query: `search`, `trangThai`, `page`, `pageSize`

```
GET http://localhost:5204/api/admin/rooms?search=A1&trangThai=Hoạt động&page=1&pageSize=20
```

### `GET /api/admin/rooms/{id}`

```
GET http://localhost:5204/api/admin/rooms/1
```

---

## 2. Lịch học định kỳ — `LichHoc`

### `GET /api/admin/schedules`

Query: `maLopHoc`, `maPhongHoc`, `thuTrongTuan` (1–7), `trangThai`, `page`, `pageSize`

```
GET http://localhost:5204/api/admin/schedules?maLopHoc=1&thuTrongTuan=2
```

### `GET /api/admin/schedules/{id}`

### `POST /api/admin/schedules`

```json
{
  "maLopHoc": 1,
  "maPhongHoc": 1,
  "thuTrongTuan": 2,
  "gioBatDau": "18:00:00",
  "gioKetThuc": "19:30:00",
  "ngayApDungTu": "2026-01-05",
  "ngayApDungDen": "2026-03-30"
}
```

`thuTrongTuan`: 1 = Thứ Hai … 7 = Chủ Nhật. `ngayApDungDen` = `null` nghĩa là không giới hạn.

Trả về:
- `201` + `scheduleDetailDto` khi thành công
- `400` khi lớp/phòng không tồn tại, giờ/ngày sai
- `409` khi **trùng phòng** (cùng phòng + cùng thứ + giờ chồng lấn + khoảng ngày chồng lấn)

### `PATCH /api/admin/schedules/{id}`

Mọi field optional. **Không đổi được `maLopHoc`.**

```json
{
  "maPhongHoc": 2,
  "gioBatDau": "17:45:00",
  "gioKetThuc": "19:15:00"
}
```

### `PATCH /api/admin/schedules/{id}/status`

```json
{ "trangThai": "Ngừng áp dụng" }
```

Chỉ chấp nhận `"Hoạt động"` hoặc `"Ngừng áp dụng"`.

### `DELETE /api/admin/schedules/{id}`

- `204` khi xóa được
- `409` khi **đã có ca học thực tế tham chiếu** → hãy đổi trạng thái sang `"Ngừng áp dụng"`

---

## 3. Ca học thực tế — `CaHoc`

### `GET /api/admin/sessions`

Query: `maLopHoc`, `maPhongHoc`, `maGiaoVien`, `trangThai`, `tuNgay`, `denNgay`, `page`, `pageSize`

```
GET http://localhost:5204/api/admin/sessions?maLopHoc=1&tuNgay=2026-01-01&denNgay=2026-01-31
```

### `GET /api/admin/sessions/{id}`

### `POST /api/admin/sessions`

```json
{
  "maLopHoc": 1,
  "maLichHoc": 1,
  "maPhongHoc": 1,
  "maGiaoVienThucTe": "GV001",
  "maTroGiangThucTe": "TG001",
  "ngayHoc": "2026-01-06",
  "gioBatDau": "18:00:00",
  "gioKetThuc": "19:30:00",
  "noiDungBaiHoc": "Unit 3 - Present Perfect",
  "lyDoThayDoi": null
}
```

- `maLichHoc` = `null` nếu là ca phát sinh ngoài lịch định kỳ.
- `maGiaoVienThucTe` bắt buộc là người có `LoaiGiaoVien = "Giáo viên"`.
- `maTroGiangThucTe` bắt buộc là người có `LoaiGiaoVien = "Trợ giảng"`.
- Hai người không được trùng nhau.

Trả về:
- `201` + `sessionDetailDto`
- `400` khi lớp/phòng/giáo viên sai, giờ sai
- `409` khi trùng phòng, trùng giáo viên/trợ giảng, hoặc lớp đã có ca trong cùng ngày

### `PATCH /api/admin/sessions/{id}`

Mọi field optional (trừ `maLopHoc`, `maLichHoc` không có trong body).

```json
{
  "maPhongHoc": 3,
  "gioBatDau": "19:00:00",
  "gioKetThuc": "20:30:00",
  "lyDoThayDoi": "Đổi phòng do hỏng điều hòa"
}
```

Gửi `"maGiaoVienThucTe": ""` để **bỏ trống** người dạy thực tế.

### `PATCH /api/admin/sessions/{id}/status`

```json
{ "trangThai": "Đang diễn ra" }
```

Chỉ chấp nhận: `"Chưa diễn ra"`, `"Đang diễn ra"`, `"Đã hoàn thành"`, `"Đã hủy"`.

### `DELETE /api/admin/sessions/{id}`

- `204` khi xóa được
- `409` khi **đã có bản ghi điểm danh** → hãy chuyển trạng thái sang `"Đã hủy"`

### `POST /api/admin/sessions/generate` — tạo hàng loạt từ lịch định kỳ

```json
{
  "maLichHoc": 1,
  "tuNgay": "2026-01-01",
  "denNgay": "2026-01-31"
}
```

Idempotent. Khoảng ngày được giao với `[ngayApDungTu, ngayApDungDen]` của lịch.

Response:

```json
{
  "maLichHoc": 1,
  "tuNgay": "2026-01-01",
  "denNgay": "2026-01-31",
  "apDungTuThucTe": "2026-01-05",
  "apDungDenThucTe": "2026-01-31",
  "tongNgayKhop": 4,
  "ngayTao": ["2026-01-06", "2026-01-13", "2026-01-20", "2026-01-27"],
  "ngayDaCo": [],
  "ngayXungDot": [
    { "ngayHoc": "2026-01-20", "lyDo": "Phòng học đã có ca khác trùng giờ." }
  ]
}
```

Ghi chú:
- Ca học được tạo **không gán sẵn giáo viên** (`maGiaoVienThucTe = null`) — gán sau bằng `PATCH /api/admin/sessions/{id}`.
- Gọi lại cùng tham số sẽ không tạo trùng: các ngày đã có nằm trong `ngayDaCo`.
- Giới hạn 366 ngày/lần.

---

## 4. Điểm danh — `DiemDanh`

### `GET /api/admin/sessions/{id}/attendance`

Trả về **bảng điểm danh**: tất cả học viên đang học lớp đó (kể cả chưa điểm danh).

```json
{
  "maCaHoc": 1,
  "maLop": "LOP2026A",
  "tenLop": "Lớp A1 - Tối 2-4",
  "maPhong": "A1",
  "tenPhong": "Phòng A1",
  "ngayHoc": "2026-01-06",
  "gioBatDau": "18:00:00",
  "gioKetThuc": "19:30:00",
  "trangThaiCaHoc": "Chưa diễn ra",
  "coTheDiemDanh": true,
  "lyDoKhongTheDiemDanh": null,
  "hocViens": [
    {
      "maHocVien": "HV001",
      "hoTen": "Nguyễn Văn A",
      "maDangKy": 12,
      "daDiemDanh": false,
      "maDiemDanh": null,
      "trangThaiDiemDanh": null,
      "ghiChu": null,
      "thoiGianDiemDanh": null,
      "tenTaiKhoanThucHien": null
    }
  ]
}
```

`coTheDiemDanh = false` khi ca đã `Đã hủy`/`Đã hoàn thành`, hoặc người gọi không có quyền trên lớp đó.

### `POST /api/admin/sessions/{id}/attendance` — ghi hàng loạt (upsert)

```json
{
  "items": [
    { "maHocVien": "HV001", "trangThaiDiemDanh": "Có mặt", "ghiChu": null },
    { "maHocVien": "HV002", "trangThaiDiemDanh": "Vắng có phép", "ghiChu": "Ốm có đơn" },
    { "maHocVien": "HV003", "trangThaiDiemDanh": "Đi trễ", "ghiChu": "Trễ 10 phút" }
  ]
}
```

Trạng thái hợp lệ: `"Có mặt"`, `"Vắng"`, `"Vắng có phép"`, `"Đi trễ"`.

**All-or-nothing**: nếu bất kỳ dòng nào sai (học viên không thuộc lớp, trạng thái sai, trùng mã trong payload), **toàn bộ** yêu cầu bị từ chối `400` kèm danh sách lỗi trong `errors`.

Upsert theo khoá `(MaCaHoc, MaHocVien)` → gọi lại nhiều lần chỉ cập nhật, không nhân bản.

### `PATCH /api/admin/attendance/{maDiemDanh}`

```json
{ "trangThaiDiemDanh": "Vắng có phép", "ghiChu": "Bổ sung đơn xin phép" }
```

- `403` khi người gọi không có quyền trên lớp của bản ghi này
- `404` khi không tìm thấy

### `GET /api/admin/students/{maHocVien}/attendance`

Query: `tuNgay`, `denNgay`

```json
{
  "maHocVien": "HV001",
  "hoTen": "Nguyễn Văn A",
  "tongCaHoc": 12,
  "coMat": 10,
  "vang": 1,
  "vangCoPhep": 1,
  "diTre": 0,
  "tiLeCoMat": 83.33,
  "items": [ { "maDiemDanh": 1, "maCaHoc": 1, "maLop": "LOP2026A", "ngayHoc": "2026-01-06", "trangThaiDiemDanh": "Có mặt" } ]
}
```

---

## 5. Đầu điểm — `LoaiDiem` (chỉ Quản trị viên)

### `GET /api/admin/grade-types`

Query: `maKhoaHoc`, `trangThai`, `search`, `page`, `pageSize`

### `GET /api/admin/grade-types/{id}`

### `POST /api/admin/grade-types`

```json
{
  "maKhoaHoc": 1,
  "tenLoaiDiem": "Giữa kỳ",
  "trongSo": 30,
  "diemToiDa": 10,
  "thuTu": 1
}
```

Ràng buộc:
- `tenLoaiDiem` 2–150 ký tự, **không trùng trong cùng khóa học**
- `thuTu` > 0, **không trùng trong cùng khóa học**
- `trongSo` trong `0..100`
- `diemToiDa` > 0

### `PATCH /api/admin/grade-types/{id}`

```json
{ "trongSo": 40, "diemToiDa": 10 }
```

> ⚠️ `LoaiDiem` có đồng thời `UNIQUE(MaKhoaHoc, TenLoaiDiem)` và `UNIQUE(MaKhoaHoc, ThuTu)` — đổi `thuTu` rất dễ nhận `409`. Hãy đổi theo thứ tự tránh trùng.

### `PATCH /api/admin/grade-types/{id}/status`

```json
{ "trangThai": "Ngừng sử dụng" }
```

### `DELETE /api/admin/grade-types/{id}`

- `409` khi đã có điểm số tham chiếu → chuyển sang `"Ngừng sử dụng"`.

---

## 6. Điểm số — `DiemSo`

### `GET /api/admin/grades`

Query: `maDangKy`, `maLoaiDiem`, `maKhoaHoc`, `maLopHoc`, `maHocVien`, `maGiaoVien`, `page`, `pageSize`

### `GET /api/admin/grades/summary?maLopHoc=1`

Bảng điểm tổng kết của cả lớp. **CHỈ TÍNH — KHÔNG ghi vào database.**

```json
{
  "maLopHoc": 1,
  "maLop": "LOP2026A",
  "tenLop": "Lớp A1 - Tối 2-4",
  "maKhoaHoc": 1,
  "tenKhoaHoc": "Tiếng Anh A1",
  "tongTrongSo": 100,
  "canhBaoTrongSo": false,
  "canhBao": null,
  "hocViens": [
    {
      "maDangKy": 12,
      "maHocVien": "HV001",
      "hoTen": "Nguyễn Văn A",
      "duDieuKien": true,
      "diemTongKet": 8.25,
      "thanhPhan": [
        { "maLoaiDiem": 1, "tenLoaiDiem": "Giữa kỳ", "trongSo": 30, "diemToiDa": 10, "diem": 8.0 },
        { "maLoaiDiem": 2, "tenLoaiDiem": "Cuối kỳ", "trongSo": 70, "diemToiDa": 10, "diem": 8.5 }
      ],
      "thanhPhanConThieu": []
    }
  ]
}
```

**Công thức:**

$$n_i = \frac{\text{Điểm}_i}{\text{ĐiểmTốiĐa}_i} \qquad \text{TổngKết} = 10 \times \frac{\sum_i \left(n_i \times \text{TrọngSố}_i\right)}{\sum_i \text{TrọngSố}_i}$$

- Chỉ tính khi **đã có đủ điểm của mọi đầu điểm đang hoạt động**; nếu thiếu → `diemTongKet = null` và `thanhPhanConThieu` liệt kê các đầu điểm còn thiếu.
- `canhBaoTrongSo = true` khi `Σ TrọngSố ≠ 100` (công thức vẫn chạy theo tỉ lệ thực tế).
- Không có quy tắc đạt/không đạt tự động.

### `GET /api/admin/grades/{id}`

### `POST /api/admin/grades`

Giáo viên (lấy mã từ JWT, `maGiaoVien` bị bỏ qua):

```json
{
  "maDangKy": 12,
  "maLoaiDiem": 1,
  "diem": 8.5,
  "nhanXet": "Nắm bài tốt",
  "trangThaiKetQua": "Đạt"
}
```

Quản trị viên **bắt buộc** gửi `maGiaoVien` (vì `DiemSo.MaGiaoVien` là NOT NULL + FK tới `GiaoVien`):

```json
{
  "maDangKy": 12,
  "maLoaiDiem": 1,
  "maGiaoVien": "GV001",
  "diem": 8.5,
  "nhanXet": null,
  "trangThaiKetQua": null
}
```

Kiểm tra:
- `maDangKy` tồn tại
- `maLoaiDiem` tồn tại, **cùng `maKhoaHoc` với đăng ký**, và đang `"Hoạt động"`
- `diem >= 0` **và** `diem <= diemToiDa` (DB không có CHECK nên chặn ở service)
- `trangThaiKetQua` nếu có phải là `"Đạt"` hoặc `"Không đạt"`
- **409** nếu cặp `(maDangKy, maLoaiDiem)` đã có điểm
- Giáo viên: phải được phân công lớp của đăng ký đó, nếu không → **403**
- Trợ giảng: **không có quyền** (chỉ điểm danh)

### `PATCH /api/admin/grades/{id}`

```json
{ "diem": 9.0, "nhanXet": "Cải thiện rõ rệt" }
```

Gửi `"nhanXet": ""` hoặc `"trangThaiKetQua": ""` để **xóa** giá trị.

### `DELETE /api/admin/grades/{id}`

---

## 7. API tự phục vụ

### `GET /api/students/me/attendance?tuNgay=&denNgay=`  (role `Học viên`)

Giống `GET /api/admin/students/{maHocVien}/attendance` nhưng **không có tham số mã** — danh tính lấy từ JWT.

### `GET /api/students/me/grades`  (role `Học viên`)

```json
[
  {
    "maDangKy": 12,
    "maLopHoc": 1,
    "maLop": "LOP2026A",
    "tenLop": "Lớp A1 - Tối 2-4",
    "tenKhoaHoc": "Tiếng Anh A1",
    "tongTrongSo": 100,
    "canhBaoTrongSo": false,
    "duDieuKien": true,
    "diemTongKet": 8.25,
    "thanhPhan": [
      { "maLoaiDiem": 1, "tenLoaiDiem": "Giữa kỳ", "trongSo": 30, "diemToiDa": 10, "diem": 8.0 }
    ],
    "thanhPhanConThieu": []
  }
]
```

### `GET /api/students/me/schedule?tuNgay=&denNgay=`  (role `Học viên`)

Danh sách ca học của các lớp học viên đang theo học.

### `GET /api/teachers/me/schedule?tuNgay=&denNgay=`  (role `Giáo viên`, `Trợ giảng`)

Ca học mà mình là người dạy thực tế, **hoặc** lớp mình đang được phân công.

---

## 8. Thứ tự test gợi ý trên Postman

1. `POST /api/auth/login` (ADMIN001) → lưu token.
2. `GET /api/admin/rooms` → lấy `maPhongHoc`.
3. `GET /api/admin/classes` → lấy `maLopHoc` đang `Đang học`.
4. `POST /api/admin/grade-types` → tạo `"Giữa kỳ"` (thuTu 1), `"Cuối kỳ"` (thuTu 2).
5. `POST /api/admin/schedules` → lịch thứ 2, 18:00–19:30.
6. `POST /api/admin/sessions/generate` → `tuNgay`/`denNgay` 1 tháng.
7. `GET /api/admin/sessions` → lấy `maCaHoc`.
8. `GET /api/admin/sessions/{id}/attendance` → xem bảng.
9. `POST /api/admin/sessions/{id}/attendance` → ghi điểm danh.
10. `GET /api/admin/enrollments?maLopHoc=...` → lấy `maDangKy`.
11. `POST /api/admin/grades` (admin, gửi `maGiaoVien`).
12. `GET /api/admin/grades/summary?maLopHoc=...` → xem điểm tổng kết.
13. Login bằng tài khoản học viên → `GET /api/students/me/grades`.

---

## 9. Giới hạn đã biết

| # | Giới hạn | Ảnh hưởng |
|---|---|---|
| 1 | `DiemSo.MaGiaoVien` NOT NULL + FK `GiaoVien` | Admin phải gửi `maGiaoVien` hợp lệ khi nhập điểm |
| 2 | Trợ giảng không nhập điểm được | Chỉ Quản trị viên và Giáo viên |
| 3 | DB không có `CHECK Diem <= DiemToiDa` | Chỉ chặn ở tầng service |
| 4 | `UNIQUE (MaDangKy, MaLoaiDiem)` | Mỗi đầu điểm chỉ 1 điểm/đăng ký, không có lịch sử sửa điểm |
| 5 | `LichHoc`, `CaHoc` không có UNIQUE | Chống trùng hoàn toàn bằng kiểm tra service |
| 6 | `LoaiDiem` có 2 UNIQUE | Đổi `thuTu` dễ 409 |
| 7 | 6/7 cột trạng thái không có CHECK | Constants là hàng rào duy nhất |
| 8 | Điểm tổng kết không có cột trong DB | Chỉ tính khi gọi `/summary` |
| 9 | `LichHoc` không lưu giáo viên | Xung đột giáo viên chỉ phát hiện ở `CaHoc` |
| 10 | `PhongHoc` chỉ đọc | Không có CRUD phòng trong Phase 5 |
| 11 | Ca học sinh từ `generate` không gán giáo viên | Phải `PATCH` để gán sau |
