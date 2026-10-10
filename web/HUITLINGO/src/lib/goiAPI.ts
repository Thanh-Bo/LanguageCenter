import {
  KHO_NGUOI_DUNG,
  KHO_LOP_HOC,
  KHO_LICH_DAY,
  KHO_DIEM,
} from './khoDuLieu'

const API_BASE = import.meta.env.VITE_API_URL || 'https://localhost:5001/api'
const DUNG_MOCK = import.meta.env.VITE_USE_MOCK !== 'false'

export type ApiUser = {
  id: string
  hoTen: string
  email: string
  vaiTro: 'Quản trị viên' | 'Giáo viên' | 'Học viên'
  avatar?: string
}

export type LoginResponse = {
  token: string
  nguoiDung: ApiUser
}

export type ClassItem = {
  id: string
  maLop: string
  tenLop: string
  siSo: number
  siSoToiDa: number
  lichHoc: string
  phong: string
  tienDo: number
  capDo: string
  ngayBatDau: string
  ngayKetThuc: string
  trangThai: 'Đang học' | 'Sắp đầy' | 'Sắp mở' | 'Đã kết thúc'
}

export type ScheduleItem = {
  id: string
  gio: string
  tenLop: string
  phong: string
  siSo: number
  thu: string
}

export type GradeItem = {
  id: string
  maHocVien: string
  tenHocVien: string
  tenLop: string
  giuaKy: number
  cuoiKy: number
  baiTap: number
  trungBinh: number
}

async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
  const token = localStorage.getItem('huitlingo_token')
  const res = await fetch(`${API_BASE}${path}`, {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
      ...options.headers,
    },
  })

  if (!res.ok) {
    const err = await res.json().catch(() => ({ message: res.statusText }))
    throw new Error(err.message || 'Có lỗi xảy ra')
  }
  return res.json()
}

const cho = (ms = 400) => new Promise((r) => setTimeout(r, ms))

export const goiAPI = {
  async dangNhap(email: string, matKhau: string): Promise<LoginResponse> {
    if (DUNG_MOCK) {
      await cho(600)
      const entry = KHO_NGUOI_DUNG[email.toLowerCase()]
      if (!entry || entry.matKhau !== matKhau) {
        throw new Error('Email hoặc mật khẩu không đúng')
      }
      return {
        token: 'mock-token-' + entry.nguoiDung.id,
        nguoiDung: entry.nguoiDung,
      }
    }
    return request<LoginResponse>('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password: matKhau }),
    })
  },

  dangXuat() {
    localStorage.removeItem('huitlingo_token')
    localStorage.removeItem('huitlingo_user')
  },

  async layThongTinCaNhan(): Promise<ApiUser> {
    if (DUNG_MOCK) {
      await cho(200)
      const stored = localStorage.getItem('huitlingo_user')
      if (!stored) throw new Error('Chưa đăng nhập')
      return JSON.parse(stored)
    }
    return request<ApiUser>('/auth/me')
  },

  async layLopCuaToi(): Promise<ClassItem[]> {
    if (DUNG_MOCK) {
      await cho(500)
      return KHO_LOP_HOC
    }
    return request<ClassItem[]>('/teacher/classes')
  },

  async layLichDay(): Promise<ScheduleItem[]> {
    if (DUNG_MOCK) {
      await cho(400)
      return KHO_LICH_DAY
    }
    return request<ScheduleItem[]>('/teacher/schedule')
  },

  async layDiem(): Promise<GradeItem[]> {
    if (DUNG_MOCK) {
      await cho(400)
      return KHO_DIEM
    }
    return request<GradeItem[]>('/teacher/grades')
  },
}