# Phase 6 — Học phí, Thanh toán & Biên nhận — API Reference (Postman)

Base URL: `http://localhost:5204`

```
Authorization: Bearer <accessToken>
Content-Type: application/json
```

---

## 0. Ma trận phân quyền

| Endpoint | Quản trị viên | NV kế toán | NV tư vấn | Học viên |
|---|:--:|:--:|:--:|:--:|
| `GET /api/admin/tuition-fees*` | ✅ | ✅ | ✅ (chỉ đọc) | ❌ |
| `POST/PATCH /api/admin/tuition-fees*` | ✅ | ✅ | ❌ | ❌ |
| `POST /api/admin/tuition-fees/{id}/payments` | ✅ | ✅ | ❌ | ❌ |
| `GET /api/admin/payments*` | ✅ | ✅ | ✅ | ❌ |
| `GET /api/admin/receipts*` | ✅ | ✅ | ✅ | ❌ |
| `GET /api/students/me/tuition-fees*` | — | — | — | ✅ (chỉ của mình) |
| `GET /api/students/me/payments` | — | — | — | ✅ (chỉ của mình) |
| `GET /api/students/me/receipts*` | — | — | — | ✅ (chỉ của mình) |

> **Học viên KHÔNG có** bất kỳ endpoint ghi nào. Việc ghi nhận thanh toán chỉ do
> Quản trị viên / Nhân viên kế toán thực hiện.

---

## 1. Học phí — `/api/admin/tuition-fees`

### `GET /api/admin/tuition-fees`

Query: `maHocVien`, `maKhoaHoc`, `maLopHoc`, `trangThai`, `quaHan`, `tuNgay`, `denNgay`, `search`, `page`, `pageSize`

```
GET http://localhost:5204/api/admin/tuition-fees?trangThai=Thanh toán một phần&page=1&pageSize=20
GET http://localhost:5204/api/admin/tuition-fees?quaHan=true
GET http://localhost:5204/api/admin/tuition-fees?search=HV00
```

`trangThai` chỉ nhận: `"Chưa thanh toán"`, `"Thanh toán một phần"`, `"Đã thanh toán"`.

Response item:

```json
{
  "maHocPhi": 2,
  "maDangKy": 2,
  "maHocVien": "HV002",
  "hoTenHocVien": "Trần Quang Huy",
  "maKhoaHoc": 2,
  "tenKhoaHoc": "English Communication Basic K01",
  "maLopHoc": 2,
  "maLop": "ECB-A",
  "tenLop": "Lớp ECB A",
  "soTienGoc": 4500000,
  "soTienGiam": 0,
  "soTienPhaiDong": 4500000,
  "soTienDaDong": 2500000,
  "soTienConThieu": 2000000,
  "hanThanhToan": "2026-09-20",
  "trangThaiHocPhi": "Thanh toán một phần",
  "ngayTao": "2026-09-06T14:35:00",
  "soGiaoDich": 1,
  "quaHan": true,
  "conNo": true
}
```

> `soTienPhaiDong` và `soTienConThieu` là **cột computed PERSISTED** trong SQL Server.
> `quaHan` và `conNo` là giá trị **suy ra**, không lưu trong database.

### `GET /api/admin/tuition-fees/{id}`

Thêm `soDienThoaiHocVien`, `emailHocVien`, `trangThaiDangKy`.

### `POST /api/admin/tuition-fees` — tạo nghĩa vụ học phí

```json
{
  "maDangKy": 12,
  "hanThanhToan": "2026-11-30"
}
```

- Chỉ nhận `maDangKy`. **Không** nhận số tiền — server copy từ
  `DangKyHoc.HocPhiGoc` và `DangKyHoc.SoTienGiam`.
- Trạng thái khởi tạo: `"Chưa thanh toán"` (hoặc `"Đã thanh toán"` nếu học phí = 0).
- `201` khi thành công.
- `400` nếu đăng ký không tồn tại hoặc đang `"Đã hủy"`.
- `409` nếu đăng ký **đã có** nghĩa vụ học phí (`HocPhi.MaDangKy` là UNIQUE).

### `PATCH /api/admin/tuition-fees/{id}`

```json
{ "hanThanhToan": "2026-12-31" }
```

```json
{ "xoaHanThanhToan": true }
```

```json
{ "dongBoTuDangKy": true }
```

- `dongBoTuDangKy`: copy lại `soTienGoc`/`soTienGiam` từ `DangKyHoc`
  (dùng khi ưu đãi của đăng ký bị sửa sau khi đã tạo học phí).
- **`409`** nếu học phí **đã có thanh toán** — không cho đổi số tiền nữa.
- Không thể gửi đồng thời `hanThanhToan` và `xoaHanThanhToan`.

### `GET /api/admin/tuition-fees/{id}/payments`

Query: `trangThai`, `loaiGiaoDich`, `tuNgay`, `denNgay`, `page`, `pageSize`.

### `GET /api/admin/tuition-fees/reports/summary`

Query: `tuNgay`, `denNgay` (lọc theo `HocPhi.NgayTao`), `maKhoaHoc`.

```json
{
  "tongSoHocPhi": 4,
  "tongPhaiThu": 14400000,
  "tongDaThu": 12700000,
  "tongConThieu": 1700000,
  "soChuaThanhToan": 1,
  "soThanhToanMotPhan": 1,
  "soDaThanhToan": 2,
  "soQuaHan": 1,
  "soHocVienConNo": 2,
  "danhSachConNo": [
    {
      "maHocVien": "HV002",
      "hoTen": "Trần Quang Huy",
      "soHocPhiConNo": 1,
      "tongConThieu": 2000000
    }
  ],
  "thongKeGiaoDichTheoPhuongThuc": [
    { "nhan": "Chuyển khoản", "soLuong": 2, "tongTien": 10200000 },
    { "nhan": "Tiền mặt", "soLuong": 1, "tongTien": 2500000 }
  ],
  "thongKeGiaoDichTheoTrangThai": [
    { "nhan": "Thành công", "soLuong": 3, "tongTien": 12700000 }
  ]
}
```

Nguồn số liệu:
- `tongPhaiThu` = Σ `HocPhi.SoTienPhaiDong` (computed, nguồn chuẩn)
- `tongDaThu` = Σ `HocPhi.SoTienDaDong`
- `tongConThieu` = Σ `HocPhi.SoTienConThieu` (computed)
- Thống kê giao dịch **chỉ tính** `TrangThai = 'Thành công'` và `LoaiGiaoDich = 'Thu tiền'`
  (riêng bảng theo trạng thái thì liệt kê tất cả để thấy cả giao dịch bị hủy).

---

## 2. Ghi nhận thanh toán

### `POST /api/admin/tuition-fees/{id}/payments`

```json
{
  "maGiaoDich": "GD20261018001",
  "soTien": 2000000,
  "phuongThucThanhToan": "Chuyển khoản",
  "loaiGiaoDich": "Thu tiền",
  "ghiChu": "Đóng đợt 2",
  "noiDungBienNhan": "Thu học phí đợt 2 - English Communication Basic K01"
}
```

**Bắt buộc:** `maGiaoDich` (3–150 ký tự), `soTien` > 0, `phuongThucThanhToan`.

**Server tự quyết định** (client gửi lên sẽ bị bỏ qua):
`maNhanVienKeToan` (từ JWT), `ngayThanhToan` (giờ server), `trangThai` (`"Thành công"`),
`soTienDaDong`, `soTienConThieu`, `trangThaiHocPhi`.

Kết quả:

| Tình huống | HTTP |
|---|---|
| Ghi nhận thành công | **201** + `paymentDto` |
| Gửi lại **cùng** `maGiaoDich` cho **cùng** học phí | **200** + giao dịch đã có (không tạo mới) |
| `maGiaoDich` đã dùng cho **học phí khác** | **409** |
| `soTien` ≤ 0 | **400** |
| `soTien` > số còn thiếu | **409** (`"Số tiền vượt quá số dư còn thiếu (…)"`) |
| Học phí không tồn tại | **404** |
| `loaiGiaoDich` ≠ `"Thu tiền"` | **400** (Phase 6 chưa hỗ trợ hoàn tiền/điều chỉnh) |
| Tài khoản chưa có hồ sơ `NhanVien` | **400** |
| Không đủ quyền (Học viên / Giáo viên) | **403** |

Response:

```json
{
  "maThanhToan": 4,
  "maHocPhi": 2,
  "maDangKy": 2,
  "maHocVien": "HV002",
  "hoTenHocVien": "Trần Quang Huy",
  "maKhoaHoc": 2,
  "tenKhoaHoc": "English Communication Basic K01",
  "soTien": 2000000,
  "ngayThanhToan": "2026-10-18T09:12:33.512",
  "phuongThucThanhToan": "Chuyển khoản",
  "maGiaoDich": "GD20261018001",
  "loaiGiaoDich": "Thu tiền",
  "trangThai": "Thành công",
  "ghiChu": "Đóng đợt 2",
  "maNhanVienKeToan": "NVKT001",
  "tenNhanVienKeToan": "Lê Ngọc Mai",
  "bienNhan": {
    "maBienNhan": 4,
    "soBienNhan": "BN20261018001",
    "ngayLap": "2026-10-18T09:12:33.512",
    "trangThai": "Đã phát hành"
  }
}
```

### Cơ chế chống trùng & chống sai số dư

1. `maGiaoDich` được bảo vệ bởi unique index có điều kiện `UX_ThanhToan_MaGiaoDich`.
2. Trong transaction, dòng `HocPhi` được khoá
   (`SELECT * FROM HocPhi WITH (UPDLOCK, HOLDLOCK) WHERE MaHocPhi = @id`)
   ⇒ nhiều request đồng thời trên cùng học phí xử lý tuần tự.
3. `SoTienDaDong` cộng dồn và `TrangThaiHocPhi` cập nhật **trong cùng transaction**
   với `INSERT ThanhToan` + `INSERT BienNhan`.
4. DB có `CK_HocPhi_SoTien` chặn `SoTienDaDong > SoTienGoc - SoTienGiam`.

---

## 3. Giao dịch — `/api/admin/payments`

### `GET /api/admin/payments`

Query: `maHocPhi`, `maHocVien`, `maKhoaHoc`, `phuongThucThanhToan`, `trangThai`, `loaiGiaoDich`, `tuNgay`, `denNgay`, `page`, `pageSize`

```
GET http://localhost:5204/api/admin/payments?maHocVien=HV002&tuNgay=2026-09-01&denNgay=2026-12-31
```

### `GET /api/admin/payments/{id}`

---

## 4. Biên nhận — `/api/admin/receipts`

Biên nhận **chỉ đọc** — được sinh tự động trong transaction thanh toán.
`BienNhan.MaThanhToan` là UNIQUE ⇒ không thể có 2 biên nhận cho 1 giao dịch.

### `GET /api/admin/receipts`

Query: `soBienNhan`, `maHocVien`, `maThanhToan`, `trangThai`, `tuNgay`, `denNgay`, `page`, `pageSize`

### `GET /api/admin/receipts/{id}`

### `GET /api/admin/receipts/by-number/{soBienNhan}`

```
GET http://localhost:5204/api/admin/receipts/by-number/BN20261018001
```

Response:

```json
{
  "maBienNhan": 4,
  "soBienNhan": "BN20261018001",
  "ngayLap": "2026-10-18T09:12:33.512",
  "soTienXacNhan": 2000000,
  "noiDung": "Thu học phí đợt 2 - English Communication Basic K01",
  "trangThai": "Đã phát hành",
  "maNhanVienLap": "NVKT001",
  "tenNhanVienLap": "Lê Ngọc Mai",
  "maThanhToan": 4,
  "maGiaoDich": "GD20261018001",
  "phuongThucThanhToan": "Chuyển khoản",
  "trangThaiThanhToan": "Thành công",
  "ngayThanhToan": "2026-10-18T09:12:33.512",
  "maHocPhi": 2,
  "maDangKy": 2,
  "maHocVien": "HV002",
  "hoTenHocVien": "Trần Quang Huy",
  "maKhoaHoc": 2,
  "tenKhoaHoc": "English Communication Basic K01"
}
```

### Quy tắc sinh số biên nhận

`BN` + `yyyyMMdd` + 3 chữ số, ví dụ `BN20261018001`.

- Đọc bản ghi lớn nhất **theo tiền tố ngày** (KHÔNG dùng `MAX(MaBienNhan) + 1`).
- UNIQUE index trên `SoBienNhan` chống trùng; nếu đụng độ thì transaction được thử lại
  tối đa 5 lần.

---

## 5. API tự phục vụ của Học viên — `/api/students/me`

Danh tính lấy từ JWT (`unique_name` = `MaHocVien`). **Không** có tham số mã học viên.

| Endpoint | Mô tả |
|---|---|
| `GET /api/students/me/tuition-fees` | Danh sách học phí của mình |
| `GET /api/students/me/tuition-fees/{id}` | Chi tiết — không thuộc sở hữu ⇒ **404** |
| `GET /api/students/me/payments` | Giao dịch của mình |
| `GET /api/students/me/receipts` | Biên nhận của mình |
| `GET /api/students/me/receipts/{id}` | Chi tiết — không thuộc sở hữu ⇒ **404** |

> Trả **404** (không phải 403) để không làm lộ sự tồn tại của bản ghi người khác.

---

## 6. Thứ tự test gợi ý trên Postman

1. Login `ADMIN001` / `LanguageCenter@2026` → lưu token.
2. `GET /api/admin/enrollments` → lấy một `maDangKy` **chưa có học phí**.
3. `POST /api/admin/tuition-fees` `{ "maDangKy": <id> }` → 201.
4. `GET /api/admin/tuition-fees` → thấy bản ghi mới, `trangThaiHocPhi = "Chưa thanh toán"`.
5. `POST /api/admin/tuition-fees/{id}/payments` với `soTien` = một phần → 201, có `bienNhan`.
6. Gửi lại **y hệt** request trên → **200**, `maThanhToan` không đổi.
7. Đổi `maGiaoDich` mới, `soTien` lớn hơn số còn thiếu → **409**.
8. `soTien = 0` → **400**.
9. `GET /api/admin/tuition-fees/{id}` → `soTienDaDong`, `trangThaiHocPhi` đúng.
10. `GET /api/admin/receipts` → biên nhận tương ứng.
11. `GET /api/admin/tuition-fees/reports/summary` → số liệu khớp.
12. Login tài khoản học viên (ví dụ `HV002`) →
    `GET /api/students/me/tuition-fees` chỉ thấy dữ liệu của mình.
13. Dùng token học viên gọi `POST /api/admin/tuition-fees/1/payments` → **403**.
14. Dùng token học viên gọi `GET /api/students/me/tuition-fees/<id của người khác>` → **404**.

---

## 7. Giới hạn đã biết

| # | Giới hạn | Ảnh hưởng |
|---|---|---|
| 1 | `HocPhi.MaDangKy` UNIQUE | Mỗi đăng ký chỉ 1 nghĩa vụ học phí, không chia nhiều đợt riêng |
| 2 | `ThanhToan` không có cột lý do hủy / người duyệt | Không hỗ trợ hoàn tiền, điều chỉnh ở Phase 6 |
| 3 | `BienNhan.MaThanhToan` UNIQUE | 1 giao dịch = tối đa 1 biên nhận |
| 4 | Không có cột liên kết cổng thanh toán | Chỉ ghi nhận thủ công, không tích hợp online |
| 5 | `TrangThaiHocPhi`, `ThanhToan.TrangThai`, `TrangThai` biên nhận **không có CHECK** | Constants ở Domain là hàng rào duy nhất |
| 6 | `MaNhanVienKeToan` NOT NULL + FK `NhanVien` | Tài khoản chưa có hồ sơ nhân viên sẽ bị từ chối **400** |
| 7 | "Quá hạn" không có cột riêng | Tính động từ `HanThanhToan` + `SoTienConThieu` |
| 8 | Sửa ưu đãi của đăng ký sau khi đã đóng tiền | Bị chặn **409**, cần quy trình kế toán |
| 9 | Không có chức năng in/xuất PDF | Đề xuất riêng nếu cần |
