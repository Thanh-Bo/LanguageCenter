// Khu vực giáo viên / trợ giảng.
import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { useNavigate, useSearchParams } from 'react-router'
import { Icon } from '../../components/BieuTuong'
import {
  DangTai,
  LOP_INPUT,
  LOP_NHAN,
  LOP_NUT_CHINH,
  LOP_NUT_PHU,
  LoiTrang,
  Nhan,
  PhanTrang,
  TheChiSo,
  TrangTieuDe,
} from '../../components/ui'
import { dungToast } from '../../components/ui/toast'
import { ThoiKhoaBieuTuan, dungLichTuan } from '../../components/ThoiKhoaBieuTuan'
import { giaoVienApi, thongBaoApi } from '../../lib/api/endpoints'
import type {
  AcademicReport,
  AttendanceSheet,
  GradeComponent,
  NotificationItem,
  PagedResult,
  SessionListItem,
  TeacherClass,
} from '../../lib/api/types'
import { TRANG_THAI_CA_HOC, TRANG_THAI_DIEM_DANH } from '../../lib/constants'
import {
  congNgayIso,
  dauTuanIso,
  dinhDangKhoangGio,
  dinhDangNgay,
  dinhDangNgayGio,
  homNayIso,
  ngayTruocIso,
  thuTrongTuan,
} from '../../lib/format'
import { dungDuLieu, thongDiepLoi } from '../../lib/useApi'

// ============================================================
// TỔNG QUAN
// ============================================================
/**
 * @deprecated Bản tổng quan CŨ, không còn dùng trong router.
 * Giao diện mới nằm ở <c>src/pages/giaovien/TongQuanGiaoVien.tsx</c>.
 */
export function GVTongQuanCu() {
  const { duLieu, dangTai, loi, taiLai } = dungDuLieu(async () => {
    const [lop, lich] = await Promise.all([
      giaoVienApi.lopCuaToi(),
      giaoVienApi.lichDay({ tuNgay: ngayTruocIso(3), denNgay: homNayIso() }),
    ])

    return {
      lop,
      sapToi: lich
        .filter((c) => c.ngayHoc >= homNayIso())
        .sort((a, b) => `${a.ngayHoc}${a.gioBatDau}`.localeCompare(`${b.ngayHoc}${b.gioBatDau}`))
        .slice(0, 5),
    }
  })

  if (dangTai) return <DangTai nhan="Đang tải dữ liệu giảng dạy…" />
  if (loi) return <LoiTrang message={loi} onThuLai={taiLai} />
  if (!duLieu) return null

  const tongHocVien = duLieu.lop.reduce((s, l) => s + l.siSo, 0)

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Giảng dạy"
        title="Tổng quan giảng dạy"
        desc="Các lớp bạn đang phụ trách và những ca học sắp tới."
        actions={
          <button type="button" onClick={taiLai} className={LOP_NUT_PHU}>
            <Icon name="arrowUp" className="size-4" /> Làm mới
          </button>
        }
      />

      <div className="grid gap-4 sm:grid-cols-3">
        <TheChiSo nhan="Lớp đang phụ trách" giaTri={duLieu.lop.length} icon="book" />
        <TheChiSo nhan="Tổng học viên" giaTri={tongHocVien} icon="users" tone="emerald" />
        <TheChiSo nhan="Ca học sắp tới" giaTri={duLieu.sapToi.length} icon="calendar" tone="amber" />
      </div>

      <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
        <h2 className="mb-4 text-sm font-bold tracking-wider text-slate-500">
          Ca học gần nhất
        </h2>
        {duLieu.sapToi.length === 0 ? (
          <p className="text-sm text-slate-400">Không có ca học sắp tới.</p>
        ) : (
          <ul className="space-y-3">
            {duLieu.sapToi.map((c) => (
              <li
                key={c.maCaHoc}
                className="flex flex-wrap items-center gap-3 rounded-xl border border-slate-100 bg-slate-50/60 p-3"
              >
                <span className="grid size-10 place-items-center rounded-xl bg-blue-600 text-white">
                  <Icon name="calendar" className="size-5" />
                </span>
                <div className="min-w-0 flex-1">
                  <p className="font-semibold text-slate-900">
                    {c.maLop} · {c.tenLop}
                  </p>
                  <p className="text-xs text-slate-500">
                    {dinhDangNgay(c.ngayHoc)} · {dinhDangKhoangGio(c.gioBatDau, c.gioKetThuc)} ·{' '}
                    {c.tenPhong}
                  </p>
                </div>
                <Nhan>{c.trangThai}</Nhan>
              </li>
            ))}
          </ul>
        )}
      </section>
    </div>
  )
}

// ============================================================
// LỚP CỦA TÔI
// ============================================================
export function GVLopCuaToi() {
  const { duLieu, dangTai, loi, taiLai } = dungDuLieu(() => giaoVienApi.lopCuaToi())

  if (dangTai) return <DangTai />
  if (loi) return <LoiTrang message={loi} onThuLai={taiLai} />

  const ds: TeacherClass[] = duLieu ?? []

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Giảng dạy"
        title="Lớp của tôi"
        desc="Danh sách lớp được phân công giảng dạy hoặc trợ giảng."
      />

      {ds.length === 0 ? (
        <p className="rounded-2xl border border-dashed border-slate-300 bg-white p-10 text-center text-slate-400">
          Bạn chưa được phân công lớp nào.
        </p>
      ) : (
        <div className="grid gap-4 md:grid-cols-2 xl:grid-cols-3">
          {ds.map((l) => (
            <article
              key={l.maLopHoc}
              className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm"
            >
              <div className="flex items-start justify-between gap-3">
                <p className="font-mono text-xs text-slate-500">{l.maLop}</p>
                <span className="shrink-0">
                  <Nhan>{l.trangThai}</Nhan>
                </span>
              </div>

              {/* Tên lớp chiếm TOÀN BỘ chiều rộng card → hiện đủ chữ, không bị "..." */}
              <h3 className="mt-1 text-lg font-bold text-slate-900">{l.tenLop}</h3>
              <p className="text-sm text-slate-500">{l.tenKhoaHoc}</p>

              <dl className="mt-4 grid grid-cols-2 gap-2 text-xs">
                <dt className="text-slate-500">Sĩ số</dt>
                <dd className="text-right font-semibold">
                  {l.siSo}/{l.siSoToiDa}
                </dd>
                <dt className="text-slate-500">Bắt đầu</dt>
                <dd className="text-right font-semibold">{dinhDangNgay(l.ngayBatDau)}</dd>
                <dt className="text-slate-500">Kết thúc dự kiến</dt>
                <dd className="text-right font-semibold">{dinhDangNgay(l.ngayKetThucDuKien)}</dd>
                <dt className="text-slate-500">Vai trò</dt>
                <dd className="text-right font-semibold">{l.vaiTroPhanCong}</dd>
              </dl>
            </article>
          ))}
        </div>
      )}
    </div>
  )
}

// ============================================================
// LỊCH DẠY
// ============================================================
export function GVLichDay() {
  const lich = dungLichTuan((q) => giaoVienApi.lichDay(q))
  const navigate = useNavigate()

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Giảng dạy"
        title="Lịch dạy"
        desc="Thời khóa biểu theo tuần của các lớp bạn phụ trách. Bấm vào một ca để điểm danh."
      />

      <ThoiKhoaBieuTuan
        duLieu={lich.duLieu}
        tuanBatDau={lich.tuanBatDau}
        tuanKetThuc={lich.tuanKetThuc}
        onDoiTuan={lich.doiTuan}
        dangTai={lich.dangTai}
        loi={lich.loi}
        onThuLai={lich.taiLai}
        dongPhu={(ca) => [ca.tenPhong, ca.tenKhoaHoc]}
        moTaRong="Tuần này bạn không có ca dạy nào."
        nhanDem="ca dạy"
        onChonCaHoc={(ca) =>
          navigate(`/giaovien/diem-danh?ca=${ca.maCaHoc}&ngay=${ca.ngayHoc}`)
        }
      />
    </div>
  )
}

// ============================================================
// ĐIỂM DANH
// ============================================================
/**
 * 4 trạng thái điểm danh — thứ tự hiển thị và màu nút cho dễ bấm
 * (giá trị phải khớp CHÍNH XÁC `TRANG_THAI_DIEM_DANH`).
 */
const NUT_DIEM_DANH = [
  { giaTri: 'Có mặt', lop: 'border-emerald-500 bg-emerald-50 text-emerald-700' },
  { giaTri: 'Đi trễ', lop: 'border-amber-500 bg-amber-50 text-amber-700' },
  { giaTri: 'Vắng có phép', lop: 'border-sky-500 bg-sky-50 text-sky-700' },
  { giaTri: 'Vắng', lop: 'border-rose-500 bg-rose-50 text-rose-700' },
] as const

export function GVDiemDanh() {
  const { hien } = dungToast()

  const [searchParams, setSearchParams] = useSearchParams()

  // Ca đang mở lấy TRỰC TIẾP từ URL: /giaovien/diem-danh?ca=<maCaHoc>
  // → bấm 1 dòng trong bảng chỉ đổi URL, không dùng hộp thoại.
  const maCaDangChon = Number(searchParams.get('ca')) || null

  /** Mở ca học: chỉ đổi URL. */
  const moCa = (id: number) => setSearchParams({ ca: String(id) })

  // ---- Bộ lọc danh sách ca học ----
  const [tuKhoa, setTuKhoa] = useState('')
  const [locTrangThai, setLocTrangThai] = useState('')
  const [locLop, setLocLop] = useState('')

  // Tải TOÀN BỘ ca học của các lớp được phân công (lọc ở client cho nhanh).
  const dsCa = dungDuLieu(async () => {
    const lop = await giaoVienApi.lopCuaToi()

    if (lop.length === 0) {
      return { lop, ca: [] as SessionListItem[] }
    }

    const tuNgay = lop.map((l) => l.ngayBatDau).sort()[0]
    const denNgay = lop.map((l) => l.ngayKetThucDuKien).sort().pop() ?? homNayIso()

    return { lop, ca: await giaoVienApi.lichDay({ tuNgay, denNgay }) }
  })

  const phieu = dungDuLieu<AttendanceSheet | null>(async () =>
    maCaDangChon ? giaoVienApi.bangDiemDanh(maCaDangChon) : null,
  )

  const taiPhieu = phieu.taiLai

  useEffect(() => {
    void taiPhieu()
  }, [taiPhieu, maCaDangChon])

  /** Cuộn tới phiếu điểm danh ngay khi dữ liệu ca về. */
  const khungPhieu = useRef<HTMLElement | null>(null)

  useEffect(() => {
    if (phieu.duLieu) {
      khungPhieu.current?.scrollIntoView({ behavior: 'smooth', block: 'start' })
    }
  }, [phieu.duLieu])

  // ---- Danh sách ca học sau khi lọc (mới nhất lên đầu) ----
  const tatCaCa = dsCa.duLieu?.ca ?? []

  const caDaLoc = useMemo(() => {
    const k = tuKhoa.trim().toLowerCase()

    return tatCaCa
      .filter((c) => (locTrangThai ? c.trangThai === locTrangThai : true))
      .filter((c) => (locLop ? String(c.maLopHoc) === locLop : true))
      .filter((c) => {
        if (!k) return true

        return [String(c.maCaHoc), c.maLop, c.tenLop, c.tenPhong, c.noiDungBaiHoc ?? ''].some(
          (v) => v.toLowerCase().includes(k),
        )
      })
      .sort((a, b) =>
        `${b.ngayHoc}${b.gioBatDau}`.localeCompare(`${a.ngayHoc}${a.gioBatDau}`),
      )
  }, [tatCaCa, tuKhoa, locTrangThai, locLop])

  const [trangThai, setTrangThai] = useState<Record<string, string>>({})
  const [ghiChu, setGhiChu] = useState<Record<string, string>>({})
  const [dangLuu, setDangLuu] = useState(false)

  /** Bản gốc trên server — dùng để biết còn thay đổi chưa lưu và để nút "Đặt lại". */
  const [goc, setGoc] = useState<Record<string, { trangThai: string; ghiChu: string }>>({})

  /** Học viên đang mở ô ghi chú (mặc định chỉ hiện khi trạng thái ≠ Có mặt). */
  const [moGhiChu, setMoGhiChu] = useState<Record<string, boolean>>({})

  // Nạp lại trạng thái mỗi khi mở một ca học khác
  useEffect(() => {
    const sheet = phieu.duLieu
    if (!sheet) return

    const moiTrangThai: Record<string, string> = {}
    const moiGhiChu: Record<string, string> = {}
    const moiGoc: Record<string, { trangThai: string; ghiChu: string }> = {}

    for (const hv of sheet.hocViens) {
      const tt = hv.trangThaiDiemDanh ?? TRANG_THAI_DIEM_DANH[0]
      const gc = hv.ghiChu ?? ''

      moiTrangThai[hv.maHocVien] = tt
      moiGhiChu[hv.maHocVien] = gc
      moiGoc[hv.maHocVien] = { trangThai: tt, ghiChu: gc }
    }

    setTrangThai(moiTrangThai)
    setGhiChu(moiGhiChu)
    setGoc(moiGoc)
    setMoGhiChu({})
  }, [phieu.duLieu])

  /** Đặt cùng một trạng thái cho tất cả học viên (nút "Tất cả có mặt"). */
  const datTatCa = (giaTri: string) => {
    const sheet = phieu.duLieu
    if (!sheet) return

    setTrangThai(Object.fromEntries(sheet.hocViens.map((hv) => [hv.maHocVien, giaTri])))
  }

  /** Trả mọi thay đổi về trạng thái đang lưu trên server. */
  const datLai = () => {
    setTrangThai(Object.fromEntries(Object.entries(goc).map(([k, v]) => [k, v.trangThai])))
    setGhiChu(Object.fromEntries(Object.entries(goc).map(([k, v]) => [k, v.ghiChu])))
    setMoGhiChu({})
  }

  const luu = async () => {
    const sheet = phieu.duLieu
    if (!sheet) return

    setDangLuu(true)
    try {
      await giaoVienApi.luuDiemDanh(
        sheet.maCaHoc,
        sheet.hocViens.map((hv) => ({
          maHocVien: hv.maHocVien,
          trangThaiDiemDanh: trangThai[hv.maHocVien] ?? TRANG_THAI_DIEM_DANH[0],
          ghiChu: ghiChu[hv.maHocVien]?.trim() || null,
        })),
      )
      hien('Đã lưu điểm danh.', 'thanhCong')
      void phieu.taiLai()
    } catch (error) {
      hien(thongDiepLoi(error), 'loi')
    } finally {
      setDangLuu(false)
    }
  }

  const sheet = phieu.duLieu

  const soDem = (t: string) => Object.values(trangThai).filter((v) => v === t).length

  const tongHocVien = sheet?.hocViens.length ?? 0
  const soDaCham = sheet?.hocViens.filter((hv) => hv.daDiemDanh).length ?? 0

  /** Viền trái của mỗi dòng theo trạng thái — quét nhanh ai vắng. */
  const VIEN_TRAI: Record<string, string> = {
    'Có mặt': 'border-l-emerald-500',
    'Đi trễ': 'border-l-amber-500',
    'Vắng có phép': 'border-l-sky-500',
    Vắng: 'border-l-rose-500',
  }

  /** Có thay đổi chưa lưu? (so với dữ liệu trên server) */
  const coThayDoi = useMemo(() => {
    if (!sheet) return false

    return sheet.hocViens.some((hv) => {
      const g = goc[hv.maHocVien]
      if (!g) return false

      return (
        (trangThai[hv.maHocVien] ?? '') !== g.trangThai ||
        (ghiChu[hv.maHocVien] ?? '') !== g.ghiChu
      )
    })
  }, [sheet, goc, trangThai, ghiChu])

  /** Ca chưa điểm danh lần nào → vẫn cho bấm Lưu dù chưa đổi trạng thái. */
  const chuaDiemDanhLanNao = sheet?.hocViens.some((hv) => !hv.daDiemDanh) ?? false
  const coTheLuu = coThayDoi || chuaDiemDanhLanNao

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Giảng dạy"
        title="Điểm danh"
        desc="Lọc danh sách ca học, bấm vào một dòng để mở phiếu điểm danh của ca đó."
      />

      {/* ===== BỘ LỌC ===== */}
      <section className="rounded-2xl border border-slate-200 bg-white p-3 shadow-sm">
        <div className="flex flex-wrap items-center gap-2">
          <input
            value={tuKhoa}
            onChange={(e) => setTuKhoa(e.target.value)}
            placeholder="Tìm theo mã ca, lớp, phòng, nội dung..."
            aria-label="Tìm ca học"
            className={`${LOP_INPUT} h-10 w-64`}
          />

          <select
            aria-label="Lọc theo trạng thái ca"
            value={locTrangThai}
            onChange={(e) => setLocTrangThai(e.target.value)}
            className={`${LOP_INPUT} h-10`}
          >
            <option value="">— Tất cả trạng thái —</option>
            {TRANG_THAI_CA_HOC.map((t) => (
              <option key={t} value={t}>
                {t}
              </option>
            ))}
          </select>

          <select
            aria-label="Lọc theo lớp"
            value={locLop}
            onChange={(e) => setLocLop(e.target.value)}
            className={`${LOP_INPUT} h-10`}
          >
            <option value="">— Tất cả lớp —</option>
            {(dsCa.duLieu?.lop ?? []).map((l) => (
              <option key={l.maLopHoc} value={l.maLopHoc}>
                {l.maLop} · {l.tenLop}
              </option>
            ))}
          </select>

          <button
            type="button"
            onClick={() => void dsCa.taiLai()}
            className={LOP_NUT_CHINH}
          >
            <Icon name="search" className="size-4" /> Tìm
          </button>

          <button
            type="button"
            onClick={() => {
              setTuKhoa('')
              setLocTrangThai('')
              setLocLop('')
              void dsCa.taiLai()
            }}
            className={LOP_NUT_PHU}
          >
            Làm mới
          </button>

          {maCaDangChon !== null && (
            <button
              type="button"
              onClick={() => setSearchParams({}, { replace: true })}
              className={`${LOP_NUT_PHU} ml-auto`}
            >
              <Icon name="close" className="size-4" /> Đóng phiếu điểm danh
            </button>
          )}
        </div>
      </section>

      {/* ===== DANH SÁCH CA HỌC THỰC TẾ ===== */}
      {dsCa.loi && <LoiTrang message={dsCa.loi} onThuLai={dsCa.taiLai} />}
      {dsCa.dangTai && <DangTai nhan="Đang tải ca học…" />}

      {!dsCa.dangTai && !dsCa.loi && (
        <section className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
          <div className="overflow-x-auto">
            <table className="w-full min-w-max border-collapse text-sm">
              <thead>
                <tr className="border-b border-slate-100 bg-slate-50/70 text-left">
                  {[
                    'Mã ca',
                    'Ngày học',
                    'Lớp học',
                    'Phòng học',
                    'Thời gian',
                    'Nội dung bài học',
                    'Trạng thái',
                  ].map((tieuDe) => (
                    <th
                      key={tieuDe}
                      className="whitespace-nowrap px-4 py-3 text-xs font-bold uppercase tracking-wider text-slate-500"
                    >
                      {tieuDe}
                    </th>
                  ))}
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {caDaLoc.length === 0 ? (
                  <tr>
                    <td colSpan={7} className="px-4 py-10 text-center text-sm text-slate-400">
                      Không có ca học nào khớp bộ lọc.
                    </td>
                  </tr>
                ) : (
                  caDaLoc.map((c) => {
                    const dangChon = c.maCaHoc === maCaDangChon

                    return (
                      <tr
                        key={c.maCaHoc}
                        onClick={() => moCa(c.maCaHoc)}
                        aria-selected={dangChon}
                        className={`cursor-pointer transition ${
                          dangChon ? 'bg-blue-50' : 'hover:bg-slate-50'
                        }`}
                      >
                        <td className="px-4 py-2.5 font-semibold text-slate-700">{c.maCaHoc}</td>
                        <td className="whitespace-nowrap px-4 py-2.5 text-slate-700">
                          {dinhDangNgay(c.ngayHoc)}
                        </td>
                        <td className="px-4 py-2.5">
                          <p className="font-semibold text-slate-900">{c.tenLop}</p>
                          <p className="font-mono text-[11px] text-slate-400">{c.maLop}</p>
                        </td>
                        <td className="whitespace-nowrap px-4 py-2.5 text-slate-700">
                          {c.tenPhong}
                        </td>
                        <td className="whitespace-nowrap px-4 py-2.5 text-slate-700">
                          {dinhDangKhoangGio(c.gioBatDau, c.gioKetThuc)}
                        </td>
                        <td
                          className="max-w-64 truncate px-4 py-2.5 text-slate-600"
                          title={c.noiDungBaiHoc ?? ''}
                        >
                          {c.noiDungBaiHoc ?? '—'}
                        </td>
                        <td className="whitespace-nowrap px-4 py-2.5">
                          <Nhan>{c.trangThai}</Nhan>
                        </td>
                      </tr>
                    )
                  })
                )}
              </tbody>
            </table>
          </div>

          <p className="border-t border-slate-100 px-4 py-2.5 text-xs text-slate-500">
            Hiển thị {caDaLoc.length}/{tatCaCa.length} ca học thực tế
            {maCaDangChon === null && ' · bấm vào một dòng để điểm danh'}
          </p>
        </section>
      )}

      {/* ===== PHIẾU ĐIỂM DANH ===== */}
      {maCaDangChon !== null && (
        <>
          {phieu.dangTai && <DangTai nhan="Đang tải phiếu điểm danh…" />}
          {phieu.loi && <LoiTrang message={phieu.loi} onThuLai={phieu.taiLai} />}

          {sheet && (
            <section
              ref={khungPhieu}
              className="scroll-mt-20 overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm"
            >
              {/* Header dính: luôn biết đang điểm danh ca nào + tiến độ */}
              <div className="sticky top-0 z-10 border-b border-slate-100 bg-white/95 px-4 py-3 backdrop-blur">
                <div className="flex flex-wrap items-center gap-x-3 gap-y-1.5">
                  <p className="font-bold text-slate-900">
                    {sheet.maLop} · {sheet.tenLop}
                  </p>
                  <Nhan>{sheet.trangThaiCaHoc}</Nhan>
                  <p className="text-xs text-slate-500">
                    {thuTrongTuan(sheet.ngayHoc)}, {dinhDangNgay(sheet.ngayHoc)} ·{' '}
                    {dinhDangKhoangGio(sheet.gioBatDau, sheet.gioKetThuc)} · {sheet.tenPhong}
                  </p>
                  <span className="ml-auto rounded-full bg-slate-100 px-3 py-1 text-xs font-semibold text-slate-600">
                    Đã điểm danh {soDaCham}/{tongHocVien}
                  </span>
                </div>

                {!sheet.coTheDiemDanh && (
                  <p className="mt-2 rounded-lg bg-amber-50 px-3 py-2 text-xs font-semibold text-amber-700">
                    {sheet.lyDoKhongTheDiemDanh ?? 'Ca học này không thể điểm danh.'}
                  </p>
                )}
              </div>

              {/* Thanh công cụ: đặt nhanh + đếm trạng thái + lưu */}
              {sheet.coTheDiemDanh && (
                <div className="flex flex-wrap items-center gap-2 border-b border-slate-100 bg-slate-50/60 px-4 py-2.5">
                  <button type="button" onClick={() => datTatCa('Có mặt')} className={LOP_NUT_PHU}>
                    <Icon name="check" className="size-4" /> Tất cả có mặt
                  </button>

                  <button
                    type="button"
                    onClick={datLai}
                    disabled={!coThayDoi}
                    className={`${LOP_NUT_PHU} disabled:opacity-40`}
                  >
                    Đặt lại
                  </button>

                  <div className="flex flex-wrap items-center gap-1.5 sm:ml-auto">
                    {NUT_DIEM_DANH.map((n) => (
                      <span
                        key={n.giaTri}
                        className={`rounded-full border px-2.5 py-1 text-xs font-semibold ${n.lop}`}
                      >
                        {n.giaTri}: {soDem(n.giaTri)}
                      </span>
                    ))}
                  </div>

                  {coTheLuu && (
                    <span className="text-xs font-semibold text-amber-600">● Chưa lưu</span>
                  )}

                  <button
                    type="button"
                    onClick={luu}
                    disabled={dangLuu || !coTheLuu}
                    className={`${LOP_NUT_CHINH} disabled:opacity-50`}
                  >
                    {dangLuu ? 'Đang lưu…' : 'Lưu điểm danh'}
                  </button>
                </div>
              )}

              {/* Danh sách học viên: dòng gọn, viền màu theo trạng thái */}
              <ul className="divide-y divide-slate-100">
                {sheet.hocViens.map((hv, i) => {
                  const giaTri = trangThai[hv.maHocVien] ?? TRANG_THAI_DIEM_DANH[0]
                  const ghiChuCua = ghiChu[hv.maHocVien] ?? ''
                  const hienOGhiChu =
                    giaTri !== 'Có mặt' || ghiChuCua.length > 0 || moGhiChu[hv.maHocVien]

                  return (
                    <li
                      key={hv.maHocVien}
                      className={`flex flex-wrap items-center gap-x-3 gap-y-2 border-l-4 px-4 py-2.5 ${
                        VIEN_TRAI[giaTri] ?? 'border-l-transparent'
                      }`}
                    >
                      <span className="w-5 shrink-0 text-right text-xs font-semibold text-slate-400">
                        {i + 1}
                      </span>

                      <div className="min-w-36 flex-1">
                        <p className="truncate text-sm font-semibold text-slate-900">{hv.hoTen}</p>
                        <p className="font-mono text-[11px] text-slate-400">{hv.maHocVien}</p>
                      </div>

                      <div className="flex flex-wrap items-center gap-2">
                        <label className="flex cursor-pointer items-center gap-2">
                          <input
                            type="checkbox"
                            checked={giaTri === 'Có mặt'}
                            disabled={!sheet.coTheDiemDanh}
                            onChange={(e) =>
                              setTrangThai((cu) => ({
                                ...cu,
                                [hv.maHocVien]: e.target.checked ? 'Có mặt' : 'Vắng',
                              }))
                            }
                            className="size-4 accent-blue-600"
                          />
                          <span className="text-xs font-semibold text-slate-600">Có mặt</span>
                        </label>

                        {giaTri !== 'Có mặt' && (
                          <select
                            aria-label={`Trạng thái của ${hv.hoTen}`}
                            value={giaTri}
                            disabled={!sheet.coTheDiemDanh}
                            onChange={(e) =>
                              setTrangThai((cu) => ({ ...cu, [hv.maHocVien]: e.target.value }))
                            }
                            className="h-8 rounded-lg border border-slate-200 px-2 text-xs font-semibold text-slate-700 disabled:opacity-50"
                          >
                            <option value="Đi trễ">Đi trễ</option>
                            <option value="Vắng có phép">Vắng có phép</option>
                            <option value="Vắng">Vắng</option>
                          </select>
                        )}

                        <Nhan
                          tone={
                            giaTri === 'Có mặt' ? 'emerald' : giaTri === 'Đi trễ' ? 'amber' : 'rose'
                          }
                        >
                          {giaTri}
                        </Nhan>
                      </div>

                      <div className="w-full sm:w-60">
                        {hienOGhiChu ? (
                          <input
                            value={ghiChuCua}
                            onChange={(e) =>
                              setGhiChu((cu) => ({ ...cu, [hv.maHocVien]: e.target.value }))
                            }
                            disabled={!sheet.coTheDiemDanh}
                            placeholder="Ghi chú (không bắt buộc)"
                            className="h-9 w-full rounded-lg border border-slate-200 px-3 text-sm disabled:opacity-50"
                          />
                        ) : (
                          <button
                            type="button"
                            onClick={() => setMoGhiChu((cu) => ({ ...cu, [hv.maHocVien]: true }))}
                            className="text-xs font-semibold text-slate-400 hover:text-blue-600"
                          >
                            ＋ Ghi chú
                          </button>
                        )}
                      </div>
                    </li>
                  )
                })}
              </ul>
            </section>
          )}
        </>
      )}
    </div>
  )
}

// ============================================================
// BẢNG ĐIỂM
// ============================================================
export function GVBangDiem() {
  const dsLop = dungDuLieu(() => giaoVienApi.lopCuaToi())
  const [maLopHoc, setMaLopHoc] = useState<number | null>(null)

  // Chỉ có 1 lớp được phân công → tự chọn luôn, không bắt bấm thêm bước.
  useEffect(() => {
    if (maLopHoc === null && dsLop.duLieu && dsLop.duLieu.length === 1) {
      setMaLopHoc(dsLop.duLieu[0].maLopHoc)
    }
  }, [dsLop.duLieu, maLopHoc])

  const bang = dungDuLieu(() =>
    maLopHoc ? giaoVienApi.bangDiemLop(maLopHoc) : Promise.resolve(null),
  )

  // dungDuLieu KHÔNG tự chạy lại khi tham số đổi → phải gọi tải lại thủ công.
  const taiBang = bang.taiLai

  useEffect(() => {
    void taiBang()
  }, [taiBang, maLopHoc])

  const duLieu = bang.duLieu

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Giảng dạy"
        title="Bảng điểm"
        desc="Điểm tổng kết do hệ thống tính tự động theo trọng số từng đầu điểm."
      />

      {/* Chọn lớp + tóm tắt cấu trúc điểm */}
      <section className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
        <div className="flex flex-wrap items-end gap-3">
          <div className="min-w-56 flex-1">
            <label className={LOP_NHAN} htmlFor="bd-lop">
              Lớp giảng dạy
            </label>
            <select
              id="bd-lop"
              value={maLopHoc ?? ''}
              onChange={(e) => setMaLopHoc(e.target.value ? Number(e.target.value) : null)}
              className={`${LOP_INPUT} h-10`}
            >
              <option value="">— Chọn lớp —</option>
              {(dsLop.duLieu ?? []).map((l) => (
                <option key={l.maLopHoc} value={l.maLopHoc}>
                  {l.maLop} · {l.tenLop}
                </option>
              ))}
            </select>
          </div>

          {duLieu && (
            <p className="text-xs text-slate-500">
              Tổng trọng số:{' '}
              <span
                className={`font-bold ${
                  duLieu.tongTrongSo === 100 ? 'text-emerald-600' : 'text-rose-600'
                }`}
              >
                {duLieu.tongTrongSo}%
              </span>
              {' · '}
              {duLieu.thanhPhan.length} đầu điểm
            </p>
          )}
        </div>

        {duLieu?.canhBaoTrongSo && (
          <p className="mt-3 rounded-xl bg-amber-50 p-3 text-sm font-semibold text-amber-700">
            Tổng trọng số các đầu điểm chưa bằng 100% — điểm tổng kết có thể chưa chính xác.
            Liên hệ quản trị viên để chỉnh cấu trúc điểm của khóa học.
          </p>
        )}
      </section>

      {!maLopHoc && !bang.dangTai && (
        <p className="rounded-2xl border border-dashed border-slate-300 bg-white p-10 text-center text-slate-400">
          Chọn lớp ở ô phía trên để xem bảng điểm.
        </p>
      )}

      {bang.dangTai && maLopHoc && <DangTai nhan="Đang tải bảng điểm…" />}
      {bang.loi && <LoiTrang message={bang.loi} onThuLai={bang.taiLai} />}

      {duLieu && (
        <div className="space-y-4">
          <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
            <TheChiSo nhan="Sĩ số lớp" giaTri={duLieu.soHocVien} icon="users" />
            <TheChiSo
              nhan="Đủ điều kiện"
              giaTri={`${duLieu.soDuDieuKien}/${duLieu.soHocVien}`}
              icon="check"
              tone="emerald"
            />
            <TheChiSo
              nhan="Điểm TB lớp"
              giaTri={duLieu.diemTongKetTrungBinh?.toFixed(2) ?? '—'}
              icon="chart"
              tone="amber"
            />
            <TheChiSo
              nhan="Cao nhất / Thấp nhất"
              giaTri={`${duLieu.diemCaoNhat?.toFixed(2) ?? '—'} / ${
                duLieu.diemThapNhat?.toFixed(2) ?? '—'
              }`}
              icon="chart"
            />
          </div>

          {duLieu.hocViens.length === 0 ? (
            <p className="rounded-2xl border border-dashed border-slate-300 bg-white p-8 text-center text-slate-400">
              Lớp chưa có học viên nào được xếp lớp.
            </p>
          ) : (
            <LuoiNhapDiem
              maLopHoc={duLieu.maLopHoc}
              baoCao={duLieu}
              onDaLuu={() => void bang.taiLai()}
            />
          )}

          <p className="text-xs text-slate-500">
            Gõ điểm trực tiếp vào ô · Enter hoặc ↓ để xuống học viên kế tiếp cùng đầu điểm.
            Để trống ô = không có điểm (ô đã có điểm sẽ bị xoá khi lưu). “Đủ điều kiện” = đã có
            điểm ở tất cả đầu điểm của khóa.
          </p>
        </div>
      )}
    </div>
  )
}

/**
 * LƯỚI NHẬP ĐIỂM CỦA CẢ LỚP.
 * <para>
 * Giáo viên thường chấm MỘT đầu điểm cho cả lớp, nên lưới cho gõ trực tiếp vào ô
 * (Enter/↓ xuống em kế tiếp), hiện tiến độ từng đầu điểm, tính tổng kết realtime
 * và chỉ lưu những ô đã thay đổi.
 * </para>
 */
function LuoiNhapDiem({
  maLopHoc,
  baoCao,
  onDaLuu,
}: {
  maLopHoc: number
  baoCao: AcademicReport
  onDaLuu: () => void
}) {
  const { hien } = dungToast()

  const khoaO = (maDangKy: number, maLoaiDiem: number) => `${maDangKy}:${maLoaiDiem}`

  // Điểm đã lưu của lớp (để biết ô nào PATCH, ô nào POST, ô nào DELETE)
  const daCo = dungDuLieu(() => giaoVienApi.diemCuaLop(maLopHoc))
  const taiDaCo = daCo.taiLai

  useEffect(() => {
    void taiDaCo()
  }, [taiDaCo, maLopHoc])

  const [giaTri, setGiaTri] = useState<Record<string, string>>({})
  const [goc, setGoc] = useState<Record<string, string>>({})
  const [maDiemSo, setMaDiemSo] = useState<Record<string, number>>({})
  const [dangLuu, setDangLuu] = useState(false)
  const [nhapCho, setNhapCho] = useState<{ maDangKy: number; hoTen: string } | null>(null)

  const hopRef = useRef<HTMLDivElement>(null)

  // Đổ điểm đã lưu vào lưới
  useEffect(() => {
    const ds = daCo.duLieu?.items ?? []

    const so: Record<string, number> = {}
    const raw: Record<string, string> = {}

    for (const g of ds) {
      const k = khoaO(g.maDangKy, g.maLoaiDiem)
      so[k] = g.maDiemSo
      raw[k] = String(g.diem)
    }

    setMaDiemSo(so)
    setGoc(raw)
    setGiaTri(raw)
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [daCo.duLieu])

  const tongTrongSo = baoCao.thanhPhan.reduce((s, tp) => s + tp.trongSo, 0)

  const soThayDoi = useMemo(() => {
    const tatCa = new Set([...Object.keys(goc), ...Object.keys(giaTri)])
    let dem = 0

    for (const k of tatCa) {
      if ((giaTri[k] ?? '') !== (goc[k] ?? '')) dem++
    }

    return dem
  }, [goc, giaTri])

  const soDaNhap = useMemo(() => {
    let dem = 0

    for (const hv of baoCao.hocViens) {
      for (const tp of baoCao.thanhPhan) {
        if ((giaTri[khoaO(hv.maDangKy, tp.maLoaiDiem)] ?? '').trim() !== '') dem++
      }
    }

    return dem
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [giaTri, baoCao])

  /** Ô nhập sai (không phải số, hoặc ngoài 0 → điểm tối đa). */
  const oSai = (tp: GradeComponent, raw: string) => {
    const t = raw.trim().replace(',', '.')
    if (t === '') return false

    const so = Number(t)
    return Number.isNaN(so) || so < 0 || so > tp.diemToiDa
  }

  /** Điểm tổng kết (thang 10) tính ngay khi gõ — null nếu còn thiếu đầu điểm. */
  const tongKetCua = (maDangKy: number) => {
    if (tongTrongSo <= 0) return null

    let diem = 0

    for (const tp of baoCao.thanhPhan) {
      const raw = (giaTri[khoaO(maDangKy, tp.maLoaiDiem)] ?? '').trim().replace(',', '.')
      if (raw === '') return null

      const so = Number(raw)
      if (Number.isNaN(so)) return null

      diem += (so / (tp.diemToiDa || 1)) * tp.trongSo
    }

    return (diem / tongTrongSo) * 10
  }

  /** Enter/↓ xuống học viên kế tiếp CÙNG đầu điểm. */
  const sangOTiepTheo = (dong: number, cot: number, xuong: boolean) => {
    const hop = hopRef.current
    if (!hop) return

    const o = hop.querySelector<HTMLInputElement>(
      `input[data-dong="${xuong ? dong + 1 : dong - 1}"][data-cot="${cot}"]`,
    )

    o?.focus()
    o?.select()
  }

  const luu = async () => {
    const tatCa = new Set([...Object.keys(goc), ...Object.keys(giaTri)])

    const thaoTac: Array<{ maDangKy: number; maLoaiDiem: number; raw: string }> = []

    for (const k of tatCa) {
      if ((giaTri[k] ?? '') === (goc[k] ?? '')) continue

      const [maDangKy, maLoaiDiem] = k.split(':').map(Number)
      thaoTac.push({ maDangKy, maLoaiDiem, raw: (giaTri[k] ?? '').trim() })
    }

    for (const t of thaoTac) {
      const tp = baoCao.thanhPhan.find((x) => x.maLoaiDiem === t.maLoaiDiem)

      if (tp && oSai(tp, t.raw)) {
        hien(`Điểm “${tp.tenLoaiDiem}” phải từ 0 đến ${tp.diemToiDa}.`, 'loi')
        return
      }
    }

    if (thaoTac.length === 0) {
      hien('Không có thay đổi nào để lưu.', 'thanhCong')
      return
    }

    setDangLuu(true)

    try {
      for (const t of thaoTac) {
        const idCu = maDiemSo[khoaO(t.maDangKy, t.maLoaiDiem)]

        // Xoá trắng ô ⇒ huỷ đầu điểm đó
        if (t.raw === '') {
          if (idCu) await giaoVienApi.xoaDiem(idCu)
          continue
        }

        const so = Number(t.raw.replace(',', '.'))

        if (idCu) {
          await giaoVienApi.suaDiem(idCu, { diem: so })
        } else {
          await giaoVienApi.taoDiem({
            maDangKy: t.maDangKy,
            maLoaiDiem: t.maLoaiDiem,
            diem: so,
          })
        }
      }

      hien(`Đã lưu ${thaoTac.length} thay đổi.`, 'thanhCong')

      await taiDaCo()
      onDaLuu()
    } catch (error) {
      hien(thongDiepLoi(error), 'loi')
    } finally {
      setDangLuu(false)
    }
  }

  const tongSoO = baoCao.hocViens.length * baoCao.thanhPhan.length

  return (
    <div className="space-y-3">
      {/* Thanh trạng thái + Lưu */}
      <div className="flex flex-wrap items-center gap-3 rounded-2xl border border-slate-200 bg-white p-3 shadow-sm">
        <div>
          <p className="text-sm font-bold text-slate-900">Nhập điểm cả lớp</p>
          <p className="text-xs text-slate-500">
            Đã nhập {soDaNhap}/{tongSoO} ô
            {soThayDoi > 0 ? ` · ${soThayDoi} thay đổi chưa lưu` : ' · chưa có thay đổi'}
          </p>
        </div>

        <div className="ml-auto flex items-center gap-2">
          {soThayDoi > 0 && (
            <button type="button" onClick={() => setGiaTri(goc)} className={LOP_NUT_PHU}>
              Hoàn tác
            </button>
          )}
          <button
            type="button"
            onClick={luu}
            disabled={dangLuu || soThayDoi === 0}
            className={LOP_NUT_CHINH}
          >
            {dangLuu ? 'Đang lưu…' : `Lưu thay đổi${soThayDoi > 0 ? ` (${soThayDoi})` : ''}`}
          </button>
        </div>
      </div>

      {daCo.loi && <LoiTrang message={daCo.loi} onThuLai={daCo.taiLai} />}

      <div
        ref={hopRef}
        className="overflow-x-auto rounded-2xl border border-slate-200 bg-white shadow-sm"
      >
        <table className="w-full min-w-max border-collapse text-sm">
          <thead className="sticky top-0 z-10">
            <tr className="border-b border-slate-100 bg-slate-50">
              <th className="px-5 py-3 text-left text-xs font-bold uppercase tracking-wider text-slate-500">
                Học viên
              </th>

              {baoCao.thanhPhan.map((tp) => {
                const daNhap = baoCao.hocViens.filter(
                  (hv) => (giaTri[khoaO(hv.maDangKy, tp.maLoaiDiem)] ?? '').trim() !== '',
                ).length

                return (
                  <th
                    key={tp.maLoaiDiem}
                    className="px-2 py-3 text-center text-xs font-bold uppercase text-slate-500"
                    title={`Tối đa ${tp.diemToiDa} điểm`}
                  >
                    {tp.tenLoaiDiem}
                    <span className="block font-normal normal-case text-slate-400">
                      {tp.trongSo}% · {daNhap}/{baoCao.hocViens.length}
                    </span>
                  </th>
                )
              })}

              <th className="px-3 py-3 text-right text-xs font-bold uppercase text-slate-500">
                Tổng kết
              </th>
              <th className="px-3 py-3 text-left text-xs font-bold uppercase text-slate-500">
                Kết quả
              </th>
              <th className="px-3 py-3 text-right text-xs font-bold uppercase text-slate-500">
                &nbsp;
              </th>
            </tr>
          </thead>

          <tbody className="divide-y divide-slate-100">
            {baoCao.hocViens.map((hv, chiSoDong) => {
              const tk = tongKetCua(hv.maDangKy)

              const duDauDiem = baoCao.thanhPhan.every(
                (tp) => (giaTri[khoaO(hv.maDangKy, tp.maLoaiDiem)] ?? '').trim() !== '',
              )

              return (
                <tr key={hv.maDangKy} className="hover:bg-slate-50/70">
                  <td className="px-5 py-2">
                    <p className="font-semibold text-slate-900">{hv.hoTen}</p>
                    <p className="font-mono text-xs text-slate-500">{hv.maHocVien}</p>
                  </td>

                  {baoCao.thanhPhan.map((tp, chiSoCot) => {
                    const k = khoaO(hv.maDangKy, tp.maLoaiDiem)
                    const raw = giaTri[k] ?? ''
                    const daSua = raw !== (goc[k] ?? '')
                    const sai = oSai(tp, raw)

                    return (
                      <td key={tp.maLoaiDiem} className="px-1 py-2 text-center">
                        <input
                          data-dong={chiSoDong}
                          data-cot={chiSoCot}
                          inputMode="decimal"
                          value={raw}
                          placeholder="—"
                          title={`0 – ${tp.diemToiDa}`}
                          onChange={(e) =>
                            setGiaTri((cu) => ({ ...cu, [k]: e.target.value }))
                          }
                          onKeyDown={(e) => {
                            if (e.key === 'Enter' || e.key === 'ArrowDown') {
                              e.preventDefault()
                              sangOTiepTheo(chiSoDong, chiSoCot, true)
                            } else if (e.key === 'ArrowUp') {
                              e.preventDefault()
                              sangOTiepTheo(chiSoDong, chiSoCot, false)
                            }
                          }}
                          className={`h-9 w-16 rounded-lg border bg-transparent px-2 text-center text-sm font-semibold outline-none focus:ring-2 focus:ring-blue-500 ${
                            sai
                              ? 'border-rose-400 text-rose-600'
                              : daSua
                                ? 'border-amber-400 bg-amber-50 text-amber-800'
                                : 'border-transparent text-slate-800 hover:border-slate-300'
                          }`}
                        />
                      </td>
                    )
                  })}

                  <td
                    className={`px-3 py-2 text-right font-bold ${
                      tk === null ? 'text-slate-300' : tk >= 5 ? 'text-emerald-600' : 'text-rose-600'
                    }`}
                  >
                    {tk === null ? '—' : tk.toFixed(2)}
                  </td>

                  <td className="px-3 py-2">
                    {duDauDiem ? (
                      <span className="rounded-full border border-emerald-200 bg-emerald-50 px-2.5 py-1 text-xs font-semibold text-emerald-700">
                        Đủ điều kiện
                      </span>
                    ) : (
                      <span className="rounded-full border border-amber-200 bg-amber-50 px-2.5 py-1 text-xs font-semibold text-amber-700">
                        Thiếu điểm
                      </span>
                    )}
                  </td>

                  <td className="px-3 py-2 text-right">
                    <button
                      type="button"
                      onClick={() => setNhapCho({ maDangKy: hv.maDangKy, hoTen: hv.hoTen })}
                      className={LOP_NUT_PHU}
                      title="Nhập nhận xét cho từng đầu điểm"
                    >
                      Nhận xét
                    </button>
                  </td>
                </tr>
              )
            })}
          </tbody>
        </table>
      </div>

      {nhapCho && (
        <NhapDiemModal
          maDangKy={nhapCho.maDangKy}
          hoTen={nhapCho.hoTen}
          thanhPhan={baoCao.thanhPhan}
          onDong={() => setNhapCho(null)}
          onXong={() => {
            setNhapCho(null)
            void taiDaCo()
            onDaLuu()
          }}
        />
      )}
    </div>
  )
}

/**
 * Popup nhập điểm cho MỘT học viên.
 * Mỗi đầu điểm là một dòng: nhập điểm (0 → điểm tối đa) + nhận xét.
 * Để trống = không có điểm (nếu đã có thì xoá bản ghi điểm đó).
 */
function NhapDiemModal({
  maDangKy,
  hoTen,
  thanhPhan,
  onDong,
  onXong,
}: {
  maDangKy: number
  hoTen: string
  thanhPhan: GradeComponent[]
  onDong: () => void
  onXong: () => void
}) {
  const { hien } = dungToast()

  const hienCo = dungDuLieu(() => giaoVienApi.diemCuaDangKy(maDangKy))

  const [diem, setDiem] = useState<Record<number, string>>({})
  const [nhanXet, setNhanXet] = useState<Record<number, string>>({})
  const [dangLuu, setDangLuu] = useState(false)

  // Đổ điểm đã lưu vào ô nhập
  useEffect(() => {
    const ds = hienCo.duLieu?.items ?? []

    const d: Record<number, string> = {}
    const n: Record<number, string> = {}

    for (const g of ds) {
      d[g.maLoaiDiem] = String(g.diem)
      n[g.maLoaiDiem] = g.nhanXet ?? ''
    }

    setDiem(d)
    setNhanXet(n)
  }, [hienCo.duLieu])

  const luu = async () => {
    const daLuu = new Map((hienCo.duLieu?.items ?? []).map((g) => [g.maLoaiDiem, g]))

    // Kiểm tra trước khi gọi API
    const daChuanBi: Array<{ maLoaiDiem: number; diem: number; nhanXet: string | null }> = []

    for (const tp of thanhPhan) {
      const raw = (diem[tp.maLoaiDiem] ?? '').trim().replace(',', '.')
      if (raw === '') continue

      const so = Number(raw)

      if (Number.isNaN(so) || so < 0 || so > tp.diemToiDa) {
        hien(`Điểm “${tp.tenLoaiDiem}” phải từ 0 đến ${tp.diemToiDa}.`, 'loi')
        return
      }

      daChuanBi.push({
        maLoaiDiem: tp.maLoaiDiem,
        diem: so,
        nhanXet: nhanXet[tp.maLoaiDiem]?.trim() || null,
      })
    }

    setDangLuu(true)

    try {
      // 1) Điểm đã có → cập nhật
      for (const d of daChuanBi) {
        const cu = daLuu.get(d.maLoaiDiem)

        if (cu) {
          await giaoVienApi.suaDiem(cu.maDiemSo, { diem: d.diem, nhanXet: d.nhanXet })
        } else {
          await giaoVienApi.taoDiem({
            maDangKy,
            maLoaiDiem: d.maLoaiDiem,
            diem: d.diem,
            nhanXet: d.nhanXet,
          })
        }
      }

      // 2) Ô đã xoá trắng nhưng trước đó có điểm → xoá bản ghi
      const giuLai = new Set(daChuanBi.map((d) => d.maLoaiDiem))

      for (const [maLoaiDiem, cu] of daLuu) {
        if (!giuLai.has(maLoaiDiem)) {
          await giaoVienApi.xoaDiem(cu.maDiemSo)
        }
      }

      hien('Đã lưu điểm.', 'thanhCong')
      onXong()
    } catch (error) {
      hien(thongDiepLoi(error), 'loi')
    } finally {
      setDangLuu(false)
    }
  }

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/50 p-4 backdrop-blur-sm">
      <div className="w-full max-w-3xl overflow-hidden rounded-2xl bg-white shadow-2xl">
        <div className="flex items-center justify-between border-b border-slate-100 p-5">
          <div>
            <h2 className="text-lg font-bold text-slate-900">Nhập điểm — {hoTen}</h2>
            <p className="text-sm text-slate-500">
              Để trống ô điểm = không có điểm của đầu điểm đó.
            </p>
          </div>
          <button
            type="button"
            onClick={onDong}
            className="grid size-9 place-items-center rounded-full text-slate-400 hover:bg-slate-100"
          >
            <Icon name="close" className="size-5" />
          </button>
        </div>

        <div className="max-h-[60vh] overflow-y-auto p-5">
          {hienCo.loi ? (
            <LoiTrang message={hienCo.loi} onThuLai={hienCo.taiLai} />
          ) : hienCo.dangTai ? (
            <DangTai nhan="Đang tải điểm đã lưu…" />
          ) : (
            <table className="w-full border-collapse text-sm">
              <thead>
                <tr className="border-b border-slate-100 bg-slate-50/60 text-left">
                  <th className="px-3 py-2 text-xs font-bold uppercase text-slate-500">
                    Đầu điểm
                  </th>
                  <th className="px-3 py-2 text-center text-xs font-bold uppercase text-slate-500">
                    Trọng số
                  </th>
                  <th className="px-3 py-2 text-center text-xs font-bold uppercase text-slate-500">
                    Tối đa
                  </th>
                  <th className="px-3 py-2 text-center text-xs font-bold uppercase text-slate-500">
                    Điểm
                  </th>
                  <th className="px-3 py-2 text-left text-xs font-bold uppercase text-slate-500">
                    Nhận xét
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {thanhPhan.map((tp) => (
                  <tr key={tp.maLoaiDiem}>
                    <td className="px-3 py-2 font-semibold text-slate-800">{tp.tenLoaiDiem}</td>
                    <td className="px-3 py-2 text-center text-slate-500">{tp.trongSo}%</td>
                    <td className="px-3 py-2 text-center text-slate-500">{tp.diemToiDa}</td>
                    <td className="px-3 py-2 text-center">
                      <input
                        inputMode="decimal"
                        value={diem[tp.maLoaiDiem] ?? ''}
                        onChange={(e) =>
                          setDiem((cu) => ({ ...cu, [tp.maLoaiDiem]: e.target.value }))
                        }
                        onFocus={(e) => e.target.select()}
                        onKeyDown={(e) => {
                          if (e.key !== 'Enter') return
                          e.preventDefault()
                          // Enter → sang ô nhận xét cùng dòng
                          const o = e.currentTarget
                            .closest('tr')
                            ?.querySelector<HTMLInputElement>('input[data-nhanxet]')
                          o?.focus()
                        }}
                        placeholder={`0 – ${tp.diemToiDa}`}
                        title={`Điểm từ 0 đến ${tp.diemToiDa}`}
                        className="h-9 w-24 rounded-lg border border-slate-200 px-2 text-center text-sm outline-none focus:ring-2 focus:ring-blue-500"
                      />
                    </td>
                    <td className="px-3 py-2">
                      <input
                        data-nhanxet
                        value={nhanXet[tp.maLoaiDiem] ?? ''}
                        onChange={(e) =>
                          setNhanXet((cu) => ({ ...cu, [tp.maLoaiDiem]: e.target.value }))
                        }
                        placeholder="Không bắt buộc"
                        className="h-9 w-full rounded-lg border border-slate-200 px-3 text-sm outline-none focus:ring-2 focus:ring-blue-500"
                      />
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>

        <div className="flex items-center justify-end gap-2 border-t border-slate-100 p-4">
          <button type="button" onClick={onDong} className={LOP_NUT_PHU}>
            Đóng
          </button>
          <button
            type="button"
            onClick={luu}
            disabled={dangLuu || hienCo.dangTai}
            className={LOP_NUT_CHINH}
          >
            {dangLuu ? 'Đang lưu…' : 'Lưu điểm'}
          </button>
        </div>
      </div>
    </div>
  )
}

// ============================================================
// THÔNG BÁO
// ============================================================
const PAGE_SIZE = 20

export function GVThongBao() {
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
        desc="Thông báo gửi tới bạn hoặc tới lớp bạn đang phụ trách."
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
                <p className="mt-3 text-xs text-slate-400">
                  {t.phamVi === 'Lớp' ? `Lớp ${t.maLop ?? ''}` : 'Cá nhân'} · Gửi bởi{' '}
                  {t.tenDangNhapNguoiGui}
                </p>
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
