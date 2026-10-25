import { FormEvent, useState } from "react";
import { Link, useNavigate, useLocation } from "react-router";
import logo from "../assets/logo.png";
import { Icon } from "../components/BieuTuong";
import { dungXacThuc, duongDanTheoVaiTro } from "../contexts/NguCanhXacThuc";

const SANS = "'Plus Jakarta Sans', system-ui, -apple-system, sans-serif";

/** Mật khẩu chung của các tài khoản mẫu (đã đặt lại toàn bộ về 111111). */
const MAT_KHAU_MAU = "111111";

export default function DangNhap() {
  const navigate = useNavigate();
  const location = useLocation();
  const { dangNhap } = dungXacThuc();

  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [tenDangNhap, setTenDangNhap] = useState("ADMIN001");
  const [password, setPassword] = useState(MAT_KHAU_MAU);

  const xuLyDangNhap = async (ten: string, matKhau: string) => {
    setLoading(true);
    setError(null);

    try {
      const nguoiDung = await dangNhap(ten.trim(), matKhau);
      const from = (location.state as { from?: string } | null)?.from;
      navigate(from || duongDanTheoVaiTro(nguoiDung.vaiTro), { replace: true });
    } catch (err) {
      setError(err instanceof Error ? err.message : "Đăng nhập thất bại");
    } finally {
      setLoading(false);
    }
  };

  const submit = async (event: FormEvent) => {
    event.preventDefault();
    await xuLyDangNhap(tenDangNhap, password);
  };

  /** Bấm vào tài khoản mẫu: điền sẵn và đăng nhập ngay, không cần nhập gì. */
  const dangNhapNhanh = (ten: string) => {
    setTenDangNhap(ten);
    setPassword(MAT_KHAU_MAU);
    void xuLyDangNhap(ten, MAT_KHAU_MAU);
  };

  return (
    <main className="min-h-screen bg-slate-50 lg:grid lg:grid-cols-[1.08fr_.92fr]">
      {/* ========== LEFT PANEL — Gradient xanh ========== */}
      <section
        className="relative hidden overflow-hidden p-12 text-white lg:flex lg:flex-col"
        style={{
          background:
            "linear-gradient(135deg, #1E3A8A 0%, #2563EB 50%, #3B82F6 100%)",
        }}
      >
        <div className="absolute -left-32 top-1/3 size-96 rounded-full bg-white/10 blur-3xl" />
        <div className="absolute -right-24 -top-20 size-80 rounded-full bg-blue-300/20 blur-3xl" />
        <div className="absolute bottom-20 right-20 size-64 rounded-full bg-indigo-400/20 blur-3xl" />

        <Link
          to="/"
          className="relative flex w-fit items-center gap-3 text-white no-underline"
        >
          <img src={logo} alt="LingoHub" className="size-11 shrink-0 rounded-full object-cover" />
          <span
            className="text-2xl font-bold tracking-tight"
            style={{ fontFamily: SANS }}
          >
            LingoHub
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
              ["1.248", "Học viên"],
              ["42", "Lớp đang học"],
              ["98,2%", "Dữ liệu đồng bộ"],
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
          © 2026 LingoHub Education Management
        </p>
      </section>

      {/* ========== RIGHT PANEL — Form ========== */}
      <section className="flex min-h-screen items-center justify-center px-6 py-12">
        <div className="w-full max-w-md">
          <Link
            to="/"
            className="mb-12 flex items-center gap-3 text-slate-900 no-underline lg:hidden"
          >
            <img src={logo} alt="LingoHub" className="size-10 shrink-0 rounded-full object-cover" />
            <span className="text-xl font-bold" style={{ fontFamily: SANS }}>
              LingoHub
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
                Tên đăng nhập
              </span>
              <span className="flex items-center gap-3 rounded-xl border border-slate-200 bg-white px-4 shadow-sm transition focus-within:border-blue-500 focus-within:ring-4 focus-within:ring-blue-50">
                <Icon name="mail" className="size-5 text-slate-400" />
                <input
                  type="text"
                  value={tenDangNhap}
                  onChange={(e) => setTenDangNhap(e.target.value)}
                  required
                  autoComplete="username"
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
                  type={showPassword ? "text" : "password"}
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
                />{" "}
                Ghi nhớ đăng nhập
              </label>
              <button
                type="button"
                onClick={() =>
                  alert(
                    "Vui lòng liên hệ trung tâm để được hỗ trợ đặt lại mật khẩu:\n\n" +
                      "Hotline: 028.1234.5678\n" +
                      "Email: hotro@lingohub.vn",
                  )
                }
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
                background: "linear-gradient(135deg, #2563EB 0%, #1D4ED8 100%)",
                boxShadow: "0 10px 30px -10px rgba(37,99,235,0.5)",
              }}
            >
              {loading ? "Đang xác thực..." : "Đăng nhập"}
              {!loading && <Icon name="arrowRight" className="size-5" />}
            </button>
          </form>

          <div className="mt-8 flex items-start gap-3 rounded-xl border border-blue-100 bg-blue-50 p-4 text-sm leading-6 text-blue-900">
            <Icon
              name="lock"
              className="mt-0.5 size-4 shrink-0 text-blue-600"
            />
            <span>
              Phiên đăng nhập có hiệu lực 60 phút. Hết hạn bạn sẽ được yêu cầu
              đăng nhập lại.
            </span>
          </div>

          {/* Tài khoản demo */}
          <div className="mt-6 rounded-xl border border-slate-200 bg-slate-50 p-5 text-sm">
            <div className="mb-3 flex items-center gap-1.5 font-bold text-slate-700">
              <Icon name="lock" className="size-3.5" /> Tài khoản mẫu (mật khẩu:{" "}
              <code className="rounded bg-slate-200 px-1.5 py-0.5">{MAT_KHAU_MAU}</code>
              ) — bấm để đăng nhập ngay
            </div>

            {/* Quản trị viên */}
            <div className="mb-3">
              <div className="mb-1 flex items-center gap-1.5 text-xs font-bold uppercase tracking-wider text-slate-500">
                <Icon name="shield" className="size-3.5" /> Quản trị viên
              </div>
              <button
                type="button"
                disabled={loading}
                onClick={() => dangNhapNhanh("ADMIN001")}
                className="block w-full rounded-lg bg-white px-3 py-2 text-left text-xs text-slate-600 transition hover:bg-blue-50 hover:text-blue-700 disabled:opacity-50"
              >
                ADMIN001
              </button>
            </div>

            {/* Giáo viên */}
            <div className="mb-3">
              <div className="mb-1 flex items-center gap-1.5 text-xs font-bold uppercase tracking-wider text-slate-500">
                <Icon name="presentation" className="size-3.5" /> Giáo viên
              </div>
              <div className="grid grid-cols-2 gap-1">
                {[
                  "GV001",
                  "GV002",
                  "GV003",
                  "TG001",
                ].map((e) => (
                  <button
                    key={e}
                    type="button"
                    disabled={loading}
                    onClick={() => dangNhapNhanh(e)}
                    className="rounded-lg bg-white px-3 py-2 text-left text-xs text-slate-600 transition hover:bg-blue-50 hover:text-blue-700 disabled:opacity-50"
                  >
                    {e}
                  </button>
                ))}
              </div>
            </div>

            {/* Học viên */}
            <div>
              <div className="mb-1 flex items-center gap-1.5 text-xs font-bold uppercase tracking-wider text-slate-500">
                <Icon name="bookUser" className="size-3.5" /> Học viên
              </div>
              <div className="grid grid-cols-2 gap-1">
                {[
                  "HV001",
                  "HV002",
                  "HV003",
                  "HV004",
                ].map((e) => (
                  <button
                    key={e}
                    type="button"
                    disabled={loading}
                    onClick={() => dangNhapNhanh(e)}
                    className="rounded-lg bg-white px-3 py-2 text-left text-xs text-slate-600 transition hover:bg-blue-50 hover:text-blue-700 disabled:opacity-50"
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
  );
}
