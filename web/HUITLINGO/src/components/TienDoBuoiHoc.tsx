// Tiến độ buổi học của một lớp — dùng chung cho Admin, Giáo viên và Học viên.
// Màu sắc: đã học = xanh lá, buổi thi = tím, buổi hiện tại = viền xanh dương,
// đã hủy = đỏ nhạt + gạch ngang, chưa tới = xám nhạt.
import { Icon } from './BieuTuong'
import type { ClassProgress, SessionProgressItem } from '../lib/api/types'
import { dinhDangKhoangGio, dinhDangNgay } from '../lib/format'

/** Class màu cho một ô buổi học. */
export function lopMauBuoi(b: SessionProgressItem): string {
  if (b.trangThai === 'Đã hủy') {
    return 'border-rose-200 bg-rose-50 text-rose-500 line-through'
  }

  if (b.laBuoiHienTai) {
    return b.laBuoiThi
      ? 'border-violet-500 bg-violet-100 text-violet-800 ring-2 ring-blue-500'
      : 'border-blue-500 bg-blue-50 text-blue-800 ring-2 ring-blue-500'
  }

  if (b.laBuoiThi) return 'border-violet-300 bg-violet-50 text-violet-700'

  if (b.daHoc) return 'border-emerald-200 bg-emerald-50 text-emerald-700'

  return 'border-slate-200 bg-white text-slate-500'
}

/** Nhãn "Buổi N" (fallback theo vị trí nếu backend chưa có soBuoi). */
export function nhanBuoi(b: SessionProgressItem, viTri: number): string {
  return `Buổi ${b.soBuoi ?? viTri + 1}`
}

export function ChuThichMau() {
  const o = 'inline-flex items-center gap-1.5 text-xs font-medium text-slate-500'

  return (
    <div className="flex flex-wrap items-center gap-x-4 gap-y-2">
      <span className={o}>
        <span className="size-3 rounded border border-emerald-200 bg-emerald-50" /> Đã học
      </span>
      <span className={o}>
        <span className="size-3 rounded border border-slate-200 bg-white" /> Chưa tới
      </span>
      <span className={o}>
        <span className="size-3 rounded border border-violet-300 bg-violet-50" /> Buổi thi
      </span>
      <span className={o}>
        <span className="size-3 rounded border-2 border-blue-500 bg-blue-50" /> Buổi hiện tại
      </span>
      <span className={o}>
        <span className="size-3 rounded border border-rose-200 bg-rose-50" /> Đã hủy
      </span>
    </div>
  )
}

export function TienDoBuoiHoc({
  progress,
  gonGang = false,
  onChonBuoi,
}: {
  progress: ClassProgress
  /** true: chỉ hiện thanh tiến độ + lưới ô (dùng trong trang tổng quan). */
  gonGang?: boolean
  onChonBuoi?: (buoi: SessionProgressItem) => void
}) {
  const phanTram = Math.max(0, Math.min(100, Number(progress.tienDoPhanTram) || 0))

  return (
    <div className="space-y-4">
      {!gonGang && (
        <div className="flex flex-wrap items-end justify-between gap-3">
          <div>
            <p className="text-xs font-bold uppercase tracking-wider text-slate-500">
              Tiến độ khóa học
            </p>
            <p className="mt-1 text-2xl font-bold text-slate-900">
              {progress.soBuoiDaHoc}
              <span className="text-base font-semibold text-slate-400">
                /{progress.tongSoBuoi} buổi
              </span>
            </p>
            {progress.soBuoiHienTai ? (
              <p className="mt-1 flex items-center gap-1.5 text-sm text-slate-500">
                <Icon name="play" className="size-3.5 text-blue-600" />
                Buổi hiện tại: <strong className="text-slate-700">Buổi {progress.soBuoiHienTai}</strong>
                {progress.ngayHocHienTai ? ` · ${dinhDangNgay(progress.ngayHocHienTai)}` : ''}
                {progress.loaiBuoiHienTai ? ` · ${progress.loaiBuoiHienTai}` : ''}
              </p>
            ) : (
              <p className="mt-1 text-sm text-slate-400">Lớp chưa sinh buổi học nào.</p>
            )}
          </div>

          <div className="text-right">
            <p className="text-xs font-semibold text-slate-400">Hoàn thành</p>
            <p className="text-xl font-bold text-blue-600">{phanTram}%</p>
          </div>
        </div>
      )}

      <div className="h-2.5 w-full overflow-hidden rounded-full bg-slate-100">
        <div
          className="h-full rounded-full bg-blue-600 transition-all"
          style={{ width: `${phanTram}%` }}
        />
      </div>

      {progress.buois.length === 0 ? (
        <p className="rounded-xl border border-dashed border-slate-300 bg-white p-6 text-center text-sm text-slate-400">
          Chưa có buổi học. Hãy sinh buổi từ lịch định kỳ.
        </p>
      ) : (
        <div className="grid grid-cols-3 gap-2 sm:grid-cols-5 lg:grid-cols-10">
          {progress.buois.map((b, viTri) => {
            const noiDung = [
              nhanBuoi(b, viTri),
              dinhDangNgay(b.ngayHoc),
              dinhDangKhoangGio(b.gioBatDau, b.gioKetThuc),
              b.loaiBuoi,
              b.trangThai,
              b.noiDungBaiHoc,
              b.tenGiaoVienThucTe ? `GV: ${b.tenGiaoVienThucTe}` : null,
              b.tenTroGiangThucTe ? `TG: ${b.tenTroGiangThucTe}` : null,
              `${b.soDiemDanh} học viên đã điểm danh`,
            ]
              .filter(Boolean)
              .join(' · ')

            return (
              <button
                key={b.maCaHoc}
                type="button"
                onClick={() => onChonBuoi?.(b)}
                title={noiDung}
                className={`flex flex-col items-start gap-0.5 rounded-xl border px-2.5 py-2 text-left transition hover:shadow-sm ${lopMauBuoi(b)}`}
              >
                <span className="text-xs font-bold">{nhanBuoi(b, viTri)}</span>
                <span className="text-[11px] font-medium opacity-80">
                  {dinhDangNgay(b.ngayHoc)}
                </span>
                {b.laBuoiThi && (
                  <span className="mt-0.5 rounded bg-violet-600 px-1.5 py-0.5 text-[10px] font-bold text-white">
                    THI
                  </span>
                )}
              </button>
            )
          })}
        </div>
      )}

      {!gonGang && <ChuThichMau />}
    </div>
  )
}
