import { useEffect, useState } from 'react'
import { Icon } from '../../components/BieuTuong'
import { goiAPI, type ScheduleItem } from '../../lib/goiAPI'
import { dungXacThuc } from '../../contexts/NguCanhXacThuc'

export default function LichHoc() {
  const { nguoiDung } = dungXacThuc()
  const [dsLich, setDsLich] = useState<ScheduleItem[]>([])
  const [dangTai, setDangTai] = useState(true)

  useEffect(() => {
    if (!nguoiDung) return
    setDangTai(true)
    goiAPI.layLichHocCuaHocVien(nguoiDung.id).then((data) => {
      setDsLich(data)
      setDangTai(false)
    })
  }, [nguoiDung])

  if (dangTai) {
    return (
      <div className="grid place-items-center py-20 text-slate-400">
        Đang tải lịch học...
      </div>
    )
  }

  return (
    <div className="space-y-6">
      <div>
        <p className="mb-1 text-sm font-semibold text-blue-600">Học tập</p>
        <h1 className="text-3xl font-bold text-slate-900">Lịch học của tôi</h1>
        <p className="mt-2 text-sm text-slate-500">
          {dsLich.length} lớp đang theo học · Xem lịch chi tiết bên dưới.
        </p>
      </div>

      {dsLich.length === 0 ? (
        <div className="rounded-2xl border border-dashed border-slate-300 bg-white p-12 text-center text-slate-400">
          Bạn chưa có lịch học nào
        </div>
      ) : (
        <div className="space-y-4">
          {dsLich.map((lich) => (
            <article
              key={lich.id}
              className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm transition hover:border-blue-200 hover:shadow-md"
            >
              <div className="flex flex-wrap items-start justify-between gap-3">
                <div className="flex items-start gap-4">
                  <span className="grid size-12 place-items-center rounded-xl bg-blue-50 text-blue-600">
                    <Icon name="book" className="size-6" />
                  </span>
                  <div>
                    <h3 className="text-lg font-bold text-slate-900">
                      {lich.tenLop}
                    </h3>
                    <p className="mt-1 text-sm font-semibold text-blue-600">
                      {lich.thu}
                    </p>
                  </div>
                </div>
                <span className="rounded-full bg-blue-600 px-3 py-1 text-xs font-bold text-white">
                  {lich.gio}
                </span>
              </div>

              <div className="mt-4 flex flex-wrap gap-4 text-sm text-slate-600">
                <div className="flex items-center gap-2">
                  <Icon name="room" className="size-4 text-slate-400" />
                  Phòng {lich.phong}
                </div>
                <div className="flex items-center gap-2">
                  <Icon name="users" className="size-4 text-slate-400" />
                  {lich.siSo} học viên
                </div>
              </div>
            </article>
          ))}
        </div>
      )}
    </div>
  )
}