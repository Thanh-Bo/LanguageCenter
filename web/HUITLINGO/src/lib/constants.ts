// Giá trị trạng thái PHẢI khớp CHÍNH XÁC dữ liệu trong database.
// Không tự dịch sang tiếng Anh, không tự đổi dấu.

export const VAI_TRO = {
  quanTriVien: 'Quản trị viên',
  nhanVienHocVu: 'Nhân viên học vụ',
  nhanVienTuVan: 'Nhân viên tư vấn',
  nhanVienKeToan: 'Nhân viên kế toán',
  giaoVien: 'Giáo viên',
  troGiang: 'Trợ giảng',
  hocVien: 'Học viên',
} as const

export const TRANG_THAI_HOC_TAP = ['Đang tư vấn', 'Đang học', 'Bảo lưu', 'Đã nghỉ'] as const

export const TRANG_THAI_LOP = [
  'Chuẩn bị mở',
  'Đang học',
  'Đã kết thúc',
  'Đã hủy',
] as const

export const TRANG_THAI_CA_HOC = [
  'Chưa diễn ra',
  'Đang diễn ra',
  'Đã hoàn thành',
  'Đã hủy',
] as const

export const TRANG_THAI_DIEM_DANH = ['Có mặt', 'Vắng', 'Vắng có phép', 'Đi trễ'] as const

export const TRANG_THAI_HOC_PHI = [
  'Chưa thanh toán',
  'Thanh toán một phần',
  'Đã thanh toán',
] as const

export const TRANG_THAI_THANH_TOAN = ['Thành công', 'Đã hủy'] as const

export const PHUONG_THUC_THANH_TOAN = ['Tiền mặt', 'Chuyển khoản'] as const

export const TRANG_THAI_KHAO_SAT = ['Hoạt động', 'Tạm dừng', 'Đã kết thúc'] as const

export const LOAI_CAU_HOI = {
  danhGia: 'Đánh giá 1-5',
  tuLuan: 'Tự luận',
} as const

export const LOAI_THONG_BAO = ['Lịch học', 'Học tập', 'Học phí', 'Khác'] as const

export type VaiTroValue =
  (typeof VAI_TRO)[keyof typeof VAI_TRO]
