// Bộ khung giao diện "Tổng quan" dùng chung cho 3 vai trò: học viên, giáo viên, trợ giảng.
// Bố cục: [Thẻ thông tin cá nhân] + [3 thẻ chỉ số] → [Dải lối tắt] → [3 khối nội dung].
import type { ReactNode } from 'react'
import { Link } from 'react-router'
import { Icon, type IconName } from './BieuTuong'

export type MucThongTin = { nhan: string; giaTri?: string | null }

export type ToneChiSo = 'blue' | 'rose' | 'slate'

export type DuLieuChiSo = {
  nhan: string
  giaTri: string | number
  moTa?: string
  icon: IconName
  tone?: ToneChiSo
  duong?: string
}

export type DuLieuLoiTat = { nhan: string; duong: string; icon: IconName }

const TONE: Record<ToneChiSo, { chu: string; nen: string; vong: string }> = {
  blue: { chu: 'text-blue-700', nen: 'bg-blue-50', vong: 'bg-blue-600/10 text-blue-700' },
  rose: { chu: 'text-rose-600', nen: 'bg-rose-50', vong: 'bg-rose-500/10 text-rose-600' },
  slate: { chu: 'text-slate-700', nen: 'bg-slate-100', vong: 'bg-slate-500/10 text-slate-600' },
}

/** Chữ viết tắt từ họ tên (tối đa 2 ký tự) để làm avatar. */
export function chuVietTat(hoTen?: string | null): string {
  if (!hoTen) return '?'
  const tu = hoTen.trim().split(/\s+/)
  const dau = tu[0]?.[0] ?? ''
  const cuoi = tu.length > 1 ? (tu[tu.length - 1]?.[0] ?? '') : ''
  return (dau + cuoi).toUpperCase()
}

/** Thẻ "Thông tin ..." bên trái: avatar + họ tên + lưới nhãn/giá trị (4 cột). */
export function TheThongTin({
  tieuDe,
  phuDe,
  hoTen,
  vaiTro,
  thongTin,
}: {
  tieuDe: string
  phuDe?: string
  hoTen: string
  vaiTro?: string
  thongTin: MucThongTin[]
}) {
  return (
    <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
      <div className="mb-4 flex flex-wrap items-center justify-between gap-2">
        <div>
          <h2 className="text-base font-bold text-slate-900">{tieuDe}</h2>
          {phuDe && <p className="text-xs text-slate-500">{phuDe}</p>}
        </div>
      </div>

      <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:gap-10">
        <div className="flex shrink-0 flex-col items-center gap-2 sm:w-32">
          <span className="grid size-28 place-items-center rounded-full bg-linear-to-br from-blue-500 to-sky-400 text-3xl font-black text-white ring-4 ring-blue-50">
            {chuVietTat(hoTen)}
          </span>
          <p className="text-center text-sm font-bold text-slate-900">{hoTen}</p>
          {vaiTro && (
            <span className="rounded-full bg-blue-50 px-3 py-1 text-[11px] font-bold uppercase tracking-wide text-blue-700">
              {vaiTro}
            </span>
          )}
        </div>

        <dl className="grid min-w-0 flex-1 gap-x-6 gap-y-3 sm:grid-cols-2 lg:grid-cols-3 xl:gap-x-10">
          {thongTin.map((m) => (
            <div key={m.nhan} className="min-w-0">
              <dt className="text-[11px] font-semibold uppercase tracking-wide text-slate-400">
                {m.nhan}
              </dt>
              <dd className="truncate text-sm font-semibold text-slate-800" title={m.giaTri ?? '—'}>
                {m.giaTri?.trim() ? m.giaTri : '—'}
              </dd>
            </div>
          ))}
        </dl>
      </div>
    </section>
  )
}

/** Một thẻ chỉ số lớn (số lớn + icon tròn + link xem chi tiết). */
export function TheChiSoLon({
  chiSo,
  co = 'nho',
}: {
  chiSo: DuLieuChiSo
  co?: 'lon' | 'nho'
}) {
  const tone = TONE[chiSo.tone ?? 'blue']

  return (
    <section
      className={`flex flex-col justify-between rounded-2xl border border-slate-200 bg-white p-5 shadow-sm ${
        co === 'lon' ? 'min-h-36' : 'min-h-32'
      }`}
    >
      <div className="flex items-start justify-between gap-3">
        <div className="min-w-0">
          <p className="text-sm font-semibold text-slate-500">{chiSo.nhan}</p>
          <p
            className={`mt-1 font-black leading-none ${tone.chu} ${
              co === 'lon' ? 'text-4xl' : 'text-3xl'
            }`}
          >
            {chiSo.giaTri}
          </p>
          {chiSo.moTa && <p className="mt-1 text-xs text-slate-500">{chiSo.moTa}</p>}
        </div>
        <span className={`grid size-11 shrink-0 place-items-center rounded-full ${tone.vong}`}>
          <Icon name={chiSo.icon} className="size-5" />
        </span>
      </div>

      {chiSo.duong && (
        <Link
          to={chiSo.duong}
          className={`mt-3 inline-flex items-center gap-1 text-xs font-bold ${tone.chu} hover:underline`}
        >
          Xem chi tiết
          <Icon name="arrowRight" className="size-3.5" />
        </Link>
      )}
    </section>
  )
}

/** Dải ô lối tắt (icon + nhãn) như thanh nghiệp vụ. */
export function DaiLoiTat({ ds }: { ds: DuLieuLoiTat[] }) {
  return (
    <nav className="grid grid-cols-2 gap-3 sm:grid-cols-4 xl:grid-cols-6">
      {ds.map((m) => (
        <Link
          key={m.duong + m.nhan}
          to={m.duong}
          className="group flex flex-col items-center gap-2 rounded-2xl border border-slate-200 bg-white px-2 py-4 text-center shadow-sm transition hover:-translate-y-0.5 hover:border-blue-300 hover:shadow-md focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-blue-600"
        >
          <span className="grid size-11 place-items-center rounded-xl bg-blue-50 text-blue-600 transition group-hover:bg-blue-600 group-hover:text-white">
            <Icon name={m.icon} className="size-5" />
          </span>
          <span className="text-[13px] font-semibold text-slate-700">{m.nhan}</span>
        </Link>
      ))}
    </nav>
  )
}

/** Khung 1 khối nội dung phía dưới (tiêu đề + phần thân). */
export function KhoiNoiDung({
  tieuDe,
  moTa,
  hanhDong,
  children,
}: {
  tieuDe: string
  moTa?: string
  hanhDong?: ReactNode
  children: ReactNode
}) {
  return (
    <section className="flex flex-col rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
      <div className="mb-4 flex flex-wrap items-start justify-between gap-2">
        <div>
          <h2 className="text-base font-bold text-slate-900">{tieuDe}</h2>
          {moTa && <p className="text-xs text-slate-500">{moTa}</p>}
        </div>
        {hanhDong}
      </div>
      <div className="min-h-0 flex-1">{children}</div>
    </section>
  )
}

/** Biểu đồ cột đơn giản bằng CSS (không cần thư viện chart). */
export function BieuDoCot({
  items,
  donVi,
}: {
  items: Array<{ nhan: string; giaTri: number }>
  donVi?: string
}) {
  const max = Math.max(1, ...items.map((i) => i.giaTri))

  return (
    <div className="flex h-56 items-stretch gap-2">
      {items.map((i) => (
        <div key={i.nhan} className="flex min-w-0 flex-1 flex-col items-center gap-1.5">
          <span className="text-xs font-bold text-slate-700">
            {i.giaTri}
            {donVi ? <span className="font-normal text-slate-400">{donVi}</span> : null}
          </span>
          <div className="flex w-full flex-1 items-end justify-center">
            <div
              className="w-full max-w-12 rounded-t-lg bg-linear-to-t from-blue-600 to-sky-400 transition-all"
              style={{
                height: `${Math.max(4, (i.giaTri / max) * 100)}%`,
              }}
            />
          </div>
          <span className="w-full truncate text-center text-[11px] text-slate-500" title={i.nhan}>
            {i.nhan}
          </span>
        </div>
      ))}
    </div>
  )
}

/** Vòng tiến độ (donut) bằng SVG. */
export function VongTienDo({
  giaTri,
  tong,
  nhan,
  mau = '#2563eb',
}: {
  giaTri: number
  tong: number
  nhan?: string
  mau?: string
}) {
  const banKinh = 54
  const chuVi = 2 * Math.PI * banKinh
  const tiLe = tong > 0 ? Math.min(1, Math.max(0, giaTri / tong)) : 0

  return (
    <div className="flex flex-col items-center justify-center gap-2">
      <svg viewBox="0 0 140 140" className="size-40" role="img" aria-label={nhan}>
        <circle cx="70" cy="70" r={banKinh} fill="none" stroke="#e2e8f0" strokeWidth="14" />
        <circle
          cx="70"
          cy="70"
          r={banKinh}
          fill="none"
          stroke={mau}
          strokeWidth="14"
          strokeLinecap="round"
          strokeDasharray={`${chuVi * tiLe} ${chuVi}`}
          transform="rotate(-90 70 70)"
        />
        <text x="70" y="70" textAnchor="middle" fontSize="26" fontWeight="800" fill="#0f172a">
          {giaTri}
        </text>
        <text x="70" y="90" textAnchor="middle" fontSize="12" fill="#94a3b8">
          / {tong}
        </text>
      </svg>
      {nhan && <p className="text-sm font-semibold text-slate-600">{nhan}</p>}
    </div>
  )
}

/** Thông báo khi khối chưa có dữ liệu. */
export function TrongRong({ noiDung }: { noiDung: string }) {
  return (
    <p className="grid h-full min-h-32 place-items-center rounded-xl border border-dashed border-slate-200 bg-slate-50/60 px-4 text-center text-sm text-slate-400">
      {noiDung}
    </p>
  )
}
