import { useMemo, useState } from 'react'
import { Icon, type IconName } from '../../components/BieuTuong'

type LoaiThongBao = 'Lịch học' | 'Điểm số' | 'Học phí' | 'Hệ thống'

type ThongBao = {
  id: string
  loai: LoaiThongBao
  tieuDe: string
  noiDung: string
  thoiGian: string
  daDoc: boolean
}

const THONG_BAO_MOCK: ThongBao[] = [
  { id: 'tb1', loai: 'Lịch học', tieuDe: 'Lớp IELTS-07 đổi phòng học', noiDung: 'Buổi học Thứ 4 (16/10) chuyển từ P.A2 sang P.A4. Vui lòng cập nhật.', thoiGian: '5 phút trước', daDoc: false },
  { id: 'tb2', loai: 'Hệ thống', tieuDe: 'Cập nhật điểm cuối kỳ lớp COMM-03', noiDung: 'Hạn nhập điểm cuối kỳ là 20/10/2026. Vui lòng hoàn tất đúng hạn.', thoiGian: '28 phút trước', daDoc: false },
  { id: 'tb3', loai: 'Học phí', tieuDe: 'Học viên Nguyễn Thị Lan đã đóng học phí', noiDung: 'Lớp COMM-03 · Số tiền: 2.500.000đ. Biên lai đã được phát hành.', thoiGian: '2 giờ trước', daDoc: false },
  { id: 'tb4', loai: 'Điểm số', tieuDe: 'Có 3 bài kiểm tra giữa kỳ cần chấm', noiDung: 'Lớp TOEIC-02 · Hạn chấm: 18/10/2026.', thoiGian: 'Hôm qua', daDoc: false },
  { id: 'tb5', loai: 'Lịch học', tieuDe: 'Nghỉ lễ 20/10', noiDung: 'Tất cả các lớp nghỉ ngày 20/10/2026. Lịch bù sẽ được thông báo sau.', thoiGian: '2 ngày trước', daDoc: true },
  { id: 'tb6', loai: 'Hệ thống', tieuDe: 'Bảo trì hệ thống định kỳ', noiDung: 'Hệ thống sẽ bảo trì từ 23:00 ngày 25/10 đến 02:00 ngày 26/10/2026.', thoiGian: '3 ngày trước', daDoc: true },
  { id: 'tb7', loai: 'Điểm số', tieuDe: 'Điểm danh lớp IELTS-07 chưa được lưu', noiDung: 'Buổi học 14/10/2026 chưa có dữ liệu điểm danh. Vui lòng kiểm tra.', thoiGian: '4 ngày trước', daDoc: true },
  { id: 'tb8', loai: 'Học phí', tieuDe: '3 học viên sắp đến hạn đóng học phí', noiDung: 'Lớp COMM-01 · Hạn đóng: 25/10/2026.', thoiGian: '5 ngày trước', daDoc: true },
]

const ICON_THEO_LOAI: Record<LoaiThongBao, IconName> = {
  'Lịch học': 'calendar',
  'Điểm số': 'chart',
  'Học phí': 'wallet',
  'Hệ thống': 'alert',
}

const MAU_THEO_LOAI: Record<LoaiThongBao, string> = {
  'Lịch học': 'bg-blue-100 text-blue-700',
  'Điểm số': 'bg-emerald-100 text-emerald-700',
  'Học phí': 'bg-amber-100 text-amber-700',
  'Hệ thống': 'bg-rose-100 text-rose-700',
}

type Tab = 'Tất cả' | 'Chưa đọc' | 'Đã đọc'

export default function ThongBao() {
  const [dsThongBao, setDsThongBao] = useState<ThongBao[]>(THONG_BAO_MOCK)
  const [tab, setTab] = useState<Tab>('Tất cả')

  const soChuaDoc = useMemo(
    () => dsThongBao.filter((tb) => !tb.daDoc).length,
    [dsThongBao],
  )

  const dsHienThi = useMemo(() => {
    if (tab === 'Chưa đọc') return dsThongBao.filter((tb) => !tb.daDoc)
    if (tab === 'Đã đọc') return dsThongBao.filter((tb) => tb.daDoc)
    return dsThongBao
  }, [dsThongBao, tab])

  const danhDauDaDoc = (id: string) => {
    setDsThongBao((ds) =>
      ds.map((tb) => (tb.id === id ? { ...tb, daDoc: true } : tb)),
    )
  }

  const danhDauTatCa = () => {
    setDsThongBao((ds) => ds.map((tb) => ({ ...tb, daDoc: true })))
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col justify-between gap-4 sm:flex-row sm:items-end">
        <div>
          <p className="mb-1 text-sm font-semibold text-blue-600">Cá nhân</p>
          <h1 className="text-3xl font-bold text-slate-900">Thông báo</h1>
          <p className="mt-2 text-sm text-slate-500">
            {soChuaDoc > 0
              ? `Bạn có ${soChuaDoc} thông báo chưa đọc.`
              : 'Bạn đã đọc hết tất cả thông báo.'}
          </p>
        </div>
        <button
          onClick={danhDauTatCa}
          disabled={soChuaDoc === 0}
          className="flex h-11 items-center gap-2 rounded-xl border border-slate-200 bg-white px-5 text-sm font-semibold text-slate-700 hover:border-blue-200 hover:text-blue-600 disabled:cursor-not-allowed disabled:opacity-40"
        >
          <Icon name="check" className="size-4" /> Đánh dấu tất cả đã đọc
        </button>
      </div>

      <div className="flex flex-wrap gap-2">
        {(['Tất cả', 'Chưa đọc', 'Đã đọc'] as const).map((t) => {
          const so =
            t === 'Tất cả'
              ? dsThongBao.length
              : t === 'Chưa đọc'
                ? soChuaDoc
                : dsThongBao.length - soChuaDoc
          const active = tab === t
          return (
            <button
              key={t}
              onClick={() => setTab(t)}
              className={`rounded-xl px-4 py-2 text-sm font-semibold transition ${
                active
                  ? 'bg-blue-600 text-white shadow-sm shadow-blue-200'
                  : 'border border-slate-200 bg-white text-slate-600 hover:border-blue-200'
              }`}
            >
              {t} <span className="opacity-70">({so})</span>
            </button>
          )
        })}
      </div>

      {dsHienThi.length === 0 ? (
        <div className="rounded-2xl border border-dashed border-slate-300 bg-white p-12 text-center text-slate-400">
          Không có thông báo nào
        </div>
      ) : (
        <div className="space-y-3">
          {dsHienThi.map((tb) => (
            <button
              key={tb.id}
              onClick={() => danhDauDaDoc(tb.id)}
              className={`flex w-full items-start gap-4 rounded-2xl border bg-white p-5 text-left shadow-sm transition hover:border-blue-200 hover:shadow-md ${
                tb.daDoc ? 'border-slate-200 opacity-90' : 'border-blue-200'
              }`}
            >
              <span
                className={`grid size-11 shrink-0 place-items-center rounded-xl ${MAU_THEO_LOAI[tb.loai]}`}
              >
                <Icon name={ICON_THEO_LOAI[tb.loai]} className="size-5" />
              </span>
              <div className="min-w-0 flex-1">
                <div className="flex flex-wrap items-center gap-2">
                  <span
                    className={`rounded-full px-2 py-0.5 text-[10px] font-bold uppercase tracking-wider ${MAU_THEO_LOAI[tb.loai]}`}
                  >
                    {tb.loai}
                  </span>
                  {!tb.daDoc && (
                    <span className="rounded-full bg-blue-600 px-2 py-0.5 text-[10px] font-bold text-white">
                      Mới
                    </span>
                  )}
                </div>
                <h3
                  className={`mt-2 text-sm ${
                    tb.daDoc ? 'font-semibold text-slate-700' : 'font-bold text-slate-900'
                  }`}
                >
                  {tb.tieuDe}
                </h3>
                <p className="mt-1 text-sm leading-6 text-slate-500">{tb.noiDung}</p>
                <p className="mt-2 text-xs text-slate-400">{tb.thoiGian}</p>
              </div>
              <Icon name="arrowRight" className="mt-1 size-4 shrink-0 text-slate-300" />
            </button>
          ))}
        </div>
      )}
    </div>
  )
}