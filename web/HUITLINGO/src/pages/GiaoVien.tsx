import { useState } from 'react'
import { Link } from 'react-router'
import { TEACHERS } from '../duLieu'

const FILTER_TAGS = ['Tất cả', 'IELTS', 'TOEIC', 'Giao tiếp', 'Business', 'Kids English']

export default function Teachers() {
  const [filter, setFilter] = useState('Tất cả')
  const [selected, setSelected] = useState<typeof TEACHERS[0] | null>(null)

  const visible = filter === 'Tất cả'
    ? TEACHERS
    : TEACHERS.filter(t => t.tags.some(tag => tag.toLowerCase().includes(filter.toLowerCase()) || filter.toLowerCase().includes(tag.toLowerCase())))

  return (
    <>
      {/* HERO */}
      <section style={{ background: 'linear-gradient(135deg, #DBEAFE 0%, #EFF6FF 100%)', padding: '64px 24px 72px' }}>
        <div style={{ maxWidth: 720, margin: '0 auto', textAlign: 'center' }}>
          <div style={{ color: '#2563EB', fontWeight: 700, fontSize: 13, letterSpacing: '0.1em', textTransform: 'uppercase', marginBottom: 12 }}>Đội ngũ giáo viên</div>
          <h1 style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 900, fontSize: 'clamp(32px, 5vw, 52px)', margin: '0 0 16px', lineHeight: 1.2 }}>
            Học cùng những<br />chuyên gia hàng đầu
          </h1>
          <p style={{ color: '#64748B', fontSize: 18, lineHeight: 1.7, margin: '0 0 32px' }}>
            Đội ngũ giáo viên đa quốc tịch — 30% native speaker từ Mỹ, Anh, Úc, Canada — tất cả có chứng chỉ giảng dạy quốc tế.
          </p>
          {/* Quick stats */}
          <div style={{ display: 'flex', justifyContent: 'center', gap: 40, flexWrap: 'wrap' }}>
            {[
              { v: '40+', l: 'Giáo viên' },
              { v: '30%', l: 'Native speaker' },
              { v: '4.95', l: 'Điểm trung bình' },
            ].map(s => (
              <div key={s.l} style={{ textAlign: 'center' }}>
                <div style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 900, fontSize: 28, color: '#2563EB' }}>{s.v}</div>
                <div style={{ color: '#64748B', fontSize: 14 }}>{s.l}</div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* FILTER */}
      <section style={{ padding: '40px 24px 0' }}>
        <div style={{ maxWidth: 1200, margin: '0 auto' }}>
          <div style={{ display: 'flex', gap: 10, flexWrap: 'wrap', justifyContent: 'center' }}>
            {FILTER_TAGS.map(f => (
              <button key={f} onClick={() => setFilter(f)} style={{
                padding: '9px 22px', borderRadius: 100, fontWeight: 600, fontSize: 14, cursor: 'pointer',
                border: filter === f ? 'none' : '1.5px solid #E2E8F0',
                background: filter === f ? '#2563EB' : '#fff',
                color: filter === f ? '#fff' : '#64748B',
                transition: 'all 0.2s',
              }}>{f}</button>
            ))}
          </div>
        </div>
      </section>

      {/* TEACHERS GRID */}
      <section style={{ padding: '40px 24px 80px' }}>
        <div style={{ maxWidth: 1200, margin: '0 auto' }}>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))', gap: 28 }}>
            {visible.map(t => (
              <div key={t.name} onClick={() => setSelected(t)} style={{
                background: '#fff', borderRadius: 20, overflow: 'hidden',
                border: '1px solid #E2E8F0', cursor: 'pointer',
                transition: 'transform 0.2s, box-shadow 0.2s',
              }}
                onMouseEnter={e => { e.currentTarget.style.transform = 'translateY(-4px)'; e.currentTarget.style.boxShadow = '0 16px 48px rgba(0,0,0,0.1)' }}
                onMouseLeave={e => { e.currentTarget.style.transform = 'translateY(0)'; e.currentTarget.style.boxShadow = 'none' }}
              >
                {/* Photo */}
                <div style={{ position: 'relative', height: 240, background: '#EFF6FF' }}>
                  <img src={t.img} alt={t.name} style={{ width: '100%', height: '100%', objectFit: 'cover', display: 'block' }} />
                  <div style={{ position: 'absolute', inset: 0, background: 'linear-gradient(to top, rgba(0,0,0,0.35), transparent)' }} />
                  <div style={{ position: 'absolute', top: 14, right: 14, background: '#2563EB', color: '#fff', borderRadius: 100, padding: '5px 13px', fontSize: 12, fontWeight: 700 }}>{t.score}</div>
                  <div style={{ position: 'absolute', bottom: 14, left: 14, display: 'flex', gap: 6 }}>
                    <span style={{ fontSize: 18 }}>{t.flag}</span>
                    <span style={{ color: '#fff', fontSize: 13, fontWeight: 600, alignSelf: 'center' }}>{t.uni}</span>
                  </div>
                </div>

                {/* Info */}
                <div style={{ padding: '20px 22px 24px' }}>
                  <h3 style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 18, margin: '0 0 4px' }}>{t.name}</h3>
                  <p style={{ color: '#2563EB', fontWeight: 600, fontSize: 14, margin: '0 0 10px' }}>{t.role}</p>
                  <p style={{ color: '#64748B', fontSize: 14, lineHeight: 1.6, margin: '0 0 16px' }}>{t.bio}</p>

                  {/* Stats row */}
                  <div style={{ display: 'flex', gap: 20, marginBottom: 16, padding: '12px 0', borderTop: '1px solid #F1F5F9', borderBottom: '1px solid #F1F5F9' }}>
                    <div style={{ textAlign: 'center', flex: 1 }}>
                      <div style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 16, color: '#2563EB' }}>{t.exp}</div>
                      <div style={{ color: '#94A3B8', fontSize: 11 }}>Kinh nghiệm</div>
                    </div>
                    <div style={{ textAlign: 'center', flex: 1 }}>
                      <div style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 16, color: '#2563EB' }}>{t.students.toLocaleString()}+</div>
                      <div style={{ color: '#94A3B8', fontSize: 11 }}>Học viên</div>
                    </div>
                    <div style={{ textAlign: 'center', flex: 1 }}>
                      <div style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 16, color: '#2563EB' }}>★ {t.rating}</div>
                      <div style={{ color: '#94A3B8', fontSize: 11 }}>Đánh giá</div>
                    </div>
                  </div>

                  <div style={{ display: 'flex', gap: 6, flexWrap: 'wrap' }}>
                    {t.tags.map(tag => <span key={tag} style={{ background: '#EFF6FF', color: '#2563EB', borderRadius: 6, padding: '4px 10px', fontSize: 12, fontWeight: 600 }}>{tag}</span>)}
                  </div>
                </div>

                <div style={{ padding: '0 22px 22px' }}>
                  <button onClick={e => { e.stopPropagation(); setSelected(t) }} style={{
                    width: '100%', padding: '11px', borderRadius: 10, background: '#EFF6FF',
                    color: '#2563EB', border: '1.5px solid #BFDBFE', cursor: 'pointer', fontWeight: 700, fontSize: 14,
                  }}>Xem hồ sơ chi tiết</button>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* JOIN US */}
      <section style={{ padding: '60px 24px', background: '#1E293B' }}>
        <div style={{ maxWidth: 800, margin: '0 auto', textAlign: 'center' }}>
          <div style={{ fontSize: 36, marginBottom: 16 }}>🎓</div>
          <h2 style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 900, fontSize: 32, color: '#fff', margin: '0 0 12px' }}>Bạn là giáo viên giỏi tiếng Anh?</h2>
          <p style={{ color: '#94A3B8', fontSize: 16, margin: '0 0 28px' }}>HUIT LingoHub luôn tìm kiếm giáo viên tài năng, nhiệt huyết để cùng xây dựng thế hệ giỏi tiếng Anh.</p>
          <a href="mailto:careers@lingohub.edu.vn" style={{ background: '#2563EB', color: '#fff', padding: '13px 28px', borderRadius: 10, textDecoration: 'none', fontWeight: 700, fontSize: 15 }}>Ứng tuyển ngay →</a>
        </div>
      </section>

      {/* MODAL */}
      {selected && (
        <div onClick={() => setSelected(null)} style={{
          position: 'fixed', inset: 0, zIndex: 200,
          background: 'rgba(0,0,0,0.5)', backdropFilter: 'blur(4px)',
          display: 'flex', alignItems: 'center', justifyContent: 'center', padding: 24,
        }}>
          <div onClick={e => e.stopPropagation()} style={{
            background: '#fff', borderRadius: 24, maxWidth: 640, width: '100%',
            maxHeight: '90vh', overflowY: 'auto',
            animation: 'fadeIn 0.2s ease',
          }}>
            <div style={{ position: 'relative', height: 280 }}>
              <img src={selected.img} alt={selected.name} style={{ width: '100%', height: '100%', objectFit: 'cover' }} />
              <div style={{ position: 'absolute', inset: 0, background: 'linear-gradient(to top, rgba(0,0,0,0.6), transparent)' }} />
              <button onClick={() => setSelected(null)} style={{ position: 'absolute', top: 16, right: 16, background: 'rgba(0,0,0,0.4)', border: 'none', color: '#fff', width: 36, height: 36, borderRadius: '50%', cursor: 'pointer', fontSize: 18 }}>✕</button>
              <div style={{ position: 'absolute', bottom: 20, left: 24 }}>
                <h2 style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 900, color: '#fff', fontSize: 26, margin: '0 0 4px' }}>{selected.name}</h2>
                <p style={{ color: 'rgba(255,255,255,0.85)', fontSize: 15, margin: 0 }}>{selected.role} · {selected.flag} {selected.uni}</p>
              </div>
            </div>

            <div style={{ padding: '28px 28px 32px' }}>
              <div style={{ display: 'flex', gap: 20, marginBottom: 24 }}>
                {[
                  { v: selected.score, l: 'Chứng chỉ' },
                  { v: selected.exp, l: 'Kinh nghiệm' },
                  { v: `${selected.students}+`, l: 'Học viên' },
                  { v: `★ ${selected.rating}`, l: 'Đánh giá' },
                ].map(s => (
                  <div key={s.l} style={{ textAlign: 'center', flex: 1, background: '#F8FAFC', borderRadius: 12, padding: '12px 8px' }}>
                    <div style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 16, color: '#2563EB' }}>{s.v}</div>
                    <div style={{ color: '#94A3B8', fontSize: 11 }}>{s.l}</div>
                  </div>
                ))}
              </div>

              <h3 style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 17, margin: '0 0 10px' }}>Giới thiệu</h3>
              <p style={{ color: '#64748B', fontSize: 15, lineHeight: 1.7, margin: '0 0 20px' }}>{selected.bio}</p>

              <h3 style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 17, margin: '0 0 10px' }}>Khóa học đang dạy</h3>
              <div style={{ display: 'flex', gap: 8, flexWrap: 'wrap', marginBottom: 24 }}>
                {selected.courses.map(c => <span key={c} style={{ background: '#EFF6FF', color: '#2563EB', borderRadius: 8, padding: '6px 14px', fontWeight: 600, fontSize: 14 }}>{c}</span>)}
              </div>

              <Link to="/khoa-hoc" onClick={() => setSelected(null)} style={{
                display: 'block', textAlign: 'center', background: '#2563EB', color: '#fff',
                padding: '14px', borderRadius: 12, textDecoration: 'none', fontWeight: 800, fontSize: 16,
              }}>Đăng ký học với {selected.name.split(' ').pop()}</Link>
            </div>
          </div>
        </div>
      )}

      <style>{`@keyframes fadeIn { from { opacity:0; transform:scale(0.96); } to { opacity:1; transform:scale(1); } }`}</style>
    </>
  )
}
