import type { ApiProblem } from './types'

const BASE: string = import.meta.env.VITE_API_BASE ?? '/api'
const TOKEN_KEY = 'huitlingo_token'

/** Lỗi API có kèm status + danh sách lỗi validate từ backend. */
export class ApiError extends Error {
  status: number
  errors: string[]

  constructor(status: number, message: string, errors: string[] = []) {
    super(message)
    this.name = 'ApiError'
    this.status = status
    this.errors = errors
  }
}

let xuLyUnauthorized: (() => void) | null = null

/** Đăng ký callback chạy khi gặp 401 (dùng để tự đăng xuất). */
export function datXuLyUnauthorized(fn: (() => void) | null) {
  xuLyUnauthorized = fn
}

export function layToken(): string | null {
  return localStorage.getItem(TOKEN_KEY)
}

export function luuToken(token: string) {
  localStorage.setItem(TOKEN_KEY, token)
}

export function xoaToken() {
  localStorage.removeItem(TOKEN_KEY)
}

type Query = Record<string, string | number | boolean | null | undefined>

/** Ghép query string, bỏ qua giá trị null/undefined/rỗng. */
export function qs(query?: Query): string {
  if (!query) return ''

  const parts: string[] = []

  for (const [key, value] of Object.entries(query)) {
    if (value === null || value === undefined || value === '') continue
    parts.push(`${encodeURIComponent(key)}=${encodeURIComponent(String(value))}`)
  }

  return parts.length ? `?${parts.join('&')}` : ''
}

async function request<T>(
  method: string,
  path: string,
  body?: unknown,
  options: { khongChan401?: boolean } = {},
): Promise<T> {
  const token = layToken()

  const headers: Record<string, string> = { Accept: 'application/json' }
  if (body !== undefined) headers['Content-Type'] = 'application/json'
  if (token) headers.Authorization = `Bearer ${token}`

  let response: Response

  try {
    response = await fetch(`${BASE}${path}`, {
      method,
      headers,
      body: body === undefined ? undefined : JSON.stringify(body),
    })
  } catch {
    throw new ApiError(0, 'Không kết nối được tới máy chủ. Kiểm tra backend đã chạy chưa.')
  }

  if (response.status === 204) {
    return undefined as T
  }

  const text = await response.text()
  const data = text ? safeJson(text) : null

  if (!response.ok) {
    const problem = data as ApiProblem | null
    const thongBao =
      problem?.message || `Yêu cầu thất bại (HTTP ${response.status})`

    if (response.status === 401 && !options.khongChan401) {
      xoaToken()
      xuLyUnauthorized?.()
    }

    throw new ApiError(response.status, thongBao, problem?.errors ?? [])
  }

  return data as T
}

function safeJson(text: string): unknown {
  try {
    return JSON.parse(text)
  } catch {
    return null
  }
}

export const http = {
  get: <T>(path: string) => request<T>('GET', path),
  post: <T>(path: string, body?: unknown) => request<T>('POST', path, body),
  patch: <T>(path: string, body?: unknown) => request<T>('PATCH', path, body),
  delete: <T>(path: string) => request<T>('DELETE', path),
  /** Dùng cho /auth/me khi khởi động: không kích hoạt đăng xuất toàn cục. */
  getKhongChan401: <T>(path: string) => request<T>('GET', path, undefined, { khongChan401: true }),
}
