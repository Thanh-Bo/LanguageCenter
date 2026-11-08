import { http, qs } from './http'
import type {
  AcademicReport,
  AssignmentItem,
  AttendanceReport,
  AttendanceSheet,
  ClassListItem,
  ClassProgress,
  CurrentUser,
  EnrollmentItem,
  FinanceReport,
  GradeItem,
  HoSoCaNhan,
  Holiday,
  LoginResponse,
  MySurvey,
  NotificationItem,
  NotificationSummary,
  PagedResult,
  Payment,
  ReportOverview,
  ScheduleListItem,
  SessionListItem,
  SettleSessionsResult,
  StudentAttendanceHistory,
  StudentCourseGrade,
  StudentListItem,
  SurveyDetail,
  SurveyItem,
  SurveyReport,
  SurveyResponse,
  SurveyStatistics,
  SyncSessionTeachersResult,
  TeacherClass,
  TeacherClassStudent,
  TeacherOption,
  TuitionItem,
} from './types'

type Query = Record<string, string | number | boolean | null | undefined>

// ===== XÁC THỰC =====
export const authApi = {
  dangNhap: (tenDangNhap: string, matKhau: string) =>
    http.post<LoginResponse>('/auth/login', {
      username: tenDangNhap,
      password: matKhau,
    }),

  toi: () => http.getKhongChan401<CurrentUser>('/auth/me'),

  doiMatKhau: (currentPassword: string, newPassword: string, confirmPassword: string) =>
    http.post<void>('/auth/change-password', {
      currentPassword,
      newPassword,
      confirmPassword,
    }),
}

// ===== TIỆN ÍCH DEVELOPMENT (chỉ có ở môi trường Development) =====
export const devApi = {
  /**
   * Tài khoản mẫu để hiện ở khung "đăng nhập nhanh".
   * Trả về `null` nếu API không có endpoint (môi trường không phải Development).
   */
  taiKhoanMau: () =>
    http.getKhongChan401<{
      matKhauMacDinh: string
      nhom: Array<{
        ma: string
        nhan: string
        icon: string
        taiKhoans: Array<{ tenDangNhap: string; hoTen: string }>
      }>
    }>('/development/demo-accounts'),
}

// ===== HỌC VIÊN (quản trị) =====
export const hocVienApi = {
  danhSach: (q: Query) => http.get<PagedResult<StudentListItem>>(`/admin/students${qs(q)}`),
  chiTiet: (maHocVien: string) => http.get<StudentListItem>(`/admin/students/${maHocVien}`),
}

// ===== LỚP HỌC =====
export const lopHocApi = {
  danhSach: (q: Query) => http.get<PagedResult<ClassListItem>>(`/admin/classes${qs(q)}`),
  chiTiet: (maLopHoc: number) => http.get<ClassListItem>(`/admin/classes/${maLopHoc}`),
  /** Tiến độ từng buổi của lớp (Admin / Nhân viên học vụ). */
  tienDo: (maLopHoc: number) =>
    http.get<ClassProgress>(`/admin/classes/${maLopHoc}/progress`),
  /** Lịch học định kỳ (2 thứ/tuần) của lớp. */
  lichHoc: (maLopHoc: number) =>
    http.get<PagedResult<ScheduleListItem>>(
      `/admin/schedules${qs({ maLopHoc, page: 1, pageSize: 20 })}`,
    ),
  phanCong: (maLopHoc: number) =>
    http.get<PagedResult<AssignmentItem>>(
      `/admin/teaching-assignments${qs({ maLopHoc, page: 1, pageSize: 50 })}`,
    ),
  dangKy: (maLopHoc: number) =>
    http.get<PagedResult<EnrollmentItem>>(
      `/admin/enrollments${qs({ maLopHoc, page: 1, pageSize: 200 })}`,
    ),
}

// ===== CA HỌC (quản trị) =====
export const caHocApi = {
  danhSach: (q: Query) => http.get<PagedResult<SessionListItem>>(`/admin/sessions${qs(q)}`),
  sinhBuoi: (maLichHoc: number) =>
    http.post<unknown>('/admin/sessions/generate', { maLichHoc }),
  /** Đồng bộ GV cho các ca CHƯA diễn ra: buổi đầu tuần = GV nước ngoài. */
  dongBoGiaoVien: (maLopHoc: number) =>
    http.post<SyncSessionTeachersResult>('/admin/sessions/sync-teachers', { maLopHoc }),
  /**
   * Chốt trạng thái theo NGÀY: buổi đã qua → "Đã hoàn thành",
   * buổi chưa tới ngày → "Chưa diễn ra". Bỏ trống lớp = toàn bộ.
   */
  chotBuoiDaQua: (maLopHoc?: number) =>
    http.post<SettleSessionsResult>('/admin/sessions/settle', { maLopHoc: maLopHoc ?? null }),
  capNhat: (
    maCaHoc: number,
    body: {
      loaiBuoi?: string | null
      maGiaoVienThucTe?: string | null
      maTroGiangThucTe?: string | null
      ngayHoc?: string | null
      gioBatDau?: string | null
      noiDungBaiHoc?: string | null
      lyDoThayDoi?: string | null
    },
  ) => http.patch<SessionListItem>(`/admin/sessions/${maCaHoc}`, body),
  doiTrangThai: (maCaHoc: number, trangThai: string) =>
    http.patch<SessionListItem>(`/admin/sessions/${maCaHoc}/status`, { trangThai }),
}

// ===== HỌC PHÍ / THANH TOÁN =====
export const hocPhiApi = {
  danhSach: (q: Query) => http.get<PagedResult<TuitionItem>>(`/admin/tuition-fees${qs(q)}`),
  chiTiet: (maHocPhi: number) => http.get<TuitionItem>(`/admin/tuition-fees/${maHocPhi}`),
  giaoDich: (maHocPhi: number, q: Query = {}) =>
    http.get<PagedResult<Payment>>(`/admin/tuition-fees/${maHocPhi}/payments${qs(q)}`),
  ghiNhanThanhToan: (
    maHocPhi: number,
    body: {
      maGiaoDich: string
      soTien: number
      phuongThucThanhToan: string
      ghiChu?: string | null
    },
  ) => http.post<Payment>(`/admin/tuition-fees/${maHocPhi}/payments`, body),
  baoCao: (q: Query = {}) =>
    http.get<FinanceReport>(`/admin/tuition-fees/reports/summary${qs(q)}`),
}

// ===== THÔNG BÁO =====
export const thongBaoApi = {
  cuaToi: (q: Query = {}) => http.get<PagedResult<NotificationItem>>(`/notifications${qs(q)}`),
  tongHop: () => http.get<NotificationSummary>('/notifications/summary'),

  /** Thông báo do CHÍNH mình gửi (kể cả đã thu hồi) — tab "Đã gửi". */
  daGui: (q: Query = {}) =>
    http.get<PagedResult<NotificationItem>>(`/notifications/sent${qs(q)}`),
  /** Thu hồi (ẩn mềm) thông báo mình đã gửi — người nhận không thấy nữa. */
  thuHoi: (maThongBao: number) => http.post<boolean>(`/notifications/${maThongBao}/revoke`, {}),

  quanTri: (q: Query = {}) =>
    http.get<PagedResult<NotificationItem>>(`/admin/notifications${qs(q)}`),
  gui: (body: {
    maTaiKhoanNhan?: number | null
    maLopHoc?: number | null
    tieuDe: string
    noiDung: string
    loaiThongBao?: string | null
  }) => http.post<NotificationItem>('/admin/notifications', body),
  xoa: (maThongBao: number) => http.delete<void>(`/admin/notifications/${maThongBao}`),
}

// ===== KHẢO SÁT (quản trị) =====
export const khaoSatApi = {
  danhSach: (q: Query = {}) => http.get<PagedResult<SurveyItem>>(`/admin/surveys${qs(q)}`),
  chiTiet: (maKhaoSat: number) => http.get<SurveyDetail>(`/admin/surveys/${maKhaoSat}`),
  tao: (body: {
    tenKhaoSat: string
    moTa?: string | null
    ngayBatDau: string
    ngayKetThuc?: string | null
  }) => http.post<SurveyDetail>('/admin/surveys', body),
  doiTrangThai: (maKhaoSat: number, trangThai: string) =>
    http.patch<SurveyDetail>(`/admin/surveys/${maKhaoSat}/status`, { trangThai }),
  thongKe: (maKhaoSat: number) =>
    http.get<SurveyStatistics>(`/admin/surveys/${maKhaoSat}/statistics`),
}

// ===== BÁO CÁO =====
export const baoCaoApi = {
  tongQuan: () => http.get<ReportOverview>('/admin/reports/overview'),
  hocTap: (maLopHoc: number) =>
    http.get<AcademicReport>(`/admin/reports/academic${qs({ maLopHoc })}`),
  diemDanh: (maLopHoc: number, q: Query = {}) =>
    http.get<AttendanceReport>(`/admin/reports/attendance${qs({ maLopHoc, ...q })}`),
  khaoSat: () => http.get<SurveyReport>('/admin/reports/surveys'),
}

// ===== KỲ NGHỈ / NGÀY LỄ =====
export const ngayLeApi = {
  danhSach: () => http.get<Holiday[]>('/admin/holidays'),
  them: (body: {
    ngay: string
    denNgay?: string | null
    tenNgayLe: string
    ghiChu?: string | null
  }) => http.post<Holiday>('/admin/holidays', body),
  capNhat: (
    maNgayLe: number,
    body: {
      ngay?: string
      denNgay?: string | null
      xoaDenNgay?: boolean
      tenNgayLe?: string
      ghiChu?: string | null
    },
  ) => http.patch<Holiday>(`/admin/holidays/${maNgayLe}`, body),
  xoa: (maNgayLe: number) => http.delete<void>(`/admin/holidays/${maNgayLe}`),
}

// ===== GIÁO VIÊN =====
export const giaoVienApi = {
  lichDay: (q: Query = {}) =>
    http.get<SessionListItem[]>(`/teachers/me/schedule${qs(q)}`),
  lopCuaToi: () => http.get<TeacherClass[]>('/teachers/me/classes'),
  hocVienCuaLop: (maLopHoc: number) =>
    http.get<TeacherClassStudent[]>(`/teachers/me/classes/${maLopHoc}/students`),
  /** Tiến độ từng buổi của lớp mình dạy. */
  tienDoLop: (maLopHoc: number) =>
    http.get<ClassProgress>(`/teachers/me/classes/${maLopHoc}/progress`),
  // Điểm danh dùng endpoint Phase 5 (đã cho phép Giáo viên/Trợ giảng)
  bangDiemDanh: (maCaHoc: number) =>
    http.get<AttendanceSheet>(`/admin/sessions/${maCaHoc}/attendance`),
  luuDiemDanh: (
    maCaHoc: number,
    items: Array<{ maHocVien: string; trangThaiDiemDanh: string; ghiChu?: string | null }>,
  ) => http.post<AttendanceSheet>(`/admin/sessions/${maCaHoc}/attendance`, { items }),
  // Bảng điểm dùng endpoint tổng kết đã có ở Phase 5
  bangDiemLop: (maLopHoc: number) =>
    http.get<AcademicReport>(`/admin/reports/academic${qs({ maLopHoc })}`),

  // ===== NHẬP ĐIỂM (giáo viên được phép theo AdminGradesController) =====
  diemCuaDangKy: (maDangKy: number) =>
    http.get<PagedResult<GradeItem>>(
      `/admin/grades${qs({ maDangKy, page: 1, pageSize: 50 })}`,
    ),
  /** Toàn bộ điểm đã lưu của một lớp — dùng cho lưới nhập điểm. */
  diemCuaLop: (maLopHoc: number) =>
    http.get<PagedResult<GradeItem>>(
      `/admin/grades${qs({ maLopHoc, page: 1, pageSize: 500 })}`,
    ),
  taoDiem: (body: {
    maDangKy: number
    maLoaiDiem: number
    diem: number
    nhanXet?: string | null
    /** Lần kiểm tra của đầu điểm (mặc định 1). */
    lanKiemTra?: number
    /** Gắn bài kiểm tra với buổi thi (tuỳ chọn). */
    maCaHoc?: number | null
  }) => http.post<GradeItem>('/admin/grades', body),
  suaDiem: (maDiemSo: number, body: { diem: number; nhanXet?: string | null }) =>
    http.patch<GradeItem>(`/admin/grades/${maDiemSo}`, body),
  xoaDiem: (maDiemSo: number) => http.delete<void>(`/admin/grades/${maDiemSo}`),
}

// ===== DANH MỤC GIÁO VIÊN (dropdown chọn người dạy) =====
export const giaoVienAdminApi = {
  danhSach: (q: Query = {}) =>
    http.get<PagedResult<TeacherOption>>(
      `/admin/teachers${qs({ page: 1, pageSize: 200, ...q })}`,
    ),
}

// ===== HỒ SƠ CÁ NHÂN (chỉ để lấy họ tên hiển thị trên sidebar) =====
export const hoSoApi = {
  giaoVien: () => http.get<HoSoCaNhan>('/teacher/profile'),
  hocVien: () => http.get<HoSoCaNhan>('/student/profile'),
}

// ===== HỌC VIÊN (tự phục vụ) =====
export const hocVienSelfApi = {
  diem: () => http.get<StudentCourseGrade[]>('/students/me/grades'),
  lichHoc: (q: Query = {}) => http.get<SessionListItem[]>(`/students/me/schedule${qs(q)}`),
  /** Tiến độ từng buổi của một lớp mình đang học. */
  tienDoLop: (maLopHoc: number) =>
    http.get<ClassProgress>(`/students/me/classes/${maLopHoc}/progress`),
  diemDanh: (q: Query = {}) =>
    http.get<StudentAttendanceHistory>(`/students/me/attendance${qs(q)}`),
  hocPhi: (q: Query = {}) =>
    http.get<PagedResult<TuitionItem>>(`/students/me/tuition-fees${qs(q)}`),
  thanhToan: (q: Query = {}) =>
    http.get<PagedResult<Payment>>(`/students/me/payments${qs(q)}`),
  /** Học viên tự thanh toán học phí — MÔ PHỎNG, server tự sinh biên nhận. */
  thanhToanHocPhi: (
    maHocPhi: number,
    body: { maGiaoDich: string; soTien: number; phuongThucThanhToan: string },
  ) => http.post<Payment>(`/students/me/tuition-fees/${maHocPhi}/payments`, body),

  khaoSat: () => http.get<MySurvey[]>('/students/me/surveys'),
  khaoSatChiTiet: (maKhaoSat: number) =>
    http.get<MySurvey>(`/students/me/surveys/${maKhaoSat}`),
  phanHoiCuaToi: (maKhaoSat: number) =>
    http.get<SurveyResponse>(`/students/me/surveys/${maKhaoSat}/response`),
  guiPhanHoi: (
    maKhaoSat: number,
    body: {
      diemDanhGiaChung?: number | null
      ghiChu?: string | null
      cauTraLois: Array<{
        maCauHoi: number
        giaTriDanhGia?: number | null
        noiDungTraLoi?: string | null
      }>
    },
  ) => http.post<SurveyResponse>(`/students/me/surveys/${maKhaoSat}/responses`, body),
}
