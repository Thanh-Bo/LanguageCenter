// Trang chi tiết lớp của Giáo viên / Trợ giảng: tiến độ 20 buổi, danh sách buổi, học viên & điểm.
import { useMemo, useState } from 'react'
import { Link, useNavigate, useParams, useSearchParams } from 'react-router'
import { Icon } from '../../components/BieuTuong'
import { Bang, DangTai, LoiTrang, Nhan, TrongRong, type CotBang } from '../../components/ui'
import { dungXacThuc } from '../../contexts/NguCanhXacThuc'
import { giaoVienApi } from '../../lib/api/endpoints'
import type {
  AcademicReport,
  AcademicStudentRow,
  ClassProgress,
  SessionProgressItem,
  TeacherClass,
  TeacherClassStudent,
} from '../../lib/api/types'
import { LOAI_BUOI, TRANG_THAI_CA_HOC } from '../../lib/constants'
import {
  dinhDangKhoangGio,
  dinhDangNgay,
  homNayIso,
  thuTrongTuan,
} from '../../lib/format'
import { dungDuLieu } from '../../lib/useApi'

/** Tab của trang — giữ trên URL để F5 / chia sẻ link không mất tab. */
const TAB = [
  { khoa: 'tien-do', nhan: 'Tiến độ 20 buổi' },
  { khoa: 'buoi', nhan: 'Danh sách buổi' },
  { khoa: 'hoc-vien', nhan: 'Học viên & Điểm' },
] as const

type KhoaTab = (typeof TAB)[number]['khoa']

/** Màu ô buổi theo LOẠI BUỔI (đồng bộ với lưới tuần). */
function lopOBuoi(b: SessionProgressItem): string {
  if (b.trangThai === 'Đã hủy') return 'border-rose-400 bg-rose-100 text-rose-800'
  if (b.loaiBuoi === 'Buổi thi') return 'border-violet-500 bg-violet-100 text-violet-800'
  if (b.loaiBuoi === 'Kiểm tra') return 'border-amber-500 bg-amber-100 text-amber-800'
  if (b.daHoc) return 'border-emerald-400 bg-emerald-100 text-emerald-800'
  if (b.laBuoiHienTai) return 'border-blue-600 bg-blue-100 text-blue-800'
  return 'border-blue-300 bg-blue-50 text-blue-800'
}

/** Nhãn ngắn của loại buổi để hiển thị trong ô. */
function nhanLoaiBuoi(loaiBuoi: string): string | null {
  if (loaiBuoi === 'Buổi thi') return 'THI'
  if (loaiBuoi === 'Kiểm tra') return 'KT'
  return null
}

/**
 * Màu NỀN cho từng DÒNG trong bảng "Danh sách buổi" — cùng bảng màu với lưới tiến độ:
 * Kiểm tra (hổ phách) · Buổi thi (tím) · Đã học (xanh lá) · Đã hủy (đỏ).
 */
function lopDongBuoi(b: SessionProgressItem): string {
  if (b.trangThai === 'Đã hủy')
    return 'border-l-4 border-l-rose-400 bg-rose-50 hover:brightness-[0.98]'
  if (b.loaiBuoi === 'Buổi thi')
    return 'border-l-4 border-l-violet-500 bg-violet-50 hover:brightness-[0.98]'
  if (b.loaiBuoi === 'Kiểm tra')
    return 'border-l-4 border-l-amber-500 bg-amber-50 hover:brightness-[0.98]'
  if (b.daHoc)
    return 'border-l-4 border-l-emerald-400 bg-emerald-50/70 hover:brightness-[0.98]'
  return 'border-l-4 border-l-blue-300 hover:bg-slate-50/70'
}

/** Chú thích màu dùng chung cho lưới tiến độ và bảng danh sách buổi. */
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

export function GVLopChiTiet() {
  const { maLop } = useParams()
  const navigate = useNavigate()
  const [searchParams, setSearchParams] = useSearchParams()
  const { nguoiDung } = dungXacThuc()
  const laTroGiang = nguoiDung?.vaiTro === 'Trợ giảng'

  const tabHienTai = (searchParams.get('tab') ?? 'tien-do') as KhoaTab
  const doiTab = (khoa: KhoaTab) => {
    const tiep = new URLSearchParams(searchParams)
    tiep.set('tab', khoa)
    setSearchParams(tiep, { replace: true })
  }

  /** Buổi đang mở panel chi tiết. */
  const [maCaDangChon, setMaCaDangChon] = useState<number | null>(null)

  const { duLieu, dangTai, loi, taiLai } = dungDuLieu(async () => {
    const dsLop = await giaoVienApi.lopCuaToi()

    // URL dùng MÃ LỚP (VD: IELTS-A-01) -> tra ra lớp tương ứng (không phân biệt hoa/thường).
    const maDangTim = (maLop ?? '').trim().toLowerCase()
    const lop =
      dsLop.find((l) => l.maLop.trim().toLowerCase() === maDangTim) ?? null

    if (!lop) {
      return { lop: null, tienDo: null, hocVien: [], bangDiem: null }
    }

    const [tienDo, hocVien, bangDiem] = await Promise.all([
      giaoVienApi.tienDoLop(lop.maLopHoc).catch(() => null),
      giaoVienApi.hocVienCuaLop(lop.maLopHoc).catch(() => [] as TeacherClassStudent[]),
      giaoVienApi.bangDiemLop(lop.maLopHoc).catch(() => null),
    ])

    return { lop, tienDo, hocVien, bangDiem }
  })

  const lop: TeacherClass | null = duLieu?.lop ?? null
  const tienDo: ClassProgress | null = duLieu?.tienDo ?? null
  const hocVien = duLieu?.hocVien ?? []
  const bangDiem: AcademicReport | null = duLieu?.bangDiem ?? null

  /** Danh sách buổi kèm số tuần (2 buổi/tuần) — dùng cho lưới 4 ô/hàng. */
  const dsTienDo = useMemo(
    () =>
      (tienDo?.buois ?? []).map((b, i) => ({
        b,
        tuan: Math.floor(i / 2) + 1,
      })),
    [tienDo],
  )

  /** Buổi kế tiếp: buổi gần nhất chưa hủy và từ hôm nay trở đi. */
  const buoiKeTiep = useMemo(() => {
    const homNay = homNayIso()
    return (
      (tienDo?.buois ?? []).find(
        (b) => b.ngayHoc >= homNay && b.trangThai !== 'Đã hủy',
      ) ?? null
    )
  }, [tienDo])

  /** Buổi của hôm nay (để bật nút Điểm danh). */
  const buoiHomNay = useMemo(() => {
    const homNay = homNayIso()
    return (
      (tienDo?.buois ?? []).find(
        (b) => b.ngayHoc === homNay && b.trangThai !== 'Đã hủy',
      ) ?? null
    )
  }, [tienDo])

  const [locTrangThai, setLocTrangThai] = useState('')
  const [locLoaiBuoi, setLocLoaiBuoi] = useState('')

  const dsBuoi = useMemo(() => {
    let ds = tienDo?.buois ?? []
    if (locTrangThai) ds = ds.filter((b) => b.trangThai === locTrangThai)
    if (locLoaiBuoi) ds = ds.filter((b) => b.loaiBuoi === locLoaiBuoi)
    return ds
  }, [tienDo, locTrangThai, locLoaiBuoi])

  const buoiDangChon =
    maCaDangChon === null
      ? null
      : (tienDo?.buois ?? []).find((b) => b.maCaHoc === maCaDangChon) ?? null

  // ------------------------------------------------------------------ render

  if (dangTai) return <DangTai nhan="Đang tải thông tin lớp…" />
  if (loi) return <LoiTrang message={loi} onThuLai={taiLai} />

  if (!lop) {
    return (
      <div className="space-y-4">
        <Link
          to="/giaovien/lop-cua-toi"
          className="inline-flex items-center gap-1.5 text-sm font-semibold text-blue-600 hover:text-blue-700"
        >
          <Icon name="arrowRight" className="size-4 rotate-180" /> Lớp của tôi
        </Link>

        <TrongRong>
          Không tìm thấy lớp này hoặc bạn không được phân công giảng dạy lớp đó.
        </TrongRong>
      </div>
    )
  }

  const phanTram = Math.max(0, Math.min(100, Number(tienDo?.tienDoPhanTram) || 0))
  const soBuoiDaHoc = tienDo?.soBuoiDaHoc ?? 0
  const tongSoBuoi = tienDo?.tongSoBuoi ?? 0
  const soBuoiConLai = tienDo?.soBuoiConLai ?? 0

  const cotBuoi: CotBang<SessionProgressItem>[] = [
    {
      tieuDe: 'Buổi',
      khoa: 'soBuoi',
      render: (b) => (
        <span className="font-bold text-slate-800">
          {b.soBuoi ? `Buổi ${b.soBuoi}` : '—'}
        </span>
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
      tieuDe: 'Nội dung',
      khoa: 'noiDungBaiHoc',
      render: (b) => (
        <span className="line-clamp-1 text-xs text-slate-600">
          {b.noiDungBaiHoc ?? '—'}
        </span>
      ),
      rong: 'min-w-56',
    },
    {
      tieuDe: 'Giáo viên',
      khoa: 'tenGiaoVienThucTe',
      render: (b) => (
        <div className="text-xs">
          <p className="font-semibold text-slate-700">{b.tenGiaoVienThucTe ?? '—'}</p>
          {b.tenTroGiangThucTe && (
            <p className="text-slate-500">TG: {b.tenTroGiangThucTe}</p>
          )}
        </div>
      ),
    },
    {
      tieuDe: 'Trạng thái',
      khoa: 'trangThai',
      render: (b) => <Nhan>{b.trangThai}</Nhan>,
    },
    {
      tieuDe: 'Điểm danh',
      khoa: 'soDiemDanh',
      canPhai: true,
      render: (b) => (
        <span className="font-semibold text-slate-700">{b.soDiemDanh}</span>
      ),
    },
  ]

  const cotHocVien: CotBang<AcademicStudentRow>[] = [
    {
      tieuDe: 'Học viên',
      khoa: 'hoTen',
      render: (h) => (
        <div>
          <p className="font-semibold text-slate-800">{h.hoTen}</p>
          <p className="font-mono text-xs text-slate-500">{h.maHocVien}</p>
        </div>
      ),
    },
    ...(bangDiem?.thanhPhan ?? []).map((tp) => ({
      tieuDe: `${tp.tenLoaiDiem} (${tp.trongSo}%)`,
      khoa: `tp-${tp.maLoaiDiem}`,
      canPhai: true,
      render: (h: AcademicStudentRow) => {
        const diem = h.thanhPhan.find((x) => x.maLoaiDiem === tp.maLoaiDiem)
        return (
          <span className={diem?.diem == null ? 'text-slate-400' : 'font-semibold'}>
            {diem?.diem == null ? '—' : diem.diem}
          </span>
        )
      },
    })),
    {
      tieuDe: 'Tổng kết',
      khoa: 'diemTongKet',
      canPhai: true,
      render: (h) => (
        <span
          className={
            h.diemTongKet == null
              ? 'text-slate-400'
              : 'text-base font-bold text-slate-900'
          }
        >
          {h.diemTongKet == null ? '—' : h.diemTongKet}
        </span>
      ),
    },
  ]

  return (
    <div className="space-y-6">
      {/* ============ ĐIỀU HƯỚNG + TIÊU ĐỀ ============ */}
      <div>
        <Link
          to="/giaovien/lop-cua-toi"
          className="inline-flex items-center gap-1.5 text-sm font-semibold text-blue-600 transition hover:text-blue-700"
        >
          <Icon name="arrowRight" className="size-4 rotate-180" /> Lớp của tôi
        </Link>

        <div className="mt-2 flex flex-wrap items-start justify-between gap-3">
          <div>
            <p className="font-mono text-xs font-semibold tracking-wide text-slate-500">
              {lop.maLop}
            </p>
            <h1 className="text-3xl font-bold tracking-tight text-slate-900">
              {lop.tenLop}
            </h1>
            <p className="mt-1 flex flex-wrap items-center gap-x-2 gap-y-1 text-sm text-slate-500">
              <span>{lop.tenKhoaHoc}</span>
              <span className="text-slate-300">·</span>
              <span>{lop.vaiTroPhanCong}</span>
              <span className="text-slate-300">·</span>
              <span>
                Sĩ số {lop.siSo}/{lop.siSoToiDa}
              </span>
              <span className="text-slate-300">·</span>
              <span>
                {dinhDangNgay(lop.ngayBatDau)} – {dinhDangNgay(lop.ngayKetThucDuKien)}
              </span>
            </p>
          </div>

          <div className="flex flex-wrap items-center gap-2">
            <Nhan>{lop.trangThai}</Nhan>

            {laTroGiang && buoiHomNay ? (
              <button
                type="button"
                onClick={() =>
                  navigate(
                    `/giaovien/diem-danh?ca=${buoiHomNay.maCaHoc}&ngay=${buoiHomNay.ngayHoc}`,
                  )
                }
                className="inline-flex h-10 items-center gap-1.5 rounded-xl bg-blue-600 px-4 text-sm font-bold text-white shadow-sm transition hover:bg-blue-700"
              >
                <Icon name="check" className="size-4" /> Điểm danh hôm nay
              </button>
            ) : null}

            <Link
              to="/giaovien/bang-diem"
              className="inline-flex h-10 items-center gap-1.5 rounded-xl border border-slate-200 bg-white px-4 text-sm font-semibold text-slate-700 transition hover:bg-slate-50"
            >
              <Icon name="chart" className="size-4" /> Bảng điểm
            </Link>
          </div>
        </div>
      </div>

      {/* ============ 4 KPI ============ */}
      <div className="grid gap-3 sm:grid-cols-2 xl:grid-cols-4">
        <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
          <p className="text-xs font-bold uppercase tracking-wider text-slate-400">
            Tiến độ
          </p>
          <p className="mt-1 text-2xl font-bold text-slate-900">
            {soBuoiDaHoc}
            <span className="text-base font-semibold text-slate-400">
              /{tongSoBuoi} buổi
            </span>
          </p>
          <div className="mt-2 h-2 w-full overflow-hidden rounded-full bg-slate-100">
            <div
              className="h-full rounded-full bg-blue-600"
              style={{ width: `${phanTram}%` }}
            />
          </div>
          <p className="mt-1 text-xs text-slate-500">{phanTram}% khóa học</p>
        </div>

        <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
          <p className="text-xs font-bold uppercase tracking-wider text-slate-400">
            Đã dạy / còn lại
          </p>
          <p className="mt-1 text-2xl font-bold text-slate-900">
            {soBuoiDaHoc}
            <span className="text-base font-semibold text-slate-400">
              {' '}
              · còn {soBuoiConLai}
            </span>
          </p>
          <p className="mt-3 text-xs text-slate-500">
            Đã sinh {tienDo?.soBuoiDaSinh ?? 0} ca học
          </p>
        </div>

        <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
          <p className="text-xs font-bold uppercase tracking-wider text-slate-400">
            Buổi kế tiếp
          </p>
          {buoiKeTiep ? (
            <>
              <p className="mt-1 text-lg font-bold text-slate-900">
                {thuTrongTuan(buoiKeTiep.ngayHoc)},{' '}
                {dinhDangNgay(buoiKeTiep.ngayHoc)}
              </p>
              <p className="text-xs text-slate-500">
                {dinhDangKhoangGio(buoiKeTiep.gioBatDau, buoiKeTiep.gioKetThuc)}
                {buoiKeTiep.soBuoi ? ` · Buổi ${buoiKeTiep.soBuoi}` : ''}
                {buoiKeTiep.loaiBuoi !== 'Buổi học'
                  ? ` · ${buoiKeTiep.loaiBuoi}`
                  : ''}
              </p>
            </>
          ) : (
            <p className="mt-2 text-sm text-slate-400">Không còn buổi nào</p>
          )}
        </div>

        <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
          <p className="text-xs font-bold uppercase tracking-wider text-slate-400">
            Điểm tổng kết TB
          </p>
          <p className="mt-1 text-2xl font-bold text-slate-900">
            {bangDiem?.diemTongKetTrungBinh == null
              ? '—'
              : bangDiem.diemTongKetTrungBinh}
          </p>
          <p className="mt-3 text-xs text-slate-500">
            {bangDiem
              ? `${bangDiem.soDuDieuKien}/${bangDiem.soHocVien} học viên đủ điều kiện`
              : 'Chưa có dữ liệu điểm'}
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
          {!tienDo || tienDo.buois.length === 0 ? (
            <TrongRong>
              Lớp chưa có ca học nào. Liên hệ nhân viên học vụ để sinh buổi.
            </TrongRong>
          ) : (
            <>
              {buoiDangChon && (
                <div className="rounded-2xl border border-blue-200 bg-blue-50/60 p-4">
                  <div className="flex flex-wrap items-start justify-between gap-2">
                    <div>
                      <p className="font-bold text-slate-900">
                        {buoiDangChon.soBuoi
                          ? `Buổi ${buoiDangChon.soBuoi}`
                          : 'Buổi học'}{' '}
                        · {buoiDangChon.loaiBuoi}
                      </p>
                      <p className="mt-0.5 text-sm text-slate-600">
                        {thuTrongTuan(buoiDangChon.ngayHoc)},{' '}
                        {dinhDangNgay(buoiDangChon.ngayHoc)} ·{' '}
                        {dinhDangKhoangGio(
                          buoiDangChon.gioBatDau,
                          buoiDangChon.gioKetThuc,
                        )}
                      </p>
                      <p className="mt-1 text-sm text-slate-600">
                        GV: {buoiDangChon.tenGiaoVienThucTe ?? '—'}
                        {buoiDangChon.tenTroGiangThucTe
                          ? ` · TG: ${buoiDangChon.tenTroGiangThucTe}`
                          : ''}
                      </p>
                      <p className="mt-1 text-sm text-slate-700">
                        <span className="font-semibold">Nội dung: </span>
                        {buoiDangChon.noiDungBaiHoc ?? 'Chưa cập nhật'}
                      </p>
                      <p className="mt-1 text-xs text-slate-500">
                        Trạng thái: {buoiDangChon.trangThai} · Điểm danh:{' '}
                        {buoiDangChon.soDiemDanh}
                      </p>
                    </div>

                    <div className="flex items-center gap-2">
                      {laTroGiang &&
                      buoiDangChon.trangThai !== 'Đã hủy' &&
                      buoiDangChon.ngayHoc <= homNayIso() ? (
                        <button
                          type="button"
                          onClick={() =>
                            navigate(
                              `/giaovien/diem-danh?ca=${buoiDangChon.maCaHoc}&ngay=${buoiDangChon.ngayHoc}`,
                            )
                          }
                          className="inline-flex h-9 items-center gap-1.5 rounded-lg bg-blue-600 px-3 text-sm font-bold text-white transition hover:bg-blue-700"
                        >
                          <Icon name="check" className="size-4" /> Điểm danh
                        </button>
                      ) : null}

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
                </div>
              )}

              <div className="space-y-4 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
                {/* 4 buổi / hàng, ô luôn dãn đầy chiều ngang (20 buổi chia hết cho 2 và 4) */}
                <div className="grid grid-cols-2 gap-2 lg:grid-cols-4">
                  {dsTienDo.map(({ b, tuan }) => (
                    <button
                      key={b.maCaHoc}
                      type="button"
                      onClick={() =>
                        setMaCaDangChon((cu) => (cu === b.maCaHoc ? null : b.maCaHoc))
                      }
                      title={`Buổi ${b.soBuoi ?? '—'} · ${thuTrongTuan(b.ngayHoc)} ${dinhDangNgay(b.ngayHoc)} · ${b.loaiBuoi} · ${b.trangThai}`}
                      className={`flex w-full flex-col rounded-xl border-2 px-3 py-2 text-left transition hover:brightness-[0.97] ${lopOBuoi(b)} ${
                        maCaDangChon === b.maCaHoc
                          ? 'ring-2 ring-blue-500 ring-offset-1'
                          : ''
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
                        {b.daHoc && (
                          <Icon name="check" className="size-3.5 text-emerald-600" />
                        )}
                        <span className="ml-auto text-[10px] font-semibold text-current opacity-60">
                          Tuần {tuan}
                        </span>
                      </p>
                      <p className="mt-0.5 text-xs font-semibold">
                        {thuTrongTuan(b.ngayHoc)} · {dinhDangNgay(b.ngayHoc)}
                      </p>
                    </button>
                  ))}
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
              Hiển thị {dsBuoi.length}/{tienDo?.buois.length ?? 0} buổi
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

      {/* ---------- TAB 3: HỌC VIÊN & ĐIỂM ---------- */}
      {tabHienTai === 'hoc-vien' && (
        <div className="space-y-4">
          {hocVien.length === 0 ? (
            <TrongRong>Lớp chưa có học viên nào được xếp lớp.</TrongRong>
          ) : !bangDiem || bangDiem.hocViens.length === 0 ? (
            <TrongRong>Chưa có dữ liệu điểm của lớp.</TrongRong>
          ) : (
            <>
              <Bang
                cot={cotHocVien}
                duLieu={bangDiem.hocViens}
                khoa={(h) => h.maDangKy}
                thongBaoRong="Chưa có dữ liệu điểm"
              />

              {bangDiem.canhBaoTrongSo && (
                <p className="rounded-xl border border-amber-200 bg-amber-50 p-3 text-sm text-amber-800">
                  Tổng trọng số các đầu điểm là {bangDiem.tongTrongSo}% (nên là
                  100%) — điểm tổng kết có thể chưa chính xác.
                </p>
              )}
            </>
          )}
        </div>
      )}
    </div>
  )
}
