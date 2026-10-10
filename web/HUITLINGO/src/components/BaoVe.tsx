// src/components/BaoVe.tsx
// Component bảo vệ route — chặn truy cập trái phép
import { Navigate, Outlet, useLocation } from 'react-router'
import { dungXacThuc, duongDanTheoVaiTro } from '../contexts/NguCanhXacThuc'
import type { ApiUser } from '../lib/goiAPI'

type Props = {
  choPhep?: ApiUser['vaiTro'][]
}

export default function BaoVe({ choPhep }: Props) {
  const { nguoiDung, dangTai } = dungXacThuc()
  const viTri = useLocation()

  // Đang kiểm tra đăng nhập
  if (dangTai) {
    return (
      <div className="grid min-h-screen place-items-center bg-slate-50">
        <div className="text-slate-500">Đang kiểm tra đăng nhập…</div>
      </div>
    )
  }

  // Chưa đăng nhập → chuyển về trang đăng nhập
  if (!nguoiDung) {
    return (
      <Navigate to="/dangnhap" state={{ from: viTri.pathname }} replace />
    )
  }

  // Đã đăng nhập nhưng không đúng vai trò → chuyển về portal đúng
  if (choPhep && !choPhep.includes(nguoiDung.vaiTro)) {
    return <Navigate to={duongDanTheoVaiTro(nguoiDung.vaiTro)} replace />
  }

  // OK → render children
  return <Outlet />
}