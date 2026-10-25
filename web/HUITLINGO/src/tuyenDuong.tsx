import { createBrowserRouter, Navigate } from 'react-router'
import BoCuc from './components/BoCuc'
import BaoVe from './components/BaoVe'
import PortalLayout from './components/layout/PortalLayout'
import TrangChu from './pages/TrangChu'
import KhoaHoc from './pages/KhoaHoc'
import GiaoVien from './pages/GiaoVien'
import DangNhap from './pages/DangNhap'
import CaiDat from './pages/CaiDat'
import {
  AdminHocPhi,
  AdminHocVien,
  AdminLopHoc,
  AdminTongQuan,
} from './pages/admin/AdminPages'
import { AdminBaoCao, AdminKhaoSat, AdminThongBao } from './pages/admin/AdminContent'
import {
  GVBangDiem,
  GVDiemDanh,
  GVLichDay,
  GVLopCuaToi,
  GVThongBao,
} from './pages/giaovien/TeacherPages'
import { GVTongQuan } from './pages/giaovien/TongQuanGiaoVien'
import {
  HVDiemSo,
  HVHocPhi,
  HVKhaoSat,
  HVLichHoc,
  HVThongBao,
} from './pages/hocvien/StudentPages'
import { HVTongQuan } from './pages/hocvien/TongQuanHocVien'
import { HVTienDoKhoaHoc } from './pages/hocvien/TienDoKhoaHoc'
import { NHOM_GIAO_VIEN, NHOM_HOC_VIEN, NHOM_QUAN_TRI } from './lib/roles'

export const router = createBrowserRouter([
  // ============ PUBLIC (Navbar + Footer) ============
  {
    path: '/',
    Component: BoCuc,
    children: [
      { index: true, Component: TrangChu },
      { path: 'khoahoc', Component: KhoaHoc },
      { path: 'giaovien-public', Component: GiaoVien },
    ],
  },

  // ============ AUTH ============
  { path: '/dangnhap', Component: DangNhap },

  // ============ QUẢN TRỊ (4 vai trò) ============
  {
    element: <BaoVe choPhep={NHOM_QUAN_TRI} />,
    children: [
      {
        path: '/admin',
        Component: PortalLayout,
        children: [
          { index: true, Component: AdminTongQuan },
          { path: 'hoc-vien', Component: AdminHocVien },
          { path: 'lop-hoc', Component: AdminLopHoc },
          { path: 'hoc-phi', Component: AdminHocPhi },
          { path: 'thong-bao', Component: AdminThongBao },
          { path: 'khao-sat', Component: AdminKhaoSat },
          { path: 'bao-cao', Component: AdminBaoCao },
          { path: 'cai-dat', Component: CaiDat },
        ],
      },
    ],
  },

  // ============ GIÁO VIÊN / TRỢ GIẢNG ============
  {
    element: <BaoVe choPhep={NHOM_GIAO_VIEN} />,
    children: [
      {
        path: '/giaovien',
        Component: PortalLayout,
        children: [
          { index: true, Component: GVTongQuan },
          { path: 'lop-cua-toi', Component: GVLopCuaToi },
          { path: 'lich-day', Component: GVLichDay },
          { path: 'diem-danh', Component: GVDiemDanh },
          { path: 'bang-diem', Component: GVBangDiem },
          { path: 'thong-bao', Component: GVThongBao },
          { path: 'cai-dat', Component: CaiDat },
        ],
      },
    ],
  },

  // ============ HỌC VIÊN ============
  {
    element: <BaoVe choPhep={NHOM_HOC_VIEN} />,
    children: [
      {
        path: '/hocvien',
        Component: PortalLayout,
        children: [
          { index: true, Component: HVTongQuan },
          { path: 'khoa-hoc/:maLopHoc', Component: HVTienDoKhoaHoc },
          { path: 'lich-hoc', Component: HVLichHoc },
          { path: 'diem-so', Component: HVDiemSo },
          { path: 'hoc-phi', Component: HVHocPhi },
          { path: 'khao-sat', Component: HVKhaoSat },
          { path: 'thong-bao', Component: HVThongBao },
          { path: 'cai-dat', Component: CaiDat },
        ],
      },
    ],
  },

  // ============ FALLBACK ============
  { path: '*', element: <Navigate to="/" replace /> },
])