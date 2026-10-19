import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'
import tailwindcss from '@tailwindcss/vite'
import path from 'node:path'

export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      '@': path.resolve(import.meta.dirname, './src'),
    },
  },
  server: {
    port: 5173,
    open: true,
    // Chuyển tiếp /api sang ASP.NET Core => same-origin, không cần CORS.
    proxy: {
      '/api': {
        target: 'http://localhost:5204',
        changeOrigin: true,
      },
    },
  },
})