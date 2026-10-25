// Kiểu dữ liệu dùng chung — phản chiếu DTO của backend ASP.NET Core.
// Backend serialize camelCase, DateOnly -> "yyyy-MM-dd", TimeOnly -> "HH:mm:ss".

export type VaiTro =
  | 'Quản trị viên'
  | 'Nhân viên học vụ'
  | 'Nhân viên tư vấn'
  | 'Nhân viên kế toán'
  | 'Giáo viên'
  | 'Trợ giảng'
  | 'Học viên'

export type PagedResult<T> = {
  items: T[]
  page: number
  pageSize: number
  totalItems: number
  totalPages: number
}

export type ApiProblem = {
  status: number
  message?: string
  errors?: string[]
}

// ===== AUTH =====
export type CurrentUser = {
  id: number
  username: string
  email: string
  role: VaiTro
}

export type LoginResponse = {
  accessToken: string
  tokenType: string
  expiresAt: string
  user: CurrentUser
}

/** Hồ sơ cá nhân (giáo viên / trợ giảng / học viên) — chỉ lấy phần cần hiển thị. */
export type HoSoCaNhan = {
  // Chung
  hoTen?: string
  email?: string | null
  soDienThoai?: string | null
  gioiTinh?: string | null
  ngaySinh?: string | null
  diaChi?: string | null

  // Học viên (/student/profile)
  maHocVien?: string
  mucTieuHocTap?: string | null
  trinhDoHienTai?: string | null
  trangThaiHocTap?: string
  ngayNhapHoc?: string | null

  // Giáo viên / trợ giảng (/teacher/profile)
  maGiaoVien?: string
  loaiGiaoVien?: string
  chuyenMon?: string | null
  soNamKinhNghiem?: number | null
  ngayVaoLam?: string | null
  trangThai?: string
}

// ===== HỌC VIÊN =====
export type StudentListItem = {
  maHocVien: string
  hoTen: string
  ngaySinh: string
  gioiTinh?: string | null
  email?: string | null
  soDienThoai?: string | null
  trangThaiHocTap: string
  ngayNhapHoc?: string | null
}

// ===== LỚP HỌC =====
export type ClassListItem = {
  maLopHoc: number
  maLop: string
  tenLop: string
  maKhoaHoc: number
  tenKhoaHoc: string
  maNhanVienHocVu: string
  tenNhanVienHocVu: string
  ngayBatDau: string
  ngayKetThucDuKien: string
  ngayKetThucThucTe?: string | null
  siSoToiDa: number
  siSoHienTai: number
  trangThai: string
}

// ===== CA HỌC =====
export type SessionListItem = {
  maCaHoc: number
  maLopHoc: number
  maLop: string
  tenLop: string
  maKhoaHoc: number
  tenKhoaHoc: string
  maLichHoc?: number | null
  maPhongHoc: number
  maPhong: string
  tenPhong: string
  maGiaoVienThucTe?: string | null
  tenGiaoVienThucTe?: string | null
  maTroGiangThucTe?: string | null
  tenTroGiangThucTe?: string | null
  ngayHoc: string
  gioBatDau: string
  gioKetThuc: string
  noiDungBaiHoc?: string | null
  trangThai: string
  lyDoThayDoi?: string | null
  soDiemDanh: number
}

export type AttendanceRow = {
  maHocVien: string
  hoTen: string
  maDangKy: number
  daDiemDanh: boolean
  maDiemDanh?: number | null
  trangThaiDiemDanh?: string | null
  ghiChu?: string | null
  thoiGianDiemDanh?: string | null
  tenTaiKhoanThucHien?: string | null
}

export type AttendanceSheet = {
  maCaHoc: number
  maLopHoc: number
  maLop: string
  tenLop: string
  maPhongHoc: number
  maPhong: string
  tenPhong: string
  ngayHoc: string
  gioBatDau: string
  gioKetThuc: string
  trangThaiCaHoc: string
  coTheDiemDanh: boolean
  lyDoKhongTheDiemDanh?: string | null
  hocViens: AttendanceRow[]
}

// ===== LỚP CỦA GIÁO VIÊN =====
export type TeacherClass = {
  maLopHoc: number
  maLop: string
  tenLop: string
  maKhoaHoc: number
  tenKhoaHoc: string
  siSo: number
  siSoToiDa: number
  trangThai: string
  ngayBatDau: string
  ngayKetThucDuKien: string
  vaiTroPhanCong: string
}

export type TeacherClassStudent = {
  maHocVien: string
  hoTen: string
  maDangKy: number
}

// ===== ĐIỂM =====
export type GradeComponent = {
  maLoaiDiem: number
  tenLoaiDiem: string
  trongSo: number
  diemToiDa: number
  thuTu: number
}

export type ComponentScore = {
  maLoaiDiem: number
  tenLoaiDiem: string
  trongSo: number
  diemToiDa: number
  diem?: number | null
}

export type StudentCourseGrade = {
  maDangKy: number
  maLopHoc: number
  maLop: string
  tenLop: string
  maKhoaHoc: number
  tenKhoaHoc: string
  ngayBatDau: string
  ngayKetThucDuKien: string
  ngayXepLop?: string | null
  tongTrongSo: number
  canhBaoTrongSo: boolean
  duDieuKien: boolean
  diemTongKet?: number | null
  thanhPhan: ComponentScore[]
  thanhPhanConThieu: string[]
}

export type AcademicStudentRow = {
  maDangKy: number
  maHocVien: string
  hoTen: string
  duDieuKien: boolean
  diemTongKet?: number | null
  thanhPhan: ComponentScore[]
  thanhPhanConThieu: string[]
}

export type AcademicReport = {
  maLopHoc: number
  maLop: string
  tenLop: string
  maKhoaHoc: number
  tenKhoaHoc: string
  soHocVien: number
  tongTrongSo: number
  canhBaoTrongSo: boolean
  thanhPhan: GradeComponent[]
  soDuDieuKien: number
  soThieuDiem: number
  diemTongKetTrungBinh?: number | null
  diemCaoNhat?: number | null
  diemThapNhat?: number | null
  hocViens: AcademicStudentRow[]
}

/** Một bản ghi điểm đã lưu (GET /api/admin/grades). */
export type GradeItem = {
  maDiemSo: number
  maDangKy: number
  maHocVien: string
  hoTenHocVien: string
  maLopHoc?: number | null
  maLop?: string | null
  tenLop?: string | null
  maLoaiDiem: number
  tenLoaiDiem: string
  diemToiDa: number
  diem: number
  nhanXet?: string | null
  trangThaiKetQua?: string | null
  maGiaoVien: string
  tenGiaoVien: string
  ngayCham: string
  ngayCapNhat?: string | null
}

// ===== ĐIỂM DANH CỦA HỌC VIÊN =====
export type StudentAttendanceItem = {
  maDiemDanh: number
  maCaHoc: number
  maLop: string
  tenLop: string
  tenKhoaHoc: string
  ngayHoc: string
  gioBatDau: string
  gioKetThuc: string
  trangThaiDiemDanh: string
  ghiChu?: string | null
  thoiGianDiemDanh: string
  trangThaiCaHoc: string
}

export type StudentAttendanceHistory = {
  maHocVien: string
  hoTen: string
  tongCaHoc: number
  coMat: number
  vang: number
  vangCoPhep: number
  diTre: number
  tiLeCoMat: number
  items: StudentAttendanceItem[]
}

// ===== HỌC PHÍ =====
export type TuitionItem = {
  maHocPhi: number
  maDangKy: number
  maHocVien: string
  hoTenHocVien: string
  maKhoaHoc: number
  tenKhoaHoc: string
  maLopHoc?: number | null
  maLop?: string | null
  tenLop?: string | null
  soTienGoc: number
  soTienGiam: number
  soTienPhaiDong: number
  soTienDaDong: number
  soTienConThieu: number
  hanThanhToan?: string | null
  trangThaiHocPhi: string
  ngayTao: string
  soGiaoDich: number
  quaHan: boolean
  conNo: boolean
}

export type ReceiptBrief = {
  maBienNhan: number
  soBienNhan: string
  ngayLap: string
  trangThai: string
}

export type Payment = {
  maThanhToan: number
  maHocPhi: number
  maDangKy: number
  maHocVien: string
  hoTenHocVien: string
  maKhoaHoc: number
  tenKhoaHoc: string
  soTien: number
  ngayThanhToan: string
  phuongThucThanhToan: string
  maGiaoDich?: string | null
  loaiGiaoDich: string
  trangThai: string
  ghiChu?: string | null
  maNhanVienKeToan: string
  tenNhanVienKeToan: string
  bienNhan?: ReceiptBrief | null
}

export type FinanceReport = {
  tongSoHocPhi: number
  tongPhaiThu: number
  tongDaThu: number
  tongConThieu: number
  soChuaThanhToan: number
  soThanhToanMotPhan: number
  soDaThanhToan: number
  soQuaHan: number
  soHocVienConNo: number
  danhSachConNo: Array<{
    maHocVien: string
    hoTen: string
    soHocPhiConNo: number
    tongConThieu: number
  }>
  thongKeGiaoDichTheoPhuongThuc: Array<{ nhan: string; soLuong: number; tongTien: number }>
  thongKeGiaoDichTheoTrangThai: Array<{ nhan: string; soLuong: number; tongTien: number }>
}

// ===== THÔNG BÁO =====
export type NotificationItem = {
  maThongBao: number
  maTaiKhoanGui: number
  tenDangNhapNguoiGui: string
  maTaiKhoanNhan?: number | null
  tenDangNhapNguoiNhan?: string | null
  maLopHoc?: number | null
  maLop?: string | null
  tenLop?: string | null
  tieuDe: string
  noiDung: string
  loaiThongBao?: string | null
  thoiGianGui: string
  trangThai: string
  phamVi: string
}

export type NotificationSummary = {
  tongSo: number
  soChuaDoc: number | null
  ghiChu: string
}

// ===== KHẢO SÁT =====
export type SurveyQuestion = {
  maCauHoi: number
  maKhaoSat: number
  noiDungCauHoi: string
  loaiCauHoi: string
  thuTu: number
  batBuoc: boolean
  trangThai: string
  soCauTraLoi: number
}

export type SurveyItem = {
  maKhaoSat: number
  tenKhaoSat: string
  moTa?: string | null
  ngayBatDau: string
  ngayKetThuc?: string | null
  trangThai: string
  soCauHoi: number
  soPhanHoi: number
  dangMo: boolean
}

export type SurveyDetail = SurveyItem & {
  cauHois: SurveyQuestion[]
}

export type MySurvey = SurveyItem & {
  maPhanHoiKhaoSat?: number | null
  thoiGianTraLoi?: string | null
  daTraLoi: boolean
  cauHois: SurveyQuestion[]
}

export type SurveyAnswer = {
  maCauTraLoi: number
  maCauHoi: number
  noiDungCauHoi: string
  loaiCauHoi: string
  giaTriDanhGia?: number | null
  noiDungTraLoi?: string | null
}

export type SurveyResponse = {
  maPhanHoiKhaoSat: number
  maKhaoSat: number
  tenKhaoSat: string
  maHocVien: string
  hoTenHocVien: string
  thoiGianGui: string
  diemDanhGiaChung?: number | null
  ghiChu?: string | null
  cauTraLois: SurveyAnswer[]
}

export type QuestionStatistic = {
  maCauHoi: number
  noiDungCauHoi: string
  loaiCauHoi: string
  thuTu: number
  batBuoc: boolean
  soLuotTraLoi: number
  giaTriTrungBinh?: number | null
  phanBo: Array<{ giaTri: number; soLuong: number }>
  noiDungTuLuan: string[]
}

export type SurveyStatistics = {
  maKhaoSat: number
  tenKhaoSat: string
  trangThai: string
  ngayBatDau: string
  ngayKetThuc?: string | null
  tongSoCauHoi: number
  tongSoPhanHoi: number
  diemDanhGiaChungTrungBinh?: number | null
  cauHois: QuestionStatistic[]
}

// ===== BÁO CÁO =====
export type ReportOverview = {
  tongTaiKhoan: number
  tongHocVien: number
  soHocVienDangHoc: number
  tongGiaoVien: number
  tongNhanVien: number
  tongChuongTrinh: number
  tongCapDo: number
  tongKhoaHoc: number
  tongLopHoc: number
  soLopDangHoc: number
  dangKy: {
    tongSo: number
    choXuLy: number
    daXepLop: number
    hoanThanh: number
    daHuy: number
  }
  caHoc: {
    tongSo: number
    chuaDienRa: number
    dangDienRa: number
    daHoanThanh: number
    daHuy: number
    sapDienRa: number
  }
  diemDanh: {
    tongLuot: number
    coMat: number
    vang: number
    vangCoPhep: number
    diTre: number
    tiLeCoMat: number
  }
  hocPhi: {
    tongPhaiThu: number
    tongDaThu: number
    tongConThieu: number
    soHocPhiConNo: number
    soHocVienConNo: number
  }
  khaoSat: {
    tongSo: number
    soHoatDong: number
    tongCauHoi: number
    tongPhanHoi: number
  }
}

export type AttendanceStudentRow = {
  maHocVien: string
  hoTen: string
  tongLuot: number
  coMat: number
  vang: number
  vangCoPhep: number
  diTre: number
  tiLeCoMat: number
  tiLeVang: number
  vangCao: boolean
}

export type AttendanceReport = {
  maLopHoc: number
  maLop: string
  tenLop: string
  maKhoaHoc: number
  tenKhoaHoc: string
  tuNgay?: string | null
  denNgay?: string | null
  soCaHocDaDiemDanh: number
  nguongVang: number
  tiLeCoMatToanLop: number
  hocViens: AttendanceStudentRow[]
  hocVienVangCao: AttendanceStudentRow[]
}

export type SurveyReport = {
  tongKhaoSat: number
  soHoatDong: number
  soTamDung: number
  soDaKetThuc: number
  tongCauHoi: number
  tongPhanHoi: number
  diemDanhGiaChungTrungBinh?: number | null
  tongHocVien: number
  khaoSats: Array<{
    maKhaoSat: number
    tenKhaoSat: string
    trangThai: string
    ngayBatDau: string
    ngayKetThuc?: string | null
    soCauHoi: number
    soPhanHoi: number
    diemTrungBinh?: number | null
    tiLePhanHoi?: number | null
  }>
}
