import { useMemo, useState } from 'react'
import { Icon } from '../../components/BieuTuong'

type View = 'Tuần' | 'Tháng'

type CaHoc = {
  id: string
  thu: number
  gio: string
  gioKetThuc: string
  tenLop: string
  maLop: string
  phong: string
  siSo: number
  siSoToiDa: number
  capDo: string
}

type Props = {
  onChuyenTrang?: (trang: string) => void
}

const CA_HOC_MOCK: CaHoc[] = [
  { id: 'c1', thu: 1, gio: '08:00', gioKetThuc: '09:30', tenLop: 'Giao tiếp cơ bản',   maLop: 'COMM-03',  phong: 'P.C1', siSo: 15, siSoToiDa: 20, capDo: 'Sơ cấp' },
  { id: 'c2', thu: 1, gio: '14:00', gioKetThuc: '15:30', tenLop: 'IELTS Foundation',   maLop: 'IELTS-07', phong: 'P.A2', siSo: 18, siSoToiDa: 18, capDo: 'Trung cấp' },
  { id: 'c3', thu: 2, gio: '09:00', gioKetThuc: '10:30', tenLop: 'TOEIC 600+',         maLop: 'TOEIC-02', phong: 'P.B1', siSo: 12, siSoToiDa: 20, capDo: 'Sơ cấp' },
  { id: 'c4', thu: 2, gio: '18:00', gioKetThuc: '20:00', tenLop: 'IELTS Foundation',   maLop: 'IELTS-07', phong: 'P.A2', siSo: 18, siSoToiDa: 18, capDo: 'Trung cấp' },
  { id: 'c5', thu: 3, gio: '08:00', gioKetThuc: '09:30', tenLop: 'Giao tiếp cơ bản',   maLop: 'COMM-03',  phong: 'P.C1', siSo: 15, siSoToiDa: 20, capDo: 'Sơ cấp' },
  { id: 'c6', thu: 3, gio: '19:00', gioKetThuc: '21:00', tenLop: 'TOEIC 600+',         maLop: 'TOEIC-02', phong: 'P.B1', siSo: 12, siSoToiDa: 20, capDo: 'Sơ cấp' },
  { id: 'c7', thu: 4, gio: '14:00', gioKetThuc: '15:30', tenLop: 'IELTS Foundation',   maLop: 'IELTS-07', phong: 'P.A2', siSo: 18, siSoToiDa: 18, capDo: 'Trung cấp' },
  { id: 'c8', thu: 5, gio: '08:00', gioKetThuc: '09:30', tenLop: 'Giao tiếp nâng cao', maLop: 'COMM-01',  phong: 'P.C3', siSo: 16, siSoToiDa: 16, capDo: 'Trung cấp' },
  { id: 'c9', thu: 5, gio: '18:00', gioKetThuc: '20:00', tenLop: 'IELTS Foundation',   maLop: 'IELTS-07', phong: 'P.A2', siSo: 18, siSoToiDa: 18, capDo: 'Trung cấp' },
  { id: 'c10', thu: 6, gio: '09:00', gioKetThuc: '11:00', tenLop: 'Giao tiếp cơ bản', maLop: 'COMM-03',  phong: 'P.C1', siSo: 15, siSoToiDa: 20, capDo: 'Sơ cấp' },
]

const TEN_THU = ['Chủ nhật', 'Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7']
const TEN_THU_NGAN = ['CN', 'T2', 'T3', 'T4', 'T5', 'T6', 'T7']

function batDauTuan(d: Date): Date {
  const copy = new Date(d)
  const thu = copy.getDay() === 0 ? 7 : copy.getDay()
  copy.setDate(copy.getDate() - (thu - 1))
  copy.setHours(0, 0, 0, 0)
  return copy
}
function themNgay(d: Date, soNgay: number): Date {
  const copy = new Date(d)
  copy.setDate(copy.getDate() + soNgay)
  return copy
}
function dinhDangNgay(d: Date): string {
  const dd = String(d.getDate()).padStart(2, '0')
  const mm = String(d.getMonth() + 1).padStart(2, '0')
  return `${dd}/${mm}`
}
function cungNgay(a: Date, b: Date): boolean {
  return (
    a.getDate() === b.getDate() &&
    a.getMonth() === b.getMonth() &&
    a.getFullYear() === b.getFullYear()
  )
}

export default function LichDay({ onChuyenTrang }: Props) {
  const [view, setView] = useState<View>('Tuần')
  const [tuanBatDau, setTuanBatDau] = useState(() => batDauTuan(new Date()))
  const [caChon, setCaChon] = useState<CaHoc | null>(null)

  const homNay = useMemo(() => new Date(), [])

  const dsNgayTrongTuan = useMemo(
    () => Array.from({ length: 7 }, (_, i) => themNgay(tuanBatDau, i)),
    [tuanBatDau],
  )

  const caTheoThu = useMemo(() => {
    const map: Record<number, CaHoc[]> = { 0: [], 1: [], 2: [], 3: [], 4: [], 5: [], 6: [] }
    CA_HOC_MOCK.forEach((ca) => map[ca.thu].push(ca))
    Object.values(map).forEach((arr) => arr.sort((a, b) => a.gio.localeCompare(b.gio)))
    return map
  }, [])

  const tongCaTrongTuan = useMemo(
    () => [1, 2, 3, 4, 5, 6, 0].reduce((sum, thu) => sum + caTheoThu[thu].length, 0),
    [caTheoThu],
  )

  const tuanTruoc = () => setTuanBatDau((d) => themNgay(d, -7))
  const tuanSau = () => setTuanBatDau((d) => themNgay(d, 7))
  const veTuanNay = () => setTuanBatDau(batDauTuan(new Date()))

  const nhanTuan = `${dinhDangNgay(tuanBatDau)} – ${dinhDangNgay(themNgay(tuanBatDau, 6))}`

  const diemDanh = () => {
    setCaChon(null)
    onChuyenTrang?.('Điểm danh')
  }

  return (
    <div className="space-y-6">
      <div className="flex flex-col justify-between gap-4 sm:flex-row sm:items-end">
        <div>
          <p className="mb-1 text-sm font-semibold text-blue-600">Giảng dạy</p>
          <h1 className="text-3xl font-bold text-slate-900">Lịch dạy</h1>
          <p className="mt-2 text-sm text-slate-500">
            {tongCaTrongTuan} ca dạy trong tuần · {nhanTuan}
          </p>
        </div>
        <div className="flex rounded-xl bg-slate-100 p-1">
          {(['Tuần', 'Tháng'] as const).map((v) => (
            <button
              key={v}
              onClick={() => setView(v)}
              className={`rounded-lg px-4 py-2 text-xs font-bold transition ${
                view === v ? 'bg-white text-blue-600 shadow-sm' : 'text-slate-500'
              }`}
            >
              {v}
            </button>
          ))}
        </div>
      </div>

      <div className="flex flex-wrap items-center justify-between gap-3 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
        <div className="flex items-center gap-2">
          <button
            onClick={tuanTruoc}
            className="grid size-10 place-items-center rounded-xl border border-slate-200 text-slate-500 hover:border-blue-200 hover:text-blue-600"
            title="Tuần trước"
          >
            <Icon name="chevronLeft" className="size-5" />
          </button>
          <button
            onClick={tuanSau}
            className="grid size-10 place-items-center rounded-xl border border-slate-200 text-slate-500 hover:border-blue-200 hover:text-blue-600"
            title="Tuần sau"
          >
            <Icon name="chevronRight" className="size-5" />
          </button>
          <span className="ml-2 text-sm font-bold text-slate-900">{nhanTuan}</span>
        </div>
        <button
          onClick={veTuanNay}
          className="rounded-xl border border-slate-200 px-4 py-2 text-xs font-semibold text-slate-600 hover:border-blue-200 hover:text-blue-600"
        >
          Về tuần này
        </button>
      </div>

      {view === 'Tuần' ? (
        <div className="overflow-x-auto rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
          <div className="grid min-w-[900px] grid-cols-7 gap-3">
            {dsNgayTrongTuan.map((ngay, idx) => {
              const thu = ngay.getDay()
              const laHomNay = cungNgay(ngay, homNay)
              const dsCa = caTheoThu[thu] ?? []
              return (
                <div key={idx} className="flex flex-col">
                  <div
                    className={`mb-3 rounded-xl px-3 py-2 text-center ${
                      laHomNay ? 'bg-blue-600 text-white' : 'bg-slate-50 text-slate-600'
                    }`}
                  >
                    <p className="text-[11px] font-bold uppercase tracking-wider opacity-80">
                      {TEN_THU_NGAN[thu]}
                    </p>
                    <p className="text-lg font-bold">{dinhDangNgay(ngay)}</p>
                  </div>
                  <div className="space-y-2">
                    {dsCa.length === 0 ? (
                      <div className="rounded-xl border border-dashed border-slate-200 p-4 text-center text-xs text-slate-400">
                        Không có ca
                      </div>
                    ) : (
                      dsCa.map((ca) => (
                        <button
                          key={ca.id}
                          onClick={() => setCaChon(ca)}
                          className="w-full rounded-xl border-l-4 border-blue-600 bg-blue-50 p-3 text-left transition hover:bg-blue-100"
                        >
                          <p className="text-[10px] font-bold text-slate-500">
                            {ca.gio}–{ca.gioKetThuc}
                          </p>
                          <p className="mt-1 text-xs font-bold text-slate-800">{ca.tenLop}</p>
                          <p className="mt-1 text-[10px] text-slate-500">
                            {ca.phong} · {ca.maLop}
                          </p>
                        </button>
                      ))
                    )}
                  </div>
                </div>
              )
            })}
          </div>
        </div>
      ) : (
        <div className="rounded-2xl border border-slate-200 bg-white p-6 shadow-sm">
          <div className="grid grid-cols-7 gap-2 text-center text-[11px] font-bold uppercase tracking-wider text-slate-400">
            {TEN_THU_NGAN.map((t) => (
              <span key={t}>{t}</span>
            ))}
          </div>
          <div className="mt-2 grid grid-cols-7 gap-2">
            {Array.from({ length: 35 }, (_, i) => {
              const ngay = themNgay(tuanBatDau, i - tuanBatDau.getDay() + 1)
              const laHomNay = cungNgay(ngay, homNay)
              const thu = ngay.getDay()
              const soCa = caTheoThu[thu].length
              const cungThang = ngay.getMonth() === tuanBatDau.getMonth()
              return (
                <div
                  key={i}
                  className={`min-h-[90px] rounded-xl border p-2 text-left ${
                    laHomNay ? 'border-blue-500 bg-blue-50' : 'border-slate-200 bg-white'
                  }`}
                >
                  <p className={`text-xs font-bold ${cungThang ? 'text-slate-700' : 'text-slate-300'}`}>
                    {ngay.getDate()}
                  </p>
                  {soCa > 0 && (
                    <div className="mt-1 space-y-1">
                      <span className="inline-block rounded-md bg-blue-600 px-1.5 py-0.5 text-[9px] font-bold text-white">
                        {soCa} ca
                      </span>
                    </div>
                  )}
                </div>
              )
            })}
          </div>
        </div>
      )}

      {caChon && (
        <div
          className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/50 p-4 backdrop-blur-sm"
          onClick={() => setCaChon(null)}
        >
          <div
            className="w-full max-w-md overflow-hidden rounded-2xl bg-white shadow-2xl"
            onClick={(e) => e.stopPropagation()}
          >
            <div className="flex items-start justify-between gap-4 border-b border-slate-100 p-6">
              <div>
                <p className="text-xs font-semibold uppercase tracking-wide text-blue-600">
                  {caChon.maLop} · {caChon.capDo}
                </p>
                <h2 className="mt-1 text-xl font-bold text-slate-900">{caChon.tenLop}</h2>
              </div>
              <button
                onClick={() => setCaChon(null)}
                className="grid size-9 place-items-center rounded-full text-slate-400 hover:bg-slate-100 hover:text-slate-600"
              >
                <Icon name="close" className="size-5" />
              </button>
            </div>
            <div className="space-y-4 p-6 text-sm">
              <div className="flex items-center gap-3 text-slate-700">
                <Icon name="clock" className="size-5 text-slate-400" />
                <span className="font-semibold">
                  {caChon.gio}–{caChon.gioKetThuc} · {TEN_THU[caChon.thu]}
                </span>
              </div>
              <div className="flex items-center gap-3 text-slate-700">
                <Icon name="room" className="size-5 text-slate-400" />
                <span>Phòng {caChon.phong}</span>
              </div>
              <div className="flex items-center gap-3 text-slate-700">
                <Icon name="users" className="size-5 text-slate-400" />
                <span>
                  Sĩ số: <b>{caChon.siSo}/{caChon.siSoToiDa}</b>
                </span>
              </div>
            </div>
            <div className="flex gap-2 border-t border-slate-100 p-4">
              <button
                onClick={() => setCaChon(null)}
                className="flex-1 rounded-xl border border-slate-200 px-4 py-3 text-sm font-semibold text-slate-600 hover:bg-slate-50"
              >
                Đóng
              </button>
              <button
                onClick={diemDanh}
                className="flex flex-1 items-center justify-center gap-2 rounded-xl bg-blue-600 px-4 py-3 text-sm font-bold text-white hover:bg-blue-700"
              >
                <Icon name="check" className="size-4" /> Điểm danh
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  )
}