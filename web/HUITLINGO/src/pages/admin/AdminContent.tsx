// Trang quản trị: Thông báo, Khảo sát, Báo cáo.
import { useCallback, useEffect, useState } from 'react'
import { Icon } from '../../components/BieuTuong'
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
import {
  baoCaoApi,
  hocPhiApi,
  khaoSatApi,
  lopHocApi,
  thongBaoApi,
} from '../../lib/api/endpoints'
import type {
  ClassListItem,
  NotificationItem,
  PagedResult,
  SurveyItem,
  SurveyStatistics,
} from '../../lib/api/types'
import {
  LOAI_THONG_BAO,
  TRANG_THAI_KHAO_SAT,
} from '../../lib/constants'
import { dinhDangNgay, dinhDangNgayGio, dinhDangSo, dinhDangTien, homNayIso } from '../../lib/format'
import { dungDuLieu, thongDiepLoi } from '../../lib/useApi'

const PAGE_SIZE = 20

/** Danh sách lớp dùng cho các ô chọn lớp. */
function dungDanhSachLop() {
  const { duLieu } = dungDuLieu(async () => {
    const kq = await lopHocApi.danhSach({ page: 1, pageSize: 200 })
    return kq.items
  })

  return duLieu ?? []
}

// ============================================================
// THÔNG BÁO
// ============================================================
export function AdminThongBao() {
  const { hien } = dungToast()
  const dsLop = dungDanhSachLop()

  const [ketQua, setKetQua] = useState<PagedResult<NotificationItem> | null>(null)
  const [dangTai, setDangTai] = useState(true)
  const [loi, setLoi] = useState<string | null>(null)
  const [page, setPage] = useState(1)
  const [moGui, setMoGui] = useState(false)

  const tai = useCallback(async () => {
    setDangTai(true)
    setLoi(null)
    try {
      setKetQua(await thongBaoApi.quanTri({ page, pageSize: PAGE_SIZE }))
    } catch (error) {
      setLoi(thongDiepLoi(error))
    } finally {
      setDangTai(false)
    }
  }, [page])

  useEffect(() => {
    void tai()
  }, [tai])

  const xoa = async (maThongBao: number) => {
    try {
      await thongBaoApi.xoa(maThongBao)
      hien('Đã thu hồi thông báo.', 'thanhCong')
      void tai()
    } catch (error) {
      hien(thongDiepLoi(error), 'loi')
    }
  }

  const cot: CotBang<NotificationItem>[] = [
    {
      tieuDe: 'Tiêu đề',
      khoa: 'tieuDe',
      render: (r) => (
        <div className="max-w-md">
          <p className="font-semibold text-slate-900">{r.tieuDe}</p>
          <p className="line-clamp-2 text-xs text-slate-500">{r.noiDung}</p>
        </div>
      ),
    },
    { tieuDe: 'Loại', khoa: 'loaiThongBao', render: (r) => r.loaiThongBao ?? '—' },
    {
      tieuDe: 'Phạm vi',
      khoa: 'phamVi',
      render: (r) => (r.maLop ? `Lớp ${r.maLop}` : r.tenDangNhapNguoiNhan ?? 'Cá nhân'),
    },
    { tieuDe: 'Người gửi', khoa: 'tenDangNhapNguoiGui' },
    { tieuDe: 'Thời gian', khoa: 'thoiGianGui', render: (r) => dinhDangNgayGio(r.thoiGianGui) },
    { tieuDe: 'Trạng thái', khoa: 'trangThai', render: (r) => <Nhan>{r.trangThai}</Nhan> },
    {
      tieuDe: '',
      khoa: 'thaoTac',
      canPhai: true,
      render: (r) => (
        <button
          type="button"
          onClick={() => void xoa(r.maThongBao)}
          className="rounded-lg border border-rose-200 px-3 py-1.5 text-xs font-bold text-rose-600 transition hover:bg-rose-50"
        >
          Thu hồi
        </button>
      ),
    },
  ]

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Truyền thông"
        title="Thông báo"
        desc="Gửi thông báo tới một lớp hoặc một tài khoản cụ thể."
        actions={
          <button type="button" onClick={() => setMoGui(true)} className={LOP_NUT_CHINH}>
            <Icon name="bell" className="size-4" /> Gửi thông báo
          </button>
        }
      />

      {loi ? (
        <LoiTrang message={loi} onThuLai={tai} />
      ) : (
        <>
          <Bang
            cot={cot}
            duLieu={ketQua?.items ?? []}
            khoa={(r) => r.maThongBao}
            dangTai={dangTai}
            thongBaoRong="Chưa có thông báo nào."
          />
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

      <HopThoaiGuiThongBao
        mo={moGui}
        dsLop={dsLop}
        onDong={() => setMoGui(false)}
        onXong={() => {
          setMoGui(false)
          hien('Đã gửi thông báo.', 'thanhCong')
          void tai()
        }}
      />
    </div>
  )
}

function HopThoaiGuiThongBao({
  mo,
  dsLop,
  onDong,
  onXong,
}: {
  mo: boolean
  dsLop: ClassListItem[]
  onDong: () => void
  onXong: () => void
}) {
  const [phamVi, setPhamVi] = useState<'lop' | 'taiKhoan'>('lop')
  const [maLopHoc, setMaLopHoc] = useState('')
  const [maTaiKhoan, setMaTaiKhoan] = useState('')
  const [tieuDe, setTieuDe] = useState('')
  const [noiDung, setNoiDung] = useState('')
  const [loai, setLoai] = useState<string>(LOAI_THONG_BAO[0])
  const [dangGui, setDangGui] = useState(false)
  const [loi, setLoi] = useState<string | null>(null)

  const gui = async () => {
    if (!tieuDe.trim() || !noiDung.trim()) {
      setLoi('Vui lòng nhập tiêu đề và nội dung.')
      return
    }
    if (phamVi === 'lop' && !maLopHoc) {
      setLoi('Vui lòng chọn lớp nhận thông báo.')
      return
    }
    if (phamVi === 'taiKhoan' && !maTaiKhoan) {
      setLoi('Vui lòng nhập mã tài khoản nhận.')
      return
    }

    setDangGui(true)
    setLoi(null)

    try {
      await thongBaoApi.gui({
        maLopHoc: phamVi === 'lop' ? Number(maLopHoc) : null,
        maTaiKhoanNhan: phamVi === 'taiKhoan' ? Number(maTaiKhoan) : null,
        tieuDe: tieuDe.trim(),
        noiDung: noiDung.trim(),
        loaiThongBao: loai,
      })
      setTieuDe('')
      setNoiDung('')
      setMaLopHoc('')
      setMaTaiKhoan('')
      onXong()
    } catch (error) {
      setLoi(thongDiepLoi(error))
    } finally {
      setDangGui(false)
    }
  }

  return (
    <HopThoai
      mo={mo}
      tieuDe="Gửi thông báo"
      onDong={onDong}
      footer={
        <>
          <button type="button" onClick={onDong} className={LOP_NUT_PHU}>
            Hủy
          </button>
          <button type="button" onClick={gui} disabled={dangGui} className={LOP_NUT_CHINH}>
            {dangGui ? 'Đang gửi…' : 'Gửi'}
          </button>
        </>
      }
    >
      <div className="space-y-4">
        <div>
          <span className={LOP_NHAN}>Người nhận</span>
          <div className="flex gap-2">
            {(
              [
                ['lop', 'Theo lớp'],
                ['taiKhoan', 'Theo tài khoản'],
              ] as const
            ).map(([giaTri, nhan]) => (
              <button
                key={giaTri}
                type="button"
                onClick={() => setPhamVi(giaTri)}
                className={`flex-1 rounded-xl border px-4 py-2.5 text-sm font-semibold transition ${
                  phamVi === giaTri
                    ? 'border-blue-500 bg-blue-50 text-blue-700'
                    : 'border-slate-200 text-slate-600'
                }`}
              >
                {nhan}
              </button>
            ))}
          </div>
        </div>

        {phamVi === 'lop' ? (
          <div>
            <label className={LOP_NHAN} htmlFor="lop-nhan">
              Lớp nhận
            </label>
            <select
              id="lop-nhan"
              value={maLopHoc}
              onChange={(e) => setMaLopHoc(e.target.value)}
              className={LOP_INPUT}
            >
              <option value="">— Chọn lớp —</option>
              {dsLop.map((l) => (
                <option key={l.maLopHoc} value={l.maLopHoc}>
                  {l.maLop} · {l.tenLop}
                </option>
              ))}
            </select>
          </div>
        ) : (
          <div>
            <label className={LOP_NHAN} htmlFor="tai-khoan-nhan">
              Mã tài khoản nhận
            </label>
            <input
              id="tai-khoan-nhan"
              type="number"
              value={maTaiKhoan}
              onChange={(e) => setMaTaiKhoan(e.target.value)}
              placeholder="Ví dụ: 12"
              className={LOP_INPUT}
            />
          </div>
        )}

        <div>
          <label className={LOP_NHAN} htmlFor="loai-tb">
            Loại thông báo
          </label>
          <select
            id="loai-tb"
            value={loai}
            onChange={(e) => setLoai(e.target.value)}
            className={LOP_INPUT}
          >
            {LOAI_THONG_BAO.map((l) => (
              <option key={l} value={l}>
                {l}
              </option>
            ))}
          </select>
        </div>

        <div>
          <label className={LOP_NHAN} htmlFor="tieu-de-tb">
            Tiêu đề
          </label>
          <input
            id="tieu-de-tb"
            value={tieuDe}
            onChange={(e) => setTieuDe(e.target.value)}
            className={LOP_INPUT}
            maxLength={200}
          />
        </div>

        <div>
          <label className={LOP_NHAN} htmlFor="noi-dung-tb">
            Nội dung
          </label>
          <textarea
            id="noi-dung-tb"
            rows={4}
            value={noiDung}
            onChange={(e) => setNoiDung(e.target.value)}
            className="w-full rounded-xl border border-slate-200 bg-white p-3 text-sm outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-50"
            maxLength={2000}
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
// KHẢO SÁT
// ============================================================
export function AdminKhaoSat() {
  const { hien } = dungToast()

  const [ketQua, setKetQua] = useState<PagedResult<SurveyItem> | null>(null)
  const [dangTai, setDangTai] = useState(true)
  const [loi, setLoi] = useState<string | null>(null)
  const [page, setPage] = useState(1)
  const [moTao, setMoTao] = useState(false)
  const [thongKe, setThongKe] = useState<SurveyStatistics | null>(null)

  const tai = useCallback(async () => {
    setDangTai(true)
    setLoi(null)
    try {
      setKetQua(await khaoSatApi.danhSach({ page, pageSize: PAGE_SIZE }))
    } catch (error) {
      setLoi(thongDiepLoi(error))
    } finally {
      setDangTai(false)
    }
  }, [page])

  useEffect(() => {
    void tai()
  }, [tai])

  const doiTrangThai = async (maKhaoSat: number, trangThai: string) => {
    try {
      await khaoSatApi.doiTrangThai(maKhaoSat, trangThai)
      hien(`Đã chuyển trạng thái: ${trangThai}`, 'thanhCong')
      void tai()
    } catch (error) {
      hien(thongDiepLoi(error), 'loi')
    }
  }

  const xemThongKe = async (maKhaoSat: number) => {
    try {
      setThongKe(await khaoSatApi.thongKe(maKhaoSat))
    } catch (error) {
      hien(thongDiepLoi(error), 'loi')
    }
  }

  const cot: CotBang<SurveyItem>[] = [
    {
      tieuDe: 'Khảo sát',
      khoa: 'tenKhaoSat',
      render: (r) => (
        <div className="max-w-sm">
          <p className="font-semibold text-slate-900">{r.tenKhaoSat}</p>
          <p className="line-clamp-1 text-xs text-slate-500">{r.moTa ?? '—'}</p>
        </div>
      ),
    },
    {
      tieuDe: 'Thời gian',
      khoa: 'ngayBatDau',
      render: (r) =>
        `${dinhDangNgay(r.ngayBatDau)} → ${r.ngayKetThuc ? dinhDangNgay(r.ngayKetThuc) : 'không giới hạn'}`,
    },
    { tieuDe: 'Câu hỏi', khoa: 'soCauHoi', canPhai: true, render: (r) => dinhDangSo(r.soCauHoi) },
    { tieuDe: 'Phản hồi', khoa: 'soPhanHoi', canPhai: true, render: (r) => dinhDangSo(r.soPhanHoi) },
    {
      tieuDe: 'Trạng thái',
      khoa: 'trangThai',
      render: (r) => (
        <div className="flex flex-col items-start gap-1">
          <Nhan>{r.trangThai}</Nhan>
          {r.dangMo && <span className="text-xs font-bold text-emerald-600">Đang nhận</span>}
        </div>
      ),
    },
    {
      tieuDe: '',
      khoa: 'thaoTac',
      canPhai: true,
      render: (r) => (
        <div className="flex justify-end gap-2">
          <button
            type="button"
            onClick={() => void xemThongKe(r.maKhaoSat)}
            className="rounded-lg border border-slate-200 px-3 py-1.5 text-xs font-bold text-slate-600 transition hover:border-blue-200 hover:text-blue-600"
          >
            Thống kê
          </button>
          <select
            value={r.trangThai}
            onChange={(e) => void doiTrangThai(r.maKhaoSat, e.target.value)}
            className="rounded-lg border border-slate-200 px-2 py-1.5 text-xs font-semibold"
          >
            {TRANG_THAI_KHAO_SAT.map((t) => (
              <option key={t} value={t}>
                {t}
              </option>
            ))}
          </select>
        </div>
      ),
    },
  ]

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Truyền thông"
        title="Khảo sát"
        desc="Tạo khảo sát, theo dõi tỉ lệ phản hồi và xem thống kê điểm đánh giá."
        actions={
          <button type="button" onClick={() => setMoTao(true)} className={LOP_NUT_CHINH}>
            <Icon name="sparkles" className="size-4" /> Tạo khảo sát
          </button>
        }
      />

      {loi ? (
        <LoiTrang message={loi} onThuLai={tai} />
      ) : (
        <>
          <Bang
            cot={cot}
            duLieu={ketQua?.items ?? []}
            khoa={(r) => r.maKhaoSat}
            dangTai={dangTai}
            thongBaoRong="Chưa có khảo sát nào."
          />
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

      <HopThoaiTaoKhaoSat
        mo={moTao}
        onDong={() => setMoTao(false)}
        onXong={() => {
          setMoTao(false)
          hien('Đã tạo khảo sát.', 'thanhCong')
          void tai()
        }}
      />

      <HopThoai
        mo={!!thongKe}
        tieuDe={thongKe ? `Thống kê: ${thongKe.tenKhaoSat}` : 'Thống kê'}
        onDong={() => setThongKe(null)}
      >
        {thongKe && (
          <div className="space-y-5">
            <div className="grid grid-cols-3 gap-3 text-center">
              <div className="rounded-xl bg-slate-50 p-3">
                <p className="text-xs text-slate-500">Câu hỏi</p>
                <p className="text-lg font-bold">{thongKe.tongSoCauHoi}</p>
              </div>
              <div className="rounded-xl bg-slate-50 p-3">
                <p className="text-xs text-slate-500">Phản hồi</p>
                <p className="text-lg font-bold">{thongKe.tongSoPhanHoi}</p>
              </div>
              <div className="rounded-xl bg-slate-50 p-3">
                <p className="text-xs text-slate-500">Điểm TB</p>
                <p className="text-lg font-bold">
                  {thongKe.diemDanhGiaChungTrungBinh?.toFixed(2) ?? '—'}
                </p>
              </div>
            </div>

            {thongKe.cauHois.map((c) => (
              <div key={c.maCauHoi} className="rounded-xl border border-slate-200 p-4">
                <p className="text-sm font-semibold text-slate-900">
                  {c.thuTu}. {c.noiDungCauHoi}
                </p>
                <p className="mt-1 text-xs text-slate-500">
                  {c.loaiCauHoi} · {c.soLuotTraLoi} lượt
                  {c.giaTriTrungBinh !== null && c.giaTriTrungBinh !== undefined
                    ? ` · TB ${c.giaTriTrungBinh.toFixed(2)}`
                    : ''}
                </p>

                {c.phanBo.length > 0 && (
                  <div className="mt-3 space-y-1.5">
                    {c.phanBo.map((p) => {
                      const tong = c.phanBo.reduce((s, x) => s + x.soLuong, 0) || 1
                      return (
                        <div key={p.giaTri} className="flex items-center gap-2 text-xs">
                          <span className="inline-flex w-8 items-center gap-0.5 text-slate-500">{p.giaTri}<Icon name="star" className="size-3" /></span>
                          <span className="h-2 flex-1 overflow-hidden rounded-full bg-slate-100">
                            <span
                              className="block h-full rounded-full bg-blue-500"
                              style={{ width: `${(p.soLuong / tong) * 100}%` }}
                            />
                          </span>
                          <span className="w-8 text-right font-semibold">{p.soLuong}</span>
                        </div>
                      )
                    })}
                  </div>
                )}

                {c.noiDungTuLuan.length > 0 && (
                  <ul className="mt-3 space-y-1.5">
                    {c.noiDungTuLuan.map((t, i) => (
                      <li
                        key={i}
                        className="rounded-lg bg-slate-50 p-2.5 text-xs text-slate-600"
                      >
                        “{t}”
                      </li>
                    ))}
                  </ul>
                )}
              </div>
            ))}
          </div>
        )}
      </HopThoai>
    </div>
  )
}

function HopThoaiTaoKhaoSat({
  mo,
  onDong,
  onXong,
}: {
  mo: boolean
  onDong: () => void
  onXong: () => void
}) {
  const [ten, setTen] = useState('')
  const [moTa, setMoTa] = useState('')
  const [tuNgay, setTuNgay] = useState(homNayIso())
  const [denNgay, setDenNgay] = useState('')
  const [dangGui, setDangGui] = useState(false)
  const [loi, setLoi] = useState<string | null>(null)

  const gui = async () => {
    if (!ten.trim()) {
      setLoi('Vui lòng nhập tên khảo sát.')
      return
    }

    setDangGui(true)
    setLoi(null)

    try {
      await khaoSatApi.tao({
        tenKhaoSat: ten.trim(),
        moTa: moTa.trim() || null,
        ngayBatDau: tuNgay,
        ngayKetThuc: denNgay || null,
      })
      setTen('')
      setMoTa('')
      setDenNgay('')
      onXong()
    } catch (error) {
      setLoi(thongDiepLoi(error))
    } finally {
      setDangGui(false)
    }
  }

  return (
    <HopThoai
      mo={mo}
      tieuDe="Tạo khảo sát"
      onDong={onDong}
      footer={
        <>
          <button type="button" onClick={onDong} className={LOP_NUT_PHU}>
            Hủy
          </button>
          <button type="button" onClick={gui} disabled={dangGui} className={LOP_NUT_CHINH}>
            {dangGui ? 'Đang lưu…' : 'Tạo'}
          </button>
        </>
      }
    >
      <div className="space-y-4">
        <div>
          <label className={LOP_NHAN} htmlFor="ten-khaosat">
            Tên khảo sát
          </label>
          <input
            id="ten-khaosat"
            value={ten}
            onChange={(e) => setTen(e.target.value)}
            className={LOP_INPUT}
          />
        </div>
        <div>
          <label className={LOP_NHAN} htmlFor="mota-khaosat">
            Mô tả
          </label>
          <textarea
            id="mota-khaosat"
            rows={3}
            value={moTa}
            onChange={(e) => setMoTa(e.target.value)}
            className="w-full rounded-xl border border-slate-200 bg-white p-3 text-sm outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-50"
          />
        </div>
        <div className="grid grid-cols-2 gap-3">
          <div>
            <label className={LOP_NHAN} htmlFor="tu-ngay">
              Bắt đầu
            </label>
            <input
              id="tu-ngay"
              type="date"
              value={tuNgay}
              onChange={(e) => setTuNgay(e.target.value)}
              className={LOP_INPUT}
            />
          </div>
          <div>
            <label className={LOP_NHAN} htmlFor="den-ngay">
              Kết thúc
            </label>
            <input
              id="den-ngay"
              type="date"
              value={denNgay}
              onChange={(e) => setDenNgay(e.target.value)}
              className={LOP_INPUT}
            />
          </div>
        </div>
        {loi && (
          <p className="rounded-xl bg-rose-50 p-3 text-sm font-semibold text-rose-700">{loi}</p>
        )}
      </div>
    </HopThoai>
  )
}

// ============================================================
// BÁO CÁO
// ============================================================
type TabBaoCao = 'taiChinh' | 'hocTap' | 'diemDanh' | 'khaoSat'

export function AdminBaoCao() {
  const dsLop = dungDanhSachLop()
  const [tab, setTab] = useState<TabBaoCao>('taiChinh')
  const [maLopHoc, setMaLopHoc] = useState('')

  const tabNhan: Array<[TabBaoCao, string]> = [
    ['taiChinh', 'Học phí'],
    ['hocTap', 'Kết quả học tập'],
    ['diemDanh', 'Điểm danh'],
    ['khaoSat', 'Khảo sát'],
  ]

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Phân tích"
        title="Báo cáo"
        desc="Số liệu tổng hợp phục vụ điều hành và đánh giá chất lượng đào tạo."
      />

      <div className="flex flex-wrap gap-2">
        {tabNhan.map(([giaTri, nhan]) => (
          <button
            key={giaTri}
            type="button"
            onClick={() => setTab(giaTri)}
            className={`rounded-xl px-4 py-2.5 text-sm font-bold transition ${
              tab === giaTri
                ? 'bg-blue-600 text-white shadow-sm shadow-blue-200'
                : 'border border-slate-200 bg-white text-slate-600 hover:text-blue-600'
            }`}
          >
            {nhan}
          </button>
        ))}
      </div>

      {tab === 'taiChinh' && <BaoCaoTaiChinh />}
      {tab === 'khaoSat' && <BaoCaoKhaoSat />}

      {(tab === 'hocTap' || tab === 'diemDanh') && (
        <>
          <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
            <label className={LOP_NHAN} htmlFor="chon-lop">
              Chọn lớp cần xem báo cáo
            </label>
            <select
              id="chon-lop"
              value={maLopHoc}
              onChange={(e) => setMaLopHoc(e.target.value)}
              className={`${LOP_INPUT} sm:max-w-md`}
            >
              <option value="">— Chọn lớp —</option>
              {dsLop.map((l) => (
                <option key={l.maLopHoc} value={l.maLopHoc}>
                  {l.maLop} · {l.tenLop}
                </option>
              ))}
            </select>
          </div>

          {!maLopHoc && (
            <p className="rounded-2xl border border-dashed border-slate-300 bg-white p-10 text-center text-slate-400">
              Vui lòng chọn một lớp để xem báo cáo.
            </p>
          )}

          {maLopHoc && tab === 'hocTap' && <BangHocTap maLopHoc={Number(maLopHoc)} />}
          {maLopHoc && tab === 'diemDanh' && <BangDiemDanh maLopHoc={Number(maLopHoc)} />}
        </>
      )}
    </div>
  )
}

function BaoCaoTaiChinh() {
  const { duLieu, dangTai, loi, taiLai } = dungDuLieu(() => hocPhiApi.baoCao())

  if (dangTai) return <DangTai />
  if (loi) return <LoiTrang message={loi} onThuLai={taiLai} />
  if (!duLieu) return null

  return (
    <div className="space-y-5">
      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <TheChiSo nhan="Tổng phải thu" giaTri={dinhDangTien(duLieu.tongPhaiThu)} icon="wallet" />
        <TheChiSo
          nhan="Đã thu"
          giaTri={dinhDangTien(duLieu.tongDaThu)}
          icon="check"
          tone="emerald"
        />
        <TheChiSo
          nhan="Còn thiếu"
          giaTri={dinhDangTien(duLieu.tongConThieu)}
          icon="alert"
          tone="rose"
        />
        <TheChiSo
          nhan="Học viên còn nợ"
          giaTri={dinhDangSo(duLieu.soHocVienConNo)}
          moTa={`${dinhDangSo(duLieu.soQuaHan)} học phí quá hạn`}
          icon="users"
          tone="amber"
        />
      </div>

      <div className="grid gap-4 lg:grid-cols-2">
        <article className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
          <h3 className="mb-3 text-sm font-bold uppercase tracking-wider text-slate-500">
            Theo phương thức
          </h3>
          <ul className="space-y-2 text-sm">
            {duLieu.thongKeGiaoDichTheoPhuongThuc.length === 0 && (
              <li className="text-slate-400">Chưa có giao dịch.</li>
            )}
            {duLieu.thongKeGiaoDichTheoPhuongThuc.map((p) => (
              <li key={p.nhan} className="flex justify-between gap-3">
                <span className="text-slate-600">
                  {p.nhan} <span className="text-xs text-slate-400">({p.soLuong})</span>
                </span>
                <strong>{dinhDangTien(p.tongTien)}</strong>
              </li>
            ))}
          </ul>
        </article>

        <article className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
          <h3 className="mb-3 text-sm font-bold uppercase tracking-wider text-slate-500">
            Học viên còn nợ nhiều nhất
          </h3>
          <ul className="space-y-2 text-sm">
            {duLieu.danhSachConNo.length === 0 && (
              <li className="text-slate-400">Không có học viên còn nợ.</li>
            )}
            {duLieu.danhSachConNo.slice(0, 10).map((h) => (
              <li key={h.maHocVien} className="flex justify-between gap-3">
                <span className="text-slate-600">
                  {h.hoTen} <span className="text-xs text-slate-400">({h.soHocPhiConNo})</span>
                </span>
                <strong className="text-rose-600">{dinhDangTien(h.tongConThieu)}</strong>
              </li>
            ))}
          </ul>
        </article>
      </div>
    </div>
  )
}

function BaoCaoKhaoSat() {
  const { duLieu, dangTai, loi, taiLai } = dungDuLieu(() => baoCaoApi.khaoSat())

  if (dangTai) return <DangTai />
  if (loi) return <LoiTrang message={loi} onThuLai={taiLai} />
  if (!duLieu) return null

  const cot: CotBang<(typeof duLieu.khaoSats)[number]>[] = [
    { tieuDe: 'Khảo sát', khoa: 'tenKhaoSat' },
    { tieuDe: 'Trạng thái', khoa: 'trangThai', render: (r) => <Nhan>{r.trangThai}</Nhan> },
    { tieuDe: 'Câu hỏi', khoa: 'soCauHoi', canPhai: true },
    { tieuDe: 'Phản hồi', khoa: 'soPhanHoi', canPhai: true },
    {
      tieuDe: 'Tỉ lệ phản hồi',
      khoa: 'tiLePhanHoi',
      canPhai: true,
      render: (r) => (r.tiLePhanHoi === null || r.tiLePhanHoi === undefined ? '—' : `${r.tiLePhanHoi.toFixed(1)}%`),
    },
    {
      tieuDe: 'Điểm TB',
      khoa: 'diemTrungBinh',
      canPhai: true,
      render: (r) => (r.diemTrungBinh === null || r.diemTrungBinh === undefined ? '—' : r.diemTrungBinh.toFixed(2)),
    },
  ]

  return (
    <div className="space-y-5">
      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <TheChiSo nhan="Tổng khảo sát" giaTri={dinhDangSo(duLieu.tongKhaoSat)} icon="sparkles" />
        <TheChiSo
          nhan="Đang hoạt động"
          giaTri={dinhDangSo(duLieu.soHoatDong)}
          icon="check"
          tone="emerald"
        />
        <TheChiSo
          nhan="Tổng phản hồi"
          giaTri={dinhDangSo(duLieu.tongPhanHoi)}
          moTa={`${dinhDangSo(duLieu.tongHocVien)} học viên`}
          icon="users"
          tone="blue"
        />
        <TheChiSo
          nhan="Điểm đánh giá TB"
          giaTri={duLieu.diemDanhGiaChungTrungBinh?.toFixed(2) ?? '—'}
          icon="chart"
          tone="amber"
        />
      </div>

      <Bang cot={cot} duLieu={duLieu.khaoSats} khoa={(r) => r.maKhaoSat} />
    </div>
  )
}

function BangHocTap({ maLopHoc }: { maLopHoc: number }) {
  const { duLieu, dangTai, loi, taiLai } = dungDuLieu(() => baoCaoApi.hocTap(maLopHoc))

  if (dangTai) return <DangTai />
  if (loi) return <LoiTrang message={loi} onThuLai={taiLai} />
  if (!duLieu) return null

  return (
    <div className="space-y-5">
      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <TheChiSo nhan="Sĩ số" giaTri={dinhDangSo(duLieu.soHocVien)} icon="users" />
        <TheChiSo
          nhan="Đủ điều kiện"
          giaTri={dinhDangSo(duLieu.soDuDieuKien)}
          icon="check"
          tone="emerald"
        />
        <TheChiSo nhan="Thiếu điểm" giaTri={dinhDangSo(duLieu.soThieuDiem)} icon="alert" tone="amber" />
        <TheChiSo
          nhan="Điểm TB lớp"
          giaTri={duLieu.diemTongKetTrungBinh?.toFixed(2) ?? '—'}
          moTa={`Cao nhất ${duLieu.diemCaoNhat?.toFixed(2) ?? '—'} · Thấp nhất ${duLieu.diemThapNhat?.toFixed(2) ?? '—'}`}
          icon="chart"
          tone="slate"
        />
      </div>

      {duLieu.canhBaoTrongSo && (
        <p className="rounded-xl bg-amber-50 p-3 text-sm font-semibold text-amber-700">
          Tổng trọng số hiện tại là {duLieu.tongTrongSo} (nên bằng 100).
        </p>
      )}

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
                      {diem?.diem !== null && diem?.diem !== undefined ? (
                        diem.diem.toFixed(2)
                      ) : (
                        <span className="text-slate-300">—</span>
                      )}
                    </td>
                  )
                })}
                <td className="px-5 py-3 text-right font-bold">
                  {hv.diemTongKet !== null && hv.diemTongKet !== undefined ? (
                    hv.diemTongKet.toFixed(2)
                  ) : (
                    <span className="text-xs font-normal text-amber-600">Thiếu điểm</span>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  )
}

function BangDiemDanh({ maLopHoc }: { maLopHoc: number }) {
  const { duLieu, dangTai, loi, taiLai } = dungDuLieu(() =>
    baoCaoApi.diemDanh(maLopHoc, { nguongVang: 30 }),
  )

  if (dangTai) return <DangTai />
  if (loi) return <LoiTrang message={loi} onThuLai={taiLai} />
  if (!duLieu) return null

  const cot: CotBang<(typeof duLieu.hocViens)[number]>[] = [
    {
      tieuDe: 'Học viên',
      khoa: 'hoTen',
      render: (r) => (
        <div>
          <p className="font-semibold text-slate-900">{r.hoTen}</p>
          <p className="font-mono text-xs text-slate-500">{r.maHocVien}</p>
        </div>
      ),
    },
    { tieuDe: 'Lượt', khoa: 'tongLuot', canPhai: true },
    { tieuDe: 'Có mặt', khoa: 'coMat', canPhai: true },
    { tieuDe: 'Vắng', khoa: 'vang', canPhai: true },
    { tieuDe: 'Có phép', khoa: 'vangCoPhep', canPhai: true },
    { tieuDe: 'Đi trễ', khoa: 'diTre', canPhai: true },
    {
      tieuDe: 'Tỉ lệ có mặt',
      khoa: 'tiLeCoMat',
      canPhai: true,
      render: (r) => (
        <span className={r.vangCao ? 'font-bold text-rose-600' : ''}>
          {r.tiLeCoMat.toFixed(1)}%
        </span>
      ),
    },
  ]

  return (
    <div className="space-y-5">
      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <TheChiSo
          nhan="Ca đã điểm danh"
          giaTri={dinhDangSo(duLieu.soCaHocDaDiemDanh)}
          icon="calendar"
        />
        <TheChiSo
          nhan="Tỉ lệ có mặt toàn lớp"
          giaTri={`${duLieu.tiLeCoMatToanLop.toFixed(1)}%`}
          icon="check"
          tone="emerald"
        />
        <TheChiSo
          nhan="Ngưỡng vắng"
          giaTri={`${duLieu.nguongVang}%`}
          icon="alert"
          tone="amber"
        />
        <TheChiSo
          nhan="Học viên vắng cao"
          giaTri={dinhDangSo(duLieu.hocVienVangCao.length)}
          icon="users"
          tone="rose"
        />
      </div>

      <Bang cot={cot} duLieu={duLieu.hocViens} khoa={(r) => r.maHocVien} thongBaoRong="Chưa có dữ liệu điểm danh." />
    </div>
  )
}
