import { useState } from 'react'
import { Link } from 'react-router'
import { COURSES } from '../duLieu'
import { Icon } from '../components/BieuTuong'

const FILTERS = ['Tất cả', 'Mầm non', 'Tiểu học', 'THCS', 'Giao tiếp', 'IELTS']

function fmt(n: number) {
  return n.toLocaleString('vi-VN') + 'đ'
}

export default function Courses() {
  const [filter, setFilter] = useState('Tất cả')
  const [expanded, setExpanded] = useState<string | null>(null)

  const visible = filter === 'Tất cả' ? COURSES : COURSES.filter(c => c.tag === filter)

  return (
    <>
      {/* HERO */}
      <section style={{ background: 'linear-gradient(135deg, #EFF6FF 0%, #EFF6FF 100%)', padding: '64px 24px 72px' }}>
        <div style={{ maxWidth: 720, margin: '0 auto', textAlign: 'center' }}>
          <div style={{ color: '#2563EB', fontWeight: 700, fontSize: 13, letterSpacing: '0.1em', textTransform: 'uppercase', marginBottom: 12 }}>Chương trình đào tạo</div>
          <h1 style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 900, fontSize: 'clamp(32px, 5vw, 52px)', margin: '0 0 16px', lineHeight: 1.2 }}>
            5 chương trình<br />theo từng độ tuổi & mục tiêu
          </h1>
          <p style={{ color: '#64748B', fontSize: 18, lineHeight: 1.7, margin: '0 0 32px' }}>
            Mầm non, Tiểu học, THCS, Giao tiếp và IELTS — mỗi chương trình có lộ trình cấp độ riêng, học phí niêm yết theo khóa.
          </p>
          <div style={{ display: 'flex', gap: 16, justifyContent: 'center', flexWrap: 'wrap' }}>
            <Link to="/hocphi" style={{ background: '#2563EB', color: '#fff', padding: '12px 24px', borderRadius: 10, textDecoration: 'none', fontWeight: 700 }}>Xem học phí</Link>
            <a href="#courses" style={{ display: 'inline-flex', alignItems: 'center', gap: 6, background: '#fff', color: '#2563EB', padding: '12px 24px', borderRadius: 10, textDecoration: 'none', fontWeight: 700, border: '2px solid #BFDBFE' }}>
              Xem chương trình <Icon ten="ArrowDown" size={16} />
            </a>
          </div>
        </div>
      </section>

      {/* FILTER + COURSES */}
      <section id="courses" style={{ padding: '60px 24px 80px' }}>
        <div style={{ maxWidth: 1200, margin: '0 auto' }}>
          {/* Filter tabs */}
          <div style={{ display: 'flex', gap: 10, marginBottom: 48, flexWrap: 'wrap', justifyContent: 'center' }}>
            {FILTERS.map(f => (
              <button key={f} onClick={() => setFilter(f)} style={{
                padding: '9px 22px', borderRadius: 100, fontWeight: 600, fontSize: 14, cursor: 'pointer',
                border: filter === f ? 'none' : '1.5px solid #E2E8F0',
                background: filter === f ? '#2563EB' : '#fff',
                color: filter === f ? '#fff' : '#64748B',
                transition: 'all 0.2s',
              }}>{f}</button>
            ))}
          </div>

          {/* Course cards */}
          <div style={{ display: 'flex', flexDirection: 'column', gap: 32 }}>
            {visible.map(c => {
              const isOpen = expanded === c.tag
              return (
                <div key={c.tag} style={{ background: '#fff', borderRadius: 20, border: '1px solid #E2E8F0', overflow: 'hidden', boxShadow: isOpen ? '0 8px 40px rgba(0,0,0,0.08)' : 'none', transition: 'box-shadow 0.3s' }}>
                  {/* Top row */}
                  <div style={{ display: 'grid', gridTemplateColumns: '380px 1fr', gap: 0 }} className="course-card-grid">
                    {/* Image */}
                    <div style={{ position: 'relative', background: c.bg, minHeight: 260 }}>
                      <img src={c.img} alt={c.title} style={{ width: '100%', height: '100%', objectFit: 'cover', display: 'block', minHeight: 260 }} />
                      <div style={{ position: 'absolute', inset: 0, background: 'linear-gradient(to right, rgba(0,0,0,0.18), transparent)' }} />
                      <div style={{ position: 'absolute', top: 16, left: 16, background: c.color, color: '#fff', borderRadius: 100, padding: '5px 14px', fontWeight: 700, fontSize: 13 }}>{c.tag}</div>
                    </div>

                    {/* Info */}
                    <div style={{ padding: '32px 32px 28px' }}>
                      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', flexWrap: 'wrap', gap: 12, marginBottom: 12 }}>
                        <h2 style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 900, fontSize: 24, margin: 0 }}>{c.title}</h2>
                        <div style={{ display: 'flex', gap: 20 }}>
                          <div style={{ textAlign: 'center' }}>
                            <div style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 20, color: c.color, display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 4 }}>
                              <Icon ten="Star" size={16} fill="currentColor" /> {c.rating}
                            </div>
                            <div style={{ color: '#94A3B8', fontSize: 12 }}>Đánh giá</div>
                          </div>
                          <div style={{ textAlign: 'center' }}>
                            <div style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 20, color: c.color }}>{c.students}</div>
                            <div style={{ color: '#94A3B8', fontSize: 12 }}>Học viên</div>
                          </div>
                        </div>
                      </div>

                      <p style={{ color: '#64748B', fontSize: 15, lineHeight: 1.7, margin: '0 0 20px' }}>{c.desc}</p>

                      {/* Highlights */}
                      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: 8, marginBottom: 24 }}>
                        {c.highlights.map(h => (
                          <div key={h} style={{ display: 'flex', alignItems: 'center', gap: 8, fontSize: 14, color: '#374151' }}>
                            <Icon ten="Check" size={16} color="#2563EB" style={{ flexShrink: 0 }} /> {h}
                          </div>
                        ))}
                      </div>

                      <div style={{ display: 'flex', alignItems: 'center', gap: 16, flexWrap: 'wrap' }}>
                        <div style={{ display: 'flex', alignItems: 'center', gap: 6, background: '#EFF6FF', color: '#1E40AF', borderRadius: 8, padding: '6px 12px', fontSize: 13, fontWeight: 600 }}>
                          <Icon ten="CalendarDays" size={14} /> {c.schedule}
                        </div>
                        <button onClick={() => setExpanded(isOpen ? null : c.tag)} style={{
                          marginLeft: 'auto', background: isOpen ? '#F8FAFC' : c.color, color: isOpen ? c.color : '#fff',
                          border: isOpen ? `2px solid ${c.color}` : 'none',
                          padding: '10px 22px', borderRadius: 10, fontWeight: 700, fontSize: 14, cursor: 'pointer', transition: 'all 0.2s',
                          display: 'inline-flex', alignItems: 'center', gap: 6,
                        }}>
                          {isOpen ? 'Thu gọn' : 'Xem cấp độ & Đăng ký'}
                          <Icon ten={isOpen ? 'ChevronUp' : 'ChevronDown'} size={16} />
                        </button>
                      </div>
                    </div>
                  </div>

                  {/* Expanded: levels */}
                  {isOpen && (
                    <div style={{ borderTop: '1px solid #E2E8F0', padding: '32px', background: '#FAFBFC' }}>
                      <h3 style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 18, margin: '0 0 20px' }}>Chọn cấp độ phù hợp</h3>
                      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(260px, 1fr))', gap: 20, marginBottom: 28 }}>
                        {c.levels.map((lv, i) => (
                          <div key={lv.name} style={{
                            background: '#fff', borderRadius: 14, padding: '22px',
                            border: i === 1 ? `2px solid ${c.color}` : '1px solid #E2E8F0',
                            position: 'relative',
                          }}>
                            {i === 1 && <div style={{ position: 'absolute', top: -12, left: '50%', transform: 'translateX(-50%)', background: c.color, color: '#fff', borderRadius: 100, padding: '3px 14px', fontSize: 11, fontWeight: 700, whiteSpace: 'nowrap' }}>Phổ biến nhất</div>}
                            <div style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 18, color: c.color, marginBottom: 6 }}>{lv.name}</div>
                            <div style={{ color: '#64748B', fontSize: 14, marginBottom: 14 }}>{lv.target}</div>
                            <div style={{ display: 'flex', flexDirection: 'column', gap: 6, marginBottom: 18, fontSize: 14, color: '#374151' }}>
                              <span style={{ display: 'flex', alignItems: 'center', gap: 6 }}><Icon ten="Clock" size={14} /> Thời gian: <strong>{lv.duration}</strong></span>
                              <span style={{ display: 'flex', alignItems: 'center', gap: 6 }}><Icon ten="CalendarRange" size={14} /> Lịch học: <strong>{lv.sessions}</strong></span>
                              <span style={{ display: 'flex', alignItems: 'center', gap: 6 }}><Icon ten="Timer" size={14} /> Khối lượng: <strong>{lv.buoi} buổi · {lv.gio} giờ</strong></span>
                              <span style={{ display: 'flex', alignItems: 'center', gap: 6 }}><Icon ten="Wallet" size={14} /> Học phí: <strong style={{ color: c.color }}>{fmt(lv.fee)}/khóa</strong></span>
                            </div>
                            <Link to="/hocphi" style={{
                              display: 'block', textAlign: 'center',
                              background: i === 1 ? c.color : '#F8FAFC',
                              color: i === 1 ? '#fff' : c.color,
                              padding: '10px', borderRadius: 8, textDecoration: 'none', fontWeight: 700, fontSize: 14,
                              border: i === 1 ? 'none' : `1.5px solid ${c.color}`,
                            }}>Đăng ký ngay</Link>
                          </div>
                        ))}
                      </div>
                      <div style={{ textAlign: 'center' }}>
                        <p style={{ color: '#64748B', fontSize: 14, margin: '0 0 12px' }}>Chưa biết chọn cấp độ nào? Liên hệ tư vấn miễn phí!</p>
                        <a href="tel:18006868" style={{ display: 'inline-flex', alignItems: 'center', gap: 8, background: '#2563EB', color: '#fff', padding: '12px 28px', borderRadius: 10, textDecoration: 'none', fontWeight: 700, fontSize: 14 }}>
                          <Icon ten="Phone" size={16} /> Gọi 1800 6868
                        </a>
                      </div>
                    </div>
                  )}
                </div>
              )
            })}
          </div>
        </div>
      </section>

      {/* BOTTOM CTA */}
      <section style={{ padding: '60px 24px', background: '#F8FAFC', borderTop: '1px solid #E2E8F0' }}>
        <div style={{ maxWidth: 700, margin: '0 auto', textAlign: 'center' }}>
          <h2 style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 900, fontSize: 32, margin: '0 0 12px' }}>Chưa biết nên chọn chương trình nào?</h2>
          <p style={{ color: '#64748B', fontSize: 16, margin: '0 0 28px' }}>Làm bài test trình độ miễn phí — chúng tôi sẽ đề xuất lộ trình phù hợp nhất cho bạn.</p>
          <div style={{ display: 'flex', gap: 16, justifyContent: 'center', flexWrap: 'wrap' }}>
            <Link to="/hocphi" style={{ display: 'inline-flex', alignItems: 'center', gap: 6, background: '#2563EB', color: '#fff', padding: '13px 28px', borderRadius: 10, textDecoration: 'none', fontWeight: 700 }}>
              Xem học phí <Icon ten="ArrowRight" size={16} />
            </Link>
            <a href="tel:18006868" style={{ display: 'inline-flex', alignItems: 'center', gap: 8, background: '#fff', color: '#2563EB', padding: '13px 28px', borderRadius: 10, textDecoration: 'none', fontWeight: 700, border: '2px solid #BFDBFE' }}>
              <Icon ten="Phone" size={16} /> Tư vấn miễn phí
            </a>
          </div>
        </div>
      </section>

      <style>{`
        @media (max-width: 900px) {
          .course-card-grid { grid-template-columns: 1fr !important; }
        }
      `}</style>
    </>
  )
}
