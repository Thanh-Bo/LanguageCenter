// Chi tiết lớp học cho Admin: Tiến độ · Lịch & ca học · Phân công · Học viên.
import { useCallback, useEffect, useMemo, useState } from 'react'
import { Icon } from '../../components/BieuTuong'
import { TienDoBuoiHoc } from '../../components/TienDoBuoiHoc'
import {
  DangTai,
  HopThoai,
  LOP_NUT_CHINH,
  LOP_NUT_PHU,
  LoiTrang,
  Nhan,
  TrongRong,
} from '../../components/ui'
import { dungToast } from '../../components/ui/toast'
import { caHocApi, giaoVienAdminApi, lopHocApi } from '../../lib/api/endpoints'
import type {
  AssignmentItem,
  ClassProgress,
  EnrollmentItem,
  ScheduleListItem,
  SessionListItem,
  TeacherOption,
} from '../../lib/api/types'
import { LOAI_BUOI, TRANG_THAI_CA_HOC, tenThu } from '../../lib/constants'
import { dinhDangKhoangGio, dinhDangNgay } from '../../lib/format'
import { thongDiepLoi } from '../../lib/useApi'

type Tab = 'tienDo' | 'caHoc' | 'phanCong' | 'hocVien'

const NHAN_TAB: Array<{ khoa: Tab; nhan: string }> = [
  { khoa: 'tienDo', nhan: 'Tiến độ buổi học' },
  { khoa: 'caHoc', nhan: 'Lịch & ca học' },
  { khoa: 'phanCong', nhan: 'Phân công' },
  { khoa: 'hocVien', nhan: 'Học viên' },
]

/** Số buổi tối đa đã diễn ra mà lớp còn nhận học viên (khớp EnrollmentRules backend). */
const SO_BUOI_TOI_DA_NHAN_HV = 4

function tuoiTuNgaySinh(ngaySinh?: string | null): number | null {
  if (!ngaySinh) return null
  const sinh = new Date(ngaySinh)
  if (Number.isNaN(sinh.getTime())) return null
  const homNay = new Date()
  let tuoi = homNay.getFullYear() - sinh.getFullYear()
  const chuaToiSinhNhat =
    homNay.getMonth() < sinh.getMonth() ||
    (homNay.getMonth() === sinh.getMonth() && homNay.getDate() < sinh.getDate())
  if (chuaToiSinhNhat) tuoi -= 1
  return tuoi
}

export function LopHocChiTiet({
  maLopHoc,
  tenLop,
  mo,
  onDong,
}: {
  maLopHoc: number
  tenLop: string
  mo: boolean
  onDong: () => void
}) {
  const { hien } = dungToast()

  const [tab, setTab] = useState<Tab>('tienDo')
  const [dangTai, setDangTai] = useState(false)
  const [dangBan, setDangBan] = useState(false)
  const [loi, setLoi] = useState<string | null>(null)

  const [tienDo, setTienDo] = useState<ClassProgress | null>(null)
  const [caHocs, setCaHocs] = useState<SessionListItem[]>([])
  const [phanCongs, setPhanCongs] = useState<AssignmentItem[]>([])
  const [dangKys, setDangKys] = useState<EnrollmentItem[]>([])
  const [lichHocs, setLichHocs] = useState<ScheduleListItem[]>([])
  const [giaoViens, setGiaoViens] = useState<TeacherOption[]>([])
  const [khongXemDuocDangKy, setKhongXemDuocDangKy] = useState(false)

  /** Danh sách GV chỉ Quản trị viên xem được → lỗi 403 không làm vỡ modal. */
  const taiGiaoVien = async (): Promise<TeacherOption[]> => {
    try {
      const kq = await giaoVienAdminApi.danhSach({ loaiGiaoVien: 'Giáo viên' })
      return kq.items
    } catch {
      return []
    }
  }

  /** Danh sách đăng ký chỉ Quản trị viên / Tư vấn xem được. */
  const taiDangKy = async (maLop: number): Promise<EnrollmentItem[]> => {
    try {
      const kq = await lopHocApi.dangKy(maLop)
      setKhongXemDuocDangKy(false)
      return kq.items
    } catch {
      setKhongXemDuocDangKy(true)
      return []
    }
  }

  const tai = useCallback(async () => {
    setDangTai(true)
    setLoi(null)

    try {
      const [td, ch, pc, dk, lh, gv] = await Promise.all([
        lopHocApi.tienDo(maLopHoc),
        caHocApi.danhSach({ maLopHoc, page: 1, pageSize: 200 }),
        lopHocApi.phanCong(maLopHoc),
        taiDangKy(maLopHoc),
        lopHocApi.lichHoc(maLopHoc),
        taiGiaoVien(),
      ])

      setTienDo(td)
      setCaHocs([...ch.items].sort((a, b) => `${a.ngayHoc}${a.gioBatDau}`.localeCompare(`${b.ngayHoc}${b.gioBatDau}`)))
      setPhanCongs(pc.items)
      setDangKys(dk)
      setLichHocs(lh.items)
      setGiaoViens(gv)
    } catch (error) {
      setLoi(thongDiepLoi(error))
    } finally {
      setDangTai(false)
    }
  }, [maLopHoc])

  useEffect(() => {
    if (!mo) return
    setTab('tienDo')
    void tai()
  }, [mo, tai])

  const lichDangDung = useMemo(
    () => lichHocs.find((l) => l.trangThai === 'Hoạt động') ?? lichHocs[0] ?? null,
    [lichHocs],
  )

  const giaoVienNuocNgoai = useMemo(
    () => phanCongs.find((p) => p.vaiTroPhanCong === 'Giáo viên nước ngoài') ?? null,
    [phanCongs],
  )

  const quaSoBuoiNhanHocVien = (tienDo?.soBuoiDaHoc ?? 0) > SO_BUOI_TOI_DA_NHAN_HV

  const sinhBuoi = useCallback(async () => {
    if (!lichDangDung) {
      hien('Lớp chưa có lịch định kỳ. Hãy tạo lịch học (2 thứ/tuần) trước.', 'loi')
      return
    }

    setDangBan(true)

    try {
      await caHocApi.sinhBuoi(lichDangDung.maLichHoc)
      hien('Đã sinh buổi học cho lớp.', 'thanhCong')
      await tai()
    } catch (error) {
      hien(thongDiepLoi(error), 'loi')
    } finally {
      setDangBan(false)
    }
  }, [hien, lichDangDung, tai])

  const dongBoGiaoVien = useCallback(async () => {
    setDangBan(true)

    try {
      const kq = await caHocApi.dongBoGiaoVien(maLopHoc)
      hien(
        `Đã đồng bộ ${kq.soCaCapNhat}/${kq.tongSoCa} ca (${kq.soCaKhongDoi} ca đã đúng, ${kq.soCaBoQua} ca bỏ qua).`,
        'thanhCong',
      )
      await tai()
    } catch (error) {
      hien(thongDiepLoi(error), 'loi')
    } finally {
      setDangBan(false)
    }
  }, [hien, maLopHoc, tai])

  const doiLoaiBuoi = useCallback(
    async (ca: SessionListItem, loaiBuoi: string) => {
      setDangBan(true)

      try {
        await caHocApi.capNhat(ca.maCaHoc, { loaiBuoi })
        hien(`Đã đổi ${ca.noiDungBaiHoc ?? `buổi ${ca.soBuoi}`} sang "${loaiBuoi}".`, 'thanhCong')
        await tai()
      } catch (error) {
        hien(thongDiepLoi(error), 'loi')
      } finally {
        setDangBan(false)
      }
    },
    [hien, tai],
  )

  const doiTrangThai = useCallback(
    async (ca: SessionListItem, trangThai: string) => {
      setDangBan(true)

      try {
        await caHocApi.doiTrangThai(ca.maCaHoc, trangThai)
        hien('Đã cập nhật trạng thái ca học.', 'thanhCong')
        await tai()
      } catch (error) {
        hien(thongDiepLoi(error), 'loi')
      } finally {
        setDangBan(false)
      }
    },
    [hien, tai],
  )

  const doiGiaoVien = useCallback(
    async (ca: SessionListItem, maGiaoVien: string) => {
      // Backend không nhận chuỗi rỗng (MinLength=1) → chỉ đổi khi đã chọn người.
      if (!maGiaoVien) {
        hien('Hãy chọn một giáo viên cụ thể.', 'loi')
        return
      }

      setDangBan(true)

      try {
        await caHocApi.capNhat(ca.maCaHoc, { maGiaoVienThucTe: maGiaoVien })
        hien('Đã đổi giáo viên cho ca học.', 'thanhCong')
        await tai()
      } catch (error) {
        hien(thongDiepLoi(error), 'loi')
      } finally {
        setDangBan(false)
      }
    },
    [hien, tai],
  )

  return (
    <HopThoai
      mo={mo}
      tieuDe={`Lớp ${tenLop} — chi tiết`}
      onDong={onDong}
      rong="max-w-5xl"
      footer={
        <>
          <button type="button" className={LOP_NUT_PHU} onClick={onDong}>
            Đóng
          </button>
          <button
            type="button"
            className={LOP_NUT_CHINH}
            onClick={() => void tai()}
            disabled={dangTai || dangBan}
          >
            <Icon name="calendarCheck" className="size-4" /> Tải lại
          </button>
        </>
      }
    >
      <div className="mb-4 flex flex-wrap gap-1 rounded-xl bg-slate-100 p-1">
        {NHAN_TAB.map((t) => (
          <button
            key={t.khoa}
            type="button"
            onClick={() => setTab(t.khoa)}
            className={`flex-1 rounded-lg px-3 py-2 text-sm font-semibold transition ${
              tab === t.khoa ? 'bg-white text-blue-700 shadow-sm' : 'text-slate-500 hover:text-slate-700'
            }`}
          >
            {t.nhan}
          </button>
        ))}
      </div>

      {loi ? (
        <LoiTrang message={loi} onThuLai={() => void tai()} />
      ) : dangTai ? (
        <DangTai nhan="Đang tải chi tiết lớp…" />
      ) : tab === 'tienDo' ? (
        tienDo ? (
          <div className="space-y-4">
            <TienDoBuoiHoc progress={tienDo} onChonBuoi={() => setTab('caHoc')} />
            {giaoVienNuocNgoai?.tenGiaoVien && (
              <p className="rounded-xl border border-blue-100 bg-blue-50 px-4 py-3 text-sm text-blue-800">
                Buổi đầu tuần do <strong>GV nước ngoài</strong> dạy:{' '}
                {giaoVienNuocNgoai.tenGiaoVien}. Buổi còn lại do giáo viên chính phụ trách.
              </p>
            )}
          </div>
        ) : (
          <TrongRong>Không có dữ liệu tiến độ.</TrongRong>
        )
      ) : tab === 'caHoc' ? (
        <div className="space-y-4">
          <div className="flex flex-wrap items-center gap-2">
            <button
              type="button"
              className={LOP_NUT_PHU}
              onClick={() => void sinhBuoi()}
              disabled={dangBan}
            >
              <Icon name="calendarCheck" className="size-4" /> Sinh 20 buổi
            </button>
            <button
              type="button"
              className={LOP_NUT_PHU}
              onClick={() => void dongBoGiaoVien()}
              disabled={dangBan}
            >
              <Icon name="users" className="size-4" /> Đồng bộ giáo viên
            </button>
            <span className="text-xs text-slate-500">
              {lichDangDung
                ? `Lịch: ${tenThu(lichDangDung.thuTrongTuan)} ${dinhDangKhoangGio(lichDangDung.gioBatDau, lichDangDung.gioKetThuc)}`
                : 'Lớp chưa khai lịch định kỳ'}
            </span>
          </div>

          {caHocs.length === 0 ? (
            <TrongRong>Chưa có ca học. Bấm "Sinh 20 buổi" để tạo từ lịch định kỳ.</TrongRong>
          ) : (
            <div className="overflow-x-auto rounded-xl border border-slate-200">
              <table className="w-full min-w-max text-sm">
                <thead className="bg-slate-50 text-left text-xs font-bold uppercase tracking-wider text-slate-500">
                  <tr>
                    <th className="px-3 py-2">Buổi</th>
                    <th className="px-3 py-2">Ngày</th>
                    <th className="px-3 py-2">Giờ</th>
                    <th className="px-3 py-2">Giáo viên</th>
                    <th className="px-3 py-2">Trợ giảng</th>
                    <th className="px-3 py-2">Loại buổi</th>
                    <th className="px-3 py-2">Trạng thái</th>
                    <th className="px-3 py-2 text-right">Điểm danh</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-100">
                  {caHocs.map((ca) => (
                    <tr key={ca.maCaHoc} className={ca.loaiBuoi === 'Buổi thi' ? 'bg-violet-50/50' : ''}>
                      <td className="px-3 py-2 font-semibold text-slate-700">
                        Buổi {ca.soBuoi ?? '—'}
                      </td>
                      <td className="px-3 py-2">{dinhDangNgay(ca.ngayHoc)}</td>
                      <td className="px-3 py-2 text-xs text-slate-500">
                        {dinhDangKhoangGio(ca.gioBatDau, ca.gioKetThuc)}
                      </td>
                      <td className="px-3 py-2">
                        {giaoViens.length > 0 ? (
                          <select
                            value={ca.maGiaoVienThucTe ?? ''}
                            disabled={dangBan}
                            onChange={(e) => void doiGiaoVien(ca, e.target.value)}
                            className="h-8 max-w-52 rounded-lg border border-slate-200 bg-white px-2 text-xs font-semibold"
                          >
                            <option value="">— Chưa gán —</option>
                            {giaoViens.map((gv) => (
                              <option key={gv.maGiaoVien} value={gv.maGiaoVien}>
                                {gv.hoTen} ({gv.maGiaoVien})
                              </option>
                            ))}
                          </select>
                        ) : (
                          <span className="text-xs text-slate-600">
                            {ca.tenGiaoVienThucTe ?? '—'}
                          </span>
                        )}
                      </td>
                      <td className="px-3 py-2">{ca.tenTroGiangThucTe ?? '—'}</td>
                      <td className="px-3 py-2">
                        <select
                          value={ca.loaiBuoi}
                          disabled={dangBan}
                          onChange={(e) => void doiLoaiBuoi(ca, e.target.value)}
                          className="h-8 rounded-lg border border-slate-200 bg-white px-2 text-xs font-semibold"
                        >
                          {LOAI_BUOI.map((l) => (
                            <option key={l} value={l}>
                              {l}
                            </option>
                          ))}
                        </select>
                      </td>
                      <td className="px-3 py-2">
                        <select
                          value={ca.trangThai}
                          disabled={dangBan}
                          onChange={(e) => void doiTrangThai(ca, e.target.value)}
                          className="h-8 rounded-lg border border-slate-200 bg-white px-2 text-xs font-semibold"
                        >
                          {TRANG_THAI_CA_HOC.map((t) => (
                            <option key={t} value={t}>
                              {t}
                            </option>
                          ))}
                        </select>
                      </td>
                      <td className="px-3 py-2 text-right text-slate-600">{ca.soDiemDanh}</td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      ) : tab === 'phanCong' ? (
        <div className="space-y-4">
          <p className="rounded-xl border border-amber-200 bg-amber-50 px-4 py-3 text-sm text-amber-800">
            Quy ước lớp 2 buổi/tuần: <strong>buổi đầu tuần (thứ nhỏ hơn) do GV nước ngoài</strong> dạy,
            buổi còn lại do GV chính. Thêm vai trò "Giáo viên nước ngoài" ở màn Phân công giảng dạy,
            sau đó bấm "Đồng bộ giáo viên" để áp dụng cho ca đã sinh.
          </p>

          {phanCongs.length === 0 ? (
            <TrongRong>Lớp chưa có phân công giảng dạy.</TrongRong>
          ) : (
            <div className="overflow-x-auto rounded-xl border border-slate-200">
              <table className="w-full min-w-max text-sm">
                <thead className="bg-slate-50 text-left text-xs font-bold uppercase tracking-wider text-slate-500">
                  <tr>
                    <th className="px-3 py-2">Vai trò</th>
                    <th className="px-3 py-2">Giáo viên</th>
                    <th className="px-3 py-2">Mã</th>
                    <th className="px-3 py-2">Từ ngày</th>
                    <th className="px-3 py-2">Đến ngày</th>
                    <th className="px-3 py-2">Trạng thái</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-100">
                  {phanCongs.map((pc) => (
                    <tr key={pc.maPhanCong}>
                      <td className="px-3 py-2">
                        <Nhan tone={pc.vaiTroPhanCong === 'Giáo viên nước ngoài' ? 'blue' : 'slate'}>
                          {pc.vaiTroPhanCong}
                        </Nhan>
                      </td>
                      <td className="px-3 py-2 font-semibold text-slate-800">
                        {pc.tenGiaoVien ?? '— chưa gắn người —'}
                      </td>
                      <td className="px-3 py-2 font-mono text-xs">{pc.maGiaoVien ?? '—'}</td>
                      <td className="px-3 py-2">{dinhDangNgay(pc.ngayBatDau)}</td>
                      <td className="px-3 py-2">{pc.ngayKetThuc ? dinhDangNgay(pc.ngayKetThuc) : '—'}</td>
                      <td className="px-3 py-2">
                        <Nhan>{pc.trangThai}</Nhan>
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      ) : (
        <div className="space-y-4">
          {quaSoBuoiNhanHocVien && (
            <p className="rounded-xl border border-rose-200 bg-rose-50 px-4 py-3 text-sm text-rose-800">
              Lớp đã diễn ra {tienDo?.soBuoiDaHoc}/{tienDo?.tongSoBuoi} buổi — quá{' '}
              {SO_BUOI_TOI_DA_NHAN_HV} buổi nên <strong>không nhận thêm học viên</strong>.
            </p>
          )}

          {khongXemDuocDangKy ? (
            <TrongRong>
              Không tải được danh sách học viên của lớp (cần quyền Quản trị viên / Tư vấn).
            </TrongRong>
          ) : dangKys.length === 0 ? (
            <TrongRong>Lớp chưa có học viên.</TrongRong>
          ) : (
            <div className="overflow-x-auto rounded-xl border border-slate-200">
              <table className="w-full min-w-max text-sm">
                <thead className="bg-slate-50 text-left text-xs font-bold uppercase tracking-wider text-slate-500">
                  <tr>
                    <th className="px-3 py-2">Mã HV</th>
                    <th className="px-3 py-2">Họ tên</th>
                    <th className="px-3 py-2">Tuổi</th>
                    <th className="px-3 py-2">Điện thoại</th>
                    <th className="px-3 py-2">Trạng thái</th>
                    <th className="px-3 py-2">Ngày xếp lớp</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-slate-100">
                  {dangKys.map((dk) => (
                    <tr key={dk.maDangKy}>
                      <td className="px-3 py-2 font-mono text-xs">{dk.maHocVien}</td>
                      <td className="px-3 py-2 font-semibold text-slate-800">{dk.tenHocVien}</td>
                      <td className="px-3 py-2">{tuoiTuNgaySinh(dk.ngaySinh) ?? '—'}</td>
                      <td className="px-3 py-2">{dk.soDienThoai ?? '—'}</td>
                      <td className="px-3 py-2">
                        <Nhan>{dk.trangThaiDangKy}</Nhan>
                      </td>
                      <td className="px-3 py-2">
                        {dk.ngayXepLop ? dinhDangNgay(dk.ngayXepLop) : '—'}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>
          )}
        </div>
      )}
    </HopThoai>
  )
}
