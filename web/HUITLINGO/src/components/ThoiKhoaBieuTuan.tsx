// Thời khóa biểu dạng lưới tuần — dùng chung cho Học viên (Lịch học) và Giáo viên (Lịch dạy).
import { useEffect, useRef, useState } from 'react'
import type { SessionListItem } from '../lib/api/types'
import { congNgayIso, dauTuanIso, dinhDangKhoangGio, dinhDangNgay, homNayIso } from '../lib/format'
import { thongDiepLoi } from '../lib/useApi'
import { Icon } from './BieuTuong'
import { DangTai, LOP_NUT_CHINH, LOP_NUT_PHU, LoiTrang } from './ui'

const NHAN_THU = ['Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7', 'CN']

const CA_HOC = [
  { khoa: 'sang', nhan: 'Sáng' },
  { khoa: 'chieu', nhan: 'Chiều' },
  { khoa: 'toi', nhan: 'Tối' },
] as const

type KhoaCa = (typeof CA_HOC)[number]['khoa']

/** Ca học thuộc buổi nào — dựa trên giờ bắt đầu "HH:mm:ss". */
function caCuaBuoiHoc(gioBatDau: string): KhoaCa {
  const gio = Number.parseInt(gioBatDau.slice(0, 2), 10)
  if (gio < 12) return 'sang'
  if (gio < 18) return 'chieu'
  return 'toi'
}

function lopTheoTrangThai(trangThai: string): string {
  if (trangThai === 'Đã hủy') return 'border-rose-200 bg-rose-50'
  if (trangThai === 'Đã hoàn thành') return 'border-slate-200 bg-slate-100'
  if (trangThai === 'Đang diễn ra') return 'border-emerald-300 bg-emerald-50'
  return 'border-blue-200 bg-blue-50'
}

function mauChuTheoTrangThai(trangThai: string): string {
  if (trangThai === 'Đã hủy') return 'text-rose-700'
  if (trangThai === 'Đã hoàn thành') return 'text-slate-600'
  if (trangThai === 'Đang diễn ra') return 'text-emerald-700'
  return 'text-blue-700'
}

function TheCaHoc({
  ca,
  dongPhu,
}: {
  ca: SessionListItem
  dongPhu: (ca: SessionListItem) => Array<string | null | undefined>
}) {
  return (
    <div
      className={`rounded-lg border p-2 ${lopTheoTrangThai(ca.trangThai)}`}
      title={`${ca.tenLop} · ${ca.trangThai}${ca.noiDungBaiHoc ? ` · ${ca.noiDungBaiHoc}` : ''}`}
    >
      <p className={`text-xs font-bold ${mauChuTheoTrangThai(ca.trangThai)}`}>
        {dinhDangKhoangGio(ca.gioBatDau, ca.gioKetThuc)}
      </p>
      <p className="mt-0.5 truncate text-xs font-semibold text-slate-900">{ca.tenLop}</p>
      {dongPhu(ca)
        .filter((dong): dong is string => Boolean(dong))
        .map((dong) => (
          <p key={dong} className="truncate text-xs text-slate-500">
            {dong}
          </p>
        ))}
    </div>
  )
}

/**
 * Hook tải ca học theo tuần cho một API bất kỳ.
 * `tai` nhận `{ tuNgay, denNgay }` và trả về danh sách `SessionListItem`.
 */
export function dungLichTuan(
  tai: (q: { tuNgay: string; denNgay: string }) => Promise<SessionListItem[]>,
) {
  const [tuanBatDau, setTuanBatDau] = useState(() => dauTuanIso(homNayIso()))
  const [duLieu, setDuLieu] = useState<SessionListItem[]>([])
  const [dangTai, setDangTai] = useState(true)
  const [loi, setLoi] = useState<string | null>(null)
  const [lanTai, setLanTai] = useState(0)

  const tuanKetThuc = congNgayIso(tuanBatDau, 6)

  // Giữ tham chiếu mới nhất để không phải đưa `tai` vào dependency (tránh vòng lặp vô hạn).
  const taiRef = useRef(tai)
  taiRef.current = tai

  useEffect(() => {
    let huy = false

    setDangTai(true)
    setLoi(null)

    taiRef
      .current({ tuNgay: tuanBatDau, denNgay: tuanKetThuc })
      .then((ketQua) => {
        if (!huy) setDuLieu(ketQua)
      })
      .catch((error) => {
        if (!huy) {
          setDuLieu([])
          setLoi(thongDiepLoi(error))
        }
      })
      .finally(() => {
        if (!huy) setDangTai(false)
      })

    return () => {
      huy = true
    }
  }, [tuanBatDau, tuanKetThuc, lanTai])

  return {
    tuanBatDau,
    tuanKetThuc,
    duLieu,
    dangTai,
    loi,
    doiTuan: setTuanBatDau,
    taiLai: () => setLanTai((n) => n + 1),
  }
}

/**
 * Lưới thời khóa biểu: 3 ca (Sáng/Chiều/Tối) × 7 ngày (Thứ 2 → CN),
 * có điều hướng tuần và chú thích màu theo trạng thái ca.
 */
export function ThoiKhoaBieuTuan({
  duLieu,
  tuanBatDau,
  tuanKetThuc,
  onDoiTuan,
  dangTai,
  loi,
  onThuLai,
  dongPhu = (ca) => [ca.tenPhong, ca.tenGiaoVienThucTe],
  moTaRong = 'Tuần này bạn không có ca học nào.',
  nhanDem = 'ca học',
}: {
  duLieu: SessionListItem[]
  tuanBatDau: string
  tuanKetThuc: string
  onDoiTuan: (tuanMoi: string) => void
  dangTai: boolean
  loi?: string | null
  onThuLai?: () => void
  /** Các dòng phụ hiển thị trong thẻ ca học (mặc định: phòng + giáo viên). */
  dongPhu?: (ca: SessionListItem) => Array<string | null | undefined>
  moTaRong?: string
  nhanDem?: string
}) {
  const cacNgay = NHAN_THU.map((_, i) => congNgayIso(tuanBatDau, i))
  const homNay = homNayIso()

  const caTheoNgay = new Map<string, SessionListItem[]>(cacNgay.map((n) => [n, []]))
  for (const ca of duLieu) {
    caTheoNgay.get(ca.ngayHoc)?.push(ca)
  }
  for (const ds of caTheoNgay.values()) {
    ds.sort((a, b) => a.gioBatDau.localeCompare(b.gioBatDau))
  }

  return (
    <>
      <div className="flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm sm:flex-row sm:items-center sm:justify-between">
        <div className="flex items-center gap-2">
          <button
            type="button"
            onClick={() => onDoiTuan(congNgayIso(tuanBatDau, -7))}
            className={LOP_NUT_PHU}
          >
            <Icon name="arrowRight" className="size-4 rotate-180" />
            Tuần trước
          </button>
          <button
            type="button"
            onClick={() => onDoiTuan(dauTuanIso(homNayIso()))}
            className={LOP_NUT_CHINH}
          >
            <Icon name="calendar" className="size-4" />
            Hiện tại
          </button>
          <button
            type="button"
            onClick={() => onDoiTuan(congNgayIso(tuanBatDau, 7))}
            className={LOP_NUT_PHU}
          >
            Tuần sau
            <Icon name="arrowRight" className="size-4" />
          </button>
        </div>

        <div className="sm:text-right">
          <p className="text-sm font-semibold text-slate-900">
            Tuần {dinhDangNgay(tuanBatDau)} – {dinhDangNgay(tuanKetThuc)}
          </p>
          <p className="text-xs text-slate-500">
            {dangTai ? 'Đang tải…' : `${duLieu.length} ${nhanDem} trong tuần`}
          </p>
        </div>
      </div>

      {loi ? (
        <LoiTrang message={loi} onThuLai={onThuLai ?? (() => undefined)} />
      ) : dangTai ? (
        <DangTai nhan="Đang tải thời khóa biểu…" />
      ) : (
        <>
          {duLieu.length === 0 ? (
            <p className="rounded-2xl border border-dashed border-slate-300 bg-white p-4 text-center text-sm text-slate-500">
              {moTaRong}
            </p>
          ) : null}

          <div className="overflow-x-auto rounded-2xl border border-slate-200 bg-white shadow-sm">
            <table className="w-full min-w-225 border-collapse text-sm">
              <thead>
                <tr>
                  <th className="w-20 border-b border-slate-200 bg-slate-50 px-3 py-3 text-left text-xs font-semibold tracking-wide text-slate-500 uppercase">
                    Ca
                  </th>
                  {cacNgay.map((ngay, i) => (
                    <th
                      key={ngay}
                      className={`border-b border-l border-slate-200 px-2 py-3 text-center ${
                        ngay === homNay ? 'bg-blue-50' : 'bg-slate-50'
                      }`}
                    >
                      <p className="text-xs font-semibold tracking-wide text-slate-500 uppercase">
                        {NHAN_THU[i]}
                      </p>
                      <p
                        className={`text-sm font-bold ${
                          ngay === homNay ? 'text-blue-600' : 'text-slate-800'
                        }`}
                      >
                        {dinhDangNgay(ngay)}
                      </p>
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody>
                {CA_HOC.map((ca) => (
                  <tr key={ca.khoa}>
                    <th className="border-b border-slate-200 bg-slate-50 px-3 py-4 text-left align-top text-xs font-semibold text-slate-600">
                      {ca.nhan}
                    </th>
                    {cacNgay.map((ngay) => {
                      const ds = (caTheoNgay.get(ngay) ?? []).filter(
                        (s) => caCuaBuoiHoc(s.gioBatDau) === ca.khoa,
                      )

                      return (
                        <td
                          key={ngay}
                          className={`h-28 border-b border-l border-slate-200 p-1.5 align-top ${
                            ngay === homNay ? 'bg-blue-50/40' : ''
                          }`}
                        >
                          <div className="space-y-1">
                            {ds.map((s) => (
                              <TheCaHoc key={s.maCaHoc} ca={s} dongPhu={dongPhu} />
                            ))}
                          </div>
                        </td>
                      )
                    })}
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="flex flex-wrap items-center gap-x-5 gap-y-2 text-xs text-slate-500">
            <span className="flex items-center gap-1.5">
              <span className="size-3 rounded border border-blue-200 bg-blue-50" /> Chưa diễn ra
            </span>
            <span className="flex items-center gap-1.5">
              <span className="size-3 rounded border border-emerald-300 bg-emerald-50" /> Đang diễn ra
            </span>
            <span className="flex items-center gap-1.5">
              <span className="size-3 rounded border border-slate-200 bg-slate-100" /> Đã hoàn thành
            </span>
            <span className="flex items-center gap-1.5">
              <span className="size-3 rounded border border-rose-200 bg-rose-50" /> Đã hủy
            </span>
          </div>
        </>
      )}
    </>
  )
}
