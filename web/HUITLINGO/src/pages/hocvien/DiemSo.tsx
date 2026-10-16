import { useEffect, useState } from 'react'
import { Icon } from '../../components/BieuTuong'
import { goiAPI } from '../../lib/goiAPI'
import { dungXacThuc } from '../../contexts/NguCanhXacThuc'

type DiemItem = {
  maLop: string
  tenLop: string
  giuaKy: number | null
  cuoiKy: number | null
  baiTap: number | null
  trungBinh: number | null
}

function xepLoai(tb: number | null): { nhan: string; mau: string } {
  if (tb === null) return { nhan: '—', mau: 'bg-slate-100 text-slate-500' }
  if (tb >= 8.5) return { nhan: 'Xuất sắc', mau: 'bg-emerald-100 text-emerald-700' }
  if (tb >= 7.0) return { nhan: 'Giỏi', mau: 'bg-blue-100 text-blue-700' }
  if (tb >= 5.5) return { nhan: 'Khá', mau: 'bg-amber-100 text-amber-700' }
  if (tb >= 4.0) return { nhan: 'Trung bình', mau: 'bg-orange-100 text-orange-700' }
  return { nhan: 'Yếu', mau: 'bg-rose-100 text-rose-700' }
}

export default function DiemSo() {
  const { nguoiDung } = dungXacThuc()
  const [dsDiem, setDsDiem] = useState<DiemItem[]>([])
  const [dangTai, setDangTai] = useState(true)

  useEffect(() => {
    if (!nguoiDung) return
    setDangTai(true)
    goiAPI.layDiemCuaHocVien(nguoiDung.id).then((data) => {
      setDsDiem(data)
      setDangTai(false)
    })
  }, [nguoiDung])

  const diemHopLe = dsDiem.filter((d) => d.trungBinh !== null)
  const diemTB =
    diemHopLe.length > 0
      ? diemHopLe.reduce((s, d) => s + (d.trungBinh ?? 0), 0) / diemHopLe.length
      : null
  const diemCaoNhat =
    diemHopLe.length > 0
      ? Math.max(...diemHopLe.map((d) => d.trungBinh ?? 0))
      : null
  const diemThapNhat =
    diemHopLe.length > 0
      ? Math.min(...diemHopLe.map((d) => d.trungBinh ?? 0))
      : null

  if (dangTai) {
    return (
      <div className="grid place-items-center py-20 text-slate-400">
        Đang tải bảng điểm...
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div>
        <p className="mb-1 text-sm font-semibold text-blue-600">Kết quả học tập</p>
        <h1 className="text-3xl font-bold text-slate-900">Điểm số của tôi</h1>
        <p className="mt-2 text-sm text-slate-500">
          Điểm được cập nhật liên tục sau mỗi bài kiểm tra.
        </p>
      </div>

      {/* 3 card thống kê */}
      <div className="grid gap-4 sm:grid-cols-3">
        <div className="rounded-2xl border border-slate-200 bg-blue-50 p-5">
          <p className="text-xs font-bold uppercase text-slate-500">
            Điểm trung bình
          </p>
          <p className="mt-2 text-3xl font-bold text-blue-600">
            {diemTB?.toFixed(2) ?? '—'}
          </p>
        </div>
        <div className="rounded-2xl border border-slate-200 bg-white p-5">
          <p className="text-xs font-bold uppercase text-slate-500">
            Điểm cao nhất
          </p>
          <p className="mt-2 text-3xl font-bold text-slate-900">
            {diemCaoNhat?.toFixed(2) ?? '—'}
          </p>
        </div>
        <div className="rounded-2xl border border-slate-200 bg-white p-5">
          <p className="text-xs font-bold uppercase text-slate-500">
            Điểm thấp nhất
          </p>
          <p className="mt-2 text-3xl font-bold text-slate-900">
            {diemThapNhat?.toFixed(2) ?? '—'}
          </p>
        </div>
      </div>

      {/* Bảng điểm */}
      <article className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
        <div className="border-b border-slate-100 p-5">
          <h2 className="text-lg font-bold">Bảng điểm chi tiết</h2>
        </div>
        {dsDiem.length === 0 ? (
          <div className="p-12 text-center text-slate-400">
            Chưa có điểm nào
          </div>
        ) : (
          <div className="overflow-x-auto">
            <table className="w-full border-collapse text-left text-sm">
              <thead>
                <tr className="border-b border-slate-200 bg-slate-50 text-xs uppercase tracking-wider text-slate-500">
                  <th className="px-5 py-3 font-bold">Lớp</th>
                  <th className="px-5 py-3 font-bold">Giữa kỳ (30%)</th>
                  <th className="px-5 py-3 font-bold">Cuối kỳ (50%)</th>
                  <th className="px-5 py-3 font-bold">Bài tập (20%)</th>
                  <th className="px-5 py-3 font-bold">Trung bình</th>
                  <th className="px-5 py-3 font-bold">Xếp loại</th>
                </tr>
              </thead>
              <tbody className="divide-y divide-slate-100">
                {dsDiem.map((d) => {
                  const xl = xepLoai(d.trungBinh)
                  return (
                    <tr key={d.maLop} className="hover:bg-slate-50">
                      <td className="px-5 py-4">
                        <strong className="text-slate-900">{d.tenLop}</strong>
                        <p className="mt-0.5 text-xs text-slate-500">
                          {d.maLop}
                        </p>
                      </td>
                      <td className="px-5 py-4 font-semibold">
                        {d.giuaKy ?? '—'}
                      </td>
                      <td className="px-5 py-4 font-semibold">
                        {d.cuoiKy ?? '—'}
                      </td>
                      <td className="px-5 py-4 font-semibold">
                        {d.baiTap ?? '—'}
                      </td>
                      <td className="px-5 py-4 text-lg font-bold text-blue-600">
                        {d.trungBinh?.toFixed(2) ?? '—'}
                      </td>
                      <td className="px-5 py-4">
                        <span
                          className={`rounded-full px-2.5 py-1 text-xs font-bold ${xl.mau}`}
                        >
                          {xl.nhan}
                        </span>
                      </td>
                    </tr>
                  )
                })}
              </tbody>
            </table>
          </div>
        )}
      </article>
    </div>
  )
}