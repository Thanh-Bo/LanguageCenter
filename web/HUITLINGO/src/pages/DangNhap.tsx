import { FormEvent, useState } from 'react'
import { Link, useNavigate, useLocation } from 'react-router'
import { Icon } from '../components/BieuTuong'
import { dungXacThuc, duongDanTheoVaiTro } from '../contexts/NguCanhXacThuc'

const SANS = "'Plus Jakarta Sans', system-ui, -apple-system, sans-serif"

export default function DangNhap() {
  const navigate = useNavigate()
  const location = useLocation()
  const { dangNhap } = dungXacThuc()

  const [showPassword, setShowPassword] = useState(false)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)
  const [email, setEmail] = useState('lananh@lingohub.vn')
  const [password, setPassword] = useState('123456')

  const submit = async (event: FormEvent) => {
    event.preventDefault()
    setLoading(true)
    setError(null)

    try {
      const nguoiDung = await dangNhap(email.trim(), password)
      const from = (location.state as { from?: string } | null)?.from
      navigate(from || duongDanTheoVaiTro(nguoiDung.vaiTro), { replace: true })
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Đăng nhập thất bại')
    } finally {
      setLoading(false)
    }
  }

  return (
    <main className="min-h-screen bg-slate-50 lg:grid lg:grid-cols-[1.08fr_.92fr]">
      {/* ========== LEFT PANEL — Gradient xanh ========== */}
      <section
        className="relative hidden overflow-hidden p-12 text-white lg:flex lg:flex-col"
        style={{
          background:
            'linear-gradient(135deg, #1E3A8A 0%, #2563EB 50%, #3B82F6 100%)',
        }}
      >
        <div className="absolute -left-32 top-1/3 size-96 rounded-full bg-white/10 blur-3xl" />
        <div className="absolute -right-24 -top-20 size-80 rounded-full bg-blue-300/20 blur-3xl" />
        <div className="absolute bottom-20 right-20 size-64 rounded-full bg-indigo-400/20 blur-3xl" />

        <Link
          to="/"
          className="relative flex w-fit items-center gap-3 text-white no-underline"
        >
          <span className="grid size-11 place-items-center rounded-xl bg-white/15 ring-1 ring-white/20 backdrop-blur-sm">
            <Icon name="logo" className="size-6" />
          </span>
          <span
            className="text-2xl font-bold tracking-tight"
            style={{ fontFamily: SANS }}
          >
            HUIT LingoHub
          </span>
        </Link>

        <div className="relative my-auto max-w-xl">
          <span className="mb-7 inline-flex items-center gap-2 rounded-full border border-white/20 bg-white/10 px-4 py-2 text-sm font-semibold backdrop-blur-sm">
            <Icon name="sparkles" className="size-4" />
            Không gian làm việc thông minh
          </span>

          <h1
            className="text-5xl font-bold leading-[1.15] tracking-tight"
            style={{ fontFamily: SANS }}
          >
            Quản lý trung tâm
            <br />
            nhẹ nhàng hơn mỗi ngày.
          </h1>

          <p className="mt-6 max-w-lg text-lg leading-8 text-blue-100">
            Tất cả lớp học, lịch dạy, học viên và học phí được đồng bộ trong một
            hệ thống duy nhất.
          </p>

          <div className="mt-12 grid grid-cols-3 gap-6 border-t border-white/20 pt-8">
            {[
              ['1.248', 'Học viên'],
              ['42', 'Lớp đang học'],
              ['98,2%', 'Dữ liệu đồng bộ'],
            ].map(([value, label]) => (
              <div key={label}>
                <div
                  className="text-3xl font-bold tracking-tight"
                  style={{ fontFamily: SANS }}
                >
                  {value}
                </div>
                <div className="mt-1 text-sm text-blue-100">{label}</div>
              </div>
            ))}
          </div>
        </div>

        <p className="relative text-sm text-blue-200">
          © 2026 HUIT LingoHub Education Management
        </p>
      </section>

      {/* ========== RIGHT PANEL — Form ========== */}
      <section className="flex min-h-screen items-center justify-center px-6 py-12">
        <div className="w-full max-w-md">
          <Link
            to="/"
            className="mb-12 flex items-center gap-3 text-slate-900 no-underline lg:hidden"
          >
            <span className="grid size-10 place-items-center rounded-xl bg-blue-600 text-white">
              <Icon name="logo" className="size-5" />
            </span>
            <span
              className="text-xl font-bold"
              style={{ fontFamily: SANS }}
            >
              HUIT LingoHub
            </span>
          </Link>

          <div className="mb-9">
            <p className="mb-2 text-sm font-bold uppercase tracking-widest text-blue-600">
              Chào mừng trở lại
            </p>
            <h2
              className="text-3xl font-bold tracking-tight text-slate-900 sm:text-4xl"
              style={{ fontFamily: SANS }}
            >
              Đăng nhập hệ thống
            </h2>
            <p className="mt-3 text-slate-500">
              Sử dụng tài khoản được trung tâm cấp để tiếp tục.
            </p>
          </div>

          <form onSubmit={submit} className="space-y-5">
            <label className="block">
              <span className="mb-2 block text-sm font-semibold text-slate-700">
                Email hoặc tên đăng nhập
              </span>
              <span className="flex items-center gap-3 rounded-xl border border-slate-200 bg-white px-4 shadow-sm transition focus-within:border-blue-500 focus-within:ring-4 focus-within:ring-blue-50">
                <Icon name="mail" className="size-5 text-slate-400" />
                <input
                  type="email"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  required
                  className="h-13 min-w-0 flex-1 border-0 bg-transparent text-sm outline-none"
                />
              </span>
            </label>

            <label className="block">
              <span className="mb-2 block text-sm font-semibold text-slate-700">
                Mật khẩu
              </span>
              <span className="flex items-center gap-3 rounded-xl border border-slate-200 bg-white px-4 shadow-sm transition focus-within:border-blue-500 focus-within:ring-4 focus-within:ring-blue-50">
                <Icon name="lock" className="size-5 text-slate-400" />
                <input
                  type={showPassword ? 'text' : 'password'}
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                  className="h-13 min-w-0 flex-1 border-0 bg-transparent text-sm outline-none"
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="text-slate-400 hover:text-blue-600"
                  aria-label="Hiện mật khẩu"
                >
                  <Icon name="eye" className="size-5" />
                </button>
              </span>
            </label>

            <div className="flex items-center justify-between text-sm">
              <label className="flex items-center gap-2 text-slate-600">
                <input
                  type="checkbox"
                  defaultChecked
                  className="size-4 accent-blue-600"
                />{' '}
                Ghi nhớ đăng nhập
              </label>
              <button
                type="button"
                className="font-semibold text-blue-600 hover:text-blue-700"
              >
                Quên mật khẩu?
              </button>
            </div>

            {error && (
              <div className="rounded-xl bg-red-50 p-3 text-sm text-red-700">
                {error}
              </div>
            )}

            <button
              type="submit"
              disabled={loading}
              className="flex h-13 w-full items-center justify-center gap-2 rounded-xl font-bold text-white shadow-lg transition hover:brightness-110 disabled:opacity-70"
              style={{
                background: 'linear-gradient(135deg, #2563EB 0%, #1D4ED8 100%)',
                boxShadow: '0 10px 30px -10px rgba(37,99,235,0.5)',
              }}
            >
              {loading ? 'Đang xác thực...' : 'Đăng nhập'}
              {!loading && <Icon name="arrowRight" className="size-5" />}
            </button>
          </form>

          <div className="mt-8 flex items-start gap-3 rounded-xl border border-blue-100 bg-blue-50 p-4 text-sm leading-6 text-blue-900">
            <Icon name="lock" className="mt-0.5 size-4 shrink-0 text-blue-600" />
            <span>
              Phiên đăng nhập được mã hóa và tự động kết thúc sau 30 phút không
              hoạt động.
            </span>
          </div>

          {/* Tài khoản demo */}
          <div className="mt-6 rounded-xl border border-slate-200 bg-slate-50 p-5 text-sm">
            <div className="mb-3 font-bold text-slate-700">
              🔑 Tài khoản demo (mật khẩu:{' '}
              <code className="rounded bg-slate-200 px-1.5 py-0.5">123456</code>
              )
            </div>

            {/* Quản trị viên */}
            <div className="mb-3">
              <div className="mb-1 text-xs font-bold uppercase tracking-wider text-slate-500">
                👨‍💼 Quản trị viên
              </div>
              <button
                type="button"
                onClick={() => {
                  setEmail('admin@lingohub.vn')
                  setPassword('123456')
                }}
                className="block w-full rounded-lg bg-white px-3 py-2 text-left text-xs text-slate-600 transition hover:bg-blue-50 hover:text-blue-700"
              >
                admin@lingohub.vn
              </button>
            </div>

            {/* Giáo viên */}
            <div className="mb-3">
              <div className="mb-1 text-xs font-bold uppercase tracking-wider text-slate-500">
                👨‍🏫 Giáo viên
              </div>
              <div className="grid grid-cols-2 gap-1">
                {[
                  'lananh@lingohub.vn',
                  'minhkhoa@lingohub.vn',
                  'david@lingohub.vn',
                  'emily@lingohub.vn',
                ].map((e) => (
                  <button
                    key={e}
                    type="button"
                    onClick={() => {
                      setEmail(e)
                      setPassword('123456')
                    }}
                    className="rounded-lg bg-white px-3 py-2 text-left text-xs text-slate-600 transition hover:bg-blue-50 hover:text-blue-700"
                  >
                    {e}
                  </button>
                ))}
              </div>
            </div>

            {/* Học viên */}
            <div>
              <div className="mb-1 text-xs font-bold uppercase tracking-wider text-slate-500">
                👩‍🎓 Học viên
              </div>
              <div className="grid grid-cols-2 gap-1">
                {[
                  'khoa.hv@lingohub.vn',
                  'ha.lt@lingohub.vn',
                  'an.nv@lingohub.vn',
                  'linh.pm@lingohub.vn',
                ].map((e) => (
                  <button
                    key={e}
                    type="button"
                    onClick={() => {
                      setEmail(e)
                      setPassword('123456')
                    }}
                    className="rounded-lg bg-white px-3 py-2 text-left text-xs text-slate-600 transition hover:bg-blue-50 hover:text-blue-700"
                  >
                    {e}
                  </button>
                ))}
              </div>
            </div>
          </div>
        </div>
      </section>
    </main>
  )
}