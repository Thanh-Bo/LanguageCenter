// Khu vực học viên.
import { useCallback, useEffect, useState } from 'react'
import { Link, useNavigate } from 'react-router'
import { Icon } from '../../components/BieuTuong'
import { ThoiKhoaBieuTuan, dungLichTuan } from '../../components/ThoiKhoaBieuTuan'
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
import { LOAI_CAU_HOI, PHUONG_THUC_MO_PHONG } from '../../lib/constants'
import {
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
                        to={`/hocvien/lop-cua-toi/${encodeURIComponent(lop.maLop)}`}
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
// LỊCH HỌC — dùng CHUNG lưới thời khóa biểu với Giáo viên / Admin
// ============================================================
export function HVLichHoc() {
  const lich = dungLichTuan((q) => hocVienSelfApi.lichHoc(q))
  const navigate = useNavigate()

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Học tập"
        title="Lịch học"
        desc="Thời khóa biểu theo tuần của các lớp bạn đang theo học. Bấm vào một ca để xem chi tiết buổi học."
      />

      <ThoiKhoaBieuTuan
        duLieu={lich.duLieu}
        tuanBatDau={lich.tuanBatDau}
        tuanKetThuc={lich.tuanKetThuc}
        onDoiTuan={lich.doiTuan}
        dangTai={lich.dangTai}
        loi={lich.loi}
        onThuLai={lich.taiLai}
        dongPhu={(ca) => [ca.tenPhong, ca.tenGiaoVienThucTe]}
        moTaRong="Tuần này bạn không có ca học nào."
        nhanDem="ca học"
        onChonCaHoc={(ca) =>
          navigate(`/hocvien/lop-cua-toi/${encodeURIComponent(ca.maLop)}?tab=buoi`)
        }
      />
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
        desc="Điểm tổng kết được tính theo trọng số từng thành phần do trung tâm quy định. Nếu một đầu điểm có nhiều lần kiểm tra, điểm hiển thị là lần mới nhất."
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
// HỌC PHÍ — học viên tự thanh toán (MÔ PHỎNG)
// ============================================================
export function HVHocPhi() {
  const { hien } = dungToast()
  const [page, setPage] = useState(1)
  const [dangTra, setDangTra] = useState<TuitionItem | null>(null)
  const [bienNhan, setBienNhan] = useState<Payment | null>(null)

  const hocPhi = useCallback(
    () => hocVienSelfApi.hocPhi({ page, pageSize: PAGE_SIZE }),
    [page],
  )
  const dsThanhToan = useCallback(
    () => hocVienSelfApi.thanhToan({ page: 1, pageSize: PAGE_SIZE }),
    [],
  )

  const hp = dungDuLieu<PagedResult<TuitionItem>>(hocPhi)
  const tt = dungDuLieu<PagedResult<Payment>>(dsThanhToan)

  const dsHocPhi = hp.duLieu?.items ?? []

  const tongPhaiDong = dsHocPhi.reduce((s, x) => s + x.soTienPhaiDong, 0)
  const tongDaDong = dsHocPhi.reduce((s, x) => s + x.soTienDaDong, 0)
  const tongConThieu = dsHocPhi.reduce((s, x) => s + x.soTienConThieu, 0)

  const hanGanNhat = dsHocPhi
    .filter((x) => x.soTienConThieu > 0 && x.hanThanhToan)
    .map((x) => x.hanThanhToan as string)
    .sort()[0]

  const taiLaiTatCa = () => {
    void hp.taiLai()
    void tt.taiLai()
  }

  const cot: CotBang<TuitionItem>[] = [
    {
      tieuDe: 'Khóa học',
      khoa: 'tenKhoaHoc',
      render: (r) => (
        <div>
          <p className="font-semibold text-slate-900">{r.tenKhoaHoc}</p>
          <p className="text-xs text-slate-500">{r.maLop ? `Lớp ${r.maLop}` : 'Chưa xếp lớp'}</p>
        </div>
      ),
    },
    {
      tieuDe: 'Phải đóng',
      khoa: 'soTienPhaiDong',
      canPhai: true,
      render: (r) => dinhDangTien(r.soTienPhaiDong),
    },
    {
      tieuDe: 'Đã đóng',
      khoa: 'soTienDaDong',
      canPhai: true,
      render: (r) => dinhDangTien(r.soTienDaDong),
    },
    {
      tieuDe: 'Còn thiếu',
      khoa: 'soTienConThieu',
      canPhai: true,
      render: (r) => (
        <span className={r.soTienConThieu > 0 ? 'font-bold text-rose-600' : 'text-slate-400'}>
          {dinhDangTien(r.soTienConThieu)}
        </span>
      ),
    },
    {
      tieuDe: 'Hạn đóng',
      khoa: 'hanThanhToan',
      render: (r) => (
        <span className={r.quaHan ? 'font-semibold text-rose-600' : ''}>
          {dinhDangNgay(r.hanThanhToan)}
          {r.quaHan && <span className="ml-1 text-[11px] font-bold">(quá hạn)</span>}
        </span>
      ),
    },
    {
      tieuDe: 'Trạng thái',
      khoa: 'trangThaiHocPhi',
      render: (r) => <Nhan>{r.trangThaiHocPhi}</Nhan>,
    },
    {
      tieuDe: '',
      khoa: 'thaoTac',
      render: (r) =>
        r.soTienConThieu > 0 ? (
          <button type="button" onClick={() => setDangTra(r)} className={LOP_NUT_CHINH}>
            Thanh toán
          </button>
        ) : (
          <span className="text-xs font-semibold text-emerald-600">Đã hoàn tất</span>
        ),
    },
  ]

  const cotTT: CotBang<Payment>[] = [
    {
      tieuDe: 'Thời gian',
      khoa: 'ngayThanhToan',
      render: (r) => dinhDangNgayGio(r.ngayThanhToan),
    },
    { tieuDe: 'Khóa học', khoa: 'tenKhoaHoc', render: (r) => r.tenKhoaHoc },
    { tieuDe: 'Số tiền', khoa: 'soTien', canPhai: true, render: (r) => dinhDangTien(r.soTien) },
    { tieuDe: 'Phương thức', khoa: 'phuongThucThanhToan' },
    {
      tieuDe: 'Biên nhận',
      khoa: 'bienNhan',
      render: (r) =>
        r.bienNhan ? (
          <button
            type="button"
            onClick={() => setBienNhan(r)}
            className="font-semibold text-blue-600 hover:underline"
          >
            {r.bienNhan.soBienNhan}
          </button>
        ) : (
          <span className="text-slate-400">—</span>
        ),
    },
    { tieuDe: 'Trạng thái', khoa: 'trangThai', render: (r) => <Nhan>{r.trangThai}</Nhan> },
  ]

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Tài chính"
        title="Học phí"
        desc="Theo dõi học phí, tự thanh toán (mô phỏng) và xem biên nhận."
      />

      <p className="rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm font-semibold text-amber-800">
        Cổng thanh toán MÔ PHỎNG — không phát sinh tiền thật. Biên nhận do hệ thống tự sinh để
        minh hoạ quy trình thu học phí.
      </p>

      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <TheChiSo
          nhan="Tổng phải đóng"
          giaTri={dinhDangTien(tongPhaiDong)}
          moTa={`${dsHocPhi.length} khoản học phí`}
        />
        <TheChiSo
          nhan="Đã đóng"
          giaTri={dinhDangTien(tongDaDong)}
          moTa="Bao gồm giao dịch mô phỏng"
        />
        <TheChiSo
          nhan="Còn thiếu"
          giaTri={dinhDangTien(tongConThieu)}
          moTa={tongConThieu > 0 ? 'Cần hoàn tất trước hạn' : 'Đã hoàn tất học phí'}
        />
        <TheChiSo
          nhan="Hạn gần nhất"
          giaTri={hanGanNhat ? dinhDangNgay(hanGanNhat) : '—'}
          moTa="Của khoản còn nợ"
        />
      </div>

      {hp.loi ? (
        <LoiTrang message={hp.loi} onThuLai={hp.taiLai} />
      ) : (
        <>
          <Bang
            cot={cot}
            duLieu={dsHocPhi}
            khoa={(r) => r.maHocPhi}
            dangTai={hp.dangTai}
            thongBaoRong="Bạn chưa có khoản học phí nào."
            lopDong={(r) => (r.quaHan ? 'bg-rose-50/40' : '')}
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
        <h2 className="text-sm font-bold tracking-wider text-slate-500 uppercase">
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

      <HopThoaiThanhToan
        hocPhi={dangTra}
        onDong={() => setDangTra(null)}
        onXong={() => {
          setDangTra(null)
          hien('Thanh toán mô phỏng thành công.', 'thanhCong')
          taiLaiTatCa()
        }}
        onXemBienNhan={(p) => {
          setDangTra(null)
          setBienNhan(p)
          taiLaiTatCa()
        }}
      />

      <HopThoaiBienNhan thanhToan={bienNhan} onDong={() => setBienNhan(null)} />
    </div>
  )
}

/** Bước 1 → 2 → 3 của cổng thanh toán mô phỏng. */
function HopThoaiThanhToan({
  hocPhi,
  onDong,
  onXong,
  onXemBienNhan,
}: {
  hocPhi: TuitionItem | null
  onDong: () => void
  onXong: () => void
  onXemBienNhan: (p: Payment) => void
}) {
  const [buoc, setBuoc] = useState<1 | 2 | 3>(1)
  const [soTien, setSoTien] = useState('')
  const [phuongThuc, setPhuongThuc] = useState<string>(PHUONG_THUC_MO_PHONG[0])
  /** Sinh MỘT LẦN cho mỗi phiếu → chống trả trùng khi bấm lại. */
  const [maGiaoDich, setMaGiaoDich] = useState('')
  const [dangGui, setDangGui] = useState(false)
  const [loi, setLoi] = useState<string | null>(null)
  const [ketQua, setKetQua] = useState<Payment | null>(null)

  useEffect(() => {
    if (!hocPhi) return

    setBuoc(1)
    setSoTien(String(hocPhi.soTienConThieu))
    setPhuongThuc(PHUONG_THUC_MO_PHONG[0])
    setMaGiaoDich(`SIM-${hocPhi.maHocPhi}-${Date.now()}`)
    setDangGui(false)
    setLoi(null)
    setKetQua(null)
  }, [hocPhi])

  if (!hocPhi) return null

  const conThieu = hocPhi.soTienConThieu
  const giaTri = Number(soTien.replace(/[^\d.]/g, ''))

  const tiepTuc = () => {
    if (!Number.isFinite(giaTri) || giaTri <= 0) {
      setLoi('Số tiền phải lớn hơn 0.')
      return
    }

    if (giaTri > conThieu) {
      setLoi(`Số tiền vượt quá số còn thiếu (${dinhDangTien(conThieu)}).`)
      return
    }

    setLoi(null)
    setBuoc(2)
  }

  const xacNhan = async () => {
    setDangGui(true)
    setLoi(null)

    try {
      const kq = await hocVienSelfApi.thanhToanHocPhi(hocPhi.maHocPhi, {
        maGiaoDich,
        soTien: giaTri,
        phuongThucThanhToan: phuongThuc,
      })
      setKetQua(kq)
      setBuoc(3)
    } catch (error) {
      setLoi(thongDiepLoi(error))
    } finally {
      setDangGui(false)
    }
  }

  const chanDung = hocPhi.maLop ? `${hocPhi.tenKhoaHoc} · Lớp ${hocPhi.maLop}` : hocPhi.tenKhoaHoc

  return (
    <HopThoai
      mo={!!hocPhi}
      tieuDe={buoc === 3 ? 'Thanh toán thành công' : 'Thanh toán học phí (mô phỏng)'}
      onDong={onDong}
      footer={
        buoc === 1 ? (
          <>
            <button type="button" onClick={onDong} className={LOP_NUT_PHU}>
              Hủy
            </button>
            <button type="button" onClick={tiepTuc} className={LOP_NUT_CHINH}>
              Tiếp tục
            </button>
          </>
        ) : buoc === 2 ? (
          <>
            <button type="button" onClick={() => setBuoc(1)} className={LOP_NUT_PHU}>
              Quay lại
            </button>
            <button
              type="button"
              onClick={() => void xacNhan()}
              disabled={dangGui}
              className={LOP_NUT_CHINH}
            >
              {dangGui ? 'Đang xử lý…' : 'Xác nhận thanh toán'}
            </button>
          </>
        ) : (
          <>
            <button type="button" onClick={onXong} className={LOP_NUT_PHU}>
              Đóng
            </button>
            <button
              type="button"
              onClick={() => ketQua && onXemBienNhan(ketQua)}
              className={LOP_NUT_CHINH}
            >
              Xem biên nhận
            </button>
          </>
        )
      }
    >
      <div className="space-y-4">
        {/* Chỉ báo 3 bước */}
        <div className="flex items-center gap-2 text-[11px] font-bold tracking-wider uppercase">
          {['Số tiền', 'Cổng thanh toán', 'Hoàn tất'].map((nhan, i) => {
            const so = i + 1
            const dangO = so === buoc
            const daXong = so < buoc

            return (
              <span key={nhan} className="flex items-center gap-2">
                <span
                  className={`grid size-6 place-items-center rounded-full ${
                    dangO
                      ? 'bg-blue-600 text-white'
                      : daXong
                        ? 'bg-emerald-100 text-emerald-700'
                        : 'bg-slate-100 text-slate-400'
                  }`}
                >
                  {so}
                </span>
                <span className={dangO ? 'text-slate-900' : 'text-slate-400'}>{nhan}</span>
                {i < 2 && <span className="text-slate-300">—</span>}
              </span>
            )
          })}
        </div>

        <div className="rounded-xl bg-slate-50 p-4 text-sm">
          <p className="font-bold text-slate-900">{hocPhi.hoTenHocVien}</p>
          <p className="text-slate-500">{chanDung}</p>
          <dl className="mt-3 grid grid-cols-2 gap-2 text-xs">
            <dt className="text-slate-500">Phải đóng</dt>
            <dd className="text-right font-semibold">{dinhDangTien(hocPhi.soTienPhaiDong)}</dd>
            <dt className="text-slate-500">Đã đóng</dt>
            <dd className="text-right font-semibold">{dinhDangTien(hocPhi.soTienDaDong)}</dd>
            <dt className="text-slate-500">Còn thiếu</dt>
            <dd className="text-right font-bold text-rose-600">{dinhDangTien(conThieu)}</dd>
          </dl>
        </div>

        {buoc === 1 && (
          <>
            <div>
              <label className={LOP_NHAN} htmlFor="hp-so-tien">
                Số tiền thanh toán
              </label>
              <input
                id="hp-so-tien"
                inputMode="numeric"
                value={soTien}
                onChange={(e) => setSoTien(e.target.value)}
                className={`${LOP_INPUT} h-10`}
              />
              <div className="mt-2 flex flex-wrap gap-2">
                <button
                  type="button"
                  onClick={() => setSoTien(String(conThieu))}
                  className="rounded-lg border border-slate-200 px-2.5 py-1 text-xs font-semibold text-slate-600 hover:border-blue-400 hover:text-blue-600"
                >
                  Toàn bộ ({dinhDangTien(conThieu)})
                </button>
                <button
                  type="button"
                  onClick={() => setSoTien(String(Math.round(conThieu / 2)))}
                  className="rounded-lg border border-slate-200 px-2.5 py-1 text-xs font-semibold text-slate-600 hover:border-blue-400 hover:text-blue-600"
                >
                  50%
                </button>
              </div>
            </div>

            <div>
              <p className={LOP_NHAN}>Phương thức thanh toán</p>
              <div className="grid gap-2 sm:grid-cols-3">
                {PHUONG_THUC_MO_PHONG.map((pt) => {
                  const dangChon = pt === phuongThuc

                  return (
                    <button
                      key={pt}
                      type="button"
                      onClick={() => setPhuongThuc(pt)}
                      className={`rounded-xl border-2 px-3 py-2 text-left text-xs font-bold transition ${
                        dangChon
                          ? 'border-blue-600 bg-blue-50 text-blue-700'
                          : 'border-slate-200 text-slate-600 hover:border-blue-300'
                      }`}
                    >
                      {pt}
                    </button>
                  )
                })}
              </div>
            </div>
          </>
        )}

        {buoc === 2 && (
          <div className="space-y-3 rounded-xl border-2 border-dashed border-slate-300 bg-white p-4">
            <p className="text-[11px] font-bold tracking-wider text-amber-600 uppercase">
              Cổng thanh toán mô phỏng — không phát sinh tiền thật
            </p>

            <p className="text-3xl font-bold text-slate-900">{dinhDangTien(giaTri)}</p>

            <div className="grid gap-2 rounded-lg bg-slate-50 p-3 text-xs text-slate-600">
              <span className="flex justify-between gap-3">
                <span>Ngân hàng</span>
                <span className="font-semibold text-slate-800">HUIT Lingohub Demo Bank</span>
              </span>
              <span className="flex justify-between gap-3">
                <span>Số tài khoản</span>
                <span className="font-mono font-semibold text-slate-800">0123 456 789</span>
              </span>
              <span className="flex justify-between gap-3">
                <span>Phương thức</span>
                <span className="font-semibold text-slate-800">{phuongThuc}</span>
              </span>
              <span className="flex justify-between gap-3">
                <span>Mã giao dịch</span>
                <span className="font-mono font-semibold text-slate-800">{maGiaoDich}</span>
              </span>
            </div>

            <div className="grid size-28 place-items-center justify-self-center rounded-xl border-4 border-slate-900 text-xs font-bold text-slate-400">
              QR DEMO
            </div>
          </div>
        )}

        {buoc === 3 && ketQua && (
          <div className="space-y-3 rounded-xl border border-emerald-200 bg-emerald-50 p-4 text-center">
            <span className="mx-auto grid size-12 place-items-center rounded-full bg-emerald-600 text-xl font-bold text-white">
              ✓
            </span>
            <p className="text-sm font-bold text-emerald-800">
              Đã ghi nhận {dinhDangTien(ketQua.soTien)}
            </p>
            <p className="text-xs text-emerald-700">
              Biên nhận: <span className="font-mono font-bold">{ketQua.bienNhan?.soBienNhan ?? '—'}</span>
              <br />
              Mã giao dịch: <span className="font-mono">{ketQua.maGiaoDich ?? maGiaoDich}</span>
            </p>
          </div>
        )}

        {loi && (
          <p className="rounded-xl border border-rose-200 bg-rose-50 px-3 py-2 text-sm font-semibold text-rose-700">
            {loi}
          </p>
        )}
      </div>
    </HopThoai>
  )
}

/** Biên nhận điện tử + xuất PDF (qua hộp thoại in của trình duyệt). */
function HopThoaiBienNhan({
  thanhToan,
  onDong,
}: {
  thanhToan: Payment | null
  onDong: () => void
}) {
  if (!thanhToan) return null

  const bn = thanhToan.bienNhan

  return (
    <HopThoai
      mo={!!thanhToan}
      tieuDe="Biên nhận thanh toán"
      onDong={onDong}
      footer={
        <>
          <button type="button" onClick={onDong} className={LOP_NUT_PHU}>
            Đóng
          </button>
          <button type="button" onClick={() => xuatBienNhanPdf(thanhToan)} className={LOP_NUT_CHINH}>
            Xuất PDF
          </button>
        </>
      }
    >
      <div className="space-y-3">
        <div className="rounded-xl border border-slate-200 p-4 text-sm">
          <div className="border-b border-dashed border-slate-200 pb-3 text-center">
            <p className="text-xs font-bold tracking-wider text-slate-500 uppercase">
              Trung tâm ngoại ngữ HUIT Lingohub
            </p>
            <p className="mt-1 text-base font-bold text-slate-900">BIÊN NHẬN THU HỌC PHÍ</p>
            <p className="text-xs text-slate-500">(bản mô phỏng — không có giá trị tài chính)</p>
          </div>

          <dl className="mt-3 space-y-1.5 text-xs">
            <div className="flex justify-between gap-3">
              <dt className="text-slate-500">Số biên nhận</dt>
              <dd className="font-mono font-bold text-slate-900">{bn?.soBienNhan ?? '—'}</dd>
            </div>
            <div className="flex justify-between gap-3">
              <dt className="text-slate-500">Ngày lập</dt>
              <dd className="font-semibold text-slate-800">
                {dinhDangNgayGio(bn?.ngayLap ?? thanhToan.ngayThanhToan)}
              </dd>
            </div>
            <div className="flex justify-between gap-3">
              <dt className="text-slate-500">Học viên</dt>
              <dd className="font-semibold text-slate-800">
                {thanhToan.hoTenHocVien} ({thanhToan.maHocVien})
              </dd>
            </div>
            <div className="flex justify-between gap-3">
              <dt className="text-slate-500">Khóa học</dt>
              <dd className="text-right font-semibold text-slate-800">{thanhToan.tenKhoaHoc}</dd>
            </div>
            <div className="flex justify-between gap-3">
              <dt className="text-slate-500">Phương thức</dt>
              <dd className="font-semibold text-slate-800">{thanhToan.phuongThucThanhToan}</dd>
            </div>
            <div className="flex justify-between gap-3">
              <dt className="text-slate-500">Mã giao dịch</dt>
              <dd className="font-mono text-slate-700">{thanhToan.maGiaoDich ?? '—'}</dd>
            </div>
            <div className="flex justify-between gap-3">
              <dt className="text-slate-500">Người ghi nhận</dt>
              <dd className="font-semibold text-slate-800">{thanhToan.tenNhanVienKeToan}</dd>
            </div>
          </dl>

          <div className="mt-3 flex items-center justify-between border-t border-dashed border-slate-200 pt-3">
            <span className="text-xs font-bold tracking-wider text-slate-500 uppercase">
              Số tiền
            </span>
            <span className="text-xl font-bold text-emerald-700">{dinhDangTien(thanhToan.soTien)}</span>
          </div>
        </div>

        <p className="text-xs text-slate-500">
          Bấm <b>Xuất PDF</b> → trong hộp thoại in của trình duyệt chọn <b>“Lưu thành PDF”</b>.
        </p>
      </div>
    </HopThoai>
  )
}

/** Mở cửa sổ in riêng để lưu biên nhận thành PDF. */
function xuatBienNhanPdf(p: Payment) {
  const bn = p.bienNhan
  const esc = (v: string) =>
    v.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;')

  const html = `<!doctype html>
<html lang="vi"><head><meta charset="utf-8" />
<title>Bien nhan ${esc(bn?.soBienNhan ?? '')}</title>
<style>
  * { box-sizing: border-box; }
  body { font-family: "Be Vietnam Pro", Arial, sans-serif; color: #1e293b; padding: 32px; }
  .wrap { max-width: 720px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 12px; padding: 28px; }
  .center { text-align: center; }
  .muted { color: #64748b; font-size: 12px; }
  h1 { font-size: 20px; margin: 6px 0 2px; }
  .hint { font-size: 11px; color: #b45309; margin-top: 2px; }
  table { width: 100%; border-collapse: collapse; margin-top: 18px; font-size: 13px; }
  td { padding: 7px 0; vertical-align: top; }
  td.k { color: #64748b; width: 40%; }
  td.v { text-align: right; font-weight: 600; }
  .total { border-top: 1px dashed #cbd5e1; margin-top: 14px; padding-top: 14px; display: flex; justify-content: space-between; align-items: center; }
  .amount { font-size: 22px; font-weight: 700; color: #047857; }
  .foot { margin-top: 28px; border-top: 1px solid #e2e8f0; padding-top: 12px; font-size: 11px; color: #94a3b8; }
  @page { margin: 12mm; }
</style></head>
<body><div class="wrap">
  <div class="center">
    <div class="muted" style="letter-spacing:.08em;text-transform:uppercase">Trung tâm ngoại ngữ HUIT Lingohub</div>
    <h1>BIÊN NHẬN THU HỌC PHÍ</h1>
    <div class="hint">Bản mô phỏng — không có giá trị tài chính</div>
  </div>
  <table>
    <tr><td class="k">Số biên nhận</td><td class="v">${esc(bn?.soBienNhan ?? '—')}</td></tr>
    <tr><td class="k">Ngày lập</td><td class="v">${esc(dinhDangNgayGio(bn?.ngayLap ?? p.ngayThanhToan))}</td></tr>
    <tr><td class="k">Học viên</td><td class="v">${esc(p.hoTenHocVien)} (${esc(p.maHocVien)})</td></tr>
    <tr><td class="k">Khóa học</td><td class="v">${esc(p.tenKhoaHoc)}</td></tr>
    <tr><td class="k">Phương thức</td><td class="v">${esc(p.phuongThucThanhToan)}</td></tr>
    <tr><td class="k">Mã giao dịch</td><td class="v">${esc(p.maGiaoDich ?? '—')}</td></tr>
    <tr><td class="k">Người ghi nhận</td><td class="v">${esc(p.tenNhanVienKeToan)} (${esc(p.maNhanVienKeToan)})</td></tr>
  </table>
  <div class="total"><span class="muted" style="text-transform:uppercase;letter-spacing:.08em">Số tiền</span><span class="amount">${esc(dinhDangTien(p.soTien))}</span></div>
  <div class="foot">Biên nhận được tạo tự động bởi hệ thống quản lý trung tâm. Giao dịch mô phỏng phục vụ minh hoạ quy trình thu học phí.</div>
</div></body></html>`

  const cuaSo = window.open('', '_blank', 'width=820,height=920')

  if (!cuaSo) {
    window.alert('Trình duyệt đã chặn cửa sổ in. Hãy cho phép popup rồi thử lại.')
    return
  }

  cuaSo.document.open()
  cuaSo.document.write(html)
  cuaSo.document.close()
  cuaSo.focus()
  window.setTimeout(() => cuaSo.print(), 400)
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

  /** Thông báo đang mở hộp thoại chi tiết. */
  const [dangXem, setDangXem] = useState<NotificationItem | null>(null)

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
                  <span className="ml-auto text-xs text-slate-400">
                    {dinhDangNgayGio(t.thoiGianGui)}
                  </span>
                </div>
                <p className="mt-2 line-clamp-2 whitespace-pre-line text-sm text-slate-600">
                  {t.noiDung}
                </p>
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

      {/* ===== HỘP THOẠI CHI TIẾT THÔNG BÁO ===== */}
      <HopThoai
        mo={dangXem !== null}
        tieuDe="Chi tiết thông báo"
        onDong={() => setDangXem(null)}
        footer={
          <button type="button" onClick={() => setDangXem(null)} className={LOP_NUT_PHU}>
            Đóng
          </button>
        }
      >
        {dangXem && (
          <div className="space-y-4">
            <div className="flex flex-wrap items-center gap-2">
              <h3 className="text-lg font-bold text-slate-900">{dangXem.tieuDe}</h3>
              {dangXem.loaiThongBao && <Nhan tone="blue">{dangXem.loaiThongBao}</Nhan>}
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
                  Lớp
                </dt>
                <dd className="mt-0.5 font-semibold text-slate-700">
                  {dangXem.maLop ?? '—'}
                  {dangXem.tenLop ? ` · ${dangXem.tenLop}` : ''}
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
    </div>
  )
}
