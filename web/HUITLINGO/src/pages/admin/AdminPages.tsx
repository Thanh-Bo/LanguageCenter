// Trang quản trị: Tổng quan, Học viên, Lớp học, Học phí.
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
import { baoCaoApi, hocPhiApi, hocVienApi, lopHocApi } from '../../lib/api/endpoints'
import type {
  ClassListItem,
  PagedResult,
  StudentListItem,
  TuitionItem,
} from '../../lib/api/types'
import {
  PHUONG_THUC_THANH_TOAN,
  TRANG_THAI_HOC_PHI,
  TRANG_THAI_HOC_TAP,
  TRANG_THAI_LOP,
} from '../../lib/constants'
import { dinhDangNgay, dinhDangSo, dinhDangTien } from '../../lib/format'
import { dungDuLieu, thongDiepLoi } from '../../lib/useApi'

const PAGE_SIZE = 20

// ============================================================
// TỔNG QUAN
// ============================================================
export function AdminTongQuan() {
  const { duLieu, dangTai, loi, taiLai } = dungDuLieu(() => baoCaoApi.tongQuan())

  if (dangTai) return <DangTai nhan="Đang tải tổng quan…" />
  if (loi) return <LoiTrang message={loi} onThuLai={taiLai} />
  if (!duLieu) return null

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Quản trị hệ thống"
        title="Tổng quan trung tâm"
        desc="Số liệu tổng hợp theo thời gian thực từ cơ sở dữ liệu."
        actions={
          <button type="button" onClick={taiLai} className={LOP_NUT_PHU}>
            <Icon name="arrowUp" className="size-4" /> Làm mới
          </button>
        }
      />

      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <TheChiSo
          nhan="Tổng học viên"
          giaTri={dinhDangSo(duLieu.tongHocVien)}
          moTa={`${dinhDangSo(duLieu.soHocVienDangHoc)} đang học`}
          icon="users"
          tone="blue"
        />
        <TheChiSo
          nhan="Lớp đang học"
          giaTri={dinhDangSo(duLieu.soLopDangHoc)}
          moTa={`${dinhDangSo(duLieu.tongLopHoc)} lớp tổng cộng`}
          icon="book"
          tone="emerald"
        />
        <TheChiSo
          nhan="Đã thu học phí"
          giaTri={dinhDangTien(duLieu.hocPhi.tongDaThu)}
          moTa={`Còn thiếu ${dinhDangTien(duLieu.hocPhi.tongConThieu)}`}
          icon="wallet"
          tone="amber"
        />
        <TheChiSo
          nhan="Tỉ lệ có mặt"
          giaTri={`${duLieu.diemDanh.tiLeCoMat.toFixed(1)}%`}
          moTa={`${dinhDangSo(duLieu.diemDanh.tongLuot)} lượt điểm danh`}
          icon="check"
          tone="slate"
        />
      </div>

      <div className="grid gap-4 lg:grid-cols-3">
        <KhoiSoLieu
          tieuDe="Đăng ký học"
          dong={[
            ['Tổng số', duLieu.dangKy.tongSo],
            ['Chờ xử lý', duLieu.dangKy.choXuLy],
            ['Đã xếp lớp', duLieu.dangKy.daXepLop],
            ['Hoàn thành', duLieu.dangKy.hoanThanh],
            ['Đã hủy', duLieu.dangKy.daHuy],
          ]}
        />
        <KhoiSoLieu
          tieuDe="Ca học"
          dong={[
            ['Tổng số', duLieu.caHoc.tongSo],
            ['Chưa diễn ra', duLieu.caHoc.chuaDienRa],
            ['Sắp diễn ra', duLieu.caHoc.sapDienRa],
            ['Đã hoàn thành', duLieu.caHoc.daHoanThanh],
            ['Đã hủy', duLieu.caHoc.daHuy],
          ]}
        />
        <KhoiSoLieu
          tieuDe="Khảo sát"
          dong={[
            ['Tổng số', duLieu.khaoSat.tongSo],
            ['Đang hoạt động', duLieu.khaoSat.soHoatDong],
            ['Câu hỏi', duLieu.khaoSat.tongCauHoi],
            ['Phản hồi', duLieu.khaoSat.tongPhanHoi],
          ]}
        />
      </div>

      <div className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <TheChiSo
          nhan="Tài khoản"
          giaTri={dinhDangSo(duLieu.tongTaiKhoan)}
          icon="lock"
          tone="slate"
        />
        <TheChiSo
          nhan="Giáo viên"
          giaTri={dinhDangSo(duLieu.tongGiaoVien)}
          icon="userPlus"
          tone="blue"
        />
        <TheChiSo
          nhan="Nhân viên"
          giaTri={dinhDangSo(duLieu.tongNhanVien)}
          icon="users"
          tone="emerald"
        />
        <TheChiSo
          nhan="Khóa học"
          giaTri={dinhDangSo(duLieu.tongKhoaHoc)}
          moTa={`${dinhDangSo(duLieu.tongChuongTrinh)} chương trình`}
          icon="book"
          tone="amber"
        />
      </div>
    </div>
  )
}

function KhoiSoLieu({
  tieuDe,
  dong,
}: {
  tieuDe: string
  dong: Array<[string, number]>
}) {
  return (
    <article className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
      <h3 className="mb-4 text-sm font-bold uppercase tracking-wider text-slate-500">
        {tieuDe}
      </h3>
      <ul className="space-y-2.5 text-sm">
        {dong.map(([nhan, giaTri]) => (
          <li key={nhan} className="flex items-center justify-between gap-3">
            <span className="text-slate-500">{nhan}</span>
            <strong className="text-slate-900">{dinhDangSo(giaTri)}</strong>
          </li>
        ))}
      </ul>
    </article>
  )
}

// ============================================================
// HỌC VIÊN
// ============================================================
export function AdminHocVien() {
  const [ketQua, setKetQua] = useState<PagedResult<StudentListItem> | null>(null)
  const [dangTai, setDangTai] = useState(true)
  const [loi, setLoi] = useState<string | null>(null)

  const [tuKhoaNhap, setTuKhoaNhap] = useState('')
  const [tuKhoa, setTuKhoa] = useState('')
  const [trangThai, setTrangThai] = useState('')
  const [page, setPage] = useState(1)

  const tai = useCallback(async () => {
    setDangTai(true)
    setLoi(null)
    try {
      const kq = await hocVienApi.danhSach({
        page,
        pageSize: PAGE_SIZE,
        search: tuKhoa,
        trangThai,
      })
      setKetQua(kq)
    } catch (error) {
      setLoi(thongDiepLoi(error))
    } finally {
      setDangTai(false)
    }
  }, [page, tuKhoa, trangThai])

  useEffect(() => {
    void tai()
  }, [tai])

  const cot: CotBang<StudentListItem>[] = [
    {
      tieuDe: 'Mã HV',
      khoa: 'maHocVien',
      render: (r) => <span className="font-mono text-xs font-semibold">{r.maHocVien}</span>,
    },
    {
      tieuDe: 'Họ tên',
      khoa: 'hoTen',
      render: (r) => <span className="font-semibold text-slate-900">{r.hoTen}</span>,
    },
    { tieuDe: 'Ngày sinh', khoa: 'ngaySinh', render: (r) => dinhDangNgay(r.ngaySinh) },
    { tieuDe: 'Giới tính', khoa: 'gioiTinh', render: (r) => r.gioiTinh ?? '—' },
    { tieuDe: 'Email', khoa: 'email', render: (r) => r.email ?? '—' },
    { tieuDe: 'Điện thoại', khoa: 'soDienThoai', render: (r) => r.soDienThoai ?? '—' },
    {
      tieuDe: 'Trạng thái',
      khoa: 'trangThaiHocTap',
      render: (r) => <Nhan>{r.trangThaiHocTap}</Nhan>,
    },
  ]

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Danh mục"
        title="Học viên"
        desc="Tra cứu hồ sơ học viên theo mã, họ tên, email hoặc trạng thái học tập."
      />

      <form
        className="flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm sm:flex-row sm:items-center"
        onSubmit={(e) => {
          e.preventDefault()
          setPage(1)
          setTuKhoa(tuKhoaNhap.trim())
        }}
      >
        <input
          value={tuKhoaNhap}
          onChange={(e) => setTuKhoaNhap(e.target.value)}
          placeholder="Tìm theo mã, họ tên hoặc email…"
          className={LOP_INPUT}
        />
        <select
          value={trangThai}
          onChange={(e) => {
            setPage(1)
            setTrangThai(e.target.value)
          }}
          className={`${LOP_INPUT} sm:w-56`}
        >
          <option value="">Tất cả trạng thái</option>
          {TRANG_THAI_HOC_TAP.map((t) => (
            <option key={t} value={t}>
              {t}
            </option>
          ))}
        </select>
        <button type="submit" className={LOP_NUT_CHINH}>
          <Icon name="search" className="size-4" /> Tìm
        </button>
      </form>

      {loi ? (
        <LoiTrang message={loi} onThuLai={tai} />
      ) : (
        <>
          <Bang
            cot={cot}
            duLieu={ketQua?.items ?? []}
            khoa={(r) => r.maHocVien}
            dangTai={dangTai}
            thongBaoRong="Không tìm thấy học viên phù hợp."
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
    </div>
  )
}

// ============================================================
// LỚP HỌC
// ============================================================
export function AdminLopHoc() {
  const [ketQua, setKetQua] = useState<PagedResult<ClassListItem> | null>(null)
  const [dangTai, setDangTai] = useState(true)
  const [loi, setLoi] = useState<string | null>(null)

  const [tuKhoaNhap, setTuKhoaNhap] = useState('')
  const [tuKhoa, setTuKhoa] = useState('')
  const [trangThai, setTrangThai] = useState('')
  const [page, setPage] = useState(1)

  const tai = useCallback(async () => {
    setDangTai(true)
    setLoi(null)
    try {
      const kq = await lopHocApi.danhSach({
        page,
        pageSize: PAGE_SIZE,
        search: tuKhoa,
        trangThai,
      })
      setKetQua(kq)
    } catch (error) {
      setLoi(thongDiepLoi(error))
    } finally {
      setDangTai(false)
    }
  }, [page, tuKhoa, trangThai])

  useEffect(() => {
    void tai()
  }, [tai])

  const cot: CotBang<ClassListItem>[] = [
    {
      tieuDe: 'Mã lớp',
      khoa: 'maLop',
      render: (r) => <span className="font-mono text-xs font-semibold">{r.maLop}</span>,
    },
    {
      tieuDe: 'Tên lớp',
      khoa: 'tenLop',
      render: (r) => (
        <div>
          <p className="font-semibold text-slate-900">{r.tenLop}</p>
          <p className="text-xs text-slate-500">{r.tenKhoaHoc}</p>
        </div>
      ),
    },
    {
      tieuDe: 'Sĩ số',
      khoa: 'siSoHienTai',
      render: (r) => `${r.siSoHienTai}/${r.siSoToiDa}`,
    },
    { tieuDe: 'Bắt đầu', khoa: 'ngayBatDau', render: (r) => dinhDangNgay(r.ngayBatDau) },
    {
      tieuDe: 'Kết thúc dự kiến',
      khoa: 'ngayKetThucDuKien',
      render: (r) => dinhDangNgay(r.ngayKetThucDuKien),
    },
    { tieuDe: 'Học vụ', khoa: 'tenNhanVienHocVu', render: (r) => r.tenNhanVienHocVu },
    { tieuDe: 'Trạng thái', khoa: 'trangThai', render: (r) => <Nhan>{r.trangThai}</Nhan> },
  ]

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Danh mục"
        title="Lớp học"
        desc="Theo dõi sĩ số, lịch khai giảng và trạng thái của từng lớp."
      />

      <form
        className="flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm sm:flex-row sm:items-center"
        onSubmit={(e) => {
          e.preventDefault()
          setPage(1)
          setTuKhoa(tuKhoaNhap.trim())
        }}
      >
        <input
          value={tuKhoaNhap}
          onChange={(e) => setTuKhoaNhap(e.target.value)}
          placeholder="Tìm theo mã lớp hoặc tên lớp…"
          className={LOP_INPUT}
        />
        <select
          value={trangThai}
          onChange={(e) => {
            setPage(1)
            setTrangThai(e.target.value)
          }}
          className={`${LOP_INPUT} sm:w-56`}
        >
          <option value="">Tất cả trạng thái</option>
          {TRANG_THAI_LOP.map((t) => (
            <option key={t} value={t}>
              {t}
            </option>
          ))}
        </select>
        <button type="submit" className={LOP_NUT_CHINH}>
          <Icon name="search" className="size-4" /> Tìm
        </button>
      </form>

      {loi ? (
        <LoiTrang message={loi} onThuLai={tai} />
      ) : (
        <>
          <Bang
            cot={cot}
            duLieu={ketQua?.items ?? []}
            khoa={(r) => r.maLopHoc}
            dangTai={dangTai}
            thongBaoRong="Không có lớp học phù hợp."
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
    </div>
  )
}

// ============================================================
// HỌC PHÍ + THANH TOÁN
// ============================================================
export function AdminHocPhi() {
  const { hien } = dungToast()

  const [ketQua, setKetQua] = useState<PagedResult<TuitionItem> | null>(null)
  const [dangTai, setDangTai] = useState(true)
  const [loi, setLoi] = useState<string | null>(null)

  const [tuKhoaNhap, setTuKhoaNhap] = useState('')
  const [tuKhoa, setTuKhoa] = useState('')
  const [trangThai, setTrangThai] = useState('')
  const [page, setPage] = useState(1)

  const [dangChon, setDangChon] = useState<TuitionItem | null>(null)

  const tai = useCallback(async () => {
    setDangTai(true)
    setLoi(null)
    try {
      const kq = await hocPhiApi.danhSach({
        page,
        pageSize: PAGE_SIZE,
        search: tuKhoa,
        trangThai,
      })
      setKetQua(kq)
    } catch (error) {
      setLoi(thongDiepLoi(error))
    } finally {
      setDangTai(false)
    }
  }, [page, tuKhoa, trangThai])

  useEffect(() => {
    void tai()
  }, [tai])

  const cot: CotBang<TuitionItem>[] = [
    {
      tieuDe: 'Học viên',
      khoa: 'hoTenHocVien',
      render: (r) => (
        <div>
          <p className="font-semibold text-slate-900">{r.hoTenHocVien}</p>
          <p className="font-mono text-xs text-slate-500">{r.maHocVien}</p>
        </div>
      ),
    },
    {
      tieuDe: 'Khóa học',
      khoa: 'tenKhoaHoc',
      render: (r) => (
        <div>
          <p>{r.tenKhoaHoc}</p>
          {r.maLop && <p className="text-xs text-slate-500">{r.maLop}</p>}
        </div>
      ),
    },
    { tieuDe: 'Phải đóng', khoa: 'soTienPhaiDong', canPhai: true, render: (r) => dinhDangTien(r.soTienPhaiDong) },
    { tieuDe: 'Đã đóng', khoa: 'soTienDaDong', canPhai: true, render: (r) => dinhDangTien(r.soTienDaDong) },
    {
      tieuDe: 'Còn thiếu',
      khoa: 'soTienConThieu',
      canPhai: true,
      render: (r) => (
        <span className={r.soTienConThieu > 0 ? 'font-bold text-rose-600' : 'text-slate-500'}>
          {dinhDangTien(r.soTienConThieu)}
        </span>
      ),
    },
    { tieuDe: 'Hạn đóng', khoa: 'hanThanhToan', render: (r) => dinhDangNgay(r.hanThanhToan) },
    {
      tieuDe: 'Trạng thái',
      khoa: 'trangThaiHocPhi',
      render: (r) => (
        <div className="flex flex-col items-start gap-1">
          <Nhan>{r.trangThaiHocPhi}</Nhan>
          {r.quaHan && <span className="text-xs font-bold text-rose-600">Quá hạn</span>}
        </div>
      ),
    },
    {
      tieuDe: '',
      khoa: 'thaoTac',
      canPhai: true,
      render: (r) => (
        <button
          type="button"
          onClick={() => setDangChon(r)}
          disabled={r.soTienConThieu <= 0}
          className="rounded-lg border border-blue-200 px-3 py-1.5 text-xs font-bold text-blue-600 transition hover:bg-blue-50 disabled:opacity-40"
        >
          Thu tiền
        </button>
      ),
    },
  ]

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Tài chính"
        title="Học phí & thanh toán"
        desc="Ghi nhận thanh toán sẽ tự sinh biên nhận và cập nhật số tiền đã đóng."
      />

      <form
        className="flex flex-col gap-3 rounded-2xl border border-slate-200 bg-white p-4 shadow-sm sm:flex-row sm:items-center"
        onSubmit={(e) => {
          e.preventDefault()
          setPage(1)
          setTuKhoa(tuKhoaNhap.trim())
        }}
      >
        <input
          value={tuKhoaNhap}
          onChange={(e) => setTuKhoaNhap(e.target.value)}
          placeholder="Tìm theo mã hoặc tên học viên…"
          className={LOP_INPUT}
        />
        <select
          value={trangThai}
          onChange={(e) => {
            setPage(1)
            setTrangThai(e.target.value)
          }}
          className={`${LOP_INPUT} sm:w-60`}
        >
          <option value="">Tất cả trạng thái</option>
          {TRANG_THAI_HOC_PHI.map((t) => (
            <option key={t} value={t}>
              {t}
            </option>
          ))}
        </select>
        <button type="submit" className={LOP_NUT_CHINH}>
          <Icon name="search" className="size-4" /> Tìm
        </button>
      </form>

      {loi ? (
        <LoiTrang message={loi} onThuLai={tai} />
      ) : (
        <>
          <Bang
            cot={cot}
            duLieu={ketQua?.items ?? []}
            khoa={(r) => r.maHocPhi}
            dangTai={dangTai}
            thongBaoRong="Không có học phí phù hợp."
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

      <HopThoaiThuTien
        hocPhi={dangChon}
        onDong={() => setDangChon(null)}
        onXong={(soBienNhan) => {
          setDangChon(null)
          hien(`Đã ghi nhận thanh toán. Biên nhận: ${soBienNhan}`, 'thanhCong')
          void tai()
        }}
      />
    </div>
  )
}

function HopThoaiThuTien({
  hocPhi,
  onDong,
  onXong,
}: {
  hocPhi: TuitionItem | null
  onDong: () => void
  onXong: (soBienNhan: string) => void
}) {
  const [soTien, setSoTien] = useState('')
  const [phuongThuc, setPhuongThuc] = useState<string>(PHUONG_THUC_THANH_TOAN[0])
  const [ghiChu, setGhiChu] = useState('')
  const [dangGui, setDangGui] = useState(false)
  const [loi, setLoi] = useState<string | null>(null)

  useEffect(() => {
    setSoTien(hocPhi ? String(hocPhi.soTienConThieu) : '')
    setPhuongThuc(PHUONG_THUC_THANH_TOAN[0])
    setGhiChu('')
    setLoi(null)
    setDangGui(false)
  }, [hocPhi])

  if (!hocPhi) return null

  const gui = async () => {
    const giaTri = Number(soTien)

    if (!Number.isFinite(giaTri) || giaTri <= 0) {
      setLoi('Số tiền phải lớn hơn 0.')
      return
    }
    if (giaTri > hocPhi.soTienConThieu) {
      setLoi(`Số tiền vượt quá số còn thiếu (${dinhDangTien(hocPhi.soTienConThieu)}).`)
      return
    }

    setDangGui(true)
    setLoi(null)

    try {
      const ketQua = await hocPhiApi.ghiNhanThanhToan(hocPhi.maHocPhi, {
        maGiaoDich: `GD${Date.now()}`,
        soTien: giaTri,
        phuongThucThanhToan: phuongThuc,
        ghiChu: ghiChu.trim() || null,
      })
      onXong(ketQua.bienNhan?.soBienNhan ?? '—')
    } catch (error) {
      setLoi(thongDiepLoi(error))
      setDangGui(false)
    }
  }

  return (
    <HopThoai
      mo={!!hocPhi}
      tieuDe="Ghi nhận thanh toán"
      onDong={onDong}
      footer={
        <>
          <button type="button" onClick={onDong} className={LOP_NUT_PHU}>
            Hủy
          </button>
          <button type="button" onClick={gui} disabled={dangGui} className={LOP_NUT_CHINH}>
            {dangGui ? 'Đang lưu…' : 'Xác nhận thu'}
          </button>
        </>
      }
    >
      <div className="space-y-4">
        <div className="rounded-xl bg-slate-50 p-4 text-sm">
          <p className="font-bold text-slate-900">{hocPhi.hoTenHocVien}</p>
          <p className="text-slate-500">
            {hocPhi.tenKhoaHoc}
            {hocPhi.maLop ? ` · ${hocPhi.maLop}` : ''}
          </p>
          <dl className="mt-3 grid grid-cols-2 gap-2 text-xs">
            <dt className="text-slate-500">Phải đóng</dt>
            <dd className="text-right font-semibold">{dinhDangTien(hocPhi.soTienPhaiDong)}</dd>
            <dt className="text-slate-500">Đã đóng</dt>
            <dd className="text-right font-semibold">{dinhDangTien(hocPhi.soTienDaDong)}</dd>
            <dt className="text-slate-500">Còn thiếu</dt>
            <dd className="text-right font-bold text-rose-600">
              {dinhDangTien(hocPhi.soTienConThieu)}
            </dd>
          </dl>
        </div>

        <div>
          <label className={LOP_NHAN} htmlFor="so-tien">
            Số tiền thu
          </label>
          <input
            id="so-tien"
            type="number"
            min={1}
            max={hocPhi.soTienConThieu}
            value={soTien}
            onChange={(e) => setSoTien(e.target.value)}
            className={LOP_INPUT}
          />
        </div>

        <div>
          <label className={LOP_NHAN} htmlFor="phuong-thuc">
            Phương thức
          </label>
          <select
            id="phuong-thuc"
            value={phuongThuc}
            onChange={(e) => setPhuongThuc(e.target.value)}
            className={LOP_INPUT}
          >
            {PHUONG_THUC_THANH_TOAN.map((p) => (
              <option key={p} value={p}>
                {p}
              </option>
            ))}
          </select>
        </div>

        <div>
          <label className={LOP_NHAN} htmlFor="ghi-chu">
            Ghi chú
          </label>
          <textarea
            id="ghi-chu"
            rows={3}
            value={ghiChu}
            onChange={(e) => setGhiChu(e.target.value)}
            className="w-full rounded-xl border border-slate-200 bg-white p-3 text-sm outline-none focus:border-blue-500 focus:ring-4 focus:ring-blue-50"
            placeholder="Không bắt buộc"
          />
        </div>

        {loi && (
          <p className="rounded-xl bg-rose-50 p-3 text-sm font-semibold text-rose-700">{loi}</p>
        )}
      </div>
    </HopThoai>
  )
}
