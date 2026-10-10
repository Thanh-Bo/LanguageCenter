import { useMemo, useState } from 'react'
import { Link } from 'react-router'
import { Icon, type IconName } from '../components/BieuTuong'

type Role = 'Quản trị viên' | 'Giáo viên' | 'Học viên'
type NavItem = { label: string; icon: IconName; badge?: string }

const navByRole: Record<Role, NavItem[]> = {
  'Quản trị viên': [
    { label: 'Tổng quan', icon: 'grid' },
    { label: 'Tài khoản', icon: 'users' },
    { label: 'Lớp học', icon: 'book' },
    { label: 'Lịch học', icon: 'calendar' },
    { label: 'Học phí', icon: 'wallet' },
    { label: 'Thông báo', icon: 'bell', badge: '5' },
    { label: 'Báo cáo', icon: 'chart' },
  ],
  'Giáo viên': [
    { label: 'Tổng quan', icon: 'grid' },
    { label: 'Lớp của tôi', icon: 'users' },
    { label: 'Lịch dạy', icon: 'calendar' },
    { label: 'Điểm danh', icon: 'check', badge: '2' },
    { label: 'Bảng điểm', icon: 'chart' },
    { label: 'Thông báo', icon: 'bell', badge: '5' },
  ],
  'Học viên': [
    { label: 'Tổng quan', icon: 'grid' },
    { label: 'Lịch học', icon: 'calendar' },
    { label: 'Điểm số', icon: 'chart' },
    { label: 'Thông báo', icon: 'bell', badge: '3' },
    { label: 'Cá nhân', icon: 'userPlus' },
  ],
}

const classes = [
  {
    code: 'IELTS-A12',
    name: 'IELTS Advanced 7.0+',
    teacher: 'Nguyễn Lan Anh',
    schedule: 'T2, T4, T6 • 18:00',
    room: 'P.302',
    students: 11,
    max: 12,
    status: 'Đang học',
  },
  {
    code: 'TOEIC-B08',
    name: 'TOEIC Intensive 750',
    teacher: 'Trần Minh Khoa',
    schedule: 'T3, T5 • 19:30',
    room: 'P.204',
    students: 16,
    max: 18,
    status: 'Đang học',
  },
  {
    code: 'COMM-C15',
    name: 'Giao tiếp Pre-Intermediate',
    teacher: 'David Walker',
    schedule: 'T2, T5 • 17:30',
    room: 'P.105',
    students: 9,
    max: 12,
    status: 'Đang học',
  },
  {
    code: 'KIDS-S06',
    name: 'Cambridge Starters',
    teacher: 'Sarah Johnson',
    schedule: 'T7, CN • 08:00',
    room: 'P.201',
    students: 12,
    max: 12,
    status: 'Sắp đầy',
  },
  {
    code: 'IELTS-F21',
    name: 'IELTS Foundation',
    teacher: 'Nguyễn Lan Anh',
    schedule: 'T3, T6 • 18:00',
    room: 'P.301',
    students: 7,
    max: 12,
    status: 'Sắp mở',
  },
  {
    code: 'BIZ-E03',
    name: 'Business English',
    teacher: 'Emily Chen',
    schedule: 'T4, T7 • 19:00',
    room: 'P.402',
    students: 8,
    max: 10,
    status: 'Đang học',
  },
]

const events = [
  {
    time: '08:00',
    end: '09:30',
    name: 'IELTS Foundation',
    meta: 'P.301 · Nguyễn Lan Anh',
    tone: 'blue',
  },
  {
    time: '10:00',
    end: '11:30',
    name: 'Cambridge Starters',
    meta: 'P.201 · Sarah Johnson',
    tone: 'blue',
  },
  {
    time: '14:00',
    end: '15:30',
    name: 'TOEIC Intensive',
    meta: 'P.204 · Trần Minh Khoa',
    tone: 'blue',
  },
  {
    time: '18:00',
    end: '19:30',
    name: 'IELTS Advanced',
    meta: 'P.302 · Nguyễn Lan Anh',
    tone: 'blue',
  },
]

function Metric({
  label,
  value,
  delta,
  icon,
  tone,
}: {
  label: string
  value: string
  delta: string
  icon: IconName
  tone: string
}) {
  const toneClasses: Record<string, string> = {
    blue: 'bg-blue-600 text-white',
  }
  return (
    <article className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
      <div className="flex items-start justify-between">
        <span
          className={`grid size-11 place-items-center rounded-xl ${toneClasses[tone]}`}
        >
          <Icon name={icon} className="size-5" />
        </span>
        <span className="flex items-center gap-1 rounded-full bg-blue-600 px-2 py-1 text-xs font-bold text-white">
          <Icon name="arrowUp" className="size-3" /> {delta}
        </span>
      </div>
      <p className="mt-5 text-sm font-medium text-slate-500">{label}</p>
      <p className="mt-1 text-3xl font-bold text-slate-900">{value}</p>
    </article>
  )
}

export default function CongQuanLy({
  vaiTroMacDinh = 'Quản trị viên',
}: {
  vaiTroMacDinh?: Role
}) {
  const [role, setRole] = useState<Role>(vaiTroMacDinh)
  const [active, setActive] = useState('Tổng quan')
  const [sidebar, setSidebar] = useState(false)
  const [view, setView] = useState<'Lịch tuần' | 'Danh sách'>('Lịch tuần')
  const [query, setQuery] = useState('')
  const [filter, setFilter] = useState('Tất cả')
  const [notice, setNotice] = useState(false)

  const filteredClasses = useMemo(
    () =>
      classes.filter((item) => {
        const matchesQuery = `${item.code} ${item.name} ${item.teacher}`
          .toLowerCase()
          .includes(query.toLowerCase())
        return matchesQuery && (filter === 'Tất cả' || item.status === filter)
      }),
    [query, filter],
  )

  const selectRole = (next: Role) => {
    setRole(next)
    setActive('Tổng quan')
  }

  return (
    <div className="min-h-screen bg-slate-50 text-slate-800">
      {sidebar && (
        <button
          className="fixed inset-0 z-30 bg-slate-950/30 lg:hidden"
          onClick={() => setSidebar(false)}
          aria-label="Đóng menu"
        />
      )}
      <aside
        className={`fixed inset-y-0 left-0 z-40 flex w-68 flex-col border-r border-slate-200 bg-white transition-transform lg:translate-x-0 ${
          sidebar ? 'translate-x-0' : '-translate-x-full'
        }`}
      >
        <div className="flex h-18 items-center justify-between border-b border-slate-100 px-6">
          <Link
            to="/"
            className="flex items-center gap-3 text-slate-900 no-underline"
          >
            <span className="grid size-10 place-items-center rounded-xl bg-blue-600 text-white">
              <Icon name="logo" className="size-5" />
            </span>
            <span>
              <strong className="block text-lg font-bold">
                <span>HUIT </span>
                <span className="text-blue-600">LingoHub</span>
              </strong>
              <small className="block text-[10px] font-bold uppercase tracking-[.18em] text-slate-400">
                Hệ thống quản lý
              </small>
            </span>
          </Link>
          <button
            onClick={() => setSidebar(false)}
            className="text-slate-400 lg:hidden"
          >
            <Icon name="close" className="size-5" />
          </button>
        </div>

        <div className="px-4 py-5">
          <p className="px-3 text-[11px] font-bold uppercase tracking-widest text-slate-400">
            Không gian làm việc
          </p>
          <div className="relative mt-2">
            <select
              value={role}
              onChange={(e) => selectRole(e.target.value as Role)}
              className="w-full appearance-none rounded-xl border border-slate-200 bg-slate-50 px-3 py-3 text-sm font-bold outline-none focus:border-blue-500"
            >
              <option>Quản trị viên</option>
              <option>Giáo viên</option>
              <option>Học viên</option>
            </select>
            <Icon
              name="chevronDown"
              className="pointer-events-none absolute right-3 top-3.5 size-4 text-slate-400"
            />
          </div>
        </div>

        <nav className="flex-1 space-y-1 overflow-y-auto px-4">
          {navByRole[role].map((item) => (
            <button
              key={item.label}
              onClick={() => {
                setActive(item.label)
                setSidebar(false)
              }}
              className={`flex w-full items-center gap-3 rounded-xl px-3 py-3 text-left text-sm font-semibold transition ${
                active === item.label
                  ? 'bg-blue-600 text-white shadow-sm shadow-blue-200'
                  : 'text-slate-500 hover:bg-slate-50 hover:text-slate-900'
              }`}
            >
              <Icon name={item.icon} className="size-5" />
              <span className="flex-1">{item.label}</span>
              {item.badge && (
                <span className="grid min-w-5 place-items-center rounded-full bg-blue-600 px-1.5 py-0.5 text-[10px] text-white">
                  {item.badge}
                </span>
              )}
            </button>
          ))}
          <div className="my-4 border-t border-slate-100" />
          <button className="flex w-full items-center gap-3 rounded-xl px-3 py-3 text-sm font-semibold text-slate-500 hover:bg-slate-50">
            <Icon name="settings" className="size-5" /> Cài đặt
          </button>
        </nav>

        <div className="m-4 rounded-2xl bg-slate-900 p-4 text-white">
          <div className="flex items-center gap-3">
            <span className="grid size-10 place-items-center rounded-full bg-blue-500 text-sm font-bold">
              MT
            </span>
            <span className="min-w-0 flex-1">
              <strong className="block truncate text-sm">Mai Thanh Thảo</strong>
              <small className="text-slate-400">
                {role.toLowerCase()}
              </small>
            </span>
            <Link to="/dangnhap" className="text-slate-400 hover:text-white">
              <Icon name="logout" className="size-5" />
            </Link>
          </div>
        </div>
      </aside>

      <div className="lg:pl-68">
        <header className="sticky top-0 z-20 flex h-18 items-center border-b border-slate-200 bg-white/90 px-4 backdrop-blur-xl sm:px-7">
          <button
            onClick={() => setSidebar(true)}
            className="mr-4 text-slate-500 lg:hidden"
          >
            <Icon name="menu" className="size-6" />
          </button>
          <div className="relative hidden max-w-md flex-1 md:block">
            <Icon
              name="search"
              className="absolute left-3 top-2.5 size-5 text-slate-400"
            />
            <input
              className="h-10 w-full rounded-xl bg-slate-100 pl-10 pr-4 text-sm outline-none ring-blue-100 transition focus:ring-4"
              placeholder="Tìm học viên, lớp học, giáo viên..."
            />
          </div>
          <div className="ml-auto flex items-center gap-2 sm:gap-4">
            <span className="hidden items-center gap-2 rounded-full bg-blue-600 px-3 py-1.5 text-xs font-bold text-white sm:flex">
              <span className="size-2 rounded-full bg-white" /> Dữ liệu đã đồng bộ
            </span>
            <button
              onClick={() => setNotice(!notice)}
              className="relative grid size-10 place-items-center rounded-xl text-slate-500 hover:bg-slate-100"
            >
              <Icon name="bell" className="size-5" />
              <span className="absolute right-2 top-2 size-2 rounded-full bg-blue-600 ring-2 ring-white" />
            </button>
            <span className="grid size-10 place-items-center rounded-xl bg-blue-600 text-sm font-bold text-white">
              MT
            </span>
          </div>
          {notice && (
            <div className="absolute right-5 top-16 w-80 rounded-2xl border border-slate-200 bg-white p-3 shadow-xl">
              <div className="flex items-center justify-between px-2 py-2">
                <strong>Thông báo mới</strong>
                <span className="text-xs font-bold text-blue-600">
                  Đánh dấu đã đọc
                </span>
              </div>
              <div className="rounded-xl bg-blue-600 p-3 text-sm text-white">
                <strong className="block">Lớp KIDS-S06 đã đủ sĩ số</strong>
                <span className="mt-1 block text-xs leading-5 text-blue-100">
                  Hệ thống vừa ghi nhận học viên thứ 12 · 5 phút trước
                </span>
              </div>
              <div className="mt-1 p-3 text-sm">
                <strong className="block">Yêu cầu đổi phòng học</strong>
                <span className="mt-1 block text-xs leading-5 text-slate-500">
                  Lớp TOEIC-B08 · 28 phút trước
                </span>
              </div>
            </div>
          )}
        </header>

        <main className="p-4 sm:p-7">
          {active === 'Tổng quan' ? (
            <>
              <section className="mb-7 flex flex-col justify-between gap-4 sm:flex-row sm:items-end">
                <div>
                  <p className="mb-1 text-sm font-semibold text-blue-600">
                    Thứ Tư, 16 tháng 10
                  </p>
                  <h1 className="text-3xl font-bold text-slate-900">
                    Chào buổi sáng, Thanh Thảo
                  </h1>
                  <p className="mt-2 text-sm text-slate-500">
                    Đây là những hoạt động cần bạn theo dõi hôm nay.
                  </p>
                </div>
                <button
                  onClick={() => setActive('Lớp học')}
                  className="flex h-11 items-center justify-center gap-2 rounded-xl bg-blue-600 px-5 text-sm font-bold text-white shadow-lg shadow-blue-100 hover:bg-blue-700"
                >
                  <Icon name="userPlus" className="size-4" /> Xếp học viên vào lớp
                </button>
              </section>

              <section className="grid gap-4 sm:grid-cols-2 xl:grid-cols-4">
                <Metric
                  label="Học viên đang học"
                  value="1.248"
                  delta="8,2%"
                  icon="users"
                  tone="blue"
                />
                <Metric
                  label="Lớp đang hoạt động"
                  value="42"
                  delta="3 lớp"
                  icon="book"
                  tone="blue"
                />
                <Metric
                  label="Tỷ lệ lấp đầy"
                  value="86%"
                  delta="4,1%"
                  icon="chart"
                  tone="blue"
                />
                <Metric
                  label="Ca học hôm nay"
                  value="18"
                  delta="2 ca"
                  icon="calendar"
                  tone="blue"
                />
              </section>

              <section className="mt-6 grid gap-6 xl:grid-cols-[1.55fr_.85fr]">
                <article className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
                  <div className="flex flex-wrap items-center justify-between gap-3 border-b border-slate-100 p-5">
                    <div>
                      <h2 className="text-xl font-bold text-slate-900">
                        Lịch học hôm nay
                      </h2>
                      <p className="mt-1 text-xs text-slate-500">
                        18 ca học · 11 phòng đang sử dụng
                      </p>
                    </div>
                    <div className="flex rounded-xl bg-slate-100 p-1">
                      {(['Lịch tuần', 'Danh sách'] as const).map((item) => (
                        <button
                          key={item}
                          onClick={() => setView(item)}
                          className={`rounded-lg px-3 py-2 text-xs font-bold ${
                            view === item
                              ? 'bg-white text-blue-600 shadow-sm'
                              : 'text-slate-500'
                          }`}
                        >
                          {item}
                        </button>
                      ))}
                    </div>
                  </div>
                  {view === 'Lịch tuần' ? (
                    <div className="overflow-x-auto p-5">
                      <div className="min-w-150">
                        <div className="mb-4 grid grid-cols-[72px_repeat(5,1fr)] text-center text-xs font-bold text-slate-400">
                          <span />
                          <span>
                            T2
                            <br />
                            <b className="text-slate-700">14/10</b>
                          </span>
                          <span>
                            T3
                            <br />
                            <b className="text-slate-700">15/10</b>
                          </span>
                          <span className="text-blue-600">
                            Hôm nay
                            <br />
                            <b>16/10</b>
                          </span>
                          <span>
                            T5
                            <br />
                            <b className="text-slate-700">17/10</b>
                          </span>
                          <span>
                            T6
                            <br />
                            <b className="text-slate-700">18/10</b>
                          </span>
                        </div>
                        <div className="grid grid-cols-[72px_repeat(5,1fr)] border-t border-slate-100">
                          <div className="space-y-14 pt-4 text-xs text-slate-400">
                            <p>08:00</p>
                            <p>10:00</p>
                            <p>14:00</p>
                            <p>18:00</p>
                          </div>
                          {[0, 1, 2, 3, 4].map((day) => (
                            <div
                              key={day}
                              className="min-h-84 border-l border-slate-100 p-1.5"
                            >
                              {day === 2 &&
                                events.map((event) => (
                                  <div
                                    key={event.name}
                                    className="mb-2 rounded-lg border-l-4 border-blue-700 bg-blue-100 p-2"
                                  >
                                    <p className="text-[10px] font-bold text-slate-500">
                                      {event.time}–{event.end}
                                    </p>
                                    <p className="mt-1 text-xs font-bold text-slate-800">
                                      {event.name}
                                    </p>
                                    <p className="mt-1 text-[10px] text-slate-500">
                                      {event.meta}
                                    </p>
                                  </div>
                                ))}
                              {day !== 2 && (
                                <div className="mt-16 rounded-lg border-l-3 border-slate-300 bg-slate-50 p-2">
                                  <p className="text-[10px] font-bold text-slate-400">
                                    18:00–19:30
                                  </p>
                                  <p className="mt-1 text-xs font-bold text-slate-600">
                                    {day % 2 ? 'Giao tiếp B1' : 'TOEIC 650+'}
                                  </p>
                                </div>
                              )}
                            </div>
                          ))}
                        </div>
                      </div>
                    </div>
                  ) : (
                    <div className="divide-y divide-slate-100">
                      {events.map((event) => (
                        <div
                          key={event.name}
                          className="flex items-center gap-4 px-5 py-4"
                        >
                          <span className="w-13 text-sm font-bold text-slate-700">
                            {event.time}
                          </span>
                          <span className="size-2 rounded-full bg-blue-500" />
                          <span className="flex-1">
                            <strong className="block text-sm">
                              {event.name}
                            </strong>
                            <small className="text-slate-500">
                              {event.meta}
                            </small>
                          </span>
                          <button className="text-slate-400">
                            <Icon name="more" className="size-5" />
                          </button>
                        </div>
                      ))}
                    </div>
                  )}
                </article>

                <div className="space-y-6">
                  <article className="rounded-2xl border border-slate-200 bg-white p-5 shadow-sm">
                    <div className="flex items-center justify-between">
                      <h2 className="text-lg font-bold">Việc cần xử lý</h2>
                      <span className="rounded-full bg-blue-600 px-2 py-1 text-xs font-bold text-white">
                        6 việc
                      </span>
                    </div>
                    <div className="mt-4 space-y-3">
                      {[
                        [
                          'alert',
                          'Xung đột lịch giáo viên',
                          '2 lớp cần phân công lại',
                          'text-white bg-blue-600',
                        ],
                        [
                          'users',
                          'Lớp sắp đầy',
                          '3 lớp chỉ còn 1 chỗ',
                          'text-white bg-blue-600',
                        ],
                        [
                          'calendar',
                          'Yêu cầu đổi ca học',
                          '1 yêu cầu đang chờ',
                          'text-white bg-blue-600',
                        ],
                      ].map(([icon, title, desc, tone]) => (
                        <button
                          key={title}
                          className="flex w-full items-center gap-3 rounded-xl border border-slate-100 p-3 text-left hover:border-blue-200 hover:bg-blue-50/40"
                        >
                          <span
                            className={`grid size-9 place-items-center rounded-lg ${tone}`}
                          >
                            <Icon name={icon as IconName} className="size-4" />
                          </span>
                          <span className="flex-1">
                            <strong className="block text-sm">{title}</strong>
                            <small className="text-slate-500">{desc}</small>
                          </span>
                          <Icon
                            name="arrowRight"
                            className="size-4 text-slate-300"
                          />
                        </button>
                      ))}
                    </div>
                  </article>
                  <article className="overflow-hidden rounded-2xl bg-blue-700 p-5 text-white shadow-lg shadow-blue-200">
                    <div className="flex items-start justify-between">
                      <span className="grid size-10 place-items-center rounded-xl bg-white/15">
                        <Icon name="sparkles" className="size-5" />
                      </span>
                      <span className="rounded-full bg-white/15 px-2 py-1 text-[10px] font-bold">
                        GỢI Ý AI
                      </span>
                    </div>
                    <h3 className="mt-5 text-lg font-bold">Tối ưu phòng học</h3>
                    <p className="mt-2 text-sm leading-6 text-blue-100">
                      Có thể chuyển 2 ca chiều sang phòng P.305 để giảm 18% thời gian trống.
                    </p>
                    <button className="mt-4 flex items-center gap-2 text-sm font-bold">
                      Xem đề xuất <Icon name="arrowRight" className="size-4" />
                    </button>
                  </article>
                </div>
              </section>
            </>
          ) : active === 'Lớp học' || active === 'Lớp của tôi' ? (
            <section>
              <div className="mb-6 flex flex-col justify-between gap-4 sm:flex-row sm:items-end">
                <div>
                  <p className="mb-1 text-sm font-semibold text-blue-600">
                    Quản lý đào tạo
                  </p>
                  <h1 className="text-3xl font-bold text-slate-900">
                    Danh sách lớp học
                  </h1>
                  <p className="mt-2 text-sm text-slate-500">
                    Theo dõi sĩ số, giáo viên và lịch học theo thời gian thực.
                  </p>
                </div>
                <button className="flex h-11 items-center justify-center gap-2 rounded-xl bg-blue-600 px-5 text-sm font-bold text-white">
                  <Icon name="userPlus" className="size-4" /> Tạo lớp mới
                </button>
              </div>
              <article className="overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm">
                <div className="flex flex-col gap-3 border-b border-slate-100 p-4 md:flex-row">
                  <label className="relative flex-1">
                    <Icon
                      name="search"
                      className="absolute left-3 top-3 size-4 text-slate-400"
                    />
                    <input
                      value={query}
                      onChange={(e) => setQuery(e.target.value)}
                      placeholder="Tìm theo mã lớp, tên lớp, giáo viên..."
                      className="h-10 w-full rounded-xl border border-slate-200 pl-9 pr-3 text-sm outline-none focus:border-blue-500"
                    />
                  </label>
                  <div className="relative">
                    <Icon
                      name="filter"
                      className="absolute left-3 top-3 size-4 text-slate-400"
                    />
                    <select
                      value={filter}
                      onChange={(e) => setFilter(e.target.value)}
                      className="h-10 appearance-none rounded-xl border border-slate-200 bg-white pl-9 pr-9 text-sm font-semibold outline-none"
                    >
                      <option>Tất cả</option>
                      <option>Đang học</option>
                      <option>Sắp đầy</option>
                      <option>Sắp mở</option>
                    </select>
                    <Icon
                      name="chevronDown"
                      className="absolute right-3 top-3 size-4 text-slate-400"
                    />
                  </div>
                </div>
                <div className="overflow-x-auto">
                  <table className="w-full min-w-220 border-collapse text-left">
                    <thead>
                      <tr className="bg-slate-50 text-[11px] uppercase tracking-wider text-slate-500">
                        <th className="px-5 py-3 font-bold">Lớp học</th>
                        <th className="px-5 py-3 font-bold">Giáo viên</th>
                        <th className="px-5 py-3 font-bold">Lịch học</th>
                        <th className="px-5 py-3 font-bold">Sĩ số</th>
                        <th className="px-5 py-3 font-bold">Trạng thái</th>
                        <th className="px-5 py-3" />
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-100">
                      {filteredClasses.map((item) => (
                        <tr key={item.code} className="hover:bg-slate-50">
                          <td className="px-5 py-4">
                            <strong className="block text-sm text-slate-900">
                              {item.name}
                            </strong>
                            <span className="mt-1 block text-xs font-semibold text-blue-600">
                              {item.code} · {item.room}
                            </span>
                          </td>
                          <td className="px-5 py-4 text-sm font-medium">
                            {item.teacher}
                          </td>
                          <td className="px-5 py-4 text-sm text-slate-500">
                            {item.schedule}
                          </td>
                          <td className="px-5 py-4">
                            <div className="flex items-center gap-2">
                              <div className="h-1.5 w-16 overflow-hidden rounded-full bg-slate-100">
                                <div
                                  className={`h-full rounded-full ${
                                    item.students === item.max
                                      ? 'bg-blue-700'
                                      : 'bg-blue-500'
                                  }`}
                                  style={{
                                    width: `${(item.students / item.max) * 100}%`,
                                  }}
                                />
                              </div>
                              <span className="text-xs font-bold">
                                {item.students}/{item.max}
                              </span>
                            </div>
                          </td>
                          <td className="px-5 py-4">
                            <span
                              className={`rounded-full px-2.5 py-1 text-xs font-bold ${
                                item.status === 'Sắp đầy'
                                  ? 'bg-blue-700 text-white'
                                  : item.status === 'Sắp mở'
                                    ? 'bg-blue-500 text-white'
                                    : 'bg-blue-600 text-white'
                              }`}
                            >
                              {item.status}
                            </span>
                          </td>
                          <td className="px-5 py-4">
                            <button className="text-slate-400 hover:text-blue-600">
                              <Icon name="more" className="size-5" />
                            </button>
                          </td>
                        </tr>
                      ))}
                    </tbody>
                  </table>
                </div>
                <div className="flex flex-col items-center justify-between gap-3 border-t border-slate-100 px-5 py-4 text-xs text-slate-500 sm:flex-row">
                  <span>
                    Hiển thị {filteredClasses.length} trong 42 lớp học
                  </span>
                  <div className="flex items-center gap-1">
                    <button className="rounded-lg border border-slate-200 px-3 py-2">
                      Trước
                    </button>
                    <button className="rounded-lg bg-blue-600 px-3 py-2 text-white">
                      1
                    </button>
                    <button className="rounded-lg border border-slate-200 px-3 py-2">
                      2
                    </button>
                    <button className="rounded-lg border border-slate-200 px-3 py-2">
                      3
                    </button>
                    <button className="rounded-lg border border-slate-200 px-3 py-2">
                      Sau
                    </button>
                  </div>
                </div>
              </article>
            </section>
          ) : (
            <section className="grid min-h-[70vh] place-items-center">
              <div className="max-w-md text-center">
                <span className="mx-auto grid size-16 place-items-center rounded-2xl bg-blue-50 text-blue-600">
                  <Icon
                    name={
                      navByRole[role].find((item) => item.label === active)
                        ?.icon ?? 'grid'
                    }
                    className="size-8"
                  />
                </span>
                <h1 className="mt-5 text-3xl font-bold">{active}</h1>
                <p className="mt-3 leading-7 text-slate-500">
                  Phân hệ {active.toLowerCase()} đã sẵn sàng kết nối API. Nội dung được
                  hiển thị theo quyền của vai trò {role.toLowerCase()}.
                </p>
                <button
                  onClick={() => setActive('Tổng quan')}
                  className="mt-6 rounded-xl bg-blue-600 px-5 py-3 text-sm font-bold text-white"
                >
                  Quay lại tổng quan
                </button>
              </div>
            </section>
          )}
        </main>
      </div>
    </div>
  )
}