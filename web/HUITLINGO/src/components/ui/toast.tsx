import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import { Icon } from '../BieuTuong'

type LoaiToast = 'thanhCong' | 'loi' | 'thongTin'

type Toast = {
  id: number
  loai: LoaiToast
  noiDung: string
}

type ToastContextType = {
  hien: (noiDung: string, loai?: LoaiToast) => void
}

const ToastContext = createContext<ToastContextType | null>(null)

let boDem = 0

export function NhaCungCapToast({ children }: { children: ReactNode }) {
  const [ds, setDs] = useState<Toast[]>([])

  const hien = useCallback((noiDung: string, loai: LoaiToast = 'thongTin') => {
    const id = ++boDem
    setDs((truoc) => [...truoc, { id, loai, noiDung }])
    window.setTimeout(() => {
      setDs((truoc) => truoc.filter((t) => t.id !== id))
    }, 4000)
  }, [])

  const value = useMemo(() => ({ hien }), [hien])

  return (
    <ToastContext.Provider value={value}>
      {children}
      <div className="pointer-events-none fixed bottom-6 right-6 z-60 flex w-80 flex-col gap-2">
        {ds.map((t) => {
          const mau =
            t.loai === 'thanhCong'
              ? 'border-emerald-200 bg-emerald-50 text-emerald-800'
              : t.loai === 'loi'
                ? 'border-rose-200 bg-rose-50 text-rose-800'
                : 'border-slate-200 bg-white text-slate-700'

          return (
            <div
              key={t.id}
              className={`pointer-events-auto flex items-start gap-3 rounded-xl border px-4 py-3 text-sm font-medium shadow-lg ${mau}`}
            >
              <Icon
                name={t.loai === 'loi' ? 'alert' : 'check'}
                className="mt-0.5 size-4 shrink-0"
              />
              <span className="flex-1">{t.noiDung}</span>
            </div>
          )
        })}
      </div>
    </ToastContext.Provider>
  )
}

export function dungToast() {
  const ctx = useContext(ToastContext)
  if (!ctx) throw new Error('dungToast phải dùng trong <NhaCungCapToast>')
  return ctx
}
