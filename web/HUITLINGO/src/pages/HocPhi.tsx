import { useState } from 'react'
import { Link } from 'react-router'
import { CHINH_SACH_HOC_PHI, CHINH_SACH_THANH_TOAN, COURSES } from '../duLieu'
import { Icon } from '../components/BieuTuong'

function fmt(n: number) {
  return n.toLocaleString('vi-VN') + 'đ'
}

const FAQS = [
  { q: 'Học phí đã bao gồm tài liệu học chưa?', a: 'Đã bao gồm tài liệu học online và giáo trình in. Học viên không phải đóng thêm phụ phí nào ngoài học phí niêm yết.' },
  { q: 'Học phí tính theo tháng hay theo khóa?', a: 'Học phí được niêm yết theo khóa — trọn gói số buổi của cấp độ. Ví dụ IELTS Foundation: 6.000.000đ cho 24 buổi (48 giờ) trong 12 tuần. Không đóng thêm theo tháng.' },
  { q: 'Có thể đóng nhiều đợt không?', a: 'Có. Tối đa 2 đợt: đợt 1 tối thiểu 50% khi nhập học, đợt 2 trước khi khóa kết thúc 2 tuần. Học viên chỉ được chuyển sang trạng thái "Đang học" khi đã hoàn tất đợt 1.' },
  { q: 'Nếu không đạt mục tiêu thì sao?', a: 'Học viên được học lại miễn phí nếu đáp ứng đủ điều kiện: chuyên cần từ 80%, làm đủ bài tập và không nợ học phí. Chính sách cụ thể được ghi rõ trong hợp đồng học.' },
  { q: 'Có ưu đãi gì không?', a: 'Có: giảm 5% khi đóng đủ trước khai giảng, giảm 10% cho anh/chị/em ruột cùng học, giảm 10% khi đăng ký từ 2 khóa trở lên. Các ưu đãi không cộng dồn.' },
  { q: 'Có thể chuyển lớp hoặc bảo lưu không?', a: 'Chuyển lớp 1 lần miễn phí mỗi khóa (lần 2 thu 10% học phí). Bảo lưu tối đa 4 tuần, không quá 1 lần mỗi chương trình. Vắng quá 30% số buổi sẽ không được công nhận hoàn thành.' },
  { q: 'Lớp học có bao nhiêu học viên?', a: 'Mầm non tối đa 10 · Tiểu học 12 · THCS 15 · Giao tiếp 12 · IELTS 14. Lớp Mầm non luôn có 1 giáo viên chính và 1 trợ giảng.' },
]

export default function Pricing() {
  const [openFaq, setOpenFaq] = useState<number | null>(null)
  const [openCourse, setOpenCourse] = useState<string | null>(null)

  const tongCapDo = COURSES.reduce((n, c) => n + c.levels.length, 0)
  const giaThapNhat = Math.min(...COURSES.flatMap(c => c.levels.map(l => l.fee)))

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
            Học phí niêm yết theo khóa, minh bạch — không phí ẩn. Cùng một bảng giá được dùng cho cả website và hệ thống quản lý của trung tâm.
          </p>

          <div style={{ display: 'flex', gap: 12, justifyContent: 'center', flexWrap: 'wrap' }}>
            {[
              `${COURSES.length} chương trình`,
              `${tongCapDo} cấp độ`,
              `Từ ${fmt(giaThapNhat)}/khóa`,
              'Đóng tối đa 2 đợt',
            ].map(t => (
              <div key={t} style={{ background: '#fff', borderRadius: 12, padding: '12px 22px', fontWeight: 700, fontSize: 14, color: '#1E40AF', boxShadow: '0 2px 12px rgba(0,0,0,0.06)' }}>{t}</div>
            ))}
          </div>
        </div>
      </section>

      {/* CHÍNH SÁCH ƯU ĐÃI */}
      <section style={{ padding: '60px 24px 20px' }}>
        <div style={{ maxWidth: 1100, margin: '0 auto' }}>
          <h2 style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 900, fontSize: 28, margin: '0 0 8px' }}>Chính sách ưu đãi</h2>
          <p style={{ color: '#64748B', fontSize: 16, margin: '0 0 32px' }}>Các ưu đãi không cộng dồn — hệ thống tự chọn mức giảm cao nhất cho học viên.</p>

          <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: 24 }} className="plans-grid">
            {CHINH_SACH_HOC_PHI.map(plan => (
              <div key={plan.tieuDe} style={{ background: '#fff', borderRadius: 20, border: '1px solid #E2E8F0', padding: '28px' }}>
                <div style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 900, fontSize: 28, color: '#2563EB', marginBottom: 8 }}>{plan.giaTri}</div>
                <h3 style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 18, margin: '0 0 10px' }}>{plan.tieuDe}</h3>
                <p style={{ color: '#64748B', fontSize: 15, lineHeight: 1.7, margin: 0 }}>{plan.moTa}</p>
              </div>
            ))}
          </div>

          <div style={{ background: '#EFF6FF', borderRadius: 16, padding: '24px 28px', marginTop: 32 }}>
            <h3 style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 17, margin: '0 0 14px', color: '#1E40AF' }}>Quy định thanh toán &amp; hoàn tiền</h3>
            <ul style={{ margin: 0, paddingLeft: 20, color: '#334155', fontSize: 15, lineHeight: 1.9 }}>
              {CHINH_SACH_THANH_TOAN.map(d => <li key={d}>{d}</li>)}
            </ul>
          </div>
        </div>
      </section>

      {/* COURSE FEES TABLE */}
      <section style={{ padding: '0 24px 80px' }}>
        <div style={{ maxWidth: 1100, margin: '0 auto' }}>
          <h2 style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 900, fontSize: 28, margin: '0 0 8px' }}>Học phí theo từng chương trình</h2>
          <p style={{ color: '#64748B', fontSize: 16, margin: '0 0 32px' }}>Click vào chương trình để xem học phí, số buổi và thời lượng từng cấp độ.</p>

          <div style={{ display: 'flex', flexDirection: 'column', gap: 12 }}>
            {COURSES.map(c => (
              <div key={c.tag} style={{ background: '#fff', borderRadius: 14, border: '1px solid #E2E8F0', overflow: 'hidden' }}>
                <button onClick={() => setOpenCourse(openCourse === c.tag ? null : c.tag)} style={{
                  width: '100%', padding: '20px 24px', background: 'none', border: 'none', cursor: 'pointer',
                  display: 'flex', alignItems: 'center', gap: 16, textAlign: 'left',
                }}>
                  <div style={{ width: 44, height: 44, borderRadius: 12, background: c.bg, display: 'flex', alignItems: 'center', justifyContent: 'center', flexShrink: 0, color: c.color }}>
                    <Icon ten={c.icon} size={22} />
                  </div>
                  <div style={{ flex: 1 }}>
                    <div style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 17 }}>{c.title}</div>
                    <div style={{ color: '#64748B', fontSize: 13 }}>{c.levels.length} cấp độ · {c.students} học viên</div>
                  </div>
                  <div style={{ display: 'flex', color: '#94A3B8', transform: openCourse === c.tag ? 'rotate(180deg)' : 'none', transition: 'transform 0.2s' }}>
                    <Icon ten="ChevronDown" size={20} />
                  </div>
                </button>

                {openCourse === c.tag && (
                  <div style={{ borderTop: '1px solid #E2E8F0', padding: '16px 24px 20px' }}>
                    <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(200px, 1fr))', gap: 16 }}>
                      {c.levels.map(lv => (
                        <div key={lv.name} style={{ background: c.bg, borderRadius: 12, padding: '16px 18px' }}>
                          <div style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 15, color: c.color, marginBottom: 6 }}>{lv.name}</div>
                          <div style={{ color: '#64748B', fontSize: 13, marginBottom: 10 }}>{lv.target}</div>
                          <div style={{ fontSize: 13, color: '#374151', display: 'flex', flexDirection: 'column', gap: 4 }}>
                            <span style={{ display: 'flex', alignItems: 'center', gap: 6 }}><Icon ten="Clock" size={13} /> {lv.duration}</span>
                            <span style={{ display: 'flex', alignItems: 'center', gap: 6 }}><Icon ten="CalendarRange" size={13} /> {lv.sessions}</span>
                            <span style={{ display: 'flex', alignItems: 'center', gap: 6 }}><Icon ten="Timer" size={13} /> {lv.buoi} buổi · {lv.gio} giờ</span>
                            <span style={{ fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 16, color: c.color, marginTop: 6 }}>{fmt(lv.fee)}<span style={{ fontWeight: 400, fontSize: 12, color: '#94A3B8' }}>/khóa</span></span>
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
                  <span style={{ display: 'flex', color: '#2563EB', flexShrink: 0, transform: openFaq === i ? 'rotate(180deg)' : 'none', transition: 'transform 0.2s' }}>
                    <Icon ten="ChevronDown" size={20} />
                  </span>
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
            <Link to="/khoahoc" style={{ display: 'inline-flex', alignItems: 'center', gap: 6, background: '#fff', color: '#2563EB', padding: '14px 28px', borderRadius: 12, textDecoration: 'none', fontWeight: 800, fontSize: 15 }}>
              Chọn chương trình <Icon ten="ArrowRight" size={16} />
            </Link>
            <a href="tel:18006868" style={{ display: 'inline-flex', alignItems: 'center', gap: 8, background: 'rgba(255,255,255,0.15)', color: '#fff', padding: '14px 28px', borderRadius: 12, textDecoration: 'none', fontWeight: 700, fontSize: 15, border: '1.5px solid rgba(255,255,255,0.3)' }}>
              <Icon ten="Phone" size={16} /> 1800 6868
            </a>
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
