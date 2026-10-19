// Bộ component nền dùng chung — giữ nhất quán về giao diện và trạng thái.
import type { ReactNode } from 'react'
import { Icon, type IconName } from '../BieuTuong'

// ===== LỚP DÙNG CHUNG =====
export const LOP_NUT_CHINH =
  'inline-flex h-11 items-center justify-center gap-2 rounded-xl bg-blue-600 px-5 text-sm font-bold text-white shadow-sm shadow-blue-200 transition hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-50'

export const LOP_NUT_PHU =
  'inline-flex h-11 items-center justify-center gap-2 rounded-xl border border-slate-200 bg-white px-5 text-sm font-semibold text-slate-700 transition hover:border-blue-200 hover:text-blue-600 disabled:cursor-not-allowed disabled:opacity-50'

export const LOP_INPUT =
  'h-11 w-full rounded-xl border border-slate-200 bg-white px-4 text-sm text-slate-900 outline-none transition focus:border-blue-500 focus:ring-4 focus:ring-blue-50'

export const LOP_NHAN =
  'mb-1.5 block text-xs font-bold uppercase tracking-wider text-slate-500'

// ===== TIÊU ĐỀ TRANG =====
export function TrangTieuDe({
  eyebrow,
  title,
  desc,
  actions,
}: {
  eyebrow?: string
  title: string
  desc?: string
  actions?: ReactNode
}) {
  return (
    <div className="flex flex-col justify-between gap-4 sm:flex-row sm:items-end">
      <div>
        {eyebrow && (
          <p className="mb-1 text-sm font-semibold text-blue-600">{eyebrow}</p>
        )}
        <h1 className="text-3xl font-bold text-slate-900">{title}</h1>
        {desc && <p className="mt-2 text-sm text-slate-500">{desc}</p>}
      </div>
      {actions && <div className="flex flex-wrap items-center gap-2">{actions}</div>}
    </div>
  )
}

// ===== CARD CHỈ SỐ =====
export function TheChiSo({
  nhan,
  giaTri,
  icon,
  moTa,
  tone = 'blue',
  onClick,
}: {
  nhan: string
  giaTri: ReactNode
  icon: IconName
  moTa?: string
  tone?: 'blue' | 'emerald' | 'amber' | 'rose' | 'slate'
  /** Có onClick → thẻ biến thành nút bấm được (hover + gợi ý "Xem chi tiết"). */
  onClick?: () => void
}) {
  const mau: Record<string, string> = {
    blue: 'bg-blue-600 text-white',
    emerald: 'bg-emerald-600 text-white',
    amber: 'bg-amber-500 text-white',
    rose: 'bg-rose-500 text-white',
    slate: 'bg-slate-800 text-white',
  }

  const noiDung = (
    <>
      <span className={`grid size-11 place-items-center rounded-xl ${mau[tone]}`}>
        <Icon name={icon} className="size-5" />
      </span>
      <span className="mt-5 block text-sm font-medium text-slate-500">{nhan}</span>
      <span className="mt-1 block text-2xl font-bold text-slate-900">{giaTri}</span>
      {moTa && <span className="mt-1 block text-xs text-slate-400">{moTa}</span>}
    </>
  )

  if (onClick) {
    return (
      <button
        type="button"
        onClick={onClick}
        className="rounded-2xl border border-slate-200 bg-white p-5 text-left shadow-sm transition hover:border-blue-400 hover:shadow-md focus-visible:ring-2 focus-visible:ring-blue-500 focus-visible:outline-none"
      >
        {noiDung}
        <span className="mt-3 inline-flex items-center gap-1 text-xs font-semibold text-blue-600">
          Xem chi tiết
          <Icon name="arrowRight" className="size-3.5" />
        </span>
      </button>
    )
  }

  return (
    <article className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
      {noiDung}
    </article>
  )
}

// ===== NHÃN TRẠNG THÁI =====
type Tone = 'blue' | 'emerald' | 'amber' | 'rose' | 'slate'

const MAU_NHAN: Record<Tone, string> = {
  blue: 'bg-blue-50 text-blue-700',
  emerald: 'bg-emerald-50 text-emerald-700',
  amber: 'bg-amber-50 text-amber-700',
  rose: 'bg-rose-50 text-rose-700',
  slate: 'bg-slate-100 text-slate-600',
}

/** Suy ra tone từ chuỗi trạng thái tiếng Việt của backend. */
export function toneTheoTrangThai(trangThai?: string | null): Tone {
  switch (trangThai) {
    case 'Đang học':
    case 'Đang diễn ra':
    case 'Có mặt':
    case 'Thành công':
    case 'Đã thanh toán':
    case 'Hoạt động':
      return 'emerald'
    case 'Chưa thanh toán':
    case 'Chưa diễn ra':
    case 'Chuẩn bị mở':
    case 'Chờ xử lý':
    case 'Đang tư vấn':
      return 'amber'
    case 'Đã hủy':
    case 'Đã kết thúc':
    case 'Vắng':
    case 'Đã nghỉ':
      return 'rose'
    case 'Vắng có phép':
    case 'Đi trễ':
    case 'Bảo lưu':
    case 'Thanh toán một phần':
      return 'amber'
    default:
      return 'slate'
  }
}

export function Nhan({ children, tone }: { children: ReactNode; tone?: Tone }) {
  const mau = MAU_NHAN[tone ?? toneTheoTrangThai(String(children))] ?? MAU_NHAN.slate

  return (
    <span
      className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-bold ${mau}`}
    >
      {children}
    </span>
  )
}

// ===== BẢNG =====
export type CotBang<T> = {
  tieuDe: string
  khoa: string
  render?: (dong: T) => ReactNode
  canPhai?: boolean
  rong?: string
}

export function Bang<T>({
  cot,
  duLieu,
  khoa,
  dangTai,
  thongBaoRong = 'Không có dữ liệu',
}: {
  cot: CotBang<T>[]
  duLieu: T[]
  khoa: (dong: T) => string | number
  dangTai?: boolean
  thongBaoRong?: string
}) {
  if (dangTai) {
    return <DangTai />
  }

  if (duLieu.length === 0) {
    return <TrongRong>{thongBaoRong}</TrongRong>
  }

  return (
    <div className="overflow-x-auto rounded-2xl border border-slate-200 bg-white shadow-sm">
      <table className="w-full min-w-max border-collapse text-sm">
        <thead>
          <tr className="border-b border-slate-100 bg-slate-50/60 text-left">
            {cot.map((c) => (
              <th
                key={c.khoa}
                className={`px-5 py-3 text-xs font-bold uppercase tracking-wider text-slate-500 ${
                  c.canPhai ? 'text-right' : ''
                }`}
              >
                {c.tieuDe}
              </th>
            ))}
          </tr>
        </thead>
        <tbody className="divide-y divide-slate-100">
          {duLieu.map((dong) => (
            <tr key={khoa(dong)} className="transition hover:bg-slate-50/70">
              {cot.map((c) => (
                <td
                  key={c.khoa}
                  className={`px-5 py-3.5 align-middle text-slate-700 ${
                    c.canPhai ? 'text-right' : ''
                  }`}
                >
                  {c.render
                    ? c.render(dong)
                    : String((dong as Record<string, unknown>)[c.khoa] ?? '—')}
                </td>
              ))}
            </tr>
          ))}
        </tbody>
      </table>
    </div>
  )
}

// ===== PHÂN TRANG =====
export function PhanTrang({
  page,
  pageSize,
  totalItems,
  totalPages,
  onDoi,
}: {
  page: number
  pageSize: number
  totalItems: number
  totalPages: number
  onDoi: (page: number) => void
}) {
  if (totalItems === 0) return null

  const tu = (page - 1) * pageSize + 1
  const den = Math.min(page * pageSize, totalItems)

  return (
    <div className="flex flex-col items-center justify-between gap-3 sm:flex-row">
      <p className="text-sm text-slate-500">
        Hiển thị <strong className="text-slate-700">{tu}</strong>–
        <strong className="text-slate-700">{den}</strong> trong{' '}
        <strong className="text-slate-700">{totalItems}</strong>
      </p>
      <div className="flex items-center gap-2">
        <button
          type="button"
          onClick={() => onDoi(page - 1)}
          disabled={page <= 1}
          className="grid size-9 place-items-center rounded-lg border border-slate-200 bg-white text-slate-600 disabled:opacity-40"
        >
          <Icon name="chevronLeft" className="size-4" />
        </button>
        <span className="text-sm font-semibold text-slate-600">
          {page} / {Math.max(totalPages, 1)}
        </span>
        <button
          type="button"
          onClick={() => onDoi(page + 1)}
          disabled={page >= totalPages}
          className="grid size-9 place-items-center rounded-lg border border-slate-200 bg-white text-slate-600 disabled:opacity-40"
        >
          <Icon name="chevronRight" className="size-4" />
        </button>
      </div>
    </div>
  )
}

// ===== TRẠNG THÁI =====
export function DangTai({ nhan = 'Đang tải dữ liệu…' }: { nhan?: string }) {
  return (
    <div className="flex items-center justify-center gap-3 rounded-2xl border border-slate-200 bg-white p-12 text-slate-500">
      <span className="size-6 animate-spin rounded-full border-2 border-blue-200 border-t-blue-600" />
      {nhan}
    </div>
  )
}

export function TrongRong({ children }: { children: ReactNode }) {
  return (
    <div className="rounded-2xl border border-dashed border-slate-300 bg-white p-12 text-center text-slate-400">
      {children}
    </div>
  )
}

export function LoiTrang({
  message,
  onThuLai,
}: {
  message: string
  onThuLai?: () => void
}) {
  return (
    <div className="rounded-2xl border border-rose-200 bg-rose-50 p-8 text-center">
      <p className="font-semibold text-rose-700">{message}</p>
      {onThuLai && (
        <button type="button" onClick={onThuLai} className={`${LOP_NUT_PHU} mt-4`}>
          Thử lại
        </button>
      )}
    </div>
  )
}

// ===== MODAL =====
export function HopThoai({
  mo,
  tieuDe,
  onDong,
  children,
  footer,
  rong = 'max-w-lg',
}: {
  mo: boolean
  tieuDe: string
  onDong: () => void
  children: ReactNode
  footer?: ReactNode
  /** Class giới hạn bề rộng hộp thoại (mặc định max-w-lg). */
  rong?: string
}) {
  if (!mo) return null

  return (
    <div
      className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/50 p-4 backdrop-blur-sm"
      onClick={onDong}
    >
      <div
        className={`w-full ${rong} overflow-hidden rounded-2xl bg-white shadow-2xl`}
        onClick={(e) => e.stopPropagation()}
      >
        <div className="flex items-start justify-between gap-4 border-b border-slate-100 p-5">
          <h2 className="text-lg font-bold text-slate-900">{tieuDe}</h2>
          <button
            type="button"
            onClick={onDong}
            className="grid size-9 place-items-center rounded-full text-slate-400 hover:bg-slate-100"
          >
            <Icon name="close" className="size-5" />
          </button>
        </div>
        <div className="max-h-[65vh] overflow-y-auto p-5">{children}</div>
        {footer && (
          <div className="flex justify-end gap-2 border-t border-slate-100 bg-slate-50/60 p-4">
            {footer}
          </div>
        )}
      </div>
    </div>
  )
}
