// src/contexts/NguCanhXacThuc.tsx
import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import { authApi, hoSoApi } from '../lib/api/endpoints'
import { datXuLyUnauthorized, layToken, luuToken, xoaToken } from '../lib/api/http'
import type { VaiTro } from '../lib/api/types'
import { chuVietTat } from '../lib/format'
import { duongDanTheoVaiTro } from '../lib/roles'

export type ApiUser = {
  id: number
  tenDangNhap: string
  hoTen: string
  email: string
  vaiTro: VaiTro
  avatar: string
}

type NguCanhXacThucType = {
  nguoiDung: ApiUser | null
  dangTai: boolean
  dangNhap: (tenDangNhap: string, matKhau: string) => Promise<ApiUser>
  dangXuat: () => void
}

const KHOA_USER = 'huitlingo_user'
const KHOA_HET_HAN = 'huitlingo_expires'

const NguCanhXacThuc = createContext<NguCanhXacThucType | null>(null)

/**
 * Lấy HỌ TÊN THẬT từ hồ sơ (giáo viên / trợ giảng / học viên).
 * Tài khoản đăng nhập chỉ có mã (GV001, HV004) nên phải hỏi hồ sơ.
 * Lỗi → giữ tên đăng nhập để không chặn đăng nhập.
 */
async function layHoTen(user: { username: string; role: VaiTro }): Promise<string> {
  try {
    if (user.role === 'Giáo viên' || user.role === 'Trợ giảng') {
      const hoSo = await hoSoApi.giaoVien()
      return hoSo.hoTen?.trim() || user.username
    }

    if (user.role === 'Học viên') {
      const hoSo = await hoSoApi.hocVien()
      return hoSo.hoTen?.trim() || user.username
    }
  } catch {
    // Không có hồ sơ (hoặc lỗi mạng) → dùng tên đăng nhập
  }

  return user.username
}

async function taoNguoiDung(user: {
  id: number
  username: string
  email: string
  role: VaiTro
}): Promise<ApiUser> {
  return {
    id: user.id,
    tenDangNhap: user.username,
    hoTen: await layHoTen(user),
    email: user.email,
    vaiTro: user.role,
    avatar: chuVietTat(user.username),
  }
}

export function NhaCungCapXacThuc({ children }: { children: ReactNode }) {
  const [nguoiDung, setNguoiDung] = useState<ApiUser | null>(null)
  const [dangTai, setDangTai] = useState(true)

  const dangXuat = useCallback(() => {
    xoaToken()
    localStorage.removeItem(KHOA_USER)
    localStorage.removeItem(KHOA_HET_HAN)
    setNguoiDung(null)
  }, [])

  // Gặp 401 ở bất kỳ request nào -> xoá phiên ngay
  useEffect(() => {
    datXuLyUnauthorized(dangXuat)
    return () => datXuLyUnauthorized(null)
  }, [dangXuat])

  // Khôi phục phiên khi tải trang: kiểm tra hạn token rồi xác thực lại với backend
  useEffect(() => {
    let huy = false

    const khoiPhuc = async () => {
      const token = layToken()

      if (!token) {
        setDangTai(false)
        return
      }

      const hetHan = localStorage.getItem(KHOA_HET_HAN)
      if (hetHan && new Date(hetHan).getTime() <= Date.now()) {
        dangXuat()
        setDangTai(false)
        return
      }

      try {
        const user = await authApi.toi()
        if (!huy) setNguoiDung(await taoNguoiDung(user))
      } catch {
        // Token hết hạn/không hợp lệ -> xoá phiên, không chặn ứng dụng
        dangXuat()
      } finally {
        if (!huy) setDangTai(false)
      }
    }

    void khoiPhuc()

    return () => {
      huy = true
    }
  }, [dangXuat])

  const dangNhap = useCallback(async (tenDangNhap: string, matKhau: string) => {
    const res = await authApi.dangNhap(tenDangNhap.trim(), matKhau)

    luuToken(res.accessToken)

    const user = await taoNguoiDung(res.user)

    localStorage.setItem(KHOA_USER, JSON.stringify(user))
    localStorage.setItem(KHOA_HET_HAN, res.expiresAt)

    setNguoiDung(user)
    return user
  }, [])

  const value = useMemo(
    () => ({ nguoiDung, dangTai, dangNhap, dangXuat }),
    [nguoiDung, dangTai, dangNhap, dangXuat],
  )

  return <NguCanhXacThuc.Provider value={value}>{children}</NguCanhXacThuc.Provider>
}

export function dungXacThuc() {
  const ctx = useContext(NguCanhXacThuc)
  if (!ctx) throw new Error('dungXacThuc phải dùng trong <NhaCungCapXacThuc>')
  return ctx
}

export { duongDanTheoVaiTro }