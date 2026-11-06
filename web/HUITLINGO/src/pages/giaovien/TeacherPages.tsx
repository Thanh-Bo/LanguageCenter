// Khu vực giáo viên / trợ giảng.
import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { Link, useNavigate, useParams, useSearchParams } from 'react-router'
import { Icon } from '../../components/BieuTuong'
import {
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
} from '../../components/ui'
import { dungToast } from '../../components/ui/toast'
import { ThoiKhoaBieuTuan, dungLichTuan } from '../../components/ThoiKhoaBieuTuan'
import { dungXacThuc } from '../../contexts/NguCanhXacThuc'
import { giaoVienApi, thongBaoApi } from '../../lib/api/endpoints'
import type {
  AcademicReport,
  AttendanceSheet,
  ClassProgress,
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

/**
 * Ô nhập GỌN dùng cho thanh lọc.
 * KHÔNG dùng `LOP_INPUT` vì lớp đó có `w-full` + `h-11` sẽ đè mất kích thước riêng
 * (Tailwind ưu tiên theo thứ tự trong CSS sinh ra, không theo thứ tự class viết ra).
 */
const LOP_O_LOC =
  'h-9 rounded-lg border border-slate-200 bg-white px-3 text-sm text-slate-900 outline-none transition focus:border-blue-500 focus:ring-4 focus:ring-blue-50'

/** Nút GỌN cùng chiều cao với ô lọc. */
const LOP_NUT_LOC =
  'inline-flex h-9 shrink-0 items-center gap-1.5 rounded-lg border border-slate-200 bg-white px-3 text-sm font-semibold text-slate-700 transition hover:border-blue-200 hover:text-blue-600'

// ============================================================
// LỚP CỦA TÔI
// ============================================================
/** Thứ tự ưu tiên khi sắp xếp danh sách lớp. */
const THU_TU_TRANG_THAI: Record<string, number> = {
  'Đang học': 0,
  'Chuẩn bị mở': 1,
  'Đã kết thúc': 2,
  'Đã hủy': 3,
}

export function GVLopCuaToi() {
  const { duLieu, dangTai, loi, taiLai } = dungDuLieu(async () => {
    const lop = await giaoVienApi.lopCuaToi()
    const tienDo: Record<number, ClassProgress> = {}

    await Promise.all(
      lop.map(async (l) => {
        try {
          tienDo[l.maLopHoc] = await giaoVienApi.tienDoLop(l.maLopHoc)
        } catch {
          // Lớp chưa sinh buổi / không có quyền → bỏ qua thanh tiến độ.
        }
      }),
    )

    return { lop, tienDo }
  })

  if (dangTai) return <DangTai />
  if (loi) return <LoiTrang message={loi} onThuLai={taiLai} />

  const ds: TeacherClass[] = [...(duLieu?.lop ?? [])].sort(
    (a, b) => (THU_TU_TRANG_THAI[a.trangThai] ?? 9) - (THU_TU_TRANG_THAI[b.trangThai] ?? 9),
  )
  const tienDo = duLieu?.tienDo ?? {}

  // 1 lớp -> 1 cột (full), 2 lớp -> 2 cột, từ 3 lớp -> 3 cột
  const lopCot = ds.length <= 1 ? 'grid-cols-1' : ds.length === 2 ? 'md:grid-cols-2' : 'md:grid-cols-2 xl:grid-cols-3'

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Giảng dạy"
        title="Lớp của tôi"
        desc="Bấm vào một lớp để xem tiến độ 20 buổi, danh sách buổi và điểm của học viên."
      />

      {ds.length === 0 ? (
        <p className="rounded-2xl border border-dashed border-slate-300 bg-white p-10 text-center text-slate-400">
          Bạn chưa được phân công lớp nào.
        </p>
      ) : (
        <div className={`grid gap-4 ${lopCot}`}>
          {ds.map((l) => {
            const td = tienDo[l.maLopHoc]
            const phanTram = Math.max(0, Math.min(100, Number(td?.tienDoPhanTram) || 0))
            const homNay = homNayIso()
            const keTiep =
              (td?.buois ?? []).find(
                (b) => b.ngayHoc >= homNay && b.trangThai !== 'Đã hủy',
              ) ?? null

            return (
              <Link
                key={l.maLopHoc}
                to={`/giaovien/lop-cua-toi/${encodeURIComponent(l.maLop)}`}
                className="group flex flex-col rounded-2xl border border-slate-200 bg-white p-5 shadow-sm transition hover:border-blue-300 hover:shadow-md"
              >
                <div className="flex items-start justify-between gap-3">
                  <p className="font-mono text-xs font-semibold text-slate-500">
                    {l.maLop}
                  </p>
                  <span className="shrink-0">
                    <Nhan>{l.trangThai}</Nhan>
                  </span>
                </div>

                <h3 className="mt-1 text-lg font-bold text-slate-900 transition group-hover:text-blue-700">
                  {l.tenLop}
                </h3>

                <p className="mt-1 flex flex-wrap items-center gap-x-1.5 text-sm text-slate-500">
                  <span>{l.tenKhoaHoc}</span>
                  <span className="text-slate-300">·</span>
                  <span>
                    {l.siSo}/{l.siSoToiDa} HV
                  </span>
                  <span className="text-slate-300">·</span>
                  <span>{l.vaiTroPhanCong}</span>
                </p>

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
                      <span className="font-semibold text-slate-600">
                        Buổi kế tiếp:
                      </span>{' '}
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
// LỊCH DẠY
// ============================================================
export function GVLichDay() {
  const lich = dungLichTuan((q) => giaoVienApi.lichDay(q))
  const navigate = useNavigate()
  const { nguoiDung } = dungXacThuc()

  // Điểm danh chỉ dành cho Trợ giảng -> Giáo viên bấm ca học không mở phiếu điểm danh.
  const laTroGiang = nguoiDung?.vaiTro === 'Trợ giảng'

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Giảng dạy"
        title="Lịch dạy"
        desc={
          laTroGiang
            ? 'Thời khóa biểu theo tuần của các lớp bạn phụ trách. Bấm vào một ca để điểm danh.'
            : 'Thời khóa biểu theo tuần của các ca bạn trực tiếp đứng lớp.'
        }
      />

      <ThoiKhoaBieuTuan
        duLieu={lich.duLieu}
        tuanBatDau={lich.tuanBatDau}
        tuanKetThuc={lich.tuanKetThuc}
        onDoiTuan={lich.doiTuan}
        dangTai={lich.dangTai}
        loi={lich.loi}
        onThuLai={lich.taiLai}
        dongPhu={(ca) => [
          ca.tenPhong,
          ca.soBuoi ? `Buổi ${ca.soBuoi}` : null,
          ca.tenKhoaHoc,
        ]}
        moTaRong="Tuần này bạn không có ca dạy nào."
        nhanDem="ca dạy"
        onChonCaHoc={
          laTroGiang
            ? (ca) =>
                navigate(`/giaovien/diem-danh?ca=${ca.maCaHoc}&ngay=${ca.ngayHoc}`)
            : undefined
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

  // ---- Danh sách ca học sau khi lọc (mới nhất lên đầu) ----
  const tatCaCa = dsCa.duLieu?.ca ?? []

  const caDaLoc = useMemo(() => {
    const k = tuKhoa.trim().toLowerCase()

    return tatCaCa
      .filter((c) => (locTrangThai ? c.trangThai === locTrangThai : true))
      .filter((c) => (locLop ? String(c.maLopHoc) === locLop : true))
      .filter((c) => {
        if (!k) return true

        return [
          c.soBuoi ? String(c.soBuoi) : '',
          String(c.maCaHoc),
          c.maLop,
          c.tenLop,
          c.tenPhong,
          c.noiDungBaiHoc ?? '',
        ].some((v) => v.toLowerCase().includes(k))
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

  /** Có ?ca= → mở RIÊNG trang điểm danh, không hiện danh sách ca học phía trên. */
  const dangMoPhieu = maCaDangChon !== null

  /** Quay về danh sách ca học. */
  const dongPhieu = () => setSearchParams({}, { replace: true })

  return (
    <div className="space-y-6">
      {!dangMoPhieu && (
        <>
          <TrangTieuDe
            eyebrow="Giảng dạy"
            title="Điểm danh"
            desc="Lọc danh sách ca học, bấm vào một dòng để mở phiếu điểm danh của ca đó."
          />

      {/* ===== BỘ LỌC (1 hàng trên desktop, tự xuống dòng trên mobile) ===== */}
      <section className="rounded-2xl border border-slate-200 bg-white px-3 py-2.5 shadow-sm">
        <div className="flex flex-wrap items-center gap-2">
          <div className="relative min-w-56 flex-1">
            <Icon
              name="search"
              className="pointer-events-none absolute left-3 top-1/2 size-4 -translate-y-1/2 text-slate-400"
            />
            <input
              value={tuKhoa}
              onChange={(e) => setTuKhoa(e.target.value)}
              placeholder="Tìm theo buổi, lớp, phòng, nội dung…"
              aria-label="Tìm ca học"
              className={`${LOP_O_LOC} w-full pl-9`}
            />
          </div>

          <select
            aria-label="Lọc theo trạng thái ca"
            value={locTrangThai}
            onChange={(e) => setLocTrangThai(e.target.value)}
            className={`${LOP_O_LOC} w-40`}
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
            className={`${LOP_O_LOC} w-56`}
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
            onClick={() => {
              setTuKhoa('')
              setLocTrangThai('')
              setLocLop('')
              void dsCa.taiLai()
            }}
            className={LOP_NUT_LOC}
            title="Xoá bộ lọc và tải lại danh sách"
          >
            Làm mới
          </button>
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
                    'Buổi',
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
                        <td className="whitespace-nowrap px-4 py-2.5">
                          <div className="flex flex-col items-start gap-1">
                            <span
                              title={`Mã ca #${c.maCaHoc}`}
                              className="font-semibold text-slate-700"
                            >
                              {c.soBuoi ? `Buổi ${c.soBuoi}` : '—'}
                            </span>

                            {c.loaiBuoi === 'Kiểm tra' && (
                              <span className="rounded-full bg-amber-100 px-2 py-0.5 text-[10px] font-bold text-amber-700">
                                Kiểm tra
                              </span>
                            )}

                            {c.loaiBuoi === 'Buổi thi' && (
                              <span className="rounded-full bg-violet-100 px-2 py-0.5 text-[10px] font-bold text-violet-700">
                                Buổi thi
                              </span>
                            )}
                          </div>
                        </td>
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
                        <td className="px-4 py-2.5 text-slate-600" title={c.noiDungBaiHoc ?? ''}>
                          <p className="line-clamp-2 max-w-72">{c.noiDungBaiHoc ?? '—'}</p>
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
            Hiển thị {caDaLoc.length}/{tatCaCa.length} ca học thực tế · bấm vào một dòng để mở
            phiếu điểm danh
          </p>
        </section>
      )}
        </>
      )}

      {/* ===== TRANG ĐIỂM DANH (mở riêng khi URL có ?ca=) ===== */}
      {dangMoPhieu && (
        <>
          <div>
            <button
              type="button"
              onClick={dongPhieu}
              className="inline-flex items-center gap-1 text-xs font-bold tracking-wider text-slate-500 uppercase transition hover:text-blue-600"
            >
              <Icon name="chevronLeft" className="size-4" />
              Danh sách ca học
            </button>

            <h1 className="mt-1 text-2xl font-bold text-slate-900">Điểm danh ca học</h1>
          </div>

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
                  {sheet.soBuoi ? (
                    <span className="rounded-full bg-slate-100 px-2 py-0.5 text-xs font-semibold text-slate-600">
                      Buổi {sheet.soBuoi}
                    </span>
                  ) : null}
                  {sheet.loaiBuoi === 'Buổi thi' && (
                    <span className="rounded-full bg-violet-100 px-2 py-0.5 text-xs font-bold text-violet-700">
                      Buổi thi
                    </span>
                  )}
                  {sheet.loaiBuoi === 'Kiểm tra' && (
                    <span className="rounded-full bg-amber-100 px-2 py-0.5 text-xs font-bold text-amber-700">
                      Kiểm tra
                    </span>
                  )}
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
/** Màu thanh trọng số + chấm chú giải cho từng đầu điểm (xoay vòng nếu nhiều hơn 5). */
const MAU_DAU_DIEM = [
  'bg-blue-500',
  'bg-emerald-500',
  'bg-amber-500',
  'bg-violet-500',
  'bg-rose-500',
] as const

const mauDauDiem = (i: number) => MAU_DAU_DIEM[i % MAU_DAU_DIEM.length]

export function GVBangDiem() {
  const { maLop: thamSo } = useParams()
  const dsLop = dungDuLieu(() => giaoVienApi.lopCuaToi())

  // Trợ giảng CHỈ ĐỌC bảng điểm (nhập điểm là việc của Giáo viên).
  const { nguoiDung } = dungXacThuc()
  const chiXem = nguoiDung?.vaiTro === 'Trợ giảng'

  const ds = dsLop.duLieu

  /**
   * Tham số URL chấp nhận 3 kiểu cho tiện chia sẻ link:
   * `IELTS-A-01` (mã lớp) · `1` (mã lớp học) · `14` (mã khóa học).
   */
  const lopDangChon = useMemo(() => {
    if (!ds) return null

    const t = (thamSo ?? '').trim().toLowerCase()
    if (t === '') return null

    return (
      ds.find((l) => l.maLop.toLowerCase() === t) ??
      ds.find((l) => String(l.maLopHoc) === t) ??
      ds.find((l) => String(l.maKhoaHoc) === t) ??
      null
    )
  }, [ds, thamSo])

  const maLopHoc = lopDangChon?.maLopHoc ?? null

  /** Chưa có mã lớp trên URL (hoặc mã không hợp lệ) → hiện danh sách lớp để chọn. */
  const dangXemDanhSach = !!ds && !lopDangChon

  const dsSapXep = useMemo(
    () =>
      [...(ds ?? [])].sort(
        (a, b) => (THU_TU_TRANG_THAI[a.trangThai] ?? 9) - (THU_TU_TRANG_THAI[b.trangThai] ?? 9),
      ),
    [ds],
  )

  const bang = dungDuLieu(() =>
    maLopHoc ? giaoVienApi.bangDiemLop(maLopHoc) : Promise.resolve(null),
  )

  // dungDuLieu KHÔNG tự chạy lại khi tham số đổi → phải gọi tải lại thủ công.
  const taiBang = bang.taiLai

  useEffect(() => {
    void taiBang()
  }, [taiBang, maLopHoc])

  const duLieu = bang.duLieu

  // ===== BƯỚC 1: DANH SÁCH LỚP =====
  if (dangXemDanhSach) {
    const lopCot =
      dsSapXep.length <= 1
        ? 'grid-cols-1'
        : dsSapXep.length === 2
          ? 'md:grid-cols-2'
          : 'md:grid-cols-2 xl:grid-cols-3'

    return (
      <div className="space-y-6">
        <TrangTieuDe
          eyebrow="Giảng dạy"
          title="Bảng điểm"
          desc={
            chiXem
              ? 'Chọn một lớp để xem bảng điểm. Điểm tổng kết do hệ thống tự tính theo trọng số từng đầu điểm.'
              : 'Chọn một lớp để xem và nhập điểm. Điểm tổng kết do hệ thống tự tính theo trọng số từng đầu điểm.'
          }
        />

        {dsLop.loi && <LoiTrang message={dsLop.loi} onThuLai={dsLop.taiLai} />}

        {dsSapXep.length === 0 ? (
          <p className="rounded-2xl border border-dashed border-slate-300 bg-white p-10 text-center text-slate-400">
            Bạn chưa được phân công lớp nào.
          </p>
        ) : (
          <div className={`grid gap-4 ${lopCot}`}>
            {dsSapXep.map((l) => (
              <Link
                key={l.maLopHoc}
                to={`/giaovien/bang-diem/${encodeURIComponent(l.maLop)}`}
                className="group flex flex-col rounded-2xl border border-slate-200 bg-white p-5 shadow-sm transition hover:border-blue-300 hover:shadow-md"
              >
                <div className="flex items-start justify-between gap-3">
                  <span className="inline-flex items-center gap-1.5 rounded-lg bg-slate-100 px-2 py-1 font-mono text-xs font-bold text-slate-600">
                    <Icon name="chart" className="size-3.5" />
                    {l.maLop}
                  </span>
                  <span className="shrink-0">
                    <Nhan>{l.trangThai}</Nhan>
                  </span>
                </div>

                <h3 className="mt-2 text-lg font-bold text-slate-900 transition group-hover:text-blue-700">
                  {l.tenLop}
                </h3>

                <p className="mt-1 flex flex-wrap items-center gap-x-1.5 text-sm text-slate-500">
                  <span>{l.tenKhoaHoc}</span>
                  <span className="text-slate-300">·</span>
                  <span>
                    {l.siSo}/{l.siSoToiDa} HV
                  </span>
                  <span className="text-slate-300">·</span>
                  <span>{l.vaiTroPhanCong}</span>
                </p>

                <p className="mt-3 text-xs text-slate-500">
                  {dinhDangNgay(l.ngayBatDau)} → {dinhDangNgay(l.ngayKetThucDuKien)}
                </p>

                <p className="mt-4 inline-flex items-center gap-1 text-sm font-semibold text-blue-600 transition group-hover:gap-2">
                  Xem bảng điểm
                  <Icon name="arrowRight" className="size-4" />
                </p>
              </Link>
            ))}
          </div>
        )}
      </div>
    )
  }

  // ===== BƯỚC 2: BẢNG ĐIỂM CỦA LỚP ĐÃ CHỌN =====
  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Giảng dạy"
        title="Bảng điểm"
        desc="Điểm tổng kết do hệ thống tự tính theo trọng số từng đầu điểm."
      />

      {dsLop.loi && <LoiTrang message={dsLop.loi} onThuLai={dsLop.taiLai} />}

      {!ds && <DangTai nhan="Đang tải lớp giảng dạy…" />}

      {bang.dangTai && maLopHoc !== null && <DangTai nhan="Đang tải bảng điểm…" />}
      {bang.loi && <LoiTrang message={bang.loi} onThuLai={bang.taiLai} />}

      {/* Đang đổi lớp → dữ liệu lớp cũ chưa khớp mã lớp mới thì chưa vẽ bảng. */}
      {duLieu && lopDangChon && duLieu.maLopHoc === maLopHoc && (
        <div className="space-y-4">
          {/* ===== THẺ THÔNG TIN LỚP + TRỌNG SỐ ===== */}
          <section className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
            <div className="flex flex-wrap items-start justify-between gap-4 border-b border-slate-100 p-5">
              <div className="min-w-0">
                <Link
                  to="/giaovien/bang-diem"
                  className="inline-flex items-center gap-1 rounded-lg border border-slate-200 bg-white px-2.5 py-1.5 text-[11px] font-bold tracking-wider text-slate-500 uppercase transition hover:border-blue-400 hover:text-blue-600"
                >
                  <Icon name="chevronLeft" className="size-3.5" />
                  Danh sách lớp
                </Link>

                <div className="mt-3 flex flex-wrap items-center gap-2">
                  <span className="rounded-md bg-slate-100 px-2 py-0.5 font-mono text-xs font-bold text-slate-700">
                    {lopDangChon.maLop}
                  </span>
                  <Nhan>{lopDangChon.trangThai}</Nhan>
                </div>

                <h2 className="mt-2 text-xl font-bold text-slate-900">{lopDangChon.tenLop}</h2>

                <p className="mt-0.5 flex flex-wrap items-center gap-x-1.5 text-sm text-slate-500">
                  <span>{lopDangChon.tenKhoaHoc}</span>
                  <span className="text-slate-300">·</span>
                  <span>{dinhDangNgay(lopDangChon.ngayBatDau)}</span>
                  <span className="text-slate-300">→</span>
                  <span>{dinhDangNgay(lopDangChon.ngayKetThucDuKien)}</span>
                </p>
              </div>

              <div className="rounded-xl border border-slate-200 bg-slate-50 px-4 py-3 text-right">
                <p className="text-[11px] font-bold tracking-wider text-slate-400 uppercase">
                  Tổng trọng số
                </p>
                <p
                  className={`text-3xl leading-tight font-bold ${
                    duLieu.tongTrongSo === 100 ? 'text-emerald-600' : 'text-amber-600'
                  }`}
                >
                  {duLieu.tongTrongSo}%
                </p>
                <p className="text-xs text-slate-400">{duLieu.thanhPhan.length} đầu điểm</p>
              </div>
            </div>

            <div className="p-5">
              <div className="flex h-2.5 w-full overflow-hidden rounded-full bg-slate-100">
                {duLieu.thanhPhan.map((tp, i) => (
                  <span
                    key={tp.maLoaiDiem}
                    title={`${tp.tenLoaiDiem} · ${tp.trongSo}% · tối đa ${tp.diemToiDa} điểm`}
                    style={{
                      width: `${duLieu.tongTrongSo > 0 ? (tp.trongSo / duLieu.tongTrongSo) * 100 : 0}%`,
                    }}
                    className={mauDauDiem(i)}
                  />
                ))}
              </div>

              <div className="mt-3 flex flex-wrap gap-x-5 gap-y-1.5">
                {duLieu.thanhPhan.map((tp, i) => (
                  <span
                    key={tp.maLoaiDiem}
                    className="inline-flex items-center gap-1.5 text-xs text-slate-600"
                  >
                    <span className={`size-2.5 rounded-full ${mauDauDiem(i)}`} />
                    <span className="font-semibold">{tp.tenLoaiDiem}</span>
                    <span className="text-slate-400">
                      {tp.trongSo}% · tối đa {tp.diemToiDa}
                    </span>
                  </span>
                ))}
              </div>

              {duLieu.canhBaoTrongSo && (
                <p className="mt-4 flex items-start gap-2 rounded-xl border border-amber-200 bg-amber-50 p-3 text-sm font-semibold text-amber-800">
                  <Icon name="alert" className="mt-0.5 size-4 shrink-0" />
                  Tổng trọng số các đầu điểm chưa bằng 100% — điểm tổng kết có thể chưa chính
                  xác. Liên hệ quản trị viên để chỉnh cấu trúc điểm của khóa học.
                </p>
              )}
            </div>
          </section>

          <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
            <TheChiSo
              nhan="Sĩ số lớp"
              giaTri={duLieu.soHocVien}
              moTa={`Đang theo học ${lopDangChon.tenKhoaHoc}`}
            />
            <TheChiSo
              nhan="Đủ điều kiện"
              giaTri={`${duLieu.soDuDieuKien}/${duLieu.soHocVien}`}
              moTa={
                duLieu.soThieuDiem > 0
                  ? `Còn ${duLieu.soThieuDiem} học viên thiếu điểm`
                  : 'Tất cả học viên đã đủ điểm'
              }
            />
            <TheChiSo
              nhan="Điểm TB lớp"
              giaTri={duLieu.diemTongKetTrungBinh?.toFixed(2) ?? '—'}
              moTa="Thang điểm 10"
            />
            <TheChiSo
              nhan="Cao nhất / Thấp nhất"
              giaTri={`${duLieu.diemCaoNhat?.toFixed(2) ?? '—'} / ${
                duLieu.diemThapNhat?.toFixed(2) ?? '—'
              }`}
              moTa="Biên độ điểm tổng kết"
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
              chiXem={chiXem}
              onDaLuu={() => void bang.taiLai()}
            />
          )}

          <div className="flex flex-wrap items-start gap-x-6 gap-y-2 rounded-2xl border border-slate-200 bg-white p-4 text-xs text-slate-500 shadow-sm">
            {chiXem ? (
              <>
                <span className="inline-flex items-center gap-1.5">
                  <Icon name="lock" className="size-3.5 text-slate-400" />
                  Bạn đang xem ở chế độ <b className="text-slate-600">chỉ đọc</b> — Trợ giảng không nhập điểm
                </span>
                <span className="inline-flex items-center gap-1.5">
                  <Icon name="badgeCheck" className="size-3.5 text-violet-500" />
                  “Đủ điều kiện” = đã có điểm ở tất cả đầu điểm
                </span>
                <span className="inline-flex items-center gap-1.5">
                  Đầu điểm có ngày thi chưa tới sẽ bị khoá
                </span>
              </>
            ) : (
              <>
                <span className="inline-flex items-center gap-1.5">
                  <Icon name="check" className="size-3.5 text-emerald-500" />
                  Gõ điểm trực tiếp vào ô
                </span>
                <span className="inline-flex items-center gap-1.5">
                  <Icon name="arrowDown" className="size-3.5 text-blue-500" />
                  Enter / ↓ xuống học viên kế tiếp, ↑ lên trên
                </span>
                <span className="inline-flex items-center gap-1.5">
                  <Icon name="close" className="size-3.5 text-rose-500" />
                  Để trống ô = xoá điểm của đầu điểm đó khi lưu
                </span>
                <span className="inline-flex items-center gap-1.5">
                  <Icon name="badgeCheck" className="size-3.5 text-violet-500" />
                  “Đủ điều kiện” = đã có điểm ở tất cả đầu điểm
                </span>
                <span className="inline-flex items-center gap-1.5">
                  Đầu điểm có ngày thi chưa tới sẽ bị khoá
                </span>
              </>
            )}
          </div>
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
  chiXem = false,
}: {
  maLopHoc: number
  baoCao: AcademicReport
  onDaLuu: () => void
  /** Trợ giảng: chỉ xem, không nhập/sửa điểm. */
  chiXem?: boolean
}) {
  const { hien } = dungToast()

  /**
   * Mỗi đầu điểm giờ là một bài riêng (Giữa kỳ 1 / Giữa kỳ 2 …) nên luôn nhập ở Lần 1.
   * Không còn chọn “lần kiểm tra” / “buổi thi” trên giao diện.
   */
  const lan = 1

  const khoaO = (maDangKy: number, maLoaiDiem: number) => `${maDangKy}:${maLoaiDiem}:${lan}`

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

  // Đổ điểm đã lưu vào lưới (chỉ lấy đúng lần kiểm tra đang chọn)
  useEffect(() => {
    const ds = (daCo.duLieu?.items ?? []).filter((g) => (g.lanKiemTra ?? 1) === lan)

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
  }, [daCo.duLieu, lan])

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
            lanKiemTra: lan,
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

  /** Trung bình của một đầu điểm (chỉ tính các ô đã nhập). */
  const trungBinhCot = (maLoaiDiem: number) => {
    const so: number[] = []

    for (const hv of baoCao.hocViens) {
      const raw = (giaTri[khoaO(hv.maDangKy, maLoaiDiem)] ?? '').trim().replace(',', '.')
      if (raw === '') continue

      const n = Number(raw)
      if (!Number.isNaN(n)) so.push(n)
    }

    if (so.length === 0) return null

    return so.reduce((a, b) => a + b, 0) / so.length
  }

  /** Trung bình điểm tổng kết của những học viên đã đủ đầu điểm. */
  const trungBinhTongKet = () => {
    const so: number[] = []

    for (const hv of baoCao.hocViens) {
      const tk = tongKetCua(hv.maDangKy)
      if (tk !== null) so.push(tk)
    }

    if (so.length === 0) return null

    return so.reduce((a, b) => a + b, 0) / so.length
  }

  const tiLeNhap = tongSoO > 0 ? Math.round((soDaNhap / tongSoO) * 100) : 0

  return (
    <div className="space-y-3">
      {daCo.loi && <LoiTrang message={daCo.loi} onThuLai={daCo.taiLai} />}

      <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
        {/* ===== Hàng 1: tiêu đề + tiến độ + nút lưu ===== */}
        <div className="flex flex-wrap items-center gap-4 border-b border-slate-100 px-4 py-3">
          <div className="min-w-48 flex-1">
            <div className="flex items-baseline gap-2">
              <p className="text-sm font-bold text-slate-900">
                {chiXem ? 'Bảng điểm cả lớp' : 'Nhập điểm cả lớp'}
              </p>
              <span className="rounded-md bg-slate-100 px-1.5 py-0.5 text-[11px] font-bold text-slate-600">
                {tiLeNhap}%
              </span>
              {chiXem && (
                <span className="rounded-md bg-slate-100 px-1.5 py-0.5 text-[11px] font-bold text-slate-500">
                  Chỉ xem
                </span>
              )}
            </div>

            <p className="mt-0.5 text-xs text-slate-500">
              Đã nhập {soDaNhap}/{tongSoO} ô
              {chiXem ? (
                ' · Trợ giảng chỉ xem, việc nhập điểm do Giáo viên phụ trách'
              ) : soThayDoi > 0 ? (
                <span className="font-semibold text-amber-600">
                  {' · '}
                  {soThayDoi} thay đổi chưa lưu
                </span>
              ) : (
                ' · chưa có thay đổi'
              )}
            </p>

            <div className="mt-2 flex h-1.5 w-full max-w-72 overflow-hidden rounded-full bg-slate-100">
              <span
                className="bg-blue-500 transition-all duration-300"
                style={{ width: `${tiLeNhap}%` }}
              />
            </div>
          </div>

          {!chiXem && (
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
          )}
        </div>

        {/* ===== CẢNH BÁO ĐẦU ĐIỂM CHƯA TỚI NGÀY THI ===== */}
        {baoCao.thanhPhan.some((tp) => tp.coTheNhapDiem === false) && (
          <div className="flex flex-wrap items-center gap-x-2 gap-y-1 border-b border-amber-100 bg-amber-50/70 px-4 py-2 text-[11px] font-semibold text-amber-800">
            <span>Chưa tới ngày thi nên chưa nhập điểm được:</span>
            {baoCao.thanhPhan
              .filter((tp) => tp.coTheNhapDiem === false)
              .map((tp) => (
                <span key={tp.maLoaiDiem} className="rounded-md bg-white/80 px-1.5 py-0.5">
                  {tp.tenLoaiDiem} · {dinhDangNgay(tp.ngayThi ?? '')}
                </span>
              ))}
          </div>
        )}

        {/* ===== BẢNG ĐIỂM ===== */}
        <div ref={hopRef} className="overflow-x-auto">
          <table className="w-full min-w-max border-collapse text-sm">
            <thead>
              <tr className="border-b border-slate-200 bg-white">
                <th className="sticky left-0 z-10 bg-white px-4 py-3 text-left text-[11px] font-bold tracking-wider text-slate-400 uppercase">
                  Học viên
                </th>

                {baoCao.thanhPhan.map((tp, i) => {
                  const chuaToiNgay = tp.coTheNhapDiem === false

                  return (
                    <th
                      key={tp.maLoaiDiem}
                      className="px-2 py-3 text-center"
                      title={
                        chuaToiNgay
                          ? `Chưa tới ngày thi — chỉ được nhập điểm từ ${dinhDangNgay(tp.ngayThi ?? '')}`
                          : `Tối đa ${tp.diemToiDa} điểm`
                      }
                    >
                      <span className="inline-flex items-center gap-1.5 text-[11px] font-bold tracking-wider text-slate-600 uppercase">
                        <span className={`size-2 rounded-full ${mauDauDiem(i)}`} />
                        {tp.tenLoaiDiem}
                      </span>

                      <span className="mt-0.5 flex items-center justify-center gap-1 text-[11px] font-semibold">
                        <span className="text-slate-400">{tp.trongSo}%</span>

                        {tp.ngayThi && (
                          <>
                            <span className="text-slate-300">·</span>
                            <span className={chuaToiNgay ? 'text-amber-600' : 'text-slate-500'}>
                              {dinhDangNgay(tp.ngayThi)}
                            </span>
                          </>
                        )}

                        {chuaToiNgay && <span className="text-amber-600">khoá</span>}
                      </span>
                    </th>
                  )
                })}

                <th className="px-3 py-3 text-right text-[11px] font-bold tracking-wider text-slate-400 uppercase">
                  Tổng kết
                </th>
                <th className="px-3 py-3 text-center text-[11px] font-bold tracking-wider text-slate-400 uppercase">
                  Kết quả
                </th>
                <th className="px-3 py-3" />
              </tr>
            </thead>

            <tbody>
              {baoCao.hocViens.map((hv, chiSoDong) => {
                const tk = tongKetCua(hv.maDangKy)

                const duDauDiem = baoCao.thanhPhan.every(
                  (tp) => (giaTri[khoaO(hv.maDangKy, tp.maLoaiDiem)] ?? '').trim() !== '',
                )

                return (
                  <tr
                    key={hv.maDangKy}
                    className={`border-b border-slate-100 transition hover:bg-blue-50/60 ${
                      chiSoDong % 2 === 0 ? 'bg-white' : 'bg-slate-50/50'
                    }`}
                  >
                    <td className="sticky left-0 z-10 bg-inherit px-4 py-2">
                      <div className="flex items-center gap-2.5">
                        <span className="grid size-8 shrink-0 place-items-center rounded-full bg-slate-100 text-[11px] font-bold text-slate-500">
                          {chiSoDong + 1}
                        </span>
                        <div className="min-w-0">
                          <p className="truncate font-semibold text-slate-900">{hv.hoTen}</p>
                          <p className="font-mono text-[11px] text-slate-400">{hv.maHocVien}</p>
                        </div>
                      </div>
                    </td>

                    {baoCao.thanhPhan.map((tp, chiSoCot) => {
                      const k = khoaO(hv.maDangKy, tp.maLoaiDiem)
                      const raw = giaTri[k] ?? ''
                      const daSua = raw !== (goc[k] ?? '')
                      const sai = oSai(tp, raw)
                      const chuaToiNgay = tp.coTheNhapDiem === false

                      return (
                        <td key={tp.maLoaiDiem} className="px-1 py-2 text-center">
                          {chuaToiNgay ? (
                            <button
                              type="button"
                              onClick={() =>
                                hien(
                                  `Đầu điểm '${tp.tenLoaiDiem}' có buổi thi ngày ${dinhDangNgay(
                                    tp.ngayThi ?? '',
                                  )} — chỉ được nhập điểm từ ngày đó trở đi.`,
                                  'loi',
                                )
                              }
                              title="Chưa tới ngày thi"
                              className="h-10 w-20 cursor-not-allowed rounded-xl border-2 border-dashed border-slate-200 bg-slate-50 px-2 text-center text-sm font-bold text-slate-300"
                            >
                              {raw === '' ? '—' : raw}
                            </button>
                          ) : chiXem ? (
                            <span
                              title={
                                raw === ''
                                  ? 'Chưa có điểm'
                                  : `Tối đa ${tp.diemToiDa} điểm · chỉ xem`
                              }
                              className={`grid h-10 w-20 place-items-center rounded-xl border-2 px-2 text-sm font-bold ${
                                raw === ''
                                  ? 'border-slate-200 bg-slate-50 text-slate-300'
                                  : 'border-slate-200 bg-white text-slate-800'
                              }`}
                            >
                              {raw === '' ? '—' : raw}
                            </span>
                          ) : (
                            <input
                              data-dong={chiSoDong}
                              data-cot={chiSoCot}
                              inputMode="decimal"
                              value={raw}
                              placeholder="—"
                              title={`0 – ${tp.diemToiDa}`}
                              onChange={(e) => setGiaTri((cu) => ({ ...cu, [k]: e.target.value }))}
                              onKeyDown={(e) => {
                                if (e.key === 'Enter' || e.key === 'ArrowDown') {
                                  e.preventDefault()
                                  sangOTiepTheo(chiSoDong, chiSoCot, true)
                                } else if (e.key === 'ArrowUp') {
                                  e.preventDefault()
                                  sangOTiepTheo(chiSoDong, chiSoCot, false)
                                }
                              }}
                              className={`h-10 w-20 rounded-xl border-2 px-2 text-center text-sm font-bold outline-none transition focus:ring-2 focus:ring-blue-500 ${
                                sai
                                  ? 'border-rose-400 bg-rose-50 text-rose-600'
                                  : daSua
                                    ? 'border-amber-400 bg-amber-50 text-amber-800'
                                    : raw === ''
                                      ? 'border-slate-200 bg-white text-slate-400 hover:border-slate-300'
                                      : 'border-slate-200 bg-white text-slate-800 hover:border-blue-300'
                              }`}
                            />
                          )}
                        </td>
                      )
                    })}

                    <td className="px-3 py-2 text-right">
                      {tk === null ? (
                        <span className="text-slate-300">—</span>
                      ) : (
                        <span
                          className={`inline-flex items-center rounded-lg px-2.5 py-1 text-sm font-bold ${
                            tk >= 5
                              ? 'bg-emerald-50 text-emerald-700'
                              : 'bg-rose-50 text-rose-700'
                          }`}
                        >
                          {tk.toFixed(2)}
                        </span>
                      )}
                    </td>

                    <td className="px-3 py-2 text-center">
                      {duDauDiem ? (
                        <span className="inline-flex items-center gap-1 rounded-full border border-emerald-200 bg-emerald-50 px-2.5 py-1 text-[11px] font-bold text-emerald-700">
                          <Icon name="check" className="size-3" />
                          Đủ điều kiện
                        </span>
                      ) : (
                        <span className="inline-flex items-center gap-1 rounded-full border border-amber-200 bg-amber-50 px-2.5 py-1 text-[11px] font-bold text-amber-700">
                          <Icon name="alert" className="size-3" />
                          Thiếu điểm
                        </span>
                      )}
                    </td>

                    <td className="px-3 py-2 text-right">
                      {!chiXem && (
                        <button
                          type="button"
                          onClick={() => setNhapCho({ maDangKy: hv.maDangKy, hoTen: hv.hoTen })}
                          title="Nhập nhận xét cho từng đầu điểm"
                          className="inline-flex items-center gap-1 rounded-lg border border-slate-200 bg-white px-2.5 py-1.5 text-[11px] font-semibold text-slate-600 transition hover:border-blue-400 hover:text-blue-600"
                        >
                          <Icon name="messageCircle" className="size-3.5" />
                          Nhận xét
                        </button>
                      )}
                    </td>
                  </tr>
                )
              })}
            </tbody>

            <tfoot>
              <tr className="border-t-2 border-slate-200 bg-slate-50/80">
                <td className="sticky left-0 z-10 bg-inherit px-4 py-2.5 text-[11px] font-bold tracking-wider text-slate-500 uppercase">
                  Trung bình lớp
                </td>

                {baoCao.thanhPhan.map((tp) => {
                  const tb = trungBinhCot(tp.maLoaiDiem)

                  return (
                    <td
                      key={tp.maLoaiDiem}
                      className="px-2 py-2.5 text-center text-xs font-bold text-slate-600"
                    >
                      {tb === null ? '—' : tb.toFixed(2)}
                    </td>
                  )
                })}

                <td className="px-3 py-2.5 text-right text-sm font-bold text-slate-900">
                  {trungBinhTongKet()?.toFixed(2) ?? '—'}
                </td>
                <td colSpan={2} />
              </tr>
            </tfoot>
          </table>
        </div>
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

/** Loại thông báo gợi ý — chọn "Khác" để tự nhập. */
const LOAI_THONG_BAO_GOI_Y = ['Học vụ', 'Lịch học', 'Kiểm tra', 'Học phí', 'Khác'] as const

export function GVThongBao() {
  const { hien } = dungToast()

  const [tab, setTab] = useState<'daNhan' | 'daGui'>('daNhan')
  const [ketQua, setKetQua] = useState<PagedResult<NotificationItem> | null>(null)
  const [dangTai, setDangTai] = useState(true)
  const [loi, setLoi] = useState<string | null>(null)
  const [page, setPage] = useState(1)

  const tai = useCallback(async () => {
    setDangTai(true)
    setLoi(null)

    try {
      setKetQua(
        tab === 'daNhan'
          ? await thongBaoApi.cuaToi({ page, pageSize: PAGE_SIZE })
          : await thongBaoApi.daGui({ page, pageSize: PAGE_SIZE }),
      )
    } catch (error) {
      setLoi(thongDiepLoi(error))
    } finally {
      setDangTai(false)
    }
  }, [page, tab])

  useEffect(() => {
    void tai()
  }, [tai])

  const doiTab = (t: 'daNhan' | 'daGui') => {
    setTab(t)
    setPage(1)
  }

  // ===== GỬI THÔNG BÁO =====
  const [moGui, setMoGui] = useState(false)
  const [dsLop, setDsLop] = useState<TeacherClass[]>([])
  const [dangTaiLop, setDangTaiLop] = useState(false)
  const [dangGui, setDangGui] = useState(false)
  const [loiGui, setLoiGui] = useState<string[]>([])
  const [maLopHoc, setMaLopHoc] = useState('')
  const [tieuDe, setTieuDe] = useState('')
  const [noiDung, setNoiDung] = useState('')
  const [loaiChon, setLoaiChon] = useState<string>(LOAI_THONG_BAO_GOI_Y[0])
  const [loaiTuNhap, setLoaiTuNhap] = useState('')

  /** Thông báo đang mở hộp thoại chi tiết. */
  const [dangXem, setDangXem] = useState<NotificationItem | null>(null)

  const moHopThoaiGui = async () => {
    setMoGui(true)
    setLoiGui([])

    if (dsLop.length > 0) return

    setDangTaiLop(true)

    try {
      const ds = await giaoVienApi.lopCuaToi()
      setDsLop(ds)
      setMaLopHoc((truoc) => truoc || (ds[0] ? String(ds[0].maLopHoc) : ''))
    } catch (error) {
      setLoiGui([thongDiepLoi(error)])
    } finally {
      setDangTaiLop(false)
    }
  }

  const guiThongBao = async () => {
    const maLop = Number(maLopHoc)
    const loai = loaiChon === 'Khác' ? loaiTuNhap.trim() : loaiChon
    const loiNhap: string[] = []

    if (!maLop) loiNhap.push('Chọn lớp nhận thông báo.')
    if (tieuDe.trim().length < 3) loiNhap.push('Tiêu đề phải từ 3 ký tự.')
    if (tieuDe.trim().length > 255) loiNhap.push('Tiêu đề tối đa 255 ký tự.')
    if (noiDung.trim().length === 0) loiNhap.push('Nội dung không được để trống.')
    if (loai.length > 100) loiNhap.push('Loại thông báo tối đa 100 ký tự.')

    if (loiNhap.length > 0) {
      setLoiGui(loiNhap)
      return
    }

    setDangGui(true)
    setLoiGui([])

    try {
      await thongBaoApi.gui({
        maLopHoc: maLop,
        tieuDe: tieuDe.trim(),
        noiDung: noiDung.trim(),
        loaiThongBao: loai.length > 0 ? loai : null,
      })

      hien('Đã gửi thông báo tới lớp.', 'thanhCong')
      setMoGui(false)
      setTieuDe('')
      setNoiDung('')
      setLoaiChon(LOAI_THONG_BAO_GOI_Y[0])
      setLoaiTuNhap('')
      doiTab('daGui')
    } catch (error) {
      setLoiGui([thongDiepLoi(error)])
    } finally {
      setDangGui(false)
    }
  }

  const thuHoi = async (t: NotificationItem) => {
    const dongY = window.confirm(
      `Thu hồi thông báo "${t.tieuDe}"? Học viên và giáo viên của lớp sẽ không thấy thông báo này nữa.`,
    )

    if (!dongY) return

    try {
      await thongBaoApi.thuHoi(t.maThongBao)
      hien('Đã thu hồi thông báo.', 'thanhCong')
      setDangXem(null)
      await tai()
    } catch (error) {
      hien(thongDiepLoi(error), 'loi')
    }
  }

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Truyền thông"
        title="Thông báo"
        desc="Thông báo gửi tới bạn hoặc tới lớp bạn đang phụ trách."
        actions={
          <button type="button" onClick={() => void moHopThoaiGui()} className={LOP_NUT_CHINH}>
            <Icon name="bell" className="size-4" />
            Gửi thông báo
          </button>
        }
      />

      <div className="inline-flex rounded-xl border border-slate-200 bg-white p-1">
        {(
          [
            { ma: 'daNhan', nhan: 'Đã nhận' },
            { ma: 'daGui', nhan: 'Đã gửi' },
          ] as const
        ).map((t) => (
          <button
            key={t.ma}
            type="button"
            onClick={() => doiTab(t.ma)}
            className={`rounded-lg px-4 py-2 text-sm font-bold transition ${
              tab === t.ma
                ? 'bg-blue-600 text-white shadow-sm'
                : 'text-slate-600 hover:text-blue-600'
            }`}
          >
            {t.nhan}
          </button>
        ))}
      </div>

      {loi ? (
        <LoiTrang message={loi} onThuLai={tai} />
      ) : dangTai ? (
        <DangTai />
      ) : (ketQua?.items.length ?? 0) === 0 ? (
        <p className="rounded-2xl border border-dashed border-slate-300 bg-white p-10 text-center text-slate-400">
          {tab === 'daNhan'
            ? 'Chưa có thông báo nào gửi tới bạn.'
            : 'Bạn chưa gửi thông báo nào.'}
        </p>
      ) : (
        <>
          <ul className="space-y-3">
            {ketQua?.items.map((t) => (
              <li
                key={t.maThongBao}
                role="button"
                tabIndex={0}
                onClick={() => setDangXem(t)}
                onKeyDown={(e) => {
                  if (e.key === 'Enter' || e.key === ' ') {
                    e.preventDefault()
                    setDangXem(t)
                  }
                }}
                title="Bấm để xem chi tiết"
                className="cursor-pointer rounded-2xl border border-slate-200 bg-white p-5 shadow-sm outline-none transition hover:border-blue-300 hover:shadow-md focus-visible:border-blue-400 focus-visible:ring-4 focus-visible:ring-blue-50"
              >
                <div className="flex flex-wrap items-center gap-3">
                  <h3 className="text-base font-bold text-slate-900">{t.tieuDe}</h3>
                  {t.loaiThongBao && <Nhan tone="blue">{t.loaiThongBao}</Nhan>}

                  {tab === 'daGui' && (
                    <span
                      className={`inline-flex items-center rounded-full border px-2.5 py-0.5 text-[11px] font-bold ${
                        t.trangThai === 'Đã thu hồi'
                          ? 'border-slate-200 bg-slate-100 text-slate-500'
                          : 'border-emerald-200 bg-emerald-50 text-emerald-700'
                      }`}
                    >
                      {t.trangThai}
                    </span>
                  )}

                  <span className="ml-auto text-xs text-slate-400">
                    {dinhDangNgayGio(t.thoiGianGui)}
                  </span>
                </div>

                <p className="mt-2 whitespace-pre-line text-sm text-slate-600">{t.noiDung}</p>

                <div className="mt-3 flex flex-wrap items-center gap-x-2 gap-y-2 text-xs text-slate-400">
                  <span>{t.phamVi === 'Lớp' ? `Lớp ${t.maLop ?? ''}` : 'Cá nhân'}</span>
                  <span>·</span>
                  <span>Gửi bởi {t.tenDangNhapNguoiGui}</span>

                  {tab === 'daGui' && t.trangThai !== 'Đã thu hồi' && (
                    <button
                      type="button"
                      onClick={(e) => {
                        e.stopPropagation()
                        void thuHoi(t)
                      }}
                      className="ml-auto inline-flex items-center gap-1 rounded-lg border border-rose-200 bg-white px-2.5 py-1.5 text-[11px] font-semibold text-rose-600 transition hover:bg-rose-50"
                    >
                      <Icon name="close" className="size-3.5" />
                      Thu hồi
                    </button>
                  )}
                </div>
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

      {/* ===== HỘP THOẠI CHI TIẾT THÔNG BÁO ===== */}
      <HopThoai
        mo={dangXem !== null}
        tieuDe="Chi tiết thông báo"
        onDong={() => setDangXem(null)}
        footer={
          <>
            {tab === 'daGui' && dangXem && dangXem.trangThai !== 'Đã thu hồi' && (
              <button
                type="button"
                onClick={() => void thuHoi(dangXem)}
                className="inline-flex h-11 items-center justify-center gap-2 rounded-xl border border-rose-200 bg-white px-5 text-sm font-semibold text-rose-600 transition hover:bg-rose-50"
              >
                <Icon name="close" className="size-4" />
                Thu hồi
              </button>
            )}
            <button type="button" onClick={() => setDangXem(null)} className={LOP_NUT_PHU}>
              Đóng
            </button>
          </>
        }
      >
        {dangXem && (
          <div className="space-y-4">
            <div className="flex flex-wrap items-center gap-2">
              <h3 className="text-lg font-bold text-slate-900">{dangXem.tieuDe}</h3>
              {dangXem.loaiThongBao && <Nhan tone="blue">{dangXem.loaiThongBao}</Nhan>}
              <span
                className={`inline-flex items-center rounded-full border px-2.5 py-0.5 text-[11px] font-bold ${
                  dangXem.trangThai === 'Đã thu hồi'
                    ? 'border-slate-200 bg-slate-100 text-slate-500'
                    : 'border-emerald-200 bg-emerald-50 text-emerald-700'
                }`}
              >
                {dangXem.trangThai}
              </span>
            </div>

            <p className="whitespace-pre-line rounded-xl bg-slate-50 p-4 text-sm leading-6 text-slate-700">
              {dangXem.noiDung}
            </p>

            <dl className="grid grid-cols-1 gap-x-6 gap-y-3 text-sm sm:grid-cols-2">
              <div>
                <dt className="text-[11px] font-bold tracking-wider text-slate-400 uppercase">
                  Phạm vi
                </dt>
                <dd className="mt-0.5 font-semibold text-slate-700">{dangXem.phamVi}</dd>
              </div>

              <div>
                <dt className="text-[11px] font-bold tracking-wider text-slate-400 uppercase">
                  Lớp nhận
                </dt>
                <dd className="mt-0.5 font-semibold text-slate-700">
                  {dangXem.maLop ?? '—'}
                  {dangXem.tenLop ? ` · ${dangXem.tenLop}` : ''}
                </dd>
              </div>

              <div>
                <dt className="text-[11px] font-bold tracking-wider text-slate-400 uppercase">
                  Người nhận
                </dt>
                <dd className="mt-0.5 font-semibold text-slate-700">
                  {dangXem.tenDangNhapNguoiNhan ?? 'Toàn bộ học viên của lớp'}
                </dd>
              </div>

              <div>
                <dt className="text-[11px] font-bold tracking-wider text-slate-400 uppercase">
                  Người gửi
                </dt>
                <dd className="mt-0.5 font-semibold text-slate-700">
                  {dangXem.tenDangNhapNguoiGui}
                </dd>
              </div>

              <div>
                <dt className="text-[11px] font-bold tracking-wider text-slate-400 uppercase">
                  Thời gian gửi
                </dt>
                <dd className="mt-0.5 font-semibold text-slate-700">
                  {dinhDangNgayGio(dangXem.thoiGianGui)}
                </dd>
              </div>

              <div>
                <dt className="text-[11px] font-bold tracking-wider text-slate-400 uppercase">
                  Mã thông báo
                </dt>
                <dd className="mt-0.5 font-mono text-slate-500">#{dangXem.maThongBao}</dd>
              </div>
            </dl>
          </div>
        )}
      </HopThoai>

      {/* ===== HỘP THOẠI GỬI THÔNG BÁO ===== */}
      <HopThoai
        mo={moGui}
        tieuDe="Gửi thông báo cho lớp"
        onDong={() => {
          if (!dangGui) setMoGui(false)
        }}
        footer={
          <>
            <button
              type="button"
              onClick={() => setMoGui(false)}
              disabled={dangGui}
              className={LOP_NUT_PHU}
            >
              Hủy
            </button>
            <button
              type="button"
              onClick={() => void guiThongBao()}
              disabled={dangGui || dangTaiLop}
              className={LOP_NUT_CHINH}
            >
              {dangGui ? 'Đang gửi…' : 'Gửi thông báo'}
            </button>
          </>
        }
      >
        <div className="space-y-4">
          {loiGui.length > 0 && (
            <ul className="space-y-1 rounded-xl border border-rose-200 bg-rose-50 p-3 text-xs font-semibold text-rose-700">
              {loiGui.map((e) => (
                <li key={e}>• {e}</li>
              ))}
            </ul>
          )}

          <div>
            <label className={LOP_NHAN}>Lớp nhận (bắt buộc)</label>
            <select
              value={maLopHoc}
              onChange={(e) => setMaLopHoc(e.target.value)}
              disabled={dangTaiLop}
              className={LOP_INPUT}
            >
              {dangTaiLop && <option value="">Đang tải lớp…</option>}
              {!dangTaiLop && dsLop.length === 0 && <option value="">Chưa có lớp</option>}
              {dsLop.map((l) => (
                <option key={l.maLopHoc} value={l.maLopHoc}>
                  {l.maLop} — {l.tenLop} ({l.siSo} học viên)
                </option>
              ))}
            </select>
            <p className="mt-1 text-[11px] text-slate-400">
              Giáo viên/Trợ giảng chỉ gửi được cho lớp mình được phân công.
            </p>
          </div>

          <div>
            <label className={LOP_NHAN}>Tiêu đề</label>
            <input
              value={tieuDe}
              onChange={(e) => setTieuDe(e.target.value)}
              maxLength={255}
              placeholder="VD: Lớp nghỉ thứ 5, học bù thứ 7"
              className={LOP_INPUT}
            />
          </div>

          <div>
            <label className={LOP_NHAN}>Loại thông báo</label>
            <select
              value={loaiChon}
              onChange={(e) => setLoaiChon(e.target.value)}
              className={LOP_INPUT}
            >
              {LOAI_THONG_BAO_GOI_Y.map((l) => (
                <option key={l} value={l}>
                  {l}
                </option>
              ))}
            </select>

            {loaiChon === 'Khác' && (
              <input
                value={loaiTuNhap}
                onChange={(e) => setLoaiTuNhap(e.target.value)}
                maxLength={100}
                placeholder="Nhập loại thông báo…"
                className={`${LOP_INPUT} mt-2`}
              />
            )}
          </div>

          <div>
            <label className={LOP_NHAN}>Nội dung</label>
            <textarea
              value={noiDung}
              onChange={(e) => setNoiDung(e.target.value)}
              rows={5}
              placeholder="Nội dung thông báo gửi tới học viên của lớp…"
              className="w-full rounded-xl border border-slate-200 bg-white p-4 text-sm text-slate-900 outline-none transition focus:border-blue-500 focus:ring-4 focus:ring-blue-50"
            />
          </div>
        </div>
      </HopThoai>
    </div>
  )
}
