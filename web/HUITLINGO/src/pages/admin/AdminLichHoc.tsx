// Admin → Lịch & ca học toàn trung tâm: xem theo tuần (lưới) và danh sách ca trong tuần.
import { useEffect, useMemo, useRef, useState } from 'react'
import { Icon } from '../../components/BieuTuong'
import { ThoiKhoaBieuTuan, dungLichTuan } from '../../components/ThoiKhoaBieuTuan'
import { Bang, LOP_INPUT, LOP_NHAN, Nhan, TrangTieuDe, type CotBang } from '../../components/ui'
import { caHocApi, lopHocApi } from '../../lib/api/endpoints'
import type { ClassListItem, SessionListItem } from '../../lib/api/types'
import { LOAI_BUOI, tenThu } from '../../lib/constants'
import { dinhDangKhoangGio, dinhDangNgay } from '../../lib/format'
import { dungDuLieu } from '../../lib/useApi'

export function AdminLichHoc() {
  const [maLopHoc, setMaLopHoc] = useState('')
  const [loaiBuoi, setLoaiBuoi] = useState('')

  const dsLop = dungDuLieu(async () => {
    const kq = await lopHocApi.danhSach({ page: 1, pageSize: 200 })
    return kq.items as ClassListItem[]
  })

  const lich = dungLichTuan((q) =>
    caHocApi
      .danhSach({
        ...q,
        maLopHoc: maLopHoc || undefined,
        page: 1,
        pageSize: 500,
      })
      .then((r) => r.items),
  )

  const taiLaiLich = lich.taiLai
  /** Bỏ qua lần render đầu (hook đã tự tải tuần hiện tại). */
  const lanDau = useRef(true)

  useEffect(() => {
    if (lanDau.current) {
      lanDau.current = false
      return
    }

    taiLaiLich()
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [maLopHoc])

  const caTrongTuan = useMemo(() => {
    const ds = loaiBuoi
      ? lich.duLieu.filter((c) => c.loaiBuoi === loaiBuoi)
      : lich.duLieu

    return [...ds].sort((a, b) =>
      `${a.ngayHoc}${a.gioBatDau}`.localeCompare(`${b.ngayHoc}${b.gioBatDau}`),
    )
  }, [lich.duLieu, loaiBuoi])

  const cot: CotBang<SessionListItem>[] = [
    {
      tieuDe: 'Ngày',
      khoa: 'ngayHoc',
      render: (r) => (
        <div>
          <p className="font-semibold text-slate-800">{dinhDangNgay(r.ngayHoc)}</p>
          <p className="text-xs text-slate-500">{tenThu(soThu(r.ngayHoc))}</p>
        </div>
      ),
    },
    {
      tieuDe: 'Giờ',
      khoa: 'gioBatDau',
      render: (r) => (
        <span className="whitespace-nowrap text-xs">
          {dinhDangKhoangGio(r.gioBatDau, r.gioKetThuc)}
        </span>
      ),
    },
    {
      tieuDe: 'Lớp',
      khoa: 'tenLop',
      render: (r) => (
        <div>
          <p className="font-semibold text-slate-900">{r.tenLop}</p>
          <p className="font-mono text-xs text-slate-500">{r.maLop}</p>
        </div>
      ),
    },
    { tieuDe: 'Phòng', khoa: 'tenPhong', render: (r) => r.tenPhong },
    {
      tieuDe: 'Giáo viên',
      khoa: 'tenGiaoVienThucTe',
      render: (r) => (
        <div className="text-xs">
          <p className="font-semibold text-slate-700">{r.tenGiaoVienThucTe ?? '—'}</p>
          {r.tenTroGiangThucTe && (
            <p className="text-slate-500">TG: {r.tenTroGiangThucTe}</p>
          )}
        </div>
      ),
    },
    {
      tieuDe: 'Buổi',
      khoa: 'soBuoi',
      render: (r) =>
        r.loaiBuoi === 'Buổi thi' ? (
          <span className="rounded-full bg-violet-100 px-2 py-0.5 text-xs font-bold text-violet-700">
            Buổi thi {r.soBuoi ? `· ${r.soBuoi}` : ''}
          </span>
        ) : (
          <span className="text-xs text-slate-600">Buổi {r.soBuoi ?? '—'}</span>
        ),
    },
    {
      tieuDe: 'Trạng thái',
      khoa: 'trangThai',
      render: (r) => <Nhan>{r.trangThai}</Nhan>,
    },
    {
      tieuDe: 'Điểm danh',
      khoa: 'soDiemDanh',
      canPhai: true,
      render: (r) => <span className="font-semibold text-slate-700">{r.soDiemDanh}</span>,
    },
  ]

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Lịch học"
        title="Lịch & ca học"
        desc="Toàn bộ ca học thực tế của trung tâm theo tuần. Bấm vào một ca để xem chi tiết ở danh sách bên dưới."
      />

      <section className="flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm sm:flex-row sm:items-end">
        <div className="flex-1">
          <label className={LOP_NHAN} htmlFor="lich-lop">
            Lọc theo lớp
          </label>
          <select
            id="lich-lop"
            value={maLopHoc}
            onChange={(e) => setMaLopHoc(e.target.value)}
            className={`${LOP_INPUT} h-10`}
          >
            <option value="">Tất cả lớp</option>
            {(dsLop.duLieu ?? []).map((l) => (
              <option key={l.maLopHoc} value={l.maLopHoc}>
                {l.maLop} · {l.tenLop}
              </option>
            ))}
          </select>
        </div>

        <div className="sm:w-56">
          <label className={LOP_NHAN} htmlFor="lich-loai">
            Loại buổi
          </label>
          <select
            id="lich-loai"
            value={loaiBuoi}
            onChange={(e) => setLoaiBuoi(e.target.value)}
            className={`${LOP_INPUT} h-10`}
          >
            <option value="">Tất cả</option>
            {LOAI_BUOI.map((l) => (
              <option key={l} value={l}>
                {l}
              </option>
            ))}
          </select>
        </div>
      </section>

      <ThoiKhoaBieuTuan
        duLieu={lich.duLieu}
        tuanBatDau={lich.tuanBatDau}
        tuanKetThuc={lich.tuanKetThuc}
        onDoiTuan={lich.doiTuan}
        dangTai={lich.dangTai}
        loi={lich.loi}
        onThuLai={lich.taiLai}
        dongPhu={(ca) => [
          `${ca.tenLop} · ${ca.tenPhong}`,
          ca.tenGiaoVienThucTe ? `GV: ${ca.tenGiaoVienThucTe}` : null,
          ca.soBuoi ? `Buổi ${ca.soBuoi}` : null,
        ]}
        moTaRong="Tuần này chưa có ca học nào."
        nhanDem="ca học"
      />

      <section className="space-y-3">
        <div className="flex items-center gap-2">
          <Icon name="calendarRange" className="size-4 text-slate-500" />
          <h2 className="text-sm font-bold uppercase tracking-wider text-slate-500">
            Danh sách ca trong tuần ({caTrongTuan.length})
          </h2>
        </div>

        <Bang
          cot={cot}
          duLieu={caTrongTuan}
          khoa={(r) => r.maCaHoc}
          dangTai={lich.dangTai}
          thongBaoRong="Chưa có ca học trong tuần này."
        />
      </section>
    </div>
  )
}

/** "yyyy-MM-dd" → 1..7 (Thứ Hai = 1). */
function soThu(ngayIso: string): number {
  const ngay = new Date(`${ngayIso}T00:00:00`)
  if (Number.isNaN(ngay.getTime())) return 0
  const dow = ngay.getDay()
  return dow === 0 ? 7 : dow
}
