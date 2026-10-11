import {
  KHO_NGUOI_DUNG,
  KHO_LOP_HOC,
  KHO_LICH_DAY,
  KHO_DIEM,
  KHO_HOC_VIEN,
  KHO_DIEM_DANH,
  KHO_BANG_DIEM,
} from './khoDuLieu'

const API_BASE = import.meta.env.VITE_API_URL || 'https://localhost:5001/api'
const DUNG_MOCK = import.meta.env.VITE_USE_MOCK !== 'false'

// ===== TYPES =====
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
  maGiaoVien?: string
}

export type StudentItem = {
  id: string
  maHV: string
  hoTen: string
  email: string
  sdt: string
  maLop: string
  trangThai: string
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

// ===== REQUEST CORE =====
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

// ===== API =====
export const goiAPI = {
  // ===== AUTH =====
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

  // ===== GIÁO VIÊN =====
  // Lấy danh sách lớp của 1 giáo viên (theo mã GV)
  async layLopCuaToi(maGiaoVien?: string): Promise<ClassItem[]> {
    if (DUNG_MOCK) {
      await cho(500)
      // Nếu không có mã GV → trả về tất cả (fallback)
      if (!maGiaoVien) return KHO_LOP_HOC
      return KHO_LOP_HOC.filter((lop) => lop.maGiaoVien === maGiaoVien)
    }
    const query = maGiaoVien ? `?teacherId=${maGiaoVien}` : ''
    return request<ClassItem[]>(`/teacher/classes${query}`)
  },

  // Lấy học viên của 1 lớp
  async layHocVienTheoLop(maLop: string): Promise<StudentItem[]> {
    if (DUNG_MOCK) {
      await cho(400)
      return KHO_HOC_VIEN.filter((hv) => hv.maLop === maLop)
    }
    return request<StudentItem[]>(`/teacher/classes/${maLop}/students`)
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
    // ===== ĐIỂM DANH =====
  async layDiemDanh(
    maLop: string,
    ngay: string,
  ): Promise<
    Array<{ maHV: string; trangThai: 'Có mặt' | 'Vắng' | 'Đi trễ'; ghiChu: string }>
  > {
    if (DUNG_MOCK) {
      await cho(400)
      const key = `${maLop}|${ngay}`
      return KHO_DIEM_DANH[key] ?? []
    }
    return request(`/teacher/attendance?classId=${maLop}&date=${ngay}`)
  },

  async luuDiemDanh(
    maLop: string,
    ngay: string,
    danhSach: Array<{
      maHV: string
      trangThai: 'Có mặt' | 'Vắng' | 'Đi trễ'
      ghiChu: string
    }>,
  ): Promise<{ ok: boolean }> {
    if (DUNG_MOCK) {
      await cho(600)
      // Lưu vào bộ nhớ tạm (thực tế sẽ gọi API)
      const key = `${maLop}|${ngay}`
      KHO_DIEM_DANH[key] = danhSach
      return { ok: true }
    }
    return request('/teacher/attendance', {
      method: 'POST',
      body: JSON.stringify({ maLop, ngay, danhSach }),
    })
  },
    // ===== BẢNG ĐIỂM =====
  async layBangDiem(maLop: string): Promise<
    Array<{
      maHV: string
      giuaKy: number | null
      cuoiKy: number | null
      baiTap: number | null
    }>
  > {
    if (DUNG_MOCK) {
      await cho(400)
      return KHO_BANG_DIEM[maLop] ?? []
    }
    return request(`/teacher/grades?classId=${maLop}`)
  },

  async luuBangDiem(
    maLop: string,
    danhSach: Array<{
      maHV: string
      giuaKy: number | null
      cuoiKy: number | null
      baiTap: number | null
    }>,
  ): Promise<{ ok: boolean }> {
    if (DUNG_MOCK) {
      await cho(600)
      KHO_BANG_DIEM[maLop] = danhSach
      return { ok: true }
    }
    return request('/teacher/grades', {
      method: 'POST',
      body: JSON.stringify({ maLop, danhSach }),
    })
  },
}