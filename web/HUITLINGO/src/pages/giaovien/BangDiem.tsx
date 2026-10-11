import { useEffect, useMemo, useState } from 'react'
import { Icon } from '../../components/BieuTuong'
import { goiAPI, type ClassItem, type StudentItem } from '../../lib/goiAPI'
import { dungXacThuc } from '../../contexts/NguCanhXacThuc'

type Diem = {
  maHV: string
  hoTen: string
  avatar: string
  giuaKy: number | null
  cuoiKy: number | null
  baiTap: number | null
}

// Tính trung bình có trọng số: GK 30% + CK 50% + BT 20%
function tinhTB(gk: number | null, ck: number | null, bt: number | null) {
  const parts: Array<{ value: number; weight: number }> = []
  if (gk !== null) parts.push({ value: gk, weight: 0.3 })
  if (ck !== null) parts.push({ value: ck, weight: 0.5 })
  if (bt !== null) parts.push({ value: bt, weight: 0.2 })
  if (parts.length === 0) return null
  const totalWeight = parts.reduce((s, p) => s + p.weight, 0)
  const sum = parts.reduce((s, p) => s + p.value * p.weight, 0)
  return +(sum / totalWeight).toFixed(2)
}

function xepLoai(tb: number | null) {
  if (tb === null) return { label: 'Chưa có', color: 'text-slate-400' }
  if (tb >= 8.5) return { label: 'Xuất sắc', color: 'text-blue-700' }
  if (tb >= 7.0) return { label: 'Giỏi', color: 'text-blue-600' }
  if (tb >= 5.5) return { label: 'Khá', color: 'text-amber-600' }
  if (tb >= 4.0) return { label: 'Trung bình', color: 'text-orange-600' }
  return { label: 'Yếu', color: 'text-red-600' }
}

export default function BangDiem() {
  const { nguoiDung } = dungXacThuc()
  const [dsLop, setDsLop] = useState<ClassItem[]>([])
  const [lopChon, setLopChon] = useState('')
  const [dsDiem, setDsDiem] = useState<Diem[]>([])
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

  // Load điểm khi chọn lớp
  useEffect(() => {
    if (!lopChon) return
    setDangTai(true)
    setThongBao('')

    Promise.all([
      goiAPI.layHocVienTheoLop(lopChon),
      goiAPI.layBangDiem(lopChon),
    ]).then(([hocVien, bangDiem]) => {
      const map: Record<string, { giuaKy: number | null; cuoiKy: number | null; baiTap: number | null }> = {}
      bangDiem.forEach((d) => {
        map[d.maHV] = { giuaKy: d.giuaKy, cuoiKy: d.cuoiKy, baiTap: d.baiTap }
      })

      const ds: Diem[] = hocVien.map((hv: StudentItem) => ({
        maHV: hv.maHV,
        hoTen: hv.hoTen,
        avatar: hv.hoTen
          .split(' ')
          .filter(Boolean)
          .map((w) => w[0])
          .slice(0, 2)
          .join('')
          .toUpperCase(),
        giuaKy: map[hv.maHV]?.giuaKy ?? null,
        cuoiKy: map[hv.maHV]?.cuoiKy ?? null,
        baiTap: map[hv.maHV]?.baiTap ?? null,
      }))
      setDsDiem(ds)
      setDangTai(false)
    })
  }, [lopChon])

  const thongKe = useMemo(() => {
    const coDiem = dsDiem
      .map((d) => tinhTB(d.giuaKy, d.cuoiKy, d.baiTap))
      .filter((x): x is number => x !== null)
    if (coDiem.length === 0)
      return { tb: null as number | null, caoNhat: null as number | null, thapNhat: null as number | null }
    return {
      tb: +(coDiem.reduce((s, x) => s + x, 0) / coDiem.length).toFixed(2),
      caoNhat: Math.max(...coDiem),
      thapNhat: Math.min(...coDiem),
    }
  }, [dsDiem])

  const capNhat = (maHV: string, field: keyof Diem, value: string) => {
    const num = value === '' ? null : Math.max(0, Math.min(10, Number(value)))
    setDsDiem((prev) =>
      prev.map((d) => (d.maHV === maHV ? { ...d, [field]: num } : d)),
    )
  }

  const luu = async () => {
    if (!lopChon) return
    setDangLuu(true)
    setThongBao('')
    try {
      await goiAPI.luuBangDiem(
        lopChon,
        dsDiem.map((d) => ({
          maHV: d.maHV,
          giuaKy: d.giuaKy,
          cuoiKy: d.cuoiKy,
          baiTap: d.baiTap,
        })),
      )
      setThongBao('✅ Đã lưu bảng điểm thành công!')
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
        <h1 className="text-3xl font-bold text-slate-900">Bảng điểm</h1>
        <p className="mt-2 text-sm text-slate-500">
          Nhập điểm giữa kỳ, cuối kỳ và bài tập. Trung bình được tính tự động.
        </p>
      </div>

      {/* Bộ chọn lớp */}
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

        {lopHienTai && (
          <div className="rounded-xl bg-blue-50 p-3">
            <div className="text-xs font-semibold uppercase text-blue-700">
              Lớp đang chọn
            </div>
            <div className="mt-1 text-lg font-bold text-blue-700">
              {lopHienTai.tenLop}
            </div>
            <div className="text-xs text-slate-500">
              {lopHienTai.siSo}/{lopHienTai.siSoToiDa} học viên · Phòng {lopHienTai.phong}
            </div>
          </div>
        )}
      </div>

      {/* Thống kê */}
      {dsDiem.length > 0 && (
        <div className="mb-6 grid gap-4 sm:grid-cols-3">
          <div className="rounded-2xl border border-blue-200 bg-blue-50 p-4 shadow-sm">
            <div className="text-xs font-semibold uppercase text-blue-700">
              Điểm TB lớp
            </div>
            <div className="mt-1 text-2xl font-bold text-blue-700">
              {thongKe.tb !== null ? thongKe.tb.toFixed(2) : '—'}
            </div>
          </div>
          <div className="rounded-2xl border border-blue-200 bg-white p-4 shadow-sm">
            <div className="text-xs font-semibold uppercase text-slate-500">
              Điểm cao nhất
            </div>
            <div className="mt-1 text-2xl font-bold text-slate-900">
              {thongKe.caoNhat !== null ? thongKe.caoNhat.toFixed(2) : '—'}
            </div>
          </div>
          <div className="rounded-2xl border border-slate-200 bg-white p-4 shadow-sm">
            <div className="text-xs font-semibold uppercase text-slate-500">
              Điểm thấp nhất
            </div>
            <div className="mt-1 text-2xl font-bold text-slate-900">
              {thongKe.thapNhat !== null ? thongKe.thapNhat.toFixed(2) : '—'}
            </div>
          </div>
        </div>
      )}

      {/* Bảng điểm */}
      <div className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
        {dangTai ? (
          <div className="grid place-items-center py-20 text-slate-400">
            Đang tải bảng điểm...
          </div>
        ) : dsDiem.length === 0 ? (
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
                  <th className="px-4 py-3 font-bold">Giữa kỳ (30%)</th>
                  <th className="px-4 py-3 font-bold">Cuối kỳ (50%)</th>
                  <th className="px-4 py-3 font-bold">Bài tập (20%)</th>
                  <th className="px-4 py-3 font-bold">Trung bình</th>
                  <th className="px-4 py-3 font-bold">Xếp loại</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {dsDiem.map((d) => {
                  const tb = tinhTB(d.giuaKy, d.cuoiKy, d.baiTap)
                  const xl = xepLoai(tb)
                  return (
                    <tr key={d.maHV} className="hover:bg-slate-50">
                      <td className="px-4 py-3 font-semibold text-blue-600">
                        {d.maHV}
                      </td>
                      <td className="px-4 py-3">
                        <div className="flex items-center gap-3">
                          <span className="grid size-8 place-items-center rounded-full bg-blue-100 text-xs font-bold text-blue-700">
                            {d.avatar}
                          </span>
                          <span className="font-medium text-slate-900">
                            {d.hoTen}
                          </span>
                        </div>
                      </td>
                      <td className="px-4 py-3">
                        <input
                          type="number"
                          min={0}
                          max={10}
                          step={0.5}
                          value={d.giuaKy ?? ''}
                          onChange={(e) => capNhat(d.maHV, 'giuaKy', e.target.value)}
                          placeholder="—"
                          className="w-20 rounded-lg border border-slate-200 px-2 py-1.5 text-sm outline-none focus:border-blue-500"
                        />
                      </td>
                      <td className="px-4 py-3">
                        <input
                          type="number"
                          min={0}
                          max={10}
                          step={0.5}
                          value={d.cuoiKy ?? ''}
                          onChange={(e) => capNhat(d.maHV, 'cuoiKy', e.target.value)}
                          placeholder="—"
                          className="w-20 rounded-lg border border-slate-200 px-2 py-1.5 text-sm outline-none focus:border-blue-500"
                        />
                      </td>
                      <td className="px-4 py-3">
                        <input
                          type="number"
                          min={0}
                          max={10}
                          step={0.5}
                          value={d.baiTap ?? ''}
                          onChange={(e) => capNhat(d.maHV, 'baiTap', e.target.value)}
                          placeholder="—"
                          className="w-20 rounded-lg border border-slate-200 px-2 py-1.5 text-sm outline-none focus:border-blue-500"
                        />
                      </td>
                      <td className="px-4 py-3">
                        <span className="text-base font-bold text-slate-900">
                          {tb !== null ? tb.toFixed(2) : '—'}
                        </span>
                      </td>
                      <td className="px-4 py-3">
                        <span className={`text-xs font-bold ${xl.color}`}>
                          {xl.label}
                        </span>
                      </td>
                    </tr>
                  )
                })}
              </tbody>
            </table>
          </div>
        )}

        {/* Footer — Nút Lưu */}
        {dsDiem.length > 0 && (
          <div className="flex flex-wrap items-center justify-between gap-3 border-t border-slate-100 bg-slate-50 px-4 py-4">
            <span className="text-sm text-slate-500">
              <b>{dsDiem.length}</b> học viên · Điểm TB lớp:{' '}
              <b className="text-blue-600">
                {thongKe.tb !== null ? thongKe.tb.toFixed(2) : '—'}
              </b>
            </span>
            <button
              onClick={luu}
              disabled={dangLuu}
              className="flex items-center gap-2 rounded-xl bg-blue-600 px-5 py-2.5 text-sm font-bold text-white shadow-sm shadow-blue-200 transition hover:bg-blue-700 disabled:opacity-50"
            >
              <Icon name="check" className="size-4" />
              {dangLuu ? 'Đang lưu...' : 'Lưu bảng điểm'}
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