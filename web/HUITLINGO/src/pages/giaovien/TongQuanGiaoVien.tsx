// Tổng quan GIÁO VIÊN / TRỢ GIẢNG — cùng bố cục với học viên, dữ liệu theo vai trò giảng dạy.
import { Link } from 'react-router'
import { Icon } from '../../components/BieuTuong'
import {
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
import { dungXacThuc } from '../../contexts/NguCanhXacThuc'
import { giaoVienApi, hoSoApi, thongBaoApi } from '../../lib/api/endpoints'
import type { SessionListItem } from '../../lib/api/types'
import {
  congNgayIso,
  dauTuanIso,
  dinhDangKhoangGio,
  dinhDangNgay,
  homNayIso,
  thuTrongTuan,
} from '../../lib/format'
import { dungDuLieu } from '../../lib/useApi'

const LOI_TAT: DuLieuLoiTat[] = [
  { nhan: 'Lớp của tôi', duong: '/giaovien/lop-cua-toi', icon: 'book' },
  { nhan: 'Lịch dạy', duong: '/giaovien/lich-day', icon: 'calendar' },
  { nhan: 'Điểm danh', duong: '/giaovien/diem-danh', icon: 'calendarCheck' },
  { nhan: 'Bảng điểm', duong: '/giaovien/bang-diem', icon: 'chart' },
  { nhan: 'Thông báo', duong: '/giaovien/thong-bao', icon: 'bell' },
  { nhan: 'Cài đặt', duong: '/giaovien/cai-dat', icon: 'settings' },
]

export function GVTongQuan() {
  const { nguoiDung } = dungXacThuc()

  const { duLieu, dangTai, loi, taiLai } = dungDuLieu(async () => {
    const homNay = homNayIso()
    const dauTuan = dauTuanIso(homNay)
    const cuoiTuan = congNgayIso(dauTuan, 6)

    const [hoSo, lop, thongBao] = await Promise.all([
      hoSoApi.giaoVien(),
      giaoVienApi.lopCuaToi(),
      thongBaoApi.tongHop(),
    ])

    // Ca dạy toàn bộ thời gian của các lớp → vừa tính tuần này vừa tính tiến độ giảng dạy.
    let caDay: SessionListItem[] = []

    if (lop.length > 0) {
      const tuNgay = lop.map((l) => l.ngayBatDau).sort()[0]
      const denNgay = lop.map((l) => l.ngayKetThucDuKien).sort().pop() ?? homNay

      try {
        caDay = await giaoVienApi.lichDay({ tuNgay, denNgay })
      } catch {
        caDay = []
      }
    }

    return { hoSo, lop, thongBao, caDay, dauTuan, cuoiTuan, homNay }
  })

  if (dangTai) return <DangTai nhan="Đang tải tổng quan giảng dạy…" />
  if (loi) return <LoiTrang message={loi} onThuLai={taiLai} />
  if (!duLieu) return null

  const { hoSo, lop, thongBao, caDay, dauTuan, cuoiTuan, homNay } = duLieu
  const laTroGiang = (nguoiDung?.vaiTro ?? '') === 'Trợ giảng'
  const nhanVaiTro = laTroGiang ? 'Trợ giảng' : 'Giáo viên'

  const caTuanNay = caDay.filter(
    (c) => c.ngayHoc >= dauTuan && c.ngayHoc <= cuoiTuan && c.trangThai !== 'Đã hủy',
  )

  const daDay = caDay.filter(
    (c) => c.trangThai === 'Đã hoàn thành' || (c.ngayHoc < homNay && c.trangThai !== 'Đã hủy'),
  ).length

  const sapToi = caDay
    .filter((c) => c.ngayHoc >= homNay && c.trangThai !== 'Đã hủy')
    .sort((a, b) => `${a.ngayHoc}${a.gioBatDau}`.localeCompare(`${b.ngayHoc}${b.gioBatDau}`))
    .slice(0, 6)

  const tongHocVien = lop.reduce((s, l) => s + l.siSo, 0)
  const soChuaDoc = thongBao.soChuaDoc ?? 0

  const dsChiSo: DuLieuChiSo[] = [
    {
      nhan: 'Thông báo mới',
      giaTri: soChuaDoc,
      moTa: soChuaDoc > 0 ? `Tổng ${thongBao.tongSo} thông báo` : 'Bạn đã xem hết thông báo',
      icon: 'bell',
      tone: 'blue',
      duong: '/giaovien/thong-bao',
    },
    {
      nhan: 'Ca dạy tuần này',
      giaTri: caTuanNay.length,
      moTa: `${dinhDangNgay(dauTuan)} → ${dinhDangNgay(cuoiTuan)}`,
      icon: 'calendar',
      tone: 'blue',
      duong: '/giaovien/lich-day',
    },
    {
      nhan: 'Lớp phụ trách',
      giaTri: lop.length,
      moTa: `${tongHocVien} học viên đang theo học`,
      icon: 'book',
      tone: 'blue',
      duong: '/giaovien/lop-cua-toi',
    },
  ]

  const thongTin: MucThongTin[] = [
    { nhan: 'Mã ' + nhanVaiTro.toLowerCase(), giaTri: hoSo.maGiaoVien },
    { nhan: 'Trạng thái làm việc', giaTri: hoSo.trangThai },
    { nhan: 'Họ tên', giaTri: hoSo.hoTen },
    { nhan: 'Loại', giaTri: hoSo.loaiGiaoVien ?? nhanVaiTro },
    { nhan: 'Giới tính', giaTri: hoSo.gioiTinh },
    { nhan: 'Chuyên môn', giaTri: hoSo.chuyenMon },
    { nhan: 'Ngày sinh', giaTri: hoSo.ngaySinh ? dinhDangNgay(hoSo.ngaySinh) : null },
    {
      nhan: 'Số năm kinh nghiệm',
      giaTri: hoSo.soNamKinhNghiem != null ? `${hoSo.soNamKinhNghiem} năm` : null,
    },
    { nhan: 'Email', giaTri: hoSo.email },
    { nhan: 'Số điện thoại', giaTri: hoSo.soDienThoai },
    { nhan: 'Ngày vào làm', giaTri: hoSo.ngayVaoLam ? dinhDangNgay(hoSo.ngayVaoLam) : null },
    { nhan: 'Địa chỉ', giaTri: hoSo.diaChi },
  ]

  return (
    <div className="space-y-5">
      {/* Hàng 1: thông tin + chỉ số */}
      <div className="grid gap-4 xl:grid-cols-[minmax(0,1fr)_400px]">
        <TheThongTin
          tieuDe={`Thông tin ${nhanVaiTro.toLowerCase()}`}
          phuDe="Hồ sơ giảng dạy của bạn trên hệ thống LingoHub"
          hoTen={hoSo.hoTen ?? nguoiDung?.hoTen ?? nhanVaiTro}
          vaiTro={nhanVaiTro}
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

      {/* Hàng 3: 2 khối dữ liệu (đã bỏ khối "Ca dạy trong tuần") */}
      <div className="grid gap-4 lg:grid-cols-2">
        <KhoiNoiDung tieuDe="Tiến độ giảng dạy" moTa="Số ca đã dạy trên tổng số ca của các lớp">
          {caDay.length === 0 ? (
            <TrongRong noiDung="Chưa có lịch dạy cho các lớp được phân công." />
          ) : (
            <div className="space-y-3">
              <VongTienDo giaTri={daDay} tong={caDay.length} nhan="ca đã hoàn thành" mau="#1D4ED8" />
              <p className="text-center text-xs text-slate-500">
                {Math.round((daDay / caDay.length) * 100)}% khối lượng giảng dạy · còn{' '}
                {Math.max(0, caDay.length - daDay)} ca
              </p>
            </div>
          )}
        </KhoiNoiDung>

        <KhoiNoiDung
          tieuDe="Lớp phụ trách"
          moTa="Lớp được phân công giảng dạy / trợ giảng"
          hanhDong={
            lop.length > 0 ? (
              <span className="text-xs font-semibold text-slate-400">{lop.length} lớp</span>
            ) : null
          }
        >
          {lop.length === 0 ? (
            <TrongRong noiDung="Bạn chưa được phân công lớp nào." />
          ) : (
            <ul className="space-y-2">
              {lop.map((l) => {
                const ca = caDay.filter((c) => c.maLopHoc === l.maLopHoc)
                const tong = ca.length
                const xong = ca.filter(
                  (c) =>
                    c.trangThai !== 'Đã hủy' &&
                    (c.trangThai === 'Đã hoàn thành' || c.ngayHoc < homNay),
                ).length
                const phanTram = tong > 0 ? Math.round((xong / tong) * 100) : 0

                return (
                  <li
                    key={l.maLopHoc}
                    className="rounded-xl border border-slate-100 bg-slate-50/70 p-3"
                  >
                    <div className="flex items-start justify-between gap-2">
                      <div className="min-w-0">
                        <p className="truncate text-sm font-bold text-slate-900">{l.tenLop}</p>
                        <p className="text-[11px] text-slate-500">
                          {l.maLop} · {l.tenKhoaHoc}
                        </p>
                      </div>
                      <Nhan>{l.trangThai}</Nhan>
                    </div>

                    <div className="mt-2 h-1.5 w-full overflow-hidden rounded-full bg-white">
                      <div
                        className="h-full rounded-full bg-blue-600"
                        style={{ width: `${phanTram}%` }}
                      />
                    </div>

                    <p className="mt-1 flex items-center justify-between text-[11px] text-slate-500">
                      <span>
                        {xong}
                        {tong > 0 ? `/${tong}` : ''} ca · {l.siSo}/{l.siSoToiDa} học viên
                      </span>
                      <span className="font-semibold text-slate-600">{l.vaiTroPhanCong}</span>
                    </p>
                  </li>
                )
              })}
            </ul>
          )}
        </KhoiNoiDung>
      </div>

      {/* Ca dạy sắp tới */}
      <KhoiNoiDung
        tieuDe="Ca dạy sắp tới"
        moTa="Các ca gần nhất chưa diễn ra"
        hanhDong={
          <Link to="/giaovien/lich-day" className="text-xs font-bold text-blue-600 hover:underline">
            Xem tất cả
          </Link>
        }
      >
        {sapToi.length === 0 ? (
          <TrongRong noiDung="Không có ca dạy nào sắp tới." />
        ) : (
          <ul className="grid gap-2 md:grid-cols-2">
            {sapToi.map((c) => (
              <li
                key={c.maCaHoc}
                className="flex flex-wrap items-center gap-3 rounded-xl border border-slate-100 bg-slate-50/60 p-3"
              >
                <span className="grid size-10 shrink-0 place-items-center rounded-xl bg-blue-600 text-white">
                  <Icon name="calendar" className="size-5" />
                </span>
                <div className="min-w-0 flex-1">
                  <p className="truncate text-sm font-semibold text-slate-900">
                    {c.maLop} · {c.tenLop}
                  </p>
                  <p className="text-[11px] text-slate-500">
                    {thuTrongTuan(c.ngayHoc)}, {dinhDangNgay(c.ngayHoc)} ·{' '}
                    {dinhDangKhoangGio(c.gioBatDau, c.gioKetThuc)} · {c.tenPhong}
                  </p>
                </div>
                <Nhan>{c.trangThai}</Nhan>
              </li>
            ))}
          </ul>
        )}
      </KhoiNoiDung>
    </div>
  )
}
