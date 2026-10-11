import { useEffect, useMemo, useState } from 'react'
import { Icon } from '../../components/BieuTuong'
import { goiAPI, type ClassItem, type StudentItem } from '../../lib/goiAPI'
import { dungXacThuc } from '../../contexts/NguCanhXacThuc'

type TrangThai = 'Có mặt' | 'Vắng' | 'Đi trễ'

type BanGhi = {
  maHV: string
  hoTen: string
  avatar: string
  trangThai: TrangThai
  ghiChu: string
}

export default function DiemDanh() {
  const { nguoiDung } = dungXacThuc()
  const [dsLop, setDsLop] = useState<ClassItem[]>([])
  const [lopChon, setLopChon] = useState<string>('')
  const [ngay, setNgay] = useState(() => new Date().toISOString().slice(0, 10))
  const [banGhi, setBanGhi] = useState<BanGhi[]>([])
  const [dangTai, setDangTai] = useState(false)
  const [dangLuu, setDangLuu] = useState(false)
  const [thongBao, setThongBao] = useState('')

  // Load lớp của GV
  useEffect(() => {
    if (!nguoiDung) return
    goiAPI.layLopCuaToi(nguoiDung.id).then((data) => {
      setDsLop(data)
      if (data.length > 0) setLopChon(data[0].maLop)
    })
  }, [nguoiDung])

  // Load điểm danh khi chọn lớp / ngày
  useEffect(() => {
    if (!lopChon || !ngay) return
    setDangTai(true)
    setThongBao('')

    Promise.all([
      goiAPI.layHocVienTheoLop(lopChon),
      goiAPI.layDiemDanh(lopChon, ngay),
    ]).then(([hocVien, diemDanh]) => {
      const map: Record<string, { trangThai: TrangThai; ghiChu: string }> = {}
      diemDanh.forEach((d) => {
        map[d.maHV] = { trangThai: d.trangThai, ghiChu: d.ghiChu }
      })

      const ds: BanGhi[] = hocVien.map((hv: StudentItem) => ({
        maHV: hv.maHV,
        hoTen: hv.hoTen,
        avatar: hv.hoTen
          .split(' ')
          .filter(Boolean)
          .map((w) => w[0])
          .slice(0, 2)
          .join('')
          .toUpperCase(),
        trangThai: map[hv.maHV]?.trangThai ?? 'Có mặt',
        ghiChu: map[hv.maHV]?.ghiChu ?? '',
      }))
      setBanGhi(ds)
      setDangTai(false)
    })
  }, [lopChon, ngay])

  const thongKe = useMemo(() => {
    const coMat = banGhi.filter((b) => b.trangThai === 'Có mặt').length
    const vang = banGhi.filter((b) => b.trangThai === 'Vắng').length
    const tre = banGhi.filter((b) => b.trangThai === 'Đi trễ').length
    return { coMat, vang, tre }
  }, [banGhi])

  const capNhat = (maHV: string, patch: Partial<BanGhi>) => {
    setBanGhi((prev) =>
      prev.map((b) => (b.maHV === maHV ? { ...b, ...patch } : b)),
    )
  }

  const danhDauTatCa = (tt: TrangThai) => {
    setBanGhi((prev) => prev.map((b) => ({ ...b, trangThai: tt })))
  }

  const luu = async () => {
    if (!lopChon || !ngay) return
    setDangLuu(true)
    setThongBao('')
    try {
      await goiAPI.luuDiemDanh(
        lopChon,
        ngay,
        banGhi.map((b) => ({
          maHV: b.maHV,
          trangThai: b.trangThai,
          ghiChu: b.ghiChu,
        })),
      )
      setThongBao('✅ Đã lưu điểm danh thành công!')
    } catch {
      setThongBao('❌ Có lỗi khi lưu. Vui lòng thử lại.')
    } finally {
      setDangLuu(false)
    }
  }

  const lopHienTai = dsLop.find((l) => l.maLop === lopChon)

  return (
    <div className="p-4 sm:p-7">
      {/* Header */}
      <div className="mb-6">
        <p className="mb-1 text-sm font-semibold text-blue-600">Giảng dạy</p>
        <h1 className="text-3xl font-bold text-slate-900">Điểm danh</h1>
        <p className="mt-2 text-sm text-slate-500">
          Điểm danh học viên trong từng buổi học và lưu ghi chú.
        </p>
      </div>

      {/* Bộ chọn lớp + ngày */}
      <div className="mb-6 grid gap-4 rounded-2xl border border-slate-200 bg-white p-5 shadow-sm sm:grid-cols-2">
        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">
            Chọn lớp
          </span>
          <div className="relative">
            <select
              value={lopChon}
              onChange={(e) => setLopChon(e.target.value)}
              className="w-full appearance-none rounded-xl border border-slate-200 bg-white px-4 py-3 pr-10 text-sm font-semibold outline-none focus:border-blue-500"
            >
              {dsLop.map((lop) => (
                <option key={lop.id} value={lop.maLop}>
                  {lop.tenLop} ({lop.maLop})
                </option>
              ))}
            </select>
            <Icon
              name="chevronDown"
              className="pointer-events-none absolute right-3 top-3.5 size-4 text-slate-400"
            />
          </div>
        </label>

        <label className="block">
          <span className="mb-2 block text-sm font-semibold text-slate-700">
            Ngày học
          </span>
          <input
            type="date"
            value={ngay}
            onChange={(e) => setNgay(e.target.value)}
            className="w-full rounded-xl border border-slate-200 px-4 py-3 text-sm font-semibold outline-none focus:border-blue-500"
          />
        </label>
      </div>

      {/* Thông tin lớp + thống kê */}
      {lopHienTai && (
        <div className="mb-6 grid gap-4 sm:grid-cols-4">
          <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
            <div className="text-xs font-semibold uppercase text-slate-500">Lớp</div>
            <div className="mt-1 text-lg font-bold text-slate-900">
              {lopHienTai.tenLop}
            </div>
            <div className="text-xs text-slate-500">
              Phòng {lopHienTai.phong} · {lopHienTai.lichHoc}
            </div>
          </div>
          <div className="rounded-2xl border border-blue-200 bg-blue-50 p-4 shadow-sm">
            <div className="text-xs font-semibold uppercase text-blue-700">Có mặt</div>
            <div className="mt-1 text-2xl font-bold text-blue-700">
              {thongKe.coMat}
            </div>
          </div>
          <div className="rounded-2xl border border-amber-200 bg-amber-50 p-4 shadow-sm">
            <div className="text-xs font-semibold uppercase text-amber-700">Đi trễ</div>
            <div className="mt-1 text-2xl font-bold text-amber-700">
              {thongKe.tre}
            </div>
          </div>
          <div className="rounded-2xl border border-red-200 bg-red-50 p-4 shadow-sm">
            <div className="text-xs font-semibold uppercase text-red-700">Vắng</div>
            <div className="mt-1 text-2xl font-bold text-red-700">
              {thongKe.vang}
            </div>
          </div>
        </div>
      )}

      {/* Nút đánh dấu nhanh */}
      {banGhi.length > 0 && (
        <div className="mb-4 flex flex-wrap items-center gap-2">
          <span className="text-sm font-semibold text-slate-600">
            Đánh dấu nhanh:
          </span>
          <button
            onClick={() => danhDauTatCa('Có mặt')}
            className="rounded-lg border border-blue-200 bg-blue-50 px-3 py-1.5 text-xs font-bold text-blue-700 hover:bg-blue-100"
          >
            Tất cả có mặt
          </button>
          <button
            onClick={() => danhDauTatCa('Vắng')}
            className="rounded-lg border border-red-200 bg-red-50 px-3 py-1.5 text-xs font-bold text-red-700 hover:bg-red-100"
          >
            Tất cả vắng
          </button>
        </div>
      )}

      {/* Bảng điểm danh */}
      <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
        {dangTai ? (
          <div className="grid place-items-center py-20 text-slate-400">
            Đang tải danh sách học viên...
          </div>
        ) : banGhi.length === 0 ? (
          <div className="grid place-items-center py-20 text-slate-400">
            Chưa có học viên trong lớp này
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full border-collapse text-left text-sm">
              <thead>
                <tr className="border-b border-slate-200 bg-slate-50 text-xs uppercase tracking-wider text-slate-500">
                  <th className="px-4 py-3 font-bold">Mã HV</th>
                  <th className="px-4 py-3 font-bold">Học viên</th>
                  <th className="px-4 py-3 font-bold">Trạng thái</th>
                  <th className="px-4 py-3 font-bold">Ghi chú</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {banGhi.map((b) => (
                  <tr key={b.maHV} className="hover:bg-slate-50">
                    <td className="px-4 py-3 font-semibold text-blue-600">
                      {b.maHV}
                    </td>
                    <td className="px-4 py-3">
                      <div className="flex items-center gap-3">
                        <span className="grid size-8 place-items-center rounded-full bg-blue-100 text-xs font-bold text-blue-700">
                          {b.avatar}
                        </span>
                        <span className="font-medium text-slate-900">
                          {b.hoTen}
                        </span>
                      </div>
                    </td>
                    <td className="px-4 py-3">
                      <div className="flex gap-1">
                        {(['Có mặt', 'Đi trễ', 'Vắng'] as const).map((tt) => (
                          <button
                            key={tt}
                            onClick={() => capNhat(b.maHV, { trangThai: tt })}
                            className={`rounded-lg px-2.5 py-1 text-xs font-bold transition ${
                              b.trangThai === tt
                                ? tt === 'Có mặt'
                                  ? 'bg-blue-600 text-white'
                                  : tt === 'Đi trễ'
                                    ? 'bg-amber-500 text-white'
                                    : 'bg-red-500 text-white'
                                : 'border border-slate-200 bg-white text-slate-500 hover:border-blue-200'
                            }`}
                          >
                            {tt}
                          </button>
                        ))}
                      </div>
                    </td>
                    <td className="px-4 py-3">
                      <input
                        type="text"
                        value={b.ghiChu}
                        onChange={(e) =>
                          capNhat(b.maHV, { ghiChu: e.target.value })
                        }
                        placeholder="Ghi chú..."
                        className="w-full min-w-40 rounded-lg border border-slate-200 px-3 py-1.5 text-sm outline-none focus:border-blue-500"
                      />
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        )}

        {/* Footer — Nút lưu */}
        {banGhi.length > 0 && (
          <div className="flex flex-wrap items-center justify-between gap-3 border-t border-slate-100 bg-slate-50 px-4 py-4">
            <span className="text-sm text-slate-500">
              Tổng: <b>{banGhi.length}</b> học viên ·{' '}
              <span className="text-blue-600 font-bold">{thongKe.coMat} có mặt</span> ·{' '}
              <span className="text-amber-600 font-bold">{thongKe.tre} trễ</span> ·{' '}
              <span className="text-red-600 font-bold">{thongKe.vang} vắng</span>
            </span>
            <button
              onClick={luu}
              disabled={dangLuu}
              className="flex items-center gap-2 rounded-xl bg-blue-600 px-5 py-2.5 text-sm font-bold text-white shadow-sm shadow-blue-200 transition hover:bg-blue-700 disabled:opacity-50"
            >
              <Icon name="check" className="size-4" />
              {dangLuu ? 'Đang lưu...' : 'Lưu điểm danh'}
            </button>
          </div>
        )}
      </div>

      {/* Thông báo */}
      {thongBao && (
        <div
          className={`mt-4 rounded-xl border p-4 text-sm font-semibold ${
            thongBao.startsWith('✅')
              ? 'border-blue-200 bg-blue-50 text-blue-700'
              : 'border-red-200 bg-red-50 text-red-700'
          }`}
        >
          {thongBao}
        </div>
      )}
    </div>
  )
}