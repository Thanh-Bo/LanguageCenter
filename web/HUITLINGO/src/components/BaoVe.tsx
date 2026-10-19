// Component bảo vệ route — chặn truy cập trái phép theo vai trò.
import { Navigate, Outlet, useLocation } from 'react-router'
import { dungXacThuc, duongDanTheoVaiTro } from '../contexts/NguCanhXacThuc'
import type { VaiTro } from '../lib/api/types'

type Props = {
  choPhep?: VaiTro[]
}

export default function BaoVe({ choPhep }: Props) {
  const { nguoiDung, dangTai } = dungXacThuc()
  const viTri = useLocation()

  if (dangTai) {
    return (
      <div className="grid min-h-screen place-items-center bg-slate-50">
        <div className="flex flex-col items-center gap-3 text-slate-500">
          <span className="size-8 animate-spin rounded-full border-2 border-blue-200 border-t-blue-600" />
          Đang kiểm tra đăng nhập…
        </div>
      </div>
    )
  }

  if (!nguoiDung) {
    return <Navigate to="/dangnhap" state={{ from: viTri.pathname }} replace />
  }

  if (choPhep && !choPhep.includes(nguoiDung.vaiTro)) {
    return <Navigate to={duongDanTheoVaiTro(nguoiDung.vaiTro)} replace />
  }

  return <Outlet />
}