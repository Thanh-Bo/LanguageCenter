// Tổng quan HỌC VIÊN — bố cục: thẻ thông tin cá nhân + 3 chỉ số → dải lối tắt → 3 khối dữ liệu.
import { Link } from 'react-router'
import { Icon } from '../../components/BieuTuong'
import {
  BieuDoCot,
  DaiLoiTat,
  KhoiNoiDung,
  TheChiSoLon,
  TheThongTin,
  TrongRong,
  VongTienDo,
  type DuLieuChiSo,
  type DuLieuLoiTat,
  type MucThongTin,
} from '../../components/TongQuanPortal'
import { DangTai, Nhan, LoiTrang } from '../../components/ui'
import { hoSoApi, hocVienSelfApi, thongBaoApi } from '../../lib/api/endpoints'
import type { SessionListItem } from '../../lib/api/types'
import {
  congNgayIso,
  dauTuanIso,
  dinhDangKhoangGio,
  dinhDangNgay,
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

const LOI_TAT: DuLieuLoiTat[] = [
  { nhan: 'Lịch học', duong: '/hocvien/lich-hoc', icon: 'calendar' },
  { nhan: 'Điểm số', duong: '/hocvien/diem-so', icon: 'chart' },
  { nhan: 'Học phí', duong: '/hocvien/hoc-phi', icon: 'wallet' },
  { nhan: 'Khảo sát', duong: '/hocvien/khao-sat', icon: 'badgeCheck' },
  { nhan: 'Thông báo', duong: '/hocvien/thong-bao', icon: 'bell' },
  { nhan: 'Cài đặt', duong: '/hocvien/cai-dat', icon: 'settings' },
]

export function HVTongQuan() {
  const { duLieu, dangTai, loi, taiLai } = dungDuLieu(async () => {
    const homNay = homNayIso()
    const dauTuan = dauTuanIso(homNay)
    const cuoiTuan = congNgayIso(dauTuan, 6)

    const [hoSo, diem, hocPhi, thongBao, khaoSat] = await Promise.all([
      hoSoApi.hocVien(),
      hocVienSelfApi.diem(),
      hocVienSelfApi.hocPhi({ page: 1, pageSize: 50 }),
      thongBaoApi.tongHop(),
      hocVienSelfApi.khaoSat(),
    ])

    // Lấy toàn bộ ca học của các lớp đang theo → dùng cho tuần này + tiến độ từng lớp.
    let buoi: SessionListItem[] = []

    if (diem.length > 0) {
      const tuNgay = diem.map((d) => d.ngayBatDau).sort()[0]
      const denNgay = diem.map((d) => d.ngayKetThucDuKien).sort().pop() ?? homNay

      try {
        buoi = await hocVienSelfApi.lichHoc({ tuNgay, denNgay })
      } catch {
        buoi = []
      }
    }

    return {
      hoSo,
      diem,
      buoi,
      hocPhi: hocPhi.items,
      thongBao,
      soKhaoSat: khaoSat.length,
      dauTuan,
      cuoiTuan,
      homNay,
    }
  })

  if (dangTai) return <DangTai nhan="Đang tải tổng quan học tập…" />
  if (loi) return <LoiTrang message={loi} onThuLai={taiLai} />
  if (!duLieu) return null

  const { hoSo, diem, buoi, hocPhi, thongBao, dauTuan, cuoiTuan, homNay } = duLieu

  const daHoc = buoi.filter((b) => laBuoiDaHoc(b, homNay)).length
  const tongBuoi = buoi.length

  const buoiTuanNay = buoi.filter(
    (b) => b.ngayHoc >= dauTuan && b.ngayHoc <= cuoiTuan && b.trangThai !== 'Đã hủy',
  )

  const conThieu = hocPhi.reduce((s, h) => s + h.soTienConThieu, 0)
  const soChuaDoc = thongBao.soChuaDoc ?? 0

  const dsChiSo: DuLieuChiSo[] = [
    {
      nhan: 'Thông báo mới',
      giaTri: soChuaDoc,
      moTa: soChuaDoc > 0 ? `Tổng ${thongBao.tongSo} thông báo` : 'Bạn đã xem hết thông báo',
      icon: 'bell',
      tone: 'blue',
      duong: '/hocvien/thong-bao',
    },
    {
      nhan: 'Buổi học tuần này',
      giaTri: buoiTuanNay.length,
      moTa: `${dinhDangNgay(dauTuan)} → ${dinhDangNgay(cuoiTuan)}`,
      icon: 'calendar',
      tone: 'blue',
      duong: '/hocvien/lich-hoc',
    },
    {
      nhan: 'Học phí còn thiếu',
      giaTri: dinhDangTien(conThieu),
      moTa: conThieu > 0 ? 'Xem chi tiết để thanh toán' : 'Đã đóng đủ học phí',
      icon: 'wallet',
      tone: conThieu > 0 ? 'rose' : 'blue',
      duong: '/hocvien/hoc-phi',
    },
  ]

  const thongTin: MucThongTin[] = [
    { nhan: 'Mã học viên', giaTri: hoSo.maHocVien },
    { nhan: 'Trạng thái', giaTri: hoSo.trangThaiHocTap },
    { nhan: 'Họ tên', giaTri: hoSo.hoTen },
    { nhan: 'Trình độ hiện tại', giaTri: hoSo.trinhDoHienTai },
    { nhan: 'Giới tính', giaTri: hoSo.gioiTinh },
    { nhan: 'Mục tiêu học tập', giaTri: hoSo.mucTieuHocTap },
    { nhan: 'Ngày sinh', giaTri: hoSo.ngaySinh ? dinhDangNgay(hoSo.ngaySinh) : null },
    { nhan: 'Ngày nhập học', giaTri: hoSo.ngayNhapHoc ? dinhDangNgay(hoSo.ngayNhapHoc) : null },
    { nhan: 'Email', giaTri: hoSo.email },
    { nhan: 'Số điện thoại', giaTri: hoSo.soDienThoai },
    { nhan: 'Địa chỉ', giaTri: hoSo.diaChi },
    { nhan: 'Số lớp đang theo', giaTri: `${diem.length} lớp` },
  ]

  const cotDiem = diem
    .filter((d) => d.diemTongKet != null)
    .slice(0, 8)
    .map((d) => ({ nhan: d.maLop, giaTri: Number(d.diemTongKet) }))

  const lopTheoMa = new Map(diem.map((d) => [d.maLopHoc, d]))

  return (
    <div className="space-y-5">
      {/* Hàng 1: thông tin + chỉ số */}
      <div className="grid gap-4 xl:grid-cols-[minmax(0,1fr)_400px]">
        <TheThongTin
          tieuDe="Thông tin học viên"
          phuDe="Hồ sơ của bạn trên hệ thống LingoHub"
          hoTen={hoSo.hoTen ?? 'Học viên'}
          vaiTro="Học viên"
          thongTin={thongTin}
        />

        <div className="grid gap-4">
          <TheChiSoLon chiSo={dsChiSo[0]} co="lon" />
          <div className="grid gap-4 sm:grid-cols-2">
            <TheChiSoLon chiSo={dsChiSo[1]} />
            <TheChiSoLon chiSo={dsChiSo[2]} />
          </div>
        </div>
      </div>

      {/* Hàng 2: lối tắt */}
      <DaiLoiTat ds={LOI_TAT} />

      {/* Hàng 3: 3 khối dữ liệu */}
      <div className="grid gap-4 lg:grid-cols-3">
        <KhoiNoiDung
          tieuDe="Kết quả học tập"
          moTa="Điểm tổng kết theo từng lớp"
          hanhDong={
            <Link to="/hocvien/diem-so" className="text-xs font-bold text-blue-600 hover:underline">
              Bảng điểm
            </Link>
          }
        >
          {cotDiem.length === 0 ? (
            <TrongRong noiDung="Chưa có điểm tổng kết nào được công bố." />
          ) : (
            <BieuDoCot items={cotDiem} />
          )}
        </KhoiNoiDung>

        <KhoiNoiDung tieuDe="Tiến độ học tập" moTa="Số buổi đã học trên tổng số buổi">
          {tongBuoi === 0 ? (
            <TrongRong noiDung="Lớp chưa được xếp lịch buổi học." />
          ) : (
            <div className="space-y-3">
              <VongTienDo giaTri={daHoc} tong={tongBuoi} nhan="buổi đã hoàn thành" />
              <p className="text-center text-xs text-slate-500">
                {tongBuoi > 0 ? Math.round((daHoc / tongBuoi) * 100) : 0}% khối lượng khóa học ·
                còn {Math.max(0, tongBuoi - daHoc)} buổi
              </p>
            </div>
          )}
        </KhoiNoiDung>

        <KhoiNoiDung
          tieuDe="Lớp học phần"
          moTa="Bấm vào lớp để xem tiến độ chi tiết"
          hanhDong={
            diem.length > 0 ? (
              <span className="text-xs font-semibold text-slate-400">{diem.length} lớp</span>
            ) : null
          }
        >
          {diem.length === 0 ? (
            <TrongRong noiDung="Bạn chưa được xếp vào lớp học nào." />
          ) : (
            <ul className="space-y-2">
              {diem.map((lop) => {
                const buoiCuaLop = buoi.filter((b) => b.maLopHoc === lop.maLopHoc)
                const tong = buoiCuaLop.length
                const hoc = buoiCuaLop.filter((b) => laBuoiDaHoc(b, homNay)).length
                const phanTram = tong > 0 ? Math.round((hoc / tong) * 100) : 0

                return (
                  <li key={lop.maDangKy}>
                    <Link
                      to={`/hocvien/khoa-hoc/${lop.maLopHoc}`}
                      className="block rounded-xl border border-slate-100 bg-slate-50/70 p-3 transition hover:border-blue-200 hover:bg-blue-50/50"
                    >
                      <div className="flex items-start justify-between gap-2">
                        <div className="min-w-0">
                          <p className="truncate text-sm font-bold text-slate-900">{lop.tenLop}</p>
                          <p className="text-[11px] text-slate-500">
                            {lop.maLop} · {lop.tenKhoaHoc}
                          </p>
                        </div>
                        <Nhan tone={lop.duDieuKien ? 'blue' : 'slate'}>
                          {lop.diemTongKet != null ? lop.diemTongKet.toFixed(2) : '—'}
                        </Nhan>
                      </div>

                      <div className="mt-2 h-1.5 w-full overflow-hidden rounded-full bg-white">
                        <div
                          className="h-full rounded-full bg-blue-600"
                          style={{ width: `${phanTram}%` }}
                        />
                      </div>

                      <p className="mt-1 flex items-center justify-between text-[11px] text-slate-500">
                        <span>
                          {hoc}
                          {tong > 0 ? `/${tong}` : ''} buổi
                        </span>
                        <span className="inline-flex items-center gap-1 font-semibold text-blue-600">
                          Xem tiến độ <Icon name="arrowRight" className="size-3" />
                        </span>
                      </p>
                    </Link>
                  </li>
                )
              })}
            </ul>
          )}
        </KhoiNoiDung>
      </div>

      {/* Buổi học tuần này */}
      <KhoiNoiDung
        tieuDe="Lịch học tuần này"
        moTa={`${dinhDangNgay(dauTuan)} → ${dinhDangNgay(cuoiTuan)}`}
        hanhDong={
          <Link to="/hocvien/lich-hoc" className="text-xs font-bold text-blue-600 hover:underline">
            Xem thời khóa biểu
          </Link>
        }
      >
        {buoiTuanNay.length === 0 ? (
          <TrongRong noiDung="Tuần này bạn không có buổi học nào." />
        ) : (
          <ul className="grid gap-2 md:grid-cols-2">
            {buoiTuanNay
              .slice()
              .sort((a, b) => `${a.ngayHoc}${a.gioBatDau}`.localeCompare(`${b.ngayHoc}${b.gioBatDau}`))
              .map((b) => {
                const lop = lopTheoMa.get(b.maLopHoc)

                return (
                  <li
                    key={b.maCaHoc}
                    className="flex flex-wrap items-center gap-3 rounded-xl border border-slate-100 bg-slate-50/60 p-3"
                  >
                    <span className="grid size-10 shrink-0 place-items-center rounded-xl bg-blue-600 text-white">
                      <Icon name="clock" className="size-5" />
                    </span>
                    <div className="min-w-0 flex-1">
                      <p className="truncate text-sm font-semibold text-slate-900">{lop?.tenLop ?? b.tenLop}</p>
                      <p className="text-[11px] text-slate-500">
                        {thuTrongTuan(b.ngayHoc)}, {dinhDangNgay(b.ngayHoc)} ·{' '}
                        {dinhDangKhoangGio(b.gioBatDau, b.gioKetThuc)} · {b.tenPhong}
                      </p>
                    </div>
                    <Nhan>{b.trangThai}</Nhan>
                  </li>
                )
              })}
          </ul>
        )}
      </KhoiNoiDung>
    </div>
  )
}
