import { useState, useRef, useEffect } from 'react'

type Message = { from: 'bot' | 'user'; text: string; options?: string[] }

const BOT_FLOW: Record<string, { text: string; options?: string[]; next?: Record<string, string> }> = {
  start: {
    text: 'Xin chào! 👋 Mình là trợ lý tư vấn của HUIT LingoHub. Bạn đang học tiếng Anh với mục tiêu gì?',
    options: ['Thi IELTS', 'Thi TOEIC', 'Giao tiếp hàng ngày', 'Cho con em học'],
    next: { 'Thi IELTS': 'ielts_level', 'Thi TOEIC': 'toeic_level', 'Giao tiếp hàng ngày': 'comm_level', 'Cho con em học': 'kids' },
  },
  ielts_level: {
    text: 'Tuyệt vời! 🎯 Điểm IELTS mục tiêu của bạn là bao nhiêu?',
    options: ['5.0 – 6.0', '6.0 – 7.0', '7.0 – 8.0+'],
    next: { '5.0 – 6.0': 'rec_ielts_foundation', '6.0 – 7.0': 'rec_ielts_inter', '7.0 – 8.0+': 'rec_ielts_adv' },
  },
  rec_ielts_foundation: {
    text: '✅ Mình đề xuất khoá **IELTS Foundation** (3 tháng). Lớp nhỏ 12 người, giáo viên bản ngữ, cam kết đạt 5.5+. Bạn muốn đăng ký tư vấn miễn phí không?',
    options: ['Đăng ký ngay', 'Xem thêm thông tin'],
    next: { 'Đăng ký ngay': 'register', 'Xem thêm thông tin': 'more_info' },
  },
  rec_ielts_inter: {
    text: '✅ Mình đề xuất khoá **IELTS Intermediate** (4 tháng). Tập trung Writing & Speaking band 6-7, luyện đề thật. Bạn muốn đăng ký tư vấn miễn phí không?',
    options: ['Đăng ký ngay', 'Xem thêm thông tin'],
    next: { 'Đăng ký ngay': 'register', 'Xem thêm thông tin': 'more_info' },
  },
  rec_ielts_adv: {
    text: '✅ Mình đề xuất khoá **IELTS Advanced** (6 tháng). Lớp chuyên sâu band 7+, giáo viên từng đạt 8.5. Bạn muốn đăng ký tư vấn miễn phí không?',
    options: ['Đăng ký ngay', 'Xem thêm thông tin'],
    next: { 'Đăng ký ngay': 'register', 'Xem thêm thông tin': 'more_info' },
  },
  toeic_level: {
    text: 'Bạn hiện đang ở trình độ nào rồi?',
    options: ['Chưa biết gì', 'Dưới 500 điểm', 'Từ 500 – 700 điểm'],
    next: { 'Chưa biết gì': 'rec_toeic_basic', 'Dưới 500 điểm': 'rec_toeic_inter', 'Từ 500 – 700 điểm': 'rec_toeic_adv' },
  },
  rec_toeic_basic: {
    text: '✅ Khoá **TOEIC Sơ cấp** (2 tháng) sẽ phù hợp nhất. Mục tiêu 500+ điểm từ đầu. Bạn muốn đăng ký tư vấn không?',
    options: ['Đăng ký ngay', 'Xem thêm thông tin'],
    next: { 'Đăng ký ngay': 'register', 'Xem thêm thông tin': 'more_info' },
  },
  rec_toeic_inter: {
    text: '✅ Khoá **TOEIC Trung cấp** (3 tháng) — từ dưới 500 lên 700+. Chiến thuật làm bài + luyện đề thật. Bạn muốn đăng ký tư vấn không?',
    options: ['Đăng ký ngay', 'Xem thêm thông tin'],
    next: { 'Đăng ký ngay': 'register', 'Xem thêm thông tin': 'more_info' },
  },
  rec_toeic_adv: {
    text: '✅ Khoá **TOEIC Cao cấp** (2 tháng) — tập trung chinh phục 850–990 điểm. Bạn muốn đăng ký tư vấn không?',
    options: ['Đăng ký ngay', 'Xem thêm thông tin'],
    next: { 'Đăng ký ngay': 'register', 'Xem thêm thông tin': 'more_info' },
  },
  comm_level: {
    text: 'Mình muốn giao tiếp trong hoàn cảnh nào?',
    options: ['Công việc / Business', 'Du lịch & cuộc sống', 'Cả hai'],
    next: { 'Công việc / Business': 'rec_business', 'Du lịch & cuộc sống': 'rec_daily', 'Cả hai': 'rec_daily' },
  },
  rec_business: {
    text: '✅ Khoá **Business English** (4 tháng) — email, meeting, thuyết trình chuyên nghiệp. Tự tin làm việc với đối tác nước ngoài. Đăng ký tư vấn nhé?',
    options: ['Đăng ký ngay', 'Xem thêm thông tin'],
    next: { 'Đăng ký ngay': 'register', 'Xem thêm thông tin': 'more_info' },
  },
  rec_daily: {
    text: '✅ Khoá **Tiếng Anh Giao Tiếp** (3 tháng) — phát âm chuẩn, phản xạ nhanh, tự nhiên. 30% giờ học là thực hành với native speaker. Đăng ký tư vấn nhé?',
    options: ['Đăng ký ngay', 'Xem thêm thông tin'],
    next: { 'Đăng ký ngay': 'register', 'Xem thêm thông tin': 'more_info' },
  },
  kids: {
    text: 'Con em bao nhiêu tuổi?',
    options: ['5 – 7 tuổi', '8 – 10 tuổi', '11 – 12 tuổi'],
    next: { '5 – 7 tuổi': 'rec_kids_starters', '8 – 10 tuổi': 'rec_kids_movers', '11 – 12 tuổi': 'rec_kids_flyers' },
  },
  rec_kids_starters: {
    text: '✅ Khoá **Starters** (6 tháng) cho bé 5–7 tuổi. Học qua trò chơi, nhạc và hình ảnh sinh động. Đăng ký cho bé không?',
    options: ['Đăng ký ngay', 'Xem thêm thông tin'],
    next: { 'Đăng ký ngay': 'register', 'Xem thêm thông tin': 'more_info' },
  },
  rec_kids_movers: {
    text: '✅ Khoá **Movers** (8 tháng) — bé 8–10 tuổi học đọc viết và giao tiếp cơ bản. Chuẩn bị cho kỳ thi Cambridge YLE. Đăng ký không?',
    options: ['Đăng ký ngay', 'Xem thêm thông tin'],
    next: { 'Đăng ký ngay': 'register', 'Xem thêm thông tin': 'more_info' },
  },
  rec_kids_flyers: {
    text: '✅ Khoá **Flyers** (6 tháng) — bé 11–12 tuổi, hướng đến chứng chỉ Cambridge Flyers quốc tế. Đăng ký không?',
    options: ['Đăng ký ngay', 'Xem thêm thông tin'],
    next: { 'Đăng ký ngay': 'register', 'Xem thêm thông tin': 'more_info' },
  },
  more_info: {
    text: 'Bạn có thể xem chi tiết tại mục **Khóa học** hoặc để lại số điện thoại — đội tư vấn sẽ gọi lại trong 30 phút! 📞',
    options: ['Để lại số điện thoại', 'Bắt đầu lại'],
    next: { 'Để lại số điện thoại': 'register', 'Bắt đầu lại': 'start' },
  },
  register: {
    text: '🎉 Tuyệt vời! Vui lòng điền form hoặc gọi hotline **1800 6868** (miễn phí). Mình có thể giúp gì thêm không?',
    options: ['Hỏi về học phí', 'Bắt đầu lại'],
    next: { 'Hỏi về học phí': 'pricing', 'Bắt đầu lại': 'start' },
  },
  pricing: {
    text: '💰 Học phí từ **2,500,000đ/tháng**. Trả 3 tháng giảm 10%, 6 tháng giảm 15%. Có học bổng cho học viên xuất sắc. Bạn muốn biết thêm?',
    options: ['Đăng ký tư vấn', 'Bắt đầu lại'],
    next: { 'Đăng ký tư vấn': 'register', 'Bắt đầu lại': 'start' },
  },
}

export default function ChatBot() {
  const [open, setOpen] = useState(false)
  const [messages, setMessages] = useState<Message[]>([])
  const [step, setStep] = useState('start')
  const [inputVal, setInputVal] = useState('')
  const [showInput, setShowInput] = useState(false)
  const [unread, setUnread] = useState(true)
  const bottomRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    if (open && messages.length === 0) {
      setTimeout(() => {
        const s = BOT_FLOW['start']
        setMessages([{ from: 'bot', text: s.text, options: s.options }])
      }, 400)
    }
    if (open) setUnread(false)
  }, [open])

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' })
  }, [messages])

  const handleOption = (option: string) => {
    const current = BOT_FLOW[step]
    const nextKey = current.next?.[option] ?? 'start'
    setMessages(prev => [...prev, { from: 'user', text: option }])
    setStep(nextKey)
    setTimeout(() => {
      const next = BOT_FLOW[nextKey]
      setMessages(prev => [...prev, { from: 'bot', text: next.text, options: next.options }])
    }, 600)
  }

  const handleSend = () => {
    if (!inputVal.trim()) return
    setMessages(prev => [...prev, { from: 'user', text: inputVal }])
    setInputVal('')
    setShowInput(false)
    setTimeout(() => {
      setMessages(prev => [...prev, {
        from: 'bot',
        text: 'Cảm ơn bạn! Đội tư vấn sẽ liên hệ trong vòng 30 phút. Bạn cần hỗ trợ thêm gì không?',
        options: ['Bắt đầu lại', 'Hỏi về học phí'],
      }])
      setStep('start')
    }, 700)
  }

  const renderText = (text: string) =>
    text.split(/\*\*(.*?)\*\*/g).map((p, i) => i % 2 === 1 ? <strong key={i}>{p}</strong> : p)

  const lastMsg = messages[messages.length - 1]
  const showOptions = lastMsg?.from === 'bot' && lastMsg.options && !showInput

  return (
    <>
      <button onClick={() => setOpen(o => !o)} style={{
        position: 'fixed', bottom: 28, right: 28, zIndex: 1000,
        width: 58, height: 58, borderRadius: '50%',
        background: open ? '#1D4ED8' : '#2563EB',
        border: 'none', cursor: 'pointer',
        boxShadow: '0 4px 24px rgba(37,99,235,0.45)',
        display: 'flex', alignItems: 'center', justifyContent: 'center',
        fontSize: 24, transition: 'background 0.2s, transform 0.2s',
        transform: open ? 'rotate(15deg) scale(1.05)' : 'scale(1)',
      }} aria-label="Mở chat tư vấn">
        {open ? '✕' : '💬'}
        {!open && unread && (
          <span style={{
            position: 'absolute', top: 2, right: 2,
            width: 14, height: 14, borderRadius: '50%',
            background: '#fff', border: '2px solid #2563EB',
          }} />
        )}
      </button>

      {open && (
        <div style={{
          position: 'fixed', bottom: 100, right: 28, zIndex: 999,
          width: 360, maxWidth: 'calc(100vw - 40px)',
          background: '#fff', borderRadius: 20,
          boxShadow: '0 8px 48px rgba(0,0,0,0.18)',
          display: 'flex', flexDirection: 'column', overflow: 'hidden',
          animation: 'slideUp 0.25s ease',
        }}>
          <div style={{ background: '#2563EB', padding: '16px 20px', display: 'flex', alignItems: 'center', gap: 12 }}>
            <div style={{ width: 42, height: 42, borderRadius: '50%', background: 'rgba(255,255,255,0.2)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 22 }}>🤖</div>
            <div>
              <div style={{ color: '#fff', fontFamily: "'Nunito', sans-serif", fontWeight: 800, fontSize: 15 }}>HUIT LingoBot</div>
              <div style={{ display: 'flex', alignItems: 'center', gap: 6 }}>
                <span style={{ width: 7, height: 7, borderRadius: '50%', background: '#fff', display: 'inline-block' }} />
                <span style={{ color: 'rgba(255,255,255,0.8)', fontSize: 12 }}>Trực tuyến — phản hồi ngay</span>
              </div>
            </div>
            <button onClick={() => setOpen(false)} style={{ marginLeft: 'auto', background: 'rgba(255,255,255,0.15)', border: 'none', color: '#fff', cursor: 'pointer', width: 30, height: 30, borderRadius: '50%', fontSize: 14, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>✕</button>
          </div>

          <div style={{ flex: 1, overflowY: 'auto', padding: '16px 16px 8px', display: 'flex', flexDirection: 'column', gap: 10, maxHeight: 340, minHeight: 200, background: '#F8FAFC' }}>
            {messages.map((msg, i) => (
              <div key={i} style={{ display: 'flex', justifyContent: msg.from === 'user' ? 'flex-end' : 'flex-start', alignItems: 'flex-end', gap: 8 }}>
                {msg.from === 'bot' && (
                  <div style={{ width: 28, height: 28, borderRadius: '50%', background: '#EFF6FF', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: 14, flexShrink: 0 }}>🤖</div>
                )}
                <div style={{
                  maxWidth: '78%', padding: '10px 14px',
                  borderRadius: msg.from === 'bot' ? '16px 16px 16px 4px' : '16px 16px 4px 16px',
                  background: msg.from === 'bot' ? '#fff' : '#2563EB',
                  color: msg.from === 'bot' ? '#1E293B' : '#fff',
                  fontSize: 14, lineHeight: 1.6,
                  boxShadow: msg.from === 'bot' ? '0 1px 4px rgba(0,0,0,0.07)' : 'none',
                  border: msg.from === 'bot' ? '1px solid #E2E8F0' : 'none',
                }}>{renderText(msg.text)}</div>
              </div>
            ))}
            <div ref={bottomRef} />
          </div>

          {showOptions && (
            <div style={{ padding: '8px 14px 4px', display: 'flex', flexWrap: 'wrap', gap: 8, background: '#F8FAFC' }}>
              {lastMsg.options!.map(opt => (
                <button key={opt} onClick={() => handleOption(opt)} style={{
                  padding: '7px 14px', borderRadius: 100,
                  background: '#EFF6FF', color: '#2563EB',
                  border: '1.5px solid #BFDBFE', cursor: 'pointer',
                  fontSize: 13, fontWeight: 600, transition: 'all 0.15s',
                }}
                  onMouseEnter={e => { e.currentTarget.style.background = '#2563EB'; e.currentTarget.style.color = '#fff' }}
                  onMouseLeave={e => { e.currentTarget.style.background = '#EFF6FF'; e.currentTarget.style.color = '#2563EB' }}
                >{opt}</button>
              ))}
              <button onClick={() => setShowInput(true)} style={{ padding: '7px 14px', borderRadius: 100, background: 'transparent', color: '#94A3B8', border: '1.5px solid #E2E8F0', cursor: 'pointer', fontSize: 13 }}>Tự nhập...</button>
            </div>
          )}

          {showInput && (
            <div style={{ padding: '10px 14px', display: 'flex', gap: 8, background: '#F8FAFC', borderTop: '1px solid #E2E8F0' }}>
              <input autoFocus value={inputVal} onChange={e => setInputVal(e.target.value)} onKeyDown={e => e.key === 'Enter' && handleSend()}
                placeholder="Nhập số điện thoại hoặc câu hỏi..."
                style={{ flex: 1, padding: '9px 12px', borderRadius: 10, border: '1.5px solid #BFDBFE', fontSize: 14, outline: 'none', background: '#fff' }} />
              <button onClick={handleSend} style={{ background: '#2563EB', color: '#fff', border: 'none', borderRadius: 10, padding: '0 14px', cursor: 'pointer', fontSize: 16, fontWeight: 700 }}>→</button>
            </div>
          )}

          <div style={{ padding: '8px 16px 12px', textAlign: 'center', background: '#fff', borderTop: '1px solid #F1F5F9' }}>
            <span style={{ fontSize: 11, color: '#CBD5E1' }}>HUIT LingoHub · Tư vấn 24/7 · 1800 6868</span>
          </div>
        </div>
      )}

      <style>{`@keyframes slideUp { from { opacity:0; transform:translateY(16px); } to { opacity:1; transform:translateY(0); } }`}</style>
    </>
  )
}
