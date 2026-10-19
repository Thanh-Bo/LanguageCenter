// Hook tải dữ liệu bất đồng bộ kèm trạng thái loading/lỗi và hàm tải lại.
import { useCallback, useEffect, useRef, useState } from 'react'
import { ApiError } from './api/http'

/** Chuyển lỗi bất kỳ thành câu thông báo hiển thị được cho người dùng. */
export function thongDiepLoi(loi: unknown): string {
  if (loi instanceof ApiError) {
    if (loi.errors.length > 0) return loi.errors.join(' · ')
    return loi.message || `Yêu cầu thất bại (HTTP ${loi.status})`
  }
  if (loi instanceof Error) return loi.message
  return 'Đã xảy ra lỗi không xác định.'
}

export function dungDuLieu<T>(tai: () => Promise<T>) {
  const [duLieu, setDuLieu] = useState<T | null>(null)
  const [dangTai, setDangTai] = useState(true)
  const [loi, setLoi] = useState<string | null>(null)

  const lanTai = useRef(0)
  const hamTai = useRef(tai)
  hamTai.current = tai

  const taiLai = useCallback(async () => {
    const lan = ++lanTai.current
    setDangTai(true)
    setLoi(null)

    try {
      const ketQua = await hamTai.current()
      if (lan === lanTai.current) setDuLieu(ketQua)
    } catch (error) {
      if (lan === lanTai.current) setLoi(thongDiepLoi(error))
    } finally {
      if (lan === lanTai.current) setDangTai(false)
    }
  }, [])

  useEffect(() => {
    void taiLai()
  }, [taiLai])

  return { duLieu, dangTai, loi, taiLai }
}
