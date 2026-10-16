import { FormEvent, useState } from 'react'
import { Icon } from './BieuTuong'

type Props = {
  onDong: () => void
}

export default function ModalDoiMatKhau({ onDong }: Props) {
  const [mkCu, setMkCu] = useState('')
  const [mkMoi, setMkMoi] = useState('')
  const [mkXacNhan, setMkXacNhan] = useState('')
  const [hienMk, setHienMk] = useState(false)
  const [loi, setLoi] = useState('')
  const [dangXuLy, setDangXuLy] = useState(false)
  const [thanhCong, setThanhCong] = useState(false)

  const kiemTra = (): string | null => {
    if (!mkCu) return 'Vui lòng nhập mật khẩu hiện tại'
    if (mkMoi.length < 8) return 'Mật khẩu mới phải có ít nhất 8 ký tự'
    if (!/[a-zA-Z]/.test(mkMoi)) return 'Mật khẩu mới phải có ít nhất 1 chữ cái'
    if (!/[0-9]/.test(mkMoi)) return 'Mật khẩu mới phải có ít nhất 1 chữ số'
    if (mkMoi !== mkXacNhan) return 'Mật khẩu xác nhận không khớp'
    if (mkMoi === mkCu) return 'Mật khẩu mới phải khác mật khẩu cũ'
    return null
  }

  const submit = async (e: FormEvent) => {
    e.preventDefault()
    setLoi('')
    const err = kiemTra()
    if (err) {
      setLoi(err)
      return
    }

    setDangXuLy(true)
    // TODO: gọi API POST /api/auth/change-password khi backend xong
    await new Promise((r) => setTimeout(r, 600))
    setDangXuLy(false)
    setThanhCong(true)
  }

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/50 p-4 backdrop-blur-sm"
      onClick={onDong}
    >
      <div
        className="w-full max-w-md overflow-hidden rounded-2xl bg-white shadow-2xl"
        onClick={(e) => e.stopPropagation()}
      >
        <div className="flex items-start justify-between gap-4 border-b border-slate-100 p-6">
          <div>
            <h2 className="text-xl font-bold text-slate-900">Đổi mật khẩu</h2>
            <p className="mt-1 text-sm text-slate-500">
              Nhập mật khẩu hiện tại và mật khẩu mới của bạn.
            </p>
          </div>
          <button
            onClick={onDong}
            className="grid size-9 place-items-center rounded-full text-slate-400 hover:bg-slate-100 hover:text-slate-600"
          >
            <Icon name="close" className="size-5" />
          </button>
        </div>

        {thanhCong ? (
          <div className="p-6 text-center">
            <span className="mx-auto grid size-16 place-items-center rounded-full bg-emerald-100 text-emerald-600">
              <Icon name="check" className="size-8" />
            </span>
            <h3 className="mt-4 text-lg font-bold text-slate-900">
              Đổi mật khẩu thành công
            </h3>
            <p className="mt-2 text-sm text-slate-500">
              Mật khẩu mới đã được cập nhật. Vui lòng sử dụng mật khẩu này cho lần
              đăng nhập tiếp theo.
            </p>
            <button
              onClick={onDong}
              className="mt-6 w-full rounded-xl bg-blue-600 px-4 py-3 text-sm font-bold text-white hover:bg-blue-700"
            >
              Hoàn tất
            </button>
          </div>
        ) : (
          <form onSubmit={submit} className="space-y-4 p-6">
            <label className="block">
              <span className="mb-2 block text-sm font-semibold text-slate-700">
                Mật khẩu hiện tại
              </span>
              <div className="flex items-center gap-3 rounded-xl border border-slate-200 bg-white px-4 focus-within:border-blue-500 focus-within:ring-4 focus-within:ring-blue-50">
                <Icon name="lock" className="size-5 text-slate-400" />
                <input
                  type={hienMk ? 'text' : 'password'}
                  value={mkCu}
                  onChange={(e) => setMkCu(e.target.value)}
                  className="h-12 min-w-0 flex-1 border-0 bg-transparent text-sm outline-none"
                  placeholder="Nhập mật khẩu hiện tại"
                />
              </div>
            </label>

            <label className="block">
              <span className="mb-2 block text-sm font-semibold text-slate-700">
                Mật khẩu mới
              </span>
              <div className="flex items-center gap-3 rounded-xl border border-slate-200 bg-white px-4 focus-within:border-blue-500 focus-within:ring-4 focus-within:ring-blue-50">
                <Icon name="lock" className="size-5 text-slate-400" />
                <input
                  type={hienMk ? 'text' : 'password'}
                  value={mkMoi}
                  onChange={(e) => setMkMoi(e.target.value)}
                  className="h-12 min-w-0 flex-1 border-0 bg-transparent text-sm outline-none"
                  placeholder="Ít nhất 8 ký tự, có chữ và số"
                />
                <button
                  type="button"
                  onClick={() => setHienMk(!hienMk)}
                  className="text-slate-400 hover:text-blue-600"
                  aria-label="Hiện mật khẩu"
                >
                  <Icon name="eye" className="size-5" />
                </button>
              </div>
            </label>

            <label className="block">
              <span className="mb-2 block text-sm font-semibold text-slate-700">
                Xác nhận mật khẩu mới
              </span>
              <div className="flex items-center gap-3 rounded-xl border border-slate-200 bg-white px-4 focus-within:border-blue-500 focus-within:ring-4 focus-within:ring-blue-50">
                <Icon name="lock" className="size-5 text-slate-400" />
                <input
                  type={hienMk ? 'text' : 'password'}
                  value={mkXacNhan}
                  onChange={(e) => setMkXacNhan(e.target.value)}
                  className="h-12 min-w-0 flex-1 border-0 bg-transparent text-sm outline-none"
                  placeholder="Nhập lại mật khẩu mới"
                />
              </div>
            </label>

            {loi && (
              <div className="rounded-xl bg-red-50 p-3 text-sm text-red-700">
                {loi}
              </div>
            )}

            <div className="flex gap-2 pt-2">
              <button
                type="button"
                onClick={onDong}
                className="flex-1 rounded-xl border border-slate-200 px-4 py-3 text-sm font-semibold text-slate-600 hover:bg-slate-50"
              >
                Hủy
              </button>
              <button
                type="submit"
                disabled={dangXuLy}
                className="flex flex-1 items-center justify-center gap-2 rounded-xl bg-blue-600 px-4 py-3 text-sm font-bold text-white hover:bg-blue-700 disabled:opacity-60"
              >
                {dangXuLy ? 'Đang xử lý...' : 'Xác nhận'}
              </button>
            </div>
          </form>
        )}
      </div>
    </div>
  )
}