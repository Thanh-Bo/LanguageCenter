import { useState } from 'react'
import { Link, useLocation, Outlet } from 'react-router'
import logo from '../assets/logo.png'
import TroLyTuVan from './TroLyTuVan'
import { Icon } from './BieuTuong'
import { NAV_LINKS } from '../duLieu'
import { dungXacThuc, duongDanTheoVaiTro } from '../contexts/NguCanhXacThuc'

export default function BoCuc() {
  const [menuOpen, setMenuOpen] = useState(false)
  const { pathname } = useLocation()
  const { nguoiDung, dangXuat } = dungXacThuc()

  return (
    <div
      style={{
        color: '#1E293B',
        background: '#fff',
        minHeight: '100vh',
      }}
    >
      {/* NAVBAR */}
      <nav
        style={{
          position: 'sticky',
          top: 0,
          zIndex: 50,
          background: 'rgba(255,255,255,0.95)',
          backdropFilter: 'blur(12px)',
          borderBottom: '1px solid #E2E8F0',
          padding: '0 24px',
        }}
      >
        <div
          style={{
            maxWidth: 1200,
            margin: '0 auto',
            display: 'flex',
            alignItems: 'center',
            justifyContent: 'space-between',
            height: 68,
            gap: 16,
          }}
        >
          {/* LOGO */}
          <Link
            to="/"
            style={{
              display: 'flex',
              alignItems: 'center',
              gap: 10,
              textDecoration: 'none',
              flexShrink: 0,
            }}
          >
            <img
              src={logo}
              alt="LingoHub"
              style={{
                width: 40,
                height: 40,
                borderRadius: '50%',
                objectFit: 'cover',
                flexShrink: 0,
                boxShadow: '0 4px 12px rgba(37,99,235,0.3)',
              }}
            />
            <span
              style={{
                fontFamily: "'Be Vietnam Pro', 'Inter', sans-serif",
                fontWeight: 800,
                fontSize: 20,
                color: '#1E293B',
                whiteSpace: 'nowrap',
              }}
            >
              <span style={{ color: '#2563EB' }}>LingoHub</span>
            </span>
          </Link>

          {/* NAV LINKS */}
          <div
            style={{ display: 'flex', gap: 32, alignItems: 'center' }}
            className="desktop-nav"
          >
            {NAV_LINKS.map(({ label, path }) => {
              const active =
                path === '/' ? pathname === '/' : pathname.startsWith(path)
              return (
                <Link
                  key={label}
                  to={path}
                  style={{
                    color: active ? '#2563EB' : '#64748B',
                    textDecoration: 'none',
                    fontSize: 15,
                    fontWeight: active ? 700 : 500,
                    borderBottom: active
                      ? '2px solid #2563EB'
                      : '2px solid transparent',
                    paddingBottom: 2,
                    whiteSpace: 'nowrap',
                    transition: 'color 0.2s',
                  }}
                  onMouseEnter={(e) =>
                    !active && (e.currentTarget.style.color = '#2563EB')
                  }
                  onMouseLeave={(e) =>
                    !active && (e.currentTarget.style.color = '#64748B')
                  }
                >
                  {label}
                </Link>
              )
            })}
          </div>

          {/* RIGHT ACTIONS */}
          <div
            style={{
              display: 'flex',
              gap: 12,
              alignItems: 'center',
              flexShrink: 0,
            }}
          >
            {nguoiDung ? (
              <>
                <Link
                  to={duongDanTheoVaiTro(nguoiDung.vaiTro)}
                  className="desktop-nav"
                  style={{
                    color: '#475569',
                    textDecoration: 'none',
                    fontWeight: 700,
                    fontSize: 14,
                    whiteSpace: 'nowrap',
                  }}
                >
                  Vào Portal
                </Link>
                <button
                  onClick={dangXuat}
                  className="desktop-nav"
                  style={{
                    background: 'none',
                    border: 'none',
                    color: '#64748B',
                    fontWeight: 700,
                    fontSize: 14,
                    cursor: 'pointer',
                    padding: 0,
                    whiteSpace: 'nowrap',
                    transition: 'color 0.2s',
                  }}
                  onMouseEnter={(e) => (e.currentTarget.style.color = '#DC2626')}
                  onMouseLeave={(e) =>
                    (e.currentTarget.style.color = '#64748B')
                  }
                >
                  Đăng xuất
                </button>
              </>
            ) : (
              <Link
                to="/dangnhap"
                className="desktop-nav"
                style={{
                  color: '#475569',
                  textDecoration: 'none',
                  fontWeight: 700,
                  fontSize: 14,
                  whiteSpace: 'nowrap',
                }}
              >
                Đăng nhập
              </Link>
            )}
            <Link
              to="/khoahoc"
              style={{
                background: '#2563EB',
                color: '#fff',
                padding: '10px 20px',
                borderRadius: 10,
                textDecoration: 'none',
                fontWeight: 700,
                fontSize: 14,
                whiteSpace: 'nowrap',
                flexShrink: 0,
                transition: 'background 0.2s',
              }}
              onMouseEnter={(e) =>
                (e.currentTarget.style.background = '#1D4ED8')
              }
              onMouseLeave={(e) =>
                (e.currentTarget.style.background = '#2563EB')
              }
            >
              Tư vấn miễn phí
            </Link>
            <button
              onClick={() => setMenuOpen(!menuOpen)}
              style={{
                display: 'none',
                background: 'none',
                border: 'none',
                cursor: 'pointer',
                padding: 4,
                fontSize: 22,
              }}
              className="mobile-menu-btn"
            >
              ☰
            </button>
          </div>
        </div>

        {/* MOBILE MENU */}
        {menuOpen && (
          <div
            style={{
              background: '#fff',
              borderTop: '1px solid #E2E8F0',
              padding: '16px 24px',
              display: 'flex',
              flexDirection: 'column',
              gap: 16,
            }}
          >
            {NAV_LINKS.map(({ label, path }) => (
              <Link
                key={label}
                to={path}
                onClick={() => setMenuOpen(false)}
                style={{
                  color: '#1E293B',
                  textDecoration: 'none',
                  fontWeight: 600,
                  fontSize: 16,
                }}
              >
                {label}
              </Link>
            ))}
            {nguoiDung ? (
              <>
                <Link
                  to={duongDanTheoVaiTro(nguoiDung.vaiTro)}
                  onClick={() => setMenuOpen(false)}
                  style={{
                    color: '#2563EB',
                    textDecoration: 'none',
                    fontWeight: 700,
                    fontSize: 16,
                  }}
                >
                  Vào Portal
                </Link>
                <button
                  onClick={() => {
                    dangXuat()
                    setMenuOpen(false)
                  }}
                  style={{
                    background: 'none',
                    border: 'none',
                    color: '#DC2626',
                    fontWeight: 700,
                    fontSize: 16,
                    cursor: 'pointer',
                    padding: 0,
                    textAlign: 'left',
                  }}
                >
                  Đăng xuất
                </button>
              </>
            ) : (
              <Link
                to="/dangnhap"
                onClick={() => setMenuOpen(false)}
                style={{
                  color: '#2563EB',
                  textDecoration: 'none',
                  fontWeight: 700,
                  fontSize: 16,
                }}
              >
                Đăng nhập
              </Link>
            )}
          </div>
        )}
      </nav>

      <Outlet />

      {/* FOOTER */}
      <footer
        style={{
          background: '#1E293B',
          color: '#CBD5E1',
          padding: '48px 24px 28px',
        }}
      >
        <div style={{ maxWidth: 1200, margin: '0 auto' }}>
          <div
            style={{
              display: 'grid',
              gridTemplateColumns: '2fr 1fr 1fr 1fr',
              gap: 40,
              marginBottom: 40,
            }}
            className="footer-grid"
          >
            <div>
              <div
                style={{
                  display: 'flex',
                  alignItems: 'center',
                  gap: 10,
                  marginBottom: 16,
                }}
              >
                <div
                  style={{
                    width: 36,
                    height: 36,
                    borderRadius: 9,
                    background: 'linear-gradient(135deg, #2563EB, #1D4ED8)',
                    display: 'flex',
                    alignItems: 'center',
                    justifyContent: 'center',
                    fontSize: 18,
                    fontWeight: 900,
                    color: '#fff',
                    fontFamily: "'Be Vietnam Pro', 'Inter', sans-serif",
                  }}
                >
                  L
                </div>
                <span
                  style={{
                    fontFamily: "'Be Vietnam Pro', 'Inter', sans-serif",
                    fontWeight: 800,
                    fontSize: 18,
                    color: '#fff',
                  }}
                >
                  LingoHub
                </span>
              </div>
              <p
                style={{
                  fontSize: 14,
                  lineHeight: 1.7,
                  maxWidth: 280,
                  margin: '0 0 20px',
                  color: '#94A3B8',
                }}
              >
                Trung tâm Anh ngữ hàng đầu TP.HCM với 12 năm kinh nghiệm đào
                tạo và hơn 15,000 học viên tốt nghiệp.
              </p>
              <div style={{ display: 'flex', gap: 12 }}>
                {['f', 'in', 'yt', 'mail'].map((s) => (
                  <div
                    key={s}
                    style={{
                      width: 36,
                      height: 36,
                      borderRadius: 8,
                      background: '#334155',
                      display: 'flex',
                      alignItems: 'center',
                      justifyContent: 'center',
                      fontSize: 13,
                      fontWeight: 700,
                      color: '#94A3B8',
                      cursor: 'pointer',
                      transition: 'background 0.2s',
                    }}
                    onMouseEnter={(e) =>
                      (e.currentTarget.style.background = '#2563EB')
                    }
                    onMouseLeave={(e) =>
                      (e.currentTarget.style.background = '#334155')
                    }
                  >
                    {s === 'mail' ? <Icon name="mail" className="size-4" /> : s}
                  </div>
                ))}
              </div>
            </div>
            {[
              {
                title: 'Chương trình',
                items: [
                  'Mầm non',
                  'Tiểu học',
                  'THCS',
                  'Giao tiếp',
                  'IELTS',
                ],
              },
              {
                title: 'Về chúng tôi',
                items: [
                  'Giới thiệu',
                  'Đội ngũ GV',
                  'Cơ sở vật chất',
                  'Học bổng',
                  'Tuyển dụng',
                ],
              },
              {
                title: 'Hỗ trợ',
                items: [
                  'Câu hỏi thường gặp',
                  'Lịch khai giảng',
                  'Chính sách hoàn tiền',
                  'Điều khoản',
                  'Liên hệ',
                ],
              },
            ].map((col) => (
              <div key={col.title}>
                <h4
                  style={{
                    fontFamily: "'Be Vietnam Pro', 'Inter', sans-serif",
                    fontWeight: 700,
                    color: '#fff',
                    fontSize: 15,
                    margin: '0 0 16px',
                  }}
                >
                  {col.title}
                </h4>
                <ul
                  style={{
                    listStyle: 'none',
                    padding: 0,
                    margin: 0,
                    display: 'flex',
                    flexDirection: 'column',
                    gap: 10,
                  }}
                >
                  {col.items.map((item) => (
                    <li key={item}>
                      <a
                        href="#"
                        style={{
                          color: '#94A3B8',
                          textDecoration: 'none',
                          fontSize: 14,
                          transition: 'color 0.2s',
                        }}
                        onMouseEnter={(e) =>
                          (e.currentTarget.style.color = '#fff')
                        }
                        onMouseLeave={(e) =>
                          (e.currentTarget.style.color = '#94A3B8')
                        }
                      >
                        {item}
                      </a>
                    </li>
                  ))}
                </ul>
              </div>
            ))}
          </div>
          <div
            style={{
              borderTop: '1px solid #334155',
              paddingTop: 24,
              display: 'flex',
              justifyContent: 'space-between',
              alignItems: 'center',
              flexWrap: 'wrap',
              gap: 12,
            }}
          >
            <p style={{ margin: 0, fontSize: 13, color: '#64748B' }}>
              © 2026 LingoHub. Bảo lưu mọi quyền.
            </p>
            <p style={{ margin: 0, fontSize: 13, color: '#64748B' }}>
              Giấy phép hoạt động số 1234/GCN-SGDĐT-HCM
            </p>
          </div>
        </div>
      </footer>

      <TroLyTuVan />

      <style>{`
        @media (max-width: 900px) {
          .desktop-nav { display: none !important; }
          .mobile-menu-btn { display: flex !important; }
          .footer-grid { grid-template-columns: 1fr 1fr !important; }
        }
        @media (max-width: 600px) {
          .footer-grid { grid-template-columns: 1fr !important; }
        }
      `}</style>
    </div>
  )
}