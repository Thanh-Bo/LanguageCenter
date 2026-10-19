import { http, qs } from './http'
import type {
  AcademicReport,
  AttendanceReport,
  AttendanceSheet,
  ClassListItem,
  CurrentUser,
  FinanceReport,
  LoginResponse,
  MySurvey,
  NotificationItem,
  NotificationSummary,
  PagedResult,
  Payment,
  ReportOverview,
  SessionListItem,
  StudentAttendanceHistory,
  StudentCourseGrade,
  StudentListItem,
  SurveyDetail,
  SurveyItem,
  SurveyReport,
  SurveyResponse,
  SurveyStatistics,
  TeacherClass,
  TeacherClassStudent,
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

// ===== HỌC VIÊN (quản trị) =====
export const hocVienApi = {
  danhSach: (q: Query) => http.get<PagedResult<StudentListItem>>(`/admin/students${qs(q)}`),
  chiTiet: (maHocVien: string) => http.get<StudentListItem>(`/admin/students/${maHocVien}`),
}

// ===== LỚP HỌC =====
export const lopHocApi = {
  danhSach: (q: Query) => http.get<PagedResult<ClassListItem>>(`/admin/classes${qs(q)}`),
  chiTiet: (maLopHoc: number) => http.get<ClassListItem>(`/admin/classes/${maLopHoc}`),
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

// ===== GIÁO VIÊN =====
export const giaoVienApi = {
  lichDay: (q: Query = {}) =>
    http.get<SessionListItem[]>(`/teachers/me/schedule${qs(q)}`),
  lopCuaToi: () => http.get<TeacherClass[]>('/teachers/me/classes'),
  hocVienCuaLop: (maLopHoc: number) =>
    http.get<TeacherClassStudent[]>(`/teachers/me/classes/${maLopHoc}/students`),
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
}

// ===== HỌC VIÊN (tự phục vụ) =====
export const hocVienSelfApi = {
  diem: () => http.get<StudentCourseGrade[]>('/students/me/grades'),
  lichHoc: (q: Query = {}) => http.get<SessionListItem[]>(`/students/me/schedule${qs(q)}`),
  diemDanh: (q: Query = {}) =>
    http.get<StudentAttendanceHistory>(`/students/me/attendance${qs(q)}`),
  hocPhi: (q: Query = {}) =>
    http.get<PagedResult<TuitionItem>>(`/students/me/tuition-fees${qs(q)}`),
  thanhToan: (q: Query = {}) =>
    http.get<PagedResult<Payment>>(`/students/me/payments${qs(q)}`),

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
