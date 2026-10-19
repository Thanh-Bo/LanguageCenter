import type { IconName } from '../components/BieuTuong'
import type { VaiTro } from './api/types'

const { quanTriVien, nhanVienHocVu, nhanVienTuVan, nhanVienKeToan, giaoVien, troGiang, hocVien } =
  {
    quanTriVien: 'Quản trị viên' as const,
    nhanVienHocVu: 'Nhân viên học vụ' as const,
    nhanVienTuVan: 'Nhân viên tư vấn' as const,
    nhanVienKeToan: 'Nhân viên kế toán' as const,
    giaoVien: 'Giáo viên' as const,
    troGiang: 'Trợ giảng' as const,
    hocVien: 'Học viên' as const,
  }

/** 4 vai trò dùng giao diện quản trị (backend: các controller /api/admin/*) */
export const NHOM_QUAN_TRI: VaiTro[] = [
  quanTriVien,
  nhanVienHocVu,
  nhanVienTuVan,
  nhanVienKeToan,
]

/** 2 vai trò dùng giao diện giáo viên */
export const NHOM_GIAO_VIEN: VaiTro[] = [giaoVien, troGiang]

export const NHOM_HOC_VIEN: VaiTro[] = [hocVien]

export type MucMenu = {
  nhan: string
  duong: string
  icon: IconName
  /** Chỉ hiện với các vai trò này; bỏ trống = mọi vai trò trong nhóm */
  chiVaiTro?: VaiTro[]
  /** Khớp chính xác đường dẫn (dùng cho trang index) */
  chinhXac?: boolean
}

const MENU_QUAN_TRI: MucMenu[] = [
  { nhan: 'Tổng quan', duong: '/admin', icon: 'grid', chinhXac: true },
  {
    nhan: 'Học viên',
    duong: '/admin/hoc-vien',
    icon: 'users',
    chiVaiTro: [quanTriVien],
  },
  {
    nhan: 'Lớp học',
    duong: '/admin/lop-hoc',
    icon: 'book',
    chiVaiTro: [quanTriVien, nhanVienHocVu],
  },
  {
    nhan: 'Học phí',
    duong: '/admin/hoc-phi',
    icon: 'wallet',
    chiVaiTro: [quanTriVien, nhanVienKeToan, nhanVienTuVan],
  },
  {
    nhan: 'Thông báo',
    duong: '/admin/thong-bao',
    icon: 'bell',
    chiVaiTro: [quanTriVien, nhanVienHocVu],
  },
  {
    nhan: 'Khảo sát',
    duong: '/admin/khao-sat',
    icon: 'chart',
    chiVaiTro: [quanTriVien, nhanVienHocVu],
  },
  {
    nhan: 'Báo cáo',
    duong: '/admin/bao-cao',
    icon: 'chart',
    chiVaiTro: [quanTriVien, nhanVienHocVu, nhanVienKeToan],
  },
  { nhan: 'Cài đặt', duong: '/admin/cai-dat', icon: 'settings' },
]

const MENU_GIAO_VIEN: MucMenu[] = [
  { nhan: 'Tổng quan', duong: '/giaovien', icon: 'grid', chinhXac: true },
  { nhan: 'Lớp của tôi', duong: '/giaovien/lop-cua-toi', icon: 'users' },
  { nhan: 'Lịch dạy', duong: '/giaovien/lich-day', icon: 'calendar' },
  { nhan: 'Điểm danh', duong: '/giaovien/diem-danh', icon: 'check' },
  { nhan: 'Bảng điểm', duong: '/giaovien/bang-diem', icon: 'chart' },
  { nhan: 'Thông báo', duong: '/giaovien/thong-bao', icon: 'bell' },
  { nhan: 'Cài đặt', duong: '/giaovien/cai-dat', icon: 'settings' },
]

const MENU_HOC_VIEN: MucMenu[] = [
  { nhan: 'Tổng quan', duong: '/hocvien', icon: 'grid', chinhXac: true },
  { nhan: 'Lịch học', duong: '/hocvien/lich-hoc', icon: 'calendar' },
  { nhan: 'Điểm số', duong: '/hocvien/diem-so', icon: 'chart' },
  { nhan: 'Học phí', duong: '/hocvien/hoc-phi', icon: 'wallet' },
  { nhan: 'Khảo sát', duong: '/hocvien/khao-sat', icon: 'sparkles' },
  { nhan: 'Thông báo', duong: '/hocvien/thong-bao', icon: 'bell' },
  { nhan: 'Cài đặt', duong: '/hocvien/cai-dat', icon: 'settings' },
]

/** Trang chủ tương ứng với vai trò. */
export function duongDanTheoVaiTro(vaiTro: VaiTro): string {
  if (NHOM_GIAO_VIEN.includes(vaiTro)) return '/giaovien'
  if (NHOM_HOC_VIEN.includes(vaiTro)) return '/hocvien'
  return '/admin'
}

/** Menu dành cho một vai trò cụ thể (đã lọc theo quyền). */
export function menuTheoVaiTro(vaiTro: VaiTro): MucMenu[] {
  const nguon = NHOM_GIAO_VIEN.includes(vaiTro)
    ? MENU_GIAO_VIEN
    : NHOM_HOC_VIEN.includes(vaiTro)
      ? MENU_HOC_VIEN
      : MENU_QUAN_TRI

  return nguon.filter((muc) => !muc.chiVaiTro || muc.chiVaiTro.includes(vaiTro))
}

/** Tiền tố khu vực của vai trò — dùng cho route guard. */
export function tienToTheoVaiTro(vaiTro: VaiTro): string {
  return duongDanTheoVaiTro(vaiTro)
}
