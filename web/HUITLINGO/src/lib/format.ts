const formatterTien = new Intl.NumberFormat('vi-VN')

/** 5700000 -> "5.700.000 ₫" */
export function dinhDangTien(value?: number | null): string {
  if (value === null || value === undefined) return '—'
  return `${formatterTien.format(value)} ₫`
}

/** 5700000 -> "5.700.000" (không kèm ký hiệu) */
export function dinhDangSo(value?: number | null): string {
  if (value === null || value === undefined) return '—'
  return formatterTien.format(value)
}

/**
 * DateOnly backend trả "2026-01-05".
 * KHÔNG dùng new Date() vì sẽ lệch múi giờ.
 */
export function dinhDangNgay(value?: string | null): string {
  if (!value) return '—'

  const phan = value.split('T')[0].split('-')
  if (phan.length !== 3) return value

  const [nam, thang, ngay] = phan
  return `${ngay}/${thang}/${nam}`
}

/** TimeOnly backend trả "18:00:00" -> "18:00" */
export function dinhDangGio(value?: string | null): string {
  if (!value) return '—'
  return value.slice(0, 5)
}

/** DateTime backend trả "2026-09-25T20:30:00" */
export function dinhDangNgayGio(value?: string | null): string {
  if (!value) return '—'

  const [ngay, gio] = value.split('T')
  return gio ? `${dinhDangNgay(ngay)} ${dinhDangGio(gio)}` : dinhDangNgay(ngay)
}

/** "18:00:00" + "19:30:00" -> "18:00 – 19:30" */
export function dinhDangKhoangGio(batDau?: string | null, ketThuc?: string | null): string {
  if (!batDau || !ketThuc) return '—'
  return `${dinhDangGio(batDau)} – ${dinhDangGio(ketThuc)}`
}

/** Chữ viết tắt cho avatar: "Nguyễn Lan Anh" -> "LA" */
export function chuVietTat(hoTen?: string | null): string {
  if (!hoTen) return '?'

  const tu = hoTen.trim().split(/\s+/)
  if (tu.length === 1) return tu[0].slice(0, 2).toUpperCase()

  return (tu[0][0] + tu[tu.length - 1][0]).toUpperCase()
}

/** Ngày ISO hôm nay, dùng cho <input type="date"> và query backend. */
export function homNayIso(): string {
  const now = new Date()
  const thang = String(now.getMonth() + 1).padStart(2, '0')
  const ngay = String(now.getDate()).padStart(2, '0')
  return `${now.getFullYear()}-${thang}-${ngay}`
}

/** "yyyy-MM-dd" của N ngày trước hôm nay. */
export function ngayTruocIso(soNgay: number): string {
  const d = new Date()
  d.setDate(d.getDate() - soNgay)

  const thang = String(d.getMonth() + 1).padStart(2, '0')
  const ngay = String(d.getDate()).padStart(2, '0')
  return `${d.getFullYear()}-${thang}-${ngay}`
}

/** Tên thứ trong tuần tiếng Việt của một DateOnly "yyyy-MM-dd". */
export function thuTrongTuan(value: string): string {
  const [nam, thang, ngay] = value.split('-').map(Number)
  const d = new Date(nam, thang - 1, ngay)
  return ['Chủ nhật', 'Thứ 2', 'Thứ 3', 'Thứ 4', 'Thứ 5', 'Thứ 6', 'Thứ 7'][d.getDay()]
}

/** "2026-10-19" + 3 -> "2026-10-22". soNgay có thể âm. */
export function congNgayIso(value: string, soNgay: number): string {
  const [nam, thang, ngay] = value.split('-').map(Number)
  const d = new Date(nam, thang - 1, ngay)
  d.setDate(d.getDate() + soNgay)

  const t = String(d.getMonth() + 1).padStart(2, '0')
  const n = String(d.getDate()).padStart(2, '0')
  return `${d.getFullYear()}-${t}-${n}`
}

/** Thứ 2 của tuần chứa ngày cho trước (tuần tính từ Thứ 2 đến Chủ nhật). */
export function dauTuanIso(value: string): string {
  const [nam, thang, ngay] = value.split('-').map(Number)
  const thu = new Date(nam, thang - 1, ngay).getDay() // 0 = Chủ nhật

  return congNgayIso(value, thu === 0 ? -6 : 1 - thu)
}
