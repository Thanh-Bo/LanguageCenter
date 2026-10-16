import { useEffect, useState } from 'react'
import { Icon, type IconName } from '../../components/BieuTuong'
import { goiAPI, type ClassItem } from '../../lib/goiAPI'
import { dungXacThuc } from '../../contexts/NguCanhXacThuc'

type DiemItem = {
  maLop: string
  tenLop: string
  giuaKy: number | null
  cuoiKy: number | null
  baiTap: number | null
  trungBinh: number | null
}

type Props = {
  onChuyenTrang?: (trang: string) => void
}

function CardThongKe({
  label,
  value,
  icon,
  mau,
}: {
  label: string
  value: string
  icon: IconName
  mau: string
}) {
  return (
    <article className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
      <span className={`grid size-11 place-items-center rounded-xl ${mau}`}>
        <Icon name={icon} className="size-5" />
      </span>
      <p className="mt-5 text-sm font-medium text-slate-500">{label}</p>
      <p className="mt-1 text-3xl font-bold text-slate-900">{value}</p>
    </article>
  )
}

export default function TongQuanHocVien({ onChuyenTrang }: Props) {
  const { nguoiDung } = dungXacThuc()
  const [dsLop, setDsLop] = useState<ClassItem[]>([])
  const [dsDiem, setDsDiem] = useState<DiemItem[]>([])
  const [dangTai, setDangTai] = useState(true)

  useEffect(() => {
    if (!nguoiDung) return
    setDangTai(true)
    Promise.all([
      goiAPI.layLopCuaHocVien(nguoiDung.id),
      goiAPI.layDiemCuaHocVien(nguoiDung.id),
    ]).then(([lop, diem]) => {
      setDsLop(lop)
      setDsDiem(diem)
      setDangTai(false)
    })
  }, [nguoiDung])

  const diemTrungBinh =
    dsDiem.filter((d) => d.trungBinh !== null).length > 0
      ? (
          dsDiem
            .filter((d) => d.trungBinh !== null)
            .reduce((sum, d) => sum + (d.trungBinh ?? 0), 0) /
          dsDiem.filter((d) => d.trungBinh !== null).length
        ).toFixed(2)
      : '—'

  const ngayHomNay = new Date().toLocaleDateString('vi-VN', {
    weekday: 'long',
    day: 'numeric',
    month: 'long',
  })

  if (dangTai) {
    return (
      <div className="grid place-items-center py-20 text-slate-400">
        Đang tải dữ liệu...
      </div>
    )
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <section className="flex flex-col justify-between gap-4 sm:flex-row sm:items-end">
        <div>
          <p className="mb-1 text-sm font-semibold text-blue-600 capitalize">
            {ngayHomNay}
          </p>
          <h1 className="text-3xl font-bold text-slate-900">
            Chào bạn, {nguoiDung?.hoTen ?? 'Học viên'} 👋
          </h1>
          <p className="mt-2 text-sm text-slate-500">
            Đây là tình hình học tập của bạn hôm nay.
          </p>
        </div>
        <button
          onClick={() => onChuyenTrang?.('Lịch học')}
          className="flex h-11 items-center justify-center gap-2 rounded-xl bg-blue-600 px-5 text-sm font-bold text-white shadow-lg shadow-blue-100 hover:bg-blue-700"
        >
          <Icon name="book" className="size-4" /> Vào lớp học
        </button>
      </section>

      {/* 4 card thống kê */}
      <section className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
        <CardThongKe label="Lớp đang học" value={String(dsLop.length)} icon="book" mau="bg-blue-600 text-white" />
        <CardThongKe label="Điểm trung bình" value={String(diemTrungBinh)} icon="chart" mau="bg-blue-600 text-white" />
        <CardThongKe label="Buổi học tuần này" value="3" icon="calendar" mau="bg-blue-600 text-white" />
        <CardThongKe label="Chuyên cần" value="96%" icon="check" mau="bg-blue-600 text-white" />
      </section>

      <section className="grid gap-6 xl:grid-cols-[1.55fr_.85fr]">
        {/* Lớp đang học */}
        <article className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
          <div className="flex items-center justify-between border-b border-slate-100 p-5">
            <div>
              <h2 className="text-xl font-bold text-slate-900">Lớp đang học</h2>
              <p className="mt-1 text-xs text-slate-500">
                {dsLop.length} lớp đang theo học
              </p>
            </div>
            <button
              onClick={() => onChuyenTrang?.('Lịch học')}
              className="text-sm font-bold text-blue-600 hover:text-blue-700"
            >
              Xem tất cả →
            </button>
          </div>
          <div className="divide-y divide-slate-100">
            {dsLop.map((lop) => (
              <div key={lop.id} className="p-5">
                <div className="flex items-start justify-between gap-3">
                  <div>
                    <h3 className="text-lg font-bold text-slate-900">{lop.tenLop}</h3>
                    <span className="mt-1 inline-block rounded-full bg-blue-50 px-2.5 py-0.5 text-xs font-semibold text-blue-700">
                      {lop.maLop} · {lop.capDo}
                    </span>
                  </div>
                  <span className="rounded-full bg-blue-600 px-2.5 py-1 text-xs font-bold text-white">
                    {lop.trangThai}
                  </span>
                </div>
                <div className="mt-3 space-y-1.5 text-sm text-slate-600">
                  <div className="flex items-center gap-2">
                    <Icon name="clock" className="size-4 text-slate-400" />
                    {lop.lichHoc}
                  </div>
                  <div className="flex items-center gap-2">
                    <Icon name="room" className="size-4 text-slate-400" />
                    Phòng {lop.phong}
                  </div>
                </div>
                <div className="mt-4">
                  <div className="mb-1 flex justify-between text-xs text-slate-500">
                    <span>Tiến độ khóa học</span>
                    <span className="font-semibold text-blue-600">{lop.tienDo}%</span>
                  </div>
                  <div className="h-2 w-full rounded-full bg-blue-50">
                    <div className="h-2 rounded-full bg-blue-600" style={{ width: `${lop.tienDo}%` }} />
                  </div>
                </div>
              </div>
            ))}
            {dsLop.length === 0 && (
              <div className="p-8 text-center text-slate-400">
                Bạn chưa đăng ký lớp nào
              </div>
            )}
          </div>
        </article>

        {/* Điểm gần đây */}
        <div className="space-y-6">
          <article className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
            <div className="flex items-center justify-between">
              <h2 className="text-lg font-bold">Điểm gần đây</h2>
              <button
                onClick={() => onChuyenTrang?.('Điểm số')}
                className="text-xs font-bold text-blue-600 hover:text-blue-700"
              >
                Xem tất cả →
              </button>
            </div>
            <div className="mt-4 space-y-3">
              {dsDiem.slice(0, 3).map((d) => (
                <div key={d.maLop} className="rounded-xl border border-slate-100 p-3">
                  <div className="flex items-center justify-between">
                    <strong className="text-sm">{d.tenLop}</strong>
                    <span className="rounded-full bg-blue-600 px-2 py-0.5 text-xs font-bold text-white">
                      {d.trungBinh?.toFixed(1) ?? '—'}
                    </span>
                  </div>
                  <div className="mt-2 grid grid-cols-3 gap-2 text-xs text-slate-500">
                    <span>GK: {d.giuaKy ?? '—'}</span>
                    <span>CK: {d.cuoiKy ?? '—'}</span>
                    <span>BT: {d.baiTap ?? '—'}</span>
                  </div>
                </div>
              ))}
              {dsDiem.length === 0 && (
                <p className="py-4 text-center text-sm text-slate-400">Chưa có điểm</p>
              )}
            </div>
          </article>

          <article className="overflow-hidden rounded-2xl bg-blue-700 p-5 text-white shadow-lg shadow-blue-200">
            <div className="flex items-start justify-between">
              <span className="grid size-10 place-items-center rounded-xl bg-white/15">
                <Icon name="sparkles" className="size-5" />
              </span>
              <span className="rounded-full bg-white/15 px-2 py-1 text-[10px] font-bold">
                GỢI Ý
              </span>
            </div>
            <h3 className="mt-5 text-lg font-bold">Học tập hiệu quả hơn</h3>
            <p className="mt-2 text-sm leading-6 text-blue-100">
              Bạn còn 2 bài tập chưa hoàn thành. Hãy dành 30 phút mỗi ngày để ôn luyện.
            </p>
            <button
              onClick={() => onChuyenTrang?.('Điểm số')}
              className="mt-4 flex items-center gap-2 text-sm font-bold hover:underline"
            >
              Xem bài tập <Icon name="arrowRight" className="size-4" />
            </button>
          </article>
        </div>
      </section>
    </div>
  )
}