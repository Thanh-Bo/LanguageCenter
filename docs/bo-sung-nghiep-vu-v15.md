# Bổ sung nghiệp vụ v15 — Phân công theo buổi, tiến độ buổi học, quy tắc xếp lớp

> Phạm vi: **backend**. Giao diện web/WinForms chưa cập nhật theo tài liệu này.

## 1. Chạy migration

Chạy tuần tự (nếu chưa chạy):

```sql
:r database\QuanLyTrungTamNgoaiNgu_v2.sql   -- chỉ khi tạo DB mới
-- sau đó các script BoSung_*.sql theo thứ tự
database/BoSung_PhanCongBuoi_GiaoVien_v15.sql
```

Script v15 thêm:

| Bảng | Cột mới | Ghi chú |
|---|---|---|
| `PhanCongGiangDay` | vai trò `Giáo viên nước ngoài` | Mở rộng `CK_PhanCong_VaiTro` |
| `CaHoc` | `LoaiBuoi` (`Buổi học` / `Buổi thi`), `SoBuoi` | Tự backfill `SoBuoi` theo ngày |
| `NoiDungBaiHoc` | `LoaiBuoi` | Khai báo buổi thi ngay trong khóa học |
| `DiemSo` | `LanKiemTra` (mặc định 1), `MaCaHoc` | UNIQUE mới `(MaDangKy, MaLoaiDiem, LanKiemTra)` |

## 2. Lớp 2 buổi/tuần: 1 buổi GV nước ngoài + 1 buổi GV Việt

**Quy ước:** buổi **đầu tuần** (thứ nhỏ hơn trong 2 thứ của lớp) do **giáo viên nước ngoài** dạy; buổi còn lại do **giáo viên chính** (người Việt) dạy.

Cách dùng:

1. `POST /api/admin/teaching-assignments` với `vaiTroPhanCong = "Giáo viên nước ngoài"`.
2. `POST /api/admin/teaching-assignments` với `vaiTroPhanCong = "Giáo viên chính"`.
3. `POST /api/admin/sessions/generate` → 20 buổi được sinh, mỗi buổi tự gán đúng giáo viên.

Nếu lớp không có GV nước ngoài → cả 2 buổi dùng GV chính (hành vi cũ). Nếu chỉ có GV nước ngoài → dùng cho cả 2 buổi.

### 2b. Áp dụng cho lớp ĐÃ sinh buổi (đồng bộ giáo viên)

```
POST /api/admin/sessions/sync-teachers
{ "maLopHoc": 12 }
```

- Chỉ xử lý ca **Chưa diễn ra** (không đụng ca đã dạy / đã hủy).
- Trả về: `tongSoCa`, `soCaCapNhat`, `soCaKhongDoi`, `soCaBoQua`, `maGiaoVienChinh`, `maGiaoVienNuocNgoai`, `maTroGiang`.
- Dùng khi lớp đã sinh 20 buổi TRƯỚC khi khai báo phân công GV nước ngoài.
- Muốn sửa tay 1 buổi: `PATCH /api/admin/sessions/{id}` với `maGiaoVienThucTe` / `maTroGiangThucTe`.

## 3. Loại buổi học / buổi thi

- Khai báo ở khóa học: `PUT /api/admin/lesson-contents/{maKhoaHoc}` — mỗi buổi có thêm `loaiBuoi` (`"Buổi học"` | `"Buổi thi"`, mặc định `"Buổi học"`).
- Khi sinh ca học, `CaHoc.LoaiBuoi` kế thừa giá trị này; có thể sửa lẻ bằng `PATCH /api/admin/sessions/{id}` body `{ "loaiBuoi": "Buổi thi" }`.
- `CaHoc.SoBuoi` = số thứ tự buổi (1..20), có trong mọi DTO ca học.

## 4. Tiến độ buổi học

```
GET /api/admin/classes/{id}/progress        (Admin / Nhân viên học vụ)
GET /api/teachers/me/classes/{id}/progress  (Giáo viên / Trợ giảng được phân công lớp)
GET /api/students/me/classes/{id}/progress  (Học viên đang học lớp)
```

Trả về:

| Trường | Ý nghĩa |
|---|---|
| `tongSoBuoi` | Số buổi theo khóa học |
| `soBuoiDaSinh` | Số ca học đã sinh |
| `soBuoiDaHoc` / `soBuoiConLai` / `tienDoPhanTram` | Tiến độ |
| `maCaHocHienTai`, `soBuoiHienTai`, `ngayHocHienTai`, `loaiBuoiHienTai` | Buổi hiện tại |
| `buois[]` | Danh sách buổi: `soBuoi`, `ngayHoc`, `loaiBuoi`, `laBuoiThi`, `trangThai`, `daHoc`, `laBuoiHienTai`, giáo viên/trợ giảng, `soDiemDanh` |

Giao diện dùng `daHoc`, `laBuoiThi`, `laBuoiHienTai` để tô màu.

## 5. Điểm danh không được trước ngày học

`GET/POST /api/admin/sessions/{id}/attendance` trả lỗi khi `NgayHoc > hôm nay`:

> Chưa tới ngày học (dd/MM/yyyy) nên chưa thể điểm danh.

Trợ giảng vẫn được điểm danh như trước (nếu là `MaTroGiangThucTe` của ca hoặc được phân công lớp).

## 5b. Kỳ nghỉ khi tạo / dời ca học

Bảng `NgayLe` lưu **kỳ nghỉ theo khoảng**: `Ngay` (từ ngày) → `DenNgay` (đến ngày).
`DenNgay = NULL` nghĩa là nghỉ đúng 1 ngày. Hai kỳ nghỉ **không được chồng lấn**
(kiểm tra ở `AcademicSetupService`).

`POST /api/admin/sessions` và `PATCH /api/admin/sessions/{id}` (khi có `ngayHoc`) sẽ trả `400` nếu ngày rơi vào **bất kỳ ngày nào** trong một kỳ nghỉ:

> Ngày học trùng ngày lễ: dd/MM/yyyy là 'Tên kỳ nghỉ' — hãy chọn ngày khác.

Ngoài ra `POST /api/admin/sessions/generate` vẫn tự dời buổi trùng kỳ nghỉ +7 ngày
(lặp lại nếu tuần sau vẫn còn trong kỳ nghỉ dài, ví dụ Tết) như trước.

Quản lý kỳ nghỉ: WinForms `FormNgayLe`, hoặc web **Quản trị → Kỳ nghỉ** (`/admin/ngay-le`).

## 6. Quy tắc xếp lớp (assign-class)

`PATCH /api/admin/enrollments/{id}/assign-class` (và `POST /api/admin/enrollments` khi có `maLopHoc`) kiểm tra thêm:

1. **Độ tuổi**: tuổi học viên tại `LopHoc.NgayBatDau` phải nằm trong `ChuongTrinh.DoTuoiTu..DoTuoiDen`.
2. **Trùng thời gian**: không trùng thứ + giờ chồng lấn + khoảng ngày chồng lấn với lớp khác học viên đang học.
3. **Gần kết thúc**: lớp đã diễn ra quá `EnrollmentRules.SoBuoiDaDienRaToiDa` (mặc định **4** buổi, xem `LanguageCenter.Domain/Constants/EnrollmentRules.cs`) thì không nhận thêm.

Vi phạm → `400 BadRequest` kèm danh sách lỗi cụ thể.

## 7. Nhiều bài kiểm tra cho một đầu điểm

`POST /api/admin/grades`:

```json
{
  "maDangKy": 12,
  "maLoaiDiem": 3,
  "lanKiemTra": 2,
  "maCaHoc": 87,
  "diem": 8.5,
  "nhanXet": "Lần 2 tốt hơn"
}
```

- `lanKiemTra`: lần kiểm tra (1..99), mặc định 1 → một đầu điểm có thể có nhiều bài.
- `maCaHoc` (tuỳ chọn): gắn điểm với **buổi thi**; server kiểm tra ca học thuộc lớp của đăng ký và `loaiBuoi = "Buổi thi"`.
- Bảng điểm tổng kết lấy điểm của **lần kiểm tra cao nhất** (lần gần nhất) cho mỗi đầu điểm.

## 8. Giao diện (web/HUITLINGO) đã cập nhật

| Màn hình | Thay đổi |
|---|---|
| Admin → Lớp học | thêm cột Thời khóa biểu + Buổi đã sinh; nút **Chi tiết** mở modal 4 tab |
| Admin → Lịch & ca học (mới) | lưới tuần + danh sách ca, lọc theo lớp / loại buổi, badge buổi thi |
| Modal chi tiết lớp | **Tiến độ buổi học** (lưới 20 ô, màu theo trạng thái), **Lịch & ca học** (sinh 20 buổi, đồng bộ GV, đổi loại buổi / trạng thái từng ca), **Phân công** (3 vai trò + ghi chú quy ước), **Học viên** (tuổi, cảnh báo quá 4 buổi) |
| Giáo viên → Lớp của tôi | thanh tiến độ `x/20 buổi`, buổi hiện tại, badge vai trò GV nước ngoài |
| Giáo viên → Lịch dạy | thẻ ca hiện `Buổi N`, badge **THI**, tô tím buổi thi |
| Giáo viên → Điểm danh | header hiện `Buổi N` + `Buổi thi`; nút lưu tự khóa khi chưa tới ngày (server trả `coTheDiemDanh=false`) |
| Giáo viên → Bảng điểm | chọn **Lần kiểm tra 1..5** + chọn **Buổi thi** để gắn điểm |
| Học viên → Tiến độ khóa học | dùng API tiến độ (số buổi chuẩn theo khóa), ô buổi thi tô tím, buổi hiện tại viền xanh |
| Học viên → Lịch học/Điểm số | thẻ ca có badge THI + số buổi; ghi chú điểm lấy lần mới nhất |
| Dùng chung | component `TienDoBuoiHoc`, type `ClassProgress` / `SessionProgressItem` / `AssignmentItem` / `EnrollmentItem` / `ScheduleListItem` |

### Chưa có màn (cần làm tiếp)

- Màn Admin **Đăng ký & xếp lớp** (hiện chưa có trang; API đã hỗ trợ).
- Màn Admin **Chương trình khóa học** chọn loại buổi cho 20 buổi (API `PUT /admin/lesson-contents` đã hỗ trợ `loaiBuoi`).
- Màn Admin **Phân công giảng dạy** (hiện chỉ xem trong modal lớp; cần form thêm/sửa để chọn vai trò *Giáo viên nước ngoài*).
