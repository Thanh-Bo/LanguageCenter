import { useState } from 'react'
import { Link, NavLink, Outlet, useNavigate } from 'react-router'
import { Icon } from '../BieuTuong'
import { dungXacThuc } from '../../contexts/NguCanhXacThuc'
import { menuTheoVaiTro } from '../../lib/roles'

/**
 * Khung giao diện dùng chung cho cả 3 khu vực (/admin, /giaovien, /hocvien).
 * Menu được lọc theo vai trò thật của người đăng nhập.
 */
export default function PortalLayout() {
  const { nguoiDung, dangXuat } = dungXacThuc()
  const navigate = useNavigate()
  const [moMenu, setMoMenu] = useState(false)

  if (!nguoiDung) return null

  const menu = menuTheoVaiTro(nguoiDung.vaiTro)

  const xuLyDangXuat = () => {
    dangXuat()
    navigate('/dangnhap', { replace: true })
  }

  return (
    <div className="min-h-screen bg-slate-50 text-slate-800">
      {moMenu && (
        <button
          className="fixed inset-0 z-30 bg-slate-950/30 lg:hidden"
          onClick={() => setMoMenu(false)}
          aria-label="Đóng menu"
        />
      )}

      <aside
        className={`fixed inset-y-0 left-0 z-40 flex w-72 flex-col border-r border-slate-200 bg-white transition-transform lg:translate-x-0 ${
          moMenu ? 'translate-x-0' : '-translate-x-full'
        }`}
      >
        <div className="flex h-18 items-center justify-between border-b border-slate-100 px-6">
          <Link to="/" className="flex items-center gap-3 text-slate-900 no-underline">
            <span className="grid size-10 place-items-center rounded-xl bg-blue-600 text-white">
              <Icon name="logo" className="size-5" />
            </span>
            <span>
              <strong className="block text-lg font-bold">
                <span>HUIT </span>
                <span className="text-blue-600">LingoHub</span>
              </strong>
              <small className="block text-[10px] font-bold uppercase tracking-[.18em] text-slate-400">
                Hệ thống quản lý
              </small>
            </span>
          </Link>
          <button
            onClick={() => setMoMenu(false)}
            className="text-slate-400 lg:hidden"
            aria-label="Đóng"
          >
            <Icon name="close" className="size-5" />
          </button>
        </div>

        <nav className="flex-1 space-y-1 overflow-y-auto px-4 py-5">
          {menu.map((muc) => (
            <NavLink
              key={muc.duong}
              to={muc.duong}
              end={muc.chinhXac}
              onClick={() => setMoMenu(false)}
              className={({ isActive }) =>
                `flex items-center gap-3 rounded-xl px-3 py-3 text-sm font-semibold transition ${
                  isActive
                    ? 'bg-blue-600 text-white shadow-sm shadow-blue-200'
                    : 'text-slate-500 hover:bg-slate-50 hover:text-slate-900'
                }`
              }
            >
              <Icon name={muc.icon} className="size-5" />
              <span className="flex-1">{muc.nhan}</span>
            </NavLink>
          ))}
        </nav>

        <div className="m-4 rounded-2xl bg-slate-900 p-4 text-white">
          <div className="flex items-center gap-3">
            <span className="grid size-10 place-items-center rounded-full bg-blue-500 text-sm font-bold">
              {nguoiDung.avatar}
            </span>
            <span className="min-w-0 flex-1">
              <strong className="block truncate text-sm">{nguoiDung.hoTen}</strong>
              <small className="text-slate-400">{nguoiDung.vaiTro}</small>
            </span>
            <button
              onClick={xuLyDangXuat}
              className="text-slate-400 transition hover:text-white"
              title="Đăng xuất"
            >
              <Icon name="logout" className="size-5" />
            </button>
          </div>
        </div>
      </aside>

      <div className="lg:pl-72">
        <header className="sticky top-0 z-20 flex h-18 items-center gap-3 border-b border-slate-200 bg-white/90 px-4 backdrop-blur-xl sm:px-7">
          <button
            onClick={() => setMoMenu(true)}
            className="text-slate-500 lg:hidden"
            aria-label="Mở menu"
          >
            <Icon name="menu" className="size-6" />
          </button>

          <p className="hidden text-sm font-semibold text-slate-500 sm:block">
            Xin chào, <span className="text-slate-900">{nguoiDung.hoTen}</span>
          </p>

          <div className="ml-auto flex items-center gap-3">
            <Link
              to="/"
              className="hidden rounded-xl border border-slate-200 px-4 py-2 text-sm font-semibold text-slate-600 transition hover:border-blue-200 hover:text-blue-600 sm:block"
            >
              Về trang chủ
            </Link>
            <span className="grid size-10 place-items-center rounded-xl bg-blue-600 text-sm font-bold text-white">
              {nguoiDung.avatar}
            </span>
          </div>
        </header>

        <main className="p-4 sm:p-7">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
