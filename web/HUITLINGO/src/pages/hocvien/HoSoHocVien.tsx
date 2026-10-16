import { useState } from 'react'
import { Icon } from '../../components/BieuTuong'
import { dungXacThuc } from '../../contexts/NguCanhXacThuc'
import ModalDoiMatKhau from '../../components/ModalDoiMatKhau'

type Props = {
  onChuyenTrang?: (trang: string) => void
}

export default function HoSoHocVien({ onChuyenTrang }: Props) {
  const { nguoiDung } = dungXacThuc()
  const [moModalDoiMK, setMoModalDoiMK] = useState(false)

  if (!nguoiDung) return null

  return (
    <div className="space-y-6">
      <div>
        <p className="mb-1 text-sm font-semibold text-blue-600">Tài khoản</p>
        <h1 className="text-3xl font-bold text-slate-900">Hồ sơ cá nhân</h1>
        <p className="mt-2 text-sm text-slate-500">
          Thông tin cá nhân và tài khoản học viên của bạn.
        </p>
      </div>

      <div className="grid gap-6 lg:grid-cols-[.85fr_1.15fr]">
        {/* Card bên trái */}
        <article className="rounded-2xl border border-slate-200 bg-white p-6 text-center shadow-sm">
          <span className="mx-auto grid size-24 place-items-center rounded-full bg-blue-600 text-3xl font-bold text-white">
            {nguoiDung.avatar ?? 'U'}
          </span>
          <h2 className="mt-4 text-xl font-bold text-slate-900">
            {nguoiDung.hoTen}
          </h2>
          <p className="mt-1 text-sm text-slate-500">{nguoiDung.email}</p>
          <span className="mt-3 inline-block rounded-full bg-blue-50 px-3 py-1 text-xs font-bold text-blue-700">
            {nguoiDung.vaiTro}
          </span>

          <div className="mt-6 grid grid-cols-2 gap-3 border-t border-slate-100 pt-6 text-left">
            <div>
              <p className="text-xs font-bold uppercase text-slate-400">
                Mã học viên
              </p>
              <p className="mt-1 font-bold text-slate-900">{nguoiDung.id}</p>
            </div>
            <div>
              <p className="text-xs font-bold uppercase text-slate-400">
                Trạng thái
              </p>
              <p className="mt-1 font-bold text-emerald-600">Đang học</p>
            </div>
          </div>
        </article>

        {/* Card bên phải */}
        <div className="space-y-6">
          <article className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
            <h3 className="text-lg font-bold">Thông tin liên hệ</h3>
            <div className="mt-4 space-y-4">
              <div className="flex items-center gap-3 text-sm">
                <span className="grid size-10 place-items-center rounded-xl bg-slate-100">
                  <Icon name="mail" className="size-5 text-slate-500" />
                </span>
                <div>
                  <p className="text-xs font-bold uppercase text-slate-400">
                    Email
                  </p>
                  <p className="font-semibold text-slate-900">
                    {nguoiDung.email}
                  </p>
                </div>
              </div>
              <div className="flex items-center gap-3 text-sm">
                <span className="grid size-10 place-items-center rounded-xl bg-slate-100">
                  <Icon name="lock" className="size-5 text-slate-500" />
                </span>
                <div>
                  <p className="text-xs font-bold uppercase text-slate-400">
                    Mật khẩu
                  </p>
                  <p className="font-semibold text-slate-900">••••••••</p>
                </div>
                <button
                  onClick={() => setMoModalDoiMK(true)}
                  className="ml-auto rounded-lg border border-slate-200 px-3 py-1.5 text-xs font-semibold text-slate-600 hover:border-blue-200 hover:text-blue-600"
                >
                  Đổi mật khẩu
                </button>
              </div>
            </div>
          </article>

          <article className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
            <h3 className="text-lg font-bold">Khóa học đang theo</h3>
            <p className="mt-2 text-sm text-slate-500">
              Xem chi tiết tại mục <b>Lịch học</b> và <b>Điểm số</b>.
            </p>
            <div className="mt-4 flex flex-wrap gap-2">
              <button
                onClick={() => onChuyenTrang?.('Lịch học')}
                className="flex h-10 items-center gap-2 rounded-xl bg-blue-600 px-4 text-sm font-bold text-white hover:bg-blue-700"
              >
                <Icon name="calendar" className="size-4" />
                Xem lịch học
              </button>
              <button
                onClick={() => onChuyenTrang?.('Điểm số')}
                className="flex h-10 items-center gap-2 rounded-xl border border-slate-200 bg-white px-4 text-sm font-bold text-slate-700 hover:border-blue-200 hover:text-blue-600"
              >
                <Icon name="chart" className="size-4" />
                Xem điểm số
              </button>
            </div>
          </article>
        </div>
      </div>

      {moModalDoiMK && <ModalDoiMatKhau onDong={() => setMoModalDoiMK(false)} />}
    </div>
  )
}