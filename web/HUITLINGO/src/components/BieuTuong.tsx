import type { SVGProps } from "react"
import {
  ArrowDown,
  ArrowRight,
  ArrowUp,
  Backpack,
  BadgeCheck,
  Blocks,
  BookUser,
  Bot,
  CalendarCheck,
  CalendarDays,
  CalendarRange,
  Check,
  ChevronDown,
  ChevronUp,
  Circle,
  Clock,
  GraduationCap,
  MessageCircle,
  Phone,
  Play,
  Presentation,
  Rocket,
  Shield,
  Star,
  Target,
  Timer,
  Trophy,
  Users,
  Wallet,
  X,
  BookOpen,
} from "lucide-react"
import type { LucideIcon } from "lucide-react"

export type IconName =
  | "logo"
  | "grid"
  | "book"
  | "users"
  | "calendar"
  | "userPlus"
  | "bell"
  | "wallet"
  | "chart"
  | "settings"
  | "search"
  | "chevronDown"
  | "chevronLeft"
  | "chevronRight"
  | "arrowUp"
  | "arrowRight"
  | "clock"
  | "room"
  | "check"
  | "alert"
  | "menu"
  | "close"
  | "mail"
  | "lock"
  | "eye"
  | "logout"
  | "sparkles"
  | "more"
  | "filter"
  // --- Bổ sung: icon lucide cho trang công khai ---
  | "star"
  | "target"
  | "blocks"
  | "backpack"
  | "messageCircle"
  | "timer"
  | "trophy"
  | "presentation"
  | "calendarCheck"
  | "calendarRange"
  | "graduationCap"
  | "rocket"
  | "shield"
  | "badgeCheck"
  | "bookUser"
  | "play"
  | "arrowDown"
  | "chevronUp"
  | "phone"
  | "bot"

/** Icon lấy từ thư viện lucide-react — hỗ trợ cả tên camelCase và PascalCase. */
const LUCIDE: Record<string, LucideIcon> = {
  star: Star,
  Star: Star,
  target: Target,
  Target: Target,
  blocks: Blocks,
  Blocks: Blocks,
  backpack: Backpack,
  Backpack: Backpack,
  messageCircle: MessageCircle,
  MessageCircle: MessageCircle,
  timer: Timer,
  Timer: Timer,
  trophy: Trophy,
  Trophy: Trophy,
  presentation: Presentation,
  Presentation: Presentation,
  calendarCheck: CalendarCheck,
  CalendarCheck: CalendarCheck,
  calendarRange: CalendarRange,
  CalendarRange: CalendarRange,
  graduationCap: GraduationCap,
  GraduationCap: GraduationCap,
  rocket: Rocket,
  Rocket: Rocket,
  shield: Shield,
  Shield: Shield,
  badgeCheck: BadgeCheck,
  BadgeCheck: BadgeCheck,
  bookUser: BookUser,
  BookUser: BookUser,
  bookOpen: BookOpen,
  BookOpen: BookOpen,
  play: Play,
  Play: Play,
  arrowDown: ArrowDown,
  ArrowDown: ArrowDown,
  arrowUp: ArrowUp,
  ArrowUp: ArrowUp,
  arrowRight: ArrowRight,
  ArrowRight: ArrowRight,
  chevronUp: ChevronUp,
  ChevronUp: ChevronUp,
  phone: Phone,
  Phone: Phone,
  bot: Bot,
  Bot: Bot,
  check: Check,
  Check: Check,
  calendar: CalendarDays,
  CalendarDays: CalendarDays,
  clock: Clock,
  Clock: Clock,
  wallet: Wallet,
  Wallet: Wallet,
  users: Users,
  Users: Users,
  close: X,
  X: X,
  chevronDown: ChevronDown,
  ChevronDown: ChevronDown,
}

const paths: Partial<Record<IconName, React.ReactNode>> = {
  logo: (
    <>
      <path d="m3 8 9-5 9 5-9 5-9-5Z" />
      <path d="M7 10.2V15c2.8 2.5 7.2 2.5 10 0v-4.8M21 8v7" />
    </>
  ),
  grid: (
    <>
      <rect x="3" y="3" width="7" height="7" rx="2" />
      <rect x="14" y="3" width="7" height="7" rx="2" />
      <rect x="3" y="14" width="7" height="7" rx="2" />
      <rect x="14" y="14" width="7" height="7" rx="2" />
    </>
  ),
  book: (
    <>
      <path d="M4 19.5A2.5 2.5 0 0 1 6.5 17H20" />
      <path d="M6.5 2H20v20H6.5A2.5 2.5 0 0 1 4 19.5v-15A2.5 2.5 0 0 1 6.5 2Z" />
    </>
  ),
  users: (
    <>
      <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2" />
      <circle cx="9" cy="7" r="4" />
      <path d="M22 21v-2a4 4 0 0 0-3-3.87M16 3.13a4 4 0 0 1 0 7.75" />
    </>
  ),
  calendar: (
    <>
      <rect x="3" y="5" width="18" height="16" rx="2" />
      <path d="M16 3v4M8 3v4M3 10h18" />
    </>
  ),
  userPlus: (
    <>
      <path d="M15 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
      <circle cx="8" cy="7" r="4" />
      <path d="M19 8v6M22 11h-6" />
    </>
  ),
  bell: (
    <>
      <path d="M18 8a6 6 0 0 0-12 0c0 7-3 7-3 9h18c0-2-3-2-3-9" />
      <path d="M13.7 21a2 2 0 0 1-3.4 0" />
    </>
  ),
  wallet: (
    <>
      <path d="M20 7V5a2 2 0 0 0-2-2H5a3 3 0 0 0 0 6h15v12H5a3 3 0 0 1-3-3V6" />
      <path d="M16 13h4" />
    </>
  ),
  chart: (
    <>
      <path d="M3 3v18h18" />
      <path d="m7 16 4-5 4 3 5-7" />
    </>
  ),
  settings: (
    <>
      <circle cx="12" cy="12" r="3" />
      <path d="M19.4 15a1.7 1.7 0 0 0 .34 1.88l.06.06-2.83 2.83-.06-.06A1.7 1.7 0 0 0 15 19.4a1.7 1.7 0 0 0-1 .6 1.7 1.7 0 0 0-.4 1.1V21h-4v-.1A1.7 1.7 0 0 0 8.6 19.4a1.7 1.7 0 0 0-1.88.34l-.06.06-2.83-2.83.06-.06A1.7 1.7 0 0 0 4.6 15a1.7 1.7 0 0 0-.6-1 1.7 1.7 0 0 0-1.1-.4H3v-4h.1A1.7 1.7 0 0 0 4.6 8.6a1.7 1.7 0 0 0-.34-1.88l-.06-.06 2.83-2.83.06.06A1.7 1.7 0 0 0 9 4.6a1.7 1.7 0 0 0 1-.6 1.7 1.7 0 0 0 .4-1.1V3h4v.1A1.7 1.7 0 0 0 15.4 4.6a1.7 1.7 0 0 0 1.88-.34l.06-.06 2.83 2.83-.06.06A1.7 1.7 0 0 0 19.4 9c.14.36.35.69.6 1 .3.3.7.45 1.1.4h.1v4h-.1a1.7 1.7 0 0 0-1.7.6Z" />
    </>
  ),
  search: (
    <>
      <circle cx="11" cy="11" r="7" />
      <path d="m20 20-4-4" />
    </>
  ),
  chevronDown: <path d="m6 9 6 6 6-6" />,
  chevronLeft: <path d="m15 6-6 6 6 6" />,
  chevronRight: <path d="m9 6 6 6-6 6" />,
  arrowUp: (
    <>
      <path d="m18 15-6-6-6 6" />
      <path d="M12 9v10" />
    </>
  ),
  arrowRight: (
    <>
      <path d="M5 12h14" />
      <path d="m13 6 6 6-6 6" />
    </>
  ),
  clock: (
    <>
      <circle cx="12" cy="12" r="9" />
      <path d="M12 7v5l3 2" />
    </>
  ),
  room: (
    <>
      <path d="M20 10c0 5-8 11-8 11S4 15 4 10a8 8 0 1 1 16 0Z" />
      <circle cx="12" cy="10" r="2" />
    </>
  ),
  check: <path d="m5 12 4 4L19 6" />,
  alert: (
    <>
      <path d="M10.3 3.7 2.5 18a2 2 0 0 0 1.8 3h15.4a2 2 0 0 0 1.8-3L13.7 3.7a2 2 0 0 0-3.4 0Z" />
      <path d="M12 9v4M12 17h.01" />
    </>
  ),
  menu: (
    <>
      <path d="M4 6h16M4 12h16M4 18h16" />
    </>
  ),
  close: (
    <>
      <path d="m6 6 12 12M18 6 6 18" />
    </>
  ),
  mail: (
    <>
      <rect x="3" y="5" width="18" height="14" rx="2" />
      <path d="m3 7 9 6 9-6" />
    </>
  ),
  lock: (
    <>
      <rect x="4" y="10" width="16" height="11" rx="2" />
      <path d="M8 10V7a4 4 0 0 1 8 0v3" />
    </>
  ),
  eye: (
    <>
      <path d="M2 12s3.5-6 10-6 10 6 10 6-3.5 6-10 6S2 12 2 12Z" />
      <circle cx="12" cy="12" r="2.5" />
    </>
  ),
  logout: (
    <>
      <path d="M10 17l5-5-5-5M15 12H3" />
      <path d="M15 3h4a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-4" />
    </>
  ),
  sparkles: (
    <>
      <path d="m12 3 1.3 3.7L17 8l-3.7 1.3L12 13l-1.3-3.7L7 8l3.7-1.3L12 3Z" />
      <path d="m5 14 .8 2.2L8 17l-2.2.8L5 20l-.8-2.2L2 17l2.2-.8L5 14ZM19 13l.7 1.8 1.8.7-1.8.7L19 18l-.7-1.8-1.8-.7 1.8-.7L19 13Z" />
    </>
  ),
  more: (
    <>
      <circle cx="5" cy="12" r="1" />
      <circle cx="12" cy="12" r="1" />
      <circle cx="19" cy="12" r="1" />
    </>
  ),
  filter: <path d="M4 5h16l-6 7v5l-4 2v-7L4 5Z" />,
}

export function Icon({
  name,
  ten,
  size,
  ...props
}: { name?: IconName | string; ten?: IconName | string; size?: number } & SVGProps<SVGSVGElement>) {
  const key = (name ?? ten ?? "") as string

  const Comp = LUCIDE[key]
  if (Comp) {
    return (
      <Comp size={size ?? 24} strokeWidth={1.8} aria-hidden="true" {...props} />
    )
  }

  if (paths[key as IconName]) {
    return (
      <svg
        viewBox="0 0 24 24"
        fill="none"
        stroke="currentColor"
        strokeWidth="1.8"
        strokeLinecap="round"
        strokeLinejoin="round"
        aria-hidden="true"
        {...(size ? { width: size, height: size } : {})}
        {...props}
      >
        {paths[key as IconName]}
      </svg>
    )
  }

  return <Circle size={size ?? 24} strokeWidth={1.8} aria-hidden="true" {...props} />
}