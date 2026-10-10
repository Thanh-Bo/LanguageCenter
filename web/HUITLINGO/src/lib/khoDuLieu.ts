import type { ApiUser, ClassItem, ScheduleItem, GradeItem } from './goiAPI'

export const KHO_NGUOI_DUNG: Record<
  string,
  { matKhau: string; nguoiDung: ApiUser }
> = {
  'admin@lingohub.vn': {
    matKhau: '123456',
    nguoiDung: {
      id: 'AD001',
      hoTen: 'Quản trị viên',
      email: 'admin@lingohub.vn',
      vaiTro: 'Quản trị viên',
      avatar: 'QT',
    },
  },
  'giaovien@lingohub.vn': {
    matKhau: '123456',
    nguoiDung: {
      id: 'GV001',
      hoTen: 'Nguyễn Lan Anh',
      email: 'giaovien@lingohub.vn',
      vaiTro: 'Giáo viên',
      avatar: 'LA',
    },
  },
  'hocvien@lingohub.vn': {
    matKhau: '123456',
    nguoiDung: {
      id: 'HV001',
      hoTen: 'Trần Minh Khoa',
      email: 'hocvien@lingohub.vn',
      vaiTro: 'Học viên',
      avatar: 'MK',
    },
  },
}

export const KHO_LOP_HOC: ClassItem[] = [
  {
    id: 'IELTS65-01',
    maLop: 'IELTS65-01',
    tenLop: 'IELTS 6.5',
    siSo: 18,
    siSoToiDa: 20,
    lichHoc: 'T2-T4-T6 · 18:00-19:30',
    phong: 'P.A1',
    tienDo: 65,
    capDo: 'Intermediate',
    ngayBatDau: '2025-09-01',
    ngayKetThuc: '2025-12-20',
    trangThai: 'Đang học',
  },
  {
    id: 'TOEIC700-02',
    maLop: 'TOEIC700-02',
    tenLop: 'TOEIC 700+',
    siSo: 22,
    siSoToiDa: 24,
    lichHoc: 'T3-T5 · 19:30-21:00',
    phong: 'P.B2',
    tienDo: 40,
    capDo: 'Pre-Intermediate',
    ngayBatDau: '2025-09-15',
    ngayKetThuc: '2025-12-15',
    trangThai: 'Đang học',
  },
  {
    id: 'COMM-03',
    maLop: 'COMM-03',
    tenLop: 'Giao tiếp cơ bản',
    siSo: 15,
    siSoToiDa: 15,
    lichHoc: 'T7-CN · 09:00-11:00',
    phong: 'P.C1',
    tienDo: 80,
    capDo: 'Beginner',
    ngayBatDau: '2025-08-01',
    ngayKetThuc: '2025-11-30',
    trangThai: 'Sắp đầy',
  },
  {
    id: 'KIDS-04',
    maLop: 'KIDS-04',
    tenLop: 'Tiếng Anh thiếu nhi',
    siSo: 12,
    siSoToiDa: 15,
    lichHoc: 'T2-T6 · 17:00-18:30',
    phong: 'P.D3',
    tienDo: 25,
    capDo: 'Starters',
    ngayBatDau: '2025-10-01',
    ngayKetThuc: '2026-01-30',
    trangThai: 'Đang học',
  },
]

export const KHO_LICH_DAY: ScheduleItem[] = [
  {
    id: 'S1',
    gio: '08:00 - 09:30',
    tenLop: 'IELTS 6.5',
    phong: 'P.A1',
    siSo: 18,
    thu: 'Thứ 2',
  },
  {
    id: 'S2',
    gio: '14:00 - 15:30',
    tenLop: 'TOEIC 700+',
    phong: 'P.B2',
    siSo: 22,
    thu: 'Thứ 3',
  },
  {
    id: 'S3',
    gio: '18:00 - 19:30',
    tenLop: 'Giao tiếp cơ bản',
    phong: 'P.C1',
    siSo: 15,
    thu: 'Thứ 4',
  },
]

export const KHO_DIEM: GradeItem[] = [
  {
    id: 'G1',
    maHocVien: 'HV001',
    tenHocVien: 'Trần Minh Khoa',
    tenLop: 'IELTS 6.5',
    giuaKy: 7.5,
    cuoiKy: 8.0,
    baiTap: 8.5,
    trungBinh: 8.0,
  },
  {
    id: 'G2',
    maHocVien: 'HV002',
    tenHocVien: 'Lê Thu Hà',
    tenLop: 'IELTS 6.5',
    giuaKy: 6.5,
    cuoiKy: 7.0,
    baiTap: 7.5,
    trungBinh: 7.0,
  },
]