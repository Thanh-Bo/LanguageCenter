// Khu vực HỌC VIÊN — "Lớp của tôi": danh sách lớp đang theo học + trang chi tiết (3 tab).
import { useMemo, useState } from 'react'
import { Link, useParams, useSearchParams } from 'react-router'
import { Icon } from '../../components/BieuTuong'
import { Bang, DangTai, LoiTrang, Nhan, TrongRong, type CotBang } from '../../components/ui'
import { hocVienSelfApi } from '../../lib/api/endpoints'
import type {
  AttendanceHistoryItem,
  ClassProgress,
  SessionListItem,
  StudentCourseGrade,
  TuitionItem,
} from '../../lib/api/types'
import { LOAI_BUOI, TRANG_THAI_CA_HOC } from '../../lib/constants'
import {
  dinhDangKhoangGio,
  dinhDangNgay,
  dinhDangTien,
  homNayIso,
  thuTrongTuan,
} from '../../lib/format'
import { dungDuLieu } from '../../lib/useApi'

// ============================================================
// DÙNG CHUNG
// ============================================================

const TAB = [
  { khoa: 'tien-do', nhan: 'Tiến độ 20 buổi' },
  { khoa: 'buoi', nhan: 'Danh sách buổi' },
  { khoa: 'diem', nhan: 'Điểm của tôi' },
] as const

type KhoaTab = (typeof TAB)[number]['khoa']

/** Màu ô/ dòng theo LOẠI BUỔI — đồng bộ với giao diện giáo viên. */
function lopTheoLoaiBuoi(b: SessionListItem): string {
  if (b.trangThai === 'Đã hủy') return 'border-rose-400 bg-rose-100 text-rose-800'
  if (b.loaiBuoi === 'Buổi thi') return 'border-violet-500 bg-violet-100 text-violet-800'
  if (b.loaiBuoi === 'Kiểm tra') return 'border-amber-500 bg-amber-100 text-amber-800'
  if (b.ngayHoc < homNayIso()) return 'border-emerald-400 bg-emerald-100 text-emerald-800'
  return 'border-blue-300 bg-blue-50 text-blue-800'
}

function lopDongBuoi(b: SessionListItem): string {
  if (b.trangThai === 'Đã hủy')
    return 'border-l-4 border-l-rose-400 bg-rose-50 hover:brightness-[0.98]'
  if (b.loaiBuoi === 'Buổi thi')
    return 'border-l-4 border-l-violet-500 bg-violet-50 hover:brightness-[0.98]'
  if (b.loaiBuoi === 'Kiểm tra')
    return 'border-l-4 border-l-amber-500 bg-amber-50 hover:brightness-[0.98]'
  if (b.ngayHoc < homNayIso())
    return 'border-l-4 border-l-emerald-400 bg-emerald-50/70 hover:brightness-[0.98]'
  return 'border-l-4 border-l-blue-300 hover:bg-slate-50/70'
}

function nhanLoaiBuoi(loaiBuoi: string): string | null {
  if (loaiBuoi === 'Buổi thi') return 'THI'
  if (loaiBuoi === 'Kiểm tra') return 'KT'
  return null
}

/** Chú thích màu dùng chung cho cả 2 tab. */
function ChuThichBuoi() {
  const o = 'flex items-center gap-1.5'
  const mau = 'size-3.5 rounded border-2'

  return (
    <div className="flex flex-wrap items-center gap-x-5 gap-y-2 text-xs font-semibold text-slate-600">
      <span className={o}>
        <span className={`${mau} border-blue-300 bg-blue-50`} /> Buổi học
      </span>
      <span className={o}>
        <span className={`${mau} border-amber-500 bg-amber-100`} /> Kiểm tra
      </span>
      <span className={o}>
        <span className={`${mau} border-violet-500 bg-violet-100`} /> Buổi thi
      </span>
      <span className={o}>
        <span className={`${mau} border-emerald-400 bg-emerald-100`} /> Đã học
      </span>
      <span className={o}>
        <span className={`${mau} border-rose-400 bg-rose-100`} /> Đã hủy
      </span>
    </div>
  )
}

// ============================================================
// DANH SÁCH LỚP
// ============================================================
export function HVLopCuaToi() {
  const { duLieu, dangTai, loi, taiLai } = dungDuLieu(async () => {
    const [diem, lichHoc] = await Promise.all([
      hocVienSelfApi.diem(),
      hocVienSelfApi.lichHoc().catch(() => [] as SessionListItem[]),
    ])

    const tienDo: Record<number, ClassProgress> = {}

    await Promise.all(
      diem.map(async (l) => {
        try {
          tienDo[l.maLopHoc] = await hocVienSelfApi.tienDoLop(l.maLopHoc)
        } catch {
          // Lớp chưa sinh buổi → bỏ qua thanh tiến độ.
        }
      }),
    )

    return { diem, lichHoc, tienDo }
  })

  if (dangTai) return <DangTai />
  if (loi) return <LoiTrang message={loi} onThuLai={taiLai} />

  const ds = [...(duLieu?.diem ?? [])].sort((a, b) => a.ngayBatDau.localeCompare(b.ngayBatDau))
  const tienDo = duLieu?.tienDo ?? {}
  const homNay = homNayIso()

  const lopCot =
    ds.length <= 1 ? 'grid-cols-1' : ds.length === 2 ? 'md:grid-cols-2' : 'md:grid-cols-2 xl:grid-cols-3'

  return (
    <div className="space-y-6">
      <div className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
        <h1 className="text-3xl font-bold tracking-tight text-slate-900">Lớp của tôi</h1>
        <p className="mt-1 text-sm text-slate-500">
          Bấm vào một lớp để xem tiến độ 20 buổi, danh sách buổi và điểm của bạn.
        </p>
      </div>

      {ds.length === 0 ? (
        <TrongRong>Bạn chưa được xếp vào lớp học nào.</TrongRong>
      ) : (
        <div className={`grid gap-4 ${lopCot}`}>
          {ds.map((l) => {
            const td = tienDo[l.maLopHoc]
            const phanTram = Math.max(0, Math.min(100, Number(td?.tienDoPhanTram) || 0))
            const keTiep =
              (td?.buois ?? []).find(
                (b) => b.ngayHoc >= homNay && b.trangThai !== 'Đã hủy',
              ) ??
              (duLieu?.lichHoc ?? []).find(
                (b) =>
                  b.maLopHoc === l.maLopHoc && b.ngayHoc >= homNay && b.trangThai !== 'Đã hủy',
              ) ??
              null

            return (
              <Link
                key={l.maDangKy}
                to={`/hocvien/lop-cua-toi/${encodeURIComponent(l.maLop)}`}
                className="group flex flex-col rounded-2xl border border-slate-200 bg-white p-5 shadow-sm transition hover:border-blue-300 hover:shadow-md"
              >
                <div className="flex items-start justify-between gap-3">
                  <p className="font-mono text-xs font-semibold text-slate-500">{l.maLop}</p>
                  <Nhan tone={l.duDieuKien ? 'emerald' : 'amber'}>
                    {l.duDieuKien ? 'Đủ điều kiện' : 'Đang học'}
                  </Nhan>
                </div>

                <h3 className="mt-1 text-lg font-bold text-slate-900 transition group-hover:text-blue-700">
                  {l.tenLop}
                </h3>
                <p className="text-sm text-slate-500">{l.tenKhoaHoc}</p>

                <div className="mt-4">
                  <div className="mb-1 flex items-center justify-between text-xs">
                    <span className="font-semibold text-slate-500">Tiến độ</span>
                    <span className="font-bold text-slate-700">
                      {td ? `${td.soBuoiDaHoc}/${td.tongSoBuoi} buổi` : 'Chưa sinh buổi'}
                    </span>
                  </div>
                  <div className="h-2 w-full overflow-hidden rounded-full bg-slate-100">
                    <div
                      className="h-full rounded-full bg-blue-600"
                      style={{ width: `${phanTram}%` }}
                    />
                  </div>
                </div>

                <p className="mt-3 text-xs text-slate-500">
                  {keTiep ? (
                    <>
                      <span className="font-semibold text-slate-600">Buổi kế tiếp:</span>{' '}
                      {thuTrongTuan(keTiep.ngayHoc)} {dinhDangNgay(keTiep.ngayHoc)}
                      {keTiep.soBuoi ? ` · Buổi ${keTiep.soBuoi}` : ''}
                      {keTiep.loaiBuoi !== 'Buổi học' ? ` · ${keTiep.loaiBuoi}` : ''}
                    </>
                  ) : (
                    'Không còn buổi nào sắp tới'
                  )}
                </p>

                <p className="mt-4 inline-flex items-center gap-1 text-sm font-semibold text-blue-600 transition group-hover:gap-2">
                  Xem chi tiết
                  <Icon name="arrowRight" className="size-4" />
                </p>
              </Link>
            )
          })}
        </div>
      )}
    </div>
  )
}

// ============================================================
// CHI TIẾT LỚP
// ============================================================
export function HVLopChiTiet() {
  const { maLop } = useParams()
  const [searchParams, setSearchParams] = useSearchParams()
  const [maCaDangChon, setMaCaDangChon] = useState<number | null>(null)
  const [locTrangThai, setLocTrangThai] = useState('')
  const [locLoaiBuoi, setLocLoaiBuoi] = useState('')

  const tabHienTai = (searchParams.get('tab') ?? 'tien-do') as KhoaTab
  const doiTab = (khoa: KhoaTab) => {
    const tiep = new URLSearchParams(searchParams)
    tiep.set('tab', khoa)
    setSearchParams(tiep, { replace: true })
  }

  const { duLieu, dangTai, loi, taiLai } = dungDuLieu(async () => {
    const diem = await hocVienSelfApi.diem()

    const maDangTim = (maLop ?? '').trim().toLowerCase()
    const lop = diem.find((d) => d.maLop.trim().toLowerCase() === maDangTim) ?? null

    if (!lop) {
      return { lop: null, tienDo: null, buoi: [], diemDanh: [], hocPhi: null }
    }

    const [tienDo, buoi, diemDanh, hocPhi] = await Promise.all([
      hocVienSelfApi.tienDoLop(lop.maLopHoc).catch(() => null),
      hocVienSelfApi
        .lichHoc({ tuNgay: lop.ngayBatDau, denNgay: lop.ngayKetThucDuKien })
        .catch(() => [] as SessionListItem[]),
      hocVienSelfApi
        .diemDanh({ tuNgay: lop.ngayBatDau, denNgay: lop.ngayKetThucDuKien })
        .catch(() => null),
      hocVienSelfApi.hocPhi({ page: 1, pageSize: 50 }).catch(() => null),
    ])

    return {
      lop,
      tienDo,
      buoi: buoi
        .filter((b) => b.maLopHoc === lop.maLopHoc)
        .sort((a, b) => `${a.ngayHoc}${a.gioBatDau}`.localeCompare(`${b.ngayHoc}${b.gioBatDau}`)),
      diemDanh: (diemDanh?.items ?? []).filter((i) => i.maLop === lop.maLop),
      hocPhi: hocPhi?.items.find((h) => h.maDangKy === lop.maDangKy) ?? null,
    }
  })

  const lop: StudentCourseGrade | null = duLieu?.lop ?? null
  const tienDo: ClassProgress | null = duLieu?.tienDo ?? null
  const buoi = duLieu?.buoi ?? []
  const diemDanh: AttendanceHistoryItem[] = duLieu?.diemDanh ?? []
  const hocPhi: TuitionItem | null = duLieu?.hocPhi ?? null

  const diemDanhTheoCa = useMemo(
    () => new Map(diemDanh.map((d) => [d.maCaHoc, d.trangThaiDiemDanh])),
    [diemDanh],
  )

  const homNay = homNayIso()

  const buoiKeTiep = useMemo(
    () => buoi.find((b) => b.ngayHoc >= homNay && b.trangThai !== 'Đã hủy') ?? null,
    [buoi, homNay],
  )

  const dsBuoi = useMemo(() => {
    let ds = buoi
    if (locTrangThai) ds = ds.filter((b) => b.trangThai === locTrangThai)
    if (locLoaiBuoi) ds = ds.filter((b) => b.loaiBuoi === locLoaiBuoi)
    return ds
  }, [buoi, locTrangThai, locLoaiBuoi])

  const buoiDangChon =
    maCaDangChon === null ? null : buoi.find((b) => b.maCaHoc === maCaDangChon) ?? null

  // ---------------------------------------------------------------- render

  if (dangTai) return <DangTai nhan="Đang tải thông tin lớp…" />
  if (loi) return <LoiTrang message={loi} onThuLai={taiLai} />

  if (!lop) {
    return (
      <div className="space-y-4">
        <Link
          to="/hocvien/lop-cua-toi"
          className="inline-flex items-center gap-1.5 text-sm font-semibold text-blue-600 hover:text-blue-700"
        >
          <Icon name="arrowRight" className="size-4 rotate-180" /> Lớp của tôi
        </Link>
        <TrongRong>Lớp này không thuộc danh sách lớp bạn đang theo học.</TrongRong>
      </div>
    )
  }

  const soBuoiDaHoc = tienDo?.soBuoiDaHoc ?? 0
  const tongSoBuoi = tienDo?.tongSoBuoi ?? 0
  const soBuoiConLai = tienDo?.soBuoiConLai ?? 0
  const phanTram = Math.max(0, Math.min(100, Number(tienDo?.tienDoPhanTram) || 0))

  const soCoMat = diemDanh.filter((d) => d.trangThaiDiemDanh === 'Có mặt').length
  const soDiTre = diemDanh.filter((d) => d.trangThaiDiemDanh === 'Đi trễ').length
  const tiLeChuyenCan =
    diemDanh.length > 0 ? Math.round(((soCoMat + soDiTre) / diemDanh.length) * 100) : null

  const cotBuoi: CotBang<SessionListItem>[] = [
    {
      tieuDe: 'Buổi',
      khoa: 'soBuoi',
      render: (b) => (
        <span className="font-bold text-slate-800">{b.soBuoi ? `Buổi ${b.soBuoi}` : '—'}</span>
      ),
    },
    {
      tieuDe: 'Ngày',
      khoa: 'ngayHoc',
      render: (b) => (
        <div>
          <p className="font-semibold text-slate-800">{dinhDangNgay(b.ngayHoc)}</p>
          <p className="text-xs text-slate-500">{thuTrongTuan(b.ngayHoc)}</p>
        </div>
      ),
    },
    {
      tieuDe: 'Giờ',
      khoa: 'gioBatDau',
      render: (b) => (
        <span className="whitespace-nowrap text-xs">
          {dinhDangKhoangGio(b.gioBatDau, b.gioKetThuc)}
        </span>
      ),
    },
    { tieuDe: 'Phòng', khoa: 'tenPhong', render: (b) => b.tenPhong },
    {
      tieuDe: 'Giáo viên',
      khoa: 'tenGiaoVienThucTe',
      render: (b) => (
        <div className="text-xs">
          <p className="font-semibold text-slate-700">{b.tenGiaoVienThucTe ?? '—'}</p>
          {b.tenTroGiangThucTe && <p className="text-slate-500">TG: {b.tenTroGiangThucTe}</p>}
        </div>
      ),
    },
    {
      tieuDe: 'Loại buổi',
      khoa: 'loaiBuoi',
      render: (b) =>
        b.loaiBuoi === 'Buổi thi' ? (
          <span className="rounded-full bg-violet-600 px-2 py-0.5 text-xs font-bold text-white">
            Buổi thi
          </span>
        ) : b.loaiBuoi === 'Kiểm tra' ? (
          <span className="rounded-full bg-amber-500 px-2 py-0.5 text-xs font-bold text-white">
            Kiểm tra
          </span>
        ) : (
          <span className="text-xs font-semibold text-slate-600">Buổi học</span>
        ),
    },
    {
      tieuDe: 'Trạng thái',
      khoa: 'trangThai',
      render: (b) => <Nhan>{b.trangThai}</Nhan>,
    },
    {
      tieuDe: 'Điểm danh',
      khoa: 'diemDanh',
      render: (b) => {
        const tt = diemDanhTheoCa.get(b.maCaHoc)
        if (!tt) return <span className="text-xs text-slate-400">—</span>

        const mau =
          tt === 'Có mặt'
            ? 'text-emerald-700'
            : tt === 'Đi trễ'
              ? 'text-amber-700'
              : 'text-rose-700'

        return <span className={`text-xs font-bold ${mau}`}>{tt}</span>
      },
    },
  ]

  return (
    <div className="space-y-6">
      {/* ============ HEADER ============ */}
      <div>
        <Link
          to="/hocvien/lop-cua-toi"
          className="inline-flex items-center gap-1.5 text-sm font-semibold text-blue-600 transition hover:text-blue-700"
        >
          <Icon name="arrowRight" className="size-4 rotate-180" /> Lớp của tôi
        </Link>

        <div className="mt-2 flex flex-wrap items-start justify-between gap-3">
          <div>
            <p className="font-mono text-xs font-semibold tracking-wide text-slate-500">
              {lop.maLop}
            </p>
            <h1 className="text-3xl font-bold tracking-tight text-slate-900">{lop.tenLop}</h1>
            <p className="mt-1 flex flex-wrap items-center gap-x-2 gap-y-1 text-sm text-slate-500">
              <span>{lop.tenKhoaHoc}</span>
              <span className="text-slate-300">·</span>
              <span>
                {dinhDangNgay(lop.ngayBatDau)} – {dinhDangNgay(lop.ngayKetThucDuKien)}
              </span>
            </p>
          </div>

          <Nhan tone={lop.duDieuKien ? 'emerald' : 'amber'}>
            {lop.duDieuKien ? 'Đủ điều kiện' : 'Đang học'}
          </Nhan>
        </div>
      </div>

      {/* ============ 4 KPI ============ */}
      <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
        <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
          <p className="text-xs font-bold uppercase tracking-wider text-slate-400">Tiến độ</p>
          <p className="mt-1 text-2xl font-bold text-slate-900">
            {soBuoiDaHoc}
            <span className="text-base font-semibold text-slate-400">/{tongSoBuoi} buổi</span>
          </p>
          <div className="mt-2 h-2 w-full overflow-hidden rounded-full bg-slate-100">
            <div className="h-full rounded-full bg-blue-600" style={{ width: `${phanTram}%` }} />
          </div>
          <p className="mt-1 text-xs text-slate-500">{phanTram}% khóa học</p>
        </div>

        <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
          <p className="text-xs font-bold uppercase tracking-wider text-slate-400">
            Đã học / còn lại
          </p>
          <p className="mt-1 text-2xl font-bold text-slate-900">
            {soBuoiDaHoc}
            <span className="text-base font-semibold text-slate-400"> · còn {soBuoiConLai}</span>
          </p>
          <p className="mt-3 text-xs text-slate-500">Điểm danh {diemDanh.length} buổi</p>
        </div>

        <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
          <p className="text-xs font-bold uppercase tracking-wider text-slate-400">
            Buổi kế tiếp
          </p>
          {buoiKeTiep ? (
            <>
              <p className="mt-1 text-lg font-bold text-slate-900">
                {thuTrongTuan(buoiKeTiep.ngayHoc)}, {dinhDangNgay(buoiKeTiep.ngayHoc)}
              </p>
              <p className="text-xs text-slate-500">
                {dinhDangKhoangGio(buoiKeTiep.gioBatDau, buoiKeTiep.gioKetThuc)} ·{' '}
                {buoiKeTiep.tenPhong}
                {buoiKeTiep.loaiBuoi !== 'Buổi học' ? ` · ${buoiKeTiep.loaiBuoi}` : ''}
              </p>
            </>
          ) : (
            <p className="mt-2 text-sm text-slate-400">Không còn buổi nào</p>
          )}
        </div>

        <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
          <p className="text-xs font-bold uppercase tracking-wider text-slate-400">
            Chuyên cần · Điểm tổng kết
          </p>
          <p className="mt-1 text-2xl font-bold text-slate-900">
            {tiLeChuyenCan == null ? '—' : `${tiLeChuyenCan}%`}
            <span className="ml-2 text-base font-semibold text-slate-400">
              · {lop.diemTongKet == null ? '—' : lop.diemTongKet}
            </span>
          </p>
          <p className="mt-3 text-xs text-slate-500">
            {hocPhi
              ? `Học phí: ${hocPhi.trangThai}`
              : 'Chưa có thông tin học phí'}
            {hocPhi ? ` · ${dinhDangTien(hocPhi.soTienConLai)} còn lại` : ''}
          </p>
        </div>
      </div>

      {/* ============ TAB ============ */}
      <div className="flex flex-wrap gap-1 rounded-xl border border-slate-200 bg-white p-1 shadow-sm">
        {TAB.map((t) => (
          <button
            key={t.khoa}
            type="button"
            onClick={() => doiTab(t.khoa)}
            className={`flex-1 rounded-lg px-3 py-2 text-sm font-semibold transition ${
              tabHienTai === t.khoa
                ? 'bg-blue-600 text-white shadow-sm'
                : 'text-slate-600 hover:bg-slate-50'
            }`}
          >
            {t.nhan}
          </button>
        ))}
      </div>

      {/* ---------- TAB 1: TIẾN ĐỘ ---------- */}
      {tabHienTai === 'tien-do' && (
        <div className="space-y-4">
          {buoi.length === 0 ? (
            <TrongRong>Lớp chưa có ca học nào.</TrongRong>
          ) : (
            <>
              {buoiDangChon && (
                <div className="rounded-2xl border border-blue-200 bg-blue-50/60 p-4">
                  <div className="flex flex-wrap items-start justify-between gap-2">
                    <div>
                      <p className="font-bold text-slate-900">
                        {buoiDangChon.soBuoi ? `Buổi ${buoiDangChon.soBuoi}` : 'Buổi học'} ·{' '}
                        {buoiDangChon.loaiBuoi}
                      </p>
                      <p className="mt-0.5 text-sm text-slate-600">
                        {thuTrongTuan(buoiDangChon.ngayHoc)},{' '}
                        {dinhDangNgay(buoiDangChon.ngayHoc)} ·{' '}
                        {dinhDangKhoangGio(buoiDangChon.gioBatDau, buoiDangChon.gioKetThuc)}
                      </p>
                      <p className="mt-1 text-sm text-slate-600">
                        Phòng {buoiDangChon.tenPhong} · GV{' '}
                        {buoiDangChon.tenGiaoVienThucTe ?? '—'}
                        {buoiDangChon.tenTroGiangThucTe
                          ? ` · TG ${buoiDangChon.tenTroGiangThucTe}`
                          : ''}
                      </p>
                      <p className="mt-1 text-sm text-slate-700">
                        <span className="font-semibold">Nội dung: </span>
                        {buoiDangChon.noiDungBaiHoc ?? 'Chưa cập nhật'}
                      </p>
                      <p className="mt-1 text-xs text-slate-500">
                        Trạng thái: {buoiDangChon.trangThai} · Điểm danh:{' '}
                        {diemDanhTheoCa.get(buoiDangChon.maCaHoc) ?? 'chưa điểm danh'}
                      </p>
                    </div>

                    <button
                      type="button"
                      onClick={() => setMaCaDangChon(null)}
                      className="grid size-9 place-items-center rounded-lg border border-slate-200 bg-white text-slate-500 transition hover:bg-slate-50"
                      title="Đóng"
                    >
                      <Icon name="close" className="size-4" />
                    </button>
                  </div>
                </div>
              )}

              <div className="space-y-4 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
                {/* 4 buổi / hàng, ô dãn đầy chiều ngang */}
                <div className="grid grid-cols-2 gap-2 lg:grid-cols-4">
                  {buoi.map((b, i) => {
                    const tt = diemDanhTheoCa.get(b.maCaHoc)

                    return (
                      <button
                        key={b.maCaHoc}
                        type="button"
                        onClick={() =>
                          setMaCaDangChon((cu) => (cu === b.maCaHoc ? null : b.maCaHoc))
                        }
                        title={`Buổi ${b.soBuoi ?? '—'} · ${thuTrongTuan(b.ngayHoc)} ${dinhDangNgay(b.ngayHoc)} · ${b.loaiBuoi} · ${b.trangThai}`}
                        className={`flex w-full flex-col rounded-xl border-2 px-3 py-2 text-left transition hover:brightness-[0.97] ${lopTheoLoaiBuoi(b)} ${
                          maCaDangChon === b.maCaHoc ? 'ring-2 ring-blue-500 ring-offset-1' : ''
                        }`}
                      >
                        <p className="flex w-full items-center gap-1.5 text-sm font-bold">
                          {b.soBuoi ? `Buổi ${b.soBuoi}` : 'Buổi'}
                          {nhanLoaiBuoi(b.loaiBuoi) && (
                            <span
                              className={`rounded px-1 py-0.5 text-[10px] font-bold text-white ${
                                b.loaiBuoi === 'Buổi thi' ? 'bg-violet-600' : 'bg-amber-500'
                              }`}
                            >
                              {nhanLoaiBuoi(b.loaiBuoi)}
                            </span>
                          )}
                          <span className="ml-auto text-[10px] font-semibold text-current opacity-60">
                            Tuần {Math.floor(i / 2) + 1}
                          </span>
                        </p>
                        <p className="mt-0.5 text-xs font-semibold">
                          {thuTrongTuan(b.ngayHoc)} · {dinhDangNgay(b.ngayHoc)}
                        </p>
                        <p
                          className={`mt-0.5 text-[11px] font-bold ${
                            tt === 'Có mặt'
                              ? 'text-emerald-700'
                              : tt === 'Đi trễ'
                                ? 'text-amber-700'
                                : tt
                                  ? 'text-rose-700'
                                  : 'text-slate-400'
                          }`}
                        >
                          {tt ?? (b.trangThai === 'Đã hủy' ? 'Đã hủy' : 'chưa điểm danh')}
                        </p>
                      </button>
                    )
                  })}
                </div>

                <div className="border-t border-slate-100 pt-3">
                  <ChuThichBuoi />
                </div>
              </div>
            </>
          )}
        </div>
      )}

      {/* ---------- TAB 2: DANH SÁCH BUỔI ---------- */}
      {tabHienTai === 'buoi' && (
        <div className="space-y-4">
          <div className="flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm sm:flex-row sm:items-end">
            <div className="sm:w-56">
              <label className="mb-1 block text-xs font-bold uppercase tracking-wider text-slate-500">
                Trạng thái
              </label>
              <select
                value={locTrangThai}
                onChange={(e) => setLocTrangThai(e.target.value)}
                className="h-10 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm"
              >
                <option value="">Tất cả</option>
                {TRANG_THAI_CA_HOC.map((t) => (
                  <option key={t} value={t}>
                    {t}
                  </option>
                ))}
              </select>
            </div>

            <div className="sm:w-56">
              <label className="mb-1 block text-xs font-bold uppercase tracking-wider text-slate-500">
                Loại buổi
              </label>
              <select
                value={locLoaiBuoi}
                onChange={(e) => setLocLoaiBuoi(e.target.value)}
                className="h-10 w-full rounded-lg border border-slate-200 bg-white px-3 text-sm"
              >
                <option value="">Tất cả</option>
                {LOAI_BUOI.map((l) => (
                  <option key={l} value={l}>
                    {l}
                  </option>
                ))}
              </select>
            </div>

            <p className="text-xs text-slate-500 sm:ml-auto">
              Hiển thị {dsBuoi.length}/{buoi.length} buổi
            </p>
          </div>

          <div className="rounded-2xl border border-slate-200 bg-white px-4 py-3 shadow-sm">
            <ChuThichBuoi />
          </div>

          <Bang
            cot={cotBuoi}
            duLieu={dsBuoi}
            khoa={(b) => b.maCaHoc}
            lopDong={lopDongBuoi}
            thongBaoRong="Không có buổi nào khớp bộ lọc"
          />
        </div>
      )}

      {/* ---------- TAB 3: ĐIỂM CỦA TÔI ---------- */}
      {tabHienTai === 'diem' && (
        <div className="space-y-4">
          {lop.thanhPhan.length === 0 ? (
            <TrongRong>Khóa học chưa có đầu điểm nào.</TrongRong>
          ) : (
            <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
              <table className="w-full border-collapse text-sm">
                <thead>
                  <tr className="border-b border-slate-100 bg-slate-50/60 text-left">
                    <th className="px-5 py-3 text-xs font-bold uppercase tracking-wider text-slate-500">
                      Đầu điểm
                    </th>
                    <th className="px-5 py-3 text-xs font-bold uppercase tracking-wider text-slate-500">
                      Trọng số
                    </th>
                    <th className="px-5 py-3 text-right text-xs font-bold uppercase tracking-wider text-slate-500">
                      Điểm
                    </th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-100">
                  {lop.thanhPhan.map((tp) => (
                    <tr key={tp.maLoaiDiem}>
                      <td className="px-5 py-3 font-semibold text-slate-800">{tp.tenLoaiDiem}</td>
                      <td className="px-5 py-3 text-slate-600">{tp.trongSo}%</td>
                      <td className="px-5 py-3 text-right">
                        <span
                          className={
                            tp.diem == null
                              ? 'font-bold text-amber-600'
                              : 'text-base font-bold text-slate-900'
                          }
                        >
                          {tp.diem == null ? 'Chưa có' : tp.diem}
                        </span>
                        <span className="text-xs text-slate-400"> /{tp.diemToiDa}</span>
                      </td>
                    </tr>
                  ))}

                  <tr className="bg-slate-50/70">
                    <td className="px-5 py-3 font-bold text-slate-900">Tổng kết</td>
                    <td className="px-5 py-3 text-slate-600">{lop.tongTrongSo}%</td>
                    <td className="px-5 py-3 text-right text-lg font-bold text-slate-900">
                      {lop.diemTongKet == null ? '—' : lop.diemTongKet}
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          )}

          {lop.thanhPhanConThieu.length > 0 && (
            <p className="rounded-xl border border-amber-200 bg-amber-50 p-3 text-sm text-amber-800">
              Còn thiếu điểm: <strong>{lop.thanhPhanConThieu.join(', ')}</strong>
            </p>
          )}

          {lop.canhBaoTrongSo && (
            <p className="rounded-xl border border-amber-200 bg-amber-50 p-3 text-sm text-amber-800">
              Tổng trọng số các đầu điểm là {lop.tongTrongSo}% (nên là 100%) — điểm tổng kết có
              thể chưa chính xác.
            </p>
          )}

          <p className="text-xs text-slate-500">
            Điều kiện hoàn thành: {lop.duDieuKien ? 'Đã đủ điều kiện ✔' : 'Chưa đủ điều kiện'}
          </p>
        </div>
      )}
    </div>
  )
}
