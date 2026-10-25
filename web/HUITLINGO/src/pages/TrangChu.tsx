import { Link } from 'react-router'
import { TEACHERS, TESTIMONIALS, STATS } from '../duLieu'
import { Icon } from '../components/BieuTuong'

/**
 * BẢNG MÀU DUY NHẤT của trang chủ: chỉ xanh nước biển + xám trung tính.
 * Dùng chung cho mọi section để không bị "mỗi chỗ một kiểu".
 */
const UC = {
  brand: '#1D4ED8',
  blue: '#2563EB',
  light: '#EFF6FF',
  border: '#BFDBFE',
  ink: '#0F172A',
  body: '#475569',
  muted: '#64748B',
  line: '#E2E8F0',
  bg: '#F8FAFC',
} as const

const FONT = "'Be Vietnam Pro', 'Inter', system-ui, -apple-system, sans-serif"

const FEATURES = [
  { icon: 'Trophy', color: '#2563EB', bg: '#EFF6FF', title: 'Cam kết đầu ra', desc: 'Học lại miễn phí nếu chưa đạt mục tiêu và đủ điều kiện chuyên cần từ 80%' },
  { icon: 'Presentation', color: '#2563EB', bg: '#EFF6FF', title: 'Chứng chỉ quốc tế', desc: 'Giáo viên có TESOL/TEFL/CELTA; giáo viên IELTS đạt 7.5 – 8.0+' },
  { icon: 'CalendarCheck', color: '#2563EB', bg: '#EFF6FF', title: 'Học bù linh hoạt', desc: 'Vắng có phép được học bù tối đa 2 buổi mỗi khóa, xếp vào lớp cùng cấp độ' },
  { icon: 'Users', color: '#2563EB', bg: '#EFF6FF', title: 'Lớp học nhỏ', desc: 'Mầm non 10 · Tiểu học 12 · THCS 15 · Giao tiếp 12 · IELTS 14 học viên/lớp' },
]

export default function Home() {
  return (
    <>
      {/* HERO */}
      <section style={{ background: `linear-gradient(180deg, ${UC.light} 0%, #FFFFFF 78%)`, padding: '72px 24px 88px', position: 'relative', overflow: 'hidden' }}>
        <div style={{ position: 'absolute', top: -120, right: -100, width: 420, height: 420, borderRadius: '50%', background: 'rgba(37,99,235,0.10)' }} />
        <div style={{ position: 'absolute', bottom: -80, left: -80, width: 320, height: 320, borderRadius: '50%', background: 'rgba(37,99,235,0.06)' }} />

        <div style={{ maxWidth: 1200, margin: '0 auto', display: 'grid', gridTemplateColumns: '1.1fr 1fr', gap: 60, alignItems: 'center' }} className="hero-grid">
          <div>
            <div style={{ display: 'inline-flex', alignItems: 'center', gap: 8, background: '#EFF6FF', border: '1px solid #BFDBFE', borderRadius: 100, padding: '8px 18px', marginBottom: 28 }}>
              <span style={{ width: 8, height: 8, borderRadius: '50%', background: '#2563EB', display: 'inline-block' }} />
              <span style={{ color: '#2563EB', fontWeight: 600, fontSize: 15 }}>Trung tâm Anh ngữ hàng đầu TP.HCM</span>
            </div>

            <h1 style={{
              fontFamily: "'Be Vietnam Pro', 'Inter', sans-serif",
              fontWeight: 800,
              fontSize: 'clamp(34px, 4.5vw, 56px)',
              lineHeight: 1.15,
              margin: '0 0 24px',
              letterSpacing: '-0.02em',
            }}>
              Chinh phục tiếng Anh{' '}
              <span style={{ color: '#2563EB' }}>
                cùng LingoHub
              </span>
            </h1>
            <p style={{ color: '#64748B', fontSize: 20, lineHeight: 1.7, margin: '0 0 40px', maxWidth: 520 }}>
              Lộ trình học cá nhân hóa, giáo viên có chứng chỉ quốc tế, cam kết đầu ra. Hơn <strong style={{ color: '#1E293B' }}>15,000 học viên</strong> đã đạt mục tiêu cùng chúng tôi.
            </p>

            <div style={{ display: 'flex', gap: 16, flexWrap: 'wrap' }}>
              <Link to="/khoahoc" style={{
                display: 'inline-flex', alignItems: 'center', gap: 8,
                background: '#2563EB', color: '#fff', padding: '16px 32px', borderRadius: 12, textDecoration: 'none',
                fontWeight: 700, fontSize: 18, boxShadow: '0 4px 24px rgba(37,99,235,0.3)', transition: 'transform 0.2s, box-shadow 0.2s',
                whiteSpace: 'nowrap',
              }}
                onMouseEnter={e => { e.currentTarget.style.transform = 'translateY(-2px)'; e.currentTarget.style.boxShadow = '0 8px 32px rgba(37,99,235,0.4)' }}
                onMouseLeave={e => { e.currentTarget.style.transform = 'translateY(0)'; e.currentTarget.style.boxShadow = '0 4px 24px rgba(37,99,235,0.3)' }}
              >
                <Icon ten="Play" size={18} /> Bắt đầu học ngay
              </Link>
              <Link to="/giaovien-public" style={{
                background: '#fff', color: '#2563EB', padding: '16px 32px', borderRadius: 12, textDecoration: 'none',
                fontWeight: 700, fontSize: 18, border: '2px solid #BFDBFE', transition: 'border-color 0.2s',
                whiteSpace: 'nowrap',
              }}
                onMouseEnter={e => (e.currentTarget.style.borderColor = '#2563EB')}
                onMouseLeave={e => (e.currentTarget.style.borderColor = '#BFDBFE')}
              >Xem giáo viên</Link>
            </div>

            <div style={{ display: 'flex', gap: 28, marginTop: 44, flexWrap: 'wrap' }}>
              {[{ icon: 'Check', text: 'Cam kết đầu ra' }, { icon: 'BadgeCheck', text: 'Chứng chỉ quốc tế' }, { icon: 'Phone', text: 'Tư vấn miễn phí' }].map(b => (
                <div key={b.text} style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                  <Icon ten={b.icon} size={18} color="#2563EB" />
                  <span style={{ color: '#64748B', fontSize: 16, fontWeight: 500, whiteSpace: 'nowrap' }}>{b.text}</span>
                </div>
              ))}
            </div>
          </div>

          <div style={{ position: 'relative' }}>
            <img src="https://images.unsplash.com/photo-1522202176988-66273c2fd55f?w=600&h=500&fit=crop&auto=format"
              alt="Học viên đang học tại LingoHub"
              style={{ width: '100%', borderRadius: 20, boxShadow: '0 20px 60px rgba(0,0,0,0.12)', display: 'block' }} />
            <div style={{ position: 'absolute', top: -20, left: -20, background: '#fff', borderRadius: 16, padding: '14px 18px', boxShadow: '0 8px 30px rgba(0,0,0,0.1)', display: 'flex', alignItems: 'center', gap: 12 }} className="hero-float">
              <div style={{ color: '#2563EB', display: 'flex' }}><Icon ten="Trophy" size={32} /></div>
              <div>
                <div style={{ fontFamily: "'Be Vietnam Pro', 'Inter', sans-serif", fontWeight: 800, fontSize: 20, color: '#2563EB' }}>IELTS 8.0+</div>
                <div style={{ color: '#64748B', fontSize: 14 }}>Học viên đạt điểm cao nhất</div>
              </div>
            </div>
            <div style={{ position: 'absolute', bottom: 30, right: -20, background: '#fff', borderRadius: 16, padding: '14px 18px', boxShadow: '0 8px 30px rgba(0,0,0,0.1)' }} className="hero-float">
              <div style={{ fontFamily: "'Be Vietnam Pro', 'Inter', sans-serif", fontWeight: 800, fontSize: 16, marginBottom: 10 }}>Học viên mới tháng này</div>
              <div style={{ display: 'flex' }}>
                {['photo-1438761681033-6461ffad8d80', 'photo-1500648767791-00dcc994a43e', 'photo-1494790108377-be9c29b29330'].map((id, i) => (
                  <img key={id} src={`https://images.unsplash.com/${id}?w=40&h=40&fit=crop&auto=format`} alt=""
                    style={{ width: 40, height: 40, borderRadius: '50%', border: '2px solid #fff', marginLeft: i > 0 ? -12 : 0 }} />
                ))}
                <div style={{ width: 40, height: 40, borderRadius: '50%', background: '#EFF6FF', border: '2px solid #fff', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 12, fontWeight: 700, color: '#2563EB', marginLeft: -12 }}>+48</div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* STATS */}
      <section style={{ background: '#2563EB', padding: '48px 24px' }}>
        <div style={{ maxWidth: 1200, margin: '0 auto', display: 'grid', gridTemplateColumns: 'repeat(4, 1fr)', gap: 24, textAlign: 'center' }} className="stats-grid">
          {STATS.map(s => (
            <div key={s.label}>
              <div style={{ fontFamily: "'Be Vietnam Pro', 'Inter', sans-serif", fontWeight: 900, fontSize: 40, color: '#fff', marginBottom: 6, display: 'flex', alignItems: 'center', justifyContent: 'center', gap: 8 }}>
                {s.value}
                {'icon' in s && s.icon && <Icon ten={s.icon} size={28} fill="currentColor" />}
              </div>
              <div style={{ color: '#BFDBFE', fontSize: 16 }}>{s.label}</div>
            </div>
          ))}
        </div>
      </section>

      {/* WHY US */}
      <section style={{ padding: '72px 24px', background: UC.bg }}>
        <div style={{ maxWidth: 1200, margin: '0 auto' }}>
          <div style={{ textAlign: 'center', marginBottom: 44 }}>
            <div className="tc-eyebrow">Tại sao chọn LingoHub?</div>
            <h2 className="tc-h2" style={{ margin: '0 auto 16px', maxWidth: 720 }}>
              Học tiếng Anh hiệu quả hơn với phương pháp của chúng tôi
            </h2>
            <p style={{ color: UC.muted, fontSize: 18, maxWidth: 600, margin: '0 auto', lineHeight: 1.7 }}>
              Kết hợp lớp học trực tiếp với hệ thống theo dõi tiến độ từng buổi
            </p>
          </div>

          <div className="tc-why-grid">
            {FEATURES.map((f) => (
              <div key={f.title} className="tc-why-cell">
                <h3 className="tc-why-title">
                  <span className="tc-why-icon">
                    <Icon ten={f.icon} size={20} />
                  </span>
                  {f.title}
                </h3>
                <p className="tc-why-desc">{f.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* TEACHERS PREVIEW */}
      <section style={{ padding: '80px 24px', background: '#F8FAFC' }}>
        <div style={{ maxWidth: 1200, margin: '0 auto' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-end', marginBottom: 48, flexWrap: 'wrap', gap: 16 }}>
            <div>
              <div style={{ color: '#2563EB', fontWeight: 700, fontSize: 15, letterSpacing: '0.1em', textTransform: 'uppercase', marginBottom: 14 }}>Đội ngũ giáo viên</div>
              <h2 style={{ fontFamily: "'Be Vietnam Pro', 'Inter', sans-serif", fontWeight: 900, fontSize: 'clamp(32px, 4.5vw, 48px)', margin: 0 }}>Học cùng những người giỏi nhất</h2>
            </div>
            <Link to="/giaovien-public" style={{ color: '#2563EB', fontWeight: 700, textDecoration: 'none', fontSize: 17, whiteSpace: 'nowrap', display: 'flex', alignItems: 'center', gap: 6 }}>
              Xem tất cả <Icon ten="ArrowRight" size={16} />
            </Link>
          </div>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(240px, 1fr))', gap: 24 }}>
            {TEACHERS.slice(0, 4).map(t => (
              <div key={t.name} style={{ background: '#fff', borderRadius: 20, overflow: 'hidden', border: '1px solid #E2E8F0', transition: 'transform 0.2s, box-shadow 0.2s' }}
                onMouseEnter={e => { e.currentTarget.style.transform = 'translateY(-4px)'; e.currentTarget.style.boxShadow = '0 16px 48px rgba(0,0,0,0.1)' }}
                onMouseLeave={e => { e.currentTarget.style.transform = 'translateY(0)'; e.currentTarget.style.boxShadow = 'none' }}
              >
                <div style={{ position: 'relative', height: 220, background: '#EFF6FF' }}>
                  <img src={t.img} alt={t.name} style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
                  <div style={{ position: 'absolute', top: 12, right: 12, background: '#2563EB', color: '#fff', borderRadius: 100, padding: '5px 14px', fontSize: 13, fontWeight: 700 }}>{t.score}</div>
                </div>
                <div style={{ padding: '20px 22px' }}>
                  <h3 style={{ fontFamily: "'Be Vietnam Pro', 'Inter', sans-serif", fontWeight: 800, fontSize: 19, margin: '0 0 6px' }}>{t.name}</h3>
                  <p style={{ color: '#64748B', fontSize: 15, margin: '0 0 12px' }}>{t.role} · {t.exp}</p>
                  <div style={{ display: 'flex', gap: 6, flexWrap: 'wrap' }}>
                    {t.tags.map(tag => <span key={tag} style={{ background: '#EFF6FF', color: '#2563EB', borderRadius: 6, padding: '4px 12px', fontSize: 13, fontWeight: 600 }}>{tag}</span>)}
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* TESTIMONIALS */}
      <section style={{ padding: '80px 24px' }}>
        <div style={{ maxWidth: 1200, margin: '0 auto' }}>
          <div style={{ textAlign: 'center', marginBottom: 56 }}>
            <div style={{ color: '#2563EB', fontWeight: 700, fontSize: 15, letterSpacing: '0.1em', textTransform: 'uppercase', marginBottom: 14 }}>Học viên nói gì?</div>
            <h2 style={{ fontFamily: "'Be Vietnam Pro', 'Inter', sans-serif", fontWeight: 900, fontSize: 'clamp(32px, 4.5vw, 48px)', margin: 0 }}>Câu chuyện thành công thực tế</h2>
          </div>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))', gap: 24 }}>
            {TESTIMONIALS.map(t => (
              <div key={t.name} style={{ background: '#F8FAFC', borderRadius: 20, padding: '32px', border: '1px solid #E2E8F0', position: 'relative' }}>
                <div style={{ position: 'absolute', top: 28, right: 28, display: 'flex', gap: 2 }}>
                  {Array(t.stars).fill(0).map((_, i) => <Icon key={i} ten="Star" size={16} fill="currentColor" />)}
                </div>
                <div style={{ fontSize: 40, color: '#BFDBFE', fontFamily: 'Georgia, serif', lineHeight: 1, marginBottom: 18 }}>"</div>
                <p style={{ color: '#374151', fontSize: 17, lineHeight: 1.75, margin: '0 0 28px' }}>{t.text}</p>
                <div style={{ display: 'flex', alignItems: 'center', gap: 14 }}>
                  <img src={t.avatar} alt={t.name} style={{ width: 52, height: 52, borderRadius: '50%', objectFit: 'cover' }} />
                  <div>
                    <div style={{ fontFamily: "'Be Vietnam Pro', 'Inter', sans-serif", fontWeight: 800, fontSize: 17 }}>{t.name}</div>
                    <div style={{ color: '#64748B', fontSize: 15 }}>{t.course}</div>
                  </div>
                  <div style={{ marginLeft: 'auto', background: '#EFF6FF', color: '#1D4ED8', borderRadius: 8, padding: '5px 12px', fontSize: 13, fontWeight: 700, whiteSpace: 'nowrap' }}>{t.score}</div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA */}
      <section style={{ padding: '72px 24px', background: 'linear-gradient(135deg, #2563EB 0%, #1D4ED8 100%)' }}>
        <div style={{ maxWidth: 800, margin: '0 auto', textAlign: 'center' }}>
          <h2 style={{ fontFamily: "'Be Vietnam Pro', 'Inter', sans-serif", fontWeight: 900, fontSize: 'clamp(32px, 4.5vw, 52px)', color: '#fff', margin: '0 0 18px', lineHeight: 1.2 }}>Sẵn sàng bắt đầu hành trình?</h2>
          <p style={{ color: '#BFDBFE', fontSize: 20, margin: '0 0 40px', lineHeight: 1.6 }}>Đăng ký học thử miễn phí ngay hôm nay — không cần thanh toán trước</p>
          <Link to="/khoahoc" style={{
            background: '#fff', color: '#2563EB', padding: '18px 40px', borderRadius: 12, textDecoration: 'none',
            fontWeight: 800, fontSize: 18, display: 'inline-flex', alignItems: 'center', gap: 8, boxShadow: '0 4px 24px rgba(0,0,0,0.2)', transition: 'transform 0.2s',
            whiteSpace: 'nowrap',
          }}
            onMouseEnter={e => (e.currentTarget.style.transform = 'translateY(-2px)')}
            onMouseLeave={e => (e.currentTarget.style.transform = 'translateY(0)')}
          >
            Học thử miễn phí <Icon ten="ArrowRight" size={18} />
          </Link>
        </div>
      </section>

      <style>{`
        /* ===== Kiểu chữ dùng chung ===== */
        .tc-eyebrow {
          color: #1D4ED8;
          font-weight: 700;
          font-size: 13px;
          letter-spacing: .08em;
          text-transform: uppercase;
          text-align: center;
          margin-bottom: 12px;
        }
        .tc-h2 {
          font-family: 'Be Vietnam Pro', 'Inter', system-ui, sans-serif;
          font-weight: 900;
          font-size: clamp(28px, 3.6vw, 40px);
          line-height: 1.2;
          color: #0F172A;
        }

        /* ===== VÌ SAO CHỌN: lưới 2x2 kẻ đường, KHÔNG khung 4 card ===== */
        .tc-why-grid {
          display: grid;
          grid-template-columns: repeat(2, minmax(0, 1fr));
          background: #fff;
          border: 1px solid #E2E8F0;
          border-radius: 20px;
          overflow: hidden;
        }
        .tc-why-cell {
          padding: 30px 32px;
          border-right: 1px solid #E2E8F0;
          border-bottom: 1px solid #E2E8F0;
        }
        .tc-why-cell:nth-child(2n) { border-right: none; }
        .tc-why-cell:nth-child(n + 3) { border-bottom: none; }
        .tc-why-title {
          display: flex;
          align-items: center;
          gap: 12px;
          margin: 0 0 10px;
          font-family: 'Be Vietnam Pro', 'Inter', system-ui, sans-serif;
          font-size: 19px;
          font-weight: 800;
          color: #0F172A;
        }
        .tc-why-icon {
          display: inline-flex;
          align-items: center;
          justify-content: center;
          width: 34px;
          height: 34px;
          border-radius: 10px;
          background: #EFF6FF;
          color: #2563EB;
          flex-shrink: 0;
        }
        .tc-why-desc {
          margin: 0;
          color: #475569;
          font-size: 16px;
          line-height: 1.7;
        }

        @media (max-width: 760px) {
          .tc-why-grid { grid-template-columns: 1fr; }
          .tc-why-cell { border-right: none; }
          .tc-why-cell:nth-child(n) { border-bottom: 1px solid #E2E8F0; }
          .tc-why-cell:last-child { border-bottom: none; }
        }

        @media (max-width: 900px) {
          .hero-grid { grid-template-columns: 1fr !important; gap: 44px !important; }
          .stats-grid { grid-template-columns: repeat(2, 1fr) !important; }
          /* Thẻ nổi đè lên ảnh trên màn nhỏ → ẩn hẳn */
          .hero-float { display: none !important; }
        }
        @media (max-width: 560px) {
          .stats-grid { grid-template-columns: 1fr !important; }
        }

        /* Trạng thái focus thống nhất cho bàn phím (trước đây chỉ có hover bằng JS) */
        a:focus-visible,
        button:focus-visible {
          outline: 3px solid rgba(37, 99, 235, 0.45);
          outline-offset: 3px;
          border-radius: 10px;
        }

        @media (prefers-reduced-motion: reduce) {
          * { transition: none !important; animation: none !important; }
        }
      `}</style>
    </>
  )
}