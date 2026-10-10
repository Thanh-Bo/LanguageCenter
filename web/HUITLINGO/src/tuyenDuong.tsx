import { createBrowserRouter, Navigate } from 'react-router'
import BoCuc from './components/BoCuc'
import BaoVe from './components/BaoVe'
import TrangChu from './pages/TrangChu'
import KhoaHoc from './pages/KhoaHoc'
import GiaoVien from './pages/GiaoVien'
import HocPhi from './pages/HocPhi'
import DangNhap from './pages/DangNhap'
import CongQuanLy from './pages/CongQuanLy'
import GiaoVienPortal from './pages/giaovien/GiaoVienPortal'
import HocVienPortal from './pages/hocvien/HocVienPortal'

export const router = createBrowserRouter([
  // ============ PUBLIC (có Navbar + Footer) ============
  {
    path: '/',
    Component: BoCuc,
    children: [
      { index: true, Component: TrangChu },
      { path: 'khoahoc', Component: KhoaHoc },
      { path: 'giaovien-public', Component: GiaoVien },
      { path: 'hocphi', Component: HocPhi },
    ],
  },

  // ============ AUTH ============
  { path: '/dangnhap', Component: DangNhap },

  // ============ PROTECTED — chỉ Admin ============
  {
    element: <BaoVe choPhep={['Quản trị viên']} />,
    children: [
      { path: '/admin', element: <CongQuanLy vaiTroMacDinh="Quản trị viên" /> },
    ],
  },

  // ============ PROTECTED — chỉ Giáo viên ============
  {
    element: <BaoVe choPhep={['Giáo viên']} />,
    children: [{ path: '/giaovien', Component: GiaoVienPortal }],
  },

  // ============ PROTECTED — chỉ Học viên ============
  {
    element: <BaoVe choPhep={['Học viên']} />,
    children: [{ path: '/hocvien', Component: HocVienPortal }],
  },

  // ============ FALLBACK ============
  { path: '*', element: <Navigate to="/" replace /> },
])