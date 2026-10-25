// Trang TIẾN ĐỘ HỌC của một lớp (khóa) mà học viên đang theo.
// Ghép dữ liệu: điểm (StudentCourseGrade) + ca học (SessionListItem) + điểm danh + học phí.
import { useState } from 'react'
import { Link, useParams } from 'react-router'
import { Icon } from '../../components/BieuTuong'
import { DangTai, LOP_NUT_PHU, LoiTrang, Nhan, TheChiSo, TrangTieuDe } from '../../components/ui'
import { hocVienSelfApi } from '../../lib/api/endpoints'
import type { SessionListItem } from '../../lib/api/types'
import {
  dinhDangKhoangGio,
  dinhDangNgay,
  dinhDangNgayGio,
  dinhDangTien,
  homNayIso,
  thuTrongTuan,
} from '../../lib/format'
import { dungDuLieu } from '../../lib/useApi'

/** Buổi đã học = ca đã hoàn thành hoặc đã qua ngày (trừ ca bị hủy). */
function laBuoiDaHoc(b: SessionListItem, homNay: string): boolean {
  if (b.trangThai === 'Đã hủy') return false
  return b.trangThai === 'Đã hoàn thành' || b.ngayHoc < homNay
}

/** Màu ô buổi trên timeline. */
function lopOBuoi(b: SessionListItem, diemDanh: string | undefined, homNay: string): string {
  if (b.trangThai === 'Đã hủy') return 'border-slate-200 bg-slate-100 text-slate-400'
  if (diemDanh === 'Vắng' || diemDanh === 'Vắng có phép')
    return 'border-rose-200 bg-rose-50 text-rose-600'
  if (diemDanh === 'Đi trễ') return 'border-amber-200 bg-amber-50 text-amber-700'
  if (diemDanh === 'Có mặt') return 'border-emerald-200 bg-emerald-50 text-emerald-700'
  if (laBuoiDaHoc(b, homNay)) return 'border-slate-200 bg-slate-50 text-slate-500'
  if (b.ngayHoc === homNay) return 'border-blue-400 bg-blue-50 text-blue-700'
  return 'border-slate-200 bg-white text-slate-400'
}

export function HVTienDoKhoaHoc() {
  const { maLopHoc } = useParams()
  const maLop = Number(maLopHoc)

  const [caDangChon, setCaDangChon] = useState<number | null>(null)

  const { duLieu, dangTai, loi, taiLai } = dungDuLieu(async () => {
    const [diem, hocPhi] = await Promise.all([
      hocVienSelfApi.diem(),
      hocVienSelfApi.hocPhi({ page: 1, pageSize: 50 }),
    ])

    const lop = diem.find((d) => d.maLopHoc === maLop)
    if (!lop) return null

    const [buoi, diemDanh] = await Promise.all([
      hocVienSelfApi.lichHoc({ tuNgay: lop.ngayBatDau, denNgay: lop.ngayKetThucDuKien }),
      hocVienSelfApi.diemDanh({ tuNgay: lop.ngayBatDau, denNgay: lop.ngayKetThucDuKien }),
    ])

    return {
      lop,
      buoi: buoi
        .filter((b) => b.maLopHoc === maLop)
        .sort((a, b) => `${a.ngayHoc}${a.gioBatDau}`.localeCompare(`${b.ngayHoc}${b.gioBatDau}`)),
      diemDanh: diemDanh.items.filter((i) => i.maLop === lop.maLop),
      hocPhi: hocPhi.items.find((h) => h.maDangKy === lop.maDangKy) ?? null,
    }
  })

  if (dangTai) return <DangTai nhan="Đang tải tiến độ khóa học…" />
  if (loi) return <LoiTrang message={loi} onThuLai={taiLai} />
  if (!duLieu) {
    return (
      <div className="space-y-6">
        <TrangTieuDe eyebrow="Học tập" title="Không tìm thấy lớp học" />
        <p className="rounded-2xl border border-dashed border-slate-300 bg-white p-8 text-center text-slate-500">
          Lớp này không thuộc danh sách lớp bạn đang theo học.
        </p>
        <Link to="/hocvien" className={LOP_NUT_PHU}>
          <Icon name="arrowRight" className="size-4 rotate-180" /> Về tổng quan
        </Link>
      </div>
    )
  }

  const { lop, buoi, diemDanh, hocPhi } = duLieu
  const homNay = homNayIso()

  const trangThaiTheoCa = new Map(diemDanh.map((d) => [d.maCaHoc, d.trangThaiDiemDanh]))

  const soBuoiDaHoc = buoi.filter((b) => laBuoiDaHoc(b, homNay)).length
  const tongBuoi = buoi.length
  const phanTramBuoi = tongBuoi > 0 ? Math.round((soBuoiDaHoc / tongBuoi) * 100) : 0

  const buoiTiepTheo = buoi.find((b) => b.ngayHoc >= homNay && b.trangThai !== 'Đã hủy')

  const soCoMat = diemDanh.filter((d) => d.trangThaiDiemDanh === 'Có mặt').length
  const soDiTre = diemDanh.filter((d) => d.trangThaiDiemDanh === 'Đi trễ').length
  const soVangCoPhep = diemDanh.filter((d) => d.trangThaiDiemDanh === 'Vắng có phép').length
  const soVang = diemDanh.filter((d) => d.trangThaiDiemDanh === 'Vắng').length

  const tiLeChuyenCan =
    diemDanh.length > 0 ? Math.round(((soCoMat + soDiTre) / diemDanh.length) * 100) : null

  const soDauDiemDaCo = lop.thanhPhan.length - lop.thanhPhanConThieu.length

  return (
    <div className="space-y-6">
      <div>
        <Link
          to="/hocvien"
          className="mb-3 inline-flex items-center gap-1.5 text-sm font-semibold text-slate-500 hover:text-blue-600"
        >
          <Icon name="arrowRight" className="size-4 rotate-180" /> Về tổng quan
        </Link>

        <TrangTieuDe
          eyebrow="Tiến độ khóa học"
          title={lop.tenLop}
          desc={`${lop.maLop} · ${lop.tenKhoaHoc} · ${dinhDangNgay(lop.ngayBatDau)} → ${dinhDangNgay(
            lop.ngayKetThucDuKien,
          )}`}
          actions={
            <div className="flex items-center gap-2">
              <Nhan tone={lop.duDieuKien ? 'emerald' : 'amber'}>
                {lop.duDieuKien ? 'Đủ điều kiện' : 'Chưa đủ điều kiện'}
              </Nhan>
              <button type="button" onClick={taiLai} className={LOP_NUT_PHU}>
                <Icon name="arrowUp" className="size-4" /> Làm mới
              </button>
            </div>
          }
        />
      </div>

      {/* 4 chỉ số chính */}
      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <TheChiSo
          nhan="Buổi đã học"
          giaTri={`${soBuoiDaHoc}/${tongBuoi || 20}`}
          icon="calendar"
          moTa={tongBuoi > 0 ? `${phanTramBuoi}% khóa học` : 'Lớp chưa bắt đầu'}
        />
        <TheChiSo
          nhan="Chuyên cần"
          giaTri={tiLeChuyenCan === null ? '—' : `${tiLeChuyenCan}%`}
          icon="check"
          tone={tiLeChuyenCan === null ? 'blue' : tiLeChuyenCan >= 80 ? 'emerald' : 'rose'}
          moTa={
            diemDanh.length === 0
              ? 'Chưa có buổi nào được điểm danh'
              : `Có mặt ${soCoMat} · Trễ ${soDiTre} · Vắng ${soVang + soVangCoPhep}`
          }
        />
        <TheChiSo
          nhan="Điểm tổng kết"
          giaTri={lop.diemTongKet != null ? lop.diemTongKet.toFixed(2) : '—'}
          icon="chart"
          tone="amber"
          moTa={`${soDauDiemDaCo}/${lop.thanhPhan.length} đầu điểm đã có`}
        />
        <TheChiSo
          nhan="Học phí còn thiếu"
          giaTri={hocPhi ? dinhDangTien(hocPhi.soTienConThieu) : '—'}
          icon="wallet"
          tone={hocPhi && hocPhi.soTienConThieu > 0 ? 'rose' : 'emerald'}
          moTa={
            hocPhi
              ? `Đã đóng ${dinhDangTien(hocPhi.soTienDaDong)} / ${dinhDangTien(hocPhi.soTienPhaiDong)}`
              : 'Chưa có thông tin học phí'
          }
        />
      </div>

      {/* Thanh tiến độ */}
      <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
        <div className="mb-2 flex flex-wrap items-end justify-between gap-2">
          <h2 className="text-sm font-bold uppercase tracking-wider text-slate-500">
            Tiến độ {tongBuoi > 0 ? `${tongBuoi} buổi` : 'khóa học'}
          </h2>
          <p className="text-sm text-slate-500">
            Đã học <span className="font-bold text-slate-900">{soBuoiDaHoc}</span>
            {tongBuoi > 0 ? ` / ${tongBuoi}` : ''} buổi
          </p>
        </div>

        <div className="h-2.5 w-full overflow-hidden rounded-full bg-slate-100">
          <div
            className="h-full rounded-full bg-blue-600 transition-all"
            style={{ width: `${phanTramBuoi}%` }}
          />
        </div>

        {tongBuoi === 0 ? (
          <p className="mt-4 rounded-xl bg-amber-50 p-3 text-sm font-semibold text-amber-700">
            Lớp chưa được xếp lịch buổi học. Vui lòng liên hệ phòng học vụ.
          </p>
        ) : (
          <>
            <div className="mt-4 grid grid-cols-5 gap-2 sm:grid-cols-10">
              {buoi.map((b, i) => {
                const diemDanhCuaBuoi = trangThaiTheoCa.get(b.maCaHoc)
                const dangChon = caDangChon === b.maCaHoc

                return (
                  <button
                    key={b.maCaHoc}
                    type="button"
                    onClick={() => setCaDangChon(dangChon ? null : b.maCaHoc)}
                    title={`Buổi ${i + 1} · ${dinhDangNgay(b.ngayHoc)} · ${
                      diemDanhCuaBuoi ?? b.trangThai
                    }`}
                    className={`rounded-lg border px-1 py-2 text-center text-xs font-bold transition hover:brightness-95 ${
                      dangChon ? 'ring-2 ring-blue-500 ring-offset-1' : ''
                    } ${lopOBuoi(b, diemDanhCuaBuoi, homNay)}`}
                  >
                    <span className="block text-[11px] leading-none opacity-70">Buổi</span>
                    <span className="text-sm">{i + 1}</span>
                  </button>
                )
              })}
            </div>

            <div className="mt-3 flex flex-wrap items-center gap-x-4 gap-y-1 text-xs text-slate-500">
              <span className="flex items-center gap-1.5">
                <span className="size-3 rounded border border-emerald-200 bg-emerald-50" /> Có mặt
              </span>
              <span className="flex items-center gap-1.5">
                <span className="size-3 rounded border border-amber-200 bg-amber-50" /> Đi trễ
              </span>
              <span className="flex items-center gap-1.5">
                <span className="size-3 rounded border border-rose-200 bg-rose-50" /> Vắng
              </span>
              <span className="flex items-center gap-1.5">
                <span className="size-3 rounded border border-slate-200 bg-white" /> Chưa diễn ra
              </span>
              <span className="flex items-center gap-1.5">
                <span className="size-3 rounded border border-slate-200 bg-slate-100" /> Đã hủy
              </span>
            </div>
          </>
        )}

        {/* Chi tiết buổi được chọn */}
        {caDangChon !== null &&
          (() => {
            const b = buoi.find((x) => x.maCaHoc === caDangChon)
            if (!b) return null
            const viTri = buoi.findIndex((x) => x.maCaHoc === caDangChon) + 1
            const dd = trangThaiTheoCa.get(b.maCaHoc)

            return (
              <div className="mt-4 rounded-xl border border-blue-200 bg-blue-50/50 p-4">
                <div className="flex flex-wrap items-center justify-between gap-2">
                  <p className="font-bold text-slate-900">
                    Buổi {viTri} · {thuTrongTuan(b.ngayHoc)}, {dinhDangNgay(b.ngayHoc)}
                  </p>
                  <Nhan tone={dd ? 'blue' : 'slate'}>{dd ?? b.trangThai}</Nhan>
                </div>
                <p className="mt-1 text-sm text-slate-600">
                  {dinhDangKhoangGio(b.gioBatDau, b.gioKetThuc)} · {b.tenPhong}
                  {b.tenGiaoVienThucTe ? ` · GV ${b.tenGiaoVienThucTe}` : ''}
                </p>
                <p className="mt-2 text-sm text-slate-700">
                  <span className="font-semibold">Nội dung: </span>
                  {b.noiDungBaiHoc ?? 'Chưa cập nhật'}
                </p>
              </div>
            )
          })()}
      </section>

      {/* Buổi tiếp theo + Thành phần điểm */}
      <div className="grid gap-4 lg:grid-cols-2">
        <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
          <h2 className="mb-3 text-sm font-bold uppercase tracking-wider text-slate-500">
            Buổi học tiếp theo
          </h2>
          {buoiTiepTheo ? (
            <div className="rounded-xl border border-slate-100 bg-slate-50/70 p-4">
              <p className="text-lg font-bold text-slate-900">
                {thuTrongTuan(buoiTiepTheo.ngayHoc)}, {dinhDangNgay(buoiTiepTheo.ngayHoc)}
              </p>
              <p className="mt-1 text-sm text-slate-600">
                {dinhDangKhoangGio(buoiTiepTheo.gioBatDau, buoiTiepTheo.gioKetThuc)} ·{' '}
                {buoiTiepTheo.tenPhong}
                {buoiTiepTheo.tenGiaoVienThucTe ? ` · GV ${buoiTiepTheo.tenGiaoVienThucTe}` : ''}
              </p>
              {buoiTiepTheo.noiDungBaiHoc && (
                <p className="mt-2 text-sm text-slate-700">{buoiTiepTheo.noiDungBaiHoc}</p>
              )}
              <div className="mt-3 flex gap-2">
                <Link to="/hocvien/lich-hoc" className={LOP_NUT_PHU}>
                  <Icon name="calendar" className="size-4" /> Xem lịch học
                </Link>
              </div>
            </div>
          ) : (
            <p className="text-sm text-slate-400">
              {tongBuoi === 0
                ? 'Chưa có buổi học nào được xếp.'
                : 'Đã học hết các buổi của lớp này.'}
            </p>
          )}
        </section>

        <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
          <div className="mb-3 flex items-center justify-between">
            <h2 className="text-sm font-bold uppercase tracking-wider text-slate-500">
              Thành phần điểm
            </h2>
            <span className="text-xs font-semibold text-slate-500">
              Tổng trọng số {lop.tongTrongSo}%
            </span>
          </div>

          {lop.thanhPhan.length === 0 ? (
            <p className="text-sm text-slate-400">Khóa học chưa cấu hình đầu điểm.</p>
          ) : (
            <ul className="space-y-2">
              {lop.thanhPhan.map((tp) => {
                const tiLe =
                  tp.diem != null && tp.diemToiDa > 0
                    ? Math.round((tp.diem / tp.diemToiDa) * 100)
                    : 0

                return (
                  <li key={tp.maLoaiDiem}>
                    <div className="flex items-center justify-between text-sm">
                      <span className="text-slate-700">{tp.tenLoaiDiem}</span>
                      <span className="text-slate-500">
                        {tp.trongSo}% ·{' '}
                        <span
                          className={tp.diem == null ? 'font-semibold text-slate-400' : 'font-bold text-slate-900'}
                        >
                          {tp.diem != null ? tp.diem.toFixed(1) : '—'}
                        </span>
                        <span className="text-slate-400">/{tp.diemToiDa}</span>
                      </span>
                    </div>
                    <div className="mt-1 h-1.5 w-full overflow-hidden rounded-full bg-slate-100">
                      <div
                        className={`h-full rounded-full ${tiLe >= 50 ? 'bg-blue-600' : 'bg-rose-400'}`}
                        style={{ width: `${tiLe}%` }}
                      />
                    </div>
                  </li>
                )
              })}
            </ul>
          )}

          {lop.thanhPhanConThieu.length > 0 && (
            <p className="mt-3 rounded-lg bg-amber-50 p-2.5 text-xs font-semibold text-amber-700">
              Còn thiếu: {lop.thanhPhanConThieu.join(', ')}
            </p>
          )}
        </section>
      </div>

      {/* Học phí */}
      {hocPhi && (
        <section className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
          <div className="flex flex-wrap items-center justify-between gap-3">
            <div>
              <h2 className="text-sm font-bold uppercase tracking-wider text-slate-500">Học phí</h2>
              <p className="mt-1 text-sm text-slate-600">
                Đã đóng <span className="font-bold text-slate-900">{dinhDangTien(hocPhi.soTienDaDong)}</span>
                {' / '}
                {dinhDangTien(hocPhi.soTienPhaiDong)}
                {hocPhi.soTienConThieu > 0 && (
                  <>
                    {' · còn thiếu '}
                    <span className="font-bold text-rose-600">{dinhDangTien(hocPhi.soTienConThieu)}</span>
                  </>
                )}
                {hocPhi.hanThanhToan && ` · hạn ${dinhDangNgay(hocPhi.hanThanhToan)}`}
              </p>
            </div>
            <div className="flex items-center gap-2">
              <Nhan tone={hocPhi.soTienConThieu > 0 ? (hocPhi.quaHan ? 'rose' : 'amber') : 'emerald'}>
                {hocPhi.trangThaiHocPhi}
              </Nhan>
              <Link to="/hocvien/hoc-phi" className={LOP_NUT_PHU}>
                Xem chi tiết
              </Link>
            </div>
          </div>
          {hocPhi.ngayTao && (
            <p className="mt-2 text-xs text-slate-400">
              Phiếu học phí tạo lúc {dinhDangNgayGio(hocPhi.ngayTao)}
            </p>
          )}
        </section>
      )}
    </div>
  )
}
