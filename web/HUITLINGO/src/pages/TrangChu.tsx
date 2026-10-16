import { Link } from 'react-router'
import { COURSES, TEACHERS, TESTIMONIALS, STATS } from '../duLieu'

const FEATURES = [
  { icon: '🏆', color: '#2563EB', bg: '#EFF6FF', title: 'Cam kết đầu ra', desc: 'Hoàn tiền 100% nếu không đạt mục tiêu đã cam kết sau khóa học' },
  { icon: '👨‍🏫', color: '#2563EB', bg: '#EFF6FF', title: 'Giáo viên bản ngữ', desc: '30% giáo viên native speaker từ Mỹ, Anh, Úc — tất cả đều có chứng chỉ quốc tế' },
  { icon: '📱', color: '#2563EB', bg: '#EFF6FF', title: 'Học mọi lúc mọi nơi', desc: 'App học 24/7 với 5,000+ bài tập, video bài giảng và AI chấm điểm Speaking' },
  { icon: '👥', color: '#2563EB', bg: '#EFF6FF', title: 'Lớp học nhỏ', desc: 'Tối đa 12 học viên/lớp để giáo viên có thể quan tâm từng học sinh' },
]

export default function Home() {
  return (
    <>
      {/* HERO */}
      <section style={{ background: 'linear-gradient(135deg, #EFF6FF 0%, #EFF6FF 50%, #EFF6FF 100%)', padding: '80px 24px 100px', position: 'relative', overflow: 'hidden' }}>
        <div style={{ position: 'absolute', top: -80, right: -80, width: 400, height: 400, borderRadius: '50%', background: 'rgba(37,99,235,0.06)' }} />
        <div style={{ position: 'absolute', bottom: -60, left: -60, width: 300, height: 300, borderRadius: '50%', background: 'rgba(139,92,246,0.06)' }} />

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
                cùng HUIT LingoHub
              </span>{' '}
              🚀
            </h1>
            <p style={{ color: '#64748B', fontSize: 20, lineHeight: 1.7, margin: '0 0 40px', maxWidth: 520 }}>
              Lộ trình học cá nhân hóa, giáo viên bản ngữ, cam kết đầu ra. Hơn <strong style={{ color: '#1E293B' }}>15,000 học viên</strong> đã đạt mục tiêu cùng chúng tôi.
            </p>

            <div style={{ display: 'flex', gap: 16, flexWrap: 'wrap' }}>
              <Link to="/khoahoc" style={{
                background: '#2563EB', color: '#fff', padding: '16px 32px', borderRadius: 12, textDecoration: 'none',
                fontWeight: 700, fontSize: 18, boxShadow: '0 4px 24px rgba(37,99,235,0.3)', transition: 'transform 0.2s, box-shadow 0.2s',
                whiteSpace: 'nowrap',
              }}
                onMouseEnter={e => { e.currentTarget.style.transform = 'translateY(-2px)'; e.currentTarget.style.boxShadow = '0 8px 32px rgba(37,99,235,0.4)' }}
                onMouseLeave={e => { e.currentTarget.style.transform = 'translateY(0)'; e.currentTarget.style.boxShadow = '0 4px 24px rgba(37,99,235,0.3)' }}
              >▶ Bắt đầu học ngay</Link>
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
              {[{ icon: '✅', text: 'Cam kết hoàn tiền' }, { icon: '🏅', text: 'Chứng chỉ quốc tế' }, { icon: '📞', text: 'Hỗ trợ 24/7' }].map(b => (
                <div key={b.text} style={{ display: 'flex', alignItems: 'center', gap: 8 }}>
                  <span style={{ fontSize: 18 }}>{b.icon}</span>
                  <span style={{ color: '#64748B', fontSize: 16, fontWeight: 500, whiteSpace: 'nowrap' }}>{b.text}</span>
                </div>
              ))}
            </div>
          </div>

          <div style={{ position: 'relative' }}>
            <img src="https://images.unsplash.com/photo-1522202176988-66273c2fd55f?w=600&h=500&fit=crop&auto=format"
              alt="Học viên đang học tại HUIT LingoHub"
              style={{ width: '100%', borderRadius: 20, boxShadow: '0 20px 60px rgba(0,0,0,0.12)', display: 'block' }} />
            <div style={{ position: 'absolute', top: -20, left: -20, background: '#fff', borderRadius: 16, padding: '14px 18px', boxShadow: '0 8px 30px rgba(0,0,0,0.1)', display: 'flex', alignItems: 'center', gap: 12 }}>
              <div style={{ fontSize: 32 }}>🏆</div>
              <div>
                <div style={{ fontFamily: "'Be Vietnam Pro', 'Inter', sans-serif", fontWeight: 800, fontSize: 20, color: '#2563EB' }}>IELTS 8.0+</div>
                <div style={{ color: '#64748B', fontSize: 14 }}>Học viên đạt điểm cao nhất</div>
              </div>
            </div>
            <div style={{ position: 'absolute', bottom: 30, right: -20, background: '#fff', borderRadius: 16, padding: '14px 18px', boxShadow: '0 8px 30px rgba(0,0,0,0.1)' }}>
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
              <div style={{ fontFamily: "'Be Vietnam Pro', 'Inter', sans-serif", fontWeight: 900, fontSize: 40, color: '#fff', marginBottom: 6 }}>{s.value}</div>
              <div style={{ color: '#BFDBFE', fontSize: 16 }}>{s.label}</div>
            </div>
          ))}
        </div>
      </section>

      {/* WHY US */}
      <section style={{ padding: '80px 24px', background: '#F8FAFC' }}>
        <div style={{ maxWidth: 1200, margin: '0 auto' }}>
          <div style={{ textAlign: 'center', marginBottom: 56 }}>
            <div style={{ color: '#2563EB', fontWeight: 700, fontSize: 15, letterSpacing: '0.1em', textTransform: 'uppercase', marginBottom: 14 }}>Tại sao chọn HUIT LingoHub?</div>
            <h2 style={{ fontFamily: "'Be Vietnam Pro', 'Inter', sans-serif", fontWeight: 900, fontSize: 'clamp(32px, 4.5vw, 48px)', margin: '0 0 20px', lineHeight: 1.15 }}>Học tiếng Anh hiệu quả hơn<br />với phương pháp của chúng tôi</h2>
            <p style={{ color: '#64748B', fontSize: 19, maxWidth: 600, margin: '0 auto', lineHeight: 1.7 }}>Hệ thống học tập kết hợp giữa lớp học trực tiếp và công nghệ AI — tối ưu hóa cho từng học viên</p>
          </div>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(260px, 1fr))', gap: 24 }}>
            {FEATURES.map(f => (
              <div key={f.title} style={{ background: '#fff', borderRadius: 16, padding: '32px 28px', border: '1px solid #E2E8F0', transition: 'transform 0.2s, box-shadow 0.2s', cursor: 'default' }}
                onMouseEnter={e => { e.currentTarget.style.transform = 'translateY(-4px)'; e.currentTarget.style.boxShadow = '0 12px 40px rgba(0,0,0,0.08)' }}
                onMouseLeave={e => { e.currentTarget.style.transform = 'translateY(0)'; e.currentTarget.style.boxShadow = 'none' }}
              >
                <div style={{ width: 64, height: 64, borderRadius: 16, background: f.bg, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 32, marginBottom: 24 }}>{f.icon}</div>
                <h3 style={{ fontFamily: "'Be Vietnam Pro', 'Inter', sans-serif", fontWeight: 800, fontSize: 22, margin: '0 0 12px', color: f.color }}>{f.title}</h3>
                <p style={{ color: '#64748B', fontSize: 17, lineHeight: 1.7, margin: 0 }}>{f.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* COURSES PREVIEW */}
      <section style={{ padding: '80px 24px' }}>
        <div style={{ maxWidth: 1200, margin: '0 auto' }}>
          <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-end', marginBottom: 48, flexWrap: 'wrap', gap: 16 }}>
            <div>
              <div style={{ color: '#2563EB', fontWeight: 700, fontSize: 15, letterSpacing: '0.1em', textTransform: 'uppercase', marginBottom: 14 }}>Khóa học</div>
              <h2 style={{ fontFamily: "'Be Vietnam Pro', 'Inter', sans-serif", fontWeight: 900, fontSize: 'clamp(32px, 4.5vw, 48px)', margin: 0 }}>Chương trình học đa dạng</h2>
            </div>
            <Link to="/khoahoc" style={{ color: '#2563EB', fontWeight: 700, textDecoration: 'none', fontSize: 17, display: 'flex', alignItems: 'center', gap: 6, whiteSpace: 'nowrap' }}>Xem tất cả →</Link>
          </div>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(260px, 1fr))', gap: 24 }}>
            {COURSES.slice(0, 4).map(c => (
              <Link key={c.tag} to="/khoahoc" style={{ textDecoration: 'none', color: 'inherit' }}>
                <div style={{ background: '#fff', borderRadius: 20, overflow: 'hidden', border: '1px solid #E2E8F0', transition: 'transform 0.2s, box-shadow 0.2s', height: '100%' }}
                  onMouseEnter={e => { e.currentTarget.style.transform = 'translateY(-4px)'; e.currentTarget.style.boxShadow = '0 16px 48px rgba(0,0,0,0.1)' }}
                  onMouseLeave={e => { e.currentTarget.style.transform = 'translateY(0)'; e.currentTarget.style.boxShadow = 'none' }}
                >
                  <div style={{ background: c.bg, padding: '28px 24px 24px' }}>
                    <span style={{ display: 'inline-block', background: c.color, color: '#fff', borderRadius: 100, padding: '5px 14px', fontSize: 13, fontWeight: 700, marginBottom: 14 }}>{c.tag}</span>
                    <div style={{ fontSize: 38, marginBottom: 12 }}>{c.icon}</div>
                    <h3 style={{ fontFamily: "'Be Vietnam Pro', 'Inter', sans-serif", fontWeight: 800, fontSize: 21, margin: '0 0 10px' }}>{c.title}</h3>
                    <p style={{ color: '#64748B', fontSize: 16, lineHeight: 1.7, margin: 0 }}>{c.desc}</p>
                  </div>
                  <div style={{ padding: '18px 24px', display: 'flex', justifyContent: 'space-between', fontSize: 15, color: '#64748B' }}>
                    <span>⭐ {c.rating}</span>
                    <span>👥 {c.students}</span>
                    <span style={{ color: c.color, fontWeight: 700 }}>Xem thêm →</span>
                  </div>
                </div>
              </Link>
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
            <Link to="/giaovien-public" style={{ color: '#2563EB', fontWeight: 700, textDecoration: 'none', fontSize: 17, whiteSpace: 'nowrap' }}>Xem tất cả →</Link>
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
                  {Array(t.stars).fill(0).map((_, i) => <span key={i} style={{ color: '#2563EB', fontSize: 16 }}>★</span>)}
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
            fontWeight: 800, fontSize: 18, display: 'inline-block', boxShadow: '0 4px 24px rgba(0,0,0,0.2)', transition: 'transform 0.2s',
            whiteSpace: 'nowrap',
          }}
            onMouseEnter={e => (e.currentTarget.style.transform = 'translateY(-2px)')}
            onMouseLeave={e => (e.currentTarget.style.transform = 'translateY(0)')}
          >Học thử miễn phí →</Link>
        </div>
      </section>

      <style>{`
        @media (max-width: 900px) {
          .hero-grid { grid-template-columns: 1fr !important; }
          .stats-grid { grid-template-columns: repeat(2, 1fr) !important; }
        }
      `}</style>
    </>
  )
}