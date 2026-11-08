// Quản trị: KỲ NGHỈ / NGÀY LỄ của trung tâm.
// Buổi học rơi vào kỳ nghỉ sẽ tự dời sang tuần sau khi bấm "Sinh ca học".
import { useCallback, useEffect, useMemo, useState } from 'react'
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
  TheChiSo,
  TrangTieuDe,
  type CotBang,
} from '../../components/ui'
import { dungToast } from '../../components/ui/toast'
import { ngayLeApi } from '../../lib/api/endpoints'
import type { Holiday } from '../../lib/api/types'
import { dinhDangNgay, homNayIso } from '../../lib/format'
import { thongDiepLoi } from '../../lib/useApi'

type FormNgayLe = {
  ngay: string
  denNgay: string
  tenNgayLe: string
  ghiChu: string
}

const FORM_RONG = (): FormNgayLe => ({
  ngay: homNayIso(),
  denNgay: '',
  tenNgayLe: '',
  ghiChu: '',
})

/** Đếm số ngày nghỉ (bao gồm cả ngày đầu và ngày cuối). */
function soNgayNghi(holiday: Holiday): number {
  const den = holiday.denNgay ?? holiday.ngay
  const [y1, m1, d1] = holiday.ngay.split('-').map(Number)
  const [y2, m2, d2] = den.split('-').map(Number)

  if (!y1 || !y2) return 1

  const tu = Date.UTC(y1, m1 - 1, d1)
  const toi = Date.UTC(y2, m2 - 1, d2)

  return Math.max(1, Math.round((toi - tu) / 86_400_000) + 1)
}

export function AdminNgayLe() {
  const { hien } = dungToast()

  const [ds, setDs] = useState<Holiday[]>([])
  const [dangTai, setDangTai] = useState(true)
  const [loi, setLoi] = useState<string | null>(null)

  const [moForm, setMoForm] = useState(false)
  const [dangSua, setDangSua] = useState<Holiday | null>(null)
  const [form, setForm] = useState<FormNgayLe>(FORM_RONG)
  const [loiForm, setLoiForm] = useState<string | null>(null)
  const [dangLuu, setDangLuu] = useState(false)

  const tai = useCallback(async () => {
    setDangTai(true)
    setLoi(null)
    try {
      setDs(await ngayLeApi.danhSach())
    } catch (error) {
      setLoi(thongDiepLoi(error))
    } finally {
      setDangTai(false)
    }
  }, [])

  useEffect(() => {
    void tai()
  }, [tai])

  const homNay = homNayIso()

  const tongNgayNghi = useMemo(
    () => ds.reduce((tong, h) => tong + soNgayNghi(h), 0),
    [ds],
  )

  const dangNghi = useMemo(
    () =>
      ds.find((h) => h.ngay <= homNay && (h.denNgay ?? h.ngay) >= homNay) ?? null,
    [ds, homNay],
  )

  const moThem = () => {
    setDangSua(null)
    setForm(FORM_RONG())
    setLoiForm(null)
    setMoForm(true)
  }

  const moSua = (holiday: Holiday) => {
    setDangSua(holiday)
    setForm({
      ngay: holiday.ngay,
      denNgay: holiday.denNgay ?? '',
      tenNgayLe: holiday.tenNgayLe,
      ghiChu: holiday.ghiChu ?? '',
    })
    setLoiForm(null)
    setMoForm(true)
  }

  const luu = async () => {
    const ten = form.tenNgayLe.trim()

    if (!form.ngay) {
      setLoiForm('Vui lòng chọn ngày bắt đầu kỳ nghỉ.')
      return
    }

    if (ten.length === 0) {
      setLoiForm('Vui lòng nhập tên kỳ nghỉ.')
      return
    }

    if (form.denNgay && form.denNgay < form.ngay) {
      setLoiForm('Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.')
      return
    }

    // Nghỉ đúng 1 ngày thì bỏ mốc kết thúc cho gọn dữ liệu.
    const denNgay =
      form.denNgay && form.denNgay !== form.ngay ? form.denNgay : null

    setDangLuu(true)
    setLoiForm(null)

    try {
      if (dangSua) {
        await ngayLeApi.capNhat(dangSua.maNgayLe, {
          ngay: form.ngay,
          denNgay,
          xoaDenNgay: denNgay === null,
          tenNgayLe: ten,
          ghiChu: form.ghiChu.trim() || null,
        })
        hien('Đã cập nhật kỳ nghỉ.', 'thanhCong')
      } else {
        await ngayLeApi.them({
          ngay: form.ngay,
          denNgay,
          tenNgayLe: ten,
          ghiChu: form.ghiChu.trim() || null,
        })
        hien('Đã thêm kỳ nghỉ.', 'thanhCong')
      }

      setMoForm(false)
      void tai()
    } catch (error) {
      setLoiForm(thongDiepLoi(error))
    } finally {
      setDangLuu(false)
    }
  }

  const xoa = async (holiday: Holiday) => {
    const khoang = holiday.denNgay
      ? `từ ${dinhDangNgay(holiday.ngay)} đến ${dinhDangNgay(holiday.denNgay)}`
      : `ngày ${dinhDangNgay(holiday.ngay)}`

    if (!window.confirm(`Xóa kỳ nghỉ ${khoang} — ${holiday.tenNgayLe}?`)) {
      return
    }

    try {
      await ngayLeApi.xoa(holiday.maNgayLe)
      hien('Đã xóa kỳ nghỉ.', 'thanhCong')
      void tai()
    } catch (error) {
      hien(thongDiepLoi(error), 'loi')
    }
  }

  const cot: CotBang<Holiday>[] = [
    {
      tieuDe: 'Khoảng nghỉ',
      khoa: 'ngay',
      render: (h) =>
        h.denNgay ? (
          <span className="font-semibold whitespace-nowrap text-slate-900">
            {dinhDangNgay(h.ngay)} – {dinhDangNgay(h.denNgay)}
          </span>
        ) : (
          <span className="font-semibold whitespace-nowrap text-slate-900">
            {dinhDangNgay(h.ngay)}
          </span>
        ),
    },
    {
      tieuDe: 'Số ngày',
      khoa: 'soNgay',
      canPhai: true,
      render: (h) => `${soNgayNghi(h)} ngày`,
    },
    {
      tieuDe: 'Tên kỳ nghỉ',
      khoa: 'tenNgayLe',
      render: (h) => (
        <div className="flex items-center gap-2">
          <span>{h.tenNgayLe}</span>
          {h.ngay <= homNay && (h.denNgay ?? h.ngay) >= homNay && (
            <Nhan tone="emerald">Đang nghỉ</Nhan>
          )}
        </div>
      ),
    },
    {
      tieuDe: 'Ghi chú',
      khoa: 'ghiChu',
      render: (h) => h.ghiChu || '—',
    },
    {
      tieuDe: '',
      khoa: 'thaoTac',
      canPhai: true,
      render: (h) => (
        <div className="flex justify-end gap-2">
          <button
            type="button"
            onClick={() => moSua(h)}
            className="rounded-lg border border-slate-200 px-3 py-1.5 text-xs font-bold text-slate-600 transition hover:border-blue-200 hover:text-blue-600"
          >
            Sửa
          </button>
          <button
            type="button"
            onClick={() => void xoa(h)}
            className="rounded-lg border border-rose-200 px-3 py-1.5 text-xs font-bold text-rose-600 transition hover:bg-rose-50"
          >
            Xóa
          </button>
        </div>
      ),
    },
  ]

  if (dangTai) return <DangTai nhan="Đang tải danh sách kỳ nghỉ…" />
  if (loi) return <LoiTrang message={loi} onThuLai={tai} />

  return (
    <div className="space-y-6">
      <TrangTieuDe
        eyebrow="Quản trị học vụ"
        title="Kỳ nghỉ & ngày lễ"
        desc="Kỳ nghỉ có thể kéo dài nhiều ngày. Khi sinh ca học, buổi rơi vào kỳ nghỉ sẽ tự dời sang tuần sau."
        actions={
          <button type="button" onClick={moThem} className={LOP_NUT_CHINH}>
            <Icon name="calendar" className="size-4" /> Thêm kỳ nghỉ
          </button>
        }
      />

      <div className="grid gap-4 sm:grid-cols-3">
        <TheChiSo
          nhan="Số kỳ nghỉ"
          giaTri={ds.length}
          moTa="Đã khai trong hệ thống"
          icon="calendarRange"
          tone="blue"
        />
        <TheChiSo
          nhan="Tổng ngày nghỉ"
          giaTri={tongNgayNghi}
          moTa="Cộng dồn mọi kỳ nghỉ"
          icon="timer"
          tone="amber"
        />
        <TheChiSo
          nhan="Hôm nay"
          giaTri={dangNghi ? 'Đang nghỉ' : 'Đi học bình thường'}
          moTa={dangNghi ? dangNghi.tenNgayLe : 'Không có kỳ nghỉ nào'}
          icon="check"
          tone={dangNghi ? 'rose' : 'emerald'}
        />
      </div>

      <Bang
        cot={cot}
        duLieu={ds}
        khoa={(h) => h.maNgayLe}
        thongBaoRong="Chưa khai kỳ nghỉ nào. Bấm 'Thêm kỳ nghỉ' để bắt đầu."
      />

      <HopThoai
        mo={moForm}
        tieuDe={dangSua ? 'Sửa kỳ nghỉ' : 'Thêm kỳ nghỉ'}
        onDong={() => !dangLuu && setMoForm(false)}
        footer={
          <>
            <button
              type="button"
              onClick={() => setMoForm(false)}
              disabled={dangLuu}
              className={LOP_NUT_PHU}
            >
              Hủy
            </button>
            <button
              type="button"
              onClick={() => void luu()}
              disabled={dangLuu}
              className={LOP_NUT_CHINH}
            >
              {dangLuu ? 'Đang lưu…' : 'Lưu'}
            </button>
          </>
        }
      >
        <div className="space-y-4">
          <div className="grid gap-4 sm:grid-cols-2">
            <div>
              <label className={LOP_NHAN} htmlFor="ngay-bat-dau">
                Từ ngày *
              </label>
              <input
                id="ngay-bat-dau"
                type="date"
                value={form.ngay}
                onChange={(e) => setForm({ ...form, ngay: e.target.value })}
                className={LOP_INPUT}
              />
            </div>
            <div>
              <label className={LOP_NHAN} htmlFor="ngay-ket-thuc">
                Đến ngày (bỏ trống = nghỉ 1 ngày)
              </label>
              <input
                id="ngay-ket-thuc"
                type="date"
                value={form.denNgay}
                min={form.ngay}
                onChange={(e) => setForm({ ...form, denNgay: e.target.value })}
                className={LOP_INPUT}
              />
            </div>
          </div>

          <div>
            <label className={LOP_NHAN} htmlFor="ten-ky-nghi">
              Tên kỳ nghỉ *
            </label>
            <input
              id="ten-ky-nghi"
              type="text"
              maxLength={100}
              placeholder="Ví dụ: Tết Nguyên đán Bính Ngọ"
              value={form.tenNgayLe}
              onChange={(e) => setForm({ ...form, tenNgayLe: e.target.value })}
              className={LOP_INPUT}
            />
          </div>

          <div>
            <label className={LOP_NHAN} htmlFor="ghi-chu">
              Ghi chú
            </label>
            <input
              id="ghi-chu"
              type="text"
              maxLength={200}
              placeholder="Trung tâm nghỉ toàn bộ"
              value={form.ghiChu}
              onChange={(e) => setForm({ ...form, ghiChu: e.target.value })}
              className={LOP_INPUT}
            />
          </div>

          {form.ngay && (
            <p className="rounded-xl bg-slate-50 px-4 py-3 text-sm text-slate-600">
              Kỳ nghỉ này kéo dài{' '}
              <strong className="text-slate-900">
                {soNgayNghi({
                  maNgayLe: 0,
                  ngay: form.ngay,
                  denNgay: form.denNgay || null,
                  tenNgayLe: '',
                })}{' '}
                ngày
              </strong>
              . Không được chồng lấn với kỳ nghỉ đã khai.
            </p>
          )}

          {loiForm && (
            <p className="rounded-xl border border-rose-200 bg-rose-50 px-4 py-3 text-sm font-semibold text-rose-700">
              {loiForm}
            </p>
          )}
        </div>
      </HopThoai>
    </div>
  )
}
