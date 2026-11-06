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
import { AdminLichHoc } from './pages/admin/AdminLichHoc'
import {
  GVBangDiem,
  GVDiemDanh,
  GVLichDay,
  GVLopCuaToi,
  GVThongBao,
} from './pages/giaovien/TeacherPages'
import { GVTongQuan } from './pages/giaovien/TongQuanGiaoVien'
import { GVLopChiTiet } from './pages/giaovien/LopChiTiet'
import {
  HVDiemSo,
  HVHocPhi,
  HVKhaoSat,
  HVLichHoc,
  HVThongBao,
} from './pages/hocvien/StudentPages'
import { HVTongQuan } from './pages/hocvien/TongQuanHocVien'
import { HVLopChiTiet, HVLopCuaToi } from './pages/hocvien/LopCuaToi'
import { NHOM_DIEM_DANH, NHOM_GIAO_VIEN, NHOM_HOC_VIEN, NHOM_QUAN_TRI } from './lib/roles'

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
          { path: 'lich-hoc', Component: AdminLichHoc },
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
          { path: 'lop-cua-toi/:maLop', Component: GVLopChiTiet },
          { path: 'lich-day', Component: GVLichDay },
          {
            // Điểm danh CHỈ dành cho Trợ giảng — Giáo viên vào sẽ bị đẩy về trang chủ.
            element: <BaoVe choPhep={NHOM_DIEM_DANH} />,
            children: [{ path: 'diem-danh', Component: GVDiemDanh }],
          },
          // /bang-diem = danh sách lớp · /bang-diem/:maLop (vd IELTS-A-01) = bảng điểm của lớp đó.
          { path: 'bang-diem', Component: GVBangDiem },
          { path: 'bang-diem/:maLop', Component: GVBangDiem },
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
          { path: 'lop-cua-toi', Component: HVLopCuaToi },
          { path: 'lop-cua-toi/:maLop', Component: HVLopChiTiet },
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