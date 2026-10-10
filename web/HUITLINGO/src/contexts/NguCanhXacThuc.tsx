// src/contexts/NguCanhXacThuc.tsx
import {
  createContext,
  useContext,
  useEffect,
  useState,
  type ReactNode,
} from 'react'
import { goiAPI, type ApiUser } from '../lib/goiAPI'

type NguCanhXacThucType = {
  nguoiDung: ApiUser | null
  dangTai: boolean
  dangNhap: (email: string, matKhau: string) => Promise<ApiUser>
  dangXuat: () => void
}

const NguCanhXacThuc = createContext<NguCanhXacThucType | null>(null)

export function NhaCungCapXacThuc({ children }: { children: ReactNode }) {
  const [nguoiDung, setNguoiDung] = useState<ApiUser | null>(null)
  const [dangTai, setDangTai] = useState(true)

  useEffect(() => {
    try {
      const stored = localStorage.getItem('huitlingo_user')
      if (stored) setNguoiDung(JSON.parse(stored))
    } catch {
      /* ignore */
    } finally {
      setDangTai(false)
    }
  }, [])

  const dangNhap = async (email: string, matKhau: string) => {
    const res = await goiAPI.dangNhap(email, matKhau)
    localStorage.setItem('huitlingo_token', res.token)
    localStorage.setItem('huitlingo_user', JSON.stringify(res.nguoiDung))
    setNguoiDung(res.nguoiDung)
    return res.nguoiDung
  }

  const dangXuat = () => {
    goiAPI.dangXuat()
    setNguoiDung(null)
  }

  return (
    <NguCanhXacThuc.Provider
      value={{ nguoiDung, dangTai, dangNhap, dangXuat }}
    >
      {children}
    </NguCanhXacThuc.Provider>
  )
}

export function dungXacThuc() {
  const ctx = useContext(NguCanhXacThuc)
  if (!ctx) throw new Error('dungXacThuc phải dùng trong <NhaCungCapXacThuc>')
  return ctx
}

/** Helper: vai trò → đường dẫn portal tương ứng */
export function duongDanTheoVaiTro(vaiTro: ApiUser['vaiTro']): string {
  switch (vaiTro) {
    case 'Giáo viên':
      return '/giaovien'
    case 'Học viên':
      return '/hocvien'
    default:
      return '/admin'
  }
}