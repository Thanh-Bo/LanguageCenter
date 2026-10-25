// Khu vực học viên.
import { useCallback, useEffect, useState } from 'react'
import { Link } from 'react-router'
import { Icon } from '../../components/BieuTuong'
import {
  Bang,
  DangTai,
  HopThoai,
  LOP_INPUT,
  LOP_NHAN,
  LOP_NUT_CHINH,
  LOP_NUT_PHU,
  LoiTrang,
  Nhan,
  PhanTrang,
  TheChiSo,
  TrangTieuDe,
  type CotBang,
} from '../../components/ui'
import { dungToast } from '../../components/ui/toast'
import { hocVienSelfApi, thongBaoApi } from '../../lib/api/endpoints'
import type {
  MySurvey,
  NotificationItem,
  PagedResult,
  SessionListItem,
  StudentAttendanceHistory,
  StudentCourseGrade,
  TuitionItem,
  Payment,
} from '../../lib/api/types'
import { LOAI_CAU_HOI } from '../../lib/constants'
import {
  congNgayIso,
  dauTuanIso,
  dinhDangKhoangGio,
  dinhDangNgay,
  dinhDangNgayGio,
  dinhDangTien,
  homNayIso,
  thuTrongTuan,
} from '../../lib/format'
import { dungDuLieu, thongDiepLoi } from '../../lib/useApi'

const PAGE_SIZE = 20

// ============================================================
// TỔNG QUAN
// ============================================================
/** Gom các lớp đang theo học về theo khóa học. */
function nhomTheoKhoaHoc(ds: StudentCourseGrade[]) {
  const nhom = new Map<number, { maKhoaHoc: number; tenKhoaHoc: string; lop: StudentCourseGrade[] }>()

  for (const d of ds) {
    const daCo = nhom.get(d.maKhoaHoc)
    if (daCo) {
      daCo.lop.push(d)
    } else {
      nhom.set(d.maKhoaHoc, { maKhoaHoc: d.maKhoaHoc, tenKhoaHoc: d.tenKhoaHoc, lop: [d] })
    }
  }

  return [...nhom.values()]
}

/**
 * @deprecated Bản tổng quan CŨ, không còn dùng trong router.
 * Giao diện mới nằm ở <c>src/pages/hocvien/TongQuanHocVien.tsx</c>.
 */
export function HVTongQuanCu() {
  const { duLieu, dangTai, loi, taiLai } = dungDuLieu(async () => {
    const [diem, lich, hocPhi] = await Promise.all([
      hocVienSelfApi.diem(),
      hocVienSelfApi.lichHoc({ tuNgay: homNayIso(), denNgay: homNayIso() }),
      hocVienSelfApi.hocPhi({ page: 1, pageSize: 50 }),
    ])

    // Lấy TOÀN BỘ ca học của các lớp đang theo → tính tiến độ từng khóa.
    let buoiTheoLop: Record<number, SessionListItem[]> = {}

    if (diem.length > 0) {
      const tuNgay = diem.map((d) => d.ngayBatDau).sort()[0]
      const denNgay = diem.map((d) => d.ngayKetThucDuKien).sort().pop() ?? homNayIso()

      try {
        const tatCa = await hocVienSelfApi.lichHoc({ tuNgay, denNgay })

        buoiTheoLop = tatCa.reduce<Record<number, SessionListItem[]>>((acc, b) => {
          ;(acc[b.maLopHoc] ??= []).push(b)
          return acc
        }, {})
      } catch {
        buoiTheoLop = {}
      }
    }

    const conThieu = hocPhi.items.reduce((s, h) => s + h.soTienConThieu, 0)

    return { diem, lich, conThieu, hocPhi: hocPhi.items, buoiTheoLop }
  })

  if (dangTai) return <DangTai nhan="Đang tải dữ liệu học tập…" />
  if (loi) return <LoiTrang message={loi} onThuLai={taiLai} />
  if (!duLieu) return null

  const nhomKhoaHoc = nhomTheoKhoaHoc(duLieu.diem)
  const hocPhiTheoDangKy = new Map(duLieu.hocPhi.map((h) => [h.maDangKy, h]))
  const homNay = homNayIso()

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Học tập"
        title="Tổng quan của tôi"
        desc="Tình hình học tập, lịch học hôm nay và công nợ học phí."
        actions={
          <button type="button" onClick={taiLai} className={LOP_NUT_PHU}>
            <Icon name="arrowUp" className="size-4" /> Làm mới
          </button>
        }
      />

      <div className="grid gap-4 sm:grid-cols-3">
        <TheChiSo
          nhan="Khóa học đang theo"
          giaTri={nhomKhoaHoc.length}
          icon="book"
          moTa={
            duLieu.diem.length > 0 ? `${duLieu.diem.length} lớp đang theo học` : 'Chưa được xếp lớp'
          }
        />
        <TheChiSo nhan="Ca học hôm nay" giaTri={duLieu.lich.length} icon="calendar" tone="emerald" />
        <TheChiSo
          nhan="Học phí còn thiếu"
          giaTri={dinhDangTien(duLieu.conThieu)}
          icon="wallet"
          tone={duLieu.conThieu > 0 ? 'rose' : 'emerald'}
        />
      </div>

      <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
        <h2 className="mb-4 text-sm font-bold  uppercase tracking-wider text-slate-500">
          Lịch học hôm nay
        </h2>
        {duLieu.lich.length === 0 ? (
          <p className="text-sm text-slate-400">Hôm nay bạn không có ca học nào.</p>
        ) : (
          <ul className="space-y-3">
            {duLieu.lich.map((c) => (
              <li
                key={c.maCaHoc}
                className="flex flex-wrap items-center gap-3 rounded-xl border border-slate-100 bg-slate-50/60 p-3"
              >
                <span className="grid size-10 place-items-center rounded-xl bg-blue-600 text-white">
                  <Icon name="clock" className="size-5" />
                </span>
                <div className="min-w-0 flex-1">
                  <p className="font-semibold text-slate-900">{c.tenLop}</p>
                  <p className="text-xs text-slate-500">
                    {dinhDangKhoangGio(c.gioBatDau, c.gioKetThuc)} · {c.tenPhong}
                    {c.tenGiaoVienThucTe ? ` · ${c.tenGiaoVienThucTe}` : ''}
                  </p>
                </div>
                <Nhan>{c.trangThai}</Nhan>
              </li>
            ))}
          </ul>
        )}
      </section>

      <section className="space-y-3">
        <div className="flex flex-wrap items-end justify-between gap-2">
          <div>
            <h2 className="text-sm font-bold uppercase tracking-wider text-slate-500">
              Tiến độ học của tôi
            </h2>
            <p className="text-xs text-slate-400">
              Bấm vào một khóa học để xem tiến độ từng buổi, điểm danh và điểm số.
            </p>
          </div>
        </div>

        {nhomKhoaHoc.length === 0 ? (
          <p className="rounded-2xl border border-dashed border-slate-300 bg-white p-8 text-center text-sm text-slate-500">
            Bạn chưa được xếp vào lớp học nào. Hãy liên hệ phòng học vụ để được xếp lớp.
          </p>
        ) : (
          <div className="space-y-4">
            {nhomKhoaHoc.map((khoa) => (
              <div key={khoa.maKhoaHoc} className="space-y-2">
                <p className="px-1 text-xs font-bold uppercase tracking-wider text-slate-400">
                  {khoa.tenKhoaHoc}
                </p>

                <div className="grid gap-3 md:grid-cols-2">
                  {khoa.lop.map((lop) => {
                    const buoi = duLieu.buoiTheoLop[lop.maLopHoc] ?? []
                    const tongBuoi = buoi.length
                    const soBuoiDaHoc = buoi.filter(
                      (b) =>
                        b.trangThai !== 'Đã hủy' &&
                        (b.trangThai === 'Đã hoàn thành' || b.ngayHoc < homNay),
                    ).length
                    const phanTram = tongBuoi > 0 ? Math.round((soBuoiDaHoc / tongBuoi) * 100) : 0
                    const buoiTiepTheo = buoi
                      .filter((b) => b.ngayHoc >= homNay && b.trangThai !== 'Đã hủy')
                      .sort((a, b) => a.ngayHoc.localeCompare(b.ngayHoc))[0]
                    const hocPhi = hocPhiTheoDangKy.get(lop.maDangKy)
                    const soDauDiemDaCo = lop.thanhPhan.length - lop.thanhPhanConThieu.length

                    return (
                      <Link
                        key={lop.maDangKy}
                        to={`/hocvien/khoa-hoc/${lop.maLopHoc}`}
                        className="group flex flex-col rounded-2xl border border-slate-200 bg-white p-4 shadow-sm transition hover:border-blue-300 hover:shadow-md"
                      >
                        <div className="flex items-start justify-between gap-3">
                          <div className="min-w-0">
                            <p className="text-base font-bold text-slate-900">{lop.tenLop}</p>
                            <p className="text-xs text-slate-500">
                              {lop.maLop} · {dinhDangNgay(lop.ngayBatDau)} →{' '}
                              {dinhDangNgay(lop.ngayKetThucDuKien)}
                            </p>
                          </div>
                          <Nhan tone={lop.duDieuKien ? 'emerald' : 'amber'}>
                            {lop.diemTongKet != null ? lop.diemTongKet.toFixed(2) : '—'}
                          </Nhan>
                        </div>

                        <div className="mt-3">
                          <div className="mb-1 flex items-center justify-between text-xs">
                            <span className="font-semibold text-slate-600">
                              {soBuoiDaHoc}
                              {tongBuoi > 0 ? `/${tongBuoi}` : ''} buổi
                            </span>
                            <span className="text-slate-400">{phanTram}%</span>
                          </div>
                          <div className="h-2 w-full overflow-hidden rounded-full bg-slate-100">
                            <div
                              className="h-full rounded-full bg-blue-600"
                              style={{ width: `${phanTram}%` }}
                            />
                          </div>
                        </div>

                        <div className="mt-3 flex flex-wrap gap-1.5">
                          <Nhan tone="blue">
                            Đầu điểm {soDauDiemDaCo}/{lop.thanhPhan.length}
                          </Nhan>
                          {hocPhi && hocPhi.soTienConThieu > 0 ? (
                            <Nhan tone={hocPhi.quaHan ? 'rose' : 'amber'}>
                              Còn {dinhDangTien(hocPhi.soTienConThieu)}
                            </Nhan>
                          ) : (
                            <Nhan tone="emerald">Học phí đã đủ</Nhan>
                          )}
                        </div>

                        <p className="mt-3 text-xs text-slate-500">
                          {tongBuoi === 0
                            ? 'Lớp chưa được xếp lịch buổi học.'
                            : buoiTiepTheo
                              ? `Buổi tới: ${thuTrongTuan(buoiTiepTheo.ngayHoc)}, ${dinhDangNgay(
                                  buoiTiepTheo.ngayHoc,
                                )} · ${dinhDangKhoangGio(
                                  buoiTiepTheo.gioBatDau,
                                  buoiTiepTheo.gioKetThuc,
                                )}`
                              : 'Đã học hết các buổi của lớp.'}
                        </p>

                        <span className="mt-3 inline-flex items-center gap-1 text-sm font-semibold text-blue-600 group-hover:gap-2">
                          Xem tiến độ
                          <Icon name="arrowRight" className="size-4 transition-all" />
                        </span>
                      </Link>
                    )
                  })}
                </div>
              </div>
            ))}
          </div>
        )}
      </section>
    </div>
  )
}

// ============================================================
// LỊCH HỌC — thời khóa biểu theo tuần
// ============================================================
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

function TheCaHoc({ ca }: { ca: SessionListItem }) {
  return (
    <div
      className={`rounded-lg border p-2 ${lopTheoTrangThai(ca.trangThai)}`}
      title={`${ca.tenLop} · ${ca.trangThai}${ca.noiDungBaiHoc ? ` · ${ca.noiDungBaiHoc}` : ''}`}
    >
      <p className={`text-xs font-bold ${mauChuTheoTrangThai(ca.trangThai)}`}>
        {dinhDangKhoangGio(ca.gioBatDau, ca.gioKetThuc)}
      </p>
      <p className="mt-0.5 truncate text-xs font-semibold text-slate-900">{ca.tenLop}</p>
      <p className="truncate text-xs text-slate-500">{ca.tenPhong}</p>
      {ca.tenGiaoVienThucTe ? (
        <p className="truncate text-xs text-slate-500">{ca.tenGiaoVienThucTe}</p>
      ) : null}
    </div>
  )
}

export function HVLichHoc() {
  const [tuanBatDau, setTuanBatDau] = useState(() => dauTuanIso(homNayIso()))
  const [duLieu, setDuLieu] = useState<SessionListItem[]>([])
  const [dangTai, setDangTai] = useState(true)
  const [loi, setLoi] = useState<string | null>(null)
  const [lanTai, setLanTai] = useState(0)

  const tuanKetThuc = congNgayIso(tuanBatDau, 6)
  const cacNgay = NHAN_THU.map((_, i) => congNgayIso(tuanBatDau, i))
  const homNay = homNayIso()

  useEffect(() => {
    let huy = false

    setDangTai(true)
    setLoi(null)

    hocVienSelfApi
      .lichHoc({ tuNgay: tuanBatDau, denNgay: tuanKetThuc })
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

  const caTheoNgay = new Map<string, SessionListItem[]>(cacNgay.map((n) => [n, []]))
  for (const ca of duLieu) {
    caTheoNgay.get(ca.ngayHoc)?.push(ca)
  }
  for (const ds of caTheoNgay.values()) {
    ds.sort((a, b) => a.gioBatDau.localeCompare(b.gioBatDau))
  }

  const veTruoc = () => setTuanBatDau((t) => congNgayIso(t, -7))
  const veSau = () => setTuanBatDau((t) => congNgayIso(t, 7))
  const veHienTai = () => setTuanBatDau(dauTuanIso(homNayIso()))

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Học tập"
        title="Lịch học"
        desc="Thời khóa biểu theo tuần của các lớp bạn đang theo học."
      />

      <div className="flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm sm:flex-row sm:items-center sm:justify-between">
        <div className="flex items-center gap-2">
          <button type="button" onClick={veTruoc} className={LOP_NUT_PHU}>
            <Icon name="arrowRight" className="size-4 rotate-180" />
            Tuần trước
          </button>
          <button type="button" onClick={veHienTai} className={LOP_NUT_CHINH}>
            <Icon name="calendar" className="size-4" />
            Hiện tại
          </button>
          <button type="button" onClick={veSau} className={LOP_NUT_PHU}>
            Tuần sau
            <Icon name="arrowRight" className="size-4" />
          </button>
        </div>

        <div className="sm:text-right">
          <p className="text-sm font-semibold text-slate-900">
            Tuần {dinhDangNgay(tuanBatDau)} – {dinhDangNgay(tuanKetThuc)}
          </p>
          <p className="text-xs text-slate-500">
            {dangTai ? 'Đang tải…' : `${duLieu.length} ca học trong tuần`}
          </p>
        </div>
      </div>

      {loi ? (
        <LoiTrang message={loi} onThuLai={() => setLanTai((n) => n + 1)} />
      ) : dangTai ? (
        <DangTai nhan="Đang tải lịch học…" />
      ) : (
        <>
          {duLieu.length === 0 ? (
            <p className="rounded-2xl border border-dashed border-slate-300 bg-white p-4 text-center text-sm text-slate-500">
              Tuần này bạn không có ca học nào.
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
                              <TheCaHoc key={s.maCaHoc} ca={s} />
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
    </div>
  )
}

// ============================================================
// ĐIỂM SỐ
// ============================================================
export function HVDiemSo() {
  const diem = dungDuLieu(() => hocVienSelfApi.diem())
  const diemDanh = dungDuLieu<StudentAttendanceHistory>(() => hocVienSelfApi.diemDanh())

  if (diem.dangTai) return <DangTai />
  if (diem.loi) return <LoiTrang message={diem.loi} onThuLai={diem.taiLai} />

  const ds: StudentCourseGrade[] = diem.duLieu ?? []

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Học tập"
        title="Điểm số"
        desc="Điểm tổng kết được tính theo trọng số từng thành phần do trung tâm quy định."
      />

      {ds.length === 0 ? (
        <p className="rounded-2xl border border-dashed border-slate-300 bg-white p-10 text-center text-slate-400">
          Bạn chưa có điểm nào được ghi nhận.
        </p>
      ) : (
        <div className="grid gap-4 lg:grid-cols-2">
          {ds.map((k) => (
            <article
              key={k.maDangKy}
              className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm"
            >
              <div className="flex items-start justify-between gap-3">
                <div className="min-w-0 flex-1">
                  <p className="font-mono text-xs text-slate-500">{k.maLop}</p>
                  <h3 className="text-lg font-bold text-slate-900">{k.tenLop}</h3>
                  <p className="text-sm text-slate-500">{k.tenKhoaHoc}</p>
                </div>
                <div className="shrink-0 text-right">
                  <p className="text-xs text-slate-500">Tổng kết</p>
                  <p className="text-2xl font-bold text-blue-600">
                    {k.diemTongKet !== null && k.diemTongKet !== undefined
                      ? k.diemTongKet.toFixed(2)
                      : '—'}
                  </p>
                </div>
              </div>

              <ul className="mt-4 space-y-2">
                {k.thanhPhan.map((tp) => (
                  <li key={tp.maLoaiDiem} className="flex items-center gap-2 text-sm">
                    <span className="flex-1 text-slate-600">
                      {tp.tenLoaiDiem}{' '}
                      <span className="text-xs text-slate-400">({tp.trongSo}%)</span>
                    </span>
                    <strong className="text-slate-900">
                      {tp.diem !== null && tp.diem !== undefined ? tp.diem.toFixed(2) : '—'}
                    </strong>
                  </li>
                ))}
              </ul>

              {k.thanhPhanConThieu.length > 0 && (
                <p className="mt-3 rounded-lg bg-amber-50 p-2.5 text-xs font-semibold text-amber-700">
                  Còn thiếu điểm: {k.thanhPhanConThieu.join(', ')}
                </p>
              )}
            </article>
          ))}
        </div>
      )}

      {diemDanh.duLieu && (
        <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
          <h2 className="mb-4 text-sm font-bold uppercase tracking-wider text-slate-500">
            Chuyên cần
          </h2>
          <div className="grid gap-4 sm:grid-cols-4">
            <TheChiSo nhan="Có mặt" giaTri={diemDanh.duLieu.coMat} icon="check" tone="emerald" />
            <TheChiSo nhan="Vắng" giaTri={diemDanh.duLieu.vang} icon="alert" tone="rose" />
            <TheChiSo
              nhan="Vắng có phép"
              giaTri={diemDanh.duLieu.vangCoPhep}
              icon="clock"
              tone="amber"
            />
            <TheChiSo
              nhan="Tỉ lệ có mặt"
              giaTri={`${diemDanh.duLieu.tiLeCoMat.toFixed(1)}%`}
              icon="chart"
              tone="blue"
            />
          </div>
        </section>
      )}
    </div>
  )
}

// ============================================================
// HỌC PHÍ
// ============================================================
export function HVHocPhi() {
  const [page, setPage] = useState(1)

  const hocPhi = useCallback(
    () => hocVienSelfApi.hocPhi({ page, pageSize: PAGE_SIZE }),
    [page],
  )

  const ds = useCallback(() => hocVienSelfApi.thanhToan({ page: 1, pageSize: PAGE_SIZE }), [])

  const hp = dungDuLieu<PagedResult<TuitionItem>>(hocPhi)
  const tt = dungDuLieu<PagedResult<Payment>>(ds)

  const cot: CotBang<TuitionItem>[] = [
    {
      tieuDe: 'Khóa học',
      khoa: 'tenKhoaHoc',
      render: (r) => (
        <div>
          <p className="font-semibold text-slate-900">{r.tenKhoaHoc}</p>
          {r.maLop && <p className="text-xs text-slate-500">{r.maLop}</p>}
        </div>
      ),
    },
    { tieuDe: 'Phải đóng', khoa: 'soTienPhaiDong', canPhai: true, render: (r) => dinhDangTien(r.soTienPhaiDong) },
    { tieuDe: 'Đã đóng', khoa: 'soTienDaDong', canPhai: true, render: (r) => dinhDangTien(r.soTienDaDong) },
    {
      tieuDe: 'Còn thiếu',
      khoa: 'soTienConThieu',
      canPhai: true,
      render: (r) => (
        <span className={r.soTienConThieu > 0 ? 'font-bold text-rose-600' : ''}>
          {dinhDangTien(r.soTienConThieu)}
        </span>
      ),
    },
    { tieuDe: 'Hạn đóng', khoa: 'hanThanhToan', render: (r) => dinhDangNgay(r.hanThanhToan) },
    { tieuDe: 'Trạng thái', khoa: 'trangThaiHocPhi', render: (r) => <Nhan>{r.trangThaiHocPhi}</Nhan> },
  ]

  const cotTT: CotBang<Payment>[] = [
    {
      tieuDe: 'Thời gian',
      khoa: 'ngayThanhToan',
      render: (r) => dinhDangNgayGio(r.ngayThanhToan),
    },
    {
      tieuDe: 'Khóa học',
      khoa: 'tenKhoaHoc',
      render: (r) => r.tenKhoaHoc,
    },
    { tieuDe: 'Số tiền', khoa: 'soTien', canPhai: true, render: (r) => dinhDangTien(r.soTien) },
    { tieuDe: 'Phương thức', khoa: 'phuongThucThanhToan' },
    {
      tieuDe: 'Biên nhận',
      khoa: 'bienNhan',
      render: (r) => r.bienNhan?.soBienNhan ?? '—',
    },
    { tieuDe: 'Trạng thái', khoa: 'trangThai', render: (r) => <Nhan>{r.trangThai}</Nhan> },
  ]

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Tài chính"
        title="Học phí"
        desc="Học phí và lịch sử thanh toán của bạn."
      />

      <p className="flex items-start gap-3 rounded-xl border border-blue-100 bg-blue-50 p-4 text-sm text-blue-900">
        <Icon name="alert" className="mt-0.5 size-4 shrink-0 text-blue-600" />
        <span>
          Học viên không tự xác nhận thanh toán. Vui lòng liên hệ nhân viên kế toán để được ghi
          nhận và nhận biên nhận.
        </span>
      </p>

      {hp.loi ? (
        <LoiTrang message={hp.loi} onThuLai={hp.taiLai} />
      ) : (
        <>
          <Bang
            cot={cot}
            duLieu={hp.duLieu?.items ?? []}
            khoa={(r) => r.maHocPhi}
            dangTai={hp.dangTai}
            thongBaoRong="Bạn chưa có khoản học phí nào."
          />
          {hp.duLieu && (
            <PhanTrang
              page={hp.duLieu.page}
              pageSize={hp.duLieu.pageSize}
              totalItems={hp.duLieu.totalItems}
              totalPages={hp.duLieu.totalPages}
              onDoi={setPage}
            />
          )}
        </>
      )}

      <section className="space-y-3">
        <h2 className="text-sm font-bold uppercase tracking-wider text-slate-500">
          Lịch sử thanh toán
        </h2>
        {tt.loi ? (
          <LoiTrang message={tt.loi} onThuLai={tt.taiLai} />
        ) : (
          <Bang
            cot={cotTT}
            duLieu={tt.duLieu?.items ?? []}
            khoa={(r) => r.maThanhToan}
            dangTai={tt.dangTai}
            thongBaoRong="Chưa có giao dịch nào."
          />
        )}
      </section>
    </div>
  )
}

// ============================================================
// KHẢO SÁT
// ============================================================
export function HVKhaoSat() {
  const { hien } = dungToast()
  const { duLieu, dangTai, loi, taiLai } = dungDuLieu(() => hocVienSelfApi.khaoSat())
  const [dangLam, setDangLam] = useState<MySurvey | null>(null)

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Phản hồi"
        title="Khảo sát"
        desc="Ý kiến của bạn giúp trung tâm nâng cao chất lượng đào tạo."
      />

      {loi ? (
        <LoiTrang message={loi} onThuLai={taiLai} />
      ) : dangTai ? (
        <DangTai />
      ) : (duLieu?.length ?? 0) === 0 ? (
        <p className="rounded-2xl border border-dashed border-slate-300 bg-white p-10 text-center text-slate-400">
          Hiện chưa có khảo sát nào.
        </p>
      ) : (
        <div className="grid gap-4 md:grid-cols-2">
          {duLieu?.map((k) => (
            <article
              key={k.maKhaoSat}
              className="flex flex-col rounded-2xl border border-slate-200 bg-white p-5 shadow-sm"
            >
              <div className="flex items-start justify-between gap-3">
                <h3 className="text-base font-bold text-slate-900">{k.tenKhaoSat}</h3>
                <Nhan>{k.trangThai}</Nhan>
              </div>
              <p className="mt-1 line-clamp-2 text-sm text-slate-500">{k.moTa ?? '—'}</p>
              <p className="mt-3 text-xs text-slate-400">
                {dinhDangNgay(k.ngayBatDau)} →{' '}
                {k.ngayKetThuc ? dinhDangNgay(k.ngayKetThuc) : 'không giới hạn'} ·{' '}
                {k.soCauHoi} câu hỏi
              </p>

              <div className="mt-auto pt-4">
                {k.daTraLoi ? (
                  <p className="rounded-lg bg-emerald-50 p-2.5 text-xs font-semibold text-emerald-700">
                    Bạn đã trả lời {dinhDangNgayGio(k.thoiGianTraLoi)}
                  </p>
                ) : k.dangMo ? (
                  <button type="button" onClick={() => setDangLam(k)} className={`${LOP_NUT_CHINH} w-full`}>
                    Làm khảo sát
                  </button>
                ) : (
                  <p className="rounded-lg bg-slate-100 p-2.5 text-xs font-semibold text-slate-500">
                    Khảo sát đã đóng
                  </p>
                )}
              </div>
            </article>
          ))}
        </div>
      )}

      <HopThoaiLamKhaoSat
        khaoSat={dangLam}
        onDong={() => setDangLam(null)}
        onXong={() => {
          setDangLam(null)
          hien('Đã gửi phản hồi. Cảm ơn bạn!', 'thanhCong')
          void taiLai()
        }}
      />
    </div>
  )
}

function HopThoaiLamKhaoSat({
  khaoSat,
  onDong,
  onXong,
}: {
  khaoSat: MySurvey | null
  onDong: () => void
  onXong: () => void
}) {
  const [diemChung, setDiemChung] = useState('')
  const [ghiChu, setGhiChu] = useState('')
  const [diemTheo, setDiemTheo] = useState<Record<number, string>>({})
  const [chuTheo, setChuTheo] = useState<Record<number, string>>({})
  const [dangGui, setDangGui] = useState(false)
  const [loi, setLoi] = useState<string | null>(null)

  useEffect(() => {
    setDiemChung('')
    setGhiChu('')
    setDiemTheo({})
    setChuTheo({})
    setLoi(null)
    setDangGui(false)
  }, [khaoSat])

  if (!khaoSat) return null

  const gui = async () => {
    const thieu = khaoSat.cauHois
      .filter((c) => c.batBuoc)
      .filter((c) =>
        c.loaiCauHoi === LOAI_CAU_HOI.danhGia
          ? !diemTheo[c.maCauHoi]
          : !chuTheo[c.maCauHoi]?.trim(),
      )

    if (thieu.length > 0) {
      setLoi(`Vui lòng trả lời ${thieu.length} câu hỏi bắt buộc còn trống.`)
      return
    }

    setDangGui(true)
    setLoi(null)

    try {
      await hocVienSelfApi.guiPhanHoi(khaoSat.maKhaoSat, {
        diemDanhGiaChung: diemChung ? Number(diemChung) : null,
        ghiChu: ghiChu.trim() || null,
        cauTraLois: khaoSat.cauHois.map((c) => ({
          maCauHoi: c.maCauHoi,
          giaTriDanhGia: diemTheo[c.maCauHoi] ? Number(diemTheo[c.maCauHoi]) : null,
          noiDungTraLoi: chuTheo[c.maCauHoi]?.trim() || null,
        })),
      })
      onXong()
    } catch (error) {
      setLoi(thongDiepLoi(error))
    } finally {
      setDangGui(false)
    }
  }

  return (
    <HopThoai
      mo={!!khaoSat}
      tieuDe={khaoSat.tenKhaoSat}
      onDong={onDong}
      footer={
        <>
          <button type="button" onClick={onDong} className={LOP_NUT_PHU}>
            Hủy
          </button>
          <button type="button" onClick={gui} disabled={dangGui} className={LOP_NUT_CHINH}>
            {dangGui ? 'Đang gửi…' : 'Gửi phản hồi'}
          </button>
        </>
      }
    >
      <div className="space-y-5">
        {khaoSat.moTa && <p className="text-sm text-slate-500">{khaoSat.moTa}</p>}

        <div>
          <label className={LOP_NHAN} htmlFor="diem-chung">
            Điểm đánh giá chung (0–5)
          </label>
          <select
            id="diem-chung"
            value={diemChung}
            onChange={(e) => setDiemChung(e.target.value)}
            className={LOP_INPUT}
          >
            <option value="">— Không chọn —</option>
            {[0, 1, 2, 3, 4, 5].map((d) => (
              <option key={d} value={d}>
                {d}
              </option>
            ))}
          </select>
        </div>

        {khaoSat.cauHois.map((c) => (
          <div key={c.maCauHoi} className="rounded-xl border border-slate-200 p-4">
            <p className="text-sm font-semibold text-slate-900">
              {c.thuTu}. {c.noiDungCauHoi}
              {c.batBuoc && <span className="ml-1 text-rose-500">*</span>}
            </p>

            {c.loaiCauHoi === LOAI_CAU_HOI.danhGia ? (
              <div className="mt-3 flex gap-2">
                {[1, 2, 3, 4, 5].map((d) => (
                  <button
                    key={d}
                    type="button"
                    onClick={() => setDiemTheo((cu) => ({ ...cu, [c.maCauHoi]: String(d) }))}
                    className={`size-10 rounded-xl border text-sm font-bold transition ${
                      diemTheo[c.maCauHoi] === String(d)
                        ? 'border-blue-500 bg-blue-600 text-white'
                        : 'border-slate-200 text-slate-600 hover:border-blue-200'
                    }`}
                  >
                    {d}
                  </button>
                ))}
              </div>
            ) : (
              <textarea
                rows={3}
                value={chuTheo[c.maCauHoi] ?? ''}
                onChange={(e) => setChuTheo((cu) => ({ ...cu, [c.maCauHoi]: e.target.value }))}
                className="mt-3 w-full rounded-xl border border-slate-200 p-3 text-sm outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-50"
                maxLength={2000}
              />
            )}
          </div>
        ))}

        <div>
          <label className={LOP_NHAN} htmlFor="ghi-chu-ks">
            Góp ý thêm
          </label>
          <textarea
            id="ghi-chu-ks"
            rows={3}
            value={ghiChu}
            onChange={(e) => setGhiChu(e.target.value)}
            className="w-full rounded-xl border border-slate-200 p-3 text-sm outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-50"
          />
        </div>

        {loi && (
          <p className="rounded-xl bg-rose-50 p-3 text-sm font-semibold text-rose-700">{loi}</p>
        )}
      </div>
    </HopThoai>
  )
}

// ============================================================
// THÔNG BÁO
// ============================================================
export function HVThongBao() {
  const [ketQua, setKetQua] = useState<PagedResult<NotificationItem> | null>(null)
  const [dangTai, setDangTai] = useState(true)
  const [loi, setLoi] = useState<string | null>(null)
  const [page, setPage] = useState(1)

  const tai = useCallback(async () => {
    setDangTai(true)
    setLoi(null)
    try {
      setKetQua(await thongBaoApi.cuaToi({ page, pageSize: PAGE_SIZE }))
    } catch (error) {
      setLoi(thongDiepLoi(error))
    } finally {
      setDangTai(false)
    }
  }, [page])

  useEffect(() => {
    void tai()
  }, [tai])

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Truyền thông"
        title="Thông báo"
        desc="Thông báo từ trung tâm và từ lớp bạn đang theo học."
      />

      {loi ? (
        <LoiTrang message={loi} onThuLai={tai} />
      ) : dangTai ? (
        <DangTai />
      ) : (ketQua?.items.length ?? 0) === 0 ? (
        <p className="rounded-2xl border border-dashed border-slate-300 bg-white p-10 text-center text-slate-400">
          Chưa có thông báo nào.
        </p>
      ) : (
        <>
          <ul className="space-y-3">
            {ketQua?.items.map((t) => (
              <li
                key={t.maThongBao}
                className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm"
              >
                <div className="flex flex-wrap items-center gap-3">
                  <h3 className="text-base font-bold text-slate-900">{t.tieuDe}</h3>
                  {t.loaiThongBao && <Nhan tone="blue">{t.loaiThongBao}</Nhan>}
                  <span className="ml-auto text-xs text-slate-400">
                    {dinhDangNgayGio(t.thoiGianGui)}
                  </span>
                </div>
                <p className="mt-2 whitespace-pre-line text-sm text-slate-600">{t.noiDung}</p>
              </li>
            ))}
          </ul>

          {ketQua && (
            <PhanTrang
              page={ketQua.page}
              pageSize={ketQua.pageSize}
              totalItems={ketQua.totalItems}
              totalPages={ketQua.totalPages}
              onDoi={setPage}
            />
          )}
        </>
      )}
    </div>
  )
}
