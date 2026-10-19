# Phase 7 — Thông báo, Khảo sát, Báo cáo — API Reference (Postman)

Base URL: `http://localhost:5204` · Swagger: `http://localhost:5204/swagger`

```
Authorization: Bearer <accessToken>
Content-Type: application/json
```

> Login body: `{ "username": "...", "password": "..." }` → `accessToken`.

---

## 0. Giới hạn schema (đọc trước khi dùng)

| Hạng mục | Thực tế trong DB | Hệ quả |
|---|---|---|
| `ThongBao` | Không có cột `DaDoc` / `ThoiGianDoc`, không có bảng người nhận | **Không thể** đánh dấu đã đọc hay đếm "chưa đọc". Chỉ trả `tongSo`. |
| `ThongBao` | `MaTaiKhoanNhan` hoặc `MaLopHoc` (CHECK: ít nhất một) | Gửi trực tiếp **hoặc** cho cả lớp; hỗ trợ gửi cả hai. |
| `CauHoiKhaoSat` | Không có bảng lựa chọn (options) | Không có "phân bố theo lựa chọn"; chỉ có `GiaTriDanhGia` (0..5) và `NoiDungTraLoi`. |
| `PhanHoiKhaoSat` | `UNIQUE (MaKhaoSat, MaHocVien)`, `MaHocVien` FK | **Mỗi học viên chỉ trả lời 1 lần**; chỉ học viên mới trả lời được. |
| `KhaoSat.TrangThai` | Không CHECK | Constants là hàng rào duy nhất. |
| `KhaoSat` | Không có "đối tượng mục tiêu" | `tiLePhanHoi` chỉ là chỉ số tham khảo trên tổng số học viên. |

---

## 1. Ma trận phân quyền

| Endpoint | QTV | NV học vụ | NV kế toán | NV tư vấn | Giáo viên | Trợ giảng | Học viên |
|---|:--:|:--:|:--:|:--:|:--:|:--:|:--:|
| `GET /api/notifications*` | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ | ✅ |
| `GET /api/admin/notifications*` | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| `POST /api/admin/notifications` | ✅ | ✅ | ❌ | ❌ | ✅ lớp mình | ✅ lớp mình | ❌ |
| `PATCH/DELETE /api/admin/notifications/{id}` | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| `/api/admin/surveys*` | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| `/api/students/me/surveys*` | ❌ | ❌ | ❌ | ❌ | ❌ | ❌ | ✅ của mình |
| `GET /api/admin/reports/overview` | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |
| `GET /api/admin/reports/academic` | ✅ | ✅ | ❌ | ❌ | ✅ lớp mình | ✅ lớp mình | ❌ |
| `GET /api/admin/reports/attendance` | ✅ | ✅ | ❌ | ❌ | ✅ lớp mình | ✅ lớp mình | ❌ |
| `GET /api/admin/reports/surveys` | ✅ | ✅ | ❌ | ❌ | ❌ | ❌ | ❌ |

Giáo viên/Trợ giảng gửi thông báo: **bắt buộc** `maLopHoc` và phải được phân công lớp đó; **không** được gửi trực tiếp cho một tài khoản.

---

## 2. Thông báo của tôi — `/api/notifications` (mọi vai trò)

Phạm vi hiển thị:
- Học viên: gửi trực tiếp cho tôi **+** gửi cho lớp tôi đang học (`Đã xếp lớp` / `Hoàn thành`).
- Giáo viên/Trợ giảng: gửi trực tiếp **+** gửi cho lớp tôi được phân công.
- Vai trò khác: **chỉ** thông báo gửi trực tiếp.
- Thông báo `Đã thu hồi` luôn bị ẩn.

### `GET /api/notifications`

Query: `loaiThongBao`, `tuNgay`, `denNgay`, `search`, `page`, `pageSize`

```json
{
  "items": [
    {
      "maThongBao": 4,
      "maTaiKhoanGui": 3,
      "tenDangNhapNguoiGui": "GV001",
      "maTaiKhoanNhan": 8,
      "tenDangNhapNguoiNhan": "HV001",
      "maLopHoc": null,
      "maLop": null,
      "tenLop": null,
      "tieuDe": "Nhắc nộp bài tập",
      "noiDung": "Vui lòng hoàn thành bài Reading...",
      "loaiThongBao": "Học tập",
      "thoiGianGui": "2026-09-25T20:30:00",
      "trangThai": "Đã gửi",
      "phamVi": "Cá nhân"
    }
  ],
  "page": 1, "pageSize": 20, "totalItems": 1, "totalPages": 1
}
```

### `GET /api/notifications/summary`

```json
{
  "tongSo": 3,
  "soChuaDoc": null,
  "ghiChu": "Bảng ThongBao không có cột theo dõi đã đọc/chưa đọc nên không thể đếm số chưa đọc."
}
```

### `GET /api/notifications/{id}`

Không thuộc phạm vi của mình ⇒ **404** (không lộ sự tồn tại).

---

## 3. Quản trị thông báo — `/api/admin/notifications`

### `GET /api/admin/notifications`

Query: `maTaiKhoanGui`, `maTaiKhoanNhan`, `maLopHoc`, `trangThai`, `loaiThongBao`, `tuNgay`, `denNgay`, `search`, `page`, `pageSize`

### `POST /api/admin/notifications`

```json
{
  "maTaiKhoanNhan": 8,
  "maLopHoc": null,
  "tieuDe": "Thông báo lịch học",
  "noiDung": "Lớp học nghỉ ngày 20/10.",
  "loaiThongBao": "Lịch học"
}
```

| Tình huống | HTTP |
|---|---|
| Thành công | **201** |
| Thiếu cả `maTaiKhoanNhan` và `maLopHoc` | **400** |
| Tài khoản / lớp không tồn tại | **400** |
| Giáo viên gửi trực tiếp cho tài khoản | **403** |
| Giáo viên gửi lớp không phụ trách | **403** |

`trangThai = "Đã gửi"`, `thoiGianGui`, `maTaiKhoanGui` do **server** quyết định.

### `PATCH /api/admin/notifications/{id}`

```json
{ "trangThai": "Đã thu hồi" }
```

Cho phép: `tieuDe`, `noiDung`, `loaiThongBao` (gửi `""` để xóa), `trangThai` (`Đã gửi` / `Đã thu hồi`).
**Không** đổi được người gửi / người nhận.

### `DELETE /api/admin/notifications/{id}` → `204`

---

## 4. Quản trị khảo sát — `/api/admin/surveys` (QTV, NV học vụ)

### `GET /api/admin/surveys`

Query: `trangThai` (`Hoạt động` / `Tạm dừng` / `Đã kết thúc`), `tuNgay`, `denNgay` (theo `ngayBatDau`), `search`, `page`, `pageSize`

### `POST /api/admin/surveys`

```json
{
  "tenKhaoSat": "Khảo sát chất lượng học kỳ 1",
  "moTa": "Đánh giá chất lượng giảng dạy và dịch vụ",
  "ngayBatDau": "2026-09-01",
  "ngayKetThuc": "2026-12-31",
  "trangThai": "Hoạt động"
}
```

`trangThai` để trống ⇒ `"Hoạt động"`. `ngayKetThuc` phải `>= ngayBatDau`.

### `GET /api/admin/surveys/{id}`

Trả kèm `cauHois[]` và cờ `dangMo` (trạng thái + khoảng ngày).

### `PATCH /api/admin/surveys/{id}`

```json
{ "tenKhaoSat": "Tên mới", "xoaNgayKetThuc": true }
```

### `PATCH /api/admin/surveys/{id}/status`

```json
{ "trangThai": "Đã kết thúc" }
```

### `DELETE /api/admin/surveys/{id}`

**409** nếu đã có phản hồi (khuyến nghị chuyển `Đã kết thúc` thay vì xóa).

### Câu hỏi

| Method | Route |
|---|---|
| `GET` | `/api/admin/surveys/{id}/questions` |
| `POST` | `/api/admin/surveys/{id}/questions` |
| `PATCH` | `/api/admin/surveys/{id}/questions/{maCauHoi}` |
| `DELETE` | `/api/admin/surveys/{id}/questions/{maCauHoi}` |

```json
{
  "noiDungCauHoi": "Bạn đánh giá chất lượng giảng dạy như thế nào?",
  "loaiCauHoi": "Đánh giá 1-5",
  "thuTu": 1,
  "batBuoc": true
}
```

- `thuTu` **không trùng** trong cùng khảo sát (`UNIQUE (MaKhaoSat, ThuTu)`) ⇒ **409**.
- `loaiCauHoi` chuẩn: `"Đánh giá 1-5"` (dùng `giaTriDanhGia`) và `"Tự luận"` (dùng `noiDungTraLoi`). Loại khác vẫn được lưu (không whitelist).
- Xóa câu hỏi đã có câu trả lời ⇒ **409** (chuyển `Ngừng hoạt động`).

### `GET /api/admin/surveys/{id}/responses`

Query: `maHocVien`, `page`, `pageSize`

### `GET /api/admin/surveys/{id}/statistics`

```json
{
  "maKhaoSat": 3,
  "tenKhaoSat": "Khảo sát chất lượng học kỳ 1",
  "trangThai": "Hoạt động",
  "ngayBatDau": "2026-09-01",
  "ngayKetThuc": "2026-12-31",
  "tongSoCauHoi": 2,
  "tongSoPhanHoi": 5,
  "diemDanhGiaChungTrungBinh": 4.3,
  "cauHois": [
    {
      "maCauHoi": 1,
      "noiDungCauHoi": "Bạn đánh giá chất lượng giảng dạy như thế nào?",
      "loaiCauHoi": "Đánh giá 1-5",
      "thuTu": 1,
      "batBuoc": true,
      "soLuotTraLoi": 5,
      "giaTriTrungBinh": 4.4,
      "phanBo": [
        { "giaTri": 3, "soLuong": 1 },
        { "giaTri": 4, "soLuong": 1 },
        { "giaTri": 5, "soLuong": 3 }
      ],
      "noiDungTuLuan": []
    },
    {
      "maCauHoi": 2,
      "noiDungCauHoi": "Bạn có góp ý gì?",
      "loaiCauHoi": "Tự luận",
      "soLuotTraLoi": 3,
      "giaTriTrungBinh": null,
      "phanBo": [],
      "noiDungTuLuan": ["Nên bổ sung giờ Speaking."]
    }
  ]
}
```

> Nội dung tự luận tối đa 50 mục và **không kèm danh tính người trả lời**.

---

## 5. Khảo sát của học viên — `/api/students/me/surveys`

| Method | Route | Mô tả |
|---|---|---|
| `GET` | `/api/students/me/surveys` | Khảo sát `Hoạt động` hoặc khảo sát tôi đã trả lời |
| `GET` | `/api/students/me/surveys/{id}` | Chi tiết + `daTraLoi`, `maPhanHoiKhaoSat` |
| `GET` | `/api/students/me/surveys/{id}/response` | Phản hồi tôi đã gửi |
| `POST` | `/api/students/me/surveys/{id}/responses` | Gửi phản hồi (1 lần duy nhất) |

```json
{
  "diemDanhGiaChung": 4.5,
  "ghiChu": "Hài lòng",
  "cauTraLois": [
    { "maCauHoi": 1, "giaTriDanhGia": 5 },
    { "maCauHoi": 2, "noiDungTraLoi": "Rất tốt" }
  ]
}
```

| Tình huống | HTTP |
|---|---|
| Thành công | **201** |
| Khảo sát không `Hoạt động` / ngoài khoảng ngày | **400** |
| Câu hỏi bắt buộc chưa trả lời | **400** |
| Câu hỏi không thuộc khảo sát / trùng mã | **400** |
| Sai kiểu (đánh giá mà gửi chữ, tự luận mà gửi số) | **400** |
| `giaTriDanhGia` ngoài 0..5 | **400** |
| Đã trả lời trước đó | **409** |

---

## 6. Báo cáo — `/api/admin/reports`

### `GET /api/admin/reports/overview` (QTV, NV học vụ)

```json
{
  "tongTaiKhoan": 32, "tongHocVien": 14, "soHocVienDangHoc": 12,
  "tongGiaoVien": 10, "tongNhanVien": 6,
  "tongChuongTrinh": 3, "tongCapDo": 8, "tongKhoaHoc": 6,
  "tongLopHoc": 5, "soLopDangHoc": 3,
  "dangKy": { "tongSo": 4, "choXuLy": 0, "daXepLop": 3, "hoanThanh": 0, "daHuy": 1 },
  "caHoc": { "tongSo": 10, "chuaDienRa": 6, "dangDienRa": 0, "daHoanThanh": 4, "daHuy": 0, "sapDienRa": 6 },
  "diemDanh": { "tongLuot": 20, "coMat": 17, "vang": 2, "vangCoPhep": 1, "diTre": 0, "tiLeCoMat": 85.0 },
  "hocPhi": { "tongPhaiThu": 14400000, "tongDaThu": 12700000, "tongConThieu": 1700000, "soHocPhiConNo": 2, "soHocVienConNo": 2 },
  "khaoSat": { "tongSo": 2, "soHoatDong": 2, "tongCauHoi": 4, "tongPhanHoi": 2 }
}
```

### `GET /api/admin/reports/academic?maLopHoc={id}` (QTV, NVHV, GV, TG)

- `maLopHoc` **bắt buộc** (thiếu ⇒ 400); lớp không tồn tại ⇒ 404.
- Giáo viên/Trợ giảng không phụ trách lớp ⇒ **403**.
- Điểm tổng kết dùng **đúng công thức đã chốt ở Phase 5 (Q6)**, không tạo công thức mới.

```json
{
  "maLopHoc": 1, "maLop": "LOP2026A", "tenLop": "Lớp A1",
  "maKhoaHoc": 1, "tenKhoaHoc": "Tiếng Anh A1",
  "soHocVien": 12, "tongTrongSo": 100, "canhBaoTrongSo": false,
  "thanhPhan": [ { "maLoaiDiem": 1, "tenLoaiDiem": "Giữa kỳ", "trongSo": 30, "diemToiDa": 10, "thuTu": 1 } ],
  "soDuDieuKien": 10, "soThieuDiem": 2,
  "diemTongKetTrungBinh": 7.85, "diemCaoNhat": 9.1, "diemThapNhat": 6.4,
  "hocViens": [
    { "maDangKy": 1, "maHocVien": "HV001", "hoTen": "...", "duDieuKien": true, "diemTongKet": 8.25,
      "thanhPhan": [ { "maLoaiDiem": 1, "tenLoaiDiem": "Giữa kỳ", "trongSo": 30, "diemToiDa": 10, "diem": 8.0 } ],
      "thanhPhanConThieu": [] }
  ]
}
```

### `GET /api/admin/reports/attendance?maLopHoc={id}&tuNgay=&denNgay=&nguongVang=30`

- `nguongVang` 0..100 (mặc định 30).
- **Ca học `Đã hủy` KHÔNG được tính.**

```json
{
  "maLopHoc": 1, "maLop": "LOP2026A", "tenLop": "Lớp A1",
  "tuNgay": "2026-09-01", "denNgay": "2026-12-31",
  "soCaHocDaDiemDanh": 10, "nguongVang": 30, "tiLeCoMatToanLop": 85.0,
  "hocViens": [
    { "maHocVien": "HV001", "hoTen": "...", "tongLuot": 10, "coMat": 9, "vang": 1,
      "vangCoPhep": 0, "diTre": 0, "tiLeCoMat": 90.0, "tiLeVang": 10.0, "vangCao": false }
  ],
  "hocVienVangCao": []
}
```

### `GET /api/admin/reports/surveys` (QTV, NV học vụ)

```json
{
  "tongKhaoSat": 2, "soHoatDong": 2, "soTamDung": 0, "soDaKetThuc": 0,
  "tongCauHoi": 4, "tongPhanHoi": 2, "diemDanhGiaChungTrungBinh": 4.25,
  "tongHocVien": 14,
  "khaoSats": [
    { "maKhaoSat": 1, "tenKhaoSat": "...", "trangThai": "Hoạt động",
      "ngayBatDau": "2026-09-01", "ngayKetThuc": "2026-12-31",
      "soCauHoi": 2, "soPhanHoi": 1, "diemTrungBinh": 4.5, "tiLePhanHoi": 7.14 }
  ]
}
```

> **Báo cáo tài chính** không lặp lại ở đây — dùng `GET /api/admin/tuition-fees/reports/summary` (Phase 6).

---

## 7. Thứ tự test gợi ý

1. Login `ADMIN001` → token.
2. `GET /api/notifications` (token `HV001`) → kiểm tra chỉ thấy thông báo của mình.
3. `POST /api/admin/notifications` (ADMIN) gửi cho `HV001` → 201.
4. `PATCH` trạng thái `Đã thu hồi` → `HV001` không còn thấy.
5. `POST /api/admin/surveys` → 201; thêm 2 câu hỏi (`Đánh giá 1-5` bắt buộc, `Tự luận`).
6. Login `HV001` → `POST /api/students/me/surveys/{id}/responses` → 201.
7. Gửi lại → 409.
8. `GET /api/admin/surveys/{id}/statistics` → số liệu đúng.
9. `PATCH /api/admin/surveys/{id}/status` → `Đã kết thúc`; học viên khác gửi → 400.
10. `GET /api/admin/reports/overview` (ADMIN) → 200.
11. `GET /api/admin/reports/academic?maLopHoc=1` (ADMIN) → 200; (GV không phụ trách) → 403.
12. `GET /api/admin/reports/attendance?maLopHoc=1` → 200.
13. `GET /api/admin/reports/surveys` → 200.
