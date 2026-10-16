import { useState } from 'react'
import { Icon } from '../components/BieuTuong'
import { dungXacThuc } from '../contexts/NguCanhXacThuc'
import ModalDoiMatKhau from '../components/ModalDoiMatKhau'

export default function CaiDat() {
  const { nguoiDung } = dungXacThuc()
  const [moModalDoiMK, setMoModalDoiMK] = useState(false)
  const [thongBao, setThongBao] = useState('')

  const hienThongBao = (msg: string) => {
    setThongBao(msg)
    setTimeout(() => setThongBao(''), 3000)
  }

  return (
    <div className="space-y-6">
      <div>
        <p className="mb-1 text-sm font-semibold text-blue-600">Hệ thống</p>
        <h1 className="text-3xl font-bold text-slate-900">Cài đặt</h1>
        <p className="mt-2 text-sm text-slate-500">
          Quản lý tài khoản, thông báo và tùy chọn hệ thống.
        </p>
      </div>

      {/* Tài khoản */}
      <article className="rounded-2xl border border-slate-200 bg-white shadow-sm">
        <div className="border-b border-slate-100 p-5">
          <h2 className="text-lg font-bold">Tài khoản</h2>
          <p className="mt-1 text-xs text-slate-500">
            Thông tin đăng nhập và bảo mật
          </p>
        </div>
        <div className="divide-y divide-slate-100">
          <div className="flex items-center justify-between p-5">
            <div className="flex items-center gap-3">
              <span className="grid size-10 place-items-center rounded-xl bg-slate-100">
                <Icon name="mail" className="size-5 text-slate-500" />
              </span>
              <div>
                <p className="font-semibold text-slate-900">Email</p>
                <p className="text-sm text-slate-500">{nguoiDung?.email}</p>
              </div>
            </div>
          </div>
          <div className="flex items-center justify-between p-5">
            <div className="flex items-center gap-3">
              <span className="grid size-10 place-items-center rounded-xl bg-slate-100">
                <Icon name="lock" className="size-5 text-slate-500" />
              </span>
              <div>
                <p className="font-semibold text-slate-900">Mật khẩu</p>
                <p className="text-sm text-slate-500">••••••••</p>
              </div>
            </div>
            <button
              onClick={() => setMoModalDoiMK(true)}
              className="rounded-lg bg-blue-600 px-4 py-2 text-xs font-bold text-white hover:bg-blue-700"
            >
              Đổi mật khẩu
            </button>
          </div>
        </div>
      </article>

      {/* Thông báo */}
      <article className="rounded-2xl border border-slate-200 bg-white shadow-sm">
        <div className="border-b border-slate-100 p-5">
          <h2 className="text-lg font-bold">Thông báo</h2>
          <p className="mt-1 text-xs text-slate-500">
            Chọn loại thông báo bạn muốn nhận
          </p>
        </div>
        <div className="divide-y divide-slate-100">
          {[
            ['Thông báo lớp học', 'Nhận thông báo về lịch học, đổi phòng, nghỉ lễ'],
            ['Thông báo điểm số', 'Nhận thông báo khi có điểm mới được cập nhật'],
            ['Thông báo học phí', 'Nhắc nhở thanh toán học phí đúng hạn'],
            ['Thông báo hệ thống', 'Cập nhật về bảo trì và tính năng mới'],
          ].map(([tieuDe, moTa], i) => (
            <label
              key={tieuDe}
              className="flex cursor-pointer items-center justify-between p-5 hover:bg-slate-50"
            >
              <div>
                <p className="font-semibold text-slate-900">{tieuDe}</p>
                <p className="text-sm text-slate-500">{moTa}</p>
              </div>
              <input
                type="checkbox"
                defaultChecked={i !== 2}
                className="size-5 accent-blue-600"
              />
            </label>
          ))}
        </div>
      </article>

      {/* Giao diện */}
      <article className="rounded-2xl border border-slate-200 bg-white shadow-sm">
        <div className="border-b border-slate-100 p-5">
          <h2 className="text-lg font-bold">Giao diện</h2>
        </div>
        <div className="p-5 space-y-3">
          <div className="flex items-center justify-between">
            <div>
              <p className="font-semibold text-slate-900">Chế độ tối</p>
              <p className="text-sm text-slate-500">Sẽ sớm ra mắt</p>
            </div>
            <button
              disabled
              className="rounded-lg border border-slate-200 px-4 py-2 text-xs font-semibold text-slate-400 cursor-not-allowed"
            >
              Sắp ra mắt
            </button>
          </div>
        </div>
      </article>

      {/* Nguy hiểm */}
      <article className="rounded-2xl border border-rose-200 bg-rose-50 shadow-sm">
        <div className="p-5">
          <h2 className="text-lg font-bold text-rose-900">Vùng nguy hiểm</h2>
          <p className="mt-1 text-sm text-rose-700">
            Các hành động này không thể hoàn tác.
          </p>
          <button
            onClick={() =>
              hienThongBao('Vui lòng liên hệ trung tâm để xóa tài khoản')
            }
            className="mt-4 rounded-xl border border-rose-300 bg-white px-4 py-2 text-sm font-bold text-rose-700 hover:bg-rose-100"
          >
            Yêu cầu xóa tài khoản
          </button>
        </div>
      </article>

      {thongBao && (
        <div className="rounded-xl border border-blue-200 bg-blue-50 p-4 text-sm font-semibold text-blue-700">
          {thongBao}
        </div>
      )}

      {moModalDoiMK && <ModalDoiMatKhau onDong={() => setMoModalDoiMK(false)} />}
    </div>
  )
}