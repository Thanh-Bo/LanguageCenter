// Khu vực giáo viên / trợ giảng.
import { useCallback, useEffect, useState } from 'react'
import { Icon } from '../../components/BieuTuong'
import {
  Bang,
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
  type CotBang,
} from '../../components/ui'
import { dungToast } from '../../components/ui/toast'
import { ThoiKhoaBieuTuan, dungLichTuan } from '../../components/ThoiKhoaBieuTuan'
import { giaoVienApi, thongBaoApi } from '../../lib/api/endpoints'
import type {
  AttendanceSheet,
  NotificationItem,
  PagedResult,
  SessionListItem,
  TeacherClass,
  TeacherClassStudent,
} from '../../lib/api/types'
import { TRANG_THAI_DIEM_DANH } from '../../lib/constants'
import {
  dinhDangKhoangGio,
  dinhDangNgay,
  dinhDangNgayGio,
  homNayIso,
  ngayTruocIso,
} from '../../lib/format'
import { dungDuLieu, thongDiepLoi } from '../../lib/useApi'

// ============================================================
// TỔNG QUAN
// ============================================================
export function GVTongQuan() {
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
        <h2 className="mb-4 text-sm font-bold uppercase tracking-wider text-slate-500">
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
  const [dangChon, setDangChon] = useState<TeacherClass | null>(null)

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
                <div>
                  <p className="font-mono text-xs text-slate-500">{l.maLop}</p>
                  <h3 className="text-lg font-bold text-slate-900">{l.tenLop}</h3>
                  <p className="text-sm text-slate-500">{l.tenKhoaHoc}</p>
                </div>
                <Nhan>{l.trangThai}</Nhan>
              </div>

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

              <button
                type="button"
                onClick={() => setDangChon(l)}
                className={`${LOP_NUT_PHU} mt-4 w-full`}
              >
                Xem học viên
              </button>
            </article>
          ))}
        </div>
      )}

      {dangChon && <DanhSachHocVien lop={dangChon} onDong={() => setDangChon(null)} />}
    </div>
  )
}

function DanhSachHocVien({ lop, onDong }: { lop: TeacherClass; onDong: () => void }) {
  const { duLieu, dangTai, loi, taiLai } = dungDuLieu(() => giaoVienApi.hocVienCuaLop(lop.maLopHoc))

  const cot: CotBang<TeacherClassStudent>[] = [
    { tieuDe: 'Mã HV', khoa: 'maHocVien' },
    {
      tieuDe: 'Họ tên',
      khoa: 'hoTen',
      render: (r) => <span className="font-semibold text-slate-900">{r.hoTen}</span>,
    },
    { tieuDe: 'Mã đăng ký', khoa: 'maDangKy', canPhai: true },
  ]

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-slate-950/50 p-4 backdrop-blur-sm">
      <div className="w-full max-w-2xl overflow-hidden rounded-2xl bg-white shadow-2xl">
        <div className="flex items-center justify-between border-b border-slate-100 p-5">
          <div>
            <h2 className="text-lg font-bold text-slate-900">{lop.tenLop}</h2>
            <p className="text-sm text-slate-500">
              {lop.maLop} · {lop.siSo} học viên
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
          {loi ? (
            <LoiTrang message={loi} onThuLai={taiLai} />
          ) : (
            <Bang
              cot={cot}
              duLieu={duLieu ?? []}
              khoa={(r) => r.maDangKy}
              dangTai={dangTai}
              thongBaoRong="Lớp chưa có học viên."
            />
          )}
        </div>
      </div>
    </div>
  )
}

// ============================================================
// LỊCH DẠY
// ============================================================
export function GVLichDay() {
  const lich = dungLichTuan((q) => giaoVienApi.lichDay(q))

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Giảng dạy"
        title="Lịch dạy"
        desc="Thời khóa biểu theo tuần của các lớp bạn phụ trách."
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
      />
    </div>
  )
}

// ============================================================
// ĐIỂM DANH
// ============================================================
export function GVDiemDanh() {
  const { hien } = dungToast()

  const [tuNgay, setTuNgay] = useState(ngayTruocIso(7))
  const [denNgay, setDenNgay] = useState(homNayIso())
  const [maCaHoc, setMaCaHoc] = useState<number | null>(null)

  const dsCa = dungDuLieu(() => giaoVienApi.lichDay({ tuNgay, denNgay }))
  const phieu = dungDuLieu<AttendanceSheet | null>(async () =>
    maCaHoc ? giaoVienApi.bangDiemDanh(maCaHoc) : null,
  )

  const [trangThai, setTrangThai] = useState<Record<string, string>>({})
  const [ghiChu, setGhiChu] = useState<Record<string, string>>({})
  const [dangLuu, setDangLuu] = useState(false)

  // Nạp lại trạng thái mỗi khi mở một ca học khác
  useEffect(() => {
    const sheet = phieu.duLieu
    if (!sheet) return

    const moi: Record<string, string> = {}
    for (const hv of sheet.hocViens) {
      moi[hv.maHocVien] = hv.trangThaiDiemDanh ?? TRANG_THAI_DIEM_DANH[0]
    }
    setTrangThai(moi)
    setGhiChu({})
  }, [phieu.duLieu])

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

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Giảng dạy"
        title="Điểm danh"
        desc="Chọn ca học để điểm danh học viên trong lớp."
      />

      <div className="flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm sm:flex-row sm:items-end">
        <div className="flex-1">
          <label className={LOP_NHAN} htmlFor="dd-tu">
            Từ ngày
          </label>
          <input
            id="dd-tu"
            type="date"
            value={tuNgay}
            onChange={(e) => setTuNgay(e.target.value)}
            className={LOP_INPUT}
          />
        </div>
        <div className="flex-1">
          <label className={LOP_NHAN} htmlFor="dd-den">
            Đến ngày
          </label>
          <input
            id="dd-den"
            type="date"
            value={denNgay}
            onChange={(e) => setDenNgay(e.target.value)}
            className={LOP_INPUT}
          />
        </div>
        <div className="flex-2">
          <label className={LOP_NHAN} htmlFor="dd-ca">
            Ca học
          </label>
          <select
            id="dd-ca"
            value={maCaHoc ?? ''}
            onChange={(e) => setMaCaHoc(e.target.value ? Number(e.target.value) : null)}
            className={LOP_INPUT}
          >
            <option value="">— Chọn ca học —</option>
            {(dsCa.duLieu ?? []).map((c) => (
              <option key={c.maCaHoc} value={c.maCaHoc}>
                {dinhDangNgay(c.ngayHoc)} · {dinhDangKhoangGio(c.gioBatDau, c.gioKetThuc)} ·{' '}
                {c.maLop}
              </option>
            ))}
          </select>
        </div>
      </div>

      {dsCa.loi && <LoiTrang message={dsCa.loi} onThuLai={dsCa.taiLai} />}

      {maCaHoc && (
        <>
          {phieu.dangTai && <DangTai nhan="Đang tải phiếu điểm danh…" />}
          {phieu.loi && <LoiTrang message={phieu.loi} onThuLai={phieu.taiLai} />}

          {sheet && (
            <div className="space-y-4">
              <div className="flex flex-wrap items-center justify-between gap-3 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
                <div>
                  <p className="font-bold text-slate-900">
                    {sheet.maLop} · {sheet.tenLop}
                  </p>
                  <p className="text-xs text-slate-500">
                    {dinhDangNgay(sheet.ngayHoc)} ·{' '}
                    {dinhDangKhoangGio(sheet.gioBatDau, sheet.gioKetThuc)} · {sheet.tenPhong}
                  </p>
                </div>
                <div className="flex items-center gap-2">
                  <Nhan>{sheet.trangThaiCaHoc}</Nhan>
                  <button
                    type="button"
                    onClick={luu}
                    disabled={dangLuu || !sheet.coTheDiemDanh}
                    className={LOP_NUT_CHINH}
                  >
                    {dangLuu ? 'Đang lưu…' : 'Lưu điểm danh'}
                  </button>
                </div>
              </div>

              {!sheet.coTheDiemDanh && (
                <p className="rounded-xl bg-amber-50 p-3 text-sm font-semibold text-amber-700">
                  {sheet.lyDoKhongTheDiemDanh ?? 'Ca học này không thể điểm danh.'}
                </p>
              )}

              <div className="overflow-x-auto rounded-2xl border border-slate-200 bg-white shadow-sm">
                <table className="w-full min-w-max border-collapse text-sm">
                  <thead>
                    <tr className="border-b border-slate-100 bg-slate-50/60 text-left">
                      <th className="px-5 py-3 text-xs font-bold uppercase tracking-wider text-slate-500">
                        Học viên
                      </th>
                      <th className="px-3 py-3 text-xs font-bold uppercase tracking-wider text-slate-500">
                        Trạng thái
                      </th>
                      <th className="px-5 py-3 text-xs font-bold uppercase tracking-wider text-slate-500">
                        Ghi chú
                      </th>
                    </tr>
                  </thead>
                  <tbody className="divide-y divide-slate-100">
                    {sheet.hocViens.map((hv) => (
                      <tr key={hv.maHocVien}>
                        <td className="px-5 py-3">
                          <p className="font-semibold text-slate-900">{hv.hoTen}</p>
                          <p className="font-mono text-xs text-slate-500">{hv.maHocVien}</p>
                        </td>
                        <td className="px-3 py-3">
                          <select
                            value={trangThai[hv.maHocVien] ?? TRANG_THAI_DIEM_DANH[0]}
                            onChange={(e) =>
                              setTrangThai((cu) => ({ ...cu, [hv.maHocVien]: e.target.value }))
                            }
                            disabled={!sheet.coTheDiemDanh}
                            className="h-9 rounded-lg border border-slate-200 px-2 text-sm disabled:opacity-50"
                          >
                            {TRANG_THAI_DIEM_DANH.map((t) => (
                              <option key={t} value={t}>
                                {t}
                              </option>
                            ))}
                          </select>
                        </td>
                        <td className="px-5 py-3">
                          <input
                            value={ghiChu[hv.maHocVien] ?? ''}
                            onChange={(e) =>
                              setGhiChu((cu) => ({ ...cu, [hv.maHocVien]: e.target.value }))
                            }
                            disabled={!sheet.coTheDiemDanh}
                            placeholder="Không bắt buộc"
                            className="h-9 w-full min-w-48 rounded-lg border border-slate-200 px-3 text-sm disabled:opacity-50"
                          />
                        </td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>
            </div>
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

  const bang = dungDuLieu(() =>
    maLopHoc ? giaoVienApi.bangDiemLop(maLopHoc) : Promise.resolve(null),
  )

  const duLieu = bang.duLieu

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Giảng dạy"
        title="Bảng điểm"
        desc="Điểm tổng kết được tính tự động theo trọng số từng thành phần."
      />

      <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
        <label className={LOP_NHAN} htmlFor="bd-lop">
          Chọn lớp
        </label>
        <select
          id="bd-lop"
          value={maLopHoc ?? ''}
          onChange={(e) => setMaLopHoc(e.target.value ? Number(e.target.value) : null)}
          className={`${LOP_INPUT} sm:max-w-md`}
        >
          <option value="">— Chọn lớp —</option>
          {(dsLop.duLieu ?? []).map((l) => (
            <option key={l.maLopHoc} value={l.maLopHoc}>
              {l.maLop} · {l.tenLop}
            </option>
          ))}
        </select>
      </div>

      {!maLopHoc && (
        <p className="rounded-2xl border border-dashed border-slate-300 bg-white p-10 text-center text-slate-400">
          Vui lòng chọn lớp để xem bảng điểm.
        </p>
      )}

      {bang.dangTai && maLopHoc && <DangTai />}
      {bang.loi && <LoiTrang message={bang.loi} onThuLai={bang.taiLai} />}

      {duLieu && (
        <div className="space-y-4">
          <div className="grid gap-4 sm:grid-cols-3">
            <TheChiSo nhan="Sĩ số" giaTri={duLieu.soHocVien} icon="users" />
            <TheChiSo
              nhan="Đủ điều kiện"
              giaTri={duLieu.soDuDieuKien}
              icon="check"
              tone="emerald"
            />
            <TheChiSo
              nhan="Điểm trung bình lớp"
              giaTri={duLieu.diemTongKetTrungBinh?.toFixed(2) ?? '—'}
              icon="chart"
              tone="amber"
            />
          </div>

          <div className="overflow-x-auto rounded-2xl border border-slate-200 bg-white shadow-sm">
            <table className="w-full min-w-max border-collapse text-sm">
              <thead>
                <tr className="border-b border-slate-100 bg-slate-50/60">
                  <th className="px-5 py-3 text-left text-xs font-bold uppercase tracking-wider text-slate-500">
                    Học viên
                  </th>
                  {duLieu.thanhPhan.map((tp) => (
                    <th
                      key={tp.maLoaiDiem}
                      className="px-3 py-3 text-right text-xs font-bold uppercase text-slate-500"
                    >
                      {tp.tenLoaiDiem}
                      <span className="block font-normal normal-case text-slate-400">
                        {tp.trongSo}%
                      </span>
                    </th>
                  ))}
                  <th className="px-5 py-3 text-right text-xs font-bold uppercase text-slate-500">
                    Tổng kết
                  </th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {duLieu.hocViens.map((hv) => (
                  <tr key={hv.maDangKy} className="hover:bg-slate-50/70">
                    <td className="px-5 py-3">
                      <p className="font-semibold text-slate-900">{hv.hoTen}</p>
                      <p className="font-mono text-xs text-slate-500">{hv.maHocVien}</p>
                    </td>
                    {duLieu.thanhPhan.map((tp) => {
                      const diem = hv.thanhPhan.find((x) => x.maLoaiDiem === tp.maLoaiDiem)
                      return (
                        <td key={tp.maLoaiDiem} className="px-3 py-3 text-right">
                          {diem?.diem !== null && diem?.diem !== undefined
                            ? diem.diem.toFixed(2)
                            : '—'}
                        </td>
                      )
                    })}
                    <td className="px-5 py-3 text-right font-bold">
                      {hv.diemTongKet !== null && hv.diemTongKet !== undefined
                        ? hv.diemTongKet.toFixed(2)
                        : '—'}
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>
      )}
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
