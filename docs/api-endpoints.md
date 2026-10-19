# HUIT LingoHub — Danh sách API Backend

> Tổng hợp toàn bộ endpoint đã triển khai trong `backend/LanguageCenter.Api/Controllers`.
> **Tổng cộng: 143 endpoint / 32 controller.**

| Thông tin | Giá trị |
|---|---|
| Base URL | `http://localhost:5204` |
| Swagger UI | `http://localhost:5204/swagger` |
| OpenAPI JSON | `http://localhost:5204/swagger/v1/swagger.json` |
| Xác thực | `Authorization: Bearer <accessToken>` (JWT HS256, hết hạn sau 60 phút) |
| Định dạng | camelCase · `DateOnly` → `"yyyy-MM-dd"` · `TimeOnly` → `"HH:mm:ss"` |
| Danh sách | Trả `PagedResult<T>`: `{ items, page, pageSize, totalItems, totalPages }` |

**Vai trò:** `Quản trị viên` · `Nhân viên học vụ` · `Nhân viên tư vấn` · `Nhân viên kế toán` · `Giáo viên` · `Trợ giảng` · `Học viên`

---

## 1. Health & Development (3)

| Method | URL | Quyền |
|---|---|---|
| GET | `/health` | Public |
| GET | `/health/database` | Public |
| GET | `/api/development/database-info` | Public (Development) |

---

## 2. Xác thực (3)

| Method | URL | Quyền | Ghi chú |
|---|---|---|---|
| POST | `/api/auth/login` | Public | Body `{ username, password }` → `{ accessToken, tokenType, expiresAt, user }` |
| GET | `/api/auth/me` | Đã đăng nhập | Danh tính lấy từ JWT |
| POST | `/api/auth/change-password` | Đã đăng nhập | Body `{ currentPassword, newPassword, confirmPassword }` |

---

## 3. Tài khoản (7)

| Method | URL | Quyền | Ghi chú |
|---|---|---|---|
| GET | `/api/admin/accounts` | Quản trị viên | Paging `page`/`pageSize`, lọc `search`/`role`/`status`. Mỗi dòng có `profileType` = `NhanVien`/`GiaoVien`/`HocVien`; **`null` = tài khoản chưa có hồ sơ** (khi đó `fullName` chỉ là bản sao của tên đăng nhập) |
| GET | `/api/admin/accounts/{id}` | Quản trị viên | |
| POST | `/api/admin/accounts` | Quản trị viên | Tạo tài khoản **không kèm hồ sơ** (Nhân viên / Kế toán / Tư vấn). Body `{ tenDangNhap, email, matKhau, vaiTro, trangThai? }` |
| PATCH | `/api/admin/accounts/{id}` | Quản trị viên | Sửa `{ email?, matKhau?, vaiTro?, trangThai? }` — null = giữ nguyên |
| PATCH | `/api/admin/accounts/{id}/status` | Quản trị viên | |
| POST | `/api/admin/accounts/{id}/reset-password` | Quản trị viên | |
| DELETE | `/api/admin/accounts/{id}` | Quản trị viên | |

---

## 4. Học viên & Giáo viên (8)

| Method | URL | Quyền | Ghi chú |
|---|---|---|---|
| GET | `/api/admin/students` | Quản trị viên | Lọc `search`, `trangThai`, `page`, `pageSize` |
| POST | `/api/admin/students` | Quản trị viên | Tự sinh `MaHocVien` + tài khoản |
| GET | `/api/admin/students/{studentId}` | Quản trị viên | |
| PATCH | `/api/admin/students/{studentId}` | Quản trị viên | |
| GET | `/api/admin/teachers` | Quản trị viên | Lọc theo loại giáo viên/trợ giảng |
| POST | `/api/admin/teachers` | Quản trị viên | |
| GET | `/api/admin/teachers/{teacherId}` | Quản trị viên | |
| PATCH | `/api/admin/teachers/{teacherId}` | Quản trị viên | |

---

## 5. Danh mục đào tạo (29)

### 5.1 Khóa học (6)
| Method | URL |
|---|---|
| GET | `/api/admin/courses` |
| POST | `/api/admin/courses` |
| GET | `/api/admin/courses/{id}` |
| PATCH | `/api/admin/courses/{id}` |
| PATCH | `/api/admin/courses/{id}/status` |

### 5.2 Cấp độ (5)
| Method | URL |
|---|---|
| GET / POST | `/api/admin/levels` |
| GET / PATCH | `/api/admin/levels/{id}` |
| PATCH | `/api/admin/levels/{id}/status` |

### 5.3 Chương trình (5)
| Method | URL |
|---|---|
| GET / POST | `/api/admin/programs` |
| GET / PATCH | `/api/admin/programs/{id}` |
| PATCH | `/api/admin/programs/{id}/status` |

### 5.4 Loại điểm (6)
| Method | URL |
|---|---|
| GET / POST | `/api/admin/grade-types` |
| GET / PATCH | `/api/admin/grade-types/{id}` |
| PATCH | `/api/admin/grade-types/{id}/status` |
| DELETE | `/api/admin/grade-types/{id}` |

### 5.5 Khuyến mãi (5)
| Method | URL |
|---|---|
| GET / POST | `/api/admin/promotions` |
| GET / PATCH | `/api/admin/promotions/{id}` |
| PATCH | `/api/admin/promotions/{id}/status` |

### 5.6 Phòng học (2)
| Method | URL |
|---|---|
| GET | `/api/admin/rooms` |
| GET | `/api/admin/rooms/{id}` |

---

## 6. Lớp – Lịch – Ca học – Phân công (24)

### 6.1 Lớp học (6)
| Method | URL |
|---|---|
| GET / POST | `/api/admin/classes` |
| GET / PATCH | `/api/admin/classes/{id}` |
| PATCH | `/api/admin/classes/{id}/status` |
| DELETE | `/api/admin/classes/{id}` |

### 6.2 Lịch học định kỳ (6)
| Method | URL |
|---|---|
| GET / POST | `/api/admin/schedules` |
| GET / PATCH | `/api/admin/schedules/{id}` |
| PATCH | `/api/admin/schedules/{id}/status` |
| DELETE | `/api/admin/schedules/{id}` |

### 6.3 Ca học (7)
| Method | URL |
|---|---|
| GET / POST | `/api/admin/sessions` |
| GET / PATCH | `/api/admin/sessions/{id}` |
| PATCH | `/api/admin/sessions/{id}/status` |
| DELETE | `/api/admin/sessions/{id}` |
| POST | `/api/admin/sessions/generate` |

### 6.4 Phân công giảng dạy (5)
| Method | URL |
|---|---|
| GET / POST | `/api/admin/teaching-assignments` |
| GET / PATCH | `/api/admin/teaching-assignments/{id}` |
| PATCH | `/api/admin/teaching-assignments/{id}/status` |

---

## 7. Ghi danh – Điểm danh – Điểm số (16)

### 7.1 Đăng ký học (6)
| Method | URL |
|---|---|
| GET / POST | `/api/admin/enrollments` |
| GET / PATCH | `/api/admin/enrollments/{id}` |
| PATCH | `/api/admin/enrollments/{id}/assign-class` |
| PATCH | `/api/admin/enrollments/{id}/status` |

### 7.2 Điểm danh (4)
| Method | URL |
|---|---|
| GET | `/api/admin/sessions/{id}/attendance` |
| POST | `/api/admin/sessions/{id}/attendance` |
| PATCH | `/api/admin/attendance/{maDiemDanh}` |
| GET | `/api/admin/students/{maHocVien}/attendance` |

### 7.3 Điểm số (6)
| Method | URL |
|---|---|
| GET | `/api/admin/grades` |
| GET | `/api/admin/grades/summary` |
| GET | `/api/admin/grades/{id}` |
| POST | `/api/admin/grades` |
| PATCH | `/api/admin/grades/{id}` |
| DELETE | `/api/admin/grades/{id}` |

---

## 8. Học phí – Thanh toán – Biên nhận (12)

### 8.1 Học phí (7)
| Method | URL | Ghi chú |
|---|---|---|
| GET | `/api/admin/tuition-fees` | Lọc `search`, `trangThai`, `quaHan`, `maLopHoc`, `maKhoaHoc` |
| GET | `/api/admin/tuition-fees/reports/summary` | Báo cáo tài chính tổng hợp |
| GET | `/api/admin/tuition-fees/{id}` | |
| GET | `/api/admin/tuition-fees/{id}/payments` | Giao dịch của 1 phiếu |
| POST | `/api/admin/tuition-fees` | Tạo phiếu học phí |
| PATCH | `/api/admin/tuition-fees/{id}` | |
| POST | `/api/admin/tuition-fees/{id}/payments` | **Ghi nhận thanh toán → tự sinh biên nhận** |

### 8.2 Giao dịch thanh toán (2)
| Method | URL |
|---|---|
| GET | `/api/admin/payments` |
| GET | `/api/admin/payments/{id}` |

### 8.3 Biên nhận (3)
| Method | URL |
|---|---|
| GET | `/api/admin/receipts` |
| GET | `/api/admin/receipts/by-number/{soBienNhan}` |
| GET | `/api/admin/receipts/{id}` |

---

## 9. Thông báo (8)

### 9.1 Quản trị (5)
| Method | URL |
|---|---|
| GET / POST | `/api/admin/notifications` |
| GET / PATCH / DELETE | `/api/admin/notifications/{id}` |

### 9.2 Người dùng nhận thông báo (3)
| Method | URL |
|---|---|
| GET | `/api/notifications` |
| GET | `/api/notifications/summary` |
| GET | `/api/notifications/{id}` |

---

## 10. Khảo sát (17)

### 10.1 Quản trị (13)
| Method | URL |
|---|---|
| GET / POST | `/api/admin/surveys` |
| GET / PATCH / DELETE | `/api/admin/surveys/{id}` |
| PATCH | `/api/admin/surveys/{id}/status` |
| GET / POST | `/api/admin/surveys/{id}/questions` |
| PATCH / DELETE | `/api/admin/surveys/{id}/questions/{maCauHoi}` |
| GET | `/api/admin/surveys/{id}/responses` |
| GET | `/api/admin/surveys/{id}/statistics` |

### 10.2 Học viên làm khảo sát (4)
| Method | URL |
|---|---|
| GET | `/api/students/me/surveys` |
| GET | `/api/students/me/surveys/{id}` |
| GET | `/api/students/me/surveys/{id}/response` |
| POST | `/api/students/me/surveys/{id}/responses` |

---

## 11. Báo cáo (4)

| Method | URL | Tham số |
|---|---|---|
| GET | `/api/admin/reports/overview` | — |
| GET | `/api/admin/reports/academic` | `maLopHoc` (bắt buộc) |
| GET | `/api/admin/reports/attendance` | `maLopHoc` (bắt buộc), `tuNgay`, `denNgay`, `nguongVang` (mặc định 30) |
| GET | `/api/admin/reports/surveys` | — |

---

## 12. Tự phục vụ — Học viên (10)

| Method | URL |
|---|---|
| GET | `/api/students/me/tuition-fees` |
| GET | `/api/students/me/tuition-fees/{id}` |
| GET | `/api/students/me/payments` |
| GET | `/api/students/me/receipts` |
| GET | `/api/students/me/receipts/{id}` |
| GET | `/api/students/me/attendance` |
| GET | `/api/students/me/grades` |
| GET | `/api/students/me/schedule` |
| GET | `/api/student/profile` |
| PATCH | `/api/student/profile` |

> Mọi endpoint `/api/students/me/*` lấy danh tính từ JWT — **không nhận mã học viên từ client**.

---

## 13. Tự phục vụ — Giáo viên / Trợ giảng (5)

| Method | URL |
|---|---|
| GET | `/api/teachers/me/schedule` |
| GET | `/api/teachers/me/classes` |
| GET | `/api/teachers/me/classes/{maLopHoc}/students` |
| GET | `/api/teacher/profile` |
| PATCH | `/api/teacher/profile` |

---

## Ghi chú triển khai

- **Database First** — không dùng migration, không `EnsureCreated`.
- Endpoint ghi dữ liệu tài chính (`POST /api/admin/tuition-fees/{id}/payments`) chạy trong transaction có khoá phiếu học phí (`UPDLOCK, HOLDLOCK`) và tự sinh số biên nhận `BNyyyyMMdd###`.
- Mã giao dịch (`maGiaoDich`) là khoá idempotency — gửi trùng sẽ bị từ chối với lỗi nghiệp vụ.
- Học viên **không thể** tự xác nhận thanh toán; chỉ `Quản trị viên` / `Nhân viên kế toán` ghi nhận qua `/api/admin/tuition-fees/{id}/payments`.
