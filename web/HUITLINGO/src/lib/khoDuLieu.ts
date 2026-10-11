// src/lib/khoDuLieu.ts
// Dữ liệu mẫu để chạy frontend khi chưa có backend

import type {
  ApiUser,
  ClassItem,
  ScheduleItem,
  GradeItem,
  StudentItem,
} from './goiAPI'

// ===== NGƯỜI DÙNG MẪU (3 role theo đề cương) =====
export const KHO_NGUOI_DUNG: Record<
  string,
  { matKhau: string; nguoiDung: ApiUser }
> = {
  // ===== QUẢN TRỊ VIÊN (1) =====
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

  // ===== GIÁO VIÊN (4) =====
  'lananh@lingohub.vn': {
    matKhau: '123456',
    nguoiDung: {
      id: 'GV001',
      hoTen: 'Nguyễn Lan Anh',
      email: 'lananh@lingohub.vn',
      vaiTro: 'Giáo viên',
      avatar: 'LA',
    },
  },
  'minhkhoa@lingohub.vn': {
    matKhau: '123456',
    nguoiDung: {
      id: 'GV002',
      hoTen: 'Trần Minh Khoa',
      email: 'minhkhoa@lingohub.vn',
      vaiTro: 'Giáo viên',
      avatar: 'MK',
    },
  },
  'david@lingohub.vn': {
    matKhau: '123456',
    nguoiDung: {
      id: 'GV003',
      hoTen: 'David Walker',
      email: 'david@lingohub.vn',
      vaiTro: 'Giáo viên',
      avatar: 'DW',
    },
  },
  'emily@lingohub.vn': {
    matKhau: '123456',
    nguoiDung: {
      id: 'GV004',
      hoTen: 'Emily Chen',
      email: 'emily@lingohub.vn',
      vaiTro: 'Giáo viên',
      avatar: 'EC',
    },
  },

  // ===== HỌC VIÊN (4) =====
  'khoa.hv@lingohub.vn': {
    matKhau: '123456',
    nguoiDung: {
      id: 'HV001',
      hoTen: 'Trần Minh Khoa',
      email: 'khoa.hv@lingohub.vn',
      vaiTro: 'Học viên',
      avatar: 'MK',
    },
  },
  'ha.lt@lingohub.vn': {
    matKhau: '123456',
    nguoiDung: {
      id: 'HV002',
      hoTen: 'Lê Thu Hà',
      email: 'ha.lt@lingohub.vn',
      vaiTro: 'Học viên',
      avatar: 'LH',
    },
  },
  'an.nv@lingohub.vn': {
    matKhau: '123456',
    nguoiDung: {
      id: 'HV003',
      hoTen: 'Nguyễn Văn An',
      email: 'an.nv@lingohub.vn',
      vaiTro: 'Học viên',
      avatar: 'NA',
    },
  },
  'linh.pm@lingohub.vn': {
    matKhau: '123456',
    nguoiDung: {
      id: 'HV004',
      hoTen: 'Phạm Mai Linh',
      email: 'linh.pm@lingohub.vn',
      vaiTro: 'Học viên',
      avatar: 'PL',
    },
  },
}

// ===== LỚP HỌC CỦA GIÁO VIÊN (có mã GV phụ trách) =====
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
    maGiaoVien: 'GV001', // Nguyễn Lan Anh
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
    maGiaoVien: 'GV002', // Trần Minh Khoa
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
    maGiaoVien: 'GV003', // David Walker
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
    maGiaoVien: 'GV004', // Emily Chen
  },
  {
    id: 'IELTS-F21',
    maLop: 'IELTS-F21',
    tenLop: 'IELTS Foundation',
    siSo: 7,
    siSoToiDa: 12,
    lichHoc: 'T3-T6 · 18:00-19:30',
    phong: 'P.A2',
    tienDo: 15,
    capDo: 'Beginner',
    ngayBatDau: '2025-10-15',
    ngayKetThuc: '2026-02-15',
    trangThai: 'Sắp mở',
    maGiaoVien: 'GV001', // Nguyễn Lan Anh
  },
]

// ===== HỌC VIÊN THEO LỚP =====
export const KHO_HOC_VIEN: StudentItem[] = [
  // Lớp IELTS65-01
  { id: 'HV001', maHV: 'HV001', hoTen: 'Trần Minh Khoa', email: 'khoa.hv@lingohub.vn', sdt: '0901234567', maLop: 'IELTS65-01', trangThai: 'Đang học' },
  { id: 'HV002', maHV: 'HV002', hoTen: 'Lê Thu Hà', email: 'ha.lt@lingohub.vn', sdt: '0902345678', maLop: 'IELTS65-01', trangThai: 'Đang học' },
  { id: 'HV003', maHV: 'HV003', hoTen: 'Nguyễn Văn An', email: 'an.nv@lingohub.vn', sdt: '0903456789', maLop: 'IELTS65-01', trangThai: 'Đang học' },
  { id: 'HV004', maHV: 'HV004', hoTen: 'Phạm Mai Linh', email: 'linh.pm@lingohub.vn', sdt: '0904567890', maLop: 'IELTS65-01', trangThai: 'Đang học' },
  { id: 'HV005', maHV: 'HV005', hoTen: 'Hoàng Minh Tuấn', email: 'tuan.hm@lingohub.vn', sdt: '0905678901', maLop: 'IELTS65-01', trangThai: 'Đang học' },
  { id: 'HV006', maHV: 'HV006', hoTen: 'Vũ Thị Ngọc', email: 'ngoc.vt@lingohub.vn', sdt: '0906789012', maLop: 'IELTS65-01', trangThai: 'Đang học' },

  // Lớp TOEIC700-02
  { id: 'HV007', maHV: 'HV007', hoTen: 'Đặng Quốc Bảo', email: 'bao.dq@lingohub.vn', sdt: '0907890123', maLop: 'TOEIC700-02', trangThai: 'Đang học' },
  { id: 'HV008', maHV: 'HV008', hoTen: 'Bùi Thanh Mai', email: 'mai.bt@lingohub.vn', sdt: '0908901234', maLop: 'TOEIC700-02', trangThai: 'Đang học' },
  { id: 'HV009', maHV: 'HV009', hoTen: 'Ngô Văn Hùng', email: 'hung.nv@lingohub.vn', sdt: '0909012345', maLop: 'TOEIC700-02', trangThai: 'Đang học' },

  // Lớp COMM-03
  { id: 'HV010', maHV: 'HV010', hoTen: 'Trịnh Thị Hương', email: 'huong.tt@lingohub.vn', sdt: '0910123456', maLop: 'COMM-03', trangThai: 'Đang học' },
  { id: 'HV011', maHV: 'HV011', hoTen: 'Lý Minh Quân', email: 'quan.lm@lingohub.vn', sdt: '0911234567', maLop: 'COMM-03', trangThai: 'Đang học' },

  // Lớp KIDS-04
  { id: 'HV012', maHV: 'HV012', hoTen: 'Nguyễn Bảo An', email: 'an.nb@lingohub.vn', sdt: '0912345678', maLop: 'KIDS-04', trangThai: 'Đang học' },
  { id: 'HV013', maHV: 'HV013', hoTen: 'Trần Gia Hân', email: 'han.tg@lingohub.vn', sdt: '0913456789', maLop: 'KIDS-04', trangThai: 'Đang học' },

  // Lớp IELTS-F21
  { id: 'HV014', maHV: 'HV014', hoTen: 'Phan Thanh Tùng', email: 'tung.pt@lingohub.vn', sdt: '0914567890', maLop: 'IELTS-F21', trangThai: 'Đang học' },
  { id: 'HV015', maHV: 'HV015', hoTen: 'Đỗ Khánh Linh', email: 'linh.dk@lingohub.vn', sdt: '0915678901', maLop: 'IELTS-F21', trangThai: 'Đang học' },
]

// ===== LỊCH DẠY =====
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

// ===== ĐIỂM HỌC VIÊN =====
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
// ===== ĐIỂM DANH THEO LỚP + NGÀY =====
// Key: "maLop|YYYY-MM-DD"
export const KHO_DIEM_DANH: Record<
  string,
  Array<{ maHV: string; trangThai: 'Có mặt' | 'Vắng' | 'Đi trễ'; ghiChu: string }>
> = {
  // IELTS 6.5 - ngày 16/10
  'IELTS65-01|2025-10-16': [
    { maHV: 'HV001', trangThai: 'Có mặt', ghiChu: '' },
    { maHV: 'HV002', trangThai: 'Có mặt', ghiChu: '' },
    { maHV: 'HV003', trangThai: 'Đi trễ', ghiChu: 'Trễ 10 phút' },
    { maHV: 'HV004', trangThai: 'Có mặt', ghiChu: '' },
    { maHV: 'HV005', trangThai: 'Vắng', ghiChu: 'Có phép' },
    { maHV: 'HV006', trangThai: 'Có mặt', ghiChu: '' },
  ],
  // TOEIC 700+ - ngày 16/10
  'TOEIC700-02|2025-10-16': [
    { maHV: 'HV007', trangThai: 'Có mặt', ghiChu: '' },
    { maHV: 'HV008', trangThai: 'Vắng', ghiChu: 'Không phép' },
    { maHV: 'HV009', trangThai: 'Có mặt', ghiChu: '' },
  ],
  // COMM-03 - ngày 16/10
  'COMM-03|2025-10-16': [
    { maHV: 'HV010', trangThai: 'Có mặt', ghiChu: '' },
    { maHV: 'HV011', trangThai: 'Có mặt', ghiChu: '' },
  ],
  // KIDS-04 - ngày 16/10
  'KIDS-04|2025-10-16': [
    { maHV: 'HV012', trangThai: 'Có mặt', ghiChu: '' },
    { maHV: 'HV013', trangThai: 'Đi trễ', ghiChu: 'Phụ huynh đưa muộn' },
  ],
  // IELTS-F21 - ngày 16/10
  'IELTS-F21|2025-10-16': [
    { maHV: 'HV014', trangThai: 'Có mặt', ghiChu: '' },
    { maHV: 'HV015', trangThai: 'Có mặt', ghiChu: '' },
  ],
}
// ===== BẢNG ĐIỂM THEO LỚP =====
// Key: maLop
// Value: danh sách điểm từng học viên
export const KHO_BANG_DIEM: Record<
  string,
  Array<{
    maHV: string
    giuaKy: number | null
    cuoiKy: number | null
    baiTap: number | null
  }>
> = {
  'IELTS65-01': [
    { maHV: 'HV001', giuaKy: 7.5, cuoiKy: 8.0, baiTap: 8.5 },
    { maHV: 'HV002', giuaKy: 6.5, cuoiKy: 7.0, baiTap: 7.5 },
    { maHV: 'HV003', giuaKy: 7.0, cuoiKy: null, baiTap: 7.5 },
    { maHV: 'HV004', giuaKy: 8.5, cuoiKy: 8.5, baiTap: 9.0 },
    { maHV: 'HV005', giuaKy: 6.0, cuoiKy: 6.5, baiTap: 6.5 },
    { maHV: 'HV006', giuaKy: 7.5, cuoiKy: 7.5, baiTap: 8.0 },
  ],
  'TOEIC700-02': [
    { maHV: 'HV007', giuaKy: 7.0, cuoiKy: 7.5, baiTap: 7.5 },
    { maHV: 'HV008', giuaKy: 6.5, cuoiKy: 6.5, baiTap: 7.0 },
    { maHV: 'HV009', giuaKy: 7.5, cuoiKy: 8.0, baiTap: 8.0 },
  ],
  'COMM-03': [
    { maHV: 'HV010', giuaKy: 8.0, cuoiKy: 8.5, baiTap: 8.5 },
    { maHV: 'HV011', giuaKy: 7.5, cuoiKy: 7.5, baiTap: 8.0 },
  ],
  'KIDS-04': [
    { maHV: 'HV012', giuaKy: 8.5, cuoiKy: null, baiTap: 9.0 },
    { maHV: 'HV013', giuaKy: 8.0, cuoiKy: null, baiTap: 8.5 },
  ],
  'IELTS-F21': [
    { maHV: 'HV014', giuaKy: 6.0, cuoiKy: null, baiTap: 6.5 },
    { maHV: 'HV015', giuaKy: 6.5, cuoiKy: null, baiTap: 7.0 },
  ],
}