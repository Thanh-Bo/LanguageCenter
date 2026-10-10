import { useState } from 'react'
import { Link } from 'react-router'
import { PRICING_PLANS, COURSES } from '../duLieu'

function fmt(n: number) {
  return n.toLocaleString('vi-VN') + 'đ'
}

const FAQS = [
  { q: 'Học phí có bao gồm tài liệu học không?', a: 'Gói Tiêu Chuẩn và VIP bao gồm tài liệu online và sách in. Gói Cơ Bản có tài liệu online. Tất cả có quyền truy cập app HUIT LingoHub 24/7.' },
  { q: 'Nếu không đạt mục tiêu thì sao?', a: 'Gói VIP cam kết hoàn tiền 100% nếu bạn học đủ giờ, làm đủ bài tập nhưng không đạt điểm mục tiêu sau khóa. Điều kiện cụ thể sẽ ghi trong hợp đồng học.' },
  { q: 'Có thể trả học phí theo tháng không?', a: 'Có. Bạn có thể trả theo tháng, 3 tháng (giảm 10%) hoặc 6 tháng (giảm 15%). Chúng tôi chấp nhận tiền mặt, chuyển khoản và thẻ tín dụng.' },
  { q: 'Có học bổng không?', a: 'Có học bổng 20–50% cho học viên xuất sắc, học sinh nghèo hiếu học và nhân viên các công ty đối tác. Liên hệ 1800 6868 để biết thêm chi tiết.' },
  { q: 'Có thể chuyển lớp hoặc tạm dừng học không?', a: 'Bạn có thể chuyển lớp 1 lần miễn phí. Bảo lưu tối đa 3 tháng với lý do hợp lệ (bệnh, công tác nước ngoài). Liên hệ trước 5 ngày để xử lý thủ tục.' },
  { q: 'Lớp học có mấy người?', a: 'Gói Cơ Bản: tối đa 15 người. Gói Tiêu Chuẩn: tối đa 12 người. Gói VIP: tối đa 8 người. Đảm bảo giáo viên có thể chú ý từng học viên.' },
]

export default function Pricing() {
  const [billing, setBilling] = useState<'month' | '3month' | '6month'>('month')
  const [openFaq, setOpenFaq] = useState<number | null>(null)
  const [openCourse, setOpenCourse] = useState<string | null>(null)

  const mult = billing === '3month' ? 0.9 : billing === '6month' ? 0.85 : 1
  const discountLabel = billing === '3month' ? '-10%' : billing === '6month' ? '-15%' : null

  return (
    <>
      {/* HERO */}
      <section style={{ background: 'linear-gradient(135deg, #EFF6FF 0%, #DBEAFE 100%)', padding: '64px 24px 72px' }}>
        <div style={{ maxWidth: 720, margin: '0 auto', textAlign: 'center' }}>
          <div style={{ color: '#1D4ED8', fontWeight: 700, fontSize: 13, letterSpacing: '0.1em', textTransform: 'uppercase', marginBottom: 12 }}>Học phí</div>
          <h1 style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 900, fontSize: 'clamp(32px, 5vw, 52px)', margin: '0 0 16px', lineHeight: 1.2 }}>
            Minh bạch, linh hoạt,<br />xứng đáng với từng đồng
          </h1>
          <p style={{ color: '#64748B', fontSize: 18, lineHeight: 1.7, margin: '0 0 32px' }}>
            Không phí ẩn. Không cam kết dài hạn bắt buộc. Hoàn tiền 100% nếu không hài lòng sau buổi học đầu tiên.
          </p>

          {/* Billing toggle */}
          <div style={{ display: 'inline-flex', background: '#fff', borderRadius: 14, padding: 6, gap: 4, boxShadow: '0 2px 12px rgba(0,0,0,0.08)' }}>
            {([
              { key: 'month', label: 'Theo tháng' },
              { key: '3month', label: '3 tháng  −10%' },
              { key: '6month', label: '6 tháng  −15%' },
            ] as const).map(opt => (
              <button key={opt.key} onClick={() => setBilling(opt.key)} style={{
                padding: '9px 18px', borderRadius: 10, fontWeight: 700, fontSize: 13, cursor: 'pointer', border: 'none',
                background: billing === opt.key ? '#2563EB' : 'transparent',
                color: billing === opt.key ? '#fff' : '#64748B',
                transition: 'all 0.2s',
              }}>{opt.label}</button>
            ))}
          </div>
        </div>
      </section>

      {/* PLANS */}
      <section style={{ padding: '60px 24px 80px' }}>
        <div style={{ maxWidth: 1100, margin: '0 auto' }}>
          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: 24 }} className="plans-grid">
            {PRICING_PLANS.map(plan => (
              <div key={plan.name} style={{
                background: '#fff', borderRadius: 20, overflow: 'visible',
                border: plan.popular ? `2px solid ${plan.color}` : '1px solid #E2E8F0',
                boxShadow: plan.popular ? '0 8px 40px rgba(37,99,235,0.15)' : 'none',
                position: 'relative', display: 'flex', flexDirection: 'column',
              }}>
                {plan.popular && (
                  <div style={{ position: 'absolute', top: -14, left: '50%', transform: 'translateX(-50%)', background: plan.color, color: '#fff', borderRadius: 100, padding: '5px 20px', fontWeight: 800, fontSize: 13, whiteSpace: 'nowrap' }}>⭐ Phổ biến nhất</div>
                )}
                {discountLabel && (
                  <div style={{ position: 'absolute', top: -14, right: 16, background: '#2563EB', color: '#fff', borderRadius: 100, padding: '4px 12px', fontWeight: 700, fontSize: 12 }}>{discountLabel}</div>
                )}

                <div style={{ background: plan.bg, padding: '28px 28px 24px', borderRadius: plan.popular ? '18px 18px 0 0' : '18px 18px 0 0' }}>
                  <h2 style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 900, fontSize: 22, margin: '0 0 4px', color: plan.color }}>{plan.name}</h2>
                  <p style={{ color: '#64748B', fontSize: 14, margin: '0 0 20px' }}>{plan.subtitle}</p>
                  <div style={{ display: 'flex', alignItems: 'baseline', gap: 6 }}>
                    <span style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 900, fontSize: 36, color: '#1E293B' }}>{fmt(Math.round(plan.price * mult))}</span>
                    <span style={{ color: '#94A3B8', fontSize: 14 }}>/tháng</span>
                  </div>
                  {billing !== 'month' && (
                    <div style={{ color: '#94A3B8', fontSize: 13, textDecoration: 'line-through', marginTop: 2 }}>{fmt(plan.price)}/tháng</div>
                  )}
                </div>

                <div style={{ padding: '24px 28px', flex: 1 }}>
                  <ul style={{ listStyle: 'none', padding: 0, margin: 0, display: 'flex', flexDirection: 'column', gap: 12 }}>
                    {plan.features.map(f => (
                      <li key={f.text} style={{ display: 'flex', alignItems: 'flex-start', gap: 10, fontSize: 14, color: f.ok ? '#1E293B' : '#CBD5E1' }}>
                        <span style={{ fontSize: 16, flexShrink: 0, color: f.ok ? '#2563EB' : '#E2E8F0', lineHeight: 1.4 }}>{f.ok ? '✓' : '✕'}</span>
                        {f.text}
                      </li>
                    ))}
                  </ul>
                </div>

                <div style={{ padding: '0 28px 28px' }}>
                  <Link to="/khoa-hoc" style={{
                    display: 'block', textAlign: 'center',
                    background: plan.popular ? plan.color : '#F8FAFC',
                    color: plan.popular ? '#fff' : plan.color,
                    border: plan.popular ? 'none' : `2px solid ${plan.border}`,
                    padding: '13px', borderRadius: 12, textDecoration: 'none', fontWeight: 800, fontSize: 15,
                    transition: 'opacity 0.2s',
                  }}
                    onMouseEnter={e => (e.currentTarget.style.opacity = '0.88')}
                    onMouseLeave={e => (e.currentTarget.style.opacity = '1')}
                  >Đăng ký {plan.name}</Link>
                  <p style={{ textAlign: 'center', color: '#94A3B8', fontSize: 12, margin: '10px 0 0' }}>Miễn phí tư vấn · Không bắt buộc</p>
                </div>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* COURSE FEES TABLE */}
      <section style={{ padding: '0 24px 80px' }}>
        <div style={{ maxWidth: 1100, margin: '0 auto' }}>
          <h2 style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 900, fontSize: 28, margin: '0 0 8px' }}>Học phí theo từng khóa</h2>
          <p style={{ color: '#64748B', fontSize: 16, margin: '0 0 32px' }}>Click vào khóa học để xem chi tiết học phí các cấp độ.</p>

          <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
            {COURSES.map(c => (
              <div key={c.tag} style={{ background: '#fff', borderRadius: 14, border: '1px solid #E2E8F0', overflow: 'hidden' }}>
                <button onClick={() => setOpenCourse(openCourse === c.tag ? null : c.tag)} style={{
                  width: '100%', padding: '20px 24px', background: 'none', border: 'none', cursor: 'pointer',
                  display: 'flex', alignItems: 'center', gap: 16, textAlign: 'left',
                }}>
                  <div style={{ width: 44, height: 44, borderRadius: 12, background: c.bg, display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 22, flexShrink: 0 }}>{c.icon}</div>
                  <div style={{ flex: 1 }}>
                    <div style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 17 }}>{c.title}</div>
                    <div style={{ color: '#64748B', fontSize: 13 }}>{c.levels.length} cấp độ · {c.students} học viên</div>
                  </div>
                  <div style={{ color: '#94A3B8', fontSize: 20, transform: openCourse === c.tag ? 'rotate(180deg)' : 'none', transition: 'transform 0.2s' }}>⌄</div>
                </button>

                {openCourse === c.tag && (
                  <div style={{ borderTop: '1px solid #E2E8F0', padding: '16px 24px 20px' }}>
                    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: 16 }}>
                      {c.levels.map(lv => (
                        <div key={lv.name} style={{ background: c.bg, borderRadius: 12, padding: '16px 18px' }}>
                          <div style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 15, color: c.color, marginBottom: 6 }}>{lv.name}</div>
                          <div style={{ color: '#64748B', fontSize: 13, marginBottom: 10 }}>{lv.target}</div>
                          <div style={{ fontSize: 13, color: '#374151', display: 'flex', flexDirection: 'column', gap: 4 }}>
                            <span>⏱ {lv.duration}</span>
                            <span>📆 {lv.sessions}</span>
                            <span style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 16, color: c.color, marginTop: 6 }}>{fmt(Math.round(lv.fee * mult))}<span style={{ fontWeight: 400, fontSize: 12, color: '#94A3B8' }}>/tháng</span></span>
                          </div>
                        </div>
                      ))}
                    </div>
                  </div>
                )}
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* FAQ */}
      <section style={{ padding: '60px 24px 80px', background: '#F8FAFC' }}>
        <div style={{ maxWidth: 760, margin: '0 auto' }}>
          <h2 style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 900, fontSize: 32, margin: '0 0 8px', textAlign: 'center' }}>Câu hỏi thường gặp</h2>
          <p style={{ color: '#64748B', textAlign: 'center', margin: '0 0 40px' }}>Không tìm thấy câu trả lời? Gọi hotline <strong style={{ color: '#2563EB' }}>1800 6868</strong> (miễn phí)</p>

          <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
            {FAQS.map((faq, i) => (
              <div key={i} style={{ background: '#fff', borderRadius: 14, border: '1px solid #E2E8F0', overflow: 'hidden' }}>
                <button onClick={() => setOpenFaq(openFaq === i ? null : i)} style={{
                  width: '100%', padding: '18px 22px', background: 'none', border: 'none', cursor: 'pointer',
                  display: 'flex', alignItems: 'center', justifyContent: 'space-between', textAlign: 'left', gap: 12,
                }}>
                  <span style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 700, fontSize: 16 }}>{faq.q}</span>
                  <span style={{ color: '#2563EB', fontSize: 20, flexShrink: 0, transform: openFaq === i ? 'rotate(180deg)' : 'none', transition: 'transform 0.2s' }}>⌄</span>
                </button>
                {openFaq === i && (
                  <div style={{ padding: '0 22px 18px', color: '#64748B', fontSize: 15, lineHeight: 1.7 }}>{faq.a}</div>
                )}
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA */}
      <section style={{ padding: '60px 24px', background: 'linear-gradient(135deg, #2563EB, #1D4ED8)' }}>
        <div style={{ maxWidth: 700, margin: '0 auto', textAlign: 'center' }}>
          <h2 style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 900, fontSize: 32, color: '#fff', margin: '0 0 12px' }}>Bắt đầu học thử miễn phí</h2>
          <p style={{ color: '#BFDBFE', fontSize: 17, margin: '0 0 28px' }}>Buổi đầu tiên miễn phí · Không cần thẻ tín dụng · Hủy bất kỳ lúc nào</p>
          <div style={{ display: 'flex', gap: 16, justifyContent: 'center', flexWrap: 'wrap' }}>
            <Link to="/khoa-hoc" style={{ background: '#fff', color: '#2563EB', padding: '14px 28px', borderRadius: 12, textDecoration: 'none', fontWeight: 800, fontSize: 15 }}>Chọn khóa học →</Link>
            <a href="tel:18006868" style={{ background: 'rgba(255,255,255,0.15)', color: '#fff', padding: '14px 28px', borderRadius: 12, textDecoration: 'none', fontWeight: 700, fontSize: 15, border: '1.5px solid rgba(255,255,255,0.3)' }}>📞 1800 6868</a>
          </div>
        </div>
      </section>

      <style>{`
        @media (max-width: 900px) {
          .plans-grid { grid-template-columns: 1fr !important; }
        }
      `}</style>
    </>
  )
}
