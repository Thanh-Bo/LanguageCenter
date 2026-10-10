import React from 'react'
import ReactDOM from 'react-dom/client'
import App from './App'
import { NhaCungCapXacThuc } from './contexts/NguCanhXacThuc'
import './index.css'

ReactDOM.createRoot(document.getElementById('root')!).render(
  <React.StrictMode>
    <NhaCungCapXacThuc>
      <App />
    </NhaCungCapXacThuc>
  </React.StrictMode>,
)